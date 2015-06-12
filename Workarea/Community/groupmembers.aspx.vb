Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea
Imports System.Data
Imports Ektron.Cms.Common.EkEnumeration
Partial Class Community_groupmembers
    Inherits workareabase

    Protected m_cGroup As New CommunityGroupData
    Protected m_aUsers As DirectoryUserData() = Array.CreateInstance(GetType(Ektron.Cms.DirectoryUserData), 0)
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected setting_data As SettingsData
    Protected m_strKeyWords As String = ""
    Protected m_PageSize As Integer = 50
    Protected m_strSelectedItem As String = "-1"
    Protected m_bAllowAdd As Boolean = False

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.Form("txtSearch") <> "" Then
            m_strKeyWords = Request.Form("txtSearch")
        End If
        Me.groupID.Value = m_iID
        m_PageSize = Me.m_refContentApi.RequestInformationRef.PagingSize
        Try
            If CheckAccess() = False Then
                Throw New Exception(Me.GetMessage("err communitymembers no access"))
            End If
            Select Case Me.m_sPageAction
                Case "adduser"
                    If Page.IsPostBack Then
                        CollectSearchText()
                        If Request.Form("isDeleted") <> "" Then
                            Process_Add()
                        ElseIf Request.Form("isSearchPostData") <> "" Then
                            Me.isSearchPostData.Value = ""
                            Display_Add()
                        End If
                    Else
                        Display_Add()
                    End If
                Case "pending"
                    If Page.IsPostBack AndAlso Request.Form("isDeleted") <> "" Then
                        Process_Pending()
                    ElseIf Page.IsPostBack AndAlso Request.Form("isSearchPostData") <> "" Then
                        CollectSearchText()
                        Display_PendingView()
                    ElseIf Not Page.IsPostBack Then
                        Display_PendingView()
                    End If
                Case Else ' "viewall"
                    If Page.IsPostBack AndAlso Request.Form("isDeleted") <> "" Then
                        Process_Remove()
                    ElseIf Page.IsPostBack AndAlso Request.Form("isSearchPostData") <> "" Then
                        Me.isSearchPostData.Value = ""
                        CollectSearchText()
                        CurrentPage.Text = 1
                        Display_View()
                    ElseIf Not Page.IsPostBack Then
                        Display_View()
                    End If
            End Select
            BuildJS()

            If Page.IsPostBack Then
                Dim m_refSiteApi As New SiteAPI
                setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)

                Dim request As New CommunityGroupObjectRequest
                request.CurrentPage = m_intCurrentPage
                request.PageSize = IIf(m_PageSize > 0, m_PageSize, 0)
                request.GroupId = Me.m_iID
                request.SearchText = m_strKeyWords
                Select Case m_strSelectedItem
                    Case "last_name"
                        request.SearchField = "last_name"
                    Case "first_name"
                        request.SearchField = "first_name"
                    Case "user_name"
                        request.SearchField = "user_name"
                    Case Else ' "-1"
                        request.SearchField = "display_name"
                End Select
                request.ObjectId = Me.m_refContentApi.UserId
                request.ObjectType = TaxonomyItemType.User
                request.ObjectStatus = DirectoryItemStatus.All
                m_aUsers = (Me.m_refCommunityGroupApi.GetAllUnassignedCommunityGroupUsers(request))
                If m_aUsers IsNot Nothing AndAlso m_aUsers.Length > 0 Then
                    m_bAllowAdd = True
                End If
            End If
            SetLabels()
            RegisterResources()
        Catch ex As Exception
            Utilities.ShowError(ex.Message & ex.StackTrace)
        End Try

    End Sub

#Region "Display"

    Public Sub Display_Add()
        m_cGroup = Me.m_refCommunityGroupApi.GetCommunityGroupByID(Me.m_iID)

        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "java/plugins/inputLabel/ektron.inputLabel.js", "EktroInputLblJS")

        ltr_search.Text = "<br/>&#160;" & GetMessage("lbl select users") & ":<br/>"
        ltr_search.Text &= "&#160;" & "<input type=text size=25 id=""txtSearch"" name=""txtSearch"" value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event)"">"
        ltr_search.Text &= "&#160;"
        ltr_search.Text &= "<select id=""searchlist"" name=""searchlist"">"
        ltr_search.Text &= "<option value=""-1"" " & IsSelected("-1") & ">" & GetMessage("display name label") & "</option>"
        ltr_search.Text &= "<option value=""last_name""" & IsSelected("last_name") & ">" & GetMessage("lbl last name") & "</option>"
        ltr_search.Text &= "<option value=""first_name""" & IsSelected("first_name") & ">" & GetMessage("lbl first name") & "</option>"
        ltr_search.Text &= "<option value=""user_name""" & IsSelected("user_name") & ">" & GetMessage("generic username") & "</option>"
        ' ltr_search.Text &= "<option value=""all"" " & IsSelected("all") & ">" & GetMessage("generic all") & "</option>"
        ltr_search.Text &= "</select>"
        ltr_search.Text &= "&#160;"
        ltr_search.Text &= "<input type=button value=""Search"" id=""btnSearch"" name=""btnSearch"" onclick=""searchuser();"">"

        If Page.IsPostBack Then
            Dim m_refSiteApi As New SiteAPI
            setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)

            Dim request As New CommunityGroupObjectRequest
            request.CurrentPage = m_intCurrentPage
            request.PageSize = IIf(m_PageSize > 0, m_PageSize, 0)
            request.GroupId = Me.m_iID
            request.SearchText = m_strKeyWords
            Select Case m_strSelectedItem
                Case "last_name"
                    request.SearchField = "last_name"
                Case "first_name"
                    request.SearchField = "first_name"
                Case "user_name"
                    request.SearchField = "user_name"
                Case Else ' "-1"
                    request.SearchField = "display_name"
            End Select
            request.ObjectId = Me.m_refContentApi.UserId
            request.ObjectType = TaxonomyItemType.User
            request.ObjectStatus = DirectoryItemStatus.All
            m_aUsers = (Me.m_refCommunityGroupApi.GetAllUnassignedCommunityGroupUsers(request))
            If m_aUsers IsNot Nothing AndAlso m_aUsers.Length > 0 Then
                m_bAllowAdd = True
            End If
            m_intTotalPages = request.TotalPages
            Populate_ViewCommunityMembersGrid(m_aUsers)
        Else
            PageSettings() ' to suppress the paging stuff
        End If
    End Sub
    Public Sub Display_PendingView()
        Dim m_refSiteApi As New SiteAPI
        Dim totalUsers As Integer = 0
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)

        m_cGroup = Me.m_refCommunityGroupApi.GetCommunityGroupByID(Me.m_iID)
        If Page.IsPostBack Then
            m_aUsers = Me.m_refCommunityGroupApi.GetPendingCommunityGroupUsers(Me.m_iID, m_intCurrentPage, IIf(m_PageSize > 0, m_PageSize, 0), m_intTotalPages, totalUsers)
        Else
            m_aUsers = Me.m_refCommunityGroupApi.GetPendingCommunityGroupUsers(Me.m_iID, m_intCurrentPage, IIf(m_PageSize > 0, m_PageSize, 0), m_intTotalPages, totalUsers)
        End If
        Populate_ViewCommunityMembersGrid(m_aUsers)
    End Sub
    Public Sub Display_View()
        Dim m_refSiteApi As New SiteAPI
        Dim totalUsers As Integer = 0
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)

        m_cGroup = Me.m_refCommunityGroupApi.GetCommunityGroupByID(Me.m_iID)
        If Page.IsPostBack Then
            m_aUsers = Me.m_refCommunityGroupApi.GetCommunityGroupUsers(Me.m_iID, Me.m_strKeyWords, "display_name", m_intCurrentPage, IIf(m_PageSize > 0, m_PageSize, 0), m_intTotalPages, totalUsers, 0)
        Else
            m_aUsers = Me.m_refCommunityGroupApi.GetCommunityGroupUsers(Me.m_iID, m_intCurrentPage, IIf(m_PageSize > 0, m_PageSize, 0), m_intTotalPages, totalUsers, 0)
        End If
        Populate_ViewCommunityMembersGrid(m_aUsers)
    End Sub

#End Region

#Region "Process"

    Protected Sub Process_Add()
        Dim aUid As String() = Array.CreateInstance(GetType(String), 0)
        aUid = Split(Request.Form("req_deleted_users"), ",")
        If aUid IsNot Nothing AndAlso aUid.Length > 0 Then
            For i As Integer = 0 To (aUid.Length - 1)
                If IsNumeric(Trim(aUid(i))) Then
                    Me.m_refCommunityGroupApi.AddUserToCommunityGroup(Me.m_iID, aUid(i), True)
                End If
            Next
        End If
        Response.Redirect("groupmembers.aspx?action=viewallusers&LangType=" & Me.ContentLanguage & "&id=" & Me.m_iID, False) ' txtSearch
    End Sub

    Protected Sub Process_Remove()
        Dim aUid As String() = Array.CreateInstance(GetType(String), 0)
        aUid = Split(Request.Form("req_deleted_users"), ",")
        If aUid IsNot Nothing AndAlso aUid.Length > 0 Then
            For i As Integer = 0 To (aUid.Length - 1)
                If IsNumeric(Trim(aUid(i))) Then
                    Me.m_refCommunityGroupApi.RemoveUserFromCommunityGroup(Me.m_iID, aUid(i))
                End If
            Next
        End If
        Response.Redirect("groupmembers.aspx?action=viewallusers&LangType=" & Me.ContentLanguage & "&id=" & Me.m_iID, False) ' txtSearch
    End Sub

    Protected Sub Process_Pending()
        Dim stype As String = Request.Form("isDeleted").ToString().ToLower()
        Dim aUid As String() = Array.CreateInstance(GetType(String), 0)
        aUid = Split(Request.Form("req_deleted_users"), ",")
        If aUid IsNot Nothing AndAlso aUid.Length > 0 Then
            For i As Integer = 0 To (aUid.Length - 1)
                If IsNumeric(Trim(aUid(i))) Then
                    If stype = "approve" Then
                        Me.m_refCommunityGroupApi.ApprovePendingGroupUser(aUid(i), Me.m_iID)
                        Ektron.Cms.Common.Cache.ApplicationCache.Invalidate("GroupAccess_" & m_iID.ToString() & "_" & aUid(i).ToString())
                    ElseIf stype = "decline" Then
                        Me.m_refCommunityGroupApi.DeletePendingGroupUser(aUid(i), Me.m_iID)
                        Ektron.Cms.Common.Cache.ApplicationCache.Invalidate("GroupAccess_" & m_iID.ToString() & "_" & aUid(i).ToString())
                    End If
                End If
            Next
        End If
        Response.Redirect("groupmembers.aspx?action=pending&LangType=" & Me.ContentLanguage & "&id=" & Me.m_iID, False) ' txtSearch
    End Sub

#End Region

#Region "Helper Functions"

    Public Function CheckAccess() As Boolean
        Return True
    End Function

    Private Function IsSelected(ByVal val As String) As String
        If (val = m_strSelectedItem) Then
            Return (" selected ")
        Else
            Return ("")
        End If
    End Function

    Public Sub SetLabels()
        m_cGroup = Me.m_refCommunityGroupApi.GetCommunityGroupByID(Me.m_iID)
        Dim url As String = String.Empty
        Select Case Me.m_sPageAction
            Case "adduser"
                If m_bAllowAdd AndAlso (m_refContentApi.IsAdmin OrElse m_cGroup.GroupAdmin.Id = m_refContentApi.UserId OrElse m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin)) Then
                    Me.AddButtonwithMessages(m_refContentApi.AppPath & "images/ui/icons/add.png", "#", "alt add sel users to cgroup", "btn add sel users to cgroup", " onclick=""return CheckAdd();"" ")
                End If
                Me.AddBackButton("groupmembers.aspx?&id=" & Me.m_iID & "&LangType=" & Me.ContentLanguage)
                Me.AddHelpButton("addcommunitygroupmembers")
                Me.SetTitleBarToString(String.Format(GetMessage("btn add cgroup members for"), Server.HtmlEncode(Me.m_cGroup.GroupName)))

            Case "pending"
                url = "groupmembers.aspx?id=" & Me.m_iID
                anchorCurrent.Attributes.Add("onclick", "window.location.href ='" & url & "'")
                anchorPending.Attributes.Add("onclick", "")
                If m_refContentApi.IsAdmin OrElse m_cGroup.GroupAdmin.Id = m_refContentApi.UserId OrElse m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) Then
                    Me.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/approvals.png", "#", "alt approve pending users for cgroup", "btn approve pending users for cgroup", " onclick=""return CheckPendingApprove();"" ")
                    Me.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/delete.png", "#", "alt remove pending users from cgroup", "btn remove pending users from cgroup", " onclick=""return CheckPendingDelete();"" ")
                End If
                Me.AddBackButton("groupmembers.aspx?id=" & Me.m_iID & "&LangType=" & Me.ContentLanguage)
                Me.AddHelpButton("viewpendingcommunitygroupmembers")
                Me.SetTitleBarToString(String.Format(GetMessage("btn view pending cgroup members for"), Server.HtmlEncode(Me.m_cGroup.GroupName)))

            Case Else ' "viewall"
                url = "groupmembers.aspx?action=pending&id=" & Me.m_iID
                anchorPending.Attributes.Add("onclick", "window.location.href ='" & url & "'")
                anchorCurrent.Attributes.Add("onclick", "")
                If m_refContentApi.IsAdmin OrElse m_cGroup.GroupAdmin.Id = m_refContentApi.UserId OrElse m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) Then
                    Me.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/add.png", "groupmembers.aspx?action=adduser&LangType=" & Me.ContentLanguage & "&id=" & Me.m_iID, "alt add users to cgroup", "btn add users to cgroup", "")
                    Me.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/delete.png", "#", "alt remove users from cgroup", "btn remove users from cgroup", " onclick=""return CheckDelete();"" ")
                End If
               
                Me.AddBackButton("groups.aspx?action=viewgroup&id=" & Me.m_iID & "&LangType=" & Me.ContentLanguage)
                Me.AddSearchBox(Me.m_strKeyWords, New ListItemCollection, "ExecSearch")
                Me.AddHelpButton("viewcommunitygroupmembers")
                Me.SetTitleBarToString(String.Format(GetMessage("btn view cgroup members for"), Server.HtmlEncode(Me.m_cGroup.GroupName)))
        End Select
    End Sub

    Protected Sub BuildJS()
        Dim sbJS As New StringBuilder()

        jsConfirmRemoveMemberFromGroup.Text = GetMessage("js confirm remove member from cgroup")
        jsSelectAtLeastOneUser.Text = GetMessage("js err communitymembers please select user remove")
        jsPleaseSelectUserRemove.Text = GetMessage("js confirm remove pending member from cgroup")
        jsApproveSelectRequestsToJoin.Text = GetMessage("js confirm approve pending member from cgroup")
        jsPleaseSelectAtLeastOneJoinRequest.Text = GetMessage("js err pending communitymembers please select user approve")
        jsPleaseSelectUserToAdd.Text = GetMessage("js err communitymembers please select user add")
        jsCancelRequest.Text = GetMessage("js confirm remove pending member from cgroup")
    End Sub

    Private Sub Populate_ViewCommunityMembersGrid(ByVal data As DirectoryUserData())
        Dim colBound As System.Web.UI.WebControls.BoundColumn
        Dim sAppend As String = ""
        Dim Icon As String = "user.png"
        Dim m_strTyAction As String = ""

        Icon = "usersMembership.png"
        If ((Not IsNothing(Request.QueryString("ty"))) AndAlso (Request.QueryString("ty") = "nonverify")) Then
            m_strTyAction = "&ty=nonverify"
        End If
        MemberGrid.Columns.Clear()

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "CHECKL"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.Width = Unit.Percentage(5)
        colBound.HeaderText = "<input type=""checkbox"" name=""checkall"" id=""req_deleted_users"" onclick=""checkAll('');"" />"
        MemberGrid.Columns.Add(colBound)


        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LEFT"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top

        colBound.HeaderText = GetMessage("generic select all msg")
        MemberGrid.Columns.Add(colBound)

        PageSettings()

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("CHECKL", GetType(String)))
        dt.Columns.Add(New DataColumn("LEFT", GetType(String)))
        Dim i As Integer = 0

        If (Not (IsNothing(data))) Then
            ' add select all row.
            'dr = dt.NewRow
            'dr("CHECKL") = ""
            'dr("LEFT") = GetMessage("generic select all msg")
            'dt.Rows.Add(dr)

            For i = 0 To data.Length - 1
                dr = dt.NewRow
                sAppend = ""
                If (setting_data.ADAuthentication = 1) And (data(i).Domain <> "") Then
                    sAppend = "@" & data(i).Domain
                End If
                If m_cGroup.GroupAdmin.Id = data(i).Id Then
                    dr("CHECKL") = "&#160;"
                Else
                    dr("CHECKL") = "<input type=""checkbox"" name=""req_deleted_users"" id=""req_deleted_users"" value=""" & data(i).Id & """ onclick=""checkAll('req_deleted_users');"">"
                End If
                dr("LEFT") = "<img align=""left"" src=""" & IIf(data(i).Avatar <> "", data(i).Avatar, Me.m_refContentApi.AppImgPath & "user.gif") & """ width=""32"" height=""32""/><span title=""" & data(i).FirstName & " " & data(i).LastName & """>" & data(i).DisplayName & "</span>"
                dt.Rows.Add(dr)
            Next
        End If

        Dim dv As New DataView(dt)

        MemberGrid.DataSource = dv
        MemberGrid.DataBind()
    End Sub

    Private Sub PageSettings()
        If (m_intTotalPages <= 1) Then
            VisiblePageControls(False)
        Else
            VisiblePageControls(True)
            TotalPages.Text = (System.Math.Ceiling(m_intTotalPages)).ToString()
            PreviousPage1.Enabled = True
            FirstPage.Enabled = True
            NextPage.Enabled = True
            LastPage.Enabled = True
            If m_intCurrentPage = 1 Then
                PreviousPage1.Enabled = False
                FirstPage.Enabled = False
            ElseIf m_intCurrentPage = m_intTotalPages Then
                NextPage.Enabled = False
                LastPage.Enabled = False
            End If
        End If
    End Sub

    Private Sub VisiblePageControls(ByVal flag As Boolean)
        TotalPages.Visible = flag
        CurrentPage.Visible = flag
        PreviousPage1.Visible = flag
        NextPage.Visible = flag
        LastPage.Visible = flag
        FirstPage.Visible = flag
        PageLabel.Visible = flag
        OfLabel.Visible = flag


    End Sub

    Private Function Quote(ByVal KeyWords As String) As String
        Dim result As String = KeyWords
        If (KeyWords.Length > 0) Then
            result = KeyWords.Replace("'", "''")
        End If
        Return result
    End Function

    Private Sub CollectSearchText()
        m_strKeyWords = Request.Form("txtSearch")
        m_strSelectedItem = Request.Form("searchlist")
        If (m_strSelectedItem = "-1") Then
            'm_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%' OR last_name like '%" & Quote(m_strKeyWords) & "%' OR user_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "last_name") Then
            'm_strSearchText = " (last_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "first_name") Then
            'm_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "user_name") Then
            'm_strSearchText = " (user_name like '%" & Quote(m_strKeyWords) & "%')"
        End If
    End Sub

#End Region

#Region "Grid Events"
    Protected Sub Grid_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        If (Me.m_sPageAction = "view") Then
            Select Case e.Item.ItemType
                Case ListItemType.AlternatingItem, ListItemType.Item
                    If (e.Item.Cells(1).Text.Equals("REMOVE-ITEM") Or e.Item.Cells(1).Text.Equals("important") Or e.Item.Cells(1).Text.Equals("input-box-text")) Then
                        e.Item.Cells(0).Attributes.Add("align", "Left")
                        e.Item.Cells(0).ColumnSpan = 2
                        If (e.Item.Cells(0).Text.Equals("REMOVE-ITEM")) Then
                            'e.Item.Cells(0).CssClass = ""
                        Else
                            e.Item.Cells(0).CssClass = e.Item.Cells(1).Text
                        End If
                        e.Item.Cells.RemoveAt(1)
                    End If
            End Select
        End If
    End Sub

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
        CurrentPage.Text = m_intCurrentPage.ToString()

        Select Case Me.m_sPageAction
            Case "adduser"
                Display_Add()
            Case Else
                Display_View()
        End Select

        ' isPostData.Value = "true"
    End Sub
#End Region

    Private Sub RegisterResources()
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronUITabsCss)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
    End Sub

End Class
