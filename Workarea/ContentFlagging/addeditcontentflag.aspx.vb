Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea

Partial Class addeditcontentflag
    Inherits workareabase

	Protected m_sPage As String = "addeditcontentflag.aspx"
    Protected content_id As Long = 0
    Protected security_data As PermissionData
    Protected cfFlag As Ektron.Cms.ContentFlagData
    Protected aFlags() As Ektron.Cms.ContentFlagData = Array.CreateInstance(GetType(Ektron.Cms.ContentFlagData), 0)
    Protected fdFlagSet As New FlagDefData

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.QueryString("fid") <> "" Then
                m_iID = Convert.ToInt64(Request.QueryString("fid"))
            End If
            If Request.QueryString("cid") <> "" Then
                content_id = Convert.ToInt64(Request.QueryString("cid"))
            End If

            CheckPermissions()

            If Page.IsPostBack Then
                Select Case MyBase.m_sPageAction
                    Case Else ' "edit"
                        Process_Edit()
                End Select
            Else
                cfFlag = Me.m_refContentApi.EkContentRef.GetContentFlag(m_iID)
                RenderJS()
                Select Case MyBase.m_sPageAction
                    Case "delete"
                        Process_Delete()
                    Case "edit"
                        Display_Edit()
                    Case Else ' "view"
                        Display_View()
                End Select
                SetLabels()
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Protected Sub Process_Edit()
        cfFlag = Me.m_refContentApi.EkContentRef.GetContentFlag(Me.m_iID)
        If cfFlag.EntryId > 0 Then
            cfFlag.FlagComment = Me.txt_comment.Text
            cfFlag.FlagId = drp_flag_data.SelectedValue
            cfFlag = Me.m_refContentApi.EkContentRef.UpdateContentFlag(cfFlag)
        End If
        Dim pagemode As String = "&page=" & Request.QueryString("page")
        Response.Redirect(m_sPage & "?action=view&id=" & Me.m_iID & "&cid=" & Me.content_id & pagemode, False)
    End Sub
    Protected Sub Process_Delete()
        cfFlag = Me.m_refContentApi.EkContentRef.GetContentFlag(Me.m_iID)
        If cfFlag.EntryId > 0 Then
            Me.m_refContentApi.EkContentRef.DeleteContentFlag(cfFlag.EntryId)
        End If
        If (Request.QueryString("page") = "workarea") Then
            ' redirect to workarea
            Response.Write("<script language=""Javascript"">" & _
                                   "top.switchDesktopTab();" & _
                                   "</script>")
        Else
            Response.Redirect("../ContentStatistics.aspx?page=ContentStatistics.aspx&id=" & Me.content_id & "&LangType=" & Me.ContentLanguage, False)
        End If
    End Sub
    Protected Sub Display_Edit()
        If cfFlag.FlaggedUser.Id = 0 Then
            ltr_uname_data.Text = MyBase.GetMessage("lbl anon")
        Else
            ltr_uname_data.Text = cfFlag.FlaggedUser.Username
        End If
        ltr_date_data.Text = cfFlag.FlagDate.ToLongDateString & " " & cfFlag.FlagDate.ToShortTimeString
        txt_comment.Text = Server.HtmlDecode(cfFlag.FlagComment)

        'fdFlagSet = cfFlag.FlagDefinition
        fdFlagSet = Me.m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(cfFlag.FlagDefinition.ID, True)

        For i As Integer = 0 To (fdFlagSet.Items.Length - 1)
            drp_flag_data.Items.Add(New ListItem(Server.HtmlDecode(fdFlagSet.Items(i).Name), fdFlagSet.Items(i).ID))
            If fdFlagSet.Items(i).ID = cfFlag.FlagID Then
                drp_flag_data.SelectedIndex = i
            End If
        Next
    End Sub
    Protected Sub Display_View()
        If cfFlag.FlaggedUser.Id = 0 Then
            ltr_uname_data.Text = MyBase.GetMessage("lbl anon")
        Else
            ltr_uname_data.Text = cfFlag.FlaggedUser.Username
        End If
        ltr_date_data.Text = cfFlag.FlagDate.ToLongDateString & " " & cfFlag.FlagDate.ToShortTimeString
        txt_comment.Text = Server.HtmlDecode(cfFlag.FlagComment)

        'fdFlagSet = cfFlag.FlagDefinition
        fdFlagSet = Me.m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(cfFlag.FlagDefinition.ID, True)

        For i As Integer = 0 To (fdFlagSet.Items.Length - 1)
            drp_flag_data.Items.Add(New ListItem(Server.HtmlDecode(fdFlagSet.Items(i).Name), fdFlagSet.Items(i).ID))
            If fdFlagSet.Items(i).ID = cfFlag.FlagID Then
                drp_flag_data.SelectedIndex = i
            End If
        Next
    End Sub
    Protected Sub CheckPermissions()
        security_data = Me.m_refContentApi.LoadPermissions(Me.content_id, "content")
        Select Case MyBase.m_sPageAction
            Case "edit"
                If security_data.CanEdit = True Then
                    ' we are good
                Else
                    Throw New Exception(Me.GetMessage("err no perm edit"))
                End If
            Case Else ' "view"
                If security_data.IsReadOnly = True Then
                    ' we are good
                Else
                    Throw New Exception(Me.GetMessage("err no perm view"))
                End If
        End Select
    End Sub
    Protected Sub SetLabels()
        Me.ltr_date.Text = Me.GetMessage("generic datecreated") & ":"
        Me.ltr_uname.Text = Me.GetMessage("generic username") & ":"
        Me.ltr_flag.Text = Me.GetMessage("flag label") & ":"
        Me.ltr_comment.Text = Me.GetMessage("comment text") & ":"
        Dim m_refStyle As New StyleHelper
        Dim pagemode As String = "&page=" & Request.QueryString("page")
        Select Case MyBase.m_sPageAction
            Case "edit"
                MyBase.SetTitleBarToMessage("generic edit title")
                MyBase.AddButtonwithMessages(AppImgPath & "../UI/Icons/save.png", "#", "alt save button text (flag)", "btn save", "OnClick=""javascript:SubmitForm();return true;""")
                MyBase.AddBackButton(m_sPage & "?action=view&id=" & Me.m_iID & "&cid=" & Me.content_id & pagemode)
            Case Else ' "view"
                Me.drp_flag_data.Enabled = False
                Me.txt_comment.Enabled = False
                MyBase.SetTitleBarToMessage("generic view")
                If security_data.CanEdit = True Then
                    MyBase.AddButtonwithMessages(AppImgPath & "../UI/Icons/contentEdit.png", m_sPage & "?action=edit&id=" & Me.m_iID & "&cid=" & Me.content_id & pagemode, "alt edit button text", "btn edit", "")
                    MyBase.AddButtonwithMessages(AppImgPath & "../UI/Icons/delete.png", m_sPage & "?action=delete&id=" & Me.m_iID & "&cid=" & Me.content_id & pagemode, "btn alt del flag", ("btn delete"), " onclick=""javascript:return confirm('" & MyBase.GetMessage("js conf del flag") & "');"" ")
                End If
                If (Request.QueryString("page") = "workarea") Then
                    ' redirect to workarea when user clicks back button if we're in workarea
                    MyBase.AddButtonwithMessages(AppImgPath & "../UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=""javascript:top.switchDesktopTab()"" ")
                Else
                    MyBase.AddBackButton("../ContentStatistics.aspx?page=ContentStatistics.aspx&id=" & Me.content_id & "&LangType=" & Me.ContentLanguage)
                End If
        End Select
        MyBase.AddHelpButton("AddEditFlags")
    End Sub

    Private Sub RenderJS()
        Dim sbJS As New StringBuilder
        sbJS.Append("<script language=""javascript"" type=""text/javascript"" >" & Environment.NewLine)
        If Me.m_sPageAction = "edit" Then
            sbJS.Append("function SubmitForm()" & Environment.NewLine)
            sbJS.Append("{" & Environment.NewLine)
            sbJS.Append("    document.forms[0].submit();" & Environment.NewLine)
            sbJS.Append("}" & Environment.NewLine)
        End If
        sbJS.Append("</script>" & Environment.NewLine)
        ltr_js.Text &= Environment.NewLine & sbJS.ToString()
    End Sub

End Class