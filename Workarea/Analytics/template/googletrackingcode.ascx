<%@ Control Language="C#" AutoEventWireup="true" CodeFile="googletrackingcode.ascx.cs" Inherits="Analytics_Template_GoogleTrackingCode" EnableTheming="false" EnableViewState="false" %>

<!-- Start Google Code -->
<script type="text/javascript">
var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
</script>
<script type="text/javascript">
try {
var pageTracker = _gat._getTracker("<asp:literal id="GoogleUserAccount" runat="server"/>");
pageTracker._trackPageview();
} catch(err) {}</script>
<!-- End Google Code -->
