Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Workarea
Imports System.DateTime
Imports System.Collections.Generic
Imports System.data

Partial Class Commerce_bycustomer
    Inherits workareabase

    Protected m_intGroupId As Integer = -1
    Protected m_strKeyWords As String = ""
    Protected m_strSelectedItem As String = "-1"
    Private m_bCommunityGroup As Boolean
    Private m_iCommunityGroup As Integer = 0
    Protected CustomerManager As CustomerApi = Nothing

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        Utils_RegisterResources()
        Utilities.ValidateUserLogin()
        If (m_refContentApi.RequestInformationRef.IsMembershipUser) Then
            Response.Redirect(m_refContentApi.ApplicationPath & "reterror.aspx?info=" & m_refContentApi.EkMsgRef.GetMessage("msg login cms user"), True)
            Exit Sub
        End If
        ltr_noCustSelected.Text = GetMessage("js alert no cust selected")
        MapCMSUserToADGrid.DataSource = ""
        If ((Not IsNothing(Request.QueryString("groupid"))) AndAlso (Request.QueryString("groupid") <> "")) Then
            m_intGroupId = Convert.ToInt32(Request.QueryString("groupid"))
            If m_bCommunityGroup Then
                m_iCommunityGroup = m_intGroupId
                m_intGroupId = Me.m_refContentApi.EkContentRef.GetCmsGroupForCommunityGroup(m_iCommunityGroup)
            End If
        End If
        If Page.IsPostBack Then
            ViewAllUsers()
        End If
        ViewAllUsersToolBar()
    End Sub

    Public Sub ViewAllUsers()
        m_strKeyWords = Request.Form("txtSearch")
        If (Page.IsPostBack And Request.Form(isPostData.UniqueID) <> "") Then
            If (Request.Form(isSearchPostData.UniqueID) <> "") Then
                DisplayUsers()
            End If
        ElseIf (IsPostBack = False) Then
            ViewAllUsersToolBar()
        End If
        isPostData.Value = "true"
    End Sub
    Private Sub ViewAllUsersToolBar()
        Dim result As New System.Text.StringBuilder
        result.Append("<table width=""100%""><tr class=""ektronToolbar"">")
        result.Append("<td class=""label"">" & m_refMsg.GetMessage("lbl text") & "</td><td><input type=""text"" class=""minWidth"" size=""25"" id=""txtSearch"" name=""txtSearch"" value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event)""/></td>")
        result.Append("<td class=""label"">" & m_refMsg.GetMessage("lbl field") & ":</td><td><select id=""searchlist"" name=""searchlist"">")
        'result.Append("<option value=""-1" & IsSelected("-1") & """>All</option>")
        result.Append("<option value=""user_name""" & IsSelected("user_name") & ">" & m_refMsg.GetMessage("lbl customer username") & "</option>")
        result.Append("<option value=""first_name""" & IsSelected("first_name") & ">" & m_refMsg.GetMessage("lbl first name") & "</option>")
        result.Append("<option value=""last_name""" & IsSelected("last_name") & ">" & m_refMsg.GetMessage("lbl last name") & "</option>")
        result.Append("</select></td><td><a class=""button buttonInline buttonFilter blueHover btnFilter"" type=""button"" name=""btnSearch"" id=""btnSearch"" Value=""" & m_refMsg.GetMessage("btn filter") & """ onClick=""searchuser();"" style=""font-size: 1em"">" & m_refMsg.GetMessage("btn filter") & "</a>")
        result.Append("<td><a type=""button"" class=""button buttonInline buttonOk greenHover btnOk"" name=""btnOK"" id=""btnOK"" Value=""" & m_refMsg.GetMessage("lbl ok") & """ onClick=""getcheckedid();"">" & m_refMsg.GetMessage("lbl ok") & "</a></td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub
    Private Function IsSelected(ByVal val As String) As String
        If (val = m_strSelectedItem) Then
            Return (" selected ")
        Else
            Return ("")
        End If
    End Function
    Private Sub DisplayUsers()
        Dim customerList As New List(Of CustomerData)
        Dim CustomerCriteria As New Ektron.Cms.Common.Criteria(Of CustomerProperty)(CustomerProperty.UserName, OrderByDirection.Ascending)

        'CustomerCriteria.AddFilter(CustomerProperty.TotalOrders, CriteriaFilterOperator.GreaterThan, 0)
        'CustomerCriteria.AddFilter(CustomerProperty.TotalOrderValue, CriteriaFilterOperator.GreaterThan, 0)

        CustomerManager = New CustomerApi()

        m_strKeyWords = Request.Form("txtSearch")

        m_strSelectedItem = Request.Form("searchlist")

        Select Case m_strSelectedItem

            'Case "-1 selected " ' All

            '    CustomerCriteria.AddFilter(CustomerProperty.FirstName, CriteriaFilterOperator.Contains, m_strKeyWords)

            '    CustomerCriteria.AddFilter(CustomerProperty.LastName, CriteriaFilterOperator.Contains, m_strKeyWords)

            '    CustomerCriteria.AddFilter(CustomerProperty.UserName, CriteriaFilterOperator.Contains, m_strKeyWords)

            'Case "-1" ' All

            '    CustomerCriteria.AddFilter(CustomerProperty.FirstName, CriteriaFilterOperator.Contains, m_strKeyWords)

            '    CustomerCriteria.AddFilter(CustomerProperty.LastName, CriteriaFilterOperator.Contains, m_strKeyWords)

            '    CustomerCriteria.AddFilter(CustomerProperty.UserName, CriteriaFilterOperator.Contains, m_strKeyWords)

            Case "last_name" ' Last Name

                CustomerCriteria.AddFilter(CustomerProperty.LastName, CriteriaFilterOperator.Contains, m_strKeyWords)

            Case "first_name" ' First Name

                CustomerCriteria.AddFilter(CustomerProperty.FirstName, CriteriaFilterOperator.Contains, m_strKeyWords)

            Case "user_name" ' User Name

                CustomerCriteria.AddFilter(CustomerProperty.UserName, CriteriaFilterOperator.Contains, m_strKeyWords)

        End Select

        customerList = CustomerManager.GetList(CustomerCriteria)

        ViewAllUsersToolBar()
        literal1.Text = ""
        If customerList IsNot Nothing Then
            If customerList.Count <> 0 Then
                If customerList.Count > 0 Then
                    dg_customers.DataSource = customerList
                    dg_customers.DataBind()
                Else
                    literal1.Text = "<br/><label style=""color:#2E6E9E;"" id=""lbl_noUsers"">" & Me.GetMessage("lbl no users") & "</label>"
                End If
            Else
                literal1.Text = "<br/><label style=""color:#2E6E9E;"" id=""lbl_noUsers"">" & Me.GetMessage("lbl no users") & "</label>"
            End If
        Else
            literal1.Text = "<br/><label style=""color:#2E6E9E;"" id=""lbl_noUsers"">" & Me.GetMessage("lbl no users") & "</label>"
        End If
        dg_customers.DataSource = customerList
        dg_customers.DataBind()
    End Sub
    Protected Sub Utils_RegisterResources()

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, m_refContentApi.AppPath & "csslib/box.css", "EktronBoxCSS")

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS)
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncsJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)

    End Sub
End Class
