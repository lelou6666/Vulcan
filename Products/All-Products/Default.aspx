﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register Src="~/products/productlist.ascx" TagName="productlist" TagPrefix="productlist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<link rel="canonical" href="http://www.vulcanequipment.com/products/all-products/" />
<style type="text/css">
  .prod_listview ul {
			list-style:none;
	}
	
	.prod_listview li {
		background-image: url("/images/spec_arrow_blue.png");
		background-repeat: no-repeat;
		padding-left: 15px;
	}
	
	h2 {
		padding-bottom:5px;	
		text-transform:uppercase;
	}
	
	.blue_h2 {
		font-size: 28px;
		margin-top:10px;
		cursor:pointer;
	}
	
	.blue_h2 span {
		font-family: 'helvetica', Arial, Sans-Serif;
		font-size: 33px;
		font-weight:bold;
	}
	
	#select_text {
		font-family: 'futura medium condensed', Arial, Sans-Serif;
		font-size: 24px;
		line-height: 30px;
		padding-top: 5px;
		color: #c4c3c3;
		width: 45%;
		position: relative;
		float: left;
		text-align: right;
	}
	
	#contact_us select 
		{
		    width:240px;
		}
		
		#contact_us 
		{
		     width:55%; 
		     position:relative; 
		     float:left;
		}
		
	.prod_listview {
		display:none;
		margin-bottom:25px;
	}
	
	@media all and (max-width:545px) 
	{
		#select_text
		{
			font-size:18px;
			width:40%;
		}
		
		#contact_us select 
		{
			font-size:14px;
			width:200px;
		}
	}
		
	@media all and (max-width:450px) 
        {
            #select_text
		    {
		        width:100%;
		        float:right;
		        text-align:center;
		    }
		    #contact_us  
		    {
		        text-align:center;
		        margin:0 auto;
		        width:100%;
		    }
        }
</style>
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div style="overflow:auto; background-color:#494949; background-image: url('/images/video_library_header_bg.jpg'); background-position:bottom left; background-repeat:repeat-x;">
    <div class="row">
        <div class="row_padding"> 
            <h1>All Products</h1>
           
            <div style="background-color:#262626; padding:15px; overflow:auto; max-width:500px; margin:0 auto;">
                <div id="select_text">Select a Product Segment:&nbsp;&nbsp;</div>
                <div id="contact_us">
                    <select id="category" onchange="goToCat();" style="vertical-align:middle; padding-top:10px;">
                      <option value="">Product Segment</option>
                      <option value="Fryers">Fryers</option>
                      <option value="Ranges">Ranges</option>
                      <option value="Ovens">Ovens</option>
                      <option value="Griddles">Griddles</option>
                      <option value="Charbroilers">Charbroilers</option>
                      <option value="Steamers">Steamers</option>
                      <option value="Kettles">Kettles</option>
                      <option value="Braising">Braising Pans</option>
                      <option value="Heated">Heated Holding</option>
                      <option value="Combi">Combi</option>
                    </select>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="dark_body_wrapper">
<div class="dark_body_wrapper2">
<div style="margin:0 auto; text-align:center; width:100%; position:relative; z-index:2000; border-top:3px solid #494949;"><img src="/images/video_arrow.png" style="position:relative; top:-4px; margin:0 auto; text-align:center;" /></div>
   
	<div class="row">
    	<div class="row_padding">
            <h2 id="Fryers">Fryers</h2>
            <div id="full_list" runat="server" />
            <asp:PlaceHolder runat="server" ID="ph" />
            <br /><br />
            <h2 id="Ranges">Ranges</h2>
            <div id="full_list2" runat="server" />
            <asp:PlaceHolder runat="server" ID="ph2" />
            <br /><br />
            <h2 id="Ovens">Ovens</h2>
            <div id="full_list3" runat="server" />
            <asp:PlaceHolder runat="server" ID="ph3" />
            <br /><br />
            <h2 id="Griddles">Griddles</h2>
            <div id="full_list4" runat="server" />
            <asp:PlaceHolder runat="server" ID="ph4" />
            <br /><br />
            <h2 id="Charbroilers">Charbroilers</h2>
            <div id="full_list10" runat="server" />
            <asp:PlaceHolder runat="server" ID="ph10" />
            <br /><br />
            <h2 id="Steamers">Steamers</h2>
            <div id="full_list5" runat="server" />
            <asp:PlaceHolder runat="server" ID="ph5" />
            <br /><br />
            <h2 id="Kettles">Kettles</h2>
            <div id="full_list6" runat="server" />
            <asp:PlaceHolder runat="server" ID="ph6" />
            <br /><br />
            <h2 id="Braising">Braising Pans</h2>
            <div id="full_list7" runat="server" />
            <asp:PlaceHolder runat="server" ID="ph7" />
            <br /><br />
            <h2 id="Heated">Heated Holding</h2>
            <div id="full_list8" runat="server" />
            <asp:PlaceHolder runat="server" ID="ph8" />
            <br /><br />
            <h2 id="Combi">Combi</h2>
            <div id="full_list9" runat="server" />
            <asp:PlaceHolder runat="server" ID="ph9" />
            <br />
            <br />
            <br />
        </div>
    </div>
</div>
</div>
</asp:Content>
<asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
	<script type="text/javascript" defer>
	function goToCat() {
		var e = document.getElementById("category");
		var vSelected = e.options[e.selectedIndex].value;

		var goTo = "#" + vSelected;
		if (vSelected != "") {
			$("html, body").animate({ scrollTop: $(goTo).offset().top - 150 }, 700);
		}
	}
	</script>
    
    <script type="text/javascript" defer>
		$(document).ready(function() {
			$(".blue_h2").click(function() {
				if($(this).children("span").html() == " –") {
					$(this).children("span").html(" +");
				}
				else {
					$(this).children("span").html(" –");
				}
				
				$(this).next(".prod_listview").slideToggle();
			});
		});
	</script>
</asp:Content>