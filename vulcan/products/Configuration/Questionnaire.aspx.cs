using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text;

public partial class Questionnaire : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //Move question answers to session to reference by results page
		Session["size"] = Request.Form["question_1"];
		Session["surveyQuestion2"] = Request.Form["question_2"];
		Session["surveyQuestion3"] = Request.Form["question_3"];
		
		//Redirect
		Response.Redirect("ProductList.aspx", false);
    }
}