Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.CustomFieldsApi
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.DataIO.LicenseManager


Partial Class editfolderattributes
    Inherits System.Web.UI.UserControl


#Region "Member Variables"

    Private Const BLANK_HTML As String = "__EkBlankHTML"
    Private _SubscriptionDataList As SubscriptionData()
    Private _SubscribedDataList As SubscriptionData()
    Private _SubscriptionPropertiesList As SubscriptionPropertiesData
    Private _BreakSubInheritance As Boolean = False
    Private _GlobalSubInherit As Boolean = False
    Private _FolderType As Integer = 0
    Private _BlogData As BlogData
    Private _i As Integer = 0

    Protected _ContentApi As New ContentAPI
    Protected _CustomFieldsApi As New CustomFieldsApi
    Protected _StyleHelper As New StyleHelper
    Protected _MessageHelper As Common.EkMessageHelper
    Protected _Id As Long = 0
    Protected _FolderData As FolderData
    Protected _PermissionData As PermissionData
    Protected _AppImgPath As String = ""
    Protected _ContentType As Integer = 1
    Protected _CurrentUserId As Long = 0
    Protected _PageData As Collection
    Protected _PageAction As String = ""
    Protected _OrderBy As String = ""
    Protected _ContentLanguage As Integer = -1
    Protected _EnableMultilingual As Integer = 0
    Protected _SitePath As String = ""
    Protected _FolderId As Long = -1
    Protected _ShowPane As String = ""
    Protected _SelectedTaxonomyList As String = ""
    Protected _CurrentCategoryChecked As Integer = 0
    Protected _SelectedTaxonomyParentList As String = ""
    Protected _ParentCategoryChecked As Integer = 0
    Protected _AssignedFlags As New Hashtable
    Protected _IsUserBlog As Boolean = False
    Protected _ProductType As ProductType = Nothing
    Protected _IsCatalog As Boolean = False
    Protected _IsPublishedAsPdf As String = String.Empty

#End Region

#Region "Events"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        RegisterResources()
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        _MessageHelper = _ContentApi.EkMsgRef
    End Sub

#End Region

#Region "Helpers"

    Public Function EditFolderAttributes() As Boolean
        If (Not (Request.QueryString("id") Is Nothing)) Then
            _Id = Convert.ToInt64(Request.QueryString("id"))
        End If

        If (_FolderData Is Nothing) Then
            _FolderData = _ContentApi.GetFolderById(_Id, True, True)
        End If

        If (Not (Request.QueryString("action") Is Nothing)) Then
            _PageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If
        If (Not (Request.QueryString("orderby") Is Nothing)) Then
            _OrderBy = Convert.ToString(Request.QueryString("orderby"))
        End If
        If (Not (Request.QueryString("showpane") Is Nothing)) Then
            _ShowPane = Convert.ToString(Request.QueryString("showpane"))
        Else
            _ShowPane = ""
        End If
        If (Not (Request.QueryString("folder_id") Is Nothing)) Then
            _FolderId = Convert.ToInt64(Request.QueryString("folder_id"))
        End If
        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            If (Request.QueryString("LangType") <> "") Then
                _ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                _ContentApi.SetCookieValue("LastValidLanguageID", _ContentLanguage)
            Else
                If _ContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    _ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If _ContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                _ContentLanguage = Convert.ToInt32(_ContentApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If
        If _ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            _ContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
            _CustomFieldsApi.ContentLanguage = _ContentApi.DefaultContentLanguage()
        Else
            _ContentApi.ContentLanguage = _ContentLanguage
            _CustomFieldsApi.ContentLanguage = _ContentLanguage
        End If
        _CurrentUserId = _ContentApi.UserId
        _AppImgPath = _ContentApi.AppImgPath
        _SitePath = _ContentApi.SitePath
        _EnableMultilingual = _ContentApi.EnableMultilingual
        If (Not (Page.IsPostBack)) Then
            _FolderData = _ContentApi.GetFolderById(_Id, True, True)
            _FolderType = _FolderData.FolderType
            Select Case _FolderType
                Case Common.EkEnumeration.FolderType.Catalog
                    _IsCatalog = True
                    Display_EditCatalog()
                Case Else
                    Display_EditFolder()
            End Select
            phWebAlerts.Visible = (_FolderType <> Common.EkEnumeration.FolderType.Catalog)
        Else
            Process_DoFolderUpdate()
            Return (True)
        End If
    End Function

    Private Sub Display_AddCommunityFolder()
        If (_FolderData Is Nothing) Then
            _FolderData = _ContentApi.GetFolderById(_Id, True)
        End If
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar("Edit Community Folder " & " """ & _FolderData.Name & """")
        ltr_vf_types.Visible = True
        ltrTypes.Visible = True
    End Sub

#End Region

#Region "ACTION - DoFolderUpdate"
    Private Sub Process_DoFolderUpdate()
        Dim bInheritanceIsDif As Boolean
        bInheritanceIsDif = False
        Dim isub As Integer = 0
        Dim init_xmlconfig As String = Request.Form("init_xmlconfig")
        Dim init_frm_xmlinheritance As String = Request.Form("init_frm_xmlinheritance")
        Dim XmlInd As Ektron.Cms.Content.EkXmlIndexing
        Dim folder_data As FolderData = Nothing
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim sub_prop_data As New SubscriptionPropertiesData
        Dim page_subscription_data As New Collection
        Dim page_sub_temp As New Collection
        Dim arrSubscriptions As Array
        Dim i As Integer = 0
        Dim abriRoll As BlogRollItem()
        Dim sCatTemp As String = ""
        Dim siteAliasList As New List(Of String)
        Dim arSiteAliasList() As String
        Dim aliasStr As String
        Dim _refSiteAliasApi As Ektron.Cms.SiteAliasApi
        Dim subscriptionRestore As Boolean = False

        m_refContent = _ContentApi.EkContentRef
        If (_FolderId = -1) Then
            _FolderId = _Id 'i.e Request.Form(folder_id.UniqueID)
        End If
        _FolderData = _ContentApi.GetFolderById(_Id, True, True)
        _FolderType = _FolderData.FolderType

        If (Convert.ToString(_FolderId) <> "") Then

            If (_FolderType <> Common.EkEnumeration.FolderType.Catalog) Then
                If Len(Request.Form("web_alert_inherit_checkbox")) Then
                    sub_prop_data.BreakInheritance = False
                    subscriptionRestore = True
                Else
                    sub_prop_data.BreakInheritance = True
                    If Len(Request.Form("web_alert_restore_inherit_checkbox")) Then subscriptionRestore = True
                End If

                Select Case Request.Form("notify_option")
                    Case ("Always")
                        sub_prop_data.NotificationType = Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Always
                    Case ("Initial")
                        sub_prop_data.NotificationType = Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Initial
                    Case ("Never")
                        sub_prop_data.NotificationType = Common.EkEnumeration.SubscriptionPropertyNotificationTypes.Never
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
                            If (arrSubscriptions(isub) <> ",") Then ' ignore empty value when web alerts are inherited
                                page_sub_temp = New Collection
                                page_sub_temp.Add(CLng(Mid(arrSubscriptions(isub), 10)), "ID")
                                page_subscription_data.Add(page_sub_temp)
                            End If
                        Next
                    End If
                Else
                    page_subscription_data = Nothing
                End If
                page_sub_temp = Nothing
            End If

            _PageData = New Collection
            _PageData.Add(Request.Form("foldername").Trim("."), "FolderName")
            If Request.Form("isblog") <> "" Then
                _PageData.Add(Request.Form(tagline.UniqueID), "FolderDescription")
            Else
                _PageData.Add(Request.Form(folderdescription.UniqueID), "FolderDescription")
            End If
            _PageData.Add(Request.Form(folder_id.UniqueID), "FolderID")
            If (Request.Form("TemplateTypeBreak") Is Nothing) Then
                _PageData.Add(Request.Form("templatefilename"), "TemplateFileName")
                Dim templateName As String = Request.Form("templatefilename").Split("(")(0).TrimEnd()
				Dim template_data As TemplateData()
                template_data = _ContentApi.GetAllTemplates("TemplateFileName")
                Dim j As Integer = 0
                For j = 0 To template_data.Length - 1
                    If (Not Request.Form("tinput_" & template_data(j).Id) Is Nothing AndAlso template_data(j).FileName = templateName) Then
                        _PageData.Add(template_data(j).SubType, "TemplateSubType")
                    End If
                Next
            Else
                _PageData.Add("", "TemplateFileName")
            End If
            '_PageData.Add(Request.Form("templatefilename"), "TemplateFileName")
            _PageData.Add(Request.Form("stylesheet"), "StyleSheet")
            If (_FolderType <> Common.EkEnumeration.FolderType.Calendar) Then
                If (LCase(Request.Form("TypeBreak")) = "on") Then
                    If init_frm_xmlinheritance = "0" Then
                        bInheritanceIsDif = True
                    End If
                    _PageData.Add(True, "XmlInherited")
                Else
                    If init_frm_xmlinheritance = "1" Then
                        bInheritanceIsDif = True
                    End If
                    _PageData.Add(False, "XmlInherited")
                End If
                _PageData.Add(Request.Form("xmlconfig"), "XmlConfiguration")
            Else
                bInheritanceIsDif = False
                _PageData.Add(False, "XmlInherited")
                _PageData.Add(Ektron.Cms.Content.Calendar.WebCalendar.WebEventSmartformId.ToString(), "XmlConfiguration")
            End If

            ' handle multitemplates if there are any
            i = 1
            Dim altinfo As New Collection
            'While (Request.Form("namealt" + CStr(i)) <> "")
            '    Dim namealt As String = Request.Form("namealt" + CStr(i))
            '    Dim xmlconfigalt As String = Request.Form("xmlconfigalt" + CStr(i))
            '    If (xmlconfigalt = "ignore") Then xmlconfigalt = -1
            '    Dim templatealt As String = Request.Form("templatealt" + CStr(i))
            '    If (templatealt = "ignore") Then templatealt = -1
            '    If ((xmlconfigalt > -1) Or (templatealt > -1)) Then
            '        ' add this multitemplate only if a template or config is selected
            '        Dim multitemplate As New Collection
            '        multitemplate.Add(m_intFolderId, "FolderID")
            '        multitemplate.Add(xmlconfigalt, "CollectionID")
            '        multitemplate.Add(templatealt, "TemplateFileID")
            '        multitemplate.Add("", "CSSFile")
            '        multitemplate.Add(namealt, "Name")
            '        altinfo.Add(multitemplate)
            '    End If
            '    i = i + 1
            'End While
            'm_refContentApi.UpdateFolderContentTemplates(m_intFolderId, altinfo)


            Dim isPublishedAsPdf As Boolean = IIf(Request.Form("publishAsPdf") = "on", True, False)
            _PageData.Add(isPublishedAsPdf, "PublishPdfActive")

            ' handle dynamic replication properties
            If (folder_data Is Nothing) Then
                folder_data = _ContentApi.GetFolderById(_FolderId, True, True)
            End If
            If (Request.Form("EnableReplication") <> "" Or folder_data.IsCommunityFolder) Then
                _PageData.Add(1, "EnableReplication")
            Else
                _PageData.Add(0, "EnableReplication")
            End If

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
            If Request.Form("isblog") <> "" Then 'isblog
                _PageData.Add(True, "isblog")
                _PageData.Add(Request.Form("blog_visibility"), "blog_visibility")
                _PageData.Add(Request.Form("blogtitle"), "blogtitle")
                If Request.Form("postsvisible") <> "" Then
                    _PageData.Add(Request.Form("postsvisible"), "postsvisible")
                Else
                    _PageData.Add(-1, "postsvisible")
                End If
                If (Request.Form("enable_comments") <> "") Then
                    _PageData.Add(True, "enablecomments")
                Else
                    _PageData.Add(False, "enablecomments")
                End If
                If (Request.Form("moderate_comments") <> "") Then
                    _PageData.Add(True, "moderatecomments")
                Else
                    _PageData.Add(False, "moderatecomments")
                End If
                If (Request.Form("require_authentication") <> "") Then
                    _PageData.Add(True, "requireauthentication")
                Else
                    _PageData.Add(False, "requireauthentication")
                End If
                _PageData.Add(Request.Form("notify_url"), "notifyurl")
                If Request.Form("categorylength") <> "" Then
                    For i = 0 To (Request.Form("categorylength") - 1)
                        If Request.Form("category" & i.ToString()) <> "" Then
                            If i = (Request.Form("categorylength") - 1) Then
                                sCatTemp &= Replace(Request.Form("category" & i.ToString()), ";", "~@~@~")
                            Else
                                sCatTemp &= Replace(Request.Form("category" & i.ToString()), ";", "~@~@~") & ";"
                            End If
                        End If
                    Next
                End If
                _PageData.Add(sCatTemp, "blogcategories")

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
            End If
            If ((Request.Form("hdnInheritSitemap") IsNot Nothing) AndAlso (Request.Form("hdnInheritSitemap").ToString().ToLower() = "true")) Then
                _PageData.Add(True, "SitemapPathInherit")
            Else
                _PageData.Add(False, "SitemapPathInherit")
            End If

            _PageData.Add(Utilities.DeserializeSitemapPath(Request.Form, Me._ContentLanguage), "SitemapPath")
            If (Request.Form("break_inherit_button") IsNot Nothing AndAlso Request.Form("break_inherit_button").ToString().ToLower() = "on") Then
                _PageData.Add(1, "InheritMetadata") 'break inherit button is check.
            Else
                _PageData.Add(0, "InheritMetadata")
            End If
            _PageData.Add(Request.Form("inherit_meta_from"), "InheritMetadataFrom")

            If (Request.Form("TaxonomyTypeBreak") IsNot Nothing AndAlso Request.Form("TaxonomyTypeBreak").ToString().ToLower() = "on") Then
                _PageData.Add(1, "InheritTaxonomy")
                If (Request.Form("CategoryRequired") IsNot Nothing AndAlso Request.Form("CategoryRequired").ToString().ToLower() = "on") Then
                    _PageData.Add(1, "CategoryRequired")
                Else
                    _PageData.Add(Request.Form(parent_category_required.UniqueID), "CategoryRequired")
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

            m_refContent.UpdateFolderPropertiesv2_0(_PageData)
            If (folder_data.FolderType = 2) Then 'OrElse folder_data.Id = 0 Avoiding root to be site aliased
                arSiteAliasList = Request.Form("savedSiteAlias").TrimStart(" ").TrimStart(",").Split(",")
                For Each aliasStr In arSiteAliasList
                    If (aliasStr <> String.Empty) Then
                        siteAliasList.Add(aliasStr)
                    End If
                Next
                _refSiteAliasApi = New Ektron.Cms.SiteAliasApi()
                _refSiteAliasApi.Save(folder_data.Id, siteAliasList)
            End If
            If ((Not (Request.Form("suppress_notification") <> "")) And (_FolderType <> Common.EkEnumeration.FolderType.Catalog)) Then
                m_refContent.UpdateSubscriptionPropertiesForFolder(_FolderId, sub_prop_data)
                m_refContent.UpdateSubscriptionsForFolder(_FolderId, page_subscription_data)
            End If
            If subscriptionRestore Then m_refContent.DeleteSubscriptionsForContentinFolder(_FolderId)

            If (init_xmlconfig <> Request.Form("xmlconfig") Or bInheritanceIsDif) And _FolderType <> Common.EkEnumeration.FolderType.Calendar Then
                XmlInd = _ContentApi.EkXmlIndexingRef
                If Request.Form("xmlconfig") <> "0" And Request.Form("xmlconfig") <> "" Then
                    XmlInd.ReIndexAllDoc(Request.Form("xmlconfig"))
                Else ' inheritance has been turned on
                    If (LCase(Request.Form("frm_xmlinheritance")) = "on") Then
                        folder_data = _ContentApi.GetFolderById(_FolderId, False, True)
                        If (Not (IsNothing(folder_data.XmlConfiguration))) Then
                            For x As Integer = 0 To (folder_data.XmlConfiguration.Length - 1)
                                XmlInd.ReIndexAllDoc(folder_data.XmlConfiguration(x).Id)
                            Next
                            'reverting 27535 - do not udpate xml_index table with new xml index search
                        Else
                            XmlInd.RemoveAllIndexDoc(_FolderId)
                        End If
                        'reverting 27535 - do not udpate xml_index table with new xml index search
                    Else
                        XmlInd.RemoveAllIndexDoc(_FolderId)
                    End If
                End If

            End If

            If Request.Form("break_inherit_button") Is Nothing Then
                _CustomFieldsApi.ProcessCustomFields(_FolderId)
            ElseIf (Request.Form("break_inherit_button") IsNot Nothing AndAlso Request.Form("break_inherit_button").ToString().ToLower() = "on") Then
                If folder_data.MetaInherited = 0 Then
                    _CustomFieldsApi.ProcessCustomFields(_FolderId)
                End If
            End If

            'If (Request.Form("break_inherit_button") IsNot Nothing AndAlso Request.Form("break_inherit_button").ToString().ToLower() = "on") Then
            '    'break inherit button is checked.
            '    _CustomFieldsApi.ProcessCustomFields(_FolderId)
            'ElseIf folder_data.MetaInherited = 0 Then
            '    _CustomFieldsApi.ProcessCustomFields(_FolderId)
            'ElseIf Request.Form("break_inherit_button") Is Nothing Then
            '    _CustomFieldsApi.ProcessCustomFields(_FolderId)
            'End If
        End If
        If (Request.Form("oldfoldername") = Request.Form("foldername")) Then
            Response.Redirect("content.aspx?LangType=" & _ContentLanguage & "&action=ViewFolder&id=" & Request.Form(folder_id.UniqueID), False)
        Else
            Response.Redirect("content.aspx?TreeUpdated=1&LangType=" & _ContentLanguage & "&action=ViewFolder&id=" & Request.Form(folder_id.UniqueID) & "&reloadtrees=Forms,Content,Library", False)
        End If
        If folder_data.FolderType = Common.EkEnumeration.FolderType.Catalog Then ProcessProductTemplatesPostBack() Else ProcessContentTemplatesPostBack()
    End Sub
#End Region

#Region "FOLDER - EditFolder"
    Private Sub Display_EditFolder()
        Dim template_data() As TemplateData
        Dim xmlconfig_data() As XmlConfigData
        Dim isBlog As Boolean = _FolderType = 1
        Dim i As Integer = 0

        ltInheritSitemapPath.Text = _MessageHelper.GetMessage("lbl inherit from parent")

        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder")

        ltrTypes.Text = _MessageHelper.GetMessage("Smart Forms txt")

        If isBlog Then
            _BlogData = _ContentApi.BlogObject(_FolderData)
            _IsUserBlog = _ContentApi.IsUserBlog(_BlogData.Id)
            _FolderData.PublishPdfEnabled = False
            EditFolderToolBar()
            phSubjects.Visible = True
            phBlogRoll.Visible = True
            phDescription.Visible = False
        Else
            EditFolderToolBar()
        End If

        template_data = _ContentApi.GetAllTemplates("TemplateFileName")
        xmlconfig_data = _ContentApi.GetAllXmlConfigurations("title")

        oldfolderdescription.Value = Server.HtmlDecode(_FolderData.Description)
        folderdescription.Value = Server.HtmlDecode(_FolderData.Description)
        folder_id.Value = _FolderData.Id
        If (_Id = 0) Then
            phFolderProperties1.Visible = True
            lit_ef_folder.Text = _FolderData.Name & "<input type=""hidden"" value=""" & _FolderData.Name & """ name=""foldername""/>"
            lit_ef_folder.Text &= "<input type=""hidden"" value=""" & _FolderData.Name & """ name=""oldfoldername""/>"
        Else
            If isBlog Then
                phBlogProperties1.Visible = True
                phBlogProperties2.Visible = True
                td_vf_nametxt.InnerHtml = "<input type=""text"" maxlength=""75"" size=""30"" value=""" & _FolderData.Name & """ name=""foldername"" />"
                td_vf_nametxt.InnerHtml &= "<input type=""hidden"" value=""" & Server.HtmlEncode(_FolderData.Name) & """ name=""oldfoldername"" id=""oldfoldername"" />"
                td_vf_nametxt.InnerHtml &= "<input type=""hidden"" name=""isblog"" id=""isblog"" value=""true""/>"
                td_vf_titletxt.InnerHtml = "<input type=""text"" maxlength=""75"" size=""30"" value=""" & _BlogData.Title & """ name=""blogtitle"" id=""blogtitle"" />"
                td_vf_visibilitytxt.InnerHtml = "<select name=""blog_visibility"" id=""blog_visibility"">"
                If _BlogData.Visibility = Common.EkEnumeration.BlogVisibility.Public Then
                    td_vf_visibilitytxt.InnerHtml &= "<option value=""0"" selected>Public</option>"
                    td_vf_visibilitytxt.InnerHtml &= "<option value=""1"">Private</option>"
                Else
                    td_vf_visibilitytxt.InnerHtml &= "<option value=""0"">Public</option>"
                    td_vf_visibilitytxt.InnerHtml &= "<option value=""1"" selected>Private</option>"
                End If
                td_vf_visibilitytxt.InnerHtml &= "</select>"
                tagline.Value = Server.HtmlDecode(_BlogData.Tagline)
                If _BlogData.PostsVisible < 0 Then
                    td_vf_postsvisibletxt.InnerHtml = "<input type=""text"" name=""postsvisible"" id=""postsvisible"" value="""" size=""1"" maxlength=""3""/>"
                Else
                    td_vf_postsvisibletxt.InnerHtml = "<input type=""text"" name=""postsvisible"" id=""postsvisible"" value=""" & _BlogData.PostsVisible.ToString() & """ size=""1"" maxlength=""3""/>"
                End If
                td_vf_postsvisibletxt.InnerHtml &= "<div class=""ektronCaption"">(leave blank for selected day)</div>"
                If _BlogData.EnableComments = True Then
                    td_vf_commentstxt.InnerHtml &= "<input type=""checkbox"" name=""enable_comments"" id=""enable_comments"" checked=""checked"" onclick=""UpdateBlogCheckBoxes();"" />Enable comments"
                    td_vf_commentstxt.InnerHtml &= "<br />"
                    If _BlogData.ModerateComments Then
                        td_vf_commentstxt.InnerHtml &= "<input type=""checkbox"" name=""moderate_comments"" id=""moderate_comments"" checked=""checked"" />Moderate comments"
                    Else
                        td_vf_commentstxt.InnerHtml &= "<input type=""checkbox"" name=""moderate_comments"" id=""moderate_comments"" />Moderate comments"
                    End If
                    td_vf_commentstxt.InnerHtml &= "<br />"
                    If _BlogData.RequiresAuthentication Then
                        td_vf_commentstxt.InnerHtml &= "<input type=""checkbox"" name=""require_authentication"" id=""require_authentication"" checked=""checked"" />Require authentication"
                    Else
                        td_vf_commentstxt.InnerHtml &= "<input type=""checkbox"" name=""require_authentication"" id=""require_authentication"" />Require authentication"
                    End If
                Else
                    td_vf_commentstxt.InnerHtml &= "<input type=""checkbox"" name=""enable_comments"" id=""enable_comments"" onclick=""UpdateBlogCheckBoxes();"" />Enable comments<br />"
                    td_vf_commentstxt.InnerHtml &= "<input type=""checkbox"" name=""moderate_comments"" id=""moderate_comments"" disabled=""disabled""/>Moderate comments<br />"
                    td_vf_commentstxt.InnerHtml &= "<input type=""checkbox"" name=""require_authentication"" id=""require_authentication"" disabled=""disabled""/>Require authentication<br />"
                End If
                If _BlogData.NotifyURL <> "" Then
                    td_vf_updateservicestxt.InnerHtml &= "<input type=""checkbox"" name=""chknotify_url"" id=""chknotify_url"" checked=""checked"" />Notify blog search engines of new posts"
                    td_vf_updateservicestxt.InnerHtml &= "<br />"
                    td_vf_updateservicestxt.InnerHtml &= "<input type=""text"" maxlength=""75"" size=""40"" value=""" & Server.HtmlEncode(_BlogData.NotifyURL) & """ name=""notify_url"" id=""notify_url""/>"
                Else
                    td_vf_updateservicestxt.InnerHtml &= "<input type=""checkbox"" name=""chknotify_url"" id=""chknotify_url"" />Notify blog search engines of new posts"
                    td_vf_updateservicestxt.InnerHtml &= "<br />"
                    td_vf_updateservicestxt.InnerHtml &= "<input type=""text"" maxlength=""75"" size=""40"" value="""" name=""notify_url"" id=""notify_url""/>"
                End If
            Else
                phFolderProperties1.Visible = True
                lit_ef_folder.Text = "<input type=""text"" maxlength=""100"" size=""75"" value=""" & _FolderData.Name & """ name=""foldername""><input type=""hidden"" value="""" name=""oldfoldername"" id=""oldfoldername"" />"
            End If
        End If
        If ((_FolderData.StyleSheetInherited) And (_FolderData.StyleSheet <> "")) Then
            lit_ef_ss.Text = _ContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""" & 75 - _ContentApi.SitePath.Length & """ value="""" name=""stylesheet"" />"
            lit_ef_ss.Text += "<br/>"
            lit_ef_ss.Text += "<span class=""ektronCaption"">"
            lit_ef_ss.Text += _MessageHelper.GetMessage("inherited style sheet msg") & _ContentApi.SitePath & _FolderData.StyleSheet
            lit_ef_ss.Text += "</span>"
        Else
            lit_ef_ss.Text = _ContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""" & 75 - _ContentApi.SitePath.Length & """ value=""" & _FolderData.StyleSheet & """ name=""stylesheet"" />"
        End If
        lit_ef_templatedata.Text = "<input type=""hidden"" maxlength=""255"" size=""" & 75 - _ContentApi.SitePath.Length & """ value="""" name=""templatefilename"" id=""templatefilename"" />"


        DrawContentTemplatesTable()

        DrawFlaggingOptions()
        Dim iTmpCaller As Long = _ContentApi.RequestInformationRef.CallerId
        Try
            _ContentApi.RequestInformationRef.CallerId = Ektron.Cms.Common.EkConstants.InternalAdmin
            _ContentApi.RequestInformationRef.UserId = Ektron.Cms.Common.EkConstants.InternalAdmin

            Dim asset_config As AssetConfigInfo() = _ContentApi.GetAssetMgtConfigInfo()
            If asset_config(10).Value.IndexOf("ektron.com") > -1 Then
                ltrCheckPdfServiceProvider.Text = _MessageHelper.GetMessage("pdf service provider")
            Else
                ltrCheckPdfServiceProvider.Text = ""
            End If
        Catch ex As Exception
            Dim _error As String = ex.Message
        Finally
            _ContentApi.RequestInformationRef.CallerId = iTmpCaller
            _ContentApi.RequestInformationRef.UserId = iTmpCaller
        End Try
		
        If _FolderData.PublishPdfEnabled AndAlso _FolderType <> Ektron.Cms.Common.EkEnumeration.FolderType.Calendar Then
            phPDF.Visible = True
            _IsPublishedAsPdf = IIf(_FolderData.PublishPdfActive, "checked=""checked"" ", String.Empty)
            Me.lblPublishAsPdf.InnerText = _MessageHelper.GetMessage("publish as pdf")
			ltrCheckPdfServiceProvider.Text = _MessageHelper.GetMessage("pdf service provider")
            ltrCheckPdfServiceProvider.Visible = True
        Else
            _IsPublishedAsPdf = String.Empty
            phPDF.Visible = False
        End If

        ' only top level folders can be domain folders and only if not a blog folder already
        Dim m_refCommonAPI As New CommonApi()
        Dim request_info As Ektron.Cms.Common.EkRequestInformation = m_refCommonAPI.RequestInformationRef

        If (_FolderType <> 1) And (_FolderData.ParentId = 0) And (_Id <> 0) Then
            Dim settings_list As SettingsData
            Dim m_refSiteAPI As New SiteAPI

            settings_list = m_refSiteAPI.GetSiteVariables()
            Dim schk As String = ""
            Dim disdomain As String = ""
            If (_FolderData.IsDomainFolder) Then
                schk = " checked "
            Else
                disdomain = " disabled "
            End If
            If (_FolderType = 2 And LicenseManager.IsFeatureEnable(request_info, Feature.MultiSite)) Then
                ' Domain folder checkbox replaced to hidden field.

                'DomainFolder.Text += "<tr><td colspan=""2"">&nbsp;</td></tr><tr><td colspan=""2"" class=""input-box-text"">Multi-Site Domain Configuration:</td></tr>"
                'DomainFolder.Text += "<tr><td colspan=""2""><input type=""checkbox""  disabled= ""true"" name=""IsDomainFolder"" id=""IsDomainFolder""" & schk & " onClick="""
                'If (settings_list.AsynchronousStaging) Then
                '    DomainFolder.Text += "document.forms[0].DomainStaging.disabled = !document.forms[0].IsDomainFolder.checked; "
                'End If
                'DomainFolder.Text += "document.forms[0].DomainProduction.disabled = !document.forms[0].IsDomainFolder.checked;"
                'If (Not request_info.LinkManagement) Then
                '    DomainFolder.Text += " if (document.forms[0].IsDomainFolder.checked) alert('Please set ek_LinkManagement to True in your web.config');"
                'End If
                'DomainFolder.Text += """/><label for=""IsDomainFolder"">" & m_refMsg.GetMessage("alt Domain for this folder") & "</label></td></tr>"

                DomainFolder.Text += "<input type=""hidden"" name=""IsDomainFolder"" id=""IsDomainFolder"" value=""on""/>"

                ' staging field should only show up on staging servers; production server can see production field
                DomainFolder.Text += "<tr>"
                DomainFolder.Text += "<td class=""label""><label for=""DomainStaging"">" & _MessageHelper.GetMessage("lbl Staging Domain") & ":</label></td>"
                DomainFolder.Text += "<td class=""value"">http://&nbsp;<input type=""text"" name=""DomainStaging"" id=""DomainStaging"" size=""50"" value=""" & _FolderData.DomainStaging & """" + disdomain & "/></td>"
                DomainFolder.Text += "</tr>"

                DomainFolder.Text += "<tr>"
                DomainFolder.Text += "<td class=""label""><label for=""DomainProduction"">" & _MessageHelper.GetMessage("lbl Production Domain") & ":</label></td>"
                DomainFolder.Text += "<td class=""value"">http://&nbsp;<input type=""text"" name=""DomainProduction"" id=""DomainProduction"" size=""50"" value=""" & _FolderData.DomainProduction & """" + disdomain & "/></td>"
                DomainFolder.Text += "</tr>"
            End If
        End If
        ' handle dynamic replication settings
        If request_info.EnableReplication And Not (_FolderType = Common.EkEnumeration.FolderType.DiscussionForum Or _FolderType = Common.EkEnumeration.FolderType.DiscussionBoard) Then
            Dim bShowReplicationMethod As Boolean = True
            If (_FolderData.ParentId <> 0 AndAlso (_FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Blog)) Then
                ' don't show for blogs under community folder
                Dim tmp_folder_data As FolderData = Nothing
                tmp_folder_data = Me._ContentApi.EkContentRef.GetFolderById(_FolderData.ParentId)
                If (tmp_folder_data.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Community) Then
                    bShowReplicationMethod = False
                End If
            End If
            If bShowReplicationMethod Then
                Dim schk As String = ""
                If (_FolderData.ReplicationMethod = 1) Then
                    schk = " checked"
                End If

                If (Not _FolderData.IsCommunityFolder) Then
                    ReplicationMethod.Text = _MessageHelper.GetMessage("lbl folderdynreplication")
                    ReplicationMethod.Text += "<input type=""checkbox"" name=""EnableReplication"" id=""EnableReplication"" value=""1""" & schk & " ><label for=""EnableReplication""/>" & _MessageHelper.GetMessage("replicate folder contents") & "</label>"
                End If
            Else
                ' if we're not showing it, it means replication is enabled because we're under a parent community folder
                ReplicationMethod.Text = "<input type=""hidden"" name=""EnableReplication"" value=""1"" />"
            End If
        End If

        ' show categories if its a blog
        If isBlog Then
            ltr_vf_categories.Text &= "<div id=""parah"">"
            If Not (_BlogData.Categories Is Nothing) AndAlso _BlogData.Categories.Length > 0 AndAlso _BlogData.Categories(0).Length > 0 Then
                For i = 0 To _BlogData.Categories.Length - 1
                    ltr_vf_categories.Text &= "<input type='text' id='category" + i.ToString() + "' name='category" + i.ToString() + "' onChange='saveValue(" + i.ToString() + ",this.value)' value='" & Replace(Replace(_BlogData.Categories(i).ToString(), "~@~@~", ";"), "'", "&#39;") & "' maxlength='75' size='75'/> "
                    ltr_vf_categories.Text &= "<a href=""#Remove"" onclick=""removeInput(" + i.ToString() + ");return false;"" class=""button buttonInlineBlock redHover buttonRemove"">" & _MessageHelper.GetMessage("btn remove") & "</a>"
                    ltr_vf_categories.Text &= "<div class='ektronTopSpace'></div>"
                    ltr_vf_categories.Text &= "<script type=""text/javascript"">addInputInit('" & Replace(Replace(_BlogData.Categories(i).ToString(), "~@~@~", ";"), "'", "\'") & "');</script>"
                    '<p>" & blog_data.Categories(i).ToString() & "</p>"
                Next
                ltr_vf_categories.Text &= "</div>"
                ltr_vf_categories.Text &= "<input type=""hidden"" id=""categorylength"" name=""categorylength"" value=""" & _BlogData.Categories.Length.ToString() & """ />"
            Else
                ltr_vf_categories.Text &= "</div>"
                ltr_vf_categories.Text &= "<input type=""hidden"" id=""categorylength"" name=""categorylength"" value=""0"" />"
            End If
            ltr_vf_categories.Text &= "<a href=""#Add"" onclick=""addInput();return false;"" class=""button buttonInlineBlock greenHover buttonAdd"">" & Me._MessageHelper.GetMessage("lnk add new subject") & "</a>"
            ltr_vf_categories.Text &= "<a href=""#Remove"" onclick=""deleteInput();return false;"" class=""button buttonInlineBlock redHover buttonRemove"">" & Me._MessageHelper.GetMessage("lnk remove last subject") & "</a>"
            Dim ltrT As New Literal
            ltrT.Text &= "<div id=""proll"" name=""proll"">"
            If Not (_BlogData.BlogRoll Is Nothing) AndAlso _BlogData.BlogRoll.Length > 0 Then
                For i = 0 To _BlogData.BlogRoll.Length - 1
                    ltrT.Text &= "<a href=""#"" class=""button buttonInlineBlock redHover buttonRemove"" onClick=""removeRoll(" + i.ToString() + ")"">Remove Roll Link</a>"
                    ltrT.Text &= "<div class=""ektronTopSpace""></div>"
                    ltrT.Text &= "<table class=""ektronGrid"">"
                    ltrT.Text &= "  <tr>"
                    ltrT.Text &= "      <td class=""label"">Link Name:</td>"
                    ltrT.Text &= "      <td class=""value""><input name=""editfolder_linkname" + i.ToString() + """ type=""text"" value=""" + Server.HtmlEncode(_BlogData.BlogRoll.RollItem(i).LinkName) + """ size=""55"" id=""editfolder_linkname" + i.ToString() + """ onChange=""saveRoll(" + i.ToString() + ",this.value,'linkname')"" /></td>"
                    ltrT.Text &= "  </tr>"
                    ltrT.Text &= "  <tr>"
                    ltrT.Text &= "      <td class=""label"">URL:</td>"
                    ltrT.Text &= "      <td class=""value""><input name=""editfolder_url" + i.ToString() + """ type=""text"" value=""" + Server.HtmlEncode(_BlogData.BlogRoll.RollItem(i).URL) + """ size=""55"" id=""editfolder_url" + i.ToString() + """ onChange=""saveRoll(" + i.ToString() + ",this.value,'url')"" /></td>"
                    ltrT.Text &= "  </tr>"
                    ltrT.Text &= "  <tr>"
                    ltrT.Text &= "      <td class=""label"">Short Description:</td>"
                    ltrT.Text &= "      <td class=""value""><input name=""editfolder_short" + i.ToString() + """ type=""text"" value=""" + Server.HtmlEncode(_BlogData.BlogRoll.RollItem(i).ShortDescription) + """ size=""55"" id=""editfolder_short" + i.ToString() + """ onChange=""saveRoll(" + i.ToString() + ",this.value,'short')"" /></td>"
                    ltrT.Text &= "  </tr>"
                    ltrT.Text &= "  <tr>"
                    ltrT.Text &= "      <td class=""label"">Relationship:</td>"
                    ltrT.Text &= "      <td class=""value"">"
                    ltrT.Text &= "          <input name=""editfolder_rel" + i.ToString() + """ type=""text"" value=""" + Server.HtmlEncode(_BlogData.BlogRoll.RollItem(i).Relationship) + """ size=""45"" id=""editfolder_rel" + i.ToString() + """ onChange=""saveRoll(" + i.ToString() + ",this.value,'rel')"" />&nbsp;"""
                    ltrT.Text &= "          <a style=""padding-top: .25em; padding-bottom: .25em;"" class=""button buttonInline blueHover buttonEdit"" href=""#"" onClick=""window.open('blogs/xfnbuilder.aspx?field=editfolder_rel" + i.ToString() + "&id=" + i.ToString() + "','XFNBuilder','location=0,status=0,scrollbars=0,width=500,height=300');"">Edit</a>"
                    ltrT.Text &= "      </td>"
                    ltrT.Text &= "  </tr>"
                    ltrT.Text &= "</table>"
                    ltrT.Text &= "<div class=""ektronTopSpace""></div>"
                    ltrT.Text &= "<script type=""text/javascript"">addRollInit('" & Replace(_BlogData.BlogRoll.RollItem(i).LinkName, "'", "\'") & "','" & Replace(_BlogData.BlogRoll.RollItem(i).URL, "'", "\'") & "','" & Replace(_BlogData.BlogRoll.RollItem(i).ShortDescription, "'", "\'") & "','" & Replace(_BlogData.BlogRoll.RollItem(i).Relationship, "'", "\'") & "');</script>"
                Next
            End If
            ltrT.Text &= "</div>"
            ltrT.Text &= "<input type=""hidden"" id=""rolllength"" name=""rolllength"" value=""" & _BlogData.BlogRoll.Length.ToString() & """ />"
            ltrT.Text &= "<div class=""ektronTopSpace""></div>"
            ltrT.Text &= "<a href=""javascript:addRoll()"" class=""button buttonInlineBlock greenHover buttonAdd"">" & _MessageHelper.GetMessage("lnk add new roll link") & "</a>"
            ltrT.Text &= "<a href=""javascript:deleteRoll()"" class=""button buttonInlineBlock redHover buttonRemove"">" & _MessageHelper.GetMessage("lnk remove last roll link") & "</a>"
            lbl_vf_roll.Controls.Add(ltrT)
        End If

        If (_Id = 0) Then
			js_ef_focus.Text = "Ektron.ready(function(){document.forms.frmContent.stylesheet.focus();});"
		Else
			If Not (Request.QueryString("showpane") <> "") Then
				js_ef_focus.Text = "Ektron.ready(function() { document.forms.frmContent.foldername.focus();" & Environment.NewLine
				js_ef_focus.Text += "   if( $ektron('#web_alert_inherit_checkbox').length > 0 ){" & Environment.NewLine
				js_ef_focus.Text += "       if( $ektron('#web_alert_inherit_checkbox')[0].checked ){" & Environment.NewLine
				js_ef_focus.Text += "           $ektron('.selectContent').css('display', 'none');" & Environment.NewLine
				js_ef_focus.Text += "           $ektron('.useCurrent').css('display', 'none');" & Environment.NewLine
				js_ef_focus.Text += "       } " & Environment.NewLine
				js_ef_focus.Text += "   } " & Environment.NewLine
				js_ef_focus.Text += "});" & Environment.NewLine
			End If
			js_ef_focus.Text &= ("function UpdateBlogCheckBoxes() {" & Environment.NewLine)
			js_ef_focus.Text &= ("   if (document.forms[0].enable_comments.checked == true) {" & Environment.NewLine)
			js_ef_focus.Text &= ("       document.forms[0].moderate_comments.disabled = false;" & Environment.NewLine)
			js_ef_focus.Text &= ("       document.forms[0].require_authentication.disabled = false;" & Environment.NewLine)
			js_ef_focus.Text &= ("   } else {" & Environment.NewLine)
			js_ef_focus.Text &= ("       document.forms[0].moderate_comments.disabled = true;" & Environment.NewLine)
			js_ef_focus.Text &= ("       document.forms[0].require_authentication.disabled = true;" & Environment.NewLine)
			js_ef_focus.Text &= ("   }" & Environment.NewLine)
			js_ef_focus.Text &= ("}" & Environment.NewLine)
		End If
		DrawFolderTaxonomyTable()
		DisplaySitemapPath()
		DisplayMetadataInfo()
		DisplaySubscriptionInfo()
		DrawContentTypesTable()
		If (_FolderType = 2) Then 'OrElse folder_data.Id = 0 Avoiding sitealias for root.
			phSiteAlias.Visible = True
			phSiteAlias2.Visible = True
			DisplaySiteAlias()
		End If
		Showpane()

		If _FolderData.IsCommunityFolder Then
			Display_AddCommunityFolder()
		End If
	End Sub

	Public Function IsPublishedAsPdf() As String
		Return _IsPublishedAsPdf
	End Function

	Private checktaxid As Long = 0
	Private Sub DrawFolderTaxonomyTable()
		Dim categorydatatemplate As String = "<input onclick=""ValidateCatSel(this)"" type=""checkbox"" id=""taxlist"" name=""taxlist"" value=""{0}"" {1} {2}/>{3}"
		Dim categorydata As New StringBuilder
		Dim catdisabled As String = ""
		If (_FolderData.FolderTaxonomy IsNot Nothing AndAlso _FolderData.FolderTaxonomy.Length > 0) Then
			For i As Integer = 0 To _FolderData.FolderTaxonomy.Length - 1
				If (_SelectedTaxonomyList.Length > 0) Then
					_SelectedTaxonomyList = _SelectedTaxonomyList & "," & _FolderData.FolderTaxonomy(i).TaxonomyId
				Else
					_SelectedTaxonomyList = _FolderData.FolderTaxonomy(i).TaxonomyId
				End If
			Next
		End If
		_CurrentCategoryChecked = Convert.ToInt32(_FolderData.CategoryRequired)
		current_category_required.Value = _CurrentCategoryChecked
		inherit_taxonomy_from.Value = _FolderData.TaxonomyInheritedFrom
		Dim TaxArr As TaxonomyBaseData() = _ContentApi.EkContentRef.GetAllTaxonomyByConfig(Common.EkEnumeration.TaxonomyType.Content)
		Dim DisabledMsg As String = ""
		If (_FolderData.TaxonomyInherited) Then
			DisabledMsg = " disabled "
			catdisabled = " disabled "
		End If
		Dim parent_has_configuration As Boolean = False
		If (TaxArr IsNot Nothing AndAlso TaxArr.Length > 0) Then
			Dim i As Integer = 0
			While (i < TaxArr.Length)
				For j As Integer = 0 To 2
					If (i < TaxArr.Length) Then
						checktaxid = TaxArr(i).TaxonomyId
						parent_has_configuration = Array.Exists(_FolderData.FolderTaxonomy, AddressOf TaxonomyExists)
						categorydata.Append(String.Format(categorydatatemplate, TaxArr(i).TaxonomyId, IsChecked(parent_has_configuration), DisabledMsg, TaxArr(i).TaxonomyName))
						i = i + 1
					Else
						Exit For
					End If
					categorydata.Append("<br/>")
				Next
			End While
		End If

		Dim str As New StringBuilder()

		str.Append("<input type=""hidden"" id=""TaxonomyParentHasConfig"" name=""TaxonomyParentHasConfig"" value=""")
		If (parent_has_configuration) Then
			str.Append("1")
		Else
			str.Append("0")
		End If

		str.Append(""" />")

		DisabledMsg = " "
		If (_FolderData.Id = 0) Then
			DisabledMsg = " disabled "
		Else
			DisabledMsg = IsChecked(_FolderData.TaxonomyInherited)
		End If
		Dim catchecked As String = ""
		If (_FolderData.CategoryRequired) Then
			catchecked = " checked "
		End If
		If (_FolderData.Id > 0) Then
			Dim parentfolderdata As FolderData = _ContentApi.GetFolderById(_FolderData.ParentId, True)
			If (parentfolderdata.FolderTaxonomy IsNot Nothing AndAlso parentfolderdata.FolderTaxonomy.Length > 0) Then
				For i As Integer = 0 To parentfolderdata.FolderTaxonomy.Length - 1
					If (_SelectedTaxonomyParentList.Length > 0) Then
						_SelectedTaxonomyParentList = _SelectedTaxonomyParentList & "," & parentfolderdata.FolderTaxonomy(i).TaxonomyId
					Else
						_SelectedTaxonomyParentList = parentfolderdata.FolderTaxonomy(i).TaxonomyId
					End If
				Next
				_ParentCategoryChecked = Convert.ToInt32(parentfolderdata.CategoryRequired)
				parent_category_required.Value = _ParentCategoryChecked
			End If
		End If

		str.Append("<input name=""TaxonomyTypeBreak"" id=""TaxonomyTypeBreak"" type=""checkbox"" onclick=""ToggleTaxonomyInherit(this)"" " & DisabledMsg & "/>" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration"))
		str.Append("<br/>")
		str.Append("<input name=""CategoryRequired"" id=""CategoryRequired"" type=""checkbox""" & catchecked & catdisabled & " />" & _MessageHelper.GetMessage("alt required at least one category selection"))
		str.Append("<br/>")
		str.Append("<br/>")
		str.Append(categorydata.ToString())
		taxonomy_list.Text = str.ToString()
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
	Private Sub Showpane()
		If _ShowPane.Length > 0 Then
			lbl_vf_showpane.Text &= "<script type=""text/javascript"">" & Environment.NewLine
			Select Case _ShowPane
				Case "blogroll"
					lbl_vf_showpane.Text &= "   ShowPane('dvRoll');"
				Case "blogcat"
					lbl_vf_showpane.Text &= "   ShowPane('dvCategories');"
			End Select
			lbl_vf_showpane.Text &= "</script>" & Environment.NewLine
		End If
	End Sub
	Private Sub EditFolderToolBar()
		Dim result As New System.Text.StringBuilder
		txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("edit properties for folder msg") & " """ & _FolderData.Name & """")
		result.Append("<table><tr>")
		Dim isBlog As Boolean = _FolderType = 1
		If isBlog Then
			Dim sbBlogjs As New StringBuilder
			sbBlogjs.Append(Environment.NewLine)
			sbBlogjs.Append("function CheckBlogForillegalChar() {" & Environment.NewLine)
			sbBlogjs.Append("   var bret = true;" & Environment.NewLine)
			sbBlogjs.Append("   for (var j = 0; j < arrInputValue.length; j++)" & Environment.NewLine)
			sbBlogjs.Append("   {" & Environment.NewLine)
			sbBlogjs.Append("       var val = Trim(arrInputValue[j]);" & Environment.NewLine)
			sbBlogjs.Append("       if ((val.indexOf("";"") > -1) || (val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
			sbBlogjs.Append("       {" & Environment.NewLine)
			sbBlogjs.Append("           alert(""" & Me._MessageHelper.GetMessage("alert subject name") & " (';', '\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')"");" & Environment.NewLine)
			sbBlogjs.Append("           bret = false;" & Environment.NewLine)
			sbBlogjs.Append("       }" & Environment.NewLine)
			sbBlogjs.Append("       else if (val.length == 0) {" & Environment.NewLine)
			sbBlogjs.Append("           alert(""" & Me._MessageHelper.GetMessage("alert blank subject name") & """);" & Environment.NewLine)
			sbBlogjs.Append("           bret = false;" & Environment.NewLine)
			sbBlogjs.Append("       }" & Environment.NewLine)
			sbBlogjs.Append("   }" & Environment.NewLine)
			sbBlogjs.Append("   if (bret == true) //go on to normal code path" & Environment.NewLine)
			sbBlogjs.Append("   {" & Environment.NewLine)
			sbBlogjs.Append("       bret = CheckFolderParameters('edit');" & Environment.NewLine)
			sbBlogjs.Append("   }" & Environment.NewLine)
			sbBlogjs.Append("   return bret;" & Environment.NewLine)
			sbBlogjs.Append("}" & Environment.NewLine)
			ltr_blog_js.Text = sbBlogjs.ToString()
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath & "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt update button text (folder)"), _MessageHelper.GetMessage("btn update"), "onclick=""return SubmitForm('frmContent', 'CheckBlogForillegalChar()');"""))
		Else
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath & "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt update button text (folder)"), _MessageHelper.GetMessage("btn update"), "onclick=""return SubmitForm('frmContent', 'CheckFolderParameters(\'edit\')');"""))
		End If
		If _ShowPane.Length > 0 And isBlog Then
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
		Else
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?action=ViewFolder&id=" & _Id & "&LangType=" & _ContentLanguage, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
		End If
		result.Append("<td>")
		Select Case _FolderType
			Case Common.EkEnumeration.FolderType.Blog
				result.Append(_StyleHelper.GetHelpButton("blog_viewfolder"))
			Case Common.EkEnumeration.FolderType.Calendar
				result.Append(_StyleHelper.GetHelpButton("calendar_" & _PageAction))
			Case Else
				result.Append(_StyleHelper.GetHelpButton(_PageAction))
		End Select
		result.Append("</td>")
		result.Append("</tr></table>")
		htmToolBar.InnerHtml = result.ToString
	End Sub
	Private Sub DisplayMetadataInfo()
		' Show Custom-Field folder assignments:
		lit_vf_customfieldassingments.Text = _CustomFieldsApi.GetEditableCustomFieldAssignments(_Id, True)
	End Sub
	Private Sub DisplaySubscriptionInfo()
		Dim strEnabled As String = " "
		Dim i As Integer = 0
		Dim findindex As Integer
		Dim arrSubscribed As Array
		Dim strNotifyA As String = ""
		Dim strNotifyI As String = ""
		Dim strNotifyN As String = ""
		Dim strNotifyBase As String = ""
		Dim intInheritFrom As Long
		Dim emailfrom_list As EmailFromData()
		Dim y As Integer = 0
		Dim optout_list As EmailMessageData()
		Dim defaultmessage_list As EmailMessageData()
		Dim unsubscribe_list As EmailMessageData()
		Dim settings_list As SettingsData
		Dim m_refSiteAPI As New SiteAPI
		Dim restoreAvailable As Boolean = True

		_SubscriptionDataList = _ContentApi.GetAllActiveSubscriptions()	'then get folder
		emailfrom_list = _ContentApi.GetAllEmailFrom()
		optout_list = _ContentApi.GetSubscriptionMessagesForType(Common.EkEnumeration.EmailMessageTypes.OptOut)
		unsubscribe_list = _ContentApi.GetSubscriptionMessagesForType(Common.EkEnumeration.EmailMessageTypes.Unsubscribe)
		defaultmessage_list = _ContentApi.GetSubscriptionMessagesForType(Common.EkEnumeration.EmailMessageTypes.DefaultMessage)
		settings_list = m_refSiteAPI.GetSiteVariables()

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

		lit_vf_subscription_properties.Text += ("function CheckBaseNotifyValue(objValue){" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("   if (objValue == document.getElementById('base_notify_option').value)" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("       document.getElementById('web_alert_restore_inherit_checkbox').checked = false;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("   else" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("       document.getElementById('web_alert_restore_inherit_checkbox').checked = true;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("}" & Environment.NewLine)

		lit_vf_subscription_properties.Text += ("function breakWebAlertInheritance(obj){" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("   if(!obj.checked){" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("       if(confirm(""" & _MessageHelper.GetMessage("js: confirm break inheritance") & """)){" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("           enableSubCheckboxes(true);" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("       } else {" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("           obj.checked = !obj.checked;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("           return false;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("       }" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("   } else {" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("       enableSubCheckboxes(true);" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("   }" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("}" & Environment.NewLine)

		lit_vf_subscription_properties.Text += ("function enableSubCheckboxes(enableFlag) {" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("    var idx, masterBtn, tableObj, qtyElements, displayUseContentBtns;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("    tableObj = document.getElementById('therows');" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("    tableObj = tableObj.getElementsByTagName('input');" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("    masterBtn = document.getElementById('web_alert_inherit_checkbox');" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("    if (enableFlag && validateObject(masterBtn)){" & Environment.NewLine)
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
		lit_vf_subscription_properties.Text += ("        document.forms.frmContent.web_alert_restore_inherit_checkbox.checked = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.forms.frmContent.web_alert_restore_inherit_checkbox.disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.forms.frmContent.notify_option[0].disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.forms.frmContent.notify_option[1].disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.forms.frmContent.notify_option[2].disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.getElementById('use_message_button').disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.getElementById('use_summary_button').disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.getElementById('use_content_button').disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.getElementById('use_contentlink_button').disabled = !enableFlag;" & Environment.NewLine)
		'lit_vf_subscription_properties.Text += ("        document.getElementById('notify_url').disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.getElementById('notify_emailfrom').disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.getElementById('notify_optoutid').disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.getElementById('notify_messageid').disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.getElementById('notify_unsubscribeid').disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        document.getElementById('notify_subject').disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        $ektron('.selectContent').css('display', displayUseContentBtns);" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("        $ektron('.useCurrent').css('display', displayUseContentBtns);" & Environment.NewLine)
		'lit_vf_subscription_properties.Text += ("        document.getElementById('notify_weblocation').disabled = !enableFlag;" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("    }" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("}" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("function validateObject(obj) {" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("     return ((obj != null) &&" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("         ((typeof(obj)).toLowerCase() != 'undefined') &&" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("         ((typeof(obj)).toLowerCase() != 'null'))" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("}" & Environment.NewLine)
		lit_vf_subscription_properties.Text += ("function valAndSaveCSubAssignments() {" & Environment.NewLine)
		If (Not (_SubscriptionDataList Is Nothing)) And (Not ((emailfrom_list Is Nothing) Or (defaultmessage_list Is Nothing) Or (unsubscribe_list Is Nothing) Or (optout_list Is Nothing) _
		 Or ((settings_list.AsynchronousLocation = "")))) Then
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

		If (emailfrom_list Is Nothing) Or (defaultmessage_list Is Nothing) Or (unsubscribe_list Is Nothing) Or (optout_list Is Nothing) Or (_SubscriptionDataList Is Nothing) _
		  Or ((settings_list.AsynchronousLocation = "")) Then
			lit_vf_subscription_properties.Text &= ("<input type=""hidden"" name=""suppress_notification"" value=""true""/>")
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
			If (_SubscriptionDataList Is Nothing) Then
				lit_vf_subscription_properties.Text &= ("<font class=""ektronErrorText"">" & _MessageHelper.GetMessage("alt No subscriptions are enabled on the folder.") & "</font><br/>")
			End If
			If (settings_list.AsynchronousLocation = "") Then
				lit_vf_subscription_properties.Text &= ("<font class=""ektronErrorText"">" & _MessageHelper.GetMessage("alt The location to the Asynchronous Data Processor is not specified.") & "</font>")
			End If
			Exit Sub
		End If

		intInheritFrom = _ContentApi.GetFolderInheritedFrom(_Id)
		If intInheritFrom <> _Id Then 'do we get settings from self?
			_GlobalSubInherit = True
		Else
			_GlobalSubInherit = False
		End If
		_SubscribedDataList = _ContentApi.GetSubscriptionsForFolder(intInheritFrom)
		_SubscriptionPropertiesList = _ContentApi.GetSubscriptionPropertiesForFolder(intInheritFrom)

		If (_SubscriptionPropertiesList Is Nothing) Then
			_SubscriptionPropertiesList = New SubscriptionPropertiesData
		End If
		If _GlobalSubInherit = True Then
			If _Id = 0 Then
				strEnabled = " "
			Else
				strEnabled = " disabled=""disabled"" "
			End If
		Else
			strEnabled = " "
		End If
		Select Case _SubscriptionPropertiesList.NotificationType.GetHashCode
			Case 0
				strNotifyA = " checked=""checked"" "
				strNotifyI = ""
				strNotifyN = ""
				strNotifyBase = "Always"
			Case 1
				strNotifyA = ""
				strNotifyI = " checked=""checked"" "
				strNotifyN = ""
				strNotifyBase = "Initial"
			Case 2
				strNotifyA = ""
				strNotifyI = ""
				strNotifyN = " checked=""checked"" "
				strNotifyBase = "Never"
		End Select

		If _Id = 0 Then	 ' root folder or not inheriting
			lit_vf_subscription_properties.Text &= "<input id=""web_alert_inherit_checkbox"" type=""hidden"" name=""web_alert_inherit_checkbox"" value=""web_alert_inherit_checkbox""/>"
		ElseIf Not _GlobalSubInherit Then
			lit_vf_subscription_properties.Text &= "<input id=""web_alert_inherit_checkbox"" onclick=""breakWebAlertInheritance(this);"" type=""checkbox"" name=""web_alert_inherit_checkbox"" value=""web_alert_inherit_checkbox""/>" & _MessageHelper.GetMessage("lbl inherit parent configuration") & ""
			lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
		Else ' non root
			lit_vf_subscription_properties.Text &= "<input id=""web_alert_inherit_checkbox"" onclick=""breakWebAlertInheritance(this);"" type=""checkbox"" name=""web_alert_inherit_checkbox"" value=""web_alert_inherit_checkbox"" checked=""checked""/>" & _MessageHelper.GetMessage("lbl inherit parent configuration") & ""
			lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
			restoreAvailable = False
		End If

		lit_vf_subscription_properties.Text &= "&nbsp;&nbsp;<input id=""web_alert_restore_inherit_checkbox"" type=""checkbox"" name=""web_alert_restore_inherit_checkbox"" value=""web_alert_restore_inherit_checkbox"" " & IIf(Not restoreAvailable, "disabled=""disabled""", "") & "/>" & _MessageHelper.GetMessage("alt restore web alert") & ""
		lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"

		lit_vf_subscription_properties.Text &= "<table class=""ektronGrid"">"
		lit_vf_subscription_properties.Text &= "<tr>"
		lit_vf_subscription_properties.Text &= "<td class=""label"">"
		lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert opt") & ":"
		lit_vf_subscription_properties.Text &= "</td>"
		lit_vf_subscription_properties.Text &= "<td class=""value""><input type=""hidden"" id=""base_notify_option"" name=""base_notify_option"" value=""" & strNotifyBase & """/>"
		lit_vf_subscription_properties.Text &= "<input type=""radio"" value=""Always"" name=""notify_option"" " & strNotifyA & " " & strEnabled & " onclick=""CheckBaseNotifyValue(this.value);"" />" & _MessageHelper.GetMessage("lbl web alert notify always")
		lit_vf_subscription_properties.Text &= "<br />"
		lit_vf_subscription_properties.Text &= "<input type=""radio"" value=""Initial"" name=""notify_option""" & strNotifyI & " " & strEnabled & " onclick=""CheckBaseNotifyValue(this.value);"" />" & _MessageHelper.GetMessage("lbl web alert notify initial")
		lit_vf_subscription_properties.Text &= "<br />"
		lit_vf_subscription_properties.Text &= "<input type=""radio"" value=""Never"" name=""notify_option""" & strNotifyN & " " & strEnabled & " onclick=""CheckBaseNotifyValue(this.value);"" />" & _MessageHelper.GetMessage("lbl web alert notify never")
		lit_vf_subscription_properties.Text &= "</td>"
		lit_vf_subscription_properties.Text &= "</tr>"

		lit_vf_subscription_properties.Text &= "<tr>"
		lit_vf_subscription_properties.Text &= "<td class=""label"">"
		lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert subject") & ":"
		lit_vf_subscription_properties.Text &= "</td>"
		lit_vf_subscription_properties.Text &= "<td class=""value"">"
		If _SubscriptionPropertiesList.Subject <> "" Then
			lit_vf_subscription_properties.Text &= "<input type=""text"" maxlength=""255"" size=""65"" value=""" & _SubscriptionPropertiesList.Subject & """ name=""notify_subject"" id=""notify_subject"" " & strEnabled & " onkeypress=""return CheckKeyValue(event, '34,13');""/>&"
		Else
			lit_vf_subscription_properties.Text &= "<input type=""text"" maxlength=""255"" size=""65"" value="""" name=""notify_subject"" id=""notify_subject""  " & strEnabled & " onkeypress=""return CheckKeyValue(event, '34,13');""/>"
		End If
		lit_vf_subscription_properties.Text &= "</td>"
		lit_vf_subscription_properties.Text &= "</tr>"

		'lit_vf_subscription_properties.Text &= "Notification Base URL:"
		'If subscription_properties_list.URL <> "" Then
		'    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" id=""notify_url"" " & strEnabled & " value=""" & subscription_properties_list.URL & """/>/<br /><br />"
		'Else
		'    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" id=""notify_url"" " & strEnabled & " value=""" & Request.ServerVariables("HTTP_HOST") & """/>/<br /><br />"
		'End If

		lit_vf_subscription_properties.Text &= "<tr>"
		lit_vf_subscription_properties.Text &= "<td class=""label"">"
		lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert emailfrom address") & ":"
		lit_vf_subscription_properties.Text &= "</td>"
		lit_vf_subscription_properties.Text &= "<td class=""value"">"

		lit_vf_subscription_properties.Text &= "<select name=""notify_emailfrom"" id=""notify_emailfrom"" " & strEnabled & ">:"
		If (Not emailfrom_list Is Nothing) AndAlso emailfrom_list.Length > 0 Then
			For y = 0 To emailfrom_list.Length - 1
				If emailfrom_list(y).Email = _SubscriptionPropertiesList.EmailFrom Then
					lit_vf_subscription_properties.Text &= "<option value=""" & emailfrom_list(y).Email & """ SELECTED>" & emailfrom_list(y).Email & "</option>"
				Else
					lit_vf_subscription_properties.Text &= "<option value=""" & emailfrom_list(y).Email & """>" & emailfrom_list(y).Email & "</option>"
				End If
			Next
		End If
		lit_vf_subscription_properties.Text &= "</select>"
		lit_vf_subscription_properties.Text &= "</td>"
		lit_vf_subscription_properties.Text &= "</tr>"

		'lit_vf_subscription_properties.Text &= "Notification File Location:"
		'If subscription_properties_list.WebLocation <> "" Then
		'lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""" & subscription_properties_list.WebLocation & """ name=""notify_weblocation"" id=""notify_weblocation"" " & strEnabled & "/>/<br /><br />"
		'Else
		'    lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""subscriptions"" name=""notify_weblocation"" " & strEnabled & "/>/<br /><br />"
		'End If

		lit_vf_subscription_properties.Text &= "<tr>"
		lit_vf_subscription_properties.Text &= "<td class=""label"">"
		lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert contents") & ":"
		'lit_vf_subscription_properties.Text &= "<br />"
		lit_vf_subscription_properties.Text &= "<img src=""" & _ContentApi.AppPath & "images/UI/Icons/preview.png"" alt=""Preview Web Alert Message"" title=""Preview Web Alert Message"" onclick="" PreviewWebAlert(); return false;"" />"
		lit_vf_subscription_properties.Text &= "</td>"
		lit_vf_subscription_properties.Text &= "<td class=""value"">"
		lit_vf_subscription_properties.Text += "<input id=""use_optout_button"" type=""checkbox"" checked=""checked"" name=""use_optout_button"" disabled=""disabled""/>" & _MessageHelper.GetMessage("lbl optout message") & "&nbsp;&nbsp;"


		lit_vf_subscription_properties.Text &= "<select " & strEnabled & " name=""notify_optoutid"" id=""notify_optoutid"">"
		If (Not optout_list Is Nothing) AndAlso optout_list.Length > 0 Then
			For y = 0 To optout_list.Length - 1
				If optout_list(y).Id = _SubscriptionPropertiesList.OptOutID Then
					lit_vf_subscription_properties.Text &= ("<option value=""" & optout_list(y).Id & """ SELECTED>" & optout_list(y).Title & "</option>")
				Else
					lit_vf_subscription_properties.Text &= ("<option value=""" & optout_list(y).Id & """>" & optout_list(y).Title & "</option>")
				End If
			Next
		End If
		lit_vf_subscription_properties.Text += "</select>"

		lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
		If _SubscriptionPropertiesList.DefaultMessageID > 0 Then
			lit_vf_subscription_properties.Text += ("<input id=""use_message_button"" type=""checkbox"" checked=""checked"" name=""use_message_button"" " & strEnabled & " />" & _MessageHelper.GetMessage("lbl use default message")) & "&nbsp;&nbsp;"
		Else
			lit_vf_subscription_properties.Text += ("<input id=""use_message_button"" type=""checkbox"" name=""use_message_button"" " & strEnabled & " />" & _MessageHelper.GetMessage("lbl use default message")) & "&nbsp;&nbsp;"
		End If


		lit_vf_subscription_properties.Text &= "<select " & strEnabled & " name=""notify_messageid"" id=""notify_messageid"">"
		If (Not defaultmessage_list Is Nothing) AndAlso defaultmessage_list.Length > 0 Then
			For y = 0 To defaultmessage_list.Length - 1
				If defaultmessage_list(y).Id = _SubscriptionPropertiesList.DefaultMessageID Then
					lit_vf_subscription_properties.Text += "<option value=""" & defaultmessage_list(y).Id & """ SELECTED>" & defaultmessage_list(y).Title & "</option>"
				Else
					lit_vf_subscription_properties.Text += "<option value=""" & defaultmessage_list(y).Id & """>" & defaultmessage_list(y).Title & "</option>"
				End If
			Next
		End If
		lit_vf_subscription_properties.Text += "</select>"

		lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
		If _SubscriptionPropertiesList.SummaryID > 0 Then
			lit_vf_subscription_properties.Text += "<input id=""use_summary_button"" type=""checkbox"" name=""use_summary_button"" checked=""checked"" " & strEnabled & "/>" & _MessageHelper.GetMessage("lbl use summary message")
		Else
			lit_vf_subscription_properties.Text += "<input id=""use_summary_button"" type=""checkbox"" name=""use_summary_button"" " & strEnabled & "/>" & _MessageHelper.GetMessage("lbl use summary message")
		End If
		lit_vf_subscription_properties.Text += "<br />"
		If _SubscriptionPropertiesList.ContentID = -1 Then
			lit_vf_subscription_properties.Text += "<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" checked=""checked"" " & strEnabled & "/>" & _MessageHelper.GetMessage("lbl use content message") & "&nbsp;&nbsp;"
			lit_vf_subscription_properties.Text += "<input type=""text"" id=""titlename"" name=""titlename"" value=""[[use current]]"" " & strEnabled & " size=""65"" disabled=""disabled""/>"
			lit_vf_subscription_properties.Text += "<a href=""#"" class=""button buttonInline greenHover selectContent"" onclick="" QuickLinkSelectBase(" & _Id.ToString() & ",'frmContent','titlename',0,0,0,0) ;return false;""/>" & _MessageHelper.GetMessage("lbl use content select") & "</a><a href=""#"" class=""button buttonInline  blueHover useCurrent"" onclick="" SetMessageContenttoDefault();return false;"">Use Current</a>"
			lit_vf_subscription_properties.Text += "&nbsp;&nbsp;"
			lit_vf_subscription_properties.Text += "<input type=""hidden"" maxlength=""20"" id=""frm_content_id"" name=""frm_content_id"" value=""-1""/>"
			lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_content_langid""/>"
			lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_qlink""/>"

		ElseIf _SubscriptionPropertiesList.ContentID > 0 Then
			lit_vf_subscription_properties.Text += "<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" checked=""checked"" " & strEnabled & "/>" & _MessageHelper.GetMessage("lbl use content message") & "&nbsp;&nbsp;"
			lit_vf_subscription_properties.Text += "<input type=""text"" id=""titlename"" name=""titlename"" value=""" & _SubscriptionPropertiesList.UseContentTitle.ToString() & """ " & strEnabled & " size=""65"" disabled=""disabled""/>"
			lit_vf_subscription_properties.Text += "<a href=""#"" class=""button buttonInline greenHover selectContent"" onclick="" QuickLinkSelectBase(" & _Id.ToString() & ",'frmContent','titlename',0,0,0,0) ;return false;"">" & _MessageHelper.GetMessage("lbl use content select") & "</a><a href=""#"" class=""button buttonInline  blueHover useCurrent"" onclick="" SetMessageContenttoDefault();return false;"">Use Current</a>"
			lit_vf_subscription_properties.Text += "<input type=""hidden"" maxlength=""20"" id=""frm_content_id"" name=""frm_content_id"" value=""" & _SubscriptionPropertiesList.ContentID.ToString() & """/>"
			lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_content_langid""/>"
			lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_qlink""/>"

		Else
			lit_vf_subscription_properties.Text += "<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" " & strEnabled & "/>" & _MessageHelper.GetMessage("lbl use content message") & "&nbsp;&nbsp;"
			lit_vf_subscription_properties.Text += "<input type=""text"" id=""titlename"" name=""titlename"" onkeydown=""return false"" value="""" " & strEnabled & " size=""65"" disabled=""disabled""/>"
			lit_vf_subscription_properties.Text += "<a href=""#"" class=""button buttonInline greenHover selectContent"" onclick=""QuickLinkSelectBase(" & _Id.ToString() & ",'frmContent','titlename',0,0,0,0) ;return false;"">" & _MessageHelper.GetMessage("lbl use content select") & "</a><a href=""#"" class=""button buttonInline  blueHover useCurrent"" onclick=""SetMessageContenttoDefault();return false;"">Use Current</a>"
			lit_vf_subscription_properties.Text += "<input type=""hidden"" maxlength=""20"" id=""frm_content_id"" name=""frm_content_id"" value=""0"" />"
			lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_content_langid""/>"
			lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_qlink""/>"
		End If

		lit_vf_subscription_properties.Text += "<br />"

		If _SubscriptionPropertiesList.UseContentLink > 0 Then
			lit_vf_subscription_properties.Text += "<input id=""use_contentlink_button"" type=""checkbox"" name=""use_contentlink_button"" checked=""checked"" " & strEnabled & "/>Use Content Link"
		Else
			lit_vf_subscription_properties.Text += "<input id=""use_contentlink_button"" type=""checkbox"" name=""use_contentlink_button"" " & strEnabled & "/>Use Content Link"
		End If

		lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
		lit_vf_subscription_properties.Text += "<input id=""use_unsubscribe_button"" type=""checkbox"" checked=""checked"" name=""use_unsubscribe_button"" disabled=""disabled""/>" & _MessageHelper.GetMessage("lbl unsubscribe message") & "&nbsp;&nbsp;"


		lit_vf_subscription_properties.Text &= "<select " & strEnabled & " name=""notify_unsubscribeid"" id=""notify_unsubscribeid"">"
		If (Not unsubscribe_list Is Nothing) AndAlso unsubscribe_list.Length > 0 Then
			For y = 0 To unsubscribe_list.Length - 1
				If unsubscribe_list(y).Id = _SubscriptionPropertiesList.UnsubscribeID Then
					lit_vf_subscription_properties.Text += "<option value=""" & unsubscribe_list(y).Id & """ selected>" & unsubscribe_list(y).Title & "</option>"
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
		lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl avail web alert")
		lit_vf_subscription_properties.Text &= "</div>"

		If Not (_SubscriptionDataList Is Nothing) Then
			lit_vf_subscription_properties.Text += "<table class=""ektronGrid"" id=""cfld_subscription_assignment"" width=""100%"">"
			lit_vf_subscription_properties.Text &= "<tbody id=""therows"">"
			lit_vf_subscription_properties.Text += "<tr class=""title-header"">"
			lit_vf_subscription_properties.Text &= "<th width=""50"">" & _MessageHelper.GetMessage("lbl assigned") & "</th>"
			lit_vf_subscription_properties.Text &= "<th>" & _MessageHelper.GetMessage("lbl name") & "</th>"
			lit_vf_subscription_properties.Text &= "</tr>"
			If Not (_SubscribedDataList Is Nothing) Then
				arrSubscribed = Array.CreateInstance(GetType(Long), _SubscribedDataList.Length)
				For i = 0 To _SubscribedDataList.Length - 1
					arrSubscribed.SetValue(_SubscribedDataList(i).Id, i)
				Next
				If (Not arrSubscribed Is Nothing) Then
					If arrSubscribed.Length > 0 Then
						Array.Sort(arrSubscribed)
					End If
				End If
			Else
				arrSubscribed = Nothing
			End If
			i = 0

			For i = 0 To _SubscriptionDataList.Length - 1
				findindex = -1
				If ((Not _SubscribedDataList Is Nothing) AndAlso (Not arrSubscribed Is Nothing)) Then
					findindex = Array.BinarySearch(arrSubscribed, _SubscriptionDataList(i).Id)
				End If
				lit_vf_subscription_properties.Text &= "<tr>"

				If findindex < 0 Then
                    lit_vf_subscription_properties.Text += "<td class=""center"" width=""10%""><input type=""checkbox"" name=""WebAlert_" & _SubscriptionDataList(i).Id & """  id=""WebAlert_" & _SubscriptionDataList(i).Id & """ " & strEnabled & "/></td>"
				Else
                    lit_vf_subscription_properties.Text += "<td class=""center"" width=""10%""><input type=""checkbox"" name=""WebAlert_" & _SubscriptionDataList(i).Id & """  id=""WebAlert_" & _SubscriptionDataList(i).Id & """ checked=""checked"" " & strEnabled & "/></td>"
				End If
				lit_vf_subscription_properties.Text += "<td>" & _SubscriptionDataList(i).Name & "</td>"
				lit_vf_subscription_properties.Text += "</tr>"
			Next
			lit_vf_subscription_properties.Text += "</tbody></table>"
		Else
			lit_vf_subscription_properties.Text += "Nothing available."
		End If
		lit_vf_subscription_properties.Text &= "<input type=""hidden"" name=""content_sub_assignments"" id=""content_sub_assignments"" value=""""/>"
	End Sub
#End Region

#Region "Catalog"
	Private Sub Display_EditCatalog()
		Dim i As Integer = 0

		ltInheritSitemapPath.Text = _MessageHelper.GetMessage("lbl Inherit Parent Configuration")

		ltrTypes.Text = _MessageHelper.GetMessage("lbl product types")

		_PermissionData = _ContentApi.LoadPermissions(_Id, "folder")
		_FolderData = _ContentApi.GetFolderById(_Id, True, True)
		_FolderType = _FolderData.FolderType

		EditCatalogToolBar()
		oldfolderdescription.Value = Server.HtmlDecode(_FolderData.Description)
		folderdescription.Value = Server.HtmlDecode(_FolderData.Description)
		folder_id.Value = _FolderData.Id

		phFolderProperties1.Visible = True
        lit_ef_folder.Text = "<input type=""text"" maxlength=""100"" size=""75"" value=""" & _FolderData.Name & """ name=""foldername""/><input type=""hidden"" value="""" name=""oldfoldername"" id=""oldfoldername"" />"
		If ((_FolderData.StyleSheetInherited) And (_FolderData.StyleSheet <> "")) Then
			lit_ef_ss.Text = "" & _ContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""" & 75 - _ContentApi.SitePath.Length & """ value="""" name=""stylesheet""/>"
			lit_ef_ss.Text += "<br />" & _MessageHelper.GetMessage("inherited style sheet msg") & _ContentApi.SitePath & _FolderData.StyleSheet
		Else
			lit_ef_ss.Text = "<tr><td>" & _ContentApi.SitePath & "</td><td><input type=""text"" maxlength=""255"" size=""" & 75 - _ContentApi.SitePath.Length & """ value=""" & _FolderData.StyleSheet & """ name=""stylesheet""/></td></tr>"
		End If
		lit_ef_templatedata.Text = "<input type=""hidden"" maxlength=""255"" size=""" & 75 - _ContentApi.SitePath.Length & """ value="""" name=""templatefilename"" id=""templatefilename""/>"

		DrawContentTemplatesTable()
		DrawFlaggingOptions()

		Dim request_info As Ektron.Cms.Common.EkRequestInformation = _ContentApi.RequestInformationRef

		' handle dynamic replication settings
		If request_info.EnableReplication Then
			Dim bShowReplicationMethod As Boolean = True
			If bShowReplicationMethod Then
				Dim schk As String = ""
				If (_FolderData.ReplicationMethod = 1) Then
					schk = " checked"
				End If
				ReplicationMethod.Text = _MessageHelper.GetMessage("lbl folderdynreplication")
				ReplicationMethod.Text += "<input type=""checkbox"" name=""EnableReplication"" id=""EnableReplication"" value=""1""" & schk & " ><label for=""EnableReplication""/>" & _MessageHelper.GetMessage("replicate folder contents") & "</label>"
			Else
				' if we're not showing it, it means replication is enabled because we're under a parent community folder
				ReplicationMethod.Text = "<input type=""hidden"" name=""EnableReplication"" value=""1"" />"
			End If
		End If
		js_ef_focus.Text = "Ektron.ready(function() { document.forms.frmContent.foldername.focus();" & Environment.NewLine
		js_ef_focus.Text += "   if( $ektron('#web_alert_inherit_checkbox').length > 0 ){" & Environment.NewLine
		js_ef_focus.Text += "       if( $ektron('#web_alert_inherit_checkbox')[0].checked ){" & Environment.NewLine
		js_ef_focus.Text += "           $ektron('.selectContent').css('display', 'none');" & Environment.NewLine
		js_ef_focus.Text += "           $ektron('.useCurrent').css('display', 'none');" & Environment.NewLine
		js_ef_focus.Text += "       } " & Environment.NewLine
		js_ef_focus.Text += "    } " & Environment.NewLine
		js_ef_focus.Text += "});" & Environment.NewLine
		DrawFolderTaxonomyTable()
		DisplaySitemapPath()
		DisplayCatalogMetadataInfo()
		DisplaySubscriptionInfo()
		DrawProductTypesTable()
	End Sub
	Private Sub EditCatalogToolBar()
		Dim result As New System.Text.StringBuilder
		txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("edit properties for folder msg") & " """ & _FolderData.Name & """")
		result.Append("<table><tr>")
		Dim isBlog As Boolean = _FolderType = 1
		If isBlog Then
			Dim sbBlogjs As New StringBuilder
			sbBlogjs.Append(Environment.NewLine)
			sbBlogjs.Append("function CheckBlogForillegalChar() {" & Environment.NewLine)
			sbBlogjs.Append("   var bret = true;" & Environment.NewLine)
			sbBlogjs.Append("   for (var j = 0; j < arrInputValue.length; j++)" & Environment.NewLine)
			sbBlogjs.Append("   {" & Environment.NewLine)
			sbBlogjs.Append("       var val = Trim(arrInputValue[j]);" & Environment.NewLine)
			sbBlogjs.Append("       if ((val.indexOf("";"") > -1) || (val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
			sbBlogjs.Append("       {" & Environment.NewLine)
			sbBlogjs.Append("           alert(""" & Me._MessageHelper.GetMessage("alert subject name") & " (';', '\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')"");" & Environment.NewLine)
			sbBlogjs.Append("           bret = false;" & Environment.NewLine)
			sbBlogjs.Append("       }" & Environment.NewLine)
			sbBlogjs.Append("       else if (val.length == 0) {" & Environment.NewLine)
			sbBlogjs.Append("           alert(""" & Me._MessageHelper.GetMessage("alert blank subject name") & """);" & Environment.NewLine)
			sbBlogjs.Append("           bret = false;" & Environment.NewLine)
			sbBlogjs.Append("       }" & Environment.NewLine)
			sbBlogjs.Append("   }" & Environment.NewLine)
			sbBlogjs.Append("   if (bret == true) //go on to normal code path" & Environment.NewLine)
			sbBlogjs.Append("   {" & Environment.NewLine)
			sbBlogjs.Append("       bret = CheckFolderParameters('edit');" & Environment.NewLine)
			sbBlogjs.Append("   }" & Environment.NewLine)
			sbBlogjs.Append("   return bret;" & Environment.NewLine)
			sbBlogjs.Append("}" & Environment.NewLine)
			ltr_blog_js.Text = sbBlogjs.ToString()
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath & "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt update button text (catalog)"), _MessageHelper.GetMessage("btn update"), "onclick=""return SubmitForm('frmContent', 'CheckBlogForillegalChar()');"""))
		Else
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath & "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt update button text (catalog)"), _MessageHelper.GetMessage("btn update"), "onclick=""return SubmitForm('frmContent', 'CheckFolderParameters(\'edit\')');"""))
		End If
		If _ShowPane.Length > 0 And Me._FolderType = 1 Then
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
		Else
			result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?action=ViewFolder&id=" & _Id & "&LangType=" & _ContentLanguage, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
		End If
		result.Append("<td>")
		result.Append(_StyleHelper.GetHelpButton("catalogedit"))
		result.Append("</td>")
		result.Append("</tr></table>")
		htmToolBar.InnerHtml = result.ToString
	End Sub
	Private Sub DisplayCatalogMetadataInfo()
		' Show Custom-Field folder assignments:
		lit_vf_customfieldassingments.Text = "<br/>" & _CustomFieldsApi.GetEditableCustomFieldAssignments(_Id, True, Common.EkEnumeration.FolderType.Catalog)
	End Sub
	Private Function DrawProductTypesBreaker(ByVal checked As Boolean, ByVal IsParentCatalog As Boolean) As String
		If Not IsParentCatalog Then
			Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleProductTypesInherit('TypeBreak', this)"" disabled autocomplete='off' />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
		End If
		If (checked) Then
			Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleProductTypesInherit('TypeBreak', this)"" checked autocomplete='off' />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
		Else
			Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleProductTypesInherit('TypeBreak', this)"" autocomplete='off' />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
		End If

	End Function

	Private Function DrawProductTypesHeader() As String
		Dim str As New StringBuilder()
		str.Append("<table width=""100%"" class=""ektronGrid""><tbody id=""contentTypeTable"">")
		str.Append("    <tr class=""title-header"">")
		str.Append("        <td width=""10%"" class=""center"">")
		str.Append(_MessageHelper.GetMessage("lbl default"))
		str.Append("        </td>")
		str.Append("        <td width=""70%"">")
		str.Append(_MessageHelper.GetMessage("lbl prod type"))
		str.Append("        </td>")
		str.Append("        <td width=""10%"" class=""center"" colspan=""2"">")
		str.Append(_MessageHelper.GetMessage("lbl action"))
		str.Append("        </td>")
		str.Append("    </tr>")
		Return str.ToString()
	End Function

	Private Function DrawProductTypesEntry(ByVal row_id As Integer, ByVal name As String, ByVal xml_id As Long, ByVal isDefault As Boolean, ByVal isEnabled As Boolean) As String
		Dim str As New StringBuilder()
		Dim k As Integer = 0

		str.Append("<tr id=""row_" & xml_id & """>")

		str.Append("<td class=""center"" width=""10%"">")
		If (Me._FolderData.Id = 0) Then
			isEnabled = True
		End If
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
			str.Append("<td class=""center"" width=""10%""><span class='hiddenWhenInheriting' style='display:" + IIf(isEnabled, "inline;", "none;") + "' ><a class=""button redHover minHeight buttonSearch"" href=""javascript:PreviewProductTypeByID(" & xml_id & ")"">" & _MessageHelper.GetMessage("lbl view") & "</a></span></td>")
		Else
			str.Append("<td class=""center"" width=""10%"">&nbsp;</td>")
		End If

		str.Append("<td class=""right"" width=""10%""><span class='hiddenWhenInheriting' style='display:" + IIf(isEnabled, "inline;", "none;") + "' ><a class=""button redHover minHeight buttonRemove"" href=""javascript:RemoveContentType(" & xml_id & ", '" & name & "')"">" & _MessageHelper.GetMessage("btn remove") & "</a></span></td>")
		str.Append("</tr>")

		Return str.ToString()
	End Function
	Private Sub DrawProductTypesTable()
		_ProductType = New ProductType(_ContentApi.RequestInformationRef)

		Dim prod_type_list As New List(Of Commerce.ProductTypeData)
		Dim criteria As New Criteria(Of ProductTypeProperty)

		prod_type_list = _ProductType.GetList(criteria)

		Dim active_prod_list As New List(Of Commerce.ProductTypeData)
		active_prod_list = _ProductType.GetFolderProductTypeList(_FolderData.Id)
		Dim addNew As New Collection()
		Dim k As Integer = 0
		Dim row_id As Integer = 0

		Dim smartFormsRequired As Boolean = True

		Dim broken As Boolean = False
		If (active_prod_list.Count > 0) Then
			broken = True
		End If

		Dim isParentCatalog As Boolean = (_ContentApi.EkContentRef.GetFolderType(_FolderData.ParentId) = Common.EkEnumeration.FolderType.Catalog)
		Dim isInheriting As Boolean = (isParentCatalog AndAlso IsInheritingXmlMultiConfig()) ' folder_data.XmlInherited
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
		str.Append("<table width=""100%""><tbody>")
		str.Append("<tr><td width=""90%"">")
		str.Append("<br /><select name=""addContentType"" id=""addContentType"" " + IIf(isEnabled Or _FolderData.Id = 0, "", " disabled ") + ">")

		str.Append("<option value=""-1"">" & "[" & _MessageHelper.GetMessage("lbl select prod type") & "]" & "</option>")
		Dim row As Collection
		For Each row In addNew
			str.Append("<option value=""" & row("xml_id") & """>" & row("xml_name") & "</option>")
		Next

		str.Append("</select>")
		str.Append("<span class='hiddenWhenInheriting' style='display:" + IIf(isEnabled, "inline; padding: .25em;", "none;") + "' ><a href=""#"" onclick=""PreviewSelectedProductType('" & _ContentApi.SitePath & "', 800,600);return false;""><img src=""" & _ContentApi.AppPath & "images/UI/Icons/preview.png" & """ alt=" & _MessageHelper.GetMessage("lbl Preview Smart Form") & " title=" & _MessageHelper.GetMessage("lbl Preview Smart Form") & "></a></span>")
		str.Append("<span class='hiddenWhenInheriting' style='display:" + IIf(isEnabled, "inline; padding: .25em;", "none;") + "' ><a href=""#"" onclick=""ActivateContentType(true);""><img src=""" & _ContentApi.AppPath & "images/ui/icons/Add.png" & """ title=" & _MessageHelper.GetMessage("lbl add link") & " alt=" & _MessageHelper.GetMessage("lbl add link") & "/></a></span></td></tr>")
		str.Append(DrawContentTypesFooter())
		If (row_id Mod 2 = 0) Then
			str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""1"" />")
		Else
			str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""0"" />")
		End If
		If (_FolderData.Id = 0) Then
			isEnabled = True
		End If
		str.Append("<div style='display:none;'>")
		If (smartFormsRequired And isEnabled) Then
			str.Append("<input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onClick=""ToggleRequireSmartForms()"" checked=""checked"" />")
		ElseIf (Not smartFormsRequired And isEnabled) Then
			str.Append("<input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onClick=""ToggleRequireSmartForms()"" />")
		ElseIf (smartFormsRequired And Not isEnabled) Then
			str.Append("<input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onClick=""ToggleRequireSmartForms()"" checked=""checked"" disabled=""disabled"" />")
		Else
			str.Append("<input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onClick=""ToggleRequireSmartForms()"" disabled=""disabled"" />")
		End If

		str.Append(_MessageHelper.GetMessage("lbl Require Smart Forms"))
		str.Append("</div>")
		ltr_vf_types.Text = str.ToString()
	End Sub
#End Region

#Region "content type selection"

    Private Function DrawContentTypesBreaker(ByVal checked As Boolean) As String
        If (_FolderData.Id = 0) Then
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TypeBreak')"" disabled />Inherit Parent Configuration"
        End If
        If (checked) Then
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TypeBreak')"" checked />Inherit Parent Configuration"
        Else
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TypeBreak')"" />Inherit Parent Configuration"
        End If

    End Function

    Private Function DrawContentTypesHeader() As String
        Dim str As New StringBuilder()
        str.Append("<table width=""100%"" class=""ektronGrid""><tbody id=""contentTypeTable"">")
        str.Append("<tr class=""title-header"">")
        str.Append("<td width=""10%"" class=""center"">")
        str.Append(_MessageHelper.GetMessage("lbl default"))
        str.Append("</td>")
        str.Append("<td width=""70%"">")
        str.Append(_MessageHelper.GetMessage("lbl Smart Form"))
        str.Append("</td>")
        str.Append("<td width=""20%"" class=""center"" colspan=""2"">")
        str.Append(_MessageHelper.GetMessage("lbl action"))
        str.Append("</td>")
        str.Append("</tr>")
        Return str.ToString()
    End Function

    Private Function DrawContentTypesEntry(ByVal row_id As Integer, ByVal name As String, ByVal xml_id As Long, ByVal isDefault As Boolean, ByVal isEnabled As Boolean) As String
        Dim str As New StringBuilder()
        Dim k As Integer = 0

        str.Append("<tr id=""row_" & xml_id & """>")
        str.Append("<td class=""center"" width=""10%"">")
        If (Me._FolderData.Id = 0) Then
            isEnabled = True
        End If
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
        str.Append("<a class=""button redHover minHeight buttonRemove"" href=""javascript:RemoveContentType(" & xml_id & ", '" & name & "')"">" & _MessageHelper.GetMessage("btn remove"))
        str.Append("</td>")
        str.Append("</tr>")

        Return str.ToString()
    End Function

    Private Function DrawContentTypesFooter() As String
        Return "</tbody></table>"
    End Function

    Private Sub DrawContentTypesTable()
        If (_FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Calendar) Then
            ltr_vf_types.Text = ""
            phContentType.Visible = False
            Return
        End If
        Dim xml_config_list As XmlConfigData()
        xml_config_list = _ContentApi.GetAllXmlConfigurations(_OrderBy)
        Dim active_xml_list As XmlConfigData()
        active_xml_list = _ContentApi.GetEnabledXmlConfigsByFolder(_FolderData.Id)
        Dim addNew As New Collection()
        Dim k As Integer = 0
        Dim row_id As Integer = 0

        Dim smartFormsRequired As Boolean = Not Utilities.IsNonFormattedContentAllowed(active_xml_list)

        Dim isEnabled As Boolean = Not IsInheritingXmlMultiConfig()

        Dim broken As Boolean = False
        If (active_xml_list.Length > 0) Then
            broken = True
        End If

        Dim isInheriting As Boolean = Not isEnabled

        Dim str As New System.Text.StringBuilder()
        str.Append(DrawContentTypesBreaker(isInheriting))
        str.Append("<div class=""ektronTopSpace""></div>")

        str.Append("<div>")
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

        str.Append(DrawContentTypesFooter())
        str.Append("</div>")

        str.Append("<div class=""ektronTopSpace""></div>")

        str.Append("<table><tbody>")
        str.Append("<tr>")
        str.Append("<td>")
        If ((Not isInheriting) Or _FolderData.Id = 0) Then
            str.Append("<select name=""addContentType"" id=""addContentType"">")
        Else
            str.Append("<select name=""addContentType"" id=""addContentType"" disabled>")
        End If

        str.Append("<option value=""-1"">" & _MessageHelper.GetMessage("select smart form") & "</option>")
        Dim row As Collection
        For Each row In addNew
            str.Append("<option value=""" & row("xml_id") & """>" & row("xml_name") & "</option>")
        Next

        str.Append("</select>")
        str.Append("</td>")
        str.Append("<td>&nbsp;</td>")
        str.Append("<td>")
        str.Append("<a href=""#"" onclick=""PreviewSelectedXmlConfig('" & _ContentApi.SitePath & "', 800,600);return false;""><img src=""" & _ContentApi.AppPath & "images/UI/Icons/preview.png" & """ alt=" & _MessageHelper.GetMessage("lbl Preview Smart Form") & " title=" & _MessageHelper.GetMessage("lbl Preview Smart Form") & "></a>")
        str.Append("</td>")
        str.Append("<td>&nbsp;</td>")
        str.Append("<td>")
        str.Append("<a href=""#"" onclick=""ActivateContentType();""><img src=""" & _ContentApi.AppPath & "images/ui/icons/add.png" & """ title=" & _MessageHelper.GetMessage("btn add") & " alt=" & _MessageHelper.GetMessage("btn add") & "/></a>")
        str.Append("</td>")
        str.Append("</tr>")
        str.Append("</tbody></table>")

        If (row_id Mod 2 = 0) Then
            str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""1"" />")
        Else
            str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""0"" />")
        End If
        If (_FolderData.Id = 0) Then
            isEnabled = True
        End If

        str.Append("<div class=""ektronTopSpace""></div>")

        If (smartFormsRequired And isEnabled) Then
            str.Append("<div><input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onClick=""ToggleRequireSmartForms()"" checked=""checked"" />")
        ElseIf (Not smartFormsRequired And isEnabled) Then
            str.Append("<div><input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onClick=""ToggleRequireSmartForms()"" />")
        ElseIf (smartFormsRequired And Not isEnabled) Then
            str.Append("<div><input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onClick=""ToggleRequireSmartForms()"" checked=""checked"" disabled=""disabled"" />")
        Else
            str.Append("<div><input type=""checkbox"" id=""requireSmartForms"" name=""requireSmartForms"" onClick=""ToggleRequireSmartForms()"" disabled=""disabled"" />")
        End If

        str.Append(_MessageHelper.GetMessage("lbl Require Smart Forms"))
        str.Append("</div>")
        ltr_vf_types.Text = str.ToString()
    End Sub

#End Region

#Region "flagging section"
    Private Sub DrawFlaggingOptions()
        'Dim str As New StringBuilder()

        'Try
        '          str.Append(m_refMsg.GetMessage("lbl flagging inherit parent config") & ": <input type=""checkbox"" id=""flagging_options_inherit_cbx"" name=""flagging_options_inherit_cbx"" " + IIf((folder_data.Id = 0), "disabled=""disabled"" ", "") + IIf(folder_data.FlagInherited And (Not (folder_data.Id = 0)), "checked=""checked"" ", "") + """ onclick=""InheritFlagingChanged()"" />" + Environment.NewLine)
        '	str.Append("<input type=""hidden"" id=""flagging_options_inherit_hf"" value=""" + IIf(folder_data.FlagInherited, "True", "False") + """ />" + Environment.NewLine)
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
        '          str.Append("                        " & m_refMsg.GetMessage("lbl assigned flags") & ": " + Environment.NewLine)
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
        '          str.Append("                        " & m_refMsg.GetMessage("lbl avail flags") & ": " + Environment.NewLine)
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
        'ddflags
        'inheritFlag.Text = m_refMsg.GetMessage("lbl flagging inherit parent config") & ": <input type=""checkbox"" id=""flagging_options_inherit_cbx"" name=""flagging_options_inherit_cbx"" " + IIf((folder_data.Id = 0), "disabled=""disabled"" ", "") + IIf(folder_data.FlagInherited And (Not (folder_data.Id = 0)), "checked=""checked"" ", "") + """ onclick=""InheritFlagingChanged('" & ddflags.ClientID & "')"" />"
        inheritFlag.Text = "<input type=""checkbox"" id=""flagging_options_inherit_cbx"" name=""flagging_options_inherit_cbx"" " + IIf((_FolderData.Id = 0), "disabled=""disabled"" ", "") + IIf(_FolderData.FlagInherited And (Not (_FolderData.Id = 0)), "checked=""checked"" ", "") + " onclick=""InheritFlagingChanged('" & ddflags.ClientID & "')"" />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration") & "" 'Fix for defect #29031
        ddflags.Items.Add(New ListItem(" -None- ", 0))
        ddflags.Items.FindByValue(0).Selected = True
        Dim flag_fdata As FolderFlagDefData = (New Community.FlaggingAPI()).GetDefaultFolderFlagDef(_FolderData.Id)
        Dim flag_data() As FlagDefData = _ContentApi.EkContentRef.GetAllFlaggingDefinitions(False)
        If (flag_data IsNot Nothing AndAlso flag_data.Length > 0) Then
            For i As Integer = 0 To flag_data.Length - 1
                ddflags.Items.Add(New ListItem(flag_data(i).Name, flag_data(i).ID))
                If (flag_fdata IsNot Nothing AndAlso flag_fdata.ID = flag_data(i).ID) Then
                    ddflags.Items.FindByValue(flag_data(i).ID).Selected = True
                    ddflags.SelectedIndex = ddflags.Items.IndexOf(ddflags.Items.FindByValue(flag_data(i).ID))
                End If
            Next
        End If
        If (_FolderData.FlagInherited) Then
            ddflags.Enabled = False
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

    '	Try
    '		flags = folder_data.FolderFlags	' flags = m_refContentApi.GetAllFolderFlagDef(folder_data.Id)

    '		For Each flag In flags
    '			If (showDefault AndAlso (flag.IsDefault)) Then
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
            End If

        Catch ex As Exception
        Finally
        End Try

    End Sub
#End Region

#Region "multi-template selection"
    Private Function DrawContentTemplatesBreaker(ByVal checked As Boolean) As String
        If _IsUserBlog Then
            Return "<input name=""TemplateTypeBreak"" id=""TemplateTypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TemplateTypeBreak')"" disabled />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration") & "<input type=""hidden"" id=""userblog"" name=""userblog"" value=""1""/>"
        ElseIf (_FolderData.Id = 0) Then
            Return "<input name=""TemplateTypeBreak"" id=""TemplateTypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TemplateTypeBreak')"" disabled />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        ElseIf (checked) Then
            Return "<input name=""TemplateTypeBreak"" id=""TemplateTypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TemplateTypeBreak')"" checked />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        Else
            Return "<input name=""TemplateTypeBreak"" id=""TemplateTypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TemplateTypeBreak')"" />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        End If
    End Function

    Private Function DrawContentTemplatesHeader() As String
        Dim str As New StringBuilder()
        str.Append("<table width=""100%"" class=""ektronGrid""><tbody id=""templateTable"">")
        str.Append("<tr class=""title-header"">")
        str.Append("<td width=""10%"" class=""center"">")
        str.Append(_MessageHelper.GetMessage("lbl default"))
        str.Append("</td>")
        str.Append("<td width=""70%"">")
        str.Append(_MessageHelper.GetMessage("lbl Page Template Name"))
        str.Append("</td>")
        str.Append("<td width=""10%"" class=""center"" colspan=""2"">")
        str.Append(_MessageHelper.GetMessage("lbl Action"))
        str.Append("</td>")
        'str.Append("<td width=""10%"" class=""center"">")
        'str.Append("&nbsp;")
        'str.Append("</td>")
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
            str.Append("<td class=""center"" width=""10%""><a href=""#"" class=""button greenHover minHeight buttonSearch"" onclick=""PreviewSpecificTemplate('" & _ContentApi.SitePath & name & "', 800,600)"">" & _MessageHelper.GetMessage("lbl View") & "</a></td>")
        End If
        str.Append("<td class=""center"" width=""10%""><a href=""#"" class=""button redHover minHeight buttonRemove""  onclick=""RemoveTemplate(" & template_id & ", '" & name.Replace("\", "\\") & "', '" & link & "')"">" & _MessageHelper.GetMessage("btn remove") & "</td>")
        str.Append("</tr>")

        Return str.ToString()
    End Function

    Private Function DrawContentTemplatesFooter() As String
        Return "</tbody></table>"
    End Function

    Private Function IsInheritingTemplateMultiConfig() As Boolean
        Dim isInheriting As Boolean = False
        If _IsUserBlog Then
            isInheriting = False
        Else
            isInheriting = _ContentApi.IsInheritingTemplateMultiConfig(_FolderData.Id)
        End If
        Return isInheriting
    End Function

    Private Function IsInheritingXmlMultiConfig() As Boolean
        Dim isInheriting As Boolean = _ContentApi.IsInheritingXmlMultiConfig(_FolderData.Id)
        Return isInheriting
    End Function

    Private Sub DrawContentTemplatesTable()
        Dim active_templates As TemplateData()
        active_templates = _ContentApi.GetEnabledTemplatesByFolder(_FolderData.Id)

        Dim template_data As TemplateData()
        If _IsUserBlog Then
            template_data = _ContentApi.GetCommunityTemplate(Common.EkEnumeration.TemplateType.User)
        Else
            template_data = _ContentApi.GetAllTemplates("TemplateFileName")
        End If

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

        Dim isInheriting As Boolean = IsInheritingTemplateMultiConfig()

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
        If ((Not isInheriting) Or _FolderData.Id = 0) Then
            str.Append("<select name=""addTemplate"" id=""addTemplate"">")
        Else
            str.Append("<select name=""addTemplate"" id=""addTemplate"" disabled>")
        End If
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
        str.Append("<a href=""#"" onclick=""PreviewTemplate('" & _ContentApi.SitePath & "', 800,600);return false;""><img src=""" & _ContentApi.AppPath & "images/UI/Icons/preview.png" & """ alt=""Preview Template"" title=""Preview Template"">")
        str.Append("</td>")
        str.Append("<td>&nbsp;</td>")
        str.Append("<td>")
        str.Append("<a href=""#"" onclick=""ActivateTemplate('" & Me._ContentApi.SitePath & "')""><img src=""" & _ContentApi.AppPath & "images/ui/icons/add.png" & """ title=" & _MessageHelper.GetMessage("btn add") & " alt=" & _MessageHelper.GetMessage("btn add") & "/></a>")
        str.Append("</td>")
        str.Append("</tr>")
        str.Append("</tbody></table>")

        If (row_id Mod 2 = 0) Then
            str.Append("<input type=""hidden"" name=""tisEven"" id=""tisEven"" value=""1"" />")
        Else
            str.Append("<input type=""hidden"" name=""tisEven"" id=""tisEven"" value=""0"" />")
        End If

        str.Append("<div class=""ektronTopSpace""></div>")
        str.Append("<a href=""#"" class=""button buttonInlineBlock greenHover buttonAdd"" onclick=""OpenAddDialog()"">Add a New Template</a>")
        'str.Append("<div id=""div3"" style=""display: none;position: block;""></div><div id=""contentidspan"" style=""display: block;position: block;""><a href=""#"" onclick=""LoadChildPage();return false;"">" & m_refMsg.GetMessage("lbl add new template") & "</a></div>")
        'str.Append("<div id=""FrameContainer"" class=""ChildPageHide"">")
        'str.Append("<iframe id=""ChildPage"" name=""ChildPage"" frameborder=""no"" marginheight=""0"" marginwidth=""0"" width=""100%"" height=""100%"" scrolling=""auto"">")
        'str.Append("</iframe>")
        'str.Append("</div>")

        template_list.Text = str.ToString()
    End Sub
#End Region

#Region "multi-xml/multi-template postback"
    Private Sub ProcessContentTemplatesPostBack()
        Dim IsInheritingTemplates As String = Request.Form("TemplateTypeBreak")
        Dim IsInheritingXml As String = Request.Form("TypeBreak")
        Dim xml_config_list As XmlConfigData()
        xml_config_list = _ContentApi.GetAllXmlConfigurations(_OrderBy)

        Dim template_data As TemplateData()
        template_data = _ContentApi.GetAllTemplates("TemplateFileName")

        Dim i As Integer = 0
        Dim xml_active_list As New Collection
        Dim template_active_list As New Collection
        Dim default_xml_id As Long = -1

        If (IsInheritingTemplates Is Nothing) Then
            For i = 0 To template_data.Length - 1
                If (Not Request.Form("tinput_" & template_data(i).Id) Is Nothing) Then
                    template_active_list.Add(template_data(i).Id, template_data(i).Id)
                End If
            Next
        End If

        If (IsInheritingXml Is Nothing) Then
            If (_FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Calendar) Then
                Dim WeSfId As Long = Ektron.Cms.Content.Calendar.WebCalendar.WebEventSmartformId
                xml_active_list.Add(WeSfId, WeSfId)
                default_xml_id = WeSfId
            Else
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
        End If

        _ContentApi.UpdateFolderMultiConfig(_FolderId, default_xml_id, template_active_list, xml_active_list)
    End Sub
    Private Sub ProcessProductTemplatesPostBack()
        _ProductType = New Commerce.ProductType(_ContentApi.RequestInformationRef)
        Dim IsInheritingTemplates As String = Request.Form("TemplateTypeBreak")
        Dim IsInheritingXml As String = Request.Form("TypeBreak")
        Dim prod_type_list As New List(Of Commerce.ProductTypeData)
        Dim template_data As TemplateData()
        Dim criteria As New Criteria(Of ProductTypeProperty)

        prod_type_list = _ProductType.GetList(criteria)
        template_data = _ContentApi.GetAllTemplates("TemplateFileName")

        Dim i As Integer = 0
        Dim active_prod_list As New Collection
        Dim template_active_list As New Collection
        Dim default_xml_id As Long = -1

        If (IsInheritingTemplates Is Nothing) Then
            For i = 0 To template_data.Length - 1
                If (Not Request.Form("tinput_" & template_data(i).Id) Is Nothing) Then
                    template_active_list.Add(template_data(i).Id, template_data(i).Id)
                End If
            Next
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

        _ContentApi.UpdateFolderMultiConfig(_FolderId, default_xml_id, template_active_list, active_prod_list)
    End Sub
#End Region

#Region "Sitemap Path"
    Private Sub DisplaySitemapPath()
        Dim sJS As New System.Text.StringBuilder
        Dim node As Common.SitemapPath
        sJS.AppendLine("var clientName_chkInheritSitemapPath = 'chkInheritSitemapPath';")
        If (_FolderData.SitemapInherited = 1 And _FolderData.Id <> 0) Then
            'chkInheritSitemapPath.Checked = True
            sJS.AppendLine("document.getElementById(""hdnInheritSitemap"").value = 'true';")
            sJS.AppendLine("document.getElementById(""chkInheritSitemapPath"").checked = true;")
            sJS.AppendLine("document.getElementById(""AddSitemapNode"").style.display = 'none';")
        Else
            sJS.AppendLine("document.getElementById(""hdnInheritSitemap"").value = 'false';")
            sJS.AppendLine("document.getElementById(""chkInheritSitemapPath"").checked = false;")
            'chkInheritSitemapPath.Checked = False
        End If
        If (_FolderData.Id = 0) Then
            'chkInheritSitemapPath.Disabled = True
            Ektron.Cms.API.JS.RegisterJS(Me, _ContentApi.AppPath & "controls/folder/sitemap.js", "EktronSitemapJS")
            sJS.AppendLine("document.getElementById(""chkInheritSitemapPath"").disable = true;")
            sJS.AppendLine("document.getElementById(""dvInheritSitemap"").style.display = 'none';")
        End If
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
        Dim sJS As New System.Text.StringBuilder
        Dim _refSiteAliasApi As New Ektron.Cms.SiteAliasApi
        Dim item As Ektron.Cms.Common.SiteAliasData
        Dim siteAliasList As System.Collections.Generic.List(Of Ektron.Cms.Common.SiteAliasData)
        Dim pagingInfo As New Ektron.Cms.PagingInfo
        Dim index As Integer = 0

        siteAliasList = _refSiteAliasApi.GetList(pagingInfo, _FolderData.Id)
        If (siteAliasList IsNot Nothing) Then
            sJS.Append("arSiteAliasNames = new Array(")
            For Each item In siteAliasList
                If (item IsNot Nothing) Then
                    If (index <> 0) Then
                        sJS.Append(",")
                    End If
                    sJS.Append("new item('" & item.SiteAliasName & "'," & index & ")")
                    index = index + 1
                End If
            Next
            sJS.AppendLine(");")
            sJS.AppendLine("renderSiteAliasNames();")
        End If
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "renderSiteAliasNames", sJS.ToString(), True)
    End Sub

#End Region

#Region "CSS, JS"

    Private Sub RegisterResources()
        'CSS
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)

        'JS
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, _ContentApi.ApplicationPath & "/controls/folder/sitemap.js", "EktronSitemapJS")
        Ektron.Cms.API.JS.RegisterJS(Me, _ContentApi.ApplicationPath & "/controls/folder/sitealias.js", "EktronSiteAliasJS")
        Ektron.Cms.API.JS.RegisterJS(Me, _ContentApi.ApplicationPath & "/tree/js/com.ektron.utils.dom.js", "EktronDomUtilsJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub

#End Region

End Class
