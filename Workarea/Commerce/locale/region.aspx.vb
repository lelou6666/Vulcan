Imports Ektron
Imports Ektron.Cms
Imports Ektron.Cms.Workarea
Imports Ektron.Cms.Commerce
Imports System.data
Imports System.Web.HttpRequest
Imports System.Web.UI.page

Partial Class Commerce_locale_region
    Inherits workareabase

    Protected m_refRegion As RegionApi = Nothing
    Protected m_refCountry As CountryApi = Nothing
    Protected m_sPageName As String = "region.aspx"
    Protected CountryList As New System.Collections.Generic.List(Of CountryData)()
    Protected criteria As New Ektron.Cms.Common.Criteria(Of CountryProperty)(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1
    Protected searchCriteria As String = ""
    Protected countryId As Long = 0
    Protected validateResult As New Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults

#Region "Page Functions"
    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        RegisterResource()
		Utilities.ValidateUserLogin()
        Util_CheckAccess()

        If Page.Request.QueryString("search") <> "" Then searchCriteria = Page.Request.QueryString("search")
        If Page.Request.QueryString("country") <> "" Then countryId = Page.Request.QueryString("country")

        Try

            m_refRegion = New RegionApi()
            m_refCountry = New CountryApi()
            criteria.PagingInfo = New PagingInfo(10000)
            criteria.AddFilter(CountryProperty.IsEnabled, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, True)
            If countryId > 0 Then criteria.AddFilter(CountryProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, countryId)

            hdnCurrentPage.Value = CurrentPage.Text
            Select Case Me.m_sPageAction
                Case "addedit"
                    CountryList = m_refCountry.GetList(criteria)
                    If Page.IsPostBack Then
                        Process_AddEdit()
                    Else
                        Display_AddEdit()
                    End If
                Case "del"
                    Process_Delete()
                Case "view"
                    CountryList = m_refCountry.GetList(criteria)
                    Display_View()
                Case Else
                    CountryList = m_refCountry.GetList(criteria)
                    If Page.IsPostBack = False Then
                        Display_All()
                    End If
            End Select
            Util_SetLabels()
            Util_SetJS()

        Catch ex As Exception
            If ex.Message.IndexOf("unique key") Then
                Utilities.ShowError(GetMessage("lbl region dupe"))
            Else
                Utilities.ShowError(ex.Message)
            End If
        End Try
    End Sub
#End Region

#Region "Process"
    Protected Sub Process_AddEdit()

        Dim rRegion As RegionData = Nothing
        If Me.m_iID > 0 Then
            rRegion = m_refRegion.GetItem(Me.m_iID)
            rRegion.Name = txt_name.Text
            rRegion.CountryId = drp_country.SelectedValue
            rRegion.Code = txt_code.Text
            rRegion.Enabled = chk_enabled.Checked
            m_refRegion.Update(rRegion)
            Response.Redirect(m_sPageName & "?action=view&id=" & m_iID.ToString(), False)
        Else
            rRegion = New RegionData(txt_name.Text, drp_country.SelectedValue, txt_code.Text, chk_enabled.Checked)
            m_refRegion.Add(rRegion)
            If chk_addanother.Checked Then
                Response.Redirect(m_sPageName & "?action=addedit&country=" & drp_country.SelectedValue, False)
            Else
                Response.Redirect(m_sPageName, False)
            End If
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
        Dim results As New Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults()
        If Me.m_iID > 0 Then
            If (Not m_refRegion.CanDelete(Me.m_iID, results)) Then
                Dim msg As New StringBuilder()
                Utilities.ShowError(Ektron.Cms.Common.EkFunctions.GetAllValidationMessages(results))
            Else
                m_refRegion.Delete(m_iID)
                Response.Redirect(m_sPageName, False)
            End If
        End If
    End Sub
#End Region

#Region "Display"
    Protected Sub Display_AddEdit()
        Dim rRegion As RegionData = New RegionData()
        If m_iID > 0 Then rRegion = m_refRegion.GetItem(Me.m_iID)

        Util_BindCountries()

        txt_name.Text = rRegion.Name
        lbl_id.Text = rRegion.Id
        chk_enabled.Checked = (countryId > 0) OrElse rRegion.Enabled
        txt_code.Text = rRegion.Code
        drp_country.SelectedIndex = Util_GetCountryIndex(rRegion.CountryId)
        chk_addanother.Checked = (countryId > 0)

        tr_addanother.Visible = (m_iID = 0)
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
        Dim RegionList As New System.Collections.Generic.List(Of RegionData)()
        Dim criteria As New Ektron.Cms.Common.Criteria(Of RegionProperty)(RegionProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim i As Integer = 0
        Dim modular As Integer = 0
        dg_viewall.PageSize = m_refContentApi.RequestInformationRef.PagingSize
        dg_viewall.AutoGenerateColumns = False
        dg_viewall.Columns.Clear()

        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        criteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        If searchCriteria <> "" Then criteria.AddFilter(RegionProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, searchCriteria)

        RegionList = m_refRegion.GetList(criteria)

        TotalPagesNumber = criteria.PagingInfo.TotalPages

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
        colBound.HeaderText = "Id"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.HeaderStyle.CssClass = "title-header"
        dg_viewall.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Name"
        colBound.HeaderText = "Name"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.HeaderStyle.CssClass = "title-header"
        dg_viewall.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Enabled"
        colBound.HeaderText = "Enabled"
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        colBound.HeaderStyle.CssClass = "title-header"
        dg_viewall.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Code"
        colBound.HeaderText = "Code"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.HeaderStyle.CssClass = "title-header"
        dg_viewall.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "Country"
        colBound.HeaderText = "Country"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.HeaderStyle.CssClass = "title-header"
        dg_viewall.Columns.Add(colBound)

        dg_viewall.BorderColor = Drawing.Color.White

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("Id", GetType(String)))
        dt.Columns.Add(New DataColumn("Name", GetType(String)))
        dt.Columns.Add(New DataColumn("Enabled", GetType(String)))
        dt.Columns.Add(New DataColumn("Code", GetType(String)))
        dt.Columns.Add(New DataColumn("Country", GetType(String)))

        If (Not (IsNothing(RegionList))) Then
            For i = 0 To RegionList.Count - 1
                dr = dt.NewRow
                dr(0) = "<a href=""region.aspx?action=View&id=" & RegionList.Item(i).Id & """>" & RegionList.Item(i).Id & "</a>"
                dr(1) = "<a href=""region.aspx?action=View&id=" & RegionList.Item(i).Id & """>" & RegionList.Item(i).Name & "</a>"
                dr(2) = "<input type=""CheckBox"" ID=""chk_enabled" & i & """ disabled=""true"" " & IIf(RegionList.Item(i).Enabled, "Checked=""checked""", "") & "/>"
                dr(3) = "<a href=""region.aspx?action=View&id=" & RegionList.Item(i).Id & """>" & RegionList.Item(i).Code & "</a>"
                dr(4) = "<label id=""lblCountry"" >" & Util_GetCountryName(RegionList.Item(i).CountryId) & "</label>"
                dt.Rows.Add(dr)
            Next
        End If
        Dim dv As New DataView(dt)

        dg_viewall.DataSource = dv
        dg_viewall.DataBind()
    End Sub

    Protected Sub Display_View()
        Dim rRegion As RegionData = Nothing

        Util_BindCountries()

        rRegion = m_refRegion.GetItem(Me.m_iID)

        txt_name.Text = rRegion.Name
        lbl_id.Text = rRegion.Id
        chk_enabled.Checked = rRegion.Enabled
        txt_code.Text = rRegion.Code
        drp_country.SelectedIndex = Util_GetCountryIndex(rRegion.CountryId)

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
        tr_addanother.Visible = False
    End Sub
#End Region

#Region "Private Helpers"
    Protected Sub Util_SetLabels()
        Select Case Me.m_sPageAction
            Case "addedit"
                Me.AddButtonwithMessages(AppImgPath & "../UI/Icons/save.png", m_sPageName & "?action=addedit&id=" & m_iID.ToString(), "btn save", "btn save", " onclick="" return SubmitForm();"" ")
                AddBackButton(m_sPageName & IIf(m_iID > 0, "?action=view&id=" & Me.m_iID.ToString(), ""))
                If Me.m_iID > 0 Then
                    SetTitleBarToMessage("lbl edit region")
                    AddHelpButton("EditRegion")
                Else
                    SetTitleBarToMessage("lbl add region")
                    AddHelpButton("AddRegion")
                End If

            Case "view"
                Me.AddButtonwithMessages(AppImgPath & "../UI/Icons/contentEdit.png", m_sPageName & "?action=addedit&id=" & m_iID.ToString(), "generic edit title", "generic edit title", "")
                If m_refRegion.CanDelete(Me.m_iID, validateResult) Then
                    Me.AddButtonwithMessages(AppImgPath & "../UI/Icons/delete.png", m_sPageName & "?action=del&id=" & m_iID.ToString(), "generic delete title", "generic delete title", " onclick=""return confirm('" & GetMessage("js confirm delete Region") & "');"" ")
                End If
                AddBackButton(m_sPageName)
                SetTitleBarToMessage("lbl view region")
                AddHelpButton("ViewRegion")
            Case Else
                Dim newMenu As New workareamenu("file", GetMessage("lbl new"), AppImgPath & "../UI/Icons/star.png")
                newMenu.AddLinkItem(AppImgPath & "/menu/document.gif", GetMessage("lbl Region"), m_sPageName & "?action=addedit")
                Me.AddMenu(newMenu)

                Me.AddSearchBox(Server.HtmlEncode(searchCriteria), New ListItemCollection(), "searchRegion")
                SetTitleBarToMessage("lbl regions")
                AddHelpButton("region")
        End Select
        ltr_name.Text = GetMessage("generic name")
        ltr_id.Text = GetMessage("generic id")
        ltr_enabled.Text = GetMessage("enabled")
        ltr_code.Text = GetMessage("lbl code")
        ltr_country.Text = GetMessage("lbl address country")
        ltr_addanother.Text = GetMessage("lbl add another region")
    End Sub
    Protected Sub Util_SetJS()
        Dim sbJS As New StringBuilder()

        sbJS.Append("<script language=""javascript"">").Append(Environment.NewLine)

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine)
        sbJS.Append(JSLibrary.AddError("aSubmitErr"))
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"))
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"))
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection))

        sbJS.Append(" function validate_Title() { ").Append(Environment.NewLine)
        sbJS.Append("   var sTitle = Trim(document.getElementById('").Append(txt_name.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   if (sTitle == '') { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err region title req")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   var sCode = Trim(document.getElementById('").Append(txt_code.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   if ( sCode == '' || sCode.length > 3 || sCode.length < 2) { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err region code req")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar('").Append(txt_name.UniqueID).Append("',""").Append(GetMessage("lbl region disallowed chars")).Append("""); ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append(" function SubmitForm() { ").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine)
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("('document.forms[0].submit();');").Append(Environment.NewLine)
        sbJS.Append("   return false; ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append(" function searchRegion() { ").Append(Environment.NewLine)
        sbJS.Append("   var sSearchTerm = $ektron('#txtSearch').getInputLabelValue(); ").Append(Environment.NewLine)
        sbJS.Append("   if (sSearchTerm != '') { window.location.href = '").Append(m_sPageName).Append("?search=' + sSearchTerm;} else { alert('").Append(GetMessage("js err please enter text")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append("</script>").Append(Environment.NewLine)

        ltr_js.Text &= sbJS.ToString()
    End Sub
    Protected Sub Util_SetEnabled(ByVal toggle As Boolean)
        Me.txt_name.Enabled = toggle
        txt_code.Enabled = toggle
        chk_enabled.Enabled = toggle
        drp_country.Enabled = toggle
    End Sub
    Protected Sub Util_BindCountries()
        If CountryList IsNot Nothing AndAlso CountryList.Count > 0 Then
            drp_country.DataSource = CountryList
            drp_country.DataTextField = "Name"
            drp_country.DataValueField = "Id"
            drp_country.DataBind()
        End If
    End Sub
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

    Protected Sub Util_CheckAccess()

        Try
            If Not Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin) Then
                Throw New Exception(GetMessage("err not role commerce-admin"))
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try

    End Sub

#End Region
    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        If hdnCurrentPage.Value <> "" Then
            _currentPageNumber = Int32.Parse(hdnCurrentPage.Value)
        End If
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
        Display_All()
        isPostData.Value = "true"
    End Sub
    Protected Sub RegisterResource()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub
End Class
