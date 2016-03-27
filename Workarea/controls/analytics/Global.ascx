<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Global.ascx.vb" Inherits="controls_analytics_Global" %>
<blockquote>
<div id="stats_aggr" style="width:50%" runat=server>
<table border="0" width="95%">
    <tr>
        <td>
            <asp:Label ID="lbl_total_hits" runat="server" Text="Total Page Views" Font-Bold="True"></asp:Label></td><td><asp:Label ID="num_total_hits" runat="server" Text="100"></asp:Label></td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lbl_total_visitors" runat="server" Text="Total Visitors" Font-Bold="True"></asp:Label></td><td><asp:Label ID="num_total_visitors" runat="server" Text="20"></asp:Label></td>
    </tr>
        <tr>
        <td><asp:Label ID="lbl_hits_per_visitor" runat="server" Text="Page Views/Visitor" Font-Bold="True"></asp:Label></td><td><asp:Label ID="num_hits_per_visitor" runat="server" Text="5"></asp:Label></td>
    </tr>
        <tr>
        <td><asp:Label ID="lbl_new_visitors" runat="server" Text="New Visitors" Font-Bold="True"></asp:Label></td><td><asp:Label ID="num_new_visitors" runat="server" Text="10"></asp:Label></td>
    </tr>
        <tr>
        <td><asp:Label ID="lbl_returning_visitors" runat="server" Text="Returning Visitors" Font-Bold="True"></asp:Label></td><td><asp:Label ID="num_returning_visitors" runat="server" Text="10"></asp:Label></td>
    </tr>
</table>
<table id="TABLE1" width="100%"><tr><td><table>
<thead>
    <asp:Label ID="lbl_hits_vs_visitors" runat="server" Text="Content Views vs. Visitors" Font-Bold="True"></asp:Label></thead><tr>
        <td><asp:Image ID="graph_hits_per_visitor" runat="server" /></td>
        <td>
            <ul><li style="color:Red"><% Response.Write(common.EkMsgRef.GetMessage("total views"))%></li><li style="color:blue"><% Response.Write(common.EkMsgRef.GetMessage("total visitors")) %></li></ul>
            </td>
    </tr>
</table></td><td><table>
    <asp:Label ID="lbl_new_vs_returning_visitors" runat="server" Text="New Vs. Returning Visitors" Font-Bold="True"></asp:Label><tr>
        <td><asp:Image ID="graph_new_vs_returning_visitors" runat="server" /></td>
        <td>
            <ul><li style="color:Red"><% Response.Write(common.EkMsgRef.GetMessage("new visitors"))%></li><li style="color:Blue"><% Response.Write(common.EkMsgRef.GetMessage("returning visitors"))%></li></ul>
            </td>
    </tr>
</table></td></tr></table></div></blockquote>

<asp:Label ID="ByTimeGraph" runat="server" Text="Time Graph Here"></asp:Label>
<asp:Label ID="graph_key" runat="server"></asp:Label>