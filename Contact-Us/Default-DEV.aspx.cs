using System;
using System.Text;

public partial class _Default : System.Web.UI.Page
{
	
    protected void Page_Load(object sender, EventArgs e)
    {
		MM.Visible = false;
		MMP.Visible = false;
		
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
					EmailNotification();
					Response.Redirect("/Contact-Us/Success.aspx");
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
            cd.FolderId = 92; //dev
            //cd.FolderId = 92; //Live
            cd.Html = CreateXml();
            cd.XmlConfiguration = new Ektron.Cms.XmlConfigData { Id = 11 }; //dev
            //cd.XmlConfiguration = new Ektron.Cms.XmlConfigData { Id = 11 }; //live

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
					EmailCC = "doug.mckinnon@hobart.ca";
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
					EmailTo="richard.manson@vulcanhart.com, mark.dietz@vulcanhart.com"; 
				} 
				else if(category.SelectedValue == "Griddles and Charboilers") 
				{
					EmailTo = "Tim.welsh@vulcanhart.com, Rob.martin@itwce.com";
				} 
				else if(category.SelectedValue == "Convection Ovens") 
				{
					EmailTo="Kenny.graven@vulcanhart.com";
				} 
				else if(category.SelectedValue == "Restaurant Ranges") 
				{
					EmailTo="Chris.stern@vulcanhart.com";
				} 
				else if(category.SelectedValue == "Holding and Transport") 
				{
					EmailTo="Jim.sherman@vulcanhart.com, steve.jensen@wittco.com";
				} 
				else if(category.SelectedValue == "Heavy Duty Cooking") 
				{
					EmailTo="chris.stern@vulcanhart.com";
				} 
				else if(category.SelectedValue == "Steam") 
				{
					EmailTo="Jim.sherman@vulcanhart.com";
				} 
				else if(category.SelectedValue == "Slicing" || category.SelectedValue == "Mixing" ||
						category.SelectedValue == "Vacuum Packaging" || category.SelectedValue == "Processing") 
				{
					EmailTo = "Duane.Shomler@itwfeg.com";
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
}