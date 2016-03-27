using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Analytics_Reporting_Reports : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        AnalyticsReport.TitleChangedEventHandler += TitleChangedHandler;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterResource();
        AnalyticsReport.Visible = true;
		AnalyticsReport.ProviderName = AnalyticsToolbar.ProviderName;
		AnalyticsReport.StartDate = AnalyticsToolbar.StartDate;
		AnalyticsReport.EndDate = AnalyticsToolbar.EndDate;
		AnalyticsReport.LoadStateFromQueryString(Request.QueryString);

        Page.Validate();
        if (!Page.IsValid || true == ErrorPanel.Visible)
        {
            AnalyticsReport.Visible = false;
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
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
    }

    protected void DateFormatError(object sender, Analytics_controls_Toolbar_EventArgs e)
	{
		litErrorMessage.Text = e.Message;
		ErrorPanel.Visible = true;
	}

    protected void TitleChangedHandler(object sender, Analytics_reporting_Report.TitleChangedEventArgs e){
        litTitle.Text = Title = e.Title;
    }
}
