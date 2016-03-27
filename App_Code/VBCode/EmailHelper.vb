Imports System.Web.HttpContext
Imports Ektron.Cms
Imports Microsoft.VisualBasic
Imports System.Web

Public Class EmailHelper
    Private gtMessEmail As Ektron.Cms.Common.EkMessageHelper
    Private senderEmailAddressInitialized As Object
    Private senderEmailAddressStr As Object
    Private AppImgPath As String
    Private AppPath As String

    Public Sub New()
        Dim objCommonApi As New CommonApi
        gtMessEmail = objCommonApi.EkMsgRef
        AppImgPath = objCommonApi.AppImgPath
        AppPath = objCommonApi.AppPath
    End Sub
    ' ---------------------------------------------------------------------------
    ' Create an area for the email window to open (draws ontop of a transparent layer, that covers the 
    ' parent window - to ensure that the user doesn't accidentally click on the parents' controls):
    Public Function MakeEmailArea() As String
		Dim result As New System.Text.StringBuilder
		If (Utilities.IsBrowserIE()) Then
            result.Append("<div allowtransparency=""true"" id=""EmailActiveOverlay"" style=""position: absolute; top: 0px; left: 0px; width: 1px; height: 1px; display: none; z-index: 1; background-color: transparent; "">")
            result.Append("<iframe allowtransparency=""true"" id=""EmailOverlayChildPage"" name=""EmailOverlayChildPage"" frameborder=""no"" marginheight=""0"" marginwidth=""0"" width=""100%"" height=""100%"" scrolling=""no"" style=""background-color: transparent; background: transparent; FILTER: chroma(color=#FFFFFF)"">")
            result.Append("</iframe>")
            result.Append("</div>")
        End If
        result.Append("<div id=""EmailFrameContainer"" style=""position: absolute; top: 48px; left: 55px; width: 1px; height: 1px; display: none; z-index: 2; Background-color: white; Border-Style: Outset"">")
        result.Append("<iframe id=""EmailChildPage"" name=""EmailChildPage"" frameborder=""yes"" marginheight=""0"" marginwidth=""0"" width=""100%"" height=""100%"" scrolling=""auto"">")
        result.Append("</iframe>")
        result.Append("</div>")
        Return (result.ToString)
    End Function

    ' ---------------------------------------------------------------------------
    Public Function DoesKeyExist_Email(ByRef collectionObject As Collection, ByRef keyName As String) As Boolean
        Dim dummy As Object
        On Error Resume Next ' Used to determine condition, only affects this procedure 
        ' (reverts back to previous method when out of scope).
        Err.Clear()
        dummy = collectionObject.Item(keyName)
        If (Err.Number = 0) Then
            DoesKeyExist_Email = True
        Else
            DoesKeyExist_Email = False
        End If
    End Function

    ' ---------------------------------------------------------------------------
    ' Safe Collection Reading (returns empty string if key-item doesn't exist):
    Public Function SafeColRead_Email(ByRef collectionObject As Collection, ByRef keyName As String) As String
        If (DoesKeyExist_Email(collectionObject, keyName)) Then
            SafeColRead_Email = collectionObject(keyName)
        Else
            SafeColRead_Email = ""
        End If
    End Function

    ' ---------------------------------------------------------------------------
    ' Creates the notes (subject) section of the hyperlink and query string:
    Public Function MakeNotes_Email(ByRef notesName As String, ByRef notesText As String) As String
        MakeNotes_Email = "&notes=" & HttpContext.Current.Server.UrlEncode(notesName & ": " & Replace(notesText, "'", "&apos;"))
    End Function

    ' ---------------------------------------------------------------------------
    ' Creates the notes (subject) section of the hyperlink and query string, using supplied collection:
    Public Function MakeNotesFmCol_Email(ByRef notesName As String, ByRef collectionObject As Collection, ByRef keyName As String) As String
        If (DoesKeyExist_Email(collectionObject, keyName)) Then
            MakeNotesFmCol_Email = MakeNotes_Email(notesName, collectionObject(keyName))
        Else
            MakeNotesFmCol_Email = ""
        End If
    End Function

    ' ---------------------------------------------------------------------------
    ' Creates the content-identification section of the hyperlink and query 
    ' string, attempt to determine content language from collection object):
    Public Function MakeContentId_Email(ByRef collectionObject As Collection, ByRef keyName As String) As String
        Dim retStr As String
        If (DoesKeyExist_Email(collectionObject, keyName)) Then
            retStr = "&contentid=" & collectionObject(keyName)
            If (DoesKeyExist_Email(collectionObject, "ContentLanguage")) Then
                retStr = retStr & "&emailclangid=" & collectionObject("ContentLanguage")
            End If
            MakeContentId_Email = retStr
        Else
            MakeContentId_Email = ""
        End If
    End Function

    ' ---------------------------------------------------------------------------
    ' Builds the email graphic image for the hyperlink, if logged in 
    ' user has a valid email address, otherwise no graphic is added:
    Public Function MakeEmailGraphic() As String
        ' Option: Add code to test for users email address in db, if it doesn't 
        ' exist then do not create graphic...
        ' Downside: this will cause a performance hit that may be unacceptable 
        ' at sites with many users/items to test/display.
        If (IsLoggedInUsersEmailValid()) Then
            MakeEmailGraphic = "<img src=""" & AppPath & "images/ui/icons/email.png"" border=""0"" align=""absbottom"">"
        Else
            MakeEmailGraphic = ""
        End If
    End Function

    ' ---------------------------------------------------------------------------
    ' Builds a hyperlink to launch the email window, user-email-target is task target:
    Public Function MakeUserTaskEmailLink(ByRef taskObj As Object, Optional ByVal blnShowFullName As Boolean = False) As String
        Dim objUserAPI As New UserAPI
        Dim objUserData As New UserData
        Dim strUserFullName As String

        objUserData = objUserAPI.GetActiveUserById(taskObj.AssignedToUserID)
        strUserFullName = objUserData.FirstName & " " & objUserData.LastName

        If blnShowFullName = False Then
            If (IsLoggedInUsersEmailValid()) Then
                MakeUserTaskEmailLink = "<a href=""#""" _
                 & "onclick=""LoadEmailChildPage('userid=" _
                  & taskObj.AssignedToUserID _
                  & MakeNotes_Email("Task", taskObj.TaskTitle) _
                  & "')""" _
                  & " title='" _
                  & gtMessEmail.GetMessage("alt send email to") _
                  & " """ _
                  & Replace(strUserFullName, "'", "`") _
                  & """" _
                  & "'>" _
                  & taskObj.AssignedToUser _
                  & "&nbsp;" _
                  & MakeEmailGraphic() _
                  & "</a>"
            Else
                MakeUserTaskEmailLink = taskObj.AssignedToUser
            End If
        Else
            If (IsLoggedInUsersEmailValid()) Then
                MakeUserTaskEmailLink = "<a href=""#""" _
                 & "onclick=""LoadEmailChildPage('userid=" _
                  & taskObj.AssignedToUserID _
                  & MakeNotes_Email("Task", taskObj.TaskTitle) _
                  & "')""" _
                  & " title='" _
                  & gtMessEmail.GetMessage("alt send email to") _
                  & " """ _
                  & Replace(strUserFullName, "'", "`") _
                  & """" _
                  & "'>" _
                  & strUserFullName & " (" & taskObj.AssignedToUser & ")" _
                  & "&nbsp;" _
                  & MakeEmailGraphic() _
                  & "</a>"
            Else
                MakeUserTaskEmailLink = strUserFullName & " (" & taskObj.AssignedToUser & ")"
            End If
        End If
    End Function

    ' ---------------------------------------------------------------------------
    ' Builds a hyperlink to launch the email window, group-email-target is task target:
    Public Function MakeUserGroupTaskEmailLink(ByRef taskObj As Object) As String
        If (IsLoggedInUsersEmailValid()) Then
            MakeUserGroupTaskEmailLink = "<a href=""#""" _
             & "onclick=""LoadEmailChildPage('groupid=" _
              & taskObj.AssignToUserGroupID _
              & MakeNotes_Email("Task", taskObj.TaskTitle) _
              & "')""" _
              & " title='" _
              & gtMessEmail.GetMessage("alt send email to") _
              & " """ _
              & Replace(taskObj.AssignedToUserGroup, "'", "`") _
              & """" _
              & "'>" _
              & taskObj.AssignedToUserGroup _
              & "&nbsp;" _
              & MakeEmailGraphic() _
              & "</a>"
        Else
            MakeUserGroupTaskEmailLink = taskObj.AssignedToUserGroup
        End If
    End Function

    ' ---------------------------------------------------------------------------
    ' Builds a hyperlink to launch the email window, for task with email-target to task author:
    Public Function MakeByUserTaskEmailLink(ByRef taskObj As Object, Optional ByVal blnShowFullName As Boolean = False) As String
        Dim objUserAPI As New UserAPI
        Dim objUserData As New UserData
        Dim strUserFullName As String

        objUserData = objUserAPI.GetActiveUserById(taskObj.AssignedByUserID)
        strUserFullName = objUserData.FirstName & " " & objUserData.LastName

        If blnShowFullName = False Then
            If (IsLoggedInUsersEmailValid()) Then
                MakeByUserTaskEmailLink = "<a href=""#""" _
                 & "onclick=""LoadEmailChildPage('userid=" _
                  & taskObj.AssignedByUserID _
                  & MakeNotes_Email("Task", taskObj.TaskTitle) _
                  & "')""" _
                  & " title='" _
                  & gtMessEmail.GetMessage("alt send email to") _
                  & " """ _
                  & Replace(strUserFullName, "'", "`") _
                  & """" _
                  & "'>" _
                  & taskObj.AssignedByUser _
                  & "&nbsp;" _
                  & MakeEmailGraphic() _
                  & "</a>"
            Else
                MakeByUserTaskEmailLink = taskObj.AssignedByUser
            End If
        Else
            If (IsLoggedInUsersEmailValid()) Then
                MakeByUserTaskEmailLink = "<a href=""#""" _
                 & "onclick=""LoadEmailChildPage('userid=" _
                  & taskObj.AssignedByUserID _
                  & MakeNotes_Email("Task", taskObj.TaskTitle) _
                  & "')""" _
                  & " title='" _
                  & gtMessEmail.GetMessage("alt send email to") _
                  & " """ _
                  & Replace(strUserFullName, "'", "`") _
                  & """" _
                  & "'>" _
                  & strUserFullName & " (" & taskObj.AssignedByUser & ")" _
                  & "&nbsp;" _
                  & MakeEmailGraphic() _
                  & "</a>"
            Else
                MakeByUserTaskEmailLink = strUserFullName & " (" & taskObj.AssignedByUser & ")"
            End If
        End If
    End Function

    ' ---------------------------------------------------------------------------
    ' Builds a hyperlink to launch the email window, for content item, 
    ' with email-target to the specified collection userKeyName:
    Public Function MakeUserContentEmailLinkKey(ByRef colObj As Collection, ByRef userKeyName As String) As String
        If (DoesKeyExist_Email(colObj, "UserID")) Then
            If (IsLoggedInUsersEmailValid()) Then
                MakeUserContentEmailLinkKey = "<a href=""#""" _
                 & "onclick=""LoadEmailChildPage('userid=" _
                  & colObj("UserID") _
                  & MakeNotesFmCol_Email("Content", colObj, "ContentTitle") _
                  & MakeContentId_Email(colObj, "ContentID") _
                  & "')""" _
                  & " title='" _
                  & gtMessEmail.GetMessage("alt send email to") _
                  & " """ _
                  & Replace(SafeColRead_Email(colObj, userKeyName), "'", "`") _
                  & """" _
                  & "'>" _
                  & SafeColRead_Email(colObj, userKeyName) _
                  & "&nbsp;" _
                  & MakeEmailGraphic() _
                  & "</a>"
            Else
                MakeUserContentEmailLinkKey = SafeColRead_Email(colObj, userKeyName)
            End If
        Else
            MakeUserContentEmailLinkKey = ""
        End If
    End Function

    ' ---------------------------------------------------------------------------
    ' Builds a hyperlink to launch the email window, for content item, 
    ' with email-target to the editors' name (can display name in forward or
    ' reverse order, can hide author name from display - where it will only be shown
    ' for the mouse-hover alt-text/tool-tips):
    Public Function MakeUserEditorContentEmailLink(ByRef colObj As Collection, ByVal reverseName As Boolean, ByVal hideText As Boolean) As String
        Dim nameStr, shownText As String
        If (DoesKeyExist_Email(colObj, "UserID")) Then
            If (reverseName) Then
                nameStr = Replace(SafeColRead_Email(colObj, "EditorLname") & ", " & SafeColRead_Email(colObj, "EditorFname"), "'", "`")
            Else
                nameStr = Replace(SafeColRead_Email(colObj, "EditorFname") & " " & SafeColRead_Email(colObj, "EditorLname"), "'", "`")
            End If
            If (hideText) Then
                shownText = ""
            Else
                shownText = nameStr & "&nbsp;"
            End If

            If (IsLoggedInUsersEmailValid()) Then
                MakeUserEditorContentEmailLink = "<a href=""#""" _
                 & "onclick=""LoadEmailChildPage('userid=" _
                  & colObj("UserID") _
                  & MakeNotesFmCol_Email("Content", colObj, "ContentTitle") _
                  & MakeContentId_Email(colObj, "ContentID") _
                  & "')""" _
                  & " title='" _
                  & gtMessEmail.GetMessage("alt send email to") _
                  & " """ _
                  & nameStr _
                  & """" _
                  & "'>" _
                  & shownText _
                  & MakeEmailGraphic() _
                  & "</a>"
            Else
                MakeUserEditorContentEmailLink = shownText
            End If
        Else
            MakeUserEditorContentEmailLink = ""
        End If
    End Function

    ' ---------------------------------------------------------------------------
    ' Get senders email address; Only hits database once. ErrorString returned 
    ' empty-string, unless error occurs:
    Public Function GetSendersEmailAddress() As String
        Dim Ret As String = ""
        Dim userObj_Email, cUserInfo_Email As Object
        If (senderEmailAddressInitialized) Then
            Ret = senderEmailAddressStr
        Else
            Dim m_refSiteapi As New SiteAPI
            userObj_Email = m_refSiteapi.EkUserRef()
            cUserInfo_Email = userObj_Email.GetUserEmailInfoByID(m_refSiteapi.UserId)
            If (cUserInfo_Email.count) Then
                senderEmailAddressStr = SafeColRead_Email(cUserInfo_Email, "EmailAddr1")
                senderEmailAddressInitialized = True
                Ret = senderEmailAddressStr
            End If
        End If
        cUserInfo_Email = Nothing
        userObj_Email = Nothing
        Return Ret
    End Function
    ' --------------------------------------------------------------------------
    ' Returns true only if the logged in user has a valid (non-empty string) 
    ' email address, otherwise false:
    Public Function IsLoggedInUsersEmailValid() As Boolean
        Dim addressStr As String
        IsLoggedInUsersEmailValid = False
        addressStr = GetSendersEmailAddress()
        If (Len(addressStr) > 0) Then
            IsLoggedInUsersEmailValid = True
        End If
    End Function

#Region " JavaScript Functions "

    Public Function EmailJS() As String
        Dim result As New System.Text.StringBuilder

        result.Append("<script language=""javascript"">" & vbCrLf)
        result.Append("	var g_emailChecked = false;" & vbCrLf)

        result.Append("	function PopUpWindow_Email (url, hWind, nWidth, nHeight, nScroll, nResize) {" & vbCrLf)
        result.Append("		var cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + nWidth + ',height=' + nHeight;" & vbCrLf)
        result.Append("		var popupwin = window.open(url, hWind, cToolBar);" & vbCrLf)
        result.Append("		return popupwin;" & vbCrLf)
        result.Append("	}" & vbCrLf)

        result.Append("	function IsBrowserIE_Email() {" & vbCrLf)
        result.Append("		if (window.location.href.indexOf('override_ie=true') > 0)" & vbCrLf)
        result.Append("			return false;" & vbCrLf)
        result.Append("" & vbCrLf)
        result.Append("		if (null == document.getElementById(""EmailFrameContainer""))" & vbCrLf)
        result.Append("			return false;" & vbCrLf)
        result.Append("" & vbCrLf)
        result.Append("		// document.all is an IE only property" & vbCrLf)
        result.Append("		return (document.all ? true : false);" & vbCrLf)
        result.Append("	}" & vbCrLf)

        result.Append(" function getIEVersion()" & vbCrLf)
        result.Append(" {" & vbCrLf)
        result.Append("   // Returns IE version or -1" & vbCrLf)
        result.Append("   var rv = -1; // Return value assumes failure." & vbCrLf)
        result.Append("   if (navigator.appName == 'Microsoft Internet Explorer')" & vbCrLf)
        result.Append("   {" & vbCrLf)
        result.Append("     var ua = navigator.userAgent;" & vbCrLf)
        result.Append("     var re  = new RegExp(""MSIE ([0-9]{1,}[\.0-9]{0,})"");" & vbCrLf)
        result.Append("     if (re.exec(ua) != null)" & vbCrLf)
        result.Append("       rv = parseFloat( RegExp.$1 );" & vbCrLf)
        result.Append("   }" & vbCrLf)
        result.Append("   return rv;" & vbCrLf)
        result.Append(" }" & vbCrLf)

        result.Append("	function ToggleEmailCheckboxes() {" & vbCrLf)
        result.Append("		var idx, prefix, name;" & vbCrLf)

        result.Append("		g_emailChecked = !g_emailChecked;" & vbCrLf)
        result.Append("for(idx = 0; idx < document.forms[0].elements.length; idx++ ) {" & vbCrLf)
        result.Append("if (document.forms[0].elements[idx].type == ""checkbox"") {" & vbCrLf)
        result.Append("name = document.forms[0].elements[idx].name;" & vbCrLf)
        result.Append("				if (name.indexOf(""emailcheckbox_"") != -1) {" & vbCrLf)
        result.Append("					document.forms[0].elements[idx].checked = g_emailChecked;" & vbCrLf)
        result.Append("				}" & vbCrLf)
        result.Append("			}" & vbCrLf)
        result.Append("		}				" & vbCrLf)
        result.Append("	}			" & vbCrLf)

        result.Append("	// Open email window/layer ontop of current window:" & vbCrLf)
        result.Append("	function LoadEmailChildPage(userGrpId) {" & vbCrLf)
        result.Append("        var pageObj, frameObj;" & vbCrLf)

        result.Append("		if (IsBrowserIE_Email() && getIEVersion() != 6) {" & vbCrLf)
        result.Append("			// Configure the email window to be visible:" & vbCrLf)
        result.Append("			frameObj = document.getElementById(""EmailChildPage"");" & vbCrLf)
        result.Append("			if ((typeof(frameObj) == ""object"") && (frameObj != null)) {" & vbCrLf)
        result.Append("				frameObj.src = ""blankredirect.aspx?email.aspx?"" + userGrpId;" & vbCrLf)
        result.Append("				pageObj = document.getElementById(""EmailFrameContainer"");" & vbCrLf)
        result.Append("				pageObj.style.display = """";" & vbCrLf)
        result.Append("				pageObj.style.width = ""85%"";" & vbCrLf)
        result.Append("				pageObj.style.height = ""90%"";" & vbCrLf)

        result.Append("				// Ensure that the transparent layer completely covers the parent window:" & vbCrLf)
        result.Append("				pageObj = document.getElementById(""EmailActiveOverlay"");" & vbCrLf)
        result.Append("				pageObj.style.display = """";" & vbCrLf)
        result.Append("				pageObj.style.width = ""100%"";" & vbCrLf)
        result.Append("				pageObj.style.height = ""100%"";" & vbCrLf)
        result.Append("			}" & vbCrLf)
        result.Append("		}" & vbCrLf)
        result.Append("		else {" & vbCrLf)
        result.Append("			// Using Netscape; cant use transparencies & eWebEditPro preperly " & vbCrLf)
        result.Append("			// - so launch in a seperate pop-up window:" & vbCrLf)
        result.Append("			PopUpWindow_Email(""blankredirect.aspx?email.aspx?"" + userGrpId,""CMSEmail"",490,500,1,1);" & vbCrLf)
        result.Append("			CloseEmailChildPage();" & vbCrLf)
        result.Append("		}" & vbCrLf)
        result.Append("	}" & vbCrLf)

        result.Append("	// Open email window/layer ontop of current window (extended version, " & vbCrLf)
        result.Append("	// iterates through the controls to determine which usuer/group to add):" & vbCrLf)
        result.Append("	function LoadEmailChildPageEx() {" & vbCrLf)
        result.Append("		var idx, name, prefix, userGrpId, pageObj, qtyElements, frameObj, haveTargets=false;" & vbCrLf)

        result.Append("		// build user-group ID string, based on current check-box status:" & vbCrLf)
        result.Append("		userGrpId = """";" & vbCrLf)
        result.Append("		prefix = ""emailcheckbox_"";" & vbCrLf)
        result.Append("        qtyElements = document.forms[0].elements.length" & vbCrLf)
        result.Append("		for(idx = 0; idx < qtyElements; idx++ ) {" & vbCrLf)
        result.Append("			if (document.forms[0].elements[idx].type == ""checkbox""){" & vbCrLf)
        result.Append("				name = document.forms[0].elements[idx].name;" & vbCrLf)
        result.Append("				if ((name.length > prefix.length)" & vbCrLf)
        result.Append("					&& (document.forms[0].elements[idx].checked == true)) {" & vbCrLf)
        result.Append("					userGrpId = userGrpId + name.substring(prefix.length) + "","";" & vbCrLf)
        result.Append("					haveTargets = true;" & vbCrLf)
        result.Append("				}" & vbCrLf)
        result.Append("			}" & vbCrLf)
        result.Append("		}			" & vbCrLf)
        result.Append("		if (haveTargets) {" & vbCrLf)
        result.Append("			// Build either a user array or a group array:" & vbCrLf)
        result.Append("			if (typeof(document.forms[0].groupMarker) == ""undefined"")" & vbCrLf)
        result.Append("				userGrpId = ""userarray="" + escape(userGrpId.substring(0, userGrpId.length - 1));" & vbCrLf)
        result.Append("        else" & vbCrLf)
        result.Append("				userGrpId = ""grouparray="" + escape(userGrpId.substring(0, userGrpId.length - 1));" & vbCrLf)
        result.Append("			if (IsBrowserIE_Email()) {" & vbCrLf)
        result.Append("				frameObj = document.getElementById(""EmailChildPage"");" & vbCrLf)
        result.Append("				if ((typeof(frameObj) == ""object"") && (frameObj != null)) {" & vbCrLf)
        result.Append("					frameObj.src = ""blankredirect.aspx?email.aspx?"" + userGrpId;" & vbCrLf)
        result.Append("					pageObj = document.getElementById(""EmailFrameContainer"");" & vbCrLf)
        result.Append("					pageObj.style.display = """";" & vbCrLf)
        result.Append("					pageObj.style.width = ""85%"";" & vbCrLf)
        result.Append("					pageObj.style.height = ""90%"";" & vbCrLf)

        result.Append("					pageObj = document.getElementById(""EmailActiveOverlay"");" & vbCrLf)
        result.Append("					pageObj.style.display = """";" & vbCrLf)
        result.Append("					pageObj.style.width = ""100%"";" & vbCrLf)
        result.Append("					pageObj.style.height = ""100%"";" & vbCrLf)
        result.Append("				}" & vbCrLf)
        result.Append("			}" & vbCrLf)
        result.Append("			else {" & vbCrLf)
        result.Append("				PopUpWindow_Email(""blankredirect.aspx?email.aspx?"" + userGrpId,""CMSEmail"",490,500,1,1);" & vbCrLf)
        result.Append("				CloseEmailChildPage();" & vbCrLf)
        result.Append("			}" & vbCrLf)
        result.Append("		}" & vbCrLf)
        result.Append("		else {" & vbCrLf)
        result.Append("			alert(""" & gtMessEmail.GetMessage("email error: No users selected to receive email") & """);" & vbCrLf)
        result.Append("		}" & vbCrLf)
        result.Append("	}" & vbCrLf)

        result.Append("	// Close email window/layer:" & vbCrLf)
        result.Append("	function CloseEmailChildPage() {" & vbCrLf)
		result.Append("		if (IsBrowserIE_Email()) {" & vbCrLf)
		result.Append("			var pageObj = document.getElementById(""EmailFrameContainer"");" & vbCrLf)

		result.Append("			// Configure the email window to be invisible:" & vbCrLf)
		result.Append("			pageObj.style.display = ""none"";" & vbCrLf)
		result.Append("			pageObj.style.width = ""1px"";" & vbCrLf)
		result.Append("			pageObj.style.height = ""1px"";" & vbCrLf)

		result.Append("			// Ensure that the transparent layer does not cover any of the parent window:" & vbCrLf)
		result.Append("			pageObj = document.getElementById(""EmailActiveOverlay"");" & vbCrLf)
		result.Append("			pageObj.style.display = ""none"";" & vbCrLf)
		result.Append("			pageObj.style.width = ""1px"";" & vbCrLf)
		result.Append("			pageObj.style.height = ""1px"";" & vbCrLf)
		result.Append("		}" & vbCrLf)
		result.Append("	}" & vbCrLf)
        result.Append("</script>" & vbCrLf)
        Return (result.ToString)

    End Function
#End Region
End Class
