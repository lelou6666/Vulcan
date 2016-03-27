<%@ Page Title="" MasterPageFile="~/MasterPage.master"  CodeFile="Default.aspx.cs" Inherits="Default" MaintainScrollPositionOnPostback="true" %>
<%@ Register Src="~/controls/pager.ascx" TagPrefix="ctrl" TagName="datapager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/sales/foodservice/" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div id="services_header">
     <div class="row" style="overflow:auto;">
        <div class="row_padding">
        	<br />
		   <h1 style="padding-bottom:15px;">Sales - Foodservice</h1>
           <p>Use the options below to locate a sales professional to help you with your order.</p>
           <div style="background-image:url(/images/grey-shade.png); background-repeat:repeat; padding:25px 30px 30px 30px; max-width:600px; margin:0 auto;">
               <h3 style="text-align:center;">I work in <font style="color:#e11b23;">foodservice</font> 
               <a href="/sales/"><img src="/images/edit-btn.jpg" alt="Edit" border="0" style="position:relative; top:8px;" /></a>
               </h3>
               <div style="border-top:1px solid #3d3d3e; height:1px; margin:15px; width:100%;">&nbsp;</div>
               <h3 style="text-align:center;">I want to:</h3>
               <div class="row" style="overflow:auto;">
                   <div class="twocolumn">
                        <div id="services_header_space" style="cursor:pointer;">
                            <a href="/sales/foodservice/sales-representative/"><img src="/images/sales-rep-btn.jpg" id="foodservice_img" alt="Find a Sales Representative" border="0" style="width:100%; max-width:267px; max-height:96px;" /></a>
                        </div>
                   </div>
                   <div class="twocolumn">
                        <div id="services_header_space2" style="cursor:pointer;">
                            <a href="/sales/foodservice/dealer/"><img src="/images/dealer-btn.jpg" id="grocery_img" alt="Find a Vulcan Dealer" border="0" style="width:100%; max-width:267px; max-height:96px;" /></a>
                        </div>
                   </div>
               </div>
           </div>
        </div>
        <br />
        <br />
        <br />
        <br />
    </div>
</div>
</asp:Content>