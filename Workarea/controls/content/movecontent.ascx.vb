Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common

Partial Class movecontent
    Inherits System.Web.UI.UserControl

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
    Protected m_refContent As Ektron.Cms.Content.EkContent
    Protected selectvalue As System.Text.StringBuilder
    Protected cAllFolders As Collection
    Protected content_data As ContentData
    Protected CallerPage As String = ""
    Protected m_rootFolderIsXml As Integer = 0
    Protected m_intContentType As Integer = 0
    Protected m_strContentStatus As String = "A"
    Protected _initIsFolderAdmin As Boolean = False
    Protected _isFolderAdmin As Boolean = False
    Protected _initIsCopyOrMoveAdmin As Boolean = False
    Protected _isCopyOrMoveAdmin As Boolean = False

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        m_refMsg = m_refContentApi.EkMsgRef
        InitFolderIsXmlFlags()
        Dim folderid As Long = 0
        If (Not (Request.QueryString("folder_id") Is Nothing)) Then
            folderid = Request.QueryString("folder_id")
            folder_data = m_refContentApi.GetFolderById(folderid)
        End If
        RegisterResources()
        Util_SetServerVariables()
    End Sub

    Private Function IsFolderAdmin() As Boolean
        If (_initIsFolderAdmin OrElse IsNothing(folder_data)) Then
            Return _isFolderAdmin
        End If
        _isFolderAdmin = m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(folder_data.Id)
        _initIsFolderAdmin = True
        Return _isFolderAdmin
    End Function

    Private Function IsCopyOrMoveAdmin() As Boolean
        If (_initIsCopyOrMoveAdmin OrElse IsNothing(folder_data)) Then
            Return _isCopyOrMoveAdmin
        End If
        _isCopyOrMoveAdmin = m_refContentApi.IsARoleMemberForFolder(EkEnumeration.CmsRoleIds.MoveOrCopy, folder_data.Id, m_refContentApi.UserId)
        _initIsCopyOrMoveAdmin = True
        Return _isCopyOrMoveAdmin
    End Function

    Public Function MoveContent() As Boolean
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
        m_refContent = m_refContentApi.EkContentRef
        If (Not (Request.QueryString("page") Is Nothing)) Then
            CallerPage = Request.QueryString("page").Trim.ToLower
        End If
        If (Not (Page.IsPostBack)) Then
            Display_MoveContent()
        Else
            Process_DoMoveContent()
        End If
    End Function
#Region "ACTION - DoMoveContent"
    Private Sub Process_DoMoveContent()
        Dim strContentIds As String = ""
        Dim intMoveFolderId As Long = 0
        Dim FolderPath As String = ""
        Dim contCount As Integer = 0
        Dim langCount As Integer = 0

        Try
            strContentIds = Request.Form(contentids.UniqueID)
            m_refContent = m_refContentApi.EkContentRef
            intMoveFolderId = m_refContent.GetFolderID(Request.Form("move_folder_id"))

            If Request.Form(hdnCopyAll.UniqueID) = "true" Then
                Dim publishContent As Boolean = False
                If (Not Request.Form("btn_PublishCopiedContent") Is Nothing AndAlso Request.Form("btn_PublishCopiedContent").ToString() = "on") Then
                    publishContent = True
                End If
                m_refContent.CopyAllLanguageContent(m_intId, intMoveFolderId, publishContent)
            Else
            If (Not Request.Form(RadBtnMoveCopyValue.UniqueID) Is Nothing AndAlso Request.Form(RadBtnMoveCopyValue.UniqueID).ToString() = "copy") Then
                Dim bPublish As Boolean = False
                If (Not Request.Form("btn_PublishCopiedContent") Is Nothing AndAlso Request.Form("btn_PublishCopiedContent").ToString() = "on") Then
                    bPublish = True
                End If
                m_refContent.CopyContentByID(strContentIds, ContentLanguage, intMoveFolderId, bPublish)
            Else
                m_refContent.MoveContent(strContentIds, ContentLanguage, intMoveFolderId)
            End If
            End If

            FolderPath = m_refContent.GetFolderPath(intMoveFolderId)
            If (Right(FolderPath, 1) = "\") Then
                FolderPath = Right(FolderPath, Len(FolderPath) - 1)
            End If
            FolderPath = Replace(FolderPath, "\", "\\")

            If CallerPage.ToLower() = "webpage" Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), "CloseScript", GetCloseScript())
            ElseIf ((CallerPage <> "content.aspx") AndAlso (CallerPage <> "cmsform.aspx") AndAlso (CallerPage <> "")) Then    'TODO:Verify
                If (CallerPage.Trim.Length = 0) Then
                    CallerPage = "Cmsform.aspx"
                End If
                Response.Redirect(CallerPage & "?LangType=" & ContentLanguage & "&action=ViewAllFormsByFolderID&folder_id=" & intMoveFolderId & "&reloadtrees=Content,Library&TreeNav=" & FolderPath, False)
            Else
                Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewContentByCategory&id=" & intMoveFolderId & "&reloadtrees=Content,Library&TreeNav=" & FolderPath, False)
            End If
        Catch ex As Exception
            Dim intError As Integer
            Dim strError As String
            strError = "because a record with the same title exists in the destination folder"
            intError = ex.Message.IndexOf(strError)
            If intError > -1 Then
                strError = Left(ex.Message, intError + strError.Length)
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(strError & ".") & "&LangType=" & ContentLanguage, False)
            Else
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & ContentLanguage, False)
            End If

        End Try
    End Sub
    Public Function GetCloseScript() As String
        Dim sb As New StringBuilder()
        sb.Append("<script language=""javascript"">")
        sb.Append("top.close();<")
        sb.Append("/script>")
        Return sb.ToString()
    End Function
#End Region
#Region "CONTENT - MoveContent"
    Private Sub Display_MoveContent()
        Dim bIsError As Boolean = False
        Try
            content_data = m_refContentApi.GetContentById(m_intId)
        Catch ex As Exception
            bIsError = True
        End Try
        If (bIsError) Then
            folder_data = m_refContentApi.GetFolderById(m_intId)
        Else
            If (Not (content_data Is Nothing)) Then
                folder_data = m_refContentApi.GetFolderById(content_data.FolderId())
            Else
                folder_data = m_refContentApi.GetFolderById(m_intId)
            End If
        End If
        security_data = m_refContentApi.LoadPermissions(m_intId, "content")
        If (InStr(CallerPage, "content.aspx") > 0) Then
            CallerPage = "" 'TODO: I am setting it "" because move content works only within folders
        End If
        MoveContentToolBar()

        selectvalue = New System.Text.StringBuilder 'POPULATE FOLDER LIST
        selectvalue.Append("" & m_refMsg.GetMessage("lbl destination folder") & ":&nbsp;	<span style=""white-space:nowrap""><input id=""move_folder_id"" name=""move_folder_id"" size=""50%"" value=""\"" readonly=""true""/>  <a class=""button buttonInline greenHover buttonCheckAll"" href=""#"" onclick=""javascript:LoadSelectFolderChildPage();return true;"">" & m_refMsg.GetMessage("lbl select folder") & "</a></span>")

        m_refContent = m_refContentApi.EkContentRef
        source_folder_is_xml.Value = IIf((Not (folder_data.XmlConfiguration Is Nothing)), 1, 0)

        tdMoveToFolderList.InnerHtml = selectvalue.ToString
        contentids.Value = content_data.Id
        m_intContentType = content_data.Type
        m_strContentStatus = content_data.Status
        Populate_MoveContent(content_data)
    End Sub
    Private Sub Populate_MoveContent(ByVal content_data As ContentData)
        Dim colBound As System.Web.UI.WebControls.BoundColumn
        colBound = New System.Web.UI.WebControls.BoundColumn

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = m_refMsg.GetMessage("generic Title")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        MoveContentGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = m_refMsg.GetMessage("generic ID")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        MoveContentGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STATUS"
        colBound.HeaderText = m_refMsg.GetMessage("generic Status")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.Wrap = False
        MoveContentGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DATEMODIFIED"
        colBound.HeaderText = m_refMsg.GetMessage("generic Date Modified")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        MoveContentGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITORNAME"
        colBound.HeaderText = m_refMsg.GetMessage("generic Last Editor")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        MoveContentGrid.Columns.Add(colBound)


        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(Int64)))
        dt.Columns.Add(New DataColumn("STATUS", GetType(String)))
        dt.Columns.Add(New DataColumn("DATEMODIFIED", GetType(String)))
        dt.Columns.Add(New DataColumn("EDITORNAME", GetType(String)))

        dr = dt.NewRow()
        dr(0) = "<a href=""content.aspx?LangType=" & content_data.LanguageId & "&action=View&id=" & content_data.Id & """ title='" & m_refMsg.GetMessage("generic View") & " """ & Replace(content_data.Title, "'", "`") & """)" & "'>" & content_data.Title & "</a>"
        dr(1) = content_data.Id
        dr(2) = content_data.Status
        dr(3) = content_data.DisplayLastEditDate
        dr(4) = content_data.EditorLastName
        dt.Rows.Add(dr)

        Dim dv As New DataView(dt)
        MoveContentGrid.DataSource = dv
        MoveContentGrid.DataBind()
    End Sub

    Private Sub MoveContentToolBar()
        Dim result As New System.Text.StringBuilder
        If folder_data.FolderType = 9 Then
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("btn just move Content") & " """ & content_data.Title & """")
        Else
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("btn move Content") & " """ & content_data.Title & """")
        End If
        result.Append("<table><tr>")
        If (security_data.IsAdmin OrElse IsFolderAdmin()  OrElse IsCopyOrMoveAdmin()) Then
            If folder_data.FolderType = 9 Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentCopy.png", "#", m_refMsg.GetMessage("btn just move content"), m_refMsg.GetMessage("btn just move content"), "OnClick=""javascript:checkMoveForm_Content('" & m_refContent.GetFolderPath(content_data.FolderId).Replace("\", "\\") & "');return false;"""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentCopy.png", "#", m_refMsg.GetMessage("btn move content"), m_refMsg.GetMessage("btn move content"), "OnClick=""javascript:checkMoveForm_Content('" & m_refContent.GetFolderPath(content_data.FolderId).Replace("\", "\\") & "');return false;"""))
            End If
        End If
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back();", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub InitFolderIsXmlFlags()
        Dim fldrData As FolderData
        Dim nFolderID As Long = 0

        fldrData = m_refContentApi.GetFolderById(0)
        m_rootFolderIsXml = IIf((Not (fldrData.XmlConfiguration Is Nothing)), 1, 0)

        If (Not (Request.QueryString("id") Is Nothing)) Then
            nFolderID = Convert.ToInt64(Request.QueryString("id"))
        End If

        Dim bIsError As Boolean = False
        Try
            content_data = m_refContentApi.GetContentById(m_intId)
        Catch ex As Exception
            bIsError = True
        End Try
        If (bIsError) Then
            fldrData = m_refContentApi.GetFolderById(m_intId)
        Else
            If (Not (content_data Is Nothing)) Then
                fldrData = m_refContentApi.GetFolderById(content_data.FolderId())
            Else
                fldrData = m_refContentApi.GetFolderById(m_intId)
            End If
        End If
        source_folder_is_xml.Value = IIf((Not (fldrData.XmlConfiguration Is Nothing)), 1, 0)
    End Sub
#End Region
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
    Private Sub Util_SetServerVariables()
        jsConfirmCopyAll.text = m_refMsg.GetMessage("jsconfirm copy all")
    End Sub
End Class