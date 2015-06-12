Imports Ektron.Cms
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Common.EkConstants
Imports Microsoft.Security.Application

Partial Class approval
    Inherits System.Web.UI.Page

#Region "Members"

    Private _CommonApi As New CommonApi
    Private _EkContent As EkContent
    Protected _SiteApi As New SiteAPI
    Protected _UserApi As New UserAPI
    Protected _PageAction As String = ""
    Private _ViewApprovalList As ViewApprovalList
    Private _ViewApprovalContent As ViewApprovalContent
    Protected _ContentLanguage As Integer = -1
    Protected _EnableMultilingual As Integer = 0

#End Region

#Region "Events"

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1
        RegisterResources()
    End Sub
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        litStyleSheetJS.Text = (New StyleHelper).GetClientScript
        If (Not (IsNothing(Request.QueryString("action")))) Then
            If (Request.QueryString("action") <> "") Then
                _PageAction = AntiXss.HtmlEncode(Request.QueryString("action").ToLower())
            End If
        End If
        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            If (Request.QueryString("LangType") <> "") Then
                _ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                _CommonApi.SetCookieValue("LastValidLanguageID", _ContentLanguage)
            Else
                If _CommonApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    _ContentLanguage = Convert.ToInt32(_CommonApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If _CommonApi.GetCookieValue("LastValidLanguageID") <> "" Then
                _ContentLanguage = Convert.ToInt32(_CommonApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If

        If _ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            _CommonApi.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            _CommonApi.ContentLanguage = _ContentLanguage
        End If

        _EnableMultilingual = _CommonApi.EnableMultilingual
        EmailArea.Text = (New EmailHelper).MakeEmailArea
    End Sub
    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
        Try
            Select Case _PageAction.ToLower()
                Case "viewapprovallist"
                    _ViewApprovalList = CType(LoadControl("controls/approval/viewapprovallist.ascx"), ViewApprovalList)
                    _ViewApprovalList.MultilingualEnabled = _EnableMultilingual
                    _ViewApprovalList.ID = "viewApprovalList"
                    _ViewApprovalList.ContentLang = _CommonApi.ContentLanguage
                    Me.DataHolder.Controls.Add(_ViewApprovalList)
                Case "approvecontentaction"
                    ApproveContent()
                Case "editContentAction"
                    EditContent()
                Case "declinecontentaction"
                    DeclineContent()
                Case "viewcontent"
                    _ViewApprovalContent = CType(LoadControl("controls/approval/viewapprovalcontent.ascx"), ViewApprovalContent)
                    Me.DataHolder.Controls.Add(_ViewApprovalContent)
            End Select
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

#End Region

#Region "Helpers"

    Private Sub EditContent()
        Dim lId As Long
        Dim ret As Boolean
        Dim strPage As String = ""
        lId = Request.QueryString("id")
        Try
            strPage = Request.QueryString("page")
            _EkContent = _CommonApi.EkContentRef()
            ret = _EkContent.TakeOwnership(lId)
            Response.Redirect("edit.aspx?LangType=" & _CommonApi.ContentLanguage & "&id=" & lId & "&type=update&back_page=" & strPage, False)
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub DeclineContent()
        Dim lId As Long
        Dim ret As Boolean
        Try
            lId = Request.QueryString("id")
            _EkContent = _CommonApi.EkContentRef()
            Dim reason As String = ""
            If (Not Request.QueryString("comment") Is Nothing) Then
                reason = Request.QueryString("comment")
            End If
            ret = _EkContent.DeclineApproval2_0(lId, reason)

            If (Request.QueryString("page") = "workarea") Then
                '' re-direct to the folder page.
                Response.Redirect("approval.aspx?action=viewApprovalList&fldid=" & Request.QueryString("fldid"), False)
                ' redirect to workarea
                'Response.Write("<script language=""Javascript"">" & _
                '                       "top.switchDesktopTab();" & _
                '                       "</script>")
            ElseIf (Request.QueryString("page") = "dmsmenu") Then
                ' re-direct to the folder page.
                'Response.Redirect("approval.aspx?action=viewApprovalList&fldid=" & Request.QueryString("fldid"), False)
            Else
                Response.Write("<script language=""Javascript"">" & _
                                       "top.opener.location.reload(true);" & _
                                       "top.close();" & _
                                       "</script>")
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try

    End Sub
    Private Sub ApproveContent()
        Dim lId As Long
        Dim ret As Boolean
        Try
            _EkContent = _CommonApi.EkContentRef()
            lId = CLng(Request.QueryString("id"))
            ret = _EkContent.Approvev2_0(lId)

            If (Request.QueryString("page") = "workarea") Then
                '' redertrect to the folder page.
                'Response.Redirect("approval.aspx?action=viewApprovalList&fldid=" & Request.QueryString("fldid"), False)
                ' redirect to workarea
                Response.Write("<script type=""text/javascript"">" & _
                    "var rightFrame = top.document.getElementById('ek_main');" & _
                    "var rightFrameUrl = 'approval.aspx?action=viewApprovalList';" & _
                    "var appPath  = '" + _CommonApi.AppPath + "';" & _
                    "rightFrame.src = appPath + rightFrameUrl;" & _
                    "</script>")
            Else
                Response.Write("<script language=""Javascript"">" & _
                             "top.opener.location.reload(true);" & _
                             "top.close();" & _
                             "</script>")

            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

#End Region

#Region "Register JS/CSS"

    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
    End Sub

#End Region
End Class
