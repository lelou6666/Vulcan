<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Summary.ascx.cs" Inherits="Analytics_reporting_Summary" %>
<style type="text/css">
    .summarytable td
    {
        padding: 5px;
    }
    .graphcolumn
    {
        vertical-align: top;
        width: 76px;
    }
    .statcolumn
    {
        font-weight: bold;
        vertical-align: top;
        text-align: right;
    }
</style>

<div class="AnalyticSummaryReport">
    <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
        <div class="AnalyticsErrorMessage"><asp:Literal ID="litErrorMessage" runat="server" /></div>
    </asp:Panel>
                
    <table border="0" cellpadding="5" cellspacing="5" class="summarytable">
	    <tbody>
			<tr id="rowVisits" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="imgVisits" runat="server" />
			    </td>
			    <td class="statcolumn">
				    <asp:Literal ID="litVisits" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblVisits" runat="server" /> <span class="perDay"><asp:Literal ID="litVisitsPerDay" runat="server"/></span>
			    </td>
		    </tr>
			<tr id="rowPagesPerVisit" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="imgPagesPerVisit" runat="server" />
			    </td>
			    <td class="statcolumn">
				    <asp:Literal ID="litPagesPerVisit" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPagesPerVisit" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowPageviews" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="img_pageviews" runat="server" />
			    </td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_pageviews" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPageviews" runat="server" /> <span class="perDay"><asp:Literal ID="ltr_pageviewsperday" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowUniqueViews" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="img_uniqueviews" runat="server" />
			    </td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_uniqueviews" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblUniqueViews" runat="server" /> <span class="perDay"><asp:Literal ID="ltr_uniqueviewsperday" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowTimeOnSite" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="imgTimeOnSite" runat="server"/>
			    </td>
			    <td class="statcolumn">
				    <asp:Literal ID="litTimeOnSite" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblTimeOnSite" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowTimeOnPage" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="img_timeonpage" runat="server"/>
			    </td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_timeonpage" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblTimeOnPage" runat="server" />
			    </td>
		    </tr>
		    <%--<tr id="rowPercentNewVisits" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="imgPercentNewVisits" runat="server"/>
			    </td>
			    <td class="statcolumn">
				    <asp:Literal ID="litPercentNewVisits" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPercentNewVisits" runat="server" />
			    </td>
		    </tr>--%>
		    <tr id="rowBounceRate" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="img_bouncerate" runat="server"/>
			    </td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_bouncerate" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblBounceRate" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowPercentExit" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="img_percentexit" runat="server"/>
			    </td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_percentexit" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPercentExit" runat="server" />
			    </td>
		    </tr>
        </tbody>
    </table>
</div>
