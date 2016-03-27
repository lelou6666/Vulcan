Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkEnumeration
Imports Microsoft.Security.Application

Partial Class customroles
	Inherits System.Web.UI.UserControl
	'
	' Custom Role Manager
	'
	' This user control allows viewing, adding and deleting custom roles.
    '
    Protected m_strKeyWords As String = ""
    Protected m_strSelectedItem As String = "-1"
    Protected m_strSearchText As String = ""
#Region " Web Form Designer Generated Code "

	'This call is required by the Web Form Designer.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

	End Sub
	Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
		'CODEGEN: This method call is required by the Web Form Designer
		'Do not modify it using the code editor.
		InitializeComponent()
	End Sub
	Protected m_refSiteApi As New SiteAPI
	Protected m_refUserApi As New UserAPI
	Protected m_refContentApi As New ContentAPI
	Protected m_refStyle As New StyleHelper
	Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
	Protected AppImgPath As String = ""
	Protected ContentLanguage As Integer = -1
	Protected m_bEditing As Boolean = False
	Protected m_strAction As String = ""
	Protected m_strOperation As String = ""
    Protected m_nRoleId As Long = -1
	Protected m_strRoleName As String = ""
	Protected m_strRoleNames As String = ""
	'Protected m_strCustomRoleNames() As String
	Protected m_UserRolePermissionData() As UserRolePermissionData
    Protected m_strUpdateMode As String = ""

#End Region

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		'Put user code to initialize the page here
		m_refMsg = m_refSiteApi.EkMsgRef
		AppImgPath = m_refSiteApi.AppImgPath
		ContentLanguage = m_refSiteApi.ContentLanguage

		If (Not Request.Form.Item("manager_mode") Is Nothing) Then
			m_strUpdateMode = Request.Form.Item("manager_mode")
		End If
		If (Not Request.Form.Item("role_names") Is Nothing) Then
			m_strRoleNames = Request.Form.Item("role_names")
		End If

		If ((Not Request.QueryString("edit") Is Nothing) AndAlso (Request.QueryString("edit") = "1")) Then
			m_bEditing = True
		Else
			m_bEditing = False
		End If

		' Determine the role-id based on the action string:
		If (Not Request.QueryString("action") Is Nothing) Then
			m_strAction = Request.QueryString("action")
		End If
		'Select Case m_strAction
		'	'
		'	' TODO:
		'	'Case "custompermissionsadmin"
		'	'	m_nRoleId = CmsRoleIds
		'	'	m_strRoleName = "Custom Permissions-Admin"
		'	'Case "membershipadmin"
		'	'	m_nRoleId = CmsRoleIds.
		'	'	m_strRoleName = "Membership-Admin"
		'	'
		'	Case Else
		'		m_nRoleId = -1
		'End Select

		' Determine operation, viewing/adding/deleting Custom Role:
		If (Not Request.QueryString("operation") Is Nothing) Then
			m_strOperation = Request.QueryString("operation")
		End If

		Select Case m_strOperation
			Case ""
				ViewCustomRoles()
			Case "addcustomroles"
				If (ProcessUpdating(False)) Then
					ViewCustomRoles()
				Else
					AddCustomRole()
				End If
			Case "deletecustomroles"
				If (ProcessUpdating(True)) Then
					ViewCustomRoles()
				Else
					DeleteCustomRole()
				End If
			Case Else
		End Select

        RegisterResources()
		RunTest()
    End Sub
    Private Sub CollectSearchText()
        m_strKeyWords = Request.Form("txtSearch")
        m_strSelectedItem = Request.Form("searchlist")
        If (m_strSelectedItem = "-1") Then
            m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%' OR last_name like '%" & Quote(m_strKeyWords) & "%' OR user_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "last_name") Then
            m_strSearchText = " (last_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "first_name") Then
            m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_strSelectedItem = "user_name") Then
            m_strSearchText = " (user_name like '%" & Quote(m_strKeyWords) & "%')"
        End If
    End Sub
    Private Function Quote(ByVal KeyWords As String) As String
        Dim result As String = KeyWords
        If (KeyWords.Length > 0) Then
            result = KeyWords.Replace("'", "''")
        End If
        Return result
    End Function

	Public Function ProcessUpdating(ByVal bDeleting As Boolean) As Boolean
		Dim contObj As Ektron.Cms.Content.EkContent
		Dim strUserRoleNamesArray As String()
        Dim nIndex As Integer
		Dim retValue As Boolean = False
		contObj = m_refContentApi.EkContentRef()

		If (m_strRoleNames.Length) Then
			strUserRoleNamesArray = m_strRoleNames.Split(",")
			For nIndex = 0 To (strUserRoleNamesArray.GetLength(0) - 1)
				If (bDeleting) Then
					retValue = contObj.DeleteRolePermission(strUserRoleNamesArray(nIndex))
				Else
					retValue = contObj.AddRolePermission(strUserRoleNamesArray(nIndex))
				End If
			Next
		End If

		contObj = Nothing
		Return retValue
	End Function

	Public Sub ViewCustomRoles()
		Dim contObj As Ektron.Cms.Content.EkContent
        If (Page.IsPostBack AndAlso Request.Form(isSearchPostData.UniqueID) <> "") Then
            CollectSearchText()
        End If
		contObj = m_refContentApi.EkContentRef()
		m_UserRolePermissionData = contObj.GetAllRolePermissions()
		ViewCustomRolesToolBar()
		Populate_CustomRoleListingGrid()
	End Sub

	Private Sub ViewCustomRolesToolBar()
		Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("alt manage custom roles")) & m_strRoleName ' m_refMsg.GetMessage("view user groups msg"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/add.png", "roles.aspx?action=" & m_strAction & "&operation=addcustomroles&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt add custom roles"), m_refMsg.GetMessage("alt add custom roles"), ""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/remove.png", "roles.aspx?action=" & m_strAction & "&operation=deletecustomroles&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt delete custom role"), m_refMsg.GetMessage("alt delete custom role"), ""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        'result.Append("<td class=""label"">&nbsp;|&nbsp;<label for=""txtSearch"">" & m_refMsg.GetMessage("generic search") & "</label><input type=text class=""ektronTextMedium"" id=txtSearch name=txtSearch value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event)"">")
        'result.Append("<select id=searchlist name=searchlist>")
        'result.Append("<option value=-1" & IsSelected("-1") & ">All</option>")
        'result.Append("<option value=""last_name""" & IsSelected("last_name") & ">Last Name</option>")
        'result.Append("<option value=""first_name""" & IsSelected("first_name") & ">First Name</option>")
        'result.Append("<option value=""user_name""" & IsSelected("user_name") & ">User Name</option>")
        'result.Append("</select><input type=button class=""ektronWorkareaSearch"" value=""Search"" id=btnSearch name=btnSearch onclick=""searchuser();""></td>")
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("ViewCustomRoles"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Function IsSelected(ByVal val As String) As String
        If (val = m_strSelectedItem) Then
            Return (" selected ")
        Else
            Return ("")
        End If
    End Function

    Private Sub Populate_CustomRoleListingGrid(Optional ByVal bShowCheckBox As Boolean = False)
        Dim dt As New DataTable
        Dim idx As Integer
        Dim strName As String
        Dim nId As Long
        Dim strDesc As String
        Dim colBound As New System.Web.UI.WebControls.BoundColumn

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "CUSTOM_ROLE_NAME"
        colBound.HeaderText = "Custom Role Name"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        CustomRoleListingGrid.Columns.Add(colBound)

        dt = New DataTable
        Dim dr As DataRow
        Dim strTypeIcon As String = ""
        Dim strNameId As String
        dt.Columns.Add(New DataColumn("CUSTOM_ROLE_NAME", GetType(String)))

        If ((Not IsNothing(m_UserRolePermissionData)) AndAlso (m_UserRolePermissionData.GetLength(0) > 0)) Then

            For idx = 1 To m_UserRolePermissionData.GetLength(0) - 1
                strName = m_UserRolePermissionData(idx).RolePermissionName
                nId = m_UserRolePermissionData(idx).RolePermissionId
                strDesc = m_UserRolePermissionData(idx).RolePermissionDescription

                'strTypeIcon = IIf(m_RoleMembers(idx).MemberType = RoleMemberData.RoleMemberType.User, "user.png", "users.png")
                dr = dt.NewRow
                'dr(0) = IIf(bShowCheckBox, "<input type=""checkbox"" name=""frm_fixme"" id=""frm_fixme"">&nbsp;", "")
                If (bShowCheckBox) Then
                    strNameId = "member_user_id" & AntiXss.HtmlEncode(strName)
                    dr(0) = "&nbsp;<input type=""checkbox"" name=""" & strNameId & """ id=""" & strNameId & """>&nbsp;" & AntiXss.HtmlEncode(strName)
                Else
                    dr(0) = "&nbsp;<a href=""roles.aspx?action=custompermissions&LangType=" & ContentLanguage & "&id=" & nId & "&name=" & AntiXss.HtmlEncode(strName) & """ title='" & "Manage Custom Role Members" & "' onclick=""return;"">" & AntiXss.HtmlEncode(strName) & "</a>"
                End If
                dt.Rows.Add(dr)
            Next

        End If

        Dim dv As New DataView(dt)
        CustomRoleListingGrid.DataSource = dv
        CustomRoleListingGrid.DataBind()

    End Sub

    Public Sub AddCustomRole()
        Dim sb As New System.Text.StringBuilder
        Dim contObj As Ektron.Cms.Content.EkContent
        contObj = m_refContentApi.EkContentRef()

        AddCustomRoleToolBar()

        ' render UI:
        sb.Append("<table class=""ektronGrid"">")
        sb.Append("<tr>")
        sb.Append("<td class=""label"">")
        sb.Append("Name:")
        sb.Append("</td>")
        sb.Append("<td class=""value"">")
        sb.Append("<input type='text' id='name_text' name='name_text' size='30' />")
        sb.Append("</td>")
        sb.Append("</tr>")
        sb.Append("</table>")
        Literal1.Text = sb.ToString()
        'm_RoleMembers = contObj.GetAllNonRoleMembers(m_nRoleId)
        'Populate_Members_CustomRoleListingGrid(True)

        sb = Nothing
    End Sub

    Private Sub AddCustomRoleToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("alt manage custom roles"))  ' m_refMsg.GetMessage("view user groups msg"))
        result.Append("<table><tr>")
        'result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/contentEdit.png", "roles.aspx?action=" & m_strAction & "&edit=0&update=1", m_refMsg.GetMessage("btn edit"), m_refMsg.GetMessage("btn edit"), "OnClick=""javascript:return true;"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("btn save"), m_refMsg.GetMessage("btn save"), "OnClick=""javascript:submitAddMembers();return true;"""))

        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("AddCustomRoles"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Public Sub DeleteCustomRole()
        Dim contObj As Ektron.Cms.Content.EkContent

        contObj = m_refContentApi.EkContentRef()
        DeleteCustomRoleToolBar()
        m_UserRolePermissionData = contObj.GetAllRolePermissions()
        Populate_CustomRoleListingGrid(True)
    End Sub

    Private Sub DeleteCustomRoleToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("alt manage custom roles"))  ' m_refMsg.GetMessage("view user groups msg"))
        result.Append("<table><tr>")
        'result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/delete.png", "roles.aspx?action=" & m_strAction & "&edit=0&update=1", m_refMsg.GetMessage("btn edit"), m_refMsg.GetMessage("btn edit"), "OnClick=""javascript:return true;"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("btn delete"), m_refMsg.GetMessage("btn delete"), "OnClick=""javascript:submitdeletecustomrole();return true;"""))

        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("DeleteCustomRoles"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

	Private Sub RunTest()
        Dim nUserId As Long = 7 ' user "vs"
		Dim nFolderId As String = 74 ' folder: "ZapFolder1"
		Dim strRoleName As String = "test1"
		Dim bFlag As Boolean = False

		bFlag = m_refContentApi.GetRolePermissionSystem(strRoleName, nUserId)
		bFlag = m_refContentApi.GetRolePermissionFolder(strRoleName, nFolderId, nUserId)

    End Sub

    Private Sub RegisterResources()
        ' register JS

        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronInputLabelJS)
        API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareSearchBoxInputLabelInitJS")
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)

        ' register CSS
    End Sub

End Class
