Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Modules.EkModule

Partial Class DateTimeSelector
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

#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim AppUI As New SiteAPI
        Dim startdate, enddate As Date
        Dim ekm As Ektron.Cms.Modules.EkModule
        Dim display As String
        'Dim fN As String
        'Dim eN As String
        Dim targetDateString As String
        Dim ekDts As New EkDTSelector(AppUI.RequestInformationRef)
        Dim EkMsg As New EkMessageHelper(AppUI.RequestInformationRef)

        ' register CSS
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.AllIE)

        ekm = AppUI.EkModuleRef

        If (Request.QueryString("LangType") <> "") Then
            AppUI.ContentLanguage = Request.QueryString("LangType")
            AppUI.SetCookieValue("LastValidLanguageID", Convert.ToInt32(Request.QueryString("LangType")))
        Else
            If CStr(AppUI.GetCookieValue("LastValidLanguageID")) <> "" Then
                AppUI.ContentLanguage = AppUI.GetCookieValue("LastValidLanguageID")
            End If
        End If

        Select Case LCase(Request.QueryString("type"))
            Case "date"
                display = "dtselectordate"
                targetDateString = Now.ToString("d")
            Case "time"
                display = "dtselectortime"
                targetDateString = ekDts.RoundMinutes(Now, 5).ToString("t")
            Case Else
                display = "dtselectordatetime"
                targetDateString = ekDts.RoundMinutes(Now, 5).ToString("g")
        End Select

        If (Request.QueryString("targetdate") <> "" AndAlso IsDateTime(Request.QueryString("targetdate"))) Then
            JSGlobals.Text = "targetdate = '" & Server.HtmlEncode(Request.QueryString("targetdate")) & "' ;"
        Else
            JSGlobals.Text = "targetdate = '" & targetDateString & "' ;"
        End If

        If Not IsBlankOrAlphaNumeric(Server.HtmlEncode(Request.QueryString("spanid"))) Then Utilities.ShowError(EkMsg.GetMessage("lbl invalid parameters"))
        JSGlobals.Text &= "spanid = '" & Server.HtmlEncode(Request.QueryString("spanid")) & "' ;"
        If Not IsBlankOrAlphaNumeric(Server.HtmlEncode(Request.QueryString("formname"))) Then Utilities.ShowError(EkMsg.GetMessage("lbl invalid parameters"))
        JSGlobals.Text &= "formname = '" & Server.HtmlEncode(Request.QueryString("formname")) & "' ;"
        If Not IsBlankOrAlphaNumeric(Server.HtmlEncode(Request.QueryString("formelement"))) Then Utilities.ShowError(EkMsg.GetMessage("lbl invalid parameters"))
        JSGlobals.Text &= "formelement = '" & Server.HtmlEncode(Request.QueryString("formelement")) & "' ;"

        ekm = AppUI.EkModuleRef
        If (Request.QueryString("sdate") <> "") Then
            startdate = CDate(Request.QueryString("sdate"))
            startdate = DateSerial(DatePart(DateInterval.Year, startdate), DatePart(DateInterval.Month, startdate), 1)
        Else
            startdate = DateSerial(DatePart(DateInterval.Year, Now()), DatePart(DateInterval.Month, Now()), 1)
        End If

        enddate = DateAdd(DateInterval.Month, 1, startdate)

        moDisplay.Text = ekm.OutputRenderedCalendarHTML(0, display, startdate, enddate, 0, 0)

        ' QueryStrings coming in:
        ' targetdate
        ' spanid
        ' formname
        ' formelement

    End Sub

    Public Function IsBlankOrAlphaNumeric(ByVal text As String) As Boolean

        If text Is Nothing Then

            Return True

        ElseIf text IsNot Nothing Then

            Return IsAlphaNumeric(text.Replace("_", ""))

        Else

            Return False

        End If

    End Function

    Public Function IsAlphaNumeric(ByVal text As String) As Boolean

        Return Regex.Match(text.Trim(), "^[a-zA-Z0-9]*$").Success

    End Function

    Public Function IsDateTime(ByVal text As String) As Boolean

        If text IsNot Nothing Then

            Dim result As Date
            Return DateTime.TryParse(text, result)

        Else

            Return False

        End If

    End Function

End Class
