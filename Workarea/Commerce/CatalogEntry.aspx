<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CatalogEntry.aspx.vb" Inherits="Ektron.Cms.Commerce.Workarea.CatalogEntry.CatalogEntry" %>
<%@ Register TagPrefix="ucEktron" TagName="Item" Src="CatalogEntry/Items/Items.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="Media" Src="CatalogEntry/Media/Media.ascx" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../controls/Editor/ContentDesignerWithValidator.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
        <title>Catalog Entry</title>
        <script type="text/javascript">
            <!--            //--><![CDATA[//><!--
            NavFrameSrc = "";
            function ResizeFrame(val)
            {
                if (!(top.location == document.location))
                    top.ResizeFrame(0);
            }
            function CanNavigate()
            {
                // Block navigation while this page loaded (called from top window-object):
                return false;
            }
            function CanShowNavTree()
            {
                // Block displaying the navigation tree while this page loaded (called from top window-object):
                return false;
            }
            Ektron.ready(function()
            {
                $ektron("#frmMain").click(function()
                {
                    LogActivity();
                });
                //TABS
                var tabsContainers = $ektron(".tabContainer");
                tabsContainers.tabs();

                // PLEASE WAIT AND ADD TAXONOMY MODAL
                $ektron("#divTimeOut, #pleaseWait, #FrameContainer").modal(
                    {
                        trigger: '',
                        modal: true,
                        toTop: true,
                        onShow: function(hash)
                        {
                            hash.o.fadeIn();
                            hash.w.fadeIn();
                        },
                        onHide: function(hash)
                        {
                            hash.w.fadeOut("fast");
                            hash.o.fadeOut("fast", function()
                            {
                                if (hash.o)
                                {
                                    hash.o.remove();
                                }
                            });
                        }
                    });
            });
            //--><!]]>
        </script>
        <style type="text/css">
            <!--/*--><![CDATA[/*><!--*/
                div#divCategories ul.ektree {position:static !important;}		        
		        div#pleaseWait
                {
                    width: 128px;
                    height: 128px;
                    margin: -64px 0 0 -64px;
                    background-color: #fff;
                    background-image: url("../images/ui/loading_big.gif");
                    backgground-repeat: no-repeat;
                    text-indent: -10000px;
                    border: none;
                    padding: 0;
                    top: 50%;
                }
                
                div#FrameContainer
                {
                    width: 80%;
                    height: 60%;
                    margin: -250px 0 0 -500px;
                }
                #ChildPage {frameborder:1; width:100%; height:100%; scrolling:auto; background-color: white; }

                div#pleaseWait {display: none;}
                
                
                table.pseudoGrid
                {
                	border-spacing:0;
	                border-collapse:collapse;
	                width:90%;
	                background-color:White;
                }

                table.pseudoGrid tr td {
	                padding: .25em .5em;
	                vertical-align: middle;
	                text-align:left;
                }

                table.pseudoGrid .label
                {
                	width: 1%;
                	color:#1d5987;
                    white-space:nowrap;
	                font-weight:bold;
	            }
                
                table.ektronMetadataForm {width:97%;}
            /*]]>*/-->
        </style>
    </head>
    <body onunload="ResizeFrame(0);">
        <form id="frmMain" runat="server">
            <asp:Literal ID="EnhancedMetadataArea" runat="server" />
            
            
            <div id="divTimeOut" class="ektronWindow ektronModalStandard">
                <div class="ektronModalHeader">
                    <h3>
                        <span class="headerText"></span> <a href="#Close" class="ektronModalClose"
                            title="Close"><span style="visibility: hidden;">Close</span></a>
                    </h3>
                </div>
                <div class="ektronModalBody">
                    <asp:Literal ID="lbl_SessionExpiringLabel" runat="server" /> 
                    <div id="sessionCountDown" class="sessionCountDown">120</div>
                    <div class="ektronModalButtonWrapper clearfix">
                        <ul class="buttonWrapper clearfix">
                            <li><a class="button buttonRight greenHover" onclick="refreshPage();"><asp:Literal ID="lbl_ContinueEditingLabel" runat="server" /> </a></li>
                        </ul>
                    </div>
                </div>
            </div>
            
            <div class="ektronWindow" id="pleaseWait">
                <h3><strong><asp:Literal ID="ltr_holdmsg" runat="server" /></strong></h3>
            </div>
            <div class="ektronPageContainer ektronPageInfo">
                <div class="ektronTabUpperContainer">
                    <table cellspacing="0" class="pseudoGrid">
                        <tr>
                            <td class="label"><asp:Literal ID="lbl_GenericTitleLabel" runat="server" /></td>
                            <td class="value">
                                <input id="content_title" runat="server" name="content_title" type="text" size="50" maxlength="200" onkeypress="return CheckKeyValue(event, '34,13');" value="" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ektronPageTabbed">
                <div class="tabContainerWrapper">
                    <div class="tabContainer">
                        <ul>
                            <li id="liContent" runat="server" visible="false"><a href="#divContent"><asp:Literal ID="litTabContentLabel" runat="server" /></a></li>
                            <li id="liSummary" runat="server" visible="false"><a href="#divSummary"><asp:Literal ID="litTabSummaryLabel" runat="server" /></a></li>
                            <li id="liProperties" runat="server" visible="false"><a href="#divProperties"><asp:Literal ID="litTabPropertiesLabel" runat="server" /></a></li>
                            <li id="liPricing" runat="server" visible="false"><a href="#divPricing"><asp:Literal ID="litTabPricingLabel" runat="server" /></a></li>
                            <li id="liAttributes" runat="server" visible="false"><a href="#divAttributes"><asp:Literal ID="litTabAttributesLabel" runat="server" /></a></li>
                            <li id="liMedia" runat="server" visible="false"><a href="#divMedia"><asp:Literal ID="litTabMediaLabel" runat="server" /></a></li>
                            <li id="liItems" runat="server" visible="false"><a href="#divItems"><asp:Literal ID="litTabItemsLabel" runat="server" /></a></li>
                            <li id="liMetadata" runat="server" visible="false"><a href="#divMetadata"><asp:Literal ID="litTabMetadataLabel" runat="server" /></a></li>
                            <li id="liAlias" runat="server" visible="false"><a href="#divAlias"><asp:Literal ID="litTabAliasLabel" runat="server" /></a></li>
                            <li id="liSchedule" runat="server" visible="false"><a href="#divSchedule"><asp:Literal ID="litTabScheduleLabel" runat="server" /></a></li>
                            <li id="liCategory" runat="server" visible="false"><a href="#divCategories"><asp:Literal ID="litTabCateogoryLabel" runat="server" /></a></li>
                        </ul>
                        <div id="divContent" runat="server" visible="false">
                            <ektron:ContentDesigner ID="contentEditor" runat="server" />
                        </div>
                        <div id="divSummary" runat="server" visible="false">
                            <ektron:ContentDesigner ID="summaryEditor" runat="server" />
                        </div>
                        <div id="divProperties" runat="server" visible="false">
                            <table id="tblmain" runat="server">
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_sku" runat="server"/>:</td>
                                    <td>
                                        <asp:TextBox ID="txt_sku" runat="server" Columns="20" MaxLength="75" />
                                        <asp:CheckBox ID="chk_field" runat="server" Text="Use Field" />
                                        <asp:DropDownList ID="drp_field" runat="Server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label"><span style="padding-right:.5em;"><asp:Literal ID="ltr_quantity" runat="server"/>:</span></td>
                                    <td>
                                        <asp:TextBox ID="txt_quantity" runat="server" Columns="20" MaxLength="9" />
                                        <asp:CheckBox ID="chk_field2" runat="server" Text="Use Field" />
                                        <asp:DropDownList ID="drp_field2" runat="Server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_taxclass" runat="server"/>:</td>
                                    <td><asp:DropDownList ID="drp_taxclass" runat="Server" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_avail" runat="server"/>:</td>
                                    <td><asp:CheckBox ID="chk_avail" runat="server" Checked="true"/></td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_buyable" runat="server"/>:</td>
                                    <td><asp:CheckBox ID="chk_buyable" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:literal ID="ltr_ship" runat="server"/></td>
                                    <td>
                                        <table id="tbl_ship" class="ektronForm">
                                            <tr>
                                                <td class="label"><asp:Literal ID="ltr_tangible" runat="server" />:</td>
                                                <td><asp:checkbox ID="chk_tangible" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="label"><asp:Literal ID="ltr_height" runat="server" />:</td>
                                                <td>
                                                    <asp:TextBox ID="txt_height" runat="server" Columns="20" MaxLength="7" />
                                                    &#160;
                                                    <asp:Literal ID="ltr_heightmeasure" runat="server"/><br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="label"><asp:Literal ID="ltr_width" runat="server" />:</td>
                                                <td>
                                                    <asp:TextBox ID="txt_width" runat="server" Columns="20" MaxLength="7" />
                                                    &#160;
                                                    <asp:Literal ID="ltr_widthmeasure" runat="server"/><br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="label"><asp:Literal ID="ltr_length" runat="server" />:</td>
                                                <td>
                                                    <asp:TextBox ID="txt_length" runat="server" Columns="20" MaxLength="7" />
                                                    &#160;
                                                    <asp:Literal ID="ltr_lengthmeasure" runat="server"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="label"><asp:Literal ID="ltr_weight" runat="server" />:</td>
                                                <td>
                                                    <asp:TextBox ID="txt_weight" runat="server" Columns="20" MaxLength="7" />
                                                    &#160;
                                                    <asp:Literal ID="ltr_weightmeasure" runat="server"/>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:literal ID="ltr_inv" runat="server"/></td>
                                    <td>
                                        <table id="tbl_inv" class="ektronForm">
                                            <tr>
                                                <td class="label"><asp:Literal ID="ltr_disableInv" runat="server" />:</td>
                                                <td><asp:checkbox ID="chk_disableInv" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="label"><asp:Literal ID="ltr_instock" runat="server" />:</td>
                                                <td><asp:TextBox ID="txt_instock" runat="server" Columns="49" MaxLength="7" /></td>
                                            </tr>
                                            <tr>
                                                <td class="label"><asp:Literal ID="ltr_onorder" runat="server" />:</td>
                                                <td><asp:TextBox ID="txt_onorder" runat="server" Columns="49" MaxLength="7" /></td>
                                            </tr>
                                            <tr>
                                                <td class="label"><asp:Literal ID="ltr_reorder" runat="server" />:</td>
                                                <td><asp:TextBox ID="txt_reorder" runat="server" Columns="49" MaxLength="7" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="divPricing" runat="server" visible="false">
                            <asp:Literal ID="ltr_pricing" runat="server" />
                        </div>
                        <div id="divAttributes" runat="server" visible="false">
                            <asp:Literal ID="ltr_attrib" runat="server" />
                        </div>
                        <div id="divMedia" runat="server" visible="false">
                            <ucEktron:Media ID="ucMedia" runat="server" />
                        </div>
                        <div id="divItems" runat="server" visible="false">
                            <ucEktron:Item ID="ucItem" runat="server" />
                        </div>
                        <div id="divMetadata" runat="server" visible="false">
                            <asp:Literal ID="ltr_meta" runat="server" />
                        </div>
                        <div id="divAlias" runat="server" visible="false">
                            <asp:Literal ID="divAliasText" runat="server" />
                            <asp:Literal ID="ltrEditAlias" runat="server" />
                        </div>
                        <div id="divSchedule" runat="server" visible="false">
                            <table>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td style="white-space:nowrap; width:5%">
                                                    <asp:Literal ID="ltr_startdate" runat="server" />:</td>
                                                <td style="white-space:nowrap; width:95%">
                                                    <asp:Literal ID="ltr_startdatesel" runat="Server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="white-space:nowrap; width:5%">
                                                    <asp:Literal ID="ltr_enddate" runat="server" />:</td>
                                                <td style="white-space:nowrap; width:95%">
                                                    <asp:Literal ID="ltr_enddatesel" runat="Server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="5"><asp:Literal ID="ltr_actionend" runat="server"/>:</td>
                                            </tr>
                                            <tr>
                                                <td colspan="5">
                                                    <asp:RadioButtonList ID="rblaction" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="divCategories" runat="server" visible="false">
                            <asp:Literal runat="server" ID="EditTaxonomyHtml" />
                            <div style="clear: both;"> </div>
                            <br />
                            <div id="wamm_float_menu_block_menunode" class="Menu" onmouseout="wamm_float_menu_block_mouseout(this)" onmouseover="wamm_float_menu_block_mouseover(this)" style="position: absolute; left: 203px; top: 311px; z-index: 3200; display: none;">
                                <input type="hidden" name="LastClickedParent" id="LastClickedParent" value="" />
                                <input type="hidden" name="ClickRootCategory" id="ClickRootCategory" value="false" />
                                <ul>
                                    <li class="MenuItem add">
                                        <a href="#" onclick="routeAction(true, 'add');">
                                                <asp:literal ID="lit_add_string" runat="server"/>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divComment" runat="server" style="display:none;">
                <table>
                    <tr>
                        <td style="white-space:nowrap; width:5%">
                            <asp:Literal ID="ltr_comment" runat="server"/>:</td>
                        <td style="white-space:nowrap; width:95%">
                            <asp:TextBox ID="txt_comment" runat="server" TextMode="MultiLine" Rows="8" Columns="50" /><br/>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td><asp:Button ID="cmdCommentOk" runat="server" /></td>
                    </tr>
                </table>
            </div>
            <div id="divTemplates" runat="server" style="display:none;">
                <div style="padding:1em;">
                    <table>
                        <tr>
                            <td style="padding-right:1em;">Template:</td>
                            <td>
                                <asp:DropDownList ID="drp_tempsel" runat="server"/>
                                <input type="hidden" name="chkLockedContentLink" id="chkLockedContentLink" value="false" />
                                <input type="button" value="Ok" onclick="$ektron('a#EkTB_closeWindowButton').click();" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="dvWaitImage"> </div>
            <asp:Literal ID="UpdateFieldJS" runat="server" />
            <div id="FrameContainer" class="ektronWindow ektronModalStandard">
                <iframe id="ChildPage">
                </iframe>
            </div>
            <asp:HiddenField ID="hdn_entrytype" runat="server" />
            <asp:HiddenField ID="hdn_publishaction" runat="server" />
            <asp:HiddenField ID="hdn_bundled" runat="server" />
            <asp:HiddenField ID="hdn_xmlid" runat="server" />
            <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
            <input type="hidden" name="LastClickedOnChecked" id="LastClickedOnChecked" value="false" />
            <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value="" runat="server" />
            <input type="hidden" name="hdn_productType" id="hdn_productType" runat="server" value="" />
            <input type="hidden" name="hdn_defaultCurrency" id="hdn_defaultCurrency" runat="server" value="" />
        </form>
    </body>
</html>
