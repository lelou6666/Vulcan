<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div class="body_wrapper">
<div class="body_wrapper2">
    <br />
     <div class="row">
        <div class="row_padding">
            <CMS:ContentBlock ID="ContentBlock2" runat="server" DynamicParameter="id" />
        </div>
     </div>
     <br />
     <br />
 </div>
 </div>
</asp:Content>