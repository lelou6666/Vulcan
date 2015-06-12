<%@ Control Language="vb" AutoEventWireup="false" Inherits="editpreapproval" CodeFile="editpreapproval.ascx.vb" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
        <tr>
            <td class="label">Preapproval Group:</td>
            <td class="value">
                <select name="selectusergroup" id="selectusergroup">
                    <asp:Literal id="lit_select_preapproval" runat="server" />
                </select>
            </td>
        </tr>
    </table>							
</div>