Imports Ektron
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea
Imports Ektron.Cms.Commerce
Imports System.data
Imports System.Web.HttpRequest
Imports System.Web.UI.page
Imports System.Collections.Generic

Partial Class Commerce_tax_postaltaxtables
    Inherits workareabase

#Region "Member Variables"

    Protected _RegionApi As RegionApi
    Protected _TaxApi As TaxApi
    Protected _CountryApi As CountryApi
    Protected _TaxClassApi As New Ektron.Cms.Commerce.TaxClassApi()
    Protected _PageName As String = "postaltaxtables.aspx"
    Protected _CountryList As New System.Collections.Generic.List(Of CountryData)()
    Protected _RegionList As New System.Collections.Generic.List(Of RegionData)()
    Protected _Criteria As New Ektron.Cms.Common.Criteria(Of CountryProperty)(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
    Protected _CurrentPageNumber As Integer = 1
    Protected _TotalPagesNumber As Integer = 1
    Protected AppPath As String = ""

#End Region

#Region "Events"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        'register page components
        Me.RegisterJS()
        Me.RegisterCSS()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        Utilities.ValidateUserLogin()
        Util_CheckAccess()
        Util_RegisterResources()
        _RegionApi = New RegionApi() '(Me.m_refContentApi.RequestInformationRef)
        _CountryApi = New CountryApi() '(Me.m_refContentApi.RequestInformationRef)
        _Criteria.PagingInfo = New PagingInfo(10000)
        Dim cCountryId As Integer = 0
        _Criteria.AddFilter(CountryProperty.IsEnabled, CriteriaFilterOperator.EqualTo, True)
        AppPath = m_refContentApi.AppPath

        Select Case Me.m_sPageAction
            Case "addedit"
                _CountryList = _CountryApi.GetList(_Criteria)
                If Page.IsPostBack And smUpdateRegion.IsInAsyncPostBack = False Then
                    If Request.Form(isCPostData.UniqueID) = "" Then
                        Process_AddEdit()
                    End If
                Else
                    If smUpdateRegion.IsInAsyncPostBack = True Then
                        UpdateRegions()
                    Else
                        Display_AddEdit()
                    End If
                End If
            Case "del"
                Process_Delete()
            Case "view"
                _CountryList = _CountryApi.GetList(_Criteria)
                Display_View()
            Case Else
                _CountryList = _CountryApi.GetList(_Criteria)
                If Page.IsPostBack = False Then
                    Display_All()
                End If
        End Select
        Util_SetLabels()
        Util_SetJS()
    End Sub

    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        'If hdnCurrentPage.Value <> "" Then
        '    _currentPageNumber = Int32.Parse(hdnCurrentPage.Value)
        'End If
        Select Case e.CommandName
            Case "First"
                _CurrentPageNumber = 1
            Case "Last"
                _CurrentPageNumber = Int32.Parse(TotalPages.Text)
            Case "Next"
                _CurrentPageNumber = Int32.Parse(CurrentPage.Text) + 1
            Case "Prev"
                _CurrentPageNumber = Int32.Parse(CurrentPage.Text) - 1
        End Select
        Display_All()
        isPostData.Value = "true"
    End Sub

#End Region

#Region "Process"

    Protected Sub Process_AddEdit()

        Dim cCountry As CountryData = Nothing
        Dim tTax As TaxRateData = Nothing
        Dim TaxClasscriteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()
        Dim page_data As Integer = _CurrentPageNumber
        Dim postalCriteria As New Ektron.Cms.Common.Criteria(Of TaxRateProperty)(TaxRateProperty.PostalCode, EkEnumeration.OrderByDirection.Ascending)
        Dim postalCode As String = 0
        Dim id As Long = 0
        Dim taxApi As New TaxApi()

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria)

        If Request.QueryString("postalid") <> "" Then
            postalCode = Request.QueryString("postalid")
        End If

        If Request.QueryString("id") <> "" Then
            id = Request.QueryString("id")
        End If
        postalCriteria.PagingInfo.CurrentPage = page_data
        postalCriteria.AddFilter(TaxRateProperty.TaxTypeId, CriteriaFilterOperator.EqualTo, TaxRateType.PostalSalesTax)

        Dim postalRateList As List(Of TaxRateData)
        postalRateList = taxApi.GetList(postalCriteria)

        If hdnRegionValue.Value <> "" Or hdnRegionValue.Value IsNot Nothing Then
            Dim rRegionId As Integer = 0
        End If

        If Me.m_iID > 0 And Page.IsPostBack Then
            Try
                For i As Integer = 0 To TaxClassList.Count - 1
                    Dim postalCodeData As New PostalCodeTaxRateData
                    tTax = taxApi.GetItemByPostalId(TaxClassList.Item(i).Id, id)

                    If tTax Is Nothing Then
                        tTax = New PostalCodeTaxRateData(postalCode, drp_region.SelectedValue, TaxClassList.Item(i).Id, 0.0)
                        If IsNumeric(Request.Form("txtClassRate" & i)) Then
                            tTax.Rate = CDec(Request.Form("txtClassRate" & i) / 100)
                            taxApi.Add(tTax)
                        End If
                    Else
                        If IsNumeric(Request.Form("txtClassRate" & i)) Then
                            postalCodeData = New PostalCodeTaxRateData(txt_name.Text, drp_region.SelectedValue, TaxClassList.Item(i).Id, CDec(Request.Form("txtClassRate" & i) / 100))
                            taxApi.Update(postalCodeData)
                        End If
                    End If
                Next
                Response.Redirect(_PageName & "?action=view&id=" & m_iID.ToString() & "&postalid=" & postalCode, False)
            Catch exc As CmsException
                Utilities.ShowError(EkFunctions.GetAllValidationMessages(exc.ValidationResults))
            End Try
        Else
            Try
                Dim postalrate As New PostalCodeTaxRateData
                Dim TypeId As Integer = 0

                For i As Integer = 0 To TaxClassList.Count - 1
                    If IsNumeric(Request.Form("txtClassRate" & i)) Then
                        postalrate = New PostalCodeTaxRateData(txt_name.Text, drp_region.SelectedValue, TaxClassList.Item(i).Id, CDec(Request.Form("txtClassRate" & i) / 100))
                        taxApi.Add(postalrate)
                    End If
                Next
                Response.Redirect(_PageName, False)
            Catch exc As CmsException
                Utilities.ShowError(EkFunctions.GetAllValidationMessages(exc.ValidationResults))
            End Try
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

    Protected Sub Process_Delete()
        _TaxApi = New TaxApi()
        Dim TaxClasscriteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()
        Dim m_CountryTax As New Ektron.Cms.Commerce.CountryTaxRateData
        TaxClasscriteria.PagingInfo.RecordsPerPage = 10
        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria)

        If Me.m_iID > 0 Then
            For j As Integer = 0 To TaxClassList.Count - 1
                Dim tTax As New Long
                tTax = _TaxApi.GetItemByPostalId(TaxClassList.Item(j).Id, m_iID).Id
                _TaxApi.Delete(tTax)
            Next
        End If

        Response.Redirect(_PageName, False)
    End Sub

#End Region

#Region "Display"

    Protected Sub Display_AddEdit()
        Dim page_data As Integer = _CurrentPageNumber
        Dim TaxClasscriteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()
        Dim postalCriteria As New Ektron.Cms.Common.Criteria(Of TaxRateProperty)(TaxRateProperty.PostalCode, EkEnumeration.OrderByDirection.Ascending)
        Dim postalCode As String = 0
        Dim id As Long = 0
        Dim taxApi As New TaxApi()
        Dim rRegion As RegionData = New RegionData()
        Dim criteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim cCountryId As Integer = 0
        Dim txtClassList As Integer = 0

        Util_BindCountries()

        If Request.QueryString("postalid") <> "" Then
            postalCode = Request.QueryString("postalid")
        End If

        If Request.QueryString("id") <> "" Then
            id = Request.QueryString("id")
        End If

        postalCriteria.PagingInfo.CurrentPage = page_data
        postalCriteria.AddFilter(TaxRateProperty.TaxTypeId, CriteriaFilterOperator.EqualTo, TaxRateType.PostalSalesTax)

        Dim postalRateList As List(Of TaxRateData)
        postalRateList = taxApi.GetList(postalCriteria)

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria)

        txt_name.Text = postalCode
        If txt_name.Text = "0" OrElse 0 = txt_name.Text.Length Then
            txt_name.Enabled = True
        Else
            txt_name.Enabled = False
        End If
        lbl_id.Text = id

        _RegionList = _RegionApi.GetList(criteria)

        cCountryId = drp_country.SelectedValue
        Util_BindRegions(cCountryId)

        ltr_txtClass.Text = "<table class=""ektronGrid"">"
        For txtClassList = 0 To TaxClassList.Count - 1
            Dim postalRegion As New PostalCodeTaxRateData
            postalRegion = taxApi.GetItemByPostalId(TaxClassList.Item(txtClassList).Id, id)
            If postalRegion IsNot Nothing Then
                criteria.AddFilter(RegionProperty.Id, CriteriaFilterOperator.EqualTo, postalRegion.RegionId)
                _RegionList = _RegionApi.GetList(criteria)
                drp_region.SelectedValue = _RegionList.Item(0).Id
                cCountryId = _RegionList.Item(0).CountryId
                drp_country.SelectedValue = cCountryId
                Util_BindRegions(cCountryId)
            End If

            ltr_txtClass.Text += "<tr>"
            ltr_txtClass.Text += "   <td class=""label"">" & TaxClassList.Item(txtClassList).Name & "</td>"
            If taxApi.GetItemByPostalId(TaxClassList.Item(txtClassList).Id, id) Is Nothing Then
                ltr_txtClass.Text += "   <td class=""value"">"
                ltr_txtClass.Text += "       <input type=""text"" name=""txtClassRate" & txtClassList & """ id=""txtClassRate" & txtClassList & """ value=""0"" />%"
                ltr_txtClass.Text += "   </td>"
            Else
                ltr_txtClass.Text += "   <td class=""value"">"
                ltr_txtClass.Text += "       <input type=""text"" name=""txtClassRate" & txtClassList & """ id=""txtClassRate" & txtClassList & """ value=""" & taxApi.GetItemByPostalId(TaxClassList.Item(txtClassList).Id, id).Rate * 100 & """/>%"
                ltr_txtClass.Text += "   </td>"
            End If
            ltr_txtClass.Text += "</tr>"
        Next
        ltr_txtClass.Text += "</table>"

        tr_id.Visible = (m_iID > 0)
        pnl_view.Visible = True
        pnl_viewall.Visible = False

        If Me.m_iID > 0 Then
            drp_country.Enabled = False
            drp_region.Enabled = False
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

    Protected Sub Display_All()
        Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()
        Dim TaxClasscriteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim taxCriteria As New Ektron.Cms.Common.Criteria(Of TaxRateProperty)(TaxRateProperty.TaxClassName, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim postalCriteria As New Ektron.Cms.Common.Criteria(Of TaxRateProperty)(TaxRateProperty.PostalCode, EkEnumeration.OrderByDirection.Ascending)
        Dim m_refCountryTaxRate As New CountryApi
        Dim page_Data As Integer = _CurrentPageNumber
        Dim i As Integer = 0
        Dim taxApi As New TaxApi()
        Dim postalRateList As List(Of TaxRateData)
        Dim iCount As Integer = 0
        Dim k As Integer = 0
        Dim p As Integer = 0
        Dim q As Integer = 0
        Dim r As Integer = 0

        dg_viewall.AutoGenerateColumns = False
        dg_viewall.Columns.Clear()

        _Criteria.PagingInfo.RecordsPerPage = 10

        taxCriteria.PagingInfo.RecordsPerPage = 10
        taxCriteria.Filters.Capacity = 1000

        '''''

        postalCriteria.AddFilter(TaxRateProperty.TaxTypeId, CriteriaFilterOperator.EqualTo, TaxRateType.PostalSalesTax)

        TaxClasscriteria.PagingInfo.RecordsPerPage = 10

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria)

        Dim iCount1 As Integer = taxApi.GetList(postalCriteria).Count / TaxClassList.Count

        postalCriteria.PagingInfo.RecordsPerPage = TaxClassList.Count * m_refContentApi.RequestInformationRef.PagingSize
        postalCriteria.PagingInfo.CurrentPage = _CurrentPageNumber.ToString()

        postalRateList = taxApi.GetList(postalCriteria)

        iCount = postalRateList.Count / TaxClassList.Count

        Dim Postal(postalRateList.Count - 1) As String
        Dim region(postalRateList.Count - 1) As Long

        For Each PostalRate As PostalCodeTaxRateData In postalRateList
            Postal(k) = PostalRate.PostalCode
            region(k) = PostalRate.TypeItemId
            k = k + 1
        Next

        Dim zipcode(iCount) As String
        Dim regionId(iCount) As Long

        If region.Length > 0 Then regionId(p) = region(r)
        If Postal.Length > 0 Then zipcode(q) = Postal(p)
        q = q + 1
        r = r + 1

        For p = 1 To postalRateList.Count - 1
            If Postal(p) <> Postal(p - 1) Then
                zipcode(q) = Postal(p)
                q = q + 1
            End If
        Next

        For p = 1 To postalRateList.Count - 1
            If region(p) <> region(p - 1) Then
                regionId(r) = region(p)
                r = r + 1
            End If
        Next
        ''''

        _TotalPagesNumber = System.Math.Ceiling(iCount1 / m_refContentApi.RequestInformationRef.PagingSize)

        If (_TotalPagesNumber <= 1) Then
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
            TotalPages.Text = (System.Math.Ceiling(_TotalPagesNumber)).ToString()

            CurrentPage.Text = _CurrentPageNumber.ToString()

            If _CurrentPageNumber = 1 Then
                lnkBtnPreviousPage.Enabled = False
                FirstPage.Enabled = False
            ElseIf _CurrentPageNumber = _TotalPagesNumber Then
                NextPage.Enabled = False
                LastPage.Enabled = False
            End If
        End If

        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Id"
        colBound.HeaderText = "Id"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        dg_viewall.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Postal Code"
        colBound.HeaderText = "Postal Code (" & m_refMsg.GetMessage("lbl view tax rate for region") & ")"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        dg_viewall.Columns.Add(colBound)

        dg_viewall.BorderColor = Drawing.Color.White

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("Id", GetType(String)))
        dt.Columns.Add(New DataColumn("Postal Code", GetType(String)))

        If (Not (IsNothing(postalRateList))) Then
            Dim j As Integer = 0
            For i = 0 To (zipcode.Length - 1)
                If zipcode(i) <> "" Then
                    dr = dt.NewRow
                    dr(0) = "<a href=""postaltaxtables.aspx?action=View&postalid=" & zipcode(i).ToString & "&id=" & regionId(i) & """>" & regionId(i) & "</a>"
                    dr(1) = "<a onmouseover=""expandcontent('sc" & i & "')"" onmouseout=""expandcontent('sc" & i & "')"" href=""postaltaxtables.aspx?action=View&postalid=" & zipcode(i).ToString & "&id=" & regionId(i) & """>" & zipcode(i) & "</a>"
                    dr(1) += "<div class=""switchcontent"" style=""position:absolute;"" id=""sc" & i & """>"
                    dr(1) += "<table>"
                    For Each taxClass As TaxClassData In TaxClassList
                        dr(1) &= "<tr><td width=""50%""><label id=""" & taxClass.Name & """>" & taxClass.Name & "</label></td>"
                        dr(1) &= "<td width=""20px""><label id=""value"">" & GetRate(taxClass.Id, regionId(i)) * 100 & "</label>" & "<label id=""lblPercentage"">" & "&nbsp;%" & "</label></td></tr>"
                    Next
                    dr(1) &= "</table></div>"
                    dt.Rows.Add(dr)
                End If
            Next
        End If
        Dim dv As New DataView(dt)

        dg_viewall.DataSource = dv
        dg_viewall.DataBind()
    End Sub

    Protected Sub Display_View()
        Dim TaxClasscriteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()
        Dim postalCriteria As New Ektron.Cms.Common.Criteria(Of TaxRateProperty)(TaxRateProperty.PostalCode, EkEnumeration.OrderByDirection.Ascending)
        Dim page_data As Integer = _CurrentPageNumber
        Dim taxApi As New TaxApi()
        Dim postalCode As String = 0
        Dim id As Long = 0
        Dim postalRateList As List(Of TaxRateData)
        Dim criteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim cCountryId As Long = 0
        Dim txtClassList As Integer = 0

        If Request.QueryString("postalid") <> "" Then
            postalCode = Request.QueryString("postalid")
        End If

        If Request.QueryString("id") <> "" Then
            id = Request.QueryString("id")
        End If

        postalCriteria.PagingInfo.CurrentPage = page_data
        postalCriteria.AddFilter(TaxRateProperty.TaxTypeId, CriteriaFilterOperator.EqualTo, TaxRateType.PostalSalesTax)

        postalRateList = taxApi.GetList(postalCriteria)
        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria)

        txt_name.Text = postalCode
        lbl_id.Text = id
        Util_BindCountries()
        For txtClassList = 0 To TaxClassList.Count - 1
            Dim postalRegion As New PostalCodeTaxRateData
            postalRegion = taxApi.GetItemByPostalId(TaxClassList.Item(txtClassList).Id, id)
            If postalRegion IsNot Nothing Then
                criteria.AddFilter(RegionProperty.Id, CriteriaFilterOperator.EqualTo, postalRegion.RegionId)
                _RegionList = _RegionApi.GetList(criteria)
                drp_region.SelectedValue = _RegionList.Item(0).Id
                cCountryId = _RegionList.Item(0).CountryId
                drp_country.SelectedValue = cCountryId
                Util_BindRegions(cCountryId)
            End If
        Next

        ltr_txtClass.Text = "<table class=""ektronGrid""><br />"
        ltr_txtClass.Text += "<tr><td class=""label""><b><label id=""lbl_taxRate"">" & m_refMsg.GetMessage("lbl tax rates") & ":</label></b></td></tr>"
        For txtClassList = 0 To TaxClassList.Count - 1
            ltr_txtClass.Text += "<tr>"
            ltr_txtClass.Text += "   <td class=""label"">"
            ltr_txtClass.Text += "       <label id=""taxClass" & txtClassList & """ value=""" & TaxClassList.Item(txtClassList).Name & """>" & TaxClassList.Item(txtClassList).Name & ":</label>"
            ltr_txtClass.Text += "   </td>"
            If taxApi.GetItemByPostalId(TaxClassList.Item(txtClassList).Id, id) Is Nothing Then
                ltr_txtClass.Text += "   <td calss=""value"">"
                ltr_txtClass.Text += "       <input disabled=""true"" type=""text"" name=""txtClassRate" & txtClassList & """ id=""txtClassRate" & txtClassList & """ value=""0""/>"
                ltr_txtClass.Text += "   </td>"
            Else
                ltr_txtClass.Text += "   <td calss=""value"">"
                ltr_txtClass.Text += "      <input disabled=""true"" type=""text"" id=""txtClassRate" & txtClassList & """ name=""txtClassRate" & txtClassList & """ value=""" & (taxApi.GetItemByPostalId(TaxClassList.Item(txtClassList).Id, id).Rate * 100) & """/>%  "
                ltr_txtClass.Text += "   </td>"
            End If
            ltr_txtClass.Text += "</tr>"
        Next
        ltr_txtClass.Text += "</table>"

        Util_SetEnabled(False)
        pnl_view.Visible = True
        pnl_viewall.Visible = False

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

#Region "Helpers"

    Protected Function GetRate(ByVal taxClassId As Long, ByVal regionId As Long) As Decimal
        Dim Rate As New TaxRateData
        Dim m_refTaxRate As New Ektron.Cms.Commerce.TaxApi
        m_refTaxRate = New TaxApi()

        Try
            Rate = m_refTaxRate.GetItemByPostalId(taxClassId, regionId)
            Return Rate.Rate
        Catch e As Exception
            Return 0
        End Try
    End Function

    Protected Sub UpdateRegions()
        Dim cCountry As Long = drp_country.SelectedValue
        Util_BindRegions(cCountry)
    End Sub

    Protected Sub Util_SetLabels()
        Select Case Me.m_sPageAction
            Case "addedit"
                Me.AddButtonwithMessages(AppPath & "images/UI/Icons/save.png", _PageName & "?action=addedit&id=" & m_iID.ToString(), "btn save", "btn save", " onclick=""TaxSubmitForm(); return SubmitForm();"" ")
                AddBackButton(_PageName & IIf(m_iID > 0, "?action=view&id=" & Me.m_iID.ToString() & "&postalid=" & txt_name.Text, ""))
                If Me.m_iID > 0 Then
                    SetTitleBarToMessage("lbl edit postal tax rate")
                    AddHelpButton("EditPostalCodeTaxRate")
                Else
                    SetTitleBarToMessage("lbl add postal tax rate")
                    AddHelpButton("AddPostalCodeTaxRate")
                End If
            Case "view"
                Me.AddButtonwithMessages(AppPath & "images/UI/Icons/contentEdit.png", _PageName & "?action=addedit&id=" & m_iID.ToString() & "&postalid=" & txt_name.Text, "generic edit title", "generic edit title", "")
                Me.AddButtonwithMessages(AppPath & "images/UI/Icons/delete.png", _PageName & "?action=del&id=" & m_iID.ToString() & "&postalid=" & txt_name.Text, "generic delete title", "generic delete title", " onclick=""return confirm('" & GetMessage("js confirm delete postal") & "');"" ")
                AddBackButton(_PageName)
                SetTitleBarToMessage("lbl view postal tax rate")
                AddHelpButton("ViewPostalCodeTaxRate")
            Case Else
                Dim newMenu As New workareamenu("file", GetMessage("lbl new"), AppPath & "images/UI/Icons/star.png")
                newMenu.AddLinkItem(AppImgPath & "menu/document.gif", GetMessage("lbl postal"), _PageName & "?action=addedit")
                Me.AddMenu(newMenu)
                SetTitleBarToMessage("lbl postal tax table")
                AddHelpButton("PostalCodeTaxRate")
        End Select

        ltr_name.Text = GetMessage("lbl address postal")
        ltr_id.Text = GetMessage("generic id")
        ltr_region.Text = GetMessage("lbl address region")
        ltr_country.Text = GetMessage("lbl country")
    End Sub

    Protected Sub Util_SetJS()
        Dim sbJS As New StringBuilder()
        Dim TaxClasscriteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria)

        sbJS.Append("<script type='text/javascript'>").Append(Environment.NewLine)

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine)
        sbJS.Append(JSLibrary.AddError("aSubmitErr"))
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"))
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"))
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection))

        sbJS.Append(" function validate_Title() {").Append(Environment.NewLine)
        sbJS.Append("   var sTitle = Trim(document.getElementById('").Append(txt_name.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var cCountryId = Trim(document.getElementById('").Append(drp_country.UniqueID).Append("').value);").Append(Environment.NewLine)
        sbJS.Append("   if (sTitle == '' || sTitle == 0) { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err postal code title req")).Append("'); document.forms['form1'].isCPostData.value = 'false'; } ").Append(Environment.NewLine)
        sbJS.Append("   if(cCountryId == '840'){").Append(Environment.NewLine)
        sbJS.Append("       if(!ValidatePostalCode(sTitle))").Append(Environment.NewLine)
        sbJS.Append("       {").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err postal code title req")).Append("');  ").Append(Environment.NewLine)
        sbJS.Append("           document.forms[""form1""].isCPostData.value = 'false';").Append(Environment.NewLine)
        sbJS.Append("       }").Append(Environment.NewLine)
        sbJS.Append("   }").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar('").Append(txt_name.UniqueID).Append("',""").Append(GetMessage("lbl region disallowed chars")).Append("""); ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append("   function ValidatePostalCode(postalCodeText){").Append(Environment.NewLine)
        sbJS.Append("       var regEx = /^\d{5}(-\d{4})?$/; ").Append(Environment.NewLine)
        sbJS.Append("       return (regEx.test(postalCodeText));").Append(Environment.NewLine)
        sbJS.Append("    }").Append(Environment.NewLine)

        sbJS.Append("function SubmitForm()").Append(Environment.NewLine)
        sbJS.Append("{").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine)
        sbJS.Append("   var taxClass = ").Append(TaxClassList.Count).Append(";").Append(Environment.NewLine)
        sbJS.Append("   var i = 0;").Append(Environment.NewLine)
        sbJS.Append("   var drp_region = document.getElementById(""").Append(drp_region.UniqueID).Append(""");" & Environment.NewLine)
        sbJS.Append("   if(drp_region.selectedIndex == -1)").Append(Environment.NewLine)
        sbJS.Append("    {").Append(Environment.NewLine)
        sbJS.Append("       alert(""" & MyBase.GetMessage("js null postalcode region msg") & """);" & Environment.NewLine)
        sbJS.Append("       document.forms[""form1""].isCPostData.value = 'false';").Append(Environment.NewLine)
        sbJS.Append("       return false;").Append(Environment.NewLine)
        sbJS.Append("    }").Append(Environment.NewLine)
        sbJS.Append("   for (i = 0; i < taxClass; i++)").Append(Environment.NewLine)
        sbJS.Append("   {").Append(Environment.NewLine)
        sbJS.Append("       var taxField = Trim(document.getElementById('txtClassRate' + i)); ").Append(Environment.NewLine)
        sbJS.Append("       if(taxField.value == '')").Append(Environment.NewLine)
        sbJS.Append("       {").Append(Environment.NewLine)
        sbJS.Append("           taxField.value = 0;").Append(Environment.NewLine)
        sbJS.Append("       }").Append(Environment.NewLine)
        sbJS.Append("       if(isNaN(taxField.value) || taxField.value > 99)").Append(Environment.NewLine)
        sbJS.Append("       {").Append(Environment.NewLine)
        sbJS.Append("          ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err postal code tax value")).Append("');").Append(Environment.NewLine)
        sbJS.Append("           break;").Append(Environment.NewLine)
        sbJS.Append("       }").Append(Environment.NewLine)
        sbJS.Append("   }").Append(Environment.NewLine)
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("('document.forms[0].submit();');").Append(Environment.NewLine)
        sbJS.Append("   return false; ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)
        sbJS.Append("</script>").Append(Environment.NewLine)

        ltr_js.Text &= sbJS.ToString()
    End Sub

    Protected Sub Util_SetEnabled(ByVal toggle As Boolean)
        Me.txt_name.Enabled = toggle
        'chk_enabled.Enabled = toggle
        drp_country.Enabled = toggle
        drp_region.Enabled = toggle
    End Sub

    Protected Sub Util_BindCountries()
        If _CountryList IsNot Nothing AndAlso _CountryList.Count > 0 Then
            drp_country.DataSource = _CountryList
            drp_country.DataTextField = "Name"
            drp_country.DataValueField = "Id"
            drp_country.DataBind()
        End If
    End Sub

    Protected Sub Util_BindRegions(ByVal cCountryId As Integer)
        Dim criteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        criteria.AddFilter(RegionProperty.CountryId, CriteriaFilterOperator.EqualTo, cCountryId)
        If Request.QueryString("postalid") Is Nothing OrElse 0 = Request.QueryString("postalid").Length Then
            criteria.AddFilter(RegionProperty.IsEnabled, CriteriaFilterOperator.EqualTo, "True")
        End If
        criteria.PagingInfo.RecordsPerPage = 10000

        _RegionList = _RegionApi.GetList(criteria)
        Try
            If _RegionList IsNot Nothing AndAlso _RegionList.Count > 0 Then
                drp_region.DataSource = _RegionList
                drp_region.DataTextField = "Name"
                drp_region.DataValueField = "Id"
                drp_region.DataBind()
            Else
                drp_region.DataSource = ""
                drp_region.DataTextField = "Name"
                drp_region.DataValueField = "Id"
                drp_region.DataBind()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Function Util_GetCountryIndex(ByVal countryId As Integer) As Integer
        Dim iRet As Integer = -1
        If _CountryList IsNot Nothing AndAlso _CountryList.Count > 0 Then
            For i As Integer = 0 To (_CountryList.Count - 1)
                If _CountryList(i).Id = countryId Then iRet = i
            Next
        End If
        Return iRet
    End Function

    Protected Function Util_GetCountryName(ByVal countryId As Integer) As String
        Dim sRet As String = ""
        If _CountryList IsNot Nothing AndAlso _CountryList.Count > 0 Then
            For i As Integer = 0 To (_CountryList.Count - 1)
                If _CountryList(i).Id = countryId Then sRet = _CountryList(i).Name
            Next
        End If
        Return sRet
    End Function

    Protected Function Util_GetRegionIndex(ByVal regionId As Integer) As Integer
        Dim iRet As Integer = -1
        If _RegionList IsNot Nothing AndAlso _RegionList.Count > 0 Then
            For i As Integer = 0 To (_RegionList.Count - 1)
                If _RegionList(i).Id = regionId Then iRet = i
            Next
        End If
        Return iRet
    End Function

    Protected Function Util_GetRegionName(ByVal RegionId As Integer) As String
        Dim sRet As String = ""
        If _RegionList IsNot Nothing AndAlso _RegionList.Count > 0 Then
            For i As Integer = 0 To (_RegionList.Count - 1)
                If _RegionList(i).Id = RegionId Then sRet = _RegionList(i).Name
            Next
        End If
        Return sRet
    End Function

    Protected Sub Util_CheckAccess()

        Try
            If Not Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin) Then
                Throw New Exception(GetMessage("err not role commerce-admin"))
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try

    End Sub

    Private Sub Util_RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS)
    End Sub

#End Region

#Region "JS/CSS"

    Private Sub RegisterJS()

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)

    End Sub

    Private Sub RegisterCSS()

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

    End Sub

#End Region

End Class
