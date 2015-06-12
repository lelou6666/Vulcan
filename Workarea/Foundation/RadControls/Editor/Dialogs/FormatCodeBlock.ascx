<%@ Control Language="c#" AutoEventWireup="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<style>
	body, td, option {
		font-family: Verdana;
		font-size: 11px;
	}
</style>
<table width ="100%" class="MainTable">
	<tr>
		<td colspan="3" class="text"><script>localization.showText('PasteSourceCode');</script></td>
	</tr>
	<tr>
		<td colspan="3">
			<textarea id="SourceCode" style="width:100%;height:180;"></textarea>
		</td>
	</tr>

	<tr>
		<td colspan="3">
		<table cellpadding="0" cellspacing="0" border="0" width="100%">
			<tr>
				<td>
					<label for="CodingLanguage"><script>localization.showText('CodeLanguage');</script></label>
				</td>
				<td>
					<select id="CodingLanguage">
						<option value ="Xml">Markup - (x)HTML, XML, ASPX, ...</option>
						<option value ="JScript">JavaScript</option>
						<option value ="CSS">CSS</option>
						<option value ="CSharp">C#</option>		
						<option value ="Vb">VB</option>	
						<option value ="Php">Php</option>
						<option value ="Sql">SQL</option>
						<option value ="Delphi">Delphi</option>
						<option value ="Python">Python</option>
						
					</select>
				</td>
				<td>
					<label for="SnippetMaxHeight"><script>localization.showText('SnippetMaxHeight');</script></label>
				</td>
				<td>
					<input type="text" id="SnippetMaxHeight" value ="500" style="width:50px">
				</td>
				<td> px/%</td>

				<td><span id = "InfoLabel" style = "background-color:white"></span></td>

			</tr>
			<tr>
				<td>
					<label for="ShowLineNumbers"><script>localization.showText('ShowLineNumbers');</script></label>
				</td>
				<td>
					<input type = "checkbox" name="ShowLineNumbers" id="ShowLineNumbers">
				</td>

				<td>
					<label for="SnippetMaxWidth"><script>localization.showText('SnippetMaxWidth');</script></label>
				</td>
				<td>
					<input type="text" id="SnippetMaxWidth" value ="100%" style="width:50px">
				</td>
				<td> px/%</td>
				<td>
					<button class ="Button" onmousedown="document.getElementById('InfoLabel').innerHTML = localization.getText('Working');" onmouseup="PerformHighliting()" onclick="return false;"><script>localization.showText('Preview');</script></button>
				</td>
			</tr>
		</table>
		</td>
	</tr>

	<tr>
		<td colspan="3" class="text">
			
			<script>localization.showText('PreviewCode');</script>
			<style>
				.codesnippet td {
					font-family: Courier New;font-size: 11px;
				}
			</style>
			<div id = "FormattedCode" style = "overflow:auto;background-color:white;height:180px;border:solid 1px #7f9db9;width:690px;line-height: 100% !important;" class="codesnippet"></div>
		</td>
	</tr>
</table>

<div align="right">
<button class ="Button" onclick="insertCode()"><script>localization.showText('Insert');</script></button>
<button class ="Button" onclick="Cancel()"><script>localization.showText('Cancel');</script></button>
</div>
<script>
	var arr = [];
	var isCodeHighlighted = false;
	// the actual highlite is done here
	function PerformHighliting() {
	
		// get the selected coding language from the dropdown
		var clDD = document.getElementById("CodingLanguage");
		var language = clDD.options[clDD.selectedIndex].value;
		
		// get not formatted source code
		var code = document.getElementById('SourceCode').value;
		
		// do we have to show line numbers?
		var showLineNumbers = document.getElementById('ShowLineNumbers').checked;
		
		// get the formatted code from the plain code 
		var formattedCode = SyntaxHighlighter.HighlightAll(code, language, showLineNumbers);

		// show the formatted code to user
		document.getElementById("FormattedCode").innerHTML   = formattedCode;

		document.getElementById("InfoLabel").innerHTML = "";
		isCodeHighlighted = true;
	}

</script>
	 <script language="javascript">
		function GetDialogArguments()
		{
			if (window.radWindow) 
			{
				return window.radWindow.Argument;
			}
			else
			{
				return null;
			}
		}

		var isRadWindow = true;
		var radWindow = GetEditorRadWindowManager().GetCurrentRadWindow(window);
		if (radWindow)
		{ 
			if (window.dialogArguments) 
			{ 
				radWindow.Window = window;
			} 
			radWindow.OnLoad(); 
		}
		
		// after we are done - call the custom callback and
		// pass the formatted code as parameter
		function insertCode()
		{
			// make sure the code is highlighted before to insert it
			if (!isCodeHighlighted) {
				PerformHighliting();
			}
		
			var codeHolder = document.getElementById("FormattedCode");
			var codeTable = codeHolder.getElementsByTagName('table')[0];
			
			// get the maxWidth/maxHeight of the code snippet
			var width = document.getElementById('SnippetMaxWidth').value;
			var heightOffset = 20;
			var height = codeTable.offsetHeight + heightOffset;
			
			var maxHeight = document.getElementById('SnippetMaxHeight').value;
			height = (height > maxHeight) ? maxHeight : height;
			
			// pass here the formatted code
			var returnValue = 
			{
				formattedCode : "<div style='overflow:auto;background-color:white;border:solid 1px #7f9db9;width:" + width + ";height:" + height + ";line-height: 100% !important;font-family: Courier New;font-size: 11px;'>" + document.getElementById("FormattedCode").innerHTML + "</div>"
			};

			// close the dialog and supply the formatted code to the callback function
			CloseDlg(returnValue);
		}
		
		function Cancel() {
			CloseDlg();
		}
		
	</script>
