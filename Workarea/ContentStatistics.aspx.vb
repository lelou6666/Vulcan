Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Common

Partial Class ContentStatistics
    Inherits System.Web.UI.Page

    Protected _ContentAPI As New Ektron.Cms.ContentAPI()
    Protected apiFlagging As New Community.FlaggingAPI()
    Protected start_date As String = ""
    Protected end_date As String = ""
    Protected start_date2 As String = ""
    Protected end_date2 As String = ""
    'Protected m_refContentApi As New ContentAPI
    Protected common As Ektron.Cms.CommonApi
    Protected contentid As Long
    Protected action As String
    Protected m_refStyle As New StyleHelper
    Protected _MessageHelper As Ektron.Cms.Common.EkMessageHelper
    Protected ContentLanguage As String
    Protected content_data As Ektron.Cms.ContentData
    Protected refUrl As String
    Protected ratingDataSource As New System.Data.DataView
    Protected results As Collection
    Protected aFlags As Ektron.Cms.ContentFlagData() = Array.CreateInstance(GetType(Ektron.Cms.ContentFlagData), 0)

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Me.MessageBoard.MaxResults = m_refContentApi.RequestInformationRef.PagingSize
        MessageBoard1.MaxResults = _ContentAPI.RequestInformationRef.PagingSize
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitPage()
        RegisterResources()
        checkAccess()

        Literal1.Text = BuildDateSelectors(True)
        Literal2.Text = BuildDateSelectors(False)

        BuildToolBar()
        getResultBtn.Text = _MessageHelper.GetMessage("lbl get reviews")
        Button2.Text = _MessageHelper.GetMessage("lbl purge reviews")
        getFlagBtn.Text = _MessageHelper.GetMessage("lbl get flags")
        cmdFlags.Text = _MessageHelper.GetMessage("lbl purge flags")
        dialogOkButtonText.Text = _MessageHelper.GetMessage("lbl ok")
        dialogCancelButtonText.Text = _MessageHelper.GetMessage("generic cancel")
        closeDialogLink.Text = "<span class=""ui-icon ui-icon-closethick"">" & _MessageHelper.GetMessage("close this dialog") & "</span>"
        confirmDialogHeader.Text = _MessageHelper.GetMessage("delete comment")
		confirmDeleteCommentMessage.Text = Ektron.Cms.API.JS.EscapeAndEncode(_MessageHelper.GetMessage("delete comment message"))
		jsAppPath.Text = Ektron.Cms.API.JS.Escape(_ContentAPI.AppPath)

        ' If the page is a redirect from addeditcontentreview.aspx, it is because we displayed
        ' the reviews and the user editted or deleted one of the reviews.
        ' Display them again so the user cn see their changes.
        If (Request.QueryString("showReviews") = "true") Then
            Try
                Dim clickSender As Object = DBNull.Value
                Dim clickEvent As System.EventArgs = New System.EventArgs()
                getResultBtn_Click(clickSender, clickEvent)
            Catch exception As Exception
                ' do nothing
            End Try
        End If
    End Sub
    Private Sub checkAccess()
        Utilities.ValidateUserLogin()
        If (_ContentAPI.RequestInformationRef.IsMembershipUser) Then
            Response.Redirect("reterror.aspx?info=" & _ContentAPI.EkMsgRef.GetMessage("msg login cms user"), False)
            Exit Sub
        End If
    End Sub

    Private Sub RegisterResources()
        'CSS
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)
        Ektron.Cms.API.Css.RegisterCss(Me, common.ApplicationPath & "wamenu/css/com.ektron.ui.menu.css", "EktronUIMenuCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronModalCss)

        'JS
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronPlatformInfoJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronDesignFormEntryJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)
        Ektron.Cms.API.JS.RegisterJS(Me, common.ApplicationPath & "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncts")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronStringJS)

    End Sub

    Private ReadOnly Property SourcePage() As String
        Get
            Return "content.aspx?action=View&folder_id=" & content_data.FolderId & "&id=" & content_data.Id & "&LangType=" & common.RequestInformationRef.ContentLanguage
        End Get
    End Property

    Protected Sub InitPage()
        SetUpRating()

        If (refUrl Is Nothing) Then
            refUrl = SourcePage
        End If

        If (Not Request.QueryString("redirect") Is Nothing) Then
            Response.Redirect(SourcePage())
        End If

        If (GridView1.Rows.Count > 0) Then
            Button1.Enabled = True
            Button1.Visible = True
        Else
            Button1.Enabled = False
            Button1.Visible = False
        End If

        _MessageHelper = common.EkMsgRef

        Dim today As DateTime = DateTime.Today

        Try
            ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
        Catch ex As Exception
            ContentLanguage = -1
        End Try

        If (Not IsPostBack) Then
            Try
                SetContentID()
                Dim res As Collection = _ContentAPI.GetContentRatingStatistics(contentid, 0, String.Empty)
                Dim total As Integer = Convert.ToInt32(res("total"))
                If (total = 0) Then
                    resultGraph.Text = "<b>" & _MessageHelper.GetMessage("content not rated") & "</b>"
                Else
                    Dim sum As Integer = Convert.ToInt32(res("sum"))
                    Dim r() As Integer = Array.CreateInstance(GetType(Integer), 11)
                    Dim i As Integer
                    For i = 0 To 10
                        r(i) = Convert.ToInt32(res("r" & i))
                    Next
                    totalResults.Text = "<b>" & _MessageHelper.GetMessage("rating label") & ":</b> " & IIf(total > 0, (Math.Round(sum / total) / 2), "0") & " out of 5 Stars - " & total & " " & _MessageHelper.GetMessage("total ratings level")
                    DrawGraph(r)
                End If
            Catch ex As Exception

            End Try

            Button2.Attributes.Add("onClick", "var arr=confirm('" & _MessageHelper.GetMessage("alert msg purge data") & "'); if( arr ) { return true; } else { return false; }")
            Me.cmdFlags.Attributes.Add("onClick", "var arr=confirm('" & _MessageHelper.GetMessage("alert msg purge data") & "'); if( arr ) { return true; } else { return false; }")
        End If
    End Sub

#Region "Content Flagging"

    Protected Sub getFlagBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles getFlagBtn.Click
        refUrl = Request.Url.OriginalString
        BuildToolBar()

        SetContentID()
        DefineFlagData()

        If (aFlags.Length = 0) Then
            no_results_lbl2.Text = _MessageHelper.GetMessage("lbl no flags") & "<br />"
        Else
            LoadContentFlags(aFlags)
            no_results_lbl2.Text = String.Empty
        End If

        Try
            Dim pageSize As Integer = Me._ContentAPI.RequestInformationRef.PagingSize
            dg_flag.PageSize = pageSize
        Catch ex As Exception
            dg_flag.PageSize = 50
        End Try

        SelectedTab.Text = "dvFlagging"
    End Sub

    Private Sub DefineFlagData()
        GridView1.Visible = False
        Button1.Visible = False
        dg_flag.Visible = True
        Dim iTotalCF As Integer = 0

        Dim pStartDate As DateTime
        Dim pEndDate As DateTime

        If (start_date2 = "") Then
            pStartDate = New DateTime(1753, 1, 1)
        Else
            pStartDate = DateTime.Parse(start_date2)
        End If

        If (end_date2 = "") Then
            pEndDate = DateTime.MaxValue
        Else
            pEndDate = DateTime.Parse(end_date2)
        End If

        aFlags = apiFlagging.GetAllFlagEntries(contentid, 0, pStartDate, pEndDate, iTotalCF)
    End Sub

    Private Sub LoadContentFlags(ByVal FlagList() As ContentFlagData)
        dg_flag.DataSource = Me.CreateContentFlagSource(FlagList)
        dg_flag.CellPadding = 3

        Dim colBound As New System.Web.UI.WebControls.BoundColumn

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "USERNAME"
        colBound.HeaderText = _MessageHelper.GetMessage("display name label")
        'colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.ItemStyle.Wrap = False
        dg_flag.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "DATETIME"
        colBound.HeaderText = _MessageHelper.GetMessage("generic date no colon")
        'colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.ItemStyle.Wrap = False
        dg_flag.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "FLAG"
        colBound.HeaderText = _MessageHelper.GetMessage("flag label")
        'colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.ItemStyle.Wrap = False
        dg_flag.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "COMMENTS"
        colBound.HeaderText = _MessageHelper.GetMessage("comment text")
        'colBound.HeaderStyle.CssClass = "title-header"
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.ItemStyle.Wrap = True
        dg_flag.Columns.Add(colBound)

        dg_flag.DataBind()
    End Sub

    Private Function CreateContentFlagSource(ByVal FlagList() As ContentFlagData) As ICollection
        Dim dt As New DataTable
        Dim dr As DataRow
        Dim bShowApprove As Boolean = False

        Dim dtS As EkDTSelector
        dtS = common.EkDTSelectorRef

        dt.Columns.Add(New DataColumn("USERNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("DATETIME", GetType(String)))
        dt.Columns.Add(New DataColumn("FLAG", GetType(String)))
        dt.Columns.Add(New DataColumn("COMMENTS", GetType(String)))

        For i As Integer = 0 To (FlagList.Length - 1)
            dr = dt.NewRow()
            If FlagList(i).FlaggedUser.Id = 0 Then
                dr(0) = "<font color=""gray"">" & Me._MessageHelper.GetMessage("lbl anon") & "</font>"
            Else
                dr(0) = FlagList(i).FlaggedUser.DisplayName
            End If
            dr(1) = GetFlagEditURL(FlagList(i).FlagID, FlagList(i).FlagDate)
            dr(2) = FlagList(i).FlagName
            dr(3) = FlagList(i).FlagComment
            dt.Rows.Add(dr)
        Next
        Dim dv As New DataView(dt)
        Return dv
    End Function

    Protected Function GetFlagEditURL(ByVal contentflagID As Long, ByVal flag_date As String) As String
        Return "<a href=""ContentFlagging/addeditcontentflag.aspx?action=view&id=" & contentflagID.ToString() & "&cid=" & contentid.ToString() & """>" & Convert.ToDateTime(flag_date).ToShortDateString & "</a>"
    End Function

    Protected Sub cmdFlags_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdFlags.Click
        refUrl = Request.Url.OriginalString
        BuildToolBar()
        Dim pStartDate As DateTime
        Dim pEndDate As DateTime

        If (start_date2 = "") Then
            pStartDate = New DateTime(1753, 1, 1)
        Else
            pStartDate = DateTime.Parse(start_date2)
        End If

        If (end_date2 = "") Then
            pEndDate = DateTime.MaxValue
        Else
            pEndDate = DateTime.Parse(end_date2)
        End If

        apiFlagging.PurgeFlagEntries(Me.contentid, pStartDate, pEndDate)

        SelectedTab.Text = "dvFlagging"
    End Sub

#End Region

    Private Function GetPurgeValidationMessage() As String
        Dim str As New StringBuilder
        str.Append("Are you sure you want to purge all content")
        If (Me.start_date = "[None]") Then
            str.Append("up until " & Me.end_date)
        Else
            str.Append("from " & Me.start_date & " to " & Me.end_date & "?")
        End If
        Return str.ToString()
    End Function

    Private Sub SetUpRating()
        SetContentID()
        common = New Ektron.Cms.CommonApi()
    End Sub

    Private Sub SetContentID()
        If (Request.QueryString("id") <> "") Then
            Try
                contentid = Convert.ToInt64(Request.QueryString("id"))
            Catch ex As Exception
                contentid = 0
            End Try
        Else
            contentid = 0
        End If
        Try
            content_data = _ContentAPI.GetContentById(contentid)
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & ex.Message.ToString, True)
        End Try
    End Sub

    Private Function BuildDateSelectors(ByVal IsDisplayRating As Boolean) As String
        Dim sbHtml As New StringBuilder
        Dim dateSchedule As New Ektron.Cms.EkDTSelector(common.RequestInformationRef)
        Dim StartElementID As String = "start_date"
        Dim EndElementID As String = "end_date"
        If (Not IsDisplayRating) Then
            StartElementID = "start_date2"
            EndElementID = "end_date2"
        End If
        Try
            If (Request.Form(StartElementID & "_iso") <> "") Then
                If (Not IsDisplayRating) Then
                    start_date2 = Request.Form(StartElementID & "_iso") & " " & Request.Form(StartElementID & "_hr") & ":" & Request.Form(StartElementID & "_mi")
                Else
                    start_date = Request.Form(StartElementID & "_iso") & " " & Request.Form(StartElementID & "_hr") & ":" & Request.Form(StartElementID & "_mi")
                End If
            End If
        Catch ex As Exception
            start_date = ""
        End Try


        Try
            If (Request.Form(EndElementID & "_iso") <> "") Then
                If (Not IsDisplayRating) Then
                    end_date2 = Request.Form(EndElementID & "_iso") & " " & Request.Form(EndElementID & "_hr") & ":" & Request.Form(EndElementID & "_mi")
                Else
                    end_date = Request.Form(EndElementID & "_iso") & " " & Request.Form(EndElementID & "_hr") & ":" & Request.Form(EndElementID & "_mi")
                End If
            End If
        Catch ex As Exception
            end_date = ""
        End Try

        sbHtml.Append("<table class=""ektronGrid"">")
        sbHtml.Append("<tr>")
        sbHtml.Append("<td class=""label"">")
        sbHtml.Append(_MessageHelper.GetMessage("generic start date label"))
        sbHtml.Append("</td>")
        sbHtml.Append("<td class=""value"">")
        dateSchedule.formName = "form1"
        dateSchedule.extendedMeta = True
        dateSchedule.formElement = StartElementID
        dateSchedule.spanId = StartElementID & "_span"
        If (start_date <> "") Then
            Try
                dateSchedule.targetDate = CDate(start_date)
            Catch ex As Exception
                start_date = ""
            End Try
        End If
        If (start_date2 <> "") Then
            Try
                dateSchedule.targetDate = CDate(start_date2)
            Catch ex As Exception
                start_date2 = ""
            End Try
        End If

        sbHtml.Append(dateSchedule.displayCultureDateTime(True))
        sbHtml.Append("</td>")
        sbHtml.Append("</tr>")
        sbHtml.Append("<tr>")
        sbHtml.Append("<td class=""label"">")
        sbHtml.Append(_MessageHelper.GetMessage("generic end date label"))
        sbHtml.Append("</td>")
        sbHtml.Append("<td class=""value"">")
        dateSchedule = New Ektron.Cms.EkDTSelector(common.RequestInformationRef)
        dateSchedule.formName = "form1"
        dateSchedule.extendedMeta = True
        dateSchedule.formElement = EndElementID
        dateSchedule.spanId = EndElementID & "_span"
        If (end_date <> "") Then
            Try
                dateSchedule.targetDate = CDate(end_date)
            Catch ex As Exception
                end_date = ""
            End Try
        End If
        If (end_date2 <> "") Then
            Try
                dateSchedule.targetDate = CDate(end_date2)
            Catch ex As Exception
                end_date2 = ""
            End Try

        End If

        sbHtml.Append(dateSchedule.displayCultureDateTime(True))
        sbHtml.Append("</td>")
        sbHtml.Append("</tr>")
        sbHtml.Append("</table>")
        Return sbHtml.ToString()

    End Function

    Private Sub DrawGraph(ByVal r() As Integer)
        resultGraph.Text = "<img src=""" & common.ApplicationPath & "ContentRatingGraph.aspx?stars=true&"
        Dim i As Integer
        For i = 0 To 10
            resultGraph.Text &= "r" & (i) & "=" & r(i) & "&"
        Next
        resultGraph.Text &= """ />"
    End Sub

    Protected Sub getResultBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles getResultBtn.Click
        DefineData()
        refUrl = Request.Url.OriginalString
        BuildToolBar()
        totalResults.Text = "" ' clear as we're getting some new results now...

        Dim index As Integer
        For index = 0 To GridView1.Columns.Count - 1
            GridView1.Columns(index).HeaderStyle.CssClass = "title-header"
        Next

        Try
            Dim pageSize As Integer = Me._ContentAPI.RequestInformationRef.PagingSize
            GridView1.PageSize = pageSize
        Catch ex As Exception
            GridView1.PageSize = 50
        End Try

        SetContentID()
        ' ratings
        Dim i As Integer
        Dim r() As Integer = Array.CreateInstance(GetType(Integer), 11)

        If (GridView1.Rows.Count = 0) Then
            no_results_lbl.Text = _MessageHelper.GetMessage("lbl no comments")
        Else
            no_results_lbl.Text = String.Empty
        End If

        If (GridView1.Rows.Count > 0) Then
            Button1.Enabled = True
            Button1.Visible = True
        Else
            Button1.Enabled = False
            Button1.Visible = False
        End If

        Dim rating As Integer
        DefineResultsData()
        Dim total As Integer = results.Count
        Dim sum As Integer = 0
        For i = 0 To 10
            r(i) = 0
        Next
        For i = 1 To results.Count
            Dim row As Collection = results(i)
            rating = Convert.ToInt32(row("user_rating"))
            r(rating) = r(rating) + 1
            sum = sum + rating
        Next

        If (total = 0) Then
            resultGraph.Text = "<b>" & _MessageHelper.GetMessage("no ratings timeline") & "</b>"
            totalResults.Text = ""
        Else
            totalResults.Text = "<b>" & _MessageHelper.GetMessage("rating label") & ":</b> " & IIf(total > 0, (Math.Round(sum / total) / 2), "0") & " out of 5 Stars - " & total & " " & _MessageHelper.GetMessage("total ratings level")
            DrawGraph(r)
        End If
    End Sub

    Public Sub GridView1_RowDataBound(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
        Dim sortBy As String = _MessageHelper.GetMessage("sort by x")
        Select Case e.Row.RowType
            Case DataControlRowType.Header
                ' cell 0 - command links
                ' cell 1 - username
                ' cell 2 - date
                ' cell 3 - individual rating
                ' cell 4 - comment
                ' cell 5 - comment status

                ' provide translated text for Header Components
                CType(e.Row.Cells(1).FindControl("userNameHeader"), LinkButton).Text = _MessageHelper.GetMessage("generic username")
                CType(e.Row.Cells(1).FindControl("userNameHeader"), LinkButton).ToolTip = String.Format(sortBy, _MessageHelper.GetMessage("generic username"))
                CType(e.Row.Cells(1).FindControl("dateHeader"), LinkButton).Text = _MessageHelper.GetMessage("lbl generic date")
                CType(e.Row.Cells(1).FindControl("dateHeader"), LinkButton).ToolTip = String.Format(sortBy, _MessageHelper.GetMessage("lbl generic date"))

            Case DataControlRowType.DataRow
                Dim contentRating As Object = CType(e.Row.DataItem, Object)
                Dim contentRatingUser As String = EkFunctions.ReadDbString(contentRating(0))
                Dim contentRatingId As String = contentRating(1).ToString()
                Dim contentRatingDate As String = Convert.ToDateTime(contentRating(2)).ToShortDateString()
                Dim contentRatingComment As String = contentRating(5)
                Dim redirectUrl As String = Request.Url.ToString()
                If (Not redirectUrl.Contains("showReviews")) Then
                    redirectUrl = redirectUrl & "&showReviews=true"
                End If
                ' provide translated text for individual items
                CType(e.Row.Cells(0).FindControl("editCommentLink"), LinkButton).Text = _MessageHelper.GetMessage("lbl edit comment")
                CType(e.Row.Cells(0).FindControl("editCommentLink"), LinkButton).ToolTip = _MessageHelper.GetMessage("lbl edit comment")
				CType(e.Row.Cells(0).FindControl("editCommentLink"), LinkButton).OnClientClick = "window.location = '" & Ektron.Cms.API.JS.Escape(_ContentAPI.AppPath & "addeditcontentreview.aspx?action=edit&id=" & contentRatingId & "&cid=" & contentid & "&page=workarea&redirectUrl=" & Server.UrlEncode(redirectUrl)) & "'; return false;"
                CType(e.Row.Cells(0).FindControl("deleteCommentLink"), LinkButton).Text = _MessageHelper.GetMessage("delete comment")
                CType(e.Row.Cells(0).FindControl("deleteCommentLink"), LinkButton).ToolTip = _MessageHelper.GetMessage("delete comment")

                CType(e.Row.Cells(0).FindControl("deleteCommentLink"), LinkButton).OnClientClick = "deleteContentRatingPrompt({ratingId: " & contentRatingId & ", contentId: " & contentid & ", user: '" & contentRatingUser & "', date: '" & contentRatingDate & "', comment: '" & Ektron.Cms.API.JS.EscapeAndEncode(UrlDecode(contentRatingComment)) & "'}); return false;"
        End Select
    End Sub

    Private Sub DefineData()
        Dim pStartDate As DateTime
        Dim pEndDate As DateTime

        If (start_date = "") Then
            pStartDate = New DateTime(1753, 1, 1)
            start_date2 = ""
        Else
            pStartDate = DateTime.Parse(start_date)
            start_date2 = ""
        End If

        If (end_date = "") Then
            pEndDate = DateTime.MaxValue
            end_date2 = ""
        Else
            pEndDate = DateTime.Parse(end_date)
            end_date2 = ""
        End If
        ratingDataSource.Table = _ContentAPI.GetContentRatingResults(contentid, pStartDate, pEndDate).Tables(0)
        GridView1.DataSource = ratingDataSource

        GridView1.DataBind()
    End Sub

    Private Sub DefineResultsData()
        GridView1.Visible = True
        Button1.Visible = True
        dg_flag.Visible = False

        Dim pStartDate As DateTime
        Dim pEndDate As DateTime

        If (start_date = "") Then
            pStartDate = New DateTime(1753, 1, 1)
            start_date2 = ""
        Else
            pStartDate = DateTime.Parse(start_date)
            end_date2 = ""
        End If

        If (end_date = "") Then
            pEndDate = DateTime.MaxValue
            end_date2 = ""
        Else
            pEndDate = DateTime.Parse(end_date)
            end_date2 = ""
        End If

        results = _ContentAPI.GetContentRatingStatistics(contentid, pStartDate, pEndDate)
    End Sub

    Private Sub BuildToolBar()
        Dim result As System.Text.StringBuilder
        result = New System.Text.StringBuilder
        Dim AppImgPath As String = _ContentAPI.AppImgPath
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(_MessageHelper.GetMessage("content report for") & " """ & content_data.Title & """")
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(_ContentAPI.AppPath & "images/UI/Icons/back.png", refUrl, _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))

        result.Append("<td>" & m_refStyle.GetHelpButton("ContentStatistics") & "</td>")

        result.Append("<td>")
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString

        StyleSheetJS.Text = (New StyleHelper).GetClientScript
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        'disable sorting and paging so it won't have the javascript link once it is in the spreadsheet.
        GridView1.AllowSorting = False
        GridView1.AllowPaging = False
        'after changing the properties of the GridView, it needs to be re-DataBind to take effect.
        GridView1.DataBind()
        Response.Clear()
        Response.AddHeader("content-disposition", "attachment;filename=" & content_data.Title & "_Ratings.xls")
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = "application/vnd.xls"
        Dim stringWrite As New System.IO.StringWriter()
        Dim htmlWrite As New HtmlTextWriter(stringWrite)
        DefineData()
        GridView1.DataSource = ratingDataSource
        GridView1.DataBind()
        GridView1.RenderControl(htmlWrite)
        Response.Write(stringWrite.ToString())
        Response.End()

        GridView1.AllowSorting = True
        GridView1.AllowPaging = True
    End Sub

    Public Overrides Sub VerifyRenderingInServerForm(ByVal control As System.Web.UI.Control)
        'MyBase.VerifyRenderingInServerForm(control)
        ' this is done to allow export to excel
    End Sub

    Protected Sub Button2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button2.Click
        refUrl = SourcePage
        BuildToolBar()
        Dim pStartDate As DateTime
        Dim pEndDate As DateTime

        If (start_date = "") Then
            pStartDate = New DateTime(1753, 1, 1)
        Else
            pStartDate = DateTime.Parse(start_date)
        End If

        If (end_date = "") Then
            pEndDate = DateTime.MaxValue
        Else
            pEndDate = DateTime.Parse(end_date)
        End If

        _ContentAPI.PurgeContentRatings(Me.contentid, pStartDate, pEndDate)
        GridView1.Visible = False
        Button1.Visible = False
        dg_flag.Visible = False
        resultGraph.Text = "<b>" & _MessageHelper.GetMessage("content not rated") & "</b>" 'resultGraph.Text = String.Empty
        totalResults.Text = String.Empty
        no_results_lbl.Text = _MessageHelper.GetMessage("lbl no comments")
    End Sub

    Protected Sub GridView1_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridView1.Sorting
        DefineData()
        If (SortOrder = SortDirection.Descending) Then
            ratingDataSource.Sort = e.SortExpression & " DESC"
        Else
            ratingDataSource.Sort = e.SortExpression & " ASC"
        End If
        GridView1.DataSource = ratingDataSource
        GridView1.DataBind()
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        DefineData()
        GridView1.PageIndex = e.NewPageIndex
        GridView1.DataBind()
    End Sub

    Protected ReadOnly Property SortOrder() As System.Web.UI.WebControls.SortDirection
        Get
            Dim order As System.Web.UI.WebControls.SortDirection
            Try
                order = CType(ViewState("ek_SortOrder"), System.Web.UI.WebControls.SortDirection)
            Catch ex As Exception
                order = WebControls.SortDirection.Ascending
                ViewState.Add("ek_SortOrder", order)
            End Try
            If (order = WebControls.SortDirection.Ascending) Then
                order = WebControls.SortDirection.Descending

            Else
                order = WebControls.SortDirection.Ascending
            End If
            ViewState("ek_SortOrder") = order
            Return order
        End Get
    End Property

    Protected Function DisplayRatingStatus(ByVal Status As Integer) As String
        If Status = 0 Then
            Return "<font color=""orange"">Pending</font>"
        ElseIf Status = 1 Then
            Return "<font color=""green"">Approved</font>"
        ElseIf Status = 2 Then
            Return "<font color=""red""><strong>Rejected</strong></font>"
        Else
            Return "<font color=""yellow"">Pending</font>"
        End If
    End Function
    Protected Function GetEditURL(ByVal contentratingID As Long, ByVal rating_date As String) As String
        Return "<a href=""addeditcontentreview.aspx?action=view&id=" & contentratingID.ToString() & "&cid=" & contentid.ToString() & """>" & Convert.ToDateTime(rating_date).ToShortDateString & "</a>"
    End Function
    Protected Function DisplayComments(ByVal Comments As String) As String
        Return Server.HtmlDecode(Comments)
    End Function
    Protected Function GetUserName(ByVal username As Object) As String
        If IsDBNull(username) OrElse Trim(username) = "" Then
            Return "<span color=""gray"">" & _MessageHelper.GetMessage("lbl anon") & "</span>"
        Else
            Return username
        End If
    End Function
    Public Function GenerateStars(ByVal irating As Integer) As String
        Dim sbRating As New StringBuilder()
		Dim strHttpType As String = String.Empty
        strHttpType = System.Web.HttpContext.Current.Request.Url.Scheme & "://"
        For i As Integer = 1 To 10
            sbRating.Append("<img border=""0"" src=""" & strHttpType & _ContentAPI.RequestInformationRef.HostUrl & _ContentAPI.AppPath & "images/ui/icons/")
			If i Mod 2 > 0 Then
                If irating < i Then
                    sbRating.Append("starEmptyLeft.png")
                Else
                    sbRating.Append("starLeft.png")
                End If
            Else
                If irating < i Then
                    sbRating.Append("starEmptyRight.png")
                Else
                    sbRating.Append("starRight.png")
                End If
            End If
            sbRating.Append(""" />")
        Next
        Return sbRating.ToString()
    End Function
End Class
