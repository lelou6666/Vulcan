using System;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Data;

public partial class ProductList : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
    {
		//Determine which sizes to show
		int rangeSize = Convert.ToInt32(Session["size"]);
		switch (rangeSize) {
			case 24:
				range1.Visible = true;
				range2.Visible = false;
				range3.Visible = false;
				range4.Visible = false;
				range5.Visible = false;
			break;
			case 36:
				range1.Visible = true;
				range2.Visible = true;
				range3.Visible = false;
				range4.Visible = false;
				range5.Visible = false;
			break;
			case 48:
				range1.Visible = true;
				range2.Visible = true;
				range3.Visible = true;
				range4.Visible = false;
				range5.Visible = false;
			break;
			case 60:
				range1.Visible = true;
				range2.Visible = true;
				range3.Visible = true;
				range4.Visible = true;
				range5.Visible = false;
			break;
			case 72:
			default:
				range1.Visible = true;
				range2.Visible = true;
				range3.Visible = true;
				range4.Visible = true;
				range5.Visible = true;
			break;
		}
    }
}