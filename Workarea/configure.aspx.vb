Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class configure
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Common.EkMessageHelper
    Protected m_strPageAction As String = ""
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected AppName As String = ""
	Protected SITEPATH As String = ""
	Protected m_blnRefreshFrame As Boolean
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1
        Dim m_refSiteApi As New SiteAPI
        m_refMsg = m_refSiteApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        AppPath = m_refSiteApi.AppPath
        AppName = m_refSiteApi.AppName
        SITEPATH = m_refSiteApi.SitePath
        litTitle.Text = AppName & " " & m_refMsg.GetMessage("config page html title")
        If ((m_refSiteApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn") = False) Then
            Response.Redirect("login.aspx?fromLnkPg=1", True)
            Exit Sub
        End If
        If m_refSiteApi.RequestInformationRef.IsMembershipUser Or m_refSiteApi.RequestInformationRef.UserId = 0 Then
            Response.Redirect("reterror.aspx?info=Please login as cms user", True)
            Exit Sub
        End If
        RegisterResources()
        If (Not (IsNothing(Request.QueryString("action")))) Then
            m_strPageAction = Request.QueryString("action")
            If (m_strPageAction.Length > 0) Then
                m_strPageAction = m_strPageAction.ToLower
            End If
        End If

		If (Not (IsNothing(Request.QueryString("RefreshFrame")))) Then
			If Request.QueryString("RefreshFrame").ToLower = "true" Then
				m_blnRefreshFrame = True
			End If
		End If

		StyleSheetJS.Text = m_refStyle.GetClientScript
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("config page title"))
        divToolBar.InnerHtml = ConfigToolBar()
	End Sub
    Private Function ConfigToolBar() As String
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>")
        If m_strPageAction = "edit" Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update settings button text"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('config', 'VerifyForm()');"""))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "configure.aspx", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "configure.aspx?action=edit", m_refMsg.GetMessage("alt edit settings button text"), m_refMsg.GetMessage("btn edit"), ""))
        End If
        result.Append("<td>" & m_refStyle.GetHelpButton("ApplicationSetup" & m_strPageAction) & "</td>")
        result.Append("</tr></table>")
        Return (result.ToString)
    End Function
    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
        Dim bCompleted As Boolean
        Try
            Select Case m_strPageAction
                Case "edit"
                    ViewSet.SetActiveView(Edit)
                    bCompleted = editconfiguration1.EditConfigurationControl()
					If (bCompleted = True) Then
                        If (Not (IsPostBack)) Then
                            Response.Redirect("configure.aspx", False)
                        Else
                            Response.Redirect("configure.aspx?RefreshFrame=true", False)
                        End If
					End If
                Case Else
                    ViewSet.SetActiveView(View)
            End Select
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronEmpJSFuncJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS)
    End Sub
End Class
