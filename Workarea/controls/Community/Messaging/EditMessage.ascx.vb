Imports System.Data
Imports System.DateTime
Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.CustomFieldsApi
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Editors

Partial Class controls_Community_Messaging_EditMessage
    Inherits System.Web.UI.UserControl

    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_id As String = ""
    Protected m_mode As String = ""
    Protected m_replying As Boolean = False
    Protected m_forwarding As Boolean = False
    Protected m_userId As Long = 0
    Protected m_refUserApi As UserAPI = Nothing
    Protected m_callbackwrap As Boolean = True
    Protected m_callbackresult As String = ""
    Protected m_ExecuteFillOnCallBack As Boolean = False
    Protected m_PostBackData As System.Collections.Specialized.NameValueCollection = Nothing
    Protected m_recipientsPageStr As String = "1"
    Protected m_recipientsPage As Integer = 1
    Protected m_friendsOnly As Boolean = False
    Protected m_refCommon As New CommonApi
    Protected m_refContentAPI As New ContentAPI
    Protected m_UserType As UserTypes = UserTypes.AuthorType
    Protected m_user_list As UserData() = Array.CreateInstance(GetType(Ektron.Cms.UserData), 0)
    Protected m_strSearchText As String = ""
    Protected m_strKeyWords As String = ""
    Protected m_intTotalPages As Integer = 0
    Protected m_uniqueId As String = ""
    Protected m_searchMode As String = "display_name"
    Protected cssFilesPath As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim refCommonAPI As New CommonApi()
        m_userId = refCommonAPI.RequestInformationRef().UserId
        m_refMsg = (New CommonApi).EkMsgRef
        m_refUserApi = New UserAPI
        Dim msgSubject As String = ""
        Dim msgText As String = ""
        RegisterResources()
        m_uniqueId = Me.ClientID
        MsgToLabel.InnerText = m_refMsg.GetMessage("lbl generic to") + ":"
        MsgSubjectLabel.InnerText = m_refMsg.GetMessage("lbl generic subject") + ":"

        cgae_userselect_done_btn.Attributes.Add("onclick", "GetCommunityMsgObject('" + m_uniqueId + "').MsgSaveMessageTargetUI(); return false")
        cgae_userselect_done_btn.Attributes.Add("class", "EktMsgTargetsDoneBtn")
        cgae_userselect_done_btn.Text = m_refMsg.GetMessage("btn done")

        cgae_userselect_cancel_btn.Attributes.Add("onclick", "GetCommunityMsgObject('" + m_uniqueId + "').MsgCancelMessageTargetUI(); return false")
        cgae_userselect_cancel_btn.Attributes.Add("class", "EktMsgTargetsCancelBtn")
        cgae_userselect_cancel_btn.Text = m_refMsg.GetMessage("btn cancel")


        If (Not IsNothing(Request.QueryString("id"))) Then
            m_id = Request.QueryString("id").ToLower()
        End If
        If (Not IsNothing(Request.QueryString("mode"))) Then
            m_mode = Request.QueryString("mode").ToLower()
        End If
        m_replying = (m_id.Trim.Length > 0) AndAlso IsNumeric(m_id)
        If (m_replying AndAlso ("fwd" = m_mode)) Then
            m_forwarding = True
            m_replying = False
        End If

        If (Page.IsCallback()) Then
        Else
            If ((IsPostBack) AndAlso (Not IsNothing(Request.Form.Item("hdnRecipientsValidated" + m_uniqueId))) AndAlso ("1" = Request.Form.Item("hdnRecipientsValidated" + m_uniqueId))) Then
                If (Not IsNothing(Request.Form.Item("msg_subject" + m_uniqueId))) Then
                    msgSubject = Request.Form.Item("msg_subject" + m_uniqueId)
				End If
				msgText = cdContent_teaser.Content
				cssFilesPath = Me.m_refContentAPI.ApplicationPath & "csslib/ektron.workarea.css"
				cdContent_teaser.Stylesheet = cssFilesPath
				RenderSentUI(SendMessage(msgSubject, msgText))
            Else
			RenderEditorUI()
			LoadMsg()
		End If
        End If

		refCommonAPI = Nothing
    End Sub

    Protected Sub RenderEditorUI()
        LoadToolBar()

        Dim browseStr As String = IIf(m_friendsOnly, GetMessage("lbl browse friends"), GetMessage("lbl browse users"))

        ltrBrowseFriends.Text = "<a href=""#EkTB_inline?height=480&width=450&caption=false&inlineId=MessageTargetUI" + ClientID + "&modal=true"" class=""ek_thickbox"" onclick=""return GetCommunityMsgObject('" + Me.ClientID + "').MsgShowMessageTargetUI('ektouserid" + m_uniqueId + "', true)"" >" + "<img alt=""" & browseStr & """ src=""images/ui/icons/usersMembership.png"" /></a> <a href=""#EkTB_inline?height=480&width=450&caption=false&inlineId=MessageTargetUI" + ClientID + "&modal=true"" class=""ek_thickbox"" onclick=""return GetCommunityMsgObject('" + Me.ClientID + "').MsgShowMessageTargetUI('ektouserid" + m_uniqueId + "', true)"" >" & browseStr & "</a>"

        ltrMsgJSObjectId.Text = "<script type=""text/javascript"" language=""javascript"">" + Environment.NewLine _
              + "GetCommunityMsgObject('" + m_uniqueId + "').SetUserSelectId('" + Invite_UsrSel.ControlId + "');" + Environment.NewLine _
        + "</script>"
    End Sub

    Protected Sub RenderSentUI(ByVal success As Boolean)
        If (success) Then
            ltrMsgView.Text = GetMessage("lbl message sent")
            Response.ClearContent()
            Response.Redirect("CommunityMessaging.aspx?action=viewall", False)
        Else
            ltrMsgView.Text = GetMessage("lbl message sent error")
        End If
    End Sub

    Protected Sub LoadToolBar()
        Dim AppImgPath As String = m_refUserApi.AppPath
        Dim sb As New System.Text.StringBuilder

        Try
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(IIf(m_replying, GetMessage("lbl reply message"), IIf(m_forwarding, GetMessage("lbl forward message"), GetMessage("lbl send message"))))
            sb.Append("<table><tr>")
            sb.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "images/UI/Icons/emailSend.png", "javascript:GetCommunityMsgObject('" + Me.ClientID + "').SendMessage();", GetMessage("lbl send this message"), GetMessage("lbl send this message"), ""))
            sb.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "images/UI/Icons/back.png", "javascript:history.back();", GetMessage("btn back"), GetMessage("btn back"), ""))
            sb.Append("<td>")
            sb.Append(m_refStyle.GetHelpButton("composecommunitymsg"))
            sb.Append("</td>")
            sb.Append("</tr></table>")

            divToolBar.InnerHtml = sb.ToString
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        Finally
            sb = Nothing
        End Try
    End Sub

    Protected Sub LoadMsg()
        Try
            Dim msgTarget As String = ""
            Dim msgTargetIds As String = ""
            Dim msgSubject As String = ""
            Dim msgText As String = ""
            cdContent_teaser.Visible = True
			cssFilesPath = Me.m_refContentAPI.ApplicationPath & "csslib/ektron.workarea.css"
			cdContent_teaser.Stylesheet = cssFilesPath

            If (m_replying Or m_forwarding) Then
                Dim pm As PrivateMessage = GetMsg()
                If ((Not IsNothing(pm)) AndAlso pm.ID > 0) Then
                    msgText += "<p></p><hr/><p>"
                    msgText += GetMessage("generic from label") & " " & pm.FromUserDisplayName + "<br />"
                    msgText += GetMessage("generic sent label") & " " & pm.DateCreated.ToLongDateString & " " & pm.DateCreated.ToShortTimeString & "<br />"
                    msgText += GetMessage("generic to label") & " " & pm.GetFormattedRecipientList() & "<br />"
                    msgText += GetMessage("generic subject label") & " " & pm.Subject
                    msgText += "</p>" & pm.Message

                    If (m_replying) Then
                        If pm.FromUserDeleted = False Then
                            msgTarget = pm.FromUserDisplayName
                            msgTargetIds = pm.FromUserID.ToString()
                        End If
                        msgSubject = IIf(pm.Subject.ToLower.Contains("re:"), "", "re: ") + pm.Subject
                    Else
                        msgSubject = IIf(pm.Subject.ToLower.Contains("fwd:"), "", "fwd: ") + pm.Subject
                    End If
                End If
				cdContent_teaser.Content = msgText
                'ftbEditor.Text = msgText
            End If
            'ltrMsgView.Text += ftbEditor.ToString()
            ltrMsgTo.Text = "<input name=""ekpmsgto" + m_uniqueId + """ id=""ekpmsgto" + m_uniqueId + """ disabled=""disabled"" value=""" + msgTarget + """ type=""text"" size=""40%"" />"
            ltrMsgSubject.Text = "<input type=""text"" name=""msg_subject" + m_uniqueId + """ id=""msg_subject" + m_uniqueId + """ value=""" + msgSubject + """ size=""40%"" />"
            litHdnToUserIds.Text = "<input type=""hidden"" name=""ektouserid" + m_uniqueId + """ id=""ektouserid" + m_uniqueId + """ value=""" + msgTargetIds + """ />"

        Catch ex As Exception
        Finally
        End Try
    End Sub

    Public Function SendMessage(ByVal msgSubject As String, ByVal msgText As String) As Boolean
        Dim result As Boolean = True
        Dim refCommonAPI As New CommonApi()
        Dim objPM As New PrivateMessage(refCommonAPI.RequestInformationRef())
        Dim aUsers() As UserData
        Dim userIds() As String = Nothing
        Dim idx As Integer = 0

        Try
            If (Not IsNothing(Request.Form("ektouserid" + m_uniqueId))) Then
                userIds = Request.Form("ektouserid" + m_uniqueId).Split(",")
            End If
            If (IsNothing(userIds) OrElse (userIds.Length = 0) OrElse ("" = userIds(0))) Then
                ' no recipients selected!
                result = False
            Else
                aUsers = Array.CreateInstance(GetType(UserData), userIds.Length)
                If (0 = msgSubject.Length) Then
                    msgSubject = m_refMsg.GetMessage("lbl no subject")
                End If
                objPM.Subject = msgSubject
				objPM.Message = msgText
                objPM.FromUserID = refCommonAPI.RequestInformationRef.UserId
                For idx = 0 To userIds.Length - 1
                    aUsers(idx) = New UserData()
                    aUsers(idx).Id = CType(userIds(idx).Trim(), Long)
                Next
                objPM.Send(aUsers)
            End If

        Catch ex As Exception
            result = False
        Finally
            SendMessage = result
            refCommonAPI = Nothing
            objPM = Nothing
        End Try
    End Function

    Protected Function GetMsg() As PrivateMessage
        Dim result As PrivateMessage = Nothing
        Dim refCommonAPI As CommonApi = Nothing
        Dim objPM As PrivateMessage = Nothing
        Try
            If (m_replying Or m_forwarding) Then
                ' recover existing message:
                refCommonAPI = New CommonApi()
                objPM = New PrivateMessage(refCommonAPI.RequestInformationRef())
                objPM.GetByMessageID(m_id)

                result = IsMyMessage(objPM)
            Else
                ' create a new message:
                result = New PrivateMessage
            End If

        Catch ex As Exception
            result = Nothing
        Finally
            If (IsNothing(result)) Then
                result = New PrivateMessage
            End If
            GetMsg = result
            refCommonAPI = Nothing
            objPM = Nothing
            'aMessages = Nothing
        End Try
    End Function

    Protected Function GetMyMsg(ByRef msgs() As PrivateMessage) As PrivateMessage
        Dim result As PrivateMessage = Nothing
        Dim idx As Integer
        Try
            If (m_forwarding) Then
                If ((Not IsNothing(msgs)) AndAlso (msgs.Length > 0)) Then
                    For idx = 0 To msgs.Length - 1
                        If (msgs(idx).IsARecipient(Request.QueryString("userid"))) Then
                            result = msgs(idx)
                            Exit For
                        End If
                    Next
                End If
            End If
            If (m_forwarding = False) Then
                If ((Not IsNothing(msgs)) AndAlso (msgs.Length > 0)) Then
                    For idx = 0 To msgs.Length - 1
                        If (msgs(idx).IsARecipient(m_userId)) Then
                            result = msgs(idx)
                            Exit For
                        End If
                    Next
                End If
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

    Protected Function IsMyMessage(ByVal msg As PrivateMessage) As PrivateMessage
        Dim result As PrivateMessage = Nothing
        Try

            If (msg.FromUserID = m_userId OrElse msg.IsARecipient(m_userId)) Then
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

    Protected Function GetMessage(ByVal resource As String) As String
        Return Me.m_refMsg.GetMessage(resource)
    End Function
    Private Sub RegisterResources()
        ' register JS
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronStyleHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronThickBoxJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)

        ' register CSS
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentAPI.ApplicationPath & "csslib/box.css", "EktronBoxCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.AllIE)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub
End Class
