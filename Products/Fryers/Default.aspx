<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<%@ Register assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.WebControls" tagprefix="asp" %>
<%@ Register Src="~/products/productlist.ascx" TagName="productlist" TagPrefix="productlist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/products/fryers/" />
    
    <style type="text/css">
		h1
		{
			font-family: 'futura medium condensed', Arial, Sans-Serif;
			font-size: 40px;
			line-height: 50px;
			color: #ffffff;
			padding:0px;
			margin:0px;
			text-transform:none;
			display:inline;
		}
		
		@media all and (max-width:500px) 
	    {
			h1
			{
				font-size: 22px;
				line-height: 30px;
			}
		}
		
        .prod_item
        {
            display:none;
        }
        
        .filter
        {
            display:table-cell;
            padding-right:40px;
			vertical-align:top;
        }
        
       .segment_body .segment_title
        {
            font-size:41px;
            line-height:45px;
            padding-top:40px;
            margin-left:50px; 
            margin-right:20px;
            color:#e8e8e8;
        }
        
        .segment_body p
        {
            font-size:18px;
            line-height:24px;
            padding-top:10px;
            margin-left:50px;
            margin-right:20px;
        }
        
        .segment_slant_td
        {
            width:155px;
        }
        
        .expand
        {
            display:none;
        }
		
		.prod_listview ul {
			list-style:none;
		}
		
		.prod_listview li {
			background-image: url("/images/spec_arrow.png");
			background-repeat: no-repeat;
			padding-left: 15px;
		}
       
        
        @media all and (max-width:920px) 
	    {
	        .segment_body .segment_title
            {
                margin-right:10px;
            }
            
            .segment_body p
            {

                margin-right:10px;
            }
	    }
	    
	    @media all and (max-width:960px) 
	    {
            .segment_body .twocolumn
            {

                width:100%;
            }
            
            .segment_body .segment_title
            {
                padding-top:10px;
                margin-left:0px;
            }
            
            .segment_body p
            {
                margin-left:0px;
            }
            
            #segment_desc
            {
                padding-bottom:30px;
            }
	    } 
	    
	    @media all and (max-width:768px) 
        {
            .segment_slant_td
            {
                background-color:#8d0a0d;
            }
        }
        
        @media all and (max-width:990px) 
	    {
            .segment_body .twocolumn
            {
                width:100%;
            }
            
            .segment_body .segment_title
            {
                padding-top:10px;
                margin-left:0px;
            }
            
            .segment_body p
            {
                margin-left:0px;
            }
            
            .filter 
            {
                display:block;
                padding-right:15px;
                margin-top:10px;
            }
            
            .filter_title
            {
                width:100%;
                cursor:pointer;
                padding: 8px 10px 6px 0px;
                border-bottom: 1px solid #282828;
                color: #fefefe;
                background-color:#323131;
                margin-bottom:0px;
                font-size:21px; 
            }
            
            a.jqTransformCheckbox {
				background: transparent url(/images/checkbox_mobile.png) no-repeat center top;
				width:26px;
				height:26px;
			}
			
			a.jqTransformChecked {
				background: transparent url(/images/checkbox_mobile.png) no-repeat center bottom;
				width:26px;
				height:26px;
			}
            
            #checkboxes label 
            {
                font-size:20px;
            }
            
            #checkbox1, #checkbox2, #checkbox3
            {
                display:none;
                background-color:#3b3a3a;
            }
            
            #checkbox1 table, #checkbox2 table, #checkbox3 table
            {
                padding:0px;
                margin:0px 0px 0px 10px;
                width:100%;
                border-spacing: 0px;
                border-collapse: separate;
                background-color:#3b3a3a;
                vertical-align:top;
            }
            
            #checkbox1 td, #checkbox2 td, #checkbox3 td
            {
                padding:15px;
            }
            
            .expand
            {
                display:table-cell;
                text-align:right;
                padding-right:10px;
            }
            
            .filter_title_text
            {
                display:table-cell;
                text-align:left;
                width:100%;
                vertical-align:middle;
				padding-left: 10px;
            }
	    }
    </style>
</asp:Content>
<asp:Content ID="product_segment" ContentPlaceHolderID="product_segment" Runat="Server">
<div class="segment_fixed">
    <div style="background-image:url('/images/products/red_bar_wide.png'); background-position:bottom left; background-repeat:repeat-x; overflow: auto; position:relative;">
        <div class="row" style="overflow: hidden;">
            <div class="segment_bar">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td valign="bottom" class="segment_slant_td" style="height:58px;  background-image:url('/images/products/red_bar_left.png'); 
                            background-position:bottom right; background-repeat:no-repeat;"><div class="segment_slants">
                            <a href="/products/"><img src="/images/products/arrow_back.png" alt="Back" class="back_arrow" border="0" /></a>
                            </div></td>
                        <td style="width:100%; background-color:#8d0a0d;">
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td valign="top" class="icon"><img src="/images/products/fryer_icon.png" alt="Fryers" style="width:100%; max-width:45px; max-height:41px;" /></td>
                                    <td>&nbsp;<h1>Commercial Fryers for Restaurants</h1></td>
                                </tr>
                            </table>
                        </td>
                        <td valign="bottom" class="segment_slant_td2" style="height:58px; background-image:url('/images/products/red_bar_right.png'); 
                            background-position:bottom left; background-repeat:no-repeat;"><div class="segment_slants"></div></td>
                    </tr>
                </table>
            </div>
        </div>
    </div> 
</div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div class="segment_body">
    <div style="background-color:#060606; width:100%; height:auto; overflow:auto; 
        background-image: url('/images/products/segment_body_bg.jpg'); background-repeat:repeat-x; background-position:bottom left; overflow:auto;">
    <div id="left_fade_product1">
    <div id="right_fade_product1">
        <div class="row" style="overflow:hidden;">
            <div class="row_padding">
                <div class="twocolumn"><img src="/images/products/fryers_hero.png" alt="Fryers" style="width:100%; max-width:521px; vertical-align:bottom;" />
                <img src="/images/products/fryers_2012.png" alt="Best in Class" class="segment_bestinclass" />
                </div>
                <div class="twocolumn">
                    <div class="row_padding" id="segment_desc">
                        <CMS:ContentBlock ID="ContentBlock1" runat="server" DefaultContentID="5234" /> 
                    </div>
                </div>
            </div>
        </div>
    </div>
    </div>
    </div>
    <div style="margin:0 auto; text-align:center; width:100%; position:relative; z-index:2000; border-top: 2px solid #353535;"><img src="/images/black_arrow.png" style="position:relative; top:-4px; margin:0 auto; text-align:center;" /></div>
    <br style="clear:both;" />
    <div class="body_wrapper">
    <div class="body_wrapper2">
        <div id="content">
            <div class="row_padding">
                <div id="checkboxes" style="display:table;">
                    <div class="filter" style="color:#c4c3c3; font-size:24px; font-family:'futura medium condensed', Arial, Sans-Serif;">Narrow product options by:</div>               
                    <div class="filter">
                         <div class="filter_title" id="filter_title1"><div class="filter_title_text">Power Source</div><div class="expand"><img src="/images/plus.jpg" id="expand1" /></div></div>
                         <div id="checkbox1"><asp:CheckBoxList runat="server" ID="powersource" name="powersource" AutoPostBack="True"></asp:CheckBoxList></div>
                    </div>
                    <div class="filter">
                         <div class="filter_title" id="filter_title2"><div class="filter_title_text">Oil Capacity per Fryer</div><div class="expand"><img src="/images/plus.jpg" id="expand2" /></div></div>
                         <div id="checkbox2"><asp:CheckBoxList runat="server" ID="size" name="size" AutoPostBack="True" RepeatColumns="3"></asp:CheckBoxList></div>
                    </div>
                    <div class="filter">
                        <div class="filter_title" id="filter_title3"><div class="filter_title_text">Battery</div><div class="expand"><img src="/images/plus.jpg" id="expand3" /></div></div>
                         <div id="checkbox3"><asp:CheckBoxList runat="server" ID="battery" name="battery" AutoPostBack="True"></asp:CheckBoxList></div>
                    </div>
                </div>
                <div style="float:right; text-align:right;">
                	<br />
                    <table cellpadding="0" cellpadding="0" width="260px">
                        <tr>
                            <td align="right" style=""><p style="font-family:'futura medium condensed', Arial, sans-serif; font-size:20px; color:#c4c3c3;">Results view:</p></td>
                            <td align="right"  width="40"><asp:ImageButton runat="server" id="btnGrid" ImageUrl="/images/grid.jpg" OnClick="showGrid" /></td>
                            <td align="center" width="50"><asp:ImageButton runat="server" id="btnList" ImageUrl="/images/list_on.jpg" OnClick="showList" /></td>
                        </tr>
                    </table>
                    <br />
                </div>
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                         <br />   <br />
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
                         
                        <div id="full_list" runat="server" />
                        <asp:PlaceHolder ID="ph" runat="server" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="powersource" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="size" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="battery" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <br style="clear:both;" />
            
            <div class="also_view_callouts">
                <div class="row">
                    <div class="row_padding">
                        <h2 class="red_h2" style='border-bottom:1px solid #6c6c6a; margin-bottom:15px; text-transform:uppercase;'>People Have Also Viewed</h2>
                        <CMS:Collection runat="server" ID="Collection1" DefaultCollectionID="9" DoInitFill="true" DisplayXslt="/xsl/also_view.xsl" GetHtml="true"  />
                    </div>
                </div>
            </div>
            
            <br style="clear:both;" />
            
			<div class="banner">
				<a href="/sustainability/"><img src="/images/products/banners/sustainability.png" border="0" onClick="ga('send', 'event', 'Product Callout', 'Sustainability');" /></a>
			</div>
			<br style="clear:both;" />
        </div>
    </div>
    </div>
</div>
</asp:Content>
<asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
    <script type="text/javascript" src="/js/segment.js"></script>
    
    <script type="text/javascript" defer>
        $(document).ready(function() {
            $(".alsoview_overlay").attr("src", "/images/products/also_view_red.png");
        });
		
		function GridTracking() {
			ga('send', 'event', 'Toggle Product View', 'Grid View');
		}
		
		function ListTracking() {
			ga('send', 'event', 'Toggle Product View', 'List View');
		}
		
		/*Event Tracking for Product Filtering */
		function TrackFiltering(filters) {
			ga('send', 'event', 'Fryers', filters);
		}
    </script>
</asp:Content>