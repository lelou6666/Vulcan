Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants

Partial Class editcontentattributes
    Inherits System.Web.UI.UserControl

    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Common.EkMessageHelper
    Protected m_intId As Long = 0
    Protected folder_data As FolderData
    Protected security_data As PermissionData
    Protected AppImgPath As String = ""
    Protected ContentType As Integer = 1
    Protected CurrentUserId As Long = 0
    Protected pagedata As Collection
    Protected m_strPageAction As String = ""
    Protected m_strOrderBy As String = ""
    Protected ContentLanguage As Integer = -1
    Protected EnableMultilingual As Integer = 0
    Protected SitePath As String = ""
    Protected content_data As ContentData
    Protected m_strCallerPage As String = ""
    Protected m_strFolderId As String = ""
    'Protected aFlagSets() As FolderFlagDefData = Array.CreateInstance(GetType(Ektron.Cms.FolderFlagDefData), 0)

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        m_refMsg = m_refContentApi.EkMsgRef
        RegisterResources()
	End Sub
    Public Function EditContentProperties() As Boolean
        If (Not (Request.QueryString("id") Is Nothing)) Then
            m_intId = Convert.ToInt64(Request.QueryString("id"))
        End If
        If (Not (Request.QueryString("action") Is Nothing)) Then
            m_strPageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If
        If (Not (Request.QueryString("orderby") Is Nothing)) Then
            m_strOrderBy = Convert.ToString(Request.QueryString("orderby"))
        End If
        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            If (Request.QueryString("LangType") <> "") Then
                ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
            Else
                If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If
        If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            m_refContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            m_refContentApi.ContentLanguage = ContentLanguage
        End If
        CurrentUserId = m_refContentApi.UserId
        AppImgPath = m_refContentApi.AppImgPath
        SitePath = m_refContentApi.SitePath
        EnableMultilingual = m_refContentApi.EnableMultilingual
		Dim content As ContentStateData
		content = m_refContentApi.GetContentState(m_intId)
		ContentType = content.Type
        ' the following group is for forms:
        If (Not (Request.QueryString("callerpage") Is Nothing)) Then
            m_strCallerPage = Request.QueryString("callerpage")
        End If
        If (Not (Request.QueryString("folder_id") Is Nothing)) Then
            m_strFolderId = Request.QueryString("folder_id")
        End If


        If (Not (Page.IsPostBack)) Then
            Display_EditContentProperties()
        Else
            Process_UpdateContentProperties()
        End If
    End Function
#Region "ACTION - UpdateContentProperties"
    Private Sub Process_UpdateContentProperties()
        Dim bInheritanceIsDif As Boolean = False
        bInheritanceIsDif = False
        Dim init_xmlconfig As String = ""
        Dim init_frm_xmlinheritance As String = ""
		Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim XmlInd As Ektron.Cms.Content.EkXmlIndexing
        Dim content_data As ContentData = Nothing
        Try
            m_refContent = m_refContentApi.EkContentRef

            Dim subtype As EkEnumeration.CMSContentSubtype = m_refContent.GetContentSubType(m_intId)
            content_data = m_refContent.GetContentById(m_intId)

            init_xmlconfig = Request.Form("init_xmlconfig")
            init_frm_xmlinheritance = Request.Form("init_frm_xmlinheritance")
            'If init_frm_xmlinheritance = "1" Then
            '    bInheritanceIsDif = True
            'End If
            bInheritanceIsDif = True
            pagedata = New Collection
            pagedata.Add(Request.Form(content_id.UniqueID), "ContentID")
            'If (Request.Form("frm_xmlinheritance") = "on") Then
            '    If init_frm_xmlinheritance = "0" Then
            '        bInheritanceIsDif = True
            '    End If
            '    pagedata.Add(True, "XmlInherited")
            'Else
            '    pagedata.Add(False, "XmlInherited")
            'End If
            If (subtype = EkEnumeration.CMSContentSubtype.WebEvent) Then
                pagedata.Add(content_data.XmlInheritedFrom, "XmlInherited")
                pagedata.Add(content_data.XmlConfiguration.Id, "CollectionID")
            Else
                pagedata.Add(False, "XmlInherited")
                pagedata.Add(Request.Form("xmlconfig"), "CollectionID")
            End If

            If (Request.Form("IsSearchable") = "on") Then
                pagedata.Add(1, "IsSearchable")
            Else
                pagedata.Add(0, "IsSearchable")
            End If

            m_refContent.UpdateContentProperties(pagedata)

            ' Update content flagging:
            'Dim ddlObj As Object = Request.Form(flaggingDefinitionsDDL.UniqueID)
            'If ((Not IsNothing(ddlObj)) AndAlso IsNumeric(ddlObj)) Then
            '	m_refContentApi.AssignFlagToContent(m_intId, CType(ddlObj.ToString, Integer))
            'End If

            'reverting 27535 - do not udpate xml_index table with new xml index search
            If (ContentType <> 2 AndAlso subtype <> EkEnumeration.CMSContentSubtype.WebEvent) Then
                'form content should not be indexed.
                If init_xmlconfig <> Request.Form("xmlconfig") Or bInheritanceIsDif Then
                    XmlInd = m_refContentApi.EkXmlIndexingRef
                    XmlInd.RemoveIndexDoc(CLng(Request.Form(content_id.UniqueID)))
                    XmlInd.IndexDoc(CLng(Request.Form(content_id.UniqueID)))
                End If
            End If

            If (subtype <> EkEnumeration.CMSContentSubtype.WebEvent) Then
                If (m_refContentApi.EkContentRef.MultiConfigExists(content_data.Id, content_data.LanguageId)) Then
                    m_refContentApi.EkContentRef.UpdateMultiConfigToXml(content_data.Id, content_data.LanguageId, Request.Form("xmlconfig"))
                Else
                    m_refContentApi.EkContentRef.CreateMulticonfigEntry(content_data.Id, content_data.FolderId, content_data.LanguageId, m_refContentApi.GetFolderById(content_data.FolderId).TemplateId, Request.Form("xmlconfig"))
                End If
            End If

            'Dim node As New SitemapPath
            'node.Description = content_data.Title
            'node.FolderId = content_data.FolderId
            'node.Language = content_data.LanguageId
            'node.Title = content_data.Title
            'node.Url = content_data.Quicklink
            'If (Request.Form("ckAddFolderBreadCrumb") IsNot Nothing) Then
            '    If (Request.Form("ckAddFolderBreadCrumb") = "on" And Request.Form("previousInFolderBreadcrumb") <> "true") Then
            '        m_refContentApi.EkContentRef.AddFolderSitemapPathNode(content_data.FolderId, node)
            '    End If
            'ElseIf (Request.Form("previousInFolderBreadcrumb") = "true") Then
            '    m_refContentApi.EkContentRef.DeleteFolderSitemapPathNode(content_data.FolderId, node)
            'End If
            If (m_strCallerPage = "cmsform.aspx") Then
                Response.Redirect("cmsform.aspx?LangType=" & ContentLanguage & "&action=ViewForm&form_id=" & m_intId & "&folder_id=" & m_strFolderId, False)
            Else
                Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & Request.Form(content_id.UniqueID), False)
            End If

        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & ContentLanguage, False)
        End Try
	End Sub

#End Region
#Region "CONTENT - EditContentProperties"
    Private Sub Display_EditContentProperties()

        Dim xmlconfig_data() As XmlConfigData
        Dim xmlseltagvalue As String = "0"
        Dim OptionSelected As Boolean = False
        Dim xmlconfig_content As MultiConfigData
        Dim i As Integer = 0
        Dim configResource As String = "xml configuration label"

        content_data = m_refContentApi.GetContentById(m_intId)
        If content_data.Type = EkConstants.CMSContentType_CatalogEntry Then
            Dim m_refProductTypeAPI As New ProductTypeApi()
            Dim criteria As New Criteria(Of ProductTypeProperty)

            xmlconfig_data = m_refProductTypeAPI.GetList(criteria).ToArray()
            configResource = "lbl product type xml config"
        Else
            xmlconfig_data = m_refContentApi.GetAllXmlConfigurations("title")
        End If
        folder_data = m_refContentApi.GetFolderById(content_data.FolderId)
        security_data = m_refContentApi.LoadPermissions(m_intId, "content")
        content_id.Value = m_intId
        xmlconfig_content = m_refContentApi.EkContentRef.GetXmlConfig(content_data.Id, content_data.LanguageId)
        If content_data.Type = EkConstants.CMSContentType_CatalogEntry Then EditEntryPropertiesToolBar() Else EditContentPropertiesToolBar()

        If (xmlconfig_content.XmlID = 0) Then
            If (content_data.XmlConfiguration Is Nothing) Then
                content_data.XmlConfiguration = New XmlConfigData()
            End If
        End If
        If (content_data.XmlConfiguration.Id <> 0) Then

            td_ecp_xmlconfiglbl.InnerHtml = m_refMsg.GetMessage(configResource)

            If content_data.Type = EkConstants.CMSContentType_CatalogEntry Then
                td_ecp_xmlconfig.InnerHtml = "<input type=""hidden"" name=""xmlconfig"" id=""xmlconfig"" value=""" & content_data.XmlConfiguration.Id & """/>"
                td_ecp_xmlconfig.InnerHtml += "<select name=""xmlconfig_disabled"" "
            Else
                td_ecp_xmlconfig.InnerHtml = "<select name=""xmlconfig"" "
            End If
            
            If (xmlconfig_content.XmlID = 0 Or content_data.Type = EkConstants.CMSContentType_CatalogEntry) Then
                td_ecp_xmlconfig.InnerHtml += " disabled "
            End If
            td_ecp_xmlconfig.InnerHtml += ">"
            If (Not (IsNothing(xmlconfig_data))) Then
                For i = 0 To xmlconfig_data.Length - 1
                    If (content_data.IsXmlInherited = 0 Or xmlconfig_content.XmlID <> 0) Then
                        If (Not (IsNothing(content_data.XmlConfiguration))) Then
                            If (content_data.XmlConfiguration.Id = xmlconfig_data(i).Id Or xmlconfig_content.XmlID = xmlconfig_data(i).Id) Then
                                OptionSelected = True
                                xmlseltagvalue = xmlconfig_data(i).Id
                            Else
                                OptionSelected = False
                            End If
                        End If
                    End If

                    td_ecp_xmlconfig.InnerHtml += "<option value=""" & xmlconfig_data(i).Id & """"
                    If (OptionSelected) Then
                        td_ecp_xmlconfig.InnerHtml += " selected "
                    End If
                    td_ecp_xmlconfig.InnerHtml += ">" & xmlconfig_data(i).Title
                Next
            End If

            td_ecp_xmlconfig.InnerHtml += "</select>"
            td_ecp_xmlconfig.InnerHtml += "<input type=""hidden"" name=""init_xmlconfig"" value=""" & xmlseltagvalue & """>"
            If Not (content_data.Type = EkConstants.CMSContentType_CatalogEntry) Then td_ecp_xmlconfig_lnk.InnerHtml = "<a href=""#"" Onclick=""javascript:PreviewXmlConfig();""><img src=""" & m_refContentApi.AppPath & "images/UI/Icons/preview.png" & """ border=""0"" alt=""" & m_refMsg.GetMessage("alt preview button text (xml config)") & """ title=""" & m_refMsg.GetMessage("alt preview button text (xml config)") & """></a>"
        End If
        If (content_data.SubType <> EkEnumeration.CMSContentSubtype.WebEvent) Then
            xmlConfigPanel.Visible = True
        Else
            xmlConfigPanel.Visible = False
        End If
        searchable.InnerHtml = m_refMsg.GetMessage("lbl content searchable")
        searchable.InnerHtml += " <input type=""checkbox"" name=""IsSearchable"""
        If (content_data.IsSearchable = True) Then
            searchable.InnerHtml += " checked "
        End If
        searchable.InnerHtml += ">"


        ' Display content flagging options:

        flagging.InnerHtml = m_refMsg.GetMessage("wa tree flag def")

        Dim contentFlagId As Long = content_data.FlagDefId
        If (contentFlagId > 0) Then
            Dim fd As FlagDefData = m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(contentFlagId, False)
            If (fd IsNot Nothing) Then
                If (String.IsNullOrEmpty(fd.Name)) Then
                    lblflag.Text = "None"
                Else
                    lblflag.Text = fd.Name ' & " (Id:" & fd.ID & ")"
                End If
            Else
                lblflag.Text = "None"
            End If
        Else
            lblflag.Text = "None"
        End If

    End Sub
    Private Sub EditContentPropertiesToolBar()
        Dim result As System.Text.StringBuilder
        result = New System.Text.StringBuilder
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view properties for content") & " """ & content_data.Title & """")
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (content props)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('editfolder', '');"""))
        If (m_strCallerPage = "cmsform.aspx") Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "cmsform.aspx?LangType=" & ContentLanguage & "&action=ViewForm&form_id=" & m_intId & "&folder_id=" & m_strFolderId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub EditEntryPropertiesToolBar()
        Dim result As System.Text.StringBuilder
        result = New System.Text.StringBuilder
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view properties for entry") & " """ & content_data.Title & """")
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (entry props)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('editfolder', '');"""))
        If (m_strCallerPage = "cmsform.aspx") Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "cmsform.aspx?LangType=" & ContentLanguage & "&action=ViewForm&form_id=" & m_intId & "&folder_id=" & m_strFolderId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("editcontentproperties_ecom"))
        result.Append("</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub

#End Region
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
End Class
