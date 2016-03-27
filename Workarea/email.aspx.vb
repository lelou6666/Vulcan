Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Partial Class email
    Inherits System.Web.UI.Page


#Region "Private Members"


    Protected globalWarningMsg As String = ""
    Protected UserId As Long
    Protected GroupId, UserArray, GroupArray, MsgNotes, ContentId, emailclangid, target, source, subject, sMessage As Object
    Protected m_refMsg As Common.EkMessageHelper
    Protected strTargUserEmail, strUserFName, strUserLName, iMaxContLength, localeFileString, strUserName As Object
    Protected m_refMail As New EmailHelper
    Protected AppeWebPath As String = ""
    Protected m_refUser As User.EkUser
    Protected m_refContent As New Ektron.Cms.Content.EkContent
    Protected m_refSiteApi As New SiteAPI
    Protected BrowserCode As String = "en"
    Protected language_data As LanguageData
    Protected ContentLanguage As Integer = -1
    Private fromModal As Boolean = False
    Private commerceAdmin As Boolean = False
    Private cancelJavascript As String = "closeEmailChildPage();"


#End Region


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        Dim var1 As String = ""
        Dim var2 As String = ""

        If Request.QueryString("fromModal") <> "" Then

            fromModal = Request.QueryString("fromModal")
            commerceAdmin = m_refSiteApi.IsARoleMember(Common.EkEnumeration.CmsRoleIds.CommerceAdmin)
            cancelJavascript = "parent.ektb_remove();"

        End If

        RegisterResources()
        m_refMsg = m_refSiteApi.EkMsgRef
        Utilities.ValidateUserLogin()
        If (m_refSiteApi.RequestInformationRef.IsMembershipUser) Then
            Response.Redirect("reterror.aspx?info=" & m_refSiteApi.EkMsgRef.GetMessage("msg login cms user"), True)
            Exit Sub
        End If
        ContentLanguage = m_refSiteApi.ContentLanguage
        If (ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Or ContentLanguage = ALL_CONTENT_LANGUAGES) Then
            ContentLanguage = m_refSiteApi.DefaultContentLanguage
        End If
        language_data = m_refSiteApi.GetLanguageById(ContentLanguage)
        BrowserCode = language_data.BrowserCode
        strTargUserEmail = ""
        strUserFName = ""
        strUserLName = ""
        globalWarningMsg = ""
        iMaxContLength = 65000
        localeFileString = "0000"
        m_refUser = m_refSiteApi.EkUserRef
        m_refContent = m_refSiteApi.EkContentRef
        UserId = Request.QueryString("userid")
        GroupId = Request.QueryString("groupid")
        UserArray = Request.QueryString("userarray")
        GroupArray = Request.QueryString("grouparray")
        MsgNotes = Request.QueryString("notes")
        ContentId = Request.QueryString("contentid")
        emailclangid = Request.QueryString("emailclangid")
        target = Request.Form("target")
        source = Request.Form("source")
        subject = Request.Form("subject")
        sMessage = Me.message.Content
        AppeWebPath = m_refSiteApi.ApplicationPath & m_refSiteApi.AppeWebPath
        var2 = m_refContent.GetEditorVariablev2_0(0, "email")
        EmailData.Text += m_refMail.EmailJS
        DisplayControl()
    End Sub

    Private Sub DisplayControl()
        Dim cUserInfo As New Collection
        Dim cGroups, cGroup, tempString As Object
        If Page.IsPostBack Then
            ' Post-Back, user clicked the "send" button:
            Try
                RegExpValidator.Validate()
                If RegExpValidator.IsValid Then
                    If SendEmail() Then ' Email Sent, hide/close window:

                        EmailData.Text += "<script type=""text/javascript"">" & cancelJavascript & "</script>"

                    End If
                End If
            Catch ex As Exception
                TD_msg.Text = "<div class=""ui-state-error ektronTopSpaceSmall""><span class=""ui-icon warning""></span>" & m_refMsg.GetMessage("Error: SendEmail-function failed") & vbCrLf & ex.Message & "</div>"
            End Try
        Else ' ---------------------------------------------------------------------
            If (GetIntSafe(UserId) > 0) Then
                '
                ' Userid was passed, get info for email target:
                '
                cUserInfo = m_refUser.GetUserEmailInfoByID(UserId)
                If (cUserInfo.Count) Then
                    strUserName = m_refMail.SafeColRead_Email(cUserInfo, "UserName")
                    strUserFName = m_refMail.SafeColRead_Email(cUserInfo, "FirstName")
                    strUserLName = m_refMail.SafeColRead_Email(cUserInfo, "LastName")
                    strTargUserEmail = m_refMail.SafeColRead_Email(cUserInfo, "EmailAddr1")
                End If
                ShowControls()
            ElseIf (GetIntSafe(GroupId) > 0) Then
                '
                ' Groupid was passed, get all users in the group:
                ' Build strTargUserEmail (a comma-delimited string of all user addresses from group):
                '
                cGroups = m_refUser.GetTaskUsersByGroupv2_0(GroupId, "")
                For Each cGroup In cGroups
                    tempString = m_refMail.SafeColRead_Email(cGroup, "EmailAddr1")
                    If (Len(tempString) > 0) Then
                        strTargUserEmail = strTargUserEmail & tempString & ", "
                    End If
                Next
                If (Len(strTargUserEmail) > 2) Then
                    strTargUserEmail = Left(strTargUserEmail, Len(strTargUserEmail) - 2)
                End If
                ShowControls()

            ElseIf ((UserArray <> "") AndAlso (IsNumeric(UserArray))) Then
                '
                ' Build strTargUserEmail (a comma-delimited string of all user addresses, from user ids passed in array):
                '
                UserArray = Split(UserArray, ",")
                For Each UserId In UserArray
                    ' Get info for email target:
                    cUserInfo = m_refUser.GetUserEmailInfoByID(UserId)

                    If (cUserInfo.Count) Then
                        tempString = m_refMail.SafeColRead_Email(cUserInfo, "EmailAddr1")
                        If (Len(tempString) > 0) Then
                            strTargUserEmail = strTargUserEmail & tempString & ", "
                        End If
                    End If

                Next
                If (Len(strTargUserEmail) > 2) Then
                    strTargUserEmail = Left(strTargUserEmail, Len(strTargUserEmail) - 2)
                End If
                ShowControls()

                cUserInfo = Nothing
            ElseIf ((GroupArray <> "") AndAlso (IsNumeric(GroupArray))) Then
                '
                ' Build strTargUserEmail (a comma-delimited string of all user addresses, from group ids passed in array):
                '
                GroupArray = Split(GroupArray, ",")
                For Each GroupId In GroupArray
                    cGroups = m_refUser.GetTaskUsersByGroupv2_0(GroupId, "")
                    For Each cGroup In cGroups
                        tempString = m_refMail.SafeColRead_Email(cGroup, "EmailAddr1")
                        If (Len(tempString) > 0) Then
                            strTargUserEmail = strTargUserEmail & tempString & ", "
                        End If
                    Next

                Next
                If (Len(strTargUserEmail) > 2) Then
                    strTargUserEmail = Left(strTargUserEmail, Len(strTargUserEmail) - 2)
                End If
                ShowControls()

            Else
                ' Error: No information obained from querystring, nothing passed from source:
                ShowError(m_refMsg.GetMessage("Error: Email missing source address") & _
                  ", " & m_refMsg.GetMessage("Error: Email missing target address"))
            End If
        End If
    End Sub
    Private Function SendEmail() As Boolean
        Dim objMailServ As Ektron.Cms.Common.EkMailService
        Try
            If (target <> "") Then
                objMailServ = m_refSiteApi.EkMailRef

                objMailServ.MailTo = target
                objMailServ.MailFrom = source
                objMailServ.MailSubject = subject
                objMailServ.MailBodyText = sMessage
                objMailServ.SendMail()
                objMailServ = Nothing
            Else
                Throw New Exception(m_refMsg.GetMessage("Error: Email missing target address"))
            End If
            Return (True)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            objMailServ = Nothing
        End Try
        ' Exiting the function restores default error handling (crash script).
        ' Alternately could do explicitly: On Error Goto 0
    End Function

    ' ---------------------------------------------------------------------------
    ' Render the email UI:
    Sub ShowControls()
        Dim msgPrefix, tempStr, strSrcUserEmail As Object
        msgPrefix = Nothing
        TD_msg.Text = ""
        ' Attempt to get senders email address (error if empty string).
        ' Note: Should not be able to get here if source address is missing,
        ' as email icons/links will not be shown, but tested here anyway:
        strSrcUserEmail = m_refMail.GetSendersEmailAddress()
        If (strSrcUserEmail = "" AndAlso Not commerceAdmin) Then
            Throw New Exception(m_refMsg.GetMessage("Error: Email missing source address"))
        End If


        ' Warn if destination email address is empty string, if nothing else to show:
        If ((strTargUserEmail = "") And (globalWarningMsg = "")) Then
            globalWarningMsg = "*** " & m_refMsg.GetMessage("Warning: destination address missing") & " ***"
        End If

        ' Prepare to insert username if available:
        If ((strUserFName <> "") Or (strUserLName <> "")) Then
            msgPrefix = strUserFName
            If ((msgPrefix <> "") And (strUserLName <> "")) Then
                msgPrefix = msgPrefix & " "
            End If
            msgPrefix = "<span>" & msgPrefix & strUserLName & ": " & "</span>"
        End If

        ' Add related content url if available (requires content-id to be supplied):
        tempStr = GetContentUrl()
        If (tempStr <> "") Then
            msgPrefix = msgPrefix & "<br/>" & tempStr & "<br/>"
        End If
        Dim result As New System.Text.StringBuilder
        ' Render the UI page:
        If (globalWarningMsg.Length > 0) Then
            result.Append("<div class=""ui-state-error ektronTopSpaceSmall""><span class=""ui-icon warning""></span>" & globalWarningMsg & "</div>")
        End If
        result.Append("<table class=""ektronGrid ektronBorder maxWidth ektronTopSpace"">")
        result.Append("<tr><td>")
        result.Append("<label class=""label right""> " & m_refMsg.GetMessage("generic to label") & "</label></td><td>&nbsp;<input type=""text"" value=""" & RemoveRedunancies(strTargUserEmail) & """ name=""target"" id=""target"" size=""49"" " & IIf(fromModal, "readonly style=""background:#f4f4f4;"" ", "") & " /><br/>")
        result.Append("</td></tr>")
        result.Append("<tr><td>")
        If fromModal AndAlso commerceAdmin Then

            result.Append("<label class=""label right""> " & m_refMsg.GetMessage("generic from label") & "</label></td><td>&nbsp;")
            result.Append("<input type=text value=""" & strSrcUserEmail & """ name=""source"" id=""source"" size=""49"" maxlength=""49""/>")

        Else

            result.Append("<label class=""label right""> " & m_refMsg.GetMessage("generic from label") & "</label></td><td>&nbsp;<label>" & strSrcUserEmail & "</label><br/>")
            result.Append("<input type=""hidden"" value=""" & strSrcUserEmail & """ name=""source"" id=""source"" />")

        End If
        result.Append("<input type=""hidden"" value=""true"" name=""postback"" id=""postback"" />")
        result.Append("</td></tr>")
        result.Append("<tr><td>")
        result.Append("<label class=""label right""> " & m_refMsg.GetMessage("generic subject label") & "</label></td><td>&nbsp;<input type=""text"" value=""" & MsgNotes & """ name=""subject"" id=""subject"" size=""49"" maxlength=""44"" /><br/>")
        result.Append("</td></tr></table>")
        EmailData.Text += result.ToString

        Me.message.Content = "<p>" & msgPrefix & "</p>"
        With Me.message
            .AllowFonts = True
        End With
        RegExpValidator.ErrorMessage = m_refMsg.GetMessage("content size exceeded")
        RegExpValidator.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(iMaxContLength)

        Dim result2 As New System.Text.StringBuilder
        result2.Append("<ul class=""buttonWrapper buttonWrapperLeft ektronTopSpace ui-helper-clearfix""><li><a class=""button buttonInlineBlock greenHover buttonSendMail"" type=""button"" onclick=""submit_form()"" value=""" & m_refMsg.GetMessage("send email button text") & """ name=""send"" id=""send"" runat=""server"" >" & m_refMsg.GetMessage("send email button text") & "</a></li>")
        result2.Append("    <li><a class=""button buttonInlineBlock redHover buttonClear"" type=""button"" onclick=""" & cancelJavascript & """ value=""" & m_refMsg.GetMessage("generic Cancel") & """ name=""cancel"" id=""cancel"" >" & m_refMsg.GetMessage("generic Cancel") & "</a></li></ul>")
        EmailData2.Text += result2.ToString
    End Sub
    ' ---------------------------------------------------------------------------
    ' Show the error message and a Cancel/Exit button:
    Sub ShowError(ByRef ErrorMsg As Object)
        Dim result As New System.Text.StringBuilder
        result.Append("<div class=""ui-state-error ektronTopSpaceSmall"">")
        result.Append("<span class=""ui-icon warning""></span>")
        result.Append(ErrorMsg)
        result.Append("<ul class=""buttonWrapper buttonWrapperLeft ektronTopSpace ui-helper-clearfix""><li><input type=""button"" onclick=""" & cancelJavascript & """ value=""" & m_refMsg.GetMessage("generic Cancel") & """ name=""cancel"" id=""cancel""></li></ul>")
        result.Append("</div>")
        TD_msg.Text = result.ToString
    End Sub
    ' ---------------------------------------------------------------------------
    Function GetContentUrl() As Object
        Dim cCont, quickLink, tempStr, urlStr, ErrString, QorAmp, folderid As Object
        ErrString = Nothing
        GetContentUrl = ""
        If (GetIntSafe(ContentId) > 0) Then
            cCont = m_refContent.ShowContentv2_0(ContentId, False)
            folderid = cCont.FolderID
            If (folderid <> "") Then
                If (folderid >= 0) Then
                    quickLink = m_refContent.GetContentQlink(ContentId, folderid)
                    If (ErrString = "") Then
                        If (quickLink <> "") Then
                            tempStr = Request.ServerVariables("HTTPS")
                            If (InStr(LCase(tempStr), "off") <> -1) Then
                                urlStr = "http://"
                            Else
                                urlStr = "https://"
                            End If

                            urlStr = urlStr & Request.ServerVariables("HTTP_HOST")

                            tempStr = Trim(CStr(CInt(Request.ServerVariables("SERVER_PORT"))))
                            If (CInt(tempStr) <> 80) Then
                                urlStr = urlStr & ":" & tempStr
                            End If

                            If (Left(quickLink, 1) <> "/") Then
                                quickLink = "/" & quickLink
                            End If

                            If (InStr(quickLink, "?") > 0) Then
                                QorAmp = "&"
                            Else
                                QorAmp = "?"
                            End If

                            If (emailclangid <> "") Then
                                urlStr = urlStr & quickLink & QorAmp & "LangType=" & emailclangid
                            Else
                                urlStr = urlStr & quickLink & QorAmp & "LangType=" & m_refSiteApi.ContentLanguage
                            End If

                            GetContentUrl = "<span>URL:</span> <a href=""" & Server.HtmlEncode(urlStr) & """>" & Server.HtmlEncode(urlStr) & "</a>"
                        End If ' (quickLink <> "")
                    End If ' (ErrString = "")
                End If ' (folderId >= 0)
            End If ' (folderId <> "")

        End If

    End Function
    ' ---------------------------------------------------------------------------
    ' Remove redundant names from the supplied text-string (assumes semicolon
    ' delimiter). Scans supplied string for a match, if none found then includes
    ' in returned string.
    Function RemoveRedunancies(ByRef textStr As Object) As Object
        Dim retVal, arrayStr, index, index2, limit As Object

        retVal = ""
        If (Len(textStr) > 0) Then
            arrayStr = Split(textStr, ",")
            limit = UBound(arrayStr)
            ' Remove whitespace from around items:
            For index = 0 To limit
                arrayStr(index) = Trim(arrayStr(index))
            Next
            ' Scan for duplicates:
            For index = 0 To limit
                For index2 = (index + 1) To limit
                    If (arrayStr(index) = arrayStr(index2)) Then
                        Exit For
                    End If
                Next
                ' Append item is match was not found:
                If (index2 > limit) Then
                    retVal = retVal & arrayStr(index) & ", "
                End If
            Next
            If (Len(retVal) > 2) Then
                retVal = Left(retVal, Len(retVal) - 2)
            End If
        End If
        RemoveRedunancies = retVal
    End Function
    '----------------------------------------------------------------------------
    ' GetIntSafe:
    ' Returns either the integer conversion, or zero (if empty string supplied).
    Function GetIntSafe(ByRef strVal As Object) As Object
        Dim retVal As Long = 0
        If (Not (IsDBNull(strVal))) Then
            If (Convert.ToString(strVal) <> "") Then
                retVal = CLng(strVal)
            End If
        End If
        GetIntSafe = retVal
    End Function
    Protected Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
    End Sub
End Class
