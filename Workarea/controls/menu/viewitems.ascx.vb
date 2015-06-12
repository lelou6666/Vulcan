Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports System.DateTime
Imports System.Collections.Generic
Imports System.IO

Partial Class viewmenuitems
    Inherits System.Web.UI.UserControl

    Protected m_refCommon As New CommonApi
    Protected m_refstyle As New StyleHelper
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected m_refMsg As EkMessageHelper
    Protected m_strPageAction As String = ""
    Protected m_refContent As Content.EkContent
    Protected m_refContentApi As ContentAPI
    Protected MenuId As Long = 0
    Protected MenuLanguage As Integer = -1
    Protected language_data As LanguageData
    Protected menu_item_data As List(Of AxMenuItemData)
    Protected ParentId As Long = 0
    Protected m_AncestorMenuId As Long = 0
    Protected m_strViewItem As String = "item"
    Protected AddDeleteIcon As Boolean = False
    Protected MenuItemCount As Long = 0
    Protected m_strMenuName As String = ""
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected m_strDelConfirm As String = ""
    Protected m_strDelItemsConfirm As String = ""
    Protected m_strSelDelWarning As String = ""
    Protected objLocalizationApi As New LocalizationAPI()
    Protected m_strBackPage As String = ""      ' URL to use to return to the current menu page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = m_refCommon.EkMsgRef
        AppImgPath = m_refCommon.AppImgPath
        AppPath = m_refCommon.AppPath
        m_strPageAction = Request.QueryString("action")
        Utilities.SetLanguage(m_refCommon)
        MenuLanguage = m_refCommon.ContentLanguage
        MenuId = Convert.ToInt64(Request.QueryString("menuid"))
        If (Request.QueryString("view") IsNot Nothing) Then
            m_strViewItem = Request.QueryString("view")
        End If
        m_refContent = m_refCommon.EkContentRef
        m_refContentApi = New ContentAPI()
        Utilities.SetLanguage(m_refContentApi)

        m_strBackPage = Request.QueryString.ToString()
        ' strip off refresh indicator
        If (m_strBackPage.EndsWith("&rf=1")) Then
            ' refresh is needed after we edit a submenu, but we don't want to keep refreshing if we use the same URL
            m_strBackPage = m_strBackPage.Substring(0, m_strBackPage.Length - 5)
        End If

        If (IsPostBack = False) Then
            DisplayPage()
            RegisterResources()
        Else
            If (Request.Form(submittedaction.Name) = "deleteitem") Then
                ' handle deleting menu items
                Dim contentids As String = Request.Form(frm_item_ids.Name)
                Server.Transfer("collectionaction.aspx?action=doDeleteMenuItem&folderid=" & MenuId & "&nid=" & MenuId & "&ids=" & contentids & "&back=" & Server.UrlEncode(Request.Url.ToString()), True)
            ElseIf (Request.Form(submittedaction.Name) = "delete") Then
                ' handle deleting the menu
                Dim menu As AxMenuData
                menu = m_refContent.GetMenuDataByID(MenuId)
                Server.Transfer("collectionaction.aspx?action=doDeleteMenu&nid=" & MenuId & "&back=" & Server.UrlEncode("menu.aspx?action=deleted&title=" & menu.Title), True)
            End If
        End If
        isPostData.Value = "true"
    End Sub

    Private Sub DisplayPage()
        'taxonomy_request.IncludeItems = True
        'taxonomy_request.PageSize = m_refCommon.RequestInformationRef.PagingSize
        'taxonomy_request.CurrentPage = m_intCurrentPage
        'taxonomy_data = m_refContent.ReadTaxonomy(taxonomy_request)
        'menu_item_data = m_refContent.GetMenuContent(MenuId, MenuLanguage)
        'Dim menu As Collection = m_refContent.GetMenuByID(MenuId, False)

        Dim menu As AxMenuData
        menu = m_refContentApi.EkContentRef.GetMenuDataByID(MenuId)

        If (menu IsNot Nothing) Then
            ParentId = menu.ParentID
            m_AncestorMenuId = menu.AncestorID
            If (ParentId = 0) Then
                ' this matches the legacy code but it doesn't make sense for a submenu's
                ' parent and grandparent to be the same ID to me...the grandparent should be root(0) :-P
                ParentId = menu.ID
            End If
            m_strMenuName = menu.Title  ' menu("MenuTitle")
            'm_intTotalPages = taxonomy_request.TotalPages
        End If

        If (Request.QueryString("rf") = "1") Then
            litRefreshAccordion.Text = "<script language=""javascript"">" & vbCrLf _
             + "top.refreshMenuAccordion(" & MenuLanguage & ");" & vbCrLf _
             + "</script>" & vbCrLf
        End If

        PopulateContentGridData(menu)
        MenuToolBar(menu)
    End Sub
    Private Sub PopulateContentGridData(ByVal menu As AxMenuData)
        If (m_strPageAction = "removeitems") Then
            MenuItemList.Columns.Add(m_refstyle.CreateBoundField("CHECK", "<input type=""Checkbox"" name=""checkall"" onclick=""checkAll('frm_content_ids',false);"">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(2), Unit.Percentage(2), False, False))
        End If
        MenuItemList.Columns.Add(m_refstyle.CreateBoundField("TITLE", m_refMsg.GetMessage("generic title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(50), False, False))
        MenuItemList.Columns.Add(m_refstyle.CreateBoundField("LANGUAGE", m_refMsg.GetMessage("generic language"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        MenuItemList.Columns.Add(m_refstyle.CreateBoundField("ID", m_refMsg.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        MenuItemList.Columns.Add(m_refstyle.CreateBoundField("URL", m_refMsg.GetMessage("generic url link"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(20), False, False))
        Dim dt As New DataTable
        Dim dr As DataRow
        If (m_strPageAction = "removeitems") Then
            dt.Columns.Add(New DataColumn("CHECK", GetType(String)))
        End If
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("URL", GetType(String)))
        'If (m_strViewItem <> "folder") Then
        PageSettings()
        'If (menu_item_data.Count > 0) Then
        If (menu.Item.Length > 0) Then
            AddDeleteIcon = True
            Dim icon As String = ""
            Dim title As String = ""
            Dim link As String = ""
            Dim backPage As String = Server.UrlEncode(Request.Url.ToString())
            'For Each item As AxMenuItemData In menu_item_data
            For Each item As AxMenuItemData In menu.Item
                If (item Is Nothing) Then
                    Continue For
                End If
                If ((item.ItemType = 4) And (m_strPageAction = "removeitems")) Then
                    ' submenus need to be deleted individually so they shouldn't show up in this list!
                    Continue For
                End If

                MenuItemCount = MenuItemCount + 1
                dr = dt.NewRow
                If (m_strPageAction = "removeitems") Then
                    dr("CHECK") = "<input type=""checkbox"" id=""frm_content_ids"" name=""frm_content_ids"" value=""" & item.ID & """ onclick=""checkAll('frm_content_ids',true);"">"
                End If

                'backPage = Server.UrlEncode("Action=viewcontent&view=item&menuid=" & MenuId)
                'link = "<a href='content.aspx?action=View&LangType=" & item.ContentLanguage & "&id=" & item.ID & "&callerpage=menu.aspx&origurl=" & backPage & "' title='" & title & "'>" & item.ItemTitle & "</a>"

                title = m_refMsg.GetMessage("generic View") & " """ & Replace(item.ItemTitle, " '", "`") & """"
                Dim editmenuitemurl As String
                editmenuitemurl = "collections.aspx?action=EditMenuItem&nid=" & MenuId & "&id=" & item.ID & "&Ty=" & item.ItemType & "&back=" & backPage
                link = "<a href='" & editmenuitemurl & " '>" & item.ItemTitle & "</a>"

                Dim iteminfo As Collection = Nothing

                Dim assetimageurl As String = ""
                If item.ItemType = 1 Then
                    If (item.ItemSubType = 8) Then
                        iteminfo = m_refContentApi.EkContentRef.GetMenuItemByID(item.ItemID, item.ID, False)
                        ' this is a DMS asset so we have to look up the icon for it because the menu api doesn't have this
                        Dim assetcontentdata As Ektron.Cms.ContentData = m_refContent.GetContentById(iteminfo("ItemID"))
                        Dim service As AssetManagement.AssetManagementService = New AssetManagement.AssetManagementService
                        Dim fileInfo As Ektron.ASM.AssetConfig.AssetFileInformation = service.GetFileInformation(assetcontentdata.AssetData.Version)
                        assetimageurl = fileInfo.ImageUrl
                        icon = "<img src=""" & fileInfo.ImageUrl & """ />&nbsp;"
                    Else
                        icon = "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/contentHtml.png" & """ />&nbsp;"
                    End If
                ElseIf item.ItemType = 2 Then
                    icon = "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/contentForm.png" & """ />&nbsp;"
                ElseIf item.ItemType = 4 Then
                    icon = "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/menu.png" & """ />&nbsp;"
                    link = "<a href='menu.aspx?Action=viewcontent&menuid=" & item.ID & "&treeViewId=-3" & "'>" & item.ItemTitle & "</a>"
                ElseIf item.ItemType = 5 Then
                    icon = "<img src=""" & m_refContentApi.AppPath & "images/ui/icons/link.png" & """ />&nbsp;"
                End If

                If ((item.ItemType = 1) Or (item.ItemType = 2)) Then
                    If (iteminfo Is Nothing) Then
                        iteminfo = m_refContentApi.EkContentRef.GetMenuItemByID(item.ItemID, item.ID, False)
                    End If

                    Dim itemtype As Integer = item.ItemType
                    If (itemtype = 2) Then
                        'For Library Items , ItemID key is a libraryId instead of ContentID and ItemType has to be passed.
                        ' this is contenttype 7 for the menu generator. 
                        itemtype = 7
                        dr("TITLE") = m_refContentApi.GetDmsContextMenuHTML(iteminfo("ContentID"), iteminfo("ContentLanguage"), _
                         itemtype, item.ItemContentSubType, item.ItemTitle, _
                          m_refMsg.GetMessage("edit menu items title") + " " + item.ItemTitle, editmenuitemurl, _
                          "", assetimageurl)
                    Else
                        dr("TITLE") = m_refContentApi.GetDmsContextMenuHTML(iteminfo("ItemID"), iteminfo("ContentLanguage"), _
                        iteminfo("ContentType"), item.ItemContentSubType, item.ItemTitle, _
                        m_refMsg.GetMessage("edit menu items title") + " " + item.ItemTitle, editmenuitemurl, _
                        "", assetimageurl)
                    End If
                    dr("URL") = iteminfo("ItemLink")
                Else
                    dr("TITLE") = icon & link
                    dr("URL") = item.ItemLink
                End If
                dr("ID") = item.ID
                dr("LANGUAGE") = item.ContentLanguage
                dt.Rows.Add(dr)
            Next
        End If
        Dim dv As New DataView(dt)
        MenuItemList.DataSource = dv
        MenuItemList.DataBind()
    End Sub

    Private Sub MenuToolBar(ByVal menu As AxMenuData)
        Dim strDeleteMsg As String = ""

        strDeleteMsg = m_refMsg.GetMessage("alt delete button text (menu)")
        m_strDelConfirm = m_refMsg.GetMessage("delete menu confirm")
        m_strDelItemsConfirm = m_refMsg.GetMessage("delete menu items confirm")
        m_strSelDelWarning = m_refMsg.GetMessage("select menu item missing warning")

        divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("view menu title") & " """ & m_strMenuName & """" & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & objLocalizationApi.GetFlagUrlByLanguageID(MenuLanguage) & "' />")
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        Dim backPage As String = Server.UrlEncode(Request.Url.ToString())
        If (m_strPageAction <> "removeitems") Then
            ' folder ID is 0 here to start the selection of content items at root!
            result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "collections.aspx?action=AddMenuItem&nid=" & MenuId & "&folderid=" & 0 & "&back=" & backPage _
                                                             & "&parentid=" & MenuId & "&ancestorid=" & m_AncestorMenuId, _
                                                             m_refMsg.GetMessage("add collection items"), m_refMsg.GetMessage("add collection items"), ""))
            If (MenuItemCount > 0) Then
                result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/remove.png", "menu.aspx?action=removeitems&menuid=" & MenuId & "&parentid=" & ParentId, m_refMsg.GetMessage("remove menu items"), m_refMsg.GetMessage("remove menu items"), ""))
            End If
            If (MenuItemCount > 1) Then
                result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/arrowUpDown.png", "collections.aspx?action=ReOrderMenuItems&nid=" & MenuId & "&folderid=" & ParentId & "&back=" & backPage _
                                                                 , m_refMsg.GetMessage("reorder menu title"), m_refMsg.GetMessage("alt: update menu order text"), ""))
            End If
            'result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "collections.aspx?action=EditMenu&nid=" & MenuId & "&folderid=" & ParentId & "&back=" & backPage _
            '                                                 , m_refMsg.GetMessage("edit menu title"), m_refMsg.GetMessage("edit menu title"), ""))
            result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/properties.png", "menu.aspx?action=viewmenu&menuid=" & MenuId & "&parentid=" & ParentId, m_refMsg.GetMessage("alt menu properties button text"), m_refMsg.GetMessage("properties text"), ""))
            result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "#", m_refMsg.GetMessage("generic delete title"), m_refMsg.GetMessage("alt delete menu"), "onclick=""return DeleteItem();"""))
        Else
            If (AddDeleteIcon) Then
                ' deletes checked/selected menu items
                divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("remove items from menu") & " """ & m_strMenuName & """" & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & objLocalizationApi.GetFlagUrlByLanguageID(MenuLanguage) & "' />")
                result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/remove.png", "#", m_refMsg.GetMessage("alt remove button text (taxonomyitems)"), m_refMsg.GetMessage("btn remove"), "onclick=""return DeleteItem('items');"""))
            End If
        End If

        If (m_strPageAction <> "viewcontent") Then
            result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "menu.aspx?action=viewcontent&view=item&menuid=" & MenuId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If

        Dim ParentMenuId As Long = menu.ParentID
        Dim FolderID As Long = menu.FolderID

        result.Append("<td>&nbsp;|&nbsp;</td>")

        result.Append("<td nowrap=""true"">")
        Dim backpagelang As String = Server.UrlDecode(backPage)
        If (Not backPage.Contains("LangType")) Then
            backpagelang = backpagelang & "&LangType=" & MenuLanguage
        End If
        Dim addDD As String
        If menu.ParentID = 0 Then
            addDD = ViewLangsForMenuID(MenuId, "", False, False, _
                                       "javascript:addBaseMenu(" & MenuId & ", " _
                                       & ParentMenuId & ", " & m_AncestorMenuId & ", " _
                                       & FolderID & ", this.value, '" & backpagelang & "');")
            If addDD <> "" Then
                addDD = "&nbsp;" & m_refMsg.GetMessage("generic add title") & ":&nbsp;" & addDD
            End If
            If (CStr(m_refContentApi.EnableMultilingual = "1")) Then
                result.Append("" & m_refMsg.GetMessage("generic view") & ":&nbsp;" & _
                              ViewLangsForMenuID(MenuId, "", True, False, _
                              "javascript:LoadLanguage(this.value);") & "&nbsp;" & addDD & "<br>")
            End If
        End If
        result.Append("</td>")

        result.Append("<td>" & m_refstyle.GetHelpButton("ViewMenu") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub

    Function ViewLangsForMenuID(ByVal fnMenuID As Long, ByVal fnBGColor As String, _
                                ByVal showTranslated As Boolean, ByVal showAllOpt As Boolean, _
                                ByVal onChangeEv As String) As String
        Dim TransCol As Collection
        Dim outDD As String
        Dim Col As Collection
        Dim frmName As String
        Dim ErrorString As String = ""

        Dim contObj As Ektron.Cms.Content.EkContent
        contObj = m_refContent

        If (CBool(showTranslated)) Then
            TransCol = contObj.GetTranslatedLangsForMenuID(fnMenuID, ErrorString)
            frmName = "frm_translated"
        Else
            TransCol = contObj.GetNonTranslatedLangsForMenuID(fnMenuID, ErrorString)
            frmName = "frm_nontranslated"
        End If

        outDD = "<select id=""" & frmName & """ name=""" & frmName & """ OnChange=""" & onChangeEv & """>" & vbCrLf

        If (CBool(showAllOpt)) Then
            If CStr(MenuLanguage) = "-1" Then
                outDD = outDD & "<option value=""-1"" selected>All</option>"
            Else
                outDD = outDD & "<option value=""-1"">All</option>"
            End If
        Else
            outDD = outDD & "<option value=""0"">-select language-</option>"
        End If

        If ((TransCol.Count > 0) And (m_refContentApi.EnableMultilingual = "1")) Then
            For Each Col In TransCol
                If CStr(MenuLanguage) = CStr(Col("LanguageID")) Then
                    outDD = outDD & "<option value=" & Col("LanguageID") & " selected>" & Col("LanguageName") & "</option>"
                Else
                    outDD = outDD & "<option value=" & Col("LanguageID") & ">" & Col("LanguageName") & "</option>"
                End If
            Next
        Else
            ViewLangsForMenuID = ""
            Exit Function
        End If

        outDD = outDD & "</select>"

        ViewLangsForMenuID = outDD
    End Function

    Private Function FindSelected(ByVal chk As String) As String
        Dim val As String = ""
        If (m_strViewItem = chk) Then
            val = " selected "
        End If
        Return val
    End Function

    'Private Function GetLanguageForTaxonomy(ByVal TaxonomyId As Long, ByVal BGColor As String, ByVal ShowTranslated As Boolean, ByVal ShowAllOpt As Boolean, ByVal onChangeEv As String) As String
    '    Dim result As String = ""
    '    Dim frmName As String = ""
    '    Dim result_language As IList(Of LanguageData) = Nothing
    '    Dim taxonomy_language_request As New TaxonomyLanguageRequest
    '    taxonomy_language_request.TaxonomyId = TaxonomyId

    '    If (ShowTranslated) Then
    '        taxonomy_language_request.IsTranslated = True
    '        result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request)
    '        frmName = "frm_translated"
    '    Else
    '        taxonomy_language_request.IsTranslated = False
    '        result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request)
    '        frmName = "frm_nontranslated"
    '    End If

    '    result = "<select id=""" & frmName & """ name=""" & frmName & """ OnChange=""" & onChangeEv & """>" & vbCrLf

    '    If (CBool(ShowAllOpt)) Then
    '        If TaxonomyLanguage = -1 Then
    '            result = result & "<option value=""-1"" selected>All</option>"
    '        Else
    '            result = result & "<option value=""-1"">All</option>"
    '        End If
    '    Else
    '        If (ShowTranslated = False) Then
    '            result = result & "<option value=""0"">-select language-</option>"
    '        End If
    '    End If
    '    If ((result_language IsNot Nothing) AndAlso (result_language.Count > 0) AndAlso (m_refCommon.EnableMultilingual = 1)) Then
    '        For Each language As LanguageData In result_language
    '            If TaxonomyLanguage = language.Id Then
    '                result = result & "<option value=" & language.Id & " selected>" & language.Name & "</option>"
    '            Else
    '                result = result & "<option value=" & language.Id & ">" & language.Name & "</option>"
    '            End If
    '        Next
    '    Else
    '        result = ""
    '    End If
    '    If (result.Length > 0) Then
    '        result = result & "</select>"
    '    End If
    '    Return (result)
    'End Function
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
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJsonJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronDmsMenuJS)

        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronDmsMenuCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronDmsMenuIE6Css, API.Css.BrowserTarget.LessThanEqualToIE6)
    End Sub
End Class
