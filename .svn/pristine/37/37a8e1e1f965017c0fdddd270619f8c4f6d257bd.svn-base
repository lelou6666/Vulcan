<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SiteSearchControl.ascx.cs" Inherits="Foodservice_usercontrols_SiteSearchControl" %>
<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>
<asp:ScriptManager runat="server">
</asp:ScriptManager>

<script type="text/javascript">
    $(document).ready(function () {
        // basic search link
        $("a#basicTermsanchor").click(function () {
            ShowBasicSearch();
        });

        // adv. search link
        $("a#advancedTermsanchor").click(function () {
            ShowAdvTab();
        });

        // search buttons
        $(".lsButtons").click(function () {
            $("#<%=divLoadinglabel.ClientID%>").show();
            $("#<%=searchresults.ClientID%>").hide();
            $("#<%=divnoresults.ClientID%>").hide();
            $("#<%=resultsheader.ClientID%>").hide();
        });

        // add filter buttons
        $("#<%=btnAddFilter.ClientID%>").click(function () {
            var firstli = $("#li_Meta_00").html();
            firstli = "<li id='li_Meta_00'>" + firstli + "</li>";
            var key = "";
            for (var i = 1; i <= 100; i++) {
                key = i.toString();
                if (key.length == 1) key = "0" + key;
                var li = $("#li_Meta_" + key);
                if (li.length == 0) break;
            }
            key = "_" + key;
            while (firstli.indexOf("_00") != -1) {
                firstli = firstli.replace("_00", key);
            }
            $(firstli).insertAfter("#parentForFilters li:last");
        });

        // show current ab
        ShowTabs();
    });

    // remove filter, making sure one stays
    function RemoveFilter(source) {

        var licount = ($("#parentForFilters li").length);
        if (licount > 2) {
            $(source).parent().remove();
        }
        else {
            var parent = $(source).parent();
            $(parent).children("input").each(function () {
                $(this).val("");
            });
        }
        return false;
    }

    function AdvOptionDefault(source) {
        var val = ($(source).val());
        if (val.indexOf("created") != -1 || val.indexOf("modified") != -1) {
            $(source).parent().children("input").each(function () {
                if ($(this).val() == "") {
                    $(this).val("YYYY/MM/DD");
                }
            });
        }
    }

    function ShowTabs() 
    {
        var tab = $("#<%=UxSearchTab.ClientID%>").val();
        if (tab == "1") {
            ShowBasicSearch();
        }
        else {
            ShowAdvTab();
        }
    }

    function ShowAdvTab() {
        $("div#basicTerms").hide();
        $("div#advancedTerms").show();

        $("a#basicTermsanchor").css("background-color", "#d6def3");
        $("a#advancedTermsanchor").css("background-color", "white");

        $("#<%=searchresults.ClientID%>").hide();
        $("#<%=resultsheader.ClientID%>").hide();
        $("#<%=UxSearchTab.ClientID%>").val("2");
        $("#<%=divnoresults.ClientID%>").hide();
    }
    function ShowBasicSearch() 
    {
        $("div#basicTerms").show();
        $("div#advancedTerms").hide();

        $("a#basicTermsanchor").css("background-color", "white");
        $("a#advancedTermsanchor").css("background-color", "#d6def3");

        $("#<%=UxSearchTab.ClientID%>").val("1");
    }
</script>

<input type="hidden" runat="server" id="UxSearchTab" />
<div class="ektronSearch">
    <!--
    <ul class="searchNavigation" id="uxTabs" runat="server">
        <li>
            <a id="basicTermsanchor" href="#">
                <asp:literal id="litBasicTab" runat="server" EnableViewState="false"></asp:literal>
            </a>
        </li>
       
        <li id="liAdvLink" runat="server">
            <a id="advancedTermsanchor" href="#">
                <asp:literal id="litAdvTab" runat="server" EnableViewState="false"></asp:literal>
            </a>
        </li>
    </ul> -->
    
    <div class="searchWrapper">
        <div style="display: block" id="basicTerms">
            <!--<input id="ecmBasicKeywords" class="basicSearchTerms" maxlength="50" size="30" type="text" name="ecmBasicKeywords" runat="server" EnableViewState="false"/>  -->
          <!--  <asp:DropDownlist id="ddlSearchScope" runat="server" EnableViewState="false">
                    <asp:listItem Value="all"></asp:listItem>
                    <asp:listItem Value="html"></asp:listItem>
                    <asp:listItem Value="documents"></asp:listItem>
                    <asp:listItem Value="images"></asp:listItem>
                    <asp:listItem Value="multiMedia"></asp:listItem>
                    <asp:listItem Value="discussionForum"></asp:listItem>
                    <asp:listItem Value="tags"></asp:listItem>
                    <asp:listItem Value="pageBuilder"></asp:listItem>
            </asp:DropDownlist>
-->
   
<!--<asp:ImageButton id="btnSearchBasic" CssClass="lsButtons" runat="server" 
onclick="btnSearchBasic_click" EnableViewState="false"  ImageUrl="/images/search_btn.png" ></asp:ImageButton> -->

            <div id="divLoadinglabel" runat="server" style="display:none"></div>
        </div>
        <div style="display: none" id="advancedTerms">
            <fieldset class="findResults">
                <legend id="legFindBy" runat="server"></legend>
                <ul>
                    <li>
                        <span>
                            <label for="<%=this.ecm_q.ClientID%>"><asp:literal id="litwithallthewords" runat="server"></asp:literal></label>
                        </span> 
                        <input id="ecm_q" class="stext" type="text" name="ecm_q" runat="server"/>
                    </li>
                    <li>
                        <span>
                            <label for="<%=this.ecm_epq.ClientID%>"><asp:literal id="litadvsearchexactphrase" runat="server" EnableViewState="false"></asp:literal></label>
                        </span> 
                        <input id="ecm_epq" class="stext" type="text" name="ecm_epq" runat="server"/> 
                    </li>
                    <li>
                        <span>
                            <label for="<%=this.ecm_oq.ClientID%>"><asp:literal id="litatleastoneofthewords" runat="server" EnableViewState="false"></asp:literal></label>
                        </span> 
                        <input id="ecm_oq" class="stext" type="text" name="ecm_oq" runat="server"/> 
                    </li>
                    <li>
                        <span>
                            <label for="<%=this.ecm_eq.ClientID%>"><asp:literal id="litwithouttheword" runat="server" EnableViewState="false"></asp:literal></label>
                        </span> 
                        <input id="ecm_eq" class="stext" type="text" name="ecm_eq" runat="server"/> 
                    </li>
                </ul>
            </fieldset> 
            <fieldset class="searchFilters">
                <legend id="legFilterResults" runat="server"></legend>
                <ul id="parentForFilters">
                    <asp:Literal ID="litAdvOptions" runat="server" EnableViewState="false"></asp:Literal>
                </ul>
                <p>
                    <input id="btnAddFilter" class="addFilter" type="button" value="" runat="server" />
                </p>
            </fieldset> 
            <asp:Button id="btnSearchAdvanced" CssClass="lsButtons" runat="server" EnableViewState="false" OnClick="btnSearchAdvanced_Click"  ImageUrl="/images/search_btn.png" ></asp:Button>
        </div>
        <div class="searchResultsHeader" id="resultsheader" runat="server" enableviewstate="false">
            <h3 id="h3NoResults" runat="server" EnableViewState="false" class="noResults" style="font-family: 'futura light condensed', Arial, Sans-Serif; color: #b0b0b0; font-size: 26px; margin-bottom:10px; border-bottom: 1px solid #b0b0b0;"></h3>
        </div>
        <div id="divnoresults" class="resultpreview" runat="server" EnableViewState="false"></div>
        <asp:UpdatePanel id="uxUpdatePanel" runat="server" EnableViewState="false" UpdateMode="Conditional">
        
            <ContentTemplate>
            <div id="searchresults" class="resultpreview" runat="server">
            <asp:ListView ID="UXListView" runat="server">
                <LayoutTemplate>
                    <div runat="server" id="itemPlaceholder"></div>
                </LayoutTemplate>

                <ItemTemplate>
                        <p>
                            <a href="<%#GetAlias(((Ektron.Cms.WebSearch.SearchData.SearchResponseData)(Container.DataItem)).ContentID) %>">
                            <%#((Ektron.Cms.WebSearch.SearchData.SearchResponseData)(Container.DataItem)).Title%>
                            </a>
                        </p>
                </ItemTemplate>
            </asp:ListView>
            
            <div id="navbar">
            <p>
                <asp:DataPager ID="DataPager1" runat="server"  PagedControlID="UXListView">
                        <Fields>
                            <asp:TemplatePagerField>
                <PagerTemplate>
                        &nbsp;
                </PagerTemplate>
                </asp:TemplatePagerField>

                <asp:NumericPagerField
                    PreviousPageText="..."
                    NextPageText="..."
                    ButtonCount="5"
                    ButtonType="Link"
                    NextPreviousButtonCssClass="inactive"
                    NumericButtonCssClass="inactive"
                    CurrentPageLabelCssClass="active" />
                            
                <asp:TemplatePagerField>
                <PagerTemplate>
                        &nbsp;
                </PagerTemplate>
                </asp:TemplatePagerField>                      
                                 
                        </Fields>
                    </asp:DataPager>
            </p>
            </div>
            
            <br /><br />
            
            <div class="searchResultsHeader" id="resultsheader2" runat="server" enableviewstate="false">
                <h3 id="h3NoResults2" runat="server" EnableViewState="false" class="noResults" style="font-family: 'futura light condensed', Arial, Sans-Serif; color: #b0b0b0; font-size: 26px; margin-bottom:10px; border-bottom: 1px solid #b0b0b0;"></h3>
            </div>
            
            <asp:ListView ID="UXListView2" runat="server">
                <LayoutTemplate>
                    <div runat="server" id="itemPlaceholder">
                    </div>
                </LayoutTemplate>

                <ItemTemplate>
                        <p><a href="<%#GetAlias(((Ektron.Cms.WebSearch.SearchData.SearchResponseData)(Container.DataItem)).ContentID) %>">
                            <%#((Ektron.Cms.WebSearch.SearchData.SearchResponseData)(Container.DataItem)).Title%>
                            </a>
                        </p>
                        <% if (this.ShowCustomSummary) { %>
                        <div class="resultPreview">
                            <p><%#((Ektron.Cms.WebSearch.SearchData.SearchResponseData)(Container.DataItem)).Summary %></p>
                        </div>
                        <% } %>
                </ItemTemplate>
            </asp:ListView>
            
            <div id="navbar2">
            <p>
                <asp:DataPager ID="DataPager2" runat="server"  PagedControlID="UXListView2">
                        <Fields>
                            <asp:TemplatePagerField>
                <PagerTemplate>
                        &nbsp;
                </PagerTemplate>
                </asp:TemplatePagerField>

                <asp:NumericPagerField
                    PreviousPageText="..."
                    NextPageText="..."
                    ButtonCount="5"
                    ButtonType="Link"
                    NextPreviousButtonCssClass="inactive"
                    NumericButtonCssClass="inactive"
                    CurrentPageLabelCssClass="active" />
                            
                <asp:TemplatePagerField>
                <PagerTemplate>
                        &nbsp;
                </PagerTemplate>
                </asp:TemplatePagerField>                      
                                 
                        </Fields>
                    </asp:DataPager>
            </p>
            </div>
            </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>


                    
   
