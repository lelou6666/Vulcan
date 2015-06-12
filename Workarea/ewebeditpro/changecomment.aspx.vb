Imports System

Imports Ektron.Cms.Content
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Controls
Imports Ektron.Cms.UI.CommonUI

Partial Class Workarea_ewebeditpro_changecomment
    Inherits System.Web.UI.Page

    Protected m_strCmdAction As String = ""  ' was Action
    Private m_refContentApi As New ContentAPI
    Protected m_refMsg As Common.EkMessageHelper
    Protected m_strActionType As String = ""
    Protected m_iCommentKeyId As Long = 0
    Protected m_iCommentId As Long = 0
    Protected m_strCommentText as String = ""
    Protected m_iContentLanguage as Integer = 0
    Protected m_iRefId As Long = 0
    Protected m_strRefType as String = ""
    Protected m_strAppeWebPath as string = ""
    Protected m_strLocaleFileString as string = "0000"
    Protected m_objAppUI As ApplicationAPI = Nothing
    Protected m_objContentObj1 as EkContent   ' was cObj1
    Protected m_iCurrentUserId As Long = 0
    Protected m_strAppName as String = ""
    Protected m_strServerName as String = ""  ' was var1
    Protected m_strEditorName as String = ""
    Protected m_strCommentType as String = ""
    Protected m_strOrderBy as String = ""
    Protected m_strvar2 as String = ""
    Protected m_bNS4 as Boolean = False
    Protected m_bResetCommentTag as Boolean = False
    Protected ErrorString as String = ""
    Protected m_bInsertElementFlag As Boolean = False

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            m_refMsg = m_refContentApi.EkMsgRef
            If ((m_refContentApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn") = False) Then
                Response.Redirect("../login.aspx?fromLnkPg=1", False)
                Exit Sub
            Else
                InitEnvironment()
                ActOnEnvironment()

                API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
                API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.AllIE)

                lEditorName.Text = m_strEditorName

                EditorPageClosed.Text = m_refMsg.GetMessage("msg editor page closed")
                If 0 = Len(m_strCmdAction) Or "Edit" = m_strCmdAction Then
                    If m_strCmdAction = "Edit" Then
                        CommentEditor.Content = m_strCommentText
                    End If
                End If
                If m_strCmdAction <> "Edit" And m_strCmdAction <> "Update" Then
                    CommentListHtml.Text = ListAllCommentsInRow()
                End If
            End If
        Catch ex As Exception
            Response.Redirect("../../workarea/reterror.aspx?info=" & Server.UrlEncode(ex.Message))
        End Try
	End Sub

	Protected Function ListAllCommentsInRow() As String

		Dim sbComments As System.Text.StringBuilder = New System.Text.StringBuilder()
		Dim iCnt As Integer
		Dim cComments As Microsoft.VisualBasic.Collection
		Dim cComment As Microsoft.VisualBasic.Collection

		' Ensure we have a good language
		m_objAppUI.ContentLanguage = m_iContentLanguage

		cComments = m_objContentObj1.GetAllComments(m_iCommentKeyId, m_iCommentId, m_iRefId, m_strRefType, m_iCurrentUserId, m_strOrderBy)
		If "" <> ErrorString Then
			Response.Redirect("../reterror.aspx?info=" & ErrorString)
		End If

		iCnt = 1
		For Each cComment In cComments
			If CInt(iCnt / 2) = (iCnt / 2) Then
				If (CLng(cComment("USER_ID")) = CLng(m_iCurrentUserId) Or (CLng(m_iCurrentUserId) = 1)) Then
					sbComments.Append("<tr class=evenrow><td><a href='changecomment.aspx?ref_type=" & m_strRefType & _
						"&editorName=" & m_strEditorName & "&ty=" & m_strActionType & _
						"&orderby=" & Request("orderby") & "&action=Edit&ref_id=" & m_iRefId & _
						"&commentkey_id=" & m_iCommentKeyId & "&comment_id=" & cComment("COMMENT_ID") & "'>" & _
						cComment("DATE_CREATED") & "</a></td><td>" & cComment("FIRST_NAME") & " " & _
						cComment("LAST_NAME") & "</td><td>" & cComment("COMMENTS_TEXT") & "</td></tr>")
				Else
					sbComments.Append("<tr class=evenrow><td>" & cComment("DATE_CREATED") & _
						"</td><td>" & cComment("FIRST_NAME") & " " & cComment("LAST_NAME") & "</td><td>" & _
						cComment("COMMENTS_TEXT") & "</td></tr>")
				End If
			Else
				If (CLng(cComment("USER_ID")) = CLng(m_iCurrentUserId) Or (CLng(m_iCurrentUserId) = 1)) Then
                    sbComments.Append("<tr><td><a href='changecomment.aspx?ref_type=" & m_strRefType & _
                     "&editorName=" & m_strEditorName & "&ty=" & m_strActionType & _
                     "&orderby=" & HttpUtility.HtmlEncode(Request.QueryString("orderby")) & "&action=Edit&ref_id=" & m_iRefId & _
                     "&commentkey_id=" & m_iCommentKeyId & "&comment_id=" & cComment("COMMENT_ID") & "'>" & _
                     cComment("DATE_CREATED") & "</a></td><td>" & cComment("FIRST_NAME") & " " & _
                     cComment("LAST_NAME") & "</td><td>" & cComment("COMMENTS_TEXT") & "</td></tr>")
				Else
					sbComments.Append("<tr><td>" & cComment("DATE_CREATED") & "</td><td>" & _
						cComment("FIRST_NAME") & " " & cComment("LAST_NAME") & "</td><td>" & cComment("COMMENTS_TEXT") & _
						"</td></tr>")
				End If
			End If

			iCnt = iCnt + 1
		Next

		Return sbComments.ToString
	End Function

    Private Function GetSelectedComment(ByVal iCommentKeyId As Long, ByVal iCommentId As Long, _
    ByVal iRefId As Long, ByVal strRefType As String) As String
        Dim sbComments As System.Text.StringBuilder = New System.Text.StringBuilder()
        'Dim iCnt as Integer
        Dim cComments As Microsoft.VisualBasic.Collection
        Dim cComment As Microsoft.VisualBasic.Collection
        Dim strCommentText As String = ""

        ' Ensure we have a good language
        m_objAppUI.ContentLanguage = m_iContentLanguage

        cComments = m_objContentObj1.GetAllComments(iCommentKeyId, iCommentId, iRefId, strRefType, m_iCurrentUserId, "")
        If "" <> ErrorString Then
            Response.Redirect("../reterror.aspx?info=" & ErrorString)
        End If

        ' This will get the last one, if there are more than one.
        For Each cComment In cComments
            strCommentText = cComment("COMMENTS_TEXT")
        Next

        Return strCommentText
    End Function

	Private Sub InitEnvironment()
        m_strCmdAction = HttpUtility.HtmlEncode(Request.QueryString("Action"))
        m_strActionType = HttpUtility.HtmlEncode(Request.QueryString("ty"))
        m_iCommentKeyId = CLng(Request("commentkey_id"))
        m_iCommentId = CLng(Request("Comment_Id"))
        m_strCommentType = HttpUtility.HtmlEncode(Request("comment_type"))
		m_strOrderBy = HttpUtility.HtmlEncode(Request("orderby"))
        m_iRefId = CLng(Request("ref_id"))
        m_strRefType = HttpUtility.HtmlEncode(Request("ref_type"))
        m_strEditorName = HttpUtility.HtmlEncode(Request("editorName"))
		m_strAppeWebPath = ""
		m_strLocaleFileString = "0000"
		m_objAppUI = New ApplicationAPI()
		m_objContentObj1 = m_objAppUI.EkContentRef

		m_strAppeWebPath = m_objAppUI.ApplicationPath & m_objAppUI.AppeWebPath

		If (Request("LangType") <> "") Then
			m_iContentLanguage = Request("LangType")
			m_objAppUI.SetCookieValue("LastValidLanguageID", m_iContentLanguage)
		Else
			If CStr(m_objAppUI.GetCookieValue("LastValidLanguageID")) <> "" Then
				m_iContentLanguage = m_objAppUI.GetCookieValue("LastValidLanguageID")
			End If
		End If

		m_objAppUI.ContentLanguage = m_iContentLanguage
		m_objContentObj1 = m_objAppUI.EkContentRef

		' ??? ekContObj = m_objAppUI.EkContentRef
		'iMaxContLength=65000
		'localeFileString="0000"
		'Messages = " "
		'Flag = False
		'ResetCommentTag = False

		m_iCurrentUserId = m_objAppUI.UserId
		m_strAppeWebPath = m_objAppUI.ApplicationPath & m_objAppUI.AppeWebPath
        m_strServerName = Request.ServerVariables("SERVER_NAME")  ' was var1
        Me.Page.Title = m_objAppUI.AppName & " Comments"

		m_strvar2 = m_objContentObj1.GetEditorVariablev2_0("0", "tasks")

		If ((InStr(UCase(Request.ServerVariables("http_user_agent")), "MOZILLA")) _
		 And (InStr(UCase(Request.ServerVariables("http_user_agent")), "4.7")) _
		 And (Not (InStr(UCase(Request.ServerVariables("http_user_agent")), "GECKO")))) Then
			m_bNS4 = True
		Else
			m_bNS4 = False
		End If

	End Sub

    Private Sub ActOnEnvironment()
        Dim iNewId As Long = 0

        If 0 = m_iCommentKeyId AND "NEW" <> m_strCommentType Then
            iNewId = m_objContentObj1.AddComment(m_iCommentKeyId, m_iCommentId, m_iRefId, m_strRefType, m_iCurrentUserId, HttpUtility.HtmlEncode(Request("commentkey_text")))
	        If "" <> ErrorString Then
		        Response.redirect("../reterror.aspx?info=" & ErrorString)
	        End If
	        m_iCommentKeyId = iNewId
	        m_bResetCommentTag = True
        End If

        Select Case m_strCmdAction
            Case "Add"
				m_strCommentText = CommentEditor.Content
                If m_strCommentText <> "" Then
                    iNewId = m_objContentObj1.AddComment(m_iCommentKeyId, m_iCommentId, m_iRefId, m_strRefType, m_iCurrentUserId, Replace(m_strCommentText, "'", "''"))
                    If "" <> ErrorString Then
                        Response.Redirect("../reterror.aspx?info=" & ErrorString)
                    End If
                    If "NEW" = m_strCommentType Then
                        m_iCommentKeyId = iNewId
                    End If
                    m_bInsertElementFlag = True
                End If
            Case "Edit"
                m_strCommentText = GetSelectedComment(m_iCommentKeyId, m_iCommentId, m_iRefId, m_strRefType)

            Case "Update"
				m_strCommentText = CommentEditor.Content
                ReplaceComment(m_iCommentId, m_strCommentText)

                Response.Redirect("changecomment.aspx?ref_type=" & m_strRefType & "&editorName=" & m_strEditorName & "&ref_id=" & m_iRefId & "&ty=" & Request("ty") & "&commentkey_id=" & m_iCommentKeyId)
        End Select

    End Sub

    Private Sub ReplaceComment(ByVal iCommentId As Long, ByVal strNewComment As String)
        Dim objNew As Object
        Dim strComment As String

        ' Ensure we have a good language
        m_objAppUI.ContentLanguage = m_iContentLanguage

        strComment = Replace(strNewComment, "'", "''")
        objNew = m_objContentObj1.UpdateComment(iCommentId, strComment)
        If "" <> ErrorString Then
            Response.Redirect("../reterror.aspx?info=" & ErrorString)
        End If
    End Sub
End Class
