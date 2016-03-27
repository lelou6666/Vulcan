Imports Ektron
Imports Ektron.Cms.Workarea
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports System.data
Imports System.Web.HttpRequest
Imports System.Web.UI.page
Partial Class Commerce_shipping_packagesize
    Inherits workareabase

    Protected criteria As New Ektron.Cms.Common.Criteria(Of CountryProperty)(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
    Protected m_refPackage As PackageApi = Nothing
    Protected m_sPageName As String = "packagesize.aspx"
    Protected _currentPageNumber As Integer = 1
    Protected m_bIsDefault As Boolean = False
    Protected TotalPagesNumber As Integer = 1
    Protected measurementSystem As String = Me.m_refContentApi.RequestInformationRef.MeasurementSystem

#Region "Page Function"
    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        Utilities.ValidateUserLogin()
        CommerceLibrary.CheckCommerceAdminAccess()

        Try
            Util_RegisterResources()

            Select Case MyBase.m_sPageAction
                Case "del"
                    Process_Delete()
                Case "addedit"
                    If Page.IsPostBack Then
                        Process_AddEdit()
                    Else
                        Display_AddEdit()
                    End If
                Case "view"
                    Display_View()
                Case Else ' "viewall"
                    If Page.IsPostBack = False Then
                        Display_View_All()
                    End If
            End Select
            SetLabels()

        Catch ex As Exception

            Utilities.ShowError(ex.Message)

        End Try

    End Sub
#End Region
#Region "Display"
    Protected Sub Display_View_All()
        Dim PackageCriteria As New Ektron.Cms.Common.Criteria(Of PackageProperty)(PackageProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim packagelist As New System.Collections.Generic.List(Of PackageData)
        m_refPackage = New PackageApi()
        Dim i As Integer = 0

        dg_package.AutoGenerateColumns = False
        dg_package.Columns.Clear()

        PackageCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        PackageCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        packagelist = m_refPackage.GetList(PackageCriteria)

        TotalPagesNumber = PackageCriteria.PagingInfo.TotalPages

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
            lnkBtnPreviousPage.Enabled = True
            FirstPage.Enabled = True
            LastPage.Enabled = True
            NextPage.Enabled = True
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
                FirstPage.Enabled = False
            ElseIf _currentPageNumber = TotalPagesNumber Then
                NextPage.Enabled = False
                LastPage.Enabled = False
            End If
        End If

        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Id"
        colBound.HeaderText = m_refMsg.GetMessage("generic id")
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.HeaderStyle.CssClass = "title-header"
        dg_package.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Name"
        colBound.HeaderText = m_refMsg.GetMessage("generic name")
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.HeaderStyle.CssClass = "title-header"
        dg_package.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Length"
        colBound.HeaderText = m_refMsg.GetMessage("lbl length")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        colBound.HeaderStyle.CssClass = "title-header"
        dg_package.Columns.Add(colBound)
        dg_package.BorderColor = Drawing.Color.White

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Height"
        colBound.HeaderText = m_refMsg.GetMessage("lbl height")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        colBound.HeaderStyle.CssClass = "title-header"
        dg_package.Columns.Add(colBound)
        dg_package.BorderColor = Drawing.Color.White

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Width"
        colBound.HeaderText = m_refMsg.GetMessage("lbl width")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        colBound.HeaderStyle.CssClass = "title-header"
        dg_package.Columns.Add(colBound)
        dg_package.BorderColor = Drawing.Color.White

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Weight"
        colBound.HeaderText = m_refMsg.GetMessage("lbl maxweight")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        colBound.HeaderStyle.CssClass = "title-header"
        dg_package.Columns.Add(colBound)
        dg_package.BorderColor = Drawing.Color.White

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("Id", GetType(String)))
        dt.Columns.Add(New DataColumn("Name", GetType(String)))
        dt.Columns.Add(New DataColumn("Length", GetType(String)))
        dt.Columns.Add(New DataColumn("Height", GetType(String)))
        dt.Columns.Add(New DataColumn("Width", GetType(String)))
        dt.Columns.Add(New DataColumn("Weight", GetType(String)))

        If (Not (IsNothing(packagelist))) Then
            For i = 0 To packagelist.Count - 1
                dr = dt.NewRow
                Dim dimensionUnit As String = ""
                Dim weightUnit As String = ""
                If measurementSystem = Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English Then
                    dimensionUnit = m_refMsg.GetMessage("lbl inches")
                    weightUnit = m_refMsg.GetMessage("lbl pounds")
                Else
                    dimensionUnit = m_refMsg.GetMessage("lbl centimeters")
                    weightUnit = m_refMsg.GetMessage("lbl kilograms")
                End If

                dr(0) = "<a href=""packagesize.aspx?action=View&id=" & packagelist.Item(i).Id & """>" & packagelist.Item(i).Id & "</a>"
                dr(1) = "<a href=""packagesize.aspx?action=View&id=" & packagelist.Item(i).Id & """>" & packagelist.Item(i).Name & "</a>"
                dr(2) = "<label id=""length"">" & packagelist.Item(i).Dimensions.Length & "&nbsp;" & dimensionUnit & "</label>"
                dr(3) = "<label id=""height"">" & packagelist.Item(i).Dimensions.Height & "&nbsp;" & dimensionUnit & "</label>"
                dr(4) = "<label id=""width"">" & packagelist.Item(i).Dimensions.Width & "&nbsp;" & dimensionUnit & "</label>"
                dr(5) = "<label id=""weight"">" & packagelist.Item(i).MaxWeight.Amount & "&nbsp;" & weightUnit & "</label>"

                dt.Rows.Add(dr)
            Next
        End If

        Dim dv As New DataView(dt)
        dg_package.DataSource = dv
        dg_package.DataBind()
    End Sub
    Protected Sub Display_View()
        Dim package As PackageData = Nothing
        m_refPackage = New PackageApi()
        package = m_refPackage.GetItem(Me.m_iID)

        txt_package_name.Text = package.Name
        lbl_package_id.Text = package.Id
        txt_package_length.Text = package.Dimensions.Length
        txt_package_height.Text = package.Dimensions.Height
        txt_package_width.Text = package.Dimensions.Width
        txt_package_weight.Text = package.MaxWeight.Amount

        If measurementSystem = Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English Then
            ltr_length_unit.Text = m_refMsg.GetMessage("lbl inches")
            ltr_width_unit.Text = m_refMsg.GetMessage("lbl inches")
            ltr_height_unit.Text = m_refMsg.GetMessage("lbl inches")
            ltr_weight_unit.Text = m_refMsg.GetMessage("lbl pounds")
        Else
            ltr_length_unit.Text = m_refMsg.GetMessage("lbl centimeters")
            ltr_width_unit.Text = m_refMsg.GetMessage("lbl centimeters")
            ltr_height_unit.Text = m_refMsg.GetMessage("lbl centimeters")
            ltr_weight_unit.Text = m_refMsg.GetMessage("lbl kilograms")
        End If

        txt_package_name.Enabled = False
        txt_package_length.Enabled = False
        txt_package_height.Enabled = False
        txt_package_width.Enabled = False
        txt_package_weight.Enabled = False

        TotalPages.Visible = False
        CurrentPage.Visible = False
        lnkBtnPreviousPage.Visible = False
        NextPage.Visible = False
        LastPage.Visible = False
        FirstPage.Visible = False
        PageLabel.Visible = False
        OfLabel.Visible = False
    End Sub
    Protected Sub Display_AddEdit()
        Dim package As PackageData = Nothing
        m_refPackage = New PackageApi()

        If Me.m_iID > 0 Then
            package = m_refPackage.GetItem(Me.m_iID)
            txt_package_name.Text = package.Name
            lbl_package_id.Text = package.Id
            txt_package_length.Text = package.Dimensions.Length
            txt_package_height.Text = package.Dimensions.Height
            txt_package_width.Text = package.Dimensions.Width
            txt_package_weight.Text = package.MaxWeight.Amount
        End If

        txt_package_name.Enabled = True
        txt_package_length.Enabled = True
        txt_package_height.Enabled = True
        txt_package_width.Enabled = True
        txt_package_weight.Enabled = True

        If measurementSystem = Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English Then
            ltr_length_unit.Text = m_refMsg.GetMessage("lbl inches")
            ltr_width_unit.Text = m_refMsg.GetMessage("lbl inches")
            ltr_height_unit.Text = m_refMsg.GetMessage("lbl inches")
            ltr_weight_unit.Text = m_refMsg.GetMessage("lbl pounds")
        Else
            ltr_length_unit.Text = m_refMsg.GetMessage("lbl centimeters")
            ltr_width_unit.Text = m_refMsg.GetMessage("lbl centimeters")
            ltr_height_unit.Text = m_refMsg.GetMessage("lbl centimeters")
            ltr_weight_unit.Text = m_refMsg.GetMessage("lbl kilograms")
        End If

        TotalPages.Visible = False
        CurrentPage.Visible = False
        lnkBtnPreviousPage.Visible = False
        NextPage.Visible = False
        LastPage.Visible = False
        FirstPage.Visible = False
        PageLabel.Visible = False
        OfLabel.Visible = False
    End Sub
#End Region

#Region "Process"
    Protected Sub Process_AddEdit()
        Dim package As PackageData = Nothing
        m_refPackage = New PackageApi()

        If Me.m_iID > 0 Then
            package = m_refPackage.GetItem(Me.m_iID)
            package.Name = txt_package_name.Text
            package.Dimensions.Length = Convert.ToSingle(txt_package_length.Text)
            package.Dimensions.Height = Convert.ToSingle(txt_package_height.Text)
            package.Dimensions.Width = Convert.ToSingle(txt_package_width.Text)
            package.MaxWeight.Amount = Convert.ToSingle(txt_package_weight.Text)
            Select Case ltr_height_unit.Text
                Case "Inches"
                    package.Dimensions.Units = LinearUnit.Inches
                Case "Centimeters"
                    package.Dimensions.Units = LinearUnit.Centimeters
            End Select
            Select Case ltr_weight_unit.Text
                Case "Pounds"
                    package.MaxWeight.Units = WeightUnit.Pounds
                Case "Kilograms"
                    package.MaxWeight.Units = WeightUnit.Kilograms
            End Select

            m_refPackage.Update(package)
            Response.Redirect(m_sPageName & "?action=view&id=" & Me.m_iID.ToString(), False)
        Else
            Dim Dimension As New Ektron.Cms.Commerce.Dimensions
            Dim maxWeight As New Ektron.Cms.Commerce.Weight

            Dimension.Length = Convert.ToSingle(txt_package_length.Text)
            Dimension.Height = Convert.ToSingle(txt_package_height.Text)
            Dimension.Width = Convert.ToSingle(txt_package_width.Text)

            package = New PackageData(txt_package_name.Text, Dimension, maxWeight)

            Select Case ltr_length_unit.Text
                Case "Inches"
                    package.Dimensions.Units = LinearUnit.Inches
                Case "Centimeters"
                    package.Dimensions.Units = LinearUnit.Centimeters
            End Select
            Select Case ltr_weight_unit.Text
                Case "Pounds"
                    package.MaxWeight.Units = WeightUnit.Pounds
                Case "Kilograms"
                    package.MaxWeight.Units = WeightUnit.Kilograms
            End Select
            package.MaxWeight.Amount = Convert.ToSingle(txt_package_weight.Text)

            m_refPackage.Add(package)
            Response.Redirect(m_sPageName, False)
        End If
    End Sub
    Protected Sub Process_Delete()
        Dim package As PackageApi = Nothing
        package = New PackageApi()
        If Me.m_iID > 0 Then
            package.Delete(Me.m_iID)
            Response.Redirect(Me.m_sPageName, False)
        End If
    End Sub
#End Region
#Region "Set And Navigation"

    Protected Sub SetLabels()
        Me.ltr_package_name.Text = Me.GetMessage("generic name")
        Me.ltr_package_id.Text = Me.GetMessage("generic id")
        Me.ltr_package_length.Text = Me.GetMessage("lbl length")
        Me.ltr_package_width.Text = Me.GetMessage("lbl width")
        Me.ltr_package_height.Text = Me.GetMessage("lbl height")
        Me.ltr_package_weight.Text = Me.GetMessage("lbl maxweight")

        Select Case MyBase.m_sPageAction

            Case "addedit"
                Me.pnl_viewaddress.Visible = True
                Me.AddButtonwithMessages(Me.AppImgPath & "../UI/Icons/save.png", m_sPageName & "?action=addedit&id=" & Me.m_iID.ToString(), "lbl alt save package", "btn save", " onclick="" return SubmitForm(); "" ")
                Me.AddBackButton(Me.m_sPageName & IIf(Me.m_iID > 0, "?action=view&id=" & Me.m_iID.ToString(), ""))
                If Me.m_iID > 0 Then
                    Me.SetTitleBarToMessage("lbl edit package")
                    Me.AddHelpButton("EditPackages")
                Else
                    Me.SetTitleBarToMessage("lbl add package")
                    Me.AddHelpButton("Addpackage")
                End If

            Case "view"
                Me.pnl_viewall.Visible = False
                Me.pnl_viewaddress.Visible = True
                Me.AddButtonwithMessages(Me.AppImgPath & "../UI/Icons/contentEdit.png", Me.m_sPageName & "?action=addedit&id=" & Me.m_iID.ToString(), "generic edit title", "generic edit title", "")
                Me.AddButtonwithMessages(Me.AppImgPath & "../UI/Icons/delete.png", Me.m_sPageName & "?action=del&id=" & Me.m_iID.ToString(), "alt del package button text", "btn delete", " onclick="" return CheckDelete();"" ")
                Me.AddBackButton(Me.m_sPageName)
                Me.SetTitleBarToMessage("lbl view package")
                Me.AddHelpButton("Viewpackage")

            Case Else ' "viewall"
                Dim newMenu As New workareamenu("file", Me.GetMessage("lbl new"), Me.AppImgPath & "../UI/Icons/star.png")
                newMenu.AddLinkItem(Me.AppImgPath & "menu/card.gif", Me.GetMessage("lbl package"), Me.m_sPageName & "?action=addedit")
                Me.AddMenu(newMenu)

                Me.SetTitleBarToMessage("lbl packages")
                Me.AddHelpButton("packages")
        End Select
        SetJs()
    End Sub
    Private Sub SetJs()
        Dim sbJS As New StringBuilder
        sbJS.Append("<script language=""javascript"" type=""text/javascript"" >" & Environment.NewLine)

        sbJS.Append(" var deletePackageMsg = '").Append(GetMessage("js package confirm del")).Append("';" & Environment.NewLine)
        sbJS.Append(" var badPackageDimensionMsg = '").Append(GetMessage("js alert package dimension value 5_2")).Append("';" & Environment.NewLine)
        sbJS.Append(" var emptyPackageMsg = '").Append(GetMessage("js null package msg")).Append("';" & Environment.NewLine)
        sbJS.Append(" var badPackageNameMsg = """).Append(GetMessage("js alert package name cant include")).Append(" ('\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')"";" & Environment.NewLine)

        sbJS.Append(JSLibrary.ToggleDiv())

        sbJS.Append("</script>" & Environment.NewLine)
        ltr_js.Text &= Environment.NewLine & sbJS.ToString()
    End Sub
    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        'If hdnCurrentPage.Value <> "" Then
        '    _currentPageNumber = Int32.Parse(hdnCurrentPage.Value)
        'End If
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
        Display_View_All()
        isPostData.Value = "true"
    End Sub
#End Region
    Private Sub Util_RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
    End Sub
End Class
