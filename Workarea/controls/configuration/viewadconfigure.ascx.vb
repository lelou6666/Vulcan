Imports Ektron.Cms
Partial Class viewadconfigure
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
    Protected m_refUserApi As UserAPI
    Protected m_refSiteApi As SiteAPI
    Protected setting_data As SettingsData
    Protected mapping_data As AdMappingData()
    Protected domain_data As DomainData()
    Protected sync_data As AdSyncData
    Protected group_data As UserGroupData
    Protected AdValid As Boolean = False
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        m_refMsg = (New CommonApi).EkMsgRef
    End Sub
    Public Function Display_ViewConfiguration() As Boolean
        m_refUserApi = New UserAPI
        m_refSiteApi = New SiteAPI
        AppImgPath = m_refUserApi.AppImgPath
        AppName = m_refUserApi.AppName
        RegisterResources()
        setting_data = m_refSiteApi.GetSiteVariables(m_refUserApi.UserId)
        mapping_data = m_refUserApi.GetADMapping(m_refUserApi.UserId, "userprop", 1, 0, 1)
        group_data = m_refUserApi.GetUserGroupById(1)
        sync_data = m_refUserApi.GetADStatus()
        'domain_data = m_refUserApi.GetDomains(1, 0)
        AdValid = setting_data.AdValid 'CBool(siteVars("AdValid"))
        ViewToolBar()
        'VERSION
        versionNumber.InnerHtml = m_refMsg.GetMessage("version") & "&nbsp;" & m_refSiteApi.Version & "&nbsp;" & m_refSiteApi.ServicePack
        'BUILD NUMBER
        buildNumber.InnerHtml = "<i>(" & m_refMsg.GetMessage("build") & m_refSiteApi.BuildNumber & ")</i>"

        licenseMessageContainer.Visible = False
        If (Not (AdValid)) Then
            TR_domaindetail.Visible = False
            licenseMessageContainer.Visible = True
            licenseMessage.InnerHtml = m_refMsg.GetMessage("entrprise license with AD required msg")
        Else
            If ((sync_data.SyncUsers) Or (sync_data.SyncGroups) Or (sync_data.SyncRelationships) Or (sync_data.DeSyncUsers) Or (sync_data.DeSyncGroups)) Then
                If (setting_data.ADAuthentication = 1) Then
                    ltr_status.Text = "<a href=""adreports.aspx?action=ViewAllReportTypes"">" & m_refMsg.GetMessage("ad enabled not configured") & "</a>"
                Else
                    ltr_status.Text = "<a href=""adreports.aspx?action=ViewAllReportTypes"">" & m_refMsg.GetMessage("ad disabled not configured") & "</a>"
                End If
            Else
                sync.Visible = False
            End If
            If (setting_data.IsAdInstalled) Then
                installed.InnerHtml = m_refMsg.GetMessage("active directory installed") & "&nbsp;"
            Else
                installed.InnerHtml = m_refMsg.GetMessage("active directory not installed") & "&nbsp;"
            End If
            td_flag.InnerHtml = m_refMsg.GetMessage("active directory authentication flag")
            If (setting_data.ADAuthentication = 1) Then
                TD_flagenabled.InnerHtml = m_refMsg.GetMessage("AD enabled")
            ElseIf (setting_data.ADAuthentication = 2) Then
                TD_flagenabled.InnerHtml = m_refMsg.GetMessage("LDAP enabled")
            Else
                TD_flagenabled.InnerHtml = m_refMsg.GetMessage("disabled")
            End If
            TD_dirflag.InnerHtml = m_refMsg.GetMessage("active directory flag")
            If (setting_data.ADIntegration) Then
                TD_intflag.InnerHtml = m_refMsg.GetMessage("enabled")
            Else
                TD_intflag.InnerHtml = m_refMsg.GetMessage("disabled")
            End If
            TD_autouser.InnerHtml = m_refMsg.GetMessage("auto add user flag")
            If (setting_data.ADAutoUserAdd) Then
                TD_autouserflag.InnerHtml = m_refMsg.GetMessage("enabled")
            Else
                TD_autouserflag.InnerHtml = m_refMsg.GetMessage("disabled")
            End If
            TD_autogroup.InnerHtml = m_refMsg.GetMessage("auto add user to group flag")

            If (setting_data.ADAutoUserToGroup) Then
                TD_autogroupflag.InnerHtml = m_refMsg.GetMessage("enabled")
            Else
                TD_autogroupflag.InnerHtml = m_refMsg.GetMessage("disabled")
            End If
            userProperty.InnerHtml = m_refMsg.GetMessage("user property mapping title")
            TD_cmstitle.InnerHtml = m_refMsg.GetMessage("cms property title")
            TD_dirproptitle.InnerHtml = m_refMsg.GetMessage("active directory property title")
            Dim i As Integer = 0
            If (Not (IsNothing(mapping_data))) Then
                Dim result As New System.Text.StringBuilder
                For i = 0 To mapping_data.Length - 1                    
                    result.Append("<tr>")
                    result.Append("<td class=""label"">" & mapping_data(i).CmsName & ":</td>")
                    result.Append("<td class=""readOnlyValue"">" & mapping_data(i).AdName & "</td>")
                    result.Append("<tr>")
                Next
                mapping_list.Text = result.ToString
            End If
            adminGroupMap.InnerHtml = m_refMsg.GetMessage("cms admin group map")
            TD_grpnameval.InnerHtml = group_data.GroupName
            TD_grpDomainVal.InnerHtml = group_data.GroupDomain
            domain.InnerHtml = m_refMsg.GetMessage("domain title") & ":"
            'If (domain_data.Length = 0) Then
            'domainValue.InnerHtml += "<font color=""red""><strong>" & m_refMsg.GetMessage("generic no domains found") & " " & m_refMsg.GetMessage("generic check ad config msg") & "</strong></font>"
            'Else
            If (setting_data.ADDomainName = "") Then
                domainValue.InnerHtml += m_refMsg.GetMessage("all domain select caption")
            ElseIf (setting_data.ADAuthentication = 2) Then
                domainValue.InnerHtml += m_refMsg.GetMessage("all domain select caption")
            Else
                domainValue.InnerHtml += setting_data.ADDomainName
            End If
            'End If
        End If
    End Function
    Private Sub ViewToolBar()
        Dim result As New System.Text.StringBuilder
        Try

            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("adconfig page title"))
            If ((AdValid = True) Or ((LCase(Request.ServerVariables("SERVER_NAME")) = "localhost") Or (Request.ServerVariables("SERVER_NAME") = "127.0.01")) And ((setting_data.ADAuthentication = 1) Or (setting_data.ADAuthentication = 2))) Then
                result.Append("<table><tr>")
                'If (domain_data.Length = 0) Then
                'result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentEdit.png", "adconfigure.aspx?action=edit", m_refMsg.GetMessage("alt edit settings button text"), m_refMsg.GetMessage("btn edit"), "Onclick=""javascript:alert('" & m_refMsg.GetMessage("javascript: alert cannot edit no domains") & "\n" & m_refMsg.GetMessage("generic check ad config msg") & "'); return false;"""))
                'Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentEdit.png", "adconfigure.aspx?action=edit", m_refMsg.GetMessage("alt edit settings button text"), m_refMsg.GetMessage("btn edit"), ""))
                'End If
                result.Append("<td>")
				result.Append(m_refStyle.GetHelpButton("viewadconfigure_ascx"))
                result.Append("</td>")
                result.Append("</tr></table>")
            End If

            divToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronEmpJSFuncJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
End Class
