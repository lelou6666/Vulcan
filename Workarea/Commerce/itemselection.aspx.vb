Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Workarea
Imports System.Data

Partial Class Commerce_itemselection
    Inherits workareabase

    Protected m_sPageName As String = "itemselection.aspx"
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1
    Protected strContType As String
    Protected ContentTypeUrlParam As String = Ektron.Cms.Common.EkConstants.ContentTypeUrlParam
    Protected objLocalizationApi As LocalizationAPI = Nothing
    Protected security_data As PermissionData
    Protected GetProducts As Boolean = True
    Protected GetComplexProducts As Boolean = True
    Protected GetKits As Boolean = True
    Protected GetBundles As Boolean = True
    Protected excludeId As Long = 0
    Protected bTabUseCase As Boolean = False

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If

        If Request.QueryString("SelectedTab") IsNot Nothing AndAlso Request.QueryString("SelectedTab") <> "" Then bTabUseCase = True
        If Request.QueryString("exclude") <> "" Then excludeId = Request.QueryString("exclude")

        Try

            If Not Util_CheckAccess() Then Throw New Exception("No permission")
            If (m_refContentApi.RequestInformationRef.IsMembershipUser) Then
                Response.Redirect(m_refContentApi.ApplicationPath & "reterror.aspx?info=" & m_refContentApi.EkMsgRef.GetMessage("msg login cms user"), False)
                Exit Sub
            End If

            If Request.QueryString(ContentTypeUrlParam) <> "" Then If IsNumeric(Request.QueryString(ContentTypeUrlParam)) Then strContType = Request.QueryString(ContentTypeUrlParam)

            If Page.IsPostBack = False Then Display_ViewAll()

            SetLabels()

        Catch ex As Exception

            Utilities.ShowError(ex.Message)

        End Try

    End Sub

#Region "Display"

    Protected Sub Display_ViewAll()
        Dim CatalogManager As New CatalogEntry(m_refContentApi.RequestInformationRef)
        Dim entryList As New System.Collections.Generic.List(Of EntryData)()
        Dim entryCriteria As New Ektron.Cms.Common.Criteria(Of EntryProperty)

        entryCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        entryCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_iID)
        entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.ContentLanguage)
        entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, False)
        entryCriteria.AddFilter(EntryProperty.IsPublished, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, True)

        entryCriteria.OrderByDirection = OrderByDirection.Ascending
        entryCriteria.OrderByField = Util_GetSortColumn()

        Select Case m_sPageAction
            Case "browsecrosssell", "browseupsell", "couponselect"

                ' If m_sPageAction = "couponselect" Then entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, CatalogEntryType.SubscriptionProduct)
                entryList = CatalogManager.GetList(entryCriteria)

            Case "browse"

                Dim IdList(2) As Long

                IdList(0) = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product
                ' IdList(1) = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct
                entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.In, IdList)
                If excludeId > 0 Then entryCriteria.AddFilter(EntryProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, excludeId)
                entryList = CatalogManager.GetList(entryCriteria)

            Case Else

                pnl_catalogs.Visible = True
                pnl_viewall.Visible = False
                Dim catalogList As New System.Collections.Generic.List(Of CatalogData)
                catalogList = CatalogManager.GetCatalogList(1, 1)
                Util_ShowCatalogs(catalogList)

        End Select

        TotalPagesNumber = entryCriteria.PagingInfo.TotalPages

        If (TotalPagesNumber <= 1) Then
            TotalPages.Visible = False
            CurrentPage.Visible = False
            lnkBtnPreviousPage.Visible = False
            NextPage.Visible = False
            LastPage.Visible = False
            FirstPage.Visible = False
            PageLabel.Visible = False
            OfLabel.Visible = False
        Else
            lnkBtnPreviousPage.Enabled = True
            FirstPage.Enabled = True
            LastPage.Enabled = True
            NextPage.Enabled = True
            TotalPages.Visible = True
            CurrentPage.Visible = True
            lnkBtnPreviousPage.Visible = True
            NextPage.Visible = True
            LastPage.Visible = True
            FirstPage.Visible = True
            PageLabel.Visible = True
            OfLabel.Visible = True

            TotalPages.Text = (System.Math.Ceiling(TotalPagesNumber)).ToString()

            CurrentPage.Text = _currentPageNumber.ToString()

            If _currentPageNumber = 1 Then
                lnkBtnPreviousPage.Enabled = False
                FirstPage.Enabled = False
            ElseIf _currentPageNumber = TotalPagesNumber Then
                NextPage.Enabled = False
                LastPage.Enabled = False
            End If
        End If
        Populate_ViewCatalogGrid(entryList)
    End Sub

#End Region

#Region "Process"

#End Region

#Region "Private Helpers"

    Private Sub Util_ShowCatalogs(ByVal catalogList As System.Collections.Generic.List(Of CatalogData))
        Dim i As Integer = 0
        Dim m_refContentApi As New ContentAPI

        CatalogGrid.DataSource = catalogList
        CatalogGrid.DataBind()

        'For i = 0 To CatalogData.Count - 1
        '    ltr_folder.Text += "<table><tr><td><img src=""" & m_refContentApi.AppPath & "Tree/images/xp/catalogplusclosefolder.gif""/></td>"
        '    ltr_folder.Text += "<td><a href=""byproduct.aspx?action=" & m_sPageAction & "select&id=" & CatalogData.Item(i).CatalogId & """>" & CatalogData.Item(i).CatalogName & "</a></td></tr></table>"
        'Next
    End Sub
    Protected Sub SetLabels()
        ' Me.SetTitleBarToMessage("lbl entry selection")
        If Request.QueryString("action") IsNot Nothing Then
            If Request.QueryString("action") = "browse" Or Request.QueryString("action") = "browsecrosssell" Or Request.QueryString("action") = "browseupsell" Then
                Me.AddBackButton("javascript:history.go(-1);")
            End If
        End If
        Me.AddHelpButton("itemselection")
    End Sub
    Private Sub Populate_ViewCatalogGrid(ByVal entryList As System.Collections.Generic.List(Of EntryData))
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strTag As String
        Dim strtag1 As String

        If m_sPageAction = "couponselect" Then
            Me.AddBackButton("javascript:history.go(-1);")
        End If

        strTag = "<a href=""itemselection.aspx?LangType=" & ContentLanguage & IIf(bTabUseCase, "&SelectedTab=Items", "") & "&action=" & m_sPageAction & "&orderby="
        strtag1 = "&id=" & m_iID & IIf(strContType <> "", "&" & ContentTypeUrlParam & "=" & strContType, "") & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>"

        colBound.DataField = "TITLE"
        colBound.HeaderText = strTag & "Title" & strtag1 & m_refMsg.GetMessage("generic Title") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LANGUAGE"
        colBound.HeaderText = strTag & "language" & strtag1 & m_refMsg.GetMessage("generic language") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = strTag & "ID" & strtag1 & m_refMsg.GetMessage("generic ID") & "</a>"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.HeaderStyle.CssClass = "title-header"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STATUS"
        colBound.HeaderText = m_refMsg.GetMessage("generic Status")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.Wrap = False
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TYPE"
        colBound.HeaderText = strTag & "type" & strtag1 & m_refMsg.GetMessage("generic type") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "PRICE"
        colBound.HeaderText = strTag & "price" & strtag1 & m_refMsg.GetMessage("lbl calatog entry price") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        FolderDataGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(Int64)))
        dt.Columns.Add(New DataColumn("STATUS", GetType(String)))
        dt.Columns.Add(New DataColumn("TYPE", GetType(String)))
        dt.Columns.Add(New DataColumn("PRICE", GetType(String)))
        Dim ViewUrl As String = ""
        Dim EditUrl As String = ""
        Dim i As Integer
        Dim bAssetItem As Boolean = False

        objLocalizationApi = New LocalizationAPI()

        For i = 0 To (entryList.Count - 1)
            dr = dt.NewRow()
            Dim dmsMenuGuid As String
            dmsMenuGuid = System.Guid.NewGuid().ToString()

            dr(0) = dr(0) + "<a"
            dr(0) = dr(0) + " id=""dmsViewItemAnchor" & entryList(i).Id & entryList(i).LanguageId & dmsMenuGuid & """"
            dr(0) = dr(0) + " class=""dmsViewItemAnchor"""
            dr(0) = dr(0) + " onclick=""" & Util_GetLinkJS(entryList(i)) & """"
            dr(0) = dr(0) + " href=""#"""
            dr(0) = dr(0) + " title=""View " & entryList(i).Title & """"
            dr(0) = dr(0) + ">"
            Select Case entryList(i).EntryType

                Case CatalogEntryType.SubscriptionProduct

                    dr(0) = dr(0) + "<img border=""0"" src=""" & m_refContentApi.AppPath & "images/ui/icons/bookGreen.png" & """></img>" & entryList(i).Title

                Case CatalogEntryType.ComplexProduct

                    dr(0) = dr(0) + "<img border=""0"" src=""" & m_refContentApi.AppPath & "images/ui/icons/bricks.png" & """></img>" & entryList(i).Title

                Case CatalogEntryType.Kit

                    dr(0) = dr(0) + "<img border=""0"" src=""" & m_refContentApi.AppPath & "images/ui/icons/box.png" & """></img>" & entryList(i).Title

                Case CatalogEntryType.Bundle

                    dr(0) = dr(0) + "<img border=""0"" src=""" & m_refContentApi.AppPath & "images/ui/icons/package.png" & """></img>" & entryList(i).Title

                Case Else ' Product

                    dr(0) = dr(0) + "<img border=""0"" src=""" & m_refContentApi.AppPath & "images/ui/icons/brick.png" & """></img>" & entryList(i).Title

            End Select
            dr(0) = dr(0) + "</a>"
            'dr(0) = dr(0) + "</p>"
            'dr(0) = dr(0) + "</div>"

            dr(1) = "<a href=""Javascript://"" style=""text-decoration:none;"">" & "<img src='" & objLocalizationApi.GetFlagUrlByLanguageID(entryList(i).LanguageId) & "' border=""0"" />" & "</a>"
            dr(2) = entryList(i).Id
            dr(3) = (entryList(i).Status)
            dr(4) = entryList(i).EntryType.ToString
            dr(5) = FormatNumber(entryList(i).ListPrice, 2)
            dt.Rows.Add(dr)
        Next i

        Dim dv As New DataView(dt)
        FolderDataGrid.DataSource = dv
        FolderDataGrid.DataBind()
    End Sub
    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                _currentPageNumber = 1
            Case "Last"
                _currentPageNumber = Int32.Parse(TotalPages.Text)
            Case "Next"
                _currentPageNumber = Int32.Parse(CurrentPage.Text) + 1
            Case "Prev"
                _currentPageNumber = Int32.Parse(CurrentPage.Text) - 1
        End Select
        Display_ViewAll()
        isPostData.Value = "true"
    End Sub
    Protected Function Util_CheckAccess() As Boolean
        security_data = m_refContentApi.LoadPermissions(Me.m_iID, "folder")
        Return security_data.IsReadOnly
    End Function
    Protected Function Util_GetLinkJS(ByVal entryList As EntryData) As String
        Dim sRet As String = ""

        If bTabUseCase = True Then
            Select Case Request.QueryString("SelectedTab").ToLower()
                Case "items"
                    sRet = "parent.Ektron.Commerce.CatalogEntry.Items.DefaultView.Add.addItem('" & entryList.Id & "', '" & entryList.Title & "');"
            End Select
        Else
            Select Case m_sPageAction
                Case "couponselect"
                    sRet = "parent.addRowToTable(null, " & entryList.Id & ", " & entryList.LanguageId & ", '" & entryList.Title & "', " & entryList.EntryType & "); parent.ektb_remove();"
                Case "coupon"
                    sRet = ""
                Case "crosssell", "upsell"
                    sRet = "parent.addRowToTable(null, " & entryList.Id & ", '" & entryList.Title & "'); parent.ektb_remove();"
                Case Else
                    sRet = "parent.addRowToTable(null, " & entryList.Id & ", '" & entryList.Title & "'); parent.ektb_remove();"
            End Select
        End If

        Return sRet
    End Function

    Protected Function Util_GetSortColumn() As EntryProperty

        Dim sort As String = ""

        If Request.QueryString("orderby") <> "" Then sort = Request.QueryString("orderby").ToLower()

        Select Case sort
            Case "language"
                Util_GetSortColumn = EntryProperty.LanguageId
            Case "id"
                Util_GetSortColumn = EntryProperty.Id
            Case "type"
                Util_GetSortColumn = EntryProperty.EntryType
            Case "price"
                Util_GetSortColumn = EntryProperty.SalePrice
            Case Else
                Util_GetSortColumn = EntryProperty.Title
        End Select

    End Function

#End Region

End Class
