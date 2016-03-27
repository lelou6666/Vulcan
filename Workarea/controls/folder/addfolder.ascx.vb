Imports System.Xml
Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Workarea
Imports Ektron.Cms.DataIO.LicenseManager

Partial Class addfolder
    Inherits System.Web.UI.UserControl

#Region "members"

    Protected _ContentApi As New ContentAPI
    Protected _CustomFieldsApi As New CustomFieldsApi
    Protected _StyleHelper As New StyleHelper
    Protected _MessageHelper As Ektron.Cms.Common.EkMessageHelper
    Protected _Id As Long = 0
    Protected _FolderData As FolderData
    Protected _PermissionData As PermissionData
    Protected _AppPath As String = ""
    Protected _AppImagePath As String = ""
    Protected _ContentType As Integer = 1
    Protected _CurrentUserId As Long = 0
    Protected _PageData As Collection
    Protected _PageAction As String = ""
    Protected _OrderBy As String = ""
    Protected _ContentLanguage As Long = -1
    Protected _EnableMultilingual As Integer = 0
    Protected _SitePath As String = ""
    Protected _FolderId As Long = -1
    Protected _SelectedTaxonomyList As String = ""
    Protected _CurrentCategoryChecked As Integer = 0
    Protected _AssignedFlags As New Hashtable
    Protected _IsCatalog As Boolean = False
    Protected _Type As String = ""
    Protected _ProductType As Commerce.ProductType = Nothing
    Protected _IsPublishedAsPdf As String = String.Empty

    Private _SubscriptionData As SubscriptionData()
    Private _SubscribedData As SubscriptionData()
    Private _SubscriptionProperties As SubscriptionPropertiesData
    Private _IsSubInheritanceBroken As Boolean = False
    Private _IsGlobalSubInheritance As Boolean = False

#End Region

#Region "Page functions"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        RegisterResources()
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        _MessageHelper = _ContentApi.EkMsgRef
        _Type = Request.QueryString("type")
        phTypes.Visible = (_Type = "folder") Or (_Type = "communityfolder") _
                            Or (_Type = "site") Or (_Type = "") Or (_Type = "catalog")
        phTypesPanel.Visible = phTypes.Visible
        phWebAlerts.Visible = (_Type <> "catalog")
    End Sub

    Private Sub RegisterResources()
        'CSS
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)

        'JS
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronDnRJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me._ContentApi.ApplicationPath & "/controls/folder/sitemap.js", "EktronSitemapJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Me._ContentApi.ApplicationPath & "/controls/folder/sitealias.js", "EktronWorkareaSiteAliasJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Me._ContentApi.ApplicationPath & "tree/js/com.ektron.utils.dom.js", "EktronWorkareaTreeUtilsJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub

#End Region

#Region "ACTION - DoAddFolder"
    Private Sub Process_DoAddCalendar()
        Dim calapi As New Ektron.Cms.Content.Calendar.CalendarDal(_ContentApi.RequestInformationRef)
        Dim calendar As New FolderRequest()
        Dim FolderPath As String

        calendar.FolderName = Request.Form("foldername").Trim(".")
        calendar.FolderDescription = Request.Form("folderdescription")
        calendar.ParentId = _Id
        If (Request.Form("TemplateTypeBreak") Is Nothing) Then
            calendar.TemplateFileName = Request.Form("templatefilename")
        Else
            calendar.TemplateFileName = ""
        End If
        calendar.StyleSheet = Request.Form("stylesheet")
        calendar.SiteMapPathInherit = ((Request.Form("hdnInheritSitemap") IsNot Nothing) AndAlso (Request.Form("hdnInheritSitemap").ToString().ToLower() = "true"))
        calendar.SitemapPath = Utilities.DeserializeSitemapPath(Request.Form, Me._ContentLanguage)
        calendar.MetaInherited = IIf((Request.Form("break_inherit_button") IsNot Nothing AndAlso Request.Form("break_inherit_button").ToString().ToLower() = "on"), 1, 0)
        calendar.MetaInheritedFrom = Request.Form("inherit_meta_from")
        calendar.FolderCfldAssignments = Request.Form("folder_cfld_assignments")
        calendar.XmlInherited = False
        calendar.XmlConfiguration = "0"
        calendar.StyleSheet = Request.Form("stylesheet")
        calendar.TaxonomyInherited = Request.Form("TaxonomyTypeBreak") IsNot Nothing AndAlso Request.Form("TaxonomyTypeBreak").ToString().ToLower() = "on"
        calendar.CategoryRequired = Request.Form("CategoryRequired") IsNot Nothing AndAlso Request.Form("CategoryRequired").ToString().ToLower() = "on"
        calendar.TaxonomyInheritedFrom = Request.Form(inherit_taxonomy_from.UniqueID)
        Dim IdRequests As String = ""
        If (Request.Form("taxlist") IsNot Nothing AndAlso Request.Form("taxlist") <> "") Then
            IdRequests = Request.Form("taxlist")
        End If
        calendar.TaxonomyIdList = IdRequests
        calendar.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Calendar
        calendar.IsDomainFolder = False
        calendar.DomainProduction = Request.Form("DomainProduction")
        calendar.DomainStaging = Request.Form("DomainStaging")
        calendar.SubscriptionProperties = New SubscriptionPropertiesData()
        calendar.SubscriptionProperties.BreakInheritance = IIf(Len(Request.Form("webalert_inherit_button")), False, True)
        Select Case Request.Form("notify_option")
            Case ("Always")
                calendar.SubscriptionProperties.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Always
            Case ("Initial")
                calendar.SubscriptionProperties.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Initial
            Case ("Never")
                calendar.SubscriptionProperties.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Never
        End Select
        calendar.SubscriptionProperties.SuspendNextNotification = False
        calendar.SubscriptionProperties.SendNextNotification = False
        calendar.SubscriptionProperties.OptOutID = Request.Form("notify_optoutid")
        calendar.SubscriptionProperties.DefaultMessageID = IIf(Len(Request.Form("use_message_button")), Request.Form("notify_messageid"), 0)
        calendar.SubscriptionProperties.SummaryID = IIf(Len(Request.Form("use_summary_button")), 1, 0)
        calendar.SubscriptionProperties.ContentID = IIf(Len(Request.Form("use_content_button")), Request.Form("frm_content_id"), 0)
        calendar.SubscriptionProperties.UnsubscribeID = Request.Form("notify_unsubscribeid")
        calendar.SubscriptionProperties.URL = IIf(Request.Form("notify_url") <> "", Request.Form("notify_url"), Request.ServerVariables("HTTP_HOST"))
        calendar.SubscriptionProperties.FileLocation = Server.MapPath(_ContentApi.AppPath & "subscriptions")
        calendar.SubscriptionProperties.WebLocation = IIf(Request.Form("notify_weblocation") <> "", Request.Form("notify_weblocation"), "subscriptions")
        calendar.SubscriptionProperties.Subject = IIf(Request.Form("notify_subject") <> "", Request.Form("notify_subject"), "")
        calendar.SubscriptionProperties.EmailFrom = IIf(Request.Form("notify_emailfrom") <> "", Request.Form("notify_emailfrom"), "")
        calendar.SubscriptionProperties.UseContentTitle = ""
        calendar.SubscriptionProperties.UseContentLink = IIf(Len(Request.Form("use_contentlink_button")), 1, 0)
        calendar.ContentSubAssignments = Request.Form("content_sub_assignments")

        Dim calendarid As Long = calapi.AddCalendar(calendar)

        _CustomFieldsApi.ProcessCustomFields(calendarid)

        FolderPath = _ContentApi.EkContentRef.GetFolderPath(calendarid)
        If (Right(FolderPath, 1) = "\") Then
            FolderPath = Right(FolderPath, Len(FolderPath) - 1)
        End If
        FolderPath = Replace(FolderPath, "\", "\\")
        Dim close As String
        close = Request.QueryString("close")
        If (close = "true") Then
            Response.Redirect("close.aspx", False)
        ElseIf (Request.Form(frm_callingpage.UniqueID) = "cmsform.aspx") Then
            Response.Redirect("cmsform.aspx?LangType=" & _ContentLanguage & "&action=ViewAllFormsByFolderID&folder_id=" & calendarid & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        Else
            Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewContentByCategory&id=" & calendarid & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        End If
    End Sub
    Private Sub Process_DoAddFolder()
        Dim tmpPath As String
        Dim libSettings As Collection
        Dim FolderPath As String
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim sub_prop_data As New SubscriptionPropertiesData
        Dim page_subscription_data As New Collection
        Dim page_sub_temp As New Collection
        Dim arrSubscriptions As Array
        Dim isub As Integer = 0
        Dim siteAliasList As New List(Of String)
        Dim arSiteAliasList() As String
        Dim aliasStr As String
        Dim _refSiteAliasApi As Ektron.Cms.SiteAliasApi


        m_refContent = _ContentApi.EkContentRef
        _PageData = New Collection
        _PageData.Add(Request.Form("foldername").Trim("."), "FolderName")
        _PageData.Add(Request.Form("folderdescription"), "FolderDescription")
        _PageData.Add(_Id, "ParentID") 'pagedata.Add(Request.Form("ParentID"), "ParentID")
        If (Request.Form("TemplateTypeBreak") Is Nothing) Then
            _PageData.Add(Request.Form("templatefilename"), "TemplateFileName")
			Dim templateName As String = Request.Form("templatefilename").Split("(")(0).TrimEnd()
            Dim template_data As TemplateData()
            template_data = _ContentApi.GetAllTemplates("TemplateFileName")
            Dim i As Integer = 0
            For i = 0 To template_data.Length - 1
				If (Not Request.Form("tinput_" & template_data(i).Id) Is Nothing AndAlso template_data(i).FileName = templateName) Then
					_PageData.Add(template_data(i).SubType, "TemplateSubType")
				End If
            Next
        Else
            _PageData.Add("", "TemplateFileName")
        End If
        _PageData.Add(Request.Form("stylesheet"), "StyleSheet")
        If ((Request.Form("hdnInheritSitemap") IsNot Nothing) AndAlso (Request.Form("hdnInheritSitemap").ToString().ToLower() = "true")) Then
            _PageData.Add(True, "SitemapPathInherit")
        Else
            _PageData.Add(False, "SitemapPathInherit")
        End If
        _PageData.Add(Utilities.DeserializeSitemapPath(Request.Form, Me._ContentLanguage), "SitemapPath")

        Dim objLib As Ektron.Cms.Library.EkLibrary
        objLib = _ContentApi.EkLibraryRef
        libSettings = objLib.GetLibrarySettingsv2_0()
        tmpPath = libSettings("ImageDirectory")
        _PageData.Add(Server.MapPath(tmpPath), "AbsImageDirectory")
        tmpPath = libSettings("FileDirectory")
        _PageData.Add(Server.MapPath(tmpPath), "AbsFileDirectory")

        If Len(Request.Form("webalert_inherit_button")) Then
            sub_prop_data.BreakInheritance = False
        Else
            sub_prop_data.BreakInheritance = True
        End If

        Select Case Request.Form("notify_option")
            Case ("Always")
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Always
            Case ("Initial")
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Initial
            Case ("Never")
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Never
        End Select

        sub_prop_data.SuspendNextNotification = False
        sub_prop_data.SendNextNotification = False

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
            sub_prop_data.FileLocation = Server.MapPath(_ContentApi.AppPath & "subscriptions")
        Else
            sub_prop_data.FileLocation = Server.MapPath(_ContentApi.AppPath & "subscriptions")
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

        Utilities.AddLBpaths(_PageData)

        If (LCase(Request.Form("TypeBreak")) = "on") Then   ' old field name was frm_xmlinheritance in V7.x
            _PageData.Add(True, "XmlInherited")
        Else
            _PageData.Add(False, "XmlInherited")
        End If
        _PageData.Add(Request.Form("xmlconfig"), "XmlConfiguration")

        Dim isPublishedAsPdf As Boolean = IIf(Request.Form("publishAsPdf") = "on", True, False)
        _PageData.Add(isPublishedAsPdf, "PublishPdfActive")
        _PageData.Add(False, "PublishHtmlActive")

        ' handle dynamic replication properties
        If (Request.Form("EnableReplication") <> "" OrElse Request.QueryString("type") = "communityfolder") Then
            _PageData.Add(Request.Form("EnableReplication"), "EnableReplication")
        Else
            _PageData.Add(0, "EnableReplication")
        End If

        If Not (Request.Form("suppress_notification") <> "") Then
            _PageData.Add(sub_prop_data, "SubscriptionProperties")
            _PageData.Add(page_subscription_data, "Subscriptions")
        End If

        If (Request.Form("break_inherit_button") IsNot Nothing AndAlso Request.Form("break_inherit_button").ToString().ToLower() = "on") Then
            _PageData.Add(0, "break_inherit_button") 'inherit button is checked => Metadata is inherited from parent.
        Else
            _PageData.Add(1, "break_inherit_button")  'break inheritance, do NOT inherit from parent
        End If

        _PageData.Add(Request.Form("folder_cfld_assignments"), "folder_cfld_assignments")

        ' add domain properties if they're there
        If ((Request.Form("IsDomainFolder") <> "") And (Request.Form("DomainProduction") <> "") And LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.MultiSite) AndAlso Not LicenseManager.IsSiteLimitReached(_ContentApi.RequestInformationRef)) Then
            _PageData.Add(True, "IsDomainFolder")
            Dim staging As String = Request.Form("DomainStaging")
            Dim production As String = Request.Form("DomainProduction")
            If (staging Is Nothing) Then
                staging = ""
            End If
            If (staging.EndsWith("/")) Then staging = staging.Substring(0, staging.Length - 1)
            If (production.EndsWith("/")) Then production = production.Substring(0, production.Length - 1)
            If (staging = "") Then
                staging = production
            End If
            _PageData.Add(staging, "DomainStaging")
            _PageData.Add(production, "DomainProduction")
        Else
            _PageData.Add(False, "IsDomainFolder")
        End If
        If (Request.Form("break_inherit_button") IsNot Nothing AndAlso Request.Form("break_inherit_button").ToString().ToLower() = "on") Then
            _PageData.Add(1, "InheritMetadata") 'break inherit button is check.
        Else
            _PageData.Add(0, "InheritMetadata")
        End If
        _PageData.Add(Request.Form("inherit_meta_from"), "InheritMetadataFrom")

        If ((Not Request.QueryString("type") Is Nothing) AndAlso (Request.QueryString("type") = "communityfolder")) Then
            _PageData.Add(True, "IsCommunityFolder")
        End If
        If (Request.Form("TaxonomyTypeBreak") IsNot Nothing AndAlso Request.Form("TaxonomyTypeBreak").ToString().ToLower() = "on") Then
            _PageData.Add(1, "InheritTaxonomy")
            If (Request.Form("CategoryRequired") IsNot Nothing AndAlso Request.Form("CategoryRequired").ToString().ToLower() = "on") Then
                _PageData.Add(1, "CategoryRequired")
            Else
                _PageData.Add(Request.Form(current_category_required.UniqueID), "CategoryRequired")
            End If
        Else
            _PageData.Add(0, "InheritTaxonomy")
            If (Request.Form("CategoryRequired") IsNot Nothing AndAlso Request.Form("CategoryRequired").ToString().ToLower() = "on") Then
                _PageData.Add(1, "CategoryRequired")
            Else
                _PageData.Add(0, "CategoryRequired")
            End If
        End If
        Dim IdRequests As String = ""
        If (Request.Form("taxlist") IsNot Nothing AndAlso Request.Form("taxlist") <> "") Then
            IdRequests = Request.Form("taxlist")
        End If
        _PageData.Add(IdRequests, "TaxonomyList")
        _PageData.Add(Request.Form(inherit_taxonomy_from.UniqueID), "InheritTaxonomyFrom")

        ' Update - add flagging items:
        ProcessFlaggingPostBack(_PageData)

        m_refContent.AddContentFolderv2_0(_PageData)

        '_CustomFieldsApi.ProcessCustomFields(_PageData("FolderID"))

        FolderPath = m_refContent.GetFolderPath(_PageData("ParentID"))
        If (Right(FolderPath, 1) = "\") Then
            FolderPath = Right(FolderPath, Len(FolderPath) - 1)
        End If
        FolderPath = Replace(FolderPath, "\", "\\")
        Dim close As String
        close = Request.QueryString("close")
        If (close = "true") Then
            Response.Redirect("close.aspx", False)
        ElseIf (Request.Form(frm_callingpage.UniqueID) = "cmsform.aspx") Then
            Response.Redirect("cmsform.aspx?LangType=" & _ContentLanguage & "&action=ViewAllFormsByFolderID&folder_id=" & _Id & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        Else
            Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewContentByCategory&id=" & _Id & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        End If

        ' find the folder_id we just created now...
        _FolderId = _ContentApi.EkContentRef.GetFolderID(_ContentApi.EkContentRef.GetFolderPath(_PageData("ParentID")) & "\" & Request.Form("foldername").Trim("."))
        If (_Type = "site") Then
            arSiteAliasList = Request.Form("savedSiteAlias").TrimStart(" ").TrimStart(",").Split(",")
            For Each aliasStr In arSiteAliasList
                If (aliasStr <> String.Empty) Then
                    siteAliasList.Add(aliasStr)
                End If
            Next
            _refSiteAliasApi = New Ektron.Cms.SiteAliasApi()
            _refSiteAliasApi.Save(_FolderId, siteAliasList)
        End If
    End Sub

#End Region

#Region "ACTION - DoAddBlog"
    Private Sub Process_DoAddBlog()
        Dim tmpPath As String
        Dim libSettings As Collection
        Dim FolderPath As String
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim isub As Integer = 0
        Dim i As Integer = 0
        Dim abriRoll As BlogRollItem()
        Dim sCatTemp As String = ""

        m_refContent = _ContentApi.EkContentRef
        _PageData = New Collection
        _PageData.Add(True, "IsBlog")
        _PageData.Add(Request.Form(txtBlogName.UniqueID).Trim("."), "FolderName")
        _PageData.Add("", "FolderDescription")
        _PageData.Add(Request.Form(txtTitle.UniqueID), "BlogTitle")
        _PageData.Add(Request.Form(drpVisibility.UniqueID), "BlogVisible")
        _PageData.Add(Request.Form(chkEnable.UniqueID), "CommentEnable")
        _PageData.Add(Request.Form(chkModerate.UniqueID), "CommentModerate")
        _PageData.Add(False, "SitemapPathInherit")
        _PageData.Add(Request.Form(chkRequire.UniqueID), "CommentRequire")
        _PageData.Add(Request.Form(hdnfolderid.UniqueID), "ParentID") 'pagedata.Add(Request.Form("ParentID"), "ParentID")
        If (Request.Form("TemplateTypeBreak") Is Nothing) Then
            _PageData.Add(Request.Form("templatefilename"), "TemplateFileName")
        Else
            _PageData.Add("", "TemplateFileName")
        End If
        If _ContentApi.SitePath = "/" Then
            _PageData.Add(Replace(_ContentApi.AppPath, _ContentApi.SitePath, "") & "/csslib/blogs.css", "StyleSheet")
        Else
            _PageData.Add(Replace(_ContentApi.AppPath, _ContentApi.SitePath, "") & "csslib/blogs.css", "StyleSheet")
        End If

        Dim objLib As Ektron.Cms.Library.EkLibrary
        objLib = _ContentApi.EkLibraryRef
        libSettings = objLib.GetLibrarySettingsv2_0()
        tmpPath = libSettings("ImageDirectory")
        _PageData.Add(Server.MapPath(tmpPath), "AbsImageDirectory")
        tmpPath = libSettings("FileDirectory")
        _PageData.Add(Server.MapPath(tmpPath), "AbsFileDirectory")
        Utilities.AddLBpaths(_PageData)

        _PageData.Add(True, "XmlInherited")
        _PageData.Add(Request.Form("xmlconfig"), "XmlConfiguration")
        _PageData.Add(False, "PublishPdfActive")
        _PageData.Add(False, "PublishHtmlActive")
        _PageData.Add(False, "IsDomainFolder")
        ' handle dynamic replication properties
        If (Request.Form("EnableReplication") <> "") Then
            _PageData.Add(Request.Form("EnableReplication"), "EnableReplication")
        Else
            _PageData.Add(0, "EnableReplication")
        End If
        If Request.Form("categorylength") <> "" Then
            For i = 0 To (Request.Form("categorylength") - 1)
                If Request.Form("category" & i.ToString()) <> "" Then
                    If i = (Request.Form("categorylength") - 1) Then
                        sCatTemp &= Request.Form("category" & i.ToString())
                    Else
                        sCatTemp &= Request.Form("category" & i.ToString()) & ";"
                    End If
                End If
            Next
        End If
        _PageData.Add(sCatTemp, "blogcategories")

        'Start Taxonomy Addition
        If (Request.Form("TaxonomyTypeBreak") IsNot Nothing AndAlso Request.Form("TaxonomyTypeBreak").ToString().ToLower() = "on") Then
            _PageData.Add(1, "InheritTaxonomy")
            If (Request.Form("CategoryRequired") IsNot Nothing AndAlso Request.Form("CategoryRequired").ToString().ToLower() = "on") Then
                _PageData.Add(1, "CategoryRequired")
            Else
                _PageData.Add(Request.Form(current_category_required.UniqueID), "CategoryRequired")
            End If
        Else
            _PageData.Add(0, "InheritTaxonomy")
            If (Request.Form("CategoryRequired") IsNot Nothing AndAlso Request.Form("CategoryRequired").ToString().ToLower() = "on") Then
                _PageData.Add(1, "CategoryRequired")
            Else
                _PageData.Add(0, "CategoryRequired")
            End If
        End If
        Dim IdRequests As String = ""
        If (Request.Form("taxlist") IsNot Nothing AndAlso Request.Form("taxlist") <> "") Then
            IdRequests = Request.Form("taxlist")
        End If
        'End Taxonomy Addition 

        _PageData.Add(IdRequests, "TaxonomyList")
        _PageData.Add(Request.Form(inherit_taxonomy_from.UniqueID), "InheritTaxonomyFrom")
        If Request.Form("rolllength") <> "" Then
            ReDim abriRoll(0)
            For i = 0 To (Request.Form("rolllength") - 1)
                ReDim Preserve abriRoll(i)
                If Request.Form("editfolder_linkname" & i.ToString()) <> "" And Request.Form("editfolder_url" & i.ToString()) <> "" Then
                    'add only if we have something with a name/url
                    abriRoll(i) = New BlogRollItem
                    abriRoll(i).LinkName = Request.Form("editfolder_linkname" & i.ToString())
                    abriRoll(i).URL = Request.Form("editfolder_url" & i.ToString())
                    If Request.Form("editfolder_short" & i.ToString()) <> "" Then
                        abriRoll(i).ShortDescription = Request.Form("editfolder_short" & i.ToString())
                    Else
                        abriRoll(i).ShortDescription = ""
                    End If
                    If Request.Form("editfolder_rel" & i.ToString()) <> "" Then
                        abriRoll(i).Relationship = Request.Form("editfolder_rel" & i.ToString())
                    Else
                        abriRoll(i).Relationship = ""
                    End If
                Else
                    abriRoll(i) = Nothing
                End If
            Next
            _PageData.Add(abriRoll, "blogroll")
        Else
            _PageData.Add(Nothing, "blogroll")
        End If

        _PageData.Add("1", "break_inherit_button")
        _PageData.Add("", "folder_cfld_assignments")
        _PageData.Add(1, "InheritMetadata") 'break inherit button is check.
        _PageData.Add(0, "InheritMetadataFrom")

        m_refContent.AddContentFolderv2_0(_PageData)

        FolderPath = m_refContent.GetFolderPath(_PageData("ParentID"))
        If (Right(FolderPath, 1) = "\") Then
            FolderPath = Right(FolderPath, Len(FolderPath) - 1)
        End If
        FolderPath = Replace(FolderPath, "\", "\\")
        Dim close As String
        close = Request.QueryString("close")
        If (close = "true") Then
            Response.Redirect("close.aspx", False)
        ElseIf (Request.Form(frm_callingpage.UniqueID) = "cmsform.aspx") Then
            Response.Redirect("cmsform.aspx?LangType=" & _ContentLanguage & "&action=ViewAllFormsByFolderID&folder_id=" & _Id & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        Else
            Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewContentByCategory&id=" & _Id & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        End If
        _FolderId = _PageData("FolderID")

    End Sub
#End Region

#Region "Action - DoAddDiscussionBoard"

    Private Sub Process_DoAddDiscussionBoard()
        Dim tmpPath As String
        Dim libSettings As Collection
        Dim FolderPath As String
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim isub As Integer = 0
        Dim i As Integer = 0
        Dim sCatTemp As String = ""

        m_refContent = _ContentApi.EkContentRef
        _PageData = New Collection
        _PageData.Add(True, "IsDiscussionBoard")
        _PageData.Add(Request.Form(txt_adb_boardname.UniqueID).Trim("."), "FolderName")
        _PageData.Add(Request.Form(txt_adb_title.UniqueID), "FolderDescription")
        If Request.Form(chk_adb_mc.UniqueID) <> "" Then
            _PageData.Add(True, "CommentModerate")
        Else
            _PageData.Add(False, "CommentModerate")
        End If
        If Request.Form(chk_adb_ra.UniqueID) <> "" Then
            _PageData.Add(True, "CommentRequire")
        Else
            _PageData.Add(False, "CommentRequire")
        End If
        _PageData.Add(Request.Form(hdn_adb_folderid.UniqueID), "ParentID") 'pagedata.Add(Request.Form("ParentID"), "ParentID")
        _PageData.Add("", "TemplateFileName")
        _PageData.Add(False, "SitemapPathInherit")
        Dim sJustAppPath As String = _ContentApi.AppPath.Replace(_ContentApi.SitePath, "")
        If sJustAppPath.Length > 0 AndAlso Not (sJustAppPath(sJustAppPath.Length - 1) = "/") Then
            sJustAppPath = sJustAppPath & "/"
        End If
        Dim sCSS As String = ""
        If Request.Form(txt_adb_stylesheet.UniqueID) <> "" Then
            sCSS = Request.Form(txt_adb_stylesheet.UniqueID)
        End If
        _PageData.Add(sCSS, "StyleSheet") 'use default forum CSS

        Dim objLib As Ektron.Cms.Library.EkLibrary
        objLib = _ContentApi.EkLibraryRef
        libSettings = objLib.GetLibrarySettingsv2_0()
        tmpPath = libSettings("ImageDirectory")
        _PageData.Add(Server.MapPath(tmpPath), "AbsImageDirectory")
        tmpPath = libSettings("FileDirectory")
        _PageData.Add(Server.MapPath(tmpPath), "AbsFileDirectory")
        Utilities.AddLBpaths(_PageData)

        _PageData.Add(False, "XmlInherited")
        _PageData.Add(Request.Form("xmlconfig"), "XmlConfiguration")
        _PageData.Add(False, "PublishPdfActive")
        _PageData.Add(False, "PublishHtmlActive")
        _PageData.Add(False, "IsDomainFolder")
        ' handle dynamic replication properties
        If (Request.Form("EnableReplication") <> "") Then
            _PageData.Add(Request.Form("EnableReplication"), "EnableReplication")
        Else
            _PageData.Add(0, "EnableReplication")
        End If
        If Request.Form("categorylength") <> "" Then
            For i = 0 To (Request.Form("categorylength") - 1)
                If Request.Form("category" & i.ToString()) <> "" Then
                    If i = (Request.Form("categorylength") - 1) Then
                        sCatTemp &= Request.Form("category" & i.ToString())
                    Else
                        sCatTemp &= Request.Form("category" & i.ToString()) & ";"
                    End If
                End If
            Next
        End If
        _PageData.Add(sCatTemp, "DiscussionBoardCategories")

        _PageData.Add("", "break_inherit_button")
        _PageData.Add("", "folder_cfld_assignments")
        _PageData.Add(0, "InheritMetadata") 'break inherit button is check.
        _PageData.Add(0, "InheritMetadataFrom")
        m_refContent.AddContentFolderv2_0(_PageData)

        FolderPath = m_refContent.GetFolderPath(_PageData("ParentID"))
        If (Right(FolderPath, 1) = "\") Then
            FolderPath = Right(FolderPath, Len(FolderPath) - 1)
        End If
        FolderPath = Replace(FolderPath, "\", "\\")
        Dim close As String
        close = Request.QueryString("close")
        If (close = "true") Then
            Response.Redirect("close.aspx", False)
        ElseIf (Request.Form(frm_callingpage.UniqueID) = "cmsform.aspx") Then
            Response.Redirect("cmsform.aspx?LangType=" & _ContentLanguage & "&action=ViewAllFormsByFolderID&folder_id=" & _Id & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        Else
            Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewContentByCategory&id=" & _Id & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        End If

        ' find the folder_id we just created now...
        _FolderId = _PageData("FolderID")
    End Sub

#End Region

#Region "Action - DoAddDiscussionForum"

    Private Sub Process_DoAddDiscussionForum()
        Dim tmpPath As String
        Dim libSettings As Collection
        Dim FolderPath As String
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim isub As Integer = 0
        Dim i As Integer = 0
        Dim sCatTemp As String = ""

        m_refContent = _ContentApi.EkContentRef
        _PageData = New Collection
        _PageData.Add(True, "IsDiscussionForum")
        _PageData.Add(Request.Form(txt_adf_forumname.UniqueID).Trim("."), "FolderName")
        _PageData.Add(Request.Form(txt_adf_forumtitle.UniqueID), "FolderDescription")
        If IsNumeric(Request.Form(txt_adf_sortorder.UniqueID)) AndAlso Request.Form(txt_adf_sortorder.UniqueID) > 0 Then
            _PageData.Add(Request.Form(txt_adf_sortorder.UniqueID), "SortOrder")
        Else
            _PageData.Add(1, "SortOrder")
        End If
        If Request.Form(chk_adf_moderate.UniqueID) <> "" Then
            _PageData.Add(True, "CommentModerate")
            Response.Write("true")
        Else
            _PageData.Add(False, "CommentModerate")
            Response.Write("false")
        End If
        If Request.Form(chk_adf_lock.UniqueID) <> "" Then
            _PageData.Add(True, "LockForum")
            Response.Write("true")
        Else
            _PageData.Add(False, "LockForum")
            Response.Write("false")
        End If
        ' handle dynamic replication properties
        If (Request.Form("EnableReplication") <> "") Then
            _PageData.Add(Request.Form("EnableReplication"), "EnableReplication")
        Else
            _PageData.Add(0, "EnableReplication")
        End If
        _PageData.Add(Request.Form(drp_adf_category.UniqueID), "CategoryID")
        Response.Write(Request.Form(drp_adf_category.UniqueID))
        _PageData.Add(False, "SitemapPathInherit")
        _PageData.Add(Request.Form(hdn_adf_folderid.UniqueID), "ParentID")
        _PageData.Add("", "TemplateFileName")
        _PageData.Add(Replace(_ContentApi.AppPath, _ContentApi.SitePath, "") & "csslib/dicussionboard.css", "StyleSheet")

        Dim objLib As Ektron.Cms.Library.EkLibrary
        objLib = _ContentApi.EkLibraryRef
        libSettings = objLib.GetLibrarySettingsv2_0()
        tmpPath = libSettings("ImageDirectory")
        _PageData.Add(Server.MapPath(tmpPath), "AbsImageDirectory")
        tmpPath = libSettings("FileDirectory")
        _PageData.Add(Server.MapPath(tmpPath), "AbsFileDirectory")
        Utilities.AddLBpaths(_PageData)

        _PageData.Add(False, "XmlInherited")
        _PageData.Add(Request.Form("xmlconfig"), "XmlConfiguration")
        _PageData.Add(False, "PublishPdfActive")
        _PageData.Add(False, "PublishHtmlActive")
        _PageData.Add(False, "IsDomainFolder")

        _PageData.Add("", "break_inherit_button")
        _PageData.Add("", "folder_cfld_assignments")
        _PageData.Add(0, "InheritMetadata") 'break inherit button is check.
        _PageData.Add(0, "InheritMetadataFrom")
        m_refContent.AddContentFolderv2_0(_PageData)

        FolderPath = m_refContent.GetFolderPath(_PageData("ParentID"))
        If (Right(FolderPath, 1) = "\") Then
            FolderPath = Right(FolderPath, Len(FolderPath) - 1)
        End If
        FolderPath = Replace(FolderPath, "\", "\\")
        Dim close As String
        close = Request.QueryString("close")
        If (close = "true") Then
            Response.Redirect("close.aspx", False)
        ElseIf (Request.Form(frm_callingpage.UniqueID) = "cmsform.aspx") Then
            Response.Redirect("cmsform.aspx?LangType=" & _ContentLanguage & "&action=ViewAllFormsByFolderID&folder_id=" & _Id & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        Else
            Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewContentByCategory&id=" & _Id & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        End If
    End Sub

#End Region

#Region "FOLDER - AddSubFolder"

    Public Function AddSubFolder() As Boolean
        If (Not (Request.QueryString("id") Is Nothing)) Then
            _Id = Convert.ToInt64(Request.QueryString("id"))
        End If
        If (Not (Request.QueryString("action") Is Nothing)) Then
            _PageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If
        If (Not (Request.QueryString("orderby") Is Nothing)) Then
            _OrderBy = Convert.ToString(Request.QueryString("orderby"))
        End If
        If Request.QueryString("type") <> "" Then
            _Type = Request.QueryString("type").ToLower()
        End If
        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            If (Request.QueryString("LangType") <> "") Then
                _ContentLanguage = Convert.ToInt64(Request.QueryString("LangType"))
                _ContentApi.SetCookieValue("LastValidLanguageID", _ContentLanguage)
            Else
                If _ContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    _ContentLanguage = Convert.ToInt64(_ContentApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If _ContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                _ContentLanguage = Convert.ToInt64(_ContentApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If
        If _ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            _ContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            _ContentApi.ContentLanguage = _ContentLanguage
        End If
        _CurrentUserId = _ContentApi.UserId
        _AppImagePath = _ContentApi.AppImgPath
        _AppPath = _ContentApi.AppPath
        _SitePath = _ContentApi.SitePath
        _EnableMultilingual = _ContentApi.EnableMultilingual
        If (Not (Page.IsPostBack)) Then
            Select Case _Type
                Case "blog"
                    Display_AddBlog()
                    Display_BlogJS()
                Case "discussionboard"
                    Display_AddDiscussionBoard()
                    Display_DiscussionBoardJS()
                Case "discussionforum"
                    Display_AddDiscussionForum()
                    Display_DiscussionForumJS()
                Case "catalog"
                    _IsCatalog = True
                    Display_AddCatalog()
                    Display_CatalogJS()
                Case "calendar"
                    Display_AddCalendar()
                    Display_FolderJS()
                Case Else
                    Display_FolderJS()
                    Display_AddSubFolder()
            End Select
        Else
            If Request.Form(txtBlogName.UniqueID) <> "" Then
                Process_DoAddBlog()
                ProcessContentTemplatesPostBack()
            ElseIf Request.Form(txt_adb_boardname.UniqueID) <> "" Then
                Process_DoAddDiscussionBoard()
                ProcessContentTemplatesPostBack("forum")
            ElseIf Request.Form(txt_adf_forumname.UniqueID) <> "" Then
                Process_DoAddDiscussionForum()
            ElseIf _Type = "catalog" Then
                Process_DoAddCatalog()
                ProcessProductTemplatesPostBack()
            ElseIf _Type = "calendar" Then
                Process_DoAddCalendar()
            Else
                Process_DoAddFolder()
                ProcessContentTemplatesPostBack()
            End If

            Return (True)
        End If
        If _Type = "catalog" Then
            DrawProductTypesTable()
        Else
            DrawContentTypesTable()
        End If
        DrawContentTemplatesTable()
        DrawFlaggingOptions()
        If Request.QueryString("type") <> "" AndAlso Request.QueryString("type").ToLower() = "communityfolder" Then
            Display_AddCommunityFolder()
        End If

    End Function
    Private Sub Display_FolderJS()
        Dim sbfolderjs As New StringBuilder
        sbfolderjs.Append("Ektron.ready(function() {" & Environment.NewLine)
        sbfolderjs.Append(" document.forms[0].foldername.onkeypress = document.forms[0].netscape.onkeypress;" & Environment.NewLine)
        sbfolderjs.Append(" document.forms[0].stylesheet.onkeypress = document.forms[0].netscape.onkeypress;" & Environment.NewLine)
        sbfolderjs.Append(" document.forms[0].templatefilename.onkeypress = document.forms[0].netscape.onkeypress;" & Environment.NewLine)
        sbfolderjs.Append(" document.forms[0].foldername.focus();" & Environment.NewLine)
        sbfolderjs.Append("   if( $ektron('#webalert_inherit_button').length > 0 ){ " & Environment.NewLine)
        sbfolderjs.Append("     if( $ektron('#webalert_inherit_button')[0].checked ){ " & Environment.NewLine)
        sbfolderjs.Append("       $ektron('.selectContent').css('display', 'none');" & Environment.NewLine)
        sbfolderjs.Append("       $ektron('.useCurrent').css('display', 'none');" & Environment.NewLine)
        sbfolderjs.Append("     } " & Environment.NewLine)
        sbfolderjs.Append("    } " & Environment.NewLine)
        sbfolderjs.Append(" });" & Environment.NewLine)

        ltr_af_js.Text = sbfolderjs.ToString()
    End Sub
    Private Sub Display_AddSubFolder()
        Dim ShowTaxonomy As Boolean = False
        Dim strFolderAddType As String = "folder"
        If Not String.IsNullOrEmpty(Request.QueryString("type")) Then
            strFolderAddType = Request.QueryString("type").ToLower()
        End If

        If strFolderAddType = "blog" Then
            Display_AddBlog()
            Display_BlogJS()
            Exit Sub
        ElseIf strFolderAddType = "discussionboard" Then
            Display_AddDiscussionBoard()
            Display_DiscussionBoardJS()
            Exit Sub
        ElseIf strFolderAddType = "discussionforum" Then
            Display_AddDiscussionForum()
            Display_DiscussionForumJS()
            Exit Sub
        Else
            ShowTaxonomy = True
            Display_FolderJS()
        End If
        Dim xmlconfig_data() As XmlConfigData
        Dim template_data() As TemplateData
        Dim backup As String = ""
        Dim strStyleSheet As String = ""
        Dim i As Integer = 0

        ltrTypes.Text = _MessageHelper.GetMessage("Smart Forms txt")
        ltInheritSitemapPath.Text = _MessageHelper.GetMessage("lbl inherit parent configuration")
		
		Try
            Dim asset_config As AssetConfigInfo() = _ContentApi.GetAssetMgtConfigInfo()
            If asset_config(10).Value.IndexOf("ektron.com") > -1 Then
                ltrCheckPdfServiceProvider.Text = _MessageHelper.GetMessage("pdf service provider")
            Else
                ltrCheckPdfServiceProvider.Text = ""
            End If
        Catch ex As Exception
            Dim _error As String = ex.Message
        End Try

        If (_Type = "site") Then
            lblSiteAlias.Visible = True
            phSiteAlias.Visible = True
            phSiteAlias2.Visible = True
        Else
            phSiteAlias2.Visible = False
            phSiteAlias.Visible = False
            lblSiteAlias.Visible = False
        End If


        _FolderData = _ContentApi.GetFolderById(_Id, True, True)
        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder")
        AddFolderToolBar()

        template_data = _ContentApi.GetAllTemplates("TemplateFileName")
        xmlconfig_data = _ContentApi.GetAllXmlConfigurations("title")
        backup = _StyleHelper.getCallBackupPage("content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage)

        If (_FolderData.StyleSheet = "") Then
            strStyleSheet = _ContentApi.GetStyleSheetByFolderID(_Id)
        End If
        Dim folder_template_data As TemplateData
        folder_template_data = _ContentApi.GetTemplatesByFolderId(_Id) 'VERIFY for dimension
        If strFolderAddType = "site" Then
            tdfoldernamelabel.InnerHtml = _MessageHelper.GetMessage("sitename label")
        Else
            tdfoldernamelabel.InnerHtml = _MessageHelper.GetMessage("foldername label")
        End If

        tdsitepath.InnerHtml = _SitePath & "<input type=""text"" maxlength=""255"" size=""" & 75 - Len(_SitePath) & """" & " value="""" name=""stylesheet"">"
        tdsitepath.InnerHtml &= "<div class=""ektronCaption"">" & _MessageHelper.GetMessage("leave blank to inherit msg") & "</div>"
        ltrTemplateFilePath.Text = "<input type=""hidden"" maxlength=""255"" size=""" & 75 - Len(_SitePath) & """ value="""" name=""templatefilename"" id=""templatefilename"">"
        ltrTemplateFilePath.Text &= "<div id=""FrameContainer"" class=""ektronWindow ektronModalStandard"">"
        ltrTemplateFilePath.Text &= "<iframe id=""ChildPage"" name=""ChildPage"">"
        ltrTemplateFilePath.Text &= "</iframe>"
        ltrTemplateFilePath.Text &= "</div>"

        If _FolderData.PublishPdfEnabled Then
            phPDF.Visible = True
            _IsPublishedAsPdf = IIf(_FolderData.PublishPdfActive, "checked=""checked"" ", String.Empty)
            Me.lblPublishAsPdf.InnerText = _MessageHelper.GetMessage("publish as pdf")
        Else
            _IsPublishedAsPdf = String.Empty
            phPDF.Visible = False
        End If

        ' only top level folders can be domain folders
        Dim m_refCommonAPI As New CommonApi()
        Dim request_info As Ektron.Cms.Common.EkRequestInformation = m_refCommonAPI.RequestInformationRef

        If (_Id = 0 And strFolderAddType = "site") Then
            If (LicenseManager.IsSiteLimitReached(request_info)) Then
                Utilities.ShowError(_MessageHelper.GetMessage("com: max sites reached"))
            End If
            If (LicenseManager.IsFeatureEnable(request_info, Feature.MultiSite)) Then
                phProductionDomain.Visible = True
                tdstagingdomain.InnerHtml = "http://&nbsp;<input type=""text"" size=""68"" name=""DomainStaging"" id=""DomainStaging"" size=""50"" value=""" & _FolderData.DomainStaging & """>"
                tdproductiondomain.InnerHtml = "http://&nbsp;<input type=""text"" size=""68"" name=""DomainProduction"" id=""DomainProduction"" size=""50"" value=""" & _FolderData.DomainProduction & """>"
                tdproductiondomain.InnerHtml += "<input type=""hidden"" name=""IsDomainFolder"" id=""IsDomainFolder"" value=""true""/>"

            End If
        End If
        If (ShowTaxonomy) Then
            DrawFolderTaxonomyTable()
        End If
        ' handle dynamic replication settings
        If request_info.EnableReplication Then
            Dim schk As String = ""
            If (_FolderData.ReplicationMethod = 1) Then
                schk = " checked"
            End If

            If (Not (Request.QueryString("type") <> "" AndAlso Request.QueryString("type").ToLower() = "communityfolder")) Then
                ' see if we should warn users about inherited metadata
                Dim fWarnMeta As Boolean = True
                If (_FolderData.ReplicationMethod = 1) Then
                    ' parent folder is QD enabled, so no need for warning
                    fWarnMeta = False
                End If

                ReplicationMethod.Text = "<tr><td colspan=""2"">&nbsp;</td></tr><tr><td colspan=""2"" class=""label"">" + _MessageHelper.GetMessage("lbl folderdynreplication") + "</td></tr>"
                ReplicationMethod.Text += "<tr><td colspan=""2""><input type=""checkbox"" name=""EnableReplication"" id=""EnableReplication"" value=""1""" & schk
                If (fWarnMeta) Then
                    ReplicationMethod.Text += " onclick=""if (!document.getElementById('break_inherit_button').checked && document.getElementById('EnableReplication').checked) {alert('"
                    ReplicationMethod.Text += _MessageHelper.GetMessage("js: alert qd metainherit")
                    ReplicationMethod.Text += "');}"""
                End If

                ReplicationMethod.Text += " ><label for=""EnableReplication"">" & _MessageHelper.GetMessage("replicate folder contents") & "</label></td></tr>"
            End If

        End If

        ParentID.Value = _Id
        frm_callingpage.Value = _StyleHelper.getCallingpage("content.aspx")
        lit_vf_customfieldassingments.Text = _CustomFieldsApi.AddNewEditableCustomFieldAssignments(_Id)
        DisplaySubscriptionInfo()
        DisplaySitemapPath()
        DisplaySiteAlias()
    End Sub
    Private Sub Display_AddCalendar()
        Display_FolderJS()
        Dim template_data() As TemplateData
        Dim backup As String = ""
        Dim strStyleSheet As String = ""
        Dim i As Integer = 0

        ltInheritSitemapPath.Text = _MessageHelper.GetMessage("lbl inherit parent configuration")
        ltrTypes.Text = _MessageHelper.GetMessage("Smart Forms txt")

        phSiteAlias2.Visible = False
        phSiteAlias.Visible = False
        lblSiteAlias.Visible = False
        phTypes.Visible = False
        ltr_vf_types.Visible = False

        _FolderData = _ContentApi.GetFolderById(_Id, True, True)
        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder")
        AddCalendarToolBar()

        template_data = _ContentApi.GetAllTemplates("TemplateFileName")
        backup = _StyleHelper.getCallBackupPage("content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage)

        If (_FolderData.StyleSheet = "") Then
            strStyleSheet = _ContentApi.GetStyleSheetByFolderID(_Id)
        End If
        Dim folder_template_data As TemplateData
        folder_template_data = _ContentApi.GetTemplatesByFolderId(_Id) 'VERIFY for dimension
        tdfoldernamelabel.InnerHtml = _MessageHelper.GetMessage("calendarname label")

        tdsitepath.InnerHtml = _SitePath & "<input type=""text"" maxlength=""255"" size=""" & 75 - Len(_SitePath) & """" & " value="""" name=""stylesheet"">"
        tdsitepath.InnerHtml &= "<div class=""ektronCaption"">" & _MessageHelper.GetMessage("leave blank to inherit msg") & "</div>"
        ltrTemplateFilePath.Text = "<input type=""hidden"" maxlength=""255"" size=""" & 75 - Len(_SitePath) & """ value="""" name=""templatefilename"" id=""templatefilename"">"

        _IsPublishedAsPdf = String.Empty
        phPDF.Visible = False

        ' only top level folders can be domain folders
        Dim m_refCommonAPI As New CommonApi()
        Dim request_info As Ektron.Cms.Common.EkRequestInformation = m_refCommonAPI.RequestInformationRef

        DrawFolderTaxonomyTable()
        ' handle dynamic replication settings
        If request_info.EnableReplication Then
            Dim schk As String = ""
            If (_FolderData.ReplicationMethod = 1) Then
                schk = " checked"
            End If
        End If

        ParentID.Value = _Id
        frm_callingpage.Value = _StyleHelper.getCallingpage("content.aspx")
        lit_vf_customfieldassingments.Text = _CustomFieldsApi.AddNewEditableCustomFieldAssignments(_Id)
        DisplaySubscriptionInfo()
        DisplaySitemapPath()
        DisplaySiteAlias()
    End Sub
    Private checktaxid As Long = 0
    Private Sub DrawFolderTaxonomyTable()
        Dim categorydatatemplate As String = "<li><input type=""checkbox"" id=""taxlist"" onclick=""ValidateCatSel(this)"" name=""taxlist"" value=""{0}"" {1} disabled/>{2}</li>"
        Dim categorydata As New StringBuilder
        Dim scatcheck As String = ""
        If (_FolderData.FolderTaxonomy IsNot Nothing AndAlso _FolderData.FolderTaxonomy.Length > 0) Then
            For i As Integer = 0 To _FolderData.FolderTaxonomy.Length - 1
                If (_SelectedTaxonomyList.Length > 0) Then
                    _SelectedTaxonomyList = _SelectedTaxonomyList & "," & _FolderData.FolderTaxonomy(i).TaxonomyId
                Else
                    _SelectedTaxonomyList = _FolderData.FolderTaxonomy(i).TaxonomyId
                End If
            Next
        End If
        _CurrentCategoryChecked = Convert.ToInt64(_FolderData.CategoryRequired)
        inherit_taxonomy_from.Value = _FolderData.TaxonomyInheritedFrom
        current_category_required.Value = _CurrentCategoryChecked
        If (_FolderData.CategoryRequired) Then
            scatcheck = " checked "
        End If
        Dim TaxArr As TaxonomyBaseData() = _ContentApi.EkContentRef.GetAllTaxonomyByConfig(Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content)
        Dim parent_has_configuration As Boolean = False
        If (TaxArr IsNot Nothing AndAlso TaxArr.Length > 0) Then
            categorydata.Append("<ul style=""list-style:none;margin:0;"">")
            Dim i As Integer = 0
            While (i < TaxArr.Length)
                For j As Integer = 0 To 2
                    If (i < TaxArr.Length) Then
                        checktaxid = TaxArr(i).TaxonomyId
                        parent_has_configuration = Array.Exists(_FolderData.FolderTaxonomy, AddressOf TaxonomyExists)
                        categorydata.Append(String.Format(categorydatatemplate, TaxArr(i).TaxonomyId, IsChecked(parent_has_configuration), TaxArr(i).TaxonomyName))
                        i = i + 1
                    Else
                        Exit For
                    End If
                Next
            End While
            categorydata.Append("</ul>")
        End If

        Dim str As New StringBuilder()

        str.Append("<input type=""hidden"" id=""TaxonomyParentHasConfig"" name=""TaxonomyParentHasConfig"" value=""")
        If (parent_has_configuration) Then
            str.Append("1")
        Else
            str.Append("0")
        End If

        str.Append(""" />")

        str.Append("<input name=""TaxonomyTypeBreak"" id=""TaxonomyTypeBreak"" type=""checkbox"" onclick=""ToggleTaxonomyInherit(this)"" checked/>" & _MessageHelper.GetMessage("lbl inherit parent configuration"))
        str.Append("<br />")
        str.Append("<input name=""CategoryRequired"" id=""CategoryRequired"" type=""checkbox"" " & scatcheck & " disabled/>" & _MessageHelper.GetMessage("alt Required at least one category selection"))
        str.Append("<br />")
        str.Append("<br />")
        str.Append(categorydata.ToString())
        taxonomy_list.Text = str.ToString()
        litBlogTaxonomy.Text = str.ToString()
    End Sub
    Private Function IsChecked(ByVal value As Boolean) As String
        If (value) Then
            Return " checked=""checked"""
        Else
            Return " "
        End If
    End Function
    Private Function TaxonomyExists(ByVal data As TaxonomyBaseData) As Boolean
        If (data IsNot Nothing) Then
            If (data.TaxonomyId = checktaxid) Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function
    Private Sub DisplaySitemapPath()
        Dim sJS As New System.Text.StringBuilder
        Dim node As Ektron.Cms.Common.SitemapPath
        sJS.AppendLine("var clientName_chkInheritSitemapPath = 'chkInheritSitemapPath';")
        'chkInheritSitemapPath.Checked = True
        sJS.AppendLine("document.getElementById(""hdnInheritSitemap"").value = 'true';")
        sJS.AppendLine("document.getElementById(""chkInheritSitemapPath"").checked = true;")
        sJS.AppendLine("document.getElementById(""AddSitemapNode"").style.display = 'none';")

        If (_FolderData.SitemapPath IsNot Nothing) Then
            sJS.Append("arSitemapPathNodes = new Array(")
            For Each node In _FolderData.SitemapPath
                If (node IsNot Nothing) Then
                    If (node.Order <> 0) Then
                        sJS.Append(",")
                    End If
                    sJS.Append("new node('" & node.Title & "','" & node.Url & "','" & node.Description & "'," & node.Order & ")")
                End If
            Next
            sJS.AppendLine(");")
            sJS.AppendLine("renderSiteMapNodes();")
        End If
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "renderSitepath", sJS.ToString(), True)

    End Sub
    Private Sub DisplaySiteAlias()
        'Dim sJS As New System.Text.StringBuilder
        'Dim node As Ektron.Cms.Common.SitemapPath
        'sJS.AppendLine("var clientName_chkInheritSitemapPath = 'chkInheritSitemapPath';")
        ''chkInheritSitemapPath.Checked = True
        'sJS.AppendLine("document.getElementById(""hdnInheritSitemap"").value = 'true';")
        'sJS.AppendLine("document.getElementById(""chkInheritSitemapPath"").checked = true;")

        'If (folder_data.SitemapPath IsNot Nothing) Then
        '    sJS.Append("arSitemapPathNodes = new Array(")
        '    For Each node In folder_data.SitemapPath
        '        If (node IsNot Nothing) Then
        '            If (node.Order <> 0) Then
        '                sJS.Append(",")
        '            End If
        '            sJS.Append("new node('" & node.Title & "','" & node.Url & "','" & node.Description & "'," & node.Order & ")")
        '        End If
        '    Next
        '    sJS.AppendLine(");")
        '    sJS.AppendLine("renderSiteMapNodes();")
        'End If
        'Page.ClientScript.RegisterStartupScript(Me.GetType(), "renderSitepath", sJS.ToString(), True)

    End Sub
    Private Sub AddCalendarToolBar()
        Dim result As New System.Text.StringBuilder
        Dim backup As String = ""
        Dim close As String
        close = Request.QueryString("close")
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("btn add calendar") & " """ & _FolderData.Name & """")
        backup = _StyleHelper.getCallBackupPage("content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage)
        result.Append("<table><tr>")
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("add calendar folder"), _MessageHelper.GetMessage("add calendar folder"), "onclick=""return SubmitForm('frmContent', 'CheckFolderParameters(\'add\')')"""))
        If (close <> "true") Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", backup, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton("calendar_" & _PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub AddFolderToolBar()
        Dim result As New System.Text.StringBuilder
        Dim backup As String = ""
        Dim close As String
        close = Request.QueryString("close")
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("add subfolder msg") & " """ & _FolderData.Name & """")
        backup = _StyleHelper.getCallBackupPage("content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage)
        result.Append("<table><tr>")
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt add folder button text"), _MessageHelper.GetMessage("btn add folder"), "onclick=""return SubmitForm('frmContent', 'CheckFolderParameters(\'add\')')"""))
        If (close <> "true") Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", backup, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton(_PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub DisplaySubscriptionInfo()
        Dim i As Integer = 0
        Dim findindex As Integer
        Dim arrSubscribed As Array = Nothing
        Dim strNotifyA As String = ""
        Dim strNotifyI As String = ""
        Dim strNotifyN As String = ""
        Dim intInheritFrom As Long
        Dim strEnabled As String = " "
        Dim emailfrom_list As EmailFromData()
        Dim unsubscribe_list As EmailMessageData()
        Dim optout_list As EmailMessageData()
        Dim defaultmessage_list As EmailMessageData()
        Dim y As Integer = 0
        Dim settings_list As SettingsData
        Dim m_refSiteAPI As New SiteAPI

        emailfrom_list = _ContentApi.GetAllEmailFrom()
        optout_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.OptOut)
        defaultmessage_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.DefaultMessage)
        unsubscribe_list = _ContentApi.GetSubscriptionMessagesForType(Ektron.Cms.Common.EkEnumeration.EmailMessageTypes.Unsubscribe)
        _SubscriptionData = _ContentApi.GetAllActiveSubscriptions() 'then get folder
        intInheritFrom = _ContentApi.GetFolderInheritedFrom(_Id)
        settings_list = m_refSiteAPI.GetSiteVariables()

        _IsGlobalSubInheritance = True

        _SubscribedData = _ContentApi.GetSubscriptionsForFolder(intInheritFrom)
        _SubscriptionProperties = _ContentApi.GetSubscriptionPropertiesForFolder(intInheritFrom)

        lit_vf_subscription_properties.Text += (Environment.NewLine & "<script type=""text/javascript"">" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("function SetMessageContenttoDefault() {" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("     document.getElementById('use_content_button').checked = true;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("     document.getElementById('frm_content_id').value = -1; " & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("     document.getElementById('titlename').value = '[[use current]]'; " & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("}" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("function  PreviewWebAlert() {" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    var contentid, defmsgid, optid, summaryid, unsubid, conttype, usecontlink;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    if (document.getElementById('use_content_button').checked == true) {;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("      contentid = document.getElementById('frm_content_id').value;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    } else {" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("      contentid = 0;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    }" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    if (document.getElementById('use_message_button').checked == true) {;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("      defmsgid = document.getElementById('notify_messageid').value;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    } else {" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("      defmsgid = 0;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    }" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    optid = document.getElementById('notify_optoutid').value;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    summaryid = document.getElementById('use_summary_button').checked; " & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    unsubid = document.getElementById('notify_unsubscribeid').value;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    conttype = 0;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    if (document.getElementById('use_contentlink_button').checked == true) {;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("      usecontlink = 1;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    } else {" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("      usecontlink = 0;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    }" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    window.open('previewwebalert.aspx?content=-1&defmsg=' + defmsgid + '&optoutid=' + optid + '&summaryid=' + summaryid + '&usecontentid=' + contentid + '&unsubscribeid=' + unsubid + '&content_type=' + conttype + '&uselink=' + usecontlink,'','menubar=no,location=no,resizable=yes,scrollbars=yes,status=yes'); " & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("}" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("function breakWebAlertInheritance(obj){" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("   if(!obj.checked){" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("       if(confirm(""" & _MessageHelper.GetMessage("js: confirm break inheritance") & """)){" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("           enableSubCheckboxes();" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("       } else {" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("           obj.checked = !obj.checked;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("           return false;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("       }" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("   } else {" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("       enableSubCheckboxes();" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("   }" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("}" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("function enableSubCheckboxes() {" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    var idx, masterBtn, tableObj, enableFlag, qtyElements, displayUseContentBtns;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    tableObj = document.getElementById('therows');" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    tableObj = tableObj.getElementsByTagName('input');" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    enableFlag = false;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    masterBtn = document.getElementById('webalert_inherit_button');" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    if (validateObject(masterBtn)){" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        enableFlag = !masterBtn.checked;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    }" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    displayUseContentBtns = enableFlag ? 'inline' : 'none';" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    if (validateObject(tableObj)){" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        qtyElements = tableObj.length;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        for(idx = 0; idx < qtyElements; idx++ ) {" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    		    if (tableObj[idx].type == 'checkbox'){" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    			    tableObj[idx].disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    		    }" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        }" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        $ektron('.selectContent').css('display', displayUseContentBtns);" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        $ektron('.useCurrent').css('display', displayUseContentBtns);" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.forms.frmContent.notify_option[0].disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.forms.frmContent.notify_option[1].disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.forms.frmContent.notify_option[2].disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.getElementById('use_message_button').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.getElementById('use_summary_button').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.getElementById('use_content_button').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.getElementById('use_contentlink_button').disabled = !enableFlag;" & Environment.NewLine)
        'lit_vf_subscription_properties.Text += ("        document.getElementById('notify_url').disabled = !enableFlag;" & Environment.NewLine)
        'lit_vf_subscription_properties.Text += ("        document.getElementById('notify_weblocation').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.getElementById('notify_emailfrom').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.getElementById('notify_optoutid').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.getElementById('notify_messageid').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.getElementById('notify_unsubscribeid').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("        document.getElementById('notify_subject').disabled = !enableFlag;" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("    }" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("}" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("function validateObject(obj) {" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("     return ((obj != null) &&" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("         ((typeof(obj)).toLowerCase() != 'undefined') &&" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("         ((typeof(obj)).toLowerCase() != 'null'))" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("}" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("function valAndSaveCSubAssignments() {" & Environment.NewLine)
        If (Not (_SubscriptionData Is Nothing)) And (Not ((emailfrom_list Is Nothing) Or (defaultmessage_list Is Nothing) Or (unsubscribe_list Is Nothing) Or (optout_list Is Nothing) Or (settings_list.AsynchronousLocation = ""))) Then
            lit_vf_subscription_properties.Text += ("    var idx, masterBtn, tableObj, enableFlag, qtyElements, retStr;" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    var hidnFld = document.getElementById('content_sub_assignments');" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    hidnFld.value='';" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    tableObj = tableObj = document.getElementById('therows');" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    tableObj = tableObj.getElementsByTagName('input');" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    enableFlag = true;" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    retStr = '';" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    if ((validateObject(tableObj)) && enableFlag){" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("        qtyElements = tableObj.length;" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("        for(idx = 0; idx < qtyElements; idx++ ) {" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    		    if ((tableObj[idx].type == 'checkbox') && tableObj[idx].checked){" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    			    retStr = retStr + tableObj[idx].name + ' ';" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    		    }" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("        }" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    }" & Environment.NewLine)
            lit_vf_subscription_properties.Text += ("    hidnFld.value = retStr;" & Environment.NewLine)
        End If
        lit_vf_subscription_properties.Text += ("    return true; // (Note: return false to prevent form submission)" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("}" & Environment.NewLine)
        lit_vf_subscription_properties.Text += ("</script>" & Environment.NewLine)

        If (emailfrom_list Is Nothing) Or (defaultmessage_list Is Nothing) Or (unsubscribe_list Is Nothing) Or (optout_list Is Nothing) Or (_SubscriptionData Is Nothing) Or (settings_list.AsynchronousLocation = "") Then
            lit_vf_subscription_properties.Text &= ("<input type=""hidden"" name=""suppress_notification"" value=""true"">")
            lit_vf_subscription_properties.Text &= (_MessageHelper.GetMessage("lbl web alert not setup"))
            lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"

            If (emailfrom_list Is Nothing) Then
                lit_vf_subscription_properties.Text &= ("<font class=""ektronErrorText"">" & _MessageHelper.GetMessage("lbl web alert emailfrom not setup") & "</font><br/>")
            End If
            If (defaultmessage_list Is Nothing) Then
                lit_vf_subscription_properties.Text &= ("<font class=""ektronErrorText"">" & _MessageHelper.GetMessage("lbl web alert def msg not setup") & "</font><br/>")
            End If
            If (unsubscribe_list Is Nothing) Then
                lit_vf_subscription_properties.Text &= ("<font class=""ektronErrorText"">" & _MessageHelper.GetMessage("lbl web alert unsub not setup") & "</font><br/>")
            End If
            If (optout_list Is Nothing) Then
                lit_vf_subscription_properties.Text &= ("<font class=""ektronErrorText"">" & _MessageHelper.GetMessage("lbl web alert optout not setup") & "</font><br/>")
            End If
            If (_SubscriptionData Is Nothing) Then
                lit_vf_subscription_properties.Text &= ("<font class=""ektronErrorText"">" & _MessageHelper.GetMessage("alt No subscriptions are enabled on the folder.") & "</font><br/>")
            End If
            If (settings_list.AsynchronousLocation = "") Then
                lit_vf_subscription_properties.Text &= ("<font class=""ektronErrorText"">" & _MessageHelper.GetMessage("alt The location to the Asynchronous Data Processor is not specified.") & "</font>")
            End If
            Exit Sub
        End If

        If (_SubscriptionProperties Is Nothing) Then
            _SubscriptionProperties = New SubscriptionPropertiesData
        End If
        strEnabled = " disabled=""disabled"" "
        Select Case _SubscriptionProperties.NotificationType.GetHashCode
            Case 0
                strNotifyA = " checked=""checked"" "
                strNotifyI = ""
                strNotifyN = ""
            Case 1
                strNotifyA = ""
                strNotifyI = " checked=""checked"" "
                strNotifyN = ""
            Case 2
                strNotifyA = ""
                strNotifyI = ""
                strNotifyN = " checked=""checked"" "
        End Select

        lit_vf_subscription_properties.Text &= "<input id=""webalert_inherit_button"" checked=""checked"" onclick=""breakWebAlertInheritance(this);"" type=""checkbox"" name=""webalert_inherit_button"" value=""webalert_inherit_button"">" & _MessageHelper.GetMessage("lbl inherit parent configuration")
        lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"

        lit_vf_subscription_properties.Text &= "<table class=""ektronGrid"">"
        lit_vf_subscription_properties.Text &= "<tr>"
        lit_vf_subscription_properties.Text &= "<td class=""label"">"
        lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert opt") & ":"
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "<td class=""value"">"
        lit_vf_subscription_properties.Text &= "<input type=""radio"" value=""Always"" name=""notify_option"" " & strNotifyA & " " & strEnabled & "> " & _MessageHelper.GetMessage("lbl web alert notify always")
        lit_vf_subscription_properties.Text &= "<br />"
        lit_vf_subscription_properties.Text &= "<input type=""radio"" value=""Initial"" name=""notify_option""" & strNotifyI & " " & strEnabled & "> " & _MessageHelper.GetMessage("lbl web alert notify initial")
        lit_vf_subscription_properties.Text &= "<br />"
        lit_vf_subscription_properties.Text &= "<input type=""radio"" value=""Never"" name=""notify_option""" & strNotifyN & " " & strEnabled & "> " & _MessageHelper.GetMessage("lbl web alert notify never")
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "</tr>"

        lit_vf_subscription_properties.Text &= "<tr>"
        lit_vf_subscription_properties.Text &= "<td class=""label"">"
        lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert subject") & ":"
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "<td class=""value"">"
        If _SubscriptionProperties.Subject <> "" Then
            lit_vf_subscription_properties.Text &= "<input type=""text"" maxlength=""255"" size=""65"" value=""" & _SubscriptionProperties.Subject & """ name=""notify_subject"" id=""notify_subject"" " & strEnabled & ">"
        Else
            lit_vf_subscription_properties.Text &= "<input type=""text"" maxlength=""255"" size=""65"" value="""" name=""notify_subject"" id=""notify_subject"" " & strEnabled & ">"
        End If
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "</tr>"

        'lit_vf_subscription_properties.Text &= "Notification Base URL:"
        'If subscription_properties_list.URL <> "" Then
        '    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" id=""notify_url"" " & strEnabled & " value=""" & subscription_properties_list.URL & """>/"
        'Else
        '    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" id=""notify_url"" " & strEnabled & " value=""" & Request.ServerVariables("HTTP_HOST") & """>/"
        'End If

        lit_vf_subscription_properties.Text &= "<tr>"
        lit_vf_subscription_properties.Text &= "<td class=""label"">"
        lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert emailfrom address") & ":"
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "<td class=""value"">"

        lit_vf_subscription_properties.Text &= "<select " & strEnabled & " name=""notify_emailfrom"" id=""notify_emailfrom"">:"
        If (Not emailfrom_list Is Nothing) AndAlso emailfrom_list.Length > 0 Then
            For y = 0 To emailfrom_list.Length - 1
                lit_vf_subscription_properties.Text &= "<option>" & emailfrom_list(y).Email & "</option>"
            Next
        End If
        lit_vf_subscription_properties.Text &= "</select>"
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "</tr>"

        'lit_vf_subscription_properties.Text &= "Notification File Location:"
        'If subscription_properties_list.WebLocation <> "" Then
        'lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""" & subscription_properties_list.WebLocation & """ name=""notify_weblocation"" id=""notify_weblocation"" " & strEnabled & ">"
        'Else
        '    lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""subscriptions"" name=""notify_weblocation"" id=""notify_weblocation"" " & strEnabled & ">"
        'End If

        lit_vf_subscription_properties.Text &= "<tr>"
        lit_vf_subscription_properties.Text &= "<td class=""label"">"
        lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert contents") & ":"
        lit_vf_subscription_properties.Text &= "<img src=""" & _AppPath & "images/UI/Icons/preview.png"" alt=""Preview Web Alert Message"" title=""Preview Web Alert Message"" onclick="" PreviewWebAlert(); return false;"" />"
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "<td class=""value"">"
        lit_vf_subscription_properties.Text += "<input id=""use_optout_button"" type=""checkbox"" checked=""checked"" name=""use_optout_button"" disabled=""disabled"">" & _MessageHelper.GetMessage("lbl optout message") & "&nbsp;&nbsp;"

        lit_vf_subscription_properties.Text += "<select " & strEnabled & " name=""notify_optoutid"" id=""notify_optoutid"">"
        If (Not optout_list Is Nothing) AndAlso optout_list.Length > 0 Then
            For y = 0 To optout_list.Length - 1
                If optout_list(y).Id = _SubscriptionProperties.OptOutID Then
                    lit_vf_subscription_properties.Text += "<option value=""" & optout_list(y).Id & """ SELECTED>" & optout_list(y).Title & "</option>"
                Else
                    lit_vf_subscription_properties.Text += "<option value=""" & optout_list(y).Id & """>" & optout_list(y).Title & "</option>"
                End If
            Next
        End If
        lit_vf_subscription_properties.Text += "</select>"

        lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
        If _SubscriptionProperties.DefaultMessageID > 0 Then
            lit_vf_subscription_properties.Text += "<input id=""use_message_button"" type=""checkbox"" checked=""checked"" name=""use_message_button"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use default message") & "&nbsp;&nbsp;"
        Else
            lit_vf_subscription_properties.Text += "<input id=""use_message_button"" type=""checkbox"" name=""use_message_button"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use default message") & "&nbsp;&nbsp;"
        End If

        lit_vf_subscription_properties.Text += "<select " & strEnabled & " name=""notify_messageid"" id=""notify_messageid"">"
        If (Not defaultmessage_list Is Nothing) AndAlso defaultmessage_list.Length > 0 Then
            For y = 0 To defaultmessage_list.Length - 1
                If defaultmessage_list(y).Id = _SubscriptionProperties.DefaultMessageID Then
                    lit_vf_subscription_properties.Text += "<option value=""" & defaultmessage_list(y).Id & """ SELECTED>" & defaultmessage_list(y).Title & "</option>"
                Else
                    lit_vf_subscription_properties.Text += "<option value=""" & defaultmessage_list(y).Id & """>" & defaultmessage_list(y).Title & "</option>"
                End If
            Next
        End If
        lit_vf_subscription_properties.Text += "</select>"

        lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
        If _SubscriptionProperties.SummaryID > 0 Then
            lit_vf_subscription_properties.Text += "<input id=""use_summary_button"" type=""checkbox"" name=""use_summary_button"" checked=""checked"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use summary message")
        Else
            lit_vf_subscription_properties.Text += "<input id=""use_summary_button"" type=""checkbox"" name=""use_summary_button"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use summary message")
        End If
        lit_vf_subscription_properties.Text += "<br />"
        If _SubscriptionProperties.ContentID = -1 Then
            lit_vf_subscription_properties.Text += "<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" checked=""checked"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use content message") & "&nbsp;&nbsp;"
            lit_vf_subscription_properties.Text += "<input type=""text"" id=""titlename"" name=""titlename"" value=""[[use current]]"" " & strEnabled & " size=""65"" disabled=""disabled""/>"
            lit_vf_subscription_properties.Text += "<a href=""#"" class=""button buttonInline greenHover selectContent"" onclick="" QuickLinkSelectBase(" & _Id.ToString() & ",'frmContent','titlename',0,0,0,0) ;return false;"">" & _MessageHelper.GetMessage("lbl use content select") & "</a><a href=""#"" class=""button buttonInline  blueHover useCurrent"" onclick="" SetMessageContenttoDefault();return false;"">Use Current</a>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" maxlength=""20"" id=""frm_content_id"" name=""frm_content_id"" value=""-1""/>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_content_langid""/>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_qlink""/>"
        ElseIf _SubscriptionProperties.ContentID > 0 Then
            lit_vf_subscription_properties.Text += "<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" checked=""checked"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use content message") & "&nbsp;&nbsp;"
            lit_vf_subscription_properties.Text += "<input type=""text"" id=""titlename"" name=""titlename"" value=""" & _SubscriptionProperties.UseContentTitle.ToString & """ size=""65"" disabled=""disabled""/>"
            lit_vf_subscription_properties.Text += "<a href=""#"" class=""button buttonInline greenHover selectContent"" onclick="" QuickLinkSelectBase(" & _Id.ToString() & ",'frmContent','titlename',0,0,0,0) ;return false;"">" & _MessageHelper.GetMessage("lbl use content select") & "</a><a href=""#"" class=""button buttonInline  blueHover useCurrent"" onclick="" SetMessageContenttoDefault();return false;"">Use Current</a>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" maxlength=""20"" id=""frm_content_id"" name=""frm_content_id"" value=""" & _SubscriptionProperties.ContentID.ToString() & """/>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_content_langid""/>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_qlink""/>"
        Else
            lit_vf_subscription_properties.Text += "<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use content message") & "&nbsp;&nbsp;"
            lit_vf_subscription_properties.Text += "<input type=""text"" id=""titlename"" name=""titlename"" onkeydown=""return false"" value="""" size=""65"" disabled=""disabled""/>"
            lit_vf_subscription_properties.Text += "<a href=""#"" class=""button buttonInline greenHover selectContent"" onclick="" QuickLinkSelectBase(" & _Id.ToString() & ",'frmContent','titlename',0,0,0,0) ;return false;"">" & _MessageHelper.GetMessage("lbl use content select") & "</a><a href=""#"" class=""button buttonInline  blueHover useCurrent"" onclick="" SetMessageContenttoDefault();return false;"">Use Current</a>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" maxlength=""20"" id=""frm_content_id"" name=""frm_content_id"" value=""0"" />"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_content_langid""/>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_qlink""/>"
        End If

        lit_vf_subscription_properties.Text += "<br />"

        If _SubscriptionProperties.UseContentLink > 0 Then
            lit_vf_subscription_properties.Text += "<input id=""use_contentlink_button"" type=""checkbox"" name=""use_contentlink_button"" checked=""checked"" " & strEnabled & ">Use Content Link"
        Else
            lit_vf_subscription_properties.Text += "<input id=""use_contentlink_button"" type=""checkbox"" name=""use_contentlink_button"" " & strEnabled & ">Use Content Link"
        End If

        lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
        lit_vf_subscription_properties.Text += "<input id=""use_unsubscribe_button"" type=""checkbox"" checked=""checked"" name=""use_unsubscribe_button"" disabled=""disabled"">" & _MessageHelper.GetMessage("lbl unsubscribe message") & "&nbsp;&nbsp;"

        lit_vf_subscription_properties.Text += "<select " & strEnabled & " name=""notify_unsubscribeid"" id=""notify_unsubscribeid"">"
        If (Not unsubscribe_list Is Nothing) AndAlso unsubscribe_list.Length > 0 Then
            For y = 0 To unsubscribe_list.Length - 1
                If unsubscribe_list(y).Id = _SubscriptionProperties.UnsubscribeID Then
                    lit_vf_subscription_properties.Text += "<option value=""" & unsubscribe_list(y).Id & """ SELECTED>" & unsubscribe_list(y).Title & "</option>"
                Else
                    lit_vf_subscription_properties.Text += "<option value=""" & unsubscribe_list(y).Id & """>" & unsubscribe_list(y).Title & "</option>"
                End If
            Next
        End If
        lit_vf_subscription_properties.Text += "</select>"

        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "</tr>"
        lit_vf_subscription_properties.Text &= "</table>"

        lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
        lit_vf_subscription_properties.Text &= "<div class=""ektronHeader"">"
        lit_vf_subscription_properties.Text += _MessageHelper.GetMessage("lbl avail web alert")
        lit_vf_subscription_properties.Text &= "</div>"

        If Not (_SubscriptionData Is Nothing) Then
            lit_vf_subscription_properties.Text += "<table id=""cfld_subscription_assignment"" class=""ektronGrid"" width=""100%"">"
            lit_vf_subscription_properties.Text += "<tbody id=""therows"">"
            lit_vf_subscription_properties.Text += "<tr class=""title-header"">"
            lit_vf_subscription_properties.Text &= "<th width=""50"">" & _MessageHelper.GetMessage("lbl assigned") & "</th>"
            lit_vf_subscription_properties.Text &= "<th>" & _MessageHelper.GetMessage("lbl name") & "</th>"
            lit_vf_subscription_properties.Text += "</tr>"
            If Not (_SubscribedData Is Nothing) Then
                arrSubscribed = Array.CreateInstance(GetType(Long), _SubscribedData.Length)
                For i = 0 To _SubscribedData.Length - 1
                    arrSubscribed.SetValue(_SubscribedData(i).Id, i)
                Next
                If (Not arrSubscribed Is Nothing) Then
                    If arrSubscribed.Length > 0 Then
                        Array.Sort(arrSubscribed)
                    End If
                End If
            End If
            i = 0

            For i = 0 To _SubscriptionData.Length - 1
                findindex = -1
                If ((Not _SubscribedData Is Nothing) AndAlso (Not arrSubscribed Is Nothing)) Then
                    findindex = Array.BinarySearch(arrSubscribed, _SubscriptionData(i).Id)
                End If
                lit_vf_subscription_properties.Text &= "<tr>"
                If findindex < 0 Then
                    lit_vf_subscription_properties.Text += "<td class=""center"" width=""10%""><input type=""checkbox"" name=""webalert_" & _SubscriptionData(i).Id & """  id=""Assigned_" & _SubscriptionData(i).Id & """ " & strEnabled & "></td>"
                Else
                    lit_vf_subscription_properties.Text += "<td class=""center"" width=""10%""><input type=""checkbox"" name=""webalert_" & _SubscriptionData(i).Id & """  id=""Assigned_" & _SubscriptionData(i).Id & """ checked=""checked"" " & strEnabled & "></td>"
                End If
                lit_vf_subscription_properties.Text += "<td>" & _SubscriptionData(i).Name & "</td>"
                lit_vf_subscription_properties.Text += "</tr>"
            Next
            lit_vf_subscription_properties.Text += "</tbody></table>"
        Else
            lit_vf_subscription_properties.Text += "Nothing available."
        End If
        lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""content_sub_assignments"" id=""content_sub_assignments"" value="""">"
    End Sub
#End Region

#Region "Catalog"

#Region "DoAddCatalog"

    Private Sub Process_DoAddCatalog()
        Dim tmpPath As String
        Dim libSettings As Collection
        Dim FolderPath As String
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim sub_prop_data As New SubscriptionPropertiesData
        Dim page_subscription_data As New Collection
        Dim page_sub_temp As New Collection
        Dim arrSubscriptions As Array
        Dim isub As Integer = 0

        m_refContent = _ContentApi.EkContentRef
        _PageData = New Collection
        _PageData.Add(True, "IsCatalog")
        _PageData.Add(Request.Form("foldername").Trim("."), "FolderName")
        _PageData.Add(Request.Form("folderdescription"), "FolderDescription")
        _PageData.Add(_Id, "ParentID") 'pagedata.Add(Request.Form("ParentID"), "ParentID")
        If (Request.Form("TemplateTypeBreak") Is Nothing) Then
            _PageData.Add(Request.Form("templatefilename"), "TemplateFileName")
        Else
            _PageData.Add("", "TemplateFileName")
        End If
        _PageData.Add(Request.Form("stylesheet"), "StyleSheet")
        If ((Request.Form("hdnInheritSitemap") IsNot Nothing) AndAlso (Request.Form("hdnInheritSitemap").ToString().ToLower() = "true")) Then
            _PageData.Add(True, "SitemapPathInherit")
        Else
            _PageData.Add(False, "SitemapPathInherit")
        End If
        _PageData.Add(Utilities.DeserializeSitemapPath(Request.Form, Me._ContentLanguage), "SitemapPath")
        Dim objLib As Ektron.Cms.Library.EkLibrary
        objLib = _ContentApi.EkLibraryRef
        libSettings = objLib.GetLibrarySettingsv2_0()
        tmpPath = libSettings("ImageDirectory")
        _PageData.Add(Server.MapPath(tmpPath), "AbsImageDirectory")
        tmpPath = libSettings("FileDirectory")
        _PageData.Add(Server.MapPath(tmpPath), "AbsFileDirectory")

        If Len(Request.Form("webalert_inherit_button")) Then
            sub_prop_data.BreakInheritance = False
        Else
            sub_prop_data.BreakInheritance = True
        End If

        Select Case Request.Form("notify_option")
            Case ("Always")
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Always
            Case ("Initial")
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Initial
            Case ("Never")
                sub_prop_data.NotificationType = Ektron.Cms.Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Never
        End Select

        sub_prop_data.SuspendNextNotification = False
        sub_prop_data.SendNextNotification = False

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
            sub_prop_data.FileLocation = Server.MapPath(_ContentApi.AppPath & "subscriptions")
        Else
            sub_prop_data.FileLocation = Server.MapPath(_ContentApi.AppPath & "subscriptions")
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

        Utilities.AddLBpaths(_PageData)

        If (LCase(Request.Form("TypeBreak")) = "on") Then
            _PageData.Add(True, "XmlInherited")
        Else
            _PageData.Add(False, "XmlInherited")
        End If
        _PageData.Add(Request.Form("xmlconfig"), "XmlConfiguration")
        If (Request.Form("PublishActive") <> "") Then
            If (Request.Form("PublishActive") = "PublishPdfActive") Then
                _PageData.Add(True, "PublishPdfActive")
                _PageData.Add(False, "PublishHtmlActive")
                'ElseIf (Request.Form("PublishActive") = "PublishHtmlActive") Then
                '   pagedata.Add(False, "PublishPdfActive")
                '  pagedata.Add(True, "PublishHtmlActive")
            End If
        Else
            _PageData.Add(False, "PublishPdfActive")
            _PageData.Add(False, "PublishHtmlActive")
        End If

        ' handle dynamic replication properties
        If (Request.Form("EnableReplication") <> "" OrElse Request.QueryString("type") = "communityfolder") Then
            _PageData.Add(Request.Form("EnableReplication"), "EnableReplication")
        Else
            _PageData.Add(0, "EnableReplication")
        End If

        If Not (Request.Form("suppress_notification") <> "") Then
            _PageData.Add(sub_prop_data, "SubscriptionProperties")
            _PageData.Add(page_subscription_data, "Subscriptions")
        End If

        If (Request.Form("break_inherit_button") IsNot Nothing AndAlso Request.Form("break_inherit_button").ToString().ToLower() = "on") Then
            _PageData.Add(0, "break_inherit_button") 'inherit button is checked => Metadata is inherited from parent.
        Else
            _PageData.Add(1, "break_inherit_button") 'break inheritance, do NOT inherit from parent
        End If
        _PageData.Add(Request.Form("folder_cfld_assignments"), "folder_cfld_assignments")

        ' add domain properties if they're there
        If ((Request.Form("IsDomainFolder") <> "") And (Request.Form("DomainProduction") <> "")) Then
            _PageData.Add(True, "IsDomainFolder")
            Dim staging As String = Request.Form("DomainStaging")
            Dim production As String = Request.Form("DomainProduction")
            If (staging Is Nothing) Then
                staging = ""
            End If
            If (staging.EndsWith("/")) Then staging = staging.Substring(0, staging.Length - 1)
            If (production.EndsWith("/")) Then production = production.Substring(0, production.Length - 1)
            If (staging = "") Then
                staging = production
            End If
            _PageData.Add(staging, "DomainStaging")
            _PageData.Add(production, "DomainProduction")
        Else
            _PageData.Add(False, "IsDomainFolder")
        End If
        If (Request.Form("break_inherit_button") IsNot Nothing AndAlso Request.Form("break_inherit_button").ToString().ToLower() = "on") Then
            _PageData.Add(1, "InheritMetadata") 'break inherit button is check.
        Else
            _PageData.Add(0, "InheritMetadata")
        End If
        _PageData.Add(Request.Form("inherit_meta_from"), "InheritMetadataFrom")

        If ((Not Request.QueryString("type") Is Nothing) AndAlso (Request.QueryString("type") = "communityfolder")) Then
            _PageData.Add(True, "IsCommunityFolder")
            _PageData.Remove("XmlInherited")
            _PageData.Add(False, "XmlInherited")
            _PageData.Remove("XmlConfiguration")
            _PageData.Add(Nothing, "XmlConfiguration")
        End If
        If (Request.Form("TaxonomyTypeBreak") IsNot Nothing AndAlso Request.Form("TaxonomyTypeBreak").ToString().ToLower() = "on") Then
            _PageData.Add(1, "InheritTaxonomy")
            If (Request.Form("CategoryRequired") IsNot Nothing AndAlso Request.Form("CategoryRequired").ToString().ToLower() = "on") Then
                _PageData.Add(1, "CategoryRequired")
            Else
                _PageData.Add(Request.Form(current_category_required.UniqueID), "CategoryRequired")
            End If
        Else
            _PageData.Add(0, "InheritTaxonomy")
            If (Request.Form("CategoryRequired") IsNot Nothing AndAlso Request.Form("CategoryRequired").ToString().ToLower() = "on") Then
                _PageData.Add(1, "CategoryRequired")
            Else
                _PageData.Add(0, "CategoryRequired")
            End If
        End If
        Dim IdRequests As String = ""
        If (Request.Form("taxlist") IsNot Nothing AndAlso Request.Form("taxlist") <> "") Then
            IdRequests = Request.Form("taxlist")
        End If
        _PageData.Add(IdRequests, "TaxonomyList")
        _PageData.Add(Request.Form(inherit_taxonomy_from.UniqueID), "InheritTaxonomyFrom")

        ' Update - add flagging items:
        ProcessFlaggingPostBack(_PageData)

        m_refContent.AddContentFolderv2_0(_PageData)

        '_CustomFieldsApi.ProcessCustomFields(_PageData("FolderID"))

        FolderPath = m_refContent.GetFolderPath(_PageData("ParentID"))
        If (Right(FolderPath, 1) = "\") Then
            FolderPath = Right(FolderPath, Len(FolderPath) - 1)
        End If
        FolderPath = Replace(FolderPath, "\", "\\")
        Dim close As String
        close = Request.QueryString("close")
        If (close = "true") Then
            Response.Redirect("close.aspx", False)
        ElseIf (Request.Form(frm_callingpage.UniqueID) = "cmsform.aspx") Then
            Response.Redirect("cmsform.aspx?LangType=" & _ContentLanguage & "&action=ViewAllFormsByFolderID&folder_id=" & _Id & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        Else
            Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewContentByCategory&id=" & _Id & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
        End If

        ' find the folder_id we just created now...
        _FolderId = _ContentApi.EkContentRef.GetFolderID(_ContentApi.EkContentRef.GetFolderPath(_PageData("ParentID")) & "\" & Request.Form("foldername").Trim("."))
    End Sub

#End Region

#Region "AddCatalog"

    Private Sub AddCatalogToolBar()
        Dim result As New System.Text.StringBuilder
        Dim backup As String = ""
        Dim close As String
        close = Request.QueryString("close")
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(String.Format(_MessageHelper.GetMessage("lbl add catalog to"), _FolderData.Name))
        backup = _StyleHelper.getCallBackupPage("content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage)
        result.Append("<table><tr>")
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt add catalog button text"), _MessageHelper.GetMessage("btn add catalog"), "onclick=""return SubmitForm('frmContent', 'CheckFolderParameters(\'add\')')"""))
        If (close <> "true") Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", backup, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton("addcatalog"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub Display_AddCatalog()
        Dim ShowTaxonomy As Boolean = False
        ShowTaxonomy = True
        Display_FolderJS()
        Dim backup As String = ""
        Dim strStyleSheet As String = ""
        Dim i As Integer = 0

        ltrTypes.Text = _MessageHelper.GetMessage("lbl product types")
        ltInheritSitemapPath.Text = _MessageHelper.GetMessage("lbl Inherit Parent Configuration")

        _FolderData = _ContentApi.GetFolderById(_Id, True, True)
        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder")
        Me.AddCatalogToolBar()

        backup = _StyleHelper.getCallBackupPage("content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage)

        If (_FolderData.StyleSheet = "") Then
            strStyleSheet = _ContentApi.GetStyleSheetByFolderID(_Id)
        End If

        tdfoldernamelabel.InnerHtml = _MessageHelper.GetMessage("catalogname label")

        tdsitepath.InnerHtml = _SitePath & "<input type=""text"" maxlength=""255"" size=""" & 75 - Len(_SitePath) & """" & " value="""" name=""stylesheet"">"
        tdsitepath.InnerHtml &= "<div class=""ektronCaption"">" & _MessageHelper.GetMessage("leave blank to inherit msg") & "</div>"
        ltrTemplateFilePath.Text = "<input type=""hidden"" maxlength=""255"" size=""" & 75 - Len(_SitePath) & """ value="""" name=""templatefilename"" id=""templatefilename"">"

        Dim request_info As Ektron.Cms.Common.EkRequestInformation = _ContentApi.RequestInformationRef

        If (ShowTaxonomy) Then
            DrawFolderTaxonomyTable()
        End If
        If request_info.EnableReplication Then
            Dim schk As String = ""
            If (_FolderData.ReplicationMethod = 1) Then
                schk = " checked"
            End If

            If (Not (Request.QueryString("type") <> "" AndAlso Request.QueryString("type").ToLower() = "communityfolder")) Then
                Dim fWarnMeta As Boolean = True
                If (_FolderData.ReplicationMethod = 1) Then
                    fWarnMeta = False
                End If

                ReplicationMethod.Text = "<tr><td colspan=""2"">&nbsp;</td></tr><tr><td colspan=""2"" class=""label"">" + _MessageHelper.GetMessage("lbl folderdynreplication") + "</td></tr>"
                ReplicationMethod.Text += "<tr><td colspan=""2""><input type=""checkbox"" name=""EnableReplication"" id=""EnableReplication"" value=""1""" & schk
                If (fWarnMeta) Then
                    ReplicationMethod.Text += " onclick=""if (!document.getElementById('break_inherit_button').checked && document.getElementById('EnableReplication').checked) {alert('"
                    ReplicationMethod.Text += _MessageHelper.GetMessage("js: alert qd metainherit")
                    ReplicationMethod.Text += "');}"""
                End If
                ReplicationMethod.Text += " >&nbsp;<label for=""EnableReplication"">" & _MessageHelper.GetMessage("replicate folder contents") & "</label></td></tr>"
            End If
        End If

        ParentID.Value = _Id
        frm_callingpage.Value = _StyleHelper.getCallingpage("content.aspx")
        lit_vf_customfieldassingments.Text = _CustomFieldsApi.AddNewEditableCustomFieldAssignments(_Id, Common.EkEnumeration.FolderType.Catalog)
        DisplaySubscriptionInfo()
        DisplaySitemapPath()
    End Sub

    Private Sub Display_CatalogJS()
        Dim sbJS As New StringBuilder

        sbJS.Append("document.forms[0].foldername.onkeypress = document.forms[0].netscape.onkeypress;" & Environment.NewLine)
        sbJS.Append("document.forms[0].stylesheet.onkeypress = document.forms[0].netscape.onkeypress;" & Environment.NewLine)
        sbJS.Append("document.forms[0].templatefilename.onkeypress = document.forms[0].netscape.onkeypress;" & Environment.NewLine)
        sbJS.Append("Ektron.ready(function() { document.forms[0].foldername.focus();" & Environment.NewLine)
        sbJS.Append("   if( $ektron('#webalert_inherit_button').length > 0 ){ " & Environment.NewLine)
        sbJS.Append("       if( $ektron('#webalert_inherit_button')[0].checked ){ " & Environment.NewLine)
        sbJS.Append("           $ektron('.selectContent').css('display', 'none');" & Environment.NewLine)
        sbJS.Append("           $ektron('.useCurrent').css('display', 'none');" & Environment.NewLine)
        sbJS.Append("       } " & Environment.NewLine)
        sbJS.Append("    } " & Environment.NewLine)
        sbJS.Append("});" & Environment.NewLine)
        sbJS.Append(" function PreviewSelectedProductType(sitepath,width,height) ").Append(Environment.NewLine)
        sbJS.Append(" { ").Append(Environment.NewLine)
        sbJS.Append(" 	var templar = document.getElementById(""addContentType"") ").Append(Environment.NewLine)
        sbJS.Append(" 	if (templar.value != -1) { ").Append(Environment.NewLine)
        sbJS.Append(" 		PopUpWindow('commerce/producttypes.aspx?LangType='+jsContentLanguage+'&action=viewproducttype&id=' + templar.options[templar.selectedIndex].value + '&caller=content', 'Preview', 700, 540, 1, 0); ").Append(Environment.NewLine)
        sbJS.Append(" 	} else { ").Append(Environment.NewLine)
        sbJS.Append(" 		alert('").Append(_MessageHelper.GetMessage("js select valid prod type")).Append("'); ").Append(Environment.NewLine)
        sbJS.Append(" 	} ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)
        sbJS.Append(" function PreviewProductTypeByID(xml_id) { ").Append(Environment.NewLine)
        sbJS.Append("   if (xml_id != 0) { ").Append(Environment.NewLine)
        sbJS.Append("       PopUpWindow('commerce/producttypes.aspx?LangType='+jsContentLanguage+'&action=viewproducttype&id=' + xml_id + '&caller=content', 'Preview', 700, 540, 1, 0); ").Append(Environment.NewLine)
        sbJS.Append("   } ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)
        ltr_af_js.Text = sbJS.ToString()
    End Sub

    Private Sub ProcessProductTemplatesPostBack(Optional ByVal type As String = "")
        _ProductType = New Commerce.ProductType(_ContentApi.RequestInformationRef)

        Dim IsInheritingTemplates As String = Request.Form("TemplateTypeBreak")
        Dim IsInheritingXml As String = Request.Form("TypeBreak")
        Dim prod_type_list As New List(Of Commerce.ProductTypeData)
        Dim criteria As New Criteria(Of ProductTypeProperty)

        prod_type_list = _ProductType.GetList(criteria)
        Dim default_template_id As Integer = 0
        Dim template_data As TemplateData()
        template_data = _ContentApi.GetAllTemplates("TemplateFileName")
        Dim i As Integer = 0
        Dim active_prod_list As New Collection
        Dim template_active_list As New Collection
        Dim default_xml_id As Long = -1
        If Request.Form(txt_adb_boardname.UniqueID) <> "" Then
            For i = 0 To template_data.Length - 1
                If (Request.Form("addTemplate") = template_data(i).Id) Then
                    template_active_list.Add(template_data(i).Id, template_data(i).Id)
                End If
            Next
        Else
            If (IsInheritingTemplates Is Nothing) Then
                For i = 0 To template_data.Length - 1
                    If (Not Request.Form("tinput_" & template_data(i).Id) Is Nothing) Then
                        template_active_list.Add(template_data(i).Id, template_data(i).Id)
                    End If
                Next
            End If
        End If
        If (IsInheritingXml Is Nothing) Then
            For i = 0 To prod_type_list.Count - 1
                If (Not Request.Form("input_" & prod_type_list(i).Id) Is Nothing) Then
                    active_prod_list.Add(prod_type_list(i).Id, prod_type_list(i).Id)
                End If
            Next

            If (Not Request.Form("sfdefault") Is Nothing) Then
                default_xml_id = Request.Form("sfdefault")
            End If

            If (Request.Form("requireSmartForms") Is Nothing) Then
                If (Not active_prod_list.Contains("0")) Then
                    active_prod_list.Add("0", "0")
                End If
            End If
        End If
        If (type = "forum") Then
            If (Request.Form("addTemplate") IsNot Nothing AndAlso Request.Form("addTemplate") <> "") Then
                default_template_id = Request.Form("addTemplate")
            End If
            _ContentApi.UpdateForumFolderMultiConfig(_FolderId, default_xml_id, default_template_id, template_active_list, active_prod_list)
        Else
            _ContentApi.UpdateFolderMultiConfig(_FolderId, default_xml_id, template_active_list, active_prod_list)
        End If
    End Sub

#End Region

#Region "product type selection"

    Private Sub DrawProductTypesTable()
        _ProductType = New Commerce.ProductType(_ContentApi.RequestInformationRef)

        Dim prod_type_list As New List(Of Commerce.ProductTypeData)
        Dim criteria As New Criteria(Of ProductTypeProperty)

        prod_type_list = _ProductType.GetList(criteria)

        Dim active_prod_list As List(Of Commerce.ProductTypeData)
        active_prod_list = _ProductType.GetFolderProductTypeList(_FolderData.Id)

        Dim addNew As New Collection()
        Dim k As Integer = 0
        Dim row_id As Integer = 0

        Dim smartFormsRequired As Boolean = True

        Dim broken As Boolean = False
        If (active_prod_list.Count > 0) Then
            broken = True
        End If

        Dim isParentCatalog As Boolean = Not (_ContentApi.EkContentRef.GetFolderType(_FolderData.Id) = Common.EkEnumeration.FolderType.Catalog)
        Dim isInheriting As Boolean = Not isParentCatalog
        Dim isEnabled As Boolean = Not isInheriting

        Dim str As New System.Text.StringBuilder()
        str.Append(DrawProductTypesBreaker(isInheriting, isParentCatalog))
        str.Append("<div class=""ektronTopSpace""></div>")

        str.Append(DrawProductTypesHeader())
        Dim ActiveXmlIdList As New Collection
        For k = 0 To active_prod_list.Count - 1
            If (Not ActiveXmlIdList.Contains(active_prod_list(k).Id)) Then
                ActiveXmlIdList.Add(active_prod_list(k).Id, active_prod_list(k).Id)
            End If
        Next
        If (Not _FolderData.XmlConfiguration Is Nothing) Then
            For j As Integer = 0 To (_FolderData.XmlConfiguration.Length - 1)
                If (Not ActiveXmlIdList.Contains(_FolderData.XmlConfiguration(j).Id)) Then
                    ActiveXmlIdList.Add(_FolderData.TemplateId, _FolderData.TemplateId)
                End If
            Next
        End If

        Dim entered As Boolean = False
        For k = 0 To prod_type_list.Count - 1
            If (ActiveXmlIdList.Contains(prod_type_list(k).Id)) Then
                entered = True
                str.Append(DrawProductTypesEntry(row_id, prod_type_list(k).Title, prod_type_list(k).Id, Utilities.IsDefaultXmlConfig(prod_type_list(k).Id, active_prod_list.ToArray()), isEnabled))
                row_id = row_id + 1
            Else
                Dim cRow As New Collection
                cRow.Add(prod_type_list(k).Title, "xml_name")
                cRow.Add(prod_type_list(k).Id, "xml_id")
                addNew.Add(cRow)
            End If
        Next

        If (Not smartFormsRequired) Then
            str.Append(DrawProductTypesEntry(row_id, _MessageHelper.GetMessage("lbl Blank HTML"), 0, Utilities.IsHTMLDefault(active_prod_list.ToArray()), isEnabled))
        End If

        str.Append("</tbody></table>")
        str.Append("</div")

        str.Append("<div class=""ektronTopSpace""></div>")
        str.Append("<table>")
        str.Append("<tbody>")
        str.Append("<tr>")
        str.Append("<td>")
        str.Append("<select name=""addContentType"" id=""addContentType"" " + IIf(isEnabled, "", " disabled ") + ">")
        str.Append("<option value=""-1"">" & "[" & _MessageHelper.GetMessage("lbl select prod type") & "]" & "</option>")
        Dim row As Collection
        For Each row In addNew
            str.Append("<option value=""" & row("xml_id") & """>" & row("xml_name") & "</option>")
        Next

        str.Append("</select>")
        str.Append("</td>")
        str.Append("<td>&nbsp;</td>")
        str.Append("<td>")
        str.Append("<span class='hiddenWhenInheriting' style='display:" + IIf(isEnabled, "inline;", "none;") + "' >")
        str.Append("<a href=""#"" onclick=""PreviewSelectedProductType('" & _ContentApi.SitePath & "', 800,600);return false;"">")
        str.Append("<img src=""" & _AppPath & "images/UI/Icons/preview.png" & """ alt=""" & _MessageHelper.GetMessage("lbl Preview prod type") & """ title=""" & _MessageHelper.GetMessage("lbl Preview prod type") & """>")
        str.Append("</a>")
        str.Append("</span>")
        str.Append("</td>")
        str.Append("<td>&nbsp;</td>")
        str.Append("<td>")
        str.Append("<span class='hiddenWhenInheriting' style='display:" + IIf(isEnabled, "inline;", "none;") + "' >")
        str.Append("<a href=""javascript:ActivateContentType(true);"">")
        str.Append("<img src=""" & _AppPath & "images/UI/Icons/add.png" & """ alt=""" & _MessageHelper.GetMessage("add title") & """ title=""" & _MessageHelper.GetMessage("add title") & """ border=""0"" />")
        str.Append("</a>")
        str.Append("</span>")
        str.Append("</td>")
        str.Append("</tr>")
        str.Append(DrawContentTypesFooter())
        If (row_id Mod 2 = 0) Then
            str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""1"" />")
        Else
            str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""0"" />")
        End If

        str.Append("<div style='display:none;'>")
        If (smartFormsRequired) Then
            str.Append("<input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onclick=""ToggleRequireSmartForms()"" checked>")
        Else
            str.Append("<input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onclick=""ToggleRequireSmartForms()"">")
        End If

        str.Append(_MessageHelper.GetMessage("lbl require prod types"))
        str.Append("</div>")
        ltr_vf_types.Text = str.ToString()
    End Sub
    Private Function DrawProductTypesBreaker(ByVal checked As Boolean, ByVal IsParentCatalog As Boolean) As String
        If IsParentCatalog Then
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleProductTypesInherit('TypeBreak', this)"" disabled autocomplete='off' />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        End If
        If (checked) Then
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleProductTypesInherit('TypeBreak', this)"" checked='checked' autocomplete='off' />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        Else
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleProductTypesInherit('TypeBreak', this)"" autocomplete='off' />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        End If

    End Function
    Private Function DrawProductTypesHeader() As String
        Dim str As New StringBuilder()
        str.Append("<div>")
        str.Append("<table class=""ektronGrid"" width=""100%""><tbody>")
        str.Append("<tr class=""title-header"">")
        str.Append("<td width=""10%"" class=""center"">")
        str.Append(_MessageHelper.GetMessage("lbl default") & "</td>")
        str.Append("<td width=""70%"" class=""center"">")
        str.Append(_MessageHelper.GetMessage("lbl prod type") & "</td>")
        str.Append("<td width=""10%"" class=""center"">")
        str.Append(_MessageHelper.GetMessage("lbl action") & "</td>")
        str.Append("<td width=""10%"" class=""center"">")
        str.Append("&nbsp;</td>")
        str.Append("</tr>")
        str.Append("</tbody></table>")
        str.Append("<table width=""100%"" class=""ektronGrid""><tbody id=""contentTypeTable"">")
        Return str.ToString()
    End Function

    Private Function DrawProductTypesEntry(ByVal row_id As Integer, ByVal name As String, ByVal xml_id As Long, ByVal isDefault As Boolean, ByVal isEnabled As Boolean) As String
        Dim str As New StringBuilder()
        Dim k As Integer = 0

        str.Append("<tr id=""row_" & xml_id & """>")
        str.Append("<td class=""center"" width=""10%"">")
        If (isDefault And isEnabled) Then
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ checked />")
        ElseIf (isDefault And Not isEnabled) Then
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ checked disabled />")
        ElseIf (Not isEnabled) Then
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ disabled />")
        Else
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ />")
        End If
        str.Append("</td>")
        str.Append("<td width=""70%"">")
        str.Append(name & "<input id=""input_" & xml_id & """ name=""input_" & xml_id & """ type=""hidden"" value=""" & xml_id & """ /></td>")
        If (xml_id <> 0) Then
            str.Append("<td class=""center"" width=""10%""><span class='hiddenWhenInheriting' style='display:" + IIf(isEnabled, "inline;", "none;") + "' ><a class=""button greenHover minHeight buttonSearch"" href=""javascript:PreviewProductTypeByID(" & xml_id & ")"">View</a></span></td>")
        Else
            str.Append("<td class=""center"" width=""10%"">&nbsp;</td>")
        End If

        str.Append("<td align=""right"" width=""10%""><span class='hiddenWhenInheriting' style='display:" + IIf(isEnabled, "inline;", "none;") + "' ><a class=""button greenHover minHeight buttonRemove"" href=""javascript:RemoveContentType(" & xml_id & ", '" & name & "')"">" & _MessageHelper.GetMessage("btn remove") & "</a></span></td>")
        str.Append("</tr>")

        Return str.ToString()
    End Function

#End Region

#End Region

#Region "AddBlog"

    Private Sub AddBlogToolBar()
        Dim result As New System.Text.StringBuilder
        Dim backup As String = ""
        Dim close As String
        close = Request.QueryString("close")
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar("""" & String.Format(_MessageHelper.GetMessage("lbl add a blog to folder x"), _FolderData.Name) & """")
        backup = _StyleHelper.getCallBackupPage("content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage)
        result.Append("<table><tr>")
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt add folder button text"), _MessageHelper.GetMessage("btn add blog"), "onclick=""return SubmitForm('frmContent', 'CheckBlogParameters()')"""))
        If (close <> "true") Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", backup, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton("AddBlogs"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub Display_AddBlog()
        phFolder.Visible = False
        pnlFolder.Visible = False
        phBlog.Visible = True
        pnlBlog.Visible = True

        ltrSelectDefTemp.text = _MessageHelper.GetMessage("js:alert select default template")

        hdnfolderid.Value = _Id.ToString()

        _FolderData = _ContentApi.GetFolderById(_Id, True)

        AddBlogToolBar()

        If _FolderData.PrivateContent = True Then
            drpVisibility.SelectedIndex = 1
        End If

        ltr_ab_cat.Text &= "<div id=""parah"">"
        ltr_ab_cat.Text &= "</div>"
        ltr_ab_cat.Text &= "<a href=""javascript:addInput()"" class=""button buttonInlineBlock greenHover buttonAdd"">" & Me._MessageHelper.GetMessage("lnk add new subject") & "</a><a href=""javascript:deleteInput()"" class=""button buttonInlineBlock redHover buttonRemove"">" & Me._MessageHelper.GetMessage("lnk remove last subject") & "</a>"
        ltr_ab_cat.Text &= "<input type=""hidden"" id=""categorylength"" name=""categorylength"" value=""0"" />"

        Dim ltrT As New Literal

        ltrT.Text = "<div id=""proll"" name=""proll"">"
        ltrT.Text &= "</div>"
        ltrT.Text &= "<input type=""hidden"" id=""rolllength"" name=""rolllength"" value=""0"" />"
        ltrT.Text &= "<a href=""javascript:addRoll()"" class=""button buttonInlineBlock greenHover buttonAdd"">Add Roll Link</a><a href=""javascript:deleteRoll()"" class=""button buttonInlineBlock redHover buttonRemove"">Remove Last Roll Link</a>"
        litBlogTemplatedata.Text = "<input type=""hidden"" maxlength=""255"" size=""" & 75 - _ContentApi.SitePath.Length & """ value="""" name=""templatefilename"" id=""templatefilename"">"
        ' handle dynamic replication settings
        Dim m_refCommonAPI As New CommonApi()
        Dim request_info As Ektron.Cms.Common.EkRequestInformation = m_refCommonAPI.RequestInformationRef
        If request_info.EnableReplication Then
            If (_FolderData.FolderType = 6) Then
                ' community folder, so just hide a hidden field w/ checkbox enabled
                BlogEnableReplication.Text = "<input type=""hidden"" name=""EnableReplication"" value=""1"" />"
            Else
                tr_enableblogreplication.Visible = True
                BlogEnableReplication.Text = "<input type=""checkbox"" name=""EnableReplication"" id=""EnableReplication"" value=""1"" />"
                BlogEnableReplication.Text += "<label for=""EnableReplication"">" & _MessageHelper.GetMessage("replicate folder contents") & "</label>"
            End If
        End If
        DrawContentTemplatesTable()
        DrawFolderTaxonomyTable()

        lbl_ab_roll.Controls.Add(ltrT)
    End Sub

    Private Sub Display_BlogJS()
        Dim sbblogjs As New StringBuilder
        'document.forms[0].foldername.onkeypress = document.forms[0].netscape.onkeypress;
        '//document.forms[0].stylesheet.onkeypress = document.forms[0].netscape.onkeypress;
        '//document.forms[0].templatefilename.onkeypress = document.forms[0].netscape.onkeypress;
        sbblogjs.Append(AJAXcheck(GetResponseString("VerifyBlog"), "action=existingfolder&pid=" & _Id.ToString() & "&fname=' + input + '")).Append(Environment.NewLine)
        sbblogjs.Append(Environment.NewLine)

        sbblogjs.Append("Ektron.ready(function(){document.forms[0]." & Replace(txtBlogName.UniqueID, "$", "_") & ".focus();});" & Environment.NewLine)
        sbblogjs.Append(Environment.NewLine & Environment.NewLine)
        sbblogjs.Append("function UpdateBlogCheckBoxes() {" & Environment.NewLine)
        sbblogjs.Append("   if (document.forms[0]." & Replace(chkEnable.UniqueID, "$", "_") & ".checked == true) {" & Environment.NewLine)
        sbblogjs.Append("       document.forms[0]." & Replace(chkModerate.UniqueID, "$", "_") & ".disabled = false;" & Environment.NewLine)
        sbblogjs.Append("       document.forms[0]." & Replace(chkRequire.UniqueID, "$", "_") & ".disabled = false;" & Environment.NewLine)
        sbblogjs.Append("   } else {" & Environment.NewLine)
        sbblogjs.Append("       document.forms[0]." & Replace(chkModerate.UniqueID, "$", "_") & ".disabled = true;" & Environment.NewLine)
        sbblogjs.Append("       document.forms[0]." & Replace(chkRequire.UniqueID, "$", "_") & ".disabled = true;" & Environment.NewLine)
        sbblogjs.Append("   }" & Environment.NewLine)
        sbblogjs.Append("}" & Environment.NewLine)
        sbblogjs.Append(Environment.NewLine & Environment.NewLine)
        sbblogjs.Append("function CheckBlogParameters() {" & Environment.NewLine)
        sbblogjs.Append("    var stext = Trim(document.getElementById('" & Replace(txtBlogName.UniqueID, "$", "_") & "').value);" & Environment.NewLine())
        sbblogjs.Append("    checkName(stext,''); " & Environment.NewLine())
        sbblogjs.Append("    TemplateConfigSave();").Append(Environment.NewLine())
        sbblogjs.Append("}" & Environment.NewLine)
        sbblogjs.Append("function VerifyBlog() {" & Environment.NewLine)

        sbblogjs.Append("   document.forms.frmContent." & Replace(txtBlogName.UniqueID, "$", "_") & ".value = Trim(document.forms.frmContent." & Replace(txtBlogName.UniqueID, "$", "_") & ".value);" & Environment.NewLine)
        sbblogjs.Append("   if ((document.forms.frmContent." & Replace(txtBlogName.UniqueID, "$", "_") & ".value == """"))" & Environment.NewLine)
        sbblogjs.Append("   {" & Environment.NewLine)
        sbblogjs.Append("   	alert(""You must supply a name for your blog."");" & Environment.NewLine)
        sbblogjs.Append("   	ShowPane('dvProperties');" & Environment.NewLine)
        sbblogjs.Append("   	document.forms.frmContent." & Replace(txtBlogName.UniqueID, "$", "_") & ".focus();" & Environment.NewLine)
        sbblogjs.Append("   	return false;" & Environment.NewLine)
        sbblogjs.Append("   }else if ((document.forms.frmContent." & Replace(txtTitle.UniqueID, "$", "_") & ".value == """"))" & Environment.NewLine)
        sbblogjs.Append("   {" & Environment.NewLine)
        sbblogjs.Append("   	ShowPane('dvProperties');" & Environment.NewLine)
        sbblogjs.Append("   	alert(""You must supply a title for your blog."");" & Environment.NewLine)
        sbblogjs.Append("   	document.forms.frmContent." & Replace(txtTitle.UniqueID, "$", "_") & ".focus();" & Environment.NewLine)
        sbblogjs.Append("   	return false;" & Environment.NewLine)
        sbblogjs.Append("   }else {" & Environment.NewLine)
        sbblogjs.Append("   	if (!CheckBlogForillegalChar()) {" & Environment.NewLine)
        sbblogjs.Append("   		return false;" & Environment.NewLine)
        sbblogjs.Append("   	}" & Environment.NewLine)
        sbblogjs.Append("   }" & Environment.NewLine)
        sbblogjs.Append("   if(checkForDefaultTemplate() == false) { return false;}" & Environment.NewLine)
        sbblogjs.Append("   var regexp1 = /""/gi;" & Environment.NewLine)
        sbblogjs.Append("   document.forms.frmContent." & Replace(txtBlogName.UniqueID, "$", "_") & ".value = document.forms.frmContent." & Replace(txtBlogName.UniqueID, "$", "_") & ".value.replace(regexp1, ""'"");" & Environment.NewLine)
        sbblogjs.Append("	SubmitForm('frmContent','true'); return true;" & Environment.NewLine)
        sbblogjs.Append("}" & Environment.NewLine)
        sbblogjs.Append("function CheckBlogForillegalChar() {" & Environment.NewLine)
        sbblogjs.Append("   var val = document.forms.frmContent." & Replace(txtBlogName.UniqueID, "$", "_") & ".value;" & Environment.NewLine)
        sbblogjs.Append("   if ((val.indexOf("";"") > -1) || (val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbblogjs.Append("   {" & Environment.NewLine)
        sbblogjs.Append("       alert(""" & Me._MessageHelper.GetMessage("js alert blog name cant include") & " (';', '\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')."");" & Environment.NewLine)
        sbblogjs.Append("       return false;" & Environment.NewLine)
        sbblogjs.Append("   }" & Environment.NewLine)
        sbblogjs.Append("   for (var j = 0; j < arrInputValue.length; j++)" & Environment.NewLine)
        sbblogjs.Append("   {" & Environment.NewLine)
        sbblogjs.Append("       val = Trim(arrInputValue[j]);" & Environment.NewLine)
        sbblogjs.Append("       if ((val.indexOf("";"") > -1) || (val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbblogjs.Append("       {" & Environment.NewLine)
        sbblogjs.Append("           alert(""" & Me._MessageHelper.GetMessage("alert subject name") & " (';', '\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')"");" & Environment.NewLine)
        sbblogjs.Append("           return false;" & Environment.NewLine)
        sbblogjs.Append("       }" & Environment.NewLine)
        sbblogjs.Append("       else if (val.length == 0) {" & Environment.NewLine)
        sbblogjs.Append("           alert(""" & Me._MessageHelper.GetMessage("alert blank subject name") & """);" & Environment.NewLine)
        sbblogjs.Append("           return false;" & Environment.NewLine)
        sbblogjs.Append("       }" & Environment.NewLine)
        sbblogjs.Append("   }" & Environment.NewLine)
        sbblogjs.Append("   return true;" & Environment.NewLine)
        sbblogjs.Append("}" & Environment.NewLine)
        ltr_af_js.Text = sbblogjs.ToString()
    End Sub
#End Region

#Region "AddDiscussionBoard"

    Private Sub Display_AddDiscussionBoard()
        phFolder.Visible = False
        pnlFolder.Visible = False
        phDiscussionBoard.Visible = True
        pnlDiscussionBoard.Visible = True

        hdn_adb_folderid.Value = _Id.ToString()

        _FolderData = _ContentApi.GetFolderById(_Id)

        AddDiscussionBoardToolBar()

        chk_adb_mc.Visible = False
        ' handle dynamic replication settings
        If _ContentApi.RequestInformationRef.EnableReplication Then
            If (_FolderData.IsCommunityFolder) Then
                ' parent folder is a community folder, so always enable replication for this board
                ltr_dyn_repl.Text = "<input type=""hidden"" name=""EnableReplication"" value=""1"" />"
            Else
                Dim schk As String = ""
                If (_FolderData.ReplicationMethod = 1) Then
                    schk = " checked"
                End If
                ltr_dyn_repl.Text = "<td class=""label"">" + _MessageHelper.GetMessage("lbl folderdynreplication") + "</td>"
                ltr_dyn_repl.Text += "<td class=""value"">"
                ltr_dyn_repl.Text += "<input type=""checkbox"" name=""EnableReplication"" id=""EnableReplication"" value=""1""" & schk & " ><label for=""EnableReplication"">" & _MessageHelper.GetMessage("replicate folder contents") & "</label>"
                ltr_dyn_repl.Text += "</td>"
            End If
        End If
        ltr_adb_cat.Text &= "<div class=""clearfix"">"

        ltr_adb_cat.Text &= "<fieldset>"
        ltr_adb_cat.Text &= "<legend>Subjects<span class=""required"">*</span></legend>"
        ltr_adb_cat.Text &= "<div id=""parah"">"
        ltr_adb_cat.Text &= "<p style=""color:silver;padding:.25em;margin:0;"">No Subjects Added</p>"
        ltr_adb_cat.Text &= "</div>"
        ltr_adb_cat.Text &= "</fieldset>"
        ltr_adb_cat.Text &= "<p class=""required"">* Required Field</p>"
        ltr_adb_cat.Text &= "<ul class=""buttonWrapperLeft"">"
        ltr_adb_cat.Text &= "<li>"
        ltr_adb_cat.Text &= "<a href=""#AddSubject"" title=""" & _MessageHelper.GetMessage("lnk Add New subject") & """ onclick=""addInput();return false;"" class=""button buttonLeft greenHover buttonAdd"">" & _MessageHelper.GetMessage("lnk Add New subject") & "</a>"
        ltr_adb_cat.Text &= "</li>"
        ltr_adb_cat.Text &= "</ul>"
        ltr_adb_cat.Text &= "</div>"
        ltr_adb_cat.Text &= "<input type=""hidden"" id=""categorylength"" name=""categorylength"" value=""0"" />"
        'css
        ltr_sitepath.Text = _ContentApi.SitePath
        Dim sJustAppPath As String = _ContentApi.AppPath.Replace(_ContentApi.SitePath, "")
        If sJustAppPath.Length > 0 AndAlso Not (sJustAppPath(sJustAppPath.Length - 1) = "/") Then
            sJustAppPath = sJustAppPath & "/"
        End If
        txt_adb_stylesheet.Text = sJustAppPath & "threadeddisc/themes/graysky/graysky.css"
        drp_theme.Attributes.Add("onchange", "updatetheme();")
        drp_theme.Items.Add(New ListItem("Select a theme", ""))
        drp_theme.Items.Add(New ListItem("Standard", sJustAppPath & "threadeddisc/themes/standard.css"))
        drp_theme.Items.Add(New ListItem("Chrome", sJustAppPath & "threadeddisc/themes/chrome.css"))
        drp_theme.Items.Add(New ListItem("Cool", sJustAppPath & "threadeddisc/themes/cool.css"))
        drp_theme.Items.Add(New ListItem("GraySky", sJustAppPath & "threadeddisc/themes/graysky/graysky.css"))
        drp_theme.Items.Add(New ListItem("Jungle", sJustAppPath & "threadeddisc/themes/jungle.css"))
        drp_theme.Items.Add(New ListItem("Modern", sJustAppPath & "threadeddisc/themes/modern.css"))
        drp_theme.Items.Add(New ListItem("Royal", sJustAppPath & "threadeddisc/themes/royal.css"))
        drp_theme.Items.Add(New ListItem("Slate", sJustAppPath & "threadeddisc/themes/slate.css"))
        drp_theme.Items.Add(New ListItem("Techno", sJustAppPath & "threadeddisc/themes/techno.css"))
        'css
        'page template
        template_list_cat.Text = "<table>"
        template_list_cat.Text &= "<tbody id=""templateTable"">"
        template_list_cat.Text &= "</tbody>"
        template_list_cat.Text &= "</table>"
        template_list_cat.Text &= "<table class=""ektronGrid"">"
        template_list_cat.Text &= "<tbody>"
        template_list_cat.Text &= "<tr>"
        template_list_cat.Text &= "<td class=""label"">Template:</td>"
        template_list_cat.Text &= "<td class=""value"">" & _ContentApi.SitePath & "<select name=""addTemplate"" id=""addTemplate""><option value=""0"">" & _MessageHelper.GetMessage("generic select template") & "</option>"
        Dim template_data As TemplateData()
        template_data = _ContentApi.GetAllTemplates("TemplateFileName")
        If Not (template_data Is Nothing) AndAlso template_data.Length > 0 Then
            For i As Integer = 0 To (template_data.Length - 1)
                template_list_cat.Text &= "<option value=""" & template_data(i).Id & """ >" & template_data(i).FileName & "</option>"
            Next
        End If
        template_list_cat.Text &= "</select><span class=""required"">*</span>"
        template_list_cat.Text &= "</td>"
        template_list_cat.Text &= "</tr>"
        template_list_cat.Text &= "</tbody>"
        template_list_cat.Text &= "</table>"
        template_list_cat.Text &= "<p class=""required"">* Required Field</p>"
        template_list_cat.Text &= "<input type=""hidden"" name=""tisEven"" id=""tisEven"" value=""1"" />"
        template_list_cat.Text &= "<div id=""div3"" style=""display: none;position: block;""></div>"
        template_list_cat.Text &= "<div id=""contentidspan"" style=""display: block;"">"
        template_list_cat.Text &= "<a href=""#PreviewSelectedTemplate"" class=""button buttonInlineBlock blueHover buttonPreview"" onclick=""PreviewTemplate('" & _ContentApi.SitePath & "', 800,600);return false;"">"
        template_list_cat.Text &= "Preview Selected Template</a>"
        template_list_cat.Text &= "<a href=""#AddTemplate"" class=""button buttonInlineBlock greenHover buttonAdd"" onclick=""LoadChildPage();return true;"">Add Template</a>"
        template_list_cat.Text &= "</div>"
        template_list_cat.Text &= "<div id=""FrameContainer"" class=""ektronWindow ektronModalStandard ektronModalWidth-50"" style=""margin-left:-25em !important;"">"
        template_list_cat.Text &= "<iframe id=""ChildPage"" name=""ChildPage"" style=""width:50em;"" frameborder=""no"">"
        template_list_cat.Text &= "</iframe>"
        template_list_cat.Text &= "</div>"

        lit_ef_templatedata.Text = "<input type=""hidden"" id=""language"" value=""" & Me._ContentLanguage & """ />"
        lit_ef_templatedata.Text &= "<input type=""hidden"" maxlength=""255"" size=""" & 75 - _ContentApi.SitePath.Length & """ value="""" name=""templatefilename"" id=""templatefilename"">"
        'page template
    End Sub

    Private Sub AddDiscussionBoardToolBar()
        Dim result As New System.Text.StringBuilder
        Dim backup As String = ""
        Dim close As String
        close = Request.QueryString("close")
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar("Add a Discussion Board to folder """ & _FolderData.Name & """")
        backup = _StyleHelper.getCallBackupPage("content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage)
        result.Append("<table><tr>")
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt add folder button text"), "Add Discussion Board", "onclick=""return SubmitForm('frmContent', 'CheckDiscussionBoardParameters()')"""))
        If (close <> "true") Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", backup, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton("AddDiscussionBoard"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub Display_DiscussionBoardJS()
        Dim sbdiscussionboardjs As New StringBuilder
        sbdiscussionboardjs.Append(AJAXcheck(GetResponseString("VerifyBoard"), "action=existingfolder&pid=" & _Id.ToString() & "&fname=' + input + '")).Append(Environment.NewLine)
        sbdiscussionboardjs.Append(Environment.NewLine)

        sbdiscussionboardjs.Append("Ektron.ready(function() {document.forms[0]." & Replace(txt_adb_boardname.UniqueID, "$", "_") & ".focus();});" & Environment.NewLine)
        sbdiscussionboardjs.Append(Environment.NewLine & Environment.NewLine)
        sbdiscussionboardjs.Append("function CheckDiscussionBoardParameters() {" & Environment.NewLine)

        sbdiscussionboardjs.Append("    var stext = Trim(document.getElementById('" & Replace(Me.txt_adb_boardname.UniqueID, "$", "_") & "').value);" & Environment.NewLine())
        sbdiscussionboardjs.Append("    checkName(stext,''); " & Environment.NewLine())
        sbdiscussionboardjs.Append("    // return bexists; " & Environment.NewLine())
        sbdiscussionboardjs.Append("}" & Environment.NewLine)
        sbdiscussionboardjs.Append("function VerifyBoard() {" & Environment.NewLine)

        sbdiscussionboardjs.Append("document.forms.frmContent." & Replace(txt_adb_boardname.UniqueID, "$", "_") & ".value = Trim(document.forms.frmContent." & Replace(txt_adb_boardname.UniqueID, "$", "_") & ".value);" & Environment.NewLine)
        sbdiscussionboardjs.Append("if ((document.forms.frmContent." & Replace(txt_adb_boardname.UniqueID, "$", "_") & ".value == """"))" & Environment.NewLine)
        sbdiscussionboardjs.Append("{" & Environment.NewLine)
        sbdiscussionboardjs.Append("	alert(""You must supply a name for your Discussion Board."");" & Environment.NewLine)
        sbdiscussionboardjs.Append("    ShowPane('dvProperties');").Append(Environment.NewLine)
        sbdiscussionboardjs.Append("	document.forms.frmContent." & Replace(txt_adb_boardname.UniqueID, "$", "_") & ".focus();" & Environment.NewLine)
        sbdiscussionboardjs.Append("	return false;" & Environment.NewLine)
        sbdiscussionboardjs.Append("}else if (!CheckCategory())" & Environment.NewLine)
        sbdiscussionboardjs.Append("{" & Environment.NewLine)
        sbdiscussionboardjs.Append("    ShowPane('dvCategories');").Append(Environment.NewLine)
        sbdiscussionboardjs.Append("	return false;" & Environment.NewLine)
        sbdiscussionboardjs.Append("}else {" & Environment.NewLine)
        sbdiscussionboardjs.Append("	if (!CheckDiscussionBoardForillegalChar()) {" & Environment.NewLine)
        sbdiscussionboardjs.Append("		return false;" & Environment.NewLine)
        sbdiscussionboardjs.Append("	}" & Environment.NewLine)
        sbdiscussionboardjs.Append("}" & Environment.NewLine)
        sbdiscussionboardjs.Append("if (document.getElementById('addTemplate').selectedIndex == 0) " & Environment.NewLine)
        sbdiscussionboardjs.Append("{" & Environment.NewLine)
        sbdiscussionboardjs.Append("    alert(""You must select a template for your Discussion Board."");" & Environment.NewLine)
        sbdiscussionboardjs.Append("    ShowPane('dvTemplates');").Append(Environment.NewLine)
        sbdiscussionboardjs.Append("	document.getElementById('addTemplate').focus();" & Environment.NewLine)
        sbdiscussionboardjs.Append("	return false;" & Environment.NewLine)
        sbdiscussionboardjs.Append("}" & Environment.NewLine)
        sbdiscussionboardjs.Append("var regexp1 = /""/gi;" & Environment.NewLine)
        sbdiscussionboardjs.Append("document.forms.frmContent." & Replace(txt_adb_boardname.UniqueID, "$", "_") & ".value = document.forms.frmContent." & Replace(txt_adb_boardname.UniqueID, "$", "_") & ".value.replace(regexp1, ""'"");" & Environment.NewLine)
        sbdiscussionboardjs.Append("	SubmitForm('frmContent','true'); return true;" & Environment.NewLine)
        sbdiscussionboardjs.Append("}" & Environment.NewLine)
        sbdiscussionboardjs.Append("function CheckDiscussionBoardForillegalChar() {" & Environment.NewLine)
        sbdiscussionboardjs.Append("   var val = document.forms.frmContent." & Replace(txt_adb_boardname.UniqueID, "$", "_") & ".value;" & Environment.NewLine)
        sbdiscussionboardjs.Append("   if ((val.indexOf("";"") > -1) || (val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbdiscussionboardjs.Append("   {" & Environment.NewLine)
        sbdiscussionboardjs.Append("       alert(""Discussion Board name can't include (';', '\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')."");" & Environment.NewLine)
        sbdiscussionboardjs.Append("       return false;" & Environment.NewLine)
        sbdiscussionboardjs.Append("   }" & Environment.NewLine)
        sbdiscussionboardjs.Append("   return true;" & Environment.NewLine)
        sbdiscussionboardjs.Append("}" & Environment.NewLine)
        sbdiscussionboardjs.Append("function CheckCategory() {" & Environment.NewLine)
        sbdiscussionboardjs.Append("   if (arrInput.length < 1)" & Environment.NewLine)
        sbdiscussionboardjs.Append("   {" & Environment.NewLine)
        sbdiscussionboardjs.Append("	   alert(""At least one subject is required."");" & Environment.NewLine)
        sbdiscussionboardjs.Append("       return false;" & Environment.NewLine)
        sbdiscussionboardjs.Append("   }" & Environment.NewLine)
        sbdiscussionboardjs.Append("   for (var j = 0; j < arrInput.length; j++)" & Environment.NewLine)
        sbdiscussionboardjs.Append("   {" & Environment.NewLine)
        sbdiscussionboardjs.Append("        val = Trim(arrInputValue[j]);" & Environment.NewLine)
        sbdiscussionboardjs.Append("        if ((val.indexOf("";"") > -1) || (val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbdiscussionboardjs.Append("        { " & Environment.NewLine)
        sbdiscussionboardjs.Append("            alert(""Subject name can't include (';', '\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')."");" & Environment.NewLine)
        sbdiscussionboardjs.Append("            return false;" & Environment.NewLine)
        sbdiscussionboardjs.Append("        } " & Environment.NewLine)
        sbdiscussionboardjs.Append("        else if (val.length == 0) " & Environment.NewLine)
        sbdiscussionboardjs.Append("        {" & Environment.NewLine)
        sbdiscussionboardjs.Append("	        alert(""Can't have a blank subject."");" & Environment.NewLine)
        sbdiscussionboardjs.Append("            return false;" & Environment.NewLine)
        sbdiscussionboardjs.Append("        }" & Environment.NewLine)
        sbdiscussionboardjs.Append("   }" & Environment.NewLine)
        sbdiscussionboardjs.Append("   return true;" & Environment.NewLine)
        sbdiscussionboardjs.Append("}" & Environment.NewLine)
        sbdiscussionboardjs.Append("function CloseChildPage(){" & Environment.NewLine)
        sbdiscussionboardjs.Append("    $ektron('#FrameContainer').modalHide();" & Environment.NewLine)
        sbdiscussionboardjs.Append("}" & Environment.NewLine)
        sbdiscussionboardjs.Append("function LoadChildPage() {" & Environment.NewLine)
        sbdiscussionboardjs.Append("    var frameObj = document.getElementById(""ChildPage"");" & Environment.NewLine)
        sbdiscussionboardjs.Append("    frameObj.src = ""template_config.aspx?view=add&folder_edit=1"";" & Environment.NewLine)
        sbdiscussionboardjs.Append("    $ektron('#FrameContainer').modalShow();" & Environment.NewLine)
        sbdiscussionboardjs.Append("}" & Environment.NewLine & Environment.NewLine)
        sbdiscussionboardjs.Append("function updatetheme()" & Environment.NewLine)
        sbdiscussionboardjs.Append("{" & Environment.NewLine)
        sbdiscussionboardjs.Append("    var mylist = document.getElementById(""" & drp_theme.UniqueID.Replace("$", "_") & """);" & Environment.NewLine)
        sbdiscussionboardjs.Append("    var tText = mylist.options[mylist.selectedIndex].value;" & Environment.NewLine)
        sbdiscussionboardjs.Append("    if (tText.length > 0) {" & Environment.NewLine)
        sbdiscussionboardjs.Append("        document.getElementById(""" & txt_adb_stylesheet.UniqueID.Replace("$", "_") & """).value = tText;" & Environment.NewLine)
        sbdiscussionboardjs.Append("    }" & Environment.NewLine)
        sbdiscussionboardjs.Append("}" & Environment.NewLine)


        ltr_af_js.Text = sbdiscussionboardjs.ToString()
    End Sub

#End Region

#Region "AddDiscussionForum"
    Private Sub Display_AddDiscussionForum()
        pnlOuterContainer.Visible = False
        pnlDiscussionForum.Visible = True

        _FolderData = _ContentApi.GetFolderById(_Id)

        AddDiscussionForumToolBar()

        Dim adcCategories As DiscussionCategory()
        Dim m_refContent As New Ektron.Cms.Content.EkContent

        hdn_adf_folderid.Value = _Id.ToString()

        ltr_adf_properties.Text &= "<input type=""hidden"" id=""EnableReplication"" name=""EnableReplication"" value=""" & _FolderData.ReplicationMethod & """ />"

        m_refContent = _ContentApi.EkContentRef
        adcCategories = m_refContent.GetCategoriesforBoard(_Id)
        If Not (adcCategories Is Nothing) AndAlso (adcCategories.Length > 0) Then
            For j As Integer = 0 To (adcCategories.Length - 1)
                drp_adf_category.Items.Add(New ListItem(adcCategories(j).Name, adcCategories(j).CategoryID))
            Next
        Else
            Throw New Exception(_MessageHelper.GetMessage("err NoBoardCategories"))
        End If

        ltr_adb_cat.Text &= "<p id=""parah"">"
        ltr_adb_cat.Text &= "</p>"
        ltr_adb_cat.Text &= "<ul class=""buttonList"">"
        ltr_adb_cat.Text &= "<li><a href=""#AddSubject"" title=""" & _MessageHelper.GetMessage("lnk Add New subject") & """ onclick=""addInput();return false;"" class=""button buttonInlineBlock greenHover"">" & _MessageHelper.GetMessage("lnk Add New subject") & "</a>"
        ltr_adb_cat.Text &= "</li>"
        ltr_adb_cat.Text &= "</ul>"
        ltr_adb_cat.Text &= "<p class=""required"">* Required Field</p>"""
        ltr_adb_cat.Text &= "<input type=""hidden"" id=""categorylength"" name=""categorylength"" value=""0"" />"
    End Sub

    Private Sub AddDiscussionForumToolBar()
        Dim result As New System.Text.StringBuilder
        Dim backup As String = ""
        Dim close As String
        close = Request.QueryString("close")
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("alt discussion forum to board") & """" & _FolderData.Name & """")
        backup = _StyleHelper.getCallBackupPage("content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage)
        result.Append("<table><tr>")
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt add dforum button text"), _MessageHelper.GetMessage("lbl add discussion forum"), "onclick=""return SubmitForm('frmContent', 'CheckDiscussionForumParameters()')"""))
        If (close <> "true") Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", backup, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton("AddDiscussionForum"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub Display_DiscussionForumJS()
        Dim sbdiscussionforumjs As New StringBuilder
        sbdiscussionforumjs.Append("Ektron.ready(function() {document.forms[0]." & Replace(txt_adf_forumname.UniqueID, "$", "_") & ".focus();});" & Environment.NewLine)
        sbdiscussionforumjs.Append(Environment.NewLine & Environment.NewLine)
        sbdiscussionforumjs.Append("function CheckDiscussionForumParameters() {" & Environment.NewLine)
        sbdiscussionforumjs.Append("document.forms.frmContent." & Replace(txt_adf_forumname.UniqueID, "$", "_") & ".value = Trim(document.forms.frmContent." & Replace(txt_adf_forumname.UniqueID, "$", "_") & ".value);" & Environment.NewLine)
        sbdiscussionforumjs.Append("document.forms.frmContent." & Replace(txt_adf_sortorder.UniqueID, "$", "_") & ".value = Trim(document.forms.frmContent." & Replace(txt_adf_sortorder.UniqueID, "$", "_") & ".value);" & Environment.NewLine)
        sbdiscussionforumjs.Append("var iSort = document.forms.frmContent." & Replace(txt_adf_sortorder.UniqueID, "$", "_") & ".value;" & Environment.NewLine)
        sbdiscussionforumjs.Append("if ((document.forms.frmContent." & Replace(txt_adf_forumname.UniqueID, "$", "_") & ".value == """"))" & Environment.NewLine)
        sbdiscussionforumjs.Append("{" & Environment.NewLine)
        sbdiscussionforumjs.Append("	alert(""" & _MessageHelper.GetMessage("alert msg name supply") & """);" & Environment.NewLine)
        sbdiscussionforumjs.Append("	document.forms.frmContent." & Replace(txt_adf_forumname.UniqueID, "$", "_") & ".focus();" & Environment.NewLine)
        sbdiscussionforumjs.Append("	return false;" & Environment.NewLine)
        'iSort
        sbdiscussionforumjs.Append("} else if (isNaN(iSort)||iSort<1)" & Environment.NewLine)
        sbdiscussionforumjs.Append("{" & Environment.NewLine)
        sbdiscussionforumjs.Append("	alert(""" & _MessageHelper.GetMessage("msg sort") & """);" & Environment.NewLine)
        sbdiscussionforumjs.Append("	document.forms.frmContent." & Replace(txt_adf_sortorder.UniqueID, "$", "_") & ".focus();" & Environment.NewLine)
        sbdiscussionforumjs.Append("	return false;" & Environment.NewLine)
        sbdiscussionforumjs.Append("}else {" & Environment.NewLine)
        sbdiscussionforumjs.Append("	if (!CheckDiscussionForumForillegalChar()) {" & Environment.NewLine)
        sbdiscussionforumjs.Append("		return false;" & Environment.NewLine)
        sbdiscussionforumjs.Append("	}" & Environment.NewLine)
        sbdiscussionforumjs.Append("}" & Environment.NewLine)
        sbdiscussionforumjs.Append("var regexp1 = /""/gi;" & Environment.NewLine)
        sbdiscussionforumjs.Append("document.forms.frmContent." & Replace(txt_adf_forumname.UniqueID, "$", "_") & ".value = document.forms.frmContent." & Replace(txt_adf_forumname.UniqueID, "$", "_") & ".value.replace(regexp1, ""'"");" & Environment.NewLine)
        sbdiscussionforumjs.Append("	return true;" & Environment.NewLine)
        sbdiscussionforumjs.Append("}" & Environment.NewLine)
        sbdiscussionforumjs.Append("function CheckDiscussionForumForillegalChar() {" & Environment.NewLine)
        sbdiscussionforumjs.Append("   var val = document.forms.frmContent." & Replace(txt_adf_forumname.UniqueID, "$", "_") & ".value;" & Environment.NewLine)
        sbdiscussionforumjs.Append("   if ((val.indexOf("";"") > -1) || (val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbdiscussionforumjs.Append("   {" & Environment.NewLine)
        sbdiscussionforumjs.Append("       alert(""" & "Forum name can't include" & " " & "(';', '\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')."");" & Environment.NewLine)
        sbdiscussionforumjs.Append("       return false;" & Environment.NewLine)
        sbdiscussionforumjs.Append("   }" & Environment.NewLine)
        sbdiscussionforumjs.Append("   return true;" & Environment.NewLine)
        sbdiscussionforumjs.Append("}" & Environment.NewLine)
        ltr_af_js.Text = sbdiscussionforumjs.ToString()
    End Sub
#End Region

#Region "AddCommunityFolder"

    Private Sub Display_AddCommunityFolder()
        If (_FolderData Is Nothing) Then
            _FolderData = _ContentApi.GetFolderById(_Id, True)
        End If
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar("Add Community Folder to Folder" & " """ & _FolderData.Name & """")
        ReplicationMethod.Text = "<input type=""hidden"" id=""EnableReplication"" name=""EnableReplication"" value=""1"" />"
        ltr_vf_types.Visible = True
        ltrTypes.Visible = True
    End Sub

#End Region

#Region "content type selection"

    Private Function DrawContentTypesBreaker(ByVal checked As Boolean) As String
        If (checked) Then
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TypeBreak')"" checked />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        Else
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TypeBreak')"" />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        End If

    End Function

    Private Function DrawContentTypesHeader() As String
        Dim str As New StringBuilder()
        str.Append("<div>")
        str.Append("    <table class=""ektronGrid""><tbody id=""contentTypeTable"" name=""contentTypeTable"">")
        str.Append("        <tr class=""title-header"">")
        str.Append("            <td width=""10%"" class=""center"">")
        str.Append(_MessageHelper.GetMessage("lbl default") & "</td>")
        str.Append("            <td width=""70%"" class=""left"">")
        str.Append(_MessageHelper.GetMessage("lbl Smart Form") & "</td>")
        str.Append("            <td width=""20%"" class=""center"" colspan=""2"">")
        str.Append(_MessageHelper.GetMessage("lbl action") & "</td>")
        str.Append("        </tr>")
        Return str.ToString()
    End Function

    Private Function DrawContentTypesEntry(ByVal row_id As Integer, ByVal name As String, ByVal xml_id As Long, ByVal isDefault As Boolean, ByVal isEnabled As Boolean) As String
        Dim str As New StringBuilder()
        Dim k As Integer = 0

        str.Append("<tr id=""row_" & xml_id & """>")
        str.Append("<td class=""center"" width=""10%"">")
        If (isDefault And isEnabled) Then
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ checked />")
        ElseIf (isDefault And Not isEnabled) Then
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ checked disabled />")
        ElseIf (Not isEnabled) Then
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ disabled />")
        Else
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ />")
        End If
        str.Append("</td>")
        str.Append("<td width=""70%"">")
        str.Append(name & "<input id=""input_" & xml_id & """ name=""input_" & xml_id & """ type=""hidden"" value=""" & xml_id & """ />")
        str.Append("</td>")
        If (xml_id <> 0) Then
            str.Append("<td class=""center"" width=""10%""><a class=""button greenHover minHeight buttonSearch"" href=""javascript:PreviewXmlConfigByID(" & xml_id & ")"">View</a></td>")
        Else
            str.Append("<td class=""center"" width=""10%"">&nbsp;</td>")
        End If

        str.Append("<td class=""center"" width=""10%"">")
        str.Append("<a class=""button greenHover minHeight buttonRemove"" href=""javascript:RemoveContentType(" & xml_id & ", '" & name & "')"">" & _MessageHelper.GetMessage("btn remove"))
        str.Append("</td>")
        str.Append("</tr>")

        Return str.ToString()
    End Function

    Private Function DrawContentTypesFooter() As String
        Return "</tbody></table>"
    End Function

    Private Sub DrawContentTypesTable()
        Dim xml_config_list As XmlConfigData()
        xml_config_list = _ContentApi.GetAllXmlConfigurations(_OrderBy)
        Dim active_xml_list As XmlConfigData()
        active_xml_list = _ContentApi.GetEnabledXmlConfigsByFolder(_FolderData.Id)
        Dim addNew As New Collection()
        Dim k As Integer = 0
        Dim row_id As Integer = 0

        Dim smartFormsRequired As Boolean = Not Utilities.IsNonFormattedContentAllowed(active_xml_list)

        Dim isEnabled As Boolean = Not IsInheritingMultiConfig()

        Dim broken As Boolean = False
        If (active_xml_list.Length > 0) Then
            broken = True
        End If

        Dim isInheriting As Boolean = IsInheritingMultiConfig()

        Dim str As New System.Text.StringBuilder()
        str.Append(DrawContentTypesBreaker(isInheriting))
        str.Append("<div class=""ektronTopSpace""></div>")

        str.Append(DrawContentTypesHeader())
        Dim ActiveXmlIdList As New Collection
        For k = 0 To active_xml_list.Length - 1
            If (Not ActiveXmlIdList.Contains(active_xml_list(k).Id)) Then
                ActiveXmlIdList.Add(active_xml_list(k).Id, active_xml_list(k).Id)
            End If
        Next
        If (Not _FolderData.XmlConfiguration Is Nothing) Then
            For j As Integer = 0 To (_FolderData.XmlConfiguration.Length - 1)
                If (Not ActiveXmlIdList.Contains(_FolderData.XmlConfiguration(j).Id)) Then
                    ActiveXmlIdList.Add(_FolderData.TemplateId, _FolderData.TemplateId)
                End If
            Next
        End If

        Dim entered As Boolean = False
        For k = 0 To xml_config_list.Length - 1
            If (ActiveXmlIdList.Contains(xml_config_list(k).Id)) Then
                entered = True
                str.Append(DrawContentTypesEntry(row_id, xml_config_list(k).Title, xml_config_list(k).Id, Utilities.IsDefaultXmlConfig(xml_config_list(k).Id, active_xml_list), isEnabled))
                row_id = row_id + 1
            Else
                Dim cRow As New Collection
                cRow.Add(xml_config_list(k).Title, "xml_name")
                cRow.Add(xml_config_list(k).Id, "xml_id")
                addNew.Add(cRow)
            End If
        Next

        If (Not smartFormsRequired) Then
            str.Append(DrawContentTypesEntry(row_id, _MessageHelper.GetMessage("lbl Blank HTML"), 0, Utilities.IsHTMLDefault(active_xml_list), isEnabled))
        End If

        str.Append("</tbody></table>")
        str.Append("</div>")
        str.Append("<div class=""ektronTopSpace""></div>")
        str.Append("<table width=""100%""><tbody>")
        str.Append("<tr><td width=""90%"">")
        str.Append("<select name=""addContentType"" id=""addContentType"" disabled>")
        str.Append("<option value=""-1"">" & _MessageHelper.GetMessage("select smart form") & "</option>")
        Dim row As Collection
        For Each row In addNew
            str.Append("<option value=""" & row("xml_id") & """>" & row("xml_name") & "</option>")
        Next

        str.Append("</select>")
        str.Append(" <a href=""#"" onclick=""PreviewSelectedXmlConfig('" & _ContentApi.SitePath & "', 800,600);return false;""><img src=""" & _AppPath & "images/UI/Icons/preview.png" & """ alt=""" & _MessageHelper.GetMessage("lbl Preview Smart Form") & """ title=""" & _MessageHelper.GetMessage("lbl Preview Smart Form") & """ border=""0"" /></a>")
        str.Append(" <a href="" javascript:ActivateContentType()""><img src=""" & _AppPath & "images/UI/Icons/add.png" & """ alt=""" & _MessageHelper.GetMessage("add title") & """ title=""" & _MessageHelper.GetMessage("add title") & """ border=""0"" /></a></td></tr>")
        str.Append(DrawContentTypesFooter())
        If (row_id Mod 2 = 0) Then
            str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""1"" />")
        Else
            str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""0"" />")
        End If


        If (smartFormsRequired) Then
            str.Append("<div><input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onclick=""ToggleRequireSmartForms()"" checked disabled>")
        Else
            str.Append("<div><input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onclick=""ToggleRequireSmartForms()"" disabled>")
        End If

        str.Append(_MessageHelper.GetMessage("lbl Require Smart Forms"))
        str.Append("</div>")
        ltr_vf_types.Text = str.ToString()
    End Sub

#End Region

#Region "multi-template selection"
    Private Function DrawContentTemplatesBreaker(ByVal checked As Boolean) As String
        If (checked) Then
            Return "<input name=""TemplateTypeBreak"" id=""TemplateTypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TemplateTypeBreak')"" checked />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        Else
            Return "<input name=""TemplateTypeBreak"" id=""TemplateTypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TemplateTypeBreak')"" />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        End If
    End Function

    Private Function DrawContentTemplatesHeader() As String
        Dim str As New StringBuilder()
        str.Append("<table class=""ektronGrid""><tbody id=""templateTable"">")
        str.Append("<tr class=""title-header"">")
        str.Append("<td width=""10%"" class=""center"">")
        str.Append(_MessageHelper.GetMessage("lbl default"))
        str.Append("</td>")
        str.Append("<td width=""70%"" class=""left"">")
        str.Append(_MessageHelper.GetMessage("lbl Page Template Name"))
        str.Append("</td>")
        str.Append("<td width=""20%"" class=""center"" colspan=""2"">")
        str.Append(_MessageHelper.GetMessage("lbl Action"))
        str.Append("</td>")
        str.Append("</tr>")
        Return str.ToString()
    End Function

    Private Function DrawContentTemplatesEntry(ByVal row_id As Integer, ByVal name As String, ByVal typestring As String, ByVal template_id As Long, ByVal isEnabled As Boolean, ByVal templatedata As TemplateData) As String
        Dim str As New StringBuilder()
        Dim k As Integer = 0
        Dim isDefault As Boolean = False

        If (template_id = _FolderData.TemplateId) Then
            isDefault = True
        End If

        str.Append("<tr id=""trow_" & template_id & """>")

        str.Append("<td width=""10%"" class=""center"">")
        If (isDefault = "1" And isEnabled) Then
            str.Append("<input type=""radio"" id=""tdefault"" name=""tdefault"" value=""" & name & """ checked />")
        ElseIf (isDefault = "1" And Not isEnabled) Then
            str.Append("<input type=""radio"" id=""tdefault"" name=""tdefault"" value=""" & name & """ checked disabled />")
        ElseIf (Not isEnabled) Then
            str.Append("<input type=""radio"" id=""tdefault"" name=""tdefault"" value=""" & name & """ disabled />")
        Else
            str.Append("<input type=""radio"" id=""tdefault"" name=""tdefault"" value=""" & name & """ />")
        End If

        str.Append("</td>")
        str.Append("<td width=""70%"">")
        str.Append(name & typestring & "<input id=""tinput_" & template_id & """ name=""tinput_" & template_id & """ type=""hidden"" value=""" & template_id & """ /></td>")
        Dim link As String = ""
        If (templatedata.SubType = EkEnumeration.TemplateSubType.MasterLayout) Then
            link = _ContentApi.EkContentRef.GetContentQlink(templatedata.MasterLayoutID, _ContentApi.GetFolderIdForContentId(templatedata.MasterLayoutID))
            str.Append("<td class=""center"" width=""10%""><a href=""#"" class=""button greenHover minHeight buttonSearch"" onclick=""PreviewSpecificTemplate('" & link & "', 800,600)"">" & _MessageHelper.GetMessage("lbl View") & "</a></td>")
        Else
            str.Append("<td class=""center"" width=""10%""><a class=""button greenHover minHeight buttonSearch""  href=""javascript:PreviewSpecificTemplate('" & _ContentApi.SitePath & name & "', 800,600)"">" & _MessageHelper.GetMessage("lbl View") & "</a></td>")
        End If
        str.Append("<td class=""center"" width=""10%""><a class=""button redHover minHeight buttonRemove"" href=""javascript:RemoveTemplate(" & template_id & ", '" & name & "', '" & link & "')"">" & _MessageHelper.GetMessage("btn remove") & "</td>")
        str.Append("</tr>")

        Return str.ToString()
    End Function

    Private Function DrawContentTemplatesFooter() As String
        Return "</tbody></table>"
    End Function

    Private Function IsInheritingMultiConfig() As Boolean
        Return True
    End Function

    Private Sub DrawContentTemplatesTable()
        Dim active_templates As TemplateData()
        active_templates = _ContentApi.GetEnabledTemplatesByFolder(_FolderData.Id)

        Dim template_data As TemplateData()
        template_data = _ContentApi.GetAllTemplates("TemplateFileName")

        Dim tmodel As New PageBuilder.TemplateModel()
        For i As Integer = 0 To template_data.Length - 1
            If (template_data(i).SubType = EkEnumeration.TemplateSubType.MasterLayout) Then
                template_data(i) = tmodel.FindByID(template_data(i).Id)
            End If
        Next

        Dim k As Integer = 0
        Dim row_id As Integer = 0
        Dim addNew As New Collection()

        Dim broken As Boolean = False
        If (active_templates.Length > 0) Then
            broken = True
        End If

        Dim foundDefault As Boolean = False
        For k = 0 To active_templates.Length - 1
            If (active_templates(k).Id = _FolderData.TemplateId) Then
                foundDefault = True
            End If
        Next

        Dim isInheriting As Boolean = IsInheritingMultiConfig()

        'If (Not foundDefault) Then
        '    isInheriting = False
        'End If

        Dim str As New StringBuilder()

        str.Append(DrawContentTemplatesBreaker(isInheriting))
        str.Append("<div class=""ektronTopSpace""></div>")

        str.Append("<div>")
        str.Append(DrawContentTemplatesHeader())

        If (_FolderData.Id = 0) Then
            isInheriting = False
        End If

        Dim ActiveTemplateIdList As New Collection
        For k = 0 To active_templates.Length - 1
            If (Not ActiveTemplateIdList.Contains(active_templates(k).Id)) Then
                ActiveTemplateIdList.Add(active_templates(k).Id, active_templates(k).Id)
            End If
        Next

        If (Not ActiveTemplateIdList.Contains(_FolderData.TemplateId)) Then
            ActiveTemplateIdList.Add(_FolderData.TemplateId, _FolderData.TemplateId)
        End If

        Dim entered As Boolean = False
        For k = 0 To template_data.Length - 1
            If (ActiveTemplateIdList.Contains(template_data(k).Id)) Then
                entered = True
                Dim typestring As String = ""
                If (template_data(k).SubType = EkEnumeration.TemplateSubType.Wireframes) Then
                    typestring = " (" & _MessageHelper.GetMessage("lbl pagebuilder wireframe template") & ")"
                ElseIf (template_data(k).SubType = EkEnumeration.TemplateSubType.MasterLayout) Then
                    typestring = " (" & _MessageHelper.GetMessage("lbl pagebuilder master layouts") & ")"
                End If
                str.Append(DrawContentTemplatesEntry(row_id, template_data(k).FileName, typestring, template_data(k).Id, Not isInheriting, template_data(k)))
                row_id = row_id + 1
            Else
                Dim cRow As New Collection
                Dim type As String
                If (template_data(k).SubType = EkEnumeration.TemplateSubType.Wireframes) Then
                    type = " (" & _MessageHelper.GetMessage("lbl pagebuilder wireframe template") & ")"
                ElseIf (template_data(k).SubType = EkEnumeration.TemplateSubType.MasterLayout) Then
                    type = " (" & _MessageHelper.GetMessage("lbl pagebuilder master layouts") & ")"
                Else
                    type = ""
                End If
                cRow.Add(type, "template_type")
                cRow.Add(template_data(k).FileName, "template_name")
                cRow.Add(template_data(k).Id, "template_id")
                Dim url As String = ""
                If (template_data(k).SubType = EkEnumeration.TemplateSubType.MasterLayout) Then
                    url = _ContentApi.EkContentRef.GetContentQlink(template_data(k).MasterLayoutID, _ContentApi.GetFolderIdForContentId(template_data(k).MasterLayoutID))
                End If
                cRow.Add(url, "url")
                addNew.Add(cRow)
            End If
        Next

        str.Append(DrawContentTemplatesFooter())
        str.Append("</div>")

        str.Append("<div class=""ektronTopSpace""></div>")

        str.Append("<table><tbody>")
        str.Append("<tr>")
        str.Append("<td>")
        str.Append("<select name=""addTemplate"" id=""addTemplate"" disabled>")
        str.Append("<option value=""0"">" & _MessageHelper.GetMessage("generic select template") & "</option>")
        Dim row As Collection
        For Each row In addNew
            str.Append("<option value=""" & row("template_id") & """")
            If (Not String.IsNullOrEmpty(row("url"))) Then
                str.Append(" url=""" & row("url") & """")
            End If
            str.Append(">" & row("template_name") & row("template_type") & "</option>")
        Next
        str.Append("</select>")
        str.Append("</td>")
        str.Append("<td>&nbsp;</td>")
        str.Append("<td>")
        str.Append("<a href=""#"" onclick=""PreviewTemplate('" & _ContentApi.SitePath & "', 800,600);return false;""><img src=""" & _AppPath & "images/UI/Icons/preview.png" & """ alt=""Preview Template"" title=""Preview Template"" /></a>")
        str.Append("</td>")
        str.Append("<td>&nbsp;</td>")
        str.Append("<td>")
        str.Append("<a href=""javascript:ActivateTemplate('" & Me._ContentApi.SitePath & "')""><img src=""" & _AppPath & "images/UI/icons/add.png"" alt=""" & _MessageHelper.GetMessage("add title") & """ title=""" & _MessageHelper.GetMessage("add title") & """ /></a>")
        str.Append("</td>")
        str.Append("</tr>")
        str.Append("</tbody></table>")

        If (row_id Mod 2 = 0) Then
            str.Append("<input type=""hidden"" name=""tisEven"" id=""tisEven"" value=""1"" />")
        Else
            str.Append("<input type=""hidden"" name=""tisEven"" id=""tisEven"" value=""0"" />")
        End If

        str.Append("<div class=""ektronTopSpace""></div>")
        str.Append("<a href=""javascript:OpenAddDialog()"" class=""button buttonInlineBlock greenHover buttonAdd"">" & _MessageHelper.GetMessage("lbl add new template") & "</a>")
        'str.Append("<a href=""javascript:LoadChildPage()"" class=""button buttonInlineBlock greenHover buttonAdd"">" & _MessageHelper.GetMessage("lbl add new template") & "</a>")

        litBlogTemplate.Text = str.ToString()
        template_list.Text = str.ToString()
    End Sub
#End Region

#Region "multi-xml/multi-template postback"
    Private Sub ProcessContentTemplatesPostBack(Optional ByVal type As String = "")

        Dim IsInheritingTemplates As String = Request.Form("TemplateTypeBreak")
        Dim IsInheritingXml As String = Request.Form("TypeBreak")
        Dim xml_config_list As XmlConfigData()
        xml_config_list = _ContentApi.GetAllXmlConfigurations(_OrderBy)
        Dim default_template_id As Long = 0
        Dim template_data As TemplateData()
        template_data = _ContentApi.GetAllTemplates("TemplateFileName")

        Dim i As Integer = 0
        Dim xml_active_list As New Collection
        Dim template_active_list As New Collection
        Dim default_xml_id As Long = -1

        If Request.Form(txt_adb_boardname.UniqueID) <> "" Then
            For i = 0 To template_data.Length - 1
                If (Request.Form("addTemplate") = template_data(i).Id) Then
                    template_active_list.Add(template_data(i).Id, template_data(i).Id)
                End If
            Next
        Else
            If (IsInheritingTemplates Is Nothing) Then
                For i = 0 To template_data.Length - 1
                    If (Not Request.Form("tinput_" & template_data(i).Id) Is Nothing) Then
                        template_active_list.Add(template_data(i).Id, template_data(i).Id)
                    End If
                Next
            End If
        End If

        If (IsInheritingXml Is Nothing AndAlso Request.Form(txtBlogName.UniqueID) = "") Then
            For i = 0 To xml_config_list.Length - 1
                If (Not Request.Form("input_" & xml_config_list(i).Id) Is Nothing) Then
                    xml_active_list.Add(xml_config_list(i).Id, xml_config_list(i).Id)
                End If
            Next

            If (Not Request.Form("sfdefault") Is Nothing) Then
                default_xml_id = Request.Form("sfdefault")
            End If

            If (Request.Form("requireSmartForms") Is Nothing) Then
                If (Not xml_active_list.Contains("0")) Then
                    xml_active_list.Add("0", "0")
                End If
            End If
        End If
        If (type = "forum") Then
            If (Request.Form("addTemplate") IsNot Nothing AndAlso Request.Form("addTemplate") <> "") Then
                default_template_id = Request.Form("addTemplate")
            End If
            _ContentApi.UpdateForumFolderMultiConfig(_FolderId, default_xml_id, default_template_id, template_active_list, xml_active_list)
        Else
            _ContentApi.UpdateFolderMultiConfig(_FolderId, default_xml_id, template_active_list, xml_active_list)
        End If

    End Sub
#End Region

#Region "Site Map/Breadcrumb"

#End Region

#Region "Ajax functions"

    Private Function AJAXcheck(ByVal sResponse As String, ByVal sURLQuery As String) As String
        Dim wb As New workareabase
        wb.AJAX.ResponseJS = sResponse
        wb.AJAX.URLQuery = sURLQuery
        wb.AJAX.FunctionName = "checkName"
        Return wb.AJAX.Render
    End Function

    Private Function GetResponseString(ByVal nextfunction As String) As String
        Dim sbAEJS As New System.Text.StringBuilder
        sbAEJS.Append("    if (response > 0){").Append(Environment.NewLine)
        sbAEJS.Append("	        alert('" & Me._MessageHelper.GetMessage("com: subfolder already exists") & "');").Append(Environment.NewLine)
        sbAEJS.Append("	        bexists = false;").Append(Environment.NewLine)
        sbAEJS.Append("    }else{").Append(Environment.NewLine)
        sbAEJS.Append("	        bexists = ").Append(nextfunction).Append("();").Append(Environment.NewLine)
        sbAEJS.Append("    } ").Append(Environment.NewLine)
        Return sbAEJS.ToString()
    End Function

#End Region

#Region "flagging section"
    Private Sub DrawFlaggingOptions()
        'Dim str As New StringBuilder()

        'Try
        '          str.Append("" & m_refMsg.GetMessage("lbl flagging inherit parent config:") & "<input type=""checkbox"" id=""flagging_options_inherit_cbx"" name=""flagging_options_inherit_cbx"" " + IIf((folder_data.Id = 0), "disabled=""disabled"" ", "") + IIf(folder_data.FlagInherited And (Not (folder_data.Id = 0)), "checked=""checked"" ", "") + """ onclick=""InheritFlagingChanged()"" />" + Environment.NewLine)
        '	str.Append("<input type=""hidden"" id=""flagging_options_inherit_hf"" value=""" + IIf(folder_data.FlagInherited, "True", "False") + """ />" + Environment.NewLine)
        '	'str.Append("<br /><br />Flagging Configuration: -HC " + Environment.NewLine)
        '	str.Append("<table width=""100%"" >" + Environment.NewLine)
        '	str.Append("  <tr>" + Environment.NewLine)
        '	str.Append("    <td>" + Environment.NewLine)
        '	str.Append("      <table cellspacing=""4"" cellpadding=""0"" width=""100%"">" + Environment.NewLine)
        '	str.Append("        <tr>" + Environment.NewLine)
        '	str.Append("          <td>" + Environment.NewLine)
        '	str.Append("            <table class=""center"" cellspacing=""0"" cellpadding=""0"" width=""100%"">" + Environment.NewLine)
        '	str.Append("              <tr>" + Environment.NewLine)
        '	str.Append("                <td width=""50%"">" + Environment.NewLine)
        '	str.Append("                  <table width=""100%"">" + Environment.NewLine)
        '	str.Append("                    <tr>" + Environment.NewLine)
        '	str.Append("                      <td width=""45%"">" + Environment.NewLine)
        '          str.Append("" & m_refMsg.GetMessage("lbl assigned flags:") + Environment.NewLine)
        '	str.Append("                        <select name=""flagging_options_assigned"" id=""flagging_options_assigned"" multiple=""multiple""" + Environment.NewLine)
        '	str.Append("                           " + IIf(folder_data.FlagInherited, "disabled=""disabled"" ", "") + " size=""4"" style=""width: 100%"">" + Environment.NewLine)
        '	'
        '	' Generate an option for each assigned flag:
        '	str.Append(GetAssignedFlags(True) + Environment.NewLine)
        '	str.Append("                        </select>" + Environment.NewLine)
        '	str.Append("                      </td>" + Environment.NewLine)
        '	str.Append("                      <td class=""center"">" + Environment.NewLine)
        '	str.Append("                        <table cellspacing=""0"" cellpadding=""5"">" + Environment.NewLine)
        '	str.Append("                          <tr>" + Environment.NewLine)
        '	str.Append("                            <td>" + Environment.NewLine)
        '	str.Append("                              &nbsp;" + Environment.NewLine)
        '	str.Append("                            </td>" + Environment.NewLine)
        '	str.Append("                          </tr>" + Environment.NewLine)
        '	str.Append("                          <tr>" + Environment.NewLine)
        '	str.Append("                            <td class=""center"">" + Environment.NewLine)
        '	str.Append("                              <input type=""button"" id=""flagging_options_moveLeftBtn"" onclick=""moveFlagsLeft();"" value="" &lt; "" " + IIf(folder_data.FlagInherited, "disabled=""disabled"" ", "") + " />" + Environment.NewLine)
        '	'str.Append("                            </td>" + Environment.NewLine)
        '	'str.Append("                          </tr>" + Environment.NewLine)
        '	'str.Append("                          <tr>" + Environment.NewLine)
        '	'str.Append("                            <td class=""center"">" + Environment.NewLine)
        '	str.Append("                              <input type=""button"" id=""flagging_options_moveRighBtn"" onclick=""moveFlagsRight();"" value="" &gt; "" " + IIf(folder_data.FlagInherited, "disabled=""disabled"" ", "") + " />" + Environment.NewLine)
        '	str.Append("                            </td>" + Environment.NewLine)
        '	str.Append("                          </tr>" + Environment.NewLine)
        '	str.Append("                          <tr>" + Environment.NewLine)
        '	str.Append("                            <td class=""center"">" + Environment.NewLine)
        '	str.Append("                              <input type=""button"" id=""flagging_options_setDefaultBtn"" onclick=""setDefaultFlag();"" value=""Default"" " + IIf(folder_data.FlagInherited, "disabled=""disabled"" ", "") + " />" + Environment.NewLine)
        '	str.Append("                            </td>" + Environment.NewLine)
        '	str.Append("                          </tr>" + Environment.NewLine)
        '	str.Append("                        </table>" + Environment.NewLine)
        '	str.Append("                      </td>" + Environment.NewLine)
        '	str.Append("                      <td width=""45%"">" + Environment.NewLine)
        '          str.Append("" & m_refMsg.GetMessage("lbl avail flags:") + Environment.NewLine)
        '	str.Append("                        <select name=""flagging_options_available"" id=""flagging_options_available"" multiple=""multiple""" + Environment.NewLine)
        '	str.Append("                          " + IIf(folder_data.FlagInherited, "disabled=""disabled"" ", "") + " size=""4"" style=""width: 100%"">" + Environment.NewLine)
        '	'
        '	' Generate an option for each un-assigned flag:
        '	str.Append(GetUnassignedFlags() + Environment.NewLine)
        '	str.Append("                        </select>" + Environment.NewLine)
        '	str.Append("                      </td>" + Environment.NewLine)
        '	str.Append("                    </tr>" + Environment.NewLine)
        '	str.Append("                  </table>" + Environment.NewLine)
        '	str.Append("                </td>" + Environment.NewLine)
        '	str.Append("              </tr>" + Environment.NewLine)
        '	str.Append("            </table>" + Environment.NewLine)
        '	str.Append("          </td>" + Environment.NewLine)
        '	str.Append("        </tr>" + Environment.NewLine)
        '	str.Append("      </table>" + Environment.NewLine)
        '	str.Append("    </td>" + Environment.NewLine)
        '	str.Append("  </tr>" + Environment.NewLine)
        '	'
        '	' Store currently assigned flags in a hidden field:
        '	str.Append("  <input type=""hidden"" name=""flagging_options_hdn"" id=""flagging_options_hdn"" value=""" + GetFolderFlags() + """ />" + Environment.NewLine)
        '	str.Append("  <input type=""hidden"" name=""flagging_options_default_hdn"" id=""flagging_options_default_hdn"" value=""" + GetDefaultFolderFlag() + """ />" + Environment.NewLine)
        '	str.Append("</table>" + Environment.NewLine)

        '	flagging_options.Text = str.ToString

        'Catch ex As Exception
        'Finally
        '	str = Nothing
        'End Try
        inheritFlag.Text = "<input type=""checkbox"" id=""flagging_options_inherit_cbx"" name=""flagging_options_inherit_cbx"" checked=""checked"" onclick=""InheritFlagingChanged('" & ddflags.ClientID & "')"" />" & _MessageHelper.GetMessage("lbl inherit parent configuration")
        ddflags.Enabled = False
        ddflags.Items.Add(New ListItem(" -None- ", 0))
        ddflags.Items.FindByValue(0).Selected = True
        'Dim flag_fdata() As FolderFlagDefData = m_refContentApi.GetAllFolderFlagDef(0)
        Dim flag_data() As FlagDefData = _ContentApi.EkContentRef.GetAllFlaggingDefinitions(False)
        If (flag_data IsNot Nothing AndAlso flag_data.Length > 0) Then
            For i As Integer = 0 To flag_data.Length - 1
                ddflags.Items.Add(New ListItem(flag_data(i).Name, flag_data(i).ID))
                'If (flag_fdata IsNot Nothing AndAlso flag_fdata.Length > 0 AndAlso flag_fdata(0).ID = flag_data(i).ID) Then
                '    ddflags.Items.FindByValue(flag_data(i).ID).Selected = True
                '    ddflags.SelectedIndex = ddflags.Items.IndexOf(ddflags.Items.FindByValue(flag_data(i).ID))
                'End If
            Next
        End If
        If (_FolderData.FolderFlags IsNot Nothing AndAlso _FolderData.FolderFlags.Length > 0) Then
            parent_flag.Value = _FolderData.FolderFlags(0).ID
        End If
    End Sub

    'Protected Function GetFolderFlags() As String
    '	Dim result As String = ""
    '	Dim flags() As FolderFlagDefData
    '	Dim flag As FolderFlagDefData

    '	Try
    '		flags = folder_data.FolderFlags
    '		For Each flag In flags
    '			If result.Length > 0 Then
    '				result += ","
    '			End If
    '			result += flag.ID.ToString
    '		Next

    '	Catch ex As Exception
    '	Finally
    '		GetFolderFlags = result
    '	End Try
    'End Function

    'Protected Function GetDefaultFolderFlag() As String
    '	Dim result As String = ""
    '	Dim flags() As FolderFlagDefData
    '	Dim flag As FolderFlagDefData

    '	Try
    '		flags = folder_data.FolderFlags
    '		For Each flag In flags
    '			If (flag.IsDefault) Then
    '				result = flag.ID.ToString
    '			End If
    '		Next

    '	Catch ex As Exception
    '	Finally
    '		GetDefaultFolderFlag = result
    '	End Try
    'End Function

    'Protected Function GetAssignedFlags(Optional ByVal showDefault As Boolean = False) As String
    '	Dim result As New StringBuilder()
    '	Dim flags() As FolderFlagDefData
    '	Dim flag As FolderFlagDefData
    '	Dim assignedDefault As Boolean = False

    '	Try
    '		flags = folder_data.FolderFlags	'flags = m_refContentApi.GetAllFolderFlagDef(folder_data.Id)
    '		For Each flag In flags
    '			' until API supports reporting the default flag for a folder, assume first item is default:
    '			If (showDefault AndAlso (Not assignedDefault)) Then
    '				assignedDefault = True
    '				result.Append("                          <option value=""" + flag.ID.ToString + """>" + flag.Name + " (default)" + "</option>" + Environment.NewLine)
    '			Else
    '				result.Append("                          <option value=""" + flag.ID.ToString + """>" + flag.Name + "</option>" + Environment.NewLine)
    '			End If
    '			_assignedFlags.Add(flag.Name, flag.Name)
    '		Next

    '	Catch ex As Exception
    '	Finally
    '		GetAssignedFlags = result.ToString
    '		result = Nothing
    '	End Try
    'End Function

    'Protected Function GetUnassignedFlags() As String
    '	Dim result As New StringBuilder()

    '	Try
    '		Dim aFlagSets() As FlagDefData = Array.CreateInstance(GetType(Ektron.Cms.FlagDefData), 0)
    '		aFlagSets = Me.m_refContentApi.EkContentRef.GetAllFlaggingDefinitions(False)
    '		Dim aFlagSet As FlagDefData
    '		For Each aFlagSet In aFlagSets
    '			If (Not _assignedFlags.Contains(aFlagSet.Name)) Then
    '				result.Append("                          <option value=""" + aFlagSet.ID.ToString + """>" + aFlagSet.Name + "</option>" + Environment.NewLine)
    '			End If
    '		Next

    '	Catch ex As Exception
    '	Finally
    '		GetUnassignedFlags = result.ToString
    '		result = Nothing
    '	End Try
    'End Function

    Private Sub ProcessFlaggingPostBack(ByRef pageCol As Collection)
        Dim inheritParentConfig As Boolean = False

        'Try
        '	If (Not IsNothing(Request.Form("flagging_options_inherit_cbx"))) Then
        '		inheritParentConfig = "on" = Request.Form("flagging_options_inherit_cbx").ToLower
        '	End If

        '	' Update settings to db:
        '	pagedata.Add(IIf(inheritParentConfig, 1, 0), "InheritFlag")
        '	If (Not inheritParentConfig) Then
        '		pagedata.Add(0, "InheritFlagFrom")
        '		If ((Not IsNothing(Request.Form("flagging_options_default_hdn"))) AndAlso (IsNumeric(Request.Form("flagging_options_default_hdn")))) Then
        '			pagedata.Add(CType(Request.Form("flagging_options_default_hdn"), Integer), "DefaultFlagId")
        '		Else
        '			pagedata.Add(0, "DefaultFlagId") ' TODO: Check, should we leave this key non-existant when no default is known?
        '		End If
        '		If ((Not inheritParentConfig) AndAlso (Not IsNothing(Request.Form("flagging_options_hdn")))) Then
        '			pagedata.Add(Request.Form("flagging_options_hdn"), "FlagList")
        '		End If
        '	End If

        'Catch ex As Exception
        'Finally
        'End Try
        Try
            If (Not IsNothing(Request.Form("flagging_options_inherit_cbx"))) Then
                inheritParentConfig = "on" = Request.Form("flagging_options_inherit_cbx").ToLower
            End If

            ' Update settings to db:
            _PageData.Add(IIf(inheritParentConfig, True, False), "InheritFlag")
            If (Not inheritParentConfig) Then
                _PageData.Add(0, "InheritFlagFrom")
                If (Request.Form(ddflags.UniqueID) IsNot Nothing) Then
                    _PageData.Add(Request.Form(ddflags.UniqueID), "DefaultFlagId")
                Else
                    _PageData.Add(0, "DefaultFlagId") ' TODO: Check, should we leave this key non-existant when no default is known?
                End If
                If ((Not inheritParentConfig) AndAlso (Request.Form(ddflags.UniqueID) IsNot Nothing)) Then
                    _PageData.Add(Request.Form(ddflags.UniqueID), "FlagList")
                End If
            Else
                _PageData.Add(Request.Form(parent_flag.UniqueID), "DefaultFlagId")
                _PageData.Add(Request.Form(parent_flag.UniqueID), "FlagList")
            End If
        Catch ex As Exception
        Finally
        End Try
    End Sub
#End Region


    Public Function IsPublishedAsPdf() As String
        Return _IsPublishedAsPdf
    End Function

End Class
