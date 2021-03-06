﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.API;
using Ektron.Cms.WebSearch;
using Ektron.Cms.WebSearch.SearchData;
using Ektron.Cms;
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;

public partial class Foodservice_usercontrols_SiteSearchControl : System.Web.UI.UserControl
{
    #region default text
    string _ofword = "of";
    string _forword = "for";
    string _secondsword = "Seconds";

    string _opt_siteAll = "Site";
    string _opt_html = "HTML";
    string _opt_documents = "Documents";
    string _opt_images = "Images";
    string _opt_multimedia = "MultiMedia";
    string _opt_discussionforum = "Forums";
    string _opt_tags = "Tags";
    string _opt_pagebuilder = "pages";

    string _opt_filter_createdbefore = "Created Before";
    string _opt_filter_createdafter = "Created After";
    string _opt_filter_modifiedbefore = "Modified Before";
    string _opt_filter_modifiedafter = "Modified after";
    string _opt_filter_author = "Author";
    string _opt_filter_filesize = "File Size";

    string _noresults =
    @"<p><b>Your search did not match any documents.</b></p>
    <p>Suggestions:</p>
    <ul>
        <li>Make sure all words are spelled correctly.</li>
        <li>Try different keywords.</li>
        <li>Try more general keywords.</li>
    </ul>";

    string _noresultstitle = "No Results";

    string _searchbuttontext = "Search";

    string _loadinglabel = "Loading...";

    string _basicsearchtab = "Basic Search";

    string _advancedsearchTab = "Advanced Search";

    string _findresultsby = "Find Results By";

    string _withallthewords = "with <B>all</B> of the words";

    string _advancedsearchexactphrase = "advanced search <B>exact phrase</B>";

    string _atleastoneofthewords = "<B>at least one</B> of the words";

    string _withouttheword = "<B>without</B> the word";

    string _filterresults = "Filter Results";

    string _remove = "Remove";

    string _addfilter = "Add Filter";

    string _results = "Results";
    #endregion

    #region properties

    private long _localizationSmartFormId = 0;
    public long LocalizationSmartFormId
    {
        get
        {
            return _localizationSmartFormId;
        }
        set
        {
            _localizationSmartFormId = value;
        }
    }


    /// Folder ID to search on
    /// </summary>
    public long FolderId { get; set; }

    /// <summary>
    /// Folder IDs to search on
    /// </summary>
    public string MultipleProductFolders { get; set; }
    
    public string MultipleOtherFolders { get; set; }

    public WebSearchOrder OrderBy { get; set; }

    public WebSearchOrderByDirection OrderDirection { get; set; }

    private string _searchQueryString = "searchtext";


    private int _ResultsPageSize = 20;
    /// <summary>
    /// Page Size
    /// </summary>
    public int ResultsPageSize
    {
        get
        {
            return _ResultsPageSize;
        }
        set
        {
            _ResultsPageSize = value;
        }
    }

    bool _enableAdvancedLink = true;
    public bool EnableAdvancedLink
    {
        get
        {
            return _enableAdvancedLink;
        }
        set
        {
            _enableAdvancedLink = value;
        }
    }

    bool _enableScopeDropDown = true;
    public bool EnableScopeDropDown
    {
        get
        {
            return _enableScopeDropDown;
        }
        set
        {
            _enableScopeDropDown = value;
        }
    }

    bool _EnableAllTabs = true;
    public bool EnableAllTabs
    {
        get
        {
            return _EnableAllTabs;
        }
        set
        {
            _EnableAllTabs = value;
        }
    }

    bool _EnableResultsCount = true;
    public bool EnableResultsCount
    {
        get
        {
            return _EnableResultsCount;
        }
        set
        {
            _EnableResultsCount = value;
        }
    }


    private int _MaxResults = 1000;
    /// <summary>
    /// Page Size
    /// </summary>
    public int MaxResults
    {
        get
        {
            return _MaxResults;
        }
        set
        {
            _MaxResults = value;
        }
    }

    /// <summary>
    /// Language Id, 0 means to use current user's sessions language
    /// </summary>
    int _languageId = 0;
    public int LanguageId
    {
        get
        {
            return _languageId;
        }
        set
        {
            _languageId = value;
        }
    }


    bool _Recursive = false;
    public bool Recursive
    {
        get
        {
            return _Recursive;
        }
        set
        {
            _Recursive = value;
        }
    }

    bool _showCustomSummary = false;
    public bool ShowCustomSummary
    {
        get
        {
            return _showCustomSummary;
        }
        set
        {
            _showCustomSummary = value;
        }
    }
    private int _cacheinterval = 60;
    public int CacheInterval
    {
        get
        {
            return _cacheinterval;
        }
        set
        {
            _cacheinterval = value;
        }
    }
    /// <summary>
    /// Search Query string to use when called from another page
    /// </summary>
    public string DynamicParameter
    {
        get
        {
            return _searchQueryString;
        }
        set
        {
            _searchQueryString = value;
        }
    }
    #endregion

    SearchResponseData[] data = null;
    SearchResponseData[] data2 = null;

    // user api
    Ektron.Cms.UserAPI _uapi = new Ektron.Cms.UserAPI();

    // posted advanced filters
    List<AdvancedFilter> _advancedfilters = null;

    protected void Page_Init(object sender, EventArgs e)
    {
        // register css and js files
        RegisterFiles();

        if (!IsPostBack)
        {
            this.UxSearchTab.Value = "1";
            this.divnoresults.Visible = false;
            this.resultsheader.Visible = false;
            this.divnoresults.Visible = false;
            this.searchresults.Visible = false;
        }

    }

    protected void Page_load(object sender, EventArgs e)
    {
        // retrieve posted advanced filters (dynamic html)
        this.RetrievePostedAdvancedFilters();

        // Show advanced options or not 
        this.liAdvLink.Visible = this.EnableAdvancedLink;

        // Show Scope drop down
        this.ddlSearchScope.Visible = this.EnableScopeDropDown;

        // hide or show all tabs at all
        this.uxTabs.Visible = this.EnableAllTabs;

        // load all locale legends 
        this.LocalizeControl();

        // Set all localizable legends on controls
        SetAllLegends();

        this.DataPager1.PageSize = this.ResultsPageSize;

        this.SearchAndRenderData();
    }

    /// <summary>
    /// Perform Search and potencially render all data
    /// </summary>
    private void SearchAndRenderData()
    {
        string searchtext = GetSearchText();

        DateTime start = DateTime.Now;
        PerformSearch(searchtext);
        DateTime stop = DateTime.Now;

		if ((data == null || data.Length == 0) && (data2 == null || data2.Length == 0))
        {
            // Show no results div
            this.divnoresults.Visible = true;
            this.searchresults.Visible = false;
            this.h3NoResults.InnerHtml = this._noresultstitle;
            this.h3NoResults2.Visible = false;
        }
        else
        {
        // Show results 
			this.divnoresults.Visible = false;
			this.searchresults.Visible = true;
        }
        
        
        if (!(data == null || data.Length == 0))
        {
            this.UXListView.DataSource = this.data;
            this.UXListView.DataBind();

            int first = (this.DataPager1.StartRowIndex) + 1;
            int last = first + this.ResultsPageSize - 1;
            if (last > data.Length) last = data.Length;

            TimeSpan ts = stop.Subtract(start);

            this.h3NoResults.InnerHtml =
                string.Format("{0} {1} - {2} {3} {4} {5} {6}",
                "Product results",
                first,
                last,
                _ofword,
                data.Length,
                _forword,
                searchtext);

            this.resultsheader.Visible = this.EnableResultsCount;
        }
        
        if (!(data2 == null || data2.Length == 0))
        {
            this.UXListView2.DataSource = this.data2;
            this.UXListView2.DataBind();

            int first2 = (this.DataPager2.StartRowIndex) + 1;
            int last2 = first2 + this.ResultsPageSize - 1;
            if (last2 > data2.Length) last2 = data2.Length;

            TimeSpan ts2 = stop.Subtract(start);

            this.h3NoResults2.InnerHtml =
                string.Format("{0} {1} - {2} {3} {4} {5} {6}",
                "Other results",
                first2,
                last2,
                _ofword,
                data2.Length,
                _forword,
                searchtext);

            this.resultsheader.Visible = this.EnableResultsCount;

        }
    }

    protected void PagerPrePrender(object sender, EventArgs e)
    {

    }
    /// <summary>
    /// handle prerender event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        this.SearchAndRenderData();
    }


    // register css and js files
    private void RegisterFiles()
    {
        string basepath = this.AppRelativeTemplateSourceDirectory.Replace("~", "");

        // register regulat css files for search control
        string filename = basepath + "css/search.css";
        Css.RegisterCss(this.Page, filename, "search.css");

        filename = basepath + "css/custom.css";
        Css.RegisterCss(this.Page, filename, "custom.css");

        // register jquery
        JS.RegisterJS(this.Page, JS.ManagedScript.EktronJS);
    }

    /// <summary>
    /// Basic Search button click event handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearchBasic_click(object sender, EventArgs e)
    {
        this.DataPager1.SetPageProperties(0, this.ResultsPageSize, true);
    }

    /// <summary>
    /// Handle adv. Search Button click event handler, builds a list of conditions in a string and 
    /// puts the text into the basic search textbox, which in turn will be used for searching
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearchAdvanced_Click(object sender, EventArgs e)
    {
        string allands = string.Empty;
        string[] allors = null;
        string not = this.ecm_eq.Value.Trim();

        // build and clause 
        if (!string.IsNullOrEmpty(this.ecm_q.Value))
        {
            string val = this.ecm_q.Value.Trim();
            while (val.Contains("  ")) val = val.Replace("  ", " ");
            allands = val.Replace(" ", " AND ");
        }
        // exact phrase - extend and clause
        if (!string.IsNullOrEmpty(this.ecm_epq.Value))
        {
            if (!string.IsNullOrEmpty(this.ecm_q.Value)) allands += " AND ";
            allands += string.Format("\"{0}\"", this.ecm_epq.Value.Trim());
        }
        // at least one of the words - build or clauses
        if (!string.IsNullOrEmpty(this.ecm_oq.Value))
        {
            string val = this.ecm_oq.Value.Trim();
            while (val.Contains("  ")) val = val.Replace("  ", " ");
            allors = val.Split(' ');
        }

        StringBuilder sb = new StringBuilder();

        // if there are no or conditions, just ands
        if (allors == null)
        {
            sb.Append(allands);
        }
        else
        {
            // add all or conditions
            foreach (string or in allors)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" OR ");
                }
                sb.Append("(" + or);
                if (!string.IsNullOrEmpty(allands))
                {
                    sb.Append(" AND " + allands);
                }
                sb.Append(")");
            }
        }

        // add not clause
        if (!string.IsNullOrEmpty(not))
        {
            if (sb.Length > 0)
            {
                sb.Append(" AND ");
            }
            sb.Append(" NOT " + not);
        }

        // add all advanced filter clauses
        if (this._advancedfilters != null)
        {
            foreach (AdvancedFilter af in this._advancedfilters)
            {
                if (af.FilterValue.Trim() != string.Empty)
                {
                    if (sb.Length > 0) sb.Append(" AND ");
                    sb.Append(string.Format("{0} {1} {2}", af.FilterType, af.Operator, af.FilterValue));
                }
            }
        }

        // finally, load the basic search text box
        this.ecmBasicKeywords.Value = sb.ToString();

        // search and render data again
        this.SearchAndRenderData();

        // go back to basic search tab
        this.UxSearchTab.Value = "1";
    }

    /// <summary>
    /// Set all localizable legends on controls
    /// </summary>
    private void SetAllLegends()
    {

        this.divnoresults.InnerHtml = this._noresults;
        //this.btnSearchAdvanced.Text =
                //btnSearchBasic.Text = this._searchbuttontext;

        this.divLoadinglabel.InnerHtml = this._loadinglabel;

        this.litBasicTab.Text = this._basicsearchtab;
        this.litAdvTab.Text = this._advancedsearchTab;

        this.legFindBy.InnerHtml = this._findresultsby;
        this.litwithallthewords.Text = this._withallthewords;
        this.litadvsearchexactphrase.Text = this._advancedsearchexactphrase;
        this.litatleastoneofthewords.Text = this._atleastoneofthewords;
        this.litwithouttheword.Text = this._withouttheword;
        this.legFilterResults.InnerHtml = this._filterresults;
        this.btnAddFilter.Value = this._addfilter;

        this.ddlSearchScope.Items[0].Text = _opt_siteAll;
        this.ddlSearchScope.Items[1].Text = _opt_html;
        this.ddlSearchScope.Items[2].Text = _opt_documents;
        this.ddlSearchScope.Items[3].Text = _opt_images;
        this.ddlSearchScope.Items[4].Text = _opt_multimedia;
        this.ddlSearchScope.Items[5].Text = _opt_discussionforum;
        this.ddlSearchScope.Items[6].Text = _opt_tags;
        this.ddlSearchScope.Items[7].Text = _opt_pagebuilder;

        this.BuildAdvancedFilters();

    }

    /// <summary>
    /// Builds the advanced filters with localization and depending on the posted amount of filters
    /// </summary>
    private void BuildAdvancedFilters()
    {
        string lioption = @"
                    <li id=""li_Meta_00"" [style]>
                        <select id=""select_meta_00"" name=""select_meta_00"" onchange=""AdvOptionDefault(this)"">
                            <option value=""@datecreatedB"">{0}</option>
                            <option value=""@datecreatedA"">{1}</option>
                            <option value=""@datemodifiedB"">{2}</option>
                            <option value=""@datemodifiedA"">{3}</option>
                            <option value=""@docauthor"">{4}</option>
                            <option value=""@cmssize"">{5}</option>
                        </select>
                        <input id=""ecm_meta_00"" value="""" type=""text"" name=""ecm_meta_00""/>
                        <a title=""{6}"" href=""#"" onclick=""return RemoveFilter(this)"">Remove
                        </a>
                    </li>";

        lioption = string.Format(lioption,
                _opt_filter_createdbefore,
                _opt_filter_createdafter,
                _opt_filter_modifiedbefore,
                _opt_filter_modifiedafter,
                _opt_filter_author,
                _opt_filter_filesize,
                _remove);

        StringBuilder sb = new StringBuilder();
        // add the base copy row
        sb.Append(lioption.Replace("[style]", "style=\"display:none\""));
        int filtercount = 0;

        if (!IsPostBack)
        {
            // on intial page just add one row for editing
            sb.Append(lioption.Replace("[style]", string.Empty).Replace("_00", "_01"));
        }
        else
        {
            if (this._advancedfilters != null)
            {

                // for each of the advanced posted filters, build a new one in dynamic sql
                foreach (AdvancedFilter af in _advancedfilters)
                {
                    filtercount++;

                    // set the value in the select (input)
                    string newli = lioption
                                    .Replace("[style]", string.Empty)
                                    .Replace("id=\"ecm_meta_00\" value=\"", "id=\"ecm_meta_00\" value=\"" + af.FilterValue);

                    // set the filter type (select)
                    string selectkey = string.Format(" value=\"{0}\"", af.FilterType);
                    newli = newli.Replace(selectkey, " selected" + selectkey);

                    // replace counter
                    sb.Append(newli.Replace("_00", "_" + filtercount.ToString("00")));
                }
            }
        }
        this.litAdvOptions.Text = sb.ToString();
    }


    /// <summary>
    /// return list of posted avd. filters
    /// </summary>
    /// <returns></returns>
    void RetrievePostedAdvancedFilters()
    {
        if (this._advancedfilters != null || !IsPostBack) return;

        this._advancedfilters = new List<AdvancedFilter>();

        for (int i = 1; i <= 100; i++)
        {
            string selectkey = string.Format("select_meta_{0}", i.ToString("00"));
            string inputkey = string.Format("ecm_meta_{0}", i.ToString("00"));
            string selectvalue = this.Page.Request.Form[selectkey] as string;
            string inputvalue = this.Page.Request.Form[inputkey] as string;
            if (selectvalue != null && inputvalue != null)
            {
                _advancedfilters.Add(new AdvancedFilter() { FilterType = selectvalue, FilterValue = inputvalue });
            }
        }

    }
    /// <summary>
    /// Load localization control from smartform
    /// </summary>
    /// <returns></returns>
    private void LocalizeControl()
    {
        if (this.LocalizationSmartFormId <= 0) return;

        Ektron.Cms.API.Content.Content capi = new Ektron.Cms.API.Content.Content();
        capi.ContentLanguage = this.CurrentLanguage;
        ContentData cd = capi.GetContent(this.LocalizationSmartFormId);
        if (cd != null)
        {
            XmlDocument xdoc = null;
            try
            {
                xdoc = new XmlDocument();
                xdoc.LoadXml(cd.Html);
            }
            catch { }

            if (xdoc != null && xdoc.InnerXml != string.Empty)
            {
                // search button
                XmlNode xnode = xdoc.SelectSingleNode("/root/SearchButton");
                if (xnode != null)
                {
                    _searchbuttontext = xnode.InnerText;
                }

                // loading
                xnode = xdoc.SelectSingleNode("/root/Loading");
                if (xnode != null)
                {
                    _loadinglabel = _basicsearchtab = xnode.InnerText;
                }

                // no results
                xnode = xdoc.SelectSingleNode("/root/NoResultsTitle");
                if (xnode != null)
                {
                    _noresultstitle = xnode.InnerText;
                }

                // no results suggestions
                xnode = xdoc.SelectSingleNode("/root/NoResultsSuggestions");
                if (xnode != null)
                {
                    _noresults = xnode.InnerText;
                }

                // results legend
                xnode = xdoc.SelectSingleNode("/root/Results");
                if (xnode != null)
                {
                    _results = xnode.InnerText;
                }

                // for word
                xnode = xdoc.SelectSingleNode("/root/forword");
                if (xnode != null)
                {
                    _forword = xnode.InnerText;
                }

                // of word
                xnode = xdoc.SelectSingleNode("/root/ofword");
                if (xnode != null)
                {
                    _ofword = xnode.InnerText;
                }

                // seconds word
                xnode = xdoc.SelectSingleNode("/root/Seconds");
                if (xnode != null)
                {
                    _secondsword = xnode.InnerText;
                }

                // BasicSearchTitle
                xnode = xdoc.SelectSingleNode("/root/BasicSearchTitle");
                if (xnode != null)
                {
                    _basicsearchtab = xnode.InnerText;
                }

                // Site all option
                xnode = xdoc.SelectSingleNode("/root/BasicOptionScopeAll");
                if (xnode != null)
                {
                    _opt_siteAll = xnode.InnerText;
                }

                // html option
                xnode = xdoc.SelectSingleNode("/root/BasicOptionScopeHtml");
                if (xnode != null)
                {
                    _opt_html = xnode.InnerText;
                }

                // html option
                xnode = xdoc.SelectSingleNode("/root/BasicOptionScopeHtml");
                if (xnode != null)
                {
                    _opt_html = xnode.InnerText;
                }

                // documents option
                xnode = xdoc.SelectSingleNode("/root/BasicOptionScopeDocuments");
                if (xnode != null)
                {
                    _opt_documents = xnode.InnerText;
                }

                // images option
                xnode = xdoc.SelectSingleNode("/root/BasicOptionScopeImages");
                if (xnode != null)
                {
                    _opt_images = xnode.InnerText;
                }

                // multimedia option
                xnode = xdoc.SelectSingleNode("/root/BasicOptionScopeMultimedia");
                if (xnode != null)
                {
                    _opt_multimedia = xnode.InnerText;
                }

                // forums option
                xnode = xdoc.SelectSingleNode("/root/BasicOptionScopeForums");
                if (xnode != null)
                {
                    _opt_discussionforum = xnode.InnerText;
                }

                // tags option
                xnode = xdoc.SelectSingleNode("/root/BasicOptionScopeTags");
                if (xnode != null)
                {
                    _opt_tags = xnode.InnerText;
                }

                // pagebuilder option
                xnode = xdoc.SelectSingleNode("/root/BasicOptionScopePageBuilder");
                if (xnode != null)
                {
                    _opt_pagebuilder = xnode.InnerText;
                }

                // adv. search tab
                xnode = xdoc.SelectSingleNode("/root/AdvancedSearchTitle");
                if (xnode != null)
                {
                    _advancedsearchTab = xnode.InnerText;
                }

                // find results by
                xnode = xdoc.SelectSingleNode("/root/FindResultsBy");
                if (xnode != null)
                {
                    _findresultsby = xnode.InnerText;
                }

                // with all words
                xnode = xdoc.SelectSingleNode("/root/WithAllWords");
                if (xnode != null)
                {
                    _withallthewords = xnode.InnerText;
                }

                // exact phrase
                xnode = xdoc.SelectSingleNode("/root/ExactPhrase");
                if (xnode != null)
                {
                    _advancedsearchexactphrase = xnode.InnerText;
                }
                // at least one
                xnode = xdoc.SelectSingleNode("/root/AtLeastOne");
                if (xnode != null)
                {
                    _atleastoneofthewords = xnode.InnerText;
                }

                // without the word
                xnode = xdoc.SelectSingleNode("/root/WithoutWord");
                if (xnode != null)
                {
                    _withouttheword = xnode.InnerText;
                }

                // filter results
                xnode = xdoc.SelectSingleNode("/root/FilterResults");
                if (xnode != null)
                {
                    _filterresults = xnode.InnerText;
                }

                // remove filter
                xnode = xdoc.SelectSingleNode("/root/RemoveFilter");
                if (xnode != null)
                {
                    _remove = xnode.InnerText;
                }

                // add filter button
                xnode = xdoc.SelectSingleNode("/root/AddFilterButton");
                if (xnode != null)
                {
                    _addfilter = xnode.InnerText;
                }

                // adv. option created before
                xnode = xdoc.SelectSingleNode("/root/AdvancedOptionCreatedBefore");
                if (xnode != null)
                {
                    _opt_filter_createdbefore = xnode.InnerText;
                }

                // adv. option created after
                xnode = xdoc.SelectSingleNode("/root/AdvancedOptionCreatedAfter");
                if (xnode != null)
                {
                    _opt_filter_createdafter = xnode.InnerText;
                }

                // adv. option modified before
                xnode = xdoc.SelectSingleNode("/root/AdvancedOptionModifiedBefore");
                if (xnode != null)
                {
                    _opt_filter_modifiedbefore = xnode.InnerText;
                }

                // adv. option author
                xnode = xdoc.SelectSingleNode("/root/AdvancedOptionAuthor");
                if (xnode != null)
                {
                    _opt_filter_author = xnode.InnerText;
                }

                // adv. option file size
                xnode = xdoc.SelectSingleNode("/root/AdvancedOptionFileSize");
                if (xnode != null)
                {
                    _opt_filter_filesize = xnode.InnerText;
                }

                // adv. option file size
                xnode = xdoc.SelectSingleNode("/root/AdvancedOptionModifiedAfter");
                if (xnode != null)
                {
                    _opt_filter_modifiedafter = xnode.InnerText;
                }
            }

        }

    }


    /// <summary>
    /// returns the language assumed to be active, either the user's or the controls set language
    /// </summary>
    private int CurrentLanguage
    {
        get
        {
            return (this.LanguageId == 0 ? _uapi.ContentLanguage : this.LanguageId);
        }
    }

    /// <summary>
    /// returns a cache key based on type of search
    /// </summary>
    private string GetCacheKey
    {
        get
        {
            return string.Format("{0}_{1}", this.UxSearchTab.Value, this.GetSearchText());
        }
    }

    /// <summary>
    /// Perform a search with parameters
    /// </summary>
    /// <param name="searchtext"></param>
    private void PerformSearch(string searchtext)
    {
        if (searchtext == string.Empty) return;

        string cachekey = this.GetCacheKey;

        if (HttpContext.Current.Cache[cachekey] != null)
        {
            //this.data = HttpContext.Current.Cache[cachekey] as SearchResponseData[];
        }

        var terms = searchtext.Split((new char[] { ' ' }), StringSplitOptions.RemoveEmptyEntries);
        if (terms != null)
        {
            searchtext = string.Empty;

            for (int i = 0; i < terms.Length; i++)// (string t in terms)
            {
                searchtext += "@DocTitle " + terms[i];
                if ((i + 1) < terms.Length)
                {
                    searchtext += " and ";
                }
            }
        }

        Ektron.Cms.API.Search.SearchManager searchManager =
            new Ektron.Cms.API.Search.SearchManager();
        Ektron.Cms.WebSearch.SearchData.SearchRequestData searchData =
            new Ektron.Cms.WebSearch.SearchData.SearchRequestData();

        int resultCount = 0;
        searchData.EnablePaging = false;
        searchData.SearchFor =
            (SearchDocumentType)
            Enum.Parse(typeof(SearchDocumentType), this.ddlSearchScope.SelectedValue);

        searchData.SearchType = WebSearchType.none;
        // searchData.IsUserBasicSearch = true;

        //searchData.SearchFor = SearchDocumentType.html;
        searchData.OrderDirection = Ektron.Cms.WebSearch.SearchData.WebSearchOrderByDirection.Descending;
        searchData.MaxResults = this.MaxResults;
        searchData.LanguageID = CurrentLanguage;
        searchData.Recursive = true;
        searchData.FolderID = this.FolderId;
        searchData.MultipleFolders = this.MultipleProductFolders;
        searchData.SearchText = searchtext;
        searchData.OrderBy = this.OrderBy;
        searchData.OrderDirection = this.OrderDirection;
        data = searchManager.Search(searchData, HttpContext.Current, ref resultCount);

        // adjust the summary, so that it doesn't have html tags nor it is too long
        if (data != null && this.ShowCustomSummary)
        {
            foreach (SearchResponseData srd in data)
            {
                srd.Summary = StripHtml(srd.Summary);
                if (srd.Summary.Length > 200)
                {
                    srd.Summary = srd.Summary.Substring(0, 200) + "...";
                }
            }
        }

        // cache data back in
        //HttpContext.Current.Cache[cachekey] = this.data;
        
        
        //Search Other
        searchData.MultipleFolders = this.MultipleOtherFolders;
        data2 = searchManager.Search(searchData, HttpContext.Current, ref resultCount);
        if (data2 != null && this.ShowCustomSummary)
        {
            foreach (SearchResponseData srd in data2)
            {
                srd.Summary = StripHtml(srd.Summary);
                if (srd.Summary.Length > 200)
                {
                    srd.Summary = srd.Summary.Substring(0, 200) + "...";
                }
            }
        }

        // cache data back in
        //HttpContext.Current.Cache[cachekey] = this.data2;
    }

    /// <summary>
    /// will strip out the html tags from any string
    /// </summary>
    /// <param name="html">html'ed text</param>
    /// <returns>stripped text  </returns>
    public string StripHtml(string html)
    {
        return Regex.Replace(html, @"<(.|\n)*?>", string.Empty).Trim();
    }

    /// <summary>
    /// Gets search text either from textbox or query string parameter
    /// </summary>
    /// <returns></returns>
    private string GetSearchText()
    {
        string res = ecmBasicKeywords.Value.Trim();
        if (res == string.Empty && this.Page.Request.QueryString[this.DynamicParameter] != null)
        {
            res = this.Page.Request.QueryString[this.DynamicParameter];
            this.ecmBasicKeywords.Value = res;
        }
        return res;
    }

    /// <summary>
    /// A private class that represents each selected adv. filter and its value
    /// </summary>
    private class AdvancedFilter
    {
        /// <summary>
        /// represents the filter type from select control 
        /// (@datecreatedB, @datecreatedA, @datemodifiedB, @datemodifiedA, @docauthor, @cmssize)
        /// 
        /// Sample text that needs to be generated for operands and variables
        /// AND ( @datecreated < 1 )  
        /// AND ( @datecreated > 2 ) 
        /// AND ( @datecreated > 3 )  
        /// AND ( @datemodified < 4 )  
        /// AND ( @docauthor 5 )  
        /// AND ( @cmssize > 6 ) 
        /// </summary>
        public string FilterType = string.Empty;

        public string FilterValue = string.Empty;

        public string Operator
        {
            get
            {
                string lastletter = this.FilterType.Substring(this.FilterType.Length - 1, 1);
                if (lastletter == "A" || this.FilterType.Contains("cmssize")) return ">";
                if (lastletter == "B") return "<";
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Use this class only during development, if your site isn't indexed yet
    /// </summary>
    /// <returns></returns>
    private SearchResponseData[] SimulateData()
    {
        List<SearchResponseData> srd = new List<SearchResponseData>();
        for (int i = 0; i < 100; i++)
        {
            SearchResponseData srdi = new SearchResponseData();
            srdi = new SearchResponseData();
            srdi.ContentID = i;
            srdi.ContentLanguage = 1033;
            srdi.FolderID = this.FolderId;
            srdi.QuickLink = "/";
            srdi.Summary = "This is just a summay";
            srdi.Title = "This is a title for content id=" + i.ToString();
            srd.Add(srdi);

        }
        return srd.ToArray();
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
