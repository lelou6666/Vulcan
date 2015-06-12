using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Analytics;
using Ektron.Cms;

public partial class Analytics_controls_SiteSelector : WorkareaBaseControl
{
	private string _cookieName = "";
    private IAnalytics _dataManager = ObjectFactory.GetAnalytics();

	public delegate void SelectionChangedHandler(object sender, EventArgs e);
	public event SelectionChangedHandler SelectionChanged;

	private string _persistenceId = "SiteAnalyticsSelectedSite";
	/// <summary>
	/// Used to store and retrieve date with client cookie.
	/// Default to none which prevents persistence.
	/// </summary>
	public string PersistenceId
	{
		get { return _persistenceId; }
		set { _persistenceId = value; }
	}

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

        if (_dataManager.IsAnalyticsViewer())
        {
			if (_persistenceId.Length > 0)
			{
				_cookieName = CommonApi.RequestInformationRef.UserId + "_" + _persistenceId;
			}
			if (SiteSelectorList.Items == null || SiteSelectorList.Items.Count == 0)
			{
				List<string> providerList = _dataManager.GetProviderList();

				SiteSelectorList.DataSource = providerList;
				SiteSelectorList.DataBind();

				if (_persistenceId.Length > 0)
				{
					// if found the selected site cookie, set the SiteSelectorList text.           
					HttpCookie selectedSiteCookie = Request.Cookies[_cookieName];
					if (selectedSiteCookie != null)
					{
						this.SelectedText = selectedSiteCookie.Value;
					}
				}
			}
        }
        lblSiteSelector.Text = GetMessage("lbl site");
	}

	protected virtual void DropDownList_SelectionChanged(object sender, EventArgs e)
	{
		if (_persistenceId.Length > 0)
		{
			//remember the selection in cookie
			HttpCookie selectedSiteCookie = new HttpCookie(_cookieName);
			selectedSiteCookie.Value = SiteSelectorList.SelectedItem.Value.ToString();
			selectedSiteCookie.Expires = DateTime.MaxValue; // Never Expires
			Response.Cookies.Add(selectedSiteCookie);
		}
		//reload the report
		if (SelectionChanged != null)
		{
			SelectionChanged(this, e);
		}
	}

	public virtual string SelectedText
	{
		get { return (SiteSelectorList != null && SiteSelectorList.SelectedItem != null) ? SiteSelectorList.SelectedItem.Text : String.Empty; }
		set
		{
			if (SiteSelectorList != null && SiteSelectorList.Items != null && SiteSelectorList.Items.Count > 0)
			{
				if (SiteSelectorList.Items.Contains(new ListItem(value)))
				{
					SiteSelectorList.SelectedValue = value;
				}
			}
		}
	}

	public string CssClass
	{
		get { return SiteSelectorContainer.Attributes["class"]; }
		set { SiteSelectorContainer.Attributes["class"] = value; }
	}

	public bool AutoPostBack
	{
		get { return SiteSelectorList.AutoPostBack; }
		set { SiteSelectorList.AutoPostBack = value; }
	}

}
