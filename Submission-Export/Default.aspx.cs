using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.WebSearch.SearchData;
using Ektron.Cms.API.Search;
using System.Xml;
using System.Data;
using System.IO;
using Ektron.Cms.API;
using Ektron.Cms.API.User;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

public partial class _Default : System.Web.UI.Page
{
	public XmlDocument xd = new XmlDocument();
	public StringBuilder sb1 = new StringBuilder();
	private User userAPI = new User();
    private long userId;
	
    protected void Page_Load(object sender, EventArgs e)
    {
		this.userId = new Ektron.Cms.API.User.User().UserId;
		
		// If there's no user logged in redirect to login page
        if (userId == 0)
        {
			loggedin.Visible = false;
		}
		else {
			loggedout.Visible = false;
			
			DateTime thisDay = DateTime.Today;
			
			if(startdate.Text == "") {
				startdate.Text = (thisDay.AddDays(-30)).ToString("d");
			}
			
			if(enddate.Text == "") {
				enddate.Text = thisDay.ToString("d");
			}
		}
    }
	
	protected void updateVisible(object sender, EventArgs e)
    {	
		if(report.SelectedItem.Value == "contact")
			GetContactUs("show");
		else if(report.SelectedItem.Value == "Newsletter")
			GetNewsletter("show");
		else if(report.SelectedItem.Value == "Combi")
			GetCombi("show"); //dev 132 live 136
		else if(report.SelectedItem.Value == "Simulator")
			GetSimulator("show");  //dev 133 live 137
	}
	
	public void GetContactUs(string option)
    {
		SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
		
		try
			{
				myConnection.Open();
				
				string sql = "select * from Vulcan_Contact_Us with (nolock) " 
									 + "where date_submitted >= '" + startdate.Text + "' " 
									 + "and date_submitted <= '" + enddate.Text + "'";
				
				 SqlDataAdapter myCommand = new SqlDataAdapter(sql, myConnection);
									  
				  // Create and fill a DataSet.
				 if(option == "show") {
					  DataSet ds = new DataSet();
					  myCommand.Fill(ds);
					  DataView source = new DataView(ds.Tables[0]);
					  myDataGrid.DataSource = source;
					  myDataGrid.DataBind();
				 }
				  
				myConnection.Close();
		}
		catch(Exception ex)
		{
			//comment.Text = ex.ToString();
		}
	}
	
	public void GetNewsletter(string option)
    {
		SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
		
		try
			{
				myConnection.Open();
				
				string sql = "select * from Vulcan_Newsletter with (nolock) " 
									 + "where date_submitted >= '" + startdate.Text + "' " 
									 + "and date_submitted <= '" + enddate.Text + "'";
				
				 SqlDataAdapter myCommand = new SqlDataAdapter(sql, myConnection);
									  
				  // Create and fill a DataSet.
				 if(option == "show") {
					  DataSet ds = new DataSet();
					  myCommand.Fill(ds);
					  DataView source = new DataView(ds.Tables[0]);
					  myDataGrid.DataSource = source;
					  myDataGrid.DataBind();
				 }
				  
				myConnection.Close();
		}
		catch(Exception ex)
		{
			//comment.Text = ex.ToString();
		}
	}
	
	public void GetCombi(string option)
    {
		SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
		
		try
			{
				myConnection.Open();
				
				string sql = "select * from Vulcan_Combi with (nolock) " 
									 + "where date_submitted >= '" + startdate.Text + "' " 
									 + "and date_submitted <= '" + enddate.Text + "'";
				
				 SqlDataAdapter myCommand = new SqlDataAdapter(sql, myConnection);
									  
				  // Create and fill a DataSet.
				 if(option == "show") {
					  DataSet ds = new DataSet();
					  myCommand.Fill(ds);
					  DataView source = new DataView(ds.Tables[0]);
					  myDataGrid.DataSource = source;
					  myDataGrid.DataBind();
				 }
				  
				myConnection.Close();
		}
		catch(Exception ex)
		{
			//comment.Text = ex.ToString();
		}
	}
	
	public void GetSimulator(string option)
    {
		SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
		
		try
			{
				myConnection.Open();
				
				string sql = "select * from Vulcan_Simulator with (nolock) " 
									 + "where date_submitted >= '" + startdate.Text + "' " 
									 + "and date_submitted <= '" + enddate.Text + "'";
				
				 SqlDataAdapter myCommand = new SqlDataAdapter(sql, myConnection);
									  
				  // Create and fill a DataSet.
				 if(option == "show") {
					  DataSet ds = new DataSet();
					  myCommand.Fill(ds);
					  DataView source = new DataView(ds.Tables[0]);
					  myDataGrid.DataSource = source;
					  myDataGrid.DataBind();
				 }
				  
				myConnection.Close();
		}
		catch(Exception ex)
		{
			//comment.Text = ex.ToString();
		}
	}
	
	protected void Export_Click(object sender, EventArgs e)
    {
		Response.ContentType = "application/vnd.ms-excel";
		Response.AddHeader("content-disposition", "attachment;filename=vulcan-submissions.xls");
		Response.Charset = "";
		this.EnableViewState = false;
		System.IO.StringWriter stringWriter = new System.IO.StringWriter();
		System.Web.UI.HtmlTextWriter htmlWriter = new System.Web.UI.HtmlTextWriter(stringWriter);
		myDataGrid.RenderControl(htmlWriter);
		Response.Write(stringWriter.ToString());
		Response.End();
	}
}