using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Data.SqlClient;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "val", "fnOnUpdateValidators();");
		
		thanks.Visible = false;
    }
	
	protected void Submit_Click(object sender, EventArgs e)
    {
        Boolean passValidation = true;
        
        if (Page.IsValid)
        {
			if (CreateContent() == true)
			{
				//SaveToDB();
				//EmailNotification();
				combiForm.Visible = false;
				thanks.Visible = true;
				//Response.Redirect("/Contact-Us/Success.aspx");
			}
			else
			{
				//lbError.Text = "There was a problem submitting your request.  Please try again.";
			} 
        } 
    }
	
	public void SaveToDB()
    {
		SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
		
		try {
			myConnection.Open();
													 
			String query = "INSERT INTO Vulcan_Simulator (first,last,email,phone,title,district,city,state,zipcode,comments,request_more,request_demo,sales_rep) "		
						 + "VALUES(@first,@last,@email,@phone,@title,@district,@city,@state,@zipcode,@comments,@request_more,@request_demo,@sales_rep)";

			SqlCommand command = new SqlCommand(query, myConnection);
			command.Parameters.Add("@first",first.Text);
			command.Parameters.Add("@last",last.Text);
			command.Parameters.Add("@email",email.Text);
			command.Parameters.Add("@phone",phone.Text);
			command.Parameters.Add("@title",title.Text);
			command.Parameters.Add("@district",district.Text);
			command.Parameters.Add("@city",city.Text);
			command.Parameters.Add("@state",state.Text);
			command.Parameters.Add("@zipcode",zipcode.Text);
			command.Parameters.Add("@comments",question.Text);
			command.Parameters.Add("@request_more",cb1.Checked);
			command.Parameters.Add("@request_demo",cb2.Checked);
			command.Parameters.Add("@sales_rep",cb3.Checked);
			
			command.ExecuteNonQuery();
												
			myConnection.Close();
		}
		catch(Exception ex)
		{
			//question.Text = ex.ToString();
		}
	}
    
    public bool CreateContent()
    {
        try
        {
            Ektron.Cms.Framework.Core.Content.Content capi = new Ektron.Cms.Framework.Core.Content.Content(Ektron.Cms.Framework.ApiAccessMode.Admin);
            Ektron.Cms.ContentData cd = new Ektron.Cms.ContentData();

            cd.Title = DateTime.Now.ToString("yyyy-MM-dd - ") + first.Text + "_" + last.Text; // title
            //cd.FolderId = 133; //dev
            cd.FolderId = 137; //Live
            cd.Html = CreateXml();
            //cd.XmlConfiguration = new Ektron.Cms.XmlConfigData { Id = 15 }; //dev
            cd.XmlConfiguration = new Ektron.Cms.XmlConfigData { Id = 15 }; //live

            capi.Add(cd);
            if (cd.Id != null && cd.Id > 1)
            {
                (new Ektron.Cms.API.Content.Content()).CheckOutContent(cd.Id);
            }
        
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public string CreateXml()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<root>");
        sb.Append(string.Format("<first>{0}</first>", first.Text));
        sb.Append(string.Format("<last>{0}</last>", last.Text));
        sb.Append(string.Format("<email>{0}</email>", email.Text));
        sb.Append(string.Format("<phone>{0}</phone>", phone.Text));
        sb.Append(string.Format("<title>{0}</title>", HttpUtility.HtmlEncode(title.Text)));
        sb.Append(string.Format("<school>{0}</school>", HttpUtility.HtmlEncode(district.Text)));
		sb.Append(string.Format("<city>{0}</city>", city.Text)); 
		sb.Append(string.Format("<state>{0}</state>", state.Text)); 
		sb.Append(string.Format("<zipcode>{0}</zipcode>", zipcode.Text)); 
        sb.Append(string.Format("<questions>{0}</questions>", HttpUtility.HtmlEncode(question.Text)));
		sb.Append(string.Format("<requestmore>{0}</requestmore>", cb1.Checked)); 
		sb.Append(string.Format("<requestdemo>{0}</requestdemo>", cb2.Checked)); 
		sb.Append(string.Format("<salesrep>{0}</salesrep>", cb3.Checked)); 
        sb.Append("</root>");

        return sb.ToString();
    }
	
	public void EmailNotification()
    {
		Ektron.Cms.CommonApi mailformat = new Ektron.Cms.CommonApi();
        Ektron.Cms.Common.EkMailService mailclient = mailformat.EkMailRef;
		string message = "";
		string EmailTo = "Raymond.Bittikofer@itwfeg.com, David.Sager@itwfeg.com";
		string EmailCC = "";
		
		message = "<span style='font-family: Arial, Verdana; font-size:12px;'>"
 				+ "The following individual has submitted a request to win a Combi.<br /><br />"
				+ "<strong>Email Address</strong>: " + email.Text
				+ "<br /><strong>First Name</strong>: " + first.Text
				+ "<br /><strong>Last Name</strong>: " + last.Text
				+ "<br /><strong>Title</strong>: " + title.Text
				+ "<br /><strong>Phone</strong>: " + phone.Text
				+ "<br /><strong>School/District</strong>: " + district.Text
				+ "<br /><strong>City</strong>: " + city.Text
				+ "<br /><strong>State</strong>: " + state.Text
				+ "<br /><strong>Zip Code</strong>: " + zipcode.Text
				+ "<br /><strong>Question(s)</strong>: " + question.Text
				+ "<br /><strong>Send more information on the combi oven</strong>: " + cb1.Checked
				+ "<br /><strong>Requesting a demo</strong>: " + cb2.Checked
				+ "<br /><strong>Have a sales rep contact them</strong>: " + cb3.Checked;
		
		mailclient.MailFrom = "noreply@vulcanequipment.com";
        mailclient.MailSubject = "Win a Combi - Submission";
        mailclient.MailBodyText = message;
        mailclient.MailTo = EmailTo;
       // mailclient.MailCC = EmailCC;
        
        mailclient.SendMail();
	}
}