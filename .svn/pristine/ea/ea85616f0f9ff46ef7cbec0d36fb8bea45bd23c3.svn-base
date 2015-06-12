using System;

public partial class _Default : System.Web.UI.Page
{
	
    protected void Page_Load(object sender, EventArgs e)
    {
		if(Request.QueryString["id"].ToString() == "1466")
		{
			detail.Visible = false;
		}
		else
		{
			overview.Visible = false;
			title.Text = testimonial_article.EkItem.Title;
		}
    }
}