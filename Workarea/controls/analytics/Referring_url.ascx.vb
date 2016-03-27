
Partial Class controls_analytics_Referring_url
    Inherits Ektron.Cms.AnalyticsBase

    Protected common As New Ektron.Cms.CommonApi()
    Private Domain As String = ""

    Public Overrides Sub Initialize()

        Me.GridView1.PageSize = PageSize
        Me.GridView2.PageSize = PageSize
        Me.GridView3.PageSize = PageSize

        lbl_total_hits.Text = common.EkMsgRef.GetMessage("total views")
        lbl_total_visitors.Text = common.EkMsgRef.GetMessage("total visitors")
        lbl_hits_per_visitor.Text = common.EkMsgRef.GetMessage("views to visitors")
        lbl_new_visitors.Text = common.EkMsgRef.GetMessage("new visitors")
        lbl_returning_visitors.Text = common.EkMsgRef.GetMessage("returning visitors")
        lbl_hits_vs_visitors.Text = common.EkMsgRef.GetMessage("views to visitors")
        lbl_new_vs_returning_visitors.Text = common.EkMsgRef.GetMessage("new to returning visitors")

        Me.stats_aggr.Visible = False
        Me.Image1.Visible = False

        If (Request.QueryString("id") Is Nothing) Then
            Description = common.EkMsgRef.GetMessage("lbl Referring Domains")
            AnalyticsData.Clear()
            Fill("SELECT referring_url, COUNT(referring_url) as Referrals FROM content_hits_tbl " & _
            "WHERE " & DateClause & " AND (visit_type = 0 OR visit_type=1) AND referring_url != '' GROUP BY referring_url")

            If (AnalyticsData.Tables(0).Rows.Count = 0) Then
                ErrMsg.Visible = True
                ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range")
            End If

            CType(GridView1.Columns(0), HyperLinkField).DataNavigateUrlFormatString = common.ApplicationPath & "/ContentAnalytics.aspx?type=referring&id={0}"

            AnalyticsDataView.Table = AnalyticsData.Tables(0)

            GridView1.DataSource = AnalyticsDataView
            GridView1.DataBind()

            Image1.Visible = False
        Else

            Domain = Request.QueryString("id")
            Description = common.EkMsgRef.GetMessage("stats on") & " '" & Domain & "'"

            Dim target As String = common.ApplicationPath & "ContentAnalytics.aspx?type=referring"
            'Me.navBar.Text = "<ul id=""tabnav"">"
            'Me.navBar.Text &= "<li><a href=" & target & "&report=1&id=" & Domain & ">" & common.EkMsgRef.GetMessage("referrer stats") & "</a></li>"
            'Me.navBar.Text &= "<li><a href=" & target & "&report=2&id=" & Domain & ">" & common.EkMsgRef.GetMessage("referrer activity") & "</a></li>"
            'Me.navBar.Text &= "<li><a href=" & target & "&report=3&id=" & Domain & ">" & common.EkMsgRef.GetMessage("top landing pages") & "</a></li></ul>"
            'Me.navBar.Text &= "<li><a href=" & target & "&report=4&id=" & Domain & ">" & "Top Landing Content" & "</a></li></ul>"

            Dim t_stats As String = "tab_disabled"
            Dim t_activity As String = "tab_disabled"
            Dim t_landing As String = "tab_disabled"
            Dim t_pages As String = "tab_disabled"

            Dim reportType As String
            reportType = Request.QueryString("report")
            If (reportType Is Nothing) Then
                reportType = "1"
            End If

            Select Case (reportType)
                Case "1"
                    t_stats = "tab_actived"
                Case "2"
                    t_activity = "tab_actived"
                Case "3"
                    t_landing = "tab_actived"
                Case "4"
                    t_pages = "tab_actived"
            End Select

            Me.navBar.Text = "<table height=""20"" style=""BACKGROUND-COLOR:white"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%""><tr>"
            Me.navBar.Text &= "<td class=""" & t_stats & """ width=""1%"" nowrap><b><a border=""0"" style=""text-decoration:none;"" href=" & target & "&report=1&id=" & Domain & ">" & common.EkMsgRef.GetMessage("referrer stats") & "</a></b></td>"
            Me.navBar.Text &= "<td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>"
            Me.navBar.Text &= "<td class=""" & t_activity & """ width=""1%"" nowrap><b><a border=""0"" style=""text-decoration:none;"" href=" & target & "&report=2&id=" & Domain & ">" & common.EkMsgRef.GetMessage("referrer activity") & "</a></b></td>"
            Me.navBar.Text &= "<td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>"
            Me.navBar.Text &= "<td class=""" & t_pages & """ width=""1%"" nowrap><b><a border=""0"" style=""text-decoration:none;"" href=" & target & "&report=4&id=" & Domain & ">" & "Paths" & "</a></b></td>"
            Me.navBar.Text &= "<td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>"
            Me.navBar.Text &= "<td class=""" & t_landing & """ width=""1%"" nowrap><b><a border=""0"" style=""text-decoration:none;"" href=" & target & "&report=3&id=" & Domain & ">" & common.EkMsgRef.GetMessage("top landing pages") & "</a></b></td>"
            Me.navBar.Text &= "<td class=""tab_last"" width=""91%"" nowrap>&nbsp;</td>"
            Me.navBar.Text &= "</tr></table>"

            Dim report As Integer = 1

            If (Not Request.QueryString("report") Is Nothing) Then
                Try
                    report = Convert.ToInt32(Request.QueryString("report"))
                Catch ex As Exception

                End Try
            End If

            Select Case (report)
                Case 1 ' Quick Statistics
                    stats_aggr.Visible = True
                    InitStatsAggr(Domain)
                Case 2 ' Referrer Activity
                    Image1.Visible = True
                    Image1.ImageUrl = common.ApplicationPath & "ContentRatingGraph.aspx?type=time&view=" & CurrentView & "&res_type=referring&res=" & Domain & "&EndDate=" & Page.Server.UrlEncode(EndDateTime.ToString())
                Case 3 ' Top Landing Pages
                    AnalyticsData.Clear()
                    Fill("SELECT url, COUNT(url) as Landings FROM content_hits_tbl " & _
                    "WHERE " & DateClause & " AND referring_url = '" & Domain & "' AND (visit_type = 0 OR visit_type = 1) GROUP BY url")

                    If (AnalyticsData.Tables(0).Rows.Count = 0) Then
                        ErrMsg.Visible = True
                        ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range")
                    End If

                    CType(GridView2.Columns(0), HyperLinkField).DataNavigateUrlFormatString = common.ApplicationPath & "ContentAnalytics.aspx?type=page&id={0}"


                    AnalyticsDataView.Table = AnalyticsData.Tables(0)
                    GridView2.DataSource = AnalyticsDataView
                    GridView2.DataBind()
                Case 4 ' Top Landing Content
                    AnalyticsData.Clear()
                    Fill("SELECT referring_url_path, COUNT(referring_url_path) as Landings FROM content_hits_tbl " & _
                    "WHERE " & DateClause & " AND referring_url = '" & Domain & "' AND referring_url_path != '' AND (visit_type = 0 OR visit_type = 1) GROUP BY referring_url_path")

                    If (AnalyticsData.Tables(0).Rows.Count = 0) Then
                        ErrMsg.Visible = True
                        ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range")
                    End If

                    AnalyticsDataView.Table = AnalyticsData.Tables(0)
                    GridView3.DataSource = AnalyticsData
                    GridView3.DataBind()
            End Select
        End If

    End Sub

    Private Sub InitStatsAggr(ByVal ref_url As String)
        Fill("SELECT COUNT(visitor_id) AS HITS, COUNT(DISTINCT visitor_id) AS VISITORS, " & _
         "(SELECT COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE visit_type = 0 AND " & DateClause & " AND referring_url = '" & ref_url & "') AS NEW, " & _
         "(SELECT COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE visit_type = 1 AND " & DateClause & " AND referring_url = '" & ref_url & "') AS RETURNING FROM content_hits_tbl WHERE " & DateClause & _
         " AND referring_url = '" & ref_url & "'")

        num_total_hits.Text = AnalyticsData.Tables(0).Rows(0)(0).ToString()
        num_total_visitors.Text = AnalyticsData.Tables(0).Rows(0)(1).ToString()

        num_new_visitors.Text = AnalyticsData.Tables(0).Rows(0)(2).ToString()
        num_returning_visitors.Text = AnalyticsData.Tables(0).Rows(0)(3).ToString()

        Dim ratio As Double = 0

        Try
            ratio = Double.Parse(num_total_hits.Text)
            ratio = Math.Round(ratio / Double.Parse(num_total_visitors.Text), 2)
            If (Double.Parse(num_total_visitors.Text) = 0) Then
                ratio = 0
            End If
        Catch ex As Exception
            ratio = 0
        End Try

        If (ratio = 0) Then
            num_hits_per_visitor.Text = "N/A"
        Else
            num_hits_per_visitor.Text = ratio.ToString()
        End If

        graph_hits_per_visitor.ImageUrl = common.ApplicationPath & "ContentRatingGraph.aspx?type=pie&r1=" & Me.num_total_hits.Text & "&r2=" & Me.num_total_visitors.Text & "&size=100"
        graph_new_vs_returning_visitors.ImageUrl = common.ApplicationPath & "ContentRatingGraph.aspx?type=pie&r1=" & Me.num_new_visitors.Text & "&r2=" & Me.num_returning_visitors.Text & "&size=100"

    End Sub

    Protected Sub GridView1_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridView1.Sorting
        If (SortOrder = SortDirection.Descending) Then
            SortExpression = e.SortExpression & " DESC"
        Else
            SortExpression = e.SortExpression & " ASC"
        End If

        AnalyticsDataView.Sort = SortExpression
        GridView1.PageIndex = PageIndex

        GridView1.DataSource = AnalyticsDataView
        GridView1.DataBind()
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        PageIndex = e.NewPageIndex

        GridView1.PageIndex = PageIndex
        AnalyticsDataView.Sort = SortExpression

        GridView1.DataSource = AnalyticsDataView
        GridView1.DataBind()
    End Sub

    Protected Sub GridView2_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridView2.Sorting
        If (SortOrder = SortDirection.Descending) Then
            SortExpression = e.SortExpression & " DESC"
        Else
            SortExpression = e.SortExpression & " ASC"
        End If

        AnalyticsDataView.Sort = SortExpression
        GridView2.PageIndex = PageIndex

        GridView2.DataSource = AnalyticsDataView
        GridView2.DataBind()
    End Sub

    Protected Sub GridView2_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView2.PageIndexChanging
        PageIndex = e.NewPageIndex

        GridView2.PageIndex = PageIndex
        AnalyticsDataView.Sort = SortExpression

        GridView2.DataSource = AnalyticsDataView
        GridView2.DataBind()
    End Sub

    Protected Sub GridView3_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridView3.Sorting
        If (SortOrder = SortDirection.Descending) Then
            SortExpression = e.SortExpression & " DESC"
        Else
            SortExpression = e.SortExpression & " ASC"
        End If

        AnalyticsDataView.Sort = SortExpression
        GridView3.PageIndex = PageIndex

        GridView3.DataSource = AnalyticsDataView
        GridView3.DataBind()
    End Sub

    Protected Sub GridView3_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView3.PageIndexChanging
        PageIndex = e.NewPageIndex

        GridView3.PageIndex = PageIndex
        AnalyticsDataView.Sort = SortExpression

        GridView3.DataSource = AnalyticsDataView
        GridView3.DataBind()
    End Sub
End Class
