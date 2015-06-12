Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkEnumeration
Imports Microsoft.Security.Application

Partial Class rolemembermgr
    Inherits System.Web.UI.UserControl
    '
    ' Role Member Manger
    '
    ' This user control will all viewing the members (users nd/or groups)
    ' of a particular role, as well as adding or deleting members.
    '

    Protected m_refSiteApi As New SiteAPI
    Protected m_refUserApi As New UserAPI
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected ContentLanguage As Integer = -1
    Protected OrderBy As String = ""
    Protected m_RoleMembers As RoleMemberData()
    Protected m_bEditing As Boolean = False
    Protected m_strAction As String = ""
    Protected m_strOperation As String = ""
    Protected m_nRoleId As Long = -1
    Protected m_strRoleName As String = ""
    Protected m_strUpdateMode As String = ""
    Protected m_strUserIds As String = ""
    Protected m_strGroupIds As String = ""
    Protected m_strCustomString As String = ""
    Protected m_IncludeMembershipItems As Boolean = False
    Protected role_request As RoleRequest
    Protected m_strSelectedItem As String = "1"
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected m_strKeyWords As String = ""
    Protected m_strSearchText As String = ""


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        m_refMsg = m_refSiteApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        ContentLanguage = m_refSiteApi.ContentLanguage

        If (Not Request.Form.Item("manager_mode") Is Nothing) Then
            m_strUpdateMode = Request.Form.Item("manager_mode")
        End If
        If (Not Request.Form.Item("member_user_ids") Is Nothing) Then
            m_strUserIds = Request.Form.Item("member_user_ids")
        End If
        If (Not Request.Form.Item("member_group_ids") Is Nothing) Then
            m_strGroupIds = Request.Form.Item("member_group_ids")
        End If

        If ((Not Request.QueryString("edit") Is Nothing) AndAlso (Request.QueryString("edit") = "1")) Then
            m_bEditing = True
        Else
            m_bEditing = False
        End If

        ' Determine the role-id based on the action string:
        If (Not Request.QueryString("action") Is Nothing) Then
            m_strAction = Request.QueryString("action")
        End If
        Select Case m_strAction
            Case "aliasedit"
                m_nRoleId = CmsRoleIds.EditAlias
                m_strRoleName = m_refMsg.GetMessage("lbl Alias-Edit")
            Case "aliasadmin"
                m_nRoleId = CmsRoleIds.UrlAliasingAdmin
                m_strRoleName = m_refMsg.GetMessage("lbl Alias-Admin")
            Case "calendaradmin"
                m_nRoleId = CmsRoleIds.AdminCalendar
                m_strRoleName = m_refMsg.GetMessage("lbl Calendar-Admin")
            Case "collectionmenuadmin"
                m_nRoleId = CmsRoleIds.AminCollectionMenu
                m_strRoleName = m_refMsg.GetMessage("lbl Collection and Menu Admin")
            Case "metadataadmin"
                m_nRoleId = CmsRoleIds.AdminMetadata
                m_strRoleName = m_refMsg.GetMessage("lbl Metadata-Admin")
            Case "masterlayoutcreate"
                m_nRoleId = CmsRoleIds.CreateMasterLayout
                m_strRoleName = m_refMsg.GetMessage("lbl masterlayout-create")
            Case "ruleedit"
                m_nRoleId = CmsRoleIds.AdminRuleEditor
                m_strRoleName = m_refMsg.GetMessage("lbl Business Rule Editor")
            Case "taskcreate"
                m_nRoleId = CmsRoleIds.CreateTask
                m_strRoleName = m_refMsg.GetMessage("lbl Task-Create")
            Case "taskredirect"
                m_nRoleId = CmsRoleIds.RedirectTask
                m_strRoleName = m_refMsg.GetMessage("lbl Task-Redirect")
            Case "taskdelete"
                m_nRoleId = CmsRoleIds.DeleteTask
                m_strRoleName = m_refMsg.GetMessage("lbl Task-Delete")
            Case "useradmin"
                m_nRoleId = CmsRoleIds.AdminUsers
                m_strRoleName = m_refMsg.GetMessage("lbl User Admin")
            Case "folderuseradmin"
                m_nRoleId = CmsRoleIds.AdminFolderUsers
                m_strRoleName = m_refMsg.GetMessage("lbl Folder User Admin")
            Case "xliffadmin"
                m_nRoleId = CmsRoleIds.AdminXliff
                m_strRoleName = m_refMsg.GetMessage("lbl XLIFF admin")
            Case "syncadmin"
                m_nRoleId = CmsRoleIds.SyncAdmin
                m_strRoleName = m_refMsg.GetMessage("lbl sync admin")
            Case "xmlconfigadmin"
                m_nRoleId = CmsRoleIds.AdminXmlConfig
                m_strRoleName = m_refMsg.GetMessage("lbl Smart Forms Admin")
            Case "templateconfig"
                m_nRoleId = CmsRoleIds.TemplateConfigurations
                m_strRoleName = m_refMsg.GetMessage("lbl Template Configuration")

            Case "personalizationadmin"
                m_nRoleId = CmsRoleIds.AdminPersonalize
                m_strRoleName = m_refMsg.GetMessage("lbl Personalization - Admin")
                m_IncludeMembershipItems = True
            Case "personalization"
                m_nRoleId = CmsRoleIds.Personalize
                m_strRoleName = m_refMsg.GetMessage("lbl Personalization")
                m_IncludeMembershipItems = True
            Case "personalizationaddonly"
                m_nRoleId = CmsRoleIds.PersonalizeAddOnly
                m_strRoleName = m_refMsg.GetMessage("lbl personalization-add/pick webparts from catalog")
                m_IncludeMembershipItems = True
            Case "personalizationmoveonly"
                m_nRoleId = CmsRoleIds.PersonalizeMoveOnly
                m_strRoleName = m_refMsg.GetMessage("lbl personalization: move web parts")
                m_IncludeMembershipItems = True
            Case "personalizationeditonly"
                m_nRoleId = CmsRoleIds.PersonalizeEditOnly
                m_strRoleName = m_refMsg.GetMessage("lbl Personalization - Edit WebParts")
                m_IncludeMembershipItems = True
            Case "collectionapprovers"
                m_nRoleId = CmsRoleIds.CollectionApprovers
                m_strRoleName = m_refMsg.GetMessage("lbl collection approver")
                m_IncludeMembershipItems = False
            Case "multivariatetester"
                m_nRoleId = CmsRoleIds.MultivariateTester
                m_strRoleName = m_refMsg.GetMessage("lbl multivariate tester")
                m_IncludeMembershipItems = False
            Case "custompermissions"
                If (Not Request.QueryString("id") Is Nothing) Then
                    m_nRoleId = CLng(Request.QueryString("id"))
                    m_strCustomString = "&id=" & m_nRoleId.ToString
                End If
                If (Not Request.QueryString("name") Is Nothing) Then
                    m_strRoleName = AntiXss.HtmlEncode(Request.QueryString("name"))
                    m_strCustomString += "&name=" & m_strRoleName
                End If
                m_IncludeMembershipItems = True
            Case "taxonomyadministrator"
                m_nRoleId = CmsRoleIds.TaxonomyAdministrator
                m_strRoleName = "Taxonomy Administrator(s)"
                m_IncludeMembershipItems = False
            Case "messageboardadmin"
                m_nRoleId = CmsRoleIds.MessageBoardAdmin
                m_strRoleName = m_refMsg.GetMessage("lbl messageboard-admin")
                m_IncludeMembershipItems = True
            Case "communitygroupadmin"
                m_nRoleId = CmsRoleIds.CommunityGroupAdmin
                m_strRoleName = m_refMsg.GetMessage("lbl role communitygroup-admin")
                m_IncludeMembershipItems = True
            Case "communitygroupcreate"
                m_nRoleId = CmsRoleIds.CommunityGroupCreate
                m_strRoleName = m_refMsg.GetMessage("lbl role communitygroup-create")
                m_IncludeMembershipItems = True
            Case "commerceadmin"
                m_nRoleId = CmsRoleIds.CommerceAdmin
                m_strRoleName = m_refMsg.GetMessage("lbl role commerce-admin")
                m_IncludeMembershipItems = False
            Case "moveorcopy"
                m_nRoleId = CmsRoleIds.MoveOrCopy
                m_strRoleName = m_refMsg.GetMessage("lbl move or copy")
            Case "analyticsviewer"
                m_nRoleId = CmsRoleIds.AnalyticsViewer
                m_strRoleName = m_refMsg.GetMessage("lbl role analytics-viewer")
                m_IncludeMembershipItems = False
            Case "communityadmin"
                m_nRoleId = CmsRoleIds.CommunityAdmin
                m_strRoleName = m_refMsg.GetMessage("lbl role community-admin")
                m_IncludeMembershipItems = False
		    Case "searchadmin"
                m_nRoleId = CmsRoleIds.SearchAdmin
                m_strRoleName = m_refMsg.GetMessage("lbl role search-admin")
            Case Else
                m_nRoleId = -1
        End Select

        ' Determine operation, viewing/adding/deleting role-members:
        If (Not Request.QueryString("operation") Is Nothing) Then
            m_strOperation = Request.QueryString("operation")
        End If
        If (Page.IsPostBack) Then
            m_strSelectedItem = Convert.ToString(Request.Form("selecttype"))
            m_strAction &= "&selecttype=" & m_strSelectedItem
            If (Request.Form(isPostData.UniqueID) <> "") Then
                ProcessAction()
                isPostData.Value = "true"
            End If
        Else
            If (Not Request.QueryString("selecttype") Is Nothing) Then
                m_strSelectedItem = Request.QueryString("selecttype")
            End If
            ProcessAction()
        End If

        RegisterResources()

    End Sub
    Private Sub ProcessAction()
        Select Case m_strOperation
            Case ""
                ViewRoleMembers()
            Case "addmembers"
                If (ProcessUpdating(False)) Then
                    ViewRoleMembers()
                Else
                    AddMembers()
                End If
            Case "dropmembers"
                If (ProcessUpdating(True)) Then
                    ViewRoleMembers()
                Else
                    DropMembers()
                End If
            Case Else
        End Select
    End Sub
    Public Function ProcessUpdating(ByVal bDropping As Boolean) As Boolean
        Dim roleMember As New RoleMemberData
        Dim contObj As Ektron.Cms.Content.EkContent
        contObj = m_refContentApi.EkContentRef()
        Dim strIds As String()
        Dim nIndex As Integer

        If ((m_nRoleId < 0) OrElse ((m_strUserIds.Length = 0) AndAlso (m_strGroupIds.Length = 0))) Then
            Return False ' no processing to do...
        End If

        ' add the selected user members
        If (m_strUserIds.Length) Then
            strIds = m_strUserIds.Split(",")
            For nIndex = 0 To (strIds.GetLength(0) - 1)
                roleMember.MemberId = strIds(nIndex)
                'roleMember.MemberName = ""
                roleMember.MemberType = RoleMemberData.RoleMemberType.User
                If (bDropping) Then
                    contObj.DropRoleMember(m_nRoleId, roleMember)
                Else
                    contObj.AddRoleMember(m_nRoleId, roleMember)
                End If
            Next
        End If

        ' add the selected group members
        If (m_strGroupIds.Length) Then
            strIds = m_strGroupIds.Split(",")
            For nIndex = 0 To (strIds.GetLength(0) - 1)
                roleMember.MemberId = strIds(nIndex)
                'roleMember.MemberName = ""
                roleMember.MemberType = RoleMemberData.RoleMemberType.Group
                If (bDropping) Then
                    contObj.DropRoleMember(m_nRoleId, roleMember)
                Else
                    contObj.AddRoleMember(m_nRoleId, roleMember)
                End If
            Next
        End If

        roleMember = Nothing
        If (m_strOperation <> "") Then
            Response.Redirect("roles.aspx?action=" & m_strAction & "&id=" & m_nRoleId & "&name=" & m_strRoleName, True)
        End If
        Return True
    End Function

    Public Sub ViewRoleMembers()
        If (Page.IsPostBack AndAlso Request.Form(isSearchPostData.UniqueID) <> "") Then
            CollectSearchText()
            DisplayUsers()
        Else
            Dim contObj As Ektron.Cms.Content.EkContent
            contObj = m_refContentApi.EkContentRef()
            role_request = New RoleRequest
            role_request.RoleId = m_nRoleId
            'role_request.IncludeMember = True
            role_request.SearchText = m_strSearchText
            role_request.IsAssigned = 1
            role_request.RoleType = Convert.ToInt32(m_strSelectedItem)
            role_request.PageSize = m_refContentApi.RequestInformationRef.PagingSize
            role_request.CurrentPage = m_intCurrentPage
            m_RoleMembers = contObj.GetAllRoleMembers(role_request) 'contObj.GetAllRoleMembers(m_nRoleId)
            m_intTotalPages = role_request.TotalPages
            ViewRoleMembersToolBar()
            Populate_Members_RoleMemberGrid()
        End If
    End Sub

    Private Sub ViewRoleMembersToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl manage members for role")) & ":" & m_strRoleName ' m_refMsg.GetMessage("view user groups msg"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/add.png", "roles.aspx?action=" & m_strAction & "&operation=addmembers&LangType=" & ContentLanguage & m_strCustomString, m_refMsg.GetMessage("lbl add role member"), m_refMsg.GetMessage("lbl add role member"), " "))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/delete.png", "roles.aspx?action=" & m_strAction & "&operation=dropmembers&LangType=" & ContentLanguage & m_strCustomString, m_refMsg.GetMessage("lbl drop role member"), m_refMsg.GetMessage("lbl drop role member"), " "))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append(AppendUserGroupDD)
        result.Append("<td nowrap valign=""top"">&nbsp;&nbsp;&nbsp;<label for=""txtSearch"">" & m_refMsg.GetMessage("generic search") & "</label>  <input type=""text"" class=""ektronTextMedium"" id=""txtSearch"" name=""txtSearch"" value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event);"">")
        result.Append("<input type=""button"" value=" & m_refMsg.GetMessage("generic Search") & " id=""btnSearch"" name=""btnSearch"" onclick=""searchuser();"" class=""ektronWorkareaSearch"" /></td>")
        result.Append("<td>")
        If (Not Request.QueryString("action") Is Nothing) Then
            result.Append(m_refStyle.GetHelpButton("viewrolemembers_" & Request.QueryString("action")))
        End If
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub Populate_Members_RoleMemberGrid(Optional ByVal bShowCheckBox As Boolean = False)
        Dim dt As DataTable
        Dim idx As Integer

        If (Not (IsNothing(m_RoleMembers))) Then
            RoleMemberGrid.Columns.Clear()
            Dim colBound As New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "ROLE_MEMBER_NAME"
            colBound.HeaderText = m_refMsg.GetMessage("alt role member name")
            RoleMemberGrid.Columns.Add(colBound)
            PageSettings()
            dt = New DataTable
            Dim dr As DataRow
            Dim strTypeIcon As String = ""
            Dim strNameId As String
            dt.Columns.Add(New DataColumn("ROLE_MEMBER_NAME", GetType(String)))

            For idx = 0 To m_RoleMembers.Length - 1
                strTypeIcon = LoadIcon(m_RoleMembers(idx).MemberType) 'IIf(m_RoleMembers(idx).MemberType = RoleMemberData.RoleMemberType.User, "user.png", "users.png")
                dr = dt.NewRow
                'dr(0) = IIf(bShowCheckBox, "<input type=""checkbox"" name=""frm_fixme"" id=""frm_fixme"">&nbsp;", "")
                If (bShowCheckBox) Then
                    If (m_RoleMembers(idx).MemberType = RoleMemberData.RoleMemberType.User Or m_RoleMembers(idx).MemberType = RoleMemberData.RoleMemberType.MemberUser) Then
                        strNameId = "member_user_id" & m_RoleMembers(idx).MemberId.ToString()
                    Else
                        strNameId = "member_group_id" & m_RoleMembers(idx).MemberId.ToString()
                    End If
                    dr(0) = "<input type=""checkbox"" name=""" & strNameId & """ id=""" & strNameId & """>"
                Else
                    dr(0) = ""
                End If
                dr(0) += "<img src=""" & m_refSiteApi.AppPath & "images/UI/Icons/" & strTypeIcon & """ align=""absbottom"">"
                dr(0) += m_RoleMembers(idx).MemberName()
                dt.Rows.Add(dr)
            Next
            Dim dv As New DataView(dt)
            RoleMemberGrid.DataSource = dv
            RoleMemberGrid.DataBind()
        End If
    End Sub
    Private Sub CollectSearchText()
        m_strKeyWords = Request.Form("txtSearch")
        m_strSearchText = Quote(m_strKeyWords)
    End Sub
    Private Function Quote(ByVal KeyWords As String) As String
        Dim result As String = KeyWords
        If (KeyWords.Length > 0) Then
            result = KeyWords.Replace("'", "''")
        End If
        Return result
    End Function
    Public Sub AddMembers()
        Dim contObj As Ektron.Cms.Content.EkContent
        contObj = m_refContentApi.EkContentRef()
        If (Page.IsPostBack AndAlso Request.Form(isSearchPostData.UniqueID) <> "") Then
            AddRoleMembersToolBar()
            CollectSearchText()
            DisplayUsers()
        Else
            AddRoleMembersToolBar()
            role_request = New RoleRequest
            role_request.RoleId = m_nRoleId
            'role_request.IncludeMember = True
            role_request.IsAssigned = 0
            role_request.RoleType = Convert.ToInt32(m_strSelectedItem)
            role_request.SortDirection = ""
            role_request.SortOrder = ""
            role_request.PageSize = m_refContentApi.RequestInformationRef.PagingSize
            role_request.CurrentPage = m_intCurrentPage
            'm_RoleMembers = contObj.GetAllNonRoleMembers(m_nRoleId, m_IncludeMembershipItems)
            m_RoleMembers = contObj.GetAllRoleMembers(role_request)
            m_intTotalPages = role_request.TotalPages
            Populate_Members_RoleMemberGrid(True)
        End If
    End Sub

    Private Sub AddRoleMembersToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl Manage Role Members"))   ' m_refMsg.GetMessage("view user groups msg"))
        result.Append("<table><tr>")
        'result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/contentEdit.png", "roles.aspx?action=" & m_strAction & "&edit=0&update=1", m_refMsg.GetMessage("btn edit"), m_refMsg.GetMessage("btn edit"), "onclick=""return true;"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("btn edit"), m_refMsg.GetMessage("btn save"), "onclick=""submitAddMembers();return true;"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append(AppendUserGroupDD)
        result.Append("<td>&nbsp;&nbsp;&nbsp;<label for=""txtSearch"">" & m_refMsg.GetMessage("generic search") & "</label><input type=""text"" class=""ektronTextMedium"" id=""txtSearch"" name=""txtSearch"" value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event);"">")
        result.Append("<input type=""button"" value=""Search"" id=""btnSearch"" name=""btnSearch"" onclick=""searchuser();"" class=""ektronWorkareaSearch"" /></td>")
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("addrolemembers_" & m_strAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Public Sub DropMembers()
        Dim contObj As Ektron.Cms.Content.EkContent
        role_request = New RoleRequest
        role_request.RoleId = m_nRoleId
        'role_request.IncludeMember = True
        role_request.IsAssigned = 1
        role_request.RoleType = Convert.ToInt32(m_strSelectedItem)
        role_request.SortDirection = ""
        role_request.SortOrder = ""
        role_request.PageSize = m_refContentApi.RequestInformationRef.PagingSize
        role_request.CurrentPage = m_intCurrentPage
        contObj = m_refContentApi.EkContentRef()
        DropRoleMembersToolBar()
        ' m_RoleMembers = contObj.GetAllRoleMembers(m_nRoleId)
        m_RoleMembers = contObj.GetAllRoleMembers(role_request)
        m_intTotalPages = role_request.TotalPages
        Populate_Members_RoleMemberGrid(True)
    End Sub

    Private Sub DropRoleMembersToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl Manage Role Members"))   ' m_refMsg.GetMessage("view user groups msg"))
        result.Append("<table><tr>")
        'result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/delete.png", "roles.aspx?action=" & m_strAction & "&edit=0&update=1", m_refMsg.GetMessage("btn edit"), m_refMsg.GetMessage("btn edit"), "onclick=""return true;"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("btn delete"), m_refMsg.GetMessage("btn delete"), "onclick=""submitDropMembers();return true;"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append(AppendUserGroupDD)
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("droprolemembers_" & m_strAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Function AppendUserGroupDD() As String
        Dim result As New StringBuilder
        result.Append("<td class=""label"">&nbsp;|&nbsp;" & m_refMsg.GetMessage("lbl show") & ":</td>")
        result.Append("<td>")
        result.Append("<select id=""selecttype"" name=""selecttype"" onchange=""submitform();"">")
        result.Append("<option value=""1""" & IsSelected("1") & ">" & m_refMsg.GetMessage("generic cms user label") & "</option>")
        result.Append("<option value=""2""" & IsSelected("2") & ">" & m_refMsg.GetMessage("cms group title") & "</option>")

        If m_IncludeMembershipItems Then

            result.Append("<option value=""3""" & IsSelected("3") & ">" & m_refMsg.GetMessage("lbl member user") & "</option>")
            result.Append("<option value=""4""" & IsSelected("4") & ">" & m_refMsg.GetMessage("lbl member group") & "</option>")

        End If

        result.Append("</select>")
        result.Append("</td>")
        Return result.ToString()
    End Function

    Private Function IsSelected(ByVal val As String) As String
        If (val = m_strSelectedItem) Then
            Return (" selected ")
        Else
            Return ("")
        End If
    End Function

    Private Function LoadIcon(ByVal val As RoleMemberData.RoleMemberType) As String
        Dim result As String = "user.png"
        Select Case val
            Case RoleMemberData.RoleMemberType.User
                result = "user.png"
            Case RoleMemberData.RoleMemberType.Group
                result = "users.png"
            Case RoleMemberData.RoleMemberType.MemberUser
                result = "userMembership.png"
            Case RoleMemberData.RoleMemberType.MemberGroup
                result = "usersMembership.png"
        End Select
        Return result
    End Function

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
        ProcessAction()
        isPostData.Value = "true"
    End Sub
    Private Sub DisplayUsers()
        Dim contObj As Ektron.Cms.Content.EkContent

        If (Request.QueryString("OrderBy") = "") Then
            OrderBy = "user_name"
        Else
            OrderBy = Request.QueryString("OrderBy")
        End If

        contObj = m_refContentApi.EkContentRef()
        role_request = New RoleRequest
        role_request.RoleId = m_nRoleId
        role_request.SearchText = m_strSearchText
        If (m_strOperation = "addmembers") Then
            role_request.IsAssigned = 0
        Else
            role_request.IsAssigned = 1
        End If
        role_request.RoleType = Convert.ToInt32(m_strSelectedItem)
        role_request.PageSize = m_refContentApi.RequestInformationRef.PagingSize
        role_request.CurrentPage = m_intCurrentPage
        m_RoleMembers = contObj.GetAllRoleMembers(role_request)
        m_intTotalPages = role_request.TotalPages

        If (m_strOperation = "") Then
            ViewRoleMembersToolBar()
        End If

        If (m_strOperation = "addmembers") Then
            Populate_Members_RoleMemberGrid(True)
        Else
            Populate_Members_RoleMemberGrid()
        End If

    End Sub

    Private Sub RegisterResources()
        ' register JS

        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronInputLabelJS)
        API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareSearchBoxInputLabelInitJS")
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)

        ' register CSS
    End Sub

End Class


Public Class RoleHelper
    Protected m_formalName As String = ""

End Class