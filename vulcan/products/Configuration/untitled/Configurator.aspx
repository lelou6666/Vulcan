﻿<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Configurator.aspx.cs" Inherits="Configurator" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<style type="text/css">
		.product_image {
			float:left;
			width:100%; 
			max-width:324px;
			max-height:347px;
		}
		
		label {
			color: #d4d3d3;
			font-family: 'helvetica', Arial, Sans-Serif;
			font-size: 16px;
			line-height: 22px;
			padding-bottom: 14px;
		}
		
		input[type="radio"] {
			margin: 3px 3px 0px 5px;
		}
		
		.left_step {
			float:right;
			width:100%;
			max-width:131px;
			max-height:61px;
		}
		
		.right_step {
			width:100%;
			max-width:134px;
			max-height:61px;
		}
		
		.seletedOptions {
			text-align:center;
			padding-top:10px;
		}
		
		.seletedOptions  input[type=checkbox] {
			display:none;
		}
		
		.seletedOptions input[type="checkbox"] + label {
			background-image:url('images/green_check.png');
			background-repeat:no-repeat;
			padding-left:30px;
			padding-right:30px;
		}
		
		.twenty {
			width:20%;
			float:left; 
			position:relative;
		}
		
		.options {
			width:60%; 
			float:left; 
			position:relative;
		}
		
		.steps {
			position:relative; 
			top:-60px;
		}
		
		#counter {
			margin: 0 auto;
		}
		
		#counter table {
			margin: 0 auto;
		}
		
		#numbers {
			position:relative;
			margin: 0 auto;
		}
		
		@media all and (max-width:720px) 
		{
			.options {
				width:100%;
			}
			
			.steps {
				top:0px;
			}
			
			.steps .twenty {
				width:50%;
				padding-bottom:20px;
			}
			
			.steps .options {
				width:0%;
			}
			
			.left_step {
				float:left;
			}
			
			.right_step {
				float:right;
			}
			
			#counter {
				width:390px;
				overflow:hidden;
			}
		}
		
		@media all and (max-width:650px) 
		{
			.forty, .sixty {
				width:100%;
			}
		}
		
		@media all and (max-width:470px) 
		{
			h3{
				font-size:25px;
				line-height:27px;
			}
			
			#counter {
				width:260px;
				overflow:hidden;
			}
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div class="dots_background" id="k12_header">
     <div class="row" style="overflow:auto; position:relative;">
     	<div class="row_padding">
        	<br />
        	<h1>Endurance&trade; Range Customizer</h1>
        	<div class="forty">
                <div style="padding-right:20px;"><asp:Image ID="ProductImage" runat="server" CssClass="product_image" /></div>
            </div>
            <div class="sixty">
            	<h3>Build the product that best fits the needs of your operations.</h3>
            	<br />
            	<p><b><asp:Label ID="ProductName" runat="server"></asp:Label></b><br />
                   The Endurance&trade; Range is built with Vulcan's legendary toughness, precision and dependability and loaded with innovative features sure to make an impact on your kitchen.</p></p><br /><br />
            </div>
        </div>
     </div>
</div>
<div>
<div class="body_wrapper">
<div class="body_wrapper2">
    <div style="width:100%; margin:0 auto; background-image:url('images/steps_bg.jpg'); background-repeat:repeat-x; height:285px;">
        <div class="row">
            <div class="row_padding">
                <br /><br />
                <h3>Customize My Endurance&#0153 Range</h3>
                <br />
                <div id="counter">
                    <table cellpadding="0" cellspacing="0" id="numbers">
                        <tr>
                            <td style="max-width:130px;"><asp:Image ID="Arrow1" runat="server" ImageUrl="Images/step1_on.png" /></td>
                            <td style="max-width:130px;"><asp:Image ID="Arrow2" runat="server" ImageUrl="Images/step2.png" /></td>
                            <td style="max-width:130px;"><asp:Image ID="Arrow3" runat="server" ImageUrl="Images/step3.png" /></td>
                            <td style="max-width:130px;"><asp:Image ID="Arrow4" runat="server" ImageUrl="Images/step4.png" /></td>
                            <td style="max-width:130px;"><asp:Image ID="Arrow5" runat="server" ImageUrl="Images/step5.png"  /></td>
                        </tr>
                        <tr>
                            <td class="seletedOptions"><asp:CheckBox ID="cbOven" runat="server" /></td>
                            <td class="seletedOptions"><asp:CheckBox ID="cbGas" runat="server" /></td>
                            <td class="seletedOptions"><asp:CheckBox ID="cbBurner" runat="server" /></td>
                            <td class="seletedOptions" colspan="2" style="text-align:left;"><asp:CheckBox ID="cbSheet" runat="server" /></td>
                            <!--<td class="seletedOptions"><asp:CheckBox ID="cbGriddle" runat="server" /></td>-->
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
	<div class="row">
    	<div class="row_padding" style="overflow:auto;">
        	<br />
                <asp:Panel ID="Panel1" runat="server">
                    <div class="twenty">&nbsp;</div>
                    <div class="options">
                        <div style="width:90%; margin:0 auto; border-bottom:1px solid #757575;"><p style="font-size:30px; line-height:32px; color:#fefefe; font-family:'futura medium condensed';"><br />OVEN OPTIONS</p></div>
                        <br />
                        <div style="width:90%; margin:0 auto;">
                            <asp:RadioButtonList runat="server" ID="cblOven" RepeatDirection="Horizontal"></asp:RadioButtonList>
                            <div style="display:none;color:red;" id="Error1"><p>Please select an oven option.</p></div>
                            <asp:Label ID="lblovenEmpty" runat="server"></asp:Label><br />
                        </div>
                    </div>
                    <div class="twenty">&nbsp;</div>
                    <br style="clear:both;" />
                    <div class="steps">
                    	<div class="twenty">&nbsp;</div>
                        <div class="options">&nbsp;</div>
                        <div class="twenty">
                            <asp:ImageButton ID="Next1" runat="server" ImageUrl="Images/step2_right.png" onclick="Next1_Click" CssClass="right_step" />
                        </div>
                    </div>
                </asp:Panel>
                
                <asp:Panel ID="Panel2" runat="server">           	
                    <div class="twenty">&nbsp;</div>
                    <div class="options">
                        <div style="width:90%; margin:0 auto; border-bottom:1px solid #757575;"><p style="font-size:30px; line-height:32px; color:#fefefe; font-family:'futura medium condensed';"><br />GAS OPTIONS</p></div>
                        <br />
                        <div style="width:90%; margin:0 auto;">
                            <asp:RadioButtonList runat="server" ID="cblGas" RepeatDirection="Horizontal"></asp:RadioButtonList>
                            <div style="display:none;color:red;" id="Error2">Please select a gas option.</div>
                            <asp:Label ID="lblGasEmpty" runat="server"></asp:Label><br />
                        </div>
                    </div>
                    <div class="twenty">&nbsp;</div>
                    <br style="clear:both;" />
                    <div class="steps">
                        <div class="twenty">
                            <asp:ImageButton ID="Back2" runat="server" ImageUrl="Images/step1_left.png" onclick="Back2_Click"  CssClass="left_step" />
                        </div>
                        <div class="options">&nbsp;</div>
                        <div class="twenty">
                            <asp:ImageButton ID="Next2" runat="server" ImageUrl="Images/step3_right.png" onclick="Next2_Click" CssClass="right_step" />
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="Panel3" runat="server">
                    <div class="twenty">&nbsp;</div>
                    <div class="options">
                    	<div style="width:90%; margin:0 auto; border-bottom:1px solid #757575;"><p style="font-size:30px; line-height:32px; color:#fefefe; font-family:'futura medium condensed';"><br />NUMBER OF BURNERS</p></div>
                        <br />
                        <div style="width:90%; margin:0 auto;">
                        	<asp:RadioButtonList runat="server" ID="cblBurners" RepeatDirection="Horizontal"></asp:RadioButtonList>
                        <div style="display:none;color:red;" id="Error3">Please select the number of burners.</div>
                        <asp:Label ID="lblBurnerEmpty" runat="server"></asp:Label><br />
                        </div>
                    </div>
                    <div class="twenty">&nbsp;</div>
                    <br style="clear:both;" />
                    <div class="steps">
                        <div class="twenty">
                            <asp:ImageButton ID="Back3" runat="server" ImageUrl="Images/step2_left.png" onclick="Back3_Click"  CssClass="left_step" />
                        </div>
                        <div class="options">&nbsp;</div>
                        <div class="twenty">
                            <asp:ImageButton ID="Next3" runat="server" ImageUrl="Images/step4_right.png" onclick="Next3_Click" CssClass="right_step" />
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="Panel4" runat="server">
                	
                    <div class="twenty">&nbsp;</div>
                    <div class="options">
                    	<div style="width:90%; margin:0 auto; border-bottom:1px solid #757575;"><p style="font-size:30px; line-height:32px; color:#fefefe; font-family:'futura medium condensed';"><br />SHEET PAN ORIENTATION</p></div>
                        <br />
                        <div style="width:90%; margin:0 auto;">
                        	<asp:RadioButtonList runat="server" ID="cblSheet" RepeatDirection="Vertical"></asp:RadioButtonList>
                            <div style="display:none;color:red;" id="Error4">Please select the sheet pan orientation.</div>
                            <asp:Label ID="lblSheetEmpty" runat="server"></asp:Label><br />
                        </div>
                    </div>
                    <div class="twenty">&nbsp;</div>
                    <br style="clear:both;" />
                    <div class="steps">
                        <div class="twenty">
                            <asp:ImageButton ID="Back4" runat="server" ImageUrl="Images/step3_left.png" onclick="Back4_Click" CssClass="left_step" />
                        </div>
                        <div class="options">&nbsp;</div>
                        <div class="twenty">
                            <asp:ImageButton ID="Next4" runat="server" ImageUrl="Images/step5_right.png" onclick="Next4_Click" CssClass="right_step" />
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="Panel5" runat="server">
                	
                    <div class="twenty">&nbsp;</div>
                    <div class="options">
                    	<div style="width:90%; margin:0 auto; border-bottom:1px solid #757575;"><p style="font-size:30px; line-height:32px; color:#fefefe; font-family:'futura medium condensed';"><br />GRIDDLE/BROILER OPTIONS</p></div>
                        <br />
                        <div style="width:90%; margin:0 auto;">
                        	<asp:RadioButtonList runat="server" ID="cblGriddle" RepeatDirection="Vertical"></asp:RadioButtonList>
                            <div style="display:none;color:red;" id="Error5">Please select a griddle option.</div>
                            <asp:Label ID="lblGriddleEmpty" runat="server"></asp:Label><br />
                        </div>
                    </div>
                    <div class="twenty">&nbsp;</div>
                    <br style="clear:both;" />
                    <div class="steps">
                        <div class="twenty">
                            <asp:ImageButton ID="Back5" runat="server" ImageUrl="Images/step4_left.png" onclick="Back5_Click" CssClass="left_step" />
                        </div>
                        <div class="options">&nbsp;</div>
                        <div class="twenty">
                            <asp:ImageButton ID="Finish" runat="server" ImageUrl="Images/finish.png" onclick="Next5_Click" CssClass="right_step"  />
                        </div>
                    </div>
                </asp:Panel>
                <br /><br />
            </div>   
        </div>  
    </div>  
</div>  
</div>  
<script type="text/javascript" src="/js/jquery-1.10.2.js"></script>
<script type="text/javascript">
	if($(document).width() < 470) {
		var step3 = document.getElementById("<%= Arrow3.ClientID %>");

		if($(step3).attr("src") == "Images/step3_on.png") {
			$("#numbers").css("left", "-130px");
		}
		
		var step4 = document.getElementById("<%= Arrow4.ClientID %>");

		if($(step4).attr("src") == "Images/step4_on.png") {
			$("#numbers").css("left", "-260px");
		}
		
		var step5 = document.getElementById("<%= Arrow5.ClientID %>");
		if($(step5).attr("src") == "Images/step5_on.png") {
			$("#numbers").css("left", "-390px");
		}
	}
	else if($(document).width() < 720) {
		var step4 = document.getElementById("<%= Arrow4.ClientID %>");

		if($(step4).attr("src") == "Images/step4_on.png") {
			$("#numbers").css("left", "-130px");
		}
		
		var step5 = document.getElementById("<%= Arrow5.ClientID %>");
		if($(step5).attr("src") == "Images/step5_on.png") {
			$("#numbers").css("left", "-260px");
		}
	}
</script>
</asp:Content>