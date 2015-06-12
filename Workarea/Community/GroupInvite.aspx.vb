Imports System.Data
Imports System.DateTime
Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.CustomFieldsApi
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration

Partial Class Community_GroupInvite
    Inherits System.Web.UI.Page

    Protected m_userId As Long = 0
    Protected m_groupId As Long = 0
    Protected m_refUserApi As New UserAPI
    Protected m_refContentAPI As New ContentAPI
    Protected m_refCommunityGroup As Ektron.Cms.Community.CommunityGroup
    Protected _isGroupMember = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_userId = m_refContentAPI.RequestInformationRef.UserId
        Utilities.ValidateUserLogin()
      
        If ((Not IsNothing(Request.QueryString("groupid"))) AndAlso IsNumeric(Request.QueryString("groupid"))) Then
            m_groupId = Long.Parse(Request.QueryString("groupid"))
        End If
        m_refCommunityGroup = New Ektron.Cms.Community.CommunityGroup(m_refContentAPI.RequestInformationRef)
        If m_groupId <> 0 Then
            _isGroupMember = m_refCommunityGroup.IsGroupUser(m_groupId, m_userId)
        End If
        If Not m_refContentAPI.IsAdmin AndAlso Not _isGroupMember AndAlso Not m_refContentAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) Then
            Response.Redirect(m_refContentAPI.ApplicationPath & "reterror.aspx?info=" & m_refContentAPI.EkMsgRef.GetMessage("msg not invite user"), True)
            Exit Sub
        End If

        If ((Not Page.IsPostBack) AndAlso (Not Page.IsCallback)) Then
            SendInviteBtn.Attributes.Add("onclick", "return (GroupInvite_ValidateInvitiations('" + Invite_UsrSel.ControlId + "'))")
        End If
    End Sub

    Protected Sub SendInviteBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SendInviteBtn.Click
        ' Process invitations:
        Dim userIds() As String = Nothing
        Dim userEmails() As String = Nothing
        Dim comGrpApi As Ektron.Cms.Community.CommunityGroupAPI = Nothing
        Dim invDat As Ektron.Cms.Common.InvitationSendRequestData = Nothing
        Dim inviteResult As System.Collections.ObjectModel.Collection(Of Ektron.Cms.Community.Invitation) = Nothing
        'System.Collections.ObjectModel.Collection<Ektron.Cms.Community.Invitation> inviteResult = null; 
        Dim itemText As String = ""
        Dim optionalText As String = ""
        Dim sendInvites As Boolean = False
        Dim infoMsg As String = ""
        Dim userMessageId As Long = 10
        Dim nonuserMessageId As Long = 12

        Try
            Invite_UsrSel_panel.Enabled = False

            comGrpApi = New Ektron.Cms.Community.CommunityGroupAPI()
            invDat = New Ektron.Cms.Common.InvitationSendRequestData

            If ((Not IsNothing(Request.QueryString("usrmsgid"))) AndAlso IsNumeric(Request.QueryString("usrmsgid"))) Then
                userMessageId = Long.Parse(Request.QueryString("usrmsgid"))
            End If
            If ((Not IsNothing(Request.QueryString("nusrmsgid"))) AndAlso IsNumeric(Request.QueryString("nusrmsgid"))) Then
                nonuserMessageId = Long.Parse(Request.QueryString("nusrmsgid"))
            End If

            If (Not IsNothing(Request.Form.Item("GroupInvite_UserIds"))) Then
                userIds = Request.Form.Item("GroupInvite_UserIds").Trim().Split(",")
            End If

            If (Not IsNothing(Request.Form.Item("GroupInviteOptionalText"))) Then
                optionalText = Request.Form.Item("GroupInviteOptionalText").Trim()
            End If

            If (Not IsNothing(Request.Form.Item("GroupInvite_Emails"))) Then
                Dim rawEmails As String = Request.Form.Item("GroupInvite_Emails").Trim()
                rawEmails = rawEmails.Replace(Environment.NewLine, ";")
                rawEmails = rawEmails.Replace("'", ";")
                rawEmails = rawEmails.Replace("""", ";")
                rawEmails = rawEmails.Replace(" ", ";").Replace(",", ";").Replace("|", ";")
                userEmails = rawEmails.Split(";")
            End If

            If ((m_userId > 0) AndAlso (m_groupId > 0)) Then
                invDat.SenderId = m_userId
                invDat.OptionalText = optionalText
                invDat.UserMessageId = userMessageId
                invDat.NonuserMessageId = nonuserMessageId

                For Each itemText In userIds
                    If (IsNumeric(itemText)) Then
                        invDat.UserIds.Add(Long.Parse(itemText))
                        sendInvites = True
                    End If
                Next

                For Each itemText In userEmails
                    If (itemText.Trim.Length > 0) Then
                        invDat.EmailAddresses.Add(itemText.Trim())
                        sendInvites = True
                    End If
                Next

                If (sendInvites) Then
                    inviteResult = comGrpApi.InviteUsers(invDat, m_groupId)
                    If (inviteResult.Count > 0) Then

                        Dim sentCount As Integer = 0
                        Dim inviteResultItem As Ektron.Cms.Community.Invitation
                        For Each inviteResultItem In inviteResult
                            If (0 = inviteResultItem.Errors.Count) Then
                                If (Not inviteResultItem.Recipient.IsUser) Then
                                    sentCount += 1
                                End If
                            End If
                        Next

                        If ((sentCount > 0)) Then
                            infoMsg = GetMessage("invitations were sent")
                        Else
                            infoMsg = GetMessage("lbl action completed")
                        End If

                    Else
                        infoMsg = "There was a problem sending the invitation: Unknown cause"
                    End If
                End If
            End If

        Catch ex As Exception
            infoMsg = "There was a problem sending the invitation: " + ex.Message
        Finally
            BuildResultUI(infoMsg, inviteResult)
            comGrpApi = Nothing
        End Try
    End Sub

    Protected Sub BuildResultUI(ByVal msg As String, ByRef inviteResult As System.Collections.ObjectModel.Collection(Of Ektron.Cms.Community.Invitation))
        Dim sb As New System.Text.StringBuilder()
        Dim idx As Integer = 0
        Dim uiStr As String = msg
        Dim errorStr As String = ""

        Try
            GroupInviteStartupUiPanel.Visible = False
            GroupInviteResultUiPanel.Visible = True

            sb.Append("<ul>" + Environment.NewLine)
            For Each inviteResultItem As Ektron.Cms.Community.Invitation In inviteResult
                sb.Append(" <li>" + Environment.NewLine)
                If ((inviteResultItem.Recipient IsNot Nothing)) Then
                    If (inviteResultItem.Recipient.IsUser) Then
                        sb.Append(inviteResultItem.Recipient.DisplayName)
                    Else
                        sb.Append(IIf(Not IsNothing(inviteResultItem.Recipient.Email), inviteResultItem.Recipient.Email, ""))
                    End If
                End If
                If (inviteResultItem.Errors.Count > 0) Then
                    errorStr = ""
                    For idx = 0 To inviteResultItem.Errors.Count - 1
                        errorStr += inviteResultItem.Errors(idx)
                    Next
                    If (errorStr.Length > 0) Then
                        sb.Append("<span class=""ekError"">" + errorStr + "</span>")
                    End If
                End If
                If ((inviteResultItem.Recipient IsNot Nothing)) Then
                    If (inviteResultItem.Recipient.IsUser) Then
                        sb.Append("&#160;" + GetMessage("lbl a site user - sent group request") + Environment.NewLine)
                    Else
                        sb.Append("&#160;" + GetMessage("lbl was sent an invitation") + Environment.NewLine)
                    End If
                End If

                sb.Append(" </li>" + Environment.NewLine)
            Next
            sb.Append("</ul>" + Environment.NewLine)
            uiStr += "<br />" + sb.ToString

        Catch ex As Exception

        Finally
            GroupInviteResults.Text = uiStr
            sb = Nothing
        End Try
    End Sub

    Protected Function GetMessage(ByVal resource As String) As String
        Return m_refContentAPI.EkMsgRef.GetMessage(resource)
    End Function

End Class
