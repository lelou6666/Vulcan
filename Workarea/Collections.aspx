<%@ Page Language="vb" AutoEventWireup="false" Inherits="Collections" CodeFile="Collections.aspx.vb" %>
<%@ Import Namespace="Ektron.Cms.UI.CommonUI" %>
<%@ Import Namespace="Ektron.Cms.Site" %>
<%@ Import Namespace="Ektron.Cms.User" %>
<%@ Import Namespace="Ektron.Cms.Content" %>
<%@ Import Namespace="Ektron.Cms" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="Ektron.Cms.Common" %>
<%@ Import Namespace="Ektron.Cms.Common.EkConstants" %>
<%@ Register Src="pagebuilder/foldertree.ascx" TagName="FolderTree" TagPrefix="CMS" %>

<!-- #include file="cmsmenuapi.aspx" -->
<script language="vb" runat="server" >
Function IsFolderAdmin(byval folderId as Long) as Boolean
    return m_refApi.IsARoleMemberForFolder_FolderUserAdmin(folderId)
End Function

function IsCollectionApprover() as boolean
    if (m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AminCollectionMenu)) then
        return true
    end if
    if (m_refApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CollectionApprovers)) then
        return true
    end if
    return false
end function

function getContentTypeIconAspx(byVal ContentTypeID As Integer, ByRef colContent As Collection) As String
	if ContentTypeID = 2 then
		getContentTypeIconAspx = formsIcon & "&nbsp;"
	elseif ContentTypeID = 1 then
	    if (Ektron.Cms.Common.EkFunctions.DoesKeyExist(colContent, "ContentSubType") andalso colContent("ContentSubType") = 1) then
		    getContentTypeIconAspx = pageIcon & "&nbsp;"
	    else
		    getContentTypeIconAspx = contentIcon & "&nbsp;"
	    end if
	elseif IsAssetContentType(ContentTypeID) then
	    if (colContent("ImageUrl").ToString <> "") then
		    getContentTypeIconAspx = "<img src=""" & colContent("ImageUrl") & """  alt=""Content"">&nbsp;"
		else
		    getContentTypeIconAspx = libraryIcon & "&nbsp;"
		end if
	else
		getContentTypeIconAspx = contentIcon & "&nbsp;"
	end if
end function

function getNewContentTypeIcon(byVal ContentTypeID As Integer, ByVal ContentText As String) As String
	if ContentTypeID = 2 then
		getNewContentTypeIcon = formsIcon & "&nbsp;"
	elseif ContentTypeID = 1 then
		getNewContentTypeIcon = contentIcon & "&nbsp;"
	elseif IsAssetContentType(ContentTypeID) then
		If (ContentText <> "") Then
            getNewContentTypeIcon= ContentText & "&nbsp;"
        Else
			getNewContentTypeIcon = contentIcon & "&nbsp;"
        End If
	else
		getNewContentTypeIcon = "<img src=""" & AppPath  & "images/application/icon_poll_edit.gif"" alt=""Content"">&nbsp;"
	end if
end function

function getTypeIcon(byVal ItemType As Integer) As String
	if (ItemType = 1) then
		getTypeIcon = contentIcon & "&nbsp;"
	elseif (ItemType = 2) then
		getTypeIcon = libraryIcon & "&nbsp;"
	elseif ((ItemType = 3) or (ItemType = 4)) then
		getTypeIcon = menuIcon & "&nbsp;"
	elseif (ItemType = 5) then
		getTypeIcon = linkIcon & "&nbsp;"
	else
		getTypeIcon = ""
	end if
end function
Function LangDD(showAllOpt As Object, bgColor As String) As String
	LangDD = ShowAllActiveLanguage(showAllOpt, bgColor, "javascript:SelLanguage(this.value);", "")
End Function

Function ViewLangsForMenuID(fnMenuID As Object, fnBGColor As String, showTranslated As Object, showAllOpt As Object, onChangeEv As String) As String
	Dim TransCol, outDD, Col, frmName As Object

	Dim contObj As Object
	contObj = AppUI.EkContentRef()

	If (CBool(showTranslated)) Then
		TransCol = contObj.GetTranslatedLangsForMenuID( fnMenuID, ErrorString)
		frmName = "frm_translated"
	Else
		TransCol = contObj.GetNonTranslatedLangsForMenuID( fnMenuID, ErrorString)
		frmName = "frm_nontranslated"
	End If

	outDD = "<select id=""" & frmName & """ name=""" & frmName & """ OnChange=""" & onChangeEv & """>" & vbcrlf

	If (CBool(showAllOpt)) Then
		If Cstr(ContentLanguage)="-1" Then
			outDD = outDD & "<option value=""-1"" selected>All</option>"
		Else
			outDD = outDD & "<option value=""-1"">All</option>"
		End If
	Else
		outDD = outDD & "<option value=""0"">-select language-</option>"
	End If

	If ((TransCol.Count > 0) And (EnableMultilingual = "1")) Then
		For each Col in TransCol
			If Cstr(ContentLanguage)=Cstr(Col("LanguageID")) Then
				outDD = outDD & "<option value=" & Col("LanguageID") & " selected>" &  Col("LanguageName") & "</option>"
			Else
				outDD = outDD & "<option value=" & Col("LanguageID") & ">" &  Col("LanguageName") & "</option>"
			End If
		Next
	Else
		ViewLangsForMenuID = ""
		Exit Function
	End If

	outDD = outDD & "</select>"

	ViewLangsForMenuID = outDD

End Function

Function GetTitlesFromFolderIds(associatedFolderIdList As String) As String
	Dim ekcontentObj As EkContent = AppUI.EkContentRef
	Dim result As String = String.Empty
	Dim listArray As Object
	Dim index As Integer

	listArray = associatedFolderIdList.Split(";")
	For index = 0 to listArray.Length - 1
		If (IsNumeric(listArray(index))) Then
			If (result.Length) Then
				result += ";"
			End If
			result += ekcontentObj.GetFolderPath(CType(listArray(index), Long))
		End If
	Next
	GetTitlesFromFolderIds = result
	ekcontentObj = Nothing
End Function
</script>
<%
If (Request.QueryString("LangType")<>"") Then
		ContentLanguage=Request.QueryString("LangType")
		AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage)
	else
		if CStr(AppUI.GetCookieValue("LastValidLanguageID")) <> ""  then
			ContentLanguage = AppUI.GetCookieValue("LastValidLanguageID")
		end if
End If
MsgHelper=AppUI.EkMsgRef
AppUI.ContentLanguage=ContentLanguage
EnableMultilingual=AppUI.EnableMultilingual
CurrentUserId=AppUI.UserId
AppPath=AppUI.AppPath
sitePath=AppUI.SitePath

gtMsgObj = AppUI.EkSiteRef

if (ErrorString <> "") then
	Response.Write(ErrorString)
end if

action = Request.QueryString("action")
folderId = Request.QueryString("folderid")
NoWorkAreaAttribute = ""
if (Request.QueryString("noworkarea") = "1") then
	NoWorkAreaAttribute = "&noworkarea=1"
end if
Dim showQDContentOnly As Object
showQDContentOnly = false

contentIcon = "<img src=""" & AppPath  & "images/UI/Icons/contentHtml.png"" alt=""Content"">" '-HC-
pageIcon = "<img src=""" & AppPath  & "images/UI/Icons/layout.png"" alt=""Page"">" '-HC-
formsIcon =  "<img src=""" & AppPath  & "images/UI/Icons/contentForm.png"" alt=""Form"">" '-HC-
menuIcon = "<img src=""" & AppPath  & "images/UI/Icons/menu.png"" alt=""Menu"">" '-HC-
libraryIcon = "<img src=""" & AppPath  & "images/UI/Icons/book.png"" alt=""Library Item"">" '-HC-
linkIcon = "<img src=""" & AppPath  & "images/UI/Icons/link.png"" alt=""External Link"">" '-HC-

Dim LanguageName, colLangName As Object
LanguageName = Nothing

If ContentLanguage <> ALL_CONTENT_LANGUAGES Then
	colLangName=gtMsgObj.GetLanguageById(ContentLanguage)
	LanguageName=colLangName("Name")
End If

%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head id="Head1" runat="server">
<asp:PlaceHolder id="phHeadTag" runat="server">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
	<meta  http-equiv="Cache-Control" content="no-cache, must-revalidate" />
	<meta http-equiv="pragma" content="no-cache" />
	<title>Collections</title>
<script type="text/javascript">
<!--//--><![CDATA[//><!--
function resetCPostback(){
    document.forms["frmCollectionList"].isCPostData.value = "";
}
function searchcollection(){
    if(document.forms["frmCollectionList"].txtSearch.value.indexOf('\"')!=-1){
        alert('remove all quote(s) then click search');
        return false;
    }
    $ektron("#txtSearch").clearInputLabel();
    document.forms["frmCollectionList"].isSearchPostData.value = "1";
    document.forms["frmCollectionList"].isCPostData.value="true";
    document.forms["frmCollectionList"].submit();
    return true;
}
function CheckForReturn(e)
	{
	    var keynum;
        var keychar;

        if(window.event) // IE
        {
            keynum = e.keyCode
        }
        else if(e.which) // Netscape/Firefox/Opera
        {
            keynum = e.which
        }

        if( keynum == 13 ) {
            document.getElementById('btnSearch').focus();
        }
	}
function LoadFolderPicker() //(type, tagtype, metadataFormTagId)
{
	var pageObj, frameObj
	var id = 0;
	var title = '';
	var delimeterChar = ";";
	var metadataFormTagId = 1;
    var menuFlag = (window.location.toString().toLowerCase().indexOf("action=editmenu") ? "&menuflag=true" : "");

	if (isBrowserIE())
	{
		// Configure the Meta window to be visible:
		frameObj = document.getElementById('FolderPickerPage');
		if (ek_ma_validateObject(frameObj))
		{
			window.scrollTo(0,0); // ensure that the iframe will be in view.
			frameObj.src = 'blankredirect.aspx?MetaSelectContainer.aspx?FolderID=' + id + '&browser=0&WantXmlInfo=1&metadataFormTagId=' + metadataFormTagId + '&separator=' + delimeterChar + '&selectids=' + id + '&selecttitles=' + title + menuFlag;

			pageObj = document.getElementById('FolderPickerPageContainer');
			pageObj.style.display = '';
			pageObj.style.width = '100%'; //'85%';
			pageObj.style.height = '100%'; //'90%';

			// Ensure that the transparent layer completely covers the parent window:
			pageObj = document.getElementById('FolderPickerAreaOverlay');
			pageObj.style.display = '';
			pageObj.style.width = '100%';
			pageObj.style.height = '100%';
		}
	}
	else
	{
		// Browser is Netscape, use a seperate pop-up window:
		var windObj = window.open('blankredirect.aspx?MetaSelectContainer.aspx?FolderID=' + id + '&browser=1&WantXmlInfo=1&metadataFormTagId=' + metadataFormTagId + '&separator=' + delimeterChar + '&selectids=' + id + '&selecttitles=' + title + menuFlag,'Preview','width=' + 600 + ',height=' + 400 +',status=no,resizable=yes,scrollbars=no,location=no,toolbar=no');
	}
}

function ek_ma_validateObject(obj)
{
	return ((obj != null) &&
		((typeof(obj)).toLowerCase() != 'undefined') &&
		((typeof(obj)).toLowerCase() != 'null'))
}

function isBrowserIE() {
  //return (document.all ? true : false);
	var ua = window.navigator.userAgent.toLowerCase();
  return((ua.indexOf('msie') > -1) && (!(ua.indexOf('opera') > -1)));
}

function ek_ma_CloseChildPage()
{
	var pageObj = document.getElementById('FolderPickerPageContainer');

	// Configure the Meta window to be invisible:
	pageObj.style.display = 'none';
	pageObj.style.width = '1px';
	pageObj.style.height = '1px';

	// Ensure that the transparent layer does not cover any of the parent window:
	pageObj = document.getElementById('FolderPickerAreaOverlay');
	pageObj.style.display = 'none';
	pageObj.style.width = '1px';
	pageObj.style.height = '1px';
}

function ek_ma_CloseMetaChildPage()
{
	ek_ma_CloseChildPage();
}

function ek_ma_ReturnMediaUploaderValue(selectedIdName, title, metadataFormTagId)
{
	var obj, testObj;
	var delimeterChar = ";";
	var namIdArray, titleArray;
	var idx;

	// clear original values:
	ek_ma_ClearSelection(metadataFormTagId, "");

	// save new selections:
	var frm_associated_folder_list_obj = document.getElementById("associated_folder_id_list");
	if (frm_associated_folder_list_obj) {
		frm_associated_folder_list_obj.value = selectedIdName;
	}
	var frm_associated_folder_title_list_obj = document.getElementById("associated_folder_title_list");
	if (frm_associated_folder_title_list_obj) {
		frm_associated_folder_title_list_obj.value = title;
	}

	// list the items for the user:
	if (delimeterChar && delimeterChar.length)
	{
		namIdArray = selectedIdName.split(delimeterChar);
		titleArray = title.split(delimeterChar);
		for (idx=0; idx < namIdArray.length; idx++)
		{
			var itemId = namIdArray[idx];
			var itemTitle;
			if (titleArray && titleArray[idx])
			{
				itemTitle = titleArray[idx];
			}
			else
			{
				itemTitle = "";
			}
			ek_ma_addMetaRow(itemId, itemTitle, metadataFormTagId)
		}
	}
}

function ek_ma_ClearSelection(metadataFormTagId, msgText)
{
	var childObj, tempEl, tblBodyObj, rowObj, cellObj, textObj;
	var containerObj = document.getElementById("EnhancedMetadataMultiContainer" + metadataFormTagId.toString());
	if (containerObj)
	{
		while (childObj = containerObj.lastChild)
		{
			tempEl = containerObj.removeChild(childObj);
		}

		if (msgText && msgText.length)
		{
			tblBodyObj = document.createElement("tbody");
			tblBodyObj = containerObj.appendChild(tblBodyObj);
			rowObj = document.createElement("tr");
			rowObj = tblBodyObj.appendChild(rowObj);
			cellObj = document.createElement("td");
			cellObj = rowObj.appendChild(cellObj);
			textObj = document.createTextNode(msgText);
			textObj = cellObj.appendChild(textObj);
		}
	}

	var frm_associated_folder_list_obj = document.getElementById("associated_folder_id_list");
	if (frm_associated_folder_list_obj) {
		frm_associated_folder_list_obj.value = "";
	}

	var frm_associated_folder_title_list_obj = document.getElementById("associated_folder_title_list");
	if (frm_associated_folder_title_list_obj) {
		frm_associated_folder_title_list_obj.value = "";
	}
}

function ek_ma_addMetaRow(id, title, metadataFormTagId)
{
	var tblBodyObj, rowObj, cellObj, textObj;
	var thumbnail, idx, textStr, obj;
	var cellBgColor = "";
	var containerObj = document.getElementById("EnhancedMetadataMultiContainer"
			+ metadataFormTagId.toString());
	if (containerObj)
	{
		if (id && id.length)
		{
			// if no table-body, must add one:
			tblBodyObj = containerObj.firstChild;
			if (null == tblBodyObj)
			{
				tblBodyObj = document.createElement("tbody");
				tblBodyObj = containerObj.appendChild(tblBodyObj);
			}

			// determine background color based on odd/even current row count:
			//if (tblBodyObj.children && (tblBodyObj.children.length & 1))
			if (tblBodyObj.childNodes && (tblBodyObj.childNodes.length & 1))
			{
				cellBgColor = "#eeeeee";
			}

			// add cell with title and id (with appropriate background color):
			rowObj = document.createElement("tr");
			rowObj = tblBodyObj.appendChild(rowObj);
			cellObj = document.createElement("td");
			cellObj = rowObj.appendChild(cellObj);
			if (cellBgColor.length) // && cellObj.bgColor)
			{
				cellObj.bgColor = cellBgColor;
			}

			textStr = title + " (Folder ID: " + id + ")";
			textObj = document.createTextNode(textStr);
			textObj = cellObj.appendChild(textObj);
		}
	}
}

function ek_ma_getSelectedFormTagId(){return (1);}
function ek_ma_getDelimiter(metadataFormTagId){return (";");}

function ek_ma_getId(metadataFormTagId){
	var result = "";
	var frm_associated_folder_list_obj = document.getElementById("associated_folder_id_list");
	if (frm_associated_folder_list_obj) {
		result = frm_associated_folder_list_obj.value;
	}
	return (result);
}

function ek_ma_getTitle(metadataFormTagId){
	var result = "";
	var frm_associated_folder_title_list_obj = document.getElementById("associated_folder_title_list");
	if (frm_associated_folder_title_list_obj) {
		result = frm_associated_folder_title_list_obj.value;
	}
	return (result);
}

function metaselect_initialize()
{
	var ids, title;
	var formTagId = ek_ma_getSelectedFormTagId();
	var frm_associated_folder_list_obj = document.getElementById("associated_folder_id_list");
	if (frm_associated_folder_list_obj) {
		ids = frm_associated_folder_list_obj.value;
	}

	var frm_associated_folder_title_list_obj = document.getElementById("associated_folder_title_list");
	if (frm_associated_folder_title_list_obj) {
		title = frm_associated_folder_title_list_obj.value;
	}

	if (ids!=null && ids.length) {
		ek_ma_ReturnMediaUploaderValue(ids, title, formTagId);
	}
	else {
		ek_ma_ClearSelection(formTagId, "None selected");
	}
}

Ektron.ready(function()
{
    if($ektron.browser.msie == true && parseInt(jQuery.browser.version, 10) < 8)
    {
        var heightFixer = $ektron(".heightFix");
        heightFixer.css("height", heightFixer.outerHeight() + "px");
    }
});
//--><!]]>
</script>

<script type="text/javascript">
<!--//--><![CDATA[//><!--
//hide drag drop uploader frame/////
if (typeof top.HideDragDropWindow != "undefined")
{
	top.HideDragDropWindow();
}
////////////////////////////////////
/***********************************************
* Contractible Headers script- � Dynamic Drive (www.dynamicdrive.com)
* This notice must stay intact for legal use. Last updated Oct 21st, 2003.
* Visit http://www.dynamicdrive.com/ for full source code
***********************************************/

var enablepersist="off" //Enable saving state of content structure using session cookies? (on/off)
var collapseprevious="no" //Collapse previously open content when opening present? (yes/no)

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
	getElementbyClass("switchcontent");
	if (enablepersist=="on" && typeof ccollect!="undefined"){
		revivecontent();
	}
	metaselect_initialize();
	ta_populateSelectedList();
}


//if (window.addEventListener)
//window.addEventListener("load", do_onload, false)
//else if (window.attachEvent)
//window.attachEvent("onload", do_onload)
//else if (document.getElementById)
//window.onload=do_onload

if (enablepersist=="on" && document.getElementById)
window.onunload=saveswitchstate
//--><!]]>
</script>


<script type="text/javascript">
<!--//--><![CDATA[//><!--

function ConfirmNavDelete() {
	return confirm("<%=(MsgHelper.GetMessage("js: confirm collection deletion msg"))%>");
}
function ConfirmMenuDelete() {
		return confirm("<%=(MsgHelper.GetMessage("js: confirm menu deletion msg"))%>");
		//Are you sure you want to delete the menu?
}
function CloseChildPage()
{
	var pageObj = document.getElementById("FrameContainer");
	pageObj.style.display = "none";
	pageObj.style.width = "1px";
	pageObj.style.height = "1px";
	<%If (Request.QueryString("action")<>"AddMenu") Then%>
	// fix for bug 19017 by Todd Etzel on St. Patrick's day 2006.
	// Firefox 1.5 bug doesn't like window.location.redirect() with POSTDATA - so just say window.location.href = window.location.href
	//window.location.reload();
	window.location.href = window.location.href;
	<%End If%>

}
function ReturnChildValue(contentid,contenttitle,QLink, FolderID,LanguageID)
{
	CancelIframe();
	document.getElementById("frm_menu_link").value = QLink.replace('<%=(SitePath)%>', '');
}
function LoadSelectContentPage()
{
	var languageID;
	languageID = '<%=ContentLanguage%>';
	PopUpWindow("SelectCreateContent.aspx?FolderID=0&rmadd=false&LangType=" + languageID+"&browser=1&ty=menu", "SelectContent", 490,500,1,1);
}
function CancelIframe()
{
	var pageObj = document.getElementById("FrameContainer");
	pageObj.style.display = "none";
	pageObj.style.width = "1px";
	pageObj.style.height = "1px";

}
function UpdateView(){
	var objSelSupertype = document.getElementById('selAssetSupertype');
	if (objSelSupertype != null)
	{
		var ContType = objSelSupertype.value;
        if (replaceQueryString != "") { replaceQueryString = replaceQueryString + "&" }
		document.location.href= location.pathname + "?" + replaceQueryString + "ContType=" + ContType;
		//strAction += "&ContType=" + ContType;   //ContentTypeUrlParam changed ContType
	}

}

function PopBrowseWin(Scope, FolderPath, retField, qdonly){
	var Url;
    if (FolderPath == "") {
        Url = 'browselibrary.aspx?actiontype=add&scope=' + Scope;
	} else {
		Url = 'browselibrary.aspx?actiontype=add&scope=' + Scope + '&autonav=' + FolderPath;
	}
	if (retField != null)
	{
		Url = Url + '&RetField=' + retField;
	}
	if ((qdonly != undefined) && (qdonly != null) && (qdonly != '')) {
	    Url = Url + '&qdo=1';
	}
    PopUpWindow(Url, 'BrowseLibrary', 790, 580, 1, 1);
}

function selectLibraryItem(libraryid, folder, title, filename, type, returnField){
	var ObjField = eval(returnField);
    var site_Path = "<%=(SitePath)%>";
	if (ObjField != null)
	{
	    if(site_Path == "/")
	        {
	            if(filename.indexOf("/") == 0)
	                ObjField.value = filename.substring(1);
	            else
	                ObjField.value = filename;
	        }
	   else if(filename.indexOf("http://") == 0)
	        ObjField.value = filename;
	   else
		    ObjField.value = filename.replace("<%=(SitePath)%>", "");
	} else {
		document.forms[0].title.value = title;
		document.forms[0].DefaultTitle.value = title;
		document.forms[0].id.value = libraryid;
	}
}
//function selectLibraryItem(libraryid, folder, title, filename, type){
//	document.forms.AddMenuItem.title.value = title;
//	document.forms.AddMenuItem.DefaultTitle.value = title;
//	document.forms.AddMenuItem.id.value = libraryid;
//}
//--><!]]>
</script>
<script type="text/javascript" >
<!--//--><![CDATA[//><!--
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
	function DefaultTitleCheck(chkDefault, frmName, FieldName) {
		var objField;
		objField = eval("document.forms." + frmName + "." + FieldName);
		if (chkDefault.checked == true) {
			objField.disabled = true;
		}
		else {
			objField.disabled = false;
		}
		return true;
	}
	function Move(sDir, objList, objOrder) {
		if (objList.selectedIndex != null && objList.selectedIndex >= 0) {
			nSelIndex = objList.selectedIndex;
			sSelValue = objList[nSelIndex].value;
			sSelText = objList[nSelIndex].text;
			objList[nSelIndex].selected = false;
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
    function selectClearAll(obj)
    {
        if(obj.checked)
        {
            SelectAll();
            return false;
        } 
        else
        {
            ClearAll();
            return false;
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
		var llang="";
		document.forms[0].frm_content_ids.value = "";
		document.forms[0].frm_content_languages.value = "";
		document.forms[0].frm_folder_ids.value = "";
		for (lLoop = 0; lLoop < document.forms[0].length; lLoop++) {
			if ((document.forms[0][lLoop].type.toLowerCase() == "hidden")
					&& (document.forms[0][lLoop].name.toLowerCase().search("frm_hidden") != -1)
					&& (document.forms[0][lLoop].value != 0)) {
					llang=document.forms[0][lLoop].name;
					llang=llang.replace("frm_hidden","");
				document.forms[0].frm_content_ids.value = document.forms[0].frm_content_ids.value + "," + document.forms[0][lLoop].value;
				document.forms[0].frm_content_languages.value = document.forms[0].frm_content_languages.value + "," + document.forms[0]["frm_languages"+llang].value;
				document.forms[0].frm_folder_ids.value = document.forms[0].frm_folder_ids.value + "," + document.forms[0][lLoop].fid;
			}
		}

		document.forms[0].frm_content_ids.value = document.forms[0].frm_content_ids.value.substring(1, document.forms[0].frm_content_ids.value.length);
		document.forms[0].frm_content_languages.value = document.forms[0].frm_content_languages.value.substring(1, document.forms[0].frm_content_languages.value.length);
		document.forms[0].frm_folder_ids.value = document.forms[0].frm_folder_ids.value.substring(1, document.forms[0].frm_folder_ids.value.length);
		if (document.forms[0].frm_content_ids.value.length == 0) {
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
	function VerifyLibraryAssest() {
		if (typeof document.forms.AddMenuItem.title == "object") {
			document.forms.AddMenuItem.title.value = Trim(document.forms.AddMenuItem.title.value);
			if (document.forms.AddMenuItem.title.value == "") {
				alert("<%=(MsgHelper.GetMessage("js: title required msg"))%>");
				return false;
			}
		}
		if (typeof document.forms.AddMenuItem.id == "object") {
			document.forms.AddMenuItem.id.value = Trim(document.forms.AddMenuItem.id.value);
			if (document.forms.AddMenuItem.id.value == "") {
				alert("Invalid Library Item.");
				return false;
			}
		}
		return true;
	}
	function resetPostback()
	{
		document.forms[0].isPostData.value = "";
	}
	function VerifyAddMenuItem(){
		if (typeof document.forms.AddMenuItem.Title == "object") {
			document.forms.AddMenuItem.Title.value = Trim(document.forms.AddMenuItem.Title.value);
			if (document.forms.AddMenuItem.Title.value == "") {
				alert("<%=(MsgHelper.GetMessage("js: title required msg"))%>");
				return false;
			}
		}
		if (typeof document.forms.AddMenuItem.Link == "object") {
			document.forms.AddMenuItem.Link.value = Trim(document.forms.AddMenuItem.Link.value);
			if (document.forms.AddMenuItem.Link.value == "") {
				alert("<%=(MsgHelper.GetMessage("js: item link required msg"))%>");
				return false;
			}
		}
		if (typeof document.forms.AddMenuItem.frm_menu_image_override == "object")
		{
			if (document.forms.AddMenuItem.frm_menu_image_override.checked)
			{
				if (document.forms.AddMenuItem.frm_menu_image.value == "")
				{
					alert("Image path is required if the checkbox \"Use Image Instead Of Title\" is checked.");
					return false;
				}
			}
		}

		if (typeof document.forms.AddMenuItem.Description == "object")
		{
			if (document.forms.AddMenuItem.Description.value.length > 254)
			{
				alert (MsgHelper.GetMessage("alt The menu description should be less than 255 charecters."));
				document.forms.AddMenuItem.Description.focus();
				return false;
			}
		}
		return true;
	}
	function VerifyMenuForm () {
		regexp1 = /"/gi; //Leave this comment "
		document.forms.menu.frm_menu_title.value = Trim(document.forms.menu.frm_menu_title.value);
		document.forms.menu.frm_menu_title.value = document.forms.menu.frm_menu_title.value.replace(regexp1, "'");

		document.forms.menu.frm_menu_description.value = Trim(document.forms.menu.frm_menu_description.value);
		document.forms.menu.frm_menu_description.value = document.forms.menu.frm_menu_description.value.replace(regexp1, "'");
		if (document.forms.menu.frm_menu_description.value.length > 254)
		{
			alert ("The menu description should be less than 255 charecters.");
			document.forms.menu.frm_menu_description.focus();
			return false;
		}
		if (document.forms.menu.frm_menu_title.value == "")
		{
			alert ("<%=(MsgHelper.GetMessage("js: title required msg"))%>");
			document.forms.menu.frm_menu_title.focus();
			return false;
		}

		var ckbImage=document.getElementById("frm_menu_image_override");

		if (("object"==typeof(ckbImage)) && (ckbImage != null))
		{
			if (true==document.forms.menu.frm_menu_image_override.checked)
			{
				var strTemp
				strTemp = Trim(document.forms.menu.frm_menu_image.value)
				if (strTemp.length <= 0)
				{
					alert ("Please select an image to use.");
					document.forms.menu.frm_menu_image.focus();
					return false;
				}
			}
		}

		var obj_template=document.getElementById("frm_menu_template");
		if (("object"==typeof(obj_template)) && (obj_template != null))
		{
			var obj_template_set=document.getElementById("frm_set_to_template");

			if (("object"==typeof(obj_template_set)) && (obj_template_set != null))
			{
				if ((obj_template_set.value=="") && (Trim(obj_template.value)!=""))
				{
					var blnAns;
					blnAns=confirm('Do you want to apply the specified template to all content items already on this menu? If so, click "OK". \n\n If you click "Cancel", the template will only be applied as new content items are added to the menu. Note that you can use the Edit Menu Item screen to manually apply this template to any content items already on the menu. \n\n Note:  The specified template will only be applied this menu, not its sub-menus.');
					if (true==blnAns)
					{
						obj_template_set.value="true";
					}
				}
			}
		}

		return true;
	}
	function VerifyCollectionForm () {
		regexp1 = /"/gi;		// quote ... "
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
	function LoadLanguage(inVal) {
		if(inVal=='0') { return false ; }
		top.notifyLanguageSwitch(inVal, -1);
		document.location = '<%= Request.ServerVariables("PATH_INFO") & "?" & Ektron.Cms.API.JS.Escape(Replace(Request.ServerVariables("QUERY_STRING"),"LangType","L", 1, -1, 1).Replace("\x", "\\x")) %>&LangType=' + inVal ;
	}

	function addBaseMenu(menuID, parentID, ancestID, foldID, langID) {
		document.location = 'collections.aspx?action=AddTransMenu&nId=' + menuID + '&backlang=<%=ContentLanguage%>&LangType=' + langID + '&folderid=' + foldID + '&ancestorid=' + ancestID + '&parentid=' + parentID   ;
	}
	//--><!]]>
	</script>
	<!--#include file="common/stylesheetsetup.inc" -->

	<script type="text/javascript">
	<!--//--><![CDATA[//><!--

	//
	// Template Association functions begin:
	//

	function ta_editSelectList(){
		var listObj = document.getElementById("template_list");
		var currentOption;
		var textControl = document.getElementById("template_text");
		if (listObj && textControl) {
			for (index = 0; index < listObj.length; index++){
				currentOption = listObj.options[index];
				if (currentOption.selected == true){
					textControl.value=listObj.options[index].text;
					break;
				}
			}
			ta_updateSelectedValues();
		}
	}

	function ta_moveItemUp(){
		var currentOption;
		var prevOption;
		var prevOptionValue;
		var prevOptionText;
		var prevOptionSelectState;
		var wasPrevIterSelected = false;
		var wasPrevIterMoved = false;
		var listObj = document.getElementById("template_list");

		if (listObj) {
			for (index = 0; index < listObj.length; index++){
				currentOption = listObj.options[index];
				if (currentOption.selected == true){
					if (index == 0){
					wasPrevIterMoved = false;
					}
					else{
						if (wasPrevIterSelected){
							if (wasPrevIterMoved){
								prevOption = listObj.options[index - 1];
								prevOptionValue = prevOption.value;
								prevOptionText = prevOption.text;
								prevOptionSelectState = prevOption.selected;
								prevOption.value = currentOption.value;
								prevOption.text = currentOption.text;
								prevOption.selected = true;
								currentOption.value = prevOptionValue;
								currentOption.text = prevOptionText;
								currentOption.selected = prevOptionSelectState;
								wasPrevInterMoved = true;
							}
							else{
								wasPrevIterMoved = false;
							}
						}
						else{
							prevOption = listObj.options[index - 1];
							prevOptionValue = prevOption.value;
							prevOptionText = prevOption.text;
							prevOptionSelectState = prevOption.selected;
							prevOption.value = currentOption.value;
							prevOption.text = currentOption.text;
							prevOption.selected = true;
							currentOption.value = prevOptionValue;
							currentOption.text = prevOptionText;
							currentOption.selected = prevOptionSelectState;
							wasPrevIterMoved = true;
						}
					}
					wasPrevIterSelected = true;
				}
				else {
					wasPrevIterSelected = false;
					wasPrevIterMoved = false;
				}
			}
			ta_updateSelectedValues();
		}
	}

	function ta_moveItemDown(){
		var currentOption;
		var nextOption;
		var nextOptionValue;
		var nextOptionText;
		var nextOptionSelectState;
		var wasPrevIterSelected = false;
		var wasPrevIterMoved = false;

		var listObj = document.getElementById("template_list");
		if (listObj) {
			for (index = listObj.length - 1; index >= 0; index--){
				currentOption = listObj.options[index];
				if (currentOption.selected == true) {
					if (index == listObj.length - 1) {
						wasPrevIterMoved = false;
					}
					else{
						if (wasPrevIterSelected){
							if (wasPrevIterMoved) {
								nextOption = listObj.options[index + 1];
								nextOptionValue = nextOption.value;
								nextOptionText = nextOption.text;
								nextOptionSelectState = nextOption.selected;
								nextOption.value = currentOption.value;
								nextOption.text = currentOption.text;
								nextOption.selected = true;
								currentOption.value = nextOptionValue;
								currentOption.text = nextOptionText;
								currentOption.selected = nextOptionSelectState;
								wasPrevIterMoved = true;
							}
							else {
								wasPrevIterMoved = false;
							}
						}
						else{
							nextOption = listObj.options[index + 1];
							nextOptionValue = nextOption.value;
							nextOptionText = nextOption.text;
							nextOptionSelectState = nextOption.selected;
							nextOption.value = currentOption.value;
							nextOption.text = currentOption.text;
							nextOption.selected = true;
							currentOption.value = nextOptionValue;
							currentOption.text = nextOptionText;
							currentOption.selected = nextOptionSelectState;
							wasPrevIterMoved = true;
						}
					}
					wasPrevIterSelected = true;
				}
				else {
					wasPrevIterSelected = false;
					wasPrevIterMoved = false;
				}
			}
			ta_updateSelectedValues();
		}
	}

	function ta_addItemToSelectList(){
	    var listObj = document.getElementById("template_list");

		if(document.getElementById("template_text").value.length == 0){
		    alert("Please enter a template.");
		    return false;
		}

		if (listObj) {
			//if(document.getElementById("template_text").value==""){
			//	setSelectedItems(listObj, false);
			//	return false;
			//}
			var currentOption;
			var index=listObj.length;
			for (index = 0; index < listObj.length; index++) {
				currentOption = listObj.options[index];
				if(document.getElementById("template_text").value==currentOption.value){
					alert("duplicate Text/Value supplied");return false;
				}
			}
			index=listObj.length;
			listObj.length=index+1;
			listObj.options[index].value=document.getElementById("template_text").value;
			listObj.options[index].text=document.getElementById("template_text").value;
			document.getElementById("template_text").value="";
			ta_updateSelectedValues();
		}
	}

	function ta_updateItemToSelectList(){
		//if(document.getElementById("template_text").value==""){
		//	setSelectedItems(listObj,false);
		//	return false;
		//}
		var currentOption;
		var duplicateIndex=-1;
		var listObj = document.getElementById("template_list");
		if (listObj) {
			for (index = 0; index < listObj.length; index++) {
				currentOption = listObj.options[index];
				if(document.getElementById("template_text").value==currentOption.value && !currentOption.selected){
					duplicateIndex=index;
				}
			}
			if(duplicateIndex>-1){alert("text already exists");return false;}
			for (index = 0; index < listObj.length; index++) {
				currentOption = listObj.options[index];

				if (currentOption.selected == true){
					listObj.options[index].text=document.getElementById("template_text").value;
					listObj.options[index].value=document.getElementById("template_text").value;
					document.getElementById("template_text").value="";
					break;
				}
			}
			ta_updateSelectedValues();
		}
	}

	function ta_removeItemsFromSelectList(){
		var currentOption;
		var listObj = document.getElementById("template_list");
		if (listObj) {
			for (index = 0; index < listObj.length; index++) {
				currentOption = listObj.options[index];
				if (currentOption.selected == true){
					listObj.options[index]=null;
				}
			}
			document.getElementById("template_text").value="";
			ta_updateSelectedValues();
		}
	}

	function ta_updateSelectedValues(){
		var selectedValues="";
		var listObj = document.getElementById("template_list");
		if (listObj) {
			for (index = 0; index < listObj.length; index++) {
				if(selectedValues==""){
					selectedValues=listObj.options[index].value;
				}else{
					selectedValues=selectedValues+";"+listObj.options[index].value;
				}
			}
			document.getElementById("associated_templates").value=selectedValues;
		}
	}

	function ta_populateSelectedList(){
		var listObj = document.getElementById("template_list");
		var values ="";
		if(document.getElementById("associated_templates") !=null){
		    values= document.getElementById("associated_templates").value;
		}

		if (listObj && (listObj.length == 0) && (values != "")) {
			var availableList = values.split(";");
			var index=listObj.length;
			for (index1 = 0; index1 < availableList.length; index1++){
				listObj.length = index+1;
				listObj.options[index].value = availableList[index1];
				listObj.options[index].text = availableList[index1];
				index+=1;
			}
		}
	}
    //--><!]]>
	</script>
</asp:PlaceHolder>
<style type="text/css">
    span.pathInfo {display: inline-block; color:#e17009; margin-left: .5em; padding: .25em; border: solid 1px #ccc; cursor: default; background-color: #eee;}
	.info img {margin-right: .5em;}
</style>

    <!-- Styles -->
    <link type="text/css" rel="stylesheet" href="java/plugins/modal/ektron.modal.css" />
    <link type="text/css" rel="stylesheet" href="java/plugins/treeview/ektron.treeview.css" />
    <!-- Scripts -->
    <script language="javascript" type="text/javascript" src="java/ektron.js"></script>
    <script language="javascript" type="text/javascript" src="java/plugins/modal/ektron.modal.js"></script>
    <script language="javascript" type="text/javascript" src="java/plugins/treeview/ektron.treeview.js"></script>

<script type="text/javascript">
    function OnFileClicked(path) {
        if (path != null && path.length > 0) {
            var listObj = document.getElementById("template_list");
            if (listObj) {
                var currentOption;
                var index = listObj.length;
                for (index = 0; index < listObj.length; index++) {
                    currentOption = listObj.options[index];
                    if (path == currentOption.value) {
                        alert("duplicate Text/Value supplied"); return false;
                    }
                }
                index = listObj.length;
                listObj.length = index + 1;
                listObj.options[index].value = path;
                listObj.options[index].text = path;
                document.getElementById("template_text").value = "";
                ta_updateSelectedValues();
            }
        }
        
        $ektron("#dlgBrowse").modal().modalHide();
    }

    Ektron.ready(function()
        {
            // add ektronPageHeader since baseClass doesn't add it for us
            $ektron("table.baseClassToolbar").wrap("<div class='ektronPageHeader'></div>");

            // Initialize browse dialog
            $ektron("#dlgBrowse").modal({
                modal: true, 
                trigger: ".ektronModal",
                onShow: function(h) {
                    $ektron("#dlgBrowse").css("margin-top", -1 * Math.round($ektron("#dlgBrowse").outerHeight()/2));h.w.show();
            }});

            // Initialize PageBuilder checkbox
            var checkbox = $ektron(".pageBuilderCheckbox input");

            //OnPageBuilderCheckboxChanged(checkbox.is(':checked'));

            checkbox.click(function ()
            {
                //OnPageBuilderCheckboxChanged($ektron(".pageBuilderCheckbox input").is(':checked'));
            });

            // Initialize widgetType display

            var widgets = $ektron(".widget");
            widgets.each(function (i)
            {
                var widget = $(widgets[i]);
                if(widget.find("input").is(":checked"))
                {
                    widget.addClass("selected");
                }

                widget.click(function ()
                {
                    var widgetCheckbox = widget.find("input");

                    ToggleCheckbox(widgetCheckbox);
                    if(widgetCheckbox.is(":checked"))
                    {
                        widget.addClass("selected");
                    }
                    else
                    {
                        widget.removeClass("selected");
                    }
                });
            });

            /*widgetCheckboxes.each(function (i)
            {
                widgetCheckboxes[i].
            });*/
        });
	</script>
	
</head>

<body>
<%
if (action = "Add") then

	SiteObj = AppUI.EkSiteRef
	cPerms = SiteObj.GetPermissions(folderID, 0, "folder")
	if (ErrorString = "") then
		if ((not IsCollectionMenuRoleMember()) AndAlso (Not(cPerms("Collections")))) then
			ErrorString = MsgHelper.GetMessage("com: user does not have permission")
		end if
	end if

	if (ErrorString <> "") then
	%>
	    <div class="ektronPageHeader">
		    <div class="ektronTitlebar">
			    <%=(GetTitleBar(MsgHelper.GetMessage("add collection title")))%>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<%
	else
	%>
		<form action="collectionaction.aspx?Action=doAdd" method="Post" id="form2" name="nav">
		    <input type="hidden" name="frm_folder_id" value="<%=(folderId)%>"/>
			<div class="ektronPageHeader">
		        <div class="ektronTitlebar">
		            <%=(MsgHelper.GetMessage("add collection title"))%>
		        </div>
		        <div class="ektronToolbar">
			        <table>
				        <tr>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt: save collection text"), MsgHelper.GetMessage("btn save"), "onclick=""return SubmitForm('nav', 'VerifyCollectionForm()');"""))%>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewCollectionReport&folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					           <td><%=objStyle.GetHelpButton(action)%></td>
				        </tr>
			        </table>
	            </div>
	        </div>

	        <div class="ektronPageContainer ektronPageInfo">
		        <table class="ektronForm">
			        <tr>
				        <td class="label"><%=(MsgHelper.GetMessage("generic title label"))%></td>
				        <td><input type="Text" name="frm_nav_title" maxlength="75" onkeypress="return CheckKeyValue(event,'34');" /></td>
			        </tr>
			        <tr>
				        <td class="label"><%=(MsgHelper.GetMessage("generic template label"))%></td>
				        <td>
				            <%=(sitePath)%><input type="Text" name="frm_nav_template" class="ektronTextMedium" maxlength="255" value="" onkeypress="return CheckKeyValue(event,'34');">
				            <div class="ektronCaption"><%= (MsgHelper.GetMessage("collections: leave template empty")) %></div>
				        </td>
			        </tr>
			        <tr>
				        <td class="label"><%=(MsgHelper.GetMessage("description label"))%></td>
				        <td><textarea name="frm_nav_description" maxlength="255" onkeypress="return CheckKeyValue(event,'34');"></textarea></td>
			        </tr>
			        <tr>
				        <td class="label"><%=(MsgHelper.GetMessage("generic include subfolders msg"))%>:</td>
				        <td class="value"><input type="Checkbox" name="frm_recursive" /></td>
			        </tr>
			        <% if (cPerms("IsAdmin") or cPerms("Collections") or (IsFolderAdmin(folderID))) then %>
			            <tr>
			                <td class="label"><span style="color:Red"><%=(MsgHelper.GetMessage("lbl approval required"))%></span>:</td>
			                <td class="value"><input type="Checkbox" name="frm_approval_methhod" id="frm_approval_methhod" /></td>
			            </tr>
			        <% else %>
			            <input type="hidden" name="frm_approval_methhod" id="Hidden1" value="" />
			        <% end if %>
			        <%if (contentAPI.RequestInformationRef.EnableReplication) then %>
			            <tr>
			                <td class="label"><%=MsgHelper.GetMessage("replicate collection")%></td>
    				        <td class="value"><input type="Checkbox" name="EnableReplication" value="1" /></td>
                        </tr>
                    <%end if %>
		        </table>
		    </div>
		</form>
	<%
	SiteObj = nothing
    cPerms = nothing
	end if
elseif (action = "Edit") then
    SiteObj = AppUI.EkSiteRef
    cPerms = SiteObj.GetPermissions(folderID, 0, "folder")
	nId = Request.QueryString("nid")
	gtObj = AppUI.EkContentRef
	'''ap
	checkout = ""
    if (request.querystring("checkout") isnot nothing) then
        checkout = "&checkout=" & request.querystring("checkout").toString()
    end if
    if (request.querystring("status") isnot nothing) then
        if (request.querystring("status").toString.ToLower() = "o") then
            checkout = checkout & "&status=o"
        end if
    end if
    if (checkout <> "") then
        'bCheckedout = gtObj.CheckoutEcmCollection(nId)
    	gtNavs = gtObj.GetEcmStageCollectionByID( nId, 0, 0,   ErrorString, true, false, true)
    else
    	gtNavs = gtObj.GetEcmCollectionByID( nId, 0, 0,   ErrorString, true, false, true)
    end if
	if (ErrorString = "") then
		if (gtNavs.Count) then
			CollectionTitle = gtNavs("CollectionTitle")
		end if
	end if
	if (ErrorString <> "") then
	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(MsgHelper.GetMessage("edit collection title")))%>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<%
	else
	%>
		<form action="collectionaction.aspx?Action=doEdit<%=checkout %>" method="Post" id="form1" name="nav">
		    <input type="hidden" name="frm_nav_id" value="<%=(nId)%>" />
		    <input type="hidden" name="frm_folder_id" value="<%=(folderId)%>" />

		    <div class="ektronPageHeader">
		        <div class="ektronTitlebar">
		            <%=(GetTitleBar(MsgHelper.GetMessage("edit collection title")& " """ & CollectionTitle & """"))%>
		        </div>
		        <div class="ektronToolbar">
			        <table>
				        <tr>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt: update collection text"), MsgHelper.GetMessage("btn update"), "onclick=""return SubmitForm('nav', 'VerifyCollectionForm()');"""))%>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?Action=View&nid=" & nId & "&folderid=" & folderId , MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					        <td><%=objStyle.GetHelpButton(action)%></td>
				        </tr>
			        </table>
		        </div>
		    </div>

		    <div class="ektronPageContainer ektronPageInfo">
		        <div class="heightFix">
			    <table class="ektronForm">
				    <tr>
					    <td class="label"><%=(MsgHelper.GetMessage("generic title label"))%></td>
					    <td class="value"><input type="Text" name="frm_nav_title" value="<%=(gtNavs("CollectionTitle"))%>" maxlength="75" onkeypress="return CheckKeyValue(event,'34');" /></td>
				    </tr>
				    <tr>
					    <td class="label"><%=(MsgHelper.GetMessage("id label"))%></td>
					    <td class="readOnlyValue"><%=(gtNavs("CollectionID"))%></td>
				    </tr>
				    <tr>

					    <td class="label"><%=(MsgHelper.GetMessage("generic template label"))%></td>
					    <td class="value">
					        <%=(sitePath)%><input type="text" name="frm_nav_template" maxlength="255" value="<%=(gtNavs("CollectionTemplate"))%>" onkeypress="return CheckKeyValue(event,'34');" />
					        <div class="ektronCaption"><%= (MsgHelper.GetMessage("collections: leave template empty")) %></div>
					    </td>
				    </tr>
				    <tr>
					    <td class="label"><%=(MsgHelper.GetMessage("description label"))%></td>
					    <td class="value"><textarea name="frm_nav_description" maxlength="255" onkeypress="return CheckKeyValue(event,'34');"><%=(gtNavs("CollectionDescription"))%></textarea></td>
				    </tr>
				    <tr>
					    <td class="checkbox" colspan="2">
					        <input type="Checkbox" name="frm_recursive"
					            <%if (gtNavs("Recursive") = 1) then
					                response.write("checked")
					            end if%> />
					        <%=(MsgHelper.GetMessage("generic include subfolders msg"))%></td>
				    </tr>
				    <% if (IsCollectionMenuRoleMember() or cPerms("Collections") or (IsFolderAdmin(gtNavs("FolderID")))) then %>
				        <tr>
				            <td class="checkbox" colspan="2">
				                <input type="Checkbox" name="frm_approval_methhod" id="Checkbox1"
				                <%if (gtNavs("ApprovalRequired")) then
					                response.write("checked")
					            end if%>
				                />
					            <%=(msghelper.getmessage("lbl approval required"))%>
				            </td>
				        </tr>
			        <% else %>
			            <input type="hidden" name="frm_approval_methhod" id="Hidden2" value="<%=(gtNavs("ApprovalRequired"))%>" />
			        <% end if %>
				    <% if (contentAPI.RequestInformationRef.EnableReplication) then %>
				        <tr>
				            <td class="checkbox" colspan="2">
			                    <input type="Checkbox" name="EnableReplication" id="EnableReplication"
			                    <%if (gtNavs("EnableReplication") = 1) then
				                    response.write("checked")
				                end if%>
			                    />
				                <%=MsgHelper.GetMessage("replicate collection")%>
				            </td>
				        </tr>
				    <% end if %>
			    </table>
			    </div>
			</div>
		</form>
	<% end if%>
<%
elseif (action = "View" or action = "ViewStage") then
	if CInt(ContentLanguage) = ALL_CONTENT_LANGUAGES then
		ContentLanguage = AppUI.DefaultContentLanguage()
	end if

	nId = Request.QueryString("nid")
	gtObj = AppUI.EkContentRef
	AppUI.FilterByLanguage=ContentLanguage
	'''ap
	' can't do preview mode because we need the DMS imageURL to make this look pretty :-P
	if (LCase(action) = "viewstage") then
    	gtNavs = gtObj.GetEcmStageCollectionByID( nId, 0, 0, ErrorString, true, false, false)
	else
    	gtNavs = gtObj.GetEcmCollectionByID( nId, 0, 0, ErrorString, true, false, false)
	end if
	checkout = ""
	if (gtNavs("ApprovalRequired") = true andalso ((gtNavs("Status") = "A") orelse (gtNavs("Status") = "S"))) then
	    checkout = "&checkout=true"
	end if
	if (gtNavs("ApprovalRequired") = true andalso gtNavs("Status") = "O") then
	    checkout = checkout & "&status=o"
	end if
	If (Request.QueryString("folderid") Is Nothing) Then
	    folderId = gtNavs("FolderID")
	End If
	if (ErrorString = "") then
		if (gtNavs.Count) then
			CollectionTitle = gtNavs("CollectionTitle")
		end if
	end if
	if (ErrorString <> "") then
	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(MsgHelper.GetMessage("view collection title")))%>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
	    </div>
	<%
	else
	%>
		<form name="netscapefix" method="post" action="#">
		    <div class="ektronPageHeader">
		        <div class="ektronTitlebar">
		            <%=(GetTitleBar(MsgHelper.GetMessage("view collection title") & " """ & CollectionTitle & """"))%>
		        </div>
			    <div class="ektronToolbar">
			        <table>
				        <tr>
				            <%
                            if (LCase(action) = "viewstage") then
                                Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentViewPublished.png", "collections.aspx?action=View&LangType=" & ContentLanguage & "&nid=" & nId & "&folderid=" & folderId, MsgHelper.GetMessage("btn view publish"), MsgHelper.GetMessage("btn view publish") ,""))
                                if (gtNavs("Status") = "S") then
                                    if (IsCollectionApprover()) then
                                        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentPublish.png", "collectionaction.aspx?action=doPublishCol&LangType=" & ContentLanguage & "&nid=" & nId & "&folderid=" & folderId, MsgHelper.GetMessage("generic Publish"), MsgHelper.GetMessage("generic Publish") ,""))
                                        Response.Write(GetButtonEventsWCaption(AppPath & "images/ui/icons/approvalDenyItem.png", "collectionaction.aspx?action=doDeclineApprCol&LangType=" & ContentLanguage & "&nid=" & nId & "&folderid=" & folderId, MsgHelper.GetMessage("lbl decline"), MsgHelper.GetMessage("lbl decline") ,""))
                                    end if
                                elseif (gtNavs("Status") = "M")
                                    if (IsCollectionApprover()) then
					                    Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentPublish.png", "collectionaction.aspx?action=doDelete&nId=" & nId & "&folderid="& folderId & checkout, MsgHelper.GetMessage("alt: delete collection text"), MsgHelper.GetMessage("btn delete"), "onclick=""return ConfirmNavDelete();"""))
                                        Response.Write(GetButtonEventsWCaption(AppPath & "images/ui/icons/approvalDenyItem.png", "collectionaction.aspx?action=doDeclineDelCol&LangType=" & ContentLanguage & "&nid=" & nId & "&folderid=" & folderId, MsgHelper.GetMessage("lbl decline"), MsgHelper.GetMessage("lbl decline") ,""))
                                    end if
                                else
                                    if (IsCollectionApprover()) then
                                        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentPublish.png", "collectionaction.aspx?action=doPublishCol&LangType=" & ContentLanguage & "&nid=" & nId & "&folderid=" & folderId, MsgHelper.GetMessage("generic Publish"), MsgHelper.GetMessage("generic Publish") ,""))
                                    else
                                        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/approvalSubmitFor.png", "collectionaction.aspx?action=doSubmitCol&LangType=" & ContentLanguage & "&nid=" & nId & "&folderid=" & folderId, MsgHelper.GetMessage("generic Submit"), MsgHelper.GetMessage("generic Submit") ,""))
                                    end if
                                        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentRestore.png", "collectionaction.aspx?action=doUndoCheckoutCol&LangType=" & ContentLanguage & "&nid=" & nId & "&folderid=" & folderId, MsgHelper.GetMessage("generic Undocheckout"), MsgHelper.GetMessage("generic Undocheckout") ,""))
                                end if
                            else
                                if (gtNavs("Status") <> "A") then
                                    response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/preview.png", "collections.aspx?action=ViewStage&LangType=" & ContentLanguage & "&nid=" & nId & "&folderid=" & folderId, MsgHelper.GetMessage("btn view stage"), MsgHelper.GetMessage("btn view stage") ,""))
                                end if
                            end if
                            Dim enableQDOparam As Object
                            If (gtNavs("EnableReplication") = 1) Then
                                enableQDOparam = "&qdo=1"
                            End if
				            Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentAdd.png", "collections.aspx?LangType=" & ContentLanguage & "&action=AddLink&nid=" & nId & "&folderid=" & folderId & checkout & enableQDOparam, MsgHelper.GetMessage("alt: add collection items text"), MsgHelper.GetMessage("add collection items"), ""))
					        if (gtNavs("Contents").Count) then
					            Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/remove.png", "collections.aspx?LangType=" & ContentLanguage & "&action=DeleteLink&nid="& nId & "&folderid=" & folderId & checkout, MsgHelper.GetMessage("alt: remove collection items text"), MsgHelper.GetMessage("remove collection items"), ""))
		 				        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/arrowUpDown.png", "collections.aspx?LangType=" & ContentLanguage & "&action=ReOrderLinks&nid=" & nId & "&folderid=" & folderId & checkout, MsgHelper.GetMessage("alt: reorder collection text"), MsgHelper.GetMessage("btn reorder"), ""))
		 			        end if

		 			        'Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "collections.aspx?action=Edit&nid=" & nId & "&folderid="& folderId & checkout, MsgHelper.GetMessage("alt: edit collection text"), MsgHelper.GetMessage("btn edit"), ""))
				            If (action = "ViewStage")
				                Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/properties.png", "collections.aspx?LangType=" & ContentLanguage & "&action=ViewStageAttributes&nid=" & nId & "&folderid=" & folderId & checkout & enableQDOparam, MsgHelper.GetMessage("alt collection properties button text"), MsgHelper.GetMessage("properties text"), ""))
				            Else
				                Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/properties.png", "collections.aspx?LangType=" & ContentLanguage & "&action=ViewAttributes&nid=" & nId & "&folderid=" & folderId & enableQDOparam, MsgHelper.GetMessage("alt collection properties button text"), MsgHelper.GetMessage("properties text"), ""))
				            End If

		 			        if (gtNavs("Status") <> "M") then
                                if (IsCollectionApprover()) then
					                Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "collectionaction.aspx?action=doDelete&nId=" & nId & "&folderid="& folderId & checkout, MsgHelper.GetMessage("alt: delete collection text"), MsgHelper.GetMessage("btn delete"), "onclick=""return ConfirmNavDelete();"""))
					            else
					                Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "collectionaction.aspx?action=doSubmitDelCol&nId=" & nId & "&folderid="& folderId & checkout, MsgHelper.GetMessage("alt: delete collection text"), MsgHelper.GetMessage("btn delete"), "onclick=""return ConfirmNavDelete();"""))
					            end if
					        end if
					        if (Request.QueryString("bpage") = "reports") then
						        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewCollectionReport", MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))
					        else
						        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))
					        end if
	                        If ((gtNavs("Status") = "A") and (gtNavs("EnableReplication") = 1)) Then
		                        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/translate.png", "DynReplication.aspx?collid="&nId, MsgHelper.GetMessage("alt quickdeploy collection button text"), MsgHelper.GetMessage("alt quickdeploy collection button text"), ""))
	                        End If
					        %>
					        <%
					        If EnableMultilingual="1" Then
				                Response.Write("<td>&nbsp;|&nbsp;</td>")
						        %><td><%
						        Response.Write(MsgHelper.GetMessage("view language"))
						        Response.Write(LangDD(false,""))
						        %></td><%
					        end if
					        %>
					        <td><%=objStyle.GetHelpButton("ViewCollectionItems")%></td>
				        </tr>
			        </table>
		        </div>
		    </div>

	        <div class="ektronPageContainer ektronPageGrid">
	            <div class="heightFix">
	            <!-------------------- Links for this Item ------------------------------------>
	            <% gtLinks = gtNavs("Contents")%>
	                <table class="ektronGrid">
		                <tr class="title-header">
			                <td><%=(MsgHelper.GetMessage("generic Title"))%></td>
			                <td><%=(MsgHelper.GetMessage("lbl Language ID"))%></td>
			                <td><%=(MsgHelper.GetMessage("generic ID"))%></td>
			                <td><%=(MsgHelper.GetMessage("generic URL Link"))%></td>
		                </tr>
		                <%
		                dim j as integer = 0
		                for j = 1 to gtLinks.count
			                Dim backPage As String
			                backPage = "Action=View&nid=" & nId & "&folderid=" &folderId
			                backPage = Server.URLEncode(backPage)
			                Dim contentUrl As String
			                contentUrl = "content.aspx?action=View&LangType=" & gtLinks(j)("ContentLanguage") & "&id=" & (gtLinks(j)("ContentID")) & "&callerpage=collections.aspx&origurl=" & backPage
			                Dim contentTitle As String
			                contentTitle = (MsgHelper.GetMessage("generic View") & " """ & Replace(gtLinks(j)("ContentTitle"), "'", "`") & """")
			                Dim iconurl As String = ""
			                Try
			                    iconurl = gtLinks(j)("ImageUrl")
			                Catch
			                    ' ignore errors if we try getting imageurl on regular content
			                End Try
                            contentAPI.ContentLanguage = ContentLanguage
			                Dim dmsmenuhtml As String
			                dmsmenuhtml = contentAPI.GetDmsContextMenuHTML(gtLinks(j)("ContentID"), gtLinks(j)("ContentLanguage"), _
                                gtLinks(j)("ContentType"), gtLinks(j)("ContentSubtype"), gtLinks(j)("ContentTitle"), _
                                MsgHelper.GetMessage("generic Title") + " " + gtLinks(j)("ContentTitle"), contentUrl, _
                                "", iconurl)
			                %>
		                    <tr>
			                    <td><%=dmsmenuhtml %></td>
			                    <td><%=(gtLinks(j)("ContentLanguage"))%></td>
			                    <td><%=(gtLinks(j)("ContentID"))%></td>
			                    <td><%=(gtLinks(j)("ContentLinks"))%></td>
		                    </tr>
		                <% Next %>
	                </table>
	            </div>
	        </div>

		    <script type="text/javascript">
		    <!--//--><![CDATA[//><!--
		        do_onload();
		    //--><!]]>
		    </script>
		    <asp:Literal ID="litRefreshCollAccordion" runat="server" />
		</form>
	<%
	end if
	gtObj = nothing
	gtNavs = nothing
	gtLinks = nothing
	%>

<%
elseif (action="ViewAttributes" or action = "ViewStageAttributes") then

	if CInt(ContentLanguage) = ALL_CONTENT_LANGUAGES then
		ContentLanguage = AppUI.DefaultContentLanguage()
	end if

	nId = Request.QueryString("nid")
	gtObj = AppUI.EkContentRef
	AppUI.FilterByLanguage=ContentLanguage
	'''ap

	if (LCase(action) = "viewstageattributes") then
    	gtNavs = gtObj.GetEcmStageCollectionByID( nId, 0, 0, ErrorString, true, false, true)
	else
    	gtNavs = gtObj.GetEcmCollectionByID( nId, 0, 0, ErrorString, true, false, true)
	end if
	checkout = ""
	if (gtNavs("ApprovalRequired") = true andalso ((gtNavs("Status") = "A") orelse (gtNavs("Status") = "S"))) then
	    checkout = "&checkout=true"
	end if
	if (gtNavs("ApprovalRequired") = true andalso gtNavs("Status") = "O") then
	    checkout = checkout & "&status=o"
	end if
	if (ErrorString = "") then
		if (gtNavs.Count) then
			CollectionTitle = gtNavs("CollectionTitle")
		end if
	end if
	if (ErrorString <> "") then
	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(MsgHelper.GetMessage("view collection title")))%>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
	    </div>
	<%
	else
	%>
		<form name="netscapefix" method="post" action="#">
		    <div class="ektronPageHeader">
		        <div class="ektronTitlebar">
		            <%=(GetTitleBar(MsgHelper.GetMessage("view collection title") & " """ & CollectionTitle & """"))%>
		        </div>
			    <div class="ektronToolbar">
			        <table>
				        <tr>
				            <%
		 			        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "collections.aspx?action=Edit&nid=" & nId & "&folderid="& folderId & checkout, MsgHelper.GetMessage("alt: edit collection text"), MsgHelper.GetMessage("btn edit"), ""))
		 			        if (Request.QueryString("bpage") = "reports") then
						        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewCollectionReport", MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))
					        else
						        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=View&nid=" & nId & "&folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))
					        end if
					        %>
					        <%
					        If EnableMultilingual="1" Then
				                Response.Write("<td>&nbsp;|&nbsp;</td>")
						        %><td><%
						        Response.Write(MsgHelper.GetMessage("view language"))
						        Response.Write(LangDD(false,""))
						        %></td><%
					        end if
					        %>
					        <td><%=objStyle.GetHelpButton("ViewCollectionItems")%></td>
				        </tr>
			        </table>
		        </div>
		    </div>

            <div class="ektronPageContainer ektronPageInfo">
                <div class="heightFix">
                <table class="ektronForm">
	                <tr>
		                <td class="label"><%=(MsgHelper.GetMessage("generic title label"))%></td>
		                <td class="readOnlyValue"><%=(gtNavs("CollectionTitle"))%></td>
	                </tr>
	                <tr>
		                <td class="label"><%=(MsgHelper.GetMessage("id label"))%></td>
		                <td class="readOnlyValue"><%=(gtNavs("CollectionID"))%></td>
	                </tr>
	                <tr>
		                <td class="label"><%=(MsgHelper.GetMessage("generic template label"))%></td>
		                <td class="readOnlyValue"><%=(gtNavs("TemplatePath"))%></td>
	                </tr>
	                <tr>
		                <td class="label"><%=(MsgHelper.GetMessage("content LUE label"))%></td>
		                <td class="readOnlyValue"><%=(gtNavs("EditorFName") & " " & gtNavs("EditorLName"))%></td>
	                </tr>
	                <tr>
		                <td class="label"><%=(MsgHelper.GetMessage("content LED label"))%></td>
		                <td class="readOnlyValue"><%=(gtNavs("DisplayLastEditDate"))%></td>
	                </tr>
	                <tr>
		                <td class="label"><%=(MsgHelper.GetMessage("content DC label"))%></td>
		                <td class="readOnlyValue"><%=(gtNavs("DisplayDateCreated"))%></td>
	                </tr>
	                <tr>
		                <td class="label"><%=(MsgHelper.GetMessage("description label"))%></td>
		                <td class="readOnlyValue"><%=(gtNavs("CollectionDescription"))%></td>
	                </tr>
	                <tr>
	                    <td class="label"><%=(MsgHelper.GetMessage("lbl linkcheck status"))%></td>
		                <td class="readOnlyValue"><%=(gtNavs("Status"))%></td>
	                </tr>
	                <tr>
		                <td class="label"><%=(MsgHelper.GetMessage("generic include subfolders msg"))%>:</td>
		                <td class="value">
		                    <input type="Checkbox" name="frm_recursive" <%if (gtNavs("Recursive") = 1) then
		                    response.write("checked")
		                    end if%> disabled="disabled" onclick="return false;" />
		                </td>
	                </tr>
	                <tr>
	                    <td class="label"><%=(MsgHelper.GetMessage("lbl approval required")) %>:</td>
	                    <td class="value">
	                        <input type="checkbox" name="frm_approval_required" <%if (gtNavs("ApprovalRequired")) then
	                        response.write("checked")
	                        end if %> disabled="disabled" onclick="return false;" />
	                    </td>
	                </tr>
	                <% if (contentAPI.RequestInformationRef.EnableReplication) then %>
	                <tr>
	                    <td class="label"><%=(MsgHelper.GetMessage("replicate collection")) %></td>
	                    <td class="value">
	                        <input type="checkbox" name="EnableReplication" <%if (gtNavs("EnableReplication") = 1) then
	                        response.write("checked")
	                        end if %> disabled="disabled" onclick="return false;" />
	                    </td>
	                </tr>
	                <% end if %>
                </table>
                </div>
            </div>
		    <script type="text/javascript">
		        <!--//--><![CDATA[//><!--
		        do_onload();
		         //--><!]]>
		    </script>
		</form>
	<%
	end if
	gtObj = nothing
	gtNavs = nothing
	gtLinks = nothing
	%>

<%
elseif (action="ReOrderLinks") then
	dim reOrderList As String
	nId = request.QueryString("nid")
	AppUI.FilterByLanguage = ContentLanguage
	gtObj = AppUI.EkContentRef
	'''ap
	checkout = ""
    if (request.querystring("checkout") isnot nothing) then
        checkout = "&checkout=" & request.querystring("checkout").toString()
    end if

    if (checkout <> "") then
        'bCheckedout = gtObj.CheckoutEcmCollection(nId)
    	gtLinks = gtObj.GetEcmStageCollectionByID( nId, 0, 0, ErrorString,true, false, true)
    else
	    if (request.querystring("status") isnot nothing andalso request.querystring("status") = "o") then
    	    gtLinks = gtObj.GetEcmStageCollectionByID( nId, 0, 0, ErrorString,true, false, true)
	    else
    	    gtLinks = gtObj.GetEcmCollectionByID( nId, 0, 0, ErrorString,true, false, true)
    	end if
    end if

    if (request.querystring("status") isnot nothing) then
        if (request.querystring("status").toString.ToLower() = "o") then
            checkout = checkout & "&status=o"
        end if
    else
        if (gtLinks("ApprovalRequired") = true andalso gtLinks("Status").toString().toUpper() = "O") then
	        checkout = checkout & "&status=o"
	    end if
    end if

	if (ErrorString = "") then
		if (gtLinks.count) then
			CollectionTitle = gtLinks("CollectionTitle")
			gtLinks = gtLinks("Contents")
		end if
	end if
	reOrderList = ""
	if (ErrorString <> "") then
	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(MsgHelper.GetMessage("reorder collection title")))%>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<%
	else
	%>
		<form name="link_order" action="collectionaction.aspx?LangType=<%=ContentLanguage%>&action=DoUpdateOrder&nid=<%=(nId)%><%=checkout %>" method="post">
		    <input type="hidden" name="frm_folder_id" value="<%=(folderId)%>" />

		    <div class="ektronPageHeader">
		        <div class="ektronTitlebar">
		            <%=(GetTitleBar(MsgHelper.GetMessage("reorder collection title") & " """ & CollectionTitle & """"))%>
		        </div>
		        <div class="ektronToolbar">
			        <table>
				        <tr>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt: update collection order text"), MsgHelper.GetMessage("btn update"), "onclick=""return SubmitForm('link_order', '');"""))%>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?Action=View&nid=" & nId & "&folderid=" & folderId , MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					        <td><%=objStyle.GetHelpButton("ReOrderLinks")%></td>
				        </tr>
			        </table>
		        </div>
		    </div>

		    <div class="ektronPageContainer ektronPageInfo">
		        <div class="heightFix">
			    <table>
				    <tr>
					    <td>
						    <select name="OrderList" size="<%if (gtLinks.Count < 20) then Response.Write(gtLinks.Count) else Response.Write("20")%>">
							    <%
							    reOrderList = ""
							    for each gtNav in gtLinks
								    if (len(reOrderList))  then
									    reOrderList = reOrderList & "," & gtNav("ContentID") & "|" & gtNav("ContentLanguage")
								    else
									    reOrderList = gtNav("ContentID")& "|" & gtNav("ContentLanguage")
								    end if
								    if (IsNumeric(gtNav("ContentID"))) then
								    %>
									    <option value="<%=(gtNav("ContentID"))%>|<%=gtNav("ContentLanguage")%>"><%=(gtNav("ContentTitle"))%></option>
								    <%
								    end if
								    %>
							    <%
							    next
							    %>
						    </select>
					    </td>
					    <td>&nbsp;&nbsp;</td>
					    <td>
						    <a href="javascript:Move('up', document.link_order.OrderList, document.link_order.LinkOrder)">
						        <img src="<%=(AppPath)%>images/UI/Icons/arrowHeadUp.png" alt="<%=(MsgHelper.GetMessage("move selection up msg"))%>" title="<%=(MsgHelper.GetMessage("move selection up msg"))%>" />
						    </a>
						    <br />
						    <a href="javascript:Move('dn', document.link_order.OrderList, document.link_order.LinkOrder)">
						        <img src="<%=(AppPath)%>images/UI/Icons/arrowHeadDown.png" alt="<%=(MsgHelper.GetMessage("move selection down msg"))%>" title="<%=(MsgHelper.GetMessage("move selection down msg"))%>" />
						    </a>
					    </td>
				    </tr>
			    </table>
			    <input type="hidden" name="LinkOrder" value="<%=(reOrderList)%>" />
			    <input type="hidden" name="navigationid" value="<%=(nId)%>" />
			    </div>
			</div>
			<%
			if (len(reOrderList)) then
			%>
				<script type="text/javascript">
				    <!--//--><![CDATA[//><!--
					document.link_order.OrderList[0].selected = true;
					 //--><!]]>
				</script>
			<%
			end if
			%>
		</form>
	<%
	end if
elseif (action = "DeleteLink") then
	nId = request.QueryString("nid")
	AppUI.FilterByLanguage = ContentLanguage
	gtObj = AppUI.EkContentRef
	'''ap
	checkout = ""
    if (request.querystring("checkout") isnot nothing) then
        checkout = request.querystring("checkout").toString()
    end if
    if (checkout = "true" or request.querystring("status") = "o") then
        'bCheckedout = gtObj.CheckoutEcmCollection(nId)
    	gtLinks = gtObj.GetEcmStageCollectionByID( nId, 0, 0, ErrorString, true, false, true)
    else
    	gtLinks = gtObj.GetEcmCollectionByID( nId, 0, 0, ErrorString, true, false, true)
    end if
	if (ErrorString = "") then
		if (gtLinks.Count) then
			CollectionTitle = gtLinks("CollectionTitle")
			gtLinks = gtLinks("Contents")
		end if
	end if
	if (ErrorString <> "") then
	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(MsgHelper.GetMessage("delete collection items title")))%>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<%
	else
	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(MsgHelper.GetMessage("delete collection items title") & " """ & CollectionTitle & """"))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/remove.png", "#", MsgHelper.GetMessage("alt: delete collection items text"), MsgHelper.GetMessage("btn delete"), "onclick=""return SubmitForm('selections', 'GetIDs()');"""))%>
					    <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?Action=View&nid="&nId & "&folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("deletecollectionitems")%></td>
				    </tr>
			    </table>
		    </div>
		</div>

		<form name="selections" method="post" action="collectionaction.aspx?Action=doDeleteLinks&folderid=<%=(folderId)%>&nid=<%=(nId)%>&status=<%=(request.querystring("status")) %>">
		    <div class="ektronPageContainer ektronPageGrid">
		        <div class="heightFix">
                <table class="ektronGrid">
			        <tr class="title-header">
				        <td width="30%"><input type="checkbox" id="chkSelectAllRemove" onclick="selectClearAll(this);"/><%=(MsgHelper.GetMessage("generic Title"))%></td>
				        <td width="5%"><%=(MsgHelper.GetMessage("generic ID"))%></td>
				        <td><%=(MsgHelper.GetMessage("generic URL Link"))%></td>
			        </tr>
			        <%
			        lLoop = 0
			        cLinkArray = ""
			        fLinkArray = ""
			        cLanguagesArray=""
			        dim k as integer = 0
			        for k = 1 to gtLinks.count
			        %>
				        <tr>
					        <td>
						        <input type="checkbox" name="frm_check<%=(lLoop)%>" onclick="document.forms.selections['frm_hidden<%=(lLoop)%>'].value=(this.checked ? <%=(gtLinks(k)("ContentID"))%> : 0);$ektron('#chkSelectAllRemove')[0].checked=false;"/>
						        <input type="hidden" name="frm_languages<%=lloop%>" value="<%=gtLinks(k)("ContentLanguage")%>"/>
						        <input size="<%=(gtLinks(k)("ContentID"))%>" type="hidden" name="frm_hidden<%=(lLoop)%>" value="0"/>
						        <%=(getContentTypeIconAspx(gtLinks(k)("ContentType"),gtLinks(k)) & gtLinks(k)("ContentTitle"))%>
					        </td>
					        <td><%=(gtLinks(k)("ContentID"))%></td>
					        <td><%=(gtLinks(k)("ContentLinks"))%></td>
				        </tr>
				        <%
				        cLinkArray = cLinkArray & "," & gtLinks(k)("ContentID")
				        cLanguagesArray=cLanguagesArray & "," & gtLinks(k)("ContentLanguage")
				        fLinkArray = fLinkArray & "," & folderId
				        lLoop = lLoop + 1
			        next
			        if (len(cLinkArray)) then
				        cLinkArray = Right(cLinkArray, len(cLinkArray) - 1)
				        fLinkArray = Right(fLinkArray, len(fLinkArray) - 1)
				        cLanguagesArray = Right(cLanguagesArray, len(cLanguagesArray) - 1)
			        end if
			        %>
		        </table>
		        <br />
		        </div>
		    </div>
		    <script type="text/javascript" language="javascript">
		        <!--//--><![CDATA[//><!--
			    Collections = "<%=(cLinkArray)%>";
			    Folders = "<%=(fLinkArray)%>";
			    //--><!]]>
		    </script>
		    <input type="hidden" name="frm_content_ids" value=""/>
		    <input type="hidden" name="frm_folder_ids" value=""/>
		    <input type="hidden" name="frm_content_languages" value=""/>
		    <input type="hidden" name="CollectionID" value="<%=(nId)%>"/>
		</form>
	<%
	end if
elseif (action = "AddLink") then
	dim result as new System.Text.StringBuilder

	if (AddType <> "menu") then
	    nId = request.QueryString("nid")
	    AppUI.FilterByLanguage = ContentLanguage
	    gtObj = AppUI.EkContentRef
	    '''ap
	    checkout = ""
	    if (request.querystring("checkout") isnot nothing) then
	        checkout = request.querystring("checkout").toString()
	    end if
	    gtLinks = gtObj.GetEcmCollectionByID( nId, 0, 0, ErrorString, true, false, true)

	    if (checkout = "true") then
	        'if (gtLinks("Status").ToLower() <> "o") then
	        '    bCheckedout = gtObj.CheckoutEcmCollection(nId)
	        'end if
	        gtLinks = gtObj.GetEcmStageCollectionByID( nId, 0, 0, ErrorString, true, false, true)
	    end if
	    if (ErrorString = "") then
		    if (gtLinks.Count) then
			    CollectionTitle = gtLinks("CollectionTitle")
		    end if
	    end if
	end if

	 if (Request.Querystring("ancestorid") <> "")
	    AncestorIDParam = "&ancestorid=" & Request.Querystring("ancestorid")
	 end if
	 if (Request.Querystring("parentid") <> "")
	    ParentIDParam = "&parentid=" & Request.Querystring("parentid")
	 end if%>
	    <form name="selections"  method="post" action="" ID="selections" runat="server" >
	        <div class="ektronPageHeader">
	            <div class="ektronTitlebar">
		            <%if (AddType = "menu") then%>
			            <%=(GetTitleBar(MsgHelper.GetMessage("Add Menu Item Title") & " """ & MenuTitle & """"))%>
		            <%else%>
			            <%=(GetTitleBar(MsgHelper.GetMessage("add collection items title") & " """ & CollectionTitle & """"))%>
		            <%end if%>
		        </div>
		        <div class="ektronToolbar">
				    <table>
					    <tr>
						    <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt: add selected collection items text"), MsgHelper.GetMessage("btn add"), "onclick=""return SubmitForm('selections', 'GetIDs()');"""))%>
						    <%
						    if (AddType = "menu") then
				                if (Request.QueryString("back") <> "") then
					                Response.Write((GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", Request.QueryString("back"), MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "")))
							        if (CanCreateContent) then
									    response.write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentAdd.png", "#", MsgHelper.GetMessage("alt add content button text"), MsgHelper.GetMessage("btn add content"), "onclick=""PopUpWindow('editarea.aspx?LangType=" & ContentLanguage & "&type=add&id=" & locID & "', 'Edit', 790, 580, 1, 1);return false;"" " ))
								    end if
							    elseif (Request.QueryString("iframe") = "true") then
								    Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=""parent.CancelIframe();"""))
							        if (CanCreateContent) then
									    response.write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentAdd.png", "#", MsgHelper.GetMessage("alt add content button text"), MsgHelper.GetMessage("btn add content"), "onclick=""PopUpWindow('editarea.aspx?LangType=" & ContentLanguage & "&type=add&id=" & locID & "', 'Edit', 790, 580, 1, 1);return false;"" " ))
								    end if
							    else
								     if (CanCreateContent) then
									    response.write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentAdd.png", "#", MsgHelper.GetMessage("alt add content button text"), MsgHelper.GetMessage("btn add content"), "onclick=""PopUpWindow('editarea.aspx?LangType=" & ContentLanguage & "&type=add&id=" & locID & "', 'Edit', 790, 580, 1, 1);return false;"" " ))
								    end if
								    Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?LangType=" & ContentLanguage & "&Action=AddMenuItem&nid="& Request.QueryString("nid") & "&folderid=" & Request.QueryString("folderid") & NoWorkAreaAttribute & AncestorIDParam & ParentIDParam, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))
							    end if
						    else
							    if (CanCreateContent) then
								    response.write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentAdd.png", "#", MsgHelper.GetMessage("alt add content button text"), MsgHelper.GetMessage("btn add content"), "onclick=""PopUpWindow('editarea.aspx?LangType=" & ContentLanguage & "&type=add&id=" & locID & "', 'Edit', 790, 580, 1, 1);return false;"" " ))
							    end if
							    if( Request.QueryString("noworkarea") = "1" ) then
							        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=""self.close();return false;"" " ))
							    else
							        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?LangType=" & ContentLanguage & "&Action=View&nid="& Request.QueryString("nid") & "&folderid=" & Request.QueryString("folderid") , MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))
							    end if
						    end if
						    %>
						    <td>&nbsp;|&nbsp;</td>
						    <%
						    If (Not (IsNothing(asset_data))) Then
							    If (asset_data.Length > 0) Then
								    result.Append("<td><select id=selAssetSupertype name=selAssetSupertype OnChange=""UpdateView();"">")
								    If Ektron.Cms.Common.EkConstants.CMSContentType_NonLibraryContent = lContentType Then
									    result.Append("<option value='" & Ektron.Cms.Common.EkConstants.CMSContentType_NonLibraryContent & "' selected>All Types</option>")
								    Else
									    result.Append("<option value='" & Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes & "'>All Types</option>")
								    End If
								    If Ektron.Cms.Common.EkConstants.CMSContentType_Content = lContentType Then
									    result.Append("<option value='" & Ektron.Cms.Common.EkConstants.CMSContentType_Content & "' selected>HTML Content</option>")
								    Else
									    result.Append("<option value='" & Ektron.Cms.Common.EkConstants.CMSContentType_Content & "'>HTML Content</option>")
								    End If
								    For count = 0 To asset_data.Length - 1
									    If (Ektron.Cms.Common.EkConstants.ManagedAsset_Min <= asset_data(count).TypeId And asset_data(count).TypeId <= Ektron.Cms.Common.EkConstants.ManagedAsset_Max) Then
										    If "*" = asset_data(count).PluginType Then
											    lAddMultiType = asset_data(count).TypeId
										    Else
											    result.Append("<option value='" & asset_data(count).TypeId & "'")
											    If asset_data(count).TypeId = lContentType Then
												    result.Append(" selected")
												    bSelectedFound = True
											    End If
											    result.Append(">" & asset_data(count).CommonName & "</option>")
										    End If
									    End If
								    Next
								    result.Append("</select></td>")
							    End If
							    result.Append("<script language=""Javascript"">" & vbCrLf)
							    result.Append("<!--//" & vbCrLf)
							    result.Append("var replaceQueryString = """"" & vbCrLf)
							    result.Append("function BuildQueryString() {" & vbCrLf)
							    result.Append("    replaceQueryString = """"" & vbCrLf)
							    result.Append("    var search = location.href.split(""?"");" & vbCrLf)
							    result.Append("    if (search.length > 1){ " & vbCrLf)
							    result.Append("        var vals=search[1].split(""&"");" & vbCrLf)
							    result.Append("        var request= new Array(); " & vbCrLf)
							    result.Append("        for (var i in vals) { " & vbCrLf)
							    result.Append("            vals[i] = vals[i].replace(/\+/g, "" "").split(""=""); " & vbCrLf)
							    result.Append("            if (unescape(vals[i][0]).toLowerCase() != ""conttype"") { //we just ignore langtype." & vbCrLf)
							    result.Append("                if (replaceQueryString == """") { replaceQueryString = unescape(vals[i][0]) + ""="" + (vals[i][1]) } " & vbCrLf)
							    result.Append("                else { replaceQueryString += ""&"" +  unescape(vals[i][0]) + ""="" + (vals[i][1])}" & vbCrLf)
							    result.Append("            }" & vbCrLf)
							    result.Append("            request[unescape(vals[i][0])] =unescape(vals[i][1]); " & vbCrLf)
							    result.Append("        }" & vbCrLf)
							    result.Append("    }" & vbCrLf)
							    result.Append("}" & vbCrLf)
							    result.Append("BuildQueryString();" & vbCrLf)
							    result.Append("//-->" & vbCrLf)
							    result.Append("</script>" & vbCrLf)
							    response.Write(result.tostring())
						    End If
						    %>
						    <td>
							    <%if (AddType = "menu") then%>
								    <%=objStyle.GetHelpButton("AddMenuItems")%>
							    <%else%>
								    <%=objStyle.GetHelpButton("AddCollectionItems")%>
							    <%end if%>
						    </td>
					    </tr>
				    </table>
		        </div>
		    </div>

	        <div class="ektronPageContainer ektronPageInfo">
	            <div class="heightFix">
	            <asp:DataGrid ID="ContentGrid"
	                runat="server"
	                OnItemDataBound="Grid_ItemDataBound"
	                AutoGenerateColumns="False"
	                Width="100%"
	                GridLines="None"
		            AllowPaging="False"
		            AllowCustomPaging="True"
		            PageSize="10"
		            EnableViewState="False"
		            PagerStyle-Visible="False">
	            </asp:DataGrid>

	            <p class="pageLinks">
		            <asp:Label runat="server" id="PageLabel" >Page</asp:Label>
		            <asp:Label id="CurrentPage" CssClass="pageLinks" runat="server" />
		            <asp:Label runat="server" id="OfLabel" >of</asp:Label>
		            <asp:Label id="TotalPages" CssClass="pageLinks" runat="server" />
	            </p>
	            <asp:LinkButton runat="server" CssClass="pageLinks"
	            id="FirstPage" Text="[First Page]"
	            OnCommand="NavigationLink_Click" CommandName="First" />
	            <asp:LinkButton runat="server" CssClass="pageLinks"
	            id="lnkBtnPreviousPage" Text="[Previous Page]"
	            OnCommand="NavigationLink_Click" CommandName="Prev" />
	            <asp:LinkButton runat="server" CssClass="pageLinks"
	            id="NextPage" Text="[Next Page]"
	            OnCommand="NavigationLink_Click" CommandName="Next" />
	            <asp:LinkButton runat="server" CssClass="pageLinks"
	            id="LastPage" Text="[Last Page]"
	            OnCommand="NavigationLink_Click" CommandName="Last" />
	            </div>
	        </div>
		<script type="text/javascript" language="javascript">
			Collections = "<%=(cLinkArray)%>";
			Folders = "<%=(fLinkArray)%>";
		</script>
		<input type="hidden" name="frm_content_ids" value=""/>
		<input type="hidden" name="frm_content_languages" value=""/>
		<input type="hidden" name="frm_folder_ids" value=""/>
        <input type="hidden" name="frm_back" value="<%=Request.QueryString("back")%>">
		<input type="hidden" name="CollectionID" value="<%=(Request.QueryString("nid"))%>"/>
		<asp:Literal id="postbackaction"  Runat="server" />
		<input type="hidden" runat="server" id="isPostData" value="true" />
	</form>
<% elseif(action="ViewCollectionReport" or action="ViewMenuReport") then%>
    <form id="frmCollectionList" runat="server" >
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageGrid">
            <div class="heightFix">
                <asp:GridView
                    ID="CollectionListGrid"
                    runat="server"
                    AutoGenerateColumns="False"
                    EnableViewState="False"
                    Width="100%"
                    CssClass="ektronGrid"
                    GridLines="None">
                    <HeaderStyle CssClass="title-header" />
                </asp:GridView>

                <p class="pageLinks">
                <asp:Label runat="server" ID="cPageLabel">Page</asp:Label>
                <asp:Label ID="cCurrentPage" CssClass="pageLinks" runat="server" />
                <asp:Label runat="server" ID="cOfLabel">of</asp:Label>
                <asp:Label ID="cTotalPages" CssClass="pageLinks" runat="server" />
            </p>

                <asp:LinkButton runat="server" CssClass="pageLinks" ID="cFirstPage" Text="[First Page]"
                    OnCommand="CollectionNavigationLink_Click" CommandName="First" OnClientClick="resetCPostback()" />
                <asp:LinkButton runat="server" CssClass="pageLinks" ID="cPreviousPage" Text="[Previous Page]"
                    OnCommand="CollectionNavigationLink_Click" CommandName="Prev" OnClientClick="resetCPostback()" />
                <asp:LinkButton runat="server" CssClass="pageLinks" ID="cNextPage" Text="[Next Page]"
                    OnCommand="CollectionNavigationLink_Click" CommandName="Next" OnClientClick="resetCPostback()" />
                <asp:LinkButton runat="server" CssClass="pageLinks" ID="cLastPage" Text="[Last Page]"
                    OnCommand="CollectionNavigationLink_Click" CommandName="Last" OnClientClick="resetCPostback()" />
                <input type="hidden" runat="server" id="isCPostData" value="true" />
                <input type="hidden" runat="server" id="isSearchPostData" value="" />
                <asp:Literal ID="litRefreshAccordion" runat="server" />
            </div>
        </div>
    </form>
<%
elseif (action = "ViewMenu") then

	nId = Request.QueryString("nid")
	folderId = Request.QueryString("folderid")
	If m_refApi.TreeModel = 1
		Server.Transfer("menutree.aspx?nid=" & nId & "&folderid="& folderId & "&LangType=" & ContentLanguage)
	Else
		Dim MenuTitle As String = String.Empty
		Dim ParentMenuId, AncestorMenuID, HighLightColor As Object
		Dim MenuXML As String = String.Empty
		' This is the hex value for the highlight color.

		AncestorMenuID = nothing
		ParentMenuId = nothing

	    HighLightColor = "DDDDDD"
		gtObj = AppUI.EkContentRef
		gtNavs = gtObj.GetMenuByID(nId, 0)
		if (ErrorString = "") then
			if (gtNavs.Count) then
				MenuXML = p_MenuXML(gtNavs, nId, MenuXML)
				MenuTitle = gtNavs("MenuTitle")
				folderId = gtNavs("FolderID")
				ParentMenuId=gtNavs("MenuID")
				AncestorMenuId=gtNavs("AncestorMenuId")
				If (AncestorMenuId=0) Then
					AncestorMenuId=ParentMenuId
				End If
			end if
		end if
		if (ErrorString <> "") then
		%>
		    <div class="ektronPageHeader">
		        <div class="ektronTitlebar">
			        <%=(GetTitleBar(MsgHelper.GetMessage("View Menu Title")))%>
			    </div>
			    <div class="titlebar-error"><%=(ErrorString)%></div>
			</div>
		<%
		else
		%>
			<script type="text/javascript">
			<!--//--><![CDATA[//><!--
			function onMenuItemEdit(ItemId, MenuId, ItemType) {
				if (MenuId == "") {
					MenuId = '<%=nId%>';
				}
				LoadChildPage("submenuedititem", ItemId, MenuId, ItemType);
				//alert(ItemId + ", " + MenuId + ', ' + ItemType);
			}
			function onMenuItemDelete(ItemId, MenuId, ItemType) {
				var lItemType;
				if (MenuId == "") {
					MenuId = '<%=nId%>';
				}
				if (confirm(MsgHelper.Getmessage("alt Are you sure you want to delete this item?"))){
					document.forms.netscapefix.frm_content_ids.value = ItemId + '.' + 0;
					document.forms.netscapefix.CollectionID.value = MenuId;
					document.forms.netscapefix.action = "collectionaction.aspx?iframe=true&action=doDeleteMenuItem&folderid=<%=folderid%>&nid=<%=nId%>";
					document.forms.netscapefix.submit();
				}
				return false;
			}
			function onSubMenuAddItem(MenuId, FolderId) {
				if (MenuId == "") {
					alert("Please select a menu.");
					return false;
				}
				LoadChildPage("submenuadditem", FolderId, MenuId, "");
				//document.location.href = "collections.aspx?iframe=true&action=AddMenuItem&nid=" + MenuId + "&folderid=" + FolderId + "&parentid=" + MenuId + "&ancestorid=<%=AncestorMenuId%>";
			}
			function onSubMenuEdit(MenuId) {
				if (MenuId == "") {
					MenuId = '<%=nId%>';
				}
				LoadChildPage("submenuedit", "", MenuId, "");
				//alert(MenuId);
			}
			function onSubMenuDelete(MenuId) {
				if (MenuId == "") {
					MenuId = '<%=nId%>';
				}
				if (confirm(msghelper.getmessage("alt Are you sure you want to delete this item?"))){
					document.forms.netscapefix.frm_content_ids.value = MenuId + '.' + 4 + '.' + MenuId;
					document.forms.netscapefix.CollectionID.value = MenuId;
					document.forms.netscapefix.action = "collectionaction.aspx?iframe=true&action=doDeleteMenuItem&folderid=<%=folderid%>&nid=<%=nId%>";
					document.forms.netscapefix.submit();
				}
			}
			function onSubMenuOrderItem(MenuId, FolderId) {
				LoadChildPage("submenuorderitem", FolderId, MenuId, "");
			}
			function LoadChildPage(tAction, ItemId, MenuId, ItemType) {
				var frameObj = document.getElementById("ChildPage");
				if (tAction == "submenuedit") {
					frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=EditMenu&nid=" + MenuId + "&folderid=" + '<%=folderid%>&Ty=' + ItemType;
				} else if (tAction == "submenuedititem") {
					frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=EditMenuItem&nid=" + MenuId + "&folderid=<%=folderid%>&id=" + ItemId + '&Ty=' + ItemType;
				} else if (tAction == "submenuadditem") {
					frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=AddMenuItem&nid=" + MenuId + "&folderid=" + ItemId + "&parentid=" + MenuId + "&ancestorid=<%=AncestorMenuId%>";
				} else if (tAction == "submenuorderitem") {
					frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=ReOrderMenuItems&nid=" + MenuId + "&folderid=" + ItemId;
				}


				var pageObj = document.getElementById("FrameContainer");
				pageObj.style.display = "";
				pageObj.style.width = "90%";
				pageObj.style.height = "90%";

			}
			//--><!]]>
			</script>
			<form name="netscapefix" method="post" action="#" ID="Form3">
			<div id="FrameContainer" class="ektronBorder" style="POSITION: absolute; TOP: 48px; LEFT: 40px; WIDTH: 1px; HEIGHT: 1px; DISPLAY: none;">
				<iframe id="ChildPage" src="javascript:false;" class name="ChildPage" frameborder="yes" marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="auto">
				</iframe>
			</div>
			<input type="hidden" name="CollectionID" id="CollectionID" value=""/>
			<input type="hidden" name="frm_content_ids" id="frm_content_ids" value=""/>

			<div class="ektronPageHeader">
			    <div class="ektronTitlebar">
			        <%=(GetTitleBar(MsgHelper.GetMessage("View Menu Title") & " """ & MenuTitle & """"))%>
			    </div>
			    <div class="ektronToolbar">
				    <table width="100%">
					    <tr>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "collections.aspx?LangType=" & ContentLanguage & "&action=AddMenuItem&nid=" & nId & "&folderid=" & folderId & "&parentid=" & ParentMenuID & "&ancestorid=" & AncestorMenuId, MsgHelper.GetMessage("alt add new item"), MsgHelper.GetMessage("btn add"), ""))%>
					        <%
					        if (gtNavs("Items").Count > 1) then
					        %>
						        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/remove.png", "collections.aspx?action=DeleteMenuItem&nid="& nId & "&folderid=" & folderId , MsgHelper.GetMessage("alt remove item"), MsgHelper.GetMessage("btn minus"), ""))%>
		 				        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/arrowUpDown.png", "collections.aspx?action=ReOrderMenuItems&nid=" & nId & "&folderid=" & folderId, MsgHelper.GetMessage("alt reorder items"), MsgHelper.GetMessage("btn reorder"), ""))%>
		 			        <%
		 			        end if
		 			        %>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "collections.aspx?action=EditMenu&nid=" & nId & "&folderid="& folderId, MsgHelper.GetMessage("alt edit menu"), MsgHelper.GetMessage("btn edit"), ""))%>
					        <%if (CLng(gtNavs("ParentMenuId"))>0) then
						        Callbackpage = "&callbackpage=collections.aspx&parm1=action&value1=ViewMenu&parm2=nid&value2=" & gtNavs("ParentMenuId") & "&parm3=folderid&value3=" & Request.querystring("pfid")
					        elseif((Request.QueryString("bpage") = "reports") OR ((Request.QueryString("bpage") = "ViewMenuReport")))then
						        Callbackpage = "&callbackpage=collections.aspx&parm1=action&value1=ViewMenuReport"
					        end if%>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "collectionaction.aspx?action=doDeleteMenu&nId=" & nId & "&folderid=" & folderId & Callbackpage, MsgHelper.GetMessage("alt delete menu"), MsgHelper.GetMessage("btn delete"), "onclick="" return ConfirmMenuDelete();"""))%>
					        <%if (Request.QueryString("bpage") = "reports") then %>
						        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewMenuReport", MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					        <%elseif (CLng(gtNavs("ParentMenuId"))>0) then %>
						        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewMenu&nid=" & gtNavs("ParentMenuId") & "&folderid=" & Request.QueryString("pfid"), MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					        <%else %>
						        <%If ((Request.QueryString("bPage")="ViewMenuReport") OR (Request.QueryString("bPage")="reports"))Then%>
							        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewMenuReport&folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
						        <%Else%>
							        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewAllMenus&folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
						        <%End If%>
					        <%end if %>
					        <td >
					        <%
					        if not gtNavs("IsSubMenu") then
						        Dim addDD As String
						        addDD = ViewLangsForMenuID(nId, "", False, False, "javascript:addBaseMenu(" & nId & ", " & ParentMenuId & ", " & AncestorMenuId & ", " & FolderID & ", this.value);")

						        If addDD <> "" Then
							        addDD = "&nbsp;" & (new Ektron.Cms.ContentAPI).EkMsgRef.GetMessage("add title") & ":&nbsp;" & addDD
						        End If
						        If (Cstr(EnableMultiLingual = "1")) Then %>
						        View In:&nbsp;<%= ViewLangsForMenuID(nId, "", True, False, "javascript:LoadLanguage(this.value);") %>&nbsp;<%= addDD %><br />
						        <% End If %>
					        <% End If 'ParentMenuID = 0 %>
					        </td>
					        <td><%=objStyle.GetHelpButton("ViewMenu")%></td>
					    </tr>
				    </table>
			    </div>
			</div>
			<br />
			<table width="100%">
				<tr>
				    <td>
			        <%
				        if (MenuXML <> "") then
					        MenuXML = "<navigation>" &  MenuXML & "</navigation>"
					        MenuXML = AppUI.TransformXSLT(MenuXML, Server.MapPath(AppPath & "cmsmenuapi.xsl"))
				        end if
				        Response.write(MenuXML)
			        %>
			        </td>
			    </tr>
			</table>

			<!--p onclick="toggleElementDisplay('moreInfo2')"><strong><span class="moreinfo" style="color:blue;cursor:hand;"><u><%=(MsgHelper.GetMessage("More info Link"))%></u></span></strong></p-->
			<div class="switchcontent" name="sc1" id="moreInfo2">
				<table class="ektronForm">
					<tr>
						<td class="label"><%=(MsgHelper.GetMessage("generic title label"))%></td>
						<td><%=(gtNavs("MenuTitle"))%></td>
					</tr>
					<tr>
						<td class="label"><%=(MsgHelper.GetMessage("id label"))%></td>
						<td><%=(gtNavs("MenuID"))%></td>
					</tr>
					<tr>
						<td class="label"><%=(MsgHelper.GetMessage("generic Path"))%>:</td>
						<td><%=(gtNavs("Path"))%></td>
					</tr>
					<tr>
						<td class="label"><%=(MsgHelper.GetMessage("content LUE label"))%></td>
						<td><%=(gtNavs("EditorFName") & " " & gtNavs("EditorLName"))%></td>
					</tr>
					<tr>
						<td class="label"><%=(MsgHelper.GetMessage("content LED label"))%></td>
						<td><%=(gtNavs("DisplayLastEditDate"))%></td>
					</tr>
					<tr>
						<td class="label"><%=(MsgHelper.GetMessage("content DC label"))%></td>
						<td><%=(gtNavs("DisplayDateCreated"))%></td>
					</tr>
					<tr>
						<td class="label"><%=(MsgHelper.GetMessage("description label"))%></td>
						<td><%=(gtNavs("MenuDescription"))%></td>
					</tr>
				</table>
			</div>
			</form>
		<%
		end if
		gtObj = nothing
		gtNavs = nothing
		gtLinks = nothing
	   End If
elseif (action = "AddMenuItem") then
    dim MenuId, mpID, maID As Object
    FolderId = Request.QueryString("folderid")
    MenuId = Request.QueryString("nId")
    mpID = Request.QueryString("parentid")
	maID = Request.QueryString("ancestorid")
    Dim enableQDOparam As Object
    enableQDOparam = ""
	gtObj = AppUI.EkContentRef
    if (MenuId <> "") then
        gtNavs = gtObj.GetMenuByID(MenuId, 0)
        if (gtNavs.Count > 0) then
            if (gtNavs("EnableReplication")) then
                enableQDOparam = "&qdo=1"
            end if
        end if
    end if
    if (ErrorString <> "") then
    %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
                <%=(GetTitleBar(MsgHelper.GetMessage("Add Menu Item Title")))%>
            </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
				        <%if (Request.QueryString("noworkarea") <> "1") then%>
				        <%if(m_refApi.TreeModel = 1) Then%>
				        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "menutree.aspx?nid=" & MenuId & "&folderid=" & FolderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
				        <%Else%>
				        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewMenu&nId=" & MenuId & "&folderid=" & FolderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
				        <%End If%>
				        <%end if%>
				        <td><%=objStyle.GetHelpButton("AddMenuItem")%></td>
				    </tr>
			    </table>
		    </div>
		</div>
		<div class="info"><%=(MsgHelper.GetMessage("Add Menu Item Title"))%></div>
		<div class="titlebar-error"><%=(ErrorString)%></div>
    <%
    else
    %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
                <%=(GetTitleBar(MsgHelper.GetMessage("Add Menu Item Title")))%>
            </div>
		    <div class="ektronToolbar">
		        <table>
			        <tr>
			            <% if (Request.QueryString("back") <> "") then %>
				            <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", Request.QueryString("back"), MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), ""))%>
			            <% elseif (Request.QueryString("iframe") = "true") then %>
			  	            <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=""parent.CancelIframe();"""))%>
			            <% else %>
				            <%if (Request.QueryString("noworkarea") <> "1") then%>
				            <% If(m_refApi.TreeModel = 1) Then %>

					            <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "menutree.aspx?nid=" & MenuId & "&folderid=" & FolderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					            <%else%>
				            <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewMenu&nId=" & MenuId & "&folderid=" & FolderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
				            <%end if %>

				            <%end if%>
			            <% end if %>
			            <td><%=objStyle.GetHelpButton("AddMenuItem")%></td>
			        </tr>
		        </table>
		    </div>
		</div>

        <form name="AddMenuItem" action="collections.aspx?LangType=<%=ContentLanguage%>&action=pAddMenuItem&nId=<%=(MenuId)%>&folderid=<%=FolderId%>&parentid=<%=mpID%>&ancestorid=<%=maID%>&iframe=<%=Request.QueryString("iframe")%>&back=<%=Server.URLEncode(Request.QueryString("back"))%><%=NoWorkAreaAttribute%><%=enableQDOparam%>" method="post">
				<script language="javascript" type="text/javascript">
				function test_new_cb() {
					var test_new_cb;
					<% if (Request.QueryString("noworkarea") = 1) then %>
						test_new_cb = 1;
					<% else %>
						test_new_cb = 0;
					<% end if %>
					if (test_new_cb == 1) {
						if (document.AddMenuItem.ItemType[1].checked)  {
							location.href = "collectiontree.aspx?action=AddLink&addto=menu&noworkarea=1&nid=<%=(MenuId)%>&folderid=<%=FolderId%>&LangType=<%=ContentLanguage%>";
						} else {
							document.AddMenuItem.submit();
						}
					} else {
					document.AddMenuItem.submit();
					}
				}
			</script>
		    <input type="hidden" name="frm_back" value="<%=(Request.QueryString("back"))%>"/>

            <div class="ektronPageContainer ektronPageInfo">
                <div class="heightFix">
			        <input type="radio" name="ItemType" checked value="content" /><%=(MsgHelper.GetMessage("lbl content item"))%>
		            <br />
		            <% if (Request.QueryString("noworkarea") = 1) then %>
			            <input type="radio" name="ItemType" value="newcontent" /><%=(MsgHelper.GetMessage("lbl new content block"))%>
			            <br />
		            <% end if %>
		            <input type="radio" name="ItemType" value="library" /><%=(MsgHelper.GetMessage("Library Asset label"))%>
		            <br />
		            <input type="radio" name="ItemType" value="link" /><%=(MsgHelper.GetMessage("External Hyperlink label"))%>
		            <br />
		            <input type="radio" name="ItemType" value="submenu" /><%=(MsgHelper.GetMessage("Sub Menu Label"))%>
                    <div class="ektronTopSpace"></div>
		            <input name="next" type="button" value="Next..." onclick="test_new_cb();"/>
		        </div>
		    </div>
		</form>
        <%
    end if
elseif (action = "pAddMenuItem") then
'This is private action raise from AddMenuItem
	mpID = Request.QueryString("parentid")
	maID = Request.QueryString("ancestorid")
	if (Request.Querystring("ancestorid") <> "")
	    AncestorIDParam = "&ancestorid=" & Request.Querystring("ancestorid")
	 end if
	 if (Request.Querystring("parentid") <> "")
	    ParentIDParam = "&parentid=" & Request.Querystring("parentid")
	 end if
    dim ItemType, gtFolderInfo, FolderPath, MenuId As Object
    FolderId = Request.QueryString("folderid")
    MenuId = Request.QueryString("nId")
    ItemType = Request.Form("ItemType")
    dim enableQDOparam As Object
	gtObj = AppUI.EkContentRef
    if (mpID <> "") then
        gtNavs = gtObj.GetMenuByID(mpID, 0)
        if (gtNavs.Count > 0) then
            if (gtNavs("EnableReplication")) then
                enableQDOparam = "&qdo=1"
            end if
        end if
    end if
    if (ItemType = "content") then
		Response.Redirect("collections.aspx?action=AddLink&addto=Menu&folderid=" & FolderId & "&nid=" & MenuId & "&LangType=" & ContentLanguage & "&iframe=" & Request.QueryString("iframe") & NoWorkAreaAttribute & AncestorIDParam & ParentIDParam & "&back=" & Server.URLEncode(Request.QueryString("back")) & enableQDOparam)
	elseif (ItemType = "submenu") then
		''''''Response.Redirect("collectiontree.aspx?action=AddLink&addto=submenu&folderid=0&nid=" & MenuId & "&parentid=" & mpID & "&ancestorid=" & maID & "&LangType=" & ContentLanguage)

		Dim enableReplicationFlag as String = String.Empty
		If (Not IsNothing(gtNavs) AndAlso Not IsNothing(gtNavs("EnableReplication"))) Then
	        enableReplicationFlag = gtNavs("EnableReplication")
		End If
		Response.Redirect("collections.aspx?action=AddSubMenu&folderid=" & gtNavs("FolderId") & "&nId=" & MenuId & "&parentid=" & mpID & "&ancestorid=" & maID & "&LangType=" & ContentLanguage & "&iframe=" & Request.QueryString("iframe") & NoWorkAreaAttribute & "&back=" & Server.URLEncode(Request.QueryString("back")) & "&QD=" & enableReplicationFlag)
	elseif (ItemType = "library") then
        gtFolderInfo = gtObj.GetFolderInfoWithPath(FolderId)
        FolderPath = gtFolderInfo("Path")
       	if (Right(FolderPath, 1) = "\") then
			FolderPath = Left(FolderPath, Len(FolderPath) - 1)
		end if
		FolderPath = Replace(FolderPath, "\", "\\")

        %>
            <div class="ektronPageHeader">
                <div class="ektronTitlebar">
                    <%=(GetTitleBar(MsgHelper.GetMessage("Add Menu Item Title")))%>
                </div>
			    <div class="ektronToolbar">
				    <table>
					    <tr>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt Save Menu Item"), MsgHelper.GetMessage("btn save"), "onclick=""return SubmitForm('AddMenuItem', 'VerifyLibraryAssest()');"""))%>
			                <% if (Request.QueryString("back") <> "") then %>
				                <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", Request.QueryString("back"), MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), ""))%>
					        <% elseif (Request.QueryString("iframe") = "true") then %>
						        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=""parent.CancelIframe();"""))%>
					        <% else %>
						        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?LangType=" & ContentLanguage & "&action=AddMenuItem&nId=" & MenuId & "&folderid=" & FolderId & NoWorkAreaAttribute, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					        <% end if %>
					        <td><%=objStyle.GetHelpButton("pAddMenuItem")%></td>
					    </tr>
				    </table>
			    </div>
			</div>

            <form name="AddMenuItem" action="collectionaction.aspx?action=doAddMenuItem&type=library&LangType=<%=ContentLanguage%>&iframe=<%=Request.QueryString("iframe")%><%=NoWorkAreaAttribute%>" method="post">
                <div class="ektronPageContainer ektronPageInfo">
                    <div class="heightFix">
			            <table class="ektronGrid">
				            <tr>
					            <td class="label"><%=(MsgHelper.GetMessage("generic title label"))%></td>
					            <td class="value"><input type="text" name="title" /></td>
					            <td>&nbsp;</td>
					            <td><input type="button" name="Browse" value="<%=(MsgHelper.GetMessage("Browse Library Button"))%>" onclick="PopBrowseWin('images,hLink,files', '<%=FolderPath%>', null, '<%=enableQDOparam%>');return false;" /></td>
				            </tr>
			            </table>
			        </div>
			    </div>
			    <input type="hidden" name="FolderID" value="<%=(FolderId)%>"/>
			    <input type="hidden" name="CollectionID" value="<%=(MenuId)%>"/>
			    <input type="hidden" name="DefaultTitle" value=""/>
			    <input type="hidden" name="id" value=""/>
		    <input type="hidden" name="frm_back" value="<%=(Request.QueryString("back"))%>"/>
			</form>
			<script type="text/javascript" language="Javascript">
			    <!--//--><![CDATA[//><!--
			    PopBrowseWin('images,hLink,files', '<%=FolderPath%>', null, '<%=enableQDOparam%>');
			    //--><!]]>
			</script>
        <%
    else
        if (ErrorString <> "") then
        %>
            <div class="ektronPageHeader">
                <div class="ektronTitlebar">
                    <%=(GetTitleBar(MsgHelper.GetMessage("Add Menu Item Title")))%>
                </div>
			    <div class="ektronToolbar">
				    <table>
					    <tr>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=AddMenuItem&nId=" & MenuId & "&folderid=" & FolderId & "&LangType=" & ContentLanguage & NoWorkAreaAttribute, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					        <td><%=objStyle.GetHelpButton("AddCollection")%></td>
					    </tr>
				    </table>
			    </div>
			</div>
			<div class="info"><%=(MsgHelper.GetMessage("Add Menu Item Title"))%></div>
			<div class="titlebar-error"><%=(ErrorString)%></div>
        <%
        else
        %>
            <div class="ektronPageHeader">
                <div class="ektronTitlebar">
                    <%=(GetTitleBar(MsgHelper.GetMessage("Add Menu Item Title")))%>
                </div>
			    <div class="ektronToolbar">
				    <table>
					    <tr>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt Save Menu Item"), MsgHelper.GetMessage("btn save"), "onclick=""return SubmitForm('AddMenuItem', 'VerifyAddMenuItem()');"""))%>
			                <% if (Request.QueryString("back") <> "") then %>
				                <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", Request.QueryString("back"), MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), ""))%>
					        <% elseif (Request.QueryString("iframe") = "true") then %>
						        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=""parent.CancelIframe();"""))%>
					        <% else %>
						        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=AddMenuItem&nId=" & MenuId & "&folderid=" & FolderId & NoWorkAreaAttribute, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					        <% end if %>
					        <td><%=objStyle.GetHelpButton("pAddMenuItem")%></td>
					    </tr>
				    </table>
			    </div>
			</div>
            <div class="ektronPageContainer ektronPageInfo">
			<form name="AddMenuItem" action="collectionaction.aspx?action=doAddMenuItem&type=link&langType=<%=ContentLanguage%>&iframe=<%=Request.QueryString("iframe")%><%=NoWorkAreaAttribute%>" method="post">
				<input type="hidden" name="FolderID" value="<%=(FolderId)%>"/>
				<input type="hidden" name="CollectionID" value="<%=(MenuId)%>"/>
		        <input type="hidden" name="frm_back" value="<%=(Request.QueryString("back"))%>"/>
			    <table width="100%">
				    <tr>
					    <td class="info" nowrap><%=(MsgHelper.GetMessage("generic title label"))%></td><td><input type="text" name="Title" value=""></td>
				    </tr>
				    <tr>
					    <td class="info" nowrap valign="top"><%=(MsgHelper.GetMessage("lbl link"))%>:</td><td><input type="text" name="Link" value="">
					    </td>
				    </tr>
				    <tr>
				    <td>&nbsp;</td>
				    <td class="info">
					    <br/> <%=(MsgHelper.GetMessage("lbl Examples"))%>: <br/><%=(MsgHelper.GetMessage("lbl external link"))%>: http://www.ektron.com   <br /><%=(MsgHelper.GetMessage("lbl Root of the web site"))%>: /news/pr.aspx   <br/><%=(MsgHelper.GetMessage("generic relative"))%>: pr.aspx
				    </td></tr>
			    </table>
			</form>
			</div>
            <%
        end if
    end if
elseif (action = "ViewAllMenus") then
	OrderBy = request.queryString("OrderBy")
	if (OrderBy = "") then
		OrderBy = "title"
	end if
    folderId = Request.QueryString("folderid")
	gtObj = AppUI.EkContentRef
    gtNavs = gtObj.GetAllMenusInfo(folderId, OrderBy)
	if (len(ErrorString) = 0) then
		gtNav = gtObj.GetFolderInfov2_0( folderId)
		if (len(ErrorString) = 0) then
			FolderName = gtNav("FolderName")
		end if
	end if
	if (ErrorString <> "") then
	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(MsgHelper.GetMessage("view all menu title")))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
					<tr>
				        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewAllMenus&folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("ViewAllMenus")%></td>
				    </tr>
			    </table>
		    </div>
		</div>
		<div class="info"><%=(MsgHelper.GetMessage("generic page error message"))%></div>
		<div class="titlebar-error"><%=(ErrorString)%></div>
	<%
	else
	%><form name="frmViewMenus" action="collections.aspx" method="post">
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(MsgHelper.GetMessage("view all menu title") & " """ & FolderName & """"))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
				        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "collections.aspx?action=AddMenu&folderid=" & folderId & "&LangType=" & ContentLanguage , MsgHelper.GetMessage("alt add new menu"), MsgHelper.GetMessage("btn add"), ""))%>
				        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "content.aspx?Action=ViewContentByCategory&id="& folderId , MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
				      <td>|&nbsp;</td>
				        <%
				        Dim idContainer As Object=""
				        if (gtNavs.Count) then
					        For Each gtNav IN gtNavs
						        idContainer = idContainer & gtNav("MenuID") & ","
					        Next
					        idContainer = Left(idContainer,Len(idContainer)-1)
				        End If
				        If ((idContainer<>"") and (EnableMultilingual="1")) Then %>
				        <td nowrap="nowrap">
					        View In:&nbsp;<%= ViewLangsForMenuID(idContainer, "", True, True, "javascript:LoadLanguage(this.value);") %>&nbsp;<br />
				        </td>
				        <%Else
					        If(EnableMultilingual="1") Then%>
					            <td nowrap="nowrap">View In:&nbsp;
						            <%
						            Dim Language As Object
						            Dim colActiveLanguages As Object = gtMsgObj.GetAllActiveLanguages()
						            Response.write("<select id=selLang name=selLang onchange=""LoadLanguageMenus('frmViewMenus');"">")
						            If ContentLanguage=-1 Then
							            response.write("<option value=" & ALL_CONTENT_LANGUAGES & " selected>" & MsgHelper.GetMessage("lbl abbreviation for all the words") & "</option>")
						            Else
							            response.write("<option value=" & ALL_CONTENT_LANGUAGES & ">" & MSgHelper.GetMessage("lbl abbreviation for all the words") & "</option>")
						            End If

						            For Each Language In colActiveLanguages
							            If CStr(ContentLanguage)=CStr(Language.value("ID")) Then
								            response.write("<option value=" & Language.value("ID") & " selected>" &  Language.value("Name") & "</option>")
							            Else
								            response.write("<option value=" & Language.value("ID") & ">" &  Language.value("Name") & "</option>")
							            End If
						            Next
						            Response.Write("</select>")
						            %>
					                <script type="text/javascript">
					                <!--//--><![CDATA[//><!--
					                function LoadLanguageMenus(FormName){
						                var num=document.forms[FormName].selLang.selectedIndex;
						                document.forms[FormName].action="collections.aspx?folderid=<%=Request.QueryString("folderid")%>&action=<%=Request.QueryString("action")%>&LangType="+document.forms[FormName].selLang.options[num].value;
						                document.forms[FormName].submit();
						                return false;
					                }
					                //--><!]]>
					                </script>
					            </td>
					        <%End If%>
				        <% End If %>
				        <td><%=objStyle.GetHelpButton(action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>
		<div class="ektronPageContainer ektronPageGrid">
		    <table class="ektronGrid" width="100%">
			    <tr class="title-header">
				    <td width="30%"><a href="collections.aspx?folderid=<%=(folderId)%>&OrderBy=navname&action=ViewAllMenus" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>"><%=(MsgHelper.GetMessage("generic Title"))%></a></td>
				    <td width="5%"><a href="collections.aspx?folderid=<%=(folderId)%>&OrderBy=collectionid&action=ViewAllMenus" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>"><%=(MsgHelper.GetMessage("generic ID"))%></a></td>
				    <td wrap="nowrap"><%=MsgHelper.GetMessage("lbl Language ID")%></td>
				    <td><a href="collections.aspx?folderid=<%=(folderId)%>&OrderBy=date&action=ViewAllMenus" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>"><%= (MsgHelper.GetMessage("generic Date Modified")) %></a></td>
			    </tr>
		    <%
            if (gtNavs.Count) then
                dim l as integer = 0
                For l = 1 to gtNavs.count
                %>
                    <tr>
                        <% If(m_refApi.TreeModel = 1) Then %>
				            <td><a href="menu.aspx?Action=viewcontent&treeviewid=-3&LangType=<%=gtNavs(l)("ContentLanguage")%>&folderid=<%=(folderId)%>&menuid=<%=(gtNavs(l)("MenuID"))%>" alt='<%=(MsgHelper.GetMessage("generic View") & " """ & replace(gtNavs(l)("MenuTitle"), "'", "`") & """")%>' title='<%=(MsgHelper.GetMessage("generic View") & " """ & replace(gtNavs(l)("MenuTitle"), "'", "`") & """")%>'><%=(gtNavs(l)("MenuTitle"))%></a></td>
				        <%else%>
				            <td><a href="collections.aspx?LangType=<%=gtNavs(l)("ContentLanguage")%>&folderid=<%=(folderId)%>&Action=ViewMenu&nid=<%=(gtNavs(l)("MenuID"))%>" alt='<%=(MsgHelper.GetMessage("generic View") & " """ & replace(gtNavs(l)("MenuTitle"), "'", "`") & """")%>' title='<%=(MsgHelper.GetMessage("generic View") & " """ & replace(gtNavs(l)("MenuTitle"), "'", "`") & """")%>'><%=(gtNavs(l)("MenuTitle"))%></a></td>
				        <%end if %>
				        <td><%=(gtNavs(l)("MenuID"))%></td>
				        <td><%=(gtNavs(l)("ContentLanguage"))%></td>
				        <td><%=(gtNavs(l)("DisplayLastEditDate"))%></td>
			        </tr>
			        <%
                Next
                %>
            </table>
        </div>
        </form>
        <%
            gtObj = Nothing
            gtNavs = Nothing
        end if
    end if
    gtMsgObj = Nothing
    gtMess = Nothing
elseif ((action = "AddMenu") or (action = "AddSubMenu") or (action = "AddTransMenu"))then
%>
	<div id="FrameContainer" class="ektronBorder" style="POSITION: absolute; TOP: 48px; LEFT: 40px; WIDTH: 1px; HEIGHT: 1px; DISPLAY: none;">
		<iframe id="ChildPage" src="javascript:false;" name="ChildPage" frameborder="yes" marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="auto">
		</iframe>
	</div>
	<div allowtransparency="true" id="FolderPickerAreaOverlay" style="POSITION: absolute; TOP: 0px; LEFT: 0px; WIDTH: 1px; HEIGHT: 1px; DISPLAY: none; Z-INDEX: 10; background-color: transparent; ">
		<iframe allowtransparency="true" src="javascript:false;" id="FolderPickerAreaOverlayChildPage" name="FolderPickerAreaOverlayChildPage" frameborder="0"
			marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="no"
			style="background-color: transparent; background: transparent; FILTER: chroma(color=#FFFFFF)">
		</iframe>
	</div>
	<div id="FolderPickerPageContainer" style="POSITION: absolute; TOP: 0px; LEFT: 0px; WIDTH: 1px; HEIGHT: 1px; DISPLAY: none; Z-INDEX: 20; Background-color: transparent; Border-Style: none">
		<iframe id="FolderPickerPage" src="blank.htm" name="FolderPickerPage" frameborder="0" marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="auto" >
		</iframe>
	</div>
<%
	If Request.QueryString("LangType") = "-1" Then
		ContentLanguage = AppUI.DefaultContentLanguage
		AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage)
		AppUI.ContentLanguage = ContentLanguage
		colLangName=gtMsgObj.GetLanguageById(AppUI.DefaultContentLanguage)
		LanguageName=colLangName("Name")
	End If

    FolderID = request.QueryString("folderid")
    nId = Request.QueryString("nId")

	If (not IsCollectionMenuRoleMember()) Then
		SiteObj = AppUI.EkSiteRef
		cPerms = SiteObj.GetPermissions(FolderID, 0,"folder")
		if (ErrorString = "") then
			if (Not(cPerms("Collections"))) then
				ErrorString = MsgHelper.GetMessage("com: user does not have permission")
			end if
		end if
		SiteObj = nothing
		cPerms = nothing
	End If
    if (ErrorString <> "") then
    %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
		        <%=(MsgHelper.GetMessage("Add Menu Title"))%>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
    <%
		Else
			Select case action
				case "AddMenu":
				%> <form action="collectionaction.aspx?Action=doAddMenu&nId=<%=(nId)%>&LangType<%=ContentLanguage%>&bPage=<%=Request.QueryString("bPage")%>" method="Post" id="menu" name="menu" > <%
				Case "AddSubMenu":
				%> <form action="collectionaction.aspx?Action=doAddSubMenu&LangType=<%=ContentLanguage%>&nId=<%=(nId)%>&iframe=<%=Request.QueryString("iframe")%>&bPage=<%=Request.QueryString("bPage")%><%=NoWorkAreaAttribute%>" method="Post" name="menu" onload="do_onload"> <%
				Case "AddTransMenu":
				%> <form action="collectionaction.aspx?Action=doAddTransMenu&LangType=<%=ContentLanguage%>&nId=<%=(nId)%>&bPage=<%=Request.QueryString("bPage")%><%=NoWorkAreaAttribute%>" method="Post" name="menu" onload="do_onload"> <%
			End Select
		%>
        <div id="dlgBrowse" class="ektronWindow ektronModalStandard">
            <div class="ektronModalHeader">
              <h3>
                <%=(MsgHelper.GetMessage("lbl template selection"))%>
                <a href="#" onclick="return false;" class="ektronModalClose"> </a>
              </h3>
            </div>
            <div class="ektronModalBody">
              <div class="folderTree">
                <CMS:FolderTree ID="folderTree_editNewMenu" runat="server" Filter="[.]aspx$" />
              </div>
            </div>
        </div>
        <input type="hidden" name="frm_folder_id" value="<%=(FolderId)%>">
        <input type="hidden" name="frm_back" value="<%=Request.QueryString("back")%>">
        <div class="ektronPageHeader">
            <div class="ektronTitlebar"><%=(MsgHelper.GetMessage("Add Menu Title"))%></div>
            <div class="ektronToolbar">
                <table>
				    <tr>
				        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt save menu"), MsgHelper.GetMessage("btn save"), "onclick=""return SubmitForm('menu', 'VerifyMenuForm()');"""))%>
				        <%
				        Select Case action
				        Case "AddMenu":
			            if (Request.QueryString("back") <> "") then
				            Response.Write((GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", Request.QueryString("back"), MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "")))
				        elseIf Request.QueryString("bPage") = "ViewMenuReport" Then
					        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewMenuReport&folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))
				        Else
					        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewAllMenus&folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))
				        End If
				        Case "AddSubMenu":
				            if (Request.QueryString("back") <> "") then
					            Response.Write((GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", Request.QueryString("back"), MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "")))
					        elseif (Request.QueryString("iframe") = "true") then
						        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=""parent.CancelIframe();"""))
					        else
					        If (m_refApi.TreeModel = 1) Then
						        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "menutree.aspx?nid=" & nId & "&folderid=" & folderId & NoWorkAreaAttribute, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))
					        Else
					        Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewMenu&nid=" & nId & "&folderid=" & folderId & NoWorkAreaAttribute, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))
					        End If
					        end if
				        Case "AddTransMenu":
				            if (Request.QueryString("back") <> "") then
					            Response.Write((GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", Request.QueryString("back"), MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "")))
					        else
				                Response.Write(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?langtype=" & Request.QueryString("backlang") & "&action=ViewAllMenus&folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))
				            end if
				        End Select
				        %>
				        <td><%=objStyle.GetHelpButton("AddMenu")%></td>
				    </tr>
			    </table>
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
			<table class="ektronForm">
				<tr>
					<td class="label"><%=(MsgHelper.GetMessage("generic title label"))%></td>
					<td class="value">
					    <input type="Text" name="frm_menu_title" maxlength="255" onkeypress="return CheckKeyValue(event,'34');" ID="frm_menu_title">
					    [<%=LanguageName%>]
					</td>
				</tr>
			    <tr>
			        <td class="label"><%=msghelper.getmessage("lbl Image Link")%>:</td>
			        <td class="value">
			            <%=(sitePath)%>
			            <input type="Text" name="frm_menu_image" size="<%=(55 - len(sitePath))%>" maxlength="75" onkeypress="return CheckKeyValue(event,'34');" />
			            <a href="#" onclick="PopBrowseWin('images', '', 'document.forms.menu.frm_menu_image');return false;">
			            <img alt="Select Image" title="Select Image" src="<%=(AppPath)%>images/UI/Icons/imageLink.png" /></a>
			            <div class="ektronCaption">
			                <input name="frm_menu_image_override" id="frm_menu_image_override" type="checkbox" /><%=msghelper.Getmessage("alt Use image instead of a title")%>
			            </div>
			        </td>
			    </tr>
			    <tr>
				    <td class="label"><%=(MsgHelper.GetMessage("generic URL Link"))%>:</td>
				    <td class="value">
				        <%=(sitePath)%>
				        <input type="Text" name="frm_menu_link" id="frm_menu_link" size="<%=(55 - len(sitePath))%>" maxlength="255" onkeypress="return CheckKeyValue(event,'34');" />
				        <a href="#" onclick="LoadSelectContentPage();return true;">
				        <img alt="Select Page" title="Select Page" src="<%=(AppPath)%>images/UI/Icons/contentLink.png" /></a>
				        <div class="ektronCaption"><%= msghelper.getmessage("alt Hyperlink this menu item to this link") %></div>
				    </td>
			    </tr>
			    <tr>
                    <td class="label"><%=(MsgHelper.GetMessage("lbl template link"))%>:</td>
				    <td class="value">
				        <%=(sitePath)%>
				        <input type="Text" name="frm_menu_template" size="<%=(55 - len(sitePath))%>" maxlength="255" onkeypress="return CheckKeyValue(event,'34');" />
				        <div class="ektronCaption"><%= MsgHelper.GetMessage("alt (Menu Template Link that contents under the current menu level may use.)")%></div>
			        </td>
			    </tr>
                <tr>
				    <td class="label"><%=(MsgHelper.GetMessage("description label"))%></td>
				    <td class="value"><textarea name="frm_menu_description" maxlength="255" Onkeypress="return CheckKeyValue(event,'34');" ID="frm_menu_description"></textarea></td>
			    </tr>
                <% If (Request.QueryString("QD") <> "") Then %>
                    <input type="hidden" name="EnableReplication" value="<%=Request.QueryString("QD")%>" />
                <% Else %>
			        <% if (contentAPI.RequestInformationRef.EnableReplication) then %>
		            <tr>
		                <td class="label"><%=(MsgHelper.GetMessage("lbl folderdynreplication"))%></td>
			            <td>
		                    <input type="Checkbox" name="EnableReplication" value="1" />
			                <%=MsgHelper.GetMessage("replicate menu").Replace("Quickdeploy", "Quick Deploy")%>
                        </td>
                    </tr>
                    <%   end if %>
                <% end if %>
			</table>

			<div class="ektronTopSpace"></div>
	        <fieldset>
	            <legend><%=(MsgHelper.GetMessage("lbl folder associations"))%>:</legend>
	            <div>
	                <a href="#" onclick="LoadFolderPicker();return (false);"><%=(MsgHelper.GetMessage("btn change"))%></a>
	            </div>
	            <br />
	            <table width="100%" id="EnhancedMetadataMultiContainer1" >
		        </table>
	        </fieldset>

		    <div class="ektronHeader"><%=(MsgHelper.GetMessage("lbl template associations"))%>:</div>
		    <table width="100%">
			    <tr>
				    <td style="width:50%">
					    <select id="template_list" style="width:100%;" onchange="ta_editSelectList();" multiple="multiple" size="5" name="template_list">
					    </select>
				    </td>
                    <td>&nbsp;</td>
                    <td style="margin-left:4px;margin-right:4px;">
					    <a href="javascript:ta_moveItemUp()">
						    <img src="images/UI/Icons/arrowHeadUp.png" alt="Click to move item up" />
					    </a>
                        <br />
					    <a href="javascript:ta_moveItemDown()">
						    <img src="images/UI/Icons/arrowHeadDown.png" alt="Click to move item down" />
					    </a>
                        <br /><br />
                        <input type="button" value="..." class="ektronModal browseButton" />
                    </td>
                    <td>&nbsp;&nbsp;</td>
                    <td style="width:50%">
                        <table class="ektronForm">
                            <tr>
                                <td class="label"><%=MsgHelper.getmessage("lbl Text")%></td>
                                <td class="value"><input id="template_text" type="text" name="template_text" /></td>
                            </tr>
                        </table>
                        <div class="ektronTopSpace"></div>
                        <div style="padding-left:50px">
						    <input id="ta_btnAdd" name="ta_btnAdd" onclick="ta_addItemToSelectList();" type="button" value="<%=(MsgHelper.GetMessage("generic add title"))%>" />
						    &nbsp;&nbsp;
						    <input id="ta_btnChange" name="ta_btnChange" onclick="ta_updateItemToSelectList();" type="button" value="<%=(MsgHelper.GetMessage("btn change"))%>" />
							&nbsp;&nbsp;
						    <input id="ta_btnRemove" name="ta_btnRemove" onclick="ta_removeItemsFromSelectList();" type="button" value="<%=(MsgHelper.GetMessage("btn remove"))%>" />
						</div>
				    </td>
			    </tr>
		    </table>

		    <!--
		    <tr>
			    <td colspan="2">< %=(MsgHelper.GetMessage("generic include subfolders msg"))% ><input type="Checkbox" name="frm_recursive" checked ID="Checkbox3"></td>
		    </tr>
		    -->

		    <input type="hidden" name="frm_menu_parentid" value="<%= Request.QueryString("parentid") %>" />
		    <input type="hidden" name="frm_menu_ancestorid" value="<%= Request.QueryString("ancestorid") %>"/>
		    <input type="hidden" id="associated_folder_id_list" name="associated_folder_id_list" value="" />
		    <input type="hidden" id="associated_folder_title_list" name="associated_folder_title_list" value="" />
		    <input type="hidden" id="associated_templates" name="associated_templates" value="" />
		    <script type="text/javascript" language="javascript">
		       <!--//--><![CDATA[//><!--
	            do_onload();
	            //--><!]]>
	        </script>
		</div>
    </form>
    <%
		End If

elseif(action = "ReOrderMenuItems") then
	Dim MenuTitle, reOrderList As String

	MenuTitle = String.Empty
    nId = request.QueryString("nid")
    gtObj = AppUI.EkContentRef
    If m_refApi.TreeModel = 1 Then
    gtLinks = gtObj.GetMenuByID(nId, 0,False)
    Else
    gtLinks = gtObj.GetMenuByID(nId, 0)
    End If
    if (ErrorString = "") then
        if (gtLinks.count) then
            MenuTitle = gtLinks("MenuTitle")
            gtLinks = gtLinks("Items")
        end if
    end if
    reOrderList = ""
    if (ErrorString <> "") then
    %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
                <%=(GetTitleBar(MsgHelper.GetMessage("reorder menu item title")))%>
            </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
    <%
    else
    %>
        <form name="link_order" action="collectionaction.aspx?action=DoUpdateMenuItemOrder&nid=<%=(nId)%>&iframe=<%=(Request.QueryString("iframe"))%>" method="post">
            <input type="hidden" name="frm_folder_id" value="<%=(folderId)%>"/>
            <div class="ektronPageHeader">
                <div class="ektronTitlebar">
                    <%=(GetTitleBar(MsgHelper.GetMessage("reorder menu item title") & " """ & MenuTitle & """"))%>
                </div>
			    <div class="ektronToolbar">
				    <table>
					    <tr>
						    <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt: update menu order text"), MsgHelper.GetMessage("btn update"), "onclick=""return SubmitForm('link_order', 'true');"""))%>
				            <% if (Request.QueryString("back") <> "") then %>
					            <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", Request.QueryString("back"), MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), ""))%>
						    <% elseif (Request.QueryString("iframe") = "true") then %>
							    <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=""parent.CancelIframe();"""))%>
						    <% else %>
						        <% If(m_refApi.TreeModel = 1) Then %>
							        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "menutree.aspx?nid="& nId & "&folderid=" & folderId , MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
						        <%Else%>
						            <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?Action=ViewMenu&nid="& nId & "&folderid=" & folderId , MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
						        <%end if%>
						    <% end if %>
						    <td><%=objStyle.GetHelpButton(action)%></td>
					    </tr>
				    </table>
			    </div>
			</div>

            <div class="ektronPageContainer ektronPageInfo">
			    <table>
				    <tr>
					    <td>
						    <select name="OrderList" size="<%if (gtLinks.Count < 20) then Response.Write(gtLinks.Count) else Response.Write("20")%>">
							    <%
							    reOrderList = ""
							    for each gtNav in gtLinks
								    if (len(reOrderList))  then
									    reOrderList = reOrderList & "," & gtNav("ID")
								    else
									    reOrderList = gtNav("ID")
								    end if
								    if (IsNumeric(gtNav("ID"))) then
								    %>
									    <option value="<%=(gtNav("ID"))%>"><%=(gtNav("ItemTitle"))%>
								    <%
								    end if
								    %>
							    <%
							    next
							    %>
						    </select>
					    </td>
					    <td>&nbsp;&nbsp;</td>
					    <td>
						    <a href="javascript:Move('up', document.link_order.OrderList, document.link_order.LinkOrder)"><img src="<%=(AppPath)%>images/UI/Icons/arrowHeadUp.png" alt="<%=(MsgHelper.GetMessage("move selection up msg"))%>" title="<%=(MsgHelper.GetMessage("move selection up msg"))%>"></a>
						    <br />
						    <a href="javascript:Move('dn', document.link_order.OrderList, document.link_order.LinkOrder)"><img src="<%=(AppPath)%>images/UI/Icons/arrowHeadDown.png" alt="<%=(MsgHelper.GetMessage("move selection down msg"))%>" title="<%=(MsgHelper.GetMessage("move selection down msg"))%>"></a>
					    </td>
				    </tr>
			    </table>
			</div>
			<input type="hidden" name="LinkOrder" value="<%=(reOrderList)%>"/>
			<input type="hidden" name="navigationid" value="<%=(nId)%>"/>
		    <input type="hidden" name="frm_back" value="<%=(Request.QueryString("back"))%>"/>
            <%
            if (len(reOrderList)) then
            %>
                <script type="text/javascript">
                    <!--//--><![CDATA[//><!--
                    document.link_order.OrderList[0].selected = true;
                    //--><!]]>
                </script>
            <%
            end if
            %>
        </form>
    <%
    end if
elseif (action = "DeleteMenuItem") then
	Dim MenuTitle As String = String.Empty
    nId = request.QueryString("nid")
    gtObj = AppUI.EkContentRef
    If m_refApi.TreeModel = 1 Then
    gtLinks = gtObj.GetMenuByID(nId, 0,False)
    Else
        gtLinks = gtObj.GetMenuByID(nId, 0)
    End If
    if (ErrorString = "") then
        if (gtLinks.Count) then
            MenuTitle = gtLinks("MenuTitle")
            gtLinks = gtLinks("Items")
        end if
    end if
    if (ErrorString <> "") then
    %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
			    <%=(GetTitleBar(MsgHelper.GetMessage("delete Menu items title")))%>
            </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
    <%
    else
    %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
			    <%=(GetTitleBar(MsgHelper.GetMessage("delete menu items title") & " """ & MenuTitle & """"))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/delete.png", "#", MsgHelper.GetMessage("alt: delete menu items text"), MsgHelper.GetMessage("btn delete"), "onclick=""return SubmitForm('selections', 'GetIDs()');"""))%>
					    <%If (m_refApi.TreeModel = 1) Then %>
					    <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "menutree.aspx?nid="&nId & "&folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					    <%Else%>
					    <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?Action=ViewMenu&nid="&nId & "&folderid=" & folderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					    <%End If %>
				        <td><%=objStyle.GetHelpButton(action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>

        <form name="selections" method="post" action="collectionaction.aspx?Action=doDeleteMenuItem&folderid=<%=(folderId)%>&nid=<%=(nId)%>">
		<a href="#" onclick="SelectAll();return false;"><%=(MsgHelper.GetMessage("generic select all msg"))%></a>&nbsp;&nbsp;<a href="#" onclick="ClearAll();return false;"><%=(MsgHelper.GetMessage("generic clear all msg"))%></a>
		<div class="ektronPageContainer ektronPageGrid">
		    <table width="100%">
			    <tr class="title-header">
				    <td width="25%"><%=(MsgHelper.GetMessage("generic Title"))%></td>
				    <td width="5%"><%=(MsgHelper.GetMessage("generic ID"))%></td>
				    <td><%=(MsgHelper.GetMessage("generic URL Link"))%></td>
			    </tr>
			    <%
			    lLoop = 0
			    cLinkArray = ""
			    fLinkArray = ""
			    cLanguagesArray=""
			    dim m as integer = 0
			    for m = 1 to gtLinks.count %>
			        <tr>
				        <td>
					        <input type="checkbox" name="frm_check<%=(lLoop)%>" onclick="document.forms.selections['frm_hidden<%=(lLoop)%>'].value=(this.checked ? '<%=(gtLinks(m)("ID"))%>.<%=(gtLinks(m)("ItemType"))%>.<%=(gtLinks(m)("ItemID"))%>' : 0);"/>
					        <input size="<%=(gtLinks(m)("ID"))%>" type="hidden" name="frm_hidden<%=(lLoop)%>" value="0"/>
					        <input type="hidden" name="frm_languages<%=lloop%>" value="<%=gtLinks(m)("ContentLanguage")%>"/>
					        <%=(gtLinks(m)("ItemTitle"))%>
				        </td>
				        <td><%=(gtLinks(m)("ItemID"))%></td>
				        <td><%=(gtLinks(m)("ItemLink"))%></td>
			        </tr>
			        <%
				    cLinkArray = cLinkArray & "," & gtLinks(m)("ID") & "_" & gtLinks(m)("ItemType")
				    cLanguagesArray=cLanguagesArray & "," & gtLinks(m)("ContentLanguage") & "_" & gtLinks(m)("ItemType")
				    fLinkArray = fLinkArray & "," & folderId
				    lLoop = lLoop + 1
			    next
			    if (len(cLinkArray)) then
				    cLinkArray = Right(cLinkArray, len(cLinkArray) - 1)
				    fLinkArray = Right(fLinkArray, len(fLinkArray) - 1)
				    cLanguagesArray=Right(cLanguagesArray, len(cLanguagesArray) - 1)
			    end if
			    %>
		    </table>
		</div>
		<script type="text/javascript" language="javascript">
		    <!--//--><![CDATA[//><!--
			Collections = "<%=(cLinkArray)%>";
			Folders = "<%=(fLinkArray)%>";
			//--><!]]>
		</script>
		<input type="hidden" name="frm_content_ids" value=""/>
		<input type="hidden" name="frm_content_languages" value=""/>
		<input type="hidden" name="frm_folder_ids" value=""/>
		<input type="hidden" name="CollectionID" value="<%=(nId)%>"/>
		</form>
    <%
    end if
elseif (action = "EditMenu") then
	Dim MenuId As Object
	Dim menuData As Object
	Dim AssociatedFolderIdListString As String = ""
	Dim AssociatedFolderTitleListString As String = ""
	Dim AssociatedTemplatesString As String = ""

    MenuId = request.QueryString("nid")
    FolderId = Request.QueryString("folderid")

    SiteObj = AppUI.EkSiteRef
    cPerms = SiteObj.GetPermissions(FolderID, 0, "folder")
    if (ErrorString = "") then
        if (Not(IsCollectionMenuRoleMember() OrElse cPerms("Collections"))) then
            ErrorString = MsgHelper.GetMessage("com: user does not have permission")
        end if
    end if
    if (ErrorString = "") then
        gtObj = AppUI.EkContentRef
        If (m_refApi.TreeModel = 1) Then
			gtLinks = gtObj.GetMenuByID(MenuId, 0,False)
        Else
            gtLinks = gtObj.GetMenuByID(MenuId, 0)
        End If
        menuData = gtObj.GetMenuDataByID(MenuId)
        If (Not IsNothing(menuData.AssociatedFolderIdList)) Then
			AssociatedFolderIdListString = menuData.AssociatedFolderIdList
			AssociatedFolderTitleListString = GetTitlesFromFolderIds(menuData.AssociatedFolderIdList)
        End If
        If (Not IsNothing(menuData.AssociatedTemplates)) Then
			AssociatedTemplatesString = menuData.AssociatedTemplates
        End If
    end if
    if (ErrorString <> "") then
    %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
			    <%=(GetTitleBar(MsgHelper.GetMessage("edit menu title")))%>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
    <%
    else
    %>
        <form action="collectionaction.aspx?Action=doEditMenu&nid=<%=(MenuId)%>&folderid=<%=(FolderId)%>&iframe=<%=Request.QueryString("iframe")%>" method="Post" name="menu" >
        <div id="dlgBrowse" class="ektronWindow ektronModalStandard">
            <div class="ektronModalHeader">
              <h3>
                    <%=(MsgHelper.GetMessage("lbl template selection"))%>
                    <a href="#" onclick="return false;" class="ektronModalClose"> </a>
              </h3>
            </div>
            <div class="ektronModalBody">
              <div class="folderTree">
                    <CMS:FolderTree ID="folderTree_editExistingMenu" runat="server" Filter="[.]aspx$" />
              </div>
            </div>
        </div>
		    <div id="FrameContainer" class="ektronBorder" style="POSITION: absolute; TOP: 48px; LEFT: 40px; WIDTH: 1px; HEIGHT: 1px; DISPLAY: none;">
			    <iframe id="ChildPage" src="javascript:false;" name="ChildPage" frameborder="yes" marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="auto">
			    </iframe>
		    </div>
		    <div allowtransparency="true" id="FolderPickerAreaOverlay" style="POSITION: absolute; TOP: 0px; LEFT: 0px; WIDTH: 1px; HEIGHT: 1px; DISPLAY: none; Z-INDEX: 10; background-color: transparent; ">
			    <iframe allowtransparency="true" src="javascript:false;" id="FolderPickerAreaOverlayChildPage" name="FolderPickerAreaOverlayChildPage" frameborder="0"
				    marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="no"
				    style="background-color: transparent; background: transparent; FILTER: chroma(color=#FFFFFF)">
			    </iframe>
		    </div>
		    <!-- TOP: 48px; LEFT: 55px; -->
		    <div id="FolderPickerPageContainer" style="POSITION: absolute; TOP: 0px; LEFT: 0px; WIDTH: 1px; HEIGHT: 1px; DISPLAY: none; Z-INDEX: 20; Background-color: transparent; Border-Style: none">
			    <iframe id="FolderPickerPage" src="blank.htm" name="FolderPickerPage" frameborder="0" marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="auto" >
			    </iframe>
		    </div>
		    <input type="hidden" name="frm_folder_id" value="<%=(FolderId)%>"/>
		    <input type="hidden" name="frm_back" value="<%=(Request.QueryString("back"))%>"/>

		    <div class="ektronPageHeader">
		        <div class="ektronTitlebar">
			        <%=(GetTitleBar(MsgHelper.GetMessage("edit menu title")))%>
		        </div>
		        <div class="ektronToolbar">
			        <table>
				        <tr>
				            <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", "Save Menu", MsgHelper.GetMessage("btn save"), "onclick=""return SubmitForm('menu', 'VerifyMenuForm()');"""))%>
				            <% if (Request.QueryString("back") <> "") then %>
					            <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", Request.QueryString("back"), MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), ""))%>
				            <% elseif (Request.QueryString("iframe") = "true") then %>
					            <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=""parent.CancelIframe();"""))%>
				            <% else %>
				                 <%If(m_refApi.TreeModel = 1) Then %>
					                <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "menutree.aspx?nid=" & MenuId & "&folderid=" & FolderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
	 						     <%Else%>
					                <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewMenu&nid=" & MenuId & "&folderid=" & FolderId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					             <%End If%>
				            <% end if %>
				            <td><%=objStyle.GetHelpButton(action)%></td>
				        </tr>
			        </table>
		        </div>
		    </div>

            <div class="ektronPageContainer ektronPageInfo">
			    <table class="ektronForm">
				    <tr>
					    <td class="label"><%=(MsgHelper.GetMessage("generic title label"))%></td>
					    <td nowrap="nowrap"><input type="Text" value="<%=(gtLinks("MenuTitle"))%>" name="frm_menu_title" maxlength="255" onkeypress="return CheckKeyValue(event,'34');">&nbsp;[<%=LanguageName%>]</td>
				    </tr>
                    <tr>
                        <td class="label"><%=msghelper.getmessage("lbl Image Link")%>:</td>
				        <td nowrap="nowrap"><%=(sitePath)%><input type="Text" value="<%=(gtLinks("MenuImage"))%>" name="frm_menu_image" size="<%=(55 - len(sitePath))%>" maxlength="75" onkeypress="return CheckKeyValue(event,'34');">
				            <a href="#" onclick="PopBrowseWin('images', '', 'document.forms.menu.frm_menu_image');return false;"><img src="<%=(AppPath)%>images/UI/Icons/imageLink.png"></a>
				            <br />
				            <input name="frm_menu_image_override" id="Checkbox2" <% if gtLinks("ImageOverride") then %>checked<% end if %> type="checkbox"/><%=msghelper.getmessage("alt Use image instead of a title")%>
				        </td>
			        </tr>
			        <tr>
				        <td class="label"><%=(MsgHelper.GetMessage("generic URL Link"))%>:</td>
				        <td nowrap="nowrap">
				            <%=(sitePath)%><input type="Text" value="<%=(gtLinks("MenuLink"))%>" name="frm_menu_link" id="frm_menu_link" size="<%=(56 - len(sitePath))%>" maxlength="255" onkeypress="return CheckKeyValue(event,'34');">&nbsp;
				            <a href="#" onclick="LoadSelectContentPage();return true;"><img alt="Select Page" title="Select Page" src="<%=(AppPath)%>images/UI/Icons/contentLink.png"></a>
				            <br />
				            <%= msghelper.getmessage("alt Hyperlink this menu item to this link") %>
				        </td>
			        </tr>
                    <tr>
                        <td nowrap="nowrap" class="label"><%=(MsgHelper.GetMessage("lbl template link"))%>:</td>
				        <td nowrap="nowrap"><%=(sitePath)%><input type="Text" value="<%=(gtLinks("MenuTemplate"))%>" name="frm_menu_template" id="frm_menu_template" size="<%=(56 - len(sitePath))%>" maxlength="255" onkeypress="return CheckKeyValue(event,'34');">
				        <br /><%= MsgHelper.GetMessage("alt (Menu Template Link that contents under the current menu level may use.)")%>
				        </td>
                    </tr>
                    <tr>
				        <td class="label"><%=(MsgHelper.GetMessage("description label"))%></td>
				        <td><textarea name="frm_menu_description" maxlength="255" onkeypress="return CheckKeyValue(event,'34');"><%=(gtLinks("MenuDescription"))%></textarea></td>
			        </tr>
			        <% if (contentAPI.RequestInformationRef.EnableReplication) then %>
				        <%   if (gtLinks("IsSubMenu")) then %>
				            <input type="hidden" name="EnableReplication" value="<%= gtLinks("EnableReplication") %>" />
				        <%   else %>
			                <tr>
				                <td class="label"><%=(MsgHelper.GetMessage("lbl folderdynreplication"))%></td>
				                <td>
			                        <input type="Checkbox" name="EnableReplication"
			                        <%if (gtLinks("EnableReplication") = 1) then
				                    response.write("checked")
				                    end if%>
			                        />
				                    <%=MsgHelper.GetMessage("replicate menu")%>
                                </td>
                            </tr>
				        <%   end if %>
			        <% end if %>
		            <!--
		            <tr>
			            <td colspan="2">< %=(MsgHelper.GetMessage("generic include subfolders msg"))% ><input type="Checkbox" name="frm_recursive" < %if (gtLinks("Recursive") = "1") then% >checked< %end if% >></td>
		            </tr>
		            -->
			    </table>

                <div class="ektronTopSpace"></div>
		        <fieldset>
		            <legend><%=(MsgHelper.GetMessage("lbl folder associations"))%></legend>
		            <div>
		                <a href="#" onclick="LoadFolderPicker();return (false);"><%=(MsgHelper.GetMessage("btn change"))%></a>
		            </div>
		            <br />
		            <table width="100%" border="1" style="border-color:#d8e6ff;" id="EnhancedMetadataMultiContainer1" >
					</table>
			    </fieldset>

		        <div class="ektronTopSpace"></div>
		        <fieldset>
		            <legend><%=(MsgHelper.GetMessage("lbl template associations"))%></legend>
		            <table>
				        <tbody>
					        <tr>
						        <td width="50%">
							        <select id="template_list" style="width:100%;" onchange="ta_editSelectList();"
								        multiple size="5" name="template_list">
							        </select>
					            </td>
                                <td>&nbsp;</td>
                                <td style="margin-left:4px;margin-right:4px;">
                                    <a href="javascript:ta_moveItemUp()">
                                        <img src="images/ui/icons/arrowHeadUp.png" alt="Click to move item up" />
                                    </a>
                                    <br />
                                    <a href="javascript:ta_moveItemDown()">
                                        <img src="images/ui/icons/arrowHeadDown.png" alt="Click to move item down" />
                                    </a>
                                    <br /><br />
                                    <input type="button" value="..." class="ektronModal browseButton" />
                                </td>
                                <td>&nbsp;&nbsp;</td>
						        <td width="50%">
                                    <table class="ektronForm">
                                        <tr>
                                            <td class="label"><%=MsgHelper.GetMessage("lbl Text")%></td>
                                            <td class="value"><input id="template_text" name="template_text" size="40"/></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>
										        <input id="Button1" onclick="ta_addItemToSelectList();" type="button" value="<%=(MsgHelper.GetMessage("generic add title"))%>" name="ta_btnAdd"/>
										        &nbsp;&nbsp;
										        <input id="Button2" onclick="ta_updateItemToSelectList();" type="button" value="<%=(MsgHelper.GetMessage("btn change"))%>" name="ta_btnChange"/>
										        &nbsp;&nbsp;
										        <input id="Button3" onclick="ta_removeItemsFromSelectList();" type="button" value="<%=(MsgHelper.GetMessage("btn remove"))%>" name="ta_btnRemove"/>
                                            </td>
                                        </tr>
                                    </table>
						        </td>
					        </tr>
				        </tbody>
			        </table>
		        </fieldset>
            </div>
		    <input type="hidden" id="frm_set_to_template" name="frm_set_to_template" value="<%=(gtLinks("MenuTemplate"))%>"/>
		    <input type="hidden" id="associated_folder_id_list" name="associated_folder_id_list" value="<%=(AssociatedFolderIdListString)%>" />
		    <input type="hidden" id="associated_folder_title_list" name="associated_folder_title_list" value="<%=(AssociatedFolderTitleListString)%>" />
		    <input type="hidden" id="associated_templates" name="associated_templates" value="<%=(AssociatedTemplatesString)%>" />
		    <script type="text/javascript" language="javascript">
		        do_onload();
		    </script>
		</form>
    <%
    end if
elseif (action = "EditMenuItem") then
    dim ItemTitle, Id, MenuId As Object
    'if (((Request.QueryString("cmsMode") = "preview")  or (Request.Cookies("ecm")("site_preview"))) _
    'and  (request.cookies("ecm")("user_id") <> 0)) then
    '   cmsPreview = true
    'else
    '   cmsPreview = false
    'end if
    Id = request.QueryString("id")
    MenuId = request.QueryString("nid")
    FolderId = Request.QueryString("folderid")
    gtObj = AppUI.EkContentRef
    gtLinks = gtObj.GetMenuItemByID(MenuId, Id, true)
    if (ErrorString = "") then
        if (gtLinks.Count) then
            ItemTitle = gtLinks("ItemTitle")
        end if
    end if

    dim t_ID, t_ItemType, t_ContentLanguage, t_iframe
    t_ID = gtLinks("ID")
    t_ItemType = gtLinks("ItemType")
    t_ContentLanguage = gtLinks("ContentLanguage")
    t_iframe = Request.QueryString("iframe")

    if (ErrorString <> "") then
    %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
			    <%=(GetTitleBar(MsgHelper.GetMessage("edit Menu items title")))%>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
    <%
    else
    %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
			    <%=(GetTitleBar(MsgHelper.GetMessage("edit Menu items title")))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
				        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", MsgHelper.GetMessage("alt save menu item"), MsgHelper.GetMessage("btn save"), "onclick=""return SubmitForm('AddMenuItem', 'VerifyAddMenuItem()');"""))%>
			            <% if (Request.QueryString("back") <> "") then %>
				            <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", Request.QueryString("back"), MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), ""))%>
				        <% elseif (Request.QueryString("iframe") = "true") then %>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic Cancel"), MsgHelper.GetMessage("generic Cancel"), "onclick=""parent.CancelIframe();"""))%>
				        <% else %>
				        <% If(m_refApi.TreeModel = 1) Then %>
					        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "menutree.aspx?nid=" & MenuId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
				        <%Else%>
				        <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx?action=ViewMenu&nId=" & MenuId, MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
				        <%End If%>
				        <% end if %>
				        <td><%=objStyle.GetHelpButton(action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>

		<form name="AddMenuItem" action="collectionaction.aspx?action=doUpdateMenuItem&id=<%=(gtLinks("ID"))%>&type=<%=gtLinks("ItemType")%>&LangType=<%=gtLinks("ContentLanguage")%>&iframe=<%=Request.QueryString("iframe")%>" method="post" ID="AddMenuItem">
			<input type="hidden" name="CollectionID" value="<%=(MenuId)%>" ID="Hidden12"/>
			<input type="hidden" name="FolderID" value="<%=(FolderId)%>" ID="Hidden13"/>
			<input type="hidden" name="DefaultTitle" value="<%=gtLinks("DefaultTitle")%>"/>
		    <input type="hidden" name="frm_back" value="<%=(Request.QueryString("back"))%>"/>

            <div class="ektronPageContainer ektronPageInfo">
		        <table class="ektronForm">
			        <tr>
				        <td class="label"><%=(MsgHelper.GetMessage("generic title label"))%></td>
				        <td class="value"><input type="text" name="Title" value="<%=gtLinks("ItemTitle")%>"/></td>
			        </tr>
			        <tr>
				        <td class="label"><%=msghelper.getmessage("lbl Image Link")%>:</td>
				        <td class="value">
				            <%=(sitePath)%>
				            <input type="Text" value="<%=gtLinks("ItemImage")%>" name="frm_menu_image" size="<%=(55 - len(sitePath))%>" maxlength="75" onkeypress="return CheckKeyValue(event,'34');" />
				            <a href="#" onclick="PopBrowseWin('images', '', 'document.forms.AddMenuItem.frm_menu_image');return false;">
				            <img alt="Select Image" title="Select Image" src="<%=(AppPath)%>images/UI/Icons/imageLink.png" /></a>
				            <div class="ektronCaption"><input name="frm_menu_image_override" id="Checkbox3" <% if (gtLinks("ImageOverride")) then %>checked<% end if %> type="checkbox" /><%=msghelper.getmessage("alt Use image instead of a title")%></div>
				        </td>
			        </tr>
			        <% if (NOT (("1" = gtLinks("ItemType")) OR ("2" = gtLinks("ItemType")) OR ("4" = gtLinks("ItemType")))) then %>
			            <tr>
				            <td class="label"><%=(MsgHelper.GetMessage("generic URL Link"))%>:</td>
				            <td class="value"><input type="text" name="Link" size=50 value="<%=gtLinks("ItemLink")%>"></td>
			            </tr>
			        <% end if %>
			        <tr>
				        <td class="label"><%=(MsgHelper.GetMessage("description label"))%></td>
				        <td class="value"><textarea name="Description" ID="Description"><%=gtLinks("ItemDescription")%></textarea></td>
			        </tr>
			        <% if ("4" <> gtLinks("ItemType")) then 'Not a sub menu %>
			            <tr>
				            <td class="label"><%=(MsgHelper.GetMessage("generic link target label"))%></td>
				            <td class="value">
					            <!-- 1 = _blank; 2 = _self; 3 = _parent; 4 = _top -->
					            <input type="radio" name="Target" value="blank" <%if (gtLinks("ItemTarget") = "popup") then %>checked<%end if%>><%=(MsgHelper.GetMessage("Popup label"))%>
					            <input type="radio" name="Target" VALUE="self" <%if (gtLinks("ItemTarget") = "self") then %>checked<%end if%>><%=(MsgHelper.GetMessage("Self label"))%>
					            <input type="radio" name="Target" VALUE="parent" <%if (gtLinks("ItemTarget") = "parent") then %>checked<%end if%>><%=(MsgHelper.GetMessage("Parent label"))%>
					            <input type="radio" name="Target" VALUE="top" <%if (gtLinks("ItemTarget") = "top") then %>checked<%end if%>><%=(MsgHelper.GetMessage("Top label"))%>
				            </td>
			            </tr>
			            <%If gtLinks("ItemType")="1" Then%>
                            <tr>
                                <td class="label"><%=(msghelper.getmessage("lbl Link"))%>:</td>
                                <td class="value">
                                    <input type="radio" name="link" value="0" <%if ((gtLinks("LinkType") = "0") OR (gtLinks("LinkType") = "")) then %>checked<%end if%>><%=(msghelper.getmessage("lbl QuickLink"))%>
                                    <input type="radio" name="link" VALUE="1" <%if (gtLinks("LinkType") = "1") then %>checked<%end if%>><%=(msghelper.getmessage("lbl Menu Template"))%>
                                </td>
                            </tr>
			                <%if Trim(gtLinks("ItemLink"))<>"" then%>
                                <tr>
                                    <td class="label"><%=msghelper.getmessage("lbl Quick Link")%>:</td>
                                    <td class="readOnlyValue"><%=Trim(gtLinks("ItemLink"))%></td>
                                </tr>
				            <%end if%>
				            <%if Trim(gtLinks("FolderId"))<>"" then%>
                                <tr>
                                    <td class="label"><%=msghelper.getmessage("lbl Folder ID")%>:</td>
                                    <td class="readOnlyValue"><%=Trim(gtLinks("FolderId"))%></td>
                                </tr>
				            <%end if%>
                        <%End If%>
			        <% end if %>
		        </table>
		    </div>
		</form>
    <%
    end if
else
    ' this is action="mainPage" which lists colections for a folder
    dim gtObj,gtNavs As Object
    OrderBy = request.queryString("OrderBy")
    if (OrderBy = "") then
        OrderBy = "title"
    end if
    gtObj = AppUI.EkContentRef
    gtNavs = gtObj.GetAllCollectionsInfo(folderId, OrderBy)
    if (len(ErrorString) = 0) then
        gtNav = gtObj.GetFolderInfov2_0(folderId)
        if (len(ErrorString) = 0) then
            FolderName = gtNav("FolderName")
        end if
    end if
    if (ErrorString <> "") then
    %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
                <%=(GetTitleBar(MsgHelper.GetMessage("view collections title")))%>
            </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "collections.aspx", MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton(action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>
    	<%=(MsgHelper.GetMessage("generic page error message"))%>
		<div class="titlebar-error"><%=(ErrorString)%></div>
    <%
    else
    %>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar">
                <%=(GetTitleBar(MsgHelper.GetMessage("view collections title") & " """ & FolderName & """"))%>
            </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/add.png", "collections.aspx?action=Add&folderid=" & folderId, MsgHelper.GetMessage("alt: add new collection text"), MsgHelper.GetMessage("btn add"), ""))%>
					    <%=(GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "content.aspx?Action=ViewContentByCategory&id="& folderId , MsgHelper.GetMessage("alt back button text"), MsgHelper.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("view_collections_in_folder")%></td>
				    </tr>
			    </table>
		    </div>
		</div>
		<div class="ektronPageContainer ektronPageGrid">
		    <table width="100%" class="ektronGrid">
			    <tr class="title-header">
				    <td width="30%"><a href="collections.aspx?folderid=<%=(folderId)%>&OrderBy=navname" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>"><%=(MsgHelper.GetMessage("generic Title"))%></a></td>
				    <td width="5%"><a href="collections.aspx?folderid=<%=(folderId)%>&OrderBy=CollectionID" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>"><%=(MsgHelper.GetMessage("generic ID"))%></a></td>
				    <td ><a href="collections.aspx?folderid=<%=(folderId)%>&OrderBy=date" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>"><%= (MsgHelper.GetMessage("generic Date Modified")) %></a></td>
				    <td><a href="collections.aspx?folderid=<%=(folderId)%>&OrderBy=CollectionTemplate" alt="<%=(MsgHelper.GetMessage("click to sort msg"))%>" title="<%=(MsgHelper.GetMessage("click to sort msg"))%>"><%= (MsgHelper.GetMessage("generic URL Link")) %></a></td>
			    </tr>
		    <%
		    if (gtNavs.Count) then
		    dim n as integer = 0
		    For n = 1 to gtNavs.count
		    %>
			        <tr>
			            <%
			            dim colAction as string
			            if (gtNavs(n)("ApprovalRequired") = true andalso ucase(gtNavs(n)("Status")) <> "A") then
			                colAction = "&action=ViewStage"
			            else
			                colAction = "&action=View"
			            end if
			            %>
				        <td><a href="collections.aspx?folderid=<%=(folderId)%><%=(colAction)%>&nid=<%=(gtNavs(n)("CollectionID"))%>" alt='<%=(MsgHelper.GetMessage("generic View") & " """ & replace(gtNavs(n)("CollectionTitle"), "'", "`") & """")%>' title='<%=(MsgHelper.GetMessage("generic View") & " """ & replace(gtNavs(n)("CollectionTitle"), "'", "`") & """")%>'><%=(gtNavs(n)("CollectionTitle"))%></a></td>
				        <td><%=(gtNavs(n)("CollectionID"))%></td>
				        <td><%=(gtNavs(n)("DisplayLastEditDate"))%></td>
				        <td><%=(gtNavs(n)("CollectionTemplate"))%></td>
			        </tr>
			    <%
			    Next
			    %>
		    </table>
		</div>
		<%
		gtObj = Nothing
		gtNavs = Nothing
	end if
	end if
end if
gtMsgObj = Nothing
gtMess = Nothing
%>
</body>
</html>