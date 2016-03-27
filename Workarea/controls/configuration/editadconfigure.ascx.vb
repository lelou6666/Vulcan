Imports Ektron.Cms
Partial Class editadconfigure
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
    Protected AppName As String = ""
    Protected m_refUserApi As UserAPI
    Protected m_refSiteApi As SiteAPI
    Protected setting_data As SettingsData
    Protected mapping_data As AdMappingData()
    Protected domain_data As DomainData()
    Protected sync_data As AdSyncData
    Protected cGroup As UserGroupData
    Protected m_bADAdvanced As Boolean = False
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        m_refUserApi = New UserAPI
        m_bADadvanced = m_refUserApi.RequestInformationRef.ADAdvancedConfig
        jsUniqueID.Text = UniqueID & "_"
        m_refMsg = (New CommonApi).EkMsgRef
        RegisterResources()
    End Sub
    Public Function EditAdConfiguration() As Boolean
        Try
            If (Not (Page.IsPostBack)) Then
                Display_EditConfiguration()
            Else
                Update()
                Return (True)
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Function
    Private Function Display_EditConfiguration() As Boolean
        Dim arrOrg As Array
        Dim arrItem As Array
        Dim arrCount As Long
        Dim arrDomain As Array
        Dim arrCount2 As Long
        Dim arrOrg2 As Array
        Dim isUnit As Boolean = False
        Dim arrServer As Array
        Dim arrLDAPDomain As Array
        Dim strLDAPDomain As String = ""
        Dim arrLDAPDomainElement As Array
        Dim first As Boolean = True
        Dim adselectedstate As String = ""

        m_refUserApi = New UserAPI
        m_refSiteApi = New SiteAPI
        AppImgPath = m_refUserApi.AppImgPath
        AppName = m_refUserApi.AppName
        setting_data = m_refSiteApi.GetSiteVariables()
        mapping_data = m_refUserApi.GetADMapping(m_refUserApi.UserId, "userprop", 1, 0, 1)
        cGroup = m_refUserApi.GetUserGroupById(1)
        sync_data = m_refUserApi.GetADStatus()

        Try
            domain_data = m_refUserApi.GetDomains(1, 0)
        Catch
            domain_data = Nothing
        End Try

        EditToolBar() 'POPULATE TOOL BAR
        'VERSION
        versionNumber.InnerHtml = m_refMsg.GetMessage("version") & "&nbsp;" & m_refSiteApi.Version & "&nbsp;" & m_refSiteApi.ServicePack
        'BUILD NUMBER
        buildNumber.InnerHtml = "<i>(" & m_refMsg.GetMessage("build") & m_refSiteApi.BuildNumber & ")</i>"

        If ((sync_data.SyncUsers) Or (sync_data.SyncGroups) Or (sync_data.SyncRelationships) Or (sync_data.DeSyncUsers) Or (sync_data.DeSyncGroups)) Then
            If (setting_data.ADAuthentication = 1) Then
                ltr_status.Text = "<a href=""adreports.aspx?action=ViewAllReportTypes"">" & m_refMsg.GetMessage("ad enabled not configured") & "</a>"
            Else
                ltr_status.Text = "<a href=""adreports.aspx?action=ViewAllReportTypes"">" & m_refMsg.GetMessage("ad disabled not configured") & "</a>"
            End If
        Else
            sync.visible = False
        End If

        If (setting_data.IsAdInstalled) Then
            installed.InnerHtml = m_refMsg.GetMessage("active directory installed")
        Else
            installed.InnerHtml = m_refMsg.GetMessage("active directory not installed")
        End If

        If (setting_data.ADAuthentication = 1) Then
            EnableADAuth.Checked = True
            adselectedstate = ""
            'OrgUnitText.Disabled = True
            OrgText.Disabled = True
            ServerText.Disabled = True
            drp_LDAPtype.Enabled = False
            PortText.Disabled = True
            LDAPDomainText.Disabled = True
            txtLDAPAttribute.Enabled = False
            LDAP_SSL.Disabled = True
            admingroupdomain.Disabled = False
            admingroupname.Disabled = False
        ElseIf (setting_data.ADAuthentication = 0) Then
            DisableAD.Checked = True
            adselectedstate = "disabled"
            'OrgUnitText.Disabled = True
            OrgText.Disabled = True
            ServerText.Disabled = True
            drp_LDAPtype.Enabled = False
            PortText.Disabled = True
            LDAPDomainText.Disabled = True
            txtLDAPAttribute.Enabled = False
            LDAP_SSL.Disabled = True
            admingroupdomain.Disabled = True
            admingroupname.Disabled = True
        Else
            EnableLDAP.Checked = True
            adselectedstate = "disabled"
            'OrgUnitText.Disabled = False
            OrgText.Disabled = False
            ServerText.Disabled = False
            drp_LDAPtype.Enabled = True
            PortText.Disabled = False
            LDAPDomainText.Disabled = False
            txtLDAPAttribute.Enabled = True
            LDAP_SSL.Disabled = False
            admingroupdomain.Disabled = True
            admingroupname.Disabled = True
        End If
        If (setting_data.ADIntegration) Then
            EnableADInt.Checked = True
        End If
        If (Not (setting_data.ADAuthentication = 1)) Then
            EnableADInt.Disabled = True
        End If
        If (setting_data.ADAutoUserAdd) Then
            EnableAutoUser.Checked = True
        End If
        If (Not (setting_data.ADAuthentication = 1)) Then
            EnableAutoUser.Disabled = True
        End If
        'EnableAutoUserToGroup
        If (setting_data.ADAutoUserToGroup) Then
            EnableAutoUserToGroup.Checked = True
        End If
        If (Not (setting_data.ADAuthentication = 1)) Then
            EnableAutoUserToGroup.Disabled = True
        End If
        userProperty.InnerHtml = m_refMsg.GetMessage("user property mapping")
        cmsProperty.InnerHtml = m_refMsg.GetMessage("cms property value")
        activeDirectoryProperty.InnerHtml = m_refMsg.GetMessage("active directory property value")
        userpropcount.Value = mapping_data.Length

        Dim i As Integer
        Dim result As New System.Text.StringBuilder
        If (Not (IsNothing(mapping_data))) Then
            For i = 0 To mapping_data.Length - 1
                result.Append("<tr>")
                result.Append("<td class=""label"">" & mapping_data(i).CmsName & ":</td>")
                result.Append("<td>")
                result.Append("<input type=""hidden"" maxlength=""50"" size=""50"" name=""userpropname" & i + 1 & """  id=""userpropname" & i + 1 & """ value=""" & mapping_data(i).CmsName & """>")
                result.Append("<input type=""text"" maxlength=""50"" size=""25"" " & adselectedstate & " name=""userpropvalue" & i + 1 & """ id=""userpropvalue" & i + 1 & """ value=""" & mapping_data(i).AdName & """>")
                result.Append("</td>")
                result.Append("</tr>")
            Next
            mapping_list.Text = result.ToString
            result = Nothing
        End If
        adminGroupMap.InnerHtml = m_refMsg.GetMessage("cms admin group map")
        adGroupName.InnerHtml = m_refMsg.GetMessage("AD Group Name")
        ADDomain.InnerHtml = m_refMsg.GetMessage("AD Domain")
        admingroupname.Value = cGroup.GroupName
        admingroupdomain.Value = cGroup.GroupDomain
        drp_LDAPtype.Items.Add(New ListItem(m_refMsg.GetMessage("LDAP AD"), "AD"))
        drp_LDAPtype.Items.Add(New ListItem(m_refMsg.GetMessage("LDAP NO"), "NO"))
        drp_LDAPtype.Items.Add(New ListItem(m_refMsg.GetMessage("LDAP SU"), "SU"))
        drp_LDAPtype.Items.Add(New ListItem(m_refMsg.GetMessage("LDAP OT"), "OT"))
        drp_LDAPtype.Attributes.Add("onchange", "javascript:CheckLDAP('', true);")
        If (setting_data.ADAuthentication = 2) Then
            If InStr(setting_data.ADDomainName, "&lt;/p&gt;") > 0 Then 'defect 17813 - SMK
                setting_data.ADDomainName = Replace(setting_data.ADDomainName, "&lt;", "<")
                setting_data.ADDomainName = Replace(setting_data.ADDomainName, "&gt;", ">")
                setting_data.ADDomainName = Replace(setting_data.ADDomainName, "&quot;", """")
                setting_data.ADDomainName = Replace(setting_data.ADDomainName, "&#39;", "'")
            End If
            Dim ldapsettings As LDAPSettingsData
            ldapsettings = Common.EkFunctions.GetLDAPSettings(setting_data.ADDomainName)
            arrDomain = Split(setting_data.ADDomainName, "</server>")
            arrServer = Split(arrDomain(0), "</p>")

            ServerText.Value = ldapsettings.Server
            PortText.Value = ldapsettings.Port
            LDAPjs.Text += "<script language=""javascript"" type=""text/javascript"">" & Environment.NewLine

            drp_LDAPtype.SelectedIndex = ldapsettings.ServerType.GetHashCode()
            LDAPjs.Text += "     CheckLDAP('" & drp_LDAPtype.Items(drp_LDAPtype.SelectedIndex).Value & "', false);" & Environment.NewLine
            LDAP_SSL.Checked = (ldapsettings.EncryptionType = Common.EkEnumeration.LDAPEncryptionType.SSL)
            txtLDAPAttribute.Text = ldapsettings.Attribute
            If (UBound(arrServer) > 1) Then
                arrLDAPDomain = Split(arrServer(2), ",")
                For arrCount = LBound(arrLDAPDomain) To UBound(arrLDAPDomain)
                    arrLDAPDomainElement = Split(arrLDAPDomain(arrCount), "=")
                    If (arrLDAPDomainElement(0) = "dc") Then
                        If (Not (strLDAPDomain = "")) Then
                            strLDAPDomain &= "."
                        End If
                        strLDAPDomain &= arrLDAPDomainElement(1)
                    End If
                Next
                LDAPDomainText.Value = strLDAPDomain
            End If
            arrOrg2 = Split(arrDomain(1), "</>")
            For arrCount2 = LBound(arrOrg2) To UBound(arrOrg2)
                'Response.Write(arrOrg2(arrCount2) & "<br/>")
                If (Not (arrOrg2(arrCount2) = "")) Then
                    arrOrg = Split(arrOrg2(arrCount2), ",")
                    For arrCount = LBound(arrOrg) To UBound(arrOrg)
                        If (Not (arrOrg(arrCount) = "")) Then
                            arrItem = Split(arrOrg(arrCount), "=")
                            If ((arrItem(0) = "o" Or arrItem(0) = " o")) And arrCount2 = UBound(arrOrg2) Then
                                OrgText.Value = arrItem(1)
                                'ElseIf (arrItem(0) = "ou" Or arrItem(0) = " ou") Then
                                '    If (Not first) Then
                                '        OrgUnitText.Value &= ","
                                '    End If
                                '    OrgUnitText.Value &= "ou=" & arrItem(1)
                                '    isUnit = True
                                '    first = False
                            Else
                                If (Not first) Then
                                    OrgUnitText.Value &= ","
                                End If
                                OrgUnitText.Value &= arrOrg(arrCount)
                                isUnit = True
                                first = False
                            End If
                        End If
                    Next
                    If (isUnit) Then
                        OrgUnitText.Value &= "</>"
                        isUnit = False
                        first = True
                    End If
                End If
            Next
        End If
        If (domain_data Is Nothing) Then
            searchLink.InnerHtml = "<a href=""#"" OnClick=""javascript:alert('" & m_refMsg.GetMessage("javascript: alert cannot search no domains") & "\n" & m_refMsg.GetMessage("generic check ad config msg") & "'); return false;"">" & m_refMsg.GetMessage("generic Search") & "</a>"
        ElseIf (domain_data.Length = 0) Then
            searchLink.InnerHtml = "<a href=""#"" OnClick=""javascript:alert('" & m_refMsg.GetMessage("javascript: alert cannot search no domains") & "\n" & m_refMsg.GetMessage("generic check ad config msg") & "'); return false;"">" & m_refMsg.GetMessage("generic Search") & "</a>"
        Else
            searchLink.InnerHtml = "<a href=""#"" OnClick=""javascript:DoSearch();"">" & m_refMsg.GetMessage("generic Search") & "</a>"
        End If
        domain.InnerHtml = m_refMsg.GetMessage("domain title") & ":"
        result = New System.Text.StringBuilder
        result.Append("&nbsp;")

        If (domain_data Is Nothing) Then
            Dim selected As String = ""
            result.Append("<select " & adselectedstate & " name=""domainname"" id=""domainname"">")
            If (setting_data.ADDomainName = "") Then
                selected = " selected"
            End If
            ' Keep the "All Domains" drop down for continuity
            result.Append("<option value="""" " & selected & ">" & m_refMsg.GetMessage("all domain select caption") & "</option>")
            result.Append("</select>")
        ElseIf (domain_data.Length = 0) Then
            result.Append("<font color=""red""><strong>" & m_refMsg.GetMessage("generic no domains found") & " " & m_refMsg.GetMessage("generic check ad config msg") & "</strong></font>")
        Else
            Dim selected As String = ""
            If m_refUserApi.RequestInformationRef.ADAdvancedConfig = True Then
                result.Append("<select " & adselectedstate & " name=""domainname"" id=""domainname"" disabled>")
            Else
                result.Append("<select " & adselectedstate & " name=""domainname"" id=""domainname"">")
            End If
            If (setting_data.ADDomainName = "") Then
                selected = " selected"
            End If

            result.Append("<option value="""" " & selected & ">" & m_refMsg.GetMessage("all domain select caption") & "</option>")
            For i = 0 To domain_data.Length - 1
                If (domain_data(i).Name = setting_data.ADDomainName) Then
                    selected = " selected"
                Else
                    selected = ""
                End If
                result.Append("<option value=""" & domain_data(i).Name & """" & selected & ">" & domain_data(i).Name & "</option>")
            Next
            result.Append("</select>")
        End If
        domainDropdown.InnerHtml = result.ToString
    End Function
    Private Sub EditToolBar()
        Dim result As New System.Text.StringBuilder
        Try
            result.Append("<table><tr>")
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("adconfig page title"))
            If (domain_data Is Nothing) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update settings button text"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('config', 'VerifyForm()');"""))
            ElseIf (domain_data.Length = 0) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update settings button text"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:alert('" & m_refMsg.GetMessage("javascript: alert cannot update no domains") & "\n" & m_refMsg.GetMessage("generic check ad config msg") & "'); return false;"""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update settings button text"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('config', 'VerifyForm()');"""))
            End If
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "adconfigure.aspx", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            result.Append("<td>")
			result.Append(m_refStyle.GetHelpButton("editadconfigure_ascx"))
            result.Append("</td>")
            result.Append("</tr></table>")
            htmToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Function Update() As Boolean
        Try
            Dim i As Integer
            Dim Org As String
            Dim Port As String
            Dim LDAPDomain As String = ""
            Dim arrLDAPDomain As Array
            Dim arrOrgU As Array
            Dim arrOrgUSep As Array
            Dim arrCount2 As Long
            Dim arrCount As Long
            Dim sChar As String = ":"
            Dim page_addata As New Hashtable
            Dim pagedata As New System.Collections.Specialized.NameValueCollection
            For i = 1 To CLng(Request.Form(userpropcount.UniqueID))
                pagedata.Add(CStr(Request.Form("userpropname" & CStr(i))), CStr(Request.Form("userpropvalue" & CStr(i))))
            Next
            Dim m_refUserApi As New UserAPI
            'TODO: The following comments added by UDAI on 11/22/05.  for defect#16785
            'while implementing LDAP, ADGroupSelect hardcoded and widely used in this page for VS2003.  I am keeping the same hardcode
            'parameter according to VS2005 (:  to  $).  This page needs radio group using servercontrol property.
            'TODO: The following comments added by SCOTTK on 1/04/06.  for defect# 17121 & 17367
            ' We were getting issues with the : to $ switch in 5.1.x, so I am placing code here to detect the right char to show
            If (Request.Form(UniqueID & "$ADGroupSelect") <> "") Then
                sChar = "$"
            ElseIf (Request.Form(UniqueID & "_ADGroupSelect") <> "") Then
                sChar = "_"
            Else
                sChar = ":"
            End If
            If (Request.Form(UniqueID & sChar & "ADGroupSelect") = "enable_adauth") Then
                'only update user properties if AD Authentication is to be enabled 
                m_refUserApi.UpdateADMapping(m_refUserApi.UserId, pagedata)
            End If

            If (Request.Form(UniqueID & sChar & "ADGroupSelect") = "enable_adauth") Then
                page_addata.Add("ADAuthentication", 1)
            ElseIf (Request.Form(UniqueID & sChar & "ADGroupSelect") = "disable_adauth") Then
                page_addata.Add("ADAuthentication", 0)
            Else
                page_addata.Add("ADAuthentication", 2)
            End If
            If (Request.Form(EnableADInt.UniqueID) <> "") Then
                page_addata.Add("ADIntegration", 1)
            Else
                page_addata.Add("ADIntegration", 0)
            End If

            If (Request.Form(EnableAutoUser.UniqueID) <> "") Then
                page_addata.Add("ADAutoUserAdd", 1)
            Else
                page_addata.Add("ADAutoUserAdd", 0)
            End If

            If (Request.Form(EnableAutoUserToGroup.UniqueID) <> "") Then
                page_addata.Add("ADAutoUserToGroup", 1)
            Else
                page_addata.Add("ADAutoUserToGroup", 0)
            End If

            If (Request.Form(UniqueID & sChar & "ADGroupSelect") = "enable_adauth") Then
                page_addata.Add("ADDomainName", CStr(Request.Form("domainname")))
            ElseIf ((Request.Form(UniqueID & sChar & "ADGroupSelect") = "enable_LDAP")) Then
                Org = CStr(Request.Form(ServerText.UniqueID))
                Port = CStr(Request.Form(PortText.UniqueID))
                arrLDAPDomain = Split(CStr(Request.Form(LDAPDomainText.UniqueID)), ".")
                For arrCount = LBound(arrLDAPDomain) To UBound(arrLDAPDomain)
                    If (Not (LDAPDomain = "")) Then
                        LDAPDomain &= ","
                    End If
                    LDAPDomain &= "dc="
                    LDAPDomain &= arrLDAPDomain(arrCount)
                Next
                arrCount = 0
                Org &= "</p>"
                Org &= Port
                If (Not (LDAPDomain = "")) Then
                    Org &= "</p>"
                    Org &= LDAPDomain
                End If
                Org &= "</server>"
                arrOrgUSep = Split(CStr(Request.Form(OrgUnitText.UniqueID)), "</>")
                For arrCount2 = LBound(arrOrgUSep) To UBound(arrOrgUSep)
                    If (Not (arrOrgUSep(arrCount2) = "")) Then
                        arrOrgU = Split(arrOrgUSep(arrCount2), ",")
                        Dim first As Boolean
                        first = True
                        For arrCount = LBound(arrOrgU) To UBound(arrOrgU)
                            If (Not (arrOrgU(arrCount) = "")) Then
                                If (Not (first)) Then
                                    Org &= ","
                                End If
                                first = False
                                'Org &= "ou="
                                Org &= arrOrgU(arrCount)
                            End If
                        Next
                        Org &= "</>"
                    End If
                Next
                If (Not (CStr(Request.Form(OrgText.UniqueID)) = "")) Then
                    Org &= "o="
                    Org &= CStr(Request.Form(OrgText.UniqueID))
                End If
                Org &= "</server>" & Request.Form(drp_LDAPtype.UniqueID)
                If Request.Form(LDAP_SSL.UniqueID) <> "" Then
                    Org &= "</server>" & "SSL"
                Else
                    Org &= "</server>"
                End If
                If Request.Form(txtLDAPAttribute.UniqueID) <> "" Then
                    Org &= "</server>" & Common.EkFunctions.GetDbString(Request.Form(txtLDAPAttribute.UniqueID), 10)
                Else
                    Org &= "</server>"
                End If
                page_addata.Add("ADDomainName", Org)
            Else
                page_addata.Add("ADDomainName", CStr(Request.Form("domainname")))
            End If

            Dim m_refSiteApi As New SiteAPI

            m_refSiteApi.UpdateSiteVariables(page_addata)


            If (Request.Form(EnableADInt.UniqueID) <> "") Then
                'only update admin mapping if AD turned on
                m_refUserApi.MapCMSUserGroupToAD(1, CStr(Request.Form(admingroupname.UniqueID)), CStr(Request.Form(admingroupdomain.UniqueID)))
            End If
            Return (True)
            'Response.Redirect("adconfigure.aspx", False)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Function
    Private Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)

        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronEmpJSFuncJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
End Class
