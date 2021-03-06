﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<link rel="canonical" href="http://www.vulcanequipment.com/irx/" />
<style type="text/css">
    h3{
        padding-bottom:12px;
    }
    
    #irx_header ul, #irx_body1 ul, #irx_body2 ul, #irx_body3 ul
    {
        list-style-image: url('/images/spec_arrow.png');
    }
    
    #irx_header li, #irx_body1 li, #irx_body2 li, #irx_body3 li
    {
        margin-left:40px;
    }
    
    @media all and (max-width:930px) 
	{
	    #irx_header .twocolumn
	    {
	        width:100%;
	    }
	}
	
	@media all and (max-width:660px) 
	{
	    #irx_body1 .twocolumn, #irx_body2 .twocolumn
	    {
	        width:100%;
	    }
	}
	
	@media all and (max-width:600px) 
	{
	    #irx_body3 .twocolumn
	    {
	        width:100%;
	    }
	}
</style>
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div id="irx_header" class="dots_background" style="overflow:auto;">
     <div class="row">
        <br />
        <div class="twocolumn">
            <div class="row_padding">
                <h3 style="font-size:44px; line-height:50px; padding-top:10px;"><span style="color:#ed1c24;">InfraRed Technology</span> in Your Kitchen</h3>
                <CMS:ContentBlock ID="ContentBlock2" runat="server" DefaultContentID="1585" /> 
            </div>
        </div>
        <div class="twocolumn">
            <div class="row_padding">
                <img src="/images/irx1.png" alt="IRX" style="width:100%; vertical-align:bottom; max-width:451px; padding-top:20px;" />
            </div>
        </div>
     </div>
</div>
<div id="irx_body1" style="overflow:auto;">
      <div class="body_wrapper">
      <div class="body_wrapper2">
         <br />
         <div class="row">
            <div class="twocolumn">
                <div class="row_padding" style="padding-bottom:20px;">
                    <CMS:ContentBlock ID="ContentBlock3" runat="server" DefaultContentID="1587" /> 
                </div>
            </div>
            <div class="twocolumn">
                <div class="row_padding" style="padding-bottom:20px;">
                    <CMS:ContentBlock ID="ContentBlock1" runat="server" DefaultContentID="1586" /> 
                </div>
            </div>
         </div>
        <br />
         <div class="row">
            <div class="row_padding">
                <div style="max-width:380px;"><CMS:ContentBlock ID="ContentBlock7" runat="server" DefaultContentID="1590" /></div>
                <a href="/video-library/?video=47963703&title=Vulcan%20VTEC%20Series%20Charbroiler&player=3"><img src="/images/irx3.png" alt="IRX" style="width:100%; max-width:835px; max-height:306px;" /></a>
            </div>
            <div class="row">
                <div class="row_padding" style="padding-bottom:20px;">
                    <CMS:ContentBlock ID="ContentBlock8" runat="server" DefaultContentID="2227" /> 
                </div>
             </div>
            <div class="twocolumn">
                <div class="row_padding">
                    <CMS:ContentBlock ID="ContentBlock6" runat="server" DefaultContentID="1591" /> 
                </div>
            </div>
            <div class="twocolumn">
                <div class="row_padding">
                    <CMS:ContentBlock ID="ContentBlock4" runat="server" DefaultContentID="1592" />
                    <br />
                    <br /> 
                </div>
            </div>
         </div>
     </div>
     </div>
</div>
</asp:Content>