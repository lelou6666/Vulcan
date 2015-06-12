Imports System.Data
Imports Ektron.Cms
Imports System.Collections.Generic
Imports Ektron.Cms.API
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.SuggestedResultData

''' <summary>
''' This control is used to output the current Suggested Results in the CMS to the workarea.
''' </summary>
''' <remarks></remarks>
Partial Class Workarea_controls_search_viewsuggestedresults
    Inherits System.Web.UI.UserControl

    Protected language_data As LanguageData()
    Protected user_data As UserData
    Protected security_data As PermissionData
    Protected m_refSiteApi As New SiteAPI
    Protected m_refUserApi As New UserAPI
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected ContentLanguage As Integer = -1
    Protected objLocalizationApi As New LocalizationAPI()

#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' initialize required page load objects/variables
        m_refMsg = (New CommonApi).EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            If (Request.QueryString("LangType") <> "") Then
                ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
            Else
                If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If
        m_refContentApi.RequestInformationRef.ContentLanguage = ContentLanguage

        ' build the toolbar
        ViewToolBar(txtTitleBar, htmToolBar)
        ' Display the Synonym Sets
        OutputSuggestedResults(ContentLanguage, suggestedResultOutput)

        ' Register JS
        JS.RegisterJS(Me, JS.ManagedScript.EktronWorkareaJS)

        ' register CSS
        Css.RegisterCss(Me, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub
#End Region

#Region "SUB: ViewToolBar"
    Protected Sub ViewToolBar(ByRef objTitleBar As Object, ByRef objHTMToolBar As Object)
        Dim result As New System.Text.StringBuilder
        ' place the proper title in the Title Bar
        objTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("msg view suggested results"))

        ' build the string for rendering the htmToolBar
        result.Append("<table><tr>")
        If ContentLanguage <> -1 Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath & "images/UI/Icons/add.png", "suggestedresults.aspx?action=AddSuggestedResult&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt add button text suggested result"), m_refMsg.GetMessage("btn add suggested result"), ""))
        End If
        If m_refContentApi.EnableMultilingual = 1 Then
            Dim m_refsite As New SiteAPI
            Dim language_data(0) As LanguageData
            language_data = m_refsite.GetAllActiveLanguages()

            result.Append("<td class=""label"">&nbsp;")
            If ContentLanguage <> -1 Then
                result.Append("|&nbsp;")
            End If
            result.Append(m_refMsg.GetMessage("generic view") & ":")
            result.Append("</td>")
            result.Append("<td>")
            result.Append("<select id='LangType' name='LangType' OnChange=""javascript:LoadLanguage(this.options[this.selectedIndex].value);"">")
            If ContentLanguage = -1 Then
                result.Append("<option value=" & ALL_CONTENT_LANGUAGES & " selected>" & m_refMsg.GetMessage("generic all") & "</option>")
            Else
                result.Append("<option value=" & ALL_CONTENT_LANGUAGES & ">" & m_refMsg.GetMessage("generic all") & "</option>")
            End If

            For count As Integer = 0 To language_data.Length - 1
                If Convert.ToString(ContentLanguage) = Convert.ToString(language_data(count).Id) Then
                    result.Append("<option value=" & language_data(count).Id & " selected>" & language_data(count).Name & "</option>")
                Else
                    result.Append("<option value=" & language_data(count).Id & ">" & language_data(count).Name & "</option>")
                End If
            Next
            result.Append("</select>")
            result.Append("</td>")
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("suggestedresults"))
        result.Append("</td>")
        result.Append("</tr></table>")

        ' output the result string to the htmToolBar
        objHTMToolBar.InnerHtml = result.ToString
    End Sub
#End Region

#Region "SUB: OutputSuggestedResults"
    Protected Sub OutputSuggestedResults(ByVal ContentLanguage As Integer, ByRef objLiteral As Object)
        Dim api As New Ektron.Cms.CommonApi()
        Dim formerTermID As Guid = Nothing
        Try
            Dim suggestedResultTermData As Ektron.Cms.Common.TermsData
            Dim suggestedResultTermsObject As New Ektron.Cms.Content.Terms(api.RequestInformationRef)
            Dim suggestedResultsData As List(Of Ektron.Cms.SuggestedResultData)
            Dim suggestedResultsObject As New Ektron.Cms.Content.EkSuggestedResults(api.RequestInformationRef)
            suggestedResultsData = suggestedResultsObject.GetSuggestedResultSet(EkEnumeration.SuggestedResultOrderBy.name, ContentLanguage)

            If IsNothing(suggestedResultsData) Then
                Throw New Exception(m_refMsg.GetMessage("generic no results found"))
            End If

            objLiteral.Text = "<table id=""viewSuggestedResults"" class=""ektronGrid"">"
            objLiteral.Text += "<tr class=""title-header"">" & vbCrLf
            objLiteral.Text += "<th>" & m_refMsg.GetMessage("lbl suggested results header search for") & "</th>" & vbCrLf
            objLiteral.Text += "<th class=""center"">" & m_refMsg.GetMessage("generic language") & "</th>" & vbCrLf
            objLiteral.Text += "<th class=""left"">" & m_refMsg.GetMessage("msg view suggested results") & "</th>" & vbCrLf
            objLiteral.Text += "</tr>" & vbCrLf

            Dim stripeValue As Integer = 1
            For Each suggestedResult As Ektron.Cms.SuggestedResultData In suggestedResultsData
                suggestedResultTermData = suggestedResultTermsObject.GetTermById(suggestedResult.KeywordID, ContentLanguage)
                If formerTermID <> suggestedResult.KeywordID Then
                    ' add a whole new row
                    If Not IsNothing(formerTermID) Then
                        ' for everything except the first time through, close the previous td and tr
                        objLiteral.Text += "</div></td>" & vbCrLf
                        objLiteral.Text += "</tr>" & vbCrLf
                    End If
                    If stripeValue = -1 Then
                        objLiteral.Text += "<tr class=""stripe"">" & vbCrLf
                    Else
                        objLiteral.Text += "<tr>" & vbCrLf
                    End If
                    objLiteral.Text += "<td><a href=""suggestedresults.aspx?termID=" & suggestedResult.KeywordID.ToString & "&#38;LangType=" & suggestedResult.ContentLanguage & "&#38;action=ViewSuggestedResult"""
                    If suggestedResult.Type = TermType.SetTerm Then
                        objLiteral.Text += " class=""termSet"""
                    End If
                    objLiteral.Text += " title=""" & Server.HtmlEncode(suggestedResultTermData.Terms) & """>" & Server.HtmlEncode(suggestedResult.TermName) & "</td>" & vbCrLf
                    objLiteral.Text += "<td class=""center""><img style='vertical-align:middle;' src='" & objLocalizationApi.GetFlagUrlByLanguageID(suggestedResult.ContentLanguage) & "' title='" & suggestedResult.ContentLanguage & "' alt='" & suggestedResult.ContentLanguage & "' /></td>"
                    objLiteral.Text += "<td class=""left""><div><a class=""suggestedResultTitles"" title=""" & Server.HtmlEncode(suggestedResult.QuickLink) & """ href=""" & Server.HtmlEncode(suggestedResult.QuickLink) & """ onclick=""window.open(this.href); return false;"">" & Server.HtmlEncode(suggestedResult.Title) & "</a>"
                Else
                    ' only add the additional SR info
                    objLiteral.Text += ", <a class=""suggestedResultTitles"" title=""" & Server.HtmlEncode(suggestedResult.QuickLink) & """ href=""" & Server.HtmlEncode(suggestedResult.QuickLink) & """ onclick=""window.open(this.href); return false;"">" & Server.HtmlEncode(suggestedResult.Title) & "</a>"
                End If
                ' clear the suggestedResultTermData object
                suggestedResultTermData = Nothing
                formerTermID = suggestedResult.KeywordID
                stripeValue = -1 * (stripeValue)
            Next
            ' close any loose ends
            If Not IsNothing(suggestedResultsData) Then
                objLiteral.Text += "</td>" & vbCrLf
                objLiteral.Text += "</tr>" & vbCrLf
            End If
            objLiteral.Text += "</table>" & vbCrLf

        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region
End Class
