<%@ Page Language="vb" AutoEventWireup="false" validateRequest="false" %>
<%@ Import Namespace="Ektron.Cms.UI.CommonUI" %>
<%@ Import Namespace="Ektron.Cms.Site" %>
<%@ Import Namespace="Ektron.Cms.Content" %>
<script runat="server">
Dim g_LogicalRefDestination As String
    Dim g_objUpload As Object
	Dim g_binaryFormData As Object
	
	'''''''''''''''''''''''''''''''''''''''''''''''''
	' This function will Read the Post Information and detect errors.
	Private Sub BinaryReadPackage()
		On Error Resume Next
		Err.Clear()
		g_binaryFormData = Request.BinaryRead(Request.TotalBytes)
		If Err.Number <> 0 Then
			If -2147467259 = Err.Number Then
                Dim ErrDescription As String = ""
				ErrDescription = "Error: The file being upload is larger than what is allowed in the IIS. "
				ErrDescription = ErrDescription & "Please change the ASPMaxRequestEntityAllowed to a larger number in the metabase.xml file (usually located in c:\windows\system32\inetsrv). "
				Response.Write(ErrDescription & Chr(13) & Chr(10) & "<br/>")
			End If
			Response.Write(Err.Description)
		End If
	End Sub
	
    '''''''''''''''''''''''''''''''''''''''''''''''''
    ' This function will receive the files and send back
    ' the required response data.  There is no processing
    ' of the files and there is no affecting the file data.
    Private Sub ReceiveSubmittedFiles()
        'Dim fileObj
        Dim ErrorCode As VariantType
        'Dim iFileIdx As Integer
        Dim strNewFileName As String
 
        ErrorCode = 0
        strNewFileName = g_objUpload.EkFileSave(g_binaryFormData, "uploadfilephoto", Server.MapPath(g_LogicalRefDestination), ErrorCode, "makeunique")        ' was 
		
        'iFileCount = g_objUpload.FileCount()
        'If iFileCount > 0 then
        'Do while iFileIdx < iFileCount
        '	iFileIdx = iFileIdx + 1
        '	Set fileObj = g_objUpload.FileObject(iFileIdx)
        '	fileObj.FileUrl("HTTP://" & Request.ServerVariables("SERVER_NAME") & g_LogicalRefDestination & "/" & fileObj.FileName())
        'loop
        'End If

        Response.Write("<html><body><h1>Upload Received</h1><p>The file resides in:</p></p>" + g_LogicalRefDestination + "</p>" + "g_objUpload.ResponseData()" + "</body></html>")
	
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''
    ' This is where the files will be seen from the web,
    ' NOT the physical disk drive location.
    Private Sub CreateVirtualDestination()
        Dim strCur As String
        Dim strDirs As Array
        Dim iMax As Integer
        Dim idx As Integer
        Dim ErrorCode As VariantType
	
        ErrorCode = 0
        g_LogicalRefDestination = g_objUpload.EkFormFieldValue(g_binaryFormData, "editor_media_path", ErrorCode)
        If Len(g_LogicalRefDestination) = 0 Then
            ' A directory was not sent to us.
            strCur = Request.ServerVariables("URL")
            strDirs = Split(strCur, "/")
            iMax = UBound(strDirs)
            If iMax > 0 Then
                idx = 1
                strCur = strDirs(0)
                While idx < iMax
                    strCur = strCur & "/" & strDirs(idx)
                    idx = idx + 1
                End While
                g_LogicalRefDestination = strCur & "/upload"
            Else
                'Could not split the directory.
                g_LogicalRefDestination = "/webimagefx/upload"
            End If
        End If
    	
    End Sub
</script>
<%
' This code is available for modification.
' Modify to follow the requirements of the server.
	g_objUpload = New Ektron.Cms.EkFileIO 'CreateObject("eWepAutoSvr4.EkFile")
	BinaryReadPackage()

	' This is where the files will be seen from the web,
	' NOT the physical disk drive location.
	CreateVirtualDestination()
	
	'Recieve and save the files
	ReceiveSubmittedFiles()



	'''''''''''''''''''''''''''''''''''''''''''''''''
	'''''''''''''''''''''''''''''''''''''''''''''''''
	' Below is an example of how you process
	' the uploaded files and send back other
	' file information such as thumbnail values.
	'Sub CreateThumbnailsFromFiles()
	'	Dim iClientMajorRev, iClientMinorRev, iFileCount
	'    Dim g_binaryFormData, g_objUpload, fileObj, g_LogicalRefDestination
	'    Dim strNewFileName, strFileLoc, ErrorCode, iFileIdx
	' 
	'    g_binaryFormData = Request.BinaryRead(Request.TotalBytes)
	'    set g_objUpload = CreateObject("EwepTransfer4.EkFile")
	'	
	'    strNewFileName = g_objUpload.EkFileSave(g_binaryFormData, "uploadfilephoto", _
	'        Server.MapPath(g_LogicalRefDestination), ErrorCode, "makeunique")
	'		
	'	iFileCount = g_objUpload.FileCount()
	'	If iFileCount > 0 then
	'		Do while iFileIdx < iFileCount
	'			iFileIdx = iFileIdx + 1
	'			Set fileObj = g_objUpload.FileObject(iFileIdx)
	'			strNewFileName = fileObj.FileName()
	'			strFileLoc = "HTTP://" & Request.ServerVariables("SERVER_NAME") & g_LogicalRefDestination & "/" & strNewFileName
	'			fileObj.FileUrl(strFileLoc)
	'			fileObj.Thumbnail(CreateThumbnail(strFileLoc))
	'			fileObj.ThumbReference(ExtractThumbnailRef(strFileLoc))
	'		loop
	'		
	'		'Retrieve client data
	'		iClientMajorRev = g_objUpload.ClientMajorRev()
	'		iClientMinorRev = g_objUpload.ClientMinorRev() 
	'		
	'		Response.Write(g_objUpload.ResponseData()) 
	'	End If
	'End Sub
%> 


