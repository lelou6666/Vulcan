
Partial Class controls_analytics_ContentReports
    Inherits Ektron.Cms.AnalyticsBase

    Protected common As New Ektron.Cms.CommonApi()
    Protected m_refContentApi As New Ektron.Cms.ContentAPI()


    Public Overrides Sub Initialize()
        navBar.Visible = False
        Image1.Visible = False
        stats_aggr.Visible = False

        GridView1.PageSize = PageSize
        auditContent.PageSize = PageSize

        lbl_total_hits.Text = common.EkMsgRef.GetMessage("total views")
        lbl_total_visitors.Text = common.EkMsgRef.GetMessage("total visitors")
        lbl_hits_per_visitor.Text = common.EkMsgRef.GetMessage("views to visitors")
        lbl_new_visitors.Text = common.EkMsgRef.GetMessage("new visitors")
        lbl_returning_visitors.Text = common.EkMsgRef.GetMessage("returning visitors")
        lbl_hits_vs_visitors.Text = common.EkMsgRef.GetMessage("views to visitors")
        lbl_new_vs_returning_visitors.Text = common.EkMsgRef.GetMessage("new to returning visitors")

        If (Request.QueryString("id") Is Nothing) Then
            Description = common.EkMsgRef.GetMessage("top content")
            AnalyticsData.Clear()
            Fill("SELECT content.content_title as content_title, COUNT(DISTINCT content_hits_tbl.visitor_id) as Visits, " & _
            "COUNT(content_hits_tbl.visitor_id) as Views, content_hits_tbl.content_id as content_id FROM content_hits_tbl " & _
            "LEFT JOIN content ON content.content_id = content_hits_tbl.content_id WHERE content.content_language = " & common.DefaultContentLanguage & _
            " AND " & DateClause & _
            "GROUP BY content_hits_tbl.content_id, content.content_title ORDER BY Visits DESC")

            If (AnalyticsData.Tables(0).Rows.Count = 0) Then
                ErrMsg.Visible = True
                ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range")
            End If

            CType(GridView1.Columns(0), HyperLinkField).DataNavigateUrlFormatString = common.ApplicationPath & "ContentAnalytics.aspx?type=content&id={0}"
            ctype(gridview1.Columns(0), HyperLinkField).SortExpression = "content_title"

            AnalyticsDataView.Table = AnalyticsData.Tables(0)

            GridView1.DataSource = AnalyticsDataView
            GridView1.DataBind()

            Me.Image1.Visible = False
        Else
            Dim content_id As Long

            Try
                content_id = Convert.ToInt64(Request.QueryString("id"))
            Catch ex As Exception
                content_id = 0
            End Try

            Dim t_stats As String = "tab_disabled"
            Dim t_activity As String = "tab_disabled"
            Dim t_audit As String = "tab_disabled"

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
                    t_audit = "tab_actived"
            End Select

            'TODO: Ross - These need to be converted to jQuery tabs
            navBar.Visible = True
            Dim target As String = common.ApplicationPath & "ContentAnalytics.aspx?type=content"
            'navBar.Text = "<ul id=""tabnav"">"
            'navBar.Text &= "<li><a href=" & target & "&report=1&id=" & content_id & ">" & common.EkMsgRef.GetMessage("content stats") & "</a></li>"
            'navBar.Text &= "<li><a href=" & target & "&report=2&id=" & content_id & ">" & common.EkMsgRef.GetMessage("content activity") & "</a></li>"
            'navBar.Text &= "<li><a href=" & target & "&report=3&id=" & content_id & ">" & common.EkMsgRef.GetMessage("audit content") & "</a></li></ul>"
            Dim content_data As Ektron.Cms.ContentData = m_refContentApi.GetContentById(content_id)
            Me.navBar.Text = "<p>" & content_data.Title & "</p>"
            Me.navBar.Text &= "<table height=""20"" width=""100%"">"
            Me.navBar.Text &= "<tr>"
            Me.navBar.Text &= "<td class=""" & t_stats & """ width=""1%"" nowrap><a style=""text-decoration:none;"" href=" & target & "&report=1&id=" & content_id & ">&nbsp;" & common.EkMsgRef.GetMessage("content stats") & "&nbsp;</a></td>"
            Me.navBar.Text &= "<td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>"
            Me.navBar.Text &= "<td class=""" & t_activity & """ width=""1%"" nowrap><a style=""text-decoration:none;"" href=" & target & "&report=2&id=" & content_id & ">&nbsp;" & common.EkMsgRef.GetMessage("content activity") & "&nbsp;</a></td>"
            Me.navBar.Text &= "<td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>"
            Me.navBar.Text &= "<td class=""" & t_audit & """ width=""1%"" nowrap><a style=""text-decoration:none;"" href=" & target & "&report=3&id=" & content_id & ">&nbsp;" & common.EkMsgRef.GetMessage("audit content") & "&nbsp;</a></td>"
            Me.navBar.Text &= "<td class=""tab_last"" width=""91%"" nowrap>&nbsp;</td>"
            Me.navBar.Text &= "</tr>"
            Me.navBar.Text &= "</table>"

            Dim report As Integer = 1

            If (Not Request.QueryString("report") Is Nothing) Then
                Try
                    report = Convert.ToInt32(Request.QueryString("report"))
                Catch ex As Exception

                End Try
            End If
            Description = "ContentID=" & content_id

            Select Case (report)
                Case 1 ' Quick Statistics
                    Description = "Statistics for ContentID=" & content_id
                    stats_aggr.Visible = True
                    InitStatsAggr(content_id)
                Case 2 ' By Time
                    Description = "Activity by time for ContentID=" & content_id
                    Image1.Visible = True
                    graph_key.Text = "       <table border=""0""><tr><td width=""20px"" height=""10px"" bgcolor=""red"">&nbsp;</td><td>" & _
                    "Views" & "</td></tr><tr><td width=""20px"" height=""10px"" bgcolor=""blue"">&nbsp;</td><td>" & _
                    "Visitors" & "</td></tr></table>"
                    Image1.ImageUrl = common.ApplicationPath & "ContentRatingGraph.aspx?type=time&view=" & CurrentView & "&res_type=content&res=" & content_id & "&EndDate=" & Page.Server.UrlEncode(EndDateTime.ToString())
                Case 3 ' Audit Content
                    Description = "User Views for ContentID=" & content_id
                    InitAuditContent(content_id)
            End Select
        End If
    End Sub

    Private Sub InitStatsAggr(ByVal content_id As Long)
        Dim str As String = "SELECT COUNT(visitor_id) AS HITS, COUNT(DISTINCT visitor_id) AS VISITORS, " & _
         "(SELECT COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE visit_type = 0 AND " & DateClause & ") AS NEW, " & _
         "(SELECT COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE visit_type = 1 AND " & DateClause & ") AS RETURNING FROM content_hits_tbl WHERE " & DateClause & _
         " AND content_id = " & content_id
        Fill(str)

        If (AnalyticsData.Tables(0).Rows.Count = 0) Then
            ErrMsg.Visible = True
            ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range")
        End If

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

        graph_hits_per_visitor.ImageUrl = common.ApplicationPath & "ContentRatingGraph.aspx?type=pie&r1=" & num_total_hits.Text & "&r2=" & num_total_visitors.Text & "&size=100"
        graph_new_vs_returning_visitors.ImageUrl = common.ApplicationPath & "ContentRatingGraph.aspx?type=pie&r1=" & num_new_visitors.Text & "&r2=" & num_returning_visitors.Text & "&size=100"
    End Sub

    Private Sub InitAuditContent(ByVal content_id As Long)
        AnalyticsData.Tables.Clear()
        Fill("SELECT DISTINCT users.user_name as UserName, users.first_name as FirstName, users.last_name as LastName, MAX(content_hits_tbl.hit_date) as HitDate " & _
        "FROM content_hits_tbl INNER JOIN users ON content_hits_tbl.user_id = users.user_id WHERE content_hits_tbl.content_id = " & content_id & "GROUP BY users.user_name, users.last_name, users.first_name")

        AnalyticsDataView.Table = AnalyticsData.Tables(0)

        auditContent.DataSource = AnalyticsDataView
        auditContent.DataBind()
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

        AnalyticsDataView.Sort = SortExpression
        GridView1.PageIndex = PageIndex

        GridView1.DataSource = AnalyticsDataView
        GridView1.DataBind()
    End Sub

    Protected Sub GridView2_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles auditContent.Sorting
        If (SortOrder = SortDirection.Descending) Then
            SortExpression = e.SortExpression & " DESC"
        Else
            SortExpression = e.SortExpression & " ASC"
        End If

        AnalyticsDataView.Sort = SortExpression
        auditContent.PageIndex = PageIndex

        auditContent.DataSource = AnalyticsDataView
        auditContent.DataBind()
    End Sub

    Protected Sub GridView2_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles auditContent.PageIndexChanging
        PageIndex = e.NewPageIndex

        AnalyticsDataView.Sort = SortExpression
        auditContent.PageIndex = PageIndex

        auditContent.DataSource = AnalyticsDataView
        auditContent.DataBind()
    End Sub

End Class
