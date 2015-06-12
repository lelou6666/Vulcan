Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants

Partial Class editpermissions
    Inherits System.Web.UI.UserControl

#Region "Member Variables"

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
    Protected _EkContent As Ektron.Cms.Content.EkContent
    Protected _EnablePreaproval As Boolean = False
    Private _IsBoard As Boolean = False
    Private _IsBlog As Boolean = False
    Protected traverseFolder As Boolean = False
#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        _MessageHelper = _ContentApi.EkMsgRef
        RegisterResources()
        traverseFolder = frm_transverse_folder.Value

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

        _IsMembership = False
        Boolean.TryParse(Request.QueryString("membership"), _IsMembership)

        If (Not (Request.QueryString("base") Is Nothing)) Then
            _Base = Request.QueryString("base").Trim.ToLower
        End If
        _CurrentUserId = _ContentApi.UserId
        _AppImgPath = _ContentApi.AppImgPath
        _SitePath = _ContentApi.SitePath
        _EnableMultilingual = _ContentApi.EnableMultilingual

        Dim m_refSiteApi As New SiteAPI
        Dim setting_data As New SettingsData
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
        If setting_data.EnablePreApproval Then _EnablePreaproval = True
        If (ddlUserType.Items.Count = 0) Then
            AddUserTypes()
        End If
    End Sub

#End Region

#Region "Helpers"

    Private Sub AddUserTypes()
        Dim item As ListItem
        item = New ListItem(_MessageHelper.GetMessage("lbl view cms users"), "standard")
        ddlUserType.Items.Add(item)
        item = New ListItem(_MessageHelper.GetMessage("lbl view memberShip users"), "membership")
        ddlUserType.Items.Add(item)
    End Sub

    Public Function EditPermission() As Boolean
        If (Not (Page.IsPostBack)) Then
            Display_EditPermissions()
        Else
            Process_DoEditPermission()
        End If
    End Function

    Public Function AddPermission() As Boolean
        If (Not (Page.IsPostBack)) Then
            Display_AddPermissions()
        Else
            Process_DoAddFolderPermission()
        End If
    End Function

    Public Function GetDisplay() As String

        Return IIf(_IsMembership, "style=""display:none !important;""", String.Empty)

    End Function

#End Region

#Region "ACTION - DoAddFolderPermission"

    Private Sub Process_DoAddFolderPermission()

        Dim userID() As String = Nothing
        Dim groupID() As String = Nothing
        Dim _userIDs As Integer
        Dim _grpIDs As Integer
        Dim base As String = Request.Form(frm_base.UniqueID)
        Dim strUserID As String = ""
        Dim strGroupID As String = ""
        frm_itemid.Value = _Id
        Dim finalUserGroupID As String = ""

        If Request.QueryString("userIds") <> "" Then
            userID = Request.QueryString("userIds").Split(",")
            _userIDs = userID.Length
        End If
        If Request.QueryString("groupIds") <> "" Then
            groupID = Request.QueryString("groupIds").Split(",")
            _grpIDs = groupID.Length
        End If
        If _userIDs + _grpIDs = 1 Then
            If _userIDs = 1 Then
                base = "user"
                strUserID = Request.QueryString("userIds")
            Else
                base = "group"
                strGroupID = Request.QueryString("groupIds")
            End If
        End If
        Dim readOnlyPermission() As String = Request.Form(frm_readonly.UniqueID).Remove(0, 1).Split(",")
        Dim editPermission() As String = Request.Form(frm_edit.UniqueID).Remove(0, 1).Split(",")
        Dim addPermission() As String = Request.Form(frm_add.UniqueID).Remove(0, 1).Split(",")
        Dim deletePermission() As String = Request.Form(frm_delete.UniqueID).Remove(0, 1).Split(",")
        Dim readOnlyLibPermission() As String = Request.Form(frm_libreadonly.UniqueID).Remove(0, 1).Split(",")
        Dim addToImageLibPermission() As String = Request.Form(frm_addimages.UniqueID).Remove(0, 1).Split(",")
        Dim addToFileLibPermission() As String = Request.Form(frm_addfiles.UniqueID).Remove(0, 1).Split(",")
        Dim restorePermission() As String = Nothing
        If Not Request.Form(frm_restore.UniqueID) Is Nothing AndAlso Request.Form(frm_restore.UniqueID) <> "" Then
            restorePermission = Request.Form(frm_restore.UniqueID).Remove(0, 1).Split(",")
        End If
        Dim addToHyperlinkLibPermission() As String = Nothing
        If Not Request.Form(frm_addhyperlinks.UniqueID) Is Nothing AndAlso Request.Form(frm_addhyperlinks.UniqueID) <> "" Then
            addToHyperlinkLibPermission = Request.Form(frm_addhyperlinks.UniqueID).Remove(0, 1).Split(",")
        End If
        Dim overwriteLibPermission() As String = Nothing
        If Not Request.Form(frm_overwritelib.UniqueID) Is Nothing AndAlso Request.Form(frm_overwritelib.UniqueID) <> "" Then
            overwriteLibPermission = Request.Form(frm_overwritelib.UniqueID).Remove(0, 1).Split(",")
        End If
        Dim addFoldersPermission() As String = Request.Form(frm_add_folders.UniqueID).Remove(0, 1).Split(",")
        Dim editFoldersPermission() As String = Request.Form(frm_edit_folders.UniqueID).Remove(0, 1).Split(",")
        Dim deleteFoldersPermission() As String = Request.Form(frm_delete_folders.UniqueID).Remove(0, 1).Split(",")
        Dim transverseFoldersPermission() As String = Nothing
        If Not Request.Form(frm_transverse_folder.UniqueID) Is Nothing AndAlso Request.Form(frm_transverse_folder.UniqueID) <> "" Then
            transverseFoldersPermission = Request.Form(frm_transverse_folder.UniqueID).Remove(0, 1).Split(",")
        End If
        Dim collectionsPermission() As String = Nothing
        If Not Request.Form(frm_navigation.UniqueID) Is Nothing AndAlso Request.Form(frm_navigation.UniqueID) <> "" Then
            collectionsPermission = Request.Form(frm_navigation.UniqueID).Remove(0, 1).Split(",")
        End If


        Dim editApprovalssPermission() As String = Nothing
        If Not (Request.Form(frm_edit_preapproval.UniqueID) Is Nothing) Then
            editApprovalssPermission = Request.Form(frm_edit_preapproval.UniqueID).Remove(0, 1).Split(",")
        End If

        Select Case base
            Case ""
                Dim userCount As Integer = 0
                Dim groupCount As Integer = 0

                Dim userGroupId As String() = New String(_userIDs + (_grpIDs - 1)) {}
                If _grpIDs > 0 AndAlso _userIDs > 0 Then
                    groupID.CopyTo(userGroupId, 0)
                    userID.CopyTo(userGroupId, groupID.Length)
                ElseIf _grpIDs > 0 Then
                    groupID.CopyTo(userGroupId, 0)
                Else
                    userID.CopyTo(userGroupId, 0)
                End If
                For userCount = 0 To userGroupId.Length - 1
                    _EkContent = _ContentApi.EkContentRef
                    _PageData = New Collection
                    If (Request.Form(hmembershiptype.UniqueID) = "1") Then
                        If ((readOnlyPermission(userCount) = "1") Or readOnlyPermission(userCount).ToString().ToLower() = "on") Then
                            _PageData.Add(1, "ReadOnly")
                        Else
                            _PageData.Add(0, "ReadOnly")
                        End If
                        If ((editPermission(userCount) = "1") Or (editPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "Edit")
                        Else
                            _PageData.Add(0, "Edit")
                        End If
                        If ((addPermission(userCount) = "1") Or (addPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "Add")
                        Else
                            _PageData.Add(0, "Add")
                        End If
                        If ((deletePermission(userCount) = "1") Or (deletePermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "Delete")
                        Else
                            _PageData.Add(0, "Delete")
                        End If
                        _PageData.Add(0, "Restore")
                        If ((readOnlyLibPermission(userCount) = "1") Or (readOnlyLibPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "ReadOnlyLib")
                        Else
                            _PageData.Add(0, "ReadOnlyLib")
                        End If
                        If ((addToImageLibPermission(userCount) = "1") Or (addToImageLibPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "AddToImageLib")
                        Else
                            _PageData.Add(0, "AddToImageLib")
                        End If
                        If ((addToFileLibPermission(userCount) = "1") Or (addToFileLibPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "AddToFileLib")
                        Else
                            _PageData.Add(0, "AddToFileLib")
                        End If
                        _PageData.Add(0, "AddToHyperlinkLib")
                        _PageData.Add(0, "OverwriteLib")
                        _PageData.Add(0, "AddFolders")
                        _PageData.Add(0, "EditFolders")
                        _PageData.Add(0, "DeleteFolders")
                        _PageData.Add(0, "Collections")
                        _PageData.Add(0, "TransverseFolder")
                        _PageData.Add(0, "EditApprovals")
                    Else
                        If ((readOnlyPermission(userCount) = "1") Or (readOnlyPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "ReadOnly")
                        Else
                            _PageData.Add(0, "ReadOnly")
                        End If
                        If ((editPermission(userCount) = "1") Or (editPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "Edit")
                        Else
                            _PageData.Add(0, "Edit")
                        End If
                        If ((addPermission(userCount) = "1") Or (addPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "Add")
                        Else
                            _PageData.Add(0, "Add")
                        End If
                        If ((deletePermission(userCount) = "1") Or (deletePermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "Delete")
                        Else
                            _PageData.Add(0, "Delete")
                        End If
                        If Not restorePermission Is Nothing AndAlso ((restorePermission(userCount) = "1") Or (restorePermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "Restore")
                        Else
                            _PageData.Add(0, "Restore")
                        End If
                        If ((readOnlyLibPermission(userCount) = "1") Or (readOnlyLibPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "ReadOnlyLib")
                        Else
                            _PageData.Add(0, "ReadOnlyLib")
                        End If
                        If ((addToImageLibPermission(userCount) = "1") Or (addToImageLibPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "AddToImageLib")
                        Else
                            _PageData.Add(0, "AddToImageLib")
                        End If
                        If ((addToFileLibPermission(userCount) = "1") Or (addToFileLibPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "AddToFileLib")
                        Else
                            _PageData.Add(0, "AddToFileLib")
                        End If
                        If Not addToHyperlinkLibPermission Is Nothing AndAlso ((addToHyperlinkLibPermission(userCount) = "1") Or (addToHyperlinkLibPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "AddToHyperlinkLib")
                        Else
                            _PageData.Add(0, "AddToHyperlinkLib")
                        End If
                        If Not overwriteLibPermission Is Nothing AndAlso ((overwriteLibPermission(userCount) = "1") Or (overwriteLibPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "OverwriteLib")
                        Else
                            _PageData.Add(0, "OverwriteLib")
                        End If

                        If ((addFoldersPermission(userCount) = "1") Or (addFoldersPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "AddFolders")
                        Else
                            _PageData.Add(0, "AddFolders")
                        End If
                        If ((editFoldersPermission(userCount) = "1") Or (editFoldersPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "EditFolders")
                        Else
                            _PageData.Add(0, "EditFolders")
                        End If
                        If ((deleteFoldersPermission(userCount) = "1") Or (deleteFoldersPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "DeleteFolders")
                        Else
                            _PageData.Add(0, "DeleteFolders")
                        End If
                        If Not transverseFoldersPermission Is Nothing AndAlso ((transverseFoldersPermission(userCount) = "1") Or (transverseFoldersPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "TransverseFolder")
                        Else
                            _PageData.Add(0, "TransverseFolder")
                        End If
                        If Not collectionsPermission Is Nothing AndAlso ((collectionsPermission(userCount) = "1") Or (collectionsPermission(userCount).ToString().ToLower() = "on")) Then
                            _PageData.Add(1, "Collections")
                        Else
                            _PageData.Add(0, "Collections")
                        End If

                        If Not (editApprovalssPermission Is Nothing) Then
                            If ((editApprovalssPermission(userCount) = "1") Or (editApprovalssPermission(userCount).ToString().ToLower() = "on")) Then
                                _PageData.Add(1, "EditApprovals")
                            Else
                                _PageData.Add(0, "EditApprovals")
                            End If
                        Else
                            _PageData.Add(0, "EditApprovals")
                        End If
                    End If

                    If (Request.Form(frm_type.UniqueID) = "folder") Then
                        _PageData.Add(Request.Form(frm_itemid.UniqueID), "FolderID")
                        _PageData.Add("", "ContentID")
                    Else
                        _PageData.Add(Request.Form(frm_itemid.UniqueID), "ContentID")
                        _PageData.Add("", "FolderID")
                    End If
                    If (userCount < _grpIDs) Then
                        _PageData.Add(userGroupId(userCount), "UserGroupID")
                        _PageData.Add("", "UserID")
                    Else
                        _PageData.Add(userGroupId(userCount), "UserID")
                        _PageData.Add("", "UserGroupID")
                    End If
                    Dim m_bReturn As Boolean
                    m_bReturn = _EkContent.AddItemPermission(_PageData)
                Next
                Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewPermissions&id=" & Request.Form(frm_itemid.UniqueID) & "&type=" & Request.Form(frm_type.UniqueID) & "&membership=" & Request.Form(frm_membership.UniqueID), False)

            Case Else
                Try
                    _EkContent = _ContentApi.EkContentRef
                    _PageData = New Collection
                    If (Request.Form(hmembershiptype.UniqueID) = "1") Then
                        If ((Request.Form("frm_readonly") = "1") Or Request.Form("frm_readonly").ToString().ToLower() = "on") Then
                            _PageData.Add(1, "ReadOnly")
                        Else
                            _PageData.Add(0, "ReadOnly")
                        End If
                        If (Request.Form(frm_edit.UniqueID) = "1") Then
                            _PageData.Add(1, "Edit")
                        Else
                            _PageData.Add(0, "Edit")
                        End If
                        If (Request.Form(frm_add.UniqueID) = "1") Then
                            _PageData.Add(1, "Add")
                        Else
                            _PageData.Add(0, "Add")
                        End If
                        If (Request.Form(frm_delete.UniqueID) = "1") Then
                            _PageData.Add(1, "Delete")
                        Else
                            _PageData.Add(0, "Delete")
                        End If
                        _PageData.Add(0, "Restore")
                        If (Request.Form(frm_libreadonly.UniqueID) = "1") Then
                            _PageData.Add(1, "ReadOnlyLib")
                        Else
                            _PageData.Add(0, "ReadOnlyLib")
                        End If
                        If (Request.Form(frm_addimages.UniqueID) = "1") Then
                            _PageData.Add(1, "AddToImageLib")
                        Else
                            _PageData.Add(0, "AddToImageLib")
                        End If
                        If (Request.Form(frm_addfiles.UniqueID) = "1") Then
                            _PageData.Add(1, "AddToFileLib")
                        Else
                            _PageData.Add(0, "AddToFileLib")
                        End If
                        _PageData.Add(0, "AddToHyperlinkLib")
                        _PageData.Add(0, "OverwriteLib")
                        _PageData.Add(0, "AddFolders")
                        _PageData.Add(0, "EditFolders")
                        _PageData.Add(0, "DeleteFolders")
                        _PageData.Add(0, "Collections")
                        _PageData.Add(0, "TransverseFolder")
                        _PageData.Add(0, "EditApprovals")
                    Else
                        If (Request.Form(frm_readonly.UniqueID) = "1") Then
                            _PageData.Add(1, "ReadOnly")
                        Else
                            _PageData.Add(0, "ReadOnly")
                        End If
                        If (Request.Form(frm_edit.UniqueID) = "1") Then
                            _PageData.Add(1, "Edit")
                        Else
                            _PageData.Add(0, "Edit")
                        End If
                        If (Request.Form(frm_add.UniqueID) = "1") Then
                            _PageData.Add(1, "Add")
                        Else
                            _PageData.Add(0, "Add")
                        End If
                        If (Request.Form(frm_delete.UniqueID) = "1") Then
                            _PageData.Add(1, "Delete")
                        Else
                            _PageData.Add(0, "Delete")
                        End If
                        If (Request.Form(frm_restore.UniqueID) = "1") Then
                            _PageData.Add(1, "Restore")
                        Else
                            _PageData.Add(0, "Restore")
                        End If
                        If (Request.Form(frm_libreadonly.UniqueID) = "1") Then
                            _PageData.Add(1, "ReadOnlyLib")
                        Else
                            _PageData.Add(0, "ReadOnlyLib")
                        End If
                        If (Request.Form(frm_addimages.UniqueID) = "1") Then
                            _PageData.Add(1, "AddToImageLib")
                        Else
                            _PageData.Add(0, "AddToImageLib")
                        End If
                        If (Request.Form(frm_addfiles.UniqueID) = "1") Then
                            _PageData.Add(1, "AddToFileLib")
                        Else
                            _PageData.Add(0, "AddToFileLib")
                        End If
                        If (Request.Form(frm_addhyperlinks.UniqueID) = "1") Then
                            _PageData.Add(1, "AddToHyperlinkLib")
                        Else
                            _PageData.Add(0, "AddToHyperlinkLib")
                        End If
                        If (Request.Form(frm_overwritelib.UniqueID) = "1") Then
                            _PageData.Add(1, "OverwriteLib")
                        Else
                            _PageData.Add(0, "OverwriteLib")
                        End If

                        If (Request.Form(frm_add_folders.UniqueID) = "1") Then
                            _PageData.Add(1, "AddFolders")
                        Else
                            _PageData.Add(0, "AddFolders")
                        End If
                        If (Request.Form(frm_edit_folders.UniqueID) = "1") Then
                            _PageData.Add(1, "EditFolders")
                        Else
                            _PageData.Add(0, "EditFolders")
                        End If
                        If (Request.Form(frm_delete_folders.UniqueID) = "1") Then
                            _PageData.Add(1, "DeleteFolders")
                        Else
                            _PageData.Add(0, "DeleteFolders")
                        End If
                        If (Request.Form(frm_transverse_folder.UniqueID) = "1") Then
                            _PageData.Add(1, "TransverseFolder")
                        Else
                            _PageData.Add(0, "TransverseFolder")
                        End If
                        If (Request.Form(frm_navigation.UniqueID) = "1") Then
                            _PageData.Add(1, "Collections")
                        Else
                            _PageData.Add(0, "Collections")
                        End If

                        If Not (Request.Form(frm_edit_preapproval.UniqueID) Is Nothing) Then
                            If (Request.Form(frm_edit_preapproval.UniqueID) = "1") Then
                                _PageData.Add(1, "EditApprovals")
                            Else
                                _PageData.Add(0, "EditApprovals")
                            End If
                        Else
                            _PageData.Add(0, "EditApprovals")
                        End If
                    End If

                    If (Request.Form(frm_type.UniqueID) = "folder") Then
                        _PageData.Add(Request.Form(frm_itemid.UniqueID), "FolderID")
                        _PageData.Add("", "ContentID")
                    Else
                        _PageData.Add(Request.Form(frm_itemid.UniqueID), "ContentID")
                        _PageData.Add("", "FolderID")
                    End If
                    If Request.Form(frm_base.UniqueID) = "" Then
                        Select Case Request.QueryString("groupIds")
                            Case ""
                                finalUserGroupID = strUserID
                            Case Else
                                finalUserGroupID = strGroupID
                        End Select
                    Else
                        finalUserGroupID = Request.Form(frm_permid.UniqueID)
                    End If

                    If (Request.Form(frm_base.UniqueID) = "group") Then
                        _PageData.Add(finalUserGroupID, "UserGroupID")
                        _PageData.Add("", "UserID")
                    ElseIf (Request.Form(frm_base.UniqueID) = "user") Then
                        _PageData.Add(finalUserGroupID, "UserID")
                        _PageData.Add("", "UserGroupID")
                    ElseIf Request.QueryString("groupIds") <> "" Then
                        _PageData.Add(finalUserGroupID, "UserGroupID")
                        _PageData.Add("", "UserID")
                    ElseIf Request.QueryString("userIds") <> "" Then
                        _PageData.Add(finalUserGroupID, "UserID")
                        _PageData.Add("", "UserGroupID")
                    End If

                    Dim m_bReturn As Boolean
                    m_bReturn = _EkContent.AddItemPermission(_PageData)
                    Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewPermissions&id=" & Request.Form(frm_itemid.UniqueID) & "&type=" & Request.Form(frm_type.UniqueID) & "&membership=" & Request.Form(frm_membership.UniqueID), False)
                Catch ex As Exception
                    Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
                End Try
        End Select

    End Sub

#End Region

#Region "ACTION - DoEditPermission"

    Private Sub Process_DoEditPermission()
        Try
            _EkContent = _ContentApi.EkContentRef
            _PageData = New Collection
            If (Request.Form(hmembershiptype.UniqueID) = "1") Then
                If (Request.Form(frm_readonly.UniqueID) = "1") Then
                    _PageData.Add(1, "ReadOnly")
                Else
                    _PageData.Add(0, "ReadOnly")
                End If
                If (Request.Form(frm_add.UniqueID) = "1") Then
                    _PageData.Add(1, "Add")
                Else
                    _PageData.Add(0, "Add")
                End If
                If (Request.Form(frm_edit.UniqueID) = "1") Then
                    _PageData.Add(1, "Edit")
                Else
                    _PageData.Add(0, "Edit")
                End If
                If (Request.Form(frm_delete.UniqueID) = "1") Then
                    _PageData.Add(1, "Delete")
                Else
                    _PageData.Add(0, "Delete")
                End If
                _PageData.Add(0, "Restore")
                If (Request.Form(frm_addimages.UniqueID) = "1") Then
                    _PageData.Add(1, "AddToImageLib")
                Else
                    _PageData.Add(0, "AddToImageLib")
                End If
                If (Request.Form(frm_addfiles.UniqueID) = "1") Then
                    _PageData.Add(1, "AddToFileLib")
                Else
                    _PageData.Add(0, "AddToFileLib")
                End If
                _PageData.Add(0, "AddToHyperlinkLib")
                _PageData.Add(0, "OverwriteLib")
                _PageData.Add(0, "AddFolders")
                _PageData.Add(0, "EditFolders")
                _PageData.Add(0, "DeleteFolders")
                _PageData.Add(0, "Collections")
                _PageData.Add(0, "TransverseFolder")
                _PageData.Add(0, "EditApprovals")
                If (Request.Form(frm_libreadonly.UniqueID) = "1") Then
                    _PageData.Add(1, "ReadOnlyLib")
                Else
                    _PageData.Add(0, "ReadOnlyLib")
                End If
            Else
                If (Request.Form(frm_readonly.UniqueID) = "1") Then
                    _PageData.Add(1, "ReadOnly")
                Else
                    _PageData.Add(0, "ReadOnly")
                End If
                If (Request.Form(frm_edit.UniqueID) = "1") Then
                    _PageData.Add(1, "Edit")
                Else
                    _PageData.Add(0, "Edit")
                End If
                If (Request.Form(frm_add.UniqueID) = "1") Then
                    _PageData.Add(1, "Add")
                Else
                    _PageData.Add(0, "Add")
                End If
                If (Request.Form(frm_delete.UniqueID) = "1") Then
                    _PageData.Add(1, "Delete")
                Else
                    _PageData.Add(0, "Delete")
                End If
                If (Request.Form(frm_restore.UniqueID) = "1") Then
                    _PageData.Add(1, "Restore")
                Else
                    _PageData.Add(0, "Restore")
                End If
                If (Request.Form(frm_libreadonly.UniqueID) = "1") Then
                    _PageData.Add(1, "ReadOnlyLib")
                Else
                    _PageData.Add(0, "ReadOnlyLib")
                End If
                If (Request.Form(frm_addimages.UniqueID) = "1") Then
                    _PageData.Add(1, "AddToImageLib")
                Else
                    _PageData.Add(0, "AddToImageLib")
                End If
                If (Request.Form(frm_addfiles.UniqueID) = "1") Then
                    _PageData.Add(1, "AddToFileLib")
                Else
                    _PageData.Add(0, "AddToFileLib")
                End If
                If (Request.Form(frm_addhyperlinks.UniqueID) = "1") Then
                    _PageData.Add(1, "AddToHyperlinkLib")
                Else
                    _PageData.Add(0, "AddToHyperlinkLib")
                End If
                If (Request.Form(frm_overwritelib.UniqueID) = "1") Then
                    _PageData.Add(1, "OverwriteLib")
                Else
                    _PageData.Add(0, "OverwriteLib")
                End If
                If (Request.Form(frm_add_folders.UniqueID) = "1") Then
                    _PageData.Add(1, "AddFolders")
                Else
                    _PageData.Add(0, "AddFolders")
                End If
                If (Request.Form(frm_edit_folders.UniqueID) = "1") Then
                    _PageData.Add(1, "EditFolders")
                Else
                    _PageData.Add(0, "EditFolders")
                End If
                If (Request.Form(frm_delete_folders.UniqueID) = "1") Then
                    _PageData.Add(1, "DeleteFolders")
                Else
                    _PageData.Add(0, "DeleteFolders")
                End If
                If (Request.Form(frm_transverse_folder.UniqueID) = "1") Then
                    _PageData.Add(1, "TransverseFolder")
                Else
                    _PageData.Add(0, "TransverseFolder")
                End If
                If (Request.Form(frm_navigation.UniqueID) = "1") Then
                    _PageData.Add(1, "Collections")
                Else
                    _PageData.Add(0, "Collections")
                End If
                If Not (Request.Form(frm_edit_preapproval.UniqueID) Is Nothing) Then
                    If (Request.Form(frm_edit_preapproval.UniqueID) = "1") Then
                        _PageData.Add(1, "EditApprovals")
                    Else
                        _PageData.Add(0, "EditApprovals")
                    End If
                Else
                    _PageData.Add(0, "EditApprovals")
                End If
            End If
            If (Request.Form(frm_type.UniqueID) = "folder") Then
                _PageData.Add(Request.Form(frm_itemid.UniqueID), "FolderID")
                _PageData.Add("", "ContentID")
            Else
                _PageData.Add(Request.Form(frm_itemid.UniqueID), "ContentID")
                _PageData.Add("", "FolderID")
            End If
            If (Request.Form(frm_base.UniqueID) = "group") Then
                _PageData.Add(Request.Form(frm_permid.UniqueID), "UserGroupID")
                _PageData.Add("", "UserID")
            Else
                _PageData.Add(Request.Form(frm_permid.UniqueID), "UserID")
                _PageData.Add("", "UserGroupID")
            End If
            _EkContent.UpdateItemPermissionv2_0(_PageData)
            Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewPermissions&id=" & Request.Form(frm_itemid.UniqueID) & "&type=" & Request.Form(frm_type.UniqueID) & "&membership=" & Request.Form(frm_membership.UniqueID), False)
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub

#End Region

#Region "PERMISSION - EditPermissions"

    Private Sub Display_EditPermissions()
        Dim nFolderId As Long

        If (_ItemType = "folder") Then
            _FolderData = _ContentApi.GetFolderById(_Id)
            nFolderId = _Id
            If _FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard OrElse _FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum Then
                _IsBoard = True
            ElseIf _FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Blog Then
                _IsBlog = True
            End If
        Else
            _ContentData = _ContentApi.GetContentById(_Id)
            _FolderData = _ContentApi.GetFolderById(_ContentData.FolderId)
            nFolderId = _ContentData.FolderId
        End If
        EditPermissionsToolBar()
        _PageData = New Collection
        Dim userpermission_data() As UserPermissionData
        Dim usergroup_data As UserGroupData
        Dim user_data As UserData
        Dim m_refUserAPI As New UserAPI
        If (Request.QueryString("base") = "group") Then
            userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, 0, Request.QueryString("PermID"), ContentAPI.PermissionUserType.All, ContentAPI.PermissionRequestType.All) 'cTmp = ContObj.GetOrderedItemPermissionsv2_0(cTmp, retString)
            usergroup_data = m_refUserAPI.GetUserGroupByIdForFolderAdmin(nFolderId, Request.QueryString("PermID"))
            _IsMembership = usergroup_data.IsMemberShipGroup
        Else
            userpermission_data = _ContentApi.GetUserPermissions(_Id, _ItemType, Request.QueryString("PermID"), "", ContentAPI.PermissionUserType.All, ContentAPI.PermissionRequestType.All)
            user_data = m_refUserAPI.GetUserByIDForFolderAdmin(nFolderId, Request.QueryString("PermID"))
            _IsMembership = user_data.IsMemberShip

        End If
        frm_itemid.Value = _Id
        frm_type.Value = Request.QueryString("type")
        frm_base.Value = _Base
        frm_permid.Value = Request.QueryString("PermID")
        frm_membership.Value = Request.QueryString("membership")


        If (_IsMembership) Then
            td_ep_membership.Visible = False
            hmembershiptype.Value = "1"
        Else
            td_ep_membership.InnerHtml = _StyleHelper.GetEnableAllPrompt()
            hmembershiptype.Value = "0"
        End If
        Populate_EditPermissionsGenericGrid(userpermission_data)
        Populate_EditPermissionsAdvancedGrid(userpermission_data)
    End Sub

    Private Sub Populate_EditPermissionsGenericGrid(ByVal userpermission_data As UserPermissionData())
        Dim strMsg As String = ""
        If (_Base = "group") Then
            If _IsMembership Then
                strMsg = "<span class=""membershipGroup"">" & _MessageHelper.GetMessage("generic membership user group label") & "</span>"
            Else
                strMsg = "<span class=""cmsGroup"">" & _MessageHelper.GetMessage("generic cms group label") & "</span>"
            End If

        Else
            If _IsMembership Then
                strMsg = "<span class=""membershipUser"">" & _MessageHelper.GetMessage("generic membership user label") & "</span>"
            Else
                strMsg = "<span class=""cmsUser"">" & _MessageHelper.GetMessage("generic cms user label") & "</span>"
            End If
        End If
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderStyle.CssClass = "left"
        colBound.ItemStyle.CssClass = "left"
        colBound.HeaderText = strMsg
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
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title")
        End If
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ADD"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title")
        End If
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DELETE"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title")
        End If
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

        If _IsBoard Then
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

        If (Not (IsNothing(userpermission_data))) Then
            dr = dt.NewRow()

            If (Request.QueryString("base") = "group") Then
                dr(0) = userpermission_data(0).DisplayGroupName
            Else
                dr(0) = userpermission_data(0).DisplayUserName
            End If

            dr(1) = "<input type=""checkbox"" name=""frm_readonly"" "
            If (userpermission_data(0).IsReadOnly) Then
                dr(1) += " checked=""checked"" "
            End If
            If (_IsMembership) Then
                dr(1) += " onclick=""return CheckReadOnlyForMembershipUser('frm_readonly');"" />"
            Else
                dr(1) += " onclick=""return CheckPermissionSettings('frm_readonly');"" />"
            End If


            dr(2) = "<input type=""checkbox"" name=""frm_edit"" "
            If (userpermission_data(0).CanEdit) Then
                dr(2) += " checked=""checked"" "
            End If
            If (_IsMembership And (Not _FolderData.IsCommunityFolder) And Not (_IsBlog)) Then
                dr(2) += " disabled=""disabled"" "
            End If
            dr(2) += "  onclick=""return CheckPermissionSettings('frm_edit');"" />"


            dr(3) = "<input type=""checkbox"" name=""frm_add"" "
            If (userpermission_data(0).CanAdd) Then
                dr(3) += " checked=""checked"" "
            End If
            If (_IsMembership And (Not _FolderData.IsCommunityFolder) And Not (_IsBoard Or _IsBlog)) Then
                dr(3) += " disabled=""disabled"" "
            End If
            dr(3) += "onclick=""return CheckPermissionSettings('frm_add');"" />"

            dr(4) = "<input type=""checkbox"" name=""frm_delete"" "
            If (userpermission_data(0).CanDelete) Then
                dr(4) += " checked=""checked"" "
            End If
            If (_IsMembership And Not (_IsBlog)) Then
                dr(4) += " disabled=""disabled"" "
            End If
            dr(4) += "onclick=""return CheckPermissionSettings('frm_delete');"" />"

            dr(5) = "<input type=""checkbox"" name=""frm_restore""  "
            If (userpermission_data(0).CanRestore) Then
                dr(5) += " checked=""checked"" "
            End If
            If (_IsMembership) Then
                dr(5) += " disabled=""disabled"" "
            End If
            dr(5) += "onclick=""return CheckPermissionSettings('frm_restore');"" />"

            dr(6) = "<input type=""checkbox"" name=""frm_libreadonly"" "
            If (userpermission_data(0).IsReadOnlyLib) Then
                dr(6) += " checked=""checked"" "
            End If
            dr(6) += "onclick=""return CheckPermissionSettings('frm_libreadonly');"" />"

            If _IsBoard Then
                ' add image/file
                dr(7) = "<input type=""checkbox"" name=""frm_addfiles""  "
                If (userpermission_data(0).CanAddToFileLib) Then
                    dr(7) += " checked=""checked"" "
                End If
                dr(7) += " onclick=""return CheckPermissionSettings('frm_addfiles');"" />"
                ' moderate
                dr(8) = "<input type=""checkbox"" name=""frm_addimages"" "
                If (userpermission_data(0).CanAddToImageLib) Then
                    dr(8) += " checked=""checked"" "
                End If
                dr(8) += "onclick=""return CheckPermissionSettings('frm_addimages');"" />"
            Else
                dr(7) = "<input type=""checkbox"" name=""frm_addimages""  "
                If (userpermission_data(0).CanAddToImageLib) Then
                    dr(7) += " checked=""checked"" "
                End If
                If (_IsMembership And Not (_IsBlog Or _FolderData.IsCommunityFolder)) Then
                    dr(7) += " disabled=""disabled"" "
                End If
                dr(7) += " onclick=""return CheckPermissionSettings('frm_addimages');"" />"


                dr(8) = "<input type=""checkbox"" name=""frm_addfiles"" "
                If (userpermission_data(0).CanAddToFileLib) Then
                    dr(8) += " checked=""checked"" "
                End If
                If (_IsMembership And Not (_IsBlog Or _FolderData.IsCommunityFolder)) Then
                    dr(8) += " disabled=""disabled"" "
                End If
                dr(8) += "onclick=""return CheckPermissionSettings('frm_addfiles');"" />"
            End If

            dr(9) = "<input type=""checkbox"" name=""frm_addhyperlinks"" "
            If (userpermission_data(0).CanAddToHyperlinkLib) Then
                dr(9) += " checked=""checked"" "
            End If
            If (_IsMembership) Then
                dr(9) += " disabled=""disabled"" "
            End If
            dr(9) += "onclick=""return CheckPermissionSettings('frm_addhyperlinks');"" />"

            dr(10) = "<input type=""checkbox"" name=""frm_overwritelib"" "
            If (userpermission_data(0).CanOverwriteLib) Then
                dr(10) += " checked=""checked"" "
            End If
            If (_IsMembership) Then
                dr(10) += " disabled=""disabled"" "
            End If
            dr(10) += "onclick=""return CheckPermissionSettings('frm_overwritelib');"" />"

            dt.Rows.Add(dr)
        End If
        Dim dv As New DataView(dt)
        PermissionsGenericGrid.DataSource = dv
        PermissionsGenericGrid.DataBind()
    End Sub

    Private Sub Populate_EditPermissionsAdvancedGrid(ByVal userpermission_data As UserPermissionData())
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strMsg As String = ""

        If (_Base = "group") Then
            strMsg = "<span class=""cmsGroup"">" & _MessageHelper.GetMessage("generic User Group Name") & "</span>"
        Else
            strMsg = "<span class=""cmsUser"">" & _MessageHelper.GetMessage("generic cms user label") & "</span>"
        End If

        colBound.DataField = "TITLE"
        colBound.HeaderText = strMsg
        colBound.HeaderStyle.CssClass = "title-header left"
        colBound.ItemStyle.CssClass = "left"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        If Not (_IsBoard) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "COLLECTIONS"
            colBound.HeaderText = _MessageHelper.GetMessage("generic collection title")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.ItemStyle.CssClass = "center"
            PermissionsAdvancedGrid.Columns.Add(colBound)
        End If

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ADDFLD"
        If _IsBoard Then
            colBound.HeaderText = "Add Forum"
            colBound.ItemStyle.CssClass = "center"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic add folders title")
            colBound.ItemStyle.CssClass = "center"
        End If
        colBound.HeaderStyle.CssClass = "title-header"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITFLD"
        If _IsBoard Then
            colBound.HeaderText = "Edit Forum"
            colBound.ItemStyle.CssClass = "center"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic edit folders title")
            colBound.ItemStyle.CssClass = "center"
        End If
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "center"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DELETEFLD"
        If _IsBoard Then
            colBound.HeaderText = "Delete Forum"
            colBound.ItemStyle.CssClass = "center"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic delete folders title")
            colBound.ItemStyle.CssClass = "center"
        End If
        colBound.HeaderStyle.CssClass = "title-header"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        If Not (_IsBoard) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "TRAVERSE"
            colBound.HeaderText = _MessageHelper.GetMessage("generic traverse folder title")
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderStyle.CssClass = "title-header"
            PermissionsAdvancedGrid.Columns.Add(colBound)
        End If

        Dim m_refSiteApi As New SiteAPI
        Dim setting_data As New SettingsData
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
        If setting_data.EnablePreApproval Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "ModifyPreapproval"
            colBound.HeaderText = "Modify Preapproval"
            colBound.ItemStyle.CssClass = "center"
            colBound.HeaderStyle.CssClass = "title-header"
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

        Dim bInherited As Boolean = False
        If (_ItemType = "folder") Then
            bInherited = _FolderData.Inherited
        Else
            bInherited = _ContentData.IsInherited
        End If

        Dim i As Integer = 0


        If (Not (IsNothing(userpermission_data))) Then
            dr = dt.NewRow()


            If (Request.QueryString("base") = "group") Then
                dr(0) = userpermission_data(0).DisplayGroupName
            Else
                dr(0) = userpermission_data(0).DisplayUserName
            End If

            frm_navigation.Value = CInt(userpermission_data(0).IsCollections)
            dr(1) = "<input type=""checkbox"" name=""frm_navigation"" "
            If (userpermission_data(0).IsCollections) Then
                dr(1) += " checked=""checked"" "
            End If
            If (_IsMembership) Then
                dr(1) += " disabled=""disabled"" "
            End If
            dr(1) += " onclick=""return CheckPermissionSettings('frm_navigation');"" />"

            frm_add_folders.Value = CInt(userpermission_data(i).CanAddFolders)
            dr(2) = "<input type=""checkbox"" name=""frm_add_folders"" "
            If (userpermission_data(i).CanAddFolders) Then
                dr(2) += " checked=""checked"" "
            End If
            If (_IsMembership) Then
                dr(2) += " disabled=""disabled"" "
            End If
            dr(2) += "onclick=""return CheckPermissionSettings('frm_add_folders');"" />"

            frm_edit_folders.Value = CInt(userpermission_data(i).CanEditFolders)
            dr(3) = "<input type=""checkbox"" name=""frm_edit_folders"" "
            If (userpermission_data(i).CanEditFolders) Then
                dr(3) += " checked=""checked"" "
            End If
            If (_IsMembership) Then
                dr(3) += " disabled=""disabled"" "
            End If
            dr(3) += "onclick=""return CheckPermissionSettings('frm_edit_folders');"" />"

            frm_delete_folders.Value = CInt(userpermission_data(i).CanDeleteFolders)
            dr(4) = "<input type=""checkbox"" name=""frm_delete_folders"" "
            If (userpermission_data(i).CanDeleteFolders) Then
                dr(4) += " checked=""checked"" "
            End If
            If (_IsMembership) Then
                dr(4) += " disabled=""disabled"" "
            End If
            dr(4) += " onclick=""return CheckPermissionSettings('frm_delete_folders');"" />"

            frm_transverse_folder.Value = CInt(userpermission_data(i).CanTraverseFolders)
            dr(5) = "<input type=""checkbox"" name=""frm_transverse_folder"" "
            If (userpermission_data(i).CanTraverseFolders) Then
                dr(5) += " checked=""checked"" "
            End If
            If (_IsMembership) Then
                dr(5) += " disabled=""disabled"" "
            End If
            dr(5) += "onclick=""return CheckPermissionSettings('frm_transverse_folder');"" />"

            If setting_data.EnablePreApproval Then
                frm_edit_preapproval.Value = CInt(userpermission_data(i).CanEditApprovals)
                dr(6) = "<input type=""checkbox"" name=""frm_edit_preapproval"" "
                If (userpermission_data(i).CanEditApprovals) Then
                    dr(6) += " checked=""checked"" "
                End If
                If (_IsMembership) Then
                    dr(6) += " disabled=""disabled"" "
                End If
                dr(6) += "onclick=""return CheckPermissionSettings('frm_edit_preapproval');"" />"
            End If
            dt.Rows.Add(dr)
        End If
        Dim dv As New DataView(dt)
        PermissionsAdvancedGrid.DataSource = dv
        PermissionsAdvancedGrid.DataBind()
    End Sub

    Private Sub EditPermissionsToolBar()
        Dim result As New System.Text.StringBuilder
        Dim WorkareaTitlebarTitle As String = ""

        If (_ItemType = "folder") Then
            WorkareaTitlebarTitle = _MessageHelper.GetMessage("edit folder permissions") & " """ & _FolderData.Name & """"
        Else
            WorkareaTitlebarTitle = _MessageHelper.GetMessage("edit content permissions") & " """ & _ContentData.Title & """"
        End If
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(WorkareaTitlebarTitle)

        result.Append("<table><tbody><tr>")
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt update button text (permissions)"), _MessageHelper.GetMessage("btn update"), "onclick=""javascript:return SubmitForm('frmContent', 'CheckEditPermissions()');"""))
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/back.png", "content.aspx?LangType=" & _ContentLanguage & "&action=ViewPermissions&id=" & _Id & "&type=" & Request.QueryString("type") & "&membership=" & Request.QueryString("membership"), _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        result.Append("<td>")
        If _IsBoard = True Then
            result.Append(_StyleHelper.GetHelpButton("editboardperms"))
        Else
            result.Append(_StyleHelper.GetHelpButton(_PageAction))
        End If
        result.Append("</td>")
        result.Append("</tr></tbody></table>")
        htmToolBar.InnerHtml = result.ToString()
    End Sub

#End Region

#Region "PERMISSION - AddPermissions"

    Private Sub Display_AddPermissions()
        Dim usergroup_data As UserGroupData
        Dim userGroupDataList As New System.Collections.Generic.List(Of UserGroupData)
        Dim user_data As UserData
        Dim userDataList As New System.Collections.Generic.List(Of UserData)
        Dim m_refUserAPI As New UserAPI
        Dim nFolderId As Long

        frm_itemid.Value = _Id
        frm_type.Value = Request.QueryString("type")
        frm_base.Value = Request.QueryString("base")
        frm_permid.Value = Request.QueryString("PermID")
        frm_membership.Value = Request.QueryString("membership")

        If (_ItemType = "folder") Then
            _FolderData = _ContentApi.GetFolderById(_Id)
            nFolderId = _Id
            If _FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionBoard OrElse _FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum Then
                _IsBoard = True
            ElseIf _FolderData.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Blog Then
                _IsBlog = True
            End If
        Else
            _ContentData = _ContentApi.GetContentById(_Id)
            _FolderData = _ContentApi.GetFolderById(_ContentData.FolderId)
            nFolderId = _ContentData.FolderId
        End If
        AddPermissionsToolBar()
        Select Case Request.QueryString("base")
            Case "group"
                usergroup_data = m_refUserAPI.GetUserGroupByIdForFolderAdmin(nFolderId, Request.QueryString("PermID"))
                Populate_AddPermissionsGenericGrid(usergroup_data)
                Populate_AddPermissionsAdvancedGrid(usergroup_data)
                _IsMembership = usergroup_data.IsMemberShipGroup
            Case "user"
                user_data = m_refUserAPI.GetUserByIDForFolderAdmin(nFolderId, Request.QueryString("PermID"))
                Populate_AddPermissionsGenericGrid(user_data)
                Populate_AddPermissionsAdvancedGrid(user_data)
                _IsMembership = user_data.IsMemberShip
            Case Else
                Dim Groups() As String = Request.QueryString("groupIDS").Split(",")
                Dim Users() As String = Request.QueryString("userIDS").Split(",")
                Dim groupCount As Integer = 0
                Dim userCount As Integer = 0

                If Request.QueryString("groupIDS") <> "" Then
                    For groupCount = 0 To Groups.Length - 1
                        userGroupDataList.Add(m_refUserAPI.GetUserGroupByIdForFolderAdmin(nFolderId, Groups(groupCount)))
                    Next
                    _IsMembership = userGroupDataList(0).IsMemberShipGroup
                End If
                If Request.QueryString("userIDS") <> "" Then
                    For userCount = 0 To Users.Length - 1
                        userDataList.Add(m_refUserAPI.GetUserByIDForFolderAdmin(nFolderId, Users(userCount)))
                    Next
                    _IsMembership = userDataList(0).IsMemberShip
                End If
                Populate_AddPermissionsGenericGridForUsersAndGroup(userGroupDataList, userDataList)
                Populate_AddPermissionsAdvancedGridForUsersAndGroup(userGroupDataList, userDataList)
        End Select
        

        If (_IsMembership) Then
            td_ep_membership.Visible = False
            hmembershiptype.Value = "1"
        Else
            td_ep_membership.InnerHtml = _StyleHelper.GetEnableAllPrompt()
            hmembershiptype.Value = "0"
        End If
    End Sub

    Private Sub Populate_AddPermissionsGenericGrid(ByVal data As Object)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strMsg As String = ""

        If (_Base = "group") Then
            If _IsMembership Then
                strMsg = "<span class=""membershipGroup"">" & _MessageHelper.GetMessage("generic membership user group label") & "</span>"
            Else
                strMsg = "<span class=""cmsGroup"">" & _MessageHelper.GetMessage("generic cms group label") & "</span>"
            End If

        Else
            If _IsMembership Then
                strMsg = "<span class=""membershipUser"">" & _MessageHelper.GetMessage("generic membership user label") & "</span>"
            Else
                strMsg = "<span class=""cmsUser"">" & _MessageHelper.GetMessage("generic cms user label") & "</span>"
            End If
        End If

        colBound.DataField = "TITLE"
        colBound.HeaderStyle.CssClass = "title-header left"
        colBound.ItemStyle.CssClass = "left"
        colBound.HeaderText = strMsg
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
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title")
        End If
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ADD"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title")
        End If
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DELETE"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title")
        End If
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
            colBound.DataField = "GREAD"
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm postreply")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDFILE"
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm addimgfil")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADD"
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm moderate")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)
        Else
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GREAD"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Library title") & " " & _MessageHelper.GetMessage("generic read only")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADD"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " " & _MessageHelper.GetMessage("generic Images")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDFILE"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " " & _MessageHelper.GetMessage("generic Files")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDHYP"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " " & _MessageHelper.GetMessage("generic Hyperlinks")
            colBound.HeaderStyle.CssClass = "title-header center"
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

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("READ", GetType(String)))
        dt.Columns.Add(New DataColumn("EDIT", GetType(String)))
        dt.Columns.Add(New DataColumn("ADD", GetType(String)))
        dt.Columns.Add(New DataColumn("DELETE", GetType(String)))
        dt.Columns.Add(New DataColumn("RESTORE", GetType(String)))
        If _IsBoard Then
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
        If (Not (IsNothing(data))) Then
            dr = dt.NewRow()

            If (Request.QueryString("base") = "group") Then
                dr(0) = data.GroupDisplayName
            Else
                dr(0) = data.DisplayUserName
            End If

            dr(1) = "<input type=""checkbox"" name=""frm_readonly"" "
            If (_IsMembership) Then
                dr(1) += " checked=""checked"" onclick=""return CheckReadOnlyForMembershipUser('frm_readonly');"">"
            Else
                dr(1) += " onclick=""return CheckPermissionSettings('frm_readonly');"" />"
            End If

            dr(2) = "<input type=""checkbox"" name=""frm_edit"" "
            If (_IsMembership And (Not _FolderData.IsCommunityFolder) And Not (_IsBlog)) Then
                dr(2) += " disabled=""disabled"" "
            End If
            dr(2) += "  onclick=""return CheckPermissionSettings('frm_edit');"" />"


            dr(3) = "<input type=""checkbox"" name=""frm_add"" "
            If (_IsMembership And (Not _FolderData.IsCommunityFolder) And Not (_IsBoard Or _IsBlog)) Then
                dr(3) += " disabled=""disabled"" "
            End If
            dr(3) += "onclick=""return CheckPermissionSettings('frm_add');"" />"

            dr(4) = "<input type=""checkbox"" name=""frm_delete"" "
            If (_IsMembership And Not (_IsBlog)) Then
                dr(4) += " disabled=""disabled"" "
            End If
            dr(4) += "onclick=""return CheckPermissionSettings('frm_delete');"" />"

            dr(5) = "<input type=""checkbox"" name=""frm_restore""  "
            If (_IsMembership) Then
                dr(5) += " disabled=""disabled"" "
            End If
            dr(5) += "onclick=""return CheckPermissionSettings('frm_restore');"" />"

            dr(6) = "<input type=""checkbox"" name=""frm_libreadonly"" "
            If (_IsMembership) And Not (_IsBoard Or _IsBlog Or _FolderData.IsCommunityFolder) Then
                dr(6) += " checked=""checked"" "
            End If
            dr(6) += "onclick=""return CheckPermissionSettings('frm_libreadonly');"" />"

            If _IsBoard = True Then
                dr(7) = "<input type=""checkbox"" name=""frm_addfiles"" "
                dr(7) += "onclick=""return CheckPermissionSettings('frm_addfiles');"" />"

                dr(8) = "<input type=""checkbox"" name=""frm_addimages""  "
                dr(8) += " onclick=""return CheckPermissionSettings('frm_addimages');"" />"
            Else
                dr(7) = "<input type=""checkbox"" name=""frm_addimages""  "
                If (_IsMembership And Not (_IsBlog Or _FolderData.IsCommunityFolder)) Then
                    dr(7) += " disabled=""disabled"" "
                End If
                dr(7) += " onclick=""return CheckPermissionSettings('frm_addimages');"" />"

                dr(8) = "<input type=""checkbox"" name=""frm_addfiles"" "
                If (_IsMembership And Not (_IsBlog Or _FolderData.IsCommunityFolder)) Then
                    dr(8) += " disabled=""disabled"" "
                End If
                dr(8) += "onclick=""return CheckPermissionSettings('frm_addfiles');"" />"
            End If

            dr(9) = "<input type=""checkbox"" name=""frm_addhyperlinks"" "
            If (_IsMembership) Then
                dr(9) += " disabled=""disabled"" "
            End If
            dr(9) += "onclick=""return CheckPermissionSettings('frm_addhyperlinks');"" />"

            dr(10) = "<input type=""checkbox"" name=""frm_overwritelib"" "
            If (_IsMembership) Then
                dr(10) += " disabled=""disabled"" "
            End If
            dr(10) += "onclick=""return CheckPermissionSettings('frm_overwritelib');"" />"

            dt.Rows.Add(dr)
        End If
        Dim dv As New DataView(dt)
        PermissionsGenericGrid.DataSource = dv
        PermissionsGenericGrid.DataBind()
    End Sub

    Private Sub Populate_AddPermissionsAdvancedGrid(ByVal data As Object)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strMsg As String = ""

        If (_Base = "group") Then
            strMsg = "<span class=""cmsGroup"">" & _MessageHelper.GetMessage("generic cms group label") & "</span>"
        Else
            strMsg = "<span class=""cmsUser"">" & _MessageHelper.GetMessage("generic cms user label") & "</span>"
        End If

        colBound.DataField = "TITLE"
        colBound.HeaderText = strMsg
        colBound.HeaderStyle.CssClass = "title-header left"
        colBound.ItemStyle.CssClass = "left"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        If Not (_IsBoard) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "COLLECTIONS"
            colBound.HeaderText = _MessageHelper.GetMessage("generic collection title")
            colBound.HeaderStyle.CssClass = "title-header center"
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
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITFLD"
        If _IsBoard Then
            colBound.HeaderText = "Edit Forum"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic edit folders title")
        End If
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DELETEFLD"
        If _IsBoard Then
            colBound.HeaderText = "Delete Forum"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic delete folders title")
        End If
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        If Not (_IsBoard) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "TRAVERSE"
            colBound.HeaderText = _MessageHelper.GetMessage("generic traverse folder title")
            colBound.HeaderStyle.CssClass = "title-header center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsAdvancedGrid.Columns.Add(colBound)
        End If

        Dim m_refSiteApi As New SiteAPI
        Dim setting_data As New SettingsData
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
        If setting_data.EnablePreApproval Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "ModifyPreapproval"
            colBound.HeaderText = "Modify Preapproval"
            colBound.HeaderStyle.CssClass = "title-header center"
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
        Dim bInherited As Boolean = False
        If (_ItemType = "folder") Then
            bInherited = _FolderData.Inherited
        Else
            bInherited = _ContentData.IsInherited
        End If

        Dim i As Integer = 0


        If (Not (IsNothing(data))) Then
            dr = dt.NewRow()

            If (Request.QueryString("base") = "group") Then
                dr(0) = data.GroupDisplayName
            Else
                dr(0) = data.DisplayUserName
            End If

            dr(1) = "<input type=""checkbox""  id=""frm_navigation"" name=""frm_navigation"" "
            dr(1) += "onclick=""return CheckPermissionSettings('frm_navigation');"" />"

            dr(2) = "<input type=""checkbox"" id=""frm_add_folders""  name=""frm_add_folders"" "
            dr(2) += "onclick=""return CheckPermissionSettings('frm_add_folders');"" />"

            dr(3) = "<input type=""checkbox"" id=""frm_edit_folders"" name=""frm_edit_folders"" "
            dr(3) += "onclick=""return CheckPermissionSettings('frm_edit_folders');"" />"

            dr(4) = "<input type=""checkbox"" id=""frm_delete_folders"" name=""frm_delete_folders"" "
            dr(4) += "onclick=""return CheckPermissionSettings('frm_delete_folders');"" />"

            dr(5) = "<input type=""checkbox"" id=""frm_transverse_folder"" name=""frm_transverse_folder"" checked=""" & traverseFolder & """  "
            dr(5) += "onclick=""return CheckPermissionSettings('frm_transverse_folder');"" />"

            If setting_data.EnablePreApproval Then
                dr(6) = "<input type=""checkbox"" id=""frm_edit_preapproval"" name=""frm_edit_preapproval"" "
                dr(6) += "onclick=""return CheckPermissionSettings('frm_edit_preapproval');"" />"
            End If
            dt.Rows.Add(dr)
        End If
        Dim dv As New DataView(dt)
        PermissionsAdvancedGrid.DataSource = dv
        PermissionsAdvancedGrid.DataBind()
    End Sub
    Private Sub Populate_AddPermissionsGenericGridForUsersAndGroup(ByVal groupData As System.Collections.Generic.List(Of UserGroupData), ByVal userData As System.Collections.Generic.List(Of UserData))
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strMsg As String = ""
        Dim i As Integer = 0
        Dim j As Integer = j

        colBound.DataField = "TITLE"
        colBound.HeaderStyle.CssClass = "title-header left"
        colBound.ItemStyle.CssClass = "left"
        colBound.HeaderText = strMsg
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
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Edit title")
        End If
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ADD"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title")
        End If
        PermissionsGenericGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DELETE"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        If _IsBoard Then
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title") & " Topic"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic Delete title")
        End If
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
            colBound.DataField = "GREAD"
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm postreply")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDFILE"
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm addimgfil")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADD"
            colBound.HeaderText = _MessageHelper.GetMessage("lbl perm moderate")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)
        Else
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GREAD"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Library title") & " " & _MessageHelper.GetMessage("generic read only")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADD"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " " & _MessageHelper.GetMessage("generic Images")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDFILE"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " " & _MessageHelper.GetMessage("generic Files")
            colBound.HeaderStyle.CssClass = "center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsGenericGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "GADDHYP"
            colBound.HeaderText = _MessageHelper.GetMessage("generic Add title") & " " & _MessageHelper.GetMessage("generic Hyperlinks")
            colBound.HeaderStyle.CssClass = "title-header center"
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

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("READ", GetType(String)))
        dt.Columns.Add(New DataColumn("EDIT", GetType(String)))
        dt.Columns.Add(New DataColumn("ADD", GetType(String)))
        dt.Columns.Add(New DataColumn("DELETE", GetType(String)))
        dt.Columns.Add(New DataColumn("RESTORE", GetType(String)))
        If _IsBoard Then
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

        If (groupData.Count > 0) Then
            For i = 0 To groupData.Count - 1
                dr = dt.NewRow()
                If groupData(i).IsMemberShipGroup Then
                    dr(0) = "<span class=""membershipGroup"">" & groupData(i).GroupDisplayName & "</span>"
                Else
                    dr(0) = "<span class=""cmsGroup"">" & groupData(i).GroupDisplayName & "</span>"
                End If

                dr(1) = "<input type=""checkbox"" name=""frm_readonly"" "
                If (_IsMembership) Then
                    dr(1) += " checked=""checked"" onclick=""return CheckReadOnlyForMembershipUser('frm_readonly');"">"
                Else
                    dr(1) += " onclick=""return CheckPermissionSettings('frm_readonly');"" />"
                End If

                dr(2) = "<input type=""checkbox"" name=""frm_edit"" "
                If (_IsMembership And (Not _FolderData.IsCommunityFolder) And Not (_IsBlog)) Then
                    dr(2) += " disabled=""disabled"" "
                End If
                dr(2) += "  onclick=""return CheckPermissionSettings('frm_edit');"" />"


                dr(3) = "<input type=""checkbox"" name=""frm_add"" "
                If (_IsMembership And (Not _FolderData.IsCommunityFolder) And Not (_IsBoard Or _IsBlog)) Then
                    dr(3) += " disabled=""disabled"" "
                End If
                dr(3) += "onclick=""return CheckPermissionSettings('frm_add');"" />"

                dr(4) = "<input type=""checkbox"" name=""frm_delete"" "
                If (_IsMembership And Not (_IsBlog)) Then
                    dr(4) += " disabled=""disabled"" "
                End If
                dr(4) += "onclick=""return CheckPermissionSettings('frm_delete');"" />"

                dr(5) = "<input type=""checkbox"" name=""frm_restore""  "
                If (_IsMembership) Then
                    dr(5) += " disabled=""disabled"" "
                End If
                dr(5) += "onclick=""return CheckPermissionSettings('frm_restore');"" />"

                dr(6) = "<input type=""checkbox"" name=""frm_libreadonly"" "
                If (_IsMembership) And Not (_IsBoard Or _IsBlog Or _FolderData.IsCommunityFolder) Then
                    dr(6) += " checked=""checked"" "
                End If
                dr(6) += "onclick=""return CheckPermissionSettings('frm_libreadonly');"" />"

                If _IsBoard = True Then
                    dr(7) = "<input type=""checkbox"" name=""frm_addfiles"" "
                    dr(7) += "onclick=""return CheckPermissionSettings('frm_addfiles');"" />"

                    dr(8) = "<input type=""checkbox"" name=""frm_addimages""  "
                    dr(8) += " onclick=""return CheckPermissionSettings('frm_addimages');"" />"
                Else
                    dr(7) = "<input type=""checkbox"" name=""frm_addimages""  "
                    If (_IsMembership And Not (_IsBlog Or _FolderData.IsCommunityFolder)) Then
                        dr(7) += " disabled=""disabled"" "
                    End If
                    dr(7) += " onclick=""return CheckPermissionSettings('frm_addimages');"" />"

                    dr(8) = "<input type=""checkbox"" name=""frm_addfiles"" "
                    If (_IsMembership And Not (_IsBlog Or _FolderData.IsCommunityFolder)) Then
                        dr(8) += " disabled=""disabled"" "
                    End If
                    dr(8) += "onclick=""return CheckPermissionSettings('frm_addfiles');"" />"
                End If

                dr(9) = "<input type=""checkbox"" name=""frm_addhyperlinks"" "
                If (_IsMembership) Then
                    dr(9) += " disabled=""disabled"" "
                End If
                dr(9) += "onclick=""return CheckPermissionSettings('frm_addhyperlinks');"" />"

                dr(10) = "<input type=""checkbox"" name=""frm_overwritelib"" "
                If (_IsMembership) Then
                    dr(10) += " disabled=""disabled"" "
                End If
                dr(10) += "onclick=""return CheckPermissionSettings('frm_overwritelib');"" />"

                dt.Rows.Add(dr)
            Next
        End If
        If (userData.Count > 0) Then
            For j = 0 To userData.Count - 1
                dr = dt.NewRow()
                If userData(j).IsMemberShip Then
                    dr(0) = "<span class=""membershipUser"">" & userData(j).Username & "</span>"
                Else
                    dr(0) = "<span class=""cmsUser"">" & userData(j).Username & "</span>"
                End If

                dr(1) = "<input type=""checkbox"" name=""frm_readonly"" "
                If (_IsMembership) Then
                    dr(1) += " checked=""checked"" onclick=""return CheckReadOnlyForMembershipUser('frm_readonly');"">"
                Else
                    dr(1) += " onclick=""return CheckPermissionSettings('frm_readonly');"" />"
                End If

                dr(2) = "<input type=""checkbox"" name=""frm_edit"" "
                If (_IsMembership And (Not _FolderData.IsCommunityFolder) And Not (_IsBlog)) Then
                    dr(2) += " disabled=""disabled"" "
                End If
                dr(2) += "  onclick=""return CheckPermissionSettings('frm_edit');"" />"


                dr(3) = "<input type=""checkbox"" name=""frm_add"" "
                If (_IsMembership And (Not _FolderData.IsCommunityFolder) And Not (_IsBoard Or _IsBlog)) Then
                    dr(3) += " disabled=""disabled"" "
                End If
                dr(3) += "onclick=""return CheckPermissionSettings('frm_add');"" />"

                dr(4) = "<input type=""checkbox"" name=""frm_delete"" "
                If (_IsMembership And Not (_IsBlog)) Then
                    dr(4) += " disabled=""disabled"" "
                End If
                dr(4) += "onclick=""return CheckPermissionSettings('frm_delete');"" />"

                dr(5) = "<input type=""checkbox"" name=""frm_restore""  "
                If (_IsMembership) Then
                    dr(5) += " disabled=""disabled"" "
                End If
                dr(5) += "onclick=""return CheckPermissionSettings('frm_restore');"" />"

                dr(6) = "<input type=""checkbox"" name=""frm_libreadonly"" "
                If (_IsMembership) And Not (_IsBoard Or _IsBlog Or _FolderData.IsCommunityFolder) Then
                    dr(6) += " checked=""checked"" "
                End If
                dr(6) += "onclick=""return CheckPermissionSettings('frm_libreadonly');"" />"

                If _IsBoard = True Then
                    dr(7) = "<input type=""checkbox"" name=""frm_addfiles"" "
                    dr(7) += "onclick=""return CheckPermissionSettings('frm_addfiles');"" />"

                    dr(8) = "<input type=""checkbox"" name=""frm_addimages""  "
                    dr(8) += " onclick=""return CheckPermissionSettings('frm_addimages');"" />"
                Else
                    dr(7) = "<input type=""checkbox"" name=""frm_addimages""  "
                    If (_IsMembership And Not (_IsBlog Or _FolderData.IsCommunityFolder)) Then
                        dr(7) += " disabled=""disabled"" "
                    End If
                    dr(7) += " onclick=""return CheckPermissionSettings('frm_addimages');"" />"

                    dr(8) = "<input type=""checkbox"" name=""frm_addfiles"" "
                    If (_IsMembership And Not (_IsBlog Or _FolderData.IsCommunityFolder)) Then
                        dr(8) += " disabled=""disabled"" "
                    End If
                    dr(8) += "onclick=""return CheckPermissionSettings('frm_addfiles');"" />"
                End If

                dr(9) = "<input type=""checkbox"" name=""frm_addhyperlinks"" "
                If (_IsMembership) Then
                    dr(9) += " disabled=""disabled"" "
                End If
                dr(9) += "onclick=""return CheckPermissionSettings('frm_addhyperlinks');"" />"

                dr(10) = "<input type=""checkbox"" name=""frm_overwritelib"" "
                If (_IsMembership) Then
                    dr(10) += " disabled=""disabled"" "
                End If
                dr(10) += "onclick=""return CheckPermissionSettings('frm_overwritelib');"" />"

                dt.Rows.Add(dr)
            Next
        End If
        Dim dv As New DataView(dt)
        PermissionsGenericGrid.DataSource = dv
        PermissionsGenericGrid.DataBind()
    End Sub

    Private Sub Populate_AddPermissionsAdvancedGridForUsersAndGroup(ByVal groupData As System.Collections.Generic.List(Of UserGroupData), ByVal userData As System.Collections.Generic.List(Of UserData))
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strMsg As String = ""

        colBound.DataField = "TITLE"
        colBound.HeaderText = strMsg
        colBound.HeaderStyle.CssClass = "title-header left"
        colBound.ItemStyle.CssClass = "left"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        If Not (_IsBoard) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "COLLECTIONS"
            colBound.HeaderText = _MessageHelper.GetMessage("generic collection title")
            colBound.HeaderStyle.CssClass = "title-header center"
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
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITFLD"
        If _IsBoard Then
            colBound.HeaderText = "Edit Forum"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic edit folders title")
        End If
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DELETEFLD"
        If _IsBoard Then
            colBound.HeaderText = "Delete Forum"
        Else
            colBound.HeaderText = _MessageHelper.GetMessage("generic delete folders title")
        End If
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        PermissionsAdvancedGrid.Columns.Add(colBound)

        If Not (_IsBoard) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "TRAVERSE"
            colBound.HeaderText = _MessageHelper.GetMessage("generic traverse folder title")
            colBound.HeaderStyle.CssClass = "title-header center"
            colBound.ItemStyle.CssClass = "center"
            PermissionsAdvancedGrid.Columns.Add(colBound)
        End If

        Dim m_refSiteApi As New SiteAPI
        Dim setting_data As New SettingsData
        setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
        If setting_data.EnablePreApproval Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "ModifyPreapproval"
            colBound.HeaderText = "Modify Preapproval"
            colBound.HeaderStyle.CssClass = "title-header center"
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
        Dim bInherited As Boolean = False
        If (_ItemType = "folder") Then
            bInherited = _FolderData.Inherited
        Else
            bInherited = _ContentData.IsInherited
        End If

        Dim i As Integer = 0
        Dim j As Integer = 0

        If groupData.Count > 0 Then
            For i = 0 To groupData.Count - 1

                dr = dt.NewRow()
                If groupData(i).IsMemberShipGroup Then
                    dr(0) = "<span class=""membershipGroup"">" & groupData(i).GroupDisplayName & "</span>"
                Else
                    dr(0) = "<span class=""cmsGroup"">" & groupData(i).GroupDisplayName & "</span>"
                End If

                dr(1) = "<input type=""checkbox""  id=""frm_navigation"" name=""frm_navigation"" "
                dr(1) += "onclick=""return CheckPermissionSettings('frm_navigation');"" />"

                dr(2) = "<input type=""checkbox"" id=""frm_add_folders""  name=""frm_add_folders"" "
                dr(2) += "onclick=""return CheckPermissionSettings('frm_add_folders');"" />"

                dr(3) = "<input type=""checkbox"" id=""frm_edit_folders"" name=""frm_edit_folders"" "
                dr(3) += "onclick=""return CheckPermissionSettings('frm_edit_folders');"" />"

                dr(4) = "<input type=""checkbox"" id=""frm_delete_folders"" name=""frm_delete_folders"" "
                dr(4) += "onclick=""return CheckPermissionSettings('frm_delete_folders');"" />"

                dr(5) = "<input type=""checkbox"" id=""frm_transverse_folder"" name=""frm_transverse_folder"" checked=""" & traverseFolder & """  "
                dr(5) += "onclick=""return CheckPermissionSettings('frm_transverse_folder');"" />"

                If setting_data.EnablePreApproval Then
                    dr(6) = "<input type=""checkbox"" id=""frm_edit_preapproval"" name=""frm_edit_preapproval"" "
                    dr(6) += "onclick=""return CheckPermissionSettings('frm_edit_preapproval');"" />"
                End If
                dt.Rows.Add(dr)
            Next
        End If
        If userData.Count > 0 Then
            For j = 0 To userData.Count - 1
                dr = dt.NewRow()
                If userData(j).IsMemberShip Then
                    dr(0) = "<span class=""membershipUser"">" & userData(j).Username & "</span>"
                Else
                    dr(0) = "<span class=""cmsUser"">" & userData(j).Username & "</span>"
                End If

                dr(1) = "<input type=""checkbox""  id=""frm_navigation"" name=""frm_navigation"" "
                dr(1) += "onclick=""return CheckPermissionSettings('frm_navigation');"">"

                dr(2) = "<input type=""checkbox"" id=""frm_add_folders""  name=""frm_add_folders"" "
                dr(2) += "onclick=""return CheckPermissionSettings('frm_add_folders');"" />"

                dr(3) = "<input type=""checkbox"" id=""frm_edit_folders"" name=""frm_edit_folders"" "
                dr(3) += "onclick=""return CheckPermissionSettings('frm_edit_folders');"" />"

                dr(4) = "<input type=""checkbox"" id=""frm_delete_folders"" name=""frm_delete_folders"" "
                dr(4) += "onclick=""return CheckPermissionSettings('frm_delete_folders');"" />"

                dr(5) = "<input type=""checkbox"" id=""frm_transverse_folder"" name=""frm_transverse_folder"" checked=""" & traverseFolder & """  "
                dr(5) += "onclick=""return CheckPermissionSettings('frm_transverse_folder');"" />"

                If setting_data.EnablePreApproval Then
                    dr(6) = "<input type=""checkbox"" id=""frm_edit_preapproval"" name=""frm_edit_preapproval"" "
                    dr(6) += "onclick=""return CheckPermissionSettings('frm_edit_preapproval');"" />"
                End If
                dt.Rows.Add(dr)
            Next
        End If
        Dim dv As New DataView(dt)
        PermissionsAdvancedGrid.DataSource = dv
        PermissionsAdvancedGrid.DataBind()
    End Sub

    Private Sub AddPermissionsToolBar()
        Dim result As New System.Text.StringBuilder
        Dim WorkareaTitlebarTitle As String = ""
        If (_ItemType = "folder") Then
            WorkareaTitlebarTitle = _MessageHelper.GetMessage("add folder permissions") & " """ & _FolderData.Name & """"
        Else
            WorkareaTitlebarTitle = _MessageHelper.GetMessage("add content permissions") & " """ & _ContentData.Title & """"
        End If
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(WorkareaTitlebarTitle)
        result.Append("<table><tr>")
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/save.png", "#", _MessageHelper.GetMessage("add permissions"), _MessageHelper.GetMessage("btn save"), "onclick=""javascript:return SubmitForm('frmContent', 'CheckForAnyPermissions()');"""))
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/back.png", "content.aspx?action=SelectPermissions&id=" & _Id & "&type=" & Request.QueryString("type") & "&membership=" & Request.QueryString("membership") & "&LangType=" & _ContentLanguage, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        result.Append("<td>")
        If _IsBoard Then
            result.Append(_StyleHelper.GetHelpButton("addboardperms"))
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
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronUITabsCss)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)

        'JS
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub

#End Region


End Class