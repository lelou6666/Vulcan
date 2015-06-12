<%@ Control Language="VB" AutoEventWireup="false" CodeFile="realplayerparams.ascx.vb" Inherits="Multimedia_realplayerparams" %>
<script type="text/javascript" language="JavaScript" src="controls/media/RealPlayerparams.js"></script>
<script id="realplayerparams" type="text/javascript" language="javascript">
    var jsrealplayer = new ektRealPlayer();
    function PreviewRealPlayer()
    {
        jsrealplayer.preview();
    }
</script>
<!-- Real Player properties -->
<div id="realplayer-panel" style="display:none">
<input name="rp_type" type="hidden" id="rp_type" value="" />
<table width="100%" cellspacing="0" cellpadding="0" border="0">
	<tr>
	    <td colspan="2">
	        <table>
	            <tr>
	                <td>Center:<input onchange="PreviewRealPlayer()" type="checkbox" id="rp_center" name="rp_center" /></td>
	                <td>NoLogo:<input onchange="PreviewRealPlayer()" type="checkbox" name="rp_nologo" id="rp_nologo" /></td>
	            </tr>
	            <tr>
	                <td>Maintain Aspect:<input onchange="PreviewRealPlayer()" type="checkbox" id="rp_aspect" name="rp_aspect" /></td>
	                <td>&nbsp;</td>	            
	            </tr>
	            <tr>
	                <td>Number of Loops:<input onchange="PreviewRealPlayer()" type="text" id="rp_numloops" name="rp_numloops" style="width: 50px" /></td>
	                <td>Controls:
	                <select onchange="PreviewRealPlayer()" id="rp_controls" name="rp_controls">
	                    <option id="rp_controls_all" value="all">All (Default)</option>
	                    <option id="rp_controls_positionslider" value="PositionSlider">Position Slider</option>
	                    <option id="rp_controls_FFCtrl" value="FFCtrl">Fast Forward Button</option>
	                    <option id="rp_controls_imagewindow" value="ImageWindow">Image Window</option>
	                    <option id="rp_controls_infopanel" value="InfoPanel">Information Panel</option>
	                    <option id="rp_controls_MuteCtrl" value="MuteCtrl">Mute Button</option>
	                    <option id="rp_controls_PauseButton" value="PauseButton">Pause Button</option>
	                    <option id="rp_controls_PlayOnlyButton" value="PlayOnlyButton">Play Button</option>
	                    <option id="rp_controls_RWCtrl" value="RWCtrl">Rewind Button</option>
	                    <option id="rp_controls_StopButton" value="StopButton">Stop Button</option>
	                    <option id="rp_controls_VolumeSlider" value="VolumeSlider">Volume Slider</option>
	                    <option id="rp_controls_HomeCtrl" value="HomeCtrl">www.real.com Home Button</option>
	                    <option id="rp_controls_MuteVolume" value="MuteVolume">Mute / Volume Bar</option>
	                    <option id="rp_controls_StatusBar" value="StatusBar">Status Bar</option>
	                    <option id="rp_controls_StatusField" value="StatusField">Status Field</option>
	                    <option id="rp_controls_PositionField" value="PositionField">PositionField</option>	                    
	                </select>
	                </td>
	            </tr>
	        </table>
	    </td>
	</tr>							
</table>
</div> 
<!-- /Real Player Properties -->