<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Referring_url.ascx.vb" Inherits="controls_analytics_Referring_url" %>
<asp:Literal ID="navBar" runat="server"/>
<asp:Image ID="Image1" runat="server" />

    <asp:GridView ID="GridView1" 
        runat="server" 
        AutoGenerateColumns="False" 
        Width="100%" 
        BorderColor="White"
        AllowPaging="True" 
        AllowSorting="True" 
        PageSize="3" >
        <HeaderStyle CssClass="title-header" />
        <Columns>
            <asp:HyperLinkField HeaderText="Page" DataNavigateUrlFields="referring_url" DataTextField="referring_url" ShowHeader="False" SortExpression="referring_url" />
            <asp:BoundField DataField="referrals" HeaderText="Referrals" SortExpression="referrals" />
        </Columns>
    </asp:GridView>

    <asp:GridView ID="GridView2" 
        runat="server" 
        AutoGenerateColumns="False" 
        Width="100%" 
        BorderColor="White"
        AllowPaging="True" 
        AllowSorting="True" 
        PageSize="3" >
        <HeaderStyle CssClass="title-header" />
        <Columns>
            <asp:HyperLinkField HeaderText="Page" DataNavigateUrlFields="url" DataTextField="url" ShowHeader="False" SortExpression="url" />
            <asp:BoundField DataField="Landings" HeaderText="Landings" SortExpression="Landings" />
        </Columns>
    </asp:GridView>

    <asp:GridView ID="GridView3" 
        runat="server" 
        AutoGenerateColumns="False" 
        Width="100%" 
        AllowPaging="True" 
        BorderColor="White"     
        AllowSorting="True" 
        PageSize="3" >
        <HeaderStyle CssClass="title-header" />
        <Columns>
            <asp:BoundField DataField="referring_url_path" HeaderText="Referring URL" SortExpression="referring_url_path" />
            <asp:BoundField DataField="Landings" HeaderText="Landings" SortExpression="Landings" />
        </Columns>
    </asp:GridView>
<asp:Label ID="ErrMsg" runat="server" EnableViewState="False" Visible="False"/>
<blockquote>
<div id="stats_aggr" runat="server" style="width: 50%">
    <table border="0" width="95%">
        <tr>
            <td>
                <asp:Label ID="lbl_total_hits" runat="server" Font-Bold="True" Text="Total Views of Page"/></td>
            <td>
                <asp:Label ID="num_total_hits" runat="server" Text="100"/></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_total_visitors" runat="server" Font-Bold="True" Text="Total Visitors to Page"/></td>
            <td>
                <asp:Label ID="num_total_visitors" runat="server" Text="20"/></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_hits_per_visitor" runat="server" Font-Bold="True" Text="Page Views/Visitor"/></td>
            <td>
                <asp:Label ID="num_hits_per_visitor" runat="server" Text="5"/></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_new_visitors" runat="server" Font-Bold="True" Text="New Visitors to Page"/></td>
            <td>
                <asp:Label ID="num_new_visitors" runat="server" Text="10"/></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_returning_visitors" runat="server" Font-Bold="True" Text="Returning Visitors to Page"/></td>
            <td>
                <asp:Label ID="num_returning_visitors" runat="server" Text="10"/></td>
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
</table></td></tr></table>
</div>
</blockquote>