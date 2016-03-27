Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.SuggestedResultData
Imports System.Collections.Generic

Partial Class Workarea_controls_search_deletesuggestedresults
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
    Protected inputTermID As Long = -1
    Protected termID As Guid = Nothing

#Region "Sub: DeleteSuggestedResultsSets"
    ''' <summary>
    ''' This subroutine accepts an integer indicating the ID for the term to be deleted as specified by the user, and then outputs the current Synonym Sets stored in the CMS to the workarea.  The Keywords associated with each Synonym Set will be truncated in the output so that only one line of terms will be displayed.  A "title" attribute provides the entire list of terms on mouseover.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteSuggestedResultSets(ByVal termID As Guid)
        Dim api As New Ektron.Cms.CommonApi()
        Dim suggestedResultData As New List(Of SuggestedResultData)
        Dim idList As New List(Of Guid)
        Dim deleteSuccessful As Boolean = False

        Try

            Dim suggetsedResultObject As New Ektron.Cms.Content.EkSuggestedResults(api.RequestInformationRef)
            ' get the suggested results to delete
            suggestedResultData = suggetsedResultObject.GetSuggestedResultSetForTerms(termID, ContentLanguage)
            For Each SR As SuggestedResultData In suggestedResultData
                idList.Add(SR.ID)
            Next
            deleteSuccessful = suggetsedResultObject.DeleteSugestedResultSet(idList)

            If Not deleteSuccessful Then
                Throw New Exception(m_refMsg.GetMessage("generic error delete unsuccessful"))
            End If

        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
        If deleteSuccessful Then Response.Redirect("suggestedresults.aspx?action=ViewSuggestedResults")
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
        If Trim(Request.QueryString("termID")).Length > 0 Then
            If Not (Trim(Request.QueryString("termID")) = Guid.Empty.ToString()) Then
                termID = New Guid(Trim(Request.QueryString("termID")))
            Else
                termID = Nothing
            End If
        Else
            termID = Nothing
        End If

        ' Display the Synonym Sets
        DeleteSuggestedResultSets(termID)
    End Sub
#End Region

End Class
