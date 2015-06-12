using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;
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
using Vulcan.Product.Lib;

public partial class downloads : UserControl
{
	Ektron.Cms.API.Search.SearchManager searchapi = new Ektron.Cms.API.Search.SearchManager();
	public List<DropDownItem> SeriesList = new List<DropDownItem>();
	public List<DropDownItem> ProductList = new List<DropDownItem>();
	public List<DropDownItem> ModelList = new List<DropDownItem>();
	public string SpecSheets = "";
	public string SellSheet = "";
	public string Manual = "";
	public string PartsCatelog = "";
	public string ServiceManual = "";
	public string CADRevit = "";
	public string HighRes = "";
	
	
    protected void Page_Load(object sender, EventArgs e)
    {
		if(segment.SelectedValue == "")
		{
			series.Items.Clear(); 
			series.Items.Add(new ListItem("Sub-Category", ""));
			
			product.Items.Clear(); 
			product.Items.Add(new ListItem("Product", ""));
			
			model.Items.Clear(); 
			model.Items.Add(new ListItem("Model", ""));
			
			buttons.Visible = false;
		}
    }
	
	protected void updateSegment(object sender, EventArgs e)
    {
		specError.Text = "";
		
		if(segment.SelectedValue != "") 
		{
			//Get Series for Segment
			switch (segment.SelectedValue)
			{
				case "Fryers":
					LoadTaxonomyData(336);
					break;
				case "Ranges":
					LoadTaxonomyData(370);
					break;
				case "Ovens":
					LoadTaxonomyData(358);
					break;
				case "Griddles":
					LoadTaxonomyData(348);
					break;
				case "Charbroilers":
					LoadTaxonomyData(947);
					break;
				case "Steamers":
					LoadTaxonomyData(485);
					break;
				case "Kettles":
					LoadTaxonomyData(493);
					break;
				case "Braising Pans":
					LoadTaxonomyData(503);
					break;
				case "Heated Holding":
					LoadTaxonomyData(361);
					break;
				case "Combi":
					LoadTaxonomyData(802);
					break;
			}
		
			series.Items.Clear(); 
			series.Items.Add(new ListItem("Sub-Category", ""));
				
			foreach (DropDownItem seriesItem in SeriesList)
			{
				series.Items.Add(new ListItem(seriesItem.Name, seriesItem.Path));
			}
			
			product.Items.Clear(); 
			product.Items.Add(new ListItem("Product", ""));
			
			model.Items.Clear(); 
			model.Items.Add(new ListItem("Model", ""));
		}
		
		if(document.SelectedValue == "" || model.SelectedValue == "") {
			buttons.Visible = false;
		}
	}
	
	protected void updateSeries(object sender, EventArgs e)
    {
		specError.Text = "";
		
		if(series.SelectedValue != "") 
		{
			product.Items.Clear(); 
			product.Items.Add(new ListItem("Product", ""));
			
			//Get Products for Selected Series
			SearchResponseData[] data = null;
			data = GetSearchResults("", series.SelectedValue, 84);

			if (data == null || data.Length < 1)
			{
				// Show no results div
			}
			else
			{
				foreach (SearchResponseData productData in data)
				{
					product.Items.Add(new ListItem(HttpUtility.HtmlDecode(productData.Title), productData.ContentID.ToString()));
				}
			}
		}
		else {
			product.Items.Clear(); 
			product.Items.Add(new ListItem("Product", ""));
			model.Items.Clear(); 
			model.Items.Add(new ListItem("Model", ""));
			buttons.Visible = false;
		}
		
		if(document.SelectedValue == "" || model.SelectedValue == "") {
			buttons.Visible = false;
		}
	}
	
	protected void updateProduct(object sender, EventArgs e)
    {
		if(product.SelectedValue != "") 
		{
			model.Items.Clear(); 
			
			//get models
			SearchResponseData[] data = null;
	
			string modelId = "";
			string TaxonomyID = "";
			string TaxonomyPath = "";
			
			//Get Product Content Details
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			var capi = new Ektron.Cms.Framework.Core.Content.Content(Ektron.Cms.Framework.ApiAccessMode.Admin);
			var content = capi.GetItem(long.Parse(product.SelectedValue));
			
			//Get Product's Content, and grab Tax reference
			doc.LoadXml(content.Html);
			System.Xml.XmlNode node = doc.SelectSingleNode("/root/tax/node()");
			
			if (node != null)
			{
				TaxonomyID = node.InnerText;
			}
			
			//Get Taxonomy Path via TaxonomyID
			Taxonomy api = new Taxonomy();
			TaxonomyData tdata = new TaxonomyData();
			TaxonomyRequest treq = new TaxonomyRequest();
			treq.TaxonomyLanguage = 1033;
			treq.TaxonomyId = long.Parse(TaxonomyID);
			tdata = api.LoadTaxonomy(ref treq);
			TaxonomyPath = tdata.TaxonomyPath;
			
			//Get Models by Taxonomy Path
			data = GetSearchResults("", TaxonomyPath, 78);
			
			foreach (SearchResponseData modelData in data)
			{ 
				model.Items.Add(new ListItem(HttpUtility.HtmlDecode(modelData.Title), modelData.ContentID.ToString()));
			}
			
			SortDDL(ref this.model);
			
			model.Items.Insert(0,new ListItem("Model", ""));
		}
		else {
			model.Items.Clear(); 
			model.Items.Add(new ListItem("Model", ""));
			buttons.Visible = false;
		}
	}
	
	protected void updateModel(object sender, EventArgs e)
    {
		specError.Text = "";
		
		if(document.SelectedValue == "" || model.SelectedValue == "") {
			buttons.Visible = false;
		}
		else {
			buttons.Visible = true;
			
			if(document.SelectedValue == "CAD Revit") {
				download.Visible = false;
				view.Visible = true;
			}
			else if(document.SelectedValue == "Images") {
				view.Visible = false;
				download.Visible = true;
			}
			else {
				download.Visible = true;
				view.Visible = true;
			}
		}
	}
	
	protected void showButtons (object sender, EventArgs e)
    {
		if(document.SelectedValue == "" || model.SelectedValue == "") {
			buttons.Visible = false;
		}
		else {
			buttons.Visible = true;
			
			if(document.SelectedValue == "CAD Revit") {
				download.Visible = false;
				view.Visible = true;
			}
			else if(document.SelectedValue == "Images") {
				view.Visible = false;
				download.Visible = true;
			}
			else {
				download.Visible = true;
				view.Visible = true;
			}
		}
	}
	
	private void SortDDL(ref DropDownList objDDL)
     {
        ArrayList textList = new ArrayList();
        ArrayList valueList = new ArrayList();


        foreach (ListItem li in objDDL.Items)
        {
            textList.Add(li.Text);
        }    

        textList.Sort();


        foreach (object item in textList)
        {
            string value = objDDL.Items.FindByText(item.ToString()).Value;
            valueList.Add(value);
        }
        objDDL.Items.Clear();

    	for(int i = 0; i < textList.Count; i++)
        {
            ListItem objItem = new ListItem(textList[i].ToString(), valueList[i].ToString());
            objDDL.Items.Add(objItem);
        }
     }
	
	protected void downloadSpecs (object sender, EventArgs e)
    {
		specError.Text = "";
		if(document.SelectedValue == "") 
		{
			//Output: please select a product
			specError.Text = "Please select a document type.";
		}
		else {
			//Get Selected Model Documents
			getModelSpec(model.SelectedItem.Text);
			
			switch (document.SelectedValue)
			{
				case "Spec Sheet":
					if(SpecSheets == "") {
						//display unavailable messeage
						specError.Text = "There is no spec sheet available for download for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						DownloadFile(SpecSheets);
					}
					break;
				case "Sell Sheet":
					if(SellSheet == "") {
						//display unavailable messeage
						specError.Text = "There is no sell sheet available for download for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						DownloadFile(SellSheet);
					}
					break;
				case "Owners Manual":
					if(Manual == "") {
						//display unavailable messeage
						specError.Text = "There is no owners manual available for download for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						DownloadFile(Manual);
					}
					break;
				case "Parts Catalog":
					if(PartsCatelog == "") {
						//display unavailable messeage
						specError.Text = "There is no parts catalog available for download for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						DownloadFile(PartsCatelog);
					}
					break;
				case "Service Manual":
					if(ServiceManual == "") {
						//display unavailable messeage
						specError.Text = "There is no service manual available for download for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						DownloadFile(ServiceManual);
					}
					break;
				case "Images":
					GetModelImages(Convert.ToInt64(model.SelectedValue));
					
					if(HighRes == "") {
						//display unavailable messeage
						specError.Text = "There are no images available for download for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						DownloadFile(HighRes);
					}
					break;
			}
		}	
	}
	
	protected void viewSpecs (object sender, EventArgs e)
    {
		specError.Text = "";
		if(document.SelectedValue == "") 
		{
			//Output: please select a product
			specError.Text = "Please select a document type.";
		}
		else {
			//Get selected model Documents
			getModelSpec(model.SelectedItem.Text);
			
			switch (document.SelectedValue)
			{
				case "Spec Sheet":
					if(SpecSheets == "") {
						//display unavailable messeage
						specError.Text = "There is no spec sheet available for download for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						ScriptManager.RegisterStartupScript(Page, this.GetType(), "OpenWindow", "window.open('" + SpecSheets + "','_newtab');", true);
					}
					break;
				case "Sell Sheet":
					if(SellSheet == "") {
						//display unavailable messeage
						specError.Text = "There is no sell sheet available for download for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						ScriptManager.RegisterStartupScript(Page, this.GetType(), "OpenWindow", "window.open('" + SellSheet + "','_newtab');", true);
					}
					break;
				case "Owners Manual":
					if(Manual == "") {
						//display unavailable messeage
						specError.Text = "There is no owners manual available for download for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						ScriptManager.RegisterStartupScript(Page, this.GetType(), "OpenWindow", "window.open('" + Manual + "','_newtab');", true);
					}
					break;
				case "Parts Catalog":
					if(PartsCatelog == "") {
						//display unavailable messeage
						specError.Text = "There is no parts catalog available for download for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						ScriptManager.RegisterStartupScript(Page, this.GetType(), "OpenWindow", "window.open('" + PartsCatelog + "','_newtab');", true);
					}
					break;
				case "Service Manual":
					if(ServiceManual == "") {
						//display unavailable messeage
						specError.Text = "There is no service manual available for download for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						ScriptManager.RegisterStartupScript(Page, this.GetType(), "OpenWindow", "window.open('" + ServiceManual + "','_newtab');", true);
					}
					break;
				case "CAD Revit":
					if(CADRevit == "") {
						//display unavailable messeage
						specError.Text = "There is no CAD and Revit available for the selected model. If you need immediate assistance, please call 800-814-2028 to speak to a Vulcan customer service representative.";
					}
					else {
						ScriptManager.RegisterStartupScript(Page, this.GetType(), "OpenWindow", "window.open('" + CADRevit + "','_newtab');", true);
					}
					break;
			}
		}	
	}
	
	public void DownloadFile(string url)
	{
		WebClient client = new WebClient();
		byte[] data = client.DownloadData(new Uri(url));
		
		switch (document.SelectedValue)
		{
			case "Spec Sheet":
				Response.Clear();
				Response.ContentType = "application/pdf";
				Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", "specsheet.pdf"));
				Response.OutputStream.Write(data, 0, data.Length);
				Response.Flush();
				Response.Close();
				break;
			case "Sell Sheet":
				Response.Clear();
				Response.ContentType = "application/pdf";
				Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", "sellsheet.pdf"));
				Response.OutputStream.Write(data, 0, data.Length);
				Response.Flush();
				Response.Close();
				break;
			case "Owners Manual":
				Response.Clear();
				Response.ContentType = "application/pdf";
				Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", "ownersmanual.pdf"));
				Response.OutputStream.Write(data, 0, data.Length);
				Response.Flush();
				Response.Close();
				break;
			case "Parts Catalog":
				Response.Clear();
				Response.ContentType = "application/pdf";
				Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", "partscatalog.pdf"));
				Response.OutputStream.Write(data, 0, data.Length);
				Response.Flush();
				Response.Close();
				break;
			case "Service Manual":
				Response.Clear();
				Response.ContentType = "application/pdf";
				Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", "servicemanual.pdf"));
				Response.OutputStream.Write(data, 0, data.Length);
				Response.Flush();
				Response.Close();
				break;
			case "Images":
				Response.Clear();
				Response.ContentType = "application/zip";
				Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", "HighResImages.zip"));
				Response.OutputStream.Write(data, 0, data.Length);
				Response.Flush();
				Response.Close();
				break;
		}
	}
	
	public void GetModelImages(long ContentId) {
		string url = HttpContext.Current.Request.Url.AbsoluteUri;
		System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
		
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
			
		System.Xml.XmlNode node = doc.SelectSingleNode("/root/high_res/node()");
		if (node != null)
		{
			if(url.IndexOf("vulcan8.marriner") != -1) {
				HighRes = "http://vulcan8.marriner.com" + node.InnerText;
			}
			else {
				HighRes = "http://vulcanequipment.com" + node.InnerText;
			}
		}
	}

	private void getModelSpec(string modelId)
    {
		DataTable dt;
		DataRow dr;
		
		SpecSheets = "";
		SellSheet = "";
		Manual = "";
		PartsCatelog = "";
		ServiceManual = "";
		CADRevit = "";
		
		try {
			//Get First Model
			CADRevit = "http://vulcanhart.kclcad.com/?search=" + modelId;
		
			//Get Model Info from Resource Center Site
			dt = Vulcan.Product.Lib.SharepointHelper.GetGrid(modelId);
			
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
	
					sModels = strModel.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
					
					if(strModel.Trim() == modelId.Trim()) {
						isExactModel = true;
					}
						
					foreach (string s in sModels)
					{
						if(s.Trim() == modelId.Trim()) {
							isExactModel = true;
						}
					}
					
					if(isExactModel == true)
					{
						if(dr["ows_Product Group"].ToString().IndexOf("Vulcan") != -1) {
							if(strTitle == "Spec Sheet")
							{
								SpecSheets = strUrl;
							}
							if(strTitle == "Sell Sheet")
							{
								SellSheet = strUrl;
							}
							else if(strTitle == "Installation and Operation Manual" || strTitle == "Installation Manual" || strTitle == "Operation Manual")
							{
								Manual = strUrl;
							}
							else if(strTitle == "Parts Catalog")
							{
								PartsCatelog = strUrl;
							}
							else if(strTitle == "Service Manual")
							{
								ServiceManual = strUrl;
							}
						}
					}
				}
			}
		}
		catch {
		}
    }
	
	public void LoadTaxonomyData(int taxId)
    {
        Taxonomy api = new Taxonomy();
        TaxonomyData tdata = new TaxonomyData();
        TaxonomyRequest treq = new TaxonomyRequest();
        EkContent tapi = new EkContent();
        TaxonomyBaseData[] tbases;

        treq.TaxonomyId = taxId; 
        treq.TaxonomyLanguage = 1033;
        tdata = api.LoadTaxonomy(ref treq);
        tbases = api.EkContentRef.ReadAllSubCategories(treq);

		foreach (TaxonomyBaseData tbase in tbases)
		{
			DropDownItem newSeries = new DropDownItem(tbase.TaxonomyName, tbase.TaxonomyPath, tbase.TaxonomyId.ToString());
			SeriesList.Add(newSeries);
		}
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
        //searchData.OrderBy = WebSearchOrder.Title;
        //searchData.OrderDirection = WebSearchOrderByDirection.Ascending;
        searchData.TaxOperator = TaxonomyOperator.and;

        return this.searchapi.Search(searchData, HttpContext.Current, ref resultCount);
    }
	
	public class DropDownItem {
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
		
		public DropDownItem(){
		}
		
		public DropDownItem(string sName, string sPath, string sID){
			_Name = sName;
			_Path = sPath;
			_ID = sID;
		}
	}
}