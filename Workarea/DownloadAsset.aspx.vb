Imports System.IO
Imports System.Net
Imports Ektron.Cms
Imports Ektron.Cms.API.Content.Content
Imports Ektron.ASM.AssetConfig

Partial Class DownloadAsset
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim content_api As New Ektron.Cms.ContentAPI()
        Try
            Dim content_data As Ektron.Cms.ContentData = Nothing
            Dim asset_id As Long = 0
            Dim valid_attempt As Boolean = False
            Dim pdf_path As String = Nothing
            Dim LangbackUp As Integer = 0
            If (Not Request.QueryString("id") Is Nothing) Then
                asset_id = Convert.ToInt64(Request.QueryString("id"))
            End If
            LangbackUp = content_api.ContentLanguage
            If (Not Request.QueryString("LangType") Is Nothing AndAlso content_api.ContentLanguage = -1) Then
                content_api.ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
            End If
            If content_api.ContentLanguage = -1 Then
                content_api.ContentLanguage = content_api.GetCookieValue("SiteLanguage")
            End If
            Dim iTmpCaller As Long = content_api.RequestInformationRef.CallerId
            If (asset_id > 0) Then
                content_api.RequestInformationRef.CallerId = Ektron.Cms.Common.EkConstants.InternalAdmin
                content_api.RequestInformationRef.UserId = Ektron.Cms.Common.EkConstants.InternalAdmin
                Try
                    content_data = content_api.GetContentById(asset_id)
                Catch
                Finally
                    content_api.RequestInformationRef.CallerId = iTmpCaller
                    content_api.RequestInformationRef.UserId = iTmpCaller
                End Try
                If Not content_data Is Nothing Then
                    content_api.ContentLanguage = content_data.LanguageId
                    content_data = Nothing
                End If
                content_data = content_api.ShowContentById(asset_id, content_api.CmsPreview, Not content_api.CmsPreview)
                content_api.ContentLanguage = LangbackUp
            End If
            If ((Not content_data Is Nothing) AndAlso (Not content_data.AssetData Is Nothing) AndAlso (content_data.AssetData.Version.Length > 0)) Then
                Dim filepath As String = Page.Server.MapPath(content_api.EkContentRef.GetViewUrl(Convert.ToInt32(content_data.Type), content_data.AssetData.Id).Replace(Page.Request.Url.Scheme + "://" + Page.Request.Url.Authority, "").Replace(":443", "").Replace(":80", ""))
                If Not filepath Is Nothing Then
                    If File.Exists(filepath) Then
                        valid_attempt = True
                        Dim filename As String = Path.GetFileName(filepath)
                        Dim ext As String = ""
                        ext = Path.GetExtension(filepath)
                        Dim _assetData As New AssetData
                        _assetData.AssetDataFromAssetID(content_data.AssetData.Id)
                        If (ext.Contains("pdf") OrElse ext.Contains("pps")) Then
                            Dim client As New WebClient
                            Dim Buffer() As Byte = client.DownloadData(Convert.ToString(filepath))
                            If Buffer.Length > 0 Then
                                valid_attempt = True
                                Response.Clear()
                                Response.ContentType = IIf(ext.Contains("pdf"), "application/pdf", "application/vnd.ms-powerpoint")
                                Response.AddHeader("Content-Disposition", "attachment; filename=""" & IIf(Request.Browser.Browser = "IE", Server.UrlPathEncode(System.IO.Path.GetFileNameWithoutExtension(_assetData.Handle)), System.IO.Path.GetFileNameWithoutExtension(_assetData.Handle)) & ext & """")
                                Response.BinaryWrite(Buffer)
                            End If
                        Else
                            'If ext.Contains("txt") OrElse ext.Contains("nxb") Then
                            '    filepath = DocumentManagerData.Instance.StorageLocation & _assetData.Storage & ConfigManager.pathChar & _assetData.Name
                            'End If
                            Response.Clear()
                            Response.ContentType = content_data.AssetData.MimeType
                            Response.AddHeader("Content-Disposition", "attachment; filename=""" & IIf(Request.Browser.Browser = "IE", Server.UrlPathEncode(_assetData.Handle), _assetData.Handle) & """")
                            Response.WriteFile(filepath)
                        End If
                        Response.Flush()
                        Try
                            Response.End()
                        Catch
                        End Try
                    End If
                End If
            End If
            If (Not valid_attempt) Then
                notification_message.Text = "File does not exist or you do not have permission to view this file"
                ' Register CSS
                API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
                API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.AllIE)
                Login.Visible = Not content_api.IsLoggedIn()
                content_api.RequestInformationRef.RedirectFromLoginKeyName = Request.Url.PathAndQuery.ToString()
                Login.RedirectFromLoginPage()
                Login.Fill()
            End If

        Catch ex As Exception
            notification_message.Text = "File does not exist or you do not have permission to view this file"
            ' Register CSS
            API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
            API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.AllIE)
            Login.Visible = Not content_api.IsLoggedIn()
            content_api.RequestInformationRef.RedirectFromLoginKeyName = Request.Url.PathAndQuery.ToString()
            Login.RedirectFromLoginPage()
            Login.Fill()
        End Try

    End Sub
End Class
