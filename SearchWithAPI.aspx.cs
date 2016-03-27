using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.WebSearch.SearchData;
using Ektron.Cms.API.Search;

public partial class SearchWithAPI : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        Ektron.Cms.API.Search.SearchManager smanager = new Ektron.Cms.API.Search.SearchManager();
        Ektron.Cms.WebSearch.SearchData.SearchResponseData[] srd;
        Ektron.Cms.WebSearch.SearchData.SearchRequestData requestd = new Ektron.Cms.WebSearch.SearchData.SearchRequestData();

        int resultcount = 1;
        requestd.SearchFor = Ektron.Cms.WebSearch.SearchDocumentType.all;
        requestd.SearchType = WebSearchType.none;
        requestd.SearchText = "steam";
        requestd.FolderID = 0;
        requestd.PageSize = 100;
        requestd.Recursive = true;

        srd = smanager.Search(requestd, HttpContext.Current, ref resultcount);

        litResultCount.Text += "Result Count: " + srd.Length.ToString() + "<br/>";
        for (int i = 0; i < srd.Length; i++)
        {
            litResults.Text += "Content ID = " + srd[i].ContentID.ToString() + " Title: " + srd[i].Title.ToString()+"<br/>";
        }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    }
}
