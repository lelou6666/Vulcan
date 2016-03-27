Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Common

Partial Class movefolderitem
    Inherits System.Web.UI.UserControl

    Private cAllFolders As Collection
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
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
    Protected m_refContent As Ektron.Cms.Content.EkContent
    Protected m_intFolderId As Long = -1
    Protected m_rootFolderIsXml As Integer = 0
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected m_intTotalRecords As Integer = 0
    Protected m_strDisabled As String = " disabled "
    Protected m_strRadBtnCopy As String = " "
    Protected m_strRadBtnMove As String = " checked "
    Protected m_bShowCheckin As Boolean = True
    Protected m_refCopyMoveHref As String = ""
    Protected FolderId As String = ""
    Protected _initIsCommerceAdmin As Boolean = False
    Protected _isCommerceAdmin As Boolean = False
    Protected _initIsFolderAdmin As Boolean = False
    Protected _isFolderAdmin As Boolean = False
    Protected _initIsCopyOrMoveAdmin As Boolean = False
    Protected _isCopyOrMoveAdmin As Boolean = False
    Protected pbcAction As String = "0"
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        m_refMsg = m_refContentApi.EkMsgRef
        RegisterResources()
        InitFolderIsXmlFlags()
        If (Not (Request.QueryString("id") Is Nothing)) Then
            m_intId = Convert.ToInt64(Request.QueryString("id"))
        End If
        If (Not (Request.QueryString("action") Is Nothing)) Then
            m_strPageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If
        If (Not (Request.QueryString("orderby") Is Nothing)) Then
            m_strOrderBy = Convert.ToString(Request.QueryString("orderby"))
        End If
        folder_data = m_refContentApi.GetFolderById(m_intId)
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
        m_refCopyMoveHref = "content.aspx?LangType=" & ContentLanguage & "&action=" & m_strPageAction & "&id=" & m_intId
        If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            m_refContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            m_refContentApi.ContentLanguage = ContentLanguage
        End If
        If (Request.QueryString("op") IsNot Nothing AndAlso Request.QueryString("op") = "copy") Then
            m_strRadBtnCopy = " checked "
            m_strRadBtnMove = " "
            m_strDisabled = ""
            m_bShowCheckin = False
        End If
        CurrentUserId = m_refContentApi.UserId
        AppImgPath = m_refContentApi.AppImgPath
        SitePath = m_refContentApi.SitePath
        EnableMultilingual = m_refContentApi.EnableMultilingual
        m_refContent = m_refContentApi.EkContentRef
        If IsPostBack And Request.Form(isPostData.UniqueID) <> "" Then
            Process_MoveMultiContent()
        Else
            If (IsPostBack() = False Or (Request.Form.Count > 0 And Request.Form(isPostData.UniqueID) <> "")) Then
                Display_Move(folder_data.FolderType)
            End If
        End If
        Util_SetServerVariables()

    End Sub

    Private Function IsCommerceAdmin() As Boolean
        If (_initIsCommerceAdmin) Then
            Return _isCommerceAdmin
        End If
        _isCommerceAdmin = m_refContentApi.IsARoleMember(CmsRoleIds.CommerceAdmin)
        _initIsCommerceAdmin = True
        Return _isCommerceAdmin
    End Function

    Private Function IsFolderAdmin() As Boolean
        If (_initIsFolderAdmin) Then
            Return _isFolderAdmin
        End If
        _isFolderAdmin = m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(m_intId)
        _initIsFolderAdmin = True
        Return _isFolderAdmin
    End Function

    Private Function IsCopyOrMoveAdmin() As Boolean
        If (_initIsCopyOrMoveAdmin) Then
            Return _isCopyOrMoveAdmin
        End If
        _isCopyOrMoveAdmin = m_refContentApi.IsARoleMemberForFolder(EkEnumeration.CmsRoleIds.MoveOrCopy, m_intId, m_refContentApi.UserId)
        _initIsCopyOrMoveAdmin = True
        Return _isCopyOrMoveAdmin
    End Function

#Region "Process"


    Private Sub Process_MoveMultiContent()
        Dim intMoveFolderId As Long = 0
        Dim strContentIds As String = ""
        Dim arrArray As Object
        Dim i As Integer = 0
        Dim strContentLanguages As String = ""
        Dim arrLanguages As Object
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim FolderPath As String
        Dim contCount As Integer = 0
        Dim m_refsite As New SiteAPI
        Dim langCount As Integer = 0

        Try
            m_refContent = m_refContentApi.EkContentRef
            m_intFolderId = Request.Form(folder_id.UniqueID)
            intMoveFolderId = m_refContent.GetFolderID(Request.Form("move_folder_id"))
            arrArray = Split(Request.Form(contentids.UniqueID), ",")
            For i = LBound(arrArray) To UBound(arrArray)
                If (InStr(Request.Form("id_" & arrArray(i)), "on") > 0) Then
                    strContentIds = strContentIds & arrArray(i) & ","
                End If
            Next
            If (strContentIds <> "") Then
                strContentIds = Left(strContentIds, Len(strContentIds) - 1)
            End If
            arrLanguages = Split(Request.Form(contentlanguages.UniqueID), ",")
            For i = LBound(arrArray) To UBound(arrArray)
                If (InStr(Request.Form("id_" & arrArray(i)), "on") > 0) Then
                    strContentLanguages = strContentLanguages & arrLanguages(i) & ","
                End If
            Next
            If (strContentLanguages <> "") Then
                strContentLanguages = Left(strContentLanguages, Len(strContentLanguages) - 1)
            End If
            If Request.Form(hdnCopyAll.UniqueID) = "true" Then
                Dim publishContent As Boolean = False
                If (Not Request.Form("btn_PublishCopiedContent") Is Nothing AndAlso Request.Form("btn_PublishCopiedContent").ToString() = "on") Then
                    publishContent = True
                End If
                Dim countContentIds = strContentIds.Split(",")
                For contCount = 0 To countContentIds.Length - 1
                    m_refContent.CopyAllLanguageContent(CLng(countContentIds(contCount)), intMoveFolderId, publishContent)
                Next
            Else
                If (Not Request.Form(RadBtnMoveCopyValue.UniqueID) Is Nothing AndAlso Request.Form(RadBtnMoveCopyValue.UniqueID).ToString() = "copy") Then
                    Dim bPublish As Boolean = False
                    If (Not Request.Form("btn_PublishCopiedContent") Is Nothing AndAlso Request.Form("btn_PublishCopiedContent").ToString() = "on") Then
                        bPublish = True
                    End If
                    m_refContent.CopyContentByID(strContentIds, strContentLanguages, intMoveFolderId, bPublish)
                Else
                    m_refContent.MoveContent(strContentIds, strContentLanguages, intMoveFolderId)
                End If
            End If

            FolderPath = m_refContent.GetFolderPath(intMoveFolderId)
            If (Right(FolderPath, 1) = "\") Then
                FolderPath = Right(FolderPath, Len(FolderPath) - 1)
            End If
            FolderPath = Replace(FolderPath, "\", "\\")
            Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewContentByCategory&id=" & intMoveFolderId & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)

        Catch ex As Exception
            Dim intError As Integer
            Dim strError As String
            strError = "because a record with the same title exists in the destination folder"
            intError = ex.Message.IndexOf(strError)
            If intError > -1 Then
                strError = Left(ex.Message, intError + strError.Length)
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(strError & "."), False)
            Else
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
            End If
        End Try
    End Sub


#End Region


#Region "Display"


    Public Sub Display_Move(ByVal containerType As FolderType)

        Select Case containerType

            Case FolderType.Catalog

                Display_MoveEntries()

            Case Else

                Display_MoveContentByCategory()

        End Select

    End Sub


#Region "Entries"


    Private Sub Display_MoveEntries()

        Dim CatalogManager As New CatalogEntry(m_refContentApi.RequestInformationRef)
        Dim entryList As New System.Collections.Generic.List(Of EntryData)()
        Dim entryCriteria As New Ektron.Cms.Common.Criteria(Of EntryProperty)
        Dim totalPages As Integer = 1

        security_data = m_refContentApi.LoadPermissions(m_intId, "folder")

        entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_intId)
        If m_refContentApi.RequestInformationRef.ContentLanguage > 0 Then entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.ContentLanguage)
        entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, False)
        entryCriteria.OrderByDirection = OrderByDirection.Ascending
        entryCriteria.OrderByField = EntryProperty.Title

        entryCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        entryCriteria.PagingInfo.CurrentPage = m_intCurrentPage

        entryList = CatalogManager.GetList(entryCriteria)

        m_intTotalPages = entryCriteria.PagingInfo.TotalPages

        ' td_copy.Visible = False

        MoveContentByCategoryToolBar()

        FolderId = folder_data.Id

        source_folder_is_xml.Value = 1

        Page.ClientScript.RegisterHiddenField("xmlinherited", "false")

        lblDestinationFolder.Text = "<input id=""move_folder_id"" size=""50%"" name=""move_folder_id"" value=""\"" readonly=""true""/>  <a href=""#"" onclick=""LoadSelectCatalogFolderChildPage();return true;"">" & m_refMsg.GetMessage("lbl ecomm coupon select folder") & "</a>"

        folder_id.Value = m_intId
        PageSettings()

        Populate_MoveCatalogGrid(entryList)

    End Sub

    Private Sub Populate_MoveCatalogGrid(ByVal entryList As System.Collections.Generic.List(Of EntryData))

        MoveContentByGategoryGrid.Controls.Clear()
        contentids.Value = ""
        contentlanguages.Value = ""
        Dim colBound As System.Web.UI.WebControls.BoundColumn
        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "BOX"
        colBound.HeaderText = "<input type=""checkbox"" name=""all"" onclick=""checkAll(document.forms[0].all.checked);"">"
        colBound.ItemStyle.Width = Unit.Parse("1")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.HeaderStyle.CssClass = "title-header"
        MoveContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=MoveContentByCategory&orderby=Title&id=" & m_intId & "&LangType=" & ContentLanguage & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Title") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        MoveContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=MoveContentByCategory&orderby=ID&id=" & m_intId & "&LangType=" & ContentLanguage & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic ID") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        MoveContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STATUS"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=MoveContentByCategory&orderby=status&id=" & m_intId & "&LangType=" & ContentLanguage & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Status") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.Wrap = False
        MoveContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DATEMODIFIED"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=MoveContentByCategory&orderby=DateModified&id=" & m_intId & "&LangType=" & ContentLanguage & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Date Modified") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        MoveContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITORNAME"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=MoveContentByCategory&orderby=editor&id=" & m_intId & "&LangType=" & ContentLanguage & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Last Editor") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        MoveContentByGategoryGrid.Columns.Add(colBound)


        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("BOX", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(Int64)))
        dt.Columns.Add(New DataColumn("STATUS", GetType(String)))
        dt.Columns.Add(New DataColumn("DATEMODIFIED", GetType(String)))
        dt.Columns.Add(New DataColumn("EDITORNAME", GetType(String)))

        Dim i As Integer
        For i = 0 To entryList.Count - 1
            dr = dt.NewRow()
            dr(0) = ""
            If (entryList(i).ContentStatus = "A" OrElse (m_bShowCheckin AndAlso entryList.Item(i).ContentStatus = "I")) Then
                If (contentids.Value = "") Then
                    contentids.Value = entryList.Item(i).Id
                Else
                    contentids.Value += "," & entryList.Item(i).Id
                End If

                If (contentlanguages.Value = "") Then
                    contentlanguages.Value = entryList.Item(i).LanguageId
                Else
                    contentlanguages.Value += "," & entryList.Item(i).LanguageId
                End If

                dr(0) = "<input type=""checkbox"" onclick=""checkAllFalse();"" name=""id_" & entryList.Item(i).Id & """>"
            End If
            Select Case entryList(i).EntryType

                Case CatalogEntryType.SubscriptionProduct

                    dr(1) = dr(1) + "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/bookGreen.png" & """ class=""ektronRightSpaceVerySmall ektronLeft""/>" & entryList(i).Title

                Case CatalogEntryType.ComplexProduct

                    dr(1) = dr(1) + "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/bricks.png" & """  class=""ektronRightSpaceVerySmall ektronLeft""/>" & entryList(i).Title

                Case CatalogEntryType.Kit

                    dr(1) = dr(1) + "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/box.png" & """  class=""ektronRightSpaceVerySmall ektronLeft""/>" & entryList(i).Title

                Case CatalogEntryType.Bundle

                    dr(1) = dr(1) + "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/package.png" & """  class=""ektronRightSpaceVerySmall ektronLeft""/>" & entryList(i).Title

                Case CatalogEntryType.Product

                    dr(1) = dr(1) + "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/brick.png" & """  class=""ektronRightSpaceVerySmall ektronLeft""/>" & entryList(i).Title

            End Select
            dr(2) = entryList.Item(i).Id
            dr(3) = entryList.Item(i).ContentStatus
            dr(4) = entryList.Item(i).DateModified.ToShortDateString
            dr(5) = entryList.Item(i).LastEditorLastName
            dt.Rows.Add(dr)
        Next i

        Dim dv As New DataView(dt)
        MoveContentByGategoryGrid.DataSource = dv
        MoveContentByGategoryGrid.DataBind()
    End Sub


#End Region


#Region "Content"


    Private Sub Display_MoveContentByCategory()
        Dim contentdata As New Ektron.Cms.Common.EkContentCol
        Dim totalPages As Integer = 1

        pagedata = New Collection
        pagedata.Add(m_intId, "FolderID")
        pagedata.Add(m_strOrderBy, "OrderBy")
        security_data = m_refContentApi.LoadPermissions(m_intId, "folder")
        folder_data = m_refContentApi.GetFolderById(m_intId)

        contentdata = m_refContent.GetAllViewableChildInfov5_0(pagedata, m_intCurrentPage, m_refContentApi.RequestInformationRef.PagingSize, m_intTotalPages, Ektron.Cms.Common.EkEnumeration.CMSContentType.AllTypes)

        MoveContentByCategoryToolBar()
        FolderId = folder_data.Id


        ' javascript needs to know if source folder is using xml:
        source_folder_is_xml.Value = IIf((Not (folder_data.XmlConfiguration Is Nothing)), 1, 0)

        ' Obsolete: The recommended alternative is ClientScript.RegisterHiddenField(string
        ' hiddenFieldName, string hiddenFieldInitialValue).
        ' http://go.microsoft.com/fwlink/?linkid=14202
        Page.ClientScript.RegisterHiddenField("xmlinherited", "false")

        m_refContent = m_refContentApi.EkContentRef
        '        cAllFolders = m_refContent.GetFolderTreeForUserIDWithXMLInfo(0)

        Dim destinationFolder As String
        If folder_data.FolderType = 9 Then
            destinationFolder = "<span style=""white-space:nowrap""><input id=""move_folder_id"" size=""50%"" name=""move_folder_id"" value=""\"" readonly=""true""/>  <a class=""button buttonInline greenHover minHeight buttonCheckAll"" style=""padding-top:.25em; padding-bottom:.25em;"" href=""#"" onclick=""LoadSelectCatalogFolderChildPage();return true;"">" & m_refMsg.GetMessage("lbl ecomm coupon select folder") & "</a></span>"
        Else
            destinationFolder = "<span style=""white-space:nowrap""><input id=""move_folder_id"" size=""50%"" name=""move_folder_id"" value=""\"" readonly=""true""/>  <a class=""button buttonInline greenHover minHeight buttonCheckAll"" style=""padding-top:.25em; padding-bottom:.25em;"" href=""#"" onclick=""LoadSelectFolderChildPage();return true;"">" & m_refMsg.GetMessage("lbl select folder") & "</a></span>"
        End If

        lblDestinationFolder.Text = destinationFolder.ToString
        folder_id.Value = m_intId
        PageSettings()

        Populate_MoveContentByCategoryGrid(contentdata)

    End Sub

    Private Sub Populate_MoveContentByCategoryGrid(ByVal contentdata As Ektron.Cms.Common.EkContentCol)
        MoveContentByGategoryGrid.Controls.Clear()
        contentids.Value = ""
        contentlanguages.Value = ""
        Dim colBound As System.Web.UI.WebControls.BoundColumn
        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "BOX"
        colBound.HeaderText = "<input type=""checkbox"" name=""all"" onclick=""checkAll(document.forms[0].all.checked);"">"
        colBound.ItemStyle.Width = Unit.Parse("1")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.HeaderStyle.CssClass = "title-header"
        MoveContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=MoveContentByCategory&orderby=Title&id=" & m_intId & "&LangType=" & ContentLanguage & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Title") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        MoveContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=MoveContentByCategory&orderby=ID&id=" & m_intId & "&LangType=" & ContentLanguage & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic ID") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        MoveContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STATUS"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=MoveContentByCategory&orderby=status&id=" & m_intId & "&LangType=" & ContentLanguage & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Status") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.Wrap = False
        MoveContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DATEMODIFIED"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=MoveContentByCategory&orderby=DateModified&id=" & m_intId & "&LangType=" & ContentLanguage & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Date Modified") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        MoveContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITORNAME"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=MoveContentByCategory&orderby=editor&id=" & m_intId & "&LangType=" & ContentLanguage & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Last Editor") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        MoveContentByGategoryGrid.Columns.Add(colBound)


        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("BOX", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(Int64)))
        dt.Columns.Add(New DataColumn("STATUS", GetType(String)))
        dt.Columns.Add(New DataColumn("DATEMODIFIED", GetType(String)))
        dt.Columns.Add(New DataColumn("EDITORNAME", GetType(String)))

        Dim i As Integer
        For i = 0 To contentdata.Count - 1
            'If (contentdata.Item(i).ContentSubType <> CMSContentSubtype.PageBuilderData) Then 
            dr = dt.NewRow()
            dr(0) = ""
            If (contentdata.Item(i).ContentStatus = "A" OrElse (m_bShowCheckin AndAlso contentdata.Item(i).ContentStatus = "I")) Then
                If (contentdata.Item(i).ContentSubType = CMSContentSubtype.PageBuilderData) Then
                    pbcAction = "1"
                End If
                If (contentids.Value = "") Then
                    contentids.Value = contentdata.Item(i).Id
                Else
                    contentids.Value += "," & contentdata.Item(i).Id
                End If

                If (contentlanguages.Value = "") Then
                    contentlanguages.Value = contentdata.Item(i).Language
                Else
                    contentlanguages.Value += "," & contentdata.Item(i).Language
                End If

                dr(0) = "<input type=""checkbox"" onclick=""checkAllFalse();"" name=""id_" & contentdata.Item(i).Id & """>"
            End If
            If contentdata.Item(i).ContentType = Ektron.Cms.Common.EkEnumeration.CMSContentType.CatalogEntry Then
                dr(1) = "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/brick.png"" class=""ektronRightSpaceVerySmall ektronLeft"" />" & "<a href=""content.aspx?LangType=" & contentdata.Item(i).Language & "&action=View&id=" & contentdata.Item(i).Id & """ title='" & m_refMsg.GetMessage("generic View") & " """ & Replace(contentdata.Item(i).Title, "'", "`") & """" & "'>" & contentdata.Item(i).Title & "</a>"
            ElseIf contentdata.Item(i).ContentType = Ektron.Cms.Common.EkEnumeration.CMSContentType.Forms Then
                dr(1) = "<img src=""images/ui/icons/contentForm.png""/>&nbsp;" & "<a href=""content.aspx?LangType=" & contentdata.Item(i).Language & "&action=View&id=" & contentdata.Item(i).Id & """ title='" & m_refMsg.GetMessage("generic View") & " """ & Replace(contentdata.Item(i).Title, "'", "`") & """" & "'>" & contentdata.Item(i).Title & "</a>"
            ElseIf contentdata.Item(i).ContentType = Ektron.Cms.Common.EkEnumeration.CMSContentType.Content Then
                If contentdata.Item(i).ContentSubType = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData Then
                    dr(1) = "<img src=""images/application/layout_content.png""/>&nbsp;" & "<a href=""content.aspx?LangType=" & contentdata.Item(i).Language & "&action=View&id=" & contentdata.Item(i).Id & """ title='" & m_refMsg.GetMessage("generic View") & " """ & Replace(contentdata.Item(i).Title, "'", "`") & """" & "'>" & contentdata.Item(i).Title & "</a>"
                Else
                    dr(1) = "<img src=""images/ui/icons/contentHtml.png""/>&nbsp;" & "<a href=""content.aspx?LangType=" & contentdata.Item(i).Language & "&action=View&id=" & contentdata.Item(i).Id & """ title='" & m_refMsg.GetMessage("generic View") & " """ & Replace(contentdata.Item(i).Title, "'", "`") & """" & "'>" & contentdata.Item(i).Title & "</a>"
                End If
            Else
                dr(1) = "<img src=""" & contentdata.Item(i).AssetInfo.ImageUrl & """  class=""ektronRightSpaceVerySmall ektronLeft""/>" & "<a href=""content.aspx?LangType=" & contentdata.Item(i).Language & "&action=View&id=" & contentdata.Item(i).Id & """ title='" & m_refMsg.GetMessage("generic View") & " """ & Replace(contentdata.Item(i).Title, "'", "`") & """" & "'>" & contentdata.Item(i).Title & "</a>"
            End If

            dr(2) = contentdata.Item(i).Id
            dr(3) = contentdata.Item(i).Status
            dr(4) = contentdata.Item(i).DisplayDateModified
            dr(5) = contentdata.Item(i).LastEditorLname
            dt.Rows.Add(dr)
            'End If
        Next i


        Dim dv As New DataView(dt)
        MoveContentByGategoryGrid.DataSource = dv
        MoveContentByGategoryGrid.DataBind()
    End Sub


#End Region


#End Region


#Region "Private Helpers"


    Private Sub Util_SetLabels()

        If folder_data.FolderType = 9 Then

            ltr_publishcopied.Text = m_refMsg.GetMessage("lbl publish copied entry")

        Else

            ltr_publishcopied.Text = m_refMsg.GetMessage("lbl publish copied content")

        End If

    End Sub

    Private Sub MoveContentByCategoryToolBar()
        Dim result As New System.Text.StringBuilder
        Util_SetLabels()
        'If folder_data.FolderType = 9 Then
        'txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("just move contents of folder") & " """ & folder_data.Name & """")
        'Else
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("move contents of folder") & " """ & folder_data.Name & """")
        'End If
        result.Append("<table><tr>")
        If (security_data.IsAdmin OrElse (folder_data.FolderType = EkEnumeration.FolderType.Catalog And True) OrElse IsFolderAdmin() OrElse IsCopyOrMoveAdmin()) Then
            'If folder_data.FolderType = EkEnumeration.FolderType.Catalog Then
            'result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentCopy.png", "#", m_refMsg.GetMessage("btn just move content"), m_refMsg.GetMessage("btn just move content"), "onclick=""checkMoveForm_Folder('" & m_refContent.GetFolderPath(m_intId).Replace("\", "\\") & "');return false;"""))
            'Else
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentCopy.png", "#", m_refMsg.GetMessage("btn move content"), m_refMsg.GetMessage("btn move content"), "onclick=""checkMoveForm_Folder('" & m_refContent.GetFolderPath(m_intId).Replace("\", "\\") & "');return false;"""))
            'End If
        End If
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "content.aspx?action=ViewContentByCategory&id=" & m_intId & "&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub InitFolderIsXmlFlags()
        Dim fldrData As FolderData
        Dim nFolderID As Long = 0
        fldrData = m_refContentApi.GetFolderById(0)
        m_rootFolderIsXml = IIf((Not (fldrData.XmlConfiguration Is Nothing)), 1, 0)
        If (Not (Request.QueryString("id") Is Nothing)) Then
            nFolderID = Convert.ToInt64(Request.QueryString("id"))
        End If
        fldrData = m_refContentApi.GetFolderById(nFolderID)
        source_folder_is_xml.Value = IIf((Not (fldrData.XmlConfiguration Is Nothing)), 1, 0)
    End Sub

    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
#End Region


#Region "Paging"


    Private Sub PageSettings()
        If (m_intTotalPages <= 1) Then
            VisiblePageControls(False)
        Else
            VisiblePageControls(True)
            TotalPages.Text = (System.Math.Ceiling(m_intTotalPages)).ToString()
            'hTotalPages.Value = (System.Math.Ceiling(m_intTotalPages)).ToString()
            CurrentPage.Text = m_intCurrentPage.ToString()
            'hCurrentPage.Value = m_intCurrentPage.ToString()
            ctrlPreviousPage.Enabled = True
            ctrlFirstPage.Enabled = True
            ctrlLastPage.Enabled = True
            ctrlNextPage.Enabled = True
            If m_intCurrentPage = 1 Then
                ctrlPreviousPage.Enabled = False
                ctrlFirstPage.Enabled = False
            ElseIf m_intCurrentPage = m_intTotalPages Then
                ctrlNextPage.Enabled = False
                ctrlLastPage.Enabled = False
            End If
        End If
    End Sub

    Private Sub VisiblePageControls(ByVal flag As Boolean)
        TotalPages.Visible = flag
        CurrentPage.Visible = flag
        ctrlPreviousPage.Visible = flag
        ctrlNextPage.Visible = flag
        ctrlLastPage.Visible = flag
        ctrlFirstPage.Visible = flag
        PageLabel.Visible = flag
        OfLabel.Visible = flag
    End Sub

    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                m_intCurrentPage = 1
            Case "Last"
                m_intCurrentPage = Int32.Parse(TotalPages.Text)
            Case "Next"
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) + 1
            Case "Prev"
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) - 1
        End Select
        Display_Move(folder_data.FolderType)
        isPostData.Value = "true"
    End Sub


#End Region
#Region "Utilities"
    Private Sub Util_SetServerVariables()
        jsConfirmCopyAll.text = m_refMsg.GetMessage("jsconfirm copy all")
    End Sub
#End Region


End Class
