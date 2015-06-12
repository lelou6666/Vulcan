Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class viewapprovals
    Inherits System.Web.UI.UserControl

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    'Protected WithEvents jsAction As System.Web.UI.WebControls.Literal
    'Protected WithEvents jsType As System.Web.UI.WebControls.Literal
    'Protected WithEvents jsId As System.Web.UI.WebControls.Literal

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub
    Protected IsApprovalChainExists As Boolean = False
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
    Public Function ViewApproval() As Boolean
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
        jsType.Text = ItemType
        jsId.Text = m_intId
        jsAction.Text = m_strPageAction
        CurrentUserId = m_refContentApi.UserId
        AppImgPath = m_refContentApi.AppImgPath
        SitePath = m_refContentApi.SitePath
        EnableMultilingual = m_refContentApi.EnableMultilingual

        Display_ViewApprovals()

    End Function
#Region "APPROVAL - ViewApprovals"
    Private Sub Display_ViewApprovals()
        Dim approval_data() As ApprovalItemData
        security_data = m_refContentApi.LoadPermissions(m_intId, ItemType)

        Dim IsInherited As Boolean = False
        Dim intApprovalMethod As Integer = 0
        If (ItemType = "folder") Then
            folder_data = m_refContentApi.GetFolderById(m_intId)
            intApprovalMethod = folder_data.ApprovalMethod
            IsInherited = folder_data.Inherited
        Else
            content_data = m_refContentApi.GetContentById(m_intId)
            intApprovalMethod = content_data.ApprovalMethod
            IsInherited = content_data.IsInherited
        End If

        approval_data = m_refContentApi.GetItemApprovals(m_intId, ItemType)
        If (Not approval_data Is Nothing) Then
            If (approval_data.Length > 0) Then
                IsApprovalChainExists = True
            End If
        End If
        ViewApprovalToolBar()
        If (IsInherited) Then
            lblInherited.Text = m_refMsg.GetMessage("approval chain inherited msg")
        Else
            pnlInherited.Visible = False
        End If

        If (intApprovalMethod = 1) Then
            lblMethod.Text = m_refMsg.GetMessage("display for force all approvers")
        Else
            lblMethod.Text = m_refMsg.GetMessage("display for do not force all approvers")
        End If
        Populate_ViewApprovalsGrid(approval_data)

    End Sub
    Private Sub Populate_ViewApprovalsGrid(ByVal approval_data As ApprovalItemData())
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = m_refMsg.GetMessage("user or group name title")
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        ViewApprovalsGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = m_refMsg.GetMessage("generic ID")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.Wrap = False
        ViewApprovalsGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ORDER"
        colBound.HeaderText = m_refMsg.GetMessage("approval order title")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.Wrap = False
        ViewApprovalsGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("ORDER", GetType(String)))


        Dim bInherited As Boolean = False
        If (ItemType = "folder") Then
            bInherited = folder_data.Inherited
        Else
            bInherited = content_data.IsInherited
        End If
        Dim i As Integer
        If (Not (IsNothing(approval_data))) Then
            For i = 0 To approval_data.Length - 1
                dr = dt.NewRow()
                If (approval_data(i).UserId <> 0) Then
                    dr(0) = "<img class=""imgUsers"" src=""" & m_refContentApi.AppPath & "images/UI/Icons/user.png"" />" & approval_data(i).DisplayUserName
                    dr(1) = approval_data(i).UserId
                Else
                    dr(0) = "<img class=""imgUsers"" src=""" & m_refContentApi.AppPath & "images/UI/Icons/users.png"" />" & approval_data(i).DisplayUserGroupName
                    dr(1) = approval_data(i).GroupId
                End If
                dr(2) = approval_data(i).ApprovalOrder

                dt.Rows.Add(dr)
            Next
        End If

        Dim dv As New DataView(dt)
        ViewApprovalsGrid.DataSource = dv
        ViewApprovalsGrid.DataBind()
    End Sub
    Private Sub ViewApprovalToolBar()
        Dim result As New System.Text.StringBuilder
        Dim bInherited As Boolean = False
        Dim WorkareaTitlebarTitle As String = ""
		Dim bFolderUserAdmin As Boolean

		If (Not (folder_data Is Nothing)) Then
			bFolderUserAdmin = security_data.IsAdmin OrElse m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(folder_data.Id)
		Else
			If (Not (content_data Is Nothing)) Then
				bFolderUserAdmin = security_data.IsAdmin OrElse m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(content_data.FolderId)
			Else
				bFolderUserAdmin = security_data.IsAdmin
			End If
		End If
		If (ItemType = "folder") Then
			bInherited = folder_data.Inherited
		Else
			bInherited = content_data.IsInherited
		End If
		If (ItemType = "folder") Then
			WorkareaTitlebarTitle = m_refMsg.GetMessage("view folder approvals msg") & " """ & folder_data.Name & """"
		Else
			WorkareaTitlebarTitle = m_refMsg.GetMessage("view content approvals msg") & " """ & content_data.Title & """"
		End If
		txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle)
        result.Append("<table><tr>")
		If (bFolderUserAdmin And (bInherited = False)) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/add.png", "content.aspx?LangType=" & ContentLanguage & "&action=AddApproval&id=" & m_intId & "&type=" & ItemType, m_refMsg.GetMessage("alt add button text (approvals)"), m_refMsg.GetMessage("btn add"), ""))
            If (IsApprovalChainExists) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/delete.png", "content.aspx?LangType=" & ContentLanguage & "&action=DeleteApproval&id=" & m_intId & "&type=" & ItemType, m_refMsg.GetMessage("alt delete button text (approvals)"), m_refMsg.GetMessage("btn delete"), ""))
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/arrowUpDown.png", "content.aspx?LangType=" & ContentLanguage & "&action=EditApprovalOrder&id=" & m_intId & "&type=" & ItemType, m_refMsg.GetMessage("alt edit button text (approvals)"), m_refMsg.GetMessage("btn reorder"), ""))
            End If
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentEdit.png", "content.aspx?LangType=" & ContentLanguage & "&action=EditApprovalMethod&id=" & m_intId & "&type=" & ItemType, "Edit Approval Method", m_refMsg.GetMessage("btn edit"), ""))
        End If
        If (ItemType = "folder") Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?action=ViewFolder&id=" & m_intId & "&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If
        If EnableMultilingual = "1" Then
            Dim m_refsite As SiteAPI
            m_refsite = New SiteAPI
            Dim language_data() As LanguageData
            language_data = m_refsite.GetAllActiveLanguages
            Dim count As Integer = 0
            If CStr(ItemType) = CStr("folder") Then
                result.Append("<td class=""label""> | " & m_refMsg.GetMessage("content language label") & ":&nbsp;")
                result.Append("<select id=""selLang"" name=""selLang"" OnChange=""javascript:LoadApproval('frmContent');"">")
                For count = 0 To language_data.Length - 1
                    If (language_data(count).Id = ContentLanguage) Then
                        result.Append("<option value=" & language_data(count).Id & " selected>" & language_data(count).Name & "</option>")
                    Else
                        result.Append("<option value=" & language_data(count).Id & ">" & language_data(count).Name & "</option>")
                    End If
                Next
                result.Append("</select></td>")
            End If
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString()

	End Sub
#End Region

    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
End Class
