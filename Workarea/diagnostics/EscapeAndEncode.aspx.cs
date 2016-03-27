using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class EscapeAndEncode : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
    }

	protected delegate string EscapeFunction(string s);

	protected string applyEscape(EscapeFunction f, string s)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		for (int i = 0; i < s.Length; i++)
		{
			string c = s[i].ToString();
			string S = f(c);
			sb.Append(S.PadLeft(s[i].CompareTo('\x7f') > 1 ? 10 : 7, '\xa0'));
			//sb.Append(S);
		}
		return sb.ToString();
	}

}
