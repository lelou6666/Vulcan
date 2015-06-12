<%@ Control Language="vb" AutoEventWireup="false" Inherits="viewpermissions" CodeFile="viewpermissions.ascx.vb" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageTabbed">
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <div class="ektronTabUpperContainer">
                <div style="position:absolute;right:1.5em;z-index:9;">
                    <asp:Label ID="labelUserType" runat="server" AssociatedControlID="ddlUserType" CssClass="permissionsUserSelectorLabel">User Type:</asp:Label>
                    <asp:DropDownList ID="ddlUserType" AutoPostBack="true" EnableViewState="false" runat="server">
                        <asp:ListItem ID="liCmsUsers" runat="server" />
                        <asp:ListItem ID="liMembershipUsers" runat="server" />
                    </asp:DropDownList>
                </div>
            </div>
            <asp:PlaceHolder ID="phTabs" runat="server">
                <ul>
                    <li>
                        <a href="#dvStandard">
                            <%=_MessageHelper.GetMessage("standard permissions")%>
                        </a>
                    </li>
                    <asp:PlaceHolder ID="phAdvancedTab" runat="server" Visible="false">
                        <li>
                            <a href="#dvAdvanced">
                                <%=_MessageHelper.GetMessage("advanced permissions")%>
                            </a>
                        </li>
                    </asp:PlaceHolder>
                </ul>
            </asp:PlaceHolder>
            <div id="divInheritedPermissions" runat="server" style="margin:1em 1em 0em 1.1em;">
                <asp:CheckBox ID="cbInheritedPermissions" runat="server" AutoPostBack="false" CssClass="inheritPermissions" />
                <asp:Label ID="lblInheritedPermissions" runat="server" AssociatedControlID="cbInheritedPermissions" />
            </div>
            <div id="divPrivateContent" runat="server" style="margin:0em 1em 0em 1.1em;">
                <asp:CheckBox ID="cbPrivateContent" runat="server" AutoPostBack="false" CssClass="privateContent" />
                <asp:Label ID="lblPrivateContent" runat="server" AssociatedControlID="cbPrivateContent" />
            </div>
            <div id="dvStandard" class="ektronPageInfo">
                <asp:DataGrid id="PermissionsGenericGrid"
                    runat="server"
                    CssClass="ektronGrid"
                    AutoGenerateColumns="False"
                    EnableViewState="False">
                    <HeaderStyle CssClass="title-header" />
                </asp:DataGrid>
            </div>
            <asp:PlaceHolder ID="phAdvancedContent" runat="server" Visible="false">
                <div id="dvAdvanced" class="ektronPageInfo">
                    <asp:DataGrid id="PermissionsAdvancedGrid"
                        runat="server"
                        CssClass="ektronGrid"
                        AutoGenerateColumns="False"
                        EnableViewState="False">
                        <HeaderStyle CssClass="title-header" />
                    </asp:DataGrid>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</div>