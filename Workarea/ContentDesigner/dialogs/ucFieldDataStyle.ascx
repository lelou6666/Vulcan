<%@ Control Language="C#" %>
<%@ Register TagPrefix="radcb" Namespace="Telerik.WebControls" Assembly="RadComboBox.NET2" %>
<%@ Register TagPrefix="picker" TagName="ColorPicker" Src="../../Foundation/RadControls/Editor/Controls/ColorPicker.ascx" %>
<script type="text/javascript" language="javascript" src="../DropDownMenu.js"></script>
<script type="text/javascript" language="javascript" src="../ColorPicker.js"></script>
<script runat="server">
    protected void Page_Load(Object src, EventArgs e)
    {
		Ektron.Cms.ContentAPI api = new Ektron.Cms.ContentAPI();
		Ektron.Cms.Common.EkMessageHelper refMsg = api.EkMsgRef;
		
		string skinPath = api.AppPath + "csslib/Editor/";
		this.FontColorPicker.SkinPath = skinPath;
		this.BackgroundColorPicker.SkinPath = skinPath;
		
		this.lblBgColor.InnerHtml = refMsg.GetMessage("lbl bg color");
		this.lblBold.InnerHtml = refMsg.GetMessage("lbl bold");
		this.lblFontColor.InnerHtml = refMsg.GetMessage("lbl font color");
		this.lblFontName.InnerHtml = refMsg.GetMessage("lbl font name");
		this.lblFontSize.InnerHtml = refMsg.GetMessage("lbl font size");
		this.lblItalic.InnerHtml = refMsg.GetMessage("lbl italic");
		this.lblTextAlign.InnerHtml = refMsg.GetMessage("lbl text align");
		this.lblTextLine.InnerHtml = refMsg.GetMessage("lbl text line");

		string sUnassigned = refMsg.GetMessage("generic unassign in parens");
		this.cboBold.Items[0].Text = sUnassigned;
		this.cboBold.Items[1].Text = refMsg.GetMessage("font weight normal");
		this.cboBold.Items[2].Text = refMsg.GetMessage("font weight bold");
		this.cboItalic.Items[0].Text = sUnassigned;
		this.cboItalic.Items[1].Text = refMsg.GetMessage("font style normal");
		this.cboItalic.Items[2].Text = refMsg.GetMessage("font style italic");
		this.cboTextAlign.Items[0].Text = sUnassigned;
		this.cboTextAlign.Items[1].Text = refMsg.GetMessage("text align left");
		this.cboTextAlign.Items[2].Text = refMsg.GetMessage("text align center");
		this.cboTextAlign.Items[3].Text = refMsg.GetMessage("text align right");
		this.cboTextAlign.Items[4].Text = refMsg.GetMessage("text align justify");
		this.cboTextLine.Items[0].Text = sUnassigned;
		this.cboTextLine.Items[1].Text = refMsg.GetMessage("text decoration none");
		this.cboTextLine.Items[2].Text = refMsg.GetMessage("text decoration underline");
		this.cboTextLine.Items[3].Text = refMsg.GetMessage("text decoration strikethrough");
		
        if (!Page.ClientScript.IsClientScriptBlockRegistered("EkFieldDataStyleScript"))
		{
			string ScriptText = EkFieldDataStyleScript.InnerText;
            ScriptText = ScriptText.Replace("<%=cboFontName.ClientID%>", cboFontName.ClientID);
            ScriptText = ScriptText.Replace("<%=cboFontSize.ClientID%>", cboFontSize.ClientID);
            ScriptText = ScriptText.Replace("<%=cboBold.ClientID%>", cboBold.ClientID);
            ScriptText = ScriptText.Replace("<%=cboItalic.ClientID%>", cboItalic.ClientID);
            ScriptText = ScriptText.Replace("<%=cboTextAlign.ClientID%>", cboTextAlign.ClientID);
            ScriptText = ScriptText.Replace("<%=cboTextLine.ClientID%>", cboTextLine.ClientID);
            ScriptText = ScriptText.Replace("<%=FontColorPicker.ClientID%>", FontColorPicker.ClientID);
            ScriptText = ScriptText.Replace("<%=BackgroundColorPicker.ClientID%>", BackgroundColorPicker.ClientID);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldDataStyleScript", ScriptText, true);
		}
        EkFieldDataStyleScript.Visible = false;
	}

	protected void cboFontProp_OnDataBound(Object sender, EventArgs e)
	{
		Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
		string sUnassigned = refMsg.GetMessage("generic unassign in parens");
		RadComboBoxItem item = ((RadComboBox)sender).Items[0];
		item.Text = sUnassigned;
		// RadComboBox fails to assign value with empty strings. :-(
		// so use "u/a" to represent unassigned (ie, empty string)
		item.Value = "u/a";
	}
</script>
<clientscript id="EkFieldDataStyleScript" runat="server">
function EkFieldDataStyleControl()
{
    this.read = function(oFieldElem)
    {
        if (null == oFieldElem) return;
        var objStyle = oFieldElem.style;
        var sStyle = "";
        if (objStyle)
        {
            sStyle = objStyle.fontFamily;
            this.setComboBoxValue(&lt;%=cboFontName.ClientID%&gt;, sStyle);

            sStyle = objStyle.fontSize;
            this.setComboBoxValue(&lt;%=cboFontSize.ClientID%&gt;, sStyle);

            sStyle = objStyle.fontWeight;
            this.setComboBoxValue(&lt;%=cboBold.ClientID%&gt;, sStyle);

            sStyle = objStyle.fontStyle;
            this.setComboBoxValue(&lt;%=cboItalic.ClientID%&gt;, sStyle);

            sStyle = objStyle.textAlign;
            this.setComboBoxValue(&lt;%=cboTextAlign.ClientID%&gt;, sStyle);

            sStyle = objStyle.textDecoration;
            this.setComboBoxValue(&lt;%=cboTextLine.ClientID%&gt;, sStyle);

            &lt;%=FontColorPicker.ClientID%&gt;.SelectColor(objStyle.color.toUpperCase());
            &lt;%=BackgroundColorPicker.ClientID%&gt;.SelectColor(objStyle.backgroundColor.toUpperCase());
        }
    }
    this.update = function(oFieldElem)
    {
        if (null == oFieldElem) return;
        var objStyle = oFieldElem.style;
        if (objStyle)
        {
			var objComboBox = null;
			objComboBox = &lt;%=cboFontName.ClientID%&gt;;
            var fontFamily = (&lt;%=cboFontName.ClientID%&gt;.GetText() || ""); // AllowCustomText="True"
            if (objComboBox.FindItemByText(fontFamily) != null)
            {
				fontFamily = objComboBox.GetValue();
            }
            objComboBox = &lt;%=cboFontSize.ClientID%&gt;;
            var fontSize = (&lt;%=cboFontSize.ClientID%&gt;.GetText() || ""); // AllowCustomText="True"
            if (objComboBox.FindItemByText(fontSize) != null)
            {
				fontSize = objComboBox.GetValue();
            }
            var fontWeight = (&lt;%=cboBold.ClientID%&gt;.GetValue() || "");
            var fontStyle = (&lt;%=cboItalic.ClientID%&gt;.GetValue() || "");
            var textAlign = (&lt;%=cboTextAlign.ClientID%&gt;.GetValue() || "");
            var textDecoration = (&lt;%=cboTextLine.ClientID%&gt;.GetValue() || "");
            if ("u/a" == fontFamily) fontFamily = "";
            if ("u/a" == fontSize) fontSize = "";
            if ("u/a" == fontWeight) fontWeight = "";
            if ("u/a" == fontStyle) fontStyle = "";
            if ("u/a" == textAlign) textAlign = "";
            if ("u/a" == textDecoration) textDecoration = "";
            if (fontFamily != objStyle.fontFamily)
            {
                // IE (7 at least) will insert empty string to cssText if there is fontFamily is an empty string.
                objStyle.fontFamily = fontFamily;
            }
            objStyle.fontSize = fontSize;
            objStyle.fontWeight = fontWeight;
            objStyle.fontStyle = fontStyle;
            objStyle.textAlign = textAlign;
            objStyle.textDecoration = textDecoration;

            objStyle.color = &lt;%=FontColorPicker.ClientID%&gt;.SelectedColor;
            objStyle.backgroundColor = &lt;%=BackgroundColorPicker.ClientID%&gt;.SelectedColor;  
            
            if ("" == fontFamily)
            {
                var text = objStyle.cssText;
                var newtext = text.replace(/\bFONT\-FAMILY\:\s*(\;|$)/i, "");
                if (text != newtext)
                {
                    try
                    {
                        objStyle.cssText = newtext;
                    }
                    catch (ex) {}
                }
            }
        } 
    }
    this.setComboBoxValue = function(comboBox, value)
    {
		if (!value) value = "u/a";
		comboBox.SetValue(value);
		comboBox.SetText(value);
        for (var i = 0; i < comboBox.Items.length; i++)
        {
            var itemValue = comboBox.Items[i].Value;
			if (!itemValue) itemValue = "u/a";
            if (value.toLowerCase() == itemValue.toLowerCase())
            {
                var itemText = comboBox.Items[i].Text;
				comboBox.SetText(itemText);
                break;
            }
        }
    }
}
var ekFieldDataStyleControl = new EkFieldDataStyleControl();
</clientscript>
<asp:PlaceHolder ID="FieldDataStyle" runat="server">
<tr>
    <td><label for="cboFontName" class="Ektron_StandardLabel" id="lblFontName" runat="server">Font Name:</label></td>
    <td>
        <radcb:radcombobox id="cboFontName" runat="server"
            DataSourceID="XmlDataSource"
            AllowCustomText="True"
            DataTextField="text"
            DataValueField="data" 
            OnDataBound="cboFontProp_OnDataBound">
        </radcb:radcombobox>

        <asp:XmlDataSource ID="XmlDataSource" runat="server"
            DataFile="../../ContentDesigner/FontName.xml">
        </asp:XmlDataSource>
	</td>
</tr>
<tr>
    <td><label for="cboFontSize" class="Ektron_StandardLabel" id="lblFontSize" runat="server">Font Size:</label></td>
    <td>
        <radcb:radcombobox id="cboFontSize" runat="server"
            DataSourceID="XmlDataSource2"
            AllowCustomText="True"
            DataTextField="text"
            DataValueField="data" 
            OnDataBound="cboFontProp_OnDataBound">
        </radcb:radcombobox>

        <asp:XmlDataSource ID="XmlDataSource2" runat="server"
            DataFile="../../ContentDesigner/FontSize.xml">
        </asp:XmlDataSource>
	</td>
</tr>
<tr>
    <td><label for="cboBold" class="Ektron_StandardLabel" id="lblBold" runat="server">Bold:</label></td>
    <td>
        <radcb:radcombobox id="cboBold" runat="server">
            <Items>
               <radcb:RadComboBoxItem Text="(Unassigned)" Value="u/a" />
               <radcb:RadComboBoxItem Text="Normal" Value="normal" />
               <radcb:RadComboBoxItem Text="Bold" Value="bold" />
            </Items>
        </radcb:radcombobox>
	</td>
</tr>
<tr>
    <td><label for="cboItalic" class="Ektron_StandardLabel" id="lblItalic" runat="server">Italic:</label></td>
    <td>
        <radcb:radcombobox id="cboItalic" runat="server">
            <Items>
               <radcb:RadComboBoxItem Text="(Unassigned)" Value="u/a" />
               <radcb:RadComboBoxItem Text="Normal" Value="normal" />
               <radcb:RadComboBoxItem Text="Italic" Value="italic" />
            </Items>
        </radcb:radcombobox>
	</td>
</tr>
<tr>
    <td><label for="cboTextAlign" class="Ektron_StandardLabel" id="lblTextAlign" runat="server">Text Alignment:</label></td>
    <td>
        <radcb:radcombobox id="cboTextAlign" runat="server">
            <Items>
               <radcb:RadComboBoxItem Text="(Unassigned)" Value="u/a" />
               <radcb:RadComboBoxItem Text="Left" Value="left" />
               <radcb:RadComboBoxItem Text="Center" Value="center" />
               <radcb:RadComboBoxItem Text="Right" Value="right" />
               <radcb:RadComboBoxItem Text="Justify" Value="justify" />
            </Items>
        </radcb:radcombobox>
	</td>
</tr>
<tr>
    <td><label for="cboTextLine" class="Ektron_StandardLabel" id="lblTextLine" runat="server">Text Line:</label></td>
    <td>
        <radcb:radcombobox id="cboTextLine" runat="server">
            <Items>
               <radcb:RadComboBoxItem Text="(Unassigned)" Value="u/a" />
               <radcb:RadComboBoxItem Text="None" Value="none" />
               <radcb:RadComboBoxItem Text="Underline" Value="underline" />
               <radcb:RadComboBoxItem Text="Strikethrough" Value="line-through" />
            </Items>
        </radcb:radcombobox>
	</td>
</tr>
<tr>
    <td><label for="chkColorUnassigned" class="Ektron_StandardLabel" id="lblFontColor" runat="server">Font Color:</label></td>
    <td><picker:colorpicker id="FontColorPicker" runat="server" /></td>
</tr>
<tr>
    <td><label for="chkColorUnassigned" class="Ektron_StandardLabel" id="lblBgColor" runat="server">Background Color:</label></td>
    <td><picker:colorpicker id="BackgroundColorPicker" runat="server" /></td>
</tr>
</asp:PlaceHolder> 
