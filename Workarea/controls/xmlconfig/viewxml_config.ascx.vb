Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Commerce

Partial Class viewxml_config
    Inherits System.Web.UI.UserControl

    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_strPageAction As String = ""
    Protected AppImgPath As String = ""
    Protected EnableMultilingual As Integer = 0
    Protected ContentLanguage As Integer = 0
    Protected m_refContentApi As ContentAPI
    Protected m_refContent As Ektron.Cms.Content.EkContent
    Protected m_strOrderBy As String = "title"
    Protected ConfigId As Long = 0
    Protected m_bIsMac As Boolean
    Protected ProductTypeAPI As ProductType = Nothing
    Protected bIsAdmin As Boolean = False


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        RegisterResources()
        m_refContentApi = New ContentAPI
        m_refMsg = m_refContentApi.EkMsgRef
        AppImgPath = m_refContentApi.AppImgPath
        ContentLanguage = m_refContentApi.ContentLanguage
        m_refContent = m_refContentApi.EkContentRef
        SetServerJSVariables()
        If (m_refContent.IsAllowed(0, 0, "users", "IsAdmin") = True OrElse m_refContent.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminXmlConfig, m_refContent.RequestInformation.UserId) = True) Then
            bIsAdmin = True
        End If

        If (Request.Browser.Platform.IndexOf("Win") = -1) Then
            m_bIsMac = True
        Else
            m_bIsMac = False
        End If

        EnableMultilingual = m_refContentApi.EnableMultilingual
        If (Not (IsNothing(Request.QueryString("action")))) Then
            m_strPageAction = Request.QueryString("action")
            If (m_strPageAction.Length > 0) Then
                m_strPageAction = m_strPageAction.ToLower
            End If
        End If
        If (Request.QueryString("orderby") <> "") Then
            m_strOrderBy = Request.QueryString("orderby")
        End If
    End Sub

#Region "XmlConfigs"
    Public Function ViewXmlConfiguration() As Boolean
        Dim xml_config_data As XmlConfigData
        TR_ViewAll.Visible = False
        TR_View.Visible = True
        If (Not (IsNothing(Request.QueryString("id")))) Then
            ConfigId = Request.QueryString("id")
        End If
        xml_config_data = m_refContentApi.GetXmlConfiguration(ConfigId)

        ViewXmlConfigToolBar(xml_config_data)

        PopulatePropertiesGrid(xml_config_data)
        PopulateDisplayGrid(xml_config_data)
        If (xml_config_data.PackageDisplayXslt.Length > 0) Then
            PopulatePreviewGrid(xml_config_data)
        End If
    End Function
    Private Sub PopulatePropertiesGrid(ByVal xml_config_data As XmlConfigData)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.ItemStyle.CssClass = "label"
        PropertiesGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "VALUE"
        colBound.ItemStyle.CssClass = "readOnlyValue"
        PropertiesGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        Dim bValidDefaultXslt As Boolean = False
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("VALUE", GetType(String)))

        dr = dt.NewRow
        dr(0) = "<strong class='headerRow'>" & m_refMsg.GetMessage("general information") & "</strong>"
        dr(1) = "REMOVE-ITEM"
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("generic title label")
        dr(1) = xml_config_data.Title
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("id label")
        dr(1) = xml_config_data.Id
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("description label")
        dr(1) = xml_config_data.Description
        dt.Rows.Add(dr)

        If (xml_config_data.PackageDisplayXslt.Length > 0) Then
            ' do nothing
        Else

            dr = dt.NewRow
            dr(0) = "<strong class='headerRow'>" & m_refMsg.GetMessage("editor info label") & "</strong>"
            dr(1) = "REMOVE-ITEM"
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("edit xslt label")
            dr(1) = xml_config_data.LogicalPathComplete("EditXslt")
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("save xslt label")
            dr(1) = xml_config_data.LogicalPathComplete("SaveXslt")
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("advanced config label")
            dr(1) = xml_config_data.LogicalPathComplete("XmlAdvConfig")
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = "<strong class='headerRow'>" & m_refMsg.GetMessage("validation info label") & "</strong>"
            dr(1) = "REMOVE-ITEM"
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("xml schema label")
            dr(1) = xml_config_data.LogicalPathComplete("XmlSchema")
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr(0) = m_refMsg.GetMessage("target namespace label")
            dr(1) = xml_config_data.LogicalPathComplete("XmlNameSpace")
            dt.Rows.Add(dr)
        End If


        Dim dv As New DataView(dt)
        PropertiesGrid.DataSource = dv
        PropertiesGrid.DataBind()
    End Sub
    Private Sub PopulatePreviewGrid(ByVal xml_config_data As XmlConfigData)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = ""
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Height = Unit.Empty
        colBound.ItemStyle.Height = Unit.Empty
        PreviewGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))

        dr = dt.NewRow
        dr(0) = "<strong> " & m_refMsg.GetMessage("lbl Preview XSLT on empty XML document") & "</strong>"
        dt.Rows.Add(dr)

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
        colBound.HeaderText = ""
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Height = Unit.Empty
        colBound.ItemStyle.Height = Unit.Empty
        colBound.ItemStyle.CssClass = "label"
        DisplayGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "VALUE"
        colBound.HeaderText = ""
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Height = Unit.Empty
        colBound.ItemStyle.Height = Unit.Empty
        DisplayGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        Dim bValidDefaultXslt As Boolean = False
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("VALUE", GetType(String)))

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("xslt 1 label")
        If (xml_config_data.DefaultXslt = "1") Then
            If (xml_config_data.LogicalPathComplete("Xslt1") <> "") Then
                bValidDefaultXslt = True
                dr(0) += "*"
            End If
        End If
        dr(1) = xml_config_data.LogicalPathComplete("Xslt1")
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("xslt 2 label")
        If (xml_config_data.DefaultXslt = "2") Then
            If (xml_config_data.LogicalPathComplete("Xslt2") <> "") Then
                bValidDefaultXslt = True
                dr(0) += "*"
            End If
        End If
        dr(1) = xml_config_data.LogicalPathComplete("Xslt2")
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("xslt 3 label")
        If (xml_config_data.DefaultXslt = "3") Then
            If (xml_config_data.LogicalPathComplete("Xslt3") <> "") Then
                bValidDefaultXslt = True
                dr(0) += "*"
            End If
        End If
        dr(1) = xml_config_data.LogicalPathComplete("Xslt3")
        dt.Rows.Add(dr)

        dr = dt.NewRow
        dr(0) = m_refMsg.GetMessage("lbl XSLT packaged")
        If (xml_config_data.DefaultXslt = "0") Then
            dr(0) += "*"
        Else
            If (Not (bValidDefaultXslt)) Then
                dr(0) += "*"
            End If
        End If
        dr(1) = "&nbsp;"
        dt.Rows.Add(dr)

        If (xml_config_data.PackageXslt.Length > 0) Then
            dr = dt.NewRow

            dr = dt.NewRow
            dr(0) = "<strong class='headerRow'>" & m_refMsg.GetMessage("lbl xpaths") & "</strong>"
            dr(1) = "REMOVE-ITEM"
            dt.Rows.Add(dr)

            Dim item As Object
            dr = dt.NewRow
            dr(0) = ""
            For Each item In m_refContentApi.GetXPaths(xml_config_data.PackageXslt)
                dr(0) += "<label class=""addLeft"">" & Convert.ToString(item) & "</label><br/>"
            Next
            dr(1) = "REMOVE-ITEM"
            dt.Rows.Add(dr)
        End If
        Dim dv As New DataView(dt)
        DisplayGrid.DataSource = dv
        DisplayGrid.DataBind()
    End Sub
    Protected Sub DisplayGrid_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        Select Case e.Item.ItemType
            Case ListItemType.AlternatingItem, ListItemType.Item
                If (e.Item.Cells(1).Text.Equals("REMOVE-ITEM")) Then
                    e.Item.Cells(0).ColumnSpan = 2
                    e.Item.Cells.RemoveAt(1)
                End If
        End Select
    End Sub
    Private Sub ViewXmlConfigToolBar(ByVal xml_config_data As XmlConfigData)
        Dim result As New System.Text.StringBuilder
        Dim pkDisplay As String = xml_config_data.PackageDisplayXslt 'cXmlCollection("PackageDisplayXslt")
        Dim PackageXslt As String = xml_config_data.PackageXslt 'cXmlCollection("PackageXslt")
        Dim caller As String = Request.QueryString("caller")
        Dim eIntranet As Boolean = False
        result.Append("<table><tr>")
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view xml config msg") & " """ & xml_config_data.Title & """")
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.MembershipUsers) AndAlso xml_config_data.Id = 15) Then
                eIntranet = True
        End If
        If caller = "" Then
            If (Not m_bIsMac AndAlso bIsAdmin) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/add.png", "xml_config.aspx?action=NewInheritConfiguration&id=" & ConfigId & "", m_refMsg.GetMessage("alt Create a new xml configuration based on this configuration"), m_refMsg.GetMessage("btn add xml"), ""))
            End If
            If bIsAdmin Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentEdit.png", "xml_config.aspx?action=EditXmlConfiguration&id=" & ConfigId & "", m_refMsg.GetMessage("alt edit button text (xml config)"), m_refMsg.GetMessage("btn edit"), ""))
            End If

            If (Not m_bIsMac AndAlso bIsAdmin) Then
                If (Len(xml_config_data.EditXslt) = 0) Or Len(pkDisplay) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/ui/icons/contentSmartFormEdit.png", "editdesign.aspx?action=EditPackage&id=" & ConfigId & "", m_refMsg.GetMessage("alt Design mode Package"), m_refMsg.GetMessage("btn data design"), ""))
                End If
            End If
            If Len(pkDisplay) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/ui/icons/FileTypes/xsl.png", "viewXslt.aspx?id=" & ConfigId & "", m_refMsg.GetMessage("alt View the presentation Xslt"), m_refMsg.GetMessage("btn view xslt"), ""))
            End If
            If bIsAdmin AndAlso Not eIntranet Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/delete.png", "xml_config.aspx?action=DeleteXmlConfiguration&id=" & ConfigId & "", m_refMsg.GetMessage("alt delete button text (xml config)"), m_refMsg.GetMessage("btn delete"), "OnClick=""return ConfirmDelete();"""))
            End If
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "xml_config.aspx?action=ViewAllXmlConfigurations", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/cancel.png", "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn cancel"), "OnClick=""javascript:self.close();"""))
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Public Function ViewAllXmlConfigurations() As Boolean
        TR_ViewAll.Visible = True
        TR_View.Visible = False
        Dim xml_config_list As XmlConfigData()
        xml_config_list = m_refContentApi.GetAllXmlConfigurations(m_strOrderBy)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = "<a href=""xml_config.aspx?action=ViewAllXmlConfigurations&orderby=title"">" & m_refMsg.GetMessage("generic Title") & "</a>"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderStyle.Width = Unit.Percentage(20)
        colBound.ItemStyle.Width = Unit.Percentage(20)
        XMLList.Columns.Add(colBound)


        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = "<a href=""xml_config.aspx?action=ViewAllXmlConfigurations&orderby=id"">" & m_refMsg.GetMessage("generic ID") & "</a>"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderStyle.Width = Unit.Percentage(1)
        colBound.ItemStyle.Width = Unit.Percentage(1)
        XMLList.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DATE"
        colBound.HeaderText = "<a href=""xml_config.aspx?action=ViewAllXmlConfigurations&orderby=LastEditDate"">" & m_refMsg.GetMessage("generic Date Modified") & "</a>"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.HeaderStyle.CssClass = "title-header"
        colBound.HeaderStyle.Width = Unit.Percentage(10)
        colBound.ItemStyle.Width = Unit.Percentage(10)
        XMLList.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "EDITOR"
        colBound.HeaderText = "<a href=""xml_config.aspx?action=ViewAllXmlConfigurations&orderby=editor"">" & m_refMsg.GetMessage("generic Last Editor") & "</a>"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.HeaderStyle.CssClass = "title-header"
        XMLList.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("DATE", GetType(String)))
        dt.Columns.Add(New DataColumn("EDITOR", GetType(String)))
        Dim i As Integer = 0
        Dim strTemp As New System.Text.StringBuilder
        If (Not (IsNothing(xml_config_list))) Then
            For i = 0 To xml_config_list.Length - 1
                dr = dt.NewRow
                dr(0) = "<a href=""xml_config.aspx?action=ViewXmlConfiguration&id=" & xml_config_list(i).Id & """ title=""" & m_refMsg.GetMessage("view xml config props") & """>" & xml_config_list(i).Title & "</a>"
                dr(1) = xml_config_list(i).Id
                dr(2) = xml_config_list(i).DisplayLastEditDate
                dr(3) = xml_config_list(i).EditorLastName & ", " & xml_config_list(i).EditorFirstName
                dt.Rows.Add(dr)
            Next
        End If

        Dim dv As New DataView(dt)
        XMLList.DataSource = dv
        XMLList.DataBind()
        ViewAllXmlToolBar()
    End Function
    Private Sub ViewAllXmlToolBar()
        Dim result As New System.Text.StringBuilder
        Try
            result.Append("<table><tr>")
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view xml configs msg"))
            If (Not m_bIsMac AndAlso bIsAdmin) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/add.png", "xml_config.aspx?action=AddXmlConfigurationV4", m_refMsg.GetMessage("alt add button text (xml config)"), m_refMsg.GetMessage("btn add xml"), ""))
            End If
            result.Append("<td>")
            result.Append(m_refStyle.GetHelpButton(m_strPageAction))
            result.Append("</td>")
            result.Append("</tr></table>")
            htmToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region

#Region "Product Types"
    Dim m_sProductTypePage As String = "producttypes.aspx"

    Public Function ViewProductType() As Boolean
        Dim product_type_data As XmlConfigData
        TR_ViewAll.Visible = False
        TR_View.Visible = True
        If (Not (IsNothing(Request.QueryString("id")))) Then
            ConfigId = Request.QueryString("id")
        End If
        product_type_data = m_refContentApi.GetXmlConfiguration(ConfigId)

        ViewProductTypeToolBar(product_type_data)

        PopulatePropertiesGrid(product_type_data)
        PopulateDisplayGrid(product_type_data)
        If (product_type_data.PackageDisplayXslt.Length > 0) Then
            PopulatePreviewGrid(product_type_data)
        End If
    End Function
    Private Sub ViewProductTypeToolBar(ByVal product_type_data As XmlConfigData)
        Dim result As New System.Text.StringBuilder
        Dim pkDisplay As String = product_type_data.PackageDisplayXslt 'cXmlCollection("PackageDisplayXslt")
        Dim PackageXslt As String = product_type_data.PackageXslt 'cXmlCollection("PackageXslt")
        Dim caller As String = Request.QueryString("caller")
        result.Append("<table><tr>")
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl view product type msg") & " """ & product_type_data.Title & """")
        If caller = "" Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/contentEdit.png", m_sProductTypePage & "?action=editproducttype&id=" & ConfigId & "", m_refMsg.GetMessage("alt edit button text (xml config)"), m_refMsg.GetMessage("btn edit"), ""))
            If (Not m_bIsMac) Then
                If (Len(product_type_data.EditXslt) = 0) Or Len(pkDisplay) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/ui/icons/contentSmartFormEdit.png", "../editdesign.aspx?action=EditPackage&type=product&id=" & ConfigId & "", m_refMsg.GetMessage("alt Design mode Package"), m_refMsg.GetMessage("btn data design"), ""))
                End If
                ' result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/add.png", m_sProductTypePage & "?action=newinheritproducttype&id=" & ConfigId & "", m_refMsg.GetMessage("alt Create a new xml configuration based on this configuration"), m_refMsg.GetMessage("btn add xml"), ""))
            End If
            If Len(pkDisplay) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/ui/icons/FileTypes/xsl.png", "../viewXslt.aspx?id=" & ConfigId & "", m_refMsg.GetMessage("alt View the presentation Xslt"), m_refMsg.GetMessage("btn view xslt"), ""))
            End If
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/delete.png", m_sProductTypePage & "?action=deleteproducttype&id=" & ConfigId & "", m_refMsg.GetMessage("alt delete button text (xml config)"), m_refMsg.GetMessage("btn delete"), "OnClick=""return ConfirmDelete();"""))
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", m_sProductTypePage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/cancel.png", "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn cancel"), "OnClick=""javascript:self.close();"""))
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
#End Region
    Private Sub RegisterResources()
        'CSS
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronUITabsCss)

        'JS
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
    End Sub
    Private Sub SetServerJSVariables()
        ltr_delXMLConfig.Text = m_refMsg.GetMessage("js: confirm xml config delete")
    End Sub
End Class
