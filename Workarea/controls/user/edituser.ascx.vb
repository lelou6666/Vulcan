Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Notifications
Imports Ektron.Cms.Framework

Partial Class edituser
    Inherits System.Web.UI.UserControl

	Protected ctlEditor As Ektron.ContentDesignerWithValidator
    Protected security_data As PermissionData
    Public setting_data As SettingsData
    Protected m_refMsg As Common.EkMessageHelper
    Protected FromUsers As String = ""
    Protected language_data As LanguageData()
    Protected uId As Long
    Protected user_data As UserData
    Protected template_list As TemplateData()
    Protected m_refSiteApi As New SiteAPI
    Protected m_refContentApi As New ContentAPI
    Protected m_refUserApi As New UserAPI
    Protected m_refStyle As New StyleHelper
    Protected defaultPreferences As UserPreferenceData
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected ContentLanguage As Integer
    Protected m_intGroupType As Integer = 0
    Protected m_intGroupId As Long = 0
    Protected m_sSignature As String = ""

    Protected _refStyle As New StyleHelper
    Protected _notificationPreferenceApi As New Ektron.Cms.Framework.Notifications.NotificationPreference
    Protected preferenceList As System.Collections.Generic.List(Of NotificationPreferenceData)
    Protected prefData As New NotificationPreferenceData
    Protected _notificationAgentApi As New Ektron.Cms.Framework.Notifications.NotificationAgentSetting
    Protected agentList As System.Collections.Generic.List(Of NotificationAgentData)
    Protected agentData As NotificationAgentData
    Protected _activityApi As New Ektron.Cms.Framework.Activity.Activity
    Protected _activityListApi As New Ektron.Cms.Framework.Activity.ActivityType
    Protected activityTypeList As System.Collections.Generic.List(Of Ektron.Cms.Activity.ActivityTypeData)
    Protected _IsCmsUser As Boolean = False
    Protected groupAliasList As String = String.Empty

    ''' <summary>
    ''' Returns true if there are more than one languages enabled for the site.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsSiteMultilingual() As Boolean
        Get
            If (m_refUserApi.EnableMultilingual = 0) Then
                Return False
            End If
            Dim languageEnabledCount As Integer = 0
            For Each lang As LanguageData In languageDataArray
                If lang.SiteEnabled Then languageEnabledCount += 1
                If languageEnabledCount > 1 Then Exit For
            Next

            Return languageEnabledCount > 1
        End Get

    End Property

    Public ReadOnly Property languageDataArray() As LanguageData()
        Get
            If (language_data Is Nothing) Then
                language_data = m_refSiteApi.GetAllActiveLanguages()
            End If

            Return language_data
        End Get

    End Property

    Public Property IsCmsUser() As Boolean
        Get
            Return _IsCmsUser
        End Get
        Set(ByVal value As Boolean)
            _IsCmsUser = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
		ContentLanguage = m_refSiteApi.ContentLanguage
		ctlEditor = LoadControl("../Editor/ContentDesignerWithValidator.ascx")
		ltr_sig.Controls.Add(ctlEditor)
		With ctlEditor
			.ID = "content_html"
			.AllowScripts = False
			.Height = New Unit(200, UnitType.Pixel)
			.Width = New Unit(570, UnitType.Pixel)
			.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Minimal
			.AllowFonts = True
			.ShowHtmlMode = False
		End With
	End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RegisterResources()
        If ((Not IsNothing(Request.QueryString("grouptype"))) AndAlso (Request.QueryString("grouptype") <> "")) Then
            m_intGroupType = Convert.ToInt32(Request.QueryString("grouptype"))
        End If
        If ((Not IsNothing(Request.QueryString("groupid"))) AndAlso (Request.QueryString("groupid") <> "")) Then
            m_intGroupId = Convert.ToInt64(Request.QueryString("groupid"))
        End If
        m_refMsg = m_refContentApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        AppPath = m_refSiteApi.AppPath

        Me.phWorkareaTab.Visible = Me.IsCmsUser
        Me.phWorkareaContent.Visible = Me.IsCmsUser
    End Sub

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        If IsPostBack Then
            m_sSignature = ctlEditor.Content
			' Remove SCRIPT elements to prevent XSS attacks, although ContentDesigner.AllowScripts="false" should prevent from getting this far.
			m_sSignature = Regex.Replace(m_sSignature, "\<script[\w\W]+\<\/script\>", "", RegexOptions.IgnoreCase)
		End If
    End Sub

    Public Function EditUser() As Boolean
        If (Not (Page.IsPostBack)) Then
            Display_EditUser()
            Display_UserCustomProperties()
            EditUserToolBar()
        End If
    End Function

    Private Sub Display_EditUser()
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim i As Integer = 0

        FromUsers = Request.QueryString("FromUsers")
        uId = Request.QueryString("id")
        user_data = m_refUserApi.GetUserById(uId, True)
        template_list = m_refContentApi.GetAllTemplates("TemplateFileName")

        m_refContent = m_refSiteApi.EkContentRef

        security_data = m_refContentApi.LoadPermissions(0, "content")

        language.Text = GetAllLanguageDropDownMarkup("language", m_refMsg.GetMessage("app default msg"))

        If (m_intGroupType = 0) Then

            defaultPreferences = m_refUserApi.GetUserPreferenceById(m_refSiteApi.UserId)
            If (defaultPreferences.FolderPath Is Nothing) Then
                jsPreferenceFolderId.Text = ""
            Else
                jsPreferenceFolderId.Text = Convert.ToString(defaultPreferences.FolderPath).Replace("\", "\\")
            End If
            jsPreferenceWidth.Text = defaultPreferences.Width
            jsPreferenceHeight.Text = defaultPreferences.Height
            jsPreferenceTemplate.Text = defaultPreferences.Template
            jsPreferenceDispTitleTxt.Text = defaultPreferences.DisplayTitleText

            TD_msg.Text += m_refMsg.GetMessage("disable email notifications msg")
            If (security_data.IsAdmin And setting_data.EnableMessaging = False) Then
                enablemsg.Text = "<div class=""ektronTopSpace""></div><label class=""ektronCaption"">" & m_refMsg.GetMessage("application emails disabled msg") & "</label>"
            End If

            If (user_data.IsDisableMessage) Then
                ltr_checkBox.Text = "<input type=""checkbox"" maxlength=""50"" size=""25"" name=""disable_msg"" id=""disable_msg"" value=""disable_msg"" CHECKED=""True"">"
            Else
                ltr_checkBox.Text = "<input type=""checkbox"" name=""disable_msg"" id=""disable_msg"" value=""disable_msg"">"
            End If

            If (user_data.UserPreference.ForceSetting <> True) Then
                If (Not (IsNothing(defaultPreferences))) Then
                    preference.Text = "<tr><td/><td><div class='ektronTopSpace'><a class=""button buttonInlineBlock blueHover buttonDefault"" href=""Javascript://"" onclick=""RestoreDefault();return false;"">" & m_refMsg.GetMessage("lnk restore default") & "</a></div><div class='ektronTopSpace'></div></td></tr>"
                End If
            End If
            folder.Text += "<td class=""wrapText""><label class=""label wrapText"">" & m_refMsg.GetMessage("lbl set smart desktop") & "</label></td><td><input type=""checkbox"""
            If (user_data.UserPreference.ForceSetting) Then
                folder.Text += " disabled "
            End If
            If (user_data.UserPreference.FolderId = "") Then
                folder.Text += " checked "
            End If
            folder.Text += " id=""chkSmartDesktop"" name=""chkSmartDesktop""/>"
            folder.Text += "<input type=""hidden"" name=""OldfolderId"" id=""OldfolderId"" value=""" & user_data.UserPreference.FolderId & """/>"
            folder.Text += "<input type=""hidden"" name=""folderId"" id=""folderId"" value=""" & user_data.UserPreference.FolderId & """/>"
            folder.Text += "</td>"

            If (user_data.UserPreference.ForceSetting) Then
                lockedmsg.Text = "<tr><td class=""important"" colspan=""2"">" & m_refMsg.GetMessage("preferences locked msg") & "</td></tr>"

            End If
            forcemsg.Text = "<td>"
            forcemsg.Text += m_refSiteApi.SitePath & "<input type=""text"""
            If (user_data.UserPreference.ForceSetting = "1") Then
                forcemsg.Text += " disabled "
            End If
            forcemsg.Text += " ID=""templatefilename"" value=""" & user_data.UserPreference.Template & """ size=""35"" class=""minWidth"" name=""templatefilename"" />"
            If (user_data.UserPreference.ForceSetting = False) Then
                forcemsg.Text += "<a class=""button buttonInline greenHover selectContent"" href=""#"" onclick=""LoadChildPage();return true;"">" & _
                    m_refMsg.GetMessage("generic select") & "</a>"
            End If
            forcemsg.Text += "</td>"

            width.Text = "<td class=""label"">" & m_refMsg.GetMessage("lbl imagetool resize width") & "</td><td><input class=""minWidth"" type=""text"" size=""4"" id=""txtWidth"""
            If (user_data.UserPreference.ForceSetting) Then
                width.Text += " disabled "
            End If
            width.Text += " value=""" & user_data.UserPreference.Width & """ name=""txtWidth"">px</td>"

            height.Text = "<td class=""label"">" & m_refMsg.GetMessage("lbl imagetool resize height") & "</td><td><input class=""minWidth"" type=""text"" size=""4"" id=""txtHeight"""
            If (user_data.UserPreference.ForceSetting) Then
                height.Text += " disabled "
            End If
            height.Text += " value=""" & user_data.UserPreference.Height & """ name=""txtHeight"">px</td>"

            disptext.Text = "<td class=""label wrapText"">" & m_refMsg.GetMessage("lbl display button caption") & "</td><td><input type=""checkbox"" id=""chkDispTitleText"""
            If (user_data.UserPreference.DisplayTitleText = "1") Then
                disptext.Text += " checked "
            End If
            If (user_data.UserPreference.ForceSetting) Then
                disptext.Text += " disabled "
            End If
            disptext.Text += " name=""chkDispTitleText""/></td>"
            disptext.Text += " <input type=""hidden"" id=""hdn_pagesize"" name=""hdn_pagesize"" value=""" & user_data.UserPreference.PageSize.ToString & """ />"
        End If

        If (security_data.IsAdmin) Then
            jsIsAdmin.Text = "true"
        Else
            jsIsAdmin.Text = "false"
        End If

        If security_data.IsAdmin And Not ADAuthenticationEnabledForUserType(user_data.IsMemberShip) Then
            username.Text = "<input type=""Text"" maxlength=""255"" size=""25"" name=""username"" id=""username"" value=""" & user_data.Username & """ onkeypress=""return CheckKeyValue(event,'34');"">"
        Else
            username.Text = "<input type=""hidden"" maxlength=""255"" size=""25"" name=""username"" id=""username"" value=""" & user_data.Username & """ onkeypress=""return CheckKeyValue(event,'34');"">" & user_data.Username
        End If
        If ADAuthenticationEnabledForUserType(user_data.IsMemberShip) Then
            TR_domain.Visible = True
            TR_organization.Visible = False
            TR_orgunit.Visible = False
            TR_ldapdomain.Visible = False
            TD_path.InnerHtml = "<input type=""hidden"" name=""userpath"" value=""" & user_data.Path & """>"
            TD_path.InnerHtml += "<input type=""hidden"" name=""domain"" value=""" & user_data.Domain & """>" & user_data.Domain
        ElseIf (setting_data.ADAuthentication = 2) Then
            TR_domain.Visible = False
            TR_organization.Visible = False
            TR_orgunit.Visible = False
            TR_ldapdomain.Visible = True
            'Dim arrOrg As Array
            'Dim arrCount As Long
            'Dim arrItem As Array
            'Dim strOrgUnit As String = ""
            'Dim strOrg As String = ""
            Dim strDC As String = user_data.Domain
            'arrOrg = Split(user_data.Domain, ",")
            'For arrCount = LBound(arrOrg) To UBound(arrOrg)
            '    If (Not (arrOrg(arrCount) = "")) Then
            '        arrItem = Split(arrOrg(arrCount), "=")
            '        If (arrItem(0) = "o" Or arrItem(0) = " o") Then
            '            strOrg = arrItem(1)
            '        ElseIf (arrItem(0) = "ou" Or arrItem(0) = "ou") Then
            '            If (Not (strOrgUnit = "")) Then
            '                strOrgUnit &= ","
            '            End If
            '            strOrgUnit &= arrItem(1)
            '        ElseIf (arrItem(0) = "dc" Or arrItem(0) = " dc") Then
            '            If (Not (strDC = "")) Then
            '                strDC &= "."
            '            End If
            '            strDC &= arrItem(1)
            '        End If
            '    End If
            'Next
            'org.Text = "<input type=""Text"" maxlength=""255"" size=""25"" name=""organization_text"" id=""organization_text"" value=""" & strOrg & """ onkeypress=""return CheckKeyValue(event,'34,32');"">"
            'orgunit.Text = "<input type=""Text"" maxlength=""255"" size=""25"" name=""orgunit_text"" id=""orgunit_text"" value=""" & strOrgUnit & """ onkeypress=""return CheckKeyValue(event,'34,32');"">"
            ldapdomain.Text = "<input type=""Text"" maxlength=""255"" size=""25"" name=""ldapdomain_text"" id=""ldapdomain_text"" value=""" & strDC & """ onkeypress=""return CheckKeyValue(event,'34,32');"">"
        Else
            TR_domain.Visible = False
            TR_organization.Visible = False
            TR_orgunit.Visible = False
            TR_ldapdomain.Visible = False
        End If
        ltr_uid.Text = user_data.Id.ToString()
        If security_data.IsAdmin And Not ADAuthenticationEnabledForUserType(user_data.IsMemberShip) Then
            firstname.Text = "<input type=""Text"" maxlength=""50"" size=""25"" name=""firstname""  id=""firstname"" value=""" & (user_data.FirstName) & """ onkeypress=""return CheckKeyValue(event,'34');"">"
        Else
            firstname.Text = "<input type=""hidden"" maxlength=""50"" size=""25"" name=""firstname"" id=""firstname"" value=""" & (user_data.FirstName) & """ onkeypress=""return CheckKeyValue(event,'34');"">" & user_data.FirstName
        End If
        If security_data.IsAdmin And Not ADAuthenticationEnabledForUserType(user_data.IsMemberShip) Then
            lastname.Text = "<input type=""Text"" maxlength=""50"" size=""25"" name=""lastname"" id=""lastname"" value=""" & (user_data.LastName) & """ onkeypress=""return CheckKeyValue(event,'34');"">"
        Else
            lastname.Text = "<input type=""hidden"" maxlength=""50"" size=""25"" name=""lastname"" id=""lastname"" value=""" & (user_data.LastName) & """ onkeypress=""return CheckKeyValue(event,'34');"">" & user_data.LastName
        End If
        displayname.Text = "<input type=""Text"" maxlength=""50"" size=""25"" name=""displayname"" id=""displayname"" value=""" & (user_data.DisplayName) & """ onkeypress=""return CheckKeyValue(event,'34');"">"
        If ADAuthenticationEnabledForUserType(user_data.IsMemberShip) Then
            hppwd.Text = "<td colspan=""2""><input type=""hidden"" maxlength=""255"" size=""25"" name=""pwd"" id=""pwd"" value=""" & user_data.Password & """ onkeypress=""return CheckKeyValue(event,'34');"">"
            hppwd.Text += "<input type=""hidden"" name=""hpwd"" id=""hpwd"" value=""" & user_data.Password & """</td>"
        Else
            hppwd.Text = "<td class=""label""><span style=""color:red;"">*</span>" & m_refMsg.GetMessage("password label") & "</td>"
            hppwd.Text += "<td class=""value""><input type=""password"" maxlength=""255"" size=""25"" name=""pwd"" id=""pwd"" value=""" & user_data.Password & """ onkeypress=""return CheckKeyValue(event,'34');"">"
            hppwd.Text += "<input type=""hidden"" name=""hpwd"" id=""hpwd"" value=""" & user_data.Password & """>"
            hppwd.Text += "</td>"
        End If
        If ADAuthenticationEnabledForUserType(user_data.IsMemberShip) Then
            confirmpwd.Text = "<td colspan=""2""><input type=""hidden"" maxlength=""255"" size=""25"" name=""confirmpwd"" id=""confirmpwd"" value=""" & user_data.Password & """ onkeypress=""return CheckKeyValue(event,'34');""></td>"
        Else
            confirmpwd.Text = "<td class=""label""><span style=""color:red;"">*</span>" & m_refMsg.GetMessage("confirm pwd label") & "</td>"
            confirmpwd.Text += "<td class=""value""><input type=""password"" maxlength=""255"" size=""25"" name=""confirmpwd"" id=""confirmpwd"" value=""" & user_data.Password & """ onkeypress=""return CheckKeyValue(event,'34');""></td>"
        End If

        If ADIntegrationEnabledForUserType(user_data.IsMemberShip) Then
            email.Text = "<td class=""label"">" & m_refMsg.GetMessage("email address label") & "</td>"
            email.Text += "<td><input type=""Hidden"" maxlength=""255"" size=""25"" name=""email_addr1"" id=""email_addr1"" value=""" & user_data.Email & """ onkeypress=""return CheckKeyValue(event,'34,32');"">" & user_data.Email & "</td>"
        Else
            email.Text = "<td class=""label"">" & m_refMsg.GetMessage("email address label") & "</td>"
            email.Text += "<td class=""value""><input type=""Text"" maxlength=""255"" size=""25"" name=""email_addr1"" id=""email_addr1"" value=""" & user_data.Email & """ onkeypress=""return CheckKeyValue(event,'34,32');""></td>"
        End If
        If m_intGroupType = 1 Then
            email.Text += "<input type=hidden id=email_addr1Org name=email_addr1Org value=""" & user_data.Email & """>"
        End If

        If ((Me.m_refContentApi.RequestInformationRef.LoginAttempts <> -1) And ((security_data.IsAdmin) Or (m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers)))) Then
            accountLocked.Text = "<input type=""checkbox"" id=""chkAccountLocked"" name=""chkAccountLocked"" "
            If (user_data.IsAccountLocked(Me.m_refContentApi.RequestInformationRef)) Then
                accountLocked.Text += " checked "
            End If
            accountLocked.Text += " />"
        Else
            accountLocked.Text = "<input type=""hidden"" id=""chkAccountLocked"" name=""chkAccountLocked"" "
            If (user_data.IsAccountLocked(Me.m_refContentApi.RequestInformationRef)) Then
                accountLocked.Text += " value=""on"" />"
            Else
                accountLocked.Text += " value="""" />"
            End If
        End If

        ltr_avatar.Text = "<input type=""Text"" maxlength=""255"" size=""19"" name=""avatar"" id=""avatar"" value=""" & Server.HtmlEncode(user_data.Avatar.Replace("http://", "")) & """ onkeypress=""return CheckKeyValue(event,'34');"">"
        ltrmapaddress.Text = "<input type=""Text"" maxlength=""100"" size=""19"" name=""mapaddress"" id=""mapaddress"" value=""" & user_data.Address & """ onkeypress=""return CheckKeyValue(event,'34');"">"
        ltrmaplatitude.Text = "<input type=""Text"" maxlength=""100"" size=""19"" name=""maplatitude"" id=""maplatitude"" value=""" & user_data.Latitude & """ onkeypress=""return CheckKeyValue(event,'34');"" disabled>"
        ltrmaplongitude.Text = "<input type=""Text"" maxlength=""100"" size=""19"" name=""maplongitude"" id=""maplongitude"" value=""" & user_data.Longitude & """ onkeypress=""return CheckKeyValue(event,'34');"" disabled>"
		ltr_upload.Text = "</asp:Literal>&nbsp;<a href=""Upload.aspx?action=edituser&addedit=true&returntarget=avatar&EkTB_iframe=true&height=300&width=400&modal=true"" title=""" & m_refMsg.GetMessage("upload txt") & """ class=""ek_thickbox button buttonInline greenHover buttonUpload btnUpload"" title=""" & m_refMsg.GetMessage("upload txt") & """>" & m_refMsg.GetMessage("upload txt") & "</a>"

        'drp_editor.SelectedIndex = 0
        'drp_editor.Items.Add(New ListItem("eWebWP", "ewebwp"))
        drp_editor.Items.Add(New ListItem(m_refMsg.GetMessage("lbl content designer"), "contentdesigner"))
        'drp_editor.Items.Add(New ListItem(m_refMsg.GetMessage("lbl jseditor"), "jseditor"))
        drp_editor.Items.Add(New ListItem("eWebEditPro", "ewebeditpro"))
        Select Case user_data.EditorOption.ToLower().Trim()
            Case "contentdesigner"
                drp_editor.SelectedIndex = 0
                'Case "jseditor"
                '    drp_editor.SelectedIndex = 1
            Case "ewebeditpro"
                drp_editor.SelectedIndex = 1
                'Case "ewebwp"
                '    drp_editor.SelectedIndex = 0
            Case Else ' "ewebwp"
                drp_editor.SelectedIndex = 0 ' default to contentdesigner
        End Select

        With ctlEditor
            .Content = (user_data.Signature)
		End With
        TD_personalTags.InnerHtml = GetPersonalTags()
        CreateColumns()
        If (_activityApi.IsActivityPublishingEnabled And agentList IsNot Nothing And agentList.Count > 0 And Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking, False)) Then
            LoadGrid("colleagues")
            LoadGrid("groups")
            EditPublishPreferencesGrid()
        Else
            EkMembershipActivityTable.Visible = False
            activitiesTab.Visible = False
        End If
        'Community alias Tab
        LoadCommunityAliasTab()
    End Sub

    Public Function GetSignature() As String
        If ctlEditor IsNot Nothing Then
			m_sSignature = ctlEditor.Content
			' Remove SCRIPT elements to prevent XSS attacks, although ContentDesigner.AllowScripts="false" should prevent from getting this far.
			m_sSignature = Regex.Replace(m_sSignature, "\<script[\w\W]+\<\/script\>", "", RegexOptions.IgnoreCase)
		End If
		Return m_sSignature
	End Function

    Private Sub EditUserToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("edit user page title") & " """ & user_data.DisplayUserName & """")
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (user)"), m_refMsg.GetMessage("btn update"), "onclick=""javascript:return SubmitForm('userinfo', 'VerifyForm()');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "javascript:GoBackToCaller()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")

        If m_intGroupType = 0 Then
            result.Append(m_refStyle.GetHelpButton("edituseronly_ascx"))
        Else
            result.Append(m_refStyle.GetHelpButton("EditMembershipUser"))
        End If

        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

#Region "Extending User Modal (Custom Properties)"
    Private Sub Display_UserCustomProperties()
        Page.ClientScript.GetPostBackEventReference(litUCPUI, "")
        Dim strHtml As String = String.Empty
        strHtml = m_refUserApi.EditUserCustomProperties(uId)
        litUCPUI.Text = strHtml
    End Sub
#End Region

    Private Function ADIntegrationEnabledForUserType(ByVal isMember As Boolean) As Boolean
        If Not setting_data.ADIntegration Then
            Return False
        Else
            If Not isMember Then
                Return True
            Else
                Return m_refContentApi.RequestInformationRef.LDAPMembershipUser
            End If
        End If
    End Function

    Private Function ADAuthenticationEnabledForUserType(ByVal isMember As Boolean) As Boolean
        If Not setting_data.ADAuthentication = 1 Then
            Return False
        Else
            If Not isMember Then
                Return True
            Else
                Return m_refContentApi.RequestInformationRef.LDAPMembershipUser
            End If
        End If
    End Function

    Private Function GetLocaleFileString(ByVal localeFileNumber As String) As String
        Dim LocaleFileString As String
        If (CStr(localeFileNumber) = "" Or CInt(localeFileNumber) = 1) Then
            LocaleFileString = "0000"
        Else
            LocaleFileString = New String("0", 4 - Len(Hex(localeFileNumber)))
            LocaleFileString = LocaleFileString & Hex(localeFileNumber)
            If Not System.IO.File.Exists(Server.MapPath(m_refContentApi.AppeWebPath & "locale" & LocaleFileString & "b.xml")) Then
                LocaleFileString = "0000"
            End If
        End If
        Return LocaleFileString.ToString
    End Function

#Region "Personal Tags"
    Public Function GetPersonalTags() As String
        Dim result As New System.Text.StringBuilder
        Dim tdaUser() As TagData
        Dim tdaAll() As TagData
        Dim td As TagData
        Dim htTagsAssignedToUser As Hashtable

        Try

            error_TagsCantBeBlank.Text = m_refMsg.GetMessage("msg error Blank Tag")
            error_InvalidChars.Text = m_refMsg.GetMessage("msg error Tag invalid chars")

            htTagsAssignedToUser = New Hashtable
            result.Append("<div id=""newTagNameDiv"" class=""ektronWindow"">")
            result.Append("<div></div>")
            result.Append(" <div class=""ektronTopSpace"" style=""margin-left: 15px !important;"">")
            result.Append("     <label class=""tagLabel"" >" & m_refMsg.GetMessage("name label") + "</label>&nbsp;&nbsp;<input type=""text"" id=""newTagName"" class=""minWidth"" value="""" size=""20"" onkeypress=""if (event && event.keyCode && (13 == event.keyCode)) {SaveNewPersonalTag(); return false;}"" />")
            result.Append(" </div>")

            If (IsSiteMultilingual) Then
                result.Append("<div class=""ektronTopSpace"" style=""margin-left: 15px !important;"">")
            Else
                result.Append("<div style=""display:none;"" >")
            End If
            result.Append("     <label class=""tagLabel"">" & m_refMsg.GetMessage("res_lngsel_lbl") + "</label>&nbsp;&nbsp;" & GetSiteEnabledLanguageDropDownMarkup("TagLanguage"))
            result.Append(" </div>")

            result.Append(" <div class=""ektronTopSpace"">")
            result.Append("     <ul class=""buttonWrapper ui-helper-clearfix"">")
            result.Append("         <li class=""left"">")
            result.Append("             <a class=""button redHover buttonClear buttonLeft""  type=""button"" value=""" + m_refMsg.GetMessage("btn cancel") + """ onclick=""CancelSaveNewPersonalTag();"" >" + m_refMsg.GetMessage("btn cancel") + "</a>")
            result.Append("         </li>")
            result.Append("         <li>")
            result.Append("             <a class=""button greenHover buttonUpdate buttonRight""  style=""margin-right:14px;"" type=""button"" value=""" + m_refMsg.GetMessage("btn save") + """ onclick=""SaveNewPersonalTag();"" >" + m_refMsg.GetMessage("btn save") + "</a>")
            result.Append("         </li>")
            result.Append("     </ul>")
            result.Append(" </div>")
            result.Append(" <input type=""hidden"" id=""newTagNameHdn"" name=""newTagNameHdn"" value=""""  />")
            result.Append("</div>")
            result.Append("<div id=""newTagNameScrollingDiv"" style='background-color: white;'>")

            Dim localizationApi As New LocalizationAPI()
            Dim lang As LanguageData


            'create hidden list of current tags so we know to delete removed ones.
            For Each lang In languageDataArray
                result.Append("<input type=""hidden"" id=""flag_" & lang.Id & """  value=""" + localizationApi.GetFlagUrlByLanguageID(lang.Id) + """  />")
            Next
            result.Append("<input type=""hidden"" id=""flag_0""  value=""" + localizationApi.GetFlagUrlByLanguageID(-1) + """  />")

            If (uId > 0) Then
                tdaUser = (New Community.TagsAPI).GetTagsForUser(uId, -1)
                Dim appliedTagIds As New StringBuilder

                'build up a list of tags used by user
                'add tags to hashtable for reference later when looping through defualt tag list
                If (Not IsNothing(tdaUser)) Then
                    For Each td In tdaUser
                        htTagsAssignedToUser.Add(td.Id, td)
                        appliedTagIds.Append(td.Id.ToString() + ",")

                        result.Append("<input checked=""checked"" type=""checkbox"" id=""userPTagsCbx_" + td.Id.ToString + """ name=""userPTagsCbx_" + td.Id.ToString + """ />&#160;")
                        result.Append("<img src='" & localizationApi.GetFlagUrlByLanguageID(td.LanguageId) & "' border=""0"" />")
                        result.Append("&#160;" + td.Text + "<br />")
                    Next
                End If

                'create hidden list of current tags so we know to delete removed ones.
                result.Append("<input type=""hidden"" id=""currentTags"" name=""currentTags"" value=""" + appliedTagIds.ToString() + """  />")

                tdaAll = (New Community.TagsAPI).GetDefaultTags(CMSObjectTypes.User, -1)
                If (Not IsNothing(tdaAll)) Then
                    For Each td In tdaAll
                        'don't add to list if its already been added with user's tags above
                        If (Not htTagsAssignedToUser.ContainsKey(td.Id)) Then
                            result.Append("<input type=""checkbox"" id=""userPTagsCbx_" + td.Id.ToString + """ name=""userPTagsCbx_" + td.Id.ToString + """ />&#160;")
                            result.Append("<img src='" & localizationApi.GetFlagUrlByLanguageID(td.LanguageId) & "' border=""0"" />")
                            result.Append("&#160;" + td.Text + "<br />")
                        End If
                    Next
                End If
            End If

            result.Append("<div id=""newAddedTagNamesDiv""></div>")
            result.Append("</div>")
            result.Append("<div class='ektronTopSpace' style='float:left;'>")
            result.Append(" <a class=""button buttonLeft greenHover buttonAddTagWithText"" href=""Javascript:ShowAddPersonalTagArea();"" title=""" & m_refMsg.GetMessage("alt add btn text (personal tag)") & """>" & m_refMsg.GetMessage("btn add personal tag") & "</a>")
            result.Append("</div>")

        Catch ex As Exception
        Finally
            GetPersonalTags = result.ToString
            tdaAll = Nothing
            tdaUser = Nothing
            td = Nothing
            htTagsAssignedToUser = Nothing
        End Try
    End Function

    Private Function GetSiteEnabledLanguageDropDownMarkup(ByVal controlId As String) As String

        Dim i As Integer
        Dim markup As New StringBuilder

        If IsSiteMultilingual Then
            markup.Append("<select id=""" & controlId & """ name=""" & controlId & """ selectedindex=""0"">")
            If (Not (IsNothing(languageDataArray))) Then
                For i = 0 To languageDataArray.Length - 1
                    If (languageDataArray(i).SiteEnabled) Then
                        markup.Append("<option ")
                        If languageDataArray(i).Id = m_refContentApi.DefaultContentLanguage Then
                            markup.Append(" selected")
                        End If
                        markup.Append(" value=" & languageDataArray(i).Id & ">" & languageDataArray(i).Name)
                    End If
                Next
            End If
            markup.Append("</select>")
        Else
            'hardcode to default site language
            markup.Append("<select id=""" & controlId & """ name=""" & controlId & """ selectedindex=""0"" >")
            markup.Append(" <option selected value=" & m_refContentApi.DefaultContentLanguage & ">")
            markup.Append("</select>")
        End If


        Return markup.ToString()
    End Function

    Private Function GetAllLanguageDropDownMarkup(ByVal controlId As String, ByVal defaultMessage As String) As String

        Dim i As Integer
        Dim markup As New StringBuilder

        If (language_data Is Nothing) Then
            language_data = m_refSiteApi.GetAllActiveLanguages()
        End If

        markup.Append("<select id=""" & controlId & """ name=""" & controlId & """ selectedindex=""0"">")
        markup.Append("<option ")
        If (user_data.LanguageId = 0) Then
            markup.Append(" selected")
        End If
        markup.Append(" value=""0"">" & defaultMessage)
        If (Not (IsNothing(language_data))) Then
            For i = 0 To language_data.Length - 1
                markup.Append("<option ")
                If language_data(i).Id = user_data.LanguageId Then
                    markup.Append(" selected")
                End If
                markup.Append(" value=" & language_data(i).Id & ">" & language_data(i).Name)
            Next
        End If
        markup.Append("</select>")

        Return markup.ToString()
    End Function
#End Region
    Protected Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)
        Ektron.Cms.API.Css.RegisterCss(Me, m_refContentApi.AppPath & "csslib/ActivityStream/activityStream.css", "ActivityStream")

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)
		Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS)

        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.AppPath & "csslib/box.css", "EktronBoxCSS")

    End Sub
    Private Sub LoadGrid(ByVal display As String)

        Dim activityListCriteria As New Criteria(Of Ektron.Cms.Activity.ActivityTypeProperty)
        activityListCriteria.OrderByDirection = OrderByDirection.Ascending

        If (display = "colleagues") Then
            activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.Colleague)
        Else
            activityListCriteria.AddFilter(Ektron.Cms.Activity.ActivityTypeProperty.Scope, CriteriaFilterOperator.EqualTo, EkEnumeration.ActivityActionSource.CommunityGroup)
        End If
        activityTypeList = _activityListApi.GetList(activityListCriteria)

        Dim dt As New Data.DataTable
        Dim dr As Data.DataRow
        dt.Columns.Add(New Data.DataColumn("EMPTY", GetType(String)))
        dt.Columns.Add(New Data.DataColumn("EMAIL", GetType(String)))
        dt.Columns.Add(New Data.DataColumn("SMS", GetType(String)))
        dt.Columns.Add(New Data.DataColumn("NEWSFEED", GetType(String)))
        LoadPreferenceList()
        For i As Integer = 0 To activityTypeList.Count - 1
            dr = dt.NewRow
            dr("EMPTY") = activityTypeList(i).Name
            If (preferenceList.Count > 0) Then
                For Each prefData In preferenceList
                    If (CompareIds(activityTypeList(i).Id, 1)) Then
                        dr("EMAIL") = "<center><input type=""Checkbox"" name = ""email" & activityTypeList(i).Id & """ id=""email" & activityTypeList(i).Id & """ checked=""checked"" /></center>"
                    Else
                        dr("EMAIL") = "<center><input type=""Checkbox"" name = ""email" & activityTypeList(i).Id & """ id=""email" & activityTypeList(i).Id & """ /></center>"
                    End If
                    If (CompareIds(activityTypeList(i).Id, 2)) Then
                        dr("NEWSFEED") = "<center><input type=""Checkbox"" name=""feed" & activityTypeList(i).Id & """ id=""feed" & activityTypeList(i).Id & """ checked=""checked""  /></center>"
                    Else
                        dr("NEWSFEED") = "<center><input type=""Checkbox"" name=""feed" & activityTypeList(i).Id & """ id=""feed" & activityTypeList(i).Id & """ /></center>"

                    End If

                    If (CompareIds(activityTypeList(i).Id, 3)) Then
                        dr("SMS") = "<center><input type=""Checkbox"" name =""sms" & activityTypeList(i).Id & """ id=""sms" & activityTypeList(i).Id & """ checked=""checked"" /></center>"
                    Else
                        dr("SMS") = "<center><input type=""Checkbox"" name =""sms" & activityTypeList(i).Id & """ id=""sms" & activityTypeList(i).Id & """ /></center>"
                    End If

                Next
                dt.Rows.Add(dr)
            Else
                dr("EMAIL") = "<center><input type=""Checkbox"" name = ""email" & activityTypeList(i).Id & """ id=""email" & activityTypeList(i).Id & """/></center>"
                dr("SMS") = "<center><input type=""Checkbox"" name =""sms" & activityTypeList(i).Id & """ id=""sms" & activityTypeList(i).Id & """/></center>"
                dr("NEWSFEED") = "<center><input type=""Checkbox"" name=""feed" & activityTypeList(i).Id & """ id=""feed" & activityTypeList(i).Id & """/></center>"
                dt.Rows.Add(dr)
            End If

        Next
        Dim dv As New Data.DataView(dt)
        If (display = "colleagues") Then
            CollGrid.DataSource = dv
            CollGrid.DataBind()
        Else
            GroupGrid.DataSource = dv
            GroupGrid.DataBind()
        End If

    End Sub
    Private Function CompareIds(ByVal prefActivityTypeId As Long, ByVal prefAgentId As Long) As Boolean
        Dim result As Boolean = False
        For Each prefData In preferenceList
            If (prefData.ActivityTypeId = prefActivityTypeId AndAlso prefAgentId = prefData.AgentId) Then
                Return True
            End If
        Next
        Return False
    End Function
    Private Sub LoadPreferenceList()
        Dim groupPrefList As System.Collections.Generic.List(Of NotificationPreferenceData)
        Dim criteria As New Criteria(Of NotificationPreferenceProperty)

        criteria.PagingInfo.RecordsPerPage = 10000
        criteria.AddFilter(NotificationPreferenceProperty.UserId, CriteriaFilterOperator.EqualTo, uId)
        'Getting the Group Preference list
        groupPrefList = _notificationPreferenceApi.GetDefaultPreferenceList(criteria)
        'Getting the Colleagues preference list
        'need to set source to 0 because we dont want individual group prefs.
        criteria.AddFilter(NotificationPreferenceProperty.ActionSourceId, CriteriaFilterOperator.EqualTo, 0)
        preferenceList = _notificationPreferenceApi.GetList(criteria)
        'Adding the group list to Preferences
        preferenceList.AddRange(groupPrefList)
    End Sub
    Private Sub CreateColumns()
        Dim criteria As New Criteria(Of NotificationAgentProperty)
        criteria.AddFilter(NotificationAgentProperty.IsEnabled, CriteriaFilterOperator.EqualTo, True)
        agentList = _notificationAgentApi.GetList(criteria)

        If (agentList IsNot Nothing AndAlso agentList.Count > 0) Then
            CollGrid.Columns.Add(_refStyle.CreateBoundField("EMPTY", "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), False, False))
            GroupGrid.Columns.Add(_refStyle.CreateBoundField("EMPTY", "", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), False, False))
            For Each agentData In agentList
                If agentData.IsEnabled Then
                    Select Case (agentData.Id)
                        Case 1
                            CollGrid.Columns.Add(_refStyle.CreateBoundField("EMAIL", "<center>" + m_refMsg.GetMessage("sync conflict email") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                            GroupGrid.Columns.Add(_refStyle.CreateBoundField("EMAIL", "<center>" + m_refMsg.GetMessage("sync conflict email") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                        Case 2
                            CollGrid.Columns.Add(_refStyle.CreateBoundField("NEWSFEED", "<center>" + m_refMsg.GetMessage("colheader newsfeed") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                            GroupGrid.Columns.Add(_refStyle.CreateBoundField("NEWSFEED", "<center>" + m_refMsg.GetMessage("colheader newsfeed") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                        Case 3
                            CollGrid.Columns.Add(_refStyle.CreateBoundField("SMS", "<center>" + m_refMsg.GetMessage("colheader sms") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                            GroupGrid.Columns.Add(_refStyle.CreateBoundField("SMS", "<center>" + m_refMsg.GetMessage("colheader sms") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(10), Unit.Percentage(10), False, False))
                    End Select
                End If
            Next

        End If
    End Sub
    Private Sub EditPublishPreferencesGrid()
        Dim _publishPrefApi As New Ektron.Cms.Framework.Notifications.NotificationPublishPreference
        Dim publishPrefList As New System.Collections.Generic.List(Of NotificationPublishPreferenceData)
        Dim prefEntry As NotificationPublishPreferenceData
        publishPrefList = _publishPrefApi.GetList(uId)
        publishPrefList.Sort(New NotificationPublishPreferenceData)
        PrivacyGrid.Columns.Add(_refStyle.CreateBoundField("TYPE", m_refMsg.GetMessage("generic actions"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(20), Unit.Percentage(20), False, False))
        PrivacyGrid.Columns.Add(_refStyle.CreateBoundField("ENABLED", "<center>" + m_refMsg.GetMessage("generic publish") + "</center>", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        Dim dt As New Data.DataTable
        Dim dr As Data.DataRow
        dt.Columns.Add(New Data.DataColumn("TYPE", GetType(String)))
        dt.Columns.Add(New Data.DataColumn("ENABLED", GetType(String)))
        For Each prefEntry In publishPrefList
            dr = dt.NewRow
            dr("TYPE") = prefEntry.ActivityTypeName
            If (prefEntry.IsEnabled) Then
                dr("ENABLED") = "<center><input type=""Checkbox"" name=""pref" & prefEntry.ActivityTypeId & """ id=""pref" & prefEntry.ActivityTypeId & """ checked=""checked""  /></center>"
            Else
                dr("ENABLED") = "<center><input type=""Checkbox"" name=""pref" & prefEntry.ActivityTypeId & """ id=""pref" & prefEntry.ActivityTypeId & """ /></center>"
            End If
            dt.Rows.Add(dr)
        Next
        Dim dv As New Data.DataView(dt)
        PrivacyGrid.DataSource = dv
        PrivacyGrid.DataBind()
    End Sub
    Private Sub LoadCommunityAliasTab()
        Dim _communityAlias As New Ektron.Cms.API.UrlAliasing.UrlAliasCommunity
        Dim aliasList As System.Collections.Generic.List(Of Ektron.Cms.Common.UrlAliasCommunityData)

        aliasList = _communityAlias.GetListUser(uId)
        If (aliasList.Count > 0) Then
            For Each item As Ektron.Cms.Common.UrlAliasCommunityData In aliasList
                groupAliasList += "<a href= " & Me.m_refContentApi.SitePath & item.AliasName & " target=_blank>" & Me.m_refContentApi.SitePath & item.AliasName & "</a>"
                groupAliasList += "<br/>"
            Next
        Else
            aliasTab.Visible = False
            tblaliasList.Visible = False
        End If
    End Sub
End Class
