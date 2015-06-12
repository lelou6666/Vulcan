using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Personalization;
using Ektron.Cms.Widget;

public partial class Analytics_reporting_Overview : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterResource();
        litTitle.Text = GetMessage("sam overview");

        AnalyticsReport1.Visible = true;
        AnalyticsReport1.ProviderName = AnalyticsToolbar.ProviderName;
		AnalyticsReport1.StartDate = AnalyticsToolbar.StartDate;
		AnalyticsReport1.EndDate = AnalyticsToolbar.EndDate;

        AnalyticsReport2.Visible = true;
        AnalyticsReport2.ProviderName = AnalyticsToolbar.ProviderName;
		AnalyticsReport2.StartDate = AnalyticsToolbar.StartDate;
		AnalyticsReport2.EndDate = AnalyticsToolbar.EndDate;

        Page.Validate();
        if (!Page.IsValid || true == ErrorPanel.Visible)
        {
            AnalyticsReport1.Visible = false;
            AnalyticsReport2.Visible = false;
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

}
