<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Reorder.ascx.vb" Inherits="controls_Reorder_Reorder" %>
<input type="hidden" name="LinkOrder" id="LinkOrder" value="<%=ReOrderList()%>"/>
<div style="padding:1em;">
    <table width="100%">
	<tr>
	    <td width="80%">
		    <select name="OrderList" id="OrderList" size="<%if (ItemList.Count < 20) then Response.Write(ItemList.Count) else Response.Write("20")%>" style="margin:0;padding:0;width:100%;">
			<%
			    For i As Integer = 0 To (ItemList.Count - 1)
			%>
					<option value="<%=ItemList(i).Value%>" <%If (i = 0) then Response.Write("selected") else Response.Write("")%>><%=ItemList(i).Text%></option>
			<%
			    Next
			%>
		    </select>
		</td>
	    <td width="20%">
            <div class="ektronTopSpace">
                <a href="#" onclick="javascript:Move('up', document.getElementById('OrderList'), document.getElementById('LinkOrder'));">
                    <img style="cursor:pointer;" src="<%=AppPath()%>Images/ui/icons/arrowHeadUp.png" border="0" width="26" height="17" alt="<%=GetMessage("move selection up msg")%>" title="<%=GetMessage("move selection up msg")%>" />
                </a><br />
                <a href="#" onclick="javascript:Move('dn', document.getElementById('OrderList'), document.getElementById('LinkOrder'));">
                    <img style="cursor:pointer;" src="<%=AppPath()%>Images/ui/icons/arrowHeadDown.png" border="0" width="26" height="17" alt="<%=GetMessage("move selection down msg")%>" title="<%=GetMessage("move selection down msg")%>" />
                </a><br />
                <br />
            </div>
		</td>
	</tr>
</table>
</div>