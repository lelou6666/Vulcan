Partial Class cmsdatalist
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
		CmsDataList.Visible = False
		CmsDataListXml.Text = Ektron.Cms.Common.EkFunctions.XSLTransform(CmsDataList.EkItem.Html, CmsDataList.EkItem.Xslt1, XsltAsFile:=True)
	End Sub

End Class
