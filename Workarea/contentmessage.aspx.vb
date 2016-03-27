Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class Workarea_controls_content_contentmessage
    Inherits System.Web.UI.Page
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected m_strPageAction As String = ""
    Protected m_intId As Long = 0
    Protected m_strStyleSheetJS As String = ""

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Me.MessageBoard.MaxResults = m_refContentApi.RequestInformationRef.PagingSize
        ltStyle.Text = (New StyleHelper).GetClientScript
        MessageBoard1.MaxResults = m_refContentApi.RequestInformationRef.PagingSize
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = m_refContentApi.EkMsgRef
        If m_refContentApi.RequestInformationRef.IsMembershipUser OrElse m_refContentApi.RequestInformationRef.UserId = 0 Then
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), False)
            Exit Sub
        End If
        AppImgPath = m_refContentApi.AppImgPath
        RegisterResources()
        'If (Not (Request.QueryString("action") Is Nothing)) Then
        '    m_strPageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        'End If
        If (Not (Request.QueryString("id") Is Nothing)) Then
            m_intId = Convert.ToInt64(Request.QueryString("id"))
        End If

        ToolBar()
    End Sub

    Private Sub ToolBar()
        Dim result As System.Text.StringBuilder
        result = New System.Text.StringBuilder
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view messages for content"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "content.aspx?&action=View&id=" & m_intId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub RegisterResources()
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub

End Class
