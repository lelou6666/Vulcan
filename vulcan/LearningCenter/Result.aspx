﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Result.aspx.cs" Inherits="Result" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="/floatbox/floatbox.js"></script>
    <link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.5/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>
    
    <style>* {font-family:Verdana, Geneva, sans-serif;} </style>  
    
    <style media="print" type="text/css">
	.noprint {display:none;}
	</style>
</head>
<body>
    <form id="form1" runat="server">
        <div align="center">
            <table width="100%" cellpadding="0" cellspacing="0" style="font-size:12px;">
                <tr>
                    <td align="left" style="font-size:16px; border-bottom:1px dotted; width:400px;">
                        <b><asp:Label ID="lblPhase" runat="server"></asp:Label></b>
                    </td>
                    <td align="right" style="border-bottom:1px dotted;"><img src="Images/logo.gif" alt="Vulcan" height="25" style="padding:5px;" /></td>
                </tr>
                <tr colspan="2">
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:left;"><b><asp:Label ID="lblResult" runat="server"></asp:Label></b>
                    </td>
                </tr>
                <tr>
                	<td colspan="2" style="padding-top:15px;">
                        <table width="100%" cellpadding="0" cellspacing="0" style="font-size:12px;">
                            <tr>
                                <td align="right" width="100">Name:&nbsp;</td>
                                <td style="text-align:left;"><asp:Label ID="lblUsername" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="right">Date:&nbsp;</td>
                                <td style="text-align:left;"><asp:Label ID="lblDate" runat="server"></asp:Label></td>
                            </tr> 
                        </table>
                    </td>
                </tr>             
                <tr colspan="2">
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:left;">Thanks for finishing the Vulcan range training module. We hope it helps enhance your knowledge and assists you in your day-to-day selling.<br /><br />
Lucky you&mdash;once you complete the phases, your name is entered in a drawing! For more information view <a href="/Range_Quiz/contest_rules.pdf" target="_blank">contest rules</a>.
</td>
                </tr>
                <tr colspan="2">
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" align="right" class="noprint"><asp:ImageButton ID="btnPrint" runat="server" Text="Print Results" ImageUrl="Images/print.jpg"  OnClientClick="javascript:window.print();return false;" /></td>
                </tr>
                <tr colspan="2">
                    <td>&nbsp;</td>
                </tr>
                <tr colspan="2">
                    <td>&nbsp;</td>
                </tr>
                <tr colspan="2">
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td align="left" class="noprint">
                        <asp:ImageButton ID="btnPhaseinside1" runat="server" Text="Start Phase I" ImageUrl="Images/retake1.jpg" OnClick="btnPhase1_Click"/>
                    </td>
                    <td align="right" class="noprint">
                        <asp:ImageButton ID="btnPhaseinside2" runat="server" Text="Start Phase II" onclick="btnPhase2_Click"/>
                    </td>
              </tr>
            </table>
        </div>
    </form>
    
    <script type="text/javascript">
var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
</script>
<script type="text/javascript">
try {
var pageTracker = _gat._getTracker("UA-516396-39");
pageTracker._trackPageview();
} catch(err) {}</script>
</body>
</html>
