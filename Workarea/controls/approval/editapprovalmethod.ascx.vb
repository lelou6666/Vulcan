Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class editapprovalmethod
    Inherits System.Web.UI.UserControl


#Region "Member Variables"

    Protected _ContentApi As New ContentAPI
    Protected _StyleHelper As New StyleHelper
    Protected _MessageHelper As Common.EkMessageHelper
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
    Protected _ItemType As String
    Protected _ContentData As ContentData

#End Region

#Region "Events"

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        'register page components
        Me.RegisterCSS()
        Me.RegisterJS()

    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        _MessageHelper = _ContentApi.EkMsgRef
    End Sub

#End Region

    Public Function EditApprovalMethod() As Boolean
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

        _CurrentUserId = _ContentApi.UserId
        _AppImgPath = _ContentApi.AppImgPath
        _SitePath = _ContentApi.SitePath
        _EnableMultilingual = _ContentApi.EnableMultilingual

        If (Not (Page.IsPostBack)) Then
            Display_EditApprovals()
        Else
            Process_DoEditApprovalMethod()
        End If

    End Function

#Region "ACTION - EditApprovalMethod"
    Private Sub Process_DoEditApprovalMethod()
        Dim ekContentRef As Ektron.Cms.Content.EkContent
        Try
            ekContentRef = _ContentApi.EkContentRef
            _PageData = New Collection
            _PageData.Add(CLng(Request.Form(rblApprovalMethod.UniqueID)), "ApprovalMethod")
            If (_ItemType = "folder") Then
                _PageData.Add(_Id, "FolderID")
                ekContentRef.UpdateFolderApprovalMethod(_PageData)
            Else
                _PageData.Add(_Id, "ContentID")
                ekContentRef.UpdateContentApprovalMethod(_PageData)
            End If
            Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewApprovals&id=" & _Id & "&type=" & _ItemType, False)
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & _ContentLanguage, False)
        End Try
    End Sub
#End Region

#Region "APPROVAL - EditApprovalsMethod"
    Private Sub Display_EditApprovals()
        _PermissionData = _ContentApi.LoadPermissions(_Id, _ItemType)

        Dim m_intApprovalMethoad As Integer = 0
        If (_ItemType = "folder") Then
            _FolderData = _ContentApi.GetFolderById(_Id)
            m_intApprovalMethoad = _FolderData.ApprovalMethod
        Else
            _ContentData = _ContentApi.GetContentById(_Id)
            m_intApprovalMethoad = _ContentData.ApprovalMethod
        End If
        EditApprovalsToolBar()
        rblApprovalMethod.Items.Add(New ListItem(_MessageHelper.GetMessage("force all approvers with description"), "1"))
        rblApprovalMethod.Items.Add(New ListItem(_MessageHelper.GetMessage("do not force all approvers with description"), "0"))
        If (m_intApprovalMethoad = 1) Then
            rblApprovalMethod.Items(0).Selected = True
        Else
            rblApprovalMethod.Items(1).Selected = True
        End If
    End Sub
    Private Sub EditApprovalsToolBar()

        Dim result As New System.Text.StringBuilder
        Dim workareaTitlebarTitle As String = ""
        Dim isFolderUserAdmin As Boolean = False

        If (_ItemType = "folder") Then
            workareaTitlebarTitle = "Edit Approval Method For The Folder" & " """ & _FolderData.Name & """"
        Else
            workareaTitlebarTitle = "Edit Approval Method For The Content" & " """ & _ContentData.Title & """"
        End If

        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(workareaTitlebarTitle)

        result.Append(" <table><tbody><tr>")
        Dim arePermissionsInherited As Boolean = False
        If (_ItemType = "folder") Then
            arePermissionsInherited = _FolderData.Inherited
        Else
            arePermissionsInherited = _ContentData.IsInherited
        End If
        If (Not (_FolderData Is Nothing)) Then
            isFolderUserAdmin = _PermissionData.IsAdmin OrElse _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_FolderData.Id)
        Else
            If (Not (_ContentData Is Nothing)) Then
                isFolderUserAdmin = _PermissionData.IsAdmin OrElse _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_ContentData.FolderId)
            Else
                isFolderUserAdmin = _PermissionData.IsAdmin
            End If
        End If
        If ((_PermissionData.IsAdmin OrElse isFolderUserAdmin) And arePermissionsInherited = False) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt save approval method button text"), _MessageHelper.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('frmContent', '');"""))
        End If
        If (_ItemType = "folder") Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/back.png", "content.aspx?LangType=" & _ContentLanguage & "&action=ViewApprovals&type=folder&id=" & _Id, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        Else
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "../UI/Icons/back.png", "content.aspx?LangType=" & _ContentLanguage & "&action=View&id=" & _Id, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton(_PageAction))
        result.Append("</td>")
        result.Append("</tr></tbody></table>")
        htmToolBar.InnerHtml = result.ToString()

    End Sub

#End Region


#Region "JS, CSS"

    Private Sub RegisterJS()

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)

    End Sub

    Private Sub RegisterCSS()

    End Sub

#End Region

End Class
