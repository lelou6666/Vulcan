Imports Ektron
Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Workarea
Imports System.Web.HttpRequest
Imports System.Data
Imports System.Web.UI.page

Partial Class Commerce_customers
    Inherits workareabase

    Protected m_sPageName As String = "customers.aspx"
    Protected m_iCustomerId As Long = 0
    Protected AddressManager As AddressApi = Nothing
    Protected CustomerManager As CustomerApi = Nothing
    Protected RegionManager As RegionApi = Nothing
    Protected CountryManager As CountryApi = Nothing
    Protected cCustomer As CustomerData = Nothing
    Protected defaultCurrency As CurrencyData = Nothing
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1
    Protected basketApi As New BasketApi
    Protected AppPath As String = ""

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        RegisterResources()
        AppPath = m_refContentApi.ApplicationPath

        Try

            Utilities.ValidateUserLogin()
            CommerceLibrary.CheckCommerceAdminAccess()

            Dim siteCookie As System.Web.HttpCookie = CommonApiBase.GetEcmCookie()
            Dim m_refCurrencyApi As New Ektron.Cms.Commerce.CurrencyApi()
            defaultCurrency = (New CurrencyApi()).GetItem(m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId)

            If siteCookie("SiteCurrency") <> defaultCurrency.Id Then
                defaultCurrency.Id = siteCookie("SiteCurrency")
                defaultCurrency = m_refCurrencyApi.GetItem(defaultCurrency.Id)
            End If

            If Request.QueryString("customerid") <> "" Then
                m_iCustomerId = Request.QueryString("customerid")
            End If
            defaultCurrency = (New CurrencyApi()).GetItem(m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId)
            CustomerManager = New CustomerApi()
            AddressManager = New AddressApi()

            If siteCookie("SiteCurrency") <> defaultCurrency.Id Then
                defaultCurrency.Id = siteCookie("SiteCurrency")
                defaultCurrency = m_refCurrencyApi.GetItem(defaultCurrency.Id)
            End If

            Select Case MyBase.m_sPageAction
                Case "addeditaddress"
                    RegionManager = New RegionApi()
                    CountryManager = New CountryApi()
                    If Page.IsPostBack Then
                        If Request.Form(isCPostData.UniqueID) = "" Then
                            Process_ViewAddress()
                        End If
                    Else
                        Display_ViewAddress(True)
                    End If
                Case "viewaddress"
                    RegionManager = New RegionApi()
                    CountryManager = New CountryApi()
                    Display_ViewAddress(False)
                Case "viewbasket"
                    Display_ViewBaskets(False)
                Case "view"
                    Display_View()
                Case "deleteaddress"
                    Process_AddressDelete()
                Case "deletebasket"
                    Process_BasketDelete()
                Case Else ' "viewall"
                    If Page.IsPostBack = False Then
                        Display_View_All()
                    End If
            End Select
            Util_SetLabels()
            Util_SetJS()
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

#Region "Display"
    Protected Sub Display_ViewAddress(ByVal WithEdit As Boolean)
        pnl_view.Visible = False
        pnl_viewall.Visible = False
        Dim aAddress As AddressData = Nothing
        RegionManager = New RegionApi()

        Dim regioncriteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        If Not Me.m_iID > 0 Then
            regioncriteria.AddFilter(RegionProperty.CountryId, CriteriaFilterOperator.EqualTo, drp_address_country.SelectedIndex)
        End If
        regioncriteria.PagingInfo.RecordsPerPage = 1000
        drp_address_region.DataTextField = "Name"
        drp_address_region.DataValueField = "Id"
        drp_address_region.DataSource = RegionManager.GetList(regioncriteria)
        drp_address_region.DataBind()

        Dim addresscriteria As New Ektron.Cms.Common.Criteria(Of CountryProperty)(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        addresscriteria.PagingInfo.RecordsPerPage = 1000
        drp_address_country.DataTextField = "Name"
        drp_address_country.DataValueField = "Id"
        drp_address_country.DataSource = CountryManager.GetList(addresscriteria)
        drp_address_country.DataBind()

        If Me.m_iID > 0 Then
            cCustomer = CustomerManager.GetItem(Me.m_iCustomerId)
            aAddress = AddressManager.GetItem(Me.m_iID)
            regioncriteria.AddFilter(RegionProperty.CountryId, CriteriaFilterOperator.EqualTo, aAddress.Country.Id)

            drp_address_region.DataSource = RegionManager.GetList(regioncriteria)
            ltr_address_id.Text = aAddress.Id
            txt_address_name.Text = aAddress.Name
            txt_address_company.Text = aAddress.Company
            txt_address_line1.Text = aAddress.AddressLine1
            txt_address_line2.Text = aAddress.AddressLine2
            txt_address_city.Text = aAddress.City
            drp_address_country.SelectedIndex = FindItem(aAddress.Country.Id, "country")
            Util_BindRegions(aAddress.Country.Id)
            drp_address_region.SelectedValue = aAddress.Region.Id
            txt_address_postal.Text = aAddress.PostalCode
            txt_address_phone.Text = aAddress.Phone
            chk_default_billing.Checked = (aAddress.Id = cCustomer.BillingAddressId)
            chk_default_shipping.Checked = (aAddress.Id = cCustomer.ShippingAddressId)
        End If
        ToggleAddressFields(WithEdit)
    End Sub

    Protected Sub Display_ViewBaskets(ByVal WithEdit As Boolean)

        Dim currentBasket As Ektron.Cms.Commerce.Basket
        Dim basketId As Long = 0
        Dim _CouponApi As New CouponApi
        Dim basketCouponData As BasketCalculatorData

        If Request.QueryString("basketid") IsNot Nothing AndAlso Request.QueryString("basketid") <> "" Then
            basketId = Request.QueryString("basketid")
        End If

        If basketId > 0 Then
            currentBasket = basketApi.GetItem(basketId)
            If currentBasket IsNot Nothing AndAlso currentBasket.Items.Count > 0 Then
                dg_viewbasket.DataSource = currentBasket.Items
                dg_viewbasket.DataBind()
                Dim basketCalc As New BasketCalculator(currentBasket, Me.m_refContentApi.RequestInformationRef)

                basketCouponData = _CouponApi.CalculateBasketCoupons(basketId)

                If basketCouponData.BasketCoupons.Count > 0 Then
                    ltr_noitems.Text = "<hr/><table width=""100%"">"
                    ltr_noitems.Text &= "<tr><td align=""right"" width=""90%"">" & GetMessage("lbl coupon discount") & "</td><td align=""right"">(" & Ektron.Cms.Common.EkFunctions.FormatCurrency(basketCouponData.TotalCouponDiscount, defaultCurrency.CultureCode) & ")</td></tr>"
                    ltr_noitems.Text &= "<tr><td align=""right"" width=""90%"">" & GetMessage("lbl total") & ": </td><td align=""right"">" & Ektron.Cms.Common.EkFunctions.FormatCurrency(basketCouponData.BasketTotal, defaultCurrency.CultureCode) & "</td></tr>"
                    ltr_noitems.Text &= "</table>"
                End If
            Else
                ltr_noitems.Text = Me.GetMessage("lbl no items")
            End If
        Else
            ltr_noitems.Text = Me.GetMessage("lbl no items")
        End If

    End Sub
    Protected Function showconfig(ByVal kitconfig As KitConfigData) As String
        If kitconfig IsNot Nothing Then
            Dim sb As New StringBuilder

            sb.Append("<table>").Append(Environment.NewLine)
            For i As Integer = 0 To kitconfig.Groups.Count - 1
                If kitconfig.Groups.Item(i).Options(0).Name <> "" Then

                    sb.Append("<tr><td>&nbsp;&nbsp;&nbsp;<label id=""kit" & i & """>" & kitconfig.Groups.Item(i).Name & "</label>").Append(Environment.NewLine)
                    sb.Append("<label id=""lbl_colon"">:&nbsp;</label><label id=""lbl_desc" & i & """>" & kitconfig.Groups.Item(i).Options(0).Name & "</label></td></tr>").Append(Environment.NewLine)
                    sb.Append("</td></tr>").Append(Environment.NewLine)

                End If
            Next
            sb.Append("</table>").Append(Environment.NewLine)
            Return sb.ToString()
        Else
            Return System.String.Empty
        End If
    End Function

    Protected Function showvariant(ByVal variantconfig As BasketVariantData) As String

        If variantconfig IsNot Nothing AndAlso variantconfig.Id > 0 Then

            Dim sb As New StringBuilder

            sb.Append("<table>").Append(Environment.NewLine)
            sb.Append("<tr><td>&nbsp;&nbsp;&nbsp;<label id=""variant"">" & variantconfig.Title & ":</label>" & variantconfig.Sku & "</td></tr>").Append(Environment.NewLine)
            sb.Append("</table>").Append(Environment.NewLine)

            Return sb.ToString()

        Else

            Return System.String.Empty

        End If

    End Function

    Protected Sub Display_View()
        pnl_viewall.Visible = False
        Dim orderList As New List(Of OrderData)
        Dim aAddreses As List(Of AddressData) = New List(Of AddressData)()
        Dim basketList As List(Of Ektron.Cms.Commerce.Basket)

        Dim orderApi As New OrderApi
        Dim basketApi As New BasketApi
        ' customer
        cCustomer = CustomerManager.GetItem(Me.m_iID)
        m_iCustomerId = cCustomer.Id
        Me.ltr_id.Text = cCustomer.Id
        Me.ltr_uname.Text = cCustomer.UserName
        Me.ltr_fname.Text = cCustomer.FirstName
        Me.ltr_lname.Text = cCustomer.LastName

        Me.ltr_dname.Text = cCustomer.DisplayName
        Me.ltr_ordertotal.Text = cCustomer.TotalOrders
        Me.ltr_orderval.Text = defaultCurrency.ISOCurrencySymbol & EkFunctions.FormatCurrency(cCustomer.TotalOrderValue, defaultCurrency.CultureCode)
        Me.ltr_pervalue.Text = defaultCurrency.ISOCurrencySymbol & EkFunctions.FormatCurrency(cCustomer.AverageOrderValue, defaultCurrency.CultureCode)
        ' customer
        ' orders
        Dim orderCriteria As New Criteria(Of OrderProperty)
        orderCriteria.AddFilter(OrderProperty.CustomerId, CriteriaFilterOperator.EqualTo, m_iID)
        orderList = orderApi.GetList(orderCriteria)
        If orderList.Count = 0 Then
            ltr_orders.Text = Me.GetMessage("lbl no orders")
        End If
        dg_orders.DataSource = orderList
        dg_orders.DataBind()
        ' orders
        ' addresses
        aAddreses = AddressManager.GetList(m_iID)
        If aAddreses.Count = 0 Then
            ltr_address.Text = Me.GetMessage("lbl no addresses")
        End If
        dg_address.DataSource = aAddreses
        dg_address.DataBind()
        ' addresses
        ' baskets
        If Me.m_iID > 0 Then
            basketList = basketApi.GetList(Me.m_iID)
            If basketList.Count = 0 Then
                ltr_baskets.Text = Me.GetMessage("lbl no baskets")
            End If
            dg_baskets.DataSource = basketList
            dg_baskets.DataBind()
        End If
    End Sub

    Protected Sub Display_View_All()
        Dim aCustomers As New System.Collections.Generic.List(Of CustomerData)()
        Dim cCriteria As New Ektron.Cms.Common.Criteria(Of CustomerProperty)
        cCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        cCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()
        ' aCustomers = Customer.GetAllCustomers(1, 1, 1, 1, Me.m_refContentApi.RequestInformationRef)

        aCustomers = CustomerManager.GetList(cCriteria)
        TotalPagesNumber = cCriteria.PagingInfo.TotalPages

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
        dg_customers.DataSource = aCustomers
        dg_customers.DataBind()
    End Sub
#End Region

#Region "Process"

    Protected Sub Process_ViewAddress()
        Dim aAddress As AddressData = Nothing
        Dim originalAddressId As Long = Me.m_iID

        'need to get customer before address update to see if default addresses have changed.
        cCustomer = CustomerManager.GetItem(m_iCustomerId)
        aAddress = IIf(Me.m_iID > 0, AddressManager.GetItem(Me.m_iID), New AddressData())

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

        If Me.m_iID > 0 Then
            AddressManager.Update(aAddress)
        Else
            AddressManager.Add(aAddress, m_iCustomerId)
        End If

        Me.m_iID = aAddress.Id

        Dim updateBilling As Boolean = False
        Dim updateShipping As Boolean = False

        If chk_default_billing.Checked Then
            cCustomer.BillingAddressId = aAddress.Id
            updateBilling = True
        End If

        If chk_default_shipping.Checked Then
            cCustomer.ShippingAddressId = aAddress.Id
            updateShipping = True
        End If

        'if the default addresses have been unchecked - need to reset them to 0.
        If Not chk_default_billing.Checked And cCustomer.BillingAddressId = originalAddressId Then
            cCustomer.BillingAddressId = 0
            updateBilling = True
        End If

        If Not chk_default_shipping.Checked And cCustomer.ShippingAddressId = originalAddressId Then
            cCustomer.ShippingAddressId = 0
            updateShipping = True
        End If

        If updateBilling Then
            CustomerManager.ChangeBillingAddress(m_iCustomerId, cCustomer.BillingAddressId)
        End If
        If updateShipping Then
            CustomerManager.ChangeShippingAddress(m_iCustomerId, cCustomer.ShippingAddressId)
        End If

        Dim pagemode As String = "&page=" & Request.QueryString("page")
        Response.Redirect(Me.m_sPageName & "?action=viewaddress&id=" & Me.m_iID.ToString() & "&customerid=" & Me.m_iCustomerId.ToString() & pagemode, False)
    End Sub
    Protected Sub Process_AddressDelete()
        Dim aAddress As AddressData = Nothing
        aAddress = IIf(Me.m_iID > 0, AddressManager.GetItem(Me.m_iID), New AddressData())

        If Me.m_iID > 0 Then
            AddressManager.Delete(Me.m_iID)
            Response.Redirect(Me.m_sPageName, False)
        End If
    End Sub
    Protected Sub Process_BasketDelete()
        Dim bBasket As Basket = Nothing

        bBasket = IIf(Me.m_iID > 0, basketApi.GetItem(Me.m_iID), New BasketData())

        If Me.m_iID > 0 Then
            basketApi.Delete(Me.m_iID)
            Response.Redirect(Me.m_sPageName, False)
        End If
    End Sub
#End Region

    Protected Sub Util_SetLabels()
        Me.ltr_id_label.Text = Me.GetMessage("lbl customer id")
        Me.ltr_uname_lbl.Text = Me.GetMessage("lbl customer username")
        Me.ltr_fname_lbl.Text = Me.GetMessage("lbl customer firstname")
        Me.ltr_lname_lbl.Text = Me.GetMessage("lbl customer lastname")
        Me.ltr_dname_lbl.Text = Me.GetMessage("lbl customer displayname")
        Me.ltr_ordertotal_lbl.Text = Me.GetMessage("lbl order total")
        Me.ltr_orderval_lbl.Text = Me.GetMessage("lbl order value")
        Me.ltr_pervalue_lbl.Text = Me.GetMessage("lbl per order value")
        Me.ltr_address_id_lbl.Text = Me.GetMessage("generic id")
        Me.ltr_address_name.Text = Me.GetMessage("lbl address name")
        Me.ltr_address_company.Text = Me.GetMessage("lbl address company")
        Me.ltr_address_line1.Text = Me.GetMessage("lbl address street")
        Me.ltr_address_city_lbl.Text = Me.GetMessage("lbl address city")
        Me.ltr_address_region.Text = Me.GetMessage("lbl address state province")
        Me.ltr_address_postal.Text = Me.GetMessage("lbl address postal")
        Me.ltr_address_country.Text = Me.GetMessage("lbl address country")
        Me.ltr_address_phone.Text = Me.GetMessage("lbl address phone")
        Me.ltr_default_billing.Text = Me.GetMessage("lbl default billing address")
        Me.ltr_default_shipping.Text = Me.GetMessage("lbl default shipping address")
        Dim pagemode As String = "&page=" & Request.QueryString("page")
        Select Case MyBase.m_sPageAction
            Case "addeditaddress"
                pnl_viewaddress.Visible = True
                Me.tr_address_id.Visible = (Me.m_iID > 0)
                Me.AddButtonwithMessages(AppPath & "images/UI/Icons/save.png", "#", "btn save", "btn save", " onclick=""CheckAddress(); return false;"" ")
                Me.AddBackButton(Me.m_sPageName & IIf(m_iID > 0, "?action=viewaddress&id=" & Me.m_iID.ToString() & "&customerid=" & Me.m_iCustomerId.ToString(), "?action=view&id=" & Me.m_iCustomerId.ToString() & pagemode & ""))
                Me.SetTitleBarToString(Me.GetMessage(IIf(Me.m_iID > 0, "lbl edit address", "lbl add address")))
                Me.AddHelpButton(IIf(Me.m_iID > 0, ("editaddress"), ("addaddress")))
            Case "viewaddress"
                pnl_viewaddress.Visible = True
                Me.AddButtonwithMessages(AppPath & "images/UI/Icons/contentEdit.png", Me.m_sPageName & "?action=addeditaddress&id=" & Me.m_iID & "&customerid=" & Me.m_iCustomerId.ToString() & pagemode, "generic edit title", "generic edit title", "")
                Me.AddButtonwithMessages(AppPath & "images/UI/Icons/delete.png", Me.m_sPageName & "?action=deleteaddress&id=" & Me.m_iID.ToString() & pagemode, "alt del address button text", "btn delete", " onclick="" return CheckDelete();"" ")
                Me.AddBackButton(Me.m_sPageName & "?action=view&id=" & Me.m_iCustomerId.ToString() & pagemode)
                Me.SetTitleBarToMessage("lbl view address")
                Me.AddHelpButton("viewaddress")
            Case "viewbasket"
                pnl_viewbasket.Visible = True
                Me.AddButtonwithMessages(AppPath & "images/UI/Icons/delete.png", Me.m_sPageName & "?action=deletebasket&id=" & Request.QueryString("basketid").ToString(), "alt del basket button text", "btn delete", " onclick="" if(CheckDeleteBasket()){ parent.ektb_remove(); history.go(-1); return true;} else {return false; };"" ")
                PageLabel.Visible = False
                OfLabel.Visible = False
                FirstPage.Visible = False
                lnkBtnPreviousPage.Visible = False
                NextPage.Visible = False
                LastPage.Visible = False
                'Me.AddBackButton(Me.m_sPageName & "?action=view&id=" & Me.m_iCustomerId.ToString())
                Me.SetTitleBarToMessage("lbl view basket")
                Me.AddHelpButton("viewcart")
            Case "view"
                Me.pnl_view.Visible = True
                'MyBase.Tabs.On()
                'MyBase.Tabs.AddTabByMessage("properties text", "dvProp")
                'MyBase.Tabs.AddTabByMessage("lbl orders", "dvOrders")
                'Me.Tabs.AddTabByMessage("lbl addresses", "dvAddress")
                'Me.Tabs.AddTabByMessage("lbl baskets", "dvBaskets")
                Dim result As StringBuilder = New StringBuilder()
                result.Append("<script language=""javascript""> ")
                result.Append("var filemenu = new Menu( ""file"" ); ")
                result.Append("filemenu.addItem(""&nbsp;<img valign='center' src='" & AppPath & "images/ui/icons/vcard.png" & "' />&nbsp;&nbsp;" & Me.GetMessage("lbl address") & """, function() { window.location.href = '" & Me.m_sPageName & "?action=addeditaddress&customerid=" & m_iCustomerId.ToString() & pagemode & "'; } ); ")
                result.Append("MenuUtil.add( filemenu ); ")
                result.Append("</script>" & Environment.NewLine)
                result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'file');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'file');"" onmouseout=""this.className='menuRootItem'""><span class=""new"">" & Me.GetMessage("lbl New") & "</span></td>")
                Me.AddButtonText(result.ToString())
                If (Request.QueryString("page") = "workarea") Then
                    ' redirect to workarea when user clicks back button if we're in workarea
                    MyBase.AddButtonwithMessages(apppath & "images/UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=""javascript:top.switchDesktopTab()"" ")
                Else
                    Me.AddBackButton(Me.m_sPageName)
                End If
                Me.SetTitleBarToMessage("lbl view customer")
                Me.AddHelpButton("viewcustomer")
            Case Else ' "viewall"
                Me.SetTitleBarToMessage("lbl customers")
                Me.AddHelpButton("customers")
        End Select

    End Sub

    Protected Sub Util_SetJS()

        Dim sbJS As New StringBuilder()

        sbJS.Append("function CheckDelete()" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("    return confirm('").Append(GetMessage("js address confirm del")).Append("');" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function CheckDeleteBasket()" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("    return confirm('").Append(GetMessage("js basket confirm del")).Append("');" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function isValid(phone)").Append(Environment.NewLine)
        sbJS.Append("{").Append(Environment.NewLine)
        sbJS.Append("   return /^(1\s*[-\/\.]?)?(\((\d{3})\)|(\d{3}))\s*([\s-./\\])?([0-9]*)([\s-./\\])?([0-9]*)$/.test(phone);").Append(Environment.NewLine)
        sbJS.Append("}").Append(Environment.NewLine)

        sbJS.Append("function CheckAddress() {").Append(Environment.NewLine)
        sbJS.Append("   var sAddrName = Trim(document.getElementById('").Append(txt_address_name.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var sCompany = Trim(document.getElementById('").Append(txt_address_company.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var sStrAddr = Trim(document.getElementById('").Append(txt_address_line1.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var sStrAddr2 = Trim(document.getElementById('").Append(txt_address_line2.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var sCity = Trim(document.getElementById('").Append(txt_address_city.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var iPostal = Trim(document.getElementById('").Append(txt_address_postal.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var iPhone = Trim(document.getElementById('").Append(txt_address_phone.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var drp_region = document.getElementById(""").Append(drp_address_region.UniqueID).Append(""");" & Environment.NewLine)
        sbJS.Append("   if(sAddrName.indexOf('<') > -1 ||sAddrName.indexOf('>') > -1 ||sCompany.indexOf('>') > -1 || sCompany.indexOf('<') > -1 || sStrAddr.indexOf('<') > -1 ||sStrAddr.indexOf('>') > -1 || sStrAddr2.indexOf('<') > -1 || sStrAddr2.indexOf('>') > -1 || sCity.indexOf('<') > -1 || sCity.indexOf('>') > -1 ){").Append(Environment.NewLine)
        sbJS.Append("        alert(""").Append(String.Format(GetMessage("js: alert address cant include"), "<, >")).Append(""");").Append(Environment.NewLine)
        sbJS.Append("			document.getElementById('").Append(txt_address_name.UniqueID).Append("').focus(); return false;").Append(Environment.NewLine)
        sbJS.Append("		} ").Append(Environment.NewLine)
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

        ltr_js.Text = sbJS.ToString()

    End Sub

    Protected Function ShowName(ByVal Name As String, ByVal Company As String) As String
        Dim sRet As String = ""
        sRet &= IIf(Name <> "", Name & IIf(Company <> "", "<br />" & Company, ""), Company)
        Return sRet
    End Function

    Protected Function ShowOptionalLine(ByVal textValue As String) As String
        Return IIf((Not IsNothing(textValue) AndAlso textValue.Length > 0), textValue & " <br /> ", "")
    End Function

    Protected Function FindItem(ByVal Id As Integer, ByVal droptype As String) As Integer
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

    Protected Function Util_IsDefaultShipping(ByVal addressId As Long) As Boolean
        Return (addressId = cCustomer.ShippingAddressId)
    End Function

    Protected Function Util_IsDefaultBilling(ByVal addressId As Long) As Boolean
        Return (addressId = cCustomer.BillingAddressId)
    End Function

    Protected Sub ToggleAddressFields(ByVal Toggle As Boolean)
        txt_address_name.Enabled = Toggle
        txt_address_company.Enabled = Toggle
        txt_address_line1.Enabled = Toggle
        txt_address_line2.Enabled = Toggle
        txt_address_city.Enabled = Toggle
        drp_address_region.Enabled = Toggle
        txt_address_postal.Enabled = Toggle
        drp_address_country.Enabled = Toggle
        txt_address_phone.Enabled = Toggle
        chk_default_shipping.Enabled = Toggle
        chk_default_billing.Enabled = Toggle
    End Sub
    Sub drp_address_country_ServerChange(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim regioncriteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim cCountryId As Integer = Request.Form("drp_address_country")
        Util_BindRegions(cCountryId)
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
        Display_View_All()
        isPostData.Value = "true"
    End Sub
    Private Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "csslib/box.css", "EktronBoxCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)

    End Sub
End Class
