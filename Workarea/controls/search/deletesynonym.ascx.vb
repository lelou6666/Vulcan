
Imports System.Data
Imports Ektron.Cms
Imports System.Collections.Generic
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.SuggestedResultData

Partial Class Workarea_search_deletesynonym
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
    Protected synonymID As Guid = Nothing

#Region "Sub: DeleteSynonymSets"
    ''' <summary>
    ''' This subroutine accepts an integer indicating the ID for the term to be deleted as specified by the user, and then outputs the current Synonym Sets stored in the CMS to the workarea.  The Keywords associated with each Synonym Set will be truncated in the output so that only one line of terms will be displayed.  A "title" attribute provides the entire list of terms on mouseover.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteSynonymSets(ByVal synonymID As Guid)
        Dim api As New Ektron.Cms.CommonApi()
        Dim deleteSuccessful As Boolean = False
        Dim deleteSynonymSuccessful As Boolean = False
        Dim deleteSuggestedResultsSuccessful As Boolean = False
        Try
            Dim synonymSet As New SynonymData
            Dim synonymSetObject As New Synonyms(api.RequestInformationRef)
            Dim suggestedResults As New List(Of SuggestedResultData)
            Dim suggestedResultsObject As New EkSuggestedResults(api.RequestInformationRef)
            Dim suggestedResultIDs As New List(Of Guid)

            synonymSet = synonymSetObject.GetSynonymForId(synonymID, ContentLanguage)
            suggestedResults = suggestedResultsObject.GetSuggestedResultSetForTerms(synonymSet.TermID, ContentLanguage)
            For Each SR As SuggestedResultData In suggestedResults
                suggestedResultIDs.Add(SR.ID)
            Next
            If suggestedResultIDs.Count > 0 Then
                deleteSuggestedResultsSuccessful = suggestedResultsObject.DeleteSugestedResultSet(suggestedResultIDs)
            Else
                deleteSuggestedResultsSuccessful = True
            End If
            deleteSynonymSuccessful = synonymSetObject.DeleteSynonyms(synonymID)
            If deleteSynonymSuccessful And deleteSuggestedResultsSuccessful Then
                deleteSuccessful = True
            End If

            If Not deleteSuccessful Then
                Throw New Exception(m_refMsg.GetMessage("generic error delete unsuccessful"))
            End If

        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
        If deleteSuccessful Then Response.Redirect("synonyms.aspx?action=ViewSynonyms")
    End Sub
#End Region


#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = (New CommonApi).EkMsgRef
        m_refMsg = m_refSiteApi.EkMsgRef
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
        ' get the specific Synonym ID
        If Trim(Request.QueryString("id")).Length > 0 Then
            If Not (Trim(Request.QueryString("id")) = Guid.Empty.ToString()) Then
                synonymID = New Guid(Trim(Request.QueryString("id")))
            Else
                synonymID = Nothing
            End If
        Else
            synonymID = Nothing
        End If

        ' Display the Synonym Sets
        DeleteSynonymSets(synonymID)
    End Sub
#End Region
End Class

