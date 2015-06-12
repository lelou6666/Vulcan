Imports system.data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.CustomFieldsApi
Imports Ektron.Cms.Content

Partial Class Messaging_ViewMessages
	Inherits System.Web.UI.UserControl

	Protected _SentMode As Boolean = False
	Protected m_refStyle As New StyleHelper
	Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
	Protected m_mode As String = ""
    Protected m_userId As Long = 0
	Protected m_action As String = ""

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Dim refCommonAPI As New CommonApi()
		m_userId = refCommonAPI.RequestInformationRef().UserId
		m_refMsg = (New CommonApi).EkMsgRef
        RegisterResources()
        If (Not IsNothing(Request.QueryString("mode"))) Then
            m_mode = Request.QueryString("mode").ToLower()
        End If
		If (Not IsNothing(Request.QueryString("action"))) Then
			m_action = Request.QueryString("action").ToLower()
		End If

		If (("del" = m_mode) AndAlso (Not IsNothing(Request.Form("MsgInboxSelCBHdn"))) AndAlso (Request.Form("MsgInboxSelCBHdn").Trim.Length > 0)) Then
			Dim sDelList() As String = (Request.Form("MsgInboxSelCBHdn").Trim.Split(","))
			Dim idx As Integer
            Dim delList() As Long = Array.CreateInstance(GetType(Long), sDelList.Length)
			For idx = 0 To sDelList.Length - 1
				If (IsNumeric(sDelList(idx))) Then
                    delList.SetValue(CType(sDelList(idx), Long), idx)
				End If
			Next
			Dim objPM As New PrivateMessage(refCommonAPI.RequestInformationRef())
            objPM.DeleteMessageList(delList, _SentMode)
			refCommonAPI = Nothing
			objPM = Nothing
			Response.ClearContent()
			If (m_action.Length = 0) Then
				m_action = "viewall"
			End If
			Response.Redirect("CommunityMessaging.aspx?action=" + m_action, False)
		Else
			LoadToolBar()
			LoadGrid()
		End If
	End Sub

	Public Property SentMode() As Boolean
		Get
			Return _SentMode
		End Get
		Set(ByVal value As Boolean)
			_SentMode = value
		End Set
	End Property

	Protected Sub LoadToolBar()
		Dim m_refUserApi As New UserAPI
		Dim AppImgPath As String = m_refUserApi.AppImgPath
        Dim result As New System.Text.StringBuilder
        Dim helpBtnText As String = String.Empty
		Try

            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(IIf(SentMode, GetMessage("lbl sent messages"), GetMessage("lbl inbox")))
            result.Append("<table ><tr>")
            If (Request.QueryString("action") = "viewallsent") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/emailDelete.png", "javascript:DelSelMsgs(" + IIf(SentMode, "true", "false") + ");", m_refMsg.GetMessage("lbl del sel"), m_refMsg.GetMessage("lbl del sel"), ""))
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "CommunityMessaging.aspx?action=viewall", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
                helpBtnText = "sentmessages"
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/emailAdd.png", "CommunityMessaging.aspx?action=editmsg", GetMessage("lbl compose a message"), GetMessage("lbl compose a message"), ""))
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/emailDelete.png", "javascript:DelSelMsgs(" + IIf(SentMode, "true", "false") + ");", m_refMsg.GetMessage("lbl del sel"), m_refMsg.GetMessage("lbl del sel"), ""))
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/emailSent.png", "CommunityMessaging.aspx?action=viewallsent", m_refMsg.GetMessage("lbl sent messages"), m_refMsg.GetMessage("lbl sent messages"), ""))
                helpBtnText = "messaging_inbox"
            End If
            '
            result.Append("<td>")
            result.Append(m_refStyle.GetHelpButton(helpBtnText))
            result.Append("</td>")
            result.Append("</tr></table>")

            divToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
	End Sub

	Protected Sub LoadGrid()
		Dim cb As New System.Web.UI.WebControls.BoundColumn
		cb.DataField = "fDelete"
        cb.HeaderText = "<input type=""checkbox"" onclick=""MsgInboxToggleAllCB(this);"" name=""MsgInboxMasterCB"" value=""" & "ID" & """ runat=""Server""/>"
        cb.Initialize()
        cb.HeaderStyle.CssClass = "center checkBoxColumn"
        cb.ItemStyle.CssClass = "center checkBoxColumn"
		_dg.Columns.Add(cb)

		cb = New System.Web.UI.WebControls.BoundColumn
		cb.DataField = "fFrom"
		If (SentMode) Then
			cb.HeaderText = GetMessage("lbl generic to")
		Else
			cb.HeaderText = GetMessage("lbl generic from")
		End If
		cb.Initialize()
		_dg.Columns.Add(cb)

		cb = New System.Web.UI.WebControls.BoundColumn
		cb.DataField = "fSubject"
		cb.HeaderText = GetMessage("lbl generic subject")
		cb.Initialize()
		_dg.Columns.Add(cb)

		cb = New System.Web.UI.WebControls.BoundColumn
		cb.DataField = "fDate"
		cb.HeaderText = GetMessage("lbl generic date")
		cb.Initialize()
		_dg.Columns.Add(cb)

		_dg.DataSource = CreateMsgData()
		_dg.DataBind()
	End Sub

	Protected Function CreateMsgData() As ICollection
		Dim dt As New DataTable
		Dim dr As DataRow
		Dim ListCheckboxes As String = ""
		Dim Name As String = ""
		Dim msgUserId As String = ""

		Try
			' header:
			dt.Columns.Add(New DataColumn("fDelete", GetType(String)))
			dt.Columns.Add(New DataColumn("fFrom", GetType(String)))
			dt.Columns.Add(New DataColumn("fSubject", GetType(String)))
			dt.Columns.Add(New DataColumn("fDate", GetType(String)))

			' data:
			Dim m_refCommonAPI As New CommonApi()
			Dim objPM As New PrivateMessage(m_refCommonAPI.RequestInformationRef())
            Dim aMessages() As PrivateMessage = Array.CreateInstance(GetType(PrivateMessage), 0)
			aMessages = objPM.GetMessagesForMe(IIf(SentMode, 1, 0), 0)	' inbox=0/sent=1.
			Dim idx As Integer = 0
			For idx = 0 To aMessages.Length - 1
				dr = dt.NewRow()
				Name = "MsgInboxCB_" & idx.ToString()
				If (ListCheckboxes.Length > 0) Then
					ListCheckboxes = ListCheckboxes & "," & Name
				Else
					ListCheckboxes = Name
				End If
                dr(0) = "<input type=""checkbox"" onclick=""MsgInboxToggleCB(this);"" className=""inboxRowCB"" name=""" & Name & """ value=""" & aMessages(idx).ID.ToString() & """ runat=""Server""/>"
				If (SentMode) Then
                    dr(1) = aMessages(idx).GetFormattedRecipientList()
				Else
					dr(1) = aMessages(idx).FromUserDisplayName
				End If
                ' msgUserId = aMessages(idx).ToUserID.ToString()
                dr(2) = "<a href=""CommunityMessaging.aspx?action=" + IIf(SentMode, "viewsentmsg", "viewmsg") + "&id=" + aMessages(idx).ID.ToString() + "&userid=" + msgUserId + """ target=""_self"" >" + FormatByStatus(aMessages(idx), SentMode) + "</a>"
				dr(3) = aMessages(idx).DateCreated
				dt.Rows.Add(dr)
			Next

		Catch ex As Exception
		Finally
			CreateMsgData = New DataView(dt)
		End Try
	End Function

    Protected Function FormatByStatus(ByRef msg As PrivateMessage, ByVal sendingMode As Boolean) As String
        Dim result As String = LimitSubjectLength(msg.Subject)

        If (Not sendingMode) Then
            If (HasRead(msg)) Then
                result = "<i>" + result + "</i>"
            Else
                result = "<b>" + result + "</b><img src='images\UI\Icons\email.png' style='margin-left: 15px;' />"
            End If
        End If

        Return result
    End Function

    Protected Function HasRead(ByRef msg As PrivateMessage) As Boolean
        Dim result As Boolean = False
        For Each target As Ektron.Cms.PrivateMessageRecipientData In msg.Recipients
            If target.ToUserID = Me.m_userId Then
                result = target.Read
                Exit For
            End If
        Next
        Return result
    End Function

    Protected Function GetMessage(ByVal resource As String) As String
        Return Me.m_refMsg.GetMessage(resource)
    End Function

    Protected Function LimitSubjectLength(ByVal msg As String) As String
        Dim result As String = msg
        Dim clipLength As Integer = 45
        If result.Length > clipLength Then
            result = result.Substring(0, clipLength - 3) + "..."
        End If

        Return result
    End Function
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronStyleHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.AllIE)
    End Sub
End Class
