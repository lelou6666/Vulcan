Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class editapprovalorder
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
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
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
    Protected ItemType As String = ""
    Protected content_data As ContentData
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        m_refMsg = m_refContentApi.EkMsgRef
        RegisterResources()
    End Sub
    Public Function EditApprovalOrder() As Boolean
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
        'If (Not (Request.QueryString("membership") Is Nothing)) Then
        '    If (Request.QueryString("membership").Trim.ToLower <> "") Then
        '        m_bMemberShip = Convert.ToBoolean(Request.QueryString("membership").Trim.ToLower)
        '    End If
        'End If
        'If (Not (Request.QueryString("base") Is Nothing)) Then
        '    m_strBase = Request.QueryString("base").Trim.ToLower
        'End If
        CurrentUserId = m_refContentApi.UserId
        AppImgPath = m_refContentApi.AppImgPath
        SitePath = m_refContentApi.SitePath
        EnableMultilingual = m_refContentApi.EnableMultilingual
        If (Not (Page.IsPostBack)) Then
            Display_EditApprovalOrder()
        Else
            Process_DoUpdateApprovalOrder()
        End If
    End Function
#Region "ACTION - UpdateApprovalOrder"
    Private Sub Process_DoUpdateApprovalOrder()
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Try
            m_refContent = m_refContentApi.EkContentRef
            pagedata = New Collection
            If (Request.QueryString("type") = "folder") Then
                pagedata.Add(Request.QueryString("id"), "FolderID")
                pagedata.Add("", "ContentID")
            Else
                pagedata.Add(Request.QueryString("id"), "ContentID")
                pagedata.Add("", "FolderID")
            End If
            pagedata.Add(Request.Form(ApprovalOrder.UniqueID), "ApprovalOrder")

            m_refContent.UpdateApprovalOrderv2_0(pagedata)

            Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewApprovals&id=" & Request.QueryString("id") & "&type=" & Request.QueryString("type"), False)

        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub
#End Region
#Region "APPROVAL - EditApprovalOrder"
	Private Sub Display_EditApprovalOrder()
		Dim bFolderUserAdmin As Boolean = False
		'FormAction = "content.aspx?LangType=" & m_intContentLanguage & "&action=DoUpdateApprovalOrder&id=" & m_intId & "&type=" & ItemType
		'SetPostBackPage()
		If (ItemType = "folder") Then
			folder_data = m_refContentApi.GetFolderById(m_intId)
		Else
			content_data = m_refContentApi.GetContentById(m_intId)
		End If
		Dim approval_data As ApprovalItemData()
		approval_data = m_refContentApi.GetItemApprovals(m_intId, ItemType)
		security_data = m_refContentApi.LoadPermissions(m_intId, ItemType)
		If (Not (folder_data Is Nothing)) Then
			bFolderUserAdmin = security_data.IsAdmin OrElse m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(folder_data.Id)
		Else
			If (Not (content_data Is Nothing)) Then
				bFolderUserAdmin = security_data.IsAdmin OrElse m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(content_data.FolderId)
			Else
				bFolderUserAdmin = security_data.IsAdmin
			End If
		End If
        If (Not (security_data.IsAdmin OrElse bFolderUserAdmin)) Then
            Throw New Exception(m_refMsg.GetMessage("error: user not permitted"))
        End If
		EditApprovalOrderToolbar()
		Dim strMsg As String = ""
		Dim i As Integer = 0
		If (Not (IsNothing(approval_data))) Then
			If (approval_data.Length < 20) Then
				ApprovalList.Rows = approval_data.Length
			End If
			For i = 0 To approval_data.Length - 1
				If (approval_data(i).UserId > 0) Then
					ApprovalList.Items.Add(New ListItem(approval_data(i).DisplayUserName, "user." & approval_data(i).UserId))
					If (strMsg.Length = 0) Then
						strMsg = "user." & approval_data(i).UserId
					Else
						strMsg += ",user." & approval_data(i).UserId
					End If
				Else
					ApprovalList.Items.Add(New ListItem(approval_data(i).DisplayUserGroupName, "group." & approval_data(i).GroupId))
					If (strMsg.Length = 0) Then
						strMsg = "group." & approval_data(i).GroupId
					Else
						strMsg += ",group." & approval_data(i).GroupId
					End If
				End If
			Next
		End If
		td_eao_link.InnerHtml = "<a href=""javascript:Move('up', document.forms[0]." & UniqueID & "_ApprovalList, document.forms[0]." & UniqueID & "_ApprovalOrder)"">"
        td_eao_link.InnerHtml += "<img src=""" & m_refContentApi.AppPath & "Images/ui/icons/arrowHeadUp.png"" valign=middle border=0 width=16 height=16 alt=""" & m_refMsg.GetMessage("move selection up msg") & """ title=""" & m_refMsg.GetMessage("move selection up msg") & """></a><br />"
		td_eao_link.InnerHtml += "<a href=""javascript:Move('dn', document.forms[0]." & UniqueID & "_ApprovalList, document.forms[0]." & UniqueID & "_ApprovalOrder)"">"
        td_eao_link.InnerHtml += "<img src=""" & m_refContentApi.AppPath & "Images/ui/icons/arrowHeadDown.png"" valign=middle border=0 width=16 height=16 alt=""" & m_refMsg.GetMessage("move selection down msg") & """ title=""" & m_refMsg.GetMessage("move selection down msg") & """></a>"

		td_eao_title.InnerHtml = m_refMsg.GetMessage("move within approvals msg")
        td_eao_msg.InnerHtml = "<label class=""label"">" & m_refMsg.GetMessage("first approver msg") & "</label>"
        td_eao_ordertitle.InnerHtml = "<h2>" & m_refMsg.GetMessage("approval order title") & "</h2>"
		ApprovalOrder.Value = strMsg
	End Sub
    Private Sub EditApprovalOrderToolbar()
        Dim result As New System.Text.StringBuilder
        Dim WorkareaTitlebarTitle As String = ""
        If (ItemType = "folder") Then
            WorkareaTitlebarTitle = m_refMsg.GetMessage("edit folder approvals msg") & " """ & folder_data.Name & """"
        Else
            WorkareaTitlebarTitle = m_refMsg.GetMessage("edit content approvals msg") & " """ & content_data.Title & """"
        End If
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle)
        result.Append("<table><tr>")
        result.Append("<td>" & m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (approvals)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('frmContent', '');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?LangType=" & ContentLanguage & "&action=ViewApprovals&id=" & m_intId & "&type=" & ItemType, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("</td>")
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString()

    End Sub

    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
#End Region
End Class
