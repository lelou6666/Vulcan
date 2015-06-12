Imports System.Collections.Generic
Imports Ektron
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.API

Partial Class Commerce_shipping_methods
    Inherits workareabase
    Implements ICallbackEventHandler

    Protected m_refShipping As ShippingMethodApi = Nothing
    Protected m_sPageName As String = "shippingmethods.aspx"
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1

#Region "Page Functions"


    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then Throw New Exception(GetMessage("feature locked error"))
            Utilities.ValidateUserLogin()
            CommerceLibrary.CheckCommerceAdminAccess()

            m_refShipping = New ShippingMethodApi()

            Select Case Me.m_sPageAction
                Case "addedit"
                    If Page.IsPostBack AndAlso Not (Page.IsCallback) Then
                        Process_AddEdit()
                    ElseIf Not (Page.IsCallback) Then
                        Display_AddEdit()
                    End If

                Case "reorder"

                    Reorder1.Initialize(m_refShipping.RequestInformationRef)
                    If Page.IsPostBack AndAlso Not (Page.IsCallback) Then Process_Reorder() Else Display_Reorder()

                Case "del"
                    Process_Delete()
                Case "view"
                    Display_View()
                Case Else
                    If Page.IsPostBack = False Then
                        Display_All()
                    End If
            End Select

            Util_SetLabels()
            Util_SetJS()

        Catch ex As Exception

            Utilities.ShowError(ex.Message)

        End Try

    End Sub
#End Region

#Region "Process"

    Protected Sub Process_Reorder()

        Dim itemList As String() = Split(Request.Form("LinkOrder"), ",")

        For i As Integer = 0 To (itemList.Length - 1)

            Dim itemArray As String() = Split(itemList(i), "|")

            If (itemArray.Length > 0 AndAlso IsNumeric(itemArray(0))) Then

                Dim shipOption As ShippingMethodData = m_refShipping.GetItem(itemArray(0))

                shipOption.DisplayOrder = (i + 1)

                m_refShipping.Update(shipOption)

            End If

        Next

        Page.Response.Write("<script language=""javascript"">parent.location.href = 'shippingmethods.aspx';</script>")

    End Sub
    Protected Sub Process_AddEdit()
        Dim shipOption As New ShippingMethodData()
        If m_iID > 0 Then shipOption = m_refShipping.GetItem(Me.m_iID)

        shipOption.Name = txt_name.Text
        shipOption.IsActive = chk_active.Checked
        shipOption.ProviderService = txt_provservice.Text
        If Me.m_iID > 0 Then
            m_refShipping.Update(shipOption)
            Response.Redirect(m_sPageName & "?action=view&id=" & m_iID.ToString(), False)
        Else
            m_refShipping.Add(shipOption)
            Response.Redirect(m_sPageName, False)
        End If
    End Sub
    Protected Sub Process_Delete()
        If Me.m_iID > 0 Then m_refShipping.Delete(m_iID)
        Response.Redirect(m_sPageName, False)
    End Sub
#End Region

#Region "Display"
    Protected Sub Display_Reorder()

        Dim optionList As List(Of ShippingMethodData)
        Dim criteria As New Criteria(Of ShippingMethodProperty)

        criteria.PagingInfo.RecordsPerPage = 1000
        criteria.PagingInfo.CurrentPage = 1
        criteria.OrderByField = ShippingMethodProperty.DisplayOrder
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending

        optionList = m_refShipping.GetList(criteria)

        For i As Integer = 0 To (optionList.Count - 1)

            Reorder1.addItem(optionList(i).Name, optionList(i).Id, 0)

        Next

        pnl_reorder.Visible = True
        pnl_viewall.Visible = False

    End Sub
    Protected Sub Display_AddEdit()
        Dim shipOption As New ShippingMethodData()
        If m_iID > 0 Then shipOption = m_refShipping.GetItem(Me.m_iID)

        txt_name.Text = shipOption.Name
        lbl_id.Text = shipOption.Id
        tr_id.Visible = (m_iID > 0)
        chk_active.Checked = shipOption.IsActive
        txt_provservice.Text = shipOption.ProviderService

        pnl_view.Visible = True
        pnl_viewall.Visible = False
        ltr_viewopt.Text = "&nbsp;<a href=""#"" onclick=""GetServiceOptions();"">View Options</a>"
    End Sub
    Protected Sub Display_All()

        Dim optionList As List(Of ShippingMethodData)
        Dim criteria As New Criteria(Of ShippingMethodProperty)

        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        criteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()
        criteria.OrderByField = ShippingMethodProperty.DisplayOrder
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending

        optionList = m_refShipping.GetList(criteria)

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

        dg_viewall.DataSource = optionList
        dg_viewall.DataBind()

    End Sub
    Protected Sub Display_View()
        Dim shipOption As New ShippingMethodData()

        shipOption = m_refShipping.GetItem(Me.m_iID)

        txt_name.Text = shipOption.Name
        lbl_id.Text = shipOption.Id
        chk_active.Checked = shipOption.IsActive
        txt_provservice.Text = shipOption.ProviderService

        txt_name.Enabled = False
        chk_active.Enabled = False
        txt_provservice.Enabled = False

        pnl_view.Visible = True
        pnl_viewall.Visible = False
    End Sub
#End Region

#Region "Private Helpers"
    Protected Sub Util_SetLabels()

        ltr_appPath.text = Me.m_refContentApi.AppPath

        Select Case Me.m_sPageAction
            Case "addedit"
                Me.AddButtonwithMessages(AppImgPath & "../UI/Icons/save.png", m_sPageName & "?action=addedit&id=" & m_iID.ToString(), "btn save", "btn save", " onclick="" return SubmitForm();"" ")
                AddBackButton(m_sPageName & IIf(m_iID > 0, "?action=view&id=" & Me.m_iID.ToString(), ""))
                If Me.m_iID > 0 Then
                    SetTitleBarToMessage("lbl edit shipping method")
                    AddHelpButton("EditShippingMethod")
                Else
                    SetTitleBarToMessage("lbl add shipping method")
                    AddHelpButton("AddShippingMethod")
                End If
            Case "view"
                Me.AddButtonwithMessages(AppImgPath & "../UI/Icons/contentEdit.png", m_sPageName & "?action=addedit&id=" & m_iID.ToString(), "generic edit title", "generic edit title", "")
                Me.AddButtonwithMessages(AppImgPath & "../UI/Icons/delete.png", m_sPageName & "?action=del&id=" & m_iID.ToString(), "generic delete title", "generic delete title", " onclick=""return confirm('" & GetMessage("js confirm delete shipping method") & "');"" ")
                AddBackButton(m_sPageName)
                SetTitleBarToMessage("lbl view shipping method")
                AddHelpButton("ViewShippingMethod")

            Case "reorder"

                Dim actionMenu As New workareamenu("action", Me.GetMessage("lbl action"), Me.AppImgPath & "../UI/Icons/check.png")
                actionMenu.AddItem(m_refContentApi.AppPath & "images/ui/icons/save.png", Me.GetMessage("btn save"), "document.forms[0].submit();")
                actionMenu.AddBreak()
                actionMenu.AddItem(m_refContentApi.AppPath & "images/ui/icons/cancel.png", Me.GetMessage("generic cancel"), "parent.$ektron('.ektronShippingReorderModal').modalHide();")
                Me.AddMenu(actionMenu)

                SetTitleBarToMessage("lbl reorder reorder shipping methods")
                AddHelpButton("ReorderShippingMethods")

            Case Else
                Dim newMenu As New workareamenu("file", GetMessage("lbl new"), m_refContentApi.AppPath & "images/UI/Icons/star.png")
                newMenu.AddLinkItem(m_refContentApi.AppPath & "images/UI/Icons/filetypes/text.png", GetMessage("lbl shipping method"), m_sPageName & "?action=addedit")
                Me.AddMenu(newMenu)

                Dim actionMenu As New workareamenu("action", Me.GetMessage("lbl action"), m_refContentApi.AppPath & "images/UI/Icons/check.png")
                actionMenu.AddItem(m_refContentApi.AppPath & "images/ui/icons/collection.png", Me.GetMessage("btn reorder"), "OpenReorder();")
                Me.AddMenu(actionMenu)

                SetTitleBarToMessage("lbl shipping methods")
                AddHelpButton("ShippingMethods")
        End Select

        ltr_name.Text = GetMessage("generic name")
        ltr_id.Text = GetMessage("generic id")
        ltr_active.Text = GetMessage("lbl active")
        ltr_provservice.Text = GetMessage("lbl provider service")
    End Sub
    Protected Sub Util_SetJS()

        JS.RegisterJS(Me, JS.ManagedScript.EktronJS)
        JS.RegisterJS(Me, JS.ManagedScript.EktronModalJS)
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "controls/Reorder/js/Reorder.js", "EktronReorderJs")

        Css.RegisterCss(Me, Css.ManagedStyleSheet.EktronModalCss)

        Dim sbJS As New StringBuilder()

        sbJS.Append("<script language=""javascript"">").Append(Environment.NewLine)

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine)
        sbJS.Append(JSLibrary.AddError("aSubmitErr"))
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"))
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"))
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection))

        sbJS.Append(" function validate_Title() { ").Append(Environment.NewLine)
        sbJS.Append("   var sTitle = Trim(document.getElementById('").Append(txt_name.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   if (sTitle == '') { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err shipping method title req")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar('").Append(txt_name.UniqueID).Append("',""").Append(GetMessage("lbl shipping method disallowed chars")).Append("""); ").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar('").Append(txt_provservice.UniqueID).Append("',""").Append(GetMessage("lbl shipping method provider service disallowed chars")).Append("""); ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append(" function SubmitForm() { ").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine)
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("('document.forms[0].submit();');").Append(Environment.NewLine)
        sbJS.Append("   return false; ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append(" function UpdateOptions(result, context) { ").Append(Environment.NewLine)
        sbJS.Append("   document.getElementById('dvOptions').innerHTML = result; ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append(" function UpdateService(selvalue) { ").Append(Environment.NewLine)
        sbJS.Append("   document.getElementById('").Append(txt_provservice.UniqueID).Append("').value = selvalue; ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append(" function GetServiceOptions() { ").Append(Environment.NewLine)
        sbJS.Append("   ").Append(Me.ClientScript.GetCallbackEventReference(Me, "", "UpdateOptions", "null")).Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append("</script>").Append(Environment.NewLine)
        ltr_js.Text &= sbJS.ToString()
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
        Display_All()
        isPostData.Value = "true"
    End Sub
#End Region
#Region "CallBack"
    Dim callbackresult As String = ""
    Public Function GetCallbackResult() As String Implements ICallbackEventHandler.GetCallbackResult
        Return callbackresult
    End Function
    Public Sub RaiseCallbackEvent(ByVal eventArgument As String) Implements ICallbackEventHandler.RaiseCallbackEvent
        Try
            Dim aServiceTypes As List(Of String)
            Dim shipProvider As New Shipment.Provider.ShipmentProviderManager()

            aServiceTypes = Shipment.Provider.ShipmentProviderManager.Provider.GetServiceTypes()
            callbackresult = "<label class=""label"">From " & Shipment.Provider.ShipmentProviderManager.Provider.Name & "</label>:<br /><br /><select id='drp_options' onchange='UpdateService(this.value);'>"
            callbackresult &= "<option value=''>" & GetMessage("generic select") & "</option>"
            For i As Integer = 0 To (aServiceTypes.Count - 1)
                callbackresult &= "<option value='" & aServiceTypes(i) & "'>" & aServiceTypes(i) & "</option>"
            Next
            callbackresult &= "</select>"
        Catch ex As Exception
            callbackresult = "<img src=""" & AppImgPath & "alert.gif""><span class=""important"">" & ex.Message & "</span>"
        End Try

    End Sub
#End Region

End Class
