<%@ Page Language="VB" AutoEventWireup="false" CodeFile="customers.aspx.vb" Inherits="Commerce_customers" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <title>Customers</title>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
            function resetPostback() {
                document.forms[0].isPostData.value = "";
            }
            function resetCPostback() {
                document.forms["form1"].isCPostData.value = "";
            }
            //--><!]]>
        </script>
    </head>
    <body onclick="MenuUtil.hide()">
        <form id="form1" runat="server">
        <div class="ektronPageContainer ektronPageTabbed">
            <asp:Panel CssClass="ektronPageGrid" ID="pnl_view" runat="Server" Visible="false">
            <div class="tabContainerWrapper">
            <div class="tabContainer">
                <ul>
                    <li>
                        <a href="#dvProp">
                            <%=Me.GetMessage("properties text")%>
                        </a>
                    </li>
                    <li>
                        <a href="#dvOrders">
                            <%=Me.GetMessage("lbl orders")%>
                        </a>
                    </li>
                    <li>
                        <a href="#dvAddress">
                            <%=Me.GetMessage("lbl addresses")%>
                        </a>
                    </li>
                    <li>
                        <a href="#dvBaskets">
                            <%=Me.GetMessage("lbl baskets")%>
                        </a>
                    </li>
                </ul>
                <div id="dvProp">
                    <table id="tblmain" runat="server" class="ektronGrid">
                        <tr id="tr_id" runat="server">
                            <td class="label">
                                <asp:Literal ID="ltr_id_label" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_id" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_uname_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_uname" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_fname_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_fname" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_lname_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_lname" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_dname_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_dname" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_ordertotal_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_ordertotal" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_orderval_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_orderval" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="ltr_pervalue_lbl" runat="server" />:</td>
                            <td>
                                <asp:Literal ID="ltr_pervalue" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="dvOrders">
                    <br />
                    <asp:DataGrid ID="dg_orders"
                        runat="server"
                        AutoGenerateColumns="false"
                        Width="100%"
                        GridLines="None"
                        CssClass="ektronGrid">
                        <HeaderStyle CssClass="title-header" />
                        <Columns>
                            <asp:HyperLinkColumn DataTextField="Id" HeaderText="Id" DataNavigateUrlField="Id" DataNavigateUrlFormatString="fulfillment.aspx?action=vieworder&id={0}"></asp:HyperLinkColumn>
                            <asp:HyperLinkColumn DataTextField="DateCreated" HeaderText="Date" DataNavigateUrlField="Id" DataNavigateUrlFormatString="fulfillment.aspx?action=vieworder&id={0}"></asp:HyperLinkColumn>
                            <asp:BoundColumn DataField="Status" HeaderText="Status" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                            <asp:TemplateColumn HeaderText="Order Value" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate><%#DataBinder.Eval(Container.DataItem, "Currency.AlphaIsoCode")%>&nbsp;<%#Ektron.Cms.Common.EkFunctions.FormatCurrency(DataBinder.Eval(Container.DataItem, "OrderTotal", "{0:c}"), DataBinder.Eval(Container.DataItem, "Currency.CultureCode"))%></ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <br />
                    <asp:Literal ID="ltr_orders" runat="server" />
                </div>
                <div id="dvAddress">
                    <br />
                    <asp:DataGrid ID="dg_address"
                        Runat="server"
                        AutoGenerateColumns="false"
                        Width="100%"
                        GridLines="None"
                        CssClass="ektronGrid">
                        <HeaderStyle CssClass="title-header" />
                        <Columns>
                            <asp:TemplateColumn HeaderText="Id" ItemStyle-VerticalAlign="Top"><ItemTemplate><a href='customers.aspx?action=viewaddress&id=<%#DataBinder.Eval(Container.DataItem, "id")%>&customerid=<%#m_iId%>'><%#DataBinder.Eval(Container.DataItem, "id")%></a></ItemTemplate></asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Address">
                                <ItemTemplate><%#ShowName(DataBinder.Eval(Container.DataItem, "Name"),DataBinder.Eval(Container.DataItem, "Company"))%><br />
                                        <%#(DataBinder.Eval(Container.DataItem, "AddressLine1"))%><br />
                                        <%#ShowOptionalLine(DataBinder.Eval(Container.DataItem, "AddressLine2"))%>
                                        <%#(DataBinder.Eval(Container.DataItem, "City"))%><br />
                                        <%#(DataBinder.Eval(Container.DataItem, "Region")).Name%> <%#(DataBinder.Eval(Container.DataItem, "PostalCode"))%><br />
                                        <%#(DataBinder.Eval(Container.DataItem, "Country")).Name%><br />
                                        <%#(DataBinder.Eval(Container.DataItem, "Phone"))%>
                                        </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Billing" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate><asp:checkbox id='chk_address_b' runat="Server" Enabled="false" checked='<%#Util_IsDefaultBilling(DataBinder.Eval(Container.DataItem, "id"))%>' /></ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Shipping" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate><asp:checkbox id='chk_address_s' runat="Server" Enabled="false" Checked='<%#Util_IsDefaultShipping(DataBinder.Eval(Container.DataItem, "id"))%>' /></ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <br />
                    <asp:Literal ID="ltr_address" runat="server" />
                </div>
                <div id="dvBaskets">
                    <br />
                    <asp:DataGrid ID="dg_baskets"
                        runat="server"
                        AutoGenerateColumns="false"
                        Width="100%"
                        GridLines="None"
                        CssClass="ektronGrid">
                        <HeaderStyle CssClass="title-header" />
                        <Columns>
                            <asp:TemplateColumn HeaderText="Basket Id" ItemStyle-VerticalAlign="Top"><ItemTemplate><a href="#" onclick="ektb_show('Items for <%#DataBinder.Eval(Container.DataItem, "Name")%>', 'customers.aspx?action=viewbasket&basketid=<%#DataBinder.Eval(Container.DataItem, "Id")%>&customerid=<%#m_iId%>&thickox=true&EkTB_iframe=true&height=300&width=700&modal=true', null);"><%#DataBinder.Eval(Container.DataItem, "Id")%></a></ItemTemplate></asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Basket Name" ItemStyle-VerticalAlign="Top"><ItemTemplate><a href="#" onclick="ektb_show('Items for <%#DataBinder.Eval(Container.DataItem, "Name")%>', 'customers.aspx?action=viewbasket&basketid=<%#DataBinder.Eval(Container.DataItem, "Id")%>&customerid=<%#m_iId%>&thickox=true&EkTB_iframe=true&height=300&width=700&modal=true', null);"><%#DataBinder.Eval(Container.DataItem, "Name")%></a></ItemTemplate></asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Sub Total" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_SubTotal"><%#Ektron.Cms.Common.EkFunctions.FormatCurrency(DataBinder.Eval(Container.DataItem, "SubTotal", "{0:c}"), defaultCurrency.CultureCode)%></label></ItemTemplate></asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Active Basket" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_DefaultBasket"><%#DataBinder.Eval(Container.DataItem, "IsDefault")%></label></ItemTemplate></asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <br />
                    <asp:Literal ID="ltr_baskets" runat="server" />
                </div>
            </div>
            </div>
            </asp:Panel>
            <asp:Panel CssClass="ektronPageGrid" ID="pnl_viewall" runat="Server">
                <asp:DataGrid ID="dg_customers"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="ektronGrid"
                    GridLines="None">
                    <HeaderStyle CssClass="title-header" />
                    <Columns>
                        <asp:HyperLinkColumn DataTextField="id" HeaderText="Id" DataNavigateUrlField="id" DataNavigateUrlFormatString="customers.aspx?action=view&id={0}"></asp:HyperLinkColumn>
                        <asp:HyperLinkColumn DataTextField="userName" HeaderText="Name" DataNavigateUrlField="id" DataNavigateUrlFormatString="customers.aspx?action=view&id={0}"></asp:HyperLinkColumn>
                        <asp:BoundColumn DataField="TotalOrders" HeaderText="Total Orders" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate><%#defaultCurrency.ISOCurrencySymbol & defaultCurrency.CurrencySymbol%><%#FormatCurrency(DataBinder.Eval(Container.DataItem, "TotalOrderValue"))%></ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Per Order Value" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate><%#defaultCurrency.ISOCurrencySymbol & defaultCurrency.CurrencySymbol%><%#FormatCurrency(DataBinder.Eval(Container.DataItem, "AverageOrderValue"))%></ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
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
            </asp:Panel>
            <asp:Panel CssClass="ektronPageInfo" ID="pnl_viewaddress" runat="Server" Visible="false">
                <table class="ektronForm">
                    <tr id="tr_address_id" runat="server">
                        <td class="label">
                            <asp:Literal ID="ltr_address_id_lbl" runat="server" />:</td>
                        <td>
                            <asp:Literal ID="ltr_address_id" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_name" runat="server" />:</td>
                        <td>
                            <asp:textbox ID="txt_address_name" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_company" runat="server" />:</td>
                        <td>
                            <asp:textbox ID="txt_address_company" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_line1" runat="server" />:</td>
                        <td>
                            <asp:textbox ID="txt_address_line1" runat="server" /><br />
                            <asp:textbox ID="txt_address_line2" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_city_lbl" runat="server" />:</td>
                        <td>
                            <asp:textbox ID="txt_address_city" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_postal" runat="server" />:</td>
                        <td>
                            <asp:TextBox ID="txt_address_postal" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_country" runat="server" />:</td>
                        <td>
                            <asp:DropDownList AutoPostBack="true" ID="drp_address_country" runat="server" OnSelectedIndexChanged="drp_address_country_ServerChange"/><br />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_region" runat="server" />:</td>
                        <td>
                            <asp:DropDownList ID="drp_address_region" runat="server"/><br />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_address_phone" runat="server" />:</td>
                        <td>
                            <asp:textbox ID="txt_address_phone" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_default_billing" runat="server" />:</td>
                        <td>
                            <asp:CheckBox ID="chk_default_billing" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Literal ID="ltr_default_shipping" runat="server" />:</td>
                        <td>
                            <asp:CheckBox ID="chk_default_shipping" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnl_viewbasket" runat="Server" CssClass="ektronPageGrid" Visible="false">
                <asp:DataGrid ID="dg_viewbasket"
                    runat="server"
                    AutoGenerateColumns="false"
                    Width="100%"
                    GridLines="None"
                    CssClass="ektronGrid">
                    <HeaderStyle CssClass="title-header" />
                    <Columns>
                        <asp:TemplateColumn HeaderText="Item" HeaderStyle-HorizontalAlign="Center"
                            ItemStyle-VerticalAlign="Top">
                            <ItemTemplate>
                                <label id="lbl_item">
                                    <%#DataBinder.Eval(Container.DataItem, "ProductTitle")%>

                                    <%#showconfig(DataBinder.Eval(Container.DataItem, "configuration")) %>

                                    <%#showvariant(DataBinder.Eval(Container.DataItem, "variant")) %>
                                </label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="SKU" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_item"><%#DataBinder.Eval(Container.DataItem,"ProductSku") %></label></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Quantity" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_item"><%#DataBinder.Eval(Container.DataItem,"Quantity") %></label></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="MSRP" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_item"><%#Ektron.Cms.Common.EkFunctions.FormatCurrency(DataBinder.Eval(Container.DataItem, "ListPrice", "{0:c}"), defaultCurrency.CultureCode)%></label></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Price" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_item"><%#Ektron.Cms.Common.EkFunctions.FormatCurrency(DataBinder.Eval(Container.DataItem, "SalePrice", "{0:c}"), defaultCurrency.CultureCode)%></label></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Total" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top"><ItemTemplate><label id="lbl_item"><%#Ektron.Cms.Common.EkFunctions.FormatCurrency(DataBinder.Eval(Container.DataItem, "AdjustedTotal", "{0:c}"), defaultCurrency.CultureCode)%></label></ItemTemplate></asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
                <asp:Literal ID="ltr_noitems"  runat="server" />
            </asp:Panel>
        </div>
        <script type="text/javascript">
        <asp:Literal ID="ltr_js" runat="server" />
        </script>
                <input type="hidden" runat="server" id="isCPostData" value="false" />
                <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
        </form>
    </body>
</html>
