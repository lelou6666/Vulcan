Imports Ektron.Cms
Imports Ektron.Cms.Site
Imports Ektron.Cms.Common.EkConstants
Imports System.Web
Imports System.Web.HttpContext
Imports System.Text
Imports System
Imports Microsoft.VisualBasic
Imports Ektron.ASM.PluginManager
Imports System.Configuration

Public Class Utilities

    Public Shared Sub ProcessThumbnail(ByVal SrcPath As String, ByVal SrcFile As String)
        ProcessThumbnail(SrcPath, SrcFile, 125, 125, 0)
    End Sub
    Public Shared Sub RegisterBaseUrl(ByVal page As Page)
        Dim api As New ContentAPI()
        Dim baseTag As Literal = New Literal()
        baseTag.Text = "<base href=""" & HttpContext.Current.Request.Url.Scheme & "://" & HttpContext.Current.Request.Url.Host & IIf(HttpContext.Current.Request.ServerVariables("SERVER_PORT") = 80, "", ":" & HttpContext.Current.Request.ServerVariables("SERVER_PORT")) & api.RequestInformationRef.SitePath & """ />"
        page.Header.Controls.Add(baseTag)
    End Sub
    Public Shared Sub ValidateUserLogin()
        Dim cApi As New ContentAPI()
        If ((cApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn") = False) Then
            Current.Response.Redirect(cApi.AppPath & "login.aspx?fromLnkPg=1", False)
            Exit Sub
        End If
    End Sub
    Public Shared Sub ProcessThumbnail(ByVal SrcPath As String, ByVal SrcFile As String, ByVal Width As Integer, ByVal height As Integer, ByVal ThumbSize As Integer)
        Dim strSrcLoc As String = ""
        Dim strDesLoc As String = ""
        Dim strExtn As String = "png"
        Dim result As Boolean = False
        Try
            strSrcLoc = SrcPath & "\" & SrcFile
            strExtn = Right(SrcFile, 3)
            strExtn = strExtn.ToLower()
            If "gif" = strExtn Then
                strExtn = "png"
                If ThumbSize = 0 Then
                    strDesLoc = SrcPath & "\thumb_" & Left(SrcFile, Len(SrcFile) - 3) & strExtn
                Else
                    strDesLoc = SrcPath & "\thumb" & ThumbSize & "_" & Left(SrcFile, Len(SrcFile) - 3) & strExtn
                End If
            Else
                If ThumbSize = 0 Then
                    strDesLoc = SrcPath & "\thumb_" & SrcFile
                Else
                    strDesLoc = SrcPath & "\thumb" & ThumbSize & "_" & SrcFile
                End If
            End If
            Dim obj As New EkFileIO
            result = obj.CreateThumbnail(strSrcLoc, strDesLoc, Width, height)
            If (result = False) Then
                Throw New Exception("<p style='background-color:red'>ERROR Initializing:  </p>")
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Prepends Path to URL if URL is relative, otherwise returns URL as is.
    ''' </summary>
    ''' <param name="Path">
    ''' 	Path to prepend to URL unless URL is already qualified, that is, not a relative path.
    ''' 	Value Type: <see cref="String" /> (System.String)
    ''' </param>
    ''' <param name="URL">
    ''' 	The URL to be qualified if it's not already.
    ''' 	Value Type: <see cref="String" /> (System.String)
    ''' </param>
    ''' <returns>A qualified URL.	<see cref="String" /> (System.String)</returns>
    ''' <remarks>
    ''' </remarks>
    ''' --------------------------------------------------------------------------------
    Public Shared Function QualifyURL(ByVal Path As String, ByVal URL As String) As String
		If (IsNothing(Path) OrElse String.Empty = Path) Then Return URL
		If (IsNothing(URL) OrElse String.Empty = URL) Then Return Path
		Dim strDelimiter As String
		If (Path.IndexOf("/") >= 0 OrElse URL.IndexOf("/") >= 0) Then
			strDelimiter = "/"
			Path = Path.Replace("\", strDelimiter)
			URL = URL.Replace("\", strDelimiter)
		Else
			strDelimiter = "\"
			Path = Path.Replace("/", strDelimiter)
			URL = URL.Replace("/", strDelimiter)
		End If
		If (URL.StartsWith(strDelimiter & strDelimiter) OrElse URL.IndexOf(":") >= 0) Then
			Return URL
		ElseIf (Path.EndsWith(strDelimiter) OrElse URL.StartsWith(strDelimiter)) Then
			If (Path.EndsWith(strDelimiter) AndAlso URL.StartsWith(strDelimiter)) Then
				Return Path & URL.Substring(strDelimiter.Length) ' remove extra delimiter
			Else
				Return Path & URL
			End If
		Else
			Return Path & strDelimiter & URL
		End If
	End Function

    ' calling FixId in EkFunctions
    Public Shared Function FixId(ByVal Id As String) As String
        Return Ektron.Cms.Common.EkFunctions.FixId(Id)
	End Function

    Public Shared Sub ShowError(ByVal Message As String)
        'http://support.microsoft.com/kb/q208427/
        'INFO: Maximum URL Length Is 2,083 Characters in Internet Explorer
        'View products that this article applies to.
        'Article ID : 208427
        '        Last(Review) : May(12, 2003)
        'Revision : 2.0
        'This article was previously published under Q208427
        'SUMMARY
        'Internet Explorer has a maximum uniform resource locator (URL) length of 2,083 characters, with a maximum path length of 2,048 characters. This limit applies to both POST and GET request URLs.
        'If you are using the GET method, you are limited to a maximum of 2,048 characters (minus the number of characters in the actual path, of course).
        'POST, however, is not limited by the size of the URL for submitting name/value pairs, because they are transferred in the header and not the URL.
        'RFC 2616, Hypertext Transfer Protocol -- HTTP/1.1 (ftp://ftp.isi.edu/in-notes/rfc2616.txt), does not specify any requirement for URL length.
        Dim strURL As String
        Dim nDiff As Integer
        Dim m_refSiteAPI As New SiteAPI
        Const MAX_URL_LENGTH As Integer = 2048
        Do
            strURL = m_refSiteAPI.AppPath & "reterror.aspx?info=" & Current.Server.UrlEncode(Message)
            nDiff = MAX_URL_LENGTH - strURL.Length
            If nDiff < 0 Then
                ' Shorten the message by a reasonable amount and try again.
                Message = Message.Substring(0, Message.Length + nDiff)
            End If
        Loop While nDiff < 0
        Current.Response.Redirect(strURL, False)
    End Sub

    Public Shared Function SetPostBackPage(ByVal FormAction As String) As String
        Return ("<script>document.forms[0].action = """ & FormAction & """;" & "document.forms[0].__VIEWSTATE.name = 'NOVIEWSTATE';</script>")
    End Function

    Public Shared Function EditorScripts(ByVal var2 As String, ByVal AppeWebPath As String, ByVal BrowserCode As String) As String
        Dim result As New StringBuilder
        If Not (BrowserCode = "ar" Or BrowserCode = "da" Or BrowserCode = "de" Or BrowserCode = "en" Or BrowserCode = "es" Or BrowserCode = "fr" Or BrowserCode = "he" Or BrowserCode = "it" Or BrowserCode = "ja" Or BrowserCode = "ko" Or BrowserCode = "nl" Or BrowserCode = "pt" Or BrowserCode = "ru" Or BrowserCode = "sv" Or BrowserCode = "zh") Then
            BrowserCode = "en"
        End If
        result.Append("<script language=""JavaScript1.2"">" & vbCrLf)
        result.Append("var LicenseKeys = """ & var2 & """;" & vbCrLf)
        result.Append("var eWebEditProPath = """ & AppeWebPath & """; " & vbCrLf)
        result.Append("var WIFXPath= """ & AppeWebPath & """;" & vbCrLf)
        result.Append("var WebImageFXPath = """ & AppeWebPath & """;" & vbCrLf)
        result.Append("var eWebEditProMsgsFilename = ""ewebeditpromessages"" + """ & BrowserCode & """+ "".js"";" & vbCrLf)


        result.Append("function InformationPassingParameters()" & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append("var strLoadPage = """";" & vbCrLf)
        result.Append("var strParamChar = ""?"";" & vbCrLf)

        result.Append("if(""undefined"" != typeof eWebEditProPath)" & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append("  strLoadPage += strParamChar + ""instewep="";" & vbCrLf)
        result.Append("strLoadPage += eWebEditProPath;" & vbCrLf)
        result.Append("strParamChar = ""&"";" & vbCrLf)
        result.Append("}" & vbCrLf)
        result.Append("else" & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append("strLoadPage += strParamChar + ""instewep=undefined"";" & vbCrLf)
        result.Append("strParamChar = ""&"";" & vbCrLf)
        result.Append("}" & vbCrLf)

        result.Append(" if(""undefined"" != typeof LicenseKeys)" & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append("strLoadPage += strParamChar + ""licnewep="";" & vbCrLf)
        result.Append("strLoadPage += LicenseKeys;" & vbCrLf)
        result.Append("strParamChar = ""&"";" & vbCrLf)
        result.Append("}    " & vbCrLf)
        result.Append("else" & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append("strLoadPage += strParamChar + ""licnewep=undefined"";" & vbCrLf)
        result.Append("strParamChar = ""&"";" & vbCrLf)
        result.Append("}" & vbCrLf)
        result.Append("if(""undefined"" != typeof WebImageFXPath)" & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append("if (WebImageFXPath.length > 0)" & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append("strLoadPage += ""&instwifx="";" & vbCrLf)

        result.Append("strLoadPage += WebImageFXPath;" & vbCrLf)
        result.Append("strParamChar =""&"";" & vbCrLf)
        result.Append(" }" & vbCrLf)
        result.Append(" }" & vbCrLf)

        result.Append("if(""undefined"" != typeof WifxLicenseKeys)" & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append("if (WifxLicenseKeys.length > 0)" & vbCrLf)
        result.Append("{" & vbCrLf)
        result.Append("strLoadPage += ""&licnwifx="";" & vbCrLf)
        result.Append("strLoadPage += WifxLicenseKeys;" & vbCrLf)
        result.Append("strParamChar = ""&"";" & vbCrLf)
        result.Append("}" & vbCrLf)
        result.Append("}" & vbCrLf)

        result.Append("return(strLoadPage);" & vbCrLf)
        result.Append("}" & vbCrLf)
        result.Append("</script>" & vbCrLf)

        result.Append("<script language=""JavaScript1.2"" src=""" & AppeWebPath & "cms_ewebeditpro.js""></script>" & vbCrLf)
        'result.Append("<script language=""JavaScript1.2"">	" & vbCrLf)
        '// The install popup was correctly created at the beginning
        '// (within the eWebEditProinstallPopupUrl variable)
        '// but it needs to be set into the installPopup.url value
        '// for it to automatically be used to install the editor.
        '// Otherwise, it sits in the variable doing nothing.
        '// eWebEditPro.parameters.installPopup.url = eWebEditProinstallPopupUrl + InformationPassingParameters();
        'result.Append("</script>" & vbCrLf)
        Return (result.ToString)
    End Function

    Public Shared Function eWebEditProField(ByVal EditorName As String, ByVal FieldName As String, ByVal SetContentType As String, ByVal GetContentType As String, ByVal ContentHtml As String) As String
        Dim result As New StringBuilder
        If EditorName <> FieldName Then
            result.Append("<input type=""hidden"" name=""" & FieldName & """ value=""" & Current.Server.HtmlEncode(ContentHtml) & """>" & vbCrLf)
        End If
        result.Append("<script language=""JavaScript1.2"" type=""text/javascript"">" & vbCrLf)
        result.Append("<!--" & vbCrLf)
        result.Append("eWebEditPro.defineField(""" & EditorName & """, """ & FieldName & """, """ & SetContentType & """, """ & GetContentType & """);" & vbCrLf)
        result.Append("//-->" & vbCrLf)
        result.Append("</script>" & vbCrLf)
        Return (result.ToString)
    End Function

    Public Shared Function eWebEditProEditor(ByVal FieldName As String, ByVal Width As String, ByVal Height As String, ByVal ContentHtml As String) As String
        Dim result As New StringBuilder
        result.Append("<input type=""hidden"" name=""" & FieldName & """ value=""" & Current.Server.HtmlEncode(ContentHtml) & """>")
        result.Append("<script language=""JavaScript1.2"" type=""text/javascript"">" & vbCrLf)
        result.Append("<!--" & vbCrLf)
        result.Append("eWebEditPro.create(""" & FieldName & """, """ & Width & """, """ & Height & """);" & vbCrLf)
        result.Append("//-->" & vbCrLf)
        result.Append("</script>")
        Return (result.ToString)
    End Function

    Public Shared Function eWebEditProPopupButton(ByVal ButtonName As String, ByVal FieldName As String) As String
        Dim result As New StringBuilder
        result.Append("<script language=""JavaScript1.2"" type=""text/javascript"">" & vbCrLf)
        result.Append("<!--" & vbCrLf)
        result.Append("eWebEditPro.createButton(""" & ButtonName & """, """ & FieldName & """);" & vbCrLf)
        result.Append("//-->" & vbCrLf)
        result.Append("</script>")
        Return (result.ToString)
    End Function

    Public Shared Function IsAsset(ByVal lContentType As Integer, ByVal strAssetID As String) As Boolean
        Dim result As Boolean = False
        result = ((ManagedAsset_Min <= lContentType And lContentType <= ManagedAsset_Max) Or (Archive_ManagedAsset_Min <= lContentType And lContentType <= Archive_ManagedAsset_Max) Or strAssetID.Length > 0)
        Return (result)
    End Function
    Public Shared Function IsAssetType(ByVal lContentType As Integer) As Boolean
        Dim result As Boolean = False
        result = ((ManagedAsset_Min <= lContentType And lContentType <= ManagedAsset_Max) Or (Archive_ManagedAsset_Min <= lContentType And lContentType <= Archive_ManagedAsset_Max))
        Return (result)
    End Function

    Public Shared Function IsBrowserIE() As Boolean
        Return (IIf((Current.Request.Browser.Type.IndexOf("IE") <> -1), True, False))
    End Function

    Public Shared Function IsPc() As Boolean
        Dim str As String
        str = Current.Request.ServerVariables("HTTP_USER_AGENT")
        IsPc = IIf((InStr(str, "Windows") > 0), True, False)
    End Function

    Public Shared Function IsMac() As Boolean
        Return (Not IsPc())
    End Function

    Public Shared Function StripHTML(ByVal strText As String) As String
        Return ContentAPI.StripHTML(strText)
    End Function

    Public Shared Function DeserializeSitemapPath(ByVal form As Collections.Specialized.NameValueCollection, ByVal language As Integer) As Ektron.Cms.Common.SitemapPath()
        Dim xml As String = System.Web.HttpUtility.UrlDecode(form("saved_sitemap_path"))
        Dim doc As New System.Xml.XmlDocument
        Dim nodes As System.Xml.XmlNodeList
        Dim node As System.Xml.XmlNode
        Dim ret As Ektron.Cms.Common.SitemapPath() = Nothing
        Dim smpNode As Ektron.Cms.Common.SitemapPath
        Dim iCount As Integer = 0
        Try
            xml = xml.Replace("&", "&amp;")
            doc.LoadXml(xml)
            nodes = doc.SelectNodes("sitemap/node")
            For Each node In nodes
                smpNode = New Ektron.Cms.Common.SitemapPath
                smpNode.Title = System.Web.HttpUtility.HtmlDecode(node.SelectSingleNode("title").InnerXml)
                smpNode.Url = System.Web.HttpUtility.HtmlDecode(node.SelectSingleNode("url").InnerXml)
                smpNode.Order = node.SelectSingleNode("order").InnerXml
                smpNode.Description = System.Web.HttpUtility.HtmlDecode(node.SelectSingleNode("description").InnerXml)
                smpNode.Language = language
                iCount += 1
                ReDim Preserve ret(iCount)
                ret(iCount) = smpNode
            Next
        Catch
        End Try
        Return ret
    End Function

    Public Shared Function FindSitemapPath(ByVal sitemapPaths() As Ektron.Cms.Common.SitemapPath, ByVal sitemapPath As Ektron.Cms.Common.SitemapPath) As Integer
        'return -1 if not found
        Dim iRet As Integer = -1
        Dim iLoop As Integer = 1
        Dim node As Ektron.Cms.Common.SitemapPath
        If sitemapPath Is Nothing Then Return -1
        If sitemapPaths IsNot Nothing Then
            For iLoop = 1 To sitemapPaths.Length - 1
                node = sitemapPaths(iLoop)
                If ((node.Title = sitemapPath.Title) And (node.Url = sitemapPath.Url) And (node.FolderId = sitemapPath.FolderId)) Then
                    iRet = iLoop
                    Exit For
                End If
            Next
        End If
        Return iRet
    End Function

    Public Shared Function IsDefaultXmlConfig(ByVal xml_id As Long, ByVal active_list As XmlConfigData()) As Boolean
        Dim i As Integer = 0
        Dim xmlData As XmlConfigData
        For Each xmlData In active_list
            If (xmlData.Id = xml_id) Then
                If (xmlData.IsDefault) Then
                    Return True
                Else
                    Return False
                End If
            End If
        Next
        Return False
    End Function

    Public Shared Function IsHTMLDefault(ByVal active_list As XmlConfigData()) As Boolean
        Dim i As Integer = 0
        Dim xmlData As XmlConfigData
        For Each xmlData In active_list
            If (xmlData.IsDefault And xmlData.Id <> 0) Then
                Return False
            End If
        Next
        Return True
    End Function

    Public Shared Function IsNonFormattedContentAllowed(ByVal active_list As XmlConfigData()) As Boolean
        Return Ektron.Cms.Common.EkFunctions.IsNonFormattedContentAllowed(active_list)
    End Function

    Public Shared Function GetDefaultXmlConfig(ByVal folder_id As Long) As Long
        Dim i As Integer = 0
        Dim m_refContentApi As New ContentAPI()
        Dim active_list As XmlConfigData()
        Dim xmlData As XmlConfigData
        active_list = m_refContentApi.GetEnabledXmlConfigsByFolder(folder_id)
        For Each xmlData In active_list
            If (xmlData.IsDefault) Then
                Return xmlData.Id
            End If
        Next
        Return 0
    End Function

    Public Shared Sub AddLBpaths(ByRef data As Object)
        Dim apiCont As New ContentAPI
        Dim objLib As Ektron.Cms.Library.EkLibrary
        Dim lbICount As Integer
        Dim lbFCount As Integer
        Dim lb As Object
        Dim cLbs As Collection
        lbICount = 0
        lbFCount = 0
        objLib = apiCont.EkLibraryRef
        cLbs = objLib.GetAllLBPaths("images")
        If cLbs.Count Then
            For Each lb In cLbs
                lbICount = lbICount + 1
                data.Add(HttpContext.Current.Server.MapPath(lb("LoadBalancePath")), "LoadBalanceImagePath_" & lbICount)
            Next
        End If
        data.Add(lbICount, "LoadBalanceImageCount")
        cLbs = Nothing
        lb = Nothing
        cLbs = objLib.GetAllLBPaths("files")
        If cLbs.Count Then
            For Each lb In cLbs
                lbFCount = lbFCount + 1
                data.Add(HttpContext.Current.Server.MapPath(lb("LoadBalancePath")), "LoadBalanceFilePath_" & lbFCount)
            Next
        End If
        data.Add(lbFCount, "LoadBalanceFileCount")
        cLbs = Nothing
    End Sub
    Public Shared Sub SetLanguage(ByRef api As Object)
        Dim ContentLanguage As Integer = -1
        If (Not (Current.Request.QueryString("LangType") Is Nothing)) Then
            If (Current.Request.QueryString("LangType") <> "") Then
                ContentLanguage = Convert.ToInt32(Current.Request.QueryString("LangType"))
                api.SetCookieValue("LastValidLanguageID", ContentLanguage)
            Else
                If api.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(api.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If api.GetCookieValue("LastValidLanguageID") <> "" Then
                ContentLanguage = Convert.ToInt32(api.GetCookieValue("LastValidLanguageID"))
            End If
        End If
        If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            api.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            api.ContentLanguage = ContentLanguage
        End If
    End Sub

    Public Shared Function GetLanguageId() As Integer
        Dim languageId As Integer = 0

        If HttpContext.Current IsNot Nothing _
            AndAlso HttpContext.Current.Request IsNot Nothing _
            AndAlso Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("LangType")) _
            AndAlso Integer.TryParse(HttpContext.Current.Request.QueryString("LangType"), languageId) _
            AndAlso languageId > 0 Then
            Return languageId
        End If

        If HttpContext.Current IsNot Nothing _
            AndAlso HttpContext.Current.Request IsNot Nothing _
            AndAlso HttpContext.Current.Request.Cookies IsNot Nothing _
            AndAlso HttpContext.Current.Request.Cookies.Count > 0 _
            AndAlso Not String.IsNullOrEmpty(HttpContext.Current.Request.Cookies("ecm")("LastValidLanguageID")) _
            AndAlso Integer.TryParse(HttpContext.Current.Request.Cookies("ecm")("LastValidLanguageID"), languageId) _
            AndAlso languageId > 0 Then
            Return languageId
        End If

        Return 0
    End Function

    Public Shared Function GetLanguageId(ByRef capi As Ektron.Cms.ContentAPI) As Integer
        Dim languageId As Integer = GetLanguageId()

        If (languageId > 0) Then
            Return languageId
        End If

        If (IsNothing(capi)) Then
            Return 0
        End If

        If (capi.RequestInformationRef.ContentLanguage > 0) Then
            Return capi.RequestInformationRef.ContentLanguage
        End If

        Return capi.RequestInformationRef.DefaultContentLanguage
    End Function


    'Gets the setting in web.config that allows/disallows executing developer samples
    Public Shared Function AllowExecDevSamples() As Boolean

        Dim value As Boolean = False
        If (ConfigurationManager.AppSettings("ek_EnableDeveloperSamples") IsNot Nothing) Then
            value = CBool(ConfigurationManager.AppSettings("ek_EnableDeveloperSamples").ToString())
        End If
        AllowExecDevSamples = value

    End Function
    'Redirect to a information page if key is not set in web.config
    Public Shared Sub CheckDevSampleEnabled()
        If Not AllowExecDevSamples() Then
            Dim m_refSiteAPI As New SiteAPI
            Dim strURL As String
            strURL = m_refSiteAPI.SitePath & "Developer/InfoDevSample.aspx"
            HttpContext.Current.Response.Redirect(strURL, False)
        End If
    End Sub

    Public Shared Function GetMembershipAddContentJavascript(ByVal folder_id As Long, ByVal lang_id As Integer, ByVal height As Integer, ByVal width As Integer) As String
        Return GetMembershipAddContentJavascript(folder_id, lang_id, height, width, 0)
    End Function

    Public Shared Function GetMembershipAddContentJavascript(ByVal folder_id As Long, ByVal lang_id As Integer, ByVal height As Integer, ByVal width As Integer, ByVal DefaultTaxonomyID As Long) As String
        Dim m_refSiteAPI As New SiteAPI()
        Dim str As New StringBuilder()

        Dim TaxonomyQuery As String = ""
        If (HttpContext.Current.Request.QueryString("taxonomyid") IsNot Nothing AndAlso HttpContext.Current.Request.QueryString("taxonomyid") <> "") Then
            TaxonomyQuery = "&amp;taxonomyid=" & HttpContext.Current.Request.QueryString("taxonomyid")
        ElseIf (DefaultTaxonomyID > 0) Then
            TaxonomyQuery = "&amp;taxonomyid=" & DefaultTaxonomyID
        End If

        If (lang_id = -1 Or lang_id = 0) Then
            lang_id = m_refSiteAPI.RequestInformationRef.DefaultContentLanguage
        End If
        str.Append("{")
        str.Append("var cToolBar = 'toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=1,resizable=1,width=")
        str.Append(width)
        str.Append(",height=")
        str.Append(height)
        str.Append("';")
        str.Append("var url='")
        str.Append(m_refSiteAPI.AppPath)
        If (m_refSiteAPI.RequestInformationRef.IsMembershipUser = 1) Then
            str.Append("/membership_add_content.aspx?mode=add&amp;mode_id=" & folder_id & "&amp;lang_id=" & lang_id & "';")
        Else
            str.Append("/edit.aspx?close=true" & TaxonomyQuery & "&ContType=1&LangType=" & m_refSiteAPI.RequestInformationRef.ContentLanguage & "&type=add&createtask=1&id=" & folder_id & "&AllowHTML=1';")
        End If
        str.Append("var taxonomyselectedtree = 0; ")
        str.Append("if (document.getElementById('taxonomyselectedtree') != null) {")
        str.Append("taxonomyselectedtree = document.getElementById('taxonomyselectedtree').value;")
        str.Append("}")
        str.Append(" if (taxonomyselectedtree >0) {url = url + '&seltaxonomyid=' + taxonomyselectedtree;} ")
        str.Append("var popupwin = window.open(url, 'Edit', cToolBar);")
        str.Append("return popupwin; };return false;")
        Return str.ToString()
    End Function

    Public Shared Function GetMembershipAddContentJavascript(ByVal folder_id As Long) As String
        Return Utilities.GetMembershipAddContentJavascript(folder_id, 0)
    End Function

    Public Shared Function GetMembershipAddContentJavascript(ByVal folder_id As Long, ByVal lang_id As Integer) As String
        Dim height As Integer = 660
        Dim width As Integer = 790
        Return Utilities.GetMembershipAddContentJavascript(folder_id, lang_id, height, width)
    End Function

    Public Shared Function GetMembershipAddContentJavascript(ByVal folder_id As Long, ByVal height As Integer, ByVal width As Integer) As String
        Return Utilities.GetMembershipAddContentJavascript(folder_id, 0, height, width)
    End Function

    Public Shared Function GetAssetDownloadLink(ByVal content_id As Long) As String
        Dim content_api As Ektron.Cms.ContentAPI = Nothing
        Dim content_data As ContentData = Nothing
        Try
            content_api = New Ektron.Cms.ContentAPI()
            content_data = content_api.GetContentById(content_id)
            If (content_data.AssetData Is Nothing OrElse content_data.AssetData.Id.Trim().Length = 0) Then
                Return String.Empty
            End If
            Return content_api.RequestInformationRef.ApplicationPath & "/DownloadAsset.aspx?id=" & content_id
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    Public Shared Function AutoSummary(ByVal sHTML As String) As String
        Return ContentAPI.AutoSummary(sHTML)
    End Function

    Private Shared Function FindFirstWords(ByVal input As String, ByVal howManyToFind As Integer) As String
        Return FindFirstWords(input, howManyToFind)
    End Function

    Public Shared Function WikiQLink(ByVal strText As String, ByVal folderID As Long) As String
        If strText.Length > 0 Then
            strText = Replace(strText, "[[", "<span style=""color:blue;"" folderid=""" & folderID & """ class=""MakeLink"" category="""" target=""_self"" >", , , CompareMethod.Text)
            strText = Replace(strText, "]]", "</span>", , , CompareMethod.Text)
        End If
        Return (strText)
    End Function

	Public Shared Function BuildRegexToCheckMaxLength(ByVal MaxLength As Integer) As String
		' Example use:
		' RegularExpressionValidator.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(iMaxContLength)
		'
		' Firefox 2.0 regular expression max repetition is 65535, that is, "{0,65535}"
		If (MaxLength <= 0) Then Throw New ArgumentException("MaxLength must be positive", "MaxLength")
		Dim strRegex As New StringBuilder
		' Form if max <= 65535: ^[\w\W]{0,max}$
		' Form if max > 65535: ^([\w\W]{0,32768}){0,<%=max \ 32768%>}[\w\W]{0,<%=max Mod 32768%>}$
		' \w\W means any character including new line
		' Example,
		' 768000 => ^([\w\W]{0,32768}){0,23}[\w\W]{0,14336}$
		strRegex.Append("^")
		If (MaxLength > 65535) Then
			strRegex.Append("([\w\W]{0,32768}){0,")
			strRegex.Append(Convert.ToString(MaxLength \ 32768)) ' quotient
			strRegex.Append("}")
		End If
		strRegex.Append("[\w\W]{0,")
		If (MaxLength <= 65535) Then
			strRegex.Append(MaxLength.ToString)
		Else
			strRegex.Append(Convert.ToString(MaxLength Mod 32768)) ' remainder
		End If
		strRegex.Append("}$")
		Return strRegex.ToString
    End Function

    Public Shared Function MSWordFilterOptions(ByVal settings_data As SettingsData) As Ektron.Telerik.WebControls.EditorStripFormattingOptions
        Dim ConfigSetting As Ektron.Telerik.WebControls.EditorStripFormattingOptions = Ektron.Telerik.WebControls.EditorStripFormattingOptions.MSWord
        If True = settings_data.PreserveWordStyles And True = settings_data.PreserveWordClasses Then
            ConfigSetting = Ektron.Telerik.WebControls.EditorStripFormattingOptions.MSWordPreserveStyles + Ektron.Telerik.WebControls.EditorStripFormattingOptions.MSWordPreserveClasses
        ElseIf True = settings_data.PreserveWordStyles Then
            ConfigSetting = Ektron.Telerik.WebControls.EditorStripFormattingOptions.MSWordPreserveStyles
        ElseIf True = settings_data.PreserveWordClasses Then
            ConfigSetting = Ektron.Telerik.WebControls.EditorStripFormattingOptions.MSWordPreserveClasses
        End If
        Return ConfigSetting
    End Function

    Public Shared Function GetEditorPreference(ByVal Request As System.Web.HttpRequest) As String
        'TODO: Move the editor choices to an xml file specified by the server then key the possible values
        'off of a matrix setup in the xml file based on OS version and browser version
        Dim SelectedEditControl As String = "ContentDesigner"
        Dim IsMac As Boolean = False
        Try
            If (Request.Browser.Platform.IndexOf("Win") = -1) Then
                IsMac = True
            End If

            'Which Editor
            If IsMac Then
                If (ConfigurationManager.AppSettings("ek_EditControlMac") IsNot Nothing) Then
                    SelectedEditControl = ConfigurationManager.AppSettings("ek_EditControlMac").ToString()
                End If
            Else
                If (ConfigurationManager.AppSettings("ek_EditControlWin") IsNot Nothing) Then
                    SelectedEditControl = ConfigurationManager.AppSettings("ek_EditControlWin").ToString()
                End If
                If (SelectedEditControl.ToLower = "userpreferred") Then
                    Dim api As CommonApi = New CommonApi()
                    If api.RequestInformationRef.UserEditorType = Common.EkEnumeration.UserEditorType.ewebeditpro Then
                        SelectedEditControl = "eWebEditPro"
                    Else
                        SelectedEditControl = "ContentDesigner"
                    End If
                End If
            End If
        Catch ex As Exception
            SelectedEditControl = "ContentDesigner"
        End Try

        Return SelectedEditControl
    End Function
    Public Shared Function IsChecked(ByVal val1 As String, ByVal val2 As String) As String
        If (val1.ToLower() = val2.ToLower()) Then
            Return (" checked=""checked"" ")
        Else
            Return ("")
        End If
    End Function
    Public Shared Function IsSelected(ByVal val1 As String, ByVal val2 As String) As String
        If (val1.ToLower() = val2.ToLower()) Then
            Return (" selected=""selected"" ")
        Else
            Return ("")
        End If
    End Function

    Public Shared Function GetCorrectThumbnailFileWithExtn(ByVal sFilename As String) As String
        Dim aTemp() As String = Split(sFilename, ".")
        Dim sRet As String = ""
        Try
            If aTemp.Length > 1 Then
                If aTemp((aTemp.Length - 1)).ToLower() = "gif" Then
                    aTemp((aTemp.Length - 1)) = "png"
                    sRet = Join(aTemp, ".")
                Else
                    sRet = sFilename
                End If
            End If
        Catch ex As Exception
            sRet = sFilename
        End Try
        Return sRet
    End Function

    Public Shared Sub DisableActionRewrite(ByVal context As HttpContext)
            If context.Items("ActionAlreadyWritten") Is Nothing Then
                context.Items("ActionAlreadyWritten") = True
            End If
    End Sub

    Public Shared Function GetFolderImage(ByVal type As Common.EkEnumeration.FolderType, ByVal applicationImagePath As String) As String

        Dim imageURL As String = applicationImagePath

        Select Case type

            Case Is = Common.EkEnumeration.FolderType.Community

                imageURL &= "images/ui/icons/folderCommunity.png"

            Case Common.EkEnumeration.FolderType.Catalog

                imageURL &= "images/ui/icons/folderGreen.png"

            Case Else

                imageURL &= "images/ui/icons/folder.png"

        End Select

        Return imageURL

    End Function

    Public Shared Function GetProductImage(ByVal entryType As Common.EkEnumeration.CatalogEntryType, ByVal applicationImagePath As String) As String

        Dim productImage As String = applicationImagePath

        Select Case entryType

            Case Common.EkEnumeration.CatalogEntryType.Bundle

                productImage &= "images/ui/icons/package.png"

            Case Common.EkEnumeration.CatalogEntryType.ComplexProduct

                productImage &= "images/ui/icons/bricks.png"

            Case Common.EkEnumeration.CatalogEntryType.Kit

                productImage &= "images/ui/icons/bulletGreen.png"

            Case Common.EkEnumeration.CatalogEntryType.SubscriptionProduct

                productImage &= "images/ui/icons/bookGreen.png"

            Case Else

                productImage &= "images/ui/icons/brick.png"

        End Select

        Return productImage

    End Function

End Class
