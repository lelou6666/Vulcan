Imports System
Imports System.Data
Imports System.Configuration
Imports System.Collections
Imports System.IO
Imports System.Text
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D

Imports Ektron.Cms

Partial Class autoupload
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub
    Protected m_refContentApi As New ContentAPI
    Protected settingsCmsLibrary As LibraryConfigData
    Protected currentUserID As Long = 0
    Protected load_balance_data As LoadBalanceData()
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            currentUserID = m_refContentApi.UserId
            Dim uploadType As String
            uploadType = Request.Form("actiontyp").ToString()  ' Tells what this upload is.  This is either "uploadfile" or "uploadcontent"
            Select Case uploadType
                Case "uploadfile"
                    ReceiveSubmittedFiles()
                Case "uploadcontent"
                    ReceiveSubmittedContent()
            End Select
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Sub ReceiveSubmittedContent()
        Dim responseToClient As New System.Text.StringBuilder
        responseToClient.Append("<h2>Automatic Content Upload Not Allowed</h2>")
        responseToClient.Append("<p>The CMS400 is not configured to receive content uploaded through this mechanism.</p>")
        responseToClient.Append("<p>You must use the Publish, Check-In, or Save buttons in the content edit page to save edited content.</p>")
        responseToClient.Append("<p>Click on the 'Undo' button to restore your edited content.</p>")
        Response.Write(responseToClient.ToString)
    End Sub

    Function SaveFileToPath(ByVal filePhysPath As String, ByVal fileName As String, ByVal overwrite As Boolean) As String
        Dim pathSaveFile As String
        Dim counterName As Long
        Dim nameNew As String
        Dim pathUpload As String

        pathUpload = filePhysPath
        Directory.CreateDirectory(pathUpload)  ' ensure that it exists
        pathSaveFile = CatPath(pathUpload, fileName, False)
        If False = overwrite Then
            counterName = 0
            While (File.Exists(pathSaveFile))
                counterName = counterName + 1
                nameNew = Path.GetFileNameWithoutExtension(fileName) & "(" & counterName.ToString() & ")" & Path.GetExtension(fileName)
                pathSaveFile = CatPath(pathUpload, nameNew, False)
            End While
        End If

        ' If it exists, get around overwrite issues
        ' by deleting the file, if it exists.
        If File.Exists(pathSaveFile) Then
            File.Delete(pathSaveFile)
        End If
        Request.Files("uploadfilephoto").SaveAs(pathSaveFile)

        'Return Path.GetFileName(pathSaveFile)
        Return pathSaveFile

    End Function

    Function SimplifyFileName(ByVal nameData As String) As String
        Dim idx As Long
        Dim ch As Integer
        Dim strRet As String = ""
        Const strReplaceChars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_"
        Dim iMax As Long = strReplaceChars.Length

        For idx = 0 To nameData.Length - 1
            ch = AscW(nameData.Chars(idx))
            If ch >= 128 Then
                ' get a good replacement character
                ch = ch - 128
                While ch >= iMax
                    ch -= iMax
                End While
                If (ch < 0) Then ch = 0
                strRet = strRet & strReplaceChars.Chars(ch)
            Else
                strRet = strRet & nameData.Chars(idx)
            End If
        Next

        Return strRet
    End Function

    Function MakeStandardUploadThumbnail(ByVal uploadFilePath As String) As String
        Dim resizeWidth As Integer = 125
        Dim resizeHeight As Integer = 125
        Dim originalFile As String
        Dim changeImagePathFile As String
        Dim img As System.Drawing.Image

        originalFile = uploadFilePath
        changeImagePathFile = CatPath(Path.GetDirectoryName(uploadFilePath), "thumb_" & Path.GetFileName(uploadFilePath), False)

        img = System.Drawing.Image.FromFile(originalFile)

        ' These need to be double for the calculation.
        Dim dOx, dOy, dRx, dRy As Double
        dOx = img.Width
        dOy = img.Height
        dRx = resizeWidth
        dRy = resizeHeight

        ' The greater side is the governing side.
        If (dOx > dOy) Then
            resizeHeight = Convert.ToInt32((dRx * dOy) / dOx)
        Else
            resizeWidth = Convert.ToInt32((dRy * dOx) / dOy)
        End If

        Dim imgCallBack As System.Drawing.Image.GetThumbnailImageAbort
        imgCallBack = New System.Drawing.Image.GetThumbnailImageAbort(AddressOf ImageResizeCallback)

        Dim imgThumbnail As System.Drawing.Image
        imgThumbnail = img.GetThumbnailImage(resizeWidth, resizeHeight, imgCallBack, IntPtr.Zero)

        img.Dispose()  ' Free up the file
        imgThumbnail.Save(changeImagePathFile)

        imgThumbnail.Dispose()

        Return changeImagePathFile
    End Function

    Function ImageResizeCallback() As Boolean
        ' Cool
        Return False
    End Function

    Function CatPath(ByVal strBegin As String, ByVal strEnd As String, ByVal bFinalSlash As Boolean) As String
        Dim strFinalPath As String
        strFinalPath = ""

        ' Normalize
        strBegin = strBegin.Replace("\", "/")
        strBegin = strBegin.Trim()
        strEnd = strEnd.Replace("\", "/")
        strEnd = strEnd.Trim()

        ' This puts them together, ensuring there is at least one slash between
        strFinalPath = strBegin & "/" & strEnd
        If bFinalSlash Then
            strFinalPath = strFinalPath & "/"
        End If

        ' This will fix if there are double slashes together.
        strFinalPath = strFinalPath.Replace("//", "/")
        strFinalPath = strFinalPath.Replace("//", "/")

        Return strFinalPath
    End Function

    Sub ReceiveSubmittedFiles()
        Dim errorMessage As String = ""
        Dim errorValue As Long = 0
        Dim sizeFile As Long
        Dim ErrorCode As Integer = 0
        Dim clientFileName As String = ""
        Dim clientFileExtension As String = ""
        Dim uploadedFilePhysPath As String = ""
        Dim altTitleFile As String = ""
        Dim typeFile As String = ""
        Dim idCmsFolder As Long = 0
        Dim fileExtnList As String()
        Dim i As Integer = 0
        Dim uploadUrlPath As String = ""
        Dim uploadPhysicalPath As String = ""
        Dim strTmpServerInfo As String = ""
        Dim typeInCmsLib As String = ""
        Dim libraryCmsObj As Ektron.Cms.Library.EkLibrary
        Dim ret As Boolean = False
        Dim metaDataList As String = ""
        Dim cMetaColl As New Collection
        Dim strName As String = ""
        Dim lLoop, lInnerLoop As Integer
        Dim teaserStoreForFile As String = ""
        Dim teaserListByFile() As String
        Dim teaserForFile As String
        Dim teaserFileNormalized As String
        Dim tempTeaser As String = ""
        Dim thisTeaser As Boolean
        Dim strJustFileName As String = ""
        Dim clientFilePath As String
        Dim isUploadOK As Boolean

        Err.Clear()

        ' This is the full set of standard fields sent with every upload.
        'lbl_ekactiontypecommand.Text = Request.Form("actiontyp").ToString()  ' Tells what this upload is.  This is either "uploadfile" or "uploadcontent"
        'lbl_ekclientname.Text = Request.Form("ekclientname").ToString()
        'lbl_ekclientversion.Text = Request.Form("ekclientversion").ToString()
        'lbl_ekclientneeds.Text = Request.Form("ekclientneeds").ToString()
        'lbl_img_date.Text = Request.Form("img_date").ToString()
        'lbl_extension_id.Text = Request.Form("extension_id").ToString()
        typeFile = Request.Form("file_type").ToString()
        'lbl_editor_media_path.Text = Request.Form("editor_media_path").ToString()
        'lbl_web_media_path.Text = Request.Form("web_media_path").ToString()
        'lbl_extensions.Text = Request.Form("extensions").ToString()
        sizeFile = Request.Form("file_size").ToString()
        'lbl_width.Text = Request.Form("width").ToString()
        'lbl_height.Text = Request.Form("height").ToString()
        altTitleFile = Request.Form("file_title").ToString()

        clientFilePath = Request.Files("uploadfilephoto").FileName.ToString()
        clientFileName = SimplifyFileName(Path.GetFileName(clientFilePath))

        ' Non-standard (CMS specific) values.
        idCmsFolder = Request.Form("folder_id").ToString()

        'get the metadata, not critical field so do not error out if it does not exist
        metaDataList = Request.Form("custom_field_meta")

        'get the teaser, not critical field so do not error out if it does not exist
        teaserStoreForFile = Request.Form("custom_field_teaser")
        If (teaserStoreForFile Is Nothing) Then
            teaserStoreForFile = ""
        Else
            teaserStoreForFile = teaserStoreForFile.Replace("<p> </p>" & vbCrLf, "<p>&#160;</p>")
        End If

        thisTeaser = False
        teaserListByFile = Split(teaserStoreForFile, "|-|")
        For lLoop = 0 To UBound(teaserListByFile)
            teaserForFile = teaserListByFile(lLoop)
            If thisTeaser = True Then
                tempTeaser = teaserListByFile(lLoop)
                Exit For
            End If
            teaserFileNormalized = Replace(teaserForFile, " ", "_")
            If CStr(teaserFileNormalized) = CStr(clientFileName) Then
                thisTeaser = True
            End If
        Next

        teaserStoreForFile = tempTeaser

        'get the extension from the filename
        clientFileExtension = Path.GetExtension(clientFileName).ToLower().TrimStart("."c)

        'get the library settings- extensions and image paths
        settingsCmsLibrary = m_refContentApi.GetLibrarySettings(idCmsFolder)

        'figure out the fileType
        typeFile = "none"

        'loop through the image extensions 
        fileExtnList = settingsCmsLibrary.ImageExtensions.Split(",")
        For i = 0 To fileExtnList.Length - 1
            If (clientFileExtension = Trim(LCase(fileExtnList(i)))) Then
                typeFile = "images"
                'needed to get correct lb paths
                typeInCmsLib = "images"
                'get correct path- cannot have a slash at the end
                uploadUrlPath = MakeUploadPath(settingsCmsLibrary.ImageDirectory)
                Exit For
            End If
        Next

        'if not an image, loop through the file extensions
        If (typeFile = "none") Then
            fileExtnList = settingsCmsLibrary.FileExtensions.Split(",")
            For i = 0 To fileExtnList.Length - 1
                If (clientFileExtension = Trim(LCase(fileExtnList(i)))) Then
                    typeFile = "files"
                    'needed to get correct lb paths
                    typeInCmsLib = "files"
                    'get correct path- cannot have a slash at the end
                    uploadUrlPath = MakeUploadPath(settingsCmsLibrary.FileDirectory)
                    Exit For
                End If
            Next
        End If

        uploadPhysicalPath = Server.MapPath(uploadUrlPath)

        Dim cPerms As PermissionData

        'if the file is one of the allowed fileTypes, check permissions and continue
        If (typeFile <> "none") Then
            'check the permissions first 
            cPerms = m_refContentApi.LoadPermissions(idCmsFolder, "folder")
            'if the uploader has the correct permission, upload the file	
            If (((typeFile = "images") And (cPerms.CanAddToImageLib)) Or ((typeFile = "files") And (cPerms.CanAddToFileLib))) Then
                'upload the file making it unique if there is already one there
                isUploadOK = False
                uploadedFilePhysPath = SaveFileToPath(uploadPhysicalPath, clientFileName, False)
                If uploadedFilePhysPath.Length > 0 Then
                    strJustFileName = Path.GetFileName(uploadedFilePhysPath)
                    If "images" = typeFile Then
                        If MakeStandardUploadThumbnail(uploadedFilePhysPath).Length = 0 Then
                            errorValue = 4
                            errorMessage = "The server was not able to produce a thumbnail for the file '" & strJustFileName & "''.\n\nThe user may not have write permission to the '" & uploadUrlPath & "' folder on the server or there may not be enough resources on the server to produce a thumbnail.\n\nPlease see your site administrator to resolve the issue."
                        End If
                    End If
                Else
                    errorValue = 3
                    errorMessage = "Error performing the upload onto the server system.\n\nThe user may not have write permission to the '" & uploadUrlPath & "' folder on the server.\n\nPlease see your administrator to modify the server security to allow uploads at this location."
                End If

                If 0 = errorValue Then
                    'if the main file uploaded fine, upload to all the load balance folders
                    'Get the extra load balance paths
                    load_balance_data = m_refContentApi.GetAllLoadBalancePathsExtn(idCmsFolder, typeInCmsLib)

                    'if there are load balance paths
                    If (Not (load_balance_data Is Nothing)) Then
                        Dim pathPhysLoadBalanceUpload As String = ""

                        'loop through each of the paths, uploading the file to those locations
                        For i = 0 To load_balance_data.Length - 1 'Each lbObj In extraPaths
                            isUploadOK = False
                            pathPhysLoadBalanceUpload = Server.MapPath(MakeUploadPath(load_balance_data(i).Path))
                            'LBString = m_refEkFile.EkFileSave2(BinaryFormData, "uploadfilephoto", Server.MapPath(pathPhysLoadBalanceUpload), LBErrCode, "overwrite", uploadedFilePhysPath)
                            If SaveFileToPath(pathPhysLoadBalanceUpload, strJustFileName, True).Length > 0 Then
                                If "images" = typeFile Then
                                    'ThumbLBString = m_refEkFile.EkFileSave2(BinaryFormData, "uploadfilephoto", Server.MapPath(pathPhysLoadBalanceUpload), LBErrCode, "overwrite", "\thumb_" & uploadedFilePhysPath)
                                    isUploadOK = (MakeStandardUploadThumbnail(CatPath(pathPhysLoadBalanceUpload, strJustFileName, False)).Length > 0)
                                Else
                                    isUploadOK = True
                                End If
                            End If
                            If False = isUploadOK Then
                                'If the load balance upload fails then notify the admin and keep going
                                Dim contObj As Object
                                contObj = m_refContentApi.EkContentRef()
                                Dim eTmp As String = "An upload failure has occured in the load balancing.  Load balancing path = '" & pathPhysLoadBalanceUpload & "'.  Uploaded Filename  = '" & CatPath(pathPhysLoadBalanceUpload, strJustFileName, False) & "'."
                                ret = contObj.ErrStatusToAdminGroup(eTmp, "Load Balance Error Report")
                            End If
                        Next
                    End If

                    'create an object with appropriate info to send to assembly to insert into the database
                    Dim cLibrary As New Collection
                    'ParentID should be folderID that the file will be inserted into
                    cLibrary.Add(idCmsFolder, "ParentID")
                    'LibraryID should be blank
                    cLibrary.Add("", "LibraryID")
                    'LibraryType is fileType
                    cLibrary.Add(typeFile, "LibraryType")
                    'LibraryTitle is going to be the name of the file
                    If altTitleFile.Length >= 150 Then
                        cLibrary.Add(altTitleFile.Substring(0, 150), "LibraryTitle")
                    Else
                        cLibrary.Add(altTitleFile, "LibraryTitle")
                    End If
                    'ContentID is going to be blank
                    cLibrary.Add("", "ContentID")
                    'UserID should be the currentUserID
                    cLibrary.Add(currentUserID, "UserID")
                    'The LibraryFilename should be the path with the slash added back and the new filename
                    cLibrary.Add(CatPath(uploadUrlPath, strJustFileName, False), "LibraryFilename")
                    'The NameConflict should be to unique for autouploads
                    cLibrary.Add("makeunique", "TitleConflict")

                    cLibrary.Add(teaserStoreForFile, "ContentTeaser")

                    If (Not (metaDataList Is Nothing)) Then
                        Dim isData As Boolean = False
                        Dim metaThis As Boolean = False
                        Dim countMeta As Integer = 1
                        Dim metaByFileList() As String
                        Dim strMetaByFile As String
                        Dim metaItemList(3) As String
                        Dim strMeta As String
                        Dim metaListFromByFile() As String

                        metaByFileList = Split(metaDataList, "|-|")
                        For lLoop = 0 To UBound(metaByFileList)
                            strMetaByFile = metaByFileList(lLoop)
                            If (metaThis = True) Then
                                metaThis = False
                                metaListFromByFile = Split(strMetaByFile, "@@ekt@@")
                                For lInnerLoop = 0 To UBound(metaListFromByFile)
                                    strMeta = metaListFromByFile(lInnerLoop)
                                    If (isData = True) Then
                                        isData = False
                                        metaItemList(1) = strName
                                        metaItemList(2) = 0
                                        metaItemList(3) = strMeta
                                        cMetaColl.Add(metaItemList, countMeta)
                                        ReDim metaItemList(3)
                                        countMeta = countMeta + 1
                                    ElseIf (strMeta <> "") Then
                                        isData = True
                                        strName = strMeta
                                    End If
                                Next
                            End If
                            teaserFileNormalized = Replace(strMetaByFile, " ", "_")
                            If (CStr(teaserFileNormalized) = CStr(clientFileName)) Then
                                metaThis = True
                            End If
                        Next
                    End If
                    cLibrary.Add(cMetaColl, "ContentMetadata")

                    libraryCmsObj = m_refContentApi.EkLibraryRef
                    ret = libraryCmsObj.AddLibraryItemv2_0(cLibrary)

                    If True = CBool(Request.ServerVariables("SERVER_PORT_SECURE")) Then
                        strTmpServerInfo = "https://"
                    Else
                        strTmpServerInfo = "http://"
                    End If
                    strTmpServerInfo = strTmpServerInfo & Request.ServerVariables("SERVER_NAME")
                    If (Request.ServerVariables("SERVER_PORT") <> "80") Then
                        strTmpServerInfo = strTmpServerInfo & ":" & Request.ServerVariables("SERVER_PORT")
                    End If

                End If  ' 0 = errorValue

            Else
                'permissions error
                'throw error
                errorValue = 1
                errorMessage = "User does not have CMS permeissions to place files into the selected folder.\n\nPlease see your site administrator for modifying the permissions to allow this operation."
                Exit Sub
            End If  ' (((typeFile = "images") And (cPerms.CanAddToImageLib)) Or ((typeFile = "files") And (cPerms.CanAddToFileLib)))
        Else
            errorValue = 2
            errorMessage = "Invalid extension for the file " & clientFileName & ".\n\nPlease select a valid file type or contact your administrator to add this type to the list of valid files allowed in the upload process."
        End If  ' (typeFile <> "none")

        Dim rData As New System.Text.StringBuilder
        rData.Append("<XML ID=EktronFileIO>" & vbCrLf)
        rData.Append("<UPLOAD>" & vbCrLf)
        rData.Append("<FILEINFO ID=""5"" discard=""False"">" & vbCrLf)
        rData.Append("<FSRC>" & clientFilePath & "</FSRC>" & vbCrLf)
        If 0 = errorValue Then
            rData.Append("<FURL>" & strTmpServerInfo & CatPath(uploadUrlPath, strJustFileName, False) & "</FURL>" & vbCrLf)
        Else
            rData.Append("<FURL></FURL>" & vbCrLf)
        End If
        rData.Append("<FID></FID>" & vbCrLf)
        rData.Append("<FSIZE>" & CLng("&h" & sizeFile) & "</FSIZE>" & vbCrLf)
        rData.Append("<DESC>" & altTitleFile & "</DESC>" & vbCrLf)
        rData.Append("<THUMBURL></THUMBURL>" & vbCrLf)
        rData.Append("<THUMBHREF></THUMBHREF>" & vbCrLf)
        rData.Append("<FTYPE>" & typeFile & "</FTYPE>" & vbCrLf)
        rData.Append("<DWIDTH>0</DWIDTH>" & vbCrLf)
        rData.Append("<DHEIGHT>0</DHEIGHT>" & vbCrLf)
        rData.Append("<DBORDER>0</DBORDER>" & vbCrLf)
        rData.Append("<FRAGMENT></FRAGMENT>" & vbCrLf)
        If 0 = errorValue Then
            rData.Append("<FERROR value=""0""></FERROR>" & vbCrLf)
        Else
            rData.Append("<FERROR value=""" & errorValue.ToString & """>" & errorMessage & "</FERROR>" & vbCrLf)
        End If
        rData.Append("</FILEINFO>" & vbCrLf)
        rData.Append("</UPLOAD>" & vbCrLf)
        rData.Append("</XML>" & vbCrLf)
        Response.Write(rData.ToString)

    End Sub
    Private Function EnsureFilePathExists(ByVal path As String) As Boolean
        ' A return of True is an error, to be consistent
        Dim bRet As Boolean
        Try
            Dim strNewPath As String
            Dim objSyst As System.IO.DirectoryInfo

            strNewPath = path.Substring(0, path.LastIndexOf("\"))

            If strNewPath.Length > 0 Then
                objSyst = New System.IO.DirectoryInfo(strNewPath)
                If False = objSyst.Exists() Then
                    objSyst.Create()
                End If
                objSyst = Nothing
            End If
        Catch
            bRet = True
        End Try
        Return bRet
    End Function
    Private Function MakeUploadPath(ByVal path As String) As String
        If (Right(path, 1) = "/") Then
            MakeUploadPath = Mid(path, 1, Len(path) - 1)
        Else
            MakeUploadPath = path
        End If
    End Function
    Private Function ReportError(ByVal ErrorCode As String, ByVal ErrorDescription As String) As String
        Dim err As New System.Text.StringBuilder
        err.Append("<XML ID=EktronFileIO>" & vbCrLf)
        err.Append("<?xml version=""1.0""?>" & vbCrLf)
        err.Append("<UPLOAD>" & vbCrLf)
        err.Append("<FILEINFO ID=""5"" discard=""False"">" & vbCrLf)
        err.Append("<FSRC></FSRC>" & vbCrLf)
        err.Append("<FURL></FURL>" & vbCrLf)
        err.Append("<FID></FID>" & vbCrLf)
        err.Append("<FSIZE></FSIZE>" & vbCrLf)
        err.Append("<DESC></DESC>" & vbCrLf)
        err.Append("<THUMBURL></THUMBURL>" & vbCrLf)
        err.Append("<THUMBHREF></THUMBHREF>" & vbCrLf)
        err.Append("<FTYPE></FTYPE>" & vbCrLf)
        err.Append("<DWIDTH>0</DWIDTH>" & vbCrLf)
        err.Append("<DHEIGHT>0</DHEIGHT>" & vbCrLf)
        err.Append("<DBORDER>0</DBORDER>" & vbCrLf)
        err.Append("<FRAGMENT></FRAGMENT>" & vbCrLf)
        err.Append("<FERROR value=""" & ErrorCode & """>" & ErrorDescription & "</FERROR>" & vbCrLf)
        err.Append("</FILEINFO>" & vbCrLf)
        err.Append("</UPLOAD>" & vbCrLf)
        err.Append("</XML>" & vbCrLf)
        Return (err.ToString)
    End Function

    Private Function GetLocalFilename(ByRef LocalDirectory As Object, ByRef strFilename As Object, ByVal strHandleConflict As String, ByRef ErrorCode As Object) As String
        Dim strLocalFilename As String
        Dim strTmpFilename() As String
        Dim strSplitFilename() As String
        Dim iUnqueNameIdentifier As Integer
        Dim strExtension As String
        Dim strTmpLocalDir As String = ""
        Try
            ErrorCode = 0

            strFilename = Replace(strFilename, "/", "\")
            strTmpFilename = Split(strFilename, "\")
            If (Right$(LocalDirectory, 1) <> "\") Then
                strTmpLocalDir = LocalDirectory & "\"
            End If

            strLocalFilename = strTmpLocalDir & UnicodeEncode(strTmpFilename(UBound(strTmpFilename)))

            If (strHandleConflict = "overwrite") Then
                If (FileExists(strLocalFilename)) Then
                    Kill(strLocalFilename)
                End If
                GetLocalFilename = strLocalFilename
            ElseIf (strHandleConflict = "makeunique") Then
                iUnqueNameIdentifier = 1
                If (FileExists(strLocalFilename)) Then
                    strSplitFilename = Split(strTmpFilename(UBound(strTmpFilename)), ".")
                    If (UBound(strSplitFilename) > 0) Then
                        strSplitFilename(0) = Left$(strTmpFilename(UBound(strTmpFilename)), Len(strTmpFilename(UBound(strTmpFilename))) - (Len(strSplitFilename(UBound(strSplitFilename))) + 1))
                        strExtension = strSplitFilename(UBound(strSplitFilename))
                    Else
                        strExtension = ""
                    End If
                    strLocalFilename = strTmpLocalDir & strSplitFilename(0) _
                                                & "(" & iUnqueNameIdentifier & ")" & "." & strExtension
                    While (FileExists(strLocalFilename))
                        iUnqueNameIdentifier = iUnqueNameIdentifier + 1
                        strLocalFilename = strTmpLocalDir & strSplitFilename(0) _
                                                & "(" & iUnqueNameIdentifier & ")" & "." & strExtension
                    End While
                End If
                GetLocalFilename = strLocalFilename
            Else
                GetLocalFilename = strLocalFilename
            End If
            Exit Function

        Catch ex As Exception
            ErrorCode = Err.Number
            GetLocalFilename = Err.Description
        End Try
    End Function

    Private Function FileExists(ByVal Filename As String) As Boolean
        Try
            FileExists = (FileLen(Filename) >= 0)
        Catch ex As Exception
            FileExists = False
        End Try
    End Function

    Private Function UnicodeEncode(ByVal strText As String) As String
        UnicodeEncode = strText
        If Len(strText) = 0 Then Exit Function
        Dim strEncoded As String : strEncoded = ""
        Dim strChar As String
        Dim P As Long
        For P = 1 To Len(strText)
            strChar = Mid$(strText, P, 1)
            Select Case strChar
                Case "A" To "Z", _
                  "a" To "z", _
                  "0" To "9", _
                  "$", "-", "_", ".", "+", "!", "*", "'", "(", ")"
                    ' no change
                    strEncoded = strEncoded & strChar
                Case Else
                    strChar = Hex(AscW(strChar))
                    strEncoded = strEncoded & strChar
            End Select
        Next
        UnicodeEncode = strEncoded
    End Function
End Class
