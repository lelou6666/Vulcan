<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<link rel="canonical" href="http://www.vulcanequipment.com/k-12/" />
<style type="text/css">
    
    .k12_heading
    {
        font-family:'helvetica bold', Arial, Sans-Serif;
        font-size:23px;
        color:#e1e0e0;
        line-height:27px;
        padding-bottom:10px;
        padding-top:20px;
    }
    
    .k12_header_img
    {
        vertical-align:bottom; 
        position: absolute; 
        bottom: 0; 
        right:0;
    }
    
    #justforschool
    {
        padding:130px 0 20px 0;
    }
    
    #k12_copy
    {
        padding-bottom:40px;
    }
    
    #k12_middle 
    {
        padding-bottom:35px;
        overflow:auto;
    }
    
    @media all and (max-width:910px) 
    {
       #k12_middle .twocolumn
        {
            width:100%;
        } 
    }
    
    @media all and (max-width:680px) 
    {
        #k12_header .twocolumn, #k12_bottom .twocolumn, .k12_prods .twocolumn
        {
            width:100%;
        }
    
    
        .k12_header_img
        {
            position: relative; 
        }
        #k12_copy
        {
            padding-bottom:0px;
        }  
    }
    
    @media all and (max-width:660px) 
    {
        #justforschool
        {
            padding:170px 0 20px 0;
        }
    }
    
     @media all and (max-width:375px) 
    {
        #justforschool
        {
            padding:200px 0 20px 0;
        }
    }
    
    .heading_background
{


box-shadow: inset 50px 0 100px -10px #3e3e3e, inset -50px 0 100px -4px #3e3e3e;
}

</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <div class="dots_background" id="k12_header">
     <div class="row" style="overflow:auto; position:relative;">
        <div class="row_padding" style=" display: block;">
            <div style="position:absolute; top:0px; color:#e2e2e2; font-family: 'futura medium condensed', Arial, Sans-Serif; font-size:46px; padding-top:45px; line-height:48px;">
            better school nutrition <span style="color:#ed1c24;">starts in the kitchen.</span>
            </div>
            <div class="twocolumn">
                <img src="/images/just4schools.png" alt="just 4 schools" id="justforschool" style="width:100%; max-width:292px; max-height:80px;" />
                <p style="font-family:'helvetica bold', Arial, Sans-Serif; font-size:22px; line-height:28px;">A really smart move for your school.</p>
                <p id="k12_copy">Just 4 Schools provides everything you need to prepare the most delicious and nutritious school meals every day.</p>
            </div>
            <div class="twocolumn k12_header_img">
                <img src="/images/k12_header.png" alt="" style="width:100%; max-width:498px; height:auto;" />
            </div>
        </div>
    </div>
 </div>
 
<div class="body_wrapper" style="border-bottom: 2px solid #464646;">
<div class="body_wrapper2">
    <div style="margin:0 auto; text-align:center; width:100%; position:relative; z-index:2000; border-top: 2px solid #353535;"><img src="/images/gray_arrow.png" style="position:relative; top:-4px; margin:0 auto; text-align:center;" /></div>
     <div class="row" id="k12_middle">
        <div class="twocolumn"><div class="row_padding"><CMS:ContentBlock ID="ContentBlock2" runat="server" DefaultContentID="1540" /></div></div>
        <div class="twocolumn"><div class="row_padding"><CMS:ContentBlock ID="ContentBlock3" runat="server" DefaultContentID="1542" /></div></div>
    </div>
</div>
</div>

<div class="dark_body_wrapper">
<div class="dark_body_wrapper2">
    <div class="row">
        <br />
        <div class="row_padding" style="font-family:'futura medium condensed', Arial, Sans-Serif; 
            font-size:44px; color:#e2e2e2; line-height:47px; text-align:center;">smart equipment to fuel healthy minds</div>
    </div>
    <br style="clear:both;" />
    <br />
    <div class="row k12_prods">
        <div class="twocolumn">
            <div class="row_padding">
                <img src="/images/k12_ovens.jpg" alt="Ovens: VC Series, SG Series" usemap="#Map" style="width:100%; max-width:432px; margin-bottom:20px;" border="0" />
                <map name="Map" id="Map">
                  <area shape="rect" coords="0,0,215,431" href="/Products/VC-Series-Electric-Convection-Ovens/" />
                  <area shape="rect" coords="216,0,430,431" href="/Products/SG-Series-Gas-Convection-Ovens/" />
                </map>
            </div>
        </div>
        <div class="twocolumn">
            <div class="row_padding">
                <img src="/images/k12_heated_holding.jpg" alt="Heated Holding" usemap="#Map2" style="width:100%; max-width:433px; margin-bottom:20px;" border="0" />
                <map name="Map2" id="Map2">
                  <area shape="rect" coords="0,0,215,432" href="/Products/VBP-Institutional-Series-Cabinets/" />
                  <area shape="rect" coords="216,0,435,438" href="/Products/VPT-Pass-Through-Series-Cabinets/" />
                </map>
            </div>
        </div>
    </div>
    <br style="clear:both;" />
    <br />
    <div class="row k12_prods">
        <div class="twocolumn">
            <div class="row_padding">
                <table cellpadding="0" cellspacing="0"  width="100%" style="max-width:433px">
                    <tr>
                        <td align="left" style="padding-right:5px;"><a href="/products/steamers/"><img src="/images/k12_steamers.jpg" alt="Steamers" style="border:1px solid #292929; width:100%; max-width:200px; margin-bottom:20px;" /></a></td>
                        <td align="right"  style="padding-left:5px;"><a href="/products/kettles/"><img src="/images/k12_kettles.jpg" alt="Kettles" style="border:1px solid #292929; width:100%; max-width:200px; margin-bottom:20px;" /></a></td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="twocolumn">
            <div class="row_padding">
                <table cellpadding="0" cellspacing="0"  width="100%" style="max-width:433px">
                    <tr>
                        <td align="left" style="padding-right:5px;"><a href="/products/braising-pans/"><img src="/images/k12_braising_pan.jpg" alt="Braising Pans" style="border:1px solid #292929; width:100%; max-width:200px; margin-bottom:20px;" /></a></td>
                        <td align="left" style="padding-left:5px;"><a href="/products/combi/"><img src="/images/k12_combi.jpg" alt="Combi" style="border:1px solid #292929; width:100%; max-width:200px; margin-bottom:20px;" /></a></td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <br style="clear:both;" />
    <br />
    <div class="also_view_callouts">
        <div class="row">
            <div class="row_padding">
                <h2 style='border-bottom:1px solid #6c6c6a; color:#c3c3c3; margin-bottom:15px; text-transform:uppercase;'>People Have Also Viewed</h2>
                <CMS:Collection runat="server" ID="Collection1" DefaultCollectionID="17" DoInitFill="true" DisplayXslt="/xsl/k12_video.xsl" GetHtml="true"  />
            </div>
        </div>
    </div>
    <br style="clear:both;" />
    <br />
</div>
</div>

</asp:Content>