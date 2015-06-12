<%@ Page Title="" MasterPageFile="~/MasterPage.master"  CodeFile="Default.aspx.cs" Inherits="Default" MaintainScrollPositionOnPostback="true" %>
<%@ Register Src="~/controls/pager.ascx" TagPrefix="ctrl" TagName="datapager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="canonical" href="http://www.vulcanequipment.com/sales/foodservice/sales-representative/" />
    
    <style type="text/css">
		.state_dropdown
        {
            display:none;
        }
		
		#map
	    {
	        width:700px; 
	        height:400px; 
	        padding-bottom:40px;
	    }
             
        .rep-lnk { font-weight: bold; color: #cc0000 !important } 
        .listings p {padding-bottom:0px; color:#000 !important; }
		
		.video_padding {
			background-color:#525252;
			box-shadow: 0px 0px 8px #222222;
		}
		
		.events_title
		{
			color:#000;
			font-size:28px;
			margin-left:5px;
		}
		
		@media all and (max-width:1000px) 
	    {
            .state_dropdown
            {
                display:inline;
                margin: 0 auto;
                width:100%;
            }
	    }
		@media all and (max-width:860px) 
	    {
	        .threecolumn
	        {
	            width:50%;
	        }
	        
	        #map
	        {
	            width:500px; 
	            height:400px; 
	        }
	    }
	    
	    @media all and (max-width:550px) 
	    {
	        .threecolumn
	        {
	            width:95%;
	        }
	        
	        #map
	        {
	            width:400px; 
	            height:400px; 
	        }
	    }
	    
	    @media all and (max-width:460px) 
	    {
	        #map
	        {
	            width:280px; 
	            height:280px; 
	        }
	    }
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <div id="services_header">
         <div class="row" style="overflow:auto;">
            <div class="row_padding">
                <br />
               <h1 style="padding-bottom:15px;">Sales - Sales Representatives</h1>
               <p>Use the options below to locate a sales professional to help you with your order.</p>
               <div style="background-image:url(/images/grey-shade.png); background-repeat:repeat; padding:25px 30px 30px 30px; max-width:600px; margin:0 auto;">
                   <h3 style="text-align:center;">I work in <font style="color:#e11b23;">foodservice</font> 
                   <a href="/sales/"><img src="/images/edit-btn.jpg" alt="Edit" border="0" style="position:relative; top:8px;" /></a>
                   </h3>
                   <div style="border-top:1px solid #3d3d3e; height:1px; margin:15px; width:100%;">&nbsp;</div>
                   <h3 style="text-align:center;">I want to <font style="color:#e11b23;">find a sales rep</font> 
                   <a href="/sales/foodservice/"><img src="/images/edit-btn.jpg" alt="Edit" border="0" style="position:relative; top:8px;" /></a>
                   </h3>
               </div>
            </div>
        </div>
    </div>
    <div style="margin:0 auto; text-align:center; width:100%; position:relative; z-index:2000;"><img src="/images/black_arrow.png" style="position:relative; top:-2px; margin:0 auto; text-align:center;" /></div>
    <div class="body-wrapper">
    <div class="row" style="overflow:auto;">
        <div class="row_padding">
        	<br /><br />
            <div class="state_dropdown" id="contact_us" style="padding-bottom:20px; text-align:center;">
                <div style="padding-bottom:20px; overflow:auto;">
                <select onchange="changeState()" id="ddlStates">
                    <option value="">Select a State</option>
                    <option value="AL">Alabama</option>
                    <option value="AK">Alaska</option>
                    <option value="AZ">Arizona</option>
                    <option value="AR">Arkansas</option>
                    <option value="CA">California</option>
                    <option value="CO">Colorado</option>
                    <option value="CT">Connecticut</option>
                    <option value="DE">Delaware</option>
                    <option value="DC">District Of Columbia</option>
                    <option value="FL">Florida</option>
                    <option value="GA">Georgia</option>
                    <option value="HI">Hawaii</option>
                    <option value="ID">Idaho</option>
                    <option value="IL">Illinois</option>
                    <option value="IN">Indiana</option>
                    <option value="IA">Iowa</option>
                    <option value="KS">Kansas</option>
                    <option value="KY">Kentucky</option>
                    <option value="LA">Louisiana</option>
                    <option value="ME">Maine</option>
                    <option value="MD">Maryland</option>
                    <option value="MA">Massachusetts</option>
                    <option value="MI">Michigan</option>
                    <option value="MN">Minnesota</option>
                    <option value="MS">Mississippi</option>
                    <option value="MO">Missouri</option>
                    <option value="MT">Montana</option>
                    <option value="NE">Nebraska</option>
                    <option value="NV">Nevada</option>
                    <option value="NH">New Hampshire</option>
                    <option value="NJ">New Jersey</option>
                    <option value="NM">New Mexico</option>
                    <option value="NY">New York</option>
                    <option value="NC">North Carolina</option>
                    <option value="ND">North Dakota</option>
                    <option value="OH">Ohio</option>
                    <option value="OK">Oklahoma</option>
                    <option value="OR">Oregon</option>
                    <option value="PA">Pennsylvania</option>
                    <option value="RI">Rhode Island</option>
                    <option value="SC">South Carolina</option>
                    <option value="SD">South Dakota</option>
                    <option value="TN">Tennessee</option>
                    <option value="TX">Texas</option>
                    <option value="UT">Utah</option>
                    <option value="VT">Vermont</option>
                    <option value="VA">Virginia</option>
                    <option value="WA">Washington</option>
                    <option value="WV">West Virginia</option>
                    <option value="WI">Wisconsin</option>
                    <option value="WY">Wyoming</option>
                </select>
                </div>
            </div>
            
            <div id="map" style="margin:0 auto;"></div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                 <div id="clicked-state">
                    <div id="RepsContent" runat="server"> 
                        <!-- This contains the hidden content for state map -->
                        <div id="state_map" style='width:100%; max-width:1055px; margin: 0 auto; height:100%; min-height:400px;'>
                            <div class="video_padding results" id='state_map_content' style="min-height:330px; background:#959595;">
                                <div class="video_heading1">
                                    <div style="position:relative; float:right;"><a href="javascript:hideMap('#clicked-state');"><img src="/images/close.png" border="0" /></a></div>
                                    <div style="position:relative; float:left; color:#000; font-size: 28px;"><asp:Literal runat="server" ID="state_title"></asp:Literal></div>
                                </div>
                                <br /><br />
                                <div style="height:auto; overflow:auto;">
                                    <% if (Reps != null)
                                       {
                                           int localidx = 0;
                                    %>       
                                        <% foreach( Sales_Rep d in Reps){ %>   
                                            <div class="threecolumn" style="padding-bottom:25px;">                                   
                                                <table class="listings" cellpadding="0" cellspacing="0">
                                                    <tr>                
                                                        <td valign="top">
                                                            <p><b><%= d.Name%></b></p>
                                                            <p><%=d.Address%></p>
                                                            <p><%=d.City%>, <%= d.State%> <%= d.Zip%></p>
                                                            <p><%=d.Phone%></p>

                                                            
                                                            <% if (!string.IsNullOrEmpty(d.Territory)) { %>
                                                                <p><%=d.Territory%></p>                                        
                                                            <% } %>
                                                            
                                                            
                                                            <% if (!string.IsNullOrEmpty(d.Website)) { %>
                                                                <p><a target="_blank" href="<%= "http://" + d.Website %>" style="color:#000;"><%= d.Website.ToString().Replace("https://", "").Replace("http://", "").Replace("www.", "") %></a></p>
                                                            <% } %>

                                                            <p><b><a href='javascript:MapWindow("<%= Server.UrlEncode(d.Address + " " + d.City + " , " + d.State + " " + d.Zip) %>")'  style="color:#000;">get directions</a></b></p>
                                                        </td> 
                                                </table>
                                            </div>
                                        <% } %>
                                        <div style="display:none;"><p><ctrl:datapager runat="server" ID="dp"></ctrl:datapager></p></div>
                                        
                                        <%  } %>
                                </div>
                            </div>
                        </div>                             
                    </div>
                    <br /><br />
                 </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="stateValue" EventName="TextChanged" />
            </Triggers>
            </asp:UpdatePanel> 
            
            <p><br /><br />If you are located outside of the United States, please fill out our <a href="/Contact-Us/">contact us</a> form to locate a Vulcan Sales Representative.<br /><br /></p>
        <div style="display:none">
          <asp:TextBox ID="stateValue" name="stateValue" runat="server" Text="" AutoPostBack="true"></asp:TextBox> 
        </div>
    </div>
    </div>
    <br /><br />
</div>
</asp:Content>
<asp:Content ID="page_script" ContentPlaceHolderID="pageScripts" Runat="Server">
    <script src="/js/raphael.js" type="text/javascript"></script>
    <script src="/js/color.jquery.js" type="text/javascript"></script>
    <script src="/js/jquery.usmap.js" type="text/javascript"></script>

    	<script type="text/javascript" language="javascript">
    	    $(document).ready(function() {
				$('#map').usmap({
					stateHoverStyles: { fill: '#891123' },
					useAllLabels: true,
					// The click action
					click: function(event, data) {
						//$('#clicked-state').text('You clicked: ' + data.name)
						//.parent().effect('highlight', { color: '#C7F464' }, 2000);

						document.getElementById('<%= stateValue.ClientID %>').value = data.name;
						__doPostBack('<%= UpdatePanel1.ClientID %>', '');
						$('#map').hide();
					}
				});
    	    });

    	    function changeState() {
    	        var e = document.getElementById("ddlStates");
    	        document.getElementById('<%= stateValue.ClientID %>').value = e.options[e.selectedIndex].value;
    	        __doPostBack('<%= UpdatePanel1.ClientID %>', '');
    	        $('#map').hide();
    	    }  

        function hideMap(map_div) {
            $(map_div).hide();
            $('#map').show();
        }
    </script>
</asp:Content>