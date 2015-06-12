Imports Ektron
Imports Ektron.Cms
Imports Ektron.Cms.Workarea
Imports Ektron.Cms.Commerce
Imports System.data
Imports System.Web.HttpRequest
Imports System.Web.UI.page

Partial Class Commerce_locale_country
    Inherits workareabase

    Protected m_refCountry As CountryApi = Nothing
    Protected m_sPageName As String = "country.aspx"
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1
    Protected sortCriteria As String = "name"
    Protected searchCriteria As String = ""
    Protected validationResult As New Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults
    Protected AppPath As String = ""

#Region "Page Functions"
    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        RegisterResources()
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        Utilities.ValidateUserLogin()
        AppPath = m_refContentApi.AppPath
        CommerceLibrary.CheckCommerceAdminAccess()

        If Page.Request.QueryString("sort") <> "" Then sortCriteria = Page.Request.QueryString("sort")
        If Page.Request.QueryString("search") <> "" Then searchCriteria = Page.Request.QueryString("search")
        m_refCountry = New CountryApi()
        hdnCurrentPage.Value = CurrentPage.Text
        Try
            Select Case Me.m_sPageAction
                Case "addedit"
                    If Page.IsPostBack Then
                        Process_AddEdit()
                    Else
                        Display_AddEdit()
                    End If
                Case "del"
                    Process_Delete()
                Case "view"
                    Display_View()
                Case Else
                    If Page.IsPostBack = False Then
                        Display_All()
                    End If
            End Select
            Util_SetLabels()
            Util_SetJS()
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region

#Region "Process"
    Protected Sub Process_AddEdit()
        Dim cCountry As CountryData = Nothing
        If Me.m_iID > 0 Then
            cCountry = m_refCountry.GetItem(Me.m_iID)
            cCountry.Id = txt_id.Text
            cCountry.Name = txt_name.Text
            cCountry.LongIsoCode = txt_long.text
            cCountry.ShortIsoCode = txt_short.Text
            cCountry.Enabled = chk_enabled.Checked
            m_refCountry.Update(cCountry)
            Response.Redirect(m_sPageName & "?action=view&id=" & m_iID.ToString(), False)
        Else
            Try
                cCountry = m_refCountry.GetItem(txt_id.Text)
            Catch ex As Exception
                cCountry = New CountryData(0, txt_name.Text, txt_short.Text, txt_long.Text, chk_enabled.Checked)
            End Try

            If cCountry IsNot Nothing AndAlso cCountry.Id > 0 Then
                Throw New Exception(GetMessage("lbl country dupe"))
            Else
                cCountry.Id = txt_id.Text
            End If
            cCountry.Name = txt_name.Text
            Try
                m_refCountry.Add(cCountry)

                Response.Redirect(m_sPageName, False)
            Catch ex As Exception
                If ex.Message.IndexOf("unique key") Then
                    Utilities.ShowError(GetMessage("lbl country dupe"))
                Else
                    Utilities.ShowError(ex.Message)
                End If
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
        If Me.m_iID > 0 Then m_refCountry.Delete(m_iID)
        Response.Redirect(m_sPageName, False)
    End Sub
#End Region

#Region "Display"
    Protected Sub Display_AddEdit()
        Dim cCountry As CountryData = New CountryData()
        If m_iID > 0 Then
            txt_id.Enabled = False
            cCountry = m_refCountry.GetItem(Me.m_iID)
        End If

        txt_name.Text = cCountry.Name
        txt_id.Text = cCountry.Id
        chk_enabled.Checked = cCountry.Enabled
        txt_long.Text = cCountry.LongIsoCode()
        txt_short.Text = cCountry.ShortIsoCode

        ' tr_id.Visible = (m_iID > 0)
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
        Dim CountryList As New System.Collections.Generic.List(Of CountryData)()
        Dim criteria As New Ektron.Cms.Common.Criteria(Of CountryProperty)(CountryProperty.Name, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending)
        Dim i As Integer = 0

        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        criteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        If sortCriteria.IndexOf("-") > -1 Then criteria.OrderByDirection = Ektron.Cms.Common.EkEnumeration.OrderByDirection.Descending
        Select Case sortCriteria
            Case "id"
                criteria.OrderByField = CountryProperty.Id
            Case "enabled"
                criteria.OrderByField = CountryProperty.IsEnabled
            Case "longiso"
                criteria.OrderByField = CountryProperty.LongIsoCode
            Case "shortiso"
                criteria.OrderByField = CountryProperty.ShortIsoCode
            Case Else
                criteria.OrderByField = CountryProperty.Name
        End Select

        If searchCriteria <> "" Then criteria.AddFilter(CountryProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, searchCriteria)

        CountryList = m_refCountry.GetList(criteria)

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

        dg_viewall.DataSource = CountryList
        dg_viewall.DataBind()
    End Sub
    Protected Sub Display_View()
        Dim cCountry As CountryData = Nothing

        cCountry = m_refCountry.GetItem(Me.m_iID)

        txt_name.Text = cCountry.Name
        txt_id.Text = cCountry.Id
        chk_enabled.Checked = cCountry.Enabled
        txt_long.Text = cCountry.LongIsoCode()
        txt_short.Text = cCountry.ShortIsoCode

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

#Region "Private Helpers"

    Protected Sub Util_SetLabels()
        Select Case Me.m_sPageAction
            Case "addedit"
                Me.AddButtonwithMessages(AppPath & "images/UI/Icons/save.png", m_sPageName & "?action=addedit&id=" & m_iID.ToString(), "btn save", "btn save", " onclick=""return SubmitForm();"" ")
                AddBackButton(m_sPageName & IIf(m_iID > 0, "?action=view&id=" & Me.m_iID.ToString(), ""))
                If Me.m_iID > 0 Then
                    SetTitleBarToMessage("lbl edit country")
                    AddHelpButton("Editcountry")
                Else
                    SetTitleBarToMessage("lbl add country")
                    AddHelpButton("Addcountry")
                End If
            Case "view"
                Me.AddButtonwithMessages(AppPath & "images/UI/Icons/contentEdit.png", m_sPageName & "?action=addedit&id=" & m_iID.ToString(), "generic edit title", "generic edit title", "")
                If m_refCountry.CanDelete(Me.m_iID, validationResult) Then
                    Me.AddButtonwithMessages(AppPath & "images/UI/Icons/delete.png", m_sPageName & "?action=del&id=" & m_iID.ToString(), "generic delete title", "generic delete title", " onclick=""return confirm('" & GetMessage("js confirm delete country") & "');"" ")
                End If
                AddBackButton(m_sPageName)
                SetTitleBarToMessage("lbl view country")
                AddHelpButton("Viewcountry")
            Case Else
                Dim newMenu As New workareamenu("file", GetMessage("lbl new"), AppPath & "images/UI/Icons/star.png")
                newMenu.AddLinkItem(AppImgPath & "/menu/document.gif", GetMessage("lbl country"), m_sPageName & "?action=addedit")
                Me.AddMenu(newMenu)

                Me.AddSearchBox(Server.HtmlEncode(searchCriteria), New ListItemCollection(), "searchCountry")
                SetTitleBarToMessage("lbl countries")
                AddHelpButton("country")
        End Select

        ltr_name.Text = GetMessage("generic name")
        ltr_id.Text = GetMessage("lbl numericisocode")
        ltr_enabled.Text = GetMessage("enabled")
        ltr_long.Text = GetMessage("lbl longisocode")
        ltr_short.Text = GetMessage("lbl shortisocode")
    End Sub

    Protected Sub Util_SetJS()
        Dim sbJS As New StringBuilder()

        sbJS.Append("<script type=""text/javascript"">").Append(Environment.NewLine)

        sbJS.Append("   var aSubmitErr = new Array();").Append(Environment.NewLine)
        sbJS.Append(JSLibrary.AddError("aSubmitErr"))
        sbJS.Append(JSLibrary.ShowError("aSubmitErr"))
        sbJS.Append(JSLibrary.ResetError("aSubmitErr"))
        sbJS.Append(JSLibrary.HasIllegalCharacters(workareajavascript.ErrorType.ErrorCollection))

        sbJS.Append(" function validate_Title() { ").Append(Environment.NewLine)
        sbJS.Append("   var sTitle = Trim(document.getElementById('").Append(txt_name.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   if (sTitle == '') { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err country title req")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar('").Append(txt_name.UniqueID).Append("',""").Append(GetMessage("lbl country disallowed chars")).Append("""); ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append(" function SubmitForm() { ").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine)
        sbJS.Append("   var nISO = Trim(document.getElementById('").Append(txt_id.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var sLISO = Trim(document.getElementById('").Append(txt_long.UniqueID).Append("').value); ").Append(Environment.NewLine)
        sbJS.Append("   var sSISO = Trim(document.getElementById('").Append(txt_short.UniqueID).Append("').value); ").Append(Environment.NewLine)

        sbJS.Append("   if (isNaN(nISO) || nISO == '' || nISO < 1 )").Append(Environment.NewLine)
        sbJS.Append("   {").Append(Environment.NewLine)
        sbJS.Append("       ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err country iso not numeric")).Append("');").Append(Environment.NewLine)
        sbJS.Append("   }").Append(Environment.NewLine)
        sbJS.Append("   else if(sLISO.length == 0 || sSISO.length == 0)").Append(Environment.NewLine)
        sbJS.Append("   {").Append(Environment.NewLine)
        sbJS.Append("       ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err long short iso empty")).Append("');").Append(Environment.NewLine)
        sbJS.Append("   }").Append(Environment.NewLine)
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("('document.forms[0].submit();');").Append(Environment.NewLine)
        sbJS.Append("   return false; ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append(" function searchCountry() { ").Append(Environment.NewLine)
        sbJS.Append("   var sSearchTerm = $ektron('#txtSearch').getInputLabelValue(); ").Append(Environment.NewLine)
        sbJS.Append("   if (sSearchTerm != '') { window.location.href = '").Append(m_sPageName).Append("?search=' + sSearchTerm;} else { alert('").Append(GetMessage("js err please enter text")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append("</script>").Append(Environment.NewLine)

        ltr_js.Text &= sbJS.ToString()
    End Sub

    Protected Sub Util_SetEnabled(ByVal toggle As Boolean)
        Me.txt_name.Enabled = toggle
        txt_long.Enabled = toggle
        txt_short.Enabled = toggle
        chk_enabled.Enabled = toggle
        txt_id.Enabled = toggle
    End Sub

    Protected Function Util_SortUrl(ByVal messageText As String, ByVal sortingValue As String) As String

        Dim urlString As String = ""
        If sortingValue = sortCriteria And sortCriteria.IndexOf("-") = -1 Then sortingValue = sortingValue & "-"
        If sortingValue = sortCriteria And sortCriteria.IndexOf("-") > -1 Then sortingValue = sortingValue.Replace("-", "")
        urlString = "<a href=""country.aspx?sort=" & sortingValue & """>" & GetMessage(messageText) & "</a>"
        Return urlString

    End Function

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
    Protected Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub
#End Region

End Class
