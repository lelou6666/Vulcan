<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    #contact_us input[type="checkbox"]
    {
        background-color:#a5a5a5;
        border: 1px solid #343434;
        width:auto;
        height:auto;
        line-height:auto;
        vertical-align:middle;
        color: #343434;
        font-family: 'helvetica', Arial, Sans-Serif;
        font-size:17px;
        padding:auto;
        margin:auto;
    }  
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div class="body_wrapper">
<div class="body_wrapper2">
 <div id="contact_us">
    <div class="row_padding">
    <br />
    <h1>Newsletter Signup</h1>
      <div class="row">
        <div class="twocolumn">
            <div class="form_label">First Name:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox runat="server" class="inputbox" name="first" ID="first" TabIndex="1" />
            </div>
        </div>
        <div class="twocolumn"> 
            <div class="form_label">Company:&nbsp;&nbsp;</div>
            <div class="form_fields"><asp:TextBox runat="server" class="inputbox" name="company" ID="company" TabIndex="4"  /></div>
        </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
        <div class="twocolumn">
            <div class="form_label">Last Name:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox runat="server" class="inputbox" name="last" ID="last" TabIndex="2"  />
            </div>
        </div>
        <div class="twocolumn">
            <div class="form_label">Title:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox runat="server" class="inputbox" name="title" ID="title"  type="text"  TabIndex="5"  />
            </div>
        </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
        <div class="twocolumn">
            <div class="form_label">Email:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox runat="server" class="inputbox" name="email" ID="email" TabIndex="3" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="ContactUs" ControlToValidate="email" CssClass="fieldReq" ForeColor="#e13433"  /><asp:RegularExpressionValidator ID="vemail2" CssClass="fieldReq" runat="server" ControlToValidate="email" ValidationGroup="ContactUs" ErrorMessage="" ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"></asp:RegularExpressionValidator>
            </div>
        </div>
        <div class="twocolumn">&nbsp;</div>
      </div>
      <br style="clear:both;" />
      <div class="row" id="checkboxes">
        <div class="twocolumn">
            <div class="form_label" style="vertical-align:top;">I'm interested in:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <p><b>Industry Segments</b><br />
                <asp:CheckBox runat="server" ID="Restaurants" Text="Restaurants" ForeColor="#b8b7b7" Font-Names="'helvetica', Arial, Sans-Serif" TabIndex="6" /><br />
                <asp:CheckBox runat="server" ID="k12" Text="K-12 Schools" ForeColor="#b8b7b7" Font-Names="'helvetica', Arial, Sans-Serif" TabIndex="7" /><br />
                <asp:CheckBox runat="server" ID="Correctional" Text="Correctional Institutions" ForeColor="#b8b7b7" Font-Names="'helvetica', Arial, Sans-Serif" TabIndex="8" /><br />
                <asp:CheckBox runat="server" ID="Healthcare" Text="Healthcare" ForeColor="#b8b7b7" Font-Names="'helvetica', Arial, Sans-Serif" TabIndex="9" />
                </p>
            </div>
        </div>
        <div class="twocolumn">
            <p id="prod_segments"><b>Product Segments</b><br />
                <asp:CheckBox runat="server" ID="Fryers" Text="Fryers" ForeColor="#b8b7b7" Font-Names="'helvetica', Arial, Sans-Serif" TabIndex="10" /><br />
                <asp:CheckBox runat="server" ID="Griddles" Text="Griddles & Charbroilers" ForeColor="#b8b7b7" Font-Names="'helvetica', Arial, Sans-Serif" TabIndex="11" /><br />
                <asp:CheckBox runat="server" ID="Heated" Text="Heated Holding" ForeColor="#b8b7b7" Font-Names="'helvetica', Arial, Sans-Serif" TabIndex="12" /><br />
                <asp:CheckBox runat="server" ID="Ovens" Text="Ovens" ForeColor="#b8b7b7" Font-Names="'helvetica', Arial, Sans-Serif" TabIndex="13" /><br />
                <asp:CheckBox runat="server" ID="Ranges" Text="Ranges" ForeColor="#b8b7b7" Font-Names="'helvetica', Arial, Sans-Serif" TabIndex="14" /><br />
                <asp:CheckBox runat="server" ID="Steam" Text="Steam" ForeColor="#b8b7b7" Font-Names="'helvetica', Arial, Sans-Serif" TabIndex="15" />
            </p>
        </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
        <div class="twocolumn">
            <div class="form_label">Type the letters&nbsp;&nbsp;<br />you see below:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox ID="fields_CAPTCHA" runat="server" class="defaultText" TabIndex="16" />
                <asp:RequiredFieldValidator ID="vCaptcha" runat="server" ValidationGroup="ContactUs" ControlToValidate="fields_CAPTCHA" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
            </div>
        </div>
        <div class="twocolumn">&nbsp;</div>
      </div>
      <br style="clear:both;" />
      <div class="row">
            <div class="form_label">&nbsp;</div>
            <div class="form_fields"><img id="CAPTCHA" runat="server" title="CAPTCHA Image" alt="" src="/CAPTCHAimage/CAPTCHAimage.aspx" border="0" /></div>
      </div>
      <br style="clear:both;" />
      <div class="row">
        <div class="twocolumn">
            <div id="contactus_go"><asp:ImageButton ID="submit" runat="server" ImageUrl="~/images/go.jpg" ValidationGroup="ContactUs" OnClick="Submit_Click" Width="97" Height="48" /></div>
        </div>
        <div class="twocolumn">&nbsp;</div>
      </div> 
      <br style="clear:both;" />
    </div>
</div>
</div>
</asp:Content>

<asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
    <script type="text/javascript">
        var prevControl;
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

        // Checkbox CSS
        $(function() {
            $('#checkboxes').jqTransform({ imgPath: '/js/jqtransformplugin/img/' });
        });
    </script>
</asp:Content>