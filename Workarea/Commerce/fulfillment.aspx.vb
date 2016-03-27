Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea
Imports System.Collections.Generic

Partial Class Commerce_fulfillment
    Inherits workareabase
    Implements ICallbackEventHandler

    Protected cCustomer As CustomerData = Nothing
    Protected order As OrderData = Nothing
    Protected CustomerManager As Customer = Nothing
    Protected bCommerceAdmin As Boolean = False
    Protected OrderBy As String = ""
    Protected m_intGroupId As Integer = -1
    Protected GroupName As String = "EveryOne"
    Protected m_intGroupType As Integer = -1 '0-CMS User; 1-Membership User
    Protected m_intUserActiveFlag As Integer = 0 '0-Active;1-Deleted;-1-Not verified
    Protected m_strDirection As String = "asc"
    Protected m_strSearchText As String = ""
    Protected m_strKeyWords As String = ""
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected user_list As UserData()
    Protected m_refUserApi As New UserAPI
    Protected setting_data As SettingsData
    Protected m_refSiteApi As New SiteAPI
    Protected user_data As UserData
    Private m_bCommunityGroup As Boolean
    Private m_iCommunityGroup As Integer = 0
    Protected m_strSelectedItem As String = "-1"
    Protected orderApi As OrderApi
    Protected SiteList As New List(Of String)
    Protected defaultCurrency As CurrencyData = Nothing
    Protected m_strWfImgPath As String = String.Empty
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1
    Protected payment As OrderPaymentData = Nothing
    Protected captureOrderId As Long = 0
    Protected RegionManager As RegionApi = Nothing
    Protected CountryManager As CountryApi = Nothing
    Protected AddressManager As IAddress = ObjectFactory.GetAddress()
    Protected addressType As String = ""

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            Utilities.ValidateUserLogin()

            If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
                Throw New Exception(GetMessage("feature locked error"))
            End If

            orderApi = New OrderApi()
            defaultCurrency = (New CurrencyApi()).GetItem(m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId)
            CustomerManager = New Customer(m_refContentApi.RequestInformationRef)
            Util_CheckAccess()
            Util_RegisterResources()
            Util_SetServerJSVariable()
            Select Case Me.m_sPageAction

                Case "editaddress"

                    If Request.QueryString("addressType") <> "" Then addressType = Request.QueryString("addressType")

                    RegionManager = New RegionApi()
                    CountryManager = New CountryApi()

                    If Page.IsPostBack AndAlso isCPostData.Value = "" Then Process_EditAddress() Else Display_ViewAddress(True)


                Case "vieworder"
                    If Page.IsPostBack() AndAlso Request.Form("hdn_code") = "2" Then
                        Process_Capture()
                    ElseIf Page.IsPostBack() AndAlso Request.Form("hdn_code") = "3" Then
                        Process_Fraud()
                    ElseIf Page.IsPostBack() AndAlso Request.Form("hdn_code") = "4" Then
                        Process_CancelOrder()
                    ElseIf Page.IsPostBack() AndAlso Request.Form("hdn_code") = "5" Then

                        Process_CallOrderEvent()

                    Else
                        Display_ViewOrder()
                    End If
                Case "showpayment"

                    If Page.IsPostBack() AndAlso Request.Form("hdn_code") = "2" Then

                        Process_Capture()

                    ElseIf Page.IsPostBack() AndAlso Request.Form("hdn_code") = "7" Then

                        Process_CaptureAndSettle()

                    ElseIf Page.IsPostBack() AndAlso Request.Form("hdn_code") = "8" Then

                        Process_Settle()

                    Else

                        Display_ViewPayment()

                    End If

                Case "editnotes"

                    If Page.IsPostBack() AndAlso Request.Form("hdn_code") = "6" Then Process_EditNotes() Else Display_ViewNotes()

                Case "trackingnumber"
                    If Page.IsPostBack Then
                        Process_TrackingNumber()
                    Else
                        Display_TrackingNumber()
                    End If
                Case "mostrecent"
                    If Page.IsPostBack = False Then
                        Display_MostRecent()
                    End If
                Case "bydates"
                    If Page.IsPostBack = False Then
                        Display_ByDates()
                    End If
                Case "byproduct"
                    If Page.IsPostBack = False Then
                        Display_ByProduct()
                    End If
                Case "bycustomer"
                    If Page.IsPostBack = False Then
                        Display_ByCustomer()
                    End If
                Case "custom"
                    Display_ViewCustom()
                Case "onhold"
                    If Page.IsPostBack = False Then
                        Display_ViewOnHold()
                    End If
                Case Else ' "viewall"
                    If Page.IsPostBack = False Then
                        Display_ViewAll()
                    End If
            End Select
            Util_SetLabels()
            Util_SetJS()

        Catch ex As Exception

            Utilities.ShowError(ex.Message)

        End Try

    End Sub
    Private Function Quote(ByVal KeyWords As String) As String
        Dim result As String = KeyWords
        If (KeyWords.Length > 0) Then
            result = KeyWords.Replace("'", "''")
        End If
        Return result
    End Function
    Private Sub CollectSearchText()
        m_strKeyWords = Request.QueryString("user")
        m_strSelectedItem = Request.QueryString("field")
        If (m_strSelectedItem = "-1 selected") Then
            m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%' OR last_name like '%" & Quote(m_strKeyWords) & "%' OR user_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "last_name") Then
            m_strSearchText = " (last_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "first_name") Then
            m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "user_name") Then
            m_strSearchText = " (user_name like '%" & Quote(m_strKeyWords) & "%')"
        End If
    End Sub
    Public Sub ViewAllUsers()
        m_strKeyWords = Request.QueryString("user")
        If (Not Page.IsPostBack) Then
            CollectSearchText()
            DisplayUsers()
        ElseIf (IsPostBack = False) Then
            ViewAllUsersToolBar()
        End If
        isPostData.Value = "false"
    End Sub

    Protected Sub Display_ViewNotes()

        pnl_view.Visible = False
        pnl_viewall.Visible = False
        pnl_notes.Visible = True

        order = orderApi.GetItem(Me.m_iID)

        txt_ordernotes.Text = order.SpecialInstructions

    End Sub

    Protected Sub Display_MostRecent()

        Dim orderList As New List(Of OrderData)
        Dim orderCriteria As New Criteria(Of OrderProperty)
        Dim Toggle As Boolean = True

        orderCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        orderCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        orderList = orderApi.GetList(orderCriteria)

        TotalPagesNumber = orderCriteria.PagingInfo.TotalPages
        PagingInfo(TotalPagesNumber, Toggle)

        dg_orders.DataSource = orderList
        dg_orders.DataBind()
    End Sub
    Private Sub Display_ByCustomer()
        If ((Not IsNothing(Request.QueryString("groupid"))) AndAlso (Request.QueryString("groupid") <> "")) Then
            m_intGroupId = Convert.ToInt32(Request.QueryString("groupid"))
            If m_bCommunityGroup Then
                m_iCommunityGroup = m_intGroupId
                m_intGroupId = Me.m_refContentApi.EkContentRef.GetCmsGroupForCommunityGroup(m_iCommunityGroup)
            End If
        End If
        ViewAllUsers()
    End Sub
    Private Sub DisplayUsers()
        Dim orderList As New List(Of OrderData)
        Dim tempList As New List(Of OrderData)
        Dim req As New UserRequestData
        Dim sIds As String = ""
        Dim userID() As String
        Dim i As Integer = 0
        Dim orderCriteria As New Criteria(Of OrderProperty)
        Dim Toggle As Boolean = True

        orderCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        orderCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()
        If Request.QueryString("user") IsNot Nothing And Request.QueryString("user") <> "" Then
            sIds = Request.QueryString("user")
            sIds = sIds.TrimEnd(",")
            userID = sIds.Split(",")

            If userID.Length > 0 Then
                For i = 0 To userID.Length - 1
                    orderList = orderApi.GetCustomerOrderList(CLng(userID(i)), orderCriteria.PagingInfo)
                    tempList.AddRange(orderList)
                Next
                If tempList.Count > 0 Then
                    TotalPagesNumber = orderCriteria.PagingInfo.TotalPages
                    PagingInfo(TotalPagesNumber, Toggle)
                    dg_orders.DataSource = tempList
                    dg_orders.DataBind()
                Else
                    Toggle = False
                    PagingInfo(TotalPagesNumber, Toggle)
                    literal1.Text = Me.GetMessage("lbl no orders")
                End If
            Else
                literal1.Text = Me.GetMessage("lbl no orders")
            End If
        Else
            literal1.Text = Me.GetMessage("lbl no orders")
        End If

    End Sub
    Private Function IsSelected(ByVal val As String) As String
        If (val = m_strSelectedItem) Then
            Return (" selected ")
        Else
            Return ("")
        End If
    End Function
    Private Sub ViewAllUsersToolBar()
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>")
        result.Append("<td><input type=""button"" id=""btnSearch"" Value=""Filter"" onClick=""searchuser()"" />&nbsp;")
        result.Append("<img id=""imgClose"" src='../images/application/close_red_sm.jpg'></img></td></tr></table><br />")
        result.Append("<table><tr>")
        result.Append("<td valign=""top"">" & m_refMsg.GetMessage("lbl text") & "</td><td><input type=""text"" size=""25"" id=""txtSearch"" name=""txtSearch"" style='background-color:white;' value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event);""></td></tr>")
        result.Append("<tr><td>" & m_refMsg.GetMessage("lbl field") & ":</td><td><select style='BACKGROUND-COLOR: white;' id=searchlist name=searchlist bgcolor=blue >")
        result.Append("<option value=""-1""" & IsSelected("-1") & ">All</option>")
        result.Append("<option value=""user_name""" & IsSelected("user_name") & ">" & m_refMsg.GetMessage("lbl customer username") & "</option>")
        'result.Append("<option value=""user_name""" & IsSelected("display_name") & ">" & m_refMsg.GetMessage("display name label") & "</option>")
        result.Append("<option value=""first_name""" & IsSelected("first_name") & ">" & m_refMsg.GetMessage("lbl first name") & "</option>")
        result.Append("<option value=""last_name""" & IsSelected("last_name") & ">" & m_refMsg.GetMessage("lbl last name") & "</option>")
        'result.Append("<option value=""last_name""" & IsSelected("orders") & ">" & m_refMsg.GetMessage("lbl orders") & "</option>")
        'result.Append("<option value=""last_name""" & IsSelected("value") & ">" & m_refMsg.GetMessage("lbl value") & "</option>")
        result.Append("</select></td></tr>")
        result.Append("<td>")
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
#Region "Display"

    Private Sub Display_TrackingNumber()

        Dim orderData As OrderData

        orderData = orderApi.GetItem(Me.m_iID)
        chk_markasshipped.Enabled = True
        chk_markasshipped.Checked = False
        If orderData.Parts(0).TrackingNumber IsNot Nothing AndAlso orderData.Parts(0).TrackingNumber <> "" Then
            txt_trackingnumber.Text = orderData.Parts(0).TrackingNumber.ToString()
        End If
        If orderData.Parts(0).DateShipped <> DateTime.MinValue AndAlso orderData.Parts(0).DateShipped <> DateTime.MaxValue Then
            chk_markasshipped.Enabled = False
            chk_markasshipped.Checked = True
        End If

    End Sub
    Private Sub Display_ByProduct()
        Dim productid As Long = Request.QueryString("productid")
        Dim orderList As New List(Of OrderData)()
        Dim reportCriteria As New Criteria(Of OrderProperty)
        Dim Toggle As Boolean = True

        reportCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        reportCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        orderList = orderApi.GetProductOrderList(productid, reportCriteria.PagingInfo)
        If orderList.Count > 0 Then

            TotalPagesNumber = reportCriteria.PagingInfo.TotalPages
            PagingInfo(TotalPagesNumber, Toggle)

            dg_orders.DataSource = orderList
            dg_orders.DataBind()
        Else
            Toggle = False
            PagingInfo(TotalPagesNumber, Toggle)
            literal1.Text = Me.GetMessage("lbl no orders")
        End If
    End Sub
    Protected Sub Display_ViewOrder(Optional ByVal capturedOrder As OrderData = Nothing)
        pnl_viewall.Visible = False

        If capturedOrder IsNot Nothing Then order = capturedOrder Else order = orderApi.GetItem(Me.m_iID)

        If order Is Nothing OrElse order.Id = 0 Then

            literal1.Text = Me.GetMessage("lbl no orders")

        Else

            ltr_orderid.Text = GetMessage("lbl order id") & ": " & order.Id.ToString()
            ltr_status.Text = "<label class=""label"">" & GetMessage("generic status") & ": " & Util_ShowStatus(order.Status) & "</label>"
            ltr_payments.Text = "<label class=""paymentLabel"">" & GetMessage("lbl payment(s)") & ":" & "</label>"
            ltr_coupons.Text = "<label class=""couponLabel"">" & GetMessage("lbl coupons") & ":" & "</label>"
            Util_PopulateCustomer(order.Customer)

            If order.SpecialInstructions <> "" Then ltr_notes.Text = order.SpecialInstructions Else ltr_notes.Text = "&#160;"

            ltr_created.Text = Util_ShowDate(order.DateCreated)
            If order.Payments.Count = 1 Then

                ltr_authorized.Text = Util_ShowDate(order.Payments(0).AuthorizedOn)
                ltr_captured.Text = Util_ShowDate(order.Payments(0).CapturedOn)
            Else

                tr_authorized.Visible = False
                tr_captured.Visible = False

            End If

            Display_ViewPayments(order.Payments)
            Display_ViewCoupons(order.Coupons)

            ltr_completed.Text = Util_ShowDate(order.DateCompleted)
            ltr_required.Text = Util_ShowDate(order.DateRequired)
            ltr_shipped.Text = Util_ShowDate(order.Parts(0).DateShipped)

            m_strWfImgPath = m_refUserApi.AppPath & "workflowimage.aspx?type=order&id=" & order.Id.ToString()

            ltr_workflow_image.Text = "<div class=""workflowimgwrapper""><img src=""" & m_refUserApi.AppPath & "wfactivities.png?instanceid=" & order.WorkflowId.ToString() & """ class=""workflowimage"" /></div>"

            ltr_order_billing.Text = Util_ShowAddress(order.BillingAddressId, False)
            ltr_order_shipping.Text = Util_ShowAddress(order.Parts(0).ShippingAddressId, True)

            ltr_subtotal.Text = order.Currency.AlphaIsoCode & EkFunctions.FormatCurrency(order.Subtotal, order.Currency.CultureCode)
            ltr_coupontotal.Text = "(" & order.Currency.AlphaIsoCode & EkFunctions.FormatCurrency(order.CouponTotal, order.Currency.CultureCode) & ")"
            ltr_taxtotal.Text = order.Currency.AlphaIsoCode & EkFunctions.FormatCurrency(order.TaxTotal, order.Currency.CultureCode)
            ltr_shippingtotal.Text = order.Currency.AlphaIsoCode & EkFunctions.FormatCurrency(order.ShippingTotal, order.Currency.CultureCode)
            ltr_ordertotal.Text = order.Currency.AlphaIsoCode & EkFunctions.FormatCurrency(order.OrderTotal, order.Currency.CultureCode)
            ltr_lineitems.Text = ""
            For i As Integer = 0 To (order.Parts.Count - 1)
                For j As Integer = 0 To (order.Parts(i).Lines.Count - 1)
                    ltr_lineitems.Text &= "<tr style=""background-color: White;"">"
                    ltr_lineitems.Text &= "<td align=""left"" valign=""top"">" & order.Parts(i).Lines(j).ProductTitle
                    If order.Parts(i).Lines.Item(j).Configuration.KitOptions.Count > 0 Then
                        Dim k As Integer = 0
                        For k = 0 To order.Parts(i).Lines.Item(j).Configuration.KitOptions.Count - 1
                            ltr_lineitems.Text &= "<br />&nbsp;&nbsp;" & order.Parts(i).Lines.Item(j).Configuration.KitOptions(k).GroupName & "&nbsp;:&nbsp;" & order.Parts(i).Lines.Item(j).Configuration.KitOptions(k).GroupOptionName
                        Next
                    End If
                    ltr_lineitems.Text &= "</td>"
                    ltr_lineitems.Text &= "<td align=""right"" valign=""top"">" & order.Currency.AlphaIsoCode & EkFunctions.FormatCurrency(order.Parts(i).Lines(j).PriceEach, order.Currency.CultureCode) & "</td>"
                    ltr_lineitems.Text &= "<td align=""right"" valign=""top"">" & order.Parts(i).Lines(j).Quantity & "</td>"
                    ltr_lineitems.Text &= "<td align=""right"" valign=""top"">" & order.Currency.AlphaIsoCode & EkFunctions.FormatCurrency(order.Parts(i).Lines(j).PriceSubTotal, order.Currency.CultureCode) & "</td>"
                    ltr_lineitems.Text &= "</tr>"
                Next
            Next

        End If

    End Sub

    Protected Sub Display_ViewPayments(ByVal payments As List(Of OrderPaymentData))

        dg_payments.DataSource = payments

        dg_payments.DataBind()

    End Sub

    Protected Sub Display_ViewCoupons(ByVal coupons As List(Of CouponData))

        dg_coupons.DataSource = coupons

        dg_coupons.DataBind()

    End Sub

    Private Sub Display_ViewPayment()

        payment = orderApi.GetOrderPayment(Me.m_iID)
        order = orderApi.GetItem(payment.OrderId)

        ltr_transactionId.Text = payment.TransactionId
        ltr_gateway.Text = payment.Gateway
        ltr_type.Text = payment.PaymentType

        If payment.PaymentType.ToLower() <> "ektron.cms.commerce.creditcardpayment" Then tr_last4.Visible = False

        ltr_last4.Text = payment.Last4Digits
        ltr_amount.Text = Util_ShowPrice(payment.PaymentTotal, payment.Currency.Id)
        ltr_paymentdate.Text = Util_ShowDate(payment.PaymentDate)
        ltr_authorizeddate.Text = Util_ShowDate(payment.AuthorizedOn)
        ltr_captureddate.Text = Util_ShowDate(payment.CapturedOn)

        If payment.PaymentType.ToLower() = "ektron.cms.commerce.paypalpayment" Or _
                                payment.PaymentType.ToLower() = "ektron.cms.commerce.checkpayment" Then

            tr_settled.Visible = True
            ltr_settleddate.Text = Util_ShowDate(payment.SettledDate)

        End If

    End Sub

    Protected Sub Display_ByDates()
        Dim startDate As DateTime = Request.QueryString("startdate")
        Dim endDate As DateTime = Request.QueryString("enddate")
        Dim orderList As New List(Of OrderData)
        Dim reportCriteria As New Criteria(Of OrderProperty)
        Dim Toggle As Boolean = True

        reportCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.GreaterThanOrEqualTo, startDate)
        reportCriteria.AddFilter(OrderProperty.DateCreated, CriteriaFilterOperator.LessThanOrEqualTo, endDate)
        reportCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        reportCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        orderList = orderApi.GetList(reportCriteria)

        If orderList.Count > 0 Then
            TotalPagesNumber = reportCriteria.PagingInfo.TotalPages
            PagingInfo(TotalPagesNumber, Toggle)
            Me.dg_orders.DataSource = orderList
            Me.dg_orders.DataBind()
        Else
            Toggle = False
            PagingInfo(TotalPagesNumber, Toggle)
            literal1.Text = Me.GetMessage("lbl no orders")
        End If
    End Sub
    Protected Sub PagingInfo(ByVal TotalPageNumber As Integer, ByVal Toggle As Boolean)

        Select Case Toggle
            Case False
                TotalPages.Visible = False
                CurrentPage.Visible = False
                lnkBtnPreviousPage.Visible = False
                NextPage.Visible = False
                LastPage.Visible = False
                FirstPage.Visible = False
                PageLabel.Visible = False
                OfLabel.Visible = False
            Case True
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
        End Select

    End Sub

    Protected Sub Display_ViewAll()
        Dim orderList As New List(Of OrderData)
        Dim orderCriteria As New Criteria(Of OrderProperty)
        Dim Toggle As Boolean = True

        orderCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        orderCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        orderCriteria.OrderByField = OrderProperty.DateCreated
        orderCriteria.OrderByDirection = EkEnumeration.OrderByDirection.Descending

        orderList = orderApi.GetList(orderCriteria)

        TotalPagesNumber = orderCriteria.PagingInfo.TotalPages
        PagingInfo(TotalPagesNumber, Toggle)

        dg_orders.DataSource = orderList
        dg_orders.DataBind()

        Util_ShowSites()

    End Sub

    Protected Sub Display_ViewCustom()
        Dim orderList As New List(Of OrderData)
        Dim reportCriteria As New Criteria(Of OrderProperty)

        If Request.QueryString("sites") <> "" Then

            Dim sites() As String = Request.QueryString("sites").Split(",")
            Dim tempSiteList As New List(Of String)

            For i As Integer = 0 To (sites.Length - 1)

                If sites(i) <> "" Then tempSiteList.Add(sites(i))

            Next

            reportCriteria.AddFilter(OrderProperty.Site, CriteriaFilterOperator.In, tempSiteList.ToArray())

        End If

        orderList = orderApi.GetList(reportCriteria)

        dg_orders.DataSource = orderList
        dg_orders.DataBind()

        Util_ShowSites()

    End Sub

    Protected Sub Display_ViewOnHold()
        Dim orderList As New List(Of OrderData)
        Dim orderApi As New OrderApi
        Dim orderCriteria As New Criteria(Of OrderProperty)
        Dim Toggle As Boolean = True

        orderCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        orderCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        orderList = orderApi.GetOnHoldOrderList(orderCriteria.PagingInfo)
        TotalPagesNumber = orderCriteria.PagingInfo.TotalPages
        PagingInfo(TotalPagesNumber, Toggle)
        dg_orders.DataSource = orderList
        dg_orders.DataBind()
    End Sub

    Protected Sub Display_ViewAddress(ByVal WithEdit As Boolean)

        pnl_view.Visible = False
        pnl_viewall.Visible = False

        Dim aAddress As AddressData = Nothing
        'Dim regioncriteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)

        'RegionManager = New RegionApi()
        'regioncriteria.AddFilter(RegionProperty.IsEnabled, CriteriaFilterOperator.EqualTo, True)

        'If Not Me.m_iID > 0 Then
        '    regioncriteria.AddFilter(RegionProperty.CountryId, CriteriaFilterOperator.EqualTo, drp_address_country.SelectedIndex)
        'End If
        'regioncriteria.PagingInfo.RecordsPerPage = 1000
        'drp_address_region.DataTextField = "Name"
        'drp_address_region.DataValueField = "Id"
        'drp_address_region.DataSource = RegionManager.GetList(regioncriteria)
        'drp_address_region.DataBind()

        Dim addresscriteria As New Ektron.Cms.Common.Criteria(Of CountryProperty)(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        addresscriteria.AddFilter(CountryProperty.IsEnabled, CriteriaFilterOperator.EqualTo, True)
        addresscriteria.PagingInfo.RecordsPerPage = 1000
        drp_address_country.DataTextField = "Name"
        drp_address_country.DataValueField = "Id"
        drp_address_country.DataSource = CountryManager.GetList(addresscriteria)
        drp_address_country.DataBind()

        If Me.m_iID > 0 Then

            order = orderApi.GetItem(Me.m_iID)

            If addressType = "billing" Then

                aAddress = AddressManager.GetItem(order.BillingAddressId)

            ElseIf addressType = "shipping" Then

                aAddress = AddressManager.GetItem(order.Parts(0).ShippingAddressId)

            End If

            If Not Page.IsPostBack Then

                ltr_address_id.Text = aAddress.Id
                txt_address_name.Text = aAddress.Name
                txt_address_company.Text = aAddress.Company
                txt_address_line1.Text = aAddress.AddressLine1
                txt_address_line2.Text = aAddress.AddressLine2
                txt_address_city.Text = aAddress.City
                drp_address_country.SelectedIndex = Util_FindItem(aAddress.Country.Id, "country")
                Util_BindRegions(aAddress.Country.Id)
                drp_address_region.SelectedValue = aAddress.Region.Id
                txt_address_postal.Text = aAddress.PostalCode
                txt_address_phone.Text = aAddress.Phone

            End If

        End If

    End Sub

#End Region

#Region "Process"

    Protected Sub Process_EditNotes()

        orderApi.SetNotes(Me.m_iID, txt_ordernotes.Text)

        literal1.Text = " <script type=""text/javascript"">" & Environment.NewLine
        literal1.Text &= "    parent.ektb_remove(); " & Environment.NewLine
        literal1.Text &= "    parent.window.location = 'fulfillment.aspx?action=vieworder&id=" & Me.m_iID.ToString() & "';" & Environment.NewLine
        literal1.Text &= "</script>" & Environment.NewLine

    End Sub

    Protected Sub Process_EditAddress()

        Dim aAddress As AddressData = Nothing

        order = orderApi.GetItem(Me.m_iID)

        If (addressType = "billing") Then

            aAddress = AddressManager.GetItem(order.BillingAddressId)

        ElseIf addressType = "shipping" Then

            aAddress = AddressManager.GetItem(order.Parts(0).ShippingAddressId)

        End If

        aAddress.Name = txt_address_name.Text
        aAddress.Company = txt_address_company.Text
        aAddress.AddressLine1 = txt_address_line1.Text
        aAddress.AddressLine2 = txt_address_line2.Text
        aAddress.City = txt_address_city.Text
        Dim rData As New RegionData()
        rData.Id = drp_address_region.SelectedValue
        aAddress.Region = rData
        aAddress.PostalCode = txt_address_postal.Text
        Dim cData As New CountryData()
        cData.Id = drp_address_country.SelectedValue
        aAddress.Country = cData
        aAddress.Phone = txt_address_phone.Text

        If aAddress.Id > 0 Then AddressManager.UpdateOrderAddress(aAddress, Me.m_iID, (addressType = "shipping"), (addressType = "billing"))

        literal1.Text = " <script type=""text/javascript"">" & Environment.NewLine
        literal1.Text &= "    parent.ektb_remove(); " & Environment.NewLine
        literal1.Text &= "    parent.window.location = 'fulfillment.aspx?action=vieworder&id=" & Me.m_iID.ToString() & "';" & Environment.NewLine
        literal1.Text &= "</script>" & Environment.NewLine

    End Sub

    Protected Sub Process_Payment()

        ' Process_Capture()

    End Sub

    Protected Sub Process_CallOrderEvent()

        If bCommerceAdmin Then

            Dim eventTrigger As String = Request.Form("hdn_event")
            Dim orderEvent As EkEnumeration.OrderWorkflowEvent

            Select Case eventTrigger.ToLower()
                Case "onorderupdated"
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderUpdated
                Case "onordercancelled"
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderCancelled
                Case "onorderfraud"
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderFraud
                Case "onordercaptured"
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderCaptured
                Case "onordershipped"
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderShipped
                Case "onorderprocessed"
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderProcessed
                Case "onorderpayment"
                    orderEvent = EkEnumeration.OrderWorkflowEvent.OrderPaymentReceived
                Case Else
                    Return
            End Select


            hdn_event.Value = ""
            hdn_code.Value = ""

            orderApi.RaiseWorkflowEvent(Me.m_iID, orderEvent)

            literal1.Text = " <script type=""text/javascript"">" & Environment.NewLine
            ' literal1.Text &= "    ektb_show('','#EkTB_inline?height=18&width=500&inlineId=dvHoldMessage&modal=true', null, '', true); " & Environment.NewLine
            literal1.Text &= "    setTimeout(""window.location = 'fulfillment.aspx?action=vieworder&id=" & Me.m_iID.ToString() & "';"",3000);" & Environment.NewLine
            literal1.Text &= "</script>" & Environment.NewLine

        End If

    End Sub

    Protected Sub Process_CancelOrder()

        If bCommerceAdmin Then

            hdn_code.Value = ""
            orderApi.SetOrderStatus(Me.m_iID, EkEnumeration.OrderStatus.Cancelled)
            Display_ViewOrder()

        End If

    End Sub

    Protected Sub Process_TrackingNumber()

        If bCommerceAdmin Then

            orderApi.SetTrackingNumber(Me.m_iID, Request.Form(txt_trackingnumber.UniqueID), chk_markasshipped.Checked)

            ltr_js.Text = " <script type=""text/javascript"">" & Environment.NewLine
            ltr_js.Text &= "parent.window.location.reload(false);" & Environment.NewLine ' refresh parent
            ltr_js.Text &= "parent.ektb_remove();" & Environment.NewLine ' close thickbox
            ltr_js.Text &= "</script>" & Environment.NewLine

        End If

    End Sub
    Protected Sub Process_Fraud()

        If bCommerceAdmin Then

            hdn_code.Value = ""
            orderApi.MarkAsFraud(Me.m_iID)
            Display_ViewOrder()

        End If

    End Sub
    Protected Sub Process_Capture()

        Try

            If Request.QueryString("orderid") <> "" Then captureOrderId = Request.QueryString("orderid")

            If bCommerceAdmin AndAlso captureOrderId > 0 Then

                hdn_code.Value = ""

                orderApi.Capture(captureOrderId, Me.m_iID)

                ltr_js.Text = " <script type=""text/javascript"">" & Environment.NewLine
                ltr_js.Text &= "parent.location.reload(true);" & Environment.NewLine ' refresh parent
                ltr_js.Text &= "parent.ektb_remove();" & Environment.NewLine ' close thickbox
                ltr_js.Text &= "</script>" & Environment.NewLine

            Else

                Throw New Exception(GetMessage("err not role commerce-admin"))

            End If

        Catch ex As Exception

            Utilities.ShowError(ex.Message)

        End Try

    End Sub

    Protected Sub Process_CaptureAndSettle()

        Try

            If Request.QueryString("orderid") <> "" Then captureOrderId = Request.QueryString("orderid")

            If bCommerceAdmin AndAlso captureOrderId > 0 Then

                hdn_code.Value = ""

                orderApi.Capture(captureOrderId, Me.m_iID)
                orderApi.MarkPaymentAsSettled(captureOrderId, Me.m_iID)

                ltr_js.Text = " <script type=""text/javascript"">" & Environment.NewLine
                ltr_js.Text &= "parent.location.reload(true);" & Environment.NewLine ' refresh parent
                ltr_js.Text &= "parent.ektb_remove();" & Environment.NewLine ' close thickbox
                ltr_js.Text &= "</script>" & Environment.NewLine

            Else

                Throw New Exception(GetMessage("err not role commerce-admin"))

            End If

        Catch ex As Exception

            Utilities.ShowError(ex.Message)

        End Try

    End Sub

    Protected Sub Process_Settle()

        Try

            If Request.QueryString("orderid") <> "" Then captureOrderId = Request.QueryString("orderid")

            If bCommerceAdmin AndAlso captureOrderId > 0 Then

                hdn_code.Value = ""

                orderApi.MarkPaymentAsSettled(captureOrderId, Me.m_iID)

                ltr_js.Text = " <script type=""text/javascript"">" & Environment.NewLine
                ltr_js.Text &= "parent.location.reload(true);" & Environment.NewLine ' refresh parent
                ltr_js.Text &= "parent.ektb_remove();" & Environment.NewLine ' close thickbox
                ltr_js.Text &= "</script>" & Environment.NewLine

            Else

                Throw New Exception(GetMessage("err not role commerce-admin"))

            End If

        Catch ex As Exception

            Utilities.ShowError(ex.Message)

        End Try

    End Sub


#End Region

#Region "Util"

    Protected Sub Util_SetLabels()

        ltr_customerorders_lbl.Text = GetMessage("lbl orders")
        ltr_customertotal_lbl.Text = GetMessage("lbl total order value")
        ltr_customeravg_lbl.Text = GetMessage("lbl avg order value")

        ltr_notes_lbl.Text = GetMessage("lbl ecomm order notes") & IIf(Not m_refContentApi.RequestInformationRef.CommerceSettings.OrderProcessingDisabled, "<a href=""#"" class=""button buttonInline greenHover buttonEdit btnEdit"" onclick=""if (allowOpen) { ektb_show('" & GetMessage("lbl ecomm edit order notes") & "', 'fulfillment.aspx?action=editnotes&id=" & m_iID.ToString() & "&EkTB_iframe=true&width=500&height=350&scrolling=true&modal=true', false); }"" title=""" & GetMessage("btn edit") & """ >" & GetMessage("btn edit") & "</a>", "")

        ltr_created_lbl.Text = GetMessage("generic datecreated")
        ltr_authorized_lbl.Text = GetMessage("lbl date authorized")
        ltr_captured_lbl.Text = GetMessage("lbl date captured")
        ltr_settleddate_lbl.Text = GetMessage("lbl date settled")
        ltr_shipped_lbl.Text = GetMessage("lbl date shipped")
        ltr_required_lbl.Text = GetMessage("lbl date required")
        ltr_completed_lbl.Text = GetMessage("lbl date completed")

        ltr_order_billing_lbl.Text = GetMessage("lbl billing") & IIf(Not m_refContentApi.RequestInformationRef.CommerceSettings.OrderProcessingDisabled, "<a href=""#"" class=""button buttonInline greenHover buttonEdit btnEdit"" onclick=""if (allowOpen) { ektb_show('" & GetMessage("lbl edit address") & "', 'fulfillment.aspx?addressType=billing&action=editaddress&id=" & m_iID.ToString() & "&EkTB_iframe=true&width=500&height=350&scrolling=true&modal=true', false); } "" title=""" & GetMessage("btn edit") & """ >" & GetMessage("btn edit") & "</a>", "")
        ltr_order_shipping_lbl.Text = GetMessage("lbl shipping") & IIf(Not m_refContentApi.RequestInformationRef.CommerceSettings.OrderProcessingDisabled, "<a href=""#"" class=""button buttonInline greenHover buttonEdit btnEdit"" onclick=""if (allowOpen) { ektb_show('" & GetMessage("lbl edit address") & "', 'fulfillment.aspx?addressType=shipping&action=editaddress&id=" & m_iID.ToString() & "&EkTB_iframe=true&width=500&height=350&scrolling=true&modal=true', false); } "" title=""" & GetMessage("btn edit") & """ >" & GetMessage("btn edit") & "</a>", "")

        ltr_subtotal_lbl.Text = GetMessage("lbl subtotal")
        ltr_coupontotal_lbl.Text = GetMessage("lbl coupon total")
        ltr_taxtotal_lbl.Text = GetMessage("lbl wa tax")
        ltr_shippingtotal_lbl.Text = GetMessage("lbl shipping")
        ltr_ordertotal_lbl.Text = GetMessage("lbl order total")

        ltr_trackingnumber.Text = GetMessage("lbl tracking number")
        ltr_markasshipped.Text = GetMessage("lbl mark as shipped")
        hdn_errorMessage.Value = String.Format(GetMessage("js alert field cannot include"), "< >")
        hdn_errOrderNotesMessage.Value = String.Format(GetMessage("js alert field cannot include"), "< >")

        ltr_desc_lbl.Text = GetMessage("generic description")
        ltr_saleprice_lbl.Text = GetMessage("lbl sale price")
        ltr_qty_lbl.Text = GetMessage("lbl quantity")
        ltr_total_lbl.Text = GetMessage("lbl total")

        ltr_transactionId_lbl.Text = GetMessage("lbl transactionid")
        ltr_gateway_lbl.Text = GetMessage("lbl payment gateway")
        ltr_type_lbl.Text = GetMessage("generic type")
        ltr_last4_lbl.Text = GetMessage("lbl last 4 digits")
        ltr_amount_lbl.Text = GetMessage("lbl payment total")
        ltr_paymentdate_lbl.Text = GetMessage("lbl payment date")
        ltr_authorizeddate_lbl.Text = GetMessage("lbl date authorized")
        ltr_captureddate_lbl.Text = GetMessage("lbl date captured")

        ltr_holdmsg.Text = m_refMsg.GetMessage("one moment msg")

        ltr_showidsearch.Text = GetMessage("lbl reporting id orders")
        ltr_searchid.Text = GetMessage("lbl err order id invalid")

        Me.ltr_address_id_lbl.Text = Me.GetMessage("generic id")
        Me.ltr_address_name.Text = Me.GetMessage("lbl address name")
        Me.ltr_address_company.Text = Me.GetMessage("lbl address company")
        Me.ltr_address_line1.Text = Me.GetMessage("lbl address street")
        Me.ltr_address_city_lbl.Text = Me.GetMessage("lbl address city")
        Me.ltr_address_region.Text = Me.GetMessage("lbl address state province")
        Me.ltr_address_postal.Text = Me.GetMessage("lbl address postal")
        Me.ltr_address_country.Text = Me.GetMessage("lbl address country")
        Me.ltr_address_phone.Text = Me.GetMessage("lbl address phone")

        Select Case Me.m_sPageAction

            Case "editnotes"

                pnl_viewall.Visible = False
                pnl_trackingnumber.Visible = False
                pnl_payment.Visible = False
                pnl_notes.Visible = True

                Dim newMenu As New workareamenu("file", GetMessage("lbl action"), m_refContentApi.AppPath & "images/UI/Icons/check.png")

                newMenu.AddItem(m_refContentApi.AppPath & "images/ui/icons/save.png", GetMessage("btn save"), "EditNotes();")

                AddMenu(newMenu)

                AddBackButton("javascript:parent.ektb_remove();")
                SetTitleBarToMessage("lbl ecomm edit order notes")

            Case "vieworder"


                If bCommerceAdmin AndAlso order IsNot Nothing Then

                    Dim newMenu As New workareamenu("file", GetMessage("lbl action"), m_refContentApi.AppPath & "images/UI/Icons/check.png")
                    Dim options As IList(Of String) = Nothing
                    Dim shippedAdded As Boolean = False

                    Try

                        If Not m_refContentApi.RequestInformationRef.CommerceSettings.OrderProcessingDisabled AndAlso Not (order.WorkflowId = Guid.Empty) Then options = Ektron.Workflow.Runtime.WorkflowHandler.GetOrderEventQueue(order.WorkflowId)

                    Catch ex As Exception

                        If order.WorkflowId <> Guid.Empty AndAlso ex.Message.IndexOf(order.WorkflowId.ToString()) = -1 Then Throw ex

                    End Try

                    If options IsNot Nothing Then

                        Dim addedOptions As Integer = 0

                        For i As Integer = 0 To (options.Count - 1)

                            Dim EventImage As String = "arrow_right_green.gif"
                            Dim EventName As String = options(i).Replace("OnOrder", "")
                            Dim addThisItem As Boolean = True


                            Select Case EventName.ToLower()

                                Case "updated"

                                    EventName = GetMessage("lbl workflow updated")
                                    addThisItem = False

                                Case "processed"

                                    EventName = GetMessage("lbl workflow processed")
                                    addedOptions = addedOptions + 1

                                Case "shipped"

                                    EventName = GetMessage("lbl workflow shipped")
                                    EventImage = "commerce/bundle.gif"
                                    addThisItem = False
                                    If order.Status <> EkEnumeration.OrderStatus.Fraud AndAlso order.Status <> EkEnumeration.OrderStatus.Cancelled AndAlso order.Status <> EkEnumeration.OrderStatus.Completed AndAlso Util_NeedsTrackingNumber() Then
                                        newMenu.AddBreak()
                                        newMenu.AddItem(AppImgPath & "commerce/bundle.gif", GetMessage("lbl add tracking number"), "AddTrackingNumber();")
                                        addedOptions = addedOptions + 1
                                    ElseIf Util_CanEditTrackingNumber() Then
                                        addThisItem = True
                                        newMenu.AddBreak()
                                        newMenu.AddItem(AppImgPath & "commerce/bundle.gif", GetMessage("lbl edit tracking number"), "AddTrackingNumber();")
                                        addedOptions = addedOptions + 1
                                    End If

                                Case "cancelled"

                                    EventName = GetMessage("lbl cancel order")
                                    EventImage = "commerce/cancel.gif"
                                    addThisItem = False
                                    If order.Status <> EkEnumeration.OrderStatus.Cancelled AndAlso order.Status <> EkEnumeration.OrderStatus.Fraud AndAlso order.Status <> EkEnumeration.OrderStatus.Completed Then
                                        newMenu.AddBreak()
                                        newMenu.AddItem(AppImgPath & "commerce/cancel.gif", GetMessage("lbl cancel order"), "CancelOrder();")
                                        addedOptions = addedOptions + 1
                                    End If

                                Case "fraud"

                                    EventName = GetMessage("lbl mark fraud")
                                    EventImage = "commerce/fraud.gif"
                                    addThisItem = False
                                    If order.Status <> EkEnumeration.OrderStatus.Fraud AndAlso order.Status <> EkEnumeration.OrderStatus.Cancelled AndAlso order.Status <> EkEnumeration.OrderStatus.Completed Then
                                        newMenu.AddItem(AppImgPath & "commerce/fraud.gif", GetMessage("lbl mark fraud"), "MarkAsFraud();")
                                        addedOptions = addedOptions + 1
                                    End If

                                Case Else

                                    EventName = options(i)
                                    addedOptions = addedOptions + 1

                            End Select

                            If addThisItem Then newMenu.AddItem(AppImgPath & EventImage, EventName, "CallOrderEvent('" & options(i) & "');")

                        Next

                        If addedOptions > 0 Then newMenu.AddBreak()

                    End If

                    If (IsCommerceAdmin) Then
                        newMenu.AddItem(AppImgPath & "adobe-pdf.gif", "Export As PDF", "window.open('" & m_refContentApi.AppPath & "commerce/export/order.aspx?id=" & m_iID.ToString() & "&type=pdf', 'PDF');")
                        newMenu.AddItem(AppImgPath & "ms-excel.gif", "Export As XLS", "window.open('" & m_refContentApi.AppPath & "commerce/export/order.aspx?id=" & m_iID.ToString() & "&type=xls', 'XLS');")
                        newMenu.AddItem(AppImgPath & "ms-notepad.gif", "Export As CSV", "window.open('" & m_refContentApi.AppPath & "commerce/export/order.aspx?id=" & m_iID.ToString() & "&type=csv', 'CSV');")
                    End If

                    Me.AddMenu(newMenu)

                End If

                If (Request.QueryString("page") = "workarea") Then
                    ' redirect to workarea when user clicks back button if we're in workarea
                    AddBackButton("javascript:top.switchDesktopTab()")
                Else
                    AddBackButton("fulfillment.aspx")
                End If
                If order Is Nothing Then
                    Me.pnl_view.Visible = False
                Else
                    Me.pnl_view.Visible = True
                End If

                Me.SetTitleBarToMessage("lbl view order")


            Case "showpayment"

                pnl_viewall.Visible = False
                pnl_trackingnumber.Visible = False
                pnl_payment.Visible = True
                tr_orderpart.Visible = False

                Dim newMenu As New workareamenu("file", GetMessage("lbl action"), AppImgPath & "check20.gif")

                If Not m_refContentApi.RequestInformationRef.CommerceSettings.OrderProcessingDisabled AndAlso Not Page.IsPostBack Then

                    If order.Status <> EkEnumeration.OrderStatus.Cancelled AndAlso order.Status <> EkEnumeration.OrderStatus.Fraud Then

                        If payment.CapturedOn = DateTime.MinValue Then
                            newMenu.AddItem(AppImgPath & "commerce/Submit.gif", GetMessage("lbl capture"), "CaptureOrder();")
                            If payment.PaymentType.ToLower() = "ektron.cms.commerce.paypalpayment" Or _
                                payment.PaymentType.ToLower() = "ektron.cms.commerce.checkpayment" Then
                                newMenu.AddItem(AppImgPath & "commerce/accept.gif", GetMessage("lbl capture and settle"), "CaptureOrderAndSettle();")
                            End If
                        End If

                        If payment.CapturedOn <> DateTime.MinValue And _
                            payment.CapturedOn <> DateTime.MaxValue And _
                            (payment.SettledDate = DateTime.MinValue Or _
                            payment.SettledDate = DateTime.MaxValue) And _
                            (payment.PaymentType.ToLower() = "ektron.cms.commerce.paypalpayment" Or _
                                payment.PaymentType.ToLower() = "ektron.cms.commerce.checkpayment") Then
                            newMenu.AddItem(AppImgPath & "commerce/accept.gif", GetMessage("lbl settle"), "SettlePayment();")

                        End If

                    End If

                End If

                If Not IsNothing(payment) AndAlso payment.PaymentType.ToLower() = "ektron.cms.commerce.paypalpayment" Then
                    newMenu.AddItem(AppImgPath & "commerce/paypal.gif", GetMessage("lbl check payment status"), "CheckPayPalStatus();")
                End If

                Me.AddMenu(newMenu)

                AddBackButton("javascript:parent.ektb_remove();")
                SetTitleBarToMessage("lbl view payment")


            Case "trackingnumber"

                pnl_viewall.Visible = False
                pnl_trackingnumber.Visible = True
                tr_orderpart.Visible = False
                AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", "btn save", "btn save", " onclick=""SubmitTrackingNumber(); return false;"" ")
                AddBackButton("javascript:parent.ektb_remove();")

            Case "editaddress"
                pnl_viewaddress.Visible = True
                Me.tr_address_id.Visible = (Me.m_iID > 0)
                Me.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", "btn save", "btn save", " onclick=""CheckAddress(); return false;"" ")
                AddBackButton("javascript:parent.ektb_remove();")
                Me.SetTitleBarToString(Me.GetMessage(IIf(Me.m_iID > 0, "lbl edit address", "lbl add address")))

            Case Else ' "viewall"

                Dim newMenu As New workareamenu("file", GetMessage("lbl order reporting"), AppImgPath & "commerce/catalog_view.gif")
                newMenu.AddItem(AppImgPath & "commerce/calendar_down.gif", GetMessage("lbl report most recent orders"), "window.location.href = 'fulfillment.aspx?action=mostrecent';")
                newMenu.AddItem(AppImgPath & "commerce/calendar.gif", GetMessage("lbl report date orders"), "ektb_show('Reporting By Dates', 'bydates.aspx?EkTB_iframe=true&width=400&height=150&scrolling=true&modal=true', false);")
                newMenu.AddItem(AppImgPath & "menu/users2.gif", GetMessage("lbl report customer orders"), "ektb_show('Reporting By Customer', 'bycustomer.aspx?EkTB_iframe=true&width=500&height=150&scrolling=true&modal=true', false);")
                newMenu.AddItem(m_refContentApi.AppPath & "images/ui/icons/brick.png", GetMessage("lbl report product orders"), "ektb_show('Reporting By Product', 'byproduct.aspx?action=reporting&EkTB_iframe=true&width=500&height=150&scrolling=true&modal=true', false);")
                newMenu.AddItem(AppImgPath & "commerce/Submit.gif", GetMessage("lbl report id orders"), "ShowOrderIdSearch();")
                'This option is being hidden for now (issue# 38244), and needs to be implemented in next release, which is 7.6.0 release/maintenance.
                'newMenu.AddItem(AppImgPath & "menu/form_blue.gif", GetMessage("lbl report custom orders"), "CustomReport();")
                Me.AddMenu(newMenu)
                Me.SetTitleBarToMessage("lbl orders")


        End Select

        Me.AddHelpButton("orders" & HttpUtility.UrlEncode(m_sPageAction))

    End Sub

    Public Function Util_NeedsTrackingNumber() As Boolean

        Dim needsTracking As Boolean = False

        If order IsNot Nothing Then
            For i As Integer = 0 To (order.Parts.Count - 1)
                If order.Parts(i).TrackingNumber = "" Then
                    needsTracking = True
                    Exit For
                End If
            Next
        End If

        Return needsTracking

    End Function

    Public Function Util_CanEditTrackingNumber() As Boolean

        Dim canEditTracking As Boolean = False

        If order IsNot Nothing AndAlso order.Status <> EkEnumeration.OrderStatus.Cancelled Then
            For i As Integer = 0 To (order.Parts.Count - 1)
                If order.Parts(i).DateShipped = DateTime.MinValue OrElse order.Parts(i).DateShipped = DateTime.MaxValue Then
                    canEditTracking = True
                    Exit For
                End If
            Next
        End If

        Return canEditTracking

    End Function

    Public Function Util_IsCaptured() As Boolean

        Dim isCaptured As Boolean = True

        If order IsNot Nothing Then
            For i As Integer = 0 To (order.Payments.Count - 1)
                If order.Payments(i).CapturedOn = DateTime.MinValue OrElse order.Payments(i).CapturedOn = DateTime.MaxValue Then
                    isCaptured = False
                    Exit For
                End If
            Next
        End If

        Return isCaptured

    End Function

    Public Function Util_ShowDate(ByVal dtDate As DateTime) As String
        Dim sRet As String = ""
        If dtDate = DateTime.MinValue Then
            sRet = "-"
        Else
            sRet = dtDate.ToShortDateString & " " & dtDate.ToShortTimeString
        End If
        Return sRet
    End Function

    Public Function Util_ShowStatus(ByVal status As EkEnumeration.OrderStatus) As String
        Dim statusText As String = ""
        If status = EkEnumeration.OrderStatus.Fraud Then
            statusText = "<img src=""" & AppImgPath & "alert.gif""/><span class=""important"">" & System.Enum.GetName(GetType(EkEnumeration.OrderStatus), status) & "</span>"
        ElseIf status = EkEnumeration.OrderStatus.Cancelled Then
            statusText = "<img src=""" & AppImgPath & "commerce/cancel.gif""/><span class=""important"">" & System.Enum.GetName(GetType(EkEnumeration.OrderStatus), status) & "</span>"
        Else
            statusText = System.Enum.GetName(GetType(EkEnumeration.OrderStatus), status)
        End If
        Return statusText
    End Function

    Public Function Util_ShowCustomer(ByVal Customer As CustomerData) As String
        Dim sRet As String = ""

        sRet &= "<a href=""customers.aspx?action=view&id=" & Customer.Id & """>" & Customer.FirstName & " " & Customer.LastName & " (" & Customer.DisplayName & ")</a>"
        sRet &= "<br/>Orders: " & Customer.TotalOrders
        sRet &= "<br/>Value:  " & defaultCurrency.ISOCurrencySymbol & EkFunctions.FormatCurrency(Customer.TotalOrderValue, defaultCurrency.CultureCode)
        sRet &= "<br/>Avg Value:  " & defaultCurrency.ISOCurrencySymbol & EkFunctions.FormatCurrency(Customer.AverageOrderValue, defaultCurrency.CultureCode)

        Return sRet
    End Function

    Public Sub Util_PopulateCustomer(ByVal Customer As CustomerData)

        ltr_customername.Text = "<a href=""customers.aspx?action=view&id=" & Customer.Id & """>" & Customer.FirstName & " " & Customer.LastName & " (" & Customer.DisplayName & ")</a>"
        If Not (Customer.IsDeleted) Then ltr_customername.Text &= "&#160;<a href=""#"" onclick=""if (allowOpen) { ektb_show('" & GetMessage("btn email") & "', '../email.aspx?userarray=" & Customer.Id.ToString() & "&fromModal=true&EkTB_iframe=true&width=460&height=425&scrolling=true&modal=true', false); } ""><img alt=""" & GetMessage("btn email") & """ title=""" & GetMessage("btn email") & """ src=""" & m_refContentApi.AppPath & "Images/ui/icons/email.png"" /></a>"
        ltr_customerorders.Text = Customer.TotalOrders
        ltr_customertotal.Text = defaultCurrency.ISOCurrencySymbol & EkFunctions.FormatCurrency(Customer.TotalOrderValue, defaultCurrency.CultureCode)
        ltr_customeravg.Text = defaultCurrency.ISOCurrencySymbol & EkFunctions.FormatCurrency(Customer.AverageOrderValue, defaultCurrency.CultureCode)

    End Sub

    Public Function Util_ShowAddress(ByVal ShippingAddressId As Long, ByVal isShipping As Boolean) As String
        Dim sbRet As New StringBuilder()
        Dim shipAddress As AddressData = Nothing

        shipAddress = AddressManager.GetItem(ShippingAddressId)
        sbRet.Append(shipAddress.AddressLine1).Append("<br />")
        If shipAddress.AddressLine2.Trim().Length() > 0 Then
            sbRet.Append(shipAddress.AddressLine2).Append("<br />")
        End If
        sbRet.Append(shipAddress.City).Append("<br />")
        sbRet.Append(shipAddress.Region.Name).Append(", ")
        sbRet.Append(shipAddress.PostalCode).Append("<br />")
        sbRet.Append(shipAddress.Country.Name).Append("<br />")

        If isShipping Then

            If order.Parts(0).ShippingMethod <> "" Then sbRet.Append("<br />Via ").Append(order.Parts(0).ShippingMethod)

            If order.Parts(0).TrackingNumber <> "" Then

                If Shipment.Provider.ShipmentProviderManager.Provider.IsTrackingSupported() Then

                    sbRet.Append("<br />").Append(GetMessage("lbl tracking number")).Append(": <a target=""_blank"" href=""").Append(Shipment.Provider.ShipmentProviderManager.Provider.GetTrackingUrl(order.Parts(0).TrackingNumber)).Append(""">").Append(order.Parts(0).TrackingNumber).Append("</a>")

                Else

                    sbRet.Append("<br />").Append(GetMessage("lbl tracking number")).Append(": ").Append(order.Parts(0).TrackingNumber)

                End If

            End If

        End If

        Return sbRet.ToString()

    End Function

    Protected Sub Util_SetJS()

        Dim sbJS As New StringBuilder()

        sbJS.Append("<script language=""javascript"">").Append(Environment.NewLine)

        Select Case m_sPageAction

            Case "trackingnumber", "showpayment"

                If Not Page.IsPostBack Then

                    sbJS.Append(" function ShowPayPalStatus(result, context) { ").Append(Environment.NewLine)

                    sbJS.Append("   document.getElementById('dvPaymentStatus').innerHTML = result; ").Append(Environment.NewLine)
                    'sbJS.Append("   alert(result); ").Append(Environment.NewLine)

                    sbJS.Append(" } ").Append(Environment.NewLine)

                    Dim transactionId As String = ""
                    If (Not IsNothing(payment) AndAlso payment.TransactionId.Length > 0) Then
                        transactionId = payment.TransactionId
                    End If
                    sbJS.Append(" function CheckPayPalStatus() { ").Append(Environment.NewLine)
                    sbJS.Append("   document.getElementById('dvPaymentStatus').innerHTML = '<img src=""" & Me.AppImgPath & "ajax-loader_circle_lg.gif"" />'; ").Append(Environment.NewLine)
                    sbJS.Append("   ").Append(Me.ClientScript.GetCallbackEventReference(Me, "'type=paypal&transId=" & transactionId & "'", "ShowPayPalStatus", "null")).Append(Environment.NewLine)
                    sbJS.Append(" } ").Append(Environment.NewLine)

                    sbJS.Append("</script>").Append(Environment.NewLine)

                    ltr_js.Text = sbJS.ToString()

                End If

            Case "editaddress"

                sbJS.Append("function isValid(phone)").Append(Environment.NewLine)
                sbJS.Append("{").Append(Environment.NewLine)
                sbJS.Append("   return /^(1\s*[-\/\.]?)?(\((\d{3})\)|(\d{3}))\s*([\s-./\\])?([0-9]*)([\s-./\\])?([0-9]*)$/.test(phone);").Append(Environment.NewLine)
                sbJS.Append("}").Append(Environment.NewLine)

                sbJS.Append("function CheckAddress() {").Append(Environment.NewLine)
                sbJS.Append("   var sAddrName = Trim(document.getElementById('").Append(txt_address_name.UniqueID).Append("').value); ").Append(Environment.NewLine)
                sbJS.Append("   var sCompany = Trim(document.getElementById('").Append(txt_address_company.UniqueID).Append("').value); ").Append(Environment.NewLine)
                sbJS.Append("   var sStrAddr = Trim(document.getElementById('").Append(txt_address_line1.UniqueID).Append("').value); ").Append(Environment.NewLine)
                sbJS.Append("   var sCity = Trim(document.getElementById('").Append(txt_address_city.UniqueID).Append("').value); ").Append(Environment.NewLine)
                sbJS.Append("   var iPostal = Trim(document.getElementById('").Append(txt_address_postal.UniqueID).Append("').value); ").Append(Environment.NewLine)
                sbJS.Append("   var iPhone = Trim(document.getElementById('").Append(txt_address_phone.UniqueID).Append("').value); ").Append(Environment.NewLine)
                sbJS.Append("   var drp_region = document.getElementById(""").Append(drp_address_region.UniqueID).Append(""");" & Environment.NewLine)
                sbJS.Append("   if(drp_region.selectedIndex == -1)").Append(Environment.NewLine)
                sbJS.Append("    {").Append(Environment.NewLine)
                sbJS.Append("       alert(""" & MyBase.GetMessage("js null postalcode region msg") & """);" & Environment.NewLine)
                sbJS.Append("       document.forms[""form1""].isCPostData.value = 'false';").Append(Environment.NewLine)
                sbJS.Append("       return false;").Append(Environment.NewLine)
                sbJS.Append("    }").Append(Environment.NewLine)
                sbJS.Append("   if(sAddrName == '' || sStrAddr == '' || sCity == '' || isNaN(iPostal) || iPostal == '' )").Append(Environment.NewLine)
                sbJS.Append("   {").Append(Environment.NewLine)
                sbJS.Append("       alert('").Append(GetMessage("js err invalid address values")).Append("');").Append(Environment.NewLine)
                sbJS.Append("       return false; ").Append(Environment.NewLine)
                sbJS.Append("   }").Append(Environment.NewLine)
                sbJS.Append("   else if( !isValid(iPhone) )").Append(Environment.NewLine)
                sbJS.Append("   {").Append(Environment.NewLine)
                sbJS.Append("       alert('").Append(GetMessage("js err invalid phone values")).Append("');").Append(Environment.NewLine)
                sbJS.Append("       return false; ").Append(Environment.NewLine)
                sbJS.Append("   }").Append(Environment.NewLine)
                sbJS.Append("   else").Append(Environment.NewLine)
                sbJS.Append("   {").Append(Environment.NewLine)
                sbJS.Append("       resetCPostback();").Append(Environment.NewLine)
                sbJS.Append("       document.forms[0].submit(); ").Append(Environment.NewLine)
                sbJS.Append("   }").Append(Environment.NewLine)
                sbJS.Append("} ").Append(Environment.NewLine)

                sbJS.Append("</script>").Append(Environment.NewLine)

                ltr_js.Text = sbJS.ToString()

            Case Else

                'sbJS.Append(JSLibrary.ToggleDiv())

                sbJS.Append("function ToggleDiv(sDiv, overrd) {" & Environment.NewLine)
                sbJS.Append("   var objcustom = document.getElementById(sDiv); " & Environment.NewLine)
                sbJS.Append("   var bOverRide = (overrd != null); " & Environment.NewLine)
                sbJS.Append("   if ((bOverRide && overrd) || (!bOverRide && objcustom.style.visibility == 'hidden')) { " & Environment.NewLine)
                ' sbJS.Append("       objcustom.style.position = ''; " & Environment.NewLine)
                sbJS.Append("       objcustom.style.visibility = 'visible';" & Environment.NewLine)
                sbJS.Append("   } else { " & Environment.NewLine)
                ' sbJS.Append("       objcustom.style.position = 'absolute'; " & Environment.NewLine)
                sbJS.Append("       objcustom.style.visibility = 'hidden';" & Environment.NewLine)
                sbJS.Append("   } " & Environment.NewLine)
                sbJS.Append("}" & Environment.NewLine)

                sbJS.Append("function CustomReport() {" & Environment.NewLine)
                sbJS.Append("   var sSites = ''; " & Environment.NewLine)
                sbJS.Append("   var LoopInt = 0; " & Environment.NewLine)
                sbJS.Append("   while ( document.getElementById('chk_site_' + LoopInt) != null ) { " & Environment.NewLine)
                sbJS.Append("       if ( document.getElementById('chk_site_' + LoopInt).checked ) { " & Environment.NewLine)
                sbJS.Append("           if (sSites == '') { sSites = document.getElementById('chk_site_' + LoopInt).value; } " & Environment.NewLine)
                sbJS.Append("           else { sSites = sSites + ',' + document.getElementById('chk_site_' + LoopInt).value; } " & Environment.NewLine)
                sbJS.Append("       } " & Environment.NewLine)
                sbJS.Append("       LoopInt = LoopInt + 1;" & Environment.NewLine)
                sbJS.Append("   } " & Environment.NewLine)
                sbJS.Append("   window.location = 'fulfillment.aspx?action=custom&sites=' + sSites; " & Environment.NewLine)
                sbJS.Append("}" & Environment.NewLine)

                sbJS.Append("function CancelOrder() {" & Environment.NewLine)
                sbJS.Append("   if (confirm('").Append(GetMessage("js confirm cancel order")).Append("')) " & Environment.NewLine)
                sbJS.Append("   { " & Environment.NewLine)
                sbJS.Append("       document.getElementById('hdn_code').value = 4; " & Environment.NewLine)
                sbJS.Append("       document.form1.submit(); " & Environment.NewLine)
                sbJS.Append("   }; " & Environment.NewLine)
                sbJS.Append("}" & Environment.NewLine)

                sbJS.Append("function CallOrderEvent(orderEvent) {" & Environment.NewLine)
                sbJS.Append("   if (confirm('").Append(GetMessage("js confirm call order event")).Append("')) " & Environment.NewLine)
                sbJS.Append("   { " & Environment.NewLine)
                sbJS.Append("       ektb_show('','#EkTB_inline?height=18&width=500&inlineId=dvHoldMessage&modal=true', null, '', true); " & Environment.NewLine)
                sbJS.Append("       document.getElementById('hdn_event').value = orderEvent; " & Environment.NewLine)
                sbJS.Append("       document.getElementById('hdn_code').value = 5; " & Environment.NewLine)
                sbJS.Append("       document.form1.submit(); " & Environment.NewLine)
                sbJS.Append("   }; " & Environment.NewLine)
                sbJS.Append("}" & Environment.NewLine)

                sbJS.Append("</script>").Append(Environment.NewLine)

                ltr_js.Text = sbJS.ToString()

        End Select

    End Sub

    Protected Sub Util_CheckAccess()
        bCommerceAdmin = Me.m_refContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin)
        Try
            If Not bCommerceAdmin Then
                Throw New Exception(GetMessage("err not role commerce-admin"))
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try

    End Sub

    Protected Sub Util_ShowSites()

        Dim literalReference As Literal = Nothing
        Dim header As System.Web.UI.Control = dg_orders.Controls(0).Controls(0)

        If header.FindControl("ltr_sites") IsNot Nothing Then literalReference = header.FindControl("ltr_sites")

        For index As Integer = 0 To (SiteList.Count - 1)

            literalReference.Text &= "<tr><td><input type=""checkbox"" checked=""checked"" id=""chk_site_" & index.ToString() & """ name=""chk_site_" & index.ToString() & """ value=""" & Server.HtmlEncode(SiteList(index)) & """ />" & SiteList(index) & "</td></tr>"

        Next

    End Sub

    Protected Function Util_AddSite(ByVal orderSite As String) As String

        If Not SiteList.Contains(orderSite) Then SiteList.Add(orderSite)
        Return orderSite

    End Function

    Protected Function Util_ShowConfig(ByVal config As OrderKitConfigData) As String

        Dim sbKit As New StringBuilder()

        If config.OrderItemId > 0 AndAlso config.KitOptions.Count > 0 Then

            For i As Integer = 0 To (config.KitOptions.Count - 1)

                sbKit.Append("<br />&#160;&#160;&#160;").Append(config.KitOptions(i).GroupName).Append(": ").Append(config.KitOptions(i).GroupOptionName)

            Next

        End If

        Return sbKit.ToString()

    End Function

    Protected Function Util_ShowCouponInfo(ByVal discountType As EkEnumeration.CouponDiscountType, ByVal discountValue As Decimal, ByVal currencyId As Long) As String

        If discountType = EkEnumeration.CouponDiscountType.Percent Then

            Return Format(discountValue, "0.00") & " %"

        Else

            Return Util_ShowPrice(discountValue, currencyId)

        End If

    End Function

    Protected Function Util_ShowPrice(ByVal price As Decimal) As String

        Return order.Currency.AlphaIsoCode & EkFunctions.FormatCurrency(price, order.Currency.CultureCode)

    End Function

    Protected Function Util_ShowPrice(ByVal price As Decimal, ByVal currencyId As Long) As String

        If currencyId = order.Currency.Id Then

            Return order.Currency.AlphaIsoCode & EkFunctions.FormatCurrency(price, order.Currency.CultureCode)

        Else

            Return order.Currency.AlphaIsoCode & EkFunctions.FormatCurrency(price, (New CurrencyApi()).GetItem(currencyId).CultureCode)

        End If

    End Function

    Protected Function Util_ShowIcon(ByVal authorized As DateTime, ByVal captured As DateTime) As String

        If captured = DateTime.MinValue OrElse captured = DateTime.MaxValue Then

            Return "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/calculator.png"" alt=""" & GetMessage("lbl authorized") & """ title=""" & GetMessage("lbl authorized") & """/>"

        Else

            Return "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/calculatorApprove.png"" alt=""" & GetMessage("lbl captured") & """ title=""" & GetMessage("lbl captured") & """/>"

        End If

    End Function

    Sub drp_address_country_ServerChange(ByVal sender As Object, ByVal e As System.EventArgs)

        Try

            Dim regioncriteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
            Dim cCountryId As Integer = Request.Form("drp_address_country")

            Util_BindRegions(cCountryId)

        Catch ex As Exception

            Utilities.ShowError(ex.Message)

        End Try

    End Sub

    Protected Sub Util_BindRegions(ByVal cCountryId As Integer)
        Dim RegionList As New System.Collections.Generic.List(Of RegionData)()
        Dim m_refRegion As RegionApi
        m_refRegion = New RegionApi() '(Me.m_refContentApi.RequestInformationRef)

        Dim criteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        criteria.AddFilter(RegionProperty.CountryId, CriteriaFilterOperator.EqualTo, cCountryId)
        criteria.AddFilter(RegionProperty.IsEnabled, CriteriaFilterOperator.EqualTo, True)
        criteria.PagingInfo.RecordsPerPage = 10000

        RegionList = m_refRegion.GetList(criteria)

        If RegionList IsNot Nothing AndAlso RegionList.Count > 0 Then

            drp_address_region.Items.Clear()
            drp_address_region.DataSource = RegionList
            drp_address_region.DataTextField = "Name"
            drp_address_region.DataValueField = "Id"
            drp_address_region.DataBind()

        Else

            drp_address_region.DataSource = ""
            drp_address_region.DataTextField = "Name"
            drp_address_region.DataValueField = "Id"
            drp_address_region.DataBind()

        End If

        drp_address_country.SelectedValue = cCountryId

    End Sub

    Protected Function Util_FindItem(ByVal Id As Integer, ByVal droptype As String) As Integer
        Dim iRet As Integer = 0
        Select Case droptype
            Case "region"
                For i As Integer = 0 To (drp_address_region.Items.Count - 1)
                    If drp_address_region.Items(i).Value = Id Then iRet = i
                Next
            Case "country"
                For i As Integer = 0 To (drp_address_country.Items.Count - 1)
                    If drp_address_country.Items(i).Value = Id Then iRet = i
                Next
        End Select
        Return iRet
    End Function

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

        Select Case Me.m_sPageAction
            Case "mostrecent"
                Display_MostRecent()
            Case "bydates"
                Display_ByDates()
            Case "byproduct"
                Display_ByProduct()
            Case "bycustomer"
                Display_ByCustomer()
            Case "onhold"
                Display_ViewOnHold()
            Case Else ' "viewall"
                Display_ViewAll()
        End Select
        isPostData.Value = "true"
    End Sub
    Private Sub Util_SetServerJSVariable()
        ltr_captOrder.Text = GetMessage("js confirm capture order?")
        ltr_msgconfirmCaptureSettle.Text = GetMessage("js confirm capture settle payment?")
        ltr_msgconfirmSettle.Text = GetMessage("js confirm settled order?")
        ltr_editNotes.Text = GetMessage("js confirm edit notes?")
        ltr_markFraud.Text = GetMessage("js confirm mark fraud?")
        ltr_addTrackNumb.Text = GetMessage("lbl add tracking number")
        ltr_mIID.Text = m_iID.ToString()
        lbl_ok.Text = GetMessage("lbl ok")
        ltr_summary.Text = GetMessage("summary text")
        ltr_dvstatus.Text = GetMessage("generic status")
        ltr_addresses.Text = GetMessage("lbl map address")
        ltr_description.Text = GetMessage("lbl description")
        ltr_workflow.Text = GetMessage("lbl workflow")
        ltr_payment.Text = GetMessage("lbl payment")
        ltr_coupon.Text = GetMessage("lbl coupons")
    End Sub
    Private Sub Util_RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "csslib/box.css", "EktronBoxCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "csslib/pop_style.css", "EktronPopStyleCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)
    End Sub
#End Region

#Region "CallBack"


    Dim callbackresult As String = ""

    Public Function GetCallbackResult() As String Implements ICallbackEventHandler.GetCallbackResult

        Return callbackresult

    End Function

    Public Sub RaiseCallbackEvent(ByVal eventArgument As String) Implements ICallbackEventHandler.RaiseCallbackEvent

        Dim paypalManager As Ektron.Cms.Commerce.PaymentMethods.IPayPal = ObjectFactory.GetPayPal()
        Dim paypalResp As Ektron.Cms.Commerce.PaymentMethods.PayPalResponse = Nothing
        Dim _callBackData As NameValueCollection = Nothing

        Try

            If Not Me.m_refContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin) Then

                Throw New Exception(GetMessage("err not role commerce-admin"))

            End If

            _callBackData = System.Web.HttpUtility.ParseQueryString(eventArgument)

            paypalManager.InitializeFromGateway()

            paypalResp = paypalManager.GetTransactionDetails(_callBackData("transId"))

            callbackresult &= GetMessage("type label") & " " & paypalResp.ResponseFields("PAYMENTTYPE")
            callbackresult &= "<br />" & GetMessage("generic status") & ": " & paypalResp.ResponseFields("PAYMENTSTATUS")

        Catch ex As Exception

            callbackresult = "<img src=""" & AppImgPath & "alert.gif""><span class=""important"">" & ex.Message & "</span>"

        End Try

    End Sub


#End Region

End Class
