
Partial Class controls_analytics_Page
    Inherits Ektron.Cms.AnalyticsBase

    Protected common As New Ektron.Cms.CommonApi()

    Public Overrides Sub Initialize()

        Me.GridView1.PageSize = PageSize
        Me.GridView2.PageSize = PageSize

        lbl_total_hits.Text = common.EkMsgRef.GetMessage("total views")
        lbl_total_visitors.Text = common.EkMsgRef.GetMessage("total visitors")
        lbl_hits_per_visitor.Text = common.EkMsgRef.GetMessage("views to visitors")
        lbl_new_visitors.Text = common.EkMsgRef.GetMessage("new visitors")
        lbl_returning_visitors.Text = common.EkMsgRef.GetMessage("returning visitors")
        lbl_hits_vs_visitors.Text = common.EkMsgRef.GetMessage("views to visitors")
        lbl_new_vs_returning_visitors.Text = common.EkMsgRef.GetMessage("new to returning visitors")

        Me.Image1.Visible = False
        Me.stats_aggr.Visible = False
        Description = common.EkMsgRef.GetMessage("lbl template stats")
        Me.stats_aggr.Visible = False
        If (Request.QueryString("id") Is Nothing) Then
            Me.Image1.Visible = False
            GridView1.Enabled = True
            GridView2.Enabled = False
            AnalyticsData.Clear()
            Fill("SELECT url, COUNT(DISTINCT visitor_id) AS Visits, COUNT(visitor_id) AS Views FROM content_hits_tbl " & _
            "WHERE " & DateClause & _
            "GROUP BY url ORDER BY Visits DESC")

            If (AnalyticsData.Tables(0).Rows.Count = 0) Then
                ErrMsg.Visible = True
                ErrMsg.Text = common.EkMsgRef.GetMessage("alt No Records for this range")
            End If

            Me.AnalyticsDataView.Table = AnalyticsData.Tables(0)
            GridView1.DataSource = AnalyticsData
            GridView1.Columns(0).HeaderText = "Template"
            GridView1.DataBind()

            CType(GridView1.Columns(0), HyperLinkField).DataNavigateUrlFormatString = common.ApplicationPath & "ContentAnalytics.aspx?type=page&id={0}"

            GridView1.DataSource = AnalyticsDataView
            GridView1.DataBind()
        Else

            GridView1.Enabled = False
            GridView2.Enabled = True
            Dim page_id As String = Request.QueryString("id")
            Description = "Statistics on template '" & page_id & "'"

            Dim target As String = common.ApplicationPath & "ContentAnalytics.aspx?type=page&id=" & page_id
            'Me.navBar.Text = "<ul id=""tabnav"">"
            'Me.navBar.Text &= "<li><a href=" & target & "&report=1>" & common.EkMsgRef.GetMessage("page stats") & "</a></li>"
            'Me.navBar.Text &= "<li><a href=" & target & "&report=2>" & common.EkMsgRef.GetMessage("page activity") & "</a></li>"
            'Me.navBar.Text &= "<li><a href=" & target & "&report=3>" & common.EkMsgRef.GetMessage("content in page") & "</a></li></ul>"
            Dim t_stats As String = "tab_disabled"
            Dim t_activity As String = "tab_disabled"
            Dim t_content As String = "tab_disabled"
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
                    t_content = "tab_actived"
            End Select
            navBar.Text = "<p><strong>" & page_id & "</strong></p>"
            Me.navBar.Text &= "<table height=""20"" style=""BACKGROUND-COLOR:white"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%""><tr>"
            Me.navBar.Text &= "<td class=""" & t_stats & """ width=""1%"" nowrap><b><a border=""0"" style=""text-decoration:none;"" href=" & target & "&report=1>&nbsp;" & common.EkMsgRef.GetMessage("lbl template stats") & "&nbsp;</a></b></td>"
            Me.navBar.Text &= "<td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>"
            Me.navBar.Text &= "<td class=""" & t_activity & """ width=""1%"" nowrap><b><a border=""0"" style=""text-decoration:none;"" href=" & target & "&report=2>&nbsp;" & common.EkMsgRef.GetMessage("lbl template activity") & "&nbsp;</a></b></td>"
            Me.navBar.Text &= "<td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>"
            Me.navBar.Text &= "<td class=""" & t_content & """ width=""1%"" nowrap><b><a border=""0"" style=""text-decoration:none;"" href=" & target & "&report=3>&nbsp;" & common.EkMsgRef.GetMessage("lbl content in template") & "&nbsp;</a></b></td>"
            Me.navBar.Text &= "<td class=""tab_last"" width=""91%"" nowrap>&nbsp;</td>"
            Me.navBar.Text &= "</tr></table>"

            Dim report As Integer = 1

            If (Not Request.QueryString("report") Is Nothing) Then
                report = Convert.ToInt32(Request.QueryString("report"))
            End If

            Select Case (report)
                Case 1 ' Quick Statistics
                    Description = common.EkMsgRef.GetMessage("stats on") & " '" & page_id & "'"
                    Me.stats_aggr.Visible = True
                    InitStatsAggr(page_id)
                Case 2 ' By Time
                    Description = common.EkMsgRef.GetMessage("activity on") & " '" & page_id & "'"
                    Me.Image1.Visible = True
                    graph_key.Text = "<table border=""0""><tr><td width=""20px"" height=""10px"" bgcolor=""red"">&nbsp;</td><td>" & _
                    "Views" & "</td></tr><tr><td width=""20px"" height=""10px"" bgcolor=""blue"">&nbsp;</td><td>" & _
                    "Visitors" & "</td></tr></table>"
                    Me.Image1.ImageUrl = common.ApplicationPath & "ContentRatingGraph.aspx?type=time&view=" & CurrentView & "&res_type=page&res=" & page_id & "&EndDate=" & Page.Server.UrlEncode(EndDateTime.ToString())
                Case 3 ' Content in Page
                    Description = common.EkMsgRef.GetMessage("content viewed on") & " '" & page_id & "'"
                    MostPopularContent(page_id)
            End Select



        End If

    End Sub


    Private Sub InitStatsAggr(ByVal page_id As String)
        QueryGetAnalyticsInfo(DateClause, page_id)
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

        graph_hits_per_visitor.ImageUrl = common.ApplicationPath & "ContentRatingGraph.aspx?type=pie&r1=" & Me.num_total_hits.Text & "&r2=" & Me.num_total_visitors.Text & "&size=100"
        graph_new_vs_returning_visitors.ImageUrl = common.ApplicationPath & "ContentRatingGraph.aspx?type=pie&r1=" & Me.num_new_visitors.Text & "&r2=" & Me.num_returning_visitors.Text & "&size=100"

    End Sub

    Private Sub MostPopularContent(ByVal page_id As String)
        AnalyticsData.Clear()
        QueryGetAnalyticsInfoForLanguage(DateClause, page_id, common.DefaultContentLanguage)
        CType(GridView2.Columns(0), HyperLinkField).DataNavigateUrlFormatString = common.ApplicationPath & "ContentAnalytics.aspx?type=content&id={0}&url=" & Request.QueryString("id")

        AnalyticsDataView.Table = AnalyticsData.Tables(0)
        GridView2.DataSource = AnalyticsDataView
        GridView2.DataBind()
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
        GridView1.PageIndex = e.NewPageIndex

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
        GridView1.PageIndex = PageIndex

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
End Class
