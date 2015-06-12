<%@ Page Language="vb" validateRequest="false" AutoEventWireup="false" %>
<%@ Import Namespace="Ektron.Cms.UI.CommonUI" %>
<%@ Import Namespace="Ektron.Cms.Site" %>
<%@ Import Namespace="Ektron.Cms.User" %>
<%@ Import Namespace="Ektron.Cms.Content" %>
<%@ Import Namespace="Ektron.Cms" %>
<%@ Import Namespace="Ektron.Cms.Common" %>
<%@ Import Namespace="Ektron.Cms.Common.EkConstants" %>
<!-- #include file="BackPage.aspx" -->
<%
Dim AppUI as New ApplicationAPI
    Dim action, brObj, Ret, actErrorString, saveObj, currentUserID, sitePath, pid as Object  ' , UserCol, uID
    Dim orderList, folderId, idArray, folderArray, lLoop, AppImgPath, nId As Object  ' linkObj1, linkObj2, contId, parentID,
    Dim ItemType, ItemObj, ContentLanguage, bPage, NoWorkAreaAttribute As Object
    Ret = Nothing
    actErrorString = ""
    ContentLanguage = Nothing
Dim m_refApi As New CommonApi
    Dim bakAction As String = ""
 
bPage=Request.QueryString("bPage")
CurrentUserId=AppUI.UserId
AppImgPath=AppUI.AppImgPath
sitePath=AppUI.SitePath
action = Request.QueryString("action")
folderId = Request.QueryString("folderid")
NoWorkAreaAttribute = ""
if (Request.QueryString("noworkarea") = "1") then
	NoWorkAreaAttribute = "&noworkarea=1"
end if
If (Request.QueryString("LangType")<>"") Then
		ContentLanguage=Request.QueryString("LangType")
		AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage)
	else
		if CStr(AppUI.GetCookieValue("LastValidLanguageID")) <> ""  then
			ContentLanguage = AppUI.GetCookieValue("LastValidLanguageID")
		end if
End If
AppUI.ContentLanguage=ContentLanguage
if (action = "doAdd") then 
        Try            
            folderId = Request.Form("frm_folder_id")
            saveObj = New Collection
            
            saveObj.Add(Request.Form("frm_nav_title"), "CollectionTitle")
            saveObj.Add(Request.Form("frm_nav_template"), "CollectionTemplate")
            saveObj.Add(Request.Form("frm_nav_description"), "CollectionDescription")
            
            saveObj.Add(folderId, "FolderID")
            If (Request.Form("frm_recursive") <> "") Then
                saveObj.Add(1, "Recursive")
            Else
                saveObj.Add(0, "Recursive")
            End If
            If (Request.Form("frm_approval_methhod") <> "") Then
                saveObj.Add(True, "ApprovalRequired")
            Else
                saveObj.Add(False, "ApprovalRequired")
            End If
            If (Request.Form("EnableReplication") <> "") Then
                saveObj.Add(1, "EnableReplication")
            Else
                saveObj.Add(0, "EnableReplication")
            End If
            brObj = AppUI.EkContentRef
            Ret = brObj.AddEcmCollectionItem(saveObj)
            If (saveObj("ApprovalRequired")) Then
                Response.Redirect("collections.aspx?action=ViewStage&nId=" & saveObj("CollectionID") & "&folderId=" & folderId & "&rf=1", False)
            Else
                Response.Redirect("collections.aspx?action=View&nId=" & saveObj("CollectionID") & "&folderId=" & folderId & "&rf=1", False)
                'Response.Redirect("collections.aspx?folderid=" & folderId)
            End If
            
            saveObj = Nothing
            brObj = Nothing
        Catch Ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(Ex.Message))
        End Try
        
	 
elseif (action = "doDeleteMenu") then
	try
    folderId = Request.QueryString("folderid")
    nID = Request.QueryString("nId")
    brObj = AppUI.EkContentRef
    Ret = brObj.DeleteMenu(nId)
    Catch Ex as Exception
        Response.Redirect("reterror.aspx?info="& Server.URLEncode(Ex.Message))
    End Try
		callBackPage = getCallBackupPage("")
        If (Request.QueryString("back") <> "") Then
            'Response.Redirect(Request.QueryString("back"), False)
            Response.Redirect("collections.aspx?action=ViewMenuReport&rf=1")
        ElseIf (callBackPage <> "") Then
            Response.Redirect(callBackPage)
        Else
            Response.Redirect("collections.aspx?action=ViewAllMenus&folderid=" & folderId)
        End If
    
        ElseIf ((action = "doAddMenu") Or (action = "doAddSubMenu") Or (action = "doAddTransMenu")) Then
            Dim folderList As Object
            nId = Request.QueryString("nId")
            folderId = Request.Form("frm_folder_id")
            saveObj = New Collection
            saveObj.Add(action, "SaveType")
	
            If (Not IsNothing(Request.Form("associated_folder_id_list"))) Then
                folderList = Request.Form("associated_folder_id_list")
                ' If (folderList.Length)) Then ...
                saveObj.Add(folderList, "AssociatedFolderIdList")
            End If

            If (Not IsNothing(Request.Form("associated_templates"))) Then
                saveObj.Add(Request.Form("associated_templates"), "AssociatedTemplates")
            End If
		
            Select Case action
                Case "doAddMenu"
                    ' Adding a new Base Menu, therefore ParentID should = 0 AncestorID should = Null and MenuID should = Null
                    ' In the DB, the ParentID will equal zero and the AncestorID will equal whatever new ID is
                    ' created for this new menu.
                    saveObj.Add(Nothing, "MenuID")
                    saveObj.Add(0, "MenuParentID")
                    saveObj.Add(Nothing, "MenuAncestorID")

                Case "doAddSubMenu"
                    ' Add a new Sub Menu. ParentID and AncestorID should just be passed normally. MenuID is generated
                    ' on the back end, so send a null for MenuID.
                    saveObj.Add(Nothing, "MenuID")
                    saveObj.Add(nId, "MenuParentID")
                    saveObj.Add(Request.Form("frm_menu_ancestorid"), "MenuAncestorID")

                Case "doAddTransMenu"
                    ' Adding a new Base Menu that is a translation of an existing menu. We want to pass the MenuID,
                    ' ParentID and AncestorID
                    saveObj.Add(nId, "MenuID")
                    saveObj.Add(0, "MenuParentID")
                    saveObj.Add(Request.Form("frm_menu_ancestorid"), "MenuAncestorID")
            End Select
	
            saveObj.Add(Request.Form("frm_menu_title"), "MenuTitle")
            saveObj.Add(Request.Form("frm_menu_link"), "MenuLink")
            saveObj.Add(Request.Form("frm_menu_template"), "MenuTemplate")
            saveObj.Add(Request.Form("frm_menu_description"), "MenuDescription")
            saveObj.Add(Request.Form("frm_menu_image"), "MenuImage")
            If (LCase(Request.Form("frm_menu_image_override")) = "on") Then
                saveObj.Add("1", "ImageOverride")
            Else
                saveObj.Add("0", "ImageOverride")
            End If
            saveObj.Add(folderId, "FolderID")
            'Menu are always recursive.
            'if (Request.Form("frm_recursive") <> "") then
            saveObj.Add(1, "Recursive")
            'else
            '    saveObj.Add (0, "Recursive")
            'end if
            If (action = "doAddSubMenu") Then
                saveObj.add(True, "IsSubMenu")
            Else
                saveObj.add(False, "IsSubMenu")
            End If

            If (Request.Form("EnableReplication") = "1") Then
                saveObj.Add(1, "EnableReplication")
            Else
                saveObj.Add(0, "EnableReplication")
            End If

            brObj = AppUI.EkContentRef
    
            Try
                Ret = brObj.AddEcmMenu(saveObj)
            Catch ex As Exception
                Response.Redirect("reterror.aspx?info=" & ex.Message)
            End Try
	
            saveObj = Nothing
    
            If (actErrorString <> "") Then ' in this case the ret is the new nav id
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(actErrorString))
                brObj = Nothing
            Else
            If (action = "doAddSubMenu") Then
                Dim submenuID As Long = Ret
                'nId = request.QueryString("nId")
                ItemObj = New Collection
                ItemObj.Add(Ret, "ItemID")
                ItemObj.Add("submenu", "ItemType")
                ItemObj.Add("self", "ItemTarget")
                ItemObj.Add("", "ItemLink")
                ItemObj.Add("", "ItemDescription")
                ItemObj.Add("", "ItemTitle")
                ItemObj.Add("0", "LinkType")
                Ret = brObj.AddItemToEcmMenu(nId, ItemObj)
                If (Request.QueryString("iframe") = "true") Then
                    Response.Write("<script language=""Javascript"">parent.CloseChildPage();</script>")
                ElseIf (NoWorkAreaAttribute <> "") Then
                    Response.Write("<script langua8ge=""Javascript"">")
                    Response.Write(" if (window.opener && !window.opener.closed) {window.opener.document.location.reload();}")
                    Response.Write(" self.close();")
                    Response.Write("</script>")
                Else
                    If (Request.Form("frm_back") <> "") Then
                        'Response.Redirect(Request.Form("frm_back"), False)
                        Response.Redirect("menu.aspx?Action=viewcontent&menuid=" & submenuID & "&LangType=" & ContentLanguage & "&treeViewId=-3&rf=1")
                    ElseIf (m_refApi.TreeModel = 1) Then
                        'Response.Redirect("menutree.aspx?nid=" & nId & "&folderid=" & folderId & "&LangType=" & ContentLanguage & "&bPage=" & bPage)
                        Response.Redirect("menu.aspx?Action=viewcontent&menuid=" & submenuID & "&LangType=" & ContentLanguage & "&treeViewId=-3&rf=1")
                    Else
                        Response.Redirect("collections.aspx?action=ViewMenu&nid=" & nId & "&folderid=" & folderId & "&LangType=" & ContentLanguage & "&bPage=" & bPage)
                    End If
                End If
            Else
                nId = Ret
                If (Request.Form("frm_back") <> "") Then
                    'Dim backpage As String = Request.Form("frm_back")
                    'If (backpage.Contains("&LangType")) Then
                    '    ' strip off language type because we successfully saved this menu
                    '    backpage = backpage.Substring(0, backpage.IndexOf("&LangType")) _
                    '        & "&LangType=" & ContentLanguage & "&rf=1"
                    'End If
                    'If (Not backpage.EndsWith("&rf=1")) Then
                    '    backpage = backpage & "&rf=1"
                    'End If
                    'Response.Redirect(backpage, False)
                    Response.Redirect("menu.aspx?Action=viewcontent&menuid=" & nId & "&LangType=" & ContentLanguage & "&treeViewId=-3&rf=1")
                ElseIf (m_refApi.TreeModel = 1) Then
                    'Response.Redirect("menutree.aspx?nid=" & nId & "&folderid=" & folderId & "&LangType=" & ContentLanguage & "&bPage=" & bPage)
                    Response.Redirect("menu.aspx?Action=viewcontent&menuid=" & nId & "&folderid=" & folderId & "&LangType=" & ContentLanguage & "&treeViewId=-3&rf=1")
                Else
                    Response.Redirect("collections.aspx?action=ViewMenu&folderid=" & folderId & "&LangType=" & ContentLanguage & "&nid=" & nId & "&bPage=" & bPage)
                End If
            End If
                brObj = Nothing
            End If
        ElseIf (action = "doEditMenu") Then
            Dim MenuID As Object
            Dim folderList As Object
            folderId = Request.Form("frm_folder_id")
            MenuID = Request.QueryString("nId")
            saveObj = New Collection
            saveObj.Add(Request.Form("frm_menu_title"), "MenuTitle")
            saveObj.Add(Server.HtmlEncode(Request.Form("frm_menu_link")), "MenuLink")
            saveObj.Add(Server.HtmlEncode(Request.Form("frm_menu_template")), "MenuTemplate")
            saveObj.Add(Server.HtmlEncode(Request.Form("frm_menu_description")), "MenuDescription") 'encode for xml BR
            saveObj.Add(Request.Form("frm_menu_image"), "MenuImage")
            If (LCase(Request.Form("frm_menu_image_override")) = "on") Then
                saveObj.Add("1", "ImageOverride")
            Else
                saveObj.Add("0", "ImageOverride")
            End If
            saveObj.Add(folderId, "FolderID")
            saveObj.Add(MenuID, "MenuID")
            'Menu are always recursive.
            'if (Request.Form("frm_recursive") <> "") then
            saveObj.Add(1, "Recursive")
            'else
            '    saveObj.Add (0, "Recursive")
            'end if
            If (Not IsNothing(Request.Form("associated_folder_id_list"))) Then
                folderList = Request.Form("associated_folder_id_list")
                ' If (folderList.Length)) Then ...
                saveObj.Add(folderList, "AssociatedFolderIdList")
            End If
            If (Not IsNothing(Request.Form("associated_templates"))) Then
                saveObj.Add(Request.Form("associated_templates"), "AssociatedTemplates")
            End If
            If (Request.Form("EnableReplication") <> "") Then
                saveObj.Add(1, "EnableReplication")
            Else
                saveObj.Add(0, "EnableReplication")
            End If
    
            brObj = AppUI.EkContentRef
            Try
                Ret = brObj.UpdateMenu(saveObj)
           
            Catch ex As Exception
                actErrorString = ex.Message
            End Try
            saveObj = Nothing
            If (actErrorString <> "") Then
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(actErrorString))
            Else
                If LCase(Request.Form("frm_set_to_template")) = "true" Then
                    Dim colMenu As Object
                    Dim colMenuItems As Object
                    Dim colMenuItem As Object
                    Dim colNewMenuItem As Object
                    colMenu = brObj.GetMenuByID(MenuID, 0)
                    colMenuItems = colMenu("Items")
			 			 
                    If colMenuItems.Count > 0 Then
                        For Each colMenuItem In colMenuItems
                            If colMenuItem("ItemType") = "1" Then
                                If colMenuItem("LinkType") = "0" Then
                                    colNewMenuItem = brObj.GetMenuItemByID(MenuID, colMenuItem("ID"), True)
						
                                    colNewMenuItem.Remove("LinkType")
                                    colNewMenuItem.Add("1", "LinkType")
							
                                    colNewMenuItem.Remove("ItemID")
                                    colNewMenuItem.Add(colNewMenuItem("ID"), "ItemID")
							
                                    colNewMenuItem.Remove("ItemLink")
                                    colNewMenuItem.Add("", "ItemLink")
																					
                                    Ret = brObj.UpdateMenuItem(colNewMenuItem)
                                End If
                            End If
                        Next
                    End If
                End If
    
                If (Request.Form("frm_back") <> "") Then
                ' the &rf=1 is to force a refresh of the accordion menu tree
                Response.Redirect(Request.Form("frm_back") & "&rf=1", False)
                ElseIf (Request.QueryString("iframe") = "true") Then
                    Response.Write("<script language=""Javascript"">parent.CloseChildPage();</script>")
                Else
                    If (m_refApi.TreeModel = 1) Then
                        Response.Redirect("menutree.aspx?nid=" & MenuID & "&folderid=" & folderId)
                    Else
                        Response.Redirect("collections.aspx?action=ViewMenu&nid=" & MenuID & "&folderid=" & folderId)
                    End If
                End If
            End If
        ElseIf ((action = "DoUpdateOrder") Or (action = "DoUpdateMenuItemOrder")) Then

            orderList = Request.Form("LinkOrder")
            nId = Request.Form("navigationid")
            folderId = Request.Form("frm_folder_id")
            AppUI.FilterByLanguage = ContentLanguage
            brObj = AppUI.EkContentRef
            If (orderList <> "") Then
                If (action = "DoUpdateMenuItemOrder") Then
                    Ret = brObj.UpdateEcmMenuItemOrder(nId, orderList)
                    If (Not Ret) Then
                        If (Request.Form("frm_back") <> "") Then
                        Response.Redirect(Request.Form("frm_back"), False)
                        ElseIf (Request.QueryString("iframe") = "true") Then
                            Response.Write("<script language=""Javascript"">parent.CloseChildPage();</script>")
                        Else
                            If (m_refApi.TreeModel = 1) Then
                                Response.Redirect("menutree.aspx?nid=" & nId & "&folderid=" & folderId)
                            Else
                                Response.Redirect("collections.aspx?Action=ViewMenu&nid=" & nId & "&folderid=" & folderId)
                            End If
                        End If
                    Else
                        Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(actErrorString))
                    End If
                Else
                    bakAction = "action=View"
                    If (Request.QueryString("status") IsNot Nothing) Then
                        If (Request.QueryString("status").ToString().ToLower() = "o") Then
                            bakAction = "action=ViewStage"
                        End If
                    End If
                    Ret = brObj.UpdateEcmCollectionItemOrder(nId, orderList)
                    If (Not Ret) Then
                        Response.Redirect("collections.aspx?" & bakAction & "&LangType=" & ContentLanguage & "&nid=" & nId & "&folderid=" & folderId)
                    Else
                        Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(actErrorString))
                    End If
                End If
            Else
                If (action = "DoUpdateMenuItemOrder") Then
                    If (Request.Form("frm_back") <> "") Then
                        Response.Redirect(Request.Form("frm_back"), False)
                    ElseIf (Request.QueryString("iframe") = "true") Then
                        Response.Write("<script language=""Javascript"">parent.CloseChildPage();</script>")
                    Else
                        If (m_refApi.TreeModel = 1) Then
                            Response.Redirect("menutree.aspx?nid=" & nId & "&folderid=" & folderId)
                        Else
                            Response.Redirect("collections.aspx?Action=ViewMenu&nid=" & nId & "&folderid=" & folderId)
                        End If
                    End If
                Else
                    Response.Redirect("collections.aspx?Action=View&LangType=" & ContentLanguage & "&nid=" & nId & "&folderid=" & folderId)
                End If
            End If
    
        ElseIf (action = "doAddMenuItem") Then
            ItemType = Request.QueryString("type")
            folderId = Request.QueryString("folderid")
            If (folderId = "") Then
                folderId = Request.Form("FolderID")
            End If
            brObj = AppUI.EkContentRef
        
            If (ItemType = "content") Then
                idArray = Split(Request.Form("frm_content_ids"), ",")
                folderArray = Split(Request.Form("frm_folder_ids"), ",")
                brObj = AppUI.EkContentRef
                For lLoop = LBound(idArray) To UBound(idArray)
                    ItemObj = New Collection
                    ItemObj.Add(idArray(lLoop), "ItemID")
                    ItemObj.Add("content", "ItemType")
                    ItemObj.Add("self", "ItemTarget")
                    ItemObj.Add("", "ItemLink")
                    ItemObj.Add("", "ItemDescription")
                    ItemObj.Add("", "ItemTitle")
                    Ret = brObj.AddItemToEcmMenu(Request.Form("CollectionID"), ItemObj)
                    If (actErrorString <> "") Then
                        Exit For
                    End If
                    ItemObj = Nothing
                Next
            ElseIf (ItemType = "link") Then
                ItemObj = New Collection
                ItemObj.Add(0, "ItemID")
                ItemObj.Add("link", "ItemType")
                ItemObj.Add(Request.Form("Title"), "ItemTitle")
                ItemObj.Add(Server.HtmlEncode(CStr(Request.Form("Link"))), "ItemLink")
                ItemObj.Add("self", "ItemTarget")
                ItemObj.Add("", "ItemDescription")
                Ret = brObj.AddItemToEcmMenu(Request.Form("CollectionID"), ItemObj)
                ItemObj = Nothing
            ElseIf (ItemType = "library") Then
                ItemObj = New Collection
                ItemObj.Add(Request.Form("id"), "ItemID")
                ItemObj.Add("library", "ItemType")
                'Titles(Captions) are alway set in menu. 
                'We provide flexibility so user could overwrite them later if needed.
                'if (Request.Form("chkDefault") = "on") then
                '    ItemObj.Add ("", "ItemTitle")
                'else
                ItemObj.Add(Request.Form("title"), "ItemTitle")
                'end if
                ItemObj.Add("self", "ItemTarget")
                ItemObj.Add("", "ItemDescription")
                ItemObj.Add("", "ItemLink")
                Ret = brObj.AddItemToEcmMenu(Request.Form("CollectionID"), ItemObj)
                ItemObj = Nothing
            End If
            brObj = Nothing
            If (actErrorString <> "") Then
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(actErrorString))
            Else
                If (Request.Form("frm_back") <> "") Then
                    ' this is from adding a library item
                Response.Redirect(Request.Form("frm_back") & "&rf=1", False)
                ElseIf (Request.QueryString("iframe") = "true") Then
                    Response.Write("<script language=""Javascript"">parent.CloseChildPage();</script>")
                ElseIf (NoWorkAreaAttribute <> "") Then
                    Response.Write("<script language=""Javascript"">")
                    Response.Write(" if (window.opener && !window.opener.closed) {window.opener.document.location.href = window.opener.document.location.href;}")
                    Response.Write(" self.close();")
                    Response.Write("</script>")
                Else
                    If (m_refApi.TreeModel = 1) Then
                        Response.Redirect("menutree.aspx?nid=" & Request.Form("CollectionID") & "&folderid=" & folderId)
                    Else
                        Response.Redirect("collections.aspx?Action=ViewMenu&nid=" & Request.Form("CollectionID") & "&folderid=" & folderId)
                    End If
                End If
            End If
        ElseIf (action = "doUpdateMenuItem") Then
            Dim MenuID, Title, Link, Target, Description, bRet As Object
            MenuID = Request.Form("CollectionID")
            ID = Request.QueryString("id")
            Title = Request.Form("Title")
            Link = Request.Form("Link")
            Target = Request.Form("Target")
            Description = Request.Form("Description")
            folderId = Request.Form("FolderId")
            ItemType = Request.QueryString("type")
            If (folderId = "") Then
                folderId = 0
            End If
            ItemObj = New Collection
            ItemObj.Add(ID, "ItemID")
            'if (Request.Form("chkDefault") = "on") then
            '    ItemObj.Add ("", "ItemTitle")
            'else
            ItemObj.Add(Title, "ItemTitle")
            'end if
            If ((ItemType = "1") Or (ItemType = 2) Or (ItemType = 4)) Then
                ItemObj.Add("", "ItemLink")
            Else
                ItemObj.Add(Server.HtmlEncode(CStr(Link)), "ItemLink")
            End If
            If ((ItemType = "1")) Then
                ItemObj.Add(Server.HtmlEncode(Request.Form("link")), "LinkType")
            Else
                ItemObj.Add("", "LinkType")
            End If
    
            ItemObj.Add(Target, "Target")
            ItemObj.Add(Description, "ItemDescription")
            ItemObj.Add(MenuID, "MenuID")
            ItemObj.Add(folderId, "FolderID")
            ItemObj.Add(Request.Form("frm_menu_image"), "ItemImage")
            If (LCase(Request.Form("frm_menu_image_override")) = "on") Then
                ItemObj.Add("1", "ImageOverride")
            Else
                ItemObj.Add("0", "ImageOverride")
            End If
    
            brObj = AppUI.EkContentRef
            bRet = brObj.UpdateMenuItem(ItemObj)
            If (actErrorString <> "") Then
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(actErrorString))
            Else
                If (Request.Form("frm_back") <> "") Then
                    Response.Redirect(Request.Form("frm_back"), False)
                ElseIf (Request.QueryString("iframe") = "true") Then
                    Response.Write("<script language=""Javascript"">parent.CloseChildPage();</script>")
                Else
                    If (m_refApi.TreeModel = 1) Then
                        Response.Redirect("menutree.aspx?nid=" & MenuID & "&folderid=" & folderId)
                    Else
                        Response.Redirect("collections.aspx?Action=ViewMenu&nid=" & MenuID & "&folderid=" & folderId)
                    End If
                End If
            End If
        ElseIf (action = "doDeleteMenuItem") Then
            Dim arTmp, arInfo As Object
            ' takes CSV list of itemID.menuID
            If (Request.QueryString("back") <> "") Then
                ' this comes from the viewitems menu control
                idArray = Split(Request.QueryString("ids"), ",")
            ElseIf (m_refApi.TreeModel = 1) Then
                idArray = Split(Request.QueryString("frm_content_ids"), ",")
            Else
                idArray = Split(Request.Form("frm_content_ids"), ",")
            End If
            folderId = Request.QueryString("folderid")
            pid = Request.QueryString("pid")
            brObj = AppUI.EkContentRef
            For lLoop = LBound(idArray) To UBound(idArray)
                arTmp = Split(idArray(lLoop), ".")
                If (arTmp.Length > 0) Then
                    arInfo = Split(arTmp(0), "_")
                    If (arInfo.Length > 0) Then
                        If (Request.QueryString("back") <> "") Then
                            If ((Request.QueryString("nid") <> "") And (arInfo(0) <> "")) Then
                                Ret = brObj.DeleteItemFromMenu(Request.QueryString("nid"), arInfo(0))
                            End If
                        ElseIf (m_refApi.TreeModel = 1) Then
                            If ((Request.QueryString("CollectionID") <> "") And (arInfo(0) <> "")) Then
                                Ret = brObj.DeleteItemFromMenu(Request.QueryString("CollectionID"), arInfo(0))
                            End If
                        Else
                            If ((Request.Form("CollectionID") <> "") And (arInfo(0) <> "")) Then
                                Ret = brObj.DeleteItemFromMenu(Request.Form("CollectionID"), arInfo(0))
                            End If
                        End If
                    End If
                    If (arTmp.Length > 1) Then
                        If (arTmp(1) = 4) Then
                            If ((actErrorString = "") And (arTmp(2) <> "")) Then
                                Ret = brObj.DeleteMenu(arTmp(2))
                            End If
                        End If
                    End If
                    If (actErrorString <> "") Then
                        Exit For
                    End If
                End If
            Next
            brObj = Nothing
            If (actErrorString <> "") Then
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(actErrorString))
            Else
                If (Request.QueryString("back") <> "") Then
                Response.Redirect(Request.QueryString("back") & "&rf=1", False)
                ElseIf (Request.Form("frm_back") <> "") Then
                    Response.Redirect(Request.Form("frm_back"), False)
                ElseIf (Request.QueryString("iframe") = "true") Then
                    If (m_refApi.TreeModel = 1) Then
                        Response.Write("<script language=""Javascript"">parent.CloseChildPage();</script>")
                    Else
                        Response.Redirect("collections.aspx?Action=ViewMenu&nid=" & Request.QueryString("nid") & "&folderid=" & folderId)
                    End If
                Else
                    If (m_refApi.TreeModel = 1) Then
                        Response.Redirect("menutree.aspx?nid=" & Request.Form("pid") & "&folderid=" & folderId)
                    Else
                        Response.Redirect("collections.aspx?Action=ViewMenu&nid=" & Request.Form("CollectionID") & "&folderid=" & folderId)
                    End If
                End If
            End If
        ElseIf ((action = "doDeleteLinks") Or (action = "doAddLinks")) Then
            Dim langArray As Object
        
            idArray = Split(Request.Form("frm_content_ids"), ",")
            langArray = Split(Request.Form("frm_content_languages"), ",")
            folderArray = Split(Request.Form("frm_folder_ids"), ",")
	
            folderId = Request.QueryString("folderid")
            brObj = AppUI.EkContentRef
            For lLoop = LBound(idArray) To UBound(idArray)
                If (action = "doDeleteLinks") Then
                    AppUI.ContentLanguage = langArray(lLoop)
                    Ret = brObj.DeleteItemFromEcmCollection(Request.Form("CollectionID"), idArray(lLoop), langArray(lLoop))
                Else
                    AppUI.ContentLanguage = langArray(lLoop)
                    Ret = brObj.AddItemToEcmCollection(Request.Form("CollectionID"), idArray(lLoop), langArray(lLoop))
                End If
                If (actErrorString <> "") Then
                    Exit For
                End If
            Next
            brObj = Nothing
            bakAction = "action=View"
            If (Request.QueryString("status") IsNot Nothing) Then
                If (Request.QueryString("status").ToString().ToLower() = "o") Then
                    bakAction = "action=ViewStage"
                End If
            End If
            If (actErrorString <> "") Then
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(actErrorString))
            Else
                If (NoWorkAreaAttribute <> "") Then
                    Response.Write("<script language=""Javascript"">")
                    Response.Write(" if (window.opener && !window.opener.closed) {window.opener.document.location.href = window.opener.document.location.href;}")
                    Response.Write(" self.close();")
                    Response.Write("</script>")
                Else
                    Response.Redirect("collections.aspx?" & bakAction & "&nid=" & Request.Form("CollectionID") & "&folderid=" & folderId)
                End If
            End If

        ElseIf (action = "doEdit") Then
            Dim edObj As Object
            nId = Request.Form("frm_nav_id")
            folderId = Request.Form("frm_folder_id")
            edObj = New Collection
            edObj.Add(Request.Form("frm_nav_title"), "CollectionTitle")
            edObj.Add(Request.Form("frm_nav_template"), "CollectionTemplate")
            edObj.Add(Request.Form("frm_nav_description"), "CollectionDescription")
            edObj.Add(Request.Form("frm_nav_id"), "CollectionID")
            edObj.Add(folderId, "FolderID")
            If (Request.Form("frm_recursive") <> "") Then
                edObj.Add(1, "Recursive")
            Else
                edObj.Add(0, "Recursive")
            End If
            If (Request.Form("frm_approval_methhod") <> "") Then
                edObj.Add(True, "ApprovalRequired")
            Else
                edObj.Add(False, "ApprovalRequired")
            End If
            If (Request.Form("EnableReplication") <> "") Then
                edObj.Add(1, "EnableReplication")
            Else
                edObj.Add(0, "EnableReplication")
            End If
            brObj = AppUI.EkContentRef
            Ret = brObj.UpdateEcmCollectionInfo(edObj)
            brObj = Nothing

            bakAction = "action=View"
            If (Request.QueryString("status") = "o") And (edObj("ApprovalRequired")) Then
                bakAction = "action=ViewStage"
            End If
            If (Len(actErrorString)) Then
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(actErrorString))
            Else
            Response.Redirect("collections.aspx?" & bakAction & "&nid=" & nId & "&folderid=" & folderId & "&rf=1")
            End If

        ElseIf (action = "doDelete") Then
            nId = Request.QueryString("nId")
            brObj = AppUI.EkContentRef
            Ret = brObj.DeleteCollectionItem(nId)
            brObj = Nothing
            If (Len(actErrorString)) Then
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(actErrorString))
            Else
            Response.Redirect("collections.aspx?action=ViewCollectionReport&rf=1")
            End If

        ElseIf (action = "doSubmitCol") Then
            nId = Request.QueryString("nId")
            brObj = AppUI.EkContentRef
            Try
                Ret = brObj.SubmitEcmCollection(nId)
                Response.Redirect("collections.aspx?Action=View&nid=" & nId & "&folderid=" & folderId, False)
            Catch ex As Exception
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message.ToString()), False)
            End Try
        ElseIf (action = "doPublishCol") Then
            nId = Request.QueryString("nId")
            brObj = AppUI.EkContentRef
            Try
            Ret = brObj.PublishEcmCollection(nId)
            ' had to use rf=1 to refresh the tree or the context menu would read the wrong state for the collection
                Response.Redirect("collections.aspx?Action=View&nid=" & nId & "&folderid=" & folderId & "&rf=1", False)
            Catch ex As Exception
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message.ToString()), False)
            End Try
        ElseIf (action = "doSubmitDelCol") Then
            nId = Request.QueryString("nId")
            brObj = AppUI.EkContentRef
            Try
                Dim bCheckout As Boolean = False
                If (Request.QueryString("checkout") IsNot Nothing) Then
                    If ((Request.QueryString("status") Is Nothing) OrElse (Request.QueryString("status") IsNot Nothing AndAlso Request.QueryString("status") <> "o")) Then
                        bCheckout = True
                        Ret = brObj.CheckoutEcmCollection(nId)
                    End If
                End If
                If (Not bCheckout) Then
                    Ret = brObj.DeleteCollectionItem(nId)
                Else
                    Ret = brObj.SubmitDeleteEcmCollection(nId)
                End If
                If (bCheckout) Then
                    Response.Redirect("collections.aspx?Action=ViewStage&nid=" & nId & "&folderid=" & folderId, False)
                Else
                    Response.Redirect("collections.aspx?Action=ViewAll&nid=" & nId & "&folderid=" & folderId, False)
                End If
            Catch ex As Exception
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message.ToString()), False)
            End Try
        ElseIf (action = "doDeclineDelCol") Then
            Dim strDeleteDeclineMsg As String = ""
            nId = Request.QueryString("nId")
            brObj = AppUI.EkContentRef
            Try
                If (Request.Form("DeleteDeclineReason") IsNot Nothing) Then
                    strDeleteDeclineMsg = Request.Form("DeleteDeclineReason")
                End If
                Ret = brObj.DeleteDeclineEcmCollection(nId, strDeleteDeclineMsg)
                Response.Redirect("collections.aspx?Action=View&nid=" & nId & "&folderid=" & folderId, False)
            Catch ex As Exception
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message.ToString()), False)
            End Try
        ElseIf (action = "doDeclineApprCol") Then
            Dim strApprovalDeclineMsg As String = ""
            nId = Request.QueryString("nId")
            brObj = AppUI.EkContentRef
            Try
                If (Request.Form("ApprovalDeclineReason") IsNot Nothing) Then
                    strApprovalDeclineMsg = Request.Form("ApprovalDeclineReason")
                End If
                Ret = brObj.ApprovalDeclineEcmCollection(nId, strApprovalDeclineMsg)
                Response.Redirect("collections.aspx?Action=View&nid=" & nId & "&folderid=" & folderId, False)
            Catch ex As Exception
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message.ToString()), False)
            End Try
        ElseIf (action = "doUndoCheckoutCol") Then
            nId = Request.QueryString("nId")
            brObj = AppUI.EkContentRef
            Try
                Ret = brObj.UndoCheckoutEcmCollection(nId)
                Response.Redirect("collections.aspx?Action=View&nid=" & nId & "&folderid=" & folderId, False)
            Catch ex As Exception
                Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message.ToString()), False)
            End Try
        End If
%>
