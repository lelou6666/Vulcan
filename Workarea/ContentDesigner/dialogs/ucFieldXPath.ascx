<%@ Control Language="C#" %>
<%@ Register TagPrefix="radcb" Namespace="Telerik.WebControls" Assembly="RadComboBox.NET2" %>
<%@ Register TagPrefix="ek" TagName="FieldTreeViewControl" Src="ucFieldTreeView.ascx" %>
<script runat="server">

    protected string ExamplesName = "CalculatedFieldExamples";
    protected string ExamplesXml = "../CalculatedFieldExamples.xml";
	
	private Ektron.Cms.CommonApi _api = null;

	private string _xpathfieldtype = "calculation";
    
    public string FieldType
    {
        get { return _xpathfieldtype; }
        set { _xpathfieldtype = value.ToLower(); }
    }
    
    private void BindComboBox()
    {
		string xmlPath = new Uri(Request.Url, ExamplesXml).AbsoluteUri;
        string xsltPath = new Uri(Request.Url, "../LocalizeComboBox.xslt").AbsoluteUri;
		Ektron.Cms.Xslt.ArgumentList objXsltArgs = new Ektron.Cms.Xslt.ArgumentList();
        objXsltArgs.AddParam("bExamples", "", true);
		objXsltArgs.AddParam("LangType", "", _api.UserLanguage);
		objXsltArgs.AddParam("localeUrl", "", "resourcexml.aspx?name=" + ExamplesName + "&LangType=" + _api.UserLanguage);
        string comboItems = Ektron.Cms.EkXml.XSLTransform(xmlPath, xsltPath, true, true, objXsltArgs, true, null);

        cboXPathEx.LoadXmlString(comboItems);
    }
    
    protected void Page_Load(Object src, EventArgs e)
    {
		_api = new Ektron.Cms.CommonApi();
		Ektron.Cms.Common.EkMessageHelper refMsg = _api.EkMsgRef;
		this.lblExamples.InnerHtml = refMsg.GetMessage("lbl examples c");
        string sConditionResourceText = "";

        if (!Page.IsPostBack)
        {

            if ("relevant" == _xpathfieldtype)
            {
                this.lblCondition.InnerHtml = refMsg.GetMessage("lbl condition");
                sConditionResourceText = refMsg.GetMessage("lbl condition nc");
            }
            else //"calculation"
            {
                this.lblCondition.InnerHtml = refMsg.GetMessage("lbl formula c");
                sConditionResourceText = refMsg.GetMessage("lbl condition nc");
            }
            ExamplesName = _xpathfieldtype + "Examples";
            ExamplesXml = "../" + ExamplesName + ".xml";
            BindComboBox();
        }
        
		if (!Page.ClientScript.IsClientScriptBlockRegistered("EkFieldXPathScript"))
		{
            string ScriptText = EkFieldXPathScript.InnerText;
            ScriptText = ScriptText.Replace("<%=this.ClientID%>", this.ClientID);
            ScriptText = ScriptText.Replace("<%=cboXPathEx.ClientID%>", cboXPathEx.ClientID);
            ScriptText = ScriptText.Replace("<%=xpathTree.fsTree.ClientID%>", xpathTree.fsTree.ClientID);
            ScriptText = ScriptText.Replace("<%=xpathTree.ClientID%>", xpathTree.ClientID);
            ScriptText = ScriptText.Replace("<%=ExamplesName%>", ExamplesName);
			
			StringBuilder sbScript = new StringBuilder();
			sbScript.AppendLine();
			sbScript.AppendLine(@"var EkFieldXPathResourceText = ");
			sbScript.AppendLine(@"{");
			sbScript.Append(@"	sExprContainsVars: """);
			sbScript.Append(refMsg.GetMessage("msg val expr contains vars"));
			sbScript.AppendLine(@"""");
			sbScript.Append(@",	sCondition: """);
            sbScript.Append(sConditionResourceText);
			sbScript.AppendLine(@"""");
            sbScript.Append(@",	sExprContainsVars: """);
            sbScript.Append(refMsg.GetMessage("msg expr contains vars"));
			sbScript.AppendLine(@"""");
            sbScript.Append(@",	sCalcFormulaReqd: """);
            sbScript.Append(refMsg.GetMessage("msg calc formula reqd"));
			sbScript.AppendLine(@"""");
			sbScript.AppendLine(@"};");
			sbScript.AppendLine(ScriptText);
        
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldXPathScript", sbScript.ToString(), true);
		}
        EkFieldXPathScript.Visible = false;
    }
    override protected void OnInit(EventArgs e)
    {
        InitializeComponent();
        base.OnInit(e);
    }

    private void InitializeComponent()
    {
        this.Load += new System.EventHandler(this.Page_Load);
    }
</script>
<clientscript id="EkFieldXPathScript" runat="server">
function EkFieldXPathControl(clientID)
{
    this.clientID = clientID;
    this.name = "";
    this.xpathName = "";
    this.customCalLang = "";
    this.customCalExpr = "";
    this.customValLang = "";
    this.configXML = null;
    this.contentElement = null;
    this.fieldElem = null;
    this.defaultErrorMessage = "";
    this.attributeName = "ektdesignns_calculate"; 
    this.init = function(contentElement, elemName)
    {
		this.contentElement = contentElement;
		this.attributeName = elemName;
    }
    this.read = function(oFieldElem)
    {
		this.fieldElem = oFieldElem;
        var fieldxpath = $ektron.htmlDecode(oFieldElem.getAttribute(this.attributeName));
        if ("xpathr:" == fieldxpath.substr(0,7))
        {
            document.getElementById("fieldXPath").value = fieldxpath.substr(7);
        }
        
    }
    this.update = function(oFieldElem)
    {
		this.fieldElem = oFieldElem;
        var strXPathExpr = document.getElementById("fieldXPath").value;
        strValExpr = m_xpath_ekXPathExpr.applyComparator(strXPathExpr, "string");
        oFieldElem.removeAttribute(this.attributeName);
        if (strXPathExpr.length > 0)
        {
            oFieldElem.setAttribute(this.attributeName, "xpathr:" + strXPathExpr);
        }
    }
    this.initXPathExpression = function()
    {
		m_xpath_ekXPathExpr.init("fieldXPath", EkFieldXPathResourceText.sCondition, this.contentElement, this.fieldElem, "&lt;%=xpathTree.ClientID%&gt;");
    }
    this.loadContentTree = function(contentTree)
    {
		m_xpath_ekXPathExpr.loadContentTree(contentTree);
    }
    this.SetComboDefaultText = function()
    {
        try
        {
            var comboBox = window[this.clientID + "_cboXPathEx"];
            comboBox.SetText(" ");
        }
        catch(e) {}
    }
}
var ekFieldXPathControl = new EkFieldXPathControl("&lt;%=this.ClientID%&gt;");
Ektron.ready(function()
{
    $ektron(document).bind("onValidateDialog", function(ev, oRet)
    {
        var errObj = null;
        var strText = document.getElementById("fieldXPath").value;
        var aFieldNameVariable = EkFormFields_GetVariableNames(strText);
        if (aFieldNameVariable && aFieldNameVariable.length > 0)
        {
            //Still contains a field name variable that needs to be replaced.
            var sVar = aFieldNameVariable.join(", ");
            errObj = 
            {
                name:       "&lt;%=ExamplesName%&gt;",
                message:    Ektron.String.format(EkFieldXPathResourceText.sExprContainsVars, sVar), 
                srcElement: document.getElementById("fieldXPath")
            };
        }
        if (!errObj && "calculationfield" == "&lt;%=ExamplesName%&gt;".toLowerCase())
        {
            if (0 == document.getElementById("fieldXPath").value.length)
            {
                var errObj = 
                {
                    name:       "&lt;%=ExamplesName%&gt;",
                    message:    EkFieldXPathResourceText.sCalcFormulaReqd, 
                    srcElement: document.getElementById("fieldXPath")
                };
            }
        }
        if (errObj)
        {
            oRet.push(errObj);
        }
    });
});
</clientscript>
<script language="javascript" type="text/javascript">
<!--
    var m_xpath_ekXPathExpr = new Ektron.XPathExpression();
    function cboXPathEx_OnClientSelectedIndexChanged(item)
    {
        document.getElementById("fieldXPath").value = item.Value;
    }

//-->
</script>
<table style="width:100%">
<tr>
    <td><label for="fieldXPath" class="Ektron_StandardLabel" id="lblCondition" runat="server">Formula:</label></td>
	<td>
		<input type="text" name="fieldXPath" class="RadETextBox" id="fieldXPath" value="" size="60" style="width:auto" />
	</td>
</tr>
<tr>
	<td><label for="cboXPathEx" class="Ektron_StandardLabel" id="lblExamples" runat="server">Examples:</label></td>
	<td>
		<radcb:radcombobox
			id="cboXPathEx"
			runat="server"
			OnClientSelectedIndexChanged="cboXPathEx_OnClientSelectedIndexChanged"
			OpenDropDownOnLoad="false" Width="350px">
		</radcb:radcombobox>
	</td>
</tr>    
<tr>    
	<td colspan="2" style="width:100%">
		<div class="Ektron_TopSpaceSmall">
			<ek:FieldTreeViewControl id="xpathTree" runat="server" />  
		</div>
	</td>
</tr>
</table>


