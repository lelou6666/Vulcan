<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataDesignerEdit.aspx.vb" Inherits="ContentDesigner_configurations_DataDesignerEdit" %>
<root>
	<modules>
		<!-- module name="EkRadEditorXhtmlValidator" dockingZone="Module" enabled="true" visible="true" dockable="false" /-->
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
		<tool separator="true"/>
		<tool name="InsertHorizontalRule" />
		<tool name="InsertSymbol" />
		<!--<tool name="AbsolutePosition" />-->
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
<%		If IsNothing(settings_data) OrElse settings_data.EnableFontButtons = True Then%>
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
	<!-- Ektron Editor Start -->
	<tools name="Data Designer" dockable="false">
	    <tool separator="true"/>
		<tool name="EkGroupBox" />
		<tool name="EkTabularDataBox" />
		<tool name="EkTextField" />
		<tool name="EkChoicesField" />
		<tool name="EkCheckBoxField" />
		<tool name="EkCalculatedField" />
		<tool name="EkCalendarField" />
		<tool name="EkImageOnlyField" />
		<tool name="EkFileLinkField" />
		<tool name="EkResourceSelectorField" />
		<tool separator="true"/>
		<tool name="EkFieldProp" />
		<tool name="EkValidateDesign" />  
		<tool name="EkCompatibility" />
        <tool name="EkPreviewXml" />
		<tool name="EkPreviewXsd" />
		<tool name="EkPreviewFld" />
		<tool name="EkPreviewNdx" />
		<tool name="EkPreviewXsl" />
		<tool separator="true"/>
		<tool name="EkValidateData" />
	</tools>
	<dialogParameters>
		<dialog name="GroupBox"/>
		<dialog name="TabularDataBox"/>
		<dialog name="CheckBoxField"/>
		<dialog name="PlainTextField"/>
		<dialog name="ChoicesField"/>
		<dialog name="CalculatedField"/>
		<dialog name="CalendarField"/>
		<dialog name="ImageOnlyField"/>
		<dialog name="FileLinkField"/>
	</dialogParameters>
	<contextMenus>
	    <contextMenu forElement="TABLE">
		    <tool name="ToggleTableBorder"/>
		    <tool name="SetTableProperties"/>
		    <tool name="DeleteTable"/>
		    <tool name="EkFieldProp"/>
		</contextMenu>
		<contextMenu forElement="TD">
		    <tool name="InsertRowAbove"/>
		    <tool name="InsertRowBelow"/>
		    <tool name="DeleteRow"/>
		    <tool name="InsertColumnLeft"/>
		    <tool name="InsertColumnRight"/>
		    <tool name="DeleteColumn"/>
		    <tool name="MergeColumns"/>
		    <tool name="MergeRows"/>
		    <tool separator="true"/>
		    <tool name="SplitCell"/>
		    <tool name="DeleteCell"/>
		    <tool name="SetCellProperties"/>
		    <tool name="SetTableProperties"/>
		    <tool name="ToggleTableBorder"/>
		    <tool name="EkFieldProp"/>
		</contextMenu>
		<contextMenu forElement="IMG">
			<tool name="SetImageProperties"/>
			<%If True = bCanModifyImg Then%>
			<tool name="EktronImageModificationTool"/>
			<%End If%>
			<tool name="EkFieldProp"/>
		</contextMenu>
		<contextMenu forElement="*">
		    <tool name="Cut"/>
		    <tool name="Copy"/>
		    <tool name="Paste"/>
		    <tool name="PasteFromWordNoFontsNoSizes"/>
		    <tool name="PastePlainText"/>
		    <tool name="EkFieldProp"/>
		</contextMenu>
	</contextMenus >
	<!-- Ektron Editor ends -->	
</root>
