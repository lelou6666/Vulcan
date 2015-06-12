<%@ Control Language="VB" AutoEventWireup="false" CodeFile="addsuggestedresult.ascx.vb" Inherits="Workarea_controls_search_addsuggestedresult" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../../controls/Editor/ContentDesignerWithValidator.ascx" %>
<script language="javascript" type="text/javascript"">
<!--
    var suggestedResultRecommendedMaxSize = "<%= SuggestedResultRecommendedMaxSize %>";
//-->
</script>
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
<!-- Modal Dialog: Browse CMS Content -->
<div class="ektronWindow ektronCMSContent ektronModalStandard" id="SyncStatusModal" >
    <div class="ektronModalHeader">
        <h3>
            <span class="headerText"><asp:Literal ID="lblCMSContent" runat="server" /></span>
            <asp:HyperLink ID="closeDialogLink3" CssClass="ektronModalClose" runat="server" />
        </h3>
    </div>
    <div class="ektronModalBody">
        <iframe id="ChildPage" name="ChildPage" frameborder="1" marginheight="0" marginwidth="0" width="100%" scrolling="auto"></iframe>
    </div>
</div>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
        <tr>
            <td class="label" id="termTypeLabel"><label for="termType" ><%=m_refMsg.GetMessage("type label")%></label></td>
            <td class="value">
                <span class="positionRelative">
                    <select class="selectField" id="termType" name="termType" onchange="toggleTermSuggestedResult(this)">
                        <option value="0"><%=m_refMsg.GetMessage("msg suggestedresults type option1")%></option>
                        <option value="1"><%=m_refMsg.GetMessage("msg suggestedresults type option2")%></option>
                    </select>
                </span>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="label"><label for="term" id="termLable"></label></td>
            <td class="value fixHeight">
                <div class="positionRelative">
                    <div id="termChoices" class="showElement">
                        <asp:TextBox ID="term" runat="server" onkeypress="var k = event.keyCode ? event.keyCode : event.charCode ? event.charCode : event.which; return /^([^,()])$/.test(String.fromCharCode(k) );"></asp:TextBox>
                        <select id="synonymSetTerm" name="synonymSetTerm" class="hideElement">
                            <asp:Literal ID="selectSynonymSet" runat="server" />
                        </select>
                    </div>
                </div>
                &nbsp;
            </td>
        </tr>
    </table>
    <div id="resultSizeWarning" style="display:none; visibility:hidden;"><p><%=String.Format(m_refMsg.GetMessage("msg suggested results size limit warning"), SuggestedResultRecommendedMaxSize)%></p></div>
    <div class="ektronHeader"><div id="optionsText"><%=m_refMsg.GetMessage("lbl suggested results options")%></div><%=m_refMsg.GetMessage("lbl suggested results")%></div>
    <div class="suggestedResultsItems" id="suggestedResultsItems">
        <ul class="selectedSuggestedResults" id="selectedSuggestedResults">
        </ul>
    </div>
    <asp:HiddenField ID="submitMode" runat="server" value="1" />
</div>

 <div id="add_edit_SuggestedResult" class="hideElement">
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
        <div class="ektronToolbar" id="divToolBar" runat="server"></div>
    </div>
    <div class="ektronPageContainer ektronPageInfo">
        <table class="ektronForm">
            <tr>
                <td class="label"><label for="sr_link"><%=m_refMsg.GetMessage("generic link")%>:</label><span class="caption">&nbsp</span></td>
                <td class="value">
                    <input type="text" id="sr_link" name="sr_link" class="suggestedResultLink" />
                    <input type="button" name="buttonBrowseContent" id="buttonBrowseContent" value="<%=m_refMsg.GetMessage("btn browse to cms content")%>" onclick="LoadChildPage('<%= thisContentLanguage %>')" />
                    <br />
                    <span class="caption"><%=m_refMsg.GetMessage("lbl to select external url")%></span>
                </td>
            </tr>
            <tr>
                <td class="label"><label for="sr_title"><%=m_refMsg.GetMessage("generic title")%>:</label></td>
                <td class="value"><input type="text" id="sr_title" name="sr_title" class="suggestedResultTitle" /></td>
            </tr>
            <tr>
                <td class="label"><label for="sr_summary"><%=m_refMsg.GetMessage("summary text")%>:</label></td>
                <td>
                    <div id="htmlEditorWrapper">
                        <ektron:ContentDesigner ID="HtmlEditor1" runat="server" AllowScripts="false" Height="350" Width="99%"
                            Toolbars="Minimal" ShowHtmlMode="false" />
                    </div>
                </td>
            </tr>
        </table>
        <input type="hidden" id="sr_contentID" name="sr_contentID" value="" />
        <input type="hidden" id="sr_contentLanguage" name="sr_contentLanguage" value="" />
        <input type="hidden" id="sr_ID" name="sr_ID" value="" />
     </div>
 </div>
 <div id="hiddenFormFields"></div>
<!-- Default InContext Menu -->
<div id="defaultSuggestedResultMenu" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic add title"))%></a></li>
        <li class="MenuItem edit"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic edit title"))%></a></li>
        <li class="MenuItem moveUp"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("lbl move up"))%></a></li>
        <li class="MenuItem moveDown"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("lbl move down"))%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic delete title"))%></a></li>
    </ul>
</div>
<!-- InContext Menu for when there are Zero Suggested Results -->
<div id="zeroSuggestedResultsMenu" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic add title"))%></a></li>
        <li class="MenuItem edit"><a class="menuLink disabled"><%=(m_refMsg.GetMessage("generic edit title"))%></a></li>
        <li class="MenuItem moveUp"><a class="menuLink disabled"><%=(m_refMsg.GetMessage("lbl move up"))%></a></li>
        <li class="MenuItem moveDown"><a class="menuLink disabled"><%=(m_refMsg.GetMessage("lbl move down"))%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a class="menuLink disabled"><%=(m_refMsg.GetMessage("generic delete title"))%></a></li>
    </ul>
</div>
<!-- InContext Menu for when there is a single Suggested Result -->
<div id="singleSuggestedResultMenu" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic add title"))%></a></li>
        <li class="MenuItem edit"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic edit title"))%></a></li>
        <li class="MenuItem moveUp"><a class="menuLink disabled"><%=(m_refMsg.GetMessage("lbl move up"))%></a></li>
        <li class="MenuItem moveDown"><a class="menuLink disabled"><%=(m_refMsg.GetMessage("lbl move down"))%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a class="menuLink"  onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic delete title"))%></a></li>
    </ul>
</div>
<!-- InContext Menu for when there is a Single Suggested Result -->
<div id="Div2" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic add title"))%></a></li>
        <li class="MenuItem edit"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic edit title"))%></a></li>
        <li class="MenuItem moveUp"><a class="menuLink disabled"><%=(m_refMsg.GetMessage("lbl move up"))%></a></li>
        <li class="MenuItem moveDown"><a class="menuLink disabled"><%=(m_refMsg.GetMessage("lbl move down"))%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic delete title"))%></a></li>
    </ul>
</div>
<!-- InContext Menu for the First Suggested Result -->
<div id="firstSuggestedResultMenu" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic add title"))%></a></li>
        <li class="MenuItem edit"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic edit title"))%></a></li>
        <li class="MenuItem moveUp"><a class="menuLink disabled"><%=(m_refMsg.GetMessage("lbl move up"))%></a></li>
        <li class="MenuItem moveDown"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("lbl move down"))%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic delete title"))%></a></li>
    </ul>
</div>
<!-- InContext Menu for the Last Suggested Result -->
<div id="lastSuggestedResultMenu" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic add title"))%></a></li>
        <li class="MenuItem edit"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic edit title"))%></a></li>
        <li class="MenuItem moveUp"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("lbl move up"))%></a></li>
        <li class="MenuItem moveDown"><a class="menuLink disabled"><%=(m_refMsg.GetMessage("lbl move down"))%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=(m_refMsg.GetMessage("generic delete title"))%></a></li>
    </ul>
</div>
<script language="javascript" type="text/javascript">
<!--
    // if editing an existing Suggested Result, this script tag will build the array
    <asp:Literal ID="javaScriptSRObjects" runat="server"></asp:Literal>
//-->
</script>