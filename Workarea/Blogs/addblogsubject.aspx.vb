Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Workarea

Partial Class blogs_addblogsubject
    Inherits workareabase

    Protected m_iBlogId As Long = 0
    Protected folder_data As FolderData
    Protected security_data As PermissionData

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            folder_data = m_refContentApi.GetFolderById(m_iID, True, True)
            If folder_data.FolderType = 1 Then 'blog
                If Not CheckAccess() Then
                    Throw New Exception(Me.GetMessage("com: user does not have permission"))
                Else
                    If Page.IsPostBack Then
                        Process_Add()
                    Else
                        Display_Add()
                    End If
                End If
            Else
                Throw New Exception(Me.GetMessage("blog not found"))
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

#Region "Display"

    Private Sub Display_Add()
        SetLabels()
        RenderJS()
    End Sub

#End Region

#Region "Process"
    Private Sub Process_Add()
        Me.m_refContentApi.EkContentRef.AddBlogSubject(Me.m_iID, txt_subject.text)
        Response.Redirect("../content.aspx?LangType=" & IIf(Me.ContentLanguage > 0, Me.ContentLanguage, Me.m_refContentApi.DefaultContentLanguage) & "&action=ViewFolder&id=" & Me.m_iID.ToString(), False)
    End Sub
#End Region

#Region "Private Helpers"
    Private Function CheckAccess() As Boolean
        security_data = m_refContentApi.LoadPermissions(m_iID, "folder")
        Return ((security_data.CanEditFolders Or security_data.CanEditApprovals)) OrElse m_refContentApi.IsARoleMemberForFolder_FolderUserAdmin(m_iID)
    End Function
    Private Sub SetLabels()
        Me.SetTitleBarToMessage("lbl add blog subject")

        MyBase.AddButtonwithMessages(AppImgPath & "../UI/Icons/save.png", "#", "alt save button text (blogsubject)", "btn save", "OnClick=""javascript:SubmitForm();return false;""")
        Me.AddBackButton("../content.aspx?action=ViewContentByCategory&id=" & Me.m_iID.ToString())
    End Sub

    Private Sub RenderJS()
        Dim sbJS As New StringBuilder
        sbJS.Append("<script language=""javascript"" type=""text/javascript"" >" & Environment.NewLine)
        sbJS.Append("var arrRollRel = new Array(0);" & Environment.NewLine)
        sbJS.Append("function SubmitForm()" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("   var val = Trim(document.getElementById('").Append(txt_subject.ID).Append("').value); " & Environment.NewLine)
        sbJS.Append("   if (val.length > 0) {" & Environment.NewLine)
        sbJS.Append("       if ((val.indexOf("";"") > -1) || (val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbJS.Append("       {" & Environment.NewLine)
        sbJS.Append("           alert(""" & Me.m_refMsg.GetMessage("alert subject name") & " (';', '\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')"");" & Environment.NewLine)
        sbJS.Append("       } else {" & Environment.NewLine)
        sbJS.Append("           document.forms[0].submit();" & Environment.NewLine)
        sbJS.Append("       } " & Environment.NewLine)
        sbJS.Append("   } else {" & Environment.NewLine)
        sbJS.Append("       alert('" & MyBase.GetMessage("js err blog subject") & "');" & Environment.NewLine)
        sbJS.Append("   } " & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)
        sbJS.Append("</script>" & Environment.NewLine)
        ltr_js.Text &= Environment.NewLine & sbJS.ToString()
    End Sub
#End Region

End Class
