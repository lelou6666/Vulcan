<%@ Page Title="" MasterPageFile="~/MasterPage.master"  CodeFile="Default.aspx.cs" Inherits="Default" MaintainScrollPositionOnPostback="true" %>
<%@ Register Src="~/controls/pager.ascx" TagPrefix="ctrl" TagName="datapager" %>
<%@ Register Src="/includes/downloads.ascx" TagName="Downloads" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link rel="canonical" href="http://vulcanequipment.com/service/" />
	<link rel="stylesheet" href="http://code.jquery.com/ui/1.9.1/themes/base/jquery-ui.css" />
    <script src="http://code.jquery.com/jquery-1.8.2.js"></script>
    <script src="http://code.jquery.com/ui/1.9.1/jquery-ui.js"></script>
    <script type="text/javascript" src="http://extjs.cachefly.net/builds/ext-cdn-1853.js"></script>
        
    <style type="text/css">
        .listings p {padding-bottom:0px;}
        .defaultText { }
        .defaultTextActive { color: #a1a1a1; font-style: italic; }
		#next_previous a { color:#2E5C7C; }
		
		.events_title
		{
			color:#000;
			font-size:28px;
			margin-left:5px;
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
    </script>
        
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBceJ4ereZCAJt8bodBzq-k58RanFKu-Ww&sensor=false"></script> 
        
         <script type="text/javascript">
            Ext.onReady(function() {
                // make sure the enter key press event acts like clicking submit
                var s = Ext.get('<%= txtSearch.ClientID %>');
                s.on('keypress', function(e) {
                    if (e.keyCode == Ext.EventObject.ENTER) {
                        e.stopEvent();
                        btnSearch_onclick();
                    }
                });
                mapon = false;
                <% if(Agents.Count != 0)
                { %>
                    mapon = true;
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
					window.location = '/Service/?search=' + escape(txt.getValue());
	                
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
    		
		    foreach (Agent d in Agents){ 

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
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <div class="service_wrapper">
        <br />
        <div class="row">
            <div class="row_padding">
                <h1 style="padding-bottom:10px;">Service</h1>
                <p>Our products are supported throughout North America and abroad by two outstanding service networks&ndash;the world-class 
                Hobart Service network or a Certified Independent Service network.</p>
                <p><a href="https://vulcanregistration.itwfeg.com/FilterRegistration/" target="_blank">Register</a> your steamer and filter, or check out the <a href="/pdf/PartsWarranty.pdf" target="_blank">2015 Warranty</a>.</p>
                <br />
        	</div>
        </div>
 		<br />
        <input type="hidden" runat="server" id="vBrand" />
        <input type="hidden" runat="server" id="hZip" />
        <div class="row" style="overflow:auto;">
            <div class="row_padding">
            	<div style="background-image:url(/images/grey-shade.png); background-repeat:repeat; padding:20px; max-width:800px; margin:0 auto;">
                    <h3 style="text-align:center;">locate a Vulcan service / parts center:</h3>
                    <table border="0" align="center" width="100%" style="padding-top:8px;">
                        <tr>
                            <td>
                                <table border="0" align="center" width="300" style="margin:0 auto;">
                                    <tr>
                                        <td align="center" id="contact_us"><asp:TextBox ID="txtSearch" class="defaultText" title="Search by zip code" runat="server" Width="200" autocomplete="off"></asp:TextBox>
                                        </td>
                                        <td align="left"><input type="button" id="Button1" onclick="return btnSearch_onclick();" style="width:81px; height:40px; background-image:url('/images/go.jpg'); border:none;" /></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" style="margin:0 auto;">
                            <% if (Request.QueryString["search"] != null) { %>
                                <p style="margin-right:235px;"><%= itemcount %> results</p>
                                <br />
                            <% } %>
                            </td>
                        </tr>
                     </table>
                 </div>
             </div>
        </div>
    </div>       
    <% if (Agents.Count != 0) { %>
        <div><a name="mloco"></a></div>
        <div id="map_canvas" style="height:390px; width:100%;"></div>
        
       <div style="background:#959595; width:100%;">
        <br />
        <div class="row">
            <div class="row_padding">
               <% int localidx = 0;
                  int count = 0;
                  bool showHobart = false;
                  bool showInd = false;

                   foreach (Agent d in Agents)
                   { 
                       count++;
                       
                       if(count > (dp.CurrentPage - 1) * 8 && count <= (dp.CurrentPage -1) * 8 + 8) 
                       {  %>
                       
                            <!-- We have used office_addr2 as a variable
                            To Display Header like 'Hobart' or 'Independent'  -->
                            <% if (showHobart == false && d.Hobart == "Hobart")
                            { 
                                showHobart = true; %>
                                <br style="clear:both;" />
                                <div class="events_title">Hobart</div>
                           <% }
                           else if (showInd == false && d.Hobart == "Independant")
                            { 
                                showInd = true; %>
                                <br style="clear:both;" />
                                <br />
                                <div class="events_title">Independent</div>
                           <% }  %>
                       
                           <div class="fourcolumn">
                               <table cellpadding="0" cellspacing="0" style="margin:5px; height:150px;" class="listings">
                                 <tr>                        
                                    <td valign="top">
                                        <p>
                                             <% if ((!string.IsNullOrEmpty(d.Address)) && (!string.IsNullOrEmpty(d.City)) && (!string.IsNullOrEmpty(d.State)))  { %> 
                                                    <b><a style='color:#000000' href='javascript: gotoPoint(<%= localidx++ %>)'><%= d.Name%></a></b>
                                              <% } else  { %>
                                                    <b><a style='color:#000000' id='<%= localidx++ %>'><%= d.Name%></a></b>
                                              <% } %> 
                                        </p>
                                         
                                         <% if ((!string.IsNullOrEmpty(d.Address)) && (!string.IsNullOrEmpty(d.City)) && (!string.IsNullOrEmpty(d.State)))  { %>   
                                                <p><%= d.Address%></p>
                                                <p><%= d.City.Trim()%>, <%= d.State%> <%= d.Zip%></p>
                                         <% } %> 
                                         
                                         <% if (d.Phone != "null") { %>
                                              <% if (!string.IsNullOrEmpty(d.Phone)) { %>
                                                    <p>Phone: <%= d.Phone%></p>
                                              <% }
                                         } %>                   
                                        
                                        <% if (!string.IsNullOrEmpty(d.Website)) { %>
                                            <p>
                                                <a target="_blank" href="<%= d.Website %>" style="color:#000;">
                                                    <%= d.Website.ToString().Replace("https://", "").Replace("http://", "").Replace("www.", "") %>
                                                </a>
                                            </p>
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
        
        <div class="row">
            <div class="row_padding" style="padding-bottom:8px;">
                <div class="row" style="padding-top:8px;">
                	<asp:Downloads runat="server" id="DL" />
                </div>
        	</div>
        </div>
        </div>
</asp:Content>
