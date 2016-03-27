
Imports Ektron.Cms.Controls

Partial Class Workarea_EditUserProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim userID As Long = 0
        Long.TryParse(Request.QueryString("id"), userID)
        If (Membership1.Text = "") Then
            Response.Write("Please login to see your Profile.")
            'Response.Redirect(api.SitePath & "login.aspx", True)
            Exit Sub
        End If
        If (Page.IsPostBack AndAlso Membership1.LocalizeString(Membership1.UserUpdateSuccessMessage).Trim() = Membership1.Text.Trim()) Then

            Session.Remove("Ektron.eIntranet." + UserID.ToString() + ".userdata")
            Dim sbJScript As New StringBuilder
            sbJScript.Append("if (window.parent.document.getElementById('Ek_MemberEditRedirectUrlValue') != null){").AppendLine(Environment.NewLine)
            sbJScript.Append("  parent.location.href = window.parent.document.getElementById('Ek_MemberEditRedirectUrlValue').value").AppendLine(Environment.NewLine)
            sbJScript.Append("}else{").AppendLine(Environment.NewLine)
            sbJScript.Append("  parent.location.href = parent.location.href").AppendLine(Environment.NewLine)
            sbJScript.Append("}").AppendLine(Environment.NewLine)
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "refreshpage", sbJScript.ToString(), True)
        End If
        If (Request.QueryString("taxonomyId") IsNot Nothing AndAlso Request.QueryString("taxonomyId") <> "") Then
            Me.Membership1.TaxonomyId = Convert.ToInt64(Request.QueryString("taxonomyId"))
        End If
    End Sub
End Class
