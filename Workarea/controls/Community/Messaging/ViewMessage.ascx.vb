Imports system.data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.CustomFieldsApi
Imports Ektron.Cms.Content
Imports Ektron.Editors

Partial Class controls_Community_Messaging_ViewMessage
	Inherits System.Web.UI.UserControl

	Protected m_refStyle As New StyleHelper
	Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
	Protected m_id As String = ""
	Protected m_mode As String = ""
	Protected m_prev As Boolean = False
	Protected m_next As Boolean = False
    Protected m_userId As Long = 0
	Protected m_msgUserId As String = ""
    Protected m_sentMode As Boolean = False
    Protected m_bCanReply As Boolean = True

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Dim refCommonAPI As New CommonApi()
		m_userId = refCommonAPI.RequestInformationRef().UserId
		m_refMsg = (New CommonApi).EkMsgRef
        RegisterResources()
		If (Not IsNothing(Request.QueryString("id"))) Then
			m_id = Request.QueryString("id").ToLower()
		End If
		If (Not IsNothing(Request.QueryString("userid"))) Then
			m_msgUserId = Request.QueryString("userid").ToLower().Trim()
		Else
			m_msgUserId = m_userId.ToString()
		End If
		If (Not IsNothing(Request.QueryString("mode"))) Then
			m_mode = Request.QueryString("mode").ToLower()
		End If
		If ("prev" = m_mode) Then
			m_prev = True
		ElseIf ("next" = m_mode) Then
			m_next = True
		End If

		If (("del" = m_mode) AndAlso (m_id.Trim.Length > 0) AndAlso IsNumeric(m_id)) Then
            Dim delList() As Long = {CType(m_id, Long)}
			Dim objPM As New PrivateMessage(refCommonAPI.RequestInformationRef())
            objPM.DeleteMessageList(delList, m_sentMode)
			refCommonAPI = Nothing
			objPM = Nothing
			Response.ClearContent()
			Response.Redirect("CommunityMessaging.aspx?action=" + IIf(SentMode, "viewallsent", "viewall"), False)
		Else
            LoadMsg()
            LoadToolBar()
		End If
		refCommonAPI = Nothing
		m_refMsg = Nothing
	End Sub

	Protected Sub LoadToolBar()
		Dim m_refUserApi As New UserAPI
        Dim AppImgPath As String = m_refUserApi.AppImgPath
        Dim refContentAPI As New ContentAPI()
        Dim AppPath As String = refContentAPI.AppPath

		Dim result As New System.Text.StringBuilder
        Try
            Dim emailDelete As String = m_refStyle.GetButtonEventsWCaption(AppPath & "images/ui/icons/emailDelete.png", "CommunityMessaging.aspx?action=" + IIf(SentMode, "viewsentmsg", "viewmsg") + "&mode=del&id=" + m_id, GetMessage("btn delete"), GetMessage("btn delete"), "")

            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(GetMessage("lbl messages"))
            result.Append("<table><tr>")

            If (m_sentMode = False) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/ui/icons/emailAdd.png", "CommunityMessaging.aspx?action=editmsg", GetMessage("lbl compose a message"), GetMessage("lbl compose a message"), ""))
                result.Append(emailDelete)
                If m_bCanReply Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/ui/icons/emailReply.png", "CommunityMessaging.aspx?action=editmsg&id=" + m_id, GetMessage("lbl reply message"), GetMessage("lbl reply message"), ""))
                End If
            Else
                result.Append(emailDelete)
            End If
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/ui/icons/emailForward.png", "CommunityMessaging.aspx?action=editmsg&mode=fwd&id=" + m_id + "&userid=" + m_msgUserId, GetMessage("lbl forward message"), GetMessage("lbl forward message"), ""))


            Dim previousMsgID As Int64 = 0
            Dim nextMsgID As Int64 = 0
            refContentAPI.GetAjacentPrivateMessagesForCurrentUser(m_id, Not (SentMode), previousMsgID, nextMsgID)
            If (previousMsgID > 0) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/ui/icons/arrowLeft.png", "CommunityMessaging.aspx?action=" + IIf(SentMode, "viewsentmsg", "viewmsg") + "&id=" + previousMsgID.ToString() + "&userid=" + m_msgUserId.ToString(), GetMessage("lbl previous message"), GetMessage("lbl previous message"), ""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/ui/icons/arrowLeftOff.png", "#", GetMessage("lbl previous message"), GetMessage("lbl previous message"), ""))
            End If
            If (nextMsgID > 0) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/ui/icons/arrowRight.png", "CommunityMessaging.aspx?action=" + IIf(SentMode, "viewsentmsg", "viewmsg") + "&id=" + nextMsgID.ToString() + "&userid=" + m_msgUserId.ToString(), GetMessage("lbl next message"), GetMessage("lbl next message"), ""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/ui/icons/arrowRightOff.png", "#", GetMessage("lbl next message"), GetMessage("lbl next message"), ""))
            End If
            'CommunityMessaging.aspx?action=" + IIf(SentMode, "viewsentmsg", "viewmsg") + "&id=" + aMessages(idx).ID.ToString()

            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/ui/icons/print.png", "javascript:window.print();", GetMessage("lbl print message"), GetMessage("lbl print message"), ""))
            ' Back button
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/ui/icons/back.png", "CommunityMessaging.aspx?action=" + IIf(SentMode, "viewallsent", "viewall"), GetMessage("btn back"), GetMessage("btn back"), ""))
            result.Append("<td>")
            If (SentMode) Then
                result.Append(m_refStyle.GetHelpButton("view_sent_msg"))
            Else
                result.Append(m_refStyle.GetHelpButton("view_msg"))
            End If
            result.Append("</td>")
            result.Append("</tr></table>")

            divToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        Finally
            refContentAPI = Nothing
        End Try
    End Sub

	Protected Sub LoadMsg()
		Try
			If ((m_id.Length > 0) AndAlso (IsNumeric(m_id))) Then
                Dim aMsg As New PrivateMessage
				Dim refCommonAPI As New CommonApi()
				Dim objPM As New PrivateMessage(refCommonAPI.RequestInformationRef())
                objPM.GetByMessageID(m_id)

                aMsg = IsMyMessage(objPM)
                If aMsg.FromUserDeleted Then
                    m_bCanReply = False
                End If
                Dim msgHeader As New StringBuilder
                msgHeader.AppendLine("<table class=""ViewMsgHeader ektronGrid"" cellspacing=""0"">")
                msgHeader.AppendLine("<tbody>")
                ' Add From and Sent
                msgHeader.AppendLine("<tr>")
                msgHeader.AppendLine("<td class=""ViewMsgLabel label"">" + m_refMsg.GetMessage("generic from label") + "</td>")
                msgHeader.AppendLine("<td><div class=""ViewMsgDate""><span class=""ViewMsgLabel"">" + m_refMsg.GetMessage("generic sent label") + "</span>" + aMsg.DateCreated + "</div>" + aMsg.FromUserDisplayName + "</td>")
                msgHeader.AppendLine("</tr>")
                ' Add To
                msgHeader.AppendLine("<tr>")
                msgHeader.Append("<td class=""ViewMsgLabel label"">" + m_refMsg.GetMessage("generic to label") + "</td>")
                msgHeader.Append("<td>" + aMsg.GetFormattedRecipientList() + "</td>")
                msgHeader.AppendLine("</tr>")
                ' Add Subject
                msgHeader.AppendLine("<tr>")
                msgHeader.Append("<td class=""ViewMsgLabel label"">" + m_refMsg.GetMessage("generic subject label") + "</td>")
                msgHeader.Append("<td>" + aMsg.Subject + "</td>")
                msgHeader.AppendLine("</tr>")
                msgHeader.AppendLine("</tbody>")
                msgHeader.AppendLine("</table>")

                ltrMsgView.Text = "<div class=""ViewMsgContainer"">"
                ltrMsgView.Text += msgHeader.ToString()
                ltrMsgView.Text += "<hr class=""ViewMsgHR""/>"
                ltrMsgView.Text += "<div class=""ViewMsgMessage"">" + HttpUtility.UrlDecode(aMsg.Message) + "</div>"
                ltrMsgView.Text += "</div>"
                aMsg.MarkAsRead()
            End If

		Catch ex As Exception
		Finally
		End Try
	End Sub

	Protected Function GetMessage(ByVal resource As String) As String
		Return Me.m_refMsg.GetMessage(resource)
	End Function

	Public Property SentMode() As Boolean
		Get
			Return m_sentMode
		End Get
		Set(ByVal value As Boolean)
			m_sentMode = value
		End Set
	End Property
    Protected Function IsMyMessage(ByVal msg As PrivateMessage) As PrivateMessage
        Dim result As PrivateMessage = Nothing
        Try

            If msg.FromUserID = m_userId OrElse (msg.IsARecipient(m_userId)) OrElse (IsNumeric(m_msgUserId) AndAlso msg.IsARecipient(m_msgUserId)) Then
                result = msg
            End If
        Catch ex As Exception
            result = Nothing
        Finally
            If (IsNothing(result)) Then
                result = New PrivateMessage
            End If
            IsMyMessage = result
        End Try
    End Function
    Protected Function GetMyMsg(ByRef msgs() As PrivateMessage) As PrivateMessage
        Dim result As PrivateMessage = Nothing
		Dim idx As Integer
		Try
			If ((Not IsNothing(msgs)) AndAlso (msgs.Length > 0)) Then
				For idx = 0 To msgs.Length - 1
                    If (msgs(idx).IsARecipient(m_userId)) OrElse (IsNumeric(m_msgUserId) AndAlso msgs(idx).IsARecipient(m_msgUserId)) Then
						result = msgs(idx)
						Exit For
					End If
				Next
			End If

		Catch ex As Exception
			result = Nothing
		Finally
			If (IsNothing(result)) Then
                result = New PrivateMessage
			End If
			GetMyMsg = result
		End Try
    End Function

    Protected Function WrapLongSubject(ByVal msg As String) As String
        Dim wrapLength As Integer = 45
        Dim result As String = ""
        Dim thisLine As String = ""
        Dim words() As String
        Dim idx As Integer = 0

        words = msg.split(" ")
        For idx = 0 To words.length - 1
            If ((thisLine + words(idx).trim).length < wrapLength) Then
                result += " " + words(idx).trim
                thisLine += " " + words(idx).trim
            Else
                result += "<br />" + words(idx).trim
                thisLine = words(idx).trim
            End If
        Next

        Return result
    End Function
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronStyleHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
End Class
