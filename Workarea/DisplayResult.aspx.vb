Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common.EkConstants
Imports System.IO

Partial Class DisplayResult
    Inherits System.Web.UI.Page

    Protected m_intContentLanguage As Integer = -1
    Protected m_refContApi As New ContentAPI


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RegisterResources()
    End Sub

    Public Sub renderInfo()
        Dim libId As Long
        Dim contId As Long
        Dim formId As Long
        Dim folderId As Long
        Dim contentId As Long
        Dim strSummary As String
        Dim m_refAPI As New CommonApi
        Dim ekc As Ektron.Cms.Content.EkContent
        Dim libItem As Ektron.Cms.LibraryData
        Dim custFldsCol As Collection
        Dim custFld As Collection
        Dim custFldName As String
        Dim custFldID As Long
        Dim metaValue As String
        Dim searchableMeta As Boolean
        Dim contCol As Collection

        Try

            ekc = m_refAPI.EkContentRef

            libId = CLng(Request.QueryString("libid"))
            contId = CLng(Request.QueryString("cid"))
            formId = CLng(Request.QueryString("fid"))

            ' Ensure that the language is selected/initialized:
            If (Not (Request.QueryString("LangType") Is Nothing)) Then
                If (Request.QueryString("LangType") <> "") Then
                    m_intContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                    m_refContApi.SetCookieValue("LastValidLanguageID", m_intContentLanguage)
                Else
                    If m_refContApi.GetCookieValue("LastValidLanguageID") <> "" Then
                        m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"))
                    End If
                End If
            Else
                If m_refContApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    m_intContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
            If m_intContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
                m_refContApi.ContentLanguage = ALL_CONTENT_LANGUAGES
            Else
                m_refContApi.ContentLanguage = m_intContentLanguage
            End If

            If (libId > 0) Then
                ' Show the library item:

                ' Get the library item object:
                libItem = m_refContApi.GetLibraryItemByID_UnAuth(libId)
                contentId = libItem.ContentId()

                ' Show the title:
                Response.Write("<h2 class=""displayresult_libitem_title"" >" & libItem.Title() & "</h2>")

                ' Show the custom fields (based on the items' folder):
                'folderId = libItem.ParentId INVESTIGATE: The value returned in libItem from GetLibraryItemByID_UnAuth is wrong (as are others, like the flolder name!)...
                folderId = ekc.GetJustFolderIdByContentId(contentId)
                custFldsCol = ekc.GetFieldsByFolder(folderId, m_refContApi.ContentLanguage)
                '    Returns collection from DataIo:
                '        CustomFieldID As Integer
                '        CustomFieldName As String
                '        Required As Integer
                '        Assigned As Integer
                '        OwningFolderID As Integer
                '
                If (custFldsCol.Count > 0) Then
                    Response.Write("<div class=""displayresult_libitem_metadata"" >")
                    For Each custFld In custFldsCol
                        custFldName = custFld("CustomFieldName")
                        custFldID = custFld("CustomFieldID")
                        ' only show searchable custom fields, that are set to allow searching:
                        searchableMeta = ekc.IsMetadataSearchableByID(custFldID)
                        If (searchableMeta) Then
                            ' Get the metadata:
                            metaValue = ekc.GetJustMetaDataByContIdMetaId(contentId, custFldID)
                            Response.Write("<div class=""displayresult_libitem_metadataitem"" >")
                            Response.Write(custFldName & ": " & metaValue)
                            Response.Write("</div>")
                        End If
                    Next
                    Response.Write("</div>")
                End If

                ' Show the image:
                Response.Write("<div class=""displayresult_libitem_image"" >")
                Response.Write("<img src=""" & libItem.FileName() & """ />")
                Response.Write("</div>")

                ' Show the Description (content teaser/summary)
                strSummary = m_refContApi.GetJustTeaserByContentId(contentId, libItem.LanguageId)
                Response.Write("<div class=""displayresult_libitem_summary"" >")
                Response.Write(strSummary)
                Response.Write("</div>")
            ElseIf (contId > 0) Then
                contCol = ekc.GetContentByIDv2_0(contId)
                folderId = CLng(contCol.Item("FolderID"))
                Response.Write("<h3>" & contCol.Item("ContentTitle") & "</h3>")

                custFldsCol = ekc.GetFieldsByFolder(folderId, m_refContApi.ContentLanguage)
                '    Returns collection from DataIo:
                '        CustomFieldID As Integer
                '        CustomFieldName As String
                '        Required As Integer
                '        Assigned As Integer
                '        OwningFolderID As Integer
                '
                For Each custFld In custFldsCol
                    custFldName = custFld("CustomFieldName")
                    custFldID = custFld("CustomFieldID")
                    ' only show searchable custom fields, that are set to allow searching:
                    searchableMeta = ekc.IsMetadataSearchableByID(custFldID)
                    If (searchableMeta) Then
                        ' Get the metadata:
                        metaValue = ekc.GetJustMetaDataByContIdMetaId(contentId, custFldID)
                        Response.Write(custFldName & ": " & metaValue & "<br />")
                    End If
                Next

                Response.Write("<br />")
                Response.Write(contCol.Item("ContentHtml"))
            ElseIf (formId > 0) Then
                contCol = ekc.GetContentByIDv2_0(contId)
                folderId = CLng(contCol.Item("FolderID"))
                Response.Write("<h3>" & contCol.Item("ContentTitle") & "</h3>")

                custFldsCol = ekc.GetFieldsByFolder(folderId, m_refContApi.ContentLanguage)
                '    Returns collection from DataIo:
                '        CustomFieldID As Integer
                '        CustomFieldName As String
                '        Required As Integer
                '        Assigned As Integer
                '        OwningFolderID As Integer
                '
                For Each custFld In custFldsCol
                    custFldName = custFld("CustomFieldName")
                    custFldID = custFld("CustomFieldID")
                    ' only show searchable custom fields, that are set to allow searching:
                    searchableMeta = ekc.IsMetadataSearchableByID(custFldID)
                    If (searchableMeta) Then
                        ' Get the metadata:
                        metaValue = ekc.GetJustMetaDataByContIdMetaId(contentId, custFldID)
                        Response.Write(custFldName & ": " & metaValue & "<br />")
                    End If
                Next

                Response.Write("<br />")
                Response.Write(contCol.Item("ContentHtml"))
                'Response.Write(m_AppRef.ecmContentBlock(formId))
            End If

            m_refContApi = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Protected Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, m_refContApi.AppPath & "default.css", "EktronDefaultCss")
        Ektron.Cms.API.Css.RegisterCss(Me, m_refContApi.AppPath & "displayresult.css", "EktronDefaultResultCss")
    End Sub
End Class
