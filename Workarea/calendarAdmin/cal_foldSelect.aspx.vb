Imports Ektron.Cms
Imports Ektron.Cms.Common
Partial Class cal_foldSelect
    Inherits System.Web.UI.Page
    Protected m_siteRef As New SiteAPI
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        JSInc.Text = "<link type=""text/css"" href=""" & m_siteRef.AppPath & "csslib/ektron.workarea.css""/>"
        Dim foldCol As New Collection
        Dim outSB As New System.Text.StringBuilder
        Dim cAPI As New CalendarAPI(m_siteRef.RequestInformationRef)
        Dim fDat As New FolderData
        fDat = cAPI.GetFolderWithChildren(0)
        TestDate.Text &= pShowFolders(fDat).ToString
    End Sub
    Private Function pShowFolders(ByVal inFold As FolderData) As System.Text.StringBuilder
        Dim ret As New System.Text.StringBuilder
        Dim z As Integer
        ret.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"">" & vbCrLf)
        ret.Append("<tr><td colspan=2 class=""info"">")
        ret.Append("&#160;<A href=""JavaScript:folderClick('" & inFold.Id & "','" & inFold.NameWithPath.Replace("'", "\'").Replace("\", "\\").Replace("/", "\/") & "')"">" & inFold.Name & "</a></td></tr>" & vbCrLf)
        If (Not inFold.ChildFolders Is Nothing) Then
            For z = 0 To (inFold.ChildFolders.Length - 1)
                ret.Append(pShowFolder(inFold.ChildFolders(z), 1))
            Next
        End If
        ret.Append("</table>")
        Return (ret)
    End Function
    Private Function pShowFolder(ByVal inFold As FolderData, ByVal level As Integer) As System.Text.StringBuilder
        Dim ret As New System.Text.StringBuilder
        Dim z As Integer
        ret.Append("<tr><td>&#160;&#160;</td><td class=""info"" style=""border-left: #000000 1px solid ;"">")
        ret.Append("<span style=""text-decoration: line-through ;"">")
        For z = 1 To level
            ret.Append("&#160;&#160;")
        Next
        ret.Append("</span>")
        ret.Append("&#160;<A href=""JavaScript:folderClick('" & inFold.Id & "','" & inFold.NameWithPath.Replace("'", "\'").Replace("\", "\\").Replace("/", "\/") & "')"">" & inFold.Name & "</a></td></tr>" & vbCrLf)
        If (Not inFold.ChildFolders Is Nothing) Then
            For z = 0 To (inFold.ChildFolders.Length - 1)
                ret.Append(pShowFolder(inFold.ChildFolders(z), level + 1))
            Next
        End If
        Return (ret)
    End Function
End Class