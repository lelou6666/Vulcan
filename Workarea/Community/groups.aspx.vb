Imports Ektron.Cms
Imports Ektron.Cms.Workarea
Imports System.Data
Imports Ektron.Cms.CommonApi
Imports System.Collections.Generic
Imports Ektron.Cms.Common.EkEnumeration
Partial Class Community_groups
    Inherits workareabase

    Protected cgGroup As New Ektron.Cms.CommunityGroupData
    Protected bAccess As Boolean = False
    Protected bAddAccess As Boolean = False
    Protected sSearch As String = ""
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 0
    Protected objLocalizationApi As New LocalizationAPI()
    Private _CalendarApi As Ektron.Cms.Content.Calendar.WebCalendar = New Ektron.Cms.Content.Calendar.WebCalendar(m_refContentApi.RequestInformationRef)
    Dim calendardata As New Ektron.Cms.Common.Calendar.WebCalendarData
    Protected _doesForumExists As Long = -1
    Protected groupAliasList As String = String.Empty


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        RegisterResources()
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' testLit1.Text = m_sPageAction
            Utilities.ValidateUserLogin()
            CheckAccess()
            If Not bAccess Then
                Throw New Exception(Me.GetMessage("err communityaddedit no access"))
            End If
            Select Case m_sPageAction
                Case "viewgroup"
                    ViewGroup()

                Case "delete"
                    Process_DeleteGroup()

                Case "addeditgroup"
                    If Page.IsPostBack Then
                        Process_EditGroup()
                    Else
                        EditGroup()
                    End If

                Case Else ' "viewallgroups"
                    If (Not Page.IsPostBack) Or (Page.IsPostBack AndAlso Request.Form("hdn_search") <> "") Then
                        ViewAllGroups() ' default to view all groups.
                    End If
            End Select
        Catch ex As Exception
            Utilities.ShowError(ex.Message & ex.StackTrace)
        End Try

    End Sub

    Private Sub RegisterResources()
        'CSS
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronUITabsCss)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        'JS
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
        API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/workareahelper.js", "EktronWorkareaHelperJS")
    End Sub

    Protected Sub ViewAllGroups()
        BuildJS()
        If Page.IsPostBack Then
            sSearch = Request.Form("txtSearch")
        End If
        If (Request.QueryString("page") <> "") Then
            m_intCurrentPage = Convert.ToInt32(Request.QueryString("page"))
        End If
        Dim aCGroups() As CommunityGroupData = Array.CreateInstance(GetType(CommunityGroupData), 0)

        panel1.Visible = True
        SetTitleBarToMessage("lbl view all cgroups")
        bAddAccess = (Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupCreate))
        If (bAddAccess) Then
            MyBase.AddButtonwithMessages(AppImgPath & "../UI/Icons/add.png", "../communitygroupaddedit.aspx?action=addeditgroup&LangType=" & Me.ContentLanguage, "alt add community group", "lbl add community group", "")
        End If
        AddSearchBox(sSearch, New ListItemCollection, "ExecSearch")
        AddHelpButton("viewallcommunitygroups")

        Dim cReq As New CommunityGroupRequest
        cReq.CurrentPage = m_intCurrentPage
        cReq.SearchText = sSearch
        cReq.PageSize = Me.m_refContentApi.RequestInformationRef.PagingSize
        aCGroups = Me.m_refCommunityGroupApi.GetAllCommunityGroups(cReq)

        ' CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("CHECK", "<input type=""Checkbox"" name=""checkall"" onclick=""javascript:checkAll('selected_communitygroup',false);"">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(2), Unit.Percentage(2), False, False))
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("TITLE", GetMessage("lbl community group name"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), False, False))
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("MEMBERS", GetMessage("lbl members"), "title-header", HorizontalAlign.Right, HorizontalAlign.Right, Unit.Percentage(5), Unit.Percentage(5), False, False))
        'CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("LANGUAGE", GetMessage("generic language"), "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(5), Unit.Percentage(5), False, False))
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("ID", GetMessage("generic ID"), "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(5), Unit.Percentage(5), False, False))
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("ENROLL", GetMessage("lbl enrollment"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), False, False))
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("DESCRIPTION", GetMessage("lbl discussionforumtitle"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), False, False))
        CommunityGroupList.Columns.Add(m_refStyle.CreateBoundField("LOCATION", GetMessage("generic location"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), False, False))

        Dim dt As New DataTable
        Dim dr As DataRow
        ' dt.Columns.Add(New DataColumn("CHECK", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("MEMBERS", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        'dt.Columns.Add(New DataColumn("LANGUAGE", GetType(String)))
        dt.Columns.Add(New DataColumn("ENROLL", GetType(String)))
        dt.Columns.Add(New DataColumn("DESCRIPTION", GetType(String)))
        dt.Columns.Add(New DataColumn("LOCATION", GetType(String)))
        m_intTotalPages = cReq.TotalPages
        PageSettings()
        If (aCGroups IsNot Nothing AndAlso aCGroups.Length > 0) Then
            ' AddDeleteIcon = True
            For i As Integer = 0 To aCGroups.Length - 1
                If m_refContentApi.RequestInformationRef.IsMembershipUser Then
                    If m_refContentApi.UserId <> aCGroups(i).GroupAdmin.Id Then
                        Continue For
                    End If
                End If
                dr = dt.NewRow
                ' dr("CHECK") = "<input type=""checkbox"" name=""selected_communitygroup"" id=""selected_communitygroup"" value=""" & aCGroups(i).GroupId & """ onClick=""javascript:checkAll('selected_communitygroup',true);"">"
                'dr("TITLE") = "<a href=""groups.aspx?action=viewgroup&id=" & aCGroups(i).GroupId & "&LangType=" & aCGroups(i).GroupLanguage & """>" & aCGroups(i).GroupName & "</a>"
                dr("TITLE") = "<a href=""groups.aspx?action=viewgroup&id=" & aCGroups(i).GroupId & """>" & aCGroups(i).GroupName & "</a>"
                dr("MEMBERS") = aCGroups(i).TotalMember
                dr("ID") = aCGroups(i).GroupId
                'dr("LANGUAGE") = "<img src='" & objLocalizationApi.GetFlagUrlByLanguageID(aCGroups(i).GroupLanguage) & "' border=""0"" />"
                'dr("LANGUAGE") = "<img src='" & objLocalizationApi.GetFlagUrlByLanguageID(aCGroups(i).GroupLanguage) & "' border=""0"" />"
                dr("ENROLL") = IIf(aCGroups(i).GroupEnroll, "Open", "Closed")
                dr("DESCRIPTION") = aCGroups(i).GroupShortDescription
                dr("LOCATION") = aCGroups(i).GroupLocation
                dt.Rows.Add(dr)
            Next
        Else
            dr = dt.NewRow
            dt.Rows.Add(dr)
            CommunityGroupList.GridLines = GridLines.None
        End If
        Dim dv As New DataView(dt)
        CommunityGroupList.DataSource = dv
        CommunityGroupList.DataBind()
    End Sub

    Protected Sub ViewGroup()
        Dim m_aCategories() As DirectoryData = Array.CreateInstance(GetType(Ektron.Cms.DirectoryData), 0)
        cgGroup = Me.m_refCommunityGroupApi.GetCommunityGroupByID(Me.m_iID)
        If m_refContentApi.RequestInformationRef.IsMembershipUser AndAlso cgGroup.GroupAdmin.Id <> Me.m_refContentApi.UserId Then
            Exit Sub
        End If
        panel3.Visible = True
        SetLabels()
        m_aCategories = Me.m_refContentApi.EkContentRef.GetAllAssignedDirectory(Me.m_iID, TaxonomyItemType.Group)
        lbl_id.Text = cgGroup.GroupId
        SetTitleBarToMessage("lbl view cgroup")
        PopulateData(cgGroup, m_aCategories)
        TD_personalTags.InnerHtml = GetGroupTags()
        bAccess = (Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) OrElse cgGroup.GroupAdmin.Id = Me.m_refContentApi.UserId)
        ' buttons
        If Me.bAccess = True Then
            MyBase.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/contentEdit.png", "../communitygroupaddedit.aspx?action=addeditgroup&LangType=" & Me.ContentLanguage & "&id=" & cgGroup.GroupId.ToString(), "alt edit community group", "lbl edit community group", "")
            ' MyBase.AddButtonwithMessages(AppImgPath & "menu/folders.gif", "workspace.aspx?groupid=" & Me.m_iID & "&LangType=" & ContentLanguage, "alt view group directory", "btn view group directory", "")
            MyBase.AddButtonwithMessages(m_refContentApi.AppPath & "images/ui/icons/usersMemberGroups.png", "groupmembers.aspx?action=viewallusers&LangType=" & ContentLanguage & "&id=" & Me.m_iID, "alt view cgroup members", "btn view cgroup members", "")
            MyBase.AddButtonwithMessages(m_refContentApi.AppPath & "images/UI/Icons/delete.png", "groups.aspx?action=delete&id=" & cgGroup.GroupId.ToString(), "alt del community group", "lbl del community group", " onclick=""javascript:return confirm('" & GetMessage("js confirm del community group") & "');"" ")
        End If
        SetAlias(Me.m_iID)
        MyBase.AddBackButton("groups.aspx")
        AddHelpButton("viewcommunitygroup")
    End Sub

    Protected Sub EditGroup()
        BuildJS()
        SetLabels()
        panel3.Visible = True
        If Me.m_iID > 0 Then
            cgGroup = Me.m_refCommunityGroupApi.GetCommunityGroupByID(Me.m_iID)
            lbl_id.Text = cgGroup.GroupId
            SetTitleBarToMessage("lbl edit cgroup")
            ' PopulateData(cgGroup)
            MyBase.AddButtonwithMessages(m_refContentApi.AppImgPath & "../UI/Icons/save.png", "#", "lbl alt save", "btn save", " onclick=""javascript: SubmitForm(); return false;"" ")
            ' AddBackButton("groups.aspx?action=viewgroup&id=" & cgGroup.GroupId.ToString() & "&LangType=" & cgGroup.GroupLanguage & "")
            AddBackButton("groups.aspx?action=viewgroup&id=" & cgGroup.GroupId.ToString() & "")
            AddHelpButton("editcommunitygroup")
            SetAlias(Me.m_iID)
        Else
            Me.PublicJoinYes_RB.Checked = True
            tr_ID.Visible = False
            Me.cmd_browse.Visible = False
            SetTitleBarToMessage("lbl add cgroup")
            MyBase.AddButtonwithMessages(m_refContentApi.AppImgPath & "../UI/Icons/save.png", "#", "lbl alt save", "btn save", " onclick=""javascript: SubmitForm(); return false;"" ")
            AddBackButton("groups.aspx")
            AddHelpButton("addcommunitygroup")
        End If

    End Sub

#Region "Process"

    Protected Sub Process_DeleteGroup()
        cgGroup = Me.m_refCommunityGroupApi.GetCommunityGroupByID(Me.m_iID)
        bAccess = (Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) OrElse cgGroup.GroupAdmin.Id = Me.m_refContentApi.UserId)
        If bAccess Then
            Me.m_refCommunityGroupApi.DeleteCommunityGroupByID(Me.m_iID)

            Response.Redirect("groups.aspx", False)
        Else
            Throw New Exception(GetMessage("err no perm del cgroup"))
        End If
    End Sub

    Protected Sub Process_EditGroup()
        If Me.m_iID > 0 Then
            bAccess = (Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) OrElse cgGroup.GroupAdmin.Id = Me.m_refContentApi.UserId)
        Else
            bAccess = (Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupCreate))
        End If
        If bAccess Then
            If m_iID > 0 Then
                cgGroup = Me.m_refCommunityGroupApi.GetCommunityGroupByID(Me.m_iID)
            Else
                cgGroup = New CommunityGroupData
            End If
            cgGroup.GroupName = Me.GroupName_TB.Text
            cgGroup.GroupShortDescription = Me.ShortDescription_TB.Text
            cgGroup.GroupLongDescription = Me.Description_TB.Text
            cgGroup.GroupEnroll = Me.PublicJoinYes_RB.Checked
            cgGroup.GroupLocation = Me.Location_TB.Text
            cgGroup.GroupEnableDistributeToSite = Me.EnableDistributeToSite_CB.Checked
            cgGroup.AllowMembersToManageFolders = Me.AllowMembersToManageFolders_CB.Checked

            If m_iID > 0 Then
                m_refCommunityGroupApi.UpdateCommunityGroup(cgGroup)
                Response.Redirect("groups.aspx", False)
            Else
                m_iID = m_refCommunityGroupApi.AddCommunityGroup(cgGroup)
                If (m_iID > 0) Then
                    Response.Redirect("groups.aspx", False)
                Else
                    EditGroup()
                    errmsg.InnerHtml = "Error occured while adding this group.  verify the group name is unique and try again, for more details check eventviewer."
                    errmsg.Attributes.Add("class", "exception")
                    GroupName_TB.Attributes.Add("onkeypress", "ClearErr();")
                    GroupName_TB.Focus()
                End If
            End If
        Else
            Throw New Exception(GetMessage("err no perm add cgroup"))
        End If
    End Sub

#End Region

#Region "Helper Functions"

    Protected Sub CheckAccess()
        If Me.m_refContentApi.IsLoggedIn() Then
            If Me.m_iID > 0 And Me.m_sPageAction = "delete" Then
                Dim mMemberStatus As Ektron.Cms.Common.EkEnumeration.GroupMemberStatus
                mMemberStatus = Me.m_refCommunityGroupApi.GetGroupMemberStatus(Me.m_iID, Me.m_refContentApi.UserId())
                bAccess = (Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) OrElse mMemberStatus = Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.Leader)
            Else ' if logged in, can see this
                bAccess = True
            End If
        End If
    End Sub

    Protected Sub BuildJS()
        Dim sbJS As New StringBuilder()

        sbJS.Append("<script language=""javascript"" type=""text/javascript"">").Append(Environment.NewLine)
        sbJS.Append("function SubmitForm() {" & Environment.NewLine)
        sbJS.Append("   var groupName = document.getElementById('GroupName_TB').value;").Append(Environment.NewLine)
        sbJS.Append("   if (groupName == '')").Append(Environment.NewLine)
        sbJS.Append("   {alert('" & GetMessage("lbl please enter group name") & "');").Append(Environment.NewLine)
        sbJS.Append("   return false;}").Append(Environment.NewLine)
        sbJS.Append("else{ " & Environment.NewLine)
        sbJS.Append("   if (!CheckGroupForillegalChar()) {" & Environment.NewLine)
        sbJS.Append("   		return false;" & Environment.NewLine)
        sbJS.Append("   } else { document.forms[0].submit(); }" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)
        sbJS.Append("}" & Environment.NewLine)

        sbJS.Append("function ExecSearch() {" & Environment.NewLine)
        sbJS.Append("   var sTerm = $ektron('#txtSearch').getInputLabelValue();" & Environment.NewLine)
        sbJS.Append("   document.getElementById('hdn_search').value = true;" & Environment.NewLine)
        sbJS.Append("   $ektron('#txtSearch').clearInputLabel();" & Environment.NewLine)
        sbJS.Append("	document.forms[0].submit();" & Environment.NewLine)
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
        sbJS.Append("  				window.location.href=""groups.aspx?action=viewallgroups""+""&LangType=""+document.forms[FormName].selLang.options[num].value; ").Append(Environment.NewLine)
        sbJS.Append("  				//document.forms[FormName].submit(); ").Append(Environment.NewLine)
        sbJS.Append("  				return false; ").Append(Environment.NewLine)
        sbJS.Append("  			} ").Append(Environment.NewLine)

        sbJS.Append("</script>").Append(Environment.NewLine)

        ltr_js.Text = sbJS.ToString()
    End Sub

    Protected Sub SetLabels()
        Me.ltr_groupname.Text = GetMessage("lbl community group name")
        Me.ltr_groupid.Text = GetMessage("generic id")
        Me.ltr_admin.Text = GetMessage("lbl administrator")
        Me.ltr_groupjoin.Text = GetMessage("lbl enrollment")
        Me.ltr_groupavatar.Text = GetMessage("lbl group image")
        Me.ltr_grouplocation.Text = GetMessage("generic location")
        Me.ltr_groupsdesc.Text = GetMessage("lbl short desc")
        Me.ltr_groupdesc.Text = GetMessage("generic description")
        Me.ltr_enabledistribute.Text = GetMessage("lbl enable distribute")
        Me.cmd_browse.Text = GetMessage("btn browse")
        PublicJoinYes_RB.Text = GetMessage("lbl enrollment open")
        PublicJoinNo_RB.Text = GetMessage("lbl enrollment restricted")
        Me.ltr_AllowMembersToManageFolders.Text = GetMessage("lbl allow member to manage folders")
        Me.ltr_groupfeatures.Text = GetMessage("lbl features") & ":"
        FeaturesCalendar_CB.Text = GetMessage("lbl enable group calendar")
        FeaturesForum_CB.Text = GetMessage("lbl enable group forum")
    End Sub

    Protected Sub PopulateData(ByVal cGrp As CommunityGroupData, ByVal aCategories As DirectoryData())
        Me.GroupName_TB.Text = cGrp.GroupName
        Me.ShortDescription_TB.Text = cGrp.GroupShortDescription
        Me.Description_TB.Text = cGrp.GroupLongDescription
        Me.PublicJoinYes_RB.Checked = cGrp.GroupEnroll
        Me.PublicJoinNo_RB.Checked = Not (cGrp.GroupEnroll)
        Me.Location_TB.Text = cGrp.GroupLocation
        Me.ltr_admin_name.Text = cGrp.GroupAdmin.DisplayName
        Me.GroupAvatar_TB.Text = cGrp.GroupImage
        Me.cmd_browse.Attributes.Add("onclick", "javascript:return false;")
        Me.cmd_browse.Visible = False
        Me.EnableDistributeToSite_CB.Checked = cGrp.GroupEnableDistributeToSite
        Me.AllowMembersToManageFolders_CB.Checked = cGrp.AllowMembersToManageFolders
        Me.ltr_avatarpath.Text = ""
        Dim cat_list As List(Of String) = New List(Of String)
        Dim TaxonomyList As String = String.Empty
        If aCategories IsNot Nothing AndAlso aCategories.Length > 0 Then
            For i As Integer = 0 To (aCategories.Length - 1)
                cat_list.Add(("<li>" & aCategories(i).DirectoryPath.Remove(0, 1).Replace("\", " > ") & "</li>"))
            Next
            TaxonomyList = String.Join(String.Empty, cat_list.ToArray())
        Else
            TaxonomyList = GetMessage("lbl cgroup no cat")
        End If
        ltr_cat.Text &= TaxonomyList
        calendardata = _CalendarApi.GetPublicCalendar(WorkSpace.Group, cGrp.GroupId)
        If calendardata IsNot Nothing Then
            Me.FeaturesCalendar_CB.Checked = True
        End If
        _doesForumExists = m_refCommunityGroupApi.DoesCommunityGroupForumExists(WorkSpace.Group, cGrp.GroupId)
        If _doesForumExists <> -1 Then
            Me.FeaturesForum_CB.Checked = True
        End If
        If Me.m_sPageAction = "viewgroup" Then
            Me.GroupName_TB.Enabled = False
            Me.ShortDescription_TB.Enabled = False
            Me.Description_TB.Enabled = False
            Me.PublicJoinYes_RB.Enabled = False
            Me.PublicJoinNo_RB.Enabled = False
            Me.Location_TB.Enabled = False
            Me.GroupAvatar_TB.Enabled = False
            Me.EnableDistributeToSite_CB.Enabled = False
            Me.AllowMembersToManageFolders_CB.Enabled = False
            Me.FeaturesCalendar_CB.Enabled = False
            Me.FeaturesForum_CB.Enabled = False
        ElseIf Me.m_sPageAction = "addeditgroup" Then
            If Me.m_refContentApi.IsAdmin = False Then
                Me.AllowMembersToManageFolders_CB.Enabled = False
            End If
        End If
    End Sub

    Private Sub PageSettings()
        If (m_intTotalPages <= 1) Then
            VisiblePageControls(False)
        Else
            VisiblePageControls(True)
            TTotalPages.Text = (System.Math.Ceiling(m_intTotalPages)).ToString()
            TCurrentPage.Text = m_intCurrentPage.ToString()
            TPreviousPage.Enabled = True
            TFirstPage.Enabled = True
            TNextPage.Enabled = True
            TLastPage.Enabled = True
            If m_intCurrentPage = 1 Then
                TPreviousPage.Enabled = False
                TFirstPage.Enabled = False
            ElseIf m_intCurrentPage = m_intTotalPages Then
                TNextPage.Enabled = False
                TLastPage.Enabled = False
            End If
        End If
    End Sub
    Private Sub VisiblePageControls(ByVal flag As Boolean)
        TTotalPages.Visible = flag
        TCurrentPage.Visible = flag
        TPreviousPage.Visible = flag
        TNextPage.Visible = flag
        TLastPage.Visible = flag
        TFirstPage.Visible = flag
        TPageLabel.Visible = flag
        TOfLabel.Visible = flag
    End Sub
    Protected Sub TNavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                m_intCurrentPage = 1
            Case "Last"
                m_intCurrentPage = Int32.Parse(TTotalPages.Text)
            Case "Next"
                m_intCurrentPage = Int32.Parse(TCurrentPage.Text) + 1
            Case "Prev"
                m_intCurrentPage = Int32.Parse(TCurrentPage.Text) - 1
        End Select
        ViewAllGroups()
        isPostData.Value = "true"
    End Sub
#End Region

    Private Sub SetAlias(ByVal groupId As Long)
        Dim _communityAlias As New Ektron.Cms.API.UrlAliasing.UrlAliasCommunity
        Dim aliasList As System.Collections.Generic.List(Of Ektron.Cms.Common.UrlAliasCommunityData)

        aliasList = _communityAlias.GetListGroup(groupId)
        If (aliasList.Count > 0) Then
            For Each item As Ektron.Cms.Common.UrlAliasCommunityData In aliasList
                groupAliasList += "<a href= " & Me.m_refContentApi.sitepath & item.AliasName & " target=_blank>" & Me.m_refContentApi.sitepath & item.AliasName & "</a>"
                groupAliasList += "<br/>"
            Next
        Else
            phAliasTab.Visible = False
            phAliasFrame.Visible = False
        End If
    End Sub
#Region "Group Tags"
    Public Function GetGroupTags() As String
        Dim result As New System.Text.StringBuilder
        Dim tdaGroup() As TagData
        Dim tdaAll() As TagData
        Dim td As TagData
        Dim htTagsAssignedToUser As Hashtable
        Dim m_refUserApi As New UserAPI()

        Try
            htTagsAssignedToUser = New Hashtable
            result.Append("<fieldset>")
            result.Append("<legend>" + m_refMsg.GetMessage("lbl group tags") + "</legend>")
            result.Append("<div style=""overflow: auto; height: 80px;"">")

            If (Me.m_iID > 0) Then
                tdaGroup = m_refTagsApi.GetTagsForObject(Me.m_iID, Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.CommunityGroup, -1)
                If tdaGroup.Length > 0 Then
                    Dim localizationApi As New LocalizationAPI()
                    For i As Integer = 0 To (tdaGroup.Length - 1)
                        result.Append("<input disabled=""disabled"" checked=""checked"" type=""checkbox"">&nbsp;<img src='" & localizationApi.GetFlagUrlByLanguageID(tdaGroup(i).LanguageId) & "' border=""0"" />&nbsp;" & tdaGroup(i).Text & "<br>")
                    Next
                End If
            End If
            result.Append("</div>")
            result.Append("</fieldset>")

        Catch ex As Exception
        Finally
            GetGroupTags = result.ToString
            tdaAll = Nothing
            tdaGroup = Nothing
            td = Nothing
            htTagsAssignedToUser = Nothing
        End Try
    End Function
#End Region

End Class
