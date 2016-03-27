<%@ Control Language="vb" AutoEventWireup="false" Inherits="viewfolderattributes" CodeFile="viewfolderattributes.ascx.vb" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageTabbed">
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <ul>
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
                <asp:PlaceHolder ID="phContentType" Visible="true" runat="server">
                <li>
                    <a href="#dvTypes"> <!-- Smart Forms or Product Types -->
                        <asp:Literal ID="ltrTypes" runat="server" />
                    </a>
                </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phBlogRoll" Visible="false" runat="server">
                    <li>
                        <a href="#dvBlogRoll">
                            <%=_MessageHelper.GetMessage("lbl blog roll")%>
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
                            <%=_MessageHelper.GetMessage("lbl site alias")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
            </ul>

            <div id="dvProperties">
                <table class="ektronGrid">
                    <tr>
                        <td class="label"><%=_MessageHelper.GetMessage("id label")%></td>
                        <td class="value" id="td_vf_idtxt" runat="server"></td>
                    </tr>
                    <asp:PlaceHolder ID="phBlogProperties1" Visible="false" runat="server">
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("lbl name")%>:</td>
                            <td class="value" id="td_vf_nametxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("lbl title")%>:</td>
                            <td class="value" id="td_vf_titletxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("lbl visibility")%>:</td>
                            <td class="value" id="td_vf_visibilitytxt" runat="server"></td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phFolderProperties1" Visible="false" runat="server">
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("foldername label")%></td>
                            <td class="value" id="td_vf_foldertxt" runat="server"></td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phBlogProperties2" Visible="false" runat="server">
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("lbl tag line")%>:</td>
                            <td class="value" id="td_vf_taglinetxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("lbl posts visible")%>:</td>
                            <td class="value" id="td_vf_postsvisibletxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("comments label")%>:</td>
                            <td class="value" id="td_vf_commentstxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("lbl update services")%>:</td>
                            <td class="value" id="td_vf_updateservicestxt" runat="server"></td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phDescription" Visible="true" runat="server">
                    <tr>
                        <td class="label"><%=_MessageHelper.GetMessage("lbl description")%>:</td>
                        <td class="value" id="td_vf_folderdesctxt" runat="server"></td>
                    </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td class="label"><%=_MessageHelper.GetMessage("lbl style sheet")%>:</td>
                        <td class="value" id="td_vf_stylesheettxt" runat="server"></td>
                    </tr>
                    <asp:PlaceHolder ID="phProductionDomain" Visible="false" runat="server">
                        <asp:Literal ID="DomainFolder" runat="server" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phPublishAsPdf" Visible="true" runat="server">
                    <tr>
                        <td class="label"><%=_MessageHelper.GetMessage("lbl office documents")%>:</td>
                        <td class="value" id="td_vf_pdfactivetxt" runat="server"></td>
                    </tr>
					 <tr>
                           <td></td>
                           <td><asp:Literal ID="ltrCheckPdfServiceProvider" runat="server" Text=""></asp:Literal>
                        </td>
                    </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phPreapprovalGroup" Visible="false" runat="server">
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("lbl preapproval group")%>:</td>
                            <td class="value" id="td_vf_preapprovaltxt" runat="server"></td>
                        </tr>
						
                    </asp:PlaceHolder>
                </table>
            </div>
            <div id="dvTaxonomy">
                <asp:Literal ID="taxonomy_list" runat="server" />
            </div>
            <div id="dvTemplates">
                <asp:Literal ID="template_list" runat="server" />
            </div>
            <div id="dvFlagging">
                <asp:Literal ID="flagging_options" runat="server" />
            </div>
            <div id="dvMetadata">
                <asp:Literal ID="litMetadata" runat="server" />
            </div>
            <div id="dvWebAlerts" class="ui-tabs-hide">
                <asp:Literal ID="lit_vf_subscription_properties" runat="server" />
                <asp:Literal ID="lit_vf_subscription_assignments" runat="server" />
            </div>
            <div id="dvTypes">
                <asp:Literal ID="ltr_vf_smartforms" runat="server" />
            </div>
            <div id="dvBreadcrumb">
                <asp:Panel ID="pnlInheritSitemapPath" runat="server">
                    <input type="checkbox" runat="server" name="chkInheritSitemapPath" id="chkInheritSitemapPath" checked="checked" />
                    <asp:Literal ID="ltInheritSitemapPath" runat="server" />
                    <div class="ektronTopSpace"></div>
                </asp:Panel>
                <table class="ektronGrid">
                    <tr>
                        <td class="label"><%=_MessageHelper.GetMessage("lbl path")%>:</td>
                        <td class="readOnlyValue"><span id="sitepath_preview"></span></td>
                    </tr>
                </table>
            </div>
            <div id="dvSiteAlias">
                <div id="viewSiteAliasList" runat="server"></div>
                <asp:Literal ID="ReplicationMethod" runat="server" />
            </div>
            <div id="dvSubjects">
                <asp:Literal ID="ltr_vf_categories_lbl" runat="server" />
                <asp:Literal ID="ltr_vf_categories" runat="server" />
            </div>
            <div id="dvBlogRoll">
                <asp:Label ID="lbl_vf_roll" runat="server" />
            </div>
        </div>
    </div>
</div>
<asp:Label runat="server" ID="lbl_vf_showpane" />