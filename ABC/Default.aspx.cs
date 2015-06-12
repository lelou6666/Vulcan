using System;

public partial class _Default : System.Web.UI.Page
{
	
    protected void Page_Load(object sender, EventArgs e)
    {
	Response.Status = "301 Moved Permanently";
	Response.AddHeader("Location","http://vulcanequipment.com/products/combi/simulator/");	
    }
}