using System;
using System.Data.OleDb;
using System.Data;
using System.Text;

public partial class _DoWork : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		// Create connection string variable. Modify the "Data Source"
		// parameter as appropriate for your environment.
		String sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
			"Data Source=" + Server.MapPath("active_prods_vulcan.xls") + ";" +
			"Extended Properties=Excel 8.0;";
		
		// Create connection object by using the preceding connection string.
		OleDbConnection objConn = new OleDbConnection(sConnectionString);
		
		// Open connection with the database.
		objConn.Open();
		
		// The code to follow uses a SQL SELECT command to display the data from the worksheet.
		
		// Create new OleDbCommand to return data from worksheet.
		OleDbCommand objCmdSelect =new OleDbCommand("SELECT * FROM content", objConn);
		
		// Create new OleDbDataAdapter that is used to build a DataSet
		// based on the preceding SQL SELECT statement.
		OleDbDataAdapter objAdapter1 = new OleDbDataAdapter();
		
		// Pass the Select command to the adapter.
		objAdapter1.SelectCommand = objCmdSelect;
		
		// Create new DataSet to hold information from the worksheet.
		DataSet objDataset1 = new DataSet();
		
		// Fill the DataSet with the information from the worksheet.
		objAdapter1.Fill(objDataset1, "XLData");
		
		// Bind data to DataGrid control.
		GridView1.DataSource = objDataset1.Tables[0].DefaultView;
		GridView1.DataBind();
		
		// Clean up objects.
		objConn.Close();
    }
    
    protected void processWork(object sender, EventArgs e)
    {
		/*for(int i = 1101; i <= 1200; i++)
		{
			StringBuilder sb = new StringBuilder();
			string model = "";
			string configuration = "";
			string base_unit = "";
			string lbs = "";
			string kgs = "";
			string depth = "";
			string width = "";
			string height = "";
			string capacity = "";
			string input = "";
			string description = "";	
			
			if(GridView1.Rows[i].Cells[3].Text != "NULL")
			{
				model = Server.HtmlDecode(GridView1.Rows[i].Cells[3].Text);
			}
			
			if(GridView1.Rows[i].Cells[1].Text != "NULL")
			{
				configuration = Server.HtmlDecode(GridView1.Rows[i].Cells[1].Text);
			}
			
			if(GridView1.Rows[i].Cells[2].Text != "NULL")
			{
				base_unit = Server.HtmlDecode(GridView1.Rows[i].Cells[2].Text);
			}
			
			if(GridView1.Rows[i].Cells[4].Text != "NULL")
			{
				lbs = Server.HtmlDecode(GridView1.Rows[i].Cells[4].Text);
			}
			
			if(GridView1.Rows[i].Cells[5].Text != "NULL")
			{
				kgs = Server.HtmlDecode(GridView1.Rows[i].Cells[5].Text);
			}
			
			if(GridView1.Rows[i].Cells[6].Text != "NULL")
			{
				depth = Server.HtmlDecode(GridView1.Rows[i].Cells[6].Text);
			}
			
			if(GridView1.Rows[i].Cells[7].Text != "NULL")
			{
				width = Server.HtmlDecode(GridView1.Rows[i].Cells[7].Text);
			}
			
			if(GridView1.Rows[i].Cells[8].Text != "NULL")
			{
				height = Server.HtmlDecode(GridView1.Rows[i].Cells[8].Text);
			}
			
			if(GridView1.Rows[i].Cells[12].Text != "NULL")
			{
				capacity = Server.HtmlDecode(GridView1.Rows[i].Cells[12].Text);
			}
			
			if(GridView1.Rows[i].Cells[13].Text != "NULL")
			{
				input = Server.HtmlDecode(GridView1.Rows[i].Cells[13].Text);
			}
			
			if(GridView1.Rows[i].Cells[9].Text != "NULL")
			{
				description = Server.HtmlDecode(GridView1.Rows[i].Cells[9].Text);
			}
			
			if(GridView1.Rows[i].Cells[11].Text != "NULL")
			{
				description += Server.HtmlDecode(GridView1.Rows[i].Cells[11].Text);
			}
			
			Ektron.Cms.Framework.Core.Content.Content capi = new Ektron.Cms.Framework.Core.Content.Content(Ektron.Cms.Framework.ApiAccessMode.Admin);
			Ektron.Cms.ContentData cd = new Ektron.Cms.ContentData();

			cd.Title = model; // + title
			cd.FolderId = 78; 
	       
			sb.Append("<root>");
			sb.Append(string.Format("<model>{0}</model>", model));
			sb.Append(string.Format("<configuration>{0}</configuration>", configuration));
			sb.Append(string.Format("<base_unit>{0}</base_unit>", base_unit));
			sb.Append(string.Format("<lbs>{0}</lbs>", lbs));
			sb.Append(string.Format("<kgs>{0}</kgs>", kgs));
			sb.Append(string.Format("<depth>{0}</depth>", depth));
			sb.Append(string.Format("<width>{0}</width>", width));
			sb.Append(string.Format("<height>{0}</height>", height));
			sb.Append(string.Format("<capacity>{0}</capacity>", capacity));
			sb.Append(string.Format("<input>{0}</input>", input));
			sb.Append(string.Format("<description>{0}</description>", description));
			sb.Append("</root>");
	        
	        
			cd.Html = sb.ToString();
	        
	        
	        
			cd.XmlConfiguration = new Ektron.Cms.XmlConfigData { Id = 6 };

			capi.Add(cd);
			
			blah.Text = model;
        }
            */
    }
}
