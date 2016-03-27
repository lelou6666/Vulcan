using System;
public partial class ContentDesigner_resourcexml : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
	protected void Page_PreInit(object sender, EventArgs e)
	{
		Page.Theme = ""; // EnableTheming="false" in Page directive has no effect.
	}
	protected void Page_Load(object sender, EventArgs e)
    {
		AssertInternalReferrer();
		//<data name="id" xml:space="preserve">
		//    <value>text</value>
		//</data>

		int lcid = 0;
		Int32.TryParse(Request.QueryString.Get("LangType"), out lcid);
		if (0 == lcid)
		{
			try
			{
				Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();
				lcid = api.UserLanguage;
			}
			catch { }
		}
		if (lcid != 0)
		{
			try
			{
				System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lcid);
			}
			catch { }
		}

		string strResourceName = Request.QueryString.Get("name");
		if (strResourceName != null)
		{
			string strResourceId = Request.QueryString.Get("id");
			if (strResourceId != null)
			{
				string strText = (String)GetGlobalResourceObject(strResourceName, strResourceId);
				litOutput.Text = String.Format(@"<data name=""{0}"" xml:space=""preserve""><value>{1}</value></data>", strResourceId, strText);
			}
			else
			{
				// get all
				string strGlobalResources = Server.MapPath("~/App_GlobalResources/ContentDesigner/");
				string xsltfile = Server.MapPath(ResolveUrl("resxdata.xslt"));
				string strLang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
				string xmlfile = strGlobalResources + strResourceName + "." + strLang + ".resx";
				if (!System.IO.File.Exists(xmlfile))
				{
					strLang = System.Threading.Thread.CurrentThread.CurrentUICulture.Parent.Name;
					xmlfile = strGlobalResources + strResourceName + "." + strLang + ".resx";
					if (!System.IO.File.Exists(xmlfile))
					{
						xmlfile = strGlobalResources + strResourceName + ".resx";
					}
				}
				string strXml = Ektron.Cms.EkXml.XSLTransform(xmlfile, xsltfile, true, true, null, true, null);
				litOutput.Text = strXml;
			}
		}
    }
}
