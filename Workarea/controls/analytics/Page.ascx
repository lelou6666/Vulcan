<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Page.ascx.vb" Inherits="controls_analytics_Page" %>

<asp:Literal ID="navBar" runat="server"/>
    <asp:GridView ID="GridView1" 
        runat="server" 
        AutoGenerateColumns="False" 
        Width="100%" 
        AllowPaging="True" 
        AllowSorting="True" 
        PageSize="3"
        GridLines="None"
        CssClass="ektronBorder">
        <HeaderStyle CssClass="title-header" />
        <Columns>
            <asp:HyperLinkField HeaderText="Page" DataNavigateUrlFields="url" DataTextField="url" ShowHeader="False" SortExpression="url" />
            <asp:BoundField DataField="Visits" HeaderText="Visitors" SortExpression="Visits" />
            <asp:BoundField DataField="Views" HeaderText="Views" SortExpression="Views" />
        </Columns>
    </asp:GridView>

    <asp:GridView ID="GridView2" 
        runat="server" 
        AutoGenerateColumns="False" 
        Width="100%" 
        AllowPaging="True" 
        AllowSorting="True" 
        PageSize="3"
        GridLines="None"
        CssClass="ektronBorder">
        <HeaderStyle CssClass="title-header" />
        <Columns>
            <asp:HyperLinkField HeaderText="Content" DataNavigateUrlFields="content_id" DataTextField="content_title" ShowHeader="False" SortExpression="content_title" />
            <asp:BoundField DataField="Visits" HeaderText="Visitors" SortExpression="Visits" />
            <asp:BoundField DataField="Views" HeaderText="Views" SortExpression="Views" />
        </Columns>
    </asp:GridView>
<asp:Label ID="ErrMsg" runat="server" EnableViewState="False" Visible="False"/>

<blockquote>
    &nbsp;<div id="stats_aggr" style="width:50%" runat="server">
<table border="0" width="95%">
    <tr>
        <td>
            <asp:Label ID="lbl_total_hits" runat="server" Text="Total Views of Page" Font-Bold="True"/></td><td><asp:Label ID="num_total_hits" runat="server" Text="100"/></td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lbl_total_visitors" runat="server" Text="Total Visitors to Page" Font-Bold="True"/></td><td><asp:Label ID="num_total_visitors" runat="server" Text="20"/></td>
    </tr>
        <tr>
        <td><asp:Label ID="lbl_hits_per_visitor" runat="server" Text="Page Views/Visitor" Font-Bold="True"/></td><td><asp:Label ID="num_hits_per_visitor" runat="server" Text="5"/></td>
    </tr>
        <tr>
        <td><asp:Label ID="lbl_new_visitors" runat="server" Text="New Visitors to Page" Font-Bold="True"/></td><td><asp:Label ID="num_new_visitors" runat="server" Text="10"/></td>
    </tr>
        <tr>
        <td><asp:Label ID="lbl_returning_visitors" runat="server" Text="Returning Visitors to Page" Font-Bold="True"/></td><td><asp:Label ID="num_returning_visitors" runat="server" Text="10"/></td>
    </tr>
</table>
<table id="TABLE1" width="100%"><tr><td><table>
<thead>
    <asp:Label ID="lbl_hits_vs_visitors" runat="server" Text="Content Views vs. Visitors" Font-Bold="True"/></thead><tr>
        <td><asp:Image ID="graph_hits_per_visitor" runat="server" /></td>
        <td>
            <ul><li style="color:Red"><% Response.Write(common.EkMsgRef.GetMessage("total views"))%></li><li style="color:blue"><% Response.Write(common.EkMsgRef.GetMessage("total visitors")) %></li></ul>
            </td>
    </tr>
</table></td><td><table>
    <asp:Label ID="lbl_new_vs_returning_visitors" runat="server" Text="New Vs. Returning Visitors" Font-Bold="True"/><tr>
        <td><asp:Image ID="graph_new_vs_returning_visitors" runat="server" /></td>
        <td>
            <ul><li style="color:Red"><% Response.Write(common.EkMsgRef.GetMessage("new visitors"))%></li><li style="color:Blue"><% Response.Write(common.EkMsgRef.GetMessage("returning visitors"))%></li></ul>
            </td>
    </tr>
</table></td></tr></table></div>
</blockquote>



<asp:Image ID="Image1" runat="server" />&nbsp;
<asp:Label ID="graph_key" runat="server"/>
