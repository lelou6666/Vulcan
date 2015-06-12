Partial Class blankredirect
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
        'Put user code to initialize the page here
        Dim queryInfo() As String
        queryInfo = Request.Url.PathAndQuery.Split("?")
        If (queryInfo.Length > 1) Then
            Dim url As System.Uri = New System.Uri(Page.ResolveUrl(Request.Url.PathAndQuery.Substring(Request.Url.PathAndQuery.IndexOf("blankredirect.aspx?") + "blankredirect.aspx?".Length)), System.UriKind.RelativeOrAbsolute)
            If (url.IsAbsoluteUri AndAlso url.Host <> Request.Url.Host) Then
                lblMessage.Text = "Not a valid redirect URL."
            Else
                Response.Redirect(url.OriginalString)
            End If
        Else
            lblMessage.Text = "Not a valid redirect URL."
        End If
    End Sub

End Class
