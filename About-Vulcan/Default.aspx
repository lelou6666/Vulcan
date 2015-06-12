<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/about-vulcan/" />
    <link href="/swiper/tabs/swiper.css" rel="stylesheet" />
    <!-- Keep CSS on page for Swiper Custom CSS -->
    <style type="text/css">
         .swiper-wrapper[style], .swiper-slide[style]
         {
             height:auto !important;
         }
         
         .swiper-container
         {
             top:-40px;
         }

        .tabs
        {
            margin:0 auto; 
            width:355px;
            text-align:center;
            position:relative;
            top:-61px;
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
            left:90px;
        }
        
        .tab_arrow2
        {
            position:absolute; 
            top:39px; 
            left:230px;
        }
        
        #news_events_bk
        {
            min-height:275px; 
            background-image: url('/images/news_events_header_bg.jpg'); 
            background-repeat:repeat-x; 
            background-position:bottom; 
            background-color:#2c2c2c;
        }
        
        .press_releases_padding 
        {
            background-color:#222222; 
            padding:30px;
        }
        
        @media all and (max-width:390px) 
	    {
		    .tabs 
		    {
		        width:290px;

		    }
		    
		    .tab_arrow1
            {

                left:60px;
            }
            
            .tab_arrow2
            {

                left:200px;
            }
	    }
	    
	    @media print
	    {
	        *
	        {
	            background-image:none;
	            background-color:#FFFFFF;
	            color:#000000;
	        }
	        body
	        {
	             background-color:#FFFFFF;
	        }
	        
	        .press_releases_padding, .body_wrapper, .body_wrapper2
	        {
	            background-image:none;
	            background-color:#FFFFFF;
	        }
	        
	        .events_title, p, li
	        {
	            color:#000000;
	        }
	        
	        #header_background , #about_vulcan_header, #about_vulcan_middle, #scrollToHere, #footer_squares, #disclaimer, .tabs, #icons
	        {
	            display:none;
	        }
	        
	        .body-top-padding
	        {
	            top:0px;
	        }
	    }  
             
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div id="about_vulcan_header">
     <div class="row">
        <div class="row_padding" style="padding-bottom:30px;">
           <br />
		   <CMS:ContentBlock ID="ContentBlock1" runat="server" DefaultContentID="1226" /> 
        </div>
    </div>
</div>

<div id="about_vulcan_middle">
    <div style="margin:0 auto; text-align:center; width:100%; position:relative; z-index:2000; border-top: 2px solid #353535;"><img src="/images/black_arrow.png" style="position:relative; top:-4px; margin:0 auto; text-align:center;" /></div>
     <div class="row">
        <div class="twocolumn"><div class="row_padding"><CMS:ContentBlock ID="ContentBlock2" runat="server" DefaultContentID="1485" /></div></div>
        <div class="twocolumn"><div class="row_padding" id="company_overview_2"><CMS:ContentBlock ID="ContentBlock3" runat="server" DefaultContentID="1486" /></div></div>
    </div>
</div>
<div id="scrollToHere" style="width:100%; position:relative; top:-55px;"></div>
<br style="clear:both;" />
    <div runat="server" id="overview">

        <!-- tabs  -->
        <div class="row">
            <div class="tabs"> 
            <a href="#" class="active">testimonials<img src="/images/current_tab_arrow.png" class="tab_arrow1" /></a>
             <a href="#" id="article_tab" style="margin: 0 10px;">affiliations<img src="/images/current_tab_arrow.png" class="tab_arrow2" /></a></div>
        </div>
        <!-- end tabs  -->
    
        <div class="body_wrapper">
        <div class="body_wrapper2">
            <br /><br />
            <div class="swiper-container">
                <div class="swiper-wrapper">
                    <!-- testimonials  -->
                    <div class="swiper-slide">
                        <div class="content-slide" style="overflow:auto;">
                            <div class="row testimonials">
                                <div class="row_padding">
                                    <CMS:ContentBlock ID="ContentBlock4" runat="server" DefaultContentID="1487" />
                                    <CMS:Collection runat="server" ID="Collection1" DefaultCollectionID="7" DoInitFill="true" GetHtml="true" DisplayXslt="/xsl/testimonials.xsl"  />
                                </div>
                            </div>
                            <br style="clear:both;" />
                            <div class="nav-toggle" style="cursor:pointer; width:100%; text-align:center; color:#f1f1f1; font-family: 'futura light', Arial, Sans-Serif; font-size:18px;">
                                <br />
                                MORE TESTIMONIALS&nbsp;&nbsp;<img src="/images/homepage/red_arrow.png" />
                                <br />
                                <br />
                            </div>
                            <div class="row testimonials expanded" style="display:none;">
                                <div class="row_padding">
                                    <CMS:Collection runat="server" ID="Collection2" DefaultCollectionID="7" DoInitFill="true" GetHtml="true" DisplayXslt="/xsl/testimonials_extended.xsl"  />
                                </div>
                            </div>
                            <br style="clear:both;" />
                        </div>
                      </div>
                      <!-- affiliations  -->
                      <div class="swiper-slide">
                        <div class="content-slide" style="overflow:auto;">
                            <div class="row affiliations">
                                <div class="row_padding">
                                    <CMS:ContentBlock ID="ContentBlock5" runat="server" DefaultContentID="1496" />
                                </div>
                                <CMS:Collection runat="server" ID="Collection3" DefaultCollectionID="6" DoInitFill="true" GetHtml="true" DisplayXslt="/xsl/affiliations.xsl"  />
                            </div>
                        </div>
                      </div>
                  </div>
              </div>
          </div>
          </div>
    </div>
    
    
    <!------------------ DETAIL ------------------------->
    <div runat="server" id="detail">

        <!-- tabs  -->
        <div class="row" id="">
            <div class="tabs"> 
            <a href="/About-Vulcan/?page=testimonials" class="active">testimonials<img src="/images/current_tab_arrow.png" class="tab_arrow1" /></a>
             <a href="/About-Vulcan/?page=affiliations" id="a1" style="margin: 0 10px;">affiliations<img src="/images/current_tab_arrow.png" class="tab_arrow2" /></a></div>
        </div>
        <!-- end tabs  -->
    
        <div class="body_wrapper">
        <div class="body_wrapper2">
            <br /><br />
            <div class="swiper-container">
                <div class="swiper-wrapper">
                    <!-- testimonials  -->
                    <div class="swiper-slide" style="width:100%; margin: 0 auto;">
                        <div class="content-slide" style="overflow:auto;">
                            <div class="row press_releases">
                                <div class="press_releases_padding">
                                    <div class="events_title" style="font-size:32px;">
                                        <asp:Literal ID="title" runat="server" />
                                        <div style="float:right;" id="icons">
                                            <a href="javascript:print();"><img src="/images/print.jpg" style="padding-right:20px;" border="0" /></a>
                                            <a href="/about-vulcan/?page=testimonials"><img src="/images/close.jpg" border="0" /></a>
                                        </div>
                                    </div>
                                    <div style="color: #d4d3d3; font-family: 'helvetica', Arial, Sans-Serif; font-size: 16px; line-height: 22px;">
                                    	<CMS:ContentBlock ID="testimonial_article" runat="server" DynamicParameter="id" DisplayXslt="/xsl/testimonial_article.xsl" />
                                    </div>
                                </div>
                            </div>
                        </div>
                      </div>
                      <!-- affiliations  -->
                      <div class="swiper-slide">
                        <div class="content-slide" style="overflow:auto;">
                            <div class="row affiliations">
                               
                            </div>
                        </div>
                      </div>
                  </div>
              </div>
          </div>
          </div>
    </div>
  </asp:Content>  
  <asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
    <script type="text/javascript" src="/swiper/tabs/swiper.js"></script>
    
    <!-- Show More Testimonials -->
    <script type="text/javascript">
        $(document).ready(function() {
            $('.nav-toggle').click(function() {
                //make the collapse content to be shown or hide
                //$("#collapse1").toggle(300);

                var parent = $(this).parent();
                var copy = parent.children(".expanded");

                copy.toggle(300, function() {
                    var height = $(".swiper-slide-active .content-slide").height();
                    $('.swiper-container, .swiper-slide').height(height);
                });
                
                $(this).hide();

                /* if ($(this).attr('src') == "/uploadedImages/About_Us/All_About_Apples/read_more.jpg") {

                    $(this).attr('src', "/uploadedImages/About_Us/All_About_Apples/read_less.jpg");
                }
                else {
                $(this).attr('src', "/uploadedImages/About_Us/All_About_Apples/read_more.jpg");
                } */

            });
        });
    </script>
    
    <!-- Tabs -->
    <script type="text/javascript">
        if (document.URL.toLowerCase().indexOf("/about-vulcan/testimonials/") == -1) {

            if (getParameterByName('page') == "affiliations") {
                var tabsSwiper = new Swiper('.swiper-container', {
                    speed: 500,
                    initialSlide: 1,
                    onSlideChangeStart: function() {
                        $(".tabs .active").removeClass('active')
                        $(".tabs a").eq(tabsSwiper.activeIndex).addClass('active')

                        var height = $(".swiper-slide-active .content-slide").height();
                        $('.swiper-container, .swiper-slide').height(height);
                    }
                })

                $(".tabs .active").removeClass('active');
                $("#article_tab").addClass('active');

                $(document).ready(function() {
                    $('html, body').animate({
                        scrollTop: $("#scrollToHere").offset().top
                    }, 0);
                });
            }
            else {
                var tabsSwiper = new Swiper('.swiper-container', {
                    speed: 500,
                    onSlideChangeStart: function() {
                        $(".tabs .active").removeClass('active')
                        $(".tabs a").eq(tabsSwiper.activeIndex).addClass('active')

                        var height = $(".swiper-slide-active .content-slide").height();
                        $('.swiper-container, .swiper-slide').height(height);
                    }
                })
            }
            $(".tabs a").on('touchstart mousedown', function(e) {
                e.preventDefault()
                $(".tabs .active").removeClass('active')
                $(this).addClass('active')
                tabsSwiper.swipeTo($(this).index())
            })
            $(".tabs a").click(function(e) {
                e.preventDefault()
            })

            if (getParameterByName('page') == "testimonials") {
                $(document).ready(function() {
                    $('html, body').animate({
                        scrollTop: $("#scrollToHere").offset().top
                    }, 0);
                });
            }
        }
        else {
            $(document).ready(function() {
                $('html, body').animate({
                    scrollTop: $("#scrollToHere").offset().top
                }, 0);
            });
        }
  </script>
</asp:Content>