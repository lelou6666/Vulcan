Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports System.IO
Imports Ektron.Cms.Commerce

Partial Class compares
    Inherits System.Web.UI.Page

#Region "Private members"

    Private m_intId As Long = 0
    Private m_intHistoryId As Long = 0
    Private newcontent As String = ""
    Private basecontent As String = ""
    Private m_refContApi As New ContentAPI
    Private IsXMLDoc As Boolean = False
    Private IsPublished As Boolean = False
    Private ContentLanguage As Integer = -1
    Private CurrentXslt As String = ""
    Private PageInfo As String = ""
    Private strContentHtml As String = ""
    Private m_ContentType As Long = CMSContentType_Content

#End Region


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try

       
            If Page.Request.Url.Scheme = "https" Then
                PageInfo = "src=""https://" & Request.ServerVariables("Server_name") & "/"
            Else
                PageInfo = "src=""http://" & Request.ServerVariables("Server_name") & "/"
            End If

            If (Request.QueryString("id") <> "") Then
                m_intId = Request.QueryString("id")
            End If
            If (Not (Request.QueryString("LangType") Is Nothing)) Then
                If (Request.QueryString("LangType") <> "") Then
                    ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                    m_refContApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
                Else
                    If m_refContApi.GetCookieValue("LastValidLanguageID") <> "" Then
                        ContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"))
                    End If
                End If
            Else
                If m_refContApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
            If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
                m_refContApi.ContentLanguage = ALL_CONTENT_LANGUAGES
            Else
                m_refContApi.ContentLanguage = ContentLanguage
            End If
            If (Request.QueryString("hist_id") <> "") Then
                m_intHistoryId = Request.QueryString("hist_id")
            End If

            m_ContentType = m_refContApi.EkContentRef.GetContentType(m_intId)

            Util_GetBaseContent()
            Util_GetNewContent()

            'If IsPublished And Request.QueryString("hist_id") Is Nothing Then

            '    Dim tempString As String = basecontent
            '    basecontent = newcontent
            '    newcontent = tempString

            'End If
            Process_Compare(basecontent, newcontent)
        Catch ex As Exception
            Response.Redirect(m_refContApi.ApplicationPath & "reterror.aspx?info=" & ex.Message, False)
        End Try
    End Sub
    Private Sub Util_GetBaseContent()
        Select Case m_ContentType
            Case CMSContentType_CatalogEntry
                Dim m_refCatalogAPI As New CatalogEntryApi()
                Dim entry_data As EntryData = Nothing

                entry_data = m_refCatalogAPI.GetItem(m_intId)

                If (entry_data.ProductType.PackageDisplayXslt.Length > 0) Then
                    basecontent = m_refContApi.TransformXsltPackage(entry_data.Html, entry_data.ProductType.PackageDisplayXslt, False)
                Else
                    If (Convert.ToString(entry_data.ProductType.PhysPathComplete("Xslt" & entry_data.ProductType.DefaultXslt)).Length > 0) Then
                        CurrentXslt = entry_data.ProductType.PhysPathComplete("Xslt" & entry_data.ProductType.DefaultXslt)
                    Else
                        CurrentXslt = entry_data.ProductType.LogicalPathComplete("Xslt" & entry_data.ProductType.DefaultXslt)
                    End If
                    basecontent = m_refContApi.TransformXSLT(entry_data.Html, CurrentXslt)
                End If
                basecontent = Server.HtmlDecode(Replace(basecontent, "src=""/", PageInfo))
            Case Else
                Dim content_data As ContentData
                Dim show_content_data As ContentData
                show_content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Published)
                If (m_intHistoryId = 0) Then
                    content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Published)
                Else
                    content_data = m_refContApi.GetContentByHistoryId(m_intHistoryId)
                End If
                If (show_content_data Is Nothing) Then ' only get staged content if there has never been published content
                    show_content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Staged)
                End If
                If (content_data Is Nothing) Then
                    content_data = show_content_data
                End If
                'Published content
                IsXMLDoc = Not (IsNothing(content_data.XmlConfiguration))
                If IsXMLDoc Then
                    If (content_data.XmlConfiguration.PackageDisplayXslt.Length > 0) Then
                        basecontent = m_refContApi.TransformXsltPackage(content_data.Html, content_data.XmlConfiguration.PackageDisplayXslt, False)
                    Else
                        If (Convert.ToString(content_data.XmlConfiguration.PhysPathComplete("Xslt" & content_data.XmlConfiguration.DefaultXslt)).Length > 0) Then
                            CurrentXslt = content_data.XmlConfiguration.PhysPathComplete("Xslt" & content_data.XmlConfiguration.DefaultXslt)
                        Else
                            CurrentXslt = content_data.XmlConfiguration.LogicalPathComplete("Xslt" & content_data.XmlConfiguration.DefaultXslt)
                        End If
                        basecontent = m_refContApi.TransformXSLT(content_data.Html, CurrentXslt)
                    End If
                    IsXMLDoc = False
                Else
                    basecontent = content_data.Html
                End If
                basecontent = Replace(basecontent, "src=""/", PageInfo)
                If (Not IsXMLDoc) Then basecontent = RemoveHTML(basecontent)
                IsPublished = show_content_data.IsPublished
        End Select
    End Sub

    Private Sub Util_GetNewContent()
        Select Case m_ContentType
            Case CMSContentType_CatalogEntry
                Dim m_refCatalogAPI As New CatalogEntryApi()
                Dim entry_version_data As EntryData = Nothing
                If m_intHistoryId = 0 Then
                    entry_version_data = m_refCatalogAPI.GetItemEdit(m_intId, m_refContApi.ContentLanguage, False)
                Else
                    entry_version_data = m_refCatalogAPI.GetItemVersion(m_intId, m_refContApi.ContentLanguage, m_intHistoryId)
                End If
                If (entry_version_data.ProductType.PackageDisplayXslt.Length > 0) Then
                    newcontent = m_refContApi.TransformXsltPackage(entry_version_data.Html, entry_version_data.ProductType.PackageDisplayXslt, False)
                Else
                    If (Convert.ToString(entry_version_data.ProductType.PhysPathComplete("Xslt" & entry_version_data.ProductType.DefaultXslt)).Length > 0) Then
                        CurrentXslt = entry_version_data.ProductType.PhysPathComplete("Xslt" & entry_version_data.ProductType.DefaultXslt)
                    Else
                        CurrentXslt = entry_version_data.ProductType.LogicalPathComplete("Xslt" & entry_version_data.ProductType.DefaultXslt)
                    End If
                    newcontent = m_refContApi.TransformXSLT(entry_version_data.Html, CurrentXslt)
                End If
                newcontent = Replace(newcontent, "src=""/", PageInfo)
            Case Else
                Dim content_data As ContentData
                'Dim show_content_data As ContentData
                If (m_intHistoryId = 0) Then
                    content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Staged)
                    'show_content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Staged)
                    If (content_data Is Nothing) Then
                        content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Published)
                        'content_data = show_content_data
                    End If
                    strContentHtml = content_data.Html
                Else
                    content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Published)
                    strContentHtml = content_data.Html
                End If
                CurrentXslt = ""
                IsXMLDoc = Not (IsNothing(content_data.XmlConfiguration))
                If IsXMLDoc Then
                    If (content_data.XmlConfiguration.PackageDisplayXslt.Length > 0) Then
                        newcontent = m_refContApi.TransformXsltPackage(strContentHtml, content_data.XmlConfiguration.PackageDisplayXslt, False)
                    Else
                        If (Convert.ToString(content_data.XmlConfiguration.PhysPathComplete("Xslt" & content_data.XmlConfiguration.DefaultXslt)).Length > 0) Then
                            CurrentXslt = content_data.XmlConfiguration.PhysPathComplete("Xslt" & content_data.XmlConfiguration.DefaultXslt)
                        Else
                            CurrentXslt = content_data.XmlConfiguration.LogicalPathComplete("Xslt" & content_data.XmlConfiguration.DefaultXslt)
                        End If
                        newcontent = m_refContApi.TransformXSLT(strContentHtml, CurrentXslt)
                    End If
                    IsXMLDoc = False
                Else
                    newcontent = strContentHtml
                End If
                newcontent = Replace(newcontent, "src=""/", PageInfo)
                If (Not IsXMLDoc) Then newcontent = RemoveHTML(newcontent)
        End Select
    End Sub

    Private Sub Process_Compare(ByVal oldXHTML As String, ByVal newXHTML As String)
        If (Not (oldXHTML = newXHTML)) Then
            ' see if we can run the cshtmldiff control server-side (doesn't work on 64-bit windows)
            Try
                Dim diffCtrl As eWebDiffCtrl.eWebDiffCtrl = Nothing
                diffCtrl = New eWebDiffCtrl.eWebDiffCtrl()
                ' note that any changes to the properties are done in the eWebDiff.eWebDiffCtrl COM object
                If (IsXMLDoc) Then
                    ' be aware that we can't do xml diff until we get an All Modes license
                    'Throw New Exception("No server-side XML diff support yet")
                    ' temporary workaround to compare it in HTML mode for now:
                    IsXMLDoc = False
                    newXHTML = "<html>" & newXHTML.Replace("<", "&lt;").Replace(">", "&gt;") & "</html>"
                    oldXHTML = "<html>" & oldXHTML.Replace("<", "&lt;").Replace(">", "&gt;") & "</html>"
                End If
                DiffResults.Text = diffCtrl.DoDiff(oldXHTML, newXHTML, IsXMLDoc)
            Catch ex As Exception
                ' log it as a msg in event log so support can ask for it
                EkException.WriteToEventLog("Unable to use eWebDiffCtrl control; reverting to control download: " + _
                                          Environment.NewLine() + ex.ToString() _
                            , Diagnostics.EventLogEntryType.Error)
                ' if we get an exception, do it the old way
                ShowDiffPanel.Visible = True
                Dim m_refSiteAPI As New SiteAPI
                Dim settings_data As SettingsData
                settings_data = m_refSiteAPI.GetSiteVariables(m_refContApi.UserId)
                If (Not (IsNothing(settings_data))) Then
                    LicKey.Text = settings_data.LicenseKey
                End If
                'If (Not IsXMLDoc) Then
                '    csbasecontent.Value = Server.HtmlEncode(oldXHTML)
                '    csnewcontent.Value = Server.HtmlEncode(newXHTML)
                'Else
                ' yes, this is weird...dunno why we can't just htmlencode both and decode it on the other side
                csbasecontent.Value = oldXHTML
                csnewcontent.Value = newXHTML
                'End If
                Exit Sub
            End Try
            Dim viewxslt As String = Server.MapPath("xslt/XmlViewXslt.xsl")
            If (IsXMLDoc) Then
                ' XmlViewXslt garbles the bookstore sample form pretty printing
                'DiffOrigContent.Text = m_refContApi.TransformXSLT(oldXHTML, viewxslt).Replace("&gt;&lt;", "&gt; &lt;")
                'DiffnewXHTML.Text = m_refContApi.TransformXSLT(newXHTML, viewxslt).Replace("&gt;&lt;", "&gt; &lt;")
                DiffOrigContent.Text = oldXHTML.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&gt;&lt;", "&gt; &lt;")
                DiffNewContent.Text = newXHTML.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&gt;&lt;", "&gt; &lt;")
            Else
                DiffOrigContent.Text = oldXHTML
                DiffNewContent.Text = newXHTML
            End If
            ShowDiffPanel.Visible = False
            ShowSvrDiffPanel.Visible = True
            Util_SetLabels()
        Else
            HideDiffPanel.Visible = True
        End If
    End Sub

    Private Sub Util_SetLabels()
        Dim refSiteApi As New SiteAPI
        Dim refMsg As Common.EkMessageHelper = refSiteApi.EkMsgRef
        lblTabOrig.Text = refMsg.GetMessage("lbl webdiff taborig")
        lblTabNew.Text = refMsg.GetMessage("lbl webdiff tabnew")
        lblTabDiff.Text = refMsg.GetMessage("lbl webdiff tabdiff")
        lblLegend.Text = refMsg.GetMessage("lbl webdiff legend")
        lblLegendAdded.Text = refMsg.GetMessage("lbl webdiff legendadded")
        lblLegendDeleted.Text = refMsg.GetMessage("lbl webdiff legenddeleted")
    End Sub

    Private Function RemoveHTML(ByVal strText As String) As String
        Dim TAGLIST As String = ";em;span;u;a;"
        Const BLOCKTAGLIST As String = ";APPLET;"
        Dim nPos1 As Object
        Dim nPos2 As Object
        Dim nPos3 As Object
        Dim strResult As String = ""
        Dim strTagName As Object
        Dim bRemove As Object
        Dim bSearchForBlock As Object
        nPos1 = InStr(strText, "<")
        Do While nPos1 > 0
            nPos2 = InStr(nPos1 + 1, strText, ">")
            If nPos2 > 0 Then
                strTagName = Mid(strText, nPos1 + 1, nPos2 - nPos1 - 1)
                strTagName = Replace(Replace(strTagName, vbCr, " "), vbLf, " ")

                nPos3 = InStr(strTagName, " ")
                If nPos3 > 0 Then
                    strTagName = Left(strTagName, nPos3 - 1)
                End If

                If Left(strTagName, 1) = "/" Then
                    strTagName = Mid(strTagName, 2)
                    bSearchForBlock = False
                Else
                    bSearchForBlock = True
                End If

                If InStr(1, TAGLIST, ";" & strTagName & ";", vbTextCompare) > 0 Then
                    bRemove = True
                    If bSearchForBlock Then
                        If InStr(1, BLOCKTAGLIST, ";" & strTagName & ";", vbTextCompare) > 0 Then
                            nPos2 = Len(strText)
                            nPos3 = InStr(nPos1 + 1, strText, "</" & strTagName, vbTextCompare)
                            If nPos3 > 0 Then
                                nPos3 = InStr(nPos3 + 1, strText, ">")
                            End If

                            If nPos3 > 0 Then
                                nPos2 = nPos3
                            End If
                        End If
                    End If
                Else
                    bRemove = False
                End If

                If bRemove Then
                    strResult = strResult & Left(strText, nPos1 - 1)
                    strText = Mid(strText, nPos2 + 1)
                Else
                    strResult = strResult & Left(strText, nPos1)
                    strText = Mid(strText, nPos1 + 1)
                End If
            Else
                strResult = strResult & strText
                strText = ""
            End If

            nPos1 = InStr(strText, "<")
        Loop
        strResult = strResult & strText
        strResult = Replace(strResult, "&#160;", " ")

        ' also run Tidy on the text
        Dim tidydoc As TidyNet.Tidy = New TidyNet.Tidy()
        tidydoc.Options.RawOut = False
        tidydoc.Options.CharEncoding = TidyNet.CharEncoding.UTF8
        tidydoc.Options.DocType = TidyNet.DocType.Omit
        tidydoc.Options.TidyMark = False
        tidydoc.Options.Word2000 = True
        tidydoc.Options.QuoteNbsp = True
        tidydoc.Options.QuoteAmpersand = True
        tidydoc.Options.NumEntities = False
        tidydoc.Options.QuoteMarks = True
        tidydoc.Options.Xhtml = False
        tidydoc.Options.MakeClean = True
        Dim messageCollection As New TidyNet.TidyMessageCollection()
        Dim tidyin As New System.IO.MemoryStream()
        Dim tidyout As New System.IO.MemoryStream()
        If (strResult Is Nothing) Then
            strResult = "<p></p>"
        End If
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(strResult)
        tidyin.Write(byteArray, 0, byteArray.Length)
        tidyin.Position = 0
        tidydoc.Parse(tidyin, tidyout, messageCollection)
        tidyout.Position = 0
        Dim strTidyResult As String = Encoding.UTF8.GetString(tidyout.ToArray())
        tidyout.Close()
        If ((strTidyResult = "") And (messageCollection.Errors > 0)) Then
            Dim msg As TidyNet.TidyMessage
            For Each msg In messageCollection
                If (msg.Level = TidyNet.MessageLevel.Error) Then
                    strTidyResult = strTidyResult & msg.ToString() & "<BR>"
                End If
            Next
        Else
            strResult = strTidyResult
        End If

        RemoveHTML = strResult
    End Function
End Class
