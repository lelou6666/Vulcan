Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports System.DateTime
Imports System.Collections.Generic
Imports System.IO

Partial Class viewitems
    Inherits System.Web.UI.UserControl

    Protected m_refCommon As New CommonApi
    Protected m_refstyle As New StyleHelper
    Protected AppImgPath As String = ""
    Protected m_refMsg As EkMessageHelper
    Protected m_strPageAction As String = ""
    Protected m_refContent As Content.EkContent
    Protected TaxonomyId As Long = 0
    Protected TaxonomyLanguage As Integer = -1
    Protected language_data As LanguageData
    Protected taxonomy_request As TaxonomyRequest
    Protected taxonomy_data As TaxonomyData
    Protected TaxonomyParentId As Long = 0
    Protected m_strViewItem As String = "item"
    Protected AddDeleteIcon As Boolean = False
    Protected m_strTaxonomyName As String = ""
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected m_strDelConfirm As String = ""
    Protected m_strDelItemsConfirm As String = ""
    Protected m_strSelDelWarning As String = ""
    Protected objLocalizationApi As New LocalizationAPI()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = m_refCommon.EkMsgRef
        AppImgPath = m_refCommon.AppImgPath
        m_strPageAction = Request.QueryString("action")
        Utilities.SetLanguage(m_refCommon)
        TaxonomyLanguage = m_refCommon.ContentLanguage
        TaxonomyId = Convert.ToInt64(Request.QueryString("taxonomyid"))
        If (Request.QueryString("view") IsNot Nothing) Then
            m_strViewItem = Request.QueryString("view")
        End If
        taxonomy_request = New TaxonomyRequest
        taxonomy_request.TaxonomyId = TaxonomyId
        taxonomy_request.TaxonomyLanguage = TaxonomyLanguage
        m_refContent = m_refCommon.EkContentRef
        If (Page.IsPostBack AndAlso Request.Form(isPostData.UniqueID) <> "") Then
            If (Request.Form("submittedaction") = "delete") Then
                m_refContent.DeleteTaxonomy(taxonomy_request)
                Response.Write("<script type=""text/javascript"">parent.CloseChildPage();</script>")
            ElseIf (Request.Form("submittedaction") = "deleteitem") Then
                If (m_strViewItem <> "folder") Then
                    taxonomy_request.TaxonomyIdList = Request.Form("selected_items")
                    If (m_strViewItem.ToLower = "cgroup") Then
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Group
                    ElseIf (m_strViewItem.ToLower = "user") Then
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.User
                    Else
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Content
                    End If
                    m_refContent.RemoveTaxonomyItem(taxonomy_request)
                Else
                    Dim tax_folder As New TaxonomySyncRequest
                    tax_folder.TaxonomyId = TaxonomyId
                    tax_folder.TaxonomyLanguage = TaxonomyLanguage
                    tax_folder.SyncIdList = Request.Form("selected_items")
                    m_refContent.RemoveTaxonomyFolder(tax_folder)
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
        isPostData.Value = "true"
    End Sub

    Private Sub DisplayPage()
        Select Case m_strViewItem.ToLower()
            Case "user"
                Dim uReq As New DirectoryUserRequest
                Dim uDirectory As New DirectoryAdvancedUserData
                uReq.GetItems = True
                uReq.DirectoryId = TaxonomyId
                uReq.DirectoryLanguage = TaxonomyLanguage
                uReq.PageSize = m_refCommon.RequestInformationRef.PagingSize
                uReq.CurrentPage = m_intCurrentPage
                uDirectory = Me.m_refContent.LoadDirectory(uReq)
                If (uDirectory IsNot Nothing) Then
                    TaxonomyParentId = uDirectory.DirectoryParentId
                    m_strTaxonomyName = uDirectory.DirectoryName
                    m_intTotalPages = uReq.TotalPages
                End If
                PopulateUserGridData(uDirectory)
                TaxonomyToolBar()
            Case "cgroup"
                Dim dagdRet As New DirectoryAdvancedGroupData
                Dim cReq As New DirectoryGroupRequest

                cReq.CurrentPage = m_intCurrentPage
                cReq.PageSize = m_refCommon.RequestInformationRef.PagingSize
                cReq.DirectoryId = TaxonomyId
                cReq.DirectoryLanguage = TaxonomyLanguage
                cReq.GetItems = True
                cReq.SortDirection = ""

                dagdRet = Me.m_refCommon.CommunityGroupRef.LoadDirectory(cReq)
                If (dagdRet IsNot Nothing) Then
                    TaxonomyParentId = dagdRet.DirectoryParentId
                    m_strTaxonomyName = dagdRet.DirectoryName
                    m_intTotalPages = cReq.TotalPages
                End If
                m_intTotalPages = cReq.TotalPages
                PopulateCommunityGroupGridData(dagdRet)
                TaxonomyToolBar()
            Case Else ' Content
                taxonomy_request.IncludeItems = True
                taxonomy_request.PageSize = m_refCommon.RequestInformationRef.PagingSize
                taxonomy_request.CurrentPage = m_intCurrentPage
                taxonomy_data = m_refContent.ReadTaxonomy(taxonomy_request)
                If (taxonomy_data IsNot Nothing) Then
                    TaxonomyParentId = taxonomy_data.TaxonomyParentId
                    m_strTaxonomyName = taxonomy_data.TaxonomyName
                    m_intTotalPages = taxonomy_request.TotalPages
                End If
                PopulateContentGridData()
                TaxonomyToolBar()
        End Select
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
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("CHECK", "<input type=""Checkbox"" name=""checkall"" onclick=""javascript:checkAll('selected_items',false);"">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(3), Unit.Percentage(2), False, False))
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("ID", m_refMsg.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("COMMUNITYGROUP", m_refMsg.GetMessage("lbl community group"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), False, True))
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("INFORMATION", "&#160;", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), False, False))

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
                dr = dt.NewRow
                dr("CHECK") = "<input type=""checkbox"" name=""selected_items"" id=""selected_items"" value=""" & item.GroupId & """ onClick=""javascript:checkAll('selected_items',true);"">"
                dr("COMMUNITYGROUP") = "<img src=""" & IIf(item.GroupImage <> "", item.GroupImage, Me.m_refCommon.AppImgPath & "member_default.gif") & """ align=""left"" width=""55"" height=""55"" />"
                dr("COMMUNITYGROUP") &= item.GroupName
                dr("COMMUNITYGROUP") &= " (" & IIf(item.GroupEnroll, Me.m_refMsg.GetMessage("lbl enrollment open"), Me.m_refMsg.GetMessage("lbl enrollment restricted")) & ")"
                dr("COMMUNITYGROUP") &= "<br/>"
                dr("COMMUNITYGROUP") &= item.GroupShortDescription

                dr("ID") = item.GroupId

                dr("INFORMATION") = Me.m_refMsg.GetMessage("content dc label") & " " & item.GroupCreatedDate.ToShortDateString
                dr("INFORMATION") &= "<br/>"
                dr("INFORMATION") &= Me.m_refMsg.GetMessage("lbl members") & ": " & item.TotalMember.ToString()
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
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("CHECK", "<input type=""Checkbox"" name=""checkall"" onclick=""javascript:checkAll('selected_items',false);"">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(3), Unit.Percentage(2), False, False))
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("ID", m_refMsg.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("USERNAME", m_refMsg.GetMessage("generic username"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), False, False))
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("DISPLAYNAME", m_refMsg.GetMessage("display name label"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), False, False))
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
                dr = dt.NewRow
                dr("CHECK") = "<input type=""checkbox"" name=""selected_items"" id=""selected_items"" value=""" & item.Id & """ onClick=""javascript:checkAll('selected_items',true);"">"
                dr("USERNAME") = item.Username  '"<a href=""taxonomy.aspx?action=viewtree&taxonomyid=" & item.TaxonomyItemId & "&LangType=" & item.TaxonomyItemLanguage & """>" & item.TaxonomyItemTitle & "</a>"
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
        If (m_strPageAction = "removeitems") Then
            TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("CHECK", "<input type=""Checkbox"" name=""checkall"" onclick=""javascript:checkAll('selected_items',false);"">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(2), Unit.Percentage(2), False, False))
        End If
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("TITLE", m_refMsg.GetMessage("generic title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(88), False, False))
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("ID", m_refMsg.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        TaxonomyItemList.Columns.Add(m_refstyle.CreateBoundField("LANGUAGE", m_refMsg.GetMessage("generic language"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        Dim dt As New DataTable
        Dim dr As DataRow
        Dim m_refContentApi As New ContentAPI
        If (m_strPageAction = "removeitems") Then
            dt.Columns.Add(New DataColumn("CHECK", GetType(String)))
        End If
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        If (m_strViewItem <> "folder") Then
            PageSettings()
            If (taxonomy_data IsNot Nothing AndAlso taxonomy_data.TaxonomyItems IsNot Nothing AndAlso taxonomy_data.TaxonomyItems.Length > 0) Then
                AddDeleteIcon = True
                Dim icon As String = ""
                Dim title As String = ""
                Dim link As String = ""
                Dim backPage As String = ""
                For Each item As TaxonomyItemData In taxonomy_data.TaxonomyItems
                    dr = dt.NewRow
                    If (m_strPageAction = "removeitems") Then
                        dr("CHECK") = "<input type=""checkbox"" name=""selected_items"" id=""selected_items"" value=""" & item.TaxonomyItemId & """ onClick=""javascript:checkAll('selected_items',true);"">"
                    End If
                    If item.ContentType = 1 Then
                        If (item.ContentSubType = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData) Then
                            icon = "&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/layout.png"" />&nbsp;"
                        ElseIf (item.ContentSubType = EkEnumeration.CMSContentSubtype.WebEvent) Then
                            icon = "&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/calendar.png" & """ />&nbsp;"
                        Else
                            icon = "&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/contentHtml.png" & """ />&nbsp;"
                        End If
                    ElseIf item.ContentType = 2 Then
                        icon = "&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/contentForm.png" & """ />&nbsp;"
                    ElseIf item.ContentType = 1111 Then
                        icon = "&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/tree/folderBoard.png" & """ alt=""" & item.TaxonomyItemAssetInfo.FileName & """ />&nbsp;"
                    ElseIf item.ContentType = 1112 Then
                        icon = "&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/tree/folderBlog.png" & """ alt=""" & item.TaxonomyItemAssetInfo.FileName & """ />&nbsp;"
                    ElseIf item.ContentType = 3333 Then
                        icon = "&nbsp;<img src=""" & m_refContentApi.AppPath & "images/ui/icons/brick.png" & """ alt=""" & item.TaxonomyItemAssetInfo.FileName & """ />&nbsp;"
                    Else
                        icon = "&nbsp;<img src=""" & item.TaxonomyItemAssetInfo.ImageUrl & """ alt=""" & item.TaxonomyItemAssetInfo.FileName & """ />&nbsp;"
                    End If
                    If item.TaxonomyItemAssetInfo.ImageUrl = "" And (item.ContentType <> 1 And item.ContentType <> 2 And item.ContentType <> 1111 And item.ContentType <> 1112 And item.ContentType <> 3333) Then
                        icon = "&nbsp;<img src=""" & m_refContentApi.AppPath & "images/UI/Icons/book.png" & """ alt=""" & item.TaxonomyItemAssetInfo.FileName & """ />&nbsp;"
                    End If

                    title = m_refMsg.GetMessage("generic View") & " """ & Replace(item.TaxonomyItemTitle, " '", "`") & """"
                    backPage = Server.UrlEncode("Action=viewcontent&view=item&taxonomyid=" & TaxonomyId)
                    If (item.ContentSubType = EkEnumeration.CMSContentSubtype.WebEvent AndAlso item.ContentType = 1) Then
                        Dim fid As Long = m_refContentApi.EkContentRef.GetFolderIDForContent(item.TaxonomyItemId)
                        link = "<a href='content.aspx?action=ViewContentByCategory&LangType=" & item.TaxonomyItemLanguage & "&id=" & fid & "&callerpage=taxonomy.aspx&origurl=" & backPage & "' title='" & title & "'>" & item.TaxonomyItemTitle & "</a>"
                    Else
                        link = "<a href='content.aspx?action=View&LangType=" & item.TaxonomyItemLanguage & "&id=" & item.TaxonomyItemId & "&callerpage=taxonomy.aspx&origurl=" & backPage & "' title='" & title & "'>" & item.TaxonomyItemTitle & "</a>"
                    End If
                    dr("TITLE") = icon & link
                    dr("ID") = item.TaxonomyItemId
                    dr("LANGUAGE") = item.TaxonomyItemLanguage
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
            taxonomy_sync_folder = m_refContent.GetAllAssignedCategoryFolder(tax_sync_folder_req)
            If (taxonomy_sync_folder IsNot Nothing AndAlso taxonomy_sync_folder.Length > 0) Then
                AddDeleteIcon = True
                For i As Integer = 0 To taxonomy_sync_folder.Length - 1
                    dr = dt.NewRow
                    dr("CHECK") = "<input type=""checkbox"" name=""selected_items"" id=""selected_items"" value=""" & taxonomy_sync_folder(i).FolderId & """ onClick=""javascript:checkAll('selected_items',true);"">"
                    dr("TITLE") = taxonomy_sync_folder(i).FolderTitle '& GetRecursiveTitle(item.FolderRecursive)
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
        Dim strDeleteMsg As String = ""
        If (TaxonomyParentId > 0) Then
            strDeleteMsg = m_refMsg.GetMessage("alt delete button text (category)")
            m_strDelConfirm = m_refMsg.GetMessage("delete category confirm")
            m_strDelItemsConfirm = m_refMsg.GetMessage("delete category items confirm")
            m_strSelDelWarning = m_refMsg.GetMessage("select category item missing warning")
        Else
            strDeleteMsg = m_refMsg.GetMessage("alt delete button text (taxonomy)")
            m_strDelConfirm = m_refMsg.GetMessage("delete taxonomy confirm")
            m_strDelItemsConfirm = m_refMsg.GetMessage("delete taxonomy items confirm")
            m_strSelDelWarning = m_refMsg.GetMessage("select taxonomy item missing warning")
        End If
        divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("view taxonomy page title") & " """ & m_strTaxonomyName & """" & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) & "' />")
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        If (m_strPageAction <> "removeitems") Then
            result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/add.png", "taxonomy.aspx?action=additem&taxonomyid=" & TaxonomyId & "&parentid=" & TaxonomyParentId, m_refMsg.GetMessage("lbl assign items"), m_refMsg.GetMessage("lbl assign items"), ""))
            result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/remove.png", "taxonomy.aspx?action=removeitems&taxonomyid=" & TaxonomyId & "&parentid=" & TaxonomyParentId, m_refMsg.GetMessage("remove taxonomy items"), m_refMsg.GetMessage("remove taxonomy items"), ""))
            result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/properties.png", "taxonomy.aspx?action=viewattributes&taxonomyid=" & TaxonomyId & "&parentid=" & TaxonomyParentId & "&LangType=" & TaxonomyLanguage, m_refMsg.GetMessage("btn properties"), m_refMsg.GetMessage("btn properties"), ""))
            ' result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "btn_delete_folder-nm.gif", "#", strDeleteMsg, m_refMsg.GetMessage("btn delete"), "Onclick=""javascript:return DeleteItem();"""))
        Else
            If (AddDeleteIcon) Then
                result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt remove button text (taxonomyitems)"), m_refMsg.GetMessage("btn remove"), "Onclick=""javascript:return DeleteItem('items');"""))
            End If
        End If

        If (m_strPageAction <> "viewcontent") Then
            result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "taxonomy.aspx?action=viewcontent&view=item&taxonomyid=" & TaxonomyId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If
        result.Append("<td nowrap=""true"">")
        Dim addDD As String
        addDD = GetLanguageForTaxonomy(TaxonomyId, "", False, False, "javascript:TranslateTaxonomy(" & TaxonomyId & ", " & TaxonomyParentId & ", this.value);")
        If addDD <> "" Then
            addDD = "&nbsp;" & m_refMsg.GetMessage("add title") & ":&nbsp;" & addDD
        End If
        If (CStr(m_refCommon.EnableMultilingual = "1")) Then
            result.Append("View In:&nbsp;" & GetLanguageForTaxonomy(TaxonomyId, "", True, False, "javascript:LoadLanguage(this.value);") & "&nbsp;" & addDD & "<br>")
        End If
        result.Append("</td>")

        If (m_strPageAction <> "viewcontent") Then
            result.Append(ViewTypeDropDown())
        End If

        result.Append("<td>" & m_refstyle.GetHelpButton("ViewTaxonomyOrCategory") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Private Function ViewTypeDropDown() As String
        Dim result As New StringBuilder
        result.Append("<td class=""label"">")
        result.Append(m_refMsg.GetMessage("lbl View") & ":")
        result.Append("</td>")
        result.Append("<td>")
        result.Append("<select id=""selviewtype"" name=""selviewtype"" onchange=""javascript:LoadViewType(this.value);"">")
        result.Append("<option value=""folder"" " & FindSelected("folder") & ">").Append(Me.m_refMsg.GetMessage("lbl folders")).Append("</option>")
        result.Append("<option value=""item""  " & FindSelected("item") & ">").Append(Me.m_refMsg.GetMessage("content button text")).Append("</option>")
        result.Append("<option value=""user""  " & FindSelected("user") & ">").Append(Me.m_refMsg.GetMessage("generic users")).Append("</option>")
        result.Append("<option value=""cgroup""  " & FindSelected("cgroup") & ">").Append(Me.m_refMsg.GetMessage("lbl community groups")).Append("</option>")
        result.Append("</select>")
        result.Append("</td>")
        Return result.ToString()
    End Function

    Private Function FindSelected(ByVal chk As String) As String
        Dim val As String = ""
        If (m_strViewItem = chk) Then
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
            result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request)
            frmName = "frm_translated"
        Else
            taxonomy_language_request.IsTranslated = False
            result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request)
            frmName = "frm_nontranslated"
        End If

        result = "<select id=""" & frmName & """ name=""" & frmName & """ OnChange=""" & onChangeEv & """>" & vbCrLf

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
        If ((result_language IsNot Nothing) AndAlso (result_language.Count > 0) AndAlso (m_refCommon.EnableMultilingual = 1)) Then
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
End Class
