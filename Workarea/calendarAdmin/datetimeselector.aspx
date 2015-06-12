<%@ Page Language="vb" AutoEventWireup="false" Inherits="DateTimeSelector" CodeFile="DateTimeSelector.aspx.vb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
	<title>Date Time Selector </title>
	<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<script type="text/javascript" language="JavaScript">

			<asp:Literal id="JSGlobals" runat="server"></asp:Literal>


			function pageInit() {
				if(typeof(initTargetDate)!='undefined') {
					initTargetDate() ;
				}
			}
		</script>
	</head>
	<body onload="pageInit();">
		<p><asp:Literal id="moDisplay" runat="server"></asp:Literal></p>
	</body>
</html>
