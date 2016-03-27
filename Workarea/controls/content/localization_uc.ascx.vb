Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports EkDS = Ektron.Cms.LanguageDataSet

Partial Class localization_uc
    Inherits System.Web.UI.UserControl

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Private ekrw As Ektron.Cms.Content.ektUrlRewrite
    Private PermToAlias As Boolean = False

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refSiteApi As New SiteAPI
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_objLocalizationApi As New LocalizationAPI
    Protected m_intId As Long = 0
    Protected folder_data As FolderData
    'Protected security_data As PermissionData
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected ContentType As Integer = 1
    Protected CurrentUserId As Long = 0
    Protected pagedata As Collection
    Protected m_strPageAction As String = ""
    Protected m_strOrderBy As String = ""
    Protected ContentLanguage As Integer = -1
    Protected EnableMultilingual As Integer = 0
    Protected SitePath As String = ""
    Protected content_data As ContentData
    Protected content_state_data As ContentStateData
    Protected m_intFolderId As Long = -1
    Protected CallerPage As String = ""
    Protected TaskExists As Boolean = False
    Private m_refContent As Ektron.Cms.Content.EkContent
    Private m_refTask As Ektron.Cms.Content.EkTask
    Private cTasks As Object
    Protected LanguageName As String = ""
    Protected language_data As LanguageData

    Private Enum CmsTranslatableType
        Content
        Folder
        Menu
        Taxonomy
    End Enum

    Private m_Type As CmsTranslatableType = CmsTranslatableType.Folder
    Private m_bRecursive As Boolean = True

    Private m_refTaskType As Ektron.Cms.Content.EkTaskType
    Private arrTaskTypeID As String()
    Private intCount As Integer
    Private colAllCategory As Collection
    'END: Task Type - LZ
    'subscription - SK
    Private subscription_data_list As SubscriptionData()
    Private subscribed_data_list As SubscriptionData()
    Private subscription_properties_list As SubscriptionPropertiesData
    Private blnBreakSubInheritance As Boolean = False
    Private intInheritFrom As Integer = 0
    Private bGlobalSubInherit As Boolean = False
    Private active_subscription_list As SubscriptionData()
    'END: Subscription - SK
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        Me.CreateChildControls()

        m_refMsg = m_refContentApi.EkMsgRef
        RegisterResources()
    End Sub

    Public Function Display() As Boolean
        m_intId = 0
        If (Not (Request.QueryString("id") Is Nothing)) Then
            m_intId = Convert.ToInt64(Request.QueryString("id"))
        End If
        If (IsPostBack) Then
            m_strPageAction = Request.Form.Item("action").Trim.ToLower
        ElseIf (Not (Request.QueryString("action") Is Nothing)) Then
            m_strPageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If
        If (IsPostBack AndAlso Not IsNothing(ddlSourceLanguage) AndAlso ddlSourceLanguage.Items.Count > 0) Then
            ' SourceLanguageList.SelectedValue doesn't work, not sure why, may be viewstate
            ' or b/c its a user control that's losing state
            Dim strSrcLang As String = Request.Form(ddlSourceLanguage.UniqueID).Trim
            If (IsNumeric(strSrcLang)) Then
                ContentLanguage = Convert.ToInt32(strSrcLang)
            End If
        ElseIf (Not (Request.QueryString("LangType") Is Nothing)) Then
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
        If (CONTENT_LANGUAGES_UNDEFINED = ContentLanguage Or ALL_CONTENT_LANGUAGES = ContentLanguage) Then
            ContentLanguage = m_refContentApi.DefaultContentLanguage
        End If
        m_refContentApi.ContentLanguage = ContentLanguage
        m_objLocalizationApi.ContentLanguage = ContentLanguage

        language_data = m_refSiteApi.GetLanguageById(ContentLanguage)
        LanguageName = language_data.Name
        m_refContent = m_refContentApi.EkContentRef

        CurrentUserId = m_refContentApi.UserId
        AppImgPath = m_refContentApi.AppImgPath
        AppPath = m_refContentApi.AppPath
        SitePath = m_refContentApi.SitePath
        EnableMultilingual = m_refContentApi.EnableMultilingual
        If (Not (IsNothing(Request.QueryString("callerpage")))) Then
            CallerPage = Request.QueryString("callerpage")
        End If

        If (CallerPage = "") Then
            If (Not (IsNothing(Request.QueryString("calledfrom")))) Then
                CallerPage = Request.QueryString("calledfrom")
            End If
        End If
        m_intFolderId = -1
        If (Not (IsNothing(Request.QueryString("folder_id")))) Then
            If (Request.QueryString("folder_id") <> "") Then
                m_intFolderId = Convert.ToInt64(Request.QueryString("folder_id"))
            End If
        End If
        Dim strType As String = Request.QueryString("type")
        If (IsNothing(strType)) Then
            strType = ""
        End If
        strType = strType.Trim.ToLower
        If ("menu" = strType) Then
            m_Type = CmsTranslatableType.Menu
        ElseIf ("taxonomy" = strType) Then
            m_Type = CmsTranslatableType.Taxonomy
        ElseIf (-1 = m_intFolderId) Then
            m_intFolderId = m_intId
            m_intId = 0
            m_Type = CmsTranslatableType.Folder
        ElseIf (m_intId > 0) Then
            m_Type = CmsTranslatableType.Content
        Else
            m_Type = CmsTranslatableType.Folder
        End If

        If (IsPostBack) Then
            If (Not IsNothing(chkRecursive)) Then
                ' chkRecursive.Checked doesn't work, not sure why, may be viewstate
                ' or b/c its a user control that's losing state
                m_bRecursive = (Not IsNothing(Request.Form(chkRecursive.UniqueID)))
                chkRecursive.Checked = m_bRecursive
            Else
                m_bRecursive = True
            End If
        Else
            Dim strRecursive As String = Request.QueryString("recursive")
            If (IsNothing(strRecursive)) Then
                m_bRecursive = True
            Else
                strRecursive = strRecursive.Trim.ToLower
                m_bRecursive = ("true" = strRecursive OrElse "1" = strRecursive OrElse "yes" = strRecursive)
            End If
            chkRecursive.Checked = m_bRecursive
        End If

        If (m_intFolderId <> -1) Then
            folder_data = m_refContentApi.GetFolderById(m_intFolderId)
            If (folder_data Is Nothing) Then
                Response.Redirect("notify_user.aspx", False)
                Exit Function
            End If
        Else
            folder_data = Nothing
        End If

        'Select Case m_Type
        '	Case CmsTranslatableType.Content
        '		security_data = m_refContentApi.LoadPermissions(m_intId, "content", ContentAPI.PermissionResultType.All)
        '	Case CmsTranslatableType.Folder
        '		security_data = m_refContentApi.LoadPermissions(m_intFolderId, "folder", ContentAPI.PermissionResultType.All)
        '	Case CmsTranslatableType.Menu
        '		security_data = m_refContentApi.LoadPermissions(0, "folder", ContentAPI.PermissionResultType.All)
        '	Case Else
        '		Response.Redirect("notify_user.aspx", False)
        '		Exit Function
        'End Select

        If (CmsTranslatableType.Content = m_Type) Then
            content_data = m_refContentApi.GetContentById(m_intId)
            If (content_data Is Nothing) Then
                Response.Redirect("notify_user.aspx", False)
                Exit Function
            End If
        Else
            content_data = Nothing
        End If

        Display_Localization()

        Select Case m_strPageAction
            Case "localizeexport"
                Display_Select(False)
                ExportForTranslation()
            Case Else ' "localize"
                m_strPageAction = "localize"
                Display_Select()
        End Select
    End Function

#Region "LOCALIZATION - Select"
    Private Sub Display_Localization()
        HoldMomentMsg.Text = m_refMsg.GetMessage("one moment msg")

        jsFolderId.Text = m_intFolderId
        jsBackStr.Text = "back_file=content.aspx"
        If (m_strPageAction.Length > 0) Then
            jsBackStr.Text += "&back_action=" & m_strPageAction
        End If
        If (Convert.ToString(m_intFolderId).Length > 0) Then
            jsBackStr.Text += "&back_folder_id=" & m_intFolderId
        End If
        If (Convert.ToString(m_intId).Length > 0) Then
            jsBackStr.Text += "&back_id=" & m_intId
        End If
        If (Convert.ToString(ContentLanguage).Length > 0) Then
            jsBackStr.Text += "&back_LangType=" & ContentLanguage
        End If
        jsToolId.Text = m_intId
        jsToolAction.Text = m_strPageAction
        jsDefaultLanguage.Text = m_refSiteApi.DefaultContentLanguage
		jsSourceLanguageListID.Text = ddlSourceLanguage.UniqueID

        GenerateToolbar()
    End Sub

    Private Sub Display_Select(Optional ByVal Visible As Boolean = True)
        ddlSourceLanguage.Items.Clear()
        pnlForm.Visible = Visible
        If (Visible) Then
            If (CmsTranslatableType.Folder = m_Type) Then
                ' Would be nice to check if folder has subfolders, but no API exists at this
                ' time and it's not a priority at this time to implement it.
                chkRecursive.Visible = True
            Else
                chkRecursive.Visible = False
            End If
            Dim aryLangs() As LanguageData = Nothing
            Select Case m_Type
                Case CmsTranslatableType.Content
                    aryLangs = m_objLocalizationApi.DisplayAddViewLanguage(m_intId)
                Case CmsTranslatableType.Menu
                    aryLangs = m_objLocalizationApi.DisplayAddViewLanguageForMenus()
                    MenuWarning.InnerHtml = "On import content titles are empty when content is not translated in selected languages."
                    MenuWarning.Visible = True
                Case CmsTranslatableType.Folder
                    ' This isn't based on the folder (it would take too long), but is
                    ' at least a decent estimate.
                    aryLangs = m_objLocalizationApi.DisplayAddViewLanguageForAllContent()
                Case Else
                    aryLangs = m_refSiteApi.GetAllActiveLanguages()
            End Select

            If (Not IsNothing(aryLangs) AndAlso aryLangs.Length > 0) Then
                'Dim strLangList As String = ""
                For iLang As Integer = 0 To aryLangs.Length - 1
                    With aryLangs(iLang)
                        If ("VIEW" = .Type) Then
                            'If (strLangList.Length > 0) Then
                            '	strLangList = strLangList & ","
                            'End If
                            'strLangList = strLangList & .Id.ToString
                            ddlSourceLanguage.Items.Add(New ListItem(.LocalName, .Id.ToString))
                        End If
                    End With
                Next
                ddlSourceLanguage.SelectedValue = ContentLanguage.ToString
                'ddlSourceLanguage.Text = m_refStyle.ShowActiveLangsInList(False, "ffffff", "javascript:SelLanguage(this.value);", CONTENT_LANGUAGES_UNDEFINED, strLangList)
            Else
                'ddlSourceLanguage.Text = m_refStyle.ShowAllActiveLanguage(False, "ffffff", "javascript:SelLanguage(this.value);", "")
            End If

            GenerateTargetLanguageList(ContentLanguage)
        Else
            chkRecursive.Visible = False
        End If
    End Sub

    Private Sub GenerateToolbar()
        Dim result As New System.Text.StringBuilder
        Dim sHtmlAction As String = ""

        Dim WorkareaTitlebarTitle As String
        If ("localizeexport" = m_strPageAction) Then
            WorkareaTitlebarTitle = m_refMsg.GetMessage("lbl Download Files")
        Else
            Select Case m_Type
                Case CmsTranslatableType.Content
                    WorkareaTitlebarTitle = String.Format(m_refMsg.GetMessage("alt Export for Translation Content") & """{0}""", content_data.Title)
                Case CmsTranslatableType.Folder
                    WorkareaTitlebarTitle = String.Format(m_refMsg.GetMessage("alt Export for Translation Folder") & """{0}""", folder_data.Name)
                Case CmsTranslatableType.Menu
                    If (0 = m_intId) Then
                        WorkareaTitlebarTitle = m_refMsg.GetMessage("alt Export All Menus for Translation")
                    Else
                        WorkareaTitlebarTitle = String.Format(m_refMsg.GetMessage("alt Export for Translation Menu") & """{0}""", m_intId)
                    End If
                Case CmsTranslatableType.Taxonomy
                    If (0 = m_intId) Then
                        WorkareaTitlebarTitle = m_refMsg.GetMessage("alt export all taxos for translation")
                    Else
                        WorkareaTitlebarTitle = String.Format(m_refMsg.GetMessage("alt export for translation taxonomy") & """{0}""", m_intId)
                    End If
                Case Else
                    Exit Sub
            End Select
        End If
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle)

        Dim strBackAction As String = Request.QueryString("backpage")
        If (strBackAction = String.Empty) Then
            strBackAction = "Localize"
        End If
        Dim strBackId As String = ""
        Select Case m_Type
            Case CmsTranslatableType.Content
                strBackId = "&id=" & m_intId.ToString & "&folder_id=" & m_intFolderId.ToString
            Case CmsTranslatableType.Folder
                strBackId = "&id=" & m_intFolderId.ToString
            Case CmsTranslatableType.Menu
                strBackId = "&id=" & m_intId.ToString
            Case CmsTranslatableType.Taxonomy
                strBackId = "&id=" & m_intId.ToString
            Case Else
                Exit Sub
        End Select

        result.Append("<table><tr>")

        If ("localizeexport" <> m_strPageAction) Then
            'result.Append(m_refStyle.GetButtonEventsWCaption(AppImg & "images/UI/icons/translationExport.png", "content.aspx?LangType=" & ContentLanguage & "&action=LocalizeExport&backpage=Localize" & strBackId, "Click here to create XLIFF files for translation", "Create XLIFF Files for Translation", "onclick='DisplayHoldMsg(true); return validate()'"))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/icons/translationExport.png", "#", m_refMsg.GetMessage("alt Click here to create XLIFF files for translation"), m_refMsg.GetMessage("lbl Create XLIFF Files for Translation"), "onclick='DisplayXLIFFPanel(false); DisplayHoldMsg(true); return SubmitForm(0,""validate()"")'"))
        End If

        Dim strBackPage As String = ""
        If (Request.QueryString("callerpage") <> "") Then
            strBackPage = Request.QueryString("callerpage") & "?" & HttpUtility.UrlDecode(Request.QueryString("origurl"))
        ElseIf (Request.QueryString("backpage") = "history") Then
            strBackPage = "javascript:history.back()"
        Else
            strBackPage = m_refStyle.getCallBackupPage("content.aspx?LangType=" & ContentLanguage & "&action=" & strBackAction & strBackId)
        End If
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", strBackPage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))


        'If EnableMultilingual = 1 Then
        '	Dim strViewDisplay As String = ""
        '	Dim strAddDisplay As String = ""
        '	Dim result_language() As LanguageData
        '	Dim count As Integer = 0
        '	result_language = m_refContentApi.DisplayAddViewLanguage(m_intId)
        '	For count = 0 To result_language.Length - 1
        '		If (result_language(count).Type = "VIEW") Then
        '			If (content_data.LanguageId = result_language(count).Id) Then
        '				strViewDisplay = strViewDisplay & "<option value=" & result_language(count).Id & " selected>" & result_language(count).Name & "</option>"
        '			Else
        '				strViewDisplay = strViewDisplay & "<option value=" & result_language(count).Id & ">" & result_language(count).Name & "</option>"
        '			End If
        '		End If
        '	Next
        '	If (strViewDisplay <> "") Then
        '		result.Append("<td nowrap=""true"">&nbsp;|&nbsp;View:")
        '		result.Append("<select id=localization name=localization OnChange=""javascript:LoadContent('frmContent','VIEW');"">")
        '		result.Append(strViewDisplay)
        '		result.Append("</select></td>")
        '	End If
        '	If (security_data.CanAdd) Then
        '		'If (bCanAddNewLanguage) Then
        '		For count = 0 To result_language.Length - 1
        '			If (result_language(count).Type = "ADD") Then
        '				strAddDisplay = strAddDisplay & "<option value=" & result_language(count).Id & ">" & result_language(count).Name & "</option>"
        '			End If
        '		Next
        '		If (strAddDisplay <> "") Then
        '			result.Append("<td nowrap=""true"">&nbsp;|&nbsp;Add:")
        '			result.Append("<select id=addcontent name=addcontent OnChange=""javascript:LoadContent('frmContent','ADD');"">")
        '			result.Append("<option value=" & "0" & ">" & "-select language-" & "</option>")
        '			result.Append(strAddDisplay)
        '			result.Append("</select></td>")
        '		End If
        '		'End If
        '	End If
        'End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
		result.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>")
		result.Append("</tr></table>")
		htmToolBar.InnerHtml = result.ToString
	End Sub

    Private Sub GenerateTargetLanguageList(ByVal ContentLanguage As Integer)
        Dim dt As EkDS.LanguageDataTable

        Dim enSortOrder As EkDS.SortDirection = LanguageDataSet.SortDirection.Ascending
        Dim enSortBy As EkDS.SortBy = LanguageDataSet.SortBy.LanguageName

        dt = m_refSiteApi.GetLanguages(EkDS.LanguageState.Active)

        Dim field As WebControls.BoundField
        With LanguageGrid

            .Columns.Clear()

            ' Selected?
            field = New WebControls.BoundField
            With field
                .DataField = ""
                '.HeaderText = "Export"
                .HeaderText = "<input type=""checkbox"" name=""chkAll"" onclick=""onCheckAll(this)"" checked=""checked"" />"
                .HtmlEncode = False
                .HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                .HeaderStyle.Width = New Unit(20, UnitType.Pixel)
                .ItemStyle.Wrap = False
                .ItemStyle.HorizontalAlign = HorizontalAlign.Center
            End With
            .Columns.Add(field)
            ' Flag Icon
            field = New WebControls.BoundField
            With field
                .DataField = "FlagFile"
                .HeaderText = ""
                .HeaderStyle.Width = New Unit(20, UnitType.Pixel)
                .ItemStyle.Wrap = False
                .ItemStyle.HorizontalAlign = HorizontalAlign.Center
            End With
            .Columns.Add(field)
            ' Language Name
            field = New WebControls.BoundField
            With field
                .DataField = "LocalName"
                .SortExpression = EkDS.SortBy.LanguageName
                .HeaderText = m_refMsg.GetMessage("generic name")
                .HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                .ItemStyle.Wrap = False
            End With
            .Columns.Add(field)
            ' Language Code
            field = New WebControls.BoundField
            With field
                .DataField = "XmlLang" ' or "BrowserCode"
                .SortExpression = EkDS.SortBy.XmlLang   ' or .BrowserCode
                .HeaderText = m_refMsg.GetMessage("lbl code")
                .HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                .HeaderStyle.Width = New Unit(6, UnitType.Em)
                .ItemStyle.Wrap = False
                .ItemStyle.HorizontalAlign = HorizontalAlign.Left
            End With
            .Columns.Add(field)
            ' Language ID (decimal)
            field = New WebControls.BoundField
            With field
                .DataField = "LanguageID"
                .SortExpression = EkDS.SortBy.LanguageID
                .HeaderText = m_refMsg.GetMessage("rulesetheader id")
                .HeaderStyle.HorizontalAlign = HorizontalAlign.Right
                .HeaderStyle.Width = New Unit(5, UnitType.Em)
                .ItemStyle.Wrap = False
                .ItemStyle.HorizontalAlign = HorizontalAlign.Right
            End With
            .Columns.Add(field)
            ' Language ID (hex)
            field = New WebControls.BoundField
            With field
                .DataField = "LanguageID"
                .HtmlEncode = False ' necessary to make DataFormatString effective
                .DataFormatString = "{0:x4}"
                .HeaderText = m_refMsg.GetMessage("lbl hex")
                .HeaderStyle.HorizontalAlign = HorizontalAlign.Right
                .HeaderStyle.Width = New Unit(4, UnitType.Em)
                .ItemStyle.Wrap = False
                .ItemStyle.HorizontalAlign = HorizontalAlign.Right
            End With
            .Columns.Add(field)

            ' FireFox: border between cells is a result of the <table rules="all" attribute, which I do not know how to eliminate.

            .ShowHeader = True
            .DataSource = dt
            .DataBind()
        End With
    End Sub

    Protected Sub LanguageGrid_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles LanguageGrid.RowDataBound
        Dim gv As GridView = CType(sender, GridView)
        If (e.Row.RowType = DataControlRowType.DataRow) Then
            Dim drv As System.Data.DataRowView = CType(e.Row.DataItem, System.Data.DataRowView)
            Dim row As EkDS.LanguageRow = CType(drv.Row, EkDS.LanguageRow)
            Dim iColumn As Integer = 0

            If (row.LanguageID = ContentLanguage) Then
                e.Row.Visible = False
            End If

            Dim chkSelect As New HtmlInputCheckBox
            With chkSelect
                .ID = "ExportLang"
                .Value = row.LanguageID
                .Attributes.Add("title", "Check to export")
                .Checked = True
            End With
            e.Row.Cells(iColumn).Controls.Add(chkSelect)
            iColumn += 1

            ' Flag Icon
            Dim objImg As New HtmlControls.HtmlImage
            With objImg
                .Src = m_objLocalizationApi.GetFlagUrl(row)
                .Alt = row.LocalName
                .Attributes.Add("title", .Alt)
                .Width = 16
                .Height = 16
            End With
            e.Row.Cells(iColumn).Controls.Add(objImg)
            iColumn += 1
        End If
    End Sub

#End Region

#Region "LOCALIZATION - Export"
    Private Sub ExportForTranslation()
        Dim strTargetLanguages As String = ""   ' comma-delimited list
        Dim sbHtml As New StringBuilder
        'Dim strText As String
        'Dim nFileSize As Integer
        'Dim strFileSize As String
        strTargetLanguages = GetTargetLanguages()

        Select Case m_Type
            Case CmsTranslatableType.Content
                m_objLocalizationApi.StartExportContentForTranslation(m_intId.ToString, strTargetLanguages)
            Case CmsTranslatableType.Folder
                'Dim strContentType As String
                'Dim lContentType As Integer = EkConstants.CMSContentType_Content
                'strContentType = Request.QueryString("ContentType")
                'If (Not IsNothing(strContentType) AndAlso IsNumeric(strContentType.Trim)) Then
                '	lContentType = Convert.ToInt32(strContentType.Trim)
                'End If
                m_objLocalizationApi.StartExportFolderForTranslation(m_intFolderId, EkConstants.CMSContentType_AllTypes, m_bRecursive, strTargetLanguages)
            Case CmsTranslatableType.Menu
                m_objLocalizationApi.StartExportMenusForTranslation(strTargetLanguages)
            Case CmsTranslatableType.Taxonomy
                m_objLocalizationApi.StartExportTaxonomyForTranslation(strTargetLanguages)
            Case Else
                Exit Sub
        End Select

        'If (Not IsNothing(objLocalizationData)) Then
        '	TargetDownloadLabel.Text = ""
        '	For Each objLang As LocalizationExportData.TargetData In objLocalizationData.TargetLanguages.Values
        '		If (objLang.CompressedFileUrl <> String.Empty) Then
        '			nFileSize = objLang.CompressedFileSize
        '			If (nFileSize < 10 * 1024) Then
        '				strFileSize = String.Format("{0:##,##0} bytes", nFileSize)
        '			ElseIf (nFileSize < 10 * 1024 * 1024) Then
        '				strFileSize = String.Format("{0:N} KB", (nFileSize / 1024))
        '			Else
        '				strFileSize = String.Format("{0:N} MB", (nFileSize / (1024 * 1024)))
        '			End If
        '			strText = String.Format("Download zipped file for {0} ({1})", objLang.LanguageData.Name, strFileSize)
        '			' Can't use asp:BulletedList b/c ListItem is only text/value and doesn't support HTML
        '			sbHtml.Append("<li><a href=""" & objLang.CompressedFileUrl & """ target='_blank'>" & strText & "</a></li>" & vbCrLf)
        '		End If
        '	Next
        '	If (sbHtml.Length > 0) Then
        '		sbHtml.Insert(0, "<ul>" & vbCrLf)
        '		sbHtml.Append("</ul>" & vbCrLf)
        '	End If
        '	TargetDownload.Text = sbHtml.ToString
        '	' TODO dwd
        '	''LocalizationReport.Text = GenerateLogReport(objLocalizationData.LogFileUrl)
        'Else
        '	TargetDownloadLabel.Text = "No files are available."
        'End If
    End Sub

    Private Function GetTargetLanguages() As String
        Dim strLanguages As String = ""
        Try
            strLanguages = Request.Form.Item("TargetLanguages")
        Catch ex As Exception
            ' ignore error
        End Try
        Return strLanguages
    End Function
#End Region

    'Private Function GenerateLogReport(ByVal LogFileUrl As String) As String
    '	Dim sbReport As New StringBuilder
    '	sbReport.Append("<iframe width='95%' height='400' src=""" & LogFileUrl & """>")
    '	sbReport.Append("<a href=""" & LogFileUrl & """ target='_blank'>View report log</a>")
    '	sbReport.Append("</iframe>")
    '	Return sbReport.ToString
    'End Function
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub
End Class
