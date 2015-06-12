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
	public List<Agent> Agents = new List<Agent>();
	public List<Agent> IndAgents = new List<Agent>();
	public List<Agent> HobartAgents = new List<Agent>();
	public string itemcount = "0";
	public string search = "";
	public string latitude = "";
	public string longitude = "";
	
	protected void Page_Load(object sender, EventArgs e)
	{
		bool val;
		
		search = Request.QueryString["search"];
		
		if(search != null){
		 	val = System.Text.RegularExpressions.Regex.IsMatch(search, @"\d{5}$");
			txtSearch.Text = search;
		}
		else{
			val = false;
		}
		
		if(val) {
			GetCenter();
			GetIndendants();
			GetHobarts();
			
			IndAgents.Sort();
			HobartAgents.Sort();
			
			foreach (Agent d in HobartAgents)
			{
				Agent newAgent = new Agent(d.Name, d.Address, d.City, d.State, d.Zip, d.Phone, d.Website);
				
				//they are hobart if from vBranchAgentsZips
				newAgent.Latitude = d.Latitude;
				newAgent.Longitude = d.Longitude;
				newAgent.Distance = d.Distance;
				newAgent.Hobart = "Hobart";
				
				Agents.Add(newAgent);
			}
			
			foreach (Agent d in IndAgents)
			{
				Agent newAgent = new Agent(d.Name, d.Address, d.City, d.State, d.Zip, d.Phone, d.Website);
				
				//they are hobart if from vBranchAgentsZips
				newAgent.Latitude = d.Latitude;
				newAgent.Longitude = d.Longitude;
				newAgent.Distance = d.Distance;
				newAgent.Hobart = "Independant";
				
				Agents.Add(newAgent);
			}
			
			dp.QueryString = true;
			dp.PageSize = 10;
			dp.RecordSize = Agents.Count;
			itemcount = Agents.Count.ToString();
		}
		else
		{
			if(search != null){
				txtSearch.Style.Add("background", "#f8dbdf");
				txtSearch.Style.Add("border", "1px solid #e13433");
			}
		}
	}     
	
	public void GetCenter()
	{
		SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
			//SqlConnection myConnection = new SqlConnection("Data Source=FSSQLTEST1.ITWFEG.BIZ;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");						   
		try
		{
			myConnection.Open();
			
			SqlDataReader myReader = null;
			
			SqlCommand  myCommand = new SqlCommand(
				"SELECT zips.latitude, zips.longitude " +
				"FROM aracms.zip_codes zips with (nolock) 	WHERE zips.zip = '" + search + "'",
			myConnection);   
													 

			myReader = myCommand.ExecuteReader();
			while(myReader.Read())
			{
				latitude = myReader["latitude"].ToString();
				longitude = myReader["longitude"].ToString();
			}
			
			myConnection.Close();
		}
		catch
		{
		}
	}
	
	public void GetIndendants()
	{
		bool Unique = true;
		
		SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
			//SqlConnection myConnection = new SqlConnection("Data Source=FSSQLTEST1.ITWFEG.BIZ;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");						   
		try
		{
			myConnection.Open();
			
			SqlDataReader myReader = null;
			
			SqlCommand  myCommand = new SqlCommand(
				"SELECT d.*, isnull(tz.distance,0) as dist, zips.latitude, zips.longitude " +
				"FROM vVulcanServiceOfficeZips d with (nolock) " +
				"join f_getzipsinrange('" + search + "', 10) tz on tz.zip = d.coverage_zipcode " +
				"join aracms.zip_codes zips with (nolock) on zips.zip = d.office_zip ",
			myConnection);   
													 

			myReader = myCommand.ExecuteReader();
			while(myReader.Read())
			{
				Unique = true;
				foreach (Agent d in IndAgents)
				{ 
					if(d.Name == myReader["office_name"].ToString() && d.Address == myReader["office_addr"].ToString()) {
						Unique = false;
						
						 if(d.Distance > int.Parse(myReader["dist"].ToString())){
							d.Distance = int.Parse(myReader["dist"].ToString());
						} 
					}
				}
				
				if(Unique)
				{
					Agent newAgent = new Agent(myReader["office_name"].ToString(), myReader["office_addr"].ToString(), myReader["office_city"].ToString(), 
					myReader["office_state"].ToString(), myReader["office_zip"].ToString(), myReader["office_phone"].ToString(), myReader["office_website"].ToString());
					
					//they are hobart if from vBranchAgentsZips
					newAgent.Latitude = myReader["latitude"].ToString();
					newAgent.Longitude = myReader["longitude"].ToString();	
					newAgent.Distance = int.Parse(myReader["dist"].ToString());	
					newAgent.Hobart = "Independant";
					
					IndAgents.Add(newAgent);
				}
			}
			
			myConnection.Close();
		}
		catch
		{
		}
	}
		
	public void GetHobarts()
	{
		bool Unique = true;
		
		SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
			//SqlConnection myConnection = new SqlConnection("Data Source=FSSQLTEST1.ITWFEG.BIZ;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");						   
		try
		{
			myConnection.Open();
			
			SqlDataReader myReader = null;
			
			SqlCommand  myCommand = new SqlCommand(
				"SELECT d.*, isnull(tz.distance,0) as dist, zips.latitude, zips.longitude " +
				"FROM vBranchAgentsZips d with (nolock) " +
				"join f_getzipsinrange('" + search + "', 10) tz on tz.zip = d.coverage_zipcode " +
				"join aracms.zip_codes zips with (nolock) on zips.zip = d.office_zip ",
			myConnection);   
													 

			myReader = myCommand.ExecuteReader();
			while(myReader.Read())
			{
				Unique = true;
				foreach (Agent d in HobartAgents)
				{ 
					if(d.Name == myReader["office_name"].ToString() && d.Address == myReader["office_addr"].ToString()) {
						Unique = false;
						
						 if(d.Distance > int.Parse(myReader["dist"].ToString())){
							d.Distance = int.Parse(myReader["dist"].ToString());
						} 
					}
				}
				
				if(Unique)
				{
					Agent newAgent = new Agent(myReader["office_name"].ToString(), myReader["office_addr"].ToString(), myReader["office_city"].ToString(), 
					myReader["office_state"].ToString(), myReader["office_zip"].ToString(), myReader["office_phone"].ToString(), myReader["office_website"].ToString());
					
					//they are hobart if from vBranchAgentsZips
					newAgent.Latitude = myReader["latitude"].ToString();
					newAgent.Longitude = myReader["longitude"].ToString();	
					newAgent.Distance = int.Parse(myReader["dist"].ToString());	
					newAgent.Hobart = "Hobart";
					
					HobartAgents.Add(newAgent);
				}
			}
			
			myConnection.Close();
		}
		catch
		{
		}
	}
}

public class Agent: IComparable<Agent> {
	private string _Name; 
	private string _Address;
	private string _City; 
	private string _State; 
	private string _Zip; 
	private string _Phone;
	private string _Website; 
	private string _Hobart; 
	private string _Latitude; 
	private string _Longitude;
	private int _Distance;
	
	
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

	public string Phone
    {
        set { this._Phone = value; }
        get { return this._Phone; }
    }
	
	public string Website
    {
        set { this._Website = value; }
        get { return this._Website; }
    }
	
	public string Hobart
    {
        set { this._Hobart = value; }
        get { return this._Hobart; }
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
	
	public int Distance
    {
        set { this._Distance = value; }
        get { return this._Distance; }
    }
	
	public Agent(){
	}
	
	public Agent(string sName, string sAddress, string sCity, string sState, string sZip, string sPhone, string sWebsite){
		_Name = sName;
		_Address = sAddress;
		_City = sCity;
		_State = sState;
		_Zip = sZip;
		_Phone = sPhone;
		_Website = sWebsite;
	}
	
	public int CompareTo( Agent other )
     {
         if ( this.Distance > other.Distance ) return 1;
         else if ( this.Distance < other.Distance ) return -1;
         else return 0;
     }
}