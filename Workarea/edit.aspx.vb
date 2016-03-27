Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.ASM.AssetConfig
Imports System.Text
Imports System.Collections.Generic

Partial Class edit
    Inherits System.Web.UI.Page

#Region "Member Variables - Private"

    Private m_refContent As Ektron.Cms.Content.EkContent
    Private m_refTask As Ektron.Cms.Content.EkTask
    Private m_refSite As Ektron.Cms.Site.EkSite
    Private folder As New Long
    Private m_refSiteApi As SiteAPI
    Private m_refStyle As New StyleHelper
    Private FormAction As String = "" 'Reset the form action
    Private BrowserCode As String = "en"
    Private EnableMultilingual As Integer = 0
    Private Appname As String = ""
    Private IsBrowserIE As Boolean = False
    Private m_strPageAction As String = ""
    Private m_strType As String = ""
    Private m_strCopy As String = ""
    Private pagedata As Collection
    Private page_content_data As Collection
    Private page_meta_data As Collection
    Private security_data As PermissionData

    Private strMyCollection As String = ""
    Private strAddToCollectionType As String = ""
    Private m_bClose As Boolean = True
    Private back_folder_id As Long = 0
    Private back_id As Long = 0
    Private back_file As String = ""
    Private back_action As String = ""
    Private back_form_id As Long = 0
    Private back_LangType As Integer = 0
    Private back_callerpage As String = ""
    Private back_origurl As String = ""
    Private m_intContentType As Integer = 1
    Private bVer4Editor As Boolean = False
    Private m_strManualAlias As String = ""
    Private m_intManualAliasId As Long = 0
    Private m_strManualAliasExt As String = ""
    Private content_edit_data As ContentEditData
    Private content_data As ContentData
    Private folder_data As FolderData
    Private settings_data As SettingsData

    Private AppLocaleString As String = ""
    Private m_strSchemaFile As String = ""
    Private m_strNamespaceFile As String = ""
    Private xmlconfig_data As XmlConfigData
    Private m_strContentTitle As String = ""
    Private m_strContentHtml As String = ""
    Private content_teaser As String = ""
    Private content_comment As String = ""
    Private content_stylesheet As String = ""
    Protected m_intContentFolder As Long = 0

    Private xml_config As String = ""
    Private save_xslt_file As String = ""
    Private editorPackage As String = ""
    Private MetaComplete As Boolean = False
    Private m_refContentId As Long = 0
    Private m_intXmlConfigId As Long = -1
    Private iSegment As Integer = 0
    Private iSegment2 As Integer = 0
    Private bIsFormDesign As Boolean = False
    Private iMaxContLength As Integer = 0
    Private iMaxSummLength As Integer = 0
    Private UserRights As PermissionData
    Private PreviousState As String = ""
    Private meta_data() As ContentMetaData

    Private nTabPanelTop As Integer = 130
    Private ret As Boolean
    Private eWebEditProPromptOnUnload As Integer = 0  ' To Do this should be a 1, but editor needs to be fixed
    Protected var2 As String = ""
    Private szdavfolder As String = ""

    'Variables used in load page for the editor
    Private endDateActionSel As Hashtable
    Private endDateActionSize As Integer = 0
    Private UploadPrivs As Boolean = False
    Private go_live As String = ""
    Private end_date As String = ""
    Private end_date_action As String = ""
    Private MetaDataNumber As Integer = 0
    Private path As String = ""
    Private urlxml As String = ""
    Private language_data As LanguageData
    Private ImagePath As String = ""
    Private AppPath As String = ""
    Private SitePath As String = ""
    Private CurrentUserID As Long = 0
    Private AppeWebPath As String = ""
    Private subscription_data_list As SubscriptionData()
    Private subscribed_data_list As SubscriptionData()
    Private subscription_properties_list As SubscriptionPropertiesData
    Private blnBreakSubInheritance As Boolean = False
    Private intInheritFrom As Long = 0
    Private bGlobalSubInherit As Boolean = False
    Private blnShowTStatusMessage As Boolean = False
    Private active_subscription_list As SubscriptionData()
    Private blnUndoCheckOut_complete As Boolean
    Private m_sSelectedDivStyleClass As String = "selected_editor"
    Private m_sUnSelectedDivStyleClass As String = "unselected_editor"
    Private m_bIsBlog As Boolean = False
    Private blog_data As BlogData
    Private blog_post_data As Ektron.Cms.BlogPostData
    Private bNewPoll As Boolean = False
    Private bReNewPoll As Boolean = False
    Private nPollChoices As Integer = 8
    Private myMeta As String
    Private m_SelectedEditControl As String
	Private m_ctlContentDesigner As Ektron.ContentDesignerWithValidator
	Private m_ctlSummaryDesigner As Ektron.ContentDesignerWithValidator
	Private m_ctlFormResponseRedirect As Ektron.ContentDesignerWithValidator
	Private m_ctlFormResponseTransfer As Ektron.ContentDesignerWithValidator
    Private m_ctlFormSummaryReport As HtmlImage
    Private m_ctlContentPane As HtmlGenericControl
    Private m_ctlSummaryPane As HtmlGenericControl
    Private m_ctlSummaryStandard As HtmlGenericControl
    Private m_ctlSummaryRedirect As HtmlGenericControl
    Private m_ctlSummaryTransfer As HtmlGenericControl
    Private m_ctlSummaryReport As HtmlGenericControl
    Private m_ctlContentValidator As RegularExpressionValidator
    Private m_ctlSummaryValidator As RegularExpressionValidator
    'Set of variables added for 7.6 Aliasing
    Private m_prevManualAliasName As String = ""
    Private m_currManualAliasStatus As String = ""
    Private m_currManualAliasName As String = ""
    Private m_prevManualAliasExt As String = ""
    Private m_currManualAliasExt As String = ""
    Private m_urlAliasSettings As New UrlAliasing.UrlAliasSettingsApi
    Private controlName As String = String.Empty

#End Region

#Region "Member Variables - Protected"

    Protected m_strAssetFileName As String = ""
    Protected TaxonomyTreeIdList As String = ""
    Protected TaxonomyTreeParentIdList As String = ""
    Protected m_intTaxFolderId As Long = 0
    Protected updateFieldId As String = ""
    Protected commparams As String = ""
    Protected TaxonomyRoleExists As Boolean = False
    Protected IsAdmin As Boolean = False
    Protected TaxonomyOverrideId As Long = 0
    Protected TaxonomySelectId As Long = 0
    Protected DisplayTab As Boolean = True
    Protected m_refContApi As ContentAPI
    Protected m_refMsg As Common.EkMessageHelper
    Protected DIRECTION As String = ""
    Protected AppImgPath As String = ""
    Protected IsMac As Boolean = False
    Protected m_intContentLanguage As Integer = 0
    Protected m_intItemId As Long = 0
    Protected m_intFolderId As Long = 0
    Protected lContentType As Integer
    Protected lContentSubType As CMSContentSubtype = CMSContentSubtype.Content
    Protected g_ContentTypeSelected As Integer = CMSContentType_AllTypes
    Protected asset_info As Hashtable = New Hashtable

#End Region

#Region "Events"

    Public Sub New()

        m_refContApi = New ContentAPI
        m_ctlContentPane = New HtmlGenericControl
        m_ctlSummaryPane = New HtmlGenericControl
        m_ctlSummaryStandard = New HtmlGenericControl
        m_ctlSummaryRedirect = New HtmlGenericControl
        m_ctlSummaryTransfer = New HtmlGenericControl
        m_ctlSummaryReport = New HtmlGenericControl
        m_ctlFormSummaryReport = New HtmlImage

    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        AppPath = m_refContApi.AppPath

        m_SelectedEditControl = Utilities.GetEditorPreference(Request)

        'Register Page Components
        Me.RegisterCSS()
        Me.RegisterJS()

        phEditContent.Controls.Add(m_ctlContentPane)
        With m_ctlContentPane
            .TagName = "div"
            .ID = "dvContent"
        End With

        phEditSummary.Controls.Add(m_ctlSummaryPane)
        With m_ctlSummaryPane
            .TagName = "div"
            .ID = "dvSummary"
        End With

        m_ctlSummaryPane.Controls.Add(m_ctlSummaryStandard)
        With m_ctlSummaryStandard
            .TagName = "div"
            .ID = "_dvSummaryStandard"
        End With

        m_ctlSummaryPane.Controls.Add(m_ctlSummaryRedirect)
        With m_ctlSummaryRedirect
            .TagName = "div"
            .ID = "_dvSummaryRedirect"
        End With

        m_ctlSummaryPane.Controls.Add(m_ctlSummaryTransfer)
        With m_ctlSummaryTransfer
            .TagName = "div"
            .ID = "_dvSummaryTransfer"
        End With

        m_ctlSummaryPane.Controls.Add(m_ctlSummaryReport)
        With m_ctlSummaryReport
            .TagName = "div"
            .ID = "_dvSummaryReport"
        End With

		' The ContentDesigner controls need to be created in the Page_Init event so the PostData
		' will be bound to them. However, they may not be displayed, so default .Visible=False.
		m_ctlContentDesigner = LoadControl("controls/Editor/ContentDesignerWithValidator.ascx")
		m_ctlContentPane.Controls.Add(m_ctlContentDesigner)
        With m_ctlContentDesigner
            .Visible = False
            .ID = "content_html"
		End With

		m_ctlSummaryDesigner = LoadControl("controls/Editor/ContentDesignerWithValidator.ascx")
		m_ctlSummaryStandard.Controls.Add(m_ctlSummaryDesigner)
        With m_ctlSummaryDesigner
            .Visible = False
            .ID = "content_teaser"
		End With


		m_ctlFormResponseRedirect = LoadControl("controls/Editor/ContentDesignerWithValidator.ascx")
		m_ctlSummaryRedirect.Controls.Add(m_ctlFormResponseRedirect)
        With m_ctlFormResponseRedirect
            .Visible = False
            .ID = "forms_redirect"
		End With


		m_ctlFormResponseTransfer = LoadControl("controls/Editor/ContentDesignerWithValidator.ascx")
		m_ctlSummaryTransfer.Controls.Add(m_ctlFormResponseTransfer)
        With m_ctlFormResponseTransfer
            .Visible = False
            .ID = "forms_transfer"
		End With

		m_ctlSummaryPane.Controls.Add(m_ctlFormSummaryReport)
        With m_ctlFormSummaryReport
            .Visible = False
            .Src = "images/application/charttypes.gif"
            .ID = "_imgFormSummaryReport"
        End With

        m_ctlContentValidator = ContentValidator ' New RegularExpressionValidator
        ' m_ctlContentPane.Controls.Add(m_ctlContentValidator)
        With m_ctlContentValidator
            .Visible = False
            .ControlToValidate = m_ctlContentDesigner.ID
            .EnableClientScript = True
        End With

        m_ctlSummaryValidator = SummaryValidator ' New RegularExpressionValidator
        ' m_ctlSummaryPane.Controls.Add(m_ctlSummaryValidator)
        With m_ctlSummaryValidator
            .Visible = False
            .ControlToValidate = m_ctlSummaryDesigner.ID
            .EnableClientScript = True
        End With
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim bAddingNew As Boolean = False
        Dim tempStr, referrerStr As String
        Try
            'INITIALIZE THE VARIABLES
            If (Request.Browser.Type.IndexOf("IE") <> -1) Then
                IsBrowserIE = True
            End If
            jsIsMac.Text = "false"
            If (Request.Browser.Platform.IndexOf("Win") = -1) Then
                IsMac = True
            End If
            ' Ensure that this is not a browser refresh (Mac-Safari bug causes
            ' the editor to load after publishing, if the browser is refreshing):
            If (IsMac And Not IsBrowserIE) Then
                referrerStr = Request.Url.LocalPath
                If (Not referrerStr Is Nothing) Then
                    tempStr = referrerStr.Substring(referrerStr.LastIndexOf("/"))
                    If (tempStr = "/workarea.aspx") Then
                        tempStr = referrerStr.Replace(tempStr, "/dashboard.aspx")
                        Response.Redirect(tempStr, False)
                        Return
                    End If
                End If
            End If

            If (m_SelectedEditControl <> "ContentDesigner") Then
                m_ctlContentPane.Controls.Remove(m_ctlContentDesigner)
                m_ctlSummaryStandard.Controls.Remove(m_ctlSummaryDesigner)
                m_ctlSummaryRedirect.Controls.Remove(m_ctlFormResponseRedirect)
                m_ctlSummaryTransfer.Controls.Remove(m_ctlFormResponseTransfer)
            End If

            Response.Expires = -1
            Response.AddHeader("Pragma", "no-cache")
            Response.AddHeader("cache-control", "no-store")

            m_refMsg = m_refContApi.EkMsgRef

            'THE NEXT THREE LINES MUST BE REMOVED BEFORE THE RELEASE
            If (Request.ServerVariables("Query_String") = "") Then
                Exit Sub
            End If

            If (IsMac) Then
                jsIsMac.Text = "true"
            End If

            ' Note: To fix a problem with the Ephox Editors on the
            ' Mac-running-Safari (assumed if "IsMac and not IsBrowserIE")
            ' we need to use different styles for the DIV-tags holding
            ' the editors, etc., otherwise they frequently draw themselves
            ' when they should remain hidden. These values cause problems
            ' with the PC/Win/IE combination, (the summary editor fails to
            ' provide a client area for the user to view/edit) so they cannot
            ' cannot be used everywhere, hence our use of alternate style classes:
            ' Pass class names to javascript:
            jsSelectedDivStyleClass.Text = m_sSelectedDivStyleClass
            jsUnSelectedDivStyleClass.Text = m_sUnSelectedDivStyleClass

            m_refContApi = New ContentAPI
            m_refSiteApi = New SiteAPI
            m_refContent = m_refContApi.EkContentRef
            m_refSite = m_refContApi.EkSiteRef
            m_refTask = m_refContApi.EkTaskRef

            CurrentUserID = m_refContApi.UserId
            AppImgPath = m_refContApi.AppImgPath
            SitePath = m_refContApi.SitePath
            Appname = m_refContApi.AppName
            AppeWebPath = m_refContApi.ApplicationPath & m_refContApi.AppeWebPath
            AppPath = m_refContApi.AppPath
            EnableMultilingual = m_refContApi.EnableMultilingual
            StyleSheetJS.Text = m_refStyle.GetClientScript()
            EnhancedMetadataScript.Text = CustomFields.GetEnhancedMetadataScript()
            EnhancedMetadataArea.Text = CustomFields.GetEnhancedMetadataArea()
            lbl_GenericTitleLabel.Text = m_refMsg.GetMessage("generic title label")

            If (Not (Request.QueryString("id") Is Nothing)) Then
                m_intItemId = Convert.ToInt64(Request.QueryString("id"))
                m_intTaxFolderId = m_intItemId
            End If
            If (Not (Request.QueryString("LangType") Is Nothing)) Then
                If (Request.QueryString("LangType") <> "") Then
                    m_intContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                    m_refContApi.SetCookieValue("LastValidLanguageID", m_intContentLanguage)
                Else
                    If m_refContApi.GetCookieValue("LastValidLanguageID") <> "" Then
                        m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"))
                    End If
                End If
            Else
                If m_refContApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
            If (m_intContentLanguage = CONTENT_LANGUAGES_UNDEFINED Or m_intContentLanguage = ALL_CONTENT_LANGUAGES) Then
                m_intContentLanguage = m_refContApi.DefaultContentLanguage
            End If
            If m_intContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
                m_refContApi.ContentLanguage = ALL_CONTENT_LANGUAGES
            Else
                m_refContApi.ContentLanguage = m_intContentLanguage
            End If
            If (Not (Request.QueryString("folder_id") Is Nothing)) Then
                m_intFolderId = Convert.ToInt64(Request.QueryString("folder_id"))
            End If

            If (Not (Request.QueryString("form_type") Is Nothing)) Then
                bNewPoll = ("poll" = Convert.ToString(Request.QueryString("form_type")).Trim.ToLower)
            End If
            If (Not (Request.QueryString("new") Is Nothing)) Then
                bAddingNew = ("true" = Convert.ToString(Request.QueryString("new")).Trim.ToLower)
            End If
            If (Not (Request.QueryString("poll") Is Nothing)) Then
                bReNewPoll = ("renew" = Convert.ToString(Request.QueryString("poll")).Trim.ToLower)
            End If
            If (Not (Request.Form("editaction") Is Nothing)) Then
                m_strPageAction = Convert.ToString(Request.Form("editaction")).ToLower.Trim
            End If
            If (Not (Request.QueryString("type") Is Nothing)) Then
                m_strType = Convert.ToString(Request.QueryString("type")).ToLower.Trim
            ElseIf (Not (Request.Form("type") Is Nothing)) Then
                m_strType = Convert.ToString(Request.Form("type")).ToLower.Trim
            End If
            If (Request.QueryString("ctlupdateid") <> "") Then
                commparams = "&ctlupdateid=" & Request.QueryString("ctlupdateid") & "&ctlmarkup=" & Request.QueryString("ctlmarkup") & "&cltid=" & Request.QueryString("cltid") & "&ctltype=" & Request.QueryString("ctltype")
                updateFieldId = Request.QueryString("ctlupdateid")
                Page.ClientScript.RegisterHiddenField("ctlupdateid", updateFieldId)
            End If
            If (Request.QueryString("cacheidentifier") IsNot Nothing AndAlso Request.QueryString("cacheidentifier") <> "") Then
                Page.ClientScript.RegisterHiddenField("cacheidentifier", Request.QueryString("cacheidentifier"))
            Else
                If (Request.QueryString("mycollection") IsNot Nothing AndAlso Request.QueryString("addto") IsNot Nothing AndAlso Request.QueryString("type") IsNot Nothing) Then
                    If (Request.QueryString("type") = "add" AndAlso Request.QueryString("addto") = "menu") Then
                        Page.ClientScript.RegisterHiddenField("cacheidentifier", "menu_" & Request.QueryString("mycollection") & m_intContentLanguage & "_mnu")
                    End If
                End If
            End If

            destination.Value = Page.Request.Url.Scheme & Uri.SchemeDelimiter & Page.Request.Url.Authority & "/" & AppPath & "processMultiupload.aspx"
            'Confirmation(-URL.Value = Page.Request.Url.Scheme & Uri.SchemeDelimiter & Page.Request.Url.Authority & "/" & AppPath & "content.aspx")
            PostURL.Value = Page.Request.Url.Scheme & Uri.SchemeDelimiter & Page.Request.Url.Authority & "/" & AppPath & "processMultiupload.aspx"
            NextUsing.Value = Page.Request.Url.Scheme & Uri.SchemeDelimiter & Page.Request.Url.Authority & "/" & AppPath & "content.aspx"

            If (Request.QueryString("ctlmarkup") <> "") Then
                Page.ClientScript.RegisterHiddenField("ctlmarkup", Request.QueryString("ctlmarkup"))
            End If
            If (Request.QueryString("ctltype") <> "") Then
                Page.ClientScript.RegisterHiddenField("ctltype", Request.QueryString("ctltype"))
            End If
            If (Request.QueryString("cltid") <> "") Then
                Page.ClientScript.RegisterHiddenField("cltid", Request.QueryString("cltid"))
            End If

            If (m_strType = "update") Then
                m_refContentId = m_intItemId
            Else
                If (Request.QueryString("content_id") <> "") Then
                    m_refContentId = Request.QueryString("content_id")
                End If
            End If
            If (Not (Request.QueryString("xid") Is Nothing)) Then
                m_intXmlConfigId = Convert.ToInt64(Request.QueryString("xid"))
            ElseIf (Not (Request.Form("SelectedXid") Is Nothing)) Then
                m_intXmlConfigId = Convert.ToInt64(Request.Form("SelectedXid"))
            Else
                If (Request.QueryString("type") = "add") Then
                    If (Request.QueryString("AllowHTML") <> "1") Then
                        m_intXmlConfigId = Utilities.GetDefaultXmlConfig(Request.QueryString("id"))
                        If (m_intXmlConfigId = 0) Then
                            m_intXmlConfigId = -1
                        End If
                    End If
                End If
            End If
            If (Request.QueryString("mycollection") <> "") Then
                strMyCollection = Request.QueryString("mycollection")
            ElseIf (Request.Form("mycollection") <> "") Then
                strMyCollection = Request.Form("mycollection")
            End If
            If (Request.QueryString("addto") <> "") Then
                strAddToCollectionType = Request.QueryString("addto")
            ElseIf (Request.Form("addto") <> "") Then
                strAddToCollectionType = Request.Form("addto")
            End If
            If (Request.QueryString("close") = "false") Then
                m_bClose = False
            End If
            If (Not (IsNothing(Request.QueryString("back_folder_id")))) Then
                back_folder_id = Request.QueryString("back_folder_id")
                m_intTaxFolderId = back_folder_id
            End If
            If (Not (IsNothing(Request.QueryString("back_id")))) Then
                back_id = Request.QueryString("back_id")
            End If
            If (Not (IsNothing(Request.QueryString("back_file")))) Then
                back_file = Request.QueryString("back_file")
            End If
            If (Not (IsNothing(Request.QueryString("back_action")))) Then
                back_action = Request.QueryString("back_action")
                If back_action.ToLower() = "viewcontentbycategory" Or back_action.ToLower() = "viewarchivecontentbycategory" Then
                    back_folder_id = back_id
                End If
            End If
            If (Not (IsNothing(Request.QueryString("control")))) Then
                controlName = Request.QueryString("control")
            End If
            If (Not (IsNothing(Request.QueryString("buttonid")))) Then
                buttonId.Value = Request.QueryString("buttonid")
            End If
            If (Not (IsNothing(Request.QueryString("back_form_id")))) Then
                back_form_id = Request.QueryString("back_form_id")
            End If
            If (Not (IsNothing(Request.QueryString("back_LangType")))) Then
                back_LangType = Request.QueryString("back_LangType")
            Else
                back_LangType = Ektron.Cms.CommonApi.GetEcmCookie()("DefaultLanguage")
            End If
            If (Not (IsNothing(Request.QueryString("back_callerpage")))) Then
                back_callerpage = "&back_callerpage=" & Request.QueryString("back_callerpage")
            End If
            If (Not (IsNothing(Request.QueryString("back_page")))) Then
                back_callerpage = back_callerpage & "&back_page=" & Request.QueryString("back_page")
            End If
            If (Not (IsNothing(Request.QueryString("back_origurl")))) Then
                back_origurl = "&back_origurl=" & HttpUtility.UrlEncode(Request.QueryString("back_origurl"))
            End If
            If Request.QueryString(ContentTypeUrlParam) <> "" Then
                If IsNumeric(Request.QueryString(ContentTypeUrlParam)) Then
                    g_ContentTypeSelected = CLng(Request.QueryString(ContentTypeUrlParam))
                    m_refContApi.SetCookieValue(ContentTypeUrlParam, g_ContentTypeSelected)
                End If
            ElseIf Ektron.Cms.CommonApi.GetEcmCookie()(ContentTypeUrlParam) <> "" Then
                If IsNumeric(Ektron.Cms.CommonApi.GetEcmCookie()(ContentTypeUrlParam)) Then
                    g_ContentTypeSelected = CLng(Ektron.Cms.CommonApi.GetEcmCookie()(ContentTypeUrlParam))
                End If
            End If
            If CMSContentType_AllTypes = g_ContentTypeSelected Then
                If (Not (IsNothing(Request.QueryString("multi")))) Then
                    If "" = Request.QueryString("multi") Then
                        lContentType = CMSContentType_Content 'set content type to "content" as default value
                    Else
                        lContentType = Request.QueryString("multi")
                        If lContentType = 9876 Then
                            lContentType = 103
                        End If
                    End If
                Else
                    lContentType = CMSContentType_Content
                End If
            Else
                lContentType = g_ContentTypeSelected
                If lContentType = 9876 Then
                    lContentType = 103
                End If
            End If
            language_data = m_refSiteApi.GetLanguageById(m_intContentLanguage)
            'If (Request.QueryString("TaxonomyId") IsNot Nothing AndAlso Request.QueryString("TaxonomyId") <> "") Then
            '    TaxonomyOverrideId = Convert.ToInt32(Request.QueryString("TaxonomyId"))
            'End If
            If (Me.m_strType.ToLower() = "add" AndAlso Request.QueryString("SelTaxonomyId") IsNot Nothing AndAlso Request.QueryString("SelTaxonomyId") <> "") Then
                TaxonomySelectId = Convert.ToInt64(Request.QueryString("SelTaxonomyId"))
            End If
            Dim settings_data As SettingsData
            settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)

            Dim UserLocale As Integer
            UserLocale = m_refSiteApi.RequestInformationRef.UserCulture
            AppLocaleString = GetLocaleFileString(CStr(UserLocale))

            'ToggleViewBtn.Src = m_refContApi.AppImgPath & "moveup.gif"
            jsMaxLengthMsg.Text = m_refMsg.GetMessage("js err encoded title exceeds max length")
            jsContentLanguage.Text = Convert.ToString(m_intContentLanguage)
            jsId.Text = Convert.ToString(m_intItemId)
            jsDefaultContentLanguage.Text = Convert.ToString(m_refContApi.DefaultContentLanguage)
            jsType.Text = Convert.ToString(m_intContentType)
            phAlias.Visible = False
            Page.Title = m_refContApi.AppName & " " & m_refMsg.GetMessage("edit content page title") & " """ & Ektron.Cms.CommonApi.GetEcmCookie()("username") & """"
            'If (m_strPageAction = "cancel") Then
            Dim editaction As String = ""
            If (Not (IsNothing(Request.Form("editaction")))) Then
                editaction = Request.Form("editaction")
            End If
            If ("workoffline" = editaction Or "cancel" = editaction Or ("" = Convert.ToString(m_intItemId) And "" = editaction)) Then
                If (m_strType = "update") Then
                    ret = m_refContent.UndoCheckOutv2_0(Request.Form("content_id"))
                    blnUndoCheckOut_complete = True
                End If
                If (Not m_bClose) Then
                    ClosePanel.Text = "<script language=javascript>" & vbCrLf & "ResizeFrame(1); // Show the navigation-tree frame." & vbCrLf & "</script>"
                    Response.Redirect(GetBackPage(Request.Form("content_id")), False)
                Else
                    Response.Redirect("close.aspx", False)
                End If
            ElseIf ((m_strPageAction = "save") Or (m_strPageAction = "checkin") Or (m_strPageAction = "publish") Or (m_strPageAction = "summary_save") Or (m_strPageAction = "meta_save")) Then
                Process_FormSubmit()
                If (m_bClose And m_strPageAction <> "save") Then
                    If (updateFieldId <> "") Then
                        Dim strQuery As String = ""
                        If (TaxonomySelectId > 0) Then
                            strQuery = "&__taxonomyid=" & TaxonomySelectId
                        ElseIf (TaxonomyOverrideId > 0) Then
                            strQuery = "&__taxonomyid=" & TaxonomyOverrideId
                        End If
                        Response.Redirect("close.aspx?toggle=true" & strQuery, False)
                    Else
                        'Response.Redirect("close.aspx", False)
                        'Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "aaaa", "alert('hi');", True)
                    End If
                End If
            Else
                Display_EditControls()

                If (Not (Page.IsPostBack) And bAddingNew) Then
                    If (Request.QueryString("copy_lang") = "") Then
                        Dim ucNewFormWizard As newformwizard
                        ucNewFormWizard = CType(LoadControl("controls/forms/newformwizard.ascx"), newformwizard)
                        ucNewFormWizard.ID = "ProgressSteps"
                        phNewFormWizard.Controls.Add(ucNewFormWizard)
                        If bNewPoll Then
                            PollHtmlScript()
                        End If
                    End If
                End If
            End If

			Dim cPerms As PermissionData
            cPerms = m_refContApi.LoadPermissions(m_intContentFolder, "folder")
			With m_ctlContentDesigner
				.FolderId = m_intContentFolder
				If 2 = m_intContentType Then
					.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Designer
				ElseIf Len(editorPackage) > 0 Then
					.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.DataEntry
				Else
					.ToolsFile = m_refContApi.ApplicationPath & "ContentDesigner/configurations/StandardEdit.aspx?wiki=1"
				End If
				.SetPermissions(cPerms)
				.AllowFonts = True
			End With
			With m_ctlSummaryDesigner
				.FolderId = m_intContentFolder
				If 2 = m_intContentType Then
					.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.XsltDesigner
				ElseIf m_bIsBlog Then
					.ToolsFile = m_refContApi.ApplicationPath & "ContentDesigner/configurations/InterfaceBlog.aspx?WMV=1"
				Else
					.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Standard
				End If
				.SetPermissions(cPerms)
				.AllowFonts = True
			End With
			With m_ctlFormResponseRedirect
				.FolderId = m_intContentFolder
				.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.NoToolbars
				.SetPermissions(cPerms)
				.AllowFonts = True
			End With
			With m_ctlFormResponseTransfer
				.FolderId = m_intContentFolder
				.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.NoToolbars
				.SetPermissions(cPerms)
				.AllowFonts = True
			End With
			With m_ctlContentValidator
				.Text = m_refMsg.GetMessage("content size exceeded")
			End With
			With m_ctlSummaryValidator
				.Text = m_refMsg.GetMessage("content size exceeded")
			End With
			' fix for 20157 by tetzel on May 2, 2006. Setting to content type 'all' after adding/editing a file.
			g_ContentTypeSelected = CMSContentType_AllTypes
			m_refContApi.SetCookieValue(ContentTypeUrlParam, g_ContentTypeSelected)
		Catch ex As Exception
			Utilities.ShowError(ex.Message)
		End Try
    End Sub

#End Region

#Region "Helpers"

    Private Function GetLocaleFileString(ByVal localeFileNumber As String) As String
        Dim LocaleFileString As String
        If (CStr(localeFileNumber) = "" Or CInt(localeFileNumber) = 1) Then
            LocaleFileString = "0000"
        Else
            LocaleFileString = New String("0", 4 - Len(Hex(localeFileNumber)))
            LocaleFileString = LocaleFileString & Hex(localeFileNumber)
            If Not System.IO.File.Exists(Server.MapPath(AppeWebPath & "locale" & LocaleFileString & "b.xml")) Then
                LocaleFileString = "0000"
            End If
        End If
        Return LocaleFileString.ToString
    End Function

    Private Function GetServerPath() As String
        Dim strPath As String
        If Request.ServerVariables("SERVER_PORT_SECURE") = "1" Then
            strPath = "https://" & Request.ServerVariables("SERVER_NAME")
            If Request.ServerVariables("SERVER_PORT") <> "443" Then
                strPath = strPath & ":" & Request.ServerVariables("SERVER_PORT")
            End If
        Else
            strPath = "http://" & Request.ServerVariables("SERVER_NAME")
            If Request.ServerVariables("SERVER_PORT") <> "80" Then
                strPath = strPath & ":" & Request.ServerVariables("SERVER_PORT")
            End If
        End If
        Return strPath
    End Function

#End Region

#Region "DISPLAY EDITOR PAGE"
    Private Sub Display_EditControls()
        Dim intContentLanguage As Integer = 1033
        Dim security_lib_data As PermissionData
        Dim i As Integer = 0
        Dim bEphoxSupport As Boolean = False
        Dim aliasContentType As String = String.Empty

        folder_data = Nothing
        Try
            netscape.Value = "" 'New String(Chr(5000), 32)
            language_data = m_refSiteApi.GetLanguageById(m_intContentLanguage)
            ImagePath = language_data.ImagePath
            BrowserCode = language_data.BrowserCode
            For i = 0 To m_AssetInfoKeys.Length - 1
                asset_info.Add(m_AssetInfoKeys(i), "")
            Next
            Page.ClientScript.RegisterHiddenField("TaxonomyOverrideId", TaxonomyOverrideId)
            If (IsMac AndAlso m_SelectedEditControl <> "ContentDesigner" AndAlso m_strType = "update") Then
                'We do not support XML content and Form. Check if the content is XML or form and if it is then don't proceed further.
                Dim cData As ContentData
                cData = m_refContApi.GetContentById(m_intItemId)
                If ((cData.Type = 2) Or ((Not cData.XmlConfiguration Is Nothing) AndAlso (cData.XmlConfiguration.PackageXslt.Length > 0))) Then
                    bEphoxSupport = False
                Else
                    bEphoxSupport = True
                End If
                If (Not bEphoxSupport) Then
                    'Show not supported message
                    Throw New Exception("Forms and XML Editing is not supported on MAC.")
                End If
            End If
            If ((Request.QueryString("pullapproval") = "true") And (m_strType = "update")) Then
                ret = m_refContent.TakeOwnership(m_intItemId)
            End If
            var2 = m_refContent.GetEditorVariablev2_0(m_intItemId, m_strType) 'TODO:Verify info param via var1 removed
            security_data = m_refContApi.LoadPermissions(m_intItemId, "content")
            endDateActionSel = GetEndDateActionStrings()
            endDateActionSize = endDateActionSel("SelectionSize")
            If (security_data IsNot Nothing) Then
                IsAdmin = security_data.IsAdmin
            End If
            active_subscription_list = m_refContApi.GetAllActiveSubscriptions()
            settings_data = m_refSiteApi.GetSiteVariables(CurrentUserID)

            If (m_strType = "update") Then
                content_edit_data = m_refContApi.GetContentForEditing(m_intItemId)
                UserRights = m_refContApi.LoadPermissions(m_intItemId, "content", ContentAPI.PermissionResultType.Content) 'm_refContent.CanIv2_0(m_intItemId, "content")
                lContentType = content_edit_data.Type
                lContentSubType = content_edit_data.SubType
                If (content_edit_data.Type = 2) Then
                    bIsFormDesign = True
                    m_intContentType = 2
                End If
                If (Not (IsNothing(content_edit_data))) Then

                    security_lib_data = m_refContApi.LoadPermissions(content_edit_data.FolderId, "folder")

                    UploadPrivs = security_lib_data.CanAddToFileLib Or security_lib_data.CanAddToImageLib
                    m_strContentTitle = Server.HtmlDecode(content_edit_data.Title)
                    m_strAssetFileName = content_edit_data.AssetData.FileName
                    m_strContentHtml = content_edit_data.Html
                    content_teaser = content_edit_data.Teaser
                    meta_data = content_edit_data.MetaData
                    content_comment = Server.HtmlDecode(content_edit_data.Comment)
                    content_stylesheet = content_edit_data.StyleSheet
                    m_intContentFolder = content_edit_data.FolderId
                    m_intTaxFolderId = content_edit_data.FolderId
                    intContentLanguage = content_edit_data.LanguageId
                    go_live = content_edit_data.GoLive
                    end_date = content_edit_data.EndDate
                    end_date_action = content_edit_data.EndDateAction
                    intInheritFrom = m_refContent.GetFolderInheritedFrom(m_intContentFolder)
                    'agofpa
                    If intInheritFrom <> m_intContentFolder Then
                        bGlobalSubInherit = True
                    End If
                    'agofpa
                    subscription_data_list = m_refContApi.GetSubscriptionsForFolder(intInheritFrom) 'AGofPA get subs for folder; set break inheritance flag false
                    subscription_properties_list = m_refContApi.GetSubscriptionPropertiesForContent(m_refContentId) 'first try content
                    If subscription_properties_list Is Nothing Then
                        subscription_properties_list = m_refContApi.GetSubscriptionPropertiesForFolder(intInheritFrom) 'then get folder
                        subscribed_data_list = subscription_data_list 'm_refContApi.GetSubscriptionsForFolder(m_intContentFolder) ' get subs for folder
                    Else 'content is populated.
                        subscribed_data_list = m_refContApi.GetSubscriptionsForContent(m_refContentId) ' get subs for folder
                    End If
                    'agofpa

                    If (Not (IsNothing(meta_data))) Then
                        MetaDataNumber = meta_data.Length
                    End If
                    PreviousState = content_edit_data.CurrentStatus
                    iMaxContLength = content_edit_data.MaxContentSize
                    iMaxSummLength = content_edit_data.MaxSummarySize

                    path = content_edit_data.Path


                    'If (content_edit_data.ManualAlias.IndexOf(".") <> -1) Then
                    '    'm_strManualAlias = Left(content_edit_data.ManualAlias, InStr(content_edit_data.ManualAlias, ".") - 1)
                    '    'm_strManualAliasExt = Right(content_edit_data.ManualAlias, (Len(content_edit_data.ManualAlias)) - (InStr(content_edit_data.ManualAlias, ".") - 1))
                    '    'm_strManualAlias = System.IO.Path.GetFileNameWithoutExtension(content_edit_data.ManualAlias)
                    '    'm_strManualAliasExt = System.IO.Path.GetExtension(content_edit_data.ManualAlias)
                    '    'Bug fix 33241
                    '    m_strManualAliasExt = System.IO.Path.GetExtension(content_edit_data.ManualAlias)
                    '    m_strManualAlias = content_edit_data.ManualAlias.Replace(m_strManualAliasExt, String.Empty)
                    'Else
                    '    m_strManualAlias = content_edit_data.ManualAlias
                    '    m_strManualAliasExt = ""
                    'End If

                    m_intManualAliasId = content_edit_data.ManualAliasId

                    folder_data = m_refContApi.GetFolderById(m_intContentFolder)

                    If (Right(path, 1) = "\") Then
                        path = Right(path, Len(path) - 1)
                    End If

                    'Check to see if this belongs to XML configuration
                    If lContentType <> 2 Then
                        xmlconfig_data = content_edit_data.XmlConfiguration
                        If (Not (IsNothing(xmlconfig_data))) Then
                            editorPackage = xmlconfig_data.PackageXslt
                            If (editorPackage.Length > 0) Then
                                bVer4Editor = True ' this means that we will be using the new Package Design for the content
                            End If
                        End If
                    End If

                    If (m_strContentTitle <> "") Then
                        MetaComplete = UserRights.CanMetadataComplete 'Changed from 1 to true
                    End If
                    'For i = 0 To m_AssetInfoKeys.Length
                    'TODO:resolve this by using contentdata class
                    'asset_info(m_AssetInfoKeys(i)) = cCont(m_AssetInfoKeys(i))
                    asset_info("AssetID") = content_edit_data.AssetData.Id '(m_AssetInfoKeys(i))
                    asset_info("AssetVersion") = content_edit_data.AssetData.Version
                    'asset_info("AssetFilename") = content_edit_data.AssetData.FileName
                    asset_info("MimeType") = content_edit_data.AssetData.MimeType
                    asset_info("FileExtension") = content_edit_data.AssetData.FileExtension
                    'asset_info("MimeName") = content_edit_data.AssetData.MimeName
                    'asset_info("ImageUrl") = content_edit_data.AssetData.ImageUrl
                    'If (asset_info("MimeType") = "application/x-shockwave-flash") Then
                    '	asset_info("MediaAsset") = True
                    'Else
                    '	asset_info("MediaAsset") = False
                    'End If
                    'Next
                End If
                validTypes.Value = asset_info("FileExtension")
            Else

                UserRights = m_refContApi.LoadPermissions(m_intItemId, "folder", ContentAPI.PermissionResultType.Folder) ' m_refContent.CanIv2_0(m_intItemId, "folder")
                folder_data = m_refContApi.GetFolderById(m_intItemId)
                MetaComplete = UserRights.CanMetadataComplete
                If (m_intXmlConfigId > -1) Then
                    xmlconfig_data = m_refContApi.GetXmlConfiguration(m_intXmlConfigId)
                    MultiTemplateID.Text = "<input type=""hidden"" name=""xid"" value=""" & CStr(m_intXmlConfigId) & """>"
                Else
                    If ((Not IsNothing(folder_data.XmlConfiguration)) AndAlso (folder_data.XmlConfiguration.Length > 0) AndAlso (Request.QueryString("AllowHTML") <> "1")) Then
                        xmlconfig_data = folder_data.XmlConfiguration(0)
                    Else
                        xmlconfig_data = Nothing
                    End If
                End If
                If (Not (IsNothing(xmlconfig_data))) Then
                    editorPackage = xmlconfig_data.PackageXslt
                    If Len(editorPackage) Then
                        bVer4Editor = True
                    End If
                End If
                content_stylesheet = m_refContApi.GetStyleSheetByFolderID(m_intItemId) 'GetStyleSheetByFolderIDv2_0(m_intItemId)
                security_lib_data = m_refContApi.LoadPermissions(m_intItemId, "folder")
                UploadPrivs = security_lib_data.CanAddToFileLib Or security_lib_data.CanAddToImageLib
                Dim TmpId As Object
                TmpId = Request.QueryString("content_id")
                If (TmpId <> "") Then
                    'translating asset
                    If (Request.QueryString("type") = "add") Then
                        If Request.QueryString("back_LangType") <> "" Then
                            m_refContApi.ContentLanguage = Request.QueryString("back_LangType")
                        Else
                            m_refContApi.ContentLanguage = Ektron.Cms.CommonApi.GetEcmCookie()("DefaultLanguage")
                        End If
                    End If
                    content_data = m_refContApi.GetContentById(TmpId)
                    If (Not (IsNothing(content_data))) Then
                        If content_data.SubType = CMSContentSubtype.PageBuilderData OrElse content_data.SubType = CMSContentSubtype.PageBuilderMasterData OrElse content_data.SubType = CMSContentSubtype.WebEvent Then
                            isOfficeDoc.Value = "true"
                        End If
                        If (m_intXmlConfigId = -1) Then
                            If (Not content_data.XmlConfiguration Is Nothing) Then
                                m_intXmlConfigId = content_data.XmlConfiguration.Id
                                xmlconfig_data = content_data.XmlConfiguration
                                editorPackage = xmlconfig_data.PackageXslt
                                If Len(editorPackage) Then
                                    bVer4Editor = True
                                End If
                                MultiTemplateID.Text = "<input type=""hidden"" name=""xid"" value=""" & CStr(m_intXmlConfigId) & """>"
                            End If
                        End If

                        m_strContentTitle = Server.HtmlDecode(content_data.Title)
                        m_strAssetFileName = content_data.AssetData.FileName
                        m_strContentHtml = content_data.Html
                        content_teaser = content_data.Teaser
                        content_comment = Server.HtmlDecode(content_data.Comment)
                        go_live = content_data.GoLive
                        end_date = content_data.EndDate
                        end_date_action = content_data.EndDateAction
                        lContentType = content_data.Type
                        lContentSubType = content_data.SubType
                        If (m_strType = "add") Then
                            If (Utilities.IsAssetType(lContentType)) Then
                                ' For i = 0 To m_AssetInfoKeys.Length - 1
                                'TODO: resolve into content_data class
                                '{"AssetID", "AssetVersion", "AssetFilename", "MimeType", "FileExtension", "MimeName", "ImageUrl"}
                                'm_intItemId = content_data.Id
                                'm_strCopy = "copy"
                                'Dim strAssetID As String = content_data.AssetData.Id
                                'Dim strVersion As String = content_data.AssetData.Version
                                'm_refContApi.MakeCopyDMSAsset(strAssetID, strVersion)
                                m_strContentTitle = Server.HtmlDecode(content_data.Title)
                                validTypes.Value = content_data.AssetData.FileExtension
                                'asset_info("AssetID") = strAssetID '(m_AssetInfoKeys(i))
                                'asset_info("AssetVersion") = strVersion
                                'asset_info("AssetFilename") = content_data.AssetData.FileName
                                'asset_info("MimeType") = content_data.AssetData.MimeType
                                'asset_info("FileExtension") = content_data.AssetData.FileExtension
                                'asset_info("MimeName") = content_data.AssetData.MimeName
                                'asset_info("ImageUrl") = content_data.AssetData.ImageUrl
                                'If (asset_info("MimeType") = "application/x-shockwave-flash") Then
                                'asset_info("MediaAsset") = True
                                'Else
                                'asset_info("MediaAsset") = False
                                'End If
                                'Next
                            End If
                        Else
                            ' For i = 0 To m_AssetInfoKeys.Length - 1
                            'TODO: resolve into content_data class
                            '{"AssetID", "AssetVersion", "AssetFilename", "MimeType", "FileExtension", "MimeName", "ImageUrl"}
                            asset_info("AssetID") = content_data.AssetData.Id '(m_AssetInfoKeys(i))
                            asset_info("AssetVersion") = content_data.AssetData.Version
                            asset_info("AssetFilename") = content_data.AssetData.FileName
                            asset_info("MimeType") = content_data.AssetData.MimeType
                            asset_info("FileExtension") = content_data.AssetData.FileExtension
                            asset_info("MimeName") = content_data.AssetData.MimeName
                            asset_info("ImageUrl") = content_data.AssetData.ImageUrl
                            If (asset_info("MimeType") = "application/x-shockwave-flash") Then
                                asset_info("MediaAsset") = True
                            Else
                                asset_info("MediaAsset") = False
                            End If
                            validTypes.Value = asset_info("FileExtension")
                            'Next
                        End If
                    End If
                Else
                    'Adding new file
                    Dim fileTypeCol As List(Of String) = New List(Of String)(DocumentManagerData.Instance.FileTypes.Split(","))
                    Dim allTypes As String = ""
                    For Each type As String In fileTypeCol
                        If allTypes.Length > 0 Then
                            allTypes &= "," & type.Trim.Replace("*.", "")
                        Else
                            allTypes &= type.Trim.Replace("*.", "")
                        End If
                    Next
                    validTypes.Value = allTypes
                End If
                m_intContentFolder = m_intItemId
                intInheritFrom = m_refContent.GetFolderInheritedFrom(m_intContentFolder)
                'agofpa
                If intInheritFrom <> m_intContentFolder Then
                    bGlobalSubInherit = True
                End If
                subscription_data_list = m_refContApi.GetSubscriptionsForFolder(intInheritFrom) 'AGofPA get subs for folder; set break inheritance flag false
                subscription_properties_list = m_refContApi.GetSubscriptionPropertiesForFolder(intInheritFrom) 'get folder properties
                subscribed_data_list = subscription_data_list 'get subs for folder
                'agofpa

                intContentLanguage = m_intContentLanguage
                m_refContApi.ContentLanguage = m_intContentLanguage

                meta_data = m_refContApi.GetMetaDataTypes("id")
                path = m_refContApi.GetPathByFolderID(m_intContentFolder)
                If (Right(path, 1) = "\") Then
                    path = Right(path, Len(path) - 1)
                End If

                iMaxContLength = settings_data.MaxContentSize
                iMaxSummLength = settings_data.MaxSummarySize
            End If
            If folder_data.FolderType = 1 Then
                m_bIsBlog = True
                blog_data = m_refContApi.BlogObject(folder_data)
                If (m_strType = "update") Then
                    blog_post_data = m_refContApi.GetBlogPostData(m_intItemId)
                ElseIf m_strType = "add" And m_refContentId > 0 Then ' add new lang
                    blog_post_data = m_refContApi.EkContentRef.GetBlogPostDataOnly(m_refContentId, back_LangType)
                Else
                    blog_post_data = m_refContApi.GetBlankBlogPostData()
                End If
            End If
            If (Not (IsNothing(xmlconfig_data))) Then
                If (bVer4Editor = False) Then 'only do this if we are using the old method
                    urlxml = "?Edit_xslt="
                    If (xmlconfig_data.EditXslt.Length > 0) Then
                        urlxml = urlxml & Server.UrlEncode(xmlconfig_data.LogicalPathComplete("EditXslt"))
                        If (Len(Trim(m_strContentHtml)) = 0) Then
                            m_strContentHtml = "<root> </root>"
                        End If
                    End If
                    urlxml = urlxml & "&Save_xslt="
                    If (xmlconfig_data.SaveXslt.Length > 0) Then
                        save_xslt_file = xmlconfig_data.LogicalPathComplete("SaveXslt")
                        urlxml = urlxml & Server.UrlEncode(save_xslt_file)
                    End If
                    urlxml = urlxml & "&Schema="
                    If (xmlconfig_data.XmlSchema.Length > 0) Then
                        m_strSchemaFile = xmlconfig_data.LogicalPathComplete("XmlSchema")
                        urlxml = urlxml & Server.UrlEncode(m_strSchemaFile)
                    End If
                    xml_config = AppeWebPath & "cms_xmlconfig.aspx" & urlxml
                    If (xmlconfig_data.XmlAdvConfig.Length > 0) Then
                        xml_config = xmlconfig_data.LogicalPathComplete("XmlAdvConfig") & urlxml
                    End If
                    m_strSchemaFile = xmlconfig_data.LogicalPathComplete("XmlSchema")
                    m_strNamespaceFile = xmlconfig_data.LogicalPathComplete("XmlNameSpace")
                End If
            End If

            'DHTML RENDERING
            'ASSET CONFIG
            For i = 0 To m_AssetInfoKeys.Length - 1
                AssetHidden.Text += "<input type=""hidden"" name=""asset_" & LCase(m_AssetInfoKeys(i)) & """ value=""" & Server.HtmlEncode(asset_info(m_AssetInfoKeys(i))) & """>"
            Next
            content_type.Value = lContentType
            content_subtype.Value = lContentSubType
            If m_SelectedEditControl <> "ContentDesigner" Then
                jsEditorScripts.Text = Utilities.EditorScripts(var2, AppeWebPath, BrowserCode)
            End If
            AutoNav.Text = Replace(path, "\", "\\")
            invalidFormatMsg.Text = m_refMsg.GetMessage("js: invalid date format error msg")
            invalidYearMsg.Text = m_refMsg.GetMessage("js: invalid year error msg")
            invalidMonthMsg.Text = m_refMsg.GetMessage("js: invalid month error msg")
            invalidDayMsg.Text = m_refMsg.GetMessage("js: invalid day error msg")
            invalidTimeMsg.Text = m_refMsg.GetMessage("js: invalid time error msg")

            If (MetaComplete) Then
                ecmMetaComplete.Text = "1"
            Else
                ecmMetaComplete.Text = "0"
            End If
            'TODO change to 0 or 1
            ecmMonths.Text = "" '<%=(AppUI.GetEnglishMonthsAbbrev()
            jsNullContent.Text = m_refMsg.GetMessage("null content warning msg")
            jsEDWarning.Text = m_refMsg.GetMessage("js: earlier end date warning")
            jsMetaCompleteWarning.Text = m_refMsg.GetMessage("js: alert cannot submit meta incomplete") & "\n" & m_refMsg.GetMessage("js: alert save or checkin or complete meta")
            'If (Not IsMac) Then
            jsSetActionFunction.Text = SetActionClientScript(folder_data.PublishHtmlActive)
            'End If
            jsSitePath.Text = m_refContApi.SitePath
            jsEditProLocale.Text = AppeWebPath & "locale" & AppLocaleString & "b.xml"
            ValidateContentPanel.Text = " var errReason = 0;" & vbCrLf
            ValidateContentPanel.Text += "var errReasonT = 0;" & vbCrLf
            ValidateContentPanel.Text += "var errAccess = false;" & vbCrLf
            ValidateContentPanel.Text += "var errMessage = """";" & vbCrLf
            ValidateContentPanel.Text += "var sInvalidContent = ""Continue saving invalid document?"";" & vbCrLf
            If m_SelectedEditControl <> "ContentDesigner" Then
                ValidateContentPanel.Text += "if (eWebEditProMessages) {" & vbCrLf
                ValidateContentPanel.Text += "  sInvalidContent = eWebEditProMessages.invalidContent;" & vbCrLf
                ValidateContentPanel.Text += "}" & vbCrLf
            End If
            ValidateContentPanel.Text += "var errContent = """ & m_refMsg.GetMessage("js: alert invalid data") & """;" & vbCrLf
            ValidateContentPanel.Text += "var objValidateInstance = null;" & vbCrLf
            If m_SelectedEditControl <> "ContentDesigner" Then
                ValidateContentPanel.Text += "objValidateInstance = eWebEditPro.instances[""content_html""];" & vbCrLf
                ValidateContentPanel.Text += "if (objValidateInstance){" & vbCrLf
                ValidateContentPanel.Text += "	if (!eWebEditPro.instances[""content_html""].validateContent()) {" & vbCrLf
                ValidateContentPanel.Text += "		errReason = objValidateInstance.event.reason;" & vbCrLf
                ValidateContentPanel.Text += "		if (-1001 == errReason || -1002 == errReason || 1003 == errReason || -1003 == errReason) {" & vbCrLf
                ValidateContentPanel.Text += "			errAccess = true;" & vbCrLf
                ValidateContentPanel.Text += "		}" & vbCrLf
                ValidateContentPanel.Text += "	}" & vbCrLf
                ValidateContentPanel.Text += "}" & vbCrLf
            Else
				ValidateContentPanel.Text += "  if (""object"" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances) {" & vbCrLf
				ValidateContentPanel.Text += "      var objContentEditor = Ektron.ContentDesigner.instances[""content_html""];" & vbCrLf
                ValidateContentPanel.Text += "      if (objContentEditor && ""function"" == typeof objContentEditor.validateContent) {" & vbCrLf
                ValidateContentPanel.Text += "          errMessage = objContentEditor.validateContent();" & vbCrLf
                ValidateContentPanel.Text += "      }" & vbCrLf
                ValidateContentPanel.Text += "      if (errMessage != null && errMessage != """") {" & vbCrLf
                ValidateContentPanel.Text += "          if (""object"" == typeof errMessage && ""undefined"" == typeof errMessage.code) {" & vbCrLf
                ValidateContentPanel.Text += "              alert(errMessage.join(""\n\n\n""));" & vbCrLf
                ValidateContentPanel.Text += "		        return false;" & vbCrLf
                ValidateContentPanel.Text += "          }" & vbCrLf
                ValidateContentPanel.Text += "          else if (""object"" == typeof errMessage && ""string"" == typeof errMessage.msg) {" & vbCrLf
                ValidateContentPanel.Text += "		        errReason = errMessage.code;" & vbCrLf
                ValidateContentPanel.Text += "			    errAccess = true;" & vbCrLf
                ValidateContentPanel.Text += "              alert(""Content is invalid."" + ""\n\n"" + errMessage.msg);" & vbCrLf
                ValidateContentPanel.Text += "          }" & vbCrLf
                ValidateContentPanel.Text += "          else if (""string"" == typeof errMessage && errMessage.length > 0) {" & vbCrLf
                ValidateContentPanel.Text += "              alert(errMessage);" & vbCrLf
                ValidateContentPanel.Text += "		        return false;" & vbCrLf
                ValidateContentPanel.Text += "          }" & vbCrLf
                ValidateContentPanel.Text += "      }" & vbCrLf
                ValidateContentPanel.Text += "  }" & vbCrLf
            End If
            ValidateContentPanel.Text += "var objTeaserInstance = null;" & vbCrLf
            If m_SelectedEditControl <> "ContentDesigner" Then
                ValidateContentPanel.Text += "objTeaserInstance = eWebEditPro.instances[""content_teaser""];" & vbCrLf
                ValidateContentPanel.Text += "if (objTeaserInstance){" & vbCrLf
                ValidateContentPanel.Text += "	if (!objTeaserInstance.validateContent()) {" & vbCrLf
                ValidateContentPanel.Text += "		errReasonT = objTeaserInstance.event.reason;" & vbCrLf
                ValidateContentPanel.Text += "		if (-1001 == errReasonT || -1002 == errReasonT || 1003 == errReasonT || -1003 == errReasonT) {" & vbCrLf
                ValidateContentPanel.Text += "			errAccess = true;" & vbCrLf
                ValidateContentPanel.Text += "		}" & vbCrLf
                ValidateContentPanel.Text += "	}" & vbCrLf
                ValidateContentPanel.Text += "}" & vbCrLf
            Else
				ValidateContentPanel.Text += "  if (""object"" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances && ("""" == errMessage || null == errMessage)) {" & vbCrLf
                ValidateContentPanel.Text += "      var teaserName = ""content_teaser"";" & vbCrLf
                ValidateContentPanel.Text += "      if (document.forms[0].response) {" & vbCrLf
                ValidateContentPanel.Text += "        var iTeaser = 0;" & vbCrLf
                ValidateContentPanel.Text += "        for (var i = 0; i < document.forms[0].response.length; i++) {" & vbCrLf
                ValidateContentPanel.Text += "            if (document.forms[0].response[i].checked) {" & vbCrLf
                ValidateContentPanel.Text += "                iTeaser = i;" & vbCrLf
                ValidateContentPanel.Text += "            }" & vbCrLf
                ValidateContentPanel.Text += "        }" & vbCrLf
                ValidateContentPanel.Text += "        switch (iTeaser) {" & vbCrLf
                ValidateContentPanel.Text += "            case 2: " & vbCrLf
                ValidateContentPanel.Text += "                teaserName = ""forms_transfer"";" & vbCrLf
                ValidateContentPanel.Text += "                break;" & vbCrLf
                ValidateContentPanel.Text += "            case 1:" & vbCrLf
                ValidateContentPanel.Text += "                teaserName = ""forms_redirect"";" & vbCrLf
                ValidateContentPanel.Text += "                break;" & vbCrLf
                ValidateContentPanel.Text += "            case 0:" & vbCrLf
                ValidateContentPanel.Text += "            default:" & vbCrLf
                ValidateContentPanel.Text += "                teaserName = ""content_teaser"";" & vbCrLf
                ValidateContentPanel.Text += "                break;" & vbCrLf
                ValidateContentPanel.Text += "        }" & vbCrLf
                ValidateContentPanel.Text += "      }" & vbCrLf
				ValidateContentPanel.Text += "      var objTeaserEditor = Ektron.ContentDesigner.instances[teaserName];" & vbCrLf
                ValidateContentPanel.Text += "      if (objTeaserEditor && ""function"" == typeof objTeaserEditor.validateContent){" & vbCrLf
                ValidateContentPanel.Text += "          errMessage = objTeaserEditor.validateContent();" & vbCrLf
                ValidateContentPanel.Text += "      }" & vbCrLf
                ValidateContentPanel.Text += "      if (errMessage != null && errMessage != """") {" & vbCrLf
                ValidateContentPanel.Text += "          if (""object"" == typeof errMessage && ""undefined"" == typeof errMessage.code) {" & vbCrLf
                ValidateContentPanel.Text += "              alert(errMessage.join(""\n\n\n""));" & vbCrLf
                ValidateContentPanel.Text += "		        return false;" & vbCrLf
                ValidateContentPanel.Text += "          }" & vbCrLf
                ValidateContentPanel.Text += "          else if (""object"" == typeof errMessage && ""string"" == typeof errMessage.msg) {" & vbCrLf
                ValidateContentPanel.Text += "		        errReason = errMessage.code;" & vbCrLf
                ValidateContentPanel.Text += "			    errAccess = true;" & vbCrLf
                ValidateContentPanel.Text += "              alert(""Content is invalid."" + ""\n\n"" + errMessage.msg);" & vbCrLf
                ValidateContentPanel.Text += "          }" & vbCrLf
                ValidateContentPanel.Text += "          else if (""string"" == typeof errMessage && errMessage.length > 0) {" & vbCrLf
                ValidateContentPanel.Text += "              alert(errMessage);" & vbCrLf
                ValidateContentPanel.Text += "		        return false;" & vbCrLf
                ValidateContentPanel.Text += "          }" & vbCrLf
                ValidateContentPanel.Text += "      }" & vbCrLf
                ValidateContentPanel.Text += "  }" & vbCrLf
            End If
            ValidateContentPanel.Text += "if (errReason != 0 || errReasonT != 0) {" & vbCrLf
            ValidateContentPanel.Text += "	if (errReasonT != 0 && typeof objTeaserInstance != ""undefined"" && objTeaserInstance) {" & vbCrLf
            ValidateContentPanel.Text += "		errMessage = objTeaserInstance.event.message + """";" & vbCrLf
            ValidateContentPanel.Text += "	}" & vbCrLf
            ValidateContentPanel.Text += "	if (errReason != 0 && typeof objValidateInstance != ""undefined"" && objValidateInstance) {" & vbCrLf
            ValidateContentPanel.Text += "		errMessage = objValidateInstance.event.message + """";" & vbCrLf
            ValidateContentPanel.Text += "	}" & vbCrLf
            ValidateContentPanel.Text += "	if (false == errAccess) {" & vbCrLf
            ValidateContentPanel.Text += "		alert(errContent + ""\n""  + errMessage);" & vbCrLf
			ValidateContentPanel.Text += "		$ektron(document).trigger(""wizardPanelShown"");" & vbCrLf
            ValidateContentPanel.Text += "		return false;" & vbCrLf
            ValidateContentPanel.Text += "	}" & vbCrLf
            ValidateContentPanel.Text += "	else {" & vbCrLf
            'ValidateContentPanel.Text += "	alert(""button = "" + Button );" & vbCrLf
            If "2" = settings_data.Accessibility Then
                ValidateContentPanel.Text += " if (typeof Button != ""undefined"") {" & vbCrLf
                ValidateContentPanel.Text += "		if (""publish"" == Button.toLowerCase() || ""submit"" == Button.toLowerCase()) {" & vbCrLf
                ValidateContentPanel.Text += "			alert(errContent);" & vbCrLf
				ValidateContentPanel.Text += "			$ektron(document).trigger(""wizardPanelShown"");" & vbCrLf
                ValidateContentPanel.Text += "			return false;" & vbCrLf
                ValidateContentPanel.Text += "		}" & vbCrLf
                ValidateContentPanel.Text += "		else { " & vbCrLf
                ValidateContentPanel.Text += "			if (confirm(errContent + ""\n"" + sInvalidContent)) {" & vbCrLf
                ValidateContentPanel.Text += "				return true;" & vbCrLf
                ValidateContentPanel.Text += "			} " & vbCrLf
                ValidateContentPanel.Text += "			else {" & vbCrLf
				ValidateContentPanel.Text += "			    $ektron(document).trigger(""wizardPanelShown"");" & vbCrLf
                ValidateContentPanel.Text += "			    return false;" & vbCrLf
                ValidateContentPanel.Text += "			} " & vbCrLf
                ValidateContentPanel.Text += "		}" & vbCrLf
                ValidateContentPanel.Text += " }" & vbCrLf
            ElseIf "1" = settings_data.Accessibility Then
                ValidateContentPanel.Text += " if (confirm(errContent + ""\n"" + sInvalidContent)) {" & vbCrLf
                ValidateContentPanel.Text += "	return true;" & vbCrLf
                ValidateContentPanel.Text += " } " & vbCrLf
				ValidateContentPanel.Text += " else {$ektron(document).trigger(""wizardPanelShown""); return false;} " & vbCrLf
            End If
            ValidateContentPanel.Text += "	}" & vbCrLf
            ValidateContentPanel.Text += "}" & vbCrLf
            'Change the action page
            FormAction = "edit.aspx?close=" & Request.QueryString("close") & "&LangType=" & m_intContentLanguage & "&content_id=" & m_refContentId & IIf(Me.TaxonomyOverrideId > 0, "&TaxonomyId=" & Me.TaxonomyOverrideId.ToString(), "") & IIf(Me.TaxonomySelectId > 0, "&SelTaxonomyId=" & Me.TaxonomySelectId.ToString(), "") & "&back_file=" & back_file & "&back_action=" & back_action & "&back_folder_id=" & back_folder_id & "&back_id=" & back_id & "&back_form_id=" & back_form_id & "&control=" & controlName & "&buttonid=" & buttonId.Value & "&back_LangType=" & back_LangType & back_callerpage & back_origurl
            If (Not Request.QueryString("pullapproval") Is Nothing) Then
                FormAction &= "&pullapproval=" & Request.QueryString("pullapproval")
            End If
            PostBackPage.Text = "<script>document.forms[0].action = """ & FormAction & """;"
            If (Utilities.IsAssetType(lContentType)) Then
                PostBackPage.Text += "document.forms[0].enctype = ""multipart/form-data"";"
            End If
            ' Corrupting viewstate causes an ASP.NET error on postback when telerik radEditor was on the page.
            ' This mechanism of fooling the viewstate was used when post back was to a different page.
            'PostBackPage.Text += "document.forms[0].__VIEWSTATE.name = 'NOVIEWSTATE';"
            PostBackPage.Text += "</script>"
            LoadingImg.Text = m_refMsg.GetMessage("one moment msg")

            content_title.Value = Server.HtmlDecode(m_strContentTitle)
            If (content_title.Attributes.Item("class") = Nothing) Then
                content_title.Attributes.Add("class", "")
            End If
            If (lContentSubType = CMSContentSubtype.PageBuilderMasterData) Then
                content_title.Attributes.Item("class") = "masterlayout"
                If (Not (m_strType = "update")) Then
                    content_title.Disabled = True
                End If
                phAlias.Visible = False
                EditAliasHtml.Visible = False
            Else
                content_title.Attributes.Remove("class")
            End If

            If (EnableMultilingual = 1) Then
                lblLangName.Text = "[" & language_data.Name & "]"
            End If
            Dim sbFolderBreadcrumb As New StringBuilder
            Dim strDisabled As String = ""
            If (Not (m_strType = "update")) Then
                QLink_Search.Text = "<td nowrap=""nowrap"" class=""checkboxIsSearchable"" >"
                'If (security_data.IsAdmin) Then
                '    If (Ektron.Cms.Common.EkConstants.IsAssetContentType(Me.lContentType) And Not Me.lContentType = Ektron.Cms.Common.EkConstants.CMSContentType_Media) Then
                '        strDisabled = "disabled"
                '    End If
                '    QLink_Search.Text += "<input type=""checkbox"" name=""AddQlink"" " & strDisabled & " checked value=""AddQlink""> " & m_refMsg.GetMessage("add to qlinks msg")
                'Else
                'If Me.folder_data.SitemapInherited = 1 Then
                'nTabPanelTop = 113
                'End If
                QLink_Search.Text += "<input type=""hidden"" name=""AddQlink"" value=""AddQlink"">"
                'End If
                'QLink_Search.Text += "</td><td>"
                If (security_data.IsAdmin) Then
                    QLink_Search.Text += "<input type=""checkbox"" name=""IsSearchable"" " & strDisabled & " checked value=""IsSearchable"" >" & m_refMsg.GetMessage("lbl content searchable") 'm_refMsg.GetMessage("Content Searchable")
                Else
                    QLink_Search.Text += "<input type=""hidden"" name=""IsSearchable"" value=""IsSearchable"">"
                End If
                QLink_Search.Text += "</td>"
            Else
                'If Me.folder_data.SitemapInherited = 1 Then
                nTabPanelTop = 105
                TR_Properties.Visible = False
                TR_Properties.Height = "0"
                'End If
            End If
            'If Me.folder_data.SitemapInherited = 0 Then
            '    Dim node As New Ektron.Cms.Common.SitemapPath
            '    Dim bSitemapExists As Boolean = False
            '    If (m_strType = "update") Then
            '        node.Url = content_edit_data.Quicklink
            '        node.Title = content_edit_data.Title
            '        node.FolderId = content_edit_data.FolderId
            '    End If

            '    sbFolderBreadcrumb.Append("<td><input type=""checkbox"" id =""ckAddFolderBreadCrumb"" name=""ckAddFolderBreadCrumb"" ")
            '    If (m_strType = "update") Then
            '        If (Utilities.FindSitemapPath(folder_data.SitemapPath, node) > -1) Then
            '            sbFolderBreadcrumb.Append(" checked ")
            '            bSitemapExists = True
            '        End If
            '    End If
            '    sbFolderBreadcrumb.Append("/>Add To Folder Breadcrumb")
            '    sbFolderBreadcrumb.Append("<input type=""hidden"" name=""previousInFolderBreadcrumb"" in=""previousInFolderBreadcrumb"" value=""")
            '    If (bSitemapExists) Then
            '        sbFolderBreadcrumb.Append("true")
            '    Else
            '        sbFolderBreadcrumb.Append("false")
            '    End If
            '    sbFolderBreadcrumb.Append(""" />")
            '    sbFolderBreadcrumb.Append("</td>")
            '    QLink_Search.Text += sbFolderBreadcrumb.ToString()
            'End If

            If (QLink_Search.Text <> "") Then
                QLink_Search.Text = "<table><tr>" & QLink_Search.Text & "</tr></table>"
            End If
            content_id.Value = m_refContentId
            type.Value = m_strType
            mycollection.Value = strMyCollection
            addto.Value = strAddToCollectionType
            content_folder.Value = m_intContentFolder
            content_language.Value = intContentLanguage
            'contentteaser.Value = Server.HtmlEncode(content_teaser)
            maxcontentsize.Value = iMaxContLength
            If (bVer4Editor) Then
                Ver4Editor.Value = "true"
            Else
                Ver4Editor.Value = "false"
            End If
            createtask.Value = Request.QueryString("dontcreatetask")

            EnumeratedHiddenFields.Text = HideVariables()
            eWebEditProJS.Text = EditProJS()

            If (m_intContentType = 2) Then
                divContentText.Text = m_refMsg.GetMessage("form text")
                divSummaryText.Text = m_refMsg.GetMessage("postback text")
            Else
                divContentText.Text = m_refMsg.GetMessage("content text")
                divSummaryText.Text = m_refMsg.GetMessage("Summary text")
            End If

            phMetadata.Visible = True
            'ekRW = m_refContApi.EkUrlRewriteRef
            'ekRW.Load()

            'Hide the alias tab for managed assets
            If Me.Request.QueryString("type") = "add" Then
                'aliasContentType = Me.Request.QueryString("ContType")
            ElseIf Me.Request.QueryString("type") = "update" Then
                aliasContentType = Me.content_edit_data.ContType
            End If

            If ((m_urlAliasSettings.IsManualAliasEnabled Or m_urlAliasSettings.IsAutoAliasEnabled) _
                And m_refContApi.IsARoleMember(Common.EkEnumeration.CmsRoleIds.EditAlias) And Request.QueryString("type") <> "multiple,add" _
                And lContentSubType <> CMSContentSubtype.PageBuilderMasterData) Then  'And Not (m_bIsBlog)
                'And (ekRW.aliasOn = True) _ Old alias
                'And aliasContentType <> 102 Then
                If (content_edit_data IsNot Nothing AndAlso content_edit_data.AssetData IsNot Nothing AndAlso Ektron.Cms.Common.EkFunctions.IsImage("." & content_edit_data.AssetData.FileExtension)) Then
                    phAlias.Visible = False
                    EditAliasHtml.Visible = False
                Else
                    phAlias.Visible = True
                    EditAliasHtml.Visible = True
                End If
            End If
            EditContentHtmlScripts()
            EditSummaryHtmlScripts()
            EditMetadataHtmlScripts()
            EditAliasHtmlScripts()
            EditScheduleHtmlScripts()
            EditCommentHtmlScripts()
            EditSubscriptionHtmlScripts()
            EditSelectedTemplate()
            EditTaxonomyScript()

            If (eWebEditProPromptOnUnload = 1) Then
                jsActionOnUnload.Text = "eWebEditPro.actionOnUnload = EWEP_ONUNLOAD_PROMPT;"
            End If

            If (Convert.ToString(m_intContentFolder) <> "") Then
                defaultFolderId.Text = m_intContentFolder
            Else
                defaultFolderId.Text = 0
            End If

            'Summary_Meta_win
            If (Request.QueryString("summary") <> "") Then
                Summary_Meta_Win.Text = "<script language=""JavaScript1.2"">"
                Summary_Meta_Win.Text += "PopUpWindow('editsummaryarea.aspx?id=" & m_intItemId & "&LangType=" & m_intContentLanguage & "&editor=true','Summary',790,580,1,1);"
                Summary_Meta_Win.Text += "</script>"
            End If
            If (Request.QueryString("meta") <> "") Then
                Summary_Meta_Win.Text += "<script language=""JavaScript1.2"">"
                If (MetaDataNumber > 0) Then
                    Summary_Meta_Win.Text += "PopUpWindow('editmeta_dataarea.aspx?id=" & m_intItemId & "&LangType=" & m_intContentLanguage & "&editor=true','Metadata',790,580,1,1);"

                Else
                    Summary_Meta_Win.Text += "alert('No metadata defined');  "
                End If
                Summary_Meta_Win.Text += "</script>"
            End If
            'TAXONOMY DATA
            If (IsAdmin Or m_refContent.IsARoleMember(Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator, CurrentUserID)) Then
                TaxonomyRoleExists = True
            End If
            Dim taxonomy_cat_arr As TaxonomyBaseData() = Nothing
            If (m_strType <> "add" AndAlso m_strType <> "multiple" AndAlso (Not (m_strType.IndexOf("add", System.StringComparison.InvariantCultureIgnoreCase) > 0 OrElse m_strType.IndexOf("multiple", System.StringComparison.InvariantCultureIgnoreCase) > 0)) OrElse (m_strType = "add" AndAlso m_refContentId > 0)) Then
                Dim tmpLang As Integer
                Dim originalLangID As Integer
                If m_strType = "add" AndAlso m_refContentId > 0 Then 'New Language
                    If (Not Request.QueryString("con_lang_id") Is Nothing AndAlso Request.QueryString("con_lang_id") <> "") Then
                        originalLangID = Convert.ToInt32(Request.QueryString("con_lang_id"))
                    End If
                    tmpLang = m_refContent.RequestInformation.ContentLanguage 'Backup the current langID
                    m_refContent.RequestInformation.ContentLanguage = originalLangID
                    taxonomy_cat_arr = m_refContent.ReadAllAssignedCategory(m_refContentId)
                    m_refContent.RequestInformation.ContentLanguage = tmpLang
                Else
                    taxonomy_cat_arr = m_refContent.ReadAllAssignedCategory(m_intItemId)
                End If
                If (taxonomy_cat_arr IsNot Nothing AndAlso taxonomy_cat_arr.Length > 0) Then
                    For Each taxonomy_cat As TaxonomyBaseData In taxonomy_cat_arr
                        If (taxonomyselectedtree.Value = "") Then
                            taxonomyselectedtree.Value = Convert.ToString(taxonomy_cat.TaxonomyId)
                        Else
                            taxonomyselectedtree.Value = taxonomyselectedtree.Value & "," & Convert.ToString(taxonomy_cat.TaxonomyId)
                        End If
                    Next
                End If
                TaxonomyTreeIdList = taxonomyselectedtree.Value
                If (TaxonomyTreeIdList.Trim.Length > 0) Then
                    If m_strType = "add" AndAlso m_refContentId > 0 Then 'New Language
                        m_refContent.RequestInformation.ContentLanguage = originalLangID 'Backup the current LangID
                        TaxonomyTreeParentIdList = m_refContent.ReadDisableNodeList(m_refContentId)
                        m_refContent.RequestInformation.ContentLanguage = tmpLang
                    Else
                        TaxonomyTreeParentIdList = m_refContent.ReadDisableNodeList(m_intItemId)
                    End If
                End If
            Else
                If (TaxonomySelectId > 0) Then
                    taxonomyselectedtree.Value = TaxonomySelectId
                    TaxonomyTreeIdList = taxonomyselectedtree.Value
                    taxonomy_cat_arr = m_refContent.GetTaxonomyRecursiveToParent(TaxonomySelectId, m_refContent.RequestInformation.ContentLanguage, 0)
                    If (taxonomy_cat_arr IsNot Nothing AndAlso taxonomy_cat_arr.Length > 0) Then
                        For Each taxonomy_cat As TaxonomyBaseData In taxonomy_cat_arr
                            If (TaxonomyTreeParentIdList = "") Then
                                TaxonomyTreeParentIdList = Convert.ToString(taxonomy_cat.TaxonomyId)
                            Else
                                TaxonomyTreeParentIdList = TaxonomyTreeParentIdList & "," & Convert.ToString(taxonomy_cat.TaxonomyId)
                            End If
                        Next
                    End If
                End If
            End If

            Dim taxonomy_request As New TaxonomyRequest
            Dim taxonomy_data_arr As TaxonomyBaseData() = Nothing
            Utilities.SetLanguage(m_refContApi)
            taxonomy_request.TaxonomyId = m_intContentFolder
            taxonomy_request.TaxonomyLanguage = m_refContApi.ContentLanguage
            taxonomy_data_arr = m_refContent.GetAllFolderTaxonomy(m_intContentFolder)
            Dim HideCategoryTab As Boolean = False
            If (Request.QueryString("HideCategoryTab") IsNot Nothing) Then
                HideCategoryTab = Convert.ToBoolean(Request.QueryString("HideCategoryTab"))
            End If
            If (HideCategoryTab OrElse (taxonomy_data_arr Is Nothing OrElse taxonomy_data_arr.Length = 0) AndAlso (TaxonomyOverrideId = 0)) Then
                If (Not HideCategoryTab AndAlso Not IsNothing(folder_data) AndAlso folder_data.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Blog AndAlso TaxonomySelectId > 0 AndAlso m_intTaxFolderId = folder_data.Id AndAlso TaxonomyTreeIdList.Trim.Length > 0) Then
                    m_intTaxFolderId = 0
                Else
                    phTaxonomy.Visible = False
                    EditTaxonomyHtml.Visible = False
                    DisplayTab = False
                End If
            End If

            'CALL THE TOOLBAR
            If folder_data Is Nothing Then
                LoadToolBar("")
            Else
                LoadToolBar(folder_data.Name)
            End If

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Protected Function GetFlaggingScript() As String
        Dim result As New System.Text.StringBuilder
        Dim aFlagSets As FolderFlagDefData = Nothing ' Array.CreateInstance(GetType(Ektron.Cms.FlagDefData), 0)
        Dim selectedStr As String = ""

        Try
            ' Display content flagging options:
            result.Append("<span style=""position: relative; top: 0px; left: 5px;"" ><select id=""FlaggingDefinitionSel"" name=""FlaggingDefinitionSel"" >" + Environment.NewLine)

            aFlagSets = (New Community.FlaggingAPI).GetDefaultFolderFlagDef(folder_data.Id)
            If (aFlagSets Is Nothing) Then
                result.Append("<option value=""0"">None Available -HC</option>" + Environment.NewLine)
            Else
                If (aFlagSets.ID > 0) Then
                    selectedStr = "selected=""selected"" "
                Else
                    selectedStr = ""
                End If
                result.Append("<option value=""" + aFlagSets.ID.ToString() + """ " + selectedStr + ">" + aFlagSets.Name + "</option>" + Environment.NewLine)
            End If
            result.Append("</select></span>" + Environment.NewLine)
            result.Append("<span style=""position: relative; top: -1px; left: 5px;"" >Flagging</span>" + Environment.NewLine)

        Catch ex As Exception
        Finally
            GetFlaggingScript = result.ToString()
            result = Nothing
            aFlagSets = Nothing
        End Try
    End Function

    Private Sub EditTaxonomyScript()
        EditTaxonomyHtml.Text = "<div id=""dvTaxonomy"">"
        EditTaxonomyHtml.Text &= m_refMsg.GetMessage("select taxonomy label")
        EditTaxonomyHtml.Text &= "<div id=""TreeOutput"" class=""ektronTreeContainer""></div>"
        EditTaxonomyHtml.Text &= "</div>"
    End Sub
    Private Sub EditContentHtmlScripts()
        Dim sbHtml As New StringBuilder
        Dim strAssetID As String = ""
        Dim strSnippet As String = ""
        Dim strPath As String = ""
        Dim editLiveCSS As String = ""
        Dim copyContID As Long = 0
        Dim addFileUpload As Boolean = False
        Dim MenuItemType As String

        If Len(content_stylesheet) > 0 Then
            strPath = GetServerPath() & SitePath & content_stylesheet
            editLiveCSS = "&css=" & content_stylesheet
        End If
        If Not (IsNothing(Request.QueryString("content_id"))) AndAlso (Request.QueryString("content_id") <> "") Then
            'this key is also used for media asset translated.
            copyContID = Request.QueryString("content_id")
        End If

        isOfficeDoc.Value = "false"
        MultiupLoadTitleMsg.Text = ""
        content_title.Visible = True

        If ((IsMac) AndAlso Not (Utilities.IsAsset(lContentType, strAssetID))) Then
            If Not content_edit_data Is Nothing AndAlso (content_edit_data.Type = 1 Or content_edit_data.Type = 3) AndAlso (content_edit_data.SubType = CMSContentSubtype.PageBuilderData Or content_edit_data.SubType = CMSContentSubtype.PageBuilderMasterData Or content_edit_data.SubType = CMSContentSubtype.WebEvent) Then
                Dim typeaction As String = Request.QueryString("type")

                If typeaction IsNot Nothing AndAlso typeaction.ToLower() = "update" Then
                    isOfficeDoc.Value = "true"
                End If
                Dim linebreak As New HtmlGenericControl("div")
                linebreak.InnerHtml = "<br /><br />"
                m_ctlContentPane.Controls.Add(linebreak)
                Dim htmlGen As New HtmlGenericControl("span")
                htmlGen.InnerHtml = Ektron.Cms.PageBuilder.PageData.RendertoString(content_edit_data.Html)
                m_ctlContentPane.Controls.Add(htmlGen)
            ElseIf "ContentDesigner" = m_SelectedEditControl Then
                With m_ctlContentDesigner
                    .Visible = True
                    .Width = New Unit(100, UnitType.Percentage)
                    .Height = New Unit(635, UnitType.Pixel)
                    If Len(content_stylesheet) > 0 Then
                        .Stylesheet = strPath
                    End If
                    If Len(editorPackage) > 0 Then
                        .LoadPackage(m_refContApi, editorPackage)
                        .DataDocumentXml = m_strContentHtml
                    Else
                        .Content = m_strContentHtml
                    End If
                End With
                With m_ctlContentValidator
                    .ValidationExpression = Utilities.BuildRegexToCheckMaxLength(iMaxContLength)
                    .Visible = True
                End With
            Else
                If (Not IsBrowserIE) Then
                    sbHtml.Append("<input type=""hidden"" name=""ephox"" id=""ephox"" value=""true"">")
                    sbHtml.Append("<input type=""hidden"" name=""selectedtext"" id=""selectedtext"">")
                    sbHtml.Append("<input type=""hidden"" name=""selectedhtml"" id=""selectedhtml"">")
                    Dim strJSEditLive As New System.Text.StringBuilder
                    strJSEditLive.Append("<script language=""JavaScript"" src=""" & Me.AppeWebPath & "editlivejava/editlivejava.js""></script>" & vbCrLf)
                    sbHtml.Append("<input type=""hidden"" name=""EphoxContent"" id=""EphoxContent"" value=""" & Server.UrlEncode(Server.HtmlDecode(m_strContentHtml)) & """>")
                    strJSEditLive.Append("<script language=""JavaScript"">" & vbCrLf)
                    strJSEditLive.Append("      var strContent;")
                    strJSEditLive.Append("		elx1 = new EditLiveJava(""content_html"", ""700"", ""400"");")
                    'strJSEditLive.Append("		elx1.setXMLURL(""" & Me.AppeWebPath & "editlivejava/config.aspx?apppath=" & Me.AppPath & """);")
                    strJSEditLive.Append("		elx1.setXMLURL(""" & Me.AppeWebPath & "editlivejava/config.aspx?apppath=" & Me.AppPath & "&sitepath=" & Me.SitePath & editLiveCSS & """);")
                    strJSEditLive.Append("      elx1.setOutputCharset(""UTF-8"");")
                    strJSEditLive.Append("		elx1.setBody(document.forms[0].EphoxContent.value);")
                    strJSEditLive.Append("		elx1.setDownloadDirectory(""" & Me.AppeWebPath & "editlivejava"");")
                    strJSEditLive.Append("		elx1.setLocalDeployment(false);")
                    strJSEditLive.Append("		elx1.setCookie("""");")
                    strJSEditLive.Append("		elx1.show();" & vbCrLf)
                    strJSEditLive.Append("	</script>" & vbCrLf)
                    sbHtml.Append(strJSEditLive.ToString())
                Else
                    sbHtml.Append("<input type=""hidden"" name=""ephox"" id=""ephox"" value=""false"">")
                    sbHtml.Append("<textarea id=""content_html"" name=""content_html"" cols=""90"" rows=""24"" ID=""Textarea2"">" & m_strContentHtml & "</textarea>")
                End If

                Dim litSnippet As New Literal
                With litSnippet
                    .ID = "ephox_control_literal"
                    .Text = sbHtml.ToString()
                End With
                m_ctlContentPane.Controls.Add(litSnippet)
            End If
        Else
            sbHtml.Append("<input type=""hidden"" name=""ephox"" id=""ephox"" value=""false"">")
            strAssetID = asset_info("AssetID")
            If Not content_edit_data Is Nothing AndAlso (content_edit_data.Type = 1 Or content_edit_data.Type = 3) AndAlso (content_edit_data.SubType = CMSContentSubtype.PageBuilderData Or content_edit_data.SubType = CMSContentSubtype.PageBuilderMasterData Or content_edit_data.SubType = CMSContentSubtype.WebEvent) Then
                Dim typeaction As String = Request.QueryString("type")

                If typeaction IsNot Nothing AndAlso typeaction.ToLower() = "update" Then
                    isOfficeDoc.Value = "true"
                End If
                Dim linebreak As New HtmlGenericControl("div")
                linebreak.InnerHtml = "<br /><br />"
                m_ctlContentPane.Controls.Add(linebreak)
                Dim htmlGen As New HtmlGenericControl("span")
                htmlGen.InnerHtml = Ektron.Cms.PageBuilder.PageData.RendertoString(content_edit_data.Html)
                m_ctlContentPane.Controls.Add(htmlGen)




            ElseIf Utilities.IsAsset(lContentType, strAssetID) Then
                If (m_strType = "multiple,add") Then
                    Dim linebreak As New HtmlGenericControl("div")
                    'linebreak.InnerHtml &= "<div id=idMultipleView style='display:none'><object id=idUploadCtl name=idUploadCtl classid=CLSID:07B06095-5687-4d13-9E32-12B4259C9813 width='100%' height='350px'></object></div>"
                    linebreak.InnerHtml &= "<br /><br />" & vbCrLf
                    linebreak.InnerHtml &= "<div id=idMultipleView style='display:none'>"
                    linebreak.InnerHtml &= "<script type=""text/javascript"">" & vbCrLf
                    linebreak.InnerHtml &= " AC_AX_RunContent('id','idUploadCtl','name','idUploadCtl','classid','CLSID:07B06095-5687-4d13-9E32-12B4259C9813','width','100%','height','350px');" & vbCrLf
                    linebreak.InnerHtml &= vbCrLf & " </script> </div> " & vbCrLf
                    linebreak.InnerHtml &= "<br /><br />"
                    linebreak.InnerHtml &= "<div> " & m_refMsg.GetMessage("lbl valid file types") & DocumentManagerData.Instance.FileTypes & "</div>"
                    m_ctlContentPane.Controls.Add(linebreak)
                    strSnippet &= vbCrLf & "<script language=""JavaScript"">" & vbCrLf
                    strSnippet &= "MultipleUploadView();" & vbCrLf
                    strSnippet &= vbCrLf & "</script>"
                    content_title.Visible = False
                    MultiupLoadTitleMsg.Text = m_refMsg.GetMessage("lbl msg for multiupload title")
                ElseIf strAssetID.Length = 0 Then
                    'strSnippet = m_refContApi.GetCreateSnippet(lContentType)
                    Dim linebreak As New HtmlGenericControl("div")
                    linebreak.InnerHtml = "<br /><br />"
                    m_ctlContentPane.Controls.Add(linebreak)
                    Dim htmlGen As New HtmlGenericControl("span")
                    htmlGen.InnerHtml = "Please select a file to upload "
                    m_ctlContentPane.Controls.Add(htmlGen)

                    Dim fileUploadWrapper As New HtmlGenericControl("span")
                    fileUploadWrapper.ID = "fileUploadWrapper"

                    Dim fileUpload As New WebControls.FileUpload()
                    fileUpload.ID = "fileupload"

                    fileUploadWrapper.Controls.Add(fileUpload)
                    m_ctlContentPane.Controls.Add(fileUploadWrapper)

                    'fileUpload.Visible = True
                    oldfilename.Value = ""
                    addFileUpload = True
                Else
                    If (lContentType = CMSContentType_Media) Then

                        Dim mediaParams As Multimedia_commonparams
                        mediaParams = CType(LoadControl("controls/media/commonparams.ascx"), Multimedia_commonparams)
                        mediaParams.ID = m_strContentTitle
                        If (m_strType = "add" And copyContID <> 0) Then
                            mediaParams.ContentHtml = Me.m_refContent.CreateMediaXML(content_data.AssetData, m_intContentFolder)
                            mediaParams.AssetID = content_data.AssetData.Id
                            mediaParams.MimeType = content_data.AssetData.MimeType
                            mediaParams.AssetVersion = content_data.AssetData.Version
                            mediaParams.AssetFileName = m_refContApi.GetViewUrl(content_data.AssetData.Id, CMSContentType_Media)
                        Else
                            mediaParams.ContentHtml = m_strContentHtml
                            mediaParams.AssetID = strAssetID
                            mediaParams.MimeType = asset_info("MimeType")
                            mediaParams.AssetVersion = asset_info("AssetVersion")
                            mediaParams.AssetFileName = m_refContApi.RequestInformationRef.AssetPath & m_refContApi.EkContentRef.GetFolderParentFolderIdRecursive(content_edit_data.FolderId).Replace(",", "/") & "/" & content_edit_data.AssetData.Id & "." & content_edit_data.AssetData.FileExtension
                            mediaParams.AssetFileName = IIf(content_edit_data.IsPrivate, m_refContApi.RequestInformationRef.SitePath & "PrivateAssets/", m_refContApi.RequestInformationRef.AssetPath) & m_refContApi.EkContentRef.GetFolderParentFolderIdRecursive(content_edit_data.FolderId).Replace(",", "/") & "/" & content_edit_data.AssetData.Id & "." & content_edit_data.AssetData.FileExtension
                            'mediaParams.AssetFileName = m_refContApi.GetViewUrl(content_edit_data.AssetData.Id, CMSContentType_Media)
                        End If

                        m_ctlContentPane.Controls.Add(mediaParams)

                        'strSnippet = m_refContApi.EkContentRef.GetEditDragDropSnippet(strAssetID, lContentType) '.GetEditSnippet(strAssetID, lContentType).Replace(".parameters.Multiple = ""2"";", ".parameters.Multiple = ""4"";")
                    Else
                        'check for type = 'add' here
                        If (m_strType = "add") Then
                            Dim linebreak As New HtmlGenericControl("div")
                            linebreak.InnerHtml = "<br /><br />"
                            m_ctlContentPane.Controls.Add(linebreak)
                            Dim htmlGen As New HtmlGenericControl("span")
                            htmlGen.InnerHtml = "Please select a file to upload "

                            Dim fileUploadWrapper As New HtmlGenericControl("span")
                            fileUploadWrapper.ID = "fileUploadWrapper"

                            Dim fileUpload As New WebControls.FileUpload()
                            fileUpload.ID = "fileupload"

                            fileUploadWrapper.Controls.Add(fileUpload)
                            m_ctlContentPane.Controls.Add(fileUploadWrapper)

                            'fileUpload.Visible = True
                            oldfilename.Value = ""
                            addFileUpload = True
                        Else
                            If (Ektron.ASM.AssetConfig.ConfigManager.IsOfficeDoc(content_edit_data.AssetData.FileExtension)) Then
                                'strSnippet &= vbCrLf & "<script language=""JavaScript"">" & vbCrLf
                                'strSnippet &= " var obj = new ActiveXObject('SharePoint.OpenDocuments.2');" & vbCrLf
                                Dim assetmanagementService As AssetManagement.AssetManagementService = New AssetManagement.AssetManagementService
                                Dim assetData As Ektron.ASM.AssetConfig.AssetData = assetmanagementService.GetAssetData(content_edit_data.AssetData.Id)
                                Dim strfilename As String
                                strfilename = GetFolderPath(content_edit_data.FolderId) & assetData.Handle
                                'strSnippet &= " obj.EditDocument2(window,'" & strfilename & "', '');" & vbCrLf
                                'strSnippet &= vbCrLf & "</script>"
                                filename.Value = strfilename
                                Dim linebreak As New HtmlGenericControl("div")
                                linebreak.InnerHtml = "<br /><br /> Currently uploaded file: " & assetData.Handle & " <br /><br />"
                                m_ctlContentPane.Controls.Add(linebreak)
                                m_ctlContentPane.Controls.Add(linebreak)
                                Dim htmlGen As New HtmlGenericControl("span")
                                htmlGen.InnerHtml = "Please select a file to upload "
                                m_ctlContentPane.Controls.Add(htmlGen)

                                Dim fileUploadWrapper As New HtmlGenericControl("span")
                                fileUploadWrapper.ID = "fileUploadWrapper"

                                Dim fileUpload As New WebControls.FileUpload()
                                fileUpload.ID = "fileupload"

                                fileUploadWrapper.Controls.Add(fileUpload)
                                m_ctlContentPane.Controls.Add(fileUploadWrapper)

                                oldfilename.Value = assetData.Handle
                                'This hidden field is used to hide content tab if office is not installed or browser is non-ie, else show content tab with browse button
                                isOfficeDoc.Value = "true"
                            Else
                                Dim assetmanagementService As AssetManagement.AssetManagementService = New AssetManagement.AssetManagementService
                                Dim assetData As Ektron.ASM.AssetConfig.AssetData = assetmanagementService.GetAssetData(asset_info("AssetID"))
                                Dim linebreak As New HtmlGenericControl("div")
                                linebreak.InnerHtml = "<br /><br /> Currently uploaded file: " & assetData.Handle & " <br /><br />"
                                m_ctlContentPane.Controls.Add(linebreak)
                                m_ctlContentPane.Controls.Add(linebreak)
                                Dim htmlGen As New HtmlGenericControl("span")
                                htmlGen.InnerHtml = "Please select a file to upload "
                                m_ctlContentPane.Controls.Add(htmlGen)

                                Dim fileUploadWrapper As New HtmlGenericControl("span")
                                fileUploadWrapper.ID = "fileUploadWrapper"

                                Dim fileUpload As New WebControls.FileUpload()
                                fileUpload.ID = "fileupload"

                                fileUploadWrapper.Controls.Add(fileUpload)
                                m_ctlContentPane.Controls.Add(fileUploadWrapper)

                                oldfilename.Value = assetData.Handle
                            End If
                            MenuItemType = Request.QueryString("menuItemType")
                            If MenuItemType IsNot Nothing AndAlso MenuItemType.ToLower() = "editproperties" Then
                                isOfficeDoc.Value = "true"
                            End If
                        End If
                        'strSnippet = m_refContApi.GetEditSnippet(strAssetID, lContentType)
                    End If
                End If
                sbHtml.Append(strSnippet)
                'sbHtml.Append("<script language=""JavaScript"" src=""Tree/js/com.ektron.utils.string.js""></script>" & vbCrLf)
                'sbHtml.Append("<script language=""JavaScript"" src=""Tree/js/com.ektron.utils.cookie.js""></script>" & vbCrLf)
                'sbHtml.Append("<script language=""JavaScript"" src=""java/assetevents.js""></script>" & vbCrLf)
                'sbHtml.Append("<script language=""JavaScript"">" & vbCrLf)
                'sbHtml.Append("<!--" & vbCrLf)
                'TODO Set additional params here
                'sbHtml.Append("	function setPostInfo() {" & vbCrLf) ' also in viewfolder.ascx
                'sbHtml.Append("		if ((typeof EktAsset == 'object') && EktAsset && EktAsset.instances[0] && EktAsset.instances[0].isReady()) {" & vbCrLf)
                'sbHtml.Append("			var basehosturl = location.protocol + '//' + location.host;" & vbCrLf)
                'sbHtml.Append("			g_AssetHandler.SetPostInfo(basehosturl + '")
                'sbHtml.Append(m_refContApi.AppPath)
                'sbHtml.Append("ProcessUpload.aspx', ")
                'sbHtml.Append(CStr(m_intContentLanguage))
                'sbHtml.Append(", ")
                'sbHtml.Append(CStr(m_intContentFolder))
                'Dim folder_data As FolderData
                'folder_data = m_refContApi.GetFolderById(m_intContentFolder)
                'If (Not IsNothing(folder_data)) Then
                '    sbHtml.Append(", ")
                '    If folder_data.PublishHtmlActive Then
                '        sbHtml.Append("1")
                '    Else
                '        sbHtml.Append("0")
                '    End If
                'End If

                'If (m_strType = "add" And m_strCopy = "copy") Then
                '    sbHtml.Append(",'copy', ")
                '    sbHtml.Append(copyContID)
                'ElseIf (m_strType = "update") Then
                '    sbHtml.Append(",'update', ")
                '    sbHtml.Append(m_intItemId)
                'Else
                '    sbHtml.Append(",'add',-1 ")
                'End If
                'If (System.Configuration.ConfigurationManager.AppSettings("ek_BatchSize") <> "") Then
                '    sbHtml.Append(", ")
                '    sbHtml.Append(System.Configuration.ConfigurationManager.AppSettings("ek_BatchSize"))
                'End If
                'sbHtml.Append(");" & vbCrLf)
                'sbHtml.Append("		} else {" & vbCrLf)
                'sbHtml.Append("			setTimeout('setPostInfo()',100);" & vbCrLf)
                'sbHtml.Append("		}" & vbCrLf)
                'sbHtml.Append("	}" & vbCrLf)
                'sbHtml.Append("	function setFormPostInfo(actiontype) { " & vbCrLf)
                'sbHtml.Append("		if ((typeof EktAsset == 'object') &&" & vbCrLf)
                'sbHtml.Append("			(EktAsset.instances[0].isReady())) {" & vbCrLf)
                'sbHtml.Append("			g_AssetHandler.SetFormPostInfo(actiontype);" & vbCrLf)
                'sbHtml.Append("		}" & vbCrLf)
                'sbHtml.Append("	}" & vbCrLf)
                'sbHtml.Append("	function SetPostEventSave() {" & vbCrLf)
                'sbHtml.Append("		if ((typeof EktAsset == 'object') &&" & vbCrLf)
                'sbHtml.Append("			(EktAsset.instances[0].isReady())) {" & vbCrLf)
                'sbHtml.Append("			g_AssetHandler.SetPostEventSave(true);" & vbCrLf)
                'sbHtml.Append("		}" & vbCrLf)
                'sbHtml.Append("	}" & vbCrLf)
                'If (lContentType = CMSContentType_Media) Then
                '    sbHtml.Append("	SetPostEventSave(); " & vbCrLf)
                'End If
                ' sbHtml.Append("//-->" & vbCrLf)
                'sbHtml.Append("</script>" & vbCrLf)
                sbHtml.Append("<input type=""hidden"" id=""content_html"" name=""content_html"" value=""" & Server.HtmlEncode(m_strContentHtml) & """>")

                'fix for 32909 - in case of Add multimedia file to Menu, lContentType is CMSContentType_Media but
                'since it is add we show the fileupload browse button, not the DragDropExplorer control
                If ((lContentType = CMSContentType_Media) AndAlso (addFileUpload = False)) Then
                    Dim DragDropContainer As New HtmlGenericControl("div")
                    Dim msgReplaceLit As New Literal()
                    msgReplaceLit.Text = "<div id=""ReplaceMsg"">" & m_refMsg.GetMessage("alt please drag and drop asset to replace") & "</div>"
                    DragDropContainer.Controls.Add(msgReplaceLit)
                    DragDropContainer.ID = "DragDropContainer"
                    Dim ddSnippet As New Ektron.Cms.Controls.ExplorerDragDrop()
                    ddSnippet.ID = "DragdropSnippent"
                    If (m_strType = "update") Then
                        ddSnippet.AssetID = m_refContentId
                        Dim taxonomies As TaxonomyBaseData() = m_refContent.ReadAllAssignedCategory(m_refContentId)
                        If (Not (IsNothing(taxonomies)) AndAlso taxonomies.Length > 0) Then
                            ddSnippet.TaxonomyId = taxonomies(0).TaxonomyId
                        End If
                    End If
                    ddSnippet.Page = Me.Page
                    ddSnippet.FolderID = m_intContentFolder
                    ddSnippet.ContentLanguage = m_intContentLanguage
                    If (m_strType = "add") Then
                        ddSnippet.OverrideExtension = content_data.AssetData.FileExtension
                    Else
                        Dim _assetData As New Ektron.ASM.AssetConfig.AssetData
                        _assetData.AssetDataFromAssetID(content_edit_data.AssetData.Id)
                        msgReplaceLit.Text = "<div id=""ReplaceMsg"">" & m_refMsg.GetMessage("alt please drag and drop asset with filename to replace") & _assetData.Handle & "</div>"
                        ddSnippet.OverrideExtension = content_edit_data.AssetData.FileExtension
                    End If
                    DragDropContainer.Controls.Add(ddSnippet)
                    m_ctlContentPane.Controls.Add(DragDropContainer)
                End If
                Dim litSnippet As New Literal
                With litSnippet
                    .ID = "ContentHtml"
                    .Text = sbHtml.ToString()
                End With
                m_ctlContentPane.Controls.Add(litSnippet)
            Else
                'sbHtml.Append(Utilities.eWebEditProEditor("content_html", "100%", "100%", ""))
                ''for debugging in eWebEditPro VB6 IDE.
                'sbHtml.Append("<input type=""button"" name=""btnOnReady"" value=""Fire onReady for debugging"" onclick=""initTransferMethod('content_html', 'mediamanager.aspx', 'autoupload.aspx');initTransferMethod('content_teaser', 'mediamanager.aspx', 'autoupload.aspx')"" />")
                If (m_strType = "add" AndAlso content_data IsNot Nothing AndAlso (content_data.SubType = CMSContentSubtype.PageBuilderData Or content_data.SubType = CMSContentSubtype.PageBuilderMasterData Or content_data.SubType = CMSContentSubtype.WebEvent)) Then
                    isOfficeDoc.Value = "true"
                End If
                Dim ctlEphox As New HtmlInputHidden
                With ctlEphox
                    .ID = "ephox"
                    .Value = "false"
                End With
                m_ctlContentPane.Controls.Add(ctlEphox)

                If "ContentDesigner" = m_SelectedEditControl Then
                    With m_ctlContentDesigner
                        .Visible = True
                        .Width = New Unit(100, UnitType.Percentage)
                        .Height = New Unit(635, UnitType.Pixel)
                        If Len(content_stylesheet) > 0 Then
                            .Stylesheet = strPath
                        End If
                        If Len(editorPackage) > 0 Then
                            .LoadPackage(m_refContApi, editorPackage)
                            .DataDocumentXml = m_strContentHtml
                        Else
                            .Content = m_strContentHtml
                        End If
                    End With
                    With m_ctlContentValidator
                        .ValidationExpression = Utilities.BuildRegexToCheckMaxLength(iMaxContLength)
                        .Visible = True
                    End With
                Else
                    Dim ctlEditor As New Ektron.Cms.Controls.HtmlEditor
                    With ctlEditor
                        .WorkareaMode(2)
                        .ID = "content_html"
                        .Width = New Unit(100, UnitType.Percentage)
                        .Height = New Unit(100, UnitType.Percentage)
                        .Path = AppeWebPath
                        .MaxContentSize = iMaxContLength
                        .Locale = AppeWebPath & "locale" & AppLocaleString & "b.xml"
                        If Len(editorPackage) > 0 Then
                            Dim objField As Ektron.WebEditorNet2.eWebEditProField
                            objField = New Ektron.WebEditorNet2.eWebEditProField
                            With objField
                                .Name = "datadesignpackage"
                                .SetContentType = "datadesignpackage"
                                .GetContentType = ""
                                .Text = editorPackage
                            End With
                            ctlEditor.Fields.Add(objField)

                            objField = New Ektron.WebEditorNet2.eWebEditProField
                            With objField
                                .Name = "datadocumentxml"
                                .SetContentType = "datadocumentxml"
                                .GetContentType = "" ' content is retrieved manually
                                .Text = m_strContentHtml
                            End With
                            ctlEditor.Fields.Add(objField)
                            objField = Nothing
                        Else
                            .Text = m_strContentHtml
                        End If
                    End With
                    Dim eWebEditProWrapper As HtmlControls.HtmlGenericControl = New HtmlControls.HtmlGenericControl("DIV")
                    eWebEditProWrapper.Attributes.Add("class", "ewebeditproWrapper ewebeditpro_dvContent")
                    eWebEditProWrapper.Controls.Add(ctlEditor)
                    m_ctlContentPane.Controls.Add(eWebEditProWrapper)
                    'Dim ctlButton As New HtmlInputButton
                    'With ctlButton
                    '    .ID = "btnOnReady"
                    '    .Value = "Fire onReady for debugging"
                    '    .Attributes.Add("onclick", "initTransferMethod('content_html', 'mediamanager.aspx', 'autoupload.aspx');initTransferMethod('content_teaser', 'mediamanager.aspx', 'autoupload.aspx')")
                    'End With
                    'm_ctlContentPane.Controls.Add(ctlButton)
                End If
            End If
        End If
            ' fix for Defect: #43308, why output this tab if you are jsut going to hide it?  We always hide it for
            '   office docs, so...
            If (isOfficeDoc.Value = "true") Then
                phContent.Visible = False
                phEditContent.Visible = False
            End If
    End Sub
    Private Function EncodeJavascriptString(ByVal str As String) As String
        Dim result As String
        result = str.Replace("'", "\'")
        result = Replace(result, """", "\""")
        Return result
    End Function

    Private Sub EditSummaryHtmlScripts()
        Dim sbHtml As New StringBuilder
        Dim bIsRedirect As Boolean = False
        Dim bIsTransfer As Boolean = False
        Dim editLiveCSS As String = ""
        Dim bIsReport As Boolean = False
        Dim iRptDisplayType As Integer = 1 'default to same window "_self"
        Dim iRptType As Integer = 4 'default to combine bar with percent
        Dim bPureRedirect As Boolean = False
        Dim bIsForm As Boolean = False

        If bIsFormDesign Then
            bIsRedirect = (content_teaser.IndexOf("<RedirectionLink") > -1)
            If bIsRedirect Then
                bIsTransfer = (content_teaser.IndexOf("EktForwardFormData") > -1)
                bIsReport = (content_teaser.IndexOf("EktReportFormData") > -1)
                If bIsReport Then
                    'find out the setting for reports.
                    If (content_teaser.IndexOf(" target") > -1) And (content_teaser.IndexOf("_self") > -1) Then
                        iRptDisplayType = 1
                    Else
                        iRptDisplayType = 0
                    End If
                    Dim iPos As Integer
                    Dim SPos As Integer
                    Dim sHolder As String
                    Dim sRptType As String = ""
                    SPos = content_teaser.IndexOf(" id=""") ' 5 char
                    If SPos > 0 Then
                        For iPos = SPos + 5 To Len(content_teaser)
                            sHolder = Mid(content_teaser, iPos + 1, 1)
                            If sHolder <> """" Then
                                sRptType = sRptType & sHolder
                            Else
                                Exit For
                            End If
                        Next
                        iRptType = Convert.ToInt16(sRptType)
                    End If
                End If
                bPureRedirect = True
                If bIsTransfer Then
                    bPureRedirect = False
                ElseIf bIsReport Then
                    bPureRedirect = False
                End If
            End If
        End If

        If bNewPoll Then
            'default report response for poll.
            bIsRedirect = True
            bIsTransfer = False
            bIsReport = True
        End If

        Dim strPath As String = ""
        If Len(content_stylesheet) > 0 Then
            strPath = GetServerPath() & SitePath & content_stylesheet
            editLiveCSS = "&css=" & content_stylesheet
        End If
        'build head of table if blog
        If m_bIsBlog Then
            sbHtml.Append("<table cellpadding=""4"">")
            sbHtml.Append("	<tr>")
            sbHtml.Append("		<td width=""20"">&nbsp;</td>")
            sbHtml.Append("		<td valign=""top"">")
            sbHtml.Append("			<b>" & m_refMsg.GetMessage("generic description") & "</b>")
            sbHtml.Append("		</td>")
            sbHtml.Append("		<td width=""20"">&nbsp;</td>")
            sbHtml.Append("		<td valign=""top"">&nbsp;")
            sbHtml.Append("		</td>")
            sbHtml.Append("	</tr>")
            sbHtml.Append("	<tr>")
            sbHtml.Append("		<td width=""20"">&nbsp;</td>")
            sbHtml.Append("		<td valign=""top"">")
            'sbHtml.Append("			<textarea cols=""50"" rows=""8""></textarea>")
        End If

        If "ContentDesigner" = m_SelectedEditControl Then
            If (Not (IsNothing(content_edit_data))) Then
                If (content_edit_data.Type = 2) Then
                    bIsForm = True
                End If
            End If
            With m_ctlSummaryDesigner
				.Visible = True
                If m_bIsBlog Then
                    .Width = New Unit(484, UnitType.Pixel)
                    .Height = New Unit(200, UnitType.Pixel)
                Else
                    .Width = New Unit(100, UnitType.Percentage)
					.Height = New Unit(635, UnitType.Pixel)
                End If
                If Len(content_stylesheet) > 0 Then
					.Stylesheet = strPath
                End If
                If (bIsForm = True) Then
                    If content_teaser.IndexOf("<ektdesignpackage_forms>") > -1 Then
                        .Content = m_refContApi.TransformXsltPackage(content_teaser, _
                      Server.MapPath(m_ctlSummaryDesigner.ScriptLocation & "unpackageDesign.xslt"), True)
                        If ("" = .Content) Then
                            .Content = "<p>" & m_refMsg.GetMessageForLanguage("lbl place post back message here", m_intContentLanguage) & "</p>"
                        End If
                    Else 'new form response, no packages
                        .Content = content_teaser
                    End If
                Else
                    .Content = content_teaser
                End If
            End With
            With m_ctlSummaryValidator
                .ValidationExpression = Utilities.BuildRegexToCheckMaxLength(iMaxSummLength)
                .Visible = True
            End With
            If (bIsForm = True) Then
				m_ctlSummaryDesigner.Height = New Unit(450, UnitType.Pixel)
				With m_ctlFormResponseRedirect
					.Visible = True
					.Width = New Unit(100, UnitType.Percentage)
					.Height = New Unit(200, UnitType.Pixel)
					If Len(content_stylesheet) > 0 Then
						.Stylesheet = strPath
					End If
					.DataEntryXslt = GenerateRedirectionPageXslt("Redirect")
					.DataSchema = ""
					.DataDocumentXml = content_teaser
				End With
				With m_ctlFormResponseTransfer
					.Visible = True
					.Width = New Unit(100, UnitType.Percentage)
					.Height = New Unit(200, UnitType.Pixel)
					If Len(content_stylesheet) > 0 Then
						.Stylesheet = strPath
					End If
					.DataEntryXslt = GenerateRedirectionPageXslt("Transfer")
					.DataSchema = ""
					.DataDocumentXml = content_teaser
				End With
				With m_ctlFormSummaryReport
					.Visible = True
				End With
            Else
                With m_ctlFormResponseRedirect
                    .Visible = False
                End With
                With m_ctlFormResponseTransfer
                    .Visible = False
                End With
                With m_ctlFormSummaryReport
                    .Visible = False
                End With
            End If
        End If

		If m_SelectedEditControl = "ContentDesigner" Then
			' Because ContentDesigner is now in a user control, the name and id are different
			sbHtml.Append("<input type=""hidden"" name=""content_teaser"" id=""content_teaser"" value="""">")
		End If
        If IsMac AndAlso m_SelectedEditControl <> "ContentDesigner" Then
            If (Not IsBrowserIE) Then
                sbHtml.Append("<input type=""hidden"" name=""selectedtext"" id=""selectedtext"">")
                sbHtml.Append("<input type=""hidden"" name=""selectedhtml"" id=""selectedhtml"">")
                Dim strJSEditLive As New System.Text.StringBuilder
                sbHtml.Append("<input type=""hidden"" name=""content_teaser"" id=""content_teaser"" value="""">")
                sbHtml.Append("<input type=""hidden"" name=""EphoxTeaser"" id=""EphoxTeaser"" value=""" & Server.UrlEncode(Server.HtmlDecode(content_teaser)) & """>")
                strJSEditLive.Append("<script language=""JavaScript"">" & vbCrLf)
                strJSEditLive.Append("      var strContent;")
                If m_bIsBlog Then
                    strJSEditLive.Append("		elx2 = new EditLiveJava(""content_teaser22"", ""484"", ""200"");")
                Else
                    strJSEditLive.Append("		elx2 = new EditLiveJava(""content_teaser22"", ""700"", ""400"");")
                End If
                'strJSEditLive.Append("		elx2.setXMLURL(""" & Me.AppeWebPath & "editlivejava/config.aspx?apppath=" & Me.AppPath & """);")
                strJSEditLive.Append("		elx2.setXMLURL(""" & Me.AppeWebPath & "editlivejava/config.aspx?apppath=" & Me.AppPath & "&sitepath=" & Me.SitePath & editLiveCSS & """);")
                strJSEditLive.Append("      elx2.setOutputCharset(""UTF-8"");")
                strJSEditLive.Append("		elx2.setBody(document.forms[0].EphoxTeaser.value);")
                strJSEditLive.Append("		elx2.setDownloadDirectory(""" & Me.AppeWebPath & "editlivejava"");")
                strJSEditLive.Append("		elx2.setLocalDeployment(false);")
                strJSEditLive.Append("		elx2.setCookie("""");")
                strJSEditLive.Append("		elx2.show();" & vbCrLf)
                strJSEditLive.Append("	</script>" & vbCrLf)
                sbHtml.Append(strJSEditLive.ToString())
            Else
                sbHtml.Append("<textarea name=""content_teaser"" cols=""90"" rows=""24"" ID=""Textarea3"">" & Server.HtmlEncode(content_teaser) & "</textarea>")
            End If
        Else
            If bIsFormDesign Then
                sbHtml.Append("<p>" & vbCrLf)

                ' Display a message
                sbHtml.Append("<input type=""radio"" id=""response_message"" name=""response"" value=""message""")
                If Not bIsRedirect Then
                    sbHtml.Append(" checked=""checked""")
                    initialSummaryPane.Text = "message"
                End If
                sbHtml.Append(" onclick=""setResponseAction('message')"" disabled /><label id=""lbl_response_message"" for=""response_message"" disabled>&#160;" & m_refMsg.GetMessage("lbl display a message") & "</label><br />" & vbCrLf)

                ' Redirect to a file or page
                sbHtml.Append("<input type=""radio"" id=""response_redirect"" name=""response"" value=""redirect""")
                If bPureRedirect Then
                    sbHtml.Append(" checked=""checked""")
                    initialSummaryPane.Text = "redirect"
                End If
                sbHtml.Append(" onclick=""setResponseAction('redirect')"" disabled /><label id=""lbl_response_redirect"" for=""response_redirect"" disabled>&#160;" & m_refMsg.GetMessage("lbl redirect to a file or page") & "</label><br />" & vbCrLf)

                ' Redirect form data to an action page
                sbHtml.Append("<input type=""radio"" id=""response_transfer"" name=""response"" value=""transfer""")
                If bIsTransfer Then
                    sbHtml.Append(" checked=""checked""")
                    initialSummaryPane.Text = "transfer"
                End If
                sbHtml.Append(" onclick=""setResponseAction('transfer')"" disabled /><label id=""lbl_response_transfer"" for=""response_transfer"" disabled>&#160;" & m_refMsg.GetMessage("lbl redirect form data to an action page") & "</label><br />" & vbCrLf)

                ' Report on the form
                sbHtml.Append("<input type=""radio"" id=""response_report"" name=""response"" value=""report""")
                If bIsReport Then
                    sbHtml.Append(" checked=""checked""")
                    initialSummaryPane.Text = "report"
                End If
                sbHtml.Append(" onclick=""showReportOptions()"" disabled /><label id=""lbl_response_report"" for=""response_report"" disabled>&#160;" & m_refMsg.GetMessage("lbl report on the form") & "</label>" & vbCrLf)


                sbHtml.Append("&nbsp;&nbsp;<SELECT onchange=""setReportOptions('rptDisplayType')"" id=rptDisplayType name=""report_display_type""" & vbCrLf)
                If bIsReport = False Then
                    sbHtml.Append(" disabled")
                End If
                sbHtml.Append(">" & vbCrLf)
                sbHtml.Append("<OPTION value=""1""" & vbCrLf)
                If iRptDisplayType = 1 Then
                    sbHtml.Append(" selected=""selected""")
                End If
                sbHtml.Append(">" & m_refMsg.GetMessage("lbl same window") & "</OPTION>" & vbCrLf)
                sbHtml.Append("<OPTION value=""0""" & vbCrLf)
                If iRptDisplayType = 0 Then
                    sbHtml.Append(" selected=""selected""")
                End If
                sbHtml.Append(">" & m_refMsg.GetMessage("lbl new window") & "</OPTION>" & vbCrLf)
                sbHtml.Append("</SELECT>" & vbCrLf)
                sbHtml.Append("&nbsp;&nbsp;&nbsp;<SELECT name=""report_type"" onchange=""setReportOptions('rptType')"" id=""rptType""" & vbCrLf)
                If bIsReport = False Then
                    sbHtml.Append(" disabled")
                End If
                sbHtml.Append(">" & vbCrLf)
                sbHtml.Append("<OPTION value=""1""" & vbCrLf)
                If iRptType = 1 Then
                    sbHtml.Append(" selected=""selected""")
                End If
                sbHtml.Append(">" & m_refMsg.GetMessage("lbl data table") & "</OPTION>" & vbCrLf)
                sbHtml.Append("<OPTION value=""2""" & vbCrLf)
                If iRptType = 2 Then
                    sbHtml.Append(" selected=""selected""")
                End If
                sbHtml.Append(">" & m_refMsg.GetMessage("lbl bar chart") & "</OPTION>" & vbCrLf)
                sbHtml.Append("<OPTION value=""3""" & vbCrLf)
                If iRptType = 3 Then
                    sbHtml.Append(" selected=""selected""")
                End If
                sbHtml.Append(">" & m_refMsg.GetMessage("lbl pie chart") & "</OPTION>" & vbCrLf)
                sbHtml.Append("<OPTION value=""4""" & vbCrLf)
                If iRptType = 4 Then
                    sbHtml.Append(" selected=""selected""")
                End If
                sbHtml.Append(">" & m_refMsg.GetMessage("lbl combined") & "</OPTION>" & vbCrLf)
                sbHtml.Append("</SELECT>" & vbCrLf)
                sbHtml.Append("</p>" & vbCrLf)
            End If
            If m_SelectedEditControl <> "ContentDesigner" Then
                sbHtml.Append("<script language=""JavaScript1.2"" type=""text/javascript"">" & vbCrLf)
                sbHtml.Append("<!--" & vbCrLf)
                sbHtml.Append("editorEstimateContentSize = false;" & vbCrLf)
                sbHtml.Append("eWebEditPro.parameters.reset();" & vbCrLf)
                sbHtml.Append("eWebEditPro.parameters.baseURL = """ & SitePath & """;" & vbCrLf)
                sbHtml.Append("eWebEditPro.parameters.locale = """ & AppeWebPath & "locale" & AppLocaleString & "b.xml"";" & vbCrLf)
                If bIsFormDesign Then
                    If bIsRedirect Then
                        sbHtml.Append("eWebEditPro.parameters.config = eWebEditProPath + ""cms_config.aspx?mode=dataentry&InterfaceName=none"";" & vbCrLf)
                    Else
                        sbHtml.Append("eWebEditPro.parameters.config = eWebEditProPath + ""cms_config.aspx?mode=xsltdesign"";" & vbCrLf)
                    End If
                    sbHtml.Append("eWebEditPro.parameters.editorGetMethod = ""getDocument"";" & vbCrLf)
                Else
                    sbHtml.Append("eWebEditPro.parameters.config = eWebEditProPath + ""cms_config.aspx?wiki=0"";" & vbCrLf)
                End If
                sbHtml.Append("eWebEditPro.parameters.maxContentSize = " & iMaxSummLength & "; " & vbCrLf)
                sbHtml.Append("eWebEditPro.parameters.xmlInfo ='';" & vbCrLf)
                If Len(content_stylesheet) > 0 Then

                    sbHtml.Append("eWebEditPro.parameters.styleSheet = """ & strPath & """; " & vbCrLf)
                End If
                sbHtml.Append("//-->" & vbCrLf)
                sbHtml.Append("</script>" & vbCrLf)
            End If
            If bIsFormDesign Then
                Dim strRedirectionData As String
                If bIsRedirect Then
                    strRedirectionData = content_teaser
                    content_teaser = "<p>" & m_refMsg.GetMessageForLanguage("lbl place post back message here", m_intContentLanguage) & "</p>"
                Else
                    strRedirectionData = "<root><RedirectionLink></RedirectionLink></root>"
                End If
                If m_SelectedEditControl <> "ContentDesigner" Then
                    ' Store the other dataentryxslt for easy switching
                    'sbHtml.Append(Utilities.eWebEditProField("content_teaser", "transfer_page", IIf(bIsTransfer, "dataentryxslt", ""), "", GenerateRedirectionPageXslt(IsTransfer:=True)))
                    'sbHtml.Append(Utilities.eWebEditProField("content_teaser", "redirection_page", IIf(bIsTransfer, "", "dataentryxslt"), "", GenerateRedirectionPageXslt(IsTransfer:=False)))
                    'sbHtml.Append(Utilities.eWebEditProField("content_teaser", "redirection_data", "datadocumentxml", "datadocumentxml", strRedirectionData))
                    sbHtml.Append(Utilities.eWebEditProField("content_teaser", "transfer_page", IIf(bIsTransfer, "dataentryxslt", ""), "", GenerateRedirectionPageXslt("Transfer")))
                    sbHtml.Append(Utilities.eWebEditProField("content_teaser", "redirection_page", IIf(bPureRedirect, "dataentryxslt", ""), "", GenerateRedirectionPageXslt("Redirect")))
                    sbHtml.Append(Utilities.eWebEditProField("content_teaser", "report_page", IIf(bIsReport, "dataentryxslt", ""), "", GenerateRedirectionPageXslt("Report")))
                    sbHtml.Append(Utilities.eWebEditProField("content_teaser", "redirection_data", "datadocumentxml", IIf(bIsReport, "", "datadocumentxml"), strRedirectionData))
                End If
                If bIsRedirect Then
                    ' NOTE
                    ' If redirecting, then change the field name after loaded so the content will be saved to content_teaser.
                    sbHtml.Append("<script language=""JavaScript1.2"" type=""text/javascript"">" & vbCrLf)
                    sbHtml.Append("<!--" & vbCrLf)
                    sbHtml.Append("g_prevResponseAction = """ & IIf(bIsTransfer, "transfer", IIf(bIsReport, "report", "redirect")) & """" & vbCrLf)
                    If m_SelectedEditControl <> "ContentDesigner" Then
                        sbHtml.Append("eWebEditPro.addEventHandler(""onload"", 'changeFieldName(""redirection_data"", ""content_teaser"")');" & vbCrLf)
                    End If
                    sbHtml.Append("// -->" & vbCrLf)
                    sbHtml.Append("</script>" & vbCrLf)
                Else
                    ' NOTE
                    ' If not redirecting, then mask the fact that these two fields are defined.
                    sbHtml.Append("<script language=""JavaScript1.2"" type=""text/javascript"">" & vbCrLf)
                    sbHtml.Append("<!--" & vbCrLf)
                    sbHtml.Append("g_prevResponseAction = ""message""" & vbCrLf)
                    If m_SelectedEditControl <> "ContentDesigner" Then
						sbHtml.Append("Ektron.ready(function()  {changeEditorNameOfFields(""content_teaser"", ""not_redirect"");});" & vbCrLf)
                    End If
                    sbHtml.Append("// -->" & vbCrLf)
                    sbHtml.Append("</script>" & vbCrLf)
                End If
            End If

            If m_SelectedEditControl <> "ContentDesigner" Then
                Dim EditorWidth As String
                Dim EditorHeight As String
                If m_bIsBlog Then
                    EditorWidth = "484"
                    EditorHeight = "200"
                Else
                    EditorWidth = "100%"
                    EditorHeight = "100%"
                End If
                sbHtml.Append("<div class=""ewebeditproWrapper ewebeditpro_dvSummary"">")
                sbHtml.Append(Utilities.eWebEditProEditor("content_teaser", EditorWidth, EditorHeight, content_teaser))
                sbHtml.Append("</div>")
            End If
        End If

        'for debugging in eWebEditPro IDE, ***** need to update @x@ to a value *****.
        'sbHtml.Append("<input type=""button"" name=""btnProp"" value=""Property Dlg"" onclick=""CallJsFieldList('content_teaser', 'jsfieldlist', '', @x@)"" />")

        Dim litSnippet As New Literal
        With litSnippet
            .ID = "TeaserHtml"
            .Text = sbHtml.ToString()
        End With
        m_ctlSummaryPane.Controls.AddAt(0, litSnippet) ' place above the ContentDesigner editor
        If m_bIsBlog Then
            Dim litBlogSnippet As New Literal 'if litSnippet is re-used here, the page layout is messed up.
            sbHtml.Length = 0
            AddBlogItems(sbHtml)
            litBlogSnippet.Text = sbHtml.ToString()
            m_ctlSummaryPane.Controls.Add(litBlogSnippet) ' place below the ContentDesigner editor
        End If
    End Sub

    Private Sub AddBlogItems(ByRef sbHtml As System.Text.StringBuilder)
        Dim arrBlogPostCategories As String()
        Dim i As Integer = 0

        sbHtml.Append("			<br/><br/>")
        sbHtml.Append("			<b>" & m_refMsg.GetMessage("lbl tags") & "</b> (" & m_refMsg.GetMessage("lbl enter multiple tags") & ")")
        sbHtml.Append("			<br/>")
        If Not (blog_post_data Is Nothing) Then
            sbHtml.Append("			<textarea cols=""58"" rows=""5"" id=""blogposttags"" name=""blogposttags"">" & blog_post_data.Tags & "</textarea><input type=""hidden"" name=""blogposttagsid"" id=""blogposttagsid"" value=""" & blog_post_data.TagsID & """ />")
        Else
            sbHtml.Append("			<textarea cols=""58"" rows=""5"" id=""blogposttags"" name=""blogposttags""></textarea>")
        End If
        sbHtml.Append("	<p>")
        If Not (blog_data.Categories Is Nothing) AndAlso blog_data.Categories.Length > 0 Then
            sbHtml.Append("			<br><br><b>" & m_refMsg.GetMessage("lbl blog cat") & "</b><br/>")

            If Not (blog_post_data Is Nothing) AndAlso Not (blog_post_data.Categories Is Nothing) Then
                arrBlogPostCategories = blog_post_data.Categories
                If arrBlogPostCategories.Length > 0 Then
                    Array.Sort(arrBlogPostCategories)
                End If
            Else
                arrBlogPostCategories = Nothing
            End If
            If blog_data.Categories.Length > 0 Then
                For i = 0 To (blog_data.Categories.Length - 1)
                    If blog_data.Categories(i).ToString() <> "" Then
                        If Not (arrBlogPostCategories Is Nothing) AndAlso Array.BinarySearch(arrBlogPostCategories, blog_data.Categories(i).ToString()) > -1 Then
                            sbHtml.Append("				<input type=""checkbox"" name=""blogcategories" & i.ToString() & """ value=""" & Replace(blog_data.Categories(i).ToString(), "~@~@~", ";") & """ checked=""true"">&nbsp;" & Replace(blog_data.Categories(i).ToString(), "~@~@~", ";") & "<br />")
                        Else
                            sbHtml.Append("				<input type=""checkbox"" name=""blogcategories" & i.ToString() & """ value=""" & Replace(blog_data.Categories(i).ToString(), "~@~@~", ";") & """>&nbsp;" & Replace(blog_data.Categories(i).ToString(), "~@~@~", ";") & "<br />")
                        End If
                    End If
                Next
                sbHtml.Append("<input type=""hidden"" name=""blogpostcatlen"" id=""blogpostcatlen"" value=""" & blog_data.Categories.Length.ToString() & """/>")
            Else
                sbHtml.Append("No categories defined.<input type=""hidden"" name=""blogpostcatlen"" id=""blogpostcatlen"" value=""0""/>")
            End If
        End If
        sbHtml.Append("<input type=""hidden"" name=""blogpostcatid"" id=""blogpostcatid"" value=""" & blog_post_data.CategoryID.ToString() & """ />")
        sbHtml.Append("		</td>")

        sbHtml.Append("		</td>")
        sbHtml.Append("		<td width=""20"">&nbsp;</td>")
        sbHtml.Append("		<td valign=""top"" style=""border: 1px solid #fffff; "">")
        sbHtml.Append("	</tr>")

        sbHtml.Append("	<tr>")
        sbHtml.Append("		<td width=""20"">&nbsp;</td>")
        sbHtml.Append("		<td valign=""top"" colspan=""2"">")
        sbHtml.Append("     ").Append(GetBlogControls())
        sbHtml.Append("		</td>")
        sbHtml.Append("	</tr>")

        sbHtml.Append("</table>")
    End Sub

    Private Sub PollHtmlScript()
        Dim sbHtml As New StringBuilder
        Dim idx As Integer
        sbHtml.Append("<input type=""hidden"" name=""numPollChoices"" id=""numPollChoices"" value=""" & nPollChoices & """ />" & vbCrLf)
        sbHtml.Append("<div id=""_dvPollWizard"" style=""position: absolute; top: 90px;"">" & vbCrLf)
        sbHtml.Append("<table width=""100%"" cellspacing=""0"" cellpadding=""5"">" & vbCrLf)
        sbHtml.Append("<tr>" & vbCrLf)
        sbHtml.Append("	<td></td>" & vbCrLf)
        sbHtml.Append("	<td colspan=""2"">Question:</td>" & vbCrLf)
        sbHtml.Append("</tr>" & vbCrLf)
        sbHtml.Append("<tr>" & vbCrLf)
        sbHtml.Append("	<td colspan=""2""></td>" & vbCrLf)
        sbHtml.Append("	<td><input name=""frm_Question"" id=""frm_Question"" type=""text"" runat=""server"" style=""width: 717px"" maxlength=""1000"" /></td>" & vbCrLf)
        sbHtml.Append("</tr>" & vbCrLf)
        sbHtml.Append("<tr>" & vbCrLf)
        sbHtml.Append("	<td></td>" & vbCrLf)
        sbHtml.Append("	<td colspan=""2"">Choices:</td>" & vbCrLf)
        sbHtml.Append("</tr>" & vbCrLf)
        For idx = 1 To nPollChoices
            sbHtml.Append("<tr>" & vbCrLf)
            sbHtml.Append("	<td></td>" & vbCrLf)
            sbHtml.Append("	<td>" & idx & ".</td>" & vbCrLf)
            sbHtml.Append("	<td><input name=""frm_Choice" & idx & """ id=""frm_Choice" & idx & """ type=""text"" runat=""server"" maxlength=""50"" /></td>" & vbCrLf)
            sbHtml.Append("</tr>" & vbCrLf)
        Next
        sbHtml.Append("</table>" & vbCrLf)
        sbHtml.Append("</div>")
        sbHtml.Append("<input type=""hidden"" name=""renewpoll"" value=""" & bReNewPoll & """ />")
        PollPaneHtml.Text = sbHtml.ToString()
    End Sub
    Private Function GenerateRedirectionPageXslt(ByVal TransferType As String) As String
        Dim sbRedirectionPage As New StringBuilder
        ' TODO localize these strings
        Dim strCaption As String = "File or page"
        Dim strCannotBeBlank As String = "Cannot be blank"
        Dim strTransferable As String = "To redirect and forward form data to another page, " _
          & "the following requirements must be met:" _
          & "\n  1. The page must be an .aspx page." _
          & "\n  2. The page must be within the same web application." ' no single or double quotes. Also, no < or > or & (unless HTML encoded)
        Dim strSelect As String = "Select"
        sbRedirectionPage.Append("<xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">" & vbCrLf)
        sbRedirectionPage.Append("<xsl:output method=""xml"" version=""1.0"" encoding=""UTF-8"" indent=""yes"" omit-xml-declaration=""yes"" />" & vbCrLf)
        sbRedirectionPage.Append("<xsl:strip-space elements=""*"" />" & vbCrLf)
        sbRedirectionPage.Append("<xsl:template match=""/"" xml:space=""preserve"">" & vbCrLf)
        If TransferType <> "Report" Then
            sbRedirectionPage.Append("<div style=""padding:3px; color:black; background-color:white; font-family:Verdana, Geneva, Arial, Helvetica, sans-serif; font-size:x-small;"">" & vbCrLf)
            If (TransferType = "Transfer") Then
                sbRedirectionPage.Append("<input type=""hidden"" name=""EktForwardFormData"" value="""" ektdesignns_name=""EktForwardFormData"" ektdesignns_nodetype=""element"" />")
            End If
            'If TransferType = "Report" Then
            '	sbRedirectionPage.Append("<input type=""hidden"" name=""EktReportFormData"" value="""" ektdesignns_name=""EktReportFormData"" ektdesignns_nodetype=""element"" />")
            'End If
            sbRedirectionPage.Append("<label for=""RedirectionLink"">" & strCaption & ":</label>" & vbCrLf)
            sbRedirectionPage.Append("<span class=""design_filelink"" ektdesignns_content=""element=a"" id=""RedirectionLink"" ektdesignns_name=""RedirectionLink"" title=""" & strCaption & """ ektdesignns_nodetype=""element""")
            If (TransferType = "Transfer") Then
                Dim strDomain As String
                Dim strSitePath As String
                strDomain = m_refContApi.FullyQualifyURL("/")
				strDomain = Ektron.Cms.API.JS.EscapeRegExp(strDomain)
                strSitePath = m_refContApi.SitePath
                If (strSitePath.StartsWith("/")) Then
                    strSitePath = strSitePath.Substring(1) ' strip leading "/"
                End If
				strSitePath = Ektron.Cms.API.JS.EscapeRegExp(strSitePath)
                ' content-req is special for design.js validation
                sbRedirectionPage.Append(" ektdesignns_validation=""content-req"" onblur=""design_validate_re(/&lt;A.*href\s*=\s*[\x22\x27\s](" & strDomain & ")?\/?" & strSitePath & ".*\.aspx[\?\x22\x27\s]/i,this,'" & strTransferable & "');"">" & vbCrLf)
            Else
                sbRedirectionPage.Append(" ektdesignns_validation=""content-req"" onblur=""design_validate_re(/&lt;A/i,this,'" & strCannotBeBlank & "');"">" & vbCrLf)
            End If
            sbRedirectionPage.Append("<xsl:copy-of select=""/root/RedirectionLink/node()"" />&#160;<img class=""design_fieldbutton"" height=""16"" alt=""" & strSelect & """ src=""[srcpath]btnfilelink.gif"" width=""16"" unselectable=""on"" />" & vbCrLf)
            sbRedirectionPage.Append("</span> &#160;" & vbCrLf)
            sbRedirectionPage.Append("</div>" & vbCrLf)
        End If
        sbRedirectionPage.Append("</xsl:template>" & vbCrLf)
        sbRedirectionPage.Append("</xsl:stylesheet>" & vbCrLf)
        Return sbRedirectionPage.ToString
    End Function

    Public ReadOnly Property IsSiteMultilingual() As Boolean
        Get
            Dim languageDataArray() As LanguageData = m_refSiteApi.GetAllActiveLanguages()
            Dim m_refUserApi As UserAPI = New UserAPI()
            If (m_refUserApi.EnableMultilingual = 0) Then
                Return False
            End If
            Dim languageEnabledCount As Integer = 0
            For Each lang As LanguageData In languageDataArray
                If lang.SiteEnabled Then languageEnabledCount += 1
                If languageEnabledCount > 1 Then Exit For
            Next

            Return languageEnabledCount > 1
        End Get

    End Property

    Private Function GetLanguageDropDownMarkup(ByVal controlId As String) As String

        Dim i As Integer
        Dim markup As New StringBuilder
        Dim m_refContentApi As New ContentAPI

        If (IsSiteMultilingual) Then
            markup.Append("<select id=""" & controlId & """ name=""" & controlId & """>")
            Dim languageDataArray() As LanguageData = m_refSiteApi.GetAllActiveLanguages()
            If (Not (IsNothing(languageDataArray))) Then
                For i = 0 To languageDataArray.Length - 1
                    If (languageDataArray(i).SiteEnabled) Then
                        markup.Append("<option ")
                        If (m_intContentLanguage = languageDataArray(i).Id) Then    'm_refContentApi.DefaultContentLanguage
                            markup.Append(" selected")
                        End If
                        markup.Append(" value=" & languageDataArray(i).Id & ">" & languageDataArray(i).LocalName)
                    End If
                Next
            End If
            markup.Append("</select>")
        Else
            'hardcode to default site language
            markup.Append("<select id=""" & controlId & """ name=""" & controlId & """ selectedindex=""0"" >")
            markup.Append(" <option selected value=" & m_intContentLanguage & ">")  ' m_refContentApi.DefaultContentLanguage
            markup.Append("</select>")
        End If

        Return markup.ToString()
    End Function

    Private Function GetTagDisplayHTML(ByVal contentId As Long) As String
        ' add content Tags section
        ' display tag edit area
        Dim taghtml As New System.Text.StringBuilder
        error_TagsCantBeBlank.Text = m_refMsg.GetMessage("msg error Blank Tag")
        error_InvalidChars.Text = m_refMsg.GetMessage("msg error Tag invalid chars")

        Dim htTagsAssignedToUser As New Hashtable
        taghtml.Append("<div class=""ektronTopSpace""></div>")

        taghtml.Append("<div style=""height:115px;"">")
        taghtml.Append("<div id=""newTagNameDiv"" class=""ektronWindow"">")
        taghtml.Append("    <table class=""ektronForm"">")
        taghtml.Append("        <tr>")
        taghtml.Append("            <td class=""label"">")
        taghtml.Append(m_refMsg.GetMessage("name label"))
        taghtml.Append("            </td>")
        taghtml.Append("            <td class=""value"">")
        taghtml.Append("                <input type=""text"" id=""newTagName"" value="""" style=""width:275px;"" size=""25"" onkeypress=""if (event && event.keyCode && (13 == event.keyCode)) {SaveNewPersonalTag(); return false;}"" />")
        taghtml.Append("            </td>")
        taghtml.Append("        </tr>")
        taghtml.Append("    </table>")

        If (IsSiteMultilingual) Then
            taghtml.Append("<div style=""display:none;"" >")
        Else
            taghtml.Append("<div style=""display:none;"" >")
        End If
        taghtml.Append(m_refMsg.GetMessage("res_lngsel_lbl") + "&#160;" & GetLanguageDropDownMarkup("TagLanguage"))
        taghtml.Append("    </div>")

        taghtml.Append("<div style=""margin-top:.5em;"">")
        taghtml.Append("    <ul class=""buttonWrapper ui-helper-clearfix"">")
        taghtml.Append("        <li>")
        taghtml.Append("            <a style='margin-right: 14px;' class=""button redHover buttonClear buttonRight"" type=""button"" alt=""" + m_refMsg.GetMessage("btn cancel") + """ title=""" + m_refMsg.GetMessage("btn cancel") + """ onclick=""CancelSaveNewPersonalTag();"">")
        taghtml.Append("                <span>" & m_refMsg.GetMessage("btn cancel") & "</span>")
        taghtml.Append("            </a>")
        taghtml.Append("        </li>")

        taghtml.Append("        <li>")
        taghtml.Append("            <a class=""button greenHover buttonUpdate buttonRight"" type=""button"" title=""" + m_refMsg.GetMessage("btn save") + """ alt=""" + m_refMsg.GetMessage("btn save") + """ onclick=""SaveNewPersonalTag();"">")
        taghtml.Append("                <span>" & m_refMsg.GetMessage("btn save") & "</span>")
        taghtml.Append("            </a>")
        taghtml.Append("        </li>")
        taghtml.Append("    </ul>")
        taghtml.Append("</div>")
        taghtml.Append("</div>")
        taghtml.Append("<input type=""hidden"" id=""newTagNameHdn"" name=""newTagNameHdn"" value=""""  />")
        taghtml.Append("<fieldset style=""margin: 10px;"">")
        taghtml.Append("    <legend>")
        taghtml.Append("        <span class=""label"">" + m_refMsg.GetMessage("lbl personal tags") + "</span>")
        taghtml.Append("    </legend>")
        taghtml.Append("    <div id=""newTagNameScrollingDiv"">")

        Dim localizationApi As New LocalizationAPI()

        'create hidden list of current tags so we know to delete removed ones.
        Dim languageDataArray() As LanguageData = m_refSiteApi.GetAllActiveLanguages()
        Dim lang As LanguageData
        For Each lang In languageDataArray
            taghtml.Append("<input type=""hidden"" id=""flag_" & lang.Id & """  value=""" + localizationApi.GetFlagUrlByLanguageID(lang.Id) + """  />")
        Next
        taghtml.Append("<input type=""hidden"" id=""flag_0""  value=""" + localizationApi.GetFlagUrlByLanguageID(-1) + """  />")

        Dim tdaUser() As TagData = Nothing
        If (contentId > 0) Then
            tdaUser = (New Community.TagsAPI).GetTagsForObject(contentId, EkEnumeration.CMSObjectTypes.Content, m_refContApi.ContentLanguage)
        End If
        Dim appliedTagIds As New StringBuilder

        'build up a list of tags used by user
        'add tags to hashtable for reference later when looping through defualt tag list
        Dim td As TagData
        If (Not IsNothing(tdaUser)) Then
            For Each td In tdaUser
                htTagsAssignedToUser.Add(td.Id, td)
                appliedTagIds.Append(td.Id.ToString() + ",")

                taghtml.Append("<input checked=""checked"" type=""checkbox"" id=""userPTagsCbx_" + td.Id.ToString + """ name=""userPTagsCbx_" + td.Id.ToString + """ />&#160;")
                taghtml.Append("<img src='" & localizationApi.GetFlagUrlByLanguageID(td.LanguageId) & "' />")
                taghtml.Append("&#160;" + td.Text + "<br />")
            Next
        End If

        'create hidden list of current tags so we know to delete removed ones.
        taghtml.Append("<input type=""hidden"" id=""currentTags"" name=""currentTags"" value=""" + appliedTagIds.ToString() + """  />")

        Dim tdaAll() As TagData
        tdaAll = (New Community.TagsAPI).GetDefaultTags(EkEnumeration.CMSObjectTypes.Content, m_refContApi.ContentLanguage)
        If (Not IsNothing(tdaAll)) Then
            For Each td In tdaAll
                'don't add to list if its already been added with user's tags above
                If (Not htTagsAssignedToUser.ContainsKey(td.Id)) Then
                    taghtml.Append("<input type=""checkbox"" id=""userPTagsCbx_" + td.Id.ToString + """ name=""userPTagsCbx_" + td.Id.ToString + """ />&#160;")
                    taghtml.Append("<img src='" & localizationApi.GetFlagUrlByLanguageID(td.LanguageId) & "' />")
                    taghtml.Append("&#160;" + td.Text + "<br />")
                End If
            Next
        End If
        taghtml.Append("<div id=""newAddedTagNamesDiv""></div>")

        taghtml.Append("</div>")

        taghtml.Append("<div style=""float:left;"">")
        taghtml.Append("    <a class=""button buttonLeft greenHover buttonAddTagWithText"" href=""#"" onclick=""ShowAddPersonalTagArea();"">" & m_refMsg.GetMessage("btn add personal tag") & "</a>" & vbCrLf)
        taghtml.Append("</div>")
        taghtml.Append("</fieldset>")


        taghtml.Append("</div>")

        GetTagDisplayHTML = taghtml.ToString()
    End Function

    Private Sub EditMetadataHtmlScripts()
        Dim sbHtml As New StringBuilder
        Dim lValidCounter As Integer = 0
        Dim sbResult As New StringBuilder
        Dim strResult As String
        Dim strImage As String = ""
        Dim fldr_data As New FolderData
        Dim contentId As New Long
        Dim contData As New ContentData

        If Request.QueryString("type") = "add" Then
            If Request.QueryString("id") IsNot Nothing And Request.QueryString("id") <> "" Then
                fldr_data = Me.m_refContApi.GetFolderById(Request.QueryString("id"))
            End If
        End If

        If Request.QueryString("type") = "update" Then
            If Request.QueryString("id") IsNot Nothing And Request.QueryString("id") <> "" Then
                contentId = Convert.ToInt64(Request.QueryString("id"))
                contData = Me.m_refContApi.GetContentById(contentId)
                If (contData IsNot Nothing) Then
                    fldr_data = Me.m_refContApi.GetFolderById(contData.FolderId)
                End If
            End If
        End If

        sbHtml.Append("<div id=""dvMetadata"">")
        If (Not meta_data Is Nothing) AndAlso (meta_data.Length > 0) Then
            sbResult = CustomFields.WriteFilteredMetadataForEdit(meta_data, False, m_strType, m_intContentFolder, lValidCounter, m_refSite.GetPermissions(m_intContentFolder, 0, "folder"))
        End If

        ' add Tag section
        sbResult.Append(GetTagDisplayHTML(contentId))

        If (m_strType = "update") Then
            strImage = content_edit_data.Image
            Dim strThumbnailPath As String = content_edit_data.ImageThumbnail
            If (content_edit_data.ImageThumbnail = "") Then
                strThumbnailPath = m_refContApi.AppImgPath & "spacer.gif"
            ElseIf (((fldr_data.IsDomainFolder OrElse fldr_data.DomainProduction <> "") And (strThumbnailPath.IndexOf("http://") <> -1 OrElse strThumbnailPath.IndexOf("https://") <> -1)) OrElse strThumbnailPath.IndexOf("http://") <> -1 OrElse strThumbnailPath.IndexOf("https://") <> -1) Then
                strThumbnailPath = strThumbnailPath
            Else
                strThumbnailPath = m_refContApi.SitePath & strThumbnailPath
            End If
            If (System.IO.Path.GetExtension(strThumbnailPath).ToLower().IndexOf(".gif") <> -1 AndAlso strThumbnailPath.ToLower().IndexOf("spacer.gif") = -1) Then
                strThumbnailPath = strThumbnailPath.Replace(".gif", ".png")
            End If
            sbResult.Append("<fieldset style=""margin-top:3em; margin-left:10px; margin-right:10px;"">")
            sbResult.Append("   <legend>")
			sbResult.Append("       <span class=""label"">" & m_refMsg.GetMessage("lbl image data") & "</span>")
            sbResult.Append("   </legend>")
            sbResult.Append("<div class=""ektronTopSpaceSmall""></div>")
            sbResult.Append("<ul class=""ui-helper-clearfix"">")
            sbResult.Append("   <li class=""inline"">")
            sbResult.Append("       <label class=""ektronHeader"">" & m_refMsg.GetMessage("lbl group image") & ":</label>")
            sbResult.Append("   </li>")
            sbResult.Append("   <li class=""inline"">")
            sbResult.Append("       <span id=""sitepath"">" & Me.m_refContApi.SitePath & "</span>")
            sbResult.Append("       <input type=""textbox"" size=""50"" readonly=""true"" id=""content_image"" name=""content_image"" value=""" & strImage & """ />")
            sbResult.Append("   </li>")
            sbResult.Append("   <li class=""inline"">")
            sbResult.Append("       <a class=""button buttonEdit greenHover inlineBlock"" href=""#"" onclick=""PopUpWindow('mediamanager.aspx?scope=images&upload=true&retfield=content_image&showthumb=false&autonav=" & folder_data.Id & "', 'Meadiamanager', 790, 580, 1,1);return false;"">" & m_refMsg.GetMessage("generic edit title") & "</a>")
            sbResult.Append("   </li>")
            sbResult.Append("   <li class=""inline"">")
            sbResult.Append("       <a class=""button buttonRemove redHover inlineBlock"" href=""#"" onclick=""RemoveContentImage('" & m_refContApi.AppImgPath & "spacer.gif');return false"">" & m_refMsg.GetMessage("btn remove") & "</a>")
            sbResult.Append("   </li>")
            sbResult.Append("</ul>")
            sbResult.Append("<div class=""ektronTopSpace""></div>")
            sbResult.Append("<img id=""content_image_thumb"" src=""" & strThumbnailPath & """ />")
            sbResult.Append("</fieldset>")
        Else
            sbResult.Append("<fieldset style=""margin-top:3em; margin-left:10px; margin-right:10px;"">")
            sbResult.Append("   <legend>")
            sbResult.Append("       <span class=""label"">" & m_refMsg.GetMessage("lbl image data") & "</label>")
            sbResult.Append("   </legend>")
            sbResult.Append("<div class=""ektronTopSpaceSmall""></div>")
            sbResult.Append("<ul class=""ui-helper-clearfix"">")
            sbResult.Append("   <li class=""inline"">")
            sbResult.Append("       <label class=""ektronHeader"">" & m_refMsg.GetMessage("lbl group image") & ":</label>")
            sbResult.Append("   </li>")
            sbResult.Append("   <li class=""inline"">")
            sbResult.Append("       <span id=""sitepath"">" & Me.m_refContApi.SitePath & "</span>")
            sbResult.Append("       <input type=""textbox"" size=""50"" readonly=""true"" id=""content_image"" name=""content_image"" value=""" & strImage & """ />")
            sbResult.Append("   </li>")
            sbResult.Append("   <li class=""inline"">")
            sbResult.Append("       <a class=""button buttonEdit greenHover inlineBlock"" href=""#"" onclick=""PopUpWindow('mediamanager.aspx?scope=images&upload=true&retfield=content_image&showthumb=false&autonav=" & folder_data.Id & "', 'Meadiamanager', 790, 580, 1,1);return false;"">" & m_refMsg.GetMessage("generic edit title") & "</a>")
            sbResult.Append("   </li>")
            sbResult.Append("   <li class=""inline"">")
            sbResult.Append("       <a class=""button buttonRemove redHover inlineBlock"" href=""#"" onclick=""RemoveContentImage('" & m_refContApi.AppImgPath & "spacer.gif');return false"">" & m_refMsg.GetMessage("btn remove") & "</a>")
            sbResult.Append("   </li>")
            sbResult.Append("</ul>")
            sbResult.Append("<div class=""ektronTopSpace""></div>")
            sbResult.Append("<img id=""content_image_thumb"" src=""" & m_refContApi.AppImgPath & "spacer.gif"" />")
            sbResult.Append("</fieldset>")
        End If

        strResult = sbResult.ToString.Trim()
        If strResult <> "" Then
            sbHtml.Append(strResult)
        End If
        sbHtml.Append("</div>")
        'frm_validcounter.Value = lValidCounter
        jsValidCounter.Text = lValidCounter
        EditMetadataHtml.Text = sbHtml.ToString()
    End Sub
    Private Sub EditAliasHtmlScripts()
        Dim sbHtml As New StringBuilder
        Dim m_aliaslist As New UrlAliasing.UrlAliasManualApi
        Dim m_autoaliasApi As New UrlAliasing.UrlAliasAutoApi
        Dim ext_alias As System.Collections.Generic.List(Of String)
        Dim ext As String = ""
        Dim i As Integer

        Dim d_alias As New Ektron.Cms.Common.UrlAliasManualData(0, 0, String.Empty, String.Empty)
        Dim auto_aliaslist As New System.Collections.Generic.List(Of UrlAliasAutoData)

        Dim IsStagingServer As Boolean

        IsStagingServer = m_refContApi.RequestInformationRef.IsStaging

        ext_alias = m_aliaslist.GetFileExtensions()
        If (Not IsNothing(content_edit_data)) Then
            d_alias = m_aliaslist.GetDefaultAlias(content_edit_data.Id)
        End If
        m_strManualAliasExt = d_alias.AliasName
        m_strManualAlias = d_alias.FileExtension

        sbHtml.Append("<div id=""dvAlias"">")
        If (m_urlAliasSettings.IsManualAliasEnabled) Then
            If m_refContApi.IsARoleMember(Common.EkEnumeration.CmsRoleIds.EditAlias) Then
                sbHtml.Append("<input type=""hidden"" name=""frm_manalias_id"" value=""" & d_alias.AliasId & """>")
                sbHtml.Append("<input type=""hidden"" name=""prev_frm_manalias_name"" value=""" & d_alias.AliasName & """>")
                sbHtml.Append("<input type=""hidden"" name=""prev_frm_manalias_ext"" value=""" & d_alias.FileExtension & """>")
                sbHtml.Append("<div class=""ektronHeader"">" & m_refMsg.GetMessage("lbl tree url manual aliasing") & "</div>")
                sbHtml.Append("<table class=""ektronForm"">")
                sbHtml.Append("<tr>")
                sbHtml.Append("<td class=""label"">")
                sbHtml.Append(m_refMsg.GetMessage("lbl primary") & " " & m_refMsg.GetMessage("lbl alias name") & ":")
                sbHtml.Append("</td>")
                sbHtml.Append("<td class=""value"">")

                If (IsStagingServer And folder_data.DomainStaging <> String.Empty) Then
                    sbHtml.Append("<td width=""95%"">http://" & folder_data.DomainStaging & "/<input type=""text""  size=""35"" name=""frm_manalias"" value=""" & d_alias.AliasName & """>")
                ElseIf (folder_data.IsDomainFolder) Then
                    sbHtml.Append("http://" & folder_data.DomainProduction & "/<input type=""text""  size=""35"" name=""frm_manalias"" value=""" & d_alias.AliasName & """>")
                Else
                    sbHtml.Append(SitePath & "<input type=""text""  size=""35"" name=""frm_manalias"" value=""" & d_alias.AliasName & """>")
                End If
                For i = 0 To ext_alias.Count - 1
                    If (ext <> "") Then
                        ext = ext & ","
                    End If
                    ext = ext & ext_alias(i)
                Next
                sbHtml.Append(m_refContApi.RenderHTML_RedirExtensionDD("frm_ManAliasExt", d_alias.FileExtension, ext))
                sbHtml.Append("</td>")
                sbHtml.Append("</tr>")
                sbHtml.Append("</table>")
                If (InStr(m_refContApi.RedirectorManExt, ",") <= 0) Then
                    ast_frm_manaliasExt.Value = m_refContApi.RedirectorManExt
                End If
                'Else
                '    sbHtml.Append("<input type=""hidden"" name=""frm_manalias_id"" value=""" & d_alias.AliasId & """>")
                '    sbHtml.Append("<input type=""hidden"" name=""frm_manalias"" value=""" & d_alias.AliasName & d_alias.FileExtension & """>")
            End If
        End If
        If (m_urlAliasSettings.IsAutoAliasEnabled) Then
            If (Not IsNothing(content_edit_data)) Then
                auto_aliaslist = m_autoaliasApi.GetListForContent(content_edit_data.Id)
            End If
            sbHtml.Append("<div class=""ektronHeader"">" & m_refMsg.GetMessage("lbl automatic") & "</div>")
            sbHtml.Append("<div class=""ektronBorder"" style=""width: auto; height: auto; overflow: auto;"" id=""autoAliasList"">")
            sbHtml.Append("<table width=""100%"">")
            sbHtml.Append("<tr class=""title-header"">")
            sbHtml.Append("<th>")
            sbHtml.Append(m_refMsg.GetMessage("generic type"))
            sbHtml.Append("</th>")
            sbHtml.Append("<th>")
            sbHtml.Append(m_refMsg.GetMessage("lbl alias name"))
            sbHtml.Append("</th>")
            For i = 0 To auto_aliaslist.Count() - 1
                sbHtml.Append("<tr>")
                sbHtml.Append("<td>" & auto_aliaslist(i).AutoAliasType.ToString() & "</td>")
                sbHtml.Append("<td>" & auto_aliaslist(i).AliasName & "</td>")
                sbHtml.Append("</tr>")
            Next
            sbHtml.Append("</table>")
            sbHtml.Append("</div>")
        End If
        sbHtml.Append("</div>")
        EditAliasHtml.Text = sbHtml.ToString()
    End Sub

    Private Sub EditCommentHtmlScripts()
        Dim sbHtml As New StringBuilder
        sbHtml.Append("<div id=""dvComment"">")
        sbHtml.Append("<table class=""ektronForm"">")
        sbHtml.Append("<tr>")
        sbHtml.Append("<td class=""label"">" & m_refMsg.GetMessage("generic comment label") & "</td>")
        sbHtml.Append("<td class=""value""><textarea OnKeyPress=""return CheckKeyValue(event, '34');"" onkeydown=""textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 255);"" onkeyup=""textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 255);"" onMouseOut=""textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 255);"" name=""content_comment"" rows=""8"" cols=""50"">" & content_comment & "</textarea><br />")
        sbHtml.Append("<input type=""hidden"" name=""remainLen"" size=""3"" maxlength=""3"" value=""255"">")
        sbHtml.Append("<script language=""javascript"">textCounter(document.forms[0].content_comment, document.forms[0].remainLen, 255)</script>")
        sbHtml.Append("</td>")
        sbHtml.Append("</tr>")
        sbHtml.Append("</table>")
        sbHtml.Append("</div>")
        EditCommentHtml.Text = sbHtml.ToString()
    End Sub

    Private Sub EditSubscriptionHtmlScripts()
        Dim sbHtml As New StringBuilder
        Dim arrSubscribed As Array = Nothing
        Dim findindex As Integer
        Dim i As Integer = 0
        Dim strEnabled As String = ""
        Dim strNotifyA As String = ""
        Dim strNotifyI As String = ""
        Dim strNotifyN As String = ""
        Dim strNotifySend As String = ""
        Dim strNotifySuspend As String = ""
        Dim strNotifyMessage As String = ""
        Dim emailfrom_list As EmailFromData()
        Dim defaultmessage_list As EmailMessageData()
        Dim unsubscribe_list As EmailMessageData()
        Dim optout_list As EmailMessageData()
        Dim y As Integer = 0
        Try
            emailfrom_list = m_refContApi.GetAllEmailFrom()
            defaultmessage_list = m_refContApi.GetSubscriptionMessagesForType(Common.EkEnumeration.EmailMessageTypes.DefaultMessage)
            unsubscribe_list = m_refContApi.GetSubscriptionMessagesForType(Common.EkEnumeration.EmailMessageTypes.Unsubscribe)
            optout_list = m_refContApi.GetSubscriptionMessagesForType(Common.EkEnumeration.EmailMessageTypes.OptOut)

            sbHtml.Append(Environment.NewLine & "<script language=""javascript"">" & Environment.NewLine)
            sbHtml.Append("function UpdateNotifyStatus() {" & Environment.NewLine)
            sbHtml.Append("if (frmMain.notify_option[0].checked == true) {" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""lbl_notification_status"").innerHTML = '<font color=""green"">Web Alerts are enabled.</font>';" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""suspend_notification_button"").disabled = false;" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""send_notification_button"").disabled = true;" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""send_notification_button"").checked = false;" & Environment.NewLine)
            sbHtml.Append("} else if (frmMain.notify_option[1].checked == true) {" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""lbl_notification_status"").innerHTML = '<font color=""green"">Web Alerts are enabled.</font>';" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""suspend_notification_button"").disabled = true;" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""suspend_notification_button"").checked = false;" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""send_notification_button"").disabled = false;" & Environment.NewLine)
            sbHtml.Append("} else {" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""lbl_notification_status"").innerHTML = '<font color=""red"">Web Alerts are disabled.</font>';" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""suspend_notification_button"").checked = false;" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""send_notification_button"").checked = false;" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""suspend_notification_button"").disabled = true;" & Environment.NewLine)
            sbHtml.Append("    document.getElementById(""send_notification_button"").disabled = true;" & Environment.NewLine)
            sbHtml.Append("}" & Environment.NewLine)
            sbHtml.Append("}" & Environment.NewLine)
            sbHtml.Append("function  PreviewWebAlert() {" & Environment.NewLine)
            sbHtml.Append("    var contentid, defmsgid, optid, summaryid, unsubid, conttype, usecontlink;" & Environment.NewLine)
            sbHtml.Append("    if (document.getElementById('use_content_button').checked == true) {;" & Environment.NewLine)
            sbHtml.Append("      contentid = document.getElementById('frm_content_id').value;" & Environment.NewLine)
            sbHtml.Append("    } else {" & Environment.NewLine)
            sbHtml.Append("      contentid = 0;" & Environment.NewLine)
            sbHtml.Append("    }" & Environment.NewLine)
            sbHtml.Append("    if (document.getElementById('use_message_button').checked == true) {;" & Environment.NewLine)
            sbHtml.Append("      defmsgid = document.getElementById('notify_messageid').value;" & Environment.NewLine)
            sbHtml.Append("    } else {" & Environment.NewLine)
            sbHtml.Append("      defmsgid = 0;" & Environment.NewLine)
            sbHtml.Append("    }" & Environment.NewLine)
            sbHtml.Append("    optid = document.getElementById('notify_optoutid').value;" & Environment.NewLine)
            sbHtml.Append("    summaryid = document.getElementById('use_summary_button').checked; " & Environment.NewLine)
            sbHtml.Append("    unsubid = document.getElementById('notify_unsubscribeid').value;" & Environment.NewLine)
            sbHtml.Append("    conttype = document.getElementById('content_type').value;" & Environment.NewLine)
            sbHtml.Append("    if (document.getElementById('use_contentlink_button').checked == true) {;" & Environment.NewLine)
            sbHtml.Append("      usecontlink = 1;" & Environment.NewLine)
            sbHtml.Append("    } else {" & Environment.NewLine)
            sbHtml.Append("      usecontlink = 0;" & Environment.NewLine)
            sbHtml.Append("    }" & Environment.NewLine)
            sbHtml.Append("    window.open('previewwebalert.aspx?content=" & m_intItemId & "&defmsg=' + defmsgid + '&optoutid=' + optid + '&summaryid=' + summaryid + '&usecontentid=' + contentid + '&unsubscribeid=' + unsubid + '&content_type=' + conttype + '&uselink=' + usecontlink,'','menubar=no,location=no,resizable=yes,scrollbars=yes,status=yes'); " & Environment.NewLine)
            sbHtml.Append("}" & Environment.NewLine)
            sbHtml.Append("function SetMessageContenttoDefault() {" & Environment.NewLine)
            sbHtml.Append("    document.getElementById('use_content_button').checked = true;" & Environment.NewLine)
            sbHtml.Append("    document.getElementById('frm_content_id').value = -1; " & Environment.NewLine)
            sbHtml.Append("    document.getElementById('titlename').value = '[[use current]]'; " & Environment.NewLine)
            sbHtml.Append("}" & Environment.NewLine)
            sbHtml.Append("function enableCheckboxes() {" & Environment.NewLine)
            sbHtml.Append("    var idx, masterBtn, tableObj, enableFlag, qtyElements;" & Environment.NewLine)
            sbHtml.Append("    tableObj = document.getElementById('cfld_subscription_assignment');" & Environment.NewLine)
            sbHtml.Append("    enableFlag = false;" & Environment.NewLine)
            sbHtml.Append("    masterBtn = document.getElementById('break_inherit_button');" & Environment.NewLine)
            sbHtml.Append("    if (validateObject(masterBtn)){" & Environment.NewLine)
            sbHtml.Append("        enableFlag = masterBtn.checked;" & Environment.NewLine)
            sbHtml.Append("    }" & Environment.NewLine)
            sbHtml.Append("    if (validateObject(tableObj)){" & Environment.NewLine)
            sbHtml.Append("        qtyElements = tableObj.all.length;" & Environment.NewLine)
            sbHtml.Append("        for(idx = 0; idx < qtyElements; idx++ ) {" & Environment.NewLine)
            sbHtml.Append("    		    if (tableObj.all[idx].type == 'checkbox'){" & Environment.NewLine)
            sbHtml.Append("    			    tableObj.all[idx].disabled = !enableFlag;" & Environment.NewLine)
            sbHtml.Append("    		    }" & Environment.NewLine)
            sbHtml.Append("        }" & Environment.NewLine)
            sbHtml.Append("    }" & Environment.NewLine)
            sbHtml.Append("}" & Environment.NewLine)
            sbHtml.Append("function validateObject(obj) {" & Environment.NewLine)
            sbHtml.Append("     return ((obj != null) &&" & Environment.NewLine)
            sbHtml.Append("         ((typeof(obj)).toLowerCase() != 'undefined') &&" & Environment.NewLine)
            sbHtml.Append("         ((typeof(obj)).toLowerCase() != 'null'))" & Environment.NewLine)
            sbHtml.Append("}" & Environment.NewLine)
            sbHtml.Append("function valAndSaveCSubAssignments() {" & Environment.NewLine)
            If (Not (active_subscription_list Is Nothing)) And (Not (subscription_data_list Is Nothing)) And (Not ((emailfrom_list Is Nothing) Or (defaultmessage_list Is Nothing) Or (unsubscribe_list Is Nothing) Or (optout_list Is Nothing) _
             Or ((settings_data.AsynchronousLocation = "")))) Then
                sbHtml.Append("    var idx, masterBtn, tableObj, enableFlag, qtyElements, retStr;" & Environment.NewLine)
                sbHtml.Append("    var hidnFld;" & Environment.NewLine)
                sbHtml.Append("    //hidnFld = document.getElementById('content_sub_assignments');" & Environment.NewLine)
                sbHtml.Append("    document.forms[0].content_sub_assignments.value = ''; //hidnFld.value=''" & Environment.NewLine)
                'sbHtml.Append("    tableObj = document.getElementById('cfld_subscription_assignment');" & Environment.NewLine)
                sbHtml.Append("    tableObj = tableObj = document.getElementById('therows');" & Environment.NewLine)
                sbHtml.Append("    tableObj = tableObj.getElementsByTagName('input');" & Environment.NewLine)
                sbHtml.Append("    enableFlag = true;" & Environment.NewLine)
                sbHtml.Append("    retStr = '';" & Environment.NewLine)
                sbHtml.Append("    if ((validateObject(tableObj)) && enableFlag){" & Environment.NewLine)
                sbHtml.Append("        qtyElements = tableObj.length;" & Environment.NewLine)
                sbHtml.Append("        for(idx = 0; idx < qtyElements; idx++ ) {" & Environment.NewLine)
                sbHtml.Append("    		    if ((tableObj[idx].type == 'checkbox') && tableObj[idx].checked){" & Environment.NewLine)
                sbHtml.Append("    			    retStr = retStr + tableObj[idx].name + ' ';" & Environment.NewLine)
                sbHtml.Append("    		    }" & Environment.NewLine)
                sbHtml.Append("        }" & Environment.NewLine)
                sbHtml.Append("    }" & Environment.NewLine)
                sbHtml.Append("    document.forms[0].content_sub_assignments.value = retStr; // hidnFld.value = " & Environment.NewLine)
            End If
            sbHtml.Append("    return true; // (Note: return false to prevent form submission)" & Environment.NewLine)
            sbHtml.Append("}" & Environment.NewLine)
            sbHtml.Append("</script>" & Environment.NewLine)

            If (active_subscription_list Is Nothing) Then
                sbHtml.Append("<input type=""hidden"" name=""suppress_notification"" value=""true"">")
                phSubscription.Visible = False
                EditSubscriptionHtml.Visible = False
                lblNotificationStatus.Text = "<input type=""hidden"" name=""suppress_notification"" value=""true"">"
            ElseIf (emailfrom_list Is Nothing) Or (defaultmessage_list Is Nothing) Or (unsubscribe_list Is Nothing) Or (optout_list Is Nothing) Or (subscription_data_list Is Nothing) _
             Or ((settings_data.AsynchronousLocation = "")) Then
                sbHtml.Append("<div id=""dvSubscription"">")
                sbHtml.Append("<input type=""hidden"" name=""suppress_notification"" value=""true"">")
                sbHtml.Append("<br/><b>" & m_refMsg.GetMessage("lbl web alert settings") & ":</b><br/><br/>" & m_refMsg.GetMessage("lbl web alert not setup") & "<br/>")
                If (emailfrom_list Is Nothing) Then
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=""red"">" & m_refMsg.GetMessage("lbl web alert emailfrom not setup") & "</font>")
                End If
                If (defaultmessage_list Is Nothing) Then
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=""red"">" & m_refMsg.GetMessage("lbl web alert def msg not setup") & "</font>")
                End If
                If (unsubscribe_list Is Nothing) Then
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=""red"">" & m_refMsg.GetMessage("lbl web alert unsub not setup") & "</font>")
                End If
                If (optout_list Is Nothing) Then
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=""red"">" & m_refMsg.GetMessage("lbl web alert optout not setup") & "</font>")
                End If
                If (subscription_data_list Is Nothing) Then
                    phSubscription.Visible = False
                    EditSubscriptionHtml.Visible = False
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=""red"">" & m_refMsg.GetMessage("alt No subscriptions are enabled on the folder.") & "</font>")
                End If
                If ((settings_data.AsynchronousLocation = "") ) Then
                    sbHtml.Append("<br/>&nbsp;&nbsp;<font color=""red"">" & m_refMsg.GetMessage("alt The location to the Asynchronous Data Processor is not specified.") & "</font>")
                End If
                sbHtml.Append("</div>")
            Else
                If subscription_properties_list Is Nothing Then
                    subscription_properties_list = New SubscriptionPropertiesData '(subscription_data_list Is Nothing) Then
                End If
                sbHtml.Append("<div id=""dvSubscription"">")
                sbHtml.Append("<table class=""ektronGrid"" width=""100%"">")
                sbHtml.Append("<tr><td class=""label"">")
                sbHtml.Append("" & m_refMsg.GetMessage("lbl web alert opt") & ":")
                sbHtml.Append("</td>")

                Select Case subscription_properties_list.NotificationType.GetHashCode
                    Case 0
                        strNotifyA = " CHECKED=""true"" "
                        strNotifyI = ""
                        strNotifyN = ""
                        strNotifySend = " DISABLED=""true"" "
                        strNotifySuspend = ""
                        strNotifyMessage = "<font color=""green"">Web Alerts are enabled.</font>"
                    Case 1
                        strNotifyA = ""
                        strNotifyI = " CHECKED=""true"" "
                        strNotifyN = ""
                        strNotifySend = ""
                        strNotifySuspend = " DISABLED=""true"" "
                        strNotifyMessage = "<font color=""green"">Web Alerts are enabled.</font>"
                    Case 2
                        strNotifyA = ""
                        strNotifyI = ""
                        strNotifyN = " CHECKED=""true"" "
                        strNotifySend = " DISABLED=""true"" "
                        strNotifySuspend = " DISABLED=""true"" "
                        strNotifyMessage = "<font color=""red"">Web Alerts are disabled.</font>"
                End Select
                sbHtml.Append("<td class=""value"">")
                sbHtml.Append("&nbsp;&nbsp;<input type=""radio"" value=""Always"" name=""notify_option"" OnClick=""UpdateNotifyStatus()""  " & strNotifyA & ">&nbsp; " & m_refMsg.GetMessage("lbl web alert notify always") & "<br />")

                sbHtml.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=""suspend_notification_button"" onclick=""//;"" type=""checkbox"" name=""suspend_notification_button"" " & strNotifySuspend & ">")

                sbHtml.Append("&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl web alert suspend") & "<br/>")
                sbHtml.Append("&nbsp;&nbsp;<input type=""radio"" value=""Initial"" name=""notify_option"" OnClick=""UpdateNotifyStatus()""  " & strNotifyI & ">")
                sbHtml.Append("&nbsp; " & m_refMsg.GetMessage("lbl web alert notify initial") & "<br />")

                sbHtml.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=""send_notification_button"" onclick=""//;"" type=""checkbox"" name=""send_notification_button"" " & strNotifySend & ">")

                sbHtml.Append("&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl web alert send") & "<br />")
                sbHtml.Append("&nbsp;&nbsp;<input type=""radio"" value=""Never"" name=""notify_option"" OnClick=""UpdateNotifyStatus()""  " & strNotifyN & ">&nbsp; " & m_refMsg.GetMessage("lbl web alert notify never") & "<br/>")

                sbHtml.Append("</td>")
                sbHtml.Append("</tr>")

                sbHtml.Append("<tr>")
                sbHtml.Append("<td class=""label"">")
                sbHtml.Append("" & m_refMsg.GetMessage("lbl web alert subject") & ":")
                sbHtml.Append("</td>")
                sbHtml.Append( "<td class=""value"">")
                If subscription_properties_list.Subject <> "" Then
                    sbHtml.Append("&nbsp;<input type=""text"" maxlength=""255"" size=""65"" value=""" & subscription_properties_list.Subject & """ name=""notify_subject"" " & strEnabled & "/>&nbsp;<br />")
                Else
                    sbHtml.Append("&nbsp;<input type=""text"" maxlength=""255"" size=""65"" value="""" name=""notify_subject"" " & strEnabled & "/>&nbsp;<br />")
                End If
                sbHtml.Append("</td>")
                sbHtml.Append("</tr>")

                'sbHtml.Append("<p><b>Notification Base URL:</b></p><p>")
                'If subscription_properties_list.URL <> "" Then
                '    sbHtml.Append("&nbsp;http://&nbsp;<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" " & strEnabled & " value=""" & subscription_properties_list.URL & """>&nbsp;/<br /><br />")
                'Else
                '    sbHtml.Append("&nbsp;http://&nbsp;<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" " & strEnabled & " value=""" & Request.ServerVariables("HTTP_HOST") & """>&nbsp;/<br /><br />")
                'End If
                'sbHtml.Append("</p>")

                sbHtml.Append("<tr>")
                sbHtml.Append("<td class=""label"">")
                sbHtml.Append("" & m_refMsg.GetMessage("lbl web alert emailfrom address") & ":")
                sbHtml.Append("</td>")
                sbHtml.Append("<td class=""value"">")
                sbHtml.Append("<select name=""notify_emailfrom"" id=""notify_emailfrom"">")

                If (Not emailfrom_list Is Nothing) AndAlso emailfrom_list.Length > 0 Then
                    For y = 0 To emailfrom_list.Length - 1
                        If emailfrom_list(y).Email = subscription_properties_list.EmailFrom Then
                            sbHtml.Append("<option value=""" & Server.HtmlEncode(emailfrom_list(y).Email) & """ selected>" & emailfrom_list(y).Email & "</option>")
                        Else
                            sbHtml.Append("<option value=""" & Server.HtmlEncode(emailfrom_list(y).Email) & """>" & emailfrom_list(y).Email & "</option>")
                        End If
                    Next
                End If
                sbHtml.Append("</select>")
                sbHtml.Append("</td>")
                sbHtml.Append("</tr>")

                'sbHtml.Append("<p><b>Notification File Location:</b></p><p>")
                'If subscription_properties_list.WebLocation <> "" Then
                '    sbHtml.Append("&nbsp;" & m_refContApi.SitePath & "&nbsp;<input type=""text"" maxlength=""255"" size=""65"" value=""" & subscription_properties_list.WebLocation & """ name=""notify_weblocation"" " & strEnabled & ">&nbsp;/<br /><br />")
                'Else
                '    sbHtml.Append("&nbsp;" & m_refContApi.SitePath & "&nbsp;<input type=""text"" maxlength=""255"" size=""65"" value=""subscriptions"" name=""notify_weblocation"" " & strEnabled & ">&nbsp;/<br /><br />")
                'End If
                'sbHtml.Append("</p>")
                sbHtml.Append("<tr>")
                sbHtml.Append("<td class=""label"">")
                sbHtml.Append("" & m_refMsg.GetMessage("lbl web alert contents") & ":&nbsp;")
                sbHtml.Append("<img src=""" & m_refContApi.AppPath & "images/UI/Icons/preview.png"" alt=""Preview Web Alert Message"" title=""Preview Web Alert Message"" onclick="" PreviewWebAlert(); return false;"" />")
                sbHtml.Append("</td>")
                sbHtml.Append("<td class=""value"" nowrap=""nowrap"">")
                sbHtml.Append("&nbsp;&nbsp;<input id=""use_optout_button"" type=""checkbox"" checked=""true"" name=""use_optout_button"" disabled=""true"">&nbsp;&nbsp;Opt Out Message")

                sbHtml.Append("&nbsp;&nbsp;<select " & strEnabled & " name=""notify_optoutid"" id=""notify_optoutid"">")
                If (Not optout_list Is Nothing) AndAlso optout_list.Length > 0 Then
                    For y = 0 To optout_list.Length - 1
                        If optout_list(y).Id = subscription_properties_list.OptOutID Then
                            sbHtml.Append("<option value=""" & optout_list(y).Id & """ selected>" & Server.HtmlEncode(optout_list(y).Title) & "</option>")
                        Else
                            sbHtml.Append("<option value=""" & optout_list(y).Id & """>" & Server.HtmlEncode(optout_list(y).Title) & "</option>")
                        End If
                    Next
                End If
                sbHtml.Append("</select><br />")

                If subscription_properties_list.DefaultMessageID > 0 Then
                    sbHtml.Append("&nbsp;&nbsp;<input id=""use_message_button"" type=""checkbox"" checked=""true"" name=""use_message_button"" " & strEnabled & ">&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl use default message"))
                Else
                    sbHtml.Append("&nbsp;&nbsp;<input id=""use_message_button"" type=""checkbox"" name=""use_message_button"" " & strEnabled & ">&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl use default message"))
                End If
                sbHtml.Append("&nbsp;&nbsp;<select " & strEnabled & " name=""notify_messageid"" id=""notify_messageid"">")

                If (Not defaultmessage_list Is Nothing) AndAlso defaultmessage_list.Length > 0 Then
                    For y = 0 To defaultmessage_list.Length - 1
                        If defaultmessage_list(y).Id = subscription_properties_list.DefaultMessageID Then
                            sbHtml.Append("<option value=""" & defaultmessage_list(y).Id & """ selected>" & Server.HtmlEncode(defaultmessage_list(y).Title) & "</option>")
                        Else
                            sbHtml.Append("<option value=""" & defaultmessage_list(y).Id & """>" & Server.HtmlEncode(defaultmessage_list(y).Title) & "</option>")
                        End If
                    Next
                End If
                sbHtml.Append("</select><br />")

                If subscription_properties_list.SummaryID > 0 Then
                    sbHtml.Append("&nbsp;&nbsp;<input id=""use_summary_button"" type=""checkbox"" name=""use_summary_button"" checked=""true"" " & strEnabled & ">&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl use summary message") & "<br />")
                Else
                    sbHtml.Append("&nbsp;&nbsp;<input id=""use_summary_button"" type=""checkbox"" name=""use_summary_button"" " & strEnabled & ">&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl use summary message") & "<br />")
                End If
                If subscription_properties_list.ContentID = -1 Then
                    sbHtml.Append("&nbsp;&nbsp;<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" checked=""true"" " & strEnabled & ">&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl use content message"))
                    sbHtml.Append("&nbsp;&nbsp;<input type=""hidden"" maxlength=""20"" name=""frm_content_id"" id=""frm_content_id"" value=""-1""/><input type=""hidden"" name=""frm_content_langid"" id=""frm_content_langid""/><input type=""hidden"" name=""frm_qlink"" id=""frm_qlink""/><input type=""text"" name=""titlename"" id=""titlename"" value=""[[use current]]"" size=""65"" disabled=""true""/>")
                    sbHtml.Append("<a href=""#"" class=""button buttonInline greenHover selectContent"" onclick="" QuickLinkSelectBase(" & m_intContentFolder.ToString() & ",'frmMain','titlename',0,0,0,0) ;return false;"">" & m_refMsg.GetMessage("lbl use content select") & "</a><a href=""#"" class=""button buttonInline  blueHover useCurrent"" onclick="" SetMessageContenttoDefault();return false;"">" & m_refMsg.GetMessage("use current") & "</a><br/>")
                ElseIf subscription_properties_list.ContentID > 0 Then
                    sbHtml.Append("&nbsp;&nbsp;<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" checked=""true"" " & strEnabled & ">&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl use content message"))
                    sbHtml.Append("&nbsp;&nbsp;<input type=""hidden"" maxlength=""20"" name=""frm_content_id"" id=""frm_content_id"" value=""" & subscription_properties_list.ContentID.ToString() & """/><input type=""hidden"" name=""frm_content_langid"" id=""frm_content_langid""/><input type=""hidden"" name=""frm_qlink"" id=""frm_qlink""/><input type=""text"" name=""titlename"" id=""titlename"" value=""" & subscription_properties_list.UseContentTitle.ToString & """ size=""65"" disabled=""true""/>")
                    sbHtml.Append("<a href=""#"" class=""button buttonInline greenHover selectContent"" onclick="" QuickLinkSelectBase(" & m_intContentFolder.ToString() & ",'frmMain','titlename',0,0,0,0) ;return false;"">" & m_refMsg.GetMessage("lbl use content select") & "</a><a href=""#"" class=""button buttonInline  blueHover useCurrent"" onclick="" SetMessageContenttoDefault();return false;"">Use Current</a><br/>")
                Else
                    sbHtml.Append("&nbsp;&nbsp;<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" " & strEnabled & ">&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl use content message"))
                    sbHtml.Append("&nbsp;&nbsp;<input type=""hidden"" maxlength=""20"" name=""frm_content_id"" id=""frm_content_id"" value=""0"" /><input type=""hidden"" name=""frm_content_langid"" id=""frm_content_langid""/><input type=""hidden"" name=""frm_qlink"" id=""frm_qlink""/><input type=""text"" name=""titlename"" id=""titlename"" onkeydown=""return false"" value="""" size=""65"" disabled=""true""/>")
                    sbHtml.Append("<a href=""#"" class=""button buttonInline greenHover selectContent"" onclick="" QuickLinkSelectBase(" & m_intContentFolder.ToString() & ",'frmMain','titlename',0,0,0,0) ;return false;"">" & m_refMsg.GetMessage("lbl use content select") & "</a><a href=""#"" class=""button buttonInline  blueHover useCurrent"" onclick="" SetMessageContenttoDefault();return false;"">Use Current</a><br/>")
                End If
                If subscription_properties_list.UseContentLink > 0 Then
                    sbHtml.Append("&nbsp;&nbsp;<input id=""use_contentlink_button"" type=""checkbox"" name=""use_contentlink_button"" checked=""true"" " & strEnabled & ">&nbsp;&nbsp;Use Content Link<br />")
                Else
                    sbHtml.Append("&nbsp;&nbsp;<input id=""use_contentlink_button"" type=""checkbox"" name=""use_contentlink_button"" " & strEnabled & ">&nbsp;&nbsp;Use Content Link<br />")
                End If
                sbHtml.Append("&nbsp;&nbsp;<input id=""use_unsubscribe_button"" type=""checkbox"" checked=""true"" name=""use_unsubscribe_button"" disabled=""true"">&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl unsubscribe message"))
                sbHtml.Append("&nbsp;&nbsp;<select " & strEnabled & " name=""notify_unsubscribeid"" id=""notify_unsubscribeid"">")
                If (Not unsubscribe_list Is Nothing) AndAlso unsubscribe_list.Length > 0 Then
                    For y = 0 To unsubscribe_list.Length - 1
                        If unsubscribe_list(y).Id = subscription_properties_list.UnsubscribeID Then
                            sbHtml.Append("<option value=""" & unsubscribe_list(y).Id & """ selected>" & Server.HtmlEncode(unsubscribe_list(y).Title) & "</option>")
                        Else
                            sbHtml.Append("<option value=""" & unsubscribe_list(y).Id & """>" & Server.HtmlEncode(unsubscribe_list(y).Title) & "</option>")
                        End If
                    Next
                End If
                sbHtml.Append("</select><br /><br />")
                sbHtml.Append("</td>")
                sbHtml.Append("</tr>")
                sbHtml.Append("</table>")

                sbHtml.Append("<div class=""ektronHeader"">" & m_refMsg.GetMessage("lbl avail web alert") & ":</div>")
                'If blnBreakSubInheritance = True Then
                'sbHtml.Append("<input type=""checkbox"" name=""break_inherit_button"" id=""break_inherit_button"" onclick=""enableCheckboxes();"" Checked=""True"">")
                'Else
                'sbHtml.Append("<input type=""checkbox"" name=""break_inherit_button"" id=""break_inherit_button"" onclick=""enableCheckboxes();"">")
                'End If

                sbHtml.Append("</td></tr>")
                sbHtml.Append("<table class=""ektronGrid"" cellspacing=""1"" id=""cfld_subscription_assignment"" name=""cfld_subscription_assignment""><tbody id=""therows"">")
                lblNotificationStatus.Text = "<span id=""lbl_notification_status"">" & strNotifyMessage & "</span>"
                If Not (subscription_data_list Is Nothing) AndAlso subscription_data_list.Length > 0 Then
                    sbHtml.Append("<tr class=""title-header""><td>" & m_refMsg.GetMessage("lbl assigned") & "</td><td>" & m_refMsg.GetMessage("lbl name") & "</td></tr>")
                    If Not (subscribed_data_list Is Nothing) Then
                        arrSubscribed = Array.CreateInstance(GetType(String), subscribed_data_list.Length)
                        For i = 0 To subscribed_data_list.Length - 1
                            arrSubscribed.SetValue(subscribed_data_list(i).Name, i)
                        Next
                        If arrSubscribed.Length > 0 Then
                            Array.Sort(arrSubscribed)
                        End If
                    End If

                    For i = 0 To subscription_data_list.Length - 1
                        findindex = -1
                        If (Not (subscribed_data_list Is Nothing)) Then
                            findindex = Array.BinarySearch(arrSubscribed, subscription_data_list(i).Name)
                        End If
                        sbHtml.Append("<tr>")
                        If findindex < 0 Then
                            sbHtml.Append("<td nowrap=""true"" align=""center""><input type=""checkbox"" name=""Assigned_" & subscription_data_list(i).Id & """  id=""Assigned_" & subscription_data_list(i).Id & """ " & strEnabled & "></td></td>")
                        Else
                            sbHtml.Append("<td nowrap=""true"" align=""center""><input type=""checkbox"" name=""Assigned_" & subscription_data_list(i).Id & """  id=""Assigned_" & subscription_data_list(i).Id & """ checked=""true"" " & strEnabled & "></td></td>")
                        End If
                        sbHtml.Append("<td nowrap=""true"" align=""Left"">" & subscription_data_list(i).Name & "</td>")
                        sbHtml.Append("</tr>")
                    Next
                Else
                    sbHtml.Append("<tr><td>Nothing available.</td></tr>")
                End If

                sbHtml.Append("</tbody></table>")
                sbHtml.Append("<input type=""hidden"" name=""content_sub_assignments"" id=""content_sub_assignments"" value=""""></td>")
                sbHtml.Append("</tr>")

                sbHtml.Append("</table>")

                sbHtml.Append("</div>")
            End If
            EditSubscriptionHtml.Visible = True
            EditSubscriptionHtml.Text = sbHtml.ToString()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub EditSelectedTemplate()
        Dim template As TemplateData
        Dim str As New StringBuilder()
        Dim bShowTemplateUI As Boolean
        Dim iContType As Integer

        If (Request.QueryString("ContType") Is Nothing) Then
            iContType = CMSContentType_Content
        Else
            iContType = Request.QueryString("ContType")
        End If

        If (m_strType = "add") Then
            If (iContType = CMSContentType_Content Or iContType = CMSContentType_Forms Or iContType = EkConstants.CMSContentType_Media Or _
             iContType = EkConstants.CMSContentType_Archive_Content Or iContType = EkConstants.CMSContentType_Archive_Forms Or _
             iContType = EkConstants.CMSContentType_Archive_Media) Then
                bShowTemplateUI = True
            End If
        Else
            If (content_edit_data IsNot Nothing) Then
                iContType = content_edit_data.Type
                If (iContType = CMSContentType_Content Or iContType = CMSContentType_Forms Or iContType = EkConstants.CMSContentType_Media Or _
                iContType = EkConstants.CMSContentType_Archive_Content Or iContType = EkConstants.CMSContentType_Archive_Forms Or iContType = EkConstants.CMSContentType_Archive_Media) Then
                    bShowTemplateUI = True
                End If
            End If
        End If
        If (Not bShowTemplateUI) Then
            str.Append("Not Applicable")
        Else
            If ((m_strType = "add" AndAlso content_data IsNot Nothing AndAlso (content_data.SubType = CMSContentSubtype.PageBuilderData Or content_data.SubType = CMSContentSubtype.PageBuilderMasterData)) OrElse (content_edit_data IsNot Nothing AndAlso (content_edit_data.SubType = CMSContentSubtype.PageBuilderData Or content_edit_data.SubType = CMSContentSubtype.PageBuilderMasterData))) Then
                'show available wireframes
                Dim wfm As New Ektron.Cms.PageBuilder.WireframeModel()
                Dim wireframe As Ektron.Cms.PageBuilder.WireframeData
                Dim active_template_list As Ektron.Cms.PageBuilder.WireframeData() = wfm.FindByFolderID(folder_data.Id)

                If (active_template_list.Length < 1) Then
                    phTemplates.Visible = False
                    EditTemplateHtml.Visible = False
                End If

                Dim selected_templateid As Long = Me.m_refContApi.GetSelectedTemplateByContent(m_refContentId, m_refContApi.RequestInformationRef.ContentLanguage)
                If (selected_templateid = 0) Then
                    'first check if default language has one
                    If (m_refContApi.RequestInformationRef.ContentLanguage <> m_refContApi.RequestInformationRef.DefaultContentLanguage) Then
                        selected_templateid = Me.m_refContApi.GetSelectedTemplateByContent(m_refContentId, m_refContApi.RequestInformationRef.DefaultContentLanguage)
                    End If
                    If (selected_templateid = 0) Then
                        selected_templateid = folder_data.TemplateId
                    End If
                End If

                str.Append("<table class=""ektronForm"">")
                str.Append("<tr>")
                str.Append("<td class=""label"">")
                str.Append(m_refMsg.GetMessage("template label") & ":")
                str.Append("</td>")
                str.Append("<td class=""value"">")
                str.Append("<select id=""templateSelect"" name=""templateSelect"">")
                For Each wireframe In active_template_list
                    If (wireframe.Template.Id = selected_templateid) Then
                        str.Append("<option value=""" & wireframe.Template.Id & """ selected>" & wireframe.Path & "</option>")
                    Else
                        str.Append("<option value=""" & wireframe.Template.Id & """>" & wireframe.Path & "</option>")
                    End If
                Next

                If (active_template_list.Length = 0) Then
                    str.Append("<option value=""" & folder_data.TemplateId & """>" & folder_data.TemplateFileName & "</option>")
                End If
                str.Append("</select>")
                str.Append("</td>")
                str.Append("</tr>")
                str.Append("</table>")
                'str.Append("&nbsp;&nbsp;&nbsp;<a href=""#"" onclick=""PreviewTemplate('" & m_refContApi.SitePath & "', " & m_refContentId & ", 800,600);return false;""><img src=""" & m_refcontapi.apppath & "images/UI/Icons/preview.png" & """ alt=""Preview Template"" title=""Preview Template"">")
                If (Not content_edit_data Is Nothing) Then
                    If (content_edit_data.LockedContentLink) Then
                        str.AppendLine("<br/>")
                        str.AppendLine("<label>Quicklink Locked:</label><input type=""checkbox"" onclick=""DisableTemplateSelect(this.checked)"" name=""chkLockedContentLink"" id=""chkLockedContentLink""" & IIf(content_edit_data.LockedContentLink, "checked=""true""", "") & """/>")
                        str.AppendLine("<br/>")
                        str.AppendLine("<label>Quicklink:</label> """ & content_edit_data.Quicklink & """")
                        str.AppendLine("<script language=""Javascript""> DisableTemplateSelect(true) </script>")
                    Else
                        str.AppendLine("<input type=""hidden"" name=""chkLockedContentLink"" id=""chkLockedContentLink"" value=""false"" />")
                    End If
                Else
                    str.AppendLine("<input type=""hidden"" name=""chkLockedContentLink"" id=""chkLockedContentLink"" value=""false"" />")
                End If

            Else
                Dim active_template_list As TemplateData() = m_refContApi.GetEnabledTemplatesByFolder(folder_data.Id)

                If (active_template_list.Length < 1) Then
                    phTemplates.Visible = False
                    EditTemplateHtml.Visible = False
                End If

                Dim selected_folder As Long = Me.m_refContApi.GetSelectedTemplateByContent(m_refContentId, m_refContApi.RequestInformationRef.ContentLanguage)
                If (selected_folder = 0) Then
                    selected_folder = folder_data.TemplateId
                End If

                str.Append("<table class=""ektronForm"">")
                str.Append("<tr>")
                str.Append("<td class=""label"">")
                str.Append(m_refMsg.GetMessage("template label") & ":")
                str.Append("</td>")
                str.Append("<td class=""value"">")
                str.Append("<select id=""templateSelect"" name=""templateSelect"">")
                For Each template In active_template_list
                    If template.SubType <> TemplateSubType.Wireframes Then
                        If (template.Id = selected_folder) Then
                            str.Append("<option value=""" & template.Id & """ selected>" & template.FileName & "</option>")
                        Else
                            str.Append("<option value=""" & template.Id & """>" & template.FileName & "</option>")
                        End If
                    End If
                Next

                If (active_template_list.Length = 0) Then
                    str.Append("<option value=""" & folder_data.TemplateId & """>" & folder_data.TemplateFileName & "</option>")
                End If
                str.Append("</select>")
                str.Append("</td>")
                str.Append("</tr>")
                str.Append("</table>")
                'str.Append("&nbsp;&nbsp;&nbsp;<a href=""#"" onclick=""PreviewTemplate('" & m_refContApi.SitePath & "', " & m_refContentId & ", 800,600);return false;""><img src=""" & m_refcontapi.apppath & "images/UI/Icons/preview.png" & """ alt=""Preview Template"" title=""Preview Template"">")
                If (Not content_edit_data Is Nothing) Then
                    If (content_edit_data.LockedContentLink) Then
                        str.AppendLine("<br/>")
                        str.AppendLine("<label>Quicklink Locked:</label><input type=""checkbox"" onclick=""DisableTemplateSelect(this.checked)"" name=""chkLockedContentLink"" id=""chkLockedContentLink""" & IIf(content_edit_data.LockedContentLink, "checked=""true""", "") & """/>")
                        str.AppendLine("<br/>")
                        str.AppendLine("<label>Quicklink:</label> """ & content_edit_data.Quicklink & """")
                        str.AppendLine("<script language=""Javascript""> DisableTemplateSelect(true) </script>")
                    Else
                        str.AppendLine("<input type=""hidden"" name=""chkLockedContentLink"" id=""chkLockedContentLink"" value=""false"" />")
                    End If
                Else
                    str.AppendLine("<input type=""hidden"" name=""chkLockedContentLink"" id=""chkLockedContentLink"" value=""false"" />")
                End If
            End If
            End If

            EditTemplateHtml.Text = "<div id=""dvTemplates"">" & str.ToString() & "</div>"
    End Sub

    Private Sub EditScheduleHtmlScripts()
        Dim sbHtml As New StringBuilder
        Dim dateSchedule As EkDTSelector
        dateSchedule = Me.m_refContApi.EkDTSelectorRef

        sbHtml.Append("<div id=""dvSchedule"">")
        sbHtml.Append("<table class=""ektronForm"">")
        sbHtml.Append("<tr>")

        sbHtml.Append("<script language=""javascript"">")
        sbHtml.Append("function OpenCalendar(bStartDate) {")
        sbHtml.Append("if (true == bStartDate) {")
        sbHtml.Append("document.forms[0].go_live.value = Trim(document.forms[0].go_live.value);CallCalendar(document.forms[0].go_live.value, 'calendar.aspx', 'go_live', 'frmMain');")
        sbHtml.Append("} else if (false == bStartDate) {")
        sbHtml.Append("document.forms[0].end_date.value = Trim(document.forms[0].end_date.value);CallCalendar(document.forms[0].end_date.value, 'calendar.aspx', 'end_date', 'frmMain');")
        sbHtml.Append("}")
        sbHtml.Append("}")
        sbHtml.Append("</script>")
        sbHtml.Append("<td class=""label"">" & m_refMsg.GetMessage("generic start date label") & "</td>")
        sbHtml.Append("<td class=""value"">")
        dateSchedule.formName = "frmMain"
        dateSchedule.extendedMeta = True
        dateSchedule.formElement = "go_live"
        dateSchedule.spanId = "go_live_span"
        If (go_live <> "") Then
            dateSchedule.targetDate = CDate(go_live)
        End If
        sbHtml.Append(dateSchedule.displayCultureDateTime(True))
        'sbHtml.Append("<input type=""Text"" onclick=""OpenCalendar(true);return (false);"" name=""go_live"" value=""" & go_live & """ size=""30"" maxlength=""30"" ID=""Text1"">")
        'sbHtml.Append("<a href=""#"" onclick=""OpenCalendar(true);return (false);""><img src=""" & AppImgPath & "btn_calendar-nm.gif"" alt=""" & m_refMsg.GetMessage("alt calendar button text") & """ title=""" & m_refMsg.GetMessage("alt calendar button text") & """></a>")
        sbHtml.Append("</td>")
        If (True) Then '(IsBrowserIE()) Then %>
            sbHtml.Append("</tr>")
            sbHtml.Append("<tr>")
        Else
            sbHtml.Append("<td>&nbsp;</td>")
        End If
        sbHtml.Append("<td class=""label"">" & m_refMsg.GetMessage("generic end date label") & "</td>")
        sbHtml.Append("<td class=""value"">")
        dateSchedule.formName = "frmMain"
        dateSchedule.extendedMeta = True
        dateSchedule.formElement = "end_date"
        dateSchedule.spanId = "end_date_span"
        If (end_date <> "") Then
            dateSchedule.targetDate = CDate(end_date)
        Else
            dateSchedule.targetDate = Nothing
        End If
        sbHtml.Append(dateSchedule.displayCultureDateTime(True))
        'sbHtml.Append("<input type=""Text"" onclick=""OpenCalendar(false);return (false);"" name=""end_date"" value=""" & end_date & """ size=""30"" maxlength=""30"" ID=""Text2"">")
        'sbHtml.Append("<a href=""#"" onclick=""OpenCalendar(false);return (false);""><img src=""" & AppImgPath & "btn_calendar-nm.gif"" alt=""" & m_refMsg.GetMessage("alt calendar button text") & """ title=""" & m_refMsg.GetMessage("alt calendar button text") & """></a>")
        sbHtml.Append("</td>")

        sbHtml.Append("</tr>")
        sbHtml.Append("<tr>")
        sbHtml.Append("<td class=""label"">" & m_refMsg.GetMessage("End Date Action Title") & ":</td>")

        If m_strType = "add" OrElse m_strType = "multiple,add" Then
            end_date_action = 1
        End If
        Dim ii As Object
        Dim DoCheck As Boolean = False
        sbHtml.Append("<td class=""value"">")
        For ii = 1 To endDateActionSize
            If folder_data.FolderType = FolderType.Blog AndAlso ii = 2 Then 'blog + archive and remain
                If CStr(ii) = CStr(end_date_action) Then
                    DoCheck = True
                End If
            Else
                sbHtml.Append("<input type=""radio"" name=""end_date_action_radio"" value=""" & ii & """")
                If CStr(ii) = CStr(end_date_action) Or DoCheck Then
                    sbHtml.Append(" checked")
                    If DoCheck Then
                        DoCheck = False
                    End If
                End If
                sbHtml.Append(">" & endDateActionSel(Convert.ToString(ii)) & "<br />")
            End If
        Next
        sbHtml.Append("</td>")
        sbHtml.Append("</tr>")
        sbHtml.Append("</table>")
        sbHtml.Append("</div>")
        EditScheduleHtml.Text = sbHtml.ToString()
    End Sub

    Private Function HideVariables() As String
        Dim result As New System.Text.StringBuilder
        iSegment = 0
        iSegment2 = 0
        Dim i As Integer = 1
        Dim iPackLoop As Integer = 1
        Dim var1 As String

        If (Trim(Len(m_strContentHtml)) = 0) Then
            If (Trim(Len(editorPackage)) > 0) Then
                m_strContentHtml = m_refContApi.TransformXsltPackage(editorPackage, Server.MapPath(Me.m_refContApi.AppeWebPath & "unpackageDocument.xslt"), True)
            End If
        End If



        'm_strContentHtml = Server.HtmlEncode(m_strContentHtml)
        'editorPackage = Server.HtmlEncode(editorPackage)

        If (Len(m_strContentHtml) > iMaxContLength) Then
            var1 = Len(m_strContentHtml)
        Else
            var1 = iMaxContLength
        End If
        Do While i <= var1
            'result.Append("<input type=""hidden"" name=""hiddencontent" & iSegment + 1 & """ value=""" & Mid(m_strContentHtml, i, 65000) & """>" & vbCrLf)
            result.Append("<input type=""hidden"" name=""hiddencontent" & iSegment + 1 & """ value="""">" & vbCrLf)
            result.Append("<input type=""hidden"" name=""searchtext" & iSegment + 1 & """ value="""">" & vbCrLf)
            i = (i + 65000)
            iSegment = (iSegment + 1)
        Loop
        iPackLoop = 1
        If (Len(editorPackage) > iMaxContLength) Then
            var1 = Len(editorPackage)
        Else
            var1 = iMaxContLength
        End If
        Do While iPackLoop <= var1

            'result.Append("<input type=""hidden"" name=""hiddenpackage" & iSegment2 + 1 & """ value=""" & Mid(editorPackage, iPackLoop, 65000) & """>" & vbCrLf)
            result.Append("<input type=""hidden"" name=""hiddenpackage" & iSegment2 + 1 & """ value="""">" & vbCrLf)

            iPackLoop = (iPackLoop + 65000)
            iSegment2 = (iSegment2 + 1)
        Loop
        result.Append("<input type=""hidden"" name=""numberoffields"" value=""" & iSegment & """> <input type=""hidden"" name=""hiddenPackageSize"" value=""" & iSegment2 & """>" & vbCrLf)
        Return (result.ToString)
    End Function

    Private Function SetActionClientScript(ByVal publishAsHtml As Boolean) As String
        Dim result As New System.Text.StringBuilder
        If (IsMac AndAlso Not IsBrowserIE AndAlso m_SelectedEditControl <> "ContentDesigner") Then
            result.Append("function launchLibrary(){" & vbCrLf)
            result.Append(" librarySelectedText('');	" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("function lauchLibraryHTML(){" & vbCrLf)
            result.Append(" elx1.GetSelectedText ('librarySelectedText');" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("function lauchWikipopup(){" & vbCrLf)
            result.Append(" elx1.GetSelectedText ('wikiSelectedText');" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("function wikiSelectedText(src){" & vbCrLf)
            result.Append(" document.getElementById('selectedhtml').value = src;" & vbCrLf)
            result.Append(" src = $ektron.removeTags(src);" & vbCrLf)
            result.Append(" document.getElementById('selectedtext').value = src;" & vbCrLf)
            result.Append(" var remote=null;" & vbCrLf)
            result.Append(" var link = ""ewebeditpro/wikipopup.aspx?FolderID=" & m_intContentFolder & "&wikititle="" + src" & vbCrLf)
            result.Append(" remote = window.open(link,'EditWikiLink','toolbar=0,location=0,directories=0,menubar=0,scrollbars=1,resizable=1,width=680,height=385');" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("function librarySelectedText(src){" & vbCrLf)
            result.Append(" document.forms[0].selectedtext.value = src;" & vbCrLf)
            result.Append(" var remote=null;" & vbCrLf)
            result.Append(" remote = window.open(""mediamanager.aspx?actiontype=library&scope=all&autonav="",'Preview','width=' + 600 + ',height=' + 400 +',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("function insertImage(src,linktitle){" & vbCrLf)
            result.Append("GetEphoxEditor().InsertHTMLAtCursor(escape(""<img src=\"""" + src + ""\"" alt=\"""" + linktitle+ ""\"" title=\"""" + linktitle + ""\"">""));" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("function insertOther(src,linktitle){" & vbCrLf)
            result.Append("GetEphoxEditor().InsertHTMLAtCursor(escape(""<a href=\"""" + src + ""\"">"" + linktitle + ""</a>""));" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("function insertHTML(html){" & vbCrLf)
            result.Append("GetEphoxEditor().InsertHTMLAtCursor(escape(html));" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("function GetMACContent(src){" & vbCrLf)
            result.Append(" document.forms[0].content_teaser.value = src;" & vbCrLf)
            result.Append(" elx1.GetBody('SetAction');" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("function VerifyManagedFileName() {" & vbCrLf)
            'result.Append("alert('verify'); " & vbCrLf)
            result.Append(" var fileupload = document.getElementById('fileupload'); " & vbCrLf)
            result.Append(" if ((fileupload != null) && (fileupload.value.length > 0)) { " & vbCrLf)
            'result.Append(" alert('fileupload.value '+fileupload.value); " & vbCrLf)
            result.Append("   var objvalidTypes = document.getElementById('validTypes'); " & vbCrLf)
            'result.Append("  alert(objvalidTypes.value);" & vbCrLf)
            result.Append("   var fileUploadExtIndex = fileupload.value.lastIndexOf('.'); " & vbCrLf)
            result.Append("   var fileUploadExt = fileupload.value.substring(fileUploadExtIndex + 1); " & vbCrLf)
            'result.Append("  alert(fileUploadExt);" & vbCrLf)
            result.Append("   var arrTypes = objvalidTypes.value.split(',');" & vbCrLf)
            result.Append("   var found = false;" & vbCrLf)
            result.Append("   var i = 0;" & vbCrLf)
            result.Append("   for (i = 0; i < arrTypes.length; ++i) {" & vbCrLf)
            result.Append("     if (arrTypes[i].toLowerCase() == fileUploadExt.toLowerCase()) { " & vbCrLf)
            result.Append("         found = true;" & vbCrLf)
            result.Append("         break;" & vbCrLf)
            result.Append("     }" & vbCrLf)
            result.Append("   }" & vbCrLf)
            result.Append("   if (!(found)) { " & vbCrLf)
            result.Append("     alert('" & m_refMsg.GetMessage("lbl invalid file type") & "');" & vbCrLf)
            result.Append("     return false;" & vbCrLf)
            result.Append("   } " & vbCrLf)
            result.Append("   var oldfilename = document.getElementById('oldfilename'); " & vbCrLf)
            result.Append("   if ((oldfilename != null) && (oldfilename.value.length > 0)) { " & vbCrLf)
            result.Append("          var justfilename = fileupload.value.match(/(.*)[\/\\]([^\/\\]+\.\w+)$/); " & vbCrLf)
            'result.Append("         alert('justfilename = ' + justfilename[2]);  " & vbCrLf)
            result.Append("         if ((justfilename[2] != null) && (justfilename[2].length > 0) && (oldfilename.value.toLowerCase() != justfilename[2].toLowerCase())) { " & vbCrLf)
            result.Append("             alert('" & m_refMsg.GetMessage("js:cannot replace provide original file") & "' + oldfilename.value);" & vbCrLf)
            result.Append("                 return false;" & vbCrLf)
            result.Append("         }" & vbCrLf)
            result.Append("   }" & vbCrLf)
            result.Append("}" & vbCrLf)
            'in case of add new asset/translate asset verify file was selected (type=add here)
            result.Append("if ((fileupload != null) && (fileupload.value.length <= 0)) { " & vbCrLf)
            result.Append("   var editmode = document.getElementById('type'); " & vbCrLf)
            result.Append("   if ((editmode != null) && (editmode.value.length > 0) && (editmode.value.toLowerCase() == 'add')) { " & vbCrLf)
            result.Append("     alert('" & m_refMsg.GetMessage("lbl upload file") & "');" & vbCrLf)
            result.Append("     return false;" & vbCrLf)
            result.Append("   }" & vbCrLf)
            result.Append("}" & vbCrLf)
            'result.Append("alert('verify done'); " & vbCrLf)
            result.Append("return true;" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("function SetAction(src) { " & vbCrLf)
            result.Append("if (src != 'cancel') {" & vbCrLf)
            result.Append("  if (false==validateContentTitle()) {return false}; " & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("if (false==VerifyManagedFileName) {return false;}} " & vbCrLf)
            result.Append("if (IsBrowserIE()) {" & vbCrLf)
            result.Append("if (false==IsCmsEditEnable) {return false;}}" & vbCrLf)
            result.Append("Button = buttonaction;" & vbCrLf)
        Else
            result.Append("var blnAsked=false;" & vbCrLf)

            result.Append("function VerifyManagedFileName() {" & vbCrLf)
            'result.Append("alert('verify'); " & vbCrLf)
            result.Append(" var fileupload = document.getElementById('fileupload'); " & vbCrLf)
            result.Append(" if ((fileupload != null) && (fileupload.value.length > 0)) { " & vbCrLf)
            'result.Append(" alert('fileupload.value '+fileupload.value); " & vbCrLf)
            result.Append("   var objvalidTypes = document.getElementById('validTypes'); " & vbCrLf)
            'result.Append("  alert(objvalidTypes.value);" & vbCrLf)
            result.Append("   var fileUploadExtIndex = fileupload.value.lastIndexOf('.'); " & vbCrLf)
            result.Append("   var fileUploadExt = fileupload.value.substring(fileUploadExtIndex + 1); " & vbCrLf)
            'result.Append("  alert(fileUploadExt);" & vbCrLf)
            result.Append("   var arrTypes = objvalidTypes.value.split(',');" & vbCrLf)
            result.Append("   var found = false;" & vbCrLf)
            result.Append("   var i = 0;" & vbCrLf)
            result.Append("   for (i = 0; i < arrTypes.length; ++i) {" & vbCrLf)
            result.Append("     if (arrTypes[i].toLowerCase() == fileUploadExt.toLowerCase()) { " & vbCrLf)
            result.Append("         found = true;" & vbCrLf)
            result.Append("         break;" & vbCrLf)
            result.Append("     }" & vbCrLf)
            result.Append("   }" & vbCrLf)
            result.Append("   if (!(found)) { " & vbCrLf)
            result.Append("     alert('" & m_refMsg.GetMessage("lbl invalid file type") & "');" & vbCrLf)
            result.Append("     return false;" & vbCrLf)
            result.Append("   } " & vbCrLf)
            result.Append("   var oldfilename = document.getElementById('oldfilename'); " & vbCrLf)
            result.Append("         var justfilename = ''; " & vbCrLf)
            result.Append("         if (IsFireFox() && GetFireFoxVersion() >= 3) { justfilename = fileupload.value; } else {" & vbCrLf)
            result.Append("             var tmpPath = fileupload.value.match(/(.*)[\/\\]([^\/\\]+\.\w+)$/); justfilename = tmpPath[2];}" & vbCrLf)
            result.Append("         if ((justfilename != null) && (justfilename.length > 0) && (justfilename.indexOf('&') > -1 || justfilename.indexOf('+') > -1 )) { " & vbCrLf)
            result.Append("             alert('" & m_refMsg.GetMessage("js:cannot add file with add and plus") & "');" & vbCrLf)
            result.Append("                 return false;" & vbCrLf)
            result.Append("         }" & vbCrLf)
            result.Append("   if ((oldfilename != null) && (oldfilename.value.length > 0)) { " & vbCrLf)
            result.Append("         if ((justfilename != null) && (justfilename.length > 0) && (oldfilename.value.toLowerCase() != justfilename.toLowerCase())) { " & vbCrLf)
            result.Append("             alert('" & m_refMsg.GetMessage("js:cannot replace provide original file") & "' + oldfilename.value);" & vbCrLf)
            result.Append("                 return false;" & vbCrLf)
            result.Append("         }" & vbCrLf)
            result.Append("   }" & vbCrLf)
            result.Append("}" & vbCrLf)
            'in case of add new asset/translate asset verify file was selected (type=add here)
            result.Append("if ((fileupload != null) && (fileupload.value.length <= 0)) { " & vbCrLf)
            result.Append("   var editmode = document.getElementById('type'); " & vbCrLf)
            result.Append("   if ((editmode != null) && (editmode.value.length > 0) && (editmode.value.toLowerCase() == 'add')) { " & vbCrLf)
            result.Append("     alert('" & m_refMsg.GetMessage("lbl upload file") & "');" & vbCrLf)
            result.Append("     return false;" & vbCrLf)
            result.Append("   }" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("return true;" & vbCrLf)
            result.Append("}" & vbCrLf)

            'Preview Content Code Starts
            result.Append("function PreviewContent(obj, contTitle) { " & vbCrLf)
            result.Append(" SetAction('save');" & vbCrLf)
            result.Append(" window.open(obj,""contTitle"",'scrollbars=yes,resizable=yes');" & vbCrLf)
            result.Append(" return false;" & vbCrLf)
            result.Append("}" & vbCrLf)
            'Preview Content Code Ends

            result.Append("function SetAction(Button) { " & vbCrLf)
            result.Append(" $ektron('#pleaseWait').modalShow(); " & vbCrLf)
            result.Append("if (Button != 'cancel') {" & vbCrLf)
            result.Append("  if (false==validateContentTitle()) {return false}; " & vbCrLf)
            result.Append("}" & vbCrLf)
            If 1 = lContentType Then
                result.Append("if (false == bContentEditorReady || false == bTeaserEditorReady)" & vbCrLf)
                result.Append("{" & vbCrLf)
                result.Append("     return false; " & vbCrLf)
                result.Append("}" & vbCrLf)
            ElseIf 2 = lContentType Then
                result.Append("if (false == bFormEditorReady || false == bResponseEditorReady)" & vbCrLf)
                result.Append("{" & vbCrLf)
                result.Append("     return false; " & vbCrLf)
                result.Append("}" & vbCrLf)
            Else
                result.Append("if (false == bTeaserEditorReady)" & vbCrLf)
                result.Append("{" & vbCrLf)
                result.Append("     return false; " & vbCrLf)
                result.Append("}" & vbCrLf)
            End If
            'result.Append(" alert(Button); " & vbCrLf)
            ' var fileUploadExtIndex = fileupload.value.lastIndexOf('.');
            ' var fileUploadExt = fileupload.value.substring(fileUploadExtIndex + 1)
            result.Append("var blnAnswer;" & vbCrLf)
            result.Append("if (('cancel' == Button) && (blnAsked==false)) {" & vbCrLf)
            result.Append("blnAnswer=confirm(""" & m_refMsg.GetMessage("js: alert confirm close no save") & """);" & vbCrLf)
            result.Append("if (false==blnAnswer) {" & vbCrLf)
            result.Append("$ektron('#pleaseWait').modalHide(); " & vbCrLf)
            result.Append("return false;" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("else {" & vbCrLf)
            result.Append("if(""undefined"" != typeof(eWebEditPro)){eWebEditPro.actionOnUnload = EWEP_ONUNLOAD_NOSAVE;}" & vbCrLf)
			result.Append(" if (""object"" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances) { " & vbCrLf)
			result.Append("     var objContent = Ektron.ContentDesigner.instances[""content_html""]; " & vbCrLf)
            result.Append("		if (objContent) " & vbCrLf)
            result.Append("		{ " & vbCrLf)
            result.Append("		    objContent.isChanged = false; " & vbCrLf)
            result.Append("		} " & vbCrLf)
			result.Append("     var objTeaser = Ektron.ContentDesigner.instances[""content_teaser""]; " & vbCrLf)
            result.Append("		if (objTeaser) " & vbCrLf)
            result.Append("		{ " & vbCrLf)
            result.Append("		    objTeaser.isChanged = false; " & vbCrLf)
            result.Append("		} " & vbCrLf)
			result.Append("     var objFormT = Ektron.ContentDesigner.instances[""forms_transfer""]; " & vbCrLf)
            result.Append("		if (objFormT) " & vbCrLf)
            result.Append("		{ " & vbCrLf)
            result.Append("		    objFormT.isChanged = false; " & vbCrLf)
            result.Append("		} " & vbCrLf)
			result.Append("     var objFormR = Ektron.ContentDesigner.instances[""forms_redirect""]; " & vbCrLf)
            result.Append("		if (objFormR) " & vbCrLf)
            result.Append("		{ " & vbCrLf)
            result.Append("		    objFormR.isChanged = false; " & vbCrLf)
            result.Append("		} " & vbCrLf)
            result.Append(" }" & vbCrLf)
            result.Append(" blnAsked=true;};" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("if (IsBrowserIE()) {" & vbCrLf)
            result.Append("if (false==IsCmsEditEnable) {return false;}}" & vbCrLf)
        End If
        If folder_data.CategoryRequired = True AndAlso m_refContent.GetAllFolderTaxonomy(m_intContentFolder).Length > 0 Then
            result.Append("      if ((Button != 'cancel') && (Trim(document.getElementById('taxonomyselectedtree').value) == '')) { ").Append(Environment.NewLine)
            result.Append("         alert('" & m_refMsg.GetMessage("js tax cat req") & "'); ").Append(Environment.NewLine)
            'result.Append("         ShowPane('dvTaxonomy'); ").Append(Environment.NewLine)
            result.Append("         $ektron('.tabContainer').tabs('select', 'dvTaxonomy'); ").Append(Environment.NewLine)
            result.Append("         $ektron('#dvTaxonomy').focus(); ").Append(Environment.NewLine)
            result.Append("         $ektron('#pleaseWait').modalHide(); ").Append(Environment.NewLine)
            result.Append("         return false; ").Append(Environment.NewLine)
            result.Append("      } ").Append(Environment.NewLine)
        End If
        result.Append("		if (Button != 'cancel') {" & vbCrLf)
        result.Append("         if (!VerifyManagedFileName()) " & vbCrLf)
        result.Append("		    {" & vbCrLf)
        result.Append("		    	buttonPressed = false;" & vbCrLf)
        result.Append("             $ektron('#pleaseWait').modalHide(); " & vbCrLf)
        result.Append("		    	return false;" & vbCrLf)
        result.Append("		    }" & vbCrLf)
        result.Append("		    if (!ValidateMeta(0))" & vbCrLf)
        result.Append("		    {" & vbCrLf)
        result.Append("		    	buttonPressed = false;" & vbCrLf)
        result.Append("             $ektron('#pleaseWait').modalHide(); " & vbCrLf)
        result.Append("		    	return false;" & vbCrLf)
        result.Append("		    }" & vbCrLf)
        result.Append("		}" & vbCrLf)

        'result.Append("function SetAction(Button) { " & vbCrLf)
        result.Append("valAndSaveCSubAssignments();" & vbCrLf)
        If (Not Utilities.IsAssetType(lContentType)) Then
            result.Append("if (typeof g_AssetHandler == ""object"")" & vbCrLf)
            result.Append("{" & vbCrLf)
        End If
        result.Append(" var sContentTitle = """";" & vbCrLf)
        result.Append(" if (Button != ""cancel"")" & vbCrLf)
        result.Append(" {" & vbCrLf)
        result.Append("     if (CheckTitle())" & vbCrLf)
        result.Append("     {" & vbCrLf)
        'content_title is not there in add multiple dms documents.
        result.Append("         if (document.forms[0].content_title != null) { " & vbCrLf)
        result.Append("            sContentTitle = document.forms[0].content_title.value.replace(/""/gi, ""'"");" & vbCrLf)
        result.Append("            sContentTitle = document.forms[0].content_title.value.replace(/\&/g, ""&amp;"");	" & vbCrLf)
        result.Append("         }" & vbCrLf)
        result.Append("     }" & vbCrLf)
        result.Append("     else" & vbCrLf)
        result.Append("     {" & vbCrLf)
        result.Append("$ektron('#pleaseWait').modalHide(); " & vbCrLf)
        result.Append("         buttonPressed = false;" & vbCrLf)
        result.Append("         return (false);" & vbCrLf)
        result.Append("     }" & vbCrLf)
        result.Append(" }" & vbCrLf)
        'TODO See VSS rev71 8/25/05 by Chandra, actually by Ken Yee to fix some problem w/ calling setFormPostInfo
        result.Append("	  var bDMSNoEditor = false;" & vbCrLf)
        If m_SelectedEditControl <> "ContentDesigner" Then
            result.Append("   var objTeaser = eWebEditPro.instances[""content_teaser""];" & vbCrLf)
            result.Append("             if (objTeaser && objTeaser.isEditor()){" & vbCrLf)
            result.Append("					if (!objTeaser.save()) { " & vbCrLf)
            result.Append("						buttonPressed = false; " & vbCrLf)
            result.Append("						return (false); " & vbCrLf)
            result.Append("						} " & vbCrLf)
            'result.Append("				    document.frmMain.content_teaser.value = eWebEditPro.instances[""content_teaser""].editor.GetContent(""htmlbody""); " & vbCrLf)
            result.Append("				} " & vbCrLf)
        Else
			result.Append("				if (""object"" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances) { " & vbCrLf)
            result.Append("					bDMSNoEditor = true;" & vbCrLf)
			result.Append("					objTeaser = Ektron.ContentDesigner.instances[""content_teaser""]; " & vbCrLf)
            result.Append("					if (objTeaser) " & vbCrLf)
            result.Append("					{ " & vbCrLf)
			result.Append("					document.forms[0].content_teaser.value = objTeaser.getContent();  " & vbCrLf)
			result.Append("					} " & vbCrLf)
            result.Append("				} " & vbCrLf)
        End If
        result.Append("				else { " & vbCrLf)
        result.Append("					bDMSNoEditor = true;" & vbCrLf)
        result.Append("                 if(document.forms[0].content_teaser && document.forms[0].ewepcontent_teaser)" & vbCrLf)
        result.Append("                 {" & vbCrLf)
        result.Append("					    document.forms[0].content_teaser.value = document.forms[0].ewepcontent_teaser.value;" & vbCrLf)
        result.Append("                 }" & vbCrLf)
        result.Append("				} " & vbCrLf)



        If (m_strAssetFileName.Trim <> "") Then
            result.Append("var strAssetTitle='" + m_strAssetFileName.Replace("'", "\'") & "';" & vbCrLf)
        Else
            result.Append("var strAssetTitle=sContentTitle;" & vbCrLf)
        End If
        If (m_strType = "update") Then

            If ((Request.QueryString("multi") Is Nothing Or "" = Request.QueryString("multi")) And Me.content_edit_data.ContType = CMSContentType_Media) Then
                'result.Append("if (Button == 'publish'){ g_AssetHandler.SetPostEventSave(false); } " & vbCrLf)
                result.Append("if (!saveMultimediaObjectsXML(Button)){ return false; }" & vbCrLf)
            End If
            'Else
            '    If g_ContentTypeSelected = CMSContentType_Media Then
            '        result.Append("if (Button == 'publish'){ g_AssetHandler.SetPostEventSave(false); } " & vbCrLf)
            '    End If
        End If
        If m_strType = "multiple,add" Then
            result.Append(" if (Button != ""cancel"")" & vbCrLf)
            result.Append("     DocumentUpload(); " & vbCrLf)
        End If
        'result.Append("	if ((Button == 'checkin') || (Button == 'publish')) {" & vbCrLf)
        'ap result.Append("		setFormPostInfo(Button);" & vbCrLf)
        result.Append("             if (Button === ""cancel"") { if (document.getElementById('fileUploadWrapper') != null) { document.getElementById('fileUploadWrapper').innerHTML = '<input type=""file"" id=""fileupload"" />';} }" & vbCrLf)
        result.Append("             if (Button != ""cancel"")" & vbCrLf)
        result.Append("				    DisplayHoldMsg_Local(true); " & vbCrLf)
        ' Corrupting viewstate causes an ASP.NET error on postback when telerik radEditor was on the page.
        ' This mechanism of fooling the viewstate was used when post back was to a different page.
        'result.Append("		 		document.forms[0].__VIEWSTATE.name = 'NOVIEWSTATE';" & vbCrLf)
        Dim queryStr As String = IIf(Me.m_bClose, "?close=true", "")
        If (Not Request.QueryString("pullapproval") Is Nothing) AndAlso (Request.QueryString("pullapproval").Length > 0) Then
            If (queryStr.Length > 0) Then
                queryStr &= "&pullapproval=" & Request.QueryString("pullapproval")
            Else
                queryStr &= "?pullapproval=" & Request.QueryString("pullapproval")
            End If
        End If
        If (Not Request.QueryString("taxoverride") Is Nothing) AndAlso (Request.QueryString("taxoverride").Length > 0) Then
            If (queryStr.Length > 0) Then
                queryStr &= "&taxoverride=" & Request.QueryString("taxoverride")
            Else
                queryStr &= "?taxoverride=" & Request.QueryString("taxoverride")
            End If
        End If
        If (TaxonomySelectId > 0) Then
            If (queryStr.Length > 0) Then
                queryStr &= "&SelTaxonomyId=" & TaxonomySelectId
            Else
                queryStr &= "?SelTaxonomyId=" & TaxonomySelectId
            End If
        End If
        result.Append("		 		document.forms[0].action = ""processupload.aspx" & queryStr & """; " & vbCrLf)
        result.Append("		 		document.forms[0].editaction.value = Button; " & vbCrLf)
        If (Request.QueryString("FromEE") <> "") Then 'If the page is opened from Ektron Explorer we need to close the page instead of returning
            result.Append("document.forms[0].FromEE.value = 'true'; ")
        End If
        'result.Append("		 		document.forms[0].submit(); " & vbCrLf)
        result.Append("		 		ektronFormSubmit(); " & vbCrLf)
        result.Append("             return (false); " & vbCrLf)
        'result.Append("	}" & vbCrLf)
        'This is where you need to check for is publish HTML and publish HTML
        'supported and change button name to publishType,checkInType and saveType
        'ap'If publishAsHtml = True Then
        'ap'    result.Append("var strButtonAction;" & vbCrLf)
        'ap'    result.Append("	if (Button == 'checkin') " & vbCrLf)
        'ap'    result.Append("strButtonAction = 'checkintype';" & vbCrLf)
        'ap'    result.Append("	if (Button == 'publish') " & vbCrLf)
        'ap'    result.Append("strButtonAction = 'publishtype';" & vbCrLf)
        'ap'    result.Append("	if (Button == 'save') " & vbCrLf)
        'ap'    result.Append("strButtonAction = 'savetype';" & vbCrLf)
        'ap'    result.Append("	if (Button == 'workoffline') " & vbCrLf)
        'ap'    result.Append("strButtonAction = 'workoffline';" & vbCrLf)
        'ap'    result.Append(" if (!g_AssetHandler.SetAction(strButtonAction, strAssetTitle) ")
        'ap'Else
        'ap'    result.Append(" if (!g_AssetHandler.SetAction(Button, strAssetTitle) ")
        'ap'End If
        'ap'result.Append(")" & vbCrLf)

        'ap'result.Append(" {" & vbCrLf)

        'ap'result.Append("     SetFullScreenView(true);" & vbCrLf)
        'ap'result.Append("     return (false);" & vbCrLf)
        'ap'result.Append(" }" & vbCrLf)
        If (Not Utilities.IsAssetType(lContentType)) Then
            result.Append("}" & vbCrLf)
        End If
        result.Append("if (""workoffline"" == Button)" & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append(" document.forms[0].elements[""type""].value = """";" & vbCrLf)
        result.Append("}" & vbCrLf)
        result.Append("if (""savelocalcopy"" == Button)" & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append(" document.forms[0].elements[""type""].value = """";" & vbCrLf)
        result.Append("}" & vbCrLf)
        result.Append("    if (buttonPressed != false) { " & vbCrLf)
        result.Append("	    return (false); " & vbCrLf)
        result.Append("    } " & vbCrLf)
        If (IsMac AndAlso m_SelectedEditControl <> "ContentDesigner") Then
            result.Append("    buttonPressed = true; " & vbCrLf)
            result.Append("    if (Button == ""cancel"") { " & vbCrLf)
            result.Append("    ResizeFrame(1); // Show the navigation-tree frame. " & vbCrLf)
            result.Append("    for (iLoop = 1; iLoop <= document.forms[0].numberoffields.value; iLoop++) { " & vbCrLf)
            result.Append("					eval(""document.forms[0].hiddencontent"" + iLoop + "".value = ''""); " & vbCrLf)
            result.Append("				} " & vbCrLf)
            result.Append("				document.forms[0].editaction.value = Button; " & vbCrLf)
            result.Append("				document.forms[0].submit(); " & vbCrLf) ' no ektronFormSubmit needed
            result.Append("				return (false); " & vbCrLf)
            result.Append("			} " & vbCrLf)
            result.Append("		if (!ValidateMeta(0))" & vbCrLf)
            result.Append("		{" & vbCrLf)
            result.Append("			buttonPressed = false;" & vbCrLf)
            result.Append("			return false;" & vbCrLf)
            result.Append("		}" & vbCrLf)
            result.Append(" editorEstimateContentSize = false;" & vbCrLf)
            result.Append(" if (false==CheckContentSize())" & vbCrLf)
            result.Append(" {" & vbCrLf)
            result.Append("   buttonPressed =false;" & vbCrLf)
            result.Append("   return false;" & vbCrLf)
            result.Append(" }" & vbCrLf)
            result.Append("	if(!CheckAllRequiredFields()){ " & vbCrLf)
            result.Append("     buttonPressed = false;" & vbCrLf)
            result.Append(" }" & vbCrLf)
            result.Append("			if ((ecmMetaComplete == 0) && (Button == ""publish"")) { " & vbCrLf)
            result.Append("				DisplayMetaIncomplete(); " & vbCrLf)
            result.Append("				buttonPressed = false; " & vbCrLf)
            result.Append("				return (false); " & vbCrLf)
            result.Append("			} " & vbCrLf)
            result.Append("			if (CheckTitle()) { " & vbCrLf)
            result.Append("				DisplayHoldMsg_Local(true);	 " & vbCrLf)
            result.Append("				var SavePosition; " & vbCrLf)
            result.Append("				var SaveContentLength; " & vbCrLf)
            result.Append("				var SaveSearchLength; " & vbCrLf)
            result.Append("				var HowMuchToSave; " & vbCrLf)
            result.Append("				var iLoop; " & vbCrLf)
            result.Append("				regexp1 = /""/gi; " & vbCrLf)
            'content_title is not there in add multiple dms documents.
            result.Append("             if (document.forms[0].content_title != null) { " & vbCrLf)
            result.Append("				   document.forms[0].content_title.value = document.forms[0].content_title.value.replace(regexp1, ""'""); " & vbCrLf)
            result.Append("             } " & vbCrLf)
            result.Append("				document.forms[0].content_comment.value = document.forms[0].content_comment.value.replace(regexp1, ""'""); " & vbCrLf)
            result.Append("				var saveContentObj; " & vbCrLf)
            result.Append("				var saveSearchObj; " & vbCrLf)
            result.Append("				saveContentObj = """"; " & vbCrLf)
            result.Append("				saveSearchObj = """"; " & vbCrLf)
            result.Append("				iLoop = 1; " & vbCrLf)
            If (IsMac AndAlso Not IsBrowserIE AndAlso m_SelectedEditControl <> "ContentDesigner") Then
                result.Append("				saveContentObj = src; " & vbCrLf)
            Else
                result.Append("				saveContentObj = document.forms[0].content_html.value; " & vbCrLf)
            End If
            result.Append("				saveSearchObj = $ektron.removeTags(saveContentObj); " & vbCrLf)
            result.Append("				SaveContentLength = saveContentObj.length; " & vbCrLf)
            result.Append("				SaveSearchLength = saveSearchObj.length; " & vbCrLf)
            result.Append("				for (iLoop = 1; iLoop <= document.forms[0].numberoffields.value; iLoop++) { " & vbCrLf)
            result.Append("					eval(""document.forms[0].hiddencontent"" + iLoop + "".value = ''""); " & vbCrLf)
            result.Append("				} " & vbCrLf)
            result.Append("				iLoop = 1; " & vbCrLf)
            result.Append("				for(SavePosition = 0; SavePosition < SaveContentLength; SavePosition += 65000) { " & vbCrLf)
            result.Append("					if ((SaveContentLength - SavePosition) < 65000) { " & vbCrLf)
            result.Append("						HowMuchToSave = (SaveContentLength - SavePosition); " & vbCrLf)
            result.Append("					} " & vbCrLf)
            result.Append("					else { " & vbCrLf)
            result.Append("						HowMuchToSave = 65000; " & vbCrLf)
            result.Append("					} " & vbCrLf)
            result.Append("					eval(""document.forms[0].hiddencontent"" + iLoop + "".value = saveContentObj.substring("" + SavePosition + "","" + (SavePosition + HowMuchToSave) + "");""); " & vbCrLf)
            result.Append("					iLoop += 1; " & vbCrLf)
            result.Append("				} " & vbCrLf)
            result.Append("				iLoop = 1; " & vbCrLf)
            result.Append("				for(SavePosition = 0; SavePosition < SaveSearchLength; SavePosition += 65000) { " & vbCrLf)
            result.Append("					if ((SaveSearchLength - SavePosition) < 65000) { " & vbCrLf)
            result.Append("						HowMuchToSave = (SaveSearchLength - SavePosition); " & vbCrLf)
            result.Append("					} " & vbCrLf)
            result.Append("					else { " & vbCrLf)
            result.Append("						HowMuchToSave = 65000; " & vbCrLf)
            result.Append("					} " & vbCrLf)
            result.Append("					eval(""document.forms[0].searchtext"" + iLoop + "".value = saveSearchObj.substring("" + SavePosition + "","" + (SavePosition + HowMuchToSave) + "");""); " & vbCrLf)
            result.Append("					iLoop += 1; " & vbCrLf)
            result.Append("				} " & vbCrLf)
            result.Append("				document.forms[0].hiddencontentsize.value = SaveContentLength; " & vbCrLf)
            result.Append("				document.forms[0].hiddensearchsize.value = SaveSearchLength; " & vbCrLf)
            If ((InStr(1, UCase(Request.ServerVariables("http_user_agent")), "MSIE") = 0) And (InStr(1, UCase(Request.ServerVariables("http_user_agent")), "4.7") > 0)) Then
                result.Append("					document.forms[0].netscape.value = """"; " & vbCrLf)
            End If
            result.Append("				ResizeFrame(1); // Show the navigation-tree frame. " & vbCrLf)
            result.Append("				document.forms[0].editaction.value = Button; " & vbCrLf)

            'result.Append("				document.forms[0].submit(); " & vbCrLf)
            result.Append("		 		ektronFormSubmit(); " & vbCrLf)
            result.Append("            return (false); " & vbCrLf)
            result.Append("			}   " & vbCrLf)
            result.Append("			else  { " & vbCrLf)
            result.Append("				buttonPressed = false; " & vbCrLf)
            result.Append("				return (false); " & vbCrLf)
            result.Append("			} " & vbCrLf)
            result.Append("		} " & vbCrLf)

        Else
            ValidateContentPanel.Visible = True
            result.Append("			buttonPressed = true; " & vbCrLf)
            result.Append("			if (Button == ""cancel"") { " & vbCrLf)
            result.Append("				DisplayHoldMsg_Local(true);" & vbCrLf)
            result.Append("				ResizeFrame(1); // Show the navigation-tree frame. " & vbCrLf)
            result.Append("				for (iLoop = 1; iLoop <= document.forms[0].numberoffields.value; iLoop++) { " & vbCrLf)
            result.Append("					eval(""document.forms[0].hiddencontent"" + iLoop + "".value = ''""); " & vbCrLf)
            result.Append("				} " & vbCrLf)
            result.Append("				document.forms[0].editaction.value = Button; " & vbCrLf)
            If m_SelectedEditControl <> "ContentDesigner" Then
                result.Append("				ShutdownImageEditor(); " & vbCrLf)
                result.Append("				//eWebEditPro.instances[""content_teaser""].editor.Clear(); " & vbCrLf)
                result.Append("				//eWebEditPro.instances[""content_html""].editor.Clear(); " & vbCrLf)
            End If
            ' The following TRY/CATCH pair is to catch the "unspecific error" that IE6 throw when hitting the cancel button
            ' in the onbeforeunload confirm box.
            ' similar cases were found at http://dbforums.com/showthread.php?threadid=483187
            result.Append("             try                                                 " & vbCrLf)
            result.Append("             {                                                   " & vbCrLf)
            result.Append("				    document.forms[0].submit();                      " & vbCrLf) ' no ektronFormSubmit needed for cancel
            result.Append("             }                                                   " & vbCrLf)
            result.Append("             catch (e)                                           " & vbCrLf)
            result.Append("             {                                                   " & vbCrLf)
            result.Append("                 // ignore the error if it fails to submit.      " & vbCrLf)
            result.Append("             }                                                   " & vbCrLf)
            result.Append("				return (false); " & vbCrLf)
            result.Append("			} " & vbCrLf)
            result.Append("			var bEditorNeeded = true;" & vbCrLf)
            result.Append("			if (""boolean"" == typeof bDMSNoEditor) {" & vbCrLf)
            result.Append("				if (true == bDMSNoEditor) {bEditorNeeded = false;}" & vbCrLf)
            result.Append("			} " & vbCrLf)
			result.Append("			else if (""object"" == typeof Ektron && Ektron.ContentDesigner && Ektron.ContentDesigner.instances) { " & vbCrLf)
            result.Append("				bEditorNeeded = false;" & vbCrLf)
            result.Append("			} " & vbCrLf)
            If m_SelectedEditControl <> "ContentDesigner" Then
                result.Append("			if (!eWebEditPro.isInstalled && bEditorNeeded) { " & vbCrLf)
                result.Append("				if(window.navigator.userAgent.search(""MSIE"") == -1) { " & vbCrLf)
                result.Append("					alert(""" & m_refMsg.GetMessage("js: netscape editor not loaded") & """); " & vbCrLf)
                result.Append("				} " & vbCrLf)
                result.Append("				else { " & vbCrLf)
                result.Append("					if(confirm(""" & m_refMsg.GetMessage("js: editor not loaded") & """)) { " & vbCrLf)
                result.Append("						self.location.reload(); " & vbCrLf)
                result.Append("					} " & vbCrLf)
                result.Append("				} " & vbCrLf)
                result.Append("				buttonPressed = false; " & vbCrLf)
                result.Append("				return (false); " & vbCrLf)
                result.Append("			} " & vbCrLf)
                result.Append("			ShutdownImageEditor();		 " & vbCrLf)
            End If
            'result.Append("		    if (!ValidateMeta(0))" & vbCrLf)
            'result.Append("		    {" & vbCrLf)
            'result.Append("		    	buttonPressed = false;" & vbCrLf)
            'result.Append("		    	return false;" & vbCrLf)
            'result.Append("		    }" & vbCrLf)
            result.Append(" editorEstimateContentSize = false;" & vbCrLf)
            result.Append(" if (false==CheckContentSize())" & vbCrLf)
            result.Append(" {" & vbCrLf)
            result.Append("   buttonPressed =false;" & vbCrLf)
            result.Append("   return false;" & vbCrLf)
            result.Append(" }" & vbCrLf)
            result.Append("	        if(!CheckAllRequiredFields()){ " & vbCrLf)
            result.Append("             buttonPressed = false;" & vbCrLf)
            result.Append("         }" & vbCrLf)
            result.Append("			if ((ecmMetaComplete == 0) && (Button == ""publish"")) { " & vbCrLf)
            result.Append("				DisplayMetaIncomplete(); " & vbCrLf)
            result.Append("				buttonPressed = false; " & vbCrLf)
            result.Append("				return (false); " & vbCrLf)
            result.Append("			} " & vbCrLf)
            result.Append("			if (CheckTitle(Button)) { " & vbCrLf)
            result.Append("				DisplayHoldMsg_Local(true); " & vbCrLf)
            result.Append("				var SavePosition; " & vbCrLf)
            result.Append("				var SaveContentLength; " & vbCrLf)
            result.Append("				var SaveSearchLength; " & vbCrLf)
            result.Append("				var HowMuchToSave; " & vbCrLf)
            result.Append("				var iLoop; " & vbCrLf)
            result.Append("				regexp1 = /""/gi; " & vbCrLf)
            'content_title is not there in add multiple dms documents.
            result.Append("             if (document.forms[0].content_title != null) { " & vbCrLf)
            result.Append("				    document.forms[0].content_title.value = document.forms[0].content_title.value.replace(regexp1, ""'""); " & vbCrLf)
            result.Append("                 document.forms[0].content_title.value = document.forms[0].content_title.value.replace(/\&/g, ""&amp;"");" & vbCrLf)
            result.Append("             } " & vbCrLf)
            result.Append("				document.forms[0].content_comment.value = document.forms[0].content_comment.value.replace(regexp1, ""'""); " & vbCrLf)
            result.Append("				document.forms[0].content_comment.value = document.forms[0].content_comment.value.replace(/\&/g, ""&amp;""); " & vbCrLf)
            result.Append("				var saveContentObj = new Object(); " & vbCrLf)
            result.Append("				var saveSearchObj = new Object(); " & vbCrLf)
            result.Append("				var saveTeaser = new Object(); " & vbCrLf)
            result.Append("				saveContentObj.value = """"; " & vbCrLf)
            result.Append("				saveSearchObj.value = """"; " & vbCrLf)
            result.Append("				iLoop = 1; " & vbCrLf)
            If m_SelectedEditControl <> "ContentDesigner" Then
                result.Append("             var objTeaser = eWebEditPro.instances[""content_teaser""];" & vbCrLf)
                result.Append("             if (objTeaser){" & vbCrLf)
                result.Append("					if (!objTeaser.save(undefined, undefined, undefined, false)) { " & vbCrLf) ' non-validating save, already done at ValidateContent
                result.Append("				        DisplayHoldMsg_Local(false); " & vbCrLf)
                result.Append("						buttonPressed = false; " & vbCrLf)
                result.Append("						return (false); " & vbCrLf)
                result.Append("						} " & vbCrLf)
                'result.Append("				    document.frmMain.content_teaser.value = objTeaser.editor.GetContent(""htmlbody""); " & vbCrLf)
                result.Append("				} " & vbCrLf)
                result.Append("             var objInstance = eWebEditPro.instances[""content_html""];" & vbCrLf)
                result.Append("            if (objInstance){" & vbCrLf)
                If bVer4Editor Then
                    result.Append("					saveContentObj.value = objInstance.editor.GetContent(""databody"");					 " & vbCrLf)

                Else
                    'result.Append("					eWebEditPro.instances[""content_html""].editor.ExecCommand(""cmdviewaswysiwyg"", """", 0); " & vbCrLf)
                    result.Append("					if (!objInstance.save(saveContentObj, undefined, undefined, false)) { " & vbCrLf) ' non-validating save, already done at ValidateContent
                    result.Append("				        DisplayHoldMsg_Local(false); " & vbCrLf)
                    result.Append("						buttonPressed = false; " & vbCrLf)
                    result.Append("						return (false); " & vbCrLf)
                    result.Append("						} " & vbCrLf)
                End If
                result.Append("}")
            End If
            If bVer4Editor = False Then
                If (m_strSchemaFile.Length > 0) Then
                    'result.Append("							var objInstance = eWebEditPro.instances['content_html']; " & vbCrLf)
                    result.Append("  							var objXmlDoc =  objInstance.editor.XMLProcessor(); " & vbCrLf)
                    result.Append("         					var sXMLString = saveContentObj.value; " & vbCrLf)
                    result.Append("							var sSchemaPath = """ & m_strSchemaFile & """; " & vbCrLf)
                    result.Append("   							var sNSTarget =  """ & m_strNamespaceFile & """; " & vbCrLf)
                    result.Append("							objXmlDoc.Validate(sXMLString, sSchemaPath, sNSTarget); " & vbCrLf)
                    result.Append("    						if(objXmlDoc.getPropertyInteger(""ErrorCode"") == 0) { " & vbCrLf)
                    result.Append("	    							// alert(""Passed!""); " & vbCrLf)
                    result.Append("   							} " & vbCrLf)
                    result.Append("    						else { " & vbCrLf)
                    result.Append("								DisplayHoldMsg_Local(false); " & vbCrLf)
                    result.Append("     						alert(objXmlDoc.getPropertyString(""ErrorReason"")); " & vbCrLf)
                    result.Append("								buttonPressed = false; " & vbCrLf)
                    result.Append("								return (false); " & vbCrLf)
                    result.Append("	     					} " & vbCrLf)
                End If
            End If
            result.Append("				//Workaround remove html and xml tags from the content." & vbCrLf)
            result.Append("				saveSearchObj.value = $ektron.removeTags(saveContentObj.value);" & vbCrLf)
            result.Append("				SaveContentLength = saveContentObj.value.length; " & vbCrLf)
            result.Append("				SaveSearchLength = saveSearchObj.value.length; " & vbCrLf)
            result.Append("				for (iLoop = 1; iLoop <= document.forms[0].numberoffields.value; iLoop++) { " & vbCrLf)
            result.Append("					eval(""document.forms[0].hiddencontent"" + iLoop + "".value = ''""); " & vbCrLf)
            result.Append("				} " & vbCrLf)
            result.Append("				iLoop = 1; " & vbCrLf)
            result.Append("				for(SavePosition = 0; SavePosition < SaveContentLength; SavePosition += 65000) { " & vbCrLf)
            result.Append("					if ((SaveContentLength - SavePosition) < 65000) { " & vbCrLf)
            result.Append("						HowMuchToSave = (SaveContentLength - SavePosition); " & vbCrLf)
            result.Append("					} " & vbCrLf)
            result.Append("					else { " & vbCrLf)
            result.Append("						HowMuchToSave = 65000; " & vbCrLf)
            result.Append("					} " & vbCrLf)
            result.Append("					eval(""document.forms[0].hiddencontent"" + iLoop + "".value = saveContentObj.value.substring("" + SavePosition + "","" + (SavePosition + HowMuchToSave) + "");""); " & vbCrLf)
            result.Append("					iLoop += 1; " & vbCrLf)
            result.Append("				} " & vbCrLf)
            result.Append("				iLoop = 1; " & vbCrLf)
            result.Append("				for(SavePosition = 0; SavePosition < SaveSearchLength; SavePosition += 65000) { " & vbCrLf)
            result.Append("					if ((SaveSearchLength - SavePosition) < 65000) { " & vbCrLf)
            result.Append("						HowMuchToSave = (SaveSearchLength - SavePosition); " & vbCrLf)
            result.Append("					} " & vbCrLf)
            result.Append("					else { " & vbCrLf)
            result.Append("						HowMuchToSave = 65000; " & vbCrLf)
            result.Append("					} " & vbCrLf)
            result.Append("					eval(""document.forms[0].searchtext"" + iLoop + "".value = saveSearchObj.value.substring("" + SavePosition + "","" + (SavePosition + HowMuchToSave) + "");""); " & vbCrLf)
            result.Append("					iLoop += 1; " & vbCrLf)
            result.Append("				} " & vbCrLf)
            result.Append("				document.forms[0].hiddencontentsize.value = SaveContentLength; " & vbCrLf)
            result.Append("				document.forms[0].hiddensearchsize.value = SaveSearchLength; " & vbCrLf)
            If ((InStr(1, UCase(Request.ServerVariables("http_user_agent")), "MSIE") = 0) And (InStr(1, UCase(Request.ServerVariables("http_user_agent")), "4.7") > 0)) Then
                result.Append("					document.forms[0].netscape.value = """"; " & vbCrLf)
            End If

            result.Append("				if (Button != ""save"") { " & vbCrLf)
            result.Append("					ResizeFrame(1); // Show the navigation-tree frame. " & vbCrLf)
            result.Append("				} " & vbCrLf)
            result.Append("				document.forms[0].editaction.value = Button; " & vbCrLf)
            'result.Append("				eWebEditPro.instances[""content_html""].editor.Clear(); " & vbCrLf)
            'replaced with next three lines
            result.Append("if (objInstance) {" & vbCrLf)
            result.Append("objInstance.editor.Clear();" & vbCrLf)
            result.Append("}" & vbCrLf)
            If (Utilities.IsAssetType(lContentType)) Then
                'posting done in ektexplorer, so just redirect to back_url
                If (Request.QueryString("FromEE") <> "") Then 'If the page is opened from Ektron Explorer we need to close the page instead of returning
                    result.Append("var loc = new String(location); ")
                    result.Append("var index = loc.lastIndexOf('?'); ")
                    result.Append("loc = loc.substring(0, index); ")
                    result.Append("index = loc.lastIndexOf('/'); ")
                    result.Append("loc = loc.substring(0, index);")
                    result.Append("loc = loc + '/close.aspx'; ")
                    result.Append("location = loc;")
                Else
                    result.Append("if (Button != ""save"") {")
                    result.Append("		ResizeFrame(1); // Show the navigation-tree frame." & vbCrLf)
                    If m_SelectedEditControl <> "ContentDesigner" Then
                        result.Append("		ShutdownImageEditor();" & vbCrLf)
                    End If
                    If (m_bClose) Then
                        result.Append("     var loc = 'close.aspx?reload=true';" & vbCrLf)
                    Else
                        result.Append("     var loc = '" & GetBackPage(m_intItemId) & "';" & vbCrLf)
                    End If
                    result.Append("     if (Button == ""publish"") { loc = loc.replace(""action=viewstaged"", ""action=view""); } " & vbCrLf)
                    result.Append("		location.replace(loc);" & vbCrLf)
                    result.Append("} else { var contentid = document.forms[0].content_id.value; location.replace('edit.aspx?close=" & Request.QueryString("close") & "&LangType=" & Request.QueryString("LangType") & "&id='+contentid+'" & "&type=update&mycollection=" & strMyCollection & "&addto" & strAddToCollectionType & "&back_file=" & back_file & "&back_action=" & back_action & "&back_folder_id=" & back_folder_id & "&back_id=" & back_id & "&back_form_id=" & back_form_id & "&back_LangType=" & back_LangType & back_callerpage & back_origurl & "');}")
                    'result.Append("} else location.reload();")
                End If
            Else
                'result.Append("				document.forms[0].submit(); " & vbCrLf)
                result.Append("		 		ektronFormSubmit(); " & vbCrLf)
            End If
            result.Append("				return (false); " & vbCrLf)
            result.Append("			} " & vbCrLf)
            result.Append("			buttonPressed = false; " & vbCrLf)
            result.Append("         $ektron('#pleaseWait').modalHide(); " & vbCrLf)
            result.Append("			return (false); " & vbCrLf)
            result.Append("		} " & vbCrLf)
        End If
        result.Append("		function CheckAllRequiredFields() {		 " & vbCrLf)
        result.Append("        var metafieldtype; " & vbCrLf)
        result.Append("     if(typeof document.forms[0].frm_validcounter != ""undefined""){" & vbCrLf)
        result.Append("			var EndLoop = document.forms[0].frm_validcounter.value; " & vbCrLf)
        result.Append("     }else{" & vbCrLf)
        result.Append("     var EndLoop =0;}" & vbCrLf)
        result.Append("			for (LoopCounter = 1; LoopCounter <= EndLoop; LoopCounter++) { " & vbCrLf)
        result.Append("				var field = ""document.forms[0].frm_text_"" + LoopCounter + "".value""; " & vbCrLf)
        result.Append("				var field1 = ""document.forms[0].frm_meta_required_"" + LoopCounter + "".value""; " & vbCrLf)
        result.Append("				eval(field + "" = Trim("" + field + "")""); " & vbCrLf)
        result.Append("				var meta_text = eval(field); " & vbCrLf)
        result.Append("				if (meta_text.length > 2000) { " & vbCrLf)
        result.Append("					alert(""" & m_refMsg.GetMessage("js: alert meta data over limit") & """ + "" "" + (meta_text.length - 2000)); " & vbCrLf)
        result.Append("					field = ""document.forms[0].frm_text_"" + LoopCounter + "".type""; " & vbCrLf)
        result.Append("					metafieldtype = eval(field); " & vbCrLf)
        result.Append("					if (metafieldtype != ""hidden"") { " & vbCrLf)
        result.Append("						field = ""document.forms[0].frm_text_"" + LoopCounter + "".focus()""; " & vbCrLf)
        result.Append("						eval(field); " & vbCrLf)
        result.Append("					}					 " & vbCrLf)
        result.Append("					return (false); " & vbCrLf)
        result.Append("				} " & vbCrLf)
        result.Append("				if ((meta_text == """") && (eval(field1) != 0)) {		 " & vbCrLf)
        result.Append("					SetMetaComplete( 0, " & m_intItemId & ");								 " & vbCrLf)
        result.Append("					return (false); " & vbCrLf)
        result.Append("				} " & vbCrLf)
        result.Append("			}		 " & vbCrLf)
        result.Append("			SetMetaComplete( 1, " & m_intItemId & ");	 " & vbCrLf)
        result.Append("			return true; " & vbCrLf)
        result.Append("		} " & vbCrLf)
        result.Append("	function SetMetaComplete(Flag, ID) { " & vbCrLf)
        result.Append("				ecmMetaComplete = Flag; " & vbCrLf)
        If (m_strType = "update") Then
            result.Append("		//this is for netscape popups " & vbCrLf)
            result.Append("		if (ID != " & m_intItemId & ") { " & vbCrLf)
            result.Append("			ecmMetaComplete = 0; " & vbCrLf)
            result.Append("			return; " & vbCrLf)
            result.Append("		} " & vbCrLf)
        Else
            'result.Append("try {if( parent.window.opener != null ) {" & vbCrLf)
            'result.Append("        parent.window.opener.DoRefreshEEWindow(); " & vbCrLf)
            'result.Append("}} catch(e){}" & vbCrLf)
            result.Append("	return; " & vbCrLf)
        End If

        ' If the edit page is called from showModalDialog, we'll return to it
        ' the title, teaser and text values, if they're available. This
        ' modal dialog is currently in use by the Ektron Explorer when the
        ' user chooses to 'edit' a piece of content form the main window
        'result.Append("try {if( parent.window.opener != null) {" & vbCrLf)
        'result.Append("      parent.window.opener.DoRefreshEEWindow()" & vbCrLf)
        'result.Append("}} catch(e){}" & vbCrLf)

        result.Append("}" & vbCrLf)
        Return (result.ToString)
    End Function
    Private Function EditProJS() As String
        ToggleViewJS()
        Dim result As New System.Text.StringBuilder
        result.Append("<script language=""JavaScript1.2""> " & vbCrLf)
        result.Append("<!-- " & vbCrLf)
        If m_SelectedEditControl <> "ContentDesigner" Then
            result.Append("eWebEditPro.parameters.reset();" & vbCrLf)
            result.Append("eWebEditPro.parameters.baseURL = """ & SitePath & """;" & vbCrLf)

            If lContentType <> 2 And (Len(save_xslt_file) > 0 Or bVer4Editor Or Len(editorPackage) > 0) Then
                result.Append("// If we have a SAVE XSLT then we need to tell the editor to dump document, which causes the Save XSLT to run " & vbCrLf)
                If (bVer4Editor Or Len(editorPackage) > 0) Then
                    result.Append("eWebEditPro.parameters.config = eWebEditProPath + ""cms_config.aspx?mode=dataentry&LangType=" & m_intContentLanguage & """; " & vbCrLf)
                Else 'use the old method
                    result.Append("eWebEditPro.parameters.config = eWebEditProPath + ""cms_config.aspx""; " & vbCrLf)
                    result.Append("eWebEditPro.parameters.xmlInfo = """ & xml_config & """; " & vbCrLf)
                End If
                result.Append("eWebEditPro.parameters.editorGetMethod = ""getDocument""; " & vbCrLf)
            ElseIf bIsFormDesign Then
                If InStr(m_strContentHtml, "class=""redvalidation""") > 0 OrElse InStr(m_strContentHtml, " ekv=") > 0 Then
                    result.Append("eWebEditPro.parameters.config = eWebEditProPath + ""cms_config.aspx?FormToolbarVisible=true""; " & vbCrLf)
                Else
                    result.Append("eWebEditPro.parameters.config = eWebEditProPath + ""cms_config.aspx?mode=formdesign&LangType=" & m_intContentLanguage & """; " & vbCrLf)
                End If
            Else
                result.Append("eWebEditPro.parameters.config = eWebEditProPath + ""cms_config.aspx""; " & vbCrLf)
                result.Append("eWebEditPro.parameters.xmlInfo = """ & xml_config & """; " & vbCrLf)
            End If
            result.Append("eWebEditPro.parameters.maxContentSize = " & iMaxContLength & "; " & vbCrLf)
            Dim strPath As String = ""
            If Len(content_stylesheet) > 0 Then
                strPath = GetServerPath() & SitePath & content_stylesheet

                result.Append("eWebEditPro.parameters.styleSheet = """ & strPath & """; " & vbCrLf)
            End If

            result.Append("function loadSegments() { " & vbCrLf)
            result.Append("var strContent; " & vbCrLf)
			'result.Append("var strTeaser; " & vbCrLf)
            'result.Append("var iLoop; " & vbCrLf)
            'result.Append("var designPackage; " & vbCrLf)

            'result.Append("designPackage = """";" & vbCrLf)
            'result.Append("for (iLoop = 1; iLoop <= " & iSegment2 & "; iLoop++){ " & vbCrLf)
            'result.Append("eval(""designPackage = designPackage + document.frmMain.hiddenpackage"" + iLoop + "".value;""); " & vbCrLf)
            'result.Append("} " & vbCrLf)

            'result.Append("strContent = """"; //set it to empty " & vbCrLf)
            'result.Append("for (iLoop = 1; iLoop <= " & iSegment & "; iLoop++) { " & vbCrLf)
            'result.Append("eval(""strContent = strContent + document.frmMain.hiddencontent"" + iLoop + "".value;""); " & vbCrLf)
            'result.Append("} " & vbCrLf)
			'result.Append("strTeaser = Trim(document.forms[0].contentteaser.value); " & vbCrLf)
            result.Append("if (eWebEditPro.instances[""content_html""]){" & vbCrLf)
            Dim JsStr As String
            JsStr = ""
            If (bVer4Editor) Then
                'JsStr = "eWebEditPro.instances[""content_html""].editor.SetContent(""datawhole"", designPackage, strContent);"
            Else
                'JsStr = "eWebEditPro.instances[""content_html""].load(strContent);"
                If (Not (IsNothing(xmlconfig_data))) Then
                    If (xmlconfig_data.EditXslt.Length = 0) Then
                        If (m_strContentHtml.Length = 0) Then
                            JsStr = "var ObjXml = eWebEditPro.instances[""content_html""].editor.XMLProcessor();"
                            JsStr = JsStr & "strContent = ObjXml.DocumentTemplate();"
                            JsStr = JsStr & "eWebEditPro.instances[""content_html""].load(strContent);"
                        End If
                    End If
                End If
            End If
            result.Append(JsStr & vbCrLf)
            result.Append("} " & vbCrLf)
            result.Append("} " & vbCrLf)

            result.Append("function DisableUpload(sEditorName) " & vbCrLf)
            result.Append("{ " & vbCrLf)
            result.Append("var objMedia = eWebEditPro.instances[sEditorName].editor.MediaFile(); " & vbCrLf)
            result.Append("if(objMedia != null) " & vbCrLf)
            result.Append("{ " & vbCrLf)
            result.Append("var objAutoUpload = objMedia.AutomaticUpload(); " & vbCrLf)
            result.Append("if(objAutoUpload != null) " & vbCrLf)
            result.Append("{ " & vbCrLf)
            result.Append("objAutoUpload.setProperty(""TransferMethod"", ""none""); " & vbCrLf)
            result.Append("var objMenu = eWebEditPro.instances[sEditorName].editor.Toolbars(); " & vbCrLf)
            result.Append("if(objMenu != null) " & vbCrLf)
            result.Append("{ " & vbCrLf)
            result.Append("var objCommand = objMenu.CommandItem(""cmdmfuuploadall""); " & vbCrLf)
            result.Append("if(objCommand != null) " & vbCrLf)
            result.Append("{ " & vbCrLf)
            result.Append("objCommand.setProperty(""CmdGray"", true); " & vbCrLf)
            result.Append("} " & vbCrLf)
            result.Append("} " & vbCrLf)
            result.Append("} " & vbCrLf)
            result.Append("} " & vbCrLf)
            result.Append("} " & vbCrLf)
        End If
        result.Append("function textCounter(field,cntfield,maxlimit) { " & vbCrLf)
        result.Append("if (field.value.length > maxlimit) { // if too long...trim it! " & vbCrLf)
        result.Append("field.value = field.value.substring(0, maxlimit); " & vbCrLf)
        result.Append("// otherwise, update 'characters left' counter " & vbCrLf)
        result.Append("} " & vbCrLf)
        result.Append("else " & vbCrLf)
        result.Append("{ " & vbCrLf)
        result.Append("cntfield.value = maxlimit - field.value.length; " & vbCrLf)
        result.Append("} " & vbCrLf)
        result.Append("} " & vbCrLf)

        result.Append("function SetDefault(textfield) { " & vbCrLf)
        result.Append("var resetfield = ""document.forms[0]."" + textfield + "".value""; " & vbCrLf)
        result.Append("var defaultfield = ""document.forms[0]."" + textfield + ""default.value""; " & vbCrLf)
        result.Append("var strTmp = eval(defaultfield); " & vbCrLf)
        result.Append("if (confirm(""" & m_refMsg.GetMessage("js: confirm restore default text") & """)) " & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append("document.forms.frmMain[textfield].value = strTmp; " & vbCrLf)
        result.Append("} " & vbCrLf)
        result.Append("} " & vbCrLf)
        result.Append("function outputSelected(selfield,textfield,seperator) { " & vbCrLf)
        result.Append("var retValue; " & vbCrLf)
        result.Append("var sel = getSelected(selfield); " & vbCrLf)
        result.Append("var strSel = """"; " & vbCrLf)
        result.Append("for (var item in sel) {        " & vbCrLf)
        result.Append("strSel += sel[item].value + seperator + "";""" & vbCrLf)
        result.Append("} " & vbCrLf)
        result.Append("strSel = strSel.substring(0, strSel.length-2); " & vbCrLf)
        result.Append("var ch = strSel.substring(0, 1); " & vbCrLf)
        result.Append("if (ch == seperator) { " & vbCrLf)
        result.Append("strSel = strSel.substring(1, strSel.length); " & vbCrLf)
        result.Append("} " & vbCrLf)
        result.Append("document.forms.frmMain[textfield].value = strSel; " & vbCrLf)
        result.Append("} " & vbCrLf)

        result.Append("function getSelected(opt) { " & vbCrLf)
        result.Append("var selected = new Array(); " & vbCrLf)
        result.Append("var index = 0; " & vbCrLf)
        result.Append("for (var intLoop = 0; intLoop < opt.length; intLoop++) { " & vbCrLf)
        result.Append("if ((opt[intLoop].selected) || " & vbCrLf)
        result.Append("(opt[intLoop].checked)) { " & vbCrLf)
        result.Append("index = selected.length; " & vbCrLf)
        result.Append("selected[index] = new Object; " & vbCrLf)
        result.Append("selected[index].value = opt[intLoop].value; " & vbCrLf)
        result.Append("selected[index].index = intLoop; " & vbCrLf)
        result.Append("} " & vbCrLf)
        result.Append("} " & vbCrLf)
        result.Append("return selected; " & vbCrLf)
        result.Append("} " & vbCrLf)
        result.Append("//--> " & vbCrLf)
        result.Append("</script> " & vbCrLf)
        Return (result.ToString)
    End Function
    Private Function GetEndDateActionStrings() As Hashtable
        Dim result As New Hashtable
        Dim strMsg As String = m_refMsg.GetMessage("Archive expire descrp")
        result.Add("SelectionSize", 3)
        If (strMsg = "") Then
            strMsg = "Archive and remove from site (expire)"
        End If
        result.Add("1", strMsg)
        result.Add("2", m_refMsg.GetMessage("Archive display descrp"))
        result.Add("3", m_refMsg.GetMessage("Refresh descrp"))
        Return (result)
    End Function
    Private Sub ToggleViewJS()
        Dim sJS As New System.Text.StringBuilder
        sJS.Append("<script language=""Javascript"">")
        sJS.Append("function ToggleView() {" & vbCrLf)
        sJS.Append("SetFullScreenView(!m_fullScreenView)" & vbCrLf)
        sJS.Append("}" & vbCrLf)
        sJS.Append("function SetFullScreenView(bViewFullScreen) {" & vbCrLf)
        sJS.Append("	// simply return if already in proper mode:" & vbCrLf)
        sJS.Append("	if (m_fullScreenView == bViewFullScreen) {	" & vbCrLf)
        sJS.Append("		return;" & vbCrLf)
        sJS.Append("	}" & vbCrLf)

        sJS.Append("	var tabArray = new Array(""_dvContent"", ""_dvSummary"", ""_dvAlias"", ""_dvMetadata"", ""_dvSchedule"", ""_dvComment"",""_dvSubscription"",""_dvTemplates"",""_dvTaxonomy"");" & vbCrLf)

        sJS.Append("	// handle add-new and update-existing conditions (added controls change offsets):" & vbCrLf)
        sJS.Append("	if (!m_initializedOffsets) {" & vbCrLf)
        sJS.Append("		var contentDivObj = document.getElementById(""dvContent"");" & vbCrLf)
        sJS.Append("		if (null != contentDivObj) {" & vbCrLf)
        sJS.Append("			if (m_stdVertOffset != contentDivObj.offsetTop) {" & vbCrLf)
        sJS.Append("				m_mainTblOffset += 3 + (contentDivObj.offsetTop - m_stdVertOffset);" & vbCrLf)
        sJS.Append("				m_stdVertOffset = contentDivObj.offsetTop;" & vbCrLf)
        sJS.Append("			}" & vbCrLf)
        sJS.Append("		}" & vbCrLf)
        sJS.Append("		m_initializedOffsets = true;" & vbCrLf)
        sJS.Append("	}" & vbCrLf)
        sJS.Append("	if (bViewFullScreen) {" & vbCrLf)
        sJS.Append("		document.getElementById(""ToggleViewBtn"").src=""" & AppImgPath & "movedown.gif"";" & vbCrLf)
        sJS.Append("		document.getElementById(""ToggleViewBtn"").title=""Goto Normal View"";" & vbCrLf)
        sJS.Append("		SetObjVisible(""upperTable"", false);" & vbCrLf)
        sJS.Append("		for (var i=0; i < tabArray.length; i++) {" & vbCrLf)
        sJS.Append("			SetObjAltOffset(tabArray[i], true);" & vbCrLf)
        sJS.Append("		}" & vbCrLf)
        sJS.Append("		m_fullScreenView = true;" & vbCrLf)
        sJS.Append("	} else {" & vbCrLf)
        sJS.Append("		document.getElementById(""ToggleViewBtn"").src=""" & AppImgPath & "moveup.gif"";" & vbCrLf)
        sJS.Append("		document.getElementById(""ToggleViewBtn"").title=""Goto Full-Screen View"";" & vbCrLf)
        sJS.Append("		SetObjVisible(""upperTable"", true);" & vbCrLf)
        sJS.Append("		for (var i=0; i < tabArray.length; i++) {" & vbCrLf)
        sJS.Append("			SetObjAltOffset(tabArray[i], false);" & vbCrLf)
        sJS.Append("		}" & vbCrLf)
        sJS.Append("		m_fullScreenView = false;" & vbCrLf)
        sJS.Append("	}" & vbCrLf)
        sJS.Append("	return false;" & vbCrLf)
        sJS.Append("}" & vbCrLf)
        sJS.Append("</script>" & vbCrLf)
        Page.ClientScript.RegisterStartupScript(GetType(Page), "ToggleView", sJS.ToString())
    End Sub

    Private Function GetBlogControls() As String
        Dim sbBlogControls As New StringBuilder
        If m_bIsBlog Then
            sbBlogControls.Append("<br/><input type=""hidden"" name=""postupdate_notify"" id=""postupdate_notify"" value=""" & Server.HtmlEncode(blog_data.NotifyURL) & """/><input type=""hidden"" name=""blogposttrackbackid"" id=""blogposttrackbackid"" value=""" & blog_post_data.TrackBackURLID.ToString() & """ /><input type=""hidden"" id=""isblogpost"" name=""isblogpost"" value=""true""/><br/><b>" & m_refMsg.GetMessage("lbl trackback url") & "</b><br/>")
            sbBlogControls.Append("<input type=""text"" name=""trackback"" id=""trackback"" size=""75"" value=""")
            If Not (blog_post_data Is Nothing) Then
                sbBlogControls.Append(Server.HtmlEncode(blog_post_data.TrackBackURL))
            End If
            sbBlogControls.Append(""" />")
            sbBlogControls.Append("<br/><br/>")
            sbBlogControls.Append("<input type=""checkbox"" id=""chkPingBack"" name=""chkPingBack"" ")
            If Not (blog_post_data Is Nothing) AndAlso blog_post_data.Pingback = True Then
                sbBlogControls.Append(" checked ")
            End If
            sbBlogControls.Append(" />")
            sbBlogControls.Append("&nbsp;" & m_refMsg.GetMessage("lbl blog ae ping") & "<input type=""hidden"" name=""blogpostchkPingBackid"" id=""blogpostchkPingBackid"" value=""" & blog_post_data.PingBackID.ToString() & """ />")
        End If
        Return sbBlogControls.ToString()
    End Function

#End Region

#Region "PROCESS EDITOR PAGE"

    Sub ProcessTags(ByVal Id As Long, ByVal langId As Integer)
        Dim tagtype As Ektron.Cms.Common.EkEnumeration.CMSObjectTypes = CMSObjectTypes.Content
        Dim defaultTags As TagData()
        Dim Tags As TagData()
        Dim m_refTagsApi As Ektron.Cms.API.Community.Tags = New Ektron.Cms.API.Community.Tags()
        Dim orginalTagIds As String
        Dim tagIdStr As String = ""
        Dim cTags As String = Page.Request.Form("currentTags")
        If (Not cTags Is Nothing) Then
            orginalTagIds = cTags.Trim().ToLower()
        Else
            orginalTagIds = ""
        End If
        'Assign all default user tags that are checked:
        'Remove tags that have been unchecked
        defaultTags = m_refTagsApi.GetDefaultTags(tagtype, -1)
        Tags = m_refTagsApi.GetTagsForObject(Id, tagtype)

        'Also, copy all users tags into defaultTags list
        'so that if they were removed, they can be deleted as well.
        Dim originalLength As Integer = defaultTags.Length
        ReDim Preserve defaultTags(defaultTags.Length + Tags.Length - 1)
        Tags.CopyTo(defaultTags, originalLength)

        If (defaultTags IsNot Nothing) Then
            Dim td As TagData
            For Each td In defaultTags
                tagIdStr = "userPTagsCbx_" + td.Id.ToString()
                If (Not Page.Request.Form(tagIdStr) Is Nothing) Then
                    If (Page.Request.Form(tagIdStr) = "on") Then
                        'if tag is checked, but not in current tag list, add it
                        If (Not orginalTagIds.Contains(td.Id.ToString() + ",")) Then
                            m_refTagsApi.AddTagToObject(td.Id, Id, tagtype, -1, langId)
                        End If
                    Else
                        'if tag is unchecked AND in current list, delete
                        If (orginalTagIds.Contains(td.Id.ToString() + ",")) Then
                            m_refTagsApi.DeleteTagOnObject(td.Id, Id, tagtype, 0)
                        End If
                    End If
                Else
                    'If tag checkbox has no postback value AND is in current tag list, delete it
                    If (orginalTagIds.Contains(td.Id.ToString() + ",")) Then
                        m_refTagsApi.DeleteTagOnObject(td.Id, Id, tagtype, 0)
                    End If
                End If
            Next

            ' Now add any new custom tags, that the user created:
            ' New tags are added to newTagNameHdn field in following format:  <TagText>~<LanguageID>;<TagText>~<LanguageID>

            If (Page.Request("newTagNameHdn") IsNot Nothing) Then
                Dim custTags As String = Page.Request("newTagNameHdn").ToString()
                Dim tagsep() As Char = {";"c}
                Dim aCustTags As String() = custTags.Split(tagsep)

                Dim languageId As Integer
                Dim langsep() As Char = {"~"c}
                Dim tag As String
                For Each tag In aCustTags
                    Dim tagPropArray As String() = tag.Split(langsep)
                    If (tagPropArray.Length > 1) Then
                        If (tagPropArray(0).Trim().Length > 0) Then
                            'Default language to -1.
                            '"ALL" option in drop down is 0 - switch to -1.
                            If (Not Int32.TryParse(tagPropArray(1), languageId)) Then
                                languageId = -1
                            End If
                            If (languageId = 0) Then
                                languageId = -1
                            End If

                            m_refTagsApi.AddTagToObject(tagPropArray(0), Id, tagtype, -1, languageId)
                        End If
                    End If
                Next
            End If
        End If
    End Sub


    Private Sub Process_FormSubmit()
        Dim dontCreateTask As Object
        Dim i As Integer = 0
        Dim y As Integer = 0
        Dim isub As Integer = 0
        Dim ValidCounter As Integer = 0
        Dim go_live As String = ""
        Dim end_date As String = ""
        Dim end_date_action As String = ""
        Dim strContent As String = ""
        Dim strSearchText As String = ""
        Dim ret As Boolean = False
        Dim strTaskName As String = ""
        Dim isAlreadyCreated As Boolean = False
        Dim site_data As SettingsData
        Dim page_subscription_data As New Collection
        Dim page_sub_temp As New Collection
        Dim arrSubscriptions As Array
        Dim blnusemessage As Boolean = False
        Dim blnusesummary As Boolean = False
        Dim blnusecontent As Boolean = False
        Dim sub_prop_data As New SubscriptionPropertiesData
        Dim strContentTeaser As String = ""
        Dim strRptDisplay As String = ""
        Dim strRpt As String = ""
        Dim bUpdateFormQuestions As Boolean = False
        Dim bAddHistogram As Boolean = False
        Dim bIsReportForm As Boolean = False
        Dim bLockedContentLink As Boolean = False
        Dim strContentTitle As String = Request.Form("content_title")
        Dim subtype As EkEnumeration.CMSContentSubtype = CMSContentSubtype.AllTypes

        Try

            dontCreateTask = Request.Form("createtask")
            m_strType = Request.Form("type")

            If "ContentDesigner" = m_SelectedEditControl Then
                Dim strResponse As String = Request.Form("response")
                Select Case strResponse
                    Case "message"
                        Dim strFormDesign As String
                        Dim strXsltDesign As String
                        Dim strFieldList As String
                        Dim strViewXslt As String
                        strFormDesign = m_ctlContentDesigner.Content
                        strXsltDesign = m_ctlSummaryDesigner.Content
						strFieldList = m_refContApi.TransformXsltPackage(strFormDesign, _
						 Server.MapPath(m_ctlSummaryDesigner.ScriptLocation & "DesignToFieldList.xslt"), True)

						Dim objXsltArgs As New System.Xml.Xsl.XsltArgumentList()
						objXsltArgs.AddParam("srcPath", "", m_ctlSummaryDesigner.ScriptLocation)

						strViewXslt = m_refContApi.XSLTransform("<root>" & strXsltDesign & "<ektdesignpackage_list>" & strFieldList & "</ektdesignpackage_list></root>", _
						 Server.MapPath(m_ctlSummaryDesigner.ScriptLocation & "DesignToViewXSLT.xslt"), True, False, objXsltArgs)
                        Dim sbFormResponse As New StringBuilder
                        With sbFormResponse
                            .Append("<ektdesignpackage_forms><ektdesignpackage_form><ektdesignpackage_designs><ektdesignpackage_design>")
                            .Append(strXsltDesign)
                            .Append("</ektdesignpackage_design></ektdesignpackage_designs><ektdesignpackage_lists><ektdesignpackage_list>")
                            .Append(strFieldList)
                            .Append("</ektdesignpackage_list></ektdesignpackage_lists><ektdesignpackage_views><ektdesignpackage_view></ektdesignpackage_view><ektdesignpackage_view>")
                            .Append(strViewXslt)
                            .Append("</ektdesignpackage_view></ektdesignpackage_views></ektdesignpackage_form></ektdesignpackage_forms>")
                        End With
                        strContentTeaser = sbFormResponse.ToString
                    Case "redirect"
                        strContentTeaser = m_ctlFormResponseRedirect.Content
                    Case "transfer"
                        strContentTeaser = m_ctlFormResponseTransfer.Content
                    Case Else
                        strContentTeaser = m_ctlSummaryDesigner.Content
                End Select
            Else
                strContentTeaser = Request.Form("content_teaser")
            End If

            strRptDisplay = Request.Form("report_display_type") ' Same Window = 1 and New Window = 0
            If Left(strRptDisplay, 1) = "," Then
                strRptDisplay = Right(strRptDisplay, Len(strRptDisplay) - 1)
            End If
            strRpt = Request.Form("report_type") ' Data Table = 1, Bar Chart = 2, Pie Chart = 3 and Combined = 4
            If Left(strRpt, 1) = "," Then
                strRpt = Right(strRpt, Len(strRpt) - 1)
            End If
            If (strRptDisplay = "1") Then
                strRptDisplay = "_self"
            Else
                strRptDisplay = "_blank"
            End If

            If Request.Form("response") = "report" Then
                bIsReportForm = True
                strContentTeaser = "<root><EktReportFormData/><RedirectionLink>"
                strContentTeaser = strContentTeaser & "<a href=""poll.aspx"" id=""" & strRpt & """"
                'If Len(strRptDisplay) > 0 Then
                strContentTeaser = strContentTeaser & " target = """ & strRptDisplay & """"
                'End If
                strContentTeaser = strContentTeaser & "></a>"
                strContentTeaser = strContentTeaser & "</RedirectionLink></root>"
            End If

            Dim acMetaInfo(3), MetaSelect, MetaSeparator As Object
            Dim MetaTextString As String = ""
            If (Request.Form("frm_validcounter") <> "") Then
                ValidCounter = CInt(Request.Form("frm_validcounter"))
            Else
                ValidCounter = 0
            End If
            page_meta_data = New Collection
            For i = 1 To ValidCounter
                acMetaInfo(1) = Request.Form("frm_meta_type_id_" & i)
                acMetaInfo(2) = Request.Form("content_id")
                MetaSeparator = Request.Form("MetaSeparator_" & i)
                MetaSelect = Request.Form("MetaSelect_" & i)
                If (MetaSelect) Then
                    MetaTextString = Replace(Request.Form("frm_text_" & i), ", ", MetaSeparator)
                    If (Left(MetaTextString, 1) = MetaSeparator) Then
                        MetaTextString = Right(MetaTextString, (Len(MetaTextString) - 1))
                    End If

                    acMetaInfo(3) = MetaTextString
                Else
                    myMeta = Request.Form("frm_text_" & i)
                    myMeta = Server.HtmlDecode(myMeta)
                    MetaTextString = Replace(myMeta, ";", MetaSeparator)
                    myMeta = Server.HtmlEncode(MetaTextString)
                    acMetaInfo(3) = MetaTextString
                End If
                page_meta_data.Add(acMetaInfo, CInt(i))
                ReDim acMetaInfo(3)
            Next
            If Request.Form("isblogpost") <> "" Then 'isblogpost
                i = i + 1
                acMetaInfo(1) = Request.Form("blogposttagsid")
                acMetaInfo(2) = Request.Form("content_id")
                MetaSeparator = ";"
                acMetaInfo(3) = Request.Form("blogposttags")
                page_meta_data.Add(acMetaInfo, CInt(i))
                ReDim acMetaInfo(3)

                i = i + 1
                acMetaInfo(1) = Request.Form("blogpostcatid")
                acMetaInfo(2) = Request.Form("content_id")
                MetaSeparator = ";"
                If Request.Form("blogpostcatlen") > 0 Then
                    MetaTextString = ""
                    For y = 0 To Request.Form("blogpostcatlen")
                        If Request.Form("blogcategories" & y.ToString()) <> "" Then
                            MetaTextString &= Replace(Request.Form("blogcategories" & y.ToString()), ";", "~@~@~") & ";"
                        End If
                    Next
                    If MetaTextString.ToString.EndsWith(";") Then
                        MetaTextString = Left(MetaTextString, (Len(MetaTextString) - 1))
                    End If
                    acMetaInfo(3) = MetaTextString
                Else
                    acMetaInfo(3) = ""
                End If
                page_meta_data.Add(acMetaInfo, CInt(i))
                ReDim acMetaInfo(3)

                i = i + 1
                acMetaInfo(1) = Request.Form("blogposttrackbackid")
                acMetaInfo(2) = Request.Form("content_id")
                MetaSeparator = ";"
                acMetaInfo(3) = Request.Form("trackback")
                page_meta_data.Add(acMetaInfo, CInt(i))
                ReDim acMetaInfo(3)

                i = i + 1
                acMetaInfo(1) = Request.Form("blogpostchkpingbackid")
                acMetaInfo(2) = Request.Form("content_id")
                MetaSeparator = ";"
                If Request.Form("chkpingback") <> "" Then
                    acMetaInfo(3) = 1
                Else
                    acMetaInfo(3) = 0
                End If
                page_meta_data.Add(acMetaInfo, CInt(i))
                ReDim acMetaInfo(3)
            End If

            sub_prop_data.BreakInheritance = True
            If Len(Request.Form("send_notification_button")) Then
                sub_prop_data.SendNextNotification = True
                sub_prop_data.SuspendNextNotification = False
            Else
                sub_prop_data.SendNextNotification = False
            End If
            Select Case Request.Form("notify_option")
                Case ("Always")
                    sub_prop_data.NotificationType = SubscriptionPropertyNotificationTypes.Always

                Case ("Initial")
                    sub_prop_data.NotificationType = SubscriptionPropertyNotificationTypes.Initial
                    If Not (m_strType = "update") Then ' if new, then set flag to email out
                        sub_prop_data.SendNextNotification = True
                        sub_prop_data.SuspendNextNotification = False
                    Else
                        If Len(Request.Form("send_notification_button")) Then
                            sub_prop_data.SendNextNotification = True
                            sub_prop_data.SuspendNextNotification = False
                        Else
                            sub_prop_data.SendNextNotification = False
                        End If
                    End If
                Case ("Never")
                    sub_prop_data.NotificationType = SubscriptionPropertyNotificationTypes.Never
            End Select
            If Len(Request.Form("suspend_notification_button")) Then
                sub_prop_data.SuspendNextNotification = True
                sub_prop_data.SendNextNotification = False
            Else
                sub_prop_data.SuspendNextNotification = False
            End If
            sub_prop_data.OptOutID = Request.Form("notify_optoutid")
            If Len(Request.Form("use_message_button")) Then
                sub_prop_data.DefaultMessageID = Request.Form("notify_messageid")
            Else
                sub_prop_data.DefaultMessageID = 0
            End If
            If Len(Request.Form("use_summary_button")) Then
                sub_prop_data.SummaryID = 1
            Else
                sub_prop_data.SummaryID = 0
            End If
            If Len(Request.Form("use_content_button")) Then
                sub_prop_data.ContentID = Request.Form("frm_content_id")
            Else
                sub_prop_data.ContentID = 0
            End If
            sub_prop_data.UnsubscribeID = Request.Form("notify_unsubscribeid")

            If Request.Form("notify_url") <> "" Then
                sub_prop_data.URL = Request.Form("notify_url")
            Else
                sub_prop_data.URL = Request.ServerVariables("HTTP_HOST")
            End If

            If Request.Form("notify_weblocation") <> "" Then
                sub_prop_data.FileLocation = Server.MapPath(m_refContApi.AppPath & "subscriptions")
            Else
                sub_prop_data.FileLocation = Server.MapPath(m_refContApi.AppPath & "subscriptions")
            End If
            If Request.Form("notify_weblocation") <> "" Then
                sub_prop_data.WebLocation = Request.Form("notify_weblocation")
            Else
                sub_prop_data.WebLocation = "subscriptions"
            End If
            If Request.Form("notify_subject") <> "" Then
                sub_prop_data.Subject = Request.Form("notify_subject")
            Else
                sub_prop_data.Subject = ""
            End If
            If Request.Form("notify_emailfrom") <> "" Then
                sub_prop_data.EmailFrom = Request.Form("notify_emailfrom")
            Else
                sub_prop_data.EmailFrom = ""
            End If

            sub_prop_data.UseContentTitle = ""

            If Len(Request.Form("use_contentlink_button")) Then
                sub_prop_data.UseContentLink = 1
            Else
                sub_prop_data.UseContentLink = 0
            End If

            If Len(Trim(Request.Form("content_sub_assignments"))) Then
                arrSubscriptions = Split(Trim(Request.Form("content_sub_assignments")), " ", -1)
                If arrSubscriptions.Length > 0 Then
                    For isub = 0 To (arrSubscriptions.Length - 1)
                        page_sub_temp = New Collection
                        page_sub_temp.Add(CLng(Mid(arrSubscriptions(isub), 10)), "ID")
                        page_subscription_data.Add(page_sub_temp)
                    Next
                End If
            Else
                page_subscription_data = Nothing
            End If
            page_sub_temp = Nothing

            If (Request.Form("go_live") <> "") Then
                go_live = CDate(Trim(Request.Form("go_live"))) 'CDate(trim(AppUI.cmsConvertDateToNumbers(Request.Form("go_live"))))
            End If
            If (Request.Form("end_date") <> "") Then
                end_date = CDate(Trim(Request.Form("end_date")))
                end_date_action = Request.Form("end_date_action_radio")
            End If
            lContentType = Request.Form("content_type")
            lContentSubType = Request.Form("content_subtype")
            For i = 0 To m_AssetInfoKeys.Length - 1
                asset_info.Add(m_AssetInfoKeys(i), Trim(Request.Form("asset_" & m_AssetInfoKeys(i).ToLower)))
            Next

            page_content_data = New Collection
            If (Request.Form("content_id") <> "") Then
                page_content_data.Add(Request.Form("content_id"), "ContentID")
            Else
                page_content_data.Add(0, "ContentID")
            End If
            page_content_data.Add(Request.Form("content_language"), "ContentLanguage")

            If (lContentType = 1 OrElse lContentType = 3) AndAlso (lContentSubType = CMSContentSubtype.PageBuilderData OrElse lContentSubType = CMSContentSubtype.PageBuilderMasterData OrElse lContentSubType = CMSContentSubtype.WebEvent) Then

                Dim cb As ContentData
                If m_strType = "add" Then
                    If Request.QueryString("back_LangType") <> "" Then
                        m_refContApi.ContentLanguage = Request.QueryString("back_LangType")
                    Else
                        m_refContApi.ContentLanguage = m_refContApi.RequestInformationRef.DefaultContentLanguage
                    End If
                    cb = m_refContApi.GetContentById(Request.Form("content_id"), Ektron.Cms.Content.EkContent.ContentResultType.Published)
                    m_refContApi.ContentLanguage = m_intContentLanguage
                Else
                    cb = m_refContApi.GetContentById(Request.Form("content_id"), Ektron.Cms.Content.EkContent.ContentResultType.Staged)
                End If
                If cb IsNot Nothing Then
                    subtype = cb.SubType
                    If subtype = CMSContentSubtype.PageBuilderData Or subtype = CMSContentSubtype.PageBuilderMasterData Or subtype = CMSContentSubtype.WebEvent Then
                        strContent = cb.Html
                        If strContentTitle Is Nothing Then
                            strContentTitle = cb.Title
                        End If
                        strSearchText = strContentTeaser
                        page_content_data.Add(subtype, "ContentSubType")
                    End If
                End If
            End If
            If "ContentDesigner" = m_SelectedEditControl And subtype <> CMSContentSubtype.PageBuilderData And subtype <> CMSContentSubtype.PageBuilderMasterData And subtype <> CMSContentSubtype.WebEvent Then
                strContent = m_ctlContentDesigner.Content
                strSearchText = m_ctlContentDesigner.Text
            Else
                If (subtype <> CMSContentSubtype.PageBuilderData And subtype <> CMSContentSubtype.PageBuilderMasterData And subtype <> CMSContentSubtype.WebEvent) Then
                    i = 1
                    Do While (Len(Request.Form("hiddencontent" & i)) > 0)
                        strContent = strContent & Request.Form("hiddencontent" & i)
                        i = i + 1
                    Loop
                    i = 1
                    Do While (Len(Request.Form("searchtext" & i)) > 0)
                        strSearchText = strSearchText & Request.Form("searchtext" & i)
                        i = (i + 1)
                    Loop
                End If
            End If

            page_content_data.Add(lContentType, "ContentType")
            If (IsMac And Not IsBrowserIE AndAlso m_SelectedEditControl <> "ContentDesigner") Then
                'Ephox outputs entity name &nbsp; which causes error in Import/Export utility.
                'If we finde more entity name being used we should use code snippet from the eWebEditPro to clean.
                strContent = strContent.Replace("&nbsp;", "&#160;")
            End If
            If ((Not asset_info Is Nothing) AndAlso (lContentType = CMSContentType_Media)) Then
                strContent = Request.Form("content_html")
                page_content_data.Add(strContent, "MediaText")
            End If
            If (subtype <> CMSContentSubtype.PageBuilderData And subtype <> CMSContentSubtype.PageBuilderMasterData And subtype <> CMSContentSubtype.WebEvent) Then
                strContent = Utilities.WikiQLink(strContent, Request.Form("content_folder"))
            End If

            If (subtype = CMSContentSubtype.WebEvent) Then
                'we need to update the inner title to match this title, so we deserialize the event, update the field, and reserialize
                Dim xs As New System.Xml.Serialization.XmlSerializer(GetType(Ektron.Cms.Content.Calendar.EventPersistence.root))
                Dim reader As New System.Xml.XmlTextReader(strContent, System.Xml.XmlNodeType.Document, Nothing)
                Dim ev As Ektron.Cms.Content.Calendar.EventPersistence.root
                ev = xs.Deserialize(reader)

                ev.DisplayTitle = strContentTitle

                Dim stream As New System.IO.MemoryStream()
                Dim writersettings As New System.Xml.XmlWriterSettings()
                writersettings.OmitXmlDeclaration = True

                Using xmlWriter As System.Xml.XmlWriter = System.Xml.XmlWriter.Create(stream, writersettings)
                    xs.Serialize(xmlWriter, ev)
                End Using

                stream.Flush()
                stream.Position = 0
                Dim streamreader As New System.IO.StreamReader(stream)
                strContent = streamreader.ReadToEnd()
            End If

            page_content_data.Add(strContent, "ContentHtml")
            If (m_strType <> "update" OrElse (strContentTeaser.ToLower() = "<br /><!-- wiki summary -->")) Then
                Dim strippedTeaser As String = Utilities.StripHTML(strContentTeaser)
                If ((lContentType = 1 Or lContentType = 3) AndAlso (subtype <> CMSContentSubtype.PageBuilderData AndAlso subtype <> CMSContentSubtype.PageBuilderMasterData AndAlso subtype <> CMSContentSubtype.WebEvent)) Then
                    If Request.Form("xid") Is Nothing AndAlso (strContentTeaser.IndexOf("<img") = -1 AndAlso (strippedTeaser = "" Or strippedTeaser = "&#160;" Or strippedTeaser = "&nbsp;" Or (strippedTeaser.ToLower() = "<!-- wiki summary -->"))) Then
                        strContentTeaser = Utilities.AutoSummary(strContent)
                        If strContentTeaser <> "" Then
                            strContentTeaser = "<p>" & strContentTeaser & "</p>"
                        End If
                    End If
                End If
            End If
            If (Request.Form("chkLockedContentLink") IsNot Nothing AndAlso Request.Form("chkLockedContentLink") = "on") Then
                bLockedContentLink = True
            End If
            page_content_data.Add(bLockedContentLink, "LockedContentLink")
            page_content_data.Add(Request.Form("content_comment"), "Comment")
            page_content_data.Add(page_meta_data, "ContentMetadata")
            page_content_data.Add(strContentTeaser, "ContentTeaser")
            page_content_data.Add(Request.Form("content_folder"), "FolderID")

            page_content_data.Add(strSearchText, "SearchText")
            page_content_data.Add(go_live, "GoLive")
            page_content_data.Add(end_date, "EndDate")
            page_content_data.Add(end_date_action, "EndDateAction")
            Dim nAssetInfoArrayLBound As Integer = 0
            Dim nAssetInfoArrayUBound As Integer = -1
            Dim j As Integer = 1
            Dim strAssetInfo As String = ""
            Dim aryAssetInfoValue As Object
            Dim nArrayLBound As Object
            Dim nArrayUBound As Object
            Dim cAssetInfoArray As New Hashtable
            Dim strKeyName As String = ""
            For i = 0 To m_AssetInfoKeys.Length - 1
                strKeyName = m_AssetInfoKeys(i)
                strAssetInfo = Convert.ToString(asset_info(m_AssetInfoKeys(i)))
                page_content_data.Add(strAssetInfo.Replace("%2C", ","), strKeyName)
                If (0 = strAssetInfo.Length And nAssetInfoArrayUBound > nAssetInfoArrayLBound) Then
                    ' This information is not provided at all, so it's not inconsistent.
                    strAssetInfo = ""
                    For j = 1 To nAssetInfoArrayUBound - nAssetInfoArrayLBound
                        strAssetInfo += ","
                    Next

                End If
                '' Append a space so that an empty string will produce an array of one item
                '' rather than an array with no items. The value will be Trimmed later to
                '' remove the space.
                aryAssetInfoValue = Split(strAssetInfo & " ", ",")

                nArrayLBound = LBound(aryAssetInfoValue)
                nArrayUBound = UBound(aryAssetInfoValue)
                If i = LBound(m_AssetInfoKeys) Then
                    nAssetInfoArrayLBound = nArrayLBound
                    nAssetInfoArrayUBound = nArrayUBound
                End If
                If nAssetInfoArrayLBound = nArrayLBound And nAssetInfoArrayUBound = nArrayUBound Then
                    cAssetInfoArray.Add(strKeyName, aryAssetInfoValue)
                Else
                    Response.Redirect("reterror.asp?info=" & Server.UrlEncode("Inconsistent number of assets. Value=" & strAssetInfo), False)
                End If
            Next
            If nAssetInfoArrayLBound = nAssetInfoArrayUBound Then
                If 0 = Len(strContentTitle) Then
                    If Len(page_content_data("AssetFilename")) > 0 Then
                        strContentTitle = page_content_data("AssetFilename")
                    Else
                        strContentTitle = "No Title"
                    End If
                End If
            End If
            page_content_data.Add(strContentTitle, "ContentTitle")
            'If (Request.Form("frm_manalias") <> "") Then
            m_strManualAlias = Trim(CStr(Request.Form("frm_manalias")))
            m_strManualAliasExt = CStr(Request.Form("frm_manaliasExt"))
            'End If

            ast_frm_manaliasExt.Value = Request.Form("frm_manaliasExt")


            'Aliasing logic for 7.6 starts here
            m_prevManualAliasName = CStr(Request.Form("prev_frm_manalias_name"))
            m_prevManualAliasExt = CStr(Request.Form("prev_frm_manalias_ext"))
            m_currManualAliasName = m_strManualAlias
            m_currManualAliasExt = m_strManualAliasExt
            If (m_prevManualAliasName = "" And m_currManualAliasName <> "" Or m_prevManualAliasExt = "" And m_currManualAliasExt <> "") Then
                m_currManualAliasStatus = "New"
            ElseIf (m_prevManualAliasName <> "" And m_currManualAliasName <> "" And (m_currManualAliasName <> m_prevManualAliasName Or m_prevManualAliasExt <> m_currManualAliasExt)) Then
                m_currManualAliasStatus = "Modified"
            ElseIf (m_prevManualAliasName <> "" And m_currManualAliasName = "") Then
                m_currManualAliasStatus = "Deleted"
            Else
                m_currManualAliasStatus = "None"
            End If
            If (Request.Form("frm_manalias_id") <> "") Then
                m_intManualAliasId = CLng(Request.Form("frm_manalias_id"))
            End If

            page_content_data.Add(m_strManualAlias, "NewUrlAliasName")
            page_content_data.Add(m_intManualAliasId, "UrlAliasId")
            page_content_data.Add(m_strManualAliasExt, "NewUrlAliasExt")
            page_content_data.Add(m_currManualAliasStatus, "UrlAliasStatus")
            page_content_data.Add(m_prevManualAliasName, "OldUrlAliasName")
            page_content_data.Add(m_prevManualAliasExt, "OldUrlAliasExt")

            page_content_data.Add(m_strManualAlias, "ManualAlias")
            page_content_data.Add(m_intManualAliasId, "ManualAliasID")

            If (Request.Form("TaxonomyOverrideId") IsNot Nothing AndAlso Request.Form("TaxonomyOverrideId") <> 0) Then
                TaxonomyOverrideId = Request.Form("TaxonomyOverrideId")
                TaxonomyTreeIdList = TaxonomyOverrideId
            End If

            If (Request.Form(taxonomyselectedtree.UniqueID) IsNot Nothing AndAlso Request.Form(taxonomyselectedtree.UniqueID) <> "") Then
                TaxonomyTreeIdList = Request.Form(taxonomyselectedtree.UniqueID)
                If (TaxonomyTreeIdList.Trim.EndsWith(",")) Then
                    TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1)
                End If
            End If
            If (TaxonomyTreeIdList.Trim() = String.Empty AndAlso TaxonomySelectId > 0) Then
                TaxonomyTreeIdList = TaxonomySelectId
            End If
            page_content_data.Add(TaxonomyTreeIdList, "Taxonomy")
            page_content_data.Add(Request.Form("content_image"), "Image")
            Dim intContentId As Long = 0
            Dim iAsset As Integer = 0
            Dim strAssetInfoValue As String = ""
            For iAsset = nAssetInfoArrayLBound To nAssetInfoArrayUBound
                If nAssetInfoArrayLBound < nAssetInfoArrayUBound Then
                    For i = 0 To m_AssetInfoKeys.Length - 1
                        strKeyName = m_AssetInfoKeys(i)
                        strAssetInfoValue = cAssetInfoArray(strKeyName)(iAsset)
                        ' Commas were escaped as %2C, so restore them now. See assetevents.js
                        strAssetInfoValue = Trim(Replace(strAssetInfoValue, "%2C", ","))
                        If (DoesKeyExist(page_content_data, strKeyName)) Then
                            page_content_data.Remove(strKeyName)
                        End If
                        page_content_data.Add(strAssetInfoValue, strKeyName)
                    Next
                    page_content_data.Remove("ContentTitle")
                    If Len(strContentTitle) > 0 Then
                        page_content_data.Add(strContentTitle & " (" & page_content_data("AssetFilename") & ")", "ContentTitle")
                    ElseIf Len(page_content_data("AssetFilename")) > 0 Then
                        page_content_data.Add(page_content_data("AssetFilename"), "ContentTitle")
                    Else
                        page_content_data.Add("No Title", "ContentTitle")
                    End If
                End If
                If (Request.Form("xid") <> "" And Request.Form("templateSelect") <> "") Then
                    page_content_data.Add(Request.Form("xid"), "MultiXmlID")
                    page_content_data.Add(Request.Form("templateSelect"), "MultiTemplateID")
                ElseIf (Request.Form("templateSelect") <> "") Then
                    page_content_data.Add(0, "MultiXmlID")
                    If lContentSubType = EkEnumeration.CMSContentSubtype.PageBuilderData OrElse lContentSubType = EkEnumeration.CMSContentSubtype.PageBuilderMasterData Then
                        Dim templmodel As ITemplateModel = ObjectFactory.GetTemplateModel()
                        Dim templdat As TemplateData = templmodel.FindByID(Convert.ToInt64(Request.Form("templateSelect")))
                        If (templdat IsNot Nothing) Then
                            If (templdat.MasterLayoutID > 0) Then
                                templdat = templmodel.FindByContentID(templdat.MasterLayoutID)
                                page_content_data.Add(templdat.Id, "MultiTemplateID", Nothing, Nothing)
                            End If
                        End If
                    Else
                        page_content_data.Add(Request.Form("templateSelect"), "MultiTemplateID")
                    End If


                ElseIf (Request.Form("xid") <> "" And lContentSubType = CMSContentSubtype.WebEvent) Then
                    page_content_data.Add(Request.Form("xid"), "MultiXmlID")
                    page_content_data.Add(0, "MultiTemplateID")
                End If
                'If (Request.Form("ckAddFolderBreadCrumb") IsNot Nothing) Then
                '    If (Request.Form("ckAddFolderBreadCrumb") = "on" And Request.Form("previousInFolderBreadcrumb") <> "true") Then
                '        page_content_data.Add("add", "FolderBreadcrumb")
                '    End If
                'ElseIf (Request.Form("previousInFolderBreadcrumb") = "true") Then
                '    page_content_data.Add("delete", "FolderBreadcrumb")
                'End If

                If (m_strType = "update") Then
                    m_refContent.SaveContentv2_0(page_content_data)
                    intContentId = Request.Form("content_id")
                Else
                    If iAsset = nAssetInfoArrayLBound Then
                        If (Request.Form("AddQlink") = "AddQlink") Then
                            page_content_data.Add(True, "AddToQlink")
                        Else
                            page_content_data.Add(False, "AddToQlink")
                        End If
                        If (Request.Form("IsSearchable") = "IsSearchable") Then
                            page_content_data.Add(True, "IsSearchable")
                        Else
                            page_content_data.Add(False, "IsSearchable")
                        End If
                    End If

                    ' Update content flagging:
                    Dim flagDefSelObj As Object = Request.Form("FlaggingDefinitionSel")
                    If ((Not IsNothing(flagDefSelObj)) AndAlso IsNumeric(flagDefSelObj)) Then
                        page_content_data.Add(CType(flagDefSelObj.ToString, Long), "FlagDefId")
                    End If


                    intContentId = m_refContent.AddNewContentv2_0(page_content_data)
                End If

                If Not (Request.Form("suppress_notification")) <> "" Then
                    m_refContent.UpdateSubscriptionPropertiesForContent(intContentId, sub_prop_data)
                    m_refContent.UpdateSubscriptionsForContent(intContentId, page_subscription_data)
                Else
                    'm_refContent.SuppressNotification(intContentId)
                End If


                ' process tag info
                ProcessTags(intContentId, m_intContentLanguage)

                If (m_strPageAction = "checkin") Then
                    m_refContent.CheckIn(intContentId)
                End If
                If (m_strPageAction = "publish") Then
                    m_refContent.CheckIn(intContentId)
                    If (ret = False) Then
                        If (bIsReportForm) Then
                            If Request.Form("renewpoll") <> "" Then
                                If "true" = Request.Form("renewpoll").ToLower Then
                                    'this needs to be done before the histogram is updated with the new data.
                                    m_refContApi.EkModuleRef.UpdatePollRev(intContentId)
                                End If
                            End If
                            bUpdateFormQuestions = m_refContApi.EkModuleRef.UpdateFormFieldQuestions(CLng(Request.Form("content_id")), strContent)
                            ' Can't do this yet, we must only do it when actually published.
                            ' If this is done here, it invalidates an active form.
                            ''bAddHistogram = m_refContApi.EkModuleRef.AddNewFormHistogram(CLng(Request.Form("content_id")))
                        End If

                        site_data = m_refSiteApi.GetSiteVariables()

                        Dim PreapprovalGroupID As Long
                        Dim cPreApproval As New Collection
                        cPreApproval = m_refContent.GetFolderPreapprovalGroup(Request.Form("content_folder"))
                        PreapprovalGroupID = CLng(cPreApproval("UserGroupID"))

                        If PreapprovalGroupID > 0 Then
                            If dontCreateTask = "" Then
                                If m_intContentLanguage = 1 Then
                                    strTaskName = Request.Form("content_title") & intContentId & "_Task"
                                Else
                                    strTaskName = Request.Form("content_title") & intContentId & "_Task" & m_intContentLanguage
                                End If
                                m_refTask.ContentLanguage = m_intContentLanguage
                                m_refTask.LanguageID = m_intContentLanguage
                                isAlreadyCreated = m_refTask.IsTaskAlreadyCreated(intContentId)
                                If isAlreadyCreated = False Then
                                    m_refTask.TaskTitle = strTaskName ' Task name would be contentname + content id + _Task
                                    m_refTask.AssignToUserGroupID = PreapprovalGroupID  'Assigned to group defined by
                                    m_refTask.AssignedByUserID = CurrentUserID 'Assigned by person creating the task
                                    m_refTask.State = 1 'Not started
                                    m_refTask.ContentID = intContentId 'Content ID of the content being created
                                    m_refTask.Priority = 2 'Normal
                                    m_refTask.CreatedByUserID = CurrentUserID ' If task is hopping this will always be created by
                                    m_refTask.ContentLanguage = m_intContentLanguage
                                    m_refTask.LanguageID = m_intContentLanguage
                                    ret = m_refTask.AddTask()
                                    ret = m_refContent.SetContentState(intContentId, "T")
                                Else
                                    ret = m_refContent.SubmitForPublicationv2_0(intContentId, Request.Form("content_folder"))
                                End If

                            Else
                                ret = m_refContent.SubmitForPublicationv2_0(intContentId, Request.Form("content_folder"))
                            End If
                        Else
                            Dim strStatusBefore As String
                            Dim strStatusAfter As String
                            Dim colContentState As Collection

                            colContentState = m_refContent.GetContentStatev2_0(intContentId)
                            strStatusBefore = colContentState("ContentStatus")
                            ret = m_refContent.SubmitForPublicationv2_0(intContentId, Request.Form("content_folder"))
                            colContentState = m_refContent.GetContentStatev2_0(intContentId)
                            strStatusAfter = colContentState("ContentStatus")

                            If strStatusBefore <> strStatusAfter And "T" = strStatusAfter Then
                                blnShowTStatusMessage = True
                            End If
                            Dim markupPath As String = ""
                            Dim cacheidentifier As String = ""
                            Dim updateContent As String = ""
                            markupPath = Request.Form("ctlmarkup")
                            cacheidentifier = Request.Form("cacheidentifier")
                            If (markupPath IsNot Nothing AndAlso markupPath.Length > 0) Then
                                markupPath = Request.PhysicalApplicationPath & markupPath
                            End If
                            If (cacheidentifier IsNot Nothing AndAlso cacheidentifier.Length > 0) Then
                                If (HttpContext.Current.Cache(cacheidentifier) IsNot Nothing) Then
                                    HttpContext.Current.Cache.Remove(cacheidentifier)
                                End If
                            End If
                            Dim ekml As Object = Nothing
                            If markupPath IsNot Nothing AndAlso HttpContext.Current.Cache(markupPath) IsNot Nothing Then
                                ekml = HttpContext.Current.Cache(markupPath)
                                Dim api As New Ektron.Cms.UI.CommonUI.ApiSupport()
                                Dim results As Ektron.Cms.UI.CommonUI.ApiSupport.ContentResult = api.LoadContent(intContentId, False)
                                m_refContApi = New ContentAPI()
                                updateContent = Me.m_refContApi.FormatOutput(ekml.ContentFormat, Request.Form("ctltype"), results.Item)
                                updateContent = Me.m_refContApi.WrapAjaxToolBar(updateContent, results.Item, commparams)
                            Else
                                updateContent = colContentState("ContentHtml")
                            End If
                            If (Request.Form("ctlupdateid") IsNot Nothing AndAlso Request.Form("ctlupdateid") <> "") Then
                                Page.ClientScript.RegisterHiddenField("updatefieldcontent", updateContent)
                                Dim strJs As StringBuilder = New StringBuilder()
                                strJs.Append("<script language=""JavaScript1.2"" type=""text/javascript""> ").Append(vbCrLf)
                                strJs.Append(" if (top.opener != null) {").Append(vbCrLf)
                                strJs.Append("      var objUpdateField = top.opener.document.getElementById('" & Request.Form("ctlupdateid") & "');").Append(vbCrLf)
                                strJs.Append("      if (objUpdateField != null) { objUpdateField.innerHTML = document.getElementById(""updatefieldcontent"").value; }").Append(vbCrLf)
                                strJs.Append(" }").Append(vbCrLf)
                                If ((m_bClose) And (m_strPageAction <> "save")) Then
                                    strJs.Append("document.location.href = ""close.aspx"";").Append(vbCrLf)
                                End If
                                strJs.Append("</script>").Append(vbCrLf)
                                UpdateFieldJS.Text = strJs.ToString()
                                'Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "objUpdateField", strJs.ToString(), True)
                            End If
                        End If
                    End If


                    If (strAddToCollectionType = "menu") Then
                        If (strMyCollection <> "") Then
                            pagedata = New Collection
                            pagedata.Add(intContentId, "ItemID")
                            pagedata.Add("content", "ItemType")
                            pagedata.Add("self", "ItemTarget")
                            pagedata.Add("", "ItemLink")
                            pagedata.Add("", "ItemTitle")
                            pagedata.Add("", "ItemDescription")
                            ret = m_refContent.AddItemToEcmMenu(strMyCollection, pagedata)
                        End If
                    Else
                        If (strMyCollection <> "") Then
                            ret = m_refContent.AddItemToEcmCollection(strMyCollection, intContentId, m_intContentLanguage)
                        End If
                    End If
                End If
            Next
            If (m_strPageAction = "summary_save") Then
                Response.Redirect("edit.aspx?close=" & Request.QueryString("close") & "&LangType=" & m_intContentLanguage & "&id=" & intContentId & "&type=update&mycollection=" & strMyCollection & "&addto" & strAddToCollectionType & "&back_file=" & back_file & "&back_action=" & back_action & "&back_folder_id=" & back_folder_id & "&back_id=" & back_id & "&back_form_id=" & back_form_id & "&back_LangType=" & back_LangType & "&summary=1" & back_callerpage & back_origurl, False)
            ElseIf (m_strPageAction = "meta_save") Then
                Response.Redirect("edit.aspx?close=" & Request.QueryString("close") & "&LangType=" & m_intContentLanguage & "&id=" & intContentId & "&type=update&mycollection=" & strMyCollection & "&addto" & strAddToCollectionType & "&back_file=" & back_file & "&back_action=" & back_action & "&back_folder_id=" & back_folder_id & "&back_id=" & back_id & "&back_form_id=" & back_form_id & "&back_LangType=" & back_LangType & "&meta=1" & back_callerpage & back_origurl, False)
            ElseIf (Not m_bClose) And (m_strPageAction <> "save") Then
                If (Request.QueryString("pullapproval") = "true") Then
                    Response.Redirect(GetBackPage(intContentId), False)
                Else
                    If (m_strType = "add" And m_strPageAction = "checkin") Then
                        'leave back_action
                    ElseIf (m_strPageAction <> "publish") Then
                        If (back_action.ToLower = "viewform") Then
                            back_action = back_action & "&staged=true"
                        Else
                            back_action = "viewstaged"
                        End If

                    End If
                    ' replaced logic added by todd 3/30/2006 - when you save then checkin content, GetBackPage() isn't aware of contentid and tries
                    ' to use 0 which causes all sorts of bad things to happen - bug# 19413 - however if you just checkin before saving it goes to a different page,
                    ' so don't replace that (which is the else statement)
                    If (m_strType = "update") Then
                        back_id = intContentId
                        If (controlName = "cbwidget") Then
                            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "UpdateContentWidget", "UpdateContentWidget(" & intContentId & ",'" & buttonId.Value & "');", True)
                        Else
                            Response.Redirect(GetBackPage(intContentId), False)
                        End If

                    Else
                        Response.Redirect(GetBackPage(intContentId), False)
                    End If

                End If
            ElseIf (m_strPageAction = "save") Then
                Response.Redirect("edit.aspx?close=" & Request.QueryString("close") & "&LangType=" & m_intContentLanguage & "&id=" & intContentId & IIf(Me.TaxonomyOverrideId > 0, "&TaxonomyId=" & Me.TaxonomyOverrideId.ToString(), "") & IIf(Me.TaxonomySelectId > 0, "&SelTaxonomyId=" & Me.TaxonomySelectId.ToString(), "") & "&type=update&mycollection=" & strMyCollection & "&addto" & strAddToCollectionType & "&back_file=" & back_file & "&back_action=" & back_action & "&back_folder_id=" & back_folder_id & "&back_id=" & back_id & "&back_form_id=" & back_form_id & "&back_LangType=" & back_LangType & back_callerpage & back_origurl, False)
            End If
            If ((m_bClose) And (m_strPageAction <> "save")) Then
                'Close the editor page
                If (Not (Request.Form("ctlupdateid") IsNot Nothing AndAlso Request.Form("ctlupdateid") <> "")) Then
                    Dim strQuery As String = ""
                    If (TaxonomySelectId > 0) Then
                        strQuery = "&__taxonomyid=" & TaxonomySelectId
                    ElseIf (TaxonomyOverrideId > 0) Then
                        strQuery = "&__taxonomyid=" & TaxonomyOverrideId
                    End If
                    If (controlName = "cbwidget") Then
                        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "UpdateContentWidget", "UpdateContentWidget(" & intContentId & ",'" & buttonId.Value & "');", True)
                    Else
                        Response.Redirect("close.aspx?toggle=true" & strQuery, False)
                    End If


                End If
            End If
            'If (bIsReportForm) Then
            '	If (m_strPageAction = "publish") Then
            '		If Request.Form("renewpoll") <> "" Then
            '			If "true" = Request.Form("renewpoll").ToLower Then
            '				'this needs to be done before the histogram is updated with the new data.
            '				m_refContApi.EkModuleRef.UpdatePollRev(intContentId)
            '			End If
            '		End If
            '                 bUpdateFormQuestions = m_refContApi.EkModuleRef.UpdateFormFieldQuestions(CLng(Request.Form("content_id")), strContent)
            '                 ' Can't do this yet, we must only do it when actually published.
            '                 ' If this is done here, it invalidates an active form.
            '                 ''bAddHistogram = m_refContApi.EkModuleRef.AddNewFormHistogram(CLng(Request.Form("content_id")))
            '	End If
            'End If
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & m_intContentLanguage, False)
        End Try
    End Sub
    Private Function GetBackPage(ByVal contentid As Long) As String
        Dim result As String = "content.aspx"
        If (back_file.Length > 0) Then
            result = back_file
        End If
        If (m_strPageAction = "publish" And back_action.ToLower() = "viewstaged") Then
            'Fix the back page b/c when the action is "viewstaged", it is from the content.aspx
            If result = "approval.aspx" Then
                result = "content.aspx"
            End If
            'Fix the action because staged version does not exists.
            If (Request.Form(submitasstagingview.UniqueID) <> "") Then
                result = result & "?action=viewstaged"
            Else
                result = result & "?action=view"
            End If
        ElseIf back_action.ToLower() = "viewstaged" And blnUndoCheckOut_complete = True Then
            result = result & "?action=view"
        Else
            If (m_strPageAction <> "cancel" AndAlso back_action.ToLower() = "viewcontentbycategory") Then
                ' change old behavior of jumping back to view on adding new content and jump back to added content instead
                back_action = "view"
                back_id = contentid
            End If
            result = result & "?action=" & back_action
        End If
        If back_action.ToLower() <> "viewcontentbycategory" And back_action.ToLower() <> "viewarchivecontentbycategory" Then
            result = result & "&id=" & back_id & "&folder_id=" & back_folder_id
        ElseIf (Convert.ToString(back_folder_id).Length > 0) Then
            result = result & "&id=" & back_folder_id
        End If
        If (Convert.ToString(back_id).Length > 0) Then
            result = result & "&contentid=" & back_id
        End If
        If (Convert.ToString(back_form_id).Length > 0) Then
            result = result & "&form_id=" & back_form_id
        End If
        If (Convert.ToString(back_LangType).Length > 0) Then
            result = result & "&LangType=" & back_LangType
        End If
        If (Convert.ToString(back_callerpage).Length > 0) Then
            result = result & back_callerpage.Replace("&back_", "&")
        End If
        If (Convert.ToString(back_origurl).Length > 0) Then
            result = result & back_origurl.Replace("&back_", "&")
        End If

        If blnShowTStatusMessage = True Then
            result = result & "&ShowTStatusMsg=1"
        End If
        Return (result)
    End Function

    Public Function GetFolderPath(ByVal Id As Long) As String
        Dim contentAPI As New ContentAPI()
        Dim siteAPI As New SiteAPI()

        szdavfolder = "ekdavroot"

        Dim sitePath As String = siteAPI.SitePath.ToString().TrimEnd(New Char() {"/"c}).TrimStart(New Char() {"/"c})
        szdavfolder = szdavfolder.TrimEnd(New Char() {"/"c}).TrimStart(New Char() {"/"c})
        If (Page.Request.Url.Host.ToLower() = "localhost") Then
            szdavfolder = Page.Request.Url.Scheme & Uri.SchemeDelimiter & System.Environment.MachineName & "/" & sitePath & "/" & _
               szdavfolder & "_" & siteAPI.UserId & "_" & siteAPI.UniqueId & _
               (IIf(Context.Request.QueryString("LangType") IsNot Nothing, "_" & Context.Request.QueryString("LangType").ToString(), "")) & "/"
        Else
            szdavfolder = Page.Request.Url.Scheme & Uri.SchemeDelimiter & Page.Request.Url.Authority & "/" & sitePath & "/" & _
               szdavfolder & "_" & siteAPI.UserId & "_" & siteAPI.UniqueId & _
               (IIf(Context.Request.QueryString("LangType") IsNot Nothing, "_" & Context.Request.QueryString("LangType").ToString(), "")) & "/"
        End If

        Dim szFolderPath As String = contentAPI.EkContentRef.GetFolderPath(Id)
        szFolderPath = szFolderPath.Replace("\", "/")
        szFolderPath = szFolderPath.TrimStart(New Char() {"/"c})
        szFolderPath = szFolderPath.Replace("\\", "/")
        If (szFolderPath.Length > 0) Then
            szFolderPath = szdavfolder & szFolderPath & "/"
        Else
            szFolderPath = szdavfolder
        End If

        Return szFolderPath
    End Function
#End Region

#Region "TOOL BAR"
    Private Sub LoadToolBar(ByVal FolderName As String)
        Dim result As New System.Text.StringBuilder
        Dim bInherited As Boolean = False
        Dim strMsg As String = String.Empty
        If FolderName.Length > 0 Then
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("alt Edit Content in Folder") & " """ & FolderName & """")
        Else
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("edit content page title"))
        End If
        result.Append("<table><tr>")
        If (UserRights.CanPublish) Then
            If (IsMac AndAlso Not IsBrowserIE AndAlso m_SelectedEditControl <> "ContentDesigner") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/contentPublish.png", "edit.aspx", m_refMsg.GetMessage("alt publish button text (save)"), m_refMsg.GetMessage("btn publish"), "onclick=""ShowPane('dvContent');buttonaction='publish';elx2.GetBody('GetMACContent');return false;"""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/contentPublish.png", "edit.aspx", m_refMsg.GetMessage("alt publish button text (save)"), m_refMsg.GetMessage("btn publish"), "onclick=""return SetAction('publish');"""))
            End If
        Else
            If (IsMac AndAlso Not IsBrowserIE AndAlso m_SelectedEditControl <> "ContentDesigner") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/approvalSubmitFor.png", "edit.aspx", m_refMsg.GetMessage("alt submit button text (save)"), m_refMsg.GetMessage("btn submit"), "onclick=""ShowPane('dvContent');buttonaction='publish';elx2.GetBody('GetMACContent');return false;"""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/approvalSubmitFor.png", "edit.aspx", m_refMsg.GetMessage("alt submit button text (save)"), m_refMsg.GetMessage("btn submit"), "onclick=""return SetAction('publish');"""))
            End If
            submitasstagingview.Value = "true"
        End If
        If (IsMac AndAlso Not IsBrowserIE AndAlso m_SelectedEditControl <> "ContentDesigner") Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/checkIn.png", "edit.aspx", m_refMsg.GetMessage("alt checkin button text (save)"), m_refMsg.GetMessage("btn checkin"), "onclick=""ShowPane('dvContent');buttonaction='checkin';elx2.GetBody('GetMACContent');return false;"""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/checkIn.png", "edit.aspx", m_refMsg.GetMessage("alt checkin button text (save)"), m_refMsg.GetMessage("btn checkin"), "onclick=""return SetAction('checkin');"""))
        End If

        If "" = Request.QueryString("multi") Then
            If (IsMac AndAlso Not IsBrowserIE AndAlso m_SelectedEditControl <> "ContentDesigner") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/save.png", "edit.aspx", m_refMsg.GetMessage("alt save button text (content)"), m_refMsg.GetMessage("btn save"), "onclick=""ShowPane('dvContent');buttonaction='save';elx2.GetBody('GetMACContent');return false;"""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/save.png", "edit.aspx", m_refMsg.GetMessage("alt save button text (content)"), m_refMsg.GetMessage("btn save"), "onclick=""return SetAction('save');  """))
            End If
        End If
        'If Utilities.IsAsset(lContentType, asset_info("AssetID")) And Len(asset_info("AssetID")) > 0 Then
        '    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_workoffline-nm.gif", "#", m_refMsg.GetMessage("alt workoffline button text"), m_refMsg.GetMessage("btn workoffline"), "onclick=""return SetAction('workoffline');"""))
        'End If
        If (PreviousState = "A") Then
            strMsg = m_refMsg.GetMessage("alt cancel button text (P)")
        Else
            strMsg = m_refMsg.GetMessage("alt cancel button text (CI)")
        End If

        If m_refContentId > 0 And Not lContentSubType = CMSContentSubtype.PageBuilderData And Not lContentSubType = CMSContentSubtype.PageBuilderMasterData AndAlso m_strType.ToLower <> "add" Then
            Dim aURL As String = String.Empty
            'Check for Multisite Content
            If content_edit_data.Quicklink.ToLower().IndexOf("http://") > -1 OrElse content_edit_data.Quicklink.ToLower().IndexOf("https://") > -1 OrElse content_edit_data.Quicklink.StartsWith(m_refContApi.SitePath) Then
                aURL = content_edit_data.Quicklink & "&cmsMode=Preview"
            Else
                aURL = m_refContApi.SitePath & content_edit_data.Quicklink & "&cmsMode=Preview"
            End If
            If (lContentType = CMSContentType_Content Or lContentType = CMSContentType_XmlConfig) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/preview.png", "edit.aspx", m_refMsg.GetMessage("btn preview"), m_refMsg.GetMessage("btn preview"), "onclick=""PreviewContent('" & aURL.Replace("ekfrm", "id") & "', '" & content_edit_data.Title.Replace("&#39;", "&apos;") & "'); return false;"""))
            End If
        End If
        If (IsMac AndAlso Not IsBrowserIE AndAlso m_SelectedEditControl <> "ContentDesigner") Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/cancel.png", "edit.aspx", strMsg, m_refMsg.GetMessage("btn cancel"), "onclick=""ShowPane('dvContent');buttonaction='cancel';elx2.GetBody('GetMACContent');return false;"""))
        Else
            'don't allow cancel for multimedia
            'cancelling will force a newly dragged asset to be published.
            If (Not lContentType = CMSContentType_Media) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/cancel.png", "edit.aspx", strMsg, m_refMsg.GetMessage("btn cancel"), "onclick=""return SetAction('cancel');"""))
            End If
        End If

        result.Append("<td>&nbsp;|&nbsp;</td>")

        'If content_edit_data IsNot Nothing Then

        '    Dim quicklinkUrl As String = m_refContApi.SitePath & content_edit_data.Quicklink
        '    Dim modalUrl As String = String.Format("onclick=""window.open('{0}/analytics/seo.aspx?uri={1}', 'Analytics400', 'width=900,height=580,scrollable=1,resizable=1');return false;""", m_refContApi.AppPath, quicklinkUrl)
        '    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContApi.AppPath & "images/UI/Icons/chartBar.png", "#", m_refMsg.GetMessage("lbl entry analytics"), m_refMsg.GetMessage("lbl entry analytics"), modalUrl))

        'End If

        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_refStyle.GetHelpAliasPrefix(folder_data) & "edittoolbar"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString()
    End Sub
#End Region

#Region "CSS, JS Registration"

    Private Sub RegisterJS()
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
        API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronModalJS)
        API.JS.RegisterJS(Me, Me.AppPath & "java/workareahelper.js", "EktronWorkareaHelperJS")
        If (m_SelectedEditControl = "eWebEditPro") Then
            API.JS.RegisterJS(Me, Me.AppPath & "java/ektron.ewebeditpro.tab.overrides.js", "EktronEWebEditProTabOverridesJS")
        End If
    End Sub

    Private Sub RegisterCSS()
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronUITabsCss)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronModalCss)
        API.Css.RegisterCss(Me, Me.AppPath & "wamenu/css/com.ektron.ui.menu.css", "EktronMenuUIMenuCSS")
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        If (m_SelectedEditControl = "eWebEditPro") Then
            API.Css.RegisterCss(Me, Me.AppPath & "csslib/ektron.ewebeditpro.tab.overrides.css", "EktronEWebEditProTabOverridesCSS")
        End If
    End Sub

#End Region
End Class
