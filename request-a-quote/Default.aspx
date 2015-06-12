<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/request-a-quote/" />
    
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
		
		.sticky-form {
			display:none;
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div class="body_wrapper">
<div class="body_wrapper2">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
	<div class="combi_form">
     <div class="row">
        <div>
        	<br /><br />
            <div class="row_padding">
            	<h1>Request a Quote</h1>
                <div id="contact_us">
                <div class="row_padding">
                  <div class="row">
                        <div class="form_label">First Name:&nbsp;&nbsp;</div>
                        <div class="form_fields">
                            <asp:TextBox runat="server" class="inputbox" name="first" ID="first" size="26"/>
                            <asp:RequiredFieldValidator ID="vfirst" runat="server" ValidationGroup="QuoteForm" ControlToValidate="first" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                        </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row"> 
                    <div class="form_label">Last Name:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="last" ID="last" size="26" />
                        <asp:RequiredFieldValidator ID="vlast" runat="server" ValidationGroup="QuoteForm" ControlToValidate="last" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">Email:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="email" ID="email" size="26" />
                        <asp:RequiredFieldValidator ID="vemail" runat="server" ValidationGroup="QuoteForm" ControlToValidate="email" Text="" CssClass="fieldReq" ForeColor="#e13433" /><asp:RegularExpressionValidator ID="vemail2" ForeColor="#e13433" CssClass="fieldReq"  runat="server" ControlToValidate="email" ValidationGroup="QuoteForm" ErrorMessage="" ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"></asp:RegularExpressionValidator>
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">Phone:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="phone" ID="phone"  type="text" maxlength="12"  />
                        <asp:RequiredFieldValidator ID="vphone" runat="server" ValidationGroup="QuoteForm" ControlToValidate="phone" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">Company:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="company" ID="company" type="text"  />
                        <asp:RequiredFieldValidator ID="vcompany" runat="server" ValidationGroup="QuoteForm" ControlToValidate="company" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">Zip Code:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="zipcode" ID="zipcode"  />
                        <asp:RequiredFieldValidator ID="vzipcode" runat="server" ValidationGroup="QuoteForm" ControlToValidate="zipcode" Text="" CssClass="fieldReq" ForeColor="#e13433" /><asp:RegularExpressionValidator ID="vzipcode2" ForeColor="#e13433" CssClass="fieldReq"  runat="server" ControlToValidate="zipcode" ValidationGroup="QuoteForm" ErrorMessage="" ValidationExpression="\d{5}-?(\d{4})?$"></asp:RegularExpressionValidator>
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">Product Category&nbsp;&nbsp;</div>
                    <div class="form_fields">
                    	<asp:DropDownList runat="server" id="category" name="category" class="inputSelect">
                            <asp:ListItem value="">--Product Category--</asp:ListItem>
                            <asp:ListItem value="Braising Pans">Braising Pans</asp:ListItem>
                            <asp:ListItem value="Charbroilers">Charbroilers</asp:ListItem>
                            <asp:ListItem value="Combi Ovens">Combi Ovens</asp:ListItem>
                            <asp:ListItem value="Convection Ovens">Convection Ovens</asp:ListItem>
                            <asp:ListItem value="Fryers">Fryers</asp:ListItem>
                            <asp:ListItem value="Griddles">Griddles</asp:ListItem>
                            <asp:ListItem value="Heated Holding">Heated Holding</asp:ListItem>
                            <asp:ListItem value="Kettles">Kettles</asp:ListItem>
                            <asp:ListItem value="Ranges">Ranges</asp:ListItem>
                            <asp:ListItem value="Steamers">Steamers</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">Product:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="product" ID="product"  />
                        <asp:RequiredFieldValidator ID="vproduct" runat="server" ValidationGroup="QuoteForm" ControlToValidate="product" Text="" CssClass="fieldReq" ForeColor="#e13433" />
                    </div>
                  </div>
                  <br style="clear:both;" />
                  <div class="row">
                    <div class="form_label">Comments:&nbsp;&nbsp;</div>
                    <div class="form_fields">
                        <asp:TextBox runat="server" class="inputbox" name="comment" ID="comment"  />
                    </div>
                  </div>
                  <br style="clear:both;" />
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
                  </div>
                  <br style="clear:both;" />
                  <br />
                  <div class="row">
                  		<div class="form_label">&nbsp;</div>
                        <div style="text-align: left; position: relative; left: -16px; display: table-cell; border:none;">
                        <asp:ImageButton ID="submit" runat="server" ImageUrl="~/images/request-quote-btn.jpg" ValidationGroup="QuoteForm" OnClick="Submit_Click" Width="196" Height="39" />
                        </div>
                  </div> 
                  <br style="clear:both;" />
                </div>
              </div>
            </div>
        </div>
    </div>
</div>
<div id="scrollToHere" style="width:100%; position:relative; top:-55px;"></div>
</ContentTemplate>
    <Triggers>
     
    </Triggers>
</asp:UpdatePanel>
</div>
</div>
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