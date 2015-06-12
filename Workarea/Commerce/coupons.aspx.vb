Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea
Imports System.Collections.Generic
Imports ektron.Cms.Workarea.workareacommerce

Partial Class Commerce_coupons
    Inherits workareabase

    Protected m_sCurrencyCharacter As String = "$"
    Protected m_sPageName As String = "coupons.aspx"
    Protected CouponManager As CouponApi
    Protected IsUsed As Boolean = False
    Protected IsActive As Boolean = False
    Protected m_refCurrency As Currency = Nothing
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1
    Protected AppPath As String = ""

#Region "Page Functions"
    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        m_refCurrency = New Currency(m_refContentApi.RequestInformationRef)
        AppPath = m_refContentApi.AppPath
        RegisterResources()
        Try
            CouponManager = New CouponApi()
            Util_CheckAccess()
            drp_discounttype.Items.Add(New ListItem(GetMessage("lbl coupon amount"), "0"))
            drp_discounttype.Items.Add(New ListItem(GetMessage("lbl coupon amount percent"), "1"))
            drp_discounttype.Items.Add(New ListItem(GetMessage("lbl free shipping"), "2"))
            drp_discounttype.Attributes.Add("onchange", "document.getElementById('txt_discountval').disabled = (this.selectedIndex == 2); document.getElementById('sel_currency').disabled = (this.selectedIndex == 1 || this.selectedIndex == 2);")
            rad_type.Items.Add(New ListItem(GetMessage("lbl coupon type basket"), EkEnumeration.CouponType.BasketLevel))
            rad_type.Items.Add(New ListItem(GetMessage("lbl coupon type basket item most exp"), EkEnumeration.CouponType.MostExpensiveItem))
            rad_type.Items.Add(New ListItem(GetMessage("lbl coupon type basket item least exp"), EkEnumeration.CouponType.LeastExpensiveItem))
            rad_type.Items.Add(New ListItem(GetMessage("lbl coupon type basket item all"), EkEnumeration.CouponType.AllItems))
            Select Case m_sPageAction
                Case "addedit"
                    Util_SetJS()
                    If Page.IsPostBack Then Process_AddEdit() Else Display_AddEdit()
                    Util_SetDropEnabled()
                Case "view"
                    Display_View()
                Case "delete"
                    Process_Delete()
                Case "deactivate"
                    Process_Deactivate()
                Case Else
                    If Page.IsPostBack = False Then
                        Display_View_All()
                    End If
            End Select
            Util_SetLabels()
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region
#Region "Display"
    Protected Sub Display_View_All()
        Dim CouponList As New List(Of CouponData)
        Dim criteria As New Criteria(Of CouponProperty)
        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        criteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        CouponList = CouponManager.GetList(criteria)
        TotalPagesNumber = criteria.PagingInfo.TotalPages

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
        dg_coupon.DataSource = CouponList
        dg_coupon.DataBind()
    End Sub
    Protected Sub Display_View()
        Dim cCoupon As CouponData
        Dim AppliedList As New List(Of ProductCouponEntryData)

        cCoupon = CouponManager.GetItem(m_iID)
        AppliedList = CouponManager.GetProductList(m_iID)

        pnl_view.Visible = True
        pnl_viewall.Visible = False

        Util_PopulateFields(cCoupon, False)
        Util_BuildAppliedTable(AppliedList)
        Util_EnableFields(False)
        para_options.Visible = False
    End Sub
    Protected Sub Display_AddEdit()
        Dim cCoupon As New CouponData()
        Dim AppliedList As New List(Of ProductCouponEntryData)

        If m_iID > 0 Then
            cCoupon = CouponManager.GetItem(m_iID)
            AppliedList = CouponManager.GetProductList(m_iID)
        Else
            tr_id.Visible = False
            tr_used.Visible = False
        End If

        pnl_view.Visible = True
        pnl_viewall.Visible = False

        Util_PopulateFields(cCoupon, True)
        Util_BuildAppliedTable(AppliedList)
        Util_EnableFields(True)
        Util_SetJS()
    End Sub
#End Region
#Region "Process"

    Protected Sub Process_Deactivate()
        If m_iID > 0 Then
            CouponManager.Deactivate(m_iID)
            Response.Redirect(m_sPageName, False)
        End If
    End Sub

    Protected Sub Process_Delete()
        If m_iID > 0 Then
            If CouponManager.IsCouponUsedForOrder(m_iID) Then
                CouponManager.Deactivate(m_iID)
            Else
                CouponManager.Delete(m_iID)
            End If
            Response.Redirect(m_sPageName, False)
        End If
    End Sub

    Protected Sub Process_AddEdit()
        Dim cCoupon As New CouponData()
        If m_iID > 0 Then cCoupon = CouponManager.GetItem(m_iID)
        cCoupon.Description = txt_desc.Text
        cCoupon.Code = txt_code.Text
        cCoupon.IsActive = chk_active.Checked

        If (Request.Form("go_live") <> "") Then cCoupon.StartDate = EkFunctions.ReadDbDate(Trim(Request.Form("go_live")))
        If (Request.Form("end_date") <> "") Then cCoupon.ExpirationDate = EkFunctions.ReadDbDate(Trim(Request.Form("end_date")))

        If drp_discounttype.SelectedIndex = 2 Then
            cCoupon.DiscountType = EkEnumeration.CouponDiscountType.FreeShipping
            cCoupon.DiscountValue = 0
        ElseIf drp_discounttype.SelectedIndex = 1 Then
            cCoupon.DiscountType = EkEnumeration.CouponDiscountType.Percent
            cCoupon.DiscountValue = txt_discountval.Text
        Else
            cCoupon.DiscountType = EkEnumeration.CouponDiscountType.Amount
            cCoupon.DiscountValue = txt_discountval.Text
        End If
        cCoupon.CouponType = rad_type.SelectedValue
        cCoupon.OnePerCustomer = chk_oneper.Checked
        cCoupon.MaximumUses = EkFunctions.ReadIntegerValue(txt_expireafter.Text)
        cCoupon.MinimumAmount = EkFunctions.ReadDecimalValue(txt_minamount.Text)
        cCoupon.MaximumAmount = EkFunctions.ReadDecimalValue(txt_maxamount.Text)
        cCoupon.CurrencyId = hdn_currency.Value

        If m_iID > 0 Then
            CouponManager.Update(cCoupon)
            Process_AppliedTo(cCoupon.Id, False)
            Response.Redirect(m_sPageName & "?id=" & m_iID.ToString() & "&action=view", False)
        Else
            CouponManager.Add(cCoupon)
            Process_AppliedTo(cCoupon.Id, True)
            Response.Redirect(m_sPageName, False)
        End If
    End Sub
    Protected Sub Process_AppliedTo(ByVal couponId As Long, ByVal IsAdd As Boolean)
        Dim i As Integer = 1
        Dim exclusionIdList As New List(Of Long)
        Dim exclusionLanguageList As New List(Of Integer)
        While Request.Form("txtitemposidx" & i) IsNot Nothing
            Dim idx As Integer = Request.Form("txtitemposidx" & i)
            Dim iEntryId As Long = Request.Form("txtitemid" & idx)
            Dim iEntryLang As Long = Request.Form("txtitemlang" & idx)
            exclusionIdList.Add(iEntryId)
            exclusionLanguageList.Add(iEntryLang)
            If IsAdd OrElse Not CouponManager.IsCouponAppliedToProduct(couponId, iEntryId) Then
                CouponManager.AddCouponToProduct(couponId, iEntryId, iEntryLang)
            End If
            i = i + 1
        End While
        CouponManager.DeleteCouponProducts(couponId, exclusionIdList)
    End Sub
#End Region
#Region "Util"
    Protected Sub Util_SetLabels()
        lbl_id.Text = GetMessage("generic id")
        lbl_used.Text = GetMessage("lbl coupon used")
        ltr_code.Text = GetMessage("lbl coupon code")
        ltr_desc.Text = GetMessage("lbl coupon desc")
        ltr_type.Text = GetMessage("lbl coupon type")
        ltr_oneper.Text = GetMessage("lbl coupon one per")
        ltr_active.Text = GetMessage("lbl coupon active")
        ltr_discountval.Text = GetMessage("lbl coupon discount")
        ltr_minamount.Text = GetMessage("lbl coupon min ammount")
        ltr_maxamount.Text = GetMessage("lbl coupon max ammount")
        ltr_expireafter.Text = GetMessage("lbl coupon expires after")
        ltr_startdate.Text = GetMessage("lbl coupon start date")
        ltr_enddate.Text = GetMessage("lbl coupon exp date")
        ltr_instructions.Text = GetMessage("lbl select entries for coupon")
        Select Case m_sPageAction
            Case "view"

                Me.Tabs.On()
                Me.Tabs.AddTabByMessage("properties text", "dvProp")
                Tabs.AddTabByMessage("generic type", "dvType")
                Tabs.AddTabByMessage("generic options", "dvOptions")
                Me.Tabs.AddTabByMessage("lbl applies to", "dvApplies")
                SetTitleBarToMessage("lbl view coupon")
                Dim actionMenu As New workareamenu("action", Me.GetMessage("lbl action"), Me.AppPath & "images/UI/Icons/check.png")
                actionMenu.AddLinkItem(Me.AppImgPath & "edit.gif", Me.GetMessage("generic edit title"), m_sPageName & "?id=" & m_iID.ToString() & "&action=addedit")
                actionMenu.AddBreak()
                If IsActive Then actionMenu.AddItem(Me.AppImgPath & "commerce/coupon-inactive.gif", Me.GetMessage("lbl coupon mark inactive"), "if(confirm('" & GetMessage("js confirm coupon mark inactive") & "')) { window.location.href = '" & m_sPageName & "?id=" & m_iID.ToString() & "&action=deactivate" & "'; } ")
                If Not IsUsed Then actionMenu.AddItem(Me.AppPath & "images/UI/Icons/delete.png", Me.GetMessage("btn delete"), "if(confirm('" & GetMessage("js confirm coupon delete") & "')) { window.location.href = '" & m_sPageName & "?id=" & m_iID.ToString() & "&action=delete" & "'; } ")
                Me.AddMenu(actionMenu)
                Me.AddBackButton(Me.m_sPageName)

            Case "addedit"
                Me.Tabs.On()
                If m_iID = 0 Then
                    Tabs.ViewAsWizard()
                    Tabs.AddTabByString("1", "dvProp")
                    Tabs.AddTabByString("2", "dvType")
                    Tabs.AddTabByString("3", "dvOptions")
                    Tabs.AddTabByString("4", "dvApplies")
                Else
                    Me.Tabs.AddTabByMessage("properties text", "dvProp")
                    Tabs.AddTabByMessage("generic type", "dvType")
                    Tabs.AddTabByMessage("generic options", "dvOptions")
                    Me.Tabs.AddTabByMessage("lbl applies to", "dvApplies")
                End If
                If Me.m_iID > 0 Then SetTitleBarToMessage("lbl edit coupon") Else SetTitleBarToMessage("lbl add coupon")
                Dim actionMenu As New workareamenu("action", Me.GetMessage("lbl action"), Me.AppImgPath & "../UI/Icons/check.png")
                actionMenu.AddItem(AppPath & "images/ui/icons/save.png", Me.GetMessage("btn save"), "SubmitForm(" & EkEnumeration.AssetActionType.Save & ");")
                actionMenu.AddBreak()
                actionMenu.AddLinkItem(AppPath & "images/ui/icons/cancel.png", Me.GetMessage("generic cancel"), IIf(m_iID > 0, m_sPageName & "?id=" & m_iID.ToString() & "&action=view", m_sPageName))
                Me.AddMenu(actionMenu)
                If Me.m_iID > 0 Then Me.AddHelpButton("editcoupon") Else Me.AddHelpButton("addcoupon")
            Case Else
                SetTitleBarToMessage("lbl coupons")
                Dim newMenu As New workareamenu("file", Me.GetMessage("lbl new"), AppPath & "images/UI/Icons/star.png")
                newMenu.AddLinkItem(Me.AppImgPath & "commerce/coupon.gif", Me.GetMessage("lbl coupon"), m_sPageName & "?action=addedit")
                Me.AddMenu(newMenu)
                Me.AddHelpButton("coupons")
        End Select

    End Sub
    Protected Sub Util_EnableFields(ByVal toggle As Boolean)
        txt_code.Enabled = toggle
        txt_desc.Enabled = toggle
        chk_active.Enabled = toggle
        chk_oneper.Enabled = toggle
        txt_discountval.Enabled = toggle
        txt_minamount.Enabled = toggle
        txt_maxamount.Enabled = toggle
        txt_expireafter.Enabled = toggle
        drp_discounttype.Enabled = toggle
        rad_type.Enabled = toggle
    End Sub
    Protected Sub Util_PopulateFields(ByVal cCoupon As CouponData, ByVal editable As Boolean)
        Dim sbCurrency As New StringBuilder()
        Dim activeCurrencyList As List(Of CurrencyData) = m_refCurrency.GetActiveCurrencyList()

        ltr_id.Text = cCoupon.Id
        If m_iID > 0 Then IsUsed = CouponManager.IsCouponUsedForOrder(m_iID)
        IsActive = cCoupon.IsActive
        chk_used.Checked = IsUsed
        txt_code.Text = cCoupon.Code
        txt_desc.Text = cCoupon.Description
        chk_active.Checked = cCoupon.IsActive
        chk_oneper.Checked = cCoupon.OnePerCustomer
        If cCoupon.DiscountType = EkEnumeration.CouponDiscountType.FreeShipping Then
            txt_discountval.Text = "0.00"
            txt_discountval.Enabled = False
            drp_discounttype.SelectedIndex = 2
        ElseIf cCoupon.DiscountType = EkEnumeration.CouponDiscountType.Percent Then
            txt_discountval.Text = FormatCurrency(cCoupon.DiscountValue, "")
            drp_discounttype.SelectedIndex = 1
        ElseIf cCoupon.DiscountType = EkEnumeration.CouponDiscountType.Amount Then
            txt_discountval.Text = FormatCurrency(cCoupon.DiscountValue, "")
            drp_discounttype.SelectedIndex = 0
        End If
        Select Case m_sPageAction
            Case "view"
                txt_minamount.Text = FormatCurrency(cCoupon.MinimumAmount, "")
                txt_maxamount.Text = FormatCurrency(cCoupon.MaximumAmount, "")
            Case "addedit"
                txt_minamount.Text = cCoupon.MinimumAmount.ToString("0.00")
                txt_maxamount.Text = cCoupon.MaximumAmount.ToString("0.00")
        End Select

        Select Case m_sPageAction
            Case "view"
                Dim currencydata As CurrencyData = m_refCurrency.GetItem(cCoupon.CurrencyId)
                If cCoupon.DiscountType = EkEnumeration.CouponDiscountType.Amount Then
                    sbCurrency.Append("<select disabled=""disabled"" id=""sel_currency""> ").Append(Environment.NewLine)
                    sbCurrency.Append(" <option value=""id:ektron_Pricing_").Append(currencydata.Id).Append(";label:").Append(currencydata.Name).Append(";symbol:").Append(currencydata.ISOCurrencySymbol).Append(currencydata.CurrencySymbol).Append(" selected=""selected"">").Append(currencydata.AlphaIsoCode).Append("</option> ").Append(Environment.NewLine)
                    sbCurrency.Append("</select> ").Append(Environment.NewLine)
                    ltr_drpCurrency.Text = sbCurrency.ToString()
                End If
            Case "addedit"
                If cCoupon.CurrencyId > 0 Then
                    If cCoupon.DiscountType = EkEnumeration.CouponDiscountType.Amount Then
                        sbCurrency.Append("<select id=""sel_currency"" > ").Append(Environment.NewLine)
                        For i As Integer = 0 To (activeCurrencyList.Count - 1)
                            sbCurrency.Append(" <option value=""id:ektron_Pricing_").Append(activeCurrencyList(i).Id).Append(";label:").Append(activeCurrencyList(i).Name).Append(";symbol:").Append(activeCurrencyList(i).ISOCurrencySymbol).Append(activeCurrencyList(i).CurrencySymbol).Append(""" " & IIf(activeCurrencyList(i).Id = cCoupon.CurrencyId, "selected=""selected""", "") & ">").Append(activeCurrencyList(i).AlphaIsoCode).Append("</option> ").Append(Environment.NewLine)
                        Next
                        sbCurrency.Append("</select> ").Append(Environment.NewLine)
                    Else
                        sbCurrency.Append("<select id=""sel_currency"" disabled=""disabled"" > ").Append(Environment.NewLine)
                        For i As Integer = 0 To (activeCurrencyList.Count - 1)
                            sbCurrency.Append(" <option value=""id:ektron_Pricing_").Append(activeCurrencyList(i).Id).Append(";label:").Append(activeCurrencyList(i).Name).Append(";symbol:").Append(activeCurrencyList(i).ISOCurrencySymbol).Append(activeCurrencyList(i).CurrencySymbol).Append(""" " & IIf(activeCurrencyList(i).Id = cCoupon.CurrencyId, "selected=""selected""", "") & ">").Append(activeCurrencyList(i).AlphaIsoCode).Append("</option> ").Append(Environment.NewLine)
                        Next
                        sbCurrency.Append("</select> ").Append(Environment.NewLine)
                    End If
                Else
                    sbCurrency.Append("<select id=""sel_currency"" > ").Append(Environment.NewLine)
                    For i As Integer = 0 To (activeCurrencyList.Count - 1)
                        sbCurrency.Append(" <option value=""id:ektron_Pricing_").Append(activeCurrencyList(i).Id).Append(";label:").Append(activeCurrencyList(i).Name).Append(";symbol:").Append(activeCurrencyList(i).ISOCurrencySymbol).Append(activeCurrencyList(i).CurrencySymbol).Append(""" " & IIf(activeCurrencyList(i).Id = m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId, "selected=""selected""", "") & ">").Append(activeCurrencyList(i).AlphaIsoCode).Append("</option> ").Append(Environment.NewLine)
                    Next
                    sbCurrency.Append("</select> ").Append(Environment.NewLine)
                End If
                ltr_drpCurrency.Text = sbCurrency.ToString()
        End Select

        txt_expireafter.Text = cCoupon.MaximumUses
        If editable Then
            Util_AssignDates(cCoupon.StartDate, cCoupon.ExpirationDate)
        Else
            ltr_startdatesel.Text = cCoupon.StartDate.ToLongDateString & " " & cCoupon.StartDate.ToLongTimeString()
            If DateTime.Compare(cCoupon.ExpirationDate, DateTime.MaxValue) = 0 Then ltr_enddatesel.Text = "-" Else ltr_enddatesel.Text = cCoupon.ExpirationDate.ToLongDateString & " " & cCoupon.ExpirationDate.ToLongTimeString()
        End If
        rad_type.SelectedValue = cCoupon.CouponType.GetHashCode()
    End Sub
    Protected Sub Util_AssignDates(ByVal startdate As DateTime, ByVal enddate As DateTime)
        Dim dateSchedule As EkDTSelector

        dateSchedule = Me.m_refContentApi.EkDTSelectorRef
        dateSchedule.formName = "form1"
        dateSchedule.extendedMeta = True
        dateSchedule.formElement = "go_live"
        dateSchedule.spanId = "go_live_span"
        dateSchedule.targetDate = startdate
        ltr_startdatesel.Text = (dateSchedule.displayCultureDateTime(True))
        dateSchedule.formElement = "end_date"
        dateSchedule.spanId = "end_date_span"
        If enddate.Year = DateTime.MaxValue.Year Then
            dateSchedule.targetDate = Nothing
        Else
            dateSchedule.targetDate = enddate
        End If
        ltr_enddatesel.Text = (dateSchedule.displayCultureDateTime(True))
    End Sub
    Protected Sub Util_SetDropEnabled()

        Dim sbJS As New StringBuilder()

        sbJS.Append("<script language=""javascript"">").Append(Environment.NewLine)
        sbJS.Append("   document.getElementById('txt_discountval').disabled = (document.getElementById('").Append(drp_discounttype.ID).Append("').selectedIndex == 2);").Append(Environment.NewLine)
        sbJS.Append("</script>").Append(Environment.NewLine)

        ltr_endJS.Text = sbJS.ToString()

    End Sub
    Protected Sub Util_SetJS()
        Dim sbJS As New StringBuilder()
        sbJS.Append("    <script language=""JavaScript"" type=""text/javascript"" src=""../java/internCalendarDisplayFuncs.js""></script>").Append(Environment.NewLine)
        sbJS.Append("<script language=""javascript"">").Append(Environment.NewLine)

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine)

        sbJS.Append(" function SubmitForm() { ").Append(Environment.NewLine)
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine)
        sbJS.Append("   validate_Amount();").Append(Environment.NewLine)
        sbJS.Append("   getCurrency();").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("('document.forms[0].submit();', null,null);").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append("   function AddItem() { ").Append(Environment.NewLine)
        sbJS.Append("       ektb_show('','byproduct.aspx?action=coupon&EkTB_iframe=true&height=300&width=500&modal=true', null); ").Append(Environment.NewLine)
        sbJS.Append("   } ").Append(Environment.NewLine)
        sbJS.Append("   function DeleteItem() {").Append(Environment.NewLine)
        sbJS.Append("       var iAttr = getCheckedInt(false);").Append(Environment.NewLine)
        sbJS.Append("       if (iAttr == -1) {").Append(Environment.NewLine)
        sbJS.Append("           alert('").Append(GetMessage("js please sel coupon")).Append("');").Append(Environment.NewLine)
        sbJS.Append("       } else {").Append(Environment.NewLine)
        sbJS.Append("           deleteChecked();").Append(Environment.NewLine)
        sbJS.Append("       }").Append(Environment.NewLine)
        sbJS.Append("   }").Append(Environment.NewLine)

        sbJS.Append(" function validate_Title() { ").Append(Environment.NewLine)
        sbJS.Append("   var sTitle = Trim(document.getElementById('").Append(txt_code.ID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   if (sTitle == '') { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err entry code req")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar('").Append(txt_code.ID).Append("',""").Append(GetMessage("lbl entry disallowed chars")).Append("""); ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append(" function validate_Amount() {").Append(Environment.NewLine)
        sbJS.Append("   var sMinAmount = Trim(document.getElementById('").Append(txt_minamount.ID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var sMaxAmount = Trim(document.getElementById('").Append(txt_maxamount.ID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var sDiscountVal = Trim(document.getElementById('").Append(txt_discountval.ID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var sMaxUses = Trim(document.getElementById('").Append(txt_expireafter.ID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var iDiscountType = Trim(document.getElementById('").Append(drp_discounttype.ID).Append("').selectedIndex); ").Append(Environment.NewLine)
        sbJS.Append("   var startDate = $ektron('input#go_live_iso')[0].value;").Append(Environment.NewLine)
        sbJS.Append("   var endDate = $ektron('input#end_date_iso')[0].value; ").Append(Environment.NewLine)
        sbJS.Append("   if ((endDate < startDate && endDate != '[None]') && (endDate < startDate && endDate != '') )").Append(Environment.NewLine)
        sbJS.Append("   { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err start end date")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   if(sMinAmount ==  '') { sMinAmount = 0; document.getElementById('").Append(txt_minamount.ID).Append("').value = 0;} ").Append(Environment.NewLine)
        sbJS.Append("   if(sMaxAmount ==  '') { sMaxAmount = 0; document.getElementById('").Append(txt_maxamount.ID).Append("').value = 0;} ").Append(Environment.NewLine)
        sbJS.Append("   if(sMaxUses ==  '') { sMaxAmount = 0; document.getElementById('").Append(txt_expireafter.ID).Append("').value = 0;} ").Append(Environment.NewLine)
        sbJS.Append("   if(isInteger(sMaxUses) ==  false || sMaxUses >= 2147483648) { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err number user not integer")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   if( iDiscountType != 2 && (isNaN(sDiscountVal.replace(/,/g,'')) || sDiscountVal == '') ) ").Append(Environment.NewLine)
        sbJS.Append("   { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err disc amount not numeric")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   if(isNaN(sMaxUses) || sMaxUses == '' ) ").Append(Environment.NewLine)
        sbJS.Append("   { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err n uses amount not numeric")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   if(sMinAmount < 0 || isNaN(sMinAmount)) ").Append(Environment.NewLine)
        sbJS.Append("   { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err min amount not numeric")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   if(sMaxAmount < 0 || isNaN(sMaxAmount)) ").Append(Environment.NewLine)
        sbJS.Append("   { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err max amount not numeric")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar('").Append(txt_minamount.ID).Append("',""").Append(GetMessage("lbl entry disallowed chars")).Append("""); ").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar('").Append(txt_maxamount.ID).Append("',""").Append(GetMessage("lbl entry disallowed chars")).Append("""); ").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar('").Append(txt_discountval.ID).Append("',""").Append(GetMessage("lbl entry disallowed chars")).Append("""); ").Append(Environment.NewLine)
        sbJS.Append("}").Append(Environment.NewLine)

        sbJS.Append("   function isInteger(s)").Append(Environment.NewLine)
        sbJS.Append("   {").Append(Environment.NewLine)
        sbJS.Append("       return s.length > 0 && !(/[^0-9]/).test(s);").Append(Environment.NewLine)
        sbJS.Append("   }").Append(Environment.NewLine)

        sbJS.Append(JSLibrary.CheckKeyValue())
        sbJS.Append(JSLibrary.AddError("aSubmitErr"))
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"))
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"))
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection))

        sbJS.Append("</script>").Append(Environment.NewLine)
        ltr_js.Text &= sbJS.ToString()
    End Sub
    Protected Sub Util_CheckAccess()
        If Not m_refContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin) Then Throw New Exception(GetMessage("err not role commerce-admin"))
    End Sub
    Protected Sub Util_BuildAppliedTable(ByVal AppliedList As List(Of ProductCouponEntryData))
        Dim sbApplies As New StringBuilder()
        sbApplies.Append("                        <table border=""0"" cellspacing=""0"" id=""tblApplies"" class=""ektableutil"">").Append(Environment.NewLine)
        sbApplies.Append("                        <thead>").Append(Environment.NewLine)
        sbApplies.Append("                          <tr class=""item_header""><th></th><th></th><th>").Append(GetMessage("generic id")).Append("</th><th>").Append(GetMessage("generic language")).Append("</th><th>").Append(GetMessage("generic name")).Append("</th><th>&#160;</th><th>&#160;</th></tr>").Append(Environment.NewLine)
        sbApplies.Append("                        </thead>").Append(Environment.NewLine)
        sbApplies.Append("                        <tbody>").Append(Environment.NewLine)
        For i As Integer = 0 To (AppliedList.Count - 1)
            sbApplies.Append("<tr")
            If i Mod 2 > 0 Then sbApplies.Append(" class=""itemrow0""")
            sbApplies.Append(">").Append(Environment.NewLine)
            sbApplies.Append("<td>").Append((i + 1)).Append("</td>").Append(Environment.NewLine)
            sbApplies.Append("<td>").Append(Util_GetEntryImage(AppliedList(i))).Append("</td>").Append(Environment.NewLine)
            sbApplies.Append("<td>").Append(AppliedList(i).ObjectId).Append("</td>").Append(Environment.NewLine)
            sbApplies.Append("<td>").Append("1033").Append("</td>").Append(Environment.NewLine) 'hardcoding for now, no language in the future?
            sbApplies.Append("<td>").Append(AppliedList(i).Title).Append("</td>").Append(Environment.NewLine)
            sbApplies.Append("<td>")
            sbApplies.Append("<input name=""txtitemid").Append((i + 1)).Append(""" type=""hidden"" id=""txtitemid").Append((i + 1)).Append(""" value=""").Append(AppliedList(i).ObjectId).Append(""" />")
            sbApplies.Append("<input name=""txtitemlang").Append((i + 1)).Append(""" type=""hidden"" id=""txtitemlang").Append((i + 1)).Append(""" value=""").Append("1033").Append(""" />") 'hardcoding - removing language????
            sbApplies.Append("<input name=""txtitemposidx").Append((i + 1)).Append(""" type=""hidden"" id=""txtitemposidx").Append((i + 1)).Append(""" value=""").Append((i + 1)).Append(""" />")
            sbApplies.Append("</td>").Append(Environment.NewLine)
            sbApplies.Append("<td><input type=""radio"" value=""").Append((i + 1)).Append(""" name=""radInput"" /></td>").Append(Environment.NewLine)
            sbApplies.Append("</tr>").Append(Environment.NewLine)
        Next
        sbApplies.Append("                        </tbody>").Append(Environment.NewLine)
        sbApplies.Append("                        </table>").Append(Environment.NewLine)
        ltr_appliesto.Text = sbApplies.ToString()
    End Sub
    Protected Function Util_GetEntryImage(ByVal couponitem As ProductCouponEntryData) As String
        Dim sImage As String = m_refContentApi.AppPath & "images/ui/icons/brick.png"
        Select Case couponitem.EntryType.GetHashCode()
            Case 1
                sImage = AppImgPath & "images/ui/icons/bricks.png"
            Case 2
                sImage = AppImgPath & "images/ui/icons/box.png"
            Case 3
                sImage = AppImgPath & "images/ui/icons/package.png"
        End Select
        Return "<img src=""" & sImage & """ />"
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
        Display_View_All()
        isPostData.Value = "true"
    End Sub
    Private Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "csslib/tables/tableutil.css", "EktronTableUtilCSS")

        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "java/dhtml/coupontableutil.js", "EktronCouponTableUtilJS")
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronThickBoxJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "java/commerce/com.Ektron.Commerce.Pricing.js", "EktronCommercePricingJS")

        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "csslib/box.css", "EktronBoxCSS")

    End Sub
#End Region

End Class
