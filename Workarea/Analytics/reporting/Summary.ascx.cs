using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Analytics;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Workarea.Reports;

public partial class Analytics_reporting_Summary : WorkareaBaseControl
{

    private AnalyticsReportData _report = new AnalyticsReportData();
    public AnalyticsReportData Report
    {
        get { return _report; }
        set { _report = value; }
    }

	private bool _showVisits;
	public bool ShowVisits
	{
		get { return _showVisits; }
		set { _showVisits = value; }
	}
	private bool _showPagesPerVisit;
	public bool ShowPagesPerVisit
	{
		get { return _showPagesPerVisit; }
		set { _showPagesPerVisit = value; }
	}
	private bool _showPageviews;
	public bool ShowPageviews
	{
		get { return _showPageviews; }
		set { _showPageviews = value; }
	}
	private bool _showUniqueViews;
	public bool ShowUniqueViews
	{
		get { return _showUniqueViews; }
		set { _showUniqueViews = value; }
	}
	private bool _showTimeOnSite;
	public bool ShowTimeOnSite
	{
		get { return _showTimeOnSite; }
		set { _showTimeOnSite = value; }
	}
	private bool _showTimeOnPage;
	public bool ShowTimeOnPage
	{
		get { return _showTimeOnPage; }
		set { _showTimeOnPage = value; }
	}
	//private bool _showPercentNewVisits;
	//public bool ShowPercentNewVisits
	//{
	//    get { return _showPercentNewVisits; }
	//    set { _showPercentNewVisits = value; }
	//}
	private bool _showBounceRate;
	public bool ShowBounceRate
	{
		get { return _showBounceRate; }
		set { _showBounceRate = value; }
	}
	private bool _showPercentExit;
	public bool ShowPercentExit
	{
		get { return _showPercentExit; }
		set { _showPercentExit = value; }
	}

	private DateTime _startDate = DateTime.MinValue;
    public DateTime StartDate
    {
        get { return _startDate; }
        set { _startDate = value; }
    }
    private DateTime _endDate = DateTime.Now;
    public DateTime EndDate 
    {
        get { return _endDate; }
        set { _endDate = value; }
    }

	private const int chartWidth = 75;
	private const int chartHeight = 18;

    public void PopulateData()
    {
		List<int> visitsOverTime = new List<int>();
		List<double> pagesPerVisitOverTime = new List<double>();
		List<int> pageViewsOverTime = new List<int>();
		List<int> uniqueViewsOverTime = new List<int>();
		List<TimeSpan> averageTimeOnSiteOverTime = new List<TimeSpan>();
		List<TimeSpan> averageTimeOnPageOverTime = new List<TimeSpan>();
		//List<float> percentNewVisitsOverTime = new List<float>();
		List<float> bounceRateOverTime = new List<float>();
		List<float> percentExitOverTime = new List<float>();

		int numDays = EndDate.Subtract(StartDate).Days + 1;
		int sampleRate = (numDays > 31 ? (numDays / 31) : 1);
		DateTime date = StartDate;

		for (int i = 0; i < numDays; i += sampleRate)
		{
			if (_showVisits) visitsOverTime.Add(_report.DayVisits(date));
			if (_showPagesPerVisit) pagesPerVisitOverTime.Add(_report.DayPagesPerVisit(date));
			if (_showPageviews) pageViewsOverTime.Add(_report.DayPageViews(date));
			if (_showUniqueViews) uniqueViewsOverTime.Add(_report.DayUniqueViews(date));
			if (_showTimeOnSite) averageTimeOnSiteOverTime.Add(_report.DayAverageTimeSpanOnSite(date));
			if (_showTimeOnPage) averageTimeOnPageOverTime.Add(_report.DayAverageTimeSpanOnPage(date));
			//if (_showPercentNewVisits) percentNewVisitsOverTime.Add(_report. TBD (date));
			if (_showBounceRate) bounceRateOverTime.Add(_report.DayBounceRate(date));
			if (_showPercentExit) percentExitOverTime.Add(_report.DayExitRate(date));

			date = date.AddDays(sampleRate);
		}

		string strPerDay = GetMessage("lbl avg per day");
		ChartData data = null;

		if (_showVisits)
		{
			data = ChartData.CreateChartData(visitsOverTime);
			imgVisits.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
			litVisits.Text = _report.TotalVisits.ToString("#,##0");
			litVisitsPerDay.Text = String.Format(strPerDay, (double)_report.TotalVisits / (double)numDays);
			lblVisits.Text = GetMessage("lbl visits");
			rowVisits.Visible = true;
		}
		if (_showPagesPerVisit)
		{
			data = ChartData.CreateChartData(pagesPerVisitOverTime);
			imgPagesPerVisit.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
			litPagesPerVisit.Text = _report.TotalPagesPerVisit.ToString("0.00");
			lblPagesPerVisit.Text = GetMessage("lbl pages visit");
			rowPagesPerVisit.Visible = true;
		}
		if (_showPageviews)
		{
			data = ChartData.CreateChartData(pageViewsOverTime);
			img_pageviews.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
			ltr_pageviews.Text = _report.TotalPageViews.ToString("#,##0");
			ltr_pageviewsperday.Text = String.Format(strPerDay, (double)_report.TotalPageViews / (double)numDays);
			lblPageviews.Text = GetMessage("lbl pageviews");
			rowPageviews.Visible = true;
		}
		if (_showUniqueViews)
		{
			data = ChartData.CreateChartData(uniqueViewsOverTime);
			img_uniqueviews.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
			ltr_uniqueviews.Text = _report.TotalUniqueViews.ToString("#,##0");
			ltr_uniqueviewsperday.Text = String.Format(strPerDay, (double)_report.TotalUniqueViews / (double)numDays);
			lblUniqueViews.Text = GetMessage("lbl unique views");
			rowUniqueViews.Visible = true;
		}
		if (_showTimeOnSite)
		{
			data = ChartData.CreateChartData(averageTimeOnSiteOverTime);
			imgTimeOnSite.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
			litTimeOnSite.Text = _report.TotalAverageTimeSpanOnSite.ToString();
			lblTimeOnSite.Text = GetMessage("lbl time on site");
			rowTimeOnSite.Visible = true;
		}
		if (_showTimeOnPage)
		{
			data = ChartData.CreateChartData(averageTimeOnPageOverTime);
			img_timeonpage.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
			ltr_timeonpage.Text = _report.TotalAverageTimeSpanOnPage.ToString();
			lblTimeOnPage.Text = GetMessage("lbl time on page");
			rowTimeOnPage.Visible = true;
		}
		//if (_showPercentNewVisits)
		//{
		//    data = ChartData.CreateChartData(percentNewVisitsOverTime);
		//    imgPercentNewVisits.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
		//    litPercentNewVisits.Text = _report.TotalPercentNewVisits.ToString("0.00%");
		//    lblPercentNewVisits.Text = GetMessage("lbl percent new visits");
		//    rowPercentNewVisits.Visible = true;
		//}
		if (_showBounceRate)
		{
			data = ChartData.CreateChartData(bounceRateOverTime);
			img_bouncerate.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
			ltr_bouncerate.Text = _report.TotalBounceRate.ToString("0.00%");
			lblBounceRate.Text = GetMessage("lbl bounce rate");
			rowBounceRate.Visible = true;
		}
		if (_showPercentExit)
		{
			data = ChartData.CreateChartData(percentExitOverTime);
			img_percentexit.ImageUrl = GoogleChartUrl(chartWidth, chartHeight, data.EncodeGoogleChartData(data.Max()));
			ltr_percentexit.Text = _report.TotalExitRate.ToString("0.00%");
			lblPercentExit.Text = GetMessage("lbl percent exit");
			rowPercentExit.Visible = true;
		}
	}

    private string GoogleChartUrl(int width, int height, string simpleEncodedDataSet)
    {
		if (String.IsNullOrEmpty(simpleEncodedDataSet)) simpleEncodedDataSet = "AA"; // flatline zero
        return this.GoogleChartBaseUrl + "?cht=ls&chs=" + width + "x" + height + "&chm=B,e6f2fa,0,0,0&chco=0077cc&chd=s:" + simpleEncodedDataSet;
    }
}
