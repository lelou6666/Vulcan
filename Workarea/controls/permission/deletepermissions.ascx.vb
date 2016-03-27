Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class deletepermissions
    Inherits System.Web.UI.UserControl

#Region "Member Variables"

    Protected _ContentApi As New ContentAPI
    Protected _StyleHelper As New StyleHelper
    Protected _MessageHelper As Ektron.Cms.Common.EkMessageHelper
    Protected _Id As Long = 0
    Protected _FolderData As FolderData
    Protected _PermissionData As PermissionData
    Protected _ApplicationImagePath As String = ""
    Protected _ContentType As Integer = 1
    Protected _CurrentUserId As Long = 0
    Protected _PageData As Collection
    Protected _PageAction As String = ""
    Protected _OrderBy As String = ""
    Protected _ContentLanguage As Integer = -1
    Protected _EnableMultilingual As Integer = 0
    Protected _SitePath As String = ""
    Protected _ContentData As ContentData
    Protected _ItemType As String = ""
    Protected _IsMembership As Boolean = False
    Protected _Base As String = ""

    Private _IsBoard As Boolean = False
    Private _UserIcon As String = ""
    Private _GroupIcon As String = ""

#End Region

#Region "Events"

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        _IsMembership = IIf(String.IsNullOrEmpty(Request.QueryString("membership")), False, Boolean.Parse(Request.QueryString("membership")))

    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        _MessageHelper = _ContentApi.EkMsgRef
        RegisterResources()
        If (ddlUserType.Items.Count = 0) Then
            AddUserTypes()
        End If
    End Sub

#End Region

#Region "Helpers"

    Public Function DeletePermission() As Boolean
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
        If (Not (Request.Form(ddlUserType.UniqueID) Is Nothing) And Request.Form(ddlUserType.UniqueID) = "membership") Then
            _IsMembership = True
            ddlUserType.SelectedIndex = 1
        End If
        If (Not (Request.QueryString("base") Is Nothing)) Then
            _Base = Request.QueryString("base").Trim.ToLower
        End If
        _CurrentUserId = _ContentApi.UserId
        _ApplicationImagePath = _ContentApi.AppImgPath
        _SitePath = _ContentApi.SitePath
        _EnableMultilingual = _ContentApi.EnableMultilingual
        If (Not (Page.IsPostBack)) Then
            Display_DeletePermissions()
        Else
        End If

        Return False    ' should this return true?
    End Function

    Private Sub AddUserTypes()
        Dim item As ListItem
        item = New ListItem(_MessageHelper.GetMessage("lbl view cms users"), "standard")
        ddlUserType.Items.Add(item)
        item = New ListItem(_MessageHelper.GetMessage("lbl view memberShip users"), "membership")
        ddlUserType.Items.Add(item)
    End Sub

#End Region

#Region "ACTION - DeleteItemApproval"

    Private Sub Process_DoDeleteItemApproval()
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Try
            m_refContent = _ContentApi.EkContentRef
            _PageData = New Collection
            If (Request.QueryString("type") = "folder") Then
                _PageData.Add(Request.QueryString("id"), "FolderID")
                _PageData.Add("", "ContentID")
            Else
                _PageData.Add(Request.QueryString("id"), "ContentID")
                _PageData.Add("", "FolderID")
            End If
            If (Request.QueryString("base") = "user") Then
                _PageData.Add(Request.QueryString("item_id"), "UserID")
                _PageData.Add("", "UserGroupID")
            Else
                _PageData.Add(Request.QueryString("item_id"), "UserGroupID")
                _PageData.Add("", "UserID")
            End If
            m_refContent.DeleteItemApprovalv2_0(_PageData)
            Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewApprovals&id=" & Request.QueryString("id") & "&type=" & Request.QueryString("type"), False)
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub
#End Region

#Region "PERMISSION - DeletePermissions"

    Private Sub Display_DeletePermissions()
        If (_ItemType = "folder") Then
            _FolderData = _ContentApi.GetFolderById(_Id)
            If _FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard Then
                _IsBoard = True
            End If
        Else
            _ContentData = _ContentApi.GetContentById(_Id)
        End If
        DeletePermissionsToolBar()
        _PageData = New Collection

        _PageData.Add("", "UserID")
        _PageData.Add("", "UserGroupList")
        _PageData.Add(_Id, "ItemID")
        _PageData.Add(_ItemType, "RequestType")
        Dim userpermission_data() As UserPermissionData
        If (_IsMembership) Then
            userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, 0, "", ContentAPI.PermissionUserType.Membership, ContentAPI.PermissionRequestType.All) 'GetMemShpOrderedItemPermissionsV4(pagedata)
            _UserIcon = "userMembership.png"
            _GroupIcon = "usersMembership.png"
        Else
            userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, 0, "", ContentAPI.PermissionUserType.Cms, ContentAPI.PermissionRequestType.All) 'GetCmsOrderedItemPermissionsV4(pagedata)
            _UserIcon = "user.png"
            _GroupIcon = "users.png"
        End If
        td_dp_title.InnerHtml = _MessageHelper.GetMessage("delete permissions msg")
        Populate_DeletePermissionsGenericGrid(userpermission_data)
        If (Not _IsMembership) Then
            phAdvancedTab.Visible = True
            phAdvancedContent.Visible = True
            Populate_DeletePermissionsAdvancedGrid(userpermission_data)
        End If
    End Sub

    Private Sub Populate_DeletePermissionsGenericGrid(ByVal userpermission_data As UserPermissionData())
        SetServerSideJSVariable()

        Dim colBound As New System.Web.UI.WebControls.BoundColumn

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Select"
        colBound.HeaderText = "Select"
        colBound.HeaderStyle.CssClass = "narrowerColumn"
        colBound.ItemStyle.CssClass = "narrowerColumn"
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = _MessageHelper.GetMessage("user or group name title")
        colBound.HeaderStyle.CssClass = "left"
        colBound.ItemStyle.CssClass = "left"
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "READ"
        colBound.HeaderText = _MessageHelper.GetMessage("generic read only")
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDIT"
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title")
        End If
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ADD"
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title")
        End If
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DELETE"
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title")
        End If
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsGenericGrid.Columns.Add(colBound)
        If Not (_IsBoard) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "RESTORE"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Restore title")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)
        End If

        If _IsBoard Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDFILE"
            colBound.HeaderText = "Post/Reply"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDHYP"
            colBound.HeaderText = "Moderate"
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)
        Else
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDFILE"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " " & _MessageHelper.GetMessage("generic Files")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDHYP"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " " & _MessageHelper.GetMessage("generic Hyperlinks")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "OVERWRITELIB"
            colBound.HeaderText = _MessageHelper.GetMessage("overwrite library title")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)
        End If

        Dim dt As New DataTable
        Dim dr As DataRow
        Dim bInherited As Boolean
        If (_ItemType = "folder") Then
            bInherited = _FolderData.Inherited
        Else
            bInherited = _ContentData.IsInherited
        End If
        dt.Columns.Add(New DataColumn("SELECT", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("READ", GetType(String)))
        dt.Columns.Add(New DataColumn("EDIT", GetType(String)))
        dt.Columns.Add(New DataColumn("ADD", GetType(String)))
        dt.Columns.Add(New DataColumn("DELETE", GetType(String)))
        dt.Columns.Add(New DataColumn("RESTORE", GetType(String)))
        dt.Columns.Add(New DataColumn("GREAD", GetType(String)))
        dt.Columns.Add(New DataColumn("GADD", GetType(String)))
        dt.Columns.Add(New DataColumn("GADDFILE", GetType(String)))
        dt.Columns.Add(New DataColumn("GADDHYP", GetType(String)))
        dt.Columns.Add(New DataColumn("OVERWRITELIB", GetType(String)))
        Dim i As Integer = 0

        If (Not (IsNothing(userpermission_data))) Then
            For i = 0 To userpermission_data.Length - 1
                dr = dt.NewRow()

                If (userpermission_data(i).GroupId <> -1) Then
                    dr(0) = "<input type=""checkbox"" id=""group" & userpermission_data(i).GroupId & """ name=""group" & userpermission_data(i).GroupId & """/>"

                    If (bInherited) Then
                        dr(1) = "<span class=""membershipGroup"">"
                        dr(1) += "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=DoDeletePermissions&type=" & _ItemType & "&id=" & _Id & "&base=group&PermID=" & userpermission_data(i).GroupId & "&membership=" & _IsMembership.ToString() & """ title='" & _MessageHelper.GetMessage("delete group permissions") & "' onclick=""return ConfirmDeletePermissions('group','" & _ItemType & "');"">" & userpermission_data(i).GroupName & "</a>"
                        dr(1) += "</span>"
                    Else
                        dr(1) = "<span class=""cmsGroup"">"
                        dr(1) += "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=DoDeletePermissions&type=" & _ItemType & "&id=" & _Id & "&base=group&PermID=" & userpermission_data(i).GroupId & "&membership=" & _IsMembership.ToString() & """ title='" & _MessageHelper.GetMessage("delete group permissions") & "' onclick=""return ConfirmDeletePermissions('group','" & _ItemType & "');"">" & userpermission_data(i).GroupName & "</a>"
                        dr(1) += "</span>"
                    End If

                Else
                    dr(0) = "<input type=""checkbox"" id=""user" & userpermission_data(i).UserId & """ name=""user" & userpermission_data(i).UserId & """/>"
                    If _IsMembership Then
                        dr(1) = "<span class=""membershipUser"">"
                        dr(1) += "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=DoDeletePermissions&type=" & _ItemType & "&id=" & _Id & "&base=user&PermID=" & userpermission_data(i).UserId & "&membership=" & _IsMembership.ToString() & """ title='" & _MessageHelper.GetMessage("delete user permissions") & "' onclick=""return ConfirmDeletePermissions('user','" & _ItemType & "');"">" & userpermission_data(i).DisplayUserName & "</a>"
                        dr(1) += "</span>"
                    Else
                        dr(1) = "<span class=""cmsUser"">"
                        dr(1) += "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=DoDeletePermissions&type=" & _ItemType & "&id=" & _Id & "&base=user&PermID=" & userpermission_data(i).UserId & "&membership=" & _IsMembership.ToString() & """ title='" & _MessageHelper.GetMessage("delete user permissions") & "' onclick=""return ConfirmDeletePermissions('user','" & _ItemType & "');"">" & userpermission_data(i).DisplayUserName & "</a>"
                        dr(1) += "</span>"
                    End If
                End If

                dr(2) = CheckPermission(userpermission_data(i).IsReadOnly)
                dr(3) = CheckPermission(userpermission_data(i).CanEdit)
                dr(4) = CheckPermission(userpermission_data(i).CanAdd)
                dr(5) = CheckPermission(userpermission_data(i).CanDelete)
                If Not (_IsBoard) Then
                    dr(6) = CheckPermission(userpermission_data(i).CanRestore)
                End If
                dr(7) = CheckPermission(userpermission_data(i).IsReadOnlyLib)
                dr(8) = CheckPermission(userpermission_data(i).CanAddToImageLib)
                If Not (_IsBoard) Then
                    dr(9) = CheckPermission(userpermission_data(i).CanAddToFileLib)
                    dr(10) = CheckPermission(userpermission_data(i).CanAddToHyperlinkLib)
                    dr(11) = CheckPermission(userpermission_data(i).CanOverwriteLib)
                End If
                dt.Rows.Add(dr)
            Next
        End If
        Dim dv As New DataView(dt)
        PermissionsGenericGrid.DataSource = dv
        PermissionsGenericGrid.DataBind()
    End Sub

    Private Function CheckPermission(ByVal bPerm As Boolean) As String
        'This method return ("x") if bPerm is true else (" ")
        If (bPerm) Then
            Return "<img src=""" & Me._ContentApi.ApplicationPath & "Images/ui/icons/check.png"" alt=""x"" />"
        Else
            Return "&#160;"
        End If

    End Function

    Private Sub Populate_DeletePermissionsAdvancedGrid(ByVal userpermission_data As UserPermissionData())
        Dim colBound As New System.Web.UI.WebControls.BoundColumn

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = _MessageHelper.GetMessage("user or group name title")
        colBound.HeaderStyle.CssClass = "left"
        colBound.ItemStyle.CssClass = "left"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        If Not (_IsBoard) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "COLLECTIONS"
            colBound.HeaderText = _MessageHelper.GetMessage("generic collection title")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsAdvancedGrid.Columns.Add(colBound)
        End If

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ADDFLD"
        If _IsBoard Then
            colBound.HeaderText = "Add Forum"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic add folders title")
        End If
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITFLD"
        If _IsBoard Then
            colBound.HeaderText = "Edit Forum"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic edit folders title")
        End If
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DELETEFLD"
        If _IsBoard Then
            colBound.HeaderText = "Delete Forum"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic delete folders title")
        End If
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        If Not (_IsBoard) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "TRAVERSE"
            colBound.HeaderText = _MessageHelper.GetMessage("generic traverse folder title")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            colBound.ItemStyle.Wrap = False
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
        Dim bInherited As Boolean = False
        If (_ItemType = "folder") Then
            bInherited = _FolderData.Inherited
        Else
            bInherited = _ContentData.IsInherited
        End If

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("COLLECTIONS", GetType(String)))
        dt.Columns.Add(New DataColumn("ADDFLD", GetType(String)))
        dt.Columns.Add(New DataColumn("EDITFLD", GetType(String)))
        dt.Columns.Add(New DataColumn("DELETEFLD", GetType(String)))
        dt.Columns.Add(New DataColumn("TRAVERSE", GetType(String)))

        If setting_data.EnablePreApproval Then
            dt.Columns.Add(New DataColumn("ModifyPreapproval", GetType(String)))
        End If

        Dim i As Integer = 0

        If (Not (IsNothing(userpermission_data))) Then
            For i = 0 To userpermission_data.Length - 1
                dr = dt.NewRow()

                If (userpermission_data(i).GroupId <> -1) Then
                    If (bInherited) Then
                        dr(0) = "<span class=""membershipGroup"">" & userpermission_data(i).GroupName & "</span>"
                    Else
                        dr(0) = "<span class=""cmsGroup"">"
                        dr(0) += "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=DoDeletePermissions&type=" & _ItemType & "&id=" & _Id & "&base=group&PermID=" & userpermission_data(i).GroupId & "&membership=" & _IsMembership.ToString & """ title='" & _MessageHelper.GetMessage("delete group permissions") & "' onclick=""return ConfirmDeletePermissions('group','" & _ItemType & "');"">" & userpermission_data(i).DisplayGroupName & "</a>"
                        dr(0) += "</span>"
                    End If

                Else
                    If _IsMembership Then
                        dr(0) = "<span class=""membershipUser"">" & userpermission_data(i).DisplayUserName & "</span>"
                    Else
                        dr(0) = "<span class=""cmsUser"">"
                        dr(0) += "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=DoDeletePermissions&type=" & _ItemType & "&id=" & _Id & "&base=user&PermID=" & userpermission_data(i).UserId & "&membership=" & _IsMembership.ToString & """ title='" & _MessageHelper.GetMessage("delete user permissions") & "' onclick=""return ConfirmDeletePermissions('user','" & _ItemType & "');"">" & userpermission_data(i).DisplayUserName & "</a>"
                        dr(0) += "</span>"
                    End If
                End If

                If Not (_IsBoard) Then
                    dr(1) = CheckPermission(userpermission_data(i).IsCollections)
                End If
                dr(2) = CheckPermission(userpermission_data(i).CanAddFolders)
                dr(3) = CheckPermission(userpermission_data(i).CanEditFolders)
                dr(4) = CheckPermission(userpermission_data(i).CanDeleteFolders)
                If Not (_IsBoard) Then
                    dr(5) = CheckPermission(userpermission_data(i).CanTraverseFolders)
                    If setting_data.EnablePreApproval Then
                        dr(6) = CheckPermission(userpermission_data(i).CanEditApprovals)
                    End If
                End If
                dt.Rows.Add(dr)
            Next
        End If
        Dim dv As New DataView(dt)
        PermissionsAdvancedGrid.DataSource = dv
        PermissionsAdvancedGrid.DataBind()
    End Sub

    Private Sub DeletePermissionsToolBar()
        Dim result As New System.Text.StringBuilder
        Dim WorkareaTitlebarTitle As String = ""

        If (_ItemType = "folder") Then
            WorkareaTitlebarTitle = _MessageHelper.GetMessage("delete folder permissions") & " """ & _FolderData.Name & """"
        Else
            WorkareaTitlebarTitle = _MessageHelper.GetMessage("delete content permissions") & " """ & _ContentData.Title & """"
        End If
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(WorkareaTitlebarTitle)
        result.Append("<table><tr>")
        result.Append(_StyleHelper.GetButtonEventsWCaption(_ApplicationImagePath & "../UI/Icons/delete.png", "javascript:Import(this);", _MessageHelper.GetMessage("delete folder permissions"), _MessageHelper.GetMessage("delete folder permissions"), ""))
        result.Append(_StyleHelper.GetButtonEventsWCaption(_ApplicationImagePath & "../UI/Icons/back.png", "content.aspx?action=ViewPermissions&id=" & _Id & "&type=" & Request.QueryString("type") & "&LangType=" & _ContentLanguage, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        result.Append("<td>")
        If _IsBoard Then
            result.Append(_StyleHelper.GetHelpButton("deleteboardperms"))
        Else
            result.Append(_StyleHelper.GetHelpButton(_PageAction))
        End If
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString()
    End Sub

#End Region

#Region "CSS, JS"

    Private Sub RegisterResources()
        'CSS
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronUITabsCss)

        'JS
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)

    End Sub

#End Region
#Region "Utilities"
    Protected Sub SetServerSideJSVariable()
        ltr_contLang.Text = _ContentLanguage
        ltr_id.Text = _Id
        ltr_itemType.Text = _ItemType
        ltr_isMembership.Text = _IsMembership.ToString()
    End Sub
#End Region


End Class