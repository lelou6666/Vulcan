<%@ Page Title="" MasterPageFile="~/MasterPage.master"  CodeFile="Default.aspx.cs" Inherits="Default" MaintainScrollPositionOnPostback="true" %>
<%@ Register Src="~/controls/pager.ascx" TagPrefix="ctrl" TagName="datapager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/sales/" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div id="services_header">
     <div class="row" style="overflow:auto;">
        <div class="row_padding">
        	<br />
		   <h1 style="padding-bottom:15px;">Sales</h1>
           <p>Use the options below to locate a sales professional to help you with your order.</p>
           
           <div style="background-image:url(/images/grey-shade.png); background-repeat:repeat; padding:20px; max-width:800px; margin:0 auto;">
               <h3 style="text-align:center;">I work in:</h3>
               <div class="row" style="overflow:auto;">
                   <div class="twocolumn">
                        <div id="services_header_space" style="cursor:pointer;">
                            <a href="/sales/foodservice/"><img src="/images/foodservice.png" id="foodservice_img" alt="Foodservice" border="0" style="width:100%; max-width:266px; max-height:94px;" /></a>
                        </div>
                   </div>
                   <div class="twocolumn">
                        <div id="services_header_space2" style="cursor:pointer;">
                            <a href="/sales/grocery/"><img src="/images/grocery.png" id="grocery_img" alt="Grocery" border="0" style="width:100%; max-width:265px; max-height:94px;" /></a>
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