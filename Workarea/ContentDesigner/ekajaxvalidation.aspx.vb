Partial Class ekvalidate
	Inherits Ektron.Cms.Workarea.Framework.WorkAreaBasePage

	Private m_bIsValid As Boolean
	Private m_strInvalidMessage As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		AssertInternalReferrer()

		Dim output As String = ""

		Try
			Dim sXml As String
			sXml = Request("xml")

			Dim objSchemas As New System.Xml.Schema.XmlSchemaSet()

			Dim iXsd As Integer
			Dim sXsd As String
			Dim sNsUri As String
			iXsd = 0
			sXsd = Request("xsd0")
			sNsUri = Request("nsuri0")
			If (IsNothing(sXsd) OrElse 0 = sXsd.Length) Then
				sXsd = Request("xsd")
				sNsUri = Request("nsuri")
			End If
			While (Not IsNothing(sXsd) AndAlso sXsd.Length > 0)
				If (Not sXsd.Contains("<")) Then
					Dim sXsdUri As String
					Try
						sXsdUri = New Uri(Request.Url, sXsd).AbsoluteUri
						objSchemas.Add(sNsUri, sXsdUri)
					Catch
						Try
							sXsdUri = Server.MapPath(sXsd)
						Catch ex As Web.HttpException
							' Ignore; URL is likely not within this web application
							sXsdUri = sXsd
						End Try
						objSchemas.Add(sNsUri, sXsdUri)
					End Try
				Else
					objSchemas.Add(sNsUri, New System.Xml.XmlTextReader(New IO.StringReader(sXsd)))
				End If
				iXsd += 1
				sXsd = Request("xsd" & iXsd)
				sNsUri = Request("nsuri" & iXsd)
			End While

			Dim objXmlDoc As System.Xml.XPath.XPathDocument
			If (String.IsNullOrEmpty(sXml)) Then
				objXmlDoc = Nothing
			ElseIf (Not sXml.Contains("<")) Then
				Dim sXmlUri As String
				Try
					sXmlUri = New Uri(Request.Url, sXml).AbsoluteUri
					objXmlDoc = New System.Xml.XPath.XPathDocument(sXmlUri)
				Catch
					Try
						sXmlUri = Server.MapPath(sXml)
					Catch ex As Web.HttpException
						' URL is likely not within this web application or is dynamic
						sXmlUri = sXml
					End Try
					objXmlDoc = New System.Xml.XPath.XPathDocument(sXmlUri)
				End Try
			Else
				objXmlDoc = New System.Xml.XPath.XPathDocument(New System.Xml.XmlTextReader(New IO.StringReader(sXml)))
			End If

			If (Not IsNothing(objXmlDoc)) Then
				' No XML, we are validating schema(s)
				output = ValidateXml(objXmlDoc, objSchemas)
			End If

		Catch ex As Exception
			output = ex.Message
		End Try

		Response.ContentType = "text/html"
		Response.ContentEncoding = System.Text.Encoding.UTF8 ' Safari does not encode properly even though this is set

		litContent.Text = output
	End Sub

	Private Function ValidateXml(ByVal objXmlDocument As System.Xml.XPath.XPathDocument, ByVal objSchemas As System.Xml.Schema.XmlSchemaSet) As String
		Dim nav As System.Xml.XPath.XPathNavigator = objXmlDocument.CreateNavigator()

		m_bIsValid = True
		m_strInvalidMessage = ""

		nav.CheckValidity(objSchemas, AddressOf ValidationHandler)

		Return m_strInvalidMessage
	End Function

	Protected Sub ValidationHandler(ByVal sender As Object, ByVal validationArgs As System.Xml.Schema.ValidationEventArgs)
		m_bIsValid = False
		' process XML/XSD validation warnings & errors errors here
		Select Case validationArgs.Severity
			Case System.Xml.Schema.XmlSeverityType.Error
				m_strInvalidMessage = m_strInvalidMessage & validationArgs.Message & vbCrLf & vbCrLf & vbCrLf
			Case System.Xml.Schema.XmlSeverityType.Warning
				m_strInvalidMessage = m_strInvalidMessage & validationArgs.Message & vbCrLf & vbCrLf & vbCrLf
		End Select
	End Sub

End Class
