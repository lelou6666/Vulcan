﻿using System;
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

public partial class productlist : System.Web.UI.UserControl
{
	Ektron.Cms.API.Search.SearchManager searchapi = new Ektron.Cms.API.Search.SearchManager();
    SearchResponseData[] data = null; 
    SearchResponseData[] modelData = null;
	
	private List<Product> Products = new List<Product>();
	private List<Product> ProductsFinal = new List<Product>();
	private List<string> Filters1 = new List<string>();
	private List<string> Filters2 = new List<string>();
	private List<string> Filters3 = new List<string>();
	private List<string> Filters4 = new List<string>();
	private string taxonomyPath = "";
	private string productView = "";
	private bool hasContent = false;
	private int productCount = 0;
	private int count = 0;
	
	public string ProductView
    {
        get { return this.productView; }
        set { this.productView = value; }
    }
	
	public string TaxonomyPath
    {
        get { return this.taxonomyPath; }
        set { this.taxonomyPath = value; }
    }
    
    public bool HasContent
    {
        get { return this.hasContent; }
        set { this.hasContent = value; }
    }
	
	public int ProductCount
    {
        get { return this.productCount; }
        set { this.productCount = value; }
    }
    
    public List<string> filters1
    {
        get { return this.Filters1; }
        set { this.Filters1 = value; }
    }
    
    public List<string> filters2
    {
        get { return this.Filters2; }
        set { this.Filters2 = value; }
    }
    
    public List<string> filters3
    {
        get { return this.Filters3; }
        set { this.Filters3 = value; }
    }
    
    public List<string> filters4
    {
        get { return this.Filters4; }
        set { this.Filters4 = value; }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    
    public void loadContent()
    {
		Ektron.Cms.API.Content.Taxonomy TaxAPI = new Ektron.Cms.API.Content.Taxonomy();
		Ektron.Cms.TaxonomyRequest TaxRequest = new Ektron.Cms.TaxonomyRequest();
		Ektron.Cms.TaxonomyData TaxData = new Ektron.Cms.TaxonomyData();
					
		if(TaxonomyPath != "")
		{
			//Get All Products in a Series
			data = GetProducts(TaxonomyPath);

			if (data == null || data.Length < 1)
			{
				hasContent = false;
			}
			else
			{
				//For Each Product
				foreach (SearchResponseData productData in data)
				{
					bool passFilter1 = false;
					bool passFilter2 = false;
					bool passFilter3 = false;
					bool passFilter4 = false;
					
					if(Filters1.Count == 0)
					{
						passFilter1 = true;
					}
						
					if(Filters2.Count == 0)
					{
						passFilter2 = true;
					}
					
					if(Filters3.Count == 0)
					{
						passFilter3 = true;
					}
					if(Filters4.Count == 0)
					{
						passFilter4 = true;
					}
					
					//if no filters, show all products
					if(Filters1.Count == 0 && Filters2.Count == 0 && Filters3.Count == 0 && Filters4.Count == 0)
					{
						GetProductInformation(productData.ContentID, productData.Title);
						HasContent = true;
					}
					else
					{		
						//get all product's taxonomies
						Ektron.Cms.API.Content.Taxonomy taxonomyApi = new Ektron.Cms.API.Content.Taxonomy();
						TaxonomyBaseData[] taxBaseData = taxonomyApi.ReadAllAssignedCategory(productData.ContentID);
						
						if (taxBaseData.Length > 0)
						{
							//For Each Product Taxonomy
							for(int i = 0; i < taxBaseData.Length; i++)
							{
								//needed to get taxonomy path (by id)
								TaxRequest.TaxonomyId = taxBaseData[i].TaxonomyId;
								TaxRequest.TaxonomyLanguage = 1033;
								TaxRequest.Depth = -1;
								TaxData = TaxAPI.LoadTaxonomy(ref TaxRequest);
						
								//For Each Filter
								foreach(string path in Filters1)
								{
									if(TaxData.TaxonomyPath == path)
									{
										passFilter1 = true;
									}
								}
								
								//For Each Filter
								foreach(string path in Filters2)
								{
									if(TaxData.TaxonomyPath == path)
									{
										passFilter2 = true;
									}
								}
								
								foreach(string path in Filters3)
								{
									if(TaxData.TaxonomyPath == path)
									{
										passFilter3 = true;
									}
								}
								
								foreach(string path in Filters4)
								{
									if(TaxData.TaxonomyPath == path)
									{
										passFilter4 = true;
									}
								}
							}
							
							if(passFilter1 && passFilter2 && passFilter3 && passFilter4)
							{
								GetProductInformation(productData.ContentID, productData.Title);
								HasContent = true;
							}
						}
					}
				}
				
				ProductCount = Products.Count;
				Products.Sort((s1, s2) => s1.Order.CompareTo(s2.Order));
				
				if(ProductView == "list") {
					this.UXListView.DataSource = Products;
					this.UXListView.DataBind();
				}
				else
				{
					this.UXGridView.DataSource = Products;
					this.UXGridView.DataBind();
				}
			}
		}
    }
    
    //This method gets all of the Products (ex. 24 in. Range, Standard Oven) for a given Series (ex. Endurance Range Line)
    private SearchResponseData[] GetProducts(string taxonomysearchtext)
    {
        var searchData = new Ektron.Cms.WebSearch.SearchData.SearchRequestData();
        int resultCount = 0;

        searchData.EnablePaging = false;
        searchData.SearchFor = Ektron.Cms.WebSearch.SearchDocumentType.html;
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
    
    //This method get detailed product information
    public void GetProductInformation(long ContentId, string Title){
		System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
		string TaxonomyID = "";
		string Image = "";
		int Order = 0;
		count += 1;
		
		Ektron.Cms.UrlAliasing.UrlAliasCommonApi uapi = new Ektron.Cms.UrlAliasing.UrlAliasCommonApi();
        string Alias = this.ResolveUrl("~") + (uapi.GetAliasForContent(ContentId)).Trim();
		
		var capi = new Ektron.Cms.Framework.Core.Content.Content(Ektron.Cms.Framework.ApiAccessMode.Admin);
        var content = capi.GetItem(ContentId);

        if (content == null) 
			return;

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
        System.Xml.XmlNode node = doc.SelectSingleNode("/root/tax/node()");
        if (node != null)
        {
            TaxonomyID = node.InnerText;
        }
        
        node = doc.SelectSingleNode("/root/thumbnail/node()");
        if (node != null)
        {
            Image = node.InnerText;
            
            if(Image.Trim() == "")
            {
				Image = "/images/no_photo.jpg";
            }
        }
        else
        {
			Image = "/images/no_photo.jpg";
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
        
		Product newProduct = new Product(ContentId, Title, TaxonomyID, Image, Alias, Order);
		Products.Add(newProduct);
    }
}

public class Product {
	private long _ID; 
	private string _Title; 
	private string _TaxonomyAssociation;
	private string _Image;
	private string _Alias;
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
	
	public string TaxonomyAssociation
    {
        set { this._TaxonomyAssociation = value; }
        get { return this._TaxonomyAssociation; }
    }
    
    public string Image
    {
        set { this._Image = value; }
        get { return this._Image; }
    }
    
    public string Alias
    {
        set { this._Alias = value; }
        get { return this._Alias; }
    }
	
	public int Order
    {
        set { this._Order = value; }
        get { return this._Order; }
    }
	
	public Product(){
	}
	
	public Product(long lID, string sTitle, string sTaxonomyAssociation, string sImage, string sAlias, int sOrder){
		
		_ID = lID;
		_Title = sTitle;
		_TaxonomyAssociation = sTaxonomyAssociation;
		_Image = sImage;
		_Alias = sAlias;
		_Order = sOrder;
	}
}