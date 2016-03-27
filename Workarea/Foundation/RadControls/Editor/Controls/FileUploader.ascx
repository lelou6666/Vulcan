<%@ Control Language="c#" AutoEventWireup="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" CodeBehind="FileUploader.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.FileUploader" %>
<table border="0" cellpadding="3" cellspacing="0">
	<tr id="messageHolderRow" runat="server" visible="false">
		<td colspan="4" valign="top" class="label">
			<span id="message" runat="server" class="ErrorMessage"></span>
		</td>
	</tr>
	<tr>
		<td nowrap class="label">
			<script>localization.showText('Directory');</script>
		</td>
		<td colspan="2">
			<input type="text" id="CurrentDirectoryBox" style="width:350px" size=50 class="RadETextBox">
		</td>
		<td rowspan="4" valign="top">
			<button runat="server" class="Button" onclick="buttonAction(submitButtonAction, event);if (!submitForUpload) return false;" type="button" id="btnUpload">
				<script>localization.showText('Upload');</script>
			</button>
		</td>
	</tr>
	<tr>
		<td nowrap class="label" valign=top>
			<script>localization.showText('File');</script>
		</td>
		<td id="frameHolderTemp" colspan="2" class="Label">
			<input style="width:340px"  type="file" id="FileUpload" size=50 runat="server" class="File" name="FileUpload">
			<input type="hidden" id="fileDir" runat="server" name="fileDir">
			<br>
			<input type="checkbox" id="cbOverwriteExisting" runat="server"><script language="javascript">localization.showText('OverwriteExisting');</script>
			<br><b><script>localization.showText('NoteMaxSize');</script></b> <asp:label id="maxFileSize" runat="server" />KB
			<asp:Label ID="lblfileExtensionsHolder" Runat="server"><br><b><script>localization.showText('NoteFileExtensions');</script></b> <asp:Label ID="allowedFileExtensions" Runat="server"/></asp:Label>
		</td>
	</tr>
	<tr>
		<td colspan="2" class="label" style="height:20px;">
			<span id="loader" style="PADDING-LEFT:5px"></span>
		</td>
	</tr>
</table>