<%@ Control Language="VB" AutoEventWireup="false" CodeFile="windowsmediaparams.ascx.vb" Inherits="Multimedia_windowsmediaparams" %>
<script type="text/javascript" language="JavaScript" src="controls/media/wmplayerparams.js"></script>
<script type="text/javascript" language="javascript">
    var jswindowsmedia = new ektWindowsMedia();
    function PreviewWindowsMedia() {
        jswindowsmedia.preview();
    }
</script>
<!--- Windows media player properties -->
<div id="windowsmedia-panel" style="display:none">		
<table width="100%" cellspacing="0" cellpadding="0" border="0">		
		<tr>
			<!-- <td>Balance:</td>
			<td><input onchange="PreviewWindowsMedia()" id="wm_balance" style="WIDTH: 64px" type="text" size="5" name="wm_balance" /></td>
			-->
			
		</tr>
		<tr>
			<!--<td colspan="2">
			<table cellspacing="0" cellpadding="0" border="0">			    
			     <tr>
			     
			        <td>
			            ContextMenu:</td>
			        <td>
			            <input onchange="PreviewWindowsMedia()" id="wm_contextmenu" type="checkbox" name="wm_contextmenu" /></td>
			           
			    </tr>
			</table>
			</td> -->
			<td colspan="2">
			<table cellspacing="0" cellpadding="0" border="0">
			    <tr>
			        <td>
                        <asp:label ID="lblContextMenu" runat="server" /></td>
                    <td align="left">
                        <input onchange="PreviewWindowsMedia()" id="wm_contextmenu" type="checkbox" name="wm_contextmenu" /></td>
			        <td><asp:label ID="lblPlayCount" runat="server" /></td>
			        <td><input onchange="PreviewWindowsMedia()" id="wm_playcount" style="WIDTH: 64px" type="text" size="5" name="wm_playcount" /></td>
			    </tr>
			    <tr>
			        <td>
			            <asp:label ID="lblEnabled" runat="server" />:</td>
			        <td>
			            <input onchange="PreviewWindowsMedia()" id="wm_enabled" type="checkbox" name="wm_enabled" /></td>
			        <td colspan="4"><asp:label ID="lblMode" runat="server" /><select onchange="PreviewWindowsMedia()" id="wm_uimode" name="wm_uimode">
					    <option selected="selected" value="full">Full</option>
					    <option value="invisible">invisible</option>
					    <option value="none">none</option>
					    <option value="mini">mini</option>
				    </select></td>
			    </tr>
			    <tr>
			        <td>
			           <asp:label ID="lblWindowless" runat="server" /></td>
			        <td colspan="3">
			            <input onchange="PreviewWindowsMedia()" id="wm_windowlessVideo" type="checkbox" name="wm_windowlessVideo" /></td>
			    </tr>
			</table>
			</td>
		</tr>
	</table> 
</div>