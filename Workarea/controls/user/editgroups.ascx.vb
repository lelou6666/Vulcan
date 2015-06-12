Imports System.Data
Imports Ektron.Cms
Partial Class editgroups
    Inherits System.Web.UI.UserControl

    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected security_data As PermissionData
    Protected language_data As LanguageData()
    Protected m_refSiteApi As New SiteAPI
    Protected m_refUserApi As New UserAPI
    Protected m_refContentApi As New ContentAPI
    Protected setting_data As SettingsData
    Protected domain_list As DomainData()
    Protected search As String = ""
    Protected m_strUserName As String = ""
    Protected ContentLanguage As Integer = -1
    Protected m_strFirstName As String = ""
    Protected m_strLastName As String = ""
    Protected m_strDomain As String = ""
    Protected uId As Long = -1
    Protected user_list As UserData()
    Protected pagedata As Collection
    Protected m_strFilter As String = ""
    Protected group_list As GroupData()
    Protected group_data As UserGroupData
    Protected OrderBy As String = ""
    Protected PageAction As String = ""
    Protected m_intGroupType As Integer = 0

    Protected m_strDirection As String = "asc"
    Protected m_strSearchText As String = ""
    Protected m_strKeyWords As String = ""
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected m_strSelectedItem As String = "-1"
    Protected m_strPageAction As String = ""
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If (Not (Request.QueryString("grouptype") Is Nothing) AndAlso Request.QueryString("grouptype") <> "") Then
            m_intGroupType = Request.QueryString("grouptype")
        End If
        If ((Not (IsNothing(Request.QueryString("action")))) AndAlso (Request.QueryString("action") <> "")) Then
            m_strPageAction = Request.QueryString("action").ToLower
        End If
        If ((Not (IsNothing(Request.QueryString("id")))) AndAlso (Request.QueryString("id") <> "")) Then
            uId = Request.QueryString("id")
        End If

        m_refMsg = m_refContentApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        ContentLanguage = m_refSiteApi.ContentLanguage
        m_strDirection = Request.QueryString("direction")
        If (m_strDirection = "asc") Then
            m_strDirection = "desc"
        Else
            m_strDirection = "asc"
        End If
        VisiblePageControls(False)
        If (m_strPageAction = "addusertogroup") Then
            AddUserToGroup()
        End If
        RegisterResources()
    End Sub

    Private Function LDAPMembers() As Boolean
        If (m_intGroupType = 1) Then 'member
            Return (m_refUserApi.RequestInformationRef.LDAPMembershipUser)
        ElseIf (m_intGroupType = 0) Then 'CMS user
            Return True
        End If
    End Function

    Public Function AddUserToGroup() As Boolean
        If (Page.IsPostBack And Request.Form(isPostData.UniqueID) <> "") Then
            If (Request.Form(isSearchPostData.UniqueID) <> "") Then
                isSearchPostData.Value = ""
                CollectSearchText()
                DisplayUsers()
            Else
                If (Request.Form(isAdded.UniqueID) <> "") Then
                    m_refUserApi.AddUserToGroup(Request.Form("selected_users").ToString, uId)
                End If
                Response.Redirect("users.aspx?action=viewallusers&grouptype=" & m_intGroupType & "&groupid=" & uId & "&id=" & uId & "&OrderBy=" & Request.QueryString("OrderBy"), False)
                'Response.Redirect("users.aspx?action=AddUserToGroup&grouptype=" & m_intGroupType & "&id=" & Request.QueryString("id") & "&OrderBy=" & Request.QueryString("OrderBy"), False)
            End If
        ElseIf (IsPostBack = False) Then
            DisplayUsers()
        End If
        isPostData.Value = "true"
    End Function
    Public Sub DisplayUsers()
        TR_AddGroupDetail.Visible = False
        If (Request.QueryString("OrderBy") = "") Then
            OrderBy = "UserName"
        Else
            OrderBy = Request.QueryString("OrderBy")
        End If
        Dim req As New GroupRequest
        req.GroupType = m_intGroupType
        req.GroupId = uId
        req.SortOrder = OrderBy
        req.SortDirection = m_strDirection
        req.SearchText = m_strSearchText
        req.PageSize = m_refContentApi.RequestInformationRef.PagingSize
        req.CurrentPage = m_intCurrentPage
        user_list = m_refUserApi.GetUsersNotInGroup(req)
        m_intTotalPages = req.TotalPages
        group_data = m_refUserApi.GetUserGroupById(uId)
        AddUserToGroupToolBar()
        Populate_AddUserToGroupGrid()
    End Sub

    Private Sub Process_AddADUserGroup()
        Dim sdGroups As New System.Collections.Specialized.NameValueCollection
        Dim lcount As Integer
        Dim strGrouppath As String = ""
        For lcount = 1 To CLng(Request.Form(addgroupcount.UniqueID))
            strGrouppath = ""
            strGrouppath = CStr(Request.Form("addgroup" & CStr(lcount)))
            If (strGrouppath <> "") Then
                sdGroups.Add(lcount, strGrouppath)
            End If
        Next
        If (m_intGroupType = 0) Then
            m_refUserApi.AddADGroupToCMS(sdGroups)
        Else
            Dim ret As Boolean = False
            Dim usr As User.EkUser
            usr = m_refUserApi.EkUserRef
            ret = usr.AddADMemberShipGroupToCmsV4(sdGroups)
        End If
        Response.Redirect("users.aspx?action=viewallgroups&grouptype=" & m_intGroupType, False)
    End Sub

    Public Sub EditUserGroup()
        TR_label.Visible = False
        If (Not (Page.IsPostBack)) Then
            TR_desc.Visible = False
            Display_EditUserGroup()
        Else
            Process_EditUserGroup()
        End If
    End Sub
    Public Sub AddUserGroup()
        TD_label.Visible = False
        PageAction = "addusergroup"
        search = Request.QueryString("search")
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
        If (Not (Page.IsPostBack)) Or (Page.IsPostBack And ((search = "1") Or (search = "2")) And setting_data.ADIntegration = True And Request.Form("domainname") <> "") Then
            If (Not (LDAPMembers())) Or (setting_data.ADIntegration = False) Then
                TR_AddGroup.Visible = False
                Display_AddUserGroup()
            Else
                domain_list = m_refUserApi.GetDomains(0, 0)
                TR_AddGroupDetail.Visible = False
                If ((search = "1") Or (search = "2")) Then
                    Display_AddUserGroup_Search()
                Else
                    Display_AddUserGroup_None()
                End If
            End If
        Else
            If ((setting_data.ADIntegration) And (search = "1" Or search = "2")) Then
                Process_AddADUserGroup()
            Else
                Process_AddUserGroup()
            End If
        End If
    End Sub
    Private Sub Display_EditUserGroup()
        uId = Request.QueryString("GroupID")
        group_data = m_refUserApi.GetUserGroupById(uId)
        EditUserGroupToolBar()
        UserGroupName.Value = Server.HtmlDecode(group_data.GroupName)
    End Sub
    Private Sub EditUserGroupToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("edit user group msg"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (user group)"), m_refMsg.GetMessage("btn update"), "onclick=""return SubmitForm('UserGroupInfo', 'VerifyGroupName()');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "users.aspx?action=viewallgroups&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&groupid=" & uId & "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("EditUserGroupToolBar"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub Process_EditUserGroup()
        pagedata = New Collection
        pagedata.Add(Request.Form(UserGroupName.UniqueID), "UserGroupName")
        pagedata.Add(Request.QueryString("groupid"), "UserGroupID")
        If (m_intGroupType = 1) Then
            Dim ret As Boolean = False
            Dim objUser As Ektron.Cms.User.EkUser
            objUser = m_refUserApi.EkUserRef
            ret = objUser.UpDateUserGroupv2_0(pagedata)
        Else
            m_refUserApi.UpDateUserGroup(pagedata)
        End If
        Response.Redirect("users.aspx?action=viewallgroups&grouptype=" & m_intGroupType & "&group8id=" & Request.QueryString("groupid"), False)
    End Sub
    Private Sub AddUserToGroupToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add user to group msg") & " """ & group_data.GroupDisplayName & """")
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", "click here to add selected users to group", m_refMsg.GetMessage("btn save"), "onclick=""AddSelectedUsers();"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("AddUserToGroupToolBar"))
        result.Append("</td>")
        result.Append("<td>&nbsp;|&nbsp;</td>")
        result.Append("<td>")
        result.Append("<label for=""txtSearch"">" & m_refMsg.GetMessage("generic search") & "</label>")
        result.Append("<input type=text class=""ektronTextMedium"" id=""txtSearch"" name=""txtSearch"" value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event)"">")
        result.Append("</td>")
        result.Append("<td>")
        result.Append("<select id=""searchlist"" name=searchlist>")
        result.Append("<option value=-1" & IsSelected("-1") & ">All</option>")
        result.Append("<option value=""last_name""" & IsSelected("last_name") & ">Last Name</option>")
        result.Append("<option value=""first_name""" & IsSelected("first_name") & ">First Name</option>")
        result.Append("<option value=""user_name""" & IsSelected("user_name") & ">User Name</option>")
        result.Append("</select><input type=button value=""Search"" class=""ektronWorkareaSearch"" id=""btnSearch"" name=""btnSearch"" onclick=""searchuser();"">")
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub Populate_AddUserToGroupGrid()
        Dim HeaderText As String = "<a href=""users.aspx?action=AddUserToGroup&OrderBy={0}&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&id=" & uId & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>{1}</a>"
        Dim Icon As String = "user.png"
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        If (m_intGroupType = 1) Then
            Icon = "userMembership.png"
        End If
        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "CHECK"
        colBound.HeaderText = "<input type=checkbox name=checkall id=checkall onclick=""checkAll('');"">"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(5)
        colBound.ItemStyle.Width = Unit.Percentage(5)
        AddGroupGrid.Columns.Add(colBound)
        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "USERNAME"
        colBound.HeaderText = String.Format(HeaderText, "user_name", m_refMsg.GetMessage("generic username"))
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        AddGroupGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LASTNAME"
        colBound.HeaderText = String.Format(HeaderText, "last_name", m_refMsg.GetMessage("generic lastname"))
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.CssClass = "title-header"
        AddGroupGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "FIRSTNAME"
        colBound.HeaderText = String.Format(HeaderText, "first_name", m_refMsg.GetMessage("generic firstname"))
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.CssClass = "title-header"
        AddGroupGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LANGUAGE"
        colBound.HeaderText = m_refMsg.GetMessage("generic Language")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.CssClass = "title-header"
        AddGroupGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("CHECK", GetType(String)))
        dt.Columns.Add(New DataColumn("USERNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("LASTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("FIRSTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        PageSettings()
        Dim i As Integer
        If (Not (IsNothing(user_list))) Then
            For i = 0 To user_list.Length - 1
                dr = dt.NewRow
                dr("CHECK") = "<input type=""checkbox"" name=""selected_users"" id=""selected_users"" value=""" & user_list(i).Id & """ onclick=""checkAll('selected_users');"">"
                dr("USERNAME") = "<img src=""" & AppImgPath & "../UI/Icons/" & Icon & """ border=""0"" align=""absbottom"">" & user_list(i).Username & "</img>"
                dr("LASTNAME") = user_list(i).LastName
                dr("FIRSTNAME") = user_list(i).FirstName
                If (user_list(i).LanguageId = 0) Then
                    dr("LANGUAGE") = m_refMsg.GetMessage("app default msg")
                Else
                    dr("LANGUAGE") = user_list(i).LanguageName
                End If
                dt.Rows.Add(dr)
            Next
        End If

        Dim dv As New DataView(dt)
        AddGroupGrid.DataSource = dv
        AddGroupGrid.DataBind()

    End Sub
    Private Sub Process_AddUserGroup()
        If (m_intGroupType = 0) Then
            m_refUserApi.AddUserGroup(Request.Form(UserGroupName.UniqueID))
        Else
            Dim objUser As Ektron.Cms.User.EkUser
            objUser = m_refSiteApi.EkUserRef
            pagedata = New Collection
            pagedata.Add(Request.Form(UserGroupName.UniqueID), "UserGroupName")
            pagedata.Add(Request.Form(group_description.UniqueID), "Description")
            objUser.AddMemberShipGroupV4(pagedata)
            pagedata = Nothing
            objUser = Nothing
        End If

        Response.Redirect("users.aspx?action=viewallgroups&grouptype=" & m_intGroupType, False)
    End Sub
#Region "AddUserGroup"
    Private Sub Display_AddUserGroup()
        language_data = m_refSiteApi.GetAllActiveLanguages()
        security_data = m_refContentApi.LoadPermissions(0, "content")
        AddUserGroupToolBar()
    End Sub
    Private Sub AddUserGroupToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add new user group msg"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (user group)"), m_refMsg.GetMessage("btn save"), "onclick=""return SubmitForm('UserGroupInfo', 'VerifyGroupName()');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "users.aspx?action=viewallgroups&grouptype=" & m_intGroupType & "&OrderBy=" & Request.QueryString("OrderBy") & "&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
		result.Append("<td>")

		If m_intGroupType = 0 Then
			result.Append(m_refStyle.GetHelpButton("AddUserGroupToolBar"))
		Else
			result.Append(m_refStyle.GetHelpButton("AddMembershipGroup"))
		End If

        result.Append("</td>")
		result.Append("</tr></table>")
		htmToolBar.InnerHtml = result.ToString
	End Sub
#End Region
#Region "AddUserGroup_Search"
    Private Sub Display_AddUserGroup_Search()
        If (search = "1") Then
            m_strFilter = Request.Form("groupname")
            m_strDomain = Request.Form("domainname")
        Else
            m_strFilter = Request.QueryString("groupname")
            m_strDomain = Request.QueryString("domainname")
        End If
        If (m_strDomain = "All Domains") Then
            m_strDomain = ""
        End If
        group_list = m_refUserApi.GetAvailableADGroups(m_strFilter, m_strDomain)
        AddUserGroupToolBar_Search()
        AddUserGroupGrid_Search()
    End Sub
    Private Sub AddUserGroupToolBar_Search()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view groups in active directory msg"))
        result.Append("<table><tr>")
        If (Not (IsNothing(group_list))) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (groups)"), m_refMsg.GetMessage("btn save"), "onclick=""return SubmitForm('aduserinfo', '');"""))
        End If
        If (Request.ServerVariables("HTTP_USER_AGENT").ToString.IndexOf("MSIE") > -1) Then 'defect 16045
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:window.location.reload(false);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("AddUserGroupToolBar_Search"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub AddUserGroupGrid_Search()
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "CHECK"
        colBound.HeaderText = "<input type=""Checkbox"" name=""checkall"" onclick=""CheckAll();"">"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(5)
        colBound.ItemStyle.Width = Unit.Percentage(5)
        AddGroupGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "GROUPTITLE"
        colBound.HeaderText = m_refMsg.GetMessage("active directory group title")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderStyle.Width = Unit.Percentage(15)
        colBound.ItemStyle.Width = Unit.Percentage(15)
        AddGroupGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DOMAINTITLE"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderText = m_refMsg.GetMessage("domain title")
        colBound.ItemStyle.Wrap = False
        AddGroupGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("CHECK", GetType(String)))
        dt.Columns.Add(New DataColumn("GROUPTITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("DOMAINTITLE", GetType(String)))

        Dim i As Integer = 0
        If (Not (IsNothing(group_list))) Then
            For i = 0 To group_list.Length - 1
                dr = dt.NewRow
                dr(0) = "<input type=""CHECKBOX"" name=""addgroup" & i + 1 & """ " & IIf(group_list(i).GroupId = 0, "", "disabled") & " value=""" & group_list(i).GroupPath & """>"
                dr(1) = group_list(i).GroupName
                dr(2) = group_list(i).GroupDomain
                dt.Rows.Add(dr)
            Next
        Else
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("no ad groups found")
            dr(1) = ""
            dr(2) = ""
            dt.Rows.Add(dr)
        End If
        addgroupcount.Value = i + 1
        Dim dv As New DataView(dt)
        AddGroupGrid.DataSource = dv
        AddGroupGrid.DataBind()
    End Sub
#End Region
#Region "AddUserGroup_None"
    Private Sub Display_AddUserGroup_None()
        postbackpage.Text = Utilities.SetPostBackPage("users.aspx?Action=AddUserGroup&grouptype=" & m_intGroupType & "&Search=1&LangType=" & ContentLanguage)
        AddUserGroupToolBar_None()
        AddUserGroupGrid_None()
    End Sub
    Private Sub AddUserGroupGrid_None()
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "GROUPTITLE"
        colBound.HeaderText = m_refMsg.GetMessage("active directory group title")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        AddGroupGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DOMAINTITLE"
        colBound.HeaderText = m_refMsg.GetMessage("domain title")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.CssClass = "title-header"
        AddGroupGrid.Columns.Add(colBound)


        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("GROUPTITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("DOMAINTITLE", GetType(String)))

        dr = dt.NewRow
        Dim i As Integer
        dr(0) = "<input type=""Text"" name=""groupname"" maxlength=""255"" size=""25"" onkeypress=""return CheckKeyValue(event,'34');"">"
        dr(1) = "<select name=""domainname"">"
        If (Not (IsNothing(domain_list))) Then
            If m_refContentApi.RequestInformationRef.ADAdvancedConfig = False Then
                dr(1) += "<option selected value=""All Domains"">" & m_refMsg.GetMessage("all domain select caption") & "</option>"
            End If
            For i = 0 To domain_list.Length - 1
                dr(1) += "<option value=""" & domain_list(i).Name & """>" & domain_list(i).Name & "</option>"
            Next
        End If
        dr(1) += "</select>"
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = "<input type=""submit"" name=""search"" value=""" & m_refMsg.GetMessage("generic Search") & """>"
        dr(1) = ""
        dt.Rows.Add(dr)
        Dim dv As New DataView(dt)
        AddGroupGrid.DataSource = dv
        AddGroupGrid.DataBind()
    End Sub
    Private Sub AddUserGroupToolBar_None()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("active directory group search msg"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("AddUserGroupToolBar_None"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Protected Sub Grid_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        If (PageAction = "addusergroup") Then
            Select Case e.Item.ItemType
                Case ListItemType.AlternatingItem, ListItemType.Item
                    If (e.Item.Cells(0).Text.Equals(m_refMsg.GetMessage("no ad groups found"))) Then
                        e.Item.Cells(0).ColumnSpan = 3
                        e.Item.Cells.RemoveAt(2)
                        e.Item.Cells.RemoveAt(1)
                    End If
            End Select
        End If
    End Sub
#End Region

    Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                m_intCurrentPage = 1
            Case "Last"
                m_intCurrentPage = Int32.Parse(TotalPages.Text)
            Case "Next"
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) + 1
            Case "Prev"
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) - 1
        End Select
        CollectSearchText()
        DisplayUsers()
        isPostData.Value = "true"
    End Sub
    Private Sub CollectSearchText()
        m_strKeyWords = Request.Form("txtSearch")
        m_strSelectedItem = Request.Form("searchlist")
        If (m_strSelectedItem = "-1") Then
            m_strSearchText = " (first_name like '%" & m_strKeyWords & "%' OR last_name like '%" & m_strKeyWords & "%' OR user_name like '%" & m_strKeyWords & "%')"
        ElseIf (m_strSelectedItem = "last_name") Then
            m_strSearchText = " (last_name like '%" & m_strKeyWords & "%')"
        ElseIf (m_strSelectedItem = "first_name") Then
            m_strSearchText = " (first_name like '%" & m_strKeyWords & "%')"
        ElseIf (m_strSelectedItem = "user_name") Then
            m_strSearchText = " (user_name like '%" & m_strKeyWords & "%')"
        End If
    End Sub
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
        If (m_intTotalPages <= 1) Then
            VisiblePageControls(False)
        Else
            VisiblePageControls(True)
            TotalPages.Text = (System.Math.Ceiling(m_intTotalPages)).ToString()
            CurrentPage.Text = m_intCurrentPage.ToString()
            PreviousPage.Enabled = True
            FirstPage.Enabled = True
            NextPage.Enabled = True
            LastPage.Enabled = True
            If m_intCurrentPage = 1 Then
                PreviousPage.Enabled = False
                FirstPage.Enabled = False
            ElseIf m_intCurrentPage = m_intTotalPages Then
                NextPage.Enabled = False
                LastPage.Enabled = False
            End If
        End If
    End Sub
    Private Function IsSelected(ByVal val As String) As String
        If (val = m_strSelectedItem) Then
            Return (" selected ")
        Else
            Return ("")
        End If
    End Function

    Private Sub RegisterResources()
        ' register JS
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronInputLabelJS)
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronSearchBoxInputLabelInitJS")
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        ' register CSS
    End Sub
End Class