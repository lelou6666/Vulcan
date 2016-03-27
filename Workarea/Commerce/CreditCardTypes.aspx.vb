Imports System.Collections.Generic
Imports Ektron.Cms.Workarea
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common

Partial Class Commerce_cctypes
    Inherits workareabase

#Region "Member Variables"

    Protected _PageName As String = "creditcardtypes.aspx"
    Protected _CurrentPageNumber As Integer = 1
    Protected _TotalPagesNumber As Integer = 1

#End Region

#Region "Events"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        'register page components
        Me.RegisterCSS()
        Me.RegisterJS()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        Try
            Utilities.ValidateUserLogin()
            Util_CheckAccess()
            Select Case MyBase.m_sPageAction
                Case "del"
                    Process_Delete()
                Case "addedit"
                    If Page.IsPostBack() Then
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
            Util_SetLabels()
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

#End Region

#Region "Display"
    Protected Sub Display_View()
        Dim ccApi As New CreditCardApi()
        Dim ccType As CreditCardTypeData = Nothing
        ccType = ccApi.GetItem(Me.m_iID)

        txt_name.Text = ccType.Name
        lbl_id.Text = ccType.Id
        chk_accepted.Checked = ccType.IsAccepted
        txt_regex.Text = ccType.Regex

        cc_image.Text = Util_ShowImagePath(ccType.Image)
        cc_image_thumb.ImageUrl = Util_ShowImagePath(ccType.Image)
        If cc_image_thumb.ImageUrl <> "" Then
            cc_image_thumb.ImageUrl = IIf(Util_ShowImagePath(ccType.Image).IndexOf("/") = 0, Util_ShowImagePath(ccType.Image), m_refContentApi.SitePath & Util_ShowImagePath(ccType.Image))
        Else
            cc_image_thumb.ImageUrl = AppImgPath & "spacer.gif"
        End If
        pnl_edit.Visible = False
        cc_image.Enabled = False

        txt_name.Enabled = False
        chk_accepted.Enabled = False
        txt_regex.Enabled = False
    End Sub

    Protected Sub Display_AddEdit()
        Dim ccApi As New CreditCardApi()
        Dim ccType As CreditCardTypeData = Nothing
        ccType = IIf(m_iID > 0, ccApi.GetItem(Me.m_iID), New CreditCardTypeData())

        txt_name.Text = ccType.Name
        lbl_id.Text = ccType.Id
        chk_accepted.Checked = ccType.IsAccepted
        txt_regex.Text = ccType.Regex

        cc_image.Text = Util_ShowImagePath(ccType.Image)
        cc_image_thumb.ImageUrl = Util_ShowImagePath(ccType.Image)
        If cc_image_thumb.ImageUrl <> "" Then
            cc_image_thumb.ImageUrl = IIf(Util_ShowImagePath(ccType.Image).IndexOf("/") = 0, Util_ShowImagePath(ccType.Image), m_refContentApi.SitePath & Util_ShowImagePath(ccType.Image))
        Else
            cc_image_thumb.ImageUrl = AppImgPath & "spacer.gif"
        End If

        tr_id.Visible = (m_iID > 0)
    End Sub

    Protected Sub Display_View_All()
        Dim ccApi As New CreditCardApi()
        Dim criteria As New Criteria(Of CreditCardTypeProperty)
        Dim ccList As List(Of CreditCardTypeData)

        criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize
        criteria.PagingInfo.CurrentPage = _CurrentPageNumber.ToString()

        ccList = ccApi.GetList(criteria)

        _TotalPagesNumber = criteria.PagingInfo.TotalPages

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

        dg_cctypes.DataSource = ccList
        dg_cctypes.DataBind()
    End Sub
#End Region

#Region "Process"

    Protected Sub Process_Delete()
        Dim ccApi As New CreditCardApi()
        ccApi.Delete(Me.m_iID)
        Response.Redirect(Me._PageName, False)
    End Sub

    Protected Sub Process_AddEdit()
        Dim ccApi As New CreditCardApi()
        Dim ccType As CreditCardTypeData = Nothing
        ccType = IIf(m_iID > 0, ccApi.GetItem(Me.m_iID), New CreditCardTypeData())
        ccType.Name = Me.txt_name.Text
        ccType.IsAccepted = Me.chk_accepted.Checked
        ccType.Image = Request.Form(cc_image.UniqueID)
        If Me.txt_regex.Text <> "" Then
            ccType.Regex = Me.txt_regex.Text
        Else
            ' The following regular expression checks if the value entered for credit card is 16 digit long, all numbers and not empty.
            ccType.Regex = "\d{4}-?\d{4}-?\d{4}-?\d{4}"
        End If

        If (ccType.Id > 0) Then
            ccApi.Update(ccType)
        Else
            ccApi.Add(ccType)
        End If

        Response.Redirect(Me._PageName & IIf(m_iID > 0, "?action=view&id=" & Me.m_iID, ""), False)
    End Sub

#End Region

#Region "Util"

    Protected Sub Util_SetLabels()
        ltr_name.Text = GetMessage("lbl cc type name")
        ltr_id.Text = GetMessage("lbl cc type id")
        ltr_accepted.Text = GetMessage("lbl cc type accepted")
        ltr_image.Text = GetMessage("lbl cc type image")
        ltr_regex.Text = GetMessage("lbl cc type regex")
        Select Case MyBase.m_sPageAction
            Case "view"
                Me.pnl_view.Visible = True
                Me.pnl_viewall.Visible = False
                Me.AddButtonwithMessages(Me.AppImgPath & "../UI/Icons/contentEdit.png", Me._PageName & "?action=addedit&id=" & Me.m_iID.ToString(), "generic edit title", "generic edit title", "")
                Me.AddButtonwithMessages(Me.AppImgPath & "../UI/Icons/delete.png", Me._PageName & "?action=del&id=" & Me.m_iID.ToString(), "alt del cc type button text", "btn delete", " onclick=""return CheckDelete();"" ")
                Me.AddBackButton(Me._PageName)
                Me.SetTitleBarToMessage("lbl view cc type")
                Me.AddHelpButton("ViewCreditCardType")
            Case "addedit"
                ' Me.ltr_cmd_img_prv.Text = "<img src=""" & AppImgPath & "btn_preview-nm.gif"" border=""0"" alt=""" & GetMessage("lbl cc type img review") & """ title=""" & GetMessage("lbl cc type img preview") & """ onclick="" PreviewImage(); return false;"" />"
                Me.pnl_view.Visible = True
                Me.pnl_viewall.Visible = False
                Me.AddButtonwithMessages(Me.AppImgPath & "../UI/Icons/save.png", "#", "lbl alt edit cc type", "btn save", " onclick=""SubmitForm(); return false;"" ")
                Me.AddBackButton(Me._PageName & IIf(Me.m_iID > 0, "?action=view&id=" & Me.m_iID, ""))
                Me.SetTitleBarToString(IIf(Me.m_iID > 0, Me.GetMessage("lbl edit cc type"), Me.GetMessage("lbl add cc type")))
                Me.AddHelpButton(IIf(Me.m_iID > 0, ("EditCreditCardType"), ("AddCreditCardType")))
            Case Else ' "viewall"
                Dim newMenu As New workareamenu("file", Me.GetMessage("lbl new"), Me.AppImgPath & "../UI/Icons/star.png")
                newMenu.AddLinkItem(Me.AppImgPath & "/menu/card.gif", Me.GetMessage("lbl cc type"), Me._PageName & "?action=addedit")
                Me.AddMenu(newMenu)
                Me.SetTitleBarToMessage("lbl cc types")
                Me.AddHelpButton("cctype")
        End Select

        Util_SetJs()
    End Sub

    Private Sub Util_SetJs()
        Dim sbJS As New StringBuilder
        sbJS.Append("<script language=""javascript"" type=""text/javascript"" >" & Environment.NewLine)

        sbJS.Append("function CheckDelete()" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("    return confirm('").Append(GetMessage("js cc type confirm del")).Append("');" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function SubmitForm()" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("    var objtitle = document.getElementById(""").Append(txt_name.UniqueID).Append(""");" & Environment.NewLine)
        sbJS.Append("    if (Trim(objtitle.value).length > 0)" & Environment.NewLine)
        sbJS.Append("    {" & Environment.NewLine)
        sbJS.Append("	    if (!CheckForillegalChar(objtitle.value)) {" & Environment.NewLine)
        sbJS.Append("           objtitle.focus();" & Environment.NewLine)
        sbJS.Append("       } else {" & Environment.NewLine)
        sbJS.Append("           document.forms[0].submit();" & Environment.NewLine)
        sbJS.Append("	    }" & Environment.NewLine)
        sbJS.Append("    }" & Environment.NewLine)
        sbJS.Append("    else" & Environment.NewLine)
        sbJS.Append("    {" & Environment.NewLine)
        sbJS.Append("        alert(""" & MyBase.GetMessage("js null cc type msg") & """);" & Environment.NewLine)
        sbJS.Append("        objtitle.focus();" & Environment.NewLine)
        sbJS.Append("    }" & Environment.NewLine)
        sbJS.Append("    return false;" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function CheckForillegalChar(txtName) {" & Environment.NewLine)
        sbJS.Append("   var val = txtName;" & Environment.NewLine)
        sbJS.Append("   if ((val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbJS.Append("   {" & Environment.NewLine)
        sbJS.Append("       alert(""").Append(String.Format(GetMessage("js alert cc type name cant include"), "('\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')")).Append(""");" & Environment.NewLine)
        sbJS.Append("       return false;" & Environment.NewLine)
        sbJS.Append("   }" & Environment.NewLine)
        sbJS.Append("   return true;" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function PreviewImage()" & Environment.NewLine)
        sbJS.Append("{" & Environment.NewLine)
        sbJS.Append("   var oPreview = document.getElementById(""").Append(cc_image.UniqueID).Append(""");" & Environment.NewLine)
        sbJS.Append("   oPreview.innerHTML = '").Append(GetMessage("lbl cc type img previewing")).Append("';" & Environment.NewLine)
        sbJS.Append("   var oImg = document.getElementById(""").Append(cc_image.UniqueID).Append(""");" & Environment.NewLine)
        sbJS.Append("   var strImg = oImg.value;" & Environment.NewLine)
        sbJS.Append("   if(strImg.length > 0) {" & Environment.NewLine)
        sbJS.Append("       strImg = strImg.replace(/\[apppath\]/,""").Append(Me.m_refContentApi.ApplicationPath).Append(""");" & Environment.NewLine)
        sbJS.Append("       strImg = strImg.replace(/\[appimgpath\]/,""").Append(Me.m_refContentApi.AppImgPath).Append(""");" & Environment.NewLine)
        sbJS.Append("       strImg = strImg.replace(/\[sitepath\]/,""").Append(Me.m_refContentApi.SitePath).Append(""");" & Environment.NewLine)
        sbJS.Append("       oPreview.innerHTML = '<img src=""' + strImg + '"" alt="""" title="""" border=""0"">';" & Environment.NewLine)
        sbJS.Append("   } else { " & Environment.NewLine)
        sbJS.Append("       oPreview.innerHTML = '';" & Environment.NewLine)
        sbJS.Append("   } " & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("</script>" & Environment.NewLine)
        ltr_js.Text &= Environment.NewLine & sbJS.ToString()
    End Sub

    Protected Function Util_ShowImagePath(ByVal image As String) As String
        Dim sRet As String = ""
        If image <> "" Then
            image = Replace(image, "[apppath]", Me.m_refContentApi.ApplicationPath)
            image = Replace(image, "[appimgpath]", Me.m_refContentApi.AppImgPath)
            image = Replace(image, "[sitepath]", Me.m_refContentApi.SitePath)
            sRet = image ' "<img src=""" & image & """ alt="""" title="""" border=""0"">"
        End If
        Return sRet
    End Function

    Protected Function Util_ShowImage(ByVal image As String) As String
        Dim sRet As String = ""
        If image <> "" Then
            image = Replace(image, "[apppath]", Me.m_refContentApi.ApplicationPath)
            image = Replace(image, "[appimgpath]", Me.m_refContentApi.AppImgPath)
            image = Replace(image, "[sitepath]", Me.m_refContentApi.SitePath)
            If image.IndexOf("/") = 0 Then
            Else
                image = m_refContentApi.SitePath & image
            End If
            sRet = "<img src=""" & image & """ alt="""" title="""" border=""0"">"
        End If
        Return sRet
    End Function

    Protected Sub Util_CheckAccess()
        If Not m_refContentApi.IsARoleMember(Common.EkEnumeration.CmsRoleIds.CommerceAdmin) Then
            Throw New Exception(GetMessage("err not role commerce-admin"))
        End If
    End Sub

    Protected Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
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
        Display_View_All()
        isPostData.Value = "true"
    End Sub

#End Region

#Region "JS/CSS"

    Private Sub RegisterJS()

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "/wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS")

    End Sub

    Private Sub RegisterCSS()

        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "/wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub

#End Region

End Class
