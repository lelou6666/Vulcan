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

public partial class Analytics_reporting_Detail : WorkareaBaseControl
{

	#region Private Members

	private DateTime _startDate = DateTime.Today.AddDays(-1).AddDays(-30);
	private DateTime _endDate = DateTime.Today.AddDays(-1); // today is a partial day

	private enum DisplayMetric
	{
		Visits,
		PagesPerVisit,
		Pageviews,
		UniqueViews,
		TimeOnSite,
		TimeOnPage,
		//PercentNewVisits,
		BounceRate,
		PercentExit
	}

	#endregion

	public delegate void SelectionChangedHandler(object sender, EventArgs e);
	public event SelectionChangedHandler SelectionChanged;

	#region Properties

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

	public bool ShowSummaryChart
	{
		get { return AnalyticsSummary.Visible; }
		set { AnalyticsSummary.Visible = value; }
	}
	public bool ShowLineChart
	{
		get { return pnlChart.Visible; }
		set { MetricSelector.Visible = pnlChart.Visible = value; }
	}
	public Unit LineChartWidth
	{
		get { return AnalyticsLineChart.Width; }
		set { AnalyticsLineChart.Width = value; }
	}
	public Unit LineChartHeight
	{
		get { return AnalyticsLineChart.Height; }
		set { AnalyticsLineChart.Height = value; }
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

	#endregion


	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			RegisterScripts();
		}

		// TODO check permissions
		//if (!IsCommerceAdmin)
		//{
		//    // TODO display message GetMessage("err not role commerce-admin");
		//    return;
		//}

		ltrlNoRecords.Text = GetMessage("lbl no records");
	}

	public void UpdatePageUrl(string url)
	{
		hyp_visitpage.Text = GetMessage("lbl visit this page");
		//lblAnalyzing.Text = GetMessage("lbl analyzing");
		//drp_analyze.Items[0].Text = GetMessage("opt content detail");
		lblVisitThisPage.Text = GetMessage("lbl visit this page");

		hyp_visitpage.NavigateUrl = url;
		hyp_visitpage.Attributes.Add("title", url);

		img_visitpage.ImageUrl = CommonApi.RequestInformationRef.AppImgPath + "../UI/Icons/linkGo.png";
		img_visitpage.AlternateText = hyp_visitpage.Text;
		img_visitpage.Attributes.Add("title", img_visitpage.AlternateText);
		img_visitpage.Visible = true;

		VisitPageArea.Visible = true;
	}

	public void UpdateReport(AnalyticsReportData report)
	{
		ltr_viewed.Text = String.Format(GetMessage("lbl page viewed times"), report.TotalPageViews);

		AnalyticsSummary.ShowVisits = _showVisits;
		AnalyticsSummary.ShowPagesPerVisit = _showPagesPerVisit;
		AnalyticsSummary.ShowPageviews = _showPageviews;
		AnalyticsSummary.ShowUniqueViews = _showUniqueViews;
		AnalyticsSummary.ShowTimeOnSite = _showTimeOnSite;
		AnalyticsSummary.ShowTimeOnPage = _showTimeOnPage;
//		AnalyticsSummary.ShowPercentNewVisits = _showPercentNewVisits;
		AnalyticsSummary.ShowBounceRate = _showBounceRate;
		AnalyticsSummary.ShowPercentExit = _showPercentExit;

		AnalyticsSummary.Report = report;
		AnalyticsSummary.StartDate = _startDate;
		AnalyticsSummary.EndDate = _endDate;
		AnalyticsSummary.PopulateData();

		//lblTrafficDateRange.Text = String.Format("{0:D} - {1:D}", _startDate, _endDate);

		if (String.IsNullOrEmpty(MetricSelector.SelectedValue))
		{
			List<ListItem> metricItems = new List<ListItem>();
			if (_showVisits) metricItems.Add(new ListItem(GetMessage("lbl visits"), DisplayMetric.Visits.ToString()));
			if (_showPagesPerVisit) metricItems.Add(new ListItem(GetMessage("lbl pages visit"), DisplayMetric.PagesPerVisit.ToString()));
			if (_showPageviews) metricItems.Add(new ListItem(GetMessage("lbl pageviews"), DisplayMetric.Pageviews.ToString()));
			if (_showUniqueViews) metricItems.Add(new ListItem(GetMessage("lbl unique views"), DisplayMetric.UniqueViews.ToString()));
			if (_showTimeOnSite) metricItems.Add(new ListItem(GetMessage("lbl time on site"), DisplayMetric.TimeOnSite.ToString()));
			if (_showTimeOnPage) metricItems.Add(new ListItem(GetMessage("lbl time on page"), DisplayMetric.TimeOnPage.ToString()));
			//if (_showPercentNewVisits) metricItems.Add();
			if (_showBounceRate) metricItems.Add(new ListItem(GetMessage("lbl bounce rate"), DisplayMetric.BounceRate.ToString()));
			if (_showPercentExit) metricItems.Add(new ListItem(GetMessage("lbl percent exit"), DisplayMetric.PercentExit.ToString()));
			MetricSelector.Items = metricItems.ToArray();
		}

		DisplayMetric[] metric = new DisplayMetric[1];
		string strMetric = MetricSelector.SelectedValue;
		if (Enum.IsDefined(typeof(DisplayMetric), strMetric))
		{
			metric[0] = (DisplayMetric)Enum.Parse(typeof(DisplayMetric), strMetric);
		}
		else if (_showVisits)
		{
			metric[0] = DisplayMetric.Visits;
		}
		else if (_showPageviews)
		{
			metric[0] = DisplayMetric.Pageviews;
		}
		string strMetric2 = MetricSelector.SelectedValue2;
		if (!string.IsNullOrEmpty(strMetric2) && strMetric != strMetric2)
		{
			if (Enum.IsDefined(typeof(DisplayMetric), strMetric2))
			{
				Array.Resize(ref metric, 2);
				metric[1] = (DisplayMetric)Enum.Parse(typeof(DisplayMetric), strMetric2);
			}
		}

		List<int> visitsOverTime = new List<int>();
		List<double> pagesPerVisitOverTime = new List<double>();
		List<int> pageViewsOverTime = new List<int>();
		List<int> uniqueViewsOverTime = new List<int>();
		List<TimeSpan> averageTimeOnSiteOverTime = new List<TimeSpan>();
		List<TimeSpan> averageTimeOnPageOverTime = new List<TimeSpan>();
		List<float> bounceRateOverTime = new List<float>();
		List<float> percentExitOverTime = new List<float>();

		int days = (int)(_endDate - _startDate).TotalDays + 1;

		for (int iMetric = 0; iMetric <= metric.Length - 1; iMetric++)
		{
			DateTime date = _startDate;

			for (int i = 0; i <= days - 1; i++)
			{
				switch (metric[iMetric])
				{
					case DisplayMetric.Visits:
						visitsOverTime.Add(report.DayVisits(date));
						break;

					case DisplayMetric.PagesPerVisit:
						pagesPerVisitOverTime.Add(report.DayPagesPerVisit(date));
						break;

					case DisplayMetric.Pageviews:
						pageViewsOverTime.Add(report.DayPageViews(date));
						break;

					case DisplayMetric.UniqueViews:
						uniqueViewsOverTime.Add(report.DayUniqueViews(date));
						break;

					case DisplayMetric.TimeOnSite:
						averageTimeOnSiteOverTime.Add(report.DayAverageTimeSpanOnSite(date));
						break;

					case DisplayMetric.TimeOnPage:
						averageTimeOnPageOverTime.Add(report.DayAverageTimeSpanOnPage(date));
						break;

					case DisplayMetric.BounceRate:
						bounceRateOverTime.Add(report.DayBounceRate(date));
						break;

					case DisplayMetric.PercentExit:
						percentExitOverTime.Add(report.DayExitRate(date));
						break;
					default:
						throw new ArgumentOutOfRangeException("metric[iMetric]", "Unknown DetailMetric: " + metric[iMetric]);
				}
				date = date.AddDays(1);
			}
		}

		AnalyticsLineChart.BriefDescription = GetMessage("lbl time line chart");
		AnalyticsLineChart.TimeUnitInterval = controls_reports_TimeLineChart.TimeUnit.Day;
		switch (metric[0])
		{
			case DisplayMetric.Visits:
				AnalyticsLineChart.LoadData(_startDate, visitsOverTime);
				break;

			case DisplayMetric.PagesPerVisit:
				AnalyticsLineChart.LoadData(_startDate, pagesPerVisitOverTime);
				break;

			case DisplayMetric.Pageviews:
				AnalyticsLineChart.LoadData(_startDate, pageViewsOverTime);
				break;

			case DisplayMetric.UniqueViews:
				AnalyticsLineChart.LoadData(_startDate, uniqueViewsOverTime);
				break;

			case DisplayMetric.TimeOnSite:
				AnalyticsLineChart.LoadData(_startDate, averageTimeOnSiteOverTime);
				break;

			case DisplayMetric.TimeOnPage:
				AnalyticsLineChart.LoadData(_startDate, averageTimeOnPageOverTime);
				break;

			case DisplayMetric.BounceRate:
				AnalyticsLineChart.LoadData(_startDate, bounceRateOverTime);
				break;

			case DisplayMetric.PercentExit:
				AnalyticsLineChart.LoadData(_startDate, percentExitOverTime);
				break;
			default:
				throw new ArgumentOutOfRangeException("metric[0]", "Unknown DetailMetric: " + metric[0]);
		}

		if ((metric.Length > 1))
		{
			switch (metric[1])
			{
				case DisplayMetric.Visits:
					AnalyticsLineChart.LoadData2(visitsOverTime);
					break;

				case DisplayMetric.PagesPerVisit:
					AnalyticsLineChart.LoadData2(pagesPerVisitOverTime);
					break;

				case DisplayMetric.Pageviews:
					AnalyticsLineChart.LoadData2(pageViewsOverTime);
					break;

				case DisplayMetric.UniqueViews:
					AnalyticsLineChart.LoadData2(uniqueViewsOverTime);
					break;

				case DisplayMetric.TimeOnSite:
					AnalyticsLineChart.LoadData2(averageTimeOnSiteOverTime);
					break;

				case DisplayMetric.TimeOnPage:
					AnalyticsLineChart.LoadData2(averageTimeOnPageOverTime);
					break;

				case DisplayMetric.BounceRate:
					AnalyticsLineChart.LoadData2(bounceRateOverTime);
					break;

				case DisplayMetric.PercentExit:
					AnalyticsLineChart.LoadData2(percentExitOverTime);
					break;
				default:
					throw new ArgumentOutOfRangeException("metric[1]", "Unknown DetailMetric: " + metric[1]);
			}
		}
	}

	protected virtual void MetricSelector_SelectionChanged(object sender, EventArgs e)
	{
		if (SelectionChanged != null)
		{
			SelectionChanged(this, e);
		}
	}

	public void RegisterScripts()
	{
		JS.RegisterJS(this, JS.ManagedScript.EktronJS);
	}

}


