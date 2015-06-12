using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

public partial class _Default : System.Web.UI.Page
{
	
    protected void Page_Load(object sender, EventArgs e)
    {
		Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "val", "fnOnUpdateValidators();");
    }
	
	protected void Submit_Click(object sender, EventArgs e)
    {
		if (Page.IsValid)
		{
			SaveToDB(); //comments out for DEV ONLY
			SendToPowerObjects();
			Response.Redirect("/ranges-sem/thank-you/");
			
			/* if (CreateContent() == true)
			{
				//EmailNotification();
				
				SendToPowerObjects();
				
				Response.Redirect("/ranges-sem/thank-you/");
			}
			else
			{
				//lbError.Text = "There was a problem submitting your request.  Please try again.";
			} */
		}
	}
	
	public bool CreateContent()
    {
        try
        {
            Ektron.Cms.Framework.Core.Content.Content capi = new Ektron.Cms.Framework.Core.Content.Content(Ektron.Cms.Framework.ApiAccessMode.Admin);
            Ektron.Cms.ContentData cd = new Ektron.Cms.ContentData();

            cd.Title = DateTime.Now.ToString("yyyy-MM-dd - ") + first.Text + "_" + last.Text; // title
            cd.FolderId = 134; //dev
			//cd.FolderId = 146; // Live
            cd.Html = CreateXml();
            cd.XmlConfiguration = new Ektron.Cms.XmlConfigData { Id = 17 }; //dev + Live

            //capi.Add(cd);
        
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
        sb.Append(string.Format("<first>{0}</first>", HttpUtility.HtmlEncode(first.Text)));
        sb.Append(string.Format("<last>{0}</last>", HttpUtility.HtmlEncode(last.Text)));
		sb.Append(string.Format("<company>{0}</company>", HttpUtility.HtmlEncode(company.Text)));
        sb.Append(string.Format("<phone>{0}</phone>", HttpUtility.HtmlEncode(phone.Text)));
		sb.Append(string.Format("<zipcode>{0}</zipcode>", HttpUtility.HtmlEncode(zipcode.Text))); 
		sb.Append(string.Format("<email>{0}</email>", HttpUtility.HtmlEncode(email.Text)));
		sb.Append(string.Format("<product>{0}</product>", "Ranges"));
        sb.Append(string.Format("<comments>{0}</comments>", HttpUtility.HtmlEncode(comments.Text)));
        sb.Append("</root>");

        return sb.ToString();
    }
	
	public void SaveToDB()
    {
		SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
		
		try {
			myConnection.Open();
													 
			String query = "INSERT INTO Quote_Request (product,first,last,company,phone,zipcode,email,comments) "		
						 + "VALUES(@product,@first,@last,@company,@phone,@zipcode,@email,@comments)";

			SqlCommand command = new SqlCommand(query, myConnection);
			command.Parameters.Add("@product","Ranges");
			command.Parameters.Add("@first",first.Text);
			command.Parameters.Add("@last",last.Text);
			command.Parameters.Add("@company",company.Text);
			command.Parameters.Add("@phone",phone.Text);
			command.Parameters.Add("@zipcode",zipcode.Text);
			command.Parameters.Add("@email",email.Text);
			command.Parameters.Add("@comments",comments.Text);
			
			command.ExecuteNonQuery();
												
			myConnection.Close();
		}
		catch(Exception ex)
		{
			//comments.Text = ex.ToString();
		}
	}
	
	public void EmailNotification()
    {
		Ektron.Cms.CommonApi mailformat = new Ektron.Cms.CommonApi();
        Ektron.Cms.Common.EkMailService mailclient = mailformat.EkMailRef;
		string message = "";
		string EmailTo = "Chris.stern@vulcanhart.com, Joe.Maresca@vulcanhart.com";
		//string EmailTo = "reneeh@marriner.com";
		
		message = "<span style='font-family: Arial, Verdana; font-size:12px;'>"
 				+ "The following individual has submitted a request for a range quote.<br /><br />"
				+ "<strong>First Name</strong>: " + first.Text
				+ "<br /><strong>Last Name</strong>: " + last.Text
				+ "<br /><strong>Company</strong>: " + company.Text
				+ "<br /><strong>Phone</strong>: " + phone.Text
				+ "<br /><strong>Zip Code</strong>: " + zipcode.Text
				+ "<br /><strong>Email Address</strong>: " + email.Text
				+ "<br /><strong>comments</strong>:<br>" + comments.Text;
		
		mailclient.MailFrom = "contactus@vulcanequipment.com";
        mailclient.MailSubject = "Vulcan Range Quote Request";
        mailclient.MailBodyText = message;
        mailclient.MailTo = EmailTo;
        
        mailclient.SendMail();
    }
	
	public void SendToPowerObjects()
	{
		string postData = "powf_4a650f6298d8e41180f3fc15b4289e3c=" + first.Text
						+ "&powf_e0fe117298d8e41180f3fc15b4289e3c=" + last.Text
						+ "&powf_c3583f8598d8e41180f3fc15b4289e3c=" + company.Text
						+ "&powf_0e4817be98d8e41180f3fc15b4289e3c=" + zipcode.Text
						+ "&powf_64ccead798d8e41180f3fc15b4289e3c=" + phone.Text
						+ "&powf_a7faf80099d8e41180f3fc15b4289e3c=" + email.Text
						+ "&powf_2bffb11d99d8e41180f3fc15b4289e3c=Ranges" //product
						+ "&powf_c1ca982c99d8e41180f3fc15b4289e3c=" + comments.Text
						+ "&tver=2013"
						+ "&powf_6e4ad089ffe8e41180fffc15b4286c00=Web"  
						+ "&powf_36de96a4ffe8e41180fffc15b4286c00=Vulcan" 
						+ "&powf_63e318f6d9ede41180f0c4346bac3fa4=Ranges SEM - Request A Quote" 
						+ "&powf_cf76466cdaede41180f0c4346bac3fa4=Web Lead-" + first.Text + " " + last.Text
						+ "&ignore_submitmessage=Thank+you+for+your+submission.+We+will+respond+at+our+earliest+convenience."
						+ "&ignore_linkbuttontext=Homepage"
						+ "&ignore_redirecturl=http%3A%2F%2Fvulcanequipment.com%2F"
						+ "&ignore_redirectmode=Link";
		byte[] byteArray = Encoding.UTF8.GetBytes(postData);
		
		WebRequest request = WebRequest.Create("https://cloud.crm.powerobjects.net/powerWebFormV3/PowerWebFormData.aspx?t=KhY9aYv1qUKMVSPXUjHSNG8AcgBnADcAYgA3ADAANwBlADAANQA%3d&formId=powf_1656093698D8E41180F3FC15B4289E3C&tver=2013&c=1");
		request.Method = "POST";
		request.ContentLength = byteArray.Length;
		request.ContentType = "application/x-www-form-urlencoded";

		Stream dataStream = request.GetRequestStream();
		dataStream.Write(byteArray, 0, byteArray.Length);
		dataStream.Close();

		WebResponse response = request.GetResponse();
		Stream data = response.GetResponseStream();
		StreamReader reader = new StreamReader(data);
		string responseFromServer = reader.ReadToEnd();

		Console.WriteLine(responseFromServer);

		reader.Close();
		data.Close();
		response.Close();
	}
}