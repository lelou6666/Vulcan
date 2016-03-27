Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Partial Class Collections
    Inherits System.Web.UI.Page

    Protected ContentIcon As String = ""
    Protected WebEventIcon As String = ""
    Protected pageIcon As String = ""
    Protected formsIcon As String = ""
    Protected AppImgPath As String = ""
    Protected FolderId As Long = 0
    Protected actName As String = "AddLink"
    Protected notSupportIFrame As String = ""
    Protected AppName As String = ""
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected CanCreateContent As Boolean = False
    Protected perm_data As PermissionData
    Protected m_refMsg As EkMessageHelper
    Protected gtNavs, cTmp, cFolders, FolderName, ParentFolderId, fPath As Object
    Protected m_refContent As Ektron.Cms.Content.EkContent
    Protected AddType As String = ""
    Protected cConts As EkContentCol
    Protected CollectionContentItems As Collection
    Protected nId As Long
    Protected subfolderid As Long
    Protected locID As Long
    Protected g_ContentTypeSelected As Integer = -1
    Protected cRecursive As Collection
    Protected rec As Boolean
    Protected MenuTitle As String
    Protected CollectionTitle As String
    Protected ErrorString As String
    Protected asset_data As AssetInfoData()
    Protected lContentType As Integer
    Protected ContentLanguage As Integer
    Protected NoWorkAreaAttribute As String
    Protected backId As Long
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1
    Protected result As New System.Text.StringBuilder
    Protected count As Integer
    Protected lAddMultiType As Long
    Protected bSelectedFound As Boolean
    Protected status As String = ""
    Public objStyle As StyleHelper
    Protected Const ALL_CONTENT_LANGUAGES As Integer = -1
    Protected CONTENT_LANGUAGES_UNDEFINED As Integer = 0
    Protected gtMsgObj, gtMess, CollectionID, msgs, currentUserID, PageTitle, AppPath, sitePath As Object
    Protected gtNav, lLoop, siteObj, cPerms, cLinkArray, fLinkArray As Object
    Protected gtObj, gtLinks, OrderBy, cLanguagesArray As Object
    Protected action, folder, menuIcon, libraryIcon, linkIcon, sTitleBar As Object
    Protected maID, mpID As Object
    Protected EnableMultilingual As Object
    Protected AppUI As New Ektron.Cms.UI.CommonUI.ApplicationAPI
    Protected Callbackpage As String = ""
    Protected MsgHelper As EkMessageHelper
    Protected contentData As EkContentCol
    Protected m_refApi As New CommonApi
    Protected contentApi As New ContentAPI
    Protected AncestorIDParam As String = ""
    Protected ParentIDParam As String = ""
    Protected checkout As String = ""
    Protected bCheckedout As Boolean = False
    Protected m_strKeyWords As String = ""

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RegisterResources()

        Head1.Title = (AppUI.AppName & " " & "Collections")

        objStyle = New StyleHelper
        MsgHelper = m_refApi.EkMsgRef
        EnableMultilingual = m_refApi.EnableMultilingual
        Utilities.ValidateUserLogin()
        If (m_refContentApi.RequestInformationRef.IsMembershipUser OrElse m_refContentApi.RequestInformationRef.UserId = 0) Then
            Response.Redirect("reterror.aspx?info=" & m_refContentApi.EkMsgRef.GetMessage("msg login collection administrator"), True)
            Exit Sub
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
        m_refContentApi.FilterByLanguage = ContentLanguage
        m_refApi.EkContentRef.RequestInformation.ContentLanguage = ContentLanguage
        NoWorkAreaAttribute = ""
        If (Request.QueryString("noworkarea") = "1") Then
            NoWorkAreaAttribute = "&noworkarea=1"
        End If
        status = ""
        If (Request.QueryString("status") <> "") Then
            status = "&status=o"
        End If
        If Not Page.IsPostBack Then
            If (Request.QueryString("checkout") <> "") Then
                If (Request.QueryString("checkout").ToString().ToLower() = "true") Then
                    Dim iColID As Long = Request.QueryString("nid")
                    m_refContentApi.EkContentRef.CheckoutEcmCollection(iColID)
                End If
                status = status & "&checout=" & Request.QueryString("checkout")
            End If
        End If
        If (Request.QueryString("action") = "AddLink" And IsPostBack() = False) Then
            FillContentFolderInfo()
        ElseIf ((Request.QueryString("action") = "ViewCollectionReport" And IsPostBack() = False) OrElse (Request.QueryString("action") = "ViewCollectionReport" And IsPostBack And Request.Form(isSearchPostData.UniqueID) <> "" And Request.Form(isCPostData.UniqueID) <> "")) Then
            LoadCollectionList()

            If (Request.QueryString("rf") = "1") Then
                litRefreshAccordion.Text = "<script language=""javascript"">" & vbCrLf _
                 + "top.refreshCollectionAccordion(" & ContentLanguage & ");" & vbCrLf _
                 + "</script>" & vbCrLf
            End If

        ElseIf ((Request.QueryString("action") = "ViewMenuReport" And IsPostBack() = False) OrElse (Request.QueryString("action") = "ViewMenuReport" And IsPostBack And Request.Form(isSearchPostData.UniqueID) <> "" And Request.Form(isCPostData.UniqueID) <> "")) Then
            LoadMenuList()

            If (Request.QueryString("rf") = "1") Then
                litRefreshAccordion.Text = "<script language=""javascript"">" & vbCrLf _
                 + "top.refreshMenuAccordion(" & ContentLanguage & ");" & vbCrLf _
                 + "</script>" & vbCrLf
            End If

        ElseIf (Request.QueryString("action") = "AddLink" And isPostData.Value <> "") Then
            If (Request.QueryString("addto") <> "" AndAlso Request.QueryString("addto").ToLower() = "menu") Then
                Server.Transfer("collectionaction.aspx?LangType=" & ContentLanguage & "&Action=doAddMenuItem&type=content&folderid=" & Request.QueryString("folderid") & "&nid=" & Request.QueryString("nid") & "&iframe=" & Request.QueryString("iframe") & NoWorkAreaAttribute & "&back=" & Server.UrlEncode(Request.QueryString("back")))
            Else
                Server.Transfer("collectionaction.aspx?LangType=" & ContentLanguage & "&Action=doAddLinks&folderid=" & Request.QueryString("folderid") & "&nid=" & Request.QueryString("nid") & NoWorkAreaAttribute & status & "&back=" & Server.UrlEncode(Request.QueryString("back")))
            End If
        ElseIf ((Request.QueryString("action") = "View") Or (Request.QueryString("action") = "ViewStage")) Then
            If (Request.QueryString("rf") = "1") Then
                litRefreshCollAccordion.Text = "<script language=""javascript"">" & vbCrLf _
                 + "top.refreshCollectionAccordion(" & ContentLanguage & ");" & vbCrLf _
                 + "</script>" & vbCrLf
            End If
        End If
        If isPostData.Value = "" Then
            isPostData.Value = "true"
        End If
    End Sub
    Private Sub LoadMenuList()
        Dim req As New PageRequestData
        req.PageSize = m_refContentApi.RequestInformationRef.PagingSize
        req.CurrentPage = _currentPageNumber
        m_strKeyWords = Request.Form("txtSearch")
        If (m_strKeyWords Is Nothing) Then
            m_strKeyWords = ""
        End If
        Dim menu_list As Collection = m_refApi.EkContentRef.GetMenuReport(m_strKeyWords, req)
        TotalPagesNumber = req.TotalPages
        PageSettings()
        ViewAllMenuToolBar(m_strKeyWords)
        If (menu_list IsNot Nothing AndAlso menu_list.Count > 0) Then
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("TITLE", MsgHelper.GetMessage("generic Title"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), False, False))
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("ID", MsgHelper.GetMessage("generic ID"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("LANGUAGE", MsgHelper.GetMessage("generic language"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("DESCRIPTION", MsgHelper.GetMessage("generic Description"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(35), Unit.Percentage(40), False, False))
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("PATH", MsgHelper.GetMessage("generic Path"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), False, False))
            Dim dt As New DataTable
            Dim dr As DataRow
            dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
            dt.Columns.Add(New DataColumn("ID", GetType(String)))
            dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
            dt.Columns.Add(New DataColumn("DESCRIPTION", GetType(String)))
            dt.Columns.Add(New DataColumn("PATH", GetType(String)))

            For i As Integer = 1 To menu_list.Count
                dr = dt.NewRow
                If (m_refApi.TreeModel = 0) Then
                    dr("TITLE") = "<a href=""collections.aspx?folderid=" & menu_list(i)("FolderID") & "&Action=ViewMenu&nid=" & menu_list(i)("MenuID") & "&bpage=reports&LangType=" & menu_list(i)("ContentLanguage") & """  alt='" & MsgHelper.GetMessage("generic View") & " """ & Replace(menu_list(i)("MenuTitle"), "'", "`") & """' title='" & MsgHelper.GetMessage("generic View") & " """ & Replace(menu_list(i)("MenuTitle"), "'", "`") & """'>" & menu_list(i)("MenuTitle") & "</a>"
                Else
                    Dim enableQDOparam As String = ""
                    If (menu_list(i)("EnableReplication") = 1) Then
                        enableQDOparam = "&qdo=1"
                    End If
                    'dr("TITLE") = "<a href=""menutree.aspx?folderid=" & menu_list(i)("FolderID") & "&nid=" & menu_list(i)("MenuID") & "&bpage=reports&LangType=" & menu_list(i)("ContentLanguage") & """  alt='" & MsgHelper.GetMessage("generic View") & " """ & Replace(menu_list(i)("MenuTitle"), "'", "`") & """' title='" & MsgHelper.GetMessage("generic View") & " """ & Replace(menu_list(i)("MenuTitle"), "'", "`") & enableQDOparam & """'>" & menu_list(i)("MenuTitle") & "</a>"
                    dr("TITLE") = "<a href=""menu.aspx?Action=viewcontent&menuid=" & menu_list(i)("MenuID") & "&LangType=" & menu_list(i)("ContentLanguage") & "&treeViewId=-3" & """  alt='" & MsgHelper.GetMessage("generic View") & " """ & Replace(menu_list(i)("MenuTitle"), "'", "`") & """' title='" & MsgHelper.GetMessage("generic View") & " """ & Replace(menu_list(i)("MenuTitle"), "'", "`") & enableQDOparam & """'>" & menu_list(i)("MenuTitle") & "</a>"
                End If
                dr("ID") = menu_list(i)("MenuID")
                dr("LANGUAGE") = menu_list(i)("ContentLanguage")
                dr("DESCRIPTION") = menu_list(i)("MenuDescription")
                dr("PATH") = menu_list(i)("Path")
                dt.Rows.Add(dr)
            Next
            Dim dv As New DataView(dt)
            CollectionListGrid.DataSource = dv
            CollectionListGrid.DataBind()
        End If
    End Sub
    Private Sub ViewAllMenuToolBar(ByVal searchstring As String)
        Dim canAddMenu As Boolean = False
        siteObj = AppUI.EkSiteRef()
        cPerms = siteObj.GetPermissions(0, 0, "folder")
        If ((cPerms("Collections"))) Then
            canAddMenu = True
        Else
            canAddMenu = IsCollectionMenuRoleMember()
        End If
        If (searchstring = "") Then
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(MsgHelper.GetMessage("view all menu title"))
        Else
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(MsgHelper.GetMessage("search menu title"))
        End If
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        If (canAddMenu) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", _
                    "collections.aspx?action=AddMenu&folderid=0&LangType=" & ContentLanguage & "&bPage=ViewMenuReport" & "&back=" & Server.UrlEncode("collections.aspx?action=ViewMenuReport"), _
                    MsgHelper.GetMessage("alt add new menu"), MsgHelper.GetMessage("btn add"), ""))
        End If
        If (EnableMultilingual = "1") Then
            If (canAddMenu) Then
                result.Append(objStyle.GetExportTranslationButton("content.aspx?type=menu&id=0&LangType=" & ContentLanguage & "&action=Localize&callbackpage=Collections.aspx&parm1=action&value1=ViewMenuReport", MsgHelper.GetMessage("alt Click here to export all menus for translation"), MsgHelper.GetMessage("lbl Export For Translation")))
            End If
            result.Append("<td>&#160;|&#160;</td>")
            result.Append("<td class=""label"">")
            result.Append(MsgHelper.GetMessage("view language"))
            result.Append("</td>")
            result.Append("<td>")
            result.Append(m_refStyle.ShowAllActiveLanguage(True, "", "javascript:LoadLanguage(this.value);", ContentLanguage) & "&nbsp;<br>")
            result.Append("</td>")
        End If
        result.Append("<td class=""label"">&#160;")
        result.Append("<label for=""txtSearch"">" & MsgHelper.GetMessage("generic search") & "</label>")
        result.Append("</td>")
        result.Append("<td>")
        result.Append("<input type=text class=""ektronTextMedium"" id=txtSearch name=txtSearch value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event)"">")
        result.Append("</td>")
        result.Append("<td>")
        result.Append("<input type=button value=" & MsgHelper.GetMessage("btn Search") & " id=btnSearch name=btnSearch class=""ektronWorkareaSearch"" onclick=""searchcollection();""></td>")
        result.Append("<td>")
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("ViewMenuReport"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Private Sub LoadCollectionList()
        Dim req As New PageRequestData
        req.PageSize = m_refContentApi.RequestInformationRef.PagingSize
        req.CurrentPage = _currentPageNumber
        m_strKeyWords = Request.Form("txtSearch")
        If (m_strKeyWords Is Nothing) Then
            m_strKeyWords = ""
        End If
        Dim collection_list As CollectionListData() = m_refApi.EkContentRef.GetCollectionList(m_strKeyWords, req)
        TotalPagesNumber = req.TotalPages
        PageSettings()
        ViewAllCollectionToolBar(m_strKeyWords)
        If (collection_list IsNot Nothing AndAlso collection_list.Length > 0) Then
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("TITLE", MsgHelper.GetMessage("generic Title"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), False, False))
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("ID", MsgHelper.GetMessage("generic ID"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("DESCRIPTION", MsgHelper.GetMessage("generic Description"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(40), Unit.Percentage(40), False, False))
            CollectionListGrid.Columns.Add(m_refStyle.CreateBoundField("PATH", MsgHelper.GetMessage("generic Path"), "", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(25), Unit.Percentage(25), False, False))
            Dim dt As New DataTable
            Dim dr As DataRow
            dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
            dt.Columns.Add(New DataColumn("ID", GetType(String)))
            dt.Columns.Add(New DataColumn("DESCRIPTION", GetType(String)))
            dt.Columns.Add(New DataColumn("PATH", GetType(String)))

            For i As Integer = 0 To collection_list.Length - 1
                Dim vAction As String = ""
                If (collection_list(i).ApprovalRequired And collection_list(i).Status <> "A") Then
                    vAction = "&Action=ViewStage"
                Else
                    vAction = "&Action=View"
                End If
                dr = dt.NewRow
                dr("TITLE") = "<a href=""collections.aspx?folderid=" & collection_list(i).FolderId & vAction & "&nid=" & collection_list(i).Id & "&bpage=reports"" alt='" & MsgHelper.GetMessage("generic View") & " """ & Replace(collection_list(i).Title, "'", "`") & """' title='" & MsgHelper.GetMessage("generic View") & " """ & Replace(collection_list(i).Title, "'", "`") & """'>" & collection_list(i).Title & "</a>"
                dr("ID") = collection_list(i).Id
                dr("DESCRIPTION") = collection_list(i).Description
                dr("PATH") = collection_list(i).FolderPath
                dt.Rows.Add(dr)
            Next
            Dim dv As New DataView(dt)
            CollectionListGrid.DataSource = dv
            CollectionListGrid.DataBind()
        End If
    End Sub
    Private Sub ViewAllCollectionToolBar(ByVal searchstring As String)
        Dim canIAddCol As Boolean = False
        siteObj = AppUI.EkSiteRef()
        cPerms = siteObj.GetPermissions(0, 0, "folder")
        If ((cPerms("Collections"))) Then
            canIAddCol = True
        Else
            canIAddCol = IsCollectionMenuRoleMember()
        End If
        If (searchstring = "") Then
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(MsgHelper.GetMessage("view all collections title"))
        Else
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(MsgHelper.GetMessage("search collections title"))
        End If
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        If (canIAddCol) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", _
                        "collections.aspx?action=Add&folderid=0&LangType=" & ContentLanguage & "&back=" & Server.UrlEncode("collections.aspx?action=ViewCollectionReport"), _
                        MsgHelper.GetMessage("alt: add new collection text"), MsgHelper.GetMessage("btn add"), ""))
        End If
        result.Append("<td class=""label"">&nbsp;|&nbsp;")
        result.Append("<label for=""txtSearch"">" & MsgHelper.GetMessage("generic search") & "</label>")
        result.Append("</td>")
        result.Append("<td>")
        result.Append("<input type=text class=""ektronTextMedium"" id=txtSearch name=txtSearch value=""" & m_strKeyWords & """ onkeydown=""CheckForReturn(event)"" autocomplete=""off"">")
        result.Append("</td>")
        result.Append("<td><input type=button value=" & MsgHelper.GetMessage("btn Search") & " id=btnSearch name=btnSearch class=""ektronWorkareaSearch"" onclick=""searchcollection();""></td>")
        result.Append("</td>")
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("ViewCollectionReport"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Private Sub PageSettings()
        If (TotalPagesNumber <= 1) Then
            VisiblePageControls(False)
        Else
            VisiblePageControls(True)
            cTotalPages.Text = (System.Math.Ceiling(TotalPagesNumber)).ToString()
            cCurrentPage.Text = _currentPageNumber.ToString()
            cPreviousPage.Enabled = True
            cFirstPage.Enabled = True
            cNextPage.Enabled = True
            cLastPage.Enabled = True
            If _currentPageNumber = 1 Then
                cPreviousPage.Enabled = False
                cFirstPage.Enabled = False
            ElseIf _currentPageNumber = TotalPagesNumber Then
                cNextPage.Enabled = False
                cLastPage.Enabled = False
            End If
        End If
    End Sub
    Private Sub VisiblePageControls(ByVal flag As Boolean)
        cTotalPages.Visible = flag
        cCurrentPage.Visible = flag
        cPreviousPage.Visible = flag
        cNextPage.Visible = flag
        cLastPage.Visible = flag
        cFirstPage.Visible = flag
        cPageLabel.Visible = flag
        cOfLabel.Visible = flag
    End Sub
    Sub CollectionNavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                _currentPageNumber = 1
            Case "Last"
                _currentPageNumber = Int32.Parse(cTotalPages.Text)
            Case "Next"
                _currentPageNumber = Int32.Parse(cCurrentPage.Text) + 1
            Case "Prev"
                _currentPageNumber = Int32.Parse(cCurrentPage.Text) - 1
        End Select
        If (Request.QueryString("action") = "ViewCollectionReport") Then
            LoadCollectionList()
        Else
            LoadMenuList()
        End If
        isCPostData.Value = "true"
    End Sub
    Private Sub FillContentFolderInfo()
        Try
            Dim bCheckout As Boolean = False
            NextPage.Attributes.Add("onclick", "return resetPostback()")
            lnkBtnPreviousPage.Attributes.Add("onclick", "return resetPostback()")
            FirstPage.Attributes.Add("onclick", "return resetPostback()")
            LastPage.Attributes.Add("onclick", "return resetPostback()")
            m_refMsg = m_refContentApi.EkMsgRef
            'Put user code to initialize the page here
            AppImgPath = m_refContentApi.AppImgPath
            AppName = m_refContentApi.AppName
            ContentIcon = "<img src=""" & AppPath & "images/UI/Icons/contentHtml.png"" alt=""Content"" />"  '-HC-
            WebEventIcon = "<img src=""" & AppPath & "images/UI/Icons/calendarViewDay.png"" alt=""WebEvent"" />"
            formsIcon = "<img src=""" & AppPath & "images/UI/Icons/contentForm.png"" alt=""Form"" />"  '-HC-
            pageIcon = "<img src=""" & AppPath & "images/UI/Icons/layout.png"" alt=""Page"" />" '-HC-
            'intQString = Request.QueryString
            AddType = LCase(Request.QueryString("addto"))
            nId = Request.QueryString("nid")
            subfolderid = Request.QueryString("subfolderid")
            FolderId = Request.QueryString("folderid")
            Dim showQDContentOnly As Boolean = (Request.QueryString("qdo") = "1")

            m_refContent = m_refContentApi.EkContentRef

            If (Not IsNothing(Request.QueryString("subfolderid"))) Then
                locID = subfolderid
            Else
                locID = FolderId
            End If
            gtNavs = m_refContent.GetFolderInfoWithPath(locID)
            If (Not Ektron.Cms.Common.EkFunctions.DoesKeyExist(gtNavs, "FolderName")) Then
                ' invalid folder, so start at root instead of taking an error
                locID = 0
                gtNavs = m_refContent.GetFolderInfoWithPath(0)
            End If

            'Set content type
            If Request.QueryString(ContentTypeUrlParam) <> "" Then
                If IsNumeric(Request.QueryString(ContentTypeUrlParam)) Then
                    g_ContentTypeSelected = CLng(Request.QueryString(ContentTypeUrlParam))
                    m_refContentApi.SetCookieValue(ContentTypeUrlParam, g_ContentTypeSelected)
                End If
            ElseIf Request.Cookies("ecm")(ContentTypeUrlParam) <> "" Then
                If IsNumeric(Request.Cookies("ecm")(ContentTypeUrlParam)) Then
                    g_ContentTypeSelected = CLng(Request.Cookies("ecm")(ContentTypeUrlParam))
                End If
            End If

            '/end set content type

            If (AddType = "menu") Then
                If (m_refContentApi.TreeModel = 1) Then
                    cRecursive = m_refContent.GetMenuByID(nId, 0, False)
                Else
                    cRecursive = m_refContent.GetMenuByID(nId, 0)
                End If
                If (cRecursive.Count) Then
                    MenuTitle = cRecursive("MenuTitle")
                    rec = cRecursive("Recursive")
                End If
            Else
                If (Request.QueryString("checkout") IsNot Nothing) Then
                    bCheckout = Request.QueryString("checkout").ToString()
                End If
                If (bCheckout) Then
                    cRecursive = m_refContent.GetEcmStageCollectionByID(nId, 0, 0, ErrorString, True, False, True)
                Else
                    cRecursive = m_refContent.GetEcmCollectionByID(nId, 0, 0, ErrorString, True, False, True)
                End If
                If (cRecursive.Count) Then
                    CollectionTitle = cRecursive("CollectionTitle")
                    rec = cRecursive("Recursive")
                End If

            End If
            perm_data = m_refContentApi.LoadPermissions(locID, "folder")
            If Not perm_data.CanTraverseFolders Then rec = False
            If (rec) Then
                cTmp = New Collection
                cTmp.Add(CLng(locID), "ParentID")
                cTmp.Add("name", "OrderBy")
                cFolders = m_refContent.GetAllViewableChildFoldersv2_0(cTmp)
            End If

            FolderName = gtNavs("FolderName")
            backId = gtNavs("ParentID")
            fPath = gtNavs("Path")

            cTmp = New Collection
            cTmp.Add("name", "OrderBy")
            cTmp.Add(FolderId, "FolderID")
            cTmp.Add(FolderId, "ParentID")
            If (AddType = "menu") Then
                cConts = m_refContent.GetAllContentNotInEcmMenu(nId, CLng(locID), "title", _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, TotalPagesNumber, g_ContentTypeSelected)
            Else
                If (bCheckout Or (Request.QueryString("status") IsNot Nothing AndAlso Request.QueryString("status").ToUpper() = "O")) Then
                    cConts = m_refContent.GetAllContentNotInEcmCollection(nId, CLng(locID), "title", _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, TotalPagesNumber, g_ContentTypeSelected, Ektron.Cms.Content.EkContent.ContentResultType.Staged)
                Else
                    ' Defect#: 49013
                    ' cConts = m_refContent.GetAllContentNotInEcmCollection(nId, CLng(locID), "title", _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, TotalPagesNumber, g_ContentTypeSelected)
                    cConts = GetAllContent(nId, CLng(locID), "title", _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, TotalPagesNumber, g_ContentTypeSelected)
                End If
            End If

            If (showQDContentOnly And (gtNavs("ReplicationMethod") <> "1") And m_refContentApi.RequestInformationRef.EnableReplication) Then
                ' only display QD content, clean out the content list
                cConts.Clear()
            End If
            gtNavs = Nothing

            If (TotalPagesNumber <= 1) Then
                TotalPages.Visible = False
                CurrentPage.Visible = False
                lnkBtnPreviousPage.Visible = False
                NextPage.Visible = False
                LastPage.Visible = False
                FirstPage.Visible = False
                PageLabel.Visible = False
                OfLabel.Visible = False
            Else

                TotalPages.Visible = True
                CurrentPage.Visible = True
                lnkBtnPreviousPage.Visible = True
                NextPage.Visible = True
                LastPage.Visible = True
                FirstPage.Visible = True
                PageLabel.Visible = True
                OfLabel.Visible = True
                TotalPages.Text = (System.Math.Ceiling(TotalPagesNumber)).ToString()

                CurrentPage.Text = _currentPageNumber.ToString()

                If _currentPageNumber = 1 Then

                    lnkBtnPreviousPage.Enabled = False

                    If TotalPagesNumber > 1 Then
                        NextPage.Enabled = True
                    Else
                        NextPage.Enabled = False
                    End If

                Else

                    lnkBtnPreviousPage.Enabled = True

                    If _currentPageNumber = TotalPagesNumber Then
                        NextPage.Enabled = False
                    Else
                        NextPage.Enabled = True
                    End If

                End If
            End If

            Dim templatelist As TemplateData() = m_refContentApi.GetEnabledTemplatesByFolder(locID)
            Dim hasNormalTemplate As Boolean = False
            Dim template As TemplateData

            For Each template In templatelist
                If (template.Type = EkEnumeration.TemplateType.Default AndAlso template.SubType = EkEnumeration.TemplateSubType.Default) Then
                    hasNormalTemplate = True
                    Exit For
                End If
            Next
            CanCreateContent = perm_data.CanAdd AndAlso hasNormalTemplate

            asset_data = m_refContent.GetAssetSuperTypes()

            If (CMSContentType_Content = g_ContentTypeSelected Or CMSContentType_Archive_Content = g_ContentTypeSelected) Then
                lContentType = CLng(g_ContentTypeSelected)
            ElseIf (ManagedAsset_Min <= g_ContentTypeSelected And g_ContentTypeSelected <= ManagedAsset_Max) Then
                If DoesAssetSupertypeExist(asset_data, g_ContentTypeSelected) Then
                    lContentType = CLng(g_ContentTypeSelected)
                End If
            End If

            PopulateGridData(rec)
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try

    End Sub

    Private Sub PopulateGridData(ByVal ShowFolders As Boolean)
        Dim showQDContentOnly As Boolean = (Request.QueryString("qdo") = "1")
        Dim enableQDOparam As String = ""
        If (showQDContentOnly) Then
            enableQDOparam = "&qdo=1"
        End If
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim bDoNotShow As Boolean = False
        colBound.DataField = "ITEM1"
        colBound.HeaderText = ""
        colBound.ItemStyle.CssClass = "info"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Height = Unit.Percentage(0)
        ContentGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ITEM2"
        colBound.HeaderText = ""
        colBound.ItemStyle.CssClass = "info"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Height = Unit.Percentage(0)
        ContentGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ITEM3"
        colBound.HeaderText = ""
        colBound.ItemStyle.CssClass = "info"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Height = Unit.Percentage(0)
        ContentGrid.Columns.Add(colBound)


        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("ITEM1", GetType(String)))
        dt.Columns.Add(New DataColumn("ITEM2", GetType(String)))
        dt.Columns.Add(New DataColumn("ITEM3", GetType(String)))

        Dim i As Integer = 0
        dr = dt.NewRow
        If (ShowFolders) Then
            dr(0) = MsgHelper.GetMessage("alt Please select content by navigating the folders below") & "<br>"
        Else
            dr(0) = ""
        End If
        dr(1) = "remove-item"
        dr(2) = "remove-item"
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = "Path: <span class=""pathInfo"">" & fPath & "</span>"
        dr(1) = "remove-item"
        dr(2) = "remove-item"
        dt.Rows.Add(dr)

        dr = dt.NewRow
        If (locID <> 0 And rec) Then
            Dim linkMarkup As String = ""
            linkMarkup = "<a href=""collections.aspx?Action=AddLink" & status & "&LangType=" & ContentLanguage & "&nId=" & nId & "&folderid=" & FolderId & "&subfolderid=" & (backId) & enableQDOparam
            If (Request.QueryString("back") <> "") Then
                linkMarkup = linkMarkup & "&back=" & Server.UrlEncode(Request.QueryString("back"))
            End If
            If (Not Request.QueryString("noworkarea") Is Nothing) Then
                linkMarkup = linkMarkup & "&noworkarea=" & Request.QueryString("noworkarea")
            End If
            If (AddType = "menu") Then
                linkMarkup = linkMarkup & "&addto=" & (AddType)
                linkMarkup = linkMarkup & "&iframe=" & Request.QueryString("iframe") & NoWorkAreaAttribute
            End If
            linkMarkup = linkMarkup & "&title=""" & m_refMsg.GetMessage("alt: generic previous dir text") & """>"
            m_refContent = m_refContentApi.EkContentRef
            'Users might not have collection rights with the parent folders # Defect: 47566
            If AddType <> "menu" AndAlso (m_refContent.IsAllowed(backId, 0, "Folder", "Collections")) Then
                dr(0) = linkMarkup & "<img src=""" & AppPath & "images/UI/Icons/folderUp.png" & """ border=""0"" title=""" & m_refMsg.GetMessage("alt: generic previous dir text") & """ alt=""" & m_refMsg.GetMessage("alt: generic previous dir text") & """  /></a>" & linkMarkup & "..</a>"
            ElseIf (AddType = "menu") Then
                dr(0) = linkMarkup & "<img src=""" & AppPath & "images/UI/Icons/folderUp.png" & """ border=""0"" title=""" & m_refMsg.GetMessage("alt: generic previous dir text") & """ alt=""" & m_refMsg.GetMessage("alt: generic previous dir text") & """  /></a>" & linkMarkup & "..</a>"
            End If
        End If
        dr(1) = "&nbsp;"
        dr(2) = "&nbsp;"
        dt.Rows.Add(dr)

        Dim folder As Object
        If Not IsNothing(cFolders) Then
            For Each folder In cFolders
                If ((EkEnumeration.FolderType.Content = folder("FolderType")) _
                 OrElse (EkEnumeration.FolderType.Blog = folder("FolderType")) _
                 OrElse (EkEnumeration.FolderType.Domain = folder("FolderType")) _
                 OrElse (EkEnumeration.FolderType.Catalog = folder("FolderType")) _
                 OrElse (EkEnumeration.FolderType.Calendar = folder("FolderType")) _
                 OrElse (EkEnumeration.FolderType.Community = folder("FolderType"))) Then
                    dr = dt.NewRow

                    dr(0) += "<a href=""collections.aspx?Action=AddLink" & status & "&LangType=" & ContentLanguage & "&nId=" & _
                     nId & "&folderid=" & FolderId & "&subfolderid=" & folder("ID") & NoWorkAreaAttribute & enableQDOparam
                    If (AddType = "menu") Then
                        dr(0) += "&addto=" + AddType + "&iframe=" + Request.QueryString("iframe")
                    End If
                    If (Request.QueryString("back") <> "") Then
                        dr(0) = dr(0) & "&back=" & Server.UrlEncode(Request.QueryString("back"))
                    End If

                    Dim linkValue As String = dr(0)
                    Dim folder_img As String = ""

                    Select Case Convert.ToInt32(folder("FolderType"))
                        Case EkEnumeration.FolderType.Domain
                            folder_img = AppPath & "images/UI/Icons/folderSite.png"
                        Case EkEnumeration.FolderType.Community
                            folder_img = AppPath & "images/UI/Icons/folderCommunity.png"
                        Case EkEnumeration.FolderType.Catalog
                            folder_img = AppPath & "images/UI/Icons/folderGreen.png"
                        Case EkEnumeration.FolderType.Calendar
                            folder_img = AppPath & "images/UI/Icons/folderCalendar.png"
                        Case Else
                            folder_img = AppPath & "images/UI/Icons/folder.png"
                    End Select

                    dr(0) += "&title=""" + m_refMsg.GetMessage("alt: generic view folder content text") & """><img src=""" & folder_img & """ title=""" & m_refMsg.GetMessage("alt: generic view folder content text") & """ alt=""" & m_refMsg.GetMessage("alt: generic view folder content text") & """  /></a>" & linkValue & "&title=""" + m_refMsg.GetMessage("alt: generic view folder content text") & """>" & folder("Name") & "</a>"
                    If (m_refContentApi.RequestInformationRef.EnableReplication And (folder("ReplicationMethod") = 1)) Then
                        dr(0) += "&nbsp; (QuickDeploy)"
                    End If
                    dr(1) = "&nbsp;"
                    dr(2) = "&nbsp;"
                    dt.Rows.Add(dr)
                End If
            Next
        End If

        Dim ContentName As String
        Dim lLoop As Integer = 0
        Dim cLinkArray As String = ""
        Dim fLinkArray As String = ""
        Dim cLanguagesArray As String = ""
        Dim IsAdded As Boolean = False
        ' For displaying child content - exclude this for reports
        For Each contInfo As ContentBase In cConts
            If ((CMSContentType_AllTypes = g_ContentTypeSelected) OrElse (contInfo.ContentType = g_ContentTypeSelected)) Then
                bDoNotShow = False
                IsAdded = IsContentInEcmCollection(contInfo.Id)
                If contInfo.ContentType = CMSContentType.Forms Then
                    ContentName = AppPath & "linkit.aspx?LinkIdentifier=ekfrm&ItemID=" & contInfo.Id 'm_refContent.GetContentFormlink(contInfo.Id, contInfo.FolderId)
                ElseIf (((contInfo.ContentType >= ManagedAsset_Min And contInfo.ContentType <= ManagedAsset_Max) Or (contInfo.ContentType = CMSContentType.Assets)) And (contInfo.ContentType <> CMSContentType.Multimedia)) Then
                    ContentName = AppPath & "linkit.aspx?LinkIdentifier=ID&ItemID=" & contInfo.Id 'm_refContent.GetContentQlink(contInfo.Id, contInfo.FolderId)
                    If InStr(ContentName.ToLower, "javascript") > 0 Then
                        ContentName = ""
                    End If
                ElseIf (contInfo.ContentType = CMSContentType.Content) Or (contInfo.ContentType = CMSContentType.Content) Or (contInfo.ContentType = CMSContentType.Multimedia) Or (contInfo.ContentType = CMSContentType.CatalogEntry) Then
                    ContentName = AppPath & "linkit.aspx?LinkIdentifier=ID&ItemID=" & contInfo.Id 'm_refContent.GetContentQlink(contInfo.Id, contInfo.FolderId)
                Else
                    'do not show
                    bDoNotShow = True
                End If
                If (contInfo.ContentSubType = CMSContentSubtype.PageBuilderMasterData) Then
                    bDoNotShow = True
                End If
                If Not (bDoNotShow) Then
                    dr = dt.NewRow

                    If IsAdded Then
                        dr(0) = "&nbsp;&nbsp;" & getContentTypeIcon(contInfo) & "<input size= " & contInfo.Id & " type=""hidden"" name=""frm_hidden" & lLoop & """ value=""0"">" & _
                      "<input type=""checkbox"" disabled=""disabled"" checked=""checked"" name=""content"" value=""" & contInfo.Id & """ ID=""content"">" & contInfo.Title & _
                      "<input  type=""hidden"" name=""frm_languages" & lLoop & """ value=""" & contInfo.Language & """>"
                    Else
                        dr(0) = "&nbsp;&nbsp;" & getContentTypeIcon(contInfo) & "<input size= " & contInfo.Id & " type=""hidden"" name=""frm_hidden" & lLoop & """ value=""0"">" & _
                      "<input type=""checkbox"" name=""content"" value=""" & contInfo.Id & """ ID=""content"" onclick=""document.forms[0]['frm_hidden" & (lLoop) & "'].value=(this.checked?" & contInfo.Id & " : 0);"">" & contInfo.Title & _
                      "<input  type=""hidden"" name=""frm_languages" & lLoop & """ value=""" & contInfo.Language & """>"
                    End If
                    dr(1) = "remove-item"
                    dr(2) = "remove-item"
                    dt.Rows.Add(dr)
                End If
                cLinkArray = cLinkArray & "," & contInfo.Id
                cLanguagesArray = cLanguagesArray & "," & contInfo.Language
                fLinkArray = fLinkArray & "," & FolderId
                lLoop = lLoop + 1
            End If
        Next
        If (Len(cLinkArray)) Then
            cLinkArray = Right(cLinkArray, Len(cLinkArray) - 1)
            fLinkArray = Right(fLinkArray, Len(fLinkArray) - 1)
            cLanguagesArray = Right(cLanguagesArray, Len(cLanguagesArray) - 1)
        End If
        Dim dv As New DataView(dt)
        ContentGrid.DataSource = dv
        ContentGrid.DataBind()
    End Sub
    Protected Sub Grid_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        Select Case e.Item.ItemType
            Case ListItemType.AlternatingItem, ListItemType.Item
                If (e.Item.Cells(1).Text.Equals("remove-item") And e.Item.Cells(2).Text.Equals("remove-item")) Then
                    e.Item.Cells(0).ColumnSpan = 3
                    e.Item.Cells.RemoveAt(2)
                    e.Item.Cells.RemoveAt(1)
                End If
        End Select

    End Sub

    Function getContentTypeIcon(ByRef objCont As Object) As String
        Try
            Dim ContentTypeID As Integer
            Dim strAssetIcon As String
            Dim contentObj As Ektron.Cms.Common.ContentBase

            contentObj = CType(objCont, Ektron.Cms.Common.ContentBase)

            ContentTypeID = objCont.ContentType
            If ContentTypeID = 2 Then
                Return (formsIcon)
            ElseIf ContentTypeID > ManagedAsset_Min And ContentTypeID < ManagedAsset_Max Then
                Try
                    strAssetIcon = objCont.AssetInfo.ImageUrl
                    strAssetIcon = "<img src=""" & strAssetIcon & """ alt=""Asset"">"
                    Return (strAssetIcon)
                Catch ex As Exception
                    Return ("<img src=""" & AppPath & "images/ui/icons/filetypes/file.png"" alt=""Content"" />")
                End Try
            ElseIf ContentTypeID = 3333 Then
                Dim catalogEntry As Ektron.Cms.Commerce.CatalogEntry = New Ektron.Cms.Commerce.CatalogEntry()
                catalogEntry.RequestInformation = m_refContent.RequestInformation
                Dim catalogEntryData As Ektron.Cms.Commerce.EntryData

                catalogEntryData = catalogEntry.GetItem(contentObj.Id, Long.Parse(contentObj.Language.ToString()))
                Return ("<img title=""" & catalogEntryData.Title & """ src=""" & GetProductIcon(catalogEntryData.EntryType) & """ alt=""" & catalogEntryData.Title & """ />")
            Else
                If (objCont.ContentSubType = 1) Then
                    Return (pageIcon)
                ElseIf (objCont.ContentSubType = 2) Then
                    Return (WebEventIcon)
                End If
                Return (ContentIcon)
            End If
        Catch ex As Exception
            Return (ContentIcon)
        End Try
    End Function

    Protected Function GetProductIcon(ByVal entryType As EkEnumeration.CatalogEntryType) As String
        Dim productImage As String
        Select Case entryType
            Case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle
                productImage = m_refContentApi.ApplicationPath & "images/ui/icons/package.png"
            Case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct
                productImage = m_refContentApi.ApplicationPath & "images/ui/icons/bricks.png"
            Case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit
                productImage = m_refContentApi.ApplicationPath & "images/ui/icons/box.png"
            Case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct
                productImage = m_refContentApi.ApplicationPath & "images/ui/icons/bookGreen.png"
            Case Else
                productImage = m_refContentApi.ApplicationPath & "images/ui/icons/brick.png"
        End Select

        Return productImage
    End Function

    Function MakeStringJSSafe(ByVal str As String) As String
        Return (str.Replace("'", "\'"))
    End Function

    Function DoesAssetSupertypeExist(ByVal asset_data As AssetInfoData(), ByVal lContentType As Integer) As Boolean
        Dim i As Integer = 0
        Dim result As Boolean = False
        If (Not (IsNothing(asset_data))) Then
            For i = 0 To asset_data.Length - 1
                If (ManagedAsset_Min <= asset_data(i).TypeId And asset_data(i).TypeId <= ManagedAsset_Max) Then
                    If (asset_data(i).TypeId = lContentType) Then
                        result = True
                        Exit For
                    End If
                End If
            Next
        End If
        Return (result)
    End Function

    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                _currentPageNumber = 1
            Case "Last"
                _currentPageNumber = Int32.Parse(TotalPages.Text)
            Case "Next"
                _currentPageNumber = Int32.Parse(CurrentPage.Text) + 1
            Case "Prev"
                _currentPageNumber = Int32.Parse(CurrentPage.Text) - 1
        End Select
        FillContentFolderInfo()
    End Sub
    Protected Function IsCollectionMenuRoleMember() As Boolean
        Return m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)
    End Function
    Protected Function IsContentInEcmCollection(ByVal ContentID As Long) As Boolean
        Dim cTmp As Collection
        If CollectionContentItems IsNot Nothing Then
            For Each cTmp In CollectionContentItems
                If (cTmp("ContentID") = ContentID) Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function
    Protected Function GetAllContent(ByVal nID As Long, ByVal RequestedFolderID As Long, ByVal OrderBy As String, ByVal currentPage As Integer, ByVal pageSize As Integer, ByRef totalPages As Integer, ByVal ContentType As CMSContentType) As EkContentCol
        Dim cRet As Collection
        Dim cCollection As New Collection
        Dim cTmp As New Collection
        Dim cTmps As New Collection
        Dim cAllViewable As New EkContentCol
        ' Dim lLoop As Integer
        Dim bIsAllowed As Boolean = False
        Dim api As New Ektron.Cms.CommonApi
        Dim GetContents As Object = ""

        Try
            GetAllContent = Nothing

            bIsAllowed = m_refContent.IsAllowed(CLng(RequestedFolderID), 0, "Folder", "Collections") _
             OrElse m_refContent.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu, api.RequestInformationRef.CallerId, False)
            If (Not bIsAllowed) Then
                Throw New Exception(m_refContentApi.EkMsgRef.GetMessage("com: user does not have permission"))
            End If


            cCollection = m_refContent.pGetEcmCollectionByID(nID, False, False, "", 0, GetContents, True, True)
            If (cCollection.Count = 0) Then
                Throw New Exception(m_refContentApi.EkMsgRef.GetMessage("com: collection not found"))
            End If

            cTmp.Add(CLng(RequestedFolderID), "FolderID")
            cTmp.Add(OrderBy, "OrderBy")
            cTmp.Add(1, "FilterContentAssetType")
            If (ContentType <> Ektron.Cms.Common.EkEnumeration.CMSContentType.AllTypes) Then
                cTmp.Add(ContentType, "ContentType")
            End If
            cAllViewable = m_refContent.GetAllViewableChildInfov5_0(cTmp, currentPage, pageSize, totalPages, ContentType)
            CollectionContentItems = cCollection("Contents")
            GetAllContent = cAllViewable
        Catch ex As Exception
            Return Nothing
        Finally
            cRet = Nothing
            cCollection = Nothing
            cTmp = Nothing
            cTmps = Nothing
            cAllViewable = Nothing
        End Try

    End Function

    Protected Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJsonJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronDmsMenuJS)

        Ektron.Cms.API.JS.RegisterJS(Me, "java/cmsmenuapi.js", "CmsMenuApiJS")
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronInputLabelJS)
        Ektron.Cms.API.JS.RegisterJS(Me, "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareaSearchInputLabelInitJS")
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)

        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, "csslib/ektron.widgets.selector.css", "EktronWidgetsSelectorCss")
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronDmsMenuCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronDmsMenuIE6Css, API.Css.BrowserTarget.LessThanEqualToIE6)
    End Sub
End Class


