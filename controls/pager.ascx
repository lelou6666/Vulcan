<%@ Control Language="C#" AutoEventWireup="true" CodeFile="pager.ascx.cs" Inherits="Vulcan.controls.pager" %>
<asp:HyperLink ID="lbtnFirst" runat="server" Text="first" ToolTip="First Page" Visible="false"></asp:HyperLink>
<span id="spanSeparator1" runat="server" class="data-pager-separator">|</span>
<asp:HyperLink ID="lbtnPrev" runat="server" Text="prev" ToolTip="Previous Page" Visible="false"></asp:HyperLink> 
<span class="data-pager-page">&nbsp;&nbsp;&nbsp;Page <%= CurrentPage%> of <%= NumberOfPages%>&nbsp;&nbsp;&nbsp;</span>
<asp:HyperLink ID="lbtnNext" runat="server" Text="next" ToolTip="Next Page" Visible="false"></asp:HyperLink>
<span id="spanSeparator2" runat="server" class="data-pager-separator">&nbsp;|&nbsp;</span>
<asp:HyperLink ID="lbtnLast" runat="server" Text="last" ToolTip="Last Page" Visible="false"></asp:HyperLink>