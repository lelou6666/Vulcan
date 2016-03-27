<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE html>
<!--[if lt IE 7 ]> <html class="ie6"> <![endif]-->
<!--[if IE 7 ]>    <html class="ie7"> <![endif]-->
<!--[if IE 8 ]>    <html class="ie8"> <![endif]-->
<!--[if IE 9 ]>    <html class="ie9"> <![endif]-->
<!--[if (gt IE 9)|!(IE)]><!--> <html> <!--<![endif]-->
<head runat="server">
    <title>Combi Simulator | Vulcan Equipement</title>
    <meta name="description" content="Try our Combi Simulator to Win!" />
    <meta name="viewport" content="width=772, user-scalable=no" />
    
    <link rel="stylesheet" href="css/knobKnob.css" type="text/css" />
	<link rel="stylesheet" href="css/simulator.css" type="text/css" />
    
    <style type="text/css">
		#circularG{
		position:relative;
		width:128px;
		height:128px}
		
		.circularG{
		position:absolute;
		background-color:#5CB0EB;
		width:29px;
		height:29px;
		-moz-border-radius:19px;
		-moz-animation-name:bounce_circularG;
		-moz-animation-duration:1.12s;
		-moz-animation-iteration-count:infinite;
		-moz-animation-direction:linear;
		-webkit-border-radius:19px;
		-webkit-animation-name:bounce_circularG;
		-webkit-animation-duration:1.12s;
		-webkit-animation-iteration-count:infinite;
		-webkit-animation-direction:linear;
		-ms-border-radius:19px;
		-ms-animation-name:bounce_circularG;
		-ms-animation-duration:1.12s;
		-ms-animation-iteration-count:infinite;
		-ms-animation-direction:linear;
		-o-border-radius:19px;
		-o-animation-name:bounce_circularG;
		-o-animation-duration:1.12s;
		-o-animation-iteration-count:infinite;
		-o-animation-direction:linear;
		border-radius:19px;
		animation-name:bounce_circularG;
		animation-duration:1.12s;
		animation-iteration-count:infinite;
		animation-direction:linear;
		}
		
		#circularG_1{
		left:0;
		top:50px;
		-moz-animation-delay:0.42s;
		-webkit-animation-delay:0.42s;
		-ms-animation-delay:0.42s;
		-o-animation-delay:0.42s;
		animation-delay:0.42s;
		}
		
		#circularG_2{
		left:14px;
		top:14px;
		-moz-animation-delay:0.56s;
		-webkit-animation-delay:0.56s;
		-ms-animation-delay:0.56s;
		-o-animation-delay:0.56s;
		animation-delay:0.56s;
		}
		
		#circularG_3{
		top:0;
		left:50px;
		-moz-animation-delay:0.7s;
		-webkit-animation-delay:0.7s;
		-ms-animation-delay:0.7s;
		-o-animation-delay:0.7s;
		animation-delay:0.7s;
		}
		
		#circularG_4{
		right:14px;
		top:14px;
		-moz-animation-delay:0.84s;
		-webkit-animation-delay:0.84s;
		-ms-animation-delay:0.84s;
		-o-animation-delay:0.84s;
		animation-delay:0.84s;
		}
		
		#circularG_5{
		right:0;
		top:50px;
		-moz-animation-delay:0.98s;
		-webkit-animation-delay:0.98s;
		-ms-animation-delay:0.98s;
		-o-animation-delay:0.98s;
		animation-delay:0.98s;
		}
		
		#circularG_6{
		right:14px;
		bottom:14px;
		-moz-animation-delay:1.12s;
		-webkit-animation-delay:1.12s;
		-ms-animation-delay:1.12s;
		-o-animation-delay:1.12s;
		animation-delay:1.12s;
		}
		
		#circularG_7{
		left:50px;
		bottom:0;
		-moz-animation-delay:1.26s;
		-webkit-animation-delay:1.26s;
		-ms-animation-delay:1.26s;
		-o-animation-delay:1.26s;
		animation-delay:1.26s;
		}
		
		#circularG_8{
		left:14px;
		bottom:14px;
		-moz-animation-delay:1.4s;
		-webkit-animation-delay:1.4s;
		-ms-animation-delay:1.4s;
		-o-animation-delay:1.4s;
		animation-delay:1.4s;
		}
		
		@-moz-keyframes bounce_circularG{
		0%{
		-moz-transform:scale(1)}
		
		100%{
		-moz-transform:scale(.3)}
		
		}
		
		@-webkit-keyframes bounce_circularG{
		0%{
		-webkit-transform:scale(1)}
		
		100%{
		-webkit-transform:scale(.3)}
		
		}
		
		@-ms-keyframes bounce_circularG{
		0%{
		-ms-transform:scale(1)}
		
		100%{
		-ms-transform:scale(.3)}
		
		}
		
		@-o-keyframes bounce_circularG{
		0%{
		-o-transform:scale(1)}
		
		100%{
		-o-transform:scale(.3)}
		
		}
		
		@keyframes bounce_circularG{
		0%{
		transform:scale(1)}
		
		100%{
		transform:scale(.3)}
		
		}
		</style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="container">
		<div id="header">
			<a href="http://www.vulcanequipment.com"><div id="logo"></div></a>
			<!--<div id="callout">
				<img src="images/callout.png" width="205" height="51" border="0" />
			</div>-->
			<div class="clearer"></div>
		</div>
		<div id="door"></div>
		<div id="door_open"></div>
		<div id="door_button"></div>
		<div id="another_batch_popup"></div>
		<div id="controls">
			<div id="power_switch"></div>
			<div id="power_switch_popup"></div>
			<div id="temp_container">
				<div class="container">
					<div class="control"></div>
				</div>
				<input type="range" class="slider" min="0" max="370" value="0" step="5" />
				<div class="output"></div>
			</div>
			<div id="temp_popup"></div>
			<div id="time_container">
				<div class="container">
					<div class="control"></div>
				</div>
				<input type="range" class="slider" min="0" max="599" value="0" />
				<div class="output"></div>
			</div>
			<div id="time_popup"></div>
			<div id="humidity_container">
				<div class="container">
					<div class="control"></div>
				</div>
				<input type="range" class="slider" min="0" max="100" value="0" step="10" />
				<div class="output"></div>
			</div>
			<div id="humidity_popup"></div>
		</div>
		<div id="intro_1">
			<div class="content"></div>
		</div>
		<div id="intro_2">
			<div class="closed"></div>
			<div class="open"></div>
		</div>
		<div id="directions">
			<div class="power"></div>
			<div class="temp"></div>
			<div class="time"></div>
			<div class="humidity"></div>
			<div class="cta">
				<p><img src="images/cta.png" width="203" height="52" border="0" /></p>
				<!--<p><a href="official_rules.pdf" target="_blank">Giveaway rules</a></p>-->
			</div>
		</div>
		<div id="form" style="display:none;">
			<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
            	<div runat="server" id="combiForm" style="position:relative;">
                <!--<div style="position: absolute; top: -120px; left: 545px; cursor:pointer;"><a onClick="hideForm();"><img src="images/close.png" alt="close" /></a></div>-->		
                <asp:UpdateProgress id="updateProgress" runat="server">
                    <ProgressTemplate>
                        <div style="position:fixed; top:50%; left:50%; margin-top:-55px; margin-left:-80px; z-index:8000;">
                            <div id="circularG">
                                <div id="circularG_1" class="circularG">
                                </div>
                                <div id="circularG_2" class="circularG">
                                </div>
                                <div id="circularG_3" class="circularG">
                                </div>
                                <div id="circularG_4" class="circularG">
                                </div>
                                <div id="circularG_5" class="circularG">
                                </div>
                                <div id="circularG_6" class="circularG">
                                </div>
                                <div id="circularG_7" class="circularG">
                                </div>
                                <div id="circularG_8" class="circularG">
                                </div>
                            </div>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
				<table cellpadding="2" cellspacing="1">
                	<tr>
                    	<td style="padding-bottom:110px;" colspan="2"><a onClick="hideForm();"><img src="images/close.png" alt="close" style="position:relative; left:557px;" /></a></td>
                    </tr>
					<tr>
						<td style="font-size:21px; text-align:right;">First Name:</td>
						<td><asp:TextBox runat="server" class="inputbox" name="first" ID="first" size="26"/>
                <asp:RequiredFieldValidator ID="vfirst" runat="server" ValidationGroup="ContactUs" ControlToValidate="first" Text="" CssClass="fieldReq" ForeColor="#e13433"  /></td>
					</tr>
					<tr>
						<td style="font-size:21px; text-align:right;">Last Name:</td>
						<td><asp:TextBox runat="server" class="inputbox" name="last" ID="last" size="26" />
                <asp:RequiredFieldValidator ID="vlast" runat="server" ValidationGroup="ContactUs" ControlToValidate="last" Text="" CssClass="fieldReq" ForeColor="#e13433"  /></td>
					</tr>
					<tr>
						<td style="font-size:21px; text-align:right;">Email:</td>
						<td><asp:TextBox runat="server" class="inputbox" name="email" ID="email" size="26" />
                <asp:RequiredFieldValidator ID="vemail" runat="server" ValidationGroup="ContactUs" ControlToValidate="email" Text="" CssClass="fieldReq" ForeColor="#e13433" /><asp:RegularExpressionValidator ID="vemail2" ForeColor="#e13433" CssClass="fieldReq"  runat="server" ControlToValidate="email" ValidationGroup="ContactUs" ErrorMessage="" ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"></asp:RegularExpressionValidator></td>
					</tr>
					<tr>
						<td style="font-size:21px; text-align:right;">Phone:</td>
						<td><asp:TextBox runat="server" class="inputbox" name="phone" ID="phone"  type="text" maxlength="12"  />
                <asp:RequiredFieldValidator ID="vphone" runat="server" ValidationGroup="ContactUs" ControlToValidate="phone" Text="" CssClass="fieldReq" ForeColor="#e13433"  /></td>
					</tr>
					<tr>
						<td style="font-size:21px; text-align:right;">Title:</td>
						<td><asp:TextBox runat="server" class="inputbox" name="title" ID="title" size="26" /><asp:RequiredFieldValidator ID="vtitle" runat="server" ValidationGroup="ContactUs" ControlToValidate="title" Text="" CssClass="fieldReq" ForeColor="#e13433"  /></td>
					</tr>
					<tr>
						<td style="font-size:21px; text-align:right;">School/District:</td>
						<td><asp:TextBox runat="server" class="inputbox" name="district" ID="district" size="26" />
                <asp:RequiredFieldValidator ID="vdistrict" runat="server" ValidationGroup="ContactUs" ControlToValidate="district" Text="" CssClass="fieldReq" ForeColor="#e13433"  /></td>
					</tr>
                    <tr>
						<td style="font-size:21px; text-align:right;">City:</td>
						<td><asp:TextBox runat="server" class="inputbox" name="city" ID="city" size="26" />
                <asp:RequiredFieldValidator ID="vcity" runat="server" ValidationGroup="ContactUs" ControlToValidate="city" Text="" CssClass="fieldReq" ForeColor="#e13433"  /></td>
					</tr>
                    <tr>
						<td style="font-size:21px; text-align:right;">State:</td>
						<td><asp:TextBox runat="server" class="inputbox" name="state" ID="state" size="26" />
                <asp:RequiredFieldValidator ID="vstate" runat="server" ValidationGroup="ContactUs" ControlToValidate="state" Text="" CssClass="fieldReq" ForeColor="#e13433"  /></td>
					</tr>
                    <tr>
						<td style="font-size:21px; text-align:right;">Zip Code:</td>
						<td><asp:TextBox runat="server" class="inputbox" name="zipcode" ID="zipcode" size="26" />
                <asp:RequiredFieldValidator ID="vzipcode1" runat="server" ValidationGroup="ContactUs" ControlToValidate="zipcode" Text="" CssClass="fieldReq" ForeColor="#e13433" /><asp:RegularExpressionValidator ID="vzipcode2" ForeColor="#e13433" CssClass="fieldReq"  runat="server" ControlToValidate="zipcode" ValidationGroup="ContactUs" ErrorMessage="" ValidationExpression="\d{5}-?(\d{4})?$"></asp:RegularExpressionValidator></td>
					</tr>
                    <tr>
						<td style="font-size:21px; text-align:right;">Tell Us What<br />You Think:</td>
						<td><asp:TextBox runat="server" TextMode="MultiLine" rows="2" name="question" ID="question" cols="41"></asp:TextBox></td>
					</tr>
					<tr>
						<td colspan="2" style="padding-top:10px; padding-left:10px; padding-bottom:0px; font-size:16px; text-align:left;"><asp:Checkbox runat="server" id="cb1" Text="Please send me more information on the combi oven." CssClass="cb" /></td>
					</tr>
                    <tr>
						<td colspan="2" style="padding-left:10px; padding-bottom:0px; font-size:16px; text-align:left;"><asp:Checkbox runat="server" id="cb2" Text="I am interested in seeing a demo of the combi oven." CssClass="cb" /></td>
					</tr>
                    <tr>
						<td colspan="2" style="padding-left:10px; padding-bottom:20px; font-size:16px; text-align:left;"><asp:Checkbox runat="server" id="cb3" Text="Please have a sales rep contact me." CssClass="cb" /></td>
					</tr>
					<tr>
						<td>&nbsp;</td>
						<td style="text-align:right;"><asp:ImageButton ID="submit" runat="server" ImageUrl="images/cta_enter.png" ValidationGroup="ContactUs" OnClick="Submit_Click" Width="178" Height="52" /><br />
						<a href="official_rules.pdf" target="_blank">Giveaway rules</a></td>
					</tr>
				</table>
                </div>
                <div runat="server" id="thanks"  style="max-width: 545px;">
                	<p style="font-size:20px; padding-left:55px; color:#FFF; font-family:Arial, Helvetica, sans-serif; padding-top:120px;">Thank you for your entry and your interest in the<br />new Vulcan Combi.<br /><br /></p>
                    <div style="width:50%; float:left; text-align:center;"><a onClick="hideForm();" style="cursor:pointer;"><img src="images/try-again.jpg" border="0" /></a></div>
                    <div style="width:50%; float:left; text-align:center;"><a href="/video-library/?video=102043945&title=Vulcan%20Combi&player=9" style="cursor:pointer;"><img src="images/watch-video.jpg" border="0" /></a></div>
                    <div style="width:50%; float:left; text-align:center;"><a href="/products/combi/" style="cursor:pointer;"><img src="images/learn-more.jpg" border="0" /></a></div>
                    <div style="width:50%; float:left; text-align:center;"><a href="/products/" style="cursor:pointer;"><img src="images/learn-more2.jpg" border="0" /></a></div>
                </div>
                </ContentTemplate>
                    <Triggers>
                     
                    </Triggers>
                </asp:UpdatePanel>
            </div>
	</div>
    </form>
    
    
    <script src="http://code.jquery.com/jquery-1.11.0.min.js" type="text/javascript"></script>
	<script src="js/transform.js" type="text/javascript"></script>
	<script src="js/knobKnob.jquery.js" type="text/javascript"></script>
	<script src="js/jquery.animate-transform.js" type="text/javascript"></script>
	<script src="js/simulator.js" type="text/javascript"></script>
    
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
    
    <!--Google Analytics  -->
    <script type="text/javascript">
        var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
        document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
    </script>
    <script type="text/javascript">
        try {
            var pageTracker = _gat._getTracker("UA-516396-112");
            pageTracker._trackPageview();
        } catch (err) { }
    </script>
</body>
</html>