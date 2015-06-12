Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Content
Imports Ektron.ASM.AssetConfig
Imports System.Text
Imports System.Collections.Generic

Partial Class Workarea_DmsMetadata
    Inherits System.Web.UI.Page

    Protected TaxonomyRoleExists As Boolean = False
    Protected m_ContentApi As New Ektron.Cms.ContentAPI
    Protected m_refMsg As Common.EkMessageHelper = m_ContentApi.EkMsgRef
    Protected m_idString As String = ""
    Protected m_refStyle As New StyleHelper
    Protected AppImgPath As String = ""
    Protected m_refContentApi As New ContentAPI
    Protected m_folderId As String = ""
    Protected page_meta_data As Collection
    Protected m_intTaxFolderId As String = "0" ' note: this is used by an include file!


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim contentId As Long
        If m_refContentApi.RequestInformationRef.IsMembershipUser OrElse m_refContentApi.RequestInformationRef.UserId = 0 Then
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), False)
            Exit Sub
        End If
        AppImgPath = m_refContentApi.AppImgPath
        ltrStyleSheetJs.Text = (New StyleHelper).GetClientScript
        EnhancedMetadataScript.Text = CustomFields.GetEnhancedMetadataScript()
        EnhancedMetadataArea.Text = CustomFields.GetEnhancedMetadataArea()
        RegisterResources()
        'Capture ContentId and FolderId from Querystring
        If Request.QueryString("idString") = String.Empty Then
            m_idString = ""
        Else
            m_idString = Request.QueryString("idString")
        End If

        If Request.QueryString("folderId") = String.Empty Then
            m_folderId = -1
        Else
            m_folderId = Request.QueryString("folderId")
            ltrTaxFolderId.Text = m_folderId
            m_intTaxFolderId = m_folderId
        End If

        ltrActionPage.Text = "DMSMetadata.aspx?idString=" & Server.HtmlEncode(m_idString) & "&folderId=" & m_folderId

        If Request.QueryString("action") <> String.Empty Then
            If (Request.QueryString("action") = "Submit") Then
                PublishContent()
                If (Request.QueryString("close") = "true") Then
                    Dim uniqueKey As String = m_ContentApi.UserId & m_ContentApi.UniqueId & "RejectedFiles"
                    If (Session(uniqueKey) IsNot Nothing) AndAlso (Session(uniqueKey).ToString().Length > 0) Then
                        Dim failedUpload As String = Session(uniqueKey)
                        failedUpload = failedUpload.Replace("\", "\\")
                        failedUpload = failedUpload.Replace("'", "\'")
                        Me.jsInvalidFileTypeMsg.Text = m_refMsg.GetMessage("lbl error message for multiupload") & " " & failedUpload
                        Session.Remove(uniqueKey)
                    Else
                        Me.jsInvalidFileTypeMsg.Text = ""
                    End If
                    Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "_closeTBScript", "CloseThickBoxandReload();", True)
                ElseIf (Request.QueryString("closeWindow") = "true") Then
                    Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "_refreshTop", "top.location.href = top.location.href;", True)
                    Response.Redirect("close.aspx")
                Else
                    Response.Redirect("content.aspx?action=ViewContentByCategory&id=" & m_folderId)
                End If
            End If
        End If

        If Request.QueryString("action") <> String.Empty Then
            If (Request.QueryString("action") = "Cancel") Then
                'CheckInContent()
                If (Request.QueryString("close") = "true") Then
                    'Response.Redirect("close.aspx")
                    Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "_closeTBScript", "if(parent != null && typeof parent.ektb_remove == 'function'){parent.ektb_remove();}", True)
                ElseIf (Request.QueryString("closeWindow") = "true") Then
                    Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "_refreshTop", "top.location.href = top.location.href;", True)
                    Response.Redirect("close.aspx")
                Else
                    Response.Redirect("content.aspx?action=ViewContentByCategory&id=" & m_folderId)
                End If
            End If
        End If


        If Request.QueryString("contentId") = String.Empty Then
            contentId = -1
        Else
            Try
                contentId = Request.QueryString("contentId") ' should always be an integer unles an error is returned from the ajax page
            Catch
                contentId = -1
            End Try
        End If

        If contentId <> -1 And m_folderId <> -1 Then
            myMetadata.Text = CaptureMetadata(contentId, m_folderId).ToString()
            myTaxonomy.Text = CaptureTaxonomy(contentId, m_folderId)
        Else
            myMetadata.Text = "<p class=" & """" & "nodata" & """" & ">No Metadata Available.</p>"
            myTaxonomy.Text = "<p class=" & """" & "nodata" & """" & ">No Taxonomy Data Available.</p>"
        End If

        ToolBar()

    End Sub

    Private Function CaptureMetadata(ByVal contentId As Long, ByVal folderId As Long) As StringBuilder
        Dim metadataOutput As New StringBuilder
        Dim myContentAPI As New ContentAPI
        Dim myContentEditData As New ContentData
        Dim myContentMetadata() As ContentMetaData
        Dim myType As String = ""
        Dim myCounter As Integer = 0
        Dim myEkSite As New Ektron.Cms.Site.EkSite

        Dim ContentLanguage As Integer = CONTENT_LANGUAGES_UNDEFINED

        If (Not (Page.Request.QueryString("LangType") Is Nothing)) Then
            If (Page.Request.QueryString("LangType") <> "") Then
                ContentLanguage = Convert.ToInt32(Page.Request.QueryString("LangType"))
                myContentAPI.SetCookieValue("LastValidLanguageID", ContentLanguage)
            Else
                If myContentAPI.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(myContentAPI.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If myContentAPI.GetCookieValue("LastValidLanguageID") <> "" Then
                ContentLanguage = Convert.ToInt32(myContentAPI.GetCookieValue("LastValidLanguageID"))
            End If
        End If

        If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            myContentAPI.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            myContentAPI.ContentLanguage = ContentLanguage
        End If

        If contentId <> Nothing Then
            myEkSite = myContentAPI.EkSiteRef
            myContentEditData = myContentAPI.GetContentById(contentId)
            myContentMetadata = myContentEditData.MetaData

            If myContentMetadata.Length > 0 Then
                metadataOutput = CustomFields.WriteFilteredMetadataForEdit(myContentMetadata, False, myType, folderId, myCounter, myEkSite.GetPermissions(folderId, 0, "folder"))
                If metadataOutput.Length > 0 Then
                    ltrShowMetadata.Text = "<li><a id=""metadataAnchor"" href=""#"" onclick=""dmsMetadataShowHideCategory('metadata');return false;"" title=""View Metadata"" class=""selected"">" & myContentAPI.EkMsgRef.GetMessage("metadata text") & "</a></li>"
                End If
            End If
        End If

        Return metadataOutput
    End Function

    Private Sub SetTaxonomy(ByVal contentid As Long, ByVal ifolderid As Long)
        Dim taxonomy_cat_arr As TaxonomyBaseData() = Nothing
        Dim taxonomy_request As New TaxonomyRequest
        Dim taxonomy_data_arr As TaxonomyBaseData() = Nothing

        taxonomy_request.TaxonomyId = ifolderid
        taxonomy_request.TaxonomyLanguage = m_refContentApi.ContentLanguage
        taxonomy_data_arr = m_refContentApi.EkContentRef.GetAllFolderTaxonomy(ifolderid)
        ltrTaxonomyOverrideId.Text = 0
        If (Request.QueryString("TaxonomyOverrideId") IsNot Nothing AndAlso Request.QueryString("TaxonomyOverrideId") <> "") Then
            ltrTaxonomyOverrideId.Text = Convert.ToInt64(Request.QueryString("TaxonomyOverrideId"))
        End If
        Dim taxonomyId As Long = 0
        If (Request.QueryString("TaxonomyId") IsNot Nothing AndAlso Request.QueryString("TaxonomyId") <> "") Then
            taxonomyId = Convert.ToInt64(Request.QueryString("TaxonomyId"))
        End If
        If (taxonomyId > 0) Then

            ltrTaxonomyTreeIdList.Text = taxonomyId 'taxonomyselectedtree.Value
            If (ltrTaxonomyTreeIdList.Text.Trim.Length > 0) Then
                taxonomy_cat_arr = m_refContentApi.EkContentRef.GetTaxonomyRecursiveToParent(taxonomyId, m_refContentApi.ContentLanguage, 0)
                If (taxonomy_cat_arr IsNot Nothing AndAlso taxonomy_cat_arr.Length > 0) Then
                    For Each taxonomy_cat As TaxonomyBaseData In taxonomy_cat_arr
                        If (ltrTaxonomyTreeParentIdList.Text = "") Then
                            ltrTaxonomyTreeParentIdList.Text = Convert.ToString(taxonomy_cat.TaxonomyId)
                        Else
                            ltrTaxonomyTreeParentIdList.Text = ltrTaxonomyTreeParentIdList.Text & "," & Convert.ToString(taxonomy_cat.TaxonomyId)
                        End If
                    Next
                End If
            End If
        End If

        ltrTaxFolderId.Text = ifolderid
        m_intTaxFolderId = ifolderid
    End Sub
    Private Function CaptureTaxonomy(ByVal contentId As Long, ByVal folderId As Long) As String
        Dim taxonomyOutput As New StringBuilder

        Dim myFolderApi As New API.Folder
        Dim myFolderData As FolderData
        Dim myContentApi As New ContentAPI
        Dim myTaxonomyBaseData() As TaxonomyBaseData
        Dim myTaxonomyIds As New List(Of Long)
        Dim Js As System.Text.StringBuilder
        Js = New System.Text.StringBuilder
        Dim iTmpCaller As Long = myContentApi.RequestInformationRef.CallerId
        Dim iTmpuserID As Long = myContentApi.RequestInformationRef.UserId

        Dim ContentLanguage As Integer = CONTENT_LANGUAGES_UNDEFINED

        If (Not (Page.Request.QueryString("LangType") Is Nothing)) Then
            If (Page.Request.QueryString("LangType") <> "") Then
                ContentLanguage = Convert.ToInt32(Page.Request.QueryString("LangType"))
                myContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
            Else
                If myContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(myContentApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If myContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                ContentLanguage = Convert.ToInt32(myContentApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If

        If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            myContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            myContentApi.ContentLanguage = ContentLanguage
        End If

        myContentApi.RequestInformationRef.CallerId = Ektron.Cms.Common.EkConstants.InternalAdmin
        myContentApi.RequestInformationRef.UserId = Ektron.Cms.Common.EkConstants.InternalAdmin

        myFolderData = myContentApi.GetFolderById(folderId, True)
        'myFolderData = myFolderApi.GetFolder(folderId, True)
        myTaxonomyBaseData = myFolderData.FolderTaxonomy
        myContentApi.RequestInformationRef.CallerId = iTmpCaller
        myContentApi.RequestInformationRef.UserId = iTmpuserID

        If (Request.QueryString("TaxonomyId") IsNot Nothing AndAlso Request.QueryString("TaxonomyId") <> "") Then
            taxonomyselectedtree.Value = Convert.ToInt64(Request.QueryString("TaxonomyId"))
        End If
        SetTaxonomy(contentId, folderId)

        Js.Append("function ValidateTax(){").Append(Environment.NewLine)
        If myTaxonomyBaseData.Length > 0 And myTaxonomyBaseData IsNot Nothing Then
            If myFolderData.CategoryRequired = True Then
                Js.Append("      document.getElementById(""taxonomyselectedtree"").value="""";").Append(Environment.NewLine)
                Js.Append("      for(var i=0;i<taxonomytreearr.length;i++){").Append(Environment.NewLine)
                Js.Append("         if(document.getElementById(""taxonomyselectedtree"").value==""""){").Append(Environment.NewLine)
                Js.Append("             document.getElementById(""taxonomyselectedtree"").value=taxonomytreearr[i];").Append(Environment.NewLine)
                Js.Append("         }else{").Append(Environment.NewLine)
                Js.Append("             document.getElementById(""taxonomyselectedtree"").value=document.getElementById(""taxonomyselectedtree"").value+"",""+taxonomytreearr[i];").Append(Environment.NewLine)
                Js.Append("         }").Append(Environment.NewLine)
                Js.Append("       } ").Append(Environment.NewLine)
                Js.Append("      if (Trim(document.getElementById('taxonomyselectedtree').value) == '') { ").Append(Environment.NewLine)
                Js.Append("         alert('" & m_refMsg.GetMessage("js tax cat req") & "'); ").Append(Environment.NewLine)
                Js.Append("         return false; ").Append(Environment.NewLine)
                Js.Append("      } ").Append(Environment.NewLine)
                Js.Append("      return true; }").Append(Environment.NewLine)
            Else
                Js.Append("      return true;}").Append(Environment.NewLine)
            End If
            ltrTaxJS.Text = Js.ToString()

            ltrShowTaxonomy.Text = "<li><a id=""taxonomyAnchor"" href=""#"" onclick=""dmsMetadataShowHideCategory('taxonomy');return false;"" title=""View Taxonomy"">" & m_refMsg.GetMessage("viewtaxonomytabtitle") & "</a></li>"
            Dim addTaxonomy As String = "<div id=" & """" & "TreeOutput" & """" & "></div>"
            Return addTaxonomy
        Else
            Js.Append("      return true;}").Append(Environment.NewLine)
            ltrTaxJS.Text = Js.ToString()
            Dim addTaxonomyEmpty As String = "<div id=""EmptyTree""></div>"
            Return addTaxonomyEmpty
        End If
    End Function
    Private Function PublishContent() As Boolean
        Dim contApi As New ContentAPI
        Dim contObj As EkContent
        Dim ContentLanguage As Integer = CONTENT_LANGUAGES_UNDEFINED

        If (Not (Page.Request.QueryString("LangType") Is Nothing)) Then
            If (Page.Request.QueryString("LangType") <> "") Then
                ContentLanguage = Convert.ToInt32(Page.Request.QueryString("LangType"))
                contApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
            Else
                If contApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(contApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If contApi.GetCookieValue("LastValidLanguageID") <> "" Then
                ContentLanguage = Convert.ToInt32(contApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If

        If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            contApi.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            contApi.ContentLanguage = ContentLanguage
        End If

        Dim acMetaInfo(3), MetaSelect, MetaSeparator As Object
        Dim MetaTextString As String = ""
        Dim ValidCounter As Integer = 0
        Dim i As Integer = 0
        Dim hasMeta As Boolean = False
        If (Page.Request.Form("frm_validcounter") <> "") Then
            ValidCounter = CInt(Page.Request.Form("frm_validcounter"))
            hasMeta = True
        Else
            ValidCounter = 1
        End If

        Dim TaxonomyTreeIdList As String = ""
        TaxonomyTreeIdList = Page.Request.Form(taxonomyselectedtree.UniqueID)
        If ((Not String.IsNullOrEmpty(TaxonomyTreeIdList)) AndAlso TaxonomyTreeIdList.Trim.EndsWith(",")) Then
            ltrTaxonomyTreeIdList.Text = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1)
        End If

        contObj = contApi.EkContentRef
        Dim ids() As String
        Dim contId As String = ""
        ids = m_idString.Split(",")
        For Each contId In ids
            If (contId <> "") Then
                page_meta_data = New Collection
                If (hasMeta = True) Then
                    For i = 1 To ValidCounter
                        acMetaInfo(1) = Page.Request.Form("frm_meta_type_id_" & i)
                        acMetaInfo(2) = contId
                        MetaSeparator = Page.Request.Form("MetaSeparator_" & i)
                        MetaSelect = Page.Request.Form("MetaSelect_" & i)
                        If (MetaSelect) Then
                            MetaTextString = Replace(Page.Request.Form("frm_text_" & i), ", ", MetaSeparator)
                            If (Left(MetaTextString, 1) = MetaSeparator) Then
                                MetaTextString = Right(MetaTextString, (Len(MetaTextString) - 1))
                            End If
                            MetaTextString = CleanString(MetaTextString)
                            acMetaInfo(3) = MetaTextString
                        Else
                            MetaTextString = Replace(Page.Request.Form("frm_text_" & i), ";", MetaSeparator)
                            MetaTextString = CleanString(MetaTextString)
                            acMetaInfo(3) = MetaTextString
                        End If
                        page_meta_data.Add(acMetaInfo, CInt(i))
                        ReDim acMetaInfo(3)
                    Next
                End If
                If (Not (String.IsNullOrEmpty(TaxonomyTreeIdList))) Then

                    Dim request As New TaxonomyContentRequest

                    request.ContentId = CLng(contId)
                    request.TaxonomyList = TaxonomyTreeIdList
                    request.FolderID = CLng(m_folderId)
                    contObj.AddTaxonomyItem(request)
                End If

                If (page_meta_data.Count > 0 And hasMeta = True) Then
                    contObj.UpdateMetadata(page_meta_data)
                End If

            End If
        Next

        For Each contId In ids
            If (contId <> "") Then
                Try
                    Dim Status As String
                    Status = contApi.GetContentStatusById(CLng(contId))
                    If (Status = CStr("O")) Then ' this will check in and publish
                        contApi.PublishContentById(CLng(contId), m_folderId, ContentLanguage, "true", -1, "")
                    ElseIf (Status = CStr("I")) Then ' this is just a publish
                        contApi.EkContentRef.SubmitForPublicationv2_0(CLng(contId), m_folderId)
                    End If
                Catch ex As Exception
                    ' I wrapped it in this try block because there is a current problem on the server where the content is already being put
                    ' into published state if there are multiple pieces of content, the metadata still updates and is put in the right state.
                End Try
            End If
        Next

        Return True
    End Function
    Function CleanString(ByVal inStr As String) As String
        Dim outStr As String = String.Empty
        Dim l As Long
        For l = 1 To Len(inStr)
            If Asc(Mid(inStr, l, 1)) > 31 Then
                outStr = outStr & Mid(inStr, l, 1)
            End If
        Next
        CleanString = outStr
    End Function

    Private Function CheckInContent() As Boolean
        Dim contApi As New ContentAPI
        Dim contObj As EkContent
        Dim ContentLanguage As Integer = CONTENT_LANGUAGES_UNDEFINED

        If (Not (Page.Request.QueryString("LangType") Is Nothing)) Then
            If (Page.Request.QueryString("LangType") <> "") Then
                ContentLanguage = Convert.ToInt32(Page.Request.QueryString("LangType"))
                contApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
            Else
                If contApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(contApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If contApi.GetCookieValue("LastValidLanguageID") <> "" Then
                ContentLanguage = Convert.ToInt32(contApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If

        If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            contApi.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            contApi.ContentLanguage = ContentLanguage
        End If

        contObj = contApi.EkContentRef

        Dim ids() As String
        'Dim contId As String
        ids = m_idString.Split(",")

        'For Each contId In ids
        '    If (contId <> "") Then
        '        Try
        '            Dim Status As String
        '            Status = contApi.GetContentStatusById(Integer.Parse(contId))
        '            If (Status <> CStr("I") And Status <> CStr("A")) Then
        '                contObj.CheckContentInv2_0(Integer.Parse(contId))
        '            End If
        '        Catch ex As Exception

        '        End Try
        '    End If
        'Next
    End Function


    Private Sub ToolBar()
        Dim closeWin As String = ""
        Dim result As System.Text.StringBuilder
        result = New System.Text.StringBuilder
        If (ltrShowMetadata.Text = "") Then
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("dms taxonomy title"))
        ElseIf (ltrShowTaxonomy.Text = "") Then
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("dms metadata title"))
        Else
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("dms metadata and taxonomy title"))
        End If
        If (Request.QueryString("close") = "true") Then
            closeWin = "close"
        End If
        result.Append("<table ><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/ui/icons/contentpublish.png", "#", m_refMsg.GetMessage("generic Publish"), m_refMsg.GetMessage("generic Publish"), "onclick=""return SubmitForm('form1', '', '" & closeWin & "');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("generic Cancel"), "onclick=""return CancelForm('form1', '', '" & closeWin & "');"""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("addmetadataforfiles"))
        result.Append("</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        '"DMSMetadata.aspx?action=Submit&idString=" & m_idString & "&folderId=" & m_folderId
    End Sub

    Private Sub RegisterResources()
        API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/workareahelper.js", "EktronWorkareaHelperJS")
        '    API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)

        '    API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        '    API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/searchfuncsupport.js", "EktronSearchFuncSupportJS")
        '    API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/internCalendarDisplayFuncs.js", "EktronIntenCalendarDisplayFuncsJS")
        '    API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.utils.url.js", "EktronTreeUtilsURLJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.explorer.init.js", "EktronTreeExplorerInitJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.explorer.js", "EktronTreeExplorerJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.explorer.config.js", "EktronTreeExplorerConfigJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.explorer.windows.js", "EktronExplorerWindowsJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.cms.types.js", "EktronTreeTypesJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.cms.parser.js", "EktronTreeParserJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.cms.toolkit.js", "EktronTreeToolKitJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.cms.api.js", "EktronTreeAPIJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.ui.contextmenu.js", "EktronTreeUIContextMenuJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.ui.iconlist.js", "EktronTreeUIIconListJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.ui.tabs.js", "EktronTreeUITabsJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.ui.explore.js", "EktronTreeUIExplorerJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.ui.taxonomytree.js", "EktronUITaxonomyTreeJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.net.http.js", "EktronTreeMetHttpJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.lang.exception.js", "EktronTreeLangExceptionJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.utils.form.js", "EktronTreeUtilsFormJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.utils.log.js", "EktronTreeUtilsLogJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.utils.dom.js", "EktronTreeUtilsDOMJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.utils.debug.js", "EktronTreeUtilsDebugJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.utils.string.js", "EktronTreeUtilsStringJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.utils.cookie.js", "EktronTreeUtilsCookieJS")
        '    API.JS.RegisterJS(Me, m_refContentApi.AppPath & "Tree/js/com.ektron.utils.querystring.js", "EktronTreeUtilsQueryStringJS")

    End Sub
End Class
