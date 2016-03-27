<%@ Control Language="VB" AutoEventWireup="false" CodeFile="edittaxonomy.ascx.vb"
    Inherits="edittaxonomy" %>

<script type="text/javascript">
    var controlid = "taxonomy_";

    Ektron.ready(function() { InitCustomPropertyEditting(); });
    
    function Validate(force){
        if(force){
          var taxonomyName = document.getElementById(controlid+"taxonomytitle").value;
            if(document.getElementById(controlid+"taxonomytitle").value==''){
                alert('<%=m_refMsg.GetMessage("js:alert taxonomy required field")%>');
                return false;
            }
            if(document.getElementById(controlid+'chkConfigContent') != null && document.getElementById(controlid+'chkConfigUser') != null && document.getElementById(controlid+'chkConfigGroup')!= null)
            {
                if(document.getElementById(controlid+'chkConfigContent').checked == false && document.getElementById(controlid+'chkConfigGroup').checked== false && document.getElementById(controlid+'chkConfigUser').checked==false)
                    { 
                            alert('<%=m_refMsg.GetMessage("js:alert configuration selection required")%>');
                            return false;
                    }
            }
            if((taxonomyName.indexOf('>') > -1) || (taxonomyName.indexOf('<') > -1) || (taxonomyName.indexOf('"') > -1)) {
                alert("The taxonomy name can not contain <, > or \"");
                return false;
            }
            else {
                document.forms[0].submit();
            }
        }
        document.forms[0].submit();
    }
    function Move(sDir, objList, objOrder) {
		if (objList.selectedIndex != null && objList.selectedIndex >= 0) {
			nSelIndex = objList.selectedIndex;
			sSelValue = objList[nSelIndex].value;
			sSelText = objList[nSelIndex].text;
			objList[nSelIndex].selected = false;
			if (sDir == "up" && nSelIndex > 0) {
				sSwitchValue = objList[nSelIndex -1].value;
				sSwitchText = objList[nSelIndex - 1].text;
				objList[nSelIndex].value = sSwitchValue;
				objList[nSelIndex].text = sSwitchText;
				objList[nSelIndex - 1].value = sSelValue;
				objList[nSelIndex - 1].text = sSelText;
				objList[nSelIndex - 1].selected = true;
			}
			else if (sDir == "dn" && nSelIndex < (objList.length - 1)) {
				sSwitchValue = objList[nSelIndex + 1].value;
				sSwitchText = objList[nSelIndex +  1].text;
				objList[nSelIndex].value = sSwitchValue;
				objList[nSelIndex].text = sSwitchText;
				objList[nSelIndex + 1].value = sSelValue;
				objList[nSelIndex + 1].text = sSelText;
				objList[nSelIndex + 1].selected = true;
			}
		}
		objOrder.value = "";
		for (i = 0; i < objList.length; i++) {
			objOrder.value = objOrder.value + objList[i].value;
			if (i < (objList.length - 1)) {
				objOrder.value = objOrder.value + ",";
			}
		}
	}
	function LoadReorderType(type){
	    document.location='taxonomy.aspx?action=reorder&reorder='+type+'&folderid=0&taxonomyid=<%=TaxonomyId %>&parentid=<%=TaxonomyParentId %>' ;
    }
    function OnInheritTemplateClicked(e){
        var control="<%=taxonomytemplate.ClientID%>";
        if(e.checked)
            document.getElementById(control).disabled=true;
        else
            document.getElementById(control).disabled=false;
        return true;
    }
    
	function RemoveTaxonomyImage(path) {
	    var elem = null;
	    var elemThumb = null;
	    elem = document.getElementById( '<%=taxonomy_image.ClientID%>' );
	    if (elem != null)
	    {
	        elem.value = '';
	    }
	    elemThumb = document.getElementById( '<%=taxonomy_image_thumb.ClientID%>' );
	    if ( elemThumb != null )
	    {
	        elemThumb.src = path;
	    }
	}
	function updateText(obj)
    {
        $ektron("#taxonomy_txtValue")[0].value = obj.value;
    }

    function InitCustomPropertyEditting() {
        var handlerUrl =
            "controls/content/CustomPropertyHandler.ashx?action=getcustompropobjectdata&objectid=" + 
            $ektron("#taxonomy_hdnTaxonomyId")[0].value;
        
        $ektron.getJSON(
            handlerUrl,
            function(data) {
                $ektron.each(data, function(index, item) { AddCustomPropertyToTable(item); });
            });
    }
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <%If m_strPageAction = "edit" Then%>
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <ul>
                <li>
                    <a href="#dvProperties">
                        <%=m_refMsg.GetMessage("properties text")%>
                    </a>
                </li>
                <li>
                    <a href="#dvMetadata">
                        <%=m_refMsg.GetMessage("custom properties")%>
                    </a>
                </li>
            </ul>
            <div id="dvProperties">
                <table class="ektronForm">
                    <tr>
                        <td class="label"><%=m_refMsg.GetMessage("lbl sitemap path")%>:</td>
                        <td class="value"><%=m_strCurrentBreadcrumb%></td>
                    </tr>
                    <tr>
                        <td class="label"><%=m_refMsg.GetMessage("lbl title")%>:</td>
                        <td class="value">
                            <asp:TextBox ID="taxonomytitle" runat="server" />
                            <asp:Label ID="lblLanguage" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label"><%=m_refMsg.GetMessage("lbl description")%>:</td>
                        <td class="value"><asp:TextBox ID="taxonomydescription" TextMode="MultiLine" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label"><%=m_refMsg.GetMessage("generic image")%>:</td>
                        <td class="value">
                            <span id="sitepath">
                                <asp:Literal ID="ltr_sitepath" runat="Server" />
                            </span>
                            <asp:TextBox Columns="30" id="taxonomy_image" runat="server" ReadOnly="true" />
                            &#160;
                            <a href="#" onclick="PopUpWindow('mediamanager.aspx?scope=images&upload=true&retfield=<%=taxonomy_image.ClientID%>&showthumb=false&autonav=0', 'Meadiamanager', 790, 580, 1,1);return false;">Change</a>
                            &nbsp;
                            <a href="#" onclick="RemoveTaxonomyImage('images/application/spacer.gif');return false">Remove</a>                         
                            <div class="ektronTopSpace"></div>
                            <asp:Image ID="taxonomy_image_thumb" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label"><%=m_refMsg.GetMessage("template label")%>:</td>
                        <td class="value"><asp:DropDownList ID="taxonomytemplate" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label"><%=m_refMsg.GetMessage("inherit label")%>:</td>
                        <td class="value">
                            <asp:CheckBox ID="inherittemplate" Text="(check here to inherit from the parent template)" runat="server" />
                            <asp:Label ID="lblInherited" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label"><%=m_refMsg.GetMessage("lbl category link")%>:</td>
                        <td class="value"><asp:TextBox ID="categoryLink" runat="server" /></td>
                    </tr>
                    <tr id="tr_enableDisable" runat="server" >
                        <td class="label"><%=m_refMsg.GetMessage("lbl enable/disable")%></td>
                        <td class="value">
                            <asp:CheckBox ID="chkEnableDisable" runat="server" />
                            <div class="ektronCaption"><%=m_refMsg.GetMessage("lbl enable/disable msg")%></div>
                            <br />
                            <asp:CheckBox ID="chkApplyDisplayAllLanguages" runat="server" />
                        </td>                    
                    </tr>
                    <tr id="tr_config" runat="server">
                        <td class="label"><%=m_refMsg.GetMessage("config page html title")%>:</td>
                        <td class="value">
                            <asp:CheckBox ID="chkConfigContent" runat="server" Text="Content"/>
                            <asp:CheckBox ID="chkConfigUser" runat="server" Text="User"/>
                            <asp:CheckBox ID="chkConfigGroup" runat="server"  Text="Group"/>
                        </td>
                    </tr>
                </table>
            </div>    
            <div id="dvMetadata">
                <div style="display:inline">
                    <asp:DropDownList runat="server" ID="availableCustomProp" AppendDataBoundItems="true">
                        <asp:ListItem Value="-1">
                        Select a Custom Property
                        </asp:ListItem>
                    </asp:DropDownList>
                    <a class="greenHover buttonAddTag" style="display:inline-block; vertical-align:middle;" onclick="AddCustomProperty();"></a>
                </div>
                <div class="ektronTopSpace"></div>
                <table id="customPropertyTable" class="ektronGrid" runat="server">
                    <tr class="title-header">
                        <td width="40%">Title</td>
                        <td width="20%">Data Type</td>
                        <td width="30">Value</td>
                        <td width="10%">Action</td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <%Else%>
        <table width="100%">
            <tr>
                <td valign="top"><asp:ListBox ID="OrderList" runat="server" /></td>
                <td id="td_moveicon" runat="server" align="left" width="80%">
                    <a href="javascript:Move('up', document.forms[0].taxonomy_OrderList, document.forms[0].taxonomy_LinkOrder)">
                        <img src="<%=(AppImgPath)%>moveup.gif" border="0" width="26" height="17" alt="<%=m_refMsg.GetMessage("move selection up msg")%>"
                            title="<%=(m_refMsg.GetMessage("move selection up msg"))%>" /></a><br />
                    <a href="javascript:Move('dn', document.forms[0].taxonomy_OrderList, document.forms[0].taxonomy_LinkOrder)">
                        <img src="<%=(AppImgPath)%>movedown.gif" border="0" width="26" height="17" alt="<%=m_refMsg.GetMessage("move selection down msg")%>"
                            title="<%=(m_refMsg.GetMessage("move selection down msg"))%>" /></a>
                </td>
            </tr>
        </table>
        <br />
        <table class="ektronForm" id="AllLangForm" runat="server">
           <tr id="tr_ordering" runat="server">
              <td class="label"><%=m_refMsg.GetMessage("lbl apply ordering languages")%>:</td>
              <td class="value"><asp:CheckBox ID="chkOrderAllLang" runat="server" /></td>
           </tr>
        </table>
        <input type="hidden" runat="server" id="LinkOrder" name="LinkOrder" value="" />
        <script type="text/javascript"><asp:Literal ID="loadscript" runat="server" /></script>
    <%End If%>
    <input type="hidden" id="alllanguages" runat="server"  />
    <input type="hidden" id="visibility" runat="server" />
    <input type="hidden" runat="server" id="txtValue" name="txtValue" />
    <input type="hidden" id='hdnSelectedIDS' name='hdnSelectedIDS' runat="server" />
    <input type="hidden" id='hdnSelectValue' name='hdnSelect' runat="server" />
    <input type="hidden" id="hdnTaxonomyId" name="hdnTaxonomyId" runat="server" />
    <asp:Literal Visible="false" ID="td_msg" runat="server" />

</div>
