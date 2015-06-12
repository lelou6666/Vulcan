<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="Admin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Vulcan Range Quiz:::Admin</title>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.2.6/jquery.min.js"></script> 
	<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.5.3/jquery-ui.min.js"></script>
    <link rel="stylesheet" href="Images/ui.all.css" type="text/css" media="screen" />
    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtFrom").datepicker({ showOn: 'button', buttonImageOnly: true, buttonImage: 'Images/icon_cal.png' });
            $("#txtTo").datepicker({ showOn: 'button', buttonImageOnly: true, buttonImage: 'Images/icon_cal.png' });
        });
	</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h2>Vulcan Range Quiz</h2>   
    <table>
        <tr>
            <td>
                Date From:
            </td>
            <td>
                <asp:TextBox ID="txtFrom" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfFrom" runat="server" ControlToValidate="txtFrom" ErrorMessage="Please Select From Date" ForeColor="Red" ValidationGroup="Test"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Date To:
            </td>
            <td>
                <asp:TextBox ID="txtTo" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfTo" runat="server" ControlToValidate="txtTo" ErrorMessage="Please Select To Date" ForeColor="Red" ValidationGroup="Test"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnExportToCsv" runat="server" Text="Export To Csv" 
                    onclick="btnExportToCsv_Click" ValidationGroup="Test" />
            </td>
        </tr>
    </table>
     
    <asp:GridView ID="gvResults" runat="server">
    </asp:GridView>


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
