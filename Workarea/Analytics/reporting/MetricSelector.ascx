<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MetricSelector.ascx.cs" Inherits="Analytics_reporting_MetricSelector" %>
<style type="text/css">
span.metric
{
	margin-left: 0em;
	margin-right: 1em;
	margin-bottom: 2px;
}
img.metric
{
	margin-right: 3px;
	margin-bottom: 2px;
}
select.metric
{
	margin-right: 1em;
	margin-bottom: 2px;
}
</style>
<asp:UpdatePanel ID="pnlChart" UpdateMode="Conditional" runat="server">
	<ContentTemplate>
<asp:Label ID="lblDisplay" runat="server" CssClass="metric" Text="Display" EnableViewState="false"></asp:Label>
<asp:Image ID="Image1" runat="server" CssClass="metric" ImageUrl="css/metricBlue.gif" EnableViewState="false" />
<asp:DropDownList ID="Selector1" runat="server" CssClass="metric" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_SelectionChanged"></asp:DropDownList>
<asp:Image ID="Image2" runat="server" CssClass="metric" ImageUrl="css/metricOrange.gif" EnableViewState="false" />
<asp:DropDownList ID="Selector2" runat="server" CssClass="metric" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_SelectionChanged"></asp:DropDownList>
	</ContentTemplate>
</asp:UpdatePanel>




