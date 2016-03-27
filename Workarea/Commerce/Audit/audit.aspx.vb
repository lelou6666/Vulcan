Imports Ektron.Cms.Workarea
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.common
Imports System.Collections.generic

Partial Class Commerce_Audit_Audit
    Inherits workareabase

#Region "Member Variables"

    Protected m_sPageName As String = "audit.aspx"
    Protected m_orderId As Long = 0
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1
    Protected sortcriteria As CommerceAuditProperty = CommerceAuditProperty.DateCreated
    Protected sortdirection As EkEnumeration.OrderByDirection = EkEnumeration.OrderByDirection.Descending
    Protected searchCriteria As String = ""
    Protected searchField As CommerceAuditProperty = CommerceAuditProperty.Message

#End Region

#Region "Events"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        'register page components
        Me.RegisterCSS()
        Me.RegisterJS()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Util_CheckAccess()
            If Request.QueryString("order") <> "" Then m_orderId = Request.QueryString("folder")
            If Page.Request.QueryString("search") <> "" Then searchCriteria = Page.Request.QueryString("search")
            If Page.Request.QueryString("searchfield") <> "" Then searchField = Page.Request.QueryString("searchfield")
            Select Case MyBase.m_sPageAction
                Case Else
                    If Page.IsPostBack = False Then Display_Audit()
            End Select
            Util_SetLabels()
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

#End Region

#Region "Display"


    Protected Sub Display_Audit()

        Dim auditApi As New CommerceAuditApi()

        Dim auditCriteria As New Ektron.Cms.Common.Criteria(Of CommerceAuditProperty)

        auditCriteria.OrderByField = sortcriteria
        auditCriteria.OrderByDirection = sortdirection

        auditCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()
        auditCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize

        If searchCriteria <> "" Then
            Select Case searchField
                Case CommerceAuditProperty.DateCreated
                    Dim searchDate As DateTime = DateTime.Now
                    If DateTime.TryParse(searchCriteria, searchDate) AndAlso Not (searchDate = DateTime.MinValue) Then
                        auditCriteria.AddFilter(searchField, CriteriaFilterOperator.GreaterThanOrEqualTo, searchDate.Date)
                        auditCriteria.AddFilter(searchField, CriteriaFilterOperator.LessThan, searchDate.Date.AddDays(1))
                    End If
                Case CommerceAuditProperty.OrderId, CommerceAuditProperty.UserId
                    Dim searchId As Long = 0
                    If Long.TryParse(searchCriteria, searchId) Then
                        auditCriteria.AddFilter(searchField, CriteriaFilterOperator.EqualTo, searchId)
                    End If
                Case Else ' CommerceAuditProperty.IPAddress, CommerceAuditProperty.FormattedMessage
                    auditCriteria.AddFilter(searchField, CriteriaFilterOperator.Contains, searchCriteria)
            End Select
        End If

        dg_audit.DataSource = auditApi.GetList(auditCriteria)

        TotalPagesNumber = auditCriteria.PagingInfo.TotalPages

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

        ' dg_audit.Columns(0).HeaderText = GetMessage("generic id")
        dg_audit.Columns(0).HeaderText = GetMessage("lbl generic date")
        ' dg_audit.Columns(1).HeaderText = GetMessage("generic type")
        dg_audit.Columns(1).HeaderText = GetMessage("lbl ip address")
        dg_audit.Columns(2).HeaderText = GetMessage("lbl desc")
        dg_audit.Columns(3).HeaderText = GetMessage("lbl order id")
        dg_audit.Columns(4).HeaderText = GetMessage("lbl user id")

        dg_audit.DataBind()

    End Sub


#End Region

#Region "Process"


    Protected Sub Process_()



    End Sub


#End Region

#Region "Util"


    Protected Function Util_ShowLocal(ByVal GMTDatetime As DateTime) As String

        Dim localDateTime As DateTime = GMTDatetime.ToLocalTime()

        Return localDateTime.ToShortDateString() & " " & localDateTime.ToShortTimeString()

    End Function

    Protected Sub Util_SetLabels()

        Select Case MyBase.m_sPageAction

            Case Else

                Dim items As New ListItemCollection()
                items.Add(New ListItem(GetMessage("lbl generic date"), CommerceAuditProperty.DateCreated.GetHashCode()))
                items.Add(New ListItem(GetMessage("lbl order id"), CommerceAuditProperty.OrderId.GetHashCode()))
                items.Add(New ListItem(GetMessage("lbl user id"), CommerceAuditProperty.UserId.GetHashCode()))
                items.Add(New ListItem(GetMessage("lbl ip address"), CommerceAuditProperty.IPAddress.GetHashCode()))
                items.Add(New ListItem(GetMessage("lbl desc"), CommerceAuditProperty.FormattedMessage.GetHashCode()))

                If (searchField = CommerceAuditProperty.DateCreated) Then items(0).Selected = True
                If (searchField = CommerceAuditProperty.OrderId) Then items(1).Selected = True
                If (searchField = CommerceAuditProperty.UserId) Then items(2).Selected = True
                If (searchField = CommerceAuditProperty.IPAddress) Then items(3).Selected = True
                If (searchField = CommerceAuditProperty.FormattedMessage) Then items(4).Selected = True

                Me.AddSearchBox(searchCriteria, items, "searchAudit")
                Me.SetTitleBarToMessage("lbl commerce audit")

        End Select

        AddHelpButton("commerceaudit")

        Util_SetJs()

    End Sub

    Private Sub Util_SetJs()

        Dim sbJS As New StringBuilder

        sbJS.Append("<script language=""javascript"" type=""text/javascript"" >" & Environment.NewLine)

        sbJS.Append(" function searchAudit() { ").Append(Environment.NewLine)
        sbJS.Append("   var sSearchTerm = $ektron('#txtSearch').val(); ").Append(Environment.NewLine)
        sbJS.Append("   var iSearchField = $ektron('#searchlist').val(); ").Append(Environment.NewLine)
        sbJS.Append("   if (sSearchTerm != '') { window.location.href = '").Append(m_sPageName).Append("?searchfield=' + iSearchField + '&search=' + sSearchTerm;} else { alert('").Append(GetMessage("js err please enter text")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append("</script>" & Environment.NewLine)

        ltr_js.Text &= Environment.NewLine & sbJS.ToString()

    End Sub

    Protected Sub Util_CheckAccess()

        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(GetMessage("feature locked error"))
        End If

        If Not m_refContentApi.IsARoleMember(Common.EkEnumeration.CmsRoleIds.CommerceAdmin) Then
            Throw New Exception(GetMessage("err not role commerce-admin"))
        End If

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

        Util_GetSortValue(dg_audit.Attributes("SortExpression"))

        Display_Audit()

        isPostData.Value = "true"

    End Sub

    Protected Sub Util_HidePagingLinks()

        PageLabel.Visible = False
        CurrentPage.Visible = False
        OfLabel.Visible = False
        TotalPages.Visible = False

        FirstPage.Visible = False
        lnkBtnPreviousPage.Visible = False
        NextPage.Visible = False
        LastPage.Visible = False

    End Sub

    Protected Sub Util_DG_Sort(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridSortCommandEventArgs) Handles dg_audit.SortCommand

        Util_GetSortExpression(dg_audit, e)

        Display_Audit()

        isPostData.Value = "true"

    End Sub

    Private Sub Util_GetSortValue(ByVal sortExpression As String)

        Select Case sortExpression.tolower()

            Case "ip"

                sortcriteria = CommerceAuditProperty.IPAddress
                sortdirection = EkEnumeration.OrderByDirection.Ascending

            Case "fmessage"

                sortcriteria = CommerceAuditProperty.Message
                sortdirection = EkEnumeration.OrderByDirection.Ascending

            Case "orderid"

                sortcriteria = CommerceAuditProperty.OrderId
                sortdirection = EkEnumeration.OrderByDirection.Ascending

            Case "userid"

                sortcriteria = CommerceAuditProperty.UserId
                sortdirection = EkEnumeration.OrderByDirection.Ascending

            Case Else

                sortcriteria = CommerceAuditProperty.DateCreated
                sortdirection = EkEnumeration.OrderByDirection.Descending

        End Select

    End Sub

    Private Sub Util_GetSortExpression(ByVal dg As DataGrid, ByVal e As DataGridSortCommandEventArgs)
        Dim sortColumns As String() = Nothing
        Dim sortAttribute As String = dg.Attributes("SortExpression")
        'Check to See if we have an existing Sort Order already in the Grid.     
        'If so get the Sort Columns into an array
        If e.SortExpression <> sortAttribute Then
            sortAttribute = e.SortExpression
        End If
        If sortAttribute <> String.Empty Then
            sortColumns = sortAttribute.Split(",".ToCharArray())
        End If
        'if User clicked on the columns in the existing sort sequence.
        'Toggle the sort order or remove the column from sort appropriately
        If sortAttribute.IndexOf(e.SortExpression) > 0 OrElse sortAttribute.StartsWith(e.SortExpression) Then
            sortAttribute = Util_ModifySortExpression(sortColumns, e.SortExpression)
        Else
            sortAttribute += e.SortExpression & " ASC,"
        End If
        dg.Attributes("SortExpression") = sortAttribute

        ' Return sortAttribute
        Util_GetSortValue(sortAttribute)

    End Sub

    Private Function Util_ModifySortExpression(ByVal sortColumns As String(), ByVal sortExpression As String) As String

        Dim ascSortExpression As String = String.Concat(sortExpression, " ASC")
        Dim descSortExpression As String = String.Concat(sortExpression, " DESC")

        For i As Integer = 0 To sortColumns.Length - 1

            If ascSortExpression.Equals(sortColumns(i)) Then
                sortColumns(i) = descSortExpression

            ElseIf descSortExpression.Equals(sortColumns(i)) Then
                Array.Clear(sortColumns, i, 1)
            End If
        Next

        Return String.Join(",", sortColumns).Replace(",,", ",").TrimStart(",".ToCharArray())

    End Function


#End Region

#Region "JS, CSS"

    Private Sub RegisterCSS()

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "/wamenu/css/com.ektron.ui.menu.css", "EktronMenuCss")
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

    End Sub

    Private Sub RegisterJS()

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "/wamenu/includes/com.ektron.ui.menu.js", "EktronMenuJs")

    End Sub

#End Region

End Class