Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Content
Imports System.IO
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI

Partial Class Workarea_EditOfficeAsset
    Inherits System.Web.UI.Page

    Private szdavfolder As String = ""
    Private content_edit_data As ContentEditData

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '30257- This document should never be cached.  When content status changes
        'the page is refreshed to show change in status.  This change does 
        'not appear when page is cached.
        Response.Cache.SetCacheability(HttpCacheability.NoCache)

        Dim contentid As Long = 0
        Dim folderid As Long = 0
        Dim ContentLanguage As Integer = -1
        Dim AppUI As New CommonApi
        Dim content_api As New Ektron.Cms.ContentAPI()

        If (Not Request.QueryString("id") Is Nothing) Then
            contentid = Convert.ToInt64(Request.QueryString("id"))

            If (Not (Request.QueryString("LangType") Is Nothing)) AndAlso (Int32.TryParse(Request.QueryString("LangType"), ContentLanguage)) AndAlso (ContentLanguage > 0) Then
                If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Or ContentLanguage = ALL_CONTENT_LANGUAGES Then
                    ContentLanguage = AppUI.DefaultContentLanguage
                End If
                AppUI.ContentLanguage = ContentLanguage
                content_api.ContentLanguage = ContentLanguage
            Else
                ContentLanguage = AppUI.DefaultContentLanguage
            End If
            content_edit_data = content_api.GetContentForEditing(contentid)
            If (Not Request.QueryString("executeActiveX") Is Nothing AndAlso Request.QueryString("executeActiveX") = "true") Then
                Dim strfilename As String
                Dim assetmanagementService As AssetManagement.AssetManagementService = New AssetManagement.AssetManagementService
                Dim assetData As Ektron.ASM.AssetConfig.AssetData = assetmanagementService.GetAssetData(content_edit_data.AssetData.Id)
                strfilename = GetFolderPath(content_edit_data.FolderId) & assetData.Handle
                Dim sJS As String = "editInMSOffice('" & strfilename & "');"
                Page.ClientScript.RegisterStartupScript(GetType(Page), "ShowInOffice", sJS.ToString(), True)
            End If

        End If
    End Sub

    Public Function GetFolderPath(ByVal Id As Long) As String
        Dim contentAPI As New ContentAPI()
        Dim siteAPI As New SiteAPI()

        szdavfolder = "ekdavroot"

        Dim sitePath As String = siteAPI.SitePath.ToString().TrimEnd(New Char() {"/"c}).TrimStart(New Char() {"/"c})
        szdavfolder = szdavfolder.TrimEnd(New Char() {"/"c}).TrimStart(New Char() {"/"c})
        If (Page.Request.Url.Host.ToLower() = "localhost") Then
            szdavfolder = Page.Request.Url.Scheme & Uri.SchemeDelimiter & System.Environment.MachineName & "/" & sitePath & "/" & _
                     szdavfolder & "_" & siteAPI.UserId & "_" & siteAPI.UniqueId & _
                     (IIf(Context.Request.QueryString("LangType") IsNot Nothing, "_" & Context.Request.QueryString("LangType").ToString(), "")) & "/"
        Else
            szdavfolder = Page.Request.Url.Scheme & Uri.SchemeDelimiter & Page.Request.Url.Authority & "/" & sitePath & "/" & _
                     szdavfolder & "_" & siteAPI.UserId & "_" & siteAPI.UniqueId & _
                     (IIf(Context.Request.QueryString("LangType") IsNot Nothing, "_" & Context.Request.QueryString("LangType").ToString(), "")) & "/"
        End If

        Dim szFolderPath As String = contentAPI.EkContentRef.GetFolderPath(Id)
        szFolderPath = szFolderPath.Replace("\", "/")
        szFolderPath = szFolderPath.TrimStart(New Char() {"/"c})
        szFolderPath = szFolderPath.Replace("\\", "/")
        If (szFolderPath.Length > 0) Then
            szFolderPath = szdavfolder & szFolderPath & "/"
        Else
            szFolderPath = szdavfolder
        End If

        Return szFolderPath
    End Function

End Class
