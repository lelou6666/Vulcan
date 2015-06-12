Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class editvirtualstaging
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
        jsUniqueID.Text = "editvirtualstaging"
        m_refMsg = (New CommonApi).EkMsgRef
    End Sub
    Private Function DisplayEditScreen() As Boolean
        Dim m_refSiteApi As New SiteAPI
        Dim m_refUserApi As New UserAPI

        Try
            AppImgPath = m_refSiteApi.AppImgPath
            AppName = m_refSiteApi.AppName
            SITEPATH = m_refSiteApi.SitePath
            'jsContentLanguage.Text = Convert.ToString(settings_data.Language)

            td_asset_loc.InnerHtml = "<input type=""text"" size=""50"" maxlength=""255"" name=""asset_loc"" id=""asset_loc"" value=""" & "assets" & """>"
            td_private_asset_loc.InnerHtml = "<input type=""text"" id=""private_asset_loc"" name=""private_asset_loc"" size=""50"" maxlength=""255"" value=""" & "privateassets" & """>"
            td_DomainUserName.InnerHtml = "<input type=""text"" id=""DomainUserName"" name=""DomainUserName"" size=""50"" maxlength=""255"" value=""" & "user name" & """>"
            td_ConfirmPassword.InnerHtml = "<input type=""password"" id=""ConfirmPassword"" name=""ConfirmPassword"" size=""50"" maxlength=""255"" value=""" & "user1" & """>"
            td_Password.InnerHtml = "<input type=""password"" id=""Password"" name=""Password"" size=""50"" maxlength=""255"" value=""" & "user1" & """>"

            Return (False)
        Catch ex As Exception
        End Try
    End Function
    Public Function EditVirtualStagingControl() As Boolean
        Dim result As Boolean = False
        Me.ID = "editvirtualstaging"
        If (Not (IsPostBack)) Then

            result = DisplayEditScreen()
        Else
            result = ProcessSubmission()
        End If
        Return (result)
    End Function
    Public Function ProcessSubmission() As Boolean
        Dim m_refSiteApi As New SiteAPI

        'read form fields here and call update api
        Return (True)
    End Function

End Class
