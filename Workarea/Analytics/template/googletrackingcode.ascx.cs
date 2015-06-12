using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.Common;
//using Ektron.Cms.Analytics;

public partial class Analytics_Template_GoogleTrackingCode : System.Web.UI.UserControl
{
    private string _useracct = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        string pagehost = Page.Request["HTTP_HOST"];
        Ektron.Cms.Analytics.IAnalytics dataManager = ObjectFactory.GetAnalytics();
        List<Ektron.Cms.Analytics.BeaconData> beacons = dataManager.GetBeaconData(pagehost);
        foreach (Ektron.Cms.Analytics.BeaconData beacon in beacons)
        {
            _useracct = beacon.UserAccount;
            if (_useracct.Length > 0)
            {
                this.GoogleUserAccount.Text = _useracct;
            }
        }
    }
}