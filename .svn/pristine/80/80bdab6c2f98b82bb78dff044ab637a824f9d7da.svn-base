using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


public partial class Default : System.Web.UI.Page
{
	public List<Company> Dealers = new List<Company>();
	public string itemcount = "0";
	
	protected void Page_Load(object sender, EventArgs e)
	{
		string distance, zip;
		string search = Request.QueryString["search"];
		bool val;
		bool Unique = true;
		
		if(search != null){
		 	val = System.Text.RegularExpressions.Regex.IsMatch(search, @"\d{5}$");
			txtSearch.Text = search;
		}
		else{
			val = false;
		}
		
		if(val) {
			//FSSQLTEST1.ITWFEG.BIZ
			//fstroysql1
			SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
			//SqlConnection myConnection = new SqlConnection("Data Source=FSSQLTEST1.ITWFEG.BIZ;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");						   
			try
			{
				myConnection.Open();
				
				SqlDataReader myReader = null;

				SqlCommand  myCommand = new SqlCommand("select d.*, isnull(tz.distance,0) as dist, zips.latitude, zips.longitude " +
														  "from Dealers d with (nolock) " +
														  "join f_getzipsinrange('" + search + "', 50) tz on tz.zip = d.zip " +  
														  "join aracms.zip_codes zips with (nolock) on zips.zip = d.zip " +
														  "where lower(d.vulcan) = 'x' " +
														  "Order by dist",
														 myConnection);  
														 
	
				myReader = myCommand.ExecuteReader();
				while(myReader.Read())
				{
					Unique = true;
					foreach (Company d in Dealers)
					{ 
						if(d.Name == myReader["name"].ToString() && d.Address == myReader["address"].ToString()) {
							
							Unique = false;
							
							if(d.Distance != "~1"){
								distance = myReader["dist"].ToString();
								
								//A closer record already exists with this name and address (so update the distance value)
								if(int.Parse(d.Distance) > int.Parse(distance))
								{
									if(distance == "0")
									{
										distance = "~1";
									}
									d.Distance = distance;
									d.Latitude = myReader["latitude"].ToString();
									d.Longitude =	myReader["longitude"].ToString();
								}
							}
						}
					}
					
					if(Unique)
					{
						distance = myReader["dist"].ToString();
						zip = myReader["zip"].ToString();
								
						if(distance == "0")
						{
							distance = "~1";
							zip = search;
						}
						
						Company newCompany = new Company(myReader["name"].ToString(), myReader["address"].ToString(), myReader["city"].ToString(), myReader["state"].ToString(),
						//myReader["location_zip"].ToString()
						zip, myReader["country"].ToString(),
						myReader["contact_name"].ToString(), myReader["phone"].ToString(), myReader["fax"].ToString(), myReader["email_address"].ToString(),
						myReader["website"].ToString(), distance, myReader["company"].ToString());
						
						newCompany.Latitude = myReader["latitude"].ToString();
						newCompany.Longitude =	myReader["longitude"].ToString();	
						
						Dealers.Add(newCompany);	
					}
				}
				
				myConnection.Close();
			
				dp.QueryString = true;
				dp.PageSize = 10;
				dp.RecordSize = Dealers.Count;
				itemcount = Dealers.Count.ToString();
				
			}
			catch
			{
				
			}
		}
		else
		{
			if(search != null){
				txtSearch.Style.Add("background", "#f8dbdf");
				txtSearch.Style.Add("border", "1px solid #e13433");
			}
		}
	}     
}
public class Company {
	private string _Name; 
	private string _Address;
	private string _City; 
	private string _State; 
	private string _Zip; 
	private string _Country;
	private string _Contact;
	private string _Phone;
	private string _Fax;  
	private string _Email; 
	private string _Website; 
	private string _Distance;
	private string _Company; 
	
	private string _Latitude; 
	private string _Longitude; 
	
	public string Name
    {
        set { this._Name = value; }
        get { return this._Name; }
    } 
	
	public string Address
    {
        set { this._Address = value; }
        get { return this._Address; }
    }
	
	public string City
    {
        set { this._City = value; }
        get { return this._City; }
    }
	
	public string State
    {
        set { this._State = value; }
        get { return this._State; }
    }
	
	public string Zip
    {
        set { this._Zip = value; }
        get { return this._Zip; }
    }
	
	public string Country
    {
        set { this._Country = value; }
        get { return this._Country; }
    }
	
	public string Contact
    {
        set { this._Contact = value; }
        get { return this._Contact; }
    }
	
	public string Phone
    {
        set { this._Phone = value; }
        get { return this._Phone; }
    }
	
	public string Fax
    {
        set { this._Fax = value; }
        get { return this._Fax; }
    }
	
	public string Email
    {
        set { this._Email = value; }
        get { return this._Email; }
    }
	
	public string Website
    {
        set { this._Website = value; }
        get { return this._Website; }
    }
	
	public string Distance
    {
        set { this._Distance = value; }
        get { return this._Distance; }
    }
	
	public string CompanyName
    {
        set { this._Company = value; }
        get { return this._Company; }
    }
	
	public string Latitude
    {
        set { this._Latitude = value; }
        get { return this._Latitude; }
    }
	
	public string Longitude
    {
        set { this._Longitude = value; }
        get { return this._Longitude; }
    }
	
	public Company(){
	}
	
	public Company(string sName, string sAddress, string sCity, string sState, string sZip, string sCountry, string sContact, string sPhone, 
					 string sFax, string sEmail, string sWebsite, string sDistance, string sCompany){
		_Name = sName;
		_Address = sAddress;
		_City = sCity;
		_State = sState;
		_Zip = sZip;
		_Country = sCountry;
		_Contact = sContact;
		_Phone = sPhone;
		_Fax = sFax;
		_Email = sEmail;
		_Website = sWebsite;
		_Distance = sDistance;
		_Company = sCompany;
	}	 
}