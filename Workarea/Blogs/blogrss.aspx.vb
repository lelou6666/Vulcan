Imports Ektron.Cms.Controls

Partial Class Blogs_blogrss
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.ContentType = "text/xml"
        If Request.QueryString("blog") <> "" Then
            Dim beEntries As New BlogEntries()
            beEntries.BlogID = Request.QueryString("blog")
            beEntries.Page = Me
            ltr_rss.Text = beEntries.GetRssFeed
        End If
    End Sub
End Class
