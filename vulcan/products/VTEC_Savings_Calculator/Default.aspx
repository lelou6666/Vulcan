<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<TITLE>VTEC Savings Calculator</TITLE>
<META http-equiv=Content-Type content=text/html;charset=UTF-8>
<META content="MSHTML 6.00.6001.18565" name=GENERATOR>
<!-- SpreadsheetConverter Header start --><!-- Parts of this page Copyright (C) 2002-2011 Framtidsforum I&M AB, Sweden -->
<STYLE type=text/css>
*.ee100 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 1px;
	FONT-WEIGHT: 400;
	FONT-SIZE: 9pt;
	VERTICAL-ALIGN: top;
	COLOR: white;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: left
}
*.ee103 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 1px;
	FONT-WEIGHT: 700;
	FONT-SIZE: 18pt;
	BACKGROUND: black;
	VERTICAL-ALIGN: middle;
	COLOR: white;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: center
}
*.ee109 {
	FONT-SIZE: 14px;
	VERTICAL-ALIGN: middle;
	COLOR: black;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: left;
}
*.ee112 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 1px;
	FONT-WEIGHT: 400;
	FONT-SIZE: 14x;
	VERTICAL-ALIGN: middle;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: left;
}
*.ee115 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 1px;
	FONT-WEIGHT: 400;
	FONT-SIZE: 11pt;
	VERTICAL-ALIGN: middle;
	COLOR: black;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: center
}
*.ee118 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 1px;
	FONT-WEIGHT: 700;
	FONT-SIZE: 11pt;
	BACKGROUND: #7f7f7f;
	VERTICAL-ALIGN: middle;
	COLOR: white;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: left
}
*.ee121 {
	FONT-WEIGHT: 700;
	FONT-SIZE: 11pt;
	COLOR: white;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-DECORATION: none
}
*.ee122 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 0px;
	FONT-WEIGHT: 700;
	FONT-SIZE: 12pt;
	VERTICAL-ALIGN: middle;
	COLOR: blue;
	PADDING-TOP: 0px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: center
}
*.ee124 {
	FONT-WEIGHT: 700;
	FONT-SIZE: 12pt;
	BACKGROUND: white;
	VERTICAL-ALIGN: middle;
	COLOR: blue;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: center
}
*.ee125 {
	font-size: 17px;
	VERTICAL-ALIGN: middle;
	COLOR: white;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: center
}
*.ee127 {
	font-size: 17px;
	VERTICAL-ALIGN: middle;
	COLOR: white;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: center
}
*.ee128, *.ee143, *.ee151   {
	PADDING-RIGHT: 10px;
	FONT-SIZE: 12px;
	VERTICAL-ALIGN: middle;
	FONT-FAMILY: Arial;
	TEXT-ALIGN: right;
}

*.ee151, *.ee154, *.ee155
{
    color: #cc0000;
    font-weight: bold;
}

*.ee131 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 0px;
	FONT-WEIGHT: 700;
	FONT-SIZE: 16pt;
	VERTICAL-ALIGN: middle;
	COLOR: blue;
	PADDING-TOP: 0px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: center
}
*.ee133 {
	FONT-WEIGHT: 700;
	FONT-SIZE: 16pt;
	VERTICAL-ALIGN: middle;
	COLOR: blue;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: center
}
*.ee134 {
	FONT-WEIGHT: 700;
	FONT-SIZE: 16pt;
	COLOR: blue;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif
}
*.ee137 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 0px;
	FONT-WEIGHT: 700;
	FONT-SIZE: 12pt;
	VERTICAL-ALIGN: middle;
	COLOR: blue;
	PADDING-TOP: 0px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: center
}
*.ee138 {
	FONT-WEIGHT: 700;
	FONT-SIZE: 12pt;
	VERTICAL-ALIGN: middle;
	COLOR: blue;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: center
}
*.ee139 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 1px;
	FONT-WEIGHT: 700;
	FONT-SIZE: 12pt;
	VERTICAL-ALIGN: middle;
	COLOR: blue;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: left
}
*.ee142 {
	FONT-WEIGHT: 400;
	FONT-SIZE: 11pt;
	COLOR: blue;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-DECORATION: none
}


*.ee148 {
	FONT-WEIGHT: 400;
	FONT-SIZE: 9pt;
	COLOR: gray;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-DECORATION: none
}


*.ee150, *.ee146, *.ee155
{
    border:none;
    background-color:Transparent;
}

*.ee145, *.ee147, *.ee149, *.ee154, *.ee156     {
	FONT-SIZE: 19px;
	font-family:Arial;
	VERTICAL-ALIGN: middle;
	FONT-FAMILY: Arial, sans-serif;
	border-top: solid 1px #cfcfcf;
    border-right: solid 1px #cfcfcf;
    border-left: solid 1px #cfcfcf;
    border-bottom: solid 1px #cfcfcf;
    background: #f5f5f5;
    padding: 9px 4px 7px 9px;
	text-align : left;
	width: 270px;
}

*.ee157 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 1px;
	FONT-WEIGHT: 400;
	FONT-SIZE: 6.6pt;
	VERTICAL-ALIGN: super;
	COLOR: black;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: left
}
*.ee160 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 1px;
	FONT-WEIGHT: 400;
	FONT-SIZE: 11pt;
	VERTICAL-ALIGN: bottom;
	COLOR: black;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: left
}
*.ee163 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 1px;
	FONT-WEIGHT: 400;
	FONT-SIZE: 11pt;
	VERTICAL-ALIGN: middle;
	COLOR: black;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: center
}
*.ee165 {
	FONT-WEIGHT: 400;
	FONT-SIZE: 11pt;
	COLOR: black;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif
}
*.ee167 {
	FONT-WEIGHT: 400;
	FONT-SIZE: 11pt;
	COLOR: white;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-DECORATION: none
}
*.ee168 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 1px;
	FONT-WEIGHT: 400;
	FONT-SIZE: 11pt;
	VERTICAL-ALIGN: middle;
	COLOR: black;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: left
}
*.ee169 {
	PADDING-RIGHT: 1px;
	PADDING-LEFT: 1px;
	FONT-WEIGHT: 400;
	FONT-SIZE: 11pt;
	VERTICAL-ALIGN: middle;
	COLOR: black;
	PADDING-TOP: 1px;
	FONT-STYLE: normal;
	FONT-FAMILY: Arial, sans-serif;
	TEXT-ALIGN: right
}
TEXTAREA {
	OVERFLOW: auto
}
</STYLE>
<STYLE type=text/css>
OBJECT {
	BORDER-RIGHT: rgb(113,114,115) 1px solid;
	BORDER-TOP: rgb(113,114,115) 1px solid;
	BORDER-LEFT: rgb(113,114,115) 1px solid;
	BORDER-BOTTOM: rgb(113,114,115) 1px solid
}
</STYLE>
<SCRIPT 
src="jquery.js" 
type=text/javascript></SCRIPT>
<SCRIPT>jQuery(document).ready(function(){
                    jQuery('#loading').fadeOut(500);});</SCRIPT>
<LINK href="YUI/fonts-min.css" type=text/css rel=stylesheet>
<SCRIPT 
src="yahoo-dom-event.js" 
type=text/javascript></SCRIPT>
<SCRIPT 
src="yahoo-min.js" 
type=text/javascript></SCRIPT>
<SCRIPT 
src="cookie-beta-min.js" 
type=text/javascript></SCRIPT>
<SCRIPT 
src="cookie-min.js" 
type=text/javascript></SCRIPT>
<LINK href="YUI/tabview.css" type=text/css rel=stylesheet>
<SCRIPT 
src="element-beta-min.js" 
type=text/javascript></SCRIPT>
<SCRIPT 
src="tabview-min.js" 
type=text/javascript></SCRIPT>
<LINK href="YUI/container.css" type=text/css rel=stylesheet>
<SCRIPT 
src="container-min.js" 
type=text/javascript></SCRIPT>
<STYLE type=text/css>
.msg_head {
	PADDING-RIGHT: 10px;
	BACKGROUND-POSITION: right 50%;
	PADDING-LEFT: 10px;
	FONT-WEIGHT: bold;
	PADDING-BOTTOM: 5px;
	MARGIN: 1px;
	CURSOR: pointer;
	PADDING-TOP: 5px;
	BACKGROUND-REPEAT: no-repeat;
	POSITION: relative;
	BACKGROUND-COLOR: #e9e7e7
}
.msg_body {
	BORDER-RIGHT: #e9e7e7 1px solid;
	PADDING-RIGHT: 10px;
	BORDER-TOP: #e9e7e7 1px solid;
	PADDING-LEFT: 10px;
	PADDING-BOTTOM: 15px;
	BORDER-LEFT: #e9e7e7 1px solid;
	PADDING-TOP: 5px;
	BORDER-BOTTOM: #e9e7e7 1px solid
}
.msg_list {
	PADDING-RIGHT: 0px;
	PADDING-LEFT: 0px;
	PADDING-BOTTOM: 0px;
	MARGIN: 0px;
	WIDTH: 753px;
	PADDING-TOP: 0px
}
</STYLE>
<STYLE type=text/css media=print>
.mystyles {
	DISPLAY: none
}
.printclass {
	DISPLAY: block
}
*.ee100 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee103 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee105 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee106 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee108 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee109 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee110 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee111 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee113 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee117 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee118 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee120 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee121 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
*.ee122 {
	BORDER-RIGHT: #000000 0px solid;
	BORDER-TOP: #000000 0px solid;
	BORDER-LEFT: #000000 0px solid;
	BORDER-BOTTOM: #000000 0px solid
}
.eebuttonbar_top {
	DISPLAY: none! important
}
.eebuttonbar_bottom {
	DISPLAY: none! important
}
.eetabs {
	DISPLAY: none! important
}
.wizardbuttons {
	DISPLAY: none! important
}
.buttonPage {
	DISPLAY: none! important
}
.buttonPage {
	DISPLAY: none! important
}
.buttonFinish {
	DISPLAY: none! important
}
.buttonCancel {
	DISPLAY: none! important
}
.yui-content .highlighted {
	DISPLAY: none! important
}
.yui-content .dimmed {
	DISPLAY: none! important
}
DIV {
	BORDER-RIGHT: medium none;
	BORDER-TOP: medium none;
	BORDER-LEFT: medium none;
	BORDER-BOTTOM: medium none
}
.msg_body {
	BACKGROUND-COLOR: white! important
}
.accordion H3 {
	DISPLAY: none! important
}
.accordion P {
	BORDER-RIGHT: medium none;
	BORDER-TOP: medium none;
	BORDER-LEFT: medium none;
	BORDER-BOTTOM: medium none
}
DIV P {
	DISPLAY: none! important
}
</STYLE>
<LINK href="support/ssc_styles.css" type=text/css rel=stylesheet>
<link rel='stylesheet' type='text/css' href='main.css' />
<LINK 
id=default media=print href="" type=text/css rel=stylesheet>
<LINK 
href="support/wizardstyle.css" type=text/css rel=stylesheet>
<STYLE type=text/css media=print>
.button {
	DISPLAY: none
}
.dactive {
	DISPLAY: none
}
.button {
	DISPLAY: none
}
.dactive {
	DISPLAY: none
}

</STYLE>
<STYLE type=text/css>
.calc_model 
{
    margin: 0 8px 10px 10px;
}


#container_calc 
{
    width:854px;
}

.wizardbuttons 
{
     width:900px; 
     margin:0 auto;
}
</STYLE>
<SCRIPT>
var isSheet0FirstDisplay = true;
</SCRIPT>
<SCRIPT>var SheetCollection = new Array();
SheetCollection[0] = new Array();
</SCRIPT>
<!-- SpreadsheetConverter Header end -->
</head>
<BODY class=yui-skin-sam 
onclick="if(document.getElementById('showagain')!=null){if(document.getElementById('showagain').checked){}else{myDialog.hide();}}" 
onload='getCookieValues();SetSliders();DisplayCurrentPanel();postcode();recalc_onclick("obj");setValueofBtn();ResetRatings();' 
onunload=setCookieValues();>
<INPUT id=xl_spreadsheet type=hidden value="VTEC Savings Calculator 10-26-2011-conversion test.xlsx" name=xl_spreadsheet>
<INPUT id=xl_client type=hidden value=h5.2.29 name=xl_client>
<INPUT id=currentpanel type=hidden>
<INPUT id=panelidz type=hidden>
</span><font> 
<!--hidden fields for success,failure and cancel urls.--> 
</font></font>
<FORM id=formc name=formc action="" method=post>
  <div style="width:900px; margin:0 auto;">
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td colspan="3" align="left">
                <div id="container_calc">
                    <div id="calc_header_container"><img src="calc_header.png" width="719" height="72" alt="" /></div>
                </div>
                    <div id="calc_options_container">
	                    <div id="calc_options_header_container">
	                        <div style="position:relative; float:left; font-size:14px; font-weight:bold; margin-left:43px;">Hours of Operation/day (hr)</div>
	                        <div style="position:relative; float:left; font-size:14px; font-weight:bold; margin-left:85px;">Cost per 1 therm ($/therm) 2</div>
	                        <br />
	                    </div>
	        
                      <div class="clearfix">
  	                    <div class="calc_option_input calc_op_time">
  		                    <INPUT 
                          id=XLEW_1_5_2 onKeyDown="onEnterPress('XLEW_1_5_2');" 
                          style="PADDING-RIGHT: 1px; width:200px;" tabIndex=4 
                          onchange="this.value=eedisplayFloatND(eeparseFloat(this.value),0);recalc_onclick('XLEW_1_5_2') ;setValueofBtn();" 
                          value=10 name=XLEW_1_5_2 downkey="XLEW_1_6_2">
                          </div>
  	                    <div class="calc_option_input calc_avg">
  		                 <INPUT 
                          id=XLEW_1_6_2 onKeyDown="onEnterPress('XLEW_1_6_2');" 
                          style="PADDING-RIGHT: 1px; width:210px;" tabIndex=5 
                          onchange="this.value=eedisplayFloatNDTh(eeparseFloatTh(this.value),2);recalc_onclick('XLEW_1_6_2') ;setValueofBtn();" 
                          value=1.00 name=XLEW_1_6_2 upkey="XLEW_1_5_2">
  	                    </div>
                      </div>
                   </div>
                </div>
              <br />
             <TABLE style="BORDER-COLLAPSE: collapse" cellPadding=3 width=683 align="center"  cellpadding="0" cellspacing="0">
                <tr style="HEIGHT: 25pt">
                  <td class=ee122 colid="2" rowid="3" sheetid="1"><FIELDSET id=XLEW_1_3_2ssc 
              style="BORDER-RIGHT: 0px; PADDING-RIGHT: 0px; BORDER-TOP: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; BORDER-LEFT: 0px; PADDING-TOP: 0px; BORDER-BOTTOM: 0px">
                      <font face="Arial"><span style=""><font>
                      <SELECT 
              class="calc_model" id=XLEW_1_3_2 style="" tabIndex=2 
              onchange="recalc_onclick('XLEW_1_3_2')" size=1 name=XLEW_1_3_2>
                        <OPTION 
                value="American Range: ADJ-xx" selected>American Range: ADJ-xx</OPTION>
                        <OPTION value="American Range: AECB-xx">American Range: AECB-xx</OPTION>
                        <OPTION value="American Range: ARKB-xx">American Range: ARKB-xx</OPTION>
                        <OPTION value="APW Wyott: GCB-xxH">APW Wyott: GCB-xxH</OPTION>
                        <OPTION value="APW Wyott: GCRB-xxH">APW Wyott: GCRB-xxH</OPTION>
                        <OPTION value="APW Wyott: HCB-xx24">APW Wyott: HCB-xx24</OPTION>
                        <OPTION value="APW Wyott: HCRB-xx24">APW Wyott: HCRB-xx24</OPTION>
                        <OPTION value="Bakers Pride: C-xxGS">Bakers Pride: C-xxGS</OPTION>
                        <OPTION value="Bakers Pride: C-xxR">Bakers Pride: C-xxR</OPTION>
                        <OPTION value="Bakers Pride: HDCB-24xx">Bakers Pride: 
                        HDCB-24xx</OPTION>
                        <OPTION value="Bakers Pride: HDCRB-24xx">Bakers Pride: 
                        HDCRB-24xx</OPTION>
                        <OPTION value="Bakers Pride: L-xxGS">Bakers Pride: L-xxGS</OPTION>
                        <OPTION value="Bakers Pride: L-xxR">Bakers Pride: L-xxR</OPTION>
                        <OPTION value="Bakers Pride: XX-xx">Bakers Pride: XX-xx</OPTION>
                        <OPTION value="Comstock Castle: 32xxLB">Comstock Castle: 
                        32xxLB</OPTION>
                        <OPTION value="Comstock Castle: 32xxRB">Comstock Castle: 
                        32xxRB</OPTION>
                        <OPTION value="Comstock Castle: ELBxx">Comstock Castle: ELBxx</OPTION>
                        <OPTION value="Comstock Castle: ERBxx">Comstock Castle: ERBxx</OPTION>
                        <OPTION value="Comstock Castle: FHPxx-2LB">Comstock Castle: 
                        FHPxx-2LB</OPTION>
                        <OPTION 
                value="Comstock Castle: FHPxx-2RBB">Comstock Castle: FHPxx-2RBB</OPTION>
                        <OPTION value="Connerton: CRB-xx-C">Connerton: CRB-xx-C</OPTION>
                        <OPTION value="Connerton: LRB-xx-C">Connerton: LRB-xx-C</OPTION>
                        <OPTION value="Eagle: CLCHRB-xx-NG">Eagle: CLCHRB-xx-NG</OPTION>
                        <OPTION value="Eagle: CLCHRB-xx-NG-X">Eagle: CLCHRB-xx-NG-X</OPTION>
                        <OPTION value="Garland: Gxx-BRL">Garland: Gxx-BRL</OPTION>
                        <OPTION value="Garland: GD-xxRB">Garland: GD-xxRB</OPTION>
                        <OPTION value="Garland: GD-xxRBFF">Garland: GD-xxRBFF</OPTION>
                        <OPTION value="Garland: GFxx-BRL">Garland: GFxx-BRL</OPTION>
                        <OPTION value="Garland: GTBGxx-ABxx">Garland: GTBGxx-ABxx</OPTION>
                        <OPTION value="Garland: GTBGxx-ARxx">Garland: GTBGxx-ARxx</OPTION>
                        <OPTION value="Garland: GTBGxx-NRxx">Garland: GTBGxx-NRxx</OPTION>
                        <OPTION value="Garland: HEEGxxCL">Garland: HEEGxxCL</OPTION>
                        <OPTION value="Garland: UTBGxx-ABxx">Garland: UTBGxx-ABxx</OPTION>
                        <OPTION value="Globe: GCBxxG">Globe: GCBxxG</OPTION>
                        <OPTION value="Globe: GCBRKxxK">Globe: GCBRKxxK</OPTION>
                        <OPTION value="Imperial: IABR-xx">Imperial: IABR-xx</OPTION>
                        <OPTION value="Imperial: IABS-xx">Imperial: IABS-xx</OPTION>
                        <OPTION value="Imperial: IRB-xx">Imperial: IRB-xx</OPTION>
                        <OPTION value="Jade: JB-xx">Jade: JB-xx</OPTION>
                        <OPTION 
                value="Jade: KC-xx">Jade: KC-xx</OPTION>
                        <OPTION 
                value="Lang: 21xxZRCB">Lang: 21xxZRCB</OPTION>
                        <OPTION 
                value="LoLo: LCBxxRMPF">LoLo: LCBxxRMPF</OPTION>
                        <OPTION 
                value="MagiKitchn: APL-RMB-6xx">MagiKitchn: APL-RMB-6xx</OPTION>
                        <OPTION 
                value="MagiKitchn: APL-RMB-6xx-H">MagiKitchn: APL-RMB-6xx-H</OPTION>
                        <OPTION 
                value="MagiKitchn: APL-SMB-6xx">MagiKitchn: APL-SMB-6xx</OPTION>
                        <OPTION 
                value="MagiKitchn: APL-SMB-6xx-H">MagiKitchn: APL-SMB-6xx-H</OPTION>
                        <OPTION 
                value="MagiKitchn: APM-RMB-6xx">MagiKitchn: APM-RMB-6xx</OPTION>
                        <OPTION 
                value="MagiKitchn: APM-RMB-6xx-H">MagiKitchn: APM-RMB-6xx-H</OPTION>
                        <OPTION 
                value="MagiKitchn: APM-SMB-6xx">MagiKitchn: APM-SMB-6xx</OPTION>
                        <OPTION 
                value="MagiKitchn: APM-SMB-6xx-H">MagiKitchn: APM-SMB-6xx-H</OPTION>
                        <OPTION value="MagiKitchn: CM-RMB-6xx">MagiKitchn: CM-RMB-6xx</OPTION>
                        <OPTION value="MagiKitchn: CM-RMB-6xx-H">MagiKitchn: 
                        CM-RMB-6xx-H</OPTION>
                        <OPTION value="MagiKitchn: CM-SMB-6xx">MagiKitchn: CM-SMB-6xx</OPTION>
                        <OPTION value="MagiKitchn: CM-SMB-6xx-H">MagiKitchn: 
                        CM-SMB-6xx-H</OPTION>
                        <OPTION value="Montague: UFLCS-xxR">Montague: UFLCS-xxR</OPTION>
                        <OPTION value="Rankin-Delux: RB-8xx-C">Rankin-Delux: RB-8xx-C</OPTION>
                        <OPTION value="Rankin-Delux: TB-3xx-C">Rankin-Delux: TB-3xx-C</OPTION>
                        <OPTION value="Rankin-Delux: TB-8xx-C">Rankin-Delux: TB-8xx-C</OPTION>
                        <OPTION value="Royal: RCB-xx">Royal: RCB-xx</OPTION>
                        <OPTION value="Royal: RIB-xx">Royal: RIB-xx</OPTION>
                        <OPTION value="Royal: RSCB-xx">Royal: RSCB-xx</OPTION>
                        <OPTION value="Saturn: 100-CRB-xx">Saturn: 100-CRB-xx</OPTION>
                        <OPTION value="Saturn: 500-CRB-xx">Saturn: 500-CRB-xx</OPTION>
                        <OPTION value="Southbend: HDC-xx">Southbend: HDC-xx</OPTION>
                        <OPTION value="Southbend: HDLC-xx">Southbend: HDLC-xx</OPTION>
                        <OPTION value="Star: 60xxCBD">Star: 60xxCBD</OPTION>
                        <OPTION value="Star: 60xxRCBD">Star: 60xxRCBD</OPTION>
                        <OPTION value="Star: 80xxCBD">Star: 80xxCBD</OPTION>
                        <OPTION value="Star: 81xxCBD">Star: 81xxCBD</OPTION>
                        <OPTION value="Toastmaster: TMLCxx">Toastmaster: TMLCxx</OPTION>
                        <OPTION value="Toastmaster: TMRCxx">Toastmaster: TMRCxx</OPTION>
                        <OPTION value="Tri-Star: TSEB-xxC">Tri-Star: TSEB-xxC</OPTION>
                        <OPTION value="Tri-Star: TSRB-xx">Tri-Star: TSRB-xx</OPTION>
                        <OPTION value="Tri-Star: TSRB-xxC">Tri-Star: TSRB-xxC</OPTION>
                        <OPTION value="Vollrath: 40xxx">Vollrath: 40xxx</OPTION>
                        <OPTION value="Vollrath: 9xxCG">Vollrath: 9xxCG</OPTION>
                        <OPTION value="Vulcan: VCCBxx">Vulcan: VCCBxx</OPTION>
                        <OPTION value="Vulcan : VACBxx">Vulcan : VACBxx</OPTION>
                        <OPTION value="Wolf: ACBxx">Wolf: ACBxx</OPTION>
                        <OPTION 
                value="Wolf: SCBxxC">Wolf: SCBxxC</OPTION>
                      </SELECT>
                      </font></span></font>
                    </FIELDSET></td>
                    <td class=ee131 colid="2" rowid="4" sheetid="1"><FIELDSET id=XLEW_1_4_2ssc 
              style="BORDER-RIGHT: 0px; PADDING-RIGHT: 0px; BORDER-TOP: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; BORDER-LEFT: 0px; PADDING-TOP: 0px; BORDER-BOTTOM: 0px">
                      <font face="Arial"><span style=""><font>
                      <SELECT 
              class="calc_model" id=XLEW_1_4_2 tabIndex=3 
              onchange="recalc_onclick('XLEW_1_4_2')" size=1 name=XLEW_1_4_2>
                        <OPTION 
                value=12 selected>12</OPTION>
                        <OPTION value=24>24</OPTION>
                        <OPTION 
                value=36>36</OPTION>
                        <OPTION value=48 selected style="font-weight:bold">48</OPTION>
                        <OPTION 
                value=60>60</OPTION>
                        <OPTION value=72>72</OPTION>
                      </SELECT>
                      </font></span><font> </font> </font>
                    </FIELDSET>
                    </font></td>
                  <td class=ee125 colid="3" rowid="3" sheetid="1" width="213">
                      <div id="calc_current_product">
                            <INPUT class=ee127  id=XLEW_1_3_3  style="OVERFLOW: hidden; WIDTH: 100%; background-color:transparent; border:none; value="Vulcan VTEC"  tabIndex=-1 readOnly name=XLEW_1_3_3>
                      </div>
                  </td>
                </tr>
             </table>
             </td>
         </tr>
     </table>
     
     <table cellpadding="0" cellspacing="0" style="padding-top:15px;">
        <tr style="HEIGHT: 17pt">
          <td class=ee143 colid="1" rowid="7" sheetid="1" width="217"><font face="Arial" style="font-size: 12px">UNIT Energy Rating (BTU/hr) </font></td>
          <td class=ee145 colid="2" rowid="7" sheetid="1" width="244"><font face="Arial"><span style=""><font>
            <INPUT class=ee146 
      id=XLEW_1_7_2 
      style="BORDER-RIGHT: #000000 0px solid; PADDING-RIGHT: 1px; BORDER-TOP: #000000 0px solid; OVERFLOW: hidden; BORDER-LEFT: #000000 0px solid; WIDTH: 100%; BORDER-BOTTOM: #000000 0px solid" 
      tabIndex=-1 readOnly value=0 name=XLEW_1_7_2>
            </font></span></font></td>
          <td class=ee147 colid="3" rowid="7" sheetid="1" width="213"><font face="Arial"> <span style=""><font>
            <INPUT class=ee146 
      id=XLEW_1_7_3 
      style="BORDER-RIGHT: #000000 0px solid; PADDING-RIGHT: 1px; BORDER-TOP: #000000 0px solid; OVERFLOW: hidden; BORDER-LEFT: #000000 0px solid; WIDTH: 100%; BORDER-BOTTOM: #000000 0px solid" 
      tabIndex=-1 readOnly value=0 name=XLEW_1_7_3>
            </font></span></font></td>
        </tr>
        <tr style="HEIGHT: 17pt">
          <td class=ee143 colid="1" rowid="8" sheetid="1" width="217"><font face="Arial" style="font-size: 12px">Operating Cost/day ($)</font><font face="Arial"> </font> <FONT class=ee148 face="Arial"><SUP>3</SUP></FONT></td>
          <td class=ee149 colid="2" rowid="8" sheetid="1" width="244"><font face="Arial"><span style=""><font>
            <INPUT class=ee146 
      id=XLEW_1_8_2 
      style="BORDER-RIGHT: #000000 0px solid; PADDING-RIGHT: 1px; BORDER-TOP: #000000 0px solid; OVERFLOW: hidden; BORDER-LEFT: #000000 0px solid; WIDTH: 100%; BORDER-BOTTOM: #000000 0px solid" 
      tabIndex=-1 readOnly value=- name=XLEW_1_8_2>
            </font></span></font></td>
          <td class=ee147 colid="3" rowid="8" sheetid="1" width="213"><font face="Arial"> <span style=""><font>
            <INPUT class=ee146 
      id=XLEW_1_8_3 
      style="BORDER-RIGHT: #000000 0px solid; PADDING-RIGHT: 1px; BORDER-TOP: #000000 0px solid; OVERFLOW: hidden; BORDER-LEFT: #000000 0px solid; WIDTH: 100%; BORDER-BOTTOM: #000000 0px solid" 
      tabIndex=-1 readOnly value=- name=XLEW_1_8_3>
            </font></span></font></td>
        </tr>
        <tr style="HEIGHT: 17pt">
          <td class=ee143 colid="1" rowid="9" sheetid="1" width="217"><font face="Arial" style="font-size: 12px">Operating Cost/year ($)</font><font face="Arial"> </font> <FONT class=ee148 face="Arial"><SUP>4</SUP></FONT></td>
          <td class=ee149 colid="2" rowid="9" sheetid="1" width="244"><font face="Arial"><span><font>
            <INPUT class=ee146 
      id=XLEW_1_9_2 
      style="BORDER-RIGHT: #000000 0px solid; PADDING-RIGHT: 1px; BORDER-TOP: #000000 0px solid; OVERFLOW: hidden; BORDER-LEFT: #000000 0px solid; WIDTH: 100%; BORDER-BOTTOM: #000000 0px solid" 
      tabIndex=-1 readOnly value=- name=XLEW_1_9_2>
            </font></span></font></td>
          <td class=ee147 colid="3" rowid="9" sheetid="1" width="213"><font face="Arial"> <span style=""><font>
            <INPUT class=ee146 
      id=XLEW_1_9_3 
      style="BORDER-RIGHT: #000000 0px solid; PADDING-RIGHT: 1px; BORDER-TOP: #000000 0px solid; OVERFLOW: hidden; BORDER-LEFT: #000000 0px solid; WIDTH: 100%; BORDER-BOTTOM: #000000 0px solid" 
      tabIndex=-1 readOnly value=- name=XLEW_1_9_3>
            </font></span></font></td>
        </tr>
        <tr style="HEIGHT: 22pt">
          <td class=ee151 colid="1" rowid="10" sheetid="1" width="217"><font face="Arial">Cost Savings/year ($) </font></td>
          <td class=ee154 colid="2" rowid="10" sheetid="1" width="243"><font face="Arial">- </font></td>
          <td class=ee156 colid="3" rowid="10" sheetid="1" width="213"><font face="Arial"> <span style=""><font>
            <INPUT class=ee155 
      id=XLEW_1_10_3 
      style="BORDER-RIGHT: #000000 0px solid; PADDING-RIGHT: 1px; BORDER-TOP: #000000 0px solid; OVERFLOW: hidden; BORDER-LEFT: #000000 0px solid; WIDTH: 100%; BORDER-BOTTOM: #000000 0px solid" 
      tabIndex=-1 readOnly value=- name=XLEW_1_10_3>
            </font></span></font></td>
        </tr>
      </TBODY>
    </TABLE>
  </DIV>
  <p>
    </DIV>
    </DIV>
  <DIV class="wizardbuttons mystyles">
  <font face="Arial"><span style="font-size: 11pt">
  <INPUT 
id=xl_templateimages type=hidden value=1 name=xl_templateimages>
  </span></font>
  </p>
</FORM>
<font face="Arial"> 
<!-- SpreadsheetConverter Body end -->
<NOSCRIPT>
</NOSCRIPT>
<SCRIPT language=JavaScript 
src="internalfunctions.js" 
type=text/javascript></SCRIPT> 
<SCRIPT language=javascript type=text/javascript>function setCookieValues(){var idvalue,idinput,elemtype,i,elemlength=document.formc.elements.length,localData,pageName,expDate;pageName=GetPageName();expDate=CookieExpiryDate(new Date,3);for(i=0;i<elemlength;i++){elemtype=document.formc.elements[i].type;if(elemtype=="text"||elemtype=="textarea"||elemtype=="hidden"){if(document.formc.elements[i].readOnly==false){idinput=document.formc.elements[i].id;if(idinput!="xl_aux2"){idvalue=document.formc.elements[i].value;YAHOO.util.Cookie.set(pageName+"_"+idinput,idvalue,{expires:expDate});localData+=pageName+"_"+idinput+","+idvalue+","+expDate+","+window.location+",No"}}}else if(elemtype=="select-one"){idvalue=document.formc.elements[i].selectedIndex;idinput=document.formc.elements[i].id;YAHOO.util.Cookie.set(pageName+"_"+idinput,idvalue,{expires:expDate});localData+=pageName+"_"+idinput+","+idvalue+","+expDate+","+window.location+",No"}else if(elemtype=="checkbox"||elemtype=="radio"){idvalue=document.formc.elements[i].checked?"yes":"no";idinput=document.formc.elements[i].id;if(idinput!="showagain"){YAHOO.util.Cookie.set(pageName+"_"+idinput,idvalue,{expires:expDate});localData+=pageName+"_"+idinput+","+idvalue+","+expDate+","+window.location+",No"}}if(localData.length>4096)break}}function getCookieValues(){for(var idinput,idvalue,elemtype,firstload="false",elemlength=document.formc.elements.length,i=0;i<elemlength;i++){elemtype=document.formc.elements[i].type;if(elemtype=="text"||elemtype=="textarea"||elemtype=="hidden"){if(document.formc.elements[i].readOnly==false){idinput=document.formc.elements[i].id;idvalue=YAHOO.util.Cookie.get(GetPageName()+"_"+idinput);if(idvalue==null){firstload="true";break}if(idvalue==GetPageName()+"_"+idinput)idvalue="";document.formc.elements[i].value=idvalue}}else if(elemtype=="select-one"){idinput=document.formc.elements[i].id;idvalue=YAHOO.util.Cookie.get(GetPageName()+"_"+idinput);document.formc.elements[i].selectedIndex=idvalue}else if(elemtype=="checkbox"||elemtype=="radio"){idinput=document.formc.elements[i].id;idvalue=YAHOO.util.Cookie.get(GetPageName()+"_"+idinput);if(idvalue=="yes")document.formc.elements[i].checked=true;else document.formc.elements[i].checked=false}}if(firstload=="true"){initial_update();eequerystring()}else{var querystring=document.location.search;if(querystring.length>0){variables=querystring.substring(1).split("&");variables.toString().toUpperCase().search("FORCE=TRUE")!=-1&&eequerystring()}}}</SCRIPT> 
<SCRIPT language=javascript type=text/javascript>for(var row1xE2E7=new Array(6),jj=0;jj<6;jj++)row1xE2E7[jj]=0;for(var row1xF2F7=new Array(6),jj=0;jj<6;jj++)row1xF2F7[jj]=0;for(var row1xG2G6=new Array(5),jj=0;jj<5;jj++)row1xG2G6[jj]="";for(var row1xH2H6=new Array(5),jj=0;jj<5;jj++)row1xH2H6[jj]=0;for(var row1xJ2J81=new Array(80),jj=0;jj<80;jj++)row1xJ2J81[jj]="";for(var row1xK2K81=new Array(80),jj=0;jj<80;jj++)row1xK2K81[jj]=0;function calc(data){var c1B3=data.XLEW_1_3_2,c1B4=data.XLEW_1_4_2,c1B5=data.XLEW_1_5_2,c1B6=data.XLEW_1_6_2;row1xE2E7[0]=s2n(n2s(12));row1xF2F7[0]=1;row1xG2G6[0]="Vulcan VTEC14";row1xH2H6[0]=22000;row1xJ2J81[0]="American Range: ADJ-xx";row1xK2K81[0]=4e4;row1xE2E7[1]=s2n(n2s(24));row1xF2F7[1]=2;row1xG2G6[1]="Vulcan VTEC25";row1xJ2J81[1]="American Range: AECB-xx";row1xK2K81[1]=35000;row1xE2E7[2]=s2n(n2s(36));row1xF2F7[2]=3;row1xG2G6[2]="Vulcan VTEC36";row1xJ2J81[2]="American Range: ARKB-xx";row1xK2K81[2]=6e4;row1xE2E7[3]=s2n(n2s(48));row1xF2F7[3]=4;row1xG2G6[3]="Vulcan VTEC48";row1xJ2J81[3]="APW Wyott: GCB-xxH";row1xK2K81[3]=4e4;row1xE2E7[4]=s2n(n2s(60));row1xF2F7[4]=5;row1xG2G6[4]="Vulcan VTEC60";row1xJ2J81[4]="APW Wyott: GCRB-xxH";row1xK2K81[4]=4e4;row1xE2E7[5]=72;row1xF2F7[5]=6;row1xJ2J81[5]="APW Wyott: HCB-xx24";row1xK2K81[5]=4e4;row1xJ2J81[6]="APW Wyott: HCRB-xx24";row1xK2K81[6]=33000;row1xJ2J81[7]="Bakers Pride: C-xxGS";row1xK2K81[7]=37500;row1xJ2J81[8]="Bakers Pride: C-xxR";row1xK2K81[8]=3e4;row1xJ2J81[9]="Bakers Pride: HDCB-24xx";row1xK2K81[9]=33000;row1xJ2J81[10]="Bakers Pride: HDCRB-24xx";row1xK2K81[10]=33000;row1xJ2J81[11]="Bakers Pride: L-xxGS";row1xK2K81[11]=37500;row1xJ2J81[12]="Bakers Pride: L-xxR";row1xK2K81[12]=37500;row1xJ2J81[13]="Bakers Pride: XX-xx";row1xK2K81[13]=36000;row1xJ2J81[14]="Comstock Castle: 32xxLB";row1xK2K81[14]=3e4;row1xJ2J81[15]="Comstock Castle: 32xxRB";row1xK2K81[15]=3e4;row1xJ2J81[16]="Comstock Castle: ELBxx";row1xK2K81[16]=3e4;row1xJ2J81[17]="Comstock Castle: ERBxx";row1xK2K81[17]=3e4;row1xJ2J81[18]="Comstock Castle: FHPxx-2LB";row1xK2K81[18]=25000;row1xJ2J81[19]="Comstock Castle: FHPxx-2RBB";row1xK2K81[19]=25000;row1xJ2J81[20]="Connerton: CRB-xx-C";row1xK2K81[20]=24000;row1xJ2J81[21]="Connerton: LRB-xx-C";row1xK2K81[21]=4e4;row1xJ2J81[22]="Eagle: CLCHRB-xx-NG";row1xK2K81[22]=4e4;row1xJ2J81[23]="Eagle: CLCHRB-xx-NG-X";row1xK2K81[23]=4e4;row1xJ2J81[24]="Garland: Gxx-BRL";row1xK2K81[24]=3e4;row1xJ2J81[25]="Garland: GD-xxRB";row1xK2K81[25]=3e4;row1xJ2J81[26]="Garland: GD-xxRBFF";row1xK2K81[26]=3e4;row1xJ2J81[27]="Garland: GFxx-BRL";row1xK2K81[27]=3e4;row1xJ2J81[28]="Garland: GTBGxx-ABxx";row1xK2K81[28]=3e4;row1xJ2J81[29]="Garland: GTBGxx-ARxx";row1xK2K81[29]=36000;row1xJ2J81[30]="Garland: GTBGxx-NRxx";row1xK2K81[30]=36000;row1xJ2J81[31]="Garland: HEEGxxCL";row1xK2K81[31]=27000;row1xJ2J81[32]="Garland: UTBGxx-ABxx";row1xK2K81[32]=3e4;row1xJ2J81[33]="Globe: GCBxxG";row1xK2K81[33]=4e4;row1xJ2J81[34]="Globe: GCBRKxxK";row1xK2K81[34]=4e4;row1xJ2J81[35]="Imperial: IABR-xx";row1xK2K81[35]=4e4;row1xJ2J81[36]="Imperial: IABS-xx";row1xK2K81[36]=4e4;row1xJ2J81[37]="Imperial: IRB-xx";row1xK2K81[37]=3e4;row1xJ2J81[38]="Jade: JB-xx";row1xK2K81[38]=3e4;row1xJ2J81[39]="Jade: KC-xx";row1xK2K81[39]=32500;row1xJ2J81[40]="Lang: 21xxZRCB";row1xK2K81[40]=32000;row1xJ2J81[41]="LoLo: LCBxxRMPF";row1xK2K81[41]=4e4;row1xJ2J81[42]="MagiKitchn: APL-RMB-6xx";row1xK2K81[42]=3e4;row1xJ2J81[43]="MagiKitchn: APL-RMB-6xx-H";row1xK2K81[43]=4e4;row1xJ2J81[44]="MagiKitchn: APL-SMB-6xx";row1xK2K81[44]=3e4;row1xJ2J81[45]="MagiKitchn: APL-SMB-6xx-H";row1xK2K81[45]=4e4;row1xJ2J81[46]="MagiKitchn: APM-RMB-6xx";row1xK2K81[46]=3e4;row1xJ2J81[47]="MagiKitchn: APM-RMB-6xx-H";row1xK2K81[47]=4e4;row1xJ2J81[48]="MagiKitchn: APM-SMB-6xx";row1xK2K81[48]=3e4;row1xJ2J81[49]="MagiKitchn: APM-SMB-6xx-H";row1xK2K81[49]=4e4;row1xJ2J81[50]="MagiKitchn: CM-RMB-6xx";row1xK2K81[50]=3e4;row1xJ2J81[51]="MagiKitchn: CM-RMB-6xx-H";row1xK2K81[51]=4e4;row1xJ2J81[52]="MagiKitchn: CM-SMB-6xx";row1xK2K81[52]=3e4;row1xJ2J81[53]="MagiKitchn: CM-SMB-6xx-H";row1xK2K81[53]=4e4;row1xJ2J81[54]="Montague: UFLCS-xxR";row1xK2K81[54]=38000;row1xJ2J81[55]="Rankin-Delux: RB-8xx-C";row1xK2K81[55]=29000;row1xJ2J81[56]="Rankin-Delux: TB-3xx-C";row1xK2K81[56]=22000;row1xJ2J81[57]="Rankin-Delux: TB-8xx-C";row1xK2K81[57]=29000;row1xJ2J81[58]="Royal: RCB-xx";row1xK2K81[58]=4e4;row1xJ2J81[59]="Royal: RIB-xx";row1xK2K81[59]=35000;row1xJ2J81[60]="Royal: RSCB-xx";row1xK2K81[60]=35000;row1xJ2J81[61]="Saturn: 100-CRB-xx";row1xK2K81[61]=34000;row1xJ2J81[62]="Saturn: 500-CRB-xx";row1xK2K81[62]=3e4;row1xJ2J81[63]="Southbend: HDC-xx";row1xK2K81[63]=4e4;row1xJ2J81[64]="Southbend: HDLC-xx";row1xK2K81[64]=4e4;row1xJ2J81[65]="Star: 60xxCBD";row1xK2K81[65]=4e4;row1xJ2J81[66]="Star: 60xxRCBD";row1xK2K81[66]=4e4;row1xJ2J81[67]="Star: 80xxCBD";row1xK2K81[67]=4e4;row1xJ2J81[68]="Star: 81xxCBD";row1xK2K81[68]=4e4;row1xJ2J81[69]="Toastmaster: TMLCxx";row1xK2K81[69]=3e4;row1xJ2J81[70]="Toastmaster: TMRCxx";row1xK2K81[70]=3e4;row1xJ2J81[71]="Tri-Star: TSEB-xxC";row1xK2K81[71]=38000;row1xJ2J81[72]="Tri-Star: TSRB-xx";row1xK2K81[72]=3e4;row1xJ2J81[73]="Tri-Star: TSRB-xxC";row1xK2K81[73]=34000;row1xJ2J81[74]="Vollrath: 40xxx";row1xK2K81[74]=28000;row1xJ2J81[75]="Vollrath: 9xxCG";row1xK2K81[75]=4e4;row1xJ2J81[76]="Vulcan: VCCBxx";row1xK2K81[76]=29000;row1xJ2J81[77]="Vulcan : VACBxx";row1xK2K81[77]=36000;row1xJ2J81[78]="Wolf: ACBxx";row1xK2K81[78]=36000;row1xJ2J81[79]="Wolf: SCBxxC";row1xK2K81[79]=29000;var c1C3=lookup3vv_var(c1B4,row1xE2E7,0,4,row1xG2G6,0,4);row1xH2H6[1]=row1xH2H6[0]*2;row1xH2H6[2]=row1xH2H6[0]*3;row1xH2H6[3]=row1xH2H6[0]*4;row1xH2H6[4]=row1xH2H6[0]*5;var c1B7=lookup3vv_var(c1B4,row1xE2E7,0,5,row1xF2F7,0,5)*lookup3vv_str(c1B3,row1xJ2J81,0,79,row1xK2K81,0,79),c1C7=lookup3vv_var(c1C3,row1xG2G6,0,4,row1xH2H6,0,4),c1B8=c1B6*c1B5*c1B7/(100*1030),c1C8=c1B6*c1B5*c1C7/(100*1030),c1B9=c1B8*360,c1C9=c1C8*360,c1C10=c1B9-c1C9;data.XLEW_1_3_3=c1C3;data.XLEW_1_7_2=c1B7;data.XLEW_1_7_3=c1C7;data.XLEW_1_8_2=c1B8;data.XLEW_1_8_3=c1C8;data.XLEW_1_9_2=c1B9;data.XLEW_1_9_3=c1C9;data.XLEW_1_10_3=c1C10}</SCRIPT> 
<SCRIPT language=javascript type=text/javascript>var eeisus=1,eetrue="TRUE",eefalse="FALSE",eedec=".",eeth=",",eedecreg=new RegExp("\\.","g"),eethreg=new RegExp(",","g"),eecurrencyreg=new RegExp("[$]","g"),eepercentreg=new RegExp("%","g"),fmtdaynamesshort=["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],fmtdaynameslong=["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],fmtmonthnamesshort=["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"],fmtmonthnameslong=["January","February","March","April","May","June","July","August","September","October","November","December"],fmtstrings=[","," ","$"],fmtdate6=[34,25,0],fmtdate7=[34,25,2],jsonLocal='{"eeisus":1,"eetrue":"TRUE","eefalse":"FALSE","eedec":".","eeth":",","eedecreg":["\\\\.","g"],"eethreg":[",","g"],"eecurrencyreg":["[$]","g"],"eepercentreg":["%","g"],"fmtdaynamesshort":["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],"fmtdaynameslong":["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],"fmtmonthnamesshort":["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"],"fmtmonthnameslong":["January","February","March","April","May","June","July","August","September","October","November","December"],"fmtstrings":[","," ","$"]}'</SCRIPT> 
<SCRIPT language=javascript type=text/javascript>var co={};function recalc_onclick(){if(true){co.XLEW_1_3_2=eegetdropdownvalue_str(document.formc.XLEW_1_3_2);co.XLEW_1_4_2=eeparseFloatV(eegetdropdownvalue(document.formc.XLEW_1_4_2));co.XLEW_1_5_2=eeparseFloat(document.formc.XLEW_1_5_2.value);co.XLEW_1_6_2=eeparseFloatTh(document.formc.XLEW_1_6_2.value);calc(co);document.formc.XLEW_1_3_3.value=eeisnumber(co.XLEW_1_3_3)?eedisplayFloat(co.XLEW_1_3_3):co.XLEW_1_3_3;document.formc.XLEW_1_7_2.value=eedisplayFloatNDTh(co.XLEW_1_7_2,0);document.formc.XLEW_1_7_3.value=eedisplayFloatNDTh(co.XLEW_1_7_3,0);document.formc.XLEW_1_8_2.value=eedatefmt(fmtdate7,co.XLEW_1_8_2);document.formc.XLEW_1_8_3.value=eedatefmt(fmtdate7,co.XLEW_1_8_3);document.formc.XLEW_1_9_2.value=eedatefmt(fmtdate6,co.XLEW_1_9_2);document.formc.XLEW_1_9_3.value=eedatefmt(fmtdate6,co.XLEW_1_9_3);document.formc.XLEW_1_10_3.value=eedatefmt(fmtdate6,co.XLEW_1_10_3)}}</SCRIPT> 
<!-- library --> 
<SCRIPT language=javascript type=text/javascript>function str_eq(x,y){if(isNaN(x)&&isNaN(y))return x.toLowerCase()==y.toLowerCase();else return x==y}function str_ls(x,y){if(isNaN(x)&&isNaN(y))return x.toLowerCase()<y.toLowerCase();else return x<y}function var_eq(x,y){var xt=mytypeof(x),yt=mytypeof(y);if(xt!=yt)return false;switch(xt){case 1:case 3:return x==y;case 2:return str_eq(x,y);default:return false}}function var_ls(x,y){var xt=mytypeof(x),yt=mytypeof(y);if(xt!=yt)return xt<yt;switch(xt){case 1:case 3:return x<y;case 2:return str_ls(x,y);default:return false}}function eegetdropdownvalue(ctl){return ctl.selectedIndex>=0&&ctl[ctl.selectedIndex]?ctl[ctl.selectedIndex].value:0}function eegetdropdownvalue_str(ctl){return ctl.selectedIndex>=0&&ctl[ctl.selectedIndex]?ctl[ctl.selectedIndex].value:""}function mytypeof(v){switch(typeof v){case "number":if(myIsNaN(v))return 4;return 1;case "string":return 2;case "boolean":return 3;case "object":if(v.constructor==Number){if(myIsNaN(v))return 4;return 1}if(v.constructor==String)return 2;if(v.constructor==Boolean)return 3;return 4;default:return 4}}function myIsNaN(x){return isNaN(x)||typeof x=="number"&&!isFinite(x)}function mod(n,d){return n-d*Math.floor(n/d)}function round(n,nd){if(isFinite(n)&&isFinite(nd)){var sign_n=n<0?-1:1,abs_n=Math.abs(n),factor=Math.pow(10,nd);return sign_n*Math.round(abs_n*factor)/factor}else return NaN}function s2n(str){str=String(str).replace(eedecreg,".");str=str.replace(eethreg,"");str=str.replace(eecurrencyreg,"");var res=parseFloat(str);if(myIsNaN(res))res=0;if(str.search(eepercentreg)>=0)res=res/100;return res}function n2s(x){return x.toString()}function eeparseFloat(str){str=String(str).replace(eedecreg,".");var res=parseFloat(str);if(isNaN(res))return 0;else return res}var near0RegExp=new RegExp("[.](.*0000000|.*9999999)");function eedisplayFloat(x){if(myIsNaN(x))return Number.NaN;else{var str=String(x);if(near0RegExp.test(str)){x=round(x,8);str=String(x)}return str.replace(/\./g,eedec)}}function eedisplayScientific(x,nd){if(myIsNaN(x))return Number.NaN;else{var str=String(x.toExponential(nd));return str.replace(/\./g,eedec)}}function eedisplayFloatND(x,nd){if(myIsNaN(x))return Number.NaN;else{var res=round(x,nd);if(nd>0){var str=String(res);if(str.indexOf("e")!=-1)return str;if(str.indexOf("E")!=-1)return str;var parts=str.split(".");if(parts.length<2){var decimals="00000000000000".substring(0,nd);return parts[0].toString()+eedec+decimals}else{var decimals=(parts[1].toString()+"00000000000000").substring(0,nd);return parts[0].toString()+eedec+decimals}}else return res}}function eedisplayPercent(x){if(myIsNaN(x))return Number.NaN;else return eedisplayFloat(x*100)+"%"}function eedisplayPercentND(x,nd){if(myIsNaN(x))return Number.NaN;else return eedisplayFloatND(x*100,nd)+"%"}function eeparseFloatTh(str){str=String(str).replace(eethreg,"");str=String(str).replace(eedecreg,".");var res=parseFloat(str);if(isNaN(res))return 0;else return res}function eedisplayFloatNDTh(x,nd){if(myIsNaN(x))return Number.NaN;else{var res=round(x,nd);if(nd>0){var str=String(res);if(str.indexOf("e")!=-1)return str;if(str.indexOf("E")!=-1)return str;var parts=str.split("."),res2=eeinsertThousand(parts[0].toString());if(parts.length<2){var decimals="00000000000000".substring(0,nd);return res2+eedec+decimals}else{var decimals=(parts[1].toString()+"00000000000000").substring(0,nd);return res2+eedec+decimals}}else return eeinsertThousand(res.toString())}}function eedisplayPercentNDTh(x,nd){if(myIsNaN(x))return Number.NaN;else return eedisplayFloatNDTh(x*100,nd)+"%"}var eeparseFloatVreg=new RegExp("^ *-?[0-9.]+ *$");function eeparseFloatV(str){if(str=="")return str;str=String(str).replace(eedecreg,".");if(!eeparseFloatVreg.test(str))return str;var res=parseFloat(str);if(isNaN(res))return str;else return res}function eeinsertThousand(whole){if(whole==""||whole.indexOf("e")>=0)return whole;else{var minus_sign="";if(whole.charAt(0)=="-"){minus_sign="-";whole=whole.substring(1)}for(var res="",str_length=whole.length-1,ii=0;ii<=str_length;ii++){if(ii>0&&ii%3==0)res=eeth+res;res=whole.charAt(str_length-ii)+res}return minus_sign+res}}function eedatefmt(fmt,x){if(!isFinite(x))return Number.NaN;for(var padding=0,tmp=0,res="",len=fmt.length,ii=0;ii<len;ii++)if(fmt[ii]>31)res+=fmtstrings[fmt[ii]-32];else switch(fmt[ii]){case 2:res+=eemonth(x);break;case 3:tmp=eemonth(x);if(tmp<10)res+="0";res+=tmp;break;case 4:res+=fmtmonthnamesshort[eemonth(x)-1];break;case 5:res+=fmtmonthnameslong[eemonth(x)-1];break;case 6:res+=eeday(x);break;case 7:tmp=eeday(x);if(tmp<10)res+="0";res+=tmp;break;case 8:res+=fmtdaynamesshort[weekday(x,1)-1];break;case 9:res+=fmtdaynameslong[weekday(x,1)-1];break;case 10:tmp=year(x)%100;if(tmp<10)res+="0";res+=tmp;break;case 11:res+=year(x);break;case 12:res+=hour(x);break;case 13:tmp=hour(x);if(tmp<10)res+="0";res+=tmp;break;case 14:tmp=hour(x)%12;if(tmp==0)res+="12";else res+=tmp%12;break;case 15:tmp=hour(x)%12;if(tmp==0)res+="12";else{if(tmp<10)res+="0";res+=tmp}break;case 16:res+=minute(x);break;case 17:tmp=minute(x);if(tmp<10)res+="0";res+=tmp;break;case 18:res+=second(x);break;case 19:tmp=second(x);if(tmp<10)res+="0";res+=tmp;break;case 21:case 22:if(hour(x)<12)res+="AM";else res+="PM";break;case 23:res+=eedisplayFloat(x);break;case 24:tmp=fmt[++ii];res+=eedisplayFloatND(x,tmp);break;case 25:tmp=fmt[++ii];res+=eedisplayFloatNDTh(x,tmp);break;case 26:res+=eedisplayPercent(x);break;case 27:tmp=fmt[++ii];res+=eedisplayPercentND(x,tmp);break;case 28:tmp=fmt[++ii];res+=eedisplayPercentNDTh(x,tmp);break;case 29:tmp=fmt[++ii];res+=eedisplayScientific(x,tmp);break;case 30:padding=fmt[++ii];tmp=hour(x)+Math.floor(x)*24;tmp=tmp.toString();if(tmp.length<padding)res+="00000000000000".substring(0,padding-tmp.length);res+=tmp}return res}function eeisnumber(v){if(isNaN(v)||v==Number.NEGATIVE_INFINITY||v==Number.POSITIVE_INFINITY)return false;else switch(typeof v){case "number":return true;case "object":return v.constructor==Number;default:return false}}function isna(x){if(typeof x=="number")return isNaN(x);else return false}function lookup3vv_str(key,kvect,kfrom_start,kto_start,vvect,vfrom_){if(isna(key))return Number.NaN;var current=0,from_=kfrom_start,to_=kto_start+1;while(true){current=from_+to_>>1;if(str_eq(kvect[current],key))break;if(from_==to_-1)break;if(str_ls(kvect[current],key))from_=current;else to_=current}while(current<kto_start)if(str_eq(kvect[current],kvect[current+1]))current++;else break;if(str_ls(key,kvect[current]))return Number.NaN;return vvect[vfrom_+current-kfrom_start]}function lookup3vv_var(key,kvect,kfrom_start,kto_start,vvect,vfrom_){if(isna(key))return Number.NaN;var current=0,from_=kfrom_start,to_=kto_start+1;while(true){current=from_+to_>>1;if(var_eq(kvect[current],key))break;if(from_==to_-1)break;if(var_ls(kvect[current],key))from_=current;else to_=current}while(current<kto_start)if(var_eq(kvect[current],kvect[current+1]))current++;else break;if(var_ls(key,kvect[current]))return Number.NaN;return vvect[vfrom_+current-kfrom_start]}function leap_gregorian(year){return year%4==0&&!(year%100==0&&year%400!=0)}var GREGORIAN_EPOCH=1721425;function gregorian_to_jd(year,month,day){return GREGORIAN_EPOCH-0+365*(year-1)+Math.floor((year-1)/4)+-Math.floor((year-1)/100)+Math.floor((year-1)/400)+Math.floor((367*month-362)/12+(month<=2?0:leap_gregorian(year)?-1:-2)+day)}function jd_to_gregorian(jd){var wjd,depoch,quadricent,dqc,cent,dcent,quad,dquad,yindex,year,yearday,leapadj;wjd=Math.floor(jd);depoch=wjd-GREGORIAN_EPOCH-1;quadricent=Math.floor(depoch/146097);dqc=mod(depoch,146097);cent=Math.floor(dqc/36524);dcent=mod(dqc,36524);quad=Math.floor(dcent/1461);dquad=mod(dcent,1461);yindex=Math.floor(dquad/365);year=quadricent*400+cent*100+quad*4+yindex;if(!(cent==4||yindex==4))year++;yearday=wjd-gregorian_to_jd(year,1,1);leapadj=wjd<gregorian_to_jd(year,3,1)?0:leap_gregorian(year)?1:2;var month=Math.floor(((yearday+leapadj)*12+373)/367),day=wjd-gregorian_to_jd(year,month,1)+1;return [year,month,day]}function eeday(serial_number){if(!isFinite(serial_number))return Number.NaN;if(serial_number<1)return 0;if(serial_number>60)serial_number--;var res=jd_to_gregorian(serial_number+2415020);return res[2]}function hour(serial_number){if(!isFinite(serial_number))return Number.NaN;var res=Math.floor((serial_number-Math.floor(serial_number))*86400+.5);return Math.floor(res/3600)}function minute(serial_number){if(!isFinite(serial_number))return Number.NaN;var res=Math.floor((serial_number-Math.floor(serial_number))*86400+.5);return Math.floor(res/60)%60}function eemonth(serial_number){if(!isFinite(serial_number))return Number.NaN;if(serial_number<1)return 1;if(serial_number>60)serial_number--;var res=jd_to_gregorian(serial_number+2415020);return res[1]}function second(serial_number){if(!isFinite(serial_number))return Number.NaN;var res=Math.floor((serial_number-Math.floor(serial_number))*86400+.5);return res%60}function weekday(serial_number,return_type){if(!isFinite(return_type)||!isFinite(serial_number))return Number.NaN;if(return_type<1||return_type>3)return Number.NaN;var res=Math.floor(serial_number+6)%7;switch(Math.floor(return_type)){case 1:return res+1;case 2:return (res+6)%7+1;case 3:return (res+6)%7}return "hej"}function year(serial_number){if(!isFinite(serial_number))return Number.NaN;if(serial_number<1)return 1900;if(serial_number>60)serial_number--;var res=jd_to_gregorian(serial_number+2415020);return res[0]}</SCRIPT> 
<SCRIPT language=javascript type=text/javascript>function postcode(){}</SCRIPT> 
<SCRIPT language=javascript type=text/javascript>function getPanelIndex(){var indexvalue,ii,accordionlength;indexvalue=0;ii=0;accordionlength=jQuery(".msg_head").length;for(ii;ii<accordionlength;ii++){var curvalue,lastvalue;curvalue=jQuery(".msg_head")[ii].innerHTML;lastvalue=YAHOO.util.Cookie.get(GetPageName()+"_currentpanel");if(curvalue==lastvalue)indexvalue=ii}return indexvalue}function DisplayCurrentPanel(){jQuery(".msg_body").hide();var x=getPanelIndex();jQuery(".msg_body").eq(x).show();for(var sliderFunctions="",len=SheetCollection[x].length,i=0;i<len;i++)sliderFunctions=sliderFunctions+"Slider"+SheetCollection[x][i]+"();";if(SheetCollection[x].length>0)eval(sliderFunctions+" isSheet"+x+"FirstDisplay = false;");jQuery(".msg_head").click(function(){jQuery(this).next(".msg_body").slideToggle(10);var res=this.style.backgroundImage;if(res!="url(support/collapse_blue.jpg)")styleresult="url('support/collapse_blue.jpg')";else styleresult="url('support/expand_blue.jpg')";this.style.backgroundImage=styleresult;document.getElementById("currentpanel").value=jQuery(this)[0].innerHTML});jQuery(".msg_head").keypress(function(e){if(e.which==32){jQuery(this).next(".msg_body").slideToggle(10);var res=this.style.backgroundImage;if(res!="url(support/collapse_blue.jpg)")styleresult="url('support/collapse_blue.jpg')";else styleresult="url('support/expand_blue.jpg')";this.style.backgroundImage=styleresult;document.getElementById("currentpanel").value=jQuery(this)[0].innerHTML}})}</SCRIPT> 
<SCRIPT language=javascript type=text/javascript>var reqlist=[];(function(){var tabView=new YAHOO.widget.TabView("demo");YAHOO.log("The example has finished loading, as you interact with it, you will see log messages appearing here.","info","example")})();var submitted=false;function check_submit(objcaptcha){var pagevalues,fadediv,lightalertdiv;if(checkrequired(reqlist,"nopanel")===true){pagevalues=getPageSize();fadediv=document.getElementById("fadealert");lightalertdiv=document.getElementById("lightalert");fadediv.style.height=pagevalues[1];fadediv.style.display="block";lightalert.style.top=pagevalues[3]/2-125/2+pagevalues[4];lightalert.style.left=pagevalues[0]/2-320/2;lightalert.style.display="block";document.getElementById("btnCaptchaalert").focus()}else if(objcaptcha=="nocaptcha")if(submitted){alert("You have already submitted the form.  Please be patient.");submitted=false;return false}else{recalc_onclick("");submitted=true;document.formc.submit();return true}else{recalc_onclick("");pagevalues=getPageSize();fadediv=document.getElementById("fade");lightalertdiv=document.getElementById("light");fadediv.style.height=pagevalues[1];fadediv.style.display="block";lightalertdiv.style.top=pagevalues[3]/2-200/2+pagevalues[4];lightalertdiv.style.left=pagevalues[0]/2-320/2;lightalertdiv.style.display="block";GenerateCaptID();document.getElementById("txtcaptcha").value="";document.getElementById("txtcaptcha").focus()}}YAHOO.example.init=function(){function onButtonClickReset(){reset_onclick("")}function onButtonClickUpdate(){recalc_onclick("")}function onButtonClickPrint(){window.print()}function onButtonClickSubmit(){setCookieValues();check_submit()}var oPushButton1,oPushButton2,oPushButton3,oPushButton4,oPushButton5,oPushButton6,oPushButton7,oPushButton8;YAHOO.util.Event.onContentReady("submitbuttonsfrommarkup",function(){oPushButton1=new YAHOO.widget.Button("btnReset");oPushButton1.on("click",onButtonClickReset);oPushButton2=new YAHOO.widget.Button("btnUpdate");oPushButton2.on("click",onButtonClickUpdate);oPushButton3=new YAHOO.widget.Button("btnPrint");oPushButton3.on("click",onButtonClickPrint);oPushButton5=new YAHOO.widget.Button("btnPrintAll");oPushButton5.on("click",onButtonClickPrint);oPushButton4=new YAHOO.widget.Button("btnSubmit");oPushButton4.on("click",onButtonClickSubmit)});YAHOO.util.Event.onContentReady("divbottom",function(){oPushButton5=new YAHOO.widget.Button("btnReset_buttom");oPushButton5.on("click",onButtonClickReset);oPushButton6=new YAHOO.widget.Button("btnUpdate_buttom");oPushButton6.on("click",onButtonClickUpdate);oPushButton7=new YAHOO.widget.Button("btnPrint_buttom");oPushButton7.on("click",onButtonClickPrint);oPushButton9=new YAHOO.widget.Button("btnPrintAll_buttom");oPushButton9.on("click",onButtonClickPrint);oPushButton8=new YAHOO.widget.Button("btnSubmit_buttom");oPushButton8.on("click",onButtonClickSubmit)})}();function reset_onclick(){document.formc.reset();postcode();recalc_onclick("");ResetRatings()}function eequerystring(){var variable,key,value,ii,querystring,variables;querystring=document.location.search;if(querystring.length>0){variables=querystring.substring(1).split("&");for(ii=0;ii<variables.length;ii++){variable=variables[ii].split("=");key=unescape(variable[0]);value=unescape(variable[1]);if(document.formc[key]!=null)document.formc[key].value=value}}}function initial_update(){postcode("");eequerystring();recalc_onclick("")}</SCRIPT> 
<SCRIPT language=javascript type=text/javascript>
function showPanel(){jQuery(".msg_body").hide();var id=parseInt(document.getElementById("panelidz").value)-1;jQuery(".msg_body").eq(id).show();}</SCRIPT> 
<SCRIPT language=javascript type=text/javascript>
function setValueofBtn()
{

}</SCRIPT> 
<SCRIPT language=javascript type=text/javascript>
function ResetRatings(){
}
function SetSliders(){
}</SCRIPT> 
<SCRIPT language=javascript type=text/javascript>try{var myDialog,handleSubmit;myDialog=new YAHOO.widget.Dialog("myDialog",{width:"330px",fixedcenter:true,draggable:false,close:false});myDialog.cfg.queueProperty("postmethod","form");handleSubmit=function(){if(document.getElementById("showagain")!=null)document.getElementById("showagain").checked==true&&YAHOO.util.Cookie.set(GetPageName()+"_showdialogagain","no",{expires:CookieExpiryDate(new Date,3)});myDialog.hide()};function onButtonsReady(){var oOKButton;oOKButton=new YAHOO.widget.Button("ok-button");function onButtonClick(){if(document.getElementById("showagain")!=null)document.getElementById("showagain").checked==true&&YAHOO.util.Cookie.set(GetPageName()+"_showdialogagain","no",{expires:CookieExpiryDate(new Date,3)});myDialog.hide()}oOKButton.addListener("click",onButtonClick)}YAHOO.util.Event.onContentReady("buttons",onButtonsReady);myDialog.render();var cookievalue=YAHOO.util.Cookie.get(GetPageName()+"_showdialogagain");if(cookievalue==null||cookievalue!="no"){document.getElementById("showagain")!=null&&document.getElementById("showagain").focus();myDialog.show()}else myDialog.hide()}catch(err){}function changestylesheet(sheets){recalc_onclick("");if(sheets=="allsheet")document.getElementById("default").href="support/print_friendly.css";else document.getElementById("default").href="";window.print()}</SCRIPT>
<div align="center" style="width:950px; padding-top:15px;">
  <TABLE style="BORDER-COLLAPSE: collapse"  cellpadding="0" cellspacing="0" width=501>
    <tr style="HEIGHT: 17pt">
      <td class=ee157 colSpan=3 colid="1" rowid="11" sheetid="1"><FONT 
      class=ee148 face="Arial"><SUP>1</SUP> : Unit size and models are 
        approximated from spec sheets. Not all sizes are available.</FONT></td>
    </tr>
    <tr style="HEIGHT: 17pt">
      <td class=ee157 colSpan=2 colid="1" rowid="12" sheetid="1"><FONT 
      class=ee148 face="Arial"><SUP>2</SUP> : $1.00/therm based on 
        National Average.</FONT></td>
      <td class=ee163 colid="3" rowid="12" sheetid="1">&nbsp;</td>
    </tr>
    <tr style="HEIGHT: 15pt">
      <td class=ee165 vAlign=top align=left colid="1" rowid="13" 
      sheetid="1" width="203"><font face="Arial"><SPAN 
      style="MARGIN-TOP: 1px; Z-INDEX: 3; MARGIN-LEFT: 19px; WIDTH: 334px; POSITION: absolute; HEIGHT: 73px"><IMG 
      height=73 
      src="ATT01843.png" 
      width=334></SPAN></font></td>
      <td class=ee163 colSpan=2 colid="2" rowid="13" sheetid="1">&nbsp;</td>
    </tr>
    <tr style="HEIGHT: 17pt">
      <td class=ee157 colid="1" rowid="14" sheetid="1" width="201"><FONT 
      class=ee148 face="Arial"><SUP>3</SUP> : </FONT> <FONT 
      class=ee167 face="Arial">therm</FONT></td>
      <td class=ee163 colSpan=2 colid="2" rowid="14" sheetid="1">&nbsp;</td>
    </tr>
    <tr style="HEIGHT: 17pt">
      <td class=ee168 colid="1" rowid="15" sheetid="1" width="201">&nbsp;</td>
      <td class=ee163 colSpan=2 colid="2" rowid="15" sheetid="1">&nbsp;</td>
    </tr>
    <tr style="HEIGHT: 17pt">
      <td class=ee157 colid="1" rowid="16" sheetid="1" width="201" style="padding-top:10px;"><FONT 
      class=ee148 face="Arial"><SUP>4</SUP> : 360 Operating days per 
        year</FONT></td>
      <td class=ee163 colid="2" rowid="16" sheetid="1" width="241">&nbsp;</td>
      <td class=ee160 colid="3" rowid="16" sheetid="1">&nbsp;</td>
    </tr>
    <tr style="HEIGHT: 15pt">
      <td class=ee169 colid="1" rowid="17" sheetid="1" width="201">&nbsp;</td>
      <td class=ee163 colSpan=2 colid="2" rowid="17" 
  sheetid="1">&nbsp;</td>
    </tr>
  </TABLE>
</div>
</font>

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
</BODY>
</html>