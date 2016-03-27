Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class editconfiguration
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
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected AppName As String = ""
    Protected SITEPATH As String = ""
    Private m_bReturn As Boolean = False
    Protected VerifyTrue As String = ""
    Protected VerifyFalse As String = ""
    Protected m_SelectedEditControl As String = ""
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RegisterResources()
        jsUniqueID.Text = "editconfiguration"
        m_refMsg = (New CommonApi).EkMsgRef
    End Sub

    Private Sub RegisterResources()
        'CSS
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)

        'JS
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
    End Sub

    Private Function DisplayEditConfiguration() As Boolean
        Dim m_refSiteApi As New SiteAPI
        Dim m_refUserApi As New UserAPI
        Dim settings_data As SettingsData
        Dim user_data As UserData
        Dim group_data As UserGroupData()
        Dim preference_data As UserPreferenceData

        Try
            AppImgPath = m_refSiteApi.AppImgPath
            AppName = m_refSiteApi.AppName
            AppPath = m_refSiteApi.AppPath
            SITEPATH = m_refSiteApi.SitePath
            user_data = m_refUserApi.GetUserById(BuiltIn)
            preference_data = m_refUserApi.GetUserPreferenceById(0)
            group_data = m_refUserApi.GetAllUserGroups("GroupName")
            VerifyTrue = "<img src=""" & AppPath & "images/UI/Icons/check.png"" border=""0"" alt=""Item is Enabled"" title=""Item is Enabled"">"
            VerifyFalse = "<img src=""" & AppImgPath & "icon_redx.gif"" border=""0"" alt=""Item is Disabled"" title=""Item is Disabled"">"
            settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId, True)
            jsContentLanguage.Text = Convert.ToString(settings_data.Language)
            'VERSION
            td_version.InnerHtml = m_refMsg.GetMessage("version") & m_refSiteApi.Version & "&nbsp;" & m_refSiteApi.ServicePack
            'BUILD NUMBER
            td_buildnumber.InnerHtml = "<i>(" & m_refMsg.GetMessage("build") & m_refSiteApi.BuildNumber & ")</i>"
     
            'Which Editor
            m_SelectedEditControl = Utilities.GetEditorPreference(Request)

            'LICENSE
            td_licensekey.InnerHtml = "<input type=""text"" maxlength=""4000"" name=""license"" value=""" & settings_data.LicenseKey & """ />"
            td_licensekey.InnerHtml += "<input type=""hidden"" maxlength=""4000"" name=""license1"" value=""" & settings_data.LicenseKey & """ />"
            td_licensekey.InnerHtml += "<br/>"
            td_licensekey.InnerHtml += "<span class=""ektronCaption"">" + m_refMsg.GetMessage("license key help text") + "</span>"
            'MODULE LICENSE
            Dim module_text As New System.Text.StringBuilder
            Dim i As Integer = 0
            If (Not (IsNothing(settings_data.ModuleLicense))) Then
                For i = 0 To settings_data.ModuleLicense.Length - 1
                    If (i > 0) Then
                        module_text.Append("<div class=""ektronTopSpaceSmall""></div>")
                    End If
                    module_text.Append(i + 1 & ". ")
                    module_text.Append("<input type=""text"" maxlength=""4000"" name=""mlicense" & i + 1 & """ value=""" & settings_data.ModuleLicense(i).License & """>" & vbCrLf)
                    module_text.Append("<input type=""hidden"" name=""mlicenseid" & i + 1 & """ value=""" & settings_data.ModuleLicense(i).Id & """>" & vbCrLf)
                Next
            End If
            module_text.Append("<div class=""ektronTopSpaceSmall""></div>")
            module_text.Append(i + 1 & ". ")
            module_text.Append("<input type=""text"" maxlength=""4000"" name=""mlicense" & (i + 1) & """ value="""">" & vbCrLf)
            module_text.Append("<input type=""hidden"" name=""mlicenseid" & (i + 1) & """ value=""0"">" & vbCrLf)            

            td_modulelicense.InnerHtml = module_text.ToString
            'LANGUAGE LIST
            Dim active_lang_list As LanguageData()
            active_lang_list = m_refSiteApi.GetAllActiveLanguages()

            td_languagelist.InnerHtml = "<select id=""language"" name=""language"" selectedindex=""0"">"
            If (Not (IsNothing(active_lang_list))) Then
                For i = 0 To active_lang_list.Length - 1
                    'If (Convert.ToString(active_lang_list(i).Id) = settings_data.Language) Then
                    td_languagelist.InnerHtml += "<option  value=""" & active_lang_list(i).Id & """ "
                    If (Convert.ToString(active_lang_list(i).Id) = settings_data.Language) Then
                        td_languagelist.InnerHtml += " selected"
                    End If
                    td_languagelist.InnerHtml += "> " & active_lang_list(i).Name & "</option>"
                    'End If
                Next
            End If
            td_languagelist.InnerHtml += "</select>"

            'MAX CONTENT SIZE

           ' td_maxcontent.InnerHtml = "<input type=""Text"" maxlength=""9"" size=""9"" name=""content_size"" value=""" & settings_data.MaxContentSize & """>"
           ' td_maxcontent.InnerHtml += "<br/>"
           ' td_maxcontent.InnerHtml += "<span class=""ektronCaption"">" + m_refMsg.GetMessage("settings max content help text") + "</span>"

            'MAX SUMMARY SIZE

            td_maxsummary.InnerHtml = "<input type=""Text"" maxlength=""9"" size=""9"" name=""summary_size"" value=""" & settings_data.MaxSummarySize & """>"
            td_maxsummary.InnerHtml += "<br/>"
            td_maxsummary.InnerHtml += "<span class=""ektronCaption"">" + m_refMsg.GetMessage("settings max summary help text") + "</span>"


            'SYSTEM EMAIL

            td_email.InnerHtml = "<input type=""Text"" maxlength=""50"" size=""25"" name=""SystemEmaillAddr"" value=""" & settings_data.Email & """>"
            td_email.InnerHtml &= "<div class=""ektronCaption"">"

            'EMAIL NOTIFICATION

            If (settings_data.EnableMessaging) Then
                td_email.InnerHtml &= "<input type=""CHECKBOX"" name=""EnableMessaging"" value=""enable_msg"" CHECKED=""True"">"
            Else
                td_email.InnerHtml &= "<input type=""CHECKBOX"" name=""EnableMessaging"" value=""enable_msg"">"
            End If
            td_email.InnerHtml += m_refMsg.GetMessage("enable mail messages") & "&nbsp;"
            td_email.InnerHtml += "</div>"
			
			'Server Type
            If (settings_data.AsynchronousStaging) Then
                td_server_type.InnerHtml &= "<input type=""CHECKBOX"" name=""SystemAsynchStaging"" value=""enable_msg"" CHECKED=""True"">"
            Else
                td_server_type.InnerHtml = "<input type=""CHECKBOX"" name=""SystemAsynchStaging"" value=""enable_msg"" >"
            End If
            td_server_type.InnerHtml += "Staging Server" & "&nbsp;"
            td_server_type.InnerHtml += "<br/>"
            td_server_type.InnerHtml += "<span class=""ektronCaption"">" + m_refMsg.GetMessage("lbl enable server type message") + "</span>"


            'Asyncronous Processor Location

            If Not (settings_data.AsynchronousLocation Is Nothing) Then
                ' OldSystemAsynchLocation is needed because a disabled input field is not posted
                td_asynch_location.InnerHtml = "<input type=""Hidden"" name=""OldSystemAsynchLocation"" value=""" & settings_data.AsynchronousLocation & """>"
                td_asynch_location.InnerHtml &= "<input type=""Text"" maxlength=""255"" name=""SystemAsynchLocation"" value=""" & settings_data.AsynchronousLocation & """"
            Else
                td_asynch_location.InnerHtml = "<input type=""Text"" maxlength=""255"" name=""SystemAsynchLocation"" value="""""
            End If
            td_asynch_location.InnerHtml &= ">"
			td_asynch_location.InnerHtml &= "<br/>"
            td_asynch_location.InnerHtml &= "<span class=""ektronCaption"">"
            td_asynch_location.InnerHtml &= m_refMsg.GetMessage("lbl Example Location") & " http://localhost/CMS400Developer/Workarea/webservices/EktronAsyncProcessorWS.asmx"
            td_asynch_location.InnerHtml &= "</span>"
			
            'PUBLISHPDF and PUBLISHHTML would check this flag.
            If (settings_data.PublishPdfSupported) Then
                Dim schk As String = ""
                If (settings_data.PublishPdfEnabled) Then
                    schk = " checked "
                End If
                PubPdf.Text = "<tr>"
                PubPdf.Text += "<td class=""label shortLabel"">"
                PubPdf.Text += "<label for=""PublishPdfEnabled"">" & m_refMsg.GetMessage("alt Enable office documents to be published in other format") & ":" & "</label>"
                PubPdf.Text += "</td>"
                PubPdf.Text += "<td class=""value"">"
                PubPdf.Text += "<input type=""checkbox"" name=""PublishPdfEnabled"" id=""PublishPdfEnabled""" & schk & " >"
                PubPdf.Text += "</td>"
                PubPdf.Text += "</tr>"
            Else
                PubPdf.Visible = False
            End If
            'LIBRARY FOLDER CREATION

            If (settings_data.FileSystemSupport) Then
                td_libfolder.InnerHtml = "<input type=""CHECKBOX"" name=""filesystemsupport"" value=""enable_msg"" CHECKED=""True"" Onclick=""javascript:return AreYouSure();"">"
            Else
                td_libfolder.InnerHtml = "<input type=""CHECKBOX"" name=""filesystemsupport"" value=""enable_msg"" Onclick=""javascript: return AreYouSure()"">"
            End If

            'BUILT IN USER
            'Dim x As System.Web.UI.WebControls.TextBox

            userid.Value = user_data.Id
            username.Value = user_data.Username
            TD_Pwd.InnerHtml = "<input type=""password"" value=""" & user_data.Password & """ id=""pwd"" name=""pwd"" maxlength=""50"" Onkeypress=""javascript:return CheckKeyValue(event,'34');"">"
            TD_Pwd2.InnerHtml = "<input type=""password"" value=""" & user_data.Password & """ id=""confirmpwd"" name=""confirmpwd"" maxlength=""50"" Onkeypress=""javascript:return CheckKeyValue(event,'34');"">"
            If ((m_refUserApi.RequestInformationRef.LoginAttempts <> -1) And (m_refUserApi.IsAdmin OrElse m_refUserApi.UserId = 999999999 OrElse (m_refUserApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers)))) Then
                accountLocked.Text = "<input type=""checkbox"" id=""chkAccountLocked"" name=""chkAccountLocked"" "
                If (user_data.IsAccountLocked(m_refUserApi.RequestInformationRef)) Then
                    accountLocked.Text += " checked "
                End If
                accountLocked.Text += " />"
            Else
                accountLocked.Text = "<input type=""hidden"" id=""chkAccountLocked"" name=""chkAccountLocked"" "
                If (user_data.IsAccountLocked(m_refUserApi.RequestInformationRef)) Then
                    accountLocked.Text += " value=""on"" />"
                Else
                    accountLocked.Text += " value="""" />"
                End If
            End If
			'styles.Items.Add(New ListItem(m_refMsg.GetMessage("remove styles"), "remove"))
			'styles.Items.Add(New ListItem(m_refMsg.GetMessage("do not remove styles"), ""))
			'If (settings_data.RemoveStyles) Then
			'    styles.Items(0).Selected = True
			'Else
			'    styles.Items(1).Selected = True
			'End If
			'styles.Attributes.Add("onClick", "javascript:checkWordStlyes();")
			'If (settings_data.RemoveStyles) Then
			'    'jsRemoveStyle = "1"
			'End If

            If (settings_data.EnableFontButtons) Then
                font_style.Checked = True
            End If
			If (settings_data.PreserveWordStyles) Then
				word_styles.Checked = True
			End If
            If (settings_data.PreserveWordClasses) Then
                word_classes.Checked = True
            End If

            If preference_data.Template <> "" Then
                templatefilename.Value = preference_data.Template
            End If
            If (Convert.ToString(preference_data.FolderId) = "") Then
                chkSmartDesktop.Checked = True
            End If
            folderId.Value = preference_data.FolderId
            If (preference_data.ForceSetting) Then
                forcePrefs.Checked = True
            End If
            If (preference_data.DisplayTitleText) Then
                disptitletext.Checked = True
            End If
            'txtHeight.Value = Convert.ToString(preference_data.Height)
            'txtWidth.Value = Convert.ToString(preference_data.Width)

            If (settings_data.VerifyUserOnAdd) Then
                chkVerifyUserOnAdd.Checked = True
            End If

			If (settings_data.EnablePreApproval) Then
				chkEnablePreApproval.Checked = True
			End If

			access_def.Value = settings_data.Accessibility

			Return (False)
        Catch ex As Exception
        End Try
    End Function
    Private Function GetCheckValue(ByVal value As Boolean) As String
        If (value) Then
            Return (VerifyTrue)
        Else
            Return (VerifyFalse)
        End If
    End Function
    Public Function EditConfigurationControl() As Boolean
        Dim result As Boolean = False
        Me.ID = "editconfiguration"
        If (Not (IsPostBack)) Then

            result = DisplayEditConfiguration()
        Else
            result = ProcessSubmission()
        End If
        Return (result)
    End Function
    Public Function ProcessSubmission() As Boolean
        Dim m_refSiteApi As New SiteAPI
        Dim pagedata As Hashtable
        Dim modulelicense As Hashtable
        Dim modulelicenses As New Hashtable
        Dim prefs As New Collection
        Dim FolderId As Long
        Dim i As Integer = 0

        pagedata = New Hashtable

        Do While (Len(Request.Form("mlicenseid" & i + 1)) > 0)
            modulelicense = New Hashtable
            modulelicense.Add("ID", CLng(Request.Form("mlicenseid" & i + 1)))
            modulelicense.Add("License", Request.Form("mlicense" & i + 1))
            modulelicense.Add("Type", 0)
            modulelicenses.Add(i, modulelicense)
            i = i + 1
            modulelicense = Nothing
        Loop
        pagedata.Add("ModuleLicenses", modulelicenses)
        pagedata.Add("LicenseKey", Request.Form("license"))
        pagedata.Add("AppLanguage", Request.Form("language"))
        pagedata.Add("SystemEmail", Request.Form("SystemEmaillAddr"))
        If (Len(Request.Form("SystemAsynchStaging"))) Then
            pagedata.Add("AsynchStaging", 1)
            m_refSiteApi.RequestInformationRef.IsStaging = True
        Else
            pagedata.Add("AsynchStaging", 0)
            m_refSiteApi.RequestInformationRef.IsStaging = False
        End If
		If (Len(Request.Form("SystemAsynchLocation"))) Then
            pagedata.Add("AsynchLocation", Request.Form("SystemAsynchLocation"))
		Else
			pagedata.Add("AsynchLocation", Request.Form("SystemAsynchLocation"))
        End If
        'pagedata.Add("MaxContentSize", String.Empty)
        pagedata.Add("MaxSummarySize", Request.Form("summary_size"))
        If (Request.Form("EnableMessaging") <> "") Then
            pagedata.Add("EnableMessaging", 1)
        Else
            pagedata.Add("EnableMessaging", 0)
        End If
        If (Request.Form("filesystemsupport") <> "") Then
            pagedata.Add("FileSystemSupport", 1)
        Else
            pagedata.Add("FileSystemSupport", 0)
        End If
        If (Not (IsNothing(Request.Form("PublishPdfEnabled")))) Then
            If (Request.Form("PublishPdfEnabled") <> "") Then
                pagedata.Add("PublishPdfEnabled", 1)
            Else
                pagedata.Add("PublishPdfEnabled", 0)
            End If
        Else
            pagedata.Add("PublishPdfEnabled", 0)
        End If
        If (Len(Request.Form(font_style.UniqueID))) Then
            pagedata.Add("EnableFontButtons", 1)
        Else
            pagedata.Add("EnableFontButtons", 0)
        End If
		'If (Len(Request.Form(styles.UniqueID))) Then
		'    pagedata.Add("RemoveStyles", 1)
		'Else
		'    pagedata.Add("RemoveStyles", 0)
		'End If
		If (Len(Request.Form(word_styles.UniqueID))) Then
			pagedata.Add("PreserveWordStyles", 1)
		Else
			pagedata.Add("PreserveWordStyles", 0)
		End If
        If (Len(Request.Form(word_classes.UniqueID))) Then
            pagedata.Add("PreserveWordClasses", 1)
        Else
            pagedata.Add("PreserveWordClasses", 0)
        End If
        pagedata.Add("PreApprovalGroup", "0")
        If (Request.Form(chkVerifyUserOnAdd.UniqueID) = "on") Then
            pagedata.Add("VerifyUserOnAdd", "1")
        Else
            pagedata.Add("VerifyUserOnAdd", "0")
        End If
        If (Request.Form(chkEnablePreApproval.UniqueID) = "on") Then
            pagedata.Add("EnablePreApproval", "1")
        Else
            pagedata.Add("EnablePreApproval", "0")
		End If
		If Len(Request.Form("access")) > 0 Then
			pagedata.Add("accessibility", Request.Form("access"))
		Else
			pagedata.Add("accessibility", 0)
		End If

        prefs.Add("9999", "width")
        prefs.Add("9999", "height")
        prefs.Add(Request.Form(templatefilename.UniqueID), "template")
        If (Request.Form(chkSmartDesktop.UniqueID) = "on") Then
            FolderId = -1
        Else
            If Request.Form("folderId") <> "" Then
                FolderId = Request.Form("folderId")
            Else
                FolderId = 0
            End If
        End If
        prefs.Add(FolderId, "folderid")
        If (Request.Form(forcePrefs.UniqueID) = "on") Then
            prefs.Add("1", "forcesetting")
        Else
            prefs.Add("0", "forcesetting")
        End If

        prefs.Add("1", "dispborders")

        If (Request.Form(disptitletext.UniqueID) = "on") Then
            prefs.Add("1", "disptitletext")
        Else
            prefs.Add("0", "disptitletext")
        End If

        m_refSiteApi.UpdateSiteVariables(pagedata)
        Ektron.Cms.DataIO.LicenseManager.LicenseManager.Reset(m_refSiteApi.RequestInformationRef)
        Dim oldUser As New Collection
        oldUser.Add(Request.Form(userid.UniqueID), "UserID")
        oldUser.Add(Request.Form(username.UniqueID), "UserName")
        oldUser.Add(Request.Form("pwd"), "Password")
        oldUser.Add("", "Domain")
        oldUser.Add("BUILTIN", "FirstName")
        oldUser.Add("BUILTIN", "LastName")
        oldUser.Add("0", "Language")
        oldUser.Add("", "EditorOptions")
        oldUser.Add("", "EmailAddr1")
        oldUser.Add("1", "DisableMsg")
        If (Request.Form("chkAccountLocked") <> "") Then
            oldUser.Add(254, "LoginAttempts")
        Else
            oldUser.Add(0, "LoginAttempts")
        End If
        Dim m_refUserApi As New UserAPI
        m_refUserApi.UpdateUser(oldUser)
        m_refUserApi.UpdateUserPreferences(0, prefs)

        Return (True)
    End Function

End Class
