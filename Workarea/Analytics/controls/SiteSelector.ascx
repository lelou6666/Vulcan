<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SiteSelector.ascx.cs" Inherits="Analytics_controls_SiteSelector" %>
<span class="SiteSelectorContainer" ID="SiteSelectorContainer" runat="server">
    <asp:Label ID="lblSiteSelector" runat="server" EnableViewState="false" />
    <asp:DropDownList ID="SiteSelectorList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_SelectionChanged" />
</span>
