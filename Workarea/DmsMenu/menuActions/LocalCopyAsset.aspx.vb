Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Content
Imports System.IO

Partial Class LocalCopyAsset
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim content_api As New Ektron.Cms.ContentAPI()
            Dim m_refContent As Ektron.Cms.Content.EkContent = content_api.EkContentRef
            Dim content_data As Ektron.Cms.ContentData = Nothing
            Dim content_id As Long = 0
            Dim valid_attempt As Boolean = False
            Dim ContentLanguage As Integer
            Dim AppUI As New CommonApi

            If (Not Request.QueryString("id") Is Nothing) Then
                content_id = Convert.ToInt64(Request.QueryString("id"))

                If (Not (Request.QueryString("content_language") Is Nothing)) AndAlso (Int32.TryParse(Request.QueryString("content_language"), ContentLanguage)) AndAlso (ContentLanguage > 0) Then
                    If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Or ContentLanguage = ALL_CONTENT_LANGUAGES Then
                        ContentLanguage = AppUI.DefaultContentLanguage
                    End If
                    AppUI.ContentLanguage = ContentLanguage
                    content_api.ContentLanguage = ContentLanguage
                Else
                    ContentLanguage = AppUI.DefaultContentLanguage
                End If

                'if checkout was clicked
                Dim checkOutAsset As Boolean
                If (Not Request.QueryString("checkout") Is Nothing) AndAlso (Boolean.TryParse(Request.QueryString("checkout"), checkOutAsset)) AndAlso (checkOutAsset) Then
                    Dim contData As ContentData = content_api.GetContentById(content_id, ContentAPI.ContentResultType.Staged)
                    'If checkout and save was clicked on a content in the "S" state, checkout will throw an exception
                    'Take ownership of content before checkout
                    If (contData IsNot Nothing) AndAlso (contData.UserId <> content_api.RequestInformationRef.UserId) AndAlso (contData.Status = "S") Then
                        content_api.EkContentRef.TakeOwnership(content_id)
                    End If
                    content_api.EkContentRef.CheckContentOutv2_0(content_id)
                End If
                '
                If (content_id > 0) Then
                    content_data = content_api.GetContentById(content_id)
                End If
                If ((Not content_data Is Nothing) AndAlso (Not content_data.AssetData Is Nothing) AndAlso (content_data.AssetData.Version.Length > 0)) Then
                    'GetContentById returns content in default language if no content exists for ContentLanguage
                    If (content_data.LanguageId = ContentLanguage) Then
                        Dim objAssetMgtService As AssetManagement.AssetManagementService
                        objAssetMgtService = New AssetManagement.AssetManagementService
                        Dim filepath As String = objAssetMgtService.GetViewUI(content_data.AssetData.Id, Ektron.ASM.AssetConfig.InstanceType.current, content_data.AssetData.Version, 2)
                        If (Request.QueryString("originalimage") IsNot Nothing AndAlso Request.QueryString("originalimage") = True) Then
                            Dim _path As String = content_api.EkContentRef.GetViewUrl(Convert.ToInt32(content_data.Type), content_data.AssetData.Id).Replace(Page.Request.Url.Scheme + "://" + Page.Request.Url.Host, "")
                            If _path.StartsWith(":") Then 'If https the assetmanagementservice tends to send the port number too
                                _path = _path.Substring(_path.IndexOf("/"))
                            End If
                            filepath = Page.Server.MapPath(_path)
                            filepath = filepath.Replace(content_data.AssetData.Id, "orig_" + content_data.AssetData.Id)
                            If Not filepath Is Nothing Then
                                If File.Exists(filepath) Then
                                    valid_attempt = True
                                    Dim assetmanagementService As AssetManagement.AssetManagementService = New AssetManagement.AssetManagementService
                                    Dim assetData As Ektron.ASM.AssetConfig.AssetData = assetmanagementService.GetAssetData(content_data.AssetData.Id)
                                    Response.Clear()
                                    'Response.ContentType = "application/octet-stream"
                                    Response.ContentType = content_data.AssetData.MimeType
                                    Response.AddHeader("Content-Disposition", _
                                      "attachment; filename=""" & HttpUtility.UrlPathEncode(assetData.Handle) & """")
                                    Response.WriteFile(filepath)
                                    Response.Flush()
                                    Try
                                        Response.End()
                                    Catch

                                    End Try
                                    Return
                                End If
                            End If
                        End If

                        If (filepath.IndexOf("?") >= 0) Then
                            filepath = filepath + "&mimeType=octet"
                        Else
                            filepath = filepath + "?mimeType=octet"
                        End If
                        Response.Redirect(filepath)
                    End If
                End If

                If (Not valid_attempt) Then
                    notification_message.Text = "File does not exist or you do not have permission to view this file"
                End If
            End If

        Catch tex As Threading.ThreadAbortException
            notification_message.Text = ""
        Catch ex As Exception
            notification_message.Text = "File does not exist or you do not have permission to view this file"
        End Try

    End Sub
End Class
