<%@ Page Language="VB" AutoEventWireup="false" CodeFile="InterfaceBlog.aspx.vb" Inherits="ContentDesigner_configurations_InterfaceBlog" %>

<root>
	<modules>
		<!-- RadEditorHtmlInspector causes defect #30021 - cursor goes back and forth in the plain text field in data entry mode -->
		<module name="RadEditorHtmlInspector" dockingZone="Module" enabled="false" visible="false" />
	</modules>
	<tools name="Edit" enabled="true" dockable="false">
		<%If IsForum And (options.ContainsKey("clipboardmenu") = False) Then%>
	    <%Else%>
	    <tool separator="true"/>
		<tool name="Cut" />
		<tool name="Copy" />
		<tool name="Paste" />
		<tool name="PasteFromWordNoFontsNoSizes" />
		<tool name="PastePlainText" />
		<tool name="Undo" />
		<tool name="Redo" />
		<tool separator="true"/>
		<% End If%>
		<%If IsForum And (options.ContainsKey("stylemenu") = False) Then%>
		<%Else%>
		<tool name="FormatBlock"/>
		<% End If%>
		<%  If IsForum And (options.ContainsKey("fontmenu") = False) Then%>
        <%Else%>
		<tool name="FontName"/>
		<tool name="FontSize"/>
		<tool name="RealFontSize"/>
		<tool name="ForeColor" />
		<tool separator="true"/>
		<% End If%>
		 <%  If IsForum And (options.ContainsKey("textformatmenu") = False) Then%>		
         <%Else%>
		<tool name="Bold" />
		<tool name="Italic" />
		<tool name="Underline" />
		<tool separator="true"/>	
		<tool name="AjaxSpellCheck" />
		<tool separator="true"/>
		<% End If%>
        <%If IsForum And (options.ContainsKey("linkmenu") = False) Then%>
		<%Else %>		     		
		<tool name="LinkManager" />
		<tool name="Unlink" />
		<% End If%>
		<%If (Not (IsNothing(Request.QueryString("LibraryAllowed"))) AndAlso True = Request.QueryString("LibraryAllowed")) OR (IsForum AndAlso (options.ContainsKey("library") = True)) Then%>
		<tool name="EkLibrary" />
		<%End If%>
		<%If IsForum And (options.ContainsKey("symbolsmenu") = False) Then%>
		<%Else%>
		<tool name="InsertSymbol" />
		<% End If%>
	</tools>	
	<tools name="Paragraph Format" enabled="true" dockable="false">
		 <%  If IsForum And (options.ContainsKey("paragraphmenu") = False) Then%>
	     <%Else%>
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
		<tool separator="true"/>
		<tool name="InsertHorizontalRule" />
		<%End If%>
		
		<%If (Not (IsNothing(Request.QueryString("wiki")))) Then%>
		<tool name="EkAddLinkPage" />
		<%End If%>
	</tools>
	<tools name="Table" dockable="false" enabled="true">
	   	<%If IsForum And (options.ContainsKey("table") = False) Then%>
	   	<%Else%>
		<tool name="InsertTable" />
		<%End If %>
		<%If (IsForum And (options.ContainsKey("wmv") = False)) Then%>
	   	<%Else%>
		<tool name="InsertWMV" />
		<%End If%>
		<%If (IsForum And (options.ContainsKey("emoticonselect") = False)) Then%>
	   	<%Else%>
		<tool name="EkEmoticonSelect" />
		<%End If%>
	</tools>
	<contextMenus>
		<contextMenu forElement="IMG">
			<tool name="SetImageProperties"/>
			<%If True = bCanModifyImg Then%>
			<tool name="EktronImageModificationTool"/>
			<%End If%>
		</contextMenu>
	</contextMenus >
</root>

