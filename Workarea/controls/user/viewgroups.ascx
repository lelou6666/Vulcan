<%@ Control Language="vb" AutoEventWireup="false" Inherits="viewgroups" CodeFile="viewgroups.ascx.vb" %>

<asp:Literal ID="PostBackPage" runat="server" />

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <asp:DataGrid ID="MapCMSGroupToADGrid" 
        runat="server" 
        AutoGenerateColumns="False"
        Width="100%" 
        EnableViewState="False" 
        CssClass="ektronGrid" 
        AllowCustomPaging="True" PagerStyle-Visible="False"
        GridLines="None">
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid> 
    <asp:Literal ID="ltr_message" runat="server" />
    <p class="pageLinks">
        <asp:Label runat="server" ID="PageLabel">Page</asp:Label>
        <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
        <asp:Label runat="server" ID="OfLabel">of</asp:Label>
        <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
    </p>
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
        OnCommand="NavigationLink_Click" CommandName="First" />
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="PreviousPage" Text="[Previous Page]"
        OnCommand="NavigationLink_Click" CommandName="Prev" />
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
        OnCommand="NavigationLink_Click" CommandName="Next" />
    <asp:LinkButton runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
        OnCommand="NavigationLink_Click" OnClick="LinkBtn_Click" CommandName="Last" />

    <input type="hidden" id="groupMarker" name="groupMarker" value="true"/>
</div>