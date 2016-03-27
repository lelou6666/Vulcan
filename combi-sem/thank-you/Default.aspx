<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageBlank.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/combi-sem/thank-you/" />
    
    <style type="text/css">
		#header_background, .segment_fixed {
			position:relative;
		}
		
		.segment_fixed {
			top:auto;
		}
		
		.body-top-padding{
			top:auto;
			padding-top: 30px;
		}
		
		iframe[name='google_conversion_frame'] { 
			height: 0 !important;
			width: 0 !important; 
			line-height: 0 !important; 
			font-size: 0 !important;
			margin-top: -13px;
			float: left;
		}
		
		.blue-list li {
			background-image: url("/images/spec_arrow_yellow.png");
			background-repeat: no-repeat;
			padding-left: 15px;
		}
		
		.blue-list ul {
			list-style: none;
			margin-left:15px;
		}
		
		.segment_body .segment_title
        {
            font-size:41px;
            line-height:45px;
            color:#e8e8e8;
        }
		
		.inline-div {
			display:table-cell;
			vertical-align:top;
		}
		
		::-webkit-input-placeholder {
			color:#1b1a1b;
		}
		
		:-moz-placeholder { /* Firefox 18- */
			color:#1b1a1b;
		}
		
		::-moz-placeholder {  /* Firefox 19+ */
			color:#1b1a1b;
		}
		
		:-ms-input-placeholder {  
			color:#1b1a1b;
		}
		
		#segment_desc {
			background-image: url("/images/grey-shade.png");
			background-repeat: repeat;
		}
		
		#free-quote input {
			width:90%;
			background-color:#a5a5a5;
			border: 1px solid #343434;
			height:29px;
			line-height:29px;
			vertical-align:middle;
			color: #343434;
			font-family: 'helvetica', Arial, Sans-Serif;
			font-size:17px;
			padding:5px;
		}
		
		.comment input {
			width:95% !important;
		}
		
		#free-quote input[type=image] {
			background-color:transparent;
			border:none;
		}
		
		#free-quote .twocolumn {
			margin-bottom: 10px;
		}
		
		.headtwocolumn1, .headtwocolumn2 {
			position: relative;
			padding-left: 0em;
			padding-right: 0em;
			float: left;
			width: 50%;
		}
		
		.form_fields {
			display: inline;
		}
		
		#logo {
			height:auto;
		}
		
		.testimonial_title {
			font-family: 'futura medium condensed', Arial, Sans-Serif;
			font-size: 30px;
			line-height: 35px;
			color: #eeeded;
		}

		
		@media all and (max-width:1075px) 
		{
			.headtwocolumn1 {
				width:40%;
			}
			.headtwocolumn2 {
				width:60%;
			}
		}
		
		@media all and (max-width:930px) 
		{
			.range-sem-body .twocolumn, .range-sem-body .twocolumn {
				width:100%;
			}
			
			.headtwocolumn1 {
				/* display:none; */
			}
			
			.headtwocolumn1, .headtwocolumn2 {
				width:100%;
			}
			
			.headtwocolumn2 .segment_title {
				text-align:center;
			}
			
			#free-quote input[type=image] {
				margin-bottom:20px;
			}
		}
		
		@media all and (max-width:768px) 
        {
            .segment_slant_td
            {
                background-color:#1e64a6;
            }
			
			.segment_slant_td .segment_slants {
				width:10px;
			}
        }
		
		@media all and (max-width:470px) 
		{
			.inline-div {
				display:block;
			}
			
			.headtwocolumn2 .row_padding {
				padding: 0 6px;
			}
			
			.segment_body .segment_title
			{
				font-size:30px;
				line-height:33px;
				color:#e8e8e8;
			}
		}
	</style>
</asp:Content>
<asp:Content ID="product_segment" ContentPlaceHolderID="product_segment" Runat="Server">
<div class="segment_fixed">
    <div style="background-image:url('/images/products/yellow_bar_wide.png'); background-position:bottom left; background-repeat:repeat-x; overflow: auto; position:relative;">
        <div class="row" style="overflow: hidden;">
            <div class="segment_bar">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td valign="bottom" class="segment_slant_td" style="height:58px; background-image:url('/images/products/yellow_bar_left.png'); 
                            background-position:bottom right; background-repeat:no-repeat;"><div class="segment_slants"></div></td>
                        <td style="width:100%; background-color:#e3ae30;">
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td valign="top" class="icon"><img src="/images/products/combi_icon.png" alt="Combi" style="width:100%; max-width:33px; max-height:42px;" /></td>
                                    <td>&nbsp;New Vulcan Combi</td>
                                </tr>
                            </table>
                        </td>
                        <td valign="bottom" class="segment_slant_td2" style="height:58px; background-image:url('/images/products/yellow_bar_right.png'); 
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
	<div class="body_wrapper">
    <div class="body_wrapper2">
        <div class="row" style="overflow:hidden;">
            <div class="row_padding">
            	<div id="segment_desc">
                	<div style="width:100%; max-width:600px; text-align:center; margin:0 auto;">
                    	<div style="padding:20px 15px;">
                        	<!--dev: DefaultContentID="2443" -->
            				<CMS:ContentBlock ID="TY" runat="server" DefaultContentID="5023" /> 
                        </div>
                    </div>
                </div>
            	<br />
                <br />
                <!--dev: DefaultContentID="2441" -->
                <CMS:ContentBlock ID="ContentBlock1" runat="server" DefaultContentID="5720" /> 
                <br />
                <div style="width:100%; text-align:center;">
                    <img src="/images/avail-shipment.jpg" alt="Available for Immediate Shipment" border="0" style="width:100%; max-width:371px; max-height:46px; border:1px solid #E3AE30;" />
                </div>
            </div>
        </div>
        <br /><br />
    </div>
    </div>
    <div style="background-color:#060606; width:100%; height:auto; overflow:auto; 
        background-image: url('/images/products/segment_body_bg.jpg'); background-repeat:repeat-x; background-position:bottom left;">
    <div id="left_fade_product1">
    <div id="right_fade_product1">
    	<!-- This contains the hidden content for videos -->
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
    
         <div class="row range-sem-body">
         	<div class="twocolumn">
                <img src="/combi-sem/img/combi.png" alt="Combi" border="0"  style="width:100%; max-width:480px; max-height:415px; vertical-align:bottom;" />
            </div>
            <div class="twocolumn">
                <div class="row_padding">
                	<br /><br />
                    <p class="segment_title" style="text-align:center; font-size:30px;">See how easy the Vulcan Combi is to use<br />
                    <a class='video' href="javascript:showVideo('#video4');" onClick="ga('send', 'event', 'Combi SEM', 'Vulcan Combi Video');"><img src="/combi-sem/img/video.jpg" alt="Vulcan Video" border="0"  style="width:100%; max-width:466px; max-height:264px; vertical-align:bottom; padding-top:15px;" /></a></p>
                </div>
            </div>
         </div>
         <br style="clear:both;" />
     </div>
     </div>
     </div>
 </div>
</asp:Content>
<asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
 <script src="/js/placeholder.js"></script>
 
 <script type="text/javascript">
  function fnOnUpdateValidators() {
            for (var i = 0; i < Page_Validators.length; i++) {
                var val = Page_Validators[i];
                var ctrl = document.getElementById(val.controltovalidate);

                if (i > 0) {
                    if (ctrl != null && ctrl.style != null) {
                        if (!val.isvalid) {
                            ctrl.style.background = '#f8dbdf';
                            ctrl.style.border = '1px solid #e13433';

                        }
                        else {
                            if (ctrl != prevControl) {
                                ctrl.style.backgroundColor = '';
                                ctrl.style.border = 'none';
                            }
                        }
                    }
                }
                else {
                    if (ctrl != null && ctrl.style != null) {
                        if (!val.isvalid) {
                            ctrl.style.background = '#f8dbdf';
                            ctrl.style.border = '1px solid #e13433';

                        }
                        else {
                            ctrl.style.backgroundColor = '';
                            ctrl.style.border = 'none';
                        }
                    }
                }
                prevControl = document.getElementById(val.controltovalidate);
            }
        }
		
	function showVideo(video) {
        $(video).fadeIn(800);
		$(".range-sem-body").hide();
    }

    function hideVideo(video) {
        $(video).hide();
		$(".range-sem-body").fadeIn(800);

        var data = { method: 'pause' };
        // This pauses all frames within tabs container.
        $("iframe").each(function() {
            var url = $(this).attr('src').split('?')[0];
            this.contentWindow.postMessage(JSON.stringify(data), url);
        });
    }
    </script>
    
    <!-- Google Code for Sign Up Conversion Page -->
	<script type="text/javascript">
    /* <![CDATA[ */
    var google_conversion_id = 964139843;
    var google_conversion_language = "en";
    var google_conversion_format = "3";
    var google_conversion_color = "ffffff";
    var google_conversion_label = "xF_lCMq-hVkQw7beywM";
    var google_remarketing_only = false;
    /* ]]> */
    </script>
    <script type="text/javascript" src="http://www.googleadservices.com/pagead/conversion.js">
    </script>
    <noscript>
    <div style="display:inline;">
    <img height="1" width="1" style="border-style:none;" alt="" src="http://www.googleadservices.com/pagead/conversion/964139843/?label=xF_lCMq-hVkQw7beywM&amp;guid=ON&amp;script=0"/>
    </div>
    </noscript>
</asp:Content>