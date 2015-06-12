<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SearchWithAPI.aspx.cs" Inherits="SearchWithAPI" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="tinytext">
        <tr>
            <td>
                <asp:Label ID="lblPageHeading" runat="server"></asp:Label>
            </td>
        </tr>    
        <tr>
            <td>
                <div>
                    <table width="100%" cellpadding="0" cellspacing="0" height="2" background="images/dotz2_orange.gif">
                        <tr> 
                            <td></td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr> 
            <td width="100%" class="text">
                <br />            
                <asp:Literal ID="litResultCount" runat="server"></asp:Literal>
                <asp:Literal ID="litResults" runat="server"></asp:Literal>
            </td>
        </tr>
    </table>              

    </div>
    </form>
</body>
</html>
