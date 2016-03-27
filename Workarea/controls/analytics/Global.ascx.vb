
Partial Class controls_analytics_Global
    Inherits Ektron.Cms.AnalyticsBase

    Protected common As New Ektron.Cms.CommonApi()

    Public Overrides Sub Initialize()

        lbl_total_hits.Text = common.EkMsgRef.GetMessage("total views")
        lbl_total_visitors.Text = common.EkMsgRef.GetMessage("total visitors")
        lbl_hits_per_visitor.Text = common.EkMsgRef.GetMessage("views to visitors")
        lbl_new_visitors.Text = common.EkMsgRef.GetMessage("new visitors")
        lbl_returning_visitors.Text = common.EkMsgRef.GetMessage("returning visitors")
        lbl_hits_vs_visitors.Text = common.EkMsgRef.GetMessage("views to visitors")
        lbl_new_vs_returning_visitors.Text = common.EkMsgRef.GetMessage("new to returning visitors")

        Dim target As String = common.ApplicationPath & "ContentAnalytics.aspx?type=global"

        Description = common.EkMsgRef.GetMessage("site stats")

        Dim report As Integer = 1

        If (Not Request.QueryString("report") Is Nothing) Then
            Try
                report = Convert.ToInt32(Request.QueryString("report"))
            Catch ex As Exception
                report = 1
            End Try
        End If


        ' Start off pessamistic - make everything invisible
        ByTimeGraph.Visible = False
        stats_aggr.Visible = False

        Select Case (report)
            Case 1 ' Quick Statistics
                stats_aggr.Visible = True
                InitStatsAggr()
            Case 2 ' By Time
                Description = common.EkMsgRef.GetMessage("site activity")
                ByTimeGraph.Visible = True
                graph_key.Text = "       <table border=""0""><tr><td width=""20px"" height=""10px"" bgcolor=""red"">&nbsp;</td><td>" & _
                common.EkMsgRef.GetMessage("views lbl") & "</td></tr><tr><td width=""20px"" height=""10px"" bgcolor=""blue"">&nbsp;</td><td>" & _
                common.EkMsgRef.GetMessage("visitors lbl") & "</td></tr></table>"
                ByTimeGraph.Text = "<img src=""" & common.ApplicationPath & "ContentRatingGraph.aspx?type=time&view=" & CurrentView.ToLower() & "&EndDate=" & Page.Server.UrlEncode(EndDateTime.ToString()) & """ />"
            Case 3 ' Popular Content
            Case 4 ' Popular Pages
            Case 5 ' Popular URLs
        End Select



    End Sub
    Private Sub InitStatsAggr()
        Dim visitorSelect As String = String.Empty

        AnalyticsData.Tables.Clear()
        Fill("SELECT COUNT(visitor_id) AS HITS, COUNT(DISTINCT visitor_id) AS VISITORS FROM content_hits_tbl WHERE " & DateClause)
        num_total_hits.Text = AnalyticsData.Tables(0).Rows(0)(0).ToString()
        num_total_visitors.Text = AnalyticsData.Tables(0).Rows(0)(1).ToString()
        AnalyticsData.Tables.Clear()
        Fill("SELECT COUNT(DISTINCT visitor_id) AS NEW FROM content_hits_tbl WHERE visit_type = 0 AND " & DateClause)
        num_new_visitors.Text = AnalyticsData.Tables(0).Rows(0)(0).ToString()
        AnalyticsData.Tables.Clear()
        'Fill("SELECT COUNT(DISTINCT visitor_id) AS RETURNING FROM content_hits_tbl WHERE visit_type = 1 AND " & DateClause)
        visitorSelect = "SELECT COUNT(DISTINCT visitor_id) AS RETURNING " & _
                        " FROM content_hits_tbl " & _
                       " WHERE visit_type = 1" & _
                         " AND visitor_id NOT IN (SELECT DISTINCT visitor_id FROM content_hits_tbl WHERE visit_type = 0 AND " & DateClause & ")" & _
                         " AND " & DateClause
        Fill(visitorSelect)
        num_returning_visitors.Text = AnalyticsData.Tables(0).Rows(0)(0).ToString()

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
End Class
