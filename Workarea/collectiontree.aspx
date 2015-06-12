<%@ Import Namespace="Ektron.Cms.UI.CommonUI" %>
<%@ Import Namespace="Ektron.Cms.Site" %>
<%@ Import Namespace="Ektron.Cms.User" %>
<%@ Import Namespace="Ektron.Cms.Content" %>
<%@ Import Namespace="Ektron.Cms" %>
<%@ Import Namespace="Ektron.Cms.Common" %>
<%@ Import Namespace="Ektron.Cms.API" %>

<script language="vb" runat="server">
Dim gtMsgObj, gtMess,CollectionID, msgs, currentUserID, ErrorString, PageTitle, cTmp, m_Site, gtObj As Object
Dim gtNav, CollectionTitle, lLoop, siteObj, cLinkArray, fLinkArray, FolderName, gtNavs As Object
Dim cPerms as Hashtable
Dim AppUI As New ApplicationAPI
dim MsgHelper as EkMessageHelper
Dim AppPath, AppImgPath, sitePath as String
Dim ContentLanguage,EnableMultilingual As Object
Const ALL_CONTENT_LANGUAGES as Integer =-1
Const CONTENT_LANGUAGES_UNDEFINED as Integer =0
</script>

<%
CurrentUserId=AppUI.UserId
AppImgPath=AppUI.AppImgPath
AppPath=AppUI.AppPath
sitePath=AppUI.SitePath
MsgHelper=AppUI.EkMsgRef
If (Request.QueryString("LangType")<>"") Then
		ContentLanguage=Request.QueryString("LangType")
		AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage)
	else
		if CStr(AppUI.GetCookieValue("LastValidLanguageID")) <> ""  then
			ContentLanguage = AppUI.GetCookieValue("LastValidLanguageID")
		end if
End If
AppUI.ContentLanguage=ContentLanguage

dim folderId, nID, cFolders, folder, mpID, maID,selTaxID As Object
cFolders = Nothing
folderId = Request.QueryString("folderid")
mpID = Request.QueryString("parentid")
maID = Request.QueryString("ancestorid")
if(Request.QueryString("SelTaxonomyID") <> "") then
selTaxID = Request.QueryString("SelTaxonomyID")
end if

IF Ektron.cms.commonApi.GetEcmCookie().HasKeys THEN
 currentUserID = Ektron.cms.commonApi.GetEcmCookie()("user_id")
 m_Site = Ektron.cms.commonApi.GetEcmCookie()("site_id")
else
 currentUserID = 0
end if
%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <meta http-equiv="pragma" content="no-cache"/>
    <title>
        <%=(AppUI.AppName & " " & "Collections")%>
    </title>

    <script type="text/javascript" language="JavaScript" src="java/toolbar_roll.js"></script>

    <link rel="stylesheet" type="text/css" href="csslib/ektron.workarea.css"/>
    <link rel="stylesheet" type="text/css" href="csslib/ektron.fixedPositionToolbar.css"/>
    <script  src="java/ektron.js" type="text/javascript"></script>
    <script  src="java/ektron.workarea.js" type="text/javascript"></script>

    <script type="text/javascript">
<!--

/***********************************************
* Contractible Headers script- © Dynamic Drive (www.dynamicdrive.com)
* This notice must stay intact for legal use. Last updated Oct 21st, 2003.
* Visit http://www.dynamicdrive.com/ for full source code
***********************************************/

var enablepersist="off"; //Enable saving state of content structure using session cookies? (on/off)
var collapseprevious="no"; //Collapse previously open content when opening present? (yes/no)


if (document.getElementById){
document.write('<style type="text/css">')
document.write('.switchcontent{display:none;}')
document.write('</style>')
}

function getElementbyClass(classname){
ccollect=new Array()
var inc=0
var alltags=document.all? document.all : document.getElementsByTagName("*")
for (i=0; i<alltags.length; i++){
if (alltags[i].className==classname)
ccollect[inc++]=alltags[i]
}
}

function contractcontent(omit){
var inc=0
while (ccollect[inc]){
if (ccollect[inc].id!=omit)
ccollect[inc].style.display="none"
inc++
}
}

function expandcontent(cid){
if (typeof ccollect!="undefined"){
if (collapseprevious=="yes")
contractcontent(cid)
document.getElementById(cid).style.display=(document.getElementById(cid).style.display!="block")? "block" : "none"
}
}

function revivecontent(){
contractcontent("omitnothing")
selectedItem=getselectedItem()
selectedComponents=selectedItem.split("|")
for (i=0; i<selectedComponents.length-1; i++)
document.getElementById(selectedComponents[i]).style.display="block"
}

function get_cookie(Name) { 
var search = Name + "="
var returnvalue = "";
if (document.cookie.length > 0) {
offset = document.cookie.indexOf(search)
if (offset != -1) { 
offset += search.length
end = document.cookie.indexOf(";", offset);
if (end == -1) end = document.cookie.length;
returnvalue=unescape(document.cookie.substring(offset, end))
}
}
return returnvalue;
}

function getselectedItem(){
if (get_cookie(window.location.pathname) != ""){
selectedItem=get_cookie(window.location.pathname)
return selectedItem
}
else
return ""
}

function saveswitchstate(){
var inc=0, selectedItem=""
while (ccollect[inc]){
if (ccollect[inc].style.display=="block")
selectedItem+=ccollect[inc].id+"|"
inc++
}

document.cookie=window.location.pathname+"="+selectedItem
}

function do_onload(){
getElementbyClass("switchcontent")
if (enablepersist=="on" && typeof ccollect!="undefined")
revivecontent()
}


if (window.addEventListener)
window.addEventListener("load", do_onload, false)
else if (window.attachEvent)
window.attachEvent("onload", do_onload)
else if (document.getElementById)
window.onload=do_onload

if (enablepersist=="on" && document.getElementById)
window.onunload=saveswitchstate
-->
    </script>

    <script type="text/javascript" language="JavaScript">
<!--

	function ConfirmNavDelete() {
		return confirm("<%=(MsgHelper.GetMessage("js: confirm collection deletion msg"))%>");
	}	
	
	function SubmitForm(FormName, Validate) {
		if (Validate.length > 0) {
			if (eval(Validate)) {
				document.forms[FormName].submit();
				return false;
			}
			else {
				return false;
			}
		}
		else {
			document.forms[FormName].submit();
			return false;
		}
	}

	function Move(sDir, objList, objOrder) {
		if (objList.selectedIndex != null) {
			nSelIndex = objList.selectedIndex;
			sSelValue = objList[nSelIndex].value;
			sSelText = objList[nSelIndex].text;
			if (sDir == "up" && nSelIndex > 0) {
				sSwitchValue = objList[nSelIndex -1].value;
				sSwitchText = objList[nSelIndex - 1].text;
				objList[nSelIndex].value = sSwitchValue;
				objList[nSelIndex].text = sSwitchText;
				objList[nSelIndex - 1].value = sSelValue;
				objList[nSelIndex - 1].text = sSelText;
				objList[nSelIndex - 1].selected = true;
			}
			else if (sDir == "dn" && nSelIndex < (objList.length - 1)) {
				sSwitchValue = objList[nSelIndex + 1].value;
				sSwitchText = objList[nSelIndex +  1].text;
				objList[nSelIndex].value = sSwitchValue;
				objList[nSelIndex].text = sSwitchText;
				objList[nSelIndex + 1].value = sSelValue;
				objList[nSelIndex + 1].text = sSelText;
				objList[nSelIndex + 1].selected = true;
			}
		}
		objOrder.value = "";
		for (i = 0; i < objList.length; i++) {
			objOrder.value = objOrder.value + objList[i].value;
			if (i < (objList.length - 1)) {
				objOrder.value = objOrder.value + ",";
			}
		}
	}
	
	function SelectAll() {
		var lLoop = 0;
		for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
			if ((document.forms.selections[lLoop].type.toLowerCase() == "checkbox")
					&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_check") != -1)) {
				document.forms.selections[lLoop].checked = true;
			}
		}
		var cArray = Collections.split(",");
		for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
			if ((document.forms.selections[lLoop].type.toLowerCase() == "hidden") 
							&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_hidden") != -1)) {
				var cIndex = document.forms.selections[lLoop].name.toLowerCase().replace("frm_hidden", "");
				document.forms.selections[lLoop].value = cArray[cIndex];
			}
		}
	}

	function ClearAll() {
		var lLoop = 0;
		for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
			if ((document.forms.selections[lLoop].type.toLowerCase() == "checkbox")
					&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_check") != -1)) {
				document.forms.selections[lLoop].checked = false;
			}
		}
		for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
			if ((document.forms.selections[lLoop].type.toLowerCase() == "hidden") 
							&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_hidden") != -1)) {
				document.forms.selections[lLoop].value = 0;
			}
		}
	}

	function GetIDs() {
		var lLoop = 0;
		document.forms.selections.frm_content_ids.value = "";
		document.forms.selections.frm_folder_ids.value = "";
		for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
			if ((document.forms.selections[lLoop].type.toLowerCase() == "hidden")
					&& (document.forms.selections[lLoop].name.toLowerCase().search("frm_hidden") != -1)
					&& (document.forms.selections[lLoop].value != 0)) {
				document.forms.selections.frm_content_ids.value = document.forms.selections.frm_content_ids.value + "," + document.forms.selections[lLoop].value;
				document.forms.selections.frm_folder_ids.value = document.forms.selections.frm_folder_ids.value + "," + document.forms.selections[lLoop].fid;
			}
		}
		document.forms.selections.frm_content_ids.value = document.forms.selections.frm_content_ids.value.substring(1, document.forms.selections.frm_content_ids.value.length);
		document.forms.selections.frm_folder_ids.value = document.forms.selections.frm_folder_ids.value.substring(1, document.forms.selections.frm_folder_ids.value.length);
		if (document.forms.selections.frm_content_ids.value.length == 0) {
			alert("<%=(MsgHelper.GetMessage("js:no items selected"))%>");
			return false;
		}
		return true;
	}

	function Trim (string) {
		if (string.length > 0) {
			string = RemoveLeadingSpaces (string);
		}
		if (string.length > 0) {
			string = RemoveTrailingSpaces(string);
		}
		return string;
	}
	
	function RemoveLeadingSpaces(string) {
		while(string.substring(0, 1) == " ") {
			string = string.substring(1, string.length);
		}
		return string;
	}

	function RemoveTrailingSpaces(string) {
		while(string.substring((string.length - 1), string.length) == " ") {
			string = string.substring(0, (string.length - 1));
		}
		return string;
	}

	function CheckKeyValue(item, keys) {
		var keyArray = keys.split(",");
		for (var i = 0; i < keyArray.length; i++) {
			if ((document.layers) || ((!document.all) && (document.getElementById))) {
				if (item.which == keyArray[i]) {
					return false;
				}
			}
			else {
				if (event.keyCode == keyArray[i]) {
					return false;
				}
			}
		}
	}

	function VerifyCollectionForm () {
		regexp1 = /"/gi; //Leave this comment "
		document.forms.nav.frm_nav_title.value = Trim(document.forms.nav.frm_nav_title.value);
		document.forms.nav.frm_nav_title.value = document.forms.nav.frm_nav_title.value.replace(regexp1, "'");

		document.forms.nav.frm_nav_description.value = Trim(document.forms.nav.frm_nav_description.value);
		document.forms.nav.frm_nav_description.value = document.forms.nav.frm_nav_description.value.replace(regexp1, "'");
		if (document.forms.nav.frm_nav_title.value == "")
		{
			alert ("<%=(MsgHelper.GetMessage("js: collection title required msg"))%>");
			document.forms.nav.frm_nav_title.focus();
			return false;
		}
		return true;
	}
	
//-->
    </script>

    <!--#include file="common/stylesheetsetup.inc" -->
</head>
<body bottommargin="0" leftmargin="5" rightmargin="0" topmargin="0">
    <%
	dim OrderBy,subfolderid, locID,backID,cRecursive,rec,CanCreateContent As Object
	dim AddType, MenuTitle As Object
	Dim CachedId as String=""
	Dim canTraverse as Boolean = True
	rec = Nothing
	backID = Nothing
	
	AddType = LCase(Request.QueryString("addto"))
	if (AddType = "") then
		AddType = "collection"
	end if
	nId =  Request.QueryString("nid")
	subfolderid = request.QueryString("subfolderid")
	If(Request.QueryString("cacheidentifier") IsNot Nothing AndAlso Request.QueryString("cacheidentifier")<>"") Then
	    CachedId="&cacheidentifier=" & Request.QueryString("cacheidentifier")
	End if
	if (len(Cstr(subfolderid))) then
		locID = subfolderid
	else
		locID = folderId
	end if
		OrderBy = "title"
	gtObj = AppUI.EkContentRef
	SiteObj = AppUI.EkSiteRef
	cPerms = SiteObj.GetPermissions(locID, 0, "folder")
	canTraverse = cPerms("TransverseFolder")
	if( len(ErrorString)) then
		CanCreateContent = false
	else
		CanCreateContent = cPerms("Add")
	end if
	SiteObj = nothing
	cPerms = nothing
	if ((AddType = "menu") or (AddType = "submenu")) then
		cRecursive = gtObj.GetMenuByID(nId, 0)
		if (ErrorString = "") then
			if (cRecursive.Count) then
				MenuTitle = cRecursive("MenuTitle")
				rec = cRecursive("Recursive")
			end if
		end if
	else
	''ap
		cRecursive = gtObj.GetEcmCollectionByID(nId, 0, 0, ErrorString, true, false, true)
		if (ErrorString = "") then
			if (cRecursive.Count) then
				CollectionTitle = cRecursive("CollectionTitle")
				rec = cRecursive("Recursive")
			end if
		end if
	end if
    If Not canTraverse Then 
	    rec = False
	End If
	if (rec AND (len(ErrorString) = 0)) then
		cTmp = New Collection
		cTmp.Add(Clng(locID), "ParentID")
		cTmp.Add("name", "OrderBy")
		cFolders = gtObj.GetAllViewableChildFoldersv2_0(cTmp)
	end if
	if (len(ErrorString) = 0) then
		gtNavs = gtObj.GetFolderInfov2_0(locID)
		if (len(ErrorString) = 0) then
			FolderName = gtNavs("FolderName")
			backId = gtNavs("ParentID") 
			gtNavs = nothing
	'		gtNavs = gtObj.GetAllContentNotInEcmCollection(nID, Clng(locID), OrderBy, ErrorString)
		end if
	end if
	if (ErrorString <> "") then
    %>
    <div class="ektronToolbarHeader">
        <table width="100%" class="ektronForm">
            <tr>
                <td class="ektronTitlebar">
                    Select Folder - Error</td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="titlebar-error">
                    <%=(ErrorString)%>
                </td>
            </tr>
        </table>
    </div>
    <%
	else
    %>
    <form name="selections" method="post" action="collectionaction.aspx?folderid=<%=(folderId)%>&nid=<%=(nId)%>&parentid=<%=mpID%>&ancestorid=<%=maID%>&SelTaxonomyID=<%=selTaxID%>">
        <div class="ektronToolbarHeader">
            <table width="100%" >
                <tr>
                    <td class="ektronTitlebar">
                        Select Folder</td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
            <table>
                <tr>
                    <td class="label">
                        Selected
                        <%=(MsgHelper.GetMessage("foldername label"))%>
                    </td>
                    <td>
                        <%=("""" & FolderName & """")%>
                    </td>
                </tr>
            </table>
        </div>
        <table width="100%">
            <% if (rec) then %>
            <%''if (locID <> folderId) then
				  if (locID <> 0 ) then %>
            <tr>
                <td>
                    <a href="collectiontree.aspx?nId=<%=nID%>&folderid=<%=(folderId)%>&subfolderid=<%=(backID)%>&addto=<%=(AddType)%>&parentid=<%=mpID%>&ancestorid=<%=maID%>&SelTaxonomyID=<%=selTaxID%>"
                        title="<%=(MsgHelper.GetMessage("alt: generic previous dir text"))%>">
                        <img src="<%=(AppImgPath & "folderbackup_1.gif")%>" title="<%=(MsgHelper.GetMessage("alt: generic previous dir text"))%>"
                            alt="<%=(MsgHelper.GetMessage("alt: generic previous dir text"))%>" align="absbottom"/>..</a></td>
                <td>
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <% end if %>
            <% for each folder in cFolders
				if folder("FolderType") <= 1 then
            %>
            <tr>
                <td nowrap="true">
                    <a href="collectiontree.aspx?nId=<%=(nID)%>&folderid=<%=(folderId)%>&subfolderid=<%=(folder("ID"))%>&addto=<%=(AddType)%>&parentid=<%=mpID%>&ancestorid=<%=maID%>&SelTaxonomyID=<%=selTaxID%>"
                        title="<%=(MsgHelper.GetMessage("alt: generic view folder content text"))%>">
                        <img src="<%=(AppImgPath)%><%= IIF(folder("FolderType") = 1, "folders/blogfolderopen.gif", "folderclosed_1.gif")%>"
                            title="<%=(MsgHelper.GetMessage("alt: generic view folder content text"))%>"
                            alt="<%=(MsgHelper.GetMessage("alt: generic view folder content text"))%>" align="absbottom"/><%=(folder("Name"))%></a></td>
                <td>
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <%End If
				next%>
            <%end if%>
            <%
			lLoop = 0
			cLinkArray = ""
			fLinkArray = ""			
            %>
            <tr>
                <td align="center" colspan="3">
                    <hr/>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="3">
                    First Pick the folder</td>
            </tr>
            <tr>
                <% if (AddType = "submenu") then %>
                <td align="center" colspan="3">
                    <input name="next" type="button" value="Next..." onclick="javascript:location.href='collections.aspx?action=AddSubMenu&LangType=<%=ContentLanguage%>&folderid=<%=(locID)%>&nId=<%=(nId)%>&parentid=<%=mpID%>&ancestorid=<%=maID%>&SelTaxonomyID=<%=selTaxID%>'"></td>
                <% else %>
                <td align="center" colspan="3">
                    <input name="next" type="button" value="Next..." onclick="javascript:location.href='editarea.aspx?LangType=<%=ContentLanguage%>&type=add&id=<%=(locID)%>&SelTaxonomyID=<%=selTaxID%>&mycollection=<%=(nId)%>&addto=<%=(AddType)%><%=CachedId%>'"></td>
                <% end if %>
            </tr>
        </table>

        <script type="text/javascript" language="javascript">
			Collections = "<%=(cLinkArray)%>";
			Folders = "<%=(fLinkArray)%>";
        </script>

        <input type="hidden" name="frm_content_ids" value=""/>
        <input type="hidden" name="frm_folder_ids" value=""/>
        <input type="hidden" name="CollectionID" value="<%=(nId)%>"/>
    </form>
    <%end if %>
    <%
gtMsgObj = Nothing
gtMess = Nothing
    %>
</body>
</html>
