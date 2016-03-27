<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DmsMetadata.aspx.vb" Inherits="Workarea_DmsMetadata" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>DMS Metadata Application Page</title>
        <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
        <link href="csslib/ektron.workarea.css" rel="stylesheet" type="text/css" />
        <link href="csslib/ektron.fixedPositionToolbar.css" rel="stylesheet" type="text/css" />
        <script type="text/javascript" src="java/ektron.js"></script>
        <script type="text/javascript" src="java/ektron.workarea.js"></script>
        <style type="text/css">
            <!--/*--><![CDATA[/*><!--*/
                #dmsMetadata {padding:1em;margin:1em;}
                #dmsMetadata ul#dmsMetadataNavigation {width:100%;list-style:none;margin:0em;padding:0em;}
                #dmsMetadata ul#dmsMetadataNavigation li {display:block;float:left;margin:0em;margin-right:.25em;border-top:1px solid #4c4c4c;border-right:1px solid #4c4c4c;border-bottom:none;border-left:1px solid #4c4c4c;}
                #dmsMetadata ul#dmsMetadataNavigation li a {display:block;padding:.25em .5em;text-decoration:none;background-color:#adc5ef;color:#000080;}
                #dmsMetadata ul#dmsMetadataNavigation li a:hover {text-decoration:underline;}
                #dmsMetadata ul#dmsMetadataNavigation li a.selected {background-color:white;padding:.25em .5em;position:relative;top:1px;}
                #dmsMetadata div.wrapper {clear:left;padding:0em 1em 1em 1em;margin:0em;border:1px solid #4c4c4c;}
                #dmsMetadata div.wrapper p.nodata {margin:1em 0em 0em 0em;}
                #dmsMetadata div.wrapper div#metadata fieldset {margin:1em 0em 0em 0em;padding:0em 1em 1em 1em;}
                #taxonomy {margin:0em;padding:0em;}
                #taxonomy ul.ektree {display:block;z-index:3;white-space:nowrap;margin:0em 0em 0em -1em;padding:0px;list-style-type:none;}
                #taxonomy ul.ektree {width:96%; height:93%;}
                #taxonomy ul.ektree ul {list-style-type:none;}
                #taxonomy .ekTreeItem {padding-bottom:2px;padding-top:2px;padding-left:9px;list-style:none;margin-left:5px;} 
                #taxonomy .ekTreeRootItem {list-style:none;margin:0px;padding:0px;margin-left:-12px;top:0px;}
                #taxonomy ul.ekSubTree {margin-left:5px;}
                #taxonomy .hasChildIcon {width:31;height:14;vertical-align:middle;margin-right:4px;}
                #taxonomy a:hover {text-decoration:underline;color:navy;}
                #taxonomy .linkStyle {padding:1px;padding-left:2px;text-decoration: none;color:black;}
                #taxonomy .linkStyle_selected {padding-left:1px;border: dashed 1px black;}
                #taxonomy .loadingMessage {margin-top:5px;margin-left: 35px;width: 50px;color:#969696;}
			/*]]>*/-->
        </style>
        <!--[if lte IE 7]>
            <style type="text/css">
                table.ektronMetadataForm {width:97%;}
                ul.buttonWrapper {float:left;clear:both;padding-top:1em;}
                ul.buttonWrapper  li {float:left;}
            </style>
        <![endif]-->
        <asp:Literal ID="ltrStyleSheetJs" EnableViewState="false" runat="server" />
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--        
            
            var actionPage = '<asp:Literal ID="ltrActionPage" EnableViewState="false" runat="server" />';
            
            //hide the drag and drop uploader ////
			if (typeof top.HideDragDropWindow != "undefined")
			{				
				top.HideDragDropWindow();
			}
			
			function ResizeFrame(val) {
				if ((typeof(top.ResizeFrame) == "function") && top != self) {
					top.ResizeFrame(val); 
				}
			}
			
			function CanShowNavTree() {
				// Block displaying the navigation tree while this page loaded (called from top window-object):
				return false;
			}
            
            function SubmitForm(FormName, Validate, closeWindow) {
                if(!ValidateMeta(document.forms[0].name))
                    return false;
                if(!ValidateTax())
                    return false;
                
                var close = "";
			    if(top.location.href.indexOf("close=true") > 0 || document.location.search.indexOf("close=true")>0)
			        close = "&close=true";
			        
			    if(top.location.href.indexOf("closeWindow=true") > 0)
			        close = "&closeWindow=true";
                    
                if(closeWindow == "close")
                    close = "&close=true";

                document.forms[0].action = actionPage + "&action=Submit" + close;
			    document.forms[0].submit();
			    
			    return true;
		    }
		    
		    function CancelForm(FormName, Validate, closeWindow) {
		    
               	var close = "";
			    if(top.location.href.indexOf("close=true") > 0)
			        close = "&close=true";			    
			       
			    if(top.location.href.indexOf("closeWindow=true") > 0)
			        close = "&closeWindow=true"; 
                
                if(closeWindow == "close")
                    close = "&close=true";

                document.forms[0].action = actionPage + "&action=Cancel" + close;
			    document.forms[0].submit();
			    
			    return true;
		    }
            
            function dmsMetadataShowHideCategory(id)
            {
                var meta = document.getElementById('metadata');
                var metaAnchor = document.getElementById('metadataAnchor');
                var tax = document.getElementById('taxonomy');
                var taxAnchor =  document.getElementById('taxonomyAnchor');
                
                if( id == 'metadata' )
                {
                    if(meta)
                       document.getElementById('metadata').style.display = "block";
                    if(tax)
                        document.getElementById('taxonomy').style.display = "none";
                    if(metaAnchor)
                        document.getElementById('metadataAnchor').className = "selected";
                    if(taxAnchor)
                        document.getElementById('taxonomyAnchor').className = "";
                }
                if( id == 'taxonomy' )
                {
                    if(meta)
                        document.getElementById('metadata').style.display = "none";
                    if(tax)
                        document.getElementById('taxonomy').style.display = "block";
                    if(metaAnchor)
                        document.getElementById('metadataAnchor').className = "";
                    if(taxAnchor)
                        document.getElementById('taxonomyAnchor').className = "selected";
                }
            }
            //--><!]]>
        </script>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
            <asp:Literal ID="ltrTaxJS" EnableViewState="false" runat="server" />
            var taxonomytreearr='<asp:Literal ID="ltrTaxonomyTreeIdList" EnableViewState="false" runat="server" />'.split(",");
            var taxonomytreedisablearr='<asp:Literal ID="ltrTaxonomyTreeParentIdList" EnableViewState="false" runat="server" />'.split(",");
            var __EkFolderId='<asp:Literal ID="ltrTaxFolderId" EnableViewState="false" runat="server" />';
            var __TaxonomyOverrideId='<asp:Literal ID="ltrTaxonomyOverrideId" EnableViewState="false" runat="server" />';
            function fetchtaxonomyid(pid){
                for(var i=0;i<taxonomytreearr.length;i++){
                    if(taxonomytreearr[i]==pid){
                        return true;
                        break;
                    }
                }
                return false;
            }
             function fetchdisabletaxonomyid(pid){
                for(var i=0;i<taxonomytreedisablearr.length;i++){
                    if(taxonomytreedisablearr[i]==pid){
                        return true;
                        break;
                    }
                }
                return false;
            }
            function updatetreearr(pid,op){
                if(op=="remove"){
                    for(var i=0;i<taxonomytreearr.length;i++){
                        if(taxonomytreearr[i]==pid){
                            taxonomytreearr.splice(i,1);break;
                        }
                    }
                }
                else{
                    taxonomytreearr.splice(0,0,pid);
                }
                document.getElementById("taxonomyselectedtree").value="";
                for(var i=0;i<taxonomytreearr.length;i++){
                    if(document.getElementById("taxonomyselectedtree").value==""){
                        document.getElementById("taxonomyselectedtree").value=taxonomytreearr[i];
                    }else{
                        document.getElementById("taxonomyselectedtree").value=document.getElementById("taxonomyselectedtree").value+","+taxonomytreearr[i];
                    }
                }
            }
           function selecttaxonomy(control){
            var pid=control.value;
            if(control.checked)
            {
                updatetreearr(pid,"add");
            }
            else
            {
                updatetreearr(pid,"remove");
            }
            var currval=eval(document.getElementById("chkTree_T"+pid).value);
            var node = document.getElementById( "T" + pid );
            var newvalue=!currval;
            document.getElementById("chkTree_T"+pid).value=eval(newvalue);
            if(control.checked)
              {
                Traverse(node,true);
              }
            else
              {
                Traverse(node,false);
                var hasSibling = false;
                if (taxonomytreearr != "")
                  { for(var i = 0 ;i<taxonomytreearr.length;i++)
                        {
                          if(taxonomytreearr[i] != "")
                            {
                              var newnode = document.getElementById( "T" + taxonomytreearr[i]);
                                if(newnode != null && newnode.parentNode == node.parentNode)
                                   {Traverse(node,true);hasSibling=true;break;}
                            }
                        }
                  }
                if(hasSibling == false)
                { 
                 checkParent(node);
                }  
              }
          }
   
            function checkParent(node)
            { if(node!= null)
                {
                      var subnode = node.parentNode;
                      if(subnode!=null && subnode.id!="T0" &&  subnode.id!="")
                      {
                                for(var j=0;j<subnode.childNodes.length;j++)
                                  {var pid=subnode.childNodes[j].id;
                                   if(document.getElementById("chkTree_"+pid).value == true || document.getElementById("chkTree_"+pid).value == "true")
                                      {Traverse(subnode.childNodes[j],true);return;}
                                  }
                       checkParent(subnode.parentNode);
                      }
                }
            }
            function Traverse(node,newvalue){
                if(node!=null){
                    subnode=node.parentNode;
                     if(subnode!=null && subnode.id!="T0" &&  subnode.id!=""){
                        for(var j=0;j<subnode.childNodes.length;j++){
                            var n=subnode.childNodes[j]
                            if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox"){
                                var pid=subnode.id;
                                updatetreearr(pid.replace("T",""),"remove");
                                document.getElementById("chkTree_"+pid).value=eval(newvalue);
                                n.setAttribute("checked",eval(newvalue));
                                n.setAttribute("disabled",eval(newvalue));
                                
                            }
                        }
                        if(HasChildren(subnode) && subnode.getAttribute("checked")){
                               subnode.setAttribute("checked",true);
                                subnode.setAttribute("disabled",true);  
                        }
                        Traverse(subnode,newvalue);
                    }
                }
            }
            function HasChildren(subnode)
            {
                if(subnode!=null){
                    for(var j=0;j<subnode.childNodes.length;j++)
                    {
                        for(var j=0;j<subnode.childNodes.length;j++){
                            var n=subnode.childNodes[j]
                            if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox"){
                                var pid=subnode.id;
                                var v=document.getElementById("chkTree_"+pid).value;
                                if(v==true || v=="true"){
                                return true;break;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            
            function CloseThickBoxandReload()
            {
                if (parent != null && typeof parent.ektb_remove == 'function')
                {
                    var invalidFileMsg = '<asp:literal id="jsInvalidFileTypeMsg" runat="server"/>'; 
                    if (invalidFileMsg.length > 0)
                        alert(invalidFileMsg);
                    parent.ektb_remove();
                    if(top.frames["ek_main"] == null) // if Not In workarea, just close the thickbox, else reload underlying page
                        parent.location.href = parent.location.href.replace('#','') + ((parent.location.href.indexOf('?') > -1) ? '&' : '?') + 'addDocument=true';
                }
            }
            
            //--><!]]>
        </script>
        <script type="text/javascript" src="java/searchfuncsupport.js"></script>
        <script type="text/javascript" src="java/jfunct.js"></script>
		<script language="JavaScript" src="java/internCalendarDisplayFuncs.js" type="text/javascript"></script>
        
        <asp:Literal id="EnhancedMetadataScript" runat="server"></asp:Literal>
        <asp:Literal id="EnhancedMetadataArea" runat="server" />
        
    </head>
    <body>
		<div id="dhtmltooltip"></div>
		<div class="ektronPageHeader">
		    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <form id="form1" runat="server">
            <div class="ektronPageContainer ektronPageInfo" id="dmsMetadata">
                <ul id="dmsMetadataNavigation">
                    <asp:Literal ID="ltrShowMetadata" EnableViewState="false" runat="server" />
                    <asp:Literal ID="ltrShowTaxonomy" EnableViewState="false" runat="server" />
                </ul>
                <div class="wrapper">
                    <div id="metadata">
                        <asp:Literal ID="myMetadata" runat="server"></asp:Literal>
                    </div>
                     <div id="taxonomy" style="display:none;">
                        <asp:Literal ID="myTaxonomy" runat="server"></asp:Literal>
                        <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
                        <input type="hidden" name="LastClickedOnChecked" id="LastClickedOnChecked" value="false" />
                        <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value="" runat="server" />
                        <!--#include file="common/taxonomy_editor_menu.inc" -->
                        <script type="text/javascript">var taxonomytreemode="editor";</script>
                        <script type="text/javascript" src="Tree/js/com.ektron.utils.url.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.explorer.init.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.explorer.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.explorer.config.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.explorer.windows.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.cms.types.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.cms.parser.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.cms.toolkit.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.cms.api.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.ui.contextmenu.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.ui.iconlist.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.ui.tabs.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.ui.explore.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.ui.taxonomytree.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.net.http.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.lang.exception.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.utils.form.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.utils.log.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.utils.dom.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.utils.debug.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.utils.string.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.utils.cookie.js"></script>
                        <script type="text/javascript" src="Tree/js/com.ektron.utils.querystring.js"></script>
                        <script type="text/javascript">
                            <!--//--><![CDATA[//><!--
                            
                            var metaExist = document.getElementById("metadataAnchor");
                            if(!metaExist)
                                dmsMetadataShowHideCategory("taxonomy");
                                
                            ResizeFrame(0);
                            
                            var clickedElementPrevious = null;
                            var clickedIdPrevious = null;

                            function onDragEnterHandler( id, element ){
                            folderID = id;
                            if( clickedElementPrevious != null ) {
                            clickedElementPrevious.style["background"] = "#ffffff";
                            clickedElementPrevious.style["color"] = "#000000";
                            }
                            element.style["background"] = "#3366CC";
                            element.style["color"] = "#ffffff";
                            }

                            function onMouseOverHandler( id, element ){
                            element.style["background"] = "#ffffff";
                            element.style["color"] = "#000000";
                            }

                            function onDragLeaveHandler( id, element ) {
                            element.style["background"] = "#ffffff";
                            element.style["color"] = "#000000";
                            }

                            function onFolderClick( id, clickedElement ){
                            if( clickedElementPrevious != null ) {
                            clickedElementPrevious.style["background"] = "#ffffff";
                            clickedElementPrevious.style["color"] = "#000000";
                            }

                            clickedElement.style["background"] = "#3366CC";
                            clickedElement.style["color"] = "#ffffff";
                            clickedElementPrevious = clickedElement;
                            clickedIdPrevious = id;

                            var name = clickedElement.innerText;
                            var folder = new Asset();
                            folder.set( "name", name );
                            folder.set( "id", id );
                            folder.set("folderid",__EkFolderId);
                            __EkFolderId=-1;
                            }

                            function onToggleClick( id, callback, args ){
                            toolkit.getAllSubCategory( id, -99, callback, args );
                            }

                            function makeElementEditable( element ) {
                            element.contentEditable = true;
                            element.focus();
                            element.style.background = "#fff";
                            element.style.color = "#000";
                            }

                            var baseUrl = URLUtil.getAppRoot(document.location) + "tree/images/xp/";
                            TreeDisplayUtil.plusclosefolder  = baseUrl + "plusclosetaxonomy.gif";
                            TreeDisplayUtil.plusopenfolder   = baseUrl + "plusopentaxonomy.gif";
                            TreeDisplayUtil.minusclosefolder = baseUrl + "minusclosetaxonomy.gif";
                            TreeDisplayUtil.minusopenfolder  = baseUrl + "minusopentaxonomy.gif";
                            TreeDisplayUtil.folder = baseUrl + "taxonomy.gif";

                            var g_menu_id = "";
                            function displayCategory( categoryRoot ) {
                            document.body.style.cursor = "default";
                            var taxonomyTitle = null;
                            try {
                            taxonomyTitle = categoryRoot.title;
                            g_menu_id = categoryRoot.id;
                            } catch( e ) {
                            ;
                            }

                            if( taxonomyTitle != null ) {
                            treeRoot = new Tree( taxonomyTitle, __TaxonomyOverrideId, null, categoryRoot );
                            TreeDisplayUtil.showSelf( treeRoot, document.getElementById( "TreeOutput" ) );
                            TreeDisplayUtil.toggleTree( treeRoot.node.id );
                            } else {
                            var element = document.getElementById( "TreeOutput" );
                            var debugInfo = "<b>Cannot connect to the service</b>";
                            element.innerHTML = debugInfo;
                            }
                            }

                            var toolkit = new EktronToolkit();
                            toolkit.getTaxonomy( __TaxonomyOverrideId, -99, displayCategory, __TaxonomyOverrideId );

                            function reloadTreeRoot( id ){
                            TREES = {};
                            toolkit.getTaxonomy( id, -99, displayCategory, __TaxonomyOverrideId );
                            }

                            var g_selectedFolderList = "0";
                            var g_timerForFolderTreeDisplay;
                            function showSelectedFolderTree(){
                            if (g_timerForFolderTreeDisplay){
                            window.clearTimeout(g_timerForFolderTreeDisplay);
                            }
                            g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
                            }

                            function showSelectedFolderTree_delayed() {
                            var bSuccessFlag = false;
                            if (g_timerForFolderTreeDisplay){
                            window.clearTimeout(g_timerForFolderTreeDisplay);
                            }

                            if (g_selectedFolderList.length > 0){
                            var tree = TreeUtil.getTreeById(g_menu_id);
                            if (tree){
                            var lastId = 0;
                            var folderList = g_selectedFolderList.split(",");
                            bSuccessFlag = TreeDisplayUtil.expandTreeSet( folderList );
                            }

                            if (!bSuccessFlag){
                            g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
                            }
                            }
                            }
                            //--><!]]>
                        </script>
                    </div>
                </div>
            </div>
        </form>
    </body>
</html>
