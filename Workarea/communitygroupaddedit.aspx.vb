Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Workarea
Partial Class communitygroupaddedit
    Inherits workareabase
    Implements ICallbackEventHandler

    Protected cgGroup As New Ektron.Cms.CommunityGroupData
    Protected bAccess As Boolean = False
    Protected TaxonomyTreeIdList As String = ""
    Protected TaxonomyTreeParentIdList As String = ""
    Protected updateFieldId As String = ""
    Protected commparams As String = ""
    Protected TaxonomyRoleExists As Boolean = False
    Protected m_intTaxFolderId As Long = 0
    Protected TaxonomyOverrideId As Long = 0
    Protected bThickBox As Boolean = False
    Protected language_data() As LanguageData = Nothing
    Protected m_refSiteApi As New SiteAPI()
    Protected TaxonomyId As Long = 0
    Protected profileTaxonomyId As Long = 0
    Protected TaxonomyLanguage As Integer = 1033
    Protected AppPath As String = ""
    Protected m_callbackresult As String = ""
    Protected m_strKeyWords As String = ""
    Protected m_strSearchText As String = ""
    Protected m_searchMode As String = "display_name"
    Protected m_recipientsPage As Integer = 1
    Protected m_intTotalPages As Integer = 0
    Protected m_friendsOnly As Boolean = False
    Protected m_userId As Long = 0
    Protected m_refUserApi As UserAPI = Nothing
    Protected m_user_list As UserData() = Array.CreateInstance(GetType(Ektron.Cms.UserData), 0)
    Protected m_uniqueId As String = "__Page"
    Protected m_bAdmin As Boolean = False
    Protected m_mMemberStatus As EkEnumeration.GroupMemberStatus = EkEnumeration.GroupMemberStatus.NotInGroup
    Protected m_bGroupAdmin As Boolean = False
    Private userList As New Ektron.Cms.API.User.User
    Private _MessageBoardApi As New Ektron.Cms.Community.MessageBoardAPI
    Private _CalendarApi As Ektron.Cms.Content.Calendar.WebCalendar = New Ektron.Cms.Content.Calendar.WebCalendar(m_refContentApi.RequestInformationRef)
    Private groupMessageBoardModerate As Boolean
    Protected bSuppressTaxTab As Boolean = False
    Dim calendardata As New Ektron.Cms.Common.Calendar.WebCalendarData
    Protected _doesForumExists As Long = 0
    Protected groupAliasList As String = String.Empty
    ''' <summary>
    ''' Returns true if there ar emore than one languages enabled for the site.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsSiteMultilingual() As Boolean
        Get
            If (m_refUserApi.EnableMultilingual = 0) Then
                Return False
            End If
            Dim languageEnabledCount As Integer = 0
            For Each lang As LanguageData In languageDataArray
                If lang.SiteEnabled Then languageEnabledCount += 1
                If languageEnabledCount > 1 Then Exit For
            Next

            Return languageEnabledCount > 1
        End Get

    End Property

    Public ReadOnly Property languageDataArray() As LanguageData()
        Get
            If (language_data Is Nothing) Then
                language_data = m_refSiteApi.GetAllActiveLanguages()
            End If

            Return language_data
        End Get

    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim bPermissions As Boolean = True
        Dim refCommonAPI As New CommonApi()
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        m_userId = refCommonAPI.RequestInformationRef().UserId
        AppPath = m_refContentApi.AppPath
        lblUpload.Text = m_refMsg.GetMessage("upload txt")
        RegisterResources()
        Try
            If (Not Me.IsCallback) Then
                cgae_userselect_done_btn.Attributes.Add("onclick", "GetCommunityMsgObject('" + m_uniqueId + "').MsgSaveMessageTargetUI(); return false;")
                cgae_userselect_done_btn.Attributes.Add("class", "EktMsgTargetsDoneBtn")
                cgae_userselect_done_btn.Text = m_refMsg.GetMessage("btn done")
                'cgae_userselect_done_btn.Tooltip = m_refMsg.GetMessage("btn done")

                cgae_userselect_cancel_btn.Attributes.Add("onclick", "GetCommunityMsgObject('" + m_uniqueId + "').MsgCancelMessageTargetUI(); return false;")
                cgae_userselect_cancel_btn.Attributes.Add("class", "EktMsgTargetsCancelBtn")
                cgae_userselect_cancel_btn.Text = m_refMsg.GetMessage("btn cancel")
                'cgae_userselect_cancel_btn.Tooltip = m_refMsg.GetMessage("btn cancel")

                Invite_UsrSel.SingleSelection = True

                CheckAccess()
                If Not bAccess Then
                    Throw New Exception(IIf(Me.m_iID > 0, Me.GetMessage("err communityaddedit no access"), Me.GetMessage("err no perm add cgroup")))
                End If
                Me.GroupAvatar_TB.Attributes.Add("onkeypress", "updateavatar();")
                If Request.QueryString("thickbox") <> "" Then
                    bThickBox = True
                    Ektron.Cms.API.Css.RegisterCss(Me, Me.AppPath & "csslib/ektron.communitygroup.addedit.tb.ui.css", "EktronCommunityGroupAddEditTbUiCSS")
                End If
                Dim langID As Integer = m_refContentApi.DefaultContentLanguage
                If (Request.QueryString("LangType") Is Nothing AndAlso Request.QueryString("LangType") = "") Then
                    langID = m_refContentApi.ContentLanguage
                Else
                    langID = Request.QueryString("LangType")
                End If
                If (langID = Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED Or langID = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES) Then
                    langID = refCommonAPI.GetCookieValue("SiteLanguage")
                End If

                m_refContentApi.SetCookieValue("LastValidLanguageID", langID)
                If Request.QueryString("thickbox") <> "" And Request.QueryString("tid") IsNot Nothing Then
                    Long.TryParse(Request.QueryString("tid").ToString(), TaxonomyId)
                    TaxonomyLanguage = langID
                End If
                If Request.QueryString("profileTaxonomyId") <> "" AndAlso IsNumeric(Request.QueryString("profileTaxonomyId")) AndAlso Request.QueryString("profileTaxonomyId") > 0 Then
                    profileTaxonomyId = Request.QueryString("profileTaxonomyId")
                End If
                If Page.IsPostBack Then
                    If (Not Page.IsCallback) Then
                        Process_EditGroup()
                    End If
                Else
                    'Invite_UsrSel.Initialize()
                    Select Case Me.m_sPageAction
                        Case "delete"
                            Process_DeleteGroup()
                        Case Else
                            EmitJavascript()
                            RenderRecipientSelect()
                            EditGroup()
                            SetTaxonomy(Me.m_iID)
                            SetAlias(Me.m_iID)
                    End Select
                End If
                SetLabels()
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

    Private Sub RegisterResources()
        'CSS
        Ektron.Cms.API.Css.RegisterCss(Me, AppPath + "csslib/community.css", "EktronCommunityCss")
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        'JS
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS)
    End Sub

    Private Sub SetTaxonomy(ByVal groupid As Long)
        EditTaxonomyHtml.Text = "<table><tr><td id=""TreeOutput""></td></tr></table>"
        Dim taxonomy_cat_arr As DirectoryData() = Nothing
        Dim taxonomy_data_arr As TaxonomyBaseData() = Nothing

        taxonomy_cat_arr = Me.m_refContentApi.EkContentRef.GetAllAssignedDirectory(groupid, TaxonomyItemType.Group)
        If (taxonomy_cat_arr IsNot Nothing AndAlso taxonomy_cat_arr.Length > 0) Then
            For Each taxonomy_cat As DirectoryData In taxonomy_cat_arr
                If (taxonomyselectedtree.Value = "") Then
                    taxonomyselectedtree.Value = Convert.ToString(taxonomy_cat.DirectoryId)
                Else
                    taxonomyselectedtree.Value = taxonomyselectedtree.Value & "," & Convert.ToString(taxonomy_cat.DirectoryId)
                End If
            Next
        End If
        taxonomy_data_arr = m_refContentApi.EkContentRef.GetAllTaxonomyByConfig(2)
        If ((taxonomy_data_arr Is Nothing OrElse taxonomy_data_arr.Length = 0) AndAlso (TaxonomyOverrideId = 0)) Then
            MyBase.Tabs.RemoveAt(2) ' taxonomy tab
            bSuppressTaxTab = True
        End If
        TaxonomyTreeIdList = taxonomyselectedtree.Value
        If (TaxonomyTreeIdList.Trim.Length > 0) Then
            TaxonomyTreeParentIdList = m_refContentApi.EkContentRef.ReadDisableNodeList(Me.m_iID, 2)
        End If
        'If in thickbox with preassigned taxonomy, display that taxonomy as checked
        If (Request.QueryString("thickbox") <> "") AndAlso (TaxonomyId > 0) Then
            TaxonomyTreeIdList = TaxonomyId
            If (TaxonomyTreeIdList.Trim.Length > 0) Then
                Dim taxonomyCategoryList As TaxonomyBaseData() = Nothing
                taxonomyCategoryList = m_refContentApi.EkContentRef.GetTaxonomyRecursiveToParent(TaxonomyId, TaxonomyLanguage, 0)
                If (taxonomyCategoryList IsNot Nothing AndAlso taxonomyCategoryList.Length > 0) Then
                    For Each taxonomy_cat As TaxonomyBaseData In taxonomyCategoryList
                        If (TaxonomyTreeParentIdList = "") Then
                            TaxonomyTreeParentIdList = taxonomy_cat.TaxonomyId.ToString()
                        Else
                            TaxonomyTreeParentIdList = TaxonomyTreeParentIdList & "," & taxonomy_cat.TaxonomyId.ToString()
                        End If
                    Next
                End If
            End If
        End If

        m_intTaxFolderId = 0
        If (Request.QueryString("TaxonomyId") IsNot Nothing AndAlso Request.QueryString("TaxonomyId") <> "") Then
            TaxonomyOverrideId = Convert.ToInt64(Request.QueryString("TaxonomyId"))
        End If
        js_taxon.Text = Environment.NewLine
        js_taxon.Text &= "var taxonomytreearr=""" & TaxonomyTreeIdList & """.split("","");" & Environment.NewLine
        js_taxon.Text &= "var taxonomytreedisablearr=""" & TaxonomyTreeParentIdList & """.split("","");" & Environment.NewLine
        js_taxon.Text &= "var __TaxonomyOverrideId=""" & TaxonomyOverrideId & """.split("","");" & Environment.NewLine
        js_taxon.Text &= "var m_fullScreenView=false;var __EkFolderId = " & -2 & ";" & Environment.NewLine
    End Sub
    Private Sub SetAlias(ByVal groupId As Long)
        Dim _communityAlias As New Ektron.Cms.API.UrlAliasing.UrlAliasCommunity
        Dim aliasList As System.Collections.Generic.List(Of Ektron.Cms.Common.UrlAliasCommunityData)

        aliasList = _communityAlias.GetListGroup(groupId)
        If (aliasList.Count > 0) Then
            For Each item As Ektron.Cms.Common.UrlAliasCommunityData In aliasList
                groupAliasList += "<a href= " & Me.m_refContentApi.SitePath & item.AliasName & " target=_blank>" & Me.m_refContentApi.SitePath & item.AliasName & "</a>"
                groupAliasList += "<br/>"
            Next
        Else
            phAliasTab.Visible = False
            phAliasFrame.Visible = False
        End If
    End Sub
    Protected Sub EditGroup()
        Dim refCommonAPI As New CommonApi()

        BuildJS()
        If Me.m_iID > 0 Then
            cgGroup = m_refCommunityGroupApi.GetCommunityGroupByID(Me.m_iID)
            lbl_id.Text = cgGroup.GroupId
            PopulateData(cgGroup)
        Else
            Me.PublicJoinYes_RB.Checked = True
            If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.MembershipUsers)) Then
                Me.FeaturesCalendar_CB.Checked = True
                Me.FeaturesForum_CB.Checked = True
                Me.EnableDistributeToSite_CB.Checked = True
            End If
            tr_ID.Visible = False
        End If
        TD_GroupTags.InnerHtml = GetGroupTags()
    End Sub

#Region "Process"

    Protected Sub Process_DeleteGroup()
        cgGroup = Me.m_refCommunityGroupApi.GetCommunityGroupByID(Me.m_iID)
        If cgGroup.GroupId > 0 And bAccess Then
            Me.m_refCommunityGroupApi.DeleteCommunityGroupByID(Me.m_iID)

            js_taxon.Text = "var __TaxonomyOverrideId=""" & TaxonomyOverrideId & """.split("","");" & Environment.NewLine
            API.JS.RegisterJSBlock(Me, "self.parent.location.reload(true); self.parent.ektb_remove();", "EktronSelfParentLocationReloadJS")
        Else
            Throw New Exception(GetMessage("err no perm del cgroup"))
        End If
    End Sub

    Protected Sub Process_EditGroup()
        If bAccess Then
            If m_iID > 0 Then
                cgGroup = Me.m_refCommunityGroupApi.GetCommunityGroupByID(Me.m_iID)
                groupMessageBoardModerate = _MessageBoardApi.IsModerated(m_iID, MessageBoardObjectType.CommunityGroup)
                If groupMessageBoardModerate <> False Or chkMsgBoardModeration.Checked <> False Then
                    _MessageBoardApi.Moderate(m_iID, MessageBoardObjectType.CommunityGroup, m_userId, Me.chkMsgBoardModeration.Checked)
                End If
            Else
                cgGroup = New CommunityGroupData
            End If

            cgGroup.GroupName = Me.GroupName_TB.Text.Trim()
            calendardata.Name = "ekCalendar"
            calendardata.Description = ""
            If Request.Form("ektouserid__Page") <> "" AndAlso IsNumeric(Request.Form("ektouserid__Page")) AndAlso Request.Form("ektouserid__Page") > 0 Then
                If m_iID > 0 Then Ektron.Cms.Common.Cache.ApplicationCache.Invalidate("GroupAccess_" & m_iID.ToString() & "_" & cgGroup.GroupAdmin.Id.ToString()) ' old
                cgGroup.GroupAdmin.Id = Request.Form("ektouserid__Page")
                If m_iID > 0 Then Ektron.Cms.Common.Cache.ApplicationCache.Invalidate("GroupAccess_" & m_iID.ToString() & "_" & cgGroup.GroupAdmin.Id.ToString()) ' new
            End If
            cgGroup.GroupShortDescription = Me.ShortDescription_TB.Text
            cgGroup.GroupLongDescription = Me.Description_TB.Text
            If m_iID > 0 AndAlso Not (cgGroup.GroupEnroll = Me.PublicJoinYes_RB.Checked) Then Ektron.Cms.Common.Cache.ApplicationCache.Invalidate("GroupEnroll_" & m_iID.ToString())
            cgGroup.GroupEnroll = Me.PublicJoinYes_RB.Checked
            cgGroup.GroupLocation = Me.Location_TB.Text
            cgGroup.GroupEnableDistributeToSite = Me.EnableDistributeToSite_CB.Checked
            cgGroup.AllowMembersToManageFolders = Me.AllowMembersToManageFolders_CB.Checked
            ' taxonomy
            TaxonomyTreeIdList = Request.Form(taxonomyselectedtree.UniqueID)
            If (TaxonomyTreeIdList.Trim.EndsWith(",")) Then
                TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1)
            End If
            Dim tax_request As New TaxonomyRequest
            tax_request.TaxonomyIdList = TaxonomyTreeIdList
            cgGroup.GroupCategory = TaxonomyTreeIdList
            ' taxonomy
            ' file
            Dim sfileloc As String = ""
            If Not fileupload1.PostedFile Is Nothing AndAlso fileupload1.PostedFile.FileName <> "" Then 'Check to make sure we actually have a file to upload
                Dim bSuccess As Boolean = False
                Dim strLongFilePath As String = fileupload1.PostedFile.FileName
                Dim aNameArray As String() = Split(fileupload1.PostedFile.FileName, "\")
                Dim strFileName As String = ""
                If aNameArray.Length > 0 Then
                    strFileName = aNameArray((aNameArray.Length - 1))
                End If
                strFileName = (System.Guid.NewGuid.ToString()).Substring(0, 5) & "_g_" & strFileName
                Select Case fileupload1.PostedFile.ContentType
                    Case "image/pjpeg", "image/jpeg", "image/gif" 'Make sure we are getting a valid JPG/gif image
                        fileupload1.PostedFile.SaveAs(Server.MapPath(m_refCommunityGroupApi.SitePath & "uploadedimages/" & strFileName))
                        lbStatus.Text = String.Format(m_refCommunityGroupApi.EkMsgRef.GetMessage("lbl success avatar uploaded"), strFileName, (m_refCommunityGroupApi.SitePath & "uploadedimages/" & strFileName))
                        Utilities.ProcessThumbnail(Server.MapPath(m_refCommunityGroupApi.SitePath & "uploadedimages/"), strFileName)
                        bSuccess = True
                    Case Else
                        'Not a valid jpeg/gif image
                        lbStatus.Text = m_refCommunityGroupApi.EkMsgRef.GetMessage("lbl err avatar not valid extension")
                End Select
                sfileloc = m_refCommunityGroupApi.SitePath & "uploadedimages/thumb_" & Utilities.GetCorrectThumbnailFileWithExtn(strFileName)
                cgGroup.GroupImage = sfileloc
            Else
                cgGroup.GroupImage = Me.GroupAvatar_TB.Text
            End If
            ' file
            If m_iID > 0 Then
                m_refCommunityGroupApi.UpdateCommunityGroup(cgGroup)
                UpdateGroupTags(False)
                InitiateProcessAction()
            Else
                m_iID = m_refCommunityGroupApi.AddCommunityGroup(cgGroup)
                'ADDTAXONOMYITEM to Group eIntranet
                If (Not (String.IsNullOrEmpty(TaxonomyTreeIdList))) Then
                    TaxonomyTreeIdList = TaxonomyTreeIdList & ","
                Else
                    TaxonomyTreeIdList = ""
                End If
                If profileTaxonomyId > 0 Then
                    m_refCommunityGroupApi.EkContentRef.AddDirectoryItem(TaxonomyTreeIdList & profileTaxonomyId.ToString(), m_iID.ToString(), TaxonomyItemType.Group)
                Else
                    profileTaxonomyId = m_refCommunityGroupApi.EkContentRef.GetTaxonomyIdByPath("\" & m_refCommunityGroupApi.UserId & "\Groups", 1)
                    m_refCommunityGroupApi.EkContentRef.AddDirectoryItem(TaxonomyTreeIdList & profileTaxonomyId.ToString(), m_iID.ToString(), TaxonomyItemType.Group)
                End If
                
                groupMessageBoardModerate = _MessageBoardApi.IsModerated(m_iID, MessageBoardObjectType.CommunityGroup)
                If groupMessageBoardModerate <> False Or chkMsgBoardModeration.Checked <> False Then
                    _MessageBoardApi.Moderate(m_iID, MessageBoardObjectType.CommunityGroup, m_userId, chkMsgBoardModeration.Checked)
                End If


                If cgGroup.GroupCategory <> "" Then
                    TaxonomyId = 0
                End If
                If (m_iID > 0) Then
                    UpdateGroupTags(True)
                    InitiateProcessAction()
                Else
                    EmitJavascript()
                    RenderRecipientSelect()
                    EditGroup()
                    SetTaxonomy(Me.m_iID)
                    SetAlias(Me.m_iID)
                    errmsg.InnerHtml = "Error occured while adding this group. Please verify the group name is unique and try again."
                    errmsg.Attributes.Add("class", "excpt")
                    ' GroupName_TB.Attributes.Add("onkeypress", "ClearErr();")
                    GroupName_TB.Focus()
                End If
            End If
            If FeaturesCalendar_CB.Checked = True Then
                _CalendarApi.Add(calendardata, WorkSpace.Group, m_iID)
            End If
            If FeaturesForum_CB.Checked = True AndAlso FeaturesForum_CB.Enabled <> False Then
                m_refCommunityGroupApi.AddCommunityGroupForum(WorkSpace.Group, m_iID)
            End If

        Else
            Throw New Exception(GetMessage("err no perm add cgroup"))
        End If
    End Sub

#End Region

#Region "Helper Functions"
    Protected Sub CheckAccess()
        Dim m_pSecurity As PermissionData = Me.m_refContentApi.LoadPermissions(0, "folder")
        If Me.m_refContentApi.UserId > 0 And m_pSecurity.IsLoggedIn Then
            If Me.m_iID > 0 Then
                Dim mMemberStatus As EkEnumeration.GroupMemberStatus
                mMemberStatus = Me.m_refCommunityGroupApi.GetGroupMemberStatus(Me.m_iID, Me.m_refContentApi.UserId())
                bAccess = (Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) OrElse mMemberStatus = EkEnumeration.GroupMemberStatus.Leader)
                m_bGroupAdmin = bAccess
            Else
                bAccess = (Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupCreate))
            End If
        End If
    End Sub
    Protected Sub InitiateProcessAction()
        If TaxonomyId > 0 Then
            ' add to taxonomy
            Dim item_request As New TaxonomyRequest
            item_request.TaxonomyId = TaxonomyId
            item_request.TaxonomyIdList = Me.m_iID
            item_request.TaxonomyItemType = TaxonomyItemType.Group
            item_request.TaxonomyLanguage = TaxonomyLanguage
            Me.m_refContentApi.EkContentRef.AddTaxonomyItem(item_request)
        End If
        If bThickBox Then
            Response.Redirect("CloseThickbox.aspx", False)
        Else
            Response.Redirect("community/groups.aspx", False)
        End If
    End Sub
    Protected Sub BuildJS()
        Dim sbJS As New StringBuilder()

        sbJS.Append("<script type=""text/javascript"">").Append(Environment.NewLine)
        sbJS.Append("<!--//--><![CDATA[//><!--").Append(Environment.NewLine)
        sbJS.Append("function SubmitForm() {" & Environment.NewLine)
        sbJS.Append("   var groupName = document.getElementById('GroupName_TB').value;").Append(Environment.NewLine)
        sbJS.Append("   if (Trim(groupName).length == 0)").Append(Environment.NewLine)
        sbJS.Append("   {alert('" & GetMessage("lbl please enter group name") & "');").Append(Environment.NewLine)
        sbJS.Append("   return false;}").Append(Environment.NewLine)
        sbJS.Append("else{ " & Environment.NewLine)
        sbJS.Append("   if (!CheckGroupForillegalChar()) {" & Environment.NewLine)
        sbJS.Append("   		return false;" & Environment.NewLine)
        sbJS.Append("   } else { document.forms[0].submit(); }" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function ExecSearch() {" & Environment.NewLine)
        sbJS.Append("   var sTerm = Trim(document.getElementById('txtSearch').value); " & Environment.NewLine)
        'sbJS.Append("   if (sTerm == '') {" & Environment.NewLine)
        'sbJS.Append("       alert('").Append(GetMessage("err js no search term")).Append("'); " & Environment.NewLine)
        'sbJS.Append("   } else {" & Environment.NewLine)
        sbJS.Append("	    document.getElementById('hdn_search').value = true;" & Environment.NewLine)
        sbJS.Append("	    document.forms[0].submit();" & Environment.NewLine)
        'sbJS.Append("   }" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)
        sbJS.Append("function resetPostback() {" & Environment.NewLine)
        sbJS.Append("   document.forms[0].isPostData.value = """"; " & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function CheckGroupForillegalChar() {" & Environment.NewLine)
        sbJS.Append("   var val = document.forms[0]." & Replace(Me.GroupName_TB.UniqueID, "$", "_") & ".value;" & Environment.NewLine)
        sbJS.Append("   if ((val.indexOf("";"") > -1) || (val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbJS.Append("   {" & Environment.NewLine)
        sbJS.Append("       alert(""" & String.Format(GetMessage("lbl group name disallowed chars"), "(';', '\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')") & """);" & Environment.NewLine)
        sbJS.Append("       return false;" & Environment.NewLine)
        sbJS.Append("   }" & Environment.NewLine)
        sbJS.Append("   return true;" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("  			function LoadLanguage(FormName){ ").Append(Environment.NewLine)
        sbJS.Append("  				var num=document.forms[FormName].selLang.selectedIndex; ").Append(Environment.NewLine)
        sbJS.Append("  				window.location.href=""community/groups.aspx?action=viewallgroups""+""&LangType=""+document.forms[FormName].selLang.options[num].value; ").Append(Environment.NewLine)
        sbJS.Append("  				//document.forms[FormName].submit(); ").Append(Environment.NewLine)
        sbJS.Append("  				return false; ").Append(Environment.NewLine)
        sbJS.Append("  			} ").Append(Environment.NewLine)

        sbJS.Append("        function CheckUpload() ").Append(Environment.NewLine)
        sbJS.Append("        { ").Append(Environment.NewLine)
        sbJS.Append("            var ofile = document.getElementById('fileupload1'); ").Append(Environment.NewLine)
        sbJS.Append("            if (ofile.value == '') { ").Append(Environment.NewLine)
        sbJS.Append("               alert('").Append(Me.m_refContentApi.EkMsgRef.GetMessage("js err select avatar upload")).Append("'); ").Append(Environment.NewLine)
        sbJS.Append("               ofile.outerHTML = ofile.outerHTML; ").Append(Environment.NewLine)
        sbJS.Append("               return false; ").Append(Environment.NewLine)
        sbJS.Append("            } else { ").Append(Environment.NewLine)
        sbJS.Append("               if (!CheckUploadExt(ofile.value)) { ").Append(Environment.NewLine)
        sbJS.Append("                   alert('").Append(Me.m_refContentApi.EkMsgRef.GetMessage("lbl err avatar not valid extension")).Append("'); ").Append(Environment.NewLine)
        sbJS.Append("                   ofile.outerHTML = ofile.outerHTML; ").Append(Environment.NewLine)
        sbJS.Append("                   return false; ").Append(Environment.NewLine)
        sbJS.Append("               } else { ").Append(Environment.NewLine)
        sbJS.Append("                   document.getElementById('GroupAvatar_TB').value = '[file]'; ").Append(Environment.NewLine)
        sbJS.Append("                   toggleVisibility('close'); ").Append(Environment.NewLine)
        sbJS.Append("               } ").Append(Environment.NewLine)
        sbJS.Append("            } ").Append(Environment.NewLine)
        sbJS.Append("        } ").Append(Environment.NewLine)

        sbJS.Append("        function CheckUploadExt(filename) ").Append(Environment.NewLine)
        sbJS.Append("        { ").Append(Environment.NewLine)
        sbJS.Append("           var extArray = new Array("".jpg"","".jpeg"", "".gif"", "".jpeg""); ").Append(Environment.NewLine)
        sbJS.Append("           allowSubmit = false; ").Append(Environment.NewLine)
        sbJS.Append("               if (filename.indexOf(""\\"") == -1)")
        sbJS.Append("               {")
        sbJS.Append("                   ext = filename.slice(filename.lastIndexOf(""."")).toLowerCase(); ").Append(Environment.NewLine)
        sbJS.Append("                   for (var i = 0; i < extArray.length; i++)  ").Append(Environment.NewLine)
        sbJS.Append("                   { ").Append(Environment.NewLine)
        sbJS.Append("               	    if (extArray[i] == ext) { allowSubmit = true; break; } ").Append(Environment.NewLine)
        sbJS.Append("                   } ").Append(Environment.NewLine)
        sbJS.Append("               }")
        sbJS.Append("           while (filename.indexOf(""\\"") != -1) ").Append(Environment.NewLine)
        sbJS.Append("           { ").Append(Environment.NewLine)
        sbJS.Append("               filename = filename.slice(filename.indexOf(""\\"") + 1); ").Append(Environment.NewLine)
        sbJS.Append("               ext = filename.slice(filename.lastIndexOf(""."")).toLowerCase(); ").Append(Environment.NewLine)
        sbJS.Append("               for (var i = 0; i < extArray.length; i++)  ").Append(Environment.NewLine)
        sbJS.Append("               { ").Append(Environment.NewLine)
        sbJS.Append("               	if (extArray[i] == ext) { allowSubmit = true; break; } ").Append(Environment.NewLine)
        sbJS.Append("               } ").Append(Environment.NewLine)
        sbJS.Append("           } ").Append(Environment.NewLine)
        sbJS.Append("           return allowSubmit; ").Append(Environment.NewLine)
        sbJS.Append("        } ").Append(Environment.NewLine)
        sbJS.Append("//--><!]]>").Append(Environment.NewLine)
        sbJS.Append("</script>").Append(Environment.NewLine)

        ltr_js.Text = sbJS.ToString()
    End Sub

    Protected Sub SetLabels()
        phCategoryTab.Visible = Not bSuppressTaxTab
        phCategoryFrame.Visible = Not bSuppressTaxTab
        Me.ltr_groupname.Text = GetMessage("lbl community group name")
        Me.ltr_groupid.Text = GetMessage("generic id")
        Me.ltr_admin.Text = GetMessage("lbl administrator")
        Me.ltr_groupjoin.Text = GetMessage("lbl enrollment")
        Me.ltr_grouplocation.Text = GetMessage("generic location")
        Me.ltr_groupsdesc.Text = GetMessage("lbl short desc")
        Me.ltr_groupdesc.Text = GetMessage("generic description")
        Me.ltr_groupavatar.Text = GetMessage("lbl group image")
        Me.ltr_enabledistribute.Text = GetMessage("lbl enable distribute")
        Me.ltr_AllowMembersToManageFolders.Text = GetMessage("lbl allow member to manage folders")
        Me.ltr_upload.Text = GetMessage("upload txt")
        Me.ltr_ok.Text = GetMessage("lbl ok")
        Me.ltr_close.Text = GetMessage("close title")
        Me.ltr_MsgBoardModeration.Text = GetMessage("lbl msgboardmoderation")
        PublicJoinYes_RB.Text = GetMessage("lbl enrollment open")
        PublicJoinNo_RB.Text = GetMessage("lbl enrollment restricted")
        Me.ltr_groupfeatures.Text = GetMessage("lbl features") & ":"
        FeaturesCalendar_CB.Text = GetMessage("lbl enable group calendar")
        FeaturesForum_CB.Text = GetMessage("lbl enable group forum")

        If Me.m_iID > 0 Then
            MyBase.AddButtonwithMessages(AppPath & "images/UI/Icons/save.png", "#", "lbl alt save cgroup", "btn save", " onclick=""javascript: SubmitForm(); return false;"" ")
            If bThickBox Then
                'MyBase.AddButtonwithMessages(AppPath & "images/UI/Icons/cancel.png", "#", "generic cancel", "generic cancel", " onclick=""self.parent.ektb_remove();"" return false;"" ")
                If m_bGroupAdmin Then
                    MyBase.AddButtonwithMessages(AppPath & "images/UI/Icons/delete.png", "communitygroupaddedit.aspx?action=delete&id=" & Me.m_iID & "&thickbox=true", "alt del community group", "lbl del community group", " onclick=""return confirm('" & GetMessage("js confirm del community group") & "');"" ")
                End If
            Else
                SetTitleBarToMessage("lbl edit cgroup")
                AddBackButton("community/groups.aspx?action=viewgroup&id=" & cgGroup.GroupId.ToString() & "")
                AddHelpButton("editcommunitygroup")
            End If
        Else
            tr_admin.Visible = Me.m_refContentApi.IsAdmin
            MyBase.AddButtonwithMessages(AppPath & "images/UI/Icons/save.png", "#", "lbl alt save cgroup", "btn save", " onclick=""javascript: SubmitForm(); return false;"" ")
            If bThickBox Then
                'MyBase.AddButtonwithMessages(AppPath & "images/UI/Icons/cancel.png", "#", "generic cancel", "generic cancel", " onclick=""self.parent.ektb_remove();"" return false;"" ")
            Else
                SetTitleBarToMessage("lbl add cgroup")
                AddBackButton("community/groups.aspx")
                AddHelpButton("addcommunitygroup")
            End If
        End If

    End Sub

    Protected Sub PopulateData(ByVal cGrp As CommunityGroupData)
        Me.GroupName_TB.Text = cGrp.GroupName
        Me.ShortDescription_TB.Text = cGrp.GroupShortDescription
        Me.Description_TB.Text = cGrp.GroupLongDescription
        Me.PublicJoinYes_RB.Checked = cGrp.GroupEnroll
        Me.PublicJoinNo_RB.Checked = Not (cGrp.GroupEnroll)
        Me.Location_TB.Text = cGrp.GroupLocation
        Me.GroupAvatar_TB.Text = cGrp.GroupImage
        Me.EnableDistributeToSite_CB.Checked = cGrp.GroupEnableDistributeToSite
        Me.AllowMembersToManageFolders_CB.Checked = cGrp.AllowMembersToManageFolders
        Me.ltr_avatarpath.Text = ""
        ' Me.tb_admin_name.Text = cGrp.GroupAdmin.DisplayName
        groupMessageBoardModerate = _MessageBoardApi.IsModerated(Me.m_iID, MessageBoardObjectType.CommunityGroup)
        Me.chkMsgBoardModeration.Checked = groupMessageBoardModerate

        ekpmsgto__Page.Value = cGrp.GroupAdmin.DisplayName
        ektouserid__Page.Value = cGrp.GroupAdmin.Id
        calendardata = _CalendarApi.GetPublicCalendar(WorkSpace.Group, Me.m_iID)
        If calendardata IsNot Nothing Then
            FeaturesCalendar_CB.Enabled = False
            FeaturesCalendar_CB.Checked = True
        End If
        _doesForumExists = m_refCommunityGroupApi.DoesCommunityGroupForumExists(WorkSpace.Group, Me.m_iID)
        If _doesForumExists > 0 Then
            FeaturesForum_CB.Enabled = False
            FeaturesForum_CB.Checked = True
        End If

        If Me.m_sPageAction = "viewgroup" Then
            Me.GroupName_TB.Enabled = False
            Me.ShortDescription_TB.Enabled = False
            Me.Description_TB.Enabled = False
            Me.PublicJoinYes_RB.Enabled = False
            Me.PublicJoinNo_RB.Enabled = False
            Me.Location_TB.Enabled = False
            Me.EnableDistributeToSite_CB.Enabled = False
            Me.AllowMembersToManageFolders_CB.Enabled = False
        End If
    End Sub
#End Region

#Region "Group Tags"
    Public Function GetGroupTags() As String
        Dim result As New System.Text.StringBuilder
        Dim tdaGroup() As TagData = Array.CreateInstance(GetType(TagData), 0)
        Dim tdaGroupDefault() As TagData = Array.CreateInstance(GetType(TagData), 0)
        Dim td As TagData
        Dim htTagsAssignedToGroup As New Hashtable
        Dim htDefTagsAssignedToGroup As New Hashtable
        Dim sAppliedTags As String = ""
        Try

            error_TagsCantBeBlank.Text = m_refMsg.GetMessage("msg error Blank Tag")
            error_InvalidChars.Text = m_refMsg.GetMessage("msg error Tag invalid chars")

            result.Append("<div id=""newTagNameDiv"" class=""ektronWindow ektronModalStandard"">")
            result.Append("<div class=""ektronModalHeader"">")
            result.Append("     <h3>")
            result.Append("         <span class=""headerText"">" + m_refMsg.GetMessage("btn add personal tag") + "</span>")
            result.Append("         <a id=""closeDialogLink3"" class=""ektronModalClose"" href=""#"" onclick=""CancelSaveNewGroupTag();""></a>")
            result.Append("     </h3>")
            result.Append("</div>")
            result.Append("<div class=""ektronModalBody"">")
            result.Append(" <label class=""nameWidth"">" + GetMessage("name label") + "</label>&#160;<input type=""text"" id=""newTagName"" value="""" size=""20"" />")
            result.Append("<div class=""ektronTopSpace""/>")

            If (IsSiteMultilingual) Then
                result.Append("<div class=""ektronTopSpace"">")
            Else
                result.Append("<div style=""display:none;"" >")
            End If
            result.Append(" <label class=""nameWidth"">" + GetMessage("res_lngsel_lbl") + "</label>&#160;" & GetLanguageDropDownMarkup("TagLanguage"))
            result.Append("</div><br />")

            If (Me.m_iID > 0) Then
                tdaGroup = m_refTagsApi.GetTagsForObject(Me.m_iID, EkEnumeration.CMSObjectTypes.CommunityGroup, -1)
            End If
            Dim appliedTagIds As New StringBuilder
            If (Not IsNothing(tdaGroup)) Then
                For Each td In tdaGroup
                    htTagsAssignedToGroup.Add(td.Id, td)
                    appliedTagIds.Append(td.Id.ToString() + ",")
                    'sAppliedTags = sAppliedTags & td.Id & ";"
                Next
            End If

            result.Append("<div class=""ektronTopSpace"">")
            result.Append(" <ul class=""buttonWrapper ui-helper-clearfix"">")
            result.Append("     <li>")
            result.Append("         <a class=""button buttonInline redHover buttonClear"" type=""button"" value=""" + GetMessage("btn cancel") + """ title=""" + GetMessage("btn cancel") + """ onclick=""CancelSaveNewGroupTag();"">" + GetMessage("btn cancel") + "</a>")
            result.Append("     </li>")
            result.Append("     <li>")
            result.Append("         <a class=""button buttonInline greenHover buttonUpdate"" type=""button"" value=""" + GetMessage("btn save") + """ title=""" + GetMessage("btn save") + """ onclick=""SaveNewGroupTag();"">" + GetMessage("btn save") + "</a> ")
            result.Append("     </li>")
            result.Append(" </ul>")
            result.Append("</div>")

            'create hidden list of current tags so we know to delete removed ones.
            result.Append("<input type=""hidden"" id=""currentTags"" name=""currentTags"" value=""" + appliedTagIds.ToString() + """  />")
            'hidden variable for capturing new tags
            result.Append("<input type=""hidden"" id=""newTagNameHdn"" name=""newTagNameHdn"" />")

            result.Append("</div>")
            result.Append("</div>")
            result.Append("</div>")

            result.Append("<div id=""newTagNameScrollingDiv"" class=""ektronBorder"">")

            If True Then
                tdaGroupDefault = m_refTagsApi.GetDefaultTags(EkEnumeration.CMSObjectTypes.CommunityGroup, -1)
                'create hidden list of current tags so we know to delete removed ones.
                result.Append("<input type=""hidden"" id=""currentTags"" name=""currentTags"" value=""" + appliedTagIds.ToString() + """  />")

                Dim localizationApi As New LocalizationAPI()
                Dim lang As LanguageData

                For Each lang In languageDataArray
                    'create hidden list of current tags so we know to delete removed ones.
                    result.Append("<input type=""hidden"" id=""flag_" & lang.Id & """  value=""" + localizationApi.GetFlagUrlByLanguageID(lang.Id) + """  />")
                Next

                If (Not IsNothing(tdaGroupDefault)) Then
                    For Each td In tdaGroupDefault
                        Dim bCheck As Boolean = False
                        If htTagsAssignedToGroup.ContainsKey(td.Id) Then
                            bCheck = True
                            htDefTagsAssignedToGroup.Add(td.Id, td)
                        End If
                        result.Append("<input type=""checkbox"" " & IIf(bCheck, "checked=""checked""", "") & " id=""userPTagsCbx_" + td.Id.ToString + """ name=""userPTagsCbx_" + td.Id.ToString + """ onclick=""ToggleCustomPTagsCbx(this, '" + Replace(td.Text, "'", "\'") + "');"" />&#160;")
                        result.Append("<img src='" & localizationApi.GetFlagUrlByLanguageID(td.LanguageId) & "' />")
                        result.Append("&#160;" + td.Text + "<br />")
                    Next
                End If
                If (Not IsNothing(tdaGroup)) Then
                    For Each td In tdaGroup
                        If Not htDefTagsAssignedToGroup.ContainsKey(td.Id) Then
                            result.Append("<input type=""checkbox"" checked=""checked"" id=""userPTagsCbx_" + td.Id.ToString + """ name=""userPTagsCbx_" + td.Id.ToString + """ onclick=""ToggleCustomPTagsCbx(this, '" + Replace(td.Text, "'", "\'") + "');"");' />&#160;")
                            result.Append("<img src='" & localizationApi.GetFlagUrlByLanguageID(td.LanguageId) & "' />")
                            result.Append("&#160;" + td.Text + "<br />")
                        End If
                    Next
                End If
            End If

            result.Append("<div id=""newAddedTagNamesDiv""></div>")

            result.Append("</div>")

            result.Append("<div style=""float:left;"">")
            result.Append("     <a class=""button buttonLeft greenHover buttonAddTagWithText"" href=""javascript:ShowAddGroupTagArea();"" title=""" + GetMessage("alt add btn text (group tag)") + """>")
            result.Append("" + GetMessage("btn add personal tag") + "</a>")
            result.Append("</div>")

        Catch ex As Exception
        Finally
            GetGroupTags = result.ToString
            tdaGroup = Nothing
            td = Nothing
            htTagsAssignedToGroup = Nothing
            htDefTagsAssignedToGroup = Nothing
        End Try
    End Function
    Public Function UpdateGroupTags(ByVal IsAdd As Boolean) As Boolean
        Dim result As Boolean = False
        Dim defaultTags() As TagData
        Dim groupTags() As TagData
        Dim td As TagData
        Dim tagIdStr As String = ""


        Try
            Dim orginalTagIds As String
            orginalTagIds = Request.Form.Item("currentTags").Trim.ToLower

            ' Assign all default group tags that are checked:
            ' Remove tags that have been unchecked
            defaultTags = m_refTagsApi.GetDefaultTags(EkEnumeration.CMSObjectTypes.CommunityGroup, -1)
            groupTags = m_refTagsApi.GetTagsForObject(m_iID, EkEnumeration.CMSObjectTypes.CommunityGroup, -1)

            'Also, copy all users tags into defaultUserTags list
            'so that if they were removed, they can be deleted as well.
            Dim originalLength As Integer = defaultTags.Length
            Array.Resize(Of TagData)(defaultTags, defaultTags.Length + groupTags.Length)
            groupTags.CopyTo(defaultTags, originalLength)

            If (Not IsNothing(defaultTags)) Then
                For Each td In defaultTags
                    tagIdStr = "userPTagsCbx_" + td.Id.ToString
                    If ((Not IsNothing(Request.Form.Item(tagIdStr)))) Then

                        If (Request.Form.Item(tagIdStr).Trim.ToLower = "on") Then
                            'if tag is checked, but not in current tag list, add it
                            If (Not orginalTagIds.Contains(td.Id.ToString + ",")) Then
                                m_refTagsApi.AddTagToCommunityGroup(td.Id, m_iID)
                            End If
                        Else
                            'if tag is unchecked AND in current list, delete
                            If (orginalTagIds.Contains(td.Id.ToString + ",")) Then
                                m_refTagsApi.DeleteTagOnObject(td.Id, m_iID, EkEnumeration.CMSObjectTypes.CommunityGroup, m_userId)
                            End If
                        End If
                    Else
                        'if tag checkbox has no postback value AND is in current tag list, delete it
                        If (orginalTagIds.Contains(td.Id.ToString + ",")) Then
                            m_refTagsApi.DeleteTagOnObject(td.Id, m_iID, EkEnumeration.CMSObjectTypes.CommunityGroup, m_userId)
                        End If
                    End If
                Next
            End If

            ' Now add any new custom tags, that the user created:
            ' New tags are added to newTagNameHdn field in following format:  <TagText>~<LanguageID>;<TagText>~<LanguageID>;
            If (Not IsNothing(Request.Form.Item("newTagNameHdn"))) Then
                Dim custTags As String = Request.Form.Item("newTagNameHdn")
                Dim aCustTags() As String = custTags.Split(";")
                Dim tag As String
                Dim languageId As Integer

                For Each tag In aCustTags

                    Dim tagPropArray() As String = tag.Split("~")
                    If (tagPropArray.Length > 1) Then
                        If (tagPropArray(0).Trim.Length > 0) Then

                            'Default language to -1.
                            '"ALL" option in drop down is 0 - switch to -1.
                            If (Not Int32.TryParse(tagPropArray(1), languageId)) Then
                                languageId = -1
                            End If
                            If (languageId = 0) Then languageId = -1

                            m_refTagsApi.AddTagToCommunityGroup(tagPropArray(0), m_iID, languageId)
                        End If
                    End If
                Next
            End If

            result = True

            result = True

        Catch ex As Exception
            result = False
        Finally
            UpdateGroupTags = result
        End Try
    End Function
    Private Function GetLanguageDropDownMarkup(ByVal controlId As String) As String

        Dim i As Integer
        Dim markup As New StringBuilder

        If (IsSiteMultilingual) Then
            markup.Append("<select id=""" & controlId & """ name=""" & controlId & """>")
            If (Not (IsNothing(languageDataArray))) Then
                For i = 0 To languageDataArray.Length - 1
                    If (languageDataArray(i).SiteEnabled) Then
                        markup.Append("<option ")
                        If (Me.m_refContentApi.DefaultContentLanguage = languageDataArray(i).Id) Then
                            markup.Append(" selected")
                        End If
                        markup.Append(" value=" & languageDataArray(i).Id & ">" & languageDataArray(i).LocalName)
                    End If
                Next
            End If
            markup.Append("</select>")
        Else
            'hardcode to default site language
            markup.Append("<select id=""" & controlId & """ name=""" & controlId & """ selectedindex=""0"" >")
            markup.Append(" <option selected value=" & m_refContentApi.DefaultContentLanguage & ">")
            markup.Append("</select>")

        End If

        Return markup.ToString()
    End Function
#End Region

#Region "Browse Users"
    Protected Function GetUserAPI() As UserAPI
        If (IsNothing(m_refUserApi)) Then
            m_refUserApi = New UserAPI()
        End If
        Return m_refUserApi
    End Function

    Protected Sub EmitJavascript()
        If ((Not IsNothing(Page)) AndAlso Not Page.ClientScript.IsClientScriptBlockRegistered("AjaxJavascript")) Then
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "AjaxJavascript", GetAjaxJavascript())

            EmitInitializationJavascript()
        End If
    End Sub

    Protected Function GetAjaxJavascript() As String
        Dim result As String = ""
        Dim sb As New System.Text.StringBuilder()

        Try
            If (Not (Page.IsCallback)) Then

                sb.Append("<script type=""text/javascript"">" + Environment.NewLine)
                sb.Append("<!--//--><![CDATA[//><!--" + Environment.NewLine)

                Dim ServerCallFunctionInvocation As String = Page.ClientScript.GetCallbackEventReference(Me, "args", "MsgTarg_AjaxDisplayResult", "context", "MsgTarg_AjaxDisplayError", False)

                sb.Append("function __MsgTargCallBackToServer" + m_uniqueId + "(args,context){" + Environment.NewLine)
                sb.Append(ServerCallFunctionInvocation)
                sb.Append("}" + Environment.NewLine)

                sb.Append("//--><!]]>" + Environment.NewLine)
                sb.Append("</script>" + Environment.NewLine)
            End If

        Catch ex As Exception

        Finally
            result = sb.ToString()
            sb = Nothing
        End Try

        Return (result)
    End Function

    Protected Sub EmitInitializationJavascript()
        Dim sb As New System.Text.StringBuilder()

        Try
            If (Not (Page.IsCallback)) Then
                ' Create initialization code:
                sb.Append("<script type=""text/javascript"">" + Environment.NewLine)
                sb.Append("<!--//--><![CDATA[//><!-- \n " + Environment.NewLine)

                sb.Append("GetCommunityMsgObject('" + m_uniqueId + "').SetUserSelectId('" + Invite_UsrSel.ControlId + "');" + Environment.NewLine)
                'sb.Append("// Intialize:" + Environment.NewLine)
                'sb.Append("GetCommunityMsgObject('" + m_uniqueId + "').MsgInitSelectedUsers();" + Environment.NewLine)
                sb.Append("//--><!]]>" + Environment.NewLine)
                sb.Append("</script>" + Environment.NewLine)
                litInitialize.Text = sb.ToString()

                ' Create browse button:
                BrowseUsers.Text = "<a class=""button buttonInlineBlock blueHover btnUpload buttonBrowseUSer"" href=""#"" onclick=""GetCommunityMsgObject('" + m_uniqueId + "').MsgShowMessageTargetUI('ektouserid" + m_uniqueId + "', true); return false;"" >" + GetMessage("btn browse") + "</a>"
            End If

        Catch ex As Exception

        Finally
            sb = Nothing
        End Try
    End Sub

    Protected Sub RenderRecipientSelect()
        Dim outStr As String = ""
        CollectSearchText()
        GetUsers()

        outStr = "<div id=""EktMsgTargetsBody" + m_uniqueId + """ class=""EktMsgTargetsBody"">" + Environment.NewLine
        outStr += BuildRecipientSelect()
        outStr += "</div>"
        'Me.ltr_recipientselect.Text = outStr
    End Sub

    Private Sub CollectSearchText()
        Dim taxType As Integer = 1 ' Set to one for real data. For development only temporarily set to zero (zero allows creating freind taxonomies using existing taxonomy UI).

        Dim myTaxId As Long = GetUserAPI().EkContentRef.GetTaxonomyIdByPath("\" + m_userId.ToString(), taxType)
        Dim mytaxonomyquery As String = " and user_id in (select taxonomy_item_id from taxonomy_item_tbl where (taxonomy_id=" + myTaxId.ToString() + " or (taxonomy_id in (select taxonomy_child_id from taxonomy_children_tbl where taxonomy_id=" + myTaxId.ToString() + "))) and taxonomy_item_type=1 )"

        If (m_searchMode = "all_names") Then
            m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%' OR last_name like '%" & Quote(m_strKeyWords) & "%' OR user_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_searchMode = "last_name") Then
            m_strSearchText = " (last_name like '%" & Quote(m_strKeyWords) & "%') "
        ElseIf (m_searchMode = "first_name") Then
            m_strSearchText = " (first_name like '%" & Quote(m_strKeyWords) & "%')"
        ElseIf (m_searchMode = "display_name") Then
            m_strSearchText = " (display_name like '%" & Quote(m_strKeyWords) & "%')"
        End If

        If (m_friendsOnly) Then
            m_strSearchText += mytaxonomyquery
        End If
    End Sub

    Private Sub GetUsers()
        If Trim(m_strSearchText) <> "" Then
            Dim req As New UserRequestData
            req.Type = -1                   ' IIf(m_UserType = UserTypes.AuthorType, 0, 1)
            If (GetUserAPI().IsAdmin) Then
                req.Group = -1              ' IIf(m_UserType = UserTypes.AuthorType, 2, 888888)
            Else
                req.Group = userList.EkUserRef.GetCmsGroupForCommunityGroup(m_iID)
            End If
            req.RequiredFlag = 0
            req.OrderBy = ""
            req.OrderDirection = "asc"
            req.SearchText = m_strSearchText
            req.PageSize = 4
            req.CurrentPage = m_recipientsPage
            m_user_list = userList.GetAllUsers(req)
            'm_user_list = GetUserAPI().GetAllUsers(req)
            m_intTotalPages = req.TotalPages
        End If
    End Sub

    Private Function Quote(ByVal KeyWords As String) As String
        Dim result As String = KeyWords
        If (KeyWords.Length > 0) Then
            result = KeyWords.Replace("'", "''")
        End If
        Return result
    End Function

    Protected Function IsSelected(ByVal msg As String) As String
        Dim result As String = ""
        If (msg = m_searchMode) Then
            result = " selected=""selected"" "
        End If
        Return (result)
    End Function

    Protected Function BuildRecipientSelect() As String
        Dim result As String = ""
        Dim DisplayName As String = ""
        Dim sbSelect As New StringBuilder()

        sbSelect.Append("	<div class=""EktMsgTargetsTopControls"" >" + Environment.NewLine)

        sbSelect.Append("	<div class=""EktMsgTargetsTopControlsSearch"" >" + Environment.NewLine)
        sbSelect.Append("		<input type=""text"" class=""EktMsgTargetsSearchText"" id=""txtSearch" + m_uniqueId + """ name=""txtSearch" + m_uniqueId + """ value=""" & m_strKeyWords & """ >" + Environment.NewLine)
        sbSelect.Append("		<select id=""searchModeSel" + m_uniqueId + """ name=""searchModeSel" + m_uniqueId + """ >" + Environment.NewLine)
        sbSelect.Append("			<option value=""display_name""" & IsSelected("display_name") & ">" + m_refMsg.GetMessage("generic display name") + "</option>" + Environment.NewLine)
        sbSelect.Append("			<option value=""last_name""" & IsSelected("last_name") & ">" + m_refMsg.GetMessage("generic last name") + "</option>" + Environment.NewLine)
        sbSelect.Append("			<option value=""first_name""" & IsSelected("first_name") & ">" + m_refMsg.GetMessage("generic first name") + "</option>" + Environment.NewLine)
        sbSelect.Append("			<option value=""all_names""" & IsSelected("all_names") & ">" + m_refMsg.GetMessage("generic all") + "</option>" + Environment.NewLine)
        sbSelect.Append("		</select>" + Environment.NewLine)
        sbSelect.Append("		<input type=""button"" value=""Search"" id=""btnSearch" + m_uniqueId + """ name=""btnSearch" + m_uniqueId + """ onclick=""GetCommunityMsgObject('" + m_uniqueId + "').MsgTarg_Search('','');"">" + Environment.NewLine)
        sbSelect.Append("	</div>" + Environment.NewLine)

        If m_user_list IsNot Nothing AndAlso m_user_list.Length > 0 Then
            sbSelect.Append("	<div class=""EktMsgTargetsTopControlsSelectAll"" >" + Environment.NewLine)
            ' sbSelect.Append("		<input id=""EktMsgTargets_SelAll" + m_uniqueId + """ class=""EktMsgTargetCtlSelAll"" name=""EktMsgTargets_SelAll" + m_uniqueId + """ onclick=""GetCommunityMsgObject('" + m_uniqueId + "').MsgToggleSelectAllTarget(this)"" title=""" + m_refMsg.GetMessage("generic select all shown msg") + """ type=""checkbox"" >" + m_refMsg.GetMessage("generic select all shown msg") + "</input>" + Environment.NewLine)
            sbSelect.Append("	</div>" + Environment.NewLine)
            sbSelect.Append("	</div>" + Environment.NewLine)

            sbSelect.Append("	<div class=""EktMsgTargetsMiddle"" >" + Environment.NewLine)
            sbSelect.Append("	<table class=""EktMsgTargetTable"">" + Environment.NewLine)
            sbSelect.Append("		<tbody>" + Environment.NewLine)
            For idx As Integer = 0 To (m_user_list.Length - 1)
                DisplayName = IIf(m_user_list(idx).DisplayName.Trim().Length > 0, m_user_list(idx).DisplayName.Trim(), m_user_list(idx).DisplayUserName.Trim())
                sbSelect.Append("			<tr>" + Environment.NewLine)
                sbSelect.Append("				<td class=""EktMsgTargetTableDataSelect"">" + Environment.NewLine)
                sbSelect.Append("					<input type=""radio"" id=""cb_EktMsgTarget_" + m_uniqueId + "_" + m_user_list(idx).Id.ToString() + """ name=""cb_EktMsgTarget_" + m_uniqueId + "_"" title=""" + DisplayName + """ onclick=""GetCommunityMsgObject('" + m_uniqueId + "').MsgUpdateSelectedUser('" + m_user_list(idx).Id.ToString() + "','" + DisplayName + "',this.checked)"" />" + Environment.NewLine)
                sbSelect.Append("				</td>" + Environment.NewLine)
                sbSelect.Append("				<td class=""EktMsgTargetTableDataAvatar""><div class=""EktMsgTargetTableData_AvatarContainer"">" + Environment.NewLine)
                sbSelect.Append("					<img src=""" + IIf(m_user_list(idx).Avatar <> "", AppendSitePathIfNone(m_user_list(idx).Avatar), m_refContentApi.AppImgPath & "who.jpg") + """ alt=""avatar"" title=""avatar_title"" />" + Environment.NewLine)
                sbSelect.Append("				</div></td>" + Environment.NewLine)
                sbSelect.Append("				<td class=""EktMsgTargetTableDataMember"">" + Environment.NewLine)
                sbSelect.Append("					" + DisplayName + "<br />(" + m_user_list(idx).FirstName + " " + m_user_list(idx).LastName + ")" + Environment.NewLine)
                sbSelect.Append("				</td>" + Environment.NewLine)
                sbSelect.Append("			</tr>" + Environment.NewLine)
            Next
            sbSelect.Append("		</tbody>" + Environment.NewLine)
            sbSelect.Append("	</table>" + Environment.NewLine)
            sbSelect.Append("	</div>" + Environment.NewLine)
        Else
            sbSelect.Append("	<span class=""EktMsgTargetsNoResults"" >" + IIf(m_friendsOnly, m_refMsg.GetMessage("friend search empty result"), m_refMsg.GetMessage("user search empty result")) + "</span>" + Environment.NewLine)
            sbSelect.Append("	</div>" + Environment.NewLine)
        End If
        sbSelect.Append("	<div class=""EktMsgTargetsBtmControls"">" + Environment.NewLine)

        sbSelect.Append("<span class=""EktMsgTargetsPagePreviousBtn"" >" + Environment.NewLine)
        If (m_recipientsPage > 1) Then
            sbSelect.Append("		<input type=""image"" src=""" + GetUserAPI().AppImgPath + "but_prev.gif"" onclick=""GetCommunityMsgObject('" + m_uniqueId + "').MsgTarg_PrevPage" + "(); return false;"" />" + Environment.NewLine)
        Else
            sbSelect.Append("		<img src=""" + GetUserAPI().AppImgPath + "but_prev_d.gif"" />" + Environment.NewLine)
        End If
        sbSelect.Append("</span>" + Environment.NewLine)

        sbSelect.Append("<span class=""EktMsgTargetsPageNextBtn"" >" + Environment.NewLine)
        If (m_recipientsPage < m_intTotalPages) Then
            sbSelect.Append("		<input type=""image"" src=""" + GetUserAPI().AppImgPath + "but_next.gif"" onclick=""GetCommunityMsgObject('" + m_uniqueId + "').MsgTarg_NextPage" + "(); return false;"" />" + Environment.NewLine)
        Else
            sbSelect.Append("		<img src=""" + GetUserAPI().AppImgPath + "but_next_d.gif"" />" + Environment.NewLine)
        End If
        sbSelect.Append("</span>" + Environment.NewLine)

        sbSelect.Append("		<input type=""button"" title=""" + m_refMsg.GetMessage("btn done") + """ value=""" + m_refMsg.GetMessage("btn done") + """ onclick=""GetCommunityMsgObject('" + m_uniqueId + "').MsgSaveMessageTargetUI()"" class=""EktMsgTargetsDoneBtn"" />" + Environment.NewLine)
        sbSelect.Append("		<input type=""button"" title=""" + m_refMsg.GetMessage("btn cancel") + """ value=""" + m_refMsg.GetMessage("btn cancel") + """ onclick=""GetCommunityMsgObject('" + m_uniqueId + "').MsgCancelMessageTargetUI()"" class=""EktMsgTargetsCancelBtn"" />" + Environment.NewLine)
        sbSelect.Append("		<input id=""RecipientsPage" + m_uniqueId + """ type=""hidden"" value=""" + m_recipientsPage.ToString() + """ />" + Environment.NewLine)
        sbSelect.Append("	</div>" + Environment.NewLine)

        result = sbSelect.ToString()
        sbSelect = Nothing
        Return (result)
    End Function
    Protected Function AppendSitePathIfNone(ByVal avatar As String) As String
        If (avatar.Trim().IndexOf(m_refContentApi.SitePath) < 0) AndAlso (("/" & avatar).Trim().IndexOf(m_refContentApi.SitePath) < 0) Then
            avatar = m_refContentApi.SitePath & avatar
        End If
        Return avatar
    End Function
    Protected Function IsUserSelected(ByVal id As Long) As String
        Dim result As String = "false"
        Return (result)
    End Function
#End Region

#Region "ICallBackEventHandler"
    Public Function GetCallbackResult() As String Implements System.Web.UI.ICallbackEventHandler.GetCallbackResult
        Return (m_callbackresult)
    End Function

    Public Sub RaiseCallbackEvent(ByVal eventArgs As String) Implements System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent
        Dim postBackData As System.Collections.Specialized.NameValueCollection = Nothing
        Dim recipientsPageStr As String = "1"

        m_callbackresult = ""
        postBackData = New System.Collections.Specialized.NameValueCollection()
        postBackData = System.Web.HttpUtility.ParseQueryString(eventArgs)

        If (Not IsNothing(postBackData("__searchtext"))) Then
            m_strKeyWords = postBackData("__searchtext")
        End If

        If (Not IsNothing(postBackData("__searchmode"))) Then
            m_searchMode = postBackData("__searchmode")
        End If

        If ((Not IsNothing(postBackData("__targpage"))) AndAlso IsNumeric(postBackData("__targpage").Trim())) Then
            recipientsPageStr = postBackData("__targpage").Trim()
            m_recipientsPage = IIf(IsNumeric(recipientsPageStr) AndAlso (CType(recipientsPageStr, Integer) > 0), CType(recipientsPageStr, Integer), 1)
        End If


        CollectSearchText()
        GetUsers()
        m_callbackresult = BuildRecipientSelect()

    End Sub

#End Region

End Class
