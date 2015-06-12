<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" EnableViewState="false" %>
<%@ Register TagPrefix="CMS" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<link rel="Stylesheet" type="text/css" href="jquery.datepick.css"></link>
    
    <style type="text/css">
		#data td {
			padding:5px;
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div class="body_wrapper">
<div class="body_wrapper2">
    <br />
     <div class="row">
        <div class="row_padding">
       		<div runat="server" id="loggedin">
                <table cellpadding="0" cellspacing="0" id="contact_us">
                    <tr>
                        <td class="form_label">Submission Form:&nbsp;</td>
                        <td class="form_fields">
                            <asp:DropDownList runat="server" ID="report" OnSelectedIndexChanged="updateVisible">
                                <asp:ListItem Text="Contact Us" Value="contact"></asp:ListItem>
                                <asp:ListItem Text="Newsletter" Value="Newsletter"></asp:ListItem>
                                <asp:ListItem Text="Combi Info Request" Value="Combi"></asp:ListItem>
                                <asp:ListItem Text="Combi Simulator" Value="Simulator"></asp:ListItem>
                            </asp:DropDownList>
                            <br /><br />
                        </td>
                    </tr>
                    <tr>
                        <td class="form_label">Start Date:&nbsp;</td>
                        <td class="form_fields" id="start">
                            <asp:TextBox ID="startdate" class="field" runat="server" OnTextChanged="updateVisible"></asp:TextBox>
                            <br /><br />
                        </td>
                    </tr>
                    <tr>
                        <td class="form_label">End Date:&nbsp;</td>
                        <td class="form_fields" id="end">
                            <asp:TextBox ID="enddate" class="field" runat="server" OnTextChanged="updateVisible"></asp:TextBox>
                            <br /><br />
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td><asp:ImageButton ID="submit" runat="server" ImageUrl="view.jpg" OnClick="updateVisible" Width="171" Height="48" /></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td><asp:ImageButton ID="submit2" runat="server" ImageUrl="export.jpg" OnClick="Export_Click" Width="124" Height="48" /></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td><br /><asp:Literal runat="server" id="litResults" />  </td>
                    </tr>
                </table>   
                <br />
                <div id="data" style="color:#b8b7b7; font-size:12px; font-family:Arial, Helvetica, sans-serif;">
                    <asp:DataGrid runat="server" ID="myDataGrid" /> 
                </div>
            </div>
            <div runat="server" id="loggedout">
            	<br /><br />
            	<p>You must be logged into the CMS to view this page.</p>
            </div>
        </div>
     </div>
     <br />
     <br />
 </div>
 </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="pageScripts" Runat="Server">
    <script type="text/javascript" src="jquery.plugin.js"></script> 
    <script type="text/javascript" src="jquery.datepick.js"></script>
    
    <script type="text/javascript">
        $( document ).ready(function() {
            $('#start input').datepick({ dateFormat: 'mm/dd/yyyy'});
            $('#end input').datepick({ dateFormat: 'mm/dd/yyyy' });
        });    
    </script>
</asp:Content>