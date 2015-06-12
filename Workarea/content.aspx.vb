Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.API

Partial Class content
    Inherits System.Web.UI.Page

#Region "Member Variables"

    Protected Enum Flag
        Disable = 0
        Enable = 1
    End Enum
    Dim MasterLayouts As String = String.Empty
    Protected m_bAjaxTree As Boolean = False
    Protected m_viewfolder As viewfolder
    Protected PageAction As String = ""
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected ContentLanguage As Integer = -1
    Protected m_intFolderId As Long = -1
    Protected m_refContentApi As New ContentAPI
    Protected m_bLangChange As Boolean = False
    Protected m_strReloadJS As String = ""
    Protected m_strEmailArea As String = ""
    Protected m_strMembership As String = ""
    Protected m_jsResources As Sync_jsResources
    Protected m_refCatalog As Ektron.Cms.Commerce.CatalogEntry = Nothing

    Private _ApplicationPath As String
    Private _SitePath As String

#End Region

#Region "Properties"

    Public Property SitePath() As String
        Get
            Return _SitePath
        End Get
        Set(ByVal Value As String)
            _SitePath = Value
        End Set
    End Property

    Public Property ApplicationPath() As String
        Get
            Return _ApplicationPath
        End Get
        Set(ByVal Value As String)
            _ApplicationPath = Value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New()

        Dim endSlash() As Char = {"/"}
        Me.ApplicationPath = m_refContentApi.ApplicationPath.TrimEnd(endSlash)
        Me.SitePath = m_refContentApi.SitePath.TrimEnd(endSlash)
        m_refMsg = m_refContentApi.EkMsgRef
    End Sub

#End Region

#Region "PreInit, Load, PreRender"

    Private Sub Page_PreInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.PreInit

        If (Not (Request.QueryString("action") Is Nothing)) Then
            PageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If

        Dim bCompleted As Boolean
        Select Case PageAction
            Case "view", "viewstaged"
                UniqueLiteral.Text = "content"
                Dim m_viewcontent As viewcontent
                m_viewcontent = CType(LoadControl("controls/content/viewcontent.ascx"), viewcontent)
                m_viewcontent.ID = "content"
                DataHolder.Controls.Add(m_viewcontent)
                bCompleted = m_viewcontent.ViewContent
                Me.ltr_commerceCSSJS.Text = m_viewcontent.GetCommerceIncludes()
        End Select

        'Load Conditional JS
        If Trim(Request.QueryString("ShowTStatusMsg")) IsNot Nothing Then
            If Trim(Request.QueryString("ShowTStatusMsg")) = "1" Then
                phShowTStatusMessage.Visible = True
            End If
        End If
        If m_bAjaxTree = True Then
            phShowAjaxTree.Visible = True
        End If
    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        Me.ShowAjaxTreeJsValues()
        Me.SetEktronContentTemplateJsValues()
        Me.RegisterJs()
        Me.RegisterCss()

        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1
        m_refMsg = m_refContentApi.EkMsgRef

        If (m_refContentApi.GetCookieValue("user_id") = 0) Then
            If Not (Request.QueryString("callerpage") Is Nothing) Then
                Session("RedirectLnk") = "Content.aspx?" & Request.QueryString.ToString()
            End If
            Response.Redirect("login.aspx?fromLnkPg=1", False)
            Exit Sub
        End If

        If (Not (Request.QueryString("action") Is Nothing)) Then
            PageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If

        If (Not (Request.QueryString("membership") Is Nothing)) Then
            m_strMembership = Convert.ToString(Request.QueryString("membership")).ToLower.Trim
        End If

        If (m_refContentApi.TreeModel = 1) Then
            m_bAjaxTree = True
        End If
        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            If (Request.QueryString("LangType") <> "") Then
                If Convert.ToInt32(Request.QueryString("LangType")) <> Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID")) Then
                    m_bLangChange = True
                End If
                ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)

            Else
                If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If
        If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            m_refContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            m_refContentApi.ContentLanguage = ContentLanguage
        End If
        ltrStyleSheetJs.Text = (New StyleHelper).GetClientScript
        txtContentLanguage.Text = m_refContentApi.ContentLanguage
        txtDefaultContentLanguage.Text = m_refContentApi.DefaultContentLanguage
        txtEnableMultilingual.Text = m_refContentApi.EnableMultilingual

        Select Case PageAction
            Case "viewarchivecontentbycategory", "viewcontentbycategory"
                UniqueLiteral.Text = "viewfolder"
                m_viewfolder = CType(LoadControl("controls/folder/viewfolder.ascx"), viewfolder)
                m_viewfolder.ID = "viewfolder"
                If m_bLangChange = True Then
                    m_viewfolder.ResetPostData()
                End If
                DataHolder.Controls.Add(m_viewfolder)
            Case "deletecontentbycategory"
                UniqueLiteral.Text = "deletecontentbycategory"
                Dim m_removefolderitem As removefolderitem
                m_removefolderitem = CType(LoadControl("controls/folder/removefolderitem.ascx"), removefolderitem)
                m_removefolderitem.ID = "deletecontentbycategory"
                DataHolder.Controls.Add(m_removefolderitem)
            Case "movecontentbycategory"
                UniqueLiteral.Text = "movefolderitem"
                Dim m_movefolderitem As movefolderitem
                m_movefolderitem = CType(LoadControl("controls/folder/movefolderitem.ascx"), movefolderitem)
                m_movefolderitem.ID = "movefolderitem"
                DataHolder.Controls.Add(m_movefolderitem)
            Case "selectpermissions"
                UniqueLiteral.Text = "permission"
                Dim m_selectpermissions As selectpermissions
                m_selectpermissions = CType(LoadControl("controls/permission/selectpermissions.ascx"), selectpermissions)
                m_selectpermissions.ID = "permission"
                DataHolder.Controls.Add(m_selectpermissions)
        End Select
        Dim m_mail As New EmailHelper

        Dim strEmailArea As String
        strEmailArea = m_mail.EmailJS
        strEmailArea += m_mail.MakeEmailArea
        ltrEmailAreaJs.Text = strEmailArea

        If (PageAction.ToLower().ToString() <> "viewcontentbycategory") Then
            ShowDropUploader(False)
        End If

        ' resource text string tokens
        Dim closeDialogText As String = m_refMsg.GetMessage("close this dialog")
        Dim cancelText As String = m_refMsg.GetMessage("btn cancel")

        ' assign resource text string values
        btnConfirmOk.Text = m_refMsg.GetMessage("lbl ok")
        btnConfirmOk.NavigateUrl = "#" & m_refMsg.GetMessage("lbl ok")
        btnConfirmCancel.Text = cancelText
        btnConfirmCancel.NavigateUrl = "#" & cancelText
        btnCloseSyncStatus.Text = m_refMsg.GetMessage("close title")
        btnCloseSyncStatus.NavigateUrl = "#" & m_refMsg.GetMessage("close title")
        btnStartSync.Text = m_refMsg.GetMessage("btn sync now")

        closeDialogLink.Text = "<span class=""ui-icon ui-icon-closethick"">" & m_refMsg.GetMessage("close title") & "</span>"
        closeDialogLink.NavigateUrl = "#" + System.Text.RegularExpressions.Regex.Replace(m_refMsg.GetMessage("close title"), "\s+", "")
        closeDialogLink.ToolTip = closeDialogText
        closeDialogLink2.Text = closeDialogLink.Text
        closeDialogLink2.NavigateUrl = closeDialogLink.NavigateUrl
        closeDialogLink2.ToolTip = closeDialogText
        closeDialogLink3.Text = closeDialogLink.Text
        closeDialogLink3.NavigateUrl = closeDialogLink.NavigateUrl
        closeDialogLink3.ToolTip = closeDialogText

        lblSyncStatus.Text = m_refMsg.GetMessage("lbl sync status")

        m_jsResources = CType(LoadControl("sync/sync_jsResources.ascx"), Sync_jsResources)
        m_jsResources.ID = "jsResources"
        sync_jsResourcesPlaceholder.Controls.Add(m_jsResources)
    End Sub

    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
        Dim bCompleted As Boolean
        Try
            If (Request.QueryString("reloadtrees") <> "") Then
                ltrEktronReloadJs.Text = ReloadClientScript()
            End If

            Select Case PageAction
                Case "addsubfolder"
                    UniqueLiteral.Text = "addfolder"
                    Dim m_addfolder As addfolder
                    m_addfolder = CType(LoadControl("controls/folder/addfolder.ascx"), addfolder)
                    m_addfolder.ID = "adddfolder"
                    DataHolder.Controls.Add(m_addfolder)
                    bCompleted = m_addfolder.AddSubFolder
                Case "viewfolder"
                    UniqueLiteral.Text = "viewfolder"
                    Dim m_viewfolderattributes As viewfolderattributes
                    m_viewfolderattributes = CType(LoadControl("controls/folder/viewfolderattributes.ascx"), viewfolderattributes)
                    m_viewfolderattributes.ID = "viewfolder"
                    DataHolder.Controls.Add(m_viewfolderattributes)
                    bCompleted = m_viewfolderattributes.ViewFolderAttributes
                Case "editfolder"
                    UniqueLiteral.Text = "editfolder"
                    Dim m_editfolderattributes As editfolderattributes
                    m_editfolderattributes = CType(LoadControl("controls/folder/editfolderattributes.ascx"), editfolderattributes)
                    m_editfolderattributes.ID = "editfolder"
                    DataHolder.Controls.Add(m_editfolderattributes)
                    bCompleted = m_editfolderattributes.EditFolderAttributes
                Case "editcontentproperties"
                    UniqueLiteral.Text = "content"
                    Dim m_editcontentattributes As editcontentattributes
                    m_editcontentattributes = CType(LoadControl("controls/content/editcontentattributes.ascx"), editcontentattributes)
                    m_editcontentattributes.ID = "content"
                    DataHolder.Controls.Add(m_editcontentattributes)
                    bCompleted = m_editcontentattributes.EditContentProperties
                Case "movecontent"
                    UniqueLiteral.Text = "content"
                    Dim m_movecontent As movecontent
                    m_movecontent = CType(LoadControl("controls/content/movecontent.ascx"), movecontent)
                    m_movecontent.ID = "content"
                    DataHolder.Controls.Add(m_movecontent)
                    bCompleted = m_movecontent.MoveContent
                Case "localize", "localizeexport"
                    UniqueLiteral.Text = "localize"
                    Dim m_localization As localization_uc
                    m_localization = CType(LoadControl("controls/content/localization_uc.ascx"), localization_uc)
                    m_localization.ID = "localization"
                    DataHolder.Controls.Add(m_localization)
                    bCompleted = m_localization.Display
                Case "viewpermissions"
                    UniqueLiteral.Text = "permission"
                    Dim m_viewpermissions As viewpermissions
                    m_viewpermissions = CType(LoadControl("controls/permission/viewpermissions.ascx"), viewpermissions)
                    m_viewpermissions.ID = "permission"
                    DataHolder.Controls.Add(m_viewpermissions)
                    bCompleted = m_viewpermissions.ViewPermission
                Case "deletepermissions"
                    UniqueLiteral.Text = "permission"
                    Dim m_deletepermissions As deletepermissions
                    m_deletepermissions = CType(LoadControl("controls/permission/deletepermissions.ascx"), deletepermissions)
                    m_deletepermissions.ID = "permission"
                    DataHolder.Controls.Add(m_deletepermissions)
                    bCompleted = m_deletepermissions.DeletePermission
                Case "editpermissions"
                    UniqueLiteral.Text = "permission"
                    Dim m_editpermissions As editpermissions
                    m_editpermissions = CType(LoadControl("controls/permission/editpermissions.ascx"), editpermissions)
                    m_editpermissions.ID = "permission"
                    DataHolder.Controls.Add(m_editpermissions)
                    bCompleted = m_editpermissions.EditPermission
                Case "addpermissions"
                    UniqueLiteral.Text = "permission"
                    Dim m_editpermissions As editpermissions
                    m_editpermissions = CType(LoadControl("controls/permission/editpermissions.ascx"), editpermissions)
                    m_editpermissions.ID = "permission"
                    DataHolder.Controls.Add(m_editpermissions)
                    bCompleted = m_editpermissions.AddPermission
                Case "viewapprovals"
                    UniqueLiteral.Text = "approval"
                    Dim m_viewapprovals As viewapprovals
                    m_viewapprovals = CType(LoadControl("controls/approval/viewapprovals.ascx"), viewapprovals)
                    m_viewapprovals.ID = "approval"
                    DataHolder.Controls.Add(m_viewapprovals)
                    bCompleted = m_viewapprovals.ViewApproval
                Case "editapprovalmethod"
                    UniqueLiteral.Text = "approval"
                    Dim m_editapprovalmethod As editapprovalmethod
                    m_editapprovalmethod = CType(LoadControl("controls/approval/editapprovalmethod.ascx"), editapprovalmethod)
                    m_editapprovalmethod.ID = "approval"
                    DataHolder.Controls.Add(m_editapprovalmethod)
                    bCompleted = m_editapprovalmethod.EditApprovalMethod
                Case "editpreapprovals"
                    UniqueLiteral.Text = "preapproval"
                    Dim m_editpreapproval As editpreapproval
                    m_editpreapproval = CType(LoadControl("controls/approval/editpreapproval.ascx"), editpreapproval)
                    m_editpreapproval.ID = "preapproval"
                    DataHolder.Controls.Add(m_editpreapproval)
                    bCompleted = m_editpreapproval.EditPreApproval
                Case "addapproval"
                    UniqueLiteral.Text = "approval"
                    Dim m_addapproval As addapproval
                    m_addapproval = CType(LoadControl("controls/approval/addapproval.ascx"), addapproval)
                    m_addapproval.ID = "approval"
                    DataHolder.Controls.Add(m_addapproval)
                    bCompleted = m_addapproval.AddApproval
                Case "editapprovalorder"
                    UniqueLiteral.Text = "approval"
                    Dim m_editapprovalorder As editapprovalorder
                    m_editapprovalorder = CType(LoadControl("controls/approval/editapprovalorder.ascx"), editapprovalorder)
                    m_editapprovalorder.ID = "approval"
                    DataHolder.Controls.Add(m_editapprovalorder)
                    bCompleted = m_editapprovalorder.EditApprovalOrder
                Case "deleteapproval"
                    UniqueLiteral.Text = "approval"
                    Dim m_deleteapproval As deleteapproval
                    m_deleteapproval = CType(LoadControl("controls/approval/deleteapproval.ascx"), deleteapproval)
                    m_deleteapproval.ID = "approval"
                    DataHolder.Controls.Add(m_deleteapproval)
                    bCompleted = m_deleteapproval.DeleteApproval
                    'Case "contentmessage"
                    '    UniqueLiteral.Text = "contentmessage"
                    '    Dim m_contentmessage As contentmessage
                    '    m_contentmessage = CType(LoadControl("controls/content/contentmessage.ascx"), contentmessage)
                    '    m_contentmessage.ID = "contentmessage"
                    '    DataHolder.Controls.Add(m_contentmessage)
                    'bCompleted = m_contentmessage.contentmessage
                Case "enableitemprivatesetting"
                    Process_DoItemPrivateSetting(Flag.Enable)
                Case "disableitemprivatesetting"
                    Process_DoItemPrivateSetting(Flag.Disable)
                Case "enableiteminheritance"
                    Process_DoItemInheritance(Flag.Enable)
                Case "disableiteminheritance"
                    Process_DoItemInheritance(Flag.Disable)
                Case "dodeletefolder"
                    Process_DoDeleteFolder()
                Case "reestablishsession"
                    Process_DoReestablishSession()
                Case "checkin"
                    Process_CheckInContent()
                Case "submit"
                    Process_DoSubmit()
                Case "dodeletepermissions"
                    Process_DoDeletePermission()
                Case "doadditemapproval"
                    Process_DoAddItemApproval()
                Case "dodeleteitemapproval"
                    Process_DoDeleteItemApproval()
                Case "submitdelcontaction"
                    Process_DeleteContent()
                Case "submitdelcatalogaction"
                    Process_DeleteContent()
                Case "workoffline"
                    Process_WorkOffLine()
                Case "approvecontent"
                    Do_ApproveContent()
                Case "declinecontent"
                    Do_DeclineContent()
                Case "restoreinheritance"
                    Do_RestoreInheritance()
            End Select
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
        AssignTextValues()
    End Sub

#End Region

#Region "Helper Methods"

    Protected Sub ShowAjaxTreeJsValues()
        litShowAjaxTreeFolderId.Text = Request.QueryString("id")
    End Sub

    Protected Sub SetEktronContentTemplateJsValues()
        'This method populates literals for <script id="EktronContentTemplateJs">
        litConfirmContentDeletePublish.Text = m_refMsg.GetMessage("js: confirm content delete")
        litConfirmContentDeleteSubmission.Text = m_refMsg.GetMessage("js: confirm content delete submission")
        litConfirmContentDeleteDialog.Text = m_refMsg.GetMessage("js: confirm content delete")
        litConfirmFolderDelete.Text = m_refMsg.GetMessage("js: confirm folder delete")
        litConfirmFolderDeleteBelowRoot.Text = m_refMsg.GetMessage("js: confirm delete folders below root")
        litAlertSupplyFoldername.Text = m_refMsg.GetMessage("js: alert supply foldername")
        litAlertRequiredDomain.Text = m_refMsg.GetMessage("js required domain msg")
        litAlertMissingAlternateStylesheet.Text = m_refMsg.GetMessage("js: alert stylesheet must have css")
        litConfirmDeleteGroupPermissions.Text = m_refMsg.GetMessage("js: confirm delete group permissions")
        litConfirmDeleteUserPermissions.Text = m_refMsg.GetMessage("js: confirm delete user permissions")
        litAlertCannotDisableReadonly.Text = m_refMsg.GetMessage("js: alert cannot disable readonly")
        litAlertCannotDisableLibraryReadonly.Text = m_refMsg.GetMessage("js: alert cannot disable library readonly")
        litAlertCannotDisablePostReply.Text = m_refMsg.GetMessage("js: alert cannot disable postreply")
        litAlertCannotDisableEdit.Text = m_refMsg.GetMessage("js: alert cannot disable edit")
        litAlertSelectPermission.Text = m_refMsg.GetMessage("js: alert select permission")
        litAlertReadContentPermissionRemovalEffectWarning.Text = m_refMsg.GetMessage("js: read content permission removal effect warning")
        litConfirmDisableInheritance.Text = m_refMsg.GetMessage("js: confirm disable inheritance")
        litConfirmEnableInheritance.Text = m_refMsg.GetMessage("js: confirm enable inheritance")
        litConfirmMakeContentPrivate.Text = m_refMsg.GetMessage("js: confirm make content private")
        litConfirmMakeContentPublic.Text = m_refMsg.GetMessage("js: confirm make content public")
        litAlertCannotAlterPContSetting.Text = m_refMsg.GetMessage("js: alert cannot alter pcont setting")
        litConfirmMakeFolderPrivate.Text = m_refMsg.GetMessage("js: confirm make folder private")
        litConfirmMakeFolderPublic.Text = m_refMsg.GetMessage("js: confirm make folder public")
        litConfirmAddApproverGroup.Text = m_refMsg.GetMessage("js: confirm add approver group")
        litConfirmAddApproverUser.Text = m_refMsg.GetMessage("js: confirm add approver user")
        litConfirmDeleteApproverGroup.Text = m_refMsg.GetMessage("js: confirm delete approver group")
        litConfirmDeleteApproverUser.Text = m_refMsg.GetMessage("js: confirm delete approver user")
        litAlertSelectUserOrGroup.Text = m_refMsg.GetMessage("js select user or group")
        litAlertMetadataNotCompleted.Text = m_refMsg.GetMessage("js: alert meta data not completed")
        litContentTypeFolderId.Text = Request.QueryString("id")
        litTemplateConfigSaveFolderId.Text = Request.QueryString("id")
        litConfirmBreakInheritanceFlagging.text = m_refMsg.GetMessage("js: confirm break inheritance")
        If m_strMembership.ToLower.Trim = "true" Then
            litDisableInheritenceIfMembershipTrue.Text = "&membership=true"
            litEnableInheritenceIfMembershipTrue.Text = "&membership=true"
            litEnableItemPrivateSettingMembershipTrue.Text = "&membership=true"
            litDisableItemPrivateSettingMembershipTrue.Text = "&membership=true"
        End If
    End Sub

    Private Function ReloadClientScript() As String
        Dim result As New System.Text.StringBuilder
        Dim pid As Long = 0
        Dim FolderPath As String = ""
        Try
            If (Not (Request.QueryString("id") Is Nothing)) Then
                pid = Request.QueryString("id")
            ElseIf (Not (Request.QueryString("id") Is Nothing)) Then
                pid = Request.QueryString("folder_id")
            End If

            Dim contObj As Ektron.Cms.Content.EkContent = m_refContentApi.EkContentRef
            If (pid >= 0) Then
                FolderPath = contObj.GetFolderPath(pid)
            End If
            contObj = Nothing
            result.Append("<script language=""javascript"">" & vbCrLf)
            If (m_refContentApi.TreeModel = 1 And pid <> 0) Then
                If ((Not (Request.QueryString("TreeUpdated") Is Nothing)) AndAlso (Request.QueryString("TreeUpdated") = 1)) Then
                    pid = m_refContentApi.GetParentIdByFolderId(pid)
                    If (pid = -1) Then
                        result.Length = 0
                        Exit Try
                    End If
                End If
                result.Append("if (typeof (reloadFolder) == 'function'){" & vbCrLf)
                result.Append("     reloadFolder(" + Convert.ToString(pid) + ");" & vbCrLf)
                result.Append("}" & vbcrlf)
                FolderPath = FolderPath.Replace("\", "\\")
                result.Append("top.TreeNavigation(""LibraryTree"", """ & FolderPath & """);" & vbCrLf)
                result.Append("top.TreeNavigation(""ContentTree"", """ & FolderPath & """);" & vbCrLf)
                If PageAction = "viewboard" Then
                    result.Append("window.location.href = 'threadeddisc/addeditboard.aspx?action=View&id=" & Request.QueryString("id") & "';" & vbCrLf)
                End If
            Else
                result.Append("<!--" & vbCrLf)
                result.Append("	// If reloadtrees paramter exists, reload selected navigation trees:" & vbCrLf)
                result.Append("	var m_reloadTrees = """ & Request.QueryString("reloadtrees") & """;" & vbCrLf)
                result.Append("	top.ReloadTrees(m_reloadTrees);" & vbCrLf)
                result.Append("	self.location.href=""" & Request.ServerVariables("path_info") & "?" & Replace(Request.ServerVariables("query_string"), "&reloadtrees=" & Request.QueryString("reloadtrees"), "") & """;" & vbCrLf)
                result.Append("	// If TreeNav parameters exist, ensure the desired folders are opened:" & vbCrLf)
                result.Append("	var strTreeNav = """ & Request.QueryString("TreeNav") & """;" & vbCrLf)
                result.Append("	if (strTreeNav != null) {" & vbCrLf)
                result.Append("		strTreeNav = strTreeNav.replace(/\\\\/g,""\\"");" & vbCrLf)
                result.Append("		top.TreeNavigation(""LibraryTree"", strTreeNav);" & vbCrLf)
                result.Append("		top.TreeNavigation(""ContentTree"", strTreeNav);" & vbCrLf)
                result.Append("	}" & vbCrLf)
                result.Append("//-->" & vbCrLf)
            End If
            result.Append("</script>" & vbCrLf)
        Catch ex As Exception
        End Try
        Return (result.ToString)
    End Function

    Private Sub ShowDropUploader(ByVal bShow As Boolean)
        Dim sJS As New System.Text.StringBuilder
        sJS.Append("<script language=""Javascript"">" & vbCrLf)
        If (bShow) Then
            sJS.Append(" if (typeof top != 'undefined') { " & vbCrLf)
            sJS.Append("    top.ShowDragDropWindow();" & vbCrLf)
            sJS.Append(" }" & vbCrLf)
        Else
            sJS.Append("if ((typeof(top.GetEkDragDropObject)).toLowerCase() != 'undefined') {" & vbCrLf)
            sJS.Append("	var dragDropFrame = top.GetEkDragDropObject();" & vbCrLf)
            sJS.Append("		if (dragDropFrame != null) {" & vbCrLf)
            sJS.Append("			dragDropFrame.location.href = ""blank.htm"";" & vbCrLf)
            sJS.Append("		}" & vbCrLf)
            sJS.Append("}" & vbCrLf)
            sJS.Append("if ((typeof(top.GetEkDragDropObject)).toLowerCase() != 'undefined') {" & vbCrLf)
            sJS.Append("	top.HideDragDropWindow();" & vbCrLf)
            sJS.Append("}" & vbCrLf)
        End If
        sJS.Append("</script>" & vbCrLf)
        Page.ClientScript.RegisterClientScriptBlock(GetType(Page), "DragUploaderJS", sJS.ToString())
    End Sub

#End Region

#Region "SUBMIT ACTION PROCESS"
#Region "ACTION - CheckIn"
    Private Sub Process_CheckInContent()
        Dim strCallBackPage As String = ""
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim m_intId As Long
        Dim m_refStyle As New StyleHelper
        Dim AssetFileName As String = ""
        Dim contentType As Integer
        Try
            m_refContent = m_refContentApi.EkContentRef
            m_intId = Request.QueryString("id")
            If Request.QueryString("content_type") <> "" Then contentType = Request.QueryString("content_type")

            Select Case contentType

                Case Ektron.Cms.Common.EkConstants.CMSContentType_CatalogEntry

                    m_refCatalog = New Ektron.Cms.Commerce.CatalogEntry(m_refContentApi.RequestInformationRef)

                    Dim entry As Ektron.Cms.Commerce.EntryData = m_refCatalog.GetItemEdit(m_intId, m_refContentApi.RequestInformationRef.ContentLanguage, False)

                    If ((entry IsNot Nothing) AndAlso (entry.LastEditorId <> m_refContentApi.RequestInformationRef.UserId) AndAlso (entry.Status = "O")) Then
                        m_refCatalog.TakeOwnershipForAdminCheckIn(m_intId, m_refContentApi.RequestInformationRef.UserId, Nothing)
                    End If

                    m_refCatalog.UpdateAndCheckIn(entry)

                Case Else

                    If (Not (IsNothing(Request.QueryString("asset_assetfilename")))) Then
                        AssetFileName = Request.QueryString("asset_assetfilename")
                    End If
                    Dim contData As ContentData = m_refContentApi.GetContentById(m_intId, ContentAPI.ContentResultType.Staged)
                    If ((contData IsNot Nothing) AndAlso (contData.UserId <> m_refContentApi.RequestInformationRef.UserId) AndAlso (contData.Status = "O")) Then
                        m_refContent.TakeOwnershipForAdminCheckIn(m_intId)
                    End If

                    If (Not (IsNothing(Request.QueryString("content_type")))) AndAlso (Int32.TryParse(Request.QueryString("content_type"), contentType)) AndAlso (Ektron.Cms.Common.EkConstants.IsAssetContentType(contentType)) Then
                        Dim cEditData As ContentEditData = m_refContentApi.GetContentForEditing(m_intId)
                        cEditData.FileChanged = False
                        m_refContentApi.SaveContent(cEditData)
                    End If

                    m_refContent.CheckIn(m_intId, AssetFileName)

            End Select

            strCallBackPage = m_refStyle.getCallBackupPage("content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId)
            Response.Redirect(strCallBackPage, False)

        Catch ex As Exception
            Utilities.ShowError(Server.UrlEncode(ex.Message))
        End Try
    End Sub
#End Region
#Region "ACTION - Submit"
    Private Sub Process_DoSubmit()
        Dim strCallBackPage As String = ""
        Dim m_refTask As Ektron.Cms.Content.EkTask
        Dim settings_data As SettingsData
        Dim strTaskName As String = ""
        Dim IsAlreadyCreated As Boolean = False
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim m_intId As Long
        Dim m_refStyle As New StyleHelper
        'Dim m_intContentLanguage As Integer
        Dim CurrentUserId As Long
        Dim contentType As Long = 0
        Try
            m_intId = Request.QueryString("id")
            m_refContent = m_refContentApi.EkContentRef
            CurrentUserId = m_refContentApi.UserId
            m_refTask = m_refContentApi.EkTaskRef
            If (Convert.ToString(m_intId) <> "") Then
                settings_data = (New SiteAPI).GetSiteVariables(CurrentUserId)
                contentType = m_refContent.GetContentType(m_intId)

                Select Case contentType
                    Case Ektron.Cms.Common.EkConstants.CMSContentType_CatalogEntry

                        m_refCatalog = New Ektron.Cms.Commerce.CatalogEntry(m_refContentApi.RequestInformationRef)
                        Dim entry As Ektron.Cms.Commerce.EntryData = m_refCatalog.GetItemEdit(m_intId, m_refContentApi.RequestInformationRef.ContentLanguage, False)
                        m_refCatalog.Publish(entry, Nothing)

                    Case Else

                        Dim PreapprovalGroupID As Long
                        Dim cPreApproval As New Collection
                        cPreApproval = m_refContent.GetFolderPreapprovalGroup(Request.QueryString("fldid"))
                        PreapprovalGroupID = CLng(cPreApproval("UserGroupID"))

                        If PreapprovalGroupID > 0 Then
                            If ContentLanguage = ALL_CONTENT_LANGUAGES Then '1 removed with ALL_CONTENT_LANGUAGES
                                strTaskName = Request.Form("content_title") & m_intId & "_Task"
                            Else
                                strTaskName = Request.Form("content_title") & m_intId & "_Task" & ContentLanguage
                            End If
                            m_refTask.ContentLanguage = ContentLanguage
                            m_refTask.LanguageID = ContentLanguage
                            IsAlreadyCreated = m_refTask.IsTaskAlreadyCreated(m_intId)
                            If IsAlreadyCreated = False Then
                                m_refTask.LanguageID = ContentLanguage
                                m_refTask.TaskTitle = strTaskName ' Task name would be contentname + content id + _Task
                                m_refTask.AssignToUserGroupID = PreapprovalGroupID  'Assigned to group defined by gtTaskAssignGroup
                                m_refTask.AssignedByUserID = CurrentUserId 'Assigned by person creating the task
                                m_refTask.State = 1 'Not started
                                m_refTask.ContentID = m_intId 'Content m_intId of the content being created
                                m_refTask.Priority = 2 'Normal
                                m_refTask.CreatedByUserID = CurrentUserId 'If task hops this will always be created user
                                m_refTask.AddTask()
                                m_refContent.SetContentState(m_intId, "T")
                            Else
                                m_refContent.SubmitForPublicationv2_0(m_intId, Request.QueryString("fldid"))
                            End If
                        Else
                            m_refContent.SubmitForPublicationv2_0(m_intId, Request.QueryString("fldid"))
                        End If

                End Select

            End If
            strCallBackPage = m_refStyle.getCallBackupPage("content.aspx?LangType=" & ContentLanguage & "&action=View&id=" & m_intId & "&fldid=" & Request.QueryString("fldid"))
            Response.Redirect(strCallBackPage, False)
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region
#Region "ACTION - DoDeleteFolder"
    Private Sub Process_DoDeleteFolder()
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim m_intContentLanguage As Integer
        Dim CurrentUserId As Long
        Dim FolderPath As String = ""
        Dim pagedata As New Collection
        Dim parentid As Long = 0

        m_intContentLanguage = m_refContentApi.ContentLanguage
        m_refContent = m_refContentApi.EkContentRef
        CurrentUserId = m_refContentApi.UserId

        If (String.IsNullOrEmpty(Request.QueryString("ParentID"))) Then
            Long.TryParse(Request.QueryString("id"), parentid)
            If (parentid > 0) Then
                parentid = m_refContent.GetParentIdByFolderId(parentid)
            End If
        Else
            Long.TryParse(Request.QueryString("ParentID"), parentid)
        End If

        pagedata.Add(Request.QueryString("id"), "FolderID")
        FolderPath = m_refContent.GetFolderPath(Request.QueryString("id"))
        If (Len(FolderPath) > 0) Then
            If (Right(FolderPath, 1) = "\") Then
                FolderPath = Right(FolderPath, Len(FolderPath) - 1)
            End If
            '
            ' Strip off the current folder name from the path:
            Dim Pos As Integer
            Pos = InStrRev(FolderPath, "\")
            If (Pos > 0) Then
                FolderPath = Left(FolderPath, Pos - 1)
            End If
            ' Escape backslashes:
            FolderPath = Replace(FolderPath, "\", "\\")
        End If

        CheckFolders(CLng(Request.QueryString("id")))
        If MasterLayouts <> String.Empty Then
            Utilities.ShowError(m_refMsg.GetMessage("com: cannot delete folder with master layout") & " " & MasterLayouts)
        Else
            m_refContent.DeleteContentFolderv2_0(pagedata)
            If (m_refContent.RequestInformation.HttpsProtocol = "on" AndAlso _
                        m_refContent.RequestInformation.CommerceSettings.ComplianceMode = True AndAlso _
                        HttpContext.Current IsNot Nothing AndAlso _
                        HttpContext.Current.Session IsNot Nothing AndAlso _
                        HttpContext.Current.Session("ecmComplianceRequired")) Then
                Response.Redirect("content.aspx?LangType=" & m_intContentLanguage & "&action=ReestablishSession&id=" & parentid, False)
            Else
                Response.Redirect("content.aspx?LangType=" & m_intContentLanguage & "&action=ViewContentByCategory&id=" & parentid & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
            End If
        End If
    End Sub
    Private Sub Process_DoReestablishSession()
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim m_intContentLanguage As Integer
        Dim CurrentUserId As Long
        Dim FolderPath As String = ""
        Dim pagedata As New Collection
        Dim parentid As Long = 0

        m_intContentLanguage = m_refContentApi.ContentLanguage
        m_refContent = m_refContentApi.EkContentRef
        CurrentUserId = m_refContentApi.UserId

        FolderPath = m_refContent.GetFolderPath(Request.QueryString("id"))
        If (Len(FolderPath) > 0) Then
            If (Right(FolderPath, 1) = "\") Then
                FolderPath = Right(FolderPath, Len(FolderPath) - 1)
            End If
            FolderPath = Replace(FolderPath, "\", "\\")
        End If
        m_intContentLanguage = m_refContentApi.ContentLanguage
        Response.Redirect("content.aspx?LangType=" & m_intContentLanguage & "&action=ViewContentByCategory&id=" & parentid & "&reloadtrees=Forms,Content,Library&TreeNav=" & FolderPath, False)
    End Sub
    Private Sub CheckFolders(ByVal FolderID As Long)
        Dim contentApi As New Ektron.Cms.ContentAPI
        CheckForMasterLayout(FolderID)
        Dim folderData As Ektron.Cms.FolderData() = contentApi.GetChildFolders(FolderID, True, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Id)
        If folderData IsNot Nothing AndAlso folderData.Length > 0 Then
            Dim i As Integer = 0
            For i = 0 To folderData.Length - 1
                CheckFolders(folderData(i).Id)
            Next
        End If
    End Sub
    Private Sub CheckForMasterLayout(ByVal FolderID As Long)
        Dim contentApi As New Ektron.Cms.ContentAPI
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim ekContentColl As Ektron.Cms.Common.EkContentCol
        m_refContent = m_refContentApi.EkContentRef
        Dim coll As New Collection
        coll.Add(FolderID, "FolderID")
        coll.Add(Ektron.Cms.Common.EkEnumeration.ContentOrderBy.Id, "OrderBy")
        coll.Add(Ektron.Cms.Common.EkEnumeration.CMSContentType.Content, "ContentType")
        coll.Add(Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData, "ContentSubType")
        ekContentColl = Nothing
        ekContentColl = m_refContent.GetAllViewableChildContentInfoV5_0(coll, 1, 10000, 1)
        If ekContentColl IsNot Nothing AndAlso ekContentColl.Count > 0 Then
            Dim j As Integer = 0
            For j = 0 To ekContentColl.Count - 1
                Dim masterLayoutFolder As Ektron.Cms.FolderData = m_refContent.GetFolderById(FolderID)
                If MasterLayouts = String.Empty Then
                    MasterLayouts = "Template Name: " & masterLayoutFolder.NameWithPath & ekContentColl.Item(j).Title
                Else
                    MasterLayouts += ", Template Name: " & masterLayoutFolder.NameWithPath & ekContentColl.Item(j).Title
                End If
            Next
        End If
    End Sub
#End Region
#Region "ACTION - submitDelContAction"
    Private Sub Process_DeleteContent()
        Dim strCallBackPage As String = ""
        Dim m_intId As Long = -1
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim m_refStyle As New StyleHelper

        m_intId = Request.QueryString("delete_id")
        Try
            m_refContent = m_refContentApi.EkContentRef
            m_intFolderId = Request.QueryString("folder_id")

            Dim subtype As Ektron.Cms.Common.EkEnumeration.CMSContentSubtype = m_refContent.GetContentSubType(m_intId)
            If (subtype = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData) Then
                Dim tm As New PageBuilder.TemplateModel()
                Dim td As TemplateData = tm.FindByMasterLayoutID(m_intId)
                If (td IsNot Nothing And td.Id > 0) Then
                    Dim folders As Long() = m_refContent.GetTemplateDefaultFolderUsage(td.Id)
                    Dim contentBlockInfo As Collection = m_refContent.GetTemplateContentBlockUsage(td.Id)
                    If (folders.Length > 0 Or contentBlockInfo.Count > 0) Then
                        Dim message As New StringBuilder()
                        message.Append("This master layout cannot be deleted until it is unassigned from the following folders: ")
                        If (folders.Length > 0) Then
                            For i As Integer = 0 To folders.Length - 1
                                message.Append(m_refContent.GetFolderById(folders(i)).NameWithPath() & ", ")
                            Next
                        End If
                        message.Append("and the following layouts: ")
                        If (contentBlockInfo.Count > 0) Then
                            Dim col As Collection
                            For Each col In contentBlockInfo
                                Dim content_data As ContentData = m_refContentApi.EkContentRef.GetContentById(col("content_id"))
                                Dim folderpath As String = m_refContent.GetFolderById(content_data.FolderId).NameWithPath()
                                message.Append(folderpath & content_data.Title & ": (id=" & content_data.Id & ", lang=" & content_data.LanguageId & "), ")
                            Next
                        End If
                        Throw New Exception(message.ToString())
                    End If
                End If
            End If

            m_refContent.SubmitForDeletev2_0(m_intId, m_intFolderId)
            If (Request.QueryString("page") = "webpage") Then
                m_strReloadJS += "top.opener.location.reload(true);"
                m_strReloadJS += "top.close();"
                JS.RegisterJSBlock(Me, m_strReloadJS.ToString(), "EktronDeleteWorkareaClose")
            Else
                strCallBackPage = m_refStyle.getCallBackupPage("content.aspx?LangType=" & ContentLanguage & "&action=ViewContentByCategory&id=" & m_intFolderId)
                Response.Redirect(strCallBackPage, False)
            End If
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub
#End Region
#Region "ACTION - DoDeletePermission"
    Private Sub Process_DoDeletePermission()
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Try
            m_refContent = m_refContentApi.EkContentRef
            Dim pagedata As New Collection
            If (Request.QueryString("type") = "folder") Then
                pagedata.Add(Request.QueryString("id"), "FolderID")
                pagedata.Add("", "ContentID")
            Else
                pagedata.Add(Request.QueryString("id"), "ContentID")
                pagedata.Add("", "FolderID")
            End If
            If (Request.QueryString("base") = "group") Then
                pagedata.Add(Request.QueryString("PermID"), "UserGroupID")
                pagedata.Add("", "UserID")
                m_refContent.DeleteItemPermissionv2_0(pagedata)
            ElseIf (Request.QueryString("base") = "user") Then
                pagedata.Add(Request.QueryString("PermID"), "UserID")
                pagedata.Add("", "UserGroupID")
                m_refContent.DeleteItemPermissionv2_0(pagedata)
            Else
                Process_DODeleteMultiplePermission()
            End If
            Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewPermissions&id=" & Request.QueryString("id") & "&type=" & Request.QueryString("type") & "&membership=" & Request.QueryString("membership"), False)
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub

    Private Sub Process_DODeleteMultiplePermission()
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim userID() As String = Nothing
        Dim groupID() As String = Nothing
        Dim _userIDs As Integer
        Dim _grpIDs As Integer
        Dim i As Integer = 0
        Dim j As Integer = 0

        m_refContent = m_refContentApi.EkContentRef

        If Request.QueryString("userIds") <> "" Then
            userID = Request.QueryString("userIds").Split(",")
            _userIDs = userID.Length
            For i = 0 To _userIDs - 1
                Dim userData As New Collection
                If (Request.QueryString("type") = "folder") Then
                    userData.Add(Request.QueryString("id"), "FolderID")
                    userData.Add("", "ContentID")
                Else
                    userData.Add(Request.QueryString("id"), "ContentID")
                    userData.Add("", "FolderID")
                End If
                userData.Add(userID(i), "UserID")
                userData.Add("", "UserGroupID")
                m_refContent.DeleteItemPermissionv2_0(userData)
            Next
        End If
        If Request.QueryString("groupIds") <> "" Then
            groupID = Request.QueryString("groupIds").Split(",")
            _grpIDs = groupID.Length
            For j = 0 To _grpIDs - 1
                Dim groupData As New Collection
                If (Request.QueryString("type") = "folder") Then
                    groupData.Add(Request.QueryString("id"), "FolderID")
                    groupData.Add("", "ContentID")
                Else
                    groupData.Add(Request.QueryString("id"), "ContentID")
                    groupData.Add("", "FolderID")
                End If
                groupData.Add(groupID(j), "UserGroupID")
                groupData.Add("", "UserID")
                m_refContent.DeleteItemPermissionv2_0(groupData)
            Next
        End If
        
    End Sub
#End Region
#Region "ACTION - DoItemInheritance (Disable/Enable)"
    Private Sub Process_DoItemInheritance(ByVal value As Flag)
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Try
            m_refContent = m_refContentApi.EkContentRef
            Dim pagedata As New Collection
            pagedata.Add(Request.QueryString("id"), "ItemID")
            pagedata.Add(Request.QueryString("type"), "RequestType")
            If (value = Flag.Disable) Then
                m_refContent.DisableItemInheritancev2_0(pagedata)
            Else
                m_refContent.EnableItemInheritancev2_0(pagedata)
            End If
            If m_strMembership = "true" Then
                Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewPermissions&id=" & Request.QueryString("id") & "&type=" & Request.QueryString("type") & "&membership=" & m_strMembership, False)
            Else
                Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewPermissions&id=" & Request.QueryString("id") & "&type=" & Request.QueryString("type"), False)
            End If
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub
#End Region
#Region "ACTION - DoItemPrivateSetting (Disable/Enable)"
    Private Sub Process_DoItemPrivateSetting(ByVal value As Flag)
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Try
            m_refContent = m_refContentApi.EkContentRef
            Dim pagedata As New Collection
            pagedata.Add(Request.QueryString("id"), "ItemID")
            pagedata.Add(Request.QueryString("type"), "RequestType")
            If (value = Flag.Disable) Then
                m_refContent.DisableItemPrivateSettingv2_0(pagedata)
            Else
                m_refContent.EnableItemPrivateSettingv2_0(pagedata)
            End If
            If m_strMembership = "true" Then
                Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewPermissions&id=" & Request.QueryString("id") & "&type=" & Request.QueryString("type") & "&membership=" & m_strMembership, False)
            Else
                Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewPermissions&id=" & Request.QueryString("id") & "&type=" & Request.QueryString("type"), False)
            End If
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub
#End Region
#Region "ACTION - DoAddItemApproval"
    Private Sub Process_DoAddItemApproval()
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Try
            m_refContent = m_refContentApi.EkContentRef
            Dim pagedata As New Collection
            If (Request.QueryString("type") = "folder") Then
                pagedata.Add(Request.QueryString("id"), "FolderID")
                pagedata.Add("", "ContentID")
            Else
                pagedata.Add(Request.QueryString("id"), "ContentID")
                pagedata.Add("", "FolderID")
            End If
            If (Request.QueryString("base") = "user") Then
                pagedata.Add(Request.QueryString("item_id"), "UserID")
                pagedata.Add("", "UserGroupID")
            Else
                pagedata.Add(Request.QueryString("item_id"), "UserGroupID")
                pagedata.Add("", "UserID")
            End If
            m_refContent.AddItemApprovalv2_0(pagedata)
            Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewApprovals&id=" & Request.QueryString("id") & "&type=" & Request.QueryString("type"), False)
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub
#End Region
#Region "ACTION - DeleteItemApproval"
    Private Sub Process_DoDeleteItemApproval()
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Try
            m_refContent = m_refContentApi.EkContentRef
            Dim pagedata As New Collection
            If (Request.QueryString("type") = "folder") Then
                pagedata.Add(Request.QueryString("id"), "FolderID")
                pagedata.Add("", "ContentID")
            Else
                pagedata.Add(Request.QueryString("id"), "ContentID")
                pagedata.Add("", "FolderID")
            End If
            If (Request.QueryString("base") = "user") Then
                pagedata.Add(Request.QueryString("item_id"), "UserID")
                pagedata.Add("", "UserGroupID")
            Else
                pagedata.Add(Request.QueryString("item_id"), "UserGroupID")
                pagedata.Add("", "UserID")
            End If
            m_refContent.DeleteItemApprovalv2_0(pagedata)
            Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewApprovals&id=" & Request.QueryString("id") & "&type=" & Request.QueryString("type"), False)
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region
#Region "ACTION - WorkOffLine"
    Private Sub Process_WorkOffLine()
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Dim m_intId As Long
        Dim ret As Boolean
        Try
            m_refContent = m_refContentApi.EkContentRef
            m_intId = Request.QueryString("id")
            If (Convert.ToString(m_intId) <> "") Then
                ret = m_refContent.CheckContentOutv2_0(m_intId)
            End If
            Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewContentByCategory&id=" & Request.QueryString("folder_id"), False)
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region
#Region "ACTION - ApproveContent"
    Private Sub Do_ApproveContent()
        Dim m_intId As Long
        Dim ret As Boolean
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Try
            m_refContent = m_refContentApi.EkContentRef()
            m_intId = CLng(Request.QueryString("id"))
            ret = m_refContent.Approvev2_0(m_intId)
            Response.Redirect("content.aspx?action=viewcontentbycategory&id=" & Request.QueryString("fldid"), False)
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region
#Region "ACTION - DeclineContent"
    Private Sub Do_DeclineContent()
        Dim m_intId As Long
        Dim ret As Boolean
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Try
            m_intId = Request.QueryString("id")
            m_refContent = m_refContentApi.EkContentRef()
            Dim reason As String = ""
            If (Not Request.QueryString("comment") Is Nothing) Then
                reason = Request.QueryString("comment")
            End If
            ret = m_refContent.DeclineApproval2_0(m_intId, reason)
            Response.Redirect("content.aspx?action=viewcontentbycategory&id=" & Request.QueryString("fldid"), False)
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region
#Region "ACTION - RestoreInheritance"
    Private Sub Do_RestoreInheritance()
        Dim m_intId As Long
        Dim ret As Boolean
        Dim m_refContent As Ektron.Cms.Content.EkContent
        Try
            m_intId = Request.QueryString("id")
            m_refContent = m_refContentApi.EkContentRef()
            ret = m_refContent.DeleteSubscriptionsForContentinFolder(m_intId)
            Response.Redirect("content.aspx?LangType=" & ContentLanguage & "&action=ViewFolder&id=" & m_intId, False)
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region
#End Region

#Region "JS, CSS"

    Public Sub RegisterJs()
		Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronContextMenuJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS)
		Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS)
		Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)
		Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronStringJS)
		Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronDmsMenuJS)
		Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronDetermineOfficeJS)
		Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS)
		Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronDnRJS)
		Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronScrollToJS)
		Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/jfunct.js", "EktronJfuncJs")
		Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/toolbar_roll.js", "EktronToolbarRollJs")
		Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/platforminfo.js", "EktronPlatformInfoJs")
		Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/designformentry.js", "EktronDesignFormEntryJs")
		Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayJs")
		Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/sync/ektron.sync.js", "EktronSyncJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/wamenu/includes/com.ektron.ui.menu.js", "EktronWamenuJs")
        Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/ektron.workarea.contextmenus.js", "EktronWorkareaContextMenusJs")
        Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/Ektron.WorkArea.Content.MoveCopy.js", "EktronWorkareaContentMovieCopyJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/ektron.workarea.contextmenus.cutcopy.js", "EktronWorkareaContextMenusCutCopyJs")
        Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/controls/permission/permissionsCheckHandler.ashx?action=getPermissionJsClass", "EktronPermissionJS")
        System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.GetType(), "dmsMenuPoundName", ("$ektron().one(""EktronDMSMenuReady"", function(){Ektron.DMSMenu.andSymbolFilename = '") + m_refMsg.GetMessage("dmsmenu and symbol in filename warning") & "';});", True)
        Ektron.Cms.api.js.registerjs(Me, Me.ApplicationPath & "/java/ektron.workarea.contextmenus.trees.js", "EktronWorkareaContextmenusTreeJS")

    End Sub

    Public Sub RegisterCss()
		Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
		Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuCss)
		Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)
		Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuIE6Css, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE6)
        Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/sync/sync.css", "EktronSyncCss")
        Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/sync/sync-IE.css", "EktronSyncIECss", Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
		Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/csslib/box.css", "EktronBoxCss")
		Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/csslib/tables/tableutil.css", "EktronTableUtilCss")
		Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/Tree/css/com.ektron.ui.tree.css", "EktronTreeCss")
		Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/wamenu/css/com.ektron.ui.menu.css", "EktronWamenuCss")
		Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronContextMenuCss)
        SetJSVariable()
    End Sub

    Protected Sub AssignTextValues()
        ' assign the various resource text strings
        folder_jslanguage.Text = ContentLanguage

        jsAppPath.Text = Me.ApplicationPath.ToString()
        contentContextCutContent.Text = m_refMsg.GetMessage("lbl cut")
        contentContextCopyContent.Text = m_refMsg.GetMessage("lbl copy")
        contentContextAssignContentToTaxonomy.Text = m_refMsg.GetMessage("lbl assign items to taxonomy")
        contentContextAssignContentToCollection.Text = m_refMsg.GetMessage("add collection items title")
        contentContextAssignContentToMenu.Text = m_refMsg.GetMessage("add items to menu")
        contentContextDeleteContent.Text = m_refMsg.GetMessage("btn delete")
        jsIsFolderAdmin.Text = IsFolderAdmin()
        jsConfirmDelete.Text = m_refMsg.GetMessage("js:delete selected content block")
    End Sub
    Private Function IsFolderAdmin() As Boolean
        Dim _isFolderAdmin As Boolean = False
        m_intFolderId = Request.QueryString("id")
        _isFolderAdmin = m_refContentApi.IsARoleMemberForFolder(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminFolderUsers, m_intFolderId, m_refContentApi.UserId, False)
        Return _isFolderAdmin
    End Function

    Protected Sub SetJSVariable()
        jsAlertCheckedOutSelected.Text = m_refMsg.GetMessage("js:alert checked out item selected")
        jsAlertCheckedOutSelectedCopy.Text = m_refMsg.GetMessage("js:alert checked out item selected copy")
        jsAlertSelectOneContent.Text = m_refMsg.GetMessage("js:alert select one item")
        jsAlertSelectNotApprovedAll.Text = m_refMsg.GetMessage("js: alert no approved item selected")
        jsConfirmMultiLingual.Text = m_refMsg.GetMessage("js: confirm multilingual")
        selMultLangOption.Text = m_refMsg.GetMessage("select multiple language")
        lblConfirmMoveContent.Text = m_refMsg.GetMessage("js:confirm move content")
        availableLanguages.Text = m_refMsg.GetMessage("generic all")
        selectedLanguages.Text = m_refMsg.GetMessage("generic selected")
        cancelButton.Text = m_refMsg.GetMessage("generic cancel")
        moveCancelButton.Text = cancelButton.Text
        moveContentConfirmTitle.Text = m_refMsg.GetMessage("move content confirm title")
        ltrOK.Text = m_refMsg.GetMessage("continue")
        jsAppPathMultiLang.Text = Me.ApplicationPath
        ltrPaste.Text = m_refMsg.GetMessage("generic paste")
    End Sub
#End Region

End Class