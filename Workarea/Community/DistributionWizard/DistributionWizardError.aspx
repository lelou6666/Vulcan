<%@ Page Language="C#" MasterPageFile="DistributionWizard.master" AutoEventWireup="true" CodeFile="DistributionWizardError.aspx.cs" Inherits="Community_DistributionWizard_DistributionWizardError" Title="Distribution Wizard" %>
<%@ MasterType VirtualPath="DistributionWizard.master" %>

<asp:Content ID="contentDistributionError" ContentPlaceHolderID="cphDistributionWizardContent" Runat="Server">
    <asp:Label ID="lblErrorMessage" runat="server"></asp:Label>
    <div id="DistributionWizardFooter"><asp:Button ID="btnClose" Text="Close" runat="server" OnClick="btnClose_Click" /></div>
</asp:Content>

