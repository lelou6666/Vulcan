<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<style type="text/css">
		.body_wrapper li {
			padding-left: 15px;
			background-image: url('/images/spec_arrow.png');
			background-repeat: no-repeat;
		}
		
		@media all and (max-width:715px) 
		{
			.twocolumn {width:100%;}
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div class="dots_background" id="k12_header">
     <div class="row" style="overflow:auto; position:relative;">
     	<div class="row_padding" style="overflow:auto;">
        	<br />
        	<h1>Endurance&trade; Range Customizer</h1>
        	<div class="twocolumn">
            	<img src="Images/range_intro_new.png" style="width:100%; max-width:467px; max-height:340px;" alt="" />
            </div>
            <div class="twocolumn">
            	<h3>Build the product that best fits the needs of your operations.</h3>
            	<br />
            	<p>Welcome to the Vulcan Range Configurator, a tool to help you make the perfect selection for your operation from the 100+ available configurations of Vulcan Ranges. Whether you know which exact model you're looking for, or if you want help narrowing down your selection, we'll help you get it right.</p>
            	<p>Get started below, with help from us, or on your own. Your fully customized Vulcan Range will be delivered after answering a few short questions. Plus, your Vulcan Range model is:</p>
                <br /> <br />
            </div>
        </div>
     </div>
</div>
<div class="body_wrapper">
<div class="body_wrapper2">
	<div style="margin:0 auto; text-align:center; width:100%; position:relative; border-top: 2px solid #353535;"><img src="/images/gray_arrow.png" style="position:relative; top:-4px; margin:0 auto; text-align:center;" /></div>
    <br style="clear:both;" />
    <div class="row">
    	<div style="overflow:auto; padding:15px;">
            <div style="margin-top:15px;">
            	<div class="twocolumn">
                	<div style="margin: 0px 30px;">
                    	<h3 style="padding-bottom:10px;">Vulcan Endurance&trade; Range</h3>
                        <ul style="list-style:none;">
                            <li>Available now</li>
                            <li>Energy-saving&mdash;up to $600 in fuel cost, depending on range model</li>
                            <li>Packed with an extra oven rack at no charge</li>
                            <li>Complete with a 1-year parts and labor warranty</li>
                            <li>Further customizable with multiple front ledge options, like towel bars, cutting boards, condiment rails or fryer shields</li>
                        </ul>
                        <br />
                        <br />
                    </div>
        		</div>
                <div class="twocolumn">
                	<div style="margin: 0px 30px;">
                        <p><a href="ProductList.aspx" onClick="ga('send', 'event', 'Range Configurator', 'I know what I need');" style="color: #FFFFFF;font-size: 15px;font-family: 'futura light', Arial, sans-serif;text-decoration: none;"><img src="images/i_know.jpg" style="width:100%; max-width:248px; max-height:48px;" alt="I know what I need" /></a>
                        <br /><br />
                        If you have an idea of what model you need, start here and get right to customizing.
                        </p>
                        <br />
                        <p><a href="Questionnaire.aspx" onClick="ga('send', 'event', 'Range Configurator', 'Help me determine what I need');" style="color: #FFFFFF;font-size: 15px;font-family: 'futura light', Arial, sans-serif;text-decoration: none;"><img src="images/help_me.jpg" style="width:100%; max-width:341px; max-height:48px;" alt="Help me determine what I need" /></a>
                        <br /><br />If you need help determining which model to customize (based on size, burners, cooking volume), start here.
                        </p>
                        <br />
                        <br />
                    </div>
        		</div>
            </div>
            <br />
            <br />
        </div>
	</div>
</div>
</div>
</asp:Content>