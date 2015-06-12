using System;
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

public partial class _Default : System.Web.UI.Page
{
	Ektron.Cms.API.Search.SearchManager searchapi = new Ektron.Cms.API.Search.SearchManager();
    SearchResponseData[] data = null;
	private List<Product> Products = new List<Product>();
	
    protected void Page_Load(object sender, EventArgs e)
    {
		//Fryers
		getData(125);
		this.UXListView.DataSource = Products;
		this.UXListView.DataBind();
		
		data = null;
		Products.Clear();
		
		//Heated Holding
		getData(203);
		this.ListView1.DataSource = Products;
		this.ListView1.DataBind();
		
		data = null;
		Products.Clear();
		
		//Griddles & Charbroilers
		getData(154);
		this.ListView2.DataSource = Products;
		this.ListView2.DataBind();
		
		data = null;
		Products.Clear();
		
		//Ovens
		getData(637);
		this.ListView3.DataSource = Products;
		this.ListView3.DataBind();
		
		data = null;
		Products.Clear();
		
		//Steamers
		getData(279);
		this.ListView4.DataSource = Products;
		this.ListView4.DataBind();
		
		data = null;
		Products.Clear();
    }
    
    private void getData(long TaxonomyID)
    {
		Ektron.Cms.API.Content.Taxonomy TaxAPI = new Ektron.Cms.API.Content.Taxonomy();
		Ektron.Cms.TaxonomyRequest TaxRequest = new Ektron.Cms.TaxonomyRequest();
		Ektron.Cms.TaxonomyData TaxData = new Ektron.Cms.TaxonomyData();
		
		TaxRequest.TaxonomyId = TaxonomyID;
		TaxRequest.TaxonomyLanguage = 1033;
		TaxRequest.Depth = -1;
		TaxData = TaxAPI.LoadTaxonomy(ref TaxRequest);

		string path = TaxData.TaxonomyPath;
		
		data = GetProducts(path);
		
		if (data == null || data.Length < 1)
		{
		
		}
		else
		{
			foreach (SearchResponseData productData in data)
			{
				GetProductInformation(productData.ContentID, productData.Title);
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
		
		Ektron.Cms.UrlAliasing.UrlAliasCommonApi uapi = new Ektron.Cms.UrlAliasing.UrlAliasCommonApi();
        string Alias = this.ResolveUrl("~") + (uapi.GetAliasForContent(ContentId)).Trim();
        
		Product newProduct = new Product(ContentId, Title, Alias);
		Products.Add(newProduct);
    }
}

public class Product {
	private long _ID; 
	private string _Title; 
	private string _Alias;
	
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
    
    public string Alias
    {
        set { this._Alias = value; }
        get { return this._Alias; }
    }
	
	public Product(){
	}
	
	public Product(long lID, string sTitle, string sAlias){
		
		_ID = lID;
		_Title = sTitle;
		_Alias = sAlias;
	}
}