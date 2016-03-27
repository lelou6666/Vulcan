Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.API
Imports Microsoft.Security.Application

Partial Class viewconfiguration
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
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected AppName As String = ""
    Protected SITEPATH As String = ""
    Private m_bReturn As Boolean = False
    Protected VerifyTrue As String = ""
    Protected VerifyFalse As String = ""
    Protected m_SelectedEditControl As String = ""
    Protected m_refUserApi As New UserAPI
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RegisterResources()

        Dim m_refSiteApi As New SiteAPI
        m_refMsg = m_refSiteApi.EkMsgRef
        Dim settings_data As SettingsData
        Dim user_data As UserData
        Dim group_data As UserGroupData()
        Dim preference_data As UserPreferenceData
        AppImgPath = m_refSiteApi.AppImgPath
        AppName = m_refSiteApi.AppName
        SITEPATH = m_refSiteApi.SitePath
        user_data = m_refUserApi.GetUserById(BuiltIn)
        preference_data = m_refUserApi.GetUserPreferenceById(0)
        group_data = m_refUserApi.GetAllUserGroups("GroupName")
        VerifyTrue = "<img src=""" & AppImgPath & "../UI/Icons/check.png"" border=""0"" alt=""Item is Enabled"" title=""Item is Enabled"">"
        VerifyFalse = "<img src=""" & AppImgPath & "icon_redx.gif"" border=""0"" alt=""Item is Disabled"" title=""Item is Disabled"">"

        settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId, True)
        'VERSION
        td_version.InnerHtml = m_refMsg.GetMessage("version") & "&nbsp;" & m_refSiteApi.Version & "&nbsp;" & m_refSiteApi.ServicePack
        'BUILD NUMBER
        td_buildnumber.InnerHtml = "<i>(" & m_refMsg.GetMessage("build") & m_refSiteApi.BuildNumber & ")</i>"

        'Which Editor
        m_SelectedEditControl = Utilities.GetEditorPreference(Request)

        Dim language_data As LanguageData
        language_data = m_refSiteApi.GetLanguageById(m_refSiteApi.DefaultContentLanguage)
        td_Language.InnerHtml = language_data.Name
        'LICENSE

        If (settings_data.LicenseKey.Length > 0) Then
            td_licensekey.InnerHtml = settings_data.LicenseKey
        Else
            td_licensekey.InnerHtml = m_refMsg.GetMessage("none specified msg")
        End If

        'MODULE LICENSE
        Dim module_text As New System.Text.StringBuilder
        Dim i As Integer = 0
        If (Not (IsNothing(settings_data.ModuleLicense))) Then            
            For i = 0 To settings_data.ModuleLicense.Length - 1
                module_text.Append(i + 1 & "." & settings_data.ModuleLicense(i).License)
                module_text.Append("<br/>")
            Next
        Else
            module_text.Append(m_refMsg.GetMessage("none specified msg"))
        End If

        td_modulelicense.InnerHtml = module_text.ToString
        'LANGUAGE LIST
        Dim active_lang_list As LanguageData()
        active_lang_list = m_refSiteApi.GetAllActiveLanguages()

        td_languagelist.InnerHtml = m_refMsg.GetMessage("none specified msg")
        If (Not (IsNothing(active_lang_list))) Then
            For i = 0 To active_lang_list.Length - 1
                If (Convert.ToString(active_lang_list(i).Id) = settings_data.Language) Then
                    td_languagelist.InnerHtml = active_lang_list(i).Name
                    Exit For
                End If
            Next
        End If


        'MAX CONTENT SIZE

        'td_maxcontent.InnerHtml = settings_data.MaxContentSize

        'MAX SUMMARY SIZE

        td_maxsummary.InnerHtml = settings_data.MaxSummarySize


        'SYSTEM EMAIL


        If (settings_data.Email.Length > 0) Then
            td_email.InnerHtml = settings_data.Email
        Else
            td_email.InnerHtml = m_refMsg.GetMessage("none specified msg")
        End If

        'EMAIL NOTIFICATION

        If (settings_data.EnableMessaging) Then
            td_email_msg.InnerHtml = m_refMsg.GetMessage("sending email enabled msg")
        Else
            td_email_msg.InnerHtml = m_refMsg.GetMessage("sending email disabled msg")
        End If
		
		'Server Type
        td_server_type.InnerHtml += GetCheckValue(settings_data.AsynchronousStaging) & "&nbsp;" & m_refMsg.GetMessage("lbl enable server type message")


        'Asyncronous Processor Location

        If Not (settings_data.AsynchronousLocation Is Nothing) AndAlso (settings_data.AsynchronousLocation.Length > 0) Then
            td_asynch_location.InnerHtml = settings_data.AsynchronousLocation
        Else
            td_asynch_location.InnerHtml = m_refMsg.GetMessage("none specified msg")
        End If
       
        'PUBPDF
        trPublishPDF.Visible = settings_data.PublishPdfSupported
        td_publish_pdf.InnerHtml += GetCheckValue(settings_data.PublishPdfEnabled) & "&nbsp;" & m_refMsg.GetMessage("alt Enable office documents to be published in other format")

        'LIBRARY FOLDER CREATION
        td_libfolder.InnerHtml = GetCheckValue(settings_data.FileSystemSupport) & "&nbsp;"
        td_libfolder.InnerHtml += m_refMsg.GetMessage("library filesystem folder prompt") & "&nbsp;"

        'BUILT IN USER
        td_user.InnerHtml = user_data.Username
        'td_removestyle.InnerHtml = GetCheckValue(settings_data.RemoveStyles) & "&nbsp;" & m_refMsg.GetMessage("remove styles")
        td_enablefont.InnerHtml = GetCheckValue(settings_data.EnableFontButtons) & "&nbsp;" & m_refMsg.GetMessage("enable font buttons") & "&nbsp;"
        td_wordstyle.InnerHtml = GetCheckValue(settings_data.PreserveWordStyles) & "&nbsp;" & m_refMsg.GetMessage("preserve word styles")
        td_wordclass.InnerHtml = GetCheckValue(settings_data.PreserveWordClasses) & "&nbsp;" & m_refMsg.GetMessage("preserve word classes")
        td_access.InnerHtml = GetAccessibilityValue(settings_data.Accessibility)


        If preference_data.Template = "" Then
            td_template.InnerHtml = m_refMsg.GetMessage("refresh login page msg")
        Else
            td_template.InnerHtml = SITEPATH & AntiXss.HtmlEncode(preference_data.Template)
        End If

        td_folder.InnerHtml = "<input type=""checkbox"" disabled "
        If (Convert.ToString(preference_data.FolderId) = "") Then
            td_folder.InnerHtml += " checked "
        End If
        td_folder.InnerHtml += " id=""checkbox"" name=""chkSmartDexktop"">"
        td_force.InnerHtml = "<input type=""checkbox"" disabled "
        If (preference_data.ForceSetting) Then
            td_force.InnerHtml += " checked "
        End If
        td_force.InnerHtml += " id=""Checkbox1"" name=""forcePrefs"">"
        td_titletext.InnerHtml = "<input type=""checkbox"" id=""disptitletext"" disabled "
        If (preference_data.DisplayTitleText) Then
            td_titletext.InnerHtml += " checked "
        End If
        td_titletext.InnerHtml += "name=""disptitletext"">"
        'td_height.InnerHtml = preference_data.Height & "px"
        'td_width.InnerHtml = preference_data.Width & "px"

        td_verify_user_on_add.InnerHtml = "<input type=""checkbox"" disabled "
        If (settings_data.VerifyUserOnAdd) Then
            td_verify_user_on_add.InnerHtml += " checked "
        End If
        td_verify_user_on_add.InnerHtml += " id=""chkVerifyUserOnAdd"" name=""chkVerifyUserOnAdd"" >"

        td_enable_preapproval.InnerHtml = "<input type=""checkbox"" disabled "
        If (settings_data.EnablePreApproval) Then
            td_enable_preapproval.InnerHtml += " checked "
        End If
        td_enable_preapproval.InnerHtml += " id=""chkEnablePreapproval"" name=""chkEnablePreapproval"" >"

    End Sub

    Private Sub RegisterResources()
        'CSS
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronUITabsCss)

        'JS
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
    End Sub

    Private Function GetCheckValue(ByVal value As Boolean) As String
        If (value) Then
            Return (VerifyTrue)
        Else
            Return (VerifyFalse)
        End If
    End Function

	Private Function GetAccessibilityValue(ByVal value As String) As String
		Dim sAccessibility As String = ""
		Select Case (value)
			Case 1 'loose
				sAccessibility = m_refMsg.GetMessage("access loose label")
			Case 2 'strict
				sAccessibility = m_refMsg.GetMessage("access strict label")
			Case Else 'value = 0 or null
				sAccessibility = m_refMsg.GetMessage("access none label")
		End Select
		Return (VerifyTrue & " " & sAccessibility)
	End Function

    Protected Sub ShutDownClick_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ShutDownClick.Click
        Dim message As String = String.Empty
        Dim ex As Exception

        message = "--- Application Restarted ---" & vbCrLf
        message = message + "Host: " & Request.Url.Host & vbCrLf
        message = message + "Time: " & Date.Now.ToString() & vbCrLf
        message = message + "UserID: " & m_refUserApi.UserId.ToString() & vbCrLf
        message = message + "Username: " & m_refUserApi.RequestInformationRef.LoggedInUsername
        ex = New Exception(message)
        EkException.LogException(ex, Diagnostics.EventLogEntryType.Error)
        HttpRuntime.UnloadAppDomain()
    End Sub
End Class
