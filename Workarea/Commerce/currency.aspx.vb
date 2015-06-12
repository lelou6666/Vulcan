Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Workarea

Partial Class Commerce_currency
    Inherits workareabase

#Region "Member Variables"

    Protected m_refCurrency As New Currency(m_refContentApi.RequestInformationRef)
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1
    Protected sortCriteria As CurrencyProperty = CurrencyProperty.Name
    Protected Const PAGE_NAME As String = "currency.aspx"
    Protected searchCriteria As String = ""
    Protected exchangeRateList As New System.Collections.Generic.List(Of ExchangeRateData)
    Protected defaultCurrency As CurrencyData = m_refCurrency.GetDefaultCurrency()
    Protected activeCurrencies As System.Collections.Generic.List(Of CurrencyData) = Nothing
    Protected AppPath As String = ""

#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        RegisterResource()
        AppPath = Me.m_refContentApi.ApplicationPath
        If Page.Request.QueryString("search") <> "" Then searchCriteria = Page.Request.QueryString("search")
        Try
            If Request.QueryString("currentpage") <> "" Then _currentPageNumber = Request.QueryString("currentpage")
            If Request.QueryString("sortcriteria") <> "" Then Util_FindSort(Request.QueryString("sortcriteria"))
            Utilities.ValidateUserLogin()
            Util_CheckAccess()
            Util_RegisterResources()
            Util_SetServerJSVariables()
            hdnCurrentPage.Value = CurrentPage.Text
            tr_addedit.Visible = False
            tr_viewall.Visible = False
            If (Not (Page.IsPostBack)) Then
                Select Case m_sPageAction
                    Case "exchangerate"
                        Display_ExchangeRate()
                    Case "goto"
                        Display_GoTo()
                    Case "edit"
                        Display_Edit()
                    Case "add"
                        Display_Add()
                    Case "delete"
                        Process_Delete()
                    Case Else
                        If Page.IsPostBack = False Then
                            Display_ViewAll()
                        End If
                End Select
            Else
                Select Case m_sPageAction
                    Case "exchangerate"
                        Process_ExchangeRate()
                    Case "edit"
                        Process_Edit()
                    Case "add"
                        Process_Add()
                    Case "delete"
                        Process_Delete()
                End Select
            End If
        Catch ex As Exception
            Utilities.ShowError(Server.UrlEncode(ex.Message))
        End Try
    End Sub

#End Region

#Region "Process"

    Private Sub Process_ExchangeRate()

        Dim exchangeRateApi As New ExchangeRateApi()

        For i As Integer = 0 To (dg_xc.Items.Count - 1)

            Dim chkUpdate As CheckBox = dg_xc.Items(i).FindControl("chk_email")
            Dim hdnCurrency As HiddenField = dg_xc.Items(i).FindControl("hdn_currencyId")
            Dim currentCurrencyId As Long = hdnCurrency.Value

            If chkUpdate.Checked AndAlso Util_IsActiveExchangeCurrency(currentCurrencyId) Then

                ' If Request.Form("chk_email_" & currencyList(i).Id) <> "" Then

                If (dg_xc.Items(i).FindControl("txt_exchange") IsNot Nothing) Then

                    Dim txtXCRate As TextBox = dg_xc.Items(i).FindControl("txt_exchange")
                    Dim newRate As Decimal = txtXCRate.Text
                    Dim exchangeRateData As New ExchangeRateData(exchangeRateApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId, currentCurrencyId, newRate, Now())

                    exchangeRateApi.Add(exchangeRateData)

                End If

            End If

        Next

        ltr_js.Text = "self.parent.location.reload(); self.parent.ektb_remove();"

    End Sub

    Private Sub Process_Edit()
        Dim currency As CurrencyData = Nothing
        currency = m_refCurrency.GetItem(m_iID)

        Dim exchangeRateApi As New ExchangeRateApi()
        Dim exchangeRateData As New ExchangeRateData(exchangeRateApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId, currency.Id, txt_exchangerate.Text, Now())

        exchangeRateApi.Add(exchangeRateData)

        currency.Name = txt_name.Text
        currency.Id = CInt(txt_numericisocode.Text)
        currency.AlphaIsoCode = txt_alphaisocode.Text
        currency.Enabled = chk_enabled.Checked

        m_refCurrency.Update(currency)
        ltr_js.Text = "self.parent.location.reload(); self.parent.ektb_remove();"
    End Sub
    Private Sub Process_Add()
        Dim currency As New CurrencyData
        Dim exchangeRateApi As New ExchangeRateApi()

        currency.Name = txt_name.Text
        currency.AlphaIsoCode = txt_alphaisocode.Text
        currency.Enabled = chk_enabled.Checked
        currency.Id = CInt(txt_numericisocode.Text)
        currency.CultureCode = txt_alphaisocode.Text

        m_refCurrency.Add(currency)

        Dim exchangeRateData As New ExchangeRateData(exchangeRateApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId, txt_numericisocode.Text, txt_exchangerate.Text, Now())
        exchangeRateApi.Add(exchangeRateData)

        ltr_js.Text = "self.parent.location.reload(); self.parent.ektb_remove();"
    End Sub
    Private Sub Process_Delete()
        Dim idList() As String = Split(Request.QueryString("Ids"), ",")
        If idList.Length > 0 Then
            For i As Integer = 0 To (idList.Length - 1)
                If IsNumeric(idList(i)) Then m_refCurrency.Delete(idList(i))
            Next
            Response.Redirect(PAGE_NAME & "?action=viewall", False)
        Else
            Throw New Exception(GetMessage("lbl err no currencies selected"))
        End If
    End Sub

#End Region

#Region "Display"

    Private Sub Display_ExchangeRate()

        Dim criteria As New Ektron.Cms.Common.Criteria(Of CurrencyProperty)(sortCriteria, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim currencyList As System.Collections.Generic.List(Of CurrencyData)

        criteria.PagingInfo = New PagingInfo(1000)
        criteria.PagingInfo.CurrentPage = _currentPageNumber
        criteria.AddFilter(CurrencyProperty.Enabled, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, True)
        criteria.AddFilter(CurrencyProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, m_refCurrency.RequestInformation.CommerceSettings.DefaultCurrencyId)

        currencyList = m_refCurrency.GetList(criteria)

        Dim exchangeRateApi As New ExchangeRateApi()
        Dim exchangeRateCriteria As New Ektron.Cms.Common.Criteria(Of Commerce.ExchangeRateProperty)
        Dim currencyIDList As New Collections.Generic.List(Of Long)
        For i As Integer = 0 To (currencyList.Count - 1)
            currencyIDList.Add(currencyList(i).Id)
        Next
        exchangeRateCriteria.AddFilter(ExchangeRateProperty.BaseCurrencyId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId)
        exchangeRateCriteria.AddFilter(ExchangeRateProperty.ExchangeCurrencyId, Ektron.Cms.Common.CriteriaFilterOperator.In, currencyIDList.ToArray())
        exchangeRateList = exchangeRateApi.GetCurrentList(exchangeRateCriteria)

        dg_xc.DataSource = currencyList
        dg_xc.DataBind()

        Util_SetJs()
        paginglinks.Visible = False
        Util_SetLabels()

    End Sub

    Private Sub Display_Edit()
        Dim currency As New CurrencyData

        currency = m_refCurrency.GetItem(m_iID)
        ltr_ISOAlpha.Text = "&nbsp;" & currency.AlphaIsoCode
        txt_numericisocode.Enabled = False
        txt_alphaisocode.Enabled = False
        Util_PopulateData(currency)
        Util_SetLabels()
    End Sub

    Private Sub Display_GoTo()
        Util_SetLabels()
    End Sub

    Private Sub Display_Add()
        Util_SetLabels()
    End Sub

    Private Sub Display_View()
        Dim currency As New CurrencyData

        currency = m_refCurrency.GetItem(m_iID)
        ltr_ISOAlpha.Text = "&nbsp;<b>" & currency.AlphaIsoCode & "</b>"

        Util_PopulateData(currency)
        Util_SetLabels()
    End Sub
    Private Sub Display_ViewAll()
        Dim criteria As New Ektron.Cms.Common.Criteria(Of CurrencyProperty)(sortCriteria, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        If sortCriteria = CurrencyProperty.Enabled Then criteria = New Ektron.Cms.Common.Criteria(Of CurrencyProperty)(sortCriteria, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Descending)
        Dim currencyList As System.Collections.Generic.List(Of CurrencyData)

        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        criteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        If searchCriteria <> "" Then criteria.AddFilter(CurrencyProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, searchCriteria)

        currencyList = m_refCurrency.GetList(criteria)

        ViewSubscriptionGrid.Columns(ViewSubscriptionGrid.Columns.Count - 1).Visible = False
        ViewSubscriptionGrid.DataSource = currencyList
        ViewSubscriptionGrid.DataBind()

        TotalPagesNumber = criteria.PagingInfo.TotalPages

        Util_SetJs()
        Util_SetPaging()
        Util_SetLabels()
    End Sub

#End Region

#Region "Util"

    Private Sub Util_SetLabels()
        ltr_name.Text = GetMessage("generic name")
        ltr_numericisocode.Text = GetMessage("lbl numeric iso code")
        ltr_alphaisocode.Text = GetMessage("lbl alpha iso code")
        ltr_enabled.Text = GetMessage("enabled")
        ltr_exchangerate.Text = GetMessage("lbl exchange rate")

        lnk_gotopage.Text = "[" & GetMessage("lbl go to page") & "]"

        lnk_first.Text = "[" & GetMessage("lbl first page") & "]"
        lnk_previous.Text = "[" & GetMessage("lbl previous page") & "]"
        lnk_next.Text = "[" & GetMessage("lbl next page") & "]"
        lnk_last.Text = "[" & GetMessage("lbl last page") & "]"

        ltr_defaultcurrency.Text = m_refContentApi.RequestInformationRef.CommerceSettings.ISOCurrencySymbol

        Select Case m_sPageAction
            Case "exchangerate"

                tr_viewall.Visible = False
                tr_exchangerate.Visible = True

                SetTitleBarToMessage("lbl edit exchange rates")

                Dim actionMenu As New workareamenu("action", GetMessage("lbl action"), AppPath & "images/UI/Icons/check.png")
                actionMenu.AddItem(AppPath & "images/ui/icons/save.png", GetMessage("btn update"), "SubmitForm(true);")
                AddMenu(actionMenu)

                AddButtonwithMessages(AppPath & "images/UI/Icons/back.png", PAGE_NAME & "?action=ViewAll", "alt back button text", "btn back", " onclick=""self.parent.ektb_remove();"" ")
                AddHelpButton("EditExchangeRates")
            Case "goto"
                ' tr_goto.Visible = True
            Case "add"
                tr_addedit.Visible = True

                SetTitleBarToMessage("lbl add currency")
                AddButtonwithMessages(AppPath & "images/UI/Icons/save.png", "#", "lbl Add Email From Address", "btn save", "onclick=""return SubmitForm( 'VerifyForm()');""")
                AddButtonwithMessages(AppPath & "images/UI/Icons/back.png", PAGE_NAME & "?action=viewall", "alt back button text", "btn back", " onclick=""self.parent.ektb_remove();"" ")
                AddHelpButton(m_sPageAction & "currency")
            Case "edit"
                tr_addedit.Visible = True

                SetTitleBarToMessage("lbl edit currency")

                AddButtonwithMessages(AppPath & "images/UI/Icons/save.png", "#", "lbl update email address", "btn update", "Onclick=""javascript:return SubmitForm('VerifyForm()');""")
                AddButtonwithMessages(AppPath & "images/UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=""javascript:self.parent.ektb_remove();"" ")
                AddHelpButton(m_sPageAction & "currency")
            Case Else
                tr_viewall.Visible = True


                SetTitleBarToMessage("lbl currencies")

                'Dim newMenu As New workareamenu("file", GetMessage("lbl new"), apppath & "images/UI/Icons/star.png")
                'newMenu.AddItem(AppImgPath & "commerce/currency.gif", GetMessage("lbl currency"), "ektb_show('','" & PAGE_NAME & "?action=Add&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true', null);")
                'AddMenu(newMenu)

                Dim actionMenu As New workareamenu("action", GetMessage("lbl action"), AppPath & "images/UI/Icons/check.png")
                actionMenu.AddItem(AppPath & "images/ui/icons/pencil.png", GetMessage("lbl edit exchange rates"), "ektb_show('','" & PAGE_NAME & "?action=ExchangeRate&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true', null);")
                actionMenu.AddBreak()
                actionMenu.AddItem(AppPath & "images/ui/icons/delete.png", GetMessage("lbl del sel"), "ConfirmDelete();")
                AddMenu(actionMenu)
                Me.AddSearchBox(Server.HtmlEncode(searchCriteria), New ListItemCollection(), "searchCurrency")
                AddHelpButton(m_sPageAction & "currency")
        End Select
    End Sub
    Protected Sub Util_CheckAccess()
        If Not m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin) Then
            Throw New Exception("err not role commerce-admin")
        End If
    End Sub
    Protected Sub Util_NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        If hdnCurrentPage.Value <> "" Then
            _currentPageNumber = Int32.Parse(hdnCurrentPage.Value)
        End If
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
    Protected Sub Util_FindSort(ByVal sortstring As String)
        Select Case sortstring.ToLower()
            Case "alphaisocode"
                sortCriteria = CurrencyProperty.AlphaIsoCode
            Case "id"
                sortCriteria = CurrencyProperty.Id
            Case "enabled"
                sortCriteria = CurrencyProperty.Enabled
            Case Else
                sortCriteria = CurrencyProperty.Name
        End Select
    End Sub
    Protected Sub Util_PopulateData(ByVal currency As CurrencyData)
        Dim exchangeRateData As New ExchangeRateData
        Dim rate As Decimal = 0.0
        Dim exchangeRateApi As New ExchangeRateApi()
        exchangeRateData = exchangeRateApi.GetCurrentExchangeRate(currency.Id)

        If exchangeRateData IsNot Nothing Then
            rate = exchangeRateData.Rate
        End If
        txt_name.Text = currency.Name
        txt_numericisocode.Text = currency.Id.ToString()
        txt_alphaisocode.Text = currency.AlphaIsoCode
        chk_enabled.Checked = currency.Enabled

        txt_exchangerate.Text = rate
        'txt_exchangerate.Text = txt_exchangerate.Text.Substring(0, txt_exchangerate.Text.LastIndexOf(".") + 3)
    End Sub
    Protected Sub Util_SetPaging()
        If (TotalPagesNumber <= 1) Then
            paginglinks.Visible = False
        Else
            paginglinks.Visible = True

            TotalPages.Text = (System.Math.Ceiling(TotalPagesNumber)).ToString()
            CurrentPage.Text = _currentPageNumber.ToString()

            lnk_gotopage.NavigateUrl = "javascript:GoToPage(document.getElementById('CurrentPage').value, " & TotalPagesNumber & ");"
            lnk_previous.NavigateUrl = Util_GetPageURL(_currentPageNumber - 1)
            lnk_first.NavigateUrl = Util_GetPageURL(1)
            lnk_next.NavigateUrl = Util_GetPageURL(_currentPageNumber + 1)
            lnk_last.NavigateUrl = Util_GetPageURL(TotalPagesNumber)

            If _currentPageNumber = 1 Then
                lnk_previous.Enabled = False
                lnk_first.Enabled = False
            ElseIf _currentPageNumber = TotalPagesNumber Then
                lnk_next.Enabled = False
                lnk_last.Enabled = False
            End If
        End If
    End Sub
    Protected Function Util_GetPageURL(ByVal pageid As Integer) As String
        Return PAGE_NAME & "?currentpage=" & IIf(pageid = -1, "' + pageid + '", pageid) & IIf(Not sortCriteria = CurrencyProperty.Name, "&sortcriteria=" & System.Enum.GetName(GetType(CurrencyProperty), sortCriteria), "")
    End Function
    Protected Sub Util_SetJs()
        Dim sbJS As New StringBuilder()

        sbJS.Append(" function GoToPage(pageid, pagetotal) { ").Append(Environment.NewLine)
        sbJS.Append("     if (pageid <= pagetotal && pageid >= 1) { ").Append(Environment.NewLine)
        sbJS.Append("         window.location.href = '").Append(Util_GetPageURL(-1)).Append("'; ").Append(Environment.NewLine)
        sbJS.Append("     } else { ").Append(Environment.NewLine)
        sbJS.Append("         alert('").Append(String.Format(GetMessage("js: err page must be between"), TotalPagesNumber)).Append("'); ").Append(Environment.NewLine)
        sbJS.Append("     } ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        ltr_js.Text = sbJS.ToString()
    End Sub

    Protected Function Util_GetExchangeRate(ByVal currencyId As Long) As Decimal

        Dim xcRate As Decimal = 0

        For Each xChangeRate As ExchangeRateData In exchangeRateList
            If xChangeRate.ExchangeCurrencyId = currencyId Then
                xcRate = xChangeRate.Rate
                Exit For
            End If
        Next

        Return xcRate

    End Function

    Private Sub Util_RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronThickBoxJS)

        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "csslib/box.css", "EktronBoxCSS")
    End Sub
    Private Sub Util_SetServerJSVariables()
        ltr_nameReq.Text = GetMessage("js: alert name required")
        ltr_nameCantHave.Text = GetMessage("js: alert currency name cant include")
        ltr_rateNotNumeric.Text = GetMessage("js: alert exchange rate not numeric")
        ltr_rateGrtZero.Text = GetMessage("js: alert to enable exchange rate must be greater than zero")
        ltr_notInteger.Text = GetMessage("js: alert numeric isocode not integer")
        ltr_delSelCur.Text = GetMessage("js: confirm delete selected currency")
        ltr_errNoCurSel.Text = GetMessage("lbl err no currencies selected")
    End Sub

    Protected Function Util_IsActiveExchangeCurrency(ByVal currencyId As Long) As Boolean

        If activeCurrencies Is Nothing Then activeCurrencies = m_refCurrency.GetActiveCurrencyList()

        For i As Integer = 0 To (activeCurrencies.Count - 1)

            If activeCurrencies(i).Id = currencyId AndAlso Not (currencyId = Me.m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId) Then Return True

        Next

        Return False

    End Function

#End Region

#Region "JS/CSS"

    Protected Sub RegisterResource()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub

#End Region

End Class
