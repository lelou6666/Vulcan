Imports Ektron.Cms.Workarea
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports System.Collections.Generic

Partial Class Commerce_paymentgateway
    Inherits workareabase

#Region "Member Variables"

    Protected m_sPageName As String = "paymentgateway.aspx"
    Protected m_bIsDefault As Boolean = False
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1
    Protected appSettings As New SettingsData()
    Protected supportsCards As New List(Of Boolean)
    Protected supportsChecks As New List(Of Boolean)
    Protected imageIconsPath As String = ""

#End Region

#Region "Events"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        'register page components
        Me.RegisterJS()
        Me.RegisterCSS()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If

        imageIconsPath = Me.m_refContentApi.AppPath & "images/ui/icons/"
        Try
            Utilities.ValidateUserLogin()
            CommerceLibrary.CheckCommerceAdminAccess()


            Select Case MyBase.m_sPageAction
                Case "markdef"
                    Process_MarkDefault()
                Case "del"
                    Process_Delete()
                Case "addedit"
                    If Page.IsPostBack() Then
                        Process_AddEdit()
                    Else
                        Display_AddEdit()
                    End If
                Case "editoptions"

                    Util_SetPaymentCheckBoxes()
                    If Page.IsPostBack Then Process_EditOptions() Else Display_EditOptions()
                    Me.phGatewaysContent.Visible = False
                    Me.phGatewaysTab.Visible = False

                Case "view"
                    Display_View()
                Case Else ' "viewall"
                    If Page.IsPostBack = False Then

                        Util_SetPaymentCheckBoxes()
                        Display_View_All()

                    End If
            End Select
            SetLabels()
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub


#End Region

#Region "Display"


    Protected Sub Display_EditOptions()

        chk_paypal.Enabled = True
        'chk_google.Enabled = True

        dg_gateway.Visible = False
        litPaymentGatways.Visible = False

        Util_HidePagingLinks()

    End Sub

    Protected Sub Display_View()

        Dim gatewayService As IPaymentGateway = ObjectFactory.GetPaymentGateway()
        Dim paymentGateway As PaymentGatewayData = Nothing
        Dim cmsPaymentManager As New PaymentManager()
        Dim paymentProviders As System.Collections.IEnumerator = cmsPaymentManager.Providers.GetEnumerator()
        Dim gatewayIndex As Integer = 0

        paymentGateway = gatewayService.GetItem(Me.m_iID)

        While paymentProviders.MoveNext()

            drp_GatewayName.Items.Add(paymentProviders.Current.Name)
            If paymentGateway.Name = paymentProviders.Current.Name Then drp_GatewayName.SelectedIndex = gatewayIndex
            gatewayIndex = gatewayIndex + 1

        End While

        lbl_id.Text = paymentGateway.Id
        chk_default.Checked = paymentGateway.IsDefault
        txt_uid.Text = paymentGateway.UserId
        txt_viewpwd.Text = ProtectPassword(paymentGateway.Password)
        txt_pwd.Visible = False
        txt_spare1.Text = paymentGateway.CustomFieldOne
        txt_spare2.Text = paymentGateway.CustomFieldTwo
        chk_cc.Checked = paymentGateway.AllowsCreditCardPayments
        chk_check.Checked = paymentGateway.AllowsCheckPayments

        drp_GatewayName.Enabled = False
        chk_default.Enabled = False
        txt_uid.Enabled = False
        txt_pwd.Enabled = False
        txt_spare1.Enabled = False
        txt_spare2.Enabled = False
        chk_cc.Enabled = False
        chk_check.Enabled = False

        m_bIsDefault = paymentGateway.IsDefault

    End Sub

    Protected Sub Display_AddEdit()

        Dim gatewayService As IPaymentGateway = ObjectFactory.GetPaymentGateway()
        Dim paymentGateway As PaymentGatewayData = Nothing
        Dim cmsPaymentManager As New PaymentManager()
        Dim paymentProviders As System.Collections.IEnumerator = cmsPaymentManager.Providers.GetEnumerator()
        Dim gatewayIndex As Integer = 0
        Dim defaultName As String = ""

        paymentGateway = IIf(m_iID > 0, gatewayService.GetItem(Me.m_iID), New PaymentGatewayData())

        While paymentProviders.MoveNext()

            Dim currentGatewayName As String = ""

            If paymentProviders.Current IsNot Nothing Then

                If paymentGateway.Name = paymentProviders.Current.Name Then chk_check.Enabled = cmsPaymentManager.Providers(paymentProviders.Current.Name).SupportsCheckPayments
                If gatewayIndex = 0 Then defaultName = paymentProviders.Current.Name

                If (paymentProviders.Current.Name.ToString().ToLower() <> "google" And _
                paymentProviders.Current.Name.ToString().ToLower() <> "paypal") Then

                    supportsCards.Add(cmsPaymentManager.Providers(paymentProviders.Current.Name).SupportsCreditCardPayments)
                    supportsChecks.Add(cmsPaymentManager.Providers(paymentProviders.Current.Name).SupportsCheckPayments)

                    currentGatewayName = paymentProviders.Current.Name
                    drp_GatewayName.Items.Add(currentGatewayName)
                    If paymentGateway.Name = currentGatewayName Then drp_GatewayName.SelectedIndex = gatewayIndex
                    gatewayIndex = gatewayIndex + 1

                End If

            End If

        End While

        drp_GatewayName.Attributes.Add("onchange", "UpdateOptions(this);")
        If paymentGateway.Id = 0 AndAlso drp_GatewayName.Items.Count > 0 Then

            If Not cmsPaymentManager.Providers(defaultName).SupportsCreditCardPayments Then chk_cc.Enabled = False
            If Not cmsPaymentManager.Providers(defaultName).SupportsCheckPayments Then chk_check.Enabled = False
            drp_GatewayName.SelectedIndex = 0

        End If

        ' txt_name.Enabled = ((m_iID = 0) Or (m_iID > 0 And paymentGateway.IsCustom))
        lbl_id.Text = paymentGateway.Id
        chk_default.Checked = paymentGateway.IsDefault
        txt_uid.Text = paymentGateway.UserId
        txt_viewpwd.Visible = False
        txt_pwd.Text = ProtectPassword(paymentGateway.Password)
        txt_spare1.Text = paymentGateway.CustomFieldOne
        txt_spare2.Text = paymentGateway.CustomFieldTwo
        chk_cc.Checked = paymentGateway.AllowsCreditCardPayments
        chk_check.Checked = paymentGateway.AllowsCheckPayments

        tr_id.Visible = (m_iID > 0)
        chk_default.Enabled = (m_iID = 0)

    End Sub

    Protected Sub Display_View_All()
        Dim gatewayService As IPaymentGateway = ObjectFactory.GetPaymentGateway()
        Dim gatewayList As System.Collections.Generic.List(Of PaymentGatewayData)
        Dim paymentCriteria As New Ektron.Cms.Common.Criteria(Of PaymentGatewayProperty)

        paymentCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()
        paymentCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize

        gatewayList = gatewayService.GetList(paymentCriteria)
        TotalPagesNumber = paymentCriteria.PagingInfo.TotalPages

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
        dg_gateway.DataSource = gatewayList
        dg_gateway.Columns(6).HeaderText = GetMessage("lbl commerce payment option cc")
        dg_gateway.Columns(7).HeaderText = GetMessage("lbl commerce payment option check")
        dg_gateway.DataBind()

    End Sub


#End Region

#Region "Process"


    Protected Sub Process_EditOptions()

        Dim m_refSiteApi As New SiteAPI
        Dim paymentOptions As New PaymentSettingsData()

        paymentOptions.PayPal = chk_paypal.Checked
        'paymentOptions.GoogleCheckout = chk_google.Checked

        m_refSiteApi.UpdatePaymentOptions(paymentOptions)

        Response.Redirect(Me.m_sPageName & IIf(m_iID > 0, "?action=view&id=" & Me.m_iID, ""), False)

    End Sub

    Protected Sub Process_MarkDefault()
        Dim gatewayService As IPaymentGateway = ObjectFactory.GetPaymentGateway()
        gatewayService.MarkAsDefault(Me.m_iID)
        Response.Redirect(Me.m_sPageName, False)
    End Sub

    Protected Sub Process_Delete()
        Dim gatewayService As IPaymentGateway = ObjectFactory.GetPaymentGateway()
        gatewayService.Delete(Me.m_iID)
        Response.Redirect(Me.m_sPageName, False)
    End Sub

    Protected Sub Process_AddEdit()
        Dim gatewayService As IPaymentGateway = ObjectFactory.GetPaymentGateway()
        Dim paymentGateway As PaymentGatewayData = Nothing
        paymentGateway = IIf(m_iID > 0, gatewayService.GetItem(Me.m_iID), New PaymentGatewayData())
        paymentGateway.Name = Me.drp_GatewayName.SelectedValue
        paymentGateway.IsDefault = Me.chk_default.Checked
        paymentGateway.UserId = Me.txt_uid.Text
        If Me.m_iID > 0 AndAlso Me.txt_pwd.Text.Trim().Length() = 0 Then
            ' no change to password
            ' paymentGateway.Password = paymentGateway.Password
        Else
            paymentGateway.Password = Me.txt_pwd.Text
        End If
        paymentGateway.CustomFieldOne = txt_spare1.Text
        paymentGateway.CustomFieldTwo = txt_spare2.Text
        paymentGateway.AllowsCreditCardPayments = chk_cc.Checked
        paymentGateway.AllowsCheckPayments = chk_check.Checked

        If (paymentGateway.Id > 0) Then
            gatewayService.Update(paymentGateway)
        Else
            gatewayService.Add(paymentGateway)
        End If

        Response.Redirect(Me.m_sPageName & IIf(m_iID > 0, "?action=view&id=" & Me.m_iID, ""), False)
    End Sub

#End Region

#Region "Helpers"

    Protected Sub SetLabels()

        Me.litPaymentOptions.Text = GetMessage("lbl commerce payment options")
        Me.litPaymentGatways.Text = GetMessage("lbl payment gateways")

        Me.ltr_name.Text = Me.GetMessage("lbl gateway name")
        Me.ltr_id.Text = Me.GetMessage("lbl gateway id")
        Me.ltr_default.Text = Me.GetMessage("lbl gateway default")
        Me.ltr_uid.Text = Me.GetMessage("lbl gateway userid")
        Me.ltr_pwd.Text = Me.GetMessage("lbl gateway password")
        Me.ltr_showcustom.Text = "<a href=""javascript: void(0);"" onclick=""ToggleDiv('tbl_custom');"">" & GetMessage("lbl gateway expand custom") & "</a>"
        Me.ltr_spare1.Text = Me.GetMessage("lbl gateway custom1")
        Me.ltr_spare2.Text = Me.GetMessage("lbl gateway custom2")
        Select Case MyBase.m_sPageAction
            Case "view"

                Me.pnl_view.Visible = True
                Me.pnl_viewall.Visible = False
                Me.AddButtonwithMessages(imageIconsPath & "contentEdit.png", Me.m_sPageName & "?action=addedit&id=" & Me.m_iID.ToString(), "generic edit title", "generic edit title", "")
                If Not m_bIsDefault Then
                    Me.AddButtonwithMessages(Me.AppImgPath & "icon_survey_enable.gif", Me.m_sPageName & "?action=markdef&id=" & Me.m_iID.ToString(), "lbl gateway mark def", "lbl gateway mark def", "")
                    Me.AddButtonwithMessages(imageIconsPath & "delete.png", Me.m_sPageName & "?action=del&id=" & Me.m_iID.ToString(), "alt del gateway button text", "btn delete", " onclick=""return CheckDelete();"" ")
                End If
                Me.AddBackButton(Me.m_sPageName)
                Me.SetTitleBarToMessage("lbl view gateway")
                Me.AddHelpButton("ViewPaymentGateway")

            Case "addedit"

                Me.pnl_view.Visible = True
                Me.pnl_viewall.Visible = False
                If Me.m_iID > 0 Then
                    Me.AddButtonwithMessages(imageIconsPath & "save.png", "#", "lbl alt save gateway", "btn save", " onclick=""SubmitForm(); return false;"" ")
                    Me.AddBackButton(Me.m_sPageName & "?action=view&id=" & m_iID.ToString())
                    Me.SetTitleBarToMessage("lbl edit gateway")
                    Page.ClientScript.RegisterStartupScript(Me.GetType(), "protectpwd", "ProtectPassword();", True)
                    Me.AddHelpButton("EditPaymentGateway")
                Else
                    Me.AddButtonwithMessages(imageIconsPath & "save.png", "#", "lbl alt add gateway", "btn save", " onclick=""SubmitForm(); return false;"" ")
                    Me.AddBackButton(Me.m_sPageName)
                    Me.SetTitleBarToMessage("lbl add gateway")
                    Me.AddHelpButton("AddPaymentGateway")
                End If

            Case "editoptions"

                AddButtonwithMessages(imageIconsPath & "save.png", "#", "lbl alt save payment options", "btn save", " onclick=""SubmitOptionsForm(); return false;"" ")
                AddBackButton(Me.m_sPageName)
                SetTitleBarToMessage("lbl payment options edit")
                AddHelpButton("paymentgateway")

            Case Else ' "viewall"

                Dim newMenu As New workareamenu("file", Me.GetMessage("lbl new"), imageIconsPath & "star.png")
                newMenu.AddLinkItem(Me.AppImgPath & "menu/card.gif", Me.GetMessage("lbl payment gateway"), Me.m_sPageName & "?action=addedit")
                Me.AddMenu(newMenu)

                Dim actionMenu As New workareamenu("action", Me.GetMessage("lbl action"), imageIconsPath & "check.png")
                actionMenu.AddItem(Me.AppImgPath & "icon_survey_enable.gif", Me.GetMessage("lbl gateway mark def"), "CheckMarkAsDef();")
                actionMenu.AddBreak()
                actionMenu.AddLinkItem(imageIconsPath & "contentEdit.png", Me.GetMessage("lbl payment options edit"), Me.m_sPageName & "?action=editoptions")
                Me.AddMenu(actionMenu)

                Me.SetTitleBarToMessage("lbl payment options")
                Me.AddHelpButton("paymentgateway")

        End Select

        SetJs()
    End Sub

    Private Sub SetJs()
        Dim sbJS As New StringBuilder
        sbJS.Append("<script language=""javascript"" type=""text/javascript"" >" & Environment.NewLine)

        sbJS.Append("function UpdateOptions(dropdown)" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("   var cards = SupportsCards(dropdown.selectedIndex);" & Environment.NewLine)
        sbJS.Append("   if (!cards) { document.getElementById('chk_cc').disabled = true; document.getElementById('chk_cc').checked = false; } " & Environment.NewLine)
        sbJS.Append("   else { document.getElementById('chk_cc').disabled = false; }" & Environment.NewLine)

        sbJS.Append("   var checks = SupportsChecks(dropdown.selectedIndex);" & Environment.NewLine)
        sbJS.Append("   if (!checks) { document.getElementById('chk_check').disabled = true; document.getElementById('chk_check').checked = false; } " & Environment.NewLine)
        sbJS.Append("   else { document.getElementById('chk_check').disabled = false; } " & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function SupportsCards(idx)" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("    var cardsupport = new Array(); " & Environment.NewLine)
        For i As Integer = 0 To (supportsCards.Count - 1)
            sbJS.Append("    cardsupport[" & i.ToString() & "] = " & supportsCards(i).ToString().ToLower() & ";" & Environment.NewLine)
        Next
        sbJS.Append("    return cardsupport[idx]; " & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function SupportsChecks(idx)" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("    var checksupport = new Array(); " & Environment.NewLine)
        For i As Integer = 0 To (supportsChecks.Count - 1)
            sbJS.Append("    checksupport[" & i.ToString() & "] = " & supportsChecks(i).ToString().ToLower() & ";" & Environment.NewLine)
        Next
        sbJS.Append("    return checksupport[idx]; " & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function CheckDelete()" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("    return confirm('").Append(GetMessage("js gateway confirm del")).Append("');" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function SubmitOptionsForm()" & Environment.NewLine)
        sbJS.Append("{ " & Environment.NewLine)
        sbJS.Append("    document.forms[0].submit();" & Environment.NewLine)
        sbJS.Append("    return false;" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function SubmitForm()" & Environment.NewLine)
        sbJS.Append("{ " & Environment.NewLine)

        sbJS.Append("} " & Environment.NewLine)

        sbJS.Append("function SubmitForm()" & Environment.NewLine)
        sbJS.Append("{var userID=document.getElementById('txt_uid').value;var pwd=document.getElementById('txt_pwd').value;" & Environment.NewLine)
        sbJS.Append("	if(userID.indexOf('<') > -1 || userID.indexOf('>') > -1 || pwd.indexOf('>') > -1 || pwd.indexOf('<') > -1) {").Append(Environment.NewLine)
        sbJS.Append("		alert(""").Append(String.Format(GetMessage("js alert field cannot include"), "<, >")).Append(""");").Append(Environment.NewLine)
        sbJS.Append("		document.getElementById('").Append(txt_uid.UniqueID).Append("').focus(); return false;").Append(Environment.NewLine)
        sbJS.Append("	} ").Append(Environment.NewLine)
        sbJS.Append("    document.forms[0].submit();" & Environment.NewLine)
        sbJS.Append("    return false;" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function CheckForillegalChar(txtName) {" & Environment.NewLine)
        sbJS.Append("   var val = txtName;" & Environment.NewLine)
        sbJS.Append("   if ((val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbJS.Append("   {" & Environment.NewLine)
        sbJS.Append("       alert(""").Append(String.Format(GetMessage("js alert cc type name cant include"), "('\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')")).Append(""");" & Environment.NewLine)
        sbJS.Append("       return false;" & Environment.NewLine)
        sbJS.Append("   }" & Environment.NewLine)
        sbJS.Append("   return true;" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function CheckMarkAsDef() {" & Environment.NewLine)
        sbJS.Append(" 	var chosen = ''; ").Append(Environment.NewLine)
        sbJS.Append(" 	var len = document.forms[0].radio_gateway.length; ").Append(Environment.NewLine)
        sbJS.Append(" 	if (len > 0) { ").Append(Environment.NewLine)
        sbJS.Append(" 	    for (i = 0; i < len; i++) { ").Append(Environment.NewLine)
        sbJS.Append(" 		    if (document.form1.radio_gateway[i].checked) { ").Append(Environment.NewLine)
        sbJS.Append(" 			    chosen = document.form1.radio_gateway[i].value; ").Append(Environment.NewLine)
        sbJS.Append(" 		    } ").Append(Environment.NewLine)
        sbJS.Append(" 	    } ").Append(Environment.NewLine)
        sbJS.Append(" 	} else { ").Append(Environment.NewLine)
        sbJS.Append(" 	    if (document.form1.radio_gateway.checked) { chosen = document.form1.radio_gateway.value; } ").Append(Environment.NewLine)
        sbJS.Append(" 	} ").Append(Environment.NewLine)
        sbJS.Append(" 	if (chosen == '') { ").Append(Environment.NewLine)
        sbJS.Append(" 		alert('").Append(GetMessage("js please choose gateway")).Append("'); ").Append(Environment.NewLine)
        sbJS.Append(" 	} else if (confirm('").Append(GetMessage("js gateway mark def")).Append("')) { ").Append(Environment.NewLine)
        sbJS.Append(" 		window.location.href = 'paymentgateway.aspx?action=markdef&id=' + chosen; ").Append(Environment.NewLine)
        sbJS.Append(" 	} ").Append(Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function ProtectPassword() {" & Environment.NewLine)
        sbJS.Append("   var objtitle = document.getElementById(""").Append(txt_pwd.UniqueID).Append(""");" & Environment.NewLine)
        sbJS.Append("   objtitle.value = '          ';" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append(JSLibrary.ToggleDiv())

        sbJS.Append("</script>" & Environment.NewLine)
        ltr_js.Text &= Environment.NewLine & sbJS.ToString()
    End Sub

    Public Function ProtectPassword(ByVal pwd As String) As String
        Return "**********"
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

    Protected Sub Util_SetPaymentCheckBoxes()

        If Not Page.IsPostBack Then

            appSettings = (New SiteAPI()).GetSiteVariables()

            chk_paypal.Checked = appSettings.PaymentSettings.PayPal
            'chk_google.Checked = appSettings.PaymentSettings.GoogleCheckout

        End If

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

#End Region

#Region "JS/CSS"

    Private Sub RegisterJS()

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, MyBase.m_refContentApi.ApplicationPath & "/wamenu/includes/com.ektron.ui.menu.js", "EktronMenuJs")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)

    End Sub

    Private Sub RegisterCSS()

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)
        Ektron.Cms.API.Css.RegisterCss(Me, MyBase.m_refContentApi.ApplicationPath & "/wamenu/css/com.ektron.ui.menu.css", "EktronMenuCss")
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)

    End Sub

#End Region

End Class
