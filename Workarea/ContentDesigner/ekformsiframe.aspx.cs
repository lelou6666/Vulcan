using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

public partial class Workarea_ContentDesigner_ekformsiframe : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
		AssertInternalReferrer();
        string filepath = string.Empty;
        System.Text.StringBuilder StyleSheets = new System.Text.StringBuilder();
        StyleSheets.Append(Environment.NewLine);
		//if (Request.QueryString["skin"] != null)
		//{
		//    filepath = Request.QueryString["skin"];
		//    ValidateQueryString(filepath);
		//    StyleSheets.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + filepath + "\" />");
		//}
		if (Request.QueryString["eca"] != null)
		{
            filepath = Request.QueryString["eca"];
            ValidateQueryString(filepath);
            StyleSheets.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + filepath + "\" />");
		}
		foreach (string p in Request.QueryString.AllKeys)
		{
			if (!String.IsNullOrEmpty(p))
			{
				if (p.StartsWith("css") && Request.QueryString[p] != null)
				{
                    filepath = Request.QueryString[p];
                    ValidateQueryString(filepath);
                    StyleSheets.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + filepath + "\" title=\"" + filepath + "\" />");
				}
			}
		}
		StyleSheets.AppendLine();
        cssLinks.Text = StyleSheets.ToString();
		
		string BaseUrl = Request.Url.AbsoluteUri;
		int pPath = BaseUrl.IndexOf(Request.Url.AbsolutePath);
		BaseUrl = BaseUrl.Remove(pPath + 1); // Example: http://my.domain.com/
		// create BASE as literal otherwise runat=server will use long notation with closing tag
		litBase.Text = String.Format("<base href=\"{0}\" />", BaseUrl); 
		
		if (Request.QueryString["id"] != null)
		{
			string id = Request.QueryString["id"];
            ValidateQueryString(id);
            theBody.ID = id;
		}
        if (Request.QueryString["height"] != null)
        {
            string ht = Request.QueryString["height"];
            ValidateQueryString(ht);
            theBody.Style.Add("height", ht);
            theHtmlTag.Style.Add("height", ht);
        }

		Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronSmartFormCss);

		if (null == Request.QueryString["js"])
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronAutoheightJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
			Ektron.Cms.API.JS.RegisterJS(this, (new Ektron.Cms.CommonApi()).AppPath + "ContentDesigner/ekxbrowser.js", "EkXBrowser");
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronSmartFormJS);
		}
	}

    private void ValidateQueryString(string queryString)
    {
        queryString = queryString.ToUpper();
        queryString = Regex.Replace(queryString, @"\/\*[\w\W]*?\*\/", "");
        if ((queryString.IndexOf("<") > -1) || (queryString.IndexOf("%3C") > -1) || (queryString.IndexOf(">") > -1) || (queryString.IndexOf("%3E") > -1) || (queryString.IndexOf("\"") > -1) || (queryString.IndexOf("%22") > -1) || (queryString.IndexOf(":EXPRESSION(") > -1) || (queryString.IndexOf("JAVASCRIPT:") > -1))
        {
            throw new ArgumentException("Invalid Query String Value");
        }
    }
}
