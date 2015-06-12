using System;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Analytics.Providers;
using Ektron.Cms.Interfaces.Analytics.Provider;
using System.Web.UI.WebControls;

public partial class Analytics_compare : workareabase {

    private long itemId = 0;
    private long versionId1 = 0;
    private long versionId2 = 0;
    private AnalyticsReportData report1 = null;
    private AnalyticsReportData report2 = null;
    private IAnalytics dataManager = ObjectFactory.GetAnalytics();
    private string errDataManager = "";
    private DateTime _startDate1;
    private DateTime _endDate1;
    private DateTime _startDate2;
    private DateTime _endDate2;
    private string version1 = "";
    private string version2 = "";
    private string _provider = "";

    private enum Metrics {
        Pageviews,
        UniqueViews,
        TimeOnPage,
        BounceRate,
        PercentExit
    }

    #region protected properties

    #endregion

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        MetricSelector.MetricSelectors = Analytics_reporting_MetricSelector.SelectorCount.Single;
    }

    protected void Page_Load(object sender, EventArgs e) {
        try {
            if (!IsPostBack) {
                litLoadingMessage.Text = GetMessage("generic loading"); // TODO should be label w/o viewstate
            }

            AnalyticsSecurity.Guard(RequestInformationRef);
            InitializeDatePickers();
            ObtainValues();

            //if ("localhost" == RequestInformationRef.HostUrl)
            //{
            //    ltr_error.Text = GetMessage("err hostname could not be parsed");
            //    errGAMsg.Visible = true;
            //}

            // abort if error: // TODO: Tie datepicker into ASP.NET Validation?
            if (errGAMsg.Visible)
                return;

            Page.Validate();
            ComparisonTimeLineChart.Visible = Page.IsValid;
            SummaryCharts1.Visible = Page.IsValid;
            SummaryCharts2.Visible = Page.IsValid;

            if (Page.IsValid) {
				SummaryCharts1.ShowPageviews = true;
				SummaryCharts1.ShowUniqueViews = true;
				SummaryCharts1.ShowTimeOnPage = true;
				SummaryCharts1.ShowBounceRate = true;
				SummaryCharts1.ShowPercentExit = true;
				SummaryCharts2.ShowPageviews = true;
				SummaryCharts2.ShowUniqueViews = true;
				SummaryCharts2.ShowTimeOnPage = true;
				SummaryCharts2.ShowBounceRate = true;
				SummaryCharts2.ShowPercentExit = true;
                GetReports();
                PopulateGraphs();
            }
        }
        catch (Exception ex) {
            ltr_error.Text = ex.Message;
            errGAMsg.Visible = true;
            ComparisonTimeLineChart.Visible = false;
            SelectorFilterRow.Visible = false;
            CaptionRow.Visible = false;
            SummaryRow.Visible = false;
        }

    }

    public void BadDateFormatErrorHandler(string defaultMessage, string rawDate) {
        ltr_error.Text = defaultMessage; // TODO: Tie datepicker into ASP.NET Validation?
        errGAMsg.Visible = true;
    }

    public void BadDateRangeErrorHandler(object sender, BadDateRangeEventArgs e)
    {
        ltr_error.Text = e.Message;
        errGAMsg.Visible = true;
    }


    protected void InitializeDatePickers() {
        DateRangePicker1.BadDateRange += BadDateRangeErrorHandler;
        DateRangePicker1.BadStartDateFormatErrorHandler += BadDateFormatErrorHandler;
        DateRangePicker1.BadEndDateFormatErrorHandler += BadDateFormatErrorHandler;
        DateRangePicker1.BadStartDateFormatMessage = GetMessage("msg bad start date format");
        DateRangePicker1.BadEndDateFormatMessage = GetMessage("msg bad end date format");
        DateRangePicker2.BadDateRange += BadDateRangeErrorHandler;
        DateRangePicker2.BadStartDateFormatErrorHandler += BadDateFormatErrorHandler;
        DateRangePicker2.BadEndDateFormatErrorHandler += BadDateFormatErrorHandler;
        DateRangePicker2.BadStartDateFormatMessage = GetMessage("msg bad start date format");
        DateRangePicker2.BadEndDateFormatMessage = GetMessage("msg bad end date format");
        DateRangePicker1.MaximumDate = DateTime.Today;
        DateRangePicker2.MaximumDate = DateTime.Today;
    }

    protected void ObtainValues() {
        if (!IsPostBack) {
            VersionCompare comparisonDates;
            if (Request.QueryString["itemid"] != null) {
                itemId = EkFunctions.ReadDbLong(Request.QueryString["itemid"]);
                if (Request.QueryString["oldid"] != null)
                    versionId1 = EkFunctions.ReadDbLong(Request.QueryString["oldid"]);
                if (Request.QueryString["diff"] != null)
                    versionId2 = EkFunctions.ReadDbLong(Request.QueryString["diff"]);

                if (Request.QueryString["oldver"] != null)
                    version1 = EkFunctions.ReadDbString(Request.QueryString["oldver"]);
                if (Request.QueryString["ver"] != null)
                    version2 = EkFunctions.ReadDbString(Request.QueryString["ver"]);

                string versionNum = GetMessage("lbl version number");
                Caption1.Text = String.Format(versionNum, version1);
                Caption2.Text = String.Format(versionNum, version2);

                comparisonDates = dataManager.GetVersionDates(itemId, versionId1, versionId2);

                string title = String.Format(GetMessage("lbl compare analytics for"), comparisonDates.ContentTitle);
                Page.Title = title;
                this.SetTitleBarToString(title);
            } else {
                // for debugging when itemid is not available
                comparisonDates = new VersionCompare("dummy", new DateTime(2009, 7, 15), new DateTime(2009, 7, 25), new DateTime(2009, 7, 26), new DateTime(2009, 8, 5));
            }

            DateRangePicker1.StartDate = comparisonDates.BaseVersion.StartDate;
            DateRangePicker1.EndDate = comparisonDates.BaseVersion.EndDate;
            DateRangePicker2.StartDate = comparisonDates.ComparisonVersion.StartDate;
            DateRangePicker2.EndDate = comparisonDates.ComparisonVersion.EndDate;
        }
            _startDate1 = DateRangePicker1.StartDate;
        _endDate1 = DateRangePicker1.EndDate;
        _startDate2 = DateRangePicker2.StartDate;
        _endDate2 = DateRangePicker2.EndDate;
        _provider = SiteSelect.SelectedText;
    }

    protected void GetReports() {
        AnalyticsCriteria criteria = new AnalyticsCriteria();
        criteria.DimensionFilters.Condition = LogicalOperation.Or;
        foreach (string pagePath in UrlFilter.PagePaths)
        {
            criteria.DimensionFilters.AddFilter(Dimension.pagePath, DimensionFilterOperator.EqualTo, pagePath);
        }
        try {
            if (_provider != "" && dataManager.HasProvider(_provider))
            {
                report1 = dataManager.GetContentDetail(_provider, _startDate1, _endDate1, criteria);
                report2 = dataManager.GetContentDetail(_provider, _startDate2, _endDate2, criteria);
            }
            else
            {
                report1 = new AnalyticsReportData();
                report2 = new AnalyticsReportData();
            }
        }
        catch (Exception ex) {
            if (ex.Message.Contains("(401)")) {
                errDataManager = GetMessage("err analytics data provider");
            } else {
                errDataManager = ex.Message;
            }
            throw new Exception(errDataManager);
        }
    }

    protected void PopulateGraphs() {
        if (!IsPostBack) {
            MetricSelector.Items = new ListItem[] 
			{
				new ListItem(GetMessage("lbl pageviews"), Metrics.Pageviews.ToString()),
				new ListItem(GetMessage("lbl unique views"), Metrics.UniqueViews.ToString()),
				new ListItem(GetMessage("lbl time on page"), Metrics.TimeOnPage.ToString()),
				new ListItem(GetMessage("lbl bounce rate"), Metrics.BounceRate.ToString()),
				new ListItem(GetMessage("lbl percent exit"), Metrics.PercentExit.ToString())
			};
        }
        Metrics metric;
        string strMetric = MetricSelector.SelectedValue;
        if (Enum.IsDefined(typeof(Metrics), strMetric)) {
            metric = (Metrics)Enum.Parse(typeof(Metrics), strMetric);
        } else {
            metric = Metrics.Pageviews;
        }

        List<int>[] pageViewsOverTime = { new List<int>(), new List<int>() };
        List<int>[] uniqueViewsOverTime = { new List<int>(), new List<int>() };
        List<TimeSpan>[] averageTimeOnPageOverTime = { new List<TimeSpan>(), new List<TimeSpan>() };
        List<float>[] bounceRateOverTime = { new List<float>(), new List<float>() };
        List<float>[] percentExitOverTime = { new List<float>(), new List<float>() };

        int[] days = { (int)(_endDate1 - _startDate1).TotalDays + 1,
					 (int)(_endDate2 - _startDate2).TotalDays + 1};
        DateTime[] reportStartDates = {_startDate1,
						  _startDate2};
        AnalyticsReportData[] reports = { report1, report2 };

        for (int iReport = 0; iReport < 2; iReport++) {
            DateTime date = reportStartDates[iReport];
            AnalyticsReportData report = reports[iReport];

            for (int i = 0; i < days[iReport]; i++) {
                switch (metric) {
                    case Metrics.Pageviews:
                        pageViewsOverTime[iReport].Add(report.DayPageViews(date));
                        break;
                    case Metrics.UniqueViews:
                        uniqueViewsOverTime[iReport].Add(report.DayUniqueViews(date));
                        break;
                    case Metrics.TimeOnPage:
                        averageTimeOnPageOverTime[iReport].Add(report.DayAverageTimeSpanOnPage(date));
                        break;
                    case Metrics.BounceRate:
                        bounceRateOverTime[iReport].Add(report.DayBounceRate(date));
                        break;
                    case Metrics.PercentExit:
                        percentExitOverTime[iReport].Add(report.DayExitRate(date));
                        break;
                }
                date = date.AddDays(1);
            }
        }

        ComparisonTimeLineChart.BriefDescription = GetMessage("lbl time line chart");
        ComparisonTimeLineChart.TimeUnitInterval = controls_reports_TimeLineChart.TimeUnit.Day;
        switch (metric) {
            case Metrics.Pageviews:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], pageViewsOverTime[0], reportStartDates[1], pageViewsOverTime[1]);
                break;
            case Metrics.UniqueViews:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], uniqueViewsOverTime[0], reportStartDates[1], uniqueViewsOverTime[1]);
                break;
            case Metrics.TimeOnPage:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], averageTimeOnPageOverTime[0], reportStartDates[1], averageTimeOnPageOverTime[1]);
                break;
            case Metrics.BounceRate:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], bounceRateOverTime[0], reportStartDates[1], bounceRateOverTime[1]);
                break;
            case Metrics.PercentExit:
                ComparisonTimeLineChart.LoadSplitData(reportStartDates[0], percentExitOverTime[0], reportStartDates[1], percentExitOverTime[1]);
                break;
        }

        //SummaryTitle1.Text = String.Format("{0:D} - {1:D}", _startDate1, _endDate1);
        //SummaryTitle2.Text = String.Format("{0:D} - {1:D}", _startDate2, _endDate2);

        SummaryCharts1.StartDate = _startDate1;
        SummaryCharts1.EndDate = _endDate1;
        SummaryCharts1.Report = report1;
        SummaryCharts1.PopulateData();

        SummaryCharts2.StartDate = _startDate2;
        SummaryCharts2.EndDate = _endDate2;
        SummaryCharts2.Report = report2;
        SummaryCharts2.PopulateData();

    }
}
