Imports Ektron.Cms
Imports Ektron.Cms.Common

Partial Class addcustomproperty
    Inherits System.Web.UI.UserControl

    Protected m_CommAPI As New CommonApi
    Protected m_UserRef As User.EkUser
    Protected PageAction As String
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Common.EkMessageHelper
    Protected AppImgPath As String = String.Empty
    Protected m_bIsEdit As Boolean = False
    Protected m_iId As Long = 0
    Protected ContentLanguage As Integer = -1
    Protected EnableMultiLanguage As Integer = -1
    Protected m_ucpdata As UserCustomPropertyData = Nothing
    Protected DisplaySelect As Boolean = False
    Protected m_strSelectedValue As String = ""
    Protected m_intValidationType As Integer = -1
#Region "Page Load"
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim ctrlName As String = String.Empty
        Me.txtMaxValue.Visible = True
        Me.txtMinValue.Visible = True
        TR_Message.Visible = True
        TR_Validation.Visible = True
        TR_inputType.Visible = True
        m_refMsg = m_CommAPI.EkMsgRef
        ctrlName = GetPostBackControlName()
        RegisterResources()
        If (ctrlName.ToString().ToLower() = ddInputType.UniqueID.ToString().ToLower() Or ctrlName.ToString().ToLower() = ddTypes.UniqueID.ToString().ToLower()) Then
            If IsPostBack Then
                If ((Not Request.Form(ddTypes.UniqueID) Is Nothing)) Then
                    ReDoForm(Request.Form(ddTypes.UniqueID))
                ElseIf (Not Request.Form("hdnddTypes") Is Nothing) Then
                    ReDoForm(Request.Form("hdnddTypes"))
                End If
            End If
        Else
            initPage()
            If (PageAction = "editcustomprop") Then
                m_bIsEdit = True
            End If
            If ((IsPostBack) Or (PageAction = "deletecustomprop")) Then
                DoProcess()
            Else
                SetForm()
            End If
        End If
    End Sub
#End Region

#Region "Private Helper Functions"
    Private Sub ReDoForm(ByVal Type As Integer)
        Dim iVlidationType As Integer = -1
        Select Case Type
            Case EkEnumeration.ObjectPropertyValueTypes.Category
                RestoreFieldValue(1)
                BindInputTypes(1)
                iVlidationType = EkEnumeration.ObjectPropertyDisplayTypes.CheckBox
            Case EkEnumeration.ObjectPropertyValueTypes.ThreadedDiscussion
                RestoreFieldValue(1)
                'ddInputType.Enabled = False
                BindInputTypes(1)
                iVlidationType = EkEnumeration.ObjectPropertyDisplayTypes.CheckBox
            Case EkEnumeration.ObjectPropertyValueTypes.Date
                RestoreFieldValue(1)
                BindInputTypes(3)
                iVlidationType = EkEnumeration.ObjectPropertyDisplayTypes.TextBox
            Case EkEnumeration.ObjectPropertyValueTypes.Boolean
                RestoreFieldValue(1)
                BindInputTypes(2)
                iVlidationType = EkEnumeration.ObjectPropertyDisplayTypes.CheckBox
            Case EkEnumeration.ObjectPropertyValueTypes.String
                RestoreFieldValue()
                BindInputTypes(4)
                iVlidationType = EkEnumeration.ObjectPropertyDisplayTypes.TextBox
            Case EkEnumeration.ObjectPropertyValueTypes.Numeric
                RestoreFieldValue(1)
                BindInputTypes(3)
                iVlidationType = EkEnumeration.ObjectPropertyDisplayTypes.TextBox
            Case EkEnumeration.ObjectPropertyValueTypes.MultiSelectList, EkEnumeration.ObjectPropertyValueTypes.SelectList
                RestoreFieldValue(8)
                BindInputTypes(8)
            Case Else
                RestoreFieldValue()
                BindInputTypes()
                'GetAllValidation()
        End Select
        If (Not m_ucpdata Is Nothing) Then
            iVlidationType = m_ucpdata.PropertyDisplayValueType
        End If
        GetAllValidation(iVlidationType)
        If (Type = EkEnumeration.ObjectPropertyValueTypes.Date) Then
            If (Not m_ucpdata Is Nothing) Then
                DisplayDateFields(m_ucpdata.PropertyValidationMinVal, m_ucpdata.PropertyValidationMaxVal)
            Else
                DisplayDateFields("", "")
            End If
        End If
    End Sub
    Private Sub BindInputTypes(Optional ByVal Type As Integer = 0)
        'Type = 0 - Display all
        'Type = 1 - Display Select, checkboxed
        'Type = 2 - Display checkbox - boolean
        'Type = 3 - Display text box
        'Type = 4 - text box, hidden, textarea
        'Type = 5 - * (can be anything, TBD)
        Dim i As Integer
        Dim lsItem As ListItem
        Dim strAR As System.Array
        strAR = System.Enum.GetValues(GetType(EkEnumeration.ObjectPropertyDisplayTypes))
        ddInputType.Items.Clear()
        For i = 0 To strAR.Length - 1
            lsItem = New ListItem
            lsItem.Text = strAR.GetValue(i).ToString()
            lsItem.Value = strAR.GetValue(i)
            If (Type = 1) Then
                If ((strAR.GetValue(i) = EkEnumeration.ObjectPropertyDisplayTypes.CheckBox) _
                    Or (strAR.GetValue(i) = EkEnumeration.ObjectPropertyDisplayTypes.DropdownList) _
                    Or (strAR.GetValue(i) = EkEnumeration.ObjectPropertyDisplayTypes.MultiSelectList) _
                    Or (strAR.GetValue(i) = EkEnumeration.ObjectPropertyDisplayTypes.RadioButton) _
                    ) Then
                    ddInputType.Items.Add(lsItem)
                End If
            ElseIf (Type = 2) Then
                If ((strAR.GetValue(i) = EkEnumeration.ObjectPropertyDisplayTypes.CheckBox)) Then
                    ddInputType.Items.Add(lsItem)
                End If
            ElseIf (Type = 3) Then
                If (strAR.GetValue(i) = EkEnumeration.ObjectPropertyDisplayTypes.TextBox) Then
                    ddInputType.Items.Add(lsItem)
                End If
            ElseIf (Type = 4) Then
                If ((strAR.GetValue(i) = EkEnumeration.ObjectPropertyDisplayTypes.TextBox) _
                    Or (strAR.GetValue(i) = EkEnumeration.ObjectPropertyDisplayTypes.TextArea) _
                    Or (strAR.GetValue(i) = EkEnumeration.ObjectPropertyDisplayTypes.Hidden)) Then
                    ddInputType.Items.Add(lsItem)
                End If
            Else
                ddInputType.Items.Add(lsItem)
            End If


        Next
        ddInputType.SelectedIndex = 0
        If (Not Request.Form(ddTypes.UniqueID.ToString()) Is Nothing) Then
            Dim selVal As String = ""
            selVal = Request.Form(ddTypes.UniqueID.ToString()).ToString()
            ddTypes.SelectedIndex = ddTypes.Items.IndexOf(ddTypes.Items.FindByValue(selVal))
            'ddTypes.SelectedValue = viewstate("ddtypes")
        End If
    End Sub
    Private Sub BindCMSObjectTypes(Optional ByVal Type As Integer = 0)

        Dim i As Integer
        Dim lsItem As ListItem
        Dim strAR As System.Array
        Dim CategorydefID As Long = 0
        Dim bAdd As Boolean = True
        Dim contentRef As Ektron.Cms.Content.EkContent = m_CommAPI.EkContentRef
        CategorydefID = contentRef.GetCategoryDefinitionID()

        strAR = System.Enum.GetValues(GetType(EkEnumeration.ObjectPropertyValueTypes))
        ddTypes.Items.Clear()
        For i = 0 To strAR.Length - 1
            bAdd = True
            If ((Not Me.m_bIsEdit) AndAlso (strAR.GetValue(i) = EkEnumeration.ObjectPropertyValueTypes.Category)) Then
                If (CategorydefID <> 0) Then
                    bAdd = False
                End If
            End If
            If ((Not Me.m_bIsEdit) AndAlso (strAR.GetValue(i) = EkEnumeration.ObjectPropertyValueTypes.CategoryProperties)) Then
                bAdd = False
            End If
            If ((Not Me.m_bIsEdit) AndAlso (strAR.GetValue(i) = EkEnumeration.ObjectPropertyValueTypes.Notification)) Then
                bAdd = False
            End If
            If ((Not Me.m_bIsEdit) AndAlso (strAR.GetValue(i) = EkEnumeration.ObjectPropertyValueTypes.ThreadedDiscussion)) Then
                bAdd = False
            End If
            If (bAdd) Then
                lsItem = New ListItem
                lsItem.Text = strAR.GetValue(i).ToString()
                lsItem.Value = strAR.GetValue(i)
                ddTypes.Items.Add(lsItem)
            End If
        Next
    End Sub
    Private Sub RestoreFieldValue(Optional ByVal Type As Integer = 0)
        'Type = 0 - Restore all
        'Type = 1 - Only restore label and required

        If (Not Request.Form(txtLabel.UniqueID) Is Nothing) Then
            txtLabel.Text = Request.Form(txtLabel.UniqueID).ToString()
        End If

        If (Type = 0) Then
            If (Not Request.Form(txtMessage.UniqueID) Is Nothing) Then
                txtMessage.Text = Request.Form(txtMessage.UniqueID).ToString()
            End If
            If (Not Request.Form(txtMinValue.UniqueID) Is Nothing) Then
                Me.txtMinValue.Text = Request.Form(txtMinValue.UniqueID).ToString()
            End If
            If (Not Request.Form(txtMaxValue.UniqueID) Is Nothing) Then
                Me.txtMaxValue.Text = Request.Form(txtMaxValue.UniqueID).ToString()
            End If
        ElseIf Type = 8 Then
            DisplaySelect = True
            TR_Message.Visible = False
            TR_Validation.Visible = False
            TR_inputType.Visible = False
        End If

        If ((Not Request.Form("hdnddTypes") Is Nothing)) Then
            'we have to reregister the hidden field otherwise it will be lost
            Page.ClientScript.RegisterHiddenField("hdnddTypes", Request.Form("hdnddTypes"))
        End If
    End Sub
    Private Sub initPage()
        If (Not Request.QueryString("id") Is Nothing) Then
            m_iId = Request.QueryString("id")
        End If
        If (Not Request.QueryString("action") Is Nothing) Then
            PageAction = Request.QueryString("action").ToString().ToLower()
        End If
        m_refMsg = m_CommAPI.EkMsgRef
        AppImgPath = m_CommAPI.AppImgPath
        EnableMultiLanguage = m_CommAPI.EnableMultilingual
        If (Request.QueryString("LangType") <> "") Then
            ContentLanguage = Request.QueryString("LangType")
            m_CommAPI.SetCookieValue("LastValidLanguageID", ContentLanguage)
        Else
            If CStr(m_CommAPI.GetCookieValue("LastValidLanguageID")) <> "" Then
                ContentLanguage = m_CommAPI.GetCookieValue("LastValidLanguageID")
            End If
        End If
        m_CommAPI.ContentLanguage = ContentLanguage
        m_UserRef = m_CommAPI.EkUserRef
        m_ucpdata = m_UserRef.GetCustomProperty(m_iId)
    End Sub
    Private Sub View_AddCustomProp_Toolbar()
        Dim result As New System.Text.StringBuilder
        Dim sJS As New System.Text.StringBuilder

        sJS.Append("<script language=""Javascript"">" & vbCrLf)

        sJS.Append("    function VerifyAddCustomProp() {" & vbCrLf)
        sJS.Append("      try{ if(document.getElementById(""selectedvalues"").value==""""){ alert(""please add atleast one item into select list"");return false;}}catch (e){}" & vbCrLf)
        sJS.Append("        var lblText = document.getElementById('" & (Replace(txtLabel.UniqueID.ToString(), "$", "_")) & "');" & vbCrLf)
        sJS.Append("        var labelpattern=/^[a-zA-Z\d\u0000-\uFFFF][\w#@\s\u0000-\uFFFF]{0,127}$/;" & vbCrLf)
        sJS.Append("        if (lblText.value == '') { " & vbCrLf & "alert(""" & m_refMsg.GetMessage("js: enter property label").ToString() & """);" & vbCrLf)
        sJS.Append("        return false; " & vbCrLf & "}" & vbCrLf)
        sJS.Append("        if (!labelpattern.test(lblText.value)) { " & vbCrLf & "alert(""" & m_refMsg.GetMessage("js: invalid property label").ToString() & """);" & vbCrLf)
        sJS.Append("        return false; " & vbCrLf & "}" & vbCrLf)
        sJS.Append("try{" & vbCrLf)
        sJS.Append("        var ddValid = document.getElementById('" & (Replace(Me.ddValidationType.UniqueID.ToString(), "$", "_")) & "');" & vbCrLf)
        sJS.Append("        if (ddValid.selectedIndex > 0) { if (document.getElementById('" & (Replace(Me.txtMessage.UniqueID.ToString(), "$", "_")) & "').value == '') {alert(""Please enter validation message.""); return false;} } " & vbCrLf)
        sJS.Append("}catch (e){}" & vbCrLf)
        sJS.Append("        return true;" & vbCrLf)
        sJS.Append("" & vbCrLf)
        sJS.Append("    }" & vbCrLf)
        sJS.Append("    function VerifyDeleteCustomProp() {" & vbCrLf)
        sJS.Append("        return confirm('" & m_refMsg.GetMessage("js: delete user prop msg") & "');" & vbCrLf)
        sJS.Append("    }" & vbCrLf)
        sJS.Append("    function VerifyDeleteCustomPropLabel() {" & vbCrLf)
        sJS.Append("        return confirm('" & m_refMsg.GetMessage("js: delete user prop trans label") & "');" & vbCrLf)
        sJS.Append("    }" & vbCrLf)
        sJS.Append("</script>" & vbCrLf)
        result.Append(sJS.ToString())
        If m_bIsEdit Then
            If (m_CommAPI.DefaultContentLanguage = ContentLanguage) Then
                txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("user custom props edit"))
            Else
                txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("user custom props trans"))
            End If
        Else
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("user custom props add"))
        End If

        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt save button text (user property)"), m_refMsg.GetMessage("btn save"), "Onclick=""javascript:return SubmitForm('userinfo', 'VerifyAddCustomProp()');"""))
        If m_bIsEdit Then

            If (m_CommAPI.DefaultContentLanguage = ContentLanguage) Then
                If (Not m_ucpdata.PropertyValueType = EkEnumeration.ObjectPropertyValueTypes.Category) And (Not m_ucpdata.PropertyValueType = EkEnumeration.ObjectPropertyValueTypes.ThreadedDiscussion) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/delete.png", "users.aspx?action=DeleteCustomProp&id=" & m_iId, m_refMsg.GetMessage("alt delete button text"), m_refMsg.GetMessage("btn delete"), "Onclick=""javascript:return VerifyDeleteCustomProp();"""))
                End If
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/remove.png", "users.aspx?action=DeleteCustomProp&type=label&id=" & m_iId, m_refMsg.GetMessage("alt delete button text"), m_refMsg.GetMessage("btn delete"), "Onclick=""javascript:return VerifyDeleteCustomPropLabel();"""))
            End If
            If (EnableMultiLanguage = 1) Then
                result.Append("<td align=""right"">" & m_refStyle.ShowAllActiveLanguage(False, "", "javascript:SelLanguage(this.value);", ContentLanguage) & "</td>")
            Else
                result.Append("<td>&nbsp;</td>")
            End If

        End If
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "users.aspx?action=ViewCustomProp&LangType=" & ContentLanguage & "&OrderBy=" & Request.QueryString("OrderBy"), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("AddCustomProperty"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub SetForm()
        If (Not m_bIsEdit) Then
            'Input
            BindInputTypes(4)
            GetAllValidation(EkEnumeration.ObjectPropertyDisplayTypes.TextBox)
        Else
            'Input
            BindInputTypes()
            GetAllValidation()
        End If
        'Type
        BindCMSObjectTypes()
        If (m_bIsEdit) Then
            ReDoForm(m_ucpdata.PropertyValueType)
            Pupolate_CustomProperty(m_iId)

            lblType.Enabled = False
            ddTypes.Enabled = False
            If (ContentLanguage <> m_CommAPI.DefaultContentLanguage) Then
                OnlyLabelEdit(True)
            End If
        End If
        If (PageAction = "addcustomprop" Or PageAction = "editcustomprop") Then
            View_AddCustomProp_Toolbar()
        End If
    End Sub
    Private Sub OnlyLabelEdit(ByVal bDisable As Boolean)
        'Hide controls
        Me.lblInputType.Visible = False
        Me.ddInputType.Visible = False
        Me.TR_Min.Visible = False
        Me.TR_Max.Visible = False
        Me.lblType.Visible = False
        Me.ddTypes.Visible = False
        'Disable controls
        Me.lblValidation.Enabled = False
        Me.ddValidationType.Enabled = False
    End Sub
    Private Sub ShowValidationUI(ByVal bShow As Boolean)
        'Me.TR_Min.Visible = bShow
        'Me.TR_Max.Visible = bShow
        'Me.TR_Message.Visible = bShow
        Me.lblMaxVal.Visible = bShow
        Me.txtMaxValue.Visible = bShow
        Me.lblMinVal.Visible = bShow
        Me.txtMinValue.Visible = bShow
        Me.lblMessage.Visible = bShow
        Me.txtMessage.Visible = bShow
    End Sub

    Private Sub GetAllValidation(Optional ByVal DisplayType As Integer = -1)
        Dim arMin As String = String.Empty
        Dim arMax As String = String.Empty
        Dim cEnums As New Collection
        Dim cEnum As Collection
        Dim lsItem As ListItem
        Dim selVal As String = ""
        Dim iValType As Integer = 0
        Dim ctrlName As String = String.Empty

        If (IsPostBack) Then
            ctrlName = GetPostBackControlName()
            If (ctrlName.ToString().ToLower() = ddInputType.UniqueID.ToString().ToLower()) Then
                selVal = Request.Form(ddInputType.UniqueID.ToString()).ToString()
            ElseIf ctrlName.ToString().ToLower() = ddTypes.UniqueID.ToString().ToLower() Then
                selVal = DisplayType
            End If
        End If
        If (selVal = "") Then
            selVal = DisplayType
        End If
        ddInputType.SelectedIndex = ddInputType.Items.IndexOf(ddInputType.Items.FindByValue(selVal))
        Select Case (selVal)
            Case EkEnumeration.ObjectPropertyDisplayTypes.CheckBox
                cEnums = Nothing
            Case EkEnumeration.ObjectPropertyDisplayTypes.RadioButton
                cEnums = Nothing
            Case EkEnumeration.ObjectPropertyDisplayTypes.Hidden
                cEnums = Nothing
            Case EkEnumeration.ObjectPropertyDisplayTypes.DropdownList
                cEnums = (m_CommAPI.EkModuleRef).GetAllValidationEnum("SELECT", "", True)
            Case EkEnumeration.ObjectPropertyDisplayTypes.MultiSelectList
                cEnums = (m_CommAPI.EkModuleRef).GetAllValidationEnum("SELECT", "", True)
            Case EkEnumeration.ObjectPropertyDisplayTypes.TextArea
                cEnums = (m_CommAPI.EkModuleRef).GetAllValidationEnum("TEXTAREA", "TEXT", False)
            Case Else 'EkEnumeration.ObjectPropertyDisplayTypes.TextBox
                cEnums = (m_CommAPI.EkModuleRef).GetAllValidationEnum("INPUT", "TEXT", False)
        End Select
        'Clear the validation dropdown list
        Me.ddValidationType.Items.Clear()
        'Populate the dropdown list but it should filter by type
        If (m_bIsEdit) Then
            iValType = m_ucpdata.PropertyValueType
        Else
            If (ddTypes.SelectedValue <> "") Then
                iValType = ddTypes.SelectedValue
            End If
        End If
        If (cEnums Is Nothing) Then
            ddValidationType.Enabled = False
            ShowValidationUI(False)
        Else
            ddValidationType.Enabled = True
            ShowValidationUI(True)
            For Each cEnum In cEnums
                lsItem = New ListItem
                lsItem.Text = cEnum("EnumName")
                lsItem.Value = cEnum("EnumID")
                If (CheckValidValidationType(iValType, cEnum("EnumID"))) Then
                    ddValidationType.Items.Add(lsItem)
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
                End If
            Next
            ddValidationType.Attributes.Add("onchange", "show_range2('" & arMin & "','" & arMax & "', this);Ektron.Workarea.Grids.init();")
            Page.ClientScript.RegisterClientScriptBlock(GetType(Page), "onChangeValidation", "<script language=""Javascript"">function myBodyLoad() { show_range2('" & (arMin) & "','" & (arMax) & "', document.forms[0].elements[""" & ddValidationType.UniqueID & """]);}</script>")
        End If
    End Sub

    Private Function CheckValidValidationType(ByVal Type As Integer, ByVal ValidationID As Integer) As Boolean
        If (Type = 0) Then Return True
        Select Case Type
            Case EkEnumeration.ObjectPropertyValueTypes.Date
                If ((ValidationID = 0) Or (ValidationID = 1) Or (ValidationID = 4)) Then
                    Return True
                End If
            Case EkEnumeration.ObjectPropertyValueTypes.Numeric
                If ((ValidationID = 0) Or (ValidationID = 1) Or (ValidationID = 2) Or (ValidationID = 11) Or (ValidationID = 16)) Then
                    Return True
                End If
            Case EkEnumeration.ObjectPropertyValueTypes.Category
                Return True
            Case Else 'EkEnumeration.ObjectPropertyValueTypes.String
        End Select
    End Function

    Private Function GetPostBackControlName() As String
        If (Not Page.Request.Params("__EVENTTARGET") Is Nothing) Then
            Return Page.Request.Params("__EVENTTARGET").ToString()
        End If
        Return String.Empty
    End Function

    Private Sub DisplayDateFields(ByVal StartDate As String, ByVal EndDate As String)
        'Hide text box
        Me.txtMaxValue.Visible = False
        Me.txtMinValue.Visible = False

        Dim dateSchedule As EkDTSelector
        dateSchedule = Me.m_CommAPI.EkDTSelectorRef
        dateSchedule.formName = "Form1"
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
    End Sub
#End Region

#Region "Populate Edit Form"
    Private Sub Pupolate_CustomProperty(ByVal Id As Long)
        If (Not m_ucpdata Is Nothing) Then
            txtLabel.Text = Server.HtmlDecode(m_ucpdata.Name)
            txtMessage.Text = Server.HtmlDecode(m_ucpdata.PropertyValidationMessage)
            ddValidationType.SelectedIndex = ddValidationType.Items.IndexOf(ddValidationType.Items.FindByValue(m_ucpdata.PropertyValidationType))
            If (ContentLanguage <> m_CommAPI.DefaultContentLanguage) Then
                Page.ClientScript.RegisterHiddenField("hdnRequired", m_ucpdata.Required)
                Page.ClientScript.RegisterHiddenField("hdnddTypes", m_ucpdata.PropertyValueType)
                Page.ClientScript.RegisterHiddenField("hdninputtype", m_ucpdata.PropertyDisplayValueType)
                Page.ClientScript.RegisterHiddenField("hdnddValidationType", m_ucpdata.PropertyValidationType)
                Page.ClientScript.RegisterHiddenField("hdnMinValue", m_ucpdata.PropertyValidationMinVal)
                Page.ClientScript.RegisterHiddenField("hdnMaxValue", m_ucpdata.PropertyValidationMaxVal)
                If (m_ucpdata.Name = "") Then
                    Page.ClientScript.RegisterHiddenField("hdnInsert", "true") 'Insert label on the pastback
                Else
                    Page.ClientScript.RegisterHiddenField("hdnInsert", "false") 'Update label on the pastback
                End If
            Else
                'chkRequired.Value = m_ucpdata.Required
                ddTypes.SelectedIndex = ddTypes.Items.IndexOf(ddTypes.Items.FindByValue(m_ucpdata.PropertyValueType))
                txtMinValue.Text = m_ucpdata.PropertyValidationMinVal
                txtMaxValue.Text = m_ucpdata.PropertyValidationMaxVal
                Page.ClientScript.RegisterHiddenField("hdnddTypes", m_ucpdata.PropertyValueType)
            End If
            If (m_ucpdata.PropertyValueType = EkEnumeration.ObjectPropertyValueTypes.SelectList Or m_ucpdata.PropertyValueType = EkEnumeration.ObjectPropertyValueTypes.MultiSelectList) Then
                DisplaySelect = True
                m_strSelectedValue = m_ucpdata.PropertyValidationSelectList
                m_intValidationType = m_ucpdata.PropertyValidationType
            End If
        End If
    End Sub
#End Region

#Region "Process on submit"
#Region "Helper Functions"
    Private Sub DoProcess()
        m_UserRef = m_CommAPI.EkUserRef

        If (PageAction = "editcustomprop") Then
            DoEditCustomProperty()
        ElseIf (PageAction = "deletecustomprop") Then
            DoDeleteCustomProperty()
        Else
            DoAddCustomProperty()
        End If
        If (PageAction <> "deletecustomprop") Then
            Response.Redirect(m_CommAPI.AppPath & "users.aspx?action=ViewCustomProp", False)
        End If

    End Sub

    Private Function ReadFormPostData() As UserCustomPropertyData
        Dim ucpdata As New UserCustomPropertyData
        Dim strLabel As String
        Dim bReq As Boolean = False

        Dim ty As EkEnumeration.ObjectPropertyValueTypes
        Dim inputtype As EkEnumeration.ObjectPropertyDisplayTypes
        Dim validation As Integer = 0
        Dim strValidationMsg As String = ""
        Dim strMinVal As String = ""
        Dim strMaxVal As String = ""

        strLabel = Request.Form(txtLabel.UniqueID).ToString.Trim
        If (Not Request.Form(txtMessage.UniqueID) Is Nothing) Then
            strValidationMsg = Request.Form(txtMessage.UniqueID).ToString().Trim()
        End If
        If (ContentLanguage <> m_CommAPI.DefaultContentLanguage) Then
            If ((Not Request.Form("hdnRequired") Is Nothing) AndAlso ((Request.Form("hdnRequired").ToString() = "on") Or (Request.Form("hdnRequired").ToString() = "1") Or (Request.Form("hdnRequired").ToString().ToLower() = "true"))) Then
                bReq = True
            End If
            ty = CInt(Request.Form("hdnddTypes").ToString())
            inputtype = CInt(Request.Form("hdninputtype"))
            validation = CInt(Request.Form("hdnddValidationType"))
            strMinVal = Request.Form("hdnMinValue").ToString().Trim()
            strMaxVal = Request.Form("hdnMaxValue").ToString().Trim()
        Else
            'If ((Not Request.Form(chkRequired.UniqueID.ToString) Is Nothing) AndAlso ((Request.Form(chkRequired.UniqueID.ToString).ToString() = "on") Or (Request.Form(chkRequired.UniqueID.ToString).ToString() = "1"))) Then
            '    bReq = True
            'End If
            If (Not Request.Form("hdnddTypes") Is Nothing) Then
                ty = CInt(Request.Form("hdnddTypes").ToString())
            Else
                ty = CInt(Request.Form(ddTypes.UniqueID.ToString()).ToString())
            End If
            inputtype = CInt(Request.Form(ddInputType.UniqueID))
            If (Not Request.Form(ddValidationType.UniqueID) Is Nothing) Then
                validation = CInt(Request.Form(ddValidationType.UniqueID))
            End If
            If (ty = EkEnumeration.ObjectPropertyValueTypes.Date) Then
                If (Not Request.Form("start_Date") Is Nothing) Then
                    strMinVal = Request.Form("start_Date").ToString().Trim()
                End If
                If (Not Request.Form("end_date") Is Nothing) Then
                    strMaxVal = Request.Form("end_date").ToString().Trim()
                End If
            Else
                If (Not Request.Form(txtMinValue.UniqueID) Is Nothing) Then
                    strMinVal = Request.Form(txtMinValue.UniqueID).ToString().Trim()
                End If
                If (Not Request.Form(txtMaxValue.UniqueID) Is Nothing) Then
                    strMaxVal = Request.Form(txtMaxValue.UniqueID).ToString().Trim()
                End If
            End If
        End If
        If ((ty = EkEnumeration.ObjectPropertyValueTypes.SelectList Or ty = EkEnumeration.ObjectPropertyValueTypes.MultiSelectList)) Then
            If (Request.Form("selectedvalues") = "") Then
                Throw New Exception("please add atleast one item into select list")
            Else
                ucpdata.PropertyValidationSelectList = Request.Form("selectedvalues")
                If (Request.Form("chkValidation") <> "") Then
                    bReq = True
                    validation = 8
                Else
                    bReq = False
                    validation = 0
                End If
            End If
        End If
        If (strLabel = String.Empty) Then Throw New Exception("Please insert the label.")
        ucpdata.Name = strLabel
        ucpdata.Required = bReq Or (validation > 0)
        ucpdata.PropertyValueType = ty
        ucpdata.PropertyValidationType = validation
        ucpdata.PropertyValidationMinVal = strMinVal
        ucpdata.PropertyValidationMaxVal = strMaxVal
        ucpdata.PropertyValidationMessage = strValidationMsg
        ucpdata.PropertyDisplayValueType = inputtype

        Return ucpdata
    End Function
#End Region

#Region "Add"
    Private Sub DoAddCustomProperty()
        Dim iRet As Long = 0

        Dim ucpdata As UserCustomPropertyData
        ucpdata = ReadFormPostData()
        iRet = m_UserRef.AddCustomProperty(ucpdata)
        'ltrJS.Text = "<script language=""JavaScript"">" & Environment.NewLine & "window.location='users.aspx?action=ViewCustomProp&LangType=" & m_CommAPI.DefaultContentLanguage & "';</script>"
    End Sub
#End Region

#Region "Edit"
    Private Sub DoEditCustomProperty()
        Dim ucpdata As UserCustomPropertyData
        Dim iRet As Long = 0
        Dim bInsert As Boolean = False

        ucpdata = ReadFormPostData()
        ucpdata.ID = m_iId
        If (ContentLanguage <> m_CommAPI.DefaultContentLanguage) Then
            If ((Not Request.Form("hdnInsert") Is Nothing) AndAlso (Request.Form("hdnInsert").ToString().ToLower() = "true")) Then
                bInsert = True
            End If
        End If
        If (bInsert) Then
            iRet = m_UserRef.AddCustomProperty(ucpdata)
        Else
            iRet = m_UserRef.UpdateCustomProperty(ucpdata)
        End If
        'ltrJS.Text = "<script language=""JavaScript"">" & Environment.NewLine & "window.location='users.aspx?action=ViewCustomProp&LangType=" & m_CommAPI.DefaultContentLanguage & "';</script>"
    End Sub
#End Region

#Region "Delete"
    Private Sub DoDeleteCustomProperty()
        Dim strAct As String = String.Empty
        Dim bRet As Boolean = False
        If (Not Request.QueryString("type") Is Nothing) Then
            If (Request.QueryString("type").ToString().Trim() <> String.Empty) Then
                strAct = Request.QueryString("type").ToString().Trim()
            End If
        End If
        If (ContentLanguage = m_CommAPI.DefaultContentLanguage) Then
            strAct = ""
        End If
        If (m_iId > 0) Then
            If (strAct = "label") Then
                'Remove only translated label not the property.
                bRet = m_UserRef.DeleteCustomProperty(m_iId, LabelOnly:=True)
                'Response.Redirect("users.aspx?action=EditCustomProp&id=" & m_iId & "&LangType=" & m_CommAPI.DefaultContentLanguage, False)
            Else
                bRet = m_UserRef.DeleteCustomProperty(m_iId)
            End If
            Response.Redirect("users.aspx?action=ViewCustomProp", False)
        End If
    End Sub
#End Region
#End Region
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_CommAPI.ApplicationPath & "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncs")
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
End Class