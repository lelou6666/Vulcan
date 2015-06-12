Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common.EkConstants
Imports System.Collections.Generic

Partial Class ViewApprovalList
    Inherits System.Web.UI.UserControl

#Region "Members"

    Private _CommonApi As New CommonApi
    Private _EkContent As EkContent
    Private _MessageHelper As Ektron.Cms.Common.EkMessageHelper
    Private _Folder As String = ""
    Private _ApprovalsCollection As Collection
    Private _ApprovedList As ArrayList
    Protected _StyleHelper As New StyleHelper
    Protected _EnableMultilingual As Integer = 0
    Protected _ContentLanguage As Integer = -1
    Protected _AssetInfoData As AssetInfoData()
    Protected _ContentApi As New ContentAPI
    Protected _ContentType As Integer = 0

#End Region

#Region "Properties"

    Public Property MultilingualEnabled() As Integer
        Get
            Return _EnableMultilingual
        End Get
        Set(ByVal Value As Integer)
            _EnableMultilingual = Value
        End Set
    End Property

    Public Property ContentLang() As Integer
        Get
            Return _ContentLanguage
        End Get
        Set(ByVal Value As Integer)
            _ContentLanguage = Value
        End Set
    End Property

#End Region

#Region "Events"

    Public Sub New()

        _CommonApi = New CommonApi()
        _ApprovedList = New ArrayList()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        _CommonApi.ContentLanguage = _ContentLanguage
        _EkContent = _CommonApi.EkContentRef
        _MessageHelper = _CommonApi.EkMsgRef

        'register js/css
        RegisterResources()

        _Folder = IIf(String.IsNullOrEmpty(Request.QueryString("id")), String.Empty, Request.QueryString("id"))
        Me.litAction.Text = Request.QueryString("action")

        'set hidden input field to submit button unique id - used in js submit (fires off lbSubmit_Click event)
        lbSubmitId.Value = Me.lbSubmit.UniqueID
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim ContentTypeUrlParam As String = Ektron.Cms.Common.EkConstants.ContentTypeUrlParam
        If Not (Request.QueryString(ContentTypeUrlParam) Is Nothing) AndAlso (Request.QueryString(ContentTypeUrlParam) <> "") Then
            If IsNumeric(Request.QueryString(ContentTypeUrlParam)) Then
                _ContentType = Request.QueryString(ContentTypeUrlParam)
                _ContentApi.SetCookieValue(ContentTypeUrlParam, _ContentType)
            End If
        ElseIf Ektron.cms.commonApi.GetEcmCookie()(ContentTypeUrlParam) <> "" Then
            If IsNumeric(Ektron.cms.commonApi.GetEcmCookie()(ContentTypeUrlParam)) Then
                _ContentType = CLng(Ektron.cms.commonApi.GetEcmCookie()(ContentTypeUrlParam))
            End If
        End If

        If (Page.IsPostBack) Then
            'ViewApprovalListActon()
            'GetSavedApprovedItems()
            GetApprovedItems()

            'lbSubmit click event not firing for some reason - sniff out postback
            'and if it's from lbSubmit, manually fire click event
            If (Request.Form("__EVENTTARGET") = Me.lbSubmit.UniqueID) Then
                lbSubmit_Click(sender, e)
            End If
        End If

        'deserialize approved items
        GetSavedApprovedItems()
        ViewApprovalList()

        'set up datagrid
        Me.dgItemsNeedingApproval.PageSize = _CommonApi.RequestInformationRef.PagingSize
        Me.dgItemsNeedingApproval.DataSource = _ApprovalsCollection
        Me.dgItemsNeedingApproval.CurrentPageIndex = Me.ucPaging.SelectedPage
        Me.dgItemsNeedingApproval.DataBind()
        If Me.dgItemsNeedingApproval.Items.Count >= 1 Then
            Me.hdnNeedingApproval.Value = Me.dgItemsNeedingApproval.Items.Count
        Else
            Me.hdnNeedingApproval.Value = 0
        End If
        If Me.dgItemsNeedingApproval.PageCount > 1 Then
            Me.ucPaging.TotalPages = Me.dgItemsNeedingApproval.PageCount
            Me.ucPaging.CurrentPageIndex = Me.dgItemsNeedingApproval.CurrentPageIndex
        Else
            Me.ucPaging.Visible = False
        End If
    End Sub

    Public Sub dgItemsNeedingApproval_ItemDataBound(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)

        Select Case e.Item.ItemType
            Case ListItemType.Header
                'cell 0 - checkbox 
                'cell 1 - icon
                'cell 2 - title
                'cell 3 - request type
                'cell 4 - start date
                'cell 5 - date modified
                'cell 6 - submitted by
                'cell 7 - id
                'cell 8 - language
                'cell 9 - path
                CType(e.Item.Cells(2).FindControl("litTitleHeader"), Literal).Text = "Title"
                CType(e.Item.Cells(3).FindControl("litRequestTypeHeader"), Literal).Text = "Request Type"
                CType(e.Item.Cells(4).FindControl("litStartDateHeader"), Literal).Text = "Start Date"
                CType(e.Item.Cells(5).FindControl("litModifiedDateHeader"), Literal).Text = "Modified Date"
                CType(e.Item.Cells(6).FindControl("litSubmittedByHeader"), Literal).Text = "Submitted By"
                CType(e.Item.Cells(7).FindControl("litIdHeader"), Literal).Text = "ID"
                CType(e.Item.Cells(8).FindControl("litLanguageHeader"), Literal).Text = "Language"
                CType(e.Item.Cells(9).FindControl("litPathHeader"), Literal).Text = "Path"
            Case ListItemType.Item, ListItemType.AlternatingItem
                CType(e.Item.Cells(0).FindControl("cbApproval"), CheckBox).Checked = IsApproved(e.Item.DataItem("ContentID"), e.Item.DataItem("ContentLanguage"))
                CType(e.Item.Cells(0).FindControl("hdnId"), HiddenField).Value = e.Item.DataItem("ContentID")
                CType(e.Item.Cells(0).FindControl("hdnLanguageID"), HiddenField).Value = e.Item.DataItem("ContentLanguage")
                CType(e.Item.Cells(1).FindControl("imgContentIcon"), Image).AlternateText = e.Item.DataItem("ContentTitle")
                CType(e.Item.Cells(1).FindControl("imgContentIcon"), Image).Attributes.Add("title", e.Item.DataItem("ContentTitle"))
                CType(e.Item.Cells(1).FindControl("imgContentIcon"), Image).ImageUrl = GetImagePath(e.Item.DataItem)
                CType(e.Item.Cells(2).FindControl("aTitle"), HyperLink).NavigateUrl = GetItemLink(e.Item.DataItem)
                CType(e.Item.Cells(2).FindControl("aTitle"), HyperLink).Text = e.Item.DataItem("ContentTitle")
                CType(e.Item.Cells(2).FindControl("aTitle"), HyperLink).Attributes.Add("title", e.Item.DataItem("ContentTitle"))
                CType(e.Item.Cells(3).FindControl("spanRequestType"), HtmlGenericControl).Attributes.Add("style", GetRequestTypeColor(e.Item.DataItem))
                CType(e.Item.Cells(3).FindControl("spanRequestType"), HtmlGenericControl).InnerText = GetRequestTypeText(e.Item.DataItem)
                CType(e.Item.Cells(3).FindControl("litStartDateValue"), Literal).Text = GetStartDate(e.Item.DataItem)
                CType(e.Item.Cells(3).FindControl("litModifiedDateValue"), Literal).Text = e.Item.DataItem("DisplayLastEditDate")
                CType(e.Item.Cells(4).FindControl("litSubmittedByValue"), Literal).Text = e.Item.DataItem("SubmittedBy")
                CType(e.Item.Cells(5).FindControl("litIdValue"), Literal).Text = e.Item.DataItem("ContentID")
                CType(e.Item.Cells(6).FindControl("litLanguageValue"), Literal).Text = e.Item.DataItem("ContentLanguage")
                CType(e.Item.Cells(7).FindControl("aPathValue"), HyperLink).NavigateUrl = GetPathLink(e.Item.DataItem)
                CType(e.Item.Cells(7).FindControl("aPathValue"), HyperLink).Text = e.Item.DataItem("Path")
                CType(e.Item.Cells(7).FindControl("aPathValue"), HyperLink).Attributes.Add("title", e.Item.DataItem("Path"))
        End Select
    End Sub

    Public Sub lbSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'this link button control is a placeholder - this event fires when user clicks submit button in toolbar
        Dim key() As String
        For Each keySet As String In _ApprovedList
            key = keySet.Split("_")
            If key(0) <> String.Empty Then
                Me._CommonApi.ContentLanguage = key(1)
                If Request("__EVENTARGUMENT") = True Then
                    Me._EkContent.Approvev2_0(key(0))
                Else
                    Me._EkContent.DeclineApproval2_0(key(0))
                End If
            End If

        Next
    End Sub
  
#End Region

#Region "DataGrid Helpers"

    Private Sub GetApprovedItems()

        For Each dataGridItem As DataGridItem In Me.dgItemsNeedingApproval.Items
            If dataGridItem.ItemType = ListItemType.Item Or dataGridItem.ItemType = ListItemType.AlternatingItem Then

                'get .net controls from row
                Dim checkboxFieldId As String = CType(dataGridItem.Cells(0).FindControl("cbApproval"), CheckBox).UniqueID
                Dim contentidFieldId As String = CType(dataGridItem.Cells(0).FindControl("hdnId"), HiddenField).UniqueID
                Dim languageIdFieldId As String = CType(dataGridItem.Cells(0).FindControl("hdnLanguageID"), HiddenField).UniqueID

                Dim approved As Boolean = IIf(Request.Form(checkboxFieldId) = "on", True, False)
                Dim contentId As String = IIf(String.IsNullOrEmpty(Request.Form(contentidFieldId)), String.Empty, Request.Form(contentidFieldId))
                Dim contentLanguage As String = IIf(String.IsNullOrEmpty(Request.Form(languageIdFieldId)), String.Empty, Request.Form(languageIdFieldId))


                'set value - either add to list or remove from list
                If (approved) Then
                    _ApprovedList.Remove(contentId & "_" & contentLanguage)
                    _ApprovedList.Add(contentId & "_" & contentLanguage)
                Else
                    _ApprovedList.Remove(contentId & "_" & contentLanguage)
                End If
            End If
        Next

        'serialize to hidden field
        Dim approvedStringArray() As String = _ApprovedList.ToArray(GetType(String))
        Dim approvedJoinedString As String = IIf(approvedStringArray.Length = 0, String.Empty, Join(approvedStringArray, ","))
        Me.hdnApprovedItems.Value = approvedJoinedString

    End Sub
    Private Sub GetSavedApprovedItems()

        Dim serializedApprovedItems As String = IIf(String.IsNullOrEmpty(Request.Form(Me.hdnApprovedItems.UniqueID)), String.Empty, Request.Form(Me.hdnApprovedItems.UniqueID))
        Dim serializedApprovedItemsArray() As String = serializedApprovedItems.Split(",")
        For Each approvedItem As String In serializedApprovedItemsArray
            _ApprovedList.Add(approvedItem)
        Next

    End Sub
    Private Function IsApproved(ByVal id As String, ByVal languageId As String) As Boolean
        Return IIf(_ApprovedList.IndexOf(id & "_" & languageId) > -1, True, False)
    End Function
    Private Function GetImagePath(ByVal item As Collection) As String
        Dim imagePath As String
        If (item("ContentSubType") = Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData) Then
            imagePath = _CommonApi.AppPath & "images/UI/Icons/contentWireframeTemplate.png"
        ElseIf (item("ContentType") = 1 Or item("ContentType") = 2 Or item("ContentType") = 3) Then
            imagePath = _CommonApi.AppPath & "images/UI/Icons/contentHtml.png"
        ElseIf (item("ContentType") = 3333) Then
            imagePath = _CommonApi.AppPath & "images/UI/Icons/brick.png"
        Else
            imagePath = item("Icon")
        End If

        Return imagePath
    End Function
    Private Function GetItemLink(ByVal dataItem As Object) As String
        Return _ContentApi.ApplicationPath & "/approval.aspx?action=viewContent&page=workarea&id=" & dataItem("ContentID") & "&LangType=" & dataItem("ContentLanguage") & "&rptType=" & dataItem("ContentType")
    End Function
    Private Function GetRequestTypeColor(ByVal dataItem As Object) As String
        Return IIf(dataItem("Status") = "S", "color:green;", "color:red;")
    End Function
    Private Function GetRequestTypeText(ByVal dataItem As Object) As String
        Return IIf(dataItem("Status") = "S", _MessageHelper.GetMessage("generic Publish"), _MessageHelper.GetMessage("generic Delete title"))
    End Function
    Private Function GetStartDate(ByVal dataItem As Object) As String
        Return IIf(dataItem("DisplayGoLive") <> "", dataItem("DisplayGoLive"), dataItem("DisplayLastEditDate"))
    End Function
    Private Function GetPathLink(ByVal dataItem As Object) As String
        Return _ContentApi.ApplicationPath & "approval.aspx?action=viewApprovalList&fldid=" & dataItem("FolderId")
    End Function

#End Region

#Region "Helpers"

    Private Sub ViewApprovalList()
        Dim OrderBy As String
        Dim bShowToolbar As Boolean = True
        Dim cTmp As Collection = New Collection

        litApproveAllWarning.Text = _MessageHelper.GetMessage("js: alert approve all selected warning")
        litDeclineAllWarning.Text = _MessageHelper.GetMessage("js: alert decline all selected warning")
        litNoItemSelected.Text = _MessageHelper.GetMessage("js:no items selected")

        OrderBy = IIf(String.IsNullOrEmpty(Request.QueryString("orderby")), Request.QueryString("orderby"), "title")

        cTmp.Add(_CommonApi.UserId, "UserID")
        _Folder = IIf(String.IsNullOrEmpty(Request.QueryString("fldid")), String.Empty, Request.QueryString("fldid"))
        cTmp.Add(_Folder, "FolderIDs")
        cTmp.Add("", "OrderBy")

        If (_ContentType > 0) Then
            cTmp.Add(_ContentType, "ContentType")
        End If

        _ApprovalsCollection = _EkContent.GetApprovalListForUserIDv1_1(cTmp)

        bShowToolbar = IIf(Request.QueryString("notoolbar") = "1", False, True)
        If (bShowToolbar) Then
            ViewToolBar()
        End If
    End Sub

    Private Sub ViewToolBar()
        Dim result As New System.Text.StringBuilder
        Dim count As Integer = 0
        Dim lAddMultiType As Integer = 0
        Dim bSelectedFound As Boolean = False
        Dim fldr As Collection

        If (_Folder = "") Then
            divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view all awaiting approval") & "")
        Else
            fldr = _EkContent.GetFolderInfov2_0(_Folder)
            divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view awaiting approval (folder)") & " """ & fldr("FolderName") & """")
        End If
        result.Append("<table><tr>")
        If (_ApprovalsCollection.Count) Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath & "Images/ui/icons/approvalApproveItem.png", "#", _MessageHelper.GetMessage("alt approve all selected button text"), _MessageHelper.GetMessage("btn approve all"), "onclick=""return Ektron.Workarea.Reports.Approval.submit(true);"""))
            result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath & "Images/ui/icons/approvalDenyItem.png", "#", _MessageHelper.GetMessage("alt deny all selected button text"), _MessageHelper.GetMessage("btn deny all"), "onclick=""return Ektron.Workarea.Reports.Approval.submit(false);"""))
        End If
        If (Request.QueryString("page") = "workarea") Then
            ' redirect to workarea when user clicks back button if we're in workarea
            result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath & "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        ElseIf (_Folder <> "") Then
            result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath & "images/UI/Icons/back.png", "approval.aspx?action=viewApprovalList", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
            'Else
            '    result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath & "images/UI/Icons/back.png", "javascript:history.back()", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), ""))
        End If

        If (Not String.IsNullOrEmpty(_CommonApi.RequestInformationRef.SystemEmail)) Then result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath & "images/UI/Icons/email.png", "#", _MessageHelper.GetMessage("Email Report button text"), _MessageHelper.GetMessage("btn email"), "onclick=""LoadUserListChildPage();"""))
        result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath & "images/UI/Icons/print.png", "#", _MessageHelper.GetMessage("Print Report button text"), _MessageHelper.GetMessage("btn print"), "onclick=""PrintReport();"""))

        result.Append(_StyleHelper.GetButtonEventsWCaption(_CommonApi.AppPath & "images/UI/Icons/folderopen.png", "#", _MessageHelper.GetMessage("alt select folder"), _MessageHelper.GetMessage("btn select folder"), "onclick=""LoadFolderChildPage('viewapprovallist','" & _ContentLanguage & "');"""))

        If _EnableMultilingual = 1 Then
            Dim m_refsite As New SiteAPI
            Dim language_data() As LanguageData = m_refsite.GetAllActiveLanguages

            result.Append("<td class=""label""> | " & _MessageHelper.GetMessage("lbl View") & ":")
            result.Append("<select id=selLang name=selLang onchange=""Ektron.Workarea.Reports.Approval.loadLanguage('frmMain');"">")
            If _ContentLanguage = -1 Then
                result.Append("<option value=" & ALL_CONTENT_LANGUAGES & " selected>All</option>")
            Else
                result.Append("<option value=" & ALL_CONTENT_LANGUAGES & ">All</option>")
            End If
            For count = 0 To language_data.Length - 1
                If Convert.ToString(_ContentLanguage) = Convert.ToString(language_data(count).Id) Then
                    result.Append("<option value=" & language_data(count).Id & " selected>" & language_data(count).Name & "</option>")
                Else
                    result.Append("<option value=" & language_data(count).Id & ">" & language_data(count).Name & "</option>")
                End If
            Next
            result.Append("</select></td>")
        End If

        GetAddMultiType()
        ' If there is no content type from querystring check for the cookie and restore it to that value else all types



        result.Append("<td><select id=selAssetSupertype name=selAssetSupertype onchange=""Ektron.Workarea.Reports.Approval.updateView();"">")
        If CMSContentType_AllTypes = _ContentType Then
            result.Append("<option value='" & CMSContentType_AllTypes & "' selected>All Types</option>")
        Else
            result.Append("<option value='" & CMSContentType_AllTypes & "'>All Types</option>")
        End If
        If CMSContentType_Content = _ContentType Then
            result.Append("<option value='" & CMSContentType_Content & "' selected>HTML Content</option>")
        Else
            result.Append("<option value='" & CMSContentType_Content & "'>HTML Content</option>")
        End If
        If (Not (IsNothing(_AssetInfoData))) Then
            If (_AssetInfoData.Length > 0) Then
                For count = 0 To _AssetInfoData.Length - 1
                    If (ManagedAsset_Min <= _AssetInfoData(count).TypeId And _AssetInfoData(count).TypeId <= ManagedAsset_Max) Then
                        If "*" = _AssetInfoData(count).PluginType Then
                            lAddMultiType = _AssetInfoData(count).TypeId
                        Else
                            result.Append("<option value='" & _AssetInfoData(count).TypeId & "'")
                            If _AssetInfoData(count).TypeId = _ContentType Then
                                result.Append(" selected")
                                bSelectedFound = True
                            End If
                            result.Append(">" & _AssetInfoData(count).CommonName & "</option>")
                        End If
                    End If
                Next
            End If
        End If
        If CMSContentType_Forms = _ContentType Then
            result.Append("<option value='" & CMSContentType_Forms & "' selected>Forms/Survey</option>")
        Else
            result.Append("<option value='" & CMSContentType_Forms & "'>Forms/Survey</option>")
        End If
        If CMSContentType_Library = _ContentType Then
            result.Append("<option value='" & CMSContentType_Library & "' selected>Images</option>")
        Else
            result.Append("<option value='" & CMSContentType_Library & "'>Images</option>")
        End If
        If (CMSContentType_NonImageLibrary = _ContentType) Then
            result.Append("<option value='" & CMSContentType_NonImageLibrary & "' selected>Non Image Managed Files</option>")
        Else
            result.Append("<option value='" & CMSContentType_NonImageLibrary & "'>Non Image Managed Files</option>")
        End If
        If (CMSContentType_PDF = _ContentType) Then
            result.Append("<option value='" & CMSContentType_PDF & "' selected>PDF</option>")
        Else
            result.Append("<option value='" & CMSContentType_PDF & "'>PDF</option>")
        End If
        result.Append("</select></td>")

        result.Append("<td>")
        result.Append(_StyleHelper.GetHelpButton("viewApprovalList"))
        result.Append("</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub

    Public Function GetAddMultiType() As Long
        ' gets ID for "add multiple" asset type
        GetAddMultiType = 0
        Dim count As Integer
        _AssetInfoData = _ContentApi.GetAssetSupertypes()
        If (Not _AssetInfoData Is Nothing) Then

            For count = 0 To _AssetInfoData.Length - 1
                If (ManagedAsset_Min <= _AssetInfoData(count).TypeId And _AssetInfoData(count).TypeId <= ManagedAsset_Max) Then
                    If "*" = _AssetInfoData(count).PluginType Then
                        GetAddMultiType = _AssetInfoData(count).TypeId
                    End If
                End If
            Next
        End If
    End Function

#End Region

#Region "JS/CSS"

    Private Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub

#End Region

End Class
