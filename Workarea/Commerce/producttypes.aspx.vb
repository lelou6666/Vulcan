Imports System
Imports System.Collections
Imports System.Data
Imports System.Text
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.Collections.Generic

Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Workarea

Partial Class product_type
    Inherits workareabase

#Region "Member Variables"

    Protected _ContentApi As ContentAPI = New ContentAPI
    Protected _PageName As String = "producttypes.aspx"
    Protected _PageAction As String = ""
    Protected _CommonApi As New CommonApi
    Protected _StyleSheetJS As String = ""
    Protected _ProductTypeData As ProductTypeData
    Protected _ProductType As ProductType = Nothing
    Protected _SelectedAttributeIndex As Integer = -1
    Protected _IsMac As Boolean = False
    Protected _IsFF As Boolean = False
    Protected _IsUsed As Boolean = False
    Protected _CurrentPage As Integer = 1
    Protected _TotalPages As Integer = 1
    Protected _MessageHelper As Ektron.Cms.Common.EkMessageHelper
    Private _ApplicationPath As String
    Private _SitePath As String

#End Region

#Region "Properties"

    Private Property ApplicationPath() As String
        Get
            Return _ApplicationPath
        End Get
        Set(ByVal value As String)
            _ApplicationPath = value
        End Set
    End Property

    Private Property SitePath() As String
        Get
            Return _SitePath
        End Get
        Set(ByVal value As String)
            _SitePath = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub New()

        Dim slash() As Char = {"/"}
        Me.SitePath = _ContentApi.SitePath.TrimEnd(slash)
        Me.ApplicationPath = _ContentApi.ApplicationPath.TrimEnd(slash)

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        'register page resources
        Me.RegisterJS()
        Me.RegisterCSS()

        _MessageHelper = _ContentApi.EkMsgRef

        If Request.QueryString("id") <> "" Then
            _ProductType = New ProductType(m_refContentApi.RequestInformationRef)
            Dim myProductTypeData As ProductTypeData = _ProductType.GetItem(Request.QueryString("id"), True)

            Me.ucAttributes.ProductData = myProductTypeData
            Me.ucAttributes.DisplayMode = Commerce.Workarea.ProductTypes.Tabs.Attributes.DisplayModeValue.View
            Me.ucAttributesEdit.ProductData = myProductTypeData
            Me.ucAttributesEdit.DisplayMode = Commerce.Workarea.ProductTypes.Tabs.Attributes.DisplayModeValue.Edit

            Me.ucMediaDefaults.ProductData = myProductTypeData
            Me.ucMediaDefaults.DisplayMode = Commerce.Workarea.ProductTypes.Tabs.Attributes.DisplayModeValue.View
            Me.ucMediaDefaultsEdit.ProductData = myProductTypeData
            Me.ucMediaDefaultsEdit.DisplayMode = Commerce.Workarea.ProductTypes.Tabs.Attributes.DisplayModeValue.Edit
        End If

        If Request.QueryString("action") = "addproducttype" Then
            Me.ucAttributesEdit.DisplayMode = Commerce.Workarea.ProductTypes.Tabs.Attributes.DisplayModeValue.Edit
            Me.ucMediaDefaultsEdit.DisplayMode = Commerce.Workarea.ProductTypes.Tabs.Attributes.DisplayModeValue.Edit
        End If

    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(GetMessage("feature locked error"))
        End If
        Utilities.ValidateUserLogin()
        If (Request.Browser.Platform.IndexOf("Win") = -1) Then _IsMac = True
        If (Request.UserAgent.ToLower().IndexOf("firefox") > -1) Then _IsFF = True

        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1
        If (Not (IsNothing(Request.QueryString("action")))) Then
            _PageAction = Request.QueryString("action")
            If (_PageAction.Length > 0) Then
                _PageAction = _PageAction.ToLower
            End If
        End If
        If (Request.QueryString("LangType") <> "") Then
            ContentLanguage = Request.QueryString("LangType")
            _CommonApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
        Else
            If CStr(_CommonApi.GetCookieValue("LastValidLanguageID")) <> "" Then
                ContentLanguage = _CommonApi.GetCookieValue("LastValidLanguageID")
            End If
        End If
        _CommonApi.ContentLanguage = ContentLanguage
        Page.Header.Controls.Add(New LiteralControl(m_refStyle.GetClientScript))
    End Sub

    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
        Dim intRetVal As Integer = 0
        Try
            Util_CheckAccess()
            mvViews.SetActiveView(vwViewAddEdit)
            Select Case Me._PageAction
                Case "addthumbnail"
                    Display_AddThumbnail()
                Case "addattribute", "editattribute"
                    Display_ProductAttribute()
                Case "viewproducttype"
                    Display_ViewProductType()
                Case "addproducttype"
                    If Page.IsPostBack Then Process_AddProductType() Else Display_AddProductType()
                Case "editproducttype"
                    If Page.IsPostBack Then Process_EditProductType() Else Display_EditProductType()
                Case "deleteproducttype"
                    _ProductType = New ProductType(m_refContentApi.RequestInformationRef)
                    _ProductType.Delete(Request.QueryString("id"))
                    Response.Redirect(_PageName & "?action=viewallproducttypes", False)
                Case Else ' "viewallproducttypes"
                    mvViews.SetActiveView(vwViewAll)
                    If Page.IsPostBack = False Then
                        Display_ViewAll()
                    End If
            End Select
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                _CurrentPage = 1
            Case "Last"
                _CurrentPage = Int32.Parse(litTotalPages.Text)
            Case "Next"
                _CurrentPage = IIf(Int32.Parse(txtCurrentPage.Text) + 1 <= Int32.Parse(litTotalPages.Text), Int32.Parse(txtCurrentPage.Text) + 1, Int32.Parse(txtCurrentPage.Text))
            Case "Prev"
                _CurrentPage = IIf(Int32.Parse(txtCurrentPage.Text) - 1 >= 1, Int32.Parse(txtCurrentPage.Text) - 1, Int32.Parse(txtCurrentPage.Text))
        End Select
        txtCurrentPage.Text = _CurrentPage
        hdnCurrentPage.Value = _CurrentPage

        Display_ViewAll()
    End Sub

    Protected Sub AdHocPaging_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        _CurrentPage = Int32.Parse(Me.txtCurrentPage.Text)

        txtCurrentPage.Text = _CurrentPage
        hdnCurrentPage.Value = _CurrentPage

        Display_ViewAll()
    End Sub

    Protected Sub DisplayGrid_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        Select Case e.Item.ItemType
            Case ListItemType.AlternatingItem, ListItemType.Item
                If (e.Item.Cells(1).Text.Equals("REMOVE-ITEM")) Then
                    e.Item.Cells(0).ColumnSpan = 2
                    e.Item.Cells(0).CssClass = "label"
                    e.Item.Cells.RemoveAt(1)
                Else
                    If e.Item.Cells(1).Text.Contains("ektronGrid") Then
                        e.Item.Cells(1).CssClass = "noPadding"
                    End If
                End If
        End Select
    End Sub

#End Region

#Region "Display"

    Protected Sub Display_AddThumbnail()
        phAddThumbnail.Visible = True
        _ProductType = New ProductType(m_refContentApi.RequestInformationRef)
        If Me.m_iID > 0 Then
            _ProductTypeData = _ProductType.GetItem(m_iID, True)
        End If

        Util_SetJs()
        Util_SetLabels()
    End Sub

    Protected Sub Display_ProductAttribute()
        phAddEditAttributes.Visible = True
        If m_iID > 0 Then
            Dim AttributeName As String = IIf(Request.QueryString("name") <> "", Request.QueryString("name"), "")
            Dim AttributeTagType As String = IIf(Request.QueryString("type") <> "", Request.QueryString("type"), "")
            Dim AttributeDefaultText As String = IIf(Request.QueryString("def") <> "", Request.QueryString("def"), "")
            txt_attrname.Text = AttributeName
            Select Case AttributeTagType
                Case "text"
                    txt_textdefault.Text = AttributeDefaultText
                    divNum.Style.Add("position", "absolute")
                    divNum.Style.Add("visibility", "hidden")
                    divChk.Style.Add("position", "absolute")
                    divChk.Style.Add("visibility", "hidden")
                    _SelectedAttributeIndex = 0
                Case "boolean"
                    chk_bool.Checked = EkFunctions.GetBoolFromYesNo(AttributeDefaultText)
                    divText.Style.Add("position", "absolute")
                    divText.Style.Add("visibility", "hidden")
                    divNum.Style.Add("position", "absolute")
                    divNum.Style.Add("visibility", "hidden")
                    _SelectedAttributeIndex = 2
                Case Else
                    txt_number.Text = AttributeDefaultText
                    divText.Style.Add("position", "absolute")
                    divText.Style.Add("visibility", "hidden")
                    divChk.Style.Add("position", "absolute")
                    divChk.Style.Add("visibility", "hidden")
                    Select Case AttributeTagType
                        Case "number"
                            _SelectedAttributeIndex = 1
                        Case "byte"
                            _SelectedAttributeIndex = 1
                        Case "double"
                            _SelectedAttributeIndex = 1
                        Case "float"
                            _SelectedAttributeIndex = 1
                        Case "integer"
                            _SelectedAttributeIndex = 1
                        Case "long"
                            _SelectedAttributeIndex = 1
                        Case "short"
                            _SelectedAttributeIndex = 1
                    End Select
            End Select
        Else
            divNum.Style.Add("position", "absolute")
            divNum.Style.Add("visibility", "hidden")
            divChk.Style.Add("position", "absolute")
            divChk.Style.Add("visibility", "hidden")
        End If
        Util_SetJs()
        Util_SetLabels()
    End Sub

    Protected Sub Display_ViewAll()
        _ProductType = New ProductType(m_refContentApi.RequestInformationRef)
        Dim producttypeList As New List(Of ProductTypeData)
        Dim criteria As New Criteria(Of ProductTypeProperty)

        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        criteria.PagingInfo.CurrentPage = _CurrentPage.ToString()

        producttypeList = _ProductType.GetList(criteria)

        _TotalPages = criteria.PagingInfo.TotalPages

        If (_TotalPages <= 1) Then
            divPaging.Visible = False
        Else
            Me.SetPagingUI()
            divPaging.Visible = True
            litTotalPages.Text = (System.Math.Ceiling(_TotalPages)).ToString()
            txtCurrentPage.Text = _CurrentPage.ToString()
        End If
        dgList.DataSource = producttypeList
        dgList.DataBind()

        Util_SetLabels()
    End Sub

    Protected Sub Display_AddProductType()
        _ProductTypeData = New ProductTypeData()
        phAddEdit.Visible = True
        phTabAttributes.Visible = True
        phTabMediaDefaults.Visible = True
        trXslt.Visible = False
        txtTitle.Attributes.Add("onkeypress", "return " & JSLibrary.CheckKeyValueName() & "(event, '34,13');")
        txtDescription.Attributes.Add("onkeypress", "return " & JSLibrary.CheckKeyValueName() & "(event, '34,13');")

        Util_XSLTLinks()
        Util_AddProductTypeItems()
        Util_SetJs()
        Util_SetLabels()

        drp_SubscriptionProvider.Items.Add(New ListItem("MembershipSubscriptionProvider"))
        Me.drp_type.Attributes.Add("onchange", "if (this.selectedIndex == 3) {toggleSubscriptionRow(this, true);} else {toggleSubscriptionRow(this, false);}")

    End Sub

    Protected Sub Display_EditProductType()
        _ProductType = New ProductType(m_refContentApi.RequestInformationRef)
        _ProductTypeData = _ProductType.GetItem(m_iID, True)

        phTabAttributes.Visible = True
        phTabMediaDefaults.Visible = True

        tr_id.Visible = True
        txt_id.Text = _ProductTypeData.Id
        phAddEdit.Visible = True
        txtTitle.Attributes.Add("onkeypress", "return " & JSLibrary.CheckKeyValueName() & "(event, '34,13');")
        txtDescription.Attributes.Add("onkeypress", "return " & JSLibrary.CheckKeyValueName() & "(event, '34,13');")

        Util_PopulateData()
        drp_type.Enabled = False

        If _ProductTypeData.EntryClass = CatalogEntryType.SubscriptionProduct Then

            drp_SubscriptionProvider.Items.Add(New ListItem("MembershipSubscriptionProvider"))
            drp_SubscriptionProvider.SelectedIndex = 0

        End If

        Util_XSLTLinks()
        Util_AddProductTypeItems(_ProductTypeData.EntryClass)
        Util_SetJs()
        Util_SetLabels()
    End Sub

    Protected Sub Display_ViewProductType()
        _ProductType = New ProductType(m_refContentApi.RequestInformationRef)
        _ProductTypeData = _ProductType.GetItem(m_iID, True)

        _IsUsed = _ProductType.IsProductTypeUsed(m_iID)

        tr_id.Visible = True
        txt_id.Text = _ProductTypeData.Id
        phAddEdit.Visible = False
        phView.Visible = True
        phTabAttributes.Visible = True
        phTabMediaDefaults.Visible = True
        txtTitle.Attributes.Add("onkeypress", "return " & JSLibrary.CheckKeyValueName() & "(event, '34,13');")
        txtDescription.Attributes.Add("onkeypress", "return " & JSLibrary.CheckKeyValueName() & "(event, '34,13');")

        Dim xml_config_data As XmlConfigData
        xml_config_data = m_refContentApi.GetXmlConfiguration(m_iID)
        PopulatePropertiesGrid(xml_config_data, _ProductTypeData.EntryClass, _ProductTypeData.SubscriptionProvider)
        PopulateDisplayGrid(xml_config_data)
        If (xml_config_data.PackageDisplayXslt.Length > 0) Then
            PopulatePreviewGrid(xml_config_data)
            phPreview.Visible = True
            phTabPreview.Visible = True
        Else
            phTabPreview.Visible = False
            phPreview.Visible = False
        End If

        Util_PopulateData()
        Util_XSLTLinks()
        Util_AddProductTypeItems(_ProductTypeData.EntryClass)
        Util_SetJs()
        Util_SetLabels()
    End Sub

    Private Sub PopulatePropertiesGrid(ByVal xml_config_data As XmlConfigData, ByVal entryId As Long, ByVal subscriptionProvider As String)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.ItemStyle.CssClass = "label"
        PropertiesGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "VALUE"
        PropertiesGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        Dim bValidDefaultXslt As Boolean = False
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("VALUE", GetType(String)))

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("generic title label")
        dr(1) = xml_config_data.Title
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("id label")
        dr(1) = xml_config_data.Id
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("lbl product type class")
        dr(1) = Util_ShowType(entryId)
        dt.Rows.Add(dr)

        If entryId = EkEnumeration.CatalogEntryType.SubscriptionProduct Then
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("lbl commerce subscription provider")
            dr(1) = subscriptionProvider
            dt.Rows.Add(dr)
        End If

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("description label")
        dr(1) = xml_config_data.Description
        dt.Rows.Add(dr)

        If (xml_config_data.PackageDisplayXslt.Length > 0) Then
            'dr = dt.NewRow
            'dr(0) = ""
            'dr(1) = "REMOVE-ITEM"
            'dt.Rows.Add(dr)
        Else

            Dim sb As New StringBuilder()
            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("editor info label")
            sb.Append(" <div class=""innerTable"">")
            sb.Append("     <table>")
            sb.Append("         <tbody>")
            sb.Append("             <tr>")
            sb.Append("                 <th>" & m_refMsg.GetMessage("edit xslt label") & "</th>")
            sb.Append("                 <td>" & IIf(xml_config_data.LogicalPathComplete("EditXslt") = String.Empty, "-", xml_config_data.LogicalPathComplete("EditXslt")) & "</td>")
            sb.Append("             </tr>")
            sb.Append("             <tr>")
            sb.Append("                 <th>" & m_refMsg.GetMessage("save xslt label") & "</th>")
            sb.Append("                 <td>" & IIf(xml_config_data.LogicalPathComplete("SaveXslt") = String.Empty, "-", xml_config_data.LogicalPathComplete("SaveXslt")) & "</td>")
            sb.Append("             </tr>")
            sb.Append("             <tr>")
            sb.Append("                 <th>" & m_refMsg.GetMessage("advanced config label") & "</th>")
            sb.Append("                 <td>" & IIf(xml_config_data.LogicalPathComplete("XmlAdvConfig") = String.Empty, "-", xml_config_data.LogicalPathComplete("XmlAdvConfig")) & "</td>")
            sb.Append("             </tr>")
            sb.Append("         </tbody>")
            sb.Append("     </table>")
            sb.Append(" </div>")

            dr(1) = sb.ToString()
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("validation info label")

            sb = New StringBuilder()
            sb.Append(" <div class=""innerTable"">")
            sb.Append("     <table")
            sb.Append("         <tbody>")
            sb.Append("             <tr>")
            sb.Append("                 <th>" & m_refMsg.GetMessage("xml schema label") & "</th>")
            sb.Append("                 <td>" & IIf(xml_config_data.LogicalPathComplete("XmlSchema") = String.Empty, "-", xml_config_data.LogicalPathComplete("XmlSchema")) & "</td>")
            sb.Append("             </tr>")
            sb.Append("             <tr>")
            sb.Append("                 <th>" & m_refMsg.GetMessage("target namespace label") & "</th>")
            sb.Append("                 <td>" & IIf(xml_config_data.LogicalPathComplete("XmlNameSpace") = String.Empty, "-", xml_config_data.LogicalPathComplete("XmlNameSpace")) & "</td>")
            sb.Append("             </tr>")
            sb.Append("         </tbody>")
            sb.Append("     </table>")
            sb.Append(" </div>")

            dr(1) = sb.ToString()
            dt.Rows.Add(dr)

        End If

        Dim dv As New DataView(dt)
        PropertiesGrid.DataSource = dv
        PropertiesGrid.DataBind()
    End Sub

    Private Sub PopulatePreviewGrid(ByVal xml_config_data As XmlConfigData)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = m_refMsg.GetMessage("lbl Preview XSLT on empty XML document")
        colBound.ItemStyle.CssClass = "label left"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Height = Unit.Empty
        colBound.ItemStyle.Height = Unit.Empty
        PreviewGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))

        dr = dt.NewRow
        dr(0) = m_refContentApi.TransformXsltPackage("<root></root>", xml_config_data.PackageDisplayXslt, False)
        dt.Rows.Add(dr)

        Dim dv As New DataView(dt)
        PreviewGrid.DataSource = dv
        PreviewGrid.DataBind()
    End Sub

    Private Sub PopulateDisplayGrid(ByVal xml_config_data As XmlConfigData)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.ItemStyle.Width = "200"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "right"
        colBound.HeaderText = "XSLT"
        DisplayGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "VALUE"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "left"
        colBound.HeaderText = "Path"
        DisplayGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DEFAULT"
        colBound.ItemStyle.Width = "50"
        colBound.HeaderStyle.CssClass = "center"
        colBound.ItemStyle.CssClass = "center"
        colBound.HeaderText = "Default"
        DisplayGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        Dim bValidDefaultXslt As Boolean = False
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("VALUE", GetType(String)))
        dt.Columns.Add(New DataColumn("DEFAULT", GetType(String)))

        Dim defaultCheck As String = "<img alt=""Default"" title=""Default"" src=""" & _ContentApi.ApplicationPath & "images/ui/icons/check.png"" />"

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("xslt 1 label")
        dr(2) = "&#160;"
        If (xml_config_data.DefaultXslt = "1") Then
            If (xml_config_data.LogicalPathComplete("Xslt1") <> "") Then
                bValidDefaultXslt = True
                dr(2) += defaultCheck
            End If
        End If
        dr(1) = xml_config_data.LogicalPathComplete("Xslt1")
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("xslt 2 label")
        dr(2) = "&#160;"
        If (xml_config_data.DefaultXslt = "2") Then
            If (xml_config_data.LogicalPathComplete("Xslt2") <> "") Then
                bValidDefaultXslt = True
                dr(2) += defaultCheck
            End If
        End If
        dr(1) = xml_config_data.LogicalPathComplete("Xslt2")
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("xslt 3 label")
        dr(2) = "&#160;"
        If (xml_config_data.DefaultXslt = "3") Then
            If (xml_config_data.LogicalPathComplete("Xslt3") <> "") Then
                bValidDefaultXslt = True
                dr(2) += defaultCheck
            End If
        End If
        dr(1) = xml_config_data.LogicalPathComplete("Xslt3")
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("lbl XSLT packaged") & ":"
        dr(2) = "&#160;"
        If (xml_config_data.DefaultXslt = "0") Then
            dr(2) += defaultCheck
        Else
            If (Not (bValidDefaultXslt)) Then
                dr(2) += defaultCheck
            End If
        End If
        dr(1) = "&nbsp;"
        dt.Rows.Add(dr)

        If (xml_config_data.PackageXslt.Length > 0) Then
            'dr = dt.NewRow
            'dr(0) = ""
            'dr(1) = "REMOVE-ITEM"
            'dt.Rows.Add(dr)

            h3Xpaths.InnerText = "XPaths"

            Dim xPaths As String = String.Empty
            Dim item As Object
            Dim counter As Integer = 1
            For Each item In m_refContentApi.GetXPaths(xml_config_data.PackageXslt)
                xPaths += "<li" + IIf(counter Mod 2 = 0, " class=""stripe"">", ">") + Convert.ToString(item) & "</li>"
                counter = counter + 1
            Next
            litXpaths.Text = xPaths

            'dr = dt.NewRow
            'dr(0) = "XPaths:"
            'dr(2) = "&#160;"
            'Dim item As Object
            'dr(1) = ""
            'For Each item In m_refContentApi.GetXPaths(xml_config_data.PackageXslt)
            '    dr(1) += Convert.ToString(item) & "<br/>"
            'Next
            'dt.Rows.Add(dr)
        End If
        Dim dv As New DataView(dt)
        DisplayGrid.DataSource = dv
        DisplayGrid.DataBind()
    End Sub

#End Region

#Region "Process"

    Public Function Process_GetAttributes() As List(Of ProductTypeAttributeData)

        Dim AttributeList As New System.Collections.Generic.List(Of ProductTypeAttributeData)

        For i As Integer = 0 To (ucAttributesEdit.AttributeData.count - 1)

            If Not ucAttributesEdit.AttributeData(i).MarkedForDelete Then

                Dim Attribute As New ProductTypeAttributeData()

                Attribute.Id = ucAttributesEdit.AttributeData(i).Id
                Attribute.Name = ucAttributesEdit.AttributeData(i).Name
                Attribute.DataType = Util_GetAttributeType(ucAttributesEdit.AttributeData(i).Type)
                Attribute.DefaultValue = ucAttributesEdit.AttributeData(i).value

                AttributeList.Add(Attribute)

            End If

        Next

        Return AttributeList

    End Function

    Public Function Process_GetThumbnails() As List(Of ThumbnailDefaultData)

        Dim thumbnailDefaultList As New System.Collections.Generic.List(Of ThumbnailDefaultData)

        If ucMediaDefaultsEdit.ClientData IsNot Nothing Then

            For i As Integer = 0 To (ucMediaDefaultsEdit.ClientData.count - 1)

                If Not ucMediaDefaultsEdit.ClientData(i).MarkedForDelete Then

                    Dim thumbnailData As New ThumbnailDefaultData
                    thumbnailData.Id = ucMediaDefaultsEdit.ClientData(i).Id
                    thumbnailData.Title = ucMediaDefaultsEdit.ClientData(i).Name
                    thumbnailData.Width = ucMediaDefaultsEdit.ClientData(i).Width
                    thumbnailData.Height = ucMediaDefaultsEdit.ClientData(i).Height

                    thumbnailDefaultList.Add(thumbnailData)

                End If

            Next

        End If

        Return thumbnailDefaultList

    End Function

    Public Sub Process_EditProductType()
        Try
            _ProductType = New ProductType(m_refContentApi.RequestInformationRef)
            _ProductTypeData = _ProductType.GetItem(m_iID, True)

            _ProductTypeData.Title = txtTitle.Text
            _ProductTypeData.Description = txtDescription.Text
            _ProductTypeData.DefaultXslt = Request.Form("frm_xsltdefault").Replace("frm_xsltdefault", "")
            _ProductTypeData.Xslt1 = txt_xslt1.Text
            _ProductTypeData.Xslt2 = txt_xslt2.Text
            _ProductTypeData.Xslt3 = txt_xslt3.Text
            _ProductTypeData.PhysicalPath = Server.MapPath(m_refContentApi.XmlPath)
            _ProductTypeData.Attributes = Process_GetAttributes()
            _ProductTypeData.DefaultThumbnails = Process_GetThumbnails()
            If _ProductTypeData.EntryClass = CatalogEntryType.SubscriptionProduct Then _ProductTypeData.SubscriptionProvider = Request.Form("drp_SubscriptionProvider")

            _ProductType.Update(_ProductTypeData)
            Response.Redirect(_PageName & "?action=viewproducttype&id=" & m_iID, False)

        Catch specialCharacterEx As Ektron.Cms.Exceptions.SpecialCharactersException

            trError.Visible = True
            litErrorMessage.Text = String.Format(GetMessage("js alert product type title cant include"), Server.HtmlEncode("<,>"))
            Display_EditProductType()

        Catch ex As Exception
            trError.Visible = True
            litErrorMessage.Text = ex.Message.ToString()
            Display_EditProductType()
            ' Utilities.ShowError()

        End Try
    End Sub

    Public Sub Process_AddProductType()

        Try

            _ProductType = New ProductType(m_refContentApi.RequestInformationRef)
            _ProductTypeData = New ProductTypeData

            _ProductTypeData.EntryClass = Request.Form(drp_type.UniqueID)
            _ProductTypeData.Title = Request.Form(txtTitle.UniqueID)
            _ProductTypeData.Description = Request.Form(txtDescription.UniqueID)
            _ProductTypeData.EditXslt = ""
            _ProductTypeData.SaveXslt = ""
            _ProductTypeData.Xslt1 = ""
            _ProductTypeData.Xslt2 = ""
            _ProductTypeData.Xslt3 = ""
            _ProductTypeData.Xslt4 = ""
            _ProductTypeData.Xslt5 = ""
            _ProductTypeData.XmlSchema = ""
            _ProductTypeData.XmlNameSpace = ""
            _ProductTypeData.XmlAdvConfig = ""
            _ProductTypeData.DefaultXslt = 0
            _ProductTypeData.PhysicalPath = Server.MapPath(m_refContentApi.XmlPath)
            _ProductTypeData.Attributes = Process_GetAttributes()
            _ProductTypeData.DefaultThumbnails = Process_GetThumbnails()
            If _ProductTypeData.EntryClass = CatalogEntryType.SubscriptionProduct Then _ProductTypeData.SubscriptionProvider = Request.Form("drp_SubscriptionProvider")

            _ProductTypeData = _ProductType.Add(_ProductTypeData)
            Response.Redirect("../editdesign.aspx?action=EditPackage&type=product&id=" & _ProductTypeData.Id.ToString(), False)

        Catch specialCharacterEx As Ektron.Cms.Exceptions.SpecialCharactersException

            trError.Visible = True
            litErrorMessage.Text = String.Format(GetMessage("js alert product type title cant include"), Server.HtmlEncode("<,>"))

            Display_AddProductType()

        Catch ex As Exception

            trError.Visible = True
            litErrorMessage.Text = ex.Message.ToString()
            Display_AddProductType()

        End Try

    End Sub

#End Region

#Region "Helpers"

    Private Sub SetPagingUI()

        'paging ui
        divPaging.Visible = True

        litPage.Text = "Page"
        txtCurrentPage.Text = IIf(_CurrentPage = 0, "1", _CurrentPage.ToString())
        litOf.Text = "of"
        litTotalPages.Text = _TotalPages.ToString()

        hdnCurrentPage.Value = IIf(_CurrentPage = 0, "1", _CurrentPage.ToString())

        ibFirstPage.ImageUrl = Me.ApplicationPath & "/images/ui/icons/arrowheadFirst.png"
        ibFirstPage.AlternateText = "First Page"
        ibFirstPage.ToolTip = "First Page"

        ibPreviousPage.ImageUrl = Me.ApplicationPath & "/images/ui/icons/arrowheadLeft.png"
        ibPreviousPage.AlternateText = "Previous Page"
        ibPreviousPage.ToolTip = "Previous Page"

        ibNextPage.ImageUrl = Me.ApplicationPath & "/images/ui/icons/arrowheadRight.png"
        ibNextPage.AlternateText = "Next Page"
        ibNextPage.ToolTip = "Next Page"

        ibLastPage.ImageUrl = Me.ApplicationPath & "/images/ui/icons/arrowheadLast.png"
        ibLastPage.AlternateText = "Last Page"
        ibLastPage.ToolTip = "Last Page"

        ibPageGo.ImageUrl = Me.ApplicationPath & "/images/ui/icons/forward.png"
        ibPageGo.AlternateText = "Go To Page"
        ibPageGo.ToolTip = "Go To Page"
        ibPageGo.OnClientClick = " return GoToPage(this);"

    End Sub

    Protected Sub Util_SetLabels()
        Dim endColon() As Char = {":"}

        litTitleLabel.Text = GetMessage("generic title").TrimEnd(endColon) & ":"
        litIdLabel.Text = GetMessage("generic id").TrimEnd(endColon) & ":"
        litDescriptionLabel.Text = GetMessage("generic description").TrimEnd(endColon) & ":"
        litTypeLabel.Text = GetMessage("lbl product type class").TrimEnd(endColon) & ":"
        litDisplayLabel.Text = GetMessage("display info label").TrimEnd(endColon) & ":"
        litDisplayXsltPathMessage.Text = GetMessage("files prefixed with msg") & " " & m_refContentApi.XmlPath
        ltr_deflabel.Text = GetMessage("default label").TrimEnd(endColon)
        litXslt1Label.Text = GetMessage("xslt 1 label").TrimEnd(endColon) & ":"
        litXslt2Label.Text = GetMessage("xslt 2 label").TrimEnd(endColon) & ":"
        litXslt3Label.Text = GetMessage("xslt 3 label").TrimEnd(endColon) & ":"
        litXsltDefaultLabel.Text = "XSLT 0:"
        ltr_def.Text = GetMessage("default label")
        ltr_attrtype.Text = GetMessage("type label")
        ltr_attrname.Text = GetMessage("generic name")
        ltr_name.Text = GetMessage("lbl name").TrimEnd(endColon) & ":"
        ltr_width.Text = GetMessage("lbl width").TrimEnd(endColon) & ":"
        ltr_height.Text = GetMessage("lbl height").TrimEnd(endColon) & ":"
        ltr_subprovider.Text = GetMessage("lbl commerce subscription provider")

        drp_attrtype.Items.Add(New ListItem(Util_AttrText(ProductTypeAttributeDataType.String), "text"))
        drp_attrtype.Items.Add(New ListItem(Util_AttrText(ProductTypeAttributeDataType.Numeric), "number"))
        drp_attrtype.Items.Add(New ListItem(Util_AttrText(ProductTypeAttributeDataType.Boolean), "boolean"))
        drp_attrtype.Attributes.Add("onchange", "ChangeOption(this);")
        If _SelectedAttributeIndex > -1 Then drp_attrtype.SelectedIndex = _SelectedAttributeIndex
        txt_xslt1.Attributes.Add("onkeyup", "MakeNoVerify(this);")
        txt_xslt1.Attributes.Add("onkeypress", "return CheckKeyValue(event,'34');")
        txt_xslt2.Attributes.Add("onkeyup", "MakeNoVerify(this);")
        txt_xslt2.Attributes.Add("onkeypress", "return CheckKeyValue(event,'34');")
        txt_xslt3.Attributes.Add("onkeyup", "MakeNoVerify(this);")
        txt_xslt3.Attributes.Add("onkeypress", "return CheckKeyValue(event,'34');")
        Select Case Me.m_sPageAction
            Case "addthumbnail"
                Dim result As New System.Text.StringBuilder
                Me.SetTitleBarToMessage("lbl add thumbnail default")
                ltr_addthumbnail.Text = GetMessage("lbl add thumbnail default")

                result.Append("<table><tr>" & Environment.NewLine)
                result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'action');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'action');"" onmouseout=""this.className='menuRootItem'""><span class=""action"">" & m_refMsg.GetMessage("lbl Action") & "</span></td>" & Environment.NewLine)
                result.Append("</tr></table>" & Environment.NewLine)
                result.Append("<script tyle=""text/javascript"">" & Environment.NewLine)
                result.Append("    var actionmenu = new Menu( ""action"" );" & Environment.NewLine)
                result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & m_refContentApi.AppPath & "images/UI/Icons/save.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("generic add title") & """, function() { OnClick=validate();  } );" & Environment.NewLine)
                result.Append("    actionmenu.addItem(""&nbsp;<img height='16px' width='16px' src='" & m_refContentApi.AppPath & "images/UI/Icons/delete.png" & " ' />&nbsp;&nbsp;" & m_refMsg.GetMessage("generic cancel") & """, function() { OnClick=Close();  } );" & Environment.NewLine)
                result.Append("    MenuUtil.add( actionmenu );" & Environment.NewLine)
                result.Append("</script>" & Environment.NewLine)
            Case "addattribute"
                AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", "btn save", "btn save", "Onclick=""SubmitAttrForm(); return false;""")
                AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=""self.parent.ektb_remove();"" ")
                AddHelpButton(m_sPageAction)
            Case "editattribute"
                AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", "alt update product type", "btn update", "Onclick=""SubmitAttrForm(); return false;""")
                AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=""self.parent.ektb_remove();"" ")
                AddHelpButton(m_sPageAction)
            Case "addproducttype"

                SetTitleBarToMessage("btn add product type")

                AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", "lbl Select to continue", "btn save", "Onclick=""SubmitForm(); return false;""")
                AddBackButton(_PageName)
                AddHelpButton(m_sPageAction)

            Case "editproducttype"

                SetTitleBarToMessage("lbl edit product type")

                AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", "alt update product type", "btn update", "onclick=""SubmitForm(); return false;""")
                AddBackButton(_PageName & "?action=viewproducttype&id=" & m_iID.ToString())
                AddHelpButton(m_sPageAction)

            Case "viewproducttype"

                Dim product_type_data As XmlConfigData = Nothing
                Dim pkDisplay As String = String.Empty
                Dim PackageXslt As String = String.Empty

                SetTitleBarToMessage("lbl view product type msg")

                phTabDisplayInfo.Visible = True

                product_type_data = m_refContentApi.GetXmlConfiguration(m_iID)
                If (Not IsNothing(product_type_data)) Then
                    pkDisplay = product_type_data.PackageDisplayXslt
                    PackageXslt = product_type_data.PackageXslt
                End If

                AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/contentEdit.png", "producttypes.aspx" & "?action=editproducttype&id=" & m_iID, "alt edit button text (xml config)", "btn edit", "")
                If Not _IsUsed Then AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/delete.png", "producttypes.aspx" & "?action=deleteproducttype&id=" & m_iID, "generic delete title", "generic delete title", " onclick=""return confirm('" & GetMessage("js confirm del product type") & "');"" ")

                If (Request.Browser.Platform.IndexOf("Win") > -1) Then
                    If (Not IsNothing(product_type_data)) Then
                        If (Len(product_type_data.EditXslt) = 0) Or Len(pkDisplay) Then
                            AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/contentFormEdit.png", "../editdesign.aspx?action=EditPackage&type=product&id=" & m_iID, "alt Design mode Package", "btn data design", "")
                        End If
                    End If
                End If

                If Len(pkDisplay) Then
                    AddButtonwithMessages(m_refContentApi.AppPath & "Images/ui/icons/FileTypes/xsl.png", "../viewXslt.aspx?id=" & m_iID, "alt View the presentation Xslt", "btn view xslt", "")
                End If

                AddBackButton(_PageName)
                AddHelpButton(m_sPageAction)

            Case Else

                SetTitleBarToMessage("lbl view product types")

                If Not (_IsMac AndAlso _IsFF) Then

                    Dim newMenu As New workareamenu("file", GetMessage("lbl new"), m_refContentApi.AppPath & "images/UI/Icons/star.png")
                    newMenu.AddLinkItem(AppImgPath & "commerce/producttype1.gif", GetMessage("lbl product type xml config"), _PageName & "?action=addproducttype")
                    AddMenu(newMenu)
                    AddHelpButton("viewproducttypes")
                End If

        End Select
    End Sub

    Protected Sub Util_PopulateData()

        txtTitle.Text = _ProductTypeData.Title
        txtDescription.Text = _ProductTypeData.Description
        If _ProductTypeData.DefaultXslt = "1" And _ProductTypeData.Xslt1 <> "" Then frm_xsltdefault1.Checked = True
        If _ProductTypeData.DefaultXslt = "2" And _ProductTypeData.Xslt2 <> "" Then frm_xsltdefault2.Checked = True
        If _ProductTypeData.DefaultXslt = "3" And _ProductTypeData.Xslt3 <> "" Then frm_xsltdefault3.Checked = True
        If _ProductTypeData.DefaultXslt = "0" Then frm_xsltdefault0.Checked = True
        txt_xslt1.Text = _ProductTypeData.Xslt1
        txt_xslt2.Text = _ProductTypeData.Xslt2
        txt_xslt3.Text = _ProductTypeData.Xslt3

        If Not (_ProductTypeData.EntryClass = CatalogEntryType.SubscriptionProduct) Then tr_provider.Visible = False

    End Sub

    Protected Sub Util_XSLTLinks()
        ltr_verify1.Text = "<a href=""#"" onclick=""VerifyXslt('txt_xslt1'); return false""><img title=""" & GetMessage("alt text for xsl or schema verification") & """ alt=""" & GetMessage("alt text for xsl or schema verification") & """ src=""" & Me.ApplicationPath & "/images/ui/icons/contentValidate.png"" border=""0"" name=""img_txt_xslt1""></a>"
        ltr_verify2.Text = "<a href=""#"" onclick=""VerifyXslt('txt_xslt2'); return false""><img title=""" & GetMessage("alt text for xsl or schema verification") & """ alt=""" & GetMessage("alt text for xsl or schema verification") & """ src=""" & Me.ApplicationPath & "/images/ui/icons/contentValidate.png"" border=""0"" name=""img_txt_xslt2""></a>"
        ltr_verify3.Text = "<a href=""#"" onclick=""VerifyXslt('txt_xslt3'); return false""><img title=""" & GetMessage("alt text for xsl or schema verification") & """ alt=""" & GetMessage("alt text for xsl or schema verification") & """ src=""" & Me.ApplicationPath & "/images/ui/icons/contentValidate.png"" border=""0"" name=""img_txt_xslt3""></a>"
    End Sub

    Private Sub Util_AddProductTypeItems(Optional ByVal EntryClass As EkEnumeration.CatalogEntryType = CatalogEntryType.Product)

        Select Case Me.m_sPageAction

            Case "addproducttype"
                drp_type.Visible = True
                drp_type.Items.Add(New ListItem(m_refMsg.GetMessage("lbl commerce product"), EkEnumeration.CatalogEntryType.Product))
                drp_type.Items.Add(New ListItem(m_refMsg.GetMessage("lbl catalog kit"), EkEnumeration.CatalogEntryType.Kit))
                drp_type.Items.Add(New ListItem(m_refMsg.GetMessage("lbl commerce bundle"), EkEnumeration.CatalogEntryType.Bundle))
                drp_type.Items.Add(New ListItem(m_refMsg.GetMessage("lbl commerce subscription"), EkEnumeration.CatalogEntryType.SubscriptionProduct))

            Case Else ' edit and view
                drp_type.Visible = False

                If EntryClass = EkEnumeration.CatalogEntryType.Product Then litType.Text = m_refMsg.GetMessage("lbl commerce product")
                If EntryClass = EkEnumeration.CatalogEntryType.Kit Then litType.Text = m_refMsg.GetMessage("lbl catalog kit")
                If EntryClass = EkEnumeration.CatalogEntryType.Bundle Then litType.Text = m_refMsg.GetMessage("lbl commerce bundle")
                If EntryClass = EkEnumeration.CatalogEntryType.SubscriptionProduct Then litType.Text = m_refMsg.GetMessage("lbl commerce subscription")
        End Select

    End Sub

    Protected Function Util_ShowType(ByVal TypeId As Long) As String
        Dim sret As String = ""
        Select Case TypeId
            Case CatalogEntryType.SubscriptionProduct
                sret = GetMessage("lbl commerce subscription")
            Case CatalogEntryType.Bundle
                sret = GetMessage("lbl commerce bundle")
            Case CatalogEntryType.Kit
                sret = GetMessage("lbl catalog kit")
            Case Else
                sret = GetMessage("lbl commerce product")
        End Select
        Return sret
    End Function

    Protected Function Util_GetType(ByVal TypeId As Long) As String

        Dim sret As String = String.Empty

        Select Case TypeId
            Case CatalogEntryType.SubscriptionProduct
                sret = "subscription"
            Case CatalogEntryType.ComplexProduct
                sret = "complexproduct"
            Case CatalogEntryType.Bundle
                sret = "bundle"
            Case CatalogEntryType.Kit
                sret = "kit"
            Case CatalogEntryType.Product
                sret = "product"
            Case Else
                sret = "product"
        End Select
        Return sret
    End Function

    Protected Sub Util_CheckAccess()
        If Not m_refContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin) Then Throw New Exception(GetMessage("err not role commerce-admin"))
    End Sub

    Protected Function Util_AttrText(ByVal OptionValue As Integer) As String
        Dim sRet As String = ""
        Select Case OptionValue
            Case ProductTypeAttributeDataType.String
                sRet = GetMessage("text")
            Case ProductTypeAttributeDataType.Date
                sRet = GetMessage("lbl attr date")
            Case ProductTypeAttributeDataType.Boolean
                sRet = GetMessage("lbl attr boolean")
            Case ProductTypeAttributeDataType.Numeric
                sRet = GetMessage("lbl attr number")
            Case ProductTypeAttributeDataType.Numeric
                sRet = GetMessage("lbl attr double")
            Case ProductTypeAttributeDataType.Numeric
                sRet = GetMessage("lbl attr float")
            Case ProductTypeAttributeDataType.Numeric
                sRet = GetMessage("lbl attr integer")
            Case ProductTypeAttributeDataType.Numeric
                sRet = GetMessage("lbl attr long")
            Case ProductTypeAttributeDataType.Numeric
                sRet = GetMessage("lbl attr short")
            Case Else
                sRet = GetMessage("text")
        End Select
        Return sRet
    End Function

    Protected Function Util_GetAttributeType(ByVal attributeTypeValue As String) As ProductTypeAttributeDataType
        attributeTypeValue = attributeTypeValue.ToLower()
        Select Case attributeTypeValue
            Case "string"
                Return ProductTypeAttributeDataType.String
            Case "date"
                Return ProductTypeAttributeDataType.Date
            Case EkConstants.BOOLEAN_PROP
                Return ProductTypeAttributeDataType.Boolean
            Case "numeric"
                Return ProductTypeAttributeDataType.Numeric
            Case EkConstants.DOUBLE_PROP
                Return ProductTypeAttributeDataType.Numeric
            Case EkConstants.FLOAT_PROP
                Return ProductTypeAttributeDataType.Numeric
            Case EkConstants.INTEGER_PROP
                Return ProductTypeAttributeDataType.Numeric
            Case EkConstants.LONG_PROP
                Return ProductTypeAttributeDataType.Numeric
            Case EkConstants.SHORT_PROP
                Return ProductTypeAttributeDataType.Numeric
            Case Else
                Return ProductTypeAttributeDataType.String
        End Select
    End Function

#End Region

#Region "CSS, JS"

    Private Sub RegisterJS()

        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronThickBoxJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/dhtml/attribtableutil.js", "EktronTableUtilitiesJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/dhtml/mediatableutil.js", "EktronMediaTableUtilitiesJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.utils.string.js", "EktronStringUtilitiesJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.utils.cookie.js", "EktronCoookieUtilitiesJS")
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)

    End Sub

    Protected Sub Util_SetJs()
        Dim sbJS As New StringBuilder()
        sbJS.Append("<script type=""text/javascript"">").Append(Environment.NewLine)
        sbJS.Append(Me.JSLibrary.CheckKeyValue())
        sbJS.Append(Me.JSLibrary.ToggleDiv())
        sbJS.Append(Me.JSLibrary.URLEncode())
        sbJS.Append("	var bVerifying = false;").Append(Environment.NewLine)
        sbJS.Append("	var bPassed = true;").Append(Environment.NewLine)
        sbJS.Append("	var numOfVerifyLoops = 0;").Append(Environment.NewLine)
        sbJS.Append("	var strXslErrorMsg = """";").Append(Environment.NewLine)
        sbJS.Append("	var unique = 0;	").Append(Environment.NewLine)
        sbJS.Append("   function GetAttrLabel(attrtype) { ").Append(Environment.NewLine)
        sbJS.Append("       switch (attrtype) { ").Append(Environment.NewLine)
        sbJS.Append("             case ""boolean"" : ").Append(Environment.NewLine)
        sbJS.Append("                 attrtype = '").Append(GetMessage("lbl attr boolean")).Append("'; break; ").Append(Environment.NewLine)
        sbJS.Append("             case ""byte"" : ").Append(Environment.NewLine)
        sbJS.Append("                 attrtype = '").Append(GetMessage("lbl attr byte")).Append("'; break; ").Append(Environment.NewLine)
        sbJS.Append("             case ""double"" : ").Append(Environment.NewLine)
        sbJS.Append("                 attrtype = '").Append(GetMessage("lbl attr double")).Append("'; break; ").Append(Environment.NewLine)
        sbJS.Append("             case ""float"" : ").Append(Environment.NewLine)
        sbJS.Append("                 attrtype = '").Append(GetMessage("lbl attr float")).Append("'; break; ").Append(Environment.NewLine)
        sbJS.Append("             case ""integer"" : ").Append(Environment.NewLine)
        sbJS.Append("                 attrtype = '").Append(GetMessage("lbl attr integer")).Append("'; break; ").Append(Environment.NewLine)
        sbJS.Append("             case ""long"" : ").Append(Environment.NewLine)
        sbJS.Append("                 attrtype = '").Append(GetMessage("lbl attr long")).Append("'; break; ").Append(Environment.NewLine)
        sbJS.Append("             case ""short"" : ").Append(Environment.NewLine)
        sbJS.Append("                 attrtype = '").Append(GetMessage("lbl attr short")).Append("'; break; ").Append(Environment.NewLine)
        sbJS.Append("             case ""number"" : ").Append(Environment.NewLine)
        sbJS.Append("                 attrtype = '").Append(GetMessage("lbl attr number")).Append("'; break; ").Append(Environment.NewLine)
        sbJS.Append("             case ""date"" : ").Append(Environment.NewLine)
        sbJS.Append("                 attrtype = '").Append(GetMessage("lbl attr date")).Append("'; break; ").Append(Environment.NewLine)
        sbJS.Append("             case ""text"" : ").Append(Environment.NewLine)
        sbJS.Append("                 attrtype = '").Append(GetMessage("text")).Append("'; break; ").Append(Environment.NewLine)
        sbJS.Append("       } ").Append(Environment.NewLine)
        sbJS.Append("       return attrtype; ").Append(Environment.NewLine)
        sbJS.Append("   } ").Append(Environment.NewLine)
        Select Case Me.m_sPageAction ' m_strPageAction
            Case "addthumbnail"
                sbJS.Append("   function validate()").Append(Environment.NewLine)
                sbJS.Append("   {").Append(Environment.NewLine)
                sbJS.Append("           var width = document.getElementById('txtWidth').value;").Append(Environment.NewLine)
                sbJS.Append("           var height = document.getElementById('txtHeight').value;").Append(Environment.NewLine)
                sbJS.Append("           var name = document.getElementById('txtName').value;").Append(Environment.NewLine)
                sbJS.Append("           if(name == '')").Append(Environment.NewLine)
                sbJS.Append("           {").Append(Environment.NewLine)
                sbJS.Append("	    		alert(""").Append(GetMessage("js: alert name required")).Append(""");").Append(Environment.NewLine)
                sbJS.Append("               return false;").Append(Environment.NewLine)
                sbJS.Append("           }").Append(Environment.NewLine)
                sbJS.Append("           else if(isNaN(width) || isNaN(height) || (width == '') || (height == '') || (width == 0) || (height == 0))").Append(Environment.NewLine)
                sbJS.Append("           {").Append(Environment.NewLine)
                sbJS.Append("               alert(""").Append(GetMessage("js alert package dimension value")).Append(""");").Append(Environment.NewLine)
                sbJS.Append("               return false;").Append(Environment.NewLine)
                sbJS.Append("           }").Append(Environment.NewLine)
                sbJS.Append("           else").Append(Environment.NewLine)
                sbJS.Append("           {").Append(Environment.NewLine)
                sbJS.Append("               submitThumbnailForm();").Append(Environment.NewLine)
                sbJS.Append("           }").Append(Environment.NewLine)
                sbJS.Append("   }").Append(Environment.NewLine)
                sbJS.Append("   function Close(){").Append(Environment.NewLine)
                sbJS.Append("   self.parent.ektb_remove();").Append(Environment.NewLine)
                sbJS.Append("   }").Append(Environment.NewLine)
                sbJS.Append("   function submitThumbnailForm()").Append(Environment.NewLine)
                sbJS.Append("   {").Append(Environment.NewLine)
                sbJS.Append("           var width = document.getElementById('txtWidth').value;").Append(Environment.NewLine)
                sbJS.Append("           var height = document.getElementById('txtHeight').value;").Append(Environment.NewLine)
                sbJS.Append("           var name = document.getElementById('txtName').value;").Append(Environment.NewLine)
                If m_iID > 0 Then
                    sbJS.Append("		    parent.editRowInMediaTable(").Append(m_iID.ToString()).Append(", name, width, height); ").Append(Environment.NewLine)
                Else
                    sbJS.Append("           parent.addRowToMediaTable(null, 0, name, width, height); ").Append(Environment.NewLine)
                End If
                sbJS.Append("		        self.parent.ektb_remove(); ").Append(Environment.NewLine)
                sbJS.Append("}   ").Append(Environment.NewLine)
            Case "addattribute", "editattribute"
                sbJS.Append("	function SubmitAttrForm() {").Append(Environment.NewLine)
                sbJS.Append("	    var sAttrName = Trim(document.getElementById('").Append(txt_attrname.UniqueID).Append("').value); ").Append(Environment.NewLine)
                sbJS.Append("       var oDrpAttr = document.getElementById('").Append(drp_attrtype.UniqueID).Append("'); ").Append(Environment.NewLine)
                sbJS.Append("       var sAttrType = oDrpAttr.options[oDrpAttr.selectedIndex].value;").Append(Environment.NewLine)
                sbJS.Append("       var sAttrDef = ''; ").Append(Environment.NewLine)
                sbJS.Append("       switch(sAttrType) {").Append(Environment.NewLine)
                sbJS.Append("           case ""text"" :").Append(Environment.NewLine)
                sbJS.Append("               sAttrDef = Trim(document.getElementById('").Append(txt_textdefault.UniqueID).Append("').value); ").Append(Environment.NewLine)
                sbJS.Append("               break;").Append(Environment.NewLine)
                sbJS.Append("           case ""boolean"" :").Append(Environment.NewLine)
                sbJS.Append("               if (document.getElementById('").Append(chk_bool.UniqueID).Append("').checked) {sAttrDef = 'True'; } else { sAttrDef = 'False'; } ").Append(Environment.NewLine)
                sbJS.Append("               break;").Append(Environment.NewLine)
                sbJS.Append("           default :").Append(Environment.NewLine)
                sbJS.Append("               sAttrDef = Trim(document.getElementById('").Append(txt_number.UniqueID).Append("').value); ").Append(Environment.NewLine)
                sbJS.Append("               break;").Append(Environment.NewLine)
                sbJS.Append("		} ").Append(Environment.NewLine)
                sbJS.Append("		if (sAttrName == """") {").Append(Environment.NewLine)
                sbJS.Append("			alert(""").Append(GetMessage("js: alert name required")).Append(""");").Append(Environment.NewLine)
                sbJS.Append("			document.getElementById('").Append(txt_attrname.UniqueID).Append("').focus();").Append(Environment.NewLine)
                sbJS.Append("       } else if (!ValidNumeric()) { ").Append(Environment.NewLine)
                sbJS.Append("			alert(""").Append(GetMessage("res_isrch_iputnum")).Append(""");").Append(Environment.NewLine)
                sbJS.Append("			document.getElementById('").Append(txt_number.UniqueID).Append("').focus();").Append(Environment.NewLine)
                sbJS.Append("		} else {").Append(Environment.NewLine)
                If m_iID > 0 Then
                    sbJS.Append("		    parent.editRowInTable(").Append(m_iID.ToString()).Append(", sAttrName, sAttrType, sAttrDef); ").Append(Environment.NewLine)
                Else
                    sbJS.Append("           parent.addRowToTable(null, 0, sAttrName, sAttrType, sAttrDef); ").Append(Environment.NewLine)
                End If
                sbJS.Append("		        self.parent.ektb_remove(); ").Append(Environment.NewLine)
                sbJS.Append("		} ").Append(Environment.NewLine)
                sbJS.Append("	}").Append(Environment.NewLine)
                sbJS.Append("     function ChangeOption(selObj) {").Append(Environment.NewLine)
                sbJS.Append("        var selIndex = selObj.selectedIndex;").Append(Environment.NewLine)
                sbJS.Append("        switch(selObj.options[selIndex].value) {").Append(Environment.NewLine)
                sbJS.Append("            case ""text"" :").Append(Environment.NewLine)
                sbJS.Append("                ToggleDiv('divText', true);").Append(Environment.NewLine)
                sbJS.Append("                ToggleDiv('divNum', false);").Append(Environment.NewLine)
                sbJS.Append("                ToggleDiv('divChk', false);").Append(Environment.NewLine)
                sbJS.Append("                break;").Append(Environment.NewLine)
                sbJS.Append("            case ""boolean"" :").Append(Environment.NewLine)
                sbJS.Append("                ToggleDiv('divText', false);").Append(Environment.NewLine)
                sbJS.Append("                ToggleDiv('divNum', false);").Append(Environment.NewLine)
                sbJS.Append("                ToggleDiv('divChk', true);").Append(Environment.NewLine)
                sbJS.Append("                break;").Append(Environment.NewLine)
                sbJS.Append("            default :").Append(Environment.NewLine)
                sbJS.Append("                ToggleDiv('divText', false);").Append(Environment.NewLine)
                sbJS.Append("                ToggleDiv('divNum', true);").Append(Environment.NewLine)
                sbJS.Append("                ToggleDiv('divChk', false);").Append(Environment.NewLine)
                sbJS.Append("                break;").Append(Environment.NewLine)
                sbJS.Append("        }").Append(Environment.NewLine)
                sbJS.Append("    }").Append(Environment.NewLine)
                sbJS.Append("    function ValidNumeric() {").Append(Environment.NewLine)
                sbJS.Append("        var bRet = false").Append(Environment.NewLine)
                sbJS.Append("        var selObj = document.getElementById('drp_attrtype');").Append(Environment.NewLine)
                sbJS.Append("        var selIndex = selObj.selectedIndex;").Append(Environment.NewLine)
                sbJS.Append("        switch(selObj.options[selIndex].value) {").Append(Environment.NewLine)
                sbJS.Append("            case ""text"" :").Append(Environment.NewLine)
                sbJS.Append("            case ""boolean"" :").Append(Environment.NewLine)
                sbJS.Append("                bRet = true;").Append(Environment.NewLine)
                sbJS.Append("                break;").Append(Environment.NewLine)
                sbJS.Append("            default :").Append(Environment.NewLine)
                sbJS.Append("                var sNum = Trim(document.getElementById('txt_number').value);").Append(Environment.NewLine)
                sbJS.Append("                if (!(isNaN(parseFloat(sNum)))) { bRet = true; } ").Append(Environment.NewLine)
                sbJS.Append("                break;").Append(Environment.NewLine)
                sbJS.Append("        }").Append(Environment.NewLine)
                sbJS.Append("        return bRet;").Append(Environment.NewLine)
                sbJS.Append("    } ").Append(Environment.NewLine)
            Case Else
                sbJS.Append("	function SubmitForm() {").Append(Environment.NewLine)
                sbJS.Append("	    document.getElementById('").Append(txtTitle.UniqueID).Append("').value = Trim(document.getElementById('").Append(txtTitle.UniqueID).Append("').value);").Append(Environment.NewLine)
                sbJS.Append("		if(document.getElementById('").Append(txtDescription.UniqueID).Append("').value.indexOf('<') > -1 || document.getElementById('").Append(txtDescription.UniqueID).Append("').value.indexOf('>') > -1) {").Append(Environment.NewLine)
                sbJS.Append("			alert(""").Append(String.Format(GetMessage("js alert product type desc cant include"), "<, >")).Append(""");").Append(Environment.NewLine)
                sbJS.Append("			$ektron('.tabContainer').tabs('select',0);document.getElementById('").Append(txtDescription.UniqueID).Append("').focus(); return false;").Append(Environment.NewLine)
                sbJS.Append("		} ").Append(Environment.NewLine)
                sbJS.Append("		if (document.getElementById('").Append(txtTitle.UniqueID).Append("').value == """") {").Append(Environment.NewLine)
                sbJS.Append("			alert(""").Append(GetMessage("js: alert title required")).Append(""");").Append(Environment.NewLine)
                sbJS.Append("			$ektron('.tabContainer').tabs('select',0);document.getElementById('").Append(txtTitle.UniqueID).Append("').focus();").Append(Environment.NewLine)
                sbJS.Append("		} else if (document.getElementById('").Append(txtTitle.UniqueID).Append("').value.indexOf('<') > -1 || document.getElementById('").Append(txtTitle.UniqueID).Append("').value.indexOf('>') > -1) {").Append(Environment.NewLine)
                sbJS.Append("			alert(""").Append(String.Format(GetMessage("js alert product type title cant include"), "<, >")).Append(""");").Append(Environment.NewLine)
                sbJS.Append("			$ektron('.tabContainer').tabs('select',0);document.getElementById('").Append(txtTitle.UniqueID).Append("').focus();").Append(Environment.NewLine)
                sbJS.Append("		} else {").Append(Environment.NewLine)
                sbJS.Append("		    document.forms[0].submit();").Append(Environment.NewLine)
                sbJS.Append("		} ").Append(Environment.NewLine)
                sbJS.Append("	}").Append(Environment.NewLine)
                sbJS.Append("	function VerifyXsltCallback (formFieldName, displayMsg) {").Append(Environment.NewLine)
                sbJS.Append("		if (bVerifying) {").Append(Environment.NewLine)
                sbJS.Append("			if (numOfVerifyLoops < 350) {").Append(Environment.NewLine)
                sbJS.Append("				setTimeout(""VerifyXsltCallback('"" + formFieldName + ""', "" + displayMsg + "")"", 100);").Append(Environment.NewLine)
                sbJS.Append("				numOfVerifyLoops++;").Append(Environment.NewLine)
                sbJS.Append("				return false;").Append(Environment.NewLine)
                sbJS.Append("			}").Append(Environment.NewLine)
                sbJS.Append("		}").Append(Environment.NewLine)
                sbJS.Append("		bVerifying = false;").Append(Environment.NewLine)
                sbJS.Append("		if (bPassed) {").Append(Environment.NewLine)
                sbJS.Append("			if (displayMsg) {").Append(Environment.NewLine)
                sbJS.Append("				document.images[""img_"" + formFieldName].src=""").Append(Me.ApplicationPath).Append("/images/ui/icons/check.png"";").Append(Environment.NewLine)
                sbJS.Append("				alert(""Verification succeeded."");").Append(Environment.NewLine)
                sbJS.Append("			}").Append(Environment.NewLine)
                sbJS.Append("			return (true);").Append(Environment.NewLine)
                sbJS.Append("		}").Append(Environment.NewLine)
                sbJS.Append("		else {").Append(Environment.NewLine)
                sbJS.Append("			if (displayMsg) {").Append(Environment.NewLine)
                sbJS.Append("				document.images[""img_"" + formFieldName].src=""").Append(Me.ApplicationPath).Append("/images/ui/icons/error.png"";").Append(Environment.NewLine)
                sbJS.Append("				alert (strXslErrorMsg);").Append(Environment.NewLine)
                sbJS.Append("			}").Append(Environment.NewLine)
                sbJS.Append("			return (false);").Append(Environment.NewLine)
                sbJS.Append("		}").Append(Environment.NewLine)
                sbJS.Append("	}").Append(Environment.NewLine)
                sbJS.Append("	function VerifyXslt(formFieldName) {").Append(Environment.NewLine)
                sbJS.Append("		var extension;").Append(Environment.NewLine)
                sbJS.Append("		var urlExtension;").Append(Environment.NewLine)
                sbJS.Append("		var thisExtension;").Append(Environment.NewLine)
                sbJS.Append("		var xslPath;").Append(Environment.NewLine)
                sbJS.Append("		").Append(Environment.NewLine)
                sbJS.Append("		if (bVerifying) {").Append(Environment.NewLine)
                sbJS.Append("			return false;").Append(Environment.NewLine)
                sbJS.Append("		}").Append(Environment.NewLine)
                sbJS.Append("		document.forms.xmlconfiguration[formFieldName].value = Trim(document.forms.xmlconfiguration[formFieldName].value);").Append(Environment.NewLine)
                sbJS.Append("		xslPath = document.forms.xmlconfiguration[formFieldName].value;").Append(Environment.NewLine)
                sbJS.Append("		if (xslPath.length == 0) {").Append(Environment.NewLine)
                sbJS.Append("			return false;").Append(Environment.NewLine)
                sbJS.Append("		}		").Append(Environment.NewLine)
                sbJS.Append("		extension = xslPath.split(""?"");").Append(Environment.NewLine)
                sbJS.Append("		extension = extension[0].split(""."");").Append(Environment.NewLine)
                sbJS.Append("		thisExtension = extension[extension.length - 1];").Append(Environment.NewLine)
                sbJS.Append("		if (((thisExtension == ""asp"") || (thisExtension == ""aspx"")").Append(Environment.NewLine)
                sbJS.Append("			|| (thisExtension == ""cfm"") || (thisExtension == ""php""))").Append(Environment.NewLine)
                sbJS.Append("			&& ((xslPath.substring(0,7) != ""http://"") && (xslPath.substring(0,8) != ""https://""))) {").Append(Environment.NewLine)
                sbJS.Append("			").Append(Environment.NewLine)
                sbJS.Append("			alert(""Dynamically generated XSLT or schema files must use a fully qualified Web path.\nExample\nhttp://localhost/xmlfiles/myxslt.aspx"");").Append(Environment.NewLine)
                sbJS.Append("			return false;").Append(Environment.NewLine)
                sbJS.Append("		}").Append(Environment.NewLine)
                sbJS.Append("		unique++;").Append(Environment.NewLine)
                sbJS.Append("		if (document.all) {").Append(Environment.NewLine)
                sbJS.Append("			document.all[""iframe1""].src=""../xml_verify.aspx?path="" + escape(xslPath);").Append(Environment.NewLine)
                sbJS.Append("			").Append(Environment.NewLine)
                sbJS.Append("		}").Append(Environment.NewLine)
                sbJS.Append("		else if (document.getElementById) {").Append(Environment.NewLine)
                sbJS.Append("			document.getElementById(""iframe1"").src=""../xml_verify.aspx?path="" + escape(xslPath) + ""&num="" + unique;").Append(Environment.NewLine)
                sbJS.Append("		}").Append(Environment.NewLine)
                sbJS.Append("		else {").Append(Environment.NewLine)
                sbJS.Append("			document.layers[""iframe1""].load(""../xml_verify.aspx?path="" + escape(xslPath) + ""&num="" + unique, ""100%"");").Append(Environment.NewLine)
                sbJS.Append("		}").Append(Environment.NewLine)
                sbJS.Append("		bVerifying = true;").Append(Environment.NewLine)
                sbJS.Append("		bPassed = false;").Append(Environment.NewLine)
                sbJS.Append("		numOfVerifyLoops = 0;").Append(Environment.NewLine)
                sbJS.Append("		strXslErrorMsg = ""Timeout"";").Append(Environment.NewLine)
                sbJS.Append("		setTimeout(""VerifyXsltCallback('"" + formFieldName + ""', "" + true + "")"", 100);").Append(Environment.NewLine)
                sbJS.Append("	}").Append(Environment.NewLine)
                sbJS.Append("	function MakeNoVerify (formName, item, keys) {").Append(Environment.NewLine)
                'sbJS.Append("		if (document.forms.xmlconfiguration[formName.name + ""_length""].value != formName.value) {").Append(Environment.NewLine)
                sbJS.Append("			document.images[""img_"" + formName.name].src = """).Append(Me.ApplicationPath).Append("/images/ui/icons/contentValidate.png"";").Append(Environment.NewLine)
                'sbJS.Append("		}").Append(Environment.NewLine)
                'sbJS.Append("		document.forms.xmlconfiguration[formName.name + ""_length""].value = formName.value;").Append(Environment.NewLine)
                sbJS.Append("	}").Append(Environment.NewLine)
                sbJS.Append("   function AddAttribute() { ").Append(Environment.NewLine)
                sbJS.Append("       ektb_show('','producttypes.aspx?action=addAttribute&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true', null); ").Append(Environment.NewLine)
                sbJS.Append("   } ").Append(Environment.NewLine)
                sbJS.Append("   function AddThumbnail() { ").Append(Environment.NewLine)
                sbJS.Append("       ektb_show('','producttypes.aspx?action=addThumbnail&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true', null); ").Append(Environment.NewLine)
                sbJS.Append("   } ").Append(Environment.NewLine)
                sbJS.Append("    function EditAttribute() {").Append(Environment.NewLine)
                sbJS.Append("        var oAttr = getCheckedObj();").Append(Environment.NewLine)
                sbJS.Append("        if (oAttr == null) {").Append(Environment.NewLine)
                sbJS.Append("            alert('").Append(GetMessage("js please sel attr")).Append("');").Append(Environment.NewLine)
                sbJS.Append("        } else {").Append(Environment.NewLine)
                sbJS.Append("            ektb_show('','producttypes.aspx?action=editAttribute&id=' + ").Append(JSLibrary.URLEncodeFunctionName).Append("(oAttr.one.data) + '&name=' + ").Append(JSLibrary.URLEncodeFunctionName).Append("(oAttr.two.data) + '&type=' + ").Append(JSLibrary.URLEncodeFunctionName).Append("(oAttr.seven.value) + '&def=' + ").Append(JSLibrary.URLEncodeFunctionName).Append("(oAttr.four.data) + '&EkTB_iframe=true&height=300&width=500&modal=true', null);").Append(Environment.NewLine)
                sbJS.Append("        }").Append(Environment.NewLine)
                sbJS.Append("    }").Append(Environment.NewLine)
                sbJS.Append("    function DeleteAttribute() {").Append(Environment.NewLine)
                sbJS.Append("        var iAttr = getCheckedInt(false);").Append(Environment.NewLine)
                sbJS.Append("        if (iAttr == -1) {").Append(Environment.NewLine)
                sbJS.Append("            alert('").Append(GetMessage("js please sel attr")).Append("');").Append(Environment.NewLine)
                sbJS.Append("        } else {").Append(Environment.NewLine)
                sbJS.Append("            deleteChecked();").Append(Environment.NewLine)
                sbJS.Append("        }").Append(Environment.NewLine)
                sbJS.Append("    }").Append(Environment.NewLine)
                sbJS.Append("    function DeleteMediaThumbnail() {").Append(Environment.NewLine)
                sbJS.Append("        var iAttr = getMediaCheckedInt(false);").Append(Environment.NewLine)
                sbJS.Append("        if (iAttr == -1) {").Append(Environment.NewLine)
                sbJS.Append("            alert('").Append(GetMessage("js please sel media default")).Append("');").Append(Environment.NewLine)
                sbJS.Append("        } else {").Append(Environment.NewLine)
                sbJS.Append("            deleteCheckedMedia();").Append(Environment.NewLine)
                sbJS.Append("        }").Append(Environment.NewLine)
                sbJS.Append("    }").Append(Environment.NewLine)
        End Select
        sbJS.Append("</script>").Append(Environment.NewLine)
        ltr_js.Text = sbJS.ToString()
    End Sub

    Private Sub RegisterCSS()

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronThickBoxCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/explorer/css/com.ektron.ui.menu.css", "EktronMenuCss")
        Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/csslib/tables/tableutil.css", "EktronTableUtilitiesCss")
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)

    End Sub

#End Region

End Class
