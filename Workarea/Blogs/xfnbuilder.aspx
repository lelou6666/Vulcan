<%@ Page Language="VB" AutoEventWireup="false" CodeFile="xfnbuilder.aspx.vb" Inherits="blogs_xfnbuilder" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Edit Relationship</title>
<script type="text/javascript" >
<!--//--><![CDATA[//><!--
 
var g_fieldid = '<asp:Literal id="m_id" runat="server"/>';
var g_fieldname = '<asp:Literal id="m_field" runat="server"/>';
var g_relativeClassPath = '../csslib/';
g_relativeClassPath = g_relativeClassPath.toLowerCase();
UpdateWorkareaTitleToolbars();
 
function GetRelativeClassPath() {
    return(g_relativeClassPath);
}
 
function UpdateWorkareaTitleToolbars() {
    if (document.styleSheets.length > 0) {
        MakeClassPathRelative('*', 'button', 'backgroundImage', '../images/application/', g_relativeClassPath)
        MakeClassPathRelative('*', 'button-over', 'backgroundImage', '../images/application/', g_relativeClassPath)
        MakeClassPathRelative('*', 'button-selected', 'backgroundImage', '../images/application/', g_relativeClassPath)
        MakeClassPathRelative('*', 'button-selectedOver', 'backgroundImage', '../images/application/', g_relativeClassPath)
        MakeClassPathRelative('*', 'ektronToolbar', 'backgroundImage', '../images/application/', g_relativeClassPath)
        MakeClassPathRelative('*', 'ektronTitlebar', 'backgroundImage', '../images/application/', g_relativeClassPath)
    } else {
        setTimeout('UpdateWorkareaTitleToolbars()', 500);
    }
}
function ShowTransString(Text) {
var ObjId = "ektronTitlebar";
var ObjShow = document.getElementById('_' + ObjId);
var ObjHide = document.getElementById(ObjId);
if ((typeof ObjShow != "undefined") && (ObjShow != null)) {
ObjShow.innerHTML = Text;
ObjShow.style.display = "inline";
if ((typeof ObjHide != "undefined") && (ObjHide != null)) {
ObjHide.style.display = "none";
}
}
}function HideTransString() {
var ObjId = "ektronTitlebar";
var ObjShow = document.getElementById(ObjId);
var ObjHide = document.getElementById('_' + ObjId);
if ((typeof ObjShow != "undefined") && (ObjShow != null)) {
ObjShow.style.display = "inline";if ((typeof ObjHide != "undefined") && (ObjHide != null)) {
ObjHide.style.display = "none";
}
}
}
function GetCellObject(MyObj) {
var tmpName = "";
tmpName = MyObj.id;
if (tmpName.indexOf("link_") >= 0) {
tmpName = tmpName.replace("link_", "cell_");
}else if (tmpName.indexOf("cell_") >= 0) {
tmpName = tmpName;
}
else {
tmpName = tmpName.replace("image_", "image_cell_");
}
MyObj = document.getElementById(tmpName);
return (MyObj);
}
var g_OldBtnObject = null;
function ClearPrevBtn() {
if (g_OldBtnObject){
  RollOut(g_OldBtnObject);
  g_OldBtnObject = null;
}
}
function RollOver(MyObj) {
ClearPrevBtn()
g_OldBtnObject = MyObj;
var tmpClassExt = "";
MyObj = GetCellObject(MyObj);tmpClassExt = MyObj.className.substring(MyObj.className.lastIndexOf("-"));
if (tmpClassExt == "-selected") {
MyObj.className = "button-selectedOver";
}
else {
MyObj.className = "button-over";
}
}
function RollOut(MyObj) {
if (g_OldBtnObject == MyObj){
g_OldBtnObject = null;
}
var tmpClassExt = "";
MyObj = GetCellObject(MyObj);
tmpClassExt = MyObj.className.substring(MyObj.className.lastIndexOf("-"));
if (tmpClassExt == "-selectedOver") {
MyObj.className = "button-selected";
}else {MyObj.className = "button";
}
}
function SelectButton(MyObj) {
}
function UnSelectButtons() { 
var iLoop = 100; 
while (document.getElementById("image_cell_" + iLoop.toString()) != null) { 
document.getElementById("image_cell_" + iLoop.toString()).className = "button"; 
iLoop++; 
} 
} 
function ReadValues() {
    var results = document.getElementById('xfnResult');
    var inputs = window.opener.arrRollRel[g_fieldid]
    results.childNodes[0].nodeValue = inputs;
    
    ShowValue(inputs, "me");
    if (!meChecked()) {
        ShowValue(inputs, "friendship-contact");
        ShowValue(inputs, "friendship-acquaintance");
        ShowValue(inputs, "friendship-friend");
        ShowValue(inputs, "friendship-none");
        ShowValue(inputs, "met");
        ShowValue(inputs, "co-worker");
        ShowValue(inputs, "colleague");
        ShowValue(inputs, "co-resident");
        ShowValue(inputs, "neighbor");
        ShowValue(inputs, "geographical-none");
        ShowValue(inputs, "family-child");
        ShowValue(inputs, "family-parent");
        ShowValue(inputs, "family-sibling");
        ShowValue(inputs, "family-spouse");
        ShowValue(inputs, "family-kin");
        ShowValue(inputs, "family-none");
        ShowValue(inputs, "muse");
        ShowValue(inputs, "crush");
        ShowValue(inputs, "date");
        ShowValue(inputs, "sweetheart");
    } else {
        upit();
    }
}
function ShowValue(xfntxt, searchtxt) {
    var elem = searchtxt;
    xfntxt = ' ' + xfntxt + ' ';
    searchtxt = ' ' + searchtxt + ' ';
    if (searchtxt.indexOf('-none') > -1) {
        // nothing
    } else if (searchtxt.indexOf('-') > -1) {
        searchtxt = searchtxt.substring((searchtxt.indexOf('-') + 1));
        if (xfntxt.indexOf(searchtxt) > -1) { document.getElementById(elem).checked = true; }   
    } else {    
        if (xfntxt.indexOf(searchtxt) > -1) { document.getElementById(elem).checked = true; }   
    }
}
//--><!]]>
</script>
</head>
<body>
    <form id="form1" runat="server" onreset="resetstuff();">
    <script type="text/javascript">
        //<![CDATA[
		    function GetElementsWithClassName(elementName, className) {
		       var allElements = document.getElementsByTagName(elementName);
		       var elemColl = new Array();
		       for (i = 0; i < allElements.length; i++) {
		           if (allElements[i].className == className) {
		               elemColl[elemColl.length] = allElements[i];
		           }
		       }
		       return elemColl;
		    }
    		
		    function meChecked()
		    {
		      var undefined;
		      var eMe = document.getElementById('me');
		      if (eMe == undefined) return false;
		      else return eMe.checked;
		    }
    		
		    function upit() {
		       var isMe = meChecked(); //document.getElementById('me').checked;
		       var inputColl = GetElementsWithClassName('input', 'valinp');
		       var results = document.getElementById('xfnResult');
		       var inputs = '';
		       for (i = 0; i < inputColl.length; i++) {
		           inputColl[i].disabled = isMe;
		           inputColl[i].parentNode.className = isMe ? 'disabled' : '';
		           if (!isMe && inputColl[i].checked && inputColl[i].value != '') {
					    inputs += inputColl[i].value + ' ';
		                }
		           }
		       inputs = inputs.substr(0,inputs.length - 1);
		       if (isMe) inputs='me';
		            results.childNodes[0].nodeValue = inputs;
		    }
    		
		    function blurry() {
		       if (!document.getElementById) return;
    		
		       var aInputs = document.getElementsByTagName('input');
    		
		       for (var i = 0; i < aInputs.length; i++) {		
		           aInputs[i].onclick = aInputs[i].onkeyup = upit;
		       }
		    }
		    
		    function WriteBack() {
		        var results = document.getElementById('xfnResult');
		        var inputs = results.childNodes[0].nodeValue;
		        window.opener.arrRollRel[g_fieldid] = inputs;
		        top.opener.document.getElementById(g_fieldname).value = inputs;
		        self.close();
		    }
    		
		    function resetstuff() {
		     if (meChecked()) document.getElementById('me').checked=''; 
		     upit();
		     document.getElementById('xfnResult').childNodes[0].nodeValue = '&nbsp;';
		    }
		    window.onload = blurry;
        //]]>
        </script>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>

<div id="dhtmltooltip"></div>

<div class="ektronPageContainer ektronPageInfo">
<table class="ektronGrid">
            <tr>
              <th class="label">
                <asp:label id="lblCurrentRelationship" runat="server" Text="Current Relationship:" />
              </th>
              <td><!-- this div stores the current relationship status -->
                <div id="xfnResult">
                    &nbsp;
                </div>
              </td>
            </tr>
            
           <tr>              
              <th class="label">
                <asp:label id="lblURL" runat="server" Text="URL:" />
              </th>
              <td>
                <label for="me"><input type="checkbox" name="identity" value="me" id="me" />&nbsp;another web address of mine</label>
              </td>
            </tr>
            <tr>
              <th class="label">
                <asp:label id="lblFriendship" runat="server" Text="Friendship:" />
              </th>
              <td>
                <label for="friendship-contact"><input class="valinp" type="radio" name="friendship" value="contact" id="friendship-contact" />&nbsp;contact </label><label for="friendship-acquaintance"><input class="valinp" type="radio" name="friendship" value="acquaintance" id="friendship-acquaintance" />&nbsp;acquaintance </label> <label for="friendship-friend"><input class="valinp" type="radio" name="friendship" value="friend" id="friendship-friend" />&nbsp;friend </label> <label for="friendship-none"><input class="valinp" type="radio" name="friendship" value="" id="friendship-none" />&nbsp;none</label>
              </td>
            </tr>
            <tr>
              <th class="label">
                <asp:label id="lblPhysical" runat="server" Text="Physical:" />
              </th>
              <td>
                <label for="met"><input class="valinp" type="checkbox" name="physical" value="met" id="met" />&nbsp;met</label>
              </td>
            </tr>
            <tr>
              <th class="label">
                <asp:label id="lblProfessional" runat="server" Text="Professional:" />
              </th>
              <td>
                <label for="co-worker"><input class="valinp" type="checkbox" name="professional" value="co-worker" id="co-worker" />&nbsp;co-worker </label> <label for="colleague"><input class="valinp" type="checkbox" name="professional" value="colleague" id="colleague" />&nbsp;colleague</label>
              </td>
            </tr>
            <tr>
              <th class="label">
                <asp:label id="lblGeographical" runat="server" Text="Geographical:" />
              </th>
              <td>
                <label for="co-resident"><input class="valinp" type="radio" name="geographical" value="co-resident" id="co-resident" />&nbsp;co-resident </label> <label for="neighbor"><input class="valinp" type="radio" name="geographical" value="neighbor" id="neighbor" />&nbsp;neighbor </label> <label for="geographical-none"><input class="valinp" type="radio" name="geographical" value="" id="geographical-none" />&nbsp;none</label>
              </td>
            </tr>
            <tr>
              <th class="label">
                <asp:label id="lblFamily" runat="server" Text="Family:" />
              </th>
              <td>
                <label for="family-child"><input class="valinp" type="radio" name="family" value="child" id="family-child" />&nbsp;child </label> <label for="family-parent"><input class="valinp" type="radio" name="family" value="parent" id="family-parent" />&nbsp;parent </label> <label for="family-sibling"><input class="valinp" type="radio" name="family" value="sibling" id="family-sibling" />&nbsp;sibling </label> <label for="family-spouse"><input class="valinp" type="radio" name="family" value="spouse" id="family-spouse" />&nbsp;spouse </label> <label for="family-kin"><input class="valinp" type="radio" name="family" value="kin" id="family-kin" />&nbsp;kin </label>
                <label for="family-none"><input class="valinp" type="radio" name="family" value="" id="family-none" />&nbsp;none</label>
              </td>
            </tr>
            <tr>
              <th class="label">
                <asp:label id="lblRomantic" runat="server" Text="Romantic" />
              </th>
              <td>
                <label for="muse"><input class="valinp" type="checkbox" name="romantic" value="muse" id="muse" />&nbsp;muse </label> <label for="crush"><input class="valinp" type="checkbox" name="romantic" value="crush" id="crush" />&nbsp;crush </label> <label for="date"><input class="valinp" type="checkbox" name="romantic" value="date" id="date" />&nbsp;date </label> <label for="sweetheart"><input class="valinp" type="checkbox" name="romantic" value="sweetheart" id="sweetheart" />&nbsp;sweetheart</label>
              </td>
            </tr>
</table>

</div>

<script  type="text/javascript">
    ReadValues();
</script>
</form>
</body>
</html>
