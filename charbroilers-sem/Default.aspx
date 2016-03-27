<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageBlank.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/charbroilers-sem/" />
    
    <style type="text/css">
		.blue-list li {
			background-image: url("/images/spec_arrow_orange.png");
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
    <div style="background-image:url('/images/products/orange_bar_wide.png'); background-position:bottom left; background-repeat:repeat-x; overflow: auto; position:relative;">
        <div class="row" style="overflow: hidden;">
            <div class="segment_bar">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td valign="bottom" class="segment_slant_td" style="height:58px; background-image:url('/images/products/orange_bar_left.png'); 
                            background-position:bottom right; background-repeat:no-repeat;"><div class="segment_slants"></div></td>
                        <td style="width:100%; background-color:#ba371e;">
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td valign="top" class="icon"><img src="/images/products/griddle_icon.png" alt="Griddles &amp; Charbroilers" style="width:100%; max-width:46px; max-height:44px;" /></td>
                                    <td>&nbsp;Charbroilers</td>
                                </tr>
                            </table>
                        </td>
                        <td valign="bottom" class="segment_slant_td2" style="height:58px; background-image:url('/images/products/orange_bar_right.png'); 
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
                <div class="headtwocolumn1">
                	<div style="padding:0 25px 0 10px;">
                    	<!--dev: DefaultContentID="2441" -->
                		<CMS:ContentBlock ID="ContentBlock1" runat="server" DefaultContentID="5164" /> 
                        <br />
                        <div style="width:100%; text-align:center;">
                        	<img src="/images/avail-shipment.jpg" alt="Available for Immediate Shipment" border="0" style="width:100%; max-width:371px; max-height:46px; border:1px solid #BA371E;" />
                        </div>
                    </div>
                </div>
                <div class="headtwocolumn2">
                    <div class="row_padding" id="segment_desc" style="margin-top:15px;">
                        <div class="segment_title" style="padding-top:20px; padding-bottom:20px; text-align:center;">Get your free quote!</div>
                         <!--begin -->
                         <div id="free-quote">
                             <div class="row">
                                 <div class="twocolumn">
                                    <div class="form_fields">
                                        <asp:TextBox runat="server" CssClass="inputbox" title="first" ID="first" placeholder="First Name:" />
                                        <asp:RequiredFieldValidator ID="vfirst" runat="server" ValidationGroup="ContactUs" ControlToValidate="first" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                                    </div>
                                 </div>
                                 <div class="twocolumn"> 
                                    <div class="form_fields">
                                        <asp:TextBox runat="server" class="inputbox" name="last" ID="last" placeholder="Last Name:" />
                                        <asp:RequiredFieldValidator ID="vlast" runat="server" ValidationGroup="ContactUs" ControlToValidate="last" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                                    </div>
                                 </div>
                             </div>
                             <br style="clear:both;" />
                             <div class="row">
                             	<div class="twocolumn">
                                    <div class="form_fields">
                                        <asp:TextBox runat="server" class="inputbox" name="company" ID="company" placeholder="Company:" />
                                        <asp:RequiredFieldValidator ID="vcompany" runat="server" ValidationGroup="ContactUs" ControlToValidate="company" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                                    </div>
                                </div>
                                <div class="twocolumn">
                                    <div class="form_fields">
                                        <asp:TextBox runat="server" class="inputbox" name="phone" ID="phone" type="text" maxlength="12" placeholder="Phone:"  />
                                        <asp:RequiredFieldValidator ID="vphone" runat="server" ValidationGroup="ContactUs" ControlToValidate="phone" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                                    </div>
                                </div>
                             </div>
                             <br style="clear:both;" />
                             <div class="row">
                             	<div class="twocolumn">
                                    <div class="form_fields">
                                        <asp:TextBox runat="server" class="inputbox" name="email" ID="email" placeholder="Email:" />
                                        <asp:RequiredFieldValidator ID="vemail" runat="server" ValidationGroup="ContactUs" ControlToValidate="email" Text="" CssClass="fieldReq" ForeColor="#e13433" /><asp:RegularExpressionValidator ID="vemail2" ForeColor="#e13433" CssClass="fieldReq"  runat="server" ControlToValidate="email" ValidationGroup="ContactUs" ErrorMessage="" ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                             	<div class="twocolumn">
                                    <div class="form_fields">
                                        <asp:TextBox runat="server" class="inputbox" name="zipcode" ID="zipcode" placeholder="Zip Code:" />
                                        <asp:RequiredFieldValidator ID="vzipcode" runat="server" ValidationGroup="ContactUs" ControlToValidate="zipcode" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                                    </div>
                                </div>
                             </div>
                             <br style="clear:both;" />
                             <div class="row">
                                <div class="form_fields comment" style="display:block;">
                                    <asp:TextBox runat="server" class="inputbox" name="comments" ID="comments" placeholder="Comments:" />
                                </div>
                            </div>
                            <br style="clear:both;" />
                            <div class="row">
            					<div style="width:100%; text-align:center;"><asp:ImageButton ID="submit" runat="server" ImageUrl="/images/products/free-quote.jpg" ValidationGroup="ContactUs" OnClick="Submit_Click" Width="264" Height="47" /></div>
                            </div>
                            <br />
                         </div>
                         <!--end -->
                	</div>
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
                <img src="/images/products/griddles_charbroilers_hero.png" alt="Charbroilers" style="width:100%; max-width:500px; vertical-align:bottom;" />
                <img src="/uploadedImages/Vulcan/Vulcan_Categories/BIC-Charbroiler-2014.png" alt="Best in Class" class="segment_bestinclass">
            </div>
            <div class="forty">
                <div class="row_padding">
                	<br />
                    <!--dev: DefaultContentID="2442" -->
                	<CMS:ContentBlock ID="TestimonialBlock" runat="server" DefaultContentID="5165" /> 
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
    
    <!-- Google Code for Remarketing Tag -->
    <!--------------------------------------------------
    Remarketing tags may not be associated with personally identifiable information or placed on pages related to sensitive categories. See more information and instructions on how to setup the tag on: http://google.com/ads/remarketingsetup
    --------------------------------------------------->
    <script type="text/javascript">
    /* <![CDATA[ */
    var google_conversion_id = 964139843;
    var google_custom_params = window.google_tag_params;
    var google_remarketing_only = true;
    /* ]]> */
    </script>
    <script type="text/javascript" src="http://www.googleadservices.com/pagead/conversion.js">
    </script>
    <noscript>
    <div style="display:inline;">
    <img height="1" width="1" style="border-style:none;" alt="" src="http://www.googleads.g.doubleclick.net/pagead/viewthroughconversion/964139843/?value=0&amp;guid=ON&amp;script=0"/>
    </div>
    </noscript>
</asp:Content>