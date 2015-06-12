<%@ Page Language="VB" AutoEventWireup="false" CodeFile="addblogsubject.aspx.vb" Inherits="blogs_addblogsubject" ValidateRequest="false"%>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Add Blog Subject</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="ltr_js" runat="server"></asp:Literal>
   <div class="selected_editor" id="_dvContent"">
        <table>
            <tr>
                <td>&#160;</td>
            </tr>
            <tr>
                <td><asp:textbox runat="server" id="txt_subject" /></td>
            </tr>
            <tr>
                <td>&#160;</td>
            </tr>
        </table>        
    </div>
    </form>
</body>
</html>
