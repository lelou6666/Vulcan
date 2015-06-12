<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="/swiper/tabs/swiper.css" rel="stylesheet" />
    
    <!-- Keep CSS on page for Swiper Custom CSS -->
    <style type="text/css">
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
            top:-40px;
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
            left:61px;
        }
        
        .tab_arrow2
        {
            position:absolute; 
            top:39px; 
            left:192px;
        }
        
        .tab_arrow3
        {
            position:absolute; 
            top:39px; 
            left:291px;
        }
        
        .press_releases_padding 
        {
            background-color:#222222; 
            padding:30px;
        }
        
        .header_copy
        {
            padding-bottom:30px;
        }
        
        
        
        @media all and (max-width:390px) 
	    {
		    .tabs 
		    {
		        width:290px;
		        top:-30px;
		    }
		    
		    .tabs a
            {
                font-size: 19px;
                padding: 8px 10px;
            }
		    
		    .article_tab
		    {
		        margin: 0 3px;
		    }
		    
		    .tab_arrow1
            {
                top:29px; 
                left:55px;
            }
            
            .tab_arrow2
            {
                top:29px; 
                left:152px;
            }
            
            .tab_arrow3
            {
                top:29px; 
                left:230px;
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
	        
	        #header_background, #scrollToHere, #footer_squares, #disclaimer, .tabs, #icons
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
    <div class="dots_background" style="min-height:275px;">
        <div class="row header_copy">
            <div class="row_padding">
                <br />
				<CMS:ContentBlock ID="ContentBlock1" runat="server" DefaultContentID="1466" />
            </div>
        </div>
        <div style="padding:30px;"></div>
        <div id="scrollToHere" style="width:100%; position:relative; top:-55px;"></div>
    </div>
    <div runat="server" id="overview">
        <!-- tabs  -->
        <div class="row">
            <div class="tabs"> 
            <a href="#" class="active">press releases<img src="/images/current_tab_arrow.png" class="tab_arrow1" /></a>
             <a href="#" class="article_tab" style="margin: 0 10px;">articles<img src="/images/current_tab_arrow.png" class="tab_arrow2" /></a> 
             <a href="#" class="event_tab">events<img src="/images/current_tab_arrow.png" class="tab_arrow3" /></a> </div>
        </div>
        <!-- end tabs  -->
        
        <div class="body_wrapper">
        <div class="body_wrapper2">
            <br /><br />
            <div class="swiper-container">
                <div class="swiper-wrapper">
                    <!-- Press Releases  -->
                    <div class="swiper-slide">
                        <div class="content-slide">
                            <div class="row press_releases">
                                <div class="row_padding">
                                    <CMS:Collection runat="server" ID="Collection1" DefaultCollectionID="4" DoInitFill="true" DisplayXslt="/xsl/press_releases.xsl"  />
                                </div>
                            </div>
                            <br style="clear:both;" />
                            <div class="nav-toggle" style="cursor:pointer; width:100%; text-align:center; color:#f1f1f1; font-family: 'futura light', Arial, Sans-Serif; font-size:18px;">
                                <br />
                                MORE RELEASES&nbsp;&nbsp;<img src="/images/homepage/red_arrow.png" />
                                <br />
                                <br />
                            </div>
                            <div class="row press_releases expanded" style="display:none;">
                                <div class="row_padding">
                                    <CMS:Collection runat="server" ID="Collection2" DefaultCollectionID="4" DoInitFill="true" DisplayXslt="/xsl/press_releases_extended.xsl"  />
                                </div>
                            </div>
                            <br style="clear:both;" />
                        </div>
                      </div>
                      <!-- Articles  -->
                      <div class="swiper-slide">
                        <div class="content-slide" style="overflow:auto;">
                            <div class="row articles">
                                <div class="row_padding">
                                    <CMS:Collection runat="server" ID="Collection4" DefaultCollectionID="18" DoInitFill="true" GetHtml="true" DisplayXslt="/xsl/articles.xsl"  />
                                </div>
                            </div>
                            <br style="clear:both;" />
                            <div class="nav-toggle" style="cursor:pointer; width:100%; text-align:center; color:#f1f1f1; font-family: 'futura light', Arial, Sans-Serif; font-size:18px;">
                                <br />
                                MORE ARTICLES&nbsp;&nbsp;<img src="/images/homepage/red_arrow.png" />
                                <br />
                                <br />
                            </div>
                            <div class="row articles expanded" style="display:none;">
                                <div class="row_padding">
                                    <CMS:Collection runat="server" ID="Collection5" DefaultCollectionID="18" DoInitFill="true" GetHtml="true" DisplayXslt="/xsl/articles-extended.xsl"  />
                                </div>
                            </div>
                            <br style="clear:both;" />
                        </div>
                      </div>
                      <!-- Events  -->
                      <div class="swiper-slide">
                        <div class="content-slide" style="overflow:auto;">
                            <div class="row events">
                                <div class="row_padding">
                                    <CMS:Collection runat="server" ID="Collection3" DefaultCollectionID="5" DoInitFill="true" GetHtml="true" DisplayXslt="/xsl/events.xsl"  />
                                </div>
                            </div>
                            <br style="clear:both;" />
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
        <div class="row">
            <div class="tabs"> 
            <a href="/News-and-Events/?page=press-releases" class="active">press releases<img src="/images/current_tab_arrow.png" class="tab_arrow1" /></a>
             <a href="/News-and-Events/?page=articles" id="a1" style="margin: 0 10px;">articles<img src="/images/current_tab_arrow.png" class="tab_arrow2" /></a> 
             <a href="/News-and-Events/?page=events">events<img src="/images/current_tab_arrow.png" class="tab_arrow3" /></a> </div>
        </div>
        <!-- end tabs  -->
        
        <div class="body_wrapper">
        <div class="body_wrapper2">
            <br />
            <div class="swiper-container">
                <div class="swiper-wrapper">
                    <!-- Press Releases  -->
                    <div class="swiper-slide" style="width:100%; margin: 0 auto;">
                        <div class="content-slide">
                            <div class="row press_releases">
                                <div class="press_releases_padding">
                                    <div style="float:right;" id="icons">
                                        <a href="javascript:print();"><img src="/images/print.jpg" style="padding-right:20px;" border="0" /></a>
                                        <a href="/News-and-Events/?page=press-releases"><img src="/images/close.jpg" border="0" /></a>
                                    </div>
                                    <div class="events_title" style="font-size:32px;">
                                        <asp:Literal ID="title" runat="server" />
                                    </div>
                                        <CMS:ContentBlock ID="testimonial_article" runat="server" DynamicParameter="id" />
                                </div>
                            </div>
                        </div>
                      </div>
                      <!-- Articles  -->
                      <div class="swiper-slide">
                        <div class="content-slide" style="overflow:auto;">
                            <div class="row">
                                <div class="row_padding">
                                  
                                </div>
                            </div>
                        </div>
                      </div>
                      <!-- Events  -->
                      <div class="swiper-slide">
                        <div class="content-slide" style="overflow:auto;">
                            <div class="row events">
                                <div class="row_padding">
                            
                                </div>
                            </div>
                        </div>
                      </div>
                  </div>
              </div>
          </div>
          <br /><br />
          </div>
          
    </div>
    </asp:Content>
    <asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
        <script type="text/javascript" src="/swiper/tabs/swiper.js"></script>
        
    <!-- Show More Releases -->
    <script type="text/javascript" defer>
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
                //tabsSwiper.resizeFix();

                

                /* if ($(this).attr('src') == "/uploadedImages/About_Us/All_About_Apples/read_more.jpg") {

                    $(this).attr('src', "/uploadedImages/About_Us/All_About_Apples/read_less.jpg");
                }
                else {
                $(this).attr('src', "/uploadedImages/About_Us/All_About_Apples/read_more.jpg");
                } */

            });
        });
        
        if (document.URL.toLowerCase().indexOf("/news-and-events/press-releases/") == -1) {

            if (getParameterByName('page') == "articles" || getParameterByName('page') == "events") {

                var initSlide = 1;

                if (getParameterByName('page') == "articles") {
                    initSlide = 1;
                }
                else {
                    initSlide = 2;
                }
                var tabsSwiper = new Swiper('.swiper-container', {
                    speed: 500,
                    initialSlide: initSlide,
                    onSlideChangeStart: function() {
                        $(".tabs .active").removeClass('active')
                        $(".tabs a").eq(tabsSwiper.activeIndex).addClass('active')

                        var height = $(".swiper-slide-active .content-slide").height();
                        $('.swiper-container, .swiper-slide').height(height);
                    }
                })

                $(".tabs .active").removeClass('active');

                if (getParameterByName('page') == "articles" ) {
                    $(".article_tab").addClass('active');
                }
                else {
                    $(".event_tab").addClass('active');
                }

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

            if (getParameterByName('page') == "press-releases") {
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