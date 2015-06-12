<%@ Control Language="C#" %>
<script runat="server">
    protected void Page_Load(Object src, EventArgs e)
    {
		Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
        this.lblWidth.InnerHtml = refMsg.GetMessage("lbl size colon");
        this.lblHeight.InnerHtml = refMsg.GetMessage("lbl imagetool resize height");
        string sChar = refMsg.GetMessage("lbl chars");
        this.lblunit1.InnerHtml = sChar;
        this.lblunit2.InnerHtml = sChar;
 
        //StylePanel.Enabled = _enabled;
        if (!Page.ClientScript.IsClientScriptBlockRegistered("EkFieldStyleScript"))
		{
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldStyleScript", EkFieldStyleScript.InnerText, true);
		}
        EkFieldStyleScript.Visible = false;
	}
</script>
<clientscript id="EkFieldStyleScript" runat="server">
function EkFieldStyleControl()
{
	this.read = function(oFieldElem) 
	{
	    if ("text" == oFieldElem.type)
	    {
	        document.getElementById("Widthpx").value = oFieldElem.getAttribute("size");
	        document.getElementById("Heightpx").value = "";
	    }
	    else
	    {
	        document.getElementById("Widthpx").value = oFieldElem.getAttribute("cols");
	        document.getElementById("Heightpx").value = oFieldElem.getAttribute("rows");
	    }
	};
	this.update = function(oFieldElem)
	{
	    var w = document.getElementById("Widthpx").value;
	    var h = document.getElementById("Heightpx").value;
	    oFieldElem.removeAttribute("size");
	    oFieldElem.removeAttribute("cols");
	    oFieldElem.removeAttribute("rows");
	    if ("text" == oFieldElem.type)
	    {
	        if (w.length > 0 && !isNaN(w))
	        {
	            oFieldElem.setAttribute("size", w);
	        }
	    }
	    else if ("TEXTAREA" == oFieldElem.tagName) 
	    {
	        if (w.length > 0 && !isNaN(w))
	        {
	            oFieldElem.setAttribute("cols", w);
	        }
	        if (h.length > 0 && !isNaN(h))
	        {
	            oFieldElem.setAttribute("rows", h);
	        }
	    }
	};
	this.updateControl = function(type)
	{
	    $lblWidth = $ektron(".lblWidth");
	    $lblHeight = $ektron(".lblHeight");
	    $lblunit2 = $ektron(".lblunit2");
	    $txtWidth = $ektron(".Ektron_Dimensions_Width");
	    $txtHeight = $ektron(".Ektron_Dimensions_Height");
	    $txtWidth.attr("disabled","").attr("readonly","");
        $txtHeight.attr("disabled","").attr("readonly","");
	    switch (type.toLowerCase())
	    {
	        case "passwordfield":
	        case "readonlyfield":
	        case "textfield":
	            $lblHeight.hide();
	            $txtHeight.hide();
	            $lblunit2.hide();
	            $lblWidth.text(ResourceText.sSize);
	            if ("" == $txtWidth.val())
	            {
	                $txtWidth.val("24");
	            }
	            break;
	        case "textareafield":
	            $lblHeight.show();
	            $txtHeight.show();
	            $lblunit2.show();
	            $lblWidth.text(ResourceText.sWidth);
	            $lblHeight.text(ResourceText.sHeight);
	            if ("" == $txtWidth.val())
	            {
	                $txtWidth.val("24");
	            }
	            if ("" == $txtHeight.val())
	            {
	                $txtHeight.val("3");
	            }
	            break;
	        case "richareafield":    
	        case "hiddenfield":
	        default:
	            document.getElementById("FieldStyleContainer").disabled = false;
                $txtWidth.val("").attr("disabled","disabled").attr("readonly","readonly");
                $txtHeight.val("").attr("disabled","disabled").attr("readonly","readonly");
	            break;
	    }
	};
}
var ekFieldStyleControl = new EkFieldStyleControl();
</clientscript>
<asp:Panel ID="StylePanel" runat="server">
    <style type="text/css">
        .Ektron_Dimensions_Width, .Ektron_Dimensions_Height
        {
            text-align: right;
	        width: 4em;
	        vertical-align: middle !important;
        }
    </style>
	<fieldset id="FieldStyleContainer">
		<legend id="lblStyle" runat="server">Dimensions</legend>
		<div class="Ektron_TopSpaceVeryVerySmall">
		    <table style="width: 100%;">
		    <tr>
		        <td style="width: 50%;">
		            <label for="Widthpx" class="lblWidth Ektron_StandardLabel" id="lblWidth" runat="server">Width:</label>
                    <input id="Widthpx" class="Ektron_Dimensions_Width" maxlength="4" value="24" /> <label class="lblunit1 Ektron_StandardLabel" id="lblunit1" runat="server">characters</label>
                </td>
		        <td style="width: 50%;">
		            <label for="Heightpx" class="lblHeight Ektron_StandardLabel" id="lblHeight" runat="server">Height:</label>
                    <input id="Heightpx" class="Ektron_Dimensions_Height" maxlength="4" value="3" /> <label class="lblunit2 Ektron_StandardLabel" id="lblunit2" runat="server">characters</label>
		        </td>
		    </tr>
		    </table>
		</div>
	</fieldset>
</asp:Panel> 