Imports System.Data
Imports Ektron.Cms
Imports System.Collections.Generic
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.SuggestedResultData

Partial Class viewsynonym
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

#Region "SUB: OutputSynonymSetsForID"
    Protected Sub OutputSynonymSetsForID(ByRef synonymSetData As SynonymData)
        synonymOutput.Text = "<table class=""ektronForm"">" & vbCrLf
        synonymOutput.Text += "  <tr>" & vbCrLf
        synonymOutput.Text += "    <td class=""label""><label for=""setName"">" & m_refMsg.GetMessage("lbl synonym set name") & ":</label></td>" & vbCrLf
        synonymOutput.Text += "    <td class=""readOnlyValue"">" & Server.HtmlEncode(synonymSetData.Name) & "</td>" & vbCrLf
        synonymOutput.Text += "  </tr>" & vbCrLf
        synonymOutput.Text += "  <tr>" & vbCrLf
        synonymOutput.Text += "    <td class=""label""><label for=""setTerms"">" & m_refMsg.GetMessage("lbl synonym header terms") & ":</label></td>" & vbCrLf
        synonymOutput.Text += "    <td class=""readOnlyValue""><div class=""viewSynonymTerms"">" & Server.HtmlEncode(synonymSetData.TermsKeywords) & "</div></td>" & vbCrLf
        synonymOutput.Text += "  </tr>" & vbCrLf
        synonymOutput.Text += "  </table>"
    End Sub
#End Region


#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = (New CommonApi).EkMsgRef
        m_refMsg = m_refSiteApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        ' get and apply the Content Language value
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
            If Not Request.QueryString("id").ToString() = Guid.Empty.ToString() Then
                synonymID = New Guid(Trim(Request.QueryString("id")))
            Else
                synonymID = Nothing
            End If
        Else
            synonymID = Nothing
        End If

        Dim api As New Ektron.Cms.CommonApi()
        Try
            Dim synonymSetData As SynonymData
            Dim synonymSetObject As New Ektron.Cms.Content.Synonyms(api.RequestInformationRef)
            synonymSetData = synonymSetObject.GetSynonymForId(synonymID, ContentLanguage)

            ViewToolBar(synonymSetData)
            OutputSynonymSetsForID(synonymSetData)
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region

#Region "SUB: ViewToolBar"
    Private Sub ViewToolBar(ByRef synonymSetData As SynonymData)
        Dim result As New System.Text.StringBuilder
        Dim referrer As String = Request.QueryString("bck")
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("msg view synonym") & " """ & synonymSetData.Name & """")

        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/contentEdit.png", "synonyms.aspx?action=EditSynonym&LangType=" & synonymSetData.LanguageID & "&id=" & synonymSetData.ID.ToString, m_refMsg.GetMessage("alt edit synonym set"), m_refMsg.GetMessage("generic edit title"), ""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/delete.png", "synonyms.aspx?action=DeleteSynonym&id=" & synonymSetData.ID.ToString & "&LangType=" & synonymSetData.LanguageID, m_refMsg.GetMessage("alt delete synonym set"), m_refMsg.GetMessage("generic delete title"), "OnClick=""javascript:return VerifyDeleteSynonym();"""))

        If referrer = "vs" Then
            ' we want to "back" button to take the user to the ViewSynonyms page
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "synonyms.aspx?action=ViewSynonyms&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            ' anything else, and return them to where they came from
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("viewsynonymset"))
        result.Append("</td>")
        result.Append("</tr></table>")

        ' output the result string to the htmToolBar
        htmToolBar.InnerHtml = result.ToString
    End Sub
#End Region


End Class

