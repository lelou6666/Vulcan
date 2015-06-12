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

public partial class Analytics_reporting_Trend : WorkareaBaseControl
{

    #region Private Members

	private ReportType _reportType = ReportType.Pageviews;
	private TrendReportData _report = null;
    private double maxValue = 0.0;
    private string _reportTitle = "";
    private bool _reportGenerated = false;

    private ContentAPI _refContentApi = new ContentAPI();
    private DataType dataType = DataType.Percentage;
    private EkRequestInformation _requestInfo = null;
    private IAnalytics _dataManager = ObjectFactory.GetAnalytics();

	private DateTime _startDate = DateTime.Today.AddDays(-1).AddDays(-30);
	private DateTime _endDate = DateTime.Today.AddDays(-1); // today is a partial day

	private enum DataType
    {
        Percentage,
        Value,
        Rate,
        Time
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
		get { return _providerName; }
		set { _providerName = value; }
	}

	public DateTime StartDate
	{
		get { return _startDate; }
		set { _startDate = value; }
	}

	public DateTime EndDate
	{
		get { return _endDate; }
		set { _endDate = value; }
	}

	public enum ReportType
	{
		Visits,
		AbsoluteUniqueVisitors,
		Pageviews,
		AveragePageviews,
		TimeOnSite,
		BounceRate
	}

	public ReportType Report
	{
		get { return _reportType; }
		set { _reportType = value; }
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
		if (null == _requestInfo) {
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
	}

	protected override void OnPreRender(EventArgs e)
	{
		try
		{
            if (!_reportGenerated)
            {
                _report = GetAnalyticsReport(_providerName, "Views", SortDirection.Descending);
                if (_report != null)
                {
                    grdData.DataSource = _report.Items;
                }
            }
		}
		catch (TypeInitializationException ex)
		{
			string _error = ex.Message;
			lblNoRecords.Text = GetMessage("err analyticsconfig");
			lblNoRecords.Visible = true;
			EkException.LogException(ex);
			return;
		}
		catch (Exception ex)
		{
			litErrorMessage.Text = ex.Message;
			ErrorPanel.Visible = true;
		}
		Util_BindData();

		base.OnPreRender(e);
	}
 
    private TrendReportData GetAnalyticsReport(string provider, string sortExpression, SortDirection sortDirection)
    {
		TrendReportData report = null;
		DateTime startDate = _startDate;
		DateTime endDate = _endDate;
		string reportSummary = "";

		if (ErrorPanel.Visible)
		{
			this.htmReportSummary.InnerText = "";
            return null;
        }

        AnalyticsCriteria criteria = new AnalyticsCriteria();

        try
        {
            switch (_reportType)
            {

                case ReportType.Visits:
                    report = _dataManager.GetVisitsTrend(provider, startDate, endDate, criteria);
                    ReportTitle = GetMessage("lbl visits for all visitors");
                    reportSummary = String.Format(GetMessage("lbl visits or visits per day"), report.Total, EkFunctions.GetRatio(report.Total, report.Items.Count));
                    break;

                case ReportType.AbsoluteUniqueVisitors:
                    report = _dataManager.GetAbsoluteUniqueVisitorsTrend(provider, startDate, endDate, criteria);
                    ReportTitle = GetMessage("sam absolute unique visitors");
                    reportSummary = String.Format(GetMessage("lbl absolute unique visitors report"), report.Total, 0);
                    break;

                case ReportType.Pageviews:
                    report = _dataManager.GetPageViewsTrend(provider, startDate, endDate, criteria);
                    ReportTitle = GetMessage("lbl pageviews for all visitors");
                    reportSummary = String.Format(GetMessage("lbl pageviews trend"), report.Total, 0);
                    break;

                case ReportType.AveragePageviews:
                    report = _dataManager.GetAveragePageViewsTrend(provider, startDate, endDate, criteria);
                    ReportTitle = GetMessage("lbl average pageviews for all visitors");
                    reportSummary = String.Format(GetMessage("lbl pages per visit"), report.Total, 0);
                    dataType = DataType.Value;
                    break;

                case ReportType.TimeOnSite:
                    report = _dataManager.GetTimeOnSiteTrend(provider, startDate, endDate, criteria);
                    ReportTitle = GetMessage("lbl time on site for all visitors");
                    reportSummary = String.Format(GetMessage("lbl avg time on site"), TimeSpan.FromSeconds(Math.Round(Convert.ToDouble(report.Total))), 0);
                    dataType = DataType.Time;
                    break;

                case ReportType.BounceRate:
                    report = _dataManager.GetBounceRateTrend(provider, startDate, endDate, criteria);
                    ReportTitle = GetMessage("lbl bounce rate for all visitors");
                    reportSummary = String.Format(GetMessage("lbl bounce rate report"), report.Total, 0);
                    dataType = DataType.Rate;
                    break;
            }

            if (report != null)
            {
                maxValue = report.MaximumValue;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message); 
        }

        if (report != null)
        {
            report.Items.Sort(new Comparison<ReportItem>(ReportItem.CompareDate));

			this.htmReportTitle.Visible = true;
			this.htmReportTitle.InnerText = reportSummary;
            RenderDateRange();
            this.htmReportSummary.Visible = false;
			//this.htmReportSubtitle.Visible = true;
			//this.htmReportSubtitle.InnerText = reportSummary;
			//this.htmReportSummary.InnerText = reportSummary;            
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
        catch { }

        AnalyticsReportDateRangeDisplay.InnerText = this.StartDate.ToShortDateString() + " - " + this.EndDate.ToShortDateString();
    }

    public void Util_BindData()
    {

        //this.grdData.Columns[0].HeaderText = "Date";
        //this.grdData.Columns[1].HeaderText = "Values";

        this.grdData.DataBind();
        lblNoRecords.Visible = false;
        pnlData.Visible = true;

    }

	public string Util_ShowValue(object value, object itemIndex)
    {

        string valueDisplay = "";
        int index = Convert.ToInt32(itemIndex);
        
        //valueDisplay = "<div style=\"display:inline;float:left;height:1.1em;background:#1A87D5;width:" + getWidthPercent(Convert.ToDouble(value)) + ";\">&#160;</div>&#160;";
		valueDisplay = "<div class=\"bar\" style=\"width:" + getWidthPercent(Convert.ToDouble(value)) + ";\">&#160;</div>";
        switch (dataType)
        {
            case DataType.Rate:
                valueDisplay += String.Format("{0:0.00%}", Convert.ToSingle(value));
                break;
            case DataType.Time:
				valueDisplay += TimeSpan.FromSeconds(Math.Round(Convert.ToDouble(value))).ToString();
                break;
            case DataType.Value:
                valueDisplay += String.Format("{0:0.00}", Convert.ToDouble(value));
                break;
            default:
                valueDisplay += String.Format("{0:0.00%} ({1:#,##0})", _report.GetItemPercent(index), Convert.ToInt32(value));
                break;
        }

        return valueDisplay;

    }

	private string getWidthPercent(double value)
    {
		if (0.0 == maxValue) return "0%";
        return ((value / maxValue) * 0.75).ToString("0%");

    }

    protected void Util_RegisterResources()
	{ RegisterScripts(); }


    public void RegisterScripts() {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
		Ektron.Cms.API.Css.RegisterCss(this, CommonApi.AppPath + "Analytics/reporting/css/Reports.css", "AnalyticsReportCss");
	}
}


