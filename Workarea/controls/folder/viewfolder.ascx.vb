Imports System
Imports System.Data
Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Site
Imports Ektron.Cms.DataIO.LicenseManager
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common

Partial Class viewfolder
    Inherits System.Web.UI.UserControl

#Region "Member Variables"

    Public Const _ContentTypeUrlParam As String = Ektron.Cms.Common.EkConstants.ContentTypeUrlParam
    Public Const _CMSContentType_AllTypes As Integer = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes
    Public Const _ManagedAsset_Max As Integer = Ektron.Cms.Common.EkConstants.ManagedAsset_Max
    Public Const _ManagedAsset_Min As Integer = Ektron.Cms.Common.EkConstants.ManagedAsset_Min

    Public _ContentApi As ContentAPI
    Public _MessageHelper As Ektron.Cms.Common.EkMessageHelper

    Private _AppImgPath As String = ""
    Private _AppPath As String = ""
    Private _ApplicationPath As String
    Private _AssetInfoData As AssetInfoData()
    Private _BlogData As BlogData = Nothing
    Private _ChangeLanguage As Boolean = False
    Private _ContentData As ContentData
    Private _ContentId As Long = 0
    Private _ContentLanguage As Integer = -1
    Private _ContentSubTypeSelected As Ektron.Cms.Common.EkEnumeration.CMSContentSubtype
    Private _ContentType As Integer = 0
    Private _ContentTypeQuerystringParam As String
    Private _ContentTypeSelected As String = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes
    Private _Comments As Ektron.Cms.Content.EkTasks
    Private _CurrentUserId As Long = 0
    Private _DiscussionBoard As DiscussionBoard
    Private _DiscussionForums As DiscussionForum()
    Private _EkContent As Ektron.Cms.Content.EkContent
    Private _EkContentCol As Ektron.Cms.Common.EkContentCol
    Private _EnableMultilingual As Integer = 0
    Private _FolderData As FolderData
    Private _FolderType As Integer = 0
    Private _From As String = ""
    Private _HasXmlConfig As Boolean = False
    Private _Id As Long = 0
    Private _CheckedInOrApproved As Boolean = False
    Private _IsMac As Boolean
    Private _IsMyBlog As Boolean = False
    Private _LocalizationApi As New LocalizationAPI()
    Private _NextActionType As String = ""
    Private _OrderBy As String = ""
    Private _PageData As Collection
    Private _PagingCurrentPageNumber As Integer = 1
    Private _PagingTotalPagesNumber As Integer = 1
    Private _PagingPageSize As Integer = 20
    Private _PageAction As String = ""
    Private _PostID As Long = 0
    Private _PermissionData As PermissionData
    Private _SelectedEditControl As String
    Private _SitePath As String = ""
    Private _StyleHelper As New StyleHelper
    Private _TakeAction As Boolean = False
    Private _Task As Ektron.Cms.Content.EkTask
    Private _TreeViewId As String
    Private _UserApi As New UserAPI
    Private _ViewImagePath As String = "images/UI/Icons/folderView.png"
    Private _XmlConfigID As Long = 0
    Private _XmlConfigType As String = "EkXmlConfigType"
    Protected _initIsFolderAdmin As Boolean = False
    Protected _isFolderAdmin As Boolean = False
    Protected _initIsCopyOrMoveAdmin As Boolean = False
    Protected _isCopyOrMoveAdmin As Boolean = False
    Protected _IsArchivedEvent As Boolean = False
    Protected _BoardID As Long = 0
#End Region

#Region "Properties"

    Public Property ApplicationPath() As String
        Get
            Return _ApplicationPath
        End Get
        Set(ByVal Value As String)
            _ApplicationPath = Value
        End Set
    End Property

#End Region

#Region "Events"

    Public Sub New()

        'set contentapi
        _ContentApi = New ContentAPI()
        _MessageHelper = _ContentApi.EkMsgRef

        'set ApplicationPath property
        Dim endSlash() As Char = {"/"}
        Me.ApplicationPath = _ContentApi.ApplicationPath.TrimEnd(endSlash)
        Me._SitePath = _ContentApi.SitePath.TrimEnd(endSlash)

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        'register page components
        Me.RegisterJS()
        Me.RegisterCSS()

        Me.GetQueryStringValues()
        Dim contentLanguage As String = Me.GetQueryStringValue("LangType")
        Utilities.SetLanguage(_ContentApi)
        _ContentLanguage = IIf(contentLanguage = String.Empty, _ContentApi.ContentLanguage, contentLanguage)
        _EkContent = _ContentApi.EkContentRef

        If Not _ContentApi.IsLoggedIn OrElse Not _ContentApi.LoadPermissions(0, "users").IsLoggedIn Then
            Response.Redirect("reterror.aspx?info=" + _MessageHelper.GetMessage("lbl not logged in"), True)
        End If

        _FolderData = _ContentApi.GetFolderById(_Id)
        If (_FolderData Is Nothing) Then
            Response.Redirect("reterror.aspx?info=" + _MessageHelper.GetMessage("com: folder does not exist"), True)
            Exit Sub
        End If
        If (_FolderData.FolderType = FolderType.Calendar) Then
            Dim foldersource As New Ektron.Cms.Controls.CalendarDataSource
            foldersource.defaultId = _Id
            foldersource.sourceType = Ektron.Cms.Controls.SourceType.SystemCalendar
            calendardisplay.DataSource.Clear()
            calendardisplay.DataSource.Add(foldersource)
            calendardisplay.LanguageID = _ContentLanguage
            calendardisplay.AllowEventEditing = IIf(_PageAction = "viewarchivecontentbycategory", False, True)
            calendardisplay.Fill()

        End If

    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'retrieve querystring values
            _IsMac = IIf(Request.Browser.Platform.IndexOf("Win") = -1, True, False)
            _SelectedEditControl = Utilities.GetEditorPreference(Request)
            _ChangeLanguage = False

            Dim uniqueKey As String = _ContentApi.UserId & _ContentApi.UniqueId & "RejectedFiles"
            If (Session(uniqueKey) IsNot Nothing) AndAlso (Session(uniqueKey).ToString().Length > 0) Then
                Dim failedUpload As String = Session(uniqueKey)
                failedUpload = failedUpload.Replace("\", "\\")
                failedUpload = failedUpload.Replace("'", "\'")
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "DisplayFailedUploads", "alert('" & _MessageHelper.GetMessage("lbl error message for multiupload") & " " & failedUpload & "\n" & _MessageHelper.GetMessage("js:cannot add file with add and plus") & "');", True)
                Session.Remove(uniqueKey)
            End If

            addContentLanguageMessage.Text = _MessageHelper.GetMessage("alert msg add content lang")
            errorLinksDisabled.Text = _MessageHelper.GetMessage("js err links disabled")
            dropuploader.Text = ""
            If (IsPostBack() = False Or (Request.Form.Count > 0 And Request.Form(hdnIsPostData.UniqueID) <> "") Or _ChangeLanguage) Then
                Select Case _PageAction
                    Case "viewarchivecontentbycategory", "viewcontentbycategory"
                        Select Case _TreeViewId
                            Case "0"
                                ViewContentByCategory()
                            Case "-1"
                                ViewTaxonomyContentByCategory()
                            Case "-2"
                                ViewCollectionContentByCategory()
                            Case Else
                                ViewContentByCategory()
                        End Select
                End Select
            End If

            Me.hdnIsPostData.Value = "true"

            'set paging ui
            If _PagingTotalPagesNumber > 1 Then
                Me.SetPagingUI()
            Else
                divPaging.Visible = False
            End If

        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & _ContentLanguage, False)
        End Try
    End Sub

    Public Sub AdHocPaging_Click(ByVal sender As Object, ByVal eventArgs As System.Web.UI.WebControls.CommandEventArgs)
        'Do nothing, handled client-side
    End Sub

    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                _PagingCurrentPageNumber = 1
            Case "Last"
                _PagingCurrentPageNumber = Int32.Parse(TotalPages.Text)
            Case "Next"
                _PagingCurrentPageNumber = IIf(Int32.Parse(CurrentPage.Text) + 1 <= Int32.Parse(TotalPages.Text), Int32.Parse(CurrentPage.Text) + 1, Int32.Parse(CurrentPage.Text))
            Case "Prev"
                _PagingCurrentPageNumber = IIf(Int32.Parse(CurrentPage.Text) - 1 >= 1, Int32.Parse(CurrentPage.Text) - 1, Int32.Parse(CurrentPage.Text))
        End Select
        CurrentPage.Text = _PagingCurrentPageNumber
        ViewContentByCategory()
        'set paging ui, postback is true here, so no need to check for IsPostBack()
        If _PagingTotalPagesNumber > 1 Then
            Me.SetPagingUI()
        Else
            divPaging.Visible = False
        End If
        Me.hdnIsPostData.Value = "true"
    End Sub

#End Region

#Region "FOLDER - ViewContentByCategory OR ViewArchiveContentByCategory"

    Private Function IsFolderAdmin() As Boolean
        If (_initIsFolderAdmin) Then
            Return _isFolderAdmin
        End If
        _isFolderAdmin = _ContentApi.IsARoleMemberForFolder_FolderUserAdmin(_Id)
        _initIsFolderAdmin = True
        Return _isFolderAdmin
    End Function

    Private Function IsCopyOrMoveAdmin() As Boolean
        If (_initIsCopyOrMoveAdmin) Then
            Return _isCopyOrMoveAdmin
        End If
        _isCopyOrMoveAdmin = _ContentApi.IsARoleMemberForFolder(EkEnumeration.CmsRoleIds.MoveOrCopy, _Id, _ContentApi.UserId)
        _initIsCopyOrMoveAdmin = True
        Return _isCopyOrMoveAdmin
    End Function

    Public Function ViewContentByCategory() As Boolean

        _CurrentUserId = _ContentApi.UserId
        _AppImgPath = _ContentApi.AppImgPath
        _AppPath = _ContentApi.AppPath
        _SitePath = _ContentApi.SitePath
        _EnableMultilingual = _ContentApi.EnableMultilingual

        url_action.Text = _PageAction
        url_id.Text = _Id

        If (_FolderData Is Nothing) Then
            _FolderData = _ContentApi.GetFolderById(_Id)
        End If

        If (_FolderData Is Nothing) Then
            Response.Redirect("reterror.aspx?info=" + _MessageHelper.GetMessage("com: folder does not exist"), True)
            Exit Function
        Else
            If (Not IsNothing(_FolderData.XmlConfiguration)) Then
                _HasXmlConfig = True
            End If
            _PermissionData = _ContentApi.LoadPermissions(_Id, "folder")
            _FolderType = _FolderData.FolderType
        End If

        'Setting JS Variable for global use through workarea.aspx page.
        pasteFolderType.Text = _FolderData.FolderType
        pasteFolderId.Text = _FolderData.Id
        pasteParentId.Text = _FolderData.ParentId

        If Not Request.QueryString("IsArchivedEvent") Is Nothing AndAlso Request.QueryString("IsArchivedEvent") <> "" Then
            _IsArchivedEvent = Convert.ToBoolean(Request.QueryString("IsArchivedEvent"))
            is_archived.Text = _IsArchivedEvent
        End If

        _AssetInfoData = _ContentApi.GetAssetSupertypes()
        If (CMSContentType_Content = _ContentTypeSelected Or CMSContentType_Archive_Content = _ContentTypeSelected Or CMSContentType_XmlConfig = _ContentTypeSelected) Then
            _ContentType = CLng(_ContentTypeSelected)
        ElseIf (CMSContentType_Forms = _ContentTypeSelected Or CMSContentType_Archive_Forms = _ContentTypeSelected) Then
            _ContentType = CLng(_ContentTypeSelected)
        ElseIf (_ManagedAsset_Min <= _ContentTypeSelected And _ContentTypeSelected <= _ManagedAsset_Max) Then
            If DoesAssetSupertypeExist(_AssetInfoData, _ContentTypeSelected) Then
                _ContentType = CLng(_ContentTypeSelected)
            End If
        ElseIf (_ContentTypeSelected = _CMSContentType_AllTypes) Then
            _ContentType = CMSContentType_NonLibraryForms
        ElseIf _IsArchivedEvent = True AndAlso (_ContentTypeSelected = EkConstants.CMSContentType_Archive_ManagedFiles OrElse _ContentTypeSelected = EkConstants.CMSContentType_Archive_OfficeDoc OrElse _ContentTypeSelected = EkConstants.CMSContentType_Archive_MultiMedia) Then
            _ContentType = _ContentTypeSelected
        End If

        _ContentTypeSelected = _ContentType

        _PageData = New Collection
        _PageData.Add(_Id, "FolderID")
        If _FolderData.FolderType = 1 Then 'blog
            _PageData.Add("BlogPost", "OrderBy")
        Else
            _PageData.Add(_OrderBy, "OrderBy")
        End If
        _PageData.Add(_ContentLanguage, "m_intContentLanguage")
        Select Case _FolderData.FolderType
            Case FolderType.Blog
                _ContentType = CMSContentType_Content
                _PageData.Add(_ContentType, "ContentType")
            Case FolderType.DiscussionForum
                _ContentType = CMSContentType_Content
                _PageData.Add(_ContentType, "ContentType")
            Case Else
                If _ContentType > 0 Then
                    _PageData.Add(_ContentType, "ContentType")
                End If
        End Select

        If _ContentType = 1 AndAlso _ContentSubTypeSelected <> CMSContentSubtype.AllTypes Then
            _PageData.Add(_ContentSubTypeSelected, "ContentSubType")
        End If

        If _FolderData.FolderType = FolderType.Calendar Then
            calendardisplay.Visible = True
            pnlThreadedDiscussions.Visible = False
            If (Request.QueryString("showAddEventForm") IsNot Nothing AndAlso Request.QueryString("showAddEventForm") = "true") Then
                If (ViewState("AddEventFormDisplay") Is Nothing OrElse CBool(ViewState("AddEventFormDisplay")) = False) Then 'only show once
                    ViewState.Add("AddEventFormDisplay", True)
                    calendardisplay.ShowInsertForm(DateTime.Now.Date.AddHours(8))
                End If
            End If
            ScriptManager.RegisterClientScriptBlock(Page, GetType(UserControl), "CalendarCleanup", "try{ window.EditorCleanup(); }catch(ex){}", True)
        End If
        _PagingPageSize = _ContentApi.RequestInformationRef.PagingSize
        If (_FolderData.FolderType = FolderType.Blog) Then
            _BlogData = _ContentApi.BlogObject(_FolderData)
        End If

        'if it's a calendar then we do it on prerender
        If _FolderData.FolderType <> FolderType.Calendar Then
            If (_PageAction = "viewarchivecontentbycategory") Then
                _EkContentCol = _EkContent.GetAllViewArchiveContentInfov5_0(_PageData, _PagingCurrentPageNumber, _PagingPageSize, _PagingTotalPagesNumber)
                _NextActionType = "ViewContentByCategory"
            ElseIf (_PageAction = "viewcontentbycategory") Then
                _EkContentCol = _EkContent.GetAllViewableChildContentInfoV5_0(_PageData, _PagingCurrentPageNumber, _PagingPageSize, _PagingTotalPagesNumber)
                _NextActionType = "ViewArchiveContentByCategory"
            End If
            'paging goes here

            Dim i As Integer
            For i = 0 To _EkContentCol.Count - 1
                If (_EkContentCol.Item(i).ContentStatus = "A") Then
                    _TakeAction = True
                    _CheckedInOrApproved = True
                    Exit For
                Else
                    If (_EkContentCol.Item(i).ContentStatus = "I") Then
                        _CheckedInOrApproved = True
                    End If
                End If
            Next
        Else
            If (_PageAction = "viewarchivecontentbycategory") Then
                _NextActionType = "ViewContentByCategory"
            ElseIf (_PageAction = "viewcontentbycategory") Then
                _NextActionType = "ViewArchiveContentByCategory"
            End If
        End If

        Select Case _FolderData.FolderType
            Case FolderType.Catalog
                If (_PageAction = "viewarchivecontentbycategory") Then
                    _NextActionType = "ViewContentByCategory"
                ElseIf (_PageAction = "viewcontentbycategory") Then
                    _NextActionType = "ViewArchiveContentByCategory"
                End If

                Page.ClientScript.RegisterClientScriptBlock(GetType(String), "objselnotice", "<script type=""text/javascript"">var objSelSupertype = null;</script>")

                Dim CatalogManager As New CatalogEntry(_ContentApi.RequestInformationRef)
                Dim entryList As New System.Collections.Generic.List(Of EntryData)()
                Dim entryCriteria As New Ektron.Cms.Common.Criteria(Of EntryProperty)

                entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _Id)
                entryCriteria.PagingInfo.CurrentPage = _PagingCurrentPageNumber.ToString()
                entryCriteria.PagingInfo.RecordsPerPage = _ContentApi.RequestInformationRef.PagingSize

                If _ContentApi.RequestInformationRef.ContentLanguage > 0 Then entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, _ContentApi.RequestInformationRef.ContentLanguage)

                Select Case Me._ContentTypeQuerystringParam
                    Case "0"
                        Dim IdList(2) As Long
                        IdList(0) = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product
                        IdList(1) = Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct
                        entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.In, IdList)
                    Case "2"
                        entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit)
                    Case "3"
                        entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle)
                    Case "4"
                        entryCriteria.AddFilter(EntryProperty.EntryType, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
                End Select

                If _PageAction = "viewarchivecontentbycategory" Then entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, True) Else entryCriteria.AddFilter(EntryProperty.IsArchived, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, False)

                entryCriteria.OrderByDirection = OrderByDirection.Ascending

                Select Case _OrderBy.ToLower()
                    Case "language"
                        entryCriteria.OrderByField = EntryProperty.LanguageId
                    Case "id"
                        entryCriteria.OrderByField = EntryProperty.Id
                    Case "status"
                        entryCriteria.OrderByField = EntryProperty.ContentStatus
                    Case "entrytype"
                        entryCriteria.OrderByField = EntryProperty.EntryType
                    Case "sale"
                        entryCriteria.OrderByField = EntryProperty.SalePrice
                    Case "list"
                        entryCriteria.OrderByField = EntryProperty.ListPrice
                    Case Else '"title"
                        entryCriteria.OrderByField = EntryProperty.Title
                End Select

                entryList = CatalogManager.GetList(entryCriteria)

                For j As Integer = 0 To entryList.Count - 1
                    If (entryList(j).Status = "A") Then
                        _TakeAction = True
                        _CheckedInOrApproved = True
                        Exit For
                    Else
                        If (entryList(j).Status = "I") Then
                            _CheckedInOrApproved = True
                        End If
                    End If
                Next

                _PagingTotalPagesNumber = entryCriteria.PagingInfo.TotalPages

                'paging goes here

                ViewCatalogToolBar(entryList.Count)
                Populate_ViewCatalogGrid(_EkContentCol, entryList)
                _ContentType = CLng(_ContentTypeSelected)
            Case FolderType.Blog
                _IsMyBlog = IIf(_BlogData.Id = _ContentApi.GetUserBlog(_ContentApi.UserId), True, False)
                Page.ClientScript.RegisterClientScriptBlock(GetType(String), "objselnotice", "<script type=""text/javascript"">var objSelSupertype = null;</script>")
                If Request.QueryString("ContType") <> "" AndAlso Request.QueryString("ContType") = CMSContentType_BlogComments Then
                    _ContentType = CInt(Request.QueryString("ContType"))
                    _Task = _ContentApi.EkTaskRef
                    If Request.QueryString("contentid") <> "" Then
                        _PostID = Request.QueryString("contentid")
                        _ContentData = _ContentApi.GetContentById(_PostID)
                        _Comments = _Task.GetTasks(_PostID, , , CMSTaskItemType.BlogCommentItem, "postcomment", )
                    Else
                        _Comments = _Task.GetTasks(, , , CMSTaskItemType.BlogCommentItem, , )
                    End If
                    ViewBlogContentByCategoryToolBar()
                    Populate_ViewBlogCommentsByCategoryGrid(_Comments)
                Else
                    Dim BlogPostCommentTally As Hashtable = New Hashtable
                    BlogPostCommentTally = _EkContent.TallyCommentsForBlogPosts(_Id)
                    ViewBlogContentByCategoryToolBar()
                    Populate_ViewBlogPostsByCategoryGrid(_EkContentCol, BlogPostCommentTally)
                End If
            Case FolderType.Media
                Page.ClientScript.RegisterClientScriptBlock(GetType(String), "objselnotice", "<script type=""text/javascript"">var objSelSupertype = null;</script>")
                ViewContentByCategoryToolBar()
                Populate_ViewMediaGrid(_EkContentCol)
            Case FolderType.DiscussionBoard
                Page.ClientScript.RegisterClientScriptBlock(GetType(String), "objselnotice", "<script type=""text/javascript"">var objSelSupertype = null;</script>")
                ViewDiscussionBoardToolBar()
                Populate_ViewDiscussionBoardGrid()
            Case FolderType.DiscussionForum
                Page.ClientScript.RegisterClientScriptBlock(GetType(String), "objselnotice", "<script type=""text/javascript"">var objSelSupertype = null;</script>")
                Dim bCanModerate As Boolean = False
                Dim itotalpages As Integer = 1
                If Request.QueryString("ContType") <> "" AndAlso Request.QueryString("ContType") = CMSContentType_BlogComments Then
                    _ContentType = CInt(Request.QueryString("ContType"))
                    If Me._ContentApi.UserId > 0 AndAlso ((Not (_PermissionData Is Nothing) AndAlso _PermissionData.CanAddToImageLib = True) Or _PermissionData.IsAdmin) Then
                        bCanModerate = True
                    End If
                    _DiscussionBoard = _ContentApi.GetTopic(_ContentId, True)
                    _BoardID = _DiscussionBoard.Id
                    _ContentData = DirectCast(_DiscussionBoard.Forums(0).Topics(0), ContentData)
                    _PermissionData = _ContentApi.LoadPermissions(_ContentId, "content", ContentAPI.PermissionResultType.All)
                    ViewRepliesToolBar()
                    _Task = _ContentApi.EkTaskRef
                    If Request.QueryString("contentid") <> "" Then
                        _PostID = Request.QueryString("contentid")
                        _Comments = _Task.GetTopicReplies(_PostID, _DiscussionBoard.Id, 1, 0, 0, itotalpages, bCanModerate)
                    Else
                        _Comments = _Task.GetTasks(, , , CMSTaskItemType.TopicReplyItem, , )
                    End If
                    Populate_ViewTopicRepliesGrid(_Comments)
                Else
                    Dim ForumPostCommentTally As New ArrayList
                    Dim thisboard As DiscussionBoard
                    Dim bModerator As Boolean = False
                    If _PermissionData.IsAdmin = True Or _PermissionData.CanAddToImageLib = True Then
                        bModerator = True
                    End If
                    thisboard = _EkContent.GetForumbyID(_Id, bModerator, _PagingCurrentPageNumber, Me._PagingTotalPagesNumber, "", Me._ContentApi.RequestInformationRef.PagingSize)

                    'paging here

                    ForumPostCommentTally = _EkContent.GetRepliesForTopics(_Id)
                    ViewDiscussionForumToolBar()
                    Populate_ViewForumPostsByCategoryGrid(thisboard.Forums(0).Topics, ForumPostCommentTally)
                End If
            Case FolderType.Calendar
                ViewCalendarToolBar()
            Case Else
                ViewContentByCategoryToolBar()
                Populate_ViewContentByCategoryGrid(_EkContentCol)
        End Select

        Util_SetJs()
    End Function

    Public Function GetAddMultiType() As Long
        ' gets ID for "add multiple" asset type
        GetAddMultiType = 0
        Dim count As Integer
        _AssetInfoData = _ContentApi.GetAssetSupertypes()
        If (Not _AssetInfoData Is Nothing) Then

            For count = 0 To _AssetInfoData.Length - 1
                If (_ManagedAsset_Min <= _AssetInfoData(count).TypeId And _AssetInfoData(count).TypeId <= _ManagedAsset_Max) Then
                    If "*" = _AssetInfoData(count).PluginType Then
                        GetAddMultiType = _AssetInfoData(count).TypeId
                    End If
                End If
            Next
        End If
    End Function
    Private Sub ViewCalendarToolBar()
        Dim result As New System.Text.StringBuilder
        Dim altText As String = ""
        Dim ParentId As Long = _FolderData.ParentId
        Dim count As Integer = 0
        Dim lAddMultiType As Integer = 0
        Dim bSelectedFound As Boolean = False
        Dim bShowAddMenu As Boolean = True
        Dim bViewContent As Boolean = ("viewcontentbycategory" = _PageAction)   ' alternative is archived content

        If (bViewContent) Then
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view events in calendar msg") & " """ & _FolderData.Name & """") & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' />"
            altText = _MessageHelper.GetMessage("Archive Event Title")
        Else
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view archive event title") & " """ & _FolderData.Name & """") & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' />"
            altText = _MessageHelper.GetMessage("view event title")
        End If
        result.Append("<table><tr>" & vbCrLf)
        If ((_PermissionData.CanAdd And bViewContent) Or _PermissionData.IsReadOnly = True) Then

            If (_PermissionData.CanAdd And bViewContent) Then
                If Not bSelectedFound Then
                    _ContentType = _CMSContentType_AllTypes
                End If
            End If
        End If

        If (_PermissionData.CanAdd And Not _IsArchivedEvent) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'file');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'file');"" onmouseout=""this.className='menuRootItem'""><span class=""new"">" & _MessageHelper.GetMessage("lbl New") & "</span></td>")
        End If

        SetViewImage()
        If ((_PermissionData.CanAdd) And bViewContent) Or _PermissionData.IsReadOnly = True Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'view');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'view');"" onmouseout=""this.className='menuRootItem'""><span class=""folderView"">" & _MessageHelper.GetMessage("lbl View") & "</span></td>")
        End If

        If (_PermissionData.CanDeleteFolders And bViewContent And _Id <> 0) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'delete');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'delete');"" onmouseout=""this.className='menuRootItem'""><span class=""delete"">" & _MessageHelper.GetMessage("lbl Delete") & "</span></td>")
        End If

        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton("calendar_" & _PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")

        result.Append("<script type=""text/javascript"">" & Environment.NewLine)

        If (_PermissionData.CanAdd) Then
            result.Append("    var filemenu = new Menu( ""file"" );" & Environment.NewLine)
            result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/calendarAdd.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("add cal event") & """, function() { AddNewEvent(); } );" & Environment.NewLine)
            result.Append("    MenuUtil.add( filemenu );" & Environment.NewLine)
        End If

        If ((_PermissionData.CanAdd) And bViewContent) Or _PermissionData.IsReadOnly = True Then
            result.Append("    var viewmenu = new Menu( ""view"" );" & Environment.NewLine)
            If ((_PermissionData.CanEditFolders Or _PermissionData.CanEditApprovals) And bViewContent) OrElse IsFolderAdmin() Then
                result.Append("    viewmenu.addBreak();" & Environment.NewLine)
                result.Append("    viewmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/properties.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Folder Properties") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=ViewFolder&id=" & _Id & "' } );" & Environment.NewLine)
            End If

            If (bViewContent) Then
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentArchived.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("archive content title") & """, function() { window.location.href = 'content.aspx?action=" & _NextActionType & "&id=" & _Id & "&IsArchivedEvent=true" & "&LangType=" & _ContentLanguage & IIf(IsAssetContentType(_ContentTypeSelected, False), "&" & _ContentTypeUrlParam & "=" & MakeArchiveAssetContentType(_ContentTypeSelected), IIf(IsArchiveAssetContentType(_ContentTypeSelected), "&" & _ContentTypeUrlParam & "=" & MakeNonArchiveAssetContentType(_ContentTypeSelected), "")) & "' } );" & Environment.NewLine)
            Else
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentArchived.png" & "' />&nbsp;&nbsp;&nbsp;" & _MessageHelper.GetMessage("unarchive event title") & """, function() { window.location.href = 'content.aspx?action=" & _NextActionType & "&id=" & _Id & "&LangType=" & _ContentLanguage & IIf(IsAssetContentType(_ContentTypeSelected, False), "&" & _ContentTypeUrlParam & "=" & MakeArchiveAssetContentType(_ContentTypeSelected), IIf(IsArchiveAssetContentType(_ContentTypeSelected), "&" & _ContentTypeUrlParam & "=" & MakeNonArchiveAssetContentType(_ContentTypeSelected), "")) & "' } );" & Environment.NewLine)
            End If

            AddLanguageMenu(result)

            result.Append("    MenuUtil.add( viewmenu );" & Environment.NewLine)
        End If

        ' Delete Menu
        If (_PermissionData.CanDeleteFolders And bViewContent And _Id <> 0) Then
            result.Append("    var deletemenu = new Menu( ""delete"" );" & Environment.NewLine)
            result.Append("    deletemenu.addItem(""&nbsp;<img src='images/UI/Icons/folderDelete.png' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl delete calendar") & """, function() { if( ConfirmFolderDelete(" & _Id & ") ) { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=DoDeleteFolder&id=" & _Id & "&ParentID=" & ParentId & "'; }} );" & Environment.NewLine)
            result.Append("    MenuUtil.add( deletemenu );" & Environment.NewLine)
        End If

        result.Append("    </script>" & Environment.NewLine)
        result.Append("" & Environment.NewLine)

        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub ViewContentByCategoryToolBar()
        Dim result As New System.Text.StringBuilder
        Dim altText As String = ""
        Dim ParentId As Long = _FolderData.ParentId
        Dim count As Integer = 0
        Dim lAddMultiType As Integer = 0
        Dim bSelectedFound As Boolean = False
        Dim bShowAddMenu As Boolean = True
        Dim bViewContent As Boolean = ("viewcontentbycategory" = _PageAction)   ' alternative is archived content
        Dim wireframeModel As New Ektron.Cms.PageBuilder.WireframeModel()

        If (bViewContent) Then
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view contents of folder msg") & " """ & _FolderData.Name & """") & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' />"
            altText = _MessageHelper.GetMessage("Archive Content Title")
        Else
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view archive content title") & " """ & _FolderData.Name & """") & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' />"
            altText = _MessageHelper.GetMessage("view content title")
        End If
        result.Append("<table><tr>" & vbCrLf)
        If ((_PermissionData.CanAdd And bViewContent) Or _PermissionData.IsReadOnly = True) Then

            If (_PermissionData.CanAdd And bViewContent) Then
                If Not bSelectedFound Then
                    _ContentType = _CMSContentType_AllTypes
                End If
            End If
        End If

        If ((_PermissionData.CanAdd Or _PermissionData.CanAddFolders) And bViewContent) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'file');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'file');"" onmouseout=""this.className='menuRootItem'""><span class=""new"">" & _MessageHelper.GetMessage("lbl New") & "</span></td>")
        End If
        SetViewImage()
        If (((_PermissionData.CanAdd) Or _PermissionData.IsReadOnly)) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'view');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'view');"" onmouseout=""this.className='menuRootItem'""><span class=""folderView"">" & _MessageHelper.GetMessage("lbl View") & "</span></td>")
        End If

        Dim totalPages As Integer
        _ContentApi.GetChildContentByFolderId(_Id, False, "name", 1, totalPages, 1)

        If ((_PermissionData.CanDeleteFolders And bViewContent And _Id <> 0) Or ((bViewContent And (_PermissionData.IsAdmin OrElse IsFolderAdmin()) OrElse _PermissionData.CanDelete) And totalPages > 0)) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'delete');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'delete');"" onmouseout=""this.className='menuRootItem'""><span class=""delete"">" & _MessageHelper.GetMessage("lbl Delete") & "</span></td>")
        End If

        result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'action');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'action');"" onmouseout=""this.className='menuRootItem'""><span class=""action"">" & _MessageHelper.GetMessage("lbl Action") & "</span></td>")

        bShowAddMenu = True
        If _EnableMultilingual = 1 Then
            Dim m_refsite As New SiteAPI
            Dim language_data(0) As LanguageData
            language_data = m_refsite.GetAllActiveLanguages
        End If
        Dim active_xml_list As XmlConfigData()
        active_xml_list = _ContentApi.GetEnabledXmlConfigsByFolder(_FolderData.Id)
        Dim smartFormsRequired As Boolean = Not Utilities.IsNonFormattedContentAllowed(active_xml_list)
        Dim canAddAssets As Boolean = (_PermissionData.CanAdd Or _PermissionData.CanAddFolders) And bViewContent
        If (_ContentLanguage < 1 Or _FolderData.FolderType = FolderType.Blog Or _FolderData.FolderType = FolderType.DiscussionForum Or _FolderData.FolderType = FolderType.DiscussionBoard Or smartFormsRequired = True Or canAddAssets = False) Then
        Else
            If (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.DocumentManagement, False)) Then
                If (Request.Browser.Browser = "IE" AndAlso Request.UserAgent.Contains("Windows NT 6.0") AndAlso Request.ServerVariables("HTTPS") = "on") Then 'Vista IE Https then take the user to file upload since vista https is not supported by webdav
                    result.Append("<td>&nbsp;<a href=""" & _ContentApi.AppPath & "edit.aspx?id=" & _FolderData.Id & "&ContType=103&type=add&close=false&lang_id=" & _ContentLanguage.ToString() & "title=""" & _MessageHelper.GetMessage("lbl file upload") & """><img id=""DeskTopHelp"" title= """ & _MessageHelper.GetMessage("alt add assets text") & """ border=""0"" src=""images/UI/Icons/Import.png""/></a>&nbsp;</td>")
                Else
                    result.Append("<td>&nbsp;<a href=""" & _ContentApi.AppPath & "DragDropCtl.aspx?id=" & _Id.ToString() & "&lang_id=" & _ContentLanguage.ToString() & "&EkTB_iframe=true&height=120&width=500&refreshCaller=true&scrolling=false&modal=true"" class=""ek_thickbox"" title=""" & _MessageHelper.GetMessage("Document Management System") & """><img id=""DeskTopHelp"" title= """ & _MessageHelper.GetMessage("alt add assets text") & """ border=""0"" src=""images/UI/Icons/Import.png""/></a>&nbsp;</td>")
                End If
            End If
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton(_PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")

        result.Append("<script type=""text/javascript"">" & Environment.NewLine)
        result.Append("    var filemenu = new Menu( ""file"" );" & Environment.NewLine)

        If (_PermissionData.CanAddFolders OrElse (_ContentApi.IsARoleMember(CmsRoleIds.CommerceAdmin) AndAlso Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))) Then

            If _PermissionData.CanAddFolders Then

                If (Not _FolderData.IsCommunityFolder) Then
                    result.Append("    filemenu.addItem(""&nbsp;<img src='images/UI/Icons/folder.png' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Folder") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&type=folder&action=AddSubFolder&id=" & _Id & "' } );" & Environment.NewLine)
                End If

                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/tree/folderBlogClosed.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Blog") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=AddSubFolder&type=blog&id=" & _Id & "' } );" & Environment.NewLine)
                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/folderBoard.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Discussion Board") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=AddSubFolder&type=discussionboard&id=" & _Id & "' } );" & Environment.NewLine)
                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/folderCommunity.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Community Folder") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=AddSubFolder&type=communityfolder&id=" & _Id & "' } );" & Environment.NewLine)
                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/ui/icons/tree/folderCalendarClosed.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Calendar Folder") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=AddSubFolder&type=calendar&id=" & _Id & "' } );" & Environment.NewLine)

                If Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce) Then
                    result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/folderGreen.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl commerce catalog") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=AddSubFolder&type=catalog&id=" & _Id & "' } );" & Environment.NewLine)
                End If
                If (_Id = 0 And LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.MultiSite)) Then 'domain folder
                    result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/folderSite.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl site Folder") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&type=site&action=AddSubFolder&id=" & _Id & "' } );" & Environment.NewLine)
                End If
                result.Append("    filemenu.addBreak();" & Environment.NewLine)

            Else

                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/folderGreen.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl commerce catalog") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=AddSubFolder&type=catalog&id=" & _Id & "' } );" & Environment.NewLine)
                result.Append("    filemenu.addBreak();" & Environment.NewLine)

            End If

        End If

        If (_PermissionData.CanAdd) Then
            Dim active_templates As TemplateData()
            active_templates = _ContentApi.GetEnabledTemplatesByFolder(_FolderData.Id)
            Dim foundWireframe As Boolean = False
            Dim foundNormal As Boolean = False
            Dim foundmasterlayout As Boolean = False

            For Each t As TemplateData In active_templates
                If (t.SubType = TemplateSubType.Wireframes) Then
                    foundWireframe = True
                ElseIf (t.SubType = TemplateSubType.MasterLayout) Then
                    foundmasterlayout = True
                Else
                    foundNormal = True
                End If
                If (foundWireframe AndAlso foundNormal AndAlso foundmasterlayout) Then
                    Exit For
                End If
            Next
            If (Utilities.IsNonFormattedContentAllowed(active_xml_list) AndAlso foundNormal) Then
                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentHtml.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl html content") & """, function() { " & _StyleHelper.GetAddAnchorByContentType(_Id, CMSContentType_Content, True) & " } );" & Environment.NewLine)
                If ((Not _IsMac) AndAlso (Not (IsNothing(_AssetInfoData)))) Or ("ContentDesigner" = _SelectedEditControl) Then
                    result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentForm.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl html formsurvey") & """, function() { " & _StyleHelper.GetAddAnchorByContentType(_Id, CMSContentType_Forms) & " } );" & Environment.NewLine)
                End If
            End If

            If foundWireframe Or foundmasterlayout Then 'folder has a wireframe associated
                ' Register JS
                Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
                Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS)
                Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)
                Ektron.Cms.API.JS.RegisterJS(Me, _ContentApi.AppPath + "PageBuilder/Wizards/js/ektron.pagebuilder.wizards.js", "EktronPageBuilderWizardsJS")
                Ektron.Cms.API.JS.RegisterJS(Me, _ContentApi.AppPath + "PageBuilder/Wizards/js/wizardResources.aspx", "EktronPageBuilderWizardResourcesJS")

                ' register necessary CSS
                Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)
                Ektron.Cms.API.Css.RegisterCss(Me, _ContentApi.AppPath + "PageBuilder/Wizards/css/ektron.pagebuilder.wizards.css", "EktronPageBuilderWizardsCSS")

                If (foundWireframe Or foundmasterlayout) Then
                    Dim layoutstr As String
                    layoutstr = "tmpContLang = AddNewPage(); if (tmpContLang > 0) { Ektron.PageBuilder.Wizards.showAddPage({mode: 'add', folderId: " & _FolderData.Id & ", language: tmpContLang, fromWorkarea: true}) };"
                    result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/application/layout_content.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl pagebuilder layouts") & """, function() { " & layoutstr & " } );" & Environment.NewLine)
                End If

                If (foundWireframe AndAlso _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CreateMasterLayout)) Then
                    Dim masterstr As String
                    masterstr = "tmpContLang = AddNewPage(); if (tmpContLang > 0) { Ektron.PageBuilder.Wizards.showAddMasterPage({mode: 'add', folderId: " & _FolderData.Id & ", language: tmpContLang, fromWorkarea: true}) };"
                    result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/application/layout_content.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl pagebuilder master layouts") & """, function() { " & masterstr & " } );" & Environment.NewLine)
                End If
            End If

            If (Not _IsMac Or ("ContentDesigner" = _SelectedEditControl)) Then
                If (active_xml_list.Length > 0 And Utilities.IsNonFormattedContentAllowed(active_xml_list)) Then
                    If ((active_xml_list.Length = 1 AndAlso active_xml_list(0) Is Nothing) OrElse (active_xml_list.Length = 1 AndAlso active_xml_list(0).Id = 0)) Then

                    Else
                        result.Append("    var contentTypesMenu = new Menu( ""contentTypes"" );" & Environment.NewLine)
                        result.Append("    filemenu.addBreak();" & Environment.NewLine)
                        Dim k As Integer
                        For k = 0 To active_xml_list.Length - 1
                            If (active_xml_list(k).Id <> 0) Then
                                result.Append("    contentTypesMenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/icons/contentsmartform.png" & "'/>&nbsp;&nbsp;" & active_xml_list(k).Title & """, function() { " & _StyleHelper.GetTypeOverrideAddAnchor(_Id, active_xml_list(k).Id, CMSContentType_Content) & " } );" & Environment.NewLine)
                            End If
                        Next
                        'result.Append("    contentTypesMenu.addBreak();" & Environment.NewLine)
                        result.Append("    filemenu.addMenu(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/icons/contentsmartform.png" & "'/>&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl smart form") & """, contentTypesMenu);" & Environment.NewLine)
                    End If

                ElseIf (active_xml_list.Length > 0 And Not Utilities.IsNonFormattedContentAllowed(active_xml_list)) Then
                    result.Append("    filemenu.addBreak();" & Environment.NewLine)
                    Dim k As Integer
                    For k = 0 To active_xml_list.Length - 1
                        If (active_xml_list(k).Id <> 0) Then
                            result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/icons/contentsmartform.png" & "'/>&nbsp;&nbsp;" & active_xml_list(k).Title & """, function() { " & _StyleHelper.GetTypeOverrideAddAnchor(_Id, active_xml_list(k).Id, CMSContentType_Content) & " } );" & Environment.NewLine)
                        End If
                    Next
                End If
                result.Append("    filemenu.addBreak();" & Environment.NewLine)
            End If

            'If ((Not m_bIsMac) AndAlso (Not (IsNothing(asset_data))) AndAlso Utilities.IsNonFormattedContentAllowed(active_xml_list)) Then
            If ((Not (IsNothing(_AssetInfoData))) AndAlso Utilities.IsNonFormattedContentAllowed(active_xml_list)) Then
                If (_AssetInfoData.Length > 0) Then
                    For count = 0 To _AssetInfoData.Length - 1
                        If (_ManagedAsset_Min <= _AssetInfoData(count).TypeId And _AssetInfoData(count).TypeId <= _ManagedAsset_Max) Then
                            If "*" = _AssetInfoData(count).PluginType Then
                                lAddMultiType = _AssetInfoData(count).TypeId
                            End If
                        End If
                    Next
                    Dim imgsrc As String = String.Empty
                    Dim txtCommName As String = String.Empty
                    If (Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.DocumentManagement, False)) Then

                        imgsrc = "&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentDMSDocument.png" & "' />&nbsp;&nbsp;"
                        txtCommName = _MessageHelper.GetMessage("lbl dms documents")
                        result.Append("filemenu.addItem(""" & imgsrc & "" & txtCommName & """, function() { " & _StyleHelper.GetAddAnchorByContentType(_Id, 103) & " } );" & Environment.NewLine)
                        result.Append(" if (ShowMultipleUpload() && CheckSTSUpload()) {")
                        imgsrc = "&nbsp;<img valign='middle' src='" & "images/ui/icons/contentStack.png" & "' />&nbsp;&nbsp;"
                        txtCommName = _MessageHelper.GetMessage("lbl multiple documents")
                        result.Append("filemenu.addItem(""" & imgsrc & "" & txtCommName & """, function() { " & _StyleHelper.GetAddAnchorByContentType(_Id, 9876) & " } );" & Environment.NewLine)
                        result.Append("}")
                    End If
                End If
            End If
        End If
        If (_PermissionData.IsCollections _
          OrElse _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)) Then
            result.Append("    filemenu.addBreak();" & Environment.NewLine)
            result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/collection.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Collection") & """, function() { window.location.href = 'collections.aspx?LangType=" & _ContentLanguage & "&action=Add&folderid=" & _Id & "' } );" & Environment.NewLine)
            result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/menu.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Menu") & """, function() { window.location.href = 'collections.aspx?LangType=" & _ContentLanguage & "&action=AddMenu&folderid=" & _Id & "' } );" & Environment.NewLine)
            result.Append("" & Environment.NewLine)
        End If

        If (_PermissionData.CanAdd Or _PermissionData.CanAddFolders) Then
            result.Append("    MenuUtil.add( filemenu );" & Environment.NewLine)
        End If

        result.Append("    var viewmenu = new Menu( ""view"" );" & Environment.NewLine)

        If (bViewContent) Then
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/folderView.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl All Types"), 98) & """, function() { UpdateView(" & _CMSContentType_AllTypes & "); } );" & Environment.NewLine)
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentHtml.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl html content"), CMSContentType_Content, CMSContentSubtype.Content) & """, function() { UpdateViewwithSubtype(" & CMSContentType_Content & ", " & CMSContentSubtype.Content & ",false" & "); } );" & Environment.NewLine)
            If ((Not _IsMac) AndAlso (Not (IsNothing(_AssetInfoData)))) Or ("ContentDesigner" = _SelectedEditControl) Then
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentForm.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl html formsurvey"), CMSContentType_Forms) & """, function() { UpdateView(" & CMSContentType_Forms & "); } );" & Environment.NewLine)
            End If
            If wireframeModel.FindByFolderID(_FolderData.Id).Length > 0 Then 'folder has a wireframe associated
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/application/layout_content.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl pagebuilder layouts"), CMSContentType_Content, CMSContentSubtype.PageBuilderData) & """, function() { UpdateViewwithSubtype(" & CMSContentType_Content & ", " & CMSContentSubtype.PageBuilderData & ",false " & "); } );" & Environment.NewLine)
            End If
        Else
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/folderView.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl All Types"), 98) & """, function() { UpdateArchiveView(" & _CMSContentType_AllTypes & ",true " & "); } );" & Environment.NewLine)
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentHtml.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl html content"), CMSContentType_Content, CMSContentSubtype.Content) & """, function() { UpdateViewwithSubtype(" & CMSContentType_Content & ", " & CMSContentSubtype.Content & ",true " & "); } );" & Environment.NewLine)
            If ((Not _IsMac) AndAlso (Not (IsNothing(_AssetInfoData)))) Or ("ContentDesigner" = _SelectedEditControl) Then
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentForm.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl html formsurvey"), CMSContentType_Forms) & """, function() { UpdateArchiveView(" & CMSContentType_Forms & ",true " & "); } );" & Environment.NewLine)
            End If
            If wireframeModel.FindByFolderID(_FolderData.Id).Length > 0 Then 'folder has a wireframe associated
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/application/layout_content.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl pagebuilder layouts"), CMSContentType_Content, CMSContentSubtype.PageBuilderData) & """, function() { UpdateViewwithSubtype(" & CMSContentType_Content & ", " & CMSContentSubtype.PageBuilderData & ",true " & "); } );" & Environment.NewLine)
            End If
        End If




        result.Append("    viewmenu.addBreak();" & Environment.NewLine)
        If ((_PermissionData.CanAdd) And bViewContent) Or _PermissionData.IsReadOnly = True Then
            If (_AssetInfoData IsNot Nothing AndAlso Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.DocumentManagement, False)) Then
                If (_AssetInfoData.Length > 0) Then
                    For count = 0 To _AssetInfoData.Length - 1
                        If (_ManagedAsset_Min <= _AssetInfoData(count).TypeId And _AssetInfoData(count).TypeId <= _ManagedAsset_Max) Then
                            If "*" = _AssetInfoData(count).PluginType Then
                                lAddMultiType = _AssetInfoData(count).TypeId
                            Else
                                Dim imgsrc As String = String.Empty
                                Dim txtCommName As String = String.Empty
                                If _IsArchivedEvent Then
                                    If (_AssetInfoData(count).TypeId + 1000 = 1101) Then
                                        imgsrc = "&nbsp;<img src='" & "images/UI/Icons/FileTypes/word.png" & "' />&nbsp;&nbsp;"
                                        txtCommName = _MessageHelper.GetMessage("lbl Office Documents")
                                    ElseIf (_AssetInfoData(count).TypeId + 1000 = 1102) Then
                                        imgsrc = "&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentManagedFiles.png" & " ' />&nbsp;&nbsp;"
                                        txtCommName = _MessageHelper.GetMessage("lbl Managed Files")
                                    ElseIf (_AssetInfoData(count).TypeId + 1000 = 1104) Then
                                        imgsrc = "&nbsp;<img valign='middle' src='" & "images/UI/Icons/film.png" & " ' />&nbsp;&nbsp;"
                                        txtCommName = _MessageHelper.GetMessage("lbl Multimedia")
                                    Else
                                        imgsrc = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                                    End If
                                    If (_AssetInfoData(count).TypeId + 1000 <> 1105) Then
                                        result.Append("viewmenu.addItem(""" & imgsrc & "" & MakeBold(txtCommName, _AssetInfoData(count).TypeId + 1000) & """, function() { UpdateView(" & _AssetInfoData(count).TypeId + 1000 & "); } );" & Environment.NewLine)
                                    End If
                                Else

                                    If (_AssetInfoData(count).TypeId = 101) Then
                                        imgsrc = "&nbsp;<img src='" & "images/UI/Icons/FileTypes/word.png" & "' />&nbsp;&nbsp;"
                                        txtCommName = _MessageHelper.GetMessage("lbl Office Documents")
                                    ElseIf (_AssetInfoData(count).TypeId = 102) Then
                                        imgsrc = "&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentManagedFiles.png" & " ' />&nbsp;&nbsp;"
                                        txtCommName = _MessageHelper.GetMessage("lbl Managed Files")
                                    ElseIf (_AssetInfoData(count).TypeId = 104) Then
                                        imgsrc = "&nbsp;<img valign='middle' src='" & "images/UI/Icons/film.png" & " ' />&nbsp;&nbsp;"
                                        txtCommName = _MessageHelper.GetMessage("lbl Multimedia")
                                    Else
                                        imgsrc = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                                    End If
                                    If (_AssetInfoData(count).TypeId <> 105) Then
                                        result.Append("viewmenu.addItem(""" & imgsrc & "" & MakeBold(txtCommName, _AssetInfoData(count).TypeId) & """, function() { UpdateView(" & _AssetInfoData(count).TypeId & "); } );" & Environment.NewLine)
                                    End If
                                End If

                            End If
                        End If
                    Next
                End If
            End If

            AddLanguageMenu(result)

            result.Append("    viewmenu.addBreak();" & Environment.NewLine)


            If (bViewContent _
             And (_PermissionData.IsCollections _
              OrElse _ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu))) Then
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/menu.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Menu") & """, function() { window.location.href = 'collections.aspx?LangType=" & _ContentLanguage & "&action=ViewAllMenus&folderid=" & _Id & "' } );" & Environment.NewLine)
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/collection.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Collection") & """, function() { window.location.href = 'collections.aspx?LangType=" & _ContentLanguage & "&action=mainPage&folderid=" & _Id & "' } );" & Environment.NewLine)
            End If
            If (bViewContent) Then
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentArchived.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("archive content") & """, function() { window.location.href = 'content.aspx?action=" & _NextActionType & "&id=" & _Id & "&IsArchivedEvent=true" & "&LangType=" & _ContentLanguage & IIf(IsAssetContentType(_ContentTypeSelected, False), "&" & _ContentTypeUrlParam & "=" & MakeArchiveAssetContentType(_ContentTypeSelected), IIf(IsArchiveAssetContentType(_ContentTypeSelected), "&" & _ContentTypeUrlParam & "=" & MakeNonArchiveAssetContentType(_ContentTypeSelected), "")) & "' } );" & Environment.NewLine)
            Else
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentArchived.png" & "' />&nbsp;&nbsp;&nbsp;" & _MessageHelper.GetMessage("top Content") & """, function() { window.location.href = 'content.aspx?action=" & _NextActionType & "&id=" & _Id & "&LangType=" & _ContentLanguage & IIf(IsAssetContentType(_ContentTypeSelected, False), "&" & _ContentTypeUrlParam & "=" & MakeArchiveAssetContentType(_ContentTypeSelected), IIf(IsArchiveAssetContentType(_ContentTypeSelected), "&" & _ContentTypeUrlParam & "=" & MakeNonArchiveAssetContentType(_ContentTypeSelected), "")) & "' } );" & Environment.NewLine)
            End If

            If ((_PermissionData.CanEditFolders Or _PermissionData.CanEditApprovals) And bViewContent) _
             OrElse IsFolderAdmin() Then
                result.Append("    viewmenu.addBreak();" & Environment.NewLine)
                result.Append("    viewmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/properties.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Folder Properties") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=ViewFolder&id=" & _Id & "' } );" & Environment.NewLine)
            End If

            result.Append("    MenuUtil.add( viewmenu );" & Environment.NewLine)

            ' Delete Menu
            If ((_PermissionData.CanDeleteFolders And bViewContent And _Id <> 0) Or ((bViewContent And (_PermissionData.IsAdmin OrElse IsFolderAdmin()) OrElse _PermissionData.CanDelete) And totalPages > 0)) Then

                result.Append("    var deletemenu = new Menu( ""delete"" );" & Environment.NewLine)
                If (_PermissionData.CanDeleteFolders And bViewContent And _Id <> 0) Then
                    Dim folderImgPath As String = "images/UI/Icons/folderDelete.png"

                    Select Case _FolderType
                        Case Ektron.Cms.Common.EkEnumeration.FolderType.Domain
                            folderImgPath = "images/UI/Icons/folderSiteDelete.png"
                        Case Ektron.Cms.Common.EkEnumeration.FolderType.Community
                            folderImgPath = "images/UI/Icons/folderCommunityDelete.png"
                        Case Else
                            ' use the default.
                    End Select
                    result.Append("    deletemenu.addItem(""&nbsp;<img src='" & folderImgPath & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl This Folder") & """, function() { if( ConfirmFolderDelete(" & _Id & ") ) { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=DoDeleteFolder&id=" & _Id & "&ParentID=" & ParentId & "'; }} );" & Environment.NewLine)
                End If
                If (bViewContent _
                 And (_PermissionData.IsAdmin _
                  OrElse IsFolderAdmin()) OrElse _PermissionData.CanDelete) Then
                    ' get a count for the content in this folder
                    If (totalPages > 0) Then
                        If ((_EnableMultilingual = "1") And (CInt(_ContentLanguage) < 1)) Then
                            result.Append("    deletemenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/contentHtmlDelete.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("top Content") & """, function() { alert('A language must be selected!'); } );" & Environment.NewLine)
                        Else
                            '44595 - Delete content from the archive view should show up archived list rather than live content list.
                            If _PageAction = "viewarchivecontentbycategory" Then
                                result.Append("    deletemenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/contentDelete.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("top Content") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=DeleteContentByCategory&id=" & _Id & "&showarchive=true'; } );" & Environment.NewLine)
                            Else
                                result.Append("    deletemenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/contentDelete.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("top Content") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=DeleteContentByCategory&id=" & _Id & "'; } );" & Environment.NewLine)
                            End If
                        End If
                    End If
                End If
                result.Append("    MenuUtil.add( deletemenu );" & Environment.NewLine)
            End If
        End If

        result.Append("    var actionmenu = new Menu( ""action"" );" & Environment.NewLine)
        If (_ContentApi.IsARoleMember(CmsRoleIds.AdminXliff) And bViewContent AndAlso Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.Xliff, False)) Then
            result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/translationExport.png" & " ' />&nbsp;&nbsp;" & Me._MessageHelper.GetMessage("lbl Export for translation") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=Localize&backpage=ViewContentByCategory&id=" & _Id & "'; } );" & Environment.NewLine)
        End If

        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/magnifier.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("generic Search") & """, function() { window.location.href = 'isearch.aspx?LangType=" & _ContentLanguage & "&action=showdlg&folderid=" & _Id & "'; } );" & Environment.NewLine)

        result.Append("    actionmenu.addBreak();" & Environment.NewLine)

        If (_CheckedInOrApproved And bViewContent _
          And (_PermissionData.IsAdmin OrElse IsFolderAdmin() OrElse IsCopyOrMoveAdmin()) And (_PermissionData.CanAdd Or _PermissionData.CanEdit)) Then
            If ((_EnableMultilingual = "1") And (CInt(_ContentLanguage) < 1)) Then
                result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/cut.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl cut") & """, function() { alert('A language must be selected!'); } );" & Environment.NewLine)
                result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/contentCopy.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl copy") & """, function() { alert('A language must be selected!'); } );" & Environment.NewLine)
            Else
                result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/cut.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl cut") & """, function() { setClipBoard(); } );" & Environment.NewLine)
                result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/contentCopy.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl copy") & """, function() { setCopyClipBoard(); }) ;" & Environment.NewLine)
            End If
        End If

        Dim site As SiteAPI = New SiteAPI
        Dim ekSiteRef As EkSite = site.EkSiteRef()
        If (_ContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncAdmin)) Then
            result.Append(GetSyncMenuOption())
        End If
        result.Append("    MenuUtil.add( actionmenu );" & Environment.NewLine)
        result.Append("    </script>" & Environment.NewLine)
        result.Append("" & Environment.NewLine)

        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub ViewBlogContentByCategoryToolBar()
        Dim result As New System.Text.StringBuilder
        Dim altText As String = ""
        Dim ParentId As Long = _FolderData.ParentId
        Dim count As Integer = 0
        Dim lAddMultiType As Integer = 0
        Dim bSelectedFound As Boolean = False
        Dim bViewContent As Boolean = ("viewcontentbycategory" = _PageAction)   ' alternative is archived content
        Dim bShowDelete As Boolean = False
        Dim helpAliasQualifier As String = ""
        Dim folderIsHidden As Boolean = _ContentApi.IsFolderHidden(_Id)

        If (_PageAction = "viewcontentbycategory") Then
            altText = _MessageHelper.GetMessage("Archive Content Title")
        Else
            altText = _MessageHelper.GetMessage("view content title")
        End If
        If _PostID > 0 Then
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl view post comments") & " """ & _FolderData.Name & """" & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' />")
        Else
            If (_PageAction = "viewcontentbycategory") Then
                txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(IIf(_IsMyBlog, _MessageHelper.GetMessage("view posts in journal msg"), _MessageHelper.GetMessage("view posts in blog msg") & " """ & _FolderData.Name & """") & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' />")
            Else
                txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view archive content title") & " """ & _FolderData.Name & """" & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' />")
            End If

        End If
        result.Append("<table><tr>" & vbCrLf)

        If (_PermissionData.CanAdd) Or (_PermissionData.CanEdit) Then
            result.Append("<script type=""text/javascript"">" & Environment.NewLine)
            result.Append("    var filemenu = new Menu( ""file"" );" & Environment.NewLine)
            If ((_PermissionData.IsAdmin = True Or _PermissionData.CanEdit = True) And _BlogData.EnableComments = True) And (_PermissionData.CanEdit And (Request.QueryString("ContType") <> "" And Request.QueryString("ContType") = CMSContentType_BlogComments)) Then
                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/ui/icons/comment.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("comment text") & """, function() { window.location.href = 'blogs/addeditcomment.aspx?action=Add&blogid=" & _Id & "&contentid=" & _PostID & "'; } );" & Environment.NewLine)
            End If
            Dim active_xml_list As XmlConfigData() = _ContentApi.GetEnabledXmlConfigsByFolder(_FolderData.Id)

            Dim xmlCount As Integer = 0
            Dim canAddHtmlPost As Boolean = False

            For xmlCount = 0 To active_xml_list.Length - 1
                If active_xml_list(xmlCount).Title = "" Then
                    canAddHtmlPost = True
                End If
            Next

            'If (Utilities.IsNonFormattedContentAllowed(active_xml_list)) Then ' we can always add normal HTML posts
            If (_PermissionData.CanAdd And canAddHtmlPost = True) Then
                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/ui/icons/blog.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl blog post html") & """, function() { " & _StyleHelper.GetAddAnchorByContentType(_Id, CMSContentType_Content, True) & " } );" & Environment.NewLine)
            End If
            If (active_xml_list.Length > 0 And Utilities.IsNonFormattedContentAllowed(active_xml_list)) Then
                If (active_xml_list.Length = 1 And active_xml_list(0).Id = 0) Then

                Else
                    result.Append("    var contentTypesMenu = new Menu( ""contentTypes"" );" & Environment.NewLine)
                    Dim k As Integer
                    For k = 0 To active_xml_list.Length - 1
                        If (active_xml_list(k).Id <> 0) Then
                            result.Append("    contentTypesMenu.addItem(""&nbsp;<img valign='middle' src='" & "images/ui/icons/blog.png" & "' />&nbsp;&nbsp;" & active_xml_list(k).Title & """, function() { " & _StyleHelper.GetTypeOverrideAddAnchor(_Id, active_xml_list(k).Id, CMSContentType_Content) & " } );" & Environment.NewLine)
                        End If
                    Next
                    result.Append("    contentTypesMenu.addBreak();" & Environment.NewLine)
                    result.Append("    filemenu.addMenu(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/icons/contentsmartform.png" & "'/>&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl smart form") & """, contentTypesMenu);" & Environment.NewLine)
                    result.Append("    filemenu.addBreak();" & Environment.NewLine)
                End If

            ElseIf (active_xml_list.Length > 0 And Not Utilities.IsNonFormattedContentAllowed(active_xml_list)) Then
                Dim k As Integer
                For k = 0 To active_xml_list.Length - 1
                    If (active_xml_list(k).Id <> 0) Then
                        result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/ui/icons/blog.png" & "' />&nbsp;&nbsp;" & active_xml_list(k).Title & """, function() { " & _StyleHelper.GetTypeOverrideAddAnchor(_Id, active_xml_list(k).Id, CMSContentType_Content) & " } );" & Environment.NewLine)
                    End If
                Next
                result.Append("    filemenu.addBreak();" & Environment.NewLine)
            End If
            'End If
            If ((_PermissionData.CanEditFolders Or _PermissionData.CanEditApprovals) And bViewContent) _
                     OrElse IsFolderAdmin() Then
                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/ui/icons/blogLink.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl roll link") & """, function() { window.location.href = 'blogs/addblogroll.aspx?id=" & _Id.ToString() & "&LangType=" & _ContentLanguage & "'; } );" & Environment.NewLine)
                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/note.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl generic subject") & """, function() { window.location.href = 'blogs/addblogsubject.aspx?id=" & _Id.ToString() & "&LangType=" & _ContentLanguage & "'; } );" & Environment.NewLine)
            End If

            result.Append("    MenuUtil.add( filemenu );" & Environment.NewLine)
            result.Append("    </script>" & Environment.NewLine)
            If ((_PermissionData.CanAdd Or _PermissionData.CanAddFolders) And bViewContent) Or ((_PermissionData.IsAdmin = True Or (_PermissionData.CanEdit = True And _BlogData.EnableComments = True)) And (_PermissionData.CanEdit And (Request.QueryString("ContType") <> "" And Request.QueryString("ContType") = CMSContentType_BlogComments))) Then
                result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'file');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'file');"" onmouseout=""this.className='menuRootItem'""><span class=""new"">" & _MessageHelper.GetMessage("lbl New") & "</span></td>")
            End If
        End If

        result.Append("<script language=""javascript"">" & Environment.NewLine)
        result.Append("    var deletemenu = new Menu( ""delete"" );" & Environment.NewLine)
        If (_PermissionData.CanDeleteFolders And bViewContent And _Id <> 0 AndAlso Not _IsMyBlog) Then
            bShowDelete = True
            result.Append("    deletemenu.addItem(""&nbsp;<img src='" & "images/ui/icons/folderBlogDelete.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl this blog") & """, function() { if( ConfirmFolderDelete(" & _Id & ") ) { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=DoDeleteFolder&id=" & _Id & "&ParentID=" & ParentId & "'; }} );" & Environment.NewLine)
        End If
        If (bViewContent _
         And (_PermissionData.IsAdmin _
          OrElse IsFolderAdmin()) OrElse _PermissionData.CanDelete) Then
            If ((_EnableMultilingual = "1") And (CInt(_ContentLanguage) < 1)) Then
                bShowDelete = True
                result.Append("    deletemenu.addItem(""&nbsp;<img src='" & "images/ui/icons/blogDelete.png" & " ' />&nbsp;&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl posts") & """, function() { alert('A language must be selected!'); } );" & Environment.NewLine)
            Else
                bShowDelete = True
                If _PageAction = "viewarchivecontentbycategory" Then
                    result.Append("    deletemenu.addItem(""&nbsp;<img src='" & "images/ui/icons/blogDelete.png" & " ' />&nbsp;&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl posts") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=DeleteContentByCategory&id=" & _Id & "&showarchive=true';  } );" & Environment.NewLine)
                Else
                    result.Append("    deletemenu.addItem(""&nbsp;<img src='" & "images/ui/icons/blogDelete.png" & " ' />&nbsp;&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl posts") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=DeleteContentByCategory&id=" & _Id & "';  } );" & Environment.NewLine)
                End If
            End If
        End If
        result.Append("    MenuUtil.add( deletemenu );" & Environment.NewLine)
        result.Append("    </script>" & Environment.NewLine)
        SetViewImage(96)
        result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'view');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'view');"" onmouseout=""this.className='menuRootItem'""><span class=""folderView"">" & _MessageHelper.GetMessage("lbl View") & "</span></td>")
        If ((Not folderIsHidden) AndAlso bShowDelete = True AndAlso Not (Request.QueryString("ContType") <> "" And Request.QueryString("ContType") = CMSContentType_BlogComments)) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'delete');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'delete');"" onmouseout=""this.className='menuRootItem'""><span class=""delete"">" & _MessageHelper.GetMessage("lbl Delete") & "</span></td>")
        End If
        result.Append("<script type=""text/javascript"">" & Environment.NewLine)
        result.Append("    var viewmenu = new Menu( ""view"" );" & Environment.NewLine)
        If Not ((Request.QueryString("ContType") <> "" And Request.QueryString("ContType") = CMSContentType_BlogComments)) Then
            AddLanguageMenu(result)
        End If
        result.Append("    viewmenu.addBreak();" & Environment.NewLine)

        If (bViewContent) Then
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentArchived.png" & "' />&nbsp;&nbsp;&nbsp;" & _MessageHelper.GetMessage("archive content") & """, function() { window.location.href = 'content.aspx?action=" & _NextActionType & "&id=" & _Id & "&LangType=" & _ContentLanguage & IIf(IsAssetContentType(_ContentTypeSelected, False), "&" & _ContentTypeUrlParam & "=" & MakeArchiveAssetContentType(_ContentTypeSelected), IIf(IsArchiveAssetContentType(_ContentTypeSelected), "&" & _ContentTypeUrlParam & "=" & MakeNonArchiveAssetContentType(_ContentTypeSelected), "")) & "' } );" & Environment.NewLine)
        Else
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentArchived.png" & "' />&nbsp;&nbsp;&nbsp;" & _MessageHelper.GetMessage("top Content") & """, function() { window.location.href = 'content.aspx?action=" & _NextActionType & "&id=" & _Id & "&LangType=" & _ContentLanguage & IIf(IsAssetContentType(_ContentTypeSelected, False), "&" & _ContentTypeUrlParam & "=" & MakeArchiveAssetContentType(_ContentTypeSelected), IIf(IsArchiveAssetContentType(_ContentTypeSelected), "&" & _ContentTypeUrlParam & "=" & MakeNonArchiveAssetContentType(_ContentTypeSelected), "")) & "' } );" & Environment.NewLine)
        End If

        result.Append("    viewmenu.addBreak();" & Environment.NewLine)

        If ((_PermissionData.CanEditFolders Or _PermissionData.CanEditApprovals) And bViewContent) _
         OrElse IsFolderAdmin() Then
            result.Append("    viewmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/properties.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Folder Properties") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=ViewFolder&id=" & _Id & "' } );" & Environment.NewLine)
        End If

        result.Append("    MenuUtil.add( viewmenu );" & Environment.NewLine)

        result.Append("    var actionmenu = new Menu( ""action"" );" & Environment.NewLine)
        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/magnifier.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("generic Search") & """, function() { window.location.href = 'isearch.aspx?LangType=" & _ContentLanguage & "&action=showdlg&folderid=" & _Id & "'; } );" & Environment.NewLine)
        result.Append("    MenuUtil.add( actionmenu );" & Environment.NewLine)
        result.Append("    </script>" & Environment.NewLine)
        If Not ((Request.QueryString("ContType") <> "" And Request.QueryString("ContType") = CMSContentType_BlogComments)) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'action');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'action');"" onmouseout=""this.className='menuRootItem'""><span class=""action"">" & _MessageHelper.GetMessage("lbl Action") & "</span></td>")
        End If
        If (Request.QueryString("ContType") = CMSContentType_BlogComments) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_ContentApi.AppPath & "images/UI/Icons/back.png", "content.aspx?action=ViewContentByCategory&id=" & _Id, _MessageHelper.GetMessage("alt back button"), _MessageHelper.GetMessage("btn back"), ""))
        End If
        If (_ContentId > 0) Then
            helpAliasQualifier = "_item"
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton(_StyleHelper.GetHelpAliasPrefix(_FolderData) & _PageAction & helpAliasQualifier))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub AddLanguageMenu(ByVal result As StringBuilder)
        If _EnableMultilingual = 1 Then
            Dim m_refsite As New SiteAPI
            Dim language_data() As LanguageData = m_refsite.GetAllActiveLanguages
            result.Append("    var languagemenu = new Menu( ""language"" );" & Environment.NewLine)
            result.Append("    viewmenu.addBreak();" & Environment.NewLine)
            'result.Append("<td class=""label"">&nbsp;|&nbsp;View:")
            'result.Append("<select id=selLang name=selLang OnChange=""javascript:LoadLanguage('frmContent');"">")

            Dim strSelectedLanguageName As String = ""
            Dim strName As String
            strName = "All"
            If _ContentLanguage = -1 Then
                strName = "<b>" & strName & "</b>"
            End If
            result.Append("    languagemenu.addItem(""&nbsp;<img src='" & _ContentApi.AppImgPath & "flags/flag0000.gif' alt=\""" & strName & "\"" />&nbsp;&nbsp;" & strName & """, function() { LoadLanguage('-1'); } );" & Environment.NewLine)
            For iLang As Integer = 0 To language_data.Length - 1
                With language_data(iLang)
                    strName = .LocalName
                    If (_ContentLanguage = .Id) Then
                        strSelectedLanguageName = strName
                        strName = "<b>" & strName & "</b>"
                    End If
                    result.Append("    languagemenu.addItem(""&nbsp;<img src='" & .FlagFile & "' />&nbsp;&nbsp;" & strName & """, function() { LoadLanguage('" & .Id & "'); } );" & Environment.NewLine)
                End With
            Next
            'result.Append("</select></td>")
            result.Append("    viewmenu.addMenu(""&nbsp;<img src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' alt=\""" & strSelectedLanguageName & "\"" />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl Language") & """, languagemenu);" & Environment.NewLine)
        End If
    End Sub

    Private Function DoesAssetSupertypeExist(ByVal asset_data As AssetInfoData(), ByVal lContentType As Integer) As Boolean
        Dim i As Integer = 0
        Dim result As Boolean = False
        If (Not (IsNothing(asset_data))) Then
            For i = 0 To asset_data.Length - 1
                If (_ManagedAsset_Min <= asset_data(i).TypeId And asset_data(i).TypeId <= _ManagedAsset_Max) Then
                    If (asset_data(i).TypeId = lContentType) Then
                        result = True
                        Exit For
                    End If
                End If
            Next
        End If
        Return (result)
    End Function

    Private Sub Populate_ViewCalendar(ByVal contentdata As Ektron.Cms.Common.EkContentCol)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strTag As String
        Dim strtag1 As String

        strTag = "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=" & _PageAction & "&orderby="
        strtag1 = "&id=" & _Id & IIf(_ContentTypeQuerystringParam <> "", "&" & _ContentTypeUrlParam & "=" & _ContentTypeQuerystringParam, "") & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>"

        colBound.DataField = "TITLE"
        colBound.HeaderText = strTag & "Title" & strtag1 & _MessageHelper.GetMessage("generic title") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "FIRSTOCCURENCE"
        colBound.HeaderText = _MessageHelper.GetMessage("webcalendar first occurence")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TYPE"
        colBound.HeaderText = "Event Type"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LANGUAGE"
        colBound.HeaderText = strTag & "language" & strtag1 & _MessageHelper.GetMessage("generic language") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = strTag & "ID" & strtag1 & _MessageHelper.GetMessage("generic ID") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STATUS"
        colBound.HeaderText = strTag & "status" & strtag1 & _MessageHelper.GetMessage("generic Status") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DATEMODIFIED"
        colBound.HeaderText = strTag & "DateModified" & strtag1 & _MessageHelper.GetMessage("generic Date Modified") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITORNAME"
        colBound.HeaderText = strTag & "editor" & strtag1 & _MessageHelper.GetMessage("generic Last Editor") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("FIRSTOCCURENCE", GetType(String)))
        dt.Columns.Add(New DataColumn("TYPE", GetType(String)))
        dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(Int64)))
        dt.Columns.Add(New DataColumn("STATUS", GetType(String)))
        dt.Columns.Add(New DataColumn("DATEMODIFIED", GetType(String)))
        dt.Columns.Add(New DataColumn("EDITORNAME", GetType(String)))

        Dim ViewUrl As String = ""
        Dim EditUrl As String = ""
        Dim i As Integer
        Dim bAssetItem As Boolean = False

        For i = 0 To contentdata.Count - 1
            dr = dt.NewRow()

            'dmsMenuGuid is created to uniquely identify menu element component in the DOM,
            'just in case there is more than one menu that contains the same contentId & language
            'This case is known to apply in non-Workarea implementations of the DmsMenu but is
            'implemented for ALL DmsMenus, including the Workarea
            Dim dmsMenuGuid As String
            dmsMenuGuid = System.Guid.NewGuid().ToString()
            Dim makeUnique As String = contentdata.Item(i).Id & contentdata.Item(i).Language & dmsMenuGuid
            Dim contentStatus As String = contentdata.Item(i).ContentStatus
            'If (contentdata.Item(i).ContentStatus = "A") Then
            dr(0) = "<div class=""ektron dmsWrapper"""
            dr(0) = dr(0) & " id=""dmsWrapper" & makeUnique & """>"
            dr(0) = dr(0) & "<p class=""dmsItemWrapper"""
            dr(0) = dr(0) & " id=""dmsItemWrapper" & makeUnique & """"
            dr(0) = dr(0) & " title=""View Menu"""
            dr(0) = dr(0) & " style=""overflow:visible;"""
            dr(0) = dr(0) & ">"
            dr(0) = dr(0) & "<input type=""hidden"" value='{""id"":" & contentdata.Item(i).Id & ","
            dr(0) = dr(0) & """parentId"":" & contentdata.Item(i).FolderId & ","
            dr(0) = dr(0) & """languageId"":" & contentdata.Item(i).Language & ","
            dr(0) = dr(0) & """status"":""" & contentStatus & ""","
            dr(0) = dr(0) & """guid"":""" & dmsMenuGuid & ""","
            dr(0) = dr(0) & """communityDocumentsMenu"":"""","
            dr(0) = dr(0) & """contentType"":" & contentdata.Item(i).ContentType & ","
            dr(0) = dr(0) & """dmsSubtype"":""""}'"
            dr(0) = dr(0) & " id=""dmsContentInfo" & makeUnique & """ />"
            dr(0) = dr(0) + "<img src=""" & _ContentApi.ApplicationPath & "images/ui/icons/calendarViewDay.png"" onclick=""event.cancelBubble=true;"" />"
            dr(0) = dr(0) + "<a"
            dr(0) = dr(0) + " id=""dmsViewItemAnchor" & makeUnique & """"
            dr(0) = dr(0) + " class=""dmsViewItemAnchor"""
            dr(0) = dr(0) + " onclick=""event.cancelBubble=true;"""
            If (contentdata.Item(i).ContentStatus = "A") Then
                ViewUrl = "content.aspx?action=View&folder_id=" & _Id & "&id=" & contentdata.Item(i).Id & "&LangType=" & contentdata.Item(i).Language & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING"))
            Else
                ViewUrl = "content.aspx?action=ViewStaged&folder_id=" & _Id & "&id=" & contentdata.Item(i).Id & "&LangType=" & contentdata.Item(i).Language & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING"))
            End If
            dr(0) = dr(0) + " href=""" & ViewUrl & """"
            dr(0) = dr(0) + " title=""View " & contentdata.Item(i).Title & """"
            dr(0) = dr(0) + ">"
            dr(0) = dr(0) + contentdata.Item(i).Title
            dr(0) = dr(0) + "</a>"
            dr(0) = dr(0) + "</p>"
            dr(0) = dr(0) + "</div>"

            Dim xd As New Xml.XmlDataDocument()
            Try
                xd.LoadXml(contentdata.Item(i).Html)
                Dim startDTXn As Xml.XmlNode = xd.SelectSingleNode("/root/StartTime")
                If startDTXn IsNot Nothing Then
                    Dim alldayXn As Xml.XmlNode = xd.SelectSingleNode("/root/IsAllDay")
                    Dim alldayBool As Boolean = False
                    Dim startDT As New DateTime
                    Dim ENci As New Globalization.CultureInfo(1033)
                    Dim userCi As New Globalization.CultureInfo(_ContentApi.RequestInformationRef.UserCulture)
                    Dim userTzi As Calendar.TimeZoneInfo

                    startDT = DateTime.ParseExact(startDTXn.InnerText, "s", ENci.DateTimeFormat)
                    userTzi = Calendar.TimeZoneInfo.GetTimeZoneInfo(_ContentApi.RequestInformationRef.UserTimeZone)
                    startDT = userTzi.ConvertUtcToTimeZone(startDT)
                    Boolean.TryParse(alldayXn.InnerText, alldayBool)

                    If (Not (startDT.Hour = 0 AndAlso startDT.Minute = 0)) Then
                        If (userCi.DateTimeFormat.PMDesignator = String.Empty) Then 'no ampm designator
                            dr(1) = startDT.ToString("ddd, MMM d yyyy hh:mm", userCi.DateTimeFormat) & " (" & userTzi.StandardName & ")"  'first occurence
                        Else
                            dr(1) = startDT.ToString("ddd, MMM d yyyy h:mm tt", userCi.DateTimeFormat) & " (" & userTzi.StandardName & ")"  'first occurence
                        End If
                    Else
                        dr(1) = startDT.ToString("ddd, MMM d yyyy", userCi.DateTimeFormat) & " (" & userTzi.StandardName & ")"  'first occurence
                    End If
                End If
                Dim isvarianceXn As Xml.XmlNode = xd.SelectSingleNode("/root/IsVariance")
                Dim isCancelledXn As Xml.XmlNode = xd.SelectSingleNode("/root/IsCancelled")
                If isvarianceXn IsNot Nothing Then
                    Dim isvariance As Boolean = Boolean.Parse(isvarianceXn.InnerText)
                    Dim isCancelled As Boolean = Boolean.Parse(isCancelledXn.InnerText)
                    If (isvariance AndAlso isCancelled) Then
                        dr(2) = "Variance - Cancelled occurence"
                    ElseIf (isvariance AndAlso Not isCancelled) Then
                        dr(2) = "Variance - Extra occurence"
                    Else
                        dr(2) = "Original"
                    End If
                End If
            Catch
                dr(1) = "Start Time could not be extracted."
            End Try

            dr(3) = "<a href=""#ShowTip" & contentdata.Item(i).LanguageDescription & """ onmouseover=""ddrivetip('" & contentdata.Item(i).LanguageDescription & "','ADC5EF', 100);"" onmouseout=""hideddrivetip()"" style=""text-decoration:none;"">" & "<img src='" & _LocalizationApi.GetFlagUrlByLanguageID(contentdata.Item(i).Language) & "' />" & "</a>"
            dr(4) = contentdata.Item(i).Id
            dr(5) = _StyleHelper.StatusWithToolTip(contentStatus)
            dr(6) = contentdata.Item(i).DateModified.ToString
            dr(7) = contentdata.Item(i).LastEditorLname & ", " & contentdata.Item(i).LastEditorFname
            dt.Rows.Add(dr)
        Next i

        Dim dv As New DataView(dt)
        FolderDataGrid.DataSource = dv
        FolderDataGrid.DataBind()
        '        _PagingTotalPagesNumber = 1
    End Sub

    Private Sub Populate_ViewContentByCategoryGrid(ByVal contentdata As Ektron.Cms.Common.EkContentCol)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strTag As String
        Dim strtag1 As String

        strTag = "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=" & _PageAction & "&orderby="
        strtag1 = "&id=" & _Id & IIf(_ContentTypeQuerystringParam <> "", "&" & _ContentTypeUrlParam & "=" & _ContentTypeQuerystringParam, "") & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>"

        colBound.DataField = "TITLE"
        colBound.HeaderText = strTag & "Title" & strtag1 & _MessageHelper.GetMessage("generic title") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "CONTENTTYPE"
        colBound.HeaderText = _MessageHelper.GetMessage("content type")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LANGUAGE"
        colBound.HeaderText = strTag & "language" & strtag1 & _MessageHelper.GetMessage("generic language") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = strTag & "ID" & strtag1 & _MessageHelper.GetMessage("generic ID") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STATUS"
        colBound.HeaderText = strTag & "status" & strtag1 & _MessageHelper.GetMessage("generic Status") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DATEMODIFIED"
        colBound.HeaderText = strTag & "DateModified" & strtag1 & _MessageHelper.GetMessage("generic Date Modified") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITORNAME"
        colBound.HeaderText = strTag & "editor" & strtag1 & _MessageHelper.GetMessage("generic Last Editor") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("CONTENTTYPE", GetType(String)))
        dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(Int64)))
        dt.Columns.Add(New DataColumn("STATUS", GetType(String)))
        dt.Columns.Add(New DataColumn("DATEMODIFIED", GetType(String)))
        dt.Columns.Add(New DataColumn("EDITORNAME", GetType(String)))

        Dim ViewUrl As String = ""
        Dim EditUrl As String = ""
        Dim i As Integer
        Dim bAssetItem As Boolean = False

        For i = 0 To contentdata.Count - 1
            bAssetItem = ((contentdata.Item(i).ContentType = CMSContentType.Assets) Or ((contentdata.Item(i).ContentType >= Ektron.Cms.Common.EkConstants.ManagedAsset_Min) And (contentdata.Item(i).ContentType <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)))
            dr = dt.NewRow()
            If (contentdata.Item(i).ContentType = CMSContentType.Forms Or contentdata.Item(i).ContentType = CMSContentType.Archive_Forms) Then
                If (contentdata.Item(i).ContentStatus = "A") Then
                    ViewUrl = "cmsform.aspx?action=ViewForm&folder_id=" & _Id & "&form_id=" & contentdata.Item(i).Id & "&LangType=" & contentdata.Item(i).Language & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING"))
                Else
                    ViewUrl = "cmsform.aspx?action=viewform&staged=true&folder_id=" & _Id & "&form_id=" & contentdata.Item(i).Id & "&LangType=" & contentdata.Item(i).Language & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING"))
                End If
            Else
                If (contentdata.Item(i).ContentStatus = "A") Then
                    ViewUrl = "content.aspx?action=View&folder_id=" & _Id & "&id=" & contentdata.Item(i).Id & "&LangType=" & contentdata.Item(i).Language & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING"))
                Else
                    ViewUrl = "content.aspx?action=ViewStaged&folder_id=" & _Id & "&id=" & contentdata.Item(i).Id & "&LangType=" & contentdata.Item(i).Language & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING"))
                End If
            End If

            'dmsMenuGuid is created to uniquely identify menu element component in the DOM,
            'just in case there is more than one menu that contains the same contentId & language
            'This case is known to apply in non-Workarea implementations of the DmsMenu but is
            'implemented for ALL DmsMenus, including the Workarea
            Dim dmsMenuGuid As String
            dmsMenuGuid = System.Guid.NewGuid().ToString()
            Dim makeUnique As String = contentdata.Item(i).Id & contentdata.Item(i).Language & dmsMenuGuid
            Dim contentStatus As String = contentdata.Item(i).ContentStatus

            'If (contentdata.Item(i).ContentStatus = "A") Then
            dr(0) = "<div class=""ektron dmsWrapper"""
            dr(0) = dr(0) & " id=""dmsWrapper" & makeUnique & """>"
            dr(0) = dr(0) & "<p class=""dmsItemWrapper"""
            dr(0) = dr(0) & " id=""dmsItemWrapper" & makeUnique & """"
            dr(0) = dr(0) & " title=""View Menu"""
            dr(0) = dr(0) & " style=""overflow:visible;"""
            dr(0) = dr(0) & ">"
            dr(0) = dr(0) & "<input type=""hidden"" value='{""id"":" & contentdata.Item(i).Id & ","
            dr(0) = dr(0) & """parentId"":" & contentdata.Item(i).FolderId & ","
            dr(0) = dr(0) & """languageId"":" & contentdata.Item(i).Language & ","
            dr(0) = dr(0) & """status"":""" & contentStatus & ""","
            dr(0) = dr(0) & """guid"":""" & dmsMenuGuid & ""","
            dr(0) = dr(0) & """communityDocumentsMenu"": """","
            dr(0) = dr(0) & """contentType"":" & contentdata.Item(i).ContentType & ","
            dr(0) = dr(0) & """dmsSubtype"":""""}'"
            dr(0) = dr(0) & " id=""dmsContentInfo" & makeUnique & """ />"

            If contentdata.Item(i).ContentType = CMSContentType.Content OrElse contentdata.Item(i).ContentType = CMSContentType.Archive_Content Then
                If contentdata.Item(i).ContentSubType = CMSContentSubtype.PageBuilderData Then
                    dr(0) = dr(0) + "<img src=""" & _ContentApi.AppImgPath & "layout_content.png" & """ onclick=""event.cancelBubble=true;"" />"
                ElseIf (contentdata.Item(i).ContentSubType = CMSContentSubtype.PageBuilderMasterData) Then
                    dr(0) = dr(0) + "<img src=""" & _ContentApi.AppImgPath & "layout_content.png" & """ onclick=""event.cancelBubble=true;"" />"
                Else
                    If contentdata.Item(i).ContentType = CMSContentType.Archive_Content Then
                        dr(0) = dr(0) + "<img src=""" & _ContentApi.ApplicationPath & "Images/ui/icons/contentArchived.png"" onclick=""event.cancelBubble=true;"" />"
                    Else
                        dr(0) = dr(0) + "<img src=""" & _ContentApi.ApplicationPath & "images/ui/icons/contentHtml.png"" onclick=""event.cancelBubble=true;"" />"
                    End If

                End If
            Else
                dr(0) = dr(0) + "<span onclick=""event.cancelBubble=true;"">" & contentdata.Item(i).AssetInfo.Icon & "</span>"
            End If
            dr(0) = dr(0) + "<a"
            dr(0) = dr(0) + " id=""dmsViewItemAnchor" & makeUnique & """"
            dr(0) = dr(0) + " class=""dmsViewItemAnchor"""
            dr(0) = dr(0) + " onclick=""event.cancelBubble=true;"""
            dr(0) = dr(0) + " href=""" & ViewUrl & """"
            dr(0) = dr(0) + " title=""View " & contentdata.Item(i).Title & """"
            dr(0) = dr(0) + ">"
            dr(0) = dr(0) + contentdata.Item(i).Title
            dr(0) = dr(0) + "</a>"
            dr(0) = dr(0) + "</p>"
            dr(0) = dr(0) + "</div>"

            dr(1) = GetContentTypeText(contentdata.Item(i).ContentType, contentdata.Item(i).XMLCollectionID, contentdata.Item(i).ContentSubType)

            dr(2) = "<a href=""#ShowTip" & contentdata.Item(i).LanguageDescription & """ onmouseover=""ddrivetip('" & contentdata.Item(i).LanguageDescription & "','ADC5EF', 100);"" onmouseout=""hideddrivetip()"" style=""text-decoration:none;"">" & "<img src='" & _LocalizationApi.GetFlagUrlByLanguageID(contentdata.Item(i).Language) & "' />" & "</a>"
            dr(3) = contentdata.Item(i).Id
            dr(4) = _StyleHelper.StatusWithToolTip(contentStatus)
            dr(5) = contentdata.Item(i).DateModified.ToString
            dr(6) = contentdata.Item(i).LastEditorLname & ", " & contentdata.Item(i).LastEditorFname
            dt.Rows.Add(dr)
        Next i

        Dim dv As New DataView(dt)
        FolderDataGrid.DataSource = dv
        FolderDataGrid.DataBind()
    End Sub
    Private Function GetContentTypeText(ByVal contentType As Long, Optional ByVal xmlId As Long = -1, Optional ByVal contentSubType As Long = 0) As String
        Dim result As String = ""

        Select Case contentType
            Case 1 ' Content or Smart Form
                If xmlId > 0 Then
                    result = _MessageHelper.GetMessage("lbl smart form") & ": " & _ContentApi.GetXmlConfiguration(xmlId).Title.ToString()
                Else
                    Select Case contentSubType
                        Case CMSContentSubtype.PageBuilderData
                            ' this is a Page Layout
                            result = _MessageHelper.GetMessage("lbl pagebuilder layouts")
                        Case CMSContentSubtype.PageBuilderMasterData
                            ' this is a Master Page Layout
                            result = _MessageHelper.GetMessage("lbl pagebuilder master layouts")
                        Case CMSContentSubtype.WebEvent
                            ' this is a web event, which indicates this is a Calendar Event entry
                            result = _MessageHelper.GetMessage("calendar event")
                        Case Else
                            result = _MessageHelper.GetMessage("lbl html content")
                    End Select
                End If
            Case 2 ' HTML Form/Survey
                result = _MessageHelper.GetMessage("lbl html formsurvey")
            Case 3 ' Archived Content
                result = _MessageHelper.GetMessage("archive content")
            Case 4 ' Archived Form/Survey
                result = _MessageHelper.GetMessage("archive forms survey")
            Case 7 ' Library Item
                result = _MessageHelper.GetMessage("lbl library item")
            Case 8 'Asset
                result = _MessageHelper.GetMessage("lbl asset")
            Case 9 ' Non Image Library Item
                result = _MessageHelper.GetMessage("nonimage library item")
            Case 10 ' PDF
                result = _MessageHelper.GetMessage("content:asset:pdf")
            Case 12 ' Archived Media
                result = _MessageHelper.GetMessage("lbl archived media")
            Case 13 ' Blog Comment
                result = _MessageHelper.GetMessage("lbl blog comment")
            Case 14 ' Smart Form
                If xmlId > 0 Then
                    result = _MessageHelper.GetMessage("lbl smart form") & ": " & _ContentApi.GetXmlConfiguration(xmlId).Title.ToString()
                End If
            Case 98 ' Non Library Form
                result = _MessageHelper.GetMessage("nonlibrary form")
            Case 99 ' Non Library Content
                result = _MessageHelper.GetMessage("nonlibrary content")
            Case 101, 1101 ' Microsoft Office Documents
                result = _MessageHelper.GetMessage("office document")
            Case 102, 1102 ' Managed ASsets (jpg, tif, gif, txt, etc.)
                result = _MessageHelper.GetMessage("managed asset")
            Case 104, 1104 ' Multi Media
                result = _MessageHelper.GetMessage("lbl multimedia")
            Case 1111 ' Discussion Topic
                result = _MessageHelper.GetMessage("discussion topic")
            Case 3333 ' Catalog Entry
                If xmlId > 0 Then
                    result = _ContentApi.GetXmlConfiguration(xmlId).Title.ToString()
                Else
                    result = _MessageHelper.GetMessage("catalog entry")
                End If
            Case Else
                result = _MessageHelper.GetMessage("unknown content type")
        End Select
        Return result
    End Function

    Private Sub Populate_ViewMediaGrid(ByVal contentdata As Ektron.Cms.Common.EkContentCol)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strTag As String
        Dim strtag1 As String

        strTag = "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=" & _PageAction & "&orderby="
        strtag1 = "&id=" & _Id & IIf(_ContentTypeQuerystringParam <> "", "&" & _ContentTypeUrlParam & "=" & _ContentTypeQuerystringParam, "") & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>"

        colBound.DataField = "A"
        colBound.HeaderText = "#&160;"
        colBound.ItemStyle.Width = Unit.Percentage(33)
        colBound.HeaderStyle.CssClass = "title-header"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "B"
        colBound.HeaderText = "#&160;"
        colBound.ItemStyle.Width = Unit.Percentage(33)
        colBound.HeaderStyle.CssClass = "title-header"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "C"
        colBound.HeaderText = "#&160;"
        colBound.ItemStyle.Width = Unit.Percentage(33)
        colBound.HeaderStyle.CssClass = "title-header"
        FolderDataGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow = dt.NewRow()

        dt.Columns.Add(New DataColumn("A", GetType(String)))
        dt.Columns.Add(New DataColumn("B", GetType(String)))
        dt.Columns.Add(New DataColumn("C", GetType(String)))

        Dim ViewUrl As String = ""
        Dim EditUrl As String = ""
        Dim i As Integer
        Dim bAssetItem As Boolean = False
        Dim iMod As Integer = 0
        Dim f() As FolderData = Me._EkContent.GetChildFolders(Me._Id, False, FolderOrderBy.Name)
        If f IsNot Nothing AndAlso f.Length > 0 Then
            For i = 0 To (f.Length - 1)
                iMod = i Mod 3
                If iMod = 0 Then
                    dr = dt.NewRow()
                End If
                dr(iMod) &= "<br/><img src=""" & Me._ContentApi.AppImgPath & "thumb_folder.gif"" border=""1""/><br/><a href=""content.aspx?action=ViewContentByCategory&id=" & f(i).Id.ToString() & """>" & f(i).Name & "</a><br/><br/>"
                If iMod = 2 Then
                    dt.Rows.Add(dr)
                    dr = Nothing
                End If
            Next
        End If
        Dim offset As Integer = iMod + 1
        For i = 0 To contentdata.Count - 1
            bAssetItem = ((contentdata.Item(i).ContentType = CMSContentType.Assets) Or ((contentdata.Item(i).ContentType >= Ektron.Cms.Common.EkConstants.ManagedAsset_Min) And (contentdata.Item(i).ContentType <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max)))
            iMod = (i + offset) Mod 3
            If iMod = 0 Then
                dr = dt.NewRow()
            End If
            dr(iMod) = "<br/><img src=""" & Me._ContentApi.AppImgPath & "thumb_bmp.gif"" border=""1""/><br/>"
            If (bAssetItem And (contentdata.Item(i).ContentStatus = "O") And (contentdata.Item(i).UserId = _CurrentUserId)) Then
                ViewUrl = "content.aspx?action=View&folder_id=" & _Id & "&id=" & contentdata.Item(i).Id & "&LangType=" & contentdata.Item(i).Language '& "&callerpage=content.aspx&origurl=" '& Server.UrlEncode(Request.ServerVariables("QUERY_STRING"))
                EditUrl = "edit.aspx?close=false&LangType=" & contentdata.Item(i).Language & "&id=" & contentdata.Item(i).Id & "&type=update&back_file=content.aspx&back_action=ViewContentByCategory&back_id=" & contentdata.Item(i).FolderId & "&back_LangType=" & contentdata.Item(i).Language
            Else
                If (contentdata.Item(i).ContentStatus = "A") Then
                    dr(iMod) &= "<a href=""content.aspx?action=View&folder_id=" & _Id & "&id=" & contentdata.Item(i).Id & "&LangType=" & contentdata.Item(i).Language & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING")) & """" & "title='" & _MessageHelper.GetMessage("generic View") & " """ & Replace(contentdata.Item(i).Title & """", "'", "`") & "'" & "> " & contentdata.Item(i).Title & " </a> "
                Else
                    dr(iMod) &= "<a href=""content.aspx?action=viewstaged&folder_id=" & _Id & "&id=" & contentdata.Item(i).Id & "&LangType=" & contentdata.Item(i).Language & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING")) & """" & "title='" & _MessageHelper.GetMessage("generic View") & " """ & Replace(contentdata.Item(i).Title & """", "'", "`") & "'" & "> " & contentdata.Item(i).Title & " </a> "
                End If
            End If
            dr(iMod) &= "<br/><br/>"
            If iMod = 2 Then
                dt.Rows.Add(dr)
                dr = Nothing
            End If
        Next i
        If iMod < 2 Then
            dt.Rows.Add(dr)
        End If

        Dim dv As New DataView(dt)
        FolderDataGrid.DataSource = dv
        FolderDataGrid.DataBind()
    End Sub

    Private Sub Populate_ViewBlogPostsByCategoryGrid(ByVal contentdata As Ektron.Cms.Common.EkContentCol, ByVal commenttally As Hashtable)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strTag As String
        Dim strtag1 As String

        strTag = "<a href=""content.aspx?LangType=" & _ContentApi.ContentLanguage & "&action=" & _PageAction & "&orderby="
        strtag1 = "&id=" & _Id & IIf(_ContentTypeQuerystringParam <> "", "&" & _ContentTypeUrlParam & "=" & _ContentTypeQuerystringParam, "") & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>"

        colBound.DataField = "TITLE"
        colBound.HeaderText = _MessageHelper.GetMessage("generic Title")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LANGUAGE"
        colBound.HeaderText = _MessageHelper.GetMessage("generic language")
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = _MessageHelper.GetMessage("generic ID")
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STATUS"
        colBound.HeaderText = _MessageHelper.GetMessage("generic Status")
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DATEMODIFIED"
        colBound.HeaderText = _MessageHelper.GetMessage("generic Date Modified")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITORNAME"
        colBound.HeaderText = _MessageHelper.GetMessage("generic Last Editor")
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "COMMENTS"
        colBound.HeaderText = _MessageHelper.GetMessage("comments label")
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(Int64)))
        dt.Columns.Add(New DataColumn("STATUS", GetType(String)))
        dt.Columns.Add(New DataColumn("DATEMODIFIED", GetType(String)))
        dt.Columns.Add(New DataColumn("EDITORNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("COMMENTS", GetType(String)))

        Dim ViewUrl As String = ""
        Dim EditUrl As String = ""
        Dim i As Integer
        Dim aValues As String()
        Dim bNewComment As Boolean = False
        For i = 0 To contentdata.Count - 1
            commenttally = commenttally.Clone
            dr = dt.NewRow()

            dr(0) &= "<img src=""" & _ContentApi.ApplicationPath & "images/ui/icons/blog.png"" style=""margin-right:.25em;"" />"
            dr(0) &= "<a href=""content.aspx?action=View&folder_id=" & _Id & "&id=" & contentdata.Item(i).Id & "&mode=1&LangType=" & contentdata.Item(i).Language & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING")) & """" & " title='" & _MessageHelper.GetMessage("generic View") & " """ & Replace(contentdata.Item(i).Title & """", "'", "`") & "'" & ">"
            dr(0) &= contentdata.Item(i).Title
            dr(0) &= "</a>"

            dr(1) = "<a href=""#ShowTip"" onmouseover=""ddrivetip('" & contentdata.Item(i).LanguageDescription & "','ADC5EF', 100);"" onmouseout=""hideddrivetip()"" style=""text-decoration:none;"">" & "<img src='" & _LocalizationApi.GetFlagUrlByLanguageID(contentdata.Item(i).Language) & "' border=""0"" />" & "</a>"
            dr(2) = contentdata.Item(i).Id
            dr(3) = _StyleHelper.StatusWithToolTip(contentdata.Item(i).ContentStatus)
            dr(4) = contentdata.Item(i).DateModified.ToString
            dr(5) = contentdata.Item(i).LastEditorLname & ", " & contentdata.Item(i).LastEditorFname
            If commenttally.ContainsKey((contentdata.Item(i).Id.ToString()) & "-" & contentdata.Item(i).Language.ToString()) Then
                aValues = commenttally((contentdata.Item(i).Id.ToString()) & "-" & contentdata.Item(i).Language.ToString())
                Dim actionRequired As String = ""

                ' let's do some math to see if any of the comments are pending admin interaction.
                ' if the comment_sum/aValues(1) value is less than the number of comments times "7"
                ' (the value of blog comment status complete), then at least one must be pending action.
                If (aValues(1) < (aValues(0) * 7)) Then
                    actionRequired = "class=""blogCommentStatusPending"" title=""" & _MessageHelper.GetMessage("moderator action required") & """ "
                End If
                dr(6) &= "<a " & actionRequired & "href=""content.aspx?id=" & _Id & "&action=ViewContentByCategory&LangType=" & _ContentLanguage.ToString() & "&ContType=" & CMSContentType_BlogComments & "&contentid=" & contentdata.Item(i).Id.ToString() & "&viewin=" & contentdata.Item(i).Language.ToString() & """>" & aValues(0).ToString() & "</a>"
            Else
                dr(6) &= "<a href=""content.aspx?id=" & _Id & "&action=ViewContentByCategory&LangType=" & _ContentLanguage.ToString() & "&ContType=" & CMSContentType_BlogComments & "&contentid=" & contentdata.Item(i).Id.ToString() & "&viewin=" & contentdata.Item(i).Language.ToString() & """>" & 0 & "</a>"
            End If

            dt.Rows.Add(dr)
        Next i
        Dim dv As New DataView(dt)
        FolderDataGrid.DataSource = dv
        FolderDataGrid.DataBind()
    End Sub

    Private Sub Populate_ViewBlogCommentsByCategoryGrid(ByVal blogcommentdata As Ektron.Cms.Content.EkTasks)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strTag As String
        Dim strtag1 As String
        Dim nBlogCommentCount As Integer
        strTag = "<a href=""content.aspx?LangType=" & _ContentApi.ContentLanguage & "&action=" & _PageAction & "&orderby="
        strtag1 = "&id=" & _Id & IIf(_ContentTypeQuerystringParam <> "", "&" & _ContentTypeUrlParam & "=" & _ContentTypeQuerystringParam, "") & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>"

        FolderDataGrid.ShowHeader = False

        colBound.DataField = "PREVIEW"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(145)
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLEDESCRIPTION"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        FolderDataGrid.Columns.Add(colBound)

        FolderDataGrid.BorderColor = Drawing.Color.White
        FolderDataGrid.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0)
        FolderDataGrid.CellPadding = 6
        FolderDataGrid.CellSpacing = 2

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("PREVIEW", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLEDESCRIPTION", GetType(String)))

        Dim ApproveURL As String = ""
        Dim ViewUrl As String = ""
        Dim EditUrl As String = ""
        Dim DeleteUrl As String = ""
        Dim sAppend As String = ""
        Dim _CommentDisplayName As String = String.Empty
        Dim i As Integer
        nBlogCommentCount = blogcommentdata.Count
        For i = 1 To blogcommentdata.Count
            If Not (blogcommentdata.Item(i) Is Nothing) Then
                If _ContentLanguage = blogcommentdata.Item(i).ContentLanguage Or (Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES = _ContentLanguage And Request.QueryString("viewin") = blogcommentdata.Item(i).ContentLanguage) Then
                    If blogcommentdata.Item(i).CommentDisplayName = String.Empty Then
                        _CommentDisplayName = blogcommentdata.Item(i).CreatedByUser
                    Else
                        _CommentDisplayName = blogcommentdata.Item(i).CommentDisplayName
                    End If
                    dr = dt.NewRow()
                    If Request.QueryString("contentid") <> "" Then
                        sAppend = "&blogid=" & _Id.ToString() & "&contentid=" & Request.QueryString("contentid")
                    Else
                        sAppend = "&blogid=" & _Id.ToString()
                    End If
                    ViewUrl = "tasks.aspx?action=ViewTask&tid=" & blogcommentdata.Item(i).TaskID.ToString() & "&fromViewContent=1&ty=both&LangType=" & _ContentApi.ContentLanguage
                    EditUrl = "blogs/addeditcomment.aspx?action=Edit&id=" & blogcommentdata.Item(i).TaskID.ToString() & sAppend
                    dr(0) &= "<p class=""center"
                    If (CInt(blogcommentdata.Item(i).State) = BlogCommentState.Pending) Then
                        dr(0) &= " blogCommentStatusPending"" title=""" & _MessageHelper.GetMessage("moderator action required")
                    End If
                    dr(0) &= """>"
                    If (CInt(blogcommentdata.Item(i).State) = BlogCommentState.Pending) Then
                        ApproveURL = "tasksaction.aspx?action=ApproveTask&tid=" & blogcommentdata.Item(i).TaskID.ToString() & "&ty=both" & sAppend
                        'dr(0) &= "<table border=""0"" width=""100%""><tr><td style=""text-align: center;"">"
                        dr(0) &= "<img style=""border: none; margin: .5em auto;"" src=""" & _ContentApi.AppImgPath & "thumb_blogcomment.gif"" width=""53"" height=""55""/><br/>"
                        'dr(0) &= "</td></tr></table>"
                        If (_PermissionData.CanEdit = True) Then
                            dr(0) &= "<a href=""" & ApproveURL & """>" & _MessageHelper.GetMessage("generic approve title") & "</a>&nbsp;|&nbsp;"
                        Else
                            dr(0) &= "<br/>&nbsp;"
                        End If
                    Else
                        dr(0) &= "<img src=""" & _ContentApi.AppImgPath & "thumb_blogcomment.gif"" width=""53"" height=""55"" style=""border: none; margin: .5em auto;""/><br />"
                    End If
                    If _PermissionData.CanEdit Then
                        dr(0) &= "<a href=""" & EditUrl & """>" & _MessageHelper.GetMessage("generic edit title") & "</a>&nbsp;|&nbsp;"
                        DeleteUrl = "tasksaction.aspx?action=DeleteTask&tid=" & blogcommentdata.Item(i).TaskID.ToString() & "&ty=both" & sAppend
                        dr(0) &= "<a href=""" & DeleteUrl & """ onclick=""return confirm('" & _MessageHelper.GetMessage("msg del comment") & "');"">" & _MessageHelper.GetMessage("generic delete title") & "</a>&nbsp;"
                    End If
                    dr(0) &= "</p>"
                    If blogcommentdata.Item(i).TaskTitle <> "BlogComment" Then
                        ' dr(1) = "<a href=""" & ViewUrl & """ >" & blogcommentdata.Item(i).TaskTitle & "</a><br/>"
                    End If
                    dr(1) &= ("<font color=""gray"">""" & Server.HtmlEncode(blogcommentdata.Item(i).Description) & """</font><br/><font color=""green"">" & _MessageHelper.GetMessage("lbl posted by") & " " & _CommentDisplayName & " " & _MessageHelper.GetMessage("res_isrch_on") & " " & blogcommentdata.Item(i).DateCreated.ToString() & "</font>")
                    dt.Rows.Add(dr)
                End If
            End If
        Next i
        Dim dv As New DataView(dt)
        FolderDataGrid.DataSource = dv
        FolderDataGrid.DataBind()

        'TotalPages.Visible = False
        'CurrentPage.Visible = False
        'lnkBtnPreviousPage.Visible = False
        'NextPage.Visible = False
        'LastPage.Visible = False
        'FirstPage.Visible = False
        'PageLabel.Visible = False
        'OfLabel.Visible = False

    End Sub

    Public Sub ResetPostData()
        _ChangeLanguage = True
    End Sub

#End Region

#Region "Catalog"

    Private Sub ViewCatalogToolBar(ByVal entryCount As Long)
        Dim result As New System.Text.StringBuilder
        Dim altText As String = ""
        Dim ParentId As Long = _FolderData.ParentId
        Dim pProductType As New ProductType(_ContentApi.RequestInformationRef)
        Dim count As Integer = 0
        Dim lAddMultiType As Integer = 0
        Dim bSelectedFound As Boolean = False
        Dim bShowAddMenu As Boolean = True
        Dim bViewContent As Boolean = ("viewcontentbycategory" = _PageAction)   ' alternative is archived content
        Dim bCommerceAdmin As Boolean = True
        Dim bFolderAdmin As Boolean = False

        bCommerceAdmin = _ContentApi.IsARoleMember(CmsRoleIds.CommerceAdmin)
        bFolderAdmin = (bFolderAdmin Or bCommerceAdmin)

        If (bViewContent) Then
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl view catalog") & " """ & _FolderData.Name & """") & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' />"
            altText = _MessageHelper.GetMessage("Archive Content Title")
        Else
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl view catalog archive") & " """ & _FolderData.Name & """") & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' />"
            altText = _MessageHelper.GetMessage("view content title")
        End If
        result.Append("<table><tr>" & vbCrLf)
        If ((_PermissionData.CanAdd And bViewContent) Or _PermissionData.IsReadOnly = True) Then
            If (_PermissionData.CanAdd And bViewContent) Then
                If Not bSelectedFound Then
                    _ContentType = _CMSContentType_AllTypes
                End If
            End If
        End If

        If ((_PermissionData.CanAdd Or _PermissionData.CanAddFolders Or bCommerceAdmin) And bViewContent) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'file');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'file');"" onmouseout=""this.className='menuRootItem'""><span class=""new"">" & _MessageHelper.GetMessage("lbl New") & "</span></td>")
        End If
        SetViewImage()
        If (((_PermissionData.CanAdd) Or _PermissionData.IsReadOnly Or bCommerceAdmin)) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'view');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'view');"" onmouseout=""this.className='menuRootItem'""><span class=""folderView"">" & _MessageHelper.GetMessage("lbl View") & "</span></td>")
        End If
        If (bViewContent _
          And (_PermissionData.IsAdmin _
          OrElse bFolderAdmin OrElse bCommerceAdmin) OrElse (_PermissionData.CanDelete)) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'delete');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'delete');"" onmouseout=""this.className='menuRootItem'""><span class=""delete"">" & _MessageHelper.GetMessage("lbl Delete") & "</span></td>")
        End If

        result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'action');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'action');"" onmouseout=""this.className='menuRootItem'""><span class=""action"">" & _MessageHelper.GetMessage("lbl Action") & "</span></td>")

        bShowAddMenu = True
        If _EnableMultilingual = 1 Then
            Dim m_refsite As New SiteAPI
            Dim language_data(0) As LanguageData
            language_data = m_refsite.GetAllActiveLanguages

        End If
        Dim active_prod_list As New List(Of ProductTypeData)
        active_prod_list = pProductType.GetFolderProductTypeList(_FolderData.Id)

        Dim smartFormsRequired As Boolean = Not Utilities.IsNonFormattedContentAllowed(active_prod_list.ToArray())
        Dim canAddAssets As Boolean = (_PermissionData.CanAdd Or _PermissionData.CanAddFolders) And bViewContent

        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton(_PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")

        result.Append("<script type=""text/javascript"">" & Environment.NewLine)

        result.Append("    var filemenu = new Menu( ""file"" );" & Environment.NewLine)
        If (_PermissionData.CanAddFolders OrElse bCommerceAdmin) Then
            result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/folderGreen.png" & "' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl commerce catalog") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=AddSubFolder&type=catalog&id=" & _Id & "' } );" & Environment.NewLine)
            result.Append("    filemenu.addBreak();" & Environment.NewLine)
        End If

        If (_PermissionData.CanAdd) Then
            If (active_prod_list.Count > 0) Then
                Dim k As Integer
                For k = 0 To active_prod_list.Count - 1
                    If (active_prod_list(k).Id <> 0) Then

                        result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & "images/ui/icons/")

                        Select Case active_prod_list(k).EntryClass

                            Case CatalogEntryType.SubscriptionProduct

                                result.Append("bookGreen.png")

                            Case CatalogEntryType.Kit

                                result.Append("box.png")

                            Case CatalogEntryType.Bundle

                                result.Append("package.png")

                            Case Else

                                result.Append("brick.png")

                        End Select

                        result.Append("' />&nbsp;&nbsp;" & active_prod_list(k).Title & """, function() { " & _StyleHelper.GetCatalogAddAnchorType(_Id, active_prod_list(k).Id) & " } );" & Environment.NewLine)
                    End If
                Next
            End If
        End If

        If (_PermissionData.CanAdd Or _PermissionData.CanAddFolders) Then
            result.Append("    MenuUtil.add( filemenu );" & Environment.NewLine)
        End If

        result.Append("    var viewmenu = new Menu( ""view"" );" & Environment.NewLine)
        result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/ui/icons/folderGreenView.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl All Types"), -1) & """, function() { UpdateView(" & _CMSContentType_AllTypes & "); } );" & Environment.NewLine)
        result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "Images/ui/icons/brick.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl commerce products"), 0) & """, function() { UpdateView(" & Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product & "); } );" & Environment.NewLine)
        result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/ui/icons/box.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl commerce kits"), 2) & """, function() { UpdateView(" & Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit & "); } );" & Environment.NewLine)
        result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/ui/icons/package.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl commerce bundles"), 3) & """, function() { UpdateView(" & Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle & "); } );" & Environment.NewLine)
        result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/ui/icons/bookGreen.png" & "' />&nbsp;&nbsp;" & MakeBold(_MessageHelper.GetMessage("lbl commerce subscriptions"), 4) & """, function() { UpdateView(" & Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct & "); } );" & Environment.NewLine)

        If ((_PermissionData.CanAdd) And bViewContent) Or _PermissionData.IsReadOnly = True Then

            AddLanguageMenu(result)

            If (bViewContent) Then
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/contentArchived.png" & "' />&nbsp;&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl archive entry title") & """, function() { window.location.href = 'content.aspx?action=" & _NextActionType & "&id=" & _Id & "&LangType=" & _ContentLanguage & IIf(IsAssetContentType(_ContentTypeSelected, False), "&" & _ContentTypeUrlParam & "=" & MakeArchiveAssetContentType(_ContentTypeSelected), IIf(IsArchiveAssetContentType(_ContentTypeSelected), "&" & _ContentTypeUrlParam & "=" & MakeNonArchiveAssetContentType(_ContentTypeSelected), "")) & "' } );" & Environment.NewLine)
            Else
                result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & "images/UI/Icons/properties.png" & "' />&nbsp;&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl catalog view entry") & """, function() { window.location.href = 'content.aspx?action=" & _NextActionType & "&id=" & _Id & "&LangType=" & _ContentLanguage & IIf(IsAssetContentType(_ContentTypeSelected, False), "&" & _ContentTypeUrlParam & "=" & MakeArchiveAssetContentType(_ContentTypeSelected), IIf(IsArchiveAssetContentType(_ContentTypeSelected), "&" & _ContentTypeUrlParam & "=" & MakeNonArchiveAssetContentType(_ContentTypeSelected), "")) & "' } );" & Environment.NewLine)
            End If
            If ((_PermissionData.CanEditFolders Or _PermissionData.CanEditApprovals) And bViewContent) _
             OrElse bFolderAdmin Then
                result.Append("    viewmenu.addBreak();" & Environment.NewLine)
                result.Append("    viewmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/properties.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl catalog Properties") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=ViewFolder&id=" & _Id & "' } );" & Environment.NewLine)
            End If
            result.Append("    MenuUtil.add( viewmenu );" & Environment.NewLine)
            result.Append("    var deletemenu = new Menu( ""delete"" );" & Environment.NewLine)
            If ((_PermissionData.CanDeleteFolders Or bCommerceAdmin) And bViewContent And _Id <> 0) Then
                result.Append("    deletemenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/folderGreenDelete.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl this catalog") & """, function() { if( ConfirmFolderDelete(" & _Id & ") ) { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=DoDeleteFolder&id=" & _Id & "&ParentID=" & ParentId & "'; }} );" & Environment.NewLine)
            End If
            If (entryCount > 0) AndAlso (bViewContent _
             And (_PermissionData.IsAdmin _
              OrElse bFolderAdmin) OrElse _PermissionData.CanDelete) Then
                If ((_EnableMultilingual = "1") And (CInt(_ContentLanguage) < 1)) Then
                    result.Append("    deletemenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/brickDelete.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl catalog del entry") & """, function() { alert('A language must be selected!'); } );" & Environment.NewLine)
                Else
                    '44595 -  Delete content from the archive view should show up archived list rather than live content list.
                    If _PageAction = "viewarchivecontentbycategory" Then
                        result.Append("    deletemenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/brickDelete.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl catalog del entry") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=DeleteContentByCategory&id=" & _Id & "&showarchive=true'; } );" & Environment.NewLine)
                    Else
                        result.Append("    deletemenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/brickDelete.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl catalog del entry") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=DeleteContentByCategory&id=" & _Id & "'; } );" & Environment.NewLine)
                    End If
                End If
            End If
            result.Append("    MenuUtil.add( deletemenu );" & Environment.NewLine)
        End If
        result.Append("    var actionmenu = new Menu( ""action"" );" & Environment.NewLine)
        result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/magnifier.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("generic Search") & """, function() { window.location.href = 'productsearch.aspx?LangType=" & _ContentLanguage & "&action=showdlg&folderid=" & _Id & "'; } );" & Environment.NewLine)

        result.Append("    actionmenu.addBreak();" & Environment.NewLine)

        If (_CheckedInOrApproved And bViewContent _
          And (_PermissionData.IsAdmin OrElse IsFolderAdmin() OrElse IsCopyOrMoveAdmin()) _
          And (_PermissionData.CanAdd Or _PermissionData.CanEdit)) Then
            If ((_EnableMultilingual = "1") And (CInt(_ContentLanguage) < 1)) Then
                result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/cut.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl cut") & """, function() { alert('A language must be selected!'); } );" & Environment.NewLine)
                result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/contentCopy.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl copy") & """, function() { alert('A language must be selected!'); } );" & Environment.NewLine)
            Else
                result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/cut.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl cut") & """, function() { setClipBoard(); } );" & Environment.NewLine)
                result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/UI/Icons/contentCopy.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl copy") & """, function() { setCopyClipBoard(); }) ;" & Environment.NewLine)
            End If
        End If

        result.Append("    MenuUtil.add( actionmenu );" & Environment.NewLine)
        result.Append("    </script>" & Environment.NewLine)
        result.Append("" & Environment.NewLine)
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Function GetSyncMenuOption() As String
        Dim site As SiteAPI = New SiteAPI
        Dim ekSiteRef As EkSite = site.EkSiteRef()
        Dim result As New System.Text.StringBuilder

        If (LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Feature.eSync)) AndAlso (_ContentApi.RequestInformationRef.IsSyncEnabled()) Then
            If _FolderData.IsDomainFolder() Then
                If (_FolderData.ParentId > 0 AndAlso ekSiteRef.IsStaged()) Then 'AndAlso ekSiteRef.MultiSiteFolderSyncEnabled(folder_data.Id)) Then
                    result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/ui/icons/folderSync.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("btn sync folder") & """, function() { Ektron.Sync.checkMultipleConfigs('" & _ContentLanguage & "',null, null, null,'" & _Id & "' , false, true); return false; } );" & Environment.NewLine)
                End If
            Else
                If (ekSiteRef.IsStaged()) Then 'AndAlso ekSiteRef.FolderSyncEnabled()) Then
                    result.Append("    actionmenu.addItem(""&nbsp;<img src='" & "images/ui/icons/folderSync.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("btn sync folder") & """, function() { Ektron.Sync.checkMultipleConfigs('" & _ContentLanguage & "', null, null, null,'" & _Id & "' ,false, false);return false; } );" & Environment.NewLine)
                End If
            End If
        End If
        Return result.ToString()
    End Function

    Private Sub Populate_ViewCatalogGrid(ByVal folder_data As Ektron.Cms.Common.EkContentCol, ByVal entryList As System.Collections.Generic.List(Of EntryData))
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strTag As String
        Dim strtag1 As String
        Dim strLang As New Hashtable
        Dim langDesc As String = String.Empty

        strTag = "<a href=""content.aspx?LangType=" & _ContentLanguage & "&action=" & _PageAction & "&orderby="
        strtag1 = "&id=" & _Id & IIf(_ContentTypeQuerystringParam <> "", "&" & _ContentTypeUrlParam & "=" & _ContentTypeQuerystringParam, "") & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>"

        colBound.DataField = "TITLE"
        colBound.HeaderText = strTag & "Title" & strtag1 & _MessageHelper.GetMessage("generic title") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "CONTENTTYPE"
        colBound.HeaderText = strTag & "Type" & strtag1 & _MessageHelper.GetMessage("lbl product type xml config") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LANGUAGE"
        colBound.HeaderText = strTag & "language" & strtag1 & _MessageHelper.GetMessage("generic language") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = strTag & "ID" & strtag1 & _MessageHelper.GetMessage("generic ID") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STATUS"
        colBound.HeaderText = strTag & "status" & strtag1 & _MessageHelper.GetMessage("generic Status") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TYPE"
        colBound.HeaderText = strTag & "entrytype" & strtag1 & _MessageHelper.GetMessage("lbl product type class") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header  center"
        colBound.ItemStyle.CssClass = "center"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "SALEPRICE"
        colBound.HeaderText = strTag & "sale" & strtag1 & _MessageHelper.GetMessage("lbl sale price") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "right"
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LISTPRICE"
        colBound.HeaderText = strTag & "list" & strtag1 & _MessageHelper.GetMessage("lbl list price") & "</a>"
        colBound.HeaderStyle.CssClass = "title-header center"
        colBound.ItemStyle.CssClass = "right"
        FolderDataGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("CONTENTTYPE", GetType(String)))
        dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(Int64)))
        dt.Columns.Add(New DataColumn("STATUS", GetType(String)))
        dt.Columns.Add(New DataColumn("TYPE", GetType(String)))
        dt.Columns.Add(New DataColumn("SALEPRICE", GetType(String)))
        dt.Columns.Add(New DataColumn("LISTPRICE", GetType(String)))
        Dim ViewUrl As String = ""
        Dim EditUrl As String = ""
        Dim i As Integer
        Dim bAssetItem As Boolean = False

        For i = 0 To (entryList.Count - 1)
            dr = dt.NewRow()
            If (entryList(i).ContentStatus = "A") Then
                ViewUrl = "content.aspx?action=View&folder_id=" & _Id & "&id=" & entryList(i).Id & "&LangType=" & entryList(i).LanguageId & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING"))
            Else
                ViewUrl = "content.aspx?action=ViewStaged&folder_id=" & _Id & "&id=" & entryList(i).Id & "&LangType=" & entryList(i).LanguageId & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING"))
            End If
            Dim dmsMenuGuid As String
            dmsMenuGuid = System.Guid.NewGuid().ToString()
            Dim makeUnique As String = entryList(i).Id & entryList(i).LanguageId & dmsMenuGuid
            Dim contentType As Long = EkEnumeration.CMSContentType.CatalogEntry

            dr(0) = "<div class=""dmsWrapper"""
            'dr(0) = dr(0) + " onmouseover=""dmsMenuDestroyMenu('" & entryList(i).Id & "','" & entryList(i).LanguageId & "','" & dmsMenuGuid & "', false);"""
            dr(0) = dr(0) & " id=""dmsWrapper" & makeUnique & """>"
            dr(0) = dr(0) & "<p class=""dmsItemWrapper"""
            dr(0) = dr(0) & " id=""dmsItemWrapper" & makeUnique & """"
            dr(0) = dr(0) & " title=""View Menu"""
            dr(0) = dr(0) & " style=""overflow:visible;"""
            dr(0) = dr(0) & ">"
            dr(0) = dr(0) & "<input type=""hidden"" value='{""id"":" & entryList(i).Id & ","
            dr(0) = dr(0) & """parentId"":" & entryList(i).FolderId & ","
            dr(0) = dr(0) & """languageId"":" & entryList(i).LanguageId & ","
            dr(0) = dr(0) & """status"":""" & entryList(i).ContentStatus & ""","
            dr(0) = dr(0) & """guid"":""" & dmsMenuGuid & ""","
            dr(0) = dr(0) & """communityDocumentsMenu"": """","
            dr(0) = dr(0) & """contentType"":" & contentType & ","
            dr(0) = dr(0) & """dmsSubtype"":""""}'"
            dr(0) = dr(0) & " id=""dmsContentInfo" & makeUnique & """ />"
            Select Case entryList(i).EntryType
                Case CatalogEntryType.SubscriptionProduct
                    dr(0) = dr(0) + "<img src=""" & _ContentApi.AppPath & "images/ui/icons/bookGreen.png" & """ onclick=""event.cancelBubble=true;"" />"
                Case CatalogEntryType.Product
                    dr(0) = dr(0) + "<img src=""" & _ContentApi.AppPath & "Images/ui/icons/brick.png" & """ onclick=""event.cancelBubble=true;"" />"
                Case CatalogEntryType.ComplexProduct
                    dr(0) = dr(0) + "<img src=""" & _ContentApi.AppPath & "Images/ui/icons/bricks.png" & """ onclick=""event.cancelBubble=true;"" />"
                Case CatalogEntryType.Kit
                    dr(0) = dr(0) + "<img src=""" & _ContentApi.AppPath & "Images/ui/icons/box.png" & """ onclick=""event.cancelBubble=true;"" />"
                Case CatalogEntryType.Bundle
                    dr(0) = dr(0) + "<img src=""" & _ContentApi.AppPath & "Images/ui/icons/package.png"" onclick=""event.cancelBubble=true;"" />"
            End Select
            dr(0) = dr(0) + "<a"
            dr(0) = dr(0) + " id=""dmsViewItemAnchor" & makeUnique & """"
            dr(0) = dr(0) + " class=""dmsViewItemAnchor"""
            dr(0) = dr(0) + " onclick=""event.cancelBubble=true;"""
            dr(0) = dr(0) + " href=""" & ViewUrl & """"
            dr(0) = dr(0) + " title=""View " & entryList(i).Title & """"
            dr(0) = dr(0) + ">" & entryList(i).Title
            dr(0) = dr(0) + "</a>"
            dr(0) = dr(0) + "</p>"
            dr(0) = dr(0) + "</div>"

            strLang = _ContentApi.GetLanguageByIDV48(entryList.Item(i).LanguageId)
            langDesc = strLang.Item("Name")

            dr(1) = GetContentTypeText(contentType, entryList(i).ProductType.Id)
            dr(2) = "<a href=""#Language"" onclick=""return false;"" onmouseover=""ddrivetip('" & langDesc.ToString() & "','ADC5EF', 100);"" onmouseout=""hideddrivetip()"" style=""text-decoration:none;"">" & "<img src='" & _LocalizationApi.GetFlagUrlByLanguageID(entryList(i).LanguageId) & "' alt=""Flag"" />" & "</a>"
            dr(3) = entryList(i).Id
            dr(4) = _StyleHelper.StatusWithToolTip(entryList(i).ContentStatus)
            dr(5) = entryList(i).EntryType.ToString
            dr(6) = Ektron.Cms.Common.EkFunctions.FormatCurrency(entryList(i).SalePrice, _ContentApi.RequestInformationRef.CommerceSettings.CurrencyCultureCode)
            dr(7) = Ektron.Cms.Common.EkFunctions.FormatCurrency(entryList(i).ListPrice, _ContentApi.RequestInformationRef.CommerceSettings.CurrencyCultureCode)
            dt.Rows.Add(dr)
        Next i

        Dim dv As New DataView(dt)
        FolderDataGrid.DataSource = dv
        FolderDataGrid.DataBind()
    End Sub

#End Region

#Region "DiscussionBoard/forum/topic/replies"

    Private Sub ViewDiscussionBoardToolBar()
        Dim result As New System.Text.StringBuilder
        Dim altText As String = ""
        Dim ParentId As Long = _FolderData.ParentId
        Dim count As Integer = 0
        Dim lAddMultiType As Integer = 0
        Dim bSelectedFound As Boolean = False
        Dim bShowAddMenu As Boolean = False
        Dim bShowViewMenu As Boolean = False
        If (_PageAction = "viewcontentbycategory") Then
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view contents of dboard msg") & " """ & _FolderData.Name & """") & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' />"
            altText = _MessageHelper.GetMessage("Archive Content Title")
        Else
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view archive content title") & " """ & _FolderData.Name & """") & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & _LocalizationApi.GetFlagUrlByLanguageID(_ContentLanguage) & "' />"
            altText = _MessageHelper.GetMessage("view content title")
        End If
        result.Append("<table><tr>" & vbCrLf)
        If ((_PermissionData.CanAdd) And (_PageAction = "viewcontentbycategory")) Or _PermissionData.IsReadOnly = True Then
            If ((_PermissionData.CanAdd) And (_PageAction = "viewcontentbycategory")) Then
                If Not bSelectedFound Then
                    _ContentType = _CMSContentType_AllTypes
                End If
            End If
        End If
        If (_PermissionData.CanAddFolders And _PageAction = "viewcontentbycategory") Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'file');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'file');"" onmouseout=""this.className='menuRootItem'""><span class=""new"">" & _MessageHelper.GetMessage("lbl New") & "</span></td>")
            result.Append("<script type=""text/javascript"">" & Environment.NewLine)
            result.Append("    var filemenu = new Menu( ""file"" );" & Environment.NewLine)
            result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/UI/Icons/folderBoard.png' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl add disc forum") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=AddSubFolder&type=discussionforum&id=" & _Id & "' } );" & Environment.NewLine)
            result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/UI/Icons/users.png' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl discussionforumsubject") & """, function() { window.location.href = 'threadeddisc/addeditboard.aspx?LangType=" & _ContentApi.ContentLanguage & "&action=addcat&id=" & _Id.ToString() & "' } );" & Environment.NewLine)
            If (_ContentApi.IsAdmin = True) Or IsFolderAdmin() Then
                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/Icons/restrictedIps.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl restricted ip") & """, function() { window.location.href = 'threadeddisc/restrictIP.aspx?action=edit&fromboard=true&boardid=" & _Id & "' } );" & Environment.NewLine)
                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/Icons/replaceWord.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl replace word") & """, function() { window.location.href = 'threadeddisc/replacewords.aspx?action=edit&fromboard=true&boardid=" & _Id & "' } );" & Environment.NewLine)
                result.Append("    filemenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/Icons/userRanks.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl user rank") & """, function() { window.location.href = 'threadeddisc/userranks.aspx?action=edit&fromboard=true&boardid=" & _Id & "' } );" & Environment.NewLine)
            End If
            result.Append("    MenuUtil.add( filemenu );" & Environment.NewLine)
            result.Append("    </script>" & Environment.NewLine)
        End If

        'The properties button should be far right.
        result.Append("<script type=""text/javascript"">" & Environment.NewLine)
        result.Append("    var viewmenu = new Menu( ""view"" );" & Environment.NewLine)
        If (_ContentApi.IsAdmin = True) Or IsFolderAdmin() Then
            bShowViewMenu = True
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/Icons/permissions.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl permissions") & """, function() { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=ViewPermissions&type=folder&id=" & _Id & "' } );" & Environment.NewLine)
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/Icons/restrictedIPs.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl restricted ips") & """, function() { window.location.href = 'threadeddisc/restrictIP.aspx?boardid=" & _Id & "' } );" & Environment.NewLine)
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/Icons/replaceWord.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl replace words") & """, function() { window.location.href = 'threadeddisc/replacewords.aspx?boardid=" & _Id & "' } );" & Environment.NewLine)
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/Icons/userRanks.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl user ranks") & """, function() { window.location.href = 'threadeddisc/userranks.aspx?boardid=" & _Id & "' } );" & Environment.NewLine)
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/Icons/notification.png" & " ' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl notifications") & """, function() { window.location.href = 'subscriptionmessages.aspx?mode=forum&fromboard=true&boardid=" & _Id & "' } );" & Environment.NewLine)
        End If
        If (_ContentApi.IsAdmin = True Or IsFolderAdmin()) Then
            result.Append("    viewmenu.addItem(""&nbsp;<img valign='middle' src='" & _ContentApi.AppPath & "images/ui/Icons/properties.png' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("properties text") & """, function() { window.location.href = 'threadeddisc/addeditboard.aspx?LangType=" & _ContentLanguage & "&action=View&id=" & _Id.ToString() & "' } );" & Environment.NewLine)
            bShowViewMenu = True
        End If
        If bShowViewMenu = True Then
            result.Append("    MenuUtil.add( viewmenu );" & Environment.NewLine)
        End If
        result.Append("    var deletemenu = new Menu( ""delete"" );" & Environment.NewLine)
        If (_PermissionData.CanDeleteFolders And _PageAction = "viewcontentbycategory" And _Id <> 0) Then
            result.Append("    deletemenu.addItem(""&nbsp;<img src='" & _ContentApi.AppPath & "images/ui/Icons/folderBoardDelete.png' />&nbsp;&nbsp;" & _MessageHelper.GetMessage("lbl This Folder") & """, function() { if( ConfirmFolderDelete(" & _Id & ") ) { window.location.href = 'content.aspx?LangType=" & _ContentLanguage & "&action=DoDeleteFolder&id=" & _Id & "&ParentID=" & ParentId & "'; }} );" & Environment.NewLine)
        End If
        result.Append("    MenuUtil.add( deletemenu );" & Environment.NewLine)
        result.Append("    </script>" & Environment.NewLine)
        SetViewImage(96)
        If bShowViewMenu = True Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'view');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'view');"" onmouseout=""this.className='menuRootItem'""><span class=""folderView"">" & _MessageHelper.GetMessage("lbl View") & "</span></td>")
        End If
        If ((_PageAction = "viewcontentbycategory") _
          And (_PermissionData.IsAdmin _
          OrElse IsFolderAdmin())) Then
            result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'delete');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'delete');"" onmouseout=""this.className='menuRootItem'""><span class=""delete"">" & _MessageHelper.GetMessage("lbl Delete") & "</span></td>")
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton(_StyleHelper.GetHelpAliasPrefix(_FolderData) & _PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub Populate_ViewDiscussionBoardGrid()
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim adcCategories As DiscussionCategory()
        Dim dt As New DataTable
        Dim dr As DataRow

        pnlThreadedDiscussions.Visible = True
        adcCategories = _EkContent.GetCategoriesforBoard(_Id)
        _DiscussionForums = _EkContent.GetForumsforBoard(_Id)

        dt.Columns.Add(New DataColumn("name", GetType(String)))
        dt.Columns.Add(New DataColumn("id", GetType(Long)))

        If Not (adcCategories Is Nothing) AndAlso (adcCategories.Length > 0) Then
            For j As Integer = 0 To (adcCategories.Length - 1)
                dr = dt.NewRow()
                dr(0) = adcCategories(j).Name
                dr(1) = adcCategories(j).CategoryID
                dt.Rows.Add(dr)
            Next
        End If

        Dim dv As New DataView(dt)
        CategoryList.DataSource = dv
        CategoryList.DataBind()
    End Sub

    Private Sub ViewDiscussionForumToolBar()
        Dim result As New System.Text.StringBuilder
        Dim altText As String = ""
        Dim ParentId As Long = _FolderData.ParentId
        Dim count As Integer = 0
        Dim lAddMultiType As Integer = 0
        Dim bSelectedFound As Boolean = False

        If (_PageAction = "viewcontentbycategory") Then
            altText = _MessageHelper.GetMessage("Archive forum Title")
        Else
            altText = _MessageHelper.GetMessage("view forum title")
        End If
        If (_PageAction = "viewcontentbycategory") Then
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view content of forum msg") & " """ & _FolderData.Name & """")
        Else
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view archive forum title") & " """ & _FolderData.Name & """")
        End If
        result.Append("<table><tr>" & vbCrLf)
        If ((_PermissionData.CanAdd) And (_PageAction = "viewcontentbycategory")) Or _PermissionData.IsReadOnly = True Then
            If ((_PermissionData.CanAdd) And (_PageAction = "viewcontentbycategory")) Then
                If Not bSelectedFound Then
                    _ContentType = _CMSContentType_AllTypes
                End If
                ' Don't allow user to add content if IsMac and XML-Config assigned to this folder:
                If (Not (_IsMac And _HasXmlConfig)) Or ("ContentDesigner" = _SelectedEditControl) Then
                    If Not (Request.QueryString("ContType") = CMSContentType_BlogComments) Then
                        If ((_EnableMultilingual = "1") And (CInt(_ContentLanguage) < 1)) Then
                            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/add.png", "javascript:AddNewTopic();", _MessageHelper.GetMessage("add topic msg"), _MessageHelper.GetMessage("btn add forumpost"), ""))
                        Else
                            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/add.png", "threadeddisc/addedittopic.aspx?action=add&id=" & _Id.ToString(), _MessageHelper.GetMessage("add topic msg"), _MessageHelper.GetMessage("btn add forumpost"), ""))
                        End If
                    End If
                End If
            End If
        End If
        If (_PermissionData.IsAdmin And _TakeAction And _PageAction = "viewcontentbycategory") Then
            If ((_EnableMultilingual = "1") And (CInt(_ContentLanguage) < 1)) Then
                result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/delete.png", "javascript:alert('A language must be selected!');", _MessageHelper.GetMessage("alt btn deletetopics"), _MessageHelper.GetMessage("btn deletetopics"), ""))
            Else
                result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/delete.png", "content.aspx?LangType=" & _ContentLanguage & "&action=DeleteContentByCategory&id=" & _Id, _MessageHelper.GetMessage("alt btn deletetopics"), _MessageHelper.GetMessage("btn deletetopics"), ""))
            End If
        End If
        'The properties button should be far right.
        If ((_PermissionData.CanEditFolders Or _PermissionData.CanEditApprovals) And _PageAction = "viewcontentbycategory") _
         OrElse IsFolderAdmin() Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/properties.png", "threadeddisc/addeditforum.aspx?LangType=" & _ContentLanguage & "&action=View&id=" & _Id, _MessageHelper.GetMessage("alt forum properties button text"), _MessageHelper.GetMessage("btn properties"), ""))
        End If

        If (_PermissionData.IsAdmin OrElse _ContentApi.IsARoleMemberForFolder(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminFolderUsers, _Id, _CurrentUserId, False)) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/permissions.png", "content.aspx?LangType=" & _ContentLanguage & "&action=ViewPermissions&type=folder&id=" & _Id, _MessageHelper.GetMessage("alt permissions button text forum (view)"), _MessageHelper.GetMessage("btn view permissions"), ""))
        End If

        If (((_PermissionData.IsAdmin) Or (_PermissionData.CanDeleteFolders)) And _PageAction = "viewcontentbycategory" And _Id <> 0) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/folderBoardDelete.png", "content.aspx?LangType=" & _ContentLanguage & "&action=DoDeleteFolder&id=" & _Id & "&ParentID=" & ParentId, _MessageHelper.GetMessage("alt delete forum button text"), _MessageHelper.GetMessage("btn delete forum"), "onclick=""return ConfirmFolderDelete(" & _Id & ");"" "))
        End If

        If _From = "dashboard" Then
            'result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/back.png", "dashboard.aspx", _MessageHelper.GetMessage("alt back button"), _MessageHelper.GetMessage("btn back"), ""))
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        ElseIf (Request.QueryString("ContType") = CMSContentType_BlogComments) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/back.png", "content.aspx?action=ViewContentByCategory&id=" & _Id, _MessageHelper.GetMessage("alt back button"), _MessageHelper.GetMessage("btn back"), ""))
        End If
        If _EnableMultilingual = 1 Then
            Dim m_refsite As New SiteAPI
            Dim language_data() As LanguageData = m_refsite.GetAllActiveLanguages

            result.Append("<td class=""label"">&nbsp;|&nbsp;" & _MessageHelper.GetMessage("lbl View") & ": ")
            result.Append("<select id=""selLang"" name=""selLang"" OnChange=""LoadLanguage(this.options[this.selectedIndex].value);"">")
            If _ContentLanguage = -1 Then
                result.Append("<option value=" & ALL_CONTENT_LANGUAGES & " selected>All</option>")
            Else
                result.Append("<option value=" & ALL_CONTENT_LANGUAGES & ">All</option>")
            End If
            For count = 0 To language_data.Length - 1
                If Convert.ToString(_ContentLanguage) = Convert.ToString(language_data(count).Id) Then
                    result.Append("<option value=" & language_data(count).Id & " selected>" & language_data(count).Name & "</option>")
                Else
                    result.Append("<option value=" & language_data(count).Id & ">" & language_data(count).Name & "</option>")
                End If
            Next
            result.Append("</select></td>")
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton(_StyleHelper.GetHelpAliasPrefix(_FolderData) & _PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub Populate_ViewForumPostsByCategoryGrid(ByVal topics As DiscussionTopic(), ByVal commenttally As ArrayList)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim objUserAPI As New UserAPI
        Dim objUserData As New UserData

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "APPROVAL"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(12)
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TOPIC"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.CssClass = "left"
        colBound.HeaderText = _MessageHelper.GetMessage("lbl Topic")
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "STARTER"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(80)
        colBound.HeaderText = _MessageHelper.GetMessage("topicstarter text")
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "REPLIES"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderText = _MessageHelper.GetMessage("lbl replies")
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "VIEWS"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderText = _MessageHelper.GetMessage("views lbl")
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "LASTPOST"
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(200)
        colBound.HeaderText = _MessageHelper.GetMessage("lbl Last Reply")
        FolderDataGrid.Columns.Add(colBound)

        FolderDataGrid.ShowHeader = True

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("APPROVAL", GetType(String)))
        dt.Columns.Add(New DataColumn("TOPIC", GetType(String)))
        dt.Columns.Add(New DataColumn("STARTER", GetType(String)))
        dt.Columns.Add(New DataColumn("REPLIES", GetType(String)))
        dt.Columns.Add(New DataColumn("VIEWS", GetType(String)))
        dt.Columns.Add(New DataColumn("LASTPOST", GetType(String)))

        Dim ViewUrl As String = ""
        Dim EditUrl As String = ""
        Dim i As Integer
        Dim bNewComment As Boolean = False
        Dim iReplyTally As Integer = 0
        Dim dtLastPosted As DateTime
        Dim istart As Integer = 0
        Dim iend As Integer = 0

        For i = 0 To (topics.Length - 1)
            If topics(i).LanguageId = _ContentLanguage Or _ContentLanguage = ALL_CONTENT_LANGUAGES Then
                dr = dt.NewRow()
                'reset these values
                iReplyTally = 0
                bNewComment = False
                For j As Integer = 0 To (commenttally.Count - 1)
                    If CLng(commenttally(j)(0)) = CLng(topics(i).Id) Then
                        iReplyTally = CLng(commenttally(j)(1))
                        bNewComment = (CLng(commenttally(j)(2)) < (iReplyTally * 7))
                        If iReplyTally > 0 Then
                            dtLastPosted = Convert.ToDateTime(commenttally(j)(3))
                        End If
                        commenttally.RemoveAt(j) ' remove so we don't need to go through this again
                        Exit For
                    End If
                Next
                iReplyTally = topics(i).Replies
                If iReplyTally > 0 Then
                    dtLastPosted = topics(i).LastPostedDate
                End If
                If (bNewComment Or topics(i).Status.ToUpper() = "I") AndAlso (_PermissionData.IsAdmin Or _PermissionData.CanAddToImageLib) Then
                    dr(0) = "<img src=""images/UI/Icons/approvalApproveItem.png"" alt=""" & Me._MessageHelper.GetMessage("lbl approval needed") & """ title=""" & Me._MessageHelper.GetMessage("lbl approval needed") & """ />"
                Else
                    dr(0) = ""
                End If
                Select Case topics(i).Priority
                    Case DiscussionObjPriority.Announcement
                        dr(1) = "<img title=""Announcement"" src=""" + _ContentApi.AppPath + "images/ui/icons/asteriskRed.png"" style=""margin-right:.25em; vertical-align: middle"" />"
                    Case DiscussionObjPriority.Sticky
                        dr(1) = "<img title=""Sticky Topic"" src=""" + _ContentApi.AppPath + "images/ui/icons/asteriskYellow.png"" style=""margin-right:.25em; vertical-align: middle"" />"
                    Case Else ' DiscussionObjPriority.Normal
                        dr(1) = "<img title=""Topic"" src=""" + _ContentApi.AppPath + "images/ui/icons/asteriskOrange.png"" style=""margin-right:.25em; vertical-align: middle"" />"
                End Select

                ViewUrl = "content.aspx?id=" & _Id & "&action=ViewContentByCategory&LangType=" & topics(i).LanguageId & "&ContType=" & CMSContentType_BlogComments & "&contentid=" & topics(i).Id.ToString() 'view posts
                EditUrl = "content.aspx?action=View&folder_id=" & _Id & "&id=" & topics(i).Id & "&LangType=" & topics(i).LanguageId & "&callerpage=content.aspx&origurl=" & Server.UrlEncode(Request.ServerVariables("QUERY_STRING")) 'more traditional content view
                dr(1) &= "<a href=""" & ViewUrl & """ title='" & _MessageHelper.GetMessage("generic View") & " """ & Replace(topics(i).Title & """", "'", "`") & "'" & ">" & topics(i).Title & " </a>"

                objUserData = objUserAPI.GetActiveUserById(topics(i).UserId)
                If (objUserData) IsNot Nothing AndAlso objUserData.Username <> "" Then
                    dr(2) = objUserData.Username
                Else
                    dr(2) = topics(i).UserId
                End If

                'replies col
                dr(3) = "<a href=""" & ViewUrl & """>" & iReplyTally & "</a>"
                'status col
                dr(4) = topics(i).Views
                'last post col
                If iReplyTally > 0 Then
                    If dtLastPosted.Date.Equals(Now().Date) Then
                        dr(5) = _MessageHelper.GetMessage("lbl today at") & " " & dtLastPosted.ToShortTimeString()
                    ElseIf dtLastPosted.Date.AddDays(1).Equals(Now.Date()) Then
                        dr(5) = _MessageHelper.GetMessage("lbl yesterday at") & " " & dtLastPosted.ToShortTimeString()
                    Else
                        dr(5) = dtLastPosted.ToLongDateString() & " " & dtLastPosted.ToShortTimeString()
                    End If
                Else
                    dr(5) = "-"
                End If
                dt.Rows.Add(dr)
            End If
        Next i
        Dim dv As New DataView(dt)
        FolderDataGrid.DataSource = dv
        FolderDataGrid.DataBind()
    End Sub

    Private Sub ViewRepliesToolBar()
        Dim result As New System.Text.StringBuilder
        Dim altText As String = ""
        Dim ParentId As Long = _FolderData.ParentId
        Dim count As Integer = 0
        Dim lAddMultiType As Integer = 0
        Dim bSelectedFound As Boolean = False

        If (_PageAction = "viewcontentbycategory") Then
            altText = _MessageHelper.GetMessage("Archive Content Title")
        Else
            altText = _MessageHelper.GetMessage("view content title")
        End If
        If (_PageAction = "viewcontentbycategory") Then
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view replies in topic msg") & " """ & _ContentData.Title & """")
        Else
            txtTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view archive content title") & " """ & _ContentData.Title & """")
        End If
        result.Append("<table><tr>" & vbCrLf)

        If (_PermissionData.CanAddTask) And _PermissionData.IsReadOnlyLib = True Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/add.png", "threadeddisc/addeditreply.aspx?action=Add&topicid=" & _ContentId & "&forumid=" & _Id & "&id=0", _MessageHelper.GetMessage("alt btn add reply"), _MessageHelper.GetMessage("btn add reply"), ""))
        End If

        If (_PermissionData.CanDelete And _PageAction = "viewcontentbycategory" And _Id <> 0) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/delete.png", "content.aspx?LangType=" & _ContentApi.ContentLanguage & "&action=submitDelContAction&delete_id=" & _ContentId & "&page=&folder_id=" & _Id, _MessageHelper.GetMessage("alt delete topic button text"), _MessageHelper.GetMessage("btn delete topic"), " OnClick=""return ConfirmDelete(true);return false;"" "))
        End If


        If _ContentData.Status.ToUpper() = "I" AndAlso (_PermissionData.CanAddToImageLib = True Or _PermissionData.IsAdmin = True) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppImgPath & "icon_verify_good.gif", "threadeddisc/addedittopic.aspx?id=" & _ContentId & "&folderid=" & _Id & "&action=approve&LangType=" & _ContentLanguage.ToString(), _MessageHelper.GetMessage("alt approve topic"), _MessageHelper.GetMessage("lbl approve topic"), ""))
        End If
        If _ContentData.Status.ToUpper() <> "I" AndAlso _PermissionData.CanEdit Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/properties.png", "threadeddisc/addedittopic.aspx?id=" & _ContentId & "&action=view&LangType=" & _ContentLanguage.ToString(), _MessageHelper.GetMessage("alt properties button text"), _MessageHelper.GetMessage("btn topic properties"), ""))
        End If

        If _From = "dashboard" Then
            'result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/back.png", "dashboard.aspx", _MessageHelper.GetMessage("alt back button"), _MessageHelper.GetMessage("btn back"), ""))
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        ElseIf (Request.QueryString("ContType") = CMSContentType_BlogComments) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/ui/Icons/back.png", "content.aspx?action=ViewContentByCategory&id=" & _Id, _MessageHelper.GetMessage("alt back button"), _MessageHelper.GetMessage("btn back"), ""))
        End If
        If Not (Me._ContentType = CMSContentType_BlogComments) And (_EnableMultilingual = 1) Then
            Dim m_refsite As New SiteAPI
            Dim language_data() As LanguageData = m_refsite.GetAllActiveLanguages

            result.Append("<td class=""label"">&nbsp;|&nbsp;" & _MessageHelper.GetMessage("lbl View") & ":")
            result.Append("<select id=selLang name=selLang OnChange=""javascript:LoadLanguage(this.options[this.selectedIndex].value);"">")
            If _ContentLanguage = -1 Then
                result.Append("<option value=" & ALL_CONTENT_LANGUAGES & " selected>All</option>")
            Else
                result.Append("<option value=" & ALL_CONTENT_LANGUAGES & ">All</option>")
            End If
            For count = 0 To language_data.Length - 1
                If Convert.ToString(_ContentLanguage) = Convert.ToString(language_data(count).Id) Then
                    result.Append("<option value=" & language_data(count).Id & " selected>" & language_data(count).Name & "</option>")
                Else
                    result.Append("<option value=" & language_data(count).Id & ">" & language_data(count).Name & "</option>")
                End If
            Next
            result.Append("</select></td>")
        End If
        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton(_StyleHelper.GetHelpAliasPrefix(_FolderData) & "topics_" & _PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub Populate_ViewTopicRepliesGrid(ByVal replydata As Ektron.Cms.Content.EkTasks)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim strTag As String
        Dim strtag1 As String
        Dim nReplyCount As Integer
        strTag = "<a href=""content.aspx?LangType=" & _ContentApi.ContentLanguage & "&action=" & _PageAction & "&orderby="
        strtag1 = "&id=" & _Id & IIf(_ContentTypeQuerystringParam <> "", "&" & _ContentTypeUrlParam & "=" & _ContentTypeQuerystringParam, "") & """ title=""" & _MessageHelper.GetMessage("click to sort msg") & """>"

        FolderDataGrid.ShowHeader = False

        colBound.DataField = "PREVIEW"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(145)
        FolderDataGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLEDESCRIPTION"
        'colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        FolderDataGrid.Columns.Add(colBound)

        FolderDataGrid.BorderColor = Drawing.Color.White
        FolderDataGrid.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0)
        FolderDataGrid.CellPadding = 6
        FolderDataGrid.CellSpacing = 2

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("PREVIEW", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLEDESCRIPTION", GetType(String)))

        Dim ApproveURL As String = ""
        Dim ViewUrl As String = ""
        Dim EditUrl As String = ""
        Dim DeleteUrl As String = ""
        Dim ReplyURL As String = ""
        Dim sAppend As String = ""
        Dim i As Integer
        nReplyCount = replydata.Count
        Dim contapi As New Ektron.Cms.ContentAPI

        dr = dt.NewRow()

        For i = 1 To replydata.Count
            If Not (replydata.Item(i) Is Nothing) Then
                If CInt(replydata.Item(i).State) = BlogCommentState.Completed Or (CInt(replydata.Item(i).State) = BlogCommentState.Pending AndAlso (((Not (_PermissionData Is Nothing) AndAlso (_PermissionData.IsAdmin Or _PermissionData.CanAddToImageLib))) Or Me._ContentApi.RequestInformationRef.CallerId = replydata.Item(i).CreatedByUserID)) Then
                    dr = dt.NewRow()
                    If Request.QueryString("contentid") <> "" Then
                        sAppend = "&blogid=" & _Id.ToString() & "&contentid=" & Request.QueryString("contentid")
                    Else
                        sAppend = "&blogid=" & _Id.ToString()
                    End If
                    ViewUrl = "tasks.aspx?action=ViewTask&tid=" & replydata.Item(i).TaskID.ToString() & "&fromViewContent=1&ty=both&LangType=" & _ContentApi.ContentLanguage
                    EditUrl = "threadeddisc/addeditreply.aspx?action=Edit&topicid=" & _ContentId.ToString() & "&forumid=" & Me._Id.ToString() & "&id=" & replydata.Item(i).TaskID.ToString() & "&boardid=" & _BoardID.ToString()
                    If i = 1 Then
                        EditUrl &= "&type=topic"
                    End If
                    ReplyURL = "threadeddisc/addeditreply.aspx?action=Add&topicid=" & _ContentId.ToString() & "&forumid=" & Me._Id.ToString() & "&id=" & replydata.Item(i).TaskID.ToString() & "&boardid=" & _BoardID.ToString()
                    If CInt(replydata.Item(i).State) = BlogCommentState.Pending Then
                        ApproveURL = "tasksaction.aspx?action=ApproveTask&tid=" & replydata.Item(i).TaskID.ToString() & "&ty=both" & sAppend
                        dr(0) = "<a name=""reply" & replydata.Item(i).TaskID.ToString() & """></a><table border=""0"" cellspacing=""6"" width=""125""><tr><td colspan=""2"" align=""center"">"
                        dr(0) &= "<br/><img border=""4"" style=""border-width: 5px; border-color: gold"" src=""" & _ContentApi.AppImgPath & "thumb_forumpost.gif"" width=""53"" height=""55""/><br/>"
                        dr(0) &= "</td></tr>"
                        dr(0) &= "<tr><td width=""50%"">"
                        If _PermissionData.IsReadOnlyLib = True Then
                            dr(0) &= "<a href=""" & ReplyURL & """>" & _MessageHelper.GetMessage("lbl reply") & "</a>"
                        Else
                            dr(0) &= _MessageHelper.GetMessage("lbl reply")
                        End If
                        dr(0) &= "</td><td width=""50%"">"
                        If _PermissionData.CanAddToImageLib = True Then
                            dr(0) &= "<a href=""" & EditUrl & """>" & _MessageHelper.GetMessage("generic edit title") & "</a>"
                        Else
                            dr(0) &= _MessageHelper.GetMessage("generic edit title")
                        End If
                        dr(0) &= "</td></tr>"
                        If i > 1 Then
                            dr(0) &= "<tr><td width=""50%"">"
                            If (Not (_PermissionData Is Nothing) AndAlso (_PermissionData.IsAdmin Or _PermissionData.CanAddToImageLib)) Then
                                dr(0) &= "<a href=""" & ApproveURL & """>approve</a>"
                            Else
                                dr(0) &= "approve"
                            End If
                        End If
                    Else
                        ApproveURL = ""
                        dr(0) = "<a name=""reply" & replydata.Item(i).TaskID.ToString() & """><table border=""0"" cellspacing=""6"" width=""125""><tr><td colspan=""2"" align=""center"">"
                        dr(0) &= "<br/><img src=""" & _ContentApi.AppImgPath & "thumb_forumpost.gif"" width=""53"" height=""55""/><br/>"
                        dr(0) &= "</td></tr>"
                        dr(0) &= "<tr><td width=""50%"">"
                        If _PermissionData.IsReadOnlyLib = True Then
                            dr(0) &= "<a href=""" & ReplyURL & """>" & _MessageHelper.GetMessage("lbl reply") & "</a>"
                        Else
                            dr(0) &= _MessageHelper.GetMessage("lbl reply")
                        End If
                        dr(0) &= "</td><td width=""50%"">"
                        If _PermissionData.CanAddToImageLib = True Or (i = 1 And _PermissionData.CanEdit) Then
                            dr(0) &= "<a href=""" & EditUrl & """>" & _MessageHelper.GetMessage("generic edit title") & "</a>"
                        Else
                            dr(0) &= _MessageHelper.GetMessage("generic edit title")
                        End If
                        dr(0) &= "</td></tr>"
                        'We do not need approve button when there's no approval for that post reply
                        'If i > 1 Then
                        '    dr(0) &= "<tr><td width=""50%"">"
                        '    dr(0) &= _MessageHelper.GetMessage("btn approve")
                        'End If
                    End If
                    If i > 2 Then
                        DeleteUrl = "tasksaction.aspx?action=DeleteTask&tid=" & replydata.Item(i).TaskID.ToString() & "&ty=both" & sAppend
                        dr(0) &= "</td><td width=""50%"">"
                        If _PermissionData.IsAdmin Or _PermissionData.CanAddToImageLib = True Or contapi.UserId = replydata.Item(i).CreatedByUserID Then
                            dr(0) &= "<a href=""" & DeleteUrl & """ onclick=""return confirm('" & _MessageHelper.GetMessage("msg del comment") & "');"">" & _MessageHelper.GetMessage("generic delete title") & "</a>&nbsp;"
                        Else
                            dr(0) &= _MessageHelper.GetMessage("generic delete title")
                        End If
                        dr(0) &= "</td></tr>"
                    End If
                    dr(0) &= "</table>"
                    If replydata.Item(i).CreatedByUserID = -1 Then
                        dr(1) &= ("<span id=""ReplyDesc"" class=""ReplyDesc"" style=""color:gray;display:block;"">" & (replydata.Item(i).Description) & "</span><span style=""color:green;display:block;"">" & _MessageHelper.GetMessage("lbl posted by") & " " & _MessageHelper.GetMessage("lbl anon") & " " & _MessageHelper.GetMessage("res_isrch_on") & " " & replydata.Item(i).DateCreated.ToString() & "</span>")
                    Else
                        dr(1) &= ("<span id=""ReplyDesc"" class=""ReplyDesc"" style=""color:gray;display:block;"">" & (replydata.Item(i).Description) & "</span><span style=""color:green;display:block;"">" & _MessageHelper.GetMessage("lbl posted by") & " " & replydata.Item(i).CommentDisplayName & " " & _MessageHelper.GetMessage("res_isrch_on") & " " & replydata.Item(i).DateCreated.ToString() & "</span>")
                    End If
                    If (Not (replydata.Item(i).FileAttachments Is Nothing) And replydata.Item(i).FileAttachments.Length > 0) Then
                        dr(1) &= "<br/>"
                        dr(1) &= "<br/>"
                        Dim filetmp As String = ""
                        For k As Integer = 0 To (replydata.Item(i).FileAttachments.Length - 1)
                            If (replydata.Item(i).FileAttachments(k).DoesExist = True) Then
                                filetmp &= ("		<img src='" & Me._ContentApi.AppPath & "images/ui/icons/filetypes/file.png' /> <a href=""" & replydata.Item(i).FileAttachments(k).Filepath & """ target=""_blank"" class=""ekattachment"">" & replydata.Item(i).FileAttachments(k).Filename & "</a> <span class='attachinfo'>(" & replydata.Item(i).FileAttachments(k).FileSize.ToString() & " bytes)</span><br/>" & Environment.NewLine)
                            End If
                        Next
                        If (filetmp.Length > 0) Then ' if we have at least one attachment
                            filetmp = ("		<span class=""ekattachments"">File Attachment(s):</span><br/>" & Environment.NewLine) & filetmp & ("		<br/>" & Environment.NewLine)
                            dr(1) &= (filetmp)
                        End If
                    End If
                    dt.Rows.Add(dr)
                End If
            End If
        Next i
        Dim dv As New DataView(dt)
        FolderDataGrid.DataSource = dv
        FolderDataGrid.DataBind()

        TotalPages.Visible = False
        CurrentPage.Visible = False

    End Sub

    Protected Sub CategoryList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles CategoryList.ItemDataBound
        Dim rptForum As New Repeater
        Dim hdntmp As HtmlInputHidden
        Dim categoryID As Long = 0
        Dim dt As New DataTable
        Dim dr As DataRow

        hdntmp = e.Item.FindControl("hdn_categoryid")
        categoryID = Convert.ToInt64(hdntmp.Value)
        rptForum = e.Item.FindControl("ForumList")

        dt.Columns.Add(New DataColumn("Name", GetType(String)))
        dt.Columns.Add(New DataColumn("SortOrder", GetType(String)))
        dt.Columns.Add(New DataColumn("Description", GetType(String)))
        dt.Columns.Add(New DataColumn("Topics", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Posts", GetType(Integer)))
        dt.Columns.Add(New DataColumn("LastPosted", GetType(String)))

        For i As Integer = 0 To (_DiscussionForums.Length - 1)
            If _DiscussionForums(i).CategoryID = categoryID Then
                dr = dt.NewRow()
                dr(0) = "<a href=""content.aspx?action=ViewContentByCategory&id=" & _DiscussionForums(i).Id.ToString() & """>" & _DiscussionForums(i).Name & "</a>"
                dr(1) = "-"
                dr(2) = _DiscussionForums(i).Description
                dr(3) = _DiscussionForums(i).NumberofTopics
                dr(4) = _DiscussionForums(i).NumberofPosts
                If _DiscussionForums(i).NumberofPosts > 0 Then
                    dr(5) = _DiscussionForums(i).LastPosted.ToLongDateString & " " & _DiscussionForums(i).LastPosted.ToShortTimeString
                Else
                    dr(5) = "-"
                End If
                dt.Rows.Add(dr)
            End If
        Next
        Dim dv As New DataView(dt)
        rptForum.DataSource = dv
        rptForum.DataBind()
    End Sub

    Protected Function Util_GetPageURL(ByVal pageid As Integer) As String

        Return "content.aspx" & Ektron.Cms.Common.EkFunctions.GetUrl(New String() {"currentpage"}, New String() {"pageid"}, Request.QueryString).Replace("pageid", IIf(pageid = -1, "' + pageid + '", pageid)).Replace("&amp;", "&").Replace("showAddEventForm=true", "showAddEventForm=false")

    End Function

    Protected Sub Util_SetJs()
        If _PagingTotalPagesNumber > 1 Then
        Dim sbJS As New StringBuilder()
        sbJS.Append(" function GoToPage(pageid, pagetotal) { ").Append(Environment.NewLine)
        sbJS.Append("     if (pageid <= pagetotal && pageid >= 1) { ").Append(Environment.NewLine)
        sbJS.Append("         window.location.href = '").Append(Util_GetPageURL(-1)).Append("'; ").Append(Environment.NewLine)
        sbJS.Append("     } else { ").Append(Environment.NewLine)
        sbJS.Append("         alert('").Append(String.Format(_MessageHelper.GetMessage("js: err page must be between"), _PagingTotalPagesNumber)).Append("'); ").Append(Environment.NewLine)
        sbJS.Append("     } ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)
        ltr_js.Text = sbJS.ToString()
        End If
    End Sub

#End Region

#Region "Helpers"

    Private Sub GetQueryStringValues()

        _PageAction = Me.GetQueryStringValue("action").ToLower().Trim()
        _From = Me.GetQueryStringValue("from").ToLower().Trim()
        _OrderBy = Me.GetQueryStringValue("orderby")
        _TreeViewId = Request.QueryString("treeViewId")

        Dim pagingCurrentPageNumber As String = Me.GetQueryStringValue("currentpage")
        pagingCurrentPageNumber = IIf(pagingCurrentPageNumber = String.Empty, "1", pagingCurrentPageNumber)
        Int32.TryParse(pagingCurrentPageNumber, _PagingCurrentPageNumber) '_PagingCurrentPageNumber = Convert.ToInt32(pagingCurrentPageNumber)

        Dim id As String = Me.GetQueryStringValue("id")
        id = IIf(id = String.Empty Or id.ToLower() = "undefined", "0", id)
        _Id = Convert.ToInt64(id)

        Dim contentId As String = Me.GetQueryStringValue("contentid")
        contentId = IIf(contentId = String.Empty, "0", contentId)
        _ContentId = Convert.ToInt64(contentId)

        _ContentTypeQuerystringParam = Me.GetQueryStringValue(_ContentTypeUrlParam)
        If _ContentTypeQuerystringParam <> String.Empty Then
            If IsNumeric(_ContentTypeQuerystringParam) Then
                _ContentTypeSelected = _ContentTypeUrlParam
                _ContentTypeSelected = CLng(_ContentTypeQuerystringParam)
                _ContentApi.SetCookieValue(_ContentTypeUrlParam, _ContentTypeQuerystringParam)
            ElseIf (_ContentTypeQuerystringParam.Length > 2 And _ContentTypeQuerystringParam.Substring(0, 3) = "14_") Then
                _ContentTypeSelected = 14
                _XmlConfigID = CLng(_ContentTypeQuerystringParam.Substring(3, _ContentTypeQuerystringParam.Length - 3))
                _ContentApi.SetCookieValue(_XmlConfigType, _XmlConfigID)
            End If
        ElseIf Ektron.Cms.CommonApi.GetEcmCookie()(_ContentTypeUrlParam) <> "" Then
            If IsNumeric(Ektron.Cms.CommonApi.GetEcmCookie()(_ContentTypeUrlParam)) Then
                _ContentTypeSelected = CLng(Ektron.Cms.CommonApi.GetEcmCookie()(_ContentTypeUrlParam))
            End If
        End If

        Dim contentSubTypeSelected As String = Request.QueryString("SubType")
        If contentSubTypeSelected = String.Empty Then
            contentSubTypeSelected = Ektron.Cms.CommonApi.GetEcmCookie()("SubType")
        Else
            _ContentApi.SetCookieValue("SubType", contentSubTypeSelected)
        End If
        _ContentSubTypeSelected = IIf(contentSubTypeSelected = String.Empty, CMSContentSubtype.AllTypes, CType(contentSubTypeSelected, Ektron.Cms.Common.EkEnumeration.CMSContentSubtype))

    End Sub

    Private Function GetQueryStringValue(ByVal key As String) As String
        Dim returnValue As String = String.Empty
        If (Not (Request.QueryString(key) Is Nothing)) Then
            returnValue = Request.QueryString(key)
        End If
        Return returnValue
    End Function

    Private Function MakeBold(ByVal str As String, ByVal ContentType As Integer, Optional ByVal SubType As Ektron.Cms.Common.EkEnumeration.CMSContentSubtype = CMSContentSubtype.AllTypes) As String
        If (_ContentTypeSelected = ContentType) Then
            If (SubType = CMSContentSubtype.AllTypes Or SubType = _ContentSubTypeSelected) Then
                Return "<b>" & str & "</b>"
            End If
        End If
        Return str
    End Function

    Private Sub SetViewImage(Optional ByVal override As String = "")
        Dim scheckval As String = ""
        If override <> "" Then
            scheckval = override
        Else
            scheckval = _ContentTypeSelected
        End If
        Select Case scheckval
            Case 101
                _ViewImagePath = "images/UI/Icons/FileTypes/word.png"
            Case 105
                _ViewImagePath = "images/UI/Icons/FileTypes/text.png"
            Case 102
                _ViewImagePath = "images/UI/Icons/contentDMSDocument.png"
            Case 104
                _ViewImagePath = "images/UI/Icons/film.png"
            Case 96
                _ViewImagePath = "images/UI/Icons/folderView.png"
            Case CMSContentType_Content
                _ViewImagePath = "images/UI/Icons/contentHtml.png"
            Case CMSContentType_Forms
                _ViewImagePath = "images/UI/Icons/contentForm.png"
            Case Else

        End Select
    End Sub
    Private Sub SetPagingUI()
        'paging ui
        divPaging.Visible = True

        litPage.Text = _MessageHelper.GetMessage("lbl pagecontrol page")
        CurrentPage.Text = IIf(_PagingCurrentPageNumber = 0, "1", _PagingCurrentPageNumber.ToString())
        litOf.Text = _MessageHelper.GetMessage("lbl pagecontrol of")
        TotalPages.Text = _PagingTotalPagesNumber.ToString()

        ibFirstPage.ImageUrl = Me.ApplicationPath & "/images/ui/icons/arrowheadFirst.png"
        ibFirstPage.AlternateText = _MessageHelper.GetMessage("lbl first page")
        ibFirstPage.ToolTip = ibFirstPage.AlternateText

        If (_PagingCurrentPageNumber = 1) Then
            ibFirstPage.Enabled = False
        Else
            ibFirstPage.Enabled = True
        End If

        ibPreviousPage.ImageUrl = Me.ApplicationPath & "/images/ui/icons/arrowheadLeft.png"
        ibPreviousPage.AlternateText = _MessageHelper.GetMessage("lbl previous page")
        ibPreviousPage.ToolTip = ibPreviousPage.AlternateText

        If (_PagingCurrentPageNumber = 1) Then
            ibPreviousPage.Enabled = False
        Else
            ibPreviousPage.Enabled = True
        End If

        ibNextPage.ImageUrl = Me.ApplicationPath & "/images/ui/icons/arrowheadRight.png"
        ibNextPage.AlternateText = _MessageHelper.GetMessage("lbl next page")
        ibNextPage.ToolTip = ibNextPage.AlternateText

        If (_PagingCurrentPageNumber = _PagingTotalPagesNumber) Then
            ibNextPage.Enabled = False
        Else
            ibNextPage.Enabled = True
        End If

        ibLastPage.ImageUrl = Me.ApplicationPath & "/images/ui/icons/arrowheadLast.png"
        ibLastPage.AlternateText = _MessageHelper.GetMessage("lbl last page")
        ibLastPage.ToolTip = ibLastPage.AlternateText

        If (_PagingCurrentPageNumber = _PagingTotalPagesNumber) Then
            ibLastPage.Enabled = False
        Else
            ibLastPage.Enabled = True
        End If

        ibPageGo.ImageUrl = Me.ApplicationPath & "/images/ui/icons/forward.png"
        ibPageGo.AlternateText = _MessageHelper.GetMessage("lbl pagecontrol go to page")
        ibPageGo.ToolTip = ibPageGo.AlternateText
        ibPageGo.OnClientClick = "GoToPage(document.getElementById('" & Me.CurrentPage.ClientID & "').value, " & _PagingTotalPagesNumber.ToString() & ");return false;"

    End Sub

    Public Function ViewTaxonomyContentByCategory() As Boolean
        Dim blah As String = ""
    End Function

    Public Function ViewCollectionContentByCategory() As Boolean
        Dim api As New Ektron.Cms.UI.CommonUI.ApplicationAPI
        Dim ErrorString As String = ""
        Dim id As Int64 = Convert.ToInt64(Request.QueryString("id"))
    End Function

#End Region

#Region "JS, CSS"

    Private Sub RegisterJS()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronThickBoxJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, _ContentApi.AppPath & "/java/ektron.workarea.contextmenus.js", "EktronWorkareaContextMenusJS")
        Ektron.Cms.API.JS.RegisterJS(Me, _ContentApi.AppPath & "/java/ektron.workarea.contextmenus.trees.js", "EktronWorkareaContextMenusTreesJS")
        Ektron.Cms.API.JS.RegisterJS(Me, _ContentApi.AppPath & "/java/ektron.workarea.contextmenus.cutcopy.js", "EktronWorkareaContextMenusCutCopyJS")
    End Sub

    Private Sub RegisterCSS()
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronThickBoxCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)
    End Sub

#End Region

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
	Try
        If (Me._FolderData IsNot Nothing AndAlso Me._FolderData.FolderType = FolderType.Calendar) Then
            If (_PageAction = "viewarchivecontentbycategory") Then
                _EkContentCol = _EkContent.GetAllViewArchiveContentInfov5_0(_PageData, _PagingCurrentPageNumber, _PagingPageSize, _PagingTotalPagesNumber)
                _NextActionType = "ViewContentByCategory"
            ElseIf (_PageAction = "viewcontentbycategory") Then
                _EkContentCol = _EkContent.GetAllViewableChildContentInfoV5_0(_PageData, _PagingCurrentPageNumber, _PagingPageSize, _PagingTotalPagesNumber)
                _NextActionType = "ViewArchiveContentByCategory"
            End If

            'paging goes here

            Populate_ViewCalendar(_EkContentCol)
            Util_SetJs()
            If _PagingTotalPagesNumber > 1 Then
                Me.SetPagingUI()
            Else
                divPaging.Visible = False
            End If
        End If
	Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & _ContentLanguage, False)
        End Try
    End Sub
    
End Class