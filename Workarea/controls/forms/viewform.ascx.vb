Imports System.Data
Imports System.Xml
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Content
Imports Ektron.Cms.DataIO.LicenseManager
Imports Ektron.Cms.Site


Partial Class viewform
    Inherits System.Web.UI.UserControl

    Private pagedata As Collection
    Private form_data As FormData
    Private m_strPageAction As String = ""
    Private ContentLanguage As Integer = 0
    Private m_intFormId As Long = 0
    Private m_refContent As Ektron.Cms.Content.EkContent
    Private m_refModule As Ektron.Cms.Modules.EkModule
    Private m_refContentApi As New ContentAPI
    Private m_refSiteApi As New SiteAPI
    Private AppImgPath As String = ""
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Private m_refStyle As New StyleHelper
    Private folder_data As FolderData
    Private security_data As PermissionData
    Private m_intFolderId As Long = 0
    Private m_strOrderBy As String = ""
    Private CurrentUserId As Long = 0
	Private VerifyTrue As String = ""
	Private VerifyFalse As String = ""
	Private EnableMultilingual As Integer = 0
	Private content_data As ContentData
	Private TaskExists As Boolean = False
	Protected m_refMailMsg As New EmailHelper
	Private m_bIsMac As Boolean = False
	Private m_bIsMacInit As Boolean = False

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Me.CreateChildControls()
		m_refMsg = m_refContentApi.EkMsgRef
        RegisterResources()
		If (Not (Request.QueryString("action") Is Nothing)) Then
			If (Request.QueryString("action") <> "") Then
				m_strPageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
			End If
		End If
		If (Not (Request.QueryString("folder_id") Is Nothing)) Then
			If (Request.QueryString("folder_id") <> "") Then
				m_intFolderId = Convert.ToInt64(Request.QueryString("folder_id"))
			End If
		End If
		If (Not (Request.QueryString("form_id") Is Nothing)) Then
			If (Request.QueryString("form_id") <> "") Then
				m_intFormId = Convert.ToInt64(Request.QueryString("form_id"))
			End If
		End If
		If (Not (Request.QueryString("LangType") Is Nothing)) Then
			If (Request.QueryString("LangType") <> "") Then
				ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
				m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
			Else
				If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
					ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
				End If
			End If
		Else
			If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
				ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
			End If
		End If
		If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
			m_refContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
		Else
			m_refContentApi.ContentLanguage = ContentLanguage
		End If
		m_refModule = m_refContentApi.EkModuleRef
		CurrentUserId = m_refContentApi.UserId
		AppImgPath = m_refContentApi.AppImgPath
		EnableMultilingual = m_refContentApi.EnableMultilingual
		VerifyTrue = "<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/check.png"" border=""0"" alt=""Enabled"" title=""Enabled"" />"
		VerifyFalse = "<img src=""" & AppImgPath & "icon_redx.gif"" border=""0"" alt=""Disabled"" title=""Disabled"" />"

		EmailArea.Text = m_refMailMsg.EmailJS
		EmailArea.Text += m_refMailMsg.MakeEmailArea

	End Sub

	Private Sub RegisterResources()
        ''CSS
        'API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronUITabsCss)

        ''JS
        'API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        'API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
        'API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
	End Sub

	Public Sub ViewForm()
		Display_ViewForm()
	End Sub

#Region "VIEWFORM"
	Private Sub Display_ViewForm()
		Dim CanIAddLang As Boolean = True
		Dim TaskExists As Boolean = False
		Dim security_task_data As PermissionData
		m_refContent = m_refContentApi.EkContentRef
		'Check  to see if it's passing it by content-id
		If Request.QueryString("form_content_id") <> "" Then
			m_intFormId = Request.QueryString("form_content_id")
			form_data = m_refContentApi.GetFormById(m_intFormId)
		Else
			form_data = m_refContentApi.GetFormById(m_intFormId)
		End If
		If IsNothing(form_data) Then
			Throw New Exception("Unable to view form. ID=" & m_intFormId)
		End If

		If (Request.QueryString("staged") <> "") Then
			content_data = m_refContentApi.GetContentById(form_data.Id, ContentAPI.ContentResultType.Staged)
		Else
			content_data = m_refContentApi.GetContentById(form_data.Id)
		End If
		TaskExists = m_refContent.DoesTaskExistForContent(form_data.Id)
		security_task_data = m_refContentApi.LoadPermissions(form_data.Id, "tasks", ContentAPI.PermissionResultType.Task)
		security_data = m_refContentApi.LoadPermissions(form_data.Id, "content", ContentAPI.PermissionResultType.All)
		security_data.CanAddTask = security_task_data.CanAddTask
		security_data.CanDestructTask = security_task_data.CanDestructTask
		security_data.CanRedirectTask = security_task_data.CanRedirectTask
		security_data.CanDeleteTask = security_task_data.CanDeleteTask
		CanIAddLang = security_data.CanAdd
		' Replace [srcpath] with actual path. [srcpath] is used in calendar field.
		If Not IsNothing(content_data.Html) AndAlso content_data.Html.Length > 0 Then
			td_vf_content.InnerHtml = content_data.Html.Replace("[srcpath]", m_refContentApi.ApplicationPath & m_refContentApi.AppeWebPath())
			td_vf_content.InnerHtml = td_vf_content.InnerHtml.Replace("[skinpath]", m_refContentApi.ApplicationPath & "csslib/ContentDesigner/")
		Else
			td_vf_content.InnerHtml = content_data.Html
		End If
		If (content_data.Teaser.Length > 0) Then
			td_vf_summary.InnerHtml = DisplayFormDesign(content_data.Teaser)
		Else
			td_vf_summary.InnerHtml = "<p>" & m_refMsg.GetMessage("lbl place post back message here") & "</p>"
		End If
		ViewToolBar()
		Populate_ViewForm(form_data)
	End Sub

	Private Function DisplayFormDesign(ByRef Teaser As String) As String
		Dim bIsRedirect As Boolean = False
		bIsRedirect = (Teaser.IndexOf("<RedirectionLink") > -1)
		If bIsRedirect Then
			If Teaser.IndexOf("<EktReportFormData") > -1 Then
				Dim doc As New XmlDocument
				Dim node As XmlNode
				Dim attr As XmlAttribute
				Dim sDisplayType As String = ""
				Dim sReportType As String = "bar chart"
				Dim sbHtml As New StringBuilder
				sbHtml.Append(m_refMsg.GetMessage("lbl report on the form") & ":" & "<br />")
				doc.LoadXml(Teaser)
				node = doc.SelectSingleNode("//a[1]")
				attr = node.Attributes("id")
				If Not IsNothing(attr) Then
					Select Case (attr.Value.ToLower)
						Case EkEnumeration.CMSFormReportType.Pie
							sReportType = "Pie chart"
						Case EkEnumeration.CMSFormReportType.DataTable
							sReportType = "Data table"
						Case EkEnumeration.CMSFormReportType.Combined
							sReportType = "Combined chart"
						Case EkEnumeration.CMSFormReportType.Bar
							sReportType = "Bar chart"
						Case Else
							sReportType = "Bar chart"
					End Select
					sbHtml.Append(sReportType & " " & m_refMsg.GetMessage("lbl will be displayed on") & " ")
				End If
				attr = Nothing
				attr = node.Attributes("target")
				If Not IsNothing(attr) Then
					If "_self" = attr.Value.ToLower Then
						sDisplayType = m_refMsg.GetMessage("lbl the same window")
					Else
						sDisplayType = m_refMsg.GetMessage("lbl a new window")
					End If
					sbHtml.Append(sDisplayType & ".")
				End If
				Return sbHtml.ToString
			Else
				Return Teaser
			End If
			'Return Teaser.Replace("<a", "<a id=""RedirectionLink""") & vbCrLf & _
			'"<script language=""JavaScript"" type=""text/javascript"" src=""" & m_refContentApi.AppeWebPath() & "java/redirectlink.js""></script>" & vbCrLf
		Else
			Dim strDesign As String
			'Dim strDisplay As String
			If Teaser.IndexOf("<ektdesignpackage_design") > -1 Then
				strDesign = m_refContentApi.XSLTransform(Teaser, _
				  Server.MapPath(m_refContentApi.AppeWebPath() & "unpackageDesign.xslt"), XsltAsFile:=True, _
				  ReturnExceptionMessage:=True)
				Return strDesign
			Else
				Return Teaser
			End If
		End If
	End Function

	Private Sub Populate_ViewForm(ByVal form_data As FormData)
		ViewFormPropertiesGrid.AutoGenerateColumns = False
		Dim colBound As New System.Web.UI.WebControls.BoundColumn
		colBound.DataField = "NAME"
		colBound.Initialize()
		colBound.ItemStyle.CssClass = "label"
		colBound.HeaderStyle.Height = Unit.Empty
		ViewFormPropertiesGrid.Columns.Add(colBound)

		colBound = New System.Web.UI.WebControls.BoundColumn
		colBound.DataField = "TITLE"
		ViewFormPropertiesGrid.Columns.Add(colBound)
		ViewFormPropertiesGrid.BackColor = Drawing.Color.White
		Dim dt As New DataTable
		Dim dr As DataRow

		dt.Columns.Add(New DataColumn("NAME", GetType(String)))
		dt.Columns.Add(New DataColumn("TITLE", GetType(String)))

		dr = dt.NewRow()
		dr(0) = m_refMsg.GetMessage("lbl form title") & ":"
		dr(1) = form_data.Title
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = m_refMsg.GetMessage("lbl formid") & ":"
		dr(1) = form_data.Id
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = m_refMsg.GetMessage("tab linkcheck status") & ":"
		Select Case form_data.Status.ToLower
			Case "a"
				dr(1) = m_refMsg.GetMessage("status:Approved (Published)")
			Case "o"
				dr(1) = m_refMsg.GetMessage("status:Checked Out")
			Case "i"
				dr(1) = m_refMsg.GetMessage("status:Checked In")
			Case "p"
				dr(1) = m_refMsg.GetMessage("status:Approved (PGLD)")
			Case "m"
				dr(1) = "<font color=""Red"">" & m_refMsg.GetMessage("status:Submitted for Deletion") & "</font>"
			Case "s"
				dr(1) = "<font color=""Red"">" & m_refMsg.GetMessage("status:Submitted for Approval") & "</font>"
			Case "t"
				dr(1) = m_refMsg.GetMessage("status:Waiting Approval")
		End Select
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = m_refMsg.GetMessage("description label")
		dr(1) = form_data.Description
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = m_refMsg.GetMessage("lbl form data") & ":"
		Dim bMail As Boolean = False
		Dim bDatabase As Boolean = False
		Dim bAutofillForm As Boolean = False
		Dim lPos As Integer = 0
		Dim strFormData As String = ""
		Dim strFormSubmission As String = ""

		bAutofillForm = form_data.Autofill
		lPos = form_data.StoreDataTo.IndexOf(",$")
		If (lPos > -1) Then
			strFormData = m_refModule.IsEmailOrDb(form_data.StoreDataTo)
			strFormSubmission = form_data.StoreDataTo.Substring(lPos + 2)
		Else
			strFormData = form_data.StoreDataTo
		End If

		'If (strFormData = "") Then
		'    bMail = True
		'End If
		If (strFormData = "mail") Then
			bMail = True
		End If
		If (strFormData = "db") Then
			bDatabase = True
		End If
		If (strFormData = "both") Then
			bMail = True
			bDatabase = True
		End If

		If bMail = True Then
			dr(1) = VerifyTrue
		Else
			dr(1) = VerifyFalse
		End If

		dr(1) += "" & m_refMsg.GetMessage("lbl mail") & "&nbsp;&nbsp;&nbsp;"

		If (bDatabase = True) Then
			dr(1) += VerifyTrue
		Else
			dr(1) += VerifyFalse
		End If
		dr(1) += m_refMsg.GetMessage("lbl database")

		If (bDatabase = True) Then
			dr(1) += "&nbsp;&nbsp;&nbsp;"
			If (bAutofillForm = True) Then
				dr(1) += VerifyTrue
			Else
				dr(1) += VerifyFalse
			End If
			dr(1) += m_refMsg.GetMessage("lbl autofill form values")
		End If

		dt.Rows.Add(dr)

		' Display Limit submission information if there is any
		If (strFormSubmission <> "") Then
			dr = dt.NewRow()
			dr(0) = m_refMsg.GetMessage("lbl form submission")
			dr(1) = strFormSubmission
			dt.Rows.Add(dr)
		End If

		Dim objTasks As EkTasks
		Dim cTask As EkTask
		cTask = m_refContentApi.EkTaskRef
		objTasks = cTask.GetTasks(form_data.Id, TaskState.Prototype, -1, CMSTaskItemType.TasksByStateAndContentID)
		If Not IsNothing(objTasks) AndAlso objTasks.Count > 0 Then
			cTask = objTasks.Item(1)
		End If

		Dim strAssignTaskTo As String
		If (cTask.AssignToUserGroupID = 0) Then
			strAssignTaskTo = m_refMsg.GetMessage("lbl all authors")
		ElseIf (cTask.AssignedToUser <> "") Then
			strAssignTaskTo = "<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/user.png"" align=""absbottom"" />"
			strAssignTaskTo += m_refMailMsg.MakeUserTaskEmailLink(cTask)
		ElseIf (cTask.AssignedToUserGroup <> "") Then
			strAssignTaskTo = "<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/users.png"" align=""absbottom"" />"
			strAssignTaskTo += m_refMailMsg.MakeUserGroupTaskEmailLink(cTask)
		Else
			strAssignTaskTo = m_refMsg.GetMessage("lbl unassigned")
		End If
		If strAssignTaskTo.Length > 0 Then
			dr = dt.NewRow()
			dr(0) = m_refMsg.GetMessage("lbl assign task to") & ":"
			dr(1) = strAssignTaskTo
			dt.Rows.Add(dr)
		End If

		If (strFormData = "mail" Or strFormData = "both") Then
			dr = dt.NewRow()
			dr(0) = "<hr />"
			dr(1) = "info-header"
			dt.Rows.Add(dr)

			dr = dt.NewRow()
			dr(0) = " " & m_refMsg.GetMessage("lbl mailproperties")	'"<span class=""info-header""> Mail Properties</span>"
			dr(1) = "info-header"
			dt.Rows.Add(dr)

			dr = dt.NewRow()
			dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("generic to label")
			dr(1) = form_data.MailTo
			dt.Rows.Add(dr)

			dr = dt.NewRow()
			dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("generic from label")
			dr(1) = form_data.MailFrom
			dt.Rows.Add(dr)

			dr = dt.NewRow()
			dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("generic cc label")
			dr(1) = form_data.MailCc
			dt.Rows.Add(dr)

			dr = dt.NewRow()
			dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("generic subject label")
			dr(1) = form_data.MailSubject
			dt.Rows.Add(dr)

			dr = dt.NewRow()
			dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl preamble") & ":"
			dr(1) = form_data.MailPreamble
			dt.Rows.Add(dr)

			dr = dt.NewRow()
			dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("alt send data in xml format") & ":"
			If form_data.SendXmlPacket Then
				dr(1) = m_refMsg.GetMessage("generic yes")
			Else
				dr(1) = m_refMsg.GetMessage("generic no")
			End If
			dt.Rows.Add(dr)
		End If

		dr = dt.NewRow()
		dr(0) = "<hr />"
		dr(1) = "info-header"
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = " " & m_refMsg.GetMessage("btn content properties")	'"<span class=""info-header""> Content Properties</span>"
		dr(1) = "info-header"
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("content title label")
		dr(1) = form_data.Title
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("content id label")
		dr(1) = form_data.Id
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("tab linkcheck status") & ":"
		Select Case form_data.Status.ToLower
			Case "a"
				dr(1) = m_refMsg.GetMessage("status:Approved (Published)")
			Case "o"
				dr(1) = m_refMsg.GetMessage("status:Checked Out")
			Case "i"
				dr(1) = m_refMsg.GetMessage("status:Checked In")
			Case "p"
				dr(1) = m_refMsg.GetMessage("status:Approved (PGLD)")
			Case "m"
				dr(1) = "<font color=""Red"">" & m_refMsg.GetMessage("status:Submitted for Deletion") & "</font>"
			Case "s"
				dr(1) = "<font color=""Red"">" & m_refMsg.GetMessage("status:Submitted for Approval") & "</font>"
			Case "t"
				dr(1) = m_refMsg.GetMessage("status:Waiting Approval")
		End Select
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("generic last editor") & ":"
		dr(1) = form_data.EditorFirstName & " " & form_data.EditorLastName
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("generic start date label")
		If (form_data.DisplayGoLive = "") Then
			dr(1) = m_refMsg.GetMessage("none specified msg")
		Else
			dr(1) = form_data.DisplayGoLive
		End If
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("generic end date label")
		If (form_data.DisplayEndDate = "") Then
			dr(1) = m_refMsg.GetMessage("none specified msg")
		Else
			dr(1) = form_data.DisplayEndDate
		End If
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("End Date Action Title") & ":"
		If (form_data.DisplayEndDate.Length > 0) Then
			If (form_data.EndDateAction = EndDateActionType_archive_display) Then
				dr(1) = m_refMsg.GetMessage("Archive display descrp")
			ElseIf (form_data.EndDateAction = EndDateActionType_refresh) Then
				dr(1) = m_refMsg.GetMessage("Refresh descrp")
			Else
				dr(1) = m_refMsg.GetMessage("Archive expire descrp")
			End If
		Else
			dr(1) = m_refMsg.GetMessage("none specified msg")
		End If
		dt.Rows.Add(dr)

		dr = dt.NewRow()
		dr(0) = "&nbsp;&nbsp;&nbsp;" & m_refMsg.GetMessage("content dc label")
		If (form_data.DisplayDateCreated = "") Then
			dr(1) = m_refMsg.GetMessage("none specified msg")
		Else
			dr(1) = form_data.DisplayDateCreated
		End If
		dt.Rows.Add(dr)

		Dim dv As New DataView(dt)
		ViewFormPropertiesGrid.DataSource = dv
		ViewFormPropertiesGrid.DataBind()
	End Sub
	Protected Sub ViewFormPropertiesGrid_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
		Select Case e.Item.ItemType
			Case ListItemType.AlternatingItem, ListItemType.Item
				If e.Item.Cells(1).Text.Equals("info-header") Then
					e.Item.Cells(0).Attributes.Add("align", "Left")
					e.Item.Cells(0).ColumnSpan = 2
					e.Item.Cells(0).CssClass = "info-header"
					e.Item.Cells.RemoveAt(1)
				End If
		End Select
	End Sub
	Private Sub ViewToolBar()
		Dim result As System.Text.StringBuilder
		Dim strBackPage As String = ""
		Dim content_state_data As ContentStateData
		result = New System.Text.StringBuilder
		txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view forms title") & " """ & form_data.Title & """")
		result.Append("<table><tr>")
		strBackPage = "LangType=" & ContentLanguage & "&Action=ViewForm&form_id=" & m_intFormId
		strBackPage = Server.UrlEncode(strBackPage)
		If (security_data.CanEdit) Then
			' Currently, we do not support editing forms on the Mac:
			'If (Not IsMac()) Then  'Editing forms is now supported on MAC so making the change rquested in #34748
			result.Append(m_refStyle.GetEditAnchor(form_data.Id, 2) & vbCrLf)
			'End If

			If (form_data.Status = "O") Then
				If security_data.IsAdmin Then
					'this is the adim so allow for the check in button
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/checkIn.png", "content.aspx?LangType=" & ContentLanguage & "&action=CheckIn&id=" & form_data.Id & "&fldid=" & form_data.FolderId & "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" & form_data.Id, m_refMsg.GetMessage("alt checkin button text"), m_refMsg.GetMessage("btn checkin"), "OnClick=""DisplayHoldMsg(true);return true;"""))
				Else
					' go find out the state of this contet to see it this user can check it in.
					content_state_data = m_refContentApi.GetContentState(form_data.Id)
					If (content_state_data.CurrentUserId = m_refContentApi.UserId) Then
						result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/checkIn.png", "content.aspx?LangType=" & ContentLanguage & "&action=CheckIn&id=" & form_data.Id & "&fldid=" & form_data.FolderId & "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" & form_data.Id, m_refMsg.GetMessage("alt checkin button text"), m_refMsg.GetMessage("btn checkin"), "OnClick=""DisplayHoldMsg(true);return true;"""))
					End If
				End If
			ElseIf (((form_data.Status = "I") Or (form_data.Status = "T")) And (content_data.UserId = m_refContentApi.UserId)) Then
				If (security_data.CanPublish) Then
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentPublish.png", "content.aspx?LangType=" & ContentLanguage & "&action=Submit&id=" & form_data.Id & "&fldid=" & form_data.FolderId & "&page=workarea&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" & m_intFormId & "&parm3=LangType&value3=" & ContentLanguage, m_refMsg.GetMessage("alt publish button text"), m_refMsg.GetMessage("btn publish"), "OnClick=""DisplayHoldMsg(true);return true;"""))
				Else
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/approvalSubmitFor.png", "content.aspx?LangType=" & ContentLanguage & "&action=Submit&id=" & form_data.Id & "&fldid=" & form_data.FolderId & "&page=workarea&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" & m_intFormId & "&parm3=LangType&value3=" & ContentLanguage, m_refMsg.GetMessage("alt submit button text"), m_refMsg.GetMessage("btn submit"), "OnClick=""DisplayHoldMsg(true);return true;"""))
				End If
			End If
			If form_data.Status = "S" Or form_data.Status = "I" Or form_data.Status = "T" Or form_data.Status = "O" Or form_data.Status = "P" Then
				If (Request.QueryString("staged") <> "") Then
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentViewPublished.png", "cmsform.aspx?LangType=" & ContentLanguage & "&action=ViewForm&form_id=" & m_intFormId & "&folder_id=" & form_data.FolderId, m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), ""))
				Else
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/preview.png", "cmsform.aspx?LangType=" & ContentLanguage & "&action=ViewForm&form_id=" & m_intFormId & "&folder_id=" & form_data.FolderId & "&staged=true&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&value2=form_id&value2=" & m_intFormId, m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), ""))
				End If
			End If
		End If
		If form_data.Status = "S" Or form_data.Status = "M" Then
			If (security_data.CanEditSumit) Then
				' Don't show edit button for Mac when using XML config:
				Dim SelectedEditControl As String = Utilities.GetEditorPreference(Request)
				If (Not (m_bIsMac AndAlso (Not IsNothing(content_data.XmlConfiguration))) Or SelectedEditControl = "ContentDesigner") Then
					result.Append(m_refStyle.GetEditAnchor(form_data.Id, 2, True) & vbCrLf)
				End If
			End If
		End If
		If (security_data.CanHistory) Then
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/history.png", "#", m_refMsg.GetMessage("alt history button text"), m_refMsg.GetMessage("lbl generic history"), "OnClick=""top.document.getElementById('ek_main').src='historyarea.aspx?action=report&LangType=" & ContentLanguage & "&id=" & form_data.Id & "';return false;"""))
		End If
		If form_data.Status = "S" Or form_data.Status = "I" Or form_data.Status = "T" Or form_data.Status = "O" Then
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentViewDifferences.png", "#", m_refMsg.GetMessage("alt view difference"), m_refMsg.GetMessage("btn view diff"), "onclick=""PopEditWindow('compare.aspx?LangType=" & ContentLanguage & "&id=" & form_data.Id & "', 'Compare', 785, 650, 1, 1);"""))
		End If
		If (security_data.CanEdit) Then
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentEdit.png", "cmsform.aspx?LangType=" & ContentLanguage & "&action=Editform&form_id=" & m_intFormId & "&folder_id=" & form_data.FolderId & "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" & m_intFormId, m_refMsg.GetMessage("alt form prop"), m_refMsg.GetMessage("btn edit prop"), ""))
		End If

		If (security_data.CanDelete) Then
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/delete.png", "content.aspx?LangType=" & ContentLanguage & "&action=submitDelContAction&delete_id=" & form_data.Id & "&folder_id=" & form_data.FolderId & "&form_id=" & form_data.Id & "&callbackpage=content.aspx&parm1=action&value1=viewcontentbycategory&parm2=id&value2=" & form_data.FolderId, m_refMsg.GetMessage("alt del form"), m_refMsg.GetMessage("btn delete"), "onclick=""return ConfirmFormDelete();"""))
		End If
		result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/linkSearch.png", "isearch.aspx?LangType=" & ContentLanguage & "&action=dofindcontent&folderid=0&form_id=" & m_intFormId & "&ObjectType=forms" & "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" & m_intFormId, m_refMsg.GetMessage("alt Check for content that is linked to this"), m_refMsg.GetMessage("btn link search"), ""))
		If (security_data.CanAddTask) Then
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/taskAdd.png", "tasks.aspx?action=AddTask&cid=" & form_data.Id & "&LangType=" & ContentLanguage & "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" & m_intFormId & "&parm3=folder_id&value3=" & form_data.FolderId & "&parm4=LangType&value4=" & ContentLanguage, m_refMsg.GetMessage("btn add task"), m_refMsg.GetMessage("btn add task"), ""))
		End If
		If (TaskExists = True) Then
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_viewtask-nm.gif", "tasks.aspx?LangType=" & ContentLanguage & "&action=viewcontenttask&ty=both&cid=" & form_data.Id & "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" & m_intFormId & "&parm3=folder_id&value3=" & form_data.FolderId & "&parm4=LangType&value4=" & ContentLanguage, m_refMsg.GetMessage("btn view task"), m_refMsg.GetMessage("btn view task"), ""))
		End If

		' Prep-work for adding move-forms capability:
		'If (security_data.IsAdmin) And (content_data.Status = "A") Then
		'    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & ../UI/Icons/contentCopy.png.png, "content.aspx?LangType=" & ContentLanguage & "&action=MoveContent&id=" & m_intFolderId & "&folder_id=" & content_data.FolderId, "Move Content", m_refMsg.GetMessage("btn move content"), ""))
		'End If

		result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/chartBar.png", "cmsformsreport.aspx?LangType=" & ContentLanguage & "&id=" & m_intFormId & "&FormTitle=" & form_data.Title & "&folder_id=" & form_data.FolderId, m_refMsg.GetMessage("alt report"), m_refMsg.GetMessage("btn report"), ""))
		Dim strAction As String
		If Utilities.IsMac Then
			strAction = "EditContentProperties"
		Else
			strAction = "View"
		End If
		result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/properties.png", "content.aspx?LangType=" & ContentLanguage & "&action=" & strAction & "&id=" & form_data.Id & "&callerpage=cmsform.aspx&folder_id=" & form_data.FolderId & "&origurl=" & strBackPage, m_refMsg.GetMessage("generic form prop"), m_refMsg.GetMessage("btn content properties"), ""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?LangType=" & ContentLanguage & "&action=ViewContentByCategory&id=" & form_data.FolderId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))

        'Sync API needs to know folder type to display the eligible sync profiles.
        If (folder_data Is Nothing) Then
            folder_data = m_refContentApi.GetFolderById(content_data.FolderId)
        End If

        Dim site As SiteAPI = New SiteAPI
        Dim ekSiteRef As EkSite = site.EkSiteRef()
        If (m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncAdmin)) AndAlso (LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Feature.eSync)) AndAlso (m_refContentApi.RequestInformationRef.IsSyncEnabled()) Then
            If ((m_strPageAction = "viewform") AndAlso (content_data.Status.ToUpper() = "A") AndAlso ekSiteRef.IsStaged()) Then
                If folder_data.IsDomainFolder() Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "sync_now_data.png", "#", m_refMsg.GetMessage("alt sync content"), m_refMsg.GetMessage("btn sync content"), "OnClick=""Ektron.Sync.checkMultipleConfigs(" & ContentLanguage & "," & m_intFormId & ",'" & content_data.AssetData.Id & "','" & content_data.AssetData.Version & "'," & content_data.FolderId & ",true);return false;"""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "sync_now_data.png", "#", m_refMsg.GetMessage("alt sync content"), m_refMsg.GetMessage("btn sync content"), "OnClick=""Ektron.Sync.checkMultipleConfigs(" & ContentLanguage & "," & m_intFormId & ",'" & content_data.AssetData.Id & "','" & content_data.AssetData.Version & "'," & content_data.FolderId & ",false);return false;"""))
                End If
            End If
        End If

		If EnableMultilingual = 1 Then
			Dim strViewDisplay As String = ""
			Dim strAddDisplay As String = ""
			Dim result_language() As LanguageData
			Dim count As Integer = 0
			Dim m_refAPI As New ContentAPI

			If (security_data.CanEdit) Then
				result.Append(m_refStyle.GetExportTranslationButton("content.aspx?LangType=" & ContentLanguage & "&action=Localize&id=" & m_intFormId & "&folder_id=" & form_data.FolderId & "&ContentType=" & EkConstants.CMSContentType_Forms & "&callbackpage=cmsform.aspx&parm1=action&value1=ViewForm&parm2=form_id&value2=" & m_intFormId & "&parm3=folder_id&value3=" & form_data.FolderId & "&parm4=LangType&value4=" & ContentLanguage, m_refMsg.GetMessage("alt form trans"), Me.m_refMsg.GetMessage("lbl Export for translation")))
			End If

			result_language = m_refAPI.DisplayAddViewLanguage(m_intFormId)
			For count = 0 To result_language.Length - 1
				If (result_language(count).Type = "VIEW") Then
					If (form_data.LanguageId = result_language(count).Id) Then
						strViewDisplay = strViewDisplay & "<option value=" & result_language(count).Id & " selected=""selected"">" & result_language(count).Name & "</option>"
					Else
						strViewDisplay = strViewDisplay & "<option value=" & result_language(count).Id & ">" & result_language(count).Name & "</option>"
					End If
				End If
			Next
			If (strViewDisplay <> "") Then
				result.Append("<td>&nbsp;|&nbsp;</td>")
				result.Append("<td class=""label"">")
				result.Append(m_refMsg.GetMessage("lbl View") & ":")
				result.Append("</td>")
				result.Append("<td>")
				result.Append("<select id=""viewcontent"" name=""viewcontent"" onchange=""LoadContent('frmContent','VIEW');"">")
				result.Append(strViewDisplay)
				result.Append("</select>")
				result.Append("</td>")
			End If
			If (security_data.CanAdd) Then
				For count = 0 To result_language.Length - 1
					If (result_language(count).Type = "ADD") Then
						strAddDisplay = strAddDisplay & "<option value=" & result_language(count).Id & ">" & result_language(count).Name & "</option>"
					End If
				Next
				If (strAddDisplay <> "") Then
					result.Append("<td>&nbsp;&nbsp;</td>")
					result.Append("<td class=""label"">" & m_refMsg.GetMessage("add title") & ":</td>")
					result.Append("<td>")
					result.Append("<select id=""addcontent"" name=""addcontent"" onchange=""LoadContent('frmContent','ADD');"">")
					result.Append("<option value=" & "0" & ">" & "-select language-" & "</option>")
					result.Append(strAddDisplay)
					result.Append("</select></td>")
				End If
			End If
		End If
		result.Append("<td>")
		result.Append(m_refStyle.GetHelpButton(m_strPageAction))
		result.Append("</td>")
		result.Append("</tr></table>")
		htmToolBar.InnerHtml = result.ToString
	End Sub

	Private Function IsMac() As Boolean
		If (Not m_bIsMacInit) Then
			If (Request.Browser.Platform.IndexOf("Win") = -1) Then
				m_bIsMac = True
			Else
				m_bIsMac = False
			End If
			m_bIsMacInit = True
		End If
		Return (m_bIsMac)

	End Function

#End Region
End Class
