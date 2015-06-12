<%@ Page Language="vb" AutoEventWireup="false" Inherits="dashboard" CodeFile="dashboard.aspx.vb" %>

<%@ Register Src="Personalization/personalization.ascx" TagName="personalization"
    TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>dashboard</title>
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    </head>
    <body>
        <form id="form1" runat="server">
		 <div id="mainDiv" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true"></asp:ScriptManager>
            <div class="ektronPageHeader">
                <div class="ektronTitlebar" id="divTitle" runat="server"></div>
                <div class="ektronToolbar">
                    <asp:Literal ID="HelpButton" runat="server" />        
                </div>
            </div>
            <div class="ektronPageContainer workareaPersonalizationWrapper">
                <uc1:personalization ID="Personalization1" runat="server" />
            </div>
		</div>
        </form>
    </body>
</html>