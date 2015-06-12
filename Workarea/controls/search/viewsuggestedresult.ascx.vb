Imports System.Data
Imports Ektron.Cms
Imports System.Collections.Generic
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.SuggestedResultData
Imports System.Web.HttpUtility

Partial Class Workarea_controls_search_viewsuggestedresult
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
    Protected thisContentLanguage As String = ""
    Protected api As New CommonApi()
    Protected PageMode As String = ""
    Protected PageAction As String = ""
    Protected inputTermID As Guid = Nothing

#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' initialize necessary variables
        m_refMsg = (New CommonApi).EkMsgRef
        m_refMsg = m_refSiteApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        ContentLanguage = m_refSiteApi.ContentLanguage
        PageAction = Request.QueryString("action").ToLower
        ' get the term info if needed
        Dim termData As New Ektron.Cms.Common.TermsData
        Dim termDataObject As New Ektron.Cms.Content.Terms(api.RequestInformationRef)
        If Not (Trim(Request.QueryString("termID")) = Guid.Empty.ToString()) Then
            inputTermID = New Guid(Trim(Request.QueryString("termID")))
            ' instantiate the data class for this Synonym Set
            termData = termDataObject.GetTermById(inputTermID, ContentLanguage)
            ViewToolBar(termData)
        End If
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
        ' fill in the data for this suggested result
        termName.Text = termData.Name.ToString

        ' if we're editing an existing set of Suggested Results,
        ' let's get them and output them to the array
        Dim suggestedResultList As New List(Of SuggestedResultData)
        Dim suggestedResultDataObj As New Content.EkSuggestedResults(api.RequestInformationRef)
        Dim ulBuilder As New System.Text.StringBuilder
        Dim jsBuilder As New System.Text.StringBuilder

        ' get the results for this term
        suggestedResultList = suggestedResultDataObj.GetSuggestedResultSetForTerms(termData.TermID, ContentLanguage)

        For Each SR As SuggestedResultData In suggestedResultList
            ulBuilder.Append("<li class=""suggestedResult"">" & vbCrLf)
            ulBuilder.Append("<div class=""anchor"">" & vbCrLf)
            ulBuilder.Append("<span class=""suggestedResultLink"">" & SR.Title.ToString & "</span>" & vbCrLf)
            ulBuilder.Append("<div class=""suggestedResultSummary"">" & HtmlDecode(SR.Summary.ToString) & "</div>" & vbCrLf)
            ulBuilder.Append("</div>" & vbCrLf)
            ulBuilder.Append("</li>" & vbCrLf)
        Next

        suggestedResultsOutput.Text = ulBuilder.ToString

        For Each SR As SuggestedResultData In suggestedResultList
            jsBuilder.Append("var existingSuggestedResultObject = new suggestedResultObject(")
            jsBuilder.Append("'" & SR.ID.ToString.Replace("'", "\'") & "',")
            jsBuilder.Append("'" & SR.Title.ToString.Replace("'", "\'") & "',")
            jsBuilder.Append("'" & SR.QuickLink.ToString.Replace("'", "\'") & "',")
            jsBuilder.Append("'" & SR.Summary.ToString.Replace("'", "\'").Replace(Chr(13), " ").Replace(Chr(10), " ") & "',")
            jsBuilder.Append(SR.ContentID.ToString & ",")
            jsBuilder.Append(SR.SearchOrder.ToString)
            jsBuilder.Append(");" & vbCrLf)
            jsBuilder.Append("arrSuggestedResults.push(existingSuggestedResultObject);" & vbCrLf & vbCrLf)
        Next
        javaScriptSRObjects.Text = jsBuilder.ToString
    End Sub
#End Region

#Region "SUB: ViewToolBar"
    Private Sub ViewToolBar(ByRef term As Ektron.Cms.Common.TermsData)
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(String.Format(m_refMsg.GetMessage("msg edit-delete suggested results"), """" & term.Name & """"))

        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentEdit.png", "suggestedresults.aspx?action=EditSuggestedResult&LangType=" & ContentLanguage & "&termID=" & term.TermID.ToString, m_refMsg.GetMessage("msg edit suggested result"), m_refMsg.GetMessage("generic edit title"), ""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/delete.png", "suggestedresults.aspx?action=DeleteSuggestedResult&termID=" & term.TermID.ToString & "&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt delete suggested results button text"), m_refMsg.GetMessage("generic delete title"), "OnClick=""javascript:return VerifyDeleteSuggestedResultSet();"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("editsuggestedresults"))
        result.Append("</td>")
        result.Append("</tr></table>")

        ' output the result string to the htmToolBar
        htmToolBar.InnerHtml = result.ToString
    End Sub
#End Region

End Class
