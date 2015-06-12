Imports System.Web.UI.HtmlControls
Imports System.Collections.Specialized
Imports System.Reflection
Imports System.Configuration
Imports Ektron.Cms

Partial Class ContentAnalytics
    Inherits System.Web.UI.Page

    Protected cAPI As New Ektron.Cms.ContentAPI()
    Protected start_date As String = ""
    Protected end_date As String = ""
    Protected common As Ektron.Cms.CommonApi
    Protected contentid As Long
    Protected action As String
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected ContentLanguage As String
    Protected content_data As Ektron.Cms.ContentData

    Protected Const ControlPath As String = "controls/analytics/"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load        
        common = New Ektron.Cms.CommonApi()
        If common.RequestInformationRef.IsMembershipUser Or common.RequestInformationRef.UserId = 0 Then            
            Response.Redirect("blank.htm", False)
        Else
            InitPage()
            BuildDateSelectors()
            BuildToolBar()
            LoadSelectedControl()
            RegisterResources()
        End If

    End Sub

    Protected Sub InitPage()
        m_refMsg = common.EkMsgRef
        BuildNav()
        DefineView()

        ' Set days to language
        ctlDay.Text = "[" & common.EkMsgRef.GetMessage("day") & "]"
        ctlWeek.Text = "[" & common.EkMsgRef.GetMessage("week") & "]"
        ctlMonth.Text = "[" & common.EkMsgRef.GetMessage("month") & "]"
        ctlYear.Text = "[" & common.EkMsgRef.GetMessage("year") & "]"
        linkToday.Text = "[" & common.EkMsgRef.GetMessage("today") & "]"

        Button1.Text = common.EkMsgRef.GetMessage("run custom range")
    End Sub

    Protected Sub BuildNav()
        If (Request.QueryString("id") Is Nothing) Then
            Me.navBar.Visible = True
            Dim ViewType As String = Request.QueryString("type")

            If (ViewType Is Nothing) Then
                ViewType = "global"
            Else
                ViewType = ViewType.ToLower()
            End If

            Dim t_global As String = "tab_disabled"
            Dim t_activity As String = "tab_disabled"
            Dim t_content As String = "tab_disabled"
            Dim t_page As String = "tab_disabled"
            Dim t_referring As String = "tab_disabled"

            Select Case (ViewType)
                Case "global"
                    If (Request.QueryString("report") = "2") Then
                        t_activity = "tab_actived"
                    Else
                        t_global = "tab_actived"
                    End If
                Case "content"
                    t_content = "tab_actived"
                Case "page"
                    t_page = "tab_actived"
                Case "referring"
                    t_referring = "tab_actived"
            End Select

            'TODO: Ross - These need to be converted to jQuery tabs
            navBar.Text &= "<table width=""100%"">"
            navBar.Text &= "<tr>"
            navBar.Text &= "<td class=""" & t_global & """ width=""1%"" nowrap><a style=""text-decoration:none;"" href=ContentAnalytics.aspx?type=global&report=1><b>&nbsp;" & common.EkMsgRef.GetMessage("site stats") & "&nbsp;</b></a></td>"
            navBar.Text &= "<td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>"
            navBar.Text &= "<td class=""" & t_activity & """ width=""1%"" nowrap><a style=""text-decoration:none;"" href=ContentAnalytics.aspx?type=global&report=2><b>&nbsp;" & common.EkMsgRef.GetMessage("site activity") & "&nbsp;</b></a></td>"
            navBar.Text &= "<td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>"
            navBar.Text &= "<td class=""" & t_content & """ width=""1%"" nowrap><a style=""text-decoration:none;"" href=ContentAnalytics.aspx?type=content><b>&nbsp;" & common.EkMsgRef.GetMessage("top content") & "&nbsp;</b></a></td>"
            navBar.Text &= "<td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>"
            navBar.Text &= "<td class=""" & t_page & """ width=""1%"" nowrap><a style=""text-decoration:none;"" href=ContentAnalytics.aspx?type=page><b>&nbsp;" & common.EkMsgRef.GetMessage("lbl top templates") & "&nbsp;</b></a></td>"
            navBar.Text &= "<td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>"
            navBar.Text &= "<td class=""" & t_referring & """ width=""1%"" nowrap><a style=""text-decoration:none;"" href=ContentAnalytics.aspx?type=referring><b>&nbsp;" & common.EkMsgRef.GetMessage("top referrers") & "&nbsp;</b></a></td>"
            navBar.Text &= "<td class=""tab_last"" width=""91%"" nowrap>&nbsp;</td>"
            navBar.Text &= "</tr>"
            navBar.Text &= "</table>"
        Else
            navBar.Visible = False
        End If
    End Sub

    Private Sub SetToTodayAndDay()
        If (Session("CurrentView") Is Nothing) Then
            Session.Add("CurrentView", "day")
            ctlDay.Font.Bold = True
        Else
            Session("CurrentView") = "day"
            ctlDay.Font.Bold = True
        End If

        If (Session("EndDate") Is Nothing) Then
            Session.Add("EndDate", DateTime.Today)
        Else
            Session("EndDate") = DateTime.Today
        End If

        If (Session("StartDate") Is Nothing) Then
            Session.Add("StartDate", DateTime.Today)
        Else
            Session("StartDate") = DateTime.Today
        End If
    End Sub

    Protected Sub DefineView()
        If (Not Request.QueryString("landing") Is Nothing) Then
            Dim landingType As Integer = 0
            Try
                landingType = Convert.ToInt32(Request.QueryString("landing"))
            Catch ex As Exception
                landingType = 0
            End Try

            If (landingType = 1) Then
                SetToTodayAndDay()
                Response.Redirect(Request.Url.ToString().Replace("&landing=1", ""))
            ElseIf (landingType = 2) Then
                SetToTodayAndDay()
                Response.Redirect(Request.Url.ToString().Replace("&landing=2", ""))
            End If
        End If

        If (Session("CurrentView") Is Nothing) Then
            Session.Add("CurrentView", "day")
            ctlDay.Font.Bold = True
        End If

        If (Session("EndDate") Is Nothing) Then
            Session.Add("EndDate", DateTime.Today)
        End If

        If (Session("StartDate") Is Nothing) Then
            Session.Add("StartDate", DateTime.Today)
        End If

        start_date = Session("StartDate")
        end_date = Session("EndDate")

        Dim sCurrView As String = (Session("CurrentView"))
        Select Case sCurrView
            Case "day"
                ctlDay.Font.Bold = True
            Case "week"
                ctlWeek.Font.Bold = True
            Case "month"
                ctlMonth.Font.Bold = True
            Case "year"
                ctlYear.Font.Bold = True
            Case Else
                ctlDay.Font.Bold = True
        End Select

        linkNext.Text = "[" & m_refMsg.GetMessage("next") & " " & common.EkMsgRef.GetMessage(CurrentView) & "]"
        linkPrevious.Text = "[" & m_refMsg.GetMessage("previous") & " " & common.EkMsgRef.GetMessage(CurrentView) & "]"
    End Sub

    Public Sub DeactivateAll()
        Me.Global1.Visible = False
        Me.ContentReports1.Visible = False
        Me.Page1.Visible = False
        Me.Referring_url1.Visible = False
    End Sub

    Public Sub ActivateControl(ByVal cont As AnalyticsBase)
        cont.Visible = True
        cont.StartDateTime = StartDate
        cont.EndDateTime = EndDate
        Me.Description.Text = cont.Description
        cont.CurrentView = CurrentView
        cont.Initialize()
    End Sub

    Private Sub LoadSelectedControl()
        Dim ControlType As String = "global"
        Try
            ControlType = Request.QueryString("type").ToLower()
            Select Case (ControlType)
                Case "global"
                    DeactivateAll()
                    ActivateControl(Me.Global1)
                    Description.Text = Global1.Description
                Case "content"
                    DeactivateAll()
                    ActivateControl(Me.ContentReports1)
                    Description.Text = ContentReports1.Description
                Case "page"
                    DeactivateAll()
                    ActivateControl(Me.Page1)
                    Description.Text = Page1.Description
                Case "referring"
                    DeactivateAll()
                    ActivateControl(Me.Referring_url1)
                    Description.Text = Referring_url1.Description
                Case Else
                    DeactivateAll()
                    ActivateControl(Me.Global1)
                    Description.Text = Global1.Description
            End Select
        Catch ex As Exception
            DeactivateAll()
            ActivateControl(Me.Global1)
        End Try
    End Sub

    Private Sub BuildDateSelectors()        
        Dim dateSchedule As New Ektron.Cms.EkDTSelector(common.RequestInformationRef)

        Me.lblQuickView.Text = common.EkMsgRef.GetMessage("quick view lbl") & ":"
        Me.lblJumpTo.Text = common.EkMsgRef.GetMessage("jump to lbl") & ":"

        Dim sbHtml As New StringBuilder
        sbHtml.Append("<tr>")
        sbHtml.Append("<td class=""label"">")
        sbHtml.Append(common.EkMsgRef.GetMessage("generic start date label"))
        sbHtml.Append("</td>")
        sbHtml.Append("<td>")
        dateSchedule.formName = "form1"
        dateSchedule.extendedMeta = True
        dateSchedule.formElement = "start_date"
        dateSchedule.spanId = "start_date_span"
        If (start_date <> "") Then
            Try
                dateSchedule.targetDate = StartDate
            Catch ex As Exception
                start_date = ""
            End Try
        End If
        sbHtml.Append(dateSchedule.displayCultureDate(True))
        sbHtml.Append("</td>")
        sbHtml.Append("</tr>")
        sbHtml.Append("<tr>")
        sbHtml.Append("<td class=""label"">")
        sbHtml.Append(common.EkMsgRef.GetMessage("generic end date label"))
        sbHtml.Append("</td>")
        sbHtml.Append("<td>")
        dateSchedule = New Ektron.Cms.EkDTSelector(common.RequestInformationRef)
        dateSchedule.formName = "form1"
        dateSchedule.extendedMeta = True
        dateSchedule.formElement = "end_date"
        dateSchedule.spanId = "end_date_span"
        If (end_date <> "") Then
            Try
                dateSchedule.targetDate = EndDate
            Catch ex As Exception
                end_date = ""
            End Try
        End If
        sbHtml.Append(dateSchedule.displayCultureDate(True))
        sbHtml.Append("</td>")
        sbHtml.Append("</tr>")
        lblViewing.Text = sbHtml.ToString()
    End Sub

    Private Sub BuildToolBar()
		Dim refUrl As String
		Dim type As String = ""
		Dim helpScreenAlias As String = "contentanalytics"

        refUrl = "ContentAnalytics.aspx?type=" & Request.QueryString("type")

        Dim result As System.Text.StringBuilder
        result = New System.Text.StringBuilder
        Dim AppImgPath As String = cAPI.AppImgPath
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl Content Analytics"))
        result.Append("<table><tr>")

        If (Request.QueryString("id") <> "") Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", refUrl, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If


        If (Not IsNothing(Request.QueryString("type"))) Then
            type = Request.QueryString("type")
            Select Case type
                Case "global"
                    helpScreenAlias = "contentanalytics_siteactivity"
                Case "content"
                    helpScreenAlias = "contentanalytics_topcontent"
                Case "page"
                    helpScreenAlias = "contentanalytics_toppages"
                Case "referring"
                    helpScreenAlias = "contentanalytics_topreferrers"
                Case Else
                    helpScreenAlias = "contentanalytics"
            End Select
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(helpScreenAlias))
        result.Append("</td>")

        result.Append("<td>")
        result.Append("</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString

        StyleSheetJS.Text = (New StyleHelper).GetClientScript
    End Sub

    Protected Sub SetControlDates(ByVal current As Ektron.Cms.AnalyticsBase)
        Try
            current.StartDateTime = DateTime.Parse(start_date)
        Catch ex As Exception
            current.StartDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(1))
        End Try

        Try
            current.EndDateTime = DateTime.Parse(end_date)
        Catch ex As Exception
            current.EndDateTime = DateTime.Now
        End Try
    End Sub

    Protected Sub linkPrevious_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles linkPrevious.Click
        Select Case (CurrentView)
            Case "day"
                EndDate = EndDate.Date.AddDays(-1)
                StartDate = EndDate.Date
            Case "week"
                EndDate = EndDate.Date.AddDays(-7)
                StartDate = EndDate.Date.AddDays(-7).AddDays(1)
            Case "month"
                EndDate = EndDate.Date.AddMonths(-1)
                StartDate = EndDate.Date.AddMonths(-1).AddDays(1)
            Case "year"
                EndDate = EndDate.Date.AddYears(-1)
                StartDate = EndDate.Date.AddYears(-1).AddDays(1)
        End Select
        Session("EndDate") = EndDate
        Session("StartDate") = StartDate
        SelectView()
        LoadSelectedControl()
    End Sub

    Protected Sub linkNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles linkNext.Click
        Select Case (CurrentView)
            Case "day"
                StartDate = EndDate.Date.AddDays(1)
                EndDate = StartDate.Date
            Case "week"
                StartDate = EndDate.Date.AddDays(1)
                EndDate = EndDate.Date.AddDays(7)
            Case "month"
                StartDate = EndDate.Date.AddDays(1)
                EndDate = EndDate.Date.AddMonths(1)
            Case "year"
                StartDate = EndDate.Date.AddDays(1)
                EndDate = EndDate.Date.AddYears(1)
        End Select
        Session("EndDate") = EndDate
        Session("StartDate") = StartDate
        SelectView()
        LoadSelectedControl()
    End Sub

    Protected Sub linkToday_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles linkToday.Click
        Select Case (CurrentView)
            Case "day"
                EndDate = DateTime.Today
                StartDate = DateTime.Today
            Case "week"
                EndDate = DateTime.Today
                StartDate = EndDate.AddDays(-7)
            Case "month"
                EndDate = DateTime.Today
                StartDate = EndDate.AddMonths(-1)
            Case "year"
                EndDate = DateTime.Today
                StartDate = EndDate.AddYears(-1)
        End Select
        Session("EndDate") = EndDate
        Session("StartDate") = StartDate
        SelectView()
        LoadSelectedControl()
    End Sub

    Private Sub SelectView()
        UnselectView()
        Dim sCurrView As String = (Session("CurrentView"))
        Select Case sCurrView
            Case "day"
                ctlDay.Font.Bold = True
            Case "week"
                ctlWeek.Font.Bold = True
            Case "month"
                ctlMonth.Font.Bold = True
            Case "year"
                ctlYear.Font.Bold = True
        End Select
        BuildDateSelectors()
    End Sub

    Private Sub UnselectView()
        ctlDay.Font.Bold = False
        ctlWeek.Font.Bold = False
        ctlMonth.Font.Bold = False
        ctlYear.Font.Bold = False
    End Sub

    Protected Sub ctlDay_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlDay.Click
        CurrentView = "day"
        StartDate = EndDate
        linkNext.Text = "[" & m_refMsg.GetMessage("next") & " " & common.EkMsgRef.GetMessage(CurrentView) & "]"
        linkPrevious.Text = "[" & m_refMsg.GetMessage("previous") & " " & common.EkMsgRef.GetMessage(CurrentView) & "]"
        SelectView()
        LoadSelectedControl()
    End Sub

    Protected Sub ctlWeek_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlWeek.Click
        CurrentView = "week"
        StartDate = EndDate.AddDays(-7)
        linkNext.Text = "[" & m_refMsg.GetMessage("next") & " " & common.EkMsgRef.GetMessage(CurrentView) & "]"
        linkPrevious.Text = "[" & m_refMsg.GetMessage("previous") & " " & common.EkMsgRef.GetMessage(CurrentView) & "]"
        SelectView()
        LoadSelectedControl()
    End Sub

    Protected Sub ctlMonth_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlMonth.Click
        CurrentView = "month"
        StartDate = EndDate.AddMonths(-1)
        linkNext.Text = "[" & m_refMsg.GetMessage("next") & " " & common.EkMsgRef.GetMessage(CurrentView) & "]"
        linkPrevious.Text = "[" & m_refMsg.GetMessage("previous") & " " & common.EkMsgRef.GetMessage(CurrentView) & "]"
        SelectView()
        LoadSelectedControl()
    End Sub

    Protected Sub ctlYear_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlYear.Click
        CurrentView = "year"
        StartDate = EndDate.AddYears(-1)
        linkNext.Text = "[" & m_refMsg.GetMessage("next") & " " & common.EkMsgRef.GetMessage(CurrentView) & "]"
        linkPrevious.Text = "[" & m_refMsg.GetMessage("previous") & " " & common.EkMsgRef.GetMessage(CurrentView) & "]"
        SelectView()
        LoadSelectedControl()
    End Sub

    Protected Property CurrentView() As String
        Get
            Try
                Return Session("CurrentView").ToString()
            Catch ex As Exception
                Return "day"
            End Try
        End Get
        Set(ByVal value As String)
            Try
                Session("CurrentView") = value
            Catch ex As Exception
                ' do nothing
            End Try
        End Set
    End Property

    Protected Property EndDate() As DateTime
        Get
            Try
                Return Session("EndDate")
            Catch ex As Exception
                Return DateTime.Today
            End Try
        End Get
        Set(ByVal value As DateTime)
            Try
                end_date = value.ToString()
                Session("EndDate") = value
            Catch ex As Exception
                ' do nothing
            End Try
        End Set
    End Property

    Protected Property StartDate() As DateTime
        Get
            Try
                Return Session("StartDate")
            Catch ex As Exception
                Return DateTime.Today
            End Try
        End Get
        Set(ByVal value As DateTime)
            Try
                start_date = value.ToString()
                Session("StartDate") = value
            Catch ex As Exception
                ' do nothing
            End Try
        End Set
    End Property

    Protected ReadOnly Property StateString() As String
        Get
            Return "view=" & Me.CurrentView & "&end=" & Page.Server.UrlEncode(EndDate) & "&start=" & Page.Server.UrlEncode(StartDate)
        End Get
    End Property

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim str_start_date As String = Request.Form("start_date_iso")
        Dim str_end_date As String = Request.Form("end_date_iso")
        StartDate = DateTime.Parse(str_start_date)
        EndDate = DateTime.Parse(str_end_date)
        SelectView()
        LoadSelectedControl()
    End Sub
    Private Sub RegisterResources()
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronStyleHelperJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronPlatformInfoJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronDesignFormEntryJS)
        API.JS.RegisterJS(Me, common.ApplicationPath & "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncsJS")
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)

        API.Css.RegisterCss(Me, common.ApplicationPath & "explorer/css/com.ektron.ui.menu.css", "EktronUIMenuCSS")
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub
End Class
