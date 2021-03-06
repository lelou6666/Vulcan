﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Ektron.Cms;
using Ektron.Cms.Framework;
using Ektron.Cms.Content;
using Ektron.Cms.Common;
using Ektron.Cms.API.Content;
using Ektron.Cms.API;
using Ektron.Cms.WebSearch;
using Ektron.Cms.WebSearch.SearchData;

public partial class _Default : System.Web.UI.Page
{
	Ektron.Cms.API.Search.SearchManager searchapi = new Ektron.Cms.API.Search.SearchManager();
    SearchResponseData[] data = null;
	SearchResponseData[] ImageData = null;
    public List<Series> SeriesList = new List<Series>();
    public List<Filter> FilterOptions1 = new List<Filter>();
    public List<Filter> FilterOptions2 = new List<Filter>();
    public List<Filter> FilterOptions3 = new List<Filter>();
    public List<string> FilterList1 = new List<string>();
    public List<string> FilterList2 = new List<string>();
    public List<string> FilterList3 = new List<string>();
	public string sFilterTracking = "";
	
	protected void showGrid(object sender, EventArgs e)
    {
		Session["view"] = "grid";
		Page.ClientScript.RegisterStartupScript(this.GetType(),"Function1","GridTracking();",true);
		Response.Redirect("/products/braising-pans/");
	}
	
	protected void showList(object sender, EventArgs e)
    {
		Session["view"] = "list";
		Page.ClientScript.RegisterStartupScript(this.GetType(),"Function2","ListTracking();",true);
		Response.Redirect("/products/braising-pans/");
	}
    
    protected void Page_Load(object sender, EventArgs e)
    {
		string photoUrl = "";
		string view = "";
		if(Session["view"] != null) {
			view = (string)(Session["view"]);
		}
		else {
			view = "list";
		}
		
		if(view == "grid"){
			btnGrid.ImageUrl = "/images/grid_on.jpg";
			btnList.ImageUrl = "/images/list.jpg";
		}

		if(view == "grid") {
			ScriptManager.RegisterStartupScript( UpdatePanel1, this.GetType(),"MyAction", "loadModels();", true);
		}
		
		bool searchMatch = false;
		full_list.Controls.Clear();
		
		LoadFilter(314, powersource, 1);
		LoadFilter(317, configuration, 2);
		LoadFilter(594, size, 3);
		
		//Filters
		foreach (ListItem oItem in powersource.Items)
        {
            if (oItem.Selected)
            {
				foreach(Filter f in FilterOptions1)
				{
					if(f.Name == oItem.Value)
					{
						 FilterList1.Add(f.Path);
						 sFilterTracking += f.Name + ", ";
					}
				}
            }
        }
        
		foreach (ListItem oItem in configuration.Items)
        {
            if (oItem.Selected)
            {
				foreach(Filter f in FilterOptions2)
				{
					if(f.Name == oItem.Value)
					{
						 FilterList2.Add(f.Path);
						 sFilterTracking += f.Name + ", ";
					}
				}
            }
        }
      
        foreach (ListItem oItem in size.Items)
        {
            if (oItem.Selected)
            {
				foreach(Filter f in FilterOptions3)
				{
					if(f.Name == oItem.Value)
					{
						 FilterList3.Add(f.Path);
						 sFilterTracking += f.Name + ", ";
					}
				}
            }
        }
		
		if(sFilterTracking != "") {
			ScriptManager.RegisterStartupScript( UpdatePanel1, this.GetType(),"Function3", "TrackFiltering('" + sFilterTracking + "');", true);
		}
		
		LoadSeries();
		
		foreach (Series series in SeriesList)
		{
			Control control = LoadControl("~/products/productlist.ascx");
			ph.Controls.Add(control);
			
			productlist uc = control as productlist;
			if (uc == null)
			{
				PartialCachingControl pcc = control as PartialCachingControl;
				if (pcc != null) uc = pcc.CachedControl as productlist;
			}
			
			if (uc != null)
			{ 
				uc.ProductView = view;
				uc.TaxonomyPath = series.Path;
				uc.filters1 = FilterList1;
				uc.filters2 = FilterList2;
				uc.filters3 = FilterList3;
				
				uc.loadContent();
			}
				
			if(view == "grid") {
				if (uc != null)
				{ 
					//Do not display Series that do not have content
					if(uc.HasContent)
					{
						Literal newLiteral = new Literal();
						Literal newLiteral2 = new Literal();
						newLiteral.Text = "<h2 class='green_h2' style='border-bottom:1px solid #6c6c6a; margin-bottom:15px; text-transform:uppercase;' id='" + series.ID + "'>" + series.Name + "</h2>" 
										+ "<div class='prodsearchitems'>";
						full_list.Controls.Add(newLiteral);
						full_list.Controls.Add(uc);
						
						newLiteral2.Text = "</div><div style='clear:both;'></div>";
						full_list.Controls.Add(newLiteral2);
						
						searchMatch = true;
					}
				}
			}
			else
			{
				if (uc != null)
				{ 
					//Do not display Series that do not have content
					if(uc.HasContent)
					{
						Literal newLiteral = new Literal();
						Literal newLiteral2 = new Literal();
						
						newLiteral.Text = "<h2 class='green_h2' style='margin-top:15px; text-transform:uppercase;' id='" + series.ID + "'>" + series.Name + "</h2>"
										+ "<div class='prod_listview' style='width:100%; background-color:#272727; border-top:1px solid #6b6a69; overflow:auto;'>";
	
						full_list.Controls.Add(newLiteral);
						full_list.Controls.Add(uc);
						
						if(uc.ProductCount > 4) {
							ImageData = GetSeriesPhoto(series.Path);
	
							if (ImageData == null || ImageData.Length < 1)
							{
							}
							else
							{
								foreach (SearchResponseData data1 in ImageData)
								{
									photoUrl = data1.QuickLink;
								}
							}
							
							if(photoUrl != "")
							{
								newLiteral2.Text = "<div class='series_photo_wrapper'><div class='series_photo_padding'><img class='series_photo' src='" + photoUrl + "' /></div></div></div>";
							}
							else
							{
								newLiteral2.Text = "</div>";
							}
						}
						else  {
							newLiteral2.Text = "</div>";
						}
						
						full_list.Controls.Add(newLiteral2);
			
						searchMatch = true;
					}
				}
			}
		}
		
		if(searchMatch == false)
		{
			Literal newLiteral = new Literal();
			newLiteral.Text = "<div class='no_products'>No products found.</div>";
			full_list.Controls.Add(newLiteral);
		}
		
		Session.Add("AllBraisingItems1", powersource.Items);
		Session.Add("AllBraisingItems2", configuration.Items);
		Session.Add("AllBraisingItems3", size.Items);
		Session.Add("BraisingFilters1", FilterList1);
		Session.Add("BraisingFilters2", FilterList2);
		Session.Add("BraisingFilters3", FilterList3);
    }
    
    public void LoadSeries()
    {
        Taxonomy api = new Taxonomy();
        TaxonomyData tdata = new TaxonomyData();
        TaxonomyRequest treq = new TaxonomyRequest();
        EkContent tapi = new EkContent();
        TaxonomyBaseData[] tbases;

        treq.TaxonomyId = 503; //Braising Series
        treq.TaxonomyLanguage = 1033;
        tdata = api.LoadTaxonomy(ref treq);
        tbases = api.EkContentRef.ReadAllSubCategories(treq);

        foreach (TaxonomyBaseData tbase in tbases)
        {
            Series newSeries = new Series(tbase.TaxonomyName, tbase.TaxonomyPath, tbase.TaxonomyId.ToString());
			SeriesList.Add(newSeries);
        }
    }
	
	//This method gets the series image
    private SearchResponseData[] GetSeriesPhoto(string taxonomysearchtext)
    {
        var searchData = new Ektron.Cms.WebSearch.SearchData.SearchRequestData();
        int resultCount = 0;

        searchData.EnablePaging = false;
        searchData.SearchFor = Ektron.Cms.WebSearch.SearchDocumentType.images;
        searchData.OrderDirection = Ektron.Cms.WebSearch.SearchData.WebSearchOrderByDirection.Descending;
        searchData.LanguageID = 1033;
        searchData.Recursive = false;  //Search Subfolders
        searchData.FolderID = 84; // Products Folder
        searchData.Category = taxonomysearchtext;
        searchData.OrderBy = WebSearchOrder.Title;
        searchData.OrderDirection = WebSearchOrderByDirection.Ascending;
        searchData.TaxOperator = TaxonomyOperator.and;

        return this.searchapi.Search(searchData, HttpContext.Current, ref resultCount);
    }
    
    public void LoadFilter(int TaxonomyId, CheckBoxList cbList, int listNumber)
    {
        Taxonomy api = new Taxonomy();
        TaxonomyData tdata = new TaxonomyData();
        TaxonomyRequest treq = new TaxonomyRequest();
        EkContent tapi = new EkContent();
        TaxonomyBaseData[] tbases;

        treq.TaxonomyId = TaxonomyId;
        treq.TaxonomyLanguage = 1033;
        tdata = api.LoadTaxonomy(ref treq);
        tbases = api.EkContentRef.ReadAllSubCategories(treq);

        foreach (TaxonomyBaseData tbase in tbases)
        {
			if(!IsPostBack)
			{
				ListItem li = new ListItem();
				li.Text = tbase.TaxonomyName;
				
				
				if(listNumber == 1)
				{
					if(Session["AllBraisingItems1"] != null) {
						 foreach(ListItem item in (ListItemCollection)Session["AllBraisingItems1"])
						 {
							 if(li.Text == item.Value) {
								if(item.Selected)
								{
									li.Selected = true;
								} 
							} 
						 } 
					 } 
				 }
				else if(listNumber == 2)
				{
					if(Session["AllBraisingItems2"] != null) {
						 foreach(ListItem item in (ListItemCollection)Session["AllBraisingItems2"])
						 {
							 if(li.Text == item.Value) {
								if(item.Selected)
								{
									li.Selected = true;
								} 
							} 
						 } 
					 } 
				 }
				else if(listNumber == 3)
				{
					if(Session["AllBraisingItems3"] != null) {
						 foreach(ListItem item in (ListItemCollection)Session["AllBraisingItems3"])
						 {
							 if(li.Text == item.Value) {
								if(item.Selected)
								{
									li.Selected = true;
								} 
							} 
						 } 
					 } 
				 }
				 
				 cbList.Items.Add(li);
			}
			
			Filter newFilter = new Filter(tbase.TaxonomyName, tbase.TaxonomyPath);
			
			if(listNumber == 1)
			{
				FilterOptions1.Add(newFilter);
			}
			else if(listNumber == 2)
			{
				FilterOptions2.Add(newFilter);
			}
			else
			{
				FilterOptions3.Add(newFilter);
			}
        }
    }
}

public class Series {
	private string _Name; 
	private string _Path;
	private string _ID; 
	
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
	
	public string ID
    {
        set { this._ID = value; }
        get { return this._ID; }
    }
	
	public Series(){
	}
	
	public Series(string sName, string sPath, string sID){
		_Name = sName;
		_Path = sPath;
		_ID = sID;
	}
}

public class Filter {
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
	
	public Filter(){
	}
	
	public Filter(string sName, string sPath){
		_Name = sName;
		_Path = sPath;
	}
}
