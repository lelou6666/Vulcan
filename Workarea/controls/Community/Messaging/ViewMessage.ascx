<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ViewMessage.ascx.vb" Inherits="controls_Community_Messaging_ViewMessage" %>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
    <asp:Literal ID="ltrMsgView" runat="server" />
</div>
<input type="hidden" id="MsgInboxSelCBHdn" name="MsgInboxSelCBHdn" value="" />
