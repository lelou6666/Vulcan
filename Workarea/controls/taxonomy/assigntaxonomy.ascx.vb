Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports System.DateTime
Imports System.Collections.Generic
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports System.IO

Partial Class assigntaxonomy
    Inherits System.Web.UI.UserControl

    Protected m_refContentApi As New ContentAPI
    Protected m_refCommonApi As New CommonApi
    Protected m_refstyle As New StyleHelper
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected m_refMsg As EkMessageHelper
    Protected m_strPageAction As String = ""
    Protected m_refContent As Ektron.Cms.Content.EkContent
    Protected TaxonomyLanguage As Integer = -1
    Protected TaxonomyId As Long = 0
    Protected TaxonomyParentId As Long = 0
    Protected language_data As LanguageData
    Protected asset_data As AssetInfoData()
    Protected SelectedContentType As Integer = -1
    Protected FolderId As Long = 0
    Protected folder_data_col As Collection
    Protected FolderName As String = ""
    Protected FolderPath As String = ""
    Protected FolderParentId As Long = 0
    Protected folder_request_col As Collection
    Protected ContentIcon As String
    Protected pageIcon As String = ""
    Protected UserIcon As String
    Protected FormsIcon As String = ""
    Protected m_selectedFolderList As String = ""
    Protected m_ObjectType As CMSObjectTypes = CMSObjectTypes.Content
    Protected m_UserType As UserTypes = UserTypes.AuthorType
    Protected m_strSelectedItem As String = "-1"
    Protected m_strKeyWords As String = ""
    Protected m_strSearchText As String = ""
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected contentFetchType As String = ""
    ' Protected user_list As DirectoryUserData() = Array.CreateInstance(GetType(Ektron.Cms.DirectoryUserData), 0)
    Protected user_list As UserData() = Array.CreateInstance(GetType(Ektron.Cms.UserData), 0)
    Protected cgroup_list As CommunityGroupData() = Array.CreateInstance(GetType(Ektron.Cms.CommunityGroupData), 0)
    Protected groupData As New DirectoryAdvancedGroupData

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = m_refContentApi.EkMsgRef
        AppImgPath = m_refContentApi.AppImgPath
        AppPath = m_refContentApi.AppPath
        m_strPageAction = Request.QueryString("action")
        Utilities.SetLanguage(m_refContentApi)
        TaxonomyLanguage = m_refContentApi.ContentLanguage
        If (TaxonomyLanguage = -1) Then
            TaxonomyLanguage = m_refContentApi.DefaultContentLanguage
        End If
        If (Request.QueryString("taxonomyid") IsNot Nothing) Then
            TaxonomyId = Convert.ToInt64(Request.QueryString("taxonomyid"))
        End If
        If (Request.QueryString("parentid") IsNot Nothing) Then
            TaxonomyParentId = Convert.ToInt64(Request.QueryString("parentid"))
        End If
        If (Request.QueryString("type") IsNot Nothing) AndAlso Request.QueryString("type").ToLower() = "author" Then
            m_ObjectType = CMSObjectTypes.User
            m_UserType = UserTypes.AuthorType
        ElseIf (Request.QueryString("type") IsNot Nothing) AndAlso Request.QueryString("type").ToLower() = "member" Then
            m_ObjectType = CMSObjectTypes.User
            m_UserType = UserTypes.MemberShipType
        ElseIf (Request.QueryString("type") IsNot Nothing) AndAlso Request.QueryString("type").ToLower() = "cgroup" Then
            m_ObjectType = CMSObjectTypes.CommunityGroup
        End If

        If (Request.QueryString("contFetchType") IsNot Nothing) AndAlso Request.QueryString("contFetchType").ToLower() <> "" Then
            contentFetchType = Request.QueryString("contFetchType")
        End If
        m_refContent = m_refContentApi.EkContentRef
        FormsIcon = "<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/contentForm.png"" alt=""Form"">"
        ContentIcon = "<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/contentHtml.png"" alt=""Content"">"
        pageIcon = "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/layout.png"" alt=""Page"">" '-HC-
        If Me.m_UserType = UserTypes.AuthorType Then
            UserIcon = "<img src=""" & m_refContentApi.AppPath & "Images/ui/icons/user.png"" alt=""Content"">"
        Else
            UserIcon = "<img src=""" & m_refContentApi.AppPath & "Images/ui/icons/userMembership.png"" alt=""Content"">"
        End If
        If (Page.IsPostBack AndAlso (Request.Form(isPostData.UniqueID) <> "") AndAlso _
            (m_ObjectType = CMSObjectTypes.Content Or (m_ObjectType = CMSObjectTypes.User And Request.Form("itemlist") <> "") Or (m_ObjectType = CMSObjectTypes.CommunityGroup And Request.Form("itemlist") <> ""))) Then
            If (m_strPageAction = "additem") Then
                Dim item_request As New TaxonomyRequest
                item_request.TaxonomyId = TaxonomyId
                item_request.TaxonomyIdList = Validate(Request.Form("itemlist"))
                If m_ObjectType = CMSObjectTypes.User Then
                    item_request.TaxonomyItemType = TaxonomyItemType.User
                ElseIf m_ObjectType = CMSObjectTypes.CommunityGroup Then
                    item_request.TaxonomyItemType = TaxonomyItemType.Group
                End If
                item_request.TaxonomyLanguage = TaxonomyLanguage
                m_refContent.AddTaxonomyItem(item_request)
            ElseIf (m_strPageAction = "addfolder") Then
                Dim sync_request As New TaxonomySyncRequest
                sync_request.TaxonomyId = TaxonomyId
                sync_request.SyncIdList = Validate(Request.Form("selectedfolder")) 'Validate(Request.Form("itemlist"))
                'sync_request.SyncRecursiveIdList = Validate(Request.Form("recursiveidlist"))
                sync_request.TaxonomyLanguage = TaxonomyLanguage
                m_refContent.AddTaxonomySyncFolder(sync_request)
            End If
            If (Request.QueryString("iframe") = "true") Then
                Response.Write("<script type=""text/javascript"">parent.CloseChildPage();</script>")
            Else
                Response.Redirect("taxonomy.aspx?action=view&taxonomyid=" & TaxonomyId)
            End If
        Else
            FolderId = Convert.ToInt64(Request.QueryString("folderid"))

            folder_data_col = m_refContent.GetFolderInfoWithPath(FolderId)
            FolderName = folder_data_col("FolderName")
            FolderParentId = folder_data_col("ParentID")
            FolderPath = folder_data_col("Path")

            folder_request_col = New Collection
            folder_request_col.Add(FolderId, "ParentID")
            folder_request_col.Add("name", "OrderBy")
            folder_data_col = m_refContent.GetAllViewableChildFoldersv2_0(folder_request_col)

            If (m_strPageAction <> "additem") Then
                If Request.QueryString(ContentTypeUrlParam) <> "" Then
                    If IsNumeric(Request.QueryString(ContentTypeUrlParam)) Then
                        SelectedContentType = CLng(Request.QueryString(ContentTypeUrlParam))
                        m_refContentApi.SetCookieValue(ContentTypeUrlParam, SelectedContentType)
                    End If
                ElseIf Ektron.Cms.CommonApi.GetEcmCookie()(ContentTypeUrlParam) <> "" Then
                    If IsNumeric(Ektron.Cms.CommonApi.GetEcmCookie()(ContentTypeUrlParam)) Then
                        SelectedContentType = CLng(Ektron.Cms.CommonApi.GetEcmCookie()(ContentTypeUrlParam))
                    End If
                End If
                asset_data = m_refContent.GetAssetSuperTypes()
            End If
            RegisterResources()
            TaxonomyToolBar()
            If (Not Page.IsPostBack OrElse m_ObjectType = CMSObjectTypes.User OrElse m_ObjectType = CMSObjectTypes.CommunityGroup) Then
                ' avoid redisplay when clicking next/prev buttons
                DisplayPage()
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        PageSettings()
    End Sub

    Private Sub DisplayPage()
        If (m_strPageAction <> "addfolder") Then
            PopulateGridData()
        Else
            Dim taxonomy_sync_folder As TaxonomyFolderSyncData() = Nothing
            Dim tax_sync_folder_req As New TaxonomyBaseRequest
            tax_sync_folder_req.TaxonomyId = TaxonomyId
            tax_sync_folder_req.TaxonomyLanguage = TaxonomyLanguage
            taxonomy_sync_folder = m_refContent.GetAllAssignedCategoryFolder(tax_sync_folder_req)
            If (taxonomy_sync_folder IsNot Nothing AndAlso taxonomy_sync_folder.Length > 0) Then
                For cnt As Integer = 0 To taxonomy_sync_folder.Length - 1
                    If (m_selectedFolderList.Length > 0) Then
                        m_selectedFolderList = m_selectedFolderList & "," & taxonomy_sync_folder(cnt).FolderId
                    Else
                        m_selectedFolderList = taxonomy_sync_folder(cnt).FolderId
                    End If
                Next
            End If
            ' add to the body's onload event to load up the folder tree
        End If
    End Sub

    Private Function Validate(ByVal value As String) As String
        If (value IsNot Nothing) Then
            Return value
        Else
            Return ""
        End If
    End Function
    Private Sub PopulateGridData()
        If (TaxonomyItemList.Columns.Count = 0) Then
            TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("ITEM1", "", "info", HorizontalAlign.NotSet, HorizontalAlign.NotSet, Unit.Percentage(0), Unit.Percentage(0), False, False))
        End If

        Dim iframe As String = ""
        If (Request.QueryString("iframe") IsNot Nothing AndAlso Request.QueryString("iframe") <> "") Then
            iframe = "&iframe=true"
        End If
        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("ITEM1", GetType(String)))

        Dim i As Integer = 0
        dr = dt.NewRow
        If (m_strPageAction = "additem") AndAlso Me.m_ObjectType = CMSObjectTypes.User Then
            dr(0) = m_refMsg.GetMessage("lbl select users") & "<br/>"
        ElseIf (m_strPageAction = "additem") AndAlso Me.m_ObjectType = CMSObjectTypes.CommunityGroup Then
            dr(0) = m_refMsg.GetMessage("lbl select cgroups") & "<br/>"
        ElseIf (m_strPageAction = "additem") Then
            dr(0) = m_refMsg.GetMessage("assigntaxonomyitemlabel") & "<br/>"
        Else
            dr(0) = m_refMsg.GetMessage("assigntaxonomyfolderlabel") & "<br/>"
        End If
        dt.Rows.Add(dr)

        If Me.m_ObjectType = CMSObjectTypes.Content Then
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("generic Path") & FolderPath
            dt.Rows.Add(dr)

            dr = dt.NewRow
            If (FolderId <> 0) Then
                dr(0) = "<a href=""taxonomy.aspx?action=" & m_strPageAction & "&taxonomyid=" & TaxonomyId & "&folderid=" & FolderParentId & "&parentid=" & FolderParentId & iframe
                dr(0) = dr(0) & "&title=""" & m_refMsg.GetMessage("alt: generic previous dir text") & """><img src=""" & m_refContentApi.AppPath & "images/ui/icons/folderUp.png" & """ border=""0"" title=""" & m_refMsg.GetMessage("alt: generic previous dir text") & """ alt=""" & m_refMsg.GetMessage("alt: generic previous dir text") & """ align=""absbottom"">..</a>"
            End If
            dt.Rows.Add(dr)


            If Not IsNothing(folder_data_col) Then
                For Each folder As Collection In folder_data_col
                    dr = dt.NewRow
                    dr(0) = "<a href=""taxonomy.aspx?action=" & m_strPageAction & "&taxonomyid=" & TaxonomyId & "&folderid=" & folder("id") & "&parentid=" & FolderParentId & iframe
                    dr(0) += "&title=""" + m_refMsg.GetMessage("alt: generic view folder content text") & """><img src="""
                    Select Case Convert.ToInt32(folder("FolderType"))
                        Case EkEnumeration.FolderType.Catalog
                            dr(0) += m_refContentApi.AppPath & "images/ui/icons/folderGreen.png"
                        Case EkEnumeration.FolderType.Community
                            dr(0) += m_refContentApi.AppPath & "images/ui/icons/folderCommunity.png"
                        Case EkEnumeration.FolderType.Blog
                            dr(0) += m_refContentApi.AppPath & "images/ui/icons/folderBlog.png"
                        Case EkEnumeration.FolderType.DiscussionBoard
                            dr(0) += m_refContentApi.AppPath & "images/ui/icons/folderBoard.png"
                        Case EkEnumeration.FolderType.DiscussionForum
                            dr(0) += m_refContentApi.AppPath & "images/ui/icons/folderBoard.png"
                        Case EkEnumeration.FolderType.Calendar
                            dr(0) += m_refContentApi.AppPath & "images/ui/icons/folderCalendar.png"
                        Case EkEnumeration.FolderType.Domain
                            dr(0) += m_refContentApi.AppPath & "images/ui/icons/foldersite.png"
                        Case Else
                            dr(0) += m_refContentApi.AppPath & "images/ui/icons/folder.png"
                    End Select
                    dr(0) += """ border=""0"" title=""" & m_refMsg.GetMessage("alt: generic view folder content text") & """ alt=""" & m_refMsg.GetMessage("alt: generic view folder content text") & """ align=""absbottom""></a> "
                    dr(0) += "<a href=""taxonomy.aspx?action=" & m_strPageAction & "&taxonomyid=" & TaxonomyId & "&folderid=" & folder("id") & "&parentid=" & FolderParentId & iframe & "&title=""" + m_refMsg.GetMessage("alt: generic view folder content text") & """>" & folder("Name") & "</a>"
                    dt.Rows.Add(dr)
                Next
            End If
            If (m_strPageAction = "additem") Then
                Dim taxonomy_unassigneditem_arr As ContentData()
                Dim request As TaxonomyRequest = New TaxonomyRequest
                request.TaxonomyId = TaxonomyId
                request.TaxonomyLanguage = TaxonomyLanguage
                request.FolderId = FolderId
                If (contentFetchType.ToLower = "activecontent") Then
                    request.TaxonomyItemType = TaxonomyItemType.ActiveContent
                ElseIf (contentFetchType.ToLower = "archivedcontent") Then
                    request.TaxonomyItemType = TaxonomyItemType.ArchivedContent
                Else
                    request.TaxonomyItemType = -1
                End If

                ' get total #pages first because the API doesn't return it (lame slow way to do this)-:
                request.PageSize = 99999999
                request.CurrentPage = 1
                taxonomy_unassigneditem_arr = m_refContent.ReadAllUnAssignedTaxonomyItems(request)
                m_intTotalPages = Math.Truncate((taxonomy_unassigneditem_arr.Length + (m_refContentApi.RequestInformationRef.PagingSize - 1)) / m_refContentApi.RequestInformationRef.PagingSize)

                ' get the real page data set
                request.PageSize = m_refContentApi.RequestInformationRef.PagingSize
                request.CurrentPage = m_intCurrentPage
                taxonomy_unassigneditem_arr = m_refContent.ReadAllUnAssignedTaxonomyItems(request)
                'm_intTotalPages = Math.Truncate((request.RecordsAffected + (m_refContentApi.RequestInformationRef.PagingSize - 1)) / m_refContentApi.RequestInformationRef.PagingSize)

                Dim library_dat As LibraryData
                For Each taxonomy_item As ContentData In taxonomy_unassigneditem_arr
                    dr = dt.NewRow
                    If taxonomy_item.Type = 1 Or taxonomy_item.Type = 2 Then
                        dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;" & GetTypeIcon(taxonomy_item.Type, taxonomy_item.SubType) & "&nbsp;" & taxonomy_item.Title
                    ElseIf taxonomy_item.Type = 3 Then
                        dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "Images/ui/icons/contentArchived.png" & """&nbsp;border=""0""  alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                    ElseIf taxonomy_item.Type = 1111 Then
                        dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/asteriskOrange.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                    ElseIf taxonomy_item.Type = 1112 Then
                        dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/tree/folderBlog.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                    ElseIf taxonomy_item.Type = 7 Then
                        library_dat = m_refContentApi.GetLibraryItemByContentID(taxonomy_item.Id)
                        Dim extension As String = ""
                        extension = System.IO.Path.GetExtension(library_dat.FileName)
                        Select Case extension.ToLower()
                            Case ".doc"
                                dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/FileTypes/word.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                            Case ".ppt"
                                dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/FileTypes/powerpoint.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                            Case ".pdf"
                                dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/FileTypes/acrobat.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                            Case ".xls"
                                dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/FileTypes/excel.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                            Case ".jpg", ".jpeg", ".png", ".gif", ".bmp"
                                dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/FileTypes/image.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                            Case Else ' other files
                                dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/book.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                        End Select
                    ElseIf taxonomy_item.Type = 3333 Then
                        dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "Images/ui/icons/brick.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                    ElseIf taxonomy_item.AssetData.ImageUrl = "" And (taxonomy_item.Type <> 1 And taxonomy_item.Type <> 2 And taxonomy_item.Type <> 3 And taxonomy_item.Type <> 1111 And taxonomy_item.Type <> 1112 And taxonomy_item.Type <> 3333) Then
                        dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/book.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                    Else
                        'Bad Approach however no other way untill AssetManagement/Images/ are updated with version 8 images or DMS points to workarea images
                        If taxonomy_item.AssetData.ImageUrl = "" Then
                            dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/book.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                        Else
                            Select Case Path.GetFileName(taxonomy_item.AssetData.ImageUrl).ToLower()
                                Case "ms-word.gif"
                                    dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/FileTypes/word.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                                Case "ms-excel.gif"
                                    dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/FileTypes/excel.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                                Case "ms-powerpoint.gif"
                                    dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/FileTypes/powerpoint.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                                Case "adobe-pdf.gif"
                                    dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/FileTypes/acrobat.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                                Case "image.gif"
                                    dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/FileTypes/image.png" & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                                Case Else
                                    dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & taxonomy_item.Id & """/>&nbsp;<img src=""" & taxonomy_item.AssetData.ImageUrl & """ alt=""" & taxonomy_item.AssetData.FileName & """></img>&nbsp;" & taxonomy_item.Title
                            End Select
                        End If
                    End If
                    dt.Rows.Add(dr)
                Next
            End If
        ElseIf Me.m_ObjectType = CMSObjectTypes.CommunityGroup Then
            CollectSearchText()
            dr = dt.NewRow
            dr(0) = "<input type=text size=25 id=""txtSearch"" name=""txtSearch"" value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event)"">"
            dr(0) &= "<input type=button value=""Search"" id=""btnSearch"" name=""btnSearch""  class=""ektronWorkareaSearch"" onclick=""searchuser();"">"
            dt.Rows.Add(dr)
            GetAssignedCommunityGroups()
            GetCommunityGroups()
            If cgroup_list IsNot Nothing Then
                For j As Integer = 0 To (cgroup_list.Length - 1)

                    dr = dt.NewRow
                    If DoesGroupExistInList(cgroup_list(j).GroupId) Then
                        dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;" & GetTypeIcon(CMSObjectTypes.User.GetHashCode()) & "<input type=""checkbox"" checked=""checked"" disabled=""disabled"" id=""itemlistNoId"" name=""itemlistNoId"" value=""" & cgroup_list(j).GroupId & """/>" & cgroup_list(j).GroupName
                    Else
                        dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;" & GetTypeIcon(CMSObjectTypes.User.GetHashCode()) & "<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & cgroup_list(j).GroupId & """/>" & cgroup_list(j).GroupName
                    End If

                    dt.Rows.Add(dr)
                Next
            End If
        ElseIf Me.m_ObjectType = CMSObjectTypes.User Then
            CollectSearchText()
            dr = dt.NewRow
            dr(0) = "<input type=text size=25 id=""txtSearch"" name=""txtSearch"" value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event)"">"
            dr(0) &= "<select id=""searchlist"" name=""searchlist"">"
            dr(0) &= "<option value=-1" & IsSelected("-1") & ">All</option>"
            dr(0) &= "<option value=""last_name""" & IsSelected("last_name") & ">Last Name</option>"
            dr(0) &= "<option value=""first_name""" & IsSelected("first_name") & ">First Name</option>"
            dr(0) &= "<option value=""user_name""" & IsSelected("user_name") & ">User Name</option>"
            dr(0) &= "</select><input type=button value=""Search"" id=""btnSearch"" name=""btnSearch"" class=""ektronWorkareaSearch""  onclick=""searchuser();"">"
            dt.Rows.Add(dr)

            GetUsers()
            If user_list IsNot Nothing Then
                For j As Integer = 0 To (user_list.Length - 1)
                    dr = dt.NewRow
                    dr(0) = "&nbsp;&nbsp;&nbsp;&nbsp;" & GetTypeIcon(CMSObjectTypes.User.GetHashCode()) & "<input type=""checkbox"" id=""itemlist"" name=""itemlist"" value=""" & user_list(j).Id & """/>" & IIf(user_list(j).DisplayName <> "", user_list(j).DisplayName, user_list(j).Username)
                    dt.Rows.Add(dr)
                Next
            End If
        End If
        Dim dv As New DataView(dt)
        TaxonomyItemList.DataSource = dv
        TaxonomyItemList.DataBind()
    End Sub
    Private Sub GetUsers()
        If Trim(m_strSearchText) <> "" Then
            Dim req As New UserRequestData
            Dim m_refUserApi As New UserAPI
            req.Type = IIf(Me.m_UserType = UserTypes.AuthorType, 0, 1) ' m_intGroupType
            req.Group = IIf(Me.m_UserType = UserTypes.AuthorType, 2, 888888)
            req.RequiredFlag = 0
            req.OrderBy = ""
            req.OrderDirection = "asc"
            req.SearchText = m_strSearchText
            req.PageSize = m_refContentApi.RequestInformationRef.PagingSize
            req.CurrentPage = m_intCurrentPage
            user_list = m_refUserApi.GetAllUsers(req)
            ' user_list = Me.m_refContent.GetAllUnAssignedDirectoryUser(TaxonomyId)
            m_intTotalPages = req.TotalPages
        End If
    End Sub
    Private Sub GetAssignedCommunityGroups()
        If Page.IsPostBack Then
            Dim cReq As New DirectoryGroupRequest
            cReq.CurrentPage = m_intCurrentPage
            cReq.PageSize = m_refCommonApi.RequestInformationRef.PagingSize
            cReq.DirectoryId = TaxonomyId
            cReq.DirectoryLanguage = TaxonomyLanguage
            cReq.GetItems = True
            cReq.SortDirection = ""
            groupData = m_refCommonApi.CommunityGroupRef.LoadDirectory(cReq)
        End If
    End Sub
    Private Function DoesGroupExistInList(ByVal GroupID As Long) As Boolean
        If groupData IsNot Nothing AndAlso groupData.DirectoryItems IsNot Nothing AndAlso groupData.DirectoryItems.Length > 0 Then
            For Each _gData As CommunityGroupData In groupData.DirectoryItems
                If _gData.GroupId = GroupID Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function

    Private Sub GetCommunityGroups()
        If Page.IsPostBack Then
            Dim cReq As New CommunityGroupRequest
            cReq.CurrentPage = m_intCurrentPage
            cReq.SearchText = m_strKeyWords
            cReq.PageSize = m_refContentApi.RequestInformationRef.PagingSize
            cgroup_list = (New Community.CommunityGroupAPI()).GetAllCommunityGroups(cReq)
            m_intTotalPages = cReq.TotalPages
        End If
    End Sub
    Private Sub CollectSearchText()
        m_strKeyWords = Request.Form("txtSearch")
        m_strSelectedItem = Request.Form("searchlist")
        If (m_strSelectedItem = "-1") Then
            m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%' OR last_name like '%" & Quote(m_strKeyWords) & "%' OR user_name like '%" & Quote(m_strKeyWords) & "%') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" & TaxonomyId & ")"
        ElseIf (m_strSelectedItem = "last_name") Then
            m_strSearchText = " (last_name like '%" & Quote(m_strKeyWords) & "%') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" & TaxonomyId & ")"
        ElseIf (m_strSelectedItem = "first_name") Then
            m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" & TaxonomyId & ")"
        ElseIf (m_strSelectedItem = "user_name") Then
            m_strSearchText = " (user_name like '%" & Quote(m_strKeyWords) & "%') AND u.user_id not in (select taxonomy_item_id from taxonomy_item_tbl where taxonomy_item_type=1 and taxonomy_id=" & TaxonomyId & ")"
        End If
    End Sub
    Private Function PopulateFolderRecursive(ByVal data As TaxonomyFolderSyncData(), ByVal id As Long) As String
        Dim result As String = ""
        For Each item As TaxonomyFolderSyncData In data
            If (id = item.FolderId) Then
                result = result & "<div id=""_dv" & id & """ style=""position:relative;display:block;"">"
                result = result & "<span id=""spacechk"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>"
                result = result & "<span id=""spanchk"">Include subfolder(s).</span>"
                result = result & "</div>"
                Exit For
            End If
        Next
        If (result = "") Then
            result = result & "<div id=""_dv" & id & """ style=""position:relative;display:none;""></div>"
        End If
        Return result
    End Function
    Private Function Checked(ByVal data As TaxonomyFolderSyncData(), ByVal id As Long) As String
        Dim result As String = ""
        For Each item As TaxonomyFolderSyncData In data
            If (id = item.FolderId) Then
                result = " checked "
                Exit For
            End If
        Next
        Return result
    End Function
    Private Function Checked(ByVal value As Boolean) As String
        If (value) Then
            Return "checked"
        Else
            Return ""
        End If
    End Function
    Private Function OnClickEvent(ByVal id As Object) As String
        Dim result As String = ""
        If (m_strPageAction <> "additem") Then
            result = " onclick=""OnFolderCheck(" & id & ",this);"""
        End If
        Return result
    End Function
    Private Function GetTypeIcon(ByVal type As Integer, Optional ByVal subType As EkEnumeration.CMSContentSubtype = CMSContentSubtype.Content) As String
        If type = CMSObjectTypes.User.GetHashCode AndAlso Me.m_ObjectType = CMSObjectTypes.User Then
            Return UserIcon
        ElseIf type = 2 AndAlso Me.m_ObjectType = CMSObjectTypes.Content Then
            Return FormsIcon
        ElseIf type = 1 Then
            If (subType = CMSContentSubtype.PageBuilderData Or subType = CMSContentSubtype.PageBuilderMasterData) Then
                Return pageIcon
            End If
            Return ContentIcon
        Else
            Return Nothing
        End If
    End Function

    Private Sub TaxonomyToolBar()
        If (m_strPageAction <> "additem") Then
            divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("assign folders to taxonomy page title"))
        Else
            divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("assign items to taxonomy page title"))
        End If

        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)

        result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (taxonomy)"), m_refMsg.GetMessage("btn update"), "onclick=""Validate();"""))
        If (Request.QueryString("iframe") = "true") Then
            result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("generic Cancel"), "onclick=""parent.CancelIframe();"""))
        Else
            result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "taxonomy.aspx?action=view&taxonomyid=" & TaxonomyId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If

        If (m_strPageAction = "additem") Then
            result.Append("<td>&nbsp;|&nbsp;</td>")

            result.Append("<td class=""label"">")
            result.Append(m_refMsg.GetMessage("view language"))
            result.Append("&nbsp;</td>")
            result.Append("<td><select id=""typelist"" name=""typelist"" OnChange=""ChangeView();"">")
            result.Append("<option value='" & Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content & "' " & IIf(Me.m_ObjectType = CMSObjectTypes.Content, "selected", "") & ">").Append(Me.m_refMsg.GetMessage("content button text")).Append("</option>")
            result.Append("<option value='" & Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User & UserTypes.AuthorType & "' " & IIf(Me.m_ObjectType = CMSObjectTypes.User And Me.m_UserType = UserTypes.AuthorType, "selected", "") & ">").Append(Me.m_refMsg.GetMessage("lbl cms authors")).Append("</option>")
            result.Append("<option value='" & Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.User & UserTypes.MemberShipType & "' " & IIf(Me.m_ObjectType = CMSObjectTypes.User And Me.m_UserType = UserTypes.MemberShipType, "selected", "") & ">").Append(Me.m_refMsg.GetMessage("lbl members")).Append("</option>")
            result.Append("<option value='" & Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup & "' " & IIf(Me.m_ObjectType = CMSObjectTypes.CommunityGroup, "selected", "") & ">").Append(Me.m_refMsg.GetMessage("lbl community groups")).Append("</option>")
            result.Append("</select></td>")
            'End If
            If Me.m_ObjectType = CMSObjectTypes.Content Then
                result.Append("<td>&nbsp;</td>")
                result.Append("<td>")
                result.Append(m_refMsg.GetMessage("type label"))
                result.Append("&nbsp;</td><td><select style='BACKGROUND-COLOR: #d8e6ff' id=""contenttype"" name=""contenttype"" bgcolor=blue OnChange=""ChangeView();"">")
                result.Append("<option value='alltypes'" & IIf(contentFetchType.ToLower = "", "selected", "") & ">").Append(Me.m_refMsg.GetMessage("lbl all types")).Append("</option>")
                result.Append("<option value='activecontent'" & IIf(contentFetchType.ToLower = "activecontent", "selected", "") & ">").Append(Me.m_refMsg.GetMessage("lbl content")).Append("</option>")
                result.Append("<option value='archivedcontent'" & IIf(contentFetchType.ToLower = "archivedcontent", "selected", "") & ">").Append(Me.m_refMsg.GetMessage("archive content title")).Append("</option>")
                result.Append("</select></td>")
            End If
            If (Not (IsNothing(asset_data))) Then
                If (asset_data.Length > 0) Then
                    result.Append("<td>&nbsp;</td>")
                    result.Append("<td><select id=selAssetSupertype name=selAssetSupertype OnChange=""UpdateView();"">")
                    If Ektron.Cms.Common.EkConstants.CMSContentType_NonLibraryContent = SelectedContentType Then
                        result.Append("<option value='" & Ektron.Cms.Common.EkConstants.CMSContentType_NonLibraryContent & "' selected>All Types</option>")
                    Else
                        result.Append("<option value='" & Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes & "'>All Types</option>")
                    End If
                    If Ektron.Cms.Common.EkConstants.CMSContentType_Content = SelectedContentType Then
                        result.Append("<option value='" & Ektron.Cms.Common.EkConstants.CMSContentType_Content & "' selected>HTML Content</option>")
                    Else
                        result.Append("<option value='" & Ektron.Cms.Common.EkConstants.CMSContentType_Content & "'>HTML Content</option>")
                    End If
                    For Each data As AssetInfoData In asset_data
                        If (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= data.TypeId And data.TypeId <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max) Then
                            If "*" <> data.PluginType Then
                                result.Append("<option value='" & data.TypeId & "'")
                                If data.TypeId = SelectedContentType Then
                                    result.Append(" selected")
                                End If
                                result.Append(">" & data.CommonName & "</option>")
                            End If
                        End If
                    Next
                    result.Append("</select></td>")
                End If
            End If
        End If

        If (m_strPageAction <> "addfolder") Then
            result.Append("<td>" & m_refstyle.GetHelpButton("AddTaxonomyOrCategoryItem") & "</td>")
        Else
            result.Append("<td>" & m_refstyle.GetHelpButton("AddTaxonomyOrCategoryFolder") & "</td>")
        End If
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Private Function IsSelected(ByVal val As String) As String
        If (val = m_strSelectedItem) Then
            Return (" selected ")
        Else
            Return ("")
        End If
    End Function
    Public Function getURL() As String
        Dim sRet As String = ""
        If Request.QueryString.Count > 0 Then
            For i As Integer = 0 To (Request.QueryString.Count - 1)
                If (Request.QueryString.Keys(i).ToLower() <> "type" AndAlso Request.QueryString.Keys(i).ToLower() <> "contfetchtype") Then
                    sRet &= Request.QueryString.Keys(i) & "=" & Request.QueryString(i) & "&"
                End If
            Next
        End If
        If sRet.Length > 0 AndAlso sRet(sRet.Length - 1) = "&" Then
            sRet = "taxonomy.aspx?" & sRet.Substring(0, sRet.Length - 1)
        Else
            sRet = "taxonomy.aspx?" & sRet
        End If
        If sRet.ToLower.IndexOf("langtype") < 0 Then
            sRet = sRet & "&LangType=" & m_refContentApi.RequestInformationRef.ContentLanguage
        End If
        Return sRet
    End Function
    Private Function Quote(ByVal KeyWords As String) As String
        Dim result As String = KeyWords
        If (KeyWords.Length > 0) Then
            result = KeyWords.Replace("'", "''")
        End If
        Return result
    End Function

    Private Sub RegisterResources()
        If (m_strPageAction = "addfolder") Then
            API.Css.RegisterCss(Me, m_refContentApi.ApplicationPath & "Tree/css/com.ektron.ui.contextmenu.css", "ContextMenuCSS")
            API.Css.RegisterCss(Me, m_refContentApi.ApplicationPath & "Tree/css/com.ektron.ui.tree.css", "TreeCSS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.explorer.init.js", "ExplorerInitJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.explorer.js", "ExplorerJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.explorer.config.js", "ExplorerConfigJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.explorer.windows.js", "ExplorerWindowsJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.cms.types.js", "CMSTypesJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.cms.parser.js", "CMSParserJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.cms.toolkit.js", "CMSToolkitJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.cms.api.js", "CMSAPIJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.ui.contextmenu.js", "UIContextMenuJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.ui.iconlist.js", "UIIconlistJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.ui.explore.js", "UIExploreJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.ui.assignfolder.js", "UIAssignFolderJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.net.http.js", "NetHTTPJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.lang.exception.js", "LangExceptionJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.utils.form.js", "UtilsFormJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.utils.log.js", "UtilsLogJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.utils.dom.js", "UtilsDOMJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.utils.debug.js", "UtilsDebugJS")
            API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "Tree/js/com.ektron.utils.string.js", "UtilsStringJS")
        End If
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronInputLabelJS)
        API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronSearchBoxInputLabelInitJS")
    End Sub

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
        m_intTotalPages = Int32.Parse(TotalPages.Text)
        Select Case e.CommandName
            Case "First"
                m_intCurrentPage = 1
            Case "Last"
                m_intCurrentPage = m_intTotalPages
            Case "Next"
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) + 1
                CurrentPage.Text = m_intCurrentPage.ToString()
            Case "Prev"
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) - 1
                CurrentPage.Text = m_intCurrentPage.ToString()
        End Select
        If (m_intCurrentPage < 1) Then
            m_intCurrentPage = 1
        End If
        If (m_intCurrentPage > m_intTotalPages) Then
            m_intCurrentPage = m_intTotalPages
        End If
        DisplayPage()
        isPostData.Value = "true"
    End Sub
End Class
