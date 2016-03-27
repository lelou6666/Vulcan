<%@ Control Language="VB" AutoEventWireup="false" CodeFile="viewvirtualstaging.ascx.vb" Inherits="viewvirtualstaging" %>
<table>
	<tr>
		<td class="info"><%=m_refMsg.GetMessage("asset location")%></td>
		<td>&nbsp;</td>
		<td align="center" id="td_asset_loc" runat="server"></td>
	</tr>
	<tr>
		<td class="info"><%=m_refMsg.GetMessage("private asset location")%></td>
		<td>&nbsp;</td>
		<td align="center" id="td_private_asset_loc" runat="server"></td>
	</tr>
	<tr>
		<td class="info"><%=m_refMsg.GetMessage("lbl domain username")%></td>
		<td>&nbsp;</td>
	    <td align="center" id="td_domain_username" runat="server"></td>
	</tr>
	
	
</table>
