Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class editpreapproval
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
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Common.EkMessageHelper
    Protected m_intId As Long = 0
    Protected folder_data As FolderData
    Protected security_data As PermissionData
    Protected AppImgPath As String = ""
    Protected ContentType As Integer = 1
    Protected CurrentUserId As Long = 0
    Protected pagedata As Collection
    Protected m_strPageAction As String = ""
    Protected m_strOrderBy As String = ""
    Protected ContentLanguage As Integer = -1
    Protected EnableMultilingual As Integer = 0
    Protected SitePath As String = ""
    Protected ItemType As String
    Protected content_data As ContentData
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        m_refMsg = m_refContentApi.EkMsgRef
        RegisterResources()
    End Sub
	Public Function EditPreApproval() As Boolean
		If (Not (Request.QueryString("type") Is Nothing)) Then
			ItemType = Convert.ToString(Request.QueryString("type")).Trim.ToLower
		End If
		If (Not (Request.QueryString("id") Is Nothing)) Then
            m_intId = Convert.ToInt64(Request.QueryString("id"))
		End If
		If (Not (Request.QueryString("action") Is Nothing)) Then
			m_strPageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
		End If
		If (Not (Request.QueryString("orderby") Is Nothing)) Then
			m_strOrderBy = Convert.ToString(Request.QueryString("orderby"))
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

		CurrentUserId = m_refContentApi.UserId
		AppImgPath = m_refContentApi.AppImgPath
		SitePath = m_refContentApi.SitePath
		EnableMultilingual = m_refContentApi.EnableMultilingual
		If (Not (Page.IsPostBack)) Then
			Display_EditPreApprovals()
		Else
			Process_DoEditPreApproval()
		End If
	End Function

#Region "ACTION -UpdatePreApproval"
	Private Sub Process_DoEditPreApproval()
		Dim m_refContent As Ektron.Cms.Content.EkContent
		Try
			m_refContent = m_refContentApi.EkContentRef
			pagedata = New Collection
			pagedata.Add(m_intId, "FolderID")
			pagedata.Add(CLng(Request.Form("selectusergroup")), "PreApprovalGroupID")
			m_refContent.UpdateFolderPreapproval(pagedata)
			Response.Redirect("content.aspx?action=ViewFolder&id=" & m_intId & "&LangType=" & ContentLanguage, False)
		Catch ex As Exception
			Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & ContentLanguage, False)
		End Try
	End Sub
#End Region

#Region "APPROVAL - Display_EditPreApprovals"
	Private Sub Display_EditPreApprovals()
		Dim m_intApprovalMethoad As Integer = 0
		Dim cPreApproval As Microsoft.VisualBasic.Collection
		Dim taskObj As Ektron.Cms.Content.EkTask
		Dim userGroups As Microsoft.VisualBasic.Collection
		Dim userGroup As Microsoft.VisualBasic.Collection

		cPreApproval = m_refContentApi.EkContentRef().GetFolderPreapprovalGroup(m_intId)
		taskObj = m_refContentApi.EkTaskRef
		'If Not taskObj Is Nothing Then
		userGroups = taskObj.GetUsersForTask(CurrentUserId, -1)
		'End If

		'FormAction = "content.aspx?action=DoEditApprovalMethod&id=" & m_intId & "&type=" & ItemType & "&LangType=" & m_intContentLanguage
		'SetPostBackPage()
		security_data = m_refContentApi.LoadPermissions(m_intId, ItemType)

		If (ItemType = "folder") Then
			folder_data = m_refContentApi.GetFolderById(m_intId)
			m_intApprovalMethoad = folder_data.ApprovalMethod
		Else
			content_data = m_refContentApi.GetContentById(m_intId)
			m_intApprovalMethoad = content_data.ApprovalMethod
		End If
		EditApprovalsToolBar()
		lit_select_preapproval.Text = "<option value=""-1"" "
		If (-1 = cPreApproval("PreApprovalGroupID")) Then
			lit_select_preapproval.Text += " selected"
		End If
		lit_select_preapproval.Text += ">"
		lit_select_preapproval.Text += "(Inherit)</option>"
		lit_select_preapproval.Text += "<option value=""0"" "
		If (0 = cPreApproval("PreApprovalGroupID")) Then
			lit_select_preapproval.Text += " selected"
		End If
		lit_select_preapproval.Text += ">"
		lit_select_preapproval.Text += "(None)</option>"
		For Each userGroup In userGroups
			If (IsNumeric(userGroup("UserGroupID"))) Then
				lit_select_preapproval.Text += "<option value=" & userGroup("UserGroupID")
				If ((userGroup("UserGroupID") = cPreApproval("PreApprovalGroupID"))) Then
					lit_select_preapproval.Text += " selected "
				End If
				lit_select_preapproval.Text += ">"
				lit_select_preapproval.Text += userGroup("DisplayUserGroupName") & "</option>"
			End If
		Next
	End Sub

	Private Sub EditApprovalsToolBar()
		Dim result As New System.Text.StringBuilder
		Dim WorkareaTitlebarTitle As String = ""
		WorkareaTitlebarTitle = "edit properties for folder msg"
		txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle) & " """ & folder_data.Name & """"
        result.Append("<table><tr>")

        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (folder)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('editfolder', '');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?action=ViewFolder&id=" & m_intId & "&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))

		result.Append("<td>")
		result.Append(m_refStyle.GetHelpButton("EditPreApproval"))
		result.Append("</td>")
		result.Append("</tr></table>")
		htmToolBar.InnerHtml = result.ToString()
	End Sub
#End Region
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
End Class
