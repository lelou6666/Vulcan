Partial Class communitygroupaddeditsplash
    Inherits System.Web.UI.Page

    Protected tlang As Integer = 0
    Protected tid As Long = 0
    Protected profileTaxonomyId As Long = 0

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim sbMoment As New StringBuilder()
        Dim sURL As String = ""
        Dim api As New Ektron.Cms.CommonApi()
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(api.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking)) Then
            Utilities.ShowError(api.EkMsgRef.GetMessage("feature locked error"))
        End If
        tlang = api.DefaultContentLanguage
        Try
            If Request.QueryString("tlang") <> "" AndAlso IsNumeric(Request.QueryString("tlang")) AndAlso Request.QueryString("tlang") > 0 Then
                tlang = Request.QueryString("tlang")
            End If
            If (tlang = Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED Or tlang = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES) Then
                tlang = api.DefaultContentLanguage
            End If
            If Request.QueryString("tid") <> "" AndAlso IsNumeric(Request.QueryString("tid")) AndAlso Request.QueryString("tid") > 0 Then
                tid = Request.QueryString("tid")
            End If
            If Request.QueryString("profileTaxonomyId") <> "" AndAlso IsNumeric(Request.QueryString("profileTaxonomyId")) AndAlso Request.QueryString("profileTaxonomyId") > 0 Then
                profileTaxonomyId = Request.QueryString("profileTaxonomyId")
            End If
            sURL = "communitygroupaddedit.aspx?thickbox=true" & IIf(tid > 0, "&tid=" & tid.ToString(), "") & IIf(tlang > 0, "&LangType=" & tlang.ToString(), "") & IIf(profileTaxonomyId > 0, "&profileTaxonomyId=" & profileTaxonomyId.ToString(), "")
            sbMoment.Append("One Moment Please...").Append(Environment.NewLine)
            sbMoment.Append("<script type=""text/javascript"" language=""Javascript"">").Append(Environment.NewLine)
            sbMoment.Append("   setTimeout(""location.href='").Append(sURL).Append("'"",1000); ").Append(Environment.NewLine)
            sbMoment.Append("</script>").Append(Environment.NewLine)

            ltr_go.Text = sbMoment.ToString()
        Catch ex As Exception

        Finally
            sbMoment = Nothing
        End Try

    End Sub

End Class
