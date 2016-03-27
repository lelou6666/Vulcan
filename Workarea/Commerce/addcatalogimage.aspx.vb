Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Workarea
Imports System.IO

Partial Class Commerce_addcatalogimage
    Inherits workareabase

#Region "Variables"
    Protected iboardid As Integer = 0
    Protected _board As DiscussionBoard
    Protected bError As Boolean = False
    Protected productTypeManager As New ProductType(m_refContentApi.RequestInformationRef)
    Protected productType As New ProductTypeData
    Protected lib_settings_data As New Ektron.Cms.LibraryConfigData
#End Region
    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        lib_settings_data = Me.m_refContentApi.GetLibrarySettings(iboardid)
        If Not Page.IsPostBack Then
            CheckAccess()
            productType = productTypeManager.GetItem(m_iID)

            lblTitle.InnerText = m_refMsg.GetMessage("generic title label")
            lblaltTitle.InnerText = m_refMsg.GetMessage("alt text")
            lblFullSize.InnerText = m_refMsg.GetMessage("lbl full size")

            If productType.DefaultThumbnails.Count > 0 Then
                ltr_pixel.Text = "<br /><table><tr><td>" & m_refMsg.GetMessage("lbl thumbnail spec") & "</td></tr>"
                For Each thumbnail As ThumbnailDefaultData In productType.DefaultThumbnails
                    ltr_pixel.Text += "<tr><td>&nbsp;&nbsp;" & thumbnail.Width.ToString() & " x " & thumbnail.Height.ToString() & " px</td></tr>"
                Next
                ltr_pixel.Text += "</table>"
            End If
            btnUpload.Attributes.Add("onclick", " return checkForEmptyTitleAndAlt(); return IsExtensionValid(); return checkntoggle(document.getElementById('dvHoldMessage'),document.getElementById('dvErrorMessage'));")
            btnUpload.Text = m_refMsg.GetMessage("upload txt")
        End If
        GenerateJS()
    End Sub
    Private Sub CheckAccess()
        Dim loggedIn As Boolean = m_refContentApi.LoadPermissions(0, "folder").IsLoggedIn
        If (Not loggedIn) OrElse (loggedIn And m_refContentApi.RequestInformationRef.IsMembershipUser) Then
            dvPage.Visible = False
            Response.Write("Please login as a cms user.")
            Exit Sub
        End If
    End Sub
    Private Sub GenerateJS()
        Dim sbJS As New StringBuilder()

        sbJS.Append("<script language=""javaScript"" type=""text/javascript"">").Append(Environment.NewLine)

        sbJS.Append("	function justtoggle(eElem, toshow){").Append(Environment.NewLine)
        sbJS.Append("	    if (toshow == true) {eElem.style.visibility = ""visible"";}").Append(Environment.NewLine)
        sbJS.Append("	    else {eElem.style.visibility=""hidden""; }").Append(Environment.NewLine)
        sbJS.Append("	}").Append(Environment.NewLine)

        sbJS.Append("	function checkntoggle(me, you){").Append(Environment.NewLine)
        sbJS.Append("       if (!checkForEmptyTitleAndAlt())").Append(Environment.NewLine)
        sbJS.Append("       {").Append(Environment.NewLine)
        sbJS.Append("           return false;").Append(Environment.NewLine)
        sbJS.Append("       }").Append(Environment.NewLine)
        sbJS.Append("		var bProceed = false; ").Append(Environment.NewLine)
        sbJS.Append("		var ofile = document.getElementById('" & ul_image.UniqueID & "'); ").Append(Environment.NewLine)
        sbJS.Append("		if ( (ofile.type == 'file') && (ofile.value != '') ) { ").Append(Environment.NewLine)
        sbJS.Append("		    bProceed = true; ").Append(Environment.NewLine)
        sbJS.Append("		} ").Append(Environment.NewLine)
        sbJS.Append("		if (bProceed){").Append(Environment.NewLine)
        sbJS.Append("			me.style.visibility=""visible"";").Append(Environment.NewLine)
        sbJS.Append("			you.style.visibility=""hidden"";").Append(Environment.NewLine)
        sbJS.Append("			}").Append(Environment.NewLine)
        sbJS.Append("		else {").Append(Environment.NewLine)
        sbJS.Append("			me.style.visibility=""hidden"";").Append(Environment.NewLine)
        sbJS.Append("			you.style.visibility=""visible"";").Append(Environment.NewLine)
        sbJS.Append("			alert('File not selected.');").Append(Environment.NewLine)
        sbJS.Append("	    }").Append(Environment.NewLine)
        sbJS.Append("		return bProceed;").Append(Environment.NewLine)
        sbJS.Append("	}").Append(Environment.NewLine)

        sbJS.Append("   function checkForEmptyTitleAndAlt()").Append(Environment.NewLine)
        sbJS.Append("   {").Append(Environment.NewLine)
        sbJS.Append("       var title = document.getElementById('txtTitle');").Append(Environment.NewLine)
        sbJS.Append("       var alt = document.getElementById('altTitle');").Append(Environment.NewLine)
        sbJS.Append("       if(title.value == '' || alt.value == '')").Append(Environment.NewLine)
        sbJS.Append("       {").Append(Environment.NewLine)
        sbJS.Append("           alert('").Append(GetMessage("js alert title alt not empty")).Append("');").Append(Environment.NewLine)
        sbJS.Append("           return false;").Append(Environment.NewLine)
        sbJS.Append("       }").Append(Environment.NewLine)
        sbJS.Append("       else").Append(Environment.NewLine)
        sbJS.Append("       {").Append(Environment.NewLine)
        sbJS.Append("           return true;").Append(Environment.NewLine)
        sbJS.Append("       }").Append(Environment.NewLine)
        sbJS.Append("   }").Append(Environment.NewLine)

        sbJS.Append("</script>").Append(Environment.NewLine)
        ltr_topjs.text = sbJS.ToString()
        'Dim ltr_de_js As New LiteralControl(sbJS.ToString())
        'If Not (Page.Header Is Nothing) Then
        '    Page.Header.Controls.Add(ltr_de_js)
        'End If
    End Sub

    Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpload.Click
        Dim parentId As String = ""
        If Request.QueryString("catalogid") IsNot Nothing AndAlso Request.QueryString("catalogid") <> "" Then
            parentId = Request.QueryString("catalogid")
        End If

        CheckAccess()
        Try
            If Not (ul_image.PostedFile Is Nothing) Then
                productType = productTypeManager.GetItem(m_iID)

                Dim iFolder As Integer = iboardid
                'lib_settings_data = Me.m_refContentApi.GetLibrarySettings(iFolder)

                ' file was sent
                Dim myFile As HttpPostedFile = ul_image.PostedFile
                Dim sFileExt As String = ""
                ' Get and check size of uploaded file
                Dim nFileLen As Integer = myFile.ContentLength
                'If nFileLen > _board.MaxFileSize Then
                '    Throw New Exception("File is too large. There is a " & _board.MaxFileSize.ToString() & " byte limit.")
                'End If
                'get and check name and extension
                Dim sFileName As String = myFile.FileName
                Dim sShortName As String = ""
                If myFile.FileName.IndexOf("\") Then
                    Dim aFilename As String() = Split(myFile.FileName, "\")
                    ' take the very last one
                    If aFilename.Length > 0 Then
                        sFileName = aFilename(aFilename.Length - 1)
                    End If
                End If
                sFileName = Replace(sFileName.Replace(" ", "_"), "'", "") ' make safe
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
                'aFileExt = Split(_board.AcceptedExtensions, ",")
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
                Dim uploadImagePath As String = m_refContentApi.GetPathByFolderID(parentId)
                Dim sWebPath As String = lib_settings_data.ImageDirectory & uploadImagePath.Replace(" ", "_").Replace(".", "")
                Dim sPhysicalPath As String = Server.MapPath(sWebPath)
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
                '----------------- Load Balance ------------------------------------------------------
                Dim loadbalance_data As LoadBalanceData()
                loadbalance_data = MyBase.m_refContentApi.GetAllLoadBalancePathsExtn(iFolder, "files")
                If (Not (IsNothing(loadbalance_data))) Then
                    For j As Integer = 0 To loadbalance_data.Length - 1
                        sPhysicalPath = Server.MapPath(loadbalance_data(j).Path)
                        If (Right(sPhysicalPath, 1) <> "\") Then
                            sPhysicalPath = sPhysicalPath & "\"
                        End If
                        WriteToFile(sPhysicalPath & "\" & sFileName, myData)
                    Next
                End If

                'Begins: Generate thumbnails. Generates thumbnails for various pixes sizes.
                If productType.DefaultThumbnails.Count > 0 Then
                    For Each thumbnail As ThumbnailDefaultData In productType.DefaultThumbnails
                        Utilities.ProcessThumbnail(sPhysicalPath, sFileName, thumbnail.Width, thumbnail.Height, thumbnail.Width)
                        ' Utilities.ProcessThumbnail(sPhysicalPath, thumbnail.FileNameFormat.Replace("[filename]", sFileName), thumbnail.Width, thumbnail.Height, thumbnail.Width)
                    Next
                End If
                'Ends : Generate Thumbnails.

                'upload this file to library.
                Dim librarydata As New LibraryData
                Dim libraryId As Long = 0
                librarydata.FileName = (sWebPath.Replace("/\", "\") + "\" + sFileName).Replace(" ", "_")
                librarydata.Type = "images"
                If txtTitle.Value = "" Then
                    librarydata.Title = sShortName
                Else
                    librarydata.Title = txtTitle.Value
                End If
                librarydata.ParentId = parentId
                libraryId = m_refContentApi.AddLibraryItem(librarydata)

                Dim retLibraryData As LibraryData = m_refContentApi.GetLibraryItemByID(libraryId, parentId)
                'Uploading image to libray ends.
                Dim imageUpload As Drawing.Bitmap
                imageUpload = New Drawing.Bitmap(sPhysicalPath + "\" + sFileName)
                ' Add media image                
                ltrAddMediaJS.Text &= "var newImageObj = {"
                ltrAddMediaJS.Text &= "        id: """ & libraryId.ToString() & """, " & Environment.NewLine
                ltrAddMediaJS.Text &= "        title: """ & retLibraryData.Title & """, " & Environment.NewLine
                ltrAddMediaJS.Text &= "        altText: """ & altTitle.Value & """, " & Environment.NewLine
                ltrAddMediaJS.Text &= "        path: """ & retLibraryData.FileName & """, " & Environment.NewLine
                ltrAddMediaJS.Text &= "        width:" & imageUpload.Width & "," & Environment.NewLine
                ltrAddMediaJS.Text &= "        height:" & imageUpload.Height & "," & Environment.NewLine
                ltrAddMediaJS.Text &= "        Thumbnails: [" & Environment.NewLine

                Dim iThumbnail As Integer = 0

                For iThumbnail = 0 To productType.DefaultThumbnails.Count - 1
                    Dim indexThumbnail As Integer = retLibraryData.FileName.LastIndexOf("/")
                    Dim thumbnailpath As String = retLibraryData.FileName.Substring(0, indexThumbnail + 1)
                    ltrAddMediaJS.Text &= "              {path: """ & thumbnailpath & "thumb" & productType.DefaultThumbnails.Item(iThumbnail).Width & "_" & sFileName & """}," & Environment.NewLine
                Next
                ltrAddMediaJS.Text &= "        ] " & Environment.NewLine
                ltrAddMediaJS.Text &= "  }; " & Environment.NewLine

                ltrAddMediaJS.Text &= "parent.Ektron.Commerce.MediaTab.Images.addNewImage(newImageObj);"

            Else
                Throw New Exception("No File")
            End If
        Catch ex As Exception
            ltr_error.Text = ex.Message
            ltr_bottomjs.Text &= "	justtoggle(document.getElementById('dvErrorMessage'), true);" & Environment.NewLine
            bError = True
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
    Private Function MakeJSSafe(ByVal JS As String) As String
        JS = JS.Replace("'", "\'")
        JS = JS.Replace(Environment.NewLine, "\n")
        Return JS
    End Function
End Class
