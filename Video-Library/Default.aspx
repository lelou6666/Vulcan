﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/video-library/" />
    <style type="text/css">
        .responsive-container { position: relative; padding-bottom: 56.25%; padding-top: 30px; height: 0; overflow: hidden; }
        .responsive-container iframe { position: absolute; top: 0; left: 0; width: 100%; height: 100%; }
        
        h2
        {
            border-bottom:1px solid #656564; 
            margin-bottom:25px; 
            padding-bottom:5px;
            padding-top:15px;
        }
        
        .video_heading h2
        {
            margin:0px;
            padding:0px;
            border:none;
        }
        
        .video_title
        {
             max-width:200px; 
             padding-top:15px;
        }
             
        .video_title a
        {
             font-family: 'futura medium condensed', Arial, Sans-Serif;
             font-size:19px;
             line-height:22px;
             color:#fdfcfc;
        }
        
    	#thumbs {}

		.thumb { border: 2px solid #535353; float: left; width:208px; margin-bottom:10px; height: 115px; background: url(http://a.vimeocdn.com/thumbnails/defaults/default.75x100.jpg); }

		#embed { background-color: #E7E7DE; width: 100%; max-width:1055px; overflow:auto; height:auto; float: left; padding: 10px; }

		#portrait { float: left; margin-right: 5px; max-width: 100px; }
		#stats { clear: both; margin-bottom: 20px; }
		
		.fourcolumn
		{
		    height:230px;
		}
		
		.videoDiv
		{
			width:50%;
			float:left;
			position:relative;
			height:auto;
			min-height:250px;
		}
		
		.video_padding
		{
		    padding:20px;
		    background-color: #1a1a1a;
		}
		
		#select_text
		{
		    font-family:'futura medium condensed', Arial, Sans-Serif; 
		    font-size:24px; 
		    line-height:30px; 
		    padding-top:5px; 
		    color:#c4c3c3; 
		    width:45%; 
		    position:relative; 
		    float:left; 
		    text-align:right;
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
		
		@media all and (max-width:940px) 
        {
            .fourcolumn
		    {
		        width:50%;
		    }
        }
        
        @media all and (max-width:700px) 
        {
            .video_heading h2
		    {
		        font-size: 28px;
		    }
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
        
        @media all and (max-width:500px) 
        {
            .fourcolumn
		    {
		        width:100%;
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
            <h1>Video Library</h1>
           
            <div style="background-color:#262626; padding:15px; overflow:auto; max-width:500px; margin:0 auto;">
                <div id="select_text">Select a video category:&nbsp;&nbsp;</div>
                <div id="contact_us">
                    <select id="category" onchange="goToCat();" style="vertical-align:middle; padding-top:10px;">
                      <option value="">Video Category</option>
                      <option value="K12">K-12</option>
                      <option value="Ranges">Ranges</option>
                      <option value="Griddles">Griddles & Charbroilers</option>
                      <option value="Fryers">Fryers</option>
                      <option value="Steams">Steamers</option>
                      <option value="Braising">Braising Pans</option>
                      <option value="Kettles">Kettles</option>
                      <option value="Ovens">Ovens</option>
                      <option value="Combi">Combi</option>
                      <!--<option value="Heated">Heated Holding</option>-->
                      <option value="Chefs">Celebrity Chefs</option>
                    </select>
                </div>
            </div>
        </div>
    </div>
</div>
 
<div class="dark_body_wrapper">
<div class="dark_body_wrapper2">
 <div style="margin:0 auto; text-align:center; width:100%; position:relative; z-index:2000; border-top:3px solid #494949;"><img src="/images/video_arrow.png" style="position:relative; top:-4px; margin:0 auto; text-align:center;" /></div>
     
    <div id="wrapper">
        <br style="clear:both;" /><br />
        	    <div class="thumb-wrapper">
        	    
        	        <!-- This contains the hidden content for videos -->
        	        <div id="video1" style="display:none; width:100%; overflow:auto; background-color:#262626; border-bottom:5px solid #1a1a1a;">
	                    <div style='max-width:1055px; margin: 0 auto;'>
	                        <br />
		                    <div class="video_padding" id='video1_content'>
		                        <div class="video_heading">
		                            <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video1');"><img src="/images/close2.png" border="0" /></a></div>
		                            <h2 style="border:none; color:#da1431; display:table-cell;">Now Playing:&nbsp;</h2>
		                            <div style="display:table-cell;" id="NowPlaying1"></div>
		                        </div>
		                        <br style="clear:both;" />
		                        <div class="responsive-container">
                                    <div id="embed1"></div>
                                </div>
		                    </div>
		                    <br />
	                    </div>
	                    <div style="margin:0 auto; text-align:center; width:100%; position:absolute; z-index:2000px;"><img src="/images/video_arrow2.png" style="position:absolute; top:5px; margin:0 auto; text-align:center;" /></div>
        	        </div>
                    
                    <div class="row">
                        <div class="row_padding">
                            <h2 id="WEBINAR">WEBINAR</h2>
                            <div style="position:relative; width:100%; overflow:auto;">
                            
                            <div class="fourcolumn"><a href="http://event.on24.com/eventRegistration/console/EventConsoleNG.jsp?uimode=nextgeneration&eventid=923723&sessionid=1&username=&partnerref=tynos&format=fhaudio&mobile=false&flashsupportedmobiledevice=false&helpcenter=false&key=92C025329F90F3814C1519467D69B136&text_language_id=en&playerwidth=1000&playerheight=650&overwritelobby=y&eventuserid=110747251&contenttype=A&mediametricsessionid=88524965&mediametricid=1410111&usercd=110747251&mode=launch#" target="_blank"><div style="display:table; position:absolute; top:10px;"><img src="/images/webinar-thumb.jpg" class="thumb"></div><div style="position: relative; display:block;"><img class="alsoview_overlay" src="/images/other_play.png" border="0"></div></a><div class="video_title"><a href="http://event.on24.com/eventRegistration/console/EventConsoleNG.jsp?uimode=nextgeneration&eventid=923723&sessionid=1&username=&partnerref=tynos&format=fhaudio&mobile=false&flashsupportedmobiledevice=false&helpcenter=false&key=92C025329F90F3814C1519467D69B136&text_language_id=en&playerwidth=1000&playerheight=650&overwritelobby=y&eventuserid=110747251&contenttype=A&mediametricsessionid=88524965&mediametricid=1410111&usercd=110747251&mode=launch#" target="_blank">Vulcan Chain Restaurant Webinar</a></div></div>
                            </div>
                        </div>
            	    </div>
                    <br style="clear:both;" />
        	        
        	        <div class="row">
                    <div class="row_padding">
            	        <h2 id="K12">K-12</h2>
            	        <div style="position:relative; width:100%; overflow:auto;" id="thumbs1"></div>
            	    </div>
            	    </div>
                    <br style="clear:both;" />
                    
                    <!-- This contains the hidden content for videos  -->  
	                <div id="video2" style="display:none; width:100%; overflow:auto; background-color:#262626; border-bottom:5px solid #1a1a1a;">
	                    <div style='max-width:1055px; margin: 0 auto;'>
	                        <br />
		                    <div class="video_padding" id='video2_content'>
		                        <div class="video_heading">
		                            <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video2');"><img src="/images/close2.png" border="0" /></a></div>
		                            <h2 style="border:none; color:#da1431; display:table-cell;">Now Playing:&nbsp;</h2>
		                            <div style="display:table-cell;" id="NowPlaying2"></div>
		                        </div>
		                        <br style="clear:both;" />
		                        <div class="responsive-container">
                                    <div id="embed2"></div>
                                </div>
		                    </div>
		                    <br />
		                </div>
	                    <div style="margin:0 auto; text-align:center; width:100%; position:absolute; z-index:2000px;"><img src="/images/video_arrow2.png" style="position:absolute; top:5px; margin:0 auto; text-align:center;" /></div>
        	        </div>
                   
                    
                    <div class="row">
                    <div class="row_padding">
                        <h2 id="Ranges">RANGES</h2>
            	        <div style="position:relative; width:100%; overflow:auto;" id="thumbs2"></div>
            	    </div>
            	    </div>
                    <br style="clear:both;" />
                    
                    <!-- This contains the hidden content for videos  -->
	                <div id="video3" style="display:none; width:100%; overflow:auto; background-color:#262626; border-bottom:5px solid #1a1a1a;">
	                    <div style='max-width:1055px; margin: 0 auto;'>
	                        <br />
		                    <div class="video_padding" id='video3_content'>
		                        <div class="video_heading">
		                            <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video3');"><img src="/images/close2.png" border="0" /></a></div>
		                            <h2 style="border:none; color:#da1431; display:table-cell;">Now Playing:&nbsp;</h2>
		                            <div style="display:table-cell;" id="NowPlaying3"></div>
		                        </div>
		                        <br style="clear:both;" />
		                        <div class="responsive-container">
                                    <div id="embed3"></div>
                                </div>
		                    </div>
		                    <br />
		                </div>
	                    <div style="margin:0 auto; text-align:center; width:100%; position:absolute; z-index:2000px;"><img src="/images/video_arrow2.png" style="position:absolute; top:5px; margin:0 auto; text-align:center;" /></div>
        	        </div>
                    
                    
                    <div class="row">
                    <div class="row_padding">
                        <h2 id="Griddles">GRIDDLES & CHARBROILERS</h2>
            	        <div style="position:relative; width:100%; overflow:auto;" id="thumbs3"></div>
            	    </div>
            	    </div>
                    
                    <!-- This contains the hidden content for videos  -->
	                <div id="video4" style="display:none; width:100%; overflow:auto; background-color:#262626; border-bottom:5px solid #1a1a1a;">
	                    <div style='max-width:1055px; margin: 0 auto;'>
	                        <br />
		                    <div class="video_padding" id='video4_content'>
		                        <div class="video_heading">
		                            <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video4');"><img src="/images/close2.png" border="0" /></a></div>
		                            <h2 style="border:none; color:#da1431; display:table-cell;">Now Playing:&nbsp;</h2>
		                            <div style="display:table-cell;" id="NowPlaying4"></div>
		                        </div>
		                        <br style="clear:both;" />
		                        <div class="responsive-container">
                                    <div id="embed4"></div>
                                </div>
		                    </div>
		                    <br />
		                </div>
	                    <div style="margin:0 auto; text-align:center; width:100%; position:absolute; z-index:2000px;"><img src="/images/video_arrow2.png" style="position:absolute; top:5px; margin:0 auto; text-align:center;" /></div>
        	        </div>
                    
                    
                    <div class="row">
                    <div class="row_padding">
                        <h2 id="Fryers">FRYERS</h2>
            	        <div style="position:relative; width:100%; overflow:auto;" id="thumbs4">
                        
                        <div class="fourcolumn"><a href="http://www.foodabletv.com/blog/2014/5/29/rock-my-restaurant-vulcan-presents-the-future-of-kitchen-equipment-video" title="Rock My Restaurant: Vulcan Presents the Future of Kitchen Equipment" target="_blank"><div style="display:table; position:absolute; top:10px;"><img src="/images/rock-my-restaurant-thumb.jpg" class="thumb"></div><div style="position: relative; display:block;"><img class="alsoview_overlay" src="/images/fryer_play.png" border="0"></div></a><div class="video_title"><a href="http://www.foodabletv.com/blog/2014/5/29/rock-my-restaurant-vulcan-presents-the-future-of-kitchen-equipment-video" title="Rock My Restaurant: Vulcan Presents the Future of Kitchen Equipment" target="_blank">Rock My Restaurant: Vulcan Presents the Future of Kitchen Equipment</a></div></div>
                        </div>
            	    </div>
            	    </div>
                    <br style="clear:both;" />
                    
                    <!-- This contains the hidden content for videos  -->
	                <div id="video5" style="display:none; width:100%; overflow:auto; background-color:#262626; border-bottom:5px solid #1a1a1a;">
	                    <div style='max-width:1055px; margin: 0 auto;'>
	                        <br />
		                    <div class="video_padding" id='video5_content'>
		                        <div class="video_heading">
		                            <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video5');"><img src="/images/close2.png" border="0" /></a></div>
		                            <h2 style="border:none; color:#da1431; display:table-cell;">Now Playing:&nbsp;</h2>
		                            <div style="display:table-cell;" id="NowPlaying5"></div>
		                        </div>
		                        <br style="clear:both;" />
		                        <div class="responsive-container">
                                    <div id="embed5"></div>
                                </div>
		                    </div>
		                    <br />
		                </div>
	                    <div style="margin:0 auto; text-align:center; width:100%; position:absolute; z-index:2000px;"><img src="/images/video_arrow2.png" style="position:absolute; top:5px; margin:0 auto; text-align:center;" /></div>
        	        </div>
                    
                    
                    <div class="row">
                    <div class="row_padding">
                        <h2 id="Steams">STEAM</h2>
            	        <div style="position:relative; width:100%; overflow:auto;" id="thumbs5"><!-- -->
                     
                        
                        <!-- --></div>
            	    </div>
            	    </div>
                    <br style="clear:both;" />
                    
                    <!-- This contains the hidden content for videos  -->
	                <div id="video6" style="display:none; width:100%; overflow:auto; background-color:#262626; border-bottom:5px solid #1a1a1a;">
	                    <div style='max-width:1055px; margin: 0 auto;'>
	                        <br />
		                    <div class="video_padding" id='video6_content'>
		                        <div class="video_heading">
		                            <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video6');"><img src="/images/close2.png" border="0" /></a></div>
		                            <h2 style="border:none; color:#da1431; display:table-cell;">Now Playing:&nbsp;</h2>
		                            <div style="display:table-cell;" id="NowPlaying6"></div>
		                        </div>
		                        <br style="clear:both;" />
		                        <div class="responsive-container">
                                    <div id="embed6"></div>
                                </div>
		                    </div>
		                    <br />
		                </div>
	                    <div style="margin:0 auto; text-align:center; width:100%; position:absolute; z-index:2000px;"><img src="/images/video_arrow2.png" style="position:absolute; top:5px; margin:0 auto; text-align:center;" /></div>
        	        </div>
                    
                    
                    <div class="row">
                    <div class="row_padding">
                        <h2 id="Ovens">OVENS</h2>
            	        <div style="position:relative; width:100%; overflow:auto;" id="thumbs6"></div>
            	    </div>
            	    </div>
                    
                    <br style="clear:both;" />
                    
                    <!-- This contains the hidden content for videos  -->
	                <!--<div id="video7" style="display:none; width:100%; overflow:auto; background-color:#262626; border-bottom:5px solid #1a1a1a;">
	                    <div style='max-width:1055px; margin: 0 auto;'>
	                        <br />
		                    <div class="video_padding" id='video7_content'>
		                        <div class="video_heading">
		                            <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video7');"><img src="/images/close2.png" border="0" /></a></div>
		                            <h2 style="border:none; color:#da1431; display:table-cell;">Now Playing:&nbsp;</h2>
		                            <div style="display:table-cell;" id="NowPlaying7"></div>
		                        </div>
		                        <br style="clear:both;" />
		                        <div class="responsive-container">
                                    <div id="embed7"></div>
                                </div>
		                    </div>
		                    <br />
		                </div>
	                    <div style="margin:0 auto; text-align:center; width:100%; position:absolute; z-index:2000px;"><img src="/images/video_arrow2.png" style="position:absolute; top:5px; margin:0 auto; text-align:center;" /></div>
        	        </div>
                    
                    
                    <div class="row">
                    <div class="row_padding">
                        <h2 id="Heated">HEATED HOLDING</h2>
            	        <div style="position:relative; width:100%; overflow:auto;" id="thumbs7"></div>
            	    </div>
            	    </div>-->
                    
                    
                    
                    <!-- This contains the hidden content for videos  -->
	                <div id="video9" style="display:none; width:100%; overflow:auto; background-color:#262626; border-bottom:5px solid #1a1a1a;">
	                    <div style='max-width:1055px; margin: 0 auto;'>
	                        <br />
		                    <div class="video_padding" id='video9_content'>
		                        <div class="video_heading">
		                            <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video9');"><img src="/images/close2.png" border="0" /></a></div>
		                            <h2 style="border:none; color:#da1431; display:table-cell;">Now Playing:&nbsp;</h2>
		                            <div style="display:table-cell;" id="NowPlaying9"></div>
		                        </div>
		                        <br style="clear:both;" />
		                        <div class="responsive-container">
                                    <div id="embed9"></div>
                                </div>
		                    </div>
		                    <br />
		                </div>
	                    <div style="margin:0 auto; text-align:center; width:100%; position:absolute; z-index:2000px;"><img src="/images/video_arrow2.png" style="position:absolute; top:5px; margin:0 auto; text-align:center;" /></div>
        	        </div>
                    
                    
                    <div class="row">
                        <div class="row_padding">
                            <h2 id="Combi">Combi</h2>
                            <div style="position:relative; width:100%; overflow:auto;" id="thumbs9"></div>
                        </div>
            	    </div>
                    <br style="clear:both;" />
                    
                    <!-- This contains the hidden content for videos  -->
	                <div id="video8" style="display:none; width:100%; overflow:auto; background-color:#262626; border-bottom:5px solid #1a1a1a;">
	                    <div style='max-width:1055px; margin: 0 auto;'>
	                        <br />
		                    <div class="video_padding" id='video8_content'>
		                        <div class="video_heading">
		                            <div style="position:relative; float:right;"><a href="javascript:hideVideo('#video8');"><img src="/images/close2.png" border="0" /></a></div>
		                            <h2 style="border:none; color:#da1431; display:table-cell;">Now Playing:&nbsp;</h2>
		                            <div style="display:table-cell;" id="NowPlaying8"></div>
		                        </div>
		                        <br style="clear:both;" />
		                        <div class="responsive-container">
                                    <div id="embed8"></div>
                                </div>
		                    </div>
		                    <br />
		                </div>
	                    <div style="margin:0 auto; text-align:center; width:100%; position:absolute; z-index:2000px;"><img src="/images/video_arrow2.png" style="position:absolute; top:5px; margin:0 auto; text-align:center;" /></div>
        	        </div>
                    
                    <div class="row">
                        <div class="row_padding">
                            <h2 id="Chefs">CELEBRITY CHEFS</h2>
                            <div style="position:relative; width:100%; overflow:auto;" id="thumbs8"></div>
                        </div>
            	    </div>
                    
                </div>
            </div>
        </div>
	</div>
</asp:Content>
<asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
	<script type="text/javascript">

	    var apiEndpoint = 'http://vimeo.com/api/v2/';
	    var oEmbedEndpoint = 'http://vimeo.com/api/oembed.json'
	    var oEmbedCallback = 'switchVideo';
	    var videosCallback = 'setupGallery';
	    var vimeoUsername = 'user12962297';
	    var count = 1;

	    // Get the user's videos
	    $(document).ready(function() {
	        LoadAlbums();
	    });

	    function LoadAlbums() {
	        if (count == 1) {
	            $.getScript(apiEndpoint + 'album/' + '2592754' + '/videos.json?callback=' + videosCallback);
	        }
	        else if (count == 2) {
	            $.getScript(apiEndpoint + 'album/' + '2592761' + '/videos.json?callback=' + videosCallback);
	        }
	        else if (count == 3) {
	            $.getScript(apiEndpoint + 'album/' + '2592765' + '/videos.json?callback=' + videosCallback);
	        }
	        else if (count == 4) {
	            $.getScript(apiEndpoint + 'album/' + '2592760' + '/videos.json?callback=' + videosCallback);
	        }
	        else if (count == 5) {
	            $.getScript(apiEndpoint + 'album/' + '2592763' + '/videos.json?callback=' + videosCallback);
	        }
	        else if (count == 6) {
	            $.getScript(apiEndpoint + 'album/' + '2592762' + '/videos.json?callback=' + videosCallback);
	        }
	        else if (count == 7) {
	            $.getScript(apiEndpoint + 'album/' + '2625619' + '/videos.json?callback=' + videosCallback);
	        }
			else if (count == 8) {
	            $.getScript(apiEndpoint + 'album/' + '2972760' + '/videos.json?callback=' + videosCallback);
	        }
	        else if (count == 9) {
	            VideoOnLoad();
	        }
	    }


	    // Get the user's videos
	    /* $(document).ready(function() {
	    $.getScript(apiEndpoint + vimeoUsername + '/videos.json?callback=' + videosCallback);
	    }); */

	    var current = "";
	    function getVideo(url, title, player) {
			//$.getScript(oEmbedEndpoint + '?url=' + url + '&width=504&height=280&callback=' + oEmbedCallback);
			var playThis = "#embed" + player;
			var video = "#video" + player;
			var TitleDiv = video + " #NowPlaying" + player;
			$(TitleDiv).html('<h2>' + title + '</h2>');
			$(playThis).html('<iframe src="' + url + '?badge=0&portrait=0&title=0&byline=0&api=1" width="504" height="260" frameborder="0" title="Vulcan VC Series Gas Convection Ovens" webkitallowfullscreen="" mozallowfullscreen="" allowfullscreen=""></iframe>');


			// This pauses all frames within tabs container.
			var data = { method: 'pause' };
			$("iframe").each(function() {
				var url = $(this).attr('src').split('?')[0];
				this.contentWindow.postMessage(JSON.stringify(data), url);
			});

			if (current != "") {
				if (current != video) {
					$(current).hide();
				}
			}
			current = video;

			$(video).fadeIn(800);

			$("html, body").animate({ scrollTop: $(video).offset().top - 150 }, 300);
	    }

	    function hideVideo(video) {
	        $(video).hide();

	        var data = { method: 'pause' };
	        // This pauses all frames within tabs container.
	        $("iframe").each(function() {
	            var url = $(this).attr('src').split('?')[0];
	            this.contentWindow.postMessage(JSON.stringify(data), url);
	        });
	    }

	    function setupGallery(videos) {

	        // Set the user's thumbnail and the page title
	        /* $('#stats').prepend('<img id="portrait" src="' + videos[0].user_portrait_medium + '" />');
	        $('#stats h2').text(videos[0].user_name + "'s Videos"); */

	        // Load the first video
	        //getVideo(videos[0].url);

	        // Add the videos to the gallery
	        for (var i = 0; i < videos.length; i++) {
	            if (count == 1) {
	                var html = '<div class="fourcolumn"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '"><div style="display:table; position:absolute; top:10px;"><img src="' + videos[i].thumbnail_medium + '" class="thumb" /></div>';
	                html += '<div style="position: relative; display:block;"><img class="alsoview_overlay" src="/images/other_play.png" border="0" />';
	                html += '</div></a><div class="video_title"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '">' + videos[i].title + '</a></div></div>';
	                $('#thumbs1').append(html);

	                // Switch to the video when a thumbnail is clicked
	                $('#thumbs1 a').click(function(event) {
						event.preventDefault();
						getVideo(this.href, this.title, 1);
						return false;
	                });
	            }
	            else if (count == 2) {
	                var html = '<div class="fourcolumn"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '"><div style="display:table; position:absolute; top:10px;"><img src="' + videos[i].thumbnail_medium + '" class="thumb" /></div>';
	                html += '<div style="position: relative; display:block;"><img class="alsoview_overlay" src="/images/range_play.png" border="0" />';
	                html += '</div></a><div class="video_title"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '">' + videos[i].title + '</a></div></div>';

	                $('#thumbs2').append(html);

	                // Switch to the video when a thumbnail is clicked
	                $('#thumbs2 a').click(function(event) {
	                    event.preventDefault();
	                    getVideo(this.href, this.title, 2);
	                    return false;
	                });
	            }
	            else if (count == 3) {
	                var html = '<div class="fourcolumn"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '"><div style="display:table; position:absolute; top:10px;"><img src="' + videos[i].thumbnail_medium + '" class="thumb" /></div>';
	                html += '<div style="position: relative; display:block;"><img class="alsoview_overlay" src="/images/griddle_play.png" border="0" />';
	                html += '</div></a><div class="video_title"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '">' + videos[i].title + '</a></div></div>';
	                $('#thumbs3').append(html);

	                // Switch to the video when a thumbnail is clicked
	                $('#thumbs3 a').click(function(event) {
	                    event.preventDefault();
	                    getVideo(this.href, this.title, 3);
	                    return false;
	                });
	            }
	            else if (count == 4) {
	                var html = '<div class="fourcolumn"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '"><div style="display:table; position:absolute; top:10px;"><img src="' + videos[i].thumbnail_medium + '" class="thumb" /></div>';
	                html += '<div style="position: relative; display:block;"><img class="alsoview_overlay" src="/images/fryer_play.png" border="0" />';
	                html += '</div></a><div class="video_title"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '">' + videos[i].title + '</a></div></div>';
	                $('#thumbs4').append(html);

	                // Switch to the video when a thumbnail is clicked
	                $('#thumbs4 a').click(function(event) {
						if(this.title != "Rock My Restaurant: Vulcan Presents the Future of Kitchen Equipment") {
							event.preventDefault();
							getVideo(this.href, this.title, 4);
							return false;
						}
	                });
	            }
	            else if (count == 5) {
	                var html = '<div class="fourcolumn"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '"><div style="display:table; position:absolute; top:10px;"><img src="' + videos[i].thumbnail_medium + '" class="thumb" /></div>';
	                html += '<div style="position: relative; display:block;"><img class="alsoview_overlay" src="/images/steam_play.png" border="0" />';
	                html += '</div></a><div class="video_title"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '">' + videos[i].title + '</a></div></div>';
	                $('#thumbs5').append(html);

	                // Switch to the video when a thumbnail is clicked
	                $('#thumbs5 a').click(function(event) {
	                    event.preventDefault();
	                    getVideo(this.href, this.title, 5);
	                    return false;
	                });
	            }
	            else if (count == 6) {
	                var html = '<div class="fourcolumn"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '"><div style="display:table; position:absolute; top:10px;"><img src="' + videos[i].thumbnail_medium + '" class="thumb" /></div>';
	                html += '<div style="position: relative; display:block;"><img class="alsoview_overlay" src="/images/oven_play.png" border="0" />';
	                html += '</div></a><div class="video_title"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '">' + videos[i].title + '</a></div></div>';
	                $('#thumbs6').append(html);

	                // Switch to the video when a thumbnail is clicked
	                $('#thumbs6 a').click(function(event) {
	                    event.preventDefault();
	                    getVideo(this.href, this.title, 6);
	                    return false;
	                });
	            }
	            else if (count == 7) {
	                var html = '<div class="fourcolumn"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '"><div style="display:table; position:absolute; top:10px;"><img src="' + videos[i].thumbnail_medium + '" class="thumb" /></div>';
	                html += '<div style="position: relative; display:block;"><img class="alsoview_overlay" src="/images/other_play.png" border="0" />';
	                html += '</div></a><div class="video_title"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '">' + videos[i].title + '</a></div></div>';
	                $('#thumbs8').append(html);

	                // Switch to the video when a thumbnail is clicked
	                $('#thumbs8 a').click(function(event) {
	                    event.preventDefault();
	                    getVideo(this.href, this.title, 8);
	                    return false;
	                });
	            }
				else if (count == 8) {
	                var html = '<div class="fourcolumn"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '"><div style="display:table; position:absolute; top:10px;"><img src="' + videos[i].thumbnail_medium + '" class="thumb" /></div>';
	                html += '<div style="position: relative; display:block;"><img class="alsoview_overlay" src="/images/combi_play.png" border="0" />';
	                html += '</div></a><div class="video_title"><a href="http://player.vimeo.com/video/' + videos[i].id + '" title="' + videos[i].title + '">' + videos[i].title + '</a></div></div>';
	                $('#thumbs9').append(html);

	                // Switch to the video when a thumbnail is clicked
	                $('#thumbs9 a').click(function(event) {
	                    event.preventDefault();
	                    getVideo(this.href, this.title, 8);
	                    return false;
	                });
	            }
	        }

	        if (count < 9) {
	            count = count + 1;
	            LoadAlbums();
	        }
	    }

	    function switchVideo(video) {
	        $('#embed').html(unescape(video.html));
	    }

	    function goToCat() {
	        var e = document.getElementById("category");
	        var vSelected = e.options[e.selectedIndex].value;

	        if (vSelected == "Braising" || vSelected == "Kettles") {
	            vSelected = "Steams";
	        }
	        var goTo = "#" + vSelected;
	        if (vSelected != "") {
	            $("html, body").animate({ scrollTop: $(goTo).offset().top - 150 }, 700);
	        }
	    }

	    function VideoOnLoad() {
	        var video = getParameterByName("video");
	        var title = getParameterByName("title");
	        var player = getParameterByName("player");
	        var playerLocation = "#video";

	        if (player == "") {
	            player = 1;
	        }
	        if (video != "" && title != "") {
	            playerLocation = playerLocation + player;

	            getVideo("http://player.vimeo.com/video/" + video, title, player);
	        }
	    }

	    function getParameterByName(name) {
	        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
	        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
	        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
	    }

	</script>
</asp:Content>