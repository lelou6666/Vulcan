<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Success.aspx.cs" Inherits="_Success" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .body_image
    {
       background-image: url('/images/products/product_heading.jpg');
        background-position: center bottom;
        background-repeat: no-repeat;
        background-color: #1a1819;
        min-height: 288px;
        overflow: auto;
    }
  </style>
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">

<div class="body_image">
     <div class="row" style="overflow:auto; padding-bottom:30px;">
            <div class="row" style="overflow:auto; min-height:300px;">
                <div class="row_padding">
                    <h2 style="padding-top:100px; text-align:center;">Thank you for the inquiry—we'll be in contact with you soon.</h2>
                </div>
            </div>
        </div>
    </div>
</asp:Content>