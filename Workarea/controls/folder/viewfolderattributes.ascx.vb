Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.CustomFieldsApi
Imports Ektron.Cms.Commerce

Partial Class viewfolderattributes
    Inherits System.Web.UI.UserControl

#Region "Member Variables"

    Protected _ContentApi As New ContentAPI
    Protected _SiteApi As New SiteAPI
    Protected _StyleHelper As New StyleHelper
    Protected _MessageHelper As Common.EkMessageHelper
    Protected _Id As Long = 0
    Protected _FolderData As FolderData
    Protected _PermissionData As PermissionData
    Protected _AppImgPath As String = ""
    Protected _AppPath As String = ""
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
    Protected _ProductType As Commerce.ProductType = Nothing
    Protected _Catalog As Boolean = False

    Private _SubscriptionData As SubscriptionData()
    Private _SubscribedData As SubscriptionData()
    Private _SubscriptionPropertiesData As SubscriptionPropertiesData
    Private _BreakSubInheritance As Boolean = False
    Private _GlobalSubInherit As Boolean = False
    Private _SettingsData As New SettingsData
    Private _FolderType As Integer = 0
    Private _BlogData As BlogData
    Private _i As Integer = 0

#End Region

#Region "Events"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _AppPath = _ContentApi.AppPath
        RegisterResources()
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        _MessageHelper = _ContentApi.EkMsgRef
        _SettingsData = _SiteApi.GetSiteVariables(_SiteApi.UserId)
    End Sub

#End Region

#Region "CSS JS"

    Private Sub RegisterResources()
        'CSS
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)

        'JS
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me._AppPath & "/controls/folder/sitemap.js", "EktronSitemapJS")

    End Sub

#End Region


    Public Function ViewFolderAttributes() As Boolean
        If (Not (Request.QueryString("id") Is Nothing)) Then
            _Id = Convert.ToInt64(Request.QueryString("id"))
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
        ' Ensure that a specific language is selected, use default if needed:
        If (_ContentLanguage = CONTENT_LANGUAGES_UNDEFINED) Or (_ContentLanguage = ALL_CONTENT_LANGUAGES) Then
            _ContentLanguage = _ContentApi.RequestInformationRef.DefaultContentLanguage
        End If
        _ContentApi.ContentLanguage = _ContentLanguage

        _CurrentUserId = _ContentApi.UserId
        _AppImgPath = _ContentApi.AppImgPath
        _AppPath = _ContentApi.AppPath
        _SitePath = _ContentApi.SitePath
        _EnableMultilingual = _ContentApi.EnableMultilingual
        If (Not (Page.IsPostBack)) Then
            _FolderData = _ContentApi.GetFolderById(_Id, True, True)
            _FolderType = _FolderData.FolderType
            _FolderId = _FolderData.Id
            Select Case _FolderType
                Case Common.EkEnumeration.FolderType.Catalog
                    _Catalog = True
                    Display_ViewCatalog()
                Case Else
                    phWebAlerts.Visible = True
                    Display_ViewFolder()
            End Select
        End If

        If (_FolderData.IsCommunityFolder) Then
            Display_AddCommunityFolder()
        End If
    End Function

    Private Sub Display_AddCommunityFolder()
        If (_FolderData Is Nothing) Then
            _FolderData = _ContentApi.GetFolderById(_Id, True)
        End If
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar("Community Folder " & " """ & _FolderData.Name & """")
        ltr_vf_smartforms.Visible = True
        ltrTypes.Visible = True
    End Sub

#Region "Folder - ViewFolder"

    Private Sub Display_ViewFolder()
        Dim cPreApproval As Collection
        Dim isBlog As Boolean
        isBlog = IIf(_FolderType = 1, True, False)
        If isBlog Then
            _BlogData = _ContentApi.BlogObject(_FolderData)
            _FolderData.PublishPdfEnabled = False
            phSubjects.Visible = True
            phBlogRoll.Visible = True
            phDescription.Visible = False
        End If

        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder")

        ltrTypes.Text = _MessageHelper.GetMessage("Smart Forms txt")
        'Sitemap Path
        ltInheritSitemapPath.Text = _MessageHelper.GetMessage("lbl Inherit Parent Configuration")

        ViewFolderToolBar()

        If isBlog Then
            phBlogProperties1.Visible = True
            If _BlogData.Visibility = Common.EkEnumeration.BlogVisibility.Public Then
                td_vf_visibilitytxt.InnerHtml = "Public"
            Else
                td_vf_visibilitytxt.InnerHtml = "Private"
            End If

            td_vf_nametxt.InnerHtml = _BlogData.Name
            td_vf_titletxt.InnerHtml = _BlogData.Title
        Else
            phFolderProperties1.Visible = True
            td_vf_foldertxt.InnerHtml = _FolderData.Name
        End If

        td_vf_idtxt.InnerHtml = _Id

        If isBlog Then
            phBlogProperties2.Visible = True
            Dim sEnabled As String = ""
            Dim sModerate As String = ""
            Dim sRequire As String = ""
            Dim sNotify As String = ""
            If _BlogData.EnableComments Then
                sEnabled = "checked=""checked"" "
            End If
            If _BlogData.ModerateComments Then
                sModerate = "checked=""checked"" "
            End If
            If _BlogData.RequiresAuthentication Then
                sRequire = "checked=""checked"" "
            End If
            If _BlogData.NotifyURL <> "" Then
                sNotify = "checked=""checked"" "
            End If
            td_vf_taglinetxt.InnerHtml = _BlogData.Tagline
            If _BlogData.PostsVisible < 0 Then
                td_vf_postsvisibletxt.InnerHtml = "(selected day)"
            Else
                td_vf_postsvisibletxt.InnerHtml &= _BlogData.PostsVisible.ToString()
            End If
            td_vf_commentstxt.InnerHtml &= "<input disabled=""disabled"" type=""checkbox"" name=""enable_comments"" id=""enable_comments"" " & sEnabled & " />Enabled"
            td_vf_commentstxt.InnerHtml &= "<br />"
            td_vf_commentstxt.InnerHtml &= "<input disabled=""disabled"" type=""checkbox"" name=""moderate_comments"" id=""moderate_comments"" " & sModerate & " />Moderated"
            td_vf_commentstxt.InnerHtml &= "<br />"
            td_vf_commentstxt.InnerHtml &= "<input disabled=""disabled"" type=""checkbox"" name=""require_authentication"" id=""require_authentication"" " & sRequire & " />Require authentication"

            td_vf_updateservicestxt.InnerHtml &= "<input type=""checkbox"" name=""notify_url"" id=""notify_url"" " & sNotify & " disabled=""disabled"" />Notify blog search engines of new posts"
            td_vf_updateservicestxt.InnerHtml &= "<br />"
            td_vf_updateservicestxt.InnerHtml &= _BlogData.NotifyURL
        Else
            td_vf_folderdesctxt.InnerHtml = _FolderData.Description
        End If

        If (_FolderData.StyleSheet = "") Then
            td_vf_stylesheettxt.InnerHtml += _MessageHelper.GetMessage("none specified msg")
        Else
            td_vf_stylesheettxt.InnerHtml += _SitePath & _FolderData.StyleSheet
        End If

        If (_FolderData.StyleSheetInherited) Then
            td_vf_stylesheettxt.InnerHtml += "<div class=""ektronCaption"">" & _MessageHelper.GetMessage("inherited style sheet msg") & "</div>"
        End If

        DrawContentTemplatesTable()
        DrawFolderTaxonomyTable() 'Assigned taxonomy
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
            If _FolderData.PublishPdfActive Then
                td_vf_pdfactivetxt.InnerHtml += "Publish as PDF"
                ltrCheckPdfServiceProvider.Visible = True
            Else
                td_vf_pdfactivetxt.InnerHtml += "Publish in native format"
                ltrCheckPdfServiceProvider.Visible = False
            End If
        Else
            phPublishAsPdf.Visible = False
        End If

        ' show domain info
        If _FolderData.IsDomainFolder Then
            phProductionDomain.Visible = True
            Dim settings_list As SettingsData
            Dim m_refSiteAPI As New SiteAPI
            Dim m_refCommonAPI As New CommonApi
            Dim request_info As Ektron.Cms.Common.EkRequestInformation
            settings_list = m_refSiteAPI.GetSiteVariables()
            request_info = m_refCommonAPI.RequestInformationRef

            DomainFolder.Text += "<tr>"
            DomainFolder.Text += "<td class=""label"">" & _MessageHelper.GetMessage("lbl Staging Domain") & ":</td>"
            DomainFolder.Text += "<td class=""value"">http://" + _FolderData.DomainStaging + "</td>"
            DomainFolder.Text += "</tr>"
            DomainFolder.Text += "<tr>"
            DomainFolder.Text += "<td class=""label"">" & _MessageHelper.GetMessage("lbl Production Domain") & ":</td>"
            DomainFolder.Text += "<td class=""value"">http://" + _FolderData.DomainProduction + "</td>"
            DomainFolder.Text += "</tr>"
        End If

        ' show categories if its a blog
        If isBlog Then
            If Not (_BlogData.Categories Is Nothing) AndAlso _BlogData.Categories.Length > 0 Then
                For Me._i = 0 To _BlogData.Categories.Length - 1
                    ltr_vf_categories.Text &= _BlogData.Categories(Me._i).ToString()
                    ltr_vf_categories.Text &= "<br/>"
                Next
            Else
                ltr_vf_categories_lbl.Text = "No subjects"
            End If

            If Not (_BlogData.BlogRoll Is Nothing) AndAlso _BlogData.BlogRoll.Length > 0 Then
                For Me._i = 0 To _BlogData.BlogRoll.Length - 1
                    Dim tRoll As New Table
                    tRoll.CssClass = "ektronGrid"
                    Dim tRollRow As New TableRow
                    Dim tRollCell As New TableCell
                    'Link Name
                    tRollCell = New TableCell
                    tRollRow = New TableRow
                    tRollCell.Text = "Link Name:"
                    tRollCell.CssClass = "label"
                    tRollRow.Controls.Add(tRollCell)
                    tRollCell = New TableCell
                    tRollCell.Text = _BlogData.BlogRoll.RollItem(Me._i).LinkName
                    tRollCell.CssClass = "readOnlyValue"
                    tRollRow.Controls.Add(tRollCell)
                    tRoll.Controls.Add(tRollRow)
                    'URL
                    tRollCell = New TableCell
                    tRollRow = New TableRow
                    tRollCell.Text = "URL:"
                    tRollCell.CssClass = "label"
                    tRollRow.Controls.Add(tRollCell)
                    tRollCell = New TableCell
                    tRollCell.Text = _BlogData.BlogRoll.RollItem(Me._i).URL
                    tRollCell.CssClass = "readOnlyValue"
                    tRollRow.Controls.Add(tRollCell)
                    tRoll.Controls.Add(tRollRow)
                    'Short Description
                    tRollCell = New TableCell
                    tRollRow = New TableRow
                    tRollCell.Text = "Short Description:"
                    tRollCell.CssClass = "label"
                    tRollRow.Controls.Add(tRollCell)
                    tRollCell = New TableCell
                    tRollCell.Text = _BlogData.BlogRoll.RollItem(Me._i).ShortDescription
                    tRollCell.CssClass = "readOnlyValue"
                    tRollRow.Controls.Add(tRollCell)
                    tRoll.Controls.Add(tRollRow)
                    'Relationship
                    tRollCell = New TableCell
                    tRollRow = New TableRow
                    tRollCell.Text = "Relationship:"
                    tRollCell.CssClass = "label"
                    tRollRow.Controls.Add(tRollCell)
                    tRollCell = New TableCell
                    tRollCell.Text = _BlogData.BlogRoll.RollItem(Me._i).Relationship
                    tRollCell.CssClass = "readOnlyValue"
                    tRollRow.Controls.Add(tRollCell)
                    tRoll.Controls.Add(tRollRow)
                    lbl_vf_roll.Controls.Add(tRoll)

                    Dim spacer As New Literal()
                    spacer.Text = "<div class='ektronTopSpace'></div>"
                    lbl_vf_roll.Controls.Add(spacer)
                Next
            End If
        End If

        If _SettingsData.EnablePreApproval Then
            phPreapprovalGroup.Visible = True
            cPreApproval = _ContentApi.EkContentRef.GetFolderPreapprovalGroup(_Id)
            If (-1 = cPreApproval("PreApprovalGroupID")) Then
                td_vf_preapprovaltxt.InnerHtml += cPreApproval("UserGroupName") & " (Inherited)"
            ElseIf (0 = cPreApproval("PreApprovalGroupID")) Then
                td_vf_preapprovaltxt.InnerHtml += "(None)"
            Else
                td_vf_preapprovaltxt.InnerHtml += cPreApproval("PreApprovalGroup")
            End If
        End If

        ' display replication settings for folder
        If (_ContentApi.RequestInformationRef.EnableReplication) Then
            Dim bShowReplicationMethod As Boolean = True
            If (_FolderData.ParentId <> 0 AndAlso (_FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Blog Or _FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum)) Then
                Dim tmp_folder_data As FolderData = Nothing
                tmp_folder_data = Me._ContentApi.EkContentRef.GetFolderById(_FolderData.ParentId)
                If (tmp_folder_data.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Community) Then
                    bShowReplicationMethod = False
                End If
            End If
            If (bShowReplicationMethod) Then
                ReplicationMethod.Text = "<tr><td>&nbsp;</td></tr><tr><td class=""label"">" + _MessageHelper.GetMessage("lbl folderdynreplication") + "</td></tr><tr><td>"
                If _FolderData.ReplicationMethod = 1 Then
                    ReplicationMethod.Text += _MessageHelper.GetMessage("replicate folder contents")
                Else
                    ReplicationMethod.Text += _MessageHelper.GetMessage("generic No")
                End If
                ReplicationMethod.Text += "	</td></tr>"
            End If
        End If

        ' Show Custom-Field folder assignments:
        Dim customFieldsApi As New CustomFieldsApi
        If _ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            customFieldsApi.ContentLanguage = _ContentApi.DefaultContentLanguage()
        Else
            customFieldsApi.ContentLanguage = _ContentLanguage
        End If
        litMetadata.Text = customFieldsApi.GetEditableCustomFieldAssignments(_Id, False)
        customFieldsApi = Nothing
        DisplaySitemapPath()
        DisplaySubscriptionInfo()
        DrawContentTypesTable()
        If (_FolderType = 2) Then 'OrElse m_intFolderId = 0 Avoiding root to be site aliased
            phSiteAlias.Visible = True
            DisplaySiteAlias()
        End If
        Showpane()
    End Sub

    Private Sub DisplaySitemapPath()
        Dim sJS As New System.Text.StringBuilder
        Dim node As Common.SitemapPath
        If (_FolderData.SitemapInherited) Then
            chkInheritSitemapPath.Checked = True
        Else
            chkInheritSitemapPath.Checked = False
        End If
        chkInheritSitemapPath.Disabled = True
        If (_FolderData.Id = 0) Then
            pnlInheritSitemapPath.Visible = False
        End If
        If (_FolderData.SitemapPath IsNot Nothing) Then
            Ektron.Cms.API.JS.RegisterJS(Me, _ContentApi.AppPath & "controls/folder/sitemap.js", "EktronSitemapJS")

            sJS.Append("arSitemapPathNodes = new Array(")
            For Each node In _FolderData.SitemapPath
                If (node IsNot Nothing) Then
                    If (node.Order <> 0) Then
                        sJS.Append(",")
                    End If
                    sJS.Append("new node('" & Server.HtmlDecode(node.Title).Replace("'", "\'") & "','" & node.Url & "','" & node.Description & "'," & node.Order & ")")
                End If
            Next
            sJS.AppendLine(");")
            sJS.AppendLine("previewSitemapPath();")
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "previewSitemapPath", sJS.ToString(), True)
        Else
            chkInheritSitemapPath.Visible = False
            ltInheritSitemapPath.Visible = True
            ltInheritSitemapPath.Text = Me._MessageHelper.GetMessage("lbl breadcrumb not created")
        End If

    End Sub

    Private Sub DisplaySiteAlias()
        Dim sJS As New System.Text.StringBuilder
        Dim _refSiteAliasApi As New Ektron.Cms.SiteAliasApi
        Dim siteAliasList As New System.Collections.Generic.List(Of Ektron.Cms.Common.SiteAliasData)
        Dim page As New Ektron.Cms.PagingInfo
        Dim item As Ektron.Cms.Common.SiteAliasData
        siteAliasList = _refSiteAliasApi.GetList(page, _FolderData.Id)
        viewSiteAliasList.InnerHtml = "<table width=""100%"">"
        For Each item In siteAliasList
            viewSiteAliasList.InnerHtml = viewSiteAliasList.InnerHtml & "<tr><td><img src=""" & _ContentApi.AppPath & "images/ui/icons/folderSite.png"" /></td>"
            viewSiteAliasList.InnerHtml = viewSiteAliasList.InnerHtml & "<td>" & item.SiteAliasName & "</td></tr>"
        Next
        viewSiteAliasList.InnerHtml = viewSiteAliasList.InnerHtml & "</table>"
    End Sub

    Private Sub Showpane()
        If _ShowPane.Length > 0 Then
            lbl_vf_showpane.Text &= "<script language=""Javascript"">" & Environment.NewLine
            Select Case _ShowPane
                Case "blogroll"
                    lbl_vf_showpane.Text &= "   ShowPane('dvRoll');"
            End Select
            lbl_vf_showpane.Text &= "</script>" & Environment.NewLine
        End If
    End Sub

    Private Sub ViewFolderToolBar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view properties for folder msg") & " """ & _FolderData.Name & """")
        result.Append("<table><tr>")

        If (_PermissionData.CanEditFolders OrElse _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id)) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/contentEdit.png", "content.aspx?LangType=" & _ContentLanguage & "&action=EditFolder&id=" & _Id, _MessageHelper.GetMessage("alt edit properties button text (folder)"), _MessageHelper.GetMessage("btn edit prop"), ""))
        End If
        If (_PermissionData.IsAdmin OrElse _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id)) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/permissions.png", "content.aspx?LangType=" & _ContentLanguage & "&action=ViewPermissions&type=folder&id=" & _Id, _MessageHelper.GetMessage("alt permissions button text (edit)"), _MessageHelper.GetMessage("btn view permissions"), ""))
        End If
        If _SettingsData.EnablePreApproval Then
            If (_PermissionData.CanEditApprovals OrElse _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id)) Then
                result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/approvalPreapprove.png", "content.aspx?LangType=" & _ContentLanguage & "&action=EditPreApprovals&type=folder&id=" & _Id, _MessageHelper.GetMessage("alt change preapp grp"), _MessageHelper.GetMessage("alt modify grp"), ""))
            End If
        End If
        If (_PermissionData.IsAdmin OrElse _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id)) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/approvals.png", "content.aspx?LangType=" & _ContentLanguage & "&action=ViewApprovals&type=folder&id=" & _Id, _MessageHelper.GetMessage("alt approvals button text (edit)"), _MessageHelper.GetMessage("btn view approvals"), ""))
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/historyDelete.png", "purgehist.aspx?LangType=" & _ContentLanguage & "&action=View&folderId=" & _Id, _MessageHelper.GetMessage("alt purge content hist"), _MessageHelper.GetMessage("btn purge history"), ""))
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/restore.png", "content.aspx?LangType=" & _ContentLanguage & "&action=RestoreInheritance&id=" & _Id, _MessageHelper.GetMessage("alt restore web alert"), _MessageHelper.GetMessage("lbl restore web"), "onclick=""return ConfirmRestoreInheritance();"""))
        End If
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", "content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        If _EnableMultilingual = 1 Then
            result.Append("<td>&#160;|&#160;</td>")
            result.Append("<td class=""label"">")
            result.Append(_MessageHelper.GetMessage("view language"))
            result.Append("</td>")
            result.Append("<td>")
            result.Append(_StyleHelper.ShowAllActiveLanguage(False, "", "javascript:SelLanguage(this.value);", _ContentLanguage))
            result.Append("&nbsp;</td>")
        End If

        result.Append("<td>")
        If _FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Calendar Then
            result.Append(_StyleHelper.GetHelpButton("calendar_" & _PageAction))
        Else
            result.Append(_StyleHelper.GetHelpButton(_StyleHelper.GetHelpAliasPrefix(_FolderData) & _PageAction))
        End If
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub DisplaySubscriptionInfo()
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
        Dim settings_list As SettingsData
        Dim m_refSiteAPI As New SiteAPI

        _SubscriptionData = _ContentApi.GetAllActiveSubscriptions() 'then get folder
        emailfrom_list = _ContentApi.GetAllEmailFrom()
        defaultmessage_list = _ContentApi.GetSubscriptionMessagesForType(Common.EkEnumeration.EmailMessageTypes.DefaultMessage)
        unsubscribe_list = _ContentApi.GetSubscriptionMessagesForType(Common.EkEnumeration.EmailMessageTypes.Unsubscribe)
        optout_list = _ContentApi.GetSubscriptionMessagesForType(Common.EkEnumeration.EmailMessageTypes.OptOut)
        settings_list = m_refSiteAPI.GetSiteVariables()

        intInheritFrom = _ContentApi.GetFolderInheritedFrom(_Id)
        If intInheritFrom <> _Id Then 'do we get settings from self?
            _GlobalSubInherit = True
        Else
            _GlobalSubInherit = False
        End If
        _SubscribedData = _ContentApi.GetSubscriptionsForFolder(intInheritFrom)
        _SubscriptionPropertiesData = _ContentApi.GetSubscriptionPropertiesForFolder(intInheritFrom)

        If (emailfrom_list Is Nothing) Or (defaultmessage_list Is Nothing) Or (unsubscribe_list Is Nothing) Or (optout_list Is Nothing) Or (_SubscriptionData Is Nothing) _
             Or ((settings_list.AsynchronousLocation = "")) Then
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

        If _SubscriptionPropertiesData Is Nothing Then
            _SubscriptionPropertiesData = New SubscriptionPropertiesData
        End If

        strEnabled = " disabled=""disabled"" "

        Select Case _SubscriptionPropertiesData.NotificationType.GetHashCode
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

        If _Id = 0 Then ' root folder
            lit_vf_subscription_properties.Text &= "<input id=""webalert_inherit_button"" type=""hidden"" name=""webalert_inherit_button"" value=""webalert_inherit_button"" checked=""checked"">"
        ElseIf Not _GlobalSubInherit Then ' not inheriting
            lit_vf_subscription_properties.Text &= "<input id=""webalert_inherit_button"" type=""checkbox"" name=""webalert_inherit_button"" value=""webalert_inherit_button"" disabled=""disabled"">" & _MessageHelper.GetMessage("lbl inherit parent configuration")
            lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
        Else ' non root
            lit_vf_subscription_properties.Text &= "<input id=""webalert_inherit_button"" type=""checkbox"" name=""webalert_inherit_button"" value=""webalert_inherit_button"" checked=""checked"" disabled=""disabled"" >" & _MessageHelper.GetMessage("lbl inherit parent configuration")
            lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
        End If

        lit_vf_subscription_properties.Text &= "<table class=""ektronGrid"">"
        lit_vf_subscription_properties.Text &= "<tr>"
        lit_vf_subscription_properties.Text &= "<td class=""label"">"
        lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert opt") & ":"
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "<td class=""value"">"
        lit_vf_subscription_properties.Text &= "<input type=""radio"" value=""Always"" name=""notify_option"" " & strNotifyA & " " & strEnabled & ">" & _MessageHelper.GetMessage("lbl web alert notify always")
        lit_vf_subscription_properties.Text &= "<br />"
        lit_vf_subscription_properties.Text &= "<input type=""radio"" value=""Initial"" name=""notify_option""" & strNotifyI & " " & strEnabled & ">" & _MessageHelper.GetMessage("lbl web alert notify initial")
        lit_vf_subscription_properties.Text &= "<br />"
        lit_vf_subscription_properties.Text &= "<input type=""radio"" value=""Never"" name=""notify_option""" & strNotifyN & " " & strEnabled & ">" & _MessageHelper.GetMessage("lbl web alert notify never")
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "</tr>"

        lit_vf_subscription_properties.Text &= "<tr>"
        lit_vf_subscription_properties.Text &= "<td class=""label"">"
        lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert subject") & ":"
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "<td class=""value"">"
        If _SubscriptionPropertiesData.Subject <> "" Then
            lit_vf_subscription_properties.Text &= "<input type=""text"" maxlength=""255"" size=""65"" value=""" & _SubscriptionPropertiesData.Subject & """ name=""notify_subject"" " & strEnabled & ">"
        Else
            lit_vf_subscription_properties.Text &= "<input type=""text"" maxlength=""255"" size=""65"" value="""" name=""notify_subject"" " & strEnabled & ">"
        End If
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "</tr>"

        'lit_vf_subscription_properties.Text &= "Notification Base URL:"
        'If subscription_properties_list.URL <> "" Then
        '    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" " & strEnabled & " value=""" & subscription_properties_list.URL & """>"
        'Else
        '    lit_vf_subscription_properties.Text &= "http://<input type=""text"" maxlength=""255"" size=""65"" name=""notify_url"" " & strEnabled & " value=""" & Request.ServerVariables("HTTP_HOST") & """>"
        'End If

        lit_vf_subscription_properties.Text &= "<tr>"
        lit_vf_subscription_properties.Text &= "<td class=""label"">"
        lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert emailfrom address") & ":"
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "<td class=""value"">"
        lit_vf_subscription_properties.Text &= "<select name=""notify_emailfrom"" " & strEnabled & ">:"

        If (Not emailfrom_list Is Nothing) AndAlso emailfrom_list.Length > 0 Then
            For y = 0 To emailfrom_list.Length - 1
                If emailfrom_list(y).Email = _SubscriptionPropertiesData.EmailFrom Then
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
        'lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""" & subscription_properties_list.WebLocation & """ name=""notify_weblocation"" " & strEnabled & ">/"
        'Else
        '    lit_vf_subscription_properties.Text &= m_refContentApi.SitePath & "<input type=""text"" maxlength=""255"" size=""65"" value=""subscriptions"" name=""notify_weblocation"" " & strEnabled & ">/"
        'End If

        lit_vf_subscription_properties.Text &= "<tr>"
        lit_vf_subscription_properties.Text &= "<td class=""label"">"
        lit_vf_subscription_properties.Text &= _MessageHelper.GetMessage("lbl web alert contents") & ":"
        lit_vf_subscription_properties.Text &= "</td>"
        lit_vf_subscription_properties.Text &= "<td class=""value"">"
        lit_vf_subscription_properties.Text += "<input id=""use_optout_button"" type=""checkbox"" checked=""checked"" name=""use_optout_button"" disabled=""disabled"">" & _MessageHelper.GetMessage("lbl optout message") & "&nbsp;&nbsp;"

        lit_vf_subscription_properties.Text += "<select " & strEnabled & " name=""notify_optoutid"">"
        If (Not optout_list Is Nothing) AndAlso optout_list.Length > 0 Then
            For y = 0 To optout_list.Length - 1
                If optout_list(y).Id = _SubscriptionPropertiesData.OptOutID Then
                    lit_vf_subscription_properties.Text &= ("<option value=""" & optout_list(y).Id & """ SELECTED>" & Server.HtmlEncode(optout_list(y).Title) & "</option>")
                Else
                    lit_vf_subscription_properties.Text &= ("<option value=""" & optout_list(y).Id & """>" & Server.HtmlEncode(optout_list(y).Title) & "</option>")
                End If
            Next
        End If
        lit_vf_subscription_properties.Text += "</select>"

        lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
        If _SubscriptionPropertiesData.DefaultMessageID > 0 Then
            lit_vf_subscription_properties.Text += ("<input id=""use_message_button"" type=""checkbox"" checked=""checked"" name=""use_message_button"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use default message")) & "&nbsp;&nbsp;"
        Else
            lit_vf_subscription_properties.Text += ("<input id=""use_message_button"" type=""checkbox"" name=""use_message_button"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use default message")) & "&nbsp;&nbsp;"
        End If

        lit_vf_subscription_properties.Text += "<select " & strEnabled & " name=""notify_messageid"">"
        If (Not defaultmessage_list Is Nothing) AndAlso defaultmessage_list.Length > 0 Then
            For y = 0 To defaultmessage_list.Length - 1
                If defaultmessage_list(y).Id = _SubscriptionPropertiesData.DefaultMessageID Then
                    lit_vf_subscription_properties.Text += "<option value=""" & defaultmessage_list(y).Id & """ SELECTED>" & Server.HtmlEncode(defaultmessage_list(y).Title) & "</option>"
                Else
                    lit_vf_subscription_properties.Text += "<option value=""" & defaultmessage_list(y).Id & """>" & Server.HtmlEncode(defaultmessage_list(y).Title) & "</option>"
                End If
            Next
        End If
        lit_vf_subscription_properties.Text += "</select>"

        lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
        If _SubscriptionPropertiesData.SummaryID > 0 Then
            lit_vf_subscription_properties.Text += "<input id=""use_summary_button"" type=""checkbox"" name=""use_summary_button"" checked=""checked"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use summary message")
        Else
            lit_vf_subscription_properties.Text += "<input id=""use_summary_button"" type=""checkbox"" name=""use_summary_button"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use summary message")
        End If
        lit_vf_subscription_properties.Text += "<br />"
        If _SubscriptionPropertiesData.ContentID = -1 Then
            lit_vf_subscription_properties.Text += "<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" checked=""checked"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use content message") & "&nbsp;&nbsp;"
            lit_vf_subscription_properties.Text += "<input type=""text"" name=""titlename"" value=""[[use current]]"" " & strEnabled & " size=""65"" />"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" maxlength=""20"" name=""frm_content_id"" value=""-1""/>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_content_langid""/>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_qlink""/>"
        ElseIf _SubscriptionPropertiesData.ContentID > 0 Then
            lit_vf_subscription_properties.Text += "<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" checked=""checked"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use content message") & "&nbsp;&nbsp;"
            lit_vf_subscription_properties.Text += "<input type=""text"" name=""titlename"" value=""" & _SubscriptionPropertiesData.UseContentTitle.ToString & """ " & strEnabled & " size=""65"" />"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" maxlength=""20"" name=""frm_content_id"" value=""" & _SubscriptionPropertiesData.ContentID.ToString() & """/>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_content_langid""/>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_qlink""/>"
        Else
            lit_vf_subscription_properties.Text += "<input id=""use_content_button"" type=""checkbox"" name=""use_content_button"" " & strEnabled & ">" & _MessageHelper.GetMessage("lbl use content message")
            lit_vf_subscription_properties.Text += "<input type=""text"" name=""titlename"" onkeydown=""return false"" value="""" " & strEnabled & " size=""65"" />"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" maxlength=""20"" name=""frm_content_id"" value=""0"" />"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_content_langid""/>"
            lit_vf_subscription_properties.Text += "<input type=""hidden"" name=""frm_qlink""/>"
        End If
        lit_vf_subscription_properties.Text += "<br />"
        If _SubscriptionPropertiesData.UseContentLink > 0 Then
            lit_vf_subscription_properties.Text += "<input id=""use_contentlink_button"" type=""checkbox"" name=""use_contentlink_button"" checked=""checked"" " & strEnabled & ">Use Content Link"
        Else
            lit_vf_subscription_properties.Text += "<input id=""use_contentlink_button"" type=""checkbox"" name=""use_contentlink_button"" " & strEnabled & ">Use Content Link"
        End If

        lit_vf_subscription_properties.Text &= "<div class=""ektronTopSpace""></div>"
        lit_vf_subscription_properties.Text += "<input id=""use_unsubscribe_button"" type=""checkbox"" checked=""checked"" name=""use_unsubscribe_button"" disabled=""disabled"">" & _MessageHelper.GetMessage("lbl unsubscribe message") & "&nbsp;&nbsp;"

        lit_vf_subscription_properties.Text += "<select " & strEnabled & " name=""notify_unsubscribeid"">"
        If (Not unsubscribe_list Is Nothing) AndAlso unsubscribe_list.Length > 0 Then
            For y = 0 To unsubscribe_list.Length - 1
                If unsubscribe_list(y).Id = _SubscriptionPropertiesData.UnsubscribeID Then
                    lit_vf_subscription_properties.Text += "<option value=""" & unsubscribe_list(y).Id & """ SELECTED>" & Server.HtmlEncode(unsubscribe_list(y).Title) & "</option>"
                Else
                    lit_vf_subscription_properties.Text += "<option value=""" & unsubscribe_list(y).Id & """>" & Server.HtmlEncode(unsubscribe_list(y).Title) & "</option>"
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
            lit_vf_subscription_properties.Text &= "<tbody>"
            lit_vf_subscription_properties.Text += "<tr class=""title-header"">"
            lit_vf_subscription_properties.Text += "<th width=""10%"">" & _MessageHelper.GetMessage("lbl assigned") & "</th>"
            lit_vf_subscription_properties.Text += "<th>" & _MessageHelper.GetMessage("lbl name") & "</th>"
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
                    lit_vf_subscription_properties.Text += "<td width=""10%"" class=""center""><input type=""checkbox"" name=""Assigned_" & _SubscriptionData(i).Id & """  id=""Assigned_" & _SubscriptionData(i).Id & """ " & strEnabled & "></td>"
                Else
                    lit_vf_subscription_properties.Text += "<td class=""center""><input type=""checkbox"" name=""Assigned_" & _SubscriptionData(i).Id & """  id=""Assigned_" & _SubscriptionData(i).Id & """ checked=""checked"" " & strEnabled & "></td>"
                End If
                lit_vf_subscription_properties.Text += "<td>" & _SubscriptionData(i).Name & "</td>"
                lit_vf_subscription_properties.Text += "</tr>"
            Next
            lit_vf_subscription_properties.Text += "</tbody></table>"
        Else
            lit_vf_subscription_properties.Text += "Nothing available."
        End If
        lit_vf_subscription_properties.Text &= "<input type=""hidden"" name=""content_sub_assignments"" id=""content_sub_assignments"" value="""">"
        DrawContentTemplatesTable()
    End Sub

#End Region

#Region "Content Type Selection"

    Private Function DrawContentTypesBreaker(ByVal checked As Boolean) As String
        If (_FolderData.Id = 0) Then
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TypeBreak')"" disabled />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        End If
        If (checked) Then
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TypeBreak')"" checked disabled />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        Else
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TypeBreak')"" disabled />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        End If
    End Function

    Private Function DrawContentTypesHeader() As String
        Dim str As New StringBuilder()
        str.Append("<table class=""ektronGrid""><tbody id=""contentTypeTable"">")
        str.Append("<tr class=""title-header"">")
        str.Append("<th></th>")
        str.Append("<th class=""left"">Name</th>")
        str.Append("</tr>")
        Return str.ToString()
    End Function

    Private Function DrawContentTypesEntry(ByVal row_id As Integer, ByVal name As String, ByVal xml_id As Long, ByVal isDefault As Boolean) As String
        Dim str As New StringBuilder()
        Dim k As Integer = 0

        str.Append("<tr id=""row_" & xml_id & """>")

        str.Append("<td class=""center"" width=""10%"">")
        If (isDefault) Then
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ checked disabled />")
        Else
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ disabled />")
        End If
        str.Append("</td>")
        str.Append("<td width=""90%"">")
        str.Append(name & "<input id=""input_" & xml_id & """ name=""input_" & xml_id & """ type=""hidden"" value=""" & xml_id & """ />")
        str.Append("</td>")
        str.Append("</tr>")

        Return str.ToString()
    End Function

    Private Function DrawContentTypesFooter() As String
        Return "</tbody></table>"
    End Function

    Private Sub DrawContentTypesTable()
        If (_FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Calendar) Then
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

        Dim broken As Boolean = False
        If (active_xml_list.Length > 0) Then
            broken = True
        End If

        Dim isEnabled As Boolean = IsInheritingXmlMultiConfig()

        Dim str As New System.Text.StringBuilder()
        str.Append(DrawContentTypesBreaker(isEnabled))
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
                str.Append(DrawContentTypesEntry(row_id, xml_config_list(k).Title, xml_config_list(k).Id, Utilities.IsDefaultXmlConfig(xml_config_list(k).Id, active_xml_list)))
                row_id = row_id + 1
            Else
                Dim cRow As New Collection
                cRow.Add(xml_config_list(k).Title, "xml_name")
                cRow.Add(xml_config_list(k).Id, "xml_id")
                addNew.Add(cRow)
            End If
        Next

        If (Not smartFormsRequired) Then
            str.Append(DrawContentTypesEntry(row_id, _MessageHelper.GetMessage("lbl Blank HTML"), 0, Utilities.IsHTMLDefault(active_xml_list)))
        End If

        str.Append(DrawContentTypesFooter())
        str.Append("</div>")

        If (row_id Mod 2 = 0) Then
            str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""1"" />")
        Else
            str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""0"" />")
        End If
        ltr_vf_smartforms.Text = str.ToString()
    End Sub

#End Region

#Region "Catalog"

    Private Sub Display_ViewCatalog()
        Dim cPreApproval As Collection

        _PermissionData = _ContentApi.LoadPermissions(_Id, "folder")

        ltrTypes.Text = _MessageHelper.GetMessage("lbl product types")
        'Sitemap Path
        ltInheritSitemapPath.Text = _MessageHelper.GetMessage("lbl Inherit Parent Configuration")

        ViewCatalogToolBar()

        td_vf_foldertxt.InnerHtml = _FolderData.Name
        td_vf_idtxt.InnerHtml = _Id
        td_vf_folderdesctxt.InnerHtml = _FolderData.Description

        If (_FolderData.StyleSheet = "") Then
            td_vf_stylesheettxt.InnerHtml += _MessageHelper.GetMessage("none specified msg")
        Else
            td_vf_stylesheettxt.InnerHtml += _SitePath & _FolderData.StyleSheet
        End If

        If (_FolderData.StyleSheetInherited) Then
            td_vf_stylesheettxt.InnerHtml += " " & _MessageHelper.GetMessage("style sheet inherited")
        End If

        DrawContentTemplatesTable()
        DrawFolderTaxonomyTable() 'Assigned taxonomy

        If _SettingsData.EnablePreApproval Then
            phPreapprovalGroup.Visible = True
            cPreApproval = _ContentApi.EkContentRef.GetFolderPreapprovalGroup(_Id)
            If (-1 = cPreApproval("PreApprovalGroupID")) Then
                td_vf_preapprovaltxt.InnerHtml += cPreApproval("UserGroupName") & " (Inherited)"
            ElseIf (0 = cPreApproval("PreApprovalGroupID")) Then
                td_vf_preapprovaltxt.InnerHtml += "(None)"
            Else
                td_vf_preapprovaltxt.InnerHtml += cPreApproval("PreApprovalGroup")
            End If
        End If

        ' display replication settings for folder
        If (_ContentApi.RequestInformationRef.EnableReplication) Then
            Dim bShowReplicationMethod As Boolean = True
            If (_FolderData.ParentId <> 0 AndAlso (_FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Blog Or _FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.DiscussionForum)) Then
                Dim tmp_folder_data As FolderData = Nothing
                tmp_folder_data = Me._ContentApi.EkContentRef.GetFolderById(_FolderData.ParentId)
                If (tmp_folder_data.FolderType = Ektron.Cms.Common.EkEnumeration.FolderType.Community) Then
                    bShowReplicationMethod = False
                End If
            End If
            If (bShowReplicationMethod) Then
                ReplicationMethod.Text = "<tr><td>&nbsp;</td></tr><tr><td class=""label"">" + _MessageHelper.GetMessage("lbl folderdynreplication") + "</td></tr><tr><td>"
                If _FolderData.ReplicationMethod = 1 Then
                    ReplicationMethod.Text += _MessageHelper.GetMessage("replicate folder contents")
                Else
                    ReplicationMethod.Text += _MessageHelper.GetMessage("generic No")
                End If
                ReplicationMethod.Text += "	</td></tr>"
            End If
        End If

        ' Show Custom-Field folder assignments:
        Dim customFieldsApi As New CustomFieldsApi
        If _ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            customFieldsApi.ContentLanguage = _ContentApi.DefaultContentLanguage()
        Else
            customFieldsApi.ContentLanguage = _ContentLanguage
        End If
        litMetadata.Text = customFieldsApi.GetEditableCustomFieldAssignments(_Id, False, Common.EkEnumeration.FolderType.Catalog)
        customFieldsApi = Nothing
        DisplaySitemapPath()
        DisplaySubscriptionInfo()
        DrawProductTypesTable()
    End Sub
    Private Sub ViewCatalogToolBar()

        Dim IsInCommerceAdminRole As Boolean = (_PermissionData.IsAdmin OrElse _ContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin))
        Dim IsInFolderAdminRole As Boolean = (_PermissionData.IsAdmin OrElse IsInCommerceAdminRole OrElse _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id))
        Dim result As New System.Text.StringBuilder

        txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view properties for catalog msg") & " """ & _FolderData.Name & """")
        result.Append("<table><tr>")

        If IsInFolderAdminRole Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/contentEdit.png", "content.aspx?LangType=" & _ContentLanguage & "&action=EditFolder&id=" & _Id, _MessageHelper.GetMessage("alt edit properties button text (catalog)"), _MessageHelper.GetMessage("btn edit prop"), ""))
        End If
        If IsInFolderAdminRole Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/permissions.png", "content.aspx?LangType=" & _ContentLanguage & "&action=ViewPermissions&type=folder&id=" & _Id, _MessageHelper.GetMessage("alt permissions button text (catalog)"), _MessageHelper.GetMessage("btn view permissions"), ""))
        End If
        If _SettingsData.EnablePreApproval Then
            If (_PermissionData.CanEditApprovals OrElse _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id)) Then
                result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/approvalPreapprove.png", "content.aspx?LangType=" & _ContentLanguage & "&action=EditPreApprovals&type=folder&id=" & _Id, _MessageHelper.GetMessage("alt change preapp grp"), _MessageHelper.GetMessage("alt modify grp"), ""))
            End If
        End If
        If IsInFolderAdminRole Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/approvals.png", "content.aspx?LangType=" & _ContentLanguage & "&action=ViewApprovals&type=folder&id=" & _Id, _MessageHelper.GetMessage("alt approvals button text (catalog)"), _MessageHelper.GetMessage("btn view approvals"), ""))
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/historyDelete.png", "purgehist.aspx?LangType=" & _ContentLanguage & "&action=View&folderId=" & _Id, _MessageHelper.GetMessage("alt purge entry hist"), _MessageHelper.GetMessage("btn purge history"), ""))
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/restore.png", "content.aspx?LangType=" & _ContentLanguage & "&action=RestoreInheritance&id=" & _Id, _MessageHelper.GetMessage("alt restore catalog web alert"), _MessageHelper.GetMessage("lbl restore web"), "onclick=""return ConfirmRestoreInheritance();"""))
        End If
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", "content.aspx?action=ViewContentByCategory&id=" & _Id & "&LangType=" & _ContentLanguage, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        If _EnableMultilingual = 1 Then
            result.Append("<td class=""right"">|&#160;" & _MessageHelper.GetMessage("view language") & "&#160;" & _StyleHelper.ShowAllActiveLanguage(False, "", "javascript:SelLanguage(this.value);", _ContentLanguage) & "</td>")
        Else
            result.Append("<td>&nbsp;</td>")
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton(_StyleHelper.GetHelpAliasPrefix(_FolderData) & _PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Function DrawProductTypesBreaker(ByVal checked As Boolean) As String
        If (checked) Then
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleProductTypesInherit('TypeBreak', this)"" checked disabled autocomplete='off' />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration") & ""
        Else
            Return "<input name=""TypeBreak"" id=""TypeBreak"" type=""checkbox"" onclick=""ToggleProductTypesInherit('TypeBreak', this)"" disabled autocomplete='off' />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration") & ""
        End If
    End Function
    Private Function DrawProductTypesHeader() As String
        Dim str As New StringBuilder()
        str.Append("<table class=""ektronGrid"" width=""100%""><tbody>")
        str.Append("    <tr class=""title-header"">")
        str.Append("        <td width=""10%"" class=""center"">")
        str.Append(_MessageHelper.GetMessage("lbl default"))
        str.Append("        </td>")
        str.Append("        <td width=""90%"">")
        str.Append(_MessageHelper.GetMessage("lbl prod type"))
        str.Append("        </td>")
        str.Append("    </tr>")
        str.Append("</tbody></table>")
        str.Append("<table class=""ektronGrid"" width=""100%""><tbody id=""contentTypeTable"" name=""contentTypeTable"">")
        Return str.ToString()
    End Function
    Private Function DrawProductTypesEntry(ByVal row_id As Integer, ByVal name As String, ByVal xml_id As Long, ByVal isDefault As Boolean) As String
        Dim str As New StringBuilder()
        Dim k As Integer = 0

        str.Append("<tr id=""row_" & xml_id & """>")

        str.Append("<td class=""center"" width=""10%"">")
        If (isDefault) Then
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ checked disabled />")
        Else
            str.Append("<input type=""radio"" id=""sfdefault"" name=""sfdefault"" value=""" & xml_id & """ disabled />")
        End If
        str.Append("<td width=""90%"">")
        str.Append(name & "<input id=""input_" & xml_id & """ name=""input_" & xml_id & """ type=""hidden"" value=""" & xml_id & """ /></td>")
        str.Append("</tr>")

        Return str.ToString()
    End Function
    Private Sub DrawProductTypesTable()
        _ProductType = New ProductType(_ContentApi.RequestInformationRef)

        Dim prod_type_list As New List(Of ProductTypeData)
        Dim criteria As New Criteria(Of ProductTypeProperty)
        prod_type_list = _ProductType.GetList(criteria)

        Dim active_prod_list As New List(Of ProductTypeData)
        active_prod_list = _ProductType.GetFolderProductTypeList(_FolderData.Id)
        Dim addNew As New Collection()
        Dim k As Integer = 0
        Dim row_id As Integer = 0

        Dim smartFormsRequired As Boolean = True

        Dim broken As Boolean = False
        If (active_prod_list.Count > 0) Then
            broken = True
        End If

        Dim isEnabled As Boolean = IsInheritingXmlMultiConfig()

        Dim str As New System.Text.StringBuilder()
        str.Append(DrawProductTypesBreaker(isEnabled))
        str.Append("<div class=""ektronTopSpace""></div>")

        str.Append("<div class="""">")
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
                str.Append(DrawProductTypesEntry(row_id, prod_type_list(k).Title, prod_type_list(k).Id, Utilities.IsDefaultXmlConfig(prod_type_list(k).Id, active_prod_list.ToArray())))
                row_id = row_id + 1
            Else
                Dim cRow As New Collection
                cRow.Add(prod_type_list(k).Title, "xml_name")
                cRow.Add(prod_type_list(k).Id, "xml_id")
                addNew.Add(cRow)
            End If
        Next

        If (Not smartFormsRequired) Then
            str.Append(DrawProductTypesEntry(row_id, _MessageHelper.GetMessage("lbl Blank HTML"), 0, Utilities.IsHTMLDefault(active_prod_list.ToArray())))
        End If

        str.Append(DrawContentTypesFooter())
        str.Append("</div>")

        If (row_id Mod 2 = 0) Then
            str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""1"" />")
        Else
            str.Append("<input type=""hidden"" name=""isEven"" id=""isEven"" value=""0"" />")
        End If
        ltr_vf_smartforms.Text = str.ToString()
    End Sub
#End Region

#Region "flagging section"
    Private Sub DrawFlaggingOptions()
        Dim str As New StringBuilder()

        Try
            str.Append("<input type=""checkbox"" id=""flagging_options_inherit_cbx"" name=""flagging_options_inherit_cbx"" disabled=""disabled"" " + IIf(_FolderData.FlagInherited And (Not (_FolderData.Id = 0)), "checked=""checked"" ", "") + " onclick=""InheritFlagingChanged()"" />" & Me._MessageHelper.GetMessage("lbl Inherit Parent Configuration"))
            str.Append("<div class=""ektronTopSpace""></div>")
            'If ((Not folder_data.FlagInherited) Or (folder_data.Id = 0)) Then
            '             str.Append("<table width=""100%"" >" + Environment.NewLine)
            '             str.Append("  <tr>" + Environment.NewLine)
            '             str.Append("    <td>" + Environment.NewLine)
            '             str.Append("      <table width=""100%"">" + Environment.NewLine)
            '             str.Append("        <tr>" + Environment.NewLine)
            '             str.Append("          <td>" + Environment.NewLine)
            '             str.Append("            <table class=""center"" width=""100%"">" + Environment.NewLine)
            '             str.Append("              <tr>" + Environment.NewLine)
            '             str.Append("                <td width=""50%"">" + Environment.NewLine)
            '             str.Append("                  <table width=""100%"">" + Environment.NewLine)
            '             str.Append("                    <tr>" + Environment.NewLine)
            '             str.Append("                      <td width=""50%"">" + Environment.NewLine)
            '             str.Append("                        " & m_refMsg.GetMessage("lbl assigned flags") & ": " & Environment.NewLine)
            '             str.Append("                        <select name=""flagging_options_assigned"" id=""flagging_options_assigned"" multiple=""multiple""" + Environment.NewLine)
            '             str.Append("                           disabled=""disabled"" size=""3"" style=""width: 100%"">" + Environment.NewLine)
            '             '
            '             ' Generate an option for each assigned flag:
            '             str.Append(GetAssignedFlags(True) + Environment.NewLine)
            '             str.Append("                        </select>" + Environment.NewLine)
            '             str.Append("                      </td>" + Environment.NewLine)
            '             str.Append("                      <td class=""center"">" + Environment.NewLine)
            '             str.Append("                      <td>" + Environment.NewLine)
            '             str.Append("                      </td>" + Environment.NewLine)
            '             str.Append("                    </tr>" + Environment.NewLine)
            '             str.Append("                  </table>" + Environment.NewLine)
            '             str.Append("                </td>" + Environment.NewLine)
            '             str.Append("              </tr>" + Environment.NewLine)
            '             str.Append("            </table>" + Environment.NewLine)
            '             str.Append("          </td>" + Environment.NewLine)
            '             str.Append("        </tr>" + Environment.NewLine)
            '             str.Append("      </table>" + Environment.NewLine)
            '             str.Append("    </td>" + Environment.NewLine)
            '             str.Append("  </tr>" + Environment.NewLine)
            '             str.Append("</table>" + Environment.NewLine)
            '         End If
            If (_FolderData.FolderFlags IsNot Nothing AndAlso _FolderData.FolderFlags.Length > 0) Then
                str.Append(_FolderData.FolderFlags(0).Name)
            Else
                str.Append("No flags assigned")
            End If

            flagging_options.Text = str.ToString

        Catch ex As Exception
        Finally
            str = Nothing
        End Try
    End Sub

    Protected Function GetAssignedFlags(Optional ByVal showDefault As Boolean = False) As String
        Dim result As New StringBuilder()
        Dim flags() As FolderFlagDefData
        Dim flag As FolderFlagDefData
        Dim tempFlag As FolderFlagDefData
        Dim idx As Integer = 0

        Try
            flags = _FolderData.FolderFlags ' flags = m_refContentApi.GetAllFolderFlagDef(folder_data.Id)

            ' reorder, placing the default first:
            For idx = 1 To flags.Length - 1
                If (flags(idx).IsDefault()) Then
                    tempFlag = flags(idx)
                    flags(idx) = flags(0)
                    flags(0) = tempFlag
                End If
            Next

            For Each flag In flags
                If (showDefault AndAlso (flag.IsDefault)) Then
                    result.Append("                          <option value=""" + flag.ID.ToString + """>" + flag.Name + " (default)" + "</option>" + Environment.NewLine)
                Else
                    result.Append("                          <option value=""" + flag.ID.ToString + """>" + flag.Name + "</option>" + Environment.NewLine)
                End If
            Next

        Catch ex As Exception
        Finally
            GetAssignedFlags = result.ToString
            result = Nothing
        End Try
    End Function

#End Region

#Region "multi-template selection"
    Private Function DrawContentTemplatesBreaker(ByVal checked As Boolean) As String
        If (_FolderData.Id = 0) Then
            Return "<input name=""TemplateTypeBreak"" id=""TemplateTypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TemplateTypeBreak')"" disabled />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        ElseIf (checked) Then
            Return "<input name=""TemplateTypeBreak"" id=""TemplateTypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TemplateTypeBreak')"" checked disabled />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        Else
            Return "<input name=""TemplateTypeBreak"" id=""TemplateTypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TemplateTypeBreak')"" disabled />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration")
        End If
    End Function

    Private Function DrawContentTemplatesHeader() As String
        Dim str As New StringBuilder()
        str.Append("<table width=""100%"" class=""ektronGrid""><tbody id=""templateTable"">")
        str.Append("<tr class=""title-header"">")
        str.Append("<td width=""10%"" class=""center"">")
        str.Append(_MessageHelper.GetMessage("lbl default"))
        str.Append("</td>")
        str.Append("<td width=""90%"" class=""left"">")
        str.Append(_MessageHelper.GetMessage("lbl Page Template Name"))
        str.Append("</td>")
        str.Append("</tr>")
        'str.Append("</tbody></table>")
        'str.Append("<table width=""100%"" class=""ektronGrid""><tbody >")
        Return str.ToString()
    End Function

    Private Function DrawContentTemplatesEntry(ByVal row_id As Integer, ByVal name As String, ByVal template_id As Long, ByVal isEnabled As Boolean) As String
        Dim str As New StringBuilder()
        Dim k As Integer = 0
        Dim isDefault As Boolean = False

        If (template_id = _FolderData.TemplateId) Then
            isDefault = True
        End If

        str.Append("<tr id=""trow_" & template_id & """>")

        str.Append("<td width=""10%"" class=""center"">")
        If (isDefault = "1" And isEnabled) Then
            str.Append("<input type=""radio"" id=""tdefault"" name=""tdefault"" value=""" & name & """ checked disabled />")
        ElseIf (isDefault = "1" And Not isEnabled) Then
            str.Append("<input type=""radio"" id=""tdefault"" name=""tdefault"" value=""" & name & """ checked disabled />")
        ElseIf (Not isEnabled) Then
            str.Append("<input type=""radio"" id=""tdefault"" name=""tdefault"" value=""" & name & """ disabled />")
        Else
            str.Append("<input type=""radio"" id=""tdefault"" name=""tdefault"" value=""" & name & """ disabled />")
        End If

        str.Append("</td>")
        str.Append("<td width=""90%"" colspan=""2"">")
        str.Append(name)
        Dim wfd As PageBuilder.WireframeData = New Ektron.Cms.PageBuilder.WireframeModel().FindByTemplateID(template_id)
        If (wfd) IsNot Nothing Then
            If (wfd.Template.SubType = EkEnumeration.TemplateSubType.Wireframes) Then
                str.Append(" (" & _MessageHelper.GetMessage("lbl pagebuilder wireframe template") & ")")
            ElseIf (wfd.Template.SubType = EkEnumeration.TemplateSubType.MasterLayout) Then
                str.Append(" (" & _MessageHelper.GetMessage("lbl pagebuilder master layouts") & ")")
            End If
        End If
        str.Append("<input id=""tinput_" & template_id & """ name=""tinput_" & template_id & """ type=""hidden"" value=""" & template_id & """ />")
        str.Append("</td>")
        str.Append("</tr>")

        Return str.ToString()
    End Function

    Private Function DrawContentTemplatesFooter() As String
        Return "</tbody></table>"
    End Function

    Private Sub DrawContentTemplatesTable()
        Dim active_templates As TemplateData()
        active_templates = _ContentApi.GetEnabledTemplatesByFolder(_FolderData.Id)

        Dim template_data As TemplateData()
        template_data = _ContentApi.GetAllTemplates("TemplateFileName")

        Dim k As Integer = 0
        Dim row_id As Integer = 0
        Dim addNew As New Collection()

        Dim broken As Boolean = False
        If (active_templates.Length > 0) Then
            broken = True
        End If

        Dim isEnabled As Boolean = IsInheritingTemplateMultiConfig()

        Dim str As New StringBuilder()

        str.Append(DrawContentTemplatesBreaker(isEnabled))
        str.Append("<div class=""ektronTopSpace""></div>")

        str.Append("<div>")
        str.Append(DrawContentTemplatesHeader())

        DrawFlaggingOptions()

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
                str.Append(DrawContentTemplatesEntry(row_id, template_data(k).FileName, template_data(k).Id, Not isEnabled))
                row_id = row_id + 1
            Else
                Dim cRow As New Collection
                cRow.Add(template_data(k).FileName, "template_name")
                cRow.Add(template_data(k).Id, "template_id")
                addNew.Add(cRow)
            End If
        Next

        str.Append(DrawContentTemplatesFooter())
        str.Append("</div>")

        If (row_id Mod 2 = 0) Then
            str.Append("<input type=""hidden"" name=""tisEven"" id=""tisEven"" value=""1"" />")
        Else
            str.Append("<input type=""hidden"" name=""tisEven"" id=""tisEven"" value=""0"" />")
        End If
        template_list.Text = str.ToString()
    End Sub
    Private checktaxid As Long = 0
    Private Sub DrawFolderTaxonomyTable()
        Dim categorydatatemplate As String = "<input type=""checkbox"" id=""taxlist"" name=""taxlist"" value=""{0}"" {1} disabled/>{2}"
        Dim categorydata As New StringBuilder
        Dim TaxArr As TaxonomyBaseData() = _ContentApi.EkContentRef.GetAllTaxonomyByConfig(Common.EkEnumeration.TaxonomyType.Content)
        If (TaxArr IsNot Nothing AndAlso TaxArr.Length > 0) Then
            Dim i As Integer = 0
            While (i < TaxArr.Length)
                For j As Integer = 0 To 2
                    If (i < TaxArr.Length) Then
                        checktaxid = TaxArr(i).TaxonomyId
                        categorydata.Append(String.Format(categorydatatemplate, TaxArr(i).TaxonomyId, IsChecked(Array.Exists(_FolderData.FolderTaxonomy, AddressOf TaxonomyExists)), TaxArr(i).TaxonomyName))
                    Else
                        Exit For
                    End If
                    i += 1
                    categorydata.Append("<br/>")
                Next
            End While
        End If

        Dim str As New StringBuilder()
        str.Append("<input name=""TaxonomyTypeBreak"" id=""TaxonomyTypeBreak"" type=""checkbox"" onclick=""ToggleMultiXmlTemplateInherit('TaxonomyTypeBreak')"" " & IsChecked(_FolderData.TaxonomyInherited) & " disabled />" & _MessageHelper.GetMessage("lbl Inherit Parent Configuration"))
        str.Append("<br/>")
        str.Append("<input name=""CategoryRequired"" id=""CategoryRequired"" type=""checkbox"" " & IsChecked(_FolderData.CategoryRequired) & "  disabled />" & _MessageHelper.GetMessage("alt Required at least one category selection"))
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
    Private Function IsInheritingXmlMultiConfig() As Boolean
        Dim isInheriting As Boolean = _ContentApi.IsInheritingXmlMultiConfig(_FolderData.Id)
        Return isInheriting
    End Function
    Private Function IsInheritingTemplateMultiConfig() As Boolean
        Dim isInheriting As Boolean = _ContentApi.IsInheritingTemplateMultiConfig(_FolderData.Id)
        Return isInheriting
    End Function
#End Region

End Class
