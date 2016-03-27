Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Partial Class viewgroups
    Inherits System.Web.UI.UserControl

    Protected language_data As LanguageData()
    Protected user_data As UserGroupData
    Protected security_data As PermissionData
    Protected m_refSiteApi As New SiteAPI
    Protected m_refUserApi As New UserAPI
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected domain_data As DomainData()
    Protected UserName As String = ""
    Protected ContentLanguage As Integer = -1
    Protected FirstName As String = ""
    Protected LastName As String = ""
    Protected Domain As String = ""
    Protected setting_data As SettingsData
    Protected usergroup_data As UserGroupData()
    Protected m_refEmail As New EmailHelper
    Protected search As String = ""
    Protected strFilter As String = ""
    Protected rp As String = ""
    Protected e1 As String = ""
    Protected e2 As String = ""
    Protected f As String = ""
    Protected m_intGroupType As Integer = 0
    Protected m_intGroupId As Long = -1
    Private _LoadedInIframe As Boolean = False
    Private _SubscriptionGroupType As String
    Private _ActiveDirectory As Boolean = False
    Protected _pagingInfo As New Ektron.Cms.PagingInfo

    'Note: this is used by Commerce >> Items Tab >> Subscriptions >> Membership
    Private Property LoadedInIframe() As Boolean
        Get
            Return _LoadedInIframe
        End Get
        Set(ByVal value As Boolean)
            _LoadedInIframe = value
        End Set
    End Property

    Private Property SubscriptionGroupType() As String
        Get
            Return _SubscriptionGroupType
        End Get
        Set(ByVal value As String)
            _SubscriptionGroupType = value
        End Set
    End Property

    Public Property ActiveDirectory() As Boolean
        Get
            Return _ActiveDirectory
        End Get
        Set(ByVal value As Boolean)
            _ActiveDirectory = value
        End Set
    End Property

#Region "Load"
    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        If ((Not IsNothing(Request.QueryString("grouptype"))) AndAlso (Request.QueryString("grouptype") <> "")) Then
            If Request.QueryString("RequestedBy") <> "" Then
                Me.LoadedInIframe = IIf((Request.QueryString("RequestedBy") = "EktronCommerceItemsSusbscriptionsMembership"), True, False)
                Me._SubscriptionGroupType = IIf((Request.QueryString("grouptype") = "0"), "cms", "membership")
            End If
        End If
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RegisterResources()
        If ((Not IsNothing(Request.QueryString("grouptype"))) AndAlso (Request.QueryString("grouptype") <> "")) Then
            m_intGroupType = Convert.ToInt32(Request.QueryString("grouptype"))
        End If
        If ((Not IsNothing(Request.QueryString("groupid"))) AndAlso (Request.QueryString("groupid") <> "")) Then
            m_intGroupId = Convert.ToInt64(Request.QueryString("groupid"))
        ElseIf ((Not IsNothing(Request.QueryString("id"))) AndAlso (Request.QueryString("id") <> "")) Then
            m_intGroupId = Convert.ToInt64(Request.QueryString("id"))
        End If
        Utilities.SetLanguage(m_refSiteApi)
        m_refMsg = m_refSiteApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        ContentLanguage = m_refSiteApi.ContentLanguage
        _pagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        If Page.IsPostBack Then
            Dim iCurrentPage As Integer = 1
            If Integer.TryParse(CurrentPage.Text, iCurrentPage) Then
                _pagingInfo.CurrentPage = iCurrentPage
            End If
        Else
            VisiblePageControls(False)
            If Not _ActiveDirectory Then ViewUserGroups()
        End If
    End Sub
#End Region

#Region "ViewUserGroup"
    Public Function ViewUserGroups() As Boolean
        Dim OrderBy As String = IIf(String.IsNullOrEmpty(Request.QueryString("OrderBy")), "GroupName", Request.QueryString("OrderBy"))
        Dim groupType As Ektron.Cms.Common.EkEnumeration.UserTypes = IIf(m_intGroupType = 0, EkEnumeration.UserTypes.AuthorType, EkEnumeration.UserTypes.MemberShipType)
        Dim apiUser As New Ektron.Cms.API.User.User()

        usergroup_data = apiUser.GetAllUserGroups(groupType, OrderBy, _pagingInfo)

        'TOOLBAR
        ViewUserGroupsToolBar()
        Populate_ViewUserGroups()
        PageSettings()

    End Function
    Private Sub Populate_ViewUserGroups()
        Dim Icon As String = "users.png"
        If (m_intGroupType = 1) Then
            Icon = "usersMembership.png"
        End If
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "GROUPNAME"
        colBound.HeaderText = m_refMsg.GetMessage("generic User Group Name")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(65)
        colBound.ItemStyle.Width = Unit.Percentage(65)
        MapCMSGroupToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "GROUPID"
        colBound.HeaderText = m_refMsg.GetMessage("lbl Group ID") 'm_refMsg.GetMessage("generic User Group Name")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(5)
        colBound.ItemStyle.Width = Unit.Percentage(5)
        MapCMSGroupToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "COUNT"
        colBound.HeaderText = m_refMsg.GetMessage("generic Number of Users")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.CssClass = "title-header"
        MapCMSGroupToADGrid.Columns.Add(colBound)
        If (m_intGroupType = 0 AndAlso Me.SubscriptionGroupType = "") Then
            If (m_refEmail.IsLoggedInUsersEmailValid) Then
                colBound = New System.Web.UI.WebControls.BoundColumn
                colBound.DataField = "EMAIL"
                colBound.HeaderText = "<a href=""#"" onclick=""ToggleEmailCheckboxes();"" title=""" & m_refMsg.GetMessage("alt send email to all") & """><input type=""checkbox""></a>&nbsp;" & m_refMsg.GetMessage("generic all")
                colBound.ItemStyle.Wrap = False
                colBound.HeaderStyle.CssClass = "title-header"
                colBound.HeaderStyle.Width = Unit.Percentage(10)
                colBound.ItemStyle.Width = Unit.Percentage(10)
                MapCMSGroupToADGrid.Columns.Add(colBound)
            End If
        End If
        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("GROUPNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("GROUPID", GetType(String)))
        dt.Columns.Add(New DataColumn("COUNT", GetType(String)))
        If (m_intGroupType = 0) Then
            If (m_refEmail.IsLoggedInUsersEmailValid) Then
                dt.Columns.Add(New DataColumn("EMAIL", GetType(String)))
            End If
        End If
        If (Not (IsNothing(usergroup_data))) Then
            For i As Integer = 0 To usergroup_data.Length - 1
                dr = dt.NewRow
                If Me.LoadedInIframe = True Then
                    'This is required for Commcerce >> Items Tab >> Subscriptions >> Membership.ascx
                    dr("GROUPNAME") = "<a href=""#AddGroup"" onclick=""parent.Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.add('" & Me.SubscriptionGroupType & "', '" & usergroup_data(i).GroupId & "', '" & usergroup_data(i).GroupDisplayName.Replace("'", "`") & "');return false;""><img src=""" & AppImgPath & "../UI/Icons/" & Icon & """ align=""absbottom"" title='" & m_refMsg.GetMessage("view user group msg") & " """ & usergroup_data(i).GroupDisplayName.Replace("'", "`") & """' alt='" & m_refMsg.GetMessage("view user group msg") & " """ & usergroup_data(i).GroupDisplayName.Replace("'", "`") & """'></a>&nbsp;<a href=""#AddGroup"" onclick=""parent.Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.add('" & Me.SubscriptionGroupType & "', '" & usergroup_data(i).GroupId & "', '" & usergroup_data(i).GroupDisplayName.Replace("'", "`") & "');return false;"">" & usergroup_data(i).GroupDisplayName.Replace("'", "`") & "</a>"
                Else
                    dr("GROUPNAME") = "<a href=""users.aspx?action=viewallusers&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&groupid=" & usergroup_data(i).GroupId & "&id=" & usergroup_data(i).GroupId & """ title='" & m_refMsg.GetMessage("view user group msg") & " """ & usergroup_data(i).GroupDisplayName.Replace("'", "`") & """'><img src=""" & AppImgPath & "../UI/Icons/" & Icon & """ align=""absbottom"" title='" & m_refMsg.GetMessage("view user group msg") & " """ & usergroup_data(i).GroupDisplayName.Replace("'", "`") & """' alt='" & m_refMsg.GetMessage("view user group msg") & " """ & usergroup_data(i).GroupDisplayName.Replace("'", "`") & """'></a>&nbsp;<a href=""users.aspx?action=viewallusers&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&groupid=" & usergroup_data(i).GroupId & "&id=" & usergroup_data(i).GroupId & """ title='" & m_refMsg.GetMessage("view user group msg") & " """ & usergroup_data(i).GroupDisplayName.Replace("'", "`") & """'>" & usergroup_data(i).GroupDisplayName.Replace("'", "`") & "</a>"
                End If

                dr("GROUPID") = usergroup_data(i).GroupId
                dr("COUNT") = usergroup_data(i).UserCount
                If (m_intGroupType = 0 AndAlso Me.SubscriptionGroupType = "") Then
                    If (m_refEmail.IsLoggedInUsersEmailValid()) Then

                        dr("EMAIL") = "<input type=""checkbox"" name=""emailcheckbox_" & usergroup_data(i).GroupId & """ ID=""Checkbox1"">"
                        dr("EMAIL") += "<a href=""#"" onclick=""SelectEmail('emailcheckbox_" & usergroup_data(i).GroupId & "');return false"">"
                        dr("EMAIL") += m_refEmail.MakeEmailGraphic() & "</a>"
                    End If
                End If
                dt.Rows.Add(dr)
            Next
        End If
        Dim dv As New DataView(dt)
        MapCMSGroupToADGrid.DataSource = dv
        MapCMSGroupToADGrid.DataBind()
    End Sub
    Private Sub ViewUserGroupsToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view user groups msg"))
        result.Append("<table><tr>")
        If (m_intGroupType = 0) Then
            If Me.SubscriptionGroupType = "" Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/add.png", "users.aspx?action=addusergroup&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "", m_refMsg.GetMessage("alt add button text (user group2)"), m_refMsg.GetMessage("btn add user group"), ""))
            End If

            If (m_refEmail.IsLoggedInUsersEmailValid() AndAlso Me.SubscriptionGroupType = "") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/email.png", "#", m_refMsg.GetMessage("alt send email to selected groups"), m_refMsg.GetMessage("btn email"), "onclick=""LoadEmailChildPageEx();"""))
            End If
        Else
            If Me.SubscriptionGroupType = "" Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/add.png", "users.aspx?action=addusergroup&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "", m_refMsg.GetMessage("alt add button text (user group2)"), m_refMsg.GetMessage("btn add membership usergroup"), ""))
            End If
        End If
        result.Append("<td>")
        If Me.SubscriptionGroupType = "" Then
            If (m_intGroupType = 0) Then
                result.Append(m_refStyle.GetHelpButton("ViewUserGroupsToolBar"))
            Else
                result.Append(m_refStyle.GetHelpButton("ViewMembershipGroups"))
            End If
        End If
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
#End Region

#Region "Grid Events"
    Public Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                _pagingInfo.CurrentPage = 1
            Case "Last"
                _pagingInfo.CurrentPage = Int32.Parse(TotalPages.Text)
            Case "Next"
                _pagingInfo.CurrentPage = Int32.Parse(CurrentPage.Text) + 1
            Case "Prev"
                _pagingInfo.CurrentPage = Int32.Parse(CurrentPage.Text) - 1
        End Select
        ViewUserGroups()
        PageSettings()
    End Sub

    Sub LinkBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub
#End Region

    Private Sub VisiblePageControls(ByVal flag As Boolean)
        TotalPages.Visible = flag
        CurrentPage.Visible = flag
        PreviousPage.Visible = flag
        NextPage.Visible = flag
        LastPage.Visible = flag
        FirstPage.Visible = flag
        PageLabel.Visible = flag
        OfLabel.Visible = flag
    End Sub

    Private Sub PageSettings()
        If (_pagingInfo.TotalPages <= 1) Then
            VisiblePageControls(False)
        Else
            VisiblePageControls(True)
            TotalPages.Text = (System.Math.Ceiling(_pagingInfo.TotalPages)).ToString()
            CurrentPage.Text = _pagingInfo.CurrentPage.ToString()
            PreviousPage.Enabled = True
            FirstPage.Enabled = True
            NextPage.Enabled = True
            LastPage.Enabled = True
            If _pagingInfo.CurrentPage = 1 Then
                PreviousPage.Enabled = False
                FirstPage.Enabled = False
            ElseIf _pagingInfo.CurrentPage = _pagingInfo.TotalPages Then
                NextPage.Enabled = False
                LastPage.Enabled = False
            End If
        End If
    End Sub

#Region "MapCMSUserGroupToAD"
    Public Function MapCMSUserGroupToAD() As Boolean
        search = Request.QueryString("search")
        If (Not (Page.IsPostBack) Or (Page.IsPostBack And Request.Form("domainname") <> "")) Then
            VisiblePageControls(False)
            Display_MapCMSUserGroupToAD()
        Else
            Process_MapCMSUserGroupToAD()
            Return (True)
        End If
    End Function
    Private Sub Process_MapCMSUserGroupToAD()
        Dim tempArray As Object = Split(Request.Form("usernameanddomain"), "_@_")
        Dim strUserName As String = CStr(tempArray(0))
        Dim strDomain As String = CStr(tempArray(1))
        m_refUserApi.MapCMSUserGroupToAD(m_intGroupId, strUserName, strDomain)
        Dim returnPage As String = ""
        If (Request.Form("rp") = "1") Then
            returnPage = "users.aspx?action=viewallgroups&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType
        Else
            returnPage = "adreports.aspx?action=SynchUsers&ReportType=" & Request.Form("rt") & "&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType
        End If
        Response.Redirect(returnPage, False)
    End Sub
    Private Sub Display_MapCMSUserGroupToAD()
        AppImgPath = m_refSiteApi.AppImgPath
        rp = Request.QueryString("rp")
        e1 = Request.QueryString("e1")
        e2 = Request.QueryString("e2")
        f = Request.QueryString("f")
        If (rp = "") Then
            rp = Request.Form("rp")
        End If

        If (e1 = "") Then
            e1 = Request.Form("e1")
        End If

        If (e2 = "") Then
            e2 = Request.Form("e2")
        End If

        If (f = "") Then
            f = Request.Form("f")
        End If
        language_data = m_refSiteApi.GetAllActiveLanguages()
        user_data = m_refUserApi.GetUserGroupById(m_intGroupId)

        If ((m_intGroupId = 1) And (rp = 3)) Then
            domain_data = m_refUserApi.GetDomains(1, 0)
        Else
            domain_data = m_refUserApi.GetDomains(0, 0)
        End If

        security_data = m_refContentApi.LoadPermissions(0, "content")
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)

        If ((search = "1") Or (search = "2")) Then

            If (search = "1") Then
                strFilter = Request.Form("groupname")
                Domain = Request.Form("domainname")
            Else
                strFilter = Request.QueryString("groupname")
                Domain = Request.QueryString("domainname")
            End If
            If (Domain = "All Domains") Then
                Domain = ""
            End If
            Dim adgroup_data As GroupData()
            adgroup_data = m_refUserApi.GetAvailableADGroups(strFilter, Domain)
            'TOOLBAR
            Dim result As New System.Text.StringBuilder
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("search ad for cms group") & " """ & user_data.GroupDisplayName & """")
            result.Append("<table><tr>")
            If (Not (IsNothing(adgroup_data))) Then
                If (rp = "1") Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (associate group)"), m_refMsg.GetMessage("btn update"), "onclick=""return SubmitForm('aduserinfo', 'CheckRadio(1);');"""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (associate group)"), m_refMsg.GetMessage("btn update"), "onclick=""return SubmitForm('aduserinfo', 'CheckReturn(1);');"""))
                End If
            End If
            If (Request.ServerVariables("HTTP_USER_AGENT").ToString.IndexOf("MSIE") > -1) Then 'defect 16045
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:window.location.reload(false);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            End If
            If (rp <> "1") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("btn cancel"), "onclick=""top.close();"""))
            End If
            result.Append("<td>")
            result.Append(m_refStyle.GetHelpButton("Display_MapCMSUserGroupToAD"))
            result.Append("</td>")
            result.Append("</tr></table>")
            htmToolBar.InnerHtml = result.ToString
            'Dim i As Integer = 0
            'If (Not (IsNothing(domain_data))) Then
            '    domainname.Items.Add(New ListItem(m_refMsg.GetMessage("all domain select caption"), ""))
            '    domainname.Items(0).Selected = True
            '    For i = 0 To domain_data.Length - 1
            '        domainname.Items.Add(New ListItem(domain_data(i).Name, domain_data(i).Name))
            '    Next
            'End If
            Populate_MapCMSGroupToADGrid(adgroup_data)
        Else

            PostBackPage.Text = Utilities.SetPostBackPage("users.aspx?Action=MapCMSUserGroupToAD&Search=1&LangType=" & ContentLanguage & "&rp=" & rp & "&e1=" & e1 & "&e2=" & e2 & "&f=" & f & "&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId)
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("search ad for cms group") & " """ & user_data.DisplayUserName & """")
            Dim result As New System.Text.StringBuilder
            result.Append("<table><tr>")
            If (rp <> "1") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("btn cancel"), "onclick=""top.close();"""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            End If
            result.Append("<td>")
            result.Append(m_refStyle.GetHelpButton("Display_MapCMSUserGroupToAD"))
            result.Append("</td>")
            result.Append("</tr></table>")
            htmToolBar.InnerHtml = result.ToString
            Populate_MapCMSUserGroupToADGrid_Search(domain_data)
        End If
    End Sub
    Private Sub Populate_MapCMSUserGroupToADGrid_Search(ByVal data As DomainData())
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "GROUPTITLE"
        colBound.HeaderText = m_refMsg.GetMessage("active directory group title")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(40)
        colBound.ItemStyle.Width = Unit.Percentage(40)
        MapCMSGroupToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DOMAINTITLE"
        colBound.HeaderText = m_refMsg.GetMessage("domain title")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderStyle.Width = Unit.Percentage(40)
        colBound.ItemStyle.Width = Unit.Percentage(40)
        MapCMSGroupToADGrid.Columns.Add(colBound)
        Dim result As New System.Text.StringBuilder
        result.Append("<input type=""hidden"" name=""id"" value=""" & user_data.UserId & """>")
        result.Append("<input type=""hidden"" name=""rp"" value=""" & rp & """>")
        result.Append("<input type=""hidden"" name=""e1"" value=""" & e1 & """>")
        result.Append("<input type=""hidden"" name=""e2"" value=""" & e2 & """>")
        result.Append("<input type=""hidden"" name=""f"" value=""" & f & """>")
        result.Append("<input type=""hidden"" name=""adusername"">")
        result.Append("<input type=""hidden"" name=""addomain"">")


        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("GROUPTITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("DOMAINTITLE", GetType(String)))
        dr = dt.NewRow
        dr(0) = "<input type=""Text"" name=""groupname"" maxlength=""255"" size=""25"" OnKeyPress=""javascript:return CheckKeyValue(event,'34');"">"
        dr(0) += result.ToString()
        Dim i As Integer = 0
        dr(1) = "<select name=""domainname"">"
        If (Not (IsNothing(domain_data))) AndAlso m_refContentApi.RequestInformationRef.ADAdvancedConfig = False Then
            dr(1) += "<option selected value=""All Domains"">" & m_refMsg.GetMessage("all domain select caption") & "</option>"
        End If
        For i = 0 To domain_data.Length - 1
            dr(1) += "<option value=""" & domain_data(i).Name & """>" & domain_data(i).Name & "</option>"
        Next
        dr(1) += "</select>"

        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = "<input type=""submit"" name=""search"" value=""" & m_refMsg.GetMessage("generic Search") & """>"
        dr(1) = ""

        dt.Rows.Add(dr)


        Dim dv As New DataView(dt)
        MapCMSGroupToADGrid.DataSource = dv
        MapCMSGroupToADGrid.DataBind()
    End Sub
    Private Sub Populate_MapCMSGroupToADGrid(ByVal data As GroupData())
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = m_refMsg.GetMessage("add title")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(5)
        colBound.HeaderStyle.Width = Unit.Percentage(5)
        MapCMSGroupToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "GROUPTITLE"
        colBound.HeaderText = m_refMsg.GetMessage("active directory group title")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderStyle.Width = Unit.Percentage(15)
        colBound.HeaderStyle.Width = Unit.Percentage(15)
        MapCMSGroupToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DOMAINTITLE"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderText = m_refMsg.GetMessage("domain title")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(40)
        colBound.HeaderStyle.Width = Unit.Percentage(40)
        MapCMSGroupToADGrid.Columns.Add(colBound)


        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("GROUPTITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("DOMAINTITLE", GetType(String)))
        Dim i As Integer = 0
        If (Not (IsNothing(data))) Then
            For i = 0 To data.Length - 1
                dr = dt.NewRow
                dr(0) = "<input type=""Radio"" name=""usernameanddomain"" value=""" & data(i).GroupName & "_@_" & data(i).GroupDomain & """ onClick=""SetUp('" & data(i).GroupName & "_@_" & data(i).GroupDomain & "')"">"
                dr(1) = data(i).GroupName
                dr(2) = data(i).GroupDomain
                dt.Rows.Add(dr)
            Next
        Else
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("no ad groups found")
            dr(1) = ""
            dt.Rows.Add(dr)
        End If
        Dim result As New System.Text.StringBuilder
        result.Append("<input type=""hidden"" name=""id"" value=""" & user_data.UserId & """>")
        result.Append("<input type=""hidden"" name=""rp"" value=""" & rp & """>")
        result.Append("<input type=""hidden"" name=""e1"" value=""" & e1 & """>")
        result.Append("<input type=""hidden"" name=""e2"" value=""" & e2 & """>")
        result.Append("<input type=""hidden"" name=""f"" value=""" & f & """>")
        result.Append("<input type=""hidden"" name=""adusername"">")
        result.Append("<input type=""hidden"" name=""addomain"">")
        dr = dt.NewRow
        dr(0) = result.ToString
        dr(1) = ""
        dt.Rows.Add(dr)

        Dim dv As New DataView(dt)
        MapCMSGroupToADGrid.DataSource = dv
        MapCMSGroupToADGrid.DataBind()
    End Sub
#End Region
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub

End Class
