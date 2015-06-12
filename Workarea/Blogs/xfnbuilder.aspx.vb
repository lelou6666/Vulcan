Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants

Partial Class blogs_xfnbuilder
    Inherits System.Web.UI.Page

    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected m_refContentApi As New ContentAPI
    Protected m_strStyleSheetJS As String = ""

    Private Sub XFNToolBar()
        Dim result As New System.Text.StringBuilder
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar("Edit Relationship")
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", "Accept", "Accept", "onclick=""WriteBack();"" "))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", "Cancel", "Cancel", "onclick=""self.close();return false;"" "))
        result.Append("<td>")
        'result.Append(m_refStyle.GetHelpButton("XFN Builder"))
        result.Append("</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        RegisterResources()

        If Not Request.QueryString("id") Is Nothing AndAlso Request.QueryString("id") <> "" Then
            m_id.Text = HttpUtility.HtmlEncode(Request.QueryString("id"))
        End If
        If Not Request.QueryString("field") Is Nothing AndAlso Request.QueryString("field") <> "" Then
            m_field.Text = HttpUtility.HtmlEncode(Request.QueryString("field"))
        End If
        m_refMsg = m_refContentApi.EkMsgRef
        AppImgPath = m_refContentApi.AppImgPath
        AppPath = m_refContentApi.AppPath
        If (m_refContentApi.UserId > 0) Then
            XFNToolBar()
        End If
    End Sub

    Protected Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS)
    End Sub
End Class
