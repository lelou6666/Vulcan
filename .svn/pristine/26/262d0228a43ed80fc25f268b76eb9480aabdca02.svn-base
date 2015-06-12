<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Quiz.aspx.cs" Inherits="Quiz" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="/floatbox/floatbox.js"></script>
    <link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.5/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>
    
    <script type="text/javascript">
        function showResult() {
            $(document).ready(function () {
                var dialogOpts = { modal: true, width: '590', resizable: false, title: 'Answer', position: [20, 30] };
                $("#Answer").dialog(dialogOpts);               
            });
        } 
    </script>
    
    <style>* {font-family:Verdana, Geneva, sans-serif;} 
    .ui-widget-header{background:#666 url(Images/gradient.png);}</style>  
</head>
<body>
    <form id="form1" runat="server">
    <div align="center">        
        <asp:Label ID="lblStatus" runat="server" ForeColor="Red"></asp:Label><br />
        <asp:Button ID="btnReTest" runat="server" Text="Do Test Again" OnClick="btnReTest_Click" Visible="false" />
        <asp:Panel ID="pnlQuestion" runat="server" Width="600">
        	<table style="font-size:12px;">
            	<tr>
                	<td colspan="2" align="left" style="font-size:16px; border-bottom:1px dotted;">
                    	<b><asp:Label ID="lblHeading" runat="server"></asp:Label></b>
                    </td>
                </tr>
                <tr>
                	<td align="left" colspan="2" style="color:#7a7a7a; padding:5px; text-align:left;">
                    	<b><asp:Label ID="lblQuestionNum" runat="server"></asp:Label></b>
                    </td>
                </tr>
                <tr>
                	<td  valign="top" style="vertical-align:text-top; padding: 15px 10px 0 0;"><asp:Image runat="server" ID="iRange" /></td>
                	<td valign="top">
                    	<table align="left" style="text-align:left;">
                            <tr>
                                <td><asp:Label ID="lblQuestion1" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><br /></td>
                            </tr>
                            <tr>
                                <td><asp:RadioButtonList ID="rblChoice" runat="server" AutoPostBack="false"></asp:RadioButtonList></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right"><asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="right"><asp:ImageButton ID="btnChkAnswer" runat="server" Text="CheckAnswer" OnClick="btnChkAnswer_Click" ImageUrl="Images/answer.jpg" /></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right"><asp:ImageButton ID="btnNextQuestion" runat="server" Text="Next Question" OnClick="btnNextQuestion_Click" ImageUrl="Images/next.jpg" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            
            <asp:Label ID="lblAnswer" runat="server" ForeColor="Red"></asp:Label>           
            <asp:HiddenField ID="hfQuestionId" runat="server" />
            <asp:HiddenField ID="hfCorrectAnswer" runat="server" /> 
            <asp:HiddenField ID="hfReason" runat="server" />            
        </asp:Panel>
    </div>
    <div id="Answer" style="display:none; width:590px;">
        <table align="center" style="font-size:12px;">
            <tr>
                <td width="115" align="right" style="vertical-align:text-top; width:115px;"><b>Your Answer:</b></td>
                <td width="475">
                    <asp:Label ID="lblUserAnswer" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td width="115" align="right" style="vertical-align:text-top; width:115px;"><b>Correct Answer:</b></td>
                <td width="475">
                    <asp:Label ID="lblCorrectAnswer" runat="server"></asp:Label>
                </td>
            </tr>  
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label ID="lblReason" runat="server"></asp:Label>
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
