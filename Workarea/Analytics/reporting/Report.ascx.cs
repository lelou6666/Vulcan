using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Common;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using Ektron.Cms.Interfaces.Analytics.Provider;
using Ektron.Cms.Analytics.Providers;

public partial class Analytics_reporting_Report : WorkareaBaseControl 
{

    #region Private Members

	private bool _bDrillDownReport = false;
	private bool _bDrillDownDetail = false;
	private string _strDrillDownArg = "for";
	private AnalyticsReportData _report = null;
	private string _reportTypeName = "";
	private string _columnName = "";
	private AnalyticsSortableField _sortField = AnalyticsSortableField.Name;
	private EkEnumeration.OrderByDirection _sortDirection = EkEnumeration.OrderByDirection.Ascending;
    private ContentAPI _refContentApi = new ContentAPI();
    private EkRequestInformation _requestInfo = null;
    IAnalytics _dataManager = ObjectFactory.GetAnalytics();
    private bool _reportGenerated = false;
    private string _reportTitle = string.Empty;

	private DateTime _startDate = DateTime.Today.AddDays(-1).AddDays(-30);
	private DateTime _endDate = DateTime.Today.AddDays(-1); // today is a partial day

	private ReportDisplayData _reportDisplayData = ReportDisplayData.SiteData;
    private enum ReportDisplayData
    {
        SiteData,
        PageData,
		LandingPageData,
		ExitPageData
    }

	private enum ReportTableColumn
	{
		DimensionName,
		[Description("{sam visits}")]
		Visits,
		[Description("%")]
		PercentVisits,
		[Description("{lbl pages visit}")]
		PagesPerVisit,
		[Description("{generic avg time on site}")]
		AverageTimeSpanOnSite,
		[Description("{lbl percent new visits}")]
		PercentNewVisits,
		[Description("{lbl entrances}")]
		Entrances,
		[Description("{lbl exits}")]
		Exits,
		[Description("{lbl pageviews}")]
		PageViews,
		[Description("%")]
		PercentPageviews,
		[Description("{lbl unique pageviews}")]
		UniqueViews,
		[Description("{lbl avg time on page}")]
		AverageTimeSpanOnPage,
		[Description("{lbl bounces}")]
		Bounces,
		[Description("{lbl bounce rate}")]
		BounceRate,
		[Description("{lbl percent exit}")]
		ExitRate
	}

    #endregion

    #region delegates, events and event arguments

    public event EventHandler<TitleChangedEventArgs> TitleChangedEventHandler;

    public class TitleChangedEventArgs : EventArgs {
        private string _title = string.Empty;

        public string Title {
            get { return _title; }
            set { _title = value; }
        }

        public TitleChangedEventArgs(string title) { _title = title; }
    }

    #endregion

    #region Properties

	private string _providerName = "";
	public string ProviderName
	{
		get { return (ViewState["ProviderName"] as string ?? _providerName); }
		set { ViewState["ProviderName"] = _providerName = value; }
	}

	public DateTime StartDate
	{
		get { return (DateTime)(ViewState["StartDate"] ?? _startDate); }
		set { ViewState["StartDate"] = _startDate = value; }
	}

	public DateTime EndDate
	{
		get { return (DateTime)(ViewState["EndDate"] ?? _endDate); }
		set { ViewState["EndDate"] = _endDate = value; }
	}

	public enum ReportType
	{
		/*MapOverlay,*/
		Locations,
		NewVsReturning,
		Languages,
		Browsers,
		OS,
		Platforms,
		Colors,
		Resolutions,
		Flash,
		Java,
		NetworkLocations,
		Hostnames,
		ConnectionSpeeds,
		UserDefined,
		Direct,
		Referring,
		SearchEngines,
		TrafficSources,
		Keywords,
		Campaigns,
		AdVersions,
		TopContent,
		ContentByTitle,
		TopLanding,
		TopExit,
		CmsSearchTerms
	}

	private ReportType _reportType = ReportType.TopContent;
	public ReportType Report
	{
		get { return (ReportType)(ViewState["Report"] ?? _reportType); }
		set { ViewState["Report"] = _reportType = value; }
	}
	private string _forValue = "";
	/// <summary>
	/// Primary criteria condition
	/// </summary>
	public string ForValue
	{
		get { return (ViewState["ForValue"] as string ?? _forValue); }
		set { ViewState["ForValue"] = _forValue = value; }
	}
	private string _andValue = "";
	/// <summary>
	/// Secondary criteria condition
	/// </summary>
	public string AndValue
	{
		get { return (ViewState["AndValue"] as string ?? _andValue); }
		set { ViewState["AndValue"] = _andValue = value; }
	}
	private string _alsoValue = "";
	/// <summary>
	/// Tertiary criteria condition
	/// </summary>
	public string AlsoValue
	{
		get { return (ViewState["AlsoValue"] as string ?? _alsoValue); }
		set { ViewState["AlsoValue"] = _alsoValue = value; }
	}

	public enum DisplayView
	{
		Default,
		Percentage,
		Table,
		Detail
	}
	private DisplayView _defaultView = DisplayView.Default;
	private DisplayView _displayView = DisplayView.Default;
	public DisplayView View
	{
		get 
		{ 
			DisplayView view = (DisplayView)(ViewState["View"] ?? _displayView);
			if (DisplayView.Default == view) view = _defaultView;
			return view; 
		}
		set { ViewState["View"] = _displayView = value; }
	}

	private bool _showTable = true;
	public bool ShowTable
	{
		get { return _showTable; }
		set { _showTable = value; }
	}
	private bool _showPieChart = true;
	public bool ShowPieChart
	{
		get { return _showPieChart; }
		set { _showPieChart = value; }
	}
	public bool ShowSummaryChart
	{
		get { return AnalyticsDetail.ShowSummaryChart; }
		set { AnalyticsDetail.ShowSummaryChart = value; }
	}
	public bool ShowLineChart
	{
		get { return AnalyticsDetail.ShowLineChart; }
		set { AnalyticsDetail.ShowLineChart = value; }
	}
	public Unit LineChartWidth
	{
		get { return AnalyticsDetail.LineChartWidth; }
		set { AnalyticsDetail.LineChartWidth = value; }
	}
	public Unit LineChartHeight
	{
		get { return AnalyticsDetail.LineChartHeight; }
		set { AnalyticsDetail.LineChartHeight = value; }
	}

    public string ReportTitle {
        get { return _reportTitle; }
        set {
            if (_reportTitle != value
                && TitleChangedEventHandler != null) {
                TitleChangedEventHandler(this, new TitleChangedEventArgs(value));
            }
            _reportTitle = value;
        }
    }

    public bool ShowDateRange {
        get { return AnalyticsReportDateRangeDisplay.Visible; }
        set { AnalyticsReportDateRangeDisplay.Visible = value; }
    }

	#endregion

    protected void Page_Load(object sender, EventArgs e)
	{
		ErrorPanel.Visible = false;
		if (null == _requestInfo) 		{
			_requestInfo = _refContentApi.RequestInformationRef;
		}

        if (!_dataManager.IsAnalyticsViewer()) {
            litErrorMessage.Text = GetMessage("com: user does not have permission");
            ErrorPanel.Visible = true;
            return;
        }

		RegisterScripts();

        Page.Validate();
        if (!Page.IsValid)
        {
            _reportGenerated = true;
        }

		ltrlNoRecords.Text = GetMessage("lbl no records");
        litCssTweaks.Text = String.Empty;
    }

	protected override void LoadViewState(object savedState)
	{
		base.LoadViewState(savedState);
		_providerName = this.ProviderName;
		_startDate = this.StartDate;
		_endDate = this.EndDate;
		_reportType = this.Report;
		_forValue = this.ForValue;
		_andValue = this.AndValue;
		_alsoValue = this.AlsoValue;
		_displayView = this.View;
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);

		if (!_reportGenerated)
		{
			RefreshAnalyticsReport(this, new EventArgs());
		}
	}

	protected virtual void RefreshAnalyticsReport(object sender, EventArgs e)
	{
        if (!String.IsNullOrEmpty(_providerName) && _dataManager.HasProvider(_providerName))
		{
			try
			{
				_report = GetAnalyticsReport(_providerName);
				bindData();
			}
			catch (TypeInitializationException ex)
			{
				string _error = ex.Message;
				lblNoRecords.Text = GetMessage("err analyticsconfig");
				lblNoRecords.Visible = true;
				EkException.LogException(ex);
				return;
			}
		}
	}

	private void UpdateCriteriaOrderBy(AnalyticsCriteria criteria, AnalyticsSortableField defaultField)
	{
		_sortField = defaultField;

		string sortExpression = ViewState["SortExpression"] as string;

		if (!String.IsNullOrEmpty(sortExpression))
		{
            if (Enum.IsDefined(typeof(AnalyticsSortableField), sortExpression))
			{
                AnalyticsSortableField newSortField = (AnalyticsSortableField)Enum.Parse(typeof(AnalyticsSortableField), sortExpression, true);
                if (!((_reportType == ReportType.TopContent || _reportType == ReportType.ContentByTitle ||_reportType == ReportType.TopLanding ||_reportType == ReportType.TopExit) && newSortField == AnalyticsSortableField.Visits))
				    _sortField = newSortField;
			}
		}

		_sortDirection = (_sortField == AnalyticsSortableField.Name ? EkEnumeration.OrderByDirection.Ascending : EkEnumeration.OrderByDirection.Descending);

		if (ViewState["SortDirection"] != null)
		{
			if (SortDirection.Ascending == (SortDirection)ViewState["SortDirection"])
			{
				_sortDirection = EkEnumeration.OrderByDirection.Ascending;
			}
			else
			{
				_sortDirection = EkEnumeration.OrderByDirection.Descending;
			}
		}

		ViewState["SortExpression"] = _sortField.ToString();
		ViewState["SortDirection"] = (_sortDirection == EkEnumeration.OrderByDirection.Ascending ? SortDirection.Ascending : SortDirection.Descending);

		criteria.OrderByField = _sortField;
		criteria.OrderByDirection = _sortDirection;
	}

	private AnalyticsReportData GetAnalyticsReport(string provider)
    {
		AnalyticsReportData report = null;
		DateTime startDate = _startDate;
		DateTime endDate = _endDate;
		string reportSubtitle = "";
		string reportSummary = "";

		if (ErrorPanel.Visible) {
            this.htmReportSummary.InnerText = "";
            return null;
        }

        AnalyticsCriteria criteria = new AnalyticsCriteria();

		if (DisplayView.Detail == this.View)
		{
			criteria.AggregationPeriod = AggregationTimePeriod.ByDay;
		}

        try
        {
			_defaultView = DisplayView.Percentage;
			_bDrillDownReport = false;
			_bDrillDownDetail = true;
			switch (_reportType)
            {

				/* case ReportType.MapOverlay: */
				case ReportType.Locations:
                    ReportTitle = _reportTypeName = GetMessage("sam locations");
                    reportSummary = GetMessage("lbl visit came from countries");
                    _columnName = GetMessage("lbl country territory");
                    _defaultView = DisplayView.Table;
					_bDrillDownReport = true;
					_bDrillDownDetail = false;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.country, DimensionFilterOperator.EqualTo, _forValue);
						criteria.Dimensions.Insert(0, Dimension.region);
						reportSummary = GetMessage("lbl visit came from regions");
                        ReportTitle = _columnName;
						if (_forValue != "(not set)") // TODO: "(not set)" is Google-specific
						{
							reportSubtitle = _forValue;
						}
						_columnName = GetMessage("lbl region");
						_strDrillDownArg = "and";
					}
					if (!String.IsNullOrEmpty(_andValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.region, DimensionFilterOperator.EqualTo, _andValue);
						criteria.Dimensions.Insert(0, Dimension.city);
						reportSummary = GetMessage("lbl visit came from cities");
                        ReportTitle = _columnName;
						if (_andValue != "(not set)") // TODO: "(not set)" is Google-specific
						{
							if (reportSubtitle.Length > 0)
							{
								reportSubtitle = _andValue + ", " + reportSubtitle;
							}
							else
							{
								reportSubtitle = _andValue;
							}
						}
						_columnName = GetMessage("lbl address city");
						_bDrillDownReport = false;
						_bDrillDownDetail = true;
						_strDrillDownArg = "also";
					}
					if (!String.IsNullOrEmpty(_alsoValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.city, DimensionFilterOperator.EqualTo, _alsoValue);
                        ReportTitle = _columnName;
						if (_alsoValue != "(not set)") // TODO: "(not set)" is Google-specific
						{
							if (reportSubtitle.Length > 0)
							{
								reportSubtitle = _alsoValue + ", " + reportSubtitle;
							}
							else
							{
								reportSubtitle = _alsoValue;
							}
						}
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetLocations(provider, startDate, endDate, criteria);
                    break;
                case ReportType.NewVsReturning:
                    ReportTitle = _reportTypeName = GetMessage("sam new vs returning");
                    reportSummary = GetMessage("lbl visit from visitor types");
                    _columnName = GetMessage("lbl visitor type");
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.visitorType, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetNewVsReturningVisitors(provider, startDate, endDate, criteria);
                    break;
                case ReportType.Languages:
                    ReportTitle = _reportTypeName = GetMessage("sam languages");
                    reportSummary = GetMessage("lbl visit used languages");
					_columnName = GetMessage("generic language");
                    _defaultView = DisplayView.Table;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.language, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetLanguages(provider, startDate, endDate, criteria);
                    break;


                case ReportType.Browsers:
                    ReportTitle = _reportTypeName = GetMessage("sam browsers");
                    reportSummary = GetMessage("lbl visit used browsers");
                    _columnName = GetMessage("lbl browser");
					_bDrillDownReport = true;
					_bDrillDownDetail = false;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.browser, DimensionFilterOperator.EqualTo, _forValue);
						criteria.Dimensions.Insert(0, Dimension.browserVersion);
						reportSummary = GetMessage("lbl visit used browser versions");
                        ReportTitle = _columnName;
						reportSubtitle = _forValue + " ";
						_columnName = GetMessage("lbl browser version");
						_bDrillDownReport = false;
						_bDrillDownDetail = true;
						_strDrillDownArg = "and";
					}
					if (!String.IsNullOrEmpty(_andValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.browserVersion, DimensionFilterOperator.EqualTo, _andValue);
                        ReportTitle = _columnName;
						reportSubtitle += _andValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetBrowsers(provider, startDate, endDate, criteria);
                    break;
                case ReportType.OS:
                    ReportTitle = _reportTypeName = GetMessage("sam operating systems");
                    reportSummary = GetMessage("lbl visit used operating systems");
                    _columnName = GetMessage("lbl operating system");
					_bDrillDownReport = true;
					_bDrillDownDetail = false;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.operatingSystem, DimensionFilterOperator.EqualTo, _forValue);
						criteria.Dimensions.Insert(0, Dimension.operatingSystemVersion);
						reportSummary = GetMessage("lbl visit used os versions");
                        ReportTitle = _columnName;
						reportSubtitle = _forValue + " ";
						_columnName = GetMessage("lbl os version");
						_bDrillDownReport = false;
						_bDrillDownDetail = true;
						_strDrillDownArg = "and";
					}
					if (!String.IsNullOrEmpty(_andValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.operatingSystemVersion, DimensionFilterOperator.EqualTo, _andValue);
                        ReportTitle = _columnName;
						reportSubtitle += _andValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetOperatingSystems(provider, startDate, endDate, criteria);
                    break;
                case ReportType.Platforms:
                    ReportTitle = _reportTypeName = GetMessage("sam browsers and os");
                    reportSummary = GetMessage("lbl visit used browser and os combinations");
                    _columnName = GetMessage("lbl browser and os");
					if (!String.IsNullOrEmpty(_forValue))
					{
						string[] values = _forValue.Split(new char[] { '/' }, 2);
						criteria.DimensionFilters.AddFilter(Dimension.browser, DimensionFilterOperator.EqualTo, values[0].Trim());
						criteria.DimensionFilters.AddFilter(Dimension.operatingSystem, DimensionFilterOperator.EqualTo, values[1].Trim());
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetPlatforms(provider, startDate, endDate, criteria);
                    break;
                case ReportType.Colors:
                    ReportTitle = _reportTypeName = GetMessage("sam screen colors");
                    reportSummary = GetMessage("lbl visit used screen colors");
                    _columnName = GetMessage("sam screen colors");
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.screenColors, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetScreenColors(provider, startDate, endDate, criteria);
                    break;
                case ReportType.Resolutions:
                    ReportTitle = _reportTypeName = GetMessage("sam screen resolutions");
                    reportSummary = GetMessage("lbl visit used screen resolutions");
                    _columnName = GetMessage("lbl screen resolution");
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.screenResolution, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetScreenResolutions(provider, startDate, endDate, criteria);
                    break;
                case ReportType.Flash:
                    ReportTitle = _reportTypeName = GetMessage("sam flash versions");
                    reportSummary = GetMessage("lbl visit used flash versions");
                    _columnName = GetMessage("lbl flash version");
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.flashVersion, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetFlashVersions(provider, startDate, endDate, criteria);
                    break;
                case ReportType.Java:
                    ReportTitle = _reportTypeName = GetMessage("sam java support");
                    reportSummary = GetMessage("lbl visit used java support");
                    _columnName = ReportTitle;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.javaEnabled, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetJavaSupport(provider, startDate, endDate, criteria);
                    break;


                case ReportType.NetworkLocations:
                    ReportTitle = _reportTypeName = GetMessage("sam network location");
                    reportSummary = GetMessage("lbl visit came from network locations");
                    _columnName = ReportTitle;
                    _defaultView = DisplayView.Table;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.networkLocation, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetNetworkLocations(provider, startDate, endDate, criteria);
                    break;
                case ReportType.Hostnames:
                    ReportTitle = _reportTypeName = GetMessage("sam hostnames");
                    reportSummary = GetMessage("lbl visit came from hostnames");
                    _columnName = GetMessage("lbl hostname");
                    _defaultView = DisplayView.Table;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.hostname, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetHostnames(provider, startDate, endDate, criteria);
                    break;
                case ReportType.ConnectionSpeeds:
                    ReportTitle = _reportTypeName = GetMessage("sam connection speeds");
                    reportSummary = GetMessage("lbl visit used connection speeds");
                    _columnName = GetMessage("lbl connection speed");
                    _defaultView = DisplayView.Table;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.connectionSpeed, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetConnectionSpeeds(provider, startDate, endDate, criteria);
                    break;


                case ReportType.UserDefined:
                    ReportTitle = _reportTypeName = GetMessage("sam user defined");
                    reportSummary = GetMessage("lbl visit used user defined values");
                    _columnName = GetMessage("lbl user defined value");
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.userDefinedValue, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetUserDefinedValue(provider, startDate, endDate, criteria);
                    break;


                case ReportType.Direct:
                    ReportTitle = _reportTypeName = GetMessage("sam direct traffic");
                    reportSummary = GetMessage("lbl visit came directly to this site");
                    _columnName = GetMessage("lbl source");
					_defaultView = DisplayView.Detail;
					if (DisplayView.Detail == this.View)
					{
						criteria.AggregationPeriod = AggregationTimePeriod.ByDay;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetDirectTraffic(provider, startDate, endDate, criteria);
                    break;
                case ReportType.Referring:
                    ReportTitle = _reportTypeName = GetMessage("sam referring sites");
                    reportSummary = GetMessage("lbl referring sites sent visits via sources");
					_columnName = GetMessage("lbl referring site");
                    _defaultView = DisplayView.Table;
					_bDrillDownReport = true;
					_bDrillDownDetail = false;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.source, DimensionFilterOperator.EqualTo, _forValue);
						criteria.Dimensions.Insert(0, Dimension.referralPath);
						reportSummary = GetMessage("lbl referring site sent visits via paths");
                        ReportTitle = _columnName;
						reportSubtitle = _forValue + " ";
						_columnName = GetMessage("lbl referring link");
						_bDrillDownReport = false;
						_bDrillDownDetail = true;
						_strDrillDownArg = "and";
					}
					if (!String.IsNullOrEmpty(_andValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.referralPath, DimensionFilterOperator.EqualTo, _andValue);
                        ReportTitle = _columnName;
						reportSubtitle += ":\xA0 " + _andValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetReferringSites(provider, startDate, endDate, criteria);
                    break;
                case ReportType.SearchEngines:
                    ReportTitle = _reportTypeName = GetMessage("sam search engines");
                    reportSummary = GetMessage("lbl search sent total visits via sources");
                    _columnName = GetMessage("lbl search engine");
                    _defaultView = DisplayView.Table;
					_bDrillDownReport = true;
					_bDrillDownDetail = false;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.source, DimensionFilterOperator.EqualTo, _forValue);
						criteria.Dimensions.Insert(0, Dimension.keyword);
						reportSummary = GetMessage("lbl search sent total visits via keywords");
                        ReportTitle = _columnName;
						reportSubtitle = _forValue + " ";
						_columnName = GetMessage("lbl keyword");
						_bDrillDownReport = false;
						_bDrillDownDetail = true;
						_strDrillDownArg = "and";
					}
					if (!String.IsNullOrEmpty(_andValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.keyword, DimensionFilterOperator.EqualTo, _andValue);
                        ReportTitle = _columnName;
						reportSubtitle += ":\xA0 \"" + _andValue + "\"";
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetSearchEngines(provider, startDate, endDate, criteria);
                    break;
                case ReportType.TrafficSources:
                    ReportTitle = _reportTypeName = GetMessage("sam all traffic sources");
                    reportSummary = GetMessage("lbl all traffic sources sent visits via sources and mediums");
                    _columnName = GetMessage("lbl source");
                    _defaultView = DisplayView.Table;
					if (!String.IsNullOrEmpty(_forValue))
					{
						string[] values = _forValue.Split(new char[] { '/' }, 2);
						criteria.DimensionFilters.AddFilter(Dimension.source, DimensionFilterOperator.EqualTo, values[0].Trim());
						criteria.DimensionFilters.AddFilter(Dimension.medium, DimensionFilterOperator.EqualTo, values[1].Trim());
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetAllTrafficSources(provider, startDate, endDate, criteria);
                    break;
                case ReportType.Keywords:
                    ReportTitle = _reportTypeName = GetMessage("sam keywords");
                    reportSummary = GetMessage("lbl search sent total visits via keywords");
                    _columnName = GetMessage("lbl keyword");
                    _defaultView = DisplayView.Table;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.keyword, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetKeywords(provider, startDate, endDate, criteria);
                    break;
                case ReportType.Campaigns:
                    ReportTitle = _reportTypeName = GetMessage("sam campaigns");
                    reportSummary = GetMessage("lbl campaign traffic sent visits via campaigns");
                    _columnName = GetMessage("lbl campaign");
                    _defaultView = DisplayView.Table;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.campaign, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetCampaigns(provider, startDate, endDate, criteria);
                    break;
                case ReportType.AdVersions:
                    ReportTitle = _reportTypeName = GetMessage("sam ad versions");
                    reportSummary = GetMessage("lbl ads sent visits via ad contents");
                    _columnName = GetMessage("lbl ad content");
                    _defaultView = DisplayView.Table;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.adContent, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Visits);
					report = _dataManager.GetAdVersions(provider, startDate, endDate, criteria);
                    break;


                case ReportType.TopContent:
                    ReportTitle = _reportTypeName = GetMessage("sam top content");
                    reportSummary = GetMessage("lbl pages were viewed a total of times");
                    _columnName = GetMessage("page lbl");
					_defaultView = DisplayView.Table;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.pagePath, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.PageViews);
					report = _dataManager.GetTopContent(provider, startDate, endDate, criteria);
                    _reportDisplayData = ReportDisplayData.PageData; 
                    break;
                case ReportType.ContentByTitle:
                    ReportTitle = _reportTypeName = GetMessage("sam content by title");
                    reportSummary = GetMessage("lbl page titles were viewed a total times");
                    _columnName = GetMessage("lbl page title");
					_defaultView = DisplayView.Table;
					_bDrillDownReport = true;
					_bDrillDownDetail = false;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.pageTitle, DimensionFilterOperator.EqualTo, _forValue);
						criteria.Dimensions.Insert(0, Dimension.pagePath);
						reportSummary = GetMessage("lbl page visited times via pages");
                        ReportTitle = _columnName;
						reportSubtitle = _forValue + " ";
						_columnName = GetMessage("page lbl");
						_bDrillDownReport = false;
						_bDrillDownDetail = true;
						_strDrillDownArg = "and";
					}
					if (!String.IsNullOrEmpty(_andValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.pagePath, DimensionFilterOperator.EqualTo, _andValue);
                        ReportTitle = _columnName;
						reportSubtitle += ":\xA0 \"" + _andValue + "\"";
						_defaultView = DisplayView.Detail;
					}
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.PageViews);
					report = _dataManager.GetContentbyTitle(provider, startDate, endDate, criteria);
                    _reportDisplayData = ReportDisplayData.PageData; 
                    break;
                case ReportType.TopLanding:
                    ReportTitle = _reportTypeName = GetMessage("sam top landing pages");
                    reportSummary = GetMessage("lbl visit entered the site through pages");
                    _columnName = GetMessage("page lbl");
					_defaultView = DisplayView.Table;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.pagePath, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;

						UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.PageViews);
						report = _dataManager.GetTopContent(provider, startDate, endDate, criteria);
						_reportDisplayData = ReportDisplayData.PageData;
					}
					else
					{
						UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Entrances);
						report = _dataManager.GetTopLandingPages(provider, startDate, endDate, criteria);
						_reportDisplayData = ReportDisplayData.LandingPageData;
					}
                    break;
                case ReportType.TopExit:
                    ReportTitle = _reportTypeName = GetMessage("sam top exit pages");
                    reportSummary = GetMessage("lbl visits exited from pages");
                    _columnName = GetMessage("page lbl");
					_defaultView = DisplayView.Table;
					if (!String.IsNullOrEmpty(_forValue))
					{
						criteria.DimensionFilters.AddFilter(Dimension.pagePath, DimensionFilterOperator.EqualTo, _forValue);
                        ReportTitle = _columnName;
						reportSubtitle = _forValue;
						_defaultView = DisplayView.Detail;

						UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.PageViews);
						report = _dataManager.GetTopContent(provider, startDate, endDate, criteria);
						_reportDisplayData = ReportDisplayData.PageData;
					}
					else
					{
						UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.Exits);
						report = _dataManager.GetTopExitPages(provider, startDate, endDate, criteria);
						_reportDisplayData = ReportDisplayData.ExitPageData;
					}
                    break;

                case ReportType.CmsSearchTerms:
                    litCssTweaks.Text = "<style type='text/css'> .EktronPersonalization .analyticsReport .SiteSelectorContainer {display: none;} </style>";
					ReportTitle = _reportTypeName = GetMessage("lbl cms search terms");
                    reportSummary = GetMessage("lbl searches used search terms");
                    _columnName = GetMessage("lbl phrase");
					_bDrillDownDetail = false; // not implemented
					UpdateCriteriaOrderBy(criteria, AnalyticsSortableField.PageViews);
					report = _dataManager.GetCmsSearchTerms(startDate, endDate);
                    break;
            }
        }
        catch (Exception ex)
        {
            litErrorMessage.Text = ex.Message;
            ErrorPanel.Visible = true;
        }

        if (report != null)
        {
            htmReportTitle.InnerText = ReportTitle;

            RenderDateRange();

			if (!String.IsNullOrEmpty(reportSubtitle))
			{
				htmReportSubtitle.InnerText = reportSubtitle;
				htmReportSubtitle.Visible = true;
			}

			if (DisplayView.Detail == this.View)
			{
				switch (_reportDisplayData)
				{
					case ReportDisplayData.SiteData:
						this.htmReportSummary.Visible = false;
						break;
					case ReportDisplayData.PageData:
					case ReportDisplayData.LandingPageData:
					case ReportDisplayData.ExitPageData:
						this.htmReportSummary.InnerText = String.Format(GetMessage("lbl page viewed times"), report.TotalPageViews);
						break;
					default:
						throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
				}
			}
			else
			{
				switch (_reportDisplayData)
				{
					case ReportDisplayData.SiteData:
						this.htmReportSummary.InnerText = String.Format(reportSummary, report.TotalVisits, report.TotalResults);
						break;
					case ReportDisplayData.PageData:
						this.htmReportSummary.InnerText = String.Format(reportSummary, report.TotalResults, report.TotalPageViews);
						break;
					case ReportDisplayData.LandingPageData:
						this.htmReportSummary.InnerText = String.Format(reportSummary, report.TotalEntrances, report.TotalResults);
						break;
					case ReportDisplayData.ExitPageData:
						this.htmReportSummary.InnerText = String.Format(reportSummary, report.TotalExits, report.TotalResults);
						break;
					default:
						throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
				}
			}
        }
        _reportGenerated = true;
        return report;
    }

    protected void RenderDateRange() {
        try {
            if (_refContentApi != null && _refContentApi.ContentLanguage > 0) {
                IFormatProvider format = new System.Globalization.CultureInfo(_refContentApi.ContentLanguage);
                AnalyticsReportDateRangeDisplay.InnerText = StartDate.ToString("d", format) + " - " + this.EndDate.ToString("d", format);
                return;
            }
        }
        catch {}

        AnalyticsReportDateRangeDisplay.InnerText = this.StartDate.ToShortDateString() + " - " + this.EndDate.ToShortDateString();
    }

    private void bindData()
    {
		if (null == _report) return;

		DisplayView view = this.View;

		string urlBreadcrumb = Request.Url.AbsolutePath;
		lnkBreadcrumbReport.Visible = false;
		lnkBreadcrumbFor.Visible = false;
		lnkBreadcrumbAnd.Visible = false;
		lnkBreadcrumbReport.InnerText = _reportTypeName;
		urlBreadcrumb += "?report=" + EkFunctions.UrlEncode(_reportType.ToString());
		lnkBreadcrumbReport.HRef = urlBreadcrumb + "&for=&and=&also=&view=";
		if (!String.IsNullOrEmpty(_forValue))
		{
			lnkBreadcrumbReport.Visible = true;
			AddUpdatePanelTrigger(lnkBreadcrumbReport);
			htmBreadcrumbSeparatorFor.Visible = true;
			lblBreadcrumbFor.Visible = true;
			lblBreadcrumbFor.Text = _forValue;
			lnkBreadcrumbFor.InnerText = lblBreadcrumbFor.Text;
			urlBreadcrumb += "&for=" + EkFunctions.UrlEncode(lnkBreadcrumbFor.InnerText);
			lnkBreadcrumbFor.HRef = urlBreadcrumb + "&and=&also=&view=";
		}
		if (!String.IsNullOrEmpty(_andValue))
		{
			lblBreadcrumbFor.Visible = false;
			lnkBreadcrumbFor.Visible = true;
			AddUpdatePanelTrigger(lnkBreadcrumbFor);
			htmBreadcrumbSeparatorAnd.Visible = true;
			lblBreadcrumbAnd.Visible = true;
			lblBreadcrumbAnd.Text = _andValue;
			lnkBreadcrumbAnd.InnerText = lblBreadcrumbAnd.Text;
			urlBreadcrumb += "&and=" + EkFunctions.UrlEncode(lnkBreadcrumbAnd.InnerText);
			lnkBreadcrumbAnd.HRef = urlBreadcrumb + "&also=&view=";
		}
		if (!String.IsNullOrEmpty(_alsoValue))
		{
			lblBreadcrumbAnd.Visible = false;
			lnkBreadcrumbAnd.Visible = true;
			AddUpdatePanelTrigger(lnkBreadcrumbAnd);
			htmBreadcrumbSeparatorAlso.Visible = true;
			lblBreadcrumbAlso.Visible = true;
			lblBreadcrumbAlso.Text = _alsoValue;
		}

		gvDataTable.Visible = false;
		AnalyticsDetail.Visible = false;
		AnalyticsPieChart.Visible = false;

		if (DisplayView.Detail == view)
		{
			AnalyticsDetail.Visible = true;
			AnalyticsDetail.StartDate = _startDate;
			AnalyticsDetail.EndDate = _endDate;

			switch (_reportDisplayData)
			{
				case ReportDisplayData.SiteData:
					AnalyticsDetail.ShowVisits = true;
					AnalyticsDetail.ShowPagesPerVisit = true;
					AnalyticsDetail.ShowTimeOnSite = true;
					AnalyticsDetail.ShowBounceRate = true;
					this.htmReportSummary.Visible = false;
					break;
				case ReportDisplayData.PageData:
					AnalyticsDetail.ShowPageviews = true;
					AnalyticsDetail.ShowUniqueViews = true;
					AnalyticsDetail.ShowTimeOnPage = true;
					AnalyticsDetail.ShowBounceRate = true;
					AnalyticsDetail.ShowPercentExit = true;
					break;
				case ReportDisplayData.LandingPageData:
					AnalyticsDetail.ShowBounceRate = true;
					this.htmReportSummary.Visible = false;
					break;
				case ReportDisplayData.ExitPageData:
					AnalyticsDetail.ShowPageviews = true;
					AnalyticsDetail.ShowPercentExit = true;
					this.htmReportSummary.Visible = false;
					break;
				default:
					throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
			}

			AnalyticsDetail.UpdateReport(_report);
		}
		else if (_showTable)
		{
			gvDataTable.Visible = true;
			ReportTableColumn[] visibleColumns = new ReportTableColumn[] { ReportTableColumn.DimensionName };
			if (DisplayView.Table == view)
			{
				switch (_reportDisplayData)
				{
					case ReportDisplayData.SiteData:
						visibleColumns = new ReportTableColumn[] 
						{ 
							ReportTableColumn.DimensionName,
							ReportTableColumn.Visits,
							ReportTableColumn.PagesPerVisit,
							ReportTableColumn.AverageTimeSpanOnSite,
							ReportTableColumn.PercentNewVisits,
							ReportTableColumn.BounceRate
						};
						break;
					case ReportDisplayData.PageData:
						visibleColumns = new ReportTableColumn[] 
						{ 
							ReportTableColumn.DimensionName,
							ReportTableColumn.PageViews,
							ReportTableColumn.UniqueViews,
							ReportTableColumn.AverageTimeSpanOnPage,
							ReportTableColumn.BounceRate,
							ReportTableColumn.ExitRate
						};
						break;
					case ReportDisplayData.LandingPageData:
						visibleColumns = new ReportTableColumn[] 
						{ 
							ReportTableColumn.DimensionName,
							ReportTableColumn.Entrances,
							ReportTableColumn.Bounces,
							ReportTableColumn.BounceRate
						};
						break;
					case ReportDisplayData.ExitPageData:
						visibleColumns = new ReportTableColumn[] 
						{ 
							ReportTableColumn.DimensionName,
							ReportTableColumn.Exits,
							ReportTableColumn.PageViews,
							ReportTableColumn.ExitRate
						};
						break;
					default:
						throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
				}
			}
			else if (DisplayView.Percentage == view)
			{
				switch (_reportDisplayData)
				{
					case ReportDisplayData.SiteData:
						visibleColumns = new ReportTableColumn[] 
						{ 
							ReportTableColumn.DimensionName,
							ReportTableColumn.Visits,
							ReportTableColumn.PercentVisits
						};
						break;
					case ReportDisplayData.PageData:
						visibleColumns = new ReportTableColumn[] 
						{ 
							ReportTableColumn.DimensionName,
							ReportTableColumn.PageViews,
							ReportTableColumn.PercentPageviews
						};
						break;
					case ReportDisplayData.LandingPageData:
						visibleColumns = new ReportTableColumn[] 
						{ 
							ReportTableColumn.DimensionName,
							ReportTableColumn.Entrances,
							ReportTableColumn.BounceRate
						};
						break;
					case ReportDisplayData.ExitPageData:
						visibleColumns = new ReportTableColumn[] 
						{ 
							ReportTableColumn.DimensionName,
							ReportTableColumn.Exits,
							ReportTableColumn.ExitRate
						};
						break;
					default:
						throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
				}
			}

			for (int iGridCol = 0; iGridCol < gvDataTable.Columns.Count; iGridCol++)
			{
				DataControlField column = gvDataTable.Columns[iGridCol];
				column.Visible = false;
				for (int iVisCol = 0; iVisCol < visibleColumns.Length; iVisCol++)
				{
					if (visibleColumns[iVisCol] == (ReportTableColumn)iGridCol)
					{
						if (iVisCol >= 0)
						{
							if (0 == iGridCol)
							{
								column.HeaderText = _columnName;
							}
							else
							{
								column.HeaderText = GetEnumDisplayText(visibleColumns[iVisCol]);
							}
							column.Visible = true;
						}
						break;
					}
				}
			}

			gvDataTable.DataSource = _report.ReportItems;
			gvDataTable.DataBind();

			lblNoRecords.Visible = false;
			pnlData.Visible = true;
		}
		else if (_showPieChart)
		{
			AnalyticsPieChart.Visible = true;
			UpdatePieChart(AnalyticsPieChart, _report);
		}
	}

	private string GetEnumDisplayText(Enum enumValue)
	{
		System.Reflection.FieldInfo enumInfo = enumValue.GetType().GetField(enumValue.ToString());
		DescriptionAttribute[] enumAttributes = (DescriptionAttribute[])enumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
		if (enumAttributes != null && enumAttributes.Length > 0)
		{
			string displayText = enumAttributes[0].Description;
			if (String.IsNullOrEmpty(displayText))
			{
				return enumValue.ToString();
			}
			else if (displayText.StartsWith("{") && displayText.EndsWith("}"))
			{
				return GetMessage(displayText.Trim(new char[] { '{', '}' }));
			}
			else
			{
				return displayText;
			}
		}
		return enumValue.ToString();
	}

    protected string GetReportName(int index) {
        ReportItem item = GetReportItem(index);
        if (item == null)
            return string.Empty;

        return item.Name;
    }

    protected AnalyticsReportItem GetReportItem(int index) {
        return (_report.ReportItems != null ? ((_report.ReportItems.Count >= index) ? _report.ReportItems[index] : null) : null);
    }

    public string GetPercentVisits(object itemValue) {
		int value = Convert.ToInt32(itemValue);
		return EkFunctions.GetPercent(value, _report.TotalVisits).ToString("0.00%");
    }

	public string GetPercentPageviews(object itemValue)
	{
        int value = Convert.ToInt32(itemValue);
		return EkFunctions.GetPercent(value, _report.TotalPageViews).ToString("0.00%");
    }

	protected virtual void UpdatePieChart(controls_reports_PercentPieChart pieChart, AnalyticsReportData report)
	{
		List<float> data = new List<float>();
		List<string> colors = new List<string>();
		List<string> names = new List<string>();
		for (int i = 0; i < report.ReportItems.Count; i++)
		{
			float itemPercent = 0;
			switch (_reportDisplayData)
			{
				case ReportDisplayData.SiteData:
					itemPercent = EkFunctions.GetPercent(report.ReportItems[i].Visits, report.TotalVisits);
					break;
				case ReportDisplayData.PageData:
					itemPercent = EkFunctions.GetPercent(report.ReportItems[i].PageViews, report.TotalPageViews);
					break;
				case ReportDisplayData.LandingPageData:
					itemPercent = EkFunctions.GetPercent(report.ReportItems[i].Entrances, report.TotalEntrances);
					break;
				case ReportDisplayData.ExitPageData:
					itemPercent = EkFunctions.GetPercent(report.ReportItems[i].Exits, report.TotalExits);
					break;
				default:
					throw new ArgumentOutOfRangeException("_reportDisplayData", "Unknown ReportDisplayData: " + _reportDisplayData);
			}
			data.Add(itemPercent);
			colors.Add(GetReportItemColor(i));
			names.Add(report.ReportItems[i].Name);
		}

		pieChart.BriefDescription = GetMessage("lbl contribution to total");
		pieChart.LoadData(data);
		pieChart.LoadColors(colors);
		pieChart.LoadNames(names);
	}

    protected void GvDataTable_RowCreated(object sender, GridViewRowEventArgs e)
    {
		if (DisplayView.Percentage == this.View && _showPieChart)
		{
			if (e.Row.RowType == DataControlRowType.Header)
			{

				TableHeaderCell tc = new TableHeaderCell();
				tc.Attributes.Add("scope", "col");
				tc.Controls.Add(new LiteralControl(GetMessage("lbl contribution to total")));
				//DropDownList contribDropdown = new DropDownList();
				//contribDropdown.ReportItems.Add(new ListItem("Visits", "0"));
				//contribDropdown.ReportItems.Add(new ListItem("Views", "1"));
				//tc.Controls.Add(contribDropdown);
				e.Row.Cells.Add(tc);

			}
			else if (e.Row.RowType == DataControlRowType.Pager)
			{

			}
			else if (e.Row.RowType == DataControlRowType.DataRow)
			{

				if (e.Row.RowIndex == 0)
				{

					TableCell tc = new TableCell();

					tc.RowSpan = 1;
					tc.VerticalAlign = VerticalAlign.Middle;
					tc.HorizontalAlign = HorizontalAlign.Center;

					if (_report != null)
					{
						controls_reports_PercentPieChart pieChart = (controls_reports_PercentPieChart)LoadControl(CommonApi.AppPath + "controls/reports/PercentPieChart.ascx");
						pieChart.Width = new Unit(250, UnitType.Pixel);
						pieChart.Height = new Unit(250, UnitType.Pixel);
						pieChart.Legend = controls_reports_PercentPieChart.LegendPosition.BottomVertical;

						UpdatePieChart(pieChart, _report);

						tc.Controls.Add(pieChart);
					}

					e.Row.Cells.Add(tc);

				}
				else
				{

					TableCell tc = gvDataTable.Rows[0].Cells[gvDataTable.Rows[0].Cells.Count - 1];
					tc.RowSpan = tc.RowSpan + 1;

				}
			}
		}
    }

    protected void GvDataTable_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList dropDown = (DropDownList)sender;
        this.gvDataTable.PageSize = int.Parse(dropDown.SelectedValue);
        _report = GetAnalyticsReport(_providerName);
        bindData();
    }

    protected void GoToPage_TextChanged(object sender, EventArgs e)
    {
        TextBox txtGoToPage = (TextBox)sender;

        int pageNumber;
        if (int.TryParse(txtGoToPage.Text.Trim(), out pageNumber) && pageNumber > 0 && pageNumber <= this.gvDataTable.PageCount)
        {
            this.gvDataTable.PageIndex = pageNumber - 1;
        }
        else
        {
            this.gvDataTable.PageIndex = 0;
        }
        _report = GetAnalyticsReport(_providerName);
        bindData();
    }

    protected void GvDataTable_OnSorting(object sender, GridViewSortEventArgs e)
    {
		string sortExpression = e.SortExpression;

		if (String.IsNullOrEmpty(sortExpression)) return;

		SortDirection sortDirection = (sortExpression == AnalyticsSortableField.Name.ToString() ? SortDirection.Ascending : SortDirection.Descending);

		string prevSortExpression = ViewState["SortExpression"] as string;
		if (prevSortExpression != null && ViewState["SortDirection"] != null)
		{
			// Check if the same column is being sorted.
			// Otherwise, the default value can be returned.
			if (sortExpression == prevSortExpression)
			{
				if (SortDirection.Ascending == (SortDirection)ViewState["SortDirection"])
				{
					sortDirection = SortDirection.Descending;
				}
				else
				{
					sortDirection = SortDirection.Ascending;
				}
			}
		}

		ViewState["SortExpression"] = sortExpression;
		ViewState["SortDirection"] = sortDirection;

		_report = GetAnalyticsReport(_providerName);
        bindData();
    }

	protected void GvDataTable_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		gvDataTable.PageIndex = e.NewPageIndex;
		RefreshAnalyticsReport(sender, e);
	}

	protected void GvDataTable_RowDataBound(object sender, GridViewRowEventArgs e)
    {
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			Label lblNameValue = (Label)e.Row.FindControl("lblNameValue");
			Label lblNameValueLinked = (Label)e.Row.FindControl("lblNameValueLinked");
			if (lblNameValue != null)
			{
				int index = e.Row.DataItemIndex;
				string name = GetReportName(index);
				string truncatedName = (name.Length > 58 ? name.Substring(0, 58) : name);
				//if (_showPieChart && DisplayView.Percentage == this.View)
				//{
				//    Image imgColorBox = (Image)e.Row.FindControl("imgColorBox");
				//    if (imgColorBox != null)
				//    {
				//        imgColorBox.ImageUrl = _refContentApi.AppImgPath + "transparent.gif";
				//        imgColorBox.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#" + GetReportItemColor(e.Row.RowIndex));
				//        imgColorBox.Visible = true;
				//    }
				//}
				//else 
				if (ReportType.TopContent == _reportType)
				{
					HyperLink lnkGo = (HyperLink)e.Row.FindControl("lnkGo");
					if (lnkGo != null)
					{
						lnkGo.ImageUrl = _refContentApi.AppPath + "images/UI/Icons/linkGo.png";
                        string siteUrl = _dataManager.GetProviderSiteURL(_providerName);
                        lnkGo.NavigateUrl = (siteUrl.IndexOf("http") == 0 ? siteUrl : "http://" + siteUrl) + name;
						lnkGo.Attributes.Add("title", GetMessage("lbl visit this page"));
						lnkGo.Visible = true;
					}
				}
				System.Web.UI.HtmlControls.HtmlAnchor lnkDrillDown = (System.Web.UI.HtmlControls.HtmlAnchor)e.Row.FindControl("lnkDrillDown");
				if (lnkDrillDown != null)
				{
					lblNameValueLinked.Text = truncatedName;
					if (_bDrillDownReport || _bDrillDownDetail)
					{
						if (Request.RawUrl.Contains("?"))
						{
							lnkDrillDown.HRef = Request.RawUrl + "&" + _strDrillDownArg + "=" + EkFunctions.UrlEncode(name);
						}
						else
						{
							StringBuilder sbUrl = new StringBuilder(Request.RawUrl);
							sbUrl.Append("?report=").Append(_reportType.ToString());
							if (!String.IsNullOrEmpty(_forValue) && _strDrillDownArg != "for")
							{
								sbUrl.Append("&for=").Append(EkFunctions.UrlEncode(_forValue));
							}
							if (!String.IsNullOrEmpty(_andValue) && _strDrillDownArg != "and")
							{
								sbUrl.Append("&and=").Append(EkFunctions.UrlEncode(_andValue));
							}
							sbUrl.Append("&").Append(_strDrillDownArg).Append("=").Append(EkFunctions.UrlEncode(name));
							lnkDrillDown.HRef = sbUrl.ToString();
						}
						if (_bDrillDownDetail)
						{
							lnkDrillDown.HRef += "&view=detail";
						}
						AddUpdatePanelTrigger(lnkDrillDown);
					}
					else
					{
						lnkDrillDown.Visible = false;
						lblNameValue.Text = truncatedName;
						lblNameValue.Visible = true;
					}
				}
				else
				{
					lblNameValue.Text = truncatedName;
					lblNameValue.Visible = true;
				}
			}
		}

        GridView gridView = (GridView)sender;

		string sortField = _sortField.ToString();
        int cellIndex = -1;
        foreach (DataControlField field in gridView.Columns)
        {
			if (field.SortExpression == sortField)
            {
                cellIndex = gridView.Columns.IndexOf(field);
                break;
            }
        }

        if (cellIndex > -1)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                //  this is a header row,
                //  set the sort style
				string className = "";
				try
				{
					className = ((DataControlFieldCell)e.Row.Cells[cellIndex]).ContainingField.HeaderStyle.CssClass;
					className = System.Text.RegularExpressions.Regex.Replace(className, @"\s*sort(asc|desc)headerstyle\s*", " ");
					className += " ";
				}
				catch { }
				e.Row.Cells[cellIndex].CssClass = className + (_sortDirection == EkEnumeration.OrderByDirection.Ascending ? "sortascheaderstyle" : "sortdescheaderstyle");
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //  this is an alternating row
				string className = "";
				try
				{
					className = ((DataControlFieldCell)e.Row.Cells[cellIndex]).ContainingField.ItemStyle.CssClass;
					className = System.Text.RegularExpressions.Regex.Replace(className, @"\s*sort(alternating)?rowstyle\s*", " ");
					className += " ";
				}
				catch { }
				e.Row.Cells[cellIndex].CssClass = className + (e.Row.RowIndex % 2 == 0 ? "sortalternatingrowstyle" : "sortrowstyle");
            }
        }

        if (e.Row.RowType == DataControlRowType.Pager)
        {

            Label lblGoTo = (Label)e.Row.FindControl("lblGoTo");
            lblGoTo.Text = GetMessage("lbl go to");

            Label lblTotalNumberOfPages = (Label)e.Row.FindControl("lblTotalNumberOfPages");
            TextBox txtGoToPage = (TextBox)e.Row.FindControl("txtGoToPage");
            txtGoToPage.Text = (gridView.PageIndex + 1).ToString();
            lblTotalNumberOfPages.Text = String.Format(GetMessage("lbl current page of total pages"), txtGoToPage.Text, gridView.PageCount);

			Label lblShowRows = (Label)e.Row.FindControl("lblShowRows");
            lblShowRows.Text = GetMessage("lbl show rows");

            DropDownList ddlPageSize = (DropDownList)e.Row.FindControl("ddlPageSize");
            ddlPageSize.SelectedValue = gridView.PageSize.ToString();

        }
    }

	private void AddUpdatePanelTrigger(System.Web.UI.HtmlControls.HtmlAnchor linkControl)
	{
		AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
		trigger.ControlID = linkControl.UniqueID;
		trigger.EventName = "ServerClick";
		UpdatePanel1.Triggers.Add(trigger);
	}

	protected virtual void HtmlLink_OnServerClick(object sender, EventArgs e)
	{
		System.Web.UI.HtmlControls.HtmlAnchor link = sender as System.Web.UI.HtmlControls.HtmlAnchor;
		string url = link.HRef;
		url = url.Substring(url.IndexOf('?'));

		System.Collections.Specialized.NameValueCollection queryString = HttpUtility.ParseQueryString(url);

		LoadStateFromQueryString(queryString);

		RefreshAnalyticsReport(sender, e);
	}

	protected virtual void AnalyticsDetail_SelectionChanged(object sender, EventArgs e)
	{
		_report = GetAnalyticsReport(_providerName);
		AnalyticsDetail.StartDate = _startDate;
		AnalyticsDetail.EndDate = _endDate;
		AnalyticsDetail.UpdateReport(_report);
	}

	public virtual void LoadStateFromQueryString(System.Collections.Specialized.NameValueCollection queryString)
	{
		string report = queryString["report"];
		if (!String.IsNullOrEmpty(report))
		{
			this.Report = (ReportType)Enum.Parse(typeof(ReportType), report, true); // ignore case
		}
		string forValue = queryString["for"];
		if (forValue != null)
		{
			this.ForValue = forValue;
		}
		string andValue = queryString["and"];
		if (andValue != null)
		{
			this.AndValue = andValue;
		}
		string alsoValue = queryString["also"];
		if (alsoValue != null)
		{
			this.AlsoValue = alsoValue;
		}
		string displayView = queryString["view"];
		if (displayView != null)
		{
			if (displayView.Length > 0)
			{
				this.View = (DisplayView)Enum.Parse(typeof(DisplayView), displayView, true); // ignore case
			}
			else
			{
				this.View = DisplayView.Default;
			}
		}
	}

    public void RegisterScripts() {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
		Ektron.Cms.API.Css.RegisterCss(this, CommonApi.AppPath + "Analytics/reporting/css/Reports.css", "AnalyticsReportCss");
	}

	// sequential multihue from http://colorbrewer2.org/
	//private string[] _colors = new string[] { "E31A1C", "FD8D3C", "FEB24C", "FED976", "CC4C02", "EC7014", "FE9929", "FEC44F" };

	private string[] _colors = new string[] { "E58F3B", "EAB733", "6DAD46", "6BABE9", "A16BAD", "B0B0B0", "656565" };
	protected virtual string GetReportItemColor(int index)
	{
		if (0 == index) return "EA5F3F";
		int i = (index - 1) % _colors.Length;
		return _colors[i];
	}

	//private Random _random = null;

	//private string RandomHexColor()
	//{
	//    if (null == _random)
	//    {
	//        _random = new Random(System.DateTime.Now.Millisecond);
	//    }
	//    string hex = Convert.ToString(_random.Next(0, 255), 16).ToUpper();
	//    if (hex.Length < 2) hex = "0" + hex;
	//    return hex;
	//}

}


