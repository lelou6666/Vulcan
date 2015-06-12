<%@ Control Language="C#" AutoEventWireup="true" CodeFile="downloads.ascx.cs" Inherits="downloads" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
	<!-- loading animation -->
     <asp:UpdateProgress id="updateProgress" runat="server">
        <ProgressTemplate>
            <div style="position:fixed; top:50%; left:50%; margin-top:-55px; margin-left:-80px; z-index:8000;">
                <div id="facebookG">
                    <div id="blockG_1" class="facebook_blockG"></div>
                    <div id="blockG_2" class="facebook_blockG"></div>
                    <div id="blockG_3" class="facebook_blockG"></div>
                </div> 
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div class="downloads">                 
        <div class="one-third">
            <div id="download-spec">Download Documents</div>
            <p>View our innovative products in our <a href="/uploadedFiles/Vulcan/2015_PriceBook.pdf" target="_blank">2015 Pricebook</a> or our <a href="/uploadedFiles/Vulcan/2015_Product_Showcase.pdf" target="_blank">Vulcan Product Showcase</a>.</p>
        </div>
        <div class="two-thirds" style="padding-top:25px;">
            <div id="download-split1">
                <asp:DropDownList runat="server" ID="segment" AutoPostBack="true" OnSelectedIndexChanged="updateSegment" EnableViewState="true" CssClass="download-segment">
                    <asp:ListItem Value="" Text="Category" />
                    <asp:ListItem Value="Braising Pans" Text="Braising Pans" />
                    <asp:ListItem Value="Charbroilers" Text="Charbroilers" />
                    <asp:ListItem Value="Combi" Text="Combi Ovens" />
                    <asp:ListItem Value="Ovens" Text="Convection Ovens" />
                    <asp:ListItem Value="Fryers" Text="Fryers" />
                    <asp:ListItem Value="Griddles" Text="Griddles" />
                    <asp:ListItem Value="Heated Holding" Text="Heated Holding" />
                    <asp:ListItem Value="Kettles" Text="Kettles" />
                    <asp:ListItem Value="Ranges" Text="Ranges" />
                    <asp:ListItem Value="Steamers" Text="Steamers" />
                </asp:DropDownList>
                <img src="/images/next-arrow.png" alt="->" width="28" height="31" style="padding-top:8px;" class="dl-arrow" />
                <asp:DropDownList runat="server" ID="series" AutoPostBack="true" OnSelectedIndexChanged="updateSeries" EnableViewState="true" CssClass="download-series" />
                <img src="/images/next-arrow.png" alt="->" width="28" height="31" style="padding-top:8px;" class="dl-arrow" />
                <asp:DropDownList runat="server" ID="product" AutoPostBack="true" OnSelectedIndexChanged="updateProduct" EnableViewState="true" CssClass="download-product" />
            </div>
            <div id="download-split2">
                <asp:DropDownList runat="server" ID="model" AutoPostBack="true" OnSelectedIndexChanged="updateModel" EnableViewState="true" CssClass="download-model" />
                <img src="/images/next-arrow.png" alt="->" width="28" height="31" style="padding-top:8px;" class="dl-arrow" />
                <asp:DropDownList runat="server" ID="document" AutoPostBack="true" OnSelectedIndexChanged="showButtons" EnableViewState="true" CssClass="download-document">
                    <asp:ListItem Value="" Text="Document Type" />
                    <asp:ListItem Value="CAD Revit" Text="CAD and Revit" />
                    <asp:ListItem Value="Images" Text="Images" />
                    <asp:ListItem Value="Owners Manual" Text="Owners Manual" />
                    <asp:ListItem Value="Parts Catalog" Text="Parts Catalog" />
                    <asp:ListItem Value="Sell Sheet" Text="Sell Sheet" />
                    <asp:ListItem Value="Service Manual" Text="Service Manual" />
                    <asp:ListItem Value="Spec Sheet" Text="Spec Sheet" />
                </asp:DropDownList>
                <div runat="server" id="buttons" style="text-align:right; display: inline; padding-left: 34px;">
                    <asp:ImageButton runat="server" ID="download" ImageUrl="/images/download-now.png" OnClick="downloadSpecs" AutoPostBack="true" width="143" height="33" style="vertical-align:top; padding-right:7px;" />
                    <asp:ImageButton runat="server" ID="view" ImageUrl="/images/view-now.png" OnClick="viewSpecs" AutoPostBack="true" width="79" height="33" style="vertical-align:top;" />
                    <br />
                </div>
                <p style="padding-bottom:0; color:#d4d3d3; padding-top:10px;"><asp:Literal runat="server" ID="specError" /></p>
            </div>
        </div>
    </div>
    <br style="clear:both;" />
</ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="download" />
    </Triggers>
</asp:UpdatePanel>
