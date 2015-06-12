<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataEntryEdit.aspx.vb" Inherits="ContentDesigner_configurations_DataEntryEdit" %>

<root>
	<modules>
		<module name="RadEditorNodeInspector" dockingZone="Module" enabled="true" visible="true" dockable="false" />
		<!-- module name="EkRadEditorXhtmlValidator" dockingZone="Module" enabled="true" visible="true" dockable="false" /-->
		<!-- RadEditorHtmlInspector causes defect #30021 - cursor goes back and forth in the plain text field in data entry mode -->
		<module name="RadEditorHtmlInspector" dockingZone="Module" enabled="false" visible="false" />
	</modules>
	<tools name="Edit" enabled="true" dockable="false">
	    <tool separator="true"/>
		<tool name="SelectAll" />
		<tool separator="true"/>
		<tool name="Cut" />
		<tool name="Copy" />
		<tool name="Paste" />
		<tool name="PasteFromWordNoFontsNoSizes" />
		<tool name="PastePlainText" />
		<tool name="FindAndReplace" />
		<tool separator="true"/>
		<tool name="Undo" />
		<tool name="Redo" />
		<tool separator="true"/>
		<tool name="AjaxSpellCheck" />
		<tool separator="true"/>
		<tool name="InsertAnchor"/>
		<tool name="LinkManager" />
		<tool name="Unlink" />
		<tool name="EkLibrary" />
		<tool name="EkAddLinkPage" />
        <tool separator="true"/>
		<tool name="InsertHorizontalRule" />
		<tool name="InsertSymbol" />
		<!--<tool name="AbsolutePosition" />-->
		<tool separator="true"/>
		<tool name="EkValidateData" />
	</tools>
	<tools name="Format" dockable="false">
	    <tool separator="true"/>
	    <tool name="EkHideShowElements" />
		<tool name="ApplyClass"/>
	    <tool name="FormatBlock"/>
	    <tool separator="true"/>
	    <tool name="Bold" />
	    <tool name="Italic" />
	    <tool name="Underline" />
	    <tool name="StrikeThrough" />
	    <tool separator="true"/>
	    <tool name="Superscript" />
	    <tool name="Subscript" />
<%  If IsNothing(settings_data) OrElse settings_data.EnableFontButtons = True Then%>
		<tool name="FontName"/>
		<tool name="FontSize"/>
		<tool name="RealFontSize"/>
		<tool name="ForeColor" />
		<tool name="BackColor"/>
<% 	 	End If%>
	</tools>
	<tools name="Paragraph Format" enabled="true" dockable="false">
	    <tool separator="true"/>
		<tool name="InsertOrderedList" />
		<tool name="InsertUnorderedList"/>
		<tool name="Outdent" />
		<tool name="Indent" />
		<tool separator="true"/>
		<tool name="JustifyLeft" />
		<tool name="JustifyCenter" />
		<tool name="JustifyRight" />
		<tool name="JustifyFull" />
		<tool name="JustifyNone" />
	</tools>
	<tools name="Table" dockable="false" enabled="true">
	    <tool separator="true"/>
		<tool name="InsertTable" />
		<tool name="InsertRowAbove" />
		<tool name="InsertRowBelow" />
		<tool name="InsertColumnLeft" />
		<tool name="InsertColumnRight" />
		<tool name="DeleteRow" />
		<tool name="DeleteColumn" />
		<tool name="DeleteCell" />
		<tool name="MergeColumns" />
		<tool name="MergeRows" />
		<tool name="SplitCell" />
		<tool name="SetTableProperties" />
		<tool name="SetCellProperties" />
		<tool name="ToggleTableBorder" />
	</tools>
	<contextMenus>
		<contextMenu forElement="IMG">
			<tool name="SetImageProperties"/>
			<%If True = bCanModifyImg Then%>
			<tool name="EktronImageModificationTool"/>
			<%End If%>
		</contextMenu>
		<contextMenu forElement="*">
			<tool name="Cut" />
		    <tool name="Copy" />
		    <tool name="Paste" />
		    <tool name="PasteFromWordNoFontsNoSizes" />
		    <tool name="PastePlainText" />
		    <tool separator="true"/>
		    <tool name="EkRichAreaSource" />
		</contextMenu>
	</contextMenus >
</root>

