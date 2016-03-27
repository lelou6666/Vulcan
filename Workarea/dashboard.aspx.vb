Imports System.Data
Imports System.Collections.Generic
Imports System.IO
Imports System.Web.UI.WebControls

Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Personalization
Imports Ektron.Cms.Widget

Partial Class dashboard
    Inherits System.Web.UI.Page

    Private _CommonApi As New CommonApi
    Private _MessageHelper As Ektron.Cms.Common.EkMessageHelper
    Private _StyleHelper As New StyleHelper
    Private _ContentLanguage As Integer = 0
    Private _ContentApi As New ContentAPI

    Private Sub Page_PreInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.PreInit
        Try
            SetWidgetSpaceID()
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        RegisterResources()
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If ((_CommonApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn") = False) Then
            Response.Redirect("login.aspx?fromLnkPg=1", True)
            Exit Sub
        End If
        If _CommonApi.RequestInformationRef.IsMembershipUser Or _CommonApi.RequestInformationRef.UserId = 0 Then
            Dim literalError As Literal = New Literal()
            literalError.Text = "<p>Please login as a cms user.<br/><a href=""login.aspx"">Click here to login</a></p>"
            form1.Controls.Add(literalError)
            mainDiv.Visible = False
            Exit Sub
        End If
        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            _ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
            _CommonApi.SetCookieValue("LastValidLanguageID", _ContentLanguage)
        Else
            _ContentLanguage = _CommonApi.GetCookieValue("LastValidLanguageID")
        End If

        If _ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            _CommonApi.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            _CommonApi.ContentLanguage = _ContentLanguage
        End If
        _CommonApi.ContentLanguage = ALL_CONTENT_LANGUAGES

        Try
            Response.CacheControl = "no-cache"
            Response.AddHeader("Pragma", "no-cache")
            Response.Expires = -1

            _MessageHelper = _CommonApi.EkMsgRef

            SetTitlebarText()
            SetHelpButtonText()
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

    Private Sub SetHelpButtonText()
        If (_CommonApi.Debug_ShowHelpAlias()) Then
            Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS)
        End If
        HelpButton.Text = _StyleHelper.GetHelpButton("dashboard_aspx")
    End Sub

    Private Sub SetTitlebarText()
        Dim strUserName As String = ""
        If (_CommonApi.UserId > 0) Then
            strUserName = Ektron.Cms.CommonApi.GetEcmCookie()("username")
            ' trim to max length and add elipsis if needed:
            If (Len(strUserName) > 20) Then
                strUserName = Left(strUserName, 20) & "..."
            End If
        End If
        divTitle.InnerHtml = "<span id=""ektronTitlebar"">" & _MessageHelper.GetMessage("lbl smart desktop for admin") & "</span> " & strUserName
    End Sub

    Private Sub SetWidgetSpaceID()
        'See if a widget scope record exists with a scope value of 2 (WorkareaDashboard)
        Dim workareaDashboardSpaceID As Long = GetWorkareaDashboardScopeID()
        If (workareaDashboardSpaceID = -1) Then
            'It does not exist, so programmatically add it and sync the 'Widgets' directory
            workareaDashboardSpaceID = RegisterWidgetSpaceID()
            SyncWidgets()
        End If

        Personalization1.WidgetSpaceID = workareaDashboardSpaceID
    End Sub

    Private Function GetWorkareaDashboardScopeID() As Long
        For Each item As WidgetSpaceData In WidgetSpaceFactory.GetModel().FindAll()
            If (item.Scope = WidgetSpaceScope.WorkareaDashboard) Then
                Return item.ID
            End If
        Next
        Return -1
    End Function

    Private Function RegisterWidgetSpaceID() As Long
        Dim widgetSpaceData As WidgetSpaceData = Nothing
        Dim title As String = "WorkareaDashboard"
        WidgetSpaceFactory.GetModel().Create(title, WidgetSpaceScope.WorkareaDashboard, WidgetSpaceData)
        Return WidgetSpaceData.ID
    End Function

    Private Sub SyncWidgets()        
        If (Directory.Exists(Server.MapPath(_ContentApi.RequestInformationRef.WidgetsPath))) Then
            WidgetTypeController.SyncWidgetsDirectory()
        End If
    End Sub

    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUIResizableJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS)

        Ektron.Cms.API.Css.RegisterCss(Me, _ContentApi.ApplicationPath & "/Personalization/css/ektron.personalization.css", "EktronPersonalziationCss")
        Ektron.Cms.API.Css.RegisterCss(Me, _ContentApi.ApplicationPath & "/Personalization/css/ektron.personalization.ie.7.css", "EktronPersonalizationIe7Css", Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, _ContentApi.ApplicationPath & "/csslib/ektron.workarea.personalization.css", "EktronWorkareaPersonalziationCss")
        Ektron.Cms.API.Css.RegisterCss(Me, _ContentApi.ApplicationPath & "/csslib/ektron.workarea.personalization.ie.7.css", "EktronWorkareaPersonalizationIe7Css", Ektron.Cms.API.Css.BrowserTarget.IE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub
End Class
