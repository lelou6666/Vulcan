<%@ Control Language="C#" %>
<script runat="server">
    private bool _enabled = true;

    public bool Enabled
    {
        get { return _enabled; }
        set { _enabled = value; }
    }

    protected void Page_Load(Object src, EventArgs e)
    {
		Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
		this.lblAllow.InnerHtml = refMsg.GetMessage("lbl allow");
		this.lblOnlyOne.InnerHtml = refMsg.GetMessage("lbl only one");
		this.lblMoreThanOne.InnerHtml = refMsg.GetMessage("lbl more than one");
			
        AllowPanel.Enabled = _enabled;
		if (!Page.ClientScript.IsClientScriptBlockRegistered("EkFieldAllowScript"))
		{
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldAllowScript", EkFieldAllowScript.InnerText, true);
		}
		EkFieldAllowScript.Visible = false;
	}
</script>
<clientscript id="EkFieldAllowScript" runat="server">
function EkFieldAllowControl()
{
	this.read = function(oFieldElem) 
	{
		if ("root" == oFieldElem.getAttribute("ektdesignns_role"))
		{
			this.disable();
		}
	    else if ("unbounded" == oFieldElem.getAttribute("ektdesignns_maxoccurs"))
	    {   
	        document.getElementById("optAllow2").checked = true;
	    }
	    else
	    {
			document.getElementById("optAllow1").checked = true;
	    }
	};
	this.update = function(oFieldElem)
	{
		if (document.getElementById("optAllow2").checked)
		{
			oFieldElem.setAttribute("ektdesignns_maxoccurs", "unbounded");
		}
		else
		{
			oFieldElem.removeAttribute("ektdesignns_maxoccurs");
		}
	};
	this.getMaxOccurs = function()
	{
		return (document.getElementById("optAllow2").checked ? "unbounded" : 1);
	};
	this.isRepeatable = function()
	{
		return (document.getElementById("optAllow2").checked);
	};
	this.enable = function()
	{
        document.getElementById("AllowContainer").disabled = false;
        document.getElementById("optAllow1").disabled = false;
        document.getElementById("optAllow2").disabled = false;
	};
	this.disable = function()
	{
		document.getElementById("optAllow1").checked = true;
        document.getElementById("AllowContainer").disabled = true;
        document.getElementById("optAllow1").disabled = true;
        document.getElementById("optAllow2").disabled = true;
	};
}
var ekFieldAllowControl = new EkFieldAllowControl();
</clientscript>
<asp:Panel ID="AllowPanel" runat="server">
	<fieldset id="AllowContainer">
		<legend id="lblAllow" runat="server">Allow</legend>
		<div class="Ektron_TopSpaceVeryVerySmall">
			<input type="radio" name="optAllow" id="optAllow1" value="True" checked="checked" /><label for="optAllow1" id="lblOnlyOne" runat="server">Only one</label><br />
			<input type="radio" name="optAllow" id="optAllow2" value="False" /><label for="optAllow2" id="lblMoreThanOne" runat="server">More than one</label><br />
		</div>
	</fieldset>
</asp:Panel> 