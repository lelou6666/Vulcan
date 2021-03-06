﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/contact-us/" />
    <style type="text/css">
        .responsive-container { position: relative; padding-bottom: 30px; padding-top:10px; width:100%; height: auto; overflow: hidden; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <div id="contactus_header">
        <div class="row">
            <div class="twocolumn contact_info">
                <div class="row_padding">
                    <br />
					<CMS:ContentBlock ID="ContentBlock1" runat="server" DefaultContentID="1533" /> 
                </div>
           </div>
           <div class="twocolumn contact_info" id="contact_info2">
                <div class="row_padding">
                    <br />
					<CMS:ContentBlock ID="ContentBlock3" runat="server" DefaultContentID="1534" /> 
                </div>
           </div>
        </div>
        <!-- This contains the hidden content for videos -->
        <div id="map_vulcan" style='display:none; width:100%; margin: 0 auto; background-color: #222222;'>
            <br />
            <div class="video_heading1" style="margin-left:10px; margin-top:10px;">3600 North Point Blvd. Baltimore, MD 21222
		        <div style="position:relative; float:right;"><a href="javascript:hideMap('#map_vulcan');"><img src="/images/close.jpg" border="0" style="margin-right:10px;" /></a></div>
		    </div>
  
            <div class="responsive-container">
               <div id="map_canvas" style="width:100%; height:350px;"></div>
            </div>
            <br />
        </div>
    </div>
    <div class="body_wrapper">
    <div class="body_wrapper2">
    <div style="margin:0 auto; text-align:center; width:100%; position:relative; border-top: 2px solid #353535;"><img src="/images/black_arrow.png" style="position:relative; top:-4px; margin:0 auto; text-align:center;" /></div>
         
    <br style="clear:both;" />
    <br />
    
     <div class="row">
        <div class="row_padding">
           <CMS:ContentBlock ID="ContentBlock2" runat="server" DefaultContentID="1227" /> 
        </div>
    </div>
    
    <br style="clear:both;" />
    <br />
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    
    <div id="contact_us">
    <div class="row_padding">
      <div class="row">
        <div class="twocolumn">
            <div class="form_label">First Name:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox runat="server" class="inputbox" name="first" ID="first" size="26"/>
                <asp:RequiredFieldValidator ID="vfirst" runat="server" ValidationGroup="ContactUs" ControlToValidate="first" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
            </div>
        </div>
        <div class="twocolumn"> 
            <div class="form_label">Last Name:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox runat="server" class="inputbox" name="last" ID="last" size="26" />
                <asp:RequiredFieldValidator ID="vlast" runat="server" ValidationGroup="ContactUs" ControlToValidate="last" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
            </div>
        </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
        <div class="twocolumn">
            <div class="form_label">Email:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox runat="server" class="inputbox" name="email" ID="email" size="26" />
                <asp:RequiredFieldValidator ID="vemail" runat="server" ValidationGroup="ContactUs" ControlToValidate="email" Text="" CssClass="fieldReq" ForeColor="#e13433" /><asp:RegularExpressionValidator ID="vemail2" ForeColor="#e13433" CssClass="fieldReq"  runat="server" ControlToValidate="email" ValidationGroup="ContactUs" ErrorMessage="" ValidationExpression="^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$"></asp:RegularExpressionValidator>
            </div>
        </div>
        <div class="twocolumn">
            <div class="form_label">Phone:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox runat="server" class="inputbox" name="phone" ID="phone"  type="text" maxlength="12"  />
                <asp:RequiredFieldValidator ID="vphone" runat="server" ValidationGroup="ContactUs" ControlToValidate="phone" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
            </div>
        </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
        <div class="twocolumn">
            <div class="form_label">Title:&nbsp;&nbsp;</div>
            <div class="form_fields"><asp:TextBox runat="server" class="inputbox" name="title" ID="title" size="26" /></div>
        </div>
        <div class="twocolumn">
            <div class="form_label">Company/&nbsp;&nbsp;<br />Organization:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox runat="server" class="inputbox" name="company" ID="company" size="26" />
                <asp:RequiredFieldValidator ID="vcompany" runat="server" ValidationGroup="ContactUs" ControlToValidate="company" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
            </div>
        </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
        <div class="twocolumn">
            <div class="form_label">Company/&nbsp;&nbsp;<br />Organization Type:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:DropDownList runat="server" name="company_type" ID="company_type">
                    <asp:ListItem value="Healthcare">Healthcare</asp:ListItem>
                    <asp:ListItem value="Consultant">Consultant</asp:ListItem>
                    <asp:ListItem value="K12">K-12</asp:ListItem>
                    <asp:ListItem value="College and University">College & University</asp:ListItem>
                    <asp:ListItem value="Business and Industry">Business & Industry</asp:ListItem>
                    <asp:ListItem value="Restaurants">Restaurants</asp:ListItem>
                    <asp:ListItem value="Hotels and Casinos">Hotels & Casinos</asp:ListItem>
                    <asp:ListItem value="Other">Other</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="vcompany_type" runat="server" ValidationGroup="ContactUs" ControlToValidate="company_type" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
            </div>
        </div>
        <div class="twocolumn">
            <div class="form_label">Address:&nbsp;&nbsp;</div>
            <div class="form_fields"><asp:TextBox runat="server" class="inputbox" name="address" ID="address" size="26" /></div>
        </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
        <div class="twocolumn">
            <div class="form_label">Address 2:&nbsp;&nbsp;</div>
            <div class="form_fields"><asp:TextBox runat="server" class="inputbox"  name="address2" ID="address2" size="26" /></div>
        </div>
        <div class="twocolumn">
            <div class="form_label">Country:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:DropDownList runat="server" name="country" ID="country">
                    <asp:ListItem value="US">United States</asp:ListItem>
                    <asp:ListItem value="AF">Afghanistan</asp:ListItem>
                    <asp:ListItem value="AL">Albania</asp:ListItem>
                    <asp:ListItem value="DZ">Algeria</asp:ListItem>
                    <asp:ListItem value="AS">American Samoa</asp:ListItem>
                    <asp:ListItem value="AD">Andorra</asp:ListItem>
                    <asp:ListItem value="AO">Angola</asp:ListItem>
                    <asp:ListItem value="AI">Anguilla</asp:ListItem>
                    <asp:ListItem value="AQ">Antarctica</asp:ListItem>
                    <asp:ListItem value="AG">Antigua And Barbuda</asp:ListItem>
                    <asp:ListItem value="AR">Argentina</asp:ListItem>
                    <asp:ListItem value="AM">Armenia</asp:ListItem>
                    <asp:ListItem value="AW">Aruba</asp:ListItem>
                    <asp:ListItem value="AU">Australia</asp:ListItem>
                    <asp:ListItem value="AT">Austria</asp:ListItem>
                    <asp:ListItem value="AZ">Azerbaijan</asp:ListItem>
                    <asp:ListItem value="BS">Bahamas</asp:ListItem>
                    <asp:ListItem value="BH">Bahrain</asp:ListItem>
                    <asp:ListItem value="BD">Bangladesh</asp:ListItem>
                    <asp:ListItem value="BB">Barbados</asp:ListItem>
                    <asp:ListItem value="BY">Belarus</asp:ListItem>
                    <asp:ListItem value="BE">Belgium</asp:ListItem>
                    <asp:ListItem value="BZ">Belize</asp:ListItem>
                    <asp:ListItem value="BJ">Benin</asp:ListItem>
                    <asp:ListItem value="BM">Bermuda</asp:ListItem>
                    <asp:ListItem value="BT">Bhutan</asp:ListItem>
                    <asp:ListItem value="BO">Bolivia</asp:ListItem>
                    <asp:ListItem value="BA">Bosnia And Herzegowina</asp:ListItem>
                    <asp:ListItem value="BW">Botswana</asp:ListItem>
                    <asp:ListItem value="BV">Bouvet Island</asp:ListItem>
                    <asp:ListItem value="BR">Brazil</asp:ListItem>
                    <asp:ListItem value="IO">British Indian Ocean Territory</asp:ListItem>
                    <asp:ListItem value="BN">Brunei Darussalam</asp:ListItem>
                    <asp:ListItem value="BG">Bulgaria</asp:ListItem>
                    <asp:ListItem value="BF">Burkina Faso</asp:ListItem>
                    <asp:ListItem value="BI">Burundi</asp:ListItem>
                    <asp:ListItem value="KH">Cambodia</asp:ListItem>
                    <asp:ListItem value="CM">Cameroon</asp:ListItem>
                    <asp:ListItem value="CA">Canada</asp:ListItem>
                    <asp:ListItem value="CV">Cape Verde</asp:ListItem>
                    <asp:ListItem value="KY">Cayman Islands</asp:ListItem>
                    <asp:ListItem value="CF">Central African Republic</asp:ListItem>
                    <asp:ListItem value="TD">Chad</asp:ListItem>
                    <asp:ListItem value="CL">Chile</asp:ListItem>
                    <asp:ListItem value="CN">China</asp:ListItem>
                    <asp:ListItem value="CX">Christmas Island</asp:ListItem>
                    <asp:ListItem value="CC">Cocos (Keeling) Islands</asp:ListItem>
                    <asp:ListItem value="CO">Colombia</asp:ListItem>
                    <asp:ListItem value="KM">Comoros</asp:ListItem>
                    <asp:ListItem value="CG">Congo</asp:ListItem>
                    <asp:ListItem value="CK">Cook Islands</asp:ListItem>
                    <asp:ListItem value="CR">Costa Rica</asp:ListItem>
                    <asp:ListItem value="CI">Cote D'Ivoire</asp:ListItem>
                    <asp:ListItem value="HR">Croatia (Local Name: Hrvatska)</asp:ListItem>
                    <asp:ListItem value="CU">Cuba</asp:ListItem>
                    <asp:ListItem value="CY">Cyprus</asp:ListItem>
                    <asp:ListItem value="CZ">Czech Republic</asp:ListItem>
                    <asp:ListItem value="DK">Denmark</asp:ListItem>
                    <asp:ListItem value="DJ">Djibouti</asp:ListItem>
                    <asp:ListItem value="DM">Dominica</asp:ListItem>
                    <asp:ListItem value="DO">Dominican Republic</asp:ListItem>
                    <asp:ListItem value="TP">East Timor</asp:ListItem>
                    <asp:ListItem value="EC">Ecuador</asp:ListItem>
                    <asp:ListItem value="EG">Egypt</asp:ListItem>
                    <asp:ListItem value="SV">El Salvador</asp:ListItem>
                    <asp:ListItem value="GQ">Equatorial Guinea</asp:ListItem>
                    <asp:ListItem value="ER">Eritrea</asp:ListItem>
                    <asp:ListItem value="EE">Estonia</asp:ListItem>
                    <asp:ListItem value="ET">Ethiopia</asp:ListItem>
                    <asp:ListItem value="FK">Falkland Islands (Malvinas)</asp:ListItem>
                    <asp:ListItem value="FO">Faroe Islands</asp:ListItem>
                    <asp:ListItem value="FJ">Fiji</asp:ListItem>
                    <asp:ListItem value="FI">Finland</asp:ListItem>
                    <asp:ListItem value="FR">France</asp:ListItem>
                    <asp:ListItem value="GF">French Guiana</asp:ListItem>
                    <asp:ListItem value="PF">French Polynesia</asp:ListItem>
                    <asp:ListItem value="TF">French Southern Territories</asp:ListItem>
                    <asp:ListItem value="GA">Gabon</asp:ListItem>
                    <asp:ListItem value="GM">Gambia</asp:ListItem>
                    <asp:ListItem value="GE">Georgia</asp:ListItem>
                    <asp:ListItem value="DE">Germany</asp:ListItem>
                    <asp:ListItem value="GH">Ghana</asp:ListItem>
                    <asp:ListItem value="GI">Gibraltar</asp:ListItem>
                    <asp:ListItem value="GR">Greece</asp:ListItem>
                    <asp:ListItem value="GL">Greenland</asp:ListItem>
                    <asp:ListItem value="GD">Grenada</asp:ListItem>
                    <asp:ListItem value="GP">Guadeloupe</asp:ListItem>
                    <asp:ListItem value="GU">Guam</asp:ListItem>
                    <asp:ListItem value="GT">Guatemala</asp:ListItem>
                    <asp:ListItem value="GN">Guinea</asp:ListItem>
                    <asp:ListItem value="GW">Guinea-Bissau</asp:ListItem>
                    <asp:ListItem value="GY">Guyana</asp:ListItem>
                    <asp:ListItem value="HT">Haiti</asp:ListItem>
                    <asp:ListItem value="HM">Heard And Mc Donald Islands</asp:ListItem>
                    <asp:ListItem value="VA">Holy See (Vatican City State)</asp:ListItem>
                    <asp:ListItem value="HN">Honduras</asp:ListItem>
                    <asp:ListItem value="HK">Hong Kong</asp:ListItem>
                    <asp:ListItem value="HU">Hungary</asp:ListItem>
                    <asp:ListItem value="IS">Icel And</asp:ListItem>
                    <asp:ListItem value="IN">India</asp:ListItem>
                    <asp:ListItem value="ID">Indonesia</asp:ListItem>
                    <asp:ListItem value="IR">Iran (Islamic Republic Of)</asp:ListItem>
                    <asp:ListItem value="IQ">Iraq</asp:ListItem>
                    <asp:ListItem value="IE">Ireland</asp:ListItem>
                    <asp:ListItem value="IL">Israel</asp:ListItem>
                    <asp:ListItem value="IT">Italy</asp:ListItem>
                    <asp:ListItem value="JM">Jamaica</asp:ListItem>
                    <asp:ListItem value="JP">Japan</asp:ListItem>
                    <asp:ListItem value="JO">Jordan</asp:ListItem>
                    <asp:ListItem value="KZ">Kazakhstan</asp:ListItem>
                    <asp:ListItem value="KE">Kenya</asp:ListItem>
                    <asp:ListItem value="KI">Kiribati</asp:ListItem>
                    <asp:ListItem value="KP">Korea, Dem People'S Republic</asp:ListItem>
                    <asp:ListItem value="KR">Korea, Republic Of</asp:ListItem>
                    <asp:ListItem value="KW">Kuwait</asp:ListItem>
                    <asp:ListItem value="KG">Kyrgyzstan</asp:ListItem>
                    <asp:ListItem value="LA">Lao People'S Dem Republic</asp:ListItem>
                    <asp:ListItem value="LV">Latvia</asp:ListItem>
                    <asp:ListItem value="LB">Lebanon</asp:ListItem>
                    <asp:ListItem value="LS">Lesotho</asp:ListItem>
                    <asp:ListItem value="LR">Liberia</asp:ListItem>
                    <asp:ListItem value="LY">Libyan Arab Jamahiriya</asp:ListItem>
                    <asp:ListItem value="LI">Liechtenstein</asp:ListItem>
                    <asp:ListItem value="LT">Lithuania</asp:ListItem>
                    <asp:ListItem value="LU">Luxembourg</asp:ListItem>
                    <asp:ListItem value="MO">Macau</asp:ListItem>
                    <asp:ListItem value="MK">Macedonia</asp:ListItem>
                    <asp:ListItem value="MG">Madagascar</asp:ListItem>
                    <asp:ListItem value="MW">Malawi</asp:ListItem>
                    <asp:ListItem value="MY">Malaysia</asp:ListItem>
                    <asp:ListItem value="MV">Maldives</asp:ListItem>
                    <asp:ListItem value="ML">Mali</asp:ListItem>
                    <asp:ListItem value="MT">Malta</asp:ListItem>
                    <asp:ListItem value="MH">Marshall Islands</asp:ListItem>
                    <asp:ListItem value="MQ">Martinique</asp:ListItem>
                    <asp:ListItem value="MR">Mauritania</asp:ListItem>
                    <asp:ListItem value="MU">Mauritius</asp:ListItem>
                    <asp:ListItem value="YT">Mayotte</asp:ListItem>
                    <asp:ListItem value="MX">Mexico</asp:ListItem>
                    <asp:ListItem value="FM">Micronesia, Federated States</asp:ListItem>
                    <asp:ListItem value="MD">Moldova, Republic Of</asp:ListItem>
                    <asp:ListItem value="MC">Monaco</asp:ListItem>
                    <asp:ListItem value="MN">Mongolia</asp:ListItem>
                    <asp:ListItem value="MS">Montserrat</asp:ListItem>
                    <asp:ListItem value="MA">Morocco</asp:ListItem>
                    <asp:ListItem value="MZ">Mozambique</asp:ListItem>
                    <asp:ListItem value="MM">Myanmar</asp:ListItem>
                    <asp:ListItem value="NA">Namibia</asp:ListItem>
                    <asp:ListItem value="NR">Nauru</asp:ListItem>
                    <asp:ListItem value="NP">Nepal</asp:ListItem>
                    <asp:ListItem value="NL">Netherlands</asp:ListItem>
                    <asp:ListItem value="AN">Netherlands Ant Illes</asp:ListItem>
                    <asp:ListItem value="NC">New Caledonia</asp:ListItem>
                    <asp:ListItem value="NZ">New Zealand</asp:ListItem>
                    <asp:ListItem value="NI">Nicaragua</asp:ListItem>
                    <asp:ListItem value="NE">Niger</asp:ListItem>
                    <asp:ListItem value="NG">Nigeria</asp:ListItem>
                    <asp:ListItem value="NU">Niue</asp:ListItem>
                    <asp:ListItem value="NF">Norfolk Island</asp:ListItem>
                    <asp:ListItem value="MP">Northern Mariana Islands</asp:ListItem>
                    <asp:ListItem value="NO">Norway</asp:ListItem>
                    <asp:ListItem value="OM">Oman</asp:ListItem>
                    <asp:ListItem value="PK">Pakistan</asp:ListItem>
                    <asp:ListItem value="PW">Palau</asp:ListItem>
                    <asp:ListItem value="PA">Panama</asp:ListItem>
                    <asp:ListItem value="PG">Papua New Guinea</asp:ListItem>
                    <asp:ListItem value="PY">Paraguay</asp:ListItem>
                    <asp:ListItem value="PE">Peru</asp:ListItem>
                    <asp:ListItem value="PH">Philippines</asp:ListItem>
                    <asp:ListItem value="PN">Pitcairn</asp:ListItem>
                    <asp:ListItem value="PL">Poland</asp:ListItem>
                    <asp:ListItem value="PT">Portugal</asp:ListItem>
                    <asp:ListItem value="PR">Puerto Rico</asp:ListItem>
                    <asp:ListItem value="QA">Qatar</asp:ListItem>
                    <asp:ListItem value="RE">Reunion</asp:ListItem>
                    <asp:ListItem value="RO">Romania</asp:ListItem>
                    <asp:ListItem value="RU">Russian Federation</asp:ListItem>
                    <asp:ListItem value="RW">Rwanda</asp:ListItem>
                    <asp:ListItem value="KN">Saint K Itts And Nevis</asp:ListItem>
                    <asp:ListItem value="LC">Saint Lucia</asp:ListItem>
                    <asp:ListItem value="VC">Saint Vincent, The Grenadines</asp:ListItem>
                    <asp:ListItem value="WS">Samoa</asp:ListItem>
                    <asp:ListItem value="SM">San Marino</asp:ListItem>
                    <asp:ListItem value="ST">Sao Tome And Principe</asp:ListItem>
                    <asp:ListItem value="SA">Saudi Arabia</asp:ListItem>
                    <asp:ListItem value="SN">Senegal</asp:ListItem>
                    <asp:ListItem value="SC">Seychelles</asp:ListItem>
                    <asp:ListItem value="SL">Sierra Leone</asp:ListItem>
                    <asp:ListItem value="SG">Singapore</asp:ListItem>
                    <asp:ListItem value="SK">Slovakia (Slovak Republic)</asp:ListItem>
                    <asp:ListItem value="SI">Slovenia</asp:ListItem>
                    <asp:ListItem value="SB">Solomon Islands</asp:ListItem>
                    <asp:ListItem value="SO">Somalia</asp:ListItem>
                    <asp:ListItem value="ZA">South Africa</asp:ListItem>
                    <asp:ListItem value="GS">South Georgia , S Sandwich Is.</asp:ListItem>
                    <asp:ListItem value="ES">Spain</asp:ListItem>
                    <asp:ListItem value="LK">Sri Lanka</asp:ListItem>
                    <asp:ListItem value="SH">St. Helena</asp:ListItem>
                    <asp:ListItem value="PM">St. Pierre And Miquelon</asp:ListItem>
                    <asp:ListItem value="SD">Sudan</asp:ListItem>
                    <asp:ListItem value="SR">Suriname</asp:ListItem>
                    <asp:ListItem value="SJ">Svalbard, Jan Mayen Islands</asp:ListItem>
                    <asp:ListItem value="SZ">Sw Aziland</asp:ListItem>
                    <asp:ListItem value="SE">Sweden</asp:ListItem>
                    <asp:ListItem value="CH">Switzerland</asp:ListItem>
                    <asp:ListItem value="SY">Syrian Arab Republic</asp:ListItem>
                    <asp:ListItem value="TW">Taiwan</asp:ListItem>
                    <asp:ListItem value="TJ">Tajikistan</asp:ListItem>
                    <asp:ListItem value="TZ">Tanzania, United Republic Of</asp:ListItem>
                    <asp:ListItem value="TH">Thailand</asp:ListItem>
                    <asp:ListItem value="TG">Togo</asp:ListItem>
                    <asp:ListItem value="TK">Tokelau</asp:ListItem>
                    <asp:ListItem value="TO">Tonga</asp:ListItem>
                    <asp:ListItem value="TT">Trinidad And Tobago</asp:ListItem>
                    <asp:ListItem value="TN">Tunisia</asp:ListItem>
                    <asp:ListItem value="TR">Turkey</asp:ListItem>
                    <asp:ListItem value="TM">Turkmenistan</asp:ListItem>
                    <asp:ListItem value="TC">Turks And Caicos Islands</asp:ListItem>
                    <asp:ListItem value="TV">Tuvalu</asp:ListItem>
                    <asp:ListItem value="UG">Uganda</asp:ListItem>
                    <asp:ListItem value="UA">Ukraine</asp:ListItem>
                    <asp:ListItem value="AE">United Arab Emirates</asp:ListItem>
                    <asp:ListItem value="GB">United Kingdom</asp:ListItem>
                    <asp:ListItem value="UM">United States Minor Is.</asp:ListItem>
                    <asp:ListItem value="UY">Uruguay</asp:ListItem>
                    <asp:ListItem value="UZ">Uzbekistan</asp:ListItem>
                    <asp:ListItem value="VU">Vanuatu</asp:ListItem>
                    <asp:ListItem value="VE">Venezuela</asp:ListItem>
                    <asp:ListItem value="VN">Viet Nam</asp:ListItem>
                    <asp:ListItem value="VG">Virgin Islands (British)</asp:ListItem>
                    <asp:ListItem value="VI">Virgin Islands (U.S.)</asp:ListItem>
                    <asp:ListItem value="WF">Wallis And Futuna Islands</asp:ListItem>
                    <asp:ListItem value="EH">Western Sahara</asp:ListItem>
                    <asp:ListItem value="YE">Yemen</asp:ListItem>
                    <asp:ListItem value="YU">Yugoslavia</asp:ListItem>
                    <asp:ListItem value="ZR">Zaire</asp:ListItem>
                    <asp:ListItem value="ZM">Zambia</asp:ListItem>
                    <asp:ListItem value="ZW">Zimbabwe</asp:ListItem>
                </asp:DropDownList>
                
                <asp:RequiredFieldValidator ID="vcountry" runat="server" ValidationGroup="ContactUs" ControlToValidate="country" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
            </div>
        </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
        <div class="twocolumn">
            <div class="form_label">City:&nbsp;&nbsp;</div>
            <div class="form_fields"><asp:TextBox runat="server" class="inputbox" name="city" ID="city" size="26" /></div>
        </div>
        <div class="twocolumn">
            <div class="form_label">State:&nbsp;&nbsp;</div>
            <div class="form_fields" style="width:75px;">
                <asp:TextBox runat="server" class="inputbox" name="state" ID="state" size="10" Width="73" />
            </div>
            <div id="zips" class="form_label" style="width:96px;">Zip Code:&nbsp;&nbsp;</div>
            <div class="form_fields" style="width:120px;">
                <asp:TextBox runat="server" class="inputbox" name="zipcode" ID="zipcode" size="10" Width="118" />
            </div>
        </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
         <div class="twocolumn">
            <div class="form_label">Type of Inquiry:&nbsp;&nbsp;</div>
            <div class="form_fields"><asp:DropDownList runat="server" name="inquiry_type" id="inquiry_type" class="inputSelect" AutoPostBack="true">
                    <asp:ListItem value="" >-- Select --</asp:ListItem>
                    <asp:ListItem value="Products">Products</asp:ListItem>
                    <asp:ListItem value="Dealers Sales">Dealers / Sales</asp:ListItem>
                    <asp:ListItem value="Customer Services">Customer Service</asp:ListItem>
                    <asp:ListItem value="Technical Issue Question">Technical Issue / Question</asp:ListItem>
                    <asp:ListItem value="Parts Repair Service">Parts / Repair Service</asp:ListItem>
                    <asp:ListItem value="Media Requests">Media Request</asp:ListItem>
                    <asp:ListItem value="Employment Opportunities"> Employment Opportunities</asp:ListItem>
                    <asp:ListItem value="K12">K-12</asp:ListItem>
                    <asp:ListItem value="Other">Other</asp:ListItem>
                </asp:DropDownList>
                
                <asp:RequiredFieldValidator ID="vinquiry_type" runat="server" ValidationGroup="ContactUs" ControlToValidate="inquiry_type" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
            </div>
        </div>
        <div class="twocolumn">
            <div id="MMP" runat="server">
                <div class="form_label">Product Brand:&nbsp;&nbsp;</div>
                <div class="form_fields">
                    <asp:DropDownList runat="server" name="brand" id="brand" class="inputSelect">
                        <asp:ListItem value="Vulcan Wolf">Vulcan / Wolf</asp:ListItem>
                        <asp:ListItem value="Berkel">Berkel</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div id="MM" runat="server">
                <div class="form_label">Categories:&nbsp;&nbsp;</div>
                <div class="form_fields">
                    <asp:DropDownList runat="server" id="category" name="category" class="inputSelect">
                    	<asp:ListItem value="">-Select a Product-</asp:ListItem>
                    	<asp:ListItem value="Braising Pans">Braising Pans</asp:ListItem>
                        <asp:ListItem value="Charboilers">Charboilers</asp:ListItem>
                    	<asp:ListItem value="Combi Ovens">Combi Ovens</asp:ListItem>
                    	<asp:ListItem value="Convection Ovens">Convection Ovens</asp:ListItem>
                        <asp:ListItem value="Fryers">Fryers</asp:ListItem>
                        <asp:ListItem value="Griddles">Charboilers</asp:ListItem>
                        <asp:ListItem value="Heavy Duty Cooking">Heavy Duty Cooking</asp:ListItem>
                        <asp:ListItem value="Heated Holding">Heated Holding</asp:ListItem>
                        <asp:ListItem value="Kettles">Kettles</asp:ListItem>
                        <asp:ListItem value="Restaurant Ranges">Restaurant Ranges</asp:ListItem>
                        <asp:ListItem value="Steamers">Steamers</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
            <div class="form_label">Comments:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox runat="server" TextMode="MultiLine" rows="3" name="comment" ID="comment" cols="41"></asp:TextBox>
            </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
            <div class="form_label">Type the letters&nbsp;&nbsp;<br />you see below:&nbsp;&nbsp;</div>
            <div class="form_fields">
                <asp:TextBox ID="fields_CAPTCHA" runat="server" class="defaultText" />
                <asp:RequiredFieldValidator ID="vCaptcha" runat="server" ValidationGroup="ContactUs" ControlToValidate="fields_CAPTCHA" Text="" CssClass="fieldReq" ForeColor="#e13433"  />
                
            </div>
      </div>
      <br style="clear:both;" />
      <div class="row">
            <div class="form_label">&nbsp;</div>
            <div class="form_fields"><asp:Image ID="CAPTCHA" ImageUrl="/CAPTCHAimage/CAPTCHAimage.aspx" runat="server" /></div>
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
    </ContentTemplate>
        <Triggers>
         
        </Triggers>
    </asp:UpdatePanel>
    </div> <!-- end middle section backgrounds -->
    </div>
</asp:Content>

<asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBceJ4ereZCAJt8bodBzq-k58RanFKu-Ww&sensor=false"></script> 
    
    <script type="text/javascript">
        function showMap(map) {
            $(map).fadeIn(800);
            $(".contact_info").hide();
            initialize();
        }

        function hideMap(map) {
            $(map).hide();
            $(".contact_info").fadeIn(800);
        }

        var map;
        var infowindow = [];
        var marker = [];

        function initialize() {
            var mapOptions = {
                zoom: 11,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                scrollwheel: false
            };

            map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

            setMarkers();
        }

        function setMarkers() {
            map.setCenter(new google.maps.LatLng(39.278925, -76.479267), 11);

            var html = "<b>Vulcan</b> <br/>3600 North Point Blvd.<br/>Baltimore, MD 21222"
					     + "<br/><a target='_blank' href='http://maps.google.com/maps?daddr=3600 North Point Blvd. Baltimore, MD 21222&hl=en'>get directions</a>";

            var myLatlng = new google.maps.LatLng(39.278925, -76.479267);
            var newmarker = new google.maps.Marker({
                position: myLatlng,
                map: map,
                html: html,
                icon: '/images/marker.png'
            });

            newmarker['infowindow'] = new google.maps.InfoWindow({
                content: html
            });

            google.maps.event.addListener(newmarker, 'click', function() {
                for (var i = 0; i < marker.length; i++) {
                    marker[i]['infowindow'].close();
                }
                this['infowindow'].open(map, this);
            });

            marker.push(newmarker);
        }

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
    </script>
</asp:Content>
