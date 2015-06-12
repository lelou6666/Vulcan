<%@ Page Language="VB" AutoEventWireup="false" CodeFile="InterfaceEditInContext.aspx.vb" Inherits="ContentDesigner_configurations_InterfaceEditInContext" Theme="" %>
<root>
	<tools name="General" dockable="false">
		<tool name="EkInContextSave" />
		<tool separator="true"/>
		<tool name="AjaxSpellCheck" />
		<tool name="LinkManager" />
		<tool name="Unlink" />
		<%If (Not (IsNothing(Request.QueryString("LibraryAllowed"))) AndAlso True = Request.QueryString("LibraryAllowed")) Then%>
		<tool name="EkLibrary" />
		<%End If%>
		<tool separator="true"/>
	    <tool name="Bold" />
	    <tool name="Italic" />
	</tools>
	<tools name="Cancel" dockable="false">
	    <tool name="EkInContextCancel" />
	</tools>
	<contextMenus>
		<contextMenu forElement="IMG">
			<tool name="SetImageProperties"/>
		</contextMenu>
	</contextMenus >
</root>
