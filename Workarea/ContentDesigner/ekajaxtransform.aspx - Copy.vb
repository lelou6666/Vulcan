Partial Class ektransform
	Inherits Ektron.Cms.Workarea.Framework.WorkAreaBasePage

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		AssertInternalReferrer()
		Dim output As String = "<root></root>"

		Try
			Dim sXml As String
			Dim sXslt As String
			sXml = Request("xml")
			sXslt = Request("xslt")

			Dim args As Ektron.Cms.Xslt.ArgumentList = Nothing
			Dim iArg As Integer
			Dim sArg As String
			Dim pEqu As Integer
			Dim sName As String
			Dim sValue As String
			iArg = 0
			sArg = Request("arg0")	' format: "arg0=" + escape(name + "=" + value)
			While (Not IsNothing(sArg) AndAlso sArg.Length > 0)
				pEqu = sArg.IndexOf("=") ' separator b/n name and value
				If (pEqu >= 1) Then
					sName = sArg.Substring(0, pEqu)
					sValue = sArg.Substring(pEqu + 1)
					If (IsNothing(args)) Then
						args = New Ektron.Cms.Xslt.ArgumentList()
					End If
					args.AddParam(sName, "", sValue)
				End If
				iArg += 1
				sArg = Request("arg" & iArg)
			End While

			If (Not String.IsNullOrEmpty(sXml) AndAlso Ektron.Cms.Common.EkFunctions.IsURL(sXml)) Then
				Try
					sXml = New Uri(Request.Url, sXml).AbsoluteUri
				Catch ex As Exception
					' Ignore
				End Try
			End If

			' Can't MapPath b/c the transform will lose its context and relative paths will fail.
			If (Not String.IsNullOrEmpty(sXslt) AndAlso Not sXslt.Contains("<")) Then
				Try
					sXslt = New Uri(Request.Url, sXslt).AbsoluteUri
				Catch ex As Exception
					' Ignore
				End Try
			End If

			'Dim sApi As String
			'sApi = Request("api")
			'If (Not IsNothing(sApi) AndAlso "cms" = sApi.ToLower()) Then
			'	' Use the API's transform so the built-in arguments are given to the XSLT.
			'	Dim api As New Ektron.Cms.CommonApi
			'	output = api.XSLTransform(sXml, sXslt, XsltArgs:=args, ReturnExceptionMessage:=False)
			'	api = Nothing
			'Else
			output = Ektron.Cms.EkXml.XSLTransform(sXml, sXslt, XsltArgs:=args, ReturnExceptionMessage:=False)
			'End If
		Catch ex As Exception
			output = String.Format("<html><head><title>ekAjaxTransform Error</title></head><body class=""ekAjaxTransformError"">{0}</body></html>", Server.HtmlEncode(ex.Message))
		End Try

		Response.ContentType = "application/xml"
		Response.ContentEncoding = System.Text.Encoding.UTF8 ' Safari does not encode properly even though this is set
		litContent.Text = output
	End Sub
End Class
