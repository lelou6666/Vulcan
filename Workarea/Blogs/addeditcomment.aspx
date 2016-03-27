<%@ Page Language="VB" AutoEventWireup="false" CodeFile="addeditcomment.aspx.vb" Inherits="blogs_addeditcomment" ValidateRequest="false"%>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Add/Edit Blog Comment</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="ltr_js" runat="server"/>
    <div class="ektronPageContainer">
       <div class="selected_editor" id="_dvContent"">
            <table id="Table8" class="ektronGrid">            
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_displayname" runat="server"/>:</td>
                    <td>
                        <asp:TextBox ID="txt_displayname" runat="server" Columns="40" MaxLength="50"/></td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_email" runat="server"/>:</td>
                    <td>
                        <asp:TextBox ID="txt_email" runat="server" Columns="40" MaxLength="50"/></td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_url" runat="server"/>:</td>
                    <td>
                        <asp:TextBox ID="txt_url" runat="server" Columns="40" MaxLength="500"/></td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_post" runat="server"/>:</td>
                    <td>
                        <asp:Literal ID="ltr_post_data" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_status" runat="server"/>:</td>
                    <td>
                        <asp:RadioButton ID="rb_approved" runat="server" GroupName="grp_status" /><br />
                        <asp:RadioButton ID="rb_pending" runat="server" GroupName="grp_status" /></td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_comment" runat="server"/>:</td>
                    <td>
                        <asp:TextBox ID="txt_comment" runat="server" TextMode="MultiLine" Columns="60" Rows="10"/></td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
