using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Success : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		int phase =  Convert.ToInt32(Request.QueryString["phase"]);
		if(phase == 1){
			btnPhaseinside2.Visible = false;
			lblCopy.Text = "Registration complete. Begin 101 Series Tutorial.";
		}
		else
		{
			btnPhaseinside1.Visible = false;
			lblCopy.Text = "Registration complete. Begin 201 Series Tutorial.";
		}
    }
    //protected void btnStartPhase1_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect("quiz.aspx?phase=1");
    //}
    protected void btnPhaseinside1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Phase1Slides/slide1.htm", false);
    }
    protected void btnPhaseinside2_Click(object sender, EventArgs e)
    {
        Response.Redirect("Phase2Slides/slide1.htm", false);
    }
}