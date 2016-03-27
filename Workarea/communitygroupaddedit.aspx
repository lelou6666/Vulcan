<%@ Page Language="VB" AutoEventWireup="false" CodeFile="communitygroupaddedit.aspx.vb" Inherits="communitygroupaddedit" ValidateRequest="false" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register TagPrefix="Community" TagName="UserSelectControl" Src="controls/Community/Components/UserSelectControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" runat="server">
<head runat="server">
    <title>Add Edit Community Group</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <asp:literal id="ltr_css" runat="server" />
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        function GetCommunityMsgObject(id){
	        var fullId = "CommunityMsgObj_" + id;
	        if (("undefined" != typeof window[fullId])
		        && (null != window[fullId])){
		        return (window[fullId]);
	        }
	        else {
	            var obj = new CommunityMsgClass(id);
	            var fullId = "CommunityMsgObj_" + id;
	            window[fullId] = obj;
		        return (obj);
	        }
        }

        function CommunityMsgClass (idTxt) {
	        this.getId = function(){
		        return (this.id);
	        },

	        this.SetUserSelectId = function (userSelId){
	            this.userSelId = userSelId;
	        },

	        this.MsgShowMessageTargetUI = function(hdnId){
	            $ektron('#MessageTargetUI' + this.getId()).modalShow();
	        },

	        this.MsgSaveMessageTargetUI = function(){
		        var hdbObj = document.getElementById('ektouserid' + this.getId());
		        var toObj = document.getElementById('ekpmsgto' + this.getId());
		        var idx;
		        if (hdbObj){
			        hdbObj.value = '';
			        var users = UserSelectCtl_GetSelectUsers(this.userSelId);
			        if (("undefined" != users) && (null != users)){
			            hdbObj.value = users;
			            if (toObj){
			                toObj.value='';
			                var userArray = users.split(',');
			                for (idx = 0; idx < userArray.length; idx++){
				                if(toObj && toObj.value.length > 0){
					                toObj.value += ', ';
				                }
				                var userName = UserSelectCtl_GetUserName(this.userSelId, userArray[idx]);
				                if (userName)
    				                toObj.value += userName;
			                }
		                }
		            }
		        }
		        this.MsgCloseMessageTargetUI();
	        },


	        this.MsgCloseMessageTargetUI = function(){
	            $ektron('#MessageTargetUI' + this.getId()).modalHide();
	        },

	        this.MsgCancelMessageTargetUI = function(){
		        this.MsgCloseMessageTargetUI();
	        },

	        ///////////////////////////
	        // initialize properties:
	        this.id = idTxt;
	        this.name = '';
	        this.MsgUsersSelArray = new Object();
	        this.userSelId = '';
        }
	    //--><!]]>
    </script>
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        var inPublishProcess = false;

        function ShowAddGroupTagArea(){
            $ektron("#newTagName").val("");
            $ektron("#newTagNameDiv").modalShow();
	    }

	    var customPTagCnt = 0;
	    function SaveNewGroupTag(){
		    var objTagName = document.getElementById("newTagName");
		    var objTagLanguage = document.getElementById("TagLanguage");
		    var objLanguageFlag = document.getElementById("flag_" + objTagLanguage.value);
		    var divObj = document.getElementById("newTagNameScrollingDiv");

    		if(!CheckForillegalChar(objTagName.value)){
		        return;
		    }

		    if (objTagName && (objTagName.value.length > 0) && divObj){
			    ++customPTagCnt;
			    divObj.innerHTML += "<input type='checkbox' checked='checked' onclick='ToggleCustomPTagsCbx(this, \"" + objTagName.value + "\");' id='userCustomPTagsCbx_" + customPTagCnt + "' name='userCustomPTagsCbx_" + customPTagCnt + "' />&#160;"

			    if(objLanguageFlag != null){
			        divObj.innerHTML += "<img src='" + objLanguageFlag.value + "' border=\"0\" />"
			    }

			    divObj.innerHTML +="&#160;" + objTagName.value + "<br />"

			    AddHdnTagNames(objTagName.value + '~' + objTagLanguage.value);
			    
                $ektron('#newTagNameScrollingDiv input[data-ektron-checkbox-flag="false"]').removeAttr('checked');
                $ektron('#newTagNameScrollingDiv input[data-ektron-checkbox-flag="true"]').attr('checked', 'checked');
		    }

		    // now close window:
		    CancelSaveNewGroupTag();
	    }

	    function CheckForillegalChar(tag) {
            if (Trim(tag) == '')
            {
               alert('<asp:Literal ID="error_TagsCantBeBlank" Text="Please enter a name for the Tag." runat="server"/>');
               return false;
            } else {

                //alphanumeric plus _ -
                var tagRegEx = /[!"#$%&'()*+,./:;<=>?@[\\\]^`{|}~ ]+/;
                if(tagRegEx.test(tag)==true) {
                    alert('<asp:Literal ID="error_InvalidChars" Text="Tag Text can only include alphanumeric characters." runat="server"/>');
                    return false;
                }

            }
            return true;
        }

	    function CancelSaveNewGroupTag(){
            $ektron("#newTagNameDiv").modalHide();
	    }

        function CancelBroswseCommunityUserModal(){
            $ektron("#MessageTargetUI__Page").modalHide();
        }
        
	    function AddHdnTagNames(newTagName){
		    objHdn = document.getElementById("newTagNameHdn");
		    if (objHdn){
			    var vals = objHdn.value.split(";");
			    var matchFound = false;
			    for (var idx = 0; idx < vals.length; idx++){
				    if (vals[idx] == newTagName){
					    matchFound = true;
					    break;
				    }
			    }
			    if (!matchFound){
				    if (objHdn.value.length > 0){
					    objHdn.value += ";";
				    }
				    objHdn.value += newTagName;
			    }
		    }
	    }

	    function RemoveHdnTagNames(oldTagName){
		    objHdn = document.getElementById("newTagNameHdn");
		    if (objHdn && (objHdn.value.length > 0)){
			    var vals = objHdn.value.split(";");
			    objHdn.value = "";
			    for (var idx = 0; idx < vals.length; idx++){
				    if (vals[idx] != oldTagName){
					    if (objHdn.value.length > 0){
						    objHdn.value += ";";
					    }
					    objHdn.value += vals[idx];
				    }
			    }
		    }
	    }

	    function ToggleCustomPTagsCbx(btnObj, tagName){
		    if (btnObj.checked){
			    AddHdnTagNames(tagName);
			    btnObj.checked = true;
			    $ektron(btnObj).attr("data-ektron-checkbox-flag","true");
		    }
		    else{
			    RemoveHdnTagNames(tagName);
			    btnObj.checked = false; // otherwise re-checks when adding new custom tag.
			    $ektron(btnObj).attr("data-ektron-checkbox-flag","false");
		    }
	    }

	    function toggleVisibility(itm)
        {
            switch(itm)
            {
                case "upload":
                    $ektron("#avatar_upload_panel").modalShow();
                    break;
                case "close":
                    $ektron("#avatar_upload_panel").modalHide();
                    break;
          }
        }

        function updateavatar()
        {
            var tfile = document.getElementById('GroupAvatar_TB');
            var ofile = document.getElementById('fileupload1');
            if (tfile.value.indexOf('[file]') > -1)
            {
                ofile.outerHTML  = document.getElementById('fileupload1').outerHTML;
                tfile.value = tfile.value.replace('[file]', '');
            }
        }
        //--><!]]>
    </script>
    <script type="text/javascript">
        Ektron.ready(function()
        {
            var tabsContainers = $ektron(".tabContainer");
            tabsContainers.tabs();

            //Tag, Upload, Browse Modal
            $ektron("#newTagNameDiv, #avatar_upload_panel, #MessageTargetUI__Page").modal({
                trigger: '',
                modal: true,
                toTop: true,
                onShow: function(hash)
                {
                    hash.w.css("margin-top", 0); //hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
                    hash.o.fadeIn();
                    hash.w.fadeIn();
                    setTimeout(CommunityGroupAddEdit__SetScrollable, 500);
                    $(window).resize(CommunityGroupAddEdit__SetScrollable);
                },
                onHide: function(hash)
                {
                    hash.w.fadeOut("fast");
                    hash.o.fadeOut("fast", function()
                    {
                        if (hash.o)
                        {
                            hash.o.remove();
                        }
                    });
                }
            });

            // reveal the content of the page
            $ektron("#form1").css({ 
                "position": "static",
                "left": "auto"
            });
        });

    function CommunityGroupAddEdit__SetScrollable(){
        var container = $ektron(".EktMsgTargets").eq(0);

        container.height($ektron(window).height() -
            (container.offset().top
                + (container.innerHeight()
                    - container.height())));

        $ektron(".analyticsReport .ektronToolbar").eq(0).width(container.outerWidth());
        container.eq(0).css("overflow-y", "auto");
        container.eq(0).css("overflow-x", "auto");
        container.find("* *").eq(0).css("overflow", "visible");
    }
    </script>
    <style type="text/css">
			#T0{ float:none; position:relative; }
			ul.ektree{ float:none; position:relative; background-color: #ffffff; border: 1px solid #000000; margin: 10px 10px 10px 10px; padding: 10px 10px 10px 10px; }
            #TreeOutput{ position:relative; width:100%; }
			#d_dvCategory table{ width:100%; }
			div#newTagNameDiv { height: 175px !important; width:375px !important; margin: 17em 0 0 -15em !important; border: solid 1px #aaaaaa; z-index: 10; background-color: white; left: 50%; position: fixed; margin-left: -20em;}
			div#avatar_upload_panel { height: 125px; width:400px;top:7%;left:35%; margin: 10em 0 0 -15em; border: solid 1px #aaaaaa; z-index: 10; background-color: white; }
			div#MessageTargetUI__Page { margin: 10em 0 0 1.0em;top:3px;left:2px; border: solid 1px #aaaaaa; z-index: 10; background-color: white; }
		    /* Styling for the Browse Members/Friends elements */
		    .EktMsgTargetCtl{ font-family: Verdana,Geneva,Arial,Helvetica,sans-serif; font-size: 12px; position: relative; top: 2px; left: 0px; background-color: white; z-index:12; }
		    .EktMsgTargets{ position: relative; top: 20px; left: 16px; border: solid 1px #dddddd; padding: 10px; }
		    .EktMsgTargets div.CommunitySearch_ResultsContainer { width: 440px; }
		    #newTagName { width: 275px !important; }
		    #newTagNameScrollingDiv { height: 80px; overflow: auto; border: z-index: 1; }
            a.btnUpload { padding-top: .2em !important; padding-bottom: .2em !important;line-height: 16pt !important; display:inline-block; text-decoration: none !important; }
            .nameWidth { color:#1D5987; font-weight: bold; text-align: right; white-space: nowrap; width:10%; }
            a.buttonBrowseUSer {background-image: url(images/ui/icons/User.png); background-position: .6em center;}
            #FrameContainer { position: relative; top: 0px; left: 0px; width: 1px; height: 1px; display: none; z-index: 1000; }
            body div.ektronWindow {position: relative !important; top: 0; left: 0; margin-left: 0; margin-top: 0;}
            div#MessageTargetUI__Page.dv_MessageTargetUI {width: auto; margin-top: -42px !important; margin-left: -21px !important;}
            .dv_MessageTargetUI .ektronModalBody {padding: 2px;}
            .CommunitySearch_BasicTextboxContainer input {width: 200px;}
            
            form#form1 {position: absolute; left: -10000px}
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" name="content_id" value="0" id="content_id" runat="server" />
        <input type="hidden" runat="server" id="submitasstagingview" name="submitasstagingview" value="" />
        <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
        <input type="hidden" name="LastClickedOnChecked" id="LastClickedOnChecked" value="false" />
        <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value=""
            runat="server" />
        <asp:Literal ID="ltr_js" runat="Server" />
        <div id="MessageTargetUI__Page" class="dv_MessageTargetUI ektronWindow ektronModalStandard">
            <div class="ektronModalBody">
            <div class="EkTB_dialog">
                <div class="EktMsgTargetCtl">
                    <div>
                        <div id="browseCommunityUsers" class="EktMsgTargets">
                            <!-- <asp_Literal ID="ltr_recipientselect" run_at="Server" /> -->
                            <Community:UserSelectControl id="Invite_UsrSel" FriendsOnly="false" runat="server" />
                            <asp:button id="cgae_userselect_done_btn" runat="server" />
                            <asp:button id="cgae_userselect_cancel_btn" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        </div>
        <div class="ektronPageContainer ektronPageTabbed">
            <div class="tabContainerWrapper">
                <div class="tabContainer">
                    <ul>
                        <li>
                            <a href="#dvProperties">
                                Properties
                            </a>
                        </li>
                        <li>
                            <a href="#dvTags">
                                Tags
                            </a>
                        </li>
                        <asp:PlaceHolder ID="phCategoryTab" runat="server">
                            <li>
                                <a href="#dvCategory">
                                    Category
                                </a>
                            </li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phAliasTab" runat="server">
                        <li>
                         <a href="#dvAlias">
                            Links
                         </a>
                        </li>
                        </asp:PlaceHolder>
                   </ul>

                <div id="dvProperties">
                    <span id="errmsg" runat="server" />
                    <table class="ektronForm">
                        <tr>
                            <td class="label"><asp:Literal ID="ltr_groupname" runat="server" />:</td>
                            <td class="value"><asp:TextBox ID="GroupName_TB" CssClass="ektronTextMedium" runat="server" /></td>
                        </tr>
                        <tr runat="server" id="tr_ID">
                            <td class="label"><asp:Literal ID="ltr_groupid" runat="server" />:</td>
                            <td class="value"><asp:Label ID="lbl_id" runat="server" /></td>
                        </tr>
                        <tr id="tr_admin" runat="server">
                            <td class="label"><asp:Literal ID="ltr_admin" runat="server" />:</td>
                            <td class="value">
                                <input type="text" id="ekpmsgto__Page" name="ekpmsgto__Page" disabled="disabled" runat="server" class="ektronTextMedium" />
                                <input type="hidden" id="ektouserid__Page" name="ektouserid__Page" value="" runat="server" />
                                <asp:Literal ID="BrowseUsers" runat="server"  />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Literal ID="ltr_groupjoin" runat="server" />:</td>
                            <td class="value">
                                <asp:RadioButton ID="PublicJoinYes_RB" runat="server" GroupName="PublicJoin" Text="Yes" />&nbsp;&nbsp;
                                <asp:RadioButton ID="PublicJoinNo_RB" runat="server" GroupName="PublicJoin" Text="No" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="white-space: nowrap;">
                                <asp:Literal ID="ltr_groupfeatures" runat="server"></asp:Literal>
                            </td>
                            <td>
                                <asp:CheckBox ID="FeaturesCalendar_CB" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:CheckBox ID="FeaturesForum_CB" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Literal ID="ltr_groupavatar" runat="server" />:</td>
                            <td class="value">
                                <asp:Literal ID="ltr_avatarpath" runat="server" /><asp:TextBox ID="GroupAvatar_TB" CssClass="ektronTextMedium" runat="server" />
                                <a class="button buttonInlineBlock greenHover buttonUpload btnUpload" href="javascript:toggleVisibility('upload');" name="upload">
                                    <asp:Literal ID="ltr_upload" runat="server" />
                                </a>
                                <div id="avatar_upload_panel" class="ektronWindow ektronModalStandard">
                                    <div class="ektronModalHeader">
                                        <h3>
                                            <span class="headerText"><asp:Literal ID="lblUpload" runat="server" /></span>
                                            <asp:HyperLink ID="closeDialogLink3" CssClass="ektronModalClose" runat="server" />
                                        </h3>
                                    </div>
                                    <div class="ektronModalBody">
                                        <asp:Label ID="lbStatus" runat="server" />
                                        <div style="float:right !important">
                                            <input type="file" id="fileupload1" runat="server" />
                                        </div>
                                        <br />
                                        <div class="ektronSpace">
                                            <div class="ektronSpace"/>
                                            <ul class="buttonWrapper ui-helper-clearfix">
                                                <li><a href="#" class="button redHover buttonClear" onclick="toggleVisibility('close'); return false;">
                                                    <asp:Literal ID="ltr_close" runat="server" />
                                                </a></li>
                                                <li><a href="#" class="button greenHover buttonOk" onclick="CheckUpload(document.getElementById('fileupload1').value); return false;">
                                                    <asp:Literal ID="ltr_ok" runat="server" />
                                                </a></li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Literal ID="ltr_grouplocation" runat="server" />:</td>
                            <td class="value"><asp:TextBox ID="Location_TB" CssClass="ektronTextMedium" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label"><asp:Literal ID="ltr_groupsdesc" runat="server" />:</td>
                            <td class="value"><asp:TextBox ID="ShortDescription_TB"  CssClass="ektronTextMedium" runat="server" MaxLength="100" /></td>
                        </tr>
                        <tr>
                            <td valign="top"class="label"><asp:Literal ID="ltr_groupdesc" runat="server" />:</td>
                            <td class="value"><asp:TextBox ID="Description_TB" runat="server" Rows="6" TextMode="MultiLine" MaxLength="500" /></td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <div runat="server" id="tr_EnableDistribute">
                                    <span class="label"><asp:CheckBox ID="EnableDistributeToSite_CB" runat="server" /><asp:Literal ID="ltr_enabledistribute" runat="server" /></span>
                                </div>

                                <div runat="server" id="tr_AllowMembersToManageFolders">
                                    <span class="label" colspan="2"><asp:CheckBox ID="AllowMembersToManageFolders_CB" runat="server" /><asp:Literal ID="ltr_AllowMembersToManageFolders" runat="server" /></span>
                                </div>

                                <div runat="server" id="tr_MessageBoardModeration">
                                    <span class="label" colspan="2"><asp:CheckBox ID="chkMsgBoardModeration" runat="server" /><asp:Literal ID="ltr_MsgBoardModeration" runat="server" /></span>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="dvTags">
                    <div id="TD_GroupTags" runat="server"></div>
                </div>
                <asp:PlaceHolder ID="phCategoryFrame" runat="server">
                    <div id="dvCategory">
                        <asp:Literal runat="server" ID="EditTaxonomyHtml" />
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phAliasFrame" runat="server">
                        <div id="dvAlias">
                            <p style="width: auto; height: auto; overflow: auto;" class="groupAliasList" ><%=groupAliasList%></p>
                        </div>
                    </asp:PlaceHolder> 
                    <div id="dvWaitImage">
                    </div>
                </div>
            </div>
        </div>
        <asp:Literal ID="UpdateFieldJS" runat="server" />
        <%--<div id="FrameContainer_">
            <iframe id="ChildPage" src="javascript:false;" frameborder="1" marginheight="0" marginwidth="0" width="100%"
                height="100%" scrolling="auto" style="background-color: white;">
            </iframe>
        </div>--%>

    </form>

    <script type="text/javascript">
        <asp:literal id="js_taxon" runat="server" />
    // var taxonomytreearr="".split(",");
    // var taxonomytreedisablearr="".split(",");
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
                        if (navigator.userAgent.indexOf("Firefox") > -1){
                            n.checked = eval(newvalue);
                            n.disabled = eval(newvalue);
                        }
                        else{
                            n.setAttribute("checked",eval(newvalue));
                            n.setAttribute("disabled",eval(newvalue));
                        }

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

    function SaveCategory()
    {
        var selected_nodes = document.getElementById('taxonomyselectedtree');
        var target = parent.document.getElementById('ekcategoryselection');
        if( target != null ) {
            target.value = selected_nodes.value;
        }
        top.CloseCategorySelect(false);
    }

    function Wait(bool)
    {
        if (bool)
        {
            ShowPane('dvWaitImage');
            document.getElementById("dvWaitImage").innerHTML = '<img src="images/application/loading_big.gif" alt="Please wait..." />';
            document.getElementById("dialog_publish").style.visibility = 'hidden';
            document.getElementById("dialog_cancel").style.visibility = 'hidden';

        } else {
            document.getElementById("dvWaitImage").innerHTML = '';
            document.getElementById("dialog_publish").style.visibility = 'visible';
            document.getElementById("dialog_cancel").style.visibility = 'visible';
            ShowPane('dvContent');
        }
        inPublishProcess = bool;
    }

    </script>
    <!--#include file="common/taxonomy_editor_menu.inc" -->
    <!--#include file="common/treejs.inc" -->
<asp:Literal ID="litInitialize" runat="server"  />
</body>
</html>
