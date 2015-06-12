Imports Ektron.Cms

Partial Class cmsformsreport
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
    Protected AppPath As String = ""
    Protected ContentLanguage As Integer = -1
    Protected Flag As String = "false"
    Protected Security_info As PermissionData
    Protected Action As String = ""
    Protected ResultType As String = ""
    Protected EnableMultilingual As Integer = 0
    Protected objForm As Ektron.Cms.Modules.EkModule
	Protected strFolderID As String
    Protected SelectedhId As Long = 0
	Protected QueryLang As String = ""
    Protected sPollFieldId As String = ""
    Protected sExcelPrefix As String = "<html><head><meta http-equiv=Content-Type content=""text/html; charset=utf-8""><meta name=ProgId content=Excel.Sheet></head><body>"
    Protected sExcelSuffix As String = "</body></html>"
#End Region

#Const SaveXmlAsFile = False

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		BtnExport.Visible = False
        If (Request.Params("HTTPS") <> "on") Then
            'Adding lines below causes HTTPS not to work.
            'IE bug http://support.microsoft.com/default.aspx?scid=kb;en-us;812935
            Response.CacheControl = "no-cache"
            Response.AddHeader("Pragma", "no-cache")
        End If
		Response.Expires = -1

        RegisterResources()

		'Put user code to initialize the page here
		StyleSheetJS.Text = m_refStyle.GetClientScript
        m_refMsg = m_refContentApi.EkMsgRef
        BtnExport.Text = m_refMsg.GetMessage("btn export")
		lblStartDate.Text = m_refMsg.GetMessage("generic start date label")
        lblEndDate.Text = m_refMsg.GetMessage("generic end date label")
        litGetResult.Text = m_refMsg.GetMessage("lbl get report")
		CurrentUserId = m_refContentApi.UserId
		ContentLanguage = m_refContentApi.ContentLanguage
        AppImgPath = m_refContentApi.AppImgPath
        AppPath = m_refContentApi.AppPath
		Action = Request.QueryString("action")
		FormId = Request.QueryString("id")
		StartDate = Request("start_date")
        EndDate = Request("end_date")
        If (m_refContentApi.RequestInformationRef.IsMembershipUser = 1) Then
            Utilities.ShowError(m_refMsg.GetMessage("com: user does not have permission"))
            Exit Sub
        End If
		If ((StartDate <> "") And (EndDate = "")) Then
			EndDate = FormatDateTime(Now(), DateFormat.ShortDate)
		End If
		Flag = Request("flag")
		DataType = Request("data_type")
		ResultType = Request("result_type")
		DisplayType = Request("display_type")
		If Not (Request.QueryString("folder_id") Is Nothing) Then
			strFolderID = Request.QueryString("folder_id")
		End If
		If Not (Request.QueryString("hid") Is Nothing) Then
            SelectedhId = CLng(Request.QueryString("hid"))
		End If

		If (DisplayType = "") Then
			DisplayType = "horizontal"
		End If

		If (Request.QueryString("LangType") <> "") Then
			ContentLanguage = Request.QueryString("LangType")
			m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
		Else
			If CStr(m_refContentApi.GetCookieValue("LastValidLanguageID")) <> "" Then
				ContentLanguage = m_refContentApi.GetCookieValue("LastValidLanguageID")
			End If
		End If
		EnableMultilingual = m_refContentApi.EnableMultilingual
		m_refContentApi.ContentLanguage = ContentLanguage

		Security_info = m_refContentApi.LoadPermissions(FormId, "content")
		objForm = m_refContentApi.EkModuleRef
		If SelectedhId > 0 Then
			DefaultFormTitle = Request.QueryString("FormTitle")
		ElseIf ((Convert.ToString(FormId) <> "") AndAlso (FormId > 0)) Then
			DefaultFormTitle = objForm.GetFormTitleById(FormId)
		Else
			DefaultFormTitle = Request.QueryString("FormTitle")
		End If
		gtForms = objForm.GetAllFormInfo()
		'If (Flag = "true" And ResultType = "export") Then
		'	Response.AddHeader("content-disposition", "attachment; filename=" & "Form_Data_Export" & ".xls")
		'End If
		If (Action = "delete") Then
			Dim DelDataID As String
			Dim ret As Boolean
			DelDataID = Request.Form("delete_data_id")
			ret = objForm.PurgeFormData(FormId, DelDataID)
			Flag = "true"
		End If
		Dim cHistData As Collection
		Dim cData As Collection
		Dim aReportHistoryId As ArrayList = Nothing
		Dim aReportTitle As ArrayList = Nothing
		cHistData = m_refContentApi.EkContentRef.GetHistoryListv2_0(FormId)
		If cHistData.Count > 0 Then
			aReportHistoryId = New ArrayList
			aReportTitle = New ArrayList
			For Each cData In cHistData
				If "A" = cData("ContentStatus") Then
					aReportHistoryId.Add(cData("HistoryID"))
					aReportTitle.Add(cData("ContentTitle"))
				End If
			Next
		End If

		If (Flag = "true") Then
            FormResult.Text = LoadResult()
            If "" = ExportResult.Text Then
                ExportResult.Text = sExcelPrefix & FormResult.Text & sExcelSuffix
            End If
            ExportResult.Visible = False
		End If
		FormsReportToolBar()
		DisplayDateFields()
		DisplayHistoryOption(aReportHistoryId, aReportTitle)
		DisplaySelectReport(DisplayType, FormId)
	End Sub

	Private Sub DisplayHistoryOption(ByVal HistoryId As ArrayList, ByVal HistoryTitle As ArrayList)
		Dim sbSelect As New StringBuilder
		Dim i As Integer
		If Not IsNothing(HistoryId) AndAlso HistoryId.Count > 1 Then
            sbSelect.Append("<tr>" & vbCrLf)
            sbSelect.Append("<td class=""label""><input type=""hidden"" id=""selhid"" name=""selhid"" value=""""/>" & m_refMsg.GetMessage("lbl select legacy report") & ":</td>" & vbCrLf)
            sbSelect.Append("<td colspan=""3"">" & vbCrLf)
            sbSelect.Append("<select name=""selhistory"">" & vbCrLf)
            For i = 0 To HistoryId.Count - 1
                sbSelect.Append("<option value=""" & HistoryId(i) & """")
                If SelectedhId = HistoryId(i) Then
                    sbSelect.Append(" selected ")
                End If
                sbSelect.Append(">" & HistoryTitle(i))
                sbSelect.Append(" (ver." & HistoryId.Count - i & ")")
                sbSelect.Append("</option>" & vbCrLf)
            Next
            sbSelect.Append("</select>" & vbCrLf)
            For i = 0 To HistoryId.Count - 1
                sbSelect.Append("<input type=""hidden"" id=""hid_" & HistoryId(i) & """ name=""hid_" & HistoryId(i) & """")
                sbSelect.Append(" value=""" & HistoryTitle(i) & """/>" & vbCrLf)
            Next
            sbSelect.Append("</td>" & vbCrLf)
            sbSelect.Append("</tr>" & vbCrLf)
        Else
            sbSelect.Append("<input type=""hidden"" id=""selhid"" name=""selhid"" value=""none""/>" & vbCrLf)
        End If
		SelectHistoryReport.Text = sbSelect.ToString

	End Sub

    Private Sub DisplayDateFields()
        Dim dateSchedule As EkDTSelector
        dateSchedule = Me.m_refContentApi.EkDTSelectorRef
        dateSchedule.formName = "frmReport"
        dateSchedule.extendedMeta = True
        dateSchedule.formElement = "start_date"
        dateSchedule.spanId = "start_date_span"
        If StartDate <> "" Then
            dateSchedule.targetDate = CDate(StartDate)
        End If
        dtStart.Text = dateSchedule.displayCultureDate(True)
        dateSchedule.formElement = "end_date"
        dateSchedule.spanId = "end_date_span"
        If EndDate <> "" Then
            dateSchedule.targetDate = CDate(EndDate)
        End If
        dtEnd.Text = dateSchedule.displayCultureDate(True)
        If (StartDate = "") Then
            dtEnd.Text &= "<script>var oForm = document.forms['frmReport']; clearDTvalue(oForm.start_date, 'start_date_span', '" & m_refMsg.GetMessage("dtselect: no date") & "'); oForm.start_date_dow.value = ''; oForm.start_date_dom.value = ''; oForm.start_date_monum.value = ''; oForm.start_date_yrnum.value = '';</script>"
        End If
        If (EndDate = "") Then
            dtEnd.Text &= "<script>var oForm = document.forms['frmReport']; clearDTvalue(oForm.end_date, 'end_date_span', '" & m_refMsg.GetMessage("dtselect: no date") & "'); oForm.end_date_dow.value = ''; oForm.end_date_dom.value = ''; oForm.end_date_monum.value = ''; oForm.end_date_yrnum.value = '';</script>"
        End If
    End Sub
    Private Sub FormsReportToolBar()
        Dim result As New System.Text.StringBuilder
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar("" & m_refMsg.GetMessage("alt view forms report") & " " & """" & DefaultFormTitle & """")
        result.Append("<table><tr>")
        If Flag = "true" Then
			'If (Not Utilities.IsMac()) Then
			'	result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_export-nm.gif", "javascript:export_result()", m_refMsg.GetMessage("btn export raw data"), m_refMsg.GetMessage("btn export raw data"), ""))
			'End If
            If (Security_info.CanDelete) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt msg del form data"), m_refMsg.GetMessage("btn delete"), "onclick=""return ConfirmDelete();return false;"" "))
            End If
        End If
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "cmsform.aspx?action=ViewForm&form_id=" & FormId & "&LangType=" & ContentLanguage & "&folder_id=" & strFolderID, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>" & m_refStyle.GetHelpButton("formreport") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing

    End Sub

    Private Sub DisplaySelectReport(ByVal DisplayType As String, ByVal FormId As Long)
        Dim strFormsURL As String
        Dim strFormsPath As String
        Dim strManifestFilePath As String
        Dim strManifestURL As String
        Dim strXsltFilePath As String
        Dim strFieldList As String
        Dim sbSelect As New StringBuilder

        strFormsURL = m_refContentApi.QualifyURL(m_refContentApi.AppPath, "controls/forms/")
        strFormsPath = Server.MapPath(strFormsURL)
        strManifestURL = m_refContentApi.FullyQualifyURL(strFormsURL & "FormReportsManifest.xml")
        strManifestFilePath = strFormsPath & "FormReportsManifest.xml"
        strXsltFilePath = strFormsPath & "SelectFormReport.xslt"

        strFieldList = ""
        If (Not IsNothing(FormId) And FormId > 0) Then
            strFieldList = m_refContentApi.EkModuleRef.GetFormFieldListXml(FormId)
        End If

        Dim objXsltArgs As New System.Xml.Xsl.XsltArgumentList
        If IsNumeric(DisplayType) Then
            objXsltArgs.AddParam("selectedIndex", String.Empty, DisplayType)
        End If
        If m_refContentApi.ContentLanguage > 0 Then
            Dim language_data As LanguageData
            Dim strLang As String
            language_data = (New SiteAPI).GetLanguageById(m_refContentApi.ContentLanguage)
            strLang = language_data.BrowserCode
            If strLang <> "" Then
                objXsltArgs.AddParam("lang", String.Empty, strLang)
            End If
        End If
        If (strFieldList.Length > 0) Then
            objXsltArgs.AddParam("manifest", String.Empty, strManifestURL)
        End If
        sbSelect.Append("<select name=""seldisplay"">")
        If (strFieldList.Length > 0) Then
            sbSelect.Append(m_refContentApi.XSLTransform(strFieldList, strXsltFilePath, XsltAsFile:=True, XmlAsFile:=False, XsltArgs:=objXsltArgs))
        Else
            sbSelect.Append(m_refContentApi.XSLTransform(strManifestFilePath, strXsltFilePath, XsltAsFile:=True, XmlAsFile:=True, XsltArgs:=objXsltArgs))
        End If
        sbSelect.Append("</select>")
        SelectFormReport.Text = sbSelect.ToString
    End Sub

    ' ---------------------------------------------------------------------------
    'Function DoesKeyExist_FrmRpt(ByRef collectionObject, ByRef keyName) As Boolean
    '    Dim dummy As Object
    '    On Error Resume Next ' Used to determine condition, only affects this procedure 
    '    ' (reverts back to previous method when out of scope).
    '    Err.Clear()
    '    dummy = collectionObject.Item(keyName)
    '    If (Err.Number = 0) Then
    '        DoesKeyExist_FrmRpt = True
    '    Else
    '        DoesKeyExist_FrmRpt = False
    '    End If
    'End Function

    ' ---------------------------------------------------------------------------
    ' Safe Collection Reading (returns empty string if key-item doesn't exist):
    'Function CanIDoThis_FrmRpt(ByRef collectionObject, ByRef keyName)
    '    CanIDoThis_FrmRpt = False
    '    If (Not collectionObject Is Nothing) Then
    '        If (collectionObject.Count > 0) Then
    '            If (DoesKeyExist_FrmRpt(collectionObject, keyName)) Then
    '                CanIDoThis_FrmRpt = collectionObject(keyName)
    '            End If
    '        End If
    '    End If
    'End Function

    Private Sub PopulateForms()
        Dim result As New System.Text.StringBuilder
        For Each gtForm In gtForms
            If (DefaultFormTitle = gtForm("FormTitle")) Then
                result.Append("<option selected value=" & Chr(34) & gtForm("FormID") & Chr(34) & ">" & gtForm("FormTitle") & "</option>")
            Else
                result.Append("<option value=" & Chr(34) & gtForm("FormID") & Chr(34) & ">" & gtForm("FormTitle") & "</option>")
            End If
        Next
    End Sub

    Private Function DisplayReport(ByVal ReportID As String, ByVal FormInfo As Ektron.Cms.FormData, _
                                ByVal Data() As Ektron.Cms.FormSubmittedData, ByVal CanDelete As Boolean) As String
		Dim strFormsURL As String
		Dim strFormsPath As String
		Dim strManifestURL As String
		Dim strManifestFilePath As String
		Dim strXsltFilePath As String
        Dim objReport As System.Xml.XmlDocument
        Dim objNode As System.Xml.XmlNode

        Try
            If "" = ReportID Then
                Return "ERROR: Please select a report."
            ElseIf Not IsNumeric(ReportID) Then
                Return "ERROR: Invalid report ID: " & ReportID
			End If

			strFormsURL = m_refContentApi.QualifyURL(m_refContentApi.AppPath, "controls/forms/")
			strFormsPath = Server.MapPath(strFormsURL)
			strManifestURL = m_refContentApi.FullyQualifyURL(strFormsURL & "FormReportsManifest.xml")
			strManifestFilePath = strFormsPath & "FormReportsManifest.xml"

            objReport = New System.Xml.XmlDocument
            objReport.Load(strManifestFilePath)
            objNode = objReport.SelectSingleNode("/*/Reports/Report[" & ReportID & "]")
            If IsNothing(objNode) Then
                Return "ERROR: Could not find report in FormReportsManifest.xml. Report: " & ReportID
            End If

            objNode = objNode.SelectSingleNode("xslt/@src")
            If IsNothing(objNode) Then
                Return "ERROR: The report does not have a specified XSLT file in FormReportsManifest.xml. Report: " & ReportID
            End If
			strXsltFilePath = m_refContentApi.QualifyURL(strFormsPath, objNode.Value)

			Dim strXml As String
			strXml = m_refContentApi.EkModuleRef.SerializeFormData(FormInfo, Data)

			Dim objXsltArgs As New System.Xml.Xsl.XsltArgumentList
			''fill in the dynamic data for the fieldlist for this report (if apply).
			Dim sUpdatedXSLT As String = ""
			Dim sUpdatedFieldList As String = strFormsPath & "UpdateFieldList.xslt"
			objXsltArgs.AddParam("baseURL", String.Empty, m_refContentApi.FullyQualifyURL(""))
			objXsltArgs.AddParam("LangType", String.Empty, Convert.ToString(m_refContentApi.ContentLanguage))
			sUpdatedXSLT = m_refContentApi.XSLTransform(strXml, sUpdatedFieldList, XsltAsFile:=True)
			strXml = m_refContentApi.XSLTransform("<root/>", sUpdatedXSLT, XsltAsFile:=False, XsltArgs:=objXsltArgs)

            If True = CanDelete Then
                'a version for export report
                objXsltArgs = New System.Xml.Xsl.XsltArgumentList
                objXsltArgs.AddParam("canDelete", String.Empty, "false")
                objXsltArgs.AddParam("checkmarkUrl", String.Empty, m_refContentApi.FullyQualifyURL(m_refContentApi.QualifyURL(m_refContentApi.AppImgPath, "../UI/Icons/check.png")))
                Dim sExport As String
                sExport = m_refContentApi.XSLTransform(strXml, strXsltFilePath, XsltAsFile:=True, XsltArgs:=objXsltArgs)
                sExport = Regex.Replace(sExport, "</?(?i:pre)(.|\n)*?>", String.Empty) 'Defect # 45861 - Removing PRE tags
                ExportResult.Text = sExcelPrefix & sExport & sExcelSuffix
            Else
                ExportResult.Text = ""
            End If
            ExportResult.Visible = False

            objXsltArgs = New System.Xml.Xsl.XsltArgumentList
            objXsltArgs.AddParam("canDelete", String.Empty, IIf(CanDelete, "true", "false"))
            objXsltArgs.AddParam("checkmarkUrl", String.Empty, m_refContentApi.FullyQualifyURL(m_refContentApi.QualifyURL(m_refContentApi.AppImgPath, "../UI/Icons/check.png")))

#If SaveXmlAsFile Then
            ' Save XML as file for debugging purposes.
            Dim strXmlFilePath As String
            strXmlFilePath = "Sample" & FormInfo.Title.Replace(" ", "") & "Data.xml"
            strXmlFilePath = m_refContentApi.QualifyURL(strFormsPath, strXmlFilePath)

            Dim sw As New System.IO.StreamWriter(strXmlFilePath)
            sw.Write(strXml)
            sw.Close()
            sw = Nothing
#End If
            Return m_refContentApi.XSLTransform(strXml, strXsltFilePath, XsltAsFile:=True, XsltArgs:=objXsltArgs)

        Catch ex As Exception
            Return ""
            EkException.ThrowException(ex)
        Finally
            objNode = Nothing
            objReport = Nothing
        End Try
    End Function

    Private Function escapeXML(ByVal inString As String) As String
        Return m_refContentApi.EkModuleRef.escapeXML(inString)
    End Function

    Private Function LoadResult() As String

        Dim msgNoData As String = m_refMsg.GetMessage("msg no data report")
        If IsNumeric(DisplayType) Then
            Dim objFormInfo As Ektron.Cms.FormData
            If SelectedhId > 0 Then
                objFormInfo = m_refContentApi.GetFormByHistoryId(SelectedhId)
            Else
                objFormInfo = m_refContentApi.GetFormById(FormId)
            End If
            If IsNothing(objFormInfo) Then
                resultsMessage.Text = "<p class=""ui-state-highlight warningError"">ERROR: Could not find form. Form ID: " & FormId & "</p>"
                Return ""
            End If
            Dim objHistData As Hashtable
            objHistData = m_refContentApi.EkModuleRef.GetFormFieldQuestionsById(FormId)
            If objHistData.Count > 0 Then
                'only provide the QueryLang as allContenLanguage (-1) for poll and survey reports.
                'only need data from all languages to match up the poll result on the site.
                QueryLang = "-1"

                If (SelectedhId > 0) Then
                    Dim sTmp As String = "<ektdesignns_choices id="""
                    Dim iPos As Integer = -1
                    Dim iPos2 As Integer = -2

                    iPos = objFormInfo.Html.ToString().ToLower().IndexOf(sTmp)
                    If (iPos > -1) Then
                        iPos = iPos + sTmp.Length
                        iPos2 = objFormInfo.Html.ToString().ToLower().IndexOf("""", iPos)
                        If (iPos2 > -1) Then
                            sPollFieldId = objFormInfo.Html.ToString().ToLower().Substring(iPos, iPos2 - iPos)
                        End If
                    End If
                End If
            End If
            Dim aryData() As Ektron.Cms.FormSubmittedData
            aryData = m_refContentApi.EkModuleRef.GetFormFieldDataById(FormId, StartDate, EndDate, -1, QueryLang, sPollFieldId)
            If IsNothing(aryData) OrElse 0 = aryData.Length Then
                resultsMessage.Text = "<p class=""ui-state-highlight warningError"">" & msgNoData & "</p>"
                Return ""
            Else
                BtnExport.Visible = True
            End If

            Dim iData As Integer
            sFormDataIds = "'" & aryData(0).FormDataID & "'"
            For iData = 1 To aryData.Length - 1
                sFormDataIds = sFormDataIds & ",'" & aryData(iData).FormDataID & "'"
            Next

            Return DisplayReport(DisplayType, objFormInfo, aryData, Security_info.CanDelete)
        End If

        Dim result As New System.Text.StringBuilder
        Dim objFormData As Collection
        Dim cDatas As Collection
        Dim cData As Collection
        objForm = m_refContentApi.EkModuleRef
        objFormData = New Collection
        objFormData.Add(FormId, "FORM_ID")
        objFormData.Add(CurrentUserId, "USER_ID")
        objFormData.Add(StartDate, "START_DATE")
        objFormData.Add(EndDate, "END_DATE")
        cDatas = objForm.GetAllFormData(objFormData)
        If (cDatas.Count = 0) Then
            result.Append("<table><tr><td>" & msgNoData & "</td></tr></table>")
            Return (result.ToString)
        End If
        Dim iCnt, tmpFormId, dataID As Long
        Dim strHtml As String
        Dim bPaste As Boolean
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
                        result.Append("<table class=""ektronGrid"" border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " & gtForm("FormTitle") & "&nbsp;&nbsp;" & "" & "</td></tr></table>")
                        result.Append("<table border=1 width=96% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>")
                        result.Append("<tr><td><table border=0 width=100% cellspacing=0>")
                        result.Append("<tr height=20>")
                        For Each fd1 In fds1
                            If IsValidCol(fd1("form_field_name")) Then
                                result.Append("<td class=headcell valign=top>" & fd1("form_field_name") & "</td>")
                            End If
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
                                If IsValidCol(fd1("form_field_name")) Then
                                    strHtml = strHtml & "<td valign=top>" & fd2(fd1("form_field_name")) & "</td>"
                                End If
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
                    result.Append("<table><tr><td>&nbsp;</td></tr></table>")
                    result.Append("<table class=""ektronGrid"" border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " & Request.Form("Form_Title") & "&nbsp;&nbsp;" & "" & "</td></tr></table>")
                    result.Append("<table border=1 width=96% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>")
                    result.Append("<tr><td><table border=0 width=100% cellspacing=0>")
                    result.Append("<tr height=20>")
                    If (Security_info.CanDelete) Then
                        result.Append("<td class=headcell align=""center"" valign=top nowrap=""true"">(Delete)<br><input type=""checkbox"" name=""chkSelectAll"" onClick=""SelectAll(this)""></td>")
                    End If
                    For Each fd1 In fds1
                        If IsValidCol(fd1("form_field_name")) Then
                            result.Append("<td class=headcell valign=top>" & fd1("form_field_name") & "</td>")
                        End If
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
                            If IsValidCol(fd1("form_field_name")) Then
                                strHtml = strHtml & "<td valign=top>" & fd2(fd1("form_field_name")) & "</td>"
                            End If
                        Next

                        If (bPaste = True) Then
                            If (sFormDataIds <> "") Then
                                sFormDataIds = sFormDataIds & ",'" & fd2("form_data_id") & "'"
                            Else
                                sFormDataIds = "'" & fd2("form_data_id") & "'"
                            End If
                            If (Security_info.CanDelete) Then
                                strHtml = "<tr><td align=""center"" valign=top><input onClick=""CheckIt(this)"" type=""checkbox"" name=""ektChk" & fd2("form_data_id") & """ id=""ektChk" & fd2("form_data_id") & """/></td>" & strHtml
                            Else
                                strHtml = "<tr>" & strHtml ' we do not need <td>s around this.
                            End If
                            strHtml = strHtml & "<td valign=top>" & fd2("date_created") & "</td>"
                            strHtml = strHtml & "</tr>"
                            result.Append(strHtml)
                            bPaste = False
                        End If
                    Next
                    result.Append("</table></td></tr></table></td></tr></table><hr>")
                End If
            End If
        ElseIf DisplayType = "vertical" Then
            If CStr(FormId) = "" Then
                For Each gtForm In gtForms
                    tmpFormId = gtForm("formid")
                    result.Append("<table><tr><td>&nbsp;</td></tr></table>")
                    result.Append("<table class=""ektronGrid"" border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " & gtForm("FormTitle") & "</td></tr><tr><td align=left class=lbls>ID: " & tmpFormId & "</td></tr></table>")
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
                result.Append("<table class=""ektronGrid"" border=0 width=96% align=center cellspacing=0><tr><td align=left class=lbls>Title: " & Request.Form("form_title") & "</td></tr><tr><td align=left class=lbls>ID: " & FormId & "</td></tr></table>")
                result.Append("<table border=1 width=100% cellspacing=0 align=center><tr><td><table border=0 width=100% cellspacing=0 align=center>")
                result.Append("<tr><td><table border=0 width=100% cellspacing=0>")
                result.Append("<tr height=20>")
                If (Security_info.CanDelete) Then
                    result.Append("<td class=headcell align=""center"" width=1% nowrap=""true"">(Delete)<br><input type=""checkbox"" name=""chkSelectAll"" onClick=""SelectAll(this)""></td>")
                End If
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
                            If (sFormDataIds <> "") Then
                                sFormDataIds = sFormDataIds & ",'" & cData("form_data_id") & "'"
                            Else
                                sFormDataIds = "'" & cData("form_data_id") & "'"
                            End If
                            If CInt(iCnt / 2) = (iCnt / 2) Then
                                strHtml = "<tr class=evenrow>"
                            Else
                                strHtml = "<tr>"
                            End If
                            If (Security_info.CanDelete) Then
                                If (dataID <> cData("FORM_DATA_ID")) Then
                                    strHtml = strHtml & "<td align=""center"" valign=top><input onClick=""CheckIt(this)"" type=""checkbox"" name=""ektChk" & cData("FORM_DATA_ID") & """ id=""ektChk" & cData("FORM_DATA_ID") & """/></td>"
                                Else
                                    strHtml = strHtml & "<td valign=top></td>"
                                End If
                            End If
                            If IsValidCol(CStr(cData("FORM_FIELD_NAME"))) Then
                                result.Append(strHtml & "<td valign=top align=center>" & cData("FORM_DATA_ID") & "</td><td valign=top>" & CStr(cData("FORM_FIELD_NAME")) & "</td><td valign=top>" & CStr(cData("FORM_FIELD_VALUE")) & "</td><td valign=top>" & cData("DATE_CREATED") & "</td></tr>")
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
    Private Function IsValidCol(ByVal VariableName As String) As Boolean
        IsValidCol = True
        If Len(VariableName) = 0 Then Exit Function
        VariableName = VariableName.ToLower

        If ".x" = Right(VariableName, 2) Then
            IsValidCol = False
        ElseIf ".y" = Right(VariableName, 2) Then
            IsValidCol = False
        ElseIf "ecm" = Left(VariableName, 3) Then
            IsValidCol = False
        End If
	End Function
    Protected Sub BtnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnExport.Click
        HttpContext.Current.ApplicationInstance.Response.Clear()
        HttpContext.Current.ApplicationInstance.Response.AddHeader("content-disposition", "attachment;filename=form_data_export.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.xls"

        Dim stringWrite As New System.IO.StringWriter()
        Dim htmlWrite As New HtmlTextWriter(stringWrite)
        ExportResult.Visible = True
        ExportResult.RenderControl(htmlWrite)
        ExportResult.Visible = False
        Response.Write(stringWrite.ToString())
        Response.AddHeader("Accept-Header", System.Text.Encoding.ASCII.GetByteCount(ExportResult.Text))
       Try
            '****Note:User might get THreadAbortException error or "Internet Explorer Cannot Download" Error Message when they use an HTTPS. 
            'Microsoft Recommends to call HttpContext.Current.ApplicationInstance.CompleteRequest method instead of Response.End. 
            'However using CompleteRequest method appends content of the page apart from HTML representation of the XLS data. 
            'It's up the user to pick the option to either go with Response.End or CompleteRequest method and can be changed 
            'according to their requirement in the file [Workarea\cmsformsreport.aspx.vb]
            'http://support.microsoft.com/default.aspx?scid=kb;en-us;812935
            'http://support.microsoft.com/kb/312629
            If (Request.Params("HTTPS") <> "on") Then
                Response.End()
            Else
                HttpContext.Current.ApplicationInstance.CompleteRequest()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, AppPath & "java/workareahelper.js", "EktronWorkareaHelperJS")
    End Sub
End Class
