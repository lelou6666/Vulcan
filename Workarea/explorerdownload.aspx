<%@ Page Language="VB" AutoEventWireup="false" CodeFile="explorerdownload.aspx.vb" Inherits="ExplorerDownload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><%=m_refMsg.GetMessage("explorer")%></title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
	<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE"/>
	<meta content="JavaScript" name="vs_defaultClientScript"/>
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
	<asp:literal id="StyleSheetJS" runat="server"></asp:literal>
	
	<script type="text/javascript">
	<!--//--><![CDATA[//><!--
	function downloadExplorer()
	{
	    var url = new String( document.location ).replace( "explorerdownload.aspx?action=download", "explorer/clientinstall/EktronExplorer.exe" );
	    document.location = url;
	}
	//--><!]]>
	</script>
</head>
<body>
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
        <div class="ektronToolbar" id="divToolBar" runat="server"></div>
    </div>
    <div class="ektronPageContainer ektronPageInfo">
        <table class="ektronForm">			
		    <tr>
		        <td id="download_cell" runat="server"></td>
		    </tr>
	    </table>
	</div>
</body>
</html>
