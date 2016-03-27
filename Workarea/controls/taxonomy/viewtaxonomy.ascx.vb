Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports System.DateTime
Imports System.Collections.Generic
Imports System.IO
Imports Ektron.Cms.Framework.Core.CustomProperty
Imports Ektron.Cms.Content



Partial Class viewtaxonomy
    Inherits System.Web.UI.UserControl

    Protected _Common As New CommonApi
    Protected _StyleHelper As New StyleHelper
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected _MessageHelper As EkMessageHelper
    Protected _PageAction As String = ""
    Protected _Content As Content.EkContent
    Protected TaxonomyId As Long = 0
    Protected TaxonomyLanguage As Integer = -1
    Protected language_data As LanguageData
    Protected taxonomy_request As TaxonomyRequest
    Protected taxonomy_data As TaxonomyData
    Protected TaxonomyParentId As Long = 0
    Protected _ViewItem As String = "item"
    Protected AddDeleteIcon As Boolean = False
    Protected TaxonomyItemCount As Long = 0
    Protected TaxonomyCategoryCount As Long = 0
    Protected _TaxonomyName As String = ""
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected m_intMetadataCurrentPage As Integer = 1
    Protected m_intMetadataTotalPages As Integer = 1
    Protected m_strDelConfirm As String = ""
    Protected m_strDelItemsConfirm As String = ""
    Protected m_strSelDelWarning As String = ""
    Protected m_strCurrentBreadcrumb As String = ""
    Protected objLocalizationApi As New LocalizationAPI()
    Protected m_refContentApi As New ContentAPI

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _MessageHelper = _Common.EkMsgRef
        AppImgPath = _Common.AppImgPath
        AppPath = _Common.AppPath
        _PageAction = Request.QueryString("action")
        Utilities.SetLanguage(_Common)
        RegisterResources()
        TaxonomyLanguage = _Common.ContentLanguage
        TaxonomyId = Convert.ToInt64(Request.QueryString("taxonomyid"))
        If (Request.QueryString("view") IsNot Nothing) Then
            _ViewItem = Request.QueryString("view")
        End If
        taxonomy_request = New TaxonomyRequest
        taxonomy_request.TaxonomyId = TaxonomyId
        taxonomy_request.TaxonomyLanguage = TaxonomyLanguage
        _Content = _Common.EkContentRef
        taxonomy_request.PageSize = 99999999    ' pagesize of 0 used to mean "all"
        Dim taxcats() As TaxonomyBaseData
        taxcats = _Content.ReadAllSubCategories(taxonomy_request)
        If (taxcats IsNot Nothing) Then
            TaxonomyCategoryCount = taxcats.Length
        End If
        If (Page.IsPostBack AndAlso Request.Form(isPostData.UniqueID) <> "") Then
            If (Request.Form("submittedaction") = "delete") Then
                _Content.DeleteTaxonomy(taxonomy_request)
                'Response.Write("<script type=""text/javascript"">parent.CloseChildPage();</script>")
                Response.Redirect("taxonomy.aspx?rf=1", False)
            ElseIf (Request.Form("submittedaction") = "deleteitem") Then
                If (_ViewItem <> "folder") Then
                    taxonomy_request.TaxonomyIdList = Request.Form("selected_items")
                    If (_ViewItem.ToLower = "cgroup") Then
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Group
                    ElseIf (_ViewItem.ToLower = "user") Then
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.User
                    Else
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Content
                    End If
                    _Content.RemoveTaxonomyItem(taxonomy_request)
                Else
                    Dim tax_folder As New TaxonomySyncRequest
                    tax_folder.TaxonomyId = TaxonomyId
                    tax_folder.TaxonomyLanguage = TaxonomyLanguage
                    tax_folder.SyncIdList = Request.Form("selected_items")
                    _Content.RemoveTaxonomyFolder(tax_folder)
                End If
                If (Request.Params("ccp") Is Nothing) Then
                    Response.Redirect("taxonomy.aspx?" & Request.ServerVariables("query_string") & "&ccp=true", True)
                Else
                    Response.Redirect("taxonomy.aspx?" & Request.ServerVariables("query_string"), True)
                End If
            End If



        ElseIf (IsPostBack = False) Then
            DisplayPage()
        End If

        AssignTextStrings()

        isPostData.Value = "true"
        hdnSourceId.Value = TaxonomyId
    End Sub

    Private Sub DisplayPage()
        Select Case _ViewItem.ToLower()
            Case "user"
                Dim uReq As New DirectoryUserRequest
                Dim uDirectory As New DirectoryAdvancedUserData
                uReq.GetItems = True
                uReq.DirectoryId = TaxonomyId
                uReq.DirectoryLanguage = TaxonomyLanguage
                uReq.PageSize = _Common.RequestInformationRef.PagingSize
                uReq.CurrentPage = m_intCurrentPage
                uDirectory = Me._Content.LoadDirectory(uReq)
                If (uDirectory IsNot Nothing) Then
                    TaxonomyParentId = uDirectory.DirectoryParentId
                    lbltaxonomyid.Text = uDirectory.DirectoryId
                    taxonomytitle.Text = uDirectory.DirectoryName
                    _TaxonomyName = uDirectory.DirectoryName
                    taxonomydescription.Text = uDirectory.DirectoryDescription
                    taxonomy_image_thumb.ImageUrl = _Common.AppImgPath & "spacer.gif"
                    m_strCurrentBreadcrumb = uDirectory.DirectoryPath.Remove(0, 1).Replace("\", " > ")
                    If (m_strCurrentBreadcrumb = "") Then
                        m_strCurrentBreadcrumb = "Root"
                    End If
                    If (uDirectory.TemplateName = "") Then
                        lblTemplate.Text = "[None]"
                    Else
                        lblTemplate.Text = uDirectory.TemplateName
                    End If
                    If (uDirectory.InheritTemplate) Then
                        lblTemplateInherit.Text = "Yes"
                    Else
                        lblTemplateInherit.Text = "No"
                    End If

                    m_intTotalPages = uReq.TotalPages
                End If
                PopulateUserGridData(uDirectory)
                TaxonomyToolBar()
            Case "cgroup"
                Dim dagdRet As New DirectoryAdvancedGroupData
                Dim cReq As New DirectoryGroupRequest

                cReq.CurrentPage = m_intCurrentPage
                cReq.PageSize = _Common.RequestInformationRef.PagingSize
                cReq.DirectoryId = TaxonomyId
                cReq.DirectoryLanguage = TaxonomyLanguage
                cReq.GetItems = True
                cReq.SortDirection = ""

                dagdRet = Me._Common.CommunityGroupRef.LoadDirectory(cReq)
                If (dagdRet IsNot Nothing) Then
                    TaxonomyParentId = dagdRet.DirectoryParentId
                    lbltaxonomyid.Text = dagdRet.DirectoryId
                    taxonomytitle.Text = dagdRet.DirectoryName
                    _TaxonomyName = dagdRet.DirectoryName
                    taxonomydescription.Text = dagdRet.DirectoryDescription
                    taxonomy_image_thumb.ImageUrl = _Common.AppImgPath & "spacer.gif"
                    m_strCurrentBreadcrumb = dagdRet.DirectoryPath.Remove(0, 1).Replace("\", " > ")
                    If (m_strCurrentBreadcrumb = "") Then
                        m_strCurrentBreadcrumb = "Root"
                    End If
                    If (dagdRet.TemplateName = "") Then
                        lblTemplate.Text = "[None]"
                    Else
                        lblTemplate.Text = dagdRet.TemplateName
                    End If
                    If (dagdRet.InheritTemplate) Then
                        lblTemplateInherit.Text = "Yes"
                    Else
                        lblTemplateInherit.Text = "No"
                    End If

                    m_intTotalPages = cReq.TotalPages
                End If
                PopulateCommunityGroupGridData(dagdRet)
                TaxonomyToolBar()
            Case Else ' Content
                taxonomy_request.IncludeItems = True
                taxonomy_request.PageSize = _Common.RequestInformationRef.PagingSize
                taxonomy_request.CurrentPage = m_intCurrentPage
                taxonomy_data = _Content.ReadTaxonomy(taxonomy_request)
                If (taxonomy_data IsNot Nothing) Then
                    TaxonomyParentId = taxonomy_data.TaxonomyParentId
                    lbltaxonomyid.Text = taxonomy_data.TaxonomyId
                    taxonomytitle.Text = taxonomy_data.TaxonomyName
                    _TaxonomyName = taxonomy_data.TaxonomyName
                    If taxonomy_data.TaxonomyDescription = "" Then
                        taxonomydescription.Text = "[None]"
                    Else
                        taxonomydescription.Text = taxonomy_data.TaxonomyDescription
                    End If
                    If taxonomy_data.TaxonomyImage = "" Then
                        taxonomy_image.Text = "[None]"
                    Else
                        taxonomy_image.Text = taxonomy_data.TaxonomyImage
                    End If
                    taxonomy_image_thumb.ImageUrl = taxonomy_data.TaxonomyImage
                    If taxonomy_data.CategoryUrl = "" Then
                        catLink.Text = "[None]"
                    Else
                        catLink.Text = taxonomy_data.CategoryUrl
                    End If

                    If taxonomy_data.Visible = True Then
                        ltrStatus.Text = "Enabled"
                    Else
                        ltrStatus.Text = "Disabled"
                    End If
                    If taxonomy_data.TaxonomyImage.Trim() <> "" Then
                        taxonomy_image_thumb.ImageUrl = IIf(taxonomy_data.TaxonomyImage.IndexOf("/") = 0, taxonomy_data.TaxonomyImage, _Common.SitePath & taxonomy_data.TaxonomyImage)
                    Else
                        taxonomy_image_thumb.ImageUrl = _Common.AppImgPath & "spacer.gif"
                    End If
                    m_strCurrentBreadcrumb = taxonomy_data.TaxonomyPath.Remove(0, 1).Replace("\", " > ")
                    If (m_strCurrentBreadcrumb = "") Then
                        m_strCurrentBreadcrumb = "Root"
                    End If
                    If (taxonomy_data.TemplateName = "") Then
                        lblTemplate.Text = "[None]"
                    Else
                        lblTemplate.Text = taxonomy_data.TemplateName
                    End If
                    If (taxonomy_data.TemplateInherited) Then
                        lblTemplateInherit.Text = "Yes"
                    Else
                        lblTemplateInherit.Text = "No"
                    End If

                    m_intTotalPages = taxonomy_request.TotalPages
                End If
                PopulateContentGridData()
                TaxonomyToolBar()
        End Select

        DisplayTaxonomyMetadata()

        If (TaxonomyParentId = 0) Then
            tr_config.Visible = True
            Dim config_list As List(Of Int32) = _Content.GetAllConfigIdListByTaxonomy(TaxonomyId, TaxonomyLanguage)
            configlist.Text = ""
            For i As Integer = 0 To config_list.Count - 1
                If (configlist.Text = "") Then
                    configlist.Text = ConfigName(config_list.Item(i))
                Else
                    configlist.Text = configlist.Text & ";" & ConfigName(config_list.Item(i))
                End If
            Next
            If (configlist.Text = "") Then
                configlist.Text = "None"
            End If
        Else
            tr_config.Visible = False
        End If

        ' display counts
        ltrCatCount.Text = TaxonomyCategoryCount
        ltrItemCount.Text = taxonomy_request.RecordsAffected
    End Sub
    Private Function ConfigName(ByVal id As Integer) As String
        Select Case id
            Case 0
                Return "Content"
            Case 1
                Return "User"
            Case 2
                Return "Group"
            Case Else
                Return "Content"
        End Select
    End Function
    Private Sub PopulateCommunityGroupGridData(ByVal cgDirectory As DirectoryAdvancedGroupData)
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("CHECK", "<input type=""checkbox"" name=""checkall"" onclick=""checkAll('selected_items',false);"">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(3), Unit.Percentage(2), False, False))
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ID", _MessageHelper.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("COMMUNITYGROUP", _MessageHelper.GetMessage("lbl community group"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), False, True))
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("INFORMATION", "&#160;", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), False, False))

        TaxonomyItemList.Columns(2).ItemStyle.VerticalAlign = VerticalAlign.Top
        TaxonomyItemList.Columns(3).ItemStyle.VerticalAlign = VerticalAlign.Top

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("CHECK", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("COMMUNITYGROUP", GetType(String)))
        dt.Columns.Add(New DataColumn("INFORMATION", GetType(String)))
        PageSettings()
        If (cgDirectory IsNot Nothing AndAlso cgDirectory.DirectoryItems IsNot Nothing AndAlso cgDirectory.DirectoryItems.Length > 0) Then
            AddDeleteIcon = True
            For Each item As CommunityGroupData In cgDirectory.DirectoryItems
                TaxonomyItemCount = TaxonomyItemCount + 1
                dr = dt.NewRow
                dr("CHECK") = "<input type=""checkbox"" name=""selected_items"" id=""selected_items"" value=""" & item.GroupId & """ onclick=""checkAll('selected_items',true);"">"

                Dim groupurl As String
                groupurl = "Community/groups.aspx?action=viewgroup&id=" & item.GroupId
                dr("COMMUNITYGROUP") = "<img src=""" & IIf(item.GroupImage <> "", item.GroupImage, Me._Common.AppImgPath & "member_default.gif") & """ align=""left"" width=""55"" height=""55"" />"
                dr("COMMUNITYGROUP") &= "<a href=""" & groupurl & """>"
                dr("COMMUNITYGROUP") &= item.GroupName
                dr("COMMUNITYGROUP") &= "</a>"
                dr("COMMUNITYGROUP") &= " (" & IIf(item.GroupEnroll, Me._MessageHelper.GetMessage("lbl enrollment open"), Me._MessageHelper.GetMessage("lbl enrollment restricted")) & ")"
                dr("COMMUNITYGROUP") &= "<br/>"
                dr("COMMUNITYGROUP") &= item.GroupShortDescription

                dr("ID") = item.GroupId

                dr("INFORMATION") = Me._MessageHelper.GetMessage("content dc label") & " " & item.GroupCreatedDate.ToShortDateString
                dr("INFORMATION") &= "<br/>"
                dr("INFORMATION") &= Me._MessageHelper.GetMessage("lbl members") & ": " & item.TotalMember.ToString()
                dt.Rows.Add(dr)
            Next
        Else
            dr = dt.NewRow
            dt.Rows.Add(dr)
            TaxonomyItemList.GridLines = GridLines.None
        End If
        Dim dv As New DataView(dt)
        TaxonomyItemList.DataSource = dv
        TaxonomyItemList.DataBind()
    End Sub
    Private Sub PopulateUserGridData(ByVal uDirectory As DirectoryAdvancedUserData)
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("CHECK", "<input type=""checkbox"" name=""checkall"" onclick=""checkAll('selected_items',false);"">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(3), Unit.Percentage(2), False, False))
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ID", _MessageHelper.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("USERNAME", _MessageHelper.GetMessage("generic username"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), False, False))
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("DISPLAYNAME", _MessageHelper.GetMessage("display name label"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), False, False))
        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("CHECK", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("USERNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("DISPLAYNAME", GetType(String)))
        PageSettings()
        If (uDirectory IsNot Nothing AndAlso uDirectory.DirectoryItems IsNot Nothing AndAlso uDirectory.DirectoryItems.Length > 0) Then
            AddDeleteIcon = True
            For Each item As DirectoryUserData In uDirectory.DirectoryItems
                TaxonomyItemCount = TaxonomyItemCount + 1
                dr = dt.NewRow
                dr("CHECK") = "<input type=""checkbox"" name=""selected_items"" id=""selected_items"" value=""" & item.Id & """ onclick=""checkAll('selected_items',true);"">"
                ' TODO: do we need to put in valid groupid and grouptype fields??
                Dim userurl As String = "users.aspx?action=View&LangType=" & TaxonomyLanguage & _
                    "&groupid=" & 0 & _
                    "&grouptype=" & 0 & "&id=" & item.Id & "&FromUsers=&OrderBy=user_name&callbackpage=taxonomy.aspx?" & Request.ServerVariables("query_string")
                dr("USERNAME") = "<a href =""" & userurl & """>"
                dr("USERNAME") += item.Username  '"<a href=""taxonomy.aspx?action=viewtree&taxonomyid=" & item.TaxonomyItemId & "&LangType=" & item.TaxonomyItemLanguage & """>" & item.TaxonomyItemTitle & "</a>"
                dr("USERNAME") += "</a>"

                dr("ID") = item.Id
                dr("DISPLAYNAME") = item.DisplayName
                dt.Rows.Add(dr)
            Next
        Else
            dr = dt.NewRow
            dt.Rows.Add(dr)
            TaxonomyItemList.GridLines = GridLines.None
        End If
        Dim dv As New DataView(dt)
        TaxonomyItemList.DataSource = dv
        TaxonomyItemList.DataBind()
    End Sub

    Private Sub PopulateContentGridData()
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("CHECK", "<input type=""checkbox"" name=""checkall"" onclick=""checkAll('selected_items',false);"">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(2), Unit.Percentage(2), False, False))
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("TITLE", _MessageHelper.GetMessage("generic title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(50), False, False))
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ID", _MessageHelper.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("LANGUAGE", _MessageHelper.GetMessage("generic language"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("URL", _MessageHelper.GetMessage("generic url link"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(30), False, False))
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ARCHIVED", _MessageHelper.GetMessage("lbl archived"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))

        Dim dt As New DataTable
        Dim dr As DataRow
        Dim libraryInfo As LibraryData
        Dim contData As New ContentData
        dt.Columns.Add(New DataColumn("CHECK", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        dt.Columns.Add(New DataColumn("URL", GetType(String)))
        dt.Columns.Add(New DataColumn("ARCHIVED", GetType(String)))
        If (_ViewItem <> "folder") Then
            PageSettings()
            If (taxonomy_data IsNot Nothing AndAlso taxonomy_data.TaxonomyItems IsNot Nothing AndAlso taxonomy_data.TaxonomyItems.Length > 0) Then
                AddDeleteIcon = True
                For Each item As TaxonomyItemData In taxonomy_data.TaxonomyItems
                    TaxonomyItemCount = TaxonomyItemCount + 1
                    dr = dt.NewRow
                    dr("CHECK") = "<input type=""checkbox"" name=""selected_items"" id=""selected_items"" value=""" & item.TaxonomyItemId & """ onclick=""checkAll('selected_items',true);"">"
                    Dim contenturl As String = ""
                    Select Case item.ContentType()
                        Case 1
                            If (item.ContentSubType = EkEnumeration.CMSContentSubtype.WebEvent) Then
                                Dim fid As Long = _Common.EkContentRef.GetFolderIDForContent(item.TaxonomyItemId)
                                contenturl = "content.aspx?action=ViewContentByCategory&LangType=" & item.TaxonomyItemLanguage & "&id=" & fid & "&callerpage=taxonomy.aspx&origurl=" & Server.UrlEncode("action=view&view=item&taxonomyid=" & TaxonomyId & "&treeViewId=-1&LangType=" & TaxonomyLanguage)
                            Else
                                contenturl = "content.aspx?action=View&LangType=" & item.TaxonomyItemLanguage & "&id=" & item.TaxonomyItemId & "&callerpage=taxonomy.aspx&origurl=" & Server.UrlEncode("action=view&view=item&taxonomyid=" & TaxonomyId & "&treeViewId=-1&LangType=" & TaxonomyLanguage)
                            End If
                        Case 7 ' Library Item
                            libraryInfo = m_refContentApi.GetLibraryItemByContentID(item.TaxonomyItemId)
                            contenturl = "library.aspx?LangType=" & libraryInfo.LanguageId & "&action=ViewLibraryItem&id=" & libraryInfo.Id & "&parent_id=" & libraryInfo.ParentId
                        Case Else
                            contenturl = "content.aspx?action=View&LangType=" & item.TaxonomyItemLanguage & "&id=" & item.TaxonomyItemId & "&callerpage=taxonomy.aspx&origurl=" & Server.UrlEncode("action=view&view=item&taxonomyid=" & TaxonomyId & "&treeViewId=-1&LangType=" & TaxonomyLanguage)
                    End Select
                    dr("TITLE") = m_refContentApi.GetDmsContextMenuHTML(item.TaxonomyItemId, item.TaxonomyItemLanguage, item.ContentType, item.ContentSubType, item.TaxonomyItemTitle, _MessageHelper.GetMessage("generic Title") + " " + item.TaxonomyItemTitle, contenturl, item.TaxonomyItemAssetInfo.FileName, item.TaxonomyItemAssetInfo.ImageUrl)
                    dr("ID") = item.TaxonomyItemId
                    dr("LANGUAGE") = item.TaxonomyItemLanguage
                    If item.ContentType = 102 Then
                        libraryInfo = m_refContentApi.GetLibraryItemByContentID(item.TaxonomyItemId)
                        dr("URL") = libraryInfo.FileName.Replace("//", "/")
                    Else
                        Dim api As New Ektron.Cms.API.Content.Content()
                        contData = api.GetContent(item.TaxonomyItemId)
                        'contData = m_refContentApi.GetContentById(item.TaxonomyItemId)
                        dr("URL") = contData.Quicklink
                    End If
                    If item.ContentType = EkEnumeration.CMSContentType.Archive_Content OrElse item.ContentType = EkEnumeration.CMSContentType.Archive_Forms OrElse item.ContentType = EkEnumeration.CMSContentType.Archive_Media OrElse (item.ContentType >= EkConstants.Archive_ManagedAsset_Min AndAlso item.ContentType < EkConstants.Archive_ManagedAsset_Max AndAlso item.ContentType <> 3333 AndAlso item.ContentType <> 1111) Then
                        dr("ARCHIVED") = "<span class=""Archived""></span>"
                    End If
                    dt.Rows.Add(dr)
                Next
            Else
                dr = dt.NewRow
                dt.Rows.Add(dr)
                TaxonomyItemList.GridLines = GridLines.None
            End If
        Else
            VisiblePageControls(False)
            Dim taxonomy_sync_folder As TaxonomyFolderSyncData() = Nothing
            Dim tax_sync_folder_req As New TaxonomyBaseRequest
            tax_sync_folder_req.TaxonomyId = TaxonomyId
            tax_sync_folder_req.TaxonomyLanguage = TaxonomyLanguage
            taxonomy_sync_folder = _Content.GetAllAssignedCategoryFolder(tax_sync_folder_req)
            If (taxonomy_sync_folder IsNot Nothing AndAlso taxonomy_sync_folder.Length > 0) Then
                AddDeleteIcon = True
                For i As Integer = 0 To taxonomy_sync_folder.Length - 1
                    TaxonomyItemCount = TaxonomyItemCount + 1
                    dr = dt.NewRow
                    dr("CHECK") = "<input type=""checkbox"" name=""selected_items"" id=""selected_items"" value=""" & taxonomy_sync_folder(i).FolderId & """ onclick=""checkAll('selected_items',true);"">"

                    Dim contenturl As String
                    contenturl = "content.aspx?action=ViewContentByCategory&id=" & taxonomy_sync_folder(i).FolderId & "&treeViewId=0"

                    dr("TITLE") = "<a href=""" & contenturl & """>"
                    dr("TITLE") += "<img src="""
                    Select Case taxonomy_sync_folder(i).FolderType
                        Case EkEnumeration.FolderType.Catalog
                            dr("TITLE") += m_refContentApi.AppPath & "images/ui/icons/folderGreen.png"
                        Case EkEnumeration.FolderType.Community
                            dr("TITLE") += m_refContentApi.AppPath & "images/ui/icons/folderCommunity.png"
                        Case EkEnumeration.FolderType.Blog
                            dr("TITLE") += m_refContentApi.AppPath & "images/ui/icons/folderBlog.png"
                        Case EkEnumeration.FolderType.DiscussionBoard
                            dr("TITLE") += m_refContentApi.AppPath & "images/ui/icons/folderBoard.png"
                        Case EkEnumeration.FolderType.DiscussionForum
                            dr("TITLE") += m_refContentApi.AppPath & "images/ui/icons/folderBoard.png"
                        Case Else
                            dr("TITLE") += m_refContentApi.AppPath & "images/ui/icons/folder.png"
                    End Select
                    dr("TITLE") += """></img>"
                    dr("TITLE") += "</a><a href=""" & contenturl & """>"
                    dr("TITLE") += taxonomy_sync_folder(i).FolderTitle '& GetRecursiveTitle(item.FolderRecursive)
                    dr("TITLE") += "</a>"

                    dr("ID") = taxonomy_sync_folder(i).FolderId
                    dr("LANGUAGE") = taxonomy_sync_folder(i).TaxonomyLanguage
                    dt.Rows.Add(dr)
                Next
            Else
                dr = dt.NewRow
                dt.Rows.Add(dr)
                TaxonomyItemList.GridLines = GridLines.None
            End If
        End If
        Dim dv As New DataView(dt)
        TaxonomyItemList.DataSource = dv
        TaxonomyItemList.DataBind()
    End Sub
    Private Function GetRecursiveTitle(ByVal value As Boolean) As String
        Dim result As String = ""
        If (value) Then
            result = "<span class=""important""> (Recursive)</span>"
        End If
        Return result
    End Function
    Private Sub TaxonomyToolBar()
        Dim IFrameVariable As String = ""
        Dim strDeleteMsg As String = ""
        If (Request.QueryString("iframe") = "true") Then
            IFrameVariable = "&iframe=true"
        End If
        If (TaxonomyParentId > 0) Then
            strDeleteMsg = _MessageHelper.GetMessage("alt delete button text (category)")
            m_strDelConfirm = _MessageHelper.GetMessage("delete category confirm")
            m_strDelItemsConfirm = _MessageHelper.GetMessage("delete category items confirm")
            m_strSelDelWarning = _MessageHelper.GetMessage("select category item missing warning")
        Else
            strDeleteMsg = _MessageHelper.GetMessage("alt delete button text (taxonomy)")
            m_strDelConfirm = _MessageHelper.GetMessage("delete taxonomy confirm")
            m_strDelItemsConfirm = _MessageHelper.GetMessage("delete taxonomy items confirm")
            m_strSelDelWarning = _MessageHelper.GetMessage("select taxonomy item missing warning")
        End If
        divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view taxonomy page title") & " """ & _TaxonomyName & """" & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) & "' />")
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)

        result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath & "images/ui/Icons/add.png", "taxonomy.aspx?action=add&parentid=" & TaxonomyId & "&LangType=" & TaxonomyLanguage & IFrameVariable, _
                                                         _MessageHelper.GetMessage("add category page title"), _MessageHelper.GetMessage("add category page title"), ""))
        If (AddDeleteIcon) Then
            removeItemsWrapper.Visible = True
        End If
        If ((TaxonomyCategoryCount > 1) Or (TaxonomyItemCount > 1)) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath & "images/ui/Icons/arrowUpDown.png", "taxonomy.aspx?action=reorder&taxonomyid=" & TaxonomyId & "&parentid=" & TaxonomyParentId & "&reorder=category" & "&LangType=" & TaxonomyLanguage & IFrameVariable, _MessageHelper.GetMessage("reorder taxonomy page title"), _MessageHelper.GetMessage("reorder taxonomy page title"), ""))
        End If
        result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath & "images/ui/Icons/contentStackAdd.png", "taxonomy.aspx?action=additem&taxonomyid=" & TaxonomyId & "&parentid=" & TaxonomyParentId & "&LangType=" & TaxonomyLanguage & IFrameVariable, _MessageHelper.GetMessage("assign items to taxonomy page title"), _MessageHelper.GetMessage("assign items to taxonomy page title"), ""))
        result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath & "images/ui/Icons/folderAdd.png", "taxonomy.aspx?action=addfolder&taxonomyid=" & TaxonomyId & "&parentid=" & TaxonomyParentId & "&LangType=" & TaxonomyLanguage & IFrameVariable, _MessageHelper.GetMessage("assign folders to taxonomy page title"), _MessageHelper.GetMessage("assign folders to taxonomy page title"), ""))

        result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath & "images/ui/Icons/contentEdit.png", "taxonomy.aspx?action=edit&taxonomyid=" & TaxonomyId & "&parentid=" & TaxonomyParentId & "&LangType=" & TaxonomyLanguage & IFrameVariable, _MessageHelper.GetMessage("alt edit button text (taxonomy)"), _MessageHelper.GetMessage("btn edit"), ""))
        If (TaxonomyParentId = 0) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath & "images/ui/Icons/translation.png", "#", _MessageHelper.GetMessage("alt export taxonomy"), _MessageHelper.GetMessage("btn export taxonomy"), "onclick=""window.open('taxonomy_imp_exp.aspx?action=export&taxonomyid=" & TaxonomyId & "&LangType=" & TaxonomyLanguage & "','exptaxonomy','status=0,toolbar=0,location=0,menubar=0,directories=0,resizable=0,scrollbars=1,height=100px,width=200px');void(0);"""))
        End If
        result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath & "images/ui/Icons/contentCopy.png", "#", _MessageHelper.GetMessage("generic move copy taxonomy"), _MessageHelper.GetMessage("generic move copy taxonomy"), "onclick=""$ektron('#TaxonomySelect').modalShow();"""))
        result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath & "images/ui/Icons/delete.png", "#", _MessageHelper.GetMessage("generic delete title"), _MessageHelper.GetMessage("alt delete button text (taxonomy)"), "onclick=""return DeleteItem();"""))

        If (Request.QueryString("iframe") = "true") Then
            Dim parentaction As String = "javascript:parent.CancelIframe();"
            If (Request.Params("ccp") IsNot Nothing) Then
                parentaction = "javascript:parent.CloseChildPage();"
            End If
            result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath & "images/ui/Icons/cancel.png", "#", _MessageHelper.GetMessage("generic Cancel"), _MessageHelper.GetMessage("generic Cancel"), "onClick=""" & parentaction & """"))
        Else
            result.Append("<td>&nbsp;|&nbsp;</td>")
            result.Append("<td nowrap=""true"">")
            Dim addDD As String
            addDD = GetLanguageForTaxonomy(TaxonomyId, "", False, False, "javascript:TranslateTaxonomy(" & TaxonomyId & ", " & TaxonomyParentId & ", this.value);")
            If addDD <> "" Then
                addDD = "&nbsp;" & _MessageHelper.GetMessage("add title") & ":&nbsp;" & addDD
            End If
            If (CStr(_Common.EnableMultilingual = "1")) Then
                result.Append("View In:&nbsp;" & GetLanguageForTaxonomy(TaxonomyId, "", True, False, "javascript:LoadLanguage(this.value);") & "&nbsp;" & addDD & "<br>")
            End If
            result.Append("</td>")
        End If

        result.Append("<td>&nbsp;</td>")
        result.Append(ViewTypeDropDown())

        result.Append("<td>" & _StyleHelper.GetHelpButton("ViewTaxonomyOrCategory") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Private Function ViewTypeDropDown() As String
        Dim result As New StringBuilder
        result.Append("<td class=""label"">")
        result.Append(_MessageHelper.GetMessage("lbl View") & ":")
        result.Append("</td>")
        result.Append("<td>")
        result.Append("<select id=""selviewtype"" name=""selviewtype"" onchange=""LoadViewType(this.value);"">")
        result.Append("<option value=""folder"" " & FindSelected("folder") & ">").Append(Me._MessageHelper.GetMessage("lbl folders")).Append("</option>")
        result.Append("<option value=""item""  " & FindSelected("item") & ">").Append(Me._MessageHelper.GetMessage("content button text")).Append("</option>")
        result.Append("<option value=""user""  " & FindSelected("user") & ">").Append(Me._MessageHelper.GetMessage("generic users")).Append("</option>")
        result.Append("<option value=""cgroup""  " & FindSelected("cgroup") & ">").Append(Me._MessageHelper.GetMessage("lbl community groups")).Append("</option>")
        result.Append("</select>")
        result.Append("</td>")
        Return result.ToString()
    End Function

    Private Function FindSelected(ByVal chk As String) As String
        Dim val As String = ""
        If (_ViewItem = chk) Then
            val = " selected "
        End If
        Return val
    End Function

    Private Function GetLanguageForTaxonomy(ByVal TaxonomyId As Long, ByVal BGColor As String, ByVal ShowTranslated As Boolean, ByVal ShowAllOpt As Boolean, ByVal onChangeEv As String) As String
        Dim result As String = ""
        Dim frmName As String = ""
        Dim result_language As IList(Of LanguageData) = Nothing
        Dim taxonomy_language_request As New TaxonomyLanguageRequest
        taxonomy_language_request.TaxonomyId = TaxonomyId

        If (ShowTranslated) Then
            taxonomy_language_request.IsTranslated = True
            result_language = _Content.LoadLanguageForTaxonomy(taxonomy_language_request)
            frmName = "frm_translated"
        Else
            taxonomy_language_request.IsTranslated = False
            result_language = _Content.LoadLanguageForTaxonomy(taxonomy_language_request)
            frmName = "frm_nontranslated"
        End If

        result = "<select id=""" & frmName & """ name=""" & frmName & """ onchange=""" & onChangeEv & """>" & vbCrLf

        If (CBool(ShowAllOpt)) Then
            If TaxonomyLanguage = -1 Then
                result = result & "<option value=""-1"" selected>All</option>"
            Else
                result = result & "<option value=""-1"">All</option>"
            End If
        Else
            If (ShowTranslated = False) Then
                result = result & "<option value=""0"">-select language-</option>"
            End If
        End If
        If ((result_language IsNot Nothing) AndAlso (result_language.Count > 0) AndAlso (_Common.EnableMultilingual = 1)) Then
            For Each language As LanguageData In result_language
                If TaxonomyLanguage = language.Id Then
                    result = result & "<option value=" & language.Id & " selected>" & language.Name & "</option>"
                Else
                    result = result & "<option value=" & language.Id & ">" & language.Name & "</option>"
                End If
            Next
        Else
            result = ""
        End If
        If (result.Length > 0) Then
            result = result & "</select>"
        End If
        Return (result)
    End Function
    Private Sub PageSettings()
        If (m_intTotalPages <= 1) Then
            VisiblePageControls(False)
        Else
            VisiblePageControls(True)
            TotalPages.Text = (System.Math.Ceiling(m_intTotalPages)).ToString()
            CurrentPage.Text = m_intCurrentPage.ToString()
            PreviousPage.Enabled = True
            FirstPage.Enabled = True
            NextPage.Enabled = True
            LastPage.Enabled = True
            If m_intCurrentPage = 1 Then
                PreviousPage.Enabled = False
                FirstPage.Enabled = False
            ElseIf m_intCurrentPage = m_intTotalPages Then
                NextPage.Enabled = False
                LastPage.Enabled = False
            End If
        End If
    End Sub

    Private Sub VisiblePageControls(ByVal flag As Boolean)
        TotalPages.Visible = flag
        CurrentPage.Visible = flag
        PreviousPage.Visible = flag
        NextPage.Visible = flag
        LastPage.Visible = flag
        FirstPage.Visible = flag
        PageLabel.Visible = flag
        OfLabel.Visible = flag
    End Sub

    Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                m_intCurrentPage = 1
            Case "Last"
                m_intCurrentPage = Int32.Parse(TotalPages.Text)
            Case "Next"
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) + 1
            Case "Prev"
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) - 1
        End Select
        DisplayPage()
        isPostData.Value = "true"
    End Sub

    Protected Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronDmsMenuJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuIE6Css, API.Css.BrowserTarget.LessThanEqualToIE6)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub

    Protected Sub AssignTextStrings()
        removeItemsLink.Text = _MessageHelper.GetMessage("remove taxonomy items")
        removeItemsLink.ToolTip = _MessageHelper.GetMessage("alt remove button text (taxonomyitems)")
        'result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath & "images/ui/Icons/remove.png", "#", _MessageHelper.GetMessage("alt remove button text (taxonomyitems)"), _MessageHelper.GetMessage("btn remove"), "onclick=""return DeleteItem('items');"""))
    End Sub
    Private Sub DisplayTaxonomyMetadata()
        ' Set hidden values here
        customPropertyObjectId.Value = TaxonomyId
        customPropertyRecordsPerPage.Value = _Common.RequestInformationRef.PagingSize
    End Sub

    Private Function GetValueDropDown(ByRef _propertyDataList As Ektron.Cms.Common.CustomPropertyData, ByVal count As Integer) As String
        Dim result As New StringBuilder
        Dim iObj As Integer = 0
        Dim itemCount As Integer = 0

        result.Append("<select disabled name=""selCustPropVal" & count & """ id=""selCustPropVal" & count & """>")
        If (Not (_propertyDataList Is Nothing)) Then
            For iObj = 0 To _propertyDataList.Items.Count - 1
                If (_propertyDataList.Items(iObj).IsDefault) Then
                    result.Append("<option selected value=""" & _propertyDataList.Items(iObj).PropertyValue & """>")
                    result.Append(_propertyDataList.Items(iObj).PropertyValue)
                    result.Append("</option>")
                Else
                    result.Append("<option value=""" & _propertyDataList.Items(iObj).PropertyValue & """>")
                    result.Append(_propertyDataList.Items(iObj).PropertyValue)
                    result.Append("</option>")
                End If
            Next
        End If
        result.Append("</select>")

        Return result.ToString()
    End Function
End Class

