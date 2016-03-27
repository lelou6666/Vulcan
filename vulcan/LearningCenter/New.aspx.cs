using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class New : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnPhase1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Registration.aspx");
    }
    protected void btnPhase2_Click(object sender, EventArgs e)
    {
        Response.Redirect("Registration.aspx");
    }
}