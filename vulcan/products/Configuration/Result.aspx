<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Result.aspx.cs" Inherits="Result" MaintainScrollPositionOnPostback="true" Debug="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="stylesheet" href="css/threeSixty.css" type="text/css" media="screen" />

	<style type="text/css">
		.model_number {
			font-size:18px;
			color:#848484;
			font-family:"futura light";
			width:100%;
			max-width:350px;
			background-color:#131313;
			border:2px solid #535353;
			padding:15px;
		}
		
		.modelExp {
			overflow:auto;
			padding-left:15px;
			color:#cecdcd;
			font-family:"futura medium";
		}
		
		.rc_model {
			font-size:18px;
			color:#f1f1f1;
			font-family:"futura medium";
		}
		
		.summary_item {
			font-family:"futura medium condensed";
			font-size:24px;
			color:#fefefe;
			background-image:url('images/option_bg.png');
			background-repeat:no-repeat;
			width:244px;
			height:54px;
			padding-left:15px;
		}
		
		.specs {
			background-image: url('/images/spec_arrow.png');
			color:#f1f1f1;
		}
		
		#summary_contact {
			padding-top:15px;
		}
		#summary_contact_title {
			position:absolute;
			width:425px;
			padding:15px 0;
			font-family:Verdana, Geneva, sans-serif;
			font-size:13px;
			font-weight:bold;
			color:#FFFFFF;
			text-align:center;
		}
		#summary_contact_content {
			position:absolute;
			top:48px;
			width:425px;
			padding:15px 0;
			font-family:Verdana, Geneva, sans-serif;
			font-size:13px;
			color:#2C2B2B;
			text-align:center;
		}
		#ctl00_ContentPlaceHolder1_Panel1 {
			position:relative;
			left:419px;
			width:242px;
			border:1px solid #CCCCCC;
			padding:0 15px 10px 15px;
			margin-top:5px;
		}
		
		.option {
			color:#fefefe;
			text-transform:uppercase;
			font-size:18px;
			font-family:"futura light";
		}
		
		#email_me {
			display:none;
		}
		
		#contact_us input {
			width:150px;
		}
		
		#model_options_lb {
			width:100%; 
			text-align:center; 
			border-bottom:1px solid #535353; 
			font-size:28px; 
			color:#FFF; 
			font-family:'futura light condensed'; 
			margin-bottom:5px;
		}
		
		.tabs a {
			font-family: 'futura medium condensed', Arial, Sans-Serif;
			font-size: 26px;
			color: #fdfdfd;
			text-align: center;
			border: 2px solid #414141;
			background: #171717;
			text-decoration: none;
			padding: 8px 15px;
		}
		
		.tabs a.active {
			background: #891123;
		}
		
		.tabs img {
			display: none;
		}
		
		.tabs a.active img {
			display: inline;
			border: none;
		}
		
		.tab_arrow1 {
			position: absolute;
			top: 46px;
			left: 95px;
		}
		
		.tab_arrow2 {
			position: absolute;
			top: 46px;
			left: 295px;
		}
		
		.salamander-option {
			display:none;
		}
		
		@media all and (max-width:1000px) 
		{
			.twocolumn {
				width:100%;
			}
		}
		
		@media print 
		{
			#header_background, #footer_squares, .container-fluid, .btn, #summary_contact, #disclaimer {
				display:none;
				height:0px;
			}
			
			body {
				background-color:transparent;
			}
			
			.dots_background_top, .summary_item {
				background-image:none;
				background-color:transparent;
			}
			
			.model_number, .rc_model { 
				background-color:transparent;
				border:none;
				color:#000;
			}
			
			.body-top-padding {
				top:0px;
			}
			
			*, h1, h2, h3, p, td, .modelExp, .specs, #model_options_lb, .option, .summary_item {
				color:#000;
			}
			
			#model_options_lb {
				text-align:left;
			}
			
			h3 {
				font-size: 28px;
			}
		}
		
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">      
    <div class="dots_background_top">
     <div class="row" style="position:relative;">
     	<div class="row_padding">
        	<br />
        	<h1 style="padding-bottom:10px;">Endurance&trade; Range Customizer</h1>
            <p>Below is the Vulcan Endurance&trade; Range perfect for your operation.</p>
        </div>
     </div>        
      <br />
     <div class="row" style="position:relative;">
     	<div class="row_padding">
        	<div class="twocolumn">    
                <div class="tabs">
                	<div style="float:left;">
                        <h3><a onclick="showMain();" class="active" style="width:210px; cursor:pointer;" id="main-link">
                            <asp:Label ID="ProductName" runat="server"></asp:Label>
                            <img src="/images/current_tab_arrow.png" class="tab_arrow1">
                        </a></h3>
                    </div>
                    <div runat="server" id="salemander_tab" style="float:left;">
                        <h3 style="padding-left:5px;"><a style="width:210px; cursor:pointer;" onclick="showSalamander();" id="salamander-link">
                            <asp:Label ID="SalamanderName" runat="server"></asp:Label>
                            <img src="/images/current_tab_arrow.png" class="tab_arrow2">
                        </a></h3>
                    </div>
                </div>
                <br style="clear:both;" /><br /><br />
                <div class="main-model">
                    <div class="model_number">
                        <div id="model_options_lb">MODEL OPTIONS</div>
                        <asp:Label ID="ProductModel" runat="server"></asp:Label>
                    </div>
                    <br />
                    <section class="container-fluid" id="container">
                        <div class="threesixty product1">
                            <div class="spinner">
                                <span>0%</span>
                            </div>
                            <ol class="threesixty_images"></ol>
                        </div>
                        <div style="position:relative; top:-60px; display:none;" id="spin_icon"><img src="Images/360_icon.png" border="0" /></div>
                    </section>
                    <div id="static_image"><asp:Image ID="ProductImage" runat="server" /></div><br /><br />
                    <div style="width:100%; position:relative; top:-40px;" class="btn"><a href="/products/ranges/?series=371" onClick="ga('send', 'event', 'Range Configurator', 'View Product Details');"><img src="Images/view_product_details.png" border="0" /></a></div>
                </div>
                <div class="salamander-option">
                	<div class="model_number">
                        <div id="model_options_lb">MODEL OPTIONS</div>
                        <asp:Label ID="SalamanderModel" runat="server"></asp:Label>
                    </div>
                    <br />
                    <div id="static_image"><asp:Image ID="SalamanderImage" runat="server" /></div><br /><br /><br />
                    <div style="width:100%; position:relative; top:-40px;" class="btn"><asp:Literal runat="server" id="view_prod" /></div>
                </div>
            </div>
        
            <div class="twocolumn">  
            	<table width="100%">
                    <tr>
                        <td>
                            <h3>Configuration Summary</h3>
                            <br />
                            <table>
                                <tr>
                                    <td class="summary_item">OVEN OPTIONS</td>
                                    <td><img src="Images/green_check.png" border="0" /> <asp:Label ID="Oven" CssClass="option" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="summary_item">GAS OPTIONS</td>
                                    <td><img src="Images/green_check.png" border="0" /> <asp:Label ID="Gas" CssClass="option" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="summary_item">NUMBER OF BURNERS</td>
                                    <td><img src="Images/green_check.png" border="0" /> <asp:Label ID="Burner" CssClass="option" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="summary_item">SHEET PAN ORIENTATION</td>
                                    <td><img src="Images/green_check.png" border="0" /> <asp:Label ID="Sheet" CssClass="option" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="summary_item">GRIDDLE/BROILER OPTIONS</td>
                                    <td><img src="Images/green_check.png" border="0" /> <asp:Label ID="Griddle" CssClass="option" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="summary_item">SALAMANDER OPTIONS</td>
                                    <td><img src="Images/green_check.png" border="0" /> <asp:Label ID="Salamander" CssClass="option" runat="server"></asp:Label></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table> 
                <br /><br />
                <div class="btn"><img src="Images/print.jpg" border="0" onclick="window.print(); ga('send', 'event', 'Range Configurator', 'Print Results');" style="cursor:pointer;" /> &nbsp; <img src="Images/email_results.png" border="0" onclick="$('#email_me').slideToggle(); ga('send', 'event', 'Range Configurator', 'Email Results');" style="cursor:pointer;" /></div>
                <div id="email_me">
                    <table id="contact_us">
                        <tr>
                            <td style="text-align:right;" class="form_label">From Address:</td>
                            <td class="form_fields"><asp:TextBox ID="txtFromAddress" runat="server" class="inputbox"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="reqFromAddress" runat="server" ControlToValidate="txtFromAddress" ErrorMessage=" !" CssClass="fieldReq" ForeColor="#e13433" ValidationGroup="mail"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="regFromAddress" runat="server" ControlToValidate="txtFromAddress" ErrorMessage=" !" CssClass="fieldReq" ForeColor="#e13433"  ValidationGroup="mail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:right;" class="form_label">To Address:</td>
                            <td class="form_fields"><asp:TextBox ID="txtToAddress" runat="server" class="inputbox"></asp:TextBox><asp:RequiredFieldValidator ID="reqToAddress" runat="server" ControlToValidate="txtToAddress" ErrorMessage=" !" CssClass="fieldReq" ForeColor="#e13433" ValidationGroup="mail"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="regToAddress" runat="server" ControlToValidate="txtToAddress" ErrorMessage=" !" CssClass="fieldReq" ForeColor="#e13433"  ValidationGroup="mail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td id="contactus_go" align="left" style="text-align:left;"><asp:ImageButton ID="btnSendMail" runat="server" ImageUrl="~/images/go.jpg" Width="97" Height="48" ValidationGroup="mail" onclick="btnSendMail_Click" /></td>
                        </tr>
                    </table>
                </div>
                <div id="summary_contact">	
                    <img src="Images/contact_bar.png" border="0" style="width:100%; max-width:470px; max-height:48px;" />
                    <p style="padding-left:15px;">PHONE: 866.726.4315<br />
                    </p><br /><br />
                <p><a href="/Vulcan/Products/Configuration/" onClick="ga('send', 'event', 'Range Configurator', 'Configure Another Range');"><img src="Images/another_range.png" border="0" /></a></p><br /><br /> </div>
            </div>
        </div>
    </div>
    <br />
  	<br />
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageScripts" Runat="Server">
	<script src="js/threesixty.js" type="text/javascript"></script>
    
	<script type="text/javascript">
		$(document).ready(function() {
			var myImage = document.getElementById("<%= ProductImage.ClientID %>");
			var imgSrc = $(myImage).attr("src");
			
			if(imgSrc.indexOf("##") == -1)
			{
				$("#container").hide();
			}
			else
			{
				$("#static_image").hide();
				
				imgSrc =  "imgs/" + imgSrc;
	
				var images = new Array();
				
				for (var i=1;i<10;i++)
				{ 
					images.push(imgSrc.replace("##","0" + i));
				}
				
				images.push(imgSrc.replace("##","10"));
				
				
				
				function init(){
			
					var imgArray = [];
					for (var i = 0; i < images.length; i++) {
						imgArray.push(images[i]);
					}
			
					product1 = $('.product1').ThreeSixty({
						totalFrames: imgArray.length,
						endFrame: 10,
						currentFrame: 1,
						totalFrames: 10,
						speedMultiplier: 0.35,
						pointerDistance: 50,
						imgList: '.threesixty_images',
						progress: '.spinner',
						filePrefix: '',
						height: 440,
						width: 300,
						navigation: false,
						disableSpin: true,
						imgArray: imgArray
					});
				}
				
				init();
				
				$('.product1').click(function() {
					$('#spin_icon').fadeOut();
				});
			}
		});
		
		function updateContainerSize()
		{
			$('.product1').css("height",$('.current-image').height());
		}
		
		function showMain() {
			$("#main-link").addClass("active");
			$("#salamander-link").removeClass("active");
			$(".salamander-option").hide();
			$(".main-model").show();
		}
		
		function showSalamander() {
			$(".main-model").hide();
			$(".salamander-option").show();
			$("#salamander-link").addClass("active");
			$("#main-link").removeClass("active");
		}
	</script>
   
</asp:Content>