<%@ Page Language="vb" AutoEventWireup="false" %>
<%@ Import Namespace="Ektron.Cms.UI.CommonUI" %>
<%@ Import Namespace="Ektron.Cms.Site" %>
<%@ Import Namespace="Ektron.Cms" %>
<%
	Dim AppUI As New ApplicationAPI
	Dim objSite as EkSite
	objSite=AppUI.EkSiteRef
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>Status CMS</title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
	</head>
	<body>
		<%
			Response.write(objSite.CMSGetStatus())
		%>
	</body>
</html>
