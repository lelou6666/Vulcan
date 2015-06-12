Imports system.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class viewpermissions
    Inherits System.Web.UI.UserControl

#Region "Member Variables"

    Protected _IsDiscussionBoardOrDiscussionForum As Boolean = False
    Protected _ContentApi As New ContentAPI
    Protected _StyleHelper As New StyleHelper
    Protected _MessageHelper As Ektron.Cms.Common.EkMessageHelper
    Protected _Id As Long = 0
    Protected _FolderData As FolderData
    Protected _PermissionData As PermissionData
    Protected _AppImgPath As String = ""
    Protected _ContentType As Integer = 1
    Protected _CurrentUserId As Long = 0
    Protected _PageData As Collection
    Protected _PageAction As String = ""
    Protected _OrderBy As String = ""
    Protected _ContentLanguage As Integer = -1
    Protected _EnableMultilingual As Integer = 0
    Protected _SitePath As String = ""
    Protected _ItemType As String = ""
    Protected _ContentData As ContentData
    Protected _IsMembership As Boolean = False
    Protected _Base As String = ""

#End Region

#Region "Events"

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        _MessageHelper = _ContentApi.EkMsgRef
        ddlUserType.Attributes.Add("style", "margin: .5em 0em .5em .5em;")
        If Me.IsPostBack Then
            _IsMembership = IIf(Request.Form(ddlUserType.UniqueID) = "membership", True, False)
        Else
            Dim membership As String = IIf(String.IsNullOrEmpty(Request.QueryString("membership")), "false", Request.QueryString("membership"))
            Boolean.TryParse(membership, _IsMembership)
        End If

        
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        RegisterResources()
        liCmsUsers.Text = _MessageHelper.GetMessage("lbl view cms users")
        liCmsUsers.Value = "standard"
        liCmsUsers.Selected = IIf(Not _IsMembership, True, False)
        liMembershipUsers.Text = _MessageHelper.GetMessage("lbl view memberShip users")
        liMembershipUsers.Value = "membership"
        liMembershipUsers.Selected = IIf(_IsMembership, True, False)

        phAdvancedTab.Visible = IIf(_IsMembership, False, True)
        phAdvancedContent.Visible = IIf(_IsMembership, False, True)

    End Sub

#End Region

#Region "Helpers"

    Public Function ViewPermission() As Boolean
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
        If (Not (Request.QueryString("base") Is Nothing)) Then
            _Base = Request.QueryString("base").Trim.ToLower
        End If
        _CurrentUserId = _ContentApi.UserId
        _AppImgPath = _ContentApi.AppImgPath
        _SitePath = _ContentApi.SitePath
        _EnableMultilingual = _ContentApi.EnableMultilingual
        Display_ViewPermissions()
    End Function

#End Region

#Region "PERMISSION - ViewPermissions"

    Private Function IsAllowed() As Boolean
        Dim bFolderUserAdmin As Boolean = False

        _PermissionData = _ContentApi.LoadPermissions(_Id, _ItemType)
        If (_Id >= 0) Then
            If (Me._ItemType <> "folder") Then
                If (_ContentData Is Nothing) Then
                    _ContentData = _ContentApi.GetContentById(_Id)
                End If
                bFolderUserAdmin = _PermissionData.IsAdmin OrElse _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_ContentData.FolderId)
            Else
                bFolderUserAdmin = _PermissionData.IsAdmin OrElse _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id)
            End If
        Else
            bFolderUserAdmin = _PermissionData.IsAdmin
        End If
        Return bFolderUserAdmin
    End Function

    Private Sub Display_ViewPermissions()
        Dim folder_sub_data As FolderData
        Dim strMsg As String = ""
        Dim bPrivate As Boolean = False
        If (Not IsAllowed()) Then
            Response.Redirect("dashboard.aspx", False)
            Return
        End If

        If (_ItemType = "folder") Then
            _FolderData = _ContentApi.GetFolderById(_Id)
            If _FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard OrElse _FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum Then
                _IsDiscussionBoardOrDiscussionForum = True
            End If
            If (_FolderData.Inherited) Then
                folder_sub_data = _ContentApi.GetFolderById(_FolderData.InheritedFrom)
            End If
            bPrivate = _FolderData.PrivateContent
        Else
            _ContentData = _ContentApi.GetContentById(_Id)
            If (_ContentData.IsInherited) Then
                folder_sub_data = _ContentApi.GetFolderById(_ContentData.InheritedFrom)
            End If
            bPrivate = _ContentData.IsPrivate
        End If
        ViewPermissionsToolBar()
        _PageData = New Collection

        _PageData.Add("", "UserID")
        _PageData.Add("", "UserGroupList")
        _PageData.Add(_Id, "ItemID")
        _PageData.Add(_ItemType, "RequestType")

        Dim userpermission_data() As UserPermissionData
        If (_IsMembership) Then
            userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, 0, "", ContentAPI.PermissionUserType.Membership, ContentAPI.PermissionRequestType.All)
        Else
            userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, 0, "", ContentAPI.PermissionUserType.Cms, ContentAPI.PermissionRequestType.All)
        End If

        If (_ItemType = "folder") Then
            If Not (_IsDiscussionBoardOrDiscussionForum) Then
                strMsg = _MessageHelper.GetMessage("folder is private msg")
            Else
                divPrivateContent.Visible = False
            End If
        Else
            strMsg = _MessageHelper.GetMessage("content is private msg")
        End If

        Me.cbPrivateContent.Attributes.Add("onclick", "return CheckPrivateContent(this, " & _Id & ",'" & _ItemType & "');")
        If (bPrivate) Then
            Me.cbPrivateContent.Checked = True
        End If
        Me.lblPrivateContent.Text = strMsg

        If ((_ItemType = "folder" Or _ItemType = "content") And (_Id > 0)) Then
            Me.cbInheritedPermissions.Attributes.Add("onclick", "return CheckInheritance(this, " & _Id & ",'" & _ItemType & "');")
            Me.cbInheritedPermissions.Checked = False
            Me.lblInheritedPermissions.Text = _MessageHelper.GetMessage("allow permission inheritance")

            If (_ItemType.ToLower = "folder") Then
                If (_FolderData.Inherited) Then
                    Me.cbInheritedPermissions.Checked = True
                End If
            ElseIf (_ItemType.ToLower = "content") Then
                If (_ContentData.IsInherited) Then
                    Me.cbInheritedPermissions.Checked = True
                End If
            End If
        Else
            Me.cbInheritedPermissions.Visible = False
            Me.lblInheritedPermissions.Visible = False
        End If
        If _IsDiscussionBoardOrDiscussionForum AndAlso _FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard Then
            divInheritedPermissions.Visible = False
        End If

        Populate_ViewPermissionsGenericGrid(userpermission_data)
        If (Not _IsMembership) Then
            Populate_ViewPermissionsAdvancedGrid(userpermission_data)
        End If
    End Sub

    Private Sub Populate_ViewPermissionsGenericGrid(ByVal userpermission_data As UserPermissionData())
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderStyle.CssClass = "left"
        colBound.ItemStyle.CssClass = "left"
        colBound.HeaderText = _MessageHelper.GetMessage("user or group name title")
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "READ"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        colBound.HeaderText = _MessageHelper.GetMessage("generic read only")
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDIT"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsDiscussionBoardOrDiscussionForum Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title")
        End If
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ADD"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsDiscussionBoardOrDiscussionForum Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title")
        End If
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DELETE"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsDiscussionBoardOrDiscussionForum Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title")
        End If
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        PermissionsGenericGrid.Columns.Add(colBound)
        If Not (_IsDiscussionBoardOrDiscussionForum) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "RESTORE"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Restore title")
            PermissionsGenericGrid.Columns.Add(colBound)
        End If

        If _IsDiscussionBoardOrDiscussionForum Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GREAD"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm postreply")
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDFILE"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm addimgfil")
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADD"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm moderate")
            PermissionsGenericGrid.Columns.Add(colBound)

        Else

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GREAD"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Library title") & " " & _MessageHelper.GetMessage("generic read only")
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADD"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " " & _MessageHelper.GetMessage("generic Images")
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDFILE"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " " & _MessageHelper.GetMessage("generic Files")
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDHYP"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " " & _MessageHelper.GetMessage("generic Hyperlinks")
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "OVERWRITELIB"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderText = _MessageHelper.GetMessage("overwrite library title")
            PermissionsGenericGrid.Columns.Add(colBound)
        End If

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("READ", GetType(String)))
        dt.Columns.Add(New DataColumn("EDIT", GetType(String)))
        dt.Columns.Add(New DataColumn("ADD", GetType(String)))
        dt.Columns.Add(New DataColumn("DELETE", GetType(String)))
        dt.Columns.Add(New DataColumn("RESTORE", GetType(String)))
        If _IsDiscussionBoardOrDiscussionForum Then
            dt.Columns.Add(New DataColumn("GREAD", GetType(String)))
            dt.Columns.Add(New DataColumn("GADDFILE", GetType(String)))
            dt.Columns.Add(New DataColumn("GADD", GetType(String)))
        Else
            dt.Columns.Add(New DataColumn("GREAD", GetType(String)))
            dt.Columns.Add(New DataColumn("GADD", GetType(String)))
            dt.Columns.Add(New DataColumn("GADDFILE", GetType(String)))
        End If
        dt.Columns.Add(New DataColumn("GADDHYP", GetType(String)))
        dt.Columns.Add(New DataColumn("OVERWRITELIB", GetType(String)))
        Dim permissionInherited As Boolean = False
        If (_ItemType = "folder") Then
            permissionInherited = _FolderData.Inherited
        Else
            permissionInherited = _ContentData.IsInherited
        End If

        Dim i As Integer
        Dim isGroup As Boolean
        If (Not (IsNothing(userpermission_data))) Then
            For i = 0 To userpermission_data.Length - 1
                dr = dt.NewRow()
                isGroup = IIf(userpermission_data(i).GroupId <> -1, True, False)
                If (isGroup) Then
                    If (_IsMembership) Then
                        dr(0) = "<span class=""membershipGroup"">"
                    Else
                        dr(0) = "<span class=""cmsGroup"">"
                    End If
                    If (permissionInherited) Then
                        dr(0) += userpermission_data(i).DisplayGroupName
                    Else
                        dr(0) += "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=EditPermissions&type=" & _ItemType & "&id=" & _Id & "&base=group&PermID=" & userpermission_data(i).GroupId & "&membership=" & _IsMembership.ToString() & """ title='" & _MessageHelper.GetMessage("edit group permissions") & "'>" & userpermission_data(i).DisplayGroupName & "</a>"
                    End If
                    dr(0) += "</span>"
                Else
                    If _IsMembership Then
                        dr(0) = "<span class=""membershipUser"">"
                    Else
                        dr(0) = "<span class=""cmsUser"">"
                    End If
                    If (permissionInherited) Then
                        dr(0) += userpermission_data(i).DisplayUserName
                    Else
                        dr(0) += "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=EditPermissions&type=" & _ItemType & "&id=" & _Id & "&base=user&PermID=" & userpermission_data(i).UserId & "&membership=" & _IsMembership.ToString() & """ title='" & _MessageHelper.GetMessage("edit user permissions") & "'>" & userpermission_data(i).DisplayUserName & "</a>"
                    End If
                    dr(0) += "</span>"
                End If

                If (userpermission_data(i).IsReadOnly) Then
                    dr(1) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                Else
                    dr(1) = "&#160;"
                End If

                If (userpermission_data(i).CanEdit) Then
                    dr(2) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                Else
                    dr(2) = "&#160;"
                End If

                If (userpermission_data(i).CanAdd) Then
                    dr(3) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                Else
                    dr(3) = "&#160;"
                End If

                If (userpermission_data(i).CanDelete) Then
                    dr(4) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                Else
                    dr(4) = "&#160;"
                End If

                If Not _IsDiscussionBoardOrDiscussionForum Then
                    If (userpermission_data(i).CanRestore) Then
                        dr(5) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                    Else
                        dr(5) = "&#160;"
                    End If
                End If

                If (userpermission_data(i).IsReadOnlyLib) Then
                    dr(6) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                Else
                    dr(6) = "&#160;"
                End If

                If (userpermission_data(i).CanAddToImageLib) Then
                    dr(7) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                Else
                    dr(7) = "&#160;"
                End If
                If Not (_IsDiscussionBoardOrDiscussionForum) Then
                    If (userpermission_data(i).CanAddToImageLib) Then
                        dr(7) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                    Else
                        dr(7) = "&#160;"
                    End If

                    If (userpermission_data(i).CanAddToFileLib) Then
                        dr(8) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                    Else
                        dr(8) = "&#160;"
                    End If

                    If (userpermission_data(i).CanAddToHyperlinkLib) Then
                        dr(9) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                    Else
                        dr(9) = "&#160;"
                    End If

                    If (userpermission_data(i).CanOverwriteLib) Then
                        dr(10) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                    Else
                        dr(10) = "&#160;"
                    End If
                Else
                    If (userpermission_data(i).CanAddToFileLib) Then ' add image/file
                        dr(7) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                    Else
                        dr(7) = "&#160;"
                    End If

                    If (userpermission_data(i).CanAddToImageLib) Then 'moderate
                        dr(8) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                    Else
                        dr(8) = "&#160;"
                    End If
                End If
                dt.Rows.Add(dr)
            Next i
        End If
        Dim dv As New DataView(dt)
        PermissionsGenericGrid.DataSource = dv
        PermissionsGenericGrid.DataBind()
    End Sub

    Private Sub Populate_ViewPermissionsAdvancedGrid(ByVal userpermission_data As UserPermissionData())
        Dim colBound As New System.Web.UI.WebControls.BoundColumn

        colBound.DataField = "TITLE"
        colBound.HeaderStyle.CssClass = "left"
        colBound.ItemStyle.CssClass = "left"
        colBound.HeaderText = _MessageHelper.GetMessage("user or group name title")
        PermissionsAdvancedGrid.Columns.Add(colBound)

        If Not (_IsDiscussionBoardOrDiscussionForum) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "COLLECTIONS"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderText = _MessageHelper.GetMessage("generic collection title")
            PermissionsAdvancedGrid.Columns.Add(colBound)
        End If

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ADDFLD"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsDiscussionBoardOrDiscussionForum Then
            colBound.HeaderText = "Add Forum"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic add folders title")
        End If
        PermissionsAdvancedGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITFLD"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsDiscussionBoardOrDiscussionForum Then
            colBound.HeaderText = "Edit Forum"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic edit folders title")
        End If
        PermissionsAdvancedGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DELETEFLD"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsDiscussionBoardOrDiscussionForum Then
            colBound.HeaderText = "Delete Forum"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic delete folders title")
        End If
        PermissionsAdvancedGrid.Columns.Add(colBound)

        If Not (_IsDiscussionBoardOrDiscussionForum) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "TRAVERSE"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderText = _MessageHelper.GetMessage("generic traverse folder title")
            PermissionsAdvancedGrid.Columns.Add(colBound)
        End If

        Dim m_refSiteApi As New SiteAPI
        Dim setting_data As New SettingsData
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
        If setting_data.EnablePreApproval Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "ModifyPreapproval"
            colBound.HeaderText = "Modify Preapproval"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsAdvancedGrid.Columns.Add(colBound)
        End If

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("COLLECTIONS", GetType(String)))
        dt.Columns.Add(New DataColumn("ADDFLD", GetType(String)))
        dt.Columns.Add(New DataColumn("EDITFLD", GetType(String)))
        dt.Columns.Add(New DataColumn("DELETEFLD", GetType(String)))
        dt.Columns.Add(New DataColumn("TRAVERSE", GetType(String)))
        If setting_data.EnablePreApproval Then
            dt.Columns.Add(New DataColumn("ModifyPreapproval", GetType(String)))
        End If

        Dim permissionInherited As Boolean = False
        If (_ItemType = "folder") Then
            permissionInherited = _FolderData.Inherited
        Else
            permissionInherited = _ContentData.IsInherited
        End If

        Dim i As Integer
        Dim isGroup As Boolean
        If (Not (IsNothing(userpermission_data))) Then
            For i = 0 To userpermission_data.Length - 1
                dr = dt.NewRow()

                isGroup = IIf(userpermission_data(i).GroupId <> -1, True, False)
                If (isGroup) Then
                    If (_IsMembership) Then
                        dr(0) = "<span class=""membershipGroup"">"
                    Else
                        dr(0) = "<span class=""cmsGroup"">"
                    End If
                    If (permissionInherited) Then
                        dr(0) += userpermission_data(i).DisplayGroupName
                    Else
                        dr(0) += "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=EditPermissions&type=" & _ItemType & "&id=" & _Id & "&base=group&PermID=" & userpermission_data(i).GroupId & "&membership=" & _IsMembership.ToString() & """ title='" & _MessageHelper.GetMessage("edit group permissions") & "'>" & userpermission_data(i).DisplayGroupName & "</a>"
                    End If
                    dr(0) += "</span>"
                Else
                    If _IsMembership Then
                        dr(0) = "<span class=""membershipUser"">"
                    Else
                        dr(0) = "<span class=""cmsUser"">"
                    End If
                    If (permissionInherited) Then
                        dr(0) += userpermission_data(i).DisplayUserName
                    Else
                        dr(0) += "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=EditPermissions&type=" & _ItemType & "&id=" & _Id & "&base=user&PermID=" & userpermission_data(i).UserId & "&membership=" & _IsMembership.ToString() & """ title='" & _MessageHelper.GetMessage("edit user permissions") & "'>" & userpermission_data(i).DisplayUserName & "</a>"
                    End If
                    dr(0) += "</span>"
                End If

                If Not (_IsDiscussionBoardOrDiscussionForum) Then
                    If (userpermission_data(i).IsCollections) Then
                        dr(1) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                    Else
                        dr(1) = "&#160;"
                    End If
                End If

                If (userpermission_data(i).CanAddFolders) Then
                    dr(2) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                Else
                    dr(2) = "&#160;"
                End If

                If (userpermission_data(i).CanEditFolders) Then
                    dr(3) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                Else
                    dr(3) = "&#160;"
                End If

                If (userpermission_data(i).CanDeleteFolders) Then
                    dr(4) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                Else
                    dr(4) = "&#160;"
                End If
                If Not (_IsDiscussionBoardOrDiscussionForum) Then
                    If (userpermission_data(i).CanTraverseFolders) Then
                        dr(5) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                    Else
                        dr(5) = "&#160;"
                    End If
                End If
                If setting_data.EnablePreApproval Then
                    If (userpermission_data(i).CanEditApprovals) Then
                        dr(6) = "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
                    Else
                        dr(6) = "&#160;"
                    End If
                End If
                dt.Rows.Add(dr)
            Next i
        End If
        Dim dv As New DataView(dt)
        PermissionsAdvancedGrid.DataSource = dv
        PermissionsAdvancedGrid.DataBind()
    End Sub

    Private Sub ViewPermissionsToolBar()
        Dim result As New System.Text.StringBuilder
        Dim inheritedPermissions As Boolean = False
        Dim WorkareaTitlebarTitle As String = ""
        If (_ItemType = "folder") Then
            If _IsDiscussionBoardOrDiscussionForum Then
                If (_FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard) Then
                    WorkareaTitlebarTitle = _MessageHelper.GetMessage("view board permissions msg") & " """ & _FolderData.Name & """"
                Else
                    WorkareaTitlebarTitle = _MessageHelper.GetMessage("view forum permissions msg") & " """ & _FolderData.Name & """"
                End If

            Else
                WorkareaTitlebarTitle = _MessageHelper.GetMessage("view folder permissions msg") & " """ & _FolderData.Name & """"
            End If
        Else
            WorkareaTitlebarTitle = _MessageHelper.GetMessage("view content permissions msg") & " """ & _ContentData.Title & """"
        End If
        divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(WorkareaTitlebarTitle)
        If (_ItemType = "folder") Then
            inheritedPermissions = _FolderData.Inherited
        Else
            inheritedPermissions = _ContentData.IsInherited
        End If
        result.Append("<table><tbody><tr>")
        If (inheritedPermissions = False) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/add.png", "content.aspx?LangType=" & _ContentLanguage & "&action=SelectPermissions&id=" & _Id & "&type=" & Request.QueryString("type") & "&membership=" & _IsMembership.ToString().ToLower(), _MessageHelper.GetMessage("alt add button text (permissions)"), _MessageHelper.GetMessage("btn add permissions"), ""))
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/delete.png", "content.aspx?LangType=" & _ContentLanguage & "&action=DeletePermissions&id=" & _Id & "&type=" & Request.QueryString("type") & "&membership=" & _IsMembership.ToString().ToLower(), _MessageHelper.GetMessage("alt delete button text (permissions)"), _MessageHelper.GetMessage("btn delete permissions"), ""))
        End If
        If (Request.QueryString("type") = "folder") Then
            If _IsDiscussionBoardOrDiscussionForum Then
                result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/back.png", "content.aspx?action=ViewContentByCategory&id=" & _FolderData.Id & "&LangType=" & _ContentLanguage, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
            Else
                result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/back.png", "content.aspx?action=ViewFolder&id=" & _Id & "&LangType=" & _ContentLanguage, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
            End If
        Else
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/back.png", "content.aspx?LangType=" & _ContentLanguage & "&action=View&id=" & _Id, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        If _IsDiscussionBoardOrDiscussionForum Then
            result.Append(_StyleHelper.GetHelpButton("viewboardperms"))
        Else
            result.Append(_StyleHelper.GetHelpButton(_PageAction))
        End If
        result.Append("</td>")
        result.Append("</tr></tbody></table>")
        divToolBar.InnerHtml = result.ToString()

    End Sub

#End Region

#Region "CSS, JS"

    Private Sub RegisterResources()
        'CSS
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)

        'JS
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
    End Sub


#End Region

End Class
