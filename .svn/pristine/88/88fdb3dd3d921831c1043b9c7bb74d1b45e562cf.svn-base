<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Registration.aspx.cs" Inherits="Registration" %>
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
    <div id="RegistrationForm" runat="server">
    <table align="center" width="600">
            <tr>
                <td style="font-size:16px; border-bottom:1px dotted;"><b>Vulcan Range Quiz</b></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td align="left">
                    <table align="center" width="570" style="background-color:#e9e9e9; font-size:12px;">
                    	<tr>
                        	<td colspan="2" align="center" style="color:#7a7a7a; font-size:16px; padding:10px; border-bottom:1px dotted;"><b>REGISTRATION</b></td>
                        </tr>
                        <tr>
                            <td colspan="2">&nbsp;</td>
                        </tr>
                        <tr>
                            <td align="right">
                                First Name:
                            </td>
                            <td>
                                <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>                               
                                <asp:RequiredFieldValidator ID="Rf1" runat="server" ControlToValidate="txtFirstName"
                                    ErrorMessage="*" ValidationGroup="Registration" ForeColor="Red"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                Last Name:
                            </td>
                            <td>
                                <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>                              
                                <asp:RequiredFieldValidator ID="Rf2" runat="server" ControlToValidate="txtLastName"
                                    ErrorMessage="*" ValidationGroup="Registration" ForeColor="Red"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                Title:
                            </td>
                            <td>
                                <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>                               
                                <asp:RequiredFieldValidator ID="Rf3" runat="server" ControlToValidate="txtTitle"
                                    ErrorMessage="*" ValidationGroup="Registration" ForeColor="Red"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                Email:
                            </td>
                            <td>
                                <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>                                
                                <asp:RequiredFieldValidator ID="Rf4" runat="server" ControlToValidate="txtEmail"
                                    ErrorMessage="*" ValidationGroup="Registration" ForeColor="Red"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td  style="height:21px;" align="right">Experience Level:</td>
                            <td style="height:21px;">
                            	<table>
                                	<tr>
                                    	<td>
                                        	<asp:RadioButtonList ID="rblExperience" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="0-2">0 - 2 years</asp:ListItem>
                                                <asp:ListItem Value="2-5">2 - 5 years</asp:ListItem>
                                                <asp:ListItem Value="5-10">5 - 10 years</asp:ListItem>
                                                <asp:ListItem Value="10+">10 +</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        <td>
                                        	<asp:RequiredFieldValidator ID="ReqiredFieldValidator1" runat="server" ControlToValidate="rblExperience"
                                  			  ErrorMessage="*" ValidationGroup="Registration" ForeColor="Red"></asp:RequiredFieldValidator> 
                                        </td>
                                    </tr>
                                </table>                         
                            </td>                            
                        </tr>
                        <tr>
                            <td align="right">
                                City:
                            </td>
                            <td>
                                <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="Rf6" runat="server" ControlToValidate="txtCity" ErrorMessage="*"
                                    ValidationGroup="Registration" ForeColor="Red"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                State:
                            </td>
                            <td>
                                <asp:TextBox ID="txtState" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="Rf7" runat="server" ControlToValidate="txtState"
                                    ErrorMessage="*" ValidationGroup="Registration" ForeColor="Red"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                Zip:
                            </td>
                            <td>
                                <asp:TextBox ID="txtZip" runat="server"></asp:TextBox>                                            
                                            <asp:RequiredFieldValidator ID="Rf8" runat="server" ControlToValidate="txtZip" ErrorMessage="*"
                                                ValidationGroup="Registration" ForeColor="Red"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right" style="padding: 0 10px 10px 0;">
                                <asp:ImageButton ID="btnGo" runat="server" Text="Go" ImageUrl="Images/go.jpg" OnClick="btnGo_Click" ValidationGroup="Registration" />
                            </td>
                        </tr>
                    </table>                    
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
