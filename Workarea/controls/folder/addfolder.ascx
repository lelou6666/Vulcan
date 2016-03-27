<%@ Control Language="vb" AutoEventWireup="false" Inherits="addfolder" CodeFile="addfolder.ascx.vb" %>
<script type="text/javascript">
    <!--//--><![CDATA[//><!--
        var taxonomytreearr="".split(",");
        var taxonomyparenttreearr="<%=_SelectedTaxonomyList%>".split(",");
        var __jscatrequired="0";
        var __jsparentcatrequired="<%=_CurrentCategoryChecked%>";
        var isProductCatalog = <%= _IsCatalog.ToString().ToLower() %>;
        
        Ektron.ready( function()
        {
            // ADD TEMPLATE MODAL DIALOG
            $ektron("#FrameContainer").modal(
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
        
		 function CheckPdfServiceProvider() {
             if ($ektron("#publishAsPdf").attr("checked")) {
              $ektron("#pdfGenerationMessage").show();
              }
               else {
              $ektron("#pdfGenerationMessage").hide();
              }             
            }     
			
        function LoadChildPage() {
		        var languageID;
			    var frameObj = document.getElementById("ChildPage");

			    languageID = document.getElementById("language").value
			    frameObj.src = "blankredirect.aspx?template_config.aspx?view=add&folder_edit=1";

			    var pageObj = document.getElementById("FrameContainer");
			    pageObj.className = "ChildPageShow";
	    }
	    function checkForDefaultTemplate(){
	        var defaultTemplate = $ektron('tbody#templateTable input:radio');
	        var i = 0;
            var j = 0;
            
            for(i = 0; i < defaultTemplate.length; i++)
            {
               if(defaultTemplate[i].checked){
                   j = j + 1;
               }
            }
            if( j == 0 ){
               alert('<asp:Literal runat="server" id="ltrSelectDefTemp" />');
               return false;
            }
	    }
    //--><!]]>
</script>
<!--[if lte IE 7]>
    <style type="text/css">
        div#parah input {position:relative;top:-5px;}
    </style>
<![endif]-->
 <style type="text/css">
    .selectContent { background-image: url('Images/ui/icons/check.png');background-repeat: no-repeat;background-position:.5em center; }
    .useCurrent{ background-image: url('Images/ui/icons/shape_square.png'); background-repeat: no-repeat; background-position:.5em center; }
    #FrameContainer{ width: 80%; height: 60%; margin: -150px 0 0 -380px !important;position:absolute; margin:-120px 0 0 -10px; display:none; }
    #ChildPage { frameborder:0; border:0; marginheight:2; marginwidth:2; scrolling:auto; border:1px solid #A6C9E2; width:100%; height:100%; scrolling:auto; background-color: white; }
</style>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
    <asp:Panel ID="pnlOuterContainer" CssClass="ektronPageTabbed" runat="server">
        <div class="tabContainerWrapper">
            <div class="tabContainerWrapper">
                <div class="tabContainer">
                    <ul>
                        <asp:PlaceHolder ID="phFolder" runat="server">
                            <li>
                                <a href="#dvProperties">
                                    <%=_MessageHelper.GetMessage("properties text")%>
                                </a>
                            </li>
                            <li>
                                <a href="#dvTaxonomy">
                                    <%=_MessageHelper.GetMessage("generic taxonomy lbl")%>
                                </a>
                            </li>
                            <li>
                                <a href="#dvTemplates">
                                     <%=_MessageHelper.GetMessage("lbl templates")%>
                                </a>
                            </li>
                            <li>
                                <a href="#dvFlagging">
                                    <%=_MessageHelper.GetMessage("lbl flagging")%>
                                </a>
                            </li>
                            <li>
                                <a href="#dvMetadata">
                                    <%=_MessageHelper.GetMessage("metadata text")%>
                                </a>
                            </li>
                            <asp:PlaceHolder ID="phSubjects" Visible="false" runat="server">
                                <li>
                                    <a href="#dvSubjects">
                                        <%=_MessageHelper.GetMessage("subjects text")%>
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phWebAlerts" Visible="false" runat="server">
                                <li>
                                    <a href="#dvWebAlerts">
                                        <%=_MessageHelper.GetMessage("lbl web alert tab")%>
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phTypes" Visible="false" runat="server">
                                <li>
                                    <a href="#dvTypes">
                                        <asp:Literal ID="ltrTypes" runat="server" />
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                            <li>
                                <a href="#dvBreadcrumb">
                                    <%=_MessageHelper.GetMessage("lbl sitemap path")%>
                                </a>
                            </li>
                            <asp:PlaceHolder ID="phSiteAlias" Visible="false" runat="server">
                                <li>
                                    <a href="#dvSiteAlias">
                                        <asp:Literal ID="lblSiteAlias" text="Site Alias" runat="server" />
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phBlog" Visible="false" runat="server">
                            <li>
                                <a href="#dvProperties">
                                    <%=_MessageHelper.GetMessage("properties text")%>
                                </a>
                            </li>
                            <li>
                                <a href="#dvTaxonomy">
                                    <%=_MessageHelper.GetMessage("generic taxonomy lbl")%>
                                </a>
                            </li>
                            <li>
                                <a href="#dvTemplates">
                                    <%=_MessageHelper.GetMessage("lbl templates")%>
                                </a>
                            </li>
                            <li>
                                <a href="#dvCategories">
                                    <%=_MessageHelper.GetMessage("subjects text")%>
                                </a>
                            </li>
                            <li>
                                <a href="#dvBlogRoll">
                                    <%=_MessageHelper.GetMessage("lbl blog roll")%>
                                </a>
                            </li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phDiscussionBoard" Visible="false" runat="server">
                            <li>
                                <a href="#dvProperties">
                                    <%=_MessageHelper.GetMessage("properties text")%>
                                </a>
                            </li>
                            <li>
                                <a href="#dvTemplates">
                                    <%=_MessageHelper.GetMessage("lbl templates")%>
                                </a>
                            </li>
                            <li>
                                <a href="#dvCategories">
                                    <%=_MessageHelper.GetMessage("lbl blog cat")%>
                                </a>
                            </li>
                        </asp:PlaceHolder>
                    </ul>
                    <asp:Panel ID="pnlFolder" runat="server">
                        <div id="dvProperties">
                            <table class="ektronForm" width="60%">
                                <tr>
                                    <td class="label" id="tdfoldernamelabel" runat="server"></td>
                                    <td><input type="text" maxlength="100" name="foldername" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=_MessageHelper.GetMessage("generic description")%>:</td>
                                    <td><input type="text" maxlength="256" name="folderdescription" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=_MessageHelper.GetMessage("lbl style sheet")%>:</td>
                                    <td id="tdsitepath" runat="server" ></td>
                                </tr>
                                <asp:PlaceHolder ID="phProductionDomain" Visible="false" runat="server">
								    <tr>
										<td class="label"><%=_MessageHelper.GetMessage("lbl Staging Domain")%>:</td>
                                        <td class="value" id="tdstagingdomain" runat="server"></td>
									</tr>
                                    <tr>
                                        <td class="label"><%=_MessageHelper.GetMessage("lbl Production Domain")%>:</td>
                                        <td class="value" id="tdproductiondomain" runat="server"></td>
                                    </tr>
								</asp:PlaceHolder>
                                 <asp:PlaceHolder ID="phPDF" Visible="false" runat="server">
                                    <tr>
                                        <td class="label"><%=_MessageHelper.GetMessage("lbl office documents")%>:</td>
                                        <td class="value">
                                            <input type="checkbox" id="publishAsPdf" name="publishAsPdf" onclick="CheckPdfServiceProvider()" <% =IsPublishedAsPdf() %> />
                                            <label for="publishAsPdf" id="lblPublishAsPdf" runat="server"></label>*
                                            <div class="ektronCaption"><%=_MessageHelper.GetMessage("pdf generation warning")%></div>
                                            <div class="ektronCaption">* <%=_MessageHelper.GetMessage("publish help")%></div>
                                            <div class="ektronCaption">
                                                <div id="pdfGenerationMessage" style="display:none">
                                                    <strong style="color:#C00000"><asp:Literal ID="ltrCheckPdfServiceProvider" runat="server" Text="Your PDF service provider is pdf.ektron.com. This service can be used for demonstration purposes only. To install a functioning PDF generator, contact your Ektron account manager"></asp:Literal></strong>
                                                 </div>
                                             </div>
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                                <asp:Literal ID="ReplicationMethod" runat="server" />
                            </table>
                        </div>
                        <div id="dvTaxonomy">
                            <asp:Literal ID="taxonomy_list" runat="server" />
                        </div>
                        <div id="dvTemplates">
                            <asp:Literal ID="template_list" runat="server" />
                            <asp:Literal ID="ltrTemplateFilePath" runat="server" />
                        </div>
                        <div id="dvFlagging">
                            <asp:Literal ID="inheritFlag" runat="server" />
                            <div class="ektronTopSpace"></div>
                            <table class="ektronForm">
                                <tr>
                                    <td class="label"><%=_MessageHelper.GetMessage("lbl flagging:")%></td>
                                    <td class="value"><asp:DropDownList ID="ddflags" runat="server" /></td>
                                </tr>
                            </table>
                        </div>
                        <div id="dvMetadata">
                            <asp:Literal ID="lit_vf_customfieldassingments" runat="server" />
                            <input type="hidden" name="folder_cfld_assignments" id="folder_cfld_assignments" value="" />
                        </div>
                        <div id="dvWebAlerts" class="ui-tabs-hide">
                            <asp:Literal ID="lit_vf_subscription_properties" runat="server" />
                            <asp:Literal ID="lit_vf_subscription_assignments" runat="server" />
                            <input type="hidden" name="folder_sub_assignments" />
                        </div>
                        <asp:PlaceHolder ID="phTypesPanel" Visible="false" runat="server">
                            <div id="dvTypes">
                                <asp:Literal ID="ltr_vf_types" runat="server" />
                                <input type="hidden" id="language" value="1033" />
                            </div>
                        </asp:PlaceHolder>
                        <div id="dvBreadcrumb">
                            <input type="checkbox" onclick="InheritSitemapPath(this.checked);" name="chkInheritSitemapPath"
                                id="chkInheritSitemapPath" checked="checked" />
                            <asp:Literal ID="ltInheritSitemapPath" runat="server" />
                            <div class="ektronTopSpace"></div>
                            <table class="ektronForm">
                                <tr>
                                    <td class="label"><%=_MessageHelper.GetMessage("lbl path")%>:</td>
                                    <td class="readOnlyValue"><span id="sitepath_preview"></span></td>
                                </tr>
                            </table>
                            <div class="ektronTopSpace"></div>
                            <img alt="Move selected up" onclick="moveSitemapPathNode('up')" src="images/UI/Icons/up.png" />
                            <img alt="Move selected down" onclick="moveSitemapPathNode('down')" src="images/UI/Icons/down.png" />
                            <div id="sitemap_nodes"></div>
                            <div class="ektronTopSpace"></div>
                            <div id="AddSitemapNode">
                                <table class="ektronForm">
                                    <tr>
                                        <td class="label"><%=_MessageHelper.GetMessage("generic title")%>:</td>
                                        <td class="value"><input type="text" id="sitemaptitle_input" /></td>
                                    </tr>
                                    <tr>
                                        <td class="label"><%=_MessageHelper.GetMessage("generic url link")%>:</td>
                                        <td class="value"><input type="text" id="sitemapurl_input" /><img alt="Select quicklink" onclick="PopBrowseWin('quicklinks', '', 'document.forms[0].sitemapurl_input');return false;" src="images/UI/Icons/linkAdd.png" class="ektronClickableImage" /></td>
                                    </tr>
                                    <tr>
                                        <td class="label"><%=_MessageHelper.GetMessage("generic description")%>:</td>
                                        <td class="value"><input type="text" id="sitemapdesc_input" /></td>
                                    </tr>
                                </table>
                                <div class="ektronTopSpaceSmall"></div>
                                <input type="button" title="Click here to add a new sitemap path." onclick="addSiteMapNode(this)" id="btnAddSitepath" value="Add" />
                                <input type="button" title="Reset title / url" onclick="clearSitemapForm()" value="Reset" />
                            </div>
                            <input type="hidden" id="hdnInheritSitemap" name="hdnInheritSitemap" value="" />
                            <input type="hidden" id="saved_sitemap_path" name="saved_sitemap_path" value="" />
                        </div>
                        <asp:PlaceHolder ID="phSiteAlias2" Visible="false" runat="server">
                            <div id="dvSiteAlias">
                                <table class="ektronForm">
                                    <tr>
                                        <td class="label">Name:</td>
                                        <td class="value">http:// <input type="text" id="txtAliasName" /></td>
                                    </tr>
                                </table>
                                <input type="button" title="Click here to add a new site alias." onclick="addSiteAliasName(this);" id="btnAddSiteAlias" value="Add" />
                                <input type="button" title="Reset Alias Name" onclick="clearAliasName();" value="Reset" />
                                <div class="ektronTopSpace"></div>
                                Alias List
                                <div id="divSiteAliasList"></div>
                                <input type="hidden" id="savedSiteAlias" name="savedSiteAlias" value="" />
                            </div>
                        </asp:PlaceHolder>
                    </asp:Panel>
                    <asp:Panel id="pnlBlog" Visible="false" runat="server">
                        <div id="dvProperties">
                            <table class="ektronForm">
                                <tr>
                                    <td class="label"><%=_MessageHelper.GetMessage("lbl blog name")%>:</td>
                                    <td class="value"><asp:TextBox ID="txtBlogName" runat="server" MaxLength="70" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=_MessageHelper.GetMessage("lbl blog title")%>:</td>
                                    <td class="value"><asp:TextBox ID="txtTitle" runat="server" MaxLength="75" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Label ID="lblVisibility" runat="server" CssClass="label"><%=_MessageHelper.GetMessage("lbl visibility")%>:</asp:Label></td>
                                    <td class="value">
                                        <asp:DropDownList ID="drpVisibility" runat="server">
                                            <asp:ListItem Value="0">Public</asp:ListItem>
                                            <asp:ListItem Value="1">Private</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label"><%=_MessageHelper.GetMessage("comments label")%>:</td>
                                    <td class="value">
                                        <asp:CheckBox ID="chkEnable" runat="server" Text="Enable Comments" Checked="True" onclick="javascript:UpdateBlogCheckBoxes();" />
                                        <br />
                                        <asp:CheckBox ID="chkModerate" runat="server" Text="Moderate Comments" Checked="True" />
                                        <br />
                                        <asp:CheckBox ID="chkRequire" runat="server" Text="Require Authentication" Checked="True" />
                                    </td>
                                </tr>
                                <tr id="tr_enableblogreplication" visible="false" runat="server">
                                    <td class="label"><asp:Literal ID="BlogEnableReplication" runat="server" /></td>
                                </tr>
                            </table>
                        </div>
                        <div id="dvTaxonomy">
                            <asp:Literal ID="litBlogTaxonomy" runat="server" />
                        </div>
                        <div id="dvTemplates">
                            <asp:Literal ID="litBlogTemplate" runat="server" />
                            <asp:Literal ID="litBlogTemplatedata" runat="server" />
                        </div>
                        <div id="dvCategories">
                            <asp:Literal ID="ltr_ab_cat" runat="server" />
                        </div>
                        <div id="dvBlogRoll">
                            <asp:Label ID="lbl_ab_roll" runat="server" />
                        </div>
                        <asp:HiddenField ID="hdnfolderid" runat="server" />
                    </asp:Panel>
                    <asp:Panel ID="pnlDiscussionBoard" Visible="false" runat="server">
                        <div id="dvProperties">
                            <table class="ektronForm">
                                <tr>
                                    <td class="label"><%=_MessageHelper.GetMessage("generic name")%>:</td>
                                    <td class="value"><asp:TextBox ID="txt_adb_boardname" runat="server" MaxLength="70" /><span class="required">*</span></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=_MessageHelper.GetMessage("generic title")%>:</td>
                                    <td class="value"><asp:TextBox ID="txt_adb_title" runat="server" MaxLength="75" /></td>
                                </tr>
                                <tr>
                                    <td class="label">Topics:</td>
                                    <td class="value">
                                        <asp:CheckBox ID="chk_adb_ra" runat="server" Text="Require Authentication" Checked="True" />
                                        <br />
                                        <asp:CheckBox ID="chk_adb_mc" runat="server" Text="Moderate Comments" Checked="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label">CSS Theme:</td>
                                    <td class="value"><asp:DropDownList ID="drp_theme" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=_MessageHelper.GetMessage("lbl style sheet")%>:</td>
                                    <td class="value">
                                        <asp:Literal ID="ltr_sitepath" runat="server" />
                                        <asp:TextBox ID="txt_adb_stylesheet" runat="server" />
                                    </td>
                                </tr>
                                <asp:Literal ID="ltr_dyn_repl" runat="server" />
                            </table>
                            <p class="required">* Required Field</p>
                        </div>
                        <div id="dvTemplates">
                            <asp:Literal ID="template_list_cat" runat="server" />
                            <asp:Literal ID="lit_ef_templatedata" runat="server" />
                        </div>
                        <div id="dvCategories">
                            <asp:Literal ID="ltr_adb_cat" runat="server" />
                        </div>
                        <asp:HiddenField ID="hdn_adb_folderid" runat="server" />
                    </asp:Panel>
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlDiscussionForum" CssClass="ektronPageInfo" Visible="false" runat="server">
        <table class="ektronForm">
            <tr>
                <td class="label"><%=_MessageHelper.GetMessage("lbl DiscussionForumName")%>:</td>
                <td class="value"><asp:TextBox ID="txt_adf_forumname" runat="server" MaxLength="70" /></td>
            </tr>
            <tr>
                <td class="label"><%=_MessageHelper.GetMessage("lbl DiscussionForumTitle")%>:</td>
                <td class="value"><asp:TextBox ID="txt_adf_forumtitle" runat="server" MaxLength="75" /></td>
            </tr>
            <tr>
                <td class="label"><%=_MessageHelper.GetMessage("lbl discussionforumsortorder")%>:</td>
                <td class="value"><asp:TextBox ID="txt_adf_sortorder" runat="server" CssClass="ektronTextXXXSmall" Text="1" /></td>
            </tr>
            <tr>
                <td class="label"><%=_MessageHelper.GetMessage("lbl discussionforumsubject")%>:</td>
                <td class="value"><asp:DropDownList ID="drp_adf_category" runat="server" /></td>
            </tr>
            <tr>
                <td class="label"><%=_MessageHelper.GetMessage("lbl moderate comments")%>:</td>
                <td class="value"><asp:CheckBox ID="chk_adf_moderate" runat="server" Checked="True" /></td>
            </tr>
            <tr>
                <td class="label"><%=_MessageHelper.GetMessage("lbl lock")%>:</td>
                <td class="value"><asp:CheckBox ID="chk_adf_lock" runat="server" Checked="False" /></td>
            </tr>
        </table>
        <asp:HiddenField ID="hdn_adf_folderid" runat="server" />
        <asp:Literal ID="ltr_adf_properties" runat="server" />
    </asp:Panel>
    <script type="text/javascript">
	    <!--//--><![CDATA[//><!--
	        var bexists = null;
            <asp:Literal runat="server" id="ltr_af_js" />
	    //--><!]]>
    </script>
    <input id="ParentID" type="hidden" name="ParentID" runat="server" />
    <input id="frm_callingpage" type="hidden" name="frm_callingpage" runat="server" />
    <input id="inherit_taxonomy_from" type="hidden" name="inherit_taxonomy_from" runat="server" value="0" />
    <input id="current_category_required" type="hidden" name="current_category_required" runat="server" value="0" />
    <input type="hidden" name="parent_flag" id="parent_flag" value="0" runat="server"/>
</div>