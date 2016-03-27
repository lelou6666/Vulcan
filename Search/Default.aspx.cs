using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.WebSearch.SearchData;
using Ektron.Cms.API.Search;

public partial class _Default : System.Web.UI.Page
{
	int count = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
		PerformSearch(0);
		PerformSearch(82);
		PerformSearch(91);
		PerformSearch(85);
		PerformSearch(86);
		PerformSearch(80);
		PerformSearch(81);
		PerformSearch(83);
		PerformSearch(120);
		PerformSearch(116);
		PerformSearch(110);
		PerformSearch(111);
		PerformSearch(113);
		PerformSearch(114);
		PerformSearch(117);
		PerformSearch(115);
		PerformSearch(112);
		
		string searchText = Request.QueryString["searchText"];
		litCount.Text += "Other Result Count: " + count.ToString() + " for " + searchText + "<br/>";
		
		Ektron.Cms.API.Search.SearchManager smanager = new Ektron.Cms.API.Search.SearchManager();
        Ektron.Cms.WebSearch.SearchData.SearchResponseData[] srd;
        Ektron.Cms.WebSearch.SearchData.SearchRequestData requestd = new Ektron.Cms.WebSearch.SearchData.SearchRequestData();

        int resultcount = 1;
        requestd.SearchFor = Ektron.Cms.WebSearch.SearchDocumentType.html;
        requestd.SearchType = WebSearchType.none;
        requestd.SearchText = searchText;
        requestd.FolderID = 84;
        requestd.PageSize = 100;
        requestd.Recursive = false;

        srd = smanager.Search(requestd, HttpContext.Current, ref resultcount);
		litProdCount.Text += "Product Result Count: " + srd.Length + " for " + searchText + "<br/>";
        
        for (int i = 0; i < srd.Length; i++)
        {
            litProdResults.Text += "<a href='" + GetAlias(srd[i].ContentID) + "' />" + srd[i].Title.ToString() + "<br/>";
			
					//Summary
        }
    }
	
	protected void PerformSearch(int folderId)
    {
		string searchText = Request.QueryString["searchText"];
		Ektron.Cms.API.Search.SearchManager smanager = new Ektron.Cms.API.Search.SearchManager();
        Ektron.Cms.WebSearch.SearchData.SearchResponseData[] srd;
        Ektron.Cms.WebSearch.SearchData.SearchRequestData requestd = new Ektron.Cms.WebSearch.SearchData.SearchRequestData();

        int resultcount = 1;
        requestd.SearchFor = Ektron.Cms.WebSearch.SearchDocumentType.html;
        requestd.SearchType = WebSearchType.none;
        requestd.SearchText = searchText;
        requestd.FolderID = folderId;
		//requestd.MultipleFolders = "0,84,82,91,85,86,80,81,83,120,116,110,111,113,114,117,115,112";
        requestd.PageSize = 100;
        requestd.Recursive = false;

        srd = smanager.Search(requestd, HttpContext.Current, ref resultcount);
		count += srd.Length;
        
        for (int i = 0; i < srd.Length; i++)
        {
            litResults.Text += "<p><a href='" + GetAlias(srd[i].ContentID) + "'>" + srd[i].Title.ToString() + "</a><br />";
			litResults.Text += "" + srd[i].Summary.ToString() + "</p>";
			
					//Summary
        }
	}
	
	public string GetAlias(long id)
    {
        Ektron.Cms.UrlAliasing.UrlAliasCommonApi uapi =  new Ektron.Cms.UrlAliasing.UrlAliasCommonApi();
        string alias = (uapi.GetAliasForContent(id)).Trim().ToLower();
       

        if (!string.IsNullOrEmpty(alias)) return "/" + alias.Trim();

        //if (string.IsNullOrEmpty(data.QuickLink)) return "empty/";

        //return data.QuickLink;
        return "/empty/";
    }
}