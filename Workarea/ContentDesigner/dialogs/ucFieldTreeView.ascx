<%@ Control language="c#" CodeFile="ucFieldTreeView.ascx.cs" Inherits="Ektron.ContentDesigner.Dialogs.ucFieldTreeView" AutoEventWireup="false" %>
<%@ Register TagPrefix="radT" Namespace="Telerik.WebControls" Assembly="RadTreeView.NET2" %>
<%@ Register TagPrefix="rada" Namespace="Telerik.WebControls" Assembly="RadAjax.NET2" %>

<clientscript id="EkFieldTreeViewScript" runat="server">
function EkFieldTreeViewControl(clientID)
{
	var m_objXPathExpr = null;
	var m_strSelection = "";
	var m_strFieldPath = "";
	var m_objTextSel = null;

	this.clientID = clientID;
	this.load = function(sContentTree)
	{
        var ajaxManager = window[clientID + "_RadAjaxManager1"];
        ajaxManager.AjaxRequest(sContentTree);
	};
	
	this.setXPathExpression = function(objXPathExpr)
	{
		m_objXPathExpr = objXPathExpr;
		var oElem = document.getElementById(m_objXPathExpr.txtFieldId);
		if (oElem)
		{
			$ektron(oElem).bind("change", m_clearTextSel).bind("click", m_clearTextSel).bind("select", m_clearTextSel);
			oElem = null;
		}
	};
	
	this.updateTextField = function()
	{
		m_objXPathExpr.updateXPath(m_objTextSel, m_strFieldPath);
		m_clearTextSel();
	};
	
	this.treeNodeDoubleClick = function(node)
	{
		this.treeNodeClick(node);
		this.updateTextField();
		return false;
	};

	this.treeNodeClick = function(node)
	{
		m_strFieldPath = node.Value;
		m_objTextSel = m_objXPathExpr.expandFieldNameSelection();
		m_updateXPathLink(m_objTextSel.text);
	};
	
	function m_clearTextSel()
	{
		if (m_objTextSel)
		{
			m_objTextSel = null;
			m_updateXPathLink();
		}
	}
	
	function m_updateXPathLink(selectedText)
	{
		var oLink = document.getElementById(clientID + "_UpdateLink");
		if (m_objTextSel)
		{
			if (selectedText)
			{
				oLink.innerHTML = Ektron.String.format(EkFieldTreeViewResourceText.sReplace0in1, selectedText, m_objXPathExpr.txtFieldName);
			}
			else
			{
				oLink.innerHTML = EkFieldTreeViewResourceText.sInsertField;
			}
			oLink.style.visibility = "visible";
		}
		else
		{
			var tree = window[clientID + "_RadTree1"];
			tree.UnSelectAllNodes();
			oLink.style.visibility = "hidden";
		}
	}
}
var ekFieldTreeViewControl = new Array();
</clientscript>
<clientscript id="EkFieldTreeViewArray" runat="server">
ekFieldTreeViewControl["&lt;%=this.ClientID%&gt;"] = new EkFieldTreeViewControl("&lt;%=this.ClientID%&gt;");
</clientscript>

<fieldset id="fsTreeControl" style="padding-bottom: 0; padding-right: 0;" runat="server">
    <legend id="lblSelectField" runat="server">Select a field to insert:</legend>
    <asp:HyperLink ID="UpdateLink" style="visibility: hidden" runat="server">Insert field</asp:HyperLink>
    <radT:RadTreeView id="RadTree1" runat="server" 
		AllowNodeEditing="False" SingleExpandPath="True"
		MultipleSelect="False" CheckBoxes="False"
		Style="overflow: auto; height: 150px; margin-top: 2px;" />
</fieldset>
<rada:RadAjaxManager id="RadAjaxManager1" runat="server" OnAjaxRequest="AjaxManager1_AjaxRequest">
    <ajaxsettings>
		<rada:AjaxSetting AjaxControlID="RadAjaxManager1">
			<UpdatedControls>
				<rada:AjaxUpdatedControl ControlID="RadTree1" />
			</UpdatedControls>
		</rada:AjaxSetting>
	</ajaxsettings>
</rada:RadAjaxManager>
