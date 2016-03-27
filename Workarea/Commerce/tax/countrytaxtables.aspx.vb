Imports Ektron
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea
Imports Ektron.Cms.Commerce
Imports System.data
Imports System.Web.HttpRequest
Imports System.Web.UI.page
Partial Class Commerce_tax_countrytaxtables
    Inherits workareabase

#Region "Member Variables"

    Protected _RegionApi As RegionApi
    Protected _TaxApi As TaxApi
    Protected _CountryApi As CountryApi
    Protected _TaxClassApi As New Ektron.Cms.Commerce.TaxClassApi()
    Protected _PageName As String = "countrytaxtables.aspx"
    Protected _CountryList As New System.Collections.Generic.List(Of CountryData)()
    Protected _CountryCriteria As New Ektron.Cms.Common.Criteria(Of CountryProperty)(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
    Protected _CurrentPageNumber As Integer = 1
    Protected _TotalPagesNumber As Integer = 1
    Protected _SearchCriteria As String = ""
    Protected _ValidateResult As New Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults

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
        If Page.Request.QueryString("search") <> "" Then _SearchCriteria = Page.Request.QueryString("search")
        _RegionApi = New RegionApi() '(Me.m_refContentApi.RequestInformationRef)
        _CountryApi = New CountryApi() '(Me.m_refContentApi.RequestInformationRef)
        _CountryCriteria.PagingInfo = New PagingInfo(10000)

        hdnCurrentPage.Value = CurrentPage.Text
        Select Case Me.m_sPageAction
            Case "addedit"
                _CountryList = _CountryApi.GetList(_CountryCriteria)
                If Page.IsPostBack Then
                    Process_AddEdit()
                Else
                    Display_AddEdit()
                End If
            Case "del"
                Process_Delete()
            Case "view"
                _CountryList = _CountryApi.GetList(_CountryCriteria)
                Display_View()
            Case Else
                _CountryCriteria.Condition = LogicalOperation.Or
                If _SearchCriteria <> "" Then _CountryCriteria.AddFilter(CountryProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria)
                If _SearchCriteria <> "" Then _CountryCriteria.AddFilter(CountryProperty.LongIsoCode, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria)
                If _SearchCriteria <> "" Then _CountryCriteria.AddFilter(CountryProperty.ShortIsoCode, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria)
                _CountryList = _CountryApi.GetList(_CountryCriteria)
                If Page.IsPostBack = False Then
                    Display_All()
                End If
        End Select
        Util_SetLabels()
        Util_SetJS()
    End Sub

    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        If hdnCurrentPage.Value <> "" Then
            _CurrentPageNumber = Int32.Parse(hdnCurrentPage.Value)
        End If
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
        Dim m_CountryTax As New Ektron.Cms.Commerce.CountryTaxRateData
        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria)
        m_CountryTax = New CountryTaxRateData()
        _TaxApi = New TaxApi()
        Dim j As Integer = 0

        If Me.m_iID > 0 Then
            cCountry = _CountryApi.GetItem(Me.m_iID)
            cCountry.Name = txt_name.Text
            cCountry.LongIsoCode = txt_long.Text
            cCountry.ShortIsoCode = txt_short.Text
            cCountry.Enabled = chk_enabled.Checked
            _CountryApi.Update(cCountry)

            For i As Integer = 0 To TaxClassList.Count - 1
                tTax = _TaxApi.GetItemByCountryId(TaxClassList.Item(i).Id, cCountry.Id)
                If tTax Is Nothing Then
                    tTax = New CountryTaxRateData(cCountry.Id, TaxClassList.Item(i).Id, 0)
                    If IsNumeric(Request.Form("txtClassRate" & i)) Then
                        tTax.Rate = CDec(Request.Form("txtClassRate" & i) / 100)
                        _TaxApi.Add(tTax)
                    End If
                Else
                    If IsNumeric(Request.Form("txtClassRate" & i)) Then
                        tTax.Rate = CDec(Request.Form("txtClassRate" & i) / 100)
                        _TaxApi.Update(tTax)
                    End If
                End If
            Next

            Response.Redirect(_PageName & "?action=view&id=" & m_iID.ToString(), False)
        Else
            cCountry = New CountryData(txt_numeric.Text, txt_name.Text, txt_short.Text, txt_long.Text, chk_enabled.Checked)
            _CountryApi.Add(cCountry)
            Dim TypeId As Integer = 0

            For i As Integer = 0 To TaxClassList.Count - 1
                tTax = New CountryTaxRateData(cCountry.Id, TaxClassList.Item(i).Id, 0)
                If IsNumeric(Request.Form("txtClassRate" & i)) Then
                    tTax.Rate = CDec(Request.Form("txtClassRate" & i) / 100)
                    _TaxApi.Add(tTax)
                End If
            Next

            Response.Redirect(_PageName, False)
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
        If Me.m_iID > 0 Then _CountryApi.Delete(m_iID)
        Response.Redirect(_PageName, False)
    End Sub
#End Region

#Region "Display"
    Protected Sub Display_AddEdit()
        Dim cCountry As CountryData = New CountryData()

        Dim tTax As CountryTaxRateData = Nothing
        Dim TaxClasscriteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria)
        _TaxApi = New TaxApi()

        If m_iID > 0 Then
            cCountry = _CountryApi.GetItem(Me.m_iID)
            txt_name.Enabled = False
            chk_enabled.Enabled = False
            txt_long.Enabled = False
            txt_short.Enabled = False
            txt_numeric.Text = Me.m_iID
            txt_numeric.Enabled = False
        End If

        Util_BindCountries()

        txt_name.Text = cCountry.Name
        lbl_id.Text = cCountry.Id
        chk_enabled.Checked = cCountry.Enabled
        txt_long.Text = cCountry.LongIsoCode()
        txt_short.Text = cCountry.ShortIsoCode

        Dim txtClassList As Integer = 0

        ltr_txtClass.Text = "<table class=""ektronGrid"">"
        For txtClassList = 0 To TaxClassList.Count - 1
            ltr_txtClass.Text += "<tr>"
            ltr_txtClass.Text += "   <td class=""label"">"
            ltr_txtClass.Text += "       <label id=""taxClass" & txtClassList & """ value=""" & TaxClassList.Item(txtClassList).Name & """>" & TaxClassList.Item(txtClassList).Name & ":</label>"
            ltr_txtClass.Text += "   </td>"
            If _TaxApi.GetItemByCountryId(TaxClassList.Item(txtClassList).Id, cCountry.Id) Is Nothing Then
                ltr_txtClass.Text += "   <td class=""value"">"
                ltr_txtClass.Text += "       <input type=""text"" name=""txtClassRate" & txtClassList & """ id=""txtClassRate" & txtClassList & """ value=""0"" />%"
                ltr_txtClass.Text += "   </td>"
            Else
                ltr_txtClass.Text += "   <td class=""value"">"
                ltr_txtClass.Text += "       <input type=""text"" name=""txtClassRate" & txtClassList & """ id=""txtClassRate" & txtClassList & """ value=""" & _TaxApi.GetItemByCountryId(TaxClassList.Item(txtClassList).Id, cCountry.Id).Rate * 100 & """/>%"
                ltr_txtClass.Text += "   </td>"
            End If
            ltr_txtClass.Text += "<td >"
            ltr_txtClass.Text += "</td>"
            ltr_txtClass.Text += "</tr>"
        Next
        ltr_txtClass.Text += "</table>"


        tr_id.Visible = (m_iID > 0)
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
    Protected Sub Display_All()
        Dim RateList As New System.Collections.Generic.List(Of TaxRateData)()
        Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()
        Dim TaxClasscriteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim taxCriteria As New Ektron.Cms.Common.Criteria(Of TaxRateProperty)(TaxRateProperty.TaxClassName, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim countryCriteria As New Ektron.Cms.Common.Criteria(Of CountryProperty)(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim i As Integer = 0
        dg_viewall.PageSize = m_refContentApi.RequestInformationRef.PagingSize
        countryCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        countryCriteria.PagingInfo.CurrentPage = _CurrentPageNumber.ToString()

        _TaxApi = New TaxApi()

        dg_viewall.AutoGenerateColumns = False
        dg_viewall.Columns.Clear()

        taxCriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize

        Dim CountryRateList As New System.Collections.Generic.List(Of CountryData)
        Dim m_refCountryTaxRate As New CountryApi
        m_refCountryTaxRate = New CountryApi()

        countryCriteria.Condition = LogicalOperation.Or
        If _SearchCriteria <> "" Then countryCriteria.AddFilter(CountryProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria)
        If _SearchCriteria <> "" Then countryCriteria.AddFilter(CountryProperty.LongIsoCode, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria)
        If _SearchCriteria <> "" Then countryCriteria.AddFilter(CountryProperty.ShortIsoCode, Ektron.Cms.Common.CriteriaFilterOperator.Contains, _SearchCriteria)

        CountryRateList = m_refCountryTaxRate.GetList(countryCriteria)

        TaxClasscriteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria)

        _TotalPagesNumber = countryCriteria.PagingInfo.TotalPages

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
        colBound.DataField = "Name"
        colBound.HeaderText = "Name (" & m_refMsg.GetMessage("lbl view tax rate for region") & ")"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        dg_viewall.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Enabled"
        colBound.HeaderText = "Enabled"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        dg_viewall.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Code"
        colBound.HeaderText = "Code"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        dg_viewall.Columns.Add(colBound)

        dg_viewall.BorderColor = Drawing.Color.White

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("Id", GetType(String)))
        dt.Columns.Add(New DataColumn("Name", GetType(String)))
        dt.Columns.Add(New DataColumn("Enabled", GetType(String)))
        dt.Columns.Add(New DataColumn("Code", GetType(String)))
        dt.Columns.Add(New DataColumn("Country", GetType(String)))

        If (Not (IsNothing(CountryRateList))) Then
            Dim j As Integer = 0
            For i = 0 To (CountryRateList.Count - 1)
                dr = dt.NewRow
                dr(0) = "<a  href=""countrytaxtables.aspx?action=View&id=" & CountryRateList.Item(i).Id & """>" & CountryRateList.Item(i).Id & "</a>"
                dr(1) = "<a onclick=""expandcontent('sc" & i & "')"">" & CountryRateList.Item(i).Name & "</a><br />"

                dr(1) += "<div class=""switchcontent"" id=""sc" & i & """><table class=""ektronForm""><a onclick=""expandcontent('sc" & i & "')"" href=""countrytaxtables.aspx?action=View&id=" & CountryRateList.Item(i).Id & """>" & m_refMsg.GetMessage("lbl view tax rate") & "</a><br />"
                For Each taxClass As TaxClassData In TaxClassList
                    dr(1) &= "<tr><td><br/><label class=""label"" id=""" & taxClass.Name & """>" & taxClass.Name & "</label></td>"
                    dr(1) &= "<td><input type=""text"" size=""10"" align=""right"" name=""value"" readonly=""true"" id=""value"" value=""" & GetRate(taxClass.Id, CountryRateList.Item(i).Id) * 100 & """/>" & "<label id=""lblPercentage"">" & "%" & "</label></td></tr>"
                Next

                dr(1) += "</table></div>"

                dr(2) = "<input type=""CheckBox"" ID=""chk_enabled" & i & """ disabled=""true""  " & IIf(CountryRateList.Item(i).Enabled, "Checked=""checked""", "") & "/>"
                dr(3) = "<a href=""countrytaxtables.aspx?action=View&id=" & CountryRateList.Item(i).Id & """>" & CountryRateList.Item(i).ShortIsoCode & "</a>"
                dt.Rows.Add(dr)
            Next
        End If
        Dim dv As New DataView(dt)

        dg_viewall.DataSource = dv
        dg_viewall.DataBind()
    End Sub

    Protected Sub Display_View()
        Dim cCountry As CountryData = Nothing
        Dim tTax As TaxRateData = Nothing
        Dim TaxClasscriteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()

        Util_BindCountries()

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria)
        _TaxApi = New TaxApi()
        cCountry = _CountryApi.GetItem(Me.m_iID)

        txt_name.Text = cCountry.Name
        lbl_id.Text = cCountry.Id
        chk_enabled.Checked = cCountry.Enabled
        txt_long.Text = cCountry.LongIsoCode()
        txt_short.Text = cCountry.ShortIsoCode

        If Me.m_iID > 0 Then
            tr_numIso.Visible = False
        End If

        Dim txtClassList As Integer = 0
        ltr_txtClass.Text = "<table class=""ektronGrid"">"
        For txtClassList = 0 To TaxClassList.Count - 1
            ltr_txtClass.Text += "<tr>"
            ltr_txtClass.Text += "   <td class=""label"">"
            ltr_txtClass.Text += "       <label id=""taxClass" & txtClassList & """ value=""" & TaxClassList.Item(txtClassList).Name & """>" & TaxClassList.Item(txtClassList).Name & ":</label>"
            ltr_txtClass.Text += "   </td>"
            If _TaxApi.GetItemByCountryId(TaxClassList.Item(txtClassList).Id, cCountry.Id) Is Nothing Then
                ltr_txtClass.Text += "   <td class=""value"">"
                ltr_txtClass.Text += "       <input disabled=""true"" type=""text"" name=""txtClassRate" & txtClassList & """ id=""txtClassRate" & txtClassList & """ value=""0""/>%"
                ltr_txtClass.Text += "   </td>"
            Else
                ltr_txtClass.Text += "   <td class=""value"">"
                ltr_txtClass.Text += "       <input disabled=""true"" type=""text"" id=""txtClassRate" & txtClassList & """ name=""txtClassRate" & txtClassList & """ value=""" & _TaxApi.GetItemByCountryId(TaxClassList.Item(txtClassList).Id, cCountry.Id).Rate * 100 & """/>%  "
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

    Protected Sub Util_SetLabels()
        Select Case Me.m_sPageAction
            Case "addedit"
                Me.AddButtonwithMessages(AppImgPath & "../UI/Icons/save.png", _PageName & "?action=addedit&id=" & m_iID.ToString(), "btn save", "btn save", " onclick="" return SubmitForm();"" ")
                AddBackButton(_PageName & IIf(m_iID > 0, "?action=view&id=" & Me.m_iID.ToString(), ""))
                If Me.m_iID > 0 Then
                    SetTitleBarToMessage("lbl edit country tax rate")
                    AddHelpButton("editcountrytaxrate")
                Else
                    SetTitleBarToMessage("lbl add country tax rate")
                    AddHelpButton("addcountrytaxrate")
                End If
            Case "view"
                Me.AddButtonwithMessages(AppImgPath & "../UI/Icons/contentEdit.png", _PageName & "?action=addedit&id=" & m_iID.ToString(), "generic edit title", "generic edit title", "")
                If _CountryApi.CanDelete(Me.m_iID, _ValidateResult) Then
                    Me.AddButtonwithMessages(AppImgPath & "../UI/Icons/delete.png", _PageName & "?action=del&id=" & m_iID.ToString(), "generic delete title", "generic delete title", " onclick=""return confirm('" & GetMessage("js confirm delete country") & "');"" ")
                End If
                AddBackButton(_PageName)
                SetTitleBarToMessage("lbl view country tax rate")
                AddHelpButton("viewcountrytaxrate")
            Case Else

                Me.AddSearchBox(Server.HtmlEncode(_SearchCriteria), New ListItemCollection(), "searchCountry")
                SetTitleBarToMessage("lbl country tax table")
                AddHelpButton("countrytaxtable")

        End Select

        ltr_name.Text = GetMessage("generic name")
        ltr_id.Text = GetMessage("generic id")
        ltr_enabled.Text = GetMessage("enabled")
        ltr_long.Text = GetMessage("lbl longisocode")
        ltr_numeric.Text = GetMessage("lbl numericisocode")
        ltr_short.Text = GetMessage("lbl shortisocode")
    End Sub

    Protected Sub Util_SetJS()
        Dim sbJS As New StringBuilder()
        Dim TaxClasscriteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()

        TaxClassList = _TaxClassApi.GetList(TaxClasscriteria)

        sbJS.Append("<script language=""javascript"">").Append(Environment.NewLine)

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine)
        sbJS.Append(JSLibrary.AddError("aSubmitErr"))
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"))
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"))
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection))

        sbJS.Append(" function validate_Title() { ").Append(Environment.NewLine)
        sbJS.Append("   var sTitle = Trim(document.getElementById('").Append(txt_name.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var iNumISO = Trim(document.getElementById('").Append(txt_numeric.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var shortISO = Trim(document.getElementById('").Append(txt_short.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var slngISO = Trim(document.getElementById('").Append(txt_long.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   if (sTitle == '' || iNumISO == '' || isNaN(iNumISO) || shortISO == '' || slngISO == '') { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err fields req")).Append("'); } ").Append(Environment.NewLine)
        'sbJS.Append("   if (sTitle == '') { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err region title req")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar('").Append(txt_name.UniqueID).Append("',""").Append(GetMessage("lbl region disallowed chars")).Append("""); ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append(" function SubmitForm() {").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine)
        sbJS.Append("   var taxClass = ").Append(TaxClassList.Count).Append(";").Append(Environment.NewLine)
        sbJS.Append("   var i = 0;").Append(Environment.NewLine)
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

        sbJS.Append(" function searchCountry() { ").Append(Environment.NewLine)
        sbJS.Append("   var sSearchTerm = Trim(document.getElementById('txtSearch').value); ").Append(Environment.NewLine)
        sbJS.Append("   if (sSearchTerm != '') { window.location.href = '").Append(_PageName).Append("?search=' + sSearchTerm;} else { alert('").Append(GetMessage("js err please enter text")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append("</script>").Append(Environment.NewLine)

        ltr_js.Text &= sbJS.ToString()
    End Sub

    Protected Sub Util_SetEnabled(ByVal toggle As Boolean)
        Me.txt_name.Enabled = toggle
        txt_long.Enabled = toggle
        txt_numeric.Enabled = toggle
        txt_short.Enabled = toggle
        chk_enabled.Enabled = toggle
    End Sub

    Protected Sub Util_BindCountries()
        If _CountryList IsNot Nothing AndAlso _CountryList.Count > 0 Then
            'drp_country.DataSource = CountryList
            'drp_country.DataTextField = "Name"
            'drp_country.DataValueField = "Id"
            'drp_country.DataBind()
        End If
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

    Protected Function GetRate(ByVal taxClassId As Long, ByVal regionId As Long) As Decimal
        Dim Rate As New TaxRateData
        Dim m_refTaxRate As New Ektron.Cms.Commerce.TaxApi
        m_refTaxRate = New TaxApi()

        Try
            Rate = m_refTaxRate.GetItemByCountryId(taxClassId, regionId)
            Return Rate.Rate
        Catch e As Exception
            Return 0
        End Try
    End Function

#End Region

#Region "JS/CSS"

    Private Sub RegisterJS()

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)

    End Sub

    Private Sub RegisterCSS()

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

    End Sub

#End Region

End Class