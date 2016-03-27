Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Content
Imports System.IO

Partial Class Workarea_DeclineContent
    Inherits System.Web.UI.Page
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Common.EkMessageHelper
    Protected content_api As New ContentAPI
    Protected ContentLanguage As Integer = 0
    Protected AppImgPath As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not Page.IsPostBack) Then
            Dim contentid As Long = 0
            Dim folderid As Long = 0
            Dim AppUI As New CommonApi
            m_refMsg = content_api.EkMsgRef()
            If content_api.RequestInformationRef.IsMembershipUser Or content_api.RequestInformationRef.UserId = 0 Then
                Response.Redirect(content_api.ApplicationPath & "reterror.aspx?info=" & Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), False)
                Exit Sub
            End If
            RegisterResources()
            If (Not Request.QueryString("contentId") Is Nothing) AndAlso (Int64.TryParse(Request.QueryString("contentId"), contentid)) AndAlso (contentid > 0) Then
                hdnContentId.Value = contentid

                If (Not (Request.QueryString("LangType") Is Nothing)) AndAlso (Int32.TryParse(Request.QueryString("LangType"), ContentLanguage)) AndAlso (ContentLanguage > 0) Then
                    If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Or ContentLanguage = ALL_CONTENT_LANGUAGES Then
                        ContentLanguage = AppUI.DefaultContentLanguage
                    End If
                    AppUI.ContentLanguage = ContentLanguage
                    content_api.ContentLanguage = ContentLanguage
                Else
                    ContentLanguage = AppUI.DefaultContentLanguage
                End If
                hdnLangType.Value = ContentLanguage

                If (Not (Request.QueryString("folderId") Is Nothing)) AndAlso (Int64.TryParse(Request.QueryString("folderId"), folderid)) AndAlso (folderid > 0) Then
                    hdnFolderId.Value = folderid
                End If
                ViewToolBar()
            End If
        End If
    End Sub

    Protected Sub btnDecline_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDecline.Click
        Dim comment As String = ""
        Dim appUI As New UI.CommonUI.ApplicationAPI
        Dim msgHelper As Ektron.Cms.Common.EkMessageHelper = appUI.EkMsgRef

        RegExpValidator.ErrorMessage = msgHelper.GetMessage("content size exceeded")
        'RegExpValidator.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(65000)
        RegExpValidator.Validate()
        If RegExpValidator.IsValid Then
            If (DeclineText.Content.Trim.Length > 0) Then
                comment = "&comment=" & HttpUtility.UrlEncode(DeclineText.Content.Trim.Replace("<p>", "").Replace("</p>", ""))
            End If
            Response.Redirect(content_api.ApplicationPath & "content.aspx?id=" & hdnContentId.Value & "&fldid=" & hdnFolderId.Value & "&action=declinecontent&LangType=" & hdnLangType.Value & comment)
        Else
            ViewToolBar()
        End If
    End Sub

    Private Sub ViewToolBar()
        Dim result As New System.Text.StringBuilder
        AppImgPath = content_api.AppImgPath
        StyleSheetJS.Text = m_refStyle.GetClientScript
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl decline content"))
        result.Append("<table><tr>")
        result.Append("<td>" & m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", content_api.AppPath & "content.aspx?LangType=" & ContentLanguage & "&action=viewcontentbycategory&id=" & Request.QueryString("folderId"), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "") & "</td>")
        result.Append("<td>" & m_refStyle.GetHelpButton("Viewcontent") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Private Sub RegisterResources()
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
End Class
