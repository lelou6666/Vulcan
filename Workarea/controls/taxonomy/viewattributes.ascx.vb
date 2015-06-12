Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports System.DateTime
Imports System.Collections.Generic
Imports System.IO

Partial Class viewattributes
    Inherits System.Web.UI.UserControl

    Protected m_refCommon As New CommonApi
    Protected m_refstyle As New StyleHelper
    Protected AppImgPath As String = ""
    Protected m_refMsg As EkMessageHelper
    Protected m_strPageAction As String = ""
    Protected m_refContent As Content.EkContent
    Protected TaxonomyId As Long = 0
    Protected TaxonomyLanguage As Integer = -1
    Protected language_data As LanguageData
    Protected taxonomy_request As TaxonomyRequest
    Protected taxonomy_data As TaxonomyData
    Protected TaxonomyParentId As Long = 0
    Protected m_strViewItem As String = "item"
    Protected AddDeleteIcon As Boolean = False
    Protected m_strTaxonomyName As String = ""
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected m_strDelConfirm As String = ""
    Protected m_strDelItemsConfirm As String = ""
    Protected m_strSelDelWarning As String = ""
    Protected m_strCurrentBreadcrumb As String = ""
    Protected objLocalizationApi As New LocalizationAPI()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = m_refCommon.EkMsgRef
        AppImgPath = m_refCommon.AppImgPath
        m_strPageAction = Request.QueryString("action")
        Utilities.SetLanguage(m_refCommon)
        TaxonomyLanguage = m_refCommon.ContentLanguage
        TaxonomyId = Convert.ToInt64(Request.QueryString("taxonomyid"))
        If (Request.QueryString("view") IsNot Nothing) Then
            m_strViewItem = Request.QueryString("view")
        End If
        taxonomy_request = New TaxonomyRequest
        taxonomy_request.TaxonomyId = TaxonomyId
        taxonomy_request.TaxonomyLanguage = TaxonomyLanguage
        m_refContent = m_refCommon.EkContentRef
        If (Page.IsPostBack) Then
            If (Request.Form("submittedaction") = "delete") Then
                m_refContent.DeleteTaxonomy(taxonomy_request)
                Response.Write("<script type=""text/javascript"">parent.CloseChildPage();</script>")
            ElseIf (Request.Form("submittedaction") = "deleteitem") Then
                If (m_strViewItem <> "folder") Then
                    taxonomy_request.TaxonomyIdList = Request.Form("selected_items")
                    If (m_strViewItem.ToLower = "cgroup") Then
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Group
                    ElseIf (m_strViewItem.ToLower = "user") Then
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.User
                    Else
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Content
                    End If
                    m_refContent.RemoveTaxonomyItem(taxonomy_request)
                Else
                    Dim tax_folder As New TaxonomySyncRequest
                    tax_folder.TaxonomyId = TaxonomyId
                    tax_folder.TaxonomyLanguage = TaxonomyLanguage
                    tax_folder.SyncIdList = Request.Form("selected_items")
                    m_refContent.RemoveTaxonomyFolder(tax_folder)
                End If
                If (Request.Params("ccp") Is Nothing) Then
                    Response.Redirect("taxonomy.aspx?" & Request.ServerVariables("query_string") & "&ccp=true", True)
                Else
                    Response.Redirect("taxonomy.aspx?" & Request.ServerVariables("query_string"), True)
                End If
            End If
        ElseIf (IsPostBack = False) Then
            DisplayPage()
        End If
    End Sub

    Private Sub DisplayPage()
        taxonomy_request.IncludeItems = True
        taxonomy_request.PageSize = m_refCommon.RequestInformationRef.PagingSize
        taxonomy_request.CurrentPage = m_intCurrentPage
        taxonomy_data = m_refContent.ReadTaxonomy(taxonomy_request)
        If (taxonomy_data IsNot Nothing) Then
            TaxonomyParentId = taxonomy_data.TaxonomyParentId
            lbltaxonomyid.Text = taxonomy_data.TaxonomyId
            taxonomytitle.Text = taxonomy_data.TaxonomyName
            m_strTaxonomyName = taxonomy_data.TaxonomyName
            If taxonomy_data.TaxonomyDescription = "" Then
                taxonomydescription.Text = "[None]"
            Else
                taxonomydescription.Text = taxonomy_data.TaxonomyDescription
            End If
            If taxonomy_data.TaxonomyImage = "" Then
                taxonomy_image.Text = "[None]"
            Else
                taxonomy_image.Text = taxonomy_data.TaxonomyImage
            End If
            taxonomy_image_thumb.ImageUrl = taxonomy_data.TaxonomyImage
            If taxonomy_data.CategoryUrl = "" Then
                catLink.Text = "[None]"
            Else
                catLink.Text = taxonomy_data.CategoryUrl
            End If

            If taxonomy_data.Visible = True Then
                ltrStatus.Text = "Enabled"
            Else
                ltrStatus.Text = "Disabled"
            End If
            If taxonomy_data.TaxonomyImage.Trim() <> "" Then
                taxonomy_image_thumb.ImageUrl = IIf(taxonomy_data.TaxonomyImage.IndexOf("/") = 0, taxonomy_data.TaxonomyImage, m_refCommon.SitePath & taxonomy_data.TaxonomyImage)
            Else
                taxonomy_image_thumb.ImageUrl = m_refCommon.AppImgPath & "spacer.gif"
            End If
            m_strCurrentBreadcrumb = taxonomy_data.TaxonomyPath.Remove(0, 1).Replace("\", " > ")
            If (m_strCurrentBreadcrumb = "") Then
                m_strCurrentBreadcrumb = "Root"
            End If
            If (taxonomy_data.TemplateName = "") Then
                lblTemplate.Text = "[None]"
            Else
                lblTemplate.Text = taxonomy_data.TemplateName
            End If
            If (taxonomy_data.TemplateInherited) Then
                lblTemplateInherit.Text = "Yes"
            Else
                lblTemplateInherit.Text = "No"
            End If
            m_intTotalPages = taxonomy_request.TotalPages
        End If
        TaxonomyToolBar()
        If (TaxonomyParentId = 0) Then
            tr_config.Visible = True
            Dim config_list As List(Of Int32) = m_refContent.GetAllConfigIdListByTaxonomy(TaxonomyId, TaxonomyLanguage)
            configlist.Text = ""
            For i As Integer = 0 To config_list.Count - 1
                If (configlist.Text = "") Then
                    configlist.Text = ConfigName(config_list.Item(i))
                Else
                    configlist.Text = configlist.Text & ";" & ConfigName(config_list.Item(i))
                End If
            Next
            If (configlist.Text = "") Then
                configlist.Text = "None"
            End If
        Else
            tr_config.Visible = False
        End If
    End Sub
    Private Function ConfigName(ByVal id As Integer) As String
        Select Case id
            Case 0
                Return "Content"
            Case 1
                Return "User"
            Case 2
                Return "Group"
            Case Else
                Return "Content"
        End Select
    End Function
    Private Function GetRecursiveTitle(ByVal value As Boolean) As String
        Dim result As String = ""
        If (value) Then
            result = "<span class=""important""> (Recursive)</span>"
        End If
        Return result
    End Function
    Private Sub TaxonomyToolBar()        
        Dim strDeleteMsg As String = ""
        If (TaxonomyParentId > 0) Then
            strDeleteMsg = m_refMsg.GetMessage("alt delete button text (category)")
            m_strDelConfirm = m_refMsg.GetMessage("delete category confirm")
            m_strDelItemsConfirm = m_refMsg.GetMessage("delete category items confirm")
            m_strSelDelWarning = m_refMsg.GetMessage("select category item missing warning")
        Else
            strDeleteMsg = m_refMsg.GetMessage("alt delete button text (taxonomy)")
            m_strDelConfirm = m_refMsg.GetMessage("delete taxonomy confirm")
            m_strDelItemsConfirm = m_refMsg.GetMessage("delete taxonomy items confirm")
            m_strSelDelWarning = m_refMsg.GetMessage("select taxonomy item missing warning")
        End If
        divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("view taxonomy page title") & " """ & m_strTaxonomyName & """" & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) & "' />")
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentEdit.png", "taxonomy.aspx?action=edit&taxonomyid=" & TaxonomyId & "&parentid=" & TaxonomyParentId & "&LangType=" & TaxonomyLanguage, m_refMsg.GetMessage("alt edit button text (taxonomy)"), m_refMsg.GetMessage("btn edit"), ""))
        ' result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "btn_delete_folder-nm.gif", "#", strDeleteMsg, m_refMsg.GetMessage("btn delete"), "Onclick=""javascript:return DeleteItem();"""))
        If (AddDeleteIcon) Then
            result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt remove button text (taxonomyitems)"), m_refMsg.GetMessage("btn remove"), "Onclick=""javascript:return DeleteItem('items');"""))
        End If

        result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "taxonomy.aspx?action=viewcontent&taxonomyid=" & TaxonomyId & "&parentid=" & TaxonomyParentId & "&LangType=" & TaxonomyLanguage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td nowrap=""true"">")
        Dim addDD As String
        addDD = GetLanguageForTaxonomy(TaxonomyId, "", False, False, "javascript:TranslateTaxonomy(" & TaxonomyId & ", " & TaxonomyParentId & ", this.value);")
        If addDD <> "" Then
            addDD = "&nbsp;" & m_refMsg.GetMessage("add title") & ":&nbsp;" & addDD
        End If
        If (CStr(m_refCommon.EnableMultilingual = "1")) Then
            result.Append("View In:&nbsp;" & GetLanguageForTaxonomy(TaxonomyId, "", True, False, "javascript:LoadLanguage(this.value);") & "&nbsp;" & addDD & "<br>")
        End If
        result.Append("</td>")

        result.Append("<td>" & m_refstyle.GetHelpButton("ViewTaxonomyOrCategory") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub

    Private Function FindSelected(ByVal chk As String) As String
        Dim val As String = ""
        If (m_strViewItem = chk) Then
            val = " selected "
        End If
        Return val
    End Function

    Private Function GetLanguageForTaxonomy(ByVal TaxonomyId As Long, ByVal BGColor As String, ByVal ShowTranslated As Boolean, ByVal ShowAllOpt As Boolean, ByVal onChangeEv As String) As String
        Dim result As String = ""
        Dim frmName As String = ""
        Dim result_language As IList(Of LanguageData) = Nothing
        Dim taxonomy_language_request As New TaxonomyLanguageRequest
        taxonomy_language_request.TaxonomyId = TaxonomyId

        If (ShowTranslated) Then
            taxonomy_language_request.IsTranslated = True
            result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request)
            frmName = "frm_translated"
        Else
            taxonomy_language_request.IsTranslated = False
            result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request)
            frmName = "frm_nontranslated"
        End If

        result = "<select id=""" & frmName & """ name=""" & frmName & """ OnChange=""" & onChangeEv & """>" & vbCrLf

        If (CBool(ShowAllOpt)) Then
            If TaxonomyLanguage = -1 Then
                result = result & "<option value=""-1"" selected>All</option>"
            Else
                result = result & "<option value=""-1"">All</option>"
            End If
        Else
            If (ShowTranslated = False) Then
                result = result & "<option value=""0"">-select language-</option>"
            End If
        End If
        If ((result_language IsNot Nothing) AndAlso (result_language.Count > 0) AndAlso (m_refCommon.EnableMultilingual = 1)) Then
            For Each language As LanguageData In result_language
                If TaxonomyLanguage = language.Id Then
                    result = result & "<option value=" & language.Id & " selected>" & language.Name & "</option>"
                Else
                    result = result & "<option value=" & language.Id & ">" & language.Name & "</option>"
                End If
            Next
        Else
            result = ""
        End If
        If (result.Length > 0) Then
            result = result & "</select>"
        End If
        Return (result)
    End Function
End Class
