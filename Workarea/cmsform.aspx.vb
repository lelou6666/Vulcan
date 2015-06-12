Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Partial Class cmsform
    Inherits System.Web.UI.Page

    Protected pagedata As Collection
    Protected PageAction As String = ""
    Protected m_refMsg As EkMessageHelper
    Protected m_refStyle As New StyleHelper
    Protected m_refContentApi As New ContentAPI
    Protected ContentLanguage As Integer = -1
    Protected m_intFolderId As Long = -1
    Protected m_intFormId As Long = -1
    Protected m_bAjaxTree As Boolean = False
    Protected m_strStyleSheetJS As String = ""
    Protected m_strReloadJS As String = ""
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected m_jsResources As Sync_jsResources

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        ' resource text string tokens
        m_refMsg = m_refContentApi.EkMsgRef

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

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1
        AppImgPath = m_refContentApi.AppImgPath
        AppPath = m_refContentApi.AppPath

        Me.RegisterCSS()
        Me.RegisterJS()

        ltr_title.Text = m_refMsg.GetMessage("lbl cmsform")

        If (m_refContentApi.GetCookieValue("user_id") = 0) Then
            If Not (Request.QueryString("callerpage") Is Nothing) Then
                Session("RedirectLnk") = "cmsform.aspx?" & Request.QueryString.ToString()
            End If
            Response.Redirect("login.aspx?fromLnkPg=1", False)
            Exit Sub
        End If
        If (m_refContentApi.RequestInformationRef.IsMembershipUser = 1) Then
            Utilities.ShowError(m_refMsg.GetMessage("com: user does not have permission"))
            Exit Sub
        End If

        If (m_refContentApi.TreeModel = 1) Then
            m_bAjaxTree = True
        End If
        If (Not (Request.QueryString("action") Is Nothing)) Then
            PageAction = Convert.ToString(Request.QueryString("action")).ToLower.Trim
        End If
        If Not (Request.QueryString("folder_id") Is Nothing) AndAlso Trim(Request.QueryString("folder_id")) <> "" Then
            m_intFolderId = Convert.ToInt64(Request.QueryString("folder_id"))
		ElseIf (IsNumeric(Request.Form("content_folder"))) Then
            m_intFolderId = CLng(Request.Form("content_folder"))
		Else
            m_intFolderId = CLng(Request.Form(folder_id.UniqueID))
		End If
        If (Not (Request.QueryString("form_id") Is Nothing)) AndAlso Trim(Request.QueryString("form_id")) <> "" Then
            If (Request.QueryString("form_id") <> "") Then
                m_intFormId = Convert.ToInt64(Request.QueryString("form_id"))
            End If
        End If
        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            If (Request.QueryString("LangType") <> "") Then
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
        jsContentLanguage.Text = ContentLanguage
        txtDefaultContentLanguage.Text = m_refContentApi.DefaultContentLanguage
        m_strStyleSheetJS = m_refStyle.GetClientScript()
        jsAction.Text = PageAction
        jsFormId.Text = m_intFormId
		vFolderId.Text = m_intFolderId
		folder_id.Value = m_intFolderId
        txtEnableMultilingual.Text = m_refContentApi.EnableMultilingual
    End Sub
    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
        Dim bCompleted As Boolean
        Try
            If (Request.QueryString("reloadtrees") <> "") Then
                m_strReloadJS = ReloadClientScript()
            End If
            Select Case PageAction
                Case "dodelete", "submitdelcontaction"
                    Process_DoDelete()
                    'The following action moved to content.aspx
                    'Case "viewallformsbyfolderid", "viewarchivefrombycategory", "viewallforms"
                    '    UniqueLiteral.Text = "form"
                    '    Dim m_viewformscategory As viewformscategory
                    '    m_viewformscategory = CType(LoadControl("controls/forms/viewformscategory.ascx"), viewformscategory)
                    '    m_viewformscategory.ID = "form"
                    '    DataHolder.Controls.Add(m_viewformscategory)
                    '    m_viewformscategory.ViewFormsByFolderId()
                Case "viewform"
                    UniqueLiteral.Text = "form"
                    Dim m_viewform As viewform
                    m_viewform = CType(LoadControl("controls/forms/viewform.ascx"), viewform)
                    m_viewform.ID = "form"
                    DataHolder.Controls.Add(m_viewform)
                    m_viewform.ViewForm()
                Case "addform"
                    If (Not (Page.IsPostBack)) Then
                        If ((Request.QueryString("back_LangType") = "") Or (Request.QueryString("form_id") = "")) Then
                            Dim ucNewFormWizard As newformwizard
                            ucNewFormWizard = CType(LoadControl("controls/forms/newformwizard.ascx"), newformwizard)
                            ucNewFormWizard.ID = "ProgressSteps"
                            DataHolder.Controls.Add(ucNewFormWizard)
                        End If
                    End If
                    UniqueLiteral.Text = "form"
                    Dim m_editform As editform
                    m_editform = CType(LoadControl("controls/forms/editform.ascx"), editform)
                    m_editform.ID = "form"
                    DataHolder.Controls.Add(m_editform)
                    bCompleted = m_editform.AddForm
                Case "editform"
                    UniqueLiteral.Text = "form"
                    Dim m_editform As editform
                    m_editform = CType(LoadControl("controls/forms/editform.ascx"), editform)
                    m_editform.ID = "form"
                    DataHolder.Controls.Add(m_editform)
                    bCompleted = m_editform.EditForm
            End Select
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Function ReloadClientScript() As String
        Dim result As New System.Text.StringBuilder
        result.Append("<script type=""text/javascript"" src=""java/QueryStringParser.js""></SCRIPT>" & vbCrLf)
        result.Append("<script type=""text/javascript"">" & vbCrLf)
        result.Append("<!--" & vbCrLf)
        result.Append("	// If reloadtrees paramter exists, reload selected navigation trees:" & vbCrLf)
        result.Append("	var m_reloadTrees = """ & Request.QueryString("reloadtrees") & """;" & vbCrLf)
        result.Append("	top.ReloadTrees(m_reloadTrees);" & vbCrLf)
        result.Append("	self.location.href=""" & Request.ServerVariables("path_info") & "?" & Replace(Request.ServerVariables("query_string"), "&reloadtrees=" & Request.QueryString("reloadtrees"), "") & """;" & vbCrLf)
        result.Append("	// If TreeNav parameters exist, ensure the desired folders are opened:" & vbCrLf)
        result.Append("	var strTreeNav = """ & Request.QueryString("TreeNav") & """;" & vbCrLf)
        result.Append("	if (strTreeNav != null) {" & vbCrLf)
        result.Append("		strTreeNav = strTreeNav.replace(/\\\\/g,""\\"");" & vbCrLf)
        result.Append("		top.TreeNavigation(""FormsTree"", strTreeNav);" & vbCrLf)
        result.Append("	}" & vbCrLf)
        result.Append("//-->" & vbCrLf)
        result.Append("</script>" & vbCrLf)
        Return (result.ToString)
    End Function
    Private Sub Process_DoDelete()
        Try
            Dim m_refModule As Ektron.Cms.Modules.EkModule
            If Request.QueryString("form_id") <> "" Then
                m_refModule = m_refContentApi.EkModuleRef
                m_refModule.DeleteFormByID(Request.QueryString("form_id")) 'ret
                Response.Redirect("content.aspx?id=" & Request.Form("frm_folder_id") & "&LangType=" & ContentLanguage & "&action=ViewContentByCategory", False)
            Else
                Response.Redirect("reterror.aspx?info=" & m_refMsg.GetMessage("msg form id passed"), False)
            End If
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message), False)
        End Try
    End Sub
    Private Sub RegisterCSS()
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronModalCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)
        Ektron.Cms.API.Css.RegisterCss(Me, m_refContentApi.ApplicationPath & "/sync/sync.css", "EktronSyncCss")
    End Sub
    Private Sub RegisterJS()
        'API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronModalJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "/sync/ektron.sync.js", "EktronSyncJS")
    End Sub
End Class