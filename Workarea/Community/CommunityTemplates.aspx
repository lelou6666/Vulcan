<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CommunityTemplates.aspx.vb"
    Inherits="Workarea_Community_CommunityTemplates" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CMS400.Net</title>
    <asp:Literal id="displaystylesheet" runat="server" />
    
    <script type="text/javascript" language="javascript">
    function Submit()
    {
        document.form1.submit();
        return true;
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <div class="ektronHeader"><asp:Label ID="lblGroupTemplates" runat="server" /></div>
            <div class="ektronTopSpaceSmall"></div>
            <table class="ektronForm">            
                <tr>
                    <td class="label"><asp:Label ID="lblGroupCommunityDocuments" runat="server" /></td>
                    <td><%=m_refContentAPI.SitePath %><asp:TextBox ID="txtGroupCommunityDocuments" Width="200px" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblGroupPhotoGallery" runat="server" /></td>
                    <td><%=m_refContentAPI.SitePath %><asp:TextBox ID="txtGroupPhotoGallery" Width="200px" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblGroupBlog" runat="server" /></td>
                    <td><%=m_refContentAPI.SitePath %><asp:TextBox ID="txtGroupBlog" Width="200px" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblGroupCalendar" runat="server" /></td>
                    <td><%=m_refContentAPI.SitePath %><asp:TextBox ID="txtGroupCalendar" Width="200px" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblGroupProfile" runat="server" /></td>
                    <td><%=m_refContentAPI.SitePath %><asp:TextBox ID="txtGroupProfile" Width="200px" runat="server" /></td>
                </tr>
                 <tr>
                    <td class="label"><asp:Label ID="lblGroupForum" runat="server" /></td>
                    <td><%=m_refContentAPI.SitePath %><asp:TextBox ID="txtGroupForum" Width="200px" runat="server" /></td>
                </tr>
            </table>
            
            <div class="ektronTopSpace"></div>
            <div class="ektronHeader"><asp:Label ID="lblUserTemplates" runat="server" /></div>
            <div class="ektronTopSpaceSmall"></div>
            <table class="ektronForm">
                <tr>
                    <td class="label"><asp:Label ID="lblUserCommunityDocuments" runat="server" /></td>
                    <td><%=m_refContentAPI.SitePath %><asp:TextBox ID="txtUserCommunityDocuments" Width="200px" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblUserPhotoGallery" runat="server" /></td>
                    <td><%=m_refContentAPI.SitePath %><asp:TextBox ID="txtUserPhotoGallery" Width="200px" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblUserBlog" runat="server" /></td>
                    <td><%=m_refContentAPI.SitePath %><asp:TextBox ID="txtUserBlog" Width="200px" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblUserCalendar" runat="server" /></td>
                    <td><%=m_refContentAPI.SitePath %><asp:TextBox ID="txtUserCalendar" Width="200px" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblUserProfile" runat="server" /></td>
                    <td><%=m_refContentAPI.SitePath %><asp:TextBox ID="txtUserProfile" Width="200px" runat="server" /></td>
                </tr>
            </table>                        
        </div>
    </form>
</body>
</html>
