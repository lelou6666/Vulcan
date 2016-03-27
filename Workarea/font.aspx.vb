Imports System.Data
Imports Ektron.Cms
Partial Class ekfont
	Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

	'This call is required by the Web Form Designer.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

	End Sub


	Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
		'CODEGEN: This method call is required by the Web Form Designer
		'Do not modify it using the code editor.
		InitializeComponent()
	End Sub
	Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
	Protected m_refStyle As New StyleHelper
	Protected m_strPageAction As String = ""
    Protected AppPath As String = ""
    Protected m_intFontId As Long = 0
	Protected font_data As FontData
	Protected m_refContApi As New ContentAPI
#End Region

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		'Put user code to initialize the page here
		Try
			m_refMsg = m_refContApi.EkMsgRef
			StyleSheetJS.Text = m_refStyle.GetClientScript
			If (Not (IsNothing(Request.QueryString("action")))) Then
				m_strPageAction = Request.QueryString("action")
				If (m_strPageAction.Length > 0) Then
					m_strPageAction = m_strPageAction.ToLower
				End If
            End If
            If ((m_refContApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn") = False) Then
                Response.Redirect("login.aspx?fromLnkPg=1", False)
                Exit Sub
            End If
            If m_refContApi.RequestInformationRef.IsMembershipUser Or m_refContApi.RequestInformationRef.UserId = 0 Then
                Response.Redirect("reterror.aspx?info=Please login as cms user", False)
                Exit Sub
            End If
            RegisterResources()
            SetJSServerVariables()
            AppPath = m_refContApi.AppPath
			TR_AddEditFont.Visible = False
			TR_ViewFont.Visible = False
			TR_ViewAllFont.Visible = False
			If (Not (Page.IsPostBack)) Then
				Select Case m_strPageAction
					Case "viewfontsbygroup"
						Display_ViewAllFont()
					Case "view"
						Display_ViewFont()
					Case "edit"
						Display_EditFont()
					Case "add"
						Display_AddFont()
					Case "delete"
						Process_DeleteFont()
				End Select
			Else
				Select Case m_strPageAction
					Case "edit"
						Process_EditFont()
					Case "add"
						Process_AddFont()
					Case "delete"
						Process_DeleteFont()
				End Select
			End If
		Catch ex As Exception
			Utilities.ShowError(Server.UrlEncode(ex.Message))
		End Try
	End Sub
	Private Sub Process_EditFont()
		Dim pagedata As Collection
		pagedata = New Collection
		pagedata.Add(Request.Form("FontID"), "FontID")
		pagedata.Add(Request.Form("FontFace"), "FontFace")
		m_refContApi.UpdateFont(pagedata)
		Response.Redirect("font.aspx?action=viewfontsbygroup", False)
	End Sub
	Private Sub Process_AddFont()
		Dim pagedata As Collection
		pagedata = New Collection
		pagedata.Add(Request.Form("FontFace"), "FontFace")
		m_refContApi.AddFont(pagedata)
		Response.Redirect("font.aspx?action=viewfontsbygroup", False)
	End Sub
	Private Sub Process_DeleteFont()
		Dim pagedata As Collection
		pagedata = New Collection
		pagedata.Add(Request.QueryString("FontID"), "FontID")
		m_refContApi.DeleteFont(pagedata)
		Response.Redirect("font.aspx?action=viewfontsbygroup", False)
	End Sub
	Private Sub Display_EditFont()
		TR_AddEditFont.Visible = True
		If (Not (IsNothing(Request.QueryString("id")))) Then
			m_intFontId = Request.QueryString("id")
		End If
		font_data = m_refContApi.GetFontById(m_intFontId)
		FontFace.Value = font_data.Face
		FontID.Value = font_data.Id
		EditFontToolBar()
	End Sub
	Private Sub Display_AddFont()
		TR_AddEditFont.Visible = True
		AddFontToolBar()
	End Sub
	Private Sub Display_ViewFont()
		TR_ViewFont.Visible = True
		If (Not (IsNothing(Request.QueryString("id")))) Then
			m_intFontId = Request.QueryString("id")
		End If
		font_data = m_refContApi.GetFontById(m_intFontId)
		TD_FontFace.InnerHtml = font_data.Face
		ViewFontToolBar()
	End Sub
	Private Sub Display_ViewAllFont()
		TR_ViewAllFont.Visible = True
		Dim font_data_list As FontData()
		font_data_list = m_refContApi.GetAllFonts
		If (Not (IsNothing(font_data_list))) Then
			Dim colBound As New System.Web.UI.WebControls.BoundColumn
			colBound.DataField = "ID"
			colBound.HeaderText = m_refMsg.GetMessage("generic Fontname")
			ViewFontGrid.Columns.Add(colBound)

			colBound = New System.Web.UI.WebControls.BoundColumn
			colBound.DataField = "TITLE"
			colBound.HeaderText = m_refMsg.GetMessage("generic Font Face Sample")
			ViewFontGrid.Columns.Add(colBound)
			Dim dt As New DataTable
			Dim dr As DataRow
			Dim i As Integer = 0
			dt.Columns.Add(New DataColumn("ID", GetType(String)))
			dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
			For i = 0 To font_data_list.Length - 1
				dr = dt.NewRow
				dr(0) = "<a href=""font.aspx?action=View&id=" & font_data_list(i).Id & """ title='" & m_refMsg.GetMessage("click to view font msg") & " """ & Replace(font_data_list(i).Face, "'", "`") & """'>" & font_data_list(i).Face & "</a>"
				dr(1) = "<font face=""" & font_data_list(i).Face & """>" & m_refMsg.GetMessage("sample font face style") & "</font>"

				dt.Rows.Add(dr)
			Next
			ViewFontGrid.BorderColor = Drawing.Color.White
			Dim dv As New DataView(dt)
			ViewFontGrid.DataSource = dv
			ViewFontGrid.DataBind()
		End If
		ViewFontsByGroupToolBar()
	End Sub
	Private Sub AddFontToolBar()
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add font page title"))
		Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (font)"), m_refMsg.GetMessage("btn save"), "Onclick=""javascript:return SubmitForm( 'VerifyForm()');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "font.aspx?action=ViewFontsByGroup", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>" & m_refStyle.GetHelpButton("AddFont") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub ViewFontToolBar()
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view system font msg") & " """ & font_data.Face & """")
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "font.aspx?action=Edit&id=" & m_intFontId & "", m_refMsg.GetMessage("alt edit button text (font)"), m_refMsg.GetMessage("btn edit"), ""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "font.aspx?action=delete&FontID=" & m_intFontId & "", m_refMsg.GetMessage("alt delete button text (font)"), m_refMsg.GetMessage("btn delete"), "OnClick=""javascript: return ConfirmFontDelete();"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "font.aspx?action=ViewFontsByGroup", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>" & m_refStyle.GetHelpButton("viewfonts") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub ViewFontsByGroupToolBar()
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view system fonts msg"))
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "font.aspx?action=Add", m_refMsg.GetMessage("alt add button text (fonts)"), m_refMsg.GetMessage("btn add font"), ""))
        result.Append("<td>" & m_refStyle.GetHelpButton("ViewFontsByGroup") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub EditFontToolBar()
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("edit font page title") & " """ & font_data.Face & """")
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "#", m_refMsg.GetMessage("alt update button text (font)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('VerifyForm()');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "font.aspx?action=View&id=" & Request.QueryString("id") & "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>" & m_refStyle.GetHelpButton("EditFont") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
    End Sub
    Private Sub SetJSServerVariables()
        jsFontNameRequiredMsg.Text = m_refMsg.GetMessage("font name required msg")
        jsConfirmDeleteFont.Text = m_refMsg.GetMessage("js: confirm delete font")
    End Sub
End Class
