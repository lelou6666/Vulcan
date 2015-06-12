<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/products/combi/like-what-you-see/" />
    
    <style type="text/css">
		.checkbox
		{
			font-family: 'helvetica', Arial, Sans-Serif;
			font-size: 18px;
			height:5px;
			color: #b8b7b7;
		}
		
		#contact_us input[type=checkbox] {
			width: 16px;
			margin-right: 10px;
			margin-left: 35px;
			background-color:transparent;
			border:none;
		}
	
		@media all and (max-width:900px) 
		{
			#contactus_header  {
				background-image:none;
			}
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
<div id="contactus_header" class="combi_form">
     <div class="row">
        <div class="forty">
            <div class="row_padding">
            	<br />
            	<h1 style="padding-bottom:10px;">Like what you see?</h1>
                <p style="font-size:18px;">It's easy to find out more – just as easy as it is to use our Combi.</p>
                <img src="imgs/combi.png" border="0" alt="Combi" style="width:100%; max-width:369px; max-height:504px;" />
            </div>
        </div>
        <div class="sixty">
        	<br /><br />
            <div class="row_padding">
                <div id="contact_us">
                <div class="row_padding">
                  <div class="row">
                        <div class="form_label">First Name:&nbsp;&nbsp;</div>
                        <div class="form_fields">
                            <asp:TextBox runat="server" class="inputbox" name="first" ID="first" size="26"/>
                            <asp:RequiredFieldValidator ID="vfirst" runat="server" ValidationGroup="CombiForm" ControlToValidate="first" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                        </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row"> 
                    <div class="form_label">Last Name:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="last" ID="last" size="26" />
                        <asp:RequiredFieldValidator ID="vlast" runat="server" ValidationGroup="CombiForm" ControlToValidate="last" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">Email:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="email" ID="email" size="26" />
                        <asp:RequiredFieldValidator ID="vemail" runat="server" ValidationGroup="CombiForm" ControlToValidate="email" Text="" CssClass="fieldReq" ForeColor="#e13433" /><asp:RegularExpressionValidator ID="vemail2" ForeColor="#e13433" CssClass="fieldReq"  runat="server" ControlToValidate="email" ValidationGroup="CombiForm" ErrorMessage="" ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"></asp:RegularExpressionValidator>
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">Phone:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="phone" ID="phone"  type="text" maxlength="12"  />
                        <asp:RequiredFieldValidator ID="vphone" runat="server" ValidationGroup="CombiForm" ControlToValidate="phone" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">Business Name:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="school" ID="school" type="text"  />
                        <asp:RequiredFieldValidator ID="vschool" runat="server" ValidationGroup="CombiForm" ControlToValidate="school" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">City:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="city" ID="city" type="text"  />
                        <asp:RequiredFieldValidator ID="vcity" runat="server" ValidationGroup="CombiForm" ControlToValidate="city" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">State:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="state" ID="state" type="text"  />
                        <asp:RequiredFieldValidator ID="vstate" runat="server" ValidationGroup="CombiForm" ControlToValidate="state" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">Zip Code:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="zipcode" ID="zipcode"  />
                        <asp:RequiredFieldValidator ID="vzipcode" runat="server" ValidationGroup="CombiForm" ControlToValidate="zipcode" Text="" CssClass="fieldReq" ForeColor="#e13433" /><asp:RegularExpressionValidator ID="vzipcode2" ForeColor="#e13433" CssClass="fieldReq"  runat="server" ControlToValidate="zipcode" ValidationGroup="CombiForm" ErrorMessage="" ValidationExpression="\d{5}-?(\d{4})?$"></asp:RegularExpressionValidator>
                    </div>
                  </div>
                  <!--<br style="clear:both;" />
                  <div class="row">
                        <div class="form_label">Type the letters&nbsp;&nbsp;<br />you see below:&nbsp;&nbsp;</div>
                        <div class="form_fields">
                            <asp:TextBox ID="fields_CAPTCHA" runat="server" class="defaultText" />
                            
                        </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                        <div class="form_label">&nbsp;</div>
                        <div class="form_fields"><img id="CAPTCHA" runat="server" title="CAPTCHA Image" alt="" src="/CAPTCHAimage/CAPTCHAimage.aspx" border="0" /></div>
                  </div>-->
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="checkbox">
                        <asp:Checkbox runat="server" id="cb1" Text="Please send me more information on the combi oven." />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="checkbox">
                        <asp:Checkbox runat="server" id="cb2" Text="I am interested in seeing a demo of the combi oven." />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="checkbox">
                        <asp:Checkbox runat="server" id="cb3" Text="Please have a sales rep contact me." />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <br />
                  <div class="row">
                  		<div class="form_label">&nbsp;</div>
                        <div style="text-align: left; position: relative; left: -16px; display: table-cell; border:none;"><asp:ImageButton ID="submit" runat="server" ImageUrl="imgs/submit.png" ValidationGroup="CombiForm" OnClick="Submit_Click" Width="143" Height="66" /></div>
                  </div> 
                  <br style="clear:both;" />
                </div>
              </div>
            </div>
        </div>
    </div>
</div>
<div id="scrollToHere" style="width:100%; position:relative; top:-55px;"></div>
<div class="body_wrapper">
<div class="body_wrapper2">
<div style="margin:0 auto; text-align:center; width:100%; position:relative; border-top: 2px solid #353535;"><img src="/images/black_arrow.png" style="position:relative; top:-4px; margin:0 auto; text-align:center;" /></div>
    <br /><br />
    <div class="banner">
        <a href="/k-12/"><img src="/images/products/banners/k12.png" border="0" /></a>
    </div>
    <br style="clear:both;" />
    <br />
</div>
</div>
</ContentTemplate>
    <Triggers>
     
    </Triggers>
</asp:UpdatePanel>
</asp:Content>  
<asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
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
</asp:Content>