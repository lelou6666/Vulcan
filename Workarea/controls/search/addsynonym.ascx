<%@ Control Language="VB" AutoEventWireup="false" CodeFile="addsynonym.ascx.vb" Inherits="addsynonym" %>

<asp:Literal ID="PostBackPage" runat="server" />

<div style="display: none; border: 1px; background-color: white; position: absolute;
    top: 48px; width: 100%; height: 1px; margin: 0px auto;" id="dvHoldMessage">
    <table border="1px" width="100%" style="background-color: #fff;">
        <tr>
            <td valign="top" align="center" style="white-space: nowrap">
                <h3 style="color: red">
                    <strong>
                        <%=m_refMsg.GetMessage("one moment msg")%>
                    </strong>
                </h3>
            </td>
        </tr>
    </table>
</div>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
    <div class="ektronPageInfo">
        <table class="ektronGrid">
            <tr>
                <td class="label"><label for="setName"><%= m_refMsg.GetMessage("lbl synonym set name") %>:</label></td>
                <td class="value">
                    <asp:TextBox ID="synonymName" runat="server" MaxLength="50" onkeypress="var k = event.keyCode ? event.keyCode : event.charCode ? event.charCode : event.which; return /^([^,;<>])$/.test(String.fromCharCode(k) );" />
                </td>
            </tr>
            <tr>
                <td class="label"><label class="setTerms" for="Terms"><%= m_refMsg.GetMessage("lbl synonym header terms") %>:</label></td>
                <td class="value">
                    <asp:TextBox ID="synonymTerms" runat="server" TextMode="MultiLine" onkeypress="var k = event.keyCode ? event.keyCode : event.charCode ? event.charCode : event.which; return /^([^,()<>])$/.test(String.fromCharCode(k) );"/>
                    <p class="ektronCaption"><%= m_refMsg.GetMessage("msg please enter synonyms") %></p>
                    <asp:Button ID="checkDuplicates" runat="server" CssClass="checkDuplicates" OnClientClick="document.Form1.addsynonym$submitMode.value=1" /><asp:HiddenField ID="submitMode" runat="server" value="1" />
                    <asp:Literal ID="showDuplicates" runat="server" />
                </td>
            </tr>
        </table>
    </div>
</div>

<asp:HiddenField ID="termID" runat="server" />