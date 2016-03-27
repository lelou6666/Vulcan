<%@ Control Language="VB" AutoEventWireup="false" CodeFile="flashparams.ascx.vb" Inherits="Multimedia_flashparams" %>
<!-- Quicktime player properties -->
<script type="text/javascript" language="JavaScript">
    var fl_cp = new ColorPicker('window');
    function pickColor( color ) { 
        document.getElementById( "fl_bgcolor" ).value = color;
        previewFlash();
    }
    fl_cp.writeDiv();
</script>

<div id="flash-panel"  style="display:none">
<script type="text/javascript" language="JavaScript" src="controls/media/flashparams.js"></script>
<script id="flashparams" type="text/javascript" language="javascript">
    var jsflash = new ektFlashPlayer();
    function previewFlash() {
        jsflash.preview();
    }
</script>
<table cellspacing="0" cellpadding="0" border="0">
    <tr>
       <td nowrap align="left">Menu:</td>
       <td nowrap align="left"><input onclick="previewFlash();" id="menu" type="checkbox" /></td>
        
       <td nowrap align="left">BGColor:</td>
       <td align="left"><input type="text" onchange="previewFlash();" id="fl_bgcolor" name="bgcolor" size="20" value="" readonly="readOnly" /> <a href="#" onclick="fl_cp.select(document.forms[0].fl_bgcolor,'fl_pick');return false;" name="fl_pick" id="fl_pick">Select</a></td>

       </tr>
       <tr>
       <td nowrap align="left">Align:</td>
       <td nowrap align="left"><select onchange="previewFlash();" id="align">
       <option id="align_l" value="l">Left</option>
       <option id="align_t" value="t">Top</option>
       <option id="align_r" value="r">Right</option>
       <option id="align_b" value="b">Bottom</option>
       </select></td>
       <td nowrap align="left">SAlign:</td>
       <td nowrap align="left"><select onchange="previewFlash();" id="salign">
       <option id="salign_l" value="l">Left Edge</option>
       <option id="salign_t" value="t">Top Edge</option>
       <option id="salign_r" value="r">Right Edge</option>
       <option id="salign_b" value="b">Bottom Edge</option>
       <option id="salign_tl" value="tl">Top Left</option>
       <option id="salign_tr" value="tr">Top Right</option>
       <option id="salign_bl" value="bl">Bottom Left</option>
       <option id="salign_br" value="br">Bottom Right</option>
       </select></td>
       </tr>
       <tr>
       <td nowrap align="left">Quality:</td>
       <td nowrap align="left"><select onchange="previewFlash();" id="quality">
       <option id="quality_Low" value="Low">Low</option>
       <option id="quality_Autolow" value="Autolow">Autolow</option>
       <option id="quality_Autohigh" value="Autohigh">Autohigh</option>
       <option id="quality_Medium" value="Medium">Medium</option>
       <option id="quality_High" value="High">High</option>
       <option id="quality_Best" value="Best">Best</option>
       </select></td>
       <td nowrap align="left">Scale:</td>
       <td nowrap align="left"><select onchange="previewFlash();" id="scale">
       <option id="scale_default" value="ShowAll">Default(Show all)</option>
       <option id="scale_NoBorder" value="NoBorder">No Border</option>
       <option id="scale_ExactFit" value="ExactFit">Exact Fit</option>
       </select></td>

       </tr>
       <tr>
       <td nowrap align="left">WMode:</td>
       <td nowrap align="left"><select onchange="previewFlash();" id="wmode">
       <option id="wmode_Window" value="Window">Window</option>
       <option id="wmode_Opaque" value="Opaque">Opaque</option>
       <option id="wmode_Transparent" value="Transparent">Transparent</option>
       </select></td>
       <td nowrap align="left">&nbsp;</td>
       <td>&nbsp;</td>
       </tr>

       </table>
</div>