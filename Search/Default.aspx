<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<%@ Register src="SiteSearchControl.ascx" tagname="SiteSearchControl" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 <link rel="canonical" href="http://www.vulcanequipment.com/search/" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <div class="body_wrapper">
    <div class="body_wrapper2">
    <div class="row">
        <div class="row_padding">
             <h1>Search</h1>
             	<div style="font-family: 'futura light condensed', Arial, Sans-Serif; color: #b0b0b0; font-size: 26px; margin-bottom:10px; border-bottom: 1px solid #b0b0b0;"><asp:Literal ID="litProdCount" runat="server"></asp:Literal></div>
                <p><asp:Literal ID="litProdResults" runat="server"></asp:Literal></p>
                <br /><br />
                <div style="font-family: 'futura light condensed', Arial, Sans-Serif; color: #b0b0b0; font-size: 26px; margin-bottom:10px; border-bottom: 1px solid #b0b0b0;"><asp:Literal ID="litCount" runat="server"></asp:Literal></div>
                <asp:Literal ID="litResults" runat="server"></asp:Literal>
        </div>
    </div>
    <br style="clear:both;" /><br />
    </div>
    </div>
</asp:Content>