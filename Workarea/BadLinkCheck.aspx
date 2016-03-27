<%@ Page Language="VB" AutoEventWireup="false" CodeFile="BadLinkCheck.aspx.vb" Inherits="BadLinkCheck" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Bad Link Checker</title>
    <asp:Literal id="StyleSheetJS" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
		<div id="dhtmltooltip"></div>
		<div class="ektronPageHeader">
		    <div class="ektronTitlebar"><%= m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl linkcheck title")) %></div>
		    <div class="ektronToolbar" id="htmToolBar" runat="server">
			    <table id="toolbar">
			        <tr>
			            <td class="button" id="printWrapper" runat="server">
			                <asp:ImageButton id="btnPrint" runat="server" ImageUrl="images/UI/Icons/print.png" OnClientClick="PrintReport()" CssClass="button"/>
	                    </td>
	                    <td class="button" id="checkWrapper" runat="server">
	                        <asp:ImageButton id="btnCheck" runat="server" ImageUrl="images/UI/icons/linkGo.png" CssClass="button"/>
	                    </td>
	                    <td class="button" id="cancelWrapper" runat="server">
	                        <asp:ImageButton id="btnCancel" runat="server" ImageUrl="images/UI/Icons/cancel.png" CssClass="button"/>
	                    </td>
	                    <td>
	                        <asp:literal id="btnHelp" runat="server"/>
	                    </td>
                    </tr>
                </table>
		    </div>
		</div>
		<div class="ektronPageContainer ektronPageInfo">
            <ul id="menu">
              <li id="nav-1"><asp:LinkButton ID="lnkTabStatus" runat="server" OnClick="lnkTabStatus_Click" BackColor="White" Font-Underline=false Font-Bold=true>Status</asp:LinkButton></li>
              <li id="nav-2"><asp:LinkButton ID="lnkTabTestURL" runat="server" OnClick="lnkTabTestURL_Click" Font-Underline=false Font-Bold=true>Select CMS Files</asp:LinkButton></li>
            </ul>
            <div id="contents" title="Bad Link Report" >
                 <div id="BadLinkReportGrid">
                        <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                        <asp:View ID="Status" runat="server">
                        <table>
                         <tr><td>
                            <asp:Label id="lblStatus" runat="server" Text="Status:" Font-Bold="True"/>
                            &nbsp;
                            <asp:Label id="txtStatus" runat="server" Text="" />
                         </td></tr>
                         <tr><td>
                            <asp:Label ID="txtReport" runat="server" Text=""/>
                        </td></tr>
                        </table>
                        </asp:View>

                        <asp:View ID="TestURL" runat="server">
                        <table>
                         <tr><td>
                            <asp:Label ID="lblURL" runat="server" Text="URL to Check:" Font-Bold="true"/>
                            <asp:TextBox ID="txtURL" runat="server"/>
                         </td></tr>
                        </table>
                        </asp:View>

                        </asp:MultiView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
