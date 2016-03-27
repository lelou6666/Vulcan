<%@ Control Language="VB" AutoEventWireup="false" CodeFile="viewsuggestedresult.ascx.vb" Inherits="Workarea_controls_search_viewsuggestedresult" %>
<asp:Literal ID="PostBackPage" runat="server" />

<div style="display: none; border: 1px; background-color: white; position: absolute;
    top: 48px; width: 100%; height: 1px; margin: 0px auto;" id="dvHoldMessage">
    <table border="1px" width="100%" style="background-color: #fff;">
        <tr>
            <td valign="top" align="center" style="white-space: nowrap">
                <h3 style="color: red">
                    <strong>
                        <%=m_refMsg.GetMessage("one moment msg")%>
                    </strong>
                </h3>
            </td>
        </tr>
    </table>
</div>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
        <tr>
            <td class="label"><label for="term" id="termLable"><%=m_refMsg.GetMessage("lbl suggested result term")%></label>:</td>
            <td class="readOnlyValue"><asp:Literal ID="termName" runat="server" /></td>
        </tr>
    </table>

    <div id="overLapResults"></div>

    <div class="ektronHeader"><%=m_refMsg.GetMessage("lbl suggested results")%></div>
    <div class="suggestedResultsItems" id="suggestedResultsItems">
        <ul class="selectedSuggestedResults" id="selectedSuggestedResults">
            <asp:Literal ID="suggestedResultsOutput" runat="server" />    
        </ul>
    </div>
</div>

<asp:HiddenField ID="thisTerm" runat="server" /> 
 <script language="javascript" type="text/javascript">
<!--
    /* if viewing an existing Suggested Result associated with a single 
       term this script will fetch related suggested results */
    <asp:Literal ID="javaScriptSRObjects" runat="server"></asp:Literal> 
//-->
</script> 


