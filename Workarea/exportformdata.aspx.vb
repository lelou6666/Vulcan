Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants

Partial Class exportformdata
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
    Protected FormId As Long
    Protected CurrentUserId As Long
    Protected StartDate As String
    Protected EndDate As String
    Protected gtForm As Collection
    Protected gtForms As Collection
    Protected m_refMsg As Common.EkMessageHelper
    Protected m_refStyle As New StyleHelper
    Protected m_refContentApi As New ContentAPI
    Protected DefaultFormTitle As String = ""
	Protected DisplayType As String = ""
	Protected sFormDataIds As String = ""
    Protected DataType As String = ""
    Protected AppImgPath As String = ""
    Protected ContentLanguage As Integer = -1
    Protected Flag As String = "false"
    Protected Security_info As PermissionData
    Protected Action As String = ""
    Protected ResultType As String = ""
    Protected EnableMultilingual As Integer = 0
    Protected objForm As Ektron.Cms.Modules.EkModule
	Protected FormTitle As String = ""
	Protected QueryLang As String = ""
	Protected FieldName As String = ""
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

		'Make sure the user is logged in. If not forward user to login page.
		If ((m_refContentApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn") = False) Then
			Dim strUrl As String
			Session("RedirectLnk") = Request.Url.AbsoluteUri
			strUrl = "login.aspx?fromLnkPg=1"
			Me.Response.ContentType = ""
			Me.Response.Redirect(strUrl, endResponse:=True)
		End If

		'Put user code to initialize the page here
		Dim ContentLanguage As Integer
		If (Not (Request.QueryString("LangType") Is Nothing)) Then
			If (Request.QueryString("LangType") <> "") Then
				ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
				m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
			Else
				If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
					ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
				End If
			End If
		Else
			If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
				ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
			End If
		End If
		If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
			m_refContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
		Else
			m_refContentApi.ContentLanguage = ContentLanguage
		End If
		If Not (Request.QueryString("fieldname") Is Nothing) Then
			FieldName = Request.QueryString("fieldname")
		End If
		QueryLang = CStr(ContentLanguage)
		If Not (Request.QueryString("qlang") Is Nothing) Then
			QueryLang = Request.QueryString("qlang")
		End If

		FormId = Request("form_id")
		StartDate = Request("start_date")
		EndDate = Request("end_date")
		Flag = Request("flag")
		DataType = Request("data_type")
		ResultType = Request("result_type")
		CurrentUserId = m_refContentApi.UserId
		DisplayType = Request("display_type")
		FormTitle = Request("form_title")
		m_refMsg = m_refContentApi.EkMsgRef
		Security_info = m_refContentApi.LoadPermissions(FormId, "content")
		Response.AddHeader("content-disposition", "attachment; filename=Form_Data_Export.xls")
		objForm = m_refContentApi.EkModuleRef
		gtForms = objForm.GetAllFormInfo()

		Dim objFormData As New Collection
		Dim cDatas As Collection
		objFormData.Add(FormId, "FORM_ID")
		objFormData.Add(CurrentUserId, "USER_ID")
		objFormData.Add(StartDate, "START_DATE")
		objFormData.Add(EndDate, "END_DATE")
		objFormData.Add(QueryLang, "Query_Language")
		objFormData.Add(FieldName, "Field_Name")
		cDatas = objForm.GetAllFormData(objFormData)
		If (cDatas.Count = 0) Then
            FormResult.Text = "<table><tr><td>" & m_refMsg.GetMessage("msg no data report") & "</td></tr></table>"
			Exit Sub
		End If

		If IsNumeric(DisplayType) Then
			'The following lines of code are extracted from dotnetjohn.com on "Export DataSets to Excel"
			'http://www.dotnetjohn.com/articles.aspx?articleid=36
			'first let's clean up the response.object
			Response.Clear()
			Response.Charset = ""
			'set the response mime type for excel
			Response.ContentType = "application/vnd.ms-excel"
			'create a string writer
			Dim stringWrite As New System.IO.StringWriter
			'create an htmltextwriter which uses the stringwriter
			Dim htmlWrite As New System.Web.UI.HtmlTextWriter(stringWrite)
			'instantiate a datagrid
			Dim dg As New DataGrid
			'set the datagrid datasource to the dataset passed in
			dg.DataSource = objForm.GetAllFormRawData(objFormData).Tables(0)
			'bind the datagrid
			dg.DataBind()
			'tell the datagrid to render itself to our htmltextwriter
			dg.RenderControl(htmlWrite)
			'all that's left is to output the html
			Response.Write(stringWrite.ToString)
			Response.End()
		Else
			FormResult.Text = LoadResult(objFormData, cDatas)
		End If

	End Sub
	Private Function LoadResult(ByVal objFormData As Collection, ByVal cDatas As Collection) As String
		Dim result As New System.Text.StringBuilder
		Dim cData As Collection

        Dim iCnt, tmpFormId, dataID As Object
        dataID = Nothing
        Dim strHtml, bPaste As Object
		strHtml = ""
		tmpFormId = 0

		If DisplayType = "horizontal" Then
            Dim fd1, fds1, fd2, fds2 As Object
			If CStr(FormId) = "" Then
				For Each gtForm In gtForms
					FormId = gtForm("formid")
					fds1 = objForm.GetFormFieldsById(FormId)
					fds2 = objForm.TransferFormVariable(objFormData)
					If fds2.count > 0 Then
						bPaste = False
						result.Append("<table><tr><td>&nbsp;</td></tr></table>")
						result.Append("<table border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " & gtForm("FormTitle") & "&nbsp;&nbsp;" & "" & "</td></tr></table>")
						result.Append("<table border=1 width=96% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>")
						result.Append("<tr><td><table border=0 width=100% cellspacing=0>")
						result.Append("<tr height=20>")
						For Each fd1 In fds1
							result.Append("<td class=headcell valign=top>" & fd1("form_field_name") & "</td>")
						Next
						result.Append("<td class=headcell valign=top>Date Created</td></tr>")
						For Each fd2 In fds2
							strHtml = ""
							strHtml = strHtml & "<tr>"
							For Each fd1 In fds1
								If (Not IsDBNull(fd2(fd1("form_field_name")))) Then
									If CheckDataType(CStr(fd2(fd1("form_field_name"))), DataType) = True Then
										If (CStr(fd2(fd1("form_field_name"))) <> "") Then
											bPaste = True
										End If
									End If
								End If
								strHtml = strHtml & "<td valign=top>" & fd2(fd1("form_field_name")) & "</td>"
							Next
							If (bPaste = True) Then
								strHtml = strHtml & "<td valign=top>" & fd2("date_created") & "</td>"
								strHtml = strHtml & "</tr>"
								result.Append(strHtml)
								bPaste = False
							End If
						Next
						result.Append("</table></td></tr></table></td></tr></table><hr>")
					End If
				Next
			Else
				fds1 = objForm.GetFormFieldsById(FormId)
				fds2 = objForm.TransferFormVariable(objFormData)
				If fds2.count > 0 Then
					bPaste = False
					bPaste = False
					result.Append("<table><tr><td>&nbsp;</td></tr></table>")
					result.Append("<table border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " & FormTitle & "&nbsp;&nbsp;" & "" & "</td></tr></table>")
					result.Append("<table border=1 width=96% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>")
					result.Append("<tr><td><table border=0 width=100% cellspacing=0>")
					result.Append("<tr height=20>")
					For Each fd1 In fds1
						result.Append("<td class=headcell valign=top>" & fd1("form_field_name") & "</td>")
					Next
					result.Append("<td class=headcell valign=top>Date Created</td></tr>")
					For Each fd2 In fds2
						strHtml = ""
						'strHtml = strHtml & "<tr>"
						'strHtml = strHtml & "<td valign=top><input type=""checkbox""/></td>"
						For Each fd1 In fds1
							If (Not IsDBNull(fd2(fd1("form_field_name")))) Then
								If CheckDataType(CStr(fd2(fd1("form_field_name"))), DataType) = True Then
									If (CStr(fd2(fd1("form_field_name"))) <> "") Then
										bPaste = True
									End If
								End If
							End If
							strHtml = strHtml & "<td valign=top>" & fd2(fd1("form_field_name")) & "</td>"
						Next

						If (bPaste = True) Then
							strHtml = strHtml & "<td valign=top>" & fd2("date_created") & "</td>"
							strHtml = strHtml & "</tr>"
							result.Append(strHtml)
							bPaste = False
						End If
					Next
					result.Append("</table></td></tr></table></td></tr></table><hr>")
				End If
			End If
		Else
			If CStr(FormId) = "" Then
				For Each gtForm In gtForms
					tmpFormId = gtForm("formid")
					result.Append("<table><tr><td>&nbsp;</td></tr></table>")
					result.Append("<table border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " & gtForm("FormTitle") & "</td></tr><tr><td align=left class=lbls>ID: " & tmpFormId & "</td></tr></table>")
					result.Append("<table border=1 width=96% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>")
					result.Append("<tr><td><table border=0 width=100% cellspacing=0>")
					result.Append("<tr height=20><td class=headcell align=center width=5% >Id</td><td class=headcell width=20% >Variable Name</td><td class=headcell width=55% >Value</td><td class=headcell width=25% >Date Submited</td></tr>")
					iCnt = 1
					For Each cData In cDatas
						If CheckDataType(CStr(cData("FORM_FIELD_VALUE")), DataType) = True Then
							bPaste = True
							If (LCase(DataType) <> "all") Then
								If (CStr(cData("FORM_FIELD_VALUE")) = "") Then
									bPaste = False
								Else
									bPaste = True
								End If
							End If
							If (bPaste) Then
								If CStr(tmpFormId) = CStr(cData("FORM_ID")) Then
									If CInt(iCnt / 2) = (iCnt / 2) Then
										result.Append("<tr class=evenrow><td valign=top align=center>" & cData("FORM_DATA_ID") & "</td><td>" & cData("FORM_FIELD_NAME") & "</td><td>" & cData("FORM_FIELD_VALUE") & "</td><td>" & cData("DATE_CREATED") & "</td></tr>")
									Else
										result.Append("<tr><td valign=top align=center>" & cData("FORM_DATA_ID") & "</td><td>" & cData("FORM_FIELD_NAME") & "</td><td>" & cData("FORM_FIELD_VALUE") & "</td><td>" & cData("DATE_CREATED") & "</td></tr>")
									End If
									iCnt = iCnt + 1
								End If
							End If
						End If
					Next
					result.Append("</table></td></tr></table></td></tr></table><hr>")
				Next
			Else
				result.Append("<table><tr><td>&nbsp;</td></tr></table>")
				result.Append("<table border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " & Request.QueryString("form_title") & "</td></tr><tr><td align=left class=lbls>ID: " & FormId & "</td></tr></table>")
				result.Append("<table border=1 width=100% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>")
				result.Append("<tr><td><table border=0 width=100% cellspacing=0>")
				result.Append("<tr height=20>")
				'If (Security_info.CanDelete) Then
				'    result.Append("<td class=headcell align=""center"" width=1% nowrap=""true"">(Delete)<br><input type=""checkbox"" name=""chkSelectAll"" onClick=""SelectAll(this)""></td>")
				'End If
				result.Append("<td class=headcell align=center width=5% >Id</td><td class=headcell width=20% >Variable Name</td><td class=headcell width=55% >Value</td><td class=headcell >Date Submited</td></tr>")
				iCnt = 1
				For Each cData In cDatas
					strHtml = ""
					If CheckDataType(CStr(cData("FORM_FIELD_VALUE")), DataType) = True Then
						bPaste = True
						If (LCase(DataType) <> "all") Then
							If (CStr(cData("FORM_FIELD_VALUE")) = "") Then
								bPaste = False
							Else
								bPaste = True
							End If
						End If
						If (bPaste) Then
							If CInt(iCnt / 2) = (iCnt / 2) Then
								result.Append("<tr class=evenrow><td align=left valign=top>" & cData("FORM_DATA_ID") & "</td><td valign=top>" & cData("FORM_FIELD_NAME") & "</td><td valign=top>" & cData("FORM_FIELD_VALUE") & "</td><td valign=top>" & cData("DATE_CREATED") & "</td></tr>")
							Else
								result.Append("<tr><td align=left valign=top>" & cData("FORM_DATA_ID") & "</td><td valign=top>" & cData("FORM_FIELD_NAME") & "</td><td valign=top>" & cData("FORM_FIELD_VALUE") & "</td><td valign=top>" & cData("DATE_CREATED") & "</td></tr>")
							End If
							iCnt = iCnt + 1
						End If
					End If
					If (dataID <> cData("FORM_DATA_ID")) Then
						dataID = cData("FORM_DATA_ID")
					End If
				Next
				result.Append("</table></td></tr></table></td></tr></table><hr>")
			End If
		End If
		Return (result.ToString)
	End Function
	Private Function CheckDataType(ByVal TEXT As String, ByVal DataType As String) As Boolean
		TEXT = TEXT.ToLower
		CheckDataType = False
		If DataType = "All" Then
			CheckDataType = True
		ElseIf DataType = "Date" Then
			If IsDate(TEXT) Then
				CheckDataType = True
			End If
		ElseIf DataType = "Boolean" Then
			If (TEXT = "1" Or TEXT = "yes" Or TEXT = "no" Or TEXT = "0" Or TEXT = "on" Or TEXT = "off" Or TEXT = "true" Or TEXT = "false") Then
				CheckDataType = True
			End If
		ElseIf DataType = "Numeric" Then
			If IsNumeric(TEXT) Then
				CheckDataType = True
			End If
		ElseIf DataType = "Text" Then
			If (Len(TEXT) > 0) Then
				CheckDataType = True
			End If
		End If
	End Function
End Class
