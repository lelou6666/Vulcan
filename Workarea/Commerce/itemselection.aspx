<%@ Page Language="VB" AutoEventWireup="false" CodeFile="itemselection.aspx.vb" Inherits="Commerce_itemselection" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Item Selection</title>
    <script type="text/javascript" language="javascript">
    function resetPostback()
    {
        document.forms[0].isPostData.value = "";
    }
    </script>
</head>
<body style="margin:0 0 0 0;">
    <form id="form1" runat="server">
    <div class="ektronPageContainer ektronPageGrid">
        <asp:Panel cssclass="ektronPageGrid" ID="pnl_viewall" runat="Server">
            <div id="dhtmltooltip"></div>
            <asp:DataGrid ID="FolderDataGrid"
                runat="server"
                AutoGenerateColumns="False"
                Width="100%"
                AllowPaging="False"
                AllowCustomPaging="True"
                PageSize="10"
                PagerStyle-Visible="False"
                EnableViewState="False"
                cssclass="ektronGrid"
                showheader="false">
                <HeaderStyle CssClass="title-header" />
            </asp:DataGrid>
            <p class="pageLinks">
                <asp:Label runat="server" ID="PageLabel">Page</asp:Label>
                <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                <asp:Label runat="server" ID="OfLabel">of</asp:Label>
                <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
            </p>
            <asp:LinkButton runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
            <asp:LinkButton runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage" Text="[Previous Page]"
                OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
            <asp:LinkButton runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
            <asp:LinkButton runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
            <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
        </asp:Panel>
        <asp:Panel CssClass="ektronPageGrid" ID="pnl_catalogs" runat="server" Visible="false">
            <asp:DataGrid ID="CatalogGrid"
                runat="server"
                AutoGenerateColumns="False"
                Width="100%"
                AllowPaging="False"
                AllowCustomPaging="True"
                PageSize="10"
                PagerStyle-Visible="False"
                EnableViewState="False"
                cssclass="ektronGrid"
                showheader="false">
                <HeaderStyle CssClass="title-header" />
                <Columns>
                    <asp:TemplateColumn ItemStyle-Width="5%"><ItemTemplate><img style="margin-left: .25em;" alt="" src="../images/ui/icons/tree/folderGreen.png"/>&#160;&#160;</ItemTemplate></asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top" >
                        <ItemTemplate><a href="itemselection.aspx?exclude=<%# excludeId %>&SelectedTab=<%# Request.QueryString("SelectedTab") %>&action=browse<%=m_sPageAction%>&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><%#DataBinder.Eval(Container.DataItem, "Name")%></a></ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
