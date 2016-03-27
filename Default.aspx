<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" MaintainScrollPositionOnPostback="true"  %>
<%@ Register Src="/includes/downloads.ascx" TagName="Downloads" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/" />
    <link href="/swiper/swiper.css" rel="stylesheet" />
  
  <style type="text/css">
    .swiper-slide a
    {
        cursor:pointer;
    }
    
   
    
    /* Swiper customized - for homepage */

@media all and (max-width:1075px) 
	{
	    .swiper-container{max-height:273px;}
		.small-swiper[style]
	    {
	        width:300px !important;
	    }
	    
	    .swiper-wrapper img
	    {
	        width:300px;
	    }
	    
	    .large-swiper[style]
	    {
	        width:601px !important;
	    }
	    
	    .large-swiper img
	    {
	        width:601px;
	    }
	}
	
	@media all and (max-width:700px) 
	{
	    .swiper-container{max-height:183px;}
		.small-swiper[style]
	    {
	        width:200px !important;
	    }
	    
	    .swiper-wrapper img
	    {
	        width:200px;
	    }
	    
	    .large-swiper[style]
	    {
	        width:400px !important;
	    }
	    
	    .large-swiper img
	    {
	        width:400px;
	    }
	}
	
	@media all and (max-width:500px) 
	{
	    .swiper-container{max-height:141px;}
		.small-swiper[style]
	    {
	        width:154px !important;
	    }
	    
	    .swiper-wrapper img
	    {
	        width:154px;
	    }
	    
	    .large-swiper[style]
	    {
	        width:308px !important;
	    }
	    
	    .large-swiper img
	    {
	        width:308px;
	    }
	}
    
  </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="slider_main" Runat="Server">

<!-- This contains the hidden content for videos -->
        <div id="slider_video" style='display:none; max-width:1055px; margin: 0 auto;'>
            <br /><br />
            <div class="video_padding" id='slider_video_content'>
                <div class="video_heading1">Voltaggio Love Vulcan
                    <div style="position:relative; float:right;"><a href="javascript:hideSlideVideo('#slider_video');"><img src="/images/close.jpg" alt="Close" border="0" /></a></div>
                </div>
                <br style="clear:both;" />
                <div class="responsive-container">
                    <iframe class="iframe_video" src="http://player.vimeo.com/video/79821473?badge=0&portrait=0&title=0&byline=0&api=1" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
                </div>
            </div>
            <br /><br />
        </div>
      <div class="swiper-container swiper-parent">
        <div class="swiper-wrapper offset_wrapper">
          <div class="swiper-slide small-swiper" style="width:455px;"><a href="/products/braising-pans/" onClick="ga('send', 'event', 'Homepage Slider', 'Braising Pans');"><img src="/images/homepage/braisingpans_slide.jpg" alt="Braising Pans" border="0" /></a> </div>
          <div class="swiper-slide small-swiper" style="width:455px;"><a href="/products/charbroilers/" onClick="ga('send', 'event', 'Homepage Slider', 'Charbroilers');"><img src="/images/homepage/charbroiler_slide.jpg" alt="Charbroilers" border="0" /></a></div>
          <div class="swiper-slide small-swiper" style="width:455px;"><a href="/products/heated-holding/" onClick="ga('send', 'event', 'Homepage Slider', 'Heated Holding');"><img src="/images/homepage/heatedholding_slide.jpg" alt="Heated Holding" border="0" /></a></div>
          <div class="swiper-slide small-swiper" style="width:455px;"><a href="/products/fryers/" onClick="ga('send', 'event', 'Homepage Slider', 'Fryers');"><img src="/images/homepage/fryers_slide.jpg" border="0" alt="Fryers" /></a> </div>
          <div class="swiper-slide small-swiper" style="width:455px;"><a href="/products/combi/" onClick="ga('send', 'event', 'Homepage Slider', 'Combi');"><img src="/images/homepage/combi_slide.jpg" alt="Combi" border="0" /></a></div>
          <div class="swiper-slide large-swiper" style="width:910px;">
          
                
                <div id="slideshowWrapper">
                    <ul id="slideshow">
                    	
                        <li><a href="/Products/IRX-Infrared-Charbroiler/" onClick="ga('send', 'event', 'Homepage Slider', 'VTEC');"><img src="/images/homepage/slide3.jpg" border="0" id="Slider2" alt="Lip-smacking Flavor. Jaw-dropping Efficiency." /></a></li>
                        <li><a href="/Products/VC-Series-Electric-Convection-Ovens/" onClick="ga('send', 'event', 'Homepage Slider', 'VC3');"><img src="/images/homepage/slide2.jpg" border="0" id="Slider2" alt="Bake the Best. Save the Most." /></a></li>
                        <li><a href="/pdf/Official_Sweepstakes_Rules.pdf" target="_blank" onClick="ga('send', 'event', 'Homepage Slider', 'Visit us at ANC');"><img src="/images/homepage/slide1.jpg" border="0" id="" alt="Visit us at ANC 2015" /></a></li>
                        <!--<li><img src="/images/homepage/150_years.jpg" border="0" id="" alt="Celebrating 150 years!" /></li>-->
                        <!--<li><a href="http://www.thenafemshow.org/" target="_blank" onClick="ga('send', 'event', 'Homepage Slider', 'NAFEM');"><img src="/images/homepage/nafem_slide.jpg" border="0" id="Slider1" alt="Visit Vulcan at NAFEM" /></a></li>-->
                        
                        <!--<li><a href="javascript:showSlideVideo('#slider_video');" ><img src="/images/homepage/sna_slide.jpg" alt="Visit SNA" id="sna" border="0" /></a></li>-->
                    </ul><br clear="all" />
                    <img src="images/drag.png" id="left_draggie" border="0" alt="left" />
                    <img src="images/drag.png" id="right_draggie" border="0" alt="right" />
                </div>
                
                
          </div>
          <div class="swiper-slide small-swiper" style="width:455px;"> <a href="/products/ranges/" onClick="ga('send', 'event', 'Homepage Slider', 'Ranges');"><img src="/images/homepage/range_slide.jpg" alt="Ranges" border="0" /></a></div>
          <div class="swiper-slide small-swiper" style="width:455px;"> <a href="/products/kettles/" onClick="ga('send', 'event', 'Homepage Slider', 'Kettles');"><img src="/images/homepage/kettles_slide.jpg" alt="Kettles" border="0" /></a></div>
          <div class="swiper-slide small-swiper" style="width:455px;"> <a href="/products/ovens/" onClick="ga('send', 'event', 'Homepage Slider', 'Ovens');"><img src="/images/homepage/ovens_slide.jpg" alt="Ovens" border="0" /></a></div>
          <div class="swiper-slide small-swiper" style="width:455px;"> <a href="/products/steamers/" onClick="ga('send', 'event', 'Homepage Slider', 'Steamers');"><img src="/images/homepage/steamers_slide.jpg" alt="Steamers" border="0" /></a></div>
          <div class="swiper-slide small-swiper" style="width:455px;"><a href="/products/griddles/" onClick="ga('send', 'event', 'Homepage Slider', 'Griddles');"><img src="/images/homepage/griddle-slide.jpg" alt="Griddles" border="0" /></a></div>   
        </div>
      </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <div class="dark_body_wrapper">
	<div class="dark_body_wrapper2">
	
	<!-- This contains the hidden content for videos -->
	<div id="video1" style='display:none; max-width:1055px; margin: 0 auto;'>
	    <br /><br />
		<div class="video_padding" id='video1_content'>
		    <div class="video_heading1">PowerFry5&trade; for Performance and Efficiency
		        <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video1');"><img src="/images/close.jpg" alt="Close" border="0" /></a></div>
		    </div>
		    <br style="clear:both;" />
		    <div class="responsive-container">
                <iframe class="iframe_video" src="http://player.vimeo.com/video/48031968?badge=0&portrait=0&title=0&byline=0&api=1" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
            </div>
		</div>
	</div>
	
	<!-- This contains the hidden content for videos -->
	<div id="video2" style='display:none; max-width:1055px; margin: 0 auto;'>
	    <br /><br />
		<div class="video_padding">
		    <div class="video_heading1">K Series Kettles Make Perfect Soup Stock
		        <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video2');"><img src="/images/close.jpg" alt="Close" border="0" /></a></div>
		    </div>
		    <br style="clear:both;" />
		    <div class="responsive-container">
                <iframe class="iframe_video" src="http://player.vimeo.com/video/48033965?badge=0&portrait=0&title=0&byline=0&api=1" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
            </div>
		</div>
	</div>
	
	<!-- This contains the hidden content for videos -->
	<div id="video3" style='display:none; max-width:1055px; margin: 0 auto;'>
	    <br /><br />
		<div class="video_padding">
		    <div class="video_heading1">VCCG: Great Food and Profitability
		        <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video3');"><img src="/images/close.jpg" alt="Close" border="0" /></a></div>
		    </div>
		    <br style="clear:both;" />
		    <div class="responsive-container">
                <iframe class="iframe_video" src="http://player.vimeo.com/video/118914666?badge=0&portrait=0&title=0&byline=0&api=1" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
            </div>
		</div>
	</div>
	
	<!-- This contains the hidden content for videos -->
	<!--<div id="video4" style='display:none; max-width:1055px; margin: 0 auto;'>
	    <br /><br />
		<div class="video_padding">
		    <div class="video_heading1">VC Series Convection Ovens: The Performance You Need
		        <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video4');"><img src="/images/close.jpg" alt="Close" border="0" /></a></div>
		    </div>
		    <br style="clear:both;" />
		    <div class="responsive-container">
                <iframe class="iframe_video" src="http://player.vimeo.com/video/47962761?badge=0&portrait=0&title=0&byline=0&api=1" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
            </div>
		</div>
	</div>-->
    <div id="video4" style='display:none; max-width:1055px; margin: 0 auto;'>
	    <br /><br />
		<div class="video_padding">
		    <div class="video_heading1">Vulcan Combi
		        <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video4');"><img src="/images/close.jpg" alt="Close" border="0" /></a></div>
		    </div>
		    <br style="clear:both;" />
		    <div class="responsive-container">
                <iframe class="iframe_video" src="http://player.vimeo.com/video/102043945?badge=0&portrait=0&title=0&byline=0&api=1" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
            </div>
		</div>
	</div>
		
	<div class="row_padding">
	<div class="row" id="homepage_center">
    	<div class="sixty">
        	<table cellpadding="0" cellspacing="0" width="100%">
            	<tr>
                	<td colspan="3" id="prod_videos">PRODUCT VIDEOS</td>
                </tr>
                <tr>
                	<td colspan="3" height="30">&nbsp;</td>
                </tr>
                <tr>
                	<td width="42%"><a class='video' href="javascript:showVideo('#video1');" onClick="ga('send', 'event', 'Homepage Video', 'PowerFry5 Performance and Efficiency');"><img src="/images/homepage/video1_thumb.png" alt="PowerFry5™ Performance and Efficiency" style="width:100%; max-width:266px; max-height:158px;" border="0" /></a></td>
                    <td width="6%">&nbsp;</td>
                    <td width="42%"><a class='video' href="javascript:showVideo('#video2');" onClick="ga('send', 'event', 'Homepage Video', 'K Series Kettles Make Perfect Soup Stock');"><img src="/images/homepage/video2_thumb.png" alt="K Series Kettles Make Perfect Soup Stock" style="width:100%; max-width:266px; max-height:158px;" border="0" /></a></td>
                </tr>
                <tr>
                	<td valign="top" class="video_heading1">PowerFry5&trade; for Performance and Efficiency</td>
                    <td>&nbsp;</td>
                    <td  valign="top" class="video_heading1">K Series Kettles Make Perfect Soup Stock</td>
                </tr>
                <tr>
                	<td  valign="top" class="video_heading2">Watch how the ENERGY STAR® qualified PowerFry5™ fryer redefines the traditional restaurant fryer with maximum efficiency and shorter cooking times.</td>
                    <td class="video_heading2">&nbsp;</td>
                    <td  valign="top" class="video_heading2">Our fast-cooking kettles feature an ellipsoidal design kettle bottom for superior heat transfer to make the perfect soup stock, ideal for any foodservice operation.</td>
                </tr>
                <tr>
                	<td colspan="3" height="30">&nbsp;</td>
                </tr>
                <tr>
                	<td><a class='video' href="javascript:showVideo('#video3');" onClick="ga('send', 'event', 'Homepage Video', 'VCCG Video');"><img src="/images/homepage/video3_thumb.png" alt="Flip Burgers, Fill Orders and Make Money Faster with the VCCG Griddle" style="width:100%; max-width:266px; max-height:158px;" border="0" /></a></td>
                    <td>&nbsp;</td>
                    <td><a class='video' href="javascript:showVideo('#video4');" onClick="ga('send', 'event', 'Homepage Video', 'Vulcan Combi');"><img src="/images/homepage/combi-video.png" alt="Vulcan Combi" style="width:100%; max-width:266px; max-height:158px;" border="0" /></a></td>
                </tr>
                <tr>
                	<td  valign="top" class="video_heading1">VCCG: Great Food and Profitability</td>
                    <td>&nbsp;</td>
                    <td  valign="top" class="video_heading1">Vulcan Combi</td>
                    <!--<td  valign="top" class="video_heading1">VC Series Convection Ovens: The Performance You Need</td>-->
                </tr>
                <tr>
                	<td  valign="top" class="video_heading2">Energy efficient with quick recovery times and even temperatures, the Vulcan Custom Chain Griddle (VCCG) is designed to make commercial kitchens more profitable.</td>
                    <td class="video_heading2">&nbsp;</td>
                    <td  valign="top" class="video_heading2">See how Vulcan's Combi Oven provides the ultimate in professional kitchen equipment convenience with its three easy-to-use knobs. The humidity level automatically adjusts after you set the temperature and time, ensuring simple operation and consistent results.</td>
                    <!--<td  valign="top" class="video_heading2">VC Series provides versatile performance for preparing a varied menu with consistently great results while saving on operating costs.</td>-->
                </tr>
                <tr>
                	<td valign="middle" colspan="3" style="padding-top:40px;">
                    	<table cellpadding="0" cellspacing="0">
                		<tr>
                        	<td><img src="/images/homepage/more_video_icon.jpg" alt="More Videos" /></td>
                            <td style="font-size:14px; color:#ffffff; font-family:'futura medium', Arial, sans-serif; line-height:25px; vertical-align:middle;">&nbsp;&nbsp;<a href="/video-library/" style="text-decoration:none;" onClick="ga('send', 'event', 'Homepage Video', 'view other product videos');">view other product videos</a></td>
                        </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        
		<div class="forty">
        	<table cellpadding="0" cellspacing="0">
                <tr>
                	<td id="home_gutter">&nbsp;</td>
                	<td>
                	    <div id="homepage_column_padding"></div>
                	    <div class="separator"></div>
                	    <div class="feature_break"><div style="margin-right:8px;"><img src="/images/homepage/k12_icon.png" alt="K12" style="padding-bottom:8px; width:100%; max-width:115px; max-height:70px; height:70px; height:auto;" /></div></div>
                	    <div class="feature_break2">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="color:#e4823a; font-size:27px; font-family:'futura bold condensed', Arial, sans-serif;">K-12 SOLUTIONS</td>
                                </tr>
                                <tr>
                                    <td style="color:#f2f2f2; font-size:36px; font-family:'futura medium condensed', Arial, sans-serif;">Just 4 Schools</td>
                                </tr>
                                <tr>
                                    <td style="color:#c3c2c2; font-size:16px; font-family:'helvetica', Arial, sans-serif;">A really smart move for your school, because better nutrition starts in the kitchen with smart equipment to fuel healthy minds.</td>
                                </tr>
                                <tr>
                                    <td style="padding-top:10px; padding-bottom:20px; border-bottom:1px solid #626160;"><a style="color:#ffffff; font-size:15px; font-family:'futura light', Arial, sans-serif; text-decoration:none;" href="/k-12/" onClick="ga('send', 'event', 'Homepage Callout', 'K12 Solutions');">LEARN MORE <img src="/images/homepage/orange_arrow.png" alt="More" border="0" /></a></td>
                                </tr>
                            </table>
                        </div>
                        <div class="separator"></div>
                        <div class="feature_break"><div style="margin-right:8px;"><img src="/images/homepage/v_heart.png" alt="Vulcan Sustainability" style="padding-top:10px; width:100%; max-width:95px; max-height:90px; height:90px; height:auto;"/><img src="/images/homepage/energy_star_small.jpg" alt="Energy Star" style="padding-top:10px; padding-left:15px; width:100%; max-width:116px; max-height:90px; height:61px; height:auto;"/></div></div>
                	    <div class="feature_break2">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="color:#5FD16D; font-size:36px; font-family:'futura medium condensed', Arial, sans-serif;">Energy Saving Products</td>
                                </tr>
                                <tr>
                                    <td style="color:#c3c2c2; font-size:16px; font-family:'helvetica', Arial, sans-serif;">ITW has been recognized by ENERGY STAR® as Partner of the Year for two years and Partner of the Year – Sustained Excellence for six years.</td>
                                </tr>
                                <tr>
                                    <td style="padding-top:10px; padding-bottom:20px; border-bottom:1px solid #626160;"><a href="/Sustainability/" style="color:#ffffff; font-size:15px; font-family:'futura light', Arial, sans-serif; text-decoration:none;" onClick="ga('send', 'event', 'Homepage Callout', 'Sustainability');">LEARN MORE <img  src="/images/homepage/green_arrow.png" alt="More" border="0" /></a></td>
                                </tr>
                            </table>
                        </div>
                        <div class="separator"></div>
                        <div class="feature_break"><div style="margin-right:8px;"><img src="/images/homepage/BIC2014.png" alt="Best in Class" style="padding-top:10px; width:100%; max-width:95px; max-height:95px; height:95px; height:auto;" /></div></div>
                	    <div class="feature_break2">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="color:#71c1c6; font-size:36px; font-family:'futura medium condensed', Arial, sans-serif;">Vulcan Honored with 10 Best In Class Awards</td>
                                </tr>
                                <tr>
                                    <td style="color:#c3c2c2; font-size:16px; font-family:'helvetica', Arial, sans-serif;">Each award highlights our mission to produce restaurant equipment with the foodservice industry's needs in mind.</td>
                                </tr>
                                <tr>
                                    <td style="padding-top:10px; padding-bottom:20px;"><a href="/News-and-Events/Press-Releases/Vulcans-Best-in-Class-Streak-Continues-with-Ten-Awards-from-Foodservice-Equipment-Supplies-Magazine/" style="color:#ffffff; font-size:15px; font-family:'futura light', Arial, sans-serif; text-decoration:none;"onClick="ga('send', 'event', 'Homepage Callout', 'Best in Class');">LEARN MORE <img src="/images/homepage/blue_arrow.png" alt="More" border="0" /></a></td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        </div>
        
        <br style="clear:both;" />
        <br /><br />
        </div>
        </div>
	</div>
    <div style="margin:0 auto; text-align:center; width:100%; position:relative; z-index:2000; border-top: 2px solid #404040;"></div>
	<div style="background-image:url(/images/download-bg.jpg); background-repeat:repeat-x; background-position:bottom left; background-color:#2F2F2F;">
        <div class="row">
        	<div class="row_padding"  style="padding-bottom:8px;">
            	<asp:Downloads runat="server" id="DL" />
            </div>
        </div>
    </div>
	</asp:Content>
    <asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
	
	<script type="application/x-javascript" src="/swiper/swiper.js"></script>
    <script type="text/javascript" src="/js/fader.js" defer></script>
  
	<script type="text/javascript">
	    jQuery(document).ready(function() {
	        jQuery('#slideshow').fadeSlideShow();
	    });

      var mySwiper = new Swiper('.swiper-parent', {
          freeMode: true,
          freeModeFluid: true,
          keyboardControl: true,
          slidesPerView: 'auto',
          useCSS3Transforms: false,
          calculateHeight: true,
          createPagination: true
      })

      /* var swiperNested1 = new Swiper('.swiper-nested-1', {
      mode: 'vertical',
      pagination: '.pagination-nested-1',
      paginationClickable: true,
      calculateHeight: true,
      autoplay: 5000
      }) */

    jQuery.fn.center = function() {
        this.css("left", ($(window).width() / 2) - (this.outerWidth() / 2)); //-- use this for even number of slides
        //this.css("left", ($(window).width() / 2) - ((this.outerWidth() + 455) / 2));  //-- use this for odd number of slides
        return this;
    }

    $('.offset_wrapper').center();

    $(document).ready(function() {
        if ($(window).width() < 750) {
            $('#Slider2').attr("src", "/images/homepage/voltaggio_slide_mobile.jpg");
            $('#Slider1').attr("src", "/images/homepage/main_slide_mobile.jpg");
        }
    }); 

    $('.offset_wrapper').resize(function() {
        if ($(window).width() < 1175) {
            mySwiper.resizeFix();
        }
    });

    function showVideo(video) {
        $(video).fadeIn(800);
        $("#homepage_center").hide();
    }

    function hideVideo(video) {
        $(video).hide();
        $("#homepage_center").fadeIn(800);

        var data = { method: 'pause' };
        // This pauses all frames within tabs container.
        $("iframe").each(function() {
            var url = $(this).attr('src').split('?')[0];
            this.contentWindow.postMessage(JSON.stringify(data), url);
        });
    }

    function showSlideVideo(video) {
        var data = { method: 'pause' };
        // This pauses all frames within tabs container.
        $("iframe").each(function() {
            var url = $(this).attr('src').split('?')[0];
            this.contentWindow.postMessage(JSON.stringify(data), url);
        });

        $(video).fadeIn(800);
        $(".swiper-container").hide();
    }

    function hideSlideVideo(video) {
        $(video).hide();
        $(".swiper-container").fadeIn(800);

        var data = { method: 'pause' };
        // This pauses all frames within tabs container.
        $("iframe").each(function() {
            var url = $(this).attr('src').split('?')[0];
            this.contentWindow.postMessage(JSON.stringify(data), url);
        });
    }
</script>	

<script type = "text/javascript">
    window.onload = function () {
        var scrollY = parseInt('<%=Request.Form["scrollY"] %>');             
        if (!isNaN(scrollY)) {
            window.scrollTo(0, scrollY);
        }
    };
    window.onscroll = function () {
        var scrollY = document.body.scrollTop;
        if (scrollY == 0) {
            if (window.pageYOffset) {
                scrollY = window.pageYOffset;
            }
            else {
                scrollY = (document.body.parentElement) ? document.body.parentElement.scrollTop : 0;
            }
        }
        if (scrollY > 0) {
            var input = document.getElementById("scrollY");
            if (input == null) {
                input = document.createElement("input");
                input.setAttribute("type", "hidden");
                input.setAttribute("id", "scrollY");
                input.setAttribute("name", "scrollY");
                document.forms[0].appendChild(input);
            }
            input.value = scrollY;
        }
    };
</script>
</asp:Content>
