Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class deleteapproval
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
    Protected content_data As ContentData
    Protected ItemType As String = ""
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        m_refMsg = m_refContentApi.EkMsgRef
        RegisterResources()
    End Sub
    Public Function DeleteApproval() As Boolean
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
            Display_DeleteApproval()
        End If
    End Function
#Region "APPROVAL - DeleteApproval"
    Private Sub Display_DeleteApproval()
        If (ItemType = "folder") Then
            folder_data = m_refContentApi.GetFolderById(m_intId)
        Else
            content_data = m_refContentApi.GetContentById(m_intId)
        End If
        Dim approval_data() As ApprovalItemData
        approval_data = m_refContentApi.GetItemApprovals(m_intId, ItemType)
        security_data = m_refContentApi.LoadPermissions(m_intId, ItemType)
        DeleteApprovalToolBar()
        Populate_DeleteApproval(approval_data)
    End Sub
    Private Sub Populate_DeleteApproval(ByVal approval_data As ApprovalItemData())
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = m_refMsg.GetMessage("user or group name title")
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        DeleteApprovalGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = m_refMsg.GetMessage("generic ID")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        DeleteApprovalGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ORDER"
        colBound.HeaderText = m_refMsg.GetMessage("approval order title")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        DeleteApprovalGrid.Columns.Add(colBound)

        DeleteApprovalGrid.BorderColor = Drawing.Color.White

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
                    dr(0) = "<a href=""content.aspx?LangType=" & ContentLanguage & "&action=DoDeleteItemApproval&item_id=" & approval_data(i).UserId & "&base=user&id=" & m_intId & "&type=" & ItemType & """ title=""" & m_refMsg.GetMessage("delete user from approvals") & """ OnClick=""javascript:return ConfirmDeleteApprovals('user');"">"
                    dr(0) += "<img class=""imgUsers"" src=""" & m_refContentApi.AppPath & "images/UI/Icons/user.png"" align=""absbottom"" alt=""" & m_refMsg.GetMessage("delete user from approvals") & """ title=""" & m_refMsg.GetMessage("delete user from approvals") & """/>" & approval_data(i).DisplayUserName & "</a>"
                    dr(1) = approval_data(i).UserId
                Else
                    dr(0) = "<a href=""content.aspx?LangType=" & ContentLanguage & "&action=DoDeleteItemApproval&item_id=" & approval_data(i).GroupId & "&base=group&id=" & m_intId & "&type=" & ItemType & """ title=""" & m_refMsg.GetMessage("delete usergroup from approvals") & """ OnClick=""javascript:return ConfirmDeleteApprovals('group');"">"
                    dr(0) += "<img class=""imgUsers"" src=""" & m_refContentApi.AppPath & "images/UI/Icons/users.png"" align=""absbottom"" alt=""" & m_refMsg.GetMessage("delete usergroup from approvals") & """ title=""" & m_refMsg.GetMessage("delete usergroup from approvals") & """/>" & approval_data(i).DisplayUserGroupName & "</a>"
                    dr(1) = approval_data(i).GroupId
                End If
                dr(2) = approval_data(i).ApprovalOrder

                dt.Rows.Add(dr)
            Next
        End If

        Dim dv As New DataView(dt)
        DeleteApprovalGrid.DataSource = dv
        DeleteApprovalGrid.DataBind()

    End Sub
    Private Sub DeleteApprovalToolBar()
        Dim result As New System.Text.StringBuilder
        Dim WorkareaTitlebarTitle As String
        If (ItemType = "folder") Then
            WorkareaTitlebarTitle = m_refMsg.GetMessage("delete folder approval msg") & " """ & folder_data.Name & """"
        Else
            WorkareaTitlebarTitle = m_refMsg.GetMessage("delete content approval msg") & " """ & content_data.Title & """"
        End If
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle)
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?LangType=" & ContentLanguage & "&action=ViewApprovals&id=" & m_intId & "&type=" & ItemType, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
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
