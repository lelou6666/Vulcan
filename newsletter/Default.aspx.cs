using System;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

public partial class _Default : System.Web.UI.Page
{
	
	bool bSuccess = true;
	
    protected void Page_Load(object sender, EventArgs e)
    {
		if(Request.QueryString["email"] != null)
		{
			email.Text = Request.QueryString["email"].ToString();
		}
		
		Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "val", "fnOnUpdateValidators();");
    }
    
    protected void Submit_Click(object sender, EventArgs e)
    {
        Boolean passValidation = true;
        
        if (Page.IsValid)
        {
			if (fields_CAPTCHA.Text != Session["CAPTCHA"].ToString())
            {
                vCaptcha.IsValid = false;
                Session.Remove("CAPTCHA");
                fields_CAPTCHA.Style.Add("background", "#f8dbdf");
                fields_CAPTCHA.Style.Add("border", "1px solid #e13433");
            }
            else
            {
				if (CreateContent() == true)
				{
					try
					{
						SaveToDB();
						addContact();
					}
					catch (Exception ex)
					{
					}
					
					Response.Redirect("/newsletter/Success.aspx");
				}
				else
				{
					//lbError.Text = "There was a problem submitting your request.  Please try again.";
				} 
			}
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
            cd.FolderId = 94; //dev + Live
            cd.Html = CreateXml();
            cd.XmlConfiguration = new Ektron.Cms.XmlConfigData { Id = 12 }; //dev + live

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
        sb.Append(string.Format("<title>{0}</title>", title.Text));
        sb.Append(string.Format("<company>{0}</company>", company.Text));
        sb.Append("</root>");

        return sb.ToString();
    }
	
	public void SaveToDB()
    {
		SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
		
		try {
			myConnection.Open();
													 
			String query = "INSERT INTO Vulcan_Newsletter (first,last,email,title,company,"
						 + "restaurants,k12_schools,correctional_institutions,healthcare,fryers,griddles_charbroilers,heated_holding,ovens,ranges,steam) "		
						 + "VALUES(@first,@last,@email,@title,@company,@restaurants,"
						 + "@k12_schools,@correctional_institutions,@healthcare,@fryers,@griddles_charbroilers,@heated_holding,@ovens,@ranges,@steam)";

			SqlCommand command = new SqlCommand(query, myConnection);
			command.Parameters.Add("@first",first.Text);
			command.Parameters.Add("@last",last.Text);
			command.Parameters.Add("@email",email.Text);
			command.Parameters.Add("@title",title.Text);
			command.Parameters.Add("@company",company.Text);
			command.Parameters.Add("@restaurants",Restaurants.Checked);
			command.Parameters.Add("@k12_schools",k12.Checked);
			command.Parameters.Add("@correctional_institutions",Correctional.Checked);
			command.Parameters.Add("@healthcare",Healthcare.Checked);
			command.Parameters.Add("@fryers",Fryers.Checked);
			command.Parameters.Add("@griddles_charbroilers",Griddles.Checked);
			command.Parameters.Add("@heated_holding",Heated.Checked);
			command.Parameters.Add("@ovens",Ovens.Checked);
			command.Parameters.Add("@ranges",Ranges.Checked);
			command.Parameters.Add("@steam",Steam.Checked);
			
			command.ExecuteNonQuery();
												
			myConnection.Close();
		}
		catch(Exception ex)
		{
			//company.Text = ex.ToString();
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
            string postData = "<contacts><contact><firstName>" + first.Text.ToString() + "</firstName><lastName>" + last.Text.ToString() + "</lastName><email>" + email.Text.ToString()
                            + "</email><business>" + company.Text + "</business><title>" + title.Text + "</title>";
                            
            if(Restaurants.Checked) {
				postData += "<restaurants>1</restaurants>";
            }
            if(k12.Checked) {
				postData += "<k12schools>1</k12schools>";
            } 
            if(Correctional.Checked) {
				postData += "<correctionalinstitutions>1</correctionalinstitutions>";
            }
            if(Healthcare.Checked) {
				postData += "<healthcare>1</healthcare>";
            }
            if(Fryers.Checked) {
				postData += "<fryers>1</fryers>";
            }
            if(Griddles.Checked) {
				postData += "<griddlescharbroilers>1</griddlescharbroilers>";
            }  
            if(Heated.Checked) {
				postData += "<heatedholding>1</heatedholding>";
            }
            if(Ovens.Checked) {
				postData += "<ovens>1</ovens>";
            } 
            if(Ranges.Checked) {
				postData += "<ranges>1</ranges>";
            }
            if(Steam.Checked) {
				postData += "<steam>1</steam>";
            } 
            
            postData += "</contact></contacts>";

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
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create("https://app.icontact.com/icp/a/48076/c/35058/contacts?&email=" + email.Text.ToString());
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