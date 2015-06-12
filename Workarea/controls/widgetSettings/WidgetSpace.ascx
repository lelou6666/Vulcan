<%@ Control Language="VB" AutoEventWireup="false" CodeFile="WidgetSpace.ascx.vb" Inherits="Workarea_controls_widgetSettings_WidgetSpace" %>

<script language="javascript" type="text/javascript">
function VerifyWidgetsSpace(mode, id)
{
    if (mode == "remove")
    {
        if (confirm('<%= m_refMsg.GetMessage("js remove sel items confirm") %>'))
        {
            if (confirm('<%= m_refMsg.GetMessage("js remove sel items confirm") %>'))
            {
                document.forms[0].action = "widgetsettings.aspx?action=widgetspace&mode=" + mode;
                document.forms[0].submit();
            }
            return false;
        }
        return false;
    }
    else
    {
        var title = document.getElementById('<%= txtTitle.ClientID %>');
        if (title.value == '')
        {
            var title = document.getElementById('<%= txtTitle.ClientID %>');
            if (title.value == '')
            {
                alert('<%= m_refMsg.GetMessage("js: alert title required") %>');
                title.focus();
                return false;
            }
            document.forms[0].action = "widgetsettings.aspx?action=widgetspace&mode=" + mode + "&id=" + id;
            document.forms[0].submit();
        }
        document.forms[0].action = "widgetsettings.aspx?action=widgetsspace&mode=" + mode + "&id=" + id;
        document.forms[0].submit();
    }
}
</script>

<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="ViewAll" runat="Server">
        <table class="ektronGrid">
            <tr class="title-header">
                <th style="width:1%; white-space:nowrap;">&#160;</th>
                <th style="width:1%; white-space:nowrap"><%=m_refMsg.GetMessage("generic id") %></th>
                <th><%=m_refMsg.GetMessage("generic title") %></th>
                <th><%=m_refMsg.GetMessage("generic scope") %></th>
            </tr>
            <asp:Repeater ID="ViewAllRepeater" runat="server">
                <ItemTemplate>
                    <tr class="row">
                        <td><asp:ImageButton ID="editButton" OnClick="editButton_Click" ImageUrl="../../images/UI/Icons/contentEdit.png" runat="server" CommandArgument="<%#Container.DataItem.ID%>" /></td>
                        <td><%#Container.DataItem.ID%></td>
                        <td><%#Container.DataItem.Title%></td>
                        <td><%#Container.DataItem.Scope.ToString()%></td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="evenrow">
                        <td><asp:ImageButton ID="editButton" OnClick="editButton_Click" ImageUrl="../../images/UI/Icons/contentEdit.png" runat="server" CommandArgument="<%#Container.DataItem.ID%>" /></td>
                        <td><%#Container.DataItem.ID%></td>
                        <td><%#Container.DataItem.Title%></td>
                        <td><%#Container.DataItem.Scope.ToString()%></td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </table>
    </asp:View>
    <asp:View ID="ViewAdd" runat="server">
        <div class="ektronPageInfo">
            <table class="ektronForm">
                <tr>
                    <td class="label"><asp:Label ID="lblWidgetsSpaceTitle" runat="Server" /></td>
                    <td class="value"><asp:TextBox ID="txtTitle" runat="server" /></td>
                </tr>
                <tr id="tr_groupSpace" runat="server">
                    <td class="label"><asp:Literal ID="ltrGroupSpace" runat="Server" />:</td>
                    <td class="value"><asp:CheckBox ID="chkGroupSpace" runat="server" /></td>
                </tr>
            </table>
            <div class="ektronTopSpace"></div>
            <div id="widgetDisplay">
                <fieldset>
                    <legend><asp:Literal ID="lblSelectWidgets" runat="server" /></legend>
                    <div class="widgetsHeader">
                        <h4>
                            <asp:Literal ID="widgetTitle" runat="server" /></h4>
                        <ul id="widgetActions" class="buttonWrapper">
                            <li>
                                <asp:LinkButton ID="btnSelectNone" runat="server" CssClass="redHover button selectNoneButton"
                                    OnClientClick="UnselectAllWidgets();return false;" /></li>
                            <li>
                                <asp:LinkButton ID="btnSelectAll" runat="server" CssClass="greenHover button selectAllButton buttonRight"
                                    OnClientClick="SelectAllWidgets();return false;" /></li>
                        </ul>
                    </div>         
                    <div class="ektronTopSpace"></div>           
                    <div id="widgets">                        
                        <ul id="widgetList">
                            <asp:Repeater ID="repWidgetTypes" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <div class="widget">
                                            <input type="checkbox" name="widget<%# Container.DataItem.ID %>" id="widget<%# Container.DataItem.ID %>" /><img
                                                src="<%#Container.DataItem.ControlURL + ".jpg"%>"
                                                alt="<%#Container.DataItem.Title%>" /><div class="widgetTitle" title="<%#Container.DataItem.Title%>">
                                                    <%#Container.DataItem.Title%>
                                                </div>
                                        </div>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                </fieldset>
            </div>
            <asp:Label ID="lbStatus" runat="server" />
        </div>
    </asp:View>
    <asp:View ID="ViewRemove" runat="server">
        <div class="ektronPageInfo">
            <ul>
                <asp:Repeater ID="viewAllForRemove" runat="server">
                    <ItemTemplate>
                        <li><input type="checkbox" id="chkSpace<%#Container.DataItem.ID%>" name="chkSpace<%#Container.DataItem.ID%>" /> <%#Container.DataItem.Title%></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
    </asp:View>
    <asp:View ID="ViewError" runat="Server">
        <asp:Label ID="lblMessage" runat="Server" />
    </asp:View>
</asp:MultiView>

<asp:Label ID="lblNoWidgetSpaces" Visible="false" runat="server" />
</div>