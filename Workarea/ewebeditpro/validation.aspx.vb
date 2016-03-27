Imports Ektron.Cms
Imports Ektron.Cms.Common
Partial Class validation
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
    Protected m_refContentApi As New ContentAPI
    Protected m_refMsg As EkMessageHelper
    Protected ValidationId As Long = 0
    Protected ValidationType As String = ""
    Protected MinVal As String = ""
    Protected MaxVal As String = ""
    Protected ValidationMsg As String = ""
    Protected editorName As String = ""
    Protected ContentId As Long = 0
    Protected Action As String = ""
    Protected FieldText As String = ""
    Protected objRuleData As Collection
    Protected rule_data As FormValidationData
    Protected arMin As String = ""
    Protected arMax As String = ""
    Protected strOptions As String = ""

#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.AllIE)
        'Put user code to initialize the page here
        Try
            ValidationId = Trim(Request("validation_id"))
            ValidationType = Request("validation_type")
            MinVal = Request("txtMin")
            MaxVal = Request("txtMax")
            ValidationMsg = Request("txtErrorMsg")
            editorName = Request("editorName")
            ContentId = Request("content_id")
            Action = Request("action").ToLower
            FieldText = Request("field_text")
            If (Not (Page.IsPostBack)) Then
                Display_Validation()
            Else
                Process_Validation()
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub Display_Validation()
        If ValidationId > 0 Then
            rule_data = m_refContentApi.GetRuleToEdit(0, ValidationId)
            If (Not (IsNothing(rule_data))) Then
                ValidationId = rule_data.Id
                ValidationType = rule_data.Type
                MinVal = rule_data.MinVal
                MaxVal = rule_data.MaxVal
                ValidationMsg = rule_data.Message
            End If
        End If
        Dim cEnums, cEnum, ElementName As Object
        Dim strOptions As String

        strOptions = ""

		If (InStr(1, FieldText, "<select") > 0 Or InStr(1, FieldText, "<SELECT") > 0) Then
			cEnums = m_refContentApi.GetAllValidationEnum("SELECT", "", True)
		ElseIf (InStr(1, FieldText, "<textarea") > 0 Or InStr(1, FieldText, "<TEXTAREA") > 0) Then
			cEnums = m_refContentApi.GetAllValidationEnum("TEXTAREA", "text", False)
		Else
			ElementName = "INPUT"
			If (InStr(1, FieldText, "type=""password") > 0 Or InStr(1, FieldText, "type=password") > 0) Then
				cEnums = m_refContentApi.GetAllValidationEnum("INPUT", "PASSWORD", False)
			Else
				cEnums = m_refContentApi.GetAllValidationEnum("INPUT", "TEXT", False)
			End If
		End If

        For Each cEnum In cEnums
            If (cEnum("EnumRange") <> 0) Then
                If (cEnum("EnumRange") = 3) Then
                    If (CStr(arMin) <> "") Then
                        arMin = arMin & ", " & cEnum("EnumID")
                    Else
                        arMin = cEnum("EnumID")
                    End If
                    If (CStr(arMax) <> "") Then
                        arMax = arMax & ", " & cEnum("EnumID")
                    Else
                        arMax = cEnum("EnumID")
                    End If
                Else
                    If (cEnum("EnumRange") = 1) Then
                        If (CStr(arMin) <> "") Then
                            arMin = arMin & ", " & cEnum("EnumID")
                        Else
                            arMin = cEnum("EnumID")
                        End If
                    Else
                        If (CStr(arMax) <> "") Then
                            arMax = arMax & ", " & cEnum("EnumID")
                        Else
                            arMax = cEnum("EnumID")
                        End If
                    End If
                End If
            End If
            strOptions = strOptions & "<option value=""" & cEnum("EnumID") & """" & isSelected(ValidationType, cEnum("EnumID")) & ">" & cEnum("EnumName") & "</option>" & vbCrLf
        Next
        vType.Text = "<select name=""selType"" onchange=""show_range2('" & arMin & "','" & arMax & "');"">"
        vType.Text += strOptions & "</select>"
    End Sub
    Private Sub Process_Validation()
        objRuleData = New Collection
        objRuleData.Add(m_refContentApi.UserId, "USER_ID")
        objRuleData.Add(ContentId, "CONTENT_ID")
        objRuleData.Add(ValidationId, "VALIDATION_ID")
        objRuleData.Add(ValidationType, "VALIDATION_TYPE")
        objRuleData.Add(MinVal, "MIN_VAL")
        objRuleData.Add(MaxVal, "MAX_VAL")
        objRuleData.Add(ValidationMsg, "VALIDATION_MSG")
        If (Action = "new") Then
            ValidationId = m_refContentApi.AddRule(objRuleData)
			If (InStr(1, FieldText, "<textarea") > 0) Then
				If (ValidationType <> 0) Then
					FieldText = Replace(FieldText, "<textarea", "<textarea ekv=" & Chr(34) & ValidationId & Chr(34) & " class=""redvalidation"" ")
				Else
					FieldText = Replace(FieldText, "<textarea", "<textarea ekv=" & Chr(34) & ValidationId & Chr(34) & " ")
				End If
			ElseIf (InStr(1, FieldText, "<TEXTAREA") > 0) Then
				If (ValidationType <> 0) Then
					FieldText = Replace(FieldText, "<TEXTAREA", "<TEXTAREA ekv=" & Chr(34) & ValidationId & Chr(34) & " class=""redvalidation"" ")
				Else
					FieldText = Replace(FieldText, "<TEXTAREA", "<TEXTAREA ekv=" & Chr(34) & ValidationId & Chr(34) & " ")
				End If
			ElseIf (InStr(1, FieldText, "<select") > 0) Then
				If ValidationType <> 0 Then
					FieldText = Replace(FieldText, "<select", "<select ekv=" & Chr(34) & ValidationId & Chr(34) & " class=""redvalidation"" ")
				Else
					FieldText = Replace(FieldText, "<select", "<select ekv=" & Chr(34) & ValidationId & Chr(34) & " ")
				End If
			ElseIf (InStr(1, FieldText, "<SELECT") > 0) Then
				If ValidationType <> 0 Then
					FieldText = Replace(FieldText, "<SELECT", "<SELECT ekv=" & Chr(34) & ValidationId & Chr(34) & " class=""redvalidation"" ")
				Else
					FieldText = Replace(FieldText, "<SELECT", "<SELECT ekv=" & Chr(34) & ValidationId & Chr(34) & " ")
				End If
			ElseIf (InStr(1, FieldText, "/>") > 0) Then
				If ValidationType <> 0 Then
					FieldText = Replace(FieldText, "/>", " ekv=" & Chr(34) & ValidationId & Chr(34) & " class=""redvalidation"" />")
				Else
					FieldText = Replace(FieldText, "/>", " ekv=" & Chr(34) & ValidationId & Chr(34) & " />")
				End If
			Else
				If ValidationType <> 0 Then
					FieldText = Replace(FieldText, ">", " ekv=" & Chr(34) & ValidationId & Chr(34) & " class=""redvalidation"" >")
				Else
					FieldText = Replace(FieldText, ">", " ekv=" & Chr(34) & ValidationId & Chr(34) & " >")
				End If

			End If
		ElseIf (Action = "update") Then
			m_refContentApi.UpdateRule(objRuleData)
			If (ValidationType = 0) Then
				FieldText = Replace(FieldText, "class=redvalidation", " ")
				FieldText = Replace(FieldText, "class=""redvalidation""", " ")
			ElseIf (InStr(1, FieldText, "class=""redvalidation""") = 0) Then
				FieldText = Replace(FieldText, "ekv=""", "class=""redvalidation"" ekv=""")
			ElseIf (InStr(1, FieldText, "class=redvalidation") = 0) Then
				FieldText = Replace(FieldText, "ekv=", "class=""redvalidation"" ekv=")
			End If
		End If
        CloseWindowScript()
    End Sub
    Private Function isSelected(ByVal sValue As String, ByVal sDefault As String) As String
        If sValue = sDefault Then
            isSelected = "selected"
        Else
            isSelected = ""
        End If
    End Function
    Private Function CloseWindowScript() As Boolean
        Dim result As New System.Text.StringBuilder
        result.Append("<script language=javascript>" & vbCrLf)
        result.Append("var objInstance = eWebEditProUtil.getOpenerInstance();" & vbCrLf)
        result.Append("objInstance.editor.pasteHTML('" & FieldText & "');" & vbCrLf)
        result.Append("self.close();" & vbCrLf)
        result.Append("</script>")
        CloseWindow.Text = result.ToString

        Return False
    End Function
End Class
