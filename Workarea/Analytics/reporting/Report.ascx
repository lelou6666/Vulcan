<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Report.ascx.cs" Inherits="Analytics_reporting_Report" %>
<%@ Register TagPrefix="ektron" TagName="PercentPieChart" Src="../../controls/reports/PercentPieChart.ascx" %> 
<%@ Register TagPrefix="analytics" TagName="Detail" Src="Detail.ascx" %>

<asp:Label ID="lblNoRecords" Visible="false" runat="server"><asp:literal ID="ltrlNoRecords" runat="server" /></asp:Label>
    
<asp:Panel ID="pnlData" runat="server">  
    <div class="ektronPageGrid analyticsReport">
        <div class="InnerRegion" >
            <asp:UpdatePanel ID="UpdatePanel1" ChildrenAsTriggers="true" UpdateMode="Conditional" runat="server">
               <ContentTemplate>
                    <asp:Literal ID="litCssTweaks" runat="server" />
                    <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
                        <div class="AnalyticsErrorMessage"><asp:Literal ID="litErrorMessage" runat="server" /></div>
                    </asp:Panel>
                    <div class="AnalyticsReportBreadcrumb">
						<a ID="lnkBreadcrumbReport" runat="server" onserverclick="HtmlLink_OnServerClick" EnableViewState="true" visible="false"></a>
						
						<span id="htmBreadcrumbSeparatorFor" class="AnalyticsReportBreadcrumbSeparator" runat="server" enableviewstate="false" visible="false">&#187;</span>
						<asp:Label ID="lblBreadcrumbFor" class="AnalyticsReportBreadcrumb" runat="server" EnableViewState="false" visible="false" />
						<a ID="lnkBreadcrumbFor" runat="server" onserverclick="HtmlLink_OnServerClick" EnableViewState="true" visible="false"></a>
						
						<span id="htmBreadcrumbSeparatorAnd" class="AnalyticsReportBreadcrumbSeparator" runat="server" enableviewstate="false" visible="false">&#187;</span>
						<asp:Label ID="lblBreadcrumbAnd" class="AnalyticsReportBreadcrumb" runat="server" EnableViewState="false" visible="false" />
						<a ID="lnkBreadcrumbAnd" runat="server" onserverclick="HtmlLink_OnServerClick" EnableViewState="true" visible="false"></a>
						
						<span id="htmBreadcrumbSeparatorAlso" class="AnalyticsReportBreadcrumbSeparator" runat="server" enableviewstate="false" visible="false">&#187;</span>
						<asp:Label ID="lblBreadcrumbAlso" class="AnalyticsReportBreadcrumb" runat="server" EnableViewState="false" visible="false" />
                    </div>
                    <h3 class="AnalyticsReportTitle" ID="htmReportTitle" runat="server" enableviewstate="false"></h3>
                    <h4 id="AnalyticsReportDateRangeDisplay" runat="server" class="AnalyticsReportDateRangeDisplay" visible="false" enableviewstate="false" ></h4>
                    <h4 class="AnalyticsReportSubtitle" ID="htmReportSubtitle" runat="server" visible="false" enableviewstate="false"></h4>
                    <p class="AnalyticsReportSummary" ID="htmReportSummary" runat="server" enableviewstate="false"></p>
                
                    <asp:GridView ID="gvDataTable" runat="server" AllowSorting="true" AllowPaging="true"
						ShowHeader="true" UseAccessibleHeader="true" OnPageIndexChanging="GvDataTable_OnPageIndexChanging"
                         PageSize="15" OnRowDataBound="GvDataTable_RowDataBound" OnSorting="GvDataTable_OnSorting"
                         OnRowCreated="GvDataTable_RowCreated" AutoGenerateColumns="false" Width="100%">
                        <AlternatingRowStyle CssClass="alternatingrowstyle" />
                        <HeaderStyle CssClass="headerstyle" />
                        <PagerStyle CssClass="pagerstyle" />
                        <PagerTemplate>
                            
                            <asp:Label id="lblGoTo" runat="server" Text="Go to" /> 
                            <asp:TextBox ID="txtGoToPage" runat="server" AutoPostBack="true" OnTextChanged="GoToPage_TextChanged" CssClass="gotopage" />
                            &#160;
                            <asp:Label ID="lblTotalNumberOfPages" runat="server" />
                            &#160;
                            <asp:Button ID="Button1" runat="server" CommandName="Page" ToolTip="Previous Page" CommandArgument="Prev" CssClass="previous" />
                            <asp:Button ID="Button2" runat="server" CommandName="Page" ToolTip="Next Page" CommandArgument="Next" CssClass="next" />
                            &#160;
                            <asp:Label ID="lblShowRows" runat="server" Text="Show rows:" />
                            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="GvDataTable_SelectedIndexChanged">
                                <asp:ListItem Value="5" />
                                <asp:ListItem Value="10" />
                                <asp:ListItem Value="15" />
                                <asp:ListItem Value="20" />
                            </asp:DropDownList>
                        </PagerTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="NameColumn" ItemStyle-CssClass="NameColumn" SortExpression="Name">
                                <ItemTemplate>
									<asp:Image ID="imgColorBox" CssClass="ReportName" Visible="false" runat="server" EnableViewState="false" />
									<asp:HyperLink ID="lnkGo" CssClass="ReportName" Target="_blank" Visible="false" runat="server" EnableViewState="false"></asp:HyperLink>
									<a ID="lnkDrillDown" CssClass="DrillDown" runat="server" onserverclick="HtmlLink_OnServerClick" EnableViewState="true"><asp:Label ID="lblNameValueLinked" CssClass="ReportName" runat="server" EnableViewState="false" /></a>
									<asp:Label ID="lblNameValue" CssClass="ReportName" runat="server" EnableViewState="false" Visible="false" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn" SortExpression="Visits">
                                <ItemTemplate>
                                    <%# Eval("Visits", "{0:#,##0}") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn">
                                <ItemTemplate>
                                    <%# GetPercentVisits(Eval("Visits"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn">
                                <ItemTemplate>
                                    <%# Eval("PagesPerVisit", "{0:0.00}") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="TimeColumn" ItemStyle-CssClass="TimeColumn">
                                <ItemTemplate>
                                    <%# Eval("AverageTimeSpanOnSite") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn">
                                <ItemTemplate>
                                    <%# Eval("PercentNewVisits", "{0:0.00%}") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn" SortExpression="Entrances">
                                <ItemTemplate>
                                    <%# Eval("Entrances", "{0:#,##0}")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn" SortExpression="Exits">
                                <ItemTemplate>
                                    <%# Eval("Exits", "{0:#,##0}")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn" SortExpression="PageViews">
                                <ItemTemplate>
                                    <%# Eval("PageViews", "{0:#,##0}") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn" SortExpression="PageViews">
                                <ItemTemplate>
                                    <%# GetPercentPageviews(Eval("PageViews"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn" SortExpression="UniqueViews">
                                <ItemTemplate>
                                    <%# Eval("UniqueViews", "{0:#,##0}") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="TimeColumn" ItemStyle-CssClass="TimeColumn">
                                <ItemTemplate>
                                    <%# Eval("AverageTimeSpanOnPage") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn" SortExpression="Bounces">
                                <ItemTemplate>
                                    <%# Eval("Bounces", "{0:#,##0}")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn">
                                <ItemTemplate>
                                    <%# Eval("BounceRate", "{0:0.00%}") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="NumericColumn" ItemStyle-CssClass="NumericColumn">
                                <ItemTemplate>
                                    <%# Eval("ExitRate", "{0:0.00%}") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                
                    <analytics:Detail ID="AnalyticsDetail" OnSelectionChanged="AnalyticsDetail_SelectionChanged" runat="server" Visible="false" />

					<ektron:PercentPieChart ID="AnalyticsPieChart" Width="250px" Height="250px" Legend="BottomVertical" runat="server" Visible="false" />
					 
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    
</asp:Panel>
