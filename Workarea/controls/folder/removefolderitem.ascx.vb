Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.API

Partial Class removefolderitem
    Inherits System.Web.UI.UserControl

#Region "Member Variables"

    Protected _ContentApi As New ContentAPI
    Protected _StyleHelper As New StyleHelper
    Protected _MessageHelper As Ektron.Cms.Common.EkMessageHelper
    Protected _Id As Long = 0
    Protected _FolderData As FolderData
    Protected _PermissionData As PermissionData
    Protected _AppImgPath As String = ""
    Protected _ContentType As Integer = 1
    Protected _CurrentUserId As Long = 0
    Protected _PageData As Collection
    Protected _PageAction As String = ""
    Protected _OrderBy As String = ""
    Protected _ContentLanguage As Integer = -1
    Protected _EnableMultilingual As Integer = 0
    Protected _SitePath As String = ""
    Protected _EkContent As Ektron.Cms.Content.EkContent
    Protected _FolderId As Long = -1
    Protected _CurrentPageId As Integer = 1
    Protected _TotalPagesNumber As Integer = 1
    Protected _TotalRecordsNumber As Integer = 0
    Protected _ShowArchive As Boolean = False

#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        _MessageHelper = _ContentApi.EkMsgRef
        If (Not (Request.QueryString("id") Is Nothing)) Then
            _Id = Convert.ToInt64(Request.QueryString("id"))
        End If
        If (Not (Request.QueryString("action") Is Nothing)) Then
            _PageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If
        If (Not (Request.QueryString("orderby") Is Nothing)) Then
            _OrderBy = Convert.ToString(Request.QueryString("orderby"))
        End If
        If (Not (Request.QueryString("showarchive") Is Nothing)) AndAlso (Request.QueryString("showarchive").ToString().ToLower() = "true") Then
            _ShowArchive = True
        End If
        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            If (Request.QueryString("LangType") <> "") Then
                _ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                _ContentApi.SetCookieValue("LastValidLanguageID", _ContentLanguage)
            Else
                If _ContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    _ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If _ContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                _ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If

        If Request.QueryString("currentpage") IsNot Nothing Then _CurrentPageId = Convert.ToInt32(Request.QueryString("currentpage"))

        If _ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            _ContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            _ContentApi.ContentLanguage = _ContentLanguage
        End If
        _CurrentUserId = _ContentApi.UserId
        _AppImgPath = _ContentApi.AppImgPath
        _SitePath = _ContentApi.SitePath
        _EkContent = _ContentApi.EkContentRef
        _EnableMultilingual = _ContentApi.EnableMultilingual
        contentids.Value = ""
        DeleteContentByCategory()
        isPostData.Value = "true"

        RegisterJS()

    End Sub

#End Region

#Region "Helpers"

    Private Sub DeleteContentByCategory()
        If IsPostBack And Request.Form(isPostData.UniqueID) <> "" Then
            Process_submitMultiDelContAction()
        Else
            If (IsPostBack() = False Or (Request.Form.Count > 0 And Request.Form(isPostData.UniqueID) <> "")) Then
                Display_Delete()
            End If
        End If
    End Sub

    Private Sub Process_submitMultiDelContAction()
        Dim strContentIds As String = ""
        Dim arrArray As Object
        Dim i As Integer = 0
        Dim m_refContent As Ektron.Cms.Content.EkContent
        _FolderId = Convert.ToInt64(Request.QueryString("id"))
        _CurrentPageId = Convert.ToInt64(Request.Form(page_id.UniqueID))
        Try
            m_refContent = _ContentApi.EkContentRef
            arrArray = Split(Request.Form(contentids.UniqueID), ",")
            For i = LBound(arrArray) To UBound(arrArray)
                If (Request.Form("id_" & arrArray(i)) = "on") Then
                    strContentIds = strContentIds & arrArray(i) & ","
                End If
            Next
            If (strContentIds <> "") Then
                strContentIds = Left(strContentIds, Len(strContentIds) - 1)
                m_refContent.SubmitForDeletev2_0(strContentIds, _FolderId)
            End If
            Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewContentByCategory&id=" & _FolderId & IIf(_CurrentPageId > 1, "&currentpage=" & _CurrentPageId, ""), False)
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub

#End Region

#Region "Display"

    Private Sub Display_Delete()

        _FolderData = _ContentApi.GetFolderById(_Id)
        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder")

        Select Case _FolderData.FolderType
            Case FolderType.Catalog
                Display_DeleteEntries()
            Case Else
                Display_DeleteContentByCategory()
        End Select

    End Sub

#Region "Entries"


    Private Sub Display_DeleteEntries()

        Dim CatalogManager As New CatalogEntry(_ContentApi.RequestInformationRef)
        Dim entryList As New System.Collections.Generic.List(Of EntryData)()
        Dim entryCriteria As New Ektron.Cms.Common.Criteria(Of EntryProperty)
        Dim totalPages As Integer = 1

        entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _Id)
        If _ContentApi.RequestInformationRef.ContentLanguage > 0 Then entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _ContentApi.RequestInformationRef.ContentLanguage)

        entryCriteria.OrderByDirection = OrderByDirection.Ascending
        entryCriteria.OrderByField = EntryProperty.Title

        entryCriteria.PagingInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize
        entryCriteria.PagingInfo.CurrentPage = _CurrentPageId

        If _ShowArchive = False Then
            entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, False)
        Else
            entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, True)
        End If

        entryList = CatalogManager.GetList(entryCriteria)

        _TotalPagesNumber = entryCriteria.PagingInfo.TotalPages

        DeleteContentByCategoryToolBar()

        PageSettings()

        Populate_DeleteCatalogGrid(entryList)

        folder_id.Value = _Id

    End Sub
    Private Sub Populate_DeleteCatalogGrid(ByVal entryList As System.Collections.Generic.List(Of EntryData))

        DeleteContentByGategoryGrid.Controls.Clear()
        contentids.Value = ""
        Dim colBound As System.Web.UI.WebControls.BoundColumn
        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "BOX"
        colBound.HeaderText = "<input type=""checkbox"" name=""all"" onclick=""javascript:checkAll(document.forms[0].all.checked);"">"
        colBound.ItemStyle.Width = Unit.Parse("1")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.HeaderStyle.CssClass = "title-header"
        DeleteContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=DeleteContentByCategory&orderby=Title&id=" & _Id & "&LangType=" & _ContentLanguage & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>" & _MessageHelper.GetMessage("generic Title") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        DeleteContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=DeleteContentByCategory&orderby=ID&id=" & _Id & "&LangType=" & _ContentLanguage & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>" & _MessageHelper.GetMessage("generic ID") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        DeleteContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STATUS"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=DeleteContentByCategory&orderby=status&id=" & _Id & "&LangType=" & _ContentLanguage & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>" & _MessageHelper.GetMessage("generic Status") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.Wrap = False
        DeleteContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DATEMODIFIED"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=DeleteContentByCategory&orderby=DateModified&id=" & _Id & "&LangType=" & _ContentLanguage & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>" & _MessageHelper.GetMessage("generic Date Modified") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        DeleteContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITORNAME"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=DeleteContentByCategory&orderby=editor&id=" & _Id & "&LangType=" & _ContentLanguage & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>" & _MessageHelper.GetMessage("generic Last Editor") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        DeleteContentByGategoryGrid.Columns.Add(colBound)


        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("BOX", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(Int64)))
        dt.Columns.Add(New DataColumn("STATUS", GetType(String)))
        dt.Columns.Add(New DataColumn("DATEMODIFIED", GetType(String)))
        dt.Columns.Add(New DataColumn("EDITORNAME", GetType(String)))

        Dim i As Integer
        For i = 0 To (entryList.Count - 1)
            dr = dt.NewRow()
            dr(0) = ""
            If (entryList(i).ContentStatus = "A") Then
                If _EkContent.IsAllowed(entryList(i).Id, entryList(i).LanguageId, "content", "delete") Then
                    If (contentids.Value = "") Then
                        contentids.Value = entryList(i).Id
                    Else
                        contentids.Value += "," & entryList(i).Id
                    End If
                    dr(0) = "<input type=""checkbox"" onclick=""checkAllFalse();"" name=""id_" & entryList(i).Id & """>"
                End If
            End If

            dr(1) = "<a href=""content.aspx?LangType=" & entryList(i).LanguageId & "&action=View&id=" & entryList(i).Id & """ title='" & _MessageHelper.GetMessage("generic View") & " """ & Replace(entryList(i).Title, "'", "`") & """" & "'>" & entryList(i).Title & "</a>"
            dr(2) = entryList(i).Id
            dr(3) = entryList(i).Status
            dr(4) = entryList(i).DateModified
            dr(5) = entryList(i).LastEditorLastName
            dt.Rows.Add(dr)
        Next i

        Dim dv As New DataView(dt)
        DeleteContentByGategoryGrid.DataSource = dv
        DeleteContentByGategoryGrid.DataBind()
    End Sub
    'Private Sub DeleteContentByCategoryToolBar()
    '    Dim result As New System.Text.StringBuilder

    '    txtTitleBar.InnerHtml = m_refStyle.GetTitleBar("Delete Contents of Folder " & " """ & folder_data.Name & """")
    '    result.Append("<table border=""0"" cellspacing=""0"" cellpadding=""0"" ID=""Table14""></tr>")
    '    If (security_data.IsAdmin _
    '     OrElse m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(folder_data.Id) _
    '     OrElse m_refContent.IsAllowed(folder_data.Id, m_refContentApi.RequestInformationRef.ContentLanguage, "folder", "delete")) Then
    '        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_delete_content-nm.gif", "#", "Delete Content", m_refMsg.GetMessage("btn delete content"), "OnClick=""javascript:checkDeleteForm();"""))
    '    End If
    '    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_back-nm.gif", "content.aspx?LangType=" & ContentLanguage & "&action=ViewContentByCategory&id=" & m_intId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
    '    result.Append("<td>")
    '    result.Append(m_refStyle.GetHelpButton(m_refStyle.GetHelpAliasPrefix(folder_data) & m_strPageAction))
    '    result.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>")
    '    result.Append("</tr></table>")
    '    htmToolBar.InnerHtml = result.ToString
    'End Sub


#End Region

#Region "Content"


    Private Sub Display_DeleteContentByCategory()

        Dim contentdataAll As New Ektron.Cms.Common.EkContentCol

        DeleteContentByCategoryToolBar()

        _PageData = New Collection
        _PageData.Add(_Id, "FolderID")
        _PageData.Add(_OrderBy, "OrderBy")
        If (_ShowArchive = False) Then
            contentdataAll = _EkContent.GetAllViewableChildContentInfoV5_0(_PageData, _CurrentPageId, _ContentApi.RequestInformationRef.PagingSize, _TotalPagesNumber)
        Else
            contentdataAll = _EkContent.GetAllViewArchiveContentInfov5_0(_PageData, _CurrentPageId, _ContentApi.RequestInformationRef.PagingSize, _TotalPagesNumber)
        End If

        Dim filteredcontentdata As New Ektron.Cms.Common.EkContentCol
        For Each item As Ektron.Cms.Common.ContentBase In contentdataAll
            If (item.ContentSubType <> CMSContentSubtype.PageBuilderMasterData) Then
                filteredcontentdata.Add(item)
            End If
        Next
        contentdataAll = filteredcontentdata

        PageSettings()

        Populate_DeleteContentByCategory(contentdataAll)

        folder_id.Value = _Id

    End Sub
    Private Sub Populate_DeleteContentByCategory(ByVal contentdata As Ektron.Cms.Common.EkContentCol)
        DeleteContentByGategoryGrid.Controls.Clear()
        contentids.Value = ""
        Dim colBound As System.Web.UI.WebControls.BoundColumn
        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "BOX"
        colBound.HeaderText = "<input type=""checkbox"" name=""all"" onclick=""javascript:checkAll(document.forms[0].all.checked);"">"
        colBound.ItemStyle.Width = Unit.Parse("1")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.HeaderStyle.CssClass = "title-header"
        DeleteContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=DeleteContentByCategory&orderby=Title&id=" & _Id & "&LangType=" & _ContentLanguage & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>" & _MessageHelper.GetMessage("generic Title") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        DeleteContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=DeleteContentByCategory&orderby=ID&id=" & _Id & "&LangType=" & _ContentLanguage & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>" & _MessageHelper.GetMessage("generic ID") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        DeleteContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STATUS"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=DeleteContentByCategory&orderby=status&id=" & _Id & "&LangType=" & _ContentLanguage & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>" & _MessageHelper.GetMessage("generic Status") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.Wrap = False
        DeleteContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DATEMODIFIED"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=DeleteContentByCategory&orderby=DateModified&id=" & _Id & "&LangType=" & _ContentLanguage & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>" & _MessageHelper.GetMessage("generic Date Modified") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        DeleteContentByGategoryGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITORNAME"
        colBound.HeaderText = "<a class=""title-header"" href=""content.aspx?action=DeleteContentByCategory&orderby=editor&id=" & _Id & "&LangType=" & _ContentLanguage & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>" & _MessageHelper.GetMessage("generic Last Editor") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        DeleteContentByGategoryGrid.Columns.Add(colBound)


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
            dr = dt.NewRow()
            dr(0) = ""
            If (contentdata.Item(i).ContentStatus = "A") OrElse (contentdata.Item(i).ContentStatus = "I") Then
                If _EkContent.IsAllowed(contentdata.Item(i).Id, contentdata.Item(i).Language, "content", "delete") Then
                    If (contentids.Value = "") Then
                        contentids.Value = contentdata.Item(i).Id
                    Else
                        contentids.Value += "," & contentdata.Item(i).Id
                    End If
                    dr(0) = "<input type=""checkbox"" onclick=""javascript:checkAllFalse();"" name=""id_" & contentdata.Item(i).Id & """>"
                End If
            End If

            dr(1) = "<a href=""content.aspx?LangType=" & contentdata.Item(i).Language & "&action=View&id=" & contentdata.Item(i).Id & """ title='" & _MessageHelper.GetMessage("generic View") & " """ & Replace(contentdata.Item(i).Title, "'", "`") & """" & "'>" & contentdata.Item(i).Title & "</a>"
            dr(2) = contentdata.Item(i).Id
            dr(3) = contentdata.Item(i).Status
            dr(4) = contentdata.Item(i).DisplayDateModified
            dr(5) = contentdata.Item(i).LastEditorLname
            dt.Rows.Add(dr)
        Next i

        Dim dv As New DataView(dt)
        DeleteContentByGategoryGrid.DataSource = dv
        DeleteContentByGategoryGrid.DataBind()
    End Sub
    Private Sub DeleteContentByCategoryToolBar()
        Dim result As New System.Text.StringBuilder

        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar("Delete Contents of Folder " & " """ & _FolderData.Name & """")
        result.Append("<table></tr>")
        If (_PermissionData.IsAdmin _
         OrElse _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_FolderData.Id) _
         OrElse _EkContent.IsAllowed(_FolderData.Id, _ContentApi.RequestInformationRef.ContentLanguage, "folder", "delete")) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/delete.png", "#", "Delete Content", _MessageHelper.GetMessage("btn delete content"), "OnClick=""javascript:checkDeleteForm();"""))
        End If
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/back.png", "content.aspx?LangType=" & _ContentLanguage & "&action=ViewContentByCategory&id=" & _Id, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton(_StyleHelper.GetHelpAliasPrefix(_FolderData) & _PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub



#End Region

#End Region

#Region "Paging"


    Private Sub PageSettings()

        If _TotalPagesNumber > 1 Then Me.SetPagingUI()

        'If (_TotalPagesNumber <= 1) Then
        '    VisiblePageControls(False)
        '    page_id.Value = 1
        'Else
        '    VisiblePageControls(True)
        '    TotalPages.Text = (System.Math.Ceiling(_TotalPagesNumber)).ToString()
        '    'hTotalPages.Value = (System.Math.Ceiling(m_intTotalPages)).ToString()
        '    CurrentPage.Text = _CurrentPageId.ToString()
        '    'hCurrentPage.Value = m_intCurrentPage.ToString()
        '    ctrlPreviousPage.Enabled = True
        '    ctrlFirstPage.Enabled = True
        '    ctrlLastPage.Enabled = True
        '    ctrlNextPage.Enabled = True
        '    If _CurrentPageId = 1 Then
        '        ctrlPreviousPage.Enabled = False
        '        ctrlFirstPage.Enabled = False
        '    ElseIf _CurrentPageId = _TotalPagesNumber Then
        '        ctrlNextPage.Enabled = False
        '        ctrlLastPage.Enabled = False
        '    End If
        '    page_id.Value = _CurrentPageId
        'End If
    End Sub

    Private Sub SetPagingUI()

        'paging ui
        divPaging.Visible = True

        litPage.Text = "Page"
        CurrentPage.Text = IIf(_CurrentPageId = 0, "1", _CurrentPageId.ToString())
        Dim previousPageIndex As Integer = IIf(_CurrentPageId <= 1, 1, _CurrentPageId - 1)
        Dim nextPageIndex As Integer = IIf(_CurrentPageId >= _TotalPagesNumber, _TotalPagesNumber, _CurrentPageId + 1)
        litOf.Text = "of"
        TotalPages.Text = _TotalPagesNumber.ToString()

        ibFirstPage.ImageUrl = _ContentApi.ApplicationPath & "/images/ui/icons/arrowheadFirst.png"
        ibFirstPage.AlternateText = "First Page"
        ibFirstPage.ToolTip = "First Page"
        ibFirstPage.OnClientClick = "GoToDeletePage(1, " & _TotalPagesNumber.ToString() & "); return false;"

        ibPreviousPage.ImageUrl = _ContentApi.ApplicationPath & "/images/ui/icons/arrowheadLeft.png"
        ibPreviousPage.AlternateText = "Previous Page"
        ibPreviousPage.ToolTip = "Previous Page"
        ibPreviousPage.OnClientClick = "GoToDeletePage(" & previousPageIndex.ToString() & ", " & _TotalPagesNumber.ToString() & "); return false;"

        ibNextPage.ImageUrl = _ContentApi.ApplicationPath & "/images/ui/icons/arrowheadRight.png"
        ibNextPage.AlternateText = "Next Page"
        ibNextPage.ToolTip = "Next Page"
        ibNextPage.OnClientClick = "GoToDeletePage(" & nextPageIndex.ToString() & ", " & _TotalPagesNumber.ToString() & "); return false;"

        ibLastPage.ImageUrl = _ContentApi.ApplicationPath & "/images/ui/icons/arrowheadLast.png"
        ibLastPage.AlternateText = "Last Page"
        ibLastPage.ToolTip = "Last Page"
        ibLastPage.OnClientClick = "GoToDeletePage(" & _TotalPagesNumber.ToString() & ", " & _TotalPagesNumber.ToString() & "); return false;"

        ibPageGo.ImageUrl = _ContentApi.ApplicationPath & "/images/ui/icons/forward.png"
        ibPageGo.AlternateText = "Go To Page"
        ibPageGo.ToolTip = "Go To Page"
        ibPageGo.OnClientClick = "GoToDeletePage(document.getElementById('" & Me.CurrentPage.ClientID & "').value, " & _TotalPagesNumber.ToString() & ");return false;"

    End Sub

    Private Sub VisiblePageControls(ByVal flag As Boolean)
        'TotalPages.Visible = flag
        'CurrentPage.Visible = flag
        'ctrlPreviousPage.Visible = flag
        'ctrlNextPage.Visible = flag
        'ctrlLastPage.Visible = flag
        'ctrlFirstPage.Visible = flag
        'PageLabel.Visible = flag
        'OfLabel.Visible = flag
    End Sub
    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                _CurrentPageId = 1
            Case "Last"
                _CurrentPageId = Int32.Parse(TotalPages.Text)
            Case "Next"
                _CurrentPageId = Int32.Parse(CurrentPage.Text) + 1
            Case "Prev"
                _CurrentPageId = Int32.Parse(CurrentPage.Text) - 1
        End Select
        Display_Delete()
        isPostData.Value = "true"
    End Sub

    Protected Function Util_GetPageURL(ByVal pageid As Integer) As String

        Return "content.aspx" & Ektron.Cms.Common.EkFunctions.GetUrl(New String() {"currentpage"}, New String() {"pageid"}, Request.QueryString).Replace("pageid", IIf(pageid = -1, "' + pageid + '", pageid)).Replace("&amp;", "&")

    End Function

#End Region

#Region "JS/CSS"

    Private Sub RegisterJS()

        JS.RegisterJS(Me, JS.ManagedScript.EktronJS)
        JS.RegisterJS(Me, JS.ManagedScript.EktronWorkareaJS)
        JS.RegisterJS(Me, JS.ManagedScript.EktronWorkareaHelperJS)

        browseURLjs.text = "content.aspx?LangType=" & Me._ContentLanguage & "&action=DeleteContentByCategory&id=" & Me._Id & "&currentpage="
        pagebetweenjs.text = String.Format(_MessageHelper.GetMessage("js: err page must be between"), _TotalPagesNumber)

    End Sub

    Private Sub RegisterCSS()

        Css.RegisterCss(Me, Css.ManagedStyleSheet.EktronWorkareaCss)
        Css.RegisterCss(Me, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7)

    End Sub

#End Region

End Class
