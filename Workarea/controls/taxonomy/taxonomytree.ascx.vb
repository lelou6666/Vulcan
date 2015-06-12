Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports System.DateTime
Imports System.Collections.Generic
Imports System.IO

Partial Class taxonomytree
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
    Protected AncestorTaxonomyId As Long = 0
    Protected m_selectedTaxonomyList As String = ""
    Protected m_strTaxonomyName As String = ""
    Protected objLocalizationApi As New LocalizationAPI()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = m_refCommon.EkMsgRef
        AppImgPath = m_refCommon.AppImgPath
        m_strPageAction = Request.QueryString("action")
        Utilities.SetLanguage(m_refCommon)
        TaxonomyLanguage = m_refCommon.ContentLanguage
        TaxonomyId = Convert.ToInt64(Request.QueryString("taxonomyid"))
        taxonomy_request = New TaxonomyRequest
        taxonomy_request.TaxonomyId = TaxonomyId
        taxonomy_request.TaxonomyLanguage = TaxonomyLanguage
        m_refContent = m_refCommon.EkContentRef
        Util_RegisterResources()
        Util_SetServerJSVariables()

        litEnable.Text = m_refMsg.GetMessage("js:Confirm enable taxonomy all languages")
        litDisable.Text = m_refMsg.GetMessage("js:Confirm disable taxonomy all languages")
        If (Page.IsPostBack) Then
            If Request.Form("submittedaction") = "delete" Then
                m_refContent.DeleteTaxonomy(taxonomy_request)
                Response.Redirect("taxonomy.aspx?LangType=" & TaxonomyLanguage, True)
            ElseIf Request.Form("submittedaction") = "deletenode" Then
                Dim CurrentDeleteId As Long = TaxonomyId
                If (Request.Form("LastClickedOn") IsNot Nothing AndAlso Request.Form("LastClickedOn") <> "") Then
                    CurrentDeleteId = Convert.ToInt64(Request.Form("LastClickedOn"))
                End If
                taxonomy_request.TaxonomyId = CurrentDeleteId
                m_refContent.DeleteTaxonomy(taxonomy_request)
                If (CurrentDeleteId = TaxonomyId) Then
                    Response.Redirect("taxonomy.aspx?taxonomyid=" & TaxonomyId, True)
                Else
                    Response.Redirect("taxonomy.aspx?action=viewtree&taxonomyid=" & TaxonomyId & "&LangType=" & TaxonomyLanguage, True)
                End If
            ElseIf Request.Form("submittedaction") = "enable" Then
                Dim CurrentEnableId As Long = TaxonomyId
                If (Request.Form("LastClickedOn") IsNot Nothing AndAlso Request.Form("LastClickedOn") <> "") Then
                    CurrentEnableId = Convert.ToInt64(Request.Form("LastClickedOn"))
                End If
                If Request.Form(alllanguages.UniqueID) = "true" Then
                    m_refContent.UpdateTaxonomyVisible(CurrentEnableId, -1, True)
                Else
                    m_refContent.UpdateTaxonomyVisible(CurrentEnableId, TaxonomyLanguage, True)
                End If
                Response.Redirect("taxonomy.aspx?action=viewtree&taxonomyid=" & TaxonomyId & "&LangType=" & TaxonomyLanguage, True)
            ElseIf Request.Form("submittedaction") = "disable" Then
                Dim CurrentDisableId As Long = TaxonomyId
                If (Request.Form("LastClickedOn") IsNot Nothing AndAlso Request.Form("LastClickedOn") <> "") Then
                    CurrentDisableId = Convert.ToInt64(Request.Form("LastClickedOn"))
                End If
                If Request.Form(alllanguages.UniqueID) = "true" Then
                    m_refContent.UpdateTaxonomyVisible(CurrentDisableId, -1, False)
                Else
                    m_refContent.UpdateTaxonomyVisible(CurrentDisableId, TaxonomyLanguage, False)
                End If
                Response.Redirect("taxonomy.aspx?action=viewtree&taxonomyid=" & TaxonomyId & "&LangType=" & TaxonomyLanguage, True)
            End If
        Else
            taxonomy_data = m_refContent.ReadTaxonomy(taxonomy_request)
            If (taxonomy_data IsNot Nothing) Then
                TaxonomyParentId = taxonomy_data.TaxonomyParentId
                m_strTaxonomyName = taxonomy_data.TaxonomyName
            End If
            AncestorTaxonomyId = TaxonomyId
            m_selectedTaxonomyList = Convert.ToString(TaxonomyId)
            TaxonomyToolBar()
        End If
    End Sub

    Private Sub TaxonomyToolBar()
        Dim AncestorMenuId As Long = 0 'to be removed
        Dim FolderId As Long = 0 ' to be removed
        divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("view all categories of taxonomy") & " """ & m_strTaxonomyName & """" & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) & "' />")
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentEdit.png", "taxonomy.aspx?backaction=viewtree&action=edit&taxonomyid=" & TaxonomyId & "&LangType=" & TaxonomyLanguage, m_refMsg.GetMessage("alt edit button text (taxonomy)"), m_refMsg.GetMessage("btn edit"), ""))
        result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/delete.png", "#", m_refMsg.GetMessage("alt delete button text (taxonomy)"), m_refMsg.GetMessage("btn delete"), "Onclick=""javascript:return DeleteNode();"""))
        result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "btn_exptaxo-nm.gif", "javascript:window.open('taxonomy_imp_exp.aspx?action=export&taxonomyid=" & TaxonomyId & "&LangType=" & TaxonomyLanguage & "','exptaxonomy','status=0,toolbar=0,location=0,menubar=0,directories=0,resizable=0,scrollbars=1,height=100px,width=200px');javascript:void(0);", m_refMsg.GetMessage("alt export taxonomy"), m_refMsg.GetMessage("btn export taxonomy"), ""))
        result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "taxonomy.aspx", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
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
        result.Append("<td>" & m_refstyle.GetHelpButton("ViewTaxonomyTree") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub

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

    Private Function ApprovalText(ByVal flag As Boolean) As String
        Dim result As String = ""
        If (flag) Then
            result = "All Items in this folder required approval."
        Else
            result = "All Items in this folder does not required approval."
        End If
        Return result
    End Function
    Private Sub Util_SetServerJSVariables()
        ltr_confrmDelTax.Text = m_refMsg.GetMessage("alt Are you sure you want to delete this category?")
        ltr_alrtDelTax.Text = m_refMsg.GetMessage("alt Are you sure you want to delete this taxonomy?")
    End Sub
    Private Sub Util_RegisterResources()
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)

        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronModalCss)

        API.Css.RegisterCss(Me, Me.m_refCommon.ApplicationPath & "Tree/css/com.ektron.ui.tree.css", "EktronTreeUITreeCSS")

        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronModalJS)

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.utils.url.js", "EktronTreeUtilsUrlJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.explorer.init.js", "EktronTreeExplorerInitJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.explorer.js", "EktronTreeExplorerJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.explorer.config.js", "EktronTreeExplorerConfigJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.explorer.windows.js", "EktronTreeExplorerWindowsJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.cms.types.js", "EktronTreeCMSTypesJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.cms.parser.js", "EktronTreeCMSParserJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.cms.toolkit.js", "EktronTreeCMSToolkitJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.cms.api.js", "EktronTreeCMSApiJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.ui.contextmenu.js", "EktronTreeUIContextMenuJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.ui.iconlist.js", "EktronTreeUIIconListJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.ui.explore.js", "EktronTreeUIExploreJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.ui.taxonomytree.js", "EktronTreeUITaxonomyTreeJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.net.http.js", "EktronTreeNetHttpJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.lang.exception.js", "EktronTreeLangExceptionJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.utils.form.js", "EktronTreeUtilsFormJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.utils.log.js", "EktronTreeUtilsLogJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.utils.dom.js", "EktronTreeUtilsDOMJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.utils.debug.js", "EktronTreeUtilsDebugJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.utils.string.js", "EktronTreeUtilsStringJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.utils.cookie.js", "EktronTreeUtilsCookieJS")

        API.JS.RegisterJS(Me, Me.m_refCommon.ApplicationPath & "Tree/js/com.ektron.utils.querystring.js", "EktronTreeUtilsQueryStringJS")

        API.Css.RegisterCss(Me, Me.m_refCommon.ApplicationPath & "wamenu/css/com.ektron.ui.menu.css", "EktronMenuUIMenuCSS")
    End Sub
End Class