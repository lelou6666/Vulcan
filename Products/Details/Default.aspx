﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<%@ Register assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.WebControls" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<link href="/swiper/tabs/swiper.css" rel="stylesheet" />
<!--[if IE 8]>
<style type="text/css">
    .scrollbar1 { width: 100%; clear: both; }
    .scrollbar1 .viewport { width: 95%; height: 370px; overflow: hidden; position: relative; }
    .scrollbar1 .overview { list-style: none; position: absolute; left: 0; top: 0; }
    .scrollbar1 .thumb .end,
    .scrollbar1 .thumb { background-color: #85878d; border-radius: 8px; }
    .scrollbar1 .scrollbar { position: relative; float: right; width: 15px;}
    .scrollbar1 .track { background-color: #222326; height: 100%; width:13px; position: relative; padding: 0; border-radius: 8px;}
    .scrollbar1 .thumb { height: 20px; width: 13px; cursor: pointer; overflow: hidden; position: absolute; top: 0; }
    .scrollbar1 .thumb .end { overflow: hidden; height: 5px; width: 13px; }
    .scrollbar1 .disable{ display: none; }
    .noSelect { user-select: none; -o-user-select: none; -moz-user-select: none; -khtml-user-select: none; -webkit-user-select: none; }
 </style>
 <![endif]-->   
 
    <!-- Keep CSS on page for Swiper Custom CSS -->
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
		}
		
         .swiper-wrapper[style], .swiper-slide[style]
         {
             height:auto !important;
         }

        .tabs
        {
            margin:0 auto; 
            width:355px;
            text-align:center;
            position:relative;
            margin-top:10px;
          
        }
        .tabs a
        {
            font-family: 'futura medium condensed', Arial, Sans-Serif;
            font-size: 26px;
            color:#fdfdfd;
            text-align:center;
            border:2px solid #414141;
            background:#171717;
            text-decoration:none;
            padding:8px 15px;
            }
            
        .tabs img
        {  
            display:none;
        }
           
        .tabs a.active img
        {  
            display:inline;
            border:none;
        }   
        
        .tabs a.active
        {
            background: #891123;
        }
        
        .tab_arrow1
        {
            position:absolute; 
            top:39px; 
            left:125px;
        }
        
        .tab_arrow2
        {
            position:absolute; 
            top:39px; 
            left:272px;
        }
        
        .tab_arrow3
        {
            position:absolute; 
            top:39px; 
            left:165px;
        }
        
        .scrollbar1  ul
        {
            list-style-type:none;
     
        }
        .scrollbar1 li
        {
            padding-left:15px;
            background-image: url('/images/spec_arrow.png');
            background-repeat: no-repeat;
        }
        
        #fryer_control 
        {
            display:none;
        }
        
        .tool  a
        {
            color:#d4d3d3;
        }
		
		#best_in_class {
			width:110px;
			height:110px;
		}
      
        
        @media all and (max-width:390px) 
	    {
	        .model_img 
	        {
	            display:none;
	        }
		    .tabs 
		    {
		        width:290px;
		        margin:0px;
		    }
		    
		    .tabs a
            {
                font-size: 20px;
            }
		    
		    .article_tab
		    {
		        margin: 0 3px;
		    }
		    
		    .tab_arrow1
            {
                top:32px; 
                left:100px;
            }
            
            .tab_arrow2
            {
                top:32px; 
                left:225px;
            }
            
            #toolbox_container
            {
                display:none;
            }
            
            #product_info .row_padding 
            {
                padding:0px;
            }
            
            .detail_padding 
            {
                padding-left:10px;
            }
	    }

.Show_Resources
{
    cursor:pointer;
    font-family: 'futura light', Arial, Sans-Serif;
    font-size:19px;
    color:#ffffff;
}

.Hide_Resources
{
    cursor:pointer;
}



.modelResourceSlider {
  position:absolute;
  bottom:0;
  padding-bottom:7px;
  width: 100%;
  z-index: 100;
  min-height:100%;

}

.modelResourceContent {
  display: none;
  background-color: #222222;
  margin-right:10px;
  
  height:100%;
  min-height:100%;
}

.segment_bar
{
    font-size:30px;
    line-height:40px;
}

.title_bar
{
    padding-bottom:5px;
}

.product_thumb
{
    background-color:#222222; 
    width:62px; 
    height:62px; 
    margin-bottom:8px;
    margin-left:7px;
    cursor:pointer;
}

.active_product_thumb
{
    border:1px solid #1f67ac;
}

#product_info
{
    background-image:url('/images/products/product_detail_background.png'); 
    background-repeat:repeat-x; 
    background-position:left bottom;
    overflow:auto;
}
    
    .detail_padding
    {
        padding-left:25px;
    }

@media all and (min-width:800px) 
{
    /*scrollbar */
    .scrollbar1 { width: 100%; clear: both; }
    .scrollbar1 .viewport { width: 95%; height: 370px; overflow: hidden; position: relative; }
    .scrollbar1 .overview { list-style: none; position: absolute; left: 0; top: 0; }
    .scrollbar1 .thumb .end,
    .scrollbar1 .thumb { background-color: #85878d; border-radius: 8px; }
    .scrollbar1 .scrollbar { position: relative; float: right; width: 15px;}
    .scrollbar1 .track { background-color: #222326; height: 100%; width:13px; position: relative; padding: 0; border-radius: 8px;}
    .scrollbar1 .thumb { height: 20px; width: 13px; cursor: pointer; overflow: hidden; position: absolute; top: 0; }
    .scrollbar1 .thumb .end { overflow: hidden; height: 5px; width: 13px; }
    .scrollbar1 .disable{ display: none; }
    .noSelect { user-select: none; -o-user-select: none; -moz-user-select: none; -khtml-user-select: none; -webkit-user-select: none; }
    
    .detail_padding
    {
        padding-left:25px;
    }
}

.back_arrow
{
    top:0px;
}

@media all and (max-width:670px) 
{
    .segment_bar
    {
        font-size:25px;
        line-height:28px;
    }
	
	#best_in_class {
		width:65px;
		height:65px;
	}
}

@media all and (max-width:800px) 
{
    #product_info .sixty, #product_info .forty
    {
        width:100%;
    }
    #product_info .forty
    {
        padding-bottom:40px;
    }
    #product_info
    {
        background-image:url('/images/products/product_detail_background_tall.png'); 
    }
}


.zoomPad{
	position:relative;
	float:left;
	z-index:99;
}

@media all and (min-width:815px)  {
    .zoomPad{
		cursor:crosshair;
	}
}


.zoomPreload{
   -moz-opacity:0.8;
   opacity: 0.8;
   filter: alpha(opacity = 80);
   color: #333;
   font-size: 12px;
   font-family: Tahoma;
   text-decoration: none;
   border: 1px solid #CCC;
   background-color: white;
   padding: 8px;
   text-align:center;
   background-image: url(../images/zoomloader.gif);
   background-repeat: no-repeat;
   background-position: 43px 30px;
   z-index:110;
   width:90px;
   height:43px;
   position:absolute;
   top:0px;
   left:0px;
    * width:100px;
    * height:49px;
}


.zoomPup{
	overflow:hidden;
	background-color: #FFF;
	-moz-opacity:0.6;
	opacity: 0.6;
	filter: alpha(opacity = 60);
	z-index:120;
	position:absolute;
	border:1px solid #CCC;
  z-index:101;
  cursor:crosshair;
}

.zoomOverlay{
	position:absolute;
	left:0px;
	top:0px;
	background:#FFF;
	/*opacity:0.5;*/
	z-index:5000;
	width:100%;
	height:100%;
	display:none;
  z-index:101;
}

.zoomWindow{
	position:absolute;
	left:110%;
	top:40px;
	background:#FFF;
	z-index:6000;
	height:auto;
  z-index:10000;
  z-index:110;
}
.zoomWrapper{
	position:relative;
	border:1px solid #999;
  z-index:110;
}
.zoomWrapperTitle{
	display:block;
	background:#999;
	color:#FFF;
	height:18px;
	line-height:18px;
	width:100%;
  overflow:hidden;
	text-align:center;
	font-size:10px;
  position:absolute;
  top:0px;
  left:0px;
  z-index:120;
  -moz-opacity:0.6;
  opacity: 0.6;
  filter: alpha(opacity = 60);
}
.zoomWrapperImage{
	display:block;
  position:relative;
  overflow:hidden;
  z-index:110;

}
.zoomWrapperImage img{
  border:0px;
  display:block;
  position:absolute;
  z-index:101;
}

.zoomIframe{
  z-index: -1;
  filter:alpha(opacity=0);
  -moz-opacity: 0.80;
  opacity: 0.80;
  position:absolute;
  display:block;
}

ul#thumblist{display:inline-block;}
ul#thumblist li{float:left;list-style:none;}
ul#thumblist li a{display:block; border: 1px solid #434343; width:60px; height:60px;}
ul#thumblist li a.zoomThumbActive{
    border:1px solid #5cb0eb;
}

.jqzoom{

	text-decoration:none;
	float:left;
}

</style>

</asp:Content>
    
<asp:Content ID="product_segment" ContentPlaceHolderID="product_segment" Runat="Server">
    <div class="segment_fixed">
        <div id="segment_divide" style="background-image:url('/images/products/green_bar_wide.png'); background-position:bottom left; background-repeat:repeat-x; overflow: auto; position:relative;">
            <div class="row" style="overflow: hidden;">
                <div class="segment_bar">
                    <table cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td valign="top" class="segment_slant_td" style="height:58px; background-image:url('/images/products/green_bar_left.png'); 
                                background-position:bottom right; background-repeat:no-repeat;"><div class="segment_slants">
                                <a href="/products/" id="product_back_btn"><img src="/images/products/arrow_back.png" alt="Back" class="back_arrow" border="0" /></a>
                                </div></td>
                            <td class="title_bar" style="width:100%; background-color:#379f2a;">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td valign="top" class="icon"><img src="/images/products/steamers_icon.png" id="segement_icon" style="width:100%; max-width:36px; max-height:42px;" /></td>
                                        <td><h1><asp:Literal ID="title" runat="server" /></h1></td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="bottom" class="segment_slant_td2" style="height:58px; background-image:url('/images/products/green_bar_right.png'); 
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
        <div id="product_body_padding" style="background-color:#060606; overflow:auto;">
            <!-- This contains the hidden content for videos -->
	        <div id="video1" style='display:none; max-width:1055px; margin: 0 auto;'>
		        <div class="video_padding">
		            <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video1');"><img src="/images/close.jpg" border="0" /></a></div>
		            <br style="clear:both;" />
		            <div class="responsive-container" style="padding-top:5px;">
                        <iframe class="iframe_video" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
                    </div>
		        </div>
		        <br />
		        <br />
	        </div>
	    
            <div class="row" id="product_info">
                <div style="background-image: url('/images/products/black_fade.png'); background-repeat:repeat-y; background-position:left top; overflow:hidden; position:relative;">
                <div  style="background-image: url('/images/products/tools_border.png'); background-repeat:repeat-y; background-position:right top; overflow:hidden; position:relative;">
                    <div class="row_padding">
                        <div style="width:90%;"><CMS:ContentBlock ID="product_details" runat="server" DynamicParameter="id" DisplayXslt="/xsl/product_details.xsl" /></div>
                        <div class="tool_bar" style="position:absolute; top:0px; right:-400px; cursor:pointer; overflow:hidden; z-index:3000;">
                            <table cellpadding="0" cellspacing="0">
                            <tr>
                            <td valign="top">
                                <div id="toolbox_container"><img src="/images/products/tools.png" alt="Tools" border="0" id="toolbox" runat="server" /></div>
                            </td>
                            <td valign="top" style="background-color:#060606; width:400px; height:493px;">
                                <div class="row">
                                    <asp:ListView ID="UXListView2" runat="server">
                                        <LayoutTemplate>
                                            <div runat="server" id="itemPlaceholder" style="position: absolute;"></div>
                                        </LayoutTemplate>

                                        <ItemTemplate>
                                            <div class="tool_item" style="margin:10px;">
                                                <div class="tool" style="width:50%; position:relative; float:left;">
                                                    <table style="height:100%;">
                                                        <tr>
                                                            <td valign="top">
                                                                <%#Eval("Thumbnail")%>
                                                            </td>
                                                        </tr>
                                                        </tr>
                                                            <td valign="top">
                                                                <div><p style="color:#FFFFFF;"><%#Eval("URL")%></p></div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </div>
                            </td>
                            </tr>
                            </table>
                        </div>
                    </div>
                </div>
                </div>
            </div>
         </div>
        <div style="margin:0 auto; text-align:center; width:100%; position:relative; z-index:2000; border-top: 2px solid #353535;"><img src="/images/black_arrow.png" style="position:relative; top:-4px; margin:0 auto; text-align:center;" /></div>
         
      <br style="clear:both;" />
        <br />
      <div class="body_wrapper">
      <div class="body_wrapper2">
        <div id="content">
            <div style="display:none;">
                <asp:TextBox runat="server" ID="segment"  />
                <asp:TextBox runat="server" ID="bestinclass"  />
                <asp:TextBox runat="server" ID="energystar"  />
                <asp:TextBox runat="server" ID="k_twelve"  />
                <asp:TextBox runat="server" ID="powerfry"  />
                <asp:TextBox runat="server" ID="frymate"  />
            </div>
            <div class="row_padding">
            <div runat="server" id="correctional_griddle">
            	<p>The correctional griddle security package is available on 900RX, MSA and HEG series Vulcan griddles.  In addition to the standard features listed above, visit the individual product pages for more details and documentation:<br /><br />
                <a href="/Products/900RX-Series-Gas-Griddles/">900RX Series</a><br />
                <a href="/Products/MSA-Series-Gas-Flat-Top-Griddles-Grills/">MSA Series</a><br />
                <a href="/Products/HEG-Series-Electric-Griddles/">HEG Series</a>
                <br />
                <br />
                </p>
            </div>
            <div runat="server" id="Model_Options">
                <div runat="server" id="FilterDisplay">
                    <asp:Literal runat="server" id="filter" />
                    <div><asp:LinkButton Text="Clear Filter" runat="server" id="clear" OnClick="ClearFiltering" CssClass="clear_filter" /> <img src="/images/homepage/red_arrow.png" alt="" /></div>
                    <br />
                </div>
                 <h2 style="border-bottom:1px solid #6c6c6a; margin-bottom:15px;">MODEL OPTIONS</h2>
                 <div id="fryer_control"><p>View more information about the different <a href="/uploadedFiles/Vulcan/Products/Fryers/Fryer_Controls.pdf" target="_blank">control options</a>.</p></div>
           
                 <div id="modelitems">
                    <asp:ListView ID="UXListView" runat="server">
                        <LayoutTemplate>
                            <div runat="server" id="itemPlaceholder" style="position: absolute;"></div>
                        </LayoutTemplate>
    
                        <ItemTemplate>
                            <div class="model_item">
                                <div class="model">
                                    <table style="height:100%;">
                                        <tr>
                                            <td valign="top">
                                                <%#Eval("Image")%>
                                            </td>
                                            <td valign="top">
                                                <div class="model_title"><%#Eval("Title")%></div>
                                                <div class="model_desc"><%#Eval("Description")%></div>
                                                <div class="model_desc"><%#Eval("AdditionalInfo")%></div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="bottom" colspan="2">
                                                <br />
                                                <div class="Show_Resources">RESOURCES <img src="/images/resource_arrow_blue.jpg" alt="" /></div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div style="position:relative; height:100%; min-height:100%;">
                                    <div class="modelResourceSlider">
                                      <div class="modelResourceContent">
                                        <div style="padding:15px;">
                                            <div style="position:relative; float:right; left:0px; top:0px;"><img src="/images/close_X.jpg" alt="close" class="Hide_Resources" /></div>
                                            <div class="model_title"><%#Eval("Title")%> Resources</div>
                                            <div class="specs_div"><%#Eval("SpecSheet")%></div>
                                            <div class="specs_div"><%#Eval("SellSheet")%></div>
                                            <div class="specs_div"><%#Eval("Manual")%></div>
                                            <div class="specs_div"><%#Eval("PartsCatelog")%></div>
                                            <div class="specs_div"><%#Eval("ServiceManual")%></div>
                                            <div class="specs_div"><a href="http://vulcanhart.kclcad.com/?search=<%#Eval("Title")%>" target="_blank" class='specs'>CAD and Revit</a></div>
                                            <div class="specs_div"><%#Eval("HighRes")%></div>
                                        </div>
                                      </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
                <br style="clear:both;" />
                <br />
            </div>
        </div>
        </div>
        <br style="clear:both;" />
    
        
        <div runat="server" id="also_info">
            <div class="also_view_callouts">
                <div class="row">
                    <div class="row_padding">
                        <h2 style='border-bottom:1px solid #6c6c6a; margin-bottom:15px; text-transform:uppercase;'>People Have Also Viewed</h2>
                        <CMS:Collection runat="server" ID="Collection1" DoInitFill="false" DisplayXslt="/xsl/also_view.xsl" GetHtml="true"  />
                    </div>
                </div>
        </div>
        </div>
        
        <br style="clear:both;" />
      
    	
	    <div class="banner"></div>
	    <br style="clear:both;" />
      </div>
      </div>
  </div>
</asp:Content>

<asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
    <script type="text/javascript" src="/js/fader.js"></script>
    <script type="text/javascript" src="/swiper/tabs/swiper.js"></script>
    <script type="text/javascript" src="/js/zoom.js"></script>

  <script type="text/javascript">
      var tabsSwiper = new Swiper('.swiper-container', {
          speed: 500,
          onlyExternal: true,
          onSlideChangeStart: function() {
              $(".tabs .active").removeClass('active')
              $(".tabs a").eq(tabsSwiper.activeIndex).addClass('active')

              var height = $(".swiper-slide-active .content-slide").height();
              $('.swiper-container, .swiper-slide').height(height);
          }
      })

      $(".tabs a").on('touchstart mousedown', function(e) {
          e.preventDefault()
          $(".tabs .active").removeClass('active')
          $(this).addClass('active')
          tabsSwiper.swipeTo($(this).index())
      })
      $(".tabs a").click(function(e) {
          e.preventDefault()
      })

     function ShowBestInClass() {
         if (document.getElementById('<%=bestinclass.ClientID%>').value != "true") {
             $('#best_in_class_container').hide();
         }
     }

     function ShowEnergyStar() {
         if (document.getElementById('<%=energystar.ClientID%>').value != "true") {
             $('#energy_star_container').hide();
         }
     }

     function ShowPowerFryLogos() {
         if (document.getElementById('<%=powerfry.ClientID%>').value != "true") {
             $('#powerfry_container').hide();
         }
     }

     function GetSegmentColor() {
         if (document.getElementById('<%=segment.ClientID%>').value == "Fryers") {
             $('h2').addClass("red_h2");
             $(".alsoview_overlay").attr("src", "/images/products/also_view_red.png");
             $(".segment_slant_td").css("background-image", "url('/images/products/red_bar_left.png')");
             $(".segment_slant_td2").css("background-image", "url('/images/products/red_bar_right.png')");
             $("#segment_divide").css("background-image", "url('/images/products/red_bar_wide.png')");
             $(".title_bar").css("background-color", "#8d0a0d");
             $(".alsoview_overlay").attr("src", "/images/products/also_view_red.png");
             $('#segement_icon').attr("src", "/images/products/fryer_icon.png");

             $("#product_back_btn").attr("href", "/products/fryers/");

			 if (document.getElementById('<%=frymate.ClientID%>').value != "true") {
				  $("#fryer_control").show();
			 }

             if ($(window).width() < 769) {
                 $(".segment_slant_td").css("background-color", "#8d0a0d");
             }
         }
         else if (document.getElementById('<%=segment.ClientID%>').value == "Ovens") {
             $('h2').addClass("purple_h2");
             $(".alsoview_overlay").attr("src", "/images/products/also_view_purple.png");
             $(".segment_slant_td").css("background-image", "url('/images/products/purple_bar_left.png')");
             $(".segment_slant_td2").css("background-image", "url('/images/products/purple_bar_right.png')");
             $("#segment_divide").css("background-image", "url('/images/products/purple_bar_wide.png')");
             $(".title_bar").css("background-color", "#6a016a");
             $('#best_in_class').attr("src", "/images/products/convection_ovens_2012.png");
             $('#segement_icon').attr("src", "/images/products/oven_icon.png");

             $("#product_back_btn").attr("href", "/products/ovens/");

             if ($(window).width() < 769) {
                 $(".segment_slant_td").css("background-color", "#6a016a");
             }
         }
         else if (document.getElementById('<%=segment.ClientID%>').value == "Griddles") {
             $('h2').addClass("orange_h2");
             $(".alsoview_overlay").attr("src", "/images/products/also_view_orange.png");
             $(".segment_slant_td").css("background-image", "url('/images/products/orange_bar_left.png')");
             $(".segment_slant_td2").css("background-image", "url('/images/products/orange_bar_right.png')");
             $("#segment_divide").css("background-image", "url('/images/products/orange_bar_wide.png')");
             $(".title_bar").css("background-color", "#ba371e");
             $('#segement_icon').attr("src", "/images/products/griddle_icon.png");

             $("#product_back_btn").attr("href", "/products/griddles/");

             if ($(window).width() < 769) {
                 $(".segment_slant_td").css("background-color", "#ba371e");
             }
         }
		 else if (document.getElementById('<%=segment.ClientID%>').value == "Charbroilers") {
             $('h2').addClass("orange_h2");
             $(".alsoview_overlay").attr("src", "/images/products/also_view_orange.png");
             $(".segment_slant_td").css("background-image", "url('/images/products/orange_bar_left.png')");
             $(".segment_slant_td2").css("background-image", "url('/images/products/orange_bar_right.png')");
             $("#segment_divide").css("background-image", "url('/images/products/orange_bar_wide.png')");
             $(".title_bar").css("background-color", "#ba371e");
             $('#best_in_class').attr("src", "/uploadedImages/Vulcan/Vulcan_Categories/BIC-Charbroiler-2014.png");
             $('#segement_icon').attr("src", "/images/products/charbroiler_icon.png");

             $("#product_back_btn").attr("href", "/products/charbroilers/");

             if ($(window).width() < 769) {
                 $(".segment_slant_td").css("background-color", "#ba371e");
             }
         }
         else if (document.getElementById('<%=segment.ClientID%>').value == "Ranges") {
             $('h2').addClass("blue_h2");
             $(".alsoview_overlay").attr("src", "/images/products/also_view_blue.png");
             $(".segment_slant_td").css("background-image", "url('/images/products/blue_bar_left.png')");
             $(".segment_slant_td2").css("background-image", "url('/images/products/blue_bar_right.png')");
             $("#segment_divide").css("background-image", "url('/images/products/blue_bar_wide.png')");
             $(".title_bar").css("background-color", "#1e64a6");
             $('#best_in_class').attr("src", "/uploadedImages/Vulcan/Vulcan_Categories/BIC-Range-2014.png");
             $('#segement_icon').attr("src", "/images/products/ranges_icon.png");

             $("#product_back_btn").attr("href", "/products/ranges/");

             if ($(window).width() < 769) {
                 $(".segment_slant_td").css("background-color", "#1e64a6");
             }
         }
         else if (document.getElementById('<%=segment.ClientID%>').value == "Heated Holding") {
             $('h2').addClass("aqua_h2");
             $(".alsoview_overlay").attr("src", "/images/products/also_view_aqua.png");
             $(".segment_slant_td").css("background-image", "url('/images/products/aqua_bar_left.png')");
             $(".segment_slant_td2").css("background-image", "url('/images/products/aqua_bar_right.png')");
             $("#segment_divide").css("background-image", "url('/images/products/aqua_bar_wide.png')");
             $(".title_bar").css("background-color", "#01866d");
             $('#segement_icon').attr("src", "/images/products/holding_icon.png");

             $("#product_back_btn").attr("href", "/products/heated-holding/");

             if ($(window).width() < 769) {
                 $(".segment_slant_td").css("background-color", "#01866d");
             }
         }
		 else if (document.getElementById('<%=segment.ClientID%>').value == "Combi") {
             $('h2').addClass("yellow_h2");
             $(".alsoview_overlay").attr("src", "/images/products/also_view_yellow.png");
             $(".segment_slant_td").css("background-image", "url('/images/products/yellow_bar_left.png')");
             $(".segment_slant_td2").css("background-image", "url('/images/products/yellow_bar_right.png')");
             $("#segment_divide").css("background-image", "url('/images/products/yellow_bar_wide.png')");
             $(".title_bar").css("background-color", "#e3ae30");
             $('#segement_icon').attr("src", "/images/products/combi_icon.png");

             $("#product_back_btn").attr("href", "/products/combi/");

             if ($(window).width() < 769) {
                 $(".segment_slant_td").css("background-color", "#e3ae30");
             }
         }
         else {
             $('h2').addClass("green_h2");
             $(".alsoview_overlay").attr("src", "/images/products/also_view_green.png");
             $(".segment_slant_td").css("background-image", "url('/images/products/green_bar_left.png')");
             $(".segment_slant_td2").css("background-image", "url('/images/products/green_bar_right.png')");
             $("#segment_divide").css("background-image", "url('/images/products/green_bar_wide.png')");
             $(".title_bar").css("background-color", "#379f2a");

             if ($(window).width() < 769) {
                 $(".segment_slant_td").css("background-color", "#379f2a");
             }

             if (document.getElementById('<%=segment.ClientID%>').value == "Braising Pans") {
                 $('#segement_icon').attr("src", "/images/products/braising_pan_icon.png");
                 $("#product_back_btn").attr("href", "/products/braising-pans/");
             }
             else if (document.getElementById('<%=segment.ClientID%>').value == "Steamers") {
                 $('#segement_icon').attr("src", "/images/products/steamers_icon.png");
                 $("#product_back_btn").attr("href", "/products/steamers/");
             }
             else if (document.getElementById('<%=segment.ClientID%>').value == "Kettles") {
                 $('#segement_icon').attr("src", "/images/products/kettles_icon.png");
                 $("#product_back_btn").attr("href", "/products/kettles/");
             }
         }
     }

     function ShowBanner() {
         if (document.getElementById('<%=k_twelve.ClientID%>').value != "true") {
             $(".banner").append("<a href=\"/k-12/\"><img src=\"/images/products/banners/k12.png\" border=\"0\" /></a>");
         }
         else if (document.getElementById('<%=segment.ClientID%>').value == "Fryers") {
             $(".banner").append("<a href=\"/sustainability/\"><img src=\"/images/products/banners/sustainability.png\" border=\"0\" /></a>");
         }
         else if (document.getElementById('<%=segment.ClientID%>').value == "Charbroilers" ) {
             $(".banner").append("<a href=\"/irx/\"><img src=\"/images/products/banners/irx.png\" border=\"0\" /></a>");
         }
     }

     $(document).ready(function() {
         $('.Show_Resources').click(function() {
             var cHeight = ($(this).parent().parent().parent().height() + 20) + 'px';

             $(this).parent().parent().parent().parent().parent().parent().find('.modelResourceSlider .modelResourceContent').css('height', cHeight);

             $(this).parent().parent().parent().parent().parent().parent().find('.modelResourceSlider .modelResourceContent').slideToggle({
                 direction: "up"
             }, 350);
         }); // end click

         $('.Hide_Resources').click(function() {
             $(this).parent().parent().parent().parent().parent().parent().find('.modelResourceSlider .modelResourceContent').slideToggle({
                 direction: "up"
             }, 350);
         }); // end click

         GetSegmentColor();
         ShowBanner();
         ShowBestInClass();
         ShowEnergyStar();
         ShowPowerFryLogos();

         if ($(window).width() > 799) {
             $('.scrollbar1').tinyscrollbar();
         }

			$('#video_thm').click(function(event) {
				showVideo("#video1");

                $('.iframe_video').attr("src", "http://player.vimeo.com/video/" + $('#video_thm').find("img").attr('alt') + "?badge=0&portrait=0&title=0&byline=0&api=1");
			});

         var close = "yes";
         $(".tool_bar").click(function() {
             if (close == "yes") {
                 $(".tool_bar").animate({
                     right: "+=400"
                 }, 800, function() {
                 });
                 close = "no";
             }
             else {
                 $(".tool_bar").animate({
                     right: "-=400"
                 }, 800, function() {
                 });
                 close = "yes";
             }
         });
     });


     $('.model').resize(function() {
         SizeModels();
     });

     function SizeModels() {
         //This script makes models in the same row the same height
         var currentTallest = 0,
                 currentRowStart = 0,
                 rowDivs = new Array(),
                 $el,
                 topPosition = 0;

         $('.model').each(function() {
             $el = $(this);
             topPostion = $el.position().top;

             if (currentRowStart != topPostion) {

                 // we just came to a new row.  Set all the heights on the completed row
                 for (currentDiv = 0; currentDiv < rowDivs.length; currentDiv++) {
                     rowDivs[currentDiv].height(currentTallest);
                 }

                 // set the variables for the new row
                 rowDivs.length = 0; // empty the array
                 currentRowStart = topPostion;
                 currentTallest = $el.height() + 40;
                 rowDivs.push($el);


             } else {

                 // another div on the current row.  Add it to the list and check if it's taller
                 rowDivs.push($el);
                 currentTallest = (currentTallest < $el.height()) ? ($el.height()) : (currentTallest);

             }

             // do the last row
             for (currentDiv = 0; currentDiv < rowDivs.length; currentDiv++) {
                 rowDivs[currentDiv].height(currentTallest);
             }

         });
     }

     function showVideo(video) {
         $(video).fadeIn(800);
         $("#product_info").hide();
     }

     function hideVideo(video) {
         $(video).hide();
         $("#product_info").fadeIn(800);

         var data = { method: 'pause' };
         // This pauses all frames within tabs container.
         $("iframe").each(function() {
             var url = $(this).attr('src').split('?')[0];
             this.contentWindow.postMessage(JSON.stringify(data), url);
         });
     }
	 
	 $(document).ready(function() {
		 if( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent) ) {
			 $('.zoom').jqzoom({
					zoomWidth: $(".viewport").width() + 10,
					zoomHeight: $(".viewport").height()  + 70,
					xOffset: 15,
					zoomType: 'standard',
					preloadImages: false,
					title: false,
					windowWidth: 10
				});
			}
			else {
				$('.zoom').jqzoom({
					zoomWidth: $(".viewport").width() + 10,
					zoomHeight: $(".viewport").height()  + 70,
					zoomType: 'standard',
					lens:true,
					preloadImages: false,
					alwaysOn:false,
					title: false,
					showEffect: 'fadein',
					hideEffect: 'fadeout',
					windowWidth: $(window).width()
				});
			}

	});
</script>
</asp:Content>