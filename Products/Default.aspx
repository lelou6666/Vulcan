<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<%@ Register assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.WebControls" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/products/" />
	<link rel="stylesheet" href="/Swiper/swiper.css" type="text/css" />
	<style type="text/css">
		#featured_product_slider {
			background:url(/images/products/featured/slider/gradient_bg.png) top center no-repeat;
			background-size:auto 100%;
			max-height:1505px;
		}
		#featured_product_slider .swiper-wrapper {
			background:url(/images/products/featured/slider/gradient_bar_bg.png) top left repeat-x;
		}
		#featured_product_slider .divot {
			background:url(/images/products/featured/slider/divot.png) top left no-repeat;
			position:absolute;
			top:0;
			left:48%;
			width:84px;
			height:41px;
			margin:0 auto;
			z-index:3;
		}
		#featured_product_slider .upper_links_container {
			position:absolute;
			top:0;
			left:0;
			width:100%;
		}
		#featured_product_slider .upper_links {
			width:100%;
			max-width:955px;
			margin:0 auto;
			padding-top:30px;
			font-family:"futura medium";
			font-size:16px;
			color:#FFFFFF;
			text-align:right;
		}
		#featured_product_slider .upper_links a {
			color:#FFFFFF;
			padding-left:25px;
		}
		#featured_product_slider .previous, #featured_product_slider .next {
			background:url(/images/products/featured/slider/previous.png) top left no-repeat;
			position:absolute;
			top:211px;
			left:5%;
			width:82px;
			height:82px;
			z-index:10;
			cursor:pointer;
		}
		#featured_product_slider .next {
			background-image:url(/images/products/featured/slider/next.png);
			left:inherit;
			right:5%;
		}
		.featured_product {
			width:100%;
			max-width:955px;
			height:505px;
			margin:0 auto;
		}
		.featured_product div.left {
			float:left;
			width:50%;
		}
		.featured_product div.left img {
			max-width:465px;
			width:100%;
			max-height:528px;
		}
		.featured_product div.right {
			float:right;
			width:50%;
			padding-top:120px;
			font-family:"helvetica";
			font-size:18px;
			color:#FFFFFF;
			text-align:right;
		}
		.clearer {
			clear:both;
		}
		.featured_product div.right h1 {
			padding-bottom:25px;
			font-family:"futura medium condensed";
			font-size:55px;
			color:#FFFFFF;
			line-height:95%;
		}
		.featured_product div.right h2 {
			font-family:"futura bold condensed";
			font-size:28px;
			/*color:#fba05e;*/
			color:#FFFFFF;
		}
		.featured_product div.right h3 {
			background:url(/images/products/featured/slider/cta_arrow.png) center right no-repeat;
			margin-top:22px;
			padding-right:18px;
			font-family:"futura light";
			font-size:18px;
			text-transform:uppercase;
		}
		.featured_product div.right a {
			color:#FFFFFF;
		}
		
		
	@media all and (max-width:1075px) 
	{
	    .swiper-container{max-height:273px;}
		.small-swiper[style]
	    {
	        width:300px !important;
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
	    
	    .large-swiper[style]
	    {
	        width:400px !important;
	    }
	    
	    .large-swiper img
	    {
	        width:400px;
	    }
	    
	    .featured_product div.right h1 {
			font-size:45px;
		}
		
		.featured_product div.right h2 {
			font-size:22px;
		}
		
		.featured_product div.right p {
			font-size:14px;
		}
		
		.featured_product div.right h3 {
			font-size:16px;
		}
		
		.featured_product div.left
		{
		    margin-top:40px;
	    }
	    
	    #featured_product_slider .divot 
	    {
	        display:none;
	    }
	}
	
	@media all and (max-width:580px) 
	{
	    .featured_product div.left
		{
		    margin-top:80px;
	    }
	    
	    .featured_product div.right h1 {
			font-size:35px;
		}
		
		.featured_product div.right h2 {
			font-size:20px;
		}
		
		.featured_product div.right p {
			font-size:12px;
		}
		
		.featured_product div.right h3 {
			font-size:14px;
			margin-top:0px;
		}
		
		#featured_product_slider .previous
		{
		    background: url(/images/products/featured/slider/previous_mobile.png) top left no-repeat;
		    width:40px;
		    height:40px;
		}
		
		#featured_product_slider .next
		{
		    background: url(/images/products/featured/slider/next_mobile.png) top left no-repeat;
		    width:40px;
		    height:40px;
		}
		
		
	}

	@media all and (max-width:450px) 
	{
	    .featured_product div.right
	    {
	        padding-top: 80px;
	    }
	    
	    .featured_product div.left
		{
		    margin-top:60px;
	    }
	    
	    .featured_product div.right h1 {
			font-size:25px;
		}
		
		#featured_product_slider .upper_links a 
		{
		    padding-left:10px;
		}
		
		
	    
	
	    /*
	    .featured_product div.right h2 {
			display:none;
		}
		
		.featured_product div.right p {
			display:none;
		} */
	}
	
	.downloads, .tools
    {
        background-image:url('/images/products/transparent_feature.png');
        background-repeat:repeat;
        min-height:250px;
        display:none;
        text-align:left;
        padding:10px;
    }
	
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
 
<div id="product_heading">
     <div class="row" style="overflow:auto; padding-bottom:30px;">
        <div class="row_padding">
             <br />
             <h1>Commercial &amp; Industrial Kitchen Appliances</h1>
             
             <div class="fivecolumn">
                  <div class="fade">
                        <a href="/products/fryers/"><img src="/images/products/fryer.png" alt="Fryers" style="width:100%; max-width:186px;" border="0" /></a>
                        <div>
                            <a href="/products/fryers/"><img src="/images/products/fryer_on.png" alt="Fryers" style="width:100%; max-width:186px; margin:0 auto;" border="0" /></a>
                        </div>
                 </div>
             </div>
             <div class="fivecolumn">
                    <div class="fade">
                        <a href="/products/ranges/"><img src="/images/products/ranges.png" alt="Ranges" style="width:100%; max-width:186px;" border="0" /></a>
                        <div>
                            <a href="/products/ranges/"><img src="/images/products/ranges_on.png" alt="Ranges" style="width:100%; max-width:186px;" border="0" /></a>
                        </div>
                 </div>
             </div>
             <div class="fivecolumn">
                <div class="fade">
                    <a href="/products/ovens/"><img src="/images/products/ovens.png" alt="Ovens" style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    <div>
                        <a href="/products/ovens/"><img src="/images/products/ovens_on.png" alt="Ovens" style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    </div>
                </div>
             </div>
             <div class="fivecolumn">
                <div class="fade">
                    <a href="/products/griddles/"><img src="/images/products/griddles.png" alt="Griddles" style="width:100%; max-height:97px; max-width:186px;" border="0" /></a>
                    <div>
                        <a href="/products/griddles/"><img src="/images/products/griddles_on.png" alt="Griddles" style="width:100%; max-height:97px; max-width:186px;" border="0" /></a>
                    </div>
                </div>
             </div>
             <div class="fivecolumn">
                <div class="fade">
                    <a href="/products/charbroilers/"><img src="/images/products/charbroilers.png" alt="Charbroilers" style="width:100%; max-height:97px; max-width:186px;" border="0" /></a>
                    <div>
                        <a href="/products/charbroilers/"><img src="/images/products/charbroilers_on.png" alt="Charbroilers" style="width:100%; max-height:97px; max-width:186px;" border="0" /></a>
                    </div>
                </div>
             </div>
             <div class="fivecolumn">
                <div class="fade">
                    <a href="/products/steamers/"><img src="/images/products/steamers.png" alt="Steamers"  style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    <div>
                        <a href="/products/steamers/"><img src="/images/products/steamers_on.png" alt="Steamers" style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    </div>
                </div>
             </div>
             <div class="fivecolumn">
                <div class="fade">
                    <a href="/products/kettles/"><img src="/images/products/kettles.png" alt="Kettles"  style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    <div>
                        <a href="/products/kettles/"><img src="/images/products/kettles_on.png" alt="Kettles"  style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    </div>
                </div>
             </div>
             <div class="fivecolumn">
                <div class="fade">
                    <a href="/products/braising-pans/"><img src="/images/products/braising.png" alt="Braising Pans"  style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    <div>
                        <a href="/products/braising-pans/"><img src="/images/products/braising_on.png" alt="Braising Pans"  style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    </div>
                </div>
             </div>
             <div class="fivecolumn">
                <div class="fade">
                    <a href="/products/heated-holding/"><img src="/images/products/heated.png" alt="Heated Holding" style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    <div>
                        <a href="/products/heated-holding/"><img src="/images/products/heated_on.png" alt="Heated Holding" style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    </div>
                </div>
             </div>
             <div class="fivecolumn">
                <div class="fade">
                    <a href="/products/combi/"><img src="/images/products/combi_btn.png" alt="Combi" style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    <div>
                        <a href="/products/combi/"><img src="/images/products/combi_btn_on.png" alt="Combi" style="width:100%; max-width:186px; max-height:97px;" border="0" /></a>
                    </div>
                </div>
             </div>
        </div>
        <div style="width:100%; text-align:center;">
        	<a href="/products/all-products/"><img src="/images/products/all_products.png" alt="All Products" style="width:100%; max-width:305px; max-height:50px; padding-top:10px;" border="0" /></a>
         </div>
    </div>
</div>
<div class="swiper-container" id="featured_product_slider">
    <div class="swiper-wrapper"></div>
    <div class="divot"></div>
    <div class="upper_links_container">
	    <div class="upper_links">
	        <div class="row_padding">
		        <a id="ul_video" href="javascript:showVideo('#video1');" onClick="ga('send', 'event', 'Product Slider', 'Video');">VIDEO</a>
		        <a id="ul_downloads" href="javascript:showDownloads();" onClick="ga('send', 'event', 'Product Slider', 'Downloads');">DOWNLOADS</a>
		        <a id="ul_tools" href="javascript:showTools();" onClick="ga('send', 'event', 'Product Slider', 'Tools');">TOOLS</a>
		    </div>
	    </div>
    </div>
    <div class="previous"></div>
    <div class="next"></div>
    
    <!-- This contains the hidden content for videos -->
	<div id="video1" style='display:none; max-width:1055px; margin: 0 auto;'>
	    <br /><br />
		<div class="video_padding" id='video1_content'>
		    <div class="video_heading1"><div class="video_title" style="position:relative; float:left;"></div>
		        <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video1');"><img src="/images/close.jpg" border="0" /></a></div>
		    </div>
		    <br style="clear:both;" />
		    <div class="responsive-container">
                <iframe class="iframe_video" src="http://player.vimeo.com/video/48031968?badge=0&portrait=0&title=0&byline=0&api=1" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
            </div>
		</div>
		<br /><br />
	</div>
</div>

<br style="clear:both;" />
<div style="background-image: url('/images/news_events_body_bg2.jpg'); background-repeat:repeat; overflow:auto; background-position:top;">
<div style="background-image: url('/images/news_events_body_bg.jpg'); background-repeat:repeat-x; background-position:top; overflow:auto;">
    <div class="row testimonial_features" style="overflow:auto; padding-bottom:40px;">
        <div class="row_padding">
            <div style="font-family:'futura light condensed', Arial, Sans-Serif; color:#b0b0b0; font-size:36px; border-bottom:1px solid #b0b0b0;">Testimonials</div>
            <br />
            <CMS:Collection runat="server" ID="Collection1" DefaultCollectionID="8" DoInitFill="true" GetHtml="true" DisplayXslt="/xsl/feat_testimonials.xsl"  />
        </div>
    </div>           
</div>
</div>
</asp:Content>
<asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
    <script src="/Swiper/swiper.js" type="text/javascript" defer></script>
	<script src="/js/jquery.color.js" type="text/javascript" defer></script>
	<script type="text/javascript" defer>
	    //Featured Product Slider
	    $(document).ready(function(e) {
	        var featuredProducts = [/*{
			backgroundColor:"#3e8936",
				image:"/images/products/featured/fryer.png",
				title:"Steamers, Kettles &amp; Braising Pans",
				subtitle:"lorem ipsum",
				copy:"Lorem ipsum dolor sit amet. Con minimim venami quis nostr ud laboris nisi ut aliquip ex ea venami quis com dolor in.",
				url:"",
				cta:"LOREM IPSUM DOLOR",
				video:"#",
				downloads:"#",
				tools:"#"
			},*/{
	backgroundColor: "#d19f28",
    		image: "/images/products/featured/combi.png",
    		title: "ABC Combi",
    		subtitle: "brilliantly simple, simply brilliant.",
    		copy: "Just set the temperature, set the time and set your mind at ease.",
    		url: "/products/combi/",
    		cta: "LEARN MORE",
    		video: "",
    		videoTitle: "",
    		downloads: "",
    		tools: ""

}, {
	backgroundColor: "#911013",
			image: "/images/products/featured/fryer.png",
			title: "PowerFry5™",
			subtitle: "more fries per hour",
			copy: "Deliver more fried food in less time. Plus dramatic energy savings. Take it for a test fry and see for yourself.",
			url: "/products/fryers/?series=337",
			cta: "LEARN MORE",
			video: "http://player.vimeo.com/video/48031968?badge=0&portrait=0&title=0&byline=0&api=1",
			videoTitle: "Vulcan PowerFry5™ VK Series Fryer",
			downloads: "",
			tools: "<p><a href='/Vulcan/Products/VK45_Savings_Calculator/' class='alt floatbox' data-fb-options='width:850 height:570 outsideClickCloses:true splitResize:false showNewWindow:false showPrint:false enableDragResize:true disableScroll:true'>VK Calculator</a></p>"
}, /*{
				backgroundColor:"#0d7a66",
				image:"/images/products/featured/fryer.png",
				title:"Heated Holding",
				subtitle:"lorem ipsum",
				copy:"Lorem ipsum dolor sit amet. Con minimim venami quis nostr ud laboris nisi ut aliquip ex ea venami quis com dolor in.",
				url:"",
				cta:"LOREM IPSUM DOLOR",
				video:false,
				downloads:"#",
				tools:false
			},{
				backgroundColor:"#741274",
				image:"/images/products/featured/fryer.png",
				title:"Ovens",
				subtitle:"lorem ipsum",
				copy:"Lorem ipsum dolor sit amet. Con minimim venami quis nostr ud laboris nisi ut aliquip ex ea venami quis com dolor in.",
				url:"",
				cta:"LOREM IPSUM DOLOR",
				video:"#",
				downloads:false,
				tools:"#"
			},*/{
			backgroundColor: "#1f5796",
			image: "/images/products/featured/voltaggio.png",
			title: "Chef Voltaggio on Vulcan",
			subtitle: "Vulcan spent a lot of time seeing from a chef's perspective on how to build a really great stove.",
			copy: "–Chef Bryan Voltaggio, Chef/Owner of Volt, Family Meal, and Lunchbox of Frederick, Maryland and Range of Washington, D.C.",
			url: "javascript:showVideo('#video1');",
			cta: "WATCH HIS TESTIMONIAL",
			video: "http://player.vimeo.com/video/79821473?badge=0&portrait=0&title=0&byline=0&api=1",
			videoTitle: "Voltaggio Loves Vulcan",
			downloads: "<a href='/uploadedFiles/Voltaggio_Testimonial.pdf' target='_blank'>Chef Voltaggio on Vulcan</a>",
			tools: ""
}];

	        //Build slides
	        for (i = 0; i < featuredProducts.length; i++) {
	            $("#featured_product_slider .swiper-wrapper").append("<div class=\"swiper-slide\"><div class=\"featured_product\"><div class=\"left\"><div style=\"padding-left:15px; padding-right:15px;\"><a href=\"" + featuredProducts[i].url + "\"><img src=\"" + featuredProducts[i].image + "\" border=\"0\" /></a></div></div><div class=\"right\"><div style=\"padding-right:15px;\"><div class='feature_copy'><h1>" + featuredProducts[i].title + "</h1><h2>" + featuredProducts[i].subtitle + "</h2><p>" + featuredProducts[i].copy + "</p><h3><a href=\"" + featuredProducts[i].url + "\">" + featuredProducts[i].cta + "</a></h3></div><div class='tools'>" + featuredProducts[i].tools + "</div><div class='downloads'>" + featuredProducts[i].downloads + "</div></div></div><div class=\"clearer\"></div></div></div>");
	        }

	        var activeSlide;
	        var swiper = new Swiper(".swiper-container", {
	            speed: 500,
	            autoplay: 8000,
	            calculateHeight: true,
	            createPagination: false,
	            grabCursor: true,
	            onFirstInit: function(swiper) {
	                activeSlide = swiper.activeIndex;

	                //Change color
	                var color = featuredProducts[activeSlide].backgroundColor;
	                $("#featured_product_slider").css("background-color", color);

	                //Modify controls
	                $("#featured_product_slider .previous").fadeTo(500, 0.3);
	                if (featuredProducts[activeSlide].video) {
	                    $(".iframe_video").attr("src", featuredProducts[activeSlide].video);
	                    $(".video_title").html(featuredProducts[activeSlide].videoTitle);
	                } else {
	                    $("#ul_video").hide();
	                }
	                if (featuredProducts[activeSlide].downloads != "") {
	                    $("#ul_downloads").show();
	                    $(".downloads").html("<div class='video_heading1'><div style='position:relative; float:left;'>Downloads</div><div style='position:relative; float:right;'><a href='javascript:hideDownloads();'><img src='/images/close.png' border='0' /></a></div></div><br /><br />" + featuredProducts[activeSlide].downloads);
	                } else {
	                    $("#ul_downloads").hide();
	                }
	                if (featuredProducts[activeSlide].tools != "") {
	                    $("#ul_tools").show();
	                    $(".tools").html("<div class='video_heading1'><div style='position:relative; float:left;'>Tools</div><div style='position:relative; float:right;'><a href='javascript:hideTools();'><img src='/images/close.png' border='0' /></a></div></div><br /><br />" + featuredProducts[activeSlide].tools);
	                } else {
	                    $("#ul_tools").hide();
	                }
	            },
	            onSlideChangeStart: function(swiper) {
	                if (stopIt == false) {
	                    hideTools();
	                    hideDownloads();
	                    activeSlide = swiper.activeIndex;

	                    //Animate color
	                    var color = featuredProducts[activeSlide].backgroundColor;
	                    $("#featured_product_slider").animate({
	                        backgroundColor: color
	                    }, 500);

	                    //Modify controls
	                    if (activeSlide == 0) {
	                        $("#featured_product_slider .previous").fadeTo(500, 0.3);
	                        $("#featured_product_slider .next").fadeTo(500, 1);
	                    } else if (activeSlide == (featuredProducts.length - 1)) {
	                        $("#featured_product_slider .previous").fadeTo(500, 1);
	                        $("#featured_product_slider .next").fadeTo(500, 0.3);
	                    } else {
	                        $("#featured_product_slider .previous, #featured_product_slider .next").fadeTo(500, 1);
	                    }
	                    if (featuredProducts[activeSlide].video) {
	                        $(".iframe_video").attr("src", featuredProducts[activeSlide].video);
	                        $(".video_title").html(featuredProducts[activeSlide].videoTitle);
	                        $("#ul_video").show();
	                    } else {
	                        $("#ul_video").hide();
	                    }
	                    if (featuredProducts[activeSlide].downloads != "") {
	                        $("#ul_downloads").show();
	                        $(".downloads").html("<div class='video_heading1'><div style='position:relative; float:left;'>Downloads</div><div style='position:relative; float:right;'><a href='javascript:hideDownloads();'><img src='/images/close.png' border='0' /></a></div></div><br /><br />" + featuredProducts[activeSlide].downloads);
	                    } else {
	                        $("#ul_downloads").hide();
	                    }
	                    if (featuredProducts[activeSlide].tools != "") {
	                        $("#ul_tools").show();
	                        $(".tools").html("<div class='video_heading1'><div style='position:relative; float:left;'>Tools</div><div style='position:relative; float:right;'><a href='javascript:hideTools();'><img src='/images/close.png' border='0' /></a></div></div><br /><br />" + featuredProducts[activeSlide].tools);
	                    } else {
	                        $("#ul_tools").hide();
	                    }
	                }
	            }
	        });

	        //Pause on hover
	        $(".featured_product").hover(function(e) {
	            swiper.stopAutoplay();
	        }, function(e) {
	            swiper.startAutoplay();
	        });

	        //Bind next/previous buttons
	        $("#featured_product_slider .next").bind("click", function(e) {
	            hideTools();
	            hideDownloads();
	            swiper.swipeNext();
	        });
	        $("#featured_product_slider .previous").bind("click", function(e) {
	            hideTools();
	            hideDownloads();
	            swiper.swipePrev();
	        });

	    });

	    var stopIt = false;

	    function showVideo(video) {
	        $(video).fadeIn(800);
	        $(".swiper-wrapper").hide();
	        $(".next").hide();
	        $(".previous").hide();
	        $(".upper_links").hide();
	        stopIt = true;
	    }

	    function hideVideo(video) {
	        $(video).hide();
	        stopIt = false;
	        $(".swiper-wrapper").show();
	        $(".next").fadeIn(800);
	        $(".previous").fadeIn(800);
	        $(".upper_links").fadeIn(800);
	        swiper.swipeNext();

	        var data = { method: 'pause' };
	        // This pauses all videos.
	        $("iframe").each(function() {
	            var url = $(this).attr('src').split('?')[0];
	            this.contentWindow.postMessage(JSON.stringify(data), url);
	        });
	    }

	    function showTools() {
	        $(".downloads").hide();
	        $(".tools").fadeIn(800);
	        $(".feature_copy").hide();
	        /* $(".next").hide();
	        $(".previous").hide(); */
	    }

	    function hideTools() {
	        $(".tools").hide();
	        $(".feature_copy").fadeIn(800);
	    }

	    function showDownloads() {
	        $(".tools").hide();
	        $(".downloads").fadeIn(800);
	        $(".feature_copy").hide();
	        /* $(".next").hide();
	        $(".previous").hide(); */
	    }

	    function hideDownloads() {
	        $(".downloads").hide();
	        $(".feature_copy").fadeIn(800);
	    }
	</script>
</asp:Content>