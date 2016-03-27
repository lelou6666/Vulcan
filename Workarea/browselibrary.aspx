<%@ Page Language="vb" %>
<%@ Import Namespace="Ektron.Cms.UI.CommonUI" %>
<%@ Import Namespace="Ektron.Cms.Site" %>
<%@ Import Namespace="Ektron.Cms.User" %>
<%@ Import Namespace="Ektron.Cms.Content" %>
<%@ Import Namespace="Ektron.Cms" %>
<script language=vb runat=server >
Dim gtMsgObj, gtMess, locFolderID, lScope, actionType, autonav, CurrentUserId, ErrorString As Object
Dim AppUI As New ApplicationAPI
Dim AppPath, AppImgPath, sitePath as String
dim QueryType as string = ""
dim disableLinkManage as string = ""
Dim ContentLanguage,EnableMultilingual, RetField As Object
dim MsgHelper as Common.EkMessageHelper=nothing
Const ALL_CONTENT_LANGUAGES As integer=-1
Const CONTENT_LANGUAGES_UNDEFINED  As integer=0
</script>

<%
MsgHelper=AppUI.EkMsgRef
CurrentUserId=AppUI.UserId
AppImgPath=AppUI.AppImgPath
AppPath=AppUI.AppPath
sitePath=AppUI.SitePath


	locFolderID = Request.QueryString("defaultFolderId")
	lScope = Request.QueryString("scope")
	actionType = Request.QueryString("actiontype")
	autonav = Request.QueryString("autonav")
	if (autonav <> "") then
		autonav = "&autonav=" & autonav
	end if
	RetField = Request.QueryString("RetField")
	if (RetField <> "") then
		RetField = "&RetField=" & RetField
	end if
	if ((Request.QueryString("type") isnot nothing) andalso (Request.QueryString("type") <> "")) then
	    QueryType = "&type=" & Request.QueryString("type")
	end if	
	
	If ((Request.QueryString("disableLinkManage") IsNot Nothing) AndAlso (Request.QueryString("disableLinkManage") <> "")) Then
        disableLinkManage = "&disableLinkManage=" & Request.QueryString("disableLinkManage")
    end if

    Dim showQDContentOnly As Boolean = (Request.QueryString("qdo") = "1")
    Dim enableQDOparam As String = ""
    If (showQDContentOnly) Then
        enableQDOparam = "&qdo=1"
    End If
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
	<title><%= (AppUI.AppName & " " & MsgHelper.GetMessage("library page html title") & " " & (Ektron.cms.commonApi.GetEcmCookie()("username")))%></title>
	<!--#include file="common/stylesheetsetup.inc" -->
</head>
<script type="text/javascript">
    <!--//--><![CDATA[//><!--
	var inserter = false;
	
	function SetLoadStatus(Page) {
		if (Page.toLowerCase() == "inserter") {
			inserter = true;
		}
	}
	
	function GetLoadStatus() {
		if (inserter == true) {
			return true;
		}
		return false;
	}
	//--><!]]>

</script>
	<frameset cols="300,*" border="5" class="library">	
			<frame name="medialist" src="librarylist.aspx?actionType=<%=actionType%>&scope=<%= Request.QueryString("scope")%><%=(autonav)%><%=(RetField)%><%=QueryType %><%=disableLinkManage %><%=enableQDOparam%>" marginwidth="2" marginheight="2" scrolling="auto" frameborder="1">		
			<frame name="libraryinsert" src="libraryinsert.aspx?actionType=<%=actionType%>&scope=<%= Request.QueryString("scope")%><%=(autonav)%><%=(RetField)%><%=QueryType %><%=disableLinkManage %><%=enableQDOparam%>" marginwidth="2" marginheight="2" scrolling="auto" frameborder="1">		
	</frameset>
</html>