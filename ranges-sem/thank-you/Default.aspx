<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageBlank.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/ranges-sem/thank-you/" />
    
    <style type="text/css">
		.blue-list li {
			background-image: url("/images/spec_arrow_blue.png");
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
			.range-sem-body .sixty, .range-sem-body .forty {
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
    <div style="background-image:url('/images/products/blue_bar_wide.png'); background-position:bottom left; background-repeat:repeat-x; overflow: auto; position:relative;">
        <div class="row" style="overflow: hidden;">
            <div class="segment_bar">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td valign="bottom" class="segment_slant_td" style="height:58px; background-image:url('/images/products/blue_bar_left.png'); 
                            background-position:bottom right; background-repeat:no-repeat;"><div class="segment_slants"></div></td>
                        <td style="width:100%; background-color:#1e64a6;">
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td valign="top" class="icon"><img src="/images/products/ranges_icon.png" alt="Range" style="width:100%; max-width:33px; max-height:42px;" /></td>
                                    <td>&nbsp;Ranges</td>
                                </tr>
                            </table>
                        </td>
                        <td valign="bottom" class="segment_slant_td2" style="height:58px; background-image:url('/images/products/blue_bar_right.png'); 
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
                <CMS:ContentBlock ID="ContentBlock1" runat="server" DefaultContentID="5021" /> 
                <br />
                <div style="width:100%; text-align:center;">
                    <img src="/images/avail-shipment.jpg" alt="Available for Immediate Shipment" border="0" style="width:100%; max-width:371px; max-height:46px; border:1px solid #1E64A6;" />
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
         <div class="row range-sem-body">
         	<div class="sixty">
                <img src="/images/products/ranges_hero.png" alt="Ranges" style="width:100%; max-width:524px; vertical-align:bottom;" />
                <img src="/images/products/range_2014.png" alt="Best in Class" class="segment_bestinclass" />
            </div>
            <div class="forty">
                <div class="row_padding">
                	<br />
                	<!--dev: DefaultContentID="2442" -->
                	<CMS:ContentBlock ID="TestimonialBlock" runat="server" DefaultContentID="5024" /> 
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