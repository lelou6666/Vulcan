<%@ Page Language="VB" AutoEventWireup="false" CodeFile="addeditcontentreview.aspx.vb" Inherits="addeditcontentreview" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>AddEditContentRating</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="ltr_js" runat="server" />
    <div class="ektronPageContainer ektronPageInfo selected_editor" id="_dvContent"">
        <table class="ektronForm">
            <tr>
                <td class="label"><asp:Literal ID="ltr_uname" runat="server" /></td>
                <td class="readOnlyValue"><asp:Literal ID="ltr_uname_data" runat="server" /></td>
            </tr>
            <tr>
                <td class="label"><asp:Literal ID="ltr_date" runat="server" /></td>
                <td class="readOnlyValue"><asp:Literal ID="ltr_date_data" runat="server" /></td>
            </tr>
            <tr>
                <td class="label"><asp:Literal ID="ltr_rating" runat="server" /></td>
                <td class="value"><asp:Literal ID="ltr_rating_val" runat="server" /></td>
            </tr>
            <tr>
                <td class="label"><asp:Literal ID="ltr_status" runat="server" /></td>
                <td class="value">
                    <asp:DropDownList id="drp_status_data" runat="server">
                        <asp:ListItem Value="0">Pending</asp:ListItem>
                        <asp:ListItem Value="1">Approved</asp:ListItem>
                        <asp:ListItem Value="2">Rejected</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="label"><asp:Literal ID="ltr_review" runat="server" /></td>
                <td class="value"><asp:TextBox TextMode="MultiLine" ID="txt_review" runat="server" Columns="50" Rows="15" /></td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hdn_action" runat="server" />
    <asp:HiddenField ID="hdn_folderid" runat="server" />
    </form>
</body>
</html>
