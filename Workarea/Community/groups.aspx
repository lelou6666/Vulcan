<%@ Page Language="VB" AutoEventWireup="false" CodeFile="groups.aspx.vb" Inherits="Community_groups" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Groups</title>
        <script type="text/javascript" src="../java/jfunct.js"></script>
        <script type="text/javascript" src="../java/toolbar_roll.js"></script>
        <script type="text/javascript">
            function ClearErr(){
	            document.getElementById ('errmsg').innerHTML='';
	        }
        </script>
        <script type="text/javascript">
            Ektron.ready( function()
                {
                    var tabsContainers = $ektron(".tabContainer");
                    tabsContainers.tabs();
                }
            );
        </script>
    </head>
    <body id="body" runat="server">
        <form id="frmContent" runat="server">
            <div class="ektronPageContainer">
            <asp:Panel ID="panel1" CssClass="ektronPageGrid" runat="server" Visible="false">
                <asp:Literal ID="ltr_allgroups" runat="Server" />
                <asp:GridView ID="CommunityGroupList"
                    runat="server"
                    AutoGenerateColumns="False"
                    Width="100%"
                    EnableViewState="False"
                    CssClass="ektronGrid"
                    GridLines="None">
                    <HeaderStyle CssClass="title-header" />
                </asp:GridView>
                <p class="pageLinks">
                    <asp:Label runat="server" ID="TPageLabel">Page</asp:Label>
                    <asp:Label ID="TCurrentPage" CssClass="pageLinks" runat="server" />
                    <asp:Label runat="server" ID="TOfLabel">of</asp:Label>
                    <asp:Label ID="TTotalPages" CssClass="pageLinks" runat="server" />
                </p>
                <asp:LinkButton runat="server" CssClass="pageLinks" ID="TFirstPage" Text="[First Page]"
                    OnCommand="TNavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
                <asp:LinkButton runat="server" CssClass="pageLinks" ID="TPreviousPage" Text="[Previous Page]"
                    OnCommand="TNavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
                <asp:LinkButton runat="server" CssClass="pageLinks" ID="TNextPage" Text="[Next Page]"
                    OnCommand="TNavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
                <asp:LinkButton runat="server" CssClass="pageLinks" ID="TLastPage" Text="[Last Page]"
                    OnCommand="TNavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
                <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
            </asp:Panel>
            <asp:Panel CssClass="ektronPageTabbed" ID="panel3" runat="server" Visible="false">
                <div class="tabContainerWrapper">
                    <div class="tabContainer">
                        <ul>
                            <li>
                                <a href="#dvProperties">
                                    Properties
                                </a>
                            </li>
                            <li>
                                <a href="#dvTags">
                                    Tags
                                </a>
                            </li>
                            <asp:PlaceHolder ID="phCategoryTab" runat="server">
                                <li>
                                    <a href="#dvCategory">
                                        Category
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                             <asp:PlaceHolder ID="phAliasTab" runat="server">
                                <li>
                                 <a href="#dvAlias">
                                    Links
                                 </a>
                                </li>
                             </asp:PlaceHolder>
                        </ul>

                        <div id="dvProperties">
                            <span id="errmsg" runat="server" />
                            <table class="ektronForm">
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_groupname" runat="server" />:</td>
                                    <td class="value"><asp:TextBox ID="GroupName_TB" runat="server" /></td>
                                </tr>
                                <tr runat="server" id="tr_ID">
                                    <td class="label"><asp:Literal ID="ltr_groupid" runat="server" />:</td>
                                    <td class="value"><asp:Label ID="lbl_id" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_admin" runat="server" />:</td>
                                    <td class="value"><asp:Literal ID="ltr_admin_name" runat="Server" />&nbsp;&nbsp;<asp:Button ID="cmd_browse" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_groupjoin" runat="server" />:</td>
                                    <td class="value">
                                        <asp:RadioButton ID="PublicJoinYes_RB" runat="server" GroupName="PublicJoin" Text="Yes" />&nbsp;&nbsp;
                                        <asp:RadioButton ID="PublicJoinNo_RB" runat="server" GroupName="PublicJoin" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="white-space: nowrap;">
                                        <asp:Literal ID="ltr_groupfeatures" runat="server"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="FeaturesCalendar_CB" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:CheckBox ID="FeaturesForum_CB" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_groupavatar" runat="server" />:</td>
                                    <td class="value"><asp:Literal ID="ltr_avatarpath" runat="server" /><asp:TextBox ID="GroupAvatar_TB" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_grouplocation" runat="server" />:</td>
                                    <td class="value"><asp:TextBox ID="Location_TB" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_groupsdesc" runat="server" />:</td>
                                    <td class="value"><asp:TextBox ID="ShortDescription_TB" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_groupdesc" runat="server" />:</td>
                                    <td class="value"><asp:TextBox ID="Description_TB" runat="server" Rows="6" TextMode="MultiLine" /></td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td>
                                        <div runat="server" id="tr_EnableDistribute">
                                            <asp:CheckBox ID="EnableDistributeToSite_CB" runat="server" /><asp:Literal ID="ltr_enabledistribute" runat="server" />
                                        </div>
                                        <div runat="server" id="tr_AllowMembersToManageFolders">
                                            <asp:CheckBox ID="AllowMembersToManageFolders_CB" runat="server" /><asp:Literal ID="ltr_AllowMembersToManageFolders" runat="server" />
                                        </div>
                                    </td>
                                </tr>

                            </table>
                        </div>
                        <div id="dvTags">
                            <div id="TD_personalTags" runat="server"></div>
                        </div>
                        <div id="dvCategory">
                            <div id="TD1" runat="server"><asp:Literal ID="ltr_cat" runat="server" /></div>
                        </div>
                        <asp:PlaceHolder ID="phAliasFrame" runat="server">
                           <div id="dvAlias">
                                <p style="width: auto; height: auto; overflow: auto;" class="groupAliasList" ><%=groupAliasList%></p>
                            </div>
                       </asp:PlaceHolder> 
                    </div>
                </div>
            </asp:Panel>
            <div>
                <input type="hidden" id="hdn_search" name="hdn_search" value="" />
                <asp:Literal ID="ltr_js" runat="server" />
            </div>
            </div>
        </form>
    </body>
</html>
