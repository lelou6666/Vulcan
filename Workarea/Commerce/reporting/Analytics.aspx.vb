Imports Ektron.Cms.Workarea
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.common
Imports System.Collections.generic

Partial Class Commerce_Reporting_Analytics
    Inherits workareabase

    Protected m_sPageName As String = "Analytics.aspx"
    Protected m_FolderId As Long = 0
    Protected SiteList As New List(Of String)
    Protected defaultCurrency As CurrencyData = Nothing
    Protected orderApi As OrderApi
    Protected CustomerManager As Customer = Nothing
    Private reportData As New Ektron.Cms.Commerce.Reporting.OrderReportData

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            orderApi = New OrderApi()
            Util_RegisterResources()
            defaultCurrency = (New CurrencyApi()).GetItem(m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId)
            Dim siteCookie As System.Web.HttpCookie = CommonApi.GetEcmCookie()
            If siteCookie("SiteCurrency") <> defaultCurrency.Id Then
                defaultCurrency.Id = siteCookie("SiteCurrency")
                Dim m_refCurrencyApi As New CurrencyApi
                defaultCurrency = m_refCurrencyApi.GetItem(defaultCurrency.Id)
            End If
            m_refMsg = m_refContentApi.EkMsgRef
            CustomerManager = New Customer(m_refContentApi.RequestInformationRef)
            Util_CheckAccess()
            If Request.QueryString("folder") <> "" Then m_FolderId = Request.QueryString("folder")

            Select Case MyBase.m_sPageAction

                Case Else ' "commerce"

                    Display_Commerce()

                    'Case Else ' "viewall"

                    'Display_View_All()

            End Select
            Util_SetLabels()

        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try

    End Sub
#Region "Display"

    Protected Sub Display_View_All()
        If (Request.Url.Scheme.ToLower() <> "https") Then
            Select Case Me.m_sPageAction

                Case "visitors"

                    img_graph.ImageUrl = "http://chart.apis.google.com/chart?cht=lc&chd=t:0,30,60,70,90,95,100&chs=250x100&chl=1|2|3|4|5|6|7"
                    Me.ltr_description.Text = "<img src=""http://chart.apis.google.com/chart?cht=p3&chd=t:10,90&chs=250x75&chl=New|Returning""/>"
                    Me.ltr_description.Text &= "<br/><img src=""http://chart.apis.google.com/chart?cht=p3&chd=t:60,15,10&chs=250x75&chl=Direct|Google|Yahoo""/>"

                Case "source"

                    img_graph.ImageUrl = "http://chart.apis.google.com/chart?cht=p3&chd=t:60,15,10&chs=250x100&chl=Direct|Google|Yahoo"

                Case "pageviews"

                    img_graph.ImageUrl = "http://chart.apis.google.com/chart?cht=lc&chd=t:0,30,60,70,90,95,100&chs=250x100&chl=1|2|3|4|5|6|7"
                    Me.ltr_description.Text = "<img src=""http://chart.apis.google.com/chart?cht=p3&chd=t:40,31,56&chs=250x75&chl=default.aspx|Specials.aspx|product.aspx""/>"

                Case "commerce"

                    img_graph.ImageUrl = "http://chart.apis.google.com/chart?cht=lc&chd=t:5,33,50,55,7&chs=250x100&chl=1|2|3|4|5|6|7"
                    Me.ltr_description.Text = "<img src=""http://chart.apis.google.com/chart?cht=p3&chd=t:10,90&chs=250x75&chl=Coupon|No Coupon""/>"
                    Me.ltr_description.Text &= "<br/><img src=""http://chart.apis.google.com/chart?cht=p3&chd=t:60,40&chs=250x75&chl=New|Returning""/>"

                Case Else

                    img_graph.ImageUrl = "http://chart.apis.google.com/chart?cht=p3&chd=t:60,40&chs=250x100&chl=Hello|World"

            End Select
        Else
            img_graph.Visible = False
            ltr_description.Visible = False
        End If
        Dim m_refAnalyticsAPI As New AnalyticsAPI()
        Dim dsAnalytics As System.Data.DataSet = m_refAnalyticsAPI.QueryAnalytics("select * from content_hits_tbl where content_id = " & m_iID.ToString())
        dg_cctypes.DataSource = dsAnalytics
        dg_cctypes.DataBind()

    End Sub

    Protected Sub Display_Commerce()

        Dim orderApi As New Ektron.Cms.Commerce.OrderApi()
        Dim reportCriteria As New Ektron.Cms.Common.Criteria(Of OrderProperty)

        reportCriteria.AddFilter(OrderProperty.ProductId, Common.CriteriaFilterOperator.EqualTo, m_iID)

        reportData = orderApi.GetReport(reportCriteria)

        If reportData.Orders.Count > 0 Then
            If (Request.Url.Scheme.ToLower() <> "https") Then

                img_graph.ImageUrl = Util_GetDateGraph()
                Me.ltr_description.Text = "<img src=""http://chart.apis.google.com/chart?cht=p3&chd=t:" & reportData.WithCoupons & "," & reportData.WithoutCoupons & "&chs=250x75&chl=Coupon-" & reportData.WithCoupons & "|No Coupon-" & reportData.WithoutCoupons & """/>"
                Me.ltr_description.Text &= "<br/><img src=""http://chart.apis.google.com/chart?cht=p3&chd=t:" & reportData.ByNewCustomers & "," & reportData.ByReturningCustomers & "&chs=250x75&chl=New-" & reportData.ByNewCustomers & "|Returning-" & reportData.ByReturningCustomers & """/>"
            Else
                img_graph.Visible = False
                ltr_description.Visible = False
            End If
            dg_cctypes.DataSource = reportData.Orders
            dg_cctypes.DataBind()

            Util_ShowSites()
        Else

            ltr_noOrders.Text = m_refMsg.GetMessage("lbl no orders")
            img_graph.Visible = False
            ltr_description.Visible = False

        End If

    End Sub

#End Region

#Region "Process"

    Protected Sub Process_()



    End Sub

#End Region

#Region "Util"

    Protected Function Util_GetDateGraph() As String

        Dim graphURL As New StringBuilder()
        Dim xAxis As String = ""
        Dim yAxis As String = ""

        xAxis = "0:"

        For i As Integer = 7 To 0 Step -1

            If xAxis <> "" Then xAxis &= "|" & DateTime.Now.Subtract(New TimeSpan(i, 0, 0, 0)).Day Else xAxis = DateTime.Now.Subtract(New TimeSpan(i, 0, 0, 0)).Day
            If yAxis <> "" Then yAxis &= "," & (10 * reportData.Dates.DayTotal(DateTime.Now.Subtract(New TimeSpan(i, 0, 0, 0)))) Else yAxis = (10 * reportData.Dates.DayTotal(DateTime.Now.Subtract(New TimeSpan(i, 0, 0, 0))))

        Next

        xAxis &= "|1:|0|2|4|6|8|10|2:||" & DateTime.Now.ToString("MMMM") & " " & DateTime.Now.Year & "||3:||Orders|"

        graphURL.Append("http://chart.apis.google.com/chart")
        graphURL.Append("?")
        graphURL.Append("cht=lc")
        graphURL.Append("&")
        graphURL.Append("chxt=x,y,x,y")
        graphURL.Append("&")
        graphURL.Append("chd=t:" & yAxis)
        graphURL.Append("&")
        graphURL.Append("chs=300x150")
        graphURL.Append("&")
        graphURL.Append("chxl=" & xAxis)

        ' chxl=0:|22|23|24|25|26|1:|0|5|10|2:||Oct||3:||Orders|

        Return graphURL.ToString()


    End Function

    Protected Sub Util_SetLabels()

        Select Case MyBase.m_sPageAction

            ' Case Else ' "view"

            'Dim newMenu As New workareamenu("file", Me.GetMessage("lbl view"), Me.AppImgPath & "commerce/analytics.gif")

            'newMenu.AddLinkItem(Me.AppImgPath & "commerce/analytics.gif", Me.GetMessage("summary text"), Me.m_sPageName & "?action=summary")
            'newMenu.AddLinkItem(Me.AppImgPath & "menu/users2.gif", Me.GetMessage("visitors lbl"), Me.m_sPageName & "?action=visitors")
            'newMenu.AddLinkItem(Me.AppImgPath & "DMSmenu/world.gif", Me.GetMessage("lbl source"), Me.m_sPageName & "?action=source")
            'newMenu.AddLinkItem(Me.AppImgPath & "menu/multipledmsdocs.gif", Me.GetMessage("page views"), Me.m_sPageName & "?action=pageviews")
            'newMenu.AddLinkItem(Me.AppImgPath & "commerce/renameCart.gif", Me.GetMessage("lbl commerce"), Me.m_sPageName & "?action=commerce")

            'Me.AddMenu(newMenu)


            Case Else

                Me.SetTitleBarToMessage("lbl catalog entry analytics")

        End Select

        If (Request.QueryString("callerpage") = "dashboard.aspx") Then
            AddBackButton("javascript:top.switchDesktopTab()")
        Else
            AddBackButton("javascript:history.go(-1);")
        End If
        AddHelpButton("entryanalytics")

        Util_SetJs()

    End Sub

    Private Sub Util_SetJs()

        Dim sbJS As New StringBuilder

        sbJS.Append("<script language=""javascript"" type=""text/javascript"" >" & Environment.NewLine)
        sbJS.Append(JSLibrary.ToggleDiv())
        sbJS.Append("</script>" & Environment.NewLine)

        ltr_js.Text &= Environment.NewLine & sbJS.ToString()

    End Sub

    Protected Sub Util_CheckAccess()

        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If

        If Not m_refContentApi.IsARoleMember(Common.EkEnumeration.CmsRoleIds.CommerceAdmin) Then
            Throw New Exception(GetMessage("err not role commerce-admin"))
        End If

    End Sub

    Protected Function Util_AddSite(ByVal orderSite As String) As String

        If Not SiteList.Contains(orderSite) Then SiteList.Add(orderSite)
        Return orderSite

    End Function
    Protected Sub Util_ShowSites()

        Dim literalReference As Literal = Nothing
        Dim header As System.Web.UI.Control = dg_cctypes.Controls(0).Controls(0)

        If header.FindControl("ltr_sites") IsNot Nothing Then literalReference = header.FindControl("ltr_sites")

        For index As Integer = 0 To (SiteList.Count - 1)
            literalReference.Text &= "<tr><td><input type=""checkbox"" checked=""checked"" id=""chk_site_" & index.ToString() & """ name=""chk_site_" & index.ToString() & """ value=""" & Server.HtmlEncode(SiteList(index)) & """ />" & SiteList(index) & "</td></tr>"
        Next

    End Sub

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

        sRet &= "<a href=""../customers.aspx?action=view&id=" & Customer.Id & """>" & Customer.FirstName & " " & Customer.LastName & " (" & Customer.DisplayName & ")</a>"
        sRet &= "<br/>Orders: " & Customer.TotalOrders
        sRet &= "<br/>Value:  " & defaultCurrency.ISOCurrencySymbol & EkFunctions.FormatCurrency(Customer.TotalOrderValue, defaultCurrency.CultureCode)
        sRet &= "<br/>Avg Value:  " & defaultCurrency.ISOCurrencySymbol & EkFunctions.FormatCurrency(Customer.AverageOrderValue, defaultCurrency.CultureCode)

        Return sRet
    End Function

    Public Function Util_ShowCustomerType(ByVal CustomerType As Ektron.Cms.Common.EkEnumeration.CustomerType) As String
        Dim sRet As String = ""

        Select Case CustomerType
            Case EkEnumeration.CustomerType.[New]
                sRet = "<img alt=""" & m_refMsg.GetMessage("alt new customer") & """ src=""" & AppImgPath & "commerce/newcust.gif"" >" & m_refMsg.GetMessage("lbl new") & ""
            Case EkEnumeration.CustomerType.Returning
                sRet = "<img alt=""" & m_refMsg.GetMessage("alt returning customer") & """ height=""16px"" width=""16px"" src=""" & AppImgPath & "commerce/cust.gif"" >" & m_refMsg.GetMessage("lbl returning") & ""
        End Select

        Return sRet
    End Function
    Private Sub Util_RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCss")
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS")
    End Sub
#End Region

End Class