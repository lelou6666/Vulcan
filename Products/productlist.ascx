﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="productlist.ascx.cs" Inherits="productlist" %>
<%@ Register assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.WebControls" tagprefix="asp" %>

<asp:ListView ID="UXListView" runat="server" EnableViewState="false">
    <LayoutTemplate>
        <div class="listview_productlist">
        	<div style="margin:20px; overflow:auto;">
                <ul>
                    <div runat="server" id="itemPlaceholder" style="position: absolute;"></div>
                </ul>
            </div>
        </div>
    </LayoutTemplate>

    <ItemTemplate>
        <li>
             <a href="<%#Eval("Alias")%>" style="color:#FFF;"><%#Eval("Title")%></a>
        </li>
    </ItemTemplate>
</asp:ListView>

<asp:ListView ID="UXGridView" runat="server" EnableViewState="false">
    <LayoutTemplate>
        <div runat="server" id="itemPlaceholder" style="position: absolute;"></div>
    </LayoutTemplate>

    <ItemTemplate>
        
        <div class="prod_item">
            <div class="prodsearchitem">
                <a href="<%#Eval("Alias")%>" style="text-decoration:none;">
                    <div class="product_img" style="margin-bottom:10px; text-align:center;">
                        <img alt="" src="<%#Eval("Image")%>" alt="<%#Eval("Title")%>" style="vertical-align: middle;" border="0" />
                    </div>
                </a>
                <a href="<%#Eval("Alias")%>"><%#Eval("Title")%></a>
            </div>
        </div>
    </ItemTemplate>
</asp:ListView>