Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Microsoft.Practices.EnterpriseLibrary.Validation

Partial Class adduser
    Inherits System.Web.UI.UserControl

    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected security_data As PermissionData
    Protected language_data As LanguageData()
    Protected m_refSiteApi As New SiteAPI
    Protected m_refUserApi As New UserAPI
    Protected m_refContentApi As New ContentAPI
    Protected setting_data As SettingsData
    Protected domain_list As DomainData()
    Protected search As String = ""
    Protected m_strUserName As String = ""
    Protected ContentLanguage As Integer = -1
    Protected m_strFirstName As String = ""
    Protected m_strLastName As String = ""
    Protected m_strDomain As String = ""
    Protected uId As Long = -1
    Protected user_list As UserData()
    Protected pagedata As Collection
    Protected FromUsers As String = ""
    Protected user_data As UserData
    Protected template_list As TemplateData()
    Protected cAllFolders As Collection
    Protected defaultPreferences As UserPreferenceData
    Protected bLDAP As Boolean = False
    Protected m_intGroupType As Integer = 0
    Protected m_intGroupId As Long = 2
    Protected userdata As New Ektron.Cms.UserData
    Protected UserId As Long = 0
    Dim _CalendarApi As Ektron.Cms.Content.Calendar.WebCalendar = New Ektron.Cms.Content.Calendar.WebCalendar(m_refUserApi.RequestInformationRef)
    Dim calendardata As New Ektron.Cms.Common.Calendar.WebCalendarData

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RegisterResources()

        If (Not IsNothing(Request.QueryString("grouptype")) AndAlso Request.QueryString("grouptype") <> "") Then
            m_intGroupType = Convert.ToInt32(Request.QueryString("grouptype"))
        End If

        If (Not IsNothing(Request.QueryString("groupid")) AndAlso Request.QueryString("groupid") <> "") Then
            m_intGroupId = Convert.ToInt64(Request.QueryString("groupid"))
        End If

        m_refMsg = m_refContentApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        ContentLanguage = m_refSiteApi.ContentLanguage
    End Sub

    Private Function LDAPMembers() As Boolean
        If (m_intGroupType = 1) Then 'member
            Return (m_refUserApi.RequestInformationRef.LDAPMembershipUser)
        ElseIf (m_intGroupType = 0) Then 'CMS user
            Return True
        End If
    End Function


    Public Sub AddUserToSystem()
        Try
            search = Request.QueryString("search")
            setting_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)

            If (LDAPMembers() And (setting_data.ADAuthentication = 1) And ((search = "") Or (search = "0"))) Then
                Response.Redirect("AD/ADsearch.aspx?grouptype=" & m_intGroupType.ToString() & "&groupid=" & m_intGroupId.ToString(), False)
                Exit Sub
                TR_AddUserDetail.Visible = False
                TR_AddLDAPDetail.Visible = False
                PostBackPage.Text = Utilities.SetPostBackPage("users.aspx?Action=AddUserToSystem&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId & "&Search=1&LangType=" & ContentLanguage)
                Display_AddUserToSystem_Search()
            ElseIf (LDAPMembers() And (setting_data.ADAuthentication = 1) And ((search = "1") Or (search = "2"))) Then

                TR_AddUserDetail.Visible = False
                TR_AddLDAPDetail.Visible = False
                If (Not (Page.IsPostBack)) Then
                    Display_AddADUserToSystem()
                Else
                    AddADUsersToSystem()
                End If
            ElseIf (LDAPMembers() And (setting_data.ADAuthentication = 2)) Then
                TR_AddUserList.Visible = False
                TR_AddUserDetail.Visible = False
                If (Not (Page.IsPostBack)) Then
                    Display_AddLDAPUserToSystem()
                    bLDAP = True
                Else
                    AddLDAPUsersToSystem()
                End If
            Else
                TR_AddUserList.Visible = False
                TR_AddLDAPDetail.Visible = False
                If (Not (Page.IsPostBack)) Then
                    Display_AddUserToSystem()
                Else
                    Process_AddUserToSystem()
                End If
            End If

            If (Not (Page.IsPostBack)) Then
                Display_UserCustomProperties(bLDAP)
            Else
                If (m_intGroupType = 1) Then
                    FromUsers = "1"
                    m_intGroupId = 888888 'default
                End If
                If (Request.QueryString("search") = "1" Or Request.QueryString("search") = "2") Then
                    Response.Redirect("users.aspx?action=viewallusers&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId & "&id=2&FromUsers=" & FromUsers, False)
                Else
                    Response.Redirect("users.aspx?action=viewallusers&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId & "&id=2&FromUsers=" & FromUsers & "&OrderBy=" & Request.Form("OrderBy"), False)
                End If
            End If
        Catch ex As Exception
            If (ex.ToString().IndexOf("Username") > -1) Then
                err_msg.Text = " <tr><td class=""label"">&nbsp;</td><td style=""color:red;"">" & m_refContentApi.EkMsgRef.GetMessage("com: duplicate username") & "</td></tr>"
                'Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message.ToString()), False)
            ElseIf (ex.ToString().IndexOf("already exists") > -1) Then
                err_msg.Text = " <tr><td class=""label"">&nbsp;</td><td style=""color:red;"">" & m_refContentApi.EkMsgRef.GetMessage("com: duplicate displayname") & "</td></tr>"
            Else
                Throw ex
            End If
        End Try
    End Sub

    Private Sub AddADUsersToSystem()
        Dim sdUsersNames As New Collection
        Dim sdUsersDomains As New Collection
        Dim lcount As Integer
        Dim strUsername As String = ""
        Dim strDomain As String = ""
        For lcount = 1 To CLng(Request.Form(addusercount.UniqueID))
            strUsername = ""
            strDomain = ""
            strUsername = CStr(Request.Form("adduser" & CStr(lcount)))
            strDomain = CStr(Request.Form("adduserdomain" & CStr(lcount)))
            If ((strUsername <> "") And (strDomain <> "")) Then
                sdUsersNames.Add(strUsername, lcount)
                sdUsersDomains.Add(strDomain, lcount)
            End If
        Next
        If (m_intGroupType = 0) Then
            m_refUserApi.AddADUsersToCMSByUsername(sdUsersNames, sdUsersDomains)
        Else
            Dim usr As User.EkUser
            Dim ret As Boolean = False
            usr = m_refUserApi.EkUserRef
            ret = usr.AddADmemberUsersToCmsByUsername(sdUsersNames, sdUsersDomains)
        End If
    End Sub
    Private Sub AddLDAPUsersToSystem()
        Try
            pagedata = New Collection
            pagedata.Add(Request.Form(LDAP_username.UniqueID), "UserName")
            pagedata.Add(Request.Form(LDAP_firstname.UniqueID), "FirstName")
            pagedata.Add(Request.Form(LDAP_lastname.UniqueID), "LastName")
            pagedata.Add(Request.Form(LDAP_displayname.UniqueID), "DisplayName")
            pagedata.Add(Request.Form(LDAP_username.UniqueID), "Password")
            pagedata.Add(Request.Form(LDAP_language.UniqueID), "Language")
            pagedata.Add(Request.Form(drp_LDAPeditor.UniqueID), "EditorOptions")
            Dim Org As String = ""
            ' Dim arrOrgU As Array
            Dim arrCount As Long = 0
            Dim strDC As String = ""
            If (Request.Form(LDAP_ldapdomain.UniqueID) <> "") Then
                'If (Request.Form(LDAP_orgunit.UniqueID) <> "") Then
                '    arrOrgU = Split(CStr(Request.Form(LDAP_orgunit.UniqueID)), ",")
                '    For arrCount = LBound(arrOrgU) To UBound(arrOrgU)
                '        If (Not (arrOrgU(arrCount) = "")) Then
                '            If (Not (Org = "")) Then
                '                Org &= ","
                '            End If
                '            Org &= "ou="
                '            Org &= arrOrgU(arrCount)
                '        End If
                '    Next
                'End If
                'If (Not (CStr(Request.Form(LDAP_organization.UniqueID)) = "")) Then
                '    Org &= ",o="
                '    Org &= CStr(Request.Form(LDAP_organization.UniqueID))
                'End If
                'If (Not (CStr(Request.Form(LDAP_ldapdomain.UniqueID)) = "")) Then
                '    arrOrgU = Split(CStr(Request.Form(LDAP_ldapdomain.UniqueID)), ".")
                '    For arrCount = LBound(arrOrgU) To UBound(arrOrgU)
                '        strDC &= ",dc="
                '        strDC &= arrOrgU(arrCount)
                '    Next
                '    Org &= strDC
                'End If
                Org = Request.Form(LDAP_ldapdomain.UniqueID)
                pagedata.Add(Org, "Domain")
            End If
            If Request.Form(LDAP_email_addr1.UniqueID) <> "" Then
                pagedata.Add(Request.Form(LDAP_email_addr1.UniqueID), "EmailAddr1")
            Else
                pagedata.Add("", "EmailAddr1")
            End If
            If Request.Form(LDAP_disable_msg.UniqueID) <> "" Then
                pagedata.Add(1, "DisableMsg")
            Else
                pagedata.Add(0, "DisableMsg")
            End If
            pagedata.Add(m_refUserApi.ReadCustomProperties(Request.Form), "UserCustomProperty")
            If (m_intGroupType = 0) Then
                m_refUserApi.AddUser(pagedata)
            Else
                Dim usr As User.EkUser
                Dim ret As Boolean = False
                usr = m_refUserApi.EkUserRef
                ret = usr.AddMemberShipUserV4(pagedata)
            End If
        Catch ex As Exception
            If (EkFunctions.DoesKeyExist(pagedata, "UserName")) Then
                Me.LDAP_username.Value = pagedata("UserName")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "FirstName")) Then
                Me.LDAP_firstname.Value = pagedata("FirstName")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "LastName")) Then
                Me.LDAP_lastname.Value = pagedata("LastName")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "Language")) Then
                If (LDAP_language.Items.FindByValue(pagedata("Language")) IsNot Nothing) Then
                    LDAP_language.Items.FindByValue(pagedata("Language")).Selected = True
                End If
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "EditorOptions")) Then
                drp_LDAPeditor.SelectedIndex = drp_editor.Items.IndexOf(drp_editor.Items.FindByValue(pagedata("EditorOptions")))
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "DisplayName")) Then
                Me.LDAP_displayname.Value = pagedata("DisplayName")
            End If

            If (EkFunctions.DoesKeyExist(pagedata, "EmailAddr1")) Then
                LDAP_email_addr1.Value = pagedata("EmailAddr1")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "DisableMsg")) Then
                LDAP_disable_msg.Checked = CBool(pagedata("DisableMsg"))
            End If
            Throw ex
        End Try
    End Sub
    Private Sub AddLDAPUserToSystemToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add new user msg"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (user)"), m_refMsg.GetMessage("btn save"), "Onclick=""javascript:return SubmitForm('ldapuserinfo', 'VerifyLDAPForm()');"""))
        'result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/loadBalance.png", "#", "Browse LDAP for a user.", "Browse LDAP", "Onclick=""javascript:window.open('LDAP/browse.aspx?from=users&uniqueid=' + UniqueID,null,'height=300,width=400,status=yes,toolbar=no,menubar=no,scrollbars=yes,location=no');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/loadBalance.png", "LDAP/browse.aspx?from=users&method=select&uniqueid=&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId.ToString(), "Browse LDAP for a user.", "Browse LDAP", ""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/magnifier.png", "LDAP/LDAPsearch.aspx?from=users&grouptype=" & m_intGroupType & "&groupid=" & m_intGroupId.ToString(), "Search LDAP for a user.", "Search LDAP", ""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/back.png", "users.aspx?action=viewallusers&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&FromUsers=" & Request.QueryString("FromUsers") & "&groupid=" & m_intGroupId.ToString() & "&id=" & m_intGroupId.ToString() & "&OrderBy=" & Request.QueryString("OrderBy"), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("editusers_ascx"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub


    Private Sub Process_AddUserToSystem()
        Dim pv As Ektron.Cms.Commerce.IPasswordValidation = ObjectFactory.GetPasswordValidation()
        Dim validationString As String = pv.GetRegexForMember().Replace("""", "\""").Replace("\t", "\\t")
        Dim results As ValidationResults
        Dim beIntranet As Boolean = False

        Try
            pagedata = New Collection
            pagedata.Add(Request.Form(username.UniqueID), "UserName")
            pagedata.Add(Request.Form(firstname.UniqueID), "FirstName")
            pagedata.Add(Request.Form(lastname.UniqueID), "LastName")
            pagedata.Add(Request.Form(pwd.UniqueID), "Password")
            pagedata.Add(Request.Form(language.UniqueID), "Language")
            pagedata.Add(Request.Form(drp_editor.UniqueID), "EditorOptions")
            If Request.Form(displayname.UniqueID) <> "" Then
                pagedata.Add(Request.Form(displayname.UniqueID), "DisplayName")
            Else
                pagedata.Add("", "DisplayName")
            End If
            pagedata.Add(Request.Form(avatar.UniqueID), "Avatar")
            pagedata.Add(Request.Form(mapaddress.UniqueID), "Address")
            pagedata.Add(Request.Form(maplatitude.UniqueID), "Latitude")
            pagedata.Add(Request.Form(maplongitude.UniqueID), "Longitude")
            If Request.Form(email_addr1.UniqueID) <> "" Then
                pagedata.Add(Request.Form(email_addr1.UniqueID), "EmailAddr1")
            Else
                pagedata.Add("", "EmailAddr1")
            End If
            If Request.Form(disable_msg.UniqueID) <> "" Then
                pagedata.Add(1, "DisableMsg")
            Else
                pagedata.Add(0, "DisableMsg")
            End If
            pagedata.Add(m_refUserApi.ReadCustomProperties(Request.Form), "UserCustomProperty")

            results = pv.ValidateForAuthor(Request.Form(pwd.UniqueID))
            If (results.IsValid()) Then
                If (m_intGroupType = 1) Then
                    Dim objUser As Ektron.Cms.User.EkUser
                    Dim ret As Boolean
                    objUser = m_refUserApi.EkUserRef
                    ret = objUser.AddMemberShipUserV4(pagedata)
                    UserId = pagedata("userid")
                Else
                    userdata = m_refUserApi.AddUser(pagedata)
                    UserId = userdata.Id
                End If
            Else
                Dim msg As String = String.Empty
                Dim result As ValidationResult
                For Each result In results
                    msg = result.Message
                    Exit For
                Next
                Throw New Exception(msg)
            End If

            beIntranet = IIf(Not (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refUserApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.MembershipUsers)), False, True)
            ''Adding calendar after the user is created.
            If beIntranet AndAlso Request.Form("ek_MsgBoardFeatures_Calendar") IsNot Nothing Then
                Dim calendarChecked As String = Request.Form("ek_MsgBoardFeatures_Calendar")
                If calendarChecked = "on" Then
                    calendardata.Name = "ek_calendar"
                    calendardata.Description = ""
                    _CalendarApi.Add(calendardata, EkEnumeration.WorkSpace.User, UserId)
                End If
            End If

        Catch ex As Exception
            If (EkFunctions.DoesKeyExist(pagedata, "UserName")) Then
                Me.username.Value = pagedata("UserName")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "FirstName")) Then
                Me.firstname.Value = pagedata("FirstName")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "LastName")) Then
                Me.lastname.Value = pagedata("LastName")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "Language")) Then
                If (language.Items.FindByValue(pagedata("Language")) IsNot Nothing) Then
                    language.Items.FindByValue(pagedata("Language")).Selected = True
                End If
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "EditorOptions")) Then
                drp_editor.SelectedIndex = drp_editor.Items.IndexOf(drp_editor.Items.FindByValue(pagedata("EditorOptions")))
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "DisplayName")) Then
                Me.displayname.Value = pagedata("DisplayName")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "Avatar")) Then
                Me.avatar.Value = pagedata("Avatar")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "Address")) Then
                Me.mapaddress.Value = pagedata("Address")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "Latitude")) Then
                Me.maplatitude.Value = pagedata("Latitude")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "Longitude")) Then
                Me.maplongitude.Value = pagedata("Longitude")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "EmailAddr1")) Then
                Me.email_addr1.Value = pagedata("EmailAddr1")
            End If
            If (EkFunctions.DoesKeyExist(pagedata, "DisableMsg")) Then
                Me.disable_msg.Checked = CBool(pagedata("DisableMsg"))
            End If
            Throw ex
        End Try
    End Sub

#Region "AddUserToSystem"
    Private Sub Display_AddUserToSystem()
        language_data = m_refSiteApi.GetAllActiveLanguages()
        security_data = m_refContentApi.LoadPermissions(0, "content")
        AddUserToSystemToolBar()
        Dim i As Integer
        ' drp_editor.Items.Add(New ListItem("eWebWP", "ewebwp"))
        drp_editor.Items.Add(New ListItem(m_refMsg.GetMessage("lbl content designer"), "contentdesigner"))
        'drp_editor.Items.Add(New ListItem(m_refMsg.GetMessage("lbl jseditor"), "jseditor"))
        drp_editor.Items.Add(New ListItem("eWebEditPro", "ewebeditpro"))

        ' All users default to contentdesigner since jsEditor is no longer supported
        'If m_intGroupType = 0 Then
        drp_editor.SelectedIndex = 0
        'Else
        'drp_editor.SelectedIndex = 1
        'End If
        language.Items.Add(New ListItem(m_refMsg.GetMessage("app default msg"), "0"))
        If (Not (IsNothing(language_data))) Then
            For i = 0 To language_data.Length - 1
                language.Items.Add(New ListItem(language_data(i).Name, language_data(i).Id))
            Next
        End If
        If (security_data.IsAdmin And setting_data.EnableMessaging = False) Then
            msg.Text = m_refMsg.GetMessage("application emails disabled msg")
        End If
        litDisableMessage.Text = m_refMsg.GetMessage("disable email notifications msg")
    	ltr_upload.Text = "</asp:Literal>&nbsp;<a href=""Upload.aspx?action=adduser&addedit=true&returntarget=user_avatar&EkTB_iframe=true&height=300&width=400&modal=true"" title=""" & m_refMsg.GetMessage("upload txt") & """ class=""ek_thickbox button buttonInline greenHover buttonUpload btnUpload"" title=""" & m_refMsg.GetMessage("upload txt") & """>" & m_refMsg.GetMessage("upload txt") & "</a>"
    End Sub
    Private Sub AddUserToSystemToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add new user msg"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (user)"), m_refMsg.GetMessage("btn save"), "Onclick=""javascript:return SubmitForm('userinfo', 'VerifyForm()');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/back.png", "users.aspx?action=viewallusers&grouptype=" & m_intGroupType & "&LangType=" & ContentLanguage & "&FromUsers=" & Request.QueryString("FromUsers") & "&groupid=" & m_intGroupId & "&OrderBy=" & Request.QueryString("OrderBy"), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        If m_intGroupType = 0 Then
            result.Append(m_refStyle.GetHelpButton("editusers_ascx"))
        Else
            result.Append(m_refStyle.GetHelpButton("AddUserMemberToSystem"))
        End If
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
#End Region
#Region "Display_AddADUserToSystem"
    Private Sub Display_AddADUserToSystem()
        Dim Sort As String = ""
        Dim sdAttributes As New System.Collections.Specialized.NameValueCollection 'New Collection
        Dim sdFilter As New System.Collections.Specialized.NameValueCollection  'New Collection

        sdAttributes.Add("UserName", "UserName")
        sdAttributes.Add("FirstName", "FirstName")
        sdAttributes.Add("LastName", "LastName")
        sdAttributes.Add("Domain", "Domain")

        If (search = "1") Then
            m_strUserName = Request.Form("username")
            m_strFirstName = Request.Form("firstname")
            m_strLastName = Request.Form("lastname")
            m_strDomain = Request.Form("domainname")
            Sort = "UserName"
        Else
            m_strUserName = Request.QueryString("username")
            m_strFirstName = Request.QueryString("firstname")
            m_strLastName = Request.QueryString("lastname")
            m_strDomain = Request.QueryString("domainname")
            Sort = Request.QueryString("OrderBy")
        End If

        If ((m_strUserName = "") And (m_strFirstName = "") And (m_strLastName = "")) Then
            sdFilter.Add("UserName", "UserName")
            sdFilter.Add("UserNameValue", "*")
        Else
            If (m_strUserName <> "") Then
                sdFilter.Add("UserName", "UserName")
                sdFilter.Add("UserNameValue", m_strUserName) 'sdFilter.add (UserName,"UserNameValue")
            End If
            If (m_strFirstName <> "") Then
                sdFilter.Add("FirstName", "FirstName")
                sdFilter.Add("FirstNameValue", m_strFirstName)
            End If
            If (m_strLastName <> "") Then
                sdFilter.Add("LastName", "LastName")
                sdFilter.Add("LastNameValue", m_strLastName)
            End If
        End If
        user_list = m_refUserApi.GetAvailableADUsers(sdAttributes, sdFilter, Sort, m_strDomain)

        AddADUserToSystemToolBar()
        AddADUserToSystemToolBarGrid()
    End Sub
    Private Sub AddADUserToSystemToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view users in active directory msg"))
        result.Append("<table><tr>")
        If ((Not (IsNothing(user_list))) AndAlso (user_list.Length > 0)) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (users)"), m_refMsg.GetMessage("btn save"), "Onclick=""javascript:return SubmitForm('aduserinfo', '');"""))
        End If
        If (Request.ServerVariables("HTTP_USER_AGENT").ToString.IndexOf("MSIE") > -1) Then 'defect 16045
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/back.png", "javascript:window.location.reload(false);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("editusers_ascx"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub AddADUserToSystemToolBarGrid()
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "CHECK"
        colBound.HeaderText = m_refMsg.GetMessage("add title")
        colBound.HeaderStyle.Width = Unit.Percentage(2)
        colBound.ItemStyle.Width = Unit.Percentage(2)
        AddUserGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "USERNAME"
        colBound.HeaderText = "<a href=""users.aspx?action=AddUserToSystem&OrderBy=UserName&LangType=" & ContentLanguage & "&username=" & m_strUserName & "&lastname=" & m_strLastName & "&firstname=" & m_strFirstName & "&id=" & uId & "&search=2"" title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Username") & "</a>"
        colBound.HeaderStyle.Width = Unit.Percentage(20)
        colBound.ItemStyle.Width = Unit.Percentage(20)
        AddUserGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LASTNAME"
        colBound.HeaderText = "<a href=""users.aspx?action=AddUserToSystem&OrderBy=LastName&LangType=" & ContentLanguage & "&username=" & m_strUserName & "&lastname=" & m_strLastName & "&firstname=" & m_strFirstName & "&id=" & uId & "&search=2"" title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Lastname") & "</a>"
        colBound.HeaderStyle.Width = Unit.Percentage(20)
        colBound.ItemStyle.Width = Unit.Percentage(20)
        AddUserGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "FIRSTNAME"
        colBound.HeaderText = "<a href=""users.aspx?action=AddUserToSystem&OrderBy=FirstName&LangType=" & ContentLanguage & "&username=" & m_strUserName & "&lastname=" & m_strLastName & "&firstname=" & m_strFirstName & "&id=" & uId & "&search=2"" title=""" & m_refMsg.GetMessage("click to sort msg") & """>" & m_refMsg.GetMessage("generic Firstname") & "</a>"
        AddUserGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DOMAIN"
        colBound.HeaderText = m_refMsg.GetMessage("domain title")
        AddUserGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("CHECK", GetType(String)))
        dt.Columns.Add(New DataColumn("USERNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("LASTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("FIRSTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("DOMAIN", GetType(String)))

        Dim i As Integer = 0

        If (Not (IsNothing(user_list))) Then
            For i = 0 To user_list.Length - 1
                dr = dt.NewRow
                dr(0) = "<input type=""CHECKBOX"" name=""adduser" & i + 1 & """ value=""" & user_list(i).Username & """>"
                dr(0) += "<input type=""HIDDEN"" name=""adduserdomain" & i + 1 & """ value=""" & user_list(i).Domain & """>"
                dr(1) = user_list(i).Username
                dr(2) = user_list(i).LastName
                dr(3) = user_list(i).FirstName
                dr(4) = user_list(i).Domain
                dt.Rows.Add(dr)
            Next
        Else
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("no ad users found")
            dr(1) = ""
            dr(2) = ""
            dr(3) = ""
            dt.Rows.Add(dr)
        End If
        addusercount.Value = i + 1
        Dim dv As New DataView(dt)
        AddUserGrid.DataSource = dv
        AddUserGrid.DataBind()
    End Sub
#End Region
#Region "Display_AddLDAPUserToSystem"
    Private Sub Display_AddLDAPUserToSystem()
        language_data = m_refSiteApi.GetAllActiveLanguages()
        security_data = m_refContentApi.LoadPermissions(0, "content")
        AddLDAPUserToSystemToolBar()
        Dim i As Integer
        ' drp_LDAPeditor.Items.Add(New ListItem("eWebWP", "ewebwp"))
        'drp_LDAPeditor.Items.Add(New ListItem(m_refMsg.GetMessage("lbl jseditor"), "jseditor"))
        drp_LDAPeditor.Items.Add(New ListItem("eWebEditPro", "ewebeditpro"))
        'If m_intGroupType = 0 Then
        '    drp_LDAPeditor.SelectedIndex = 1
        'Else
        drp_LDAPeditor.SelectedIndex = 0
        'End If
        LDAP_language.Items.Add(New ListItem(m_refMsg.GetMessage("app default msg"), "0"))
        If (Not (IsNothing(language_data))) Then
            For i = 0 To language_data.Length - 1
                LDAP_language.Items.Add(New ListItem(language_data(i).Name, language_data(i).Id))
            Next
        End If
        If (security_data.IsAdmin And setting_data.EnableMessaging = False) Then
            LDAP_msg.Text = "<tr><td colspan=""2"" class=""important"">" & m_refMsg.GetMessage("application emails disabled msg") & "</td></tr>"
        End If
        LDAP_disable_msg.Text = m_refMsg.GetMessage("disable email notifications msg")
        ' <input type="CHECKBOX" name="disable_msg" value="disable_msg" >
        '<%= (m_refMsg.GetMessage("disable email notifications msg")) %>&nbsp;
    End Sub
#End Region
#Region "Display_AddUserToSystem_Search"
    Private Sub Display_AddUserToSystem_Search()
        language_data = m_refSiteApi.GetAllActiveLanguages()
        security_data = m_refContentApi.LoadPermissions(0, "content")
        domain_list = m_refUserApi.GetDomains(0, 0)
        AddUserToSystemToolBar_Search()
        AddUserToSystemGrid_Search()
    End Sub
    Private Sub AddUserToSystemToolBar_Search()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("active directory search msg"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("editusers_ascx"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub AddUserToSystemGrid_Search()
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "USERNAME"
        colBound.HeaderText = m_refMsg.GetMessage("generic Username")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Height = Unit.Empty
        AddUserGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "FIRSTNAME"
        colBound.HeaderText = m_refMsg.GetMessage("generic Firstname")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderStyle.Height = Unit.Empty
        AddUserGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LASTNAME"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderText = m_refMsg.GetMessage("generic Lastname")
        colBound.ItemStyle.Wrap = False
        AddUserGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderText = m_refMsg.GetMessage("domain title")
        colBound.ItemStyle.Wrap = False
        AddUserGrid.Columns.Add(colBound)



        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("USERNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("FIRSTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("LASTNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))


        dr = dt.NewRow
        dr(0) = "<input type=""Text"" name=""username"" maxlength=""255"" size=""15"" OnKeyPress=""javascript:return CheckKeyValue(event,'34');"">"
        dr(1) = "<input type=""Text"" name=""firstname"" maxlength=""50"" size=""15"" OnKeyPress=""javascript:return CheckKeyValue(event,'34');"">"
        dr(2) = "<input type=""Text"" name=""lastname"" maxlength=""50"" size=""15"" OnKeyPress=""javascript:return CheckKeyValue(event,'34');"">"
        dr(2) += "<input type=""hidden"" id=""uid"" name=""uid"" value=""""> <input type=""hidden"" id=""rp"" name=""rp"" value="""">"
        dr(2) += "<input type=""hidden"" id=""ep"" name=""e1"" value=""""> <input type=""hidden"" id=""e2"" name=""e2"" value="""">"
        dr(2) += "<input type=""hidden"" id=""f"" name=""f"" value="""">"
        dr(3) = "<select name=""domainname"">"
        If (Not (IsNothing(domain_list))) AndAlso m_refContentApi.RequestInformationRef.ADAdvancedConfig = False Then
            dr(3) += "<option selected value="""">" & m_refMsg.GetMessage("all domain select caption") & "</option>"
        End If
        Dim i As Integer
        For i = 0 To domain_list.Length - 1
            dr(3) += "<option value=""" & domain_list(i).Name & """>" & domain_list(i).Name & "</option>"
        Next
        dr(3) += "</select>"
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = "<input type=""submit"" name=""search"" value=""" & m_refMsg.GetMessage("generic Search") & ">"
        dr(1) = ""
        dr(2) = ""
        dr(3) = ""
        dt.Rows.Add(dr)
        Dim dv As New DataView(dt)
        AddUserGrid.DataSource = dv
        AddUserGrid.DataBind()
    End Sub
#End Region
#Region "Extending User Modal (Custom Properties)"
    Private Sub Display_UserCustomProperties(Optional ByVal LDAP As Boolean = False)
        Dim strHtml As String = String.Empty
        strHtml = m_refUserApi.EditUserCustomProperties()
        If LDAP Then
            LDAP_litUCPUI.Visible = True
            LDAP_litUCPUI.Text = strHtml
        Else
            litUCPUI.Text = strHtml
        End If
    End Sub
#End Region

    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
		Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS)
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.AppPath & "csslib/box.css", "EktronBoxCSS")
    End Sub

End Class
