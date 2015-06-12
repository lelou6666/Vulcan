Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Workarea
Imports System.IO

Namespace Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Media

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

    Partial Class AddImage
        Inherits System.Web.UI.Page

#Region "Variables"

        Protected _Id As Long = 0
        Protected _ContentApi As ContentAPI = New ContentAPI()
        Protected _MessageHelper As EkMessageHelper = _ContentApi.EkMsgRef
        Protected _ProductType As New ProductType(_ContentApi.RequestInformationRef)
        Protected _ProductTypeData As New ProductTypeData
        Protected lib_settings_data As New Ektron.Cms.LibraryConfigData
        Protected _Thumbnails As New List(Of ThumbnailData)
        Protected _CatalogEntryApi As New Ektron.Cms.Commerce.CatalogEntryApi
        Protected _ProductData As New ProductData

#End Region

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Response.CacheControl = "no-cache"
            Response.AddHeader("Pragma", "no-cache")
            Response.Expires = -1

            If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
                Utilities.ShowError(_ContentApi.EkMsgRef.GetMessage("feature locked error"))
            End If

            If Request.QueryString("productTypeId") <> "" Then
                _Id = Request.QueryString("productTypeId")
            End If

            lib_settings_data = Me._ContentApi.GetLibrarySettings(0)

            SetLocalizedStrings()

            btnUpload.Attributes.Add("onclick", "return checkForEmptyTitleAndAlt(); return checkntoggle(document.getElementById('dvHoldMessage'),document.getElementById('dvErrorMessage'));")
            btnUpload.Text = Me.GetMessage("upload txt")

            _ProductTypeData = _ProductType.GetItem(_Id)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            CheckAccess()

            If Not Page.IsPostBack Then
                If _ProductTypeData.DefaultThumbnails.Count > 0 Then
                    rptThumbnails.DataSource = _ProductTypeData.DefaultThumbnails
                    rptThumbnails.DataBind()
                Else
                    phThumbnails.Visible = False
                End If
            End If
        End Sub

        Private Sub SetLocalizedStrings()
            lblTitle.Text = Me.GetMessage("generic title label")
            lblAlt.Text = Me.GetMessage("alt text")
            lblImage.Text = Me.GetMessage("lbl full size")
            legendImage.Text = "Image"
            legendThumbnails.Text = "Thumbails"
            litImageHeader.Text = "Enter title, alt text, and file path in the table below."
        End Sub

        Public Function GetMessage(ByVal MessageTitle As String) As String
            Return _MessageHelper.GetMessage(MessageTitle)
        End Function

        Private Sub CheckAccess()
            Dim loggedIn As Boolean = _ContentApi.LoadPermissions(0, "folder").IsLoggedIn
            If (Not loggedIn) OrElse (loggedIn And _ContentApi.RequestInformationRef.IsMembershipUser) Then
                'user is not logged-in or is logged-in as a Membership User
                mvAddImage.SetActiveView(vwError)
            Else
                'user is logged-in: show form
                mvAddImage.SetActiveView(vwForm)
            End If
        End Sub

        Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpload.Click
            Dim parentId As String = ""
            If Request.QueryString("catalogid") IsNot Nothing AndAlso Request.QueryString("catalogid") <> "" Then
                parentId = Request.QueryString("catalogid")
            End If

            If txtTitle.Text.IndexOfAny(New Char() {"<", ">"}) > -1 Then Throw New Ektron.Cms.Exceptions.SpecialCharactersException()
            If txtAlt.Text.IndexOfAny(New Char() {"<", ">"}) > -1 Then Throw New Ektron.Cms.Exceptions.SpecialCharactersException()

            CheckAccess()

            Try
                If Not (fuImage.PostedFile Is Nothing) Then
                    ' file was sent
                    Dim myFile As HttpPostedFile = fuImage.PostedFile
                    Dim sFileExt As String = ""

                    ' Get and check size of uploaded file
                    Dim nFileLen As Integer = myFile.ContentLength

                    'get and check name and extension
                    Dim sFileName As String = myFile.FileName

                    Dim sShortName As String = ""
                    sFileName = sFileName.Replace("%", "")
                    If sFileName.IndexOf("\") Then
                        Dim aFilename As String() = Split(sFileName, "\")
                        ' take the very last one
                        If aFilename.Length > 0 Then
                            sFileName = aFilename(aFilename.Length - 1)
                        End If
                    End If

                    'make safe
                    sFileName = Replace(sFileName.Replace(" ", "_"), "'", "")

                    Dim aFileExt As String() = Split(sFileName, ".")
                    If aFileExt.Length > 1 Then
                        sFileExt = aFileExt((aFileExt.Length - 1)).Trim().ToLower() 'use the LAASSTT one.
                        If sFileExt = "tif" Or sFileExt = "bmp" Then
                            Throw New Exception("The extension """ & sFileExt & """ is not allowed.")
                        End If
                        sShortName = sFileName.Substring(0, (sFileName.Length - (sFileExt.Length + 1)))
                    Else
                        Throw New Exception("The extension """ & sFileExt & """ is not allowed.")
                    End If

                    If aFileExt.Length > 0 Then
                        Dim bGo As Boolean = False
                        For i As Integer = 0 To (aFileExt.Length - 1)
                            If sFileExt = aFileExt(i).Trim().ToLower() Then
                                bGo = True
                                Exit For
                            End If
                        Next
                        If bGo = False Then
                            Throw New Exception("The extension """ & sFileExt & """ is not allowed.")
                        End If
                    Else
                        Throw New Exception("The extension """ & sFileExt & """ is not allowed.")
                    End If

                    ' Allocate a buffer for reading of the file
                    Dim myData(nFileLen) As Byte

                    ' Read uploaded file from the Stream
                    myFile.InputStream.Read(myData, 0, nFileLen)

                    'check for existence of file.
                    Dim CheckFile As FileInfo
                    Dim iUnqueNameIdentifier As Integer = 0
                    Dim uploadImagePath As String = _ContentApi.GetPathByFolderID(parentId)
                    Dim sWebPath As String = lib_settings_data.ImageDirectory.TrimEnd("/") & uploadImagePath.Replace("\", "/").Replace(" ", "_").Replace(".", "").TrimEnd("/")
                    Dim sPhysicalPath As String = Server.MapPath(sWebPath)

                    If Not Directory.Exists(sPhysicalPath) Then Directory.CreateDirectory(sPhysicalPath)

                    CheckFile = New FileInfo(sPhysicalPath & "\" & sFileName)
                    If (CheckFile.Exists) Then
                        Do While (CheckFile.Exists)
                            iUnqueNameIdentifier = iUnqueNameIdentifier + 1
                            sFileName = sShortName & "(" & iUnqueNameIdentifier & ")." & sFileExt
                            CheckFile = New FileInfo(sPhysicalPath & sFileName)
                        Loop
                    End If

                    'write
                    WriteToFile(sPhysicalPath & "\" & sFileName, myData)
                    Utilities.ProcessThumbnail(sPhysicalPath, sFileName)

                    'Begins: Generate thumbnails. Generates thumbnails for various pixes sizes.
                    If _ProductTypeData.DefaultThumbnails.Count > 0 Then
                        Dim thumbnailCreator As New EkFileIO
                        Dim thumbnailResult As Boolean = False
                        Dim sourceFile As String = sPhysicalPath & "\" & sFileName
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

                    'upload this file to library.
                    Dim librarydata As New LibraryData
                    Dim libraryId As Long = 0
                    librarydata.FileName = (sWebPath.Replace("/\", "\") + "/" + sFileName).Replace(" ", "_")
                    librarydata.Type = "images"
                    If txtTitle.Text = "" Then
                        librarydata.Title = sShortName
                    Else
                        librarydata.Title = txtTitle.Text
                    End If
                    librarydata.ParentId = parentId
                    libraryId = _ContentApi.AddLibraryItem(librarydata)

                    Dim retLibraryData As LibraryData = _ContentApi.GetLibraryItemByID(libraryId, parentId)

                    'Uploading image to libray ends.
                    Dim imageUpload As Drawing.Bitmap
                    imageUpload = New Drawing.Bitmap(sPhysicalPath + "\" + sFileName)

                    ' Add media image                
                    ltrAddMediaJS.Text &= "var newImageObj = {"
                    ltrAddMediaJS.Text &= "        id: """ & libraryId.ToString() & ""","
                    ltrAddMediaJS.Text &= "        title: """ & retLibraryData.Title & ""","
                    ltrAddMediaJS.Text &= "        altText: """ & txtAlt.Text & ""","
                    ltrAddMediaJS.Text &= "        path: """ & retLibraryData.FileName & ""","
                    ltrAddMediaJS.Text &= "        width:" & imageUpload.Width & ","
                    ltrAddMediaJS.Text &= "        height:" & imageUpload.Height


                    Dim iThumbnail As Integer = 0
                    Dim x As Integer = 0
                    If _Thumbnails.Count > 0 Then
                        Dim sourceFile As String = sPhysicalPath & sFileName
                        ltrAddMediaJS.Text &= ","
                        ltrAddMediaJS.Text &= "Thumbnails: ["
                        For x = 0 To _Thumbnails.Count - 1
                            Dim thumbnail As ThumbnailData = _Thumbnails(x)
                            Dim fileNameNoExtension As String = sFileName.Replace(System.IO.Path.GetExtension(sFileName), "")
                            Dim fileNameExtension As String = System.IO.Path.GetExtension(sFileName)
                            Dim thumbnailPath As String = retLibraryData.FileName.Replace(System.IO.Path.GetFileName(retLibraryData.FileName), "")
                            Dim thumbnailFile As String = fileNameNoExtension & Replace(thumbnail.Title, "[filename]", "") & fileNameExtension

                            ltrAddMediaJS.Text &= "{"
                            ltrAddMediaJS.Text &= "     title: """ & thumbnail.Title & ""","
                            ltrAddMediaJS.Text &= "     imageName: """ & thumbnail.ImageName & ""","
                            ltrAddMediaJS.Text &= "     path: """ & thumbnail.Path & """"
                            ltrAddMediaJS.Text &= "}" & Environment.NewLine
                            If x <> _Thumbnails.Count - 1 Then
                                ltrAddMediaJS.Text &= "," & Environment.NewLine
                            End If

                        Next

                        ltrAddMediaJS.Text &= "] " & Environment.NewLine

                    End If

                    ltrAddMediaJS.Text &= "  }" & Environment.NewLine
                    ltrAddMediaJS.Text &= "parent.Ektron.Commerce.MediaTab.Images.addNewImage(newImageObj);"

                    '766 LOAD BALANCING HANDLED BY SERVICE
                    ''----------------- Load Balance ------------------------------------------------------
                    'Dim loadbalance_data As LoadBalanceData()
                    'loadbalance_data = _ContentApi.GetAllLoadBalancePathsExtn(parentId, "images")
                    'If (Not (IsNothing(loadbalance_data))) Then
                    '    For j As Integer = 0 To loadbalance_data.Length - 1
                    '        sPhysicalPath = Server.MapPath(loadbalance_data(j).Path)
                    '        If (Right(sPhysicalPath, 1) <> "\") Then
                    '            sPhysicalPath = sPhysicalPath & "\"
                    '        End If
                    '        WriteToFile(sPhysicalPath & "\" & sFileName, myData)
                    '    Next
                    '    'Begins: Generate thumbnails. Generates thumbnails for various pixes sizes.
                    '    If _ProductTypeData.DefaultThumbnails.Count > 0 Then
                    '        Dim thumbnailCreator As New EkFileIO
                    '        Dim thumbnailResult As Boolean = False
                    '        Dim sourceFile As String = sPhysicalPath & "\" & sFileName
                    '        For Each thumbnail As ThumbnailDefaultData In _ProductTypeData.DefaultThumbnails
                    '            Dim fileNameNoExtension As String = sFileName.Replace(System.IO.Path.GetExtension(sFileName), "")
                    '            Dim fileNameExtension As String = System.IO.Path.GetExtension(sFileName)
                    '            Dim thumbnailFile As String = sPhysicalPath & "\" & fileNameNoExtension & Replace(thumbnail.Title, "[filename]", "") & fileNameExtension
                    '            thumbnailResult = thumbnailCreator.CreateThumbnail(sourceFile, thumbnailFile, thumbnail.Width, thumbnail.Height)
                    '        Next
                    '    End If
                    '    'Ends : Generate Thumbnails.
                    'End If
                Else
                    Throw New Exception("No File")
                End If
            Catch ex As Exception
                litError.Text = ex.Message
                ltrErrorJS.Text &= "justtoggle(document.getElementById('dvErrorMessage'), true);" & Environment.NewLine
            End Try
        End Sub

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
    End Class

End Namespace