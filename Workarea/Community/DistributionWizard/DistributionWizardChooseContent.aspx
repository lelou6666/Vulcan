<%@ Page Language="C#" MasterPageFile="DistributionWizard.master" AutoEventWireup="true" CodeFile="DistributionWizardChooseContent.aspx.cs" Inherits="Community_DistributionWizard_DistributionWizardChooseContent" Title="Distribution Wizard" %>
<%@ Register Src="SelectContent.ascx" TagName="SelectContent" TagPrefix="uc1" %>
<%@ MasterType VirtualPath="DistributionWizard.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphDistributionWizardContent" Runat="Server">
    <div class="DistributionWizardErrorMessage"><asp:Label ID="lblErrorMessage" runat="server"></asp:Label></div>
    <div id="DistributionWizardChooseContent">
        <uc1:SelectContent ID="scSelectContent" runat="server" />
    </div>
    <div id="DistributionWizardFooter"><asp:Button ID="btnBack" Text="Back" runat="server" OnClick="btnBack_Click" />&nbsp;<asp:Button ID="btnNext" runat="server" Text="Next" OnClick="btnNext_Click" />&nbsp;&nbsp;<asp:Button ID="btnDone" runat="server" Text="Done" Enabled="false" />&nbsp;<asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" /></div>
</asp:Content>

