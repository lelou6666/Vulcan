﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Framework;
using Ektron.Cms.Content;
using Ektron.Cms.Common;
using Ektron.Cms.API.Content;
using Ektron.Cms.API;
using Ektron.Cms.WebSearch;
using Ektron.Cms.WebSearch.SearchData;
using Vulcan.Product.Lib;

public partial class _Default : System.Web.UI.Page
{
	Ektron.Cms.API.Search.SearchManager searchapi = new Ektron.Cms.API.Search.SearchManager();
    SearchResponseData[] data = null;
    SearchResponseData[] data2 = null;
    private List<vModel> Models = new List<vModel>();
    private List<vTool> Tools = new List<vTool>();
    string productType = "";
    string bestInClass = "false";
    string energyStar = "false";
    string k12 = "false";
    string Powerfry = "false";
	string Frymate = "false";
	string Filter1 = "";
	string Filter2 = "";
	string Filter3 = "";
	string Filter4 = "";
	string FilterName1 = "";
	string FilterName2 = "";
	string FilterName3 = "";
	string FilterName4 = "";
	bool specialFryer = false;
	private int count = 0;
    
    protected void Page_Load(object sender, EventArgs e)
    {
		if(Request["id"].ToString() == "1413") {
			correctional_griddle.Visible = true;
			Model_Options.Visible = false;
		}
		else {
			correctional_griddle.Visible = false;
			Model_Options.Visible = true;
		}
		getModels();
		segment.Text = productType;
		title.Text = product_details.EkItem.Title;
		bestinclass.Text = bestInClass;
		energystar.Text = energyStar;
		k_twelve.Text = k12;
		
		if(product_details.EkItem.Title.IndexOf("PowerFry") != -1)
		{
			Powerfry = "true";
		}
		if(product_details.EkItem.Title.IndexOf("Frymate") != -1)
		{
			Frymate = "true";
		}
		
		powerfry.Text = Powerfry;
		frymate.Text = Frymate;
		
		if(filter.Text == "")
		{
			FilterDisplay.Visible = false;
		}
    }
	
	protected void ClearFiltering(object sender, EventArgs e)
    {
		if(productType == "Ovens") {
			Session.Remove("AllOvenItems1");
			//Session.Remove("AllOvenItems2");
			//Session.Remove("AllOvenItems3");
			Session.Remove("OvenFilters1");
			//Session.Remove("OvenFilters2");
			//Session.Remove("OvenFilters3");
		}
		if(productType == "Fryers") {
			Session.Remove("FryerItems1");
			Session.Remove("FryerItems2");
			Session.Remove("FryerItems3");
			Session.Remove("FryerFilters1");
			Session.Remove("FryerFilters2");
			Session.Remove("FryerFilters3");
		}
		if(productType == "Braising Pans") {
			Session.Remove("AllBraisingItems1");
			Session.Remove("AllBraisingItems2");
			Session.Remove("AllBraisingItems3");
			Session.Remove("BraisingFilters1");
			Session.Remove("BraisingFilters2");
			Session.Remove("BraisingFilters3");
		}
		if(productType == "Griddles") {
			Session.Remove("AllGriddleItems1");
			Session.Remove("AllGriddleItems2");
			Session.Remove("GriddleFilters1");
			Session.Remove("GriddleFilters2");
		}
		if(productType == "Charbroilers") {
			Session.Remove("AllCharbroilerItems1");
			Session.Remove("CharbroilerFilters1");
		}
		if(productType == "Heated Holding") {
			Session.Remove("AllHoldingItems1");
			Session.Remove("AllHoldingItems2");
			Session.Remove("AllHoldingItems3");
			Session.Remove("HoldingFilters1");
			Session.Remove("HoldingFilters2");
			Session.Remove("HoldingFilters3");
		}
		if(productType == "Kettles") {
			Session.Remove("AllKettleItems1");
			Session.Remove("AllKettleItems2");
			Session.Remove("AllKettleItems3");
			Session.Remove("KettleFilters1");
			Session.Remove("KettleFilters2");
			Session.Remove("KettleFilters3");
		}
		if(productType == "Steamers") {
			Session.Remove("AllSteamItems1");
			Session.Remove("AllSteamItems2");
			Session.Remove("AllSteamItems3");
			Session.Remove("SteamFilters1");
			Session.Remove("SteamFilters2");
			Session.Remove("SteamFilters3");
		}
		if(productType == "Ranges") {
			Session.Remove("AllRangeItems1");
			Session.Remove("AllRangeItems2");
			Session.Remove("AllRangeItems3");
			Session.Remove("AllRangeItems4");
			Session.Remove("RangeFilters1");
			Session.Remove("RangeFilters2");
			Session.Remove("RangeFilters3");
			Session.Remove("RangeFilters4");
		}
		if(productType == "Combi") {
			Session.Remove("AllCombiItems1");
			Session.Remove("AllCombiItems2");
			Session.Remove("CombiFilters1");
			Session.Remove("CombiFilters2");
		}
		
		Response.Redirect(Request.RawUrl.ToString());
	}
    
    private void getModels()
    {
		System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
		var capi = new Ektron.Cms.Framework.Core.Content.Content(Ektron.Cms.Framework.ApiAccessMode.Admin);
        var content = capi.GetItem(long.Parse(Request["id"]));
        string TaxonomyID = "";
        
        doc.LoadXml(content.Html);
        
        // load file path from xml
        System.Xml.XmlNode node = doc.SelectSingleNode("/root/tax/node()");
        if (node != null)
        {
            TaxonomyID = node.InnerText;
        }
        
		Taxonomy api = new Taxonomy();
        TaxonomyData tdata = new TaxonomyData();
        TaxonomyRequest treq = new TaxonomyRequest();
        treq.TaxonomyLanguage = 1033;

        string searchtext = "";
        string taxonomyPath = "";

        treq.TaxonomyId = long.Parse(TaxonomyID);
        tdata = api.LoadTaxonomy(ref treq);

        taxonomyPath = tdata.TaxonomyPath;
        
        if(taxonomyPath.IndexOf(@"\Vulcan Products\Ranges") != -1)
        {
			productType = "Ranges";
			Collection1.DefaultCollectionID = 10;
			Collection1.Fill();
        }
        else if(taxonomyPath.IndexOf(@"\Vulcan Products\Fryers") != -1)
        {
			productType = "Fryers";
			Collection1.DefaultCollectionID = 9;
			Collection1.Fill();
        }
        else if(taxonomyPath.IndexOf(@"\Vulcan Products\Griddles") != -1)
        {
			productType = "Griddles";
			Collection1.DefaultCollectionID = 12;
			Collection1.Fill();
        }
		else if(taxonomyPath.IndexOf(@"\Vulcan Products\Charbroilers") != -1)
        {
			productType = "Charbroilers";
			Collection1.DefaultCollectionID = 20;
			Collection1.Fill();
        }
        else if(taxonomyPath.IndexOf(@"\Vulcan Products\Ovens") != -1)
        {
			productType = "Ovens";
			Collection1.DefaultCollectionID = 11;
			Collection1.Fill();
        }
        else if(taxonomyPath.IndexOf(@"\Vulcan Products\Heated Holding") != -1)
        {
			productType = "Heated Holding";
			also_info.Visible = false;
			Collection1.DefaultCollectionID = 16;
			Collection1.Fill();
        }
        else if(taxonomyPath.IndexOf(@"\Vulcan Products\Steamers") != -1)
        {
			productType = "Steamers";
			Collection1.DefaultCollectionID = 13;
			Collection1.Fill();
        }
        else if(taxonomyPath.IndexOf(@"\Vulcan Products\Kettles") != -1)
        {
			productType = "Kettles";
			Collection1.DefaultCollectionID = 14;
			Collection1.Fill();
        }
        else if(taxonomyPath.IndexOf(@"\Vulcan Products\Braising Pans") != -1)
        {
			productType = "Braising Pans";
			Collection1.DefaultCollectionID = 15;
			Collection1.Fill();
        }
		else if(taxonomyPath.IndexOf(@"\Vulcan Products\Combi") != -1)
        {
			productType = "Combi";
			also_info.Visible = false;
			Collection1.DefaultCollectionID = 19;
			Collection1.Fill();
        }

        data = GetSearchResults(searchtext, taxonomyPath, 78);

        if (data == null || data.Length < 1)
        {
            // Show no results div

        }
        else
        {
			foreach (SearchResponseData productData in data)
			{
				if(taxonomyPath.IndexOf(@"KleenScreen") == -1 && productData.Title.IndexOf(@"F") != -1 && productType == "Fryers" && productData.Title.IndexOf(@"Frymate") == -1 && productData.Title.IndexOf(@"MF-1") == -1)
				{
					//If we are viewing a Freestanding non-kleanscreen product, do not show filteration models (indicated by an F in the model name).
					specialFryer = true;
				}
				else {
					GetModelInformation(productData.ContentID, productData.Title);
				}
			}
			
			Models.Sort((s1, s2) => s1.Order.CompareTo(s2.Order));
			
            this.UXListView.DataSource = Models;
            this.UXListView.DataBind();
        }
        
        data2 = GetSearchResults(searchtext, taxonomyPath, 118);
        if (data2 == null || data2.Length < 1)
        {
            // Show no results div
            toolbox.Visible = false;
        }
        else
        {
			foreach (SearchResponseData productData in data2)
			{
				GetTools(productData.ContentID, productData.Title);
			}

            this.UXListView2.DataSource = Tools;
            this.UXListView2.DataBind();
        }
        
        ScriptManager.RegisterStartupScript( this, this.GetType(),"MyAction", "SizeModels();", true);
    }
    
    public void GetModelInformation(long ContentId, string Title){
		System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
		string Description = "";
		string Image = "";
		string Specs = "";
		string SellSheet = "";
		string Manual = "";
		string PartsCatelog = "";
		string ServiceManual = "";
		string WaterFilter = "";
		string ToolboxTitle = "";
		string url = "";
		string thumbnail = "";
		string AdditionalInfo = "";
		string HighRes = "";
		bool   matchFilter1 = false;
		bool   matchFilter2 = false;
		bool   matchFilter3 = false;
		bool   matchFilter4 = false;
		int Order = 0;
		count += 1;
		
		
		var capi = new Ektron.Cms.Framework.Core.Content.Content(Ektron.Cms.Framework.ApiAccessMode.Admin);
        var content = capi.GetItem(ContentId);

        if (content == null) 
			return;
			
		//Get Taxonomy Data	
		Ektron.Cms.API.Content.Taxonomy taxonomyApi = new Ektron.Cms.API.Content.Taxonomy();
		TaxonomyBaseData[] taxBaseData = taxonomyApi.ReadAllAssignedCategory(ContentId);
		 
		if (taxBaseData.Length > 0)
		{
			long taxonomyid = taxBaseData[0].TaxonomyId;

			for(int i = 0; i < taxBaseData.Length; i++) {
				if(productType == "Ovens") {
					matchFilter2 = true;
					matchFilter3 = true;
					matchFilter4 = true;

					if(Session["OvenFilters1"] != null) {
						FilterName1 = "Power Source: ";
						FilterName2 = "Size: ";
						FilterName3 = "Configuration: ";
						
						
						if(checkFilter(Session["OvenFilters1"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 1) == true)
						{
							matchFilter1 = true;
						}
					} 
					else
					{
						matchFilter1 = true;
						matchFilter2 = true;
						matchFilter3 = true;
						matchFilter4 = true;
						FilterDisplay.Visible = false;
					} 
				}
				if(productType == "Fryers") {
					matchFilter4 = true;

					if(Session["FryerFilters1"] != null || Session["FryerFilters2"] != null || Session["FryerFilters3"] != null) {
						FilterName1 = "Power Source: ";
						FilterName2 = "Oil Capacity Per Fryer: ";
						FilterName3 = "Battery: ";
						
						
						if(checkFilter(Session["FryerFilters1"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 1) == true)
						{
							matchFilter1 = true;
						}
						
						if(checkFilter(Session["FryerFilters2"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 2) == true)
						{
							matchFilter2 = true;
						}
						
						if(checkFilter(Session["FryerFilters3"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 3) == true)
						{
							matchFilter3 = true;
						}
					} 
					else
					{
						matchFilter1 = true;
						matchFilter2 = true;
						matchFilter3 = true;
						matchFilter4 = true;
						FilterDisplay.Visible = false;
					} 
				}
				if(productType == "Braising Pans") {
					matchFilter4 = true;

					if(Session["BraisingFilters1"] != null || Session["BraisingFilters2"] != null || Session["BraisingFilters3"] != null) {
						FilterName1 = "Power Source: ";
						FilterName2 = "Configuration: ";
						FilterName3 = "Capacity: ";
						
						
						if(checkFilter(Session["BraisingFilters1"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 1) == true)
						{
							matchFilter1 = true;
						}
						
						if(checkFilter(Session["BraisingFilters2"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 2) == true)
						{
							matchFilter2 = true;
						}
						
						if(checkFilter(Session["BraisingFilters3"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 3) == true)
						{
							matchFilter3 = true;
						}
					} 
					else
					{
						matchFilter1 = true;
						matchFilter2 = true;
						matchFilter3 = true;
						matchFilter4 = true;
						FilterDisplay.Visible = false;
					} 
				}
				if(productType == "Griddles") {
					matchFilter3 = true;
					matchFilter4 = true;

					if(Session["GriddleFilters1"] != null || Session["GriddleFilters2"] != null) {
						FilterName1 = "Power Source: ";
						FilterName2 = "Size: ";
						
						
						if(checkFilter(Session["GriddleFilters1"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 1) == true)
						{
							matchFilter1 = true;
						}
						
						if(checkFilter(Session["GriddleFilters2"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 2) == true)
						{
							matchFilter2 = true;
						}
					} 
					else
					{
						matchFilter1 = true;
						matchFilter2 = true;
						matchFilter3 = true;
						matchFilter4 = true;
						FilterDisplay.Visible = false;
					} 
				}
				if(productType == "Charbroilers") {
					matchFilter2 = true;
					matchFilter3 = true;
					matchFilter4 = true;

					if(Session["CharbroilerFilters1"] != null) {
						FilterName1 = "Size: ";
						
						
						if(checkFilter(Session["CharbroilerFilters1"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 1) == true)
						{
							matchFilter1 = true;
						}
					} 
					else
					{
						matchFilter1 = true;
						matchFilter2 = true;
						matchFilter3 = true;
						matchFilter4 = true;
						FilterDisplay.Visible = false;
					} 
				}
				if(productType == "Heated Holding") {
					matchFilter4 = true;

					if(Session["HoldingFilters1"] != null || Session["HoldingFilters2"] != null || Session["HoldingFilters3"] != null) {
						FilterName1 = "Power Source: ";
						FilterName2 = "Product Type: ";
						FilterName3 = "Doors: ";
						
						
						if(checkFilter(Session["HoldingFilters1"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 1) == true)
						{
							matchFilter1 = true;
						}
						
						if(checkFilter(Session["HoldingFilters2"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 2) == true)
						{
							matchFilter2 = true;
						}
						
						if(checkFilter(Session["HoldingFilters3"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 3) == true)
						{
							matchFilter3 = true;
						}
					} 
					else
					{
						matchFilter1 = true;
						matchFilter2 = true;
						matchFilter3 = true;
						matchFilter4 = true;
						FilterDisplay.Visible = false;
					} 
				}
				if(productType == "Kettles") {
					matchFilter4 = true;

					if(Session["KettleFilters1"] != null || Session["KettleFilters2"] != null || Session["KettleFilters3"] != null) {
						FilterName1 = "Power Source: ";
						FilterName2 = "Configuration: ";
						FilterName3 = "Capacity: ";
						
						
						if(checkFilter(Session["KettleFilters1"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 1) == true)
						{
							matchFilter1 = true;
						}
						
						if(checkFilter(Session["KettleFilters2"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 2) == true)
						{
							matchFilter2 = true;
						}
						
						if(checkFilter(Session["KettleFilters3"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 3) == true)
						{
							matchFilter3 = true;
						}
					} 
					else
					{
						matchFilter1 = true;
						matchFilter2 = true;
						matchFilter3 = true;
						matchFilter4 = true;
						FilterDisplay.Visible = false;
					} 
				}
				if(productType == "Steamers") {
					matchFilter4 = true;

					if(Session["SteamFilters1"] != null || Session["SteamFilters2"] != null || Session["SteamFilters3"] != null) {
						FilterName1 = "Power Source: ";
						FilterName2 = "Water Connection: ";
						FilterName3 = "Configuration: ";
						
						
						if(checkFilter(Session["SteamFilters1"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 1) == true)
						{
							matchFilter1 = true;
						}
						
						if(checkFilter(Session["SteamFilters2"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 2) == true)
						{
							matchFilter2 = true;
						}
						
						if(checkFilter(Session["SteamFilters3"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 3) == true)
						{
							matchFilter3 = true;
						}
					} 
					else
					{
						matchFilter1 = true;
						matchFilter2 = true;
						matchFilter3 = true;
						matchFilter4 = true;
						FilterDisplay.Visible = false;
					} 
				}
				if(productType == "Ranges") {
					if(Session["RangeFilters1"] != null || Session["RangeFilters2"] != null || Session["RangeFilters3"] != null  || Session["RangeFilters4"] != null) {
						FilterName1 = "";
						FilterName2 = "Power Source: ";
						FilterName3 = "Product Type: ";
						FilterName4 = "Size: ";
						
						
						if(checkFilter(Session["RangeFilters1"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 1) == true)
						{
							matchFilter1 = true;
						}
						
						if(checkFilter(Session["RangeFilters2"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 2) == true)
						{
							matchFilter2 = true;
						}
						
						if(checkFilter(Session["RangeFilters3"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 3) == true)
						{
							matchFilter3 = true;
						}
						if(checkFilter(Session["RangeFilters4"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 4) == true)
						{
							matchFilter4 = true;
						}
					} 
					else
					{
						matchFilter1 = true;
						matchFilter2 = true;
						matchFilter3 = true;
						matchFilter4 = true;
						FilterDisplay.Visible = false;
					} 
				}
				if(productType == "Combi") {
					matchFilter3 = true;
					matchFilter4 = true;
						
					if(Session["CombiFilters1"] != null || Session["CombiFilters2"] != null) {
						FilterName1 = "Power Source: ";
						FilterName2 = "Capacity: ";
						//FilterName3 = "Product Type: ";
						//FilterName4 = "Size: ";
						
						
						if(checkFilter(Session["CombiFilters1"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 1) == true)
						{
							matchFilter1 = true;
						}
						
						if(checkFilter(Session["CombiFilters2"] as List<string>, taxBaseData[i].TaxonomyPath, taxBaseData[i].TaxonomyName, 2) == true)
						{
							matchFilter2 = true;
						}
					} 
					else
					{
						matchFilter1 = true;
						matchFilter2 = true;
						matchFilter3 = true;
						matchFilter4 = true;
						FilterDisplay.Visible = false;
					} 
				}
			} 
			
			for(int i = 0; i < taxBaseData.Length; i++){
				if(taxBaseData[i].TaxonomyName == "Best in Class")
				{
					bestInClass = "true";
				}
				else if(taxBaseData[i].TaxonomyName == "Energy Star")
				{
					energyStar = "true";
				}
				else
				{
					Taxonomy api2 = new Taxonomy();
					TaxonomyData tdata2 = new TaxonomyData();
					TaxonomyRequest treq2 = new TaxonomyRequest();
					treq2.TaxonomyLanguage = 1033;

					string searchtext = "";
					string taxonomyPath = "";

					treq2.TaxonomyId = taxBaseData[i].TaxonomyParentId;
					tdata2 = api2.LoadTaxonomy(ref treq2);

					if(tdata2.TaxonomyPath.IndexOf("Series") == -1)
					{
						if(taxBaseData[i].TaxonomyName == "Filtration")
						{
							AdditionalInfo += "<p><strong>Available with Filtration System</strong></p>";
						}
						else if(taxBaseData[i].TaxonomyName == "K-12")
						{
							//AdditionalInfo += "<p><strong>Available with the Just 4 Schools program</strong></p>";
							k12 = "True";
						}
						else if(taxBaseData[i].TaxonomyName == "Energy Efficient" || taxBaseData[i].TaxonomyName == "Insulated")
						{
							AdditionalInfo += "<p><strong>" + taxBaseData[i].TaxonomyName + "</strong></p>";
						}
						else
						{
							if(tdata2.TaxonomyName == "Oil Capacity per Fryer"  && specialFryer == false)
							{
								AdditionalInfo = "<p><strong>" + tdata2.TaxonomyName + ":</strong> " + taxBaseData[i].TaxonomyName + "</p>" + AdditionalInfo;
							}
							else {
								AdditionalInfo += "<p><strong>" + tdata2.TaxonomyName + ":</strong> " + taxBaseData[i].TaxonomyName + "</p>";
							}
						}
					} 
				}
			}
			
			//Places here so added at the end of the list.
			if(k12 == "True") {
				AdditionalInfo += "<p><strong>Available with the Just 4 Schools program</strong></p>";
			}
		}
		
		if(matchFilter1 && matchFilter2 && matchFilter3 && matchFilter4)
		{
			
			if(Filter1 != "" || Filter2 != "" || Filter3 != "" || Filter4 != "") {
				
				filter.Text = "<div style='color:#c4c3c3; font-size:24px; font-family:\"futura medium condensed\", Arial, Sans-Serif;'><u>Models Filtered By</u></div><p>";
				
				if(Filter1 != "")
					filter.Text += FilterName1 + Filter1.Substring(0,Filter1.Length -2) + "<br />";
					
				if(Filter2 != "")
					filter.Text += FilterName2 + Filter2.Substring(0,Filter2.Length -2) + "<br />";
					
				if(Filter3 != "")
					filter.Text += FilterName3 + Filter3.Substring(0,Filter3.Length -2) + "<br />";
					
				if(Filter4 != "")
					filter.Text += FilterName4 + Filter4.Substring(0,Filter4.Length -2) + "<br />";
					
				filter.Text += "</p>";
			}
		}
		else
		{
			return;
		} 

		try
		{
			doc.LoadXml(content.Html);
		}
		catch
		{
			doc = null;
		}
		if (doc == null) 
			return;
			
		// load file path from xml
		System.Xml.XmlNode node = doc.SelectSingleNode("/root/description");
		if (node != null)
		{
			Description = node.InnerXml.ToString();
		}
        
		node = doc.SelectSingleNode("/root/modelimage/node()");
		if (node != null)
		{
			Image = "<div class='model_img'><img border='0' alt='' src='" + node.InnerText + "' /></div>";
		}
		
		node = doc.SelectSingleNode("/root/high_res/node()");
		if (node != null)
		{
			HighRes = "<a href='" + node.InnerText + "' class='specs'>High Resolution Images</a>";
		}
		
		node = doc.SelectSingleNode("/root/order/node()");
        if (node != null)
        {
			if(node.InnerText.Trim() != "") {
            	Order = Convert.ToInt32(node.InnerText.Trim());
			}
        }
		
		if(Order == 0)
		{
			Order = count + 100;
		}
		
		//Get Model Info from Resource Center Site
		DataTable dt;
		DataRow dr;

		dt = Vulcan.Product.Lib.SharepointHelper.GetGrid(Title);
		
		//blah.DataSource = dt;
		//blah.DataBind();
		
		if (dt != null)
		{
			for(int i = 0; i < dt.Rows.Count; i++)
			{
				dr = dt.Rows[i];
				string strTitle = dr["ows_Document Type"].ToString();
				string strUrl = dr["ows_EncodedAbsUrl"].ToString();
				string strModel = dr["ows_Modles"].ToString();
				string[] stringSeparators = new string[] {";", ","};
				string[] sModels;
				bool isExactModel = false;
				
				if(strModel.Trim() == Title.Trim()) {
					isExactModel = true;
				}
				
				sModels = strModel.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
					
				foreach (string s in sModels)
      			{
					if(s.Trim() == Title.Trim()) {
						isExactModel = true;
					}
				}
				
				if(isExactModel == true)
				{	
					if(dr["ows_Product Group"].ToString().IndexOf("Vulcan") != -1) {
						if(strTitle == "Spec Sheet")
						{
							Specs = "<a class='specs' href='" + strUrl + "' target='_blank'>Spec Sheet</a>";
						}
						if(strTitle == "Sell Sheet")
						{
							SellSheet = "<a class='specs' href='" + strUrl + "' target='_blank'>Sell Sheet</a>";
						}
						else if(strTitle == "Installation and Operation Manual" || strTitle == "Installation Manual" || strTitle == "Operation Manual")
						{
							Manual = "<a class='specs' href='" + strUrl + "' target='_blank'>Owners Manual</a>";
						}
						else if(strTitle == "Parts Catalog")
						{
							PartsCatelog = "<a class='specs' href='" + strUrl + "' target='_blank'>Parts Catalog</a>";
						}
						else if(strTitle == "Service Manual")
						{
							ServiceManual = "<a class='specs' href='" + strUrl + "' target='_blank'>Service Manual</a>";
						}
						else if(strTitle == "Water Filter Specs")
						{
							WaterFilter = "<a class='specs' href='" + strUrl + "' target='_blank'>Water Filter Specs</a>";
						}
					}
				}
			}
		} 

		vModel newModel = new vModel(ContentId, Title, Description, Image, Specs, Manual, SellSheet, PartsCatelog, ServiceManual, AdditionalInfo, HighRes, Order);
		Models.Add(newModel);
    }
	
	public bool checkFilter(List<string> sessionList, string TaxonomyPath, string FilterName, int filterCount){
		bool match = false;
		
		//filter.Text += TaxonomyPath + "<br />";
		
		if(sessionList != null) {
			if(sessionList.Count == 0)
			{
				match = true;
			}
			else
			{
				foreach(string f in sessionList)
				{
					if(f == TaxonomyPath)
					{
						if(filterCount == 1) {
							if(Filter1.IndexOf(FilterName) == -1)
								Filter1 += FilterName + ", ";
						}
						if(filterCount == 2) {
							if(Filter2.IndexOf(FilterName) == -1)
								Filter2 += FilterName + ", ";
						}
						if(filterCount == 3) {
							if(Filter3.IndexOf(FilterName) == -1)
								Filter3 += FilterName + ", ";
						}
						if(filterCount == 4) {
							if(Filter4.IndexOf(FilterName) == -1)
								Filter4 += FilterName + ", ";
						}
						
						match = true;
					}
				}
			}
		}
		else
		{
			//filter.Text += filterCount.ToString() + " is null <br />";
		}
		 
		 return match;
	}
    
    public void GetTools(long ContentId, string Title){
		System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
		string ToolboxTitle = "";
		string url = "";
		string link = "";
		string thumbnail = "";
		
		
		var capi = new Ektron.Cms.Framework.Core.Content.Content(Ektron.Cms.Framework.ApiAccessMode.Admin);
        var content = capi.GetItem(ContentId);

        if (content == null) 
        {
			toolbox.Visible = false;
			return;
		}
		
		try
		{
			doc.LoadXml(content.Html);
		}
		catch
		{
			doc = null;
		}
		if (doc == null)
		{
			toolbox.Visible = false;
			return;
		}
			
		// load file path from xml
		System.Xml.XmlNode node = doc.SelectSingleNode("/root/title");
		if (node != null)
		{
			ToolboxTitle = node.InnerXml.ToString();
		}
		
		node = doc.SelectSingleNode("/root/link");
		if (node != null)
		{
			url = node.InnerXml.ToString();
		}
		
		
		if(url != "") {
			if(url.IndexOf("Savings_Calculator") != -1)
			{
				link = "<a href='" + url + "' class='alt floatbox;' data-fb-options='width:950 height:570 outsideClickCloses:true splitResize:false " +
							"showNewWindow:false showPrint:false enableDragResize:true disableScroll:true'>" + ToolboxTitle + "</a>";
			}
			else
			{
				link = "<a href='" + url + "' target='_blank'>" + ToolboxTitle + "</a>";
			}
		}
		
		node = doc.SelectSingleNode("/root/thumbnail");
		if (node != null)
		{
			thumbnail = "<a href='" + url + "' target='_blank'><img src='" + node.InnerXml.ToString() + "'/></a>";
		}
		
		vTool newTool = new vTool(ToolboxTitle, link, thumbnail);
		Tools.Add(newTool);
    }
    
    private SearchResponseData[] GetSearchResults(string searchtxt, string taxonomysearchtext, long folderId)
    {
        int resultCount = 0;
        var searchData = new Ektron.Cms.WebSearch.SearchData.SearchRequestData();

        searchData.EnablePaging = false;
        searchData.SearchFor = Ektron.Cms.WebSearch.SearchDocumentType.html;
        //searchData.SearchType = WebSearchType.none;
        searchData.OrderDirection = Ektron.Cms.WebSearch.SearchData.WebSearchOrderByDirection.Descending;
        searchData.LanguageID = 1033;
        searchData.Recursive = false;  //This means no sub folders
        searchData.FolderID = folderId;
        //searchData.SearchText = searchtxt;
        searchData.Category = taxonomysearchtext;
        searchData.OrderBy = WebSearchOrder.Title;
        searchData.OrderDirection = WebSearchOrderByDirection.Ascending;
        searchData.TaxOperator = TaxonomyOperator.and;

        return this.searchapi.Search(searchData, HttpContext.Current, ref resultCount);
    }
}

public class vModel {
	private long _ID; 
	private string _Title; 
	private string _Description; 
	private string _Image;
	private string _SpecSheet;
	private string _SellSheet;
	private string _Manual;
	private string _PartsCatelog;
	private string _ServiceManual;
	private string _AdditionalInfo;
	private string _HighRes;
	private int _Order;
	
	public long ID
    {
        set { this._ID = value; }
        get { return this._ID; }
    }
	
	public string Title
    {
        set { this._Title = value; }
        get { return this._Title; }
    } 
    
    public string Description
    {
        set { this._Description = value; }
        get { return this._Description; }
    } 
    
    public string Image
    {
        set { this._Image = value; }
        get { return this._Image; }
    }
    
    public string SpecSheet
    {
        set { this._SpecSheet = value; }
        get { return this._SpecSheet; }
    }
	
	public string SellSheet
    {
        set { this._SellSheet = value; }
        get { return this._SellSheet; }
    }
    
    public string Manual
    {
        set { this._Manual = value; }
        get { return this._Manual; }
    }
    
    public string PartsCatelog
    {
        set { this._PartsCatelog = value; }
        get { return this._PartsCatelog; }
    }
    
    public string ServiceManual
    {
        set { this._ServiceManual = value; }
        get { return this._ServiceManual; }
    }
    
    public string AdditionalInfo
    {
        set { this._AdditionalInfo = value; }
        get { return this._AdditionalInfo; }
    }
    
    public string HighRes
    {
        set { this._HighRes = value; }
        get { return this._HighRes; }
    }
	
	public int Order
    {
        set { this._Order = value; }
        get { return this._Order; }
    }
	
	public vModel(){
	}
	
	public vModel(long lID, string sTitle, string sDescription, string sImage, string sSpecSheet, string sSellSheet, string sManual, string sPartsCatelog, string sServiceManual, string sAdditionalInfo, string sHighRes, int iOrder){
		
		_ID = lID;
		_Title = sTitle;
		_Description = sDescription;
		_Image = sImage;
		_SpecSheet = sSpecSheet;
		_SellSheet = sSellSheet;
		_Manual = sManual;
		_PartsCatelog = sPartsCatelog;
		_ServiceManual = sServiceManual;
		_AdditionalInfo = sAdditionalInfo;
		_HighRes = sHighRes;
		_Order = iOrder;
	}
}

public class vTool {
	private string _Title; 
	private string _URL; 
	private string _Thumbnail;
	
	
	public string Title
    {
        set { this._Title = value; }
        get { return this._Title; }
    } 
    
    public string URL
    {
        set { this._URL = value; }
        get { return this._URL; }
    } 
    
    public string Thumbnail
    {
        set { this._Thumbnail = value; }
        get { return this._Thumbnail; }
    }
	public vTool(){
	}
	
	public vTool(string sTitle, string sURL, string sThumbnail){
		
		_Title = sTitle;
		_URL = sURL;
		_Thumbnail = sThumbnail;
	}
}

public class vFilter {
	private string _Name; 
	private string _Path;
	
	public string Name
    {
        set { this._Name = value; }
        get { return this._Name; }
    } 
	
	public string Path
    {
        set { this._Path = value; }
        get { return this._Path; }
    }
	
	public vFilter(){
	}
	
	public vFilter(string sName, string sPath){
		_Name = sName;
		_Path = sPath;
	}
}