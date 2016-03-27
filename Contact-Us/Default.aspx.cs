using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

public partial class _Default : System.Web.UI.Page
{
	
    protected void Page_Load(object sender, EventArgs e)
    {
		MM.Visible = false;
		MMP.Visible = false;
		
		string dateStr = DateTime.Now.ToString();
		CAPTCHA.ImageUrl = "/CAPTCHAimage/CAPTCHAimage.aspx?id=" + dateStr;
		
		if(IsPostBack)
		{
			if(inquiry_type.SelectedValue == "Products" || inquiry_type.SelectedValue == "Technical Issue / Question")
			{
				MM.Visible = true;
			}
			else if (inquiry_type.SelectedValue == "Parts / Repair Service")
			{
				MMP.Visible = true;
			}
		}
		
		Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "val", "fnOnUpdateValidators();");
    }
    
    protected void Submit_Click(object sender, EventArgs e)
    {
		string dateStr = DateTime.Now.ToString();
        Boolean passValidation = true;
        
        if (Page.IsValid)
        {
			if (fields_CAPTCHA.Text != Session["CAPTCHA"].ToString())
            {
                vCaptcha.IsValid = false;
                CAPTCHA.ImageUrl = "/CAPTCHAimage/CAPTCHAimage.aspx?id=" + dateStr;
                Session.Remove("CAPTCHA");
                fields_CAPTCHA.Style.Add("background", "#f8dbdf");
                fields_CAPTCHA.Style.Add("border", "1px solid #e13433");
            }
            else
            {
				SaveToDB();
				SendToPowerObjects();
				Response.Redirect("/Contact-Us/Success.aspx");
				
				/* if (CreateContent() == true)
				{
					EmailNotification();
					SendToPowerObjects();
					Response.Redirect("/Contact-Us/Success.aspx");
				}
				else
				{
					//lbError.Text = "There was a problem submitting your request.  Please try again.";
				}  */
			}
        } 
		else {
			Session.Remove("CAPTCHA");
		}
    }
    
    protected void Page_Unload(object sender, EventArgs e)
    {
        Session.Remove("CAPTCHA");
    }
    
    public bool CreateContent()
    {
        try
        {
            Ektron.Cms.Framework.Core.Content.Content capi = new Ektron.Cms.Framework.Core.Content.Content(Ektron.Cms.Framework.ApiAccessMode.Admin);
            Ektron.Cms.ContentData cd = new Ektron.Cms.ContentData();

            cd.Title = DateTime.Now.ToString("yyyy-MM-dd - ") + first.Text + "_" + last.Text; // title
            cd.FolderId = 92; //dev + Live
            cd.Html = CreateXml();
            cd.XmlConfiguration = new Ektron.Cms.XmlConfigData { Id = 11 }; //dev + live

            capi.Add(cd);
        
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
        string addressTwo = "";
        string prods = "";
        string isChecked = "no";

        //if (signup.Checked)
        //    isChecked = "yes"; 

        sb.Append("<root>");
        sb.Append(string.Format("<first>{0}</first>", first.Text));
        sb.Append(string.Format("<last>{0}</last>", last.Text));
        sb.Append(string.Format("<email>{0}</email>", email.Text));
        sb.Append(string.Format("<phone>{0}</phone>", phone.Text));
        sb.Append(string.Format("<title>{0}</title>", title.Text));
        sb.Append(string.Format("<company>{0}</company>", company.Text));
        sb.Append(string.Format("<company_type>{0}</company_type>", company_type.SelectedValue));
        sb.Append(string.Format("<address>{0}</address>", address.Text));
        sb.Append(string.Format("<address2>{0}</address2>", address2.Text));
        sb.Append(string.Format("<country>{0}</country>", country.SelectedValue));
        sb.Append(string.Format("<city>{0}</city>", city.Text));
        sb.Append(string.Format("<state>{0}</state>", state.Text));
        sb.Append(string.Format("<zipcode>{0}</zipcode>", zipcode.Text)); 
        sb.Append(string.Format("<inquiry_type>{0}</inquiry_type>", inquiry_type.SelectedValue));
        sb.Append(string.Format("<categories>{0}</categories>", category.SelectedValue));
        sb.Append(string.Format("<brand>{0}</brand>", brand.SelectedValue));
        sb.Append(string.Format("<comments>{0}</comments>", comment.Text));
        //sb.Append(string.Format("<signup>{0}</signup>", isChecked));
        sb.Append("</root>");

        return sb.ToString();
    }
	
	public void SaveToDB()
    {
		SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
		
		try {
			myConnection.Open();
													 
			String query = "INSERT INTO Vulcan_Contact_Us (first,last,email,phone,title,company,company_type,"
						 + "address,address2,country,city,state,zipcode,inquiry_type,categories,brand,comments) "		
						 + "VALUES(@first,@last,@email,@phone,@title,@company,@company_type,"
						 + "@address,@address2,@country,@city,@state,@zipcode,@inquiry_type,@categories,@brand,@comments)";

			SqlCommand command = new SqlCommand(query, myConnection);
			command.Parameters.Add("@first",first.Text);
			command.Parameters.Add("@last",last.Text);
			command.Parameters.Add("@email",email.Text);
			command.Parameters.Add("@phone",phone.Text);
			command.Parameters.Add("@title",title.Text);
			command.Parameters.Add("@company",company.Text);
			command.Parameters.Add("@company_type",company_type.SelectedValue);
			
			command.Parameters.Add("@address",address.Text);
			command.Parameters.Add("@address2",address2.Text);
			command.Parameters.Add("@country",country.SelectedValue);
			command.Parameters.Add("@city",city.Text);
			command.Parameters.Add("@state",state.Text);
			command.Parameters.Add("@zipcode",zipcode.Text);
			
			command.Parameters.Add("@inquiry_type",inquiry_type.SelectedValue);
			command.Parameters.Add("@categories",category.SelectedValue);
			command.Parameters.Add("@brand",brand.SelectedValue);
			command.Parameters.Add("@comments",comment.Text);
			
			command.ExecuteNonQuery();
												
			myConnection.Close();
		}
		catch(Exception ex)
		{
			//comment.Text = ex.ToString();
		}
	}
    
    public void EmailNotification()
    {
		Ektron.Cms.CommonApi mailformat = new Ektron.Cms.CommonApi();
        Ektron.Cms.Common.EkMailService mailclient = mailformat.EkMailRef;
		string message = "";
		string EmailTo = "";
		string EmailCC = "";
		
		message = "<span style='font-family: Arial, Verdana; font-size:12px;'>"
 				+ "The following individual has submitted a form.<br /><br />"
				+ "<strong>Email Address</strong>: " + email.Text
				+ "<br /><strong>First Name</strong>: " + first.Text
				+ "<br /><strong>Last Name</strong>: " + last.Text
				+ "<br /><strong>Title</strong>: " + title.Text
				+ "<br /><strong>Company</strong>: " + company.Text
				+ "<br /><strong>Company Type</strong>: " + company_type.SelectedValue
				+ "<br /><strong>Address</strong>: " + address.Text + " " + address2.Text
				+ "<br />" + city.Text + ", " + state.Text + ", " + zipcode.Text
				+ "<br /><strong>Country</strong>: " + country.SelectedValue
				+ "<br /><strong>Phone</strong>: " + phone.Text
				+ "<br /><strong>Topic</strong>: " + inquiry_type.SelectedValue;
	
		if (inquiry_type.SelectedValue == "Products" || inquiry_type.SelectedValue == "Technical Issue / Question"){ 
			message	+=	"<br /><strong>Categories</strong>: " + category.SelectedValue;
		}
	
		if (inquiry_type.SelectedValue == "Parts / Repair Service") {
			message	+=	"<br /><strong>Product Brand</strong>: " + brand.SelectedValue;
		}
	
		message +=	"<br /><strong>Comments</strong>:<br> " + comment.Text;
		message += "<br /><br /><i>Please do not reply to this email!</i></span>";

		
		//Determine email address(es) for international contacts based on brand and country
		if (country.SelectedValue != "US") {
			switch (country.SelectedValue) {
				case "CA":
					EmailTo = "Karen.Caswell@hobart.ca";
					EmailCC = "Mark.Schilling@hobart.ca";
					break;
				case "MX":
					EmailTo = "Ralph.ottmueller@hobart-mexico.com";
					EmailCC = "Aridai.valdez@hobart-mexico.com";
					break;
				case "BH":
				case "CY":
				case "EG":
				case "IL":
				case "JO":
				case "KW":
				case "OM":
				case "QA":
				case "SA":
				case "SY":
				case "AE":
				case "YE":
					EmailTo = "rmetros@hdsheldon.com";
					EmailCC = "sales@hdsheldon.com";
					break;
				case "AW":
				case "BS":
				case "BB":
				case "BM":
				case "KY":
				case "DO":
				case "GD":
				case "HT":
				case "JM":
				case "PR":
				case "TT":
				case "VG":
				case "VI":
					EmailTo = "rmetros@hdsheldon.com";
					EmailCC = "sales@hdsheldon.com";
					break;
				default:
					EmailTo = "sue.walker@hobartcorp.com";
					EmailCC = "bonnie.small@itwfeg.com";
					break;
			}
		}
		else {
	
			/************ Here we send email depending on what the drop down selection is for topic / subcategory  **********/
            if(inquiry_type.SelectedValue == "Customer Services") 
            {
				EmailTo = "Elaine.edwards@vulcanfeg.com";    
			}
		    else if(inquiry_type.SelectedValue == "Dealers / Sales") 
            {
				EmailTo = "jim.sherman@vulcanfeg.com";             
				EmailCC="Robert.walthall@vulcanfeg.com";  
			}    
			if(inquiry_type.SelectedValue == "Products" || inquiry_type.SelectedValue == "Technical Issue / Question")
			{    
				if(category.SelectedValue == "Fryers") 
				{
					EmailTo="richard.manson@vulcanhart.com, rob.martin@itwce.com"; 
				} 
				else if(category.SelectedValue == "Griddles") 
				{
					EmailTo = "stacey.turek@vulcanhart.com, glenn.ranger@itwce.com";
				} 
				else if(category.SelectedValue == "Charboilers") 
				{
					EmailTo = "stacey.turek@vulcanhart.com, glenn.ranger@itwce.com";
				} 
				else if(category.SelectedValue == "Convection Ovens") 
				{
					EmailTo="Kenny.graven@vulcanhart.com";
				} 
				else if(category.SelectedValue == "Restaurant Ranges") 
				{
					EmailTo="Chris.stern@vulcanhart.com";
				} 
				else if(category.SelectedValue == "Holding Holding") 
				{
					EmailTo="Jim.sherman@vulcanhart.com, steve.jensen@wittco.com";
				} 
				else if(category.SelectedValue == "Heavy Duty Cooking") 
				{
					EmailTo="chris.stern@vulcanhart.com";
				} 
				else if(category.SelectedValue == "Steamers" || category.SelectedValue == "Braising Pans" || category.SelectedValue == "Kettles") 
				{
					EmailTo="Jim.sherman@vulcanhart.com";
				} 
				else if(category.SelectedValue == "Combi Oven") 
				{
					EmailTo="david.sager@itwfeg.com";
				} 
				else if(category.SelectedValue == "") {
					EmailTo="Jim.sherman@vulcanhart.com";
				}
			}
			else if(inquiry_type.SelectedValue == "Media Request") 
            {
				EmailTo = "susang@marriner.com";
			} 
			else if(inquiry_type.SelectedValue == "Employment Opportunities") 
            {
				EmailTo = "Flemming.Scott@vulcanhart.com"; 
			} 
			else if(inquiry_type.SelectedValue == "K-12") 
            {
				EmailTo = "William.Burke@itwfeg.com"; 
			} 
			else if(inquiry_type.SelectedValue == "Other") 
            {
				EmailTo = "Robert.walthall@vulcanfeg.com";
			} 
				
			if(inquiry_type.SelectedValue == "Parts / Repair Service")
			{
				if(brand.SelectedValue == "Vulcan Wolf")
				{
					EmailTo = "ronald.arrington@vulcanhart.com";
				}
				else //Berkel
				{
					EmailTo = "duane.shomler@itwfeg.com";
				}
			}
		}
		
		mailclient.MailFrom = "contactus@vulcanequipment.com";
        mailclient.MailSubject = "Vulcan Contact Us Submission - " + inquiry_type.SelectedValue;
        mailclient.MailBodyText = message;
        mailclient.MailTo = EmailTo;
        mailclient.MailCC = EmailCC;
        
        mailclient.SendMail();
    }
	
	public void SendToPowerObjects()
	{
		string postData = "powf_0303dc9294d8e41180f3fc15b4289e3c=" + first.Text
						+ "&powf_d25718b394d8e41180f3fc15b4289e3c=" + last.Text
						+ "&powf_12862edd94d8e41180f3fc15b4289e3c=" + title.Text
						+ "&powf_a006fff794d8e41180f3fc15b4289e3c=" + company.Text
						+ "&powf_ce5e4a1d95d8e41180f3fc15b4289e3c=" + company_type.SelectedValue
						+ "&powf_952e804195d8e41180f3fc15b4289e3c=" + address.Text + " " + address2.Text
						+ "&powf_7b2e275895d8e41180f3fc15b4289e3c=" + city.Text
						+ "&powf_c6a0196b95d8e41180f3fc15b4289e3c=" + state.Text
						+ "&powf_05cc8ab348f3e41180fafc15b4289e3c=" + country.SelectedValue
						+ "&powf_d41a1f8395d8e41180f3fc15b4289e3c=" + zipcode.Text
						+ "&powf_fd7821a495d8e41180f3fc15b4289e3c=" + phone.Text
						+ "&powf_3612e1be95d8e41180f3fc15b4289e3c=" + email.Text
						+ "&powf_c221c9f995d8e41180f3fc15b4289e3c=" + inquiry_type.SelectedValue
						+ "&powf_5abe8f0c96d8e41180f3fc15b4289e3c=" + comment.Text
						+ "&tver=2013"
						+ "&powf_8deb8626ffe8e41180fffc15b4286c00=Web"  
						+ "&powf_cc355654ffe8e41180fffc15b4286c00=Vulcan" 
						+ "&powf_0b7b29e0d9ede41180f0c4346bac3fa4=Contact Us" 
						+ "&powf_fb15c452daede41180f0c4346bac3fa4=Web Lead-" + first.Text + " " + last.Text
						+ "&ignore_submitmessage=Thank+you+for+your+submission.+We+will+respond+at+our+earliest+convenience."
						+ "&ignore_linkbuttontext=Homepage"
						+ "&ignore_redirecturl=http%3A%2F%2Fvulcanequipment.com%2F"
						+ "&ignore_redirectmode=Link";
						
		if (inquiry_type.SelectedValue == "Products" || inquiry_type.SelectedValue == "Technical Issue / Question") {
			postData += "&powf_b152e2da95d8e41180f3fc15b4289e3c=" + category.SelectedValue;
		} else if (inquiry_type.SelectedValue == "Parts / Repair Service") {
			postData += "&powf_b152e2da95d8e41180f3fc15b4289e3c=" + brand.SelectedValue;
		}
		byte[] byteArray = Encoding.UTF8.GetBytes(postData);
		
		WebRequest request = WebRequest.Create("https://cloud.crm.powerobjects.net/powerWebFormV3/PowerWebFormData.aspx?t=KhY9aYv1qUKMVSPXUjHSNG8AcgBnADcAYgA3ADAANwBlADAANQA%3d&formId=powf_CE826E0494D8E41180F3FC15B4289E3C&tver=2013&c=1");
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