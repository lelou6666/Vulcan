'NOTE id=mapped to uid
Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Notifications
Imports Ektron.Cms.Framework
Partial Class viewusers
    Inherits System.Web.UI.UserControl

    Protected language_data As LanguageData()
    Protected user_data As UserData
    Protected security_data As PermissionData
    Protected m_refSiteApi As New SiteAPI
    Protected m_refUserApi As New UserAPI
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected domain_data As DomainData()
    Protected UserName As String = ""
    Protected ContentLanguage As Integer = -1
    Protected FirstName As String = ""
    Protected LastName As String = ""
    Protected uId As Long = -1
    Protected setting_data As SettingsData
    Protected OrderBy As String = ""
    Protected FromUsers As String = ""
    Protected usergroup_data As UserGroupData
    Protected user_list As UserData()
    Protected m_refMailMsg As New EmailHelper
    Protected group_list As GroupData()
    Protected CurrentUserID As Long = -1
    Protected PageAction As String = ""
    Protected search As String = ""
    Protected rp As String = ""
    Protected e1 As String = ""
    Protected e2 As String = ""
    Protected f As String = ""
    Protected GroupName As String = "EveryOne"
    Protected m_intGroupType As Integer = -1 '0-CMS User; 1-Membership User
    Protected m_intGroupId As Long = -1
    Protected m_intUserActiveFlag As Integer = 0 '0-Active;1-Deleted;-1-Not verified
    Protected m_strDirection As String = "asc"
    Protected m_strSearchText As String = ""
    Protected m_strKeyWords As String = ""
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected m_strPageAction As String = ""
    Protected m_strSelectedItem As String = "-1"
    Private m_strBackAction As String = "viewallgroups"
    Private m_strCallerPage As String = ""
    Private m_bCommunityGroup As Boolean
    Private m_iCommunityGroup As Long = 0
    Protected _refStyle As New StyleHelper
    Protected _notificationPreferenceApi As New Ektron.Cms.Framework.Notifications.NotificationPreference
    Protected preferenceList As System.Collections.Generic.List(Of NotificationPreferenceData)
    Protected prefData As New NotificationPreferenceData
    Protected _notificationAgentApi As New Ektron.Cms.Framework.Notifications.NotificationAgentSetting
    Protected agentList As System.Collections.Generic.List(Of NotificationAgentData)
    Protected agentData As NotificationAgentData
    Protected _activityApi As New Ektron.Cms.Framework.Activity.Activity
    Protected _activityListApi As New Ektron.Cms.Framework.Activity.ActivityType
    Protected activityTypeList As System.Collections.Generic.List(Of Ektron.Cms.Activity.ActivityTypeData)
    Protected groupAliasList As String = String.Empty




#Region "Load"
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        RegisterResources()

        workareaTab.Visible = False
        workareaDiv.Visible = False

        If ((Not IsNothing(Request.QueryString("grouptype"))) AndAlso (Request.QueryString("grouptype") <> "")) Then
            m_intGroupType = Convert.ToInt32(Request.QueryString("grouptype"))
        End If
        If ((Not IsNothing(Request.QueryString("communitygroup"))) AndAlso (Request.QueryString("communitygroup") <> "")) Then
            m_bCommunityGroup = True
        End If
        If ((Not IsNothing(Request.QueryString("groupid"))) AndAlso (Request.QueryString("groupid") <> "")) Then
            m_intGroupId = Convert.ToInt64(Request.QueryString("groupid"))
            If m_bCommunityGroup Then
                m_iCommunityGroup = m_intGroupId
                m_intGroupId = Me.m_refContentApi.EkContentRef.GetCmsGroupForCommunityGroup(m_iCommunityGroup)
            End If
        End If
        If ((Not IsNothing(Request.QueryString("id"))) AndAlso (Request.QueryString("id") <> "")) Then
            uId = Convert.ToInt64(Request.QueryString("id"))
        End If
        If ((Not (IsNothing(Request.QueryString("action")))) AndAlso (Request.QueryString("action") <> "")) Then
            m_strPageAction = Request.QueryString("action").ToLower
        End If
        If ((Not (IsNothing(Request.QueryString("backaction")))) AndAlso (Request.QueryString("backaction") <> "")) Then
            m_strBackAction = Request.QueryString("backaction").ToLower
        End If
        If ((Not IsNothing(Request.QueryString("ty"))) AndAlso (Request.QueryString("ty") = "nonverify")) Then
            m_intUserActiveFlag = -1
            m_strBackAction = m_strBackAction & "&ty=nonverify"
        End If
        m_strDirection = Request.QueryString("direction")

        If (m_strDirection = "asc") Then
            m_strDirection = "desc"
        Else
            m_strDirection = "asc"
        End If

        'VisiblePageControls(False)
        Me.uxPaging.Visible = False

        Utilities.SetLanguage(m_refSiteApi)
        Utilities.SetLanguage(m_refUserApi)
        Utilities.SetLanguage(m_refContentApi)
        m_refMsg = m_refSiteApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        AppPath = m_refSiteApi.AppPath
        ContentLanguage = m_refSiteApi.ContentLanguage

        If (m_strPageAction = "viewallusers") Then
            If (Not (IsNothing(Request.QueryString("callerpage")))) Then
                If (Request.QueryString("callerpage") <> "") Then
                    m_strCallerPage = Request.QueryString("callerpage")
                End If
            End If
            ViewAllUsers()
        End If
    End Sub

#End Region

#Region "VIEW"
    Public Function View() As Boolean
        'VisiblePageControls(False)
        Me.uxPaging.Visible = False
        PageAction = "view"
        CurrentUserID = m_refSiteApi.UserId

        FromUsers = Request.QueryString("FromUsers")
        Dim bPreference As Boolean = True
        Dim bReturnDeleted As Boolean = False
        If (m_intGroupType = 1) Then
            bPreference = False
        End If
        If (m_intUserActiveFlag = -1) Then
            bReturnDeleted = True
        End If
        user_data = m_refUserApi.GetUserById(uId, bPreference, bReturnDeleted)
		group_list = m_refUserApi.GetGroupsUserIsIn(uId, Ektron.Cms.Common.EkEnumeration.GroupOrderBy.GroupName)
        security_data = m_refContentApi.LoadPermissions(0, "content")
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
        ViewToolBar()
        Populate_ViewGrid()
        CreateColumns()
        If (_activityApi.IsActivityPublishingEnabled And agentList IsNot Nothing And agentList.Count > 0 And Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking, False)) Then
            LoadGrid("colleagues")
            LoadGrid("groups")
            ViewUserPublishPreferences()
        Else
            EkMembershipActivityTable.Visible = False
            activitiesTab.Visible = False
        End If
        'community aliasing Tab
        LoadCommunityAliasTab()

    End Function

    Private Sub Populate_ViewGrid()
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.ItemStyle.CssClass = "label"
        FormGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "VALUE"
        colBound.ItemStyle.CssClass = "readOnlyValue"
        FormGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("VALUE", GetType(String)))

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("username label")
        dr(1) = user_data.Username
        dt.Rows.Add(dr)

        If (LDAPMembers() And setting_data.ADAuthentication = 1) Then
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("domain title")
            dr(1) = user_data.Domain
            dt.Rows.Add(dr)
        ElseIf (LDAPMembers() And setting_data.ADAuthentication = 2) Then
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("generic path") & ":"
            dr(1) = user_data.Domain
            dt.Rows.Add(dr)
        End If

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("generic id") & ":"
        dr(1) = user_data.Id.ToString
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("first name label")
        dr(1) = user_data.FirstName
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("last name label")
        dr(1) = user_data.LastName
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("display name label") & ":"
        dr(1) = user_data.DisplayName
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("user language label")
        If (user_data.LanguageId = 0) Then
            dr(1) = m_refMsg.GetMessage("app default msg")
        Else
            dr(1) = user_data.LanguageName
        End If
        dt.Rows.Add(dr)


        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("email address label")
        If (user_data.Email.Length = 0) Then
            dr(1) = m_refMsg.GetMessage("none specified msg")
        Else
            dr(1) = user_data.Email
        End If
        dt.Rows.Add(dr)
        If ((Me.m_refContentApi.RequestInformationRef.LoginAttempts <> -1) And ((security_data.IsAdmin) Or m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers))) Then
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("account locked") & ": "
            dr(1) = "<input type=""checkbox"" id=""accLocked_" & user_data.Id & """ disabled "
            If (user_data.IsAccountLocked(Me.m_refContentApi.RequestInformationRef)) Then
                dr(1) += " checked "
            End If
            dr(1) += " />"
            dt.Rows.Add(dr)
        End If

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("lbl last login date") & ": "
        dr(1) = user_data.LastLoginDate
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("lbl editor") & ":"
        dr(1) = "<select disabled>"
        'dr(1) &= "<option value=""contentdesigner"">" & m_refMsg.GetMessage("lbl content designer") & "</option>"
        Select Case user_data.EditorOption.ToLower().Trim()
            Case "ewebeditpro"
                'dr(1) &= "<option value=""ewebwp"" >eWebWP</option>"
                'dr(1) &= "<option value=""jseditor"">" & m_refMsg.GetMessage("lbl jseditor") & "</option>"
                dr(1) &= "<option value=""ewebeditpro"" selected>eWebEditPro</option>"
                'Case "ewebwp"
                '    dr(1) &= "<option value=""ewebwp"" selected>eWebWP</option>"
                '    dr(1) &= "<option value=""jseditor"">" & m_refMsg.GetMessage("lbl jseditor") & "</option>"
                '    dr(1) &= "<option value=""ewebeditpro"" >eWebEditPro</option>"
            Case "jseditor"
                '    dr(1) &= "<option value=""jseditor"" selected>" & m_refMsg.GetMessage("lbl jseditor") & "</option>"
                '    dr(1) &= "<option value=""ewebeditpro"">eWebEditPro</option>"
            Case Else ' "jseditor" or "ewebwp"
                'dr(1) &= "<option value=""ewebwp"">eWebWP</option>"
                dr(1) &= "<option value=""contentdesigner"" selected>" & m_refMsg.GetMessage("lbl content designer") & "</option>"
                'dr(1) &= "<option value=""jseditor"">" & m_refMsg.GetMessage("lbl jseditor") & "</option>"
                dr(1) &= "<option value=""ewebeditpro"">eWebEditPro</option>"
        End Select
        dr(1) &= "</select>"
        dt.Rows.Add(dr)

        If (m_intGroupType = 0) Then
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("lbl workflow and task email")
            If (user_data.IsDisableMessage) Then
                dr(1) = m_refMsg.GetMessage("email disabled msg")
            Else
                dr(1) = m_refMsg.GetMessage("email enabled msg")
            End If

            If (security_data.IsAdmin And setting_data.EnableMessaging = False) Then
                dr(1) += "<br /><label class=""ektronCaption"">" & m_refMsg.GetMessage("application emails disabled msg") & "</label>" 'application emails disabled msg
            End If

            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("lbl avatar") & ":"
            dr(1) = IIf(user_data.Avatar.Length > 0, "<a href=""" & Server.HtmlEncode(user_data.Avatar) & """ target=""_blank"">" & Server.HtmlEncode(user_data.Avatar) & "</a>", "")
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = "Address:"
            dr(1) = user_data.Address
            dt.Rows.Add(dr)
            dr = dt.NewRow
            dr(0) = "Latitude:"
            dr(1) = user_data.Latitude
            dt.Rows.Add(dr)
            dr = dt.NewRow
            dr(0) = "Longitude:"
            dr(1) = user_data.Longitude
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("lbl signature") & ":"
			dr(1) = user_data.Signature
            dt.Rows.Add(dr)

            ' Personal Tags:
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("lbl personal tags") + ":"
            dr(1) = GetPersonalTags()
            dt.Rows.Add(dr)

            If (Not (IsNothing(user_data.UserPreference))) Then
                If (user_data.UserPreference.ForceSetting) Then
                    dr = dt.NewRow
                    dr(0) = "Preferences are locked by the CMS."
                    dr(1) = "important" 'class=important
                    dt.Rows.Add(dr)
                End If
            End If

            Display_WorkPage()
        Else
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("lbl avatar") & ":"
            dr(1) = IIf(user_data.Avatar.Length > 0, "<a href=""" & Server.HtmlEncode(user_data.Avatar) & """ target=""_blank"">" & Server.HtmlEncode(user_data.Avatar) & "</a>", "")
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("lbl signature") & ":"
            dr(1) = (user_data.Signature)
            dt.Rows.Add(dr)

            ' Personal Tags:
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("lbl personal tags") + ":"
            dr(1) = GetPersonalTags()
            dt.Rows.Add(dr)
        End If

        If (Not (IsNothing(group_list))) Then
            Me.rptUserGroups.DataSource = group_list
            Me.rptUserGroups.DataBind()
        End If

        Dim dv As New DataView(dt)
        FormGrid.DataSource = dv
        FormGrid.DataBind()

        Display_CustomProperties()

        viewUser.Visible = True
    End Sub

    Private Sub Display_WorkPage()
        workareaTab.Visible = True
        workareaDiv.Visible = True

        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.ItemStyle.CssClass = "label"
        WorkPage.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "VALUE"
        colBound.ItemStyle.CssClass = "readOnlyValue"
        WorkPage.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("VALUE", GetType(String)))

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("lbl fullscreen") & ":"
        If (user_data.UserPreference.Height = "9999" AndAlso user_data.UserPreference.Width = "9999") Then
            dr(1) = "<input type=""checkbox"" disabled=""disabled"" id=""chkFullScreen"" name=""chkFullScreen"" checked=""on"" />"
        Else
            dr(1) = "<input type=""checkbox"" disabled=""disabled"" id=""chkFullScreen"" name=""chkFullScreen"" />"
        End If
        dt.Rows.Add(dr)

        If (user_data.UserPreference.Height <> "9999" AndAlso user_data.UserPreference.Width <> "9999") Then
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("lbl Width") & ":"
            dr(1) = user_data.UserPreference.Width & "px"
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("lbl height") & ":"
            dr(1) = user_data.UserPreference.Height & "px"
            dt.Rows.Add(dr)
        End If

        dr = dt.NewRow
        dr(0) += m_refMsg.GetMessage("lbl display button text in the title bar") & ":"
        dr(1) = "<input type=""checkbox"" id=""chkDispTitleText"" disabled"
        If (user_data.UserPreference.DisplayTitleText = "1") Then
            dr(1) += " checked"
        End If
        dr(1) += " name=""chkDispTitleText"">"
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("lbl Landing Page after login") & ":"
        If (user_data.UserPreference.Template = "") Then
            dr(1) = m_refMsg.GetMessage("refresh login page msg")
        Else
            dr(1) = m_refSiteApi.SitePath & user_data.UserPreference.Template
        End If
        dt.Rows.Add(dr)

        dr = dt.NewRow

        dr(0) = m_refMsg.GetMessage("alt set smart desktop as the start location in the workarea") & ":"
        dr(1) = "<input type=""checkbox"" disabled"
        If (user_data.UserPreference.FolderId = "") Then
            dr(1) += " checked"
        End If
        dr(1) += "   id=""checkbox"" name=""chkSmartDesktop""> "

        dt.Rows.Add(dr)
        Dim dv As New DataView(dt)
        WorkPage.DataSource = dv
        WorkPage.DataBind()
    End Sub
    Protected Sub Display_CustomProperties()
        'Dim colBound As New System.Web.UI.WebControls.BoundColumn
        'colBound.DataField = "TITLE"
        'colBound.ItemStyle.CssClass = "label"
        'CustomProperties.Columns.Add(colBound)

        'colBound = New System.Web.UI.WebControls.BoundColumn
        'colBound.DataField = "VALUE"
        'colBound.ItemStyle.CssClass = "readOnlyValue"
        'CustomProperties.Columns.Add(colBound)

        'Dim dt As New DataTable
        'Dim dr As DataRow
        'dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        'dt.Columns.Add(New DataColumn("VALUE", GetType(String)))
        Dim strHtml As String = ""
        strHtml = m_refUserApi.EditUserCustomProperties(uId, True)

        ltr_CustomProperty.Text = strHtml
        'If (strHtml <> "") Then
        '    dr = dt.NewRow
        '    dr(0) = strHtml
        '    dt.Rows.Add(dr)
        'End If
        'Dim dv As New DataView(dt)
        'CustomProperties.DataSource = dv
        'CustomProperties.DataBind()
    End Sub

    Public Function GetPersonalTags() As String
        Dim result As New System.Text.StringBuilder
        Dim tdaUser() As TagData
        Dim tdaAll() As TagData
        Dim td As TagData
        Dim htTagsAssignedToUser As Hashtable

        Try
            htTagsAssignedToUser = New Hashtable
            result.Append("<div>")
            If (uId > 0) Then

                Dim localizationApi As New LocalizationAPI()
                tdaUser = (New Community.TagsAPI).GetTagsForUser(uId, -1)

                If (Not IsNothing(tdaUser)) Then
                    For Each td In tdaUser
                        result.Append("<input disabled=""disabled"" checked=""checked"" type=""checkbox"" id=""userPTagsCbx_" + td.Id.ToString + """ name=""userPTagsCbx_" + td.Id.ToString + """ />&#160;")
                        result.Append("<img src='" & localizationApi.GetFlagUrlByLanguageID(td.LanguageId) & "' border=""0"" />")
                        result.Append("&#160;" + td.Text + "<br />")
                    Next
                End If

            End If
            result.Append("</div>")

        Catch ex As Exception
        Finally
            GetPersonalTags = result.ToString
            tdaAll = Nothing
            tdaUser = Nothing
            td = Nothing
            htTagsAssignedToUser = Nothing
        End Try
    End Function

    Public Sub ViewToolBar()
        Dim result As New System.Text.StringBuilder
        Dim tempTy As String
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view user information msg") & " """ & user_data.DisplayUserName & """")
        result.Append("<table><tr>")
        If (m_intUserActiveFlag <> -1) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "users.aspx?action=EditUser&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&id=" & uId & "&FromUsers=" & FromUsers, m_refMsg.GetMessage("alt edit button text (user)") & " " & user_data.FirstName & " " & user_data.LastName & "", m_refMsg.GetMessage("btn edit"), ""))
            'RW - Journal functionality removed per Alpesh's request
            'If ((security_data.IsAdmin Or Me.uId = Me.m_refContentApi.UserId) AndAlso Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, DataIO.LicenseManager.Feature.SocialNetworking)) Then
            '    result.Append(m_refStyle.GetButtonEventsWCaption(apppath & "images/UI/Icons/journal.png", "MyWorkspace/MyJournal.aspx?id=" & Me.uId, m_refMsg.GetMessage("alt view user journal"), IIf(Me.uId = Me.m_refContentApi.UserId, m_refMsg.GetMessage("btn view my user journal"), m_refMsg.GetMessage("btn view user journal")), "onclick=""return NavWorkspaceTree(this, '\\journal');"""))
            'End If
        End If

        If (security_data.IsAdmin _
         OrElse m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers, CurrentUserID)) Then
            If (m_intUserActiveFlag = -1) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/approvals.png", "users.aspx?action=activateuseraccount&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&id=" & uId & "&FromUsers=" & FromUsers, "Click here to activate the user.", "Activate", "onclick=""return ConfirmActivateUser();"""))
            End If
            If (Request.QueryString("GroupID") = "2" Or Request.QueryString("GroupID") = "888888") Then
                If (uId <> CurrentUserID) Then

                    If ((Not IsNothing(Request.QueryString("ty"))) AndAlso (Request.QueryString("ty") = "nonverify")) Then
                        tempTy = "&ty=nonverify"
                    Else
                        tempTy = ""
                    End If
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "users.aspx?action=DeleteUserFromSystem&groupid=" & m_intGroupId & tempTy & "&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&id=" & uId & "&OrderBy=" & Request.QueryString("OrderBy") & "&FromUsers=" & FromUsers, m_refMsg.GetMessage("alt delete button text (user)"), m_refMsg.GetMessage("btn delete"), "onclick=""return ConfirmDeleteUser();"""))
                End If
            Else
                If uId <> CurrentUserID Then
                    Dim strUserParam As String = "action=DeleteUserFromGroup"
                    If (m_intUserActiveFlag = -1) Then
                        strUserParam = "action=deleteuserfromsystem&ty=nonverify"
                    End If
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "users.aspx?" & strUserParam & "&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&id=" & uId & "&OrderBy=" & Request.QueryString("OrderBy") & "&FromUsers=" & FromUsers, m_refMsg.GetMessage("alt delete button text (user2)"), m_refMsg.GetMessage("btn delete"), "onclick=""return ConfirmDeleteUserFromGroup();"""))
                End If
            End If
            If ((setting_data.ADIntegration) And (user_data.Domain <> "")) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/refresh.png", "users.aspx?action=UpdateADUser&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&id=" & user_data.Id & "&username=" & user_data.Username & "&domain=" & user_data.Domain & "&FromUsers=" & FromUsers, "Refresh", m_refMsg.GetMessage("btn refresh"), ""))
            End If
            If (setting_data.ADAuthentication = 1) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_adbrowse-nm.gif", "users.aspx?action=MapCMSUserToAD&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&id=" & user_data.Id & "&rp=1&FromUsers=" & FromUsers, m_refMsg.GetMessage("alt browse button text (user)"), m_refMsg.GetMessage("alt browse button text (user)"), ""))
            End If
            If m_intGroupId = 0 AndAlso Request.QueryString("callbackpage") IsNot Nothing Then
                If Request.QueryString("callbackpage") IsNot Nothing AndAlso Request.QueryString("folderid") IsNot Nothing AndAlso Request.QueryString("taxonomyid") IsNot Nothing AndAlso Request.QueryString("parentid") IsNot Nothing Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", Request.QueryString("callbackpage") & "&view=user&folderid=" & Request.QueryString("folderid") & "&taxonomyid=" & Request.QueryString("taxonomyid") & "&parentid=" & Request.QueryString("parentid"), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
                End If
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "users.aspx?action=viewallusers&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&id=" & Request.QueryString("GroupID") & "&FromUsers=" & FromUsers, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            End If
        End If
        result.Append("<td>")
        If m_intGroupType = 0 Then
            result.Append(m_refStyle.GetHelpButton("viewusers_ascx"))
        Else
            result.Append(m_refStyle.GetHelpButton("ViewMembershipUser"))
        End If
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
#End Region

#Region "ViewUsers"
    Public Sub ViewAllUsers()
        If (Page.IsPostBack And Request.Form(isPostData.UniqueID) <> "") Then
            If (Request.Form(isSearchPostData.UniqueID) <> "") Then
                CollectSearchText()
                DisplayUsers()
            Else

                If (m_intUserActiveFlag = -1) Then
                    Dim objUser As Ektron.Cms.User.EkUser
                    objUser = m_refSiteApi.EkUserRef
                    If (Request.Form(isDeleted.UniqueID) <> "") Then
                        objUser.DeleteMembershipUsers(Request.Form("req_deleted_users"))
                    Else
                        objUser.ActivateUserAccounts(Request.Form("req_deleted_users"))
                    End If
                    Response.Redirect("users.aspx?ty=nonverify&action=viewallusers&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId & "&OrderBy=" & Request.QueryString("OrderBy"), False)
                Else
                    If (Request.Form(isDeleted.UniqueID) <> "") Then
                        m_refUserApi.DeleteUserByIds(Request.Form("req_deleted_users"))
                        'after delete do a full postback to recalculate #TotalPages
                        Response.Redirect("users.aspx?action=viewallusers&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId & "&OrderBy=" & Request.QueryString("OrderBy"), False)
                    Else
                        'Page link selected
                        DisplayUsers()
                    End If
                End If
            End If
        ElseIf (IsPostBack = False) Then
            DisplayUsers()
        End If
        isPostData.Value = "true"
    End Sub
    Private Sub CollectSearchText()
        m_strKeyWords = Request.Form("txtSearch")
        m_strSelectedItem = Request.Form("searchlist")
        If (m_strSelectedItem = "-1") Then
            m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%' OR last_name like '%" & Quote(m_strKeyWords) & "%' OR user_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "last_name") Then
            m_strSearchText = " (last_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "first_name") Then
            m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "user_name") Then
            m_strSearchText = " (user_name like '%" & Quote(m_strKeyWords) & "%')"
        End If
    End Sub
    Private Function Quote(ByVal KeyWords As String) As String
        Dim result As String = KeyWords
        If (KeyWords.Length > 0) Then
            result = KeyWords.Replace("'", "''")
        End If
        Return result
    End Function

    Private Sub DisplayUsers()
        Dim req As New UserRequestData
        If (Request.QueryString("OrderBy") = "") Then
            OrderBy = "user_name"
        Else
            OrderBy = Request.QueryString("OrderBy")
        End If
        If (m_intGroupId = 888888) Then
            GroupName = "All_Members"
        End If
        If (m_intGroupId <> 888888 Or m_intGroupId <> 2) Then
            usergroup_data = m_refUserApi.GetUserGroupById(m_intGroupId)
            If (Not (IsNothing(usergroup_data))) Then
                GroupName = usergroup_data.GroupName
            End If
        End If

        ltr_groupsubscription.Text = m_refUserApi.EkUserRef.IsGroupPartOfSubscriptionProduct(m_intGroupId).ToString().ToLower()

        m_intCurrentPage = Me.uxPaging.SelectedPage

        req.Type = IIf(m_intGroupType = 3, 0, m_intGroupType)
        req.Group = m_intGroupId
        req.RequiredFlag = m_intUserActiveFlag
        req.OrderBy = OrderBy
        req.OrderDirection = m_strDirection
        req.SearchText = m_strSearchText
        req.PageSize = m_refContentApi.RequestInformationRef.PagingSize
        req.CurrentPage = m_intCurrentPage + 1
        user_list = m_refUserApi.GetAllUsers(req)
        m_intTotalPages = req.TotalPages
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
        ViewAllUsersToolBar()
        If Me.m_bCommunityGroup Then
            Populate_ViewCommunityMembersGrid(user_list)
        Else
            Populate_ViewAllUsersGrid(user_list)
        End If
    End Sub
    Private Sub Populate_ViewCommunityMembersGrid(ByVal data As UserData())
        Dim colBound As System.Web.UI.WebControls.BoundColumn
        Dim sAppend As String = ""
        Dim Icon As String = "user.png"
        Dim m_strTyAction As String = ""
        If (m_intGroupType = 1) Then
            Icon = "userMembership.png"
        End If
        If ((Not IsNothing(Request.QueryString("ty"))) AndAlso (Request.QueryString("ty") = "nonverify")) Then
            m_strTyAction = "&ty=nonverify"
        End If
        Dim HeaderText As String = "<a href=""users.aspx?action=viewallusers&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId & "&direction=" & m_strDirection & "&OrderBy={0}&LangType=" & ContentLanguage & "&id=" & uId & (IIf(FromUsers = "", "", "&FromUsers=" & CStr(FromUsers))) & m_strTyAction & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>{1}</a>"

        If (m_intUserActiveFlag = -1 Or Me.m_bCommunityGroup) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "CHECKL"
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            colBound.HeaderStyle.Width = Unit.Percentage(5)
            colBound.ItemStyle.Width = Unit.Percentage(5)
            MapCMSUserToADGrid.Columns.Add(colBound)
        End If

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LEFT"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.Width = Unit.Percentage(45)
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        MapCMSUserToADGrid.Columns.Add(colBound)

        If (m_intUserActiveFlag = -1 Or Me.m_bCommunityGroup) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "CHECKR"
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            colBound.ItemStyle.Width = Unit.Percentage(5)
            MapCMSUserToADGrid.Columns.Add(colBound)
        End If

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "RIGHT"
        colBound.ItemStyle.Width = Unit.Percentage(45)
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.Wrap = False
        MapCMSUserToADGrid.Columns.Add(colBound)

        'PageSettings()

        Dim dt As New DataTable
        Dim dr As DataRow
        If (m_intUserActiveFlag = -1 Or m_intGroupId = 2 Or m_intGroupId = 888888 Or Me.m_bCommunityGroup) Then
            dt.Columns.Add(New DataColumn("CHECKL", GetType(String)))
        End If
        dt.Columns.Add(New DataColumn("LEFT", GetType(String)))
        If (m_intUserActiveFlag = -1 Or m_intGroupId = 2 Or m_intGroupId = 888888 Or Me.m_bCommunityGroup) Then
            dt.Columns.Add(New DataColumn("CHECKR", GetType(String)))
        End If
        dt.Columns.Add(New DataColumn("RIGHT", GetType(String)))
        Dim i As Integer = 0
        If (Not (IsNothing(data))) Then
            For i = 0 To data.Length - 1
                dr = dt.NewRow
                sAppend = ""
                If (setting_data.ADAuthentication = 1) And (data(i).Domain <> "") Then
                    sAppend = "@" & data(i).Domain
                End If
                If (m_intUserActiveFlag = -1 Or Me.m_bCommunityGroup) Then
                    dr("CHECKL") = "<input type=""checkbox"" name=""req_deleted_users"" id=""req_deleted_users"" value=""" & data(i).Id & """ onclick=""checkAll('req_deleted_users');"">"
                End If
                dr("LEFT") = "<img align=""left"" src=""" & IIf(data(i).Avatar <> "", data(i).Avatar, Me.m_refContentApi.AppPath & "images/UI/Icons/user.png") & """ />" & data(i).DisplayName
                If i < (data.Length - 1) Then
                    i = i + 1
                    sAppend = ""
                    If (setting_data.ADAuthentication = 1) And (data(i).Domain <> "") Then
                        sAppend = "@" & data(i).Domain
                    End If
                    If (m_intUserActiveFlag = -1 Or Me.m_bCommunityGroup) Then
                        dr("CHECKR") = "<input type=""checkbox"" name=""req_deleted_users"" id=""req_deleted_users"" value=""" & data(i).Id & """ onclick=""checkAll('req_deleted_users');"">"
                    End If
                    dr("RIGHT") = "<img align=""left"" src=""" & Me.m_refContentApi.AppPath & "images/UI/Icons/user.png""/>" & data(i).DisplayName

                End If
                dt.Rows.Add(dr)
            Next
        End If

        Dim dv As New DataView(dt)
        MapCMSUserToADGrid.PageSize = Me.m_refContentApi.RequestInformationRef.PagingSize
        MapCMSUserToADGrid.DataSource = dv
        MapCMSUserToADGrid.CurrentPageIndex = m_intCurrentPage
        MapCMSUserToADGrid.DataBind()

        If m_intTotalPages > 1 Then
            Me.uxPaging.Visible = True
            Me.uxPaging.TotalPages = m_intTotalPages
            Me.uxPaging.CurrentPageIndex = m_intCurrentPage
        Else
            Me.uxPaging.Visible = False
        End If

    End Sub
    Private Sub Populate_ViewAllUsersGrid(ByVal data As UserData())
        Dim colBound As System.Web.UI.WebControls.BoundColumn
        Dim sAppend As String = ""
        Dim Icon As String = "user.png"
        Dim m_strTyAction As String = ""
        If (m_intGroupType = 1) Then
            Icon = "userMembership.png"
        End If
        If ((Not IsNothing(Request.QueryString("ty"))) AndAlso (Request.QueryString("ty") = "nonverify")) Then
            m_strTyAction = "&ty=nonverify"
        End If
        Dim HeaderText As String = "<a href=""users.aspx?action=viewallusers&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId & "&direction=" & m_strDirection & "&OrderBy={0}&LangType=" & ContentLanguage & "&id=" & uId & (IIf(FromUsers = "", "", "&FromUsers=" & CStr(FromUsers))) & m_strTyAction & """ title=""" & m_refMsg.GetMessage("click to sort msg") & """>{1}</a>"

        If (m_intUserActiveFlag = -1 Or m_intGroupId = 2 Or m_intGroupId = 888888 Or Me.m_bCommunityGroup) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "CHECK"
            colBound.HeaderText = "<input type=checkbox name=checkall id=checkall onclick=""checkAll('');"">"
            colBound.ItemStyle.Wrap = False
            colBound.HeaderStyle.Width = Unit.Percentage(5)
            colBound.ItemStyle.Width = Unit.Percentage(5)
            MapCMSUserToADGrid.Columns.Add(colBound)
        End If

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "USERNAME"
        colBound.HeaderText = String.Format(HeaderText, "user_name", m_refMsg.GetMessage("generic Username"))
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(23)
        colBound.ItemStyle.Width = Unit.Percentage(23)
        MapCMSUserToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LASTNAME"
        colBound.HeaderText = String.Format(HeaderText, "last_name", m_refMsg.GetMessage("generic lastname"))
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(23)
        colBound.ItemStyle.Width = Unit.Percentage(23)
        MapCMSUserToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "FirstName"
        colBound.HeaderText = String.Format(HeaderText, "first_name", m_refMsg.GetMessage("generic firstname"))
        colBound.HeaderStyle.Width = Unit.Percentage(23)
        colBound.ItemStyle.Width = Unit.Percentage(23)
        colBound.ItemStyle.Wrap = False
        MapCMSUserToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LANGUAGE"
        colBound.HeaderText = m_refMsg.GetMessage("generic Language") 'String.Format(HeaderText, "language", m_refMsg.GetMessage("generic Language"))
        colBound.ItemStyle.Wrap = False
        MapCMSUserToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LASTLOGINDATE"
        colBound.HeaderText = m_refMsg.GetMessage("generic lastlogindate") 'String.Format(HeaderText, "last_login_date", m_refMsg.GetMessage("generic lastlogindate"))
        colBound.ItemStyle.Wrap = False
        MapCMSUserToADGrid.Columns.Add(colBound)

        If (m_intGroupType = 1) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "DATECREATED"
            colBound.HeaderText = m_refMsg.GetMessage("generic datecreated") 'String.Format(HeaderText, "date_created", m_refMsg.GetMessage("generic datecreated"))
            colBound.ItemStyle.Wrap = False
            MapCMSUserToADGrid.Columns.Add(colBound)
        Else
            If (m_refMailMsg.IsLoggedInUsersEmailValid()) Then
                colBound = New System.Web.UI.WebControls.BoundColumn
                colBound.DataField = "EMAILAREA"
                colBound.HeaderText = "<a href=""#"" onclick=""ToggleEmailCheckboxes();"" title=""" & m_refMsg.GetMessage("alt send email to all") & """><input type=""checkbox""></a>&nbsp;" & m_refMsg.GetMessage("generic all")
                colBound.ItemStyle.Wrap = False
                MapCMSUserToADGrid.Columns.Add(colBound)
            End If
        End If

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ACCOUNTLOCK"
        colBound.HeaderText = m_refMsg.GetMessage("generic locked") 'String.Format(HeaderText, "last_login_date", m_refMsg.GetMessage("generic lastlogindate"))
        colBound.ItemStyle.Wrap = False
        MapCMSUserToADGrid.Columns.Add(colBound)

        'PageSettings()

        Dim dt As New DataTable
        Dim dr As DataRow
        If (m_intUserActiveFlag = -1 Or m_intGroupId = 2 Or m_intGroupId = 888888 Or Me.m_bCommunityGroup) Then
            dt.Columns.Add(New DataColumn("CHECK", GetType(String)))
        End If
        dt.Columns.Add(New DataColumn("USERNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("LASTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("FIRSTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        dt.Columns.Add(New DataColumn("LASTLOGINDATE", GetType(String)))
        If (m_intGroupType = 1) Then
            dt.Columns.Add(New DataColumn("DATECREATED", GetType(String)))
        Else
            If (m_refMailMsg.IsLoggedInUsersEmailValid()) Then
                dt.Columns.Add(New DataColumn("EMAILAREA", GetType(String)))
            End If
        End If
        dt.Columns.Add(New DataColumn("ACCOUNTLOCK", GetType(String)))
        Dim i As Integer = 0
        If (Not (IsNothing(data))) Then
            For i = 0 To data.Length - 1
                dr = dt.NewRow
                sAppend = ""
                If (setting_data.ADAuthentication = 1) And (data(i).Domain <> "") Then
                    sAppend = "@" & data(i).Domain
                End If
                If ((m_intUserActiveFlag = -1 Or m_intGroupId = 2 Or m_intGroupId = 888888 Or Me.m_bCommunityGroup) AndAlso (Not (data(i).Id = m_refUserApi.UserId OrElse data(i).Id = 1 OrElse data(i).Id = 999999999))) Then
                    dr("CHECK") = "<input type=""checkbox"" name=""req_deleted_users"" id=""req_deleted_users"" value=""" & data(i).Id & """ onclick=""checkAll('req_deleted_users');"">"
                End If

                Dim AltText As String = ""
                If data(i).Domain <> "" Then
                    AltText = m_refMsg.GetMessage("view information on msg") & " " & data(i).Username & "@" & data(i).Domain
                Else
                    AltText = m_refMsg.GetMessage("view information on msg") & " " & data(i).DisplayUserName
                End If

                If (m_intUserActiveFlag = -1) Then
                    dr("USERNAME") = "<a href=""users.aspx?action=View&ty=nonverify&LangType=" & ContentLanguage & "&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&id=" & data(i).Id & "&FromUsers=" & FromUsers & "&OrderBy=" & OrderBy & """ title=""" & AltText & """><img src=""" & AppPath & "images/UI/Icons/" & Icon & """ border=""0"" align=""absbottom"" title=""" & AltText & """ alt=""" & AltText & """></a> <a href=""users.aspx?action=View&ty=nonverify&LangType=" & ContentLanguage & "&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&id=" & data(i).Id & "&FromUsers=" & FromUsers & "&OrderBy=" & OrderBy & """ title=""" & AltText & """>" & data(i).Username & sAppend & "</a>"
                Else
                    dr("USERNAME") = "<a href=""users.aspx?action=View&LangType=" & ContentLanguage & "&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&id=" & data(i).Id & "&FromUsers=" & FromUsers & "&OrderBy=" & OrderBy & """ title=""" & AltText & """><img src=""" & AppPath & "images/UI/Icons/" & Icon & """ border=""0"" align=""absbottom"" title=""" & AltText & """ alt=""" & AltText & """></a> <a href=""users.aspx?action=View&LangType=" & ContentLanguage & "&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&id=" & data(i).Id & "&FromUsers=" & FromUsers & "&OrderBy=" & OrderBy & """ title=""" & AltText & """>" & data(i).Username & sAppend & "</a>"
                End If

                dr("LASTNAME") = data(i).LastName
                dr("FIRSTNAME") = data(i).FirstName
                If (data(i).LanguageId = 0) Then
                    dr("LANGUAGE") = m_refMsg.GetMessage("app default msg")
                Else
                    dr("LANGUAGE") = data(i).LanguageName
                End If
                dr("LASTLOGINDATE") = data(i).LastLoginDate
                If (m_intGroupType = 1) Then
                    dr("DATECREATED") = data(i).DateCreated
                Else
                    If (m_refMailMsg.IsLoggedInUsersEmailValid()) Then
                        dr("EMAILAREA") = "<input type=""checkbox"" name=""emailcheckbox_" & data(i).Id & """ ID=""EmailTargetCheckboxes"">"
                        dr("EMAILAREA") += "<a href=""#"" onclick=""SelectEmail('emailcheckbox_" & data(i).Id & "');return false"">"
                        dr("EMAILAREA") += m_refMailMsg.MakeEmailGraphic() & "</a>"
                    End If
                End If
                dr("ACCOUNTLOCK") = "<input type=""checkbox"" name=""accLocked_" & data(i).Id & """ ID=""accLocked_" & data(i).Id & """ disabled "
                If (data(i).IsAccountLocked(Me.m_refContentApi.RequestInformationRef)) Then
                    dr("ACCOUNTLOCK") += " checked "
                End If
                dr("ACCOUNTLOCK") += " >"
                dt.Rows.Add(dr)
            Next
        End If

        Dim dv As New DataView(dt)
        MapCMSUserToADGrid.PageSize = Me.m_refContentApi.RequestInformationRef.PagingSize
        MapCMSUserToADGrid.DataSource = dv
        MapCMSUserToADGrid.CurrentPageIndex = m_intCurrentPage
        MapCMSUserToADGrid.DataBind()
        If m_intTotalPages > 1 Then
            Me.uxPaging.Visible = True
            Me.uxPaging.TotalPages = m_intTotalPages
            Me.uxPaging.CurrentPageIndex = m_intCurrentPage
        Else
            Me.uxPaging.Visible = False
        End If


    End Sub
    'Private Sub PageSettings()
    'If (m_intTotalPages <= 1) Then
    '    VisiblePageControls(False)
    'Else
    '    VisiblePageControls(True)
    '    TotalPages.Text = (System.Math.Ceiling(m_intTotalPages)).ToString()
    '    CurrentPage.Text = m_intCurrentPage.ToString()
    '    PreviousPage.Enabled = True
    '    FirstPage.Enabled = True
    '    NextPage.Enabled = True
    '    LastPage.Enabled = True
    '    If m_intCurrentPage = 1 Then
    '        PreviousPage.Enabled = False
    '        FirstPage.Enabled = False
    '    ElseIf m_intCurrentPage = m_intTotalPages Then
    '        NextPage.Enabled = False
    '        LastPage.Enabled = False
    '    End If
    'End If
    'End Sub
    'Private Sub VisiblePageControls(ByVal flag As Boolean)
    'TotalPages.Visible = flag
    'CurrentPage.Visible = flag
    'PreviousPage.Visible = flag
    'NextPage.Visible = flag
    'LastPage.Visible = flag
    'FirstPage.Visible = flag
    'PageLabel.Visible = flag
    'OfLabel.Visible = flag
    'End Sub
    Private Sub ViewAllUsersToolBar()
        Dim result As New System.Text.StringBuilder
        If (m_intUserActiveFlag = -1) Then
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view non verified users in group msg") & " """ & GroupName & """")
        Else
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view users in group msg") & " """ & GroupName & """")
        End If

        result.Append("<table width=""100%""><tr>")
        If (m_intGroupType = 0) Then ' cms authors
            If (m_intGroupId > 2) Then
                If (setting_data.ADIntegration = False) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&id=" & uId & "&OrderBy=" & OrderBy & "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add user"), ""))
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "users.aspx?action=EditUserGroup&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&Groupid=" & uId & "&OrderBy=" & OrderBy & "", m_refMsg.GetMessage("alt edit button text (user group)"), m_refMsg.GetMessage("btn edit"), ""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_adbrowse-nm.gif", "users.aspx?action=MapCMSUserGroupToAD&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&id=" & uId & "&rp=1", m_refMsg.GetMessage("alt browse button text (group)"), m_refMsg.GetMessage("btn ad browse"), ""))
                End If
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "users.aspx?action=DeleteGroup&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&Groupid=" & uId & "&OrderBy=" & OrderBy & "", m_refMsg.GetMessage("alt delete button text (user group)"), m_refMsg.GetMessage("btn delete"), "onclick=""return VerifyDeleteGroup();"""))
            ElseIf (m_intGroupId = 2) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "users.aspx?action=AddUserToSystem&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&id=" & uId & "&OrderBy=" & OrderBy & "&FromUsers=" & Request.QueryString("FromUsers"), m_refMsg.GetMessage("alt add button text (user3)"), m_refMsg.GetMessage("btn add user"), ""))
            Else
                If (setting_data.ADIntegration = False) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&id=" & uId & "&OrderBy=" & OrderBy & "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add user"), ""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_adbrowse-nm.gif", "users.aspx?action=MapCMSUserGroupToAD&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&id=" & uId & "&rp=1", m_refMsg.GetMessage("alt browse button text (group)"), m_refMsg.GetMessage("btn ad browse"), ""))
                End If
            End If
            'If FromUsers = "" Then
            '    result.Append(m_refStyle.GetButtonEventsWCaption(apppath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (user group)"), m_refMsg.GetMessage("btn save"), "onclick=""return document.forms[0].submit();"""))
            'End If
            If ((New EmailHelper).IsLoggedInUsersEmailValid()) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/email.png", "#", m_refMsg.GetMessage("alt send email to selected users"), m_refMsg.GetMessage("btn email"), "onclick=""LoadEmailChildPageEx();"""))
            End If
        ElseIf Me.m_bCommunityGroup And Me.m_iCommunityGroup > 0 Then ' community group
            If (setting_data.ADIntegration = False) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&id=" & uId & "&OrderBy=" & OrderBy & "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add user"), ""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_adbrowse-nm.gif", "users.aspx?action=MapCMSUserGroupToAD&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&id=" & uId & "&rp=1", m_refMsg.GetMessage("alt browse button text (group)"), m_refMsg.GetMessage("btn ad browse"), ""))
            End If
            If (setting_data.ADAutoUserToGroup And setting_data.ADIntegration = True And Me.m_refContentApi.RequestInformationRef.LDAPMembershipUser = True) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_adbrowse-nm.gif", "users.aspx?action=MapCMSUserGroupToAD&groupid=" & usergroup_data.GroupId & "&grouptype=" & m_intGroupType & "&rp=1", m_refMsg.GetMessage("alt browse button text (group)"), m_refMsg.GetMessage("btn ad browse"), ""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&id=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&OrderBy=" & OrderBy & "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add membership user"), ""))
            End If
            If ((New EmailHelper).IsLoggedInUsersEmailValid()) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/email.png", "#", m_refMsg.GetMessage("alt send email to selected users"), m_refMsg.GetMessage("btn email"), "onclick=""LoadEmailChildPageEx();"""))
            End If
        Else ' members
            If (m_intUserActiveFlag <> -1) Then
                If (m_intGroupId <> 888888) Then
                    If (setting_data.ADAutoUserToGroup And setting_data.ADIntegration = True And Me.m_refContentApi.RequestInformationRef.LDAPMembershipUser = True) Then
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_adbrowse-nm.gif", "users.aspx?action=MapCMSUserGroupToAD&groupid=" & usergroup_data.GroupId & "&grouptype=" & m_intGroupType & "&rp=1", m_refMsg.GetMessage("alt browse button text (group)"), m_refMsg.GetMessage("btn ad browse"), ""))
                    Else
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&id=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&OrderBy=" & OrderBy & "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add membership user"), ""))
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "users.aspx?action=EditUserGroup&Groupid=" & uId & "&grouptype=" & m_intGroupType & "&OrderBy=" & OrderBy & "", m_refMsg.GetMessage("alt edit button text (user group)"), m_refMsg.GetMessage("btn edit"), ""))
                    End If
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "users.aspx?action=DeleteGroup&Groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&OrderBy=" & OrderBy & "", m_refMsg.GetMessage("alt delete button text (user group)"), m_refMsg.GetMessage("btn delete"), "onclick="" return VerifyDeleteGroup();"""))
                ElseIf (m_intGroupId = 888888) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "users.aspx?action=AddUserToSystem&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId & "&OrderBy=" & OrderBy & "", m_refMsg.GetMessage("alt add button text (user3)"), m_refMsg.GetMessage("btn add membership user"), ""))

                Else
                    If (setting_data.ADIntegration = False) Then
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "users.aspx?action=AddUserToGroup&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId & "&OrderBy=" & OrderBy & "", m_refMsg.GetMessage("alt add button text (user2)"), m_refMsg.GetMessage("btn add membership user"), ""))
                    Else
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_adbrowse-nm.gif", "users.aspx?action=MapCMSUserGroupToAD&LangType=" & ContentLanguage & "&grouptype=" & m_intGroupType & "&groupid=" & usergroup_data.GroupId & "&rp=1", m_refMsg.GetMessage("alt browse button text (group)"), m_refMsg.GetMessage("btn ad browse"), ""))
                    End If
                End If
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/approvals.png", "#", m_refMsg.GetMessage("alt activate users"), m_refMsg.GetMessage("lbl activate users"), "onclick=""ActivateUsers();"""))
                'result.Append(m_refStyle.GetButtonEventsWCaption(apppath & "images/UI/Icons/delete.png", "#", "click here to delete selected users", m_refMsg.GetMessage("btn save"), "onclick=""DeleteSelectedUsers();"""))
            End If

        End If
        If (Not Me.m_bCommunityGroup) And (m_intUserActiveFlag = -1 Or m_intGroupId = 2 Or m_intGroupId = 888888) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt click here to delete selected users"), m_refMsg.GetMessage("btn delete"), "onclick=""DeleteSelectedUsers();"""))
        End If
        If m_strCallerPage <> "" Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", m_strCallerPage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            If Me.m_bCommunityGroup Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "Community/groups.aspx?action=viewgroup&id=" & Me.m_iCommunityGroup & "&LangType=" & Me.ContentLanguage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            ElseIf ((Not (m_intGroupType = 0 AndAlso m_intGroupId = 2 AndAlso Request.QueryString("FromUsers") = "1")) AndAlso (Request.QueryString("backaction") <> Request.QueryString("action"))) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "users.aspx?action=" & m_strBackAction & "&backaction=" & m_strBackAction & "&groupid=" & m_intGroupId & "&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            End If
        End If
        'If (m_intGroupType = 1) Then

        result.Append("<td width=""100%"" align=""right""> | <label for=""txtSearch"">" & m_refMsg.GetMessage("generic search") & "</label><input type=text class=""ektronTextMedium"" id=txtSearch name=txtSearch value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event)"" />")
        result.Append(" <select id=searchlist name=searchlist>")
        result.Append("     <option value=-1" & IsSelected("-1") & ">" & m_refMsg.GetMessage("generic all") & "</option>")
        result.Append("     <option value=""last_name""" & IsSelected("last_name") & ">" & m_refMsg.GetMessage("generic lastname") & "</option>")
        result.Append("     <option value=""first_name""" & IsSelected("first_name") & ">" & m_refMsg.GetMessage("generic firstname") & "</option>")
        result.Append("     <option value=""user_name""" & IsSelected("user_name") & ">" & m_refMsg.GetMessage("generic username") & "</option>")
        result.Append(" </select>")
        result.Append(" <input type=button value=" & m_refMsg.GetMessage("btn search") & " id=btnSearch name=btnSearch onclick=""searchuser();"" class=""ektronWorkareaSearch"" />")
        'End If

        'Help
        If m_intGroupType = 0 Then
            result.Append(m_refStyle.GetHelpButton("ViewUsersByGroupToolBar"))
        Else
            If (-1 = m_intUserActiveFlag) Then
                result.Append(m_refStyle.GetHelpButton("Viewnotverifiedusers"))
            Else
                result.Append(m_refStyle.GetHelpButton("ViewMembershipUsers"))
            End If
        End If

        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Function IsSelected(ByVal val As String) As String
        If (val = m_strSelectedItem) Then
            Return (" selected ")
        Else
            Return ("")
        End If
    End Function
#End Region

#Region "MapCMSUserToAD"
    Public Function MapCMSUserToAD() As Boolean
        search = Request.QueryString("search")
        If Not Page.IsPostBack Or (Page.IsPostBack And Request.Form(isPostData.UniqueID) <> "") Then
            Display_MapCMSUserToAD()
        ElseIf Page.IsPostBack Then
            Process_MapCMSUserToAD()
            Return (True)
        End If
    End Function
    Private Sub Process_MapCMSUserToAD()
        Dim uID As Long = CLng(Request.Form("id"))
        Dim tempArray As Object = Split(Request.Form("usernameanddomain"), "_@_")
        Dim strUserName As String = CStr(tempArray(0))
        Dim strDomain As String = CStr(tempArray(1))
        m_refUserApi.RemapCMSUserToAD(uID, strUserName, strDomain, 0)
        Dim returnPage As String = ""
        If (Request.Form("rp") = "1") Then
            returnPage = "users.aspx?action=View&id=" & uID
        Else
            returnPage = "adreports.aspx?action=SynchUsers&ReportType=" & Request.Form("rt")
        End If
        Response.Redirect(returnPage, False)
    End Sub
    Private Sub Display_MapCMSUserToAD()
        AppImgPath = m_refSiteApi.AppImgPath
        f = Request.QueryString("f")
        rp = Request.QueryString("rp")
        e1 = Request.QueryString("e1")
        e2 = Request.QueryString("e2")
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
        If (Not (IsNothing(Request.QueryString("id")))) Then
            If (Request.QueryString("id") <> "") Then
                uId = Request.QueryString("id")
            End If
            If (uId = -1) Then
                uId = Request.Form("id")
            End If
        End If
        user_data = m_refUserApi.GetUserById(uId)
        security_data = m_refContentApi.LoadPermissions(0, "content")
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)

        If ((setting_data.ADAuthentication = 1) And (search = "" Or search = "0")) Then
            PostBackPage.Text = Utilities.SetPostBackPage("users.aspx?Action=MapCMSUserToAD&Search=1&LangType=" & ContentLanguage & "&rp=" & rp & "&e1=" & e1 & "&e2=" & e2 & "&f=" & f & "&id=" & uId)
            domain_data = m_refUserApi.GetDomains(0, 0)
            'TOOLBAR
            Dim result As New System.Text.StringBuilder
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("search ad for cms user") & " """ & user_data.DisplayUserName & """")
            result.Append("<table><tr>")
            If (rp <> "1") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("btn cancel"), "onclick=""top.close();"""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            End If
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
            Populate_MapCMSUserToADGrid()
        Else
            Dim Domain As String = ""
            Dim Sort As String = ""

            Dim sdAttributes As New System.Collections.Specialized.NameValueCollection 'New Collection
            Dim sdFilter As New System.Collections.Specialized.NameValueCollection  'New Collection

            sdAttributes.Add("UserName", "UserName")
            sdAttributes.Add("FirstName", "FirstName")
            sdAttributes.Add("LastName", "LastName")
            sdAttributes.Add("Domain", "Domain")

            If (search = "1") Then
                UserName = Request.Form("username")
                FirstName = Request.Form("firstname")
                LastName = Request.Form("lastname")
                Domain = Request.Form("domainname")
                Sort = "UserName"
            Else
                UserName = Request.QueryString("username")
                FirstName = Request.QueryString("firstname")
                LastName = Request.QueryString("lastname")
                Domain = Request.QueryString("domainname")
                Sort = Request.QueryString("OrderBy")
            End If

            If ((UserName = "") And (FirstName = "") And (LastName = "")) Then
                sdFilter.Add("UserName", "UserName")
                sdFilter.Add("UserNameValue", "*")
            Else
                If (UserName <> "") Then
                    sdFilter.Add("UserName", "UserName")
                    sdFilter.Add("UserNameValue", UserName) 'sdFilter.add (UserName,"UserNameValue")
                End If
                If (FirstName <> "") Then
                    sdFilter.Add("FirstName", "FirstName")
                    sdFilter.Add("FirstNameValue", FirstName)
                End If
                If (LastName <> "") Then
                    sdFilter.Add("LastName", "LastName")
                    sdFilter.Add("LastNameValue", LastName)
                End If
            End If
            Dim result_data As UserData()
            result_data = m_refUserApi.GetAvailableADUsers(sdAttributes, sdFilter, Sort, Domain)
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("search ad for cms user"))
            Dim result As New System.Text.StringBuilder
            result.Append("<table><tr>")
            If (Not (IsNothing(result_data))) Then
                If (rp = "1") Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "#", m_refMsg.GetMessage("alt update button text (associate user)"), m_refMsg.GetMessage("alt update button text (associate user)"), "onclick=""document.forms[0].user_isPostData.value=''; return SubmitForm('aduserinfo', 'CheckRadio(0);');"""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "#", m_refMsg.GetMessage("alt update button text (associate user)"), m_refMsg.GetMessage("alt update button text (associate user)"), "onclick=""document.forms[0].user_isPostData.value=''; return SubmitForm('aduserinfo', 'CheckReturn(0);');"""))
                End If
            End If
            If Request.ServerVariables("HTTP_USER_AGENT").ToLower().Contains("msie") Then 'defect 17586 - SMK
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "javascript:window.location.reload( false );", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            End If
            If (rp <> "1") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("btn cancel"), "onclick=""top.close();"""))
            End If
            result.Append("<td>")
            result.Append(m_refStyle.GetHelpButton("Display_MapCMSUserToAD"))
            result.Append("</td>")
            result.Append("</tr></table>")
            htmToolBar.InnerHtml = result.ToString
            Populate_MapCMSUserToADGrid_Search(result_data)
        End If
    End Sub
    Private Sub Populate_MapCMSUserToADGrid_Search(ByVal data As UserData())
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ADD"
        colBound.HeaderText = m_refMsg.GetMessage("add title")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(2)
        colBound.ItemStyle.Width = Unit.Percentage(2)
        MapCMSUserToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "USERNAME"
        colBound.HeaderText = "<a href=""users.aspx?action=AddUserToSystem&OrderBy=UserName&LangType=" & ContentLanguage & "&username=" & UserName & "&lastname=" & LastName & "&firstname=" & FirstName & "&id=" & uId & "&search=2"" title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Username") & "</a>"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(20)
        colBound.ItemStyle.Width = Unit.Percentage(20)
        MapCMSUserToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LASTNAME"
        colBound.HeaderText = "<a href=""users.aspx?action=AddUserToSystem&OrderBy=LastName&LangType=" & ContentLanguage & "&username=" & UserName & "&lastname=" & LastName & "&firstname=" & FirstName & "&id=" & uId & "&search=2"" title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Lastname") & "</a>"
        colBound.HeaderStyle.Width = Unit.Percentage(20)
        colBound.ItemStyle.Width = Unit.Percentage(20)
        colBound.ItemStyle.Wrap = False
        MapCMSUserToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "FIRSTNAME"
        colBound.HeaderText = "<a href=""users.aspx?action=AddUserToSystem&OrderBy=FirstName&LangType=" & ContentLanguage & "&username=" & UserName & "&lastname=" & LastName & "&firstname=" & FirstName & "&id=" & uId & "&search=2"" title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Firstname") & "</a>"
        colBound.ItemStyle.Wrap = False
        MapCMSUserToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = m_refMsg.GetMessage("domain title")
        colBound.ItemStyle.Wrap = False
        MapCMSUserToADGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("ADD", GetType(String)))
        dt.Columns.Add(New DataColumn("USERNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("LASTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("FIRSTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))

        Dim i As Integer = 0
        If (Not (IsNothing(data))) Then
            If data.Length > 0 Then
                ltr_message.Text = ""
                For i = 0 To data.Length - 1
                    dr = dt.NewRow
                    dr(0) = "<input type=""Radio"" name=""usernameanddomain"" value=""" & data(i).Username & "_@_" & data(i).Domain & """ onClick=""SetUp('" & data(i).Username.Replace("'", "\'") & "_@_" & data(i).Domain & "')"">"
                    dr(1) = data(i).Username
                    dr(2) = data(i).LastName
                    dr(3) = data(i).FirstName
                    dr(4) = data(i).Domain
                    dt.Rows.Add(dr)
                Next
            Else
                ltr_message.Text = "<br />" & m_refMsg.GetMessage("the search resulted in zero matches")
            End If
        Else
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("no ad users found")
            dr(1) = ""
            dr(2) = ""
            dr(3) = ""
            dt.Rows.Add(dr)
        End If
        dr = dt.NewRow
        Dim result As New System.Text.StringBuilder
        result.Append("<input type=""hidden"" name=""id"" value=""" & uId & """>")
        result.Append("<input type=""hidden"" name=""rp"" value=""" & rp & """>")
        result.Append("<input type=""hidden"" name=""e1"" value=""" & e1 & """>")
        result.Append("<input type=""hidden"" name=""e2"" value=""" & e2 & """>")
        result.Append("<input type=""hidden"" name=""f"" value=""" & f & """>")
        result.Append("<input type=""hidden"" name=""adusername"">")
        result.Append("<input type=""hidden"" name=""addomain"">")
        dr(0) = result.ToString
        dt.Rows.Add(dr)
        Dim dv As New DataView(dt)
        MapCMSUserToADGrid.DataSource = dv
        MapCMSUserToADGrid.DataBind()
    End Sub
    Private Sub Populate_MapCMSUserToADGrid()
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "USERNAME"
        colBound.HeaderText = m_refMsg.GetMessage("generic Username")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Height = Unit.Empty
        MapCMSUserToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "FIRSTNAME"
        colBound.HeaderText = m_refMsg.GetMessage("generic Firstname")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Height = Unit.Empty
        MapCMSUserToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LASTNAME"
        colBound.HeaderText = m_refMsg.GetMessage("generic Lastname")
        colBound.ItemStyle.Wrap = False
        MapCMSUserToADGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = m_refMsg.GetMessage("domain title")
        colBound.ItemStyle.Wrap = False
        MapCMSUserToADGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("USERNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("FIRSTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("LASTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))

        dr = dt.NewRow
        dr(0) = "<input type=""Text"" name=""username"" maxlength=""255"" class=""ektronTextXSmall"" OnKeyPress=""javascript:return CheckKeyValue(event,'34');"">"
        dr(1) = "<input type=""Text"" name=""firstname"" maxlength=""50"" class=""ektronTextXSmall"" OnKeyPress=""javascript:return CheckKeyValue(event,'34');"">"
        dr(2) = "<input type=""Text"" name=""lastname"" maxlength=""50"" class=""ektronTextXSmall"" OnKeyPress=""javascript:return CheckKeyValue(event,'34');"">"
        dr(2) += "<input type=""hidden"" id=""uid"" name=""uid"" value=""""> <input type=""hidden"" id=""rp"" name=""rp"" value="""">"
        dr(2) += "<input type=""hidden"" id=""ep"" name=""e1"" value=""""> <input type=""hidden"" id=""e2"" name=""e2"" value="""">"
        dr(2) += "<input type=""hidden"" id=""f"" name=""f"" value="""">"
        dr(3) = "<select name=""domainname"">"
        If (Not (IsNothing(domain_data))) AndAlso m_refContentApi.RequestInformationRef.ADAdvancedConfig = False Then
            dr(3) += "<option selected value="""">" & m_refMsg.GetMessage("all domain select caption") & "</option>"
        End If
        Dim i As Integer
        For i = 0 To domain_data.Length - 1
            dr(3) += "<option value=""" & domain_data(i).Name & """>" & domain_data(i).Name & "</option>"
        Next
        dr(3) += "</select>"
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = "<input type=""submit"" name=""search"" value=""" & m_refMsg.GetMessage("generic Search") & """>"
        dr(1) = ""
        dr(2) = ""
        dr(3) = ""
        dt.Rows.Add(dr)
        Dim dv As New DataView(dt)
        MapCMSUserToADGrid.DataSource = dv
        MapCMSUserToADGrid.DataBind()
    End Sub
#End Region

#Region "Grid Events"
    'Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
    '    Select Case e.CommandName
    '        Case "First"
    '            m_intCurrentPage = 1
    '        Case "Last"
    '            m_intCurrentPage = Int32.Parse(TotalPages.Text)
    '        Case "Next"
    '            m_intCurrentPage = Int32.Parse(CurrentPage.Text) + 1
    '        Case "Prev"
    '            m_intCurrentPage = Int32.Parse(CurrentPage.Text) - 1
    '    End Select
    '    CollectSearchText()
    '    DisplayUsers()
    '    isPostData.Value = "true"
    'End Sub
#End Region

    Private Function LDAPMembers() As Boolean
        If (m_intGroupType = 1) Then 'member
            Return (m_refUserApi.RequestInformationRef.LDAPMembershipUser)
        ElseIf (m_intGroupType = 0) Then 'CMS user
            Return True
        End If
    End Function
    Protected Sub RegisterResources()
        ' register JS
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS)
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareaSearchBoxInputLabelInitJS")
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)

        ' register CSS
        Ektron.Cms.API.Css.RegisterCss(Me, m_refContentApi.AppPath & "csslib/ActivityStream/activityStream.css", "ActivityStream")
    End Sub
    Private Sub LoadGrid(ByVal display As String)

        Dim activityListCriteria As New Criteria(Of Ektron.Cms.Activity.ActivityTypeProperty)
        activityListCriteria.OrderByDirection = OrderByDirection.Ascending

        If (display = "colleagues") Then
            activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.Colleague)
        Else
            activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.CommunityGroup)
        End If
        activityTypeList = _activityListApi.GetList(activityListCriteria)

        Dim dt As New Data.DataTable
        Dim dr As Data.DataRow
        dt.Columns.Add(New Data.DataColumn("EMPTY", GetType(String)))
        dt.Columns.Add(New Data.DataColumn("EMAIL", GetType(String)))
        dt.Columns.Add(New Data.DataColumn("SMS", GetType(String)))
        dt.Columns.Add(New Data.DataColumn("NEWSFEED", GetType(String)))
        LoadPreferenceList()
        For i As Integer = 0 To activityTypeList.Count - 1
            dr = dt.NewRow
            dr("EMPTY") = activityTypeList(i).Name
            If (preferenceList.Count > 0) Then
                For Each prefData In preferenceList
                    If (CompareIds(activityTypeList(i).Id, 1)) Then
                        dr("EMAIL") = "<center><input type=""Checkbox"" name = ""email" & activityTypeList(i).Id & """ id=""email" & activityTypeList(i).Id & """ checked=""checked"" DISABLED /></center>"
                    Else
                        dr("EMAIL") = "<center><input type=""Checkbox"" name = ""email" & activityTypeList(i).Id & """ id=""email" & activityTypeList(i).Id & """  DISABLED /></center>"
                    End If
                    If (CompareIds(activityTypeList(i).Id, 2)) Then
                        dr("NEWSFEED") = "<center><input type=""Checkbox"" name=""feed" & activityTypeList(i).Id & """ id=""feed" & activityTypeList(i).Id & """ checked=""checked"" DISABLED  /></center>"
                    Else
                        dr("NEWSFEED") = "<center><input type=""Checkbox"" name=""feed" & activityTypeList(i).Id & """ id=""feed" & activityTypeList(i).Id & """ DISABLED /></center>"

                    End If

                    If (CompareIds(activityTypeList(i).Id, 3)) Then
                        dr("SMS") = "<center><input type=""Checkbox"" name =""sms" & activityTypeList(i).Id & """ id=""sms" & activityTypeList(i).Id & """ checked=""checked"" DISABLED /></center>"
                    Else
                        dr("SMS") = "<center><input type=""Checkbox"" name =""sms" & activityTypeList(i).Id & """ id=""sms" & activityTypeList(i).Id & """ DISABLED /></center>"
                    End If

                Next
                dt.Rows.Add(dr)
            Else
                dr("EMAIL") = "<center><input type=""Checkbox"" name = ""email" & activityTypeList(i).Id & """ id=""email" & activityTypeList(i).Id & """/></center>"
                dr("SMS") = "<center><input type=""Checkbox"" name =""sms" & activityTypeList(i).Id & """ id=""sms" & activityTypeList(i).Id & """/></center>"
                dr("NEWSFEED") = "<center><input type=""Checkbox"" name=""feed" & activityTypeList(i).Id & """ id=""feed" & activityTypeList(i).Id & """/></center>"
                dt.Rows.Add(dr)
            End If

        Next
        Dim dv As New Data.DataView(dt)
        If (display = "colleagues") Then
            CollGrid.DataSource = dv
            CollGrid.DataBind()
        Else
            GroupGrid.DataSource = dv
            GroupGrid.DataBind()
        End If

    End Sub
    Private Function CompareIds(ByVal prefActivityTypeId As Long, ByVal prefAgentId As Long) As Boolean
        Dim result As Boolean = False
        For Each prefData In preferenceList
            If (prefData.ActivityTypeId = prefActivityTypeId AndAlso prefAgentId = prefData.AgentId) Then
                Return True
            End If
        Next
        Return False
    End Function
    Private Sub LoadPreferenceList()
        Dim groupPrefList As System.Collections.Generic.List(Of NotificationPreferenceData)
        Dim criteria As New Criteria(Of NotificationPreferenceProperty)
        criteria.PagingInfo.RecordsPerPage = 1000
        criteria.AddFilter(NotificationPreferenceProperty.UserId, CriteriaFilterOperator.EqualTo, uId)
        'Getting the Group Preference list
        groupPrefList = _notificationPreferenceApi.GetDefaultPreferenceList(criteria)
        'need to set source to 0 because we dont want individual group prefs.
        criteria.AddFilter(NotificationPreferenceProperty.ActionSourceId, CriteriaFilterOperator.EqualTo, 0)
        'Getting the Colleagues preference list  
        preferenceList = _notificationPreferenceApi.GetList(criteria)

        'Adding the group list to Preferences
        preferenceList.AddRange(groupPrefList)
    End Sub
    Private Sub CreateColumns()
        Dim criteria As New Criteria(Of NotificationAgentProperty)
        criteria.AddFilter(NotificationAgentProperty.IsEnabled, CriteriaFilterOperator.EqualTo, True)
        agentList = _notificationAgentApi.GetList(criteria)

        If (agentList IsNot Nothing AndAlso agentList.Count > 0) Then
            CollGrid.Columns.Add(_refStyle.CreateBoundField("EMPTY", "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), False, False))
            GroupGrid.Columns.Add(_refStyle.CreateBoundField("EMPTY", "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), False, False))
            For Each agentData In agentList
                If agentData.IsEnabled Then
                    Select Case (agentData.Id)
                        Case 1
                            CollGrid.Columns.Add(_refStyle.CreateBoundField("EMAIL", "<center>" + m_refMsg.GetMessage("sync conflict email") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                            GroupGrid.Columns.Add(_refStyle.CreateBoundField("EMAIL", "<center>" + m_refMsg.GetMessage("sync conflict email") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                        Case 2
                            CollGrid.Columns.Add(_refStyle.CreateBoundField("NEWSFEED", "<center>" + m_refMsg.GetMessage("colheader newsfeed") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                            GroupGrid.Columns.Add(_refStyle.CreateBoundField("NEWSFEED", "<center>" + m_refMsg.GetMessage("colheader newsfeed") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                        Case 3
                            CollGrid.Columns.Add(_refStyle.CreateBoundField("SMS", "<center>" + m_refMsg.GetMessage("colheader sms") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                            GroupGrid.Columns.Add(_refStyle.CreateBoundField("SMS", "<center>" + m_refMsg.GetMessage("colheader sms") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                    End Select
                End If
            Next
        End If
    End Sub

    Private Sub ViewUserPublishPreferences()
        Dim _publishPrefApi As New Ektron.Cms.Framework.Notifications.NotificationPublishPreference
        Dim publishPrefList As New System.Collections.Generic.List(Of NotificationPublishPreferenceData)
        Dim prefEntry As NotificationPublishPreferenceData
        publishPrefList = _publishPrefApi.GetList(uId)
        publishPrefList.Sort(New NotificationPublishPreferenceData)
        PrivacyGrid.Columns.Add(_refStyle.CreateBoundField("TYPE", m_refMsg.GetMessage("generic actions"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), False, False))
        PrivacyGrid.Columns.Add(_refStyle.CreateBoundField("ENABLED", "<center>" + m_refMsg.GetMessage("generic publish") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        Dim dt As New Data.DataTable
        Dim dr As Data.DataRow
        dt.Columns.Add(New Data.DataColumn("TYPE", GetType(String)))
        dt.Columns.Add(New Data.DataColumn("ENABLED", GetType(String)))
        For Each prefEntry In publishPrefList
            dr = dt.NewRow
            dr("TYPE") = prefEntry.ActivityTypeName
            If (prefEntry.IsEnabled) Then
                dr("ENABLED") = "<center><input type=""Checkbox"" name=""pref" & prefEntry.ActivityTypeId & """ id=""pref" & prefEntry.ActivityTypeId & """ checked=""checked"" DISABLED /></center>"
            Else
                dr("ENABLED") = "<center><input type=""Checkbox"" name=""pref" & prefEntry.ActivityTypeId & """ id=""pref" & prefEntry.ActivityTypeId & """ DISABLED /></center>"
            End If
            dt.Rows.Add(dr)
        Next
        Dim dv As New Data.DataView(dt)
        PrivacyGrid.DataSource = dv
        PrivacyGrid.DataBind()
    End Sub
    Private Sub LoadCommunityAliasTab()
        Dim _communityAlias As New Ektron.Cms.API.UrlAliasing.UrlAliasCommunity
        Dim aliasList As System.Collections.Generic.List(Of Ektron.Cms.Common.UrlAliasCommunityData)

        aliasList = _communityAlias.GetListUser(uId)
        If (aliasList.Count > 0) Then
            For Each item As Ektron.Cms.Common.UrlAliasCommunityData In aliasList
                groupAliasList += "<a href= " & Me.m_refContentApi.SitePath & item.AliasName & " target=_blank>" & Me.m_refContentApi.SitePath & item.AliasName & "</a>"
                groupAliasList += "<br/>"
            Next
        Else
            aliasTab.Visible = False
            tblaliasList.Visible = False
        End If
    End Sub
End Class
