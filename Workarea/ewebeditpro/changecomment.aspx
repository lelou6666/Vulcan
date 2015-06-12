<%@ Page Language="VB" AutoEventWireup="false" validateRequest="false" CodeFile="changecomment.aspx.vb" Inherits="Workarea_ewebeditpro_changecomment" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../controls/Editor/ContentDesignerWithValidator.ascx" %>

<%@ Import Namespace="Ektron.Cms.UI.CommonUI" %>
<%@ Import Namespace="Ektron.Cms.Content" %>
<%@ Import Namespace="Ektron.Cms" %>

<script language="vb" runat="server">
    Const ALL_CONTENT_LANGUAGES As Integer = -1
    Const CONTENT_LANGUAGES_UNDEFINED As Integer = 0
    Dim ContentLanguage As Integer
    Dim EnableMultilingual As Integer
    Dim CommentKeyId As Long
    Dim CommentId As Long
    Dim RefId As Integer
    Dim RefType As String
    Dim CurrentUserID As Long
    Dim Action As String
    Dim ActionType As String
    Dim cComment As Object
    Dim cComments As Object
    Dim Flag As Boolean
    Dim retVal As Object
    Dim CommentText As String
    Dim CommentType As String
    Dim Messages As String
    Dim NS4 As Boolean
    Dim ResetCommentTag As Boolean
    Dim OrderBy As String
    Dim AppeWebPath As String
    Dim var1 As String
    Dim var2 As String
    Dim ekContObj As Object
    Dim JsStr As Object
</script>

<%
    ' Local Values
    ContentLanguage = m_iContentLanguage
    CurrentUserId = m_iCurrentUserId
    AppeWebPath = m_strAppeWebPath
    var1 = m_strServerName
    Messages = " "
    RefType = m_strRefType
    Flag = m_bInsertElementFlag
    ResetCommentTag = False
    CommentKeyId = m_iCommentKeyId
    Action = m_strCmdAction
    ActionType = m_strActionType
    CommentId = m_iCommentId
    RefId = m_iRefId
    CommentType = m_strCommentType
    OrderBy = m_strOrderBy
    var2 = m_strvar2
    NS4 = m_bNS4
    ResetCommentTag = m_bResetCommentTag
    CommentText = m_strCommentText
%>

<html>
	<head runat="server">
		<meta http-equiv="pragma" content="no-cache">
		<title runat="server"></title>
		<script language="JavaScript1.2" type="text/javascript" src="eweputil.js"></script>
		<script language="JavaScript" type="text/javascript" src="../java/toolbar_roll.js"></script>
		<script language="JavaScript" type="text/javascript" src="../java/empjsfunc.js"></script>
		<script language="JavaScript" type="text/javascript" src="../java/dateonlyjsfunc.js"></script>
		<script language="JavaScript" type="text/javascript">
		<!--
		var editorName = "<asp:Literal id="lEditorName" runat="server"/>"
		function submit_form(op)
		{
			var oForm = document.forms[0];
			var tid = oForm.ref_id.value;
			var ty = oForm.ty.value;
			document.getElementById("editorName").value = editorName;
			if ("insert" == op || "update" == op)
			{
				if ("update" == op)
				{
					oForm.action='changecomment.aspx?editorName=' + editorName + '&action=Update';
				}
				else
				{
					oForm.action = 'changecomment.aspx?editorName=' + editorName + '&action=Add&comment_id=0';
				}
				if (eWebEditProUtil.isOpenerAvailable())
				{
				    oForm.submit();
				}
				else
				{
				    alert('<asp:Literal id="EditorPageClosed" runat="server"/>');
				}
            }
			return false;
		}
		function DoSort(key)
		{
			document.forms[0].action="changecomment.aspx?editorName=" + editorName + "&orderby=" + replace_string(key," ","%20");
			document.forms[0].submit();
		}
		function replace_string(string,text,by)
		{
			var strLength = string.length, txtLength = text.length;
			if ((strLength == 0) || (txtLength == 0)) return string;
			var i = string.indexOf(text);
			if ((!i) && (text != string.substring(0,txtLength))) return string;
			if (i == -1) return string;
			var newstr = string.substring(0,i) + by;
			if (i+txtLength < strLength)
				newstr += replace_string(string.substring(i+txtLength,strLength),text,by);
			return newstr;
		}
		//-->
		</script>
	</head>
	<body bgcolor="#ffffff" bottommargin="0" leftmargin="5" rightmargin="0" topmargin="0">
		<form action="changecomment.aspx" name="contentcomment" method="Post" id="contentcomment" runat="server">
			<input type="hidden" name="OrderBy" value="<%= (HttpUtility.HtmlEncode(Request.QueryString("OrderBy"))) %>">
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
				<tr><td>&nbsp;</td></tr>
				<%If Action <> "Edit" AND Action <> "Update" Then %>
				<tr>
					<td>
						<table border="1" width="100%" cellspacing="0">
							<tr class=cal-header2>
								<%If  OrderBy="" Then%>
									<td width="15%"><a href="javascript: DoSort('date_created asc');" title="Sort by Date/Time"><%=(m_refmsg.GetMessage("lbl date/time"))%></a></td><td width="20%"><a href="javascript: DoSort('last_name asc');" title="Sort by Last Name"><%=(m_refMsg.GetMessage("lbl added by"))%></a></td><td width="65%"><%=(m_refMsg.GetMessage("comments label"))%></td>
								<%ElseIf OrderBy="date_created asc" Then%>
									<td width="15%"><a href="javascript: DoSort('date_created desc');" title="Sort by Date/Time"><%=(m_refMsg.GetMessage("lbl date/time"))%></a></td><td width="20%"><a href="javascript: DoSort('last_name asc');" title="Sort by Last Name"><%=(m_refMsg.GetMessage("lbl added by"))%></a></td><td width="65%"><%=(m_refMsg.GetMessage("comments label"))%></td>
								<%ElseIf OrderBy="date_created desc" Then%>
									<td width="15%"><a href="javascript: DoSort('date_created asc');" title="Sort by Date/Time"><%=(m_refMsg.GetMessage("lbl date/time"))%></a></td><td width="20%"><a href="javascript: DoSort('last_name asc');" title="Sort by Last Name"><%=(m_refMsg.GetMessage("lbl added by"))%></a></td><td width="65%"><%=(m_refMsg.GetMessage("comments label"))%></td>
								<%ElseIf OrderBy="last_name asc" Then%>
									<td width="15%"><a href="javascript: DoSort('date_created asc');" title="Sort by Date/Time"><%=(m_refMsg.GetMessage("lbl date/time"))%></a></td><td width="20%"><a href="javascript: DoSort('last_name desc');" title="Sort by Last Name"><%=(m_refMsg.GetMessage("lbl added by"))%></a></td><td width="65%"><%=(m_refMsg.GetMessage("comments label"))%></td>
								<%ElseIf OrderBy="last_name desc" Then%>
									<td width="15%"><a href="javascript: DoSort('date_created asc');" title="Sort by Date/Time"><%=(m_refMsg.GetMessage("lbl date/time"))%></a></td><td width="20%"><a href="javascript: DoSort('last_name asc');" title="Sort by Last Name"><%=(m_refMsg.GetMessage("lbl added by"))%></a></td><td width="65%"><%=(m_refMsg.GetMessage("comments label"))%></td>
								<%End If%>
							</tr>
							<!-- ListAllCommentsInRow() -->

							<asp:Literal ID="CommentListHtml" runat="server"></asp:Literal>

						</table>
					</td>
				</tr>
				<%End If%>
				<tr><td>&nbsp;</td></tr>
				<% 	If 0 = Len(Action) Or "Edit" = Action Then%>
				<tr>
					<td class="input-box-text"><%=(m_refMsg.GetMessage("comments label"))%>:</td>
				</tr>
				<tr>
					<td>
						<ektron:ContentDesigner ID="CommentEditor" Width="450px" Height="200px"
							Toolbars="Minimal" AllowFonts="true" AllowScripts="true" ShowHtmlMode="false" runat="server" />
					</td>
				</tr>
				<tr><td>&nbsp;</td></tr>
				<tr>
					<td>
					<%If Action="Edit" Then%>
						<input type=button name="btn_submit" value="<%= m_refmsg.getmessage("alt update button text")%>" onclick="return submit_form('update');">
					<%Else%>
						<input type=button name="btn_submit" value="<%= m_refmsg.getmessage("btn insert")%>" onclick="return submit_form('insert');">
					<%End If%>
					&nbsp;&nbsp;<input type=button name="btn_cancel" value="<%= m_refmsg.getmessage("close title")%>" onclick="top.close()"></td>
				</tr>
				<% Else %>
				<tr>
					<td><input type=button name="btn_cancel" value="Close" onclick="top.close()">
					<input type="hidden" name="commenttext" value="<%=Server.HtmlEncode(CommentText)%>" /></td>
				</tr>
				<% End If %>
			</table>
			<input type="hidden" name="netscape" Onkeypress="javascript:return CheckKeyValue(event,'34');">
			<input type="hidden" name="comment_id" value="<%=CommentId%>">
			<input type="hidden" name="commentkey_id" value="<%=CommentKeyId%>">
			<input type="hidden" name="ref_id" value="<%=RefId%>">
			<input type="hidden" name="LangType" value="<%=ContentLanguage%>">
			<input type="hidden" name="ref_type" value="<%=RefType%>">
			<input type="hidden" name="comment_type" value="<%=CommentType%>">
			<input type="hidden" name="user_id" value="">
			<input type="hidden" name="editorName" id="editorName" value="">
			<input type="hidden" name="orderyby" value="<%=OrderBy%>">
			<input type="hidden" name="ty" value="<%=(HttpUtility.HtmlEncode(Request.QueryString("ty")))%>">
		</form>
	</body>
</html>
<script Language="JavaScript1.2" type="text/javascript">
<!--
var CommentText =
{
	Comment:		"Comment"
,	OK: 			"OK"
,	Cancel:			"Cancel"

,	ErrPageClosed:	"Unable to save changes. The editor page has been closed."
}
function isMyTag(objInstance, objXmlTag)
{
	var strTagName = "mycomment";
	return (objXmlTag != null && objXmlTag.IsValid() && strTagName == objXmlTag.getPropertyString("TagName") && objInstance.editor.IsTagApplied(strTagName));
}


function insertElement()
{
	if (!eWebEditProUtil.isOpenerAvailable())
	{
		alert(CommentText["ErrPageClosed"]);
	}
	else
	{
		var objInstance = eWebEditProUtil.getOpenerInstance();
		if (objInstance)
		{
		    var objXmlDoc = objInstance.editor.XMLProcessor();
		    var objXmlTag = null;
		    if (objXmlDoc != null)
		    {
			    objXmlTag = objXmlDoc.ActiveTag();
		    }
		    if (objXmlDoc != null && isMyTag(objInstance, objXmlTag))
		    {
			    objXmlTag.SetTagAttribute("comment", objXmlDoc.EncodeAttributeValue(<%=CommentKeyId%>));
			    objXmlDoc.ApplyTag(objXmlTag);
		    }
		    else
		    {
			    var strHTML = '<mycomment comment="' + objXmlDoc.EncodeAttributeValue(eWebEditProUtil.HTMLEncode(<%=CommentKeyId%>)) + '" />';
			    objInstance.editor.pasteHTML(strHTML);
		    }
		}
		try
		{
			top.close();
		}
		catch (e)
		{
			// ignore
		}
	}
}
//-->
</script>
<%If ((Action="Add" AND Flag=True AND CommentType="NEW") OR (ResetCommentTag=True)) Then%>
<script language="JavaScript" type="text/javascript">
<!--
insertElement();
document.forms[0].comment_type.value="";
//-->
</script>
<% ElseIf "Add" = Action Then %>
<script language="JavaScript" type="text/javascript">
<!--
try
{
	top.close();
}
catch (e)
{
	// ignore
}
//-->
</script>
<%End If%>