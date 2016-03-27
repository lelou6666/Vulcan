<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta name='GENERATOR' content='SpreadsheetConverter to HTML/JavaScript version 5.2.15.2036)' />
<meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
<!-- SpreadsheetConverter Header start -->
<!-- Parts of this page Copyright (C) 2002-2010 Framtidsforum I&M AB, Sweden -->
<title>Power Fry VK45 Savings Calculator</title>
<style type="text/css">
/* Styles needed by SpreadsheetConverter */

*.ee100 {
	background : #333333;
	color : white;
	font-family : Arial, sans-serif;
	font-size : 16.00pt;
	font-style : normal;
	font-weight : 700
}
*.ee102 {
	color : white;
	font-family : Arial, sans-serif;
	font-size : 14.00pt;
	font-style : normal;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : middle
}
*.ee105 {
	color : white;
	font-family : Arial, sans-serif;
	font-size : 21pt;
	font-style : normal;
	font-weight : 700;
	text-decoration : none;
	line-height: 20pt;
}
*.ee107 {
	color : white;
	font-family : Arial, sans-serif;
	font-size : 19px;
	font-style : normal;
	font-weight : 700;
	text-decoration : none;
	line-height: 22px;
}
*.ee109 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 20.00pt;
	font-style : normal;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : bottom
}
*.ee112 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : italic;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : middle
}
*.ee115 {
	color : black;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 700;
	padding-left : 0px;
	padding-right : 1px;
	padding-top : 0px;
	text-align : center;
	vertical-align : bottom
}
*.ee117 {
	font-family : Arial, sans-serif;
	font-size : 17px;
	text-align : center;
	vertical-align : bottom
}
*.ee118 {
	/* background : #595959;
	color : white;
	font-family : Arial, sans-serif;
	font-size : 14.00pt;
	font-style : normal;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px; */
	text-align : center;
	vertical-align : middle ;
}
*.ee121 {
	font-family : Arial;
	font-size : 13px;
	text-align : right;
	padding-right:10px;
}
*.ee124 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : bottom
}
*.ee127 {
	color : windowtext;
	font-family : Arial;
	font-size : 12px;
	font-style : normal;
	font-weight : 400
}
*.ee129 {
	color : windowtext;
	font-family : Arial;
	font-size : 12px;
	font-style : normal;
	font-weight : 400;
	padding-left : 0px;
	padding-right : 1px;
	padding-top : 0px;
	text-align : center;
	vertical-align : bottom
}

*.ee132 {
	color : windowtext;
	font-family : Arial;
	font-size : 12px;
	font-style : normal;
	font-weight : 400;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : center;
	vertical-align : bottom;
	background-color:Transparent
}
*.ee133, *.ee161, *.ee171  {
	color : windowtext;
	font-family : Arial;
	font-size: 19px;
	text-align : left;
	vertical-align : bottom;
	background-color:Transparent;
}
*.ee171 
{
    color: #cc0000;
}

*.ee134 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : center;
	vertical-align : bottom
}
*.ee137 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : center;
	vertical-align : bottom
}
*.ee139 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	text-decoration : underline;
	vertical-align : bottom
}
*.ee142 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : center;
	text-decoration : underline;
	vertical-align : bottom
}
*.ee145 
{
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : bottom
}
*.ee148, *.ee149, *.ee150, *.ee151, *.ee152, *.ee153, *.ee159, *.ee162, *.ee167, *.ee170, *.ee175, *.ee176
{
    border-top: solid 1px #cfcfcf;
    border-right: solid 1px #cfcfcf;
    border-left: solid 1px #cfcfcf;
    border-bottom: solid 1px #cfcfcf;
    background: #f5f5f5;
    padding: 9px 4px 7px 9px;
	text-align : left;
	vertical-align : bottom;
	margin-left:10px;
	width:270px;
	
}

*.ee156 {
	text-align : right;
	padding-right:10px;
}


*.ee163 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 400;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : bottom
}
*.ee164 {
	color: #cc0000;
	text-align : right;
	padding-right:10px;
	font-weight:bold;
}

*.ee167 {
	color: #cc0000;
	text-align : left;
	font-weight:bold;
	font-size:19px;
}

*.ee172 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 400;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : bottom
}
*.ee174 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 400;
	text-decoration : none
}

*.ee177 {
	color : #66FF66;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 400;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : bottom
}
*.ee179 {
	color : #66FF66;
	font-family : Arial, sans-serif;
	font-size : 12px;
	font-style : normal;
	font-weight : 400;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : center;
	vertical-align : bottom
}
*.ee181 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 14px;

	text-align : left;
	vertical-align : bottom
}
*.ee184 {
	background : #FF5050;
	color : white;
	font-family : Arial, sans-serif;
	font-size : 8.00pt;
	font-style : normal;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : center;
	vertical-align : bottom
}
*.ee187 {
	color : white;
	font-family : Arial, sans-serif;
	font-size : 8.00pt;
	font-style : normal;
	font-weight : 700;
	text-decoration : none
}
*.ee188 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 6.00pt;
	font-style : normal;
	font-weight : 700;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : bottom
}
*.ee191 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 8.00pt;
	font-style : normal;
	font-weight : 400;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : bottom
}
*.ee194 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 6.00pt;
	font-style : normal;
	font-weight : 400;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : bottom
}
*.ee197 {
	color : windowtext;
	font-family : Arial, sans-serif;
	font-size : 6.00pt;
	font-style : italic;
	font-weight : 400;
	padding-left : 1px;
	padding-right : 1px;
	padding-top : 1px;
	text-align : left;
	vertical-align : bottom
}
textarea {
	overflow: auto;
}
sup {
	font-size: 12pt;
	position: relative top;
}
sub {
	font-size: 8pt;
}


/*New */

</style>
<script type="text/javascript" src="support/jquery.js"></script>
<script>jQuery(document).ready(function(){
                    jQuery('#loading').fadeOut(500);});</script>
<link rel="stylesheet" type="text/css" href="YUI/fonts-min.css" />
<script type="text/javascript" src="YUI/yahoo-dom-event.js"></script>
<script type="text/javascript" src="YUI/yahoo-min.js"></script>
<script type="text/javascript" src="YUI/cookie-beta-min.js"></script>
<script type="text/javascript" src="YUI/cookie-min.js"></script>
<link rel="stylesheet" type="text/css" href="YUI/tabview.css" />
<script type="text/javascript" src="YUI/element-beta-min.js"></script>
<script type="text/javascript" src="YUI/tabview-min.js"></script>
<link rel="stylesheet" type="text/css" href="YUI/container.css" />
<script type="text/javascript" src="YUI/container-min.js"></script>
<style type='text/css' media='print'>
.mystyles {display:none;}.printclass {display:block;}*.ee100 {border:0px solid #000000;}*.ee103 {border:0px solid #000000;}*.ee105 {border:0px solid #000000;}*.ee106 {border:0px solid #000000;}*.ee108 {border:0px solid #000000;}*.ee109 {border:0px solid #000000;}*.ee110 {border:0px solid #000000;}*.ee111 {border:0px solid #000000;}*.ee113 {border:0px solid #000000;}*.ee117 {border:0px solid #000000;}*.ee118 {border:0px solid #000000;}*.ee120 {border:0px solid #000000;}*.ee121 {border:0px solid #000000;}*.ee122 {border:0px solid #000000;}.eebuttonbar_top ,.eebuttonbar_bottom, .eetabs,.wizardbuttons,.buttonPage,.buttonPage,.buttonFinish,.buttonCancel,.yui-content .highlighted, .yui-content .dimmed{display:none!important;}div {border:none!important;}.msg_body{background-color:White !important;}.accordion h3 {display:none!important;}.accordion p {border:none;}
</style>
<link rel='stylesheet' type='text/css' href='support/ssc_styles.css' />
<link rel='stylesheet' type='text/css' href='' media="print" id="default" />
<link rel='stylesheet' type='text/css' href='support/wizardstyle.css' />
<link rel='stylesheet' type='text/css' href='main.css' />
<style type="text/css" media="print">
.button,.dactive,.button,.dactive{ display:none ;}




</style>
<script>
var isSheet0FirstDisplay = true;
</script>
<script>var SheetCollection = new Array();
SheetCollection[0] = new Array();
</script>

<!-- SpreadsheetConverter Header end -->
</head>
<body class='yui-skin-sam' onload='getCookieValues();SetSliders();postcode();recalc_onclick("obj");setValueofBtn();ResetRatings();' onunload='setCookieValues();' onclick="if(document.getElementById('showagain')!=null){if(document.getElementById('showagain').checked){}else{myDialog.hide();}}">
<!-- SpreadsheetConverter Body start --> 
<!--<img id='loading' style='z-index:103;position:absolute;top:50%;left:50%;' src='support/loading.gif'/>-->
<div id="lightalert" class="white_content_alert" style="padding:10px 10px 10px 10px">
  <table width="100%">
    <tr style="height:50px;">
      <td colspan="2"><i>Please fill all required fields</i></td>
    </tr>
    <tr>
      <td align="right"><input type="button" id="btnCaptchaalert" value="OK" class="formBttn" onClick="CancelCaptchaalert();showPanel();if(ControlToFocus != null){ControlToFocus.scrollIntoView();ControlToFocus.focus();}" /></td>
    </tr>
  </table>
</div>
<div id="fadealert" class="black_overlay"></div>
<form id='formc' name='formc' method='post' action=''>
  <input type="hidden" id="currentpanel" />
  <input type="hidden" id="panelidz" />
  <!--hidden fields for success,failure and cancel urls.--> 
  <!--hidden fields for target, spreadsheet,cleint.-->
  <input type='hidden' id='xl_spreadsheet' name='xl_spreadsheet' value='NEW Savings calculator 01-05-12_6.xls' />
  <input type='hidden' id='xl_client' name='xl_client' value='h5.2.15' />
  <!--<div id='demo1'>
<div class='mystyles'>
<ul class='yui-nav'></ul>--> 
  <!--<div class='yui-content' style="padding:0!important; margin:0!important">
<div style="page-break-after:auto;display:block;padding:5px;" class='printclass'>-->
  
  <div align="center">
    <table align="center" cellpadding="0" cellspacing="0" border="0" width="100%">
      <tr>
        <td><div align="center">
            <table cellspacing='0' cellpadding='0' width='920' style='border-collapse:collapse;' border="0">
              <tr>
                <td width='328'></td>
                <td width='221'></td>
                <td width='228'></td>
              </tr>
              <tr>
                <td colspan="3" align="left">
                    <div id="container_calc">
                        <div id="calc_header_container"><img src="calc_header.png" width="719" height="104" alt="" /></div>
                        <div id="calc_options_container">
    	                    <div id="calc_options_header_container"><img src="calc_options_header.png" width="796" height="16" alt="" /></div>
                          <div class="clearfix">
      	                    <div class="calc_option_input calc_op_time">
      		                    <input onChange="this.value=eedisplayFloatNDTh(eeparseFloatTh(this.value),0);recalc_onclick('XLEW_1_4_2') ;setValueofBtn();" value='16' type='text' tabindex='2'  name='XLEW_1_4_2' id='XLEW_1_4_2' class='ee131' downkey='XLEW_1_5_2' onKeyDown="onEnterPress('XLEW_1_4_2');" />
      	                    </div>
      	                    <div class="calc_option_input calc_avg">
      		                    <input onChange="this.value=eedisplayFloatNDTh(eeparseFloatTh(this.value),0);recalc_onclick('XLEW_1_5_2') ;setValueofBtn();" value='200' type='text' tabindex='3' name='XLEW_1_5_2' id='XLEW_1_5_2' class='ee131' style="width:223px;" downkey='XLEW_1_6_2' upkey='XLEW_1_4_2' onKeyDown="onEnterPress('XLEW_1_5_2');" />
      	                    </div>
      	                    <div class="calc_option_input calc_vats">
      		                    <input onChange="this.value=eedisplayFloatNDTh(eeparseFloatTh(this.value),0);recalc_onclick('XLEW_1_6_2') ;setValueofBtn();" value='4' type='text' tabindex='4'  name='XLEW_1_6_2' id='XLEW_1_6_2' class='ee131' style="width:123px;" upkey='XLEW_1_5_2' onKeyDown="onEnterPress('XLEW_1_6_2');" />
      	                    </div>
                          </div>
                        </div>
                    </div>
                 </td>
             </tr>
         </table>
         <table cellspacing='0' cellpadding='0' width='820' style='border-collapse:collapse;' border="0">
              <tr style='height:17pt'>
                <td style="width:270px;"  class='ee112' sheetid="1" rowid="2" colid="1"></td>
                <td class='ee115' sheetid="1" rowid="2" colid="2"><fieldset id="XLEW_1_2_2ssc" style="border:0;padding:0;">
                    <select name='XLEW_1_2_2' id='XLEW_1_2_2' class='ee117 calc_model' tabindex='1' onChange="recalc_onclick('XLEW_1_2_2')" size='1'>
                      <option  value='Anets GoldenFRY **'>Anets GoldenFRY **</option>
                      <option  value='Dean D50G'>Dean D50G</option>
                      <option  value='Dean HD50 **'>Dean HD50 **</option>
                      <option  value='Dean HD60 **'>Dean HD60 **</option>
                      <option  value='Dean SM50G'>Dean SM50G</option>
                      <option  value='Dean SR52G'>Dean SR52G</option>
                      <option  value='Imperial Elite 50lb Fryer' selected='selected'>Imperial Elite 50lb Fryer</option>
                      <option  value='Infinity G2842 **'>Infinity G2842 **</option>
                      <option  value='Frymaster GF14'>Frymaster GF14</option>
                      <option  value='Frymaster H55 **'>Frymaster H55 **</option>
                      <option  value='Frymaster MJ45'>Frymaster MJ45</option>
                      <option  value='Frymaster Protector **'>Frymaster Protector **</option>
                      <option  value='Henny Penny EEG-14X **'>Henny Penny EEG-14X **</option>
                      <option  value='Henny Penny OFG-321 **'>Henny Penny OFG-321 **</option>
                      <option  value='Keating 14 IFM **'>Keating 14 IFM **</option>
                      <option  value='Pitco AG14 **'>Pitco AG14 **</option>
                      <option  value='Pitco 45C'>Pitco 45C</option>
                      <option  value='Pitco Frialator RPB14 **'>Pitco Frialator RPB14 **</option>
                      <option  value='Pitco SGH50 **'>Pitco SGH50 **</option>
                      <option  value='Pitco SG14R'>Pitco SG14R</option>
                      <option  value='Pitco SSH55 **'>Pitco SSH55 **</option>
                      <option  value='Pitco VF35 **'>Pitco VF35 **</option>
                      <option  value='Vulcan GR 35 **'>Vulcan GR 35 **</option>
                      <option  value='Vulcan GR 45 **'>Vulcan GR 45 **</option>
                      <option  value='Ultrafryer BP 20-18'>Ultrafryer BP 20-18</option>
                      <option  value='Ultrafryer Par 3-14'>Ultrafryer Par 3-14</option>
                    </select>
                  </fieldset></td>
                <td class='ee118' sheetid="1" rowid="2" colid="3">
                    <div id="calc_current_product">
                      <div>Vulcan PowerFry&trade; VK45</div>
                    </div>
                </td>
              </tr>
          </table>
         <table cellspacing='0' cellpadding='0' width='820' style='border-collapse:collapse;' border="0">
              <tr style='height:3pt'>
                <td style="width:270px;" class='ee121' sheetid="1" rowid="3" colid="1"><div style="height:3pt;overflow:hidden"></div></td>
                <td class='ee121' sheetid="1" rowid="3" colid="2"><div style="height:3pt;overflow:hidden"></div></td>
                <td class='ee121' sheetid="1" rowid="3" colid="3"><div style="height:3pt;overflow:hidden"></div></td>
              </tr>
              <tr style='height:13pt'>
                <td style="width:270px;" class='ee121' sheetid="1" rowid="9" colid="1"> Preheat Time (mins)
                  * </td>
                <td class='ee148' sheetid="1" rowid="9" colid="2"><input readonly='readonly' value='0' type='text' tabindex='-1' style='overflow:hidden; border:0px solid #000000;' name='XLEW_1_9_2' id='XLEW_1_9_2' class='ee133' /></td>
                <td class='ee149' sheetid="1" rowid="9" colid="3"><input readonly='readonly' value='7.01' type='text' tabindex='5' style='overflow:hidden; border:0px solid #000000; ' name='XLEW_1_9_3' id='XLEW_1_9_3' class='ee133' /></td>
              </tr>
              <tr style='height:13pt'>
                <td class='ee121' sheetid="1" rowid="11" colid="1"> Idle Energy Rate
                  (BTU/hr) ** </td>
                <td class='ee148' sheetid="1" rowid="11" colid="2"><input readonly='readonly' value='0' type='text' tabindex='-1' style='overflow:hidden; border:0px solid #000000;' name='XLEW_1_11_2' id='XLEW_1_11_2' class='ee133' /></td>
                <td class='ee151' sheetid="1" rowid="11" colid="3"><input readonly='readonly' value='4,251' type='text' tabindex='6' style='overflow:hidden; border:0px solid #000000; ' name='XLEW_1_11_3' id='XLEW_1_11_3' class='ee133' /></td>
              </tr>
              <tr style='height:13pt'>
                <td class='ee121' sheetid="1" rowid="12" colid="1"> Heavy-Load Cooking
                  Energy Rate (BTU/hr) ** </td>
                <td class='ee148' sheetid="1" rowid="12" colid="2"><input readonly='readonly' value='0' type='text' tabindex='-1' style='overflow:hidden; border:0px solid #000000;' name='XLEW_1_12_2' id='XLEW_1_12_2' class='ee133' /></td>
                <td class='ee151' sheetid="1" rowid="12" colid="3"><input readonly='readonly' value='52,617' type='text' tabindex='7' style='overflow:hidden; border:0px solid #000000; ' name='XLEW_1_12_3' id='XLEW_1_12_3' class='ee133' /></td>
              </tr>
              <tr style='height:13pt'>
                <td class='ee121' sheetid="1" rowid="14" colid="1"> Manufacturer's
                  Rated Energy Rate (BTU/hr) </td>
                <td class='ee148' sheetid="1" rowid="14" colid="2"><input readonly='readonly' value='0' type='text' tabindex='-1' style='overflow:hidden; border:0px solid #000000;' name='XLEW_1_14_2' id='XLEW_1_14_2' class='ee133' /></td>
                <td class='ee152' sheetid="1" rowid="14" colid="3"><input readonly='readonly' value='70,000' type='text' tabindex='-1' style='overflow:hidden; border:0px solid #000000;'  class='ee133' /></td>
              </tr>
              <tr style='height:13pt'>
                <td class='ee156' sheetid="1" rowid="36" colid="1"> Cost of
                  Gas Consumption </td>
                <td class='ee159' sheetid="1" rowid="36" colid="2"><input readonly='readonly' value='-' type='text' tabindex='-1' style='overflow:hidden; border:0px solid #000000;' name='XLEW_1_36_2' id='XLEW_1_36_2' class='ee161' /></td>
                <td class='ee162' sheetid="1" rowid="36" colid="3"><input readonly='readonly' value='-' type='text' tabindex='-1' style='overflow:hidden; border:0px solid #000000;' name='XLEW_1_36_3' id='XLEW_1_36_3' class='ee161' /></td>
              </tr>
              <tr style='height:17pt'>
                <td class='ee164' sheetid="1" rowid="38" colid="1"> Yearly Savings </td>
                <td class='ee167' sheetid="1" rowid="38" colid="2">-</td>
                <td class='ee170' sheetid="1" rowid="38" colid="3"><input readonly='readonly' value='-' type='text' tabindex='-1' style='overflow:hidden; border:0px solid #000000;' name='XLEW_1_38_3' id='XLEW_1_38_3' class='ee171' /></td>
              </tr>
              <tr style='height:14pt'>
                <td class='ee156' sheetid="1" rowid="39" colid="1"> Total Number of Years in Service</td>
                <td class='ee159' sheetid="1" rowid="39" colid="2"><input readonly='readonly' value='-' type='text' tabindex='-1' style='overflow:hidden; border:0px solid #000000;' name='XLEW_1_39_2' id='XLEW_1_39_2' class='ee161' /></td>
                <td class='ee175' sheetid="1" rowid="39" colid="3"><input readonly='readonly' value='-' type='text' tabindex='-1' style='overflow:hidden; border:0px solid #000000;' name='XLEW_1_39_3' id='XLEW_1_39_3' class='ee161' /></td>
              </tr>
              <tr style='height:17pt'>
                <td class='ee164' sheetid="1" rowid="40" colid="1"> Overall Total Savings </td>
                <td class='ee167' sheetid="1" rowid="40" colid="2">-</td>
                <td class='ee176' sheetid="1" rowid="40" colid="3"><input readonly='readonly' value='-' type='text' tabindex='-1' style='overflow:hidden; border:0px solid #000000;' name='XLEW_1_40_3' id='XLEW_1_40_3' class='ee171' /></td>
              </tr>
              <tr style='height:3pt'>
                <td class='ee177' sheetid="1" rowid="41" colid="1"><div style="height:3pt;overflow:hidden"></div></td>
                <td class='ee179' sheetid="1" rowid="41" colid="2"><div style="height:3pt;overflow:hidden"></div></td>
                <td class='ee179' sheetid="1" rowid="41" colid="3"><div style="height:3pt;overflow:hidden"></div></td>
              </tr>
              <tr style='height:12pt'>
                <td colspan="3" class='ee181' sheetid="1" rowid="42" colid="1"> 
                <br /><br />
                    <div class="formula">
                       Cost Savings Formula:
                       <br />
           	            <img src="calc_formula.png" height="128" width="494" alt=""/>
                    </div>
                </td>
              </tr>
              <!--<tr style='height:11pt'>
                <td class='ee184' sheetid="1" rowid="43" colid="1" colspan='3'> E<font class='ee187'><sub>gas,daily</sub></font><font class='ee187'> = {q</font><font class='ee187'><sub>gas,h</sub></font><font class='ee187'> x (%h x W)/PC + q</font><font class='ee187'><sub>gas,i</sub></font><font class='ee187'> x t</font><font class='ee187'><sub>on</sub></font><font class='ee187'> - t</font><font class='ee187'><sub>h</sub></font><font class='ee187'> - [(n</font><font class='ee187'><sub>p</sub></font><font class='ee187'> x t</font><font class='ee187'><sub>p</sub></font><font class='ee187'>)/60]</font><font class='ee187'><sub></sub></font><font class='ee187'>+ n</font><font class='ee187'><sub>p</sub></font><font class='ee187'> x E</font><font class='ee187'><sub>gas,p</sub></font><font class='ee187'>}/100000</font></td>
              </tr>
              <tr style='height:1pt'>
                <td class='ee121' sheetid="1" rowid="44" colid="1"><div style="height:1pt;overflow:hidden"></div></td>
                <td class='ee121' sheetid="1" rowid="44" colid="2"><div style="height:1pt;overflow:hidden"></div></td>
                <td class='ee121' sheetid="1" rowid="44" colid="3"><div style="height:1pt;overflow:hidden"></div></td>
              </tr>
              <tr style='height:8pt'>
                <td class='ee188' sheetid="1" rowid="45" colid="1"> Assumptions: </td>
                <td class='ee191' sheetid="1" rowid="45" colid="2"><div style="height:8pt;overflow:hidden"></div></td>
                <td class='ee191' sheetid="1" rowid="45" colid="3"><div style="height:8pt;overflow:hidden"></div></td>
              </tr>
              <tr style='height:8pt'>
                <td class='ee194' sheetid="1" rowid="46" colid="1"> Number
                  of preheats per day = 1<span> </span></td>
                <td class='ee191' sheetid="1" rowid="46" colid="2"><div style="height:8pt;overflow:hidden"></div></td>
                <td rowspan="4" class='ee191' sheetid="1" rowid="46" colid="3"><div style="overflow:hidden; padding-right:10px;" align="right"><a id="reset" onClick="reset_onclick('');" tabindex='10' style="font-size:10pt; font-family:Arial, Helvetica, sans-serif; font-weight:bold; color:#666; cursor:pointer;">Reset Calculator</a></div></td>
              </tr>
              <tr style='height:8pt'>
                <td class='ee194' sheetid="1" rowid="47" colid="1"> Cost per
                  Therm = $1 </td>
                <td class='ee191' sheetid="1" rowid="47" colid="2"><div style="height:8pt;overflow:hidden"><span class="ee194">PowerFry
                    Production Capacity = 103 lbs/hr * </span></div></td>
              </tr>
              <tr style='height:8pt'>
                <td class='ee194' sheetid="1" rowid="48" colid="1"> Days of
                  operation per year = 365 days </td>
                <td class='ee191' sheetid="1" rowid="48" colid="2"><div style="height:8pt;overflow:hidden"><span class="ee197">* Based
                    on ASTM Std. 30% weight loss +/-2 lbs /hr </span></div></td>
              </tr>
              <tr style='height:8pt'>
                <td class='ee194' sheetid="1" rowid="49" colid="1"> Number
                  of years in service = 10 years </td>
                <td class='ee191' sheetid="1" rowid="49" colid="2"><div style="height:8pt;overflow:hidden"><span class="ee197">** Above
                    Data Based on NSF third party testing </span></div></td>
              </tr> -->
            </table>
            <table>
            <!--<tr>
                <td><input type='image' src='image006.png' value='Submit' name='xl_submit_bottom' id='Image1' onClick="check_submit('nocaptcha');"  tabindex='12' height="34" width="136" border="0"/></td>
              </tr>-->
            </table>
          </div></td>
      </tr>
      <!--<tr>
<td><img src="/Vulcan/Products/VK45_Savings_Calculator/bg_end.jpg" width="820" height="20" border="0" style="display:block;" /></td>
</tr>-->
    </table>
  </div>
  
  <!--<div class="wizardbuttons mystyles">
<table border='0' align="left"><tr><td align="center">
<div style="background:url('support/btn-bg_boxed.png'); repeat-x left top;border-right:1px solid #C3C4C3;border-left:1px solid #C3C4C3;height:40px;">
<table border='0' width='98%'><tr style="height:38px;">
<td style="width:19%" valign="middle" align="left"><input class='buttonsBlue' type='button' value='Update' name='xl_update_bottom' id='btnUpdate_buttom' tabindex='9' onClick="recalc_onclick('');"  /></td>
<td style="width:19%" align="left"><input class='buttonsBlue' type='button' value='Reset' name='btnReset_buttom' id='btnReset_buttom' onClick="reset_onclick('');" tabindex='10'  /></td>
<td style="width:19%" align="left"><input class='buttonsBlue' type='button' value='Print' name='xl_print_bottom' id='btnPrint_buttom' onClick="changestylesheet('onesheet');" tabindex='11'  /></td>
<td style="width:5%">&nbsp;</td>
<td style="width:19%" align="right"><input class='buttonsGreen' type='button' value='Submit' name='xl_submit_bottom' id='btnSubmit_buttom' onClick="check_submit('nocaptcha');"  tabindex='12'/></td>
<td style="width:19%">&nbsp;
</tr></table></div></td>
<td style='width: 100%;text-align:right;'>
<span style='margin-left:20px;'><a href='http://www.spreadsheetconverter.com/excel-html-web.htm' target='_blank' tabindex='-1'><img style='vertical-align: bottom;' border='0' src='powered-by-spreadsheetconverter.png' width='50' height='46' title='Convert Excel spreadsheet to online form' alt='Powered By SpreadsheetConverter'></img></a></span>

</td></tr></table></div>-->
  <input type=hidden id="xl_aux2" name="xl_aux2" value="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">
  <input type=hidden id="xl_templateimages" name="xl_templateimages" value="1">
</form>
<!-- SpreadsheetConverter Body end --> 
<!--<noscript>The browser does not support JavaScript. The calculations created using  <a href='http://www.spreadsheetconverter.com/poweredby.htm' target='_blank'>SpreadsheetConverter</a>  will not work. Please access the web page using another browser.<p></p></noscript>--> 

<script language="JavaScript" type="text/javascript" src="support/internalfunctions.js"></script> 
<script language='javascript' type='text/javascript'>function setCookieValues(){var idvalue,idinput,elemtype,i,elemlength=document.formc.elements.length,localData,pageName,expDate;pageName=GetPageName();expDate=CookieExpiryDate(new Date,3);for(i=0;i<elemlength;i++){elemtype=document.formc.elements[i].type;if(elemtype=="text"||elemtype=="textarea"||elemtype=="hidden"){if(document.formc.elements[i].readOnly==false){idinput=document.formc.elements[i].id;if(idinput!="xl_aux2"){idvalue=document.formc.elements[i].value;YAHOO.util.Cookie.set(pageName+"_"+idinput,idvalue,{expires:expDate});localData+=pageName+"_"+idinput+","+idvalue+","+expDate+","+window.location+",No"}}}else if(elemtype=="select-one"){idvalue=document.formc.elements[i].selectedIndex;idinput=document.formc.elements[i].id;YAHOO.util.Cookie.set(pageName+"_"+idinput,idvalue,{expires:expDate});localData+=pageName+"_"+idinput+","+idvalue+","+expDate+","+window.location+",No"}else if(elemtype=="checkbox"||elemtype=="radio"){idvalue=document.formc.elements[i].checked?"yes":"no";idinput=document.formc.elements[i].id;if(idinput!="showagain"){YAHOO.util.Cookie.set(pageName+"_"+idinput,idvalue,{expires:expDate});localData+=pageName+"_"+idinput+","+idvalue+","+expDate+","+window.location+",No"}}if(localData.length>4096)break}}function getCookieValues(){for(var idinput,idvalue,elemtype,firstload="false",elemlength=document.formc.elements.length,i=0;i<elemlength;i++){elemtype=document.formc.elements[i].type;if(elemtype=="text"||elemtype=="textarea"||elemtype=="hidden"){if(document.formc.elements[i].readOnly==false){idinput=document.formc.elements[i].id;idvalue=YAHOO.util.Cookie.get(GetPageName()+"_"+idinput);if(idvalue==null){firstload="true";break}if(idvalue==GetPageName()+"_"+idinput)idvalue="";document.formc.elements[i].value=idvalue}}else if(elemtype=="select-one"){idinput=document.formc.elements[i].id;idvalue=YAHOO.util.Cookie.get(GetPageName()+"_"+idinput);document.formc.elements[i].selectedIndex=idvalue}else if(elemtype=="checkbox"||elemtype=="radio"){idinput=document.formc.elements[i].id;idvalue=YAHOO.util.Cookie.get(GetPageName()+"_"+idinput);if(idvalue=="yes")document.formc.elements[i].checked=true;else document.formc.elements[i].checked=false}}if(firstload=="true"){initial_update();eequerystring()}else{var querystring=document.location.search;if(querystring.length>0){variables=querystring.substring(1).split("&");variables.toString().toUpperCase().search("FORCE=TRUE")!=-1&&eequerystring()}}}</script><script language='javascript' type='text/javascript'>for(var row1xA58A83=new Array(26),jj=0;jj<26;jj++)row1xA58A83[jj]="";for(var row1xB58B83=new Array(26),jj=0;jj<26;jj++)row1xB58B83[jj]=0;for(var row1xC58C83=new Array(26),jj=0;jj<26;jj++)row1xC58C83[jj]=0;for(var row1xD58D83=new Array(26),jj=0;jj<26;jj++)row1xD58D83[jj]=0;for(var row1xE58E83=new Array(26),jj=0;jj<26;jj++)row1xE58E83[jj]=0;for(var row1xF58F83=new Array(26),jj=0;jj<26;jj++)row1xF58F83[jj]=0;for(var row1xG58G83=new Array(26),jj=0;jj<26;jj++)row1xG58G83[jj]=0;function calc(data){var c1B2=data.XLEW_1_2_2,c1B4=data.XLEW_1_4_2,c1B5=data.XLEW_1_5_2,c1B6=data.XLEW_1_6_2,c1C9=data.XLEW_1_9_3,c1C11=data.XLEW_1_11_3,c1C12=data.XLEW_1_12_3,c1C10=7560,c1C13=58.5,c1B17=1,c1B19=1,c1B20=365,c1B22=10;row1xA58A83[0]="Anets GoldenFRY **";row1xB58B83[0]=18.33;row1xC58C83[0]=20713;row1xD58D83[0]=12103;row1xE58E83[0]=93843;row1xF58F83[0]=60.5;row1xG58G83[0]=111000;row1xA58A83[1]="Dean D50G";row1xB58B83[1]=8.55;row1xC58C83[1]=15430;row1xD58D83[1]=14250;row1xE58E83[1]=97190;row1xF58F83[1]=70.5;row1xG58G83[1]=1.2e5;row1xA58A83[2]="Dean HD50 **";row1xB58B83[2]=7.58;row1xC58C83[2]=11117;row1xD58D83[2]=8386;row1xE58E83[2]=81513;row1xF58F83[2]=71.6;row1xG58G83[2]=1e5;row1xA58A83[3]="Dean HD60 **";row1xB58B83[3]=8.25;row1xC58C83[3]=16362;row1xD58D83[3]=10242;row1xE58E83[3]=60037;row1xF58F83[3]=66.2;row1xG58G83[3]=125000;row1xA58A83[4]="Dean SM50G";row1xB58B83[4]=8.45;row1xC58C83[4]=15430;row1xD58D83[4]=14250;row1xE58E83[4]=97190;row1xF58F83[4]=70.1;row1xG58G83[4]=1.2e5;row1xA58A83[5]="Dean SR52G";row1xB58B83[5]=8.5;row1xC58C83[5]=15430;row1xD58D83[5]=14250;row1xE58E83[5]=97190;row1xF58F83[5]=70.2;row1xG58G83[5]=1.2e5;row1xA58A83[6]="Imperial Elite 50lb Fryer";row1xB58B83[6]=8.2;row1xC58C83[6]=17430;row1xD58D83[6]=14250;row1xE58E83[6]=107190;row1xF58F83[6]=71;row1xG58G83[6]=1.4e5;row1xA58A83[7]="Infinity G2842 **";row1xB58B83[7]=9.2;row1xC58C83[7]=10660;row1xD58D83[7]=4010;row1xE58E83[7]=63000;row1xF58F83[7]=63.6;row1xG58G83[7]=75000;row1xA58A83[8]="Frymaster GF14";row1xB58B83[8]=8.5;row1xC58C83[8]=14117;row1xD58D83[8]=8386;row1xE58E83[8]=81513;row1xF58F83[8]=68;row1xG58G83[8]=1e5;row1xA58A83[9]="Frymaster H55 **";row1xB58B83[9]=10.5;row1xC58C83[9]=13951;row1xD58D83[9]=5604;row1xE58E83[9]=74254;row1xF58F83[9]=68.9;row1xG58G83[9]=8e4;row1xA58A83[10]="Frymaster MJ45";row1xB58B83[10]=8.45;row1xC58C83[10]=15430;row1xD58D83[10]=14250;row1xE58E83[10]=97190;row1xF58F83[10]=70.5;row1xG58G83[10]=122000;row1xA58A83[11]="Frymaster Protector **";row1xB58B83[11]=22;row1xC58C83[11]=9556;row1xD58D83[11]=4382;row1xE58E83[11]=67997;row1xF58F83[11]=67.7;row1xG58G83[11]=7e4;row1xA58A83[12]="Henny Penny EEG-14X **";row1xB58B83[12]=7.33;row1xC58C83[12]=8935;row1xD58D83[12]=5790;row1xE58E83[12]=68001;row1xF58F83[12]=64.1;row1xG58G83[12]=75000;row1xA58A83[13]="Henny Penny OFG-321 **";row1xB58B83[13]=10.7;row1xC58C83[13]=13755;row1xD58D83[13]=7040;row1xE58E83[13]=81235;row1xF58F83[13]=72;row1xG58G83[13]=85000;row1xA58A83[14]="Keating 14 IFM **";row1xB58B83[14]=7;row1xC58C83[14]=9730;row1xD58D83[14]=4700;row1xE58E83[14]=58700;row1xF58F83[14]=56;row1xG58G83[14]=79000;row1xA58A83[15]="Pitco AG14 **";row1xB58B83[15]=11.4;row1xC58C83[15]=11956;row1xD58D83[15]=4991;row1xE58E83[15]=61630;row1xF58F83[15]=58.4;row1xG58G83[15]=8e4;row1xA58A83[16]="Pitco 45C";row1xB58B83[16]=8.45;row1xC58C83[16]=15430;row1xD58D83[16]=14250;row1xE58E83[16]=97190;row1xF58F83[16]=70;row1xG58G83[16]=122000;row1xA58A83[17]="Pitco Frialator RPB14 **";row1xB58B83[17]=10.5;row1xC58C83[17]=13800;row1xD58D83[17]=5600;row1xE58E83[17]=71300;row1xF58F83[17]=62.3;row1xG58G83[17]=8e4;row1xA58A83[18]="Pitco SGH50 **";row1xB58B83[18]=8.25;row1xC58C83[18]=10275;row1xD58D83[18]=8510;row1xE58E83[18]=70653;row1xF58F83[18]=67;row1xG58G83[18]=8e4;row1xA58A83[19]="Pitco SG14R";row1xB58B83[19]=8.4;row1xC58C83[19]=15430;row1xD58D83[19]=14250;row1xE58E83[19]=97190;row1xF58F83[19]=70;row1xG58G83[19]=122000;row1xA58A83[20]="Pitco SSH55 **";row1xB58B83[20]=9.51;row1xC58C83[20]=11138;row1xD58D83[20]=8140;row1xE58E83[20]=77508;row1xF58F83[20]=71.8;row1xG58G83[20]=8e4;row1xA58A83[21]="Pitco VF35 **";row1xB58B83[21]=13.6;row1xC58C83[21]=9456;row1xD58D83[21]=7349;row1xE58E83[21]=67900;row1xF58F83[21]=58.7;row1xG58G83[21]=7e4;row1xA58A83[22]="Vulcan GR 35 **";row1xB58B83[22]=9.41;row1xC58C83[22]=13790;row1xD58D83[22]=12830;row1xE58E83[22]=87760;row1xF58F83[22]=62.8;row1xG58G83[22]=9e4;row1xA58A83[23]="Vulcan GR 45 **";row1xB58B83[23]=8.3;row1xC58C83[23]=15430;row1xD58D83[23]=14250;row1xE58E83[23]=97190;row1xF58F83[23]=71.8;row1xG58G83[23]=1.2e5;row1xA58A83[24]="Ultrafryer BP 20-18";row1xB58B83[24]=15.6;row1xC58C83[24]=16923;row1xD58D83[24]=5641;row1xE58E83[24]=65949;row1xF58F83[24]=67.9;row1xG58G83[24]=65000;row1xA58A83[25]="Ultrafryer Par 3-14";row1xB58B83[25]=15;row1xC58C83[25]=7450;row1xD58D83[25]=4180;row1xE58E83[25]=65370;row1xF58F83[25]=72.7;row1xG58G83[25]=9e4;var c1B9=hlookup_str(c1B2,row1xA58A83,0,25,row1xB58B83,0,25,false),c1B10=hlookup_str(c1B2,row1xA58A83,0,25,row1xC58C83,0,25,false),c1B11=hlookup_str(c1B2,row1xA58A83,0,25,row1xD58D83,0,25,false),c1B12=hlookup_str(c1B2,row1xA58A83,0,25,row1xE58E83,0,25,false),c1B13=hlookup_str(c1B2,row1xA58A83,0,25,row1xF58F83,0,25,false),c1B14=hlookup_str(c1B2,row1xA58A83,0,25,row1xG58G83,0,25,false),c1B16=c1B4,c1C17=c1B17,c1B18=c1B5,c1C19=c1B19,c1C20=c1B20,c1B21=c1B6,c1C22=c1B22,c1C16=c1B16,c1C18=c1B18,c1C21=c1B21,c1B24=1*c1B18/c1B13,c1C24=1*c1C18/c1C13,c1B26=c1B12*c1B24,c1C26=c1C12*c1C24,c1B28=c1B16-c1B24-c1B17*c1B9/60,c1C28=c1C16-c1C24-c1C17*c1C9/60,c1B30=c1B11*c1B28,c1C30=c1C11*c1C28,c1B32=c1B26+c1B30+c1B17*c1B10,c1C32=c1C26+c1C30+c1C17*c1C10,c1B34=c1B32/1e5,c1C34=c1C32/1e5,c1B36=c1B34*c1B19*c1B20,c1C36=c1C34*c1C19*c1C20,c1B37=c1B21*c1B36,c1C37=c1C21*c1C36,c1B39=c1B37*c1B22,c1C39=c1C37*c1C22,c1C38=c1B37-c1C37,c1C40=c1B39-c1C39;data.XLEW_1_9_2=c1B9;data.XLEW_1_11_2=c1B11;data.XLEW_1_12_2=c1B12;data.XLEW_1_14_2=c1B14;data.XLEW_1_36_2=c1B36;data.XLEW_1_36_3=c1C36;data.XLEW_1_38_3=c1C38;data.XLEW_1_39_2=c1B39;data.XLEW_1_39_3=c1C39;data.XLEW_1_40_3=c1C40}</script> 
<script language='javascript' type='text/javascript'>var eeisus=1,eetrue="TRUE",eefalse="FALSE",eedec=".",eeth=",",eedecreg=new RegExp("\\.","g"),eethreg=new RegExp(",","g"),eecurrencyreg=new RegExp("[$]","g"),eepercentreg=new RegExp("%","g"),fmtdaynamesshort=["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],fmtdaynameslong=["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],fmtmonthnamesshort=["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"],fmtmonthnameslong=["January","February","March","April","May","June","July","August","September","October","November","December"],fmtstrings=[","," ","$"],fmtdate6=[34,25,2],jsonLocal='{"eeisus":1,"eetrue":"TRUE","eefalse":"FALSE","eedec":".","eeth":",","eedecreg":["\\\\.","g"],"eethreg":[",","g"],"eecurrencyreg":["[$]","g"],"eepercentreg":["%","g"],"fmtdaynamesshort":["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],"fmtdaynameslong":["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],"fmtmonthnamesshort":["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"],"fmtmonthnameslong":["January","February","March","April","May","June","July","August","September","October","November","December"],"fmtstrings":[","," ","$"]}'</script> 
<script language='javascript' type='text/javascript'>var co={};function recalc_onclick(){if(true){co.XLEW_1_2_2=eegetdropdownvalue_str(document.formc.XLEW_1_2_2);co.XLEW_1_4_2=eeparseFloatTh(document.formc.XLEW_1_4_2.value);co.XLEW_1_5_2=eeparseFloatTh(document.formc.XLEW_1_5_2.value);co.XLEW_1_6_2=eeparseFloatTh(document.formc.XLEW_1_6_2.value);co.XLEW_1_9_3=eeparseFloat(document.formc.XLEW_1_9_3.value);co.XLEW_1_11_3=eeparseFloatTh(document.formc.XLEW_1_11_3.value);co.XLEW_1_12_3=eeparseFloatTh(document.formc.XLEW_1_12_3.value);calc(co);document.formc.XLEW_1_9_2.value=eedisplayFloat(co.XLEW_1_9_2);document.formc.XLEW_1_11_2.value=eedisplayFloatNDTh(co.XLEW_1_11_2,0);document.formc.XLEW_1_12_2.value=eedisplayFloatNDTh(co.XLEW_1_12_2,0);document.formc.XLEW_1_14_2.value=eedisplayFloatNDTh(co.XLEW_1_14_2,0);document.formc.XLEW_1_36_2.value=eedatefmt(fmtdate6,co.XLEW_1_36_2);document.formc.XLEW_1_36_3.value=eedatefmt(fmtdate6,co.XLEW_1_36_3);document.formc.XLEW_1_38_3.value=eedatefmt(fmtdate6,co.XLEW_1_38_3);document.formc.XLEW_1_39_2.value=eedatefmt(fmtdate6,co.XLEW_1_39_2);document.formc.XLEW_1_39_3.value=eedatefmt(fmtdate6,co.XLEW_1_39_3);document.formc.XLEW_1_40_3.value=eedatefmt(fmtdate6,co.XLEW_1_40_3)}}</script> 
<!-- library --> 
<script language='javascript' type='text/javascript'>function str_eq(x,y){if(isNaN(x)&&isNaN(y))return x.toLowerCase()==y.toLowerCase();else return x==y}function str_ls(x,y){if(isNaN(x)&&isNaN(y))return x.toLowerCase()<y.toLowerCase();else return x<y}function eegetdropdownvalue_str(ctl){return ctl.selectedIndex>=0&&ctl[ctl.selectedIndex]?ctl[ctl.selectedIndex].value:""}function myIsNaN(x){return isNaN(x)||typeof x=="number"&&!isFinite(x)}function mod(n,d){return n-d*Math.floor(n/d)}function round(n,nd){if(isFinite(n)&&isFinite(nd)){var sign_n=n<0?-1:1,abs_n=Math.abs(n),factor=Math.pow(10,nd);return sign_n*Math.round(abs_n*factor)/factor}else return NaN}function eeparseFloat(str){str=String(str).replace(eedecreg,".");var res=parseFloat(str);if(isNaN(res))return 0;else return res}var near0RegExp=new RegExp("[.](.*0000000|.*9999999)");function eedisplayFloat(x){if(myIsNaN(x))return Number.NaN;else{var str=String(x);if(near0RegExp.test(str)){x=round(x,8);str=String(x)}return str.replace(/\./g,eedec)}}function eedisplayScientific(x,nd){if(myIsNaN(x))return Number.NaN;else{var str=String(x.toExponential(nd));return str.replace(/\./g,eedec)}}function eedisplayFloatND(x,nd){if(myIsNaN(x))return Number.NaN;else{var res=round(x,nd);if(nd>0){var str=String(res);if(str.indexOf("e")!=-1)return str;if(str.indexOf("E")!=-1)return str;var parts=str.split(".");if(parts.length<2){var decimals="00000000000000".substring(0,nd);return parts[0].toString()+eedec+decimals}else{var decimals=(parts[1].toString()+"00000000000000").substring(0,nd);return parts[0].toString()+eedec+decimals}}else return res}}function eedisplayPercent(x){if(myIsNaN(x))return Number.NaN;else return eedisplayFloat(x*100)+"%"}function eedisplayPercentND(x,nd){if(myIsNaN(x))return Number.NaN;else return eedisplayFloatND(x*100,nd)+"%"}function eeparseFloatTh(str){str=String(str).replace(eethreg,"");str=String(str).replace(eedecreg,".");var res=parseFloat(str);if(isNaN(res))return 0;else return res}function eedisplayFloatNDTh(x,nd){if(myIsNaN(x))return Number.NaN;else{var res=round(x,nd);if(nd>0){var str=String(res);if(str.indexOf("e")!=-1)return str;if(str.indexOf("E")!=-1)return str;var parts=str.split("."),res2=eeinsertThousand(parts[0].toString());if(parts.length<2){var decimals="00000000000000".substring(0,nd);return res2+eedec+decimals}else{var decimals=(parts[1].toString()+"00000000000000").substring(0,nd);return res2+eedec+decimals}}else return eeinsertThousand(res.toString())}}function eedisplayPercentNDTh(x,nd){if(myIsNaN(x))return Number.NaN;else return eedisplayFloatNDTh(x*100,nd)+"%"}function eeinsertThousand(whole){if(whole==""||whole.indexOf("e")>=0)return whole;else{var minus_sign="";if(whole.charAt(0)=="-"){minus_sign="-";whole=whole.substring(1)}for(var res="",str_length=whole.length-1,ii=0;ii<=str_length;ii++){if(ii>0&&ii%3==0)res=eeth+res;res=whole.charAt(str_length-ii)+res}return minus_sign+res}}function eedatefmt(fmt,x){if(!isFinite(x))return Number.NaN;for(var padding=0,tmp=0,res="",len=fmt.length,ii=0;ii<len;ii++)if(fmt[ii]>31)res+=fmtstrings[fmt[ii]-32];else switch(fmt[ii]){case 2:res+=eemonth(x);break;case 3:tmp=eemonth(x);if(tmp<10)res+="0";res+=tmp;break;case 4:res+=fmtmonthnamesshort[eemonth(x)-1];break;case 5:res+=fmtmonthnameslong[eemonth(x)-1];break;case 6:res+=eeday(x);break;case 7:tmp=eeday(x);if(tmp<10)res+="0";res+=tmp;break;case 8:res+=fmtdaynamesshort[weekday(x,1)-1];break;case 9:res+=fmtdaynameslong[weekday(x,1)-1];break;case 10:tmp=year(x)%100;if(tmp<10)res+="0";res+=tmp;break;case 11:res+=year(x);break;case 12:res+=hour(x);break;case 13:tmp=hour(x);if(tmp<10)res+="0";res+=tmp;break;case 14:tmp=hour(x)%12;if(tmp==0)res+="12";else res+=tmp%12;break;case 15:tmp=hour(x)%12;if(tmp==0)res+="12";else{if(tmp<10)res+="0";res+=tmp}break;case 16:res+=minute(x);break;case 17:tmp=minute(x);if(tmp<10)res+="0";res+=tmp;break;case 18:res+=second(x);break;case 19:tmp=second(x);if(tmp<10)res+="0";res+=tmp;break;case 21:case 22:if(hour(x)<12)res+="AM";else res+="PM";break;case 23:res+=eedisplayFloat(x);break;case 24:tmp=fmt[++ii];res+=eedisplayFloatND(x,tmp);break;case 25:tmp=fmt[++ii];res+=eedisplayFloatNDTh(x,tmp);break;case 26:res+=eedisplayPercent(x);break;case 27:tmp=fmt[++ii];res+=eedisplayPercentND(x,tmp);break;case 28:tmp=fmt[++ii];res+=eedisplayPercentNDTh(x,tmp);break;case 29:tmp=fmt[++ii];res+=eedisplayScientific(x,tmp);break;case 30:padding=fmt[++ii];tmp=hour(x)+Math.floor(x)*24;tmp=tmp.toString();if(tmp.length<padding)res+="00000000000000".substring(0,padding-tmp.length);res+=tmp}return res}function isna(x){if(typeof x=="number")return isNaN(x);else return false}function hlookup_str(key,kvect,kfrom_start,kto_start,vvect,vfrom_,vto_,range_lookup){if(isna(key))return Number.NaN;if(range_lookup)return lookup3vv_str(key,kvect,kfrom_start,kto_start,vvect,vfrom_,vto_);else{for(var ii=kfrom_start;ii<=kto_start;ii++)if(str_eq(kvect[ii],key))return vvect[vfrom_+ii-kfrom_start];return Number.NaN}}function lookup3vv_str(key,kvect,kfrom_start,kto_start,vvect,vfrom_){if(isna(key))return Number.NaN;var current=0,from_=kfrom_start,to_=kto_start+1;while(true){current=from_+to_>>1;if(str_eq(kvect[current],key))break;if(from_==to_-1)break;if(str_ls(kvect[current],key))from_=current;else to_=current}while(current<kto_start)if(str_eq(kvect[current],kvect[current+1]))current++;else break;if(str_ls(key,kvect[current]))return Number.NaN;return vvect[vfrom_+current-kfrom_start]}function leap_gregorian(year){return year%4==0&&!(year%100==0&&year%400!=0)}var GREGORIAN_EPOCH=1721425;function gregorian_to_jd(year,month,day){return GREGORIAN_EPOCH-0+365*(year-1)+Math.floor((year-1)/4)+-Math.floor((year-1)/100)+Math.floor((year-1)/400)+Math.floor((367*month-362)/12+(month<=2?0:leap_gregorian(year)?-1:-2)+day)}function jd_to_gregorian(jd){var wjd,depoch,quadricent,dqc,cent,dcent,quad,dquad,yindex,year,yearday,leapadj;wjd=Math.floor(jd);depoch=wjd-GREGORIAN_EPOCH-1;quadricent=Math.floor(depoch/146097);dqc=mod(depoch,146097);cent=Math.floor(dqc/36524);dcent=mod(dqc,36524);quad=Math.floor(dcent/1461);dquad=mod(dcent,1461);yindex=Math.floor(dquad/365);year=quadricent*400+cent*100+quad*4+yindex;if(!(cent==4||yindex==4))year++;yearday=wjd-gregorian_to_jd(year,1,1);leapadj=wjd<gregorian_to_jd(year,3,1)?0:leap_gregorian(year)?1:2;var month=Math.floor(((yearday+leapadj)*12+373)/367),day=wjd-gregorian_to_jd(year,month,1)+1;return [year,month,day]}function eeday(serial_number){if(!isFinite(serial_number))return Number.NaN;if(serial_number<1)return 0;if(serial_number>60)serial_number--;var res=jd_to_gregorian(serial_number+2415020);return res[2]}function hour(serial_number){if(!isFinite(serial_number))return Number.NaN;var res=Math.floor((serial_number-Math.floor(serial_number))*86400+.5);return Math.floor(res/3600)}function minute(serial_number){if(!isFinite(serial_number))return Number.NaN;var res=Math.floor((serial_number-Math.floor(serial_number))*86400+.5);return Math.floor(res/60)%60}function eemonth(serial_number){if(!isFinite(serial_number))return Number.NaN;if(serial_number<1)return 1;if(serial_number>60)serial_number--;var res=jd_to_gregorian(serial_number+2415020);return res[1]}function second(serial_number){if(!isFinite(serial_number))return Number.NaN;var res=Math.floor((serial_number-Math.floor(serial_number))*86400+.5);return res%60}function weekday(serial_number,return_type){if(!isFinite(return_type)||!isFinite(serial_number))return Number.NaN;if(return_type<1||return_type>3)return Number.NaN;var res=Math.floor(serial_number+6)%7;switch(Math.floor(return_type)){case 1:return res+1;case 2:return (res+6)%7+1;case 3:return (res+6)%7}return "hej"}function year(serial_number){if(!isFinite(serial_number))return Number.NaN;if(serial_number<1)return 1900;if(serial_number>60)serial_number--;var res=jd_to_gregorian(serial_number+2415020);return res[0]}</script> 
<script language='javascript' type='text/javascript'>function postcode(){}</script> 
<script language='javascript' type='text/javascript'>var reqlist=[];(function(){var tabView=new YAHOO.widget.TabView("demo");YAHOO.log("The example has finished loading, as you interact with it, you will see log messages appearing here.","info","example")})();var submitted=false;function check_submit(objcaptcha){var pagevalues,fadediv,lightalertdiv;if(checkrequired(reqlist,"nopanel")===true){pagevalues=getPageSize();fadediv=document.getElementById("fadealert");lightalertdiv=document.getElementById("lightalert");fadediv.style.height=pagevalues[1];fadediv.style.display="block";lightalert.style.top=pagevalues[3]/2-125/2+pagevalues[4];lightalert.style.left=pagevalues[0]/2-320/2;lightalert.style.display="block";document.getElementById("btnCaptchaalert").focus()}else if(objcaptcha=="nocaptcha")if(submitted){alert("You have already submitted the form.  Please be patient.");submitted=false;return false}else{recalc_onclick("");submitted=true;document.formc.submit();return true}else{recalc_onclick("");pagevalues=getPageSize();fadediv=document.getElementById("fade");lightalertdiv=document.getElementById("light");fadediv.style.height=pagevalues[1];fadediv.style.display="block";lightalertdiv.style.top=pagevalues[3]/2-200/2+pagevalues[4];lightalertdiv.style.left=pagevalues[0]/2-320/2;lightalertdiv.style.display="block";GenerateCaptID();document.getElementById("txtcaptcha").value="";document.getElementById("txtcaptcha").focus()}}YAHOO.example.init=function(){function onButtonClickReset(){reset_onclick("")}function onButtonClickUpdate(){recalc_onclick("")}function onButtonClickPrint(){window.print()}function onButtonClickSubmit(){setCookieValues();check_submit()}var oPushButton1,oPushButton2,oPushButton3,oPushButton4,oPushButton5,oPushButton6,oPushButton7,oPushButton8;YAHOO.util.Event.onContentReady("submitbuttonsfrommarkup",function(){oPushButton1=new YAHOO.widget.Button("btnReset");oPushButton1.on("click",onButtonClickReset);oPushButton2=new YAHOO.widget.Button("btnUpdate");oPushButton2.on("click",onButtonClickUpdate);oPushButton3=new YAHOO.widget.Button("btnPrint");oPushButton3.on("click",onButtonClickPrint);oPushButton5=new YAHOO.widget.Button("btnPrintAll");oPushButton5.on("click",onButtonClickPrint);oPushButton4=new YAHOO.widget.Button("btnSubmit");oPushButton4.on("click",onButtonClickSubmit)});YAHOO.util.Event.onContentReady("divbottom",function(){oPushButton5=new YAHOO.widget.Button("btnReset_buttom");oPushButton5.on("click",onButtonClickReset);oPushButton6=new YAHOO.widget.Button("btnUpdate_buttom");oPushButton6.on("click",onButtonClickUpdate);oPushButton7=new YAHOO.widget.Button("btnPrint_buttom");oPushButton7.on("click",onButtonClickPrint);oPushButton9=new YAHOO.widget.Button("btnPrintAll_buttom");oPushButton9.on("click",onButtonClickPrint);oPushButton8=new YAHOO.widget.Button("btnSubmit_buttom");oPushButton8.on("click",onButtonClickSubmit)})}();function reset_onclick(){document.formc.reset();postcode();recalc_onclick("");ResetRatings()}function eequerystring(){var variable,key,value,ii,querystring,variables;querystring=document.location.search;if(querystring.length>0){variables=querystring.substring(1).split("&");for(ii=0;ii<variables.length;ii++){variable=variables[ii].split("=");key=unescape(variable[0]);value=unescape(variable[1]);if(document.formc[key]!=null)document.formc[key].value=value}}}function initial_update(){postcode("");eequerystring();recalc_onclick("")}</script><script type="text/javascript" language="javascript">
function showPanel(){var id=parseInt(document.getElementById("panelidz").value)-1;jQuery(".yui-nav li").each(function(i){jQuery(".yui-nav li").eq(i).removeClass("selected");});jQuery(".yui-nav li").eq(id).addClass("selected");var myTabs = new YAHOO.widget.TabView("demo");myTabs.set('activeIndex',id);}</script> 
<script language="javascript" type="text/javascript">
function setValueofBtn()
{

}</script> 
<script type="text/javascript" language="javascript">
function ResetRatings(){
}
function SetSliders(){
}</script> 
<script language='javascript' type='text/javascript'>try{var myDialog,handleSubmit;myDialog=new YAHOO.widget.Dialog("myDialog",{width:"330px",fixedcenter:true,draggable:false,close:false});myDialog.cfg.queueProperty("postmethod","form");handleSubmit=function(){if(document.getElementById("showagain")!=null)document.getElementById("showagain").checked==true&&YAHOO.util.Cookie.set(GetPageName()+"_showdialogagain","no",{expires:CookieExpiryDate(new Date,3)});myDialog.hide()};function onButtonsReady(){var oOKButton;oOKButton=new YAHOO.widget.Button("ok-button");function onButtonClick(){if(document.getElementById("showagain")!=null)document.getElementById("showagain").checked==true&&YAHOO.util.Cookie.set(GetPageName()+"_showdialogagain","no",{expires:CookieExpiryDate(new Date,3)});myDialog.hide()}oOKButton.addListener("click",onButtonClick)}YAHOO.util.Event.onContentReady("buttons",onButtonsReady);myDialog.render();var cookievalue=YAHOO.util.Cookie.get(GetPageName()+"_showdialogagain");if(cookievalue==null||cookievalue!="no"){document.getElementById("showagain")!=null&&document.getElementById("showagain").focus();myDialog.show()}else myDialog.hide()}catch(err){}function changestylesheet(sheets){recalc_onclick("");if(sheets=="allsheet")document.getElementById("default").href="support/print_friendly.css";else document.getElementById("default").href="";window.print()}</script>

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