Imports System.Data
Imports System.Collections.Generic
Imports Ektron.Cms
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports Ektron.Cms.Common.EkEnumeration

Partial Class Workarea_Community_CommunityTemplates
    Inherits System.Web.UI.Page
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_refStyle As New StyleHelper
    Protected m_strPageAction As String = ""
    Protected AppImgPath As String = ""
    Protected m_refContentAPI As New ContentAPI()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        displaystylesheet.Text = m_refStyle.GetClientScript
        AppImgPath = m_refContentAPI.AppImgPath
        m_refMsg = m_refContentAPI.EkMsgRef
        Utilities.SetLanguage(m_refContentAPI)
        RegisterResources()
        Utilities.ValidateUserLogin()
        If (Not m_refContentAPI.IsAdmin) Then
            Response.Redirect(m_refContentAPI.ApplicationPath & "reterror.aspx?info=" & m_refContentAPI.EkMsgRef.GetMessage("msg login cms administrator"), True)
            Exit Sub
        End If
        lblGroupTemplates.Text = m_refMsg.GetMessage("lbl group templates")
        lblGroupCommunityDocuments.Text = m_refMsg.GetMessage("lbl community document") & ":"
        lblGroupPhotoGallery.Text = m_refMsg.GetMessage("lbl photo gallery") & ":"
        lblGroupBlog.Text = m_refMsg.GetMessage("lbl journal") & ":"
        lblGroupCalendar.Text = m_refMsg.GetMessage("calendar lbl") & ":"
        lblGroupProfile.Text = m_refMsg.GetMessage("lbl profile") & ":"
        lblGroupForum.Text = m_refMsg.GetMessage("lbl forum") & ":"

        lblUserTemplates.Text = m_refMsg.GetMessage("lbl user templates")
        lblUserCommunityDocuments.Text = m_refMsg.GetMessage("lbl community document") & ":"
        lblUserPhotoGallery.Text = m_refMsg.GetMessage("lbl photo gallery") & ":"
        lblUserBlog.Text = m_refMsg.GetMessage("lbl journal") & ":"
        lblUserCalendar.Text = m_refMsg.GetMessage("calendar lbl") & ":"
        lblUserProfile.Text = m_refMsg.GetMessage("lbl profile") & ":"

        Dim grouptemplate() As TemplateData
        Dim userTemplate() As TemplateData
        grouptemplate = m_refContentAPI.GetCommunityTemplate(Ektron.Cms.Common.EkEnumeration.TemplateType.Group)
        userTemplate = m_refContentAPI.GetCommunityTemplate(Ektron.Cms.Common.EkEnumeration.TemplateType.User)

        If (Not Page.IsPostBack) Then
            ViewAllToolBar()
            If (grouptemplate IsNot Nothing AndAlso grouptemplate.length > 0) Then
                For i As Integer = 0 To grouptemplate.length - 1
                    If (grouptemplate(i).subtype = TemplateSubType.Workspace) Then
                        txtGroupCommunityDocuments.Text = grouptemplate(i).FileName
                    ElseIf (grouptemplate(i).subtype = TemplateSubType.Photos) Then
                        txtGroupPhotoGallery.Text = grouptemplate(i).FileName
                    ElseIf (grouptemplate(i).SubType = TemplateSubType.Profile) Then
                        txtGroupProfile.Text = grouptemplate(i).FileName
                    ElseIf (grouptemplate(i).SubType = TemplateSubType.Calendar) Then
                        txtGroupCalendar.Text = grouptemplate(i).FileName
                    ElseIf (grouptemplate(i).SubType = TemplateSubType.Forum) Then
                        txtGroupForum.Text = grouptemplate(i).FileName
                    ElseIf (grouptemplate(i).SubType = TemplateSubType.Blog) Then
                        txtGroupBlog.Text = grouptemplate(i).FileName
                    End If
                Next
            End If

            If (userTemplate IsNot Nothing AndAlso userTemplate.length > 0) Then
                For i As Integer = 0 To userTemplate.length - 1
                    If (userTemplate(i).subtype = TemplateSubType.Workspace) Then
                        txtUserCommunityDocuments.Text = userTemplate(i).FileName
                    ElseIf (userTemplate(i).subtype = TemplateSubType.Photos) Then
                        txtUserPhotoGallery.Text = userTemplate(i).FileName
                    ElseIf (userTemplate(i).SubType = TemplateSubType.Profile) Then
                        txtUserProfile.Text = userTemplate(i).FileName
                    ElseIf (userTemplate(i).SubType = TemplateSubType.Calendar) Then
                        txtUserCalendar.Text = userTemplate(i).FileName
                    ElseIf (userTemplate(i).SubType = TemplateSubType.Blog) Then
                        txtUserBlog.Text = userTemplate(i).FileName
                    End If
                Next
            End If

        Else
            Dim data As TemplateData = Nothing
            If (grouptemplate IsNot Nothing AndAlso grouptemplate.length > 0) Then
                For i As Integer = 0 To grouptemplate.length - 1
                    data = New TemplateData()
                    data.Type = TemplateType.Group
                    data.Id = grouptemplate(i).Id
                    If (grouptemplate(i).SubType = TemplateSubType.Workspace) Then
                        data.FileName = txtGroupCommunityDocuments.Text.ToString()
                        data.SubType = TemplateSubType.Workspace
                    ElseIf (grouptemplate(i).SubType = TemplateSubType.Photos) Then
                        data.FileName = txtGroupPhotoGallery.Text.ToString()
                        data.SubType = TemplateSubType.Photos
                    ElseIf (grouptemplate(i).SubType = TemplateSubType.Profile) Then
                        data.FileName = txtGroupProfile.Text
                        data.SubType = TemplateSubType.Profile
                    ElseIf (grouptemplate(i).SubType = TemplateSubType.Calendar) Then
                        data.FileName = txtGroupCalendar.Text
                        data.SubType = TemplateSubType.Calendar
                    ElseIf (grouptemplate(i).SubType = TemplateSubType.Forum) Then
                        data.FileName = txtGroupForum.Text
                        data.SubType = TemplateSubType.Forum
                    ElseIf (grouptemplate(i).SubType = TemplateSubType.Blog) Then
                        data.FileName = txtGroupBlog.Text.ToString()
                        data.SubType = TemplateSubType.Blog
                    End If
                    If (data.FileName.Length > 0) Then
                        m_refContentAPI.EkContentRef.UpdateTemplatev2_0(data)
                    End If
                Next
            End If

            If (userTemplate IsNot Nothing AndAlso userTemplate.length > 0) Then
                For i As Integer = 0 To userTemplate.length - 1
                    data = New TemplateData()
                    data.Type = TemplateType.User
                    data.Id = userTemplate(i).Id
                    If (userTemplate(i).SubType = TemplateSubType.Workspace) Then
                        data.FileName = txtUserCommunityDocuments.Text.ToString()
                        data.SubType = TemplateSubType.Workspace
                    ElseIf (userTemplate(i).SubType = TemplateSubType.Photos) Then
                        data.FileName = txtUserPhotoGallery.Text.ToString()
                        data.SubType = TemplateSubType.Photos
                    ElseIf (userTemplate(i).SubType = TemplateSubType.Profile) Then
                        data.FileName = txtUserProfile.Text
                        data.SubType = TemplateSubType.Profile
                    ElseIf (userTemplate(i).SubType = TemplateSubType.Calendar) Then
                        data.FileName = txtUserCalendar.Text
                        data.SubType = TemplateSubType.Calendar
                    ElseIf (userTemplate(i).SubType = TemplateSubType.Blog) Then
                        data.FileName = txtUserBlog.Text.ToString()
                        data.SubType = TemplateSubType.Blog
                    End If
                    If (data.FileName.Length > 0) Then
                        m_refContentAPI.EkContentRef.UpdateTemplatev2_0(data)
                    End If
                Next
            End If
        End If

    End Sub

    Private Sub ViewAllToolBar()
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl templates"))
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text"), m_refMsg.GetMessage("alt update button text"), "onclick=""return Submit();"""))
        result.Append("<td>" & m_refStyle.GetHelpButton("ViewEditCommunityTemplates") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Private Sub RegisterResources()
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronThickBoxJS)

        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronThickBoxCss)
    End Sub
End Class
