<%@ Page Language="vb" AutoEventWireup="false" Inherits="cmsformsreport" CodeFile="cmsformsreport.aspx.vb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
	    <asp:PlaceHolder runat="server" id="phHead">
		    <title id="title" runat="server" />
		    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
		    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
		    <meta name="vs_defaultClientScript" content="JavaScript"/>
		    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
		    <asp:literal id="StyleSheetJS" runat="server" />		
		    <link rel="stylesheet" type="text/css" href="csslib/ektron.workarea.css"/>
		    <link rel="stylesheet" type="text/css" href="csslib/ektron.fixedPositionToolbar.css"/>
		    <script  src="java/ektron.js" type="text/javascript"></script>
		    <script  src="java/ektron.workarea.js" type="text/javascript"></script>
		    <script  src="java/empjsfunc.js" type="text/javascript"></script>
		    <script  src="java/toolbar_roll.js" type="text/javascript"></script>
		    <script  src="java/internCalendarDisplayFuncs.js" type="text/javascript"></script>
		    <script type="text/javascript">
		    <!--//--><![CDATA[//><!--
		    var invalidFormatMsg = "<%=(m_refMsg.GetMessage("js: invalid date format error msg"))%>";		
		    var deleteFormDataID;
		    //ecmMonths = "AppUI.GetenglishMonthsAbbrev"; // Both IE and NS	
		    function submit_form(op){	
			    /*
			    Don't know why start date is required.	
			    if ((document.getElementById("start_date").value == undefined) || (document.getElementById("start_date").value == "")) {
				    alert ("You must enter the start date ");
				    return false;
			    }
			    */
    			
			    document.frmReport.flag.value="true";
			    if (typeof document.frmReport.selform != "undefined") {
				    var idx=document.frmReport.selform.selectedIndex;
				    document.frmReport.form_title.value =document.frmReport.selform.options[idx].text;
				    document.frmReport.form_id.value=document.frmReport.selform.options[idx].value;			
			    }
			    else
			    {
				    //document.frmReport.form_title.value = document.frmReport.selformTitle.value;
				    //document.frmReport.form_id.value = document.frmReport.selformId.value;
			    }
			    document.frmReport.data_type.value=document.frmReport.seltype.value;
			    document.frmReport.display_type.value=document.frmReport.seldisplay.value;
			    var selhid = 0;
			    var selhtitle = "";
			    if (typeof document.getElementById("selhid") != null && document.getElementById("selhid").value != "none")
			    {
				    document.frmReport.selhid.value = document.frmReport.selhistory.value;	
				    selhid = document.frmReport.selhid.value + "";	
				    if (typeof document.getElementById("hid_" + selhid) != null)
				    {
					    selhtitle = document.getElementById("hid_" + selhid).value;	
				    }
			    }
			    if (op=="show"){
				    if ((document.forms.frmReport.start_date.value != "") && (document.forms.frmReport.end_date.value != "")) {
					    if (!EkDTCompareDates(document.forms.frmReport.start_date, document.forms.frmReport.end_date))
					    {
						    var msg = "You cannot have the start date later than the end date.";
						    alert("<%= m_refmsg.getmessage("alert msg start date")%>");
						    return false;				
					    }
				    }
				    document.frmReport.result_type.value="show";
				    if (0 == selhid)
				    {
					    document.frmReport.action="cmsformsreport.aspx?id=" + "<%=(FormID)%>" + "&LangType=" + "<%=(ContentLanguage)%>&FormTitle=" + "<%=(DefaultFormTitle)%>" + <%if strFolderID<>"" then%>"&folder_id=" + <%=(strFolderID)%><%else%>""<%end if%>;
				    }
				    else
				    {
					    if ("" == selhtitle)
					    {
						    selhtitle = "<%=(DefaultFormTitle)%>";
					    }
					    document.frmReport.action="cmsformsreport.aspx?id=" + "<%=(FormID)%>" + "&LangType=" + "<%=(ContentLanguage)%>&FormTitle=" + selhtitle + "&hid=" + selhid + <%if strFolderID<>"" then%>"&folder_id=" + <%=(strFolderID)%><%else%>""<%end if%>;
				    }
				    document.frmReport.submit();
			    }else{
				    document.frmReport.result_type.value="export";
				    window.open("cmsformsreport.aspx?flag=true&result_type=export&data_type=" +document.frmReport.data_type.value + "&LangType=" + "<%=(ContentLanguage)%>","rptwin1","width=650,height=350,resizable,scrollbars,status,titlebar");
			    }
    			
		    }
		    function export_result(){
			    document.frmReport.result_type.value="export";
			    location.href = "exportformdata.aspx?flag=true&result_type=export&display_type=" +document.frmReport.display_type.value + "&data_type=" +document.frmReport.data_type.value+"&form_id="+document.frmReport.form_id.value+"&form_title="+document.frmReport.form_title.value+"&fieldname="+document.frmReport.fieldname.value+"&qlang="+document.frmReport.qlang.value;
			    //window.open("exportformdata.aspx?flag=true&result_type=export&display_type=" +document.frmReport.display_type.value + "&data_type=" +document.frmReport.data_type.value+"&form_id="+document.frmReport.form_id.value+"&form_title="+document.frmReport.form_title.value,"rptwin","width=650,height=350,resizable,scrollbars,status,titlebar");
			    // I know of no way to close the popup window (except to just call .close after some time).
			    // Calling .close when the Save As dialog is displayed sounds a beep.
			    // This is a common problem, as found when searching the web.
			    //    "You don't need an additional window. Just link to the page directly. 
			    //     The browser will see you're downloading a file and leave the current page in tact. ...
			    //     It shows the download dialog as a separate dialog, without affecting the page that has the link."
			    //     Imar Spaanjaars
			    //     Everyone is unique, except for me.
			    //		[http://p2p.wrox.com/topic.asp?TOPIC_ID=31831 as seen on 2005-12-05]
			    // -doug.domeny 2005-12-05
		    }
    		
		    function CheckIt(Obj) {
			    return false;
		    }
		    function SelectAll(Obj){
			    if (Obj.checked){
				    for (var i = 0; i < arFormDataId.length; i++) {
					    var objTmp = eval("document.frmReport.ektChk"+ arFormDataId[i]);
					    objTmp.checked = true;
				    }
			    }else{
				    for (var i = 0; i < arFormDataId.length; i++) {
					    var objTmp = eval("document.frmReport.ektChk"+ arFormDataId[i]);
					    objTmp.checked = false;
				    }
			    }
		    }
		    function ConfirmDelete() 
		    {
			    if ("8" == document.frmReport.seldisplay.options[document.frmReport.seldisplay.selectedIndex].value)
			    {
				    // cannot delete individual entry from "Submitted Data as XML" report type.
				    // the seldisplay.options value is "8"
				    return false;
			    }
			    var DeleteFormDataID = "";	
			    for (var i = 0; i < arFormDataId.length; i++) 
			    {	
				    var obj = eval("document.frmReport.ektChk"+ arFormDataId[i]);
				    if ("undefined" == typeof obj)
				    {
					    alert("<%= m_refmsg.getmessage("alert msg report del")%>");
					    return false;
				    }
				    else
				    {
					    if (typeof obj.checked && obj.checked) 
					    {
						    if (DeleteFormDataID == "") 
						    {
							    DeleteFormDataID = arFormDataId[i];
						    }
						    else{
							    DeleteFormDataID = DeleteFormDataID + "," + arFormDataId[i];
						    }
					    }
				    }
			    }
			    if (DeleteFormDataID == "") 
			    {
				    alert("<%= m_refmsg.getmessage("alert msg del form data")%>");
			    } 
			    else 
			    {
				    if (confirm("<%= m_refmsg.getmessage("alert msg del sel form data")%>"))
				    {								
					    document.frmReport.delete_data_id.value = DeleteFormDataID;
					    document.frmReport.action = "cmsformsreport.aspx?action=delete&id=<%=request.QueryString("id")%>" + "&LangType=" + "<%=(ContentLanguage)%>";
					    document.frmReport.submit();
				    }
			    }
			    return false;
		    }
		     //--><!]]>
		    </script>
		    <style type="text/css">
		        a.buttonGetResult 
		        {
		        	background-image: url(Images/ui/icons/chartBar.png); 
		        	background-position: .6em center; 
		        }
		        .warningError  
		        {
		        	padding: .5em 1em .5em 2.25em; 
		        	margin: .5em 0; 
		        	background-repeat: no-repeat; 
		        	background-image: url(Images/ui/icons/error.png); 
		        	background-position: .6em center; 
		        }
		        
                table.ektronReport td,th
                {
	                border: 1px solid #e7f0f7 !important;
                }
                table.ektronReport
                {
                	border-color: #e7f0f7 !important;
                }
                .lblsFormTitle
                {
                	text-align: center !important;
                	font-size: larger;
                }
		    </style>
		</asp:PlaceHolder>
	</head>
	<body>
		<form id="frmReport" name="frmReport" method="post" runat="server">
			<div class="ektronPageHeader">
                <div id="dhtmltooltip"></div>
                <div class="ektronPageHeader">
                    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
                    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
                </div>			            
			</div>
			
			<div class="ektronPageContainer ektronPageInfo">
			    <input type="hidden" name="flag" value="false"/> <input type="hidden" name="form_id" value="<%=FormId%>"/>
			    <input type="hidden" name="data_type" value="<%=DataType%>"/> <input type="hidden" name="form_title" value="<%=(DefaultFormTitle)%>"/>
			    <input type="hidden" name="result_type" value="<%=ResultType%>"/> <input type="hidden" name="display_type" value="<%=DisplayType%>"/>
			    <input type="hidden" name="delete_data_id" value=""/><input type="hidden" name="fieldname" value="<%=sPollFieldId%>"/>
			    <input type="hidden" name="qlang" value="<%=QueryLang%>"/>
			    <asp:Literal runat="server" ID="resultsMessage" />
			    <table class="ektronGrid">
				    <tr>
					    <td class="label"><asp:literal id="lblStartDate" runat="server"/></td>
					    <td><asp:literal id="dtStart" runat="server"/></td>
					</tr>
					<tr>
					    <td class="label"><asp:literal id="lblEndDate" runat="server"/></td>
					    <td><asp:literal id="dtEnd" runat="server"/></td>
				    </tr>
				    <tr>
					    <td class="label"><%= m_refmsg.getmessage("lbl report display")%>:</td>
					    <td colspan="3">
						    <asp:Literal id="SelectFormReport" runat="server" />
						    <!--
						    <select id="seldisplay" name="seldisplay">
							    <option value="vertical" <% if (DisplayType = "vertical") then%>selected<% end if %>>Vertical</option>
							    <option value="horizontal" <% if (DisplayType = "horizontal") then%> selected<%end if %>>Horizontal</option>
						    </select>
						    -->
					    </td>
				    </tr>				    
				    <asp:Literal id="SelectHistoryReport" runat="server" />				   
			    </table>
			    <input type="hidden" id="seltype" name="seltype" value="All" />			    
                <div class="ektronTopSpace">
			        <ul class="buttonWrapperLeft ui-helper-clearfix">
			            <li>
			                <a class="button buttonLeft greenHover buttonGetResult" type="button" id="btnShow" name="btnShow" value="<%= m_refmsg.getmessage("btn get result")%>" onclick="javascript:submit_form('show');" ><asp:Literal runat="server" ID="litGetResult" /></a>
			            </li>
			        </ul>
			    </div>
			    <div class="ektronTopSpace" style="clear:both;">
                    <asp:Literal ID="FormResult" Runat="server"/>
                </div>

                <div class="ektronTopSpace">
			    <asp:LinkButton class="button buttonLeft greenHover buttonExport" ID="BtnExport" runat="server" Text="Export Report" />
			    <asp:Literal ID="ExportResult" runat="server"/>
			    </div>
			</div>
		</form>
		<script type="text/javascript">
		<!--//--><![CDATA[//><!--
		var arFormDataId = new Array(<%=(sFormDataIds)%>);
		//--><!]]>
		</script>
	</body>
</html>
