using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;

public partial class Admin : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection("Server=FSTROYSQL1; Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=vulcancms;Password=Vulcan123");
    protected void Page_Load(object sender, EventArgs e)
    {
        DateTime currentDateTime = DateTime.Now;
        DateTime timeMin = currentDateTime.Date;
        DateTime timeMax = currentDateTime.Date.AddDays(1).AddTicks(-1);
        SqlCommand cmd = new SqlCommand("select * from testresult where createddate between '" + timeMin + "' and '" + timeMax + "' order by createddate asc", con);
        SqlDataAdapter sda = new SqlDataAdapter(cmd);
        DataSet ds = new DataSet();
        sda.Fill(ds);
        gvResults.DataSource = ds;
        gvResults.DataBind();

    }
    protected void btnExportToCsv_Click(object sender, EventArgs e)
    {        
        DateTime from = Convert.ToDateTime(txtFrom.Text);
        DateTime to = Convert.ToDateTime(txtTo.Text);
        DateTime timeMin = from.Date;
        DateTime timeMax = to.Date.AddDays(1).AddTicks(-1);
        SqlCommand cmd = new SqlCommand("select * from testresult where createddate between '" + timeMin + "' and '" + timeMax + "' order by createddate asc", con);
        SqlDataAdapter sda = new SqlDataAdapter(cmd);
        DataTable ds = new DataTable();
        sda.Fill(ds);
        gvResults.DataSource = ds;
        gvResults.DataBind();
        CreateCSVFile(ds, "c:\\csvData.csv");
    }
    public void CreateCSVFile(DataTable dt, string strFilePath)
    {
        //StreamWriter sw = new StreamWriter(strFilePath, false);
        //int iColCount = dt.Columns.Count;
        //for (int i = 0; i < iColCount; i++)
        //{
        //    sw.Write(dt.Columns[i]);
        //    if (i < iColCount - 1)
        //    {
        //        sw.Write(",");
        //    }
        //}
        //sw.Write(sw.NewLine);       
        //foreach (DataRow dr in dt.Rows)
        //{
        //    for (int i = 0; i < iColCount; i++)
        //    {
        //        if (!Convert.IsDBNull(dr[i]))
        //        {
        //            sw.Write(dr[i].ToString());
        //        }
        //        if (i < iColCount - 1)
        //        {
        //            sw.Write(",");
        //        }
        //    }
        //    sw.Write(sw.NewLine);
        //}
        //sw.Close();         


        //string filename = Server.MapPath("~/download.csv");
        //StreamWriter sw = new StreamWriter(filename, false);

        //int iColCount = dt.Columns.Count;
        ////for (int i = 0; i < iColCount; i++)
        ////{
        ////    sw.Write(dt.Columns[i]);
        ////    if (i < iColCount - 1)
        ////    {
        ////        sw.Write(",");
        ////    }
        ////}
        ////sw.Write(sw.NewLine);

        //foreach (DataRow dr in dt.Rows)
        //{
        //    for (int i = 0; i < iColCount; i++)
        //    {
        //        if (!Convert.IsDBNull(dr[i]))
        //        {
        //            sw.Write(dr[i].ToString());
        //        }
        //        if (i < iColCount - 1)
        //        {
        //            sw.Write(",");
        //        }
        //    }
        //    sw.Write(sw.NewLine);
        //}
        //sw.Close();

        //Response.Clear();
        //Response.ContentType = "application/csv";
        //Response.AddHeader("Content-Disposition", "attachment; filename=download.csv");
        //Response.WriteFile(filename);
        //Response.Flush();
        //Response.End();


        HttpContext context = HttpContext.Current;
        context.Response.Clear();
        foreach (DataColumn column in dt.Columns)
        {
            context.Response.Write(column.ColumnName + ",");
        }
        context.Response.Write(Environment.NewLine);
        foreach (DataRow row in dt.Rows)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                context.Response.Write(row[i].ToString().Replace(",", string.Empty) + ",");
            }
            context.Response.Write(Environment.NewLine);
        }
        context.Response.ContentType = "text/csv";
        context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + "TestResults" + ".csv");
        context.Response.End();

    }
   
}