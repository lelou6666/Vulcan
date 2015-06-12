Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports System.Web.UI.HtmlControls

Partial Class editform
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
    Private pagedata As Collection
    Private form_data As FormData
    Private m_strPageAction As String = ""
    Private ContentLanguage As Integer = 0
    Private m_intFormId As Long = 0
	Private m_refModule As Ektron.Cms.Modules.EkModule
    Protected m_refContentApi As New ContentAPI
    Private m_refSiteApi As New SiteAPI
    Private AppImgPath As String = ""
    Protected m_refMsg As Common.EkMessageHelper
    Private m_refStyle As New StyleHelper
    Private folder_data As FolderData
    Private security_data As PermissionData
    Private m_intFolderId As Long = 0
    Private m_strOrderBy As String = ""
    Private CurrentUserId As Long = 0
    Private EnableMultilingual As Integer = 0
    Protected language_data As LanguageData
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        m_refMsg = m_refContentApi.EkMsgRef
        SetJSResourceStrings()
        RegisterResources()
        If (Not (Request.QueryString("action") Is Nothing)) Then
            m_strPageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If
        If (Not (Request.QueryString("folder_id") Is Nothing)) Then
            m_intFolderId = Convert.ToInt64(Request.QueryString("folder_id"))
        End If
        If (Not (Request.QueryString("form_id") Is Nothing)) Then
            If (Request.QueryString("form_id") <> "") Then
                m_intFormId = Convert.ToInt64(Request.QueryString("form_id"))
            End If
        End If
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

        ltr_formTitle.Text = m_refMsg.GetMessage("msg form title")
        ltr_msgSubmission.Text = m_refMsg.GetMessage("alert msg no of submission")
        ltr_emailReq.text = m_refMsg.GetMessage("msg email req")

        CurrentUserId = m_refContentApi.UserId
        AppImgPath = m_refContentApi.AppImgPath
        m_refModule = m_refContentApi.EkModuleRef
    End Sub
    Public Function AddForm() As Boolean
        If (Not (Page.IsPostBack)) Then
            frm_folder_id.Value = m_intFolderId
			frm_copy_lang_from.Value = Request.QueryString("back_LangType")
            language_data = m_refSiteApi.GetLanguageById(ContentLanguage)
            If EnableMultilingual = "1" Then
                lblLangName.Text = language_data.Name
            End If
            Display_AddForm()
        Else
            Process_DoAdd()
        End If
    End Function
    Public Function EditForm() As Boolean
        If (Not (Page.IsPostBack)) Then
            language_data = m_refSiteApi.GetLanguageById(ContentLanguage)
            If EnableMultilingual = "1" Then
                lblLangName.Text = language_data.Name
            End If
            Display_EditForm()
        Else
            Process_DoUpdate()
        End If
    End Function
#Region "ADDFORM"
    Private Sub Display_AddForm()
        'PostBackPage.Text = Utilities.SetPostBackPage("cmsform.aspx?LangType=" & ContentLanguage & "&Action=doAdd")
        Dim FormTitle As String = ""
        Dim FormDescription As String = ""
        Dim MailTo As String = ""
        Dim MailFrom As String = ""
        Dim MailCC As String = ""
        Dim MailPreamble As String = ""
        Dim MailSubject As String = ""
        Dim FormData As String = ""
        Dim CanCreateContent As Boolean = False
        Dim bCopyFromLang As Boolean = False
        FormTitle = Request.QueryString("form_title")
        FormDescription = Request.QueryString("form_description")
        MailTo = Request.QueryString("mail_to")
        MailFrom = Request.QueryString("mail_from")
        MailCC = Request.QueryString("mail_cc")
        MailFrom = Request.QueryString("mail_from")
        MailSubject = Request.QueryString("mail_subject")
        MailPreamble = Request.QueryString("mail_preamble")
        If (Request.Form(frm_form_mail.UniqueID) <> "" And Request.Form(frm_form_db.UniqueID) <> "") Then
            FormData = "both"
        ElseIf (Request.Form(frm_form_mail.UniqueID) <> "") Then
            FormData = Request.Form(frm_form_mail.UniqueID)
        Else
            FormData = Request.Form(frm_form_db.UniqueID)
        End If
		frm_form_db.Checked = True
		frm_form_af.Checked = True
		security_data = m_refContentApi.LoadPermissions(m_intFolderId, "folder")
        CanCreateContent = security_data.CanAdd
        If ((Request.QueryString("back_LangType") <> "") And (Request.QueryString("form_id") <> "")) Then
            'load translated form title
            m_refContentApi.ContentLanguage = Request.QueryString("back_LangType")
            FormTitle = m_refContentApi.GetFormTitleById(m_intFormId)
            FormTitle = FormTitle & "_"
            bCopyFromLang = True
        End If
        AddToolBar()

        DisplayAssignTaskTo()

        frm_folder_id.Value = m_intFolderId
        frm_copy_lang_from.Value = Request.QueryString("back_LangType")
		frm_form_title.Value = Ektron.Cms.Common.EkFunctions.HtmlDecode(FormTitle)
		frm_form_description.Value = Ektron.Cms.Common.EkFunctions.HtmlDecode(FormDescription)
        frm_form_mailto.Value = MailTo
        frm_form_mailfrom.Value = MailFrom
        frm_form_mailcc.Value = MailCC
        frm_form_mailpreamble.Value = MailPreamble
        frm_form_mailsubject.Value = MailSubject

        frm_initial_form.Value = ""
        frm_initial_response.Value = ""

        If Not bCopyFromLang Then
            Dim strFormsPath As String
            Dim strManifestFilePath As String
            Dim strXsltFilePath As String
			frm_copy_lang_from.Value = ""
            strFormsPath = Server.MapPath(m_refContentApi.AppPath) & "controls\forms\"
			strManifestFilePath = Utilities.QualifyURL(strFormsPath, "InitialFormsManifest.xml")
            strXsltFilePath = Utilities.QualifyURL(strFormsPath, "SelectInitialForm.xslt")

            Dim objXsltArgs As System.Xml.Xsl.XsltArgumentList = Nothing
            If m_refContentApi.ContentLanguage > 0 Then
                Dim language_data As LanguageData
                Dim strLang As String
                language_data = m_refSiteApi.GetLanguageById(m_refContentApi.ContentLanguage)
                strLang = language_data.BrowserCode
                If strLang <> "" Then
                    objXsltArgs = New System.Xml.Xsl.XsltArgumentList
                    objXsltArgs.AddParam("lang", String.Empty, strLang)
                End If
            End If
			SelectInitialForm.Text = m_refContentApi.XSLTransform(strManifestFilePath, strXsltFilePath, XsltAsFile:=True, XmlAsFile:=True, XsltArgs:=objXsltArgs)
        End If
    End Sub
    Private Sub Process_DoAdd()
        Dim result As Long = 0
		Dim strFormOutput As String = ""
		Try
			pagedata = New Collection
			pagedata.Add(Request.Form(frm_form_title.UniqueID), "FormTitle")
			pagedata.Add(Request.Form(frm_form_description.UniqueID), "FormDescription")
			If (Request.Form(frm_form_mail.UniqueID) <> "" And Request.Form(frm_form_db.UniqueID) <> "") Then
				strFormOutput = "both"
			ElseIf (Request.Form(frm_form_mail.UniqueID) <> "") Then
				strFormOutput = Request.Form(frm_form_mail.UniqueID)
			Else
				strFormOutput = Request.Form(frm_form_db.UniqueID)
			End If
			If Request.Form(frm_form_limit_submission.UniqueID) = "on" Then
				If Len(Request.Form(frm_form_number_of_submission.UniqueID)) AndAlso IsNumeric(Request.Form(frm_form_number_of_submission.UniqueID)) Then
					strFormOutput = strFormOutput & ",$" & Request.Form(frm_form_number_of_submission.UniqueID)
				Else
					strFormOutput = strFormOutput & ",$1" 'default to 1
				End If
			End If
			pagedata.Add(strFormOutput, "FormOutput")
			pagedata.Add((Request.Form(frm_form_af.UniqueID) <> ""), "Autofill")
			pagedata.Add(Request.Form(frm_form_mailto.UniqueID), "MailTo")
			pagedata.Add(Request.Form(frm_form_mailcc.UniqueID), "MailCc")
			pagedata.Add(Request.Form(frm_form_mailfrom.UniqueID), "MailFrom")
			pagedata.Add(Request.Form(frm_form_mailsubject.UniqueID), "MailSubject")
			pagedata.Add(Request.Form(frm_form_mailpreamble.UniqueID), "MailPreamble")
			pagedata.Add(Request.Form(frm_folder_id.UniqueID), "FolderID")
			If (Request.Form(frm_send_xml_packet.UniqueID) <> "") Then
				pagedata.Add(1, "SendXmlPacket")
			Else
				pagedata.Add(0, "SendXmlPacket")
			End If
			If (Request.Form(frm_multi_form_id.UniqueID) <> "") Then
				pagedata.Add(Request.Form(frm_multi_form_id.UniqueID), "FormId")
			Else
				pagedata.Add("0", "FormId")
			End If

			Dim strCopyLang As String
			Dim strFormType As String
			strCopyLang = Request.Form(frm_copy_lang_from.UniqueID)
			strFormType = Request.Form(frm_form_type.UniqueID)
			pagedata.Add(strCopyLang, "RefLanguage")

			pagedata.Add(Request.Form(assigned_to_user_id.UniqueID), "AssignTaskToUser")
			pagedata.Add(Request.Form(assigned_to_usergroup_id.UniqueID), "AssignTaskToUserGroup")

			Dim strFormsPath As String
			Dim strInitialForm As String
			Dim strInitialResponse As String
			strFormsPath = Server.MapPath(m_refContentApi.AppPath) & "controls\forms\"
			strInitialForm = Request.Form(frm_initial_form.UniqueID)
			strInitialResponse = Request.Form(frm_initial_response.UniqueID)
			strInitialForm = m_refContentApi.GetFileContents(Utilities.QualifyURL(strFormsPath, strInitialForm))
			strInitialResponse = m_refContentApi.GetFileContents(Utilities.QualifyURL(strFormsPath, strInitialResponse))

			pagedata.Add(strInitialForm, "InitialForm")
			pagedata.Add(strInitialResponse, "InitialResponse")

			result = m_refModule.AddNewFormV42(pagedata) 'ret as boolean
			'Response.Redirect("cmsform.aspx?folder_id=" & Request.Form(frm_folder_id.UniqueID) & "&LangType=" & ContentLanguage & "&Action=ViewForm&form_id=" & result, False)
			If Len(strCopyLang) > 0 Then
				strCopyLang = "&copy_lang=" & strCopyLang
			End If
			If Len(strFormType) > 0 Then
				strFormType = "&form_type=" & strFormType
            End If
            If (Not (Request.QueryString("FromEE") Is Nothing)) Then
                Response.Redirect("edit.aspx?new=true&close=true&LangType=" & ContentLanguage & strCopyLang & strFormType & "&id=" & result & "&type=update&back_file=cmsform.aspx&back_action=ViewForm&back_folder_id=" & Request.Form(frm_folder_id.UniqueID) & "&back_form_id=" & result & "&back_LangType=" & ContentLanguage, False)
            ElseIf (Not (Request.QueryString("buttonid") Is Nothing)) Then
                Response.Redirect("edit.aspx?new=true&close=false&LangType=" & ContentLanguage & strCopyLang & strFormType & "&id=" & result & "&type=update&back_file=&back_action=ViewForm&control=cbwidget&buttonid=" & Request.QueryString("buttonid") & "&back_folder_id=" & Request.Form(frm_folder_id.UniqueID) & "&back_form_id=" & result & "&back_LangType=" & ContentLanguage, False)
            Else
                Response.Redirect("edit.aspx?new=true&close=false&LangType=" & ContentLanguage & strCopyLang & strFormType & "&id=" & result & "&type=update&back_file=cmsform.aspx&back_action=ViewForm&back_folder_id=" & Request.Form(frm_folder_id.UniqueID) & "&back_form_id=" & result & "&back_LangType=" & ContentLanguage, False)
            End If
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub
    Private Sub AddToolBar()
        Dim result As System.Text.StringBuilder
        Dim callBackPage As String = ""
        result = New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("btn add form"))
        result.Append("<table><tr>")
        If (security_data.CanAdd) Then 'CanCreateContent
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", "Click here to save this Form", m_refMsg.GetMessage("btn save"), "Onclick=""javascript:$ektron('#pleaseWait').modalShow(); return SubmitForm('myform', 'VerifyAddForm()');$ektron('#pleaseWait').modalHide();"""))
        End If
		callBackPage = m_refStyle.getCallBackupPage("content.aspx?action=viewcontentbycategory&ID=" & m_intFolderId)
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", callBackPage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
#End Region
#Region "EDITFORM"
    Private Sub Display_EditForm()
        Dim strFormOutput As String = ""
        Dim strFormSubmission As String = ""

        EditFormPanel1.Visible = True
        form_data = m_refContentApi.GetFormById(m_intFormId)
        frm_folder_id.Value = m_intFolderId
        frm_copy_lang_from.Value = Request.QueryString("back_LangType")
        lblFormId.Text = form_data.Id
        frm_form_id.Value = form_data.Id
        frm_form_title.Value = Ektron.Cms.Common.EkFunctions.HtmlDecode(form_data.Title)
        frm_form_description.Value = Ektron.Cms.Common.EkFunctions.HtmlDecode(form_data.Description)
        frm_form_mailto.Value = form_data.MailTo
        If Len(ExtractFieldName(form_data.MailTo)) > 0 Then
            frm_form_mailto.Attributes.Add("readOnly", "true")
        End If
        frm_form_mailfrom.Value = form_data.MailFrom
        If Len(ExtractFieldName(form_data.MailFrom)) > 0 Then
            frm_form_mailfrom.Attributes.Add("readOnly", "true")
        End If
        frm_form_mailcc.Value = form_data.MailCc
        If Len(ExtractFieldName(form_data.MailCc)) > 0 Then
            frm_form_mailcc.Attributes.Add("readOnly", "true")
        End If
        frm_form_mailpreamble.Value = form_data.MailPreamble
        If Len(ExtractFieldName(form_data.MailPreamble)) > 0 Then
            frm_form_mailpreamble.Attributes.Add("readOnly", "true")
        End If
        frm_form_mailsubject.Value = form_data.MailSubject
        If Len(ExtractFieldName(form_data.MailSubject)) > 0 Then
            frm_form_mailsubject.Attributes.Add("readOnly", "true")
        End If

        If (form_data.StoreDataTo.IndexOf(",$") > -1) Then
            strFormOutput = m_refModule.IsEmailOrDb(form_data.StoreDataTo)
            strFormSubmission = form_data.StoreDataTo.Substring(form_data.StoreDataTo.IndexOf(",$") + 2)
        Else
            strFormOutput = m_refModule.IsEmailOrDb(form_data.StoreDataTo)
        End If

        'If ((form_data.StoreDataTo = "") Or (form_data.StoreDataTo = "mail") Or (form_data.StoreDataTo = "both")) Then
        If ((strFormOutput = "mail") Or (strFormOutput = "both")) Then
            frm_form_mail.Checked = True
        End If
        'If ((form_data.StoreDataTo = "") Or (form_data.StoreDataTo = "db") Or (form_data.StoreDataTo = "both")) Then
        If ((strFormOutput = "db") Or (strFormOutput = "both")) Then
            frm_form_db.Checked = True
        End If
        frm_form_af.Checked = form_data.Autofill

        If (strFormSubmission <> "") Then
            frm_form_limit_submission.Checked = True
            frm_form_number_of_submission.Value = strFormSubmission
        Else
            frm_form_limit_submission.Checked = False
        End If

        If (form_data.SendXmlPacket) Then
            frm_send_xml_packet.Checked = True
        End If
        EditToolBar()
        DisplayAssignTaskTo()
        DisplayMailFieldSelectors(form_data)
    End Sub
    Private Sub Process_DoUpdate()
        Dim strFormOutput As String = ""
        Try
            m_intFormId = Request.Form(frm_form_id.UniqueID)
            pagedata = New Collection
            pagedata.Add(m_intFormId, "FormID")
            pagedata.Add(Request.Form(frm_form_title.UniqueID), "FormTitle")
            pagedata.Add(m_intFolderId, "FolderID")
            pagedata.Add(Request.Form(frm_form_description.UniqueID), "FormDescription")
            If (Request.Form(frm_form_mail.UniqueID) <> "" And Request.Form(frm_form_db.UniqueID) <> "") Then
                strFormOutput = "both"
            ElseIf (Request.Form(frm_form_mail.UniqueID) <> "") Then
                strFormOutput = Request.Form(frm_form_mail.UniqueID)
            Else
                strFormOutput = Request.Form(frm_form_db.UniqueID)
            End If
            If Request.Form(frm_form_limit_submission.UniqueID) = "on" Then
                If Len(Request.Form(frm_form_number_of_submission.UniqueID)) AndAlso IsNumeric(Request.Form(frm_form_number_of_submission.UniqueID)) Then
                    strFormOutput = strFormOutput & ",$" & Request.Form(frm_form_number_of_submission.UniqueID)
                Else
                    strFormOutput = strFormOutput & ",$1" 'default to 1
                End If
            End If
            pagedata.Add(strFormOutput, "FormOutput")
            pagedata.Add((Request.Form(frm_form_af.UniqueID) <> ""), "Autofill")
            pagedata.Add(Request.Form(frm_form_mailto.UniqueID), "MailTo")
            pagedata.Add(Request.Form(frm_form_mailcc.UniqueID), "MailCc")
            pagedata.Add(Request.Form(frm_form_mailfrom.UniqueID), "MailFrom")
            pagedata.Add(Request.Form(frm_form_mailsubject.UniqueID), "MailSubject")
            pagedata.Add(Request.Form(frm_form_mailpreamble.UniqueID), "MailPreamble")
            If (Request.Form(frm_send_xml_packet.UniqueID) <> "") Then
                pagedata.Add(1, "SendXmlPacket")
            Else
                pagedata.Add(0, "SendXmlPacket")
            End If

            pagedata.Add(Request.Form(assigned_to_user_id.UniqueID), "AssignTaskToUser")
            pagedata.Add(Request.Form(assigned_to_usergroup_id.UniqueID), "AssignTaskToUserGroup")

            m_refModule.UpdateFormInfo(pagedata) 'ret
            Response.Redirect("cmsform.aspx?folder_id=" & Request.Form(frm_folder_id.UniqueID) & "&LangType=" & ContentLanguage & "&Action=ViewForm&form_id=" & m_intFormId, False)
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub
    Private Sub EditToolBar()
        Dim result As System.Text.StringBuilder
        Dim callBackPage As String = m_refStyle.getCallBackupPage("cmsform.aspx?LangType=" & ContentLanguage & "&folder_id=" & form_data.FolderId & "&action=ViewAllForms")
        result = New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar("Edit Form" & " """ & form_data.Title & """")
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", "Click here to save this Form", m_refMsg.GetMessage("btn save"), "Onclick=""javascript:return SubmitForm('myform', 'VerifyAddForm()');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", callBackPage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

#End Region

    Private Sub DisplayAssignTaskTo()
        Try
            Dim sbHtml As New System.Text.StringBuilder
            Dim bUnassigned As Boolean = False
            Dim security_task_data As PermissionData
            security_task_data = m_refContentApi.LoadPermissions(m_intFormId, "tasks", ContentAPI.PermissionResultType.Task)
            Dim objTasks As Ektron.Cms.Content.EkTasks
            Dim cTask As Ektron.Cms.content.EkTask
            cTask = m_refContentApi.EkTaskRef

            cTask.AssignedByUserID = CurrentUserId
            cTask.AssignedToUserID = -1
            cTask.AssignToUserGroupID = -1
            cTask.ContentID = m_intFormId
            cTask.ContentLanguage = ContentLanguage
            If m_intFormId > 0 Then
                ' Existing form
                objTasks = cTask.GetTasks(m_intFormId, TaskState.Prototype, -1, CMSTaskItemType.TasksByStateAndContentID)
                If Not IsNothing(objTasks) AndAlso objTasks.Count > 0 Then
                    cTask = objTasks.Item(1)
                End If
            End If

            content_id.Value = m_intFormId
            current_language.Value = ContentLanguage
            assigned_to_user_id.Value = ""
            If cTask.AssignedToUserID > -1 Then
                assigned_to_user_id.Value = cTask.AssignedToUserID
            End If
            assigned_to_usergroup_id.Value = ""
            If cTask.AssignToUserGroupID > -1 Then
                assigned_to_usergroup_id.Value = cTask.AssignToUserGroupID
            End If
            current_user_id.Value = CurrentUserId
            assigned_by_user_id.Value = CurrentUserId


            sbHtml.Append("<span id=""user"" style=""display:inline;"">")
            If (cTask.AssignToUserGroupID = 0) Then
                sbHtml.Append("All Authors")
            ElseIf (cTask.AssignedToUser <> "") Then
                sbHtml.Append("<img src=""" & AppImgPath & "../UI/Icons/user.png"" align=""absbottom"">" & Replace(cTask.AssignedToUser, "'", "`"))
            ElseIf (cTask.AssignedToUserGroup <> "") Then
                sbHtml.Append("<img src=""" & AppImgPath & "../UI/Icons/user.png"" align=""absbottom"">" & Replace(cTask.AssignedToUserGroup, "'", "`"))
            Else
                sbHtml.Append(m_refMsg.GetMessage("lbl unassigned"))
                bUnassigned = True
            End If
            sbHtml.Append("</span>")
            sbHtml.Append("&nbsp;")
            If security_task_data.CanRedirectTask Then
                sbHtml.Append("<a href=""#"" onclick=""javascript:ShowUsers();"" class='selusers'>" & m_refMsg.GetMessage("select user email report") & " </a>")
            End If
            sbHtml.Append("<span id=""idUnassignTask"" style=""display:" & IIf(bUnassigned, "none", "inline") & """>&#160;&#160;&#160;<a href=""#"" onclick=""unassignTask();return false;"">" & m_refMsg.GetMessage("lbl unassign") & "</a></span>")
            sbHtml.Append("<div id=""nouser"" style=""display:none;""></div>")
            AssignTaskTo.Text = sbHtml.ToString
        Catch ex As Exception
            EkException.ThrowException(ex)
        End Try
    End Sub

    Private Sub DisplayMailFieldSelectors(ByVal FormInfo As FormData)
        Try
            Dim strSelector As String
            Dim strFieldList As String
            ' Use non-breaking spaces (&#160;) to prevent ugly wrapping
            strFieldList = m_refModule.GetFormFieldListXml(FormInfo.Id)
            If Len(strFieldList) > 0 Then
                strSelector = CreateSelectList(strFieldList, frm_form_mailto, ExtractFieldName(FormInfo.MailTo), DataTypes:="email emailList")
                If Len(strSelector) > 0 Then
                    litMailTo.Text = " OR&#160;to&#160;addresses&#160;in&#160;field:&#160;" & strSelector
                End If
                strSelector = CreateSelectList(strFieldList, frm_form_mailfrom, ExtractFieldName(FormInfo.MailFrom), DataTypes:="email")
                If Len(strSelector) > 0 Then
                    litMailFrom.Text = " OR&#160;from&#160;address&#160;in&#160;field:&#160;" & strSelector
                End If
                strSelector = CreateSelectList(strFieldList, frm_form_mailcc, ExtractFieldName(FormInfo.MailCc), DataTypes:="email emailList")
                If Len(strSelector) > 0 Then
                    litMailCC.Text = " OR&#160;to&#160;addresses&#160;in&#160;field:&#160;" & strSelector
                End If
                strSelector = CreateSelectList(strFieldList, frm_form_mailsubject, ExtractFieldName(FormInfo.MailSubject), BaseTypes:="text")
                If Len(strSelector) > 0 Then
                    litMailSubject.Text = " OR&#160;use&#160;text&#160;in&#160;field:&#160;" & strSelector
                End If
                strSelector = CreateSelectList(strFieldList, frm_form_mailpreamble, ExtractFieldName(FormInfo.MailPreamble), BaseTypes:="text textbox")
                If Len(strSelector) > 0 Then
                    litMailMessageBody.Text = " OR&#160;use&#160;text&#160;in&#160;field:&#160;" & strSelector
                End If
            Else
                litMailTo.Text = ""
                litMailFrom.Text = ""
                litMailCC.Text = ""
                litMailSubject.Text = ""
                litMailMessageBody.Text = ""
            End If
        Catch ex As Exception
            EkException.ThrowException(ex)
        End Try
    End Sub

    ' Duplicate copy in EkModule.vb, editform.ascx.vb. Copied to keep it private.
    Private Function ExtractFieldName(ByVal Text As String) As String
        ' Return empty string if not a field name
        ' 171 = left (double) angle quote (guillemet)
        ' 187 = right (double) angle quote (guillemet)
        If IsNothing(Text) Then Return ""
        If Text.StartsWith(Chr(171)) And Text.EndsWith(Chr(187)) Then
            Return Text.Substring(1, Text.Length - 2)
        Else
            Return ""
        End If
    End Function

    Private Function CreateSelectList(ByVal FieldListXml As String, ByVal AssociatedControl As HtmlInputText, Optional ByVal CurrentValue As String = "", Optional ByVal DataTypes As String = "", Optional ByVal BaseTypes As String = "") As String
        Dim strSelectList As String = ""
        Dim strOptionList As String
        Dim strFormsPath As String
        Dim strXsltFilePath As String
        Try
            strFormsPath = Server.MapPath(m_refContentApi.AppPath) & "controls\forms\"
            strXsltFilePath = Utilities.QualifyURL(strFormsPath, "SelectFormField.xslt")

            Dim objXsltArgs As System.Xml.Xsl.XsltArgumentList = Nothing
            If Len(CurrentValue) > 0 Then
                If IsNothing(objXsltArgs) Then
                    objXsltArgs = New System.Xml.Xsl.XsltArgumentList
                End If
                objXsltArgs.AddParam("value", String.Empty, CurrentValue)
            End If
            If Len(DataTypes) > 0 Then
                If IsNothing(objXsltArgs) Then
                    objXsltArgs = New System.Xml.Xsl.XsltArgumentList
                End If
                objXsltArgs.AddParam("datatypes", String.Empty, DataTypes)
            End If
            If Len(BaseTypes) > 0 Then
                If IsNothing(objXsltArgs) Then
                    objXsltArgs = New System.Xml.Xsl.XsltArgumentList
                End If
                objXsltArgs.AddParam("basetypes", String.Empty, BaseTypes)
            End If
            strOptionList = m_refModule.XSLTransform(FieldListXml, strXsltFilePath, XsltAsFile:=True, XsltArgs:=objXsltArgs)
            If Len(strOptionList) > 0 Then
                ' useFieldValue depends on "_sel"
                strSelectList = strSelectList & "<select name=""" & AssociatedControl.Name & "_sel"" id=""" & AssociatedControl.ClientID & "_sel"" onchange=""useFieldValue(this)"">" & vbCrLf
                strSelectList = strSelectList & "<option value="""">" & "(No field selected)" & "</option>" & vbCrLf
                strSelectList = strSelectList & strOptionList & vbCrLf
                strSelectList = strSelectList & "</select>" & vbCrLf
            End If
        Catch ex As Exception
            EkException.ThrowException(ex)
        End Try
        Return strSelectList
    End Function

    Private Function StringToInt(ByVal strValue As String, Optional ByVal DefaultValue As Integer = 0) As Integer
        Try
            Return CInt(strValue)
        Catch ex As Exception
            Return DefaultValue
        End Try
    End Function
    Protected Sub SetJSResourceStrings()
        ltr_valemailaddr.text = m_refMsg.GetMessage("enter valid email address or leave blank")
    End Sub
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
End Class
