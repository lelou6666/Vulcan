<%@ Page Title="" MasterPageFile="~/MasterPage.master"  CodeFile="Default.aspx.cs" Inherits="Default" MaintainScrollPositionOnPostback="true" %>
<%@ Register Src="~/controls/pager.ascx" TagPrefix="ctrl" TagName="datapager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/sales/foodservice/dealer/" />

    <style type="text/css">
        .defaultText { }
        .defaultTextActive { color: #a1a1a1; font-style: italic; }
		#next_previous a { color:#2E5C7C; }
		.listing a {
			color: #000 !important;
		}
		
		.listing p {
			color: #000 !important;
		}
        
        @media all and (max-width:860px) 
	    {
	        .fourcolumn
	        {
	            width:50%;
	        }
	    }
	    
	    @media all and (max-width:550px) 
	    {
	        .fourcolumn
	        {
	            width:100%;
	        }
	    }
    </style> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <div id="services_header">
         <div class="row" style="overflow:auto;">
            <div class="row_padding">
                <br />
               <h1 style="padding-bottom:15px;">Sales - Vulcan Dealers</h1>
               <p>Use the options below to locate a sales professional to help you with your order.</p>
               <div style="background-image:url(/images/grey-shade.png); background-repeat:repeat; padding:25px 30px 30px 30px; max-width:600px; margin:0 auto;">
                   <h3 style="text-align:center;">I work in <font style="color:#e11b23;">foodservice</font> 
                   <a href="/sales/"><img src="/images/edit-btn.jpg" alt="Edit" border="0" style="position:relative; top:8px;" /></a>
                   </h3>
                   <div style="border-top:1px solid #3d3d3e; height:1px; margin:15px; width:100%;">&nbsp;</div>
                   <h3 style="text-align:center;">I want to <font style="color:#e11b23;">find a Vulcan dealer</font> 
                   <a href="/sales/foodservice/"><img src="/images/edit-btn.jpg" alt="Edit" border="0" style="position:relative; top:8px;" /></a>
                   </h3>
               </div>
            </div>
        </div>
    </div>
    <div style="margin:0 auto; text-align:center; width:100%; position:relative; z-index:2000;"><img src="/images/black_arrow.png" style="position:relative; top:-2px; margin:0 auto; text-align:center;" /></div>
    	<div class="body-wrapper">
    	<br />
        <br />
      <input type="hidden" runat="server" id="vBrand" />
      <input type="hidden" runat="server" id="hZip" />
      <div id="itemcontainer" style="overflow:auto;">
            <div class="row">
                <div class="row_padding">
                    <table border="0" align="center" width="100%">
                        <tr>
                            <td id="contact_us">
                                <table border="0" align="center" width="300" style="margin:0 auto;">
                                    <tr>
                                        <td align="center"><asp:TextBox ID="txtSearch" class="defaultText" title="Search by zip code" runat="server" Width="200" AutoCompleteType="Disabled" autocomplete="off"></asp:TextBox></td>
                                        <td align="left"><input type="button" id="Button1" onclick="return btnSearch_onclick();" style="width:81px; height:40px; background-image:url('/images/go.jpg'); border:none;" /></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" style="height:42px; margin:0 auto;">
                            <% if (Request.QueryString["search"] != null) { %>
                                <p style="margin-right:235px;"><%= itemcount %> results</p>
                                <br />
                            <% } %>
                            </td>
                        </tr>
                     </table>
                 </div>
            </div>
     
         <% if (Dealers.Count != 0) { %>
            <div><a name="mloco"></a></div>
            <div id="map_canvas" style="height:350px; width:100%;"></div>
            
        <div style="background:#959595; width:100%;">    
            <div class="row">
            	<div class="row-padding">            
            	<br />
                <% int localidx = 0;
	   	          int count = 0;

                   foreach (Company d in Dealers)
                   { 
		           count++;
    			   
		               if(count > (dp.CurrentPage - 1) * 8 && count <= (dp.CurrentPage -1) * 8 + 8) { %>
			                <div class="fourcolumn">
                               <table cellpadding="0" cellspacing="0" style="margin:5px; height:200px;" class="listings">
                                   <tr>                        
                                        <td valign="top">
                                            <p style="padding-top:12px;">
							                     <% if ((!string.IsNullOrEmpty(d.Address)) && (!string.IsNullOrEmpty(d.City)) && (!string.IsNullOrEmpty(d.State))) { %> 
                                                        <b><a style='color:#000000' href='javascript: gotoPoint(<%= localidx++ %>)'><%= d.Name%></a></b>
                                                  <%} else { %>
                                                        <b><a style='color:#000000' id='<%= localidx++ %>'><%= d.Name%></a></b>
                                                  <%} %> 
                                             </p>
                                         
                                                <%  if ((!string.IsNullOrEmpty(d.CompanyName))) { %>
                                                        <p><%=d.CompanyName%></p>
                                                <% } 
                                                   
						                         if ((!string.IsNullOrEmpty(d.Address)) && (!string.IsNullOrEmpty(d.City)) && (!string.IsNullOrEmpty(d.State))) { %>   
                                                    <p><%= d.Address%></p>
                                                    <p><%= d.City %>, <%= d.State%> <%= d.Zip%></p>
                                                    
                                                <%} %> 

                                                <% if (d.Phone != "null") { %>
                                                      <% if (!string.IsNullOrEmpty(d.Phone)) { %>
                                                            <p>Phone: <%= d.Phone%></p>
                                                      <% } %>
                                                <% } %>
                                               
                                               <% if (d.Email != "null") { %>
                                                    <% if (!string.IsNullOrEmpty(d.Email)) { %>
                                                        <p>Email: <a href="mailto:<%= d.Email%>"><%= d.Email%></a></p>
                                                    <% } %>
                                               <% } %>
                                                        
		                                       <% if (!string.IsNullOrEmpty(d.Website))
                                               {
                                                    if (Convert.ToString(d.Website)!= "")
                                                    { %>
                                                        <p><a target="_blank" style="color:#000;" href="<%= d.Website %>"><%= d.Website.ToString().Replace("https://", "").Replace("http://", "").Replace("www.", "")  %></a></p>
                                                    <% } 
                                               } %>
                                               
                                               <% if (d.Distance != "null") { %>
                                                    <p><b>Distance:</b> <%= d.Distance %> mile(s)</p>
                                               <% } %>
                                        </td>
                                   </tr>
                               </table>
                            </div>
                      <% } %>  
                   <% } %>  
                </div>
            </div>
            <br style="clear:both;" />
            <div class="row" id="next_previous">
                    <p style="border-top:1px solid #626160; width:100%; padding-top:10px; color:#000;"><ctrl:datapager runat="server" ID="dp"></ctrl:datapager></p>
           </div> 
        <% } %> 
        </div> 
        <div class="row">
        	<div class="row_padding">
           		<br /><br />
           		<p style="text-align:center;">If you are located outside of the United States, please <a href="/contact-us/">contact us</a> to locate a Vulcan Dealer.</p>
                <br />
            </div>
        </div>
    </div>
    </div>
    
</asp:Content>
<asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
	<script type="text/javascript" src="http://extjs.cachefly.net/builds/ext-cdn-1853.js"></script>
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBceJ4ereZCAJt8bodBzq-k58RanFKu-Ww&sensor=false"></script>
    
	<script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $(".defaultText").focus(function(srcc) {
                if ($(this).val() == $(this)[0].title) {
                    $(this).removeClass("defaultTextActive");
                    $(this).val("");
                }
            });

            $(".defaultText").blur(function() {
                if ($(this).val() == "") {
                    $(this).addClass("defaultTextActive");
                    $(this).val($(this)[0].title);
                }
            });

            $(".defaultText").blur();
        });

        Ext.onReady(function() {
            // make sure the enter key press event acts like clicking submit
            var s = Ext.get('<%= txtSearch.ClientID %>');
            s.on('keypress', function(e) {
                if (e.keyCode == Ext.EventObject.ENTER) {
                    e.stopEvent();
                    btnSearch_onclick();
                }
            });
            
            <% if(Dealers.Count != 0) { %>
                zipcode = Ext.get('<%= hZip.ClientID %>').getValue();
            	initialize();
            <% } %>
        });
        // this is fired when either someone clicks submit or they press enter while in the search box
        function btnSearch_onclick() {

            var txt = Ext.get('<%= txtSearch.ClientID %>');
            var dist = 50;
            var brand = Ext.get('<%= vBrand.ClientID %>');
            
            if (txt.getValue().length > 0) {
                //window.location = 'Default.aspx?brand=' + escape(brand.getValue()) + '&search=' + escape(txt.getValue()) + '&distance=' + escape(dist);
				window.location = '/sales/foodservice/dealer/?search=' + escape(txt.getValue());
                return true;

            } else {
                return false;
            }
        }
        
        var map;
        var infowindow = [];
        var marker = [];
        
        function initialize() {
            var mapOptions = {
              zoom: 8,
              mapTypeId: google.maps.MapTypeId.ROADMAP,
              scrollwheel: false
            };
            
            map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
            
            setMarkers();
        }
          
          function setMarkers() {
            <%  
			int counter = 0;
			
			foreach (Company d in Dealers){ 

				counter++;
				
			    if(counter == 1) { %>
						map.setCenter(new google.maps.LatLng("<%= d.Latitude %>", "<%= d.Longitude %>"), 8);
				<% } %>
				
				<% if(counter > (dp.CurrentPage - 1) * 8 && counter <= (dp.CurrentPage -1) * 8 + 8) { %>
				    var inner = "<%= d.Address %> <%= d.City %>, <%= d.State %> <%= d.Zip %>";
					var html = "<b><%= d.Name %></b> <br/><%= d.Address %><br/><%= d.City %>, <%= d.State %> <%= d.Zip %>"
						 + "<br/><a target='_blank' href='http://maps.google.com/maps?daddr=" + escape(inner) + "&hl=en'>get directions</a>" ;
					
					var myLatlng = new google.maps.LatLng(<%= d.Latitude %>, <%= d.Longitude %>);
                    var newmarker = new google.maps.Marker({
                        position: myLatlng,
                        map: map,
                        html: html,
                        icon: '/images/marker.png'
                    });
                    
                    newmarker['infowindow'] = new google.maps.InfoWindow({
                        content: html
                    });
                    
                    <% if ((!string.IsNullOrEmpty(d.Address)) && (!string.IsNullOrEmpty(d.City)) && (!string.IsNullOrEmpty(d.State))) { %> 
                        google.maps.event.addListener(newmarker, 'click', function() {
                            for (var i=0;i<marker.length;i++) {
                                 marker[i]['infowindow'].close();
                              }
                            this['infowindow'].open(map, this);
                        });
                    <% } %> 
                    
                    marker.push(newmarker);
            

				<% } %>
			<% } %>
          }
        
        function gotoPoint(myPoint){
             for (var i=0;i<marker.length;i++) {
                 marker[i]['infowindow'].close();
              }
    
            map.setCenter(new google.maps.LatLng(marker[myPoint].position.lat(), marker[myPoint].position.lng()));
            marker[myPoint]['infowindow'].open(map, marker[myPoint]);
        }
    </script>
</asp:Content>