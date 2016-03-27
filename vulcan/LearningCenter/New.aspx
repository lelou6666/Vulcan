<%@ Page Language="C#" AutoEventWireup="true" CodeFile="New.aspx.cs" Inherits="New" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div align="center">
        <h1 style="text-align: left;">Vulcan Range Quiz</h1>
        <table width="700" style="text-align: left;">
            <tr>
                <td>
                    Welcome to the Vulcan Range Quiz. These phases were developed in conjunction with the Beolter Companies to provide basic and advanced training on Vulcan medium and heavy duty ranges. Our goal is to provide you with information you've learned from the phases to enhance your selling knowledge (so you close more sales in your territory).
                </td>
                <td>
                    <img src="Images/eq.jpg" alt="" />
                </td>
            </tr> 
            <tr>
                <td>
                    There are two phases. Complete Phase 1 before taking Phase 2. Lucky you—once you complete the phases, your name is entered in a drawing for an <b>Apple iPad</b>!<br /><br />
                    The phases will be offered for a designated period of time twice in 2013. The questions will differ slightly each time. If you have questions or comments please contact ___________. 
                </td>
                <td>
                    <img src="Images/ipad.jpg" alt="" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:ImageButton ID="btnPhase1" runat="server" Text="Start Phase I" ImageUrl="Images/Phase1.jpg" OnClick="btnPhase1_Click" />
                <br /><br />
                    <asp:ImageButton ID="btnPhase2" runat="server" Text="Start Phase II" ImageUrl="Images/Phase2.jpg"  OnClick="btnPhase2_Click"/>                                   
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
