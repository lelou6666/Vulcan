Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Workarea
Imports System.Data
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Partial Class Commerce_byproduct
    Inherits workareabase

    Protected m_selectedFolderList As String = ""
    Protected GetProducts As Boolean = True
    Protected GetBundles As Boolean = True
    Protected GetComplexProducts As Boolean = True
    Protected GetKits As Boolean = True
    Protected TotalPagesNumber As Integer = 1
    Protected _currentPageNumber As Integer = 1
    Protected strContType As String
    Protected objLocalizationApi As LocalizationAPI = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        Dim CatalogManager As New CatalogEntry(m_refContentApi.RequestInformationRef)
        Dim catalogData As New System.Collections.Generic.List(Of CatalogData)

        TotalPages.Visible = False
        CurrentPage.Visible = False
        PageLabel.Visible = False
        OfLabel.Visible = False

        Select Case m_sPageAction
            Case "crosssellselect", "upsellselect", "couponselect", "select", "reportingselect"
                Display_ViewAll()
            Case Else
                catalogData = CatalogManager.GetCatalogList(1, 1)
                populateFolder(catalogData)
        End Select
    End Sub
    Protected Sub Display_ViewAll()
        Dim CatalogManager As New CatalogEntryApi()
        Dim entryList As New System.Collections.Generic.List(Of EntryData)()
        Dim entryCriteria As New Ektron.Cms.Common.Criteria(Of EntryProperty)
        Dim OrderBy As String = ""

        entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_iID)
        entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.ContentLanguage)

        If (Request.QueryString("orderby") <> Nothing) Then
            OrderBy = Request.QueryString("orderby")
        End If

        Select Case m_sPageAction
            Case "couponselect", "crosssellselect", "upsellselect", "reportingselect"
                ' do nothing
            Case Else
                GetProducts = True
                GetBundles = False
                GetComplexProducts = False
                GetKits = False
                Dim IdList(1) As Long
                IdList(0) = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product
                entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.In, IdList)
        End Select

        Select Case OrderBy.ToLower()
            Case "title"
                entryCriteria.OrderByField = EntryProperty.Title
            Case "language"
                entryCriteria.OrderByField = EntryProperty.LanguageId
            Case "id"
                entryCriteria.OrderByField = EntryProperty.Id
            Case "status"
                entryCriteria.OrderByField = EntryProperty.Status
            Case "datemodified"
                entryCriteria.OrderByField = EntryProperty.EntryType
            Case "editor"
                entryCriteria.OrderByField = EntryProperty.ListPrice
            Case Else
                entryCriteria.OrderByField = EntryProperty.LastEditDate
        End Select

        entryCriteria.PagingInfo.CurrentPage = Me.ucPaging.SelectedPage + 1
        entryList = CatalogManager.GetList(entryCriteria)

        If (entryCriteria.PagingInfo.TotalPages > 1) Then
            Me.ucPaging.Visible = True
            Me.ucPaging.TotalPages = entryCriteria.PagingInfo.TotalPages
            Me.ucPaging.CurrentPageIndex = Me.ucPaging.SelectedPage
        End If

        If (TotalPagesNumber <= 1) Then
            TotalPages.Visible = False
            CurrentPage.Visible = False
            PageLabel.Visible = False
            OfLabel.Visible = False
        Else
            TotalPages.Visible = True
            CurrentPage.Visible = True
            PageLabel.Visible = True
            OfLabel.Visible = True
            TotalPages.Text = (System.Math.Ceiling(TotalPagesNumber)).ToString()
            CurrentPage.Text = _currentPageNumber.ToString()
        End If

        Populate_ViewCatalogGrid(entryList)
    End Sub
    Protected Function Util_GetLinkJS(ByVal entryList As EntryData) As String
        Dim sRet As String = ""
        Select Case m_sPageAction
            Case "couponselect"
                sRet = "parent.addRowToTable(null, " & entryList.Id & ", " & entryList.LanguageId & ", '" & entryList.Title & "', " & entryList.EntryType & "); parent.ektb_remove();"
            Case "crosssellselect", "upsellselect"
                sRet = "parent.addRowToTable(null, " & entryList.Id & ", '" & entryList.Title & "'); parent.ektb_remove();"
            Case Else
                sRet = "parent.location.href='fulfillment.aspx?action=byproduct&productid=" & entryList.Id & "'; "
        End Select
        Return sRet
    End Function
    Private Sub Populate_ViewCatalogGrid(ByVal entryList As System.Collections.Generic.List(Of EntryData))
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strTag As String
        Dim strtag1 As String

        If Request.QueryString("action") IsNot Nothing Then
            If Request.QueryString("action") = "reportingselect" Then
                Me.AddBackButton("javascript:history.go(-1);")
            End If
        End If

        strTag = "<a href=""byproduct.aspx?LangType=" & ContentLanguage & "&action=" & m_sPageAction & "&orderby="
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
        colBound.HeaderText = strTag & "status" & strtag1 & m_refMsg.GetMessage("generic Status") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.Wrap = False
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TYPE"
        colBound.HeaderText = strTag & "DateModified" & strtag1 & m_refMsg.GetMessage("generic type") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "PRICE"
        colBound.HeaderText = strTag & "editor" & strtag1 & m_refMsg.GetMessage("lbl calatog entry price") & "</a>"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        FolderDataGrid.Columns.Add(colBound)
        FolderDataGrid.BorderColor = Drawing.Color.White

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
            'dr(0) = dr(0) + " title=""View " & entryList(i).Title & """"
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

            dr(1) = "<img src='" & objLocalizationApi.GetFlagUrlByLanguageID(entryList(i).LanguageId) & "' border=""0"" />"
            dr(2) = entryList(i).Id
            dr(3) = m_refStyle.StatusWithToolTip(entryList(i).Status)
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
    End Sub
    Private Sub populateFolder(ByVal catalogData As System.Collections.Generic.List(Of CatalogData))
        ltr_folder.Text = Nothing
        Dim i As Integer = 0
        Dim m_refContentApi As New ContentAPI
        Dim imageFolder As String = m_refContentApi.AppPath & "images/ui/icons/tree/folderCollapsed.png"
        Dim CatalogManager As New CatalogEntryApi()
        Dim entryList As New System.Collections.Generic.List(Of EntryData)()


        For i = 0 To catalogData.Count - 1

            'entryList = CatalogManager.GetCatalogEntries(catalogData.Item(i).Id, m_refContentApi.RequestInformationRef.ContentLanguage, New EntryTypeFilter(GetProducts, GetBundles, GetComplexProducts, GetKits))
            'If entryList.Count = 0 Then
            imageFolder = m_refContentApi.AppPath & "images/ui/icons/folderGreen.png"
            'End If
            If m_sPageAction = "coupon" Then
                ltr_folder.Text += "<table><tr><td><a href=""itemselection.aspx?action=" & m_sPageAction & "select&id=" & catalogData.Item(i).Id & """><img style=""border:0px;"" src=""" & imageFolder & """/></a></td>"
            Else
                ltr_folder.Text += "<table><tr><td><a href=""byproduct.aspx?action=" & m_sPageAction & "select&id=" & catalogData.Item(i).Id & """><img style=""border:0px;"" src=""" & imageFolder & """/></a></td>"
            End If
            If m_sPageAction = "coupon" Then
                ltr_folder.Text += "<td><a href=""itemselection.aspx?action=" & m_sPageAction & "select&id=" & catalogData.Item(i).Id & """>" & catalogData.Item(i).Name & "</a></td></tr></table>"
            Else
                ltr_folder.Text += "<td><a href=""byproduct.aspx?action=" & m_sPageAction & "select&id=" & catalogData.Item(i).Id & """>" & catalogData.Item(i).Name & "</a></td></tr></table>"
            End If
        Next
    End Sub
End Class
