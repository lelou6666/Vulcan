<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ManageCommunityDocuments.aspx.vb"
    Inherits="Workarea_Community_ManageCommunityDocuments" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../controls/Editor/ContentDesignerWithValidator.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        body {font-family: Arial, Helvetica, sans-serif; font-size: .92em;}
       form#form1{
            display:none;
        }
        div#ekOnemoment {position: absolute; top: 2em; left: 15em;color:red;}
        div.ekOnemomentHidden {visibility: hidden;}
        div.ekOnemomentShow {visibility: visibile;}
        input.buttonNext {display: none;}
        .greenHoverDD:hover {background-color:#E6EFC2;border:1px solid #C6D880;color:#529214;}
        .redHoverDD:hover {background-color:#fbe3e4;border:1px solid #fbc2c4;color:#d12f19;}
        .buttonDD {display:block;margin:0;background-color:#f5f5f5;border:1px solid #dedede;border-top:1px solid #eee;border-left:1px solid #eee;font-size:100%;line-height:100%;text-decoration:none;color:#565656;cursor:pointer;padding:5px 10px 6px 7px;}
        .buttonLeftDD {float:Left;}
        .WaitLoadDD {padding: 0 auto;margin-top: 50px;text-align:center;}
    </style>
    <script type="text/javascript" src="managecommunitydocuments.js"></script>
   
    <script type="text/javascript" >
        function ResizeContainer()
        {
            var obj = parent.document.getElementById("EkTB_iframeContent");
            if (obj){
                obj.style.height = "175px";                
            }
        }
        function HideContainer(obj)
        {
            var title = document.getElementById("ekImagegalleryTitle");
            var titleText = title.value;
            if(titleText.indexOf('\'') != -1 || titleText.indexOf('\"') != -1 || titleText.indexOf('*') != -1 || titleText.indexOf('&') != -1 || titleText.indexOf('^') != -1)
            {
                alert('Content Title can not contain \', *, &, ^ or \".');
                return false;
            }

            var objParent = obj.parentNode.parentNode.parentNode;
            var objectToShow = document.getElementById("ekOnemoment");
            var objectToResize = parent.document.getElementById("EkTB_iframeContent");
            if(objectToResize)
            {
             objectToResize.style.height = "100px";
            }
            objParent.style.visibility = 'hidden';
            objectToShow.className='ekOnemomentShow';
            return true;
        }
        
        function Close()
        {
            if (parent != null && parent != self && typeof parent.ektb_remove == 'function')
            {
                parent.ektb_remove();
                if (parent.$ektron("span.bc_current").length > 0 && parent.$ektron("#taxonomyselectedtree").length > 0) {
                    // request came from a Community Documents control
                    // refresh the control
                    parent.$ektron("span.bc_current:last").parent().parent().click(); //FF
                    parent.$ektron("span.bc_current:last").click(); //IE
                }
                else {
                    parent.location.href = parent.location.href;
                }
            } else {
                if (top.opener.$ektron("span.bc_current").length > 0 && top.opener.$ektron("#taxonomyselectedtree").length > 0) {
                    // request came from a Community Documents control
                    // refresh the control
                    top.opener.$ektron("span.bc_current:last").parent().parent().click(); // FF
                    top.opener.$ektron("span.bc_current:last").click(); // IE
                }
                else {
                    top.opener.location.href = top.opener.location.href;
                }              
                self.close();
            }
        }
        
        $ektron.addLoadEvent(function(){
                $ektron("input.buttonNext").show();
            }
        );
        
        function uploadClick()
		{
		    var uploadCheck  = document.getElementById("isFileUploadComplete");
		    if(uploadCheck != null)
		        {
		            if(uploadCheck.value == "true")
		            {
		                var m_Url = '<asp:literal id="jsMetaUrl" runat="server"/>';
		                if(m_Url != "")
		                    self.parent.ektb_show("Add asset data", m_Url);
		                else           
		                   closeDialog();
		             }
		            else
		                setTimeout("uploadClick();",  500);
		        }
		}
		
		function showLoadingBar()
		{
		
		   var waitImg  = document.getElementById("ek_DmsFileUploadWait");
		   var tContent =  document.getElementById("DragDropUI") ;
		   if(tContent != null)
		    {
		            tContent.style.position="relative";
		            tContent.style.left="-10000px";
                    
		    }
		   if(waitImg != null)
		    {
		            waitImg.style.position =  'relative';
		            waitImg.style.top = '-150px';
                    waitImg.style.left = '10px';
                   
		    }
		}
		
		function CheckFileExists()
		{
		  var folder_id = '<asp:literal id="jsFolderID" runat="server"/>';
		  var lang_id = '<asp:literal id="jsLanguageID" runat="server"/>';
		  PageMethods.CheckFileExists(document.getElementById('ekFileUpload').value,folder_id,lang_id,FileExistsCallBack);
		  return false;
		}
		
		function toggleSupportedFileTypes()
		{
		   $ektron("#divFileTypes").toggle("slow");
		    var toggleText =  $ektron(".toggleSupportedFileTypes").text();
		    if(toggleText=="Show File Types")
		    {
		       $ektron(".toggleSupportedFileTypes").text("Hide File Types");
		    }
		    else
		    {
		       $ektron(".toggleSupportedFileTypes").text("Show File Types");
		    }
		}
		
		function FileExistsCallBack(result)
        {
             if(result) 
             {
                if(confirm("File already exists. Do you want to overwrite"))
                {
                     showLoadingBar();
                     __doPostBack('uploadFile','');
                }
                else
                     return false;
                     
             }
            else
            {
                showLoadingBar();
               __doPostBack('uploadFile','');
            }
       }
		
	    $ektron(document).ready(function() {
	        var uploadCheck  = document.getElementById("isFileUploadComplete");
	        if (!(ShowMultipleUpload() && CheckSTSUpload()))
            {
               if(document.getElementById("liMultipleDMS")!= null)
              	  document.getElementById("liMultipleDMS").style.display ="none";
               if(document.getElementById("tabMultipleDMS")!= null)  
                  document.getElementById("tabMultipleDMS").style.display ="none";
            }
           
            if(uploadCheck !== null)
            {
		        if(uploadCheck.value == "invalid")
	            {
		           var ddTabs = $ektron("#DMStabs").tabs({
                        select: function(event, ui)
                        {
                            ResizeDocumentContainer(ui);
                        }
                    });
		           ddTabs.tabs('select', 0);
		        }
                else
                {
                   $ektron("#DMStabs").tabs({
                        select: function(event, ui)
                        {
                            ResizeDocumentContainer(ui);
                        }
                    });
                }
	        }
	        else
	        {
	              $ektron("#DMStabs").tabs({
                        select: function(event, ui)
                        {
                            ResizeDocumentContainer(ui);
                        }
                    });	   
	        }
	        $ektron("form#form1").show();
	        if ($ektron(".confirmationURL").length > 0 && $ektron("#NextUsing").length > 0)
	            $ektron(".confirmationURL").attr("value", $ektron("#NextUsing").val());
	    });
	    
	    function ResizeDocumentContainer(ui)
	    {
            if (self.parent !== null)
            {
                var iframeParent = $ektron(self.parent.document);
                var iframes = iframeParent.find("iframe");
                var dragDropIframeIsThickbox = false;
                var dragDropIframe, iframeParentWrapper;
                iframes.each(function(i)
                {
                       try
                       {
                        var contents = $ektron(this).contents();
                        var hasMyDragDrop = (contents.find("div#DMStabs a.tabFileUpload").length > 0) ? true : false;
                        if (hasMyDragDrop)
                        {
                           dragDropIframe = iframes.eq(i);
                           iframeParentWrapper = dragDropIframe.parent();
                        }
                        }
                       catch(e)
                       {
                       }
                });
                if (dragDropIframe.is("#EkTB_iframeContent"))
                {
                    dragDropIframeIsThickbox = true;
                }
                // set new height and width
                var newHeight, newWidth, tabClicked;
                tabClicked = $ektron(ui.tab);
                switch (tabClicked.text())
                {
                    case "Multiple DMS Documents":
                                    newHeight = "500";
                                    newWidth = "700";
                                    newIframeHeight = "550";
                                    break;
                    default:
                                    // file upload
                                    newHeight = "200";
                                    newWidth = "525";
                                    newIframeHeight = "180";
                                    break;
                }
                // raise an event that indicates that a DMS tab was clicked
                if (self.parent.$ektron)
                {
                    self.parent.$ektron(self.parent.document).trigger({
                        type: "ektronDmsTabClicked",
                        tab: tabClicked,
                        iframe: dragDropIframe
                    });
            }

            dragDropIframe.css({
               "width": "100%" ,
               "height": newIframeHeight          
            });
                if (dragDropIframeIsThickbox)
                {
            iframeParentWrapper.css({
                "width": newWidth + "px",
                "height": newHeight + "px",
                "margin-left": -(parseInt(newWidth/2,10)) + "px",
                "margin-top": -(parseInt(newHeight/2,10)) + "px"
            });
	    }
            }
    	 }
	    
	    function MultipleDocumentUpload()
        {          
            if(document.all.idUploadCtl != null)
            {
                document.all.idUploadCtl.MultipleUpload();
            }
          	document.forms[0].action = "../processupload.aspx";
            document.forms[0].submit();
            return false;
        }
	
	
		function cancelDialog()
		{
		  self.parent.ektb_remove();      
		}
	   
	   function closeDialog()
		{

			 if(top.frames["ek_main"] != null) // In workarea, Can just close the thickbox
			      self.parent.ektb_remove(); 		
			 else if(top.frames["mainFrame"] != null) //An Iframe
			      ReloadFramePage();
			 else
		          ReloadParentPage();  

			 var tDDContentTabs =  document.getElementById("DragDropUI") ;
		   	 if(tDDContentTabs != null)
		    	    {
		            	tDDContentTabs.style.position="relative";
		            	tDDContentTabs.style.left="-10000px";
                    }
		}
		
		function ReloadFramePage()
		{
                //Not in workarea and an Iframe
                var buffer = '';
	            try {
	                buffer = new String( top.frames["mainFrame"].location.href );
	            }
	            catch( ex ) {
	            }
                if (buffer.indexOf("#") != -1)
	            {
                    //Found a # sign
		            var sUrl = top.frames["mainFrame"].location.pathname;
		            var taxonomyId = "";
			        taxonomyId = '<asp:literal id="jsTaxonomyIdReloadFrame" runat="server"/>';
			       
		            if(taxonomyId != "")
		            {
		                var tempBuffer = new String( top.frames["mainFrame"].location.pathname );
		                if (tempBuffer.indexOf("__taxonomyid=") > -1)
		                {
		                    var startindex = tempBuffer.indexOf("__taxonomyid=");
		                    var endindex = tempBuffer.indexOf("&", startindex);

		                    if (endindex == -1)
		                    {
		                        endindex = tempBuffer.length;
		                        startindex--;
		                    }
		                    else
		                        endindex++;

		                    var replaceTerm = tempBuffer.substring(startindex, endindex);
		                    tempBuffer = tempBuffer.replace(replaceTerm, "");
		                }

		                if (tempBuffer.indexOf("?") > -1)
			                sUrl = tempBuffer + "&__taxonomyid=" +taxonomyId;
			            else
			                sUrl = tempBuffer + "?__taxonomyid=" + taxonomyId;
			        }

    	            var num = GetImgReloadNumber(sUrl);
    	            sUrl = CleanExisitingImgReloadNum(sUrl);
                    top.frames["mainFrame"].location.href = sUrl + AddReloadImageFlag(sUrl, num);                
	            }
	            else
	            {
	                var num = GetImgReloadNumber(top.frames["mainFrame"].location.href);
	    	        var newUrl = CleanExisitingImgReloadNum(top.frames["mainFrame"].location.href);
                    top.frames["mainFrame"].location.href = newUrl + AddReloadImageFlag(newUrl, num);
                }
		}

        function ReloadParentPage()
		{
		        var buffer = '';
     	        var objDoc = null;
	            try {
	                        if (top != null && top.opener != null)
	                        {
	                            buffer = new String( top.opener.location );
	                            objDoc = top.opener;
	                        }
	                        else if(self != null && self.parent != null)
	                        {
	                            buffer = new String( self.parent.location );
	                            objDoc = self.parent;
	                        }
                	        
	                }
	            catch( ex ) {
		         
	            }
	            
	            var taxonomyId = '<asp:literal id="jsTaxonomyId" runat="server"/>';
	            var sUrl ;
	            
	            if (buffer.indexOf("#") != -1)                         
	                 {
	                            sUrl = objDoc.location.pathname;
	                            if(objDoc.location.search != "")
	                              sUrl = sUrl + objDoc.location.search;
	                              
		                        if(taxonomyId != "")
		                             {
		                                    var tempBuffer = new String( objDoc.parent.location.pathname );
		                                    if(objDoc.parent.location.search != "")
		                                    tempBuffer = tempBuffer + objDoc.parent.location.search ; 
		                                    if (tempBuffer.indexOf("__taxonomyid=") > -1)
		                                           {

	                                                       var startindex = tempBuffer.indexOf("__taxonomyid=");
		                                                    var endindex = tempBuffer.indexOf("&", startindex);

		                                                    if (endindex == -1)
		                                                    {
		                                                        endindex = tempBuffer.length;
		                                                        startindex--;
		                                                    }
		                                                    else
		                                                        endindex++;

		                                                    var replaceTerm = tempBuffer.substring(startindex, endindex);
		                                                    tempBuffer = tempBuffer.replace(replaceTerm, "");
		                                           }

		                                    if (tempBuffer.indexOf("?") > -1)
			                                    sUrl = tempBuffer + "&__taxonomyid=" +taxonomyId;
			                                else
			                                    sUrl = tempBuffer + "?__taxonomyid=" + taxonomyId;
			                        }
			                    else
			                        {
			                                  if(sUrl.indexOf("__taxonomyid=") > -1)
			                                  {
			                                        //No TaxID Remove Existing;
			                                        var startindex = sUrl.indexOf("__taxonomyid=");
		                                            var endindex = sUrl.indexOf("&", startindex);

		                                            if (endindex == -1)
		                                            {
		                                                endindex = sUrl.length;
		                                                startindex--;
		                                            }
		                                            else
		                                                endindex++;

		                                            var replaceTerm = sUrl.substring(startindex, endindex);
		                                            sUrl = sUrl.replace(replaceTerm, "");
			                                  }
			                        }

	    	                        if (self.parent != null)
	    	                         {
	                                    
	                                    var num = GetImgReloadNumber(sUrl);
	    	                            sUrl = CleanExisitingImgReloadNum(sUrl);
                                        objDoc.location.href = sUrl + AddReloadImageFlag(sUrl, num);
                                     }
                                      else
                                     {
	                                  
	                                   var num = GetImgReloadNumber(sUrl);
	    	                           sUrl = CleanExisitingImgReloadNum(sUrl);
                                       objDoc.location.href = sUrl + AddReloadImageFlag(sUrl, num);
                                    }
	                 } //end   if (buffer.indexOf("#") != -1)
	            else
	            {
	                                //No #
	                                var sUrl1="";
	                                if (taxonomyId != "")
                                    {
		                                sUrl1 = sUrl1 + "?__taxonomyid=" + taxonomyId;
		                            }

	    	                        if (parent != null)
	    	                         {
	    	                              if (sUrl1 != "")
	    	                                {
		                                            //sUrl1 not 0
	    		                                    sUrl1 = parent.location.href;
	    		                                    var tempBuffer = new String( parent.location.href );
                                                    if (tempBuffer.indexOf("__taxonomyid=") > -1)
                                                        {
		                                                        //__taxonomyid exists
                                                                var startindex = tempBuffer.indexOf("__taxonomyid=");
                                                                var endindex = tempBuffer.indexOf("&", startindex);

                                                                if (endindex == -1)
                                                                   {
                                                                       endindex = tempBuffer.length;
                                                                        startindex--;
		                                                           }
                                                                else
		                                                            endindex++;

                                                                var replaceTerm = tempBuffer.substring(startindex, endindex);
                                                                tempBuffer = tempBuffer.replace(replaceTerm, "");
                                                        }

                                                   if (tempBuffer.indexOf("?") > -1)
                                                        {
                                                                var poundsign = tempBuffer.indexOf("#");
                                                                if (poundsign > -1)
                                                                {
			                                                        if(tempBuffer.substring(0,poundsign).lastIndexOf("&") == tempBuffer.substring(0,poundsign).length)
			                                                        {
                           		                                        sUrl1 = tempBuffer.substring(0,poundsign) + "__taxonomyid=" +taxonomyId;
			                                                        }
			                                                        else
			                                                        {
                            	                                        sUrl1 = tempBuffer.substring(0,poundsign) + "&__taxonomyid=" +taxonomyId;
			                                                        }
			                                                        var num = GetImgReloadNumber(sUrl1);
	    	                                                        sUrl1 = CleanExisitingImgReloadNum(sUrl1);
                                                                    sUrl1 = sUrl1 + AddReloadImageFlag(sUrl1, num) + tempBuffer.substring(poundsign);
                                                                }
                                                                else
                                                                {
			                                                        if(tempBuffer.lastIndexOf("&") == tempBuffer.length)
			                                                        {
                                                                        sUrl1 = tempBuffer + "__taxonomyid=" +taxonomyId;
			                                                        }
			                                                        else
			                                                        {
                            	                                        sUrl1 = tempBuffer + "&__taxonomyid=" +taxonomyId;
			                                                        }
			                                                        var num = GetImgReloadNumber(sUrl1);
	    	                                                        sUrl1 = CleanExisitingImgReloadNum(sUrl1);
	                                                                sUrl1 = sUrl1 + AddReloadImageFlag(sUrl1, num);
	                                                            }
	                                                  } //End if (tempBuffer.indexOf("?") > -1)
	                                             else
	                                                  {
	                                                                sUrl1 = tempBuffer + "?__taxonomyid=" + taxonomyId;
	                                                                var num = GetImgReloadNumber(sUrl1);
	    	                                                        sUrl1 = CleanExisitingImgReloadNum(sUrl1);
	                                                                sUrl1 = sUrl1 + AddReloadImageFlag(sUrl1, num);
                                                             
	                                                  }
                                                var querystring = sUrl1.substring(sUrl1.indexOf("?"), sUrl1.length);
                                                parent.location.href = parent.location.pathname + querystring;
		                                 } //End  if (sUrl1 != "")
		                          else
		                                {
	                                        
			                                var num = GetImgReloadNumber(parent.location.pathname + parent.location.search);
	    	                                var newUrl = CleanExisitingImgReloadNum(parent.location.pathname + parent.location.search);
			                                parent.location.href = newUrl + AddReloadImageFlag(parent.location.pathname + parent.location.search, num);
			                            }
		                     }
		              else
		                    {
                                var tempBuffer = new String( top.location.href );
			                    var poundsign = tempBuffer.indexOf("#");
			                    var newUrl = tempBuffer;
                                if (poundsign > -1)
                                {
                                    newUrl = tempBuffer.substring(0,poundsign);
                                    var num = GetImgReloadNumber(newUrl);
	    	                        newUrl = CleanExisitingImgReloadNum(newUrl);
                                    newUrl = newUrl + AddReloadImageFlag(newUrl, num);
                                    newUrl=newUrl.replace(/__taxonomyid=[0-9]+/ig,'__taxonomyid='+taxonomyId);
                                   
                                }
			                    else
			                    {
                                    var num = GetImgReloadNumber(newUrl);
	    	                        newUrl = CleanExisitingImgReloadNum(newUrl);
			                        newUrl = newUrl + AddReloadImageFlag(newUrl, num);
			                    }
			                   
                                top.location.href = newUrl;
                            }
		        }
		}

		function AddReloadImageFlag(path, num)
		{
                if(num != 1)
                    num = 1;
                else
                    num = 2;

                var result = "";
                var delim = (path.indexOf("?") >= 0) ? "&" : "?";
                var parmName = "ekimgreload";
                if (path.indexOf(parmName) < 0){
                    result += delim + parmName + "=" + num;
                }
                return (result);
         }
		
		function GetImgReloadNumber(url)
		{
            if(url.indexOf("ekimgreload=2") > 0)
    	        return 2;
            else
                return 1;
		}
		function CleanExisitingImgReloadNum(url)
        {
            var newUrl = new String( url );
            // if it is just one parameter in a string of them, remove it
            newUrl = newUrl.replace("&ekimgreload=1", "");
            newUrl = newUrl.replace("&ekimgreload=2", "");

            // if it is the first parameter in a string with others, make sure to switch the next param's '&' with a '?'
            newUrl = newUrl.replace("?ekimgreload=1&", "?");
            newUrl = newUrl.replace("?ekimgreload=2&", "?");

            // if it is the only param, just remove it
            newUrl = newUrl.replace("?ekimgreload=1", "");
            newUrl = newUrl.replace("?ekimgreload=2", "");
            return newUrl;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel Visible="false" ID="panelImageProperties" runat="server">
            <div id="ImageProperties">
                <div style="width:100%; text-align:right;">
                    <asp:Button ID="btnNext" CssClass="buttonNext" runat="server" />
                </div>
                
                <fieldset>
                    <legend>
                        <asp:Label ID="HeaderLabel" runat="server" Text=""></asp:Label></legend>
                        <div style="position:absolute; font-weight:bold;z-index:-100;top:200px;left:250px;"><asp:Literal ID="literal_wait" runat="server"></asp:Literal></div>
                    <table width="100%">
                        <thead>
                            <tr>
                                <th colspan="2">
                                    <asp:Label ID="HelpMessage" runat="server" Text="Help Message"></asp:Label></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td class="label" style="width:1%; white-space:nowrap; vertical-align:top;">
                                    <label id="ekImagegalleryTitleLbl" for="ekImagegalleryTitle" runat="server">
                                        <%=ImagegalleryTitleLbl%></label>
                                </td>
                                <td>
                                    <input id="ekImagegalleryTitle" type="text" name="ekImagegalleryTitle" runat="server" size="50" /></td>
                            </tr>
                            <tr>
                                <td class="label" style="width:1%; white-space:nowrap; vertical-align:top;">
                                    <label id="ekImagegalleryImageWidthLbl" for="ekImagegalleryImageWidth" runat="server">
                                        <%=ImagegalleryImageWidthLbl%></label></td>
                                <td>
                                    <select id="ekImagegalleryImageWidth" name="ekImagegalleryImageWidth" runat="server" >
                                        <option value="250">250 pixel</option>
                                        <option value="800" selected="selected">800 pixel (suitable for on-screen viewing)</option>
                                        <option value="1000">1000 pixel</option>
                                        <option value="1400">1400 pixel</option>
                                        <option value="1600">1600 pixel</option>
                                    </select>
                                </td>
                            </tr>
                            
                            <tr>
                               
                                <td class="label" style="width:1%; white-space:nowrap; vertical-align:top;">
                                    <label id="ekImagegalleryAddressLbl" for="ekImagegalleryAddress" visible="false" runat="server">
                                        <%=ImagegalleryAddressLbl%></label></td>
                                <td>
                                    <input type="text" visible="false" name="ekImagegalleryAddress" id="ekImagegalleryAddress" runat="server" size="37" /></td>
                                    
                            </tr>
                            
                            <tr>
                                <td class="label" style="width:1%; white-space:nowrap; vertical-align:top;">
                                    <label id="ekImagegalleryDescriptionLbl" for="ekImagegalleryDescription" runat="server">
                                        <%=ImagegalleryDescriptionLbl%></label></td>
                                <td>
                                    <asp:PlaceHolder ID="ekImagegalleryDescriptionEditorHolder" runat="server"></asp:PlaceHolder>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </fieldset>
            </div>
         </asp:Panel>
         
        <asp:Panel Visible="false" ID="panelDragDrop" runat="server">
             <div class="ektronModalBody" runat="server" id="DragDropUI">
                     <div id="DMStabs">
	                        <ul>
		                        <li><a class="tabFileUpload" href="#tabFileUpload">File Upload</a></li>
	                            <li id="liMultipleDMS" runat="server"><a href="#tabMultipleDMS">Multiple DMS Documents</a></li>
		                        <li id="liDragDrop" visible="false"  runat="server"><a href="#tabDragDrop">Drag Drop</a></li>
	                        </ul>
	                                             
	                        <div id="tabFileUpload">
		                          <div id="ek_DMSUploadUI" runat="server" >
                                        <asp:Literal ID="ltrStatus" runat="server" /><br />
                                        <asp:ScriptManager runat="server"  ID="ScriptManager1" EnablePageMethods="true" />
                                        <asp:FileUpload size="50"  ID="ekFileUpload" Width="80%" runat="server" /><br /><br />
                                        <asp:Button Text="Upload" CssClass="greenHoverDD buttonDD  buttonLeftDD" ID="uploadFile" runat="server" OnClientClick="return CheckFileExists();" OnClick="uploadFile_Click" /> 
                                        <asp:Button Text="Cancel" CssClass="redHoverDD buttonDD  buttonLeftDD" ID="btnCancel" runat="server"  OnClientClick="cancelDialog();" />                                                  
                                  </div>
	                        </div>
	                        <div runat="server"  id="tabMultipleDMS">
	                            <div>
	                                <input type="button" class="greenHoverDD buttonDD  buttonLeftDD" value="Upload" onclick="MultipleDocumentUpload();" />
	                                <a class="toggleSupportedFileTypes" style="float:right; text-decoration:underline;" href="#" onclick="toggleSupportedFileTypes();">Show File Types</a>
	                                <br />
	                            </div>		                       
	                        </div>
	                        <div runat="server" visible="false" id="tabDragDrop">
		                      
	                        </div>
	                        
                    </div>                
            </div> 
            <div runat="server" class="WaitLoadDD" id="ek_DmsFileUploadWait" style="position:relative;left:-10000px" >
                  <asp:Literal ID="ltrPleaseWait" runat="server" ></asp:Literal> <br />
                  <img src="../images/application/loading_bar.gif" alt="uploading image" />
            </div>
        </asp:Panel>
        
        
        <div id="ekOnemoment" class="ekOnemomentHidden">
        <center><h4>One Moment Please...</h4></center>
        </div> 
        <input id="isFileUploadComplete" runat="server" type="hidden" value="" />
        <input type="hidden" name="Cmd" value="Save" />
        <input type="hidden" name="NextUsing" id="NextUsing" runat ="server" />
        <input type="hidden" value="New" />
        <input type="hidden" name="putopts" value="true" />
        <input type="hidden" name="destination" id="destination"  runat ="server" />
        <input type="hidden" name="Confirmation-URL" class="confirmationURL" />
        <input type="hidden" name="PostURL"  id="PostURL"  runat ="server" />
        <input type="hidden" name="VTI-GROUP" value="0" />
        <input type="hidden" name="type" value="multiple,add" id="type" runat="server" />
        <input type="hidden" name="content_id" id="content_id" runat="server" />
        <input type="hidden" name="content_folder" id="content_folder" runat="server" />
        <input type="hidden" name="content_language" id="content_language" runat="server" />
        <input type="hidden" name="content_teaser" id="content_teaser" runat="server" />
        <input type="hidden" name="requireMetaTaxonomy" id="requireMetaTaxonomy" runat="server" />
        <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value="" runat="server" />
        <input type="hidden" name="IsSearchable" id="IsSearchable" value="IsSearchable" runat="server" />
    </form>
</body>
</html>
