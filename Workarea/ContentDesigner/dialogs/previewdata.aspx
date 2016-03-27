<%@ Page Language="C#" AutoEventWireup="true" CodeFile="previewdata.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.previewdata" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<style type="text/css">
html, body  {width: 100%; height: 100%; padding: 0; margin: 0}
#mainform
{
    height: 90%;
}
#Ektron_FrameContentMiddle
{
	overflow: hidden !important;
	width: 99%; 
	top: 2px !important;
}
#txtContent
{
	overflow: auto;
	width: 99%; 
	height: 95%;
}
</style>
</head>
<body onload="initField()">
<form id="mainform" runat="server">
    <div id="Ektron_FrameContentMiddle">
        <textarea id="txtContent" class="Text Ektron_ReadOnlyBox" readonly="readonly"></textarea>
    </div> 
    <div class="Ektron_Dialogs_LineContainer">
        <div class="Ektron_TopSpaceSmall"></div>
        <div class="Ektron_StandardLine"></div>
    </div>	
    <div class="Ektron_Dialogs_ButtonContainer Ektron_Dialogs_Buttons" style="position: absolute; bottom: 0;">
        <asp:button ID="btnCancel" CssClass="Ektron_StandardButton" OnClientClick="CloseDlg(); return false;" runat="server" />
    </div>
</form>
<script language="javascript" type="text/javascript">
<!--
	function initField()
	{
	    // To retrieve arguments from the dialog.
	    var args = GetDialogArguments();
	    if (args)
	    {
	        document.getElementById("txtContent").value = args.content;
	    }
	}
	
//-->
</script>
</body>
</html>