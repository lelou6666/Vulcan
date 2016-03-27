Imports System.Data
Imports Ektron.Cms
Imports System.Collections.Generic
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.SuggestedResultData

''' <summary>
''' This control is used to output the current Synonym Sets in the CMS to the workarea.
''' </summary>
''' <remarks></remarks>
Partial Class viewsynonyms
    Inherits System.Web.UI.UserControl

    Protected language_data As LanguageData()
    Protected user_data As UserData
    Protected security_data As PermissionData
    Protected m_refSiteApi As New SiteAPI
    Protected m_refUserApi As New UserAPI
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppPath As String = ""
    Protected ContentLanguage As Integer = -1
    Protected objLocalizationApi As New LocalizationAPI()



#Region "Sub: OutputSynonymSets"
    ''' <summary>
    ''' This subroutine accepts an integer indicating the current ContentLanguage specified by the user, and then outputs the current Synonym Sets stored in the CMS to the workarea.  The Keywords associated with each Synonym Set will be truncated in the output so that only one line of terms will be displayed.  A "title" attribute provides the entire list of terms on mouseover.
    ''' </summary>
    ''' <param name="ContentLanguage">An integer representing the current user selected language for the content.</param>
    ''' <remarks></remarks>
    Protected Sub OutputSynonymSets(ByVal ContentLanguage As Integer, ByRef objLiteral As Object)
        Dim api As New Ektron.Cms.CommonApi()
        Try
            Dim synonymSetData As List(Of Ektron.Cms.SynonymData)
            Dim synonymSetObject As New Ektron.Cms.Content.Synonyms(api.RequestInformationRef)
            synonymSetData = synonymSetObject.GetSynonyms(ContentLanguage, EkEnumeration.SynonymOrderBy.name)

            If IsNothing(synonymSetData) Then
                Throw New Exception(m_refMsg.GetMessage("generic no results found"))
            End If

            objLiteral.Text = "<table id=""viewSynonymSets"" class=""ektronGrid"">"
            objLiteral.Text += "<tr class=""title-header"">" & vbCrLf
            objLiteral.Text += "<th class=""left"">" & m_refMsg.GetMessage("lbl synonym header set") & "</th>" & vbCrLf
            objLiteral.Text += "<th class=""center"">" & m_refMsg.GetMessage("generic language") & "</th>" & vbCrLf
            objLiteral.Text += "<th  class=""left"" width=""90%"">" & m_refMsg.GetMessage("lbl synonym header terms") & "</th>" & vbCrLf
            objLiteral.Text += "</tr>" & vbCrLf

            For Each synonymSet As Ektron.Cms.SynonymData In synonymSetData
                'TODO: Pinkesh - Add row striping
                objLiteral.Text += "<tr class=""row"">" & vbCrLf
                objLiteral.Text += "<td><a href=""synonyms.aspx?id=" & synonymSet.ID.ToString & "&#38;LangType=" & synonymSet.LanguageID & "&#38;action=ViewSynonym&#38;bck=vs"">" & Server.HtmlEncode(synonymSet.Name) & "</td>" & vbCrLf
                objLiteral.Text += "<td  class=""center""><img style='vertical-align:middle;' src='" & objLocalizationApi.GetFlagUrlByLanguageID(synonymSet.LanguageID) & "' title='" & synonymSet.LanguageID & "' alt='" & synonymSet.LanguageID & "' /></td>"
                objLiteral.Text += "<td>" & Server.HtmlEncode(synonymSet.TermsKeywords) & "</td>" & vbCrLf
                objLiteral.Text += "</tr>" & vbCrLf
            Next
            objLiteral.Text += "</table>" & vbCrLf
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region

#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = (New CommonApi).EkMsgRef
        AppPath = m_refSiteApi.AppPath

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
        OutputSynonymSets(ContentLanguage, synonymOutput)
    End Sub
#End Region

#Region "Sub: ViewToolBar"
    Private Sub ViewToolBar(ByRef objTitleBar As Object, ByRef objHTMToolBar As Object)
        Dim result As New System.Text.StringBuilder
        ' place the proper title in the Title Bar
        objTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("msg view synonyms"))

        ' build the string for rendering the htmToolBar
        result.Append("<table cellspacing=""0""><tr>")
        If ContentLanguage <> -1 Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/ui/icons/add.png", "synonyms.aspx?action=AddSynonym&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt add button text synonym"), m_refMsg.GetMessage("btn add synonym"), ""))
        End If
        ' if the system is
        If m_refContentApi.EnableMultilingual = 1 Then
            Dim m_refsite As New SiteAPI
            Dim language_data(0) As LanguageData
            language_data = m_refsite.GetAllActiveLanguages()

            result.Append("<td class=""label"" id=""viewText"">")
            If ContentLanguage <> -1 Then
                result.Append("&nbsp;|")
            End If
            result.Append("&nbsp;" & m_refMsg.GetMessage("generic view") & ": ")
            result.Append("<select id='LangType' name='LangType'OnChange=""javascript:LoadLanguage(this.options[this.selectedIndex].value);"">")
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
            result.Append("</select></td>")
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("synonymsets"))
        result.Append("</td>")
        result.Append("</tr></table>")

        ' output the result string to the htmToolBar
        objHTMToolBar.InnerHtml = result.ToString
    End Sub
#End Region
End Class

