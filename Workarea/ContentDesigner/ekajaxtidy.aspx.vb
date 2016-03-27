Partial Class ektidy
	Inherits Ektron.Cms.Workarea.Framework.WorkAreaBasePage

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		AssertInternalReferrer()
		Dim output As String = ""

		Try
			Dim sHtml As String
			sHtml = Request("html")
			If (IsNothing(sHtml)) Then
				Throw New ArgumentException("Argument 'html' is required.")
			End If

			Dim objEkContent As Ektron.Cms.Content.EkContent = New Ektron.Cms.Content.EkContent(Me.GetCommonApi().RequestInformationRef)

			output = objEkContent.ConvertHtmlContenttoXHTML(sHtml)

		Catch ex As Exception
			output = String.Format("<html><head><title>ekAjaxTidy Error</title></head><body class=""ekAjaxTidyError"">{0}</body></html>", Server.HtmlEncode(ex.Message))
		End Try

		Response.ContentType = "application/xml"
		Response.ContentEncoding = System.Text.Encoding.UTF8 ' Safari does not encode properly even though this is set
		litContent.Text = output
	End Sub
End Class
