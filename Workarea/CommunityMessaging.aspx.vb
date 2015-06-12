Imports Ektron.Cms
Imports Ektron.Cms.API
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.CustomFieldsApi
Imports Ektron.Cms.Content

Partial Class CommunityMessaging
    Inherits System.Web.UI.Page

	Protected m_action As String = ""
	Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_userId As Long = 0
	Protected m_id As String = ""

	Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
		Dim ctl As System.Web.UI.UserControl = Nothing
		Dim refCommonAPI As New CommonApi()

		RegisterResources()

		If refCommonAPI.RequestInformationRef.IsMembershipUser Or refCommonAPI.RequestInformationRef.UserId = 0 Then
			Response.Redirect(refCommonAPI.SitePath & "login.aspx")
			Exit Sub

		Else
			If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(refCommonAPI.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking)) Then
				Utilities.ShowError(refCommonAPI.EkMsgRef.GetMessage("feature locked error"))
			End If
			m_userId = refCommonAPI.RequestInformationRef().UserId
			If (Not IsNothing(Request.QueryString("id"))) Then
				m_id = Request.QueryString("id").ToLower()
			End If
			m_refMsg = (New CommonApi).EkMsgRef
			msgJSContainer.Text = (New StyleHelper).GetClientScript

			If (Not IsNothing(Request.QueryString("action"))) Then
				m_action = Request.QueryString("action").ToLower()
			End If

			Select Case m_action
				Case "viewall"
					ctl = LoadControl("Controls/Community/Messaging/ViewMessages.ascx")
				Case "viewallsent"
					ctl = LoadControl("Controls/Community/Messaging/ViewMessages.ascx")
					CType(ctl, Messaging_ViewMessages).SentMode = True
				Case "viewmsg"
					ctl = LoadControl("Controls/Community/Messaging/ViewMessage.ascx")
				Case "viewsentmsg"
					ctl = LoadControl("Controls/Community/Messaging/ViewMessage.ascx")
					CType(ctl, controls_Community_Messaging_ViewMessage).SentMode = True
				Case "editmsg"
					ctl = LoadControl("Controls/Community/Messaging/EditMessage.ascx")
				Case "replymsg"

				Case Else
					ctl = LoadControl("Controls/Community/Messaging/ViewMessages.ascx")
			End Select
			If (Not IsNothing(ctl)) Then
				FindControl("MsgCtlHolder").Controls.Add(ctl)
			End If
		End If

	End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Expires = 0
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
    End Sub

	Public ReadOnly Property UserId() As Long
		Get
			Return (m_userId)
		End Get
	End Property

	Protected Sub RegisterResources()
		' register JS
		JS.RegisterJS(Me, JS.ManagedScript.EktronJS)

		' register CSS
		Css.RegisterCss(Me, Css.ManagedStyleSheet.EktronCommunityCss)
		Css.RegisterCss(Me, Css.ManagedStyleSheet.EktronCommunitySearchCss)
		Css.RegisterCss(Me, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
	End Sub
End Class
