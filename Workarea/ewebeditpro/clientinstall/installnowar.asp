<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<!-- Copyright 2001 Ektron, Inc. -->
<!-- Revision Date: 2001-08-22 -->
<html>
<head>
<meta http-equiv=Content-Type content="text/html; charset=windows-1256">
<title>تثبيت برنامج &quot;eWebEditPro with WebImageFX&quot;</title>
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
var LicenseKeys = "<%=licitem%>";
var eWebEditProPath = "<%= AppeWebPath %>";
var WebImageFXPath = "<%= AppeWebPath %>";
var eWebEditProMsgsFilename = "ewebeditpromessages" + "<%=gtMess("BrowserCode")%>" + ".js";
var eWebEditProinstallPopupUrl ="<%=(AppeWebPath)%>clientinstall/intro" + "<%=gtMess("BrowserCode")%>.htm";
</script>
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
function reloadOpener()
{
	if (top.opener && !top.opener.closed)
	{
		top.opener.location.reload();
	}
}
//-->
</script>

</head>

<body onunload="reloadOpener()">

<p dir="rtl" align="center"><h2 align="center">
التحميل السفلي لبرنامج &quot;eWebEditPro with WebImageFX&quot; وتثبيته بصفة أوتوماتيكية
</h2></p>

<form method="post">
<input type=hidden name="DoneMsg" value="&lt;p dir='rtl'&gt; &lt;/p&gt;&lt;p dir='rtl' align=center&gt;&lt;font face='Arial' size=4&gt;انتهاء عملية التثبيت.&lt;/font&gt;&lt;/p&gt;">
<input type=hidden name="RestartMsg" value="&lt;p dir='rtl'&gt; &lt;/p&gt;&lt;p dir='rtl' align=center&gt;&lt;font face='Arial' size=4&gt;يرجى بدء سير نظام التشغيل Windows من جديد لاستكمال عملية التثبيت &quot;.&lt;/font&gt;&lt;/p&gt;">

<p dir="rtl" align="center"><font size=-1>
<img name=loadingMsg src="loading.gif" alt="سير عملية الشحن السفلي للبيانات، يرجى الانتظار ..." width=234 height=30><br>
إن تمت العملية فعلا بنجاح، فإن العبارة &quot;انتهاء عملية التثبيت&quot; تظهر داخل الصندوق أدناه.
</font></p>
<script language="JavaScript1.2">
<!--
	eWebEditPro.onready = onReadyHandler;
	eWebEditPro.actionOnUnload = EWEP_ONUNLOAD_NOSAVE;
	eWebEditPro.parameters.installPopup = null;
	eWebEditPro.create("DoneMsg", "100%", 200);
	
	function onReadyHandler()
	{
		document.loadingMsg.style.visibility = "hidden";
		eWebEditPro.refreshStatus();
		if (eWebEditPro.autoInstallExpected())
		{
			eWebEditPro.instances[0].load(document.forms[0].elements.RestartMsg.value);
			document.body.onunload = ""; // don't reload the opener window
		}
	}
//-->
</script>
<p dir="rtl"><font size=-1>
<script language="JavaScript1.2">
<!--
	document.write('إن ظهر الحرف الصغير X  بلون أحمر، يجب محاولة انجاز عملية التحميل السفلي');
	document.write('<a href="' + eWebEditProDefaults.clientInstall + '">');
	document.write('برنامج التثبيت لدى الزبون</ a> وتشغيله. ');
// -->
</script>
</font></p>
<p dir="rtl"><font size=-1>
للحصول على مساندة إضافية, يرجى الإطلاع على <a href="http://www.ektron.com/support" target="_blank">صفحة معلومات المساعدة لشركة Ektron (اكترون)</a>.
</font></p>
<p dir="rtl" align="center">
<input type="button" name="btnClose" value="إغلاق" onclick="self.close()"> 
 
</p>
</form>

</body>
</html>
