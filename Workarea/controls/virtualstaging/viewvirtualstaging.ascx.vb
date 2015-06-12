Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class viewvirtualstaging
    Inherits System.Web.UI.UserControl

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
    Protected AppImgPath As String = ""
    Protected AppName As String = ""
    Protected SITEPATH As String = ""
    Private m_bReturn As Boolean = False
    Protected VerifyTrue As String = ""
    Protected VerifyFalse As String = ""
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim m_refSiteApi As New SiteAPI
        Dim m_refUserApi As New UserAPI
        m_refMsg = m_refSiteApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        AppName = m_refSiteApi.AppName
        SITEPATH = m_refSiteApi.SitePath

        'call api and display values

        td_asset_loc.InnerHtml = "assets"
        td_private_asset_loc.InnerHtml = "private"
        td_domain_username.InnerHtml = "user1"
    End Sub
End Class

