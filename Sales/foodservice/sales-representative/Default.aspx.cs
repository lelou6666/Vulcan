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
	public List<Sales_Rep> Reps = new List<Sales_Rep>();
	public string itemcount = "0";
	
	protected void Page_Load(object sender, EventArgs e)
	{
		string address;
		string search = stateValue.Text;
		bool Unique = true;
		
		if (stateValue.Text.Trim() == "")
        {
            //errormessage.Text = "Please enter a valid zipcode";
			state_title.Text = "";
			RepsContent.Visible = false;
        }
        else
        {
			RepsContent.Visible = true;

			//FSSQLTEST1.ITWFEG.BIZ
			//fstroysql1
			SqlConnection myConnection = new SqlConnection("Data Source=fstroysql1;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");
			//SqlConnection myConnection = new SqlConnection("Data Source=FSSQLTEST1.ITWFEG.BIZ;Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=marriner;Password=Vulcan123");						   
			try
			{
				myConnection.Open();
				
				SqlDataReader myReader = null;
				
				SqlCommand  myCommand = new SqlCommand("select * from sales_reps_all where [state_code] like '%" + search + "%'", myConnection);  
	
				myReader = myCommand.ExecuteReader();
				while(myReader.Read())
				{
					address = myReader["address"].ToString();
					if(myReader["Address2"].ToString() != "NULL")
					{
						address = address + " " + myReader["Address2"].ToString();
					}
					
					Sales_Rep newRep = new Sales_Rep(myReader["id"].ToString(), myReader["name"].ToString(), address, myReader["City"].ToString(), myReader["State"].ToString(),
					myReader["Zip"].ToString(), myReader["Phone"].ToString(), myReader["URL"].ToString(), myReader["territory"].ToString());
					
					Reps.Add(newRep);	
				}
				
				myConnection.Close();
			
				dp.QueryString = true;
				dp.PageSize = 10;
				dp.RecordSize = Reps.Count;
				itemcount = Reps.Count.ToString();

				state_title.Text = GetState(stateValue.Text);
				
			}
			catch
			{
				
			}

			if(Reps.Count == 0)
			{
				state_title.Text = "<h2>There are no Vulcan Sales Representatives for " + GetState(stateValue.Text) + ".</h2>";
			}
		}
	}     

	public string GetState(string state)
    {
        switch (state)
        {
            case "AL":
                return "Alabama";

            case "AK":
                return "Alaska";

            case "AZ":
                return "Arizona";

            case "AR":
                return "Arkansas";

            case "CA":
                return "California";

            case "CO":
                return "Colorado";

            case "CT":
                return "Connecticut";

            case "DE":
                return "Delaware";

			case "DC":
                return "District of Columbia";

            case "FL":
                return "Florida";

            case "GA":
                return "Georgia";

            case "HI":
                return "Hawaii";

            case "ID":
                return "Idaho";

            case "IL":
                return "Illinois";

            case "IN":
                return "Indiana";

            case "IA":
                return "Iowa";

            case "KS":
                return "Kansas";

            case "KY":
                return "Kentucky";

            case "LA":
                return "Louisiana";

            case "ME":
                return "Maine";

            case "MD":
                return "Maryland";

            case "MA":
                return "Massachusetts";

            case "MI":
                return "Michigan";

            case "MN":
                return "Minnesota";

            case "MS":
                return "Mississippi";

            case "MO":
                return "Missouri";

            case "MT":
                return "Montana";

            case "NE":
                return "Nebraska";

            case "NV":
                return "Nevada";

            case "NH":
                return "New Hampshire";

            case "NJ":
                return "New Jersey";

            case "NM":
                return "New Mexico";

            case "NY":
                return "New York";

            case "NC":
                return "North Carolina";

            case "ND":
                return "North Dakota";

            case "OH": 
                return "Ohio";

            case "OK":
                return "Oklahoma";

            case "OR":
                return "Oregon";

            case "PA":
                return "Pennsylvania";

            case "RI":
                return "Rhode Island";

            case "SC":
                return "South Carolina";

            case "SD":
                return "South Dakota";

            case "TN":
                return "Tennessee";

            case "TX":
                return "Texas";

            case "UT":
                return "Utah";

            case "VT":
                return "Vermont";

            case "VA":
                return "Virginia";

            case "WA":
                return "Washington";

            case "WV":
                return "West Virginia";

            case "WI":
                return "Wisconsin";

            case "WY":
                return "Wyoming";
        }

        return "";
    }
}

public class Sales_Rep {
	private string _Id; 
	private string _Name;
	private string _Address; 
	private string _City; 
	private string _State; 
	private string _Zip; 
	private string _Phone; 
	private string _Website; 
	private string _Territory;
	
	public string Id
    {
        set { this._Id = value; }
        get { return this._Id; }
    } 
	
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
        get { return this._Phone;; }
    }
	
	public string Website
    {
        set { this._Website = value; }
        get { return this._Website; }
    }
	
	public string Territory
    {
        set { this._Territory = value; }
        get { return this._Territory; }
    }
	
	public Sales_Rep(){
	}
	
	public Sales_Rep(string sId, string sName, string sAddress, string sCity, string sState, string sZip, string sPhone, string sWebsite, string sTerritory){
		_Id = sId;
		_Name = sName;
		_Address = sAddress;
		_City = sCity;
		_State = sState;
		_Zip = sZip;
		_Phone = sPhone;
		_Website = sWebsite;
		_Territory = sTerritory;
	}	 
}