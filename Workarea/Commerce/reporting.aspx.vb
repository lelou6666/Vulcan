Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea
Imports System.Web.UI.Page
Imports System.Collections.generic
Partial Class Commerce_reporting
    Inherits workareabase

    Protected cCustomer As CustomerData = Nothing
    Protected order As OrderData = Nothing
    Protected CustomerManager As Customer = Nothing

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        Util_RegisterResources()
        Utilities.ValidateUserLogin()
        If (m_refContentApi.RequestInformationRef.IsMembershipUser) Then
            Response.Redirect(m_refContentApi.ApplicationPath & "reterror.aspx?info=" & m_refContentApi.EkMsgRef.GetMessage("msg login cms user"), True)
            Exit Sub
        End If
        CustomerManager = New Customer(m_refContentApi.RequestInformationRef)
        Select Case Me.m_sPageAction
            Case "mostrecent"
                Display_MostRecent()
            Case "onhold"
                Display_OnHold()
            Case "bydates"
                Display_ByDates()
            Case "bycustomer"
                Display_ByCustomer()
            Case "byproduct"
                Display_ByProduct()
            Case "custom"
                Display_Custom()
        End Select
        Util_SetLabels()
    End Sub


#Region "Display"
    Protected Sub Display_OnHold()
        Dim orderList As New List(Of OrderData)
        Dim orderApi As New OrderApi

        orderList = orderApi.GetOnHoldOrderList(New PagingInfo)
        dg_orders.DataSource = orderList
        dg_orders.DataBind()
    End Sub
    Protected Sub Display_ViewOrder()
        Dim orderApi As New OrderApi
        order = orderApi.GetItem(Me.m_iID)

        Me.ltr_id.Text = order.Id
        Me.ltr_customer.Text = Util_ShowCustomer(order.Customer)
        Me.ltr_created.Text = Util_ShowDate(order.DateCreated)
        Me.ltr_completed.Text = Util_ShowDate(order.DateCompleted)
        Me.ltr_required.Text = Util_ShowDate(order.DateRequired)
        Me.ltr_orderstatus.Text = System.Enum.GetName(GetType(EkEnumeration.OrderStatus), order.Status)
        Me.ltr_ordertotal.Text = FormatCurrency(order.OrderTotal)
        Me.ltr_pipelinestage.Text = order.StageName

        Me.dg_orderparts.DataSource = order.Parts
        Me.dg_orderparts.DataBind()
        Me.dg_orderlines.DataSource = order.Parts(0).Lines
        Me.dg_orderlines.DataBind()
    End Sub

    Protected Sub Display_MostRecent()

        Dim orderList As New List(Of OrderData)
        Dim orderApi As New OrderApi

        orderList = orderApi.GetList(New Criteria(Of OrderProperty))

        dg_orders.DataSource = orderList
        dg_orders.DataBind()
    End Sub

    Protected Sub Display_ByDates()

        Dim startDate As DateTime = Request.QueryString("startdate")
        Dim endDate As DateTime = Request.QueryString("enddate")
        Dim orderList As New List(Of OrderData)
        Dim orderApi As New OrderApi

        orderList = orderApi.GetList(startDate, endDate, New PagingInfo)

        dg_orders.DataSource = orderList
        dg_orders.DataBind()
    End Sub

    Protected Sub Display_ByCustomer()

        Response.Redirect("customers.aspx")
        'Dim aOrders() As Order = Array.CreateInstance(GetType(Order), 0)
        'aOrders = Order.GetAllOrders(1, 0, 0, 0, Me.m_refContentApi.RequestInformationRef)

        'dg_orders.DataSource = aOrders
        'dg_orders.DataBind()
    End Sub

    Protected Sub Display_ByProduct()
        Dim orderList As New List(Of OrderData)
        Dim orderApi As New OrderApi

        orderList = orderApi.GetList(New Criteria(Of OrderProperty))

        dg_orders.DataSource = orderList
        dg_orders.DataBind()
    End Sub

    Protected Sub Display_Custom()
        Dim orderList As New List(Of OrderData)
        Dim orderApi As New OrderApi

        orderList = orderApi.GetList(New Criteria(Of OrderProperty))

        dg_orders.DataSource = orderList
        dg_orders.DataBind()
    End Sub

#End Region

#Region "Process"

#End Region

#Region "Util"
    Protected Sub Util_SetLabels()
        Me.ltr_id_lbl.Text = Me.GetMessage("lbl order id")
        Me.ltr_customer_lbl.Text = Me.GetMessage("lbl customer")
        Me.ltr_created_lbl.Text = Me.GetMessage("generic datecreated")
        Me.ltr_required_lbl.Text = Me.GetMessage("lbl date required")
        Me.ltr_completed_lbl.Text = Me.GetMessage("lbl date completed")
        Me.ltr_orderstatus_lbl.Text = Me.GetMessage("lbl order status")
        Me.ltr_ordertotal_lbl.Text = Me.GetMessage("lbl order total")
        Me.ltr_pipelinestage_lbl.Text = Me.GetMessage("lbl order pipeline stage")
        Select Case Me.m_sPageAction
            Case "vieworder"
                Me.pnl_view.Visible = True
                Me.SetTitleBarToMessage("lbl view order")
            Case Else ' "viewall"
                Dim newMenu As New workareamenu("file", Me.GetMessage("lbl order reporting"), Me.AppImgPath & "commerce/catalog_view.gif")
                newMenu.AddItem(Me.AppImgPath & "/commerce/calendar_down.gif", Me.GetMessage("lbl report most recent orders"), "window.location.href='reporting.aspx?action=mostrecent';")
                newMenu.AddItem(Me.AppImgPath & "/commerce/calendar.gif", Me.GetMessage("lbl report date orders"), "window.location.href='reporting.aspx?action=bydates';")
                newMenu.AddItem(Me.AppImgPath & "/menu/users2.gif", Me.GetMessage("lbl report customer orders"), "window.location.href='reporting.aspx?action=bycustomer';")
                newMenu.AddItem(m_refContentApi.AppPath & "images/ui/icons/brick.png", Me.GetMessage("lbl report product orders"), "window.location.href='reporting.aspx?action=byproduct';")
                newMenu.AddItem(Me.AppImgPath & "/menu/form_blue.gif", Me.GetMessage("lbl report custom orders"), "window.location.href='reporting.aspx?action=custom';")
                Me.AddMenu(newMenu)
                Me.SetTitleBarToMessage("lbl orders")
        End Select
        Me.AddHelpButton("orders")
    End Sub
    Public Function Util_ShowDate(ByVal dtDate As DateTime) As String
        Dim sRet As String = ""
        If dtDate = DateTime.MinValue Then
            sRet = "-"
        Else
            sRet = dtDate.ToShortDateString & " " & dtDate.ToShortTimeString
        End If
        Return sRet
    End Function
    Public Function Util_ShowCustomer(ByVal Customer As CustomerData) As String
        Dim sRet As String = ""

        ' sRet &= cCustomer.CustomerId & "<br/>"
        sRet &= "<a href=""customers.aspx?action=view&id=" & Customer.Id
        sRet &= "<br/>Value:  " & FormatCurrency(Customer.TotalOrderValue)

        Return sRet
    End Function
    Public Function Util_ShowAddress(ByVal ShippingAddressId As Integer) As String
        Dim sbRet As New StringBuilder()
        Dim shipAddress As AddressData = Nothing
        Dim AddressManager As IAddress = ObjectFactory.GetAddress()

        shipAddress = AddressManager.GetItem(cCustomer.ShippingAddressId)
        sbRet.Append(shipAddress.AddressLine1).Append("<br />")
        If shipAddress.AddressLine2.Trim().Length() > 0 Then
            sbRet.Append(shipAddress.AddressLine2).Append("<br />")
        End If
        sbRet.Append(shipAddress.City).Append("<br />")
        sbRet.Append(shipAddress.Region.Name).Append(", ")
        sbRet.Append(shipAddress.PostalCode).Append("<br />")
        sbRet.Append(shipAddress.Country.Name).Append("<br />")

        Return sbRet.ToString()
    End Function
    Private Sub Util_RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCSS")
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS")
    End Sub
#End Region

End Class
