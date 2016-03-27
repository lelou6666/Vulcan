Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Workarea

Partial Class blogs_addeditcomment
    Inherits workareabase

    Dim ctlEditor As New Ektron.Cms.Controls.HtmlEditor
    Dim m_iPostID As Long = 0
    Dim m_iBlogID As Long = 0
    Dim security_data As PermissionData
    Dim m_refContent As Ektron.Cms.Content.EkContent = New Ektron.Cms.Content.EkContent(m_refContentApi.RequestInformationRef)
    Dim m_reftask As Ektron.Cms.Content.EkTask = m_refContentApi.EkTaskRef
    Dim closeOnFinish As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        RegisterResources()
        If Request.QueryString("contentid") <> "" Then
            m_iPostID = Convert.ToInt64(Request.QueryString("contentid"))
        End If
        If Request.QueryString("blogid") <> "" Then
            m_iBlogID = Convert.ToInt64(Request.QueryString("blogid"))
        End If

        If Request.QueryString("close") <> "" Then
            closeOnFinish = True
        End If

        If Page.IsPostBack Then
            Select Case MyBase.m_sPageAction
                Case "add"
                    Process_Add()
                Case "edit"
                    Process_Edit()
            End Select
        Else
            Select Case MyBase.m_sPageAction
                Case "add"
                    Display_Add()
                Case "edit"
                    Display_Edit()
            End Select
        End If
    End Sub

#Region "Display"

    Private Sub Display_Add()
        Dim cConts As New Collection
        Dim udME As New UserData
        Dim uaUser As New UserAPI
        If Me.m_refContentApi.UserId > 0 Then
            udME = uaUser.GetUserById(Me.m_refContentApi.UserId, False, False)
        End If
        If Me.m_iPostID = 0 Then
            Me.m_iPostID = m_iID
            m_iID = 0
        End If

        If (Me.m_iPostID <> 0 And Me.m_iPostID <> -1) Then
            cConts = m_refContent.GetContentByIDv2_0(Me.m_iPostID)
            If (cConts.Count = 0) Then
                Throw New Exception(MyBase.GetMessage("error: post does not exist") & ".")
            Else
                ltr_post_data.Text = "(" & Me.m_iPostID.ToString() & ") " & cConts("ContentTitle")
            End If
        End If

        MyBase.SetTitleBarToMessage("btn comment add")
        MyBase.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", "alt save comment", "btn save", "OnClick=""javascript:SubmitForm();return false;""")
        If closeOnFinish <> True Then
            MyBase.AddBackButton("../content.aspx?action=ViewContentByCategory&id=" & m_iBlogID.ToString() & "&LangType=" & m_refContentApi.RequestInformationRef.ContentLanguage & "&ContType=" & Ektron.Cms.Common.EkEnumeration.TaskType.TopicReply & "&contentid=" & m_iPostID)
        End If
        MyBase.AddHelpButton("AddComment")

        'MyBase.Tabs.On()
        'MyBase.Tabs.AddTabByMessage("comment text", "dvContent")
        txt_displayname.Text = udME.DisplayName
        txt_email.Text = udME.Email
        txt_url.Text = "http://"
        SetLabels()
        rb_approved.Checked = True

        RenderJS()
    End Sub

    Public Sub Display_Edit()
        Dim content_data As ContentData
        m_reftask = m_reftask.GetTaskByID(m_iID)
        content_data = m_refContentApi.GetContentById(m_iPostID)
        security_data = m_refContentApi.LoadPermissions(m_iPostID, "content")
        If (Me.m_iPostID <> 0 And Me.m_iPostID <> -1) Then
            ltr_post_data.Text = "(" & Me.m_iPostID.ToString() & ") " & content_data.Title
        End If
        MyBase.SetTitleBarToMessage("lbl edit comment")
        MyBase.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", "alt save comment", "btn save", "OnClick=""javascript:SubmitForm();return false;""")
        MyBase.AddBackButton("../content.aspx?action=ViewContentByCategory&id=" & content_data.FolderId.ToString() & "&ContType=" & CMSContentType_BlogComments & "&LangType=" & m_refContentApi.ContentLanguage & "&contentid=" & m_iPostID.ToString())
        MyBase.AddHelpButton("EditComment")

        'MyBase.Tabs.On()
        'MyBase.Tabs.AddTabByMessage("comment text", "dvContent")

        SetLabels()

        If m_reftask.State = EkEnumeration.TaskState.Pending.GetHashCode Then
            rb_pending.Checked = True
        ElseIf m_reftask.State = EkEnumeration.TaskState.Completed Then
            rb_approved.Checked = True
        End If

        txt_displayname.Text = m_reftask.CommentDisplayName
        txt_email.Text = m_reftask.CommentEmail
        txt_url.Text = m_reftask.CommentURI
        txt_comment.Text = m_reftask.Description
        RenderJS()
    End Sub

#End Region

#Region "Process"
    Private Sub Process_Add()
        If ContentLanguage > 0 Then
            m_reftask.ContentLanguage = ContentLanguage
            m_reftask.LanguageID = ContentLanguage
        Else
            m_reftask.ContentLanguage = m_refContentApi.RequestInformationRef.DefaultContentLanguage
            m_reftask.LanguageID = m_refContentApi.RequestInformationRef.DefaultContentLanguage
        End If
        m_reftask.ContentID = m_iPostID
        m_reftask.AssignedByUserID = m_refContentApi.RequestInformationRef.UserId
        m_reftask.CreatedByUserID = m_refContentApi.RequestInformationRef.UserId
        m_reftask.DateCreated = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString()
        m_reftask.TaskTypeID = EkEnumeration.TaskType.BlogPostComment

        m_reftask.CommentDisplayName = Context.Request.Form(txt_displayname.UniqueID)
        m_reftask.CommentEmail = Context.Request.Form(txt_email.UniqueID)
        If (Context.Request.Form(txt_url.UniqueID).ToLower() = "http://") Then
            m_reftask.CommentURI = ""
        Else
            m_reftask.CommentURI = Ektron.Cms.Common.EkFunctions.FixExternalHyperlink(Context.Request.Form(txt_url.UniqueID))
        End If
        If rb_pending.Checked = True Then
            m_reftask.State = EkEnumeration.TaskState.Pending
        Else
            m_reftask.State = EkEnumeration.TaskState.Completed
        End If
        m_reftask.Description = Context.Request.Form(txt_comment.UniqueID)

        m_reftask.TaskTitle = "BlogComment"
        m_reftask.ImpersonateUser = True
        m_reftask.AddTask()

        If (closeOnFinish = True) Then
            Response.Redirect("../close.aspx", False)
        Else
            Response.Redirect("../content.aspx?id=" & m_iBlogID.ToString() & "&action=ViewContentByCategory&LangType=" & ContentLanguage.ToString() & "&ContType=" & EkEnumeration.TaskType.TopicReply & "&contentid=" & m_iPostID.ToString())
        End If
    End Sub

    Private Sub Process_Edit()
        m_reftask = m_reftask.GetTaskByID(m_iID)

        m_reftask.CommentDisplayName = Context.Request.Form(txt_displayname.UniqueID)
        m_reftask.CommentEmail = Context.Request.Form(txt_email.UniqueID)
        If (Context.Request.Form(txt_url.UniqueID).ToLower() = "http://") Then
            m_reftask.CommentURI = ""
        Else
            m_reftask.CommentURI = Ektron.Cms.Common.EkFunctions.FixExternalHyperlink(Context.Request.Form(txt_url.UniqueID))
        End If
        If (rb_pending.Checked = True) Then
            m_reftask.State = EkEnumeration.TaskState.Pending
        Else
            m_reftask.State = EkEnumeration.TaskState.Completed
        End If
        m_reftask.Description = Context.Request.Form(txt_comment.UniqueID)

        m_reftask.ImpersonateUser = True
        m_reftask.UpdateTask()

        If (closeOnFinish = True) Then
            Response.Redirect("../close.aspx", False)
        Else
            Response.Redirect("../content.aspx?id=" & m_iBlogID.ToString() & "&action=ViewContentByCategory&LangType=" & ContentLanguage.ToString() & "&ContType=" & EkEnumeration.TaskType.TopicReply & "&contentid=" & m_iPostID.ToString())
        End If
    End Sub
#End Region

#Region "Private Helpers"
    Private Sub SetLabels()
        ltr_displayname.Text = MyBase.GetMessage("display name label")
        ltr_email.Text = MyBase.GetMessage("generic email")
        ltr_url.Text = MyBase.GetMessage("lbl url")
        ltr_post.Text = MyBase.GetMessage("lbl blog post")
        ltr_status.Text = MyBase.GetMessage("lbl state")
        ltr_comment.Text = MyBase.GetMessage("comment text")

        rb_approved.Text = "&nbsp;" & MyBase.GetMessage("lbl approved")
        rb_pending.Text = "&nbsp;" & MyBase.GetMessage("lbl pending")
    End Sub


    Private Sub RenderJS()
        Dim sbJS As New StringBuilder
        sbJS.Append("<script type=""text/javascript"" >" & Environment.NewLine)
        sbJS.Append("function SubmitForm()" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("   if (Trim(document.getElementById('" & txt_displayname.UniqueID & "').value).length > 0) {" & Environment.NewLine)
        sbJS.Append("       if (Trim(document.getElementById('" & txt_comment.UniqueID & "').value).length > 0) {" & Environment.NewLine)
        sbJS.Append("           document.forms[0].submit();" & Environment.NewLine)
        sbJS.Append("       } else {" & Environment.NewLine)
        sbJS.Append("           alert('" & MyBase.GetMessage("js err comment") & "');" & Environment.NewLine)
        sbJS.Append("       }" & Environment.NewLine)
        sbJS.Append("   } else {" & Environment.NewLine)
        sbJS.Append("       alert('" & MyBase.GetMessage("js err display name") & "');" & Environment.NewLine)
        sbJS.Append("   } " & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)
        sbJS.Append("</script>" & Environment.NewLine)
        ltr_js.Text &= Environment.NewLine & sbJS.ToString()
    End Sub

    Protected Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub
#End Region

End Class
