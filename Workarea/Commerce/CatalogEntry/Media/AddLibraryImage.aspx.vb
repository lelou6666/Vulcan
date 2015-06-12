Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Workarea
Imports System.IO

Public Class ThumbnailData

    Private _ImageName As String
    Private _Filename As String
    Private _Path As String
    Private _Alt As String
    Private _Title As String

    Public Sub New(ByVal imageName As String, ByVal path As String, ByVal alt As String, ByVal title As String)

        _ImageName = imageName
        _Path = path
        _Alt = alt
        _Title = title

    End Sub

    Public ReadOnly Property ImageName() As String
        Get
            Return _ImageName
        End Get
    End Property
    Public ReadOnly Property Path() As String
        Get
            Return _Path
        End Get
    End Property
    Public ReadOnly Property Alt() As String
        Get
            Return _Alt
        End Get
    End Property
    Public ReadOnly Property Title() As String
        Get
            Return _Title
        End Get
    End Property
End Class

Partial Class Workarea_Commerce_CatalogEntry_Media_AddLibraryImage
    Inherits System.Web.UI.Page

#Region "Member Variables"

    Protected _ImageId As Long = 0
    Protected _ProductId As Long = 0
    Protected _ContentApi As ContentAPI = New ContentAPI()
    Protected _MessageHelper As EkMessageHelper = _ContentApi.EkMsgRef
    Protected _ProductType As New ProductType(_ContentApi.RequestInformationRef)
    Protected _ProductTypeData As New ProductTypeData
    Protected _LibraryConfigData As New Ektron.Cms.LibraryConfigData
    Protected _CatalogEntryApi As New Ektron.Cms.Commerce.CatalogEntryApi
    Protected _ProductData As New ProductData
    Protected _Thumbnails As New List(Of ThumbnailData)

#End Region

#Region "Events"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _LibraryConfigData = Me._ContentApi.GetLibrarySettings(0)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            _ImageId = Long.Parse(Request.QueryString("imageId"))
            _ProductTypeData = _ProductType.GetItem(Long.Parse(Request.QueryString("productTypeId")))

            Dim imageData As LibraryData = _ContentApi.GetLibraryItemByID_UnAuth(_ImageId)
            Dim folderData As FolderData = _ContentApi.GetFolderById(imageData.ParentId)

            If folderData.PrivateContent = False Then
                Dim uploadImagePath As String = _ContentApi.GetPathByFolderID(_ProductData.Id)

                'Dim sPhysicalPath As String = Server.MapPath(sWebPath)

                Dim sFileName As String = imageData.FileName.Substring((imageData.FileName.LastIndexOf("/") + 1), (imageData.FileName.Length - 1) - imageData.FileName.LastIndexOf("/"))
                Dim sWebPath As String = imageData.FileName.Replace(sFileName, String.Empty)
                sWebPath = sWebPath.Replace("//", "/")
                Dim sPhysicalPath As String = Server.MapPath(imageData.FileName.Replace(sFileName, ""))


                'Begins: Generate thumbnails. Generates thumbnails for various pixes sizes.
                If _ProductTypeData.DefaultThumbnails.Count > 0 Then
                    Dim thumbnailCreator As New EkFileIO
                    Dim thumbnailResult As Boolean = False
                    'Dim sourceFile As String = Server.MapPath(_LibraryConfigData.ImageDirectory) & sFileName
                    Dim sourceFile As String = Server.MapPath(imageData.FileName)
                    For Each thumbnail As ThumbnailDefaultData In _ProductTypeData.DefaultThumbnails
                        Dim fileNameNoExtension As String = sFileName.Replace(System.IO.Path.GetExtension(sFileName), "")
                        Dim fileNameExtension As String = System.IO.Path.GetExtension(sFileName)
                        Dim thumbnailFile As String = sPhysicalPath & "\" & "thumb_" & fileNameNoExtension & Replace(thumbnail.Title, "[filename]", "") & fileNameExtension
                        thumbnailResult = thumbnailCreator.CreateThumbnail(sourceFile, thumbnailFile, thumbnail.Width, thumbnail.Height)

                        '766 load balancing handled by service - no code needed for load balancing

                        _Thumbnails.Add(New ThumbnailData("thumb_" & fileNameNoExtension & Replace(thumbnail.Title, "[filename]", "") & fileNameExtension, sWebPath.TrimEnd("/") & "/", fileNameNoExtension & Replace(thumbnail.Title, "[filename]", "") & fileNameExtension, fileNameNoExtension & Replace(thumbnail.Title, "[filename]", "") & fileNameExtension))
                    Next
                End If
                'Ends : Generate Thumbnails.

                Dim libraryImage As Drawing.Bitmap
                Dim libraryPhysicalPath As String = Server.MapPath(_LibraryConfigData.ImageDirectory)
                libraryImage = New Drawing.Bitmap(Server.MapPath(imageData.FileName))

                ' Add media image    
                Me.litAddMediaJS.Text &= "<script type=""text/javascript"">" & Environment.NewLine
                Me.litAddMediaJS.Text &= "  var newImageObj = {" & Environment.NewLine
                Me.litAddMediaJS.Text &= "      id: """ & imageData.Id.ToString() & """," & Environment.NewLine
                Me.litAddMediaJS.Text &= "      title: """ & imageData.Title & """," & Environment.NewLine
                Me.litAddMediaJS.Text &= "      altText: """ & imageData.Title & """," & Environment.NewLine
                Me.litAddMediaJS.Text &= "      path: """ & imageData.FileName.Replace("//", "/") & """," & Environment.NewLine
                Me.litAddMediaJS.Text &= "      width:" & libraryImage.Width & "," & Environment.NewLine
                Me.litAddMediaJS.Text &= "      height:" & libraryImage.Height


                Dim iThumbnail As Integer = 0
                Dim i As Integer = 0
                If _Thumbnails.Count > 0 Then
                    Dim sourceFile As String = sPhysicalPath & sFileName
                    Me.litAddMediaJS.Text &= "," & Environment.NewLine
                    Me.litAddMediaJS.Text &= "     Thumbnails: [" & Environment.NewLine
                    For i = 0 To _Thumbnails.Count - 1
                        Me.litAddMediaJS.Text &= "         {"
                        Me.litAddMediaJS.Text &= "             title: """ & _Thumbnails(i).Title & """," & Environment.NewLine
                        Me.litAddMediaJS.Text &= "             imageName: """ & _Thumbnails(i).ImageName & """," & Environment.NewLine
                        Me.litAddMediaJS.Text &= "             path: """ & _Thumbnails(i).Path & """" & Environment.NewLine
                        Me.litAddMediaJS.Text &= "         }"
                        If i <> _Thumbnails.Count - 1 Then
                            Me.litAddMediaJS.Text &= "," & Environment.NewLine
                        End If
                    Next
                    Me.litAddMediaJS.Text &= Environment.NewLine & "] " & Environment.NewLine
                End If

                Me.litAddMediaJS.Text &= "}" & Environment.NewLine
                Me.litAddMediaJS.Text &= "parent.CommerceMediaTabAddLibraryImage(newImageObj);"
                Me.litAddMediaJS.Text &= "</script>"

            Else

                Me.litAddMediaJS.Text &= "<script type=""text/javascript"">" & Environment.NewLine
                Me.litAddMediaJS.Text &= "alert('Images in this folder are private and cannot be added to a catalog');"
                Me.litAddMediaJS.Text &= "</script>"

            End If
            

        Catch ex As Exception
            Dim reason As String = ex.Message
        End Try
    End Sub


#End Region

#Region "Helpers"

    Private Sub WriteToFile(ByVal strPath As String, ByRef Buffer As Byte())
        Try
            ' Create a file
            Dim newFile As FileStream = New FileStream(strPath, FileMode.Create)

            ' Write data to the file
            newFile.Write(Buffer, 0, Buffer.Length)

            ' Close file
            newFile.Close()
        Catch ex As Exception
            Utilities.ShowError("Unable to write file " & strPath)
        End Try
    End Sub

#End Region

End Class
