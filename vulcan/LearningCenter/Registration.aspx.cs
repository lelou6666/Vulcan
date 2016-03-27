using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


public partial class Registration : System.Web.UI.Page
{
//SqlConnection con =new SqlConnection();

    SqlConnection con = new SqlConnection("Server=FSTROYSQL1; Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=vulcancms;Password=Vulcan123");
	
    protected void Page_Load(object sender, EventArgs e)
    {
		int Uid;

		if(Session["UserId"] != null) { 
			Uid = Convert.ToInt32(Session["UserId"].ToString());
			GetUserName(Uid);
		}
		
		
    }
	
	public void GetUserName(int UserId)
    {
        string QryUserDetails = "select * from users where UserId=" + UserId;
        SqlDataAdapter sdaUserDetail = new SqlDataAdapter(QryUserDetails, con);
        DataSet dsUserDetail = new DataSet();
        sdaUserDetail.Fill(dsUserDetail, "UserDetail");
        if (dsUserDetail.Tables[0].Rows.Count > 0)
        {
            DataRow dtrUserDetail;
            int j = 0;
            while (j < dsUserDetail.Tables[0].Rows.Count)
            {
                dtrUserDetail = dsUserDetail.Tables[0].Rows[j];
                if (dtrUserDetail["FirstName"].ToString() != null)
                {
                    txtFirstName.Text = dtrUserDetail["FirstName"].ToString();                   
                }
				if (dtrUserDetail["LastName"].ToString() != null)
                {
                    txtLastName.Text = dtrUserDetail["LastName"].ToString();                   
                }
				if (dtrUserDetail["Title"].ToString() != null)
                {
                    txtTitle.Text = dtrUserDetail["Title"].ToString();                   
                }
				if (dtrUserDetail["Email"].ToString() != null)
                {
                    txtEmail.Text = dtrUserDetail["Email"].ToString();                   
                }
				if (dtrUserDetail["City"].ToString() != null)
                {
                    txtCity.Text = dtrUserDetail["City"].ToString();                   
                }
				if (dtrUserDetail["State"].ToString() != null)
                {
                    txtState.Text = dtrUserDetail["State"].ToString();                   
                }
				if (dtrUserDetail["Zip"].ToString() != null)
                {
                    txtZip.Text = dtrUserDetail["Zip"].ToString();                   
                }
				if (dtrUserDetail["Experience"].ToString() != null)
                {
                    string experience = dtrUserDetail["Experience"].ToString();  
					
					if(experience == "0-2")  
					{
						rblExperience.SelectedIndex = 0;
					}
					else if(experience == "2-5")  
					{
						rblExperience.SelectedIndex = 1;
					}
					else if(experience == "5-10")  
					{
						rblExperience.SelectedIndex = 2;
					}
					else if(experience == "10+")  
					{
						rblExperience.SelectedIndex = 3;
					}
                }
				
                j++;
            }
        }
    }
	
    protected void btnGo_Click(object sender, EventArgs e)
    {
		if (Page.IsValid) {
			string Date = DateTime.Now.ToString();
			string str = "insert into users (FirstName, LastName ,Title ,Email , Experience, City, State, Zip, CreatedDate) values ('" + txtFirstName.Text + "','" + txtLastName.Text + "'";
			str += ",'" + txtTitle.Text + "','" + txtEmail.Text + "','" + rblExperience.SelectedItem.Value + "','" + txtCity.Text + "','" + txtState.Text + "'";
			str += ",'" + txtZip.Text + "','" + Date + "')";
	
			SqlCommand cmd = new SqlCommand(str, con);
			con.Open();
			cmd.ExecuteNonQuery();
			con.Close();
			//RegistrationForm.Visible = false;
			//Success.Attributes["src"] = "Success.aspx";
	
	
			getuserid(txtEmail.Text, Date);
			Response.Redirect("Success.aspx?phase=" + Request.QueryString["phase"]);
		}
    }
    public void getuserid(string email, string date)
    {
        string qryUserId = "select * from users where Email='"+email+"' and CreatedDate='" + date + "'";
        SqlDataAdapter sdaUserid = new SqlDataAdapter(qryUserId, con);
        DataSet dsUserId = new DataSet();
        sdaUserid.Fill(dsUserId, "Users");
        if (dsUserId.Tables[0].Rows.Count > 0 && dsUserId.Tables[0].Rows.Count == 1)
        {
            DataRow dtrUserId;
            int j = 0;
            while (j < dsUserId.Tables[0].Rows.Count)
            {
                dtrUserId = dsUserId.Tables[0].Rows[j];               
                Session["UserId"] = dtrUserId["Userid"].ToString();
                j++;
            }
        }
    }
}