Imports System.Data
Imports Ektron.Cms
Imports System.Web.UI
Imports Ektron.Cms.Common.EkConstants

Partial Class ExplorerDownload
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
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected AppPath As String = ""
    Protected m_refSiteApi As New SiteAPI

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim result As New System.Text.StringBuilder
        StyleSheetJS.Text = m_refStyle.GetClientScript
        m_refMsg = m_refContentApi.EkMsgRef
        divTitleBar.InnerHtml = m_refMsg.GetMessage("lbl Ektron Explorer Download")
        RegisterResources()
		result.Append(m_refStyle.GetHelpButton("ExplorerDownloadManuals"))
        divToolBar.InnerHtml = result.ToString
        Dim html As New System.Text.StringBuilder
        html.Append("<table width='100%'><tr><td>&nbsp;</td></tr><tr><td align='center'>" & m_refMsg.GetMessage("alt Click download to begin download of the ektron explorer client install") & "</td></tr>")
        html.Append("<tr><td>&nbsp;</td></tr><tr><td align='center'><input type='button' value=" & m_refMsg.GetMessage("btn download") & " id='install_button' onclick='downloadExplorer();'></td></tr></table>")
        download_cell.InnerHtml = html.ToString()
        AppPath = m_refSiteApi.AppPath
        'Put user code to initialize the page here
    End Sub
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronEmpJSFuncJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncsJS")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "java/validation.js", "EktronValidationJS")
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
End Class
