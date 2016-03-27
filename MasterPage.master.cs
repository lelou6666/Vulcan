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

public partial class MasterPage : System.Web.UI.MasterPage
{
	bool bSuccess;
	string sEmail = "";
	
    protected void Page_Load(object sender, EventArgs e)
    {
		string request = Request.Url.AbsoluteUri;
		string url = HttpContext.Current.Request.Url.AbsoluteUri;
	

		//New Site product rename rediects
		if(request.IndexOf("/Products/24-in-Range-Standard-Oven-and-4-Burners/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/Products/24-in-Gas-Range-Standard-Oven-and-4-Burners/");
		}
		else if(request.IndexOf("/Products/72-in-Range-Standard-or-Convection-Ovens-8-Burners-and-24-in-Griddle-Right/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/products/72-in-gas-range-standard-or-convection-ovens-8-burners-and-24-in-griddle-right/");
		}
		else if(request.IndexOf("/Products/Gas-Boilerless-Combi-Oven-Steamer/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/Products/Gas-Boilerless-Commercial-Restaurant-Combi-Oven-Steamer/");
		}
		else if(request.IndexOf("/Products/V-Series-Braising-Pans/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/products/v-series-commercial-braising-pans/");
		}
		else if(request.IndexOf("/products/electric-cheesemelters/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/Products/Commercial-Electric-Cheesemelters/");
		}
		else if(request.IndexOf("/Products/Electric-Salamander-Broilers/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/products/infrared-restaurant-salamander-broilers/");
		}
		else if(request.IndexOf("/Products/MSA-Series-Gas-Griddles/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/Products/MSA-Series-Gas-Flat-Top-Griddles-Grills/");
		}
		else if(request.IndexOf("/Products/VCCB-Series-Charbroilers/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/products/vccb-series-low-profile-charbroilers/");
		}
		else if(request.IndexOf("/Products/VACB-Series-Achiever-Charbroilers/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/Products/VACB-Series-Achiever-Restaurant-Charbroilers/");
		}
		else if(request.IndexOf("/Products/GTS12-Heavy-Duty-Electric-Griddle-Top/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/products/clamshell-griddle-accessory/");
		}
		else if(request.IndexOf("/Products/V-Series-Braising-Pans/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/products/v-series-commercial-braising-pans/");
		}
		else if(request.IndexOf("/Products/VHP-Series/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/products/commercial-industrial-hot-plates/");
		}
		else if(request.IndexOf("/Products/36-in-Range-Standard-or-Convection-Oven-and-6-Burners/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/Products/36-in-Gas-Range-Standard-or-Convection-Oven-and-6-Burners/");
		}
		else if(request.IndexOf("/Products/SG-Series-Convection-Ovens/") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/Products/SG-Series-Gas-Convection-Ovens/");
		}
	
		//Redirects from Old Site
		if(request.IndexOf("/Vulcan/Media_Center/Press_Releases/viewRelease.aspx?id=1379") != -1)
		{
			Response.Status = "301 Moved Permanently";
			Response.AddHeader("Location","http://vulcanequipment.com/News-and-Events/Press-Releases/EPA-Recognizes-ITW-Food-Equipment-Group-with-2013-ENERGY-STAR-Partner-of-the-Year-Sustained-Excellence-Award/");
		}
		else if(request.IndexOf("/Vulcan/Products/products.aspx?brand=Vulcan&cid=3&gid=43&scid=6") > -1) {
			Response.Redirect("/Products/Electric-Counter-Convection-Steamers/");
		}
		else if(request.IndexOf("/Vulcan/Products/news.aspx?id=1261&title=PowerFry%20VK%20Series") > -1) {
			Response.Redirect("/video-library/?video=48031968&title=Vulcan%20PowerFry™%20VK%20Series%20Fryer&player=4");
		}
		else if(request.IndexOf("/Vulcan/Products/news.aspx?id=1304&title=Vulcan%20Endurance%20Ranges") > -1) {
			Response.Redirect("/video-library/?video=79725652&title=Vulcan%20Endurance™%20Ranges&player=2");
		}
		else if(request.IndexOf("/Vulcan/Products/news.aspx?id=1297&title=VC%20Series%20Gas%20Convection%20Ovens>&title=VC%20Series%20Gas%20Convection%20Ovens") > -1) {
			Response.Redirect("/video-library/?video=47962761&title=Vulcan%20VC%20Series%20Gas%20Convection%20Ovens&player=6");
		}
		else if(request.IndexOf("/Vulcan/Products/news.aspx?id=1296&title=VRH%20Restaurant%20Series%20Cook%20&%20Hold%20Ovens") > -1) {
			Response.Redirect("/products/ovens/");
		}
		else if(request.IndexOf("/Vulcan/Products/news.aspx?id=1295&title=VTEC%20Charbroiler%20Featuring%20IRX%20100%25%20Infrared%20Technology") > -1) {
			Response.Redirect("/video-library/?video=47963703&title=Vulcan%20VTEC%20Series%20Charbroiler&player=3");
		}
		else if(request.IndexOf("/Vulcan/Products/Steam/?brand=Vulcan&cid=3") > -1) {
			Response.Redirect("/products/steamers/");
		}
		else if(request.IndexOf("/vulcan/products/news.aspx?id=1323") > -1) {
			Response.Redirect("/video-library/?video=48033965&title=Vulcan%20K%20Series%20Kettles%20Make%20Perfect%20Soup%20Stock&player=5");
		}
		else if(request.IndexOf("/Vulcan/Products/news.aspx?id=1222&title=Rapid") > -1) {
			Response.Redirect("/video-library/?video=47962760&title=Vulcan%20Rapid%20Recovery%20Griddle™%20Pancake%20Test&player=3");
		}
		else if(request.IndexOf("/Vulcan/Products/news.aspx?id=1261&title=VK%20PowerFry") > -1) {
			Response.Redirect("/video-library/?video=48031968&title=Vulcan%20PowerFry™%20VK%20Series%20Fryer&player=4");
		}
		else if(request.IndexOf("/Vulcan/Products/products.aspx?brand=Vulcan&cid=7&gid=715&scid=17") > -1) {
			Response.Redirect("/products/vpt-pass-through-series-cabinets/");
		}
		else if(request.IndexOf("/Vulcan/Products/news.aspx?id=1296&title=VRH%20Restaurant%20Series%20Cook%20&%20Hold%20Ovens") > -1) {
			Response.Redirect("/video-library/?video=48031970&title=Vulcan%20VRH%20Restaurant%20Series%20Cook%20%26%20Hold%20Ovens&player=7");
		}
		else if(request.IndexOf("/Vulcan/Products/RestaurantRanges/") > -1) {
			Response.Redirect("/products/ranges/");
		}
		else if(request.IndexOf("/Vulcan/Products/news.aspx?id=1241") > -1) {
			Response.Redirect("/video-library/?video=47668356&title=V%20Series%20Heavy%20Duty%20Ranges&player=2");
		}
		else if(request.IndexOf("/Vulcan/Products/news.aspx?id=1323") > -1) {
			Response.Redirect("/video-library/?video=48033965&title=Vulcan%20K%20Series%20Kettles%20Make%20Perfect%20Soup%20Stock&player=5");
		}
		
		Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "val", "fnOnUpdateValidators();");
    }
    
    protected void searchText(object sender, EventArgs e)
    {
		if(txtSearch.Text != "keyword search")
		{
			if(txtSearch.Text == "kettle")
			{
				txtSearch.Text = "kettles";
			}
			Response.Redirect("/search/?searchtext=" + txtSearch.Text);
		}
    }
	
	protected void Submit_Quote(object sender, EventArgs e)
    {
		string dateStr = DateTime.Now.ToString();
        Boolean passValidation = true;
        
        if (Page.IsValid)
        {
			ScriptManager.RegisterStartupScript(UpdatePanelQuote, this.GetType(),"MyAction10", "toggleThanksQuote();", true);
			
			//SaveToDB();
			//EmailNotification();
			//SendToPowerObjects();
		}
    }
	
	public bool CreateContent()
    {
        try
        {
            Ektron.Cms.Framework.Core.Content.Content capi = new Ektron.Cms.Framework.Core.Content.Content(Ektron.Cms.Framework.ApiAccessMode.Admin);
            Ektron.Cms.ContentData cd = new Ektron.Cms.ContentData();

            cd.Title = DateTime.Now.ToString("yyyy-MM-dd - ") + first.Text + "_" + last.Text; // title
            cd.FolderId = 149; //dev + Live
            cd.Html = CreateXml();
            cd.XmlConfiguration = new Ektron.Cms.XmlConfigData { Id = 11 }; //dev + live

            capi.Add(cd);
        
            return true;
        }
        catch (Exception ex)
        {
            return false;
        } 
		
		return true;
    }
	
	 public string CreateXml()
    {
        StringBuilder sb = new StringBuilder();

       sb.Append("<root>");
        sb.Append(string.Format("<first>{0}</first>", first.Text));
        sb.Append(string.Format("<last>{0}</last>", last.Text));
        sb.Append(string.Format("<email>{0}</email>", email.Text));
        sb.Append(string.Format("<phone>{0}</phone>", phone.Text));
        sb.Append(string.Format("<company>{0}</company>", company.Text));
        sb.Append(string.Format("<zipcode>{0}</zipcode>", zipcode.Text)); 
        sb.Append(string.Format("<categories>{0}</categories>", category.SelectedValue));
        sb.Append(string.Format("<product>{0}</product>", product.Text));
        sb.Append(string.Format("<comments>{0}</comments>", comment.Text));
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
			command.Parameters.Add("@product",category.SelectedValue + ": " + product.Text);
			command.Parameters.Add("@first",first.Text);
			command.Parameters.Add("@last",last.Text);
			command.Parameters.Add("@company",company.Text);
			command.Parameters.Add("@phone",phone.Text);
			command.Parameters.Add("@zipcode",zipcode.Text);
			command.Parameters.Add("@email",email.Text);
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
				+ "<br /><strong>Company</strong>: " + company.Text
				+ "<br /><strong>Zip Code:</strong>: " + zipcode.Text
				+ "<br /><strong>Phone</strong>: " + phone.Text
				+ "<br /><strong>Product</strong>: " + product.Text
				+ "<br /><strong>Product Category</strong>: " + category.SelectedValue;
	
		message +=	"<br /><strong>Comments</strong>:<br> " + comment.Text;
		message += "<br /><br /><i>Please do not reply to this email!</i></span>";

		if(category.SelectedValue == "Fryers") 
		{
			EmailTo="richard.manson@vulcanhart.com, rob.martin@itwce.com"; 
		} 
		else if(category.SelectedValue == "Griddles" || category.SelectedValue == "Charbroilers") 
		{
			EmailTo = "stacey.turek@vulcanhart.com, glenn.ranger@itwce.com";
		} 
		else if(category.SelectedValue == "Convection Ovens") 
		{
			EmailTo="Kenny.graven@vulcanhart.com";
		} 
		else if(category.SelectedValue == "Ranges") 
		{
			EmailTo="Chris.stern@vulcanhart.com";
		} 
		else if(category.SelectedValue == "Holding Holding") 
		{
			EmailTo="Jim.sherman@vulcanhart.com, steve.jensen@wittco.com";
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
		
		mailclient.MailFrom = "request-quote@vulcanequipment.com";
        mailclient.MailSubject = "Vulcan Quote Request";
        mailclient.MailBodyText = message;
        mailclient.MailTo = EmailTo;
        mailclient.MailCC = EmailCC;
        
		//mailclient.MailTo = "reneeh@marriner.com";
		
		if(email.Text != "reneeh@marriner.com") {
        	mailclient.SendMail();
		}
    }
	
	public void SendToPowerObjects()
	{
		string postData = "powf_4a650f6298d8e41180f3fc15b4289e3c=" + first.Text
						+ "&powf_e0fe117298d8e41180f3fc15b4289e3c=" + last.Text
						+ "&powf_c3583f8598d8e41180f3fc15b4289e3c=" + company.Text
						+ "&powf_0e4817be98d8e41180f3fc15b4289e3c=" + zipcode.Text
						+ "&powf_64ccead798d8e41180f3fc15b4289e3c=" + phone.Text
						+ "&powf_a7faf80099d8e41180f3fc15b4289e3c=" + email.Text
						+ "&powf_2bffb11d99d8e41180f3fc15b4289e3c=" + category.SelectedValue + ": " + product.Text
						+ "&powf_c1ca982c99d8e41180f3fc15b4289e3c="  + comment.Text
						+ "&powf_6e4ad089ffe8e41180fffc15b4286c00=Web"  
						+ "&powf_36de96a4ffe8e41180fffc15b4286c00=Vulcan" 
						+ "&powf_63e318f6d9ede41180f0c4346bac3fa4=Request a Quote" 
						+ "&powf_cf76466cdaede41180f0c4346bac3fa4=Web Lead-" + first.Text + " " + last.Text
						+ "&tver=2013"
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
    
    protected void searchText2(object sender, EventArgs e)
    {
		if(txtSearch2.Text != "keyword search")
		{
			Response.Redirect("/search/?searchtext=" + txtSearch2.Text);
		}
    }
    
    protected void signup(object sender, EventArgs e)
    {
		if(signup_email.Text != "" && signup_email.Text != "email address")
		{
			sEmail = signup_email.Text;
		}
		else if(signup_email_main.Text != "" && signup_email_main.Text != "email address")
		{
			sEmail = signup_email_main.Text;
		}
		
		if(sEmail.Trim() != "")
		{
			string MatchEmailPattern = @"\w+([-+.']\w+)@\w+([-.]\w+).\w+([-.]\w+)*";
			
			if (Regex.IsMatch(sEmail, MatchEmailPattern))
			{
				addContact();
			}
			Response.Redirect("/newsletter/?source=NewsBar&email=" + sEmail);
		}
		else
		{
			signup_email.Style.Add("background", "#f8dbdf");
            signup_email.Style.Add("border", "1px solid #e13433");
            signup_email_main.Style.Add("background", "#f8dbdf");
            signup_email_main.Style.Add("border", "1px solid #e13433");
		}
    }
    
    /*
     * The following process will subscribe the new contact to the newsletter in iContact.
     * */
    private void addContact()
    {
        try
        {
            // Create a request using a URL that can receive a post.  48076 is the account id, and 35058 is the vulcan client id
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://app.icontact.com/icp/a/48076/c/35058/contacts/");

            // Set the Method property of the request to POST.
            request.Method = "POST";
            request.ContentType = "text/xml";
            request.Accept = "text/xml";

            //Set header information to gain access to correct contact list
            request.Headers.Add("API-Version", "2.0");
            request.Headers.Add("API-AppId", "sjLUUCROIyPqbUI7lGNKNv0tqAKyjdmB");
            request.Headers.Add("API-Username", "marriner");
            request.Headers.Add("API-Password", "mmcdevpass");

            // Create POST data string in xml format. (iContacts.com accepts xml)
            string postData = "<contacts><contact><email>" + sEmail + "</email></contact></contacts>";

            //Write xml data to site
            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            writer.WriteLine(postData);
            writer.Close();

            // Get the response.
            WebResponse response = request.GetResponse();

            if (((HttpWebResponse)response).StatusDescription.ToString() == "OK")
            {
                addToSubscrption();
            }
            else
            {
                bSuccess = false;
            }
        }
        catch (Exception ex)
        {
            bSuccess = false;
        }
    }

    private void addToSubscrption()
    {
        string sContactId;

        try
        {
            // Create a request using a URL that can receive a post. 
            HttpWebRequest subRequest = (HttpWebRequest)WebRequest.Create("https://app.icontact.com/icp/a/48076/c/35058/subscriptions/");

            // Set the Method property of the request to POST.
            subRequest.Method = "POST";
            subRequest.ContentType = "text/xml";
            subRequest.Accept = "text/xml";

            //Set header information to gain access to correct contact list
            subRequest.Headers.Add("API-Version", "2.0");
            subRequest.Headers.Add("API-AppId", "sjLUUCROIyPqbUI7lGNKNv0tqAKyjdmB");
            subRequest.Headers.Add("API-Username", "marriner");
            subRequest.Headers.Add("API-Password", "mmcdevpass");

            // Create POST data string in xml format.
            sContactId = getContactId();                         //Note: this list ID is specific to the Vulcan Newsletter 9919023 (got this id via url when viewing list settings)
            string postData = "<subscriptions><subscription><contactId>" + sContactId + "</contactId><listId>9919023</listId><status>normal</status></subscription></subscriptions>";

            //Write xml data to site
            StreamWriter writer = new StreamWriter(subRequest.GetRequestStream());
            writer.WriteLine(postData);
            writer.Close();

            // Get the response.
            WebResponse subResponse = subRequest.GetResponse();

            if (((HttpWebResponse)subResponse).StatusDescription.ToString() == "OK")
            {
                bSuccess = true;
            }
            else
            {
                bSuccess = false;
            }
        }
        catch (Exception ex)
        {
            bSuccess = false;
        }
    }

    /*
     * This method will obtain the contactId of recently added contact.
     * */
    private string getContactId()
    {
        string contactId = "";

        try
        {
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create("https://app.icontact.com/icp/a/48076/c/35058/contacts?&email=" + sEmail);
            getRequest.Method = "GET";
            getRequest.ContentType = "text/xml";
            getRequest.Accept = "text/xml";

            //Set header information to gain access to correct contact list
            getRequest.Headers.Add("API-Version", "2.0");
            getRequest.Headers.Add("API-AppId", "sjLUUCROIyPqbUI7lGNKNv0tqAKyjdmB");
            getRequest.Headers.Add("API-Username", "marriner");
            getRequest.Headers.Add("API-Password", "mmcdevpass");

            // Get the response.
            WebResponse GetResponse = getRequest.GetResponse();
            XmlDocument xdoc = new XmlDocument();

            xdoc.Load(((HttpWebResponse)GetResponse).GetResponseStream());

            XmlNodeList xNodeList = xdoc.SelectNodes("/response/contacts/contact");
            foreach (XmlNode xNode in xNodeList)
            {
                contactId = xNode.SelectSingleNode("contactId").InnerText;
            }
        }
        catch (Exception ex)
        {
            bSuccess = false;
        }

        return contactId;
    }
}
