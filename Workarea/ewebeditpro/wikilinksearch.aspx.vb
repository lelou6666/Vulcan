Imports Ektron.Cms.WebSearch.SearchData
Imports Ektron.Cms.API.Search
Imports ektron.Cms
Partial Class wikilinksearch
    Inherits System.Web.UI.Page

    Protected m_commonApi As New CommonApi
    Protected m_refMsg As Common.EkMessageHelper

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Response.ContentType = "text/xml"
        Page.Response.Clear()
        Page.Response.BufferOutput = True

        Dim text As String = ""
        Dim pageNumber As Integer = 1
        Dim totalPages As Integer = 0
        Dim MaxResults As Integer = 0
        Dim iLoop As Integer = 1
        Dim strID As String = ""
        Dim strFields As StringBuilder = New StringBuilder()
        Dim strOnclick As String = ""
        Dim contentId As Long = 0
        Dim selectedId As Long = -1
        Dim languageID As Integer = m_commonApi.RequestInformationRef.ContentLanguage
        m_refMsg = m_commonApi.EkMsgRef

        Dim content_api As Ektron.Cms.ContentAPI = Nothing
        content_api = New Ektron.Cms.ContentAPI()
        If content_api.GetCookieValue("LastValidLanguageID") <> "" AndAlso Convert.ToInt32(content_api.GetCookieValue("LastValidLanguageID") <> -1) Then
            languageID = Convert.ToInt32(content_api.GetCookieValue("LastValidLanguageID"))
        End If


        text = Request.QueryString("text")
        If (Request.QueryString("pnum") <> "" AndAlso CInt(Request.QueryString("pnum") > 0)) Then
            pageNumber = Request.QueryString("pnum")
        End If
        If (Request.QueryString("cid") IsNot Nothing AndAlso Request.QueryString("cid") <> "") Then
            contentId = Request.QueryString("cid")
        End If
        If (Request.QueryString("selectedid") IsNot Nothing AndAlso Request.QueryString("selectedid") <> "") Then
            selectedId = Request.QueryString("selectedid")
        End If
        Dim search As Ektron.Cms.API.Search.SearchManager = New Ektron.Cms.API.Search.SearchManager()
        Dim requestData As SearchRequestData = New SearchRequestData()
        requestData.SearchFor = Ektron.Cms.WebSearch.SearchDocumentType.all
        requestData.SearchText = text
        requestData.PageSize = 10
        requestData.LanguageID = languageID
        requestData.CurrentPage = pageNumber
        Dim resultCount As Integer
        Dim result() As SearchResponseData = search.Search(requestData, HttpContext.Current, resultCount)
        Dim str As StringBuilder = New StringBuilder()
        Dim strRet As StringBuilder = New StringBuilder()
        Dim tmpCount As Integer = 0
        Dim strLink As String = ""
        Dim arLink() As String = Nothing

        MaxResults = requestData.PageSize

        Dim backColor As String = "silver"
        If (resultCount <> 0) Then

            str.Append("<table width=""100%"" class=""ektronGrid"">")
            For Each data As SearchResponseData In result
                strLink = ""
                strID = "ek_sel_cont" & iLoop

                strLink = data.QuickLink.Replace("'", "\'").Replace("//", "/")
                If (strLink.ToLower.IndexOf("window.open") < 0) Then
                    arLink = strLink.Split("?")
                    If (arLink.Length > 1) Then
                        strLink = arLink(0)
                        arLink = arLink(1).Split("&amp;")
                        For Each val As String In arLink
                            If (val.IndexOf("terms=") = -1) Then
                                If strLink = "" Then
                                    strLink = val
                                Else
                                    If (strLink.IndexOf("?") < 0) Then
                                        strLink = strLink & "?" & val
                                    Else
                                        strLink = strLink & "&" & val
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If

                strOnclick = "SelectContent('" & strID & "','" & strLink & "')"
                str.Append("<tr><td valign=""top"" style=""width:1%;"" valign=""top"">")
                str.Append("<input type=""radio"" ")
                If (selectedId <> -1 AndAlso selectedId = data.ContentID) Then
                    str.Append(" checked=""true"" ")
                End If
                If data.ContentID = contentId Then
                    str.Append(" disabled ")
                End If
                str.Append("onclick=""" & strOnclick & """ id=""")
                str.Append(strID)
                str.Append(""" name=""ek_sel_cont""/></td><td valign=""top"">")
                str.Append("<span onclick=""" & strOnclick & """ class=""title"">")
                str.Append(data.Title).Append("</span><br/>")
                str.Append("<span onclick=""SelectContent('" & strID & "','" & strLink & "')"" class=""summary"">")
                If (data.ContentType <> 2 And data.ContentType <> 4) Then
                    str.Append(data.Summary.Replace("<p> </p>", "").Replace("<p>&nbsp;</p>", "").Replace("<p>&#160;</p>", ""))
                End If
                str.Append("</span></td></tr>")
                strFields.Append(",").Append(strID)
                iLoop += 1
                'If data.ContentID <> contentId Then
                'Else
                '    resultCount = resultCount - 1
                'End If
            Next
        End If
        If (resultCount > 0 And MaxResults > 0) Then
            If (resultCount Mod MaxResults = 0) Then
                totalPages = resultCount / MaxResults
            Else
                tmpCount = (resultCount / MaxResults)
                If (tmpCount * MaxResults < resultCount) Then
                    totalPages = tmpCount + 1
                Else
                    totalPages = tmpCount
                End If
            End If
        End If
        str.Append("</table>")

        If totalPages = 0 Then
            strRet = New StringBuilder()
            strRet.Append("<content>")
            strRet.Append("<table style=""width:100%"" border=""0"" cellpadding=""0"" cellspacing=""0"">")
            strRet.Append("<tr><td>" & m_refMsg.GetMessage("alt no related content") & "</td></tr>")
            strRet.Append("</table>")
            strRet.Append("</content>")
            strRet.Append("<totalPages>").Append(totalPages).Append("</totalPages>")
        Else
            strRet.Append("<content>")
            strRet.Append("<div class=""header"">" & m_refMsg.GetMessage("lbl total") & ": ").Append(resultCount).Append("<br/>")
            strRet.Append("" & m_refMsg.GetMessage("page lbl") & ": ").Append(pageNumber).Append(" " & m_refMsg.GetMessage("lbl of") & " ").Append(totalPages).Append("</div>")
            strRet.Append(str.ToString())
            strRet.Append("</content>")
            strRet.Append("<totalPages>").Append(totalPages).Append("</totalPages>")
        End If
        Response.Write(strRet.ToString())
    End Sub

End Class
