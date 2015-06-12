Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports System.DateTime
Imports System.Collections.Generic

Partial Class edittaxonomy
    Inherits System.Web.UI.UserControl

    Protected m_refApi As New ContentAPI
    Protected m_refstyle As New StyleHelper
    Protected AppImgPath As String = ""
    Protected m_refMsg As EkMessageHelper
    Protected m_strPageAction As String = ""
    Protected m_refContent As Content.EkContent
    Protected TaxonomyLanguage As Integer = -1
    Protected TaxonomyId As Long = 0
    Protected TaxonomyParentId As Long = 0
    Protected language_data As LanguageData
    Protected taxonomy_data As New TaxonomyData
    Protected taxonomy_request As TaxonomyRequest
    Protected ShowSaveIcon As Boolean = True
    Protected m_strReorderAction As String = "category"
    Protected taxonomy_arr As TaxonomyBaseData() = Nothing
    Protected TotalItems As Integer = 0
    Protected m_strCurrentBreadcrumb As String = ""
    Protected m_bSynchronized As Boolean = True
    Protected objLocalizationApi As New LocalizationAPI()
    Protected _customPropertyDataList As List(Of Ektron.Cms.Common.CustomPropertyData)
    Protected _customProperty As New Ektron.Cms.Framework.Core.CustomProperty.CustomProperty
    Protected _customPropertyData As New Ektron.Cms.Common.CustomPropertyData
    Protected _customPropertyObjectData As New Ektron.Cms.Common.CustomPropertyObjectData
    Protected _coreCustomProperty As New Ektron.Cms.Core.CustomProperty.CustomPropertyObjectBL

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = m_refApi.EkMsgRef
        AppImgPath = m_refApi.AppImgPath
        m_strPageAction = Request.QueryString("action")
        Utilities.SetLanguage(m_refApi)
        TaxonomyLanguage = m_refApi.ContentLanguage
        If (TaxonomyLanguage = -1) Then
            TaxonomyLanguage = m_refApi.DefaultContentLanguage
        End If
        If (Request.QueryString("taxonomyid") IsNot Nothing) Then
            TaxonomyId = Convert.ToInt64(Request.QueryString("taxonomyid"))
            hdnTaxonomyId.Value = TaxonomyId
        End If
        If (Request.QueryString("parentid") IsNot Nothing) Then
            TaxonomyParentId = Convert.ToInt64(Request.QueryString("parentid"))
        End If
        m_refContent = m_refApi.EkContentRef
        If (Request.QueryString("reorder") IsNot Nothing) Then
            m_strReorderAction = Convert.ToString(Request.QueryString("reorder"))
        End If
        chkApplyDisplayAllLanguages.Text = m_refMsg.GetMessage("lbl apply display taxonomy languages")

        If (Page.IsPostBack) Then
            If (m_strPageAction = "edit") Then
                taxonomy_data.TaxonomyLanguage = TaxonomyLanguage
                taxonomy_data.TaxonomyParentId = TaxonomyParentId
                taxonomy_data.TaxonomyId = TaxonomyId
                taxonomy_data.TaxonomyDescription = Request.Form(taxonomydescription.UniqueID)
                taxonomy_data.TaxonomyName = Request.Form(taxonomytitle.UniqueID)
                taxonomy_data.TaxonomyImage = Request.Form(taxonomy_image.UniqueID)
                taxonomy_data.CategoryUrl = Request.Form(categoryLink.UniqueID)
                If tr_enableDisable.Visible = True Then
                    If Not String.IsNullOrEmpty(Request.Form(chkEnableDisable.UniqueID)) Then
                        taxonomy_data.Visible = True
                    Else
                        taxonomy_data.Visible = False
                    End If
                Else
                    taxonomy_data.Visible = Request.Form(visibility.UniqueID)
                End If
                If (Request.Form(inherittemplate.UniqueID) IsNot Nothing) Then
                    taxonomy_data.TemplateInherited = True
                End If
                If (Request.Form(taxonomytemplate.UniqueID) IsNot Nothing) Then
                    taxonomy_data.TemplateId = Convert.ToInt64(Request.Form(taxonomytemplate.UniqueID))
                Else
                    taxonomy_data.TemplateId = 0
                End If

                Try
                    m_refContent.UpdateTaxonomy(taxonomy_data)
                Catch ex As Exception
                    Utilities.ShowError(ex.Message)
                End Try
                If (Not (Request.Form(chkApplyDisplayAllLanguages.UniqueID) Is Nothing)) AndAlso (Request.Form(chkApplyDisplayAllLanguages.UniqueID).ToString().ToLower() = "on") Then
                    m_refContent.UpdateTaxonomyVisible(TaxonomyId, -1, taxonomy_data.Visible)
                Else
                    m_refContent.UpdateTaxonomyVisible(TaxonomyId, TaxonomyLanguage, taxonomy_data.Visible)
                End If
                If (TaxonomyParentId = 0) Then
                    Dim strConfig As String = String.Empty
                    If (Not String.IsNullOrEmpty(Request.Form(chkConfigContent.UniqueID))) Then
                        strConfig = "0"
                    End If
                    If (Not String.IsNullOrEmpty(Request.Form(chkConfigUser.UniqueID))) Then
                        If (String.IsNullOrEmpty(strConfig)) Then
                            strConfig = "1"
                        Else
                            strConfig = strConfig & ",1"
                        End If
                    End If
                    If (Not String.IsNullOrEmpty(Request.Form(chkConfigGroup.UniqueID))) Then
                        If (String.IsNullOrEmpty(strConfig)) Then
                            strConfig = "2"
                        Else
                            strConfig = strConfig & ",2"
                        End If
                    End If
                    If (Not (String.IsNullOrEmpty(strConfig))) Then
                        m_refContent.UpdateTaxonomyConfig(TaxonomyId, strConfig)
                    End If
                End If
                UpdateCustomProperties()

                If (Request.QueryString("iframe") = "true") Then
                    Response.Write("<script type=""text/javascript"">parent.CloseChildPage();</script>")
                Else
                    If (Request.QueryString("backaction") IsNot Nothing AndAlso Convert.ToString(Request.QueryString("backaction")).ToLower = "viewtree") Then
                        Response.Redirect("taxonomy.aspx?action=viewtree&taxonomyid=" & TaxonomyId & "&LangType=" & TaxonomyLanguage, True)
                    Else
                        Response.Redirect("taxonomy.aspx?action=view&view=item&taxonomyid=" & TaxonomyId & "&rf=1", True)
                    End If
                End If
            Else
                If (Request.Form(LinkOrder.UniqueID) <> "") Then
                    taxonomy_request = New TaxonomyRequest
                    taxonomy_request.TaxonomyId = TaxonomyId
                    taxonomy_request.TaxonomyLanguage = TaxonomyLanguage
                    taxonomy_request.TaxonomyIdList = Request.Form(LinkOrder.UniqueID)
                    If Not String.IsNullOrEmpty(Request.Form(chkOrderAllLang.UniqueID)) Then
                        If (m_strReorderAction = "category") Then
                            m_refContent.ReOrderAllLanguageCategories(taxonomy_request)
                        End If
                    Else
                        If (m_strReorderAction = "category") Then
                            m_refContent.ReOrderCategories(taxonomy_request)
                        Else
                            m_refContent.ReOrderTaxonomyItems(taxonomy_request)
                        End If
                    End If
                End If
                If (Request.QueryString("iframe") = "true") Then
                    Response.Write("<script type=""text/javascript"">parent.CloseChildPage();</script>")
                Else
                    Response.Redirect("taxonomy.aspx?action=view&taxonomyid=" & TaxonomyId & "&rf=1", True)
                End If
            End If
        Else
            ltr_sitepath.Text = m_refApi.SitePath
            taxonomy_request = New TaxonomyRequest()
            taxonomy_request.TaxonomyId = TaxonomyId
            taxonomy_request.TaxonomyLanguage = TaxonomyLanguage

            If (m_strPageAction = "edit") Then
                taxonomy_data = m_refContent.ReadTaxonomy(taxonomy_request)

                taxonomydescription.Text = taxonomy_data.TaxonomyDescription
                taxonomytitle.Text = taxonomy_data.TaxonomyName
                taxonomy_image.Text = taxonomy_data.TaxonomyImage
                taxonomy_image_thumb.ImageUrl = taxonomy_data.TaxonomyImage
                categoryLink.Text = taxonomy_data.CategoryUrl
                visibility.Value = taxonomy_data.Visible
                If (Request.QueryString("taxonomyid") IsNot Nothing) Then
                    TaxonomyId = Convert.ToInt64(Request.QueryString("taxonomyid"))
                End If

                If TaxonomyParentId > 0 Then
                    m_bSynchronized = m_refContent.IsSynchronizedTaxonomy(TaxonomyParentId, TaxonomyLanguage)
                ElseIf TaxonomyId > 0 Then
                    m_bSynchronized = m_refContent.IsSynchronizedTaxonomy(TaxonomyId, TaxonomyLanguage)
                End If

                ' ' why in the world would you disable the visible flag if it's not synchronized???
                'If Not m_bSynchronized Then
                'tr_enableDisable.Visible = False
                'End If

                If taxonomy_data.Visible = True Then
                    chkEnableDisable.Checked = True
                Else
                    chkEnableDisable.Checked = False
                End If
                If taxonomy_data.TaxonomyImage <> "" Then
                    taxonomy_image_thumb.ImageUrl = IIf(taxonomy_data.TaxonomyImage.IndexOf("/") = 0, taxonomy_data.TaxonomyImage, m_refApi.SitePath & taxonomy_data.TaxonomyImage)
                Else
                    taxonomy_image_thumb.ImageUrl = m_refApi.AppImgPath & "spacer.gif"
                End If
                language_data = (New SiteAPI).GetLanguageById(TaxonomyLanguage)
                If (taxonomy_data.TaxonomyParentId = 0) Then
                    inherittemplate.Visible = False
                    lblInherited.Text = "No"
                    inherittemplate.Checked = taxonomy_data.TemplateInherited
                Else
                    inherittemplate.Visible = True
                    lblInherited.Text = ""
                    inherittemplate.Checked = taxonomy_data.TemplateInherited
                    If (taxonomy_data.TemplateInherited) Then
                        taxonomytemplate.Enabled = False
                    End If
                End If
                Dim templates As TemplateData() = Nothing
                templates = m_refApi.GetAllTemplates("TemplateFileName")
                taxonomytemplate.Items.Add(New System.Web.UI.WebControls.ListItem("-select template-", 0))
                If (templates IsNot Nothing AndAlso templates.Length > 0) Then
                    For i As Integer = 0 To templates.Length - 1
                        taxonomytemplate.Items.Add(New System.Web.UI.WebControls.ListItem(templates(i).FileName, templates(i).Id))
                        If (taxonomy_data.TemplateId = templates(i).Id) Then
                            taxonomytemplate.SelectedIndex = i + 1
                        End If
                    Next
                End If
                If ((language_data IsNot Nothing) AndAlso (m_refApi.EnableMultilingual = 1)) Then
                    lblLanguage.Text = "[" & language_data.Name & "]"
                End If
                m_strCurrentBreadcrumb = taxonomy_data.TaxonomyPath.Remove(0, 1).Replace("\", " > ")
                If (m_strCurrentBreadcrumb = "") Then
                    m_strCurrentBreadcrumb = "Root"
                End If
                inherittemplate.Attributes.Add("onclick", "OnInheritTemplateClicked(this)")
                If (TaxonomyParentId = 0) Then
                    tr_config.Visible = True
                    Dim config_list As List(Of Int32) = m_refApi.EkContentRef.GetAllConfigIdListByTaxonomy(TaxonomyId, TaxonomyLanguage)
                    For i As Integer = 0 To config_list.Count - 1
                        If (config_list.Item(i) = 0) Then
                            chkConfigContent.Checked = True
                        ElseIf (config_list.Item(i) = 1) Then
                            chkConfigUser.Checked = True
                        ElseIf (config_list.Item(i) = 2) Then
                            chkConfigGroup.Checked = True
                        End If
                    Next
                Else
                    tr_config.Visible = False
                End If

                LoadCustomPropertyList()

            Else
                If (m_strReorderAction = "category") Then
                    taxonomy_request.PageSize = 99999999    ' pagesize of 0 used to mean "all"
                    taxonomy_arr = m_refContent.ReadAllSubCategories(taxonomy_request)
                    If (taxonomy_arr IsNot Nothing) Then
                        TotalItems = taxonomy_arr.Length
                    Else
                        TotalItems = 0
                    End If
                    If (TotalItems > 1) Then
                        td_msg.Text = m_refMsg.GetMessage("generic first msg")
                        OrderList.DataSource = taxonomy_arr
                        OrderList.DataTextField = "TaxonomyName"
                        OrderList.DataValueField = "TaxonomyId"
                        OrderList.DataBind()
                        OrderList.SelectionMode = ListSelectionMode.Multiple
                        OrderList.CssClass = "width: 100%; border-style: none; border-color: White; font-family: Verdana;font-size: x-small;"
                        If (TotalItems > 20) Then
                            OrderList.Rows = 20
                        Else
                            OrderList.Rows = TotalItems
                        End If
                        OrderList.Width = "300"
                        If (TotalItems > 0) Then
                            loadscript.Text = "document.forms[0].taxonomy_OrderList[0].selected = true;"
                        End If
                        For i As Integer = 0 To taxonomy_arr.Length - 1
                            If (LinkOrder.Value = "") Then
                                LinkOrder.Value = Convert.ToString(taxonomy_arr(i).TaxonomyId)
                            Else
                                LinkOrder.Value = Convert.ToString(taxonomy_arr(i).TaxonomyId) & ","
                            End If
                        Next
                    Else
                        LoadNoItem()
                    End If
                Else
                    AllLangForm.Visible = False     ' the all languages checkbox is only valid for categories
                    taxonomy_request.PageSize = 99999999
                    taxonomy_request.IncludeItems = True
                    taxonomy_data = m_refContent.ReadTaxonomy(taxonomy_request)
                    tr_ordering.Visible = True 'Not showing for items
                    If (taxonomy_data.TaxonomyItems IsNot Nothing) Then
                        TotalItems = taxonomy_data.TaxonomyItems.Length
                        If (TotalItems > 1) Then
                            td_msg.Text = m_refMsg.GetMessage("generic first msg")
                            OrderList.DataSource = taxonomy_data.TaxonomyItems
                            OrderList.DataTextField = "TaxonomyItemTitle"
                            OrderList.DataValueField = "TaxonomyItemId"
                            OrderList.DataBind()
                            OrderList.SelectionMode = ListSelectionMode.Multiple
                            OrderList.CssClass = "width: 100%; border-style: none; border-color: White; font-family: Verdana;font-size: x-small;"
                            If (TotalItems > 20) Then
                                OrderList.Rows = 20
                            Else
                                OrderList.Rows = TotalItems
                            End If
                            OrderList.Width = "300"
                            If (TotalItems > 0) Then
                                loadscript.Text = "document.forms[0].taxonomy_OrderList[0].selected = true;"
                            End If
                            For Each taxonomy_item As TaxonomyItemData In taxonomy_data.TaxonomyItems
                                If (LinkOrder.Value = "") Then
                                    LinkOrder.Value = Convert.ToString(taxonomy_item.TaxonomyItemId)
                                Else
                                    LinkOrder.Value = Convert.ToString(taxonomy_item.TaxonomyItemId) & ","
                                End If
                            Next
                        Else
                            LoadNoItem()
                        End If
                    End If
                End If
        End If
        TaxonomyToolBar()
        End If
    End Sub
    Private Sub LoadNoItem()
        ShowSaveIcon = False
        td_moveicon.Visible = False
        td_msg.Text = m_refMsg.GetMessage("msg no items available to perform reorder")
        OrderList.Visible = False
        tr_ordering.Visible = False
    End Sub
    Private Sub TaxonomyToolBar()
        Dim _taxName As String = ""
        If Not taxonomy_data Is Nothing AndAlso Not taxonomy_data.TaxonomyName Is Nothing AndAlso taxonomy_data.TaxonomyName <> "" Then
            _taxName = taxonomy_data.TaxonomyName
        ElseIf TaxonomyId > 0 Then 'will be called only on reorder items screen. No other way to get the taxonomy name
            taxonomy_request = New TaxonomyRequest()
            taxonomy_request.TaxonomyId = TaxonomyId
            taxonomy_request.TaxonomyLanguage = TaxonomyLanguage
            Dim _taxData As TaxonomyData = m_refContent.ReadTaxonomy(taxonomy_request)
            _taxName = _taxData.TaxonomyName
        End If
        If m_strPageAction = "reorder" Then
            divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("reorder taxonomy page title") & "&nbsp;&nbsp;""" & _taxName & """&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) & "' />")
        Else
            divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("edit taxonomy page title") & "&nbsp;&nbsp;""" & _taxName & """&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) & "' />")
        End If
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        If (m_strPageAction = "edit") Then
            result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (taxonomy)"), m_refMsg.GetMessage("btn update"), "onclick=""javascript:if(SetPropertyIds()){Validate(true);}"""))
        Else
            If (ShowSaveIcon) Then
                result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (taxonomy)"), m_refMsg.GetMessage("btn update"), "onclick=""javascript:if(SetPropertyIds()){Validate(false);}"""))
            End If
        End If
        If (Request.QueryString("iframe") = "true") Then
            result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("generic Cancel"), "onClick=""javascript:parent.CancelIframe();"""))
        Else
            result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "taxonomy.aspx?action=view&taxonomyid=" & TaxonomyId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If
        If (m_strPageAction <> "edit") Then
            result.Append("<td>&nbsp;|&nbsp;</td>")
            result.Append("<td>" & ReorderDropDown() & "</td>")
            result.Append("<td>" & m_refstyle.GetHelpButton("ReOrderTaxonomyOrCategoryItem") & "</td>")
        Else
            result.Append("<td>" & m_refstyle.GetHelpButton("EditTaxonomyOrCategory") & "</td>")
        End If
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Private Function ReorderDropDown() As String
        Dim result As New StringBuilder
        result.Append(m_refMsg.GetMessage("type label") & _
                      "<select id=""selreorderitem"" name=""selreorderitem"" onchange=""javascript:LoadReorderType(this.value);"">" _
                      & "<option value=""category"" " & FindSelected("category") & ">Category</option>" _
                      & "<option value=""item""  " & FindSelected("item") & ">Items</option></select>")
        Return result.ToString()
    End Function
    Private Function FindSelected(ByVal chk As String) As String
        Dim val As String = ""
        If (m_strReorderAction = chk) Then
            val = " selected "
        End If
        Return val
    End Function

    Private Sub LoadCustomPropertyList()

        Dim i As Integer = 0
        Dim j As Integer = 1

        Dim pageInfo As New PagingInfo
        pageInfo.CurrentPage = 1
        pageInfo.RecordsPerPage = 99999

        _customPropertyDataList = _customProperty.GetList(EkEnumeration.CustomPropertyObjectType.TaxonomyNode, m_refApi.ContentLanguage, pageInfo)
        For i = 0 To _customPropertyDataList.Count - 1
            If _customPropertyDataList(i).IsEnabled Then
                availableCustomProp.Items.Insert(j, _customPropertyDataList(i).PropertyName)
                availableCustomProp.Items.Item(j).Value = _customPropertyDataList(i).PropertyId
                j = j + 1
            End If
        Next

        availableCustomProp.DataBind()

    End Sub

    Private Sub UpdateCustomProperties()
        Dim propertyCount As Integer = 0
        Dim itemsCount As Integer = 0
        Dim valueCount As Integer = 0
        Dim i As Integer = 0
        Dim cp As New Ektron.Cms.Framework.Core.CustomProperty.CustomProperty
        Dim cpo As New Ektron.Cms.Framework.Core.CustomProperty.CustomPropertyObject
        Dim listData As System.Collections.Generic.List(Of Ektron.Cms.Common.CustomPropertyObjectData)
        Dim selectedIds As String() = Nothing
        Dim selectedValues As String() = Nothing

        Dim pageInfo As New PagingInfo
        pageInfo.CurrentPage = 1
        pageInfo.RecordsPerPage = 99999

        listData = cpo.GetList(TaxonomyId, m_refApi.ContentLanguage, EkEnumeration.CustomPropertyObjectType.TaxonomyNode, pageInfo)
        If Request.Form(hdnSelectedIDS.UniqueID) <> "" Then
            selectedIds = Request.Form(hdnSelectedIDS.UniqueID).Remove(Request.Form(hdnSelectedIDS.UniqueID).Length - 1, 1).Split(";")
        End If
        If Request.Form(hdnSelectValue.UniqueID) <> "" Then
            selectedValues = Request.Form(hdnSelectValue.UniqueID).Remove(Request.Form(hdnSelectValue.UniqueID).Length - 1, 1).Split(";")
        End If
        _customPropertyDataList = _customProperty.GetList(Ektron.Cms.Common.EkEnumeration.CustomPropertyObjectType.TaxonomyNode, pageInfo)

        For i = 0 To listData.Count - 1
            cpo.Delete(TaxonomyId, m_refApi.ContentLanguage, EkEnumeration.CustomPropertyObjectType.TaxonomyNode, listData(i).PropertyId)
        Next

        If selectedIds IsNot Nothing Or selectedValues IsNot Nothing Then
            For i = 0 To selectedIds.Length - 1
                Dim customPropertyData = cp.GetItem(selectedIds(i), m_refApi.ContentLanguage)
                Dim data As CustomPropertyObjectData = New CustomPropertyObjectData(TaxonomyId, m_refApi.ContentLanguage, selectedIds(i), EkEnumeration.CustomPropertyObjectType.TaxonomyNode)

                If ((Not IsNothing(customPropertyData)) AndAlso (Not IsNothing(data))) Then

                    Dim inputValue As String = HttpUtility.UrlDecode(selectedValues(i).ToString())

                    Select Case customPropertyData.PropertyDataType
                        Case EkEnumeration.CustomPropertyItemDataType.Boolean
                            Dim booleanValue As Boolean
                            If (Boolean.TryParse(inputValue, booleanValue)) Then
                                data.AddItem(booleanValue)
                            End If

                        Case EkEnumeration.CustomPropertyItemDataType.DateTime
                            Dim dateTimeValue As DateTime
                            If (DateTime.TryParse(inputValue, dateTimeValue)) Then
                                data.AddItem(dateTimeValue)
                            End If
                        Case Else
                            data.AddItem(inputValue)
                    End Select
                    cpo.Add(data)
                End If
            Next
        End If
    End Sub

End Class

