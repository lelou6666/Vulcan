<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditDictionary.ascx.cs" Inherits="Controls_EditDictionary" %>
<b>Select Dictionary: </b>
<asp:dropdownlist id="dictionarySelector" runat="server">
</asp:dropdownlist><br />
<asp:label id="messageLabel" runat="server" width="265px"></asp:label>
<table width="100%">
    <tr>
        <td valign="top">
            <asp:panel id="searchPanel" runat="server" class="module">
                <asp:label id="Label2" runat="server">Find word:</asp:label>
                <asp:textbox id="findWordBox" runat="server"></asp:textbox>
                <asp:button id="findButton" runat="server" cssclass="button" onclick="findButton_Click"
                    text="Find" />
                <p>
                    <asp:listbox id="wordsFound" runat="server" height="164px" selectionmode="Multiple"
                        width="298px"></asp:listbox><br />
                    <asp:button id="deleteButton" runat="server" cssclass="button" onclick="deleteButton_Click"
                        text="Delete selected" width="298" />
                </p>
            </asp:panel>
        </td>
        <td valign="top">
            <asp:panel id="importPanel" runat="server" class="module" style="height: 124px; margin-bottom: 6px;">
                <br />
                <br />
                <asp:label id="Label3" runat="server">Import wordlist:</asp:label>
                <input id="importedFile" runat="server" name="importedFile" type="file" />
                <asp:button id="importButton" runat="server" cssclass="button" onclick="importButton_Click"
                    text="Import" />
            </asp:panel>
            <asp:panel id="addPanel" runat="server" class="module" style="height: 124px;">
                <br />
                <br />
                <br />
                <asp:label id="Label1" runat="server">Add a word:</asp:label>
                <asp:textbox id="addWordBox" runat="server"></asp:textbox>
                <asp:button id="addButton" runat="server" class="button" onclick="addButton_Click"
                    text="Add" />
            </asp:panel>
        </td>
    </tr>
</table>
