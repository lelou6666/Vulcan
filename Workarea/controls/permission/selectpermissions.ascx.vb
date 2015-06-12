Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports System.Collections.Generic

Public Class ApprovedUsersAndGroups

#Region "members"

    Private _id As Long
    Private _userOrGroup As String
    Private _cmsOrMembership As String

#End Region

#Region "properties"

    Public Property ID() As Long
        Get
            Return _id
        End Get
        Set(ByVal value As Long)
            _id = value
        End Set
    End Property

    Public Property UserOrGroup() As String
        Get
            Return _userOrGroup
        End Get
        Set(ByVal value As String)
            _userOrGroup = value
        End Set
    End Property

    Public Property CmsOrMembership() As String
        Get
            Return _cmsOrMembership
        End Get
        Set(ByVal value As String)
            _cmsOrMembership = value
        End Set
    End Property

#End Region

End Class

Partial Class selectpermissions
    Inherits System.Web.UI.UserControl

#Region "enums"

    Private Enum FilterType
        Group
        User
    End Enum

#End Region

#Region "members"

    Private _ContentApi As New ContentAPI
    Private _StyleHelper As New StyleHelper
    Private _MessageHelper As Ektron.Cms.Common.EkMessageHelper
    Private _Id As Long = 0
    Private _FolderData As FolderData
    Private _PermissionData As PermissionData
    Private _AppImgPath As String = ""
    Private _ContentType As Integer = 1
    Private _CurrentUserId As Long = 0
    Private _PageData As Collection
    Private _PageAction As String = ""
    Private _OrderBy As String = ""
    Private _ContentLanguage As Integer = -1
    Private _EnableMultilingual As Integer = 0
    Private _ItemType As String = ""
    Private _ContentData As ContentData
    Private _UserIcon As String = ""
    Private _GroupIcon As String = ""
    Private _IsMembership As Boolean = False
    Private _Base As String = ""
    Private _IsDiscussionBoardOrDiscussionForum As Boolean = False
    Private _CurrentPage As Integer = 1
    Private _TotalPages As Integer = 1
    Private _PermissionSelectType As String = "1"
    Private m_strAction As String = ""
    Private m_strKeyWords As String = ""
    Private m_strSearchText As String = ""
    Private role_request As RoleRequest
    Private _UserApi As New UserAPI
    Private OrderBy As String = ""
    Private m_intGroupId As Long = -1
    Private user_list As UserData()
    Private _UserGroupData() As UserGroupData
    Private _ApplicationPath As String
    Private _SitePath As String
    Private _IsSearch As Boolean = False
    Private _SearchText As String
    Private _AssignedUsers() As String

#End Region

#Region "properties"

    Public Property SitePath() As String
        Get
            Return _SitePath
        End Get
        Set(ByVal Value As String)
            _SitePath = Value
        End Set
    End Property

    Public Property ApplicationPath() As String
        Get
            Return _ApplicationPath
        End Get
        Set(ByVal Value As String)
            _ApplicationPath = Value
        End Set
    End Property

#End Region

#Region "events"

    Public Sub New()

        Dim endSlash() As Char = {"/"}
        Me.ApplicationPath = _ContentApi.ApplicationPath.TrimEnd(endSlash)
        Me.SitePath = _ContentApi.SitePath.TrimEnd(endSlash)
        _MessageHelper = _ContentApi.EkMsgRef

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'set  up page data
        SetMemberValues()

        'register page components
        RegisterJS()
        RegisterCSS()

    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If hdnStopExecution.Value = "false" Then
            hdnStopExecution.Value = ""
            Exit Sub
        End If
        'set selected users and groups to viewstate
        Me.hdnApprovedUsersAndGroups.Value = Request.Form(Me.hdnApprovedUsersAndGroups.UniqueID)

        'determine if user is searching or loading all users and groups
        _IsSearch = IIf(Request.Form("txtSearch") <> String.Empty, True, False)
        _IsSearch = IIf(hdnRetrievalMode.Value = "search", True, _IsSearch)
        _SearchText = IIf(_IsSearch, Request.Form("txtSearch"), String.Empty)
        _SearchText = IIf(_SearchText = String.Empty, Me.hdnSearchTerms.Value, _SearchText)
        _PermissionSelectType = IIf(_IsSearch, Convert.ToString(Request.Form("selecttype")), "1")
        
        'set retrieval mode and type
        Me.hdnRetrievalMode.Value = IIf(_IsSearch, "search", "full")
        Me.hdnUserOrGroups.Value = IIf((_PermissionSelectType = "2" Or _PermissionSelectType = "4"), "user", "group")
        Me.hdnSearchTerms.Value = _SearchText
        If (Request.QueryString("selectType") <> "" And Request.QueryString("selectType") IsNot Nothing) Then
            If Request.QueryString("selectType") = -1 Then
                _PermissionSelectType = 1
            Else
                _PermissionSelectType = Request.QueryString("selectType")
            End If

        End If
        'set up datagrid
        _CurrentPage = Me.uxPaging.SelectedPage
        Me.GetPermissionData()
        Me.uxPermissionsGrid.DataSource = _UserGroupData
        Me.uxPermissionsGrid.DataBind()
        

        If _TotalPages > 1 Then
            Me.uxPaging.TotalPages = _TotalPages
            Me.uxPaging.CurrentPageIndex = Me._CurrentPage
        Else
            Me.uxPaging.Visible = False
        End If

        'set toolbar
        SelectPermissionsToolBar()
    End Sub

    Protected Sub uxPermissionsGrid_OnItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)

        Dim userGroupDataItem As UserGroupData = DirectCast(e.Item.DataItem, UserGroupData)
        If userGroupDataItem IsNot Nothing Then
            Select Case e.Item.ItemType
                Case ListItemType.Header
                    CType(e.Item.Cells(0).FindControl("uxSelectAll"), HtmlInputCheckBox).Attributes.Add("title", "Select All")
                    CType(e.Item.Cells(1).FindControl("uxNameHeader"), Literal).Text = "User or Group Name"
                    CType(e.Item.Cells(2).FindControl("uxTypeHeader"), Literal).Text = "User or Group Type"
                Case ListItemType.Item, ListItemType.AlternatingItem
                    CType(e.Item.Cells(0).FindControl("uxId"), HtmlInputControl).Value = GetUserOrGroupId(userGroupDataItem)
                    CType(e.Item.Cells(0).FindControl("uxIsGroup"), HtmlInputControl).Value = GetIsGroup(userGroupDataItem)
                    CType(e.Item.Cells(1).FindControl("uxIcon"), Image).ImageUrl = GetUserOrGroupIcon(userGroupDataItem)
                    CType(e.Item.Cells(1).FindControl("uxIcon"), Image).Attributes.Add("style", "vertical-align:text-top;")
                    CType(e.Item.Cells(1).FindControl("uxName"), HyperLink).Text = GetUserOrGroupName(userGroupDataItem)
                    CType(e.Item.Cells(1).FindControl("uxName"), HyperLink).ToolTip = "Set permissions for " & GetUserOrGroupName(userGroupDataItem)
                    CType(e.Item.Cells(1).FindControl("uxName"), HyperLink).NavigateUrl = GetUserOrGroupLink(userGroupDataItem)
                    CType(e.Item.Cells(2).FindControl("uxType"), Literal).Text = GetUserOrGroupType(userGroupDataItem)
            End Select
        ElseIf e.Item.ItemType = ListItemType.Header Then
            CType(e.Item.Cells(0).FindControl("uxSelectAll"), HtmlInputCheckBox).Attributes.Add("title", "Select All")
            CType(e.Item.Cells(1).FindControl("uxNameHeader"), Literal).Text = "User or Group Name"
            CType(e.Item.Cells(2).FindControl("uxTypeHeader"), Literal).Text = "User or Group Type"
        Else
            e.Item.Visible = False
        End If
    End Sub

#End Region

#Region "helpers"

    Private Sub SetMemberValues()
        If (Not (Request.QueryString("type") Is Nothing)) Then
            _ItemType = Convert.ToString(Request.QueryString("type")).Trim.ToLower
        End If
        If (Not (Request.QueryString("id") Is Nothing)) Then
            _Id = Convert.ToInt64(Request.QueryString("id"))
        End If
        If (Not (Request.QueryString("action") Is Nothing)) Then
            _PageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If
        If (Not (Request.QueryString("orderby") Is Nothing)) Then
            _OrderBy = Convert.ToString(Request.QueryString("orderby"))
        End If
        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            If (Request.QueryString("LangType") <> "") Then
                _ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                _ContentApi.SetCookieValue("LastValidLanguageID", _ContentLanguage)
            Else
                If _ContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    _ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If _ContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                _ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If
        If _ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            _ContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            _ContentApi.ContentLanguage = _ContentLanguage
        End If
        If (Not (Request.QueryString("membership") Is Nothing)) Then
            If (Request.QueryString("membership").Trim.ToLower <> "") Then
                _IsMembership = Convert.ToBoolean(Request.QueryString("membership").Trim.ToLower)
            End If
        End If

        If (_IsMembership) Then
            _UserIcon = "userMembership.png"
            _GroupIcon = "usersMembership.png"
        Else
            _UserIcon = "user.png"
            _GroupIcon = "users.png"
        End If

        If (Not (Request.QueryString("base") Is Nothing)) Then
            _Base = Request.QueryString("base").Trim.ToLower
        End If

        _CurrentUserId = _ContentApi.UserId
        _AppImgPath = _ContentApi.AppImgPath
        _EnableMultilingual = _ContentApi.EnableMultilingual

    End Sub

    Private Function GetUserOrGroupId(ByVal userGroupDataItem As UserGroupData) As String
        Return IIf(userGroupDataItem.GroupId <> -1, userGroupDataItem.GroupId.ToString(), userGroupDataItem.UserId.ToString())
    End Function

    Private Function GetIsGroup(ByVal userGroupDataItem As UserGroupData) As String
        Return IIf(userGroupDataItem.GroupId <> -1, "true", "false")
    End Function

    Private Function GetUserOrGroupIcon(ByVal userGroupDataItem As UserGroupData) As String
        Dim isGroup As Boolean = IIf(userGroupDataItem.GroupId <> -1, True, False)
        Return Me.ApplicationPath & "/images/ui/icons/" & IIf(isGroup, _GroupIcon, _UserIcon)
    End Function

    Private Function GetUserOrGroupName(ByVal userGroupDataItem As UserGroupData) As String
        Return IIf(userGroupDataItem.GroupId <> -1, userGroupDataItem.GroupName, userGroupDataItem.UserName)
    End Function

    Private Function GetUserOrGroupLink(ByVal userGroupDataItem As UserGroupData) As String
        Dim group As String = Me.ApplicationPath & "/content.aspx?LangType=" & _ContentLanguage & "&action=AddPermissions&id=" & _Id & "&type=" & _ItemType & "&base=group&PermID=" & userGroupDataItem.GroupId & "&membership=" & _IsMembership.ToString()
        Dim user As String = Me.ApplicationPath & "/content.aspx?LangType=" & _ContentLanguage & "&action=AddPermissions&id=" & _Id & "&type=" & _ItemType & "&base=user&PermID=" & userGroupDataItem.UserId & "&membership=" & _IsMembership.ToString()
        Return IIf(userGroupDataItem.GroupId <> -1, group, user)
    End Function

    Private Function GetUserOrGroupType(ByVal userGroupDataItem As UserGroupData) As String
        Dim type As String
        type = IIf(userGroupDataItem.IsMemberShipUser, "Membership ", "Cms ")
        type = IIf(userGroupDataItem.IsMemberShipGroup, "Membership ", type)
        Return IIf(userGroupDataItem.GroupId <> -1, type & "Group", type & "User")
    End Function

    Private Sub GetPermissionData()
        If _IsSearch Then
            GetPermissionDataSearch()
        Else
            GetPermissionDataAll()
        End If

    End Sub

    Private Sub GetPermissionDataAll()

        'set paging size
        Dim pageInfo As New Ektron.Cms.PagingInfo
        pageInfo.CurrentPage = _CurrentPage + 1
        pageInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize

        Select Case _PermissionSelectType
            Case 1 'Default
                Dim permissionUserType As Integer = IIf(_IsMembership, ContentAPI.PermissionUserType.Membership, ContentAPI.PermissionUserType.Cms)
                _UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", "All", permissionUserType, ContentAPI.PermissionRequestType.UnAssigned, pageInfo)
            Case 2 'CMS User
                _UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", "Users", ContentAPI.PermissionUserType.Cms, ContentAPI.PermissionRequestType.UnAssigned, pageInfo)
                'UserGroupDataFilter(FilterType.User)
            Case 3 'CMS Group
                _UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", "Groups", ContentAPI.PermissionUserType.Cms, ContentAPI.PermissionRequestType.UnAssigned, pageInfo)
                'UserGroupDataFilter(FilterType.Group)
            Case 4 'Member User
                _UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", "Users", ContentAPI.PermissionUserType.Membership, ContentAPI.PermissionRequestType.UnAssigned, pageInfo)
                'UserGroupDataFilter(FilterType.User)
            Case 5 'Member Group
                _UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", "Groups", ContentAPI.PermissionUserType.Membership, ContentAPI.PermissionRequestType.UnAssigned, pageInfo)
                'UserGroupDataFilter(FilterType.Group)
        End Select

        _TotalPages = pageInfo.TotalPages
    End Sub

    Private Sub GetPermissionDataSearch()
        'search for user or group

        Select Case _PermissionSelectType
            Case 1, 2, 4 'CMS or Membership User
                _UserGroupData = GetSearchedUsers()
            Case 3, 5 'CMS or Membership Group
                _UserGroupData = GetSearchedGroups()
        End Select

    End Sub

    Private Sub ValidatePermissions()
        If (_ContentApi.IsAdmin() OrElse _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers)) Then
            Return
        End If

        Dim id As Long
        If (Long.TryParse(Request.QueryString("id"), id) AndAlso _ContentApi.IsARoleMemberForFolder(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminFolderUsers, id, False)) Then
            Return
        End If

        EkException.ThrowException(New Ektron.Cms.Common.CmsException(_MessageHelper.GetMessage("com: user does not have permission")))
    End Sub

    Private Function GetSearchedUsers() As UserGroupData()
        ValidatePermissions()

        Dim userDataList As List(Of UserData)
        Dim userCriteria As New Ektron.Cms.Common.Criteria(Of Ektron.Cms.Common.UserProperty)
        Dim user As New UserData
        Dim userFrameworkApi As New Ektron.Cms.Framework.Users.User(Ektron.Cms.Framework.ApiAccessMode.Admin)
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim count As Integer = 0
        Dim userGroupData() As UserGroupData = Nothing
        Dim permissionUserType As Integer = IIf(_IsMembership, ContentAPI.PermissionUserType.Membership, ContentAPI.PermissionUserType.Cms)
        Dim isAssignedUser As Boolean = False
        Dim emptyUserGroupData() As Ektron.Cms.UserGroupData = Nothing
        Dim assignedUser As String = hdnAssignedUserGroupIds.Value
        _AssignedUsers = assignedUser.Split(",")
        'set paging size
        Dim pageInfo As New Ektron.Cms.PagingInfo
        If (Not Me._IsSearch) Then
            pageInfo.CurrentPage = _CurrentPage + 1
            pageInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize
        End If

        '_UserGroupData = _ContentApi.GetPermissionsByItem(_Id, _ItemType, 0, "", permissionUserType, ContentAPI.PermissionRequestType.UnAssigned, pageInfo)

        userCriteria.PagingInfo = pageInfo
        userCriteria.AddFilter(Ektron.Cms.Common.UserProperty.UserName, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchText)
        userCriteria.AddFilter(Ektron.Cms.Common.UserProperty.IsMemberShip, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _IsMembership)
        userCriteria.AddFilter(Ektron.Cms.Common.UserProperty.UserName, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, "internaladmin")
        userCriteria.AddFilter(Ektron.Cms.Common.UserProperty.IsDeleted, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, True)
        userDataList = userFrameworkApi.GetList(userCriteria)

        While (i < userDataList.Count)
            For j = 0 To _AssignedUsers.Length - 2
                If _AssignedUsers(j) = userDataList(i).Id Then
                    userDataList.Remove(userDataList(i))
                End If
                If userDataList.Count = 0 Then
                    Exit While
                End If
            Next
            i = i + 1
        End While
        i = 0
        If userDataList.Count > 0 Then
            If userDataList.Count = 1 Then
                ReDim userGroupData(userDataList.Count)
            Else
                ReDim userGroupData(userDataList.Count - 1)
            End If
            'isAssignedUser = isAssigned(_UserGroupData, userDataList)
            'If (isAssignedUser = False) Then
            For Each user In userDataList
                userGroupData(i) = New UserGroupData
                userGroupData(i).UserId = user.Id
                userGroupData(i).UserName = user.Username
                userGroupData(i).GroupId = -1
                userGroupData(i).IsMemberShipUser = user.IsMemberShip
                i = i + 1
            Next
            'End If
        End If
        'Return IIf(userGroupDataItem.GroupId <> -1, userGroupDataItem.GroupId.ToString(), userGroupDataItem.UserId.ToString())
        If userGroupData IsNot Nothing Then
            For j = 0 To userGroupData.Length - 1
                If userGroupData(j) Is Nothing Then
                    count = count + 1
                End If
            Next
            If count = userGroupData.Length Then
                Return emptyUserGroupData
            Else
                Return userGroupData
            End If
        End If
        _TotalPages = pageInfo.TotalPages

        Return emptyUserGroupData
    End Function
    Private Function isAssigned(ByVal userGroupData() As Ektron.Cms.UserGroupData, ByVal userDataList As List(Of UserData)) As Boolean
        Dim userGroup As UserGroupData
        Dim user As New UserData
        Dim returnVal As Boolean = True
        For Each userGroup In userGroupData
            For Each user In userDataList
                If userGroup.UserId = user.Id Then
                    returnVal = False
                End If
            Next
        Next
        Return returnVal

    End Function
    Private Function GetSearchedGroups() As UserGroupData()
        ValidatePermissions()

        'set paging size
        Dim pageInfo As New Ektron.Cms.PagingInfo
        Dim groupList As New System.Collections.Generic.List(Of Ektron.Cms.UserGroupData)
        Dim i As Integer
        Dim j As Integer
        pageInfo.CurrentPage = _CurrentPage + 1
        pageInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize
        Dim userGroupData() As UserGroupData = Nothing
        Dim groupData As New UserGroupData

        Dim userGroupCriteria As New Ektron.Cms.Common.Criteria(Of Ektron.Cms.Common.UserGroupProperty)
        userGroupCriteria.PagingInfo = pageInfo
        userGroupCriteria.AddFilter(Ektron.Cms.Common.UserGroupProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchText)
        userGroupCriteria.AddFilter(Ektron.Cms.Common.UserGroupProperty.IsMembershipGroup, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _IsMembership)

        Dim assignedUser As String = hdnAssignedUserGroupIds.Value
        _AssignedUsers = assignedUser.Split(",")
        groupList = _UserApi.GetUserGroupList(userGroupCriteria)
        
        If groupList.Count > 0 Then
            While (i < groupList.Count)
                For j = 0 To _AssignedUsers.Length - 2
                    If _AssignedUsers(j) = groupList(i).GroupId Then
                        groupList.Remove(groupList(i))
                    End If
                    If groupList.Count = 0 Then
                        Exit While
                    End If
                Next
                i = i + 1
            End While
            If groupList.Count = 1 Then
                ReDim userGroupData(groupList.Count)
            Else
                ReDim userGroupData(groupList.Count - 1)
            End If
            i = 0

            'isAssignedUser = isAssigned(_UserGroupData, userDataList)
            'If (isAssignedUser = False) Then
            For Each groupData In groupList
                userGroupData(i) = New UserGroupData
                userGroupData(i).GroupId = groupData.GroupId
                userGroupData(i).GroupName = groupData.GroupName
                userGroupData(i).UserId = -1
                userGroupData(i).IsMemberShipUser = groupData.IsMemberShipGroup
                i = i + 1
            Next
            'End If
        End If
        _TotalPages = pageInfo.TotalPages
        Return userGroupData
    End Function

    Private Sub UserGroupDataFilter(ByVal KeepOnly As FilterType)
        Dim i As Integer = 0
        Select Case KeepOnly
            Case FilterType.Group
                For i = 0 To _UserGroupData.Length - 1
                    If _UserGroupData(i).UserId <> -1 Then
                        Array.Clear(_UserGroupData, i, 1)
                    End If
                Next
            Case FilterType.User
                For i = 0 To _UserGroupData.Length - 1
                    If _UserGroupData(i).GroupId <> -1 Then
                        Array.Clear(_UserGroupData, i, 1)
                    End If
                Next
        End Select

    End Sub

    Private Sub SelectPermissionsToolBar()
        Dim result As New System.Text.StringBuilder
        Dim WorkareaTitlebarTitle As String = ""

        If (_ItemType = "folder") Then
            _FolderData = _ContentApi.GetFolderById(_Id)
            If _FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard Then
                _IsDiscussionBoardOrDiscussionForum = True
            End If
            If _IsDiscussionBoardOrDiscussionForum Then
                WorkareaTitlebarTitle = _MessageHelper.GetMessage("add board permissions") & " """ & _FolderData.Name & """"
            Else
                WorkareaTitlebarTitle = _MessageHelper.GetMessage("add folder permissions") & " """ & _FolderData.Name & """"
            End If
        Else
            _ContentData = _ContentApi.GetContentById(_Id)
            WorkareaTitlebarTitle = _MessageHelper.GetMessage("add content permissions") & " """ & _ContentData.Title & """"
        End If
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(WorkareaTitlebarTitle)
        result.Append("<table><tr>").Append(Environment.NewLine)
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/save.png", "javascript:Ektron.Workarea.Permissions.Add.submit(this);", _MessageHelper.GetMessage("btn save"), _MessageHelper.GetMessage("btn save"), "")).Append(Environment.NewLine)
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/back.png", "content.aspx?LangType=" & _ContentLanguage & "&action=ViewPermissions&id=" & _Id & "&type=" & Request.QueryString("type") & "&membership=" & Request.QueryString("membership"), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "")).Append(Environment.NewLine)
        result.Append(AppendUserGroupDD).Append(Environment.NewLine)
        result.Append("<td nowrap valign=""top"">&nbsp;|&nbsp;<label for=""txtSearch"">" & _MessageHelper.GetMessage("generic search") & "</label>").Append(Environment.NewLine)
        result.Append(" <input type=""text"" class=""ektronTextMedium"" id=""txtSearch"" name=""txtSearch"" value=""" & m_strKeyWords & """ onkeydown=""Ektron.Workarea.Permissions.Add.Search.checkForReturn(event);"" /></td>").Append(Environment.NewLine)
        result.Append(" <td><input type=""button"" value=" & _MessageHelper.GetMessage("generic Search") & " id=""btnSearch"" name=""btnSearch"" onclick=""Ektron.Workarea.Permissions.Add.Search.submit();"" class=""ektronWorkareaSearch"" /></td>").Append(Environment.NewLine)
        result.Append("<td>").Append(Environment.NewLine)
        If _IsDiscussionBoardOrDiscussionForum Then
            result.Append(_StyleHelper.GetHelpButton("selectboardperms"))
        Else
            result.Append(_StyleHelper.GetHelpButton(_PageAction))
        End If
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString()

    End Sub

    Private Function AppendUserGroupDD() As String
        Dim result As New StringBuilder
        result.Append("<td class=""label"">&nbsp;|&nbsp;" & _MessageHelper.GetMessage("lbl show") & ":</td>")
        result.Append("<td>")
        result.Append(" <select id=""selecttype"" name=""selecttype"" onchange=""setUserGroupView(this);"">")
        result.Append("     <option value=""-1""" & IsSelected("1") & ">" & _MessageHelper.GetMessage("lbl select user or group") & "</option>")

        Select Case _IsMembership
            Case False
                result.Append("     <option value=""2""" & IsSelected("2") & ">" & _MessageHelper.GetMessage("generic cms user label") & "</option>")
                result.Append("     <option value=""3""" & IsSelected("3") & ">" & _MessageHelper.GetMessage("cms group title") & "</option>")
            Case True
                result.Append("     <option value=""4""" & IsSelected("4") & ">" & _MessageHelper.GetMessage("lbl member user") & "</option>")
                result.Append("     <option value=""5""" & IsSelected("5") & ">" & _MessageHelper.GetMessage("lbl member group") & "</option>")
        End Select

        result.Append(" </select>")
        result.Append("</td>")
        Return result.ToString()
    End Function

    Private Function IsSelected(ByVal val As String) As String
        If (val = _PermissionSelectType) Then
            Return (" selected ")
        Else
            Return ("")
        End If
    End Function

#End Region

#Region "CSS, JS"

    Private Sub RegisterCSS()

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)

    End Sub

    Private Sub RegisterJS()

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me._ContentApi.AppPath & "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareaSearchBoxInputLabelInitJS")

        Me.litLanguageId.Text = Me._ContentLanguage
        ltrLanguageId.text = Me._ContentLanguage
        ltrid.text = Me._Id
        Me.litItemType.Text = Me._ItemType
        Me.ltritemtype.text = Me._ItemType
        Me.ltrIsMembership.Text = Me._IsMembership
        Me.litIsMembership.Text = Me._IsMembership
        Me.litId.Text = Me._Id
        Me.litNoItemSelected.Text = _MessageHelper.GetMessage("js:no items selected")
    End Sub

#End Region

End Class