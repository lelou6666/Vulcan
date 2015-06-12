Imports Ektron.Cms
Imports Ektron.Cms.UI.CommonUI
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common

Partial Class AddTaskComment
    Inherits System.Web.UI.Page
	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Dim AppUI As New ApplicationAPI
		Dim AppPath, AppImgPath, SitePath, AppeWebPath, AppName As String
		Dim CommentKeyId As Long = 0
		Dim CommentId As Long = 0
		Dim RefType, CurrentUserID, Action, ActionType, IsMac, platform As Object
		Dim cContObj, Flag, retVal, CommentText As Object
		Dim NS4, OrderBy As Object
		Dim iMaxContLength, localeFileString, cid As Object
		Dim var1, var2 As Object
		Dim taskObj, taskIDs As Object
		Dim tasksArray As Object
		Dim lCounter As Object
		Dim height, width As Object
		Dim EnableMultilingual, ContentLanguage As Object
		Dim MsgHelper As Ektron.Cms.Common.EkMessageHelper
		Dim sbScript As New System.Text.StringBuilder

		MsgHelper = AppUI.EkMsgRef
		AppPath = AppUI.AppPath
		AppImgPath = AppUI.AppImgPath
		SitePath = AppUI.SitePath
		AppeWebPath = AppUI.AppeWebPath
		AppPath = AppUI.AppPath
		AppName = AppUI.AppName
		EnableMultilingual = AppUI.EnableMultilingual
		ContentLanguage = 1033 'set default value
        Utilities.ValidateUserLogin()
        If AppUI.RequestInformationRef.IsMembershipUser Then
            Response.Redirect("reterror.aspx?info=" & MsgHelper.GetMessage("msg login cms user"), False)
            Exit Sub
        End If
		If (Request.QueryString("LangType") <> "") Then
			ContentLanguage = Request.QueryString("LangType")
			AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage)
		Else
			If CStr(AppUI.GetCookieValue("LastValidLanguageID")) <> "" Then
				ContentLanguage = AppUI.GetCookieValue("LastValidLanguageID")
			End If
		End If

		platform = Request.ServerVariables("HTTP_USER_AGENT")
		If (InStr(platform, "Windows") > 0) Then
			IsMac = 0
		Else
			IsMac = 1
		End If

		RefType = "T"
		Flag = False
		iMaxContLength = 65000
		localeFileString = "0000"
		var1 = Request.ServerVariables("SERVER_NAME")
		If Not IsNothing(Request.QueryString("commentkey_id")) AndAlso Request.QueryString("commentkey_id").Length > 0 Then
			CommentKeyId = Convert.ToInt64(Request.QueryString("commentkey_id"))
		End If
		Action = Request.QueryString("action")
		ActionType = Request.QueryString("ty")
		OrderBy = Request.QueryString("orderby")
		cid = Request.QueryString("id")
		If Not IsNothing(Request.QueryString("Comment_Id")) AndAlso Request("Comment_Id").Length > 0 Then
			CommentId = Convert.ToInt64(Request.QueryString("Comment_Id"))
		End If
		If Not IsNothing(Request.QueryString("height")) AndAlso Request("height").Length > 0 Then
			height = Convert.ToDouble(Request.QueryString("height"))
		End If
		If Not IsNothing(Request.QueryString("width")) AndAlso Request.QueryString("width").Length > 0 Then
			width = Convert.ToDouble(Request.QueryString("width"))
		End If
		lCounter = 0
		CurrentUserID = AppUI.UserId


		cContObj = AppUI.EkContentRef

		If ((InStr(UCase(Request.ServerVariables("http_user_agent")), "MOZILLA")) _
		 And (InStr(UCase(Request.ServerVariables("http_user_agent")), "4.7")) _
		 And (Not (InStr(UCase(Request.ServerVariables("http_user_agent")), "GECKO")))) Then
			NS4 = True
		Else
			NS4 = False
		End If

		var2 = cContObj.GetEditorVariablev2_0("0", "tasks")
		RegExpValidator.Validate()
		If Action = "Add" AndAlso RegExpValidator.IsValid Then
			CommentText = Me.commenttext.Content
			If cid <> "" Then
				'Get all tasks associated with the content and add same comment
				taskObj = AppUI.EkTaskRef
				Dim strStates As Object
				strStates = CStr(EkEnumeration.TaskState.NotStarted) & "," & CStr(EkEnumeration.TaskState.Active) & "," & CStr(EkEnumeration.TaskState.AwaitingData) & "," & CStr(EkEnumeration.TaskState.OnHold) & "," & CStr(EkEnumeration.TaskState.Pending) & "," & CStr(EkEnumeration.TaskState.Reopened)
				taskIDs = taskObj.GetTaskIDs(cid, strStates, -1, EkEnumeration.CMSTaskItemType.TasksByStateAndContentID)

				If taskIDs <> "" Then
					tasksArray = Split(taskIDs, ",")
					While lCounter <= UBound(tasksArray)
						retVal = cContObj.AddComment(Convert.ToInt64(CommentKeyId), Convert.ToInt64(CommentId), tasksArray(lCounter), RefType, CurrentUserID, Replace(CommentText, "'", "''"))
						lCounter = lCounter + 1
					End While
				End If

			End If
			Flag = True
		End If
		If True = Flag Then
			sbScript.Append("<script language=""JavaScript"" type=""text/javascript"">" & vbCrLf)
			sbScript.Append("<!--")
			sbScript.Append("if (IsBrowserIE())")
			sbScript.Append("{")
			sbScript.Append("   parent.ReturnChildValue(""action="" + document.getElementById(""actionName"").value + ""&id="" + document.getElementById(""cid"").value + ""&fldid="" + document.getElementById(""fldid"").value + ""&page="" + document.getElementById(""page"").value );")
			sbScript.Append("}")
			sbScript.Append("else")
			sbScript.Append("{")
			sbScript.Append("   top.opener.ReturnChildValue(""action="" + document.getElementById(""actionName"").value + " & ID = " + document.getElementById(""cid"").value + ""&fldid="" + document.getElementById(""fldid"").value + ""&page="" + document.getElementById(""page"").value );")
			sbScript.Append("   close();")
			sbScript.Append("}")
			sbScript.Append("//-->")
			sbScript.Append("</script>" & vbCrLf)
			ClosePanel.Text = sbScript.ToString()
		End If

		If (Request.QueryString("action")) = "Add" Then
			actionName.Value = Request.QueryString("actionName")
		Else
			actionName.Value = Request.QueryString("action")
		End If

		With Me.commenttext
			.AllowFonts = True
		End With
		RegExpValidator.ErrorMessage = MsgHelper.GetMessage("content size exceeded")
		RegExpValidator.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(iMaxContLength)
	End Sub


End Class
