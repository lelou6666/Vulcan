﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.WebControls" tagprefix="asp" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="cms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/sustainability/" />
    <style type="text/css">
        h3 {
            padding-bottom:20px;
        }
        
        .green_segment_title
        {
            font-family: 'futura medium condensed', Arial, Sans-Serif;
            font-size:32px;
            line-height: 35px;
            color:#f1f1f1;
            height:55px;
        }
        
        .green_segment_body
        {
            background-color:#2c2c2c;
            overflow:auto;
            min-height:250px;
        }
        
        #fryer_title
        {
            width:100%;
            background-color:#911013; 
        }
        
        #heated_holding_title
        {
            width:100%;
            background-color:#0c7561;
        }
        
        #ovens_title
        {
            width:100%;
            background-color:#741274;
        }
        
        #steamers_title
        {
            width:100%;
            background-color:#38872e;
        }
        
        #griddles_title
        {
            width:100%;
            background-color:#af412b;
        }
        
        #heated_holding, #ovens
        {
            margin-right:15px;
        }
        
        #griddles, #steamers
        {
            margin-left:15px;
        }
       
        #column1 
        {
            width:75%; 
        }
        
        #column2
        {
            width:25%; 
        }
        
        #green_fryer_img
        {
            width:100%; 
            max-width:240px; 
            position:absolute; 
            top:7px; 
            right:0px;
        }
        
        .product_height
        {
            height:45px;
        }
        
        .body_right
        {
            width:25%;
        }
        
        .body_left
        {
            width:75%;
        }
        
        .body_right2
        {
            width:40%;
        }
        
        .body_left2
        {
            width:60%;
        }
        
        @media all and (max-width:890px) 
	    {
            .green_segment_body .twocolumn 
            {
                width:100%;
            }
            
            .green_twocolumn  .twocolumn 
            {
                width:100%;
            }
            
            .green_segment_body
            {
                min-height:150px;
            }
            
            #heated_holding, #ovens
            {
                margin-left:15px;
            }
            
            #griddles, #steamers
            {
                margin-right:15px;
            }
            
            .green_segment_body
            {
                margin-bottom:30px;
            }
	    }
	    
	    @media all and (max-width:800px) 
	    {
	        #column1 
            {
                width:100%; 
            }
            
            #column2
            {
                width:0%; 
            }
            
            #green_fryer_img
            {
                width:150px;
            }
	    }
	    
	    @media all and (max-width:720px) 
	    {
	        #irx_header .forty, #irx_header .sixty , #irx_body1 .twocolumn
	        {
	            width:100%;
	        }
	        
	         #irx_body1 .twocolumn
	         {
	             padding-top:15px;
	         }
	         
	         .product_height
            {
                height:auto;
            }
	    }
	    
	    @media all and (max-width:680px) 
	    {
	        #green_fryer_img
            {
                display:none;
            }
            
            .body_left2
            {
                width:100%;
            }
	    }
	    
	    @media all and (max-width:545px) 
	    {
	        .side_img
	        {
	            display:none;
	        }
	        
	        .body_right, .body_right2
	        {
	            width:0%;
	        }
	        
	        .body_left, body_left2
	        {
	            width:100%;
	        }
	        
	        .green_segment_title
            {
                font-size:24px;
            }
	    }
    </style>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
 <div id="irx_header" class="dots_background" style="overflow:auto;">
     <div class="row">
        <br />
        <div class="forty">
            <div class="row_padding">
                <img src="/images/green.png" alt="Green" style="width:100%; vertical-align:bottom; max-width:421px;" />
            </div>
        </div>
        <div class="sixty">
            <div class="row_padding">
                <CMS:ContentBlock ID="ContentBlock2" runat="server" DefaultContentID="1225" /> 
                <br />
                <div style="width:100%; text-align:center;"><a href="http://www.energystar.gov/index.cfm?fuseaction=CFSrebate.CFSrebate_locator" target="_blank"><img src="/images/rebate_finder.png" alt="Energy Star Rebate Finder" border="0" style="margin:0 auto; padding-bottom:15px;" /></a></div>
            </div>
        </div>
     </div>
</div>

<div id="irx_body1" style="overflow:auto; border-bottom:2px solid #464646;">
    <div class="body_wrapper">
    <div class="body_wrapper2">
      <div style="margin:0 auto; text-align:center; width:100%; position:relative; z-index:2000; border-top: 2px solid #353535;"><img src="/images/gray_arrow.png" style="position:relative; top:-4px; margin:0 auto; text-align:center;" /></div>
         <div style="overflow:auto; padding:15px 0 35px 0;">
             <CMS:ContentBlock ID="ContentBlock1" runat="server" DefaultContentID="1608" /> 
         </div>
     </div>
     </div>
</div>

<div id="irx_body2" style="overflow:auto;">
      <div class="dark_body_wrapper">
      <div class="dark_body_wrapper2">
        <div class="row" style="overflow:auto; padding-top:15px;">
             <div style="width:100%; text-align:center; vertical-align:middle; margin-top:20px; margin-bottom:20px;"><h3>Vulcan energy efficient products</h3></div>
             <div id="fryers" style="position:relative;">
                <img id="green_fryer_img" src="/images/green_fryer.png" />
                
                <div id="fryer_title" class="green_segment_title">
                    <div style="padding:9px 0 0 10px;"><img src="/images/products/fryer_icon.png" style="height:32px;" /> FRYERS</div>
                </div>
                <div class="green_segment_body">
                    <div id="column1" style="position:relative; float:left; padding-top: 20px;">
                        <asp:ListView ID="UXListView" runat="server" EnableViewState="false">
                            <LayoutTemplate>
                                <div runat="server" id="itemPlaceholder" style="position: absolute;"></div>
                            </LayoutTemplate>

                            <ItemTemplate>
                                <div class="twocolumn">
                                    <div class="row_padding">
                                        <table cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td valign="top"><img src="/images/spec_arrow.png" /></td>
                                                <td><p class="product_height"><a href="<%#Eval("Alias")%>" style="color:#fdfcfc;"><%#Eval("Title")%></a></p>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:ListView>
                    </div>
                    <div id="column2" style="position:relative; float:left;">
                        <div class="row_padding">
                            
                        </div>
                    </div>
                </div>
             </div>
        </div>   
        <br style="clear:both;" />
        <div class="row green_twocolumn" style="overflow:auto; padding-top:30px;">
             <div class="twocolumn">
                 <div id="heated_holding" style="position:relative;">
                    <img class="side_img" src="/images/green_headed_holding.png" style="width:100%; max-width:106px; position:absolute; top:7px; right:0px;" />
                    <div id="heated_holding_title" class="green_segment_title">
                        <div style="padding:9px 0 0 10px;"><img src="/images/products/holding_icon.png" style="height:32px;" /> HEATED HOLDING</div>
                    </div>
                    <div class="green_segment_body">
                        <div class="body_left" style="position:relative; float:left; padding-top: 20px;">
                        <asp:ListView ID="ListView1" runat="server" EnableViewState="false">
                            <LayoutTemplate>
                                <div runat="server" id="itemPlaceholder" style="position: absolute;"></div>
                            </LayoutTemplate>

                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td valign="top" style="padding-left:15px;"><img src="/images/spec_arrow_aqua.png" /></td>
                                        <td><p class="product_height"><a href="<%#Eval("Alias")%>" style="color:#fdfcfc;"><%#Eval("Title")%></a></p></td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:ListView>
                        </div>
     
                        <div class="body_right" style="position:relative; float:left;">
                            <div class="row_padding">
                                
                            </div>
                        </div>
                    </div>
                 </div>
             </div>
             <div class="twocolumn">
                 <div id="griddles" style="position:relative;">
                    <div id="griddles_title" class="green_segment_title">
                        <div style="padding:9px 0 0 10px;"><img src="/images/products/griddle_icon.png" style="height:32px;" /> GRIDDLES & CHARBROILERS</div>
                    </div>
                    <div class="green_segment_body">
                        <div class="body_left2" style="position:relative; float:left; padding-top: 20px;">
                        <asp:ListView ID="ListView2" runat="server" EnableViewState="false">
                            <LayoutTemplate>
                                <div runat="server" id="itemPlaceholder" style="position: absolute;"></div>
                            </LayoutTemplate>

                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td valign="top" style="padding-left:15px;"><img src="/images/spec_arrow_orange.png" /></td>
                                        <td><p class="product_height"><a href="<%#Eval("Alias")%>" style="color:#fdfcfc;"><%#Eval("Title")%></a></p></td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:ListView>
                        </div>
     
                        <div class="body_right2" style="position:relative; float:left;">
                            <div class="row_padding">
                                <img class="side_img" src="/images/green_griddles.png" style="width:100%; max-width:188px; position:absolute; top:7px; right:0px;" />
                            </div>
                        </div>
                    </div>
                 </div>
             </div>
        </div>   
        <br style="clear:both;" />
        <div class="row green_twocolumn" style="overflow:auto; padding-top:30px;">
             <div class="twocolumn">
                 <div id="ovens" style="position:relative;">
                    <img class="side_img" src="/images/green_oven.png" style="width:100%; max-width:120px; position:absolute; top:7px; right:0px;" />
                    <div id="ovens_title" class="green_segment_title">
                        <div style="padding:9px 0 0 10px;"><img src="/images/products/oven_icon.png" style="height:32px;" /> OVENS</div>
                    </div>
                    <div class="green_segment_body">
                        <div class="body_left" style="position:relative; float:left; padding-top: 20px;">
                        <asp:ListView ID="ListView3" runat="server" EnableViewState="false">
                            <LayoutTemplate>
                                <div runat="server" id="itemPlaceholder" style="position: absolute;"></div>
                            </LayoutTemplate>

                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td valign="top" style="padding-left:15px;"><img src="/images/spec_arrow_purple.png" /></td>
                                        <td><p class="product_height"><a href="<%#Eval("Alias")%>" style="color:#fdfcfc;"><%#Eval("Title")%></a></p></td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:ListView>
                        </div>
     
                        <div class="body_right" style="position:relative; float:left;">
                            <div class="row_padding">
                                
                            </div>
                        </div>
                    </div>
                 </div>
             </div>
           
             <div class="twocolumn">
                 <div id="steamers" style="position:relative;">
                    <img class="side_img" src="/images/green_steam.png" style="width:100%; max-width:127px; position:absolute; top:7px; right:0px;" />
                    <div id="steamers_title" class="green_segment_title">
                        <div style="padding:9px 0 0 10px;"><img src="/images/products/steamers_icon.png" style="height:32px;" /> STEAMERS</div>
                    </div>
                    <div class="green_segment_body">
                        <div class="body_left" style="position:relative; float:left; padding-top: 20px;">
                        <asp:ListView ID="ListView4" runat="server" EnableViewState="false">
                            <LayoutTemplate>
                                <div runat="server" id="itemPlaceholder" style="position: absolute;"></div>
                            </LayoutTemplate>

                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td valign="top" style="padding-left:15px;"><img src="/images/spec_arrow_green.png" /></td>
                                        <td><p class="product_height"><a href="<%#Eval("Alias")%>" style="color:#fdfcfc;"><%#Eval("Title")%></a></p></td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:ListView>
                        </div>
     
                        <div class="body_right" style="position:relative; float:left;">
                            <div class="row_padding">
                                
                            </div>
                        </div>
                    </div>
                 </div>
             </div> 
        </div>
        <br /><br />
     </div>
     </div>
</div>
</asp:Content>