using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Analytics_Reporting_Trends : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        AnalyticsTrend.TitleChangedEventHandler += TitleChangedHandler;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterResource();
        AnalyticsTrend.Visible = true;
		AnalyticsTrend.ProviderName = AnalyticsToolbar.ProviderName;
		AnalyticsTrend.StartDate = AnalyticsToolbar.StartDate;
		AnalyticsTrend.EndDate = AnalyticsToolbar.EndDate;
		string report = Request.QueryString["report"];
        if (!String.IsNullOrEmpty(report) && false == ErrorPanel.Visible)
        {
            AnalyticsTrend.Report = (Analytics_reporting_Trend.ReportType)Enum.Parse(typeof(Analytics_reporting_Trend.ReportType), report, true); // ignore case
        }
        else
        {
            AnalyticsTrend.Visible = false;
        }
        if (!IsPostBack)
        {
            litLoadingMessage.Text = GetMessage("generic loading"); // TODO should be label w/o viewstate
        }
	}

    protected void RegisterResource()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }

	protected void DateFormatError(object sender, Analytics_controls_Toolbar_EventArgs e)
	{
		litErrorMessage.Text = e.Message;
		ErrorPanel.Visible = true;
	}

	protected void TitleChangedHandler(object sender, Analytics_reporting_Trend.TitleChangedEventArgs e)
	{
        litTitle.Text = Title = e.Title;
    }
}
