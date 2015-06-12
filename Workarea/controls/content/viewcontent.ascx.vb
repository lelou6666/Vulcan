Imports System.Data
Imports System.Xml
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports ektron.Cms.Site
Imports System.Collections.Generic
Imports Ektron.Cms.DataIO.LicenseManager
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.workarea
Imports Ektron.Cms.Common

Partial Class viewcontent
    Inherits System.Web.UI.UserControl

#Region "Member Variables - Private"

    Private _ApplicationPath As String
    Private _SiteApi As Ektron.Cms.SiteAPI
    Private ekrw As Ektron.Cms.Content.ektUrlRewrite
    Private m_refContent As Ektron.Cms.Content.EkContent
    Private m_refTask As Ektron.Cms.Content.EkTask
    Private cTasks As Object
    Private m_refTaskType As Ektron.Cms.Content.EkTaskType
    Private arrTaskTypeID As String()
    Private intCount As Integer
    Private colAllCategory As Collection

    'subscription - SK
    Private subscription_data_list As SubscriptionData()
    Private subscribed_data_list As SubscriptionData()
    Private subscription_properties_list As SubscriptionPropertiesData
    Private blnBreakSubInheritance As Boolean = False
    Private intInheritFrom As Long = 0
    Private bGlobalSubInherit As Boolean = False
    Private active_subscription_list As SubscriptionData()
    'END: Subscription - SK

    'blog - SK
    Private m_bIsBlog As Boolean = False
    Private blog_post_data As Ektron.Cms.BlogPostData
    Private arrBlogPostCategories As String()
    Private i As Integer = 0

    Private m_SelectedEditControl As String
    Private approvaldata() As ApprovalData = Nothing
    Private IsLastApproval As Boolean = False
    Private IsCurrentApproval As Boolean = False

#End Region

#Region "Member Variables - Protected"

    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_intId As Long = 0
    Protected folder_data As FolderData
    Protected security_data As PermissionData
    Protected AppImgPath As String = ""
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
    Protected LanguageName As String = ""
    Protected language_data As LanguageData
    Protected m_bIsMac As Boolean
    Protected xml_id As Long = 0
    Protected allowHtml As String = ""
    Protected TaxonomyList As String = ""
    Protected ContentPaneHeight As String = "100%"
    Protected entry_edit_data As EntryData = Nothing
    Protected m_refCurrency As Currency = Nothing
    Protected m_refMedia As MediaData = Nothing
    Protected m_refCatalog As CatalogEntry = Nothing
    Protected meta_data As New List(Of ContentMetaData)
    Protected m_refSite As Ektron.Cms.Site.EkSite = Nothing
    Protected m_iFolder As Long = 0
    Protected m_sEditAction As String = ""
    Protected lValidCounter As Integer = 0
    Protected xid As Long = 0
    Protected catalog_data As New FolderData
    Protected objLocalizationApi As New LocalizationAPI()
    Protected lContentType As Integer = 0
    Protected m_bHasXmlConfig As Boolean = False
    Protected m_xmlConfigID As Long = 0
    Protected bTakeAction As Boolean = False
    Protected g_ContentTypeSelected As String = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes
    Protected ViewImage As String = "images/UI/Icons/folderView.png"
    Protected asset_data As AssetInfoData()
    Protected NextActionType As String = ""
    Protected bInOrApproved As Boolean = False
    Protected prod_type_data As ProductTypeData = Nothing
    Protected showAlert As Boolean = True
    Protected _initIsFolderAdmin As Boolean = False
    Protected _isFolderAdmin As Boolean = False
    Protected _initIsCopyOrMoveAdmin As Boolean = False
    Protected _isCopyOrMoveAdmin As Boolean = False

#End Region

#Region "Properties"

    Private Property ApplicationPath() As String
        Get
            Return _ApplicationPath
        End Get
        Set(ByVal value As String)
            _ApplicationPath = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub New()
        _SiteApi = New SiteAPI()

        Dim slash() As Char = {"/"}
        Me.ApplicationPath = _SiteApi.ApplicationPath.TrimEnd(slash)

    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        'register page components
        Me.RegisterCSS()
        Me.RegisterJS()

    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        m_SelectedEditControl = Utilities.GetEditorPreference(Request)
        If (Request.Browser.Platform.IndexOf("Win") = -1) Then
            m_bIsMac = True
        Else
            m_bIsMac = False
        End If

        Me.CreateChildControls()

        m_refMsg = m_refContentApi.EkMsgRef
        ApprovalScript.Visible = False
    End Sub

#End Region

#Region "Helpers"

    Private Function IsAnalyticsViewer() As Boolean
        Dim dataManager As Ektron.Cms.Analytics.IAnalytics = ObjectFactory.GetAnalytics()
        Return dataManager.IsAnalyticsViewer()
    End Function

    Private Function IsFolderAdmin() As Boolean
        If (_initIsFolderAdmin) Then
            Return _isFolderAdmin
        End If
        _isFolderAdmin = m_refContentApi.IsARoleMemberForFolder(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminFolderUsers, m_intFolderId, m_refContentApi.UserId, False)
        _initIsFolderAdmin = True
        Return _isFolderAdmin
    End Function

    Private Function IsCopyOrMoveAdmin() As Boolean
        If (_initIsCopyOrMoveAdmin) Then
            Return _isCopyOrMoveAdmin
        End If
        _isCopyOrMoveAdmin = m_refContentApi.IsARoleMemberForFolder(EkEnumeration.CmsRoleIds.MoveOrCopy, m_intFolderId, m_refContentApi.UserId)
        _initIsCopyOrMoveAdmin = True
        Return _isCopyOrMoveAdmin
    End Function

    Public Function ViewContent() As Boolean
        If (Not (Request.QueryString("id") Is Nothing)) Then
            m_intId = Convert.ToInt64(Request.QueryString("id"))
            If m_intId = 0 Then
                If (Not (Request.QueryString("contentid") Is Nothing)) Then
                    m_intId = Convert.ToInt64(Request.QueryString("contentid"))
                End If
            End If
        End If

        If (Not (Request.QueryString("action") Is Nothing)) Then
            m_strPageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If
        If (Not (Request.QueryString("orderby") Is Nothing)) Then
            m_strOrderBy = Convert.ToString(Request.QueryString("orderby"))
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
        If (Not (Request.QueryString("cancelaction") Is Nothing)) Then
            If (Convert.ToString(Request.QueryString("cancelaction")).ToLower = "undocheckout") Then
                Dim retval As Boolean = False
                m_refContent = m_refContentApi.EkContentRef
                retval = m_refContent.UndoCheckOutv2_0(m_intId)
            End If
        End If
        language_data = (New SiteAPI).GetLanguageById(ContentLanguage)
        LanguageName = language_data.Name
        m_refContent = m_refContentApi.EkContentRef
        TaskExists = m_refContent.DoesTaskExistForContent(m_intId)

        CurrentUserId = m_refContentApi.UserId
        AppImgPath = m_refContentApi.AppImgPath
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
        If (Not (IsNothing(Request.QueryString("folder_id")))) Then
            If (Request.QueryString("folder_id") <> "") Then
                m_intFolderId = Convert.ToInt64(Request.QueryString("folder_id"))
            End If
        End If
        If (m_intFolderId = -1) Then
            'let try again to get folder id
            If (Not (IsNothing(Request.QueryString("fldid")))) Then
                If (Request.QueryString("fldid") <> "") Then
                    m_intFolderId = Convert.ToInt64(Request.QueryString("fldid"))
                End If
            End If
        End If
        Dim MenuItemType As String
        MenuItemType = Request.QueryString("menuItemType")
        If MenuItemType IsNot Nothing AndAlso MenuItemType.ToLower() = "viewproperties" Then
            DefaultTab.Value = "dvProperties"
        End If

        Display_ViewContent()
    End Function

    Public Function FixPath(ByVal html As String, ByVal assetFileName As String) As String
        If (content_data.Status.ToUpper() <> "A") Then
            html = html.Replace(assetFileName, m_refContentApi.SitePath & "assetmanagement/DownloadAsset.aspx?history=true&ID=" & content_data.AssetData.Id & "&version=" & content_data.AssetData.Version)
        End If
        Return html
    End Function

    Private Sub Display_ViewContent()
        m_refMsg = m_refContentApi.EkMsgRef
        Dim bCanAlias As Boolean = False
        Dim security_task_data As PermissionData
        Dim sSummaryText As StringBuilder
        Dim m_aliasname As New UrlAliasing.UrlAliasManualApi
        Dim m_autoaliasApi As New UrlAliasing.UrlAliasAutoApi
        Dim d_alias As Ektron.Cms.Common.UrlAliasManualData
        Dim auto_aliaslist As New System.Collections.Generic.List(Of Ektron.Cms.Common.UrlAliasAutoData)
        Dim m_urlAliasSettings As New UrlAliasing.UrlAliasSettingsApi
        Dim i As Integer
        Dim IsStagingServer As Boolean

        IsStagingServer = m_refContentApi.RequestInformationRef.IsStaging

        security_task_data = m_refContentApi.LoadPermissions(m_intId, "tasks", ContentAPI.PermissionResultType.Task)
        security_data = m_refContentApi.LoadPermissions(m_intId, "content", ContentAPI.PermissionResultType.All)
        security_data.CanAddTask = security_task_data.CanAddTask
        security_data.CanDestructTask = security_task_data.CanDestructTask
        security_data.CanRedirectTask = security_task_data.CanRedirectTask
        security_data.CanDeleteTask = security_task_data.CanDeleteTask



        active_subscription_list = m_refContentApi.GetAllActiveSubscriptions()

        If ("viewstaged" = m_strPageAction) Then
            Dim objContentState As ContentStateData
            objContentState = m_refContentApi.GetContentState(m_intId)
            If ("A" = objContentState.Status) Then
                ' Can't view staged
                m_strPageAction = "view"
            End If
        End If
        If (m_strPageAction = "view") Then
            content_data = m_refContentApi.GetContentById(m_intId)
        ElseIf (m_strPageAction = "viewstaged") Then
            content_data = m_refContentApi.GetContentById(m_intId, ContentAPI.ContentResultType.Staged)
        End If
        If (content_data IsNot Nothing) AndAlso (Ektron.Cms.Common.EkConstants.IsAssetContentType(content_data.Type)) Then
            ContentPaneHeight = "700px"
        End If
        'ekrw = m_refContentApi.EkUrlRewriteRef()
        'ekrw.Load()
        If ((m_urlAliasSettings.IsManualAliasEnabled Or m_urlAliasSettings.IsAutoAliasEnabled) _
          And m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.EditAlias)) _
            And (content_data IsNot Nothing) AndAlso content_data.AssetData IsNot Nothing AndAlso Not (Ektron.Cms.Common.EkFunctions.IsImage("." & content_data.AssetData.FileExtension)) Then
            bCanAlias = True
        End If

        blog_post_data = New BlogPostData
        blog_post_data.Categories = Array.CreateInstance(GetType(String), 0)
        If Not content_data.MetaData Is Nothing Then
            For i = 0 To (content_data.MetaData.Length - 1)
                Select Case content_data.MetaData(i).TypeName.ToLower()
                    Case "blog categories"
                        content_data.MetaData(i).Text = content_data.MetaData(i).Text.Replace("&#39;", "'")
                        content_data.MetaData(i).Text = content_data.MetaData(i).Text.Replace("&quot", """")
                        content_data.MetaData(i).Text = content_data.MetaData(i).Text.Replace("&gt;", ">")
                        content_data.MetaData(i).Text = content_data.MetaData(i).Text.Replace("&lt;", "<")
                        blog_post_data.Categories = Split(content_data.MetaData(i).Text, ";")
                    Case "blog pingback"
                        If Not (content_data.MetaData(i).Text.Trim().ToLower() = "no") Then
                            m_bIsBlog = True
                        End If
                        blog_post_data.Pingback = Ektron.Cms.Common.EkFunctions.GetBoolFromYesNo(content_data.MetaData(i).Text)
                    Case "blog tags"
                        blog_post_data.Tags = content_data.MetaData(i).Text
                    Case "blog trackback"
                        blog_post_data.TrackBackURL = content_data.MetaData(i).Text
                End Select
            Next
        End If

        'THE FOLLOWING LINES ADDED DUE TO TASK
        ':BEGIN / PROPOSED BY PAT
        'TODO: Need to recheck this part of the code e.r.
        If (content_data Is Nothing) Then
            If (ContentLanguage <> 0) Then
                If (CStr(ContentLanguage) <> CStr(Ektron.Cms.CommonApi.GetEcmCookie()("DefaultLanguage"))) Then
                    Response.Redirect(Request.ServerVariables("URL") & "?" & Replace(Request.ServerVariables("Query_String"), "LangType=" & ContentLanguage, "LangType=" & m_refContentApi.DefaultContentLanguage()), False)
                    Exit Sub
                End If
            Else
                If (CStr(ContentLanguage) <> CStr(Ektron.Cms.CommonApi.GetEcmCookie()("DefaultLanguage"))) Then
                    Response.Redirect(Request.ServerVariables("URL") & "?" & Request.ServerVariables("Query_String") & "&LangType=" & m_refContentApi.DefaultContentLanguage(), False)
                    Exit Sub
                End If
            End If
        End If
        ':END
        If (m_intFolderId = -1) Then
            m_intFolderId = content_data.FolderId
        End If
        HoldMomentMsg.Text = m_refMsg.GetMessage("one moment msg")

        If (active_subscription_list Is Nothing) OrElse (active_subscription_list.Length = 0) Then
            phWebAlerts.Visible = False
            phWebAlerts2.Visible = False
        End If
        content_state_data = m_refContentApi.GetContentState(m_intId)

        jsFolderId.Text = m_intFolderId
        jsIsForm.Text = content_data.Type
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
        jsLangId.Text = m_refContentApi.ContentLanguage
        If content_data.Type = 3333 Then
            ViewCatalogToolBar()
        Else
            ViewToolBar()
        End If

        If (bCanAlias And content_data.SubType <> CMSContentSubtype.PageBuilderMasterData) Then  'And folder_data.FolderType <> 1 Don't Show alias tab for Blogs.
            Dim m_strAliasPageName As String = ""

            d_alias = m_aliasname.GetDefaultAlias(content_data.Id)
            If (d_alias.QueryString <> "") Then
                m_strAliasPageName = d_alias.AliasName & d_alias.FileExtension & d_alias.QueryString 'content_data.ManualAlias
            Else
                m_strAliasPageName = d_alias.AliasName & d_alias.FileExtension  'content_data.ManualAlias
            End If

            If (m_strAliasPageName <> "") Then

                If IsStagingServer And folder_data.DomainStaging <> String.Empty Then
                    m_strAliasPageName = "http://" & folder_data.DomainStaging & "/" & m_strAliasPageName
                ElseIf (folder_data.IsDomainFolder) Then
                    m_strAliasPageName = "http://" & folder_data.DomainProduction & "/" & m_strAliasPageName
                Else
                    m_strAliasPageName = SitePath & m_strAliasPageName
                End If
                m_strAliasPageName = "<a href=""" & m_strAliasPageName & """ target=""_blank"" >" & m_strAliasPageName & "</a>"
            Else
                m_strAliasPageName = " [Not Defined]"
            End If
            tdAliasPageName.InnerHtml = m_strAliasPageName
        Else
            phAliases.Visible = False
            phAliases2.Visible = False
        End If
        auto_aliaslist = m_autoaliasApi.GetListForContent(content_data.Id)
        autoAliasList.InnerHtml = "<div class=""ektronHeader"">" & m_refMsg.GetMessage("lbl automatic") & "</div>"
        autoAliasList.InnerHtml &= "<div class=""ektronBorder"">"
        autoAliasList.InnerHtml &= "<table width=""100%"">"
        autoAliasList.InnerHtml &= "<tr class=""title-header"">"
        autoAliasList.InnerHtml &= "<th>" & m_refMsg.GetMessage("generic type") & "</th>"
        autoAliasList.InnerHtml &= "<th>" & m_refMsg.GetMessage("lbl alias name") & "</th>"
        autoAliasList.InnerHtml &= "</tr>"
        For i = 0 To auto_aliaslist.Count() - 1
            autoAliasList.InnerHtml &= "<tr class=""row"">"
            If (auto_aliaslist(i).AutoAliasType = AutoAliasType.Folder) Then
                autoAliasList.InnerHtml &= "<td><img src =""" & m_refContentApi.AppPath & "images/UI/Icons/folder.png""  alt=""" & m_refContentApi.EkMsgRef.GetMessage("lbl folder") & """ title=""" & m_refContentApi.EkMsgRef.GetMessage("lbl folder") & """/ ></td>"
            Else
                autoAliasList.InnerHtml &= "<td><img src =""" & m_refContentApi.AppPath & "images/UI/Icons/taxonomy.png""  alt=""" & m_refContentApi.EkMsgRef.GetMessage("generic taxonomy lbl") & """ title=""" & m_refContentApi.EkMsgRef.GetMessage("generic taxonomy lbl") & """/ ></td>"
            End If

            If IsStagingServer And folder_data.DomainStaging <> String.Empty Then
                autoAliasList.InnerHtml = autoAliasList.InnerHtml & "<td> <a href = ""http://" & folder_data.DomainStaging & "/" & auto_aliaslist(i).AliasName & """ target=""_blank"" >" & auto_aliaslist(i).AliasName & " </a></td></tr>"
            ElseIf (folder_data.IsDomainFolder) Then
                autoAliasList.InnerHtml &= "<td> <a href = ""http://" & folder_data.DomainProduction & "/" & auto_aliaslist(i).AliasName & """ target=""_blank"" >" & auto_aliaslist(i).AliasName & " </a></td>"
            Else
                autoAliasList.InnerHtml &= "<td> <a href = """ & SitePath & auto_aliaslist(i).AliasName & """ target=""_blank"" >" & auto_aliaslist(i).AliasName & " </a></td>"
            End If
            autoAliasList.InnerHtml &= "</tr>"
        Next
        autoAliasList.InnerHtml &= "</table>"
        autoAliasList.InnerHtml &= "</div>"
        If content_data Is Nothing Then
            content_data = m_refContentApi.GetContentById(m_intId)
        End If
        If content_data.Type = 3333 Then
            m_refCatalog = New CatalogEntry(m_refContentApi.RequestInformationRef)
            m_refCurrency = New Currency(m_refContentApi.RequestInformationRef)
            'm_refMedia = MediaData()
            entry_edit_data = m_refCatalog.GetItemEdit(m_intId, ContentLanguage, False)

            Dim m_refProductType As ProductType = New ProductType(m_refContentApi.RequestInformationRef)
            prod_type_data = m_refProductType.GetItem(entry_edit_data.ProductType.Id, True)

            If prod_type_data.Attributes.Count = 0 Then
                phAttributes.Visible = False
                phAttributes2.Visible = False
            End If

            Display_PropertiesTab(content_data)
            Display_PricingTab()
            Display_ItemTab()
            Display_MetadataTab()
            Display_MediaTab()
        Else
            ViewContentProperties(content_data)
            phCommerce.Visible = False
            phCommerce2.Visible = False
            phItems.Visible = False
        End If

        Dim bPackageDisplayXSLT As Boolean = False
        Dim CurrentXslt As String = ""
        Dim XsltPntr As Integer

        If ((Not (IsNothing(content_data.XmlConfiguration))) And (content_data.Type = CMSContentType_CatalogEntry Or content_data.Type = CMSContentType_Content Or content_data.Type = CMSContentType_Forms)) Then
            If (Not (IsNothing(content_data.XmlConfiguration))) Then
                If (content_data.XmlConfiguration.DefaultXslt.Length > 0) Then
                    If (content_data.XmlConfiguration.DefaultXslt = 0) Then
                        bPackageDisplayXSLT = True
                    Else
                        bPackageDisplayXSLT = False
                    End If
                    If (Not bPackageDisplayXSLT) Then
                        XsltPntr = content_data.XmlConfiguration.DefaultXslt
                        If (Len(content_data.XmlConfiguration.PhysPathComplete("Xslt" & XsltPntr))) Then
                            CurrentXslt = content_data.XmlConfiguration.PhysPathComplete("Xslt" & XsltPntr)
                        Else
                            CurrentXslt = content_data.XmlConfiguration.LogicalPathComplete("Xslt" & XsltPntr)
                        End If
                    End If
                Else
                    bPackageDisplayXSLT = True
                End If
                'End If

                Dim objXsltArgs As New Ektron.Cms.Xslt.ArgumentList
                objXsltArgs.AddParam("mode", String.Empty, "preview")
                If (bPackageDisplayXSLT) Then
                    divContentHtml.InnerHtml = m_refContentApi.XSLTransform(content_data.Html, content_data.XmlConfiguration.PackageDisplayXslt, XsltAsFile:=False, XsltArgs:=objXsltArgs, ReturnExceptionMessage:=True)
                Else
                    divContentHtml.InnerHtml = m_refContentApi.XSLTransform(content_data.Html, CurrentXslt, XsltAsFile:=True, XsltArgs:=objXsltArgs, ReturnExceptionMessage:=True)
                End If
            Else
                divContentHtml.InnerHtml = content_data.Html
            End If
        Else
            If (content_data.Type = 104) Then
                media_html.Value = content_data.MediaText
                'Get Url from content
                Dim tPath As String = m_refContentApi.RequestInformationRef.AssetPath & m_refContentApi.EkContentRef.GetFolderParentFolderIdRecursive(content_data.FolderId).Replace(",", "/") & "/" & content_data.AssetData.Id & "." & content_data.AssetData.FileExtension
                Dim mediaHTML As String = FixPath(content_data.Html, tPath)
                Dim scriptStartPtr As Integer = 0
                Dim scriptEndPtr As Integer = 0
                Dim len As Integer = 0
                'Registering the javascript & CSS
                Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "linkReg", "<link href=""" & m_refContentApi.ApplicationPath & "csslib/EktTabs.css"" rel=""stylesheet"" type=""text/css"" />", False)
                mediaHTML = mediaHTML.Replace("<link href=""" & m_refContentApi.ApplicationPath & "csslib/EktTabs.css"" rel=""stylesheet"" type=""text/css"" />", "")
                While (1 = 1)
                    scriptStartPtr = mediaHTML.IndexOf("<script", scriptStartPtr)
                    scriptEndPtr = mediaHTML.IndexOf("</script>", scriptEndPtr)
                    If (scriptStartPtr = -1 OrElse scriptEndPtr = -1) Then
                        Exit While
                    End If
                    len = scriptEndPtr - scriptStartPtr + 9
                    Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "scriptreg" & scriptEndPtr, mediaHTML.Substring(scriptStartPtr, len), False)
                    mediaHTML = mediaHTML.Replace(mediaHTML.Substring(scriptStartPtr, len), "")
                    scriptStartPtr = 0
                    scriptEndPtr = 0
                End While
                media_display_html.Value = mediaHTML
                divContentHtml.InnerHtml = "<a href=""#"" onclick=""document.getElementById('" & divContentHtml.ClientID & "').innerHTML = document.getElementById('" & media_display_html.ClientID & "').value;return false;"" alt=""" & m_refMsg.GetMessage("alt show media content") & """ title=""" & m_refMsg.GetMessage("alt show media content") & """>" & m_refMsg.GetMessage("lbl show media content") & "<br/><img align=""middle"" src=""" & m_refContentApi.AppPath & "images/filmstrip_ph.jpg"" /></a>"
            Else
                If (Ektron.Cms.Common.EkConstants.IsAssetContentType(content_data.Type)) Then
                    Dim ver As String = ""
                    ver = "&version=" & content_data.AssetData.Version
                    If IsImage(content_data.AssetData.Version) Then
                        divContentHtml.InnerHtml = "<img src=""" & m_refContentApi.SitePath & "assetmanagement/DownloadAsset.aspx?ID=" & content_data.AssetData.Id & ver & """ />"
                    Else
                        divContentHtml.InnerHtml = "<div align=""center"" style=""padding:15px;""><a style=""text-decoration:none;"" href=""#"" onclick=""javascript:window.open('" & m_refContentApi.SitePath & "assetmanagement/DownloadAsset.aspx?ID=" & content_data.AssetData.Id & ver & "','DownloadAsset','toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=1,resizable=1,width=1000,height=800');return false;""><img align=""middle"" src=""" & m_refContentApi.AppPath & "images/application/download.gif"" />" & m_refMsg.GetMessage("btn download") & " &quot;" & content_data.Title & "&quot;</a></div>"
                    End If

                ElseIf content_data.SubType = CMSContentSubtype.PageBuilderData Or content_data.SubType = CMSContentSubtype.PageBuilderMasterData Then
                    Dim u As New Ektron.Cms.API.UrlAliasing.UrlAliasCommon
                    Dim fd As FolderData = Me.m_refContentApi.GetFolderById(content_data.FolderId)
                    Dim stralias As String = u.GetAliasForContent(content_data.Id)
                    If (stralias = String.Empty Or fd.IsDomainFolder) Then
                        stralias = content_data.Quicklink
                    End If

                    Dim link As String = ""
                    If (content_data.ContType = EkEnumeration.CMSContentType.Content OrElse (content_data.ContType = EkEnumeration.CMSContentType.Archive_Content And content_data.EndDateAction <> 1)) Then
                        Dim url As String = Me.m_refContent.RequestInformation.SitePath & stralias
                        If (url.Contains("?")) Then
                            url &= "&"
                        Else
                            url &= "?"
                        End If
                        If ("viewstaged" = m_strPageAction) Then
                            url &= "view=staged"
                        Else
                            url &= "view=published"
                        End If
                        url &= "&LangType=" & content_data.LanguageId.ToString()
                        link = "<a href=""" & url & """ onclick=""window.open(this.href);return false;"">Click here to view the page</a><br/><br/>"
                    End If
                    divContentHtml.InnerHtml = link & Ektron.Cms.PageBuilder.PageData.RendertoString(content_data.Html)
                Else
                    divContentHtml.InnerHtml = content_data.Html
                    If (m_bIsBlog) Then
                        Dim blogData As Collection = m_refContentApi.EkContentRef.GetBlogData(content_data.FolderId)
                        If blogData IsNot Nothing Then
                            If blogData("enablecomments") <> 0 Then
                                litBlogComment.Text = "<a class=""button buttonInline greenHover buttonNoIcon"" href=""" & m_refContentApi.AppPath & "content.aspx?id=" & content_data.FolderId & "&action=ViewContentByCategory&LangType=" & content_data.LanguageId & "&ContType=" & CMSContentType_BlogComments & "&contentid=" & content_data.Id & "&viewin=" & content_data.LanguageId & """ title=""" & m_refMsg.GetMessage("alt view comments label") & """>" & m_refMsg.GetMessage("view comments") & "</a>"
                                litBlogComment.Visible = True
                            End If
                        End If
                    End If
                End If
            End If
        End If

        sSummaryText = New StringBuilder
        If CMSContentType.Forms = content_data.Type Or CMSContentType.Archive_Forms = content_data.Type Then
            If Not IsNothing(content_data.Teaser) Then
                If content_data.Teaser.IndexOf("<ektdesignpackage_design") > -1 Then
                    Dim strDesign As String
                    strDesign = m_refContentApi.XSLTransform(content_data.Teaser, _
                      Server.MapPath(m_refContentApi.AppeWebPath() & "unpackageDesign.xslt"), XsltAsFile:=True, _
                      ReturnExceptionMessage:=True)
                    tdsummarytext.InnerHtml = strDesign
                Else
                    tdsummarytext.InnerHtml = content_data.Teaser
                End If
            Else
                tdsummarytext.InnerHtml = ""
            End If
        Else
            If m_bIsBlog Then
                sSummaryText.AppendLine("<table class=""ektronGrid"">")
                sSummaryText.AppendLine("	<tr>")
                sSummaryText.AppendLine("		<td valign=""top"" class=""label"">")
                sSummaryText.AppendLine("			" & m_refMsg.GetMessage("generic description") & "")
                sSummaryText.AppendLine("		</td>")
                sSummaryText.AppendLine("		<td valign=""top"">")
            End If
            sSummaryText.AppendLine(content_data.Teaser)
            If m_bIsBlog Then
                sSummaryText.AppendLine("		</td>")
                sSummaryText.AppendLine("	</tr>")
                sSummaryText.AppendLine("	<tr>")
                sSummaryText.AppendLine("		<td valign=""top"" class=""label"">")
                sSummaryText.AppendLine("			" & m_refMsg.GetMessage("lbl blog cat") & "")
                sSummaryText.AppendLine("		</td>")
                sSummaryText.AppendLine("		<td>")
                If Not (blog_post_data.Categories Is Nothing) Then
                    arrBlogPostCategories = blog_post_data.Categories
                    If arrBlogPostCategories.Length > 0 Then
                        Array.Sort(arrBlogPostCategories)
                    End If
                Else
                    arrBlogPostCategories = Nothing
                End If
                If blog_post_data.Categories.Length > 0 Then
                    For i = 0 To (blog_post_data.Categories.Length - 1)
                        If blog_post_data.Categories(i).ToString() <> "" Then
                            sSummaryText.AppendLine("				<input type=""checkbox"" name=""blogcategories" & i.ToString() & """ value=""" & blog_post_data.Categories(i).ToString() & """ checked=""true"" disabled>&nbsp;" & Replace(blog_post_data.Categories(i).ToString(), "~@~@~", ";") & "<br />")
                        End If
                    Next
                Else
                    sSummaryText.AppendLine("No categories defined.")
                End If
                sSummaryText.AppendLine("		</td>")
                sSummaryText.AppendLine("	</tr>")
                sSummaryText.AppendLine("	<tr>")
                sSummaryText.AppendLine("		<td class=""label"" valign=""top"">")
                sSummaryText.AppendLine("			" & m_refMsg.GetMessage("lbl personal tags") & "")
                sSummaryText.AppendLine("		</td>")
                sSummaryText.AppendLine("		<td>")
                If Not (blog_post_data Is Nothing) Then
                    sSummaryText.AppendLine(blog_post_data.Tags)
                End If
                sSummaryText.AppendLine("		</td>")
                sSummaryText.AppendLine("	</tr>")
                sSummaryText.AppendLine("	<tr>")
                sSummaryText.AppendLine("	    <td class=""label"">")
                If Not (blog_post_data Is Nothing) Then
                    sSummaryText.AppendLine("   <input type=""hidden"" name=""blogposttrackbackid"" id=""blogposttrackbackid"" value=""" & blog_post_data.TrackBackURLID.ToString() & """ />")
                    sSummaryText.AppendLine("   <input type=""hidden"" id=""isblogpost"" name=""isblogpost"" value=""true""/>" & m_refMsg.GetMessage("lbl trackback url") & "")
                    sSummaryText.AppendLine("		</td>")
                    sSummaryText.AppendLine("		<td>")
                    sSummaryText.AppendLine("<input type=""text"" size=""75"" id=""trackback"" name=""trackback"" value=""" & Server.HtmlEncode(blog_post_data.TrackBackURL) & """ disabled/>")
                    sSummaryText.AppendLine("		</td>")
                    sSummaryText.AppendLine("	</tr>")
                    sSummaryText.AppendLine("	<tr>")
                    sSummaryText.AppendLine("		<td class=""label"">")
                    If blog_post_data.Pingback = True Then
                        sSummaryText.AppendLine("" & m_refMsg.GetMessage("lbl blog ae ping") & "")
                        sSummaryText.AppendLine("		</td>")
                        sSummaryText.AppendLine("		<td>")
                        sSummaryText.AppendLine("           <input type=""checkbox"" name=""pingback"" id=""pingback"" checked disabled/>")

                    Else
                        sSummaryText.AppendLine("" & m_refMsg.GetMessage("lbl blog ae ping") & "")
                        sSummaryText.AppendLine("		</td>")
                        sSummaryText.AppendLine("		<td>")
                        sSummaryText.AppendLine("           <input type=""checkbox"" name=""pingback"" id=""pingback"" disabled/>")
                    End If
                Else
                    sSummaryText.AppendLine("           <input type=""hidden"" name=""blogposttrackbackid"" id=""blogposttrackbackid"" value="""" />")
                    sSummaryText.AppendLine("           <input type=""hidden"" id=""isblogpost"" name=""isblogpost"" value=""true""/>" & m_refMsg.GetMessage("lbl trackback url") & "")
                    sSummaryText.AppendLine("<input type=""text"" size=""75"" id=""trackback"" name=""trackback"" value="""" disabled/>")
                    sSummaryText.AppendLine("		</td>")
                    sSummaryText.AppendLine("	</tr>")
                    sSummaryText.AppendLine("	<tr>")
                    sSummaryText.AppendLine("		<td class=""label"">" & m_refMsg.GetMessage("lbl blog ae ping") & "")
                    sSummaryText.AppendLine("		</td>")
                    sSummaryText.AppendLine("		<td>")
                    sSummaryText.AppendLine("           <input type=""checkbox"" name=""pingback"" id=""pingback"" disabled/>")
                End If
                sSummaryText.AppendLine("		</td>")
                sSummaryText.AppendLine("	</tr>")
                sSummaryText.AppendLine("</table>")
            End If
            tdsummarytext.InnerHtml = sSummaryText.ToString()
        End If


        ViewMetaData(content_data)

        tdcommenttext.InnerHtml = content_data.Comment
        AddTaskTypeDropDown()
        ViewTasks()
        ViewSubscriptions()
        Dim cref As Ektron.Cms.Content.EkContent
        cref = m_refContentApi.EkContentRef
        Dim dat As TaxonomyBaseData()
        dat = cref.GetAllFolderTaxonomy(folder_data.Id)
        If (dat Is Nothing OrElse dat.Length = 0) Then
            phCategories.Visible = False
            phCategories2.Visible = False
        End If
        ViewAssignedTaxonomy()
        If (content_data IsNot Nothing) AndAlso ((content_data.Type >= EkConstants.ManagedAsset_Min AndAlso content_data.Type <= EkConstants.ManagedAsset_Max AndAlso content_data.Type <> 104) Or (content_data.Type >= EkConstants.Archive_ManagedAsset_Min AndAlso content_data.Type <= EkConstants.Archive_ManagedAsset_Max AndAlso content_data.Type <> 1104) Or content_data.SubType = CMSContentSubtype.PageBuilderData Or content_data.SubType = CMSContentSubtype.PageBuilderMasterData) Then
            showAlert = False
        End If
    End Sub
    Public Function GetCommerceIncludes() As String
        Dim strReturn As String = ""
        'Display these following commerce related css and javascript files only if the content is of 3333 (which is under catalog folder) type.
        If (content_data IsNot Nothing) AndAlso content_data.Type = 3333 Then
            strReturn += "<script id=""EktronCommercePricingJs"" type=""text/javascript"" src=""" & m_refContentApi.AppPath & "java/commerce/com.ektron.commerce.pricing.js""></script>"
            strReturn += "<link id=""EktronPricingCss"" type=""text/css"" rel=""stylesheet"" href=""" & m_refContentApi.AppPath & "csslib/commerce/Ektron.Commerce.Pricing.css"" />"
        End If
        Return strReturn
    End Function
    Private Sub ViewAssignedTaxonomy()
        Dim cref As Ektron.Cms.Content.EkContent
        cref = m_refContentApi.EkContentRef
        Dim taxonomy_cat_arr As TaxonomyBaseData() = Nothing
        Dim result As List(Of String) = New List(Of String)
        taxonomy_cat_arr = cref.ReadAllAssignedCategory(m_intId, TaxonomyType.Content)
        If (taxonomy_cat_arr IsNot Nothing AndAlso taxonomy_cat_arr.Length > 0) Then
            result.Add("<ul class=""assignedTaxonomyList"">")
            For Each taxonomy_cat As TaxonomyBaseData In taxonomy_cat_arr
                result.Add(("<li>" & taxonomy_cat.TaxonomyPath.Remove(0, 1).Replace("\", " > ") & "</li>"))
            Next
            result.Add("</ul>")
            TaxonomyList = String.Join(String.Empty, result.ToArray())
        Else
            TaxonomyList = m_refMsg.GetMessage("lbl nocatselected")
        End If
    End Sub

    Private Sub ViewTasks()
        Dim actiontype As String = "both"
        Dim callBackPage As String = "" 'unknown
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = m_refMsg.GetMessage("generic Title")
        TaskDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = m_refMsg.GetMessage("generic ID")
        TaskDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STATE"
        colBound.HeaderText = m_refMsg.GetMessage("lbl state")
        TaskDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "PRIORITY"
        colBound.HeaderText = m_refMsg.GetMessage("lbl priority")
        TaskDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DUEDATE"
        colBound.HeaderText = m_refMsg.GetMessage("lbl Due Date")
        TaskDataGrid.Columns.Add(colBound)

        If ((actiontype = "by") Or (actiontype = "all") Or (actiontype = "both")) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "ASSIGNEDTO"
            colBound.HeaderText = m_refMsg.GetMessage("lbl Assigned to")
            TaskDataGrid.Columns.Add(colBound)
        End If
        If ((actiontype = "to") Or (actiontype = "all") Or (actiontype = "both")) Then
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "ASSIGNEDBY"
            colBound.HeaderText = m_refMsg.GetMessage("lbl Assigned By")
            TaskDataGrid.Columns.Add(colBound)
        End If

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "COMMENT"
        colBound.HeaderText = m_refMsg.GetMessage("lbl Last Added comments")
        TaskDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DATECREATED"
        colBound.HeaderText = m_refMsg.GetMessage("lbl Create Date")
        TaskDataGrid.Columns.Add(colBound)

        TaskDataGrid.BorderColor = Drawing.Color.White

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("STATE", GetType(String)))
        dt.Columns.Add(New DataColumn("PRIORITY", GetType(String)))
        dt.Columns.Add(New DataColumn("DUEDATE", GetType(String)))
        dt.Columns.Add(New DataColumn("ASSIGNEDTO", GetType(String)))
        dt.Columns.Add(New DataColumn("ASSIGNEDBY", GetType(String)))
        dt.Columns.Add(New DataColumn("COMMENT", GetType(String)))
        dt.Columns.Add(New DataColumn("DATECREATED", GetType(String)))

        If (TaskExists = True) Then
            Dim TaskItemType As Integer = 1
            m_refTask = m_refContentApi.EkTaskRef
            cTasks = m_refTask.GetTasks(m_intId, -1, -1, TaskItemType, Request.QueryString("orderby"), ContentLanguage)
        End If

        Dim i As Integer
        Dim cTask As Object

        If (Not IsNothing(cTasks)) Then
            Dim m_refMail As New EmailHelper
            While i < cTasks.Count
                i = i + 1
                cTask = cTasks.Item(i)
                If Not (cTask.TaskTypeID = Ektron.Cms.Common.EkEnumeration.TaskType.BlogPostComment) Then
                    ReDim Preserve arrTaskTypeID(i - 1)
                    arrTaskTypeID(i - 1) = "shown_task_" & i & "_" & IIf(cTask.TaskTypeID <= 0, "NotS", CStr(cTask.TaskTypeID))

                    dr = dt.NewRow()

                    dr("TITLE") = "<a href=""tasks.aspx?action=ViewTask&tid=" & cTask.TaskID & "&fromViewContent=1&ty=both&LangType=" & cTask.ContentLanguage & callBackPage & """>" & cTask.TaskTitle & "</a>"
                    dr("TITLE") += "	<script language=""JavaScript"">" & vbCrLf
                    dr("TITLE") += "					AddShownTaskID('" & arrTaskTypeID(i - 1) & "');" & vbCrLf
                    dr("TITLE") += "				</script>	" & vbCrLf

                    dr("ID") = cTask.TaskID
                    Dim iState As Integer = cTask.State
                    Select Case iState
                        Case 1
                            dr("STATE") = "Not Started"
                        Case 2
                            dr("STATE") = "Active"
                        Case 3
                            dr("STATE") = "Awaiting Data"
                        Case 4
                            dr("STATE") = "On Hold"
                        Case 5
                            dr("STATE") = "Pending"
                        Case 6
                            dr("STATE") = "ReOpened"
                        Case 7
                            dr("STATE") = "Completed"
                        Case 8
                            dr("STATE") = "Archived"
                        Case 9
                            dr("STATE") = "Deleted"
                    End Select
                    Dim iPrio As Integer = cTask.Priority
                    Select Case iPrio
                        Case 1
                            dr("PRIORITY") = "Low"
                        Case 2
                            dr("PRIORITY") = "Normal"
                        Case 3
                            dr("PRIORITY") = "High"
                    End Select

                    If (cTask.DueDate <> "") Then
                        If (CDate(cTask.DueDate) < DateTime.Today) Then 'Verify:Udai 11/22/04 Replaced Now.ToOADate - 1 with DateTime.Today
                            dr("DUEDATE") = cTask.DueDate 'Response.Write("<td class=""important"">" & AppUI.GetInternationalDateOnly(cTask.DueDate) & "</td>")
                        Else
                            dr("DUEDATE") = cTask.DueDate 'Response.Write("<td>" & AppUI.GetInternationalDateOnly(cTask.DueDate) & "</td>")
                        End If
                    Else
                        dr("DUEDATE") = "[Not Specified]"
                    End If

                    If ((actiontype = "by") Or (actiontype = "all") Or (actiontype = "both")) Then
                        If (cTask.AssignToUserGroupID = 0) Then
                            dr("ASSIGNEDTO") = "All Authors of (" & CStr(cTask.ContentID) & ")"
                        ElseIf (cTask.AssignedToUser <> "") Then
                            dr("ASSIGNEDTO") = "<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/user.png"" align=""absbottom"">" & m_refMail.MakeUserTaskEmailLink(cTask)
                        ElseIf (cTask.AssignedToUserGroup <> "") Then
                            dr("ASSIGNEDTO") = "<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/users.png"" align=""absbottom"">"
                            If (cTask.AssignToUserGroupID <> -1) Then
                                dr(5) += m_refMail.MakeUserGroupTaskEmailLink(cTask)
                            Else
                                dr(5) += cTask.AssignedToUserGroup()
                            End If
                        End If
                    End If
                    If ((actiontype = "to") Or (actiontype = "all") Or (actiontype = "both")) Then
                        dr("ASSIGNEDBY") = m_refMail.MakeByUserTaskEmailLink(cTask)

                    End If

                    If cTask.LastComment = "" Then
                        dr("COMMENT") = "[Not Specified]"
                    Else
                        dr("COMMENT") = "<div class=""comment-block"">" & cTask.LastComment & "</div>"
                    End If
                    dr("DATECREATED") = cTask.DateCreated 'GetInternationalDateOnly

                    dt.Rows.Add(dr)
                End If
            End While
        End If
        Dim dv As New DataView(dt)
        TaskDataGrid.DataSource = dv
        TaskDataGrid.DataBind()

    End Sub

    'Task Type
    Private Sub AddTaskTypeDropDown()
        m_refTaskType = m_refContentApi.EkTaskTypeRef()
        colAllCategory = m_refTaskType.SelectAllCategory()
        TaskTypeJS.Visible = True
        TaskTypeJS.Text = m_refTaskType.GetTaskTypeJS(colAllCategory, m_refMsg)
    End Sub
    'End: Task Type

    Private Sub ViewContentProperties(ByVal data As ContentData)
        'GET PROPERTY: status
        Dim dataStatus As String = ""
        Select Case data.Status.ToLower
            Case "a"
                dataStatus = m_refMsg.GetMessage("status:Approved (Published)")
            Case "o"
                dataStatus = m_refMsg.GetMessage("status:Checked Out")
            Case "i"
                dataStatus = m_refMsg.GetMessage("status:Checked In")
            Case "p"
                dataStatus = m_refMsg.GetMessage("status:Approved (PGLD)")
            Case "m"
                dataStatus = "<font color=""Red"">" & m_refMsg.GetMessage("status:Submitted for Deletion") & "</font>"
            Case "s"
                dataStatus = "<font color=""Red"">" & m_refMsg.GetMessage("status:Submitted for Approval") & "</font>"
            Case "t"
                dataStatus = m_refMsg.GetMessage("status:Waiting Approval")
            Case "d"
                dataStatus = "Deleted (Pending Start Date)"
        End Select

        'GET PROPERTY: start date
        Dim goLive As String
        If (data.DisplayGoLive.Length = 0) Then
            goLive = m_refMsg.GetMessage("none specified msg")
        Else
            goLive = data.DisplayGoLive
        End If

        'GET PROPERTY: end date
        Dim endDate As String
        If (data.DisplayEndDate = "") Then
            endDate = m_refMsg.GetMessage("none specified msg")
        Else
            endDate = data.DisplayEndDate
        End If

        'GET PROPERTY: action on end date
        Dim endDateActionTitle As String
        If (data.DisplayEndDate.Length > 0) Then
            If (data.EndDateAction = EndDateActionType_archive_display) Then
                endDateActionTitle = m_refMsg.GetMessage("Archive display descrp")
            ElseIf (data.EndDateAction = EndDateActionType_refresh) Then
                endDateActionTitle = m_refMsg.GetMessage("Refresh descrp")
            Else
                endDateActionTitle = m_refMsg.GetMessage("Archive expire descrp")
            End If
        Else
            endDateActionTitle = m_refMsg.GetMessage("none specified msg")
        End If

        'GET PROPERTY: approval method
        Dim apporvalMethod As String
        If (data.ApprovalMethod = 1) Then
            apporvalMethod = m_refMsg.GetMessage("display for force all approvers")
        Else
            apporvalMethod = m_refMsg.GetMessage("display for do not force all approvers")
        End If

        'GET PROPERTY: approvals
        Dim approvallist As New System.Text.StringBuilder
        Dim i As Integer
        If approvaldata Is Nothing Then approvaldata = m_refContentApi.GetCurrentApprovalInfoByID(m_intId)
        approvallist.Append(m_refMsg.GetMessage("none specified msg"))
        If (Not (IsNothing(approvaldata))) Then
            If (approvaldata.Length > 0) Then
                approvallist.Length = 0
                For i = 0 To approvaldata.Length - 1
                    If (approvaldata(i).Type.ToLower = "user") Then
                        approvallist.Append("<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/user.png"" alt=""" & m_refMsg.GetMessage("approver is user") & """ title=""" & m_refMsg.GetMessage("approver is user") & """>")
                    Else
                        approvallist.Append("<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/users.png"" alt=""" & m_refMsg.GetMessage("approver is user group") & """ title=""" & m_refMsg.GetMessage("approver is user group") & """>")
                    End If

                    approvallist.Append("<span")
                    If (approvaldata(i).IsCurrentApprover) Then
                        approvallist.Append(" class=""important""")
                    End If
                    approvallist.Append(">")

                    If (approvaldata(i).Type.ToLower = "user") Then
                        approvallist.Append(approvaldata(i).DisplayUserName)
                    Else
                        approvallist.Append(approvaldata(i).DisplayUserName)
                    End If

                    approvallist.Append("</span>")
                Next
            End If
        End If

        'GET PROPERTY: smart form configuration
        Dim type As String
        If data.Type = 3333 Then
            type = m_refMsg.GetMessage("lbl product type xml config")
        Else
            type = m_refMsg.GetMessage("xml configuration label")
        End If

        'GET PROPERTY: smart form title
        Dim typeValue As String
        If (Not (IsNothing(data.XmlConfiguration))) Then
            typeValue = "&nbsp;" & data.XmlConfiguration.Title
            xml_id = data.XmlConfiguration.Id
        Else
            typeValue = m_refMsg.GetMessage("none specified msg") & " " & m_refMsg.GetMessage("html content assumed")
        End If

        If (folder_data Is Nothing) Then
            folder_data = m_refContentApi.EkContentRef.GetFolderById(content_data.FolderId)
        End If

        'GET PROPERTY: template name
        Dim fileName As String
        If (m_refContent.MultiConfigExists(content_data.Id, m_refContentApi.RequestInformationRef.ContentLanguage)) Then
            Dim t_templateData As TemplateData = m_refContent.GetMultiTemplateASPX(content_data.Id)
            If Not IsNothing(t_templateData) Then
                fileName = t_templateData.FileName
            Else
                fileName = folder_data.TemplateFileName
            End If
        Else
            fileName = folder_data.TemplateFileName
        End If

        'GET PROPERTY: rating
        Dim rating As String
        Dim dataCol As Collection = m_refContentApi.GetContentRatingStatistics(data.Id, 0, Nothing)
        Dim total As Integer = 0
        Dim sum As Integer = 0
        Dim hits As Integer = 0
        If (dataCol.Count > 0) Then
            total = dataCol("total")
            sum = dataCol("sum")
            hits = dataCol("hits")
        End If
        If (total = 0) Then
            rating = m_refMsg.GetMessage("content not rated")
        Else
            rating = Math.Round(Convert.ToDouble(sum) / total, 2)
        End If

        Dim contentPropertyValues As New NameValueCollection()
        contentPropertyValues.Add(m_refMsg.GetMessage("content title label"), data.Title)
        contentPropertyValues.Add(m_refMsg.GetMessage("content id label"), data.Id)
        contentPropertyValues.Add(m_refMsg.GetMessage("content language label"), LanguageName)
        contentPropertyValues.Add(m_refMsg.GetMessage("content status label"), dataStatus)
        contentPropertyValues.Add(m_refMsg.GetMessage("content LUE label"), data.EditorFirstName & " " & data.EditorLastName)
        contentPropertyValues.Add(m_refMsg.GetMessage("content LED label"), data.DisplayLastEditDate)
        contentPropertyValues.Add(m_refMsg.GetMessage("generic start date label"), goLive)
        contentPropertyValues.Add(m_refMsg.GetMessage("generic end date label"), endDate)
        contentPropertyValues.Add(m_refMsg.GetMessage("End Date Action Title"), endDateActionTitle)
        contentPropertyValues.Add(m_refMsg.GetMessage("content DC label"), data.DateCreated)
        contentPropertyValues.Add(m_refMsg.GetMessage("lbl approval method"), apporvalMethod)
        contentPropertyValues.Add(m_refMsg.GetMessage("content approvals label"), approvallist.ToString())
        If (content_data.Type = CMSContentType_CatalogEntry Or content_data.Type = CMSContentType_Content Or content_data.Type = CMSContentType_Forms) Then
            contentPropertyValues.Add(type, typeValue)
        End If
        If (content_data.Type = CMSContentType_CatalogEntry Or content_data.Type = 1 Or content_data.Type = 2 Or content_data.Type = 104) Then
            contentPropertyValues.Add(m_refMsg.GetMessage("template label"), fileName)
        End If
        contentPropertyValues.Add(m_refMsg.GetMessage("generic Path"), data.Path)
        contentPropertyValues.Add(m_refMsg.GetMessage("rating label"), rating)
        contentPropertyValues.Add(m_refMsg.GetMessage("lbl content searchable"), data.IsSearchable.ToString())

        Dim endColon() As Char = {":"}
        Dim propertyName As String
        Dim propertyRows As New StringBuilder()
        For i = 0 To contentPropertyValues.Count - 1
            propertyName = contentPropertyValues.GetKey(i).TrimEnd(endColon)
            propertyRows.Append("<tr><td class=""label"">")
            propertyRows.Append(propertyName & ":")
            propertyRows.Append("</td><td>")
            propertyRows.Append(contentPropertyValues(contentPropertyValues.GetKey(i)))
            propertyRows.Append("</td></tr>")
        Next

        litPropertyRows.Text = propertyRows.ToString()
    End Sub

    Private Sub ViewMetaData(ByVal data As ContentData)
        Dim result As New System.Text.StringBuilder
        Dim customFields As CustomFields
        Dim strResult As String = ""
        Dim strImagePath As String = ""
        Dim fldr_Data As New FolderData
        Dim contentapi As New ContentAPI

        fldr_Data = contentapi.GetFolderById(data.FolderId)
        If (Not data Is Nothing) Then
            strResult = Ektron.Cms.CustomFields.WriteFilteredMetadataForView(data.MetaData, m_intFolderId, False).Trim
            strImagePath = data.Image
            If strImagePath.IndexOf(Me.AppImgPath & "spacer.gif") <> -1 Then
                strImagePath = ""
            End If


            If ((fldr_Data.IsDomainFolder = True OrElse fldr_Data.DomainProduction <> "") And SitePath <> "/") Then
                If strImagePath.IndexOf("http://") <> -1 Then
                    strImagePath = strImagePath.Substring(strImagePath.IndexOf("http://"))
                    data.ImageThumbnail = data.ImageThumbnail.Substring(data.ImageThumbnail.IndexOf("http://"))
                Else
                    If (strImagePath <> "") Then
                        strImagePath = strImagePath.Replace(SitePath, "")
                        data.ImageThumbnail = data.ImageThumbnail.Replace(SitePath, "")
                        strImagePath = "http://" & fldr_Data.DomainProduction & "/" & strImagePath
                        data.ImageThumbnail = "http://" & fldr_Data.DomainProduction & "/" & data.ImageThumbnail
                    End If
                End If
            ElseIf ((fldr_Data.IsDomainFolder = True OrElse fldr_Data.DomainProduction <> "") And SitePath = "/") Then

                If strImagePath.IndexOf("http://") <> -1 Then
                    strImagePath = strImagePath.Substring(strImagePath.IndexOf("http://"))
                    data.ImageThumbnail = data.ImageThumbnail.Substring(data.ImageThumbnail.IndexOf("http://"))
                Else
                    If (strImagePath <> "") Then
                        strImagePath = "http://" & fldr_Data.DomainProduction & "/" & strImagePath.Substring(1)
                        data.ImageThumbnail = "http://" & fldr_Data.DomainProduction & "/" & data.ImageThumbnail.Substring(1)
                    End If
                End If
            ElseIf (fldr_Data.IsDomainFolder = False And strImagePath.IndexOf("http://") <> -1) Then
                If (strImagePath.IndexOf(SitePath) = 0) Then
                    strImagePath = Replace(strImagePath, SitePath, "", 1, 1)
                    data.ImageThumbnail = Replace(data.ImageThumbnail, SitePath, "", 1, 1)
                End If
            End If

            ' display tag info for this library item
            Dim taghtml As New System.Text.StringBuilder
            taghtml.Append("<fieldset style=""margin:10px"">")
            taghtml.Append("<legend>" + m_refMsg.GetMessage("lbl personal tags") + "</legend>")
            taghtml.Append("<div style=""height: 80px; overflow: auto;"" >")
            If (content_data.Id > 0) Then
                Dim localizationApi As New LocalizationAPI()
                Dim tdaUser() As TagData
                tdaUser = (New Community.TagsAPI).GetTagsForObject(content_data.Id, CMSObjectTypes.Content, m_refContentApi.ContentLanguage)

                If (Not IsNothing(tdaUser) AndAlso tdaUser.Length > 0) Then
                    Dim td As TagData
                    For Each td In tdaUser
                        taghtml.Append("<input disabled=""disabled"" checked=""checked"" type=""checkbox"" id=""userPTagsCbx_" + td.Id.ToString + """ name=""userPTagsCbx_" + td.Id.ToString + """ />&#160;")
                        taghtml.Append("<img src='" & localizationApi.GetFlagUrlByLanguageID(td.LanguageId) & "' />")
                        taghtml.Append("&#160;" + td.Text + "<br />")
                    Next
                Else
                    taghtml.Append(m_refMsg.GetMessage("lbl notagsselected"))
                End If
            End If
            taghtml.Append("</div>")
            taghtml.Append("</fieldset>")
            strResult = strResult & taghtml.ToString()
            If (System.IO.Path.GetExtension(data.ImageThumbnail).ToLower().IndexOf(".gif") <> -1 AndAlso data.ImageThumbnail.ToLower().IndexOf("spacer.gif") = -1) Then

                data.ImageThumbnail = data.ImageThumbnail.Replace(".gif", ".png")
            End If
            strResult = strResult & "<fieldset style=""margin:10px""><legend>" & Me.m_refMsg.GetMessage("lbl image data") & "</legend><table width=""100%""><tr><td class=""info"" width=""1%"" nowrap=""true"" align=""left"">" & Me.m_refMsg.GetMessage("images label") & "</td><td width=""99%"" align=""left"">" & strImagePath & "</td></tr><tr><td class=""info"" colomnspan=""2"" align=""left""><img src=""" & data.ImageThumbnail & """ atl=""Thumbnail"" /></td></tr></table></fieldset>"

            If strResult <> "" Then
                result.Append(strResult)
            Else
                result.Append(Me.m_refMsg.GetMessage("lbl nometadefined"))
            End If
        End If

        MetaDataValue.Text = result.ToString()
        customFields = Nothing
    End Sub

    Private Sub Display_PropertiesTab(ByVal data As ContentData)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "NAME"
        colBound.ItemStyle.CssClass = "label"
        PropertiesGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        PropertiesGrid.Columns.Add(colBound)
        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("NAME", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        Dim i As Integer = 0
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic title")
        dr(1) = entry_edit_data.Title
        dt.Rows.Add(dr)
        dr = dt.NewRow()

        content_title.Value = data.Title

        dr(0) = m_refMsg.GetMessage("generic id")
        dr(1) = entry_edit_data.Id
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic language")
        dr(1) = LanguageName
        dt.Rows.Add(dr)

        ' commerce

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl product type xml config")
        dr(1) = entry_edit_data.ProductType.Title
        xml_id = entry_edit_data.ProductType.Id
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl calatog entry sku")
        dr(1) = entry_edit_data.Sku
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl number of units")
        dr(1) = entry_edit_data.QuantityMultiple
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl tax class")
        dr(1) = (New TaxClass(m_refContentApi.RequestInformationRef)).GetItem(entry_edit_data.TaxClassId).Name
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl archived")
        dr(1) = "<input type=""checkbox"" " & IIf(entry_edit_data.IsArchived, "checked=""checked"" ", "") & "disabled=""disabled"" />"
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl buyable")
        dr(1) = "<input type=""checkbox"" " & IIf(entry_edit_data.IsBuyable, "checked=""checked"" ", "") & "disabled=""disabled"" />"
        dt.Rows.Add(dr)

        ' dimensions

        Dim sizeMeasure As String = m_refMsg.GetMessage("lbl inches")
        Dim weightMeasure As String = m_refMsg.GetMessage("lbl pounds")

        If m_refContentApi.RequestInformationRef.MeasurementSystem = MeasurementSystem.Metric Then

            sizeMeasure = m_refMsg.GetMessage("lbl centimeters")
            weightMeasure = m_refMsg.GetMessage("lbl kilograms")

        End If

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl tangible")
        dr(1) = "<input type=""checkbox"" " & IIf(entry_edit_data.IsTangible, "checked=""checked"" ", "") & "disabled=""disabled"" />"
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl height")
        dr(1) = entry_edit_data.Dimensions.Height & " " & sizeMeasure
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl width")
        dr(1) = entry_edit_data.Dimensions.Width & " " & sizeMeasure
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl length")
        dr(1) = entry_edit_data.Dimensions.Length & " " & sizeMeasure
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl weight")
        dr(1) = entry_edit_data.Weight.Amount & " " & weightMeasure
        dt.Rows.Add(dr)

        ' dimensions

        ' inventory
        Dim inventoryApi As New InventoryApi()
        Dim inventoryData As InventoryData = inventoryApi.GetInventory(entry_edit_data.Id)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl disable inventory")
        dr(1) = "<input type=""checkbox"" " & IIf(entry_edit_data.DisableInventoryManagement, "checked=""checked"" ", "") & "disabled=""disabled"" />"
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl in stock")
        dr(1) = inventoryData.UnitsInStock
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl on order")
        dr(1) = inventoryData.UnitsOnOrder
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl reorder")
        dr(1) = inventoryData.ReorderLevel
        dt.Rows.Add(dr)

        ' inventory

        ' end commerce

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content status label")
        Select Case entry_edit_data.ContentStatus.ToLower()
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
            Case "d"
                dr(1) = "Deleted (Pending Start Date)"
        End Select
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content LUE label")
        dr(1) = entry_edit_data.LastEditorFirstName & " " & entry_edit_data.LastEditorLastName
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content LED label")
        dr(1) = entry_edit_data.DateModified
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic start date label")
        If (entry_edit_data.GoLive = DateTime.MinValue Or entry_edit_data.GoLive = DateTime.MaxValue) Then
            dr(1) = m_refMsg.GetMessage("none specified msg")
        Else
            dr(1) = entry_edit_data.GoLive.ToLongDateString() & " " & entry_edit_data.GoLive.ToShortTimeString()
        End If
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic end date label")
        If (entry_edit_data.EndDate = DateTime.MinValue Or entry_edit_data.EndDate = DateTime.MaxValue) Then
            dr(1) = m_refMsg.GetMessage("none specified msg")
        Else
            dr(1) = entry_edit_data.EndDate.ToLongDateString() & " " & entry_edit_data.EndDate.ToShortTimeString()
        End If
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("End Date Action Title")
        If Not (entry_edit_data.EndDate = DateTime.MinValue Or entry_edit_data.EndDate = DateTime.MaxValue) Then
            If (entry_edit_data.EndDateAction = EndDateActionType_archive_display) Then
                dr(1) = m_refMsg.GetMessage("Archive display descrp")
            ElseIf (entry_edit_data.EndDateAction = EndDateActionType_refresh) Then
                dr(1) = m_refMsg.GetMessage("Refresh descrp")
            Else
                dr(1) = m_refMsg.GetMessage("Archive expire descrp")
            End If
        Else
            dr(1) = m_refMsg.GetMessage("none specified msg")
        End If
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content DC label")
        dr(1) = data.DateCreated 'DisplayDateCreated
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = Me.m_refMsg.GetMessage("lbl approval method")
        If (data.ApprovalMethod = 1) Then
            dr(1) = m_refMsg.GetMessage("display for force all approvers")
        Else
            dr(1) = m_refMsg.GetMessage("display for do not force all approvers")
        End If
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content approvals label")
        Dim approvallist As New System.Text.StringBuilder
        If approvaldata Is Nothing Then approvaldata = m_refContentApi.GetCurrentApprovalInfoByID(m_intId)
        approvallist.Append(m_refMsg.GetMessage("none specified msg"))
        If (Not (IsNothing(approvaldata))) Then
            If (approvaldata.Length > 0) Then
                approvallist.Length = 0
                For i = 0 To approvaldata.Length - 1
                    If (approvaldata(i).Type.ToLower = "user") Then
                        approvallist.Append("<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/user.png"" align=""absbottom"" alt=""" & m_refMsg.GetMessage("approver is user") & """ title=""" & m_refMsg.GetMessage("approver is user") & """>")
                    Else
                        approvallist.Append("<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/users.png"" align=""absbottom"" alt=""" & m_refMsg.GetMessage("approver is user group") & """ title=""" & m_refMsg.GetMessage("approver is user group") & """>")
                    End If
                    If (approvaldata(i).IsCurrentApprover) Then
                        approvallist.Append("<span class=""important"">")
                    Else
                        approvallist.Append("<span>")
                    End If
                    If (approvaldata(i).Type.ToLower = "user") Then
                        approvallist.Append(approvaldata(i).DisplayUserName)
                    Else
                        approvallist.Append(approvaldata(i).DisplayUserName)
                    End If
                    approvallist.Append("</span>")
                Next
            End If
        End If
        dr(1) = approvallist.ToString
        dt.Rows.Add(dr)

        If (folder_data Is Nothing) Then
            folder_data = m_refContentApi.EkContentRef.GetFolderById(entry_edit_data.FolderId)
        End If

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("template label")

        If (m_refContent.MultiConfigExists(entry_edit_data.Id, m_refContentApi.RequestInformationRef.ContentLanguage)) Then
            Dim t_templateData As TemplateData = m_refContent.GetMultiTemplateASPX(entry_edit_data.Id)
            If Not IsNothing(t_templateData) Then
                dr(1) = t_templateData.FileName
            Else
                dr(1) = folder_data.TemplateFileName
            End If
        Else
            dr(1) = folder_data.TemplateFileName
        End If

        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic Path")
        dr(1) = data.Path
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("rating label")

        Dim dataCol As Collection = m_refContentApi.GetContentRatingStatistics(entry_edit_data.Id, 0, Nothing)
        Dim total As Integer = 0
        Dim sum As Integer = 0
        Dim hits As Integer = 0
        If (dataCol.Count > 0) Then
            total = dataCol("total")
            sum = dataCol("sum")
            hits = dataCol("hits")
        End If

        If (total = 0) Then
            dr(1) = m_refMsg.GetMessage("content not rated")
        Else
            dr(1) = Math.Round(Convert.ToDouble(sum) / total, 2)
        End If

        dt.Rows.Add(dr)

        'dr = dt.NewRow()
        'dr(0) = "Content Hits:"
        'dr(1) = hits

        'dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = Me.m_refMsg.GetMessage("lbl content searchable")
        dr(1) = data.IsSearchable.ToString()
        dt.Rows.Add(dr)

        Dim dv As New DataView(dt)
        PropertiesGrid.DataSource = dv
        PropertiesGrid.DataBind()
    End Sub

    Private Sub Display_PricingTab()

        Dim workarearef As New Ektron.Cms.Workarea.workareabase()
        Dim activeCurrencyList As List(Of CurrencyData) = m_refCurrency.GetActiveCurrencyList()
        Dim exchangeRateList As New List(Of ExchangeRateData)
        If activeCurrencyList.Count > 1 Then
            Dim exchangeRateApi As New ExchangeRateApi()
            Dim exchangeRateCriteria As New Criteria(Of ExchangeRateProperty)
            Dim currencyIDList As New List(Of Long)
            For i As Integer = 0 To (activeCurrencyList.Count - 1)
                currencyIDList.Add(activeCurrencyList(i).Id)
            Next
            exchangeRateCriteria.AddFilter(ExchangeRateProperty.BaseCurrencyId, CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId)
            exchangeRateCriteria.AddFilter(ExchangeRateProperty.ExchangeCurrencyId, CriteriaFilterOperator.In, currencyIDList.ToArray())
            exchangeRateList = exchangeRateApi.GetCurrentList(exchangeRateCriteria)
        End If
        ltr_pricing.Text = workarearef.CommerceLibrary.GetPricingMarkup(entry_edit_data.Pricing, activeCurrencyList, exchangeRateList, entry_edit_data.EntryType, False, workareaCommerce.ModeType.View)

    End Sub
    Private Sub Display_MediaTab()
        Me.ucMedia.EntryEditData = Me.entry_edit_data
        Me.ucMedia.DisplayMode = Commerce.Workarea.CatalogEntry.Tabs.Medias.Media.DisplayModeValue.View
    End Sub
    Private Sub Display_ItemTab()
        If entry_edit_data IsNot Nothing Then
            Me.ucItems.EntryEditData = entry_edit_data
            Me.ucItems.ItemsFolderId = m_iFolder
            Me.ucItems.SubscriptionControlPath = Me.ApplicationPath + "/Commerce/CatalogEntry/Items/Subscriptions/Membership/Membership.ascx"
            Me.ucItems.DisplayMode = Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Item.DisplayModeValue.View
        End If
    End Sub
    Private Sub Display_MetadataTab()
        Dim sbAttrib As New StringBuilder
        Dim sbResult As New StringBuilder
        Dim strResult As String
        Dim strAttrResult As String
        Dim strImage As String = ""

        EnhancedMetadataScript.Text = Replace(CustomFields.GetEnhancedMetadataScript(), "src=""java/", "src=""../java/")
        EnhancedMetadataArea.Text = CustomFields.GetEnhancedMetadataArea()
        If (Not meta_data Is Nothing) OrElse prod_type_data IsNot Nothing Then
            m_refSite = New Site.EkSite(Me.m_refContentApi.RequestInformationRef)
            Dim hPerm As Hashtable = m_refSite.GetPermissions(m_iFolder, 0, "folder")

            If prod_type_data IsNot Nothing Then sbAttrib.Append(CustomFields.WriteFilteredAttributesForView(entry_edit_data.Attributes, prod_type_data.Id, False))
        End If
        If (m_sEditAction = "update") Then
            strImage = entry_edit_data.Image
            Dim strThumbnailPath As String = entry_edit_data.ImageThumbnail
            If (entry_edit_data.ImageThumbnail = "") Then
                strThumbnailPath = m_refContentApi.AppImgPath & "spacer.gif"
            ElseIf (catalog_data.IsDomainFolder.ToString = True) Then
                strThumbnailPath = strThumbnailPath
            Else
                strThumbnailPath = m_refContentApi.SitePath & strThumbnailPath
            End If
        End If
        strAttrResult = sbAttrib.ToString().Trim()
        strAttrResult = Replace(strAttrResult, "src=""java/", "src=""../java/")
        strAttrResult = Replace(strAttrResult, "src=""images/", "src=""../images/")

        strResult = sbResult.ToString.Trim()
        strResult = Util_FixPath(strResult)
        strResult = Replace(strResult, "src=""java/", "src=""../java/")
        strResult = Replace(strResult, "src=""images/", "src=""../images/")

        ltr_attrib.Text = strAttrResult
    End Sub
    Private Function Util_FixPath(ByVal MetaScript As String) As String
        Dim iTmp As Integer = -1
        iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", 0)
        While iTmp > -1
            iTmp = MetaScript.IndexOf(");return (false);", iTmp)
            MetaScript = MetaScript.Insert(iTmp, ", '" & Me.m_refContentApi.ApplicationPath & "'")
            iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", iTmp + 1)
        End While
        Return MetaScript
    End Function
    Private Sub ViewCatalogToolBar()
        Dim result As New System.Text.StringBuilder
        Dim altText As String = ""
        If content_data Is Nothing Then
            content_data = m_refContentApi.GetContentById(m_intId)
        End If
        Dim ParentId As Long = content_data.FolderId
        Dim pProductType As New ProductType(m_refContentApi.RequestInformationRef)
        Dim count As Integer = 0
        Dim lAddMultiType As Integer = 0
        Dim bSelectedFound As Boolean = False
        Dim bShowAddMenu As Boolean = True
        Dim bViewContent As Boolean = ("view" = m_strPageAction)   ' alternative is archived content
        Dim SRC As String = ""
        Dim str, backStr As String
        Dim bFromApproval As Boolean = False
        Dim type As Integer = 3333
        Dim folderIsHidden As Boolean = m_refContentApi.IsFolderHidden(content_data.FolderId)
        Dim IsOrdered As Boolean = m_refContentApi.EkContentRef.IsOrdered(content_data.Id)

        If (type = 1) Then
            If bFromApproval Then
                backStr = "back_file=approval.aspx"
            Else
                backStr = "back_file=content.aspx"
            End If
        Else
            backStr = "back_file=cmsform.aspx"
        End If
        str = Request.QueryString("action")
        If (Len(str) > 0) Then
            backStr = backStr & "&back_action=" & str
        End If

        If bFromApproval Then
            str = Request.QueryString("page")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_page=" & str
            End If
        End If

        If Not bFromApproval Then
            str = Request.QueryString("folder_id")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_folder_id=" & str
            End If
        End If

        If (type = 1) Then
            str = Request.QueryString("id")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_id=" & str
            End If
        Else
            str = Request.QueryString("form_id")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_form_id=" & str
            End If
        End If
        If (Not IsNothing(Request.QueryString("callerpage"))) Then
            str = Request.QueryString("callerpage")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_callerpage=" & str
            End If
        End If
        If (Not IsNothing(Request.QueryString("origurl"))) Then
            str = Request.QueryString("origurl")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_origurl=" & HttpUtility.UrlEncode(str)
            End If
        End If
        str = ContentLanguage
        If (Len(str) > 0) Then
            backStr = backStr & "&back_LangType=" & str & "&rnd=" & CInt(Int((10 * Rnd()) + 1))
        End If

        SRC = "commerce/catalogentry.aspx?close=false&LangType=" & ContentLanguage & "&id=" & m_intId & "&type=update&" & backStr
        If (bFromApproval) Then
            SRC &= "&pullapproval=true"
        End If

        If (m_strPageAction = "view" Or m_strPageAction = "viewstaged") Then
            Dim WorkareaTitlebarTitle As String = m_refMsg.GetMessage("lbl view catalog entry") & " """ & content_data.Title & """ "
            If (m_strPageAction = "viewstaged") Then WorkareaTitlebarTitle = WorkareaTitlebarTitle & m_refMsg.GetMessage("staged version msg")
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle)
        End If

        result.Append("<table><tr>" & vbCrLf)
        If ((security_data.CanAdd And bViewContent) Or security_data.IsReadOnly = True) Then

            If (security_data.CanAdd And bViewContent) Then
                If Not bSelectedFound Then
                    lContentType = CMSContentType_AllTypes
                End If
            End If
        End If

        SetViewImage()
        result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'action');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'action');"" onmouseout=""this.className='menuRootItem'""><span class=""action"">" & m_refMsg.GetMessage("lbl Action") & "</span></td>")

        If (((security_data.CanAdd) Or security_data.IsReadOnly)) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'view');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'view');"" onmouseout=""this.className='menuRootItem'""><span class=""folderView"">" & m_refMsg.GetMessage("lbl View") & "</span></td>")
        End If

        result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'delete');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'delete');"" onmouseout=""this.className='menuRootItem'""><span class=""chartBar"">" & m_refMsg.GetMessage("generic reports title") & "</span></td>")
        If (Not folderIsHidden AndAlso content_data.SubType <> CMSContentSubtype.PageBuilderData AndAlso content_data.SubType <> CMSContentSubtype.PageBuilderMasterData) Then 'hiding the move button for pagebuilder type.
            If (Request.QueryString("callerpage") = "dashboard.aspx") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            ElseIf (Request.QueryString("callerpage") <> "") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", Request.QueryString("callerpage") & "?" & HttpUtility.UrlDecode(Request.QueryString("origurl")), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            ElseIf (Request.QueryString("backpage") = "history") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?LangType=" & ContentLanguage & "&action=ViewContentByCategory&id=" & content_data.FolderId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            End If
        End If


        bShowAddMenu = True

        If EnableMultilingual = 1 Then
            Dim strViewDisplay As String = ""
            Dim strAddDisplay As String = ""
            Dim result_language() As LanguageData

            If (security_data.CanEdit Or security_data.CanEditSumit) Then
				If (m_refStyle.IsExportTranslationSupportedForContentType(content_data.Type)) Then
					result.Append(m_refStyle.GetExportTranslationButton("content.aspx?LangType=" & ContentLanguage & "&action=Localize&backpage=View&id=" & m_intId & "&folder_id=" & content_data.FolderId, m_refMsg.GetMessage("alt Click here to export this content for translation"), m_refMsg.GetMessage("lbl Export for translation")))
				End If
            End If

            result_language = m_refContentApi.DisplayAddViewLanguage(m_intId)
            For count = 0 To result_language.Length - 1
                If (result_language(count).Type = "VIEW") Then
                    If (content_data.LanguageId = result_language(count).Id) Then
                        strViewDisplay = strViewDisplay & "<option value=" & result_language(count).Id & " selected>" & result_language(count).Name & "</option>"
                    Else
                        strViewDisplay = strViewDisplay & "<option value=" & result_language(count).Id & ">" & result_language(count).Name & "</option>"
                    End If
                End If
            Next
            If (strViewDisplay <> "") Then
                result.Append("<td nowrap=""true"">&nbsp;|&nbsp;" & m_refMsg.GetMessage("lbl View") & ":")
                result.Append("<select id=viewcontent name=viewcontent OnChange=""javascript:LoadContent('frmContent','VIEW');"">")
                result.Append(strViewDisplay)
                result.Append("</select></td>")
            End If
            If (security_data.CanAdd) Then
                'If (bCanAddNewLanguage) Then
                For count = 0 To result_language.Length - 1
                    If (result_language(count).Type = "ADD") Then
                        strAddDisplay = strAddDisplay & "<option value=" & result_language(count).Id & ">" & result_language(count).Name & "</option>"
                    End If
                Next
                If (strAddDisplay <> "") Then
                    result.Append("<td class=""label"">&nbsp;|&nbsp;" & m_refMsg.GetMessage("add title") & ":")
                    If (folder_data Is Nothing) Then
                        folder_data = m_refContentApi.GetFolderById(content_data.FolderId)
                    End If
                    If (Utilities.IsNonFormattedContentAllowed(m_refContentApi.GetEnabledXmlConfigsByFolder(Me.folder_data.Id))) Then
                        allowHtml = "&AllowHtml=1"
                    End If
                    result.Append("<select id=addcontent name=addcontent OnChange=""javascript:LoadContent('frmContent','ADD');"">")
                    result.Append("<option value=" & "0" & ">" & "-select language-" & "</option>")
                    result.Append(strAddDisplay)
                    result.Append("</select></td>")
                End If
                'End If
            End If

            'End If
        End If

        Dim canAddAssets As Boolean = (security_data.CanAdd Or security_data.CanAddFolders) And bViewContent

        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")

        result.Append("<script language=""javascript"">" & Environment.NewLine)

        result.Append("    var filemenu = new Menu( ""file"" );" & Environment.NewLine)
        If (security_data.CanAddFolders) Then
            result.Append("    filemenu.addItem(""&nbsp;<img valign='center' src='" & "images/ui/icons/folderGreen.png" & "' />&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl commerce catalog") & """, function() { window.location.href = 'content.aspx?LangType=" & ContentLanguage & "&action=AddSubFolder&type=catalog&id=" & m_intId & "' } );" & Environment.NewLine)
            result.Append("    filemenu.addBreak();" & Environment.NewLine)
        End If

        If (security_data.IsCollections _
          OrElse m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)) Then
            result.Append("" & Environment.NewLine)
        End If
        result.Append("    var viewmenu = new Menu( ""view"" );" & Environment.NewLine)
        If (security_data.CanHistory) Then
            result.Append("    viewmenu.addItem(""&nbsp;<img height='16px' width='16px' valign='center' src='" & "images/ui/icons/history.png" & "' />&nbsp;&nbsp;" & MakeBold(m_refMsg.GetMessage("lbl content history"), 98) & """, function() { top.document.getElementById('ek_main').src=""historyarea.aspx?action=report&LangType=" & ContentLanguage & "&id=" & m_intId & """;return false;});" & Environment.NewLine)
        End If
        If (content_data.Status <> "A") Then
            If Not ((ManagedAsset_Min <= content_data.Type) And (content_data.Type <= ManagedAsset_Max)) Then
                result.Append("    viewmenu.addItem(""&nbsp;<img height='16px' width='16px' valign='center' src='" & "images/UI/Icons/contentViewDifferences.png" & "' />&nbsp;&nbsp;" & MakeBold(m_refMsg.GetMessage("btn view diff"), 98) & """, function() { PopEditWindow('compare.aspx?LangType=" & ContentLanguage & "&id=" & m_intId & "', 'Compare', 785, 500, 1, 1); } );" & Environment.NewLine)
            End If
        End If
        If (security_data.IsAdmin OrElse IsFolderAdmin()) Then
            result.Append("    viewmenu.addItem(""&nbsp;<img height='16px' width='16px' valign='center' src='" & "images/UI/Icons/approvals.png" & "' />&nbsp;&nbsp;" & MakeBold(m_refMsg.GetMessage("btn view approvals"), 98) & """, function() { location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=ViewApprovals&type=content&id=" & m_intId & """;} );" & Environment.NewLine)
            result.Append("    viewmenu.addItem(""&nbsp;<img height='16px' width='16px' valign='center' src='" & "images/UI/Icons/permissions.png" & "' />&nbsp;&nbsp;" & MakeBold(m_refMsg.GetMessage("btn view permissions"), 98) & """, function() { location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=ViewPermissions&type=content&id=" & m_intId & """;} );" & Environment.NewLine)
        End If
        result.Append("    viewmenu.addBreak();" & Environment.NewLine)
        result.Append("    viewmenu.addItem(""&nbsp;<img valign='center' src='" & "images/ui/icons/brickLeftRight.png" & "' />&nbsp;&nbsp;" & MakeBold(m_refMsg.GetMessage("lbl cross sell"), 98) & """, function() { location.href = ""commerce/recommendations/recommendations.aspx?action=crosssell&folder=" & m_intFolderId & "&id=" & m_intId & """;} );" & Environment.NewLine)
        result.Append("    viewmenu.addItem(""&nbsp;<img valign='center' src='" & "images/ui/icons/brickUp.png" & "' />&nbsp;&nbsp;" & MakeBold(m_refMsg.GetMessage("lbl up sell"), CMSContentType_Content) & """, function() { location.href = ""commerce/recommendations/recommendations.aspx?action=upsell&folder=" & m_intFolderId & "&id=" & m_intId & """;} );" & Environment.NewLine)
        If ((security_data.CanEditFolders And bViewContent) _
         OrElse m_refContentApi.IsARoleMember(CmsRoleIds.CommerceAdmin)) Then
            result.Append("    viewmenu.addBreak();" & Environment.NewLine)
            result.Append("    viewmenu.addItem(""&nbsp;<img height='16px' width='16px' valign='center' src='" & "images/UI/Icons/properties.png" & "' />&nbsp;&nbsp;" & MakeBold(m_refMsg.GetMessage("btn properties"), CMSContentType_Content) & """, function() { window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=EditContentProperties&id=" & m_intId & """;} );" & Environment.NewLine)
        End If

        If ((security_data.CanAdd) And bViewContent) Or security_data.IsReadOnly = True Then
            If ((Not (IsNothing(asset_data)))) Then
                If (asset_data.Length > 0) Then
                    For count = 0 To asset_data.Length - 1
                        If (ManagedAsset_Min <= asset_data(count).TypeId And asset_data(count).TypeId <= ManagedAsset_Max) Then
                            If "*" = asset_data(count).PluginType Then
                                lAddMultiType = asset_data(count).TypeId
                            Else
                                Dim imgsrc As String = String.Empty
                                Dim txtCommName As String = String.Empty
                                If (asset_data(count).TypeId = 101) Then
                                    imgsrc = "&nbsp;<img src='" & "images/UI/Icons/FileTypes/word.png" & "' />&nbsp;&nbsp;"
                                    txtCommName = m_refMsg.GetMessage("lbl Office Documents")
                                ElseIf (asset_data(count).TypeId = 102) Then
                                    imgsrc = "&nbsp;<img valign='center' src='" & "images/UI/Icons/contentHtml.png" & " ' />&nbsp;&nbsp;"
                                    txtCommName = m_refMsg.GetMessage("lbl Managed Files")
                                ElseIf (asset_data(count).TypeId = 104) Then
                                    imgsrc = "&nbsp;<img valign='center' src='" & "images/UI/Icons/film.png" & " ' />&nbsp;&nbsp;"
                                    txtCommName = m_refMsg.GetMessage("lbl Multimedia")
                                Else
                                    imgsrc = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                                End If
                                If (asset_data(count).TypeId <> 105) Then
                                    result.Append("viewmenu.addItem(""" & imgsrc & "" & MakeBold(txtCommName, asset_data(count).TypeId) & """, function() { UpdateView(" & asset_data(count).TypeId & "); } );" & Environment.NewLine)
                                End If
                            End If
                        End If
                    Next
                End If
            End If

            result.Append("    MenuUtil.add( viewmenu );" & Environment.NewLine)

            result.Append("    var deletemenu = new Menu( ""delete"" );" & Environment.NewLine)
            result.Append("    deletemenu.addItem(""&nbsp;<img height='16px' width='16px' valign='center' src='" & "images/UI/Icons/chartBar.png" & "' />&nbsp;&nbsp;" & m_refMsg.GetMessage("content stats") & """, function() { location.href = ""ContentStatistics.aspx?LangType=" & ContentLanguage & "&id=" & m_intId & """;} );" & Environment.NewLine)
            result.Append("    deletemenu.addItem(""&nbsp;<img height='16px' width='16px' valign='center' src='" & "images/ui/icons/chartPie.png" & "' />&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl entry reports") & """, function() { location.href = ""Commerce/reporting/analytics.aspx?LangType=" & ContentLanguage & "&id=" & m_intId & """;} );" & Environment.NewLine)
            Dim quicklinkUrl As String = SitePath & content_data.Quicklink
            If Ektron.Cms.Common.EkConstants.IsAssetContentType(content_data.Type) AndAlso Ektron.Cms.Common.EkFunctions.IsImage("." & content_data.AssetData.FileExtension) Then
                quicklinkUrl = m_refContentApi.RequestInformationRef.AssetPath & content_data.Quicklink
            ElseIf Ektron.Cms.Common.EkConstants.IsAssetContentType(content_data.Type) And SitePath <> "/" Then
                Dim appPathOnly As String = m_refContentApi.RequestInformationRef.ApplicationPath.Replace(SitePath, "")
                If content_data.Quicklink.Contains(appPathOnly) Or Not content_data.Quicklink.Contains("downloadasset.aspx") Then
                    quicklinkUrl = SitePath & IIf(content_data.Quicklink.StartsWith("/"), content_data.Quicklink.Substring(1), content_data.Quicklink)
                Else
                    quicklinkUrl = m_refContentApi.RequestInformationRef.ApplicationPath & content_data.Quicklink
                End If
            End If
            If IsAnalyticsViewer() AndAlso ObjectFactory.GetAnalytics().HasProviders() Then
                Dim modalUrl As String = String.Format("window.open(""{0}/analytics/seo.aspx?tab=traffic&uri={1}"", ""Analytics400"", ""width=900,height=580,scrollable=1,resizable=1"");", ApplicationPath, quicklinkUrl)
                result.Append("    deletemenu.addItem(""&nbsp;<img height='16px' width='16px' valign='center' src='" & "images/ui/icons/chartBar.png" & "' />&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl entry analytics") & """, function() { " & modalUrl & " } );" & Environment.NewLine)
            End If
            result.Append("    MenuUtil.add( deletemenu );" & Environment.NewLine)
        End If

        result.Append("    var actionmenu = new Menu( ""action"" );" & Environment.NewLine)
        If security_data.CanEdit AndAlso (content_data.Status <> "S" And content_data.Status <> "O" OrElse (content_data.Status = "O" AndAlso content_state_data.CurrentUserId = CurrentUserId)) Then
            result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/contentEdit.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn edit") & """, function() { javascript:top.document.getElementById('ek_main').src='" & SRC & "';return false;""" & ",'EDIT',790,580,1,1);return false;" & """ ; } );" & Environment.NewLine)
        End If

        If (security_data.CanDelete) Then
            Dim href As String
            href = "content.aspx?LangType=" & ContentLanguage & "&action=submitDelCatalogAction&delete_id=" & m_intId & "&page=" & Request.QueryString("calledfrom") & "&folder_id=" & content_data.FolderId
            If Not IsOrdered Then result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/delete.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn delete") & """, function() { DeleteConfirmationDialog('" + href & "');return false;} );" & Environment.NewLine)
        End If

        If (security_data.CanEdit) Then

            If ((content_data.Status = "O") And ((content_state_data.CurrentUserId = CurrentUserId) Or (security_data.IsAdmin _
             OrElse IsFolderAdmin()))) Then
                If ((content_data.Status = "O") And ((content_state_data.CurrentUserId = CurrentUserId) Or (security_data.IsAdmin OrElse m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin)))) Then

                    result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/ui/icons/checkIn.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn checkin") & """, function() { DisplayHoldMsg(true); window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=CheckIn&id=" & m_intId & "&content_type=" & content_data.Type & """ ; } );" & Environment.NewLine)

                ElseIf IsFolderAdmin() Then

                    result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/ui/icons/lockEdit.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl take ownership") & """, function() { DisplayHoldMsg(true); window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=TakeOwnerShip&id=" & m_intId & "&content_type=" & content_data.Type & """ ; } );" & Environment.NewLine)

                End If

                If (m_strPageAction = "view") Then
                    result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/preview.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn view stage") & """, function() { window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=ViewStaged&id=" & m_intId & """ ; } );" & Environment.NewLine)
                Else
                    result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/contentViewPublished.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn view publish") & """, function() { window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId & """ ; } );" & Environment.NewLine)
                End If
            ElseIf (((content_data.Status = "I") Or (content_data.Status = "T")) And (content_data.UserId = CurrentUserId)) Then
                If (security_data.CanPublish) Then
                    Dim metaRequuired As Boolean = False
                    Dim categoryRequired As Boolean = False
                    Dim msg As String = String.Empty
                    m_refContentApi.EkContentRef.ValidateMetaDataAndTaxonomy(content_data.FolderId, content_data.Id, content_data.LanguageId, metaRequuired, categoryRequired)
                    If (metaRequuired = False AndAlso categoryRequired = False) Then
                        result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/application/commerce/submit.gif" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn publish") & """, function() { if(CheckTitle()) { DisplayHoldMsg(true); window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=Submit&id=" & m_intId & """ ; } } );" & Environment.NewLine)
                    Else
                        If (metaRequuired = True AndAlso categoryRequired = True) Then
                            msg = m_refMsg.GetMessage("validate meta and category required")
                        ElseIf (metaRequuired = True) Then
                            msg = m_refMsg.GetMessage("validate meta required")
                        Else
                            msg = m_refMsg.GetMessage("validate category required")
                        End If
                        result.Append("    actionmenu.addItem(""&nbsp;<img  height='16px' width='16px' src='" & "images/application/commerce/submit.gif" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn publish") & """, function() { DisplayHoldMsg(true); window.location.href = ""alert('" & msg & "')""" & "; } );" & Environment.NewLine)

                    End If
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/approvalSubmitFor.png", "content.aspx?LangType=" & ContentLanguage & "&action=Submit&id=" & m_intId & "&fldid=" & content_data.FolderId & "&page=workarea", m_refMsg.GetMessage("alt submit button text"), m_refMsg.GetMessage("btn submit"), "onclick=""DisplayHoldMsg(true);return CheckForMeta(" & Convert.ToInt32(security_data.CanMetadataComplete) & ");""")) 'TODO need to pass integer not boolean
                End If
                If (m_strPageAction = "view") Then
                    result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/preview.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn view stage") & """, function() { window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=ViewStaged&id=" & m_intId & "&fldid=" & content_data.FolderId & """ ; } );" & Environment.NewLine)
                Else
                    result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/contentViewPublished.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn view publish") & """, function() { window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId & "&fldid=" & content_data.FolderId & """ ; } );" & Environment.NewLine)
                End If
            ElseIf (content_data.Status = "O") Or (content_data.Status = "I") Or (content_data.Status = "S") Or (content_data.Status = "T") Or (content_data.Status = "P") Then

                If (m_strPageAction = "view") Then
                    result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/preview.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn view stage") & """, function() { window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=ViewStaged&id=" & m_intId & "&fldid=" & content_data.FolderId & """ ; } );" & Environment.NewLine)
                Else
                    result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/contentViewPublished.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn view publish") & """, function() { window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId & "&fldid=" & content_data.FolderId & """ ; } );" & Environment.NewLine)
                End If
            End If

            If (content_data.Status = "S" Or content_data.Status = "M") Then

                Util_CheckIsCurrentApprover(CurrentUserId)

                ApprovalScript.Visible = True
                Dim AltPublishMsg As String = ""
                Dim AltApproveMsg As String = ""
                Dim AltDeclineMsg As String = ""
                Dim PublishIcon As String = ""
                Dim CaptionKey As String = ""
                Dim m_TaskExists As Boolean = m_refContent.DoesTaskExistForContent(content_data.Id)
                Dim m_sPage As String = "workarea" 'To be remove not required.
                If (content_data.Status = "S") Then
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (change)")
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (change)")
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (change)")
                    PublishIcon = "commerce/submit.gif"
                    CaptionKey = "btn publish"
                Else
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (delete)")
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (delete)")
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (delete)")
                    PublishIcon = "../UI/Icons/delete.png"
                    CaptionKey = "btn delete"
                End If
                If (security_data.CanPublish AndAlso IsLastApproval) Then
                    If m_TaskExists = True Then
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/" & PublishIcon & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage(CaptionKey) & """, function() { if(CheckTitle()) { DisplayHoldMsg(true); window.location.href = ('content.aspx?action=approveContent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & content_data.LanguageId & "') ; } } );" & Environment.NewLine)
                    Else
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/" & PublishIcon & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage(CaptionKey) & """, function() { if(CheckTitle()) { DisplayHoldMsg(true); window.location.href = ""content.aspx?action=approvecontent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & ContentLanguage & "" & """ ; } } );" & Environment.NewLine)
                    End If
                ElseIf (security_data.CanApprove AndAlso IsCurrentApproval) Then
                    If m_TaskExists = True Then
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/Commerce/Approve.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn approve") & """, function() { DisplayHoldMsg(true); window.location.href = ('content.aspx?action=approveContent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & content_data.LanguageId & "') ; } );" & Environment.NewLine)
                    Else
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/Commerce/Approve.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn approve") & """, function() { DisplayHoldMsg(true); window.location.href = ""content.aspx?action=approvecontent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & ContentLanguage & "" & """ ; } );" & Environment.NewLine)
                    End If
                End If
                If ((security_data.CanPublish Or security_data.CanApprove) AndAlso IsCurrentApproval) Then
                    If m_TaskExists = True Then
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/DMSMenu/page_white_decline.gif" & "' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn decline") & """, function() { window.location.href = ('content.aspx?action=declineContent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & content_data.LanguageId & "') ; } );" & Environment.NewLine)
                    Else
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/DMSMenu/page_white_decline.gif" & "' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn decline") & """, function() { DeclineContent('" & content_data.Id & "', '" & content_data.FolderId & "', '" & m_sPage & "', '" & ContentLanguage & "')" & " ; } );" & Environment.NewLine)
                    End If
                End If
            End If
        Else
            If (content_data.Status = "S" Or content_data.Status = "M") Then
                Util_CheckIsCurrentApprover(CurrentUserId)

                ApprovalScript.Visible = True
                Dim AltPublishMsg As String = ""
                Dim AltApproveMsg As String = ""
                Dim AltDeclineMsg As String = ""
                Dim PublishIcon As String = ""
                Dim CaptionKey As String = ""
                Dim m_TaskExists As Boolean = m_refContent.DoesTaskExistForContent(content_data.Id)
                Dim m_sPage As String = "workarea" 'To be remove not required.
                If (content_data.Status = "S") Then
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (change)")
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (change)")
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (change)")
                    PublishIcon = "commerce/submit.gif"
                    CaptionKey = "btn publish"
                Else
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (delete)")
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (delete)")
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (delete)")
                    PublishIcon = "commerce/ApproveDelete.png"
                    CaptionKey = "approvals:lbl publish msg (delete)"
                End If
                If (security_data.CanPublish AndAlso IsLastApproval) Then
                    If m_TaskExists = True Then
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/" & PublishIcon & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage(CaptionKey) & """, function() { if(CheckTitle()) { DisplayHoldMsg(true); window.location.href = ('content.aspx?action=approveContent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & content_data.LanguageId & "') ; } } );" & Environment.NewLine)
                    Else
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/" & PublishIcon & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage(CaptionKey) & """, function() { if(CheckTitle()) { DisplayHoldMsg(true); window.location.href = ""content.aspx?action=approvecontent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & ContentLanguage & "" & """ ; } } );" & Environment.NewLine)
                    End If
                ElseIf (security_data.CanApprove AndAlso IsCurrentApproval) Then
                    If m_TaskExists = True Then
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/Commerce/Approve.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn approve") & """, function() { DisplayHoldMsg(true); window.location.href = ('content.aspx?action=approveContent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & content_data.LanguageId & "') ; } );" & Environment.NewLine)
                    Else
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/Commerce/Approve.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn approve") & """, function() { DisplayHoldMsg(true); window.location.href = ""content.aspx?action=approvecontent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & ContentLanguage & "" & """ ; } );" & Environment.NewLine)
                    End If
                End If
                If ((security_data.CanPublish Or security_data.CanApprove) AndAlso IsCurrentApproval) Then
                    If m_TaskExists = True Then
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/DMSMenu/page_white_decline.gif" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn decline") & """, function() { window.location.href = ('content.aspx?action=declineContent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & content_data.LanguageId & "') ; } );" & Environment.NewLine)
                    Else
                        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/application/DMSMenu/page_white_decline.gif" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn decline") & """, function() { DeclineContent('" & content_data.Id & "', '" & content_data.FolderId & "', '" & m_sPage & "', '" & ContentLanguage & "')" & " ; } );" & Environment.NewLine)
                    End If
                End If
                If (security_data.CanEditSumit) Then
                    ' Don't show edit button for Mac when using XML config:
                    If (Not (m_bIsMac AndAlso (Not IsNothing(content_data.XmlConfiguration))) Or m_SelectedEditControl = "ContentDesigner") Then
                        ' result.Append(m_refStyle.GetEditAnchor(m_intId, , True))
                    End If
                End If
                If (m_strPageAction = "view") Then
                    result.Append("    actionmenu.addItem(""&nbsp;<img  height='16px' width='16px' src='" & "images/UI/Icons/preview.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn view stage") & """, function() { window.location.href = ""content.aspx?action=ViewStaged&id=" & m_intId & "&LangType=" & ContentLanguage & """ ; } );" & Environment.NewLine)
                Else
                    result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/contentViewPublished.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn view publish") & """, function() { window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId & """ ; } );" & Environment.NewLine)
                End If
                'End If
                'END
            Else
                If ((content_data.Status = "O") And ((security_data.IsAdmin _
                 OrElse IsFolderAdmin()) _
                 Or (security_data.CanBreakPending))) Then
                    If ((content_data.Status = "O") And ((content_state_data.CurrentUserId = CurrentUserId) Or (security_data.IsAdmin OrElse m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin)))) Then

                        result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/ui/icons/checkIn.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn checkin") & """, function() { DisplayHoldMsg(true); window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=CheckIn&id=" & m_intId & "&fldid=" & content_data.FolderId & "&page=workarea" & "&content_type=" & content_data.Type & """ ; ""DisplayHoldMsg(true);return true;""" & " } );" & Environment.NewLine)

                    End If

                    If (m_strPageAction = "view") Then
                        result.Append("    actionmenu.addItem(""&nbsp;<img  height='16px' width='16px'  src='" & "images/UI/Icons/preview.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn view stage") & """, function() { window.location.href = ""content.aspx?action=ViewStaged&id=" & m_intId & "&LangType=" & ContentLanguage & """ ; } );" & Environment.NewLine)
                    Else
                        result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/contentViewPublished.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn view publish") & """, function() { window.location.href = ""content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId & """ ; } );" & Environment.NewLine)
                    End If
                End If
            End If
        End If
        result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/linkSearch.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn link search") & """, function() { window.location.href = ""isearch.aspx?LangType=" & ContentLanguage & "&action=dofindcontent&folderid=0&content_id=" & m_intId & IIf(content_data.AssetData.MimeType.IndexOf("image") <> -1, "&asset_name=" & content_data.AssetData.Id & "." & content_data.AssetData.FileExtension, "") & """ ; } );" & Environment.NewLine)
        If (security_data.CanAddTask) Then
            result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & "images/UI/Icons/taskAdd.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("btn add task") & """, function() { window.location.href = ""tasks.aspx?LangType=" & ContentLanguage & "&action=AddTask&cid=" & m_intId & "&callbackpage=content.aspx&parm1=action&value1=" & m_strPageAction & "&parm2=id&value2=" & m_intId & "&parm3=LangType&value3=" & ContentLanguage & """ ; } );" & Environment.NewLine)
        End If

        result.Append("    MenuUtil.add( actionmenu );" & Environment.NewLine)
        result.Append("    </script>" & Environment.NewLine)
        result.Append("" & Environment.NewLine)
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Function MakeBold(ByVal str As String, ByVal ContentType As Integer) As String
        If (g_ContentTypeSelected = ContentType) Then
            Return "" & str & ""
        Else
            Return str
        End If

    End Function
    Private Sub SetViewImage(Optional ByVal override As String = "")
        Dim scheckval As String = ""
        If override <> "" Then
            scheckval = override
        Else
            scheckval = g_ContentTypeSelected
        End If
        Select Case scheckval
            Case 101
                ViewImage = "images/UI/Icons/FileTypes/word.png"
            Case 105
                ViewImage = "images/UI/Icons/FileTypes/text.png"
            Case 102
                ViewImage = "images/UI/Icons/contentDMSDocument.png"
            Case 104
                ViewImage = "images/UI/Icons/film.png"
            Case 96
                ViewImage = "images/UI/Icons/folderView.png"
            Case CMSContentType_Content
                ViewImage = "images/UI/Icons/contentHtml.png"
            Case CMSContentType_Forms
                ViewImage = "images/UI/Icons/contentForm.png"
            Case Else

        End Select
    End Sub
    Private Sub ViewToolBar()
        Dim result As New System.Text.StringBuilder
        Dim strAssetId As String = content_data.AssetData.Id
        Dim sHtmlAction As String = ""
        Dim bIsAsset As Boolean = False
        Dim asset_info As New Hashtable
        Dim pubAsHtml As String = "0"
        Dim i As Integer
        Dim folderIsHidden As Boolean = m_refContentApi.IsFolderHidden(content_data.FolderId)

        bIsAsset = Utilities.IsAsset(content_data.Type, strAssetId)
        If (bIsAsset) Then

            For i = 0 To m_AssetInfoKeys.Length - 1
                asset_info.Add(m_AssetInfoKeys(i), "")
            Next
            asset_info("AssetID") = content_data.AssetData.Id '(m_AssetInfoKeys(i))
            asset_info("AssetVersion") = content_data.AssetData.Version
            asset_info("AssetFilename") = content_data.AssetData.FileName
            asset_info("MimeType") = content_data.AssetData.MimeType
            asset_info("FileExtension") = content_data.AssetData.FileExtension
            asset_info("MimeName") = content_data.AssetData.MimeName
            asset_info("ImageUrl") = content_data.AssetData.ImageUrl

            'This code is used to pass the file name to the control to handle work-offline feature.
            If (content_data.AssetData.FileName.Trim <> "") Then
                lblContentTitle.Text = content_data.AssetData.FileName
            Else
                lblContentTitle.Text = content_data.Title
            End If


            For i = 0 To m_AssetInfoKeys.Length - 1
                AssetHidden.Text += "<input type=""hidden"" name=""asset_" & LCase(m_AssetInfoKeys(i)) & """ value=""" & Server.HtmlEncode(asset_info(m_AssetInfoKeys(i))) & """>"
            Next
            AssetHidden.Text += "<script language=""JavaScript"" src=""Tree/js/com.ektron.utils.string.js""></script>" & vbCrLf
            AssetHidden.Text += "<script language=""JavaScript"" src=""Tree/js/com.ektron.utils.cookie.js""></script>" & vbCrLf
            AssetHidden.Text += "<script language=""JavaScript"" src=""java/assetevents.js""></script>" & vbCrLf
            AssetHidden.Text += "<script language=""JavaScript"">" & vbCrLf
            AssetHidden.Text += "setTimeout(""SetTraceFormName()"",1);" & vbCrLf
            AssetHidden.Text += "function SetTraceFormName()" & vbCrLf
            AssetHidden.Text += "{" & vbCrLf
            AssetHidden.Text += "if (""object"" == typeof g_AssetHandler)" & vbCrLf
            AssetHidden.Text += "{" & vbCrLf
            AssetHidden.Text += "g_AssetHandler.formName = ""frmContent"";" & vbCrLf
            AssetHidden.Text += "}" & vbCrLf
            AssetHidden.Text += "}" & vbCrLf
            AssetHidden.Text += "</script>"
        End If
        If (m_strPageAction = "view" Or m_strPageAction = "viewstaged") Then
            Dim WorkareaTitlebarTitle As String = m_refMsg.GetMessage("view content title") & " """ & content_data.Title & """"
            If (m_strPageAction = "viewstaged") Then
                WorkareaTitlebarTitle = WorkareaTitlebarTitle & m_refMsg.GetMessage("staged version msg")
            End If
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle)
        End If

        result.Append("<table><tr>")

        If (security_data.CanEdit) Then
            ' Don't show edit button for Mac when using XML config:
            If (Not (m_bIsMac AndAlso (Not IsNothing(content_data.XmlConfiguration))) Or m_SelectedEditControl = "ContentDesigner") Then
                If content_data.Type = 3333 Then
                    result.Append(m_refStyle.GetCatalogEditAnchor(m_intId))
                Else
                    If (content_data.SubType = CMSContentSubtype.PageBuilderData Or content_data.SubType = CMSContentSubtype.PageBuilderMasterData) Then
                        result.Append(m_refStyle.GetEditAnchor(m_intId, 1, False, CMSContentSubtype.PageBuilderData)) ' to be commented out
                    Else
                        result.Append(m_refStyle.GetEditAnchor(m_intId)) ' to be commented out
                    End If
                    result.Append(m_refStyle.GetPageBuilderEditAnchor(m_intId, content_data))
                End If
            End If
            If (bIsAsset) Then
                Dim folder_data As FolderData
                folder_data = m_refContentApi.GetFolderById(m_intFolderId)
                If (Not IsNothing(folder_data)) Then
                    If folder_data.PublishHtmlActive Then
                        pubAsHtml = "1"
                    Else
                        pubAsHtml = "0"
                    End If
                Else
                    pubAsHtml = "0"
                End If
            End If
        End If

        If (security_data.CanHistory) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/history.png", "#", m_refMsg.GetMessage("alt history button text"), m_refMsg.GetMessage("lbl generic history"), "onclick=""top.document.getElementById('ek_main').src='historyarea.aspx?action=report&LangType=" & ContentLanguage & "&id=" & m_intId & "';return false;"""))
        End If

        If (security_data.CanEdit) Then
            If ((content_data.Status = "O") And ((content_state_data.CurrentUserId = CurrentUserId) Or (security_data.IsAdmin _
             OrElse IsFolderAdmin()))) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/checkIn.png", "content.aspx?LangType=" & ContentLanguage & "&action=CheckIn&id=" & m_intId & "&content_type=" & content_data.Type, m_refMsg.GetMessage("alt checkin button text"), m_refMsg.GetMessage("btn checkin"), ""))
                If (m_strPageAction = "view") Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/preview.png", "content.aspx?LangType=" & ContentLanguage & "&action=ViewStaged&id=" & m_intId, m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), ""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentViewPublished.png", "content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId, m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), ""))
                End If
            ElseIf (((content_data.Status = "I") Or (content_data.Status = "T")) And (content_data.UserId = CurrentUserId)) Then
                If (security_data.CanPublish) Then
                    Dim metaRequuired As Boolean = False
                    Dim categoryRequired As Boolean = False
                    Dim msg As String = String.Empty
                    m_refContentApi.EkContentRef.ValidateMetaDataAndTaxonomy(content_data.FolderId, content_data.Id, content_data.LanguageId, metaRequuired, categoryRequired)
                    If (metaRequuired = False AndAlso categoryRequired = False) Then
                        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentPublish.png", "content.aspx?LangType=" & ContentLanguage & "&action=Submit&id=" & m_intId & "&fldid=" & content_data.FolderId & "&page=workarea", m_refMsg.GetMessage("alt publish button text"), m_refMsg.GetMessage("btn publish"), "onclick=""DisplayHoldMsg(true);return CheckForMeta(" & Convert.ToInt32(security_data.CanMetadataComplete) & ");"""))
                    Else
                        If (metaRequuired = True AndAlso categoryRequired = True) Then
                            msg = m_refMsg.GetMessage("validate meta and category required")
                        ElseIf (metaRequuired = True) Then
                            msg = m_refMsg.GetMessage("validate meta required")
                        Else
                            msg = m_refMsg.GetMessage("validate category required")
                        End If
                        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentPublish.png", "#", m_refMsg.GetMessage("alt publish button text"), m_refMsg.GetMessage("btn publish"), "onclick=""alert('" & msg & "');"""))
                    End If
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/approvalSubmitFor.png", "content.aspx?LangType=" & ContentLanguage & "&action=Submit&id=" & m_intId & "&fldid=" & content_data.FolderId & "&page=workarea", m_refMsg.GetMessage("alt submit button text"), m_refMsg.GetMessage("btn submit"), "onclick=""DisplayHoldMsg(true);return CheckForMeta(" & Convert.ToInt32(security_data.CanMetadataComplete) & ");""")) 'TODO need to pass integer not boolean
                End If
                If (m_strPageAction = "view") Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/preview.png", "content.aspx?LangType=" & ContentLanguage & "&action=ViewStaged&id=" & m_intId & "&fldid=" & content_data.FolderId, m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), ""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentViewPublished.png", "content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId & "&fldid=" & content_data.FolderId, m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), ""))
                End If
            ElseIf (content_data.Status = "O") Or (content_data.Status = "I") Or (content_data.Status = "S") Or (content_data.Status = "T") Or (content_data.Status = "P") Then
                If (m_strPageAction = "view") Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/preview.png", "content.aspx?LangType=" & ContentLanguage & "&action=ViewStaged&id=" & m_intId & "&fldid=" & content_data.FolderId, m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), ""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentViewPublished.png", "content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId & "&fldid=" & content_data.FolderId, m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), ""))
                End If
            End If
        Else
            'NEW CODE IMPLEMENTATION ADDED BY UDAI On 06/16/05 FOR THE DEFECT#13694,13914
            'BEGIN
            If (content_data.Status = "S" Or content_data.Status = "M") Then
                ApprovalScript.Visible = True
                Dim AltPublishMsg As String = ""
                Dim AltApproveMsg As String = ""
                Dim AltDeclineMsg As String = ""
                Dim PublishIcon As String = ""
                Dim CaptionKey As String = ""
                Dim m_TaskExists As Boolean = m_refContent.DoesTaskExistForContent(content_data.Id)
                Dim m_sPage As String = "workarea" 'To be remove not required.
                If (content_data.Status = "S") Then
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (change)")
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (change)")
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (change)")
                    PublishIcon = "../UI/Icons/contentPublish.png"
                    CaptionKey = "btn publish"
                Else
                    AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (delete)")
                    AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (delete)")
                    AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (delete)")
                    PublishIcon = "../UI/Icons/delete.png"
                    CaptionKey = "btn delete"
                End If
                If (security_data.CanPublish AndAlso content_state_data.CurrentUserId = CurrentUserId) Then
                    If m_TaskExists = True Then
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & PublishIcon, "#", AltPublishMsg, m_refMsg.GetMessage(CaptionKey), "Onclick=""javascript:return LoadChildPage('action=approveContent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & content_data.LanguageId & "');"""))
                    Else
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & PublishIcon, "content.aspx?action=approvecontent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & ContentLanguage & "", AltPublishMsg, m_refMsg.GetMessage(CaptionKey), ""))
                    End If
                ElseIf (security_data.CanApprove AndAlso content_state_data.CurrentUserId = CurrentUserId) Then
                    If m_TaskExists = True Then
                        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/approvalApproveItem.png", "#", AltApproveMsg, m_refMsg.GetMessage("btn approve"), "Onclick=""javascript:return LoadChildPage('action=approveContent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & content_data.LanguageId & "');"""))
                    Else
                        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/approvalApproveItem.png", "content.aspx?action=approvecontent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & ContentLanguage & "", AltApproveMsg, m_refMsg.GetMessage("btn approve"), ""))
                    End If
                End If
                If ((security_data.CanPublish Or security_data.CanApprove) AndAlso content_state_data.CurrentUserId = CurrentUserId) Then
                    If m_TaskExists = True Then
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_decline-nm.gif", "#", AltDeclineMsg, m_refMsg.GetMessage("btn decline"), "Onclick=""javascript:return LoadChildPage('action=declineContent&id=" & content_data.Id & "&fldid=" & content_data.FolderId & "&page=" & m_sPage & "&LangType=" & content_data.LanguageId & "');"""))
                    Else
                        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_decline-nm.gif", "javascript:DeclineContent('" & content_data.Id & "', '" & content_data.FolderId & "', '" & m_sPage & "', '" & ContentLanguage & "')", AltDeclineMsg, m_refMsg.GetMessage("btn decline"), ""))
                    End If
                End If
                If (security_data.CanEditSumit) Then
                    ' Don't show edit button for Mac when using XML config:
                    If (Not (m_bIsMac AndAlso (Not IsNothing(content_data.XmlConfiguration))) Or m_SelectedEditControl = "ContentDesigner") Then
                        If (content_data.SubType = CMSContentSubtype.PageBuilderData Or content_data.SubType = CMSContentSubtype.PageBuilderMasterData) Then
                            result.Append(m_refStyle.GetEditAnchor(m_intId, 1, True, CMSContentSubtype.PageBuilderData))
                        Else
                            result.Append(m_refStyle.GetEditAnchor(m_intId, , True, content_data.SubType))
                        End If
                        result.Append(m_refStyle.GetPageBuilderEditAnchor(m_intId, content_data))


                    End If
                End If
                If (m_strPageAction = "view") Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/preview.png", "content.aspx?action=ViewStaged&id=" & m_intId & "&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), ""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentViewPublished.png", "content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId, m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), ""))
                End If
                'End If
                'END
            Else
                If ((content_data.Status = "O") And ((security_data.IsAdmin _
                 OrElse IsFolderAdmin()) _
                 Or (security_data.CanBreakPending))) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/checkIn.png", "content.aspx?LangType=" & ContentLanguage & "&action=CheckIn&id=" & m_intId & "&fldid=" & content_data.FolderId & "&page=workarea" & "&content_type=" & content_data.Type, m_refMsg.GetMessage("alt checkin button text"), m_refMsg.GetMessage("btn checkin"), "onclick=""DisplayHoldMsg(true);return true;"""))
                    If (m_strPageAction = "view") Then
                        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/preview.png", "content.aspx?action=ViewStaged&id=" & m_intId & "&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt view staged button text"), m_refMsg.GetMessage("btn view stage"), ""))
                    Else
                        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentViewPublished.png", "content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId, m_refMsg.GetMessage("alt view published button text"), m_refMsg.GetMessage("btn view publish"), ""))
                    End If
                End If
            End If
        End If

        If (security_data.CanDelete) Then
            Dim href As String
            href = "content.aspx?LangType=" & ContentLanguage & "&action=submitDelContAction&delete_id=" & m_intId & "&page=" & Request.QueryString("calledfrom") & "&folder_id=" & content_data.FolderId
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt delete button text"), m_refMsg.GetMessage("btn delete"), "onclick=""DeleteConfirmationDialog('" + href & "');return false;"" "))
        End If

        If (content_data.Status <> "A") Then
            If Not ((ManagedAsset_Min <= content_data.Type) And (content_data.Type <= ManagedAsset_Max)) Then
                If (content_data.SubType <> CMSContentSubtype.PageBuilderData And content_data.SubType <> CMSContentSubtype.PageBuilderMasterData) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentViewDifferences.png", "#", "View Difference", m_refMsg.GetMessage("btn view diff"), "onclick=""PopEditWindow('compare.aspx?LangType=" & ContentLanguage & "&id=" & m_intId & "', 'Compare', 785, 650, 1, 1);"""))
                End If
            End If
        End If
        'If (Not folderIsHidden) Then
        '    If ((content_data.Status = "A" Or content_data.Status = "I") _
        '        And (security_data.IsAdmin OrElse IsFolderAdmin() OrElse IsCopyOrMoveAdmin())) Then
        '        If (CallerPage <> "") Then
        '            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentCopy.png", "content.aspx?LangType=" & ContentLanguage & "&action=MoveContent&id=" & m_intId & "&page=" & CallerPage & "&folder_id=" & content_data.FolderId, m_refMsg.GetMessage("btn move content"), m_refMsg.GetMessage("btn move content"), ""))
        '        Else
        '            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentCopy.png", "content.aspx?LangType=" & ContentLanguage & "&action=MoveContent&id=" & m_intId & "&folder_id=" & content_data.FolderId, m_refMsg.GetMessage("btn move content"), m_refMsg.GetMessage("btn move content"), ""))
        '        End If
        '    End If
        'End If
        If (security_data.IsAdmin OrElse IsFolderAdmin()) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/permissions.png", "content.aspx?LangType=" & ContentLanguage & "&action=ViewPermissions&type=content&id=" & m_intId, m_refMsg.GetMessage("alt permissions button text content (view)"), m_refMsg.GetMessage("btn view permissions"), ""))
            If (Not folderIsHidden) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/approvals.png", "content.aspx?LangType=" & ContentLanguage & "&action=ViewApprovals&type=content&id=" & m_intId, m_refMsg.GetMessage("alt approvals button text content (view)"), m_refMsg.GetMessage("btn view approvals"), ""))
            End If
        End If
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/linkSearch.png", "isearch.aspx?LangType=" & ContentLanguage & "&action=dofindcontent&folderid=0&content_id=" & m_intId & IIf(content_data.AssetData.MimeType.IndexOf("image") <> -1, "&asset_name=" & content_data.AssetData.Id & "." & content_data.AssetData.FileExtension, ""), m_refMsg.GetMessage("btn link search"), m_refMsg.GetMessage("btn link search"), ""))

        If (security_data.CanAddTask) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/taskAdd.png", "tasks.aspx?LangType=" & ContentLanguage & "&action=AddTask&cid=" & m_intId & "&callbackpage=content.aspx&parm1=action&value1=" & m_strPageAction & "&parm2=id&value2=" & m_intId & "&parm3=LangType&value3=" & ContentLanguage, m_refMsg.GetMessage("btn add task"), m_refMsg.GetMessage("btn add task"), ""))
        End If

        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/chartBar.png", "ContentStatistics.aspx?LangType=" & ContentLanguage & "&id=" & m_intId, m_refMsg.GetMessage("click view content reports"), m_refMsg.GetMessage("click view content reports"), ""))
        Dim quicklinkUrl As String = SitePath & content_data.Quicklink
        If Ektron.Cms.Common.EkConstants.IsAssetContentType(content_data.Type) AndAlso Ektron.Cms.Common.EkFunctions.IsImage("." & content_data.AssetData.FileExtension) Then
            quicklinkUrl = m_refContentApi.RequestInformationRef.AssetPath & content_data.Quicklink
        ElseIf Ektron.Cms.Common.EkConstants.IsAssetContentType(content_data.Type) And SitePath <> "/" Then
            Dim appPathOnly As String = m_refContentApi.RequestInformationRef.ApplicationPath.Replace(SitePath, "")
            If content_data.Quicklink.Contains(appPathOnly) Or Not content_data.Quicklink.Contains("downloadasset.aspx") Then
                quicklinkUrl = SitePath & IIf(content_data.Quicklink.StartsWith("/"), content_data.Quicklink.Substring(1), content_data.Quicklink)
            Else
                quicklinkUrl = m_refContentApi.RequestInformationRef.ApplicationPath & content_data.Quicklink
            End If
        End If
        If IsAnalyticsViewer() AndAlso ObjectFactory.GetAnalytics().HasProviders() Then
            Dim modalUrl As String = String.Format("onclick=""window.open('{0}/analytics/seo.aspx?tab=traffic&uri={1}', 'Analytics400', 'width=900,height=580,scrollable=1,resizable=1');return false;""", ApplicationPath, quicklinkUrl)
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/chartPie.png", "#", m_refMsg.GetMessage("lbl entry analytics"), m_refMsg.GetMessage("lbl entry analytics"), modalUrl))
        End If

        If (security_data.IsAdmin _
        OrElse IsFolderAdmin()) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/properties.png", "content.aspx?LangType=" & ContentLanguage & "&action=EditContentProperties&id=" & m_intId, m_refMsg.GetMessage("btn edit prop"), m_refMsg.GetMessage("btn edit prop"), ""))
        End If

        If (Not folderIsHidden) Then
            If (Request.QueryString("callerpage") = "dashboard.aspx") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            ElseIf (Request.QueryString("callerpage") <> "") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", Request.QueryString("callerpage") & "?" & HttpUtility.UrlDecode(Request.QueryString("origurl")), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            ElseIf (Request.QueryString("backpage") = "history") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?LangType=" & ContentLanguage & "&action=ViewContentByCategory&id=" & content_data.FolderId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            End If
        End If

        'Sync API needs to know folder type to display the eligible sync profiles.
        If (folder_data Is Nothing) Then
            folder_data = m_refContentApi.GetFolderById(content_data.FolderId)
        End If

        Dim site As SiteAPI = New SiteAPI
        Dim ekSiteRef As EkSite = site.EkSiteRef()
        If (m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncAdmin)) AndAlso (LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Feature.eSync)) AndAlso (m_refContentApi.RequestInformationRef.IsSyncEnabled()) Then
            If ((m_strPageAction = "view") AndAlso (content_data.Status.ToUpper() = "A") AndAlso ekSiteRef.IsStaged()) Then
                If folder_data.IsDomainFolder() Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "sync_now_data.png", "#", m_refMsg.GetMessage("alt sync content"), m_refMsg.GetMessage("btn sync content"), "OnClick=""Ektron.Sync.checkMultipleConfigs(" & ContentLanguage & "," & m_intId & ",'" & content_data.AssetData.Id & "','" & content_data.AssetData.Version & "'," & content_data.FolderId & ",true);return false;"""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "sync_now_data.png", "#", m_refMsg.GetMessage("alt sync content"), m_refMsg.GetMessage("btn sync content"), "OnClick=""Ektron.Sync.checkMultipleConfigs(" & ContentLanguage & "," & m_intId & ",'" & content_data.AssetData.Id & "','" & content_data.AssetData.Version & "'," & content_data.FolderId & ",false);return false;"""))
                End If
            End If
        End If

        If EnableMultilingual = 1 Then
            Dim strViewDisplay As String = ""
            Dim strAddDisplay As String = ""
            Dim result_language() As LanguageData
            Dim count As Integer = 0

            If (security_data.CanEdit Or security_data.CanEditSumit) Then
                If (m_refStyle.IsExportTranslationSupportedForContentType(content_data.Type)) Then
                    result.Append(m_refStyle.GetExportTranslationButton("content.aspx?LangType=" & ContentLanguage & "&action=Localize&backpage=View&id=" & m_intId & "&folder_id=" & content_data.FolderId, m_refMsg.GetMessage("alt Click here to export this content for translation"), m_refMsg.GetMessage("lbl Export for translation")))
                End If
            End If

            result_language = m_refContentApi.DisplayAddViewLanguage(m_intId)
            For count = 0 To result_language.Length - 1
                If (result_language(count).Type = "VIEW") Then
                    If (content_data.LanguageId = result_language(count).Id) Then
                        strViewDisplay = strViewDisplay & "<option value=" & result_language(count).Id & " selected>" & result_language(count).Name & "</option>"
                    Else
                        strViewDisplay = strViewDisplay & "<option value=" & result_language(count).Id & ">" & result_language(count).Name & "</option>"
                    End If
                End If
            Next
            If (strViewDisplay <> "") Then
                result.Append("<td class=""label"">&nbsp;|&nbsp;</td>")
                result.Append("<td class=""label"">" & m_refMsg.GetMessage("view language") & "</td>")
                result.Append("<td>")
                result.Append("<select id=viewcontent name=viewcontent OnChange=""javascript:LoadContent('frmContent','VIEW');"">")
                result.Append(strViewDisplay)
                result.Append("</select>")
                result.Append("</td>")
            End If
            If (security_data.CanAdd) Then
                'If (bCanAddNewLanguage) Then
                For count = 0 To result_language.Length - 1
                    If (result_language(count).Type = "ADD") Then
                        strAddDisplay = strAddDisplay & "<option value=" & result_language(count).Id & ">" & result_language(count).Name & "</option>"
                    End If
                Next
                If (strAddDisplay <> "") Then
                    result.Append("<td>&nbsp;&nbsp;</td>")
                    result.Append("<td class=""label"">" & m_refMsg.GetMessage("add title") & ":</td>")
                    result.Append("<td>")
                    If (folder_data Is Nothing) Then
                        folder_data = m_refContentApi.GetFolderById(content_data.FolderId)
                    End If
                    If (Utilities.IsNonFormattedContentAllowed(m_refContentApi.GetEnabledXmlConfigsByFolder(Me.folder_data.Id))) Then
                        allowHtml = "&AllowHtml=1"
                    End If
                    result.Append("<select id=addcontent name=addcontent OnChange=""javascript:LoadContent('frmContent','ADD');"">")
                    result.Append("<option value=" & "0" & ">" & "-select language-" & "</option>")
                    result.Append(strAddDisplay)
                    result.Append("</select></td>")
                End If
                'End If
            End If
        End If

        result.Append("<td>&nbsp;&nbsp;</td>")
        result.Append("<td>")
        If (m_strPageAction = "view") Then
            result.Append(m_refStyle.GetHelpButton("Viewcontent"))
        ElseIf (m_strPageAction = "viewstaged") Then
            result.Append(m_refStyle.GetHelpButton("Viewstaged"))
        End If
        result.Append("</td>")
        result.Append("</tr>")
        result.Append("</table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Protected Sub TaskDataGrid_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        Select Case e.Item.ItemType
            Case ListItemType.AlternatingItem, ListItemType.Item
                If (Not (e.Item.Cells(4).Text.Equals("[Not Specified]"))) Then
                    e.Item.Cells(4).CssClass = "important"
                End If
                e.Item.Attributes.Add("id", arrTaskTypeID(intCount))
                intCount += 1
        End Select
    End Sub

    Private Sub ViewSubscriptions()
        Dim strEnabled As String = " "
        Dim i As Integer = 0
        Dim findindex As Integer
        Dim arrSubscribed As Array = Nothing
        Dim strNotifyA As String = ""
        Dim strNotifyI As String = ""
        Dim strNotifyN As String = ""
        Dim intInheritFrom As Long
        Dim emailfrom_list As EmailFromData()
        Dim y As Integer = 0
        Dim defaultmessage_list As EmailMessageData()
        Dim unsubscribe_list As EmailMessageData()
        Dim optout_list As EmailMessageData()
        Dim sbOutput As New System.Text.StringBuilder
        Dim settings_list As SettingsData

        intInheritFrom = m_refContentApi.GetFolderInheritedFrom(m_intFolderId)

        subscription_data_list = m_refContentApi.GetSubscriptionsForFolder(intInheritFrom) 'AGofPA get subs for folder; set break inheritance flag false
        subscription_properties_list = m_refContentApi.GetSubscriptionPropertiesForContent(m_intId) 'first try content
        If subscription_properties_list Is Nothing Then
            subscription_properties_list = m_refContentApi.GetSubscriptionPropertiesForFolder(intInheritFrom) 'then get folder
            subscribed_data_list = subscription_data_list ' get subs for folder
        Else 'content is populated.
            subscribed_data_list = m_refContentApi.GetSubscriptionsForContent(m_intId) ' get subs for folder
        End If

        emailfrom_list = m_refContentApi.GetAllEmailFrom()
        defaultmessage_list = m_refContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.DefaultMessage)
        unsubscribe_list = m_refContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Unsubscribe)
        optout_list = m_refContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OptOut)
        settings_list = (New SiteAPI).GetSiteVariables()

        If intInheritFrom <> m_intId Then 'do we get settings from self?
            bGlobalSubInherit = True
        Else
            bGlobalSubInherit = False
        End If

        If (emailfrom_list Is Nothing) Or (defaultmessage_list Is Nothing) Or (unsubscribe_list Is Nothing) Or (optout_list Is Nothing) Or (subscription_data_list Is Nothing) _
             Or ((settings_list.AsynchronousLocation = "")) Then
            tdsubscriptiontext.Text &= ("<input type=""hidden"" name=""suppress_notification"" value=""true"">")
            tdsubscriptiontext.Text &= ("<br/>" & m_refMsg.GetMessage("lbl web alert settings") & ":<br/><br/>" & m_refMsg.GetMessage("lbl web alert not setup") & "<br/>")
            If (emailfrom_list Is Nothing) Then
                tdsubscriptiontext.Text &= ("<br/><font color=""red"">" & m_refMsg.GetMessage("lbl web alert emailfrom not setup") & "</font>")
            End If
            If (defaultmessage_list Is Nothing) Then
                tdsubscriptiontext.Text &= ("<br/><font color=""red"">" & m_refMsg.GetMessage("lbl web alert def msg not setup") & "</font>")
            End If
            If (unsubscribe_list Is Nothing) Then
                tdsubscriptiontext.Text &= ("<br/><font color=""red"">" & m_refMsg.GetMessage("lbl web alert unsub not setup") & "</font>")
            End If
            If (optout_list Is Nothing) Then
                tdsubscriptiontext.Text &= ("<br/><font color=""red"">" & m_refMsg.GetMessage("lbl web alert optout not setup") & "</font>")
            End If
            If (subscription_data_list Is Nothing) Then
                phWebAlerts.Visible = False
                phWebAlerts2.Visible = False
                tdsubscriptiontext.Text &= ("<br/><font color=""red"">" & m_refMsg.GetMessage("alt No subscriptions are enabled on the folder.") & "</font>")
            End If
            If ((settings_list.AsynchronousLocation = "")) Then
                tdsubscriptiontext.Text &= ("<br/><font color=""red"">" & m_refMsg.GetMessage("alt The location to the Asynchronous Data Processor is not specified.") & "</font>")
            End If
            Exit Sub
        End If

        If subscription_properties_list Is Nothing Then
            subscription_properties_list = New SubscriptionPropertiesData
        End If

        strEnabled = " disabled=""true"" "

        Select Case subscription_properties_list.NotificationType.GetHashCode
            Case 0
                strNotifyA = " CHECKED=""true"" "
                strNotifyI = ""
                strNotifyN = ""
            Case 1
                strNotifyA = ""
                strNotifyI = " CHECKED=""true"" "
                strNotifyN = ""
            Case 2
                strNotifyA = ""
                strNotifyI = ""
                strNotifyN = " CHECKED=""true"" "
        End Select
        'always break inheritance because its content
        tdsubscriptiontext.Text &= "<input id=""break_sub_inherit_button"" type=""hidden"" name=""break_sub_inherit_button"" value=""break_sub_inherit_button"">"

        tdsubscriptiontext.Text &= "<table class=""ektronGrid"">"
        tdsubscriptiontext.Text &= "    <tr>"
        tdsubscriptiontext.Text &= "        <td class=""label"">"
        tdsubscriptiontext.Text &= "            " & m_refMsg.GetMessage("lbl web alert opt") & ":"
        tdsubscriptiontext.Text &= "        </td>"
        tdsubscriptiontext.Text &= "        <td class=""value"">"
        tdsubscriptiontext.Text &= "            <input type=""radio"" value=""Always"" name=""notify_option"" " & strNotifyA & " " & strEnabled & "> " & m_refMsg.GetMessage("lbl web alert notify always") & "<br />"
        tdsubscriptiontext.Text &= "            <input type=""radio"" value=""Initial"" name=""notify_option""" & strNotifyI & " " & strEnabled & "> " & m_refMsg.GetMessage("lbl web alert notify initial") & "<br />"
        tdsubscriptiontext.Text &= "            <input type=""radio"" value=""Never"" name=""notify_option""" & strNotifyN & " " & strEnabled & "> " & m_refMsg.GetMessage("lbl web alert notify never")
        tdsubscriptiontext.Text &= "        </td>"
        tdsubscriptiontext.Text &= "    </tr>"

        tdsubscriptiontext.Text &= "    <tr>"
        tdsubscriptiontext.Text &= "        <td class=""label"">"
        tdsubscriptiontext.Text &= "            " & m_refMsg.GetMessage("lbl web alert subject") & ":"
        tdsubscriptiontext.Text &= "        </td>"

        tdsubscriptiontext.Text &= "        <td class=""value"">"
        If subscription_properties_list.Subject <> "" Then
            tdsubscriptiontext.Text &= "        <input type=""text"" maxlength=""255"" size=""65"" value=""" & subscription_properties_list.Subject & """ name=""notify_subject"" " & strEnabled & "/>"
        Else
            tdsubscriptiontext.Text &= "        <input type=""text"" maxlength=""255"" size=""65"" value="""" name=""notify_subject"" " & strEnabled & "/>"
        End If
        tdsubscriptiontext.Text &= ""

        tdsubscriptiontext.Text &= "        </td>"
        tdsubscriptiontext.Text &= "    </tr>"

        tdsubscriptiontext.Text &= "    <tr>"
        tdsubscriptiontext.Text &= "        <td class=""label"">"
        tdsubscriptiontext.Text &= "            " & m_refMsg.GetMessage("lbl web alert emailfrom address") & ":"
        tdsubscriptiontext.Text &= "        </td>"
        tdsubscriptiontext.Text &= "        <td class=""value"">"
        tdsubscriptiontext.Text &= "            <select name=""notify_emailfrom"" " & strEnabled & ">:"

        If (Not emailfrom_list Is Nothing) AndAlso emailfrom_list.Length > 0 Then
            For y = 0 To emailfrom_list.Length - 1
                If emailfrom_list(y).Email = subscription_properties_list.EmailFrom Then
                    tdsubscriptiontext.Text &= "<option value=""" & emailfrom_list(y).Id & """ SELECTED>" & emailfrom_list(y).Email & "</option>"
                Else
                    tdsubscriptiontext.Text &= "<option value=""" & emailfrom_list(y).Id & """>" & emailfrom_list(y).Email & "</option>"
                End If
            Next
        End If
        tdsubscriptiontext.Text &= "            </select>"
        tdsubscriptiontext.Text &= ""
        tdsubscriptiontext.Text &= "        </td>"
        tdsubscriptiontext.Text &= "    </tr>"

        tdsubscriptiontext.Text &= "    <tr>"
        tdsubscriptiontext.Text &= "        <td class=""label"">"
        tdsubscriptiontext.Text &= "            " & m_refMsg.GetMessage("lbl web alert contents") & ":"
        tdsubscriptiontext.Text &= "        </td>"

        tdsubscriptiontext.Text &= "        <td class=""value"">"
        tdsubscriptiontext.Text += ("           <input id=""use_optout_button"" type=""checkbox"" checked=""true"" name=""use_optout_button"" disabled=""true"">" & m_refMsg.GetMessage("lbl optout message"))

        tdsubscriptiontext.Text += "            &nbsp;<select " & strEnabled & " name=""notify_optoutid"">"

        If (Not optout_list Is Nothing) AndAlso optout_list.Length > 0 Then
            For y = 0 To optout_list.Length - 1
                If optout_list(y).Id = subscription_properties_list.OptOutID Then
                    tdsubscriptiontext.Text &= ("<option value=""" & optout_list(y).Id & """ SELECTED>" & Server.HtmlEncode(optout_list(y).Title) & "</option>")
                Else
                    tdsubscriptiontext.Text &= ("<option value=""" & optout_list(y).Id & """>" & Server.HtmlEncode(optout_list(y).Title) & "</option>")
                End If
            Next
        End If
        tdsubscriptiontext.Text += "            </select><br />"

        If subscription_properties_list.DefaultMessageID > 0 Then
            tdsubscriptiontext.Text += ("       <input id=""use_message_button"" type=""checkbox"" checked=""true"" name=""use_message_button"" " & strEnabled & ">" & m_refMsg.GetMessage("lbl use default message"))
        Else
            tdsubscriptiontext.Text += ("       <input id=""use_message_button"" type=""checkbox"" name=""use_message_button"" " & strEnabled & ">" & m_refMsg.GetMessage("lbl use default message"))
        End If
        tdsubscriptiontext.Text += "            &nbsp;<select " & strEnabled & " name=""notify_messageid"">"

        If (Not defaultmessage_list Is Nothing) AndAlso defaultmessage_list.Length > 0 Then
            For y = 0 To defaultmessage_list.Length - 1
                If defaultmessage_list(y).Id = subscription_properties_list.DefaultMessageID Then
                    tdsubscriptiontext.Text += "<option value=""" & defaultmessage_list(y).Id & """ SELECTED>" & Server.HtmlEncode(defaultmessage_list(y).Title) & "</option>"
                Else
                    tdsubscriptiontext.Text += "<option value=""" & defaultmessage_list(y).Id & """>" & Server.HtmlEncode(defaultmessage_list(y).Title) & "</option>"
                End If
            Next
        End If
        tdsubscriptiontext.Text += "            </select><br />"

        If subscription_properties_list.SummaryID > 0 Then
            tdsubscriptiontext.Text += ("       <input id=""use_summary_button"" type=""checkbox"" name=""use_summary_button"" checked=""true"" " & strEnabled & ">" & m_refMsg.GetMessage("lbl use summary message") & "<br />")
        Else
            tdsubscriptiontext.Text += ("       <input id=""use_summary_button"" type=""checkbox"" name=""use_summary_button"" " & strEnabled & ">" & m_refMsg.GetMessage("lbl use summary message") & "<br />")
        End If
        If subscription_properties_list.ContentID = -1 Then
            tdsubscriptiontext.Text += ("       <input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" checked=""true"" " & strEnabled & ">" & m_refMsg.GetMessage("lbl use content message"))
            tdsubscriptiontext.Text += "        &nbsp;"
            tdsubscriptiontext.Text += "        <input type=""hidden"" maxlength=""20"" name=""frm_content_id"" value=""" & subscription_properties_list.ContentID.ToString() & """/><input type=""hidden"" name=""frm_content_langid""/><input type=""hidden"" name=""frm_qlink""/><input type=""text"" name=""titlename"" value=""[[use current]]"" " & strEnabled & " size=""65""/><br/>"
        ElseIf subscription_properties_list.ContentID > 0 Then
            tdsubscriptiontext.Text += ("       <input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" checked=""true"" " & strEnabled & ">" & m_refMsg.GetMessage("lbl use content message"))
            tdsubscriptiontext.Text += "        &nbsp;"
            tdsubscriptiontext.Text += "        <input type=""hidden"" maxlength=""20"" name=""frm_content_id"" value=""" & subscription_properties_list.ContentID.ToString() & """/><input type=""hidden"" name=""frm_content_langid""/><input type=""hidden"" name=""frm_qlink""/><input type=""text"" name=""titlename"" value=""" & subscription_properties_list.UseContentTitle.ToString & """ " & strEnabled & " size=""65""/><br/><br/>"
        Else
            tdsubscriptiontext.Text += ("       <input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" " & strEnabled & ">" & m_refMsg.GetMessage("lbl use content message"))
            tdsubscriptiontext.Text += "        &nbsp;"
            tdsubscriptiontext.Text += "        <input type=""hidden"" maxlength=""20"" name=""frm_content_id"" value=""0"" /><input type=""hidden"" name=""frm_content_langid""/><input type=""hidden"" name=""frm_qlink""/><input type=""text"" name=""titlename"" onkeydown=""return false"" value="""" " & strEnabled & " size=""65""/><br/>"
        End If
        If subscription_properties_list.UseContentLink > 0 Then
            tdsubscriptiontext.Text += ("       <input id=""use_contentlink_button"" type=""checkbox"" name=""use_contentlink_button"" checked=""true"" " & strEnabled & ">Use Content Link<br />")
        Else
            tdsubscriptiontext.Text += ("       <input id=""use_contentlink_button"" type=""checkbox"" name=""use_contentlink_button"" " & strEnabled & ">Use Content Link<br />")
        End If
        tdsubscriptiontext.Text += ("           <input id=""use_unsubscribe_button"" type=""checkbox"" checked=""true"" name=""use_unsubscribe_button"" disabled=""true"">" & m_refMsg.GetMessage("lbl unsubscribe message"))
        tdsubscriptiontext.Text += "            &nbsp;<select " & strEnabled & " name=""notify_unsubscribeid"">"

        If (Not unsubscribe_list Is Nothing) AndAlso unsubscribe_list.Length > 0 Then
            For y = 0 To unsubscribe_list.Length - 1
                If unsubscribe_list(y).Id = subscription_properties_list.UnsubscribeID Then
                    tdsubscriptiontext.Text += "<option value=""" & unsubscribe_list(y).Id & """ SELECTED>" & Server.HtmlEncode(unsubscribe_list(y).Title) & "</option>"
                Else
                    tdsubscriptiontext.Text += "<option value=""" & unsubscribe_list(y).Id & """>" & Server.HtmlEncode(unsubscribe_list(y).Title) & "</option>"
                End If
            Next
        End If
        tdsubscriptiontext.Text += "            </select><br />"
        tdsubscriptiontext.Text &= "            </td>"
        tdsubscriptiontext.Text &= "         </tr>"
        tdsubscriptiontext.Text &= "     </table>"

        tdsubscriptiontext.Text += ("<div class=""ektronHeader"">" & m_refMsg.GetMessage("lbl avail web alert") & "</div>")
        tdsubscriptiontext.Text += ("<table class=""ektronGrid"" cellspacing=""1"" id=""cfld_subscription_assignment"" id=""cfld_folder_assignment"">")

        If Not (subscription_data_list Is Nothing) Then
            tdsubscriptiontext.Text += ("<tr class=""title-header""><td>" & m_refMsg.GetMessage("lbl assigned") & "</td><td align=""left"">" & m_refMsg.GetMessage("lbl name") & "</td></tr>")
            If Not (subscribed_data_list Is Nothing) Then
                arrSubscribed = Array.CreateInstance(GetType(Long), subscribed_data_list.Length)
                For i = 0 To subscribed_data_list.Length - 1
                    arrSubscribed.SetValue(subscribed_data_list(i).Id, i)
                Next
                If (Not arrSubscribed Is Nothing) Then
                    If arrSubscribed.Length > 0 Then
                        Array.Sort(arrSubscribed)
                    End If
                End If
            End If
            i = 0
            For i = 0 To subscription_data_list.Length - 1
                findindex = -1
                If ((Not subscribed_data_list Is Nothing) AndAlso (Not arrSubscribed Is Nothing)) Then
                    findindex = Array.BinarySearch(arrSubscribed, subscription_data_list(i).Id)
                End If
                tdsubscriptiontext.Text += "<tr>"
                If findindex < 0 Then
                    tdsubscriptiontext.Text += "<td nowrap=""true"" align=""center""><input type=""checkbox"" name=""Assigned_" & subscription_data_list(i).Id & """  id=""Assigned_" & subscription_data_list(i).Id & """ " & strEnabled & "></td></td>"
                Else
                    tdsubscriptiontext.Text += "<td nowrap=""true"" align=""center""><input type=""checkbox"" name=""Assigned_" & subscription_data_list(i).Id & """  id=""Assigned_" & subscription_data_list(i).Id & """ checked=""true"" " & strEnabled & "></td></td>"
                End If
                tdsubscriptiontext.Text += "<td nowrap=""true"" align=""Left"">" & subscription_data_list(i).Name & "</td>"
                tdsubscriptiontext.Text += "</tr>"
            Next
        Else
            tdsubscriptiontext.Text &= ("<tr><td>Nothing available.</td></tr>")
        End If
        tdsubscriptiontext.Text += ("</table><input type=""hidden"" name=""content_sub_assignments"" value="""">")
    End Sub

    Private Function IsImage(ByVal fileName As String) As Boolean
        Dim imageArray As String() = {".gif", ".jpeg", ".dib", ".jpg", ".bmp", ".tiff", ".tif", ".png", ".jpe", "jfif"}
        Dim extension As String
        If fileName <> "" Then
            extension = System.IO.Path.GetExtension(fileName)
            For Each ext As String In imageArray
                If (extension.ToLower() = ext) Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function

    Private Sub Util_CheckIsCurrentApprover(ByVal userId As Long)

        If approvaldata Is Nothing Then approvaldata = m_refContentApi.GetCurrentApprovalInfoByID(m_intId)

        If approvaldata IsNot Nothing AndAlso approvaldata.Length > 0 Then

            IsLastApproval = approvaldata(approvaldata.Length - 1).IsCurrentApprover AndAlso (approvaldata(approvaldata.Length - 1).UserId = CurrentUserId OrElse New UserAPI().IsAGroupMember(CurrentUserId, approvaldata(approvaldata.Length - 1).GroupId))

            If IsLastApproval Then

                IsCurrentApproval = True

            Else

                For i As Integer = 0 To (approvaldata.Length - 1)

                    If approvaldata(i).IsCurrentApprover Then

                        IsCurrentApproval = (approvaldata(i).UserId = CurrentUserId OrElse New UserAPI().IsAGroupMember(CurrentUserId, approvaldata(i).GroupId))

                    End If

                Next

            End If

        End If

    End Sub

#End Region

#Region "CSS, JS, Images"

    Private Sub RegisterCSS()

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)

    End Sub

    Private Sub RegisterJS()

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)
		Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/jfunct.js", "EktronJfuncJs")

    End Sub

#End Region

End Class
