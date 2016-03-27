<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Success.aspx.cs" Inherits="Success" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="/floatbox/floatbox.js"></script>
    <link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.5/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>
    
    <style>* {font-family:Verdana, Geneva, sans-serif;} </style>  
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table align="center" width="600" style="font-size:12px;">           
            <tr>
            	<td style="font-size:16px; border-bottom:1px dotted;"><b>Registration Successful</b></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblCopy" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:ImageButton ID="btnPhaseinside1" runat="server" Text="Start Phase I" ImageUrl="Images/Phase1.jpg" OnClick="btnPhaseinside1_Click"/>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:ImageButton ID="btnPhaseinside2" runat="server" Text="Start Phase II" ImageUrl="Images/Phase2.jpg" onclick="btnPhaseinside2_Click"/>
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
