Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea

Partial Class addeditcontentreview
    Inherits workareabase

    Protected content_id As Long = 0
    Protected security_data As PermissionData
    Protected crReview As Ektron.Cms.ContentReviewData
    Protected aReviews() As Ektron.Cms.ContentReviewData = Array.CreateInstance(GetType(Ektron.Cms.ContentReviewData), 0)
    Protected redirectUrl As String = ""

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.QueryString("rid") <> "" Then
                m_iID = Convert.ToInt64(Request.QueryString("rid"))
            End If
            If Request.QueryString("cid") <> "" Then
                content_id = Convert.ToInt64(Request.QueryString("cid"))
            End If

            ' the redirectUrl is provided for use by ContentStatistics.aspx specifically
            ' so we can redirect back to that page after editting or deleting reviews or comments.
            If Request.QueryString("redirectUrl") <> "" Then
                redirectUrl = Request.QueryString("redirectUrl")
            End If

            CheckPermissions()

            If Page.IsPostBack Then
                Select Case MyBase.m_sPageAction
                    Case Else ' "edit"
                        Process_Edit()
                End Select
            Else
                aReviews = Me.m_refContentApi.EkContentRef.GetContentRating(m_iID, 0, 0, -1, "", "")
                If aReviews.Length > 0 Then
                    crReview = aReviews(0)
                End If
                RenderJS()
                Select Case MyBase.m_sPageAction
                    Case "delete"
                        Process_Delete()
                    Case "edit"
                        Display_Edit()
                    Case Else ' "view"
                        Display_View()
                End Select
                SetLabels()
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Protected Sub Process_Edit()
        aReviews = Me.m_refContentApi.EkContentRef.GetContentRating(Me.m_iID, 0, 0, -1, "", "")
        If aReviews.Length > 0 Then
            crReview = aReviews(0)
            crReview.UserComments = txt_review.Text
            crReview.State = drp_status_data.SelectedValue
            crReview.Rating = Request.Form("irating")
            crReview = Me.m_refContentApi.EkContentRef.UpdateContentReview(crReview)
        End If
        Dim pagemode As String = "&page=" & Request.QueryString("page")
        Try
            If (redirectUrl.Length > 0) Then
                Response.Redirect(redirectUrl, False)
            Else
                Response.Redirect("addeditcontentreview.aspx?action=view&id=" & Me.m_iID & "&cid=" & Me.content_id & pagemode, False)
            End If
        Catch ex As Exception
            ' do nothing
        End Try
    End Sub
    Protected Sub Process_Delete()
        aReviews = Me.m_refContentApi.EkContentRef.GetContentRating(Me.m_iID, 0, 0, -1, "", "")
        If aReviews.Length > 0 Then
            crReview = aReviews(0)
            Me.m_refContentApi.EkContentRef.DeleteContentReview(crReview)
        End If
        If (Request.QueryString("page") = "workarea") Then
            Try
                If (redirectUrl.Length > 0) Then
                    Response.Redirect(redirectUrl, False)
                Else
                    ' redirect to workarea
                    Response.Write("<script language=""Javascript"">" & _
                                           "top.switchDesktopTab();" & _
                                           "</script>")
                End If
            Catch ex As Exception
                ' do nothing
            End Try
        Else
            Response.Redirect("ContentStatistics.aspx?page=ContentStatistics.aspx&id=" & Me.content_id & "&LangType=" & Me.ContentLanguage, False)
        End If
    End Sub
    Protected Sub Display_Edit()
        If crReview.UserID = 0 Then
            ltr_uname_data.Text = MyBase.GetMessage("lbl anon")
        Else
            ltr_uname_data.Text = crReview.Username
        End If
        ltr_date_data.Text = crReview.RatingDate.ToLongDateString & " " & crReview.RatingDate.ToShortTimeString
        txt_review.Text = Server.HtmlDecode(crReview.UserComments)
        ltr_rating_val.Text = ShowRating(crReview.Rating, True)
        Select Case crReview.State
            Case EkEnumeration.ContentReviewState.Pending
                drp_status_data.SelectedIndex = 0
            Case EkEnumeration.ContentReviewState.Approved
                drp_status_data.SelectedIndex = 1
            Case EkEnumeration.ContentReviewState.Rejected
                drp_status_data.SelectedIndex = 2
        End Select
    End Sub
    Protected Sub Display_View()
        If crReview.UserID = 0 Then
            ltr_uname_data.Text = MyBase.GetMessage("lbl anon")
        Else
            ltr_uname_data.Text = crReview.Username
        End If
        ltr_date_data.Text = crReview.RatingDate.ToLongDateString & " " & crReview.RatingDate.ToShortTimeString
        txt_review.Text = Server.HtmlDecode(crReview.UserComments)
        ltr_rating_val.Text = ShowRating(crReview.Rating, False)
        Select Case crReview.State
            Case EkEnumeration.ContentReviewState.Pending
                drp_status_data.SelectedIndex = 0
            Case EkEnumeration.ContentReviewState.Approved
                drp_status_data.SelectedIndex = 1
            Case EkEnumeration.ContentReviewState.Rejected
                drp_status_data.SelectedIndex = 2
        End Select
    End Sub
    Protected Sub CheckPermissions()
        security_data = Me.m_refContentApi.LoadPermissions(Me.content_id, "content")
        Select Case MyBase.m_sPageAction
            Case "edit"
                If security_data.CanEdit = True Then
                    ' we are good
                Else
                    Throw New Exception("You do not have permissions to edit.")
                End If
            Case Else ' "view"
                If security_data.IsReadOnly = True Then
                    ' we are good
                Else
                    Throw New Exception("You do not have permissions to view.")
                End If
        End Select
    End Sub
    Protected Sub SetLabels()
        Me.ltr_date.Text = "Date:"
        Me.ltr_uname.Text = "Username:"
        Me.ltr_status.Text = "Status:"
        Me.ltr_rating.Text = "Rating:"
        Me.ltr_review.Text = "Review:"
        Dim pagemode As String = "&page=" & Request.QueryString("page")
        Select Case MyBase.m_sPageAction
            Case "edit"
                MyBase.SetTitleBarToString("Edit")
                MyBase.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", "alt save button text (content)", "btn save", "OnClick=""javascript:SubmitForm();return true;""")
                If (redirectUrl.Length > 0) Then
                    MyBase.AddBackButton(redirectUrl)
                Else
                    MyBase.AddBackButton("?action=view&id=" & Me.m_iID & "&cid=" & Me.content_id & pagemode)
                End If

            Case Else ' "view"
                Me.drp_status_data.Enabled = False
                Me.txt_review.Enabled = False
                MyBase.SetTitleBarToString("View Content Review " & """" & crReview.ContentTitle & """")
                If security_data.CanEdit = True Then
                    MyBase.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/contentEdit.png", "?action=edit&id=" & Me.m_iID & "&cid=" & Me.content_id & pagemode, "alt edit button text", "btn edit", "")
                    MyBase.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/delete.png", "?action=delete&id=" & Me.m_iID & "&cid=" & Me.content_id & pagemode, "btn alt del review", ("btn delete"), " onclick=""javascript:return confirm('" & MyBase.GetMessage("js conf del review") & "');"" ")
                End If
                If (Request.QueryString("page") = "workarea") Then
                    ' redirect to workarea when user clicks back button if we're in workarea
                    MyBase.AddButtonwithMessages(AppImgPath & "../UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=""javascript:top.switchDesktopTab()"" ")
                Else
                    MyBase.AddBackButton("ContentStatistics.aspx?page=ContentStatistics.aspx&id=" & Me.content_id & "&LangType=" & Me.ContentLanguage)
                End If
        End Select
        MyBase.AddHelpButton("AddEditReviews")
    End Sub

    Private Sub RenderJS()
        Dim sbJS As New StringBuilder
        sbJS.Append("<script language=""javascript"" type=""text/javascript"" >" & Environment.NewLine)
        If Me.m_sPageAction = "edit" Then
            sbJS.Append("function SubmitForm()" & Environment.NewLine)
            sbJS.Append("{" & Environment.NewLine)
            sbJS.Append("    document.forms[0].submit();" & Environment.NewLine)
            sbJS.Append("}" & Environment.NewLine)

            sbJS.Append("  function rhdl(item, act) ").Append(Environment.NewLine)
            sbJS.Append("  { ").Append(Environment.NewLine)
            sbJS.Append("  	var defrating = document.getElementById('irating').value; ").Append(Environment.NewLine)
            sbJS.Append("  	switch(act) ").Append(Environment.NewLine)
            sbJS.Append("  	{ ").Append(Environment.NewLine)
            sbJS.Append("  	case ""over"" : ").Append(Environment.NewLine)
            sbJS.Append("  	case ""out"" : ").Append(Environment.NewLine)
            sbJS.Append("  		for (var w = 0; w <= 10; w++) ").Append(Environment.NewLine)
            sbJS.Append("  		{ ").Append(Environment.NewLine)
            sbJS.Append("  			if (w == 0) { (document.getElementById('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/stop.png'; } ").Append(Environment.NewLine)
            sbJS.Append("  			else if (((w % 2) > 0) && w > item) { (document.getElementById('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starEmptyLeft.png'; } ").Append(Environment.NewLine)
            sbJS.Append("  			else if (((w % 2) > 0) && w <= item) { (document.getElementById('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starLeft.png'; } ").Append(Environment.NewLine)
            sbJS.Append("  			else if (((w % 2) == 0) && w > item) { (document.getElementById('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starEmptyRight.png'; } ").Append(Environment.NewLine)
            sbJS.Append("  			else if (((w % 2) == 0) && w <= item) { (document.getElementById('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starRight.png'; } ").Append(Environment.NewLine)
            sbJS.Append("  		} ").Append(Environment.NewLine)
            sbJS.Append("  		break; ").Append(Environment.NewLine)
            'sbJS.Append("  	case ""out"" : ").Append(Environment.NewLine)
            'sbJS.Append("  		for (var w = 0; w <= 10; w++) ").Append(Environment.NewLine)
            'sbJS.Append("  		{ ").Append(Environment.NewLine)
            'sbJS.Append("  			if (w == 0) { (document.getElementById(""img_"" + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/stop.png'; } ").Append(Environment.NewLine)
            'sbJS.Append("  			else if (((w % 2) > 0) && item > defrating) { (document.getElementByID('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starEmptyLeft.png'; } ").Append(Environment.NewLine)
            'sbJS.Append("  			else if (((w % 2) > 0) && item <= defrating) { (document.getElementByID('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starLeft.png'; } ").Append(Environment.NewLine)
            'sbJS.Append("  			else if (((w % 2) == 0) && item > defrating) { (document.getElementByID('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starEmptyRight.png'; } ").Append(Environment.NewLine)
            'sbJS.Append("  			else if (((w % 2) == 0) && item <= defrating) { (document.getElementByID('img_' + w)).src = '" & Me.m_refContentApi.AppPath & "images/UI/Icons/starRight.png'; } ").Append(Environment.NewLine)
            'sbJS.Append("  		} ").Append(Environment.NewLine)
            'sbJS.Append("  		break; ").Append(Environment.NewLine)
            sbJS.Append("  	case ""click"" : ").Append(Environment.NewLine)
            sbJS.Append("  		document.getElementById('irating').value = item; ").Append(Environment.NewLine)
            sbJS.Append("  	    if (item == 0) { document.getElementById('d_rating').innerHTML = 'No Rating'; } ").Append(Environment.NewLine)
            sbJS.Append("  	    else if ((item % 2) > 0) {  ").Append(Environment.NewLine)
            sbJS.Append("  	        if ((Math.floor(item / 2)) == 0) { document.getElementById('d_rating').innerHTML = '1/2 star'; } ").Append(Environment.NewLine)
            sbJS.Append("  	        else { document.getElementById('d_rating').innerHTML = (Math.floor(item / 2)) + ' 1/2 stars'; } ").Append(Environment.NewLine)
            sbJS.Append("  	    } else if ((item % 2) == 0) { document.getElementById('d_rating').innerHTML = Math.floor(item / 2) + ' stars'; } ").Append(Environment.NewLine)

            sbJS.Append("  		break; ").Append(Environment.NewLine)
            sbJS.Append("  	} ").Append(Environment.NewLine)
            sbJS.Append("  } ").Append(Environment.NewLine)

        End If
        sbJS.Append("</script>" & Environment.NewLine)
        ltr_js.Text &= Environment.NewLine & sbJS.ToString()
    End Sub
    Private Function ShowRating(ByVal iRating As Integer, ByVal IsEdit As Boolean) As String
        Dim sbRating As New StringBuilder()

        If IsEdit = False Then 'view
            For i As Integer = 0 To 10
                If i Mod 2 = 0 And i > 0 And i > iRating Then
                    sbRating.Append("<img src=""" & Me.m_refContentApi.AppPath & "images/UI/Icons/starEmptyRight.png"" />")
                ElseIf i Mod 2 = 0 And i > 0 And i <= iRating Then
                    sbRating.Append("<img src=""" & Me.m_refContentApi.AppPath & "images/UI/Icons/starRight.png"" />")
                ElseIf i > 0 And i > iRating Then
                    sbRating.Append("<img src=""" & Me.m_refContentApi.AppPath & "images/UI/Icons/starEmptyLeft.png"" />")
                ElseIf i > 0 And i <= iRating Then
                    sbRating.Append("<img src=""" & Me.m_refContentApi.AppPath & "images/UI/Icons/starLeft.png"" />")
                Else
                    sbRating.Append("<img src=""" & Me.m_refContentApi.AppPath & "images/UI/Icons/stop.png"" />")
                End If
            Next
        Else ' edit
            sbRating.Append("<input type=""hidden"" id=""irating"" name=""irating"" value=""" & iRating & """/>")
            For i As Integer = 0 To 10
                sbRating.Append("<img id=""img_" & i & """ name=""img_" & i & """ onmouseover=""rhdl(" & i & ",'over');"" onmouseoout=""rhdl(" & i & ",'out');"" onclick=""rhdl(" & i & ",'click');"" ")
                If i Mod 2 = 0 And i > 0 And i > iRating Then
                    sbRating.Append("src=""" & Me.m_refContentApi.AppPath & "images/UI/Icons/starEmptyRight.png"" ")
                ElseIf i Mod 2 = 0 And i > 0 And i <= iRating Then
                    sbRating.Append("src=""" & Me.m_refContentApi.AppPath & "images/UI/Icons/starRight.png"" ")
                ElseIf i > 0 And i > iRating Then
                    sbRating.Append("src=""" & Me.m_refContentApi.AppPath & "images/UI/Icons/starEmptyLeft.png"" ")
                ElseIf i > 0 And i <= iRating Then
                    sbRating.Append("src=""" & Me.m_refContentApi.AppPath & "images/UI/Icons/starLeft.png"" ")
                Else
                    sbRating.Append("src=""" & Me.m_refContentApi.AppPath & "images/UI/Icons/stop.png"" ")
                End If
                sbRating.Append("/>")
            Next
        End If
        If iRating = 0 Then
            sbRating.Append("&nbsp;&nbsp;<span id=""d_rating"" name=""d_rating"">No rating</span>")
        ElseIf iRating = 1 Then
            sbRating.Append("&nbsp;&nbsp;<span id=""d_rating"" name=""d_rating"">1/2 star</span>")
        ElseIf iRating Mod 2 > 0 Then
            sbRating.Append("&nbsp;&nbsp;<span id=""d_rating"" name=""d_rating"">" & (iRating \ 2) & IIf(iRating > 2, " 1/2 stars", " star") & "</span>")
        ElseIf iRating Mod 2 = 0 Then
            sbRating.Append("&nbsp;&nbsp;<span id=""d_rating"" name=""d_rating"">" & (iRating \ 2) & IIf(iRating > 2, " stars", " star") & "</span>")
        End If

        Return sbRating.ToString()
    End Function
End Class