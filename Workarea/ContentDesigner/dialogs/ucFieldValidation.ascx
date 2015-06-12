<%@ Control language="c#" CodeFile="ucFieldValidation.ascx.cs" Inherits="Ektron.ContentDesigner.Dialogs.ucFieldValidation" AutoEventWireup="false" %>
<%@ Register TagPrefix="radcb" Namespace="Telerik.WebControls" Assembly="RadComboBox.NET2" %>
<%@ Register TagPrefix="ek" TagName="FieldTreeViewControl" Src="ucFieldTreeView.ascx" %>
<clientscript id="EkFieldValidationScript" runat="server">
Ektron.ready(function()
{
	var cboV8n = &lt;%=cboV8n.ClientID%&gt;;
	cboV8n.ClientID = "&lt;%=cboV8n.ClientID%&gt;";
	var cboDataType = &lt;%=cboDataType.ClientID%&gt;;
	cboDataType.ClientID = "&lt;%=cboDataType.ClientID%&gt;";
	var cboValEx = &lt;%=cboValEx.ClientID%&gt;;
	cboValEx.ClientID = "&lt;%=cboValEx.ClientID%&gt;";
	var validationTree = 
	{
		ClientID: "&lt;%=validationTree.ClientID%&gt;"
	,	fsTree:
		{
			ClientID: "&lt;%=validationTree.fsTree.ClientID%&gt;"
		}
	};
		
	window.ekFieldValidationControl = new EkFieldValidationControl("&lt;%=this.ClientID%&gt;",
		{
			cboV8n: cboV8n
		,	cboDataType: cboDataType
		,	cboValEx: cboValEx
		,	validationTree: validationTree
		});
});
</clientscript>
<asp:PlaceHolder ID="FieldValidation" runat="server">
<tr>
    <td><label for="cboV8n" class="Ektron_TopLabel" id="lblValidation" runat="server">Validation:</label></td>
    <td>
		<input type="hidden" name="v8nSelectedValue" id="v8nSelectedValue" />
		<input type="hidden" name="datatypeSelectedValue" id="datatypeSelectedValue" />
	</td>
</tr>
<tr>
    <td>
        <radcb:radcombobox id="cboV8n" runat="server"            
            OnClientSelectedIndexChanged="cboV8n_OnClientSelectedV8nIndexChanged"
            OnItemsRequested="cboV8n_ItemsRequested"
            OnClientItemsRequested="cboV8n_SetSelectedItem"
            EnableLoadOnDemand="true"
            Width="230px">
        </radcb:radcombobox>
    </td>
</tr>
<tr>
    <td><label for="ErrorMessage" class="Ektron_TopLabel" id="lblMessage" runat="server">Message:</label></td>
</tr>
<tr>
    <td><input type="text" name="ErrorMessage" id="ErrorMessage" class="Ektron_StandardTextBox" /></td>
</tr>
<tr>
    <td>
        <fieldset id="fsCustomV8n" class="Ektron_TopSpaceVerySmall">
            <legend id="lblCustomValidation" runat="server">Custom Validation</legend>
            <table width="100%">
                <tr>
                    <td><label for="cboDataType" class="Ektron_TopLabel" id="lblDataType" runat="server">Data Type:</label></td>
                </tr>
                <tr>
                    <td>
                        <radcb:radcombobox id="cboDataType" runat="server"                                
                            OnItemsRequested="cboDataType_ItemsRequested"
                            OnClientDropDownOpening="onValComboOpening"
                            OnClientItemsRequested="cboDataType_SetSelectedItem"
                            EnableLoadOnDemand="true"
                            Width="230px">
                        </radcb:radcombobox>
                    </td>
                </tr>
                <tr>
                    <td><label for="txtValXPath" class="Ektron_TopLabel" id="lblCondition" runat="server">Condition:</label></td>
                </tr>
                <tr>
                    <td>
                        <input type="text" name="txtValXPath" id="txtValXPath" class="Ektron_StandardTextBox" />
                    </td>
                </tr>
                <tr>
                    <td><label for="cboValEx" class="Ektron_TopLabel" id="lblExamples" runat="server">Examples:</label></td>
                </tr>
                <tr>
                    <td>
                        <radcb:radcombobox id="cboValEx" runat="server"
                            OnItemsRequested="cboValEx_ItemsRequested"
                            EnableLoadOnDemand="true"
                            OnClientSelectedIndexChanged="cboValEx_OnClientSelectedIndexChanged"
                            OnClientDropDownOpening="onValComboOpening"
                            Width="230px">
                        </radcb:radcombobox>
                    </td>
                </tr>
            </table>
        </fieldset>
    </td>
    <td width="100%">
        <ek:FieldTreeViewControl id="validationTreeControl" runat="server"  />   
    </td>
</tr>
</asp:PlaceHolder> 
