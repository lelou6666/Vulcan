Imports Ektron
Imports Ektron.Cms.Workarea
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports System.data
Imports System.Web.HttpRequest
Imports System.Web.UI.page
Partial Class Commerce_shipping_shippingsource
    Inherits workareabase

    Protected criteria As New Ektron.Cms.Common.Criteria(Of CountryProperty)(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
    Protected RegionCriteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
    Protected m_refRegion As RegionApi = Nothing
    Protected m_refCountry As CountryApi = Nothing
    Protected m_refWarehouse As WarehouseApi = Nothing
    Protected m_sPageName As String = "shippingsource.aspx"
    Protected _currentPageNumber As Integer = 1
    Protected m_bIsDefault As Boolean = False
    Protected TotalPagesNumber As Integer = 1
    Protected CountryList As New System.Collections.Generic.List(Of CountryData)()
    Protected RegionList As New System.Collections.Generic.List(Of RegionData)()

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        Utilities.ValidateUserLogin()
        Me.CommerceLibrary.CheckCommerceAdminAccess()
        Util_RegisterResources()
        m_refRegion = New RegionApi()
        m_refCountry = New CountryApi()
        criteria.PagingInfo = New PagingInfo(10000)
        RegionCriteria.PagingInfo = New PagingInfo(10000)
        CountryList = m_refCountry.GetList(criteria)
        RegionList = m_refRegion.GetList(RegionCriteria)

        Try
            Select Case MyBase.m_sPageAction
                Case "markdef"
                    Process_MarkDefault()
                Case "del"
                    Process_Delete()
                Case "addedit"
                    If Page.IsPostBack And smAddressCountry.IsInAsyncPostBack = False Then
                        Process_AddEdit()
                    ElseIf smAddressCountry.IsInAsyncPostBack = True Then
                        Util_BindRegions(drp_address_country.SelectedValue)
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
    Protected Sub Util_Bind()
        If CountryList IsNot Nothing AndAlso CountryList.Count > 0 Then
            drp_address_country.DataSource = CountryList
            drp_address_country.DataTextField = "Name"
            drp_address_country.DataValueField = "Id"
            drp_address_country.DataBind()
        End If
        If m_sPageAction = "addedit" Then
            If m_iID > 0 Then
                Dim wareHouse As WarehouseData = Nothing
                m_refWarehouse = New WarehouseApi()
                wareHouse = m_refWarehouse.GetItem(Me.m_iID)

                Dim cCountryId As Integer = wareHouse.Address.Country.Id

                RegionCriteria.AddFilter(RegionProperty.CountryId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, cCountryId)
                RegionList = m_refRegion.GetList(RegionCriteria)
                If RegionList IsNot Nothing AndAlso RegionList.Count > 0 Then
                    drp_address_region.DataSource = RegionList
                    drp_address_region.DataTextField = "Name"
                    drp_address_region.DataValueField = "Id"
                    drp_address_region.DataBind()
                End If
            Else
                Dim cCountryId As Integer = drp_address_country.SelectedIndex
                If cCountryId = 0 Then
                    cCountryId = CountryList.Item(0).Id 'The first country in the contryList.
                End If
                RegionCriteria.AddFilter(RegionProperty.CountryId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, cCountryId)
                RegionList = m_refRegion.GetList(RegionCriteria)
                If RegionList IsNot Nothing AndAlso RegionList.Count > 0 Then
                    drp_address_region.DataSource = RegionList
                    drp_address_region.DataTextField = "Name"
                    drp_address_region.DataValueField = "Id"
                    drp_address_region.DataBind()
                End If
            End If
        Else
            RegionList = m_refRegion.GetList(RegionCriteria)
            If RegionList IsNot Nothing AndAlso RegionList.Count > 0 Then
                drp_address_region.DataSource = RegionList
                drp_address_region.DataTextField = "Name"
                drp_address_region.DataValueField = "Id"
                drp_address_region.DataBind()
            End If
        End If
    End Sub
#Region "Display"
    Protected Sub Display_View()
        Dim wareHouse As WarehouseData = Nothing
        m_refWarehouse = New WarehouseApi()
        wareHouse = m_refWarehouse.GetItem(Me.m_iID)
        Util_Bind()

        txt_address_name.Text = wareHouse.Name
        lbl_address_id.Text = wareHouse.Id
        txt_address_line1.Text = wareHouse.Address.AddressLine1
        txt_address_line2.Text = wareHouse.Address.AddressLine2
        txt_address_city.Text = wareHouse.Address.City
        drp_address_region.SelectedIndex = Util_GetRegionIndex(wareHouse.Address.Region.Id)
        txt_address_postal.Text = wareHouse.Address.PostalCode
        drp_address_country.SelectedIndex = Util_GetCountryIndex(wareHouse.Address.Country.Id)
        chk_default_warehouse.Checked = wareHouse.IsDefaultWarehouse

        txt_address_name.Enabled = False
        txt_address_line1.Enabled = False
        txt_address_line2.Enabled = False
        txt_address_city.Enabled = False
        drp_address_region.Enabled = False
        txt_address_postal.Enabled = False
        drp_address_country.Enabled = False
        chk_default_warehouse.Enabled = False

        m_bIsDefault = wareHouse.IsDefaultWarehouse

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
        Dim wareHouse As WarehouseData = Nothing
        m_refWarehouse = New WarehouseApi()
        wareHouse = m_refWarehouse.GetItem(Me.m_iID)
        Util_Bind()

        If Me.m_iID > 0 Then
            txt_address_name.Text = wareHouse.Name
            lbl_address_id.Text = wareHouse.Id
            txt_address_line1.Text = wareHouse.Address.AddressLine1
            txt_address_line2.Text = wareHouse.Address.AddressLine2
            txt_address_city.Text = wareHouse.Address.City
            drp_address_region.SelectedIndex = Util_GetRegionIndex(wareHouse.Address.Region.Id)
            txt_address_postal.Text = wareHouse.Address.PostalCode
            drp_address_country.SelectedIndex = Util_GetCountryIndex(wareHouse.Address.Country.Id)
            chk_default_warehouse.Checked = wareHouse.IsDefaultWarehouse

            txt_address_name.Enabled = True
            txt_address_line1.Enabled = True
            txt_address_line2.Enabled = True
            txt_address_city.Enabled = True
            drp_address_region.Enabled = True
            txt_address_postal.Enabled = True
            drp_address_country.Enabled = True
            chk_default_warehouse.Enabled = False

            m_bIsDefault = wareHouse.IsDefaultWarehouse
        Else
            phAddressID.Visible = False
            ltr_address_id.Visible = False
            lbl_colon.Visible = False
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

    Protected Sub Display_View_All()
        Dim warehouseCriteria As New Ektron.Cms.Common.Criteria(Of WarehouseProperty)(WarehouseProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim WarehouseList As New System.Collections.Generic.List(Of WarehouseData)
        Dim i As Integer = 0

        m_refWarehouse = New WarehouseApi()

        dg_warehouse.AutoGenerateColumns = False
        dg_warehouse.Columns.Clear()

        warehouseCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        warehouseCriteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        WarehouseList = m_refWarehouse.GetList(warehouseCriteria)

        TotalPagesNumber = warehouseCriteria.PagingInfo.TotalPages

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
        colBound.DataField = "Radio"
        colBound.HeaderText = ""
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center

        dg_warehouse.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Name"
        colBound.HeaderText = "Name"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left

        dg_warehouse.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = "Id"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center

        dg_warehouse.Columns.Add(colBound)
        dg_warehouse.BorderColor = Drawing.Color.White

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "IsDefaultWarehouse"
        colBound.HeaderText = "Default"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center

        dg_warehouse.Columns.Add(colBound)
        dg_warehouse.BorderColor = Drawing.Color.White

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("Radio", GetType(String)))
        dt.Columns.Add(New DataColumn("Name", GetType(String)))
        dt.Columns.Add(New DataColumn("Id", GetType(String)))
        dt.Columns.Add(New DataColumn("IsDefaultWarehouse", GetType(String)))

        If (Not (IsNothing(WarehouseList))) Then
            For i = 0 To WarehouseList.Count - 1
                m_bIsDefault = WarehouseList.Item(i).IsDefaultWarehouse
                dr = dt.NewRow
                dr(0) = "<input type=""radio"" id=""radio_warehouse"" name=""radio_warehouse"" value=""" & WarehouseList.Item(i).Id & """ />"
                dr(1) = "<a href=""shippingsource.aspx?action=View&id=" & WarehouseList.Item(i).Id & """>" & WarehouseList.Item(i).Name & "</a>"
                dr(2) = "<label id=""lbl_warehouseId"">" & WarehouseList.Item(i).Id & "</label>"
                dr(3) = "<input type=""CheckBox"" id=""chk_default" & i & """ name=""chk_default" & i & """ disabled=""true"" " & IIf(m_bIsDefault, "Checked=""checked""", "") & "/>"
                dt.Rows.Add(dr)
            Next
        End If

        Dim dv As New DataView(dt)
        dg_warehouse.DataSource = dv
        dg_warehouse.DataBind()
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

#Region "Process"
    Protected Sub Process_MarkDefault()
        m_refWarehouse = New WarehouseApi()
        m_refWarehouse.MarkAsDefault(Me.m_iID)
        Response.Redirect(Me.m_sPageName, False)
    End Sub

    Protected Sub Process_Delete()
        m_refWarehouse = New WarehouseApi()
        If Me.m_iID > 0 And Not m_refWarehouse.GetItem(Me.m_iID).IsDefaultWarehouse Then
            m_refWarehouse.Delete(Me.m_iID)
            Response.Redirect(Me.m_sPageName, False)
        Else
            Response.Redirect(Me.m_sPageName & "?action=view&id=" & Me.m_iID.ToString(), False)
        End If
    End Sub

    Protected Sub Process_AddEdit()
        Dim wareHouse As WarehouseData = Nothing
        m_refWarehouse = New WarehouseApi()

        If Me.m_iID > 0 Then
            wareHouse = m_refWarehouse.GetItem(Me.m_iID)
        End If

        Dim rData As RegionData
        rData = New RegionData
        rData = m_refRegion.GetItem(drp_address_region.SelectedValue)

        Dim cData As CountryData
        cData = New CountryData
        cData = m_refCountry.GetItem(drp_address_country.SelectedValue)


        If Me.m_iID = 0 Then
            wareHouse = New WarehouseData(txt_address_name.Text, New AddressData())
        End If

        wareHouse.Name = txt_address_name.Text

        If Me.m_iID > 0 Then
            wareHouse.Id = lbl_address_id.Text
        End If

        wareHouse.Address.AddressLine1 = txt_address_line1.Text
        wareHouse.Address.AddressLine2 = txt_address_line2.Text
        wareHouse.Address.City = txt_address_city.Text
        If wareHouse.Address.Region Is Nothing Then wareHouse.Address.Region = New RegionData()
        wareHouse.Address.Region.Id = drp_address_region.SelectedValue
        wareHouse.Address.PostalCode = txt_address_postal.Text
        If wareHouse.Address.Country Is Nothing Then wareHouse.Address.Country = New CountryData()
        wareHouse.Address.Country.Id = drp_address_country.SelectedValue
        wareHouse.IsDefaultWarehouse = chk_default_warehouse.Checked

        If Me.m_iID > 0 Then
            m_refWarehouse.Update(wareHouse)
            Response.Redirect(m_sPageName & "?action=view&id=" & Me.m_iID.ToString(), False)
        Else
            m_refWarehouse.Add(wareHouse)
            Response.Redirect(m_sPageName, False)
        End If
    End Sub
#End Region

    Protected Sub SetLabels()
        Me.ltr_address_name.Text = Me.GetMessage("lbl address name")
        Me.ltr_address_id.Text = Me.GetMessage("generic id")
        Me.ltr_address_line1.Text = Me.GetMessage("lbl address line1")
        Me.ltr_address_line2.Text = Me.GetMessage("lbl address line2")
        Me.ltr_address_city_lbl.Text = Me.GetMessage("lbl address city")
        Me.ltr_address_region.Text = Me.GetMessage("lbl state province")
        Me.ltr_address_postal.Text = Me.GetMessage("lbl address postal")
        Me.ltr_address_country.Text = Me.GetMessage("lbl address country")
        Me.ltr_default_warehouse.Text = Me.GetMessage("lbl default warehouse")

        Select Case MyBase.m_sPageAction

            Case "addedit"
                Me.pnl_viewaddress.Visible = True
                Me.AddButtonwithMessages(Me.AppImgPath & "../UI/Icons/save.png", m_sPageName & "?action=addedit&id=" & Me.m_iID.ToString(), "lbl alt save warehouse", "btn save", " onclick="" resetCPostback(); return SubmitForm(); "" ")
                Me.AddBackButton(Me.m_sPageName & IIf(Me.m_iID > 0, "?action=view&id=" & Me.m_iID.ToString(), ""))
                If Me.m_iID > 0 Then
                    Me.SetTitleBarToMessage("lbl edit warehouse")
                    Me.AddHelpButton("Editwarehouse")
                Else
                    Me.SetTitleBarToMessage("lbl add warehouse")
                    Me.AddHelpButton("Addwarehouse")
                End If

            Case "view"
                Me.pnl_viewall.Visible = False
                Me.pnl_viewaddress.Visible = True
                Me.AddButtonwithMessages(Me.AppImgPath & "../UI/Icons/contentEdit.png", Me.m_sPageName & "?action=addedit&id=" & Me.m_iID.ToString(), "generic edit title", "generic edit title", "")
                If Not m_bIsDefault Then

                    Me.AddButtonwithMessages(Me.AppImgPath & "icon_survey_enable.gif", Me.m_sPageName & "?action=markdef&id=" & Me.m_iID.ToString(), "lbl warehouse mark def", "lbl warehouse mark def", "")
                    Me.AddButtonwithMessages(Me.AppImgPath & "../UI/Icons/delete.png", Me.m_sPageName & "?action=del&id=" & Me.m_iID.ToString(), "alt del warehouse button text", "btn delete", " onclick="" return CheckDelete();"" ")

                End If
                Me.AddBackButton(Me.m_sPageName)
                Me.SetTitleBarToMessage("lbl view warehouse")
                Me.AddHelpButton("Viewwarehouse")
            Case Else ' "viewall"
                Dim newMenu As New workareamenu("file", Me.GetMessage("lbl new"), Me.AppImgPath & "../UI/Icons/star.png")
                newMenu.AddLinkItem(Me.AppImgPath & "menu/card.gif", Me.GetMessage("lbl warehouse"), Me.m_sPageName & "?action=addedit")
                Me.AddMenu(newMenu)
                Dim actionMenu As New workareamenu("action", Me.GetMessage("lbl action"), Me.AppImgPath & "../UI/Icons/check.png")
                actionMenu.AddItem(Me.AppImgPath & "icon_survey_enable.gif", Me.GetMessage("lbl warehouse mark def"), "CheckMarkAsDef();")
                Me.AddMenu(actionMenu)

                Me.SetTitleBarToMessage("lbl warehouses")
                Me.AddHelpButton("warehouses")
        End Select

        SetJs()
    End Sub

    Private Sub SetJs()
        Dim sbJS As New StringBuilder
        sbJS.Append("<script language=""javascript"" type=""text/javascript"" >" & Environment.NewLine)

        sbJS.Append("function CheckDelete()" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("if (document.forms[0].chk_default_warehouse.checked == true)" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("   alert('").Append(GetMessage("lbl delete err default warehouse")).Append("');" & Environment.NewLine)
        sbJS.Append("   return true;" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)
        sbJS.Append("    return confirm('").Append(GetMessage("js warehouse confirm del")).Append("');" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function SubmitForm()" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("    var objtitle = document.getElementById(""").Append(txt_address_name.UniqueID).Append(""");" & Environment.NewLine)
        sbJS.Append("    var rRegion = document.getElementById(""").Append(drp_address_region.UniqueID).Append(""");" & Environment.NewLine)
        sbJS.Append("    var postalCode=document.getElementById(""").Append(txt_address_postal.UniqueID).Append(""");" & Environment.NewLine)
        sbJS.Append("    var isValidZip=/^\d{5}$/ ;" & Environment.NewLine)
        sbJS.Append("    if (postalCode.value.search(isValidZip)==-1 && document.getElementById('drp_address_country').value == '840')").Append(Environment.NewLine)
        sbJS.Append("    {").Append(Environment.NewLine)
        sbJS.Append("        alert(""" & MyBase.GetMessage("js postal code validation") & """);" & Environment.NewLine)
        sbJS.Append("        postalCode.focus();return false;" & Environment.NewLine)
        sbJS.Append("    }" & Environment.NewLine)
        sbJS.Append("    if(rRegion != null){if(rRegion.selectedIndex == -1)").Append(Environment.NewLine)
        sbJS.Append("    {").Append(Environment.NewLine)
        sbJS.Append("       alert(""" & MyBase.GetMessage("js null warehouse region msg") & """);" & Environment.NewLine)
        sbJS.Append("       document.forms[""form1""].isCPostData.value = 'false';").Append(Environment.NewLine)
        sbJS.Append("       return false;").Append(Environment.NewLine)
        sbJS.Append("    }}").Append(Environment.NewLine)
        sbJS.Append("    if (Trim(objtitle.value).length > 0)" & Environment.NewLine)
        sbJS.Append("    {" & Environment.NewLine)
        sbJS.Append("	    if (!CheckForillegalChar(objtitle.value)) {" & Environment.NewLine)
        sbJS.Append("           objtitle.focus();" & Environment.NewLine)
        sbJS.Append("       } else {" & Environment.NewLine)
        sbJS.Append("           document.forms[""form1""].isCPostData.value = '';").Append(Environment.NewLine)
        sbJS.Append("           document.forms[0].submit();" & Environment.NewLine)
        sbJS.Append("	    }" & Environment.NewLine)
        sbJS.Append("    }" & Environment.NewLine)
        sbJS.Append("    else" & Environment.NewLine)
        sbJS.Append("    {" & Environment.NewLine)
        sbJS.Append("        alert(""" & MyBase.GetMessage("js null warehouse msg") & """);" & Environment.NewLine)
        sbJS.Append("        objtitle.focus();" & Environment.NewLine)
        sbJS.Append("    }" & Environment.NewLine)
        sbJS.Append("    return false;" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function CheckForillegalChar(txtName) {" & Environment.NewLine)
        sbJS.Append("   var val = txtName;" & Environment.NewLine)
        sbJS.Append("   if ((val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbJS.Append("   {" & Environment.NewLine)
        sbJS.Append("       alert(""").Append(String.Format(GetMessage("js alert warehouse name cant include"), "('\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')")).Append(""");" & Environment.NewLine)
        sbJS.Append("       return false;" & Environment.NewLine)
        sbJS.Append("   }" & Environment.NewLine)
        sbJS.Append("   return true;" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function CheckMarkAsDef() {" & Environment.NewLine)
        sbJS.Append(" 	var chosen = ''; ").Append(Environment.NewLine)
        sbJS.Append("   if(document.forms[0].radio_warehouse == undefined)").Append(Environment.NewLine)
        sbJS.Append("   {").Append(Environment.NewLine)
        sbJS.Append(" 		alert('").Append(GetMessage("js err no warehouse")).Append("'); ").Append(Environment.NewLine)
        sbJS.Append("       return false;").Append(Environment.NewLine)
        sbJS.Append("   }").Append(Environment.NewLine)
        sbJS.Append("   else").Append(Environment.NewLine)
        sbJS.Append("   {").Append(Environment.NewLine)
        sbJS.Append(" 	    var len = document.forms[0].radio_warehouse.length; ").Append(Environment.NewLine)
        sbJS.Append(" 	    if (len > 0) { ").Append(Environment.NewLine)
        sbJS.Append(" 	        for (i = 0; i < len; i++) { ").Append(Environment.NewLine)
        sbJS.Append(" 		        if (document.form1.radio_warehouse[i].checked) { ").Append(Environment.NewLine)
        sbJS.Append(" 			        chosen = document.form1.radio_warehouse[i].value; ").Append(Environment.NewLine)
        sbJS.Append(" 		        } ").Append(Environment.NewLine)
        sbJS.Append(" 	        } ").Append(Environment.NewLine)
        sbJS.Append(" 	    } else { ").Append(Environment.NewLine)
        sbJS.Append(" 	        if (document.form1.radio_warehouse.checked) { chosen = document.form1.radio_warehouse.value; } ").Append(Environment.NewLine)
        sbJS.Append(" 	    } ").Append(Environment.NewLine)
        sbJS.Append(" 	    if (chosen == '') { ").Append(Environment.NewLine)
        sbJS.Append(" 		    alert('").Append(GetMessage("js please choose warehouse")).Append("'); ").Append(Environment.NewLine)
        sbJS.Append(" 	    } else if (confirm('").Append(GetMessage("js warehouse mark def")).Append("')) { ").Append(Environment.NewLine)
        sbJS.Append(" 		    window.location.href = 'shippingsource.aspx?action=markdef&id=' + chosen; ").Append(Environment.NewLine)
        sbJS.Append(" 	    } ").Append(Environment.NewLine)
        sbJS.Append(" 	} ").Append(Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append(JSLibrary.ToggleDiv())

        sbJS.Append("</script>" & Environment.NewLine)
        ltr_js.Text &= Environment.NewLine & sbJS.ToString()
    End Sub

    Public Function ProtectPassword(ByVal pwd As String) As String
        Return "**********"
    End Function
    Protected Function Util_GetCountryIndex(ByVal countryId As Integer) As Integer
        Dim iRet As Integer = -1
        If CountryList IsNot Nothing AndAlso CountryList.Count > 0 Then
            For i As Integer = 0 To (CountryList.Count - 1)
                If CountryList(i).Id = countryId Then iRet = i
            Next
        End If
        Return iRet
    End Function
    Protected Function Util_GetCountryName(ByVal countryId As Integer) As String
        Dim sRet As String = ""
        If CountryList IsNot Nothing AndAlso CountryList.Count > 0 Then
            For i As Integer = 0 To (CountryList.Count - 1)
                If CountryList(i).Id = countryId Then sRet = CountryList(i).Name
            Next
        End If
        Return sRet
    End Function
    Protected Function Util_GetRegionIndex(ByVal regionId As Integer) As Integer
        Dim iRet As Integer = -1
        If RegionList IsNot Nothing AndAlso RegionList.Count > 0 Then
            For i As Integer = 0 To (RegionList.Count - 1)
                If RegionList(i).Id = regionId Then iRet = i
            Next
        End If
        Return iRet
    End Function
    Protected Function Util_GetRegionName(ByVal regionId As Integer) As String
        Dim sRet As String = ""
        If RegionList IsNot Nothing AndAlso RegionList.Count > 0 Then
            For i As Integer = 0 To (RegionList.Count - 1)
                If RegionList(i).Id = regionId Then sRet = RegionList(i).Name
            Next
        End If
        Return sRet
    End Function

    Protected Sub Util_BindRegions(ByVal countryId As String)
        Dim cCountryId As Integer = drp_address_country.SelectedValue
        Dim criteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)

        drp_address_region.DataSource = ""
        criteria.AddFilter(RegionProperty.CountryId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, cCountryId)
        criteria.PagingInfo.RecordsPerPage = 10000

        RegionList = m_refRegion.GetList(criteria)
        If RegionList IsNot Nothing AndAlso RegionList.Count > 0 Then
            drp_address_region.DataSource = RegionList
            drp_address_region.DataTextField = "Name"
            drp_address_region.DataValueField = "Id"
            drp_address_region.DataBind()
        Else
            Dim NoRegions As String = "No Regions"
            'NoRegions(0).ToString = "No Regions"
            drp_address_region.DataSource = ""
            drp_address_region.DataTextField = ""
            drp_address_region.DataValueField = ""

            drp_address_region.DataBind()
        End If
    End Sub
    Private Sub Util_RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
    End Sub
End Class
