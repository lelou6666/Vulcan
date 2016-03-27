<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<!-- Copyright 2001 Ektron, Inc. -->
<!-- Revision Date: 2001-08-22 -->
<html>
<head>
<title>eWebEditPro with WebImageFX Installation</title>
<!-- #include file="../../setup.asp" -->
<%
Dim licitem, cObj, ErrString, SiteInfo
if (request.cookies("ecm").HasKeys) then
	currentUserID = request.cookies("ecm")("user_id")
	Site = request.cookies("ecm")("site_id")
else
	currentUserID = 0
	Site = ""
end if
Set cObj = Server.CreateObject(SITE_OBJ)
set SiteInfo = cObj.GetSiteVariablesv2_0(AppConfStr, currentUserID, Site, ErrString) 
licitem = SiteInfo("LicKey")
Set cObj = nothing
Dim gtMsgObj, gtMess
Set gtMsgObj = Server.CreateObject(MSG_OBJ)
Set gtMess = gtMsgObj.GetMsgsByTitleTwo(AppConfStr,"generic page error message", Request.Cookies("ecm")("user_id"),ErrorString)
%>
<script language="JavaScript1.2">
// These are the values extracted
var given_eWebEditProPath = "<%= AppeWebPath %>";
var given_WebImageFXPath = "<%= AppeWebPath %>";
var given_LicenseKeys = "<%=licitem%>";  // These make the intro pages happy.
var given_WifxLicenseKeys = "<%=licitem%>";

var WIFXPath = "<%= AppeWebPath %>";
var eWebEditProMsgsFilename = "ewebeditpromessages" + "<%=gtMess("BrowserCode")%>" + ".js";
//var eWebEditProinstallPopupUrl ="<%=(AppeWebPath)%>clientinstall/intro" + "<%=gtMess("BrowserCode")%>.htm";
</script>
<script language="JavaScript1.2" src="cms_parseinstallparams.js"></script>
<script language="JavaScript1.2" src="../cms_ewebeditpro.js"></script>
<style>
P { font-size : small; font-family : verdana, helvetica; }
H1 { font-family : verdana, helvetica; }
H2 { font-family : verdana, helvetica; }
A { font-family : verdana, helvetica; }
BODY { font-size : small; }
</style>

<script language="JavaScript1.2">
<!--
var g_bWifxReady = false;
var g_bEwepReady = false;
var g_bSetTimeout = false;

function ReloadParent()
{
	document.loadingMsg.style.visibility = "hidden";
	if (top.opener && !top.opener.closed)
	{
		top.opener.location.reload();
		setTimeout("self.close()", 5000);
	}
}
function OnReadyHandler()
{
	g_bEwepReady = true;
	if (true == g_bWifxReady && false == g_bSetTimeout)
	{
		g_bSetTimeout = true;
		ReloadParent();
	}
}
function OnWifxReadyHandler()
{
	if (false == g_bWifxReady)
    {
        g_bWifxReady = true;
		if (true == g_bEwepReady && false == g_bSetTimeout) 
		{
			g_bSetTimeout = true;
			ReloadParent();
		}
     }
}
//-->
</script>

</head>
<body>

<p align="center"><h2 align="center">
eWebEditPro with WebImageFX <br>Automatic <br>Download and Installation
</h2></p>

<form method="post">
<input type=hidden name="DoneMsg" value="&lt;p&gt; &lt;/p&gt;&lt;p align=center&gt;&lt;font face='Arial' size=4&gt;Installation complete.&lt;/font&gt;&lt;/p&gt;">
<input type=hidden name="RestartMsg" value="&lt;p&gt; &lt;/p&gt;&lt;p align=center&gt;&lt;font face='Arial' size=4&gt;Please restart Windows to complete the installation.&lt;/font&gt;&lt;/p&gt;">

<p align="center"><font size=-1>
<img name=loadingMsg src="loading.gif" alt="Downloading, please wait..." width=234 height=30><br>
If successful, this window will close automatically.
</font></p>
<!-- The install object -->
<script language="JavaScript1.2">
<!--
	var sCaption = "   eWebEditPro client installed.";
	var sWifxCaption = "   WebImageFX client installed.";
	writeLoader(sCaption, sWifxCaption);
// -->
</script>
<p><font size=-1>
<script language="JavaScript1.2">
<!--
	document.write('If a small red <font color="red"><b>X</b></font> appears, try downloading the ');
	document.write('<a href="' + eWebEditProDefaults.clientInstall + '">');
	document.write('client installation program</a> and running it. ');
// -->
</script>
</font></p>
<p><font size=-1>
For additional assistance, visit <a href="http://www.ektron.com/support" target="_blank">Ektron's support page</a>.
</font></p>
</form>

</body>
</html>
