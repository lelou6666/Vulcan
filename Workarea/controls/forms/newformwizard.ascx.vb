Partial Class newformwizard
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

#End Region

#Region "Private Members"
    Protected m_refmsg As Ektron.Cms.Common.EkMessageHelper
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Dim objStyle As New StyleHelper
        m_refmsg = (New Ektron.Cms.CommonApi()).EkMsgRef

		HelpButton1.Text = objStyle.GetHelpButton("FormWizardStep1")
		HelpButton2.Text = objStyle.GetHelpButton("FormWizardStep2")
		HelpButton3.Text = objStyle.GetHelpButton("FormWizardStep3")
		HelpButton4.Text = objStyle.GetHelpButton("FormWizardStep4")
		HelpButton5.Text = objStyle.GetHelpButton("FormWizardStep5")
		objStyle = Nothing
	End Sub

End Class
