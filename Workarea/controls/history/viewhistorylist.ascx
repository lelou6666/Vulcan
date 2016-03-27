<%@ Control Language="vb" AutoEventWireup="false" Inherits="ViewHistoryList" CodeFile="ViewHistoryList.ascx.vb" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <asp:DataGrid id="HistoryListGrid" 
        runat="server" 
        Width="100%" 
        AutoGenerateColumns="False" 
        EnableViewState="False"
        CssClass="ektronGrid"
        GridLines="None">
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid>
</div>
