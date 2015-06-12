Imports Ektron
Imports Ektron.Cms
Imports Ektron.Cms.Workarea
Imports Ektron.Cms.Commerce

Partial Class Commerce_tax_taxclass
    Inherits workareabase

    Protected m_refTaxClass As TaxClass = Nothing
    Protected m_sPageName As String = "taxclass.aspx"
    Protected _currentPageNumber As Integer = 1
    Protected TotalPagesNumber As Integer = 1
    Protected AppPath As String = ""
#Region "Page Functions"
    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If

        Utilities.ValidateUserLogin()
        Util_CheckAccess()
        AppPath = m_refContentApi.ApplicationPath

        Try
            m_refTaxClass = New TaxClass(Me.m_refContentApi.RequestInformationRef)
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
            Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region

#Region "Process"
    Protected Sub Process_AddEdit()
        Dim txClass As TaxClassData = Nothing
        If Me.m_iID > 0 Then
            txClass = m_refTaxClass.GetItem(Me.m_iID)
            txClass.Name = txt_name.Text
            m_refTaxClass.Update(txClass)
            Response.Redirect(m_sPageName & "?action=view&id=" & m_iID.ToString(), False)
        Else
            txClass = New TaxClassData(txt_name.Text)
            m_refTaxClass.Add(txClass)
            Response.Redirect(m_sPageName, False)
        End If
    End Sub
    Protected Sub Process_Delete()
        If Me.m_iID > 0 Then m_refTaxClass.Delete(m_iID)
        Response.Redirect(m_sPageName, False)
    End Sub
#End Region

#Region "Display"
    Protected Sub Display_AddEdit()
        Dim txClass As TaxClassData = New TaxClassData()
        If m_iID > 0 Then txClass = m_refTaxClass.GetItem(Me.m_iID)

        txt_name.Text = txClass.Name
        lbl_id.Text = txClass.Id
        tr_id.Visible = (m_iID > 0)
        pnl_view.Visible = True
        pnl_viewall.Visible = False
    End Sub
    Protected Sub Display_All()
        Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()
        Dim criteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Name, Common.EkEnumeration.OrderByDirection.Ascending)
        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        criteria.PagingInfo.CurrentPage = _currentPageNumber.ToString()

        TaxClassList = m_refTaxClass.GetList(criteria)

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

        dg_viewall.DataSource = TaxClassList
        dg_viewall.DataBind()
    End Sub
    Protected Sub Display_View()
        Dim txClass As TaxClassData = Nothing

        txClass = m_refTaxClass.GetItem(Me.m_iID)

        txt_name.Text = txClass.Name
        lbl_id.Text = txClass.Id

        Me.txt_name.Enabled = False
        pnl_view.Visible = True
        pnl_viewall.Visible = False
    End Sub
#End Region

#Region "Private Helpers"
    Protected Sub Util_SetLabels()
        Select Case Me.m_sPageAction
            Case "addedit"
                Me.AddButtonwithMessages(AppPath & "images/UI/Icons/save.png", m_sPageName & "?action=addedit&id=" & m_iID.ToString(), "btn save", "btn save", " onclick="" return SubmitForm();"" ")
                AddBackButton(m_sPageName & IIf(m_iID > 0, "?action=view&id=" & Me.m_iID.ToString(), ""))
                If Me.m_iID > 0 Then
                    SetTitleBarToMessage("lbl edit tax class")
                    AddHelpButton("Edittaxclass")
                Else
                    SetTitleBarToMessage("lbl add tax class")
                    AddHelpButton("Addtaxclass")
                End If
            Case "view"
                Me.AddButtonwithMessages(AppPath & "images/UI/Icons/contentEdit.png", m_sPageName & "?action=addedit&id=" & m_iID.ToString(), "generic edit title", "generic edit title", "")
                If Not m_refTaxClass.IsUsed(m_iID) Then Me.AddButtonwithMessages(AppPath & "images/UI/Icons/delete.png", m_sPageName & "?action=del&id=" & m_iID.ToString(), "generic delete title", "generic delete title", " onclick=""return confirm('" & GetMessage("js confirm delete tax class") & "');"" ")
                AddBackButton(m_sPageName)
                SetTitleBarToMessage("lbl view tax class")
                AddHelpButton("Viewtaxclass")
            Case Else
                Dim newMenu As New workareamenu("file", GetMessage("lbl new"), AppPath & "images/UI/Icons/star.png")
                newMenu.AddLinkItem(AppImgPath & "/menu/document.gif", GetMessage("lbl tax class"), m_sPageName & "?action=addedit")
                Me.AddMenu(newMenu)
                SetTitleBarToMessage("lbl tax classes")
                AddHelpButton("taxclass")
        End Select

        ltr_name.Text = GetMessage("generic name")
        ltr_id.Text = GetMessage("generic id")
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
        sbJS.Append("   if (sTitle == '') { ").Append(JSLibrary.AddErrorFunctionName).Append("('").Append(GetMessage("js err tax class title req")).Append("'); } ").Append(Environment.NewLine)
        sbJS.Append("   HasIllegalChar('").Append(txt_name.UniqueID).Append("',""").Append(GetMessage("lbl tax class disallowed chars")).Append("""); ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)

        sbJS.Append(" function SubmitForm() { ").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ResetErrorFunctionName).Append("();").Append(Environment.NewLine)
        sbJS.Append("   validate_Title(); ").Append(Environment.NewLine)
        sbJS.Append("   ").Append(JSLibrary.ShowErrorFunctionName).Append("('document.forms[0].submit();');").Append(Environment.NewLine)
        sbJS.Append("   return false; ").Append(Environment.NewLine)
        sbJS.Append(" } ").Append(Environment.NewLine)
        sbJS.Append("</script>").Append(Environment.NewLine)

        ltr_js.Text &= sbJS.ToString()
    End Sub
    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
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

End Class
