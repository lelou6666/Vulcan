Imports System.Data
Imports Ektron.Cms

Partial Class adreports
    Inherits System.Web.UI.Page

    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected CmsUserIcon As String = ""
    Protected MemberShipUserIcon As String = ""
    Protected CmsGroupIcon As String = ""
    Protected MemberShipGroupIcon As String = ""
    Protected settings_data As SettingsData
    Protected m_refSiteApi As SiteAPI
    Protected m_refUserApi As UserAPI
    Protected sync_data As AdSyncData
    Protected m_intMax As Integer = 5
    Protected m_strPageAction As String = ""
    Const INPUTCLASS As String = "ektronTextXXSmall"

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1
        Try
            'Put user code to initialize the page here
            TR_count.Visible = False
            m_refSiteApi = New SiteAPI
            m_refUserApi = New UserAPI
            m_refMsg = m_refSiteApi.EkMsgRef
            AppImgPath = m_refUserApi.AppImgPath
            CmsUserIcon = "<img src=""" & AppImgPath & "../UI/Icons/user.png"" valign=""absbottom"" title=""CMS User"">"
            MemberShipUserIcon = "<img src=""" & AppImgPath & "../UI/Icons/userMembership.png"" valign=""absbottom"" title=""MemberShip User"">"
            CmsGroupIcon = "<img src=""" & AppImgPath & "../UI/Icons/users.png"" valign=""absbottom"" title=""CMS Group"">"
            MemberShipGroupIcon = "<img src=""" & AppImgPath & "../UI/Icons/usersMembership.png"" valign=""absbottom"" title=""MemberShip User"">"
            AppPath = m_refUserApi.AppPath
            CmsUserIcon = "<img src=""" & AppImgPath & "../UI/Icons/user.png"" valign=""absbottom"" title=""CMS User"">"
            MemberShipUserIcon = "<img src=""" & AppImgPath & "../UI/Icons/userMembership.png"" valign=""absbottom"" title=""MemberShip User"">"
            CmsGroupIcon = "<img src=""" & AppImgPath & "../UI/Icons/users.png"" valign=""absbottom"" title=""CMS Group"">"
            MemberShipGroupIcon = "<img src=""" & AppImgPath & "../UI/Icons/usersMembership.png"" valign=""absbottom"" title=""MemberShip User"">"
            CmsUserIcon = "<img src=""" & AppImgPath & "../UI/Icons/user.png"" valign=""absbottom"" title=""CMS User"">"
            MemberShipUserIcon = "<img src=""" & AppImgPath & "../UI/Icons/userMembership.png"" valign=""absbottom"" title=""MemberShip User"">"
            CmsGroupIcon = "<img src=""" & AppImgPath & "../UI/Icons/users.png"" valign=""absbottom"" title=""CMS Group"">"
            MemberShipGroupIcon = "<img src=""" & AppImgPath & "../UI/Icons/usersMembership.png"" valign=""absbottom"" title=""MemberShip User"">"
            If (Not (IsNothing(Request.QueryString("action")))) Then
                If (Request.QueryString("action") <> "") Then
                    m_strPageAction = Request.QueryString("action").ToLower
                End If
            End If
            StyleSheetJS.Text = m_refStyle.GetClientScript
            RegisterResources()

            If (Not (Page.IsPostBack)) Then
                Select Case m_strPageAction
                    Case "desynchgroups"
                        DeSynchGroups()
                    Case "desynchusers"
                        DeSynchUsers()
                    Case "viewallreporttypes"
                        ViewAllReportTypes()
                    Case "getusersforsynch"
                        GetUsersForSync()
                    Case "getgroupsforsynch"
                        GetGroupsForSync()
                    Case "getrelationshipsforsynch"
                        GetRelationshipsForSync()
                End Select
            Else
                Select Case m_strPageAction
                    Case "getusersforsynch"
                        Process_SynchCMSUsersToAD()
                    Case "getgroupsforsynch"
                        Process_SynchCMSGroupsToAD()
                    Case "getrelationshipsforsynch"
                        Process_SynchCMSRelationShipsToAD()
                    Case "desynchusers"
                        Process_DeSynchUsers()
                End Select
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub DeSynchGroups()
        Try
            TR_count.Visible = True
            Dim usersForm As String = Request.Form("submitted")
            Dim result As AdDeSyncGroupData()
            m_refSiteApi = New SiteAPI
            m_refUserApi = New UserAPI
            If (usersForm <> "") Then
                result = m_refUserApi.DeSynchUserGroups(True)
                Response.Redirect("adreports.aspx?action=ViewAllReportTypes", False)

            Else
                settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)

                result = m_refUserApi.DeSynchUserGroups(False)

                If (IsNothing(result)) Then
                    Response.Redirect("adreports.aspx?action=ViewAllReportTypes", False)
                End If
                Dim colBound As New System.Web.UI.WebControls.BoundColumn
                colBound.DataField = "CMSUSER"
                colBound.HeaderText = m_refMsg.GetMessage("generic User Group Name")
                colBound.HeaderStyle.Width = Unit.Percentage(30)
                colBound.ItemStyle.Wrap = False
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
                AdReportsGrid.Columns.Add(colBound)

                colBound = New System.Web.UI.WebControls.BoundColumn
                colBound.DataField = "AT"
                colBound.HeaderText = "@"
                colBound.HeaderStyle.Width = Unit.Percentage(2)
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                colBound.ItemStyle.Wrap = False
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
                AdReportsGrid.Columns.Add(colBound)

                colBound = New System.Web.UI.WebControls.BoundColumn
                colBound.DataField = "TITLE"
                colBound.HeaderText = m_refMsg.GetMessage("domain title")
                colBound.HeaderStyle.Width = Unit.Percentage(30)
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                colBound.ItemStyle.Wrap = False
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
                AdReportsGrid.Columns.Add(colBound)

                colBound = New System.Web.UI.WebControls.BoundColumn
                colBound.DataField = "USER"
                colBound.HeaderText = m_refMsg.GetMessage("unique group name")
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                colBound.ItemStyle.Wrap = False
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
                AdReportsGrid.Columns.Add(colBound)

                Dim dt As New DataTable
                Dim dr As DataRow
                dt.Columns.Add(New DataColumn("CMSUSER", GetType(String)))
                dt.Columns.Add(New DataColumn("AT", GetType(String)))
                dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
                dt.Columns.Add(New DataColumn("USER", GetType(String)))

                Dim i As Integer = 0
                Dim arrayCount As Integer = 0
                Dim currentUserID As Long = m_refUserApi.UserId
                If (Not (settings_data.ADAuthentication = 1)) Then
                    For i = 0 To result.Length - 1
                        dr = dt.NewRow
                        dr(0) = result(i).OldGroupName
                        dr(1) = "@"
                        dr(2) = result(i).OldGroupDomain
                        dr(3) = result(i).NewGroupName
                        dt.Rows.Add(dr)
                    Next

                    AdReportsGrid.ShowFooter = True

                End If
                usercount.Value = i + 1
                Dim dv As New DataView(dt)
                AdReportsGrid.DataSource = dv
                AdReportsGrid.DataBind()
            End If

            DeSynchGroupsToolBar()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub
    Private Sub DeSynchGroupsToolBar()
        Dim result As New System.Text.StringBuilder
        Try
            result.Append("<table><tr>")
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("make groups unique"))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (groups)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('desynchgroups', '');"""))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            result.Append("<td>" & m_refStyle.GetHelpButton(m_strPageAction) & "</td>")
            result.Append("</tr></table>")
            divToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub DeSynchUsers()
        Try
            TR_count.Visible = True
            Dim usersForm As String = Request.Form("submitted")
            Dim result As AdDeSyncUserData()
            m_refSiteApi = New SiteAPI
            m_refUserApi = New UserAPI
            If (usersForm <> "") Then
                result = m_refUserApi.DeSynchUsers(True)
                Response.Redirect("adreports.aspx?action=ViewAllReportTypes", False)

            Else
                settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)

                result = m_refUserApi.DeSynchUsers(False)

                If (IsNothing(result)) Then
                    Response.Redirect("adreports.aspx?action=ViewAllReportTypes", False)
                End If
                Dim colBound As New System.Web.UI.WebControls.BoundColumn
                colBound.DataField = "CMSUSER"
                colBound.HeaderText = m_refMsg.GetMessage("generic Username")
                colBound.HeaderStyle.Width = Unit.Percentage(25)
                colBound.ItemStyle.Wrap = False
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
                AdReportsGrid.Columns.Add(colBound)

                colBound = New System.Web.UI.WebControls.BoundColumn
                colBound.DataField = "AT"
                colBound.HeaderText = "@"
                colBound.HeaderStyle.Width = Unit.Percentage(2)
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                colBound.ItemStyle.Wrap = False
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
                AdReportsGrid.Columns.Add(colBound)

                colBound = New System.Web.UI.WebControls.BoundColumn
                colBound.DataField = "TITLE"
                colBound.HeaderText = m_refMsg.GetMessage("domain title")
                colBound.HeaderStyle.Width = Unit.Percentage(25)
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                colBound.ItemStyle.Wrap = False
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
                AdReportsGrid.Columns.Add(colBound)

                colBound = New System.Web.UI.WebControls.BoundColumn
                colBound.DataField = "USER"
                colBound.HeaderText = m_refMsg.GetMessage("unique username")
                colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                colBound.ItemStyle.Wrap = False
                colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
                AdReportsGrid.Columns.Add(colBound)

                Dim dt As New DataTable
                Dim dr As DataRow
                dt.Columns.Add(New DataColumn("CMSUSER", GetType(String)))
                dt.Columns.Add(New DataColumn("AT", GetType(String)))
                dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
                dt.Columns.Add(New DataColumn("USER", GetType(String)))

                Dim i As Integer = 0
                Dim arrayCount As Integer = 0
                Dim currentUserID As Long = m_refUserApi.UserId

                If (Not (settings_data.ADAuthentication = 1)) Then
                    For i = 0 To result.Length - 1
                        dr = dt.NewRow
                        dr(0) = result(i).OldUserName
                        dr(1) = "@"
                        dr(2) = result(i).OldUserDomain
                        dr(3) = result(i).NewUserName
                        dt.Rows.Add(dr)
                    Next

                    AdReportsGrid.ShowFooter = True

                End If
                usercount.Value = i + 1
                Dim dv As New DataView(dt)
                AdReportsGrid.DataSource = dv
                AdReportsGrid.DataBind()
            End If
            DeSynchUsersToolBar()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub
    Private Sub DeSynchUsersToolBar()
        Dim result As New System.Text.StringBuilder
        Try
            result.Append("<table><tr>")
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("make users unique"))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (users)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('desynchusers', '');"""))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            result.Append("<td>" & m_refStyle.GetHelpButton(m_strPageAction) & "</td>")
            result.Append("</tr></table>")
            divToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub GetRelationshipsForSync()
        Try
            TR_count.Visible = True
            Dim user_group_data As UserGroupData()
            If (Not (Request.QueryString("max"))) Then
                If (Request.QueryString("max") <> "") Then
                    m_intMax = Request.QueryString("max")
                End If
            End If
            m_refSiteApi = New SiteAPI
            m_refUserApi = New UserAPI
            settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
            user_group_data = m_refUserApi.GetCMSRelationshipsToSync(m_intMax)
            If user_group_data Is Nothing Then user_group_data = Array.CreateInstance(GetType(UserGroupData), 0)

            If (IsNothing(user_group_data)) Then
                Response.Redirect("adreports.aspx?action=ViewAllReportTypes", False)
            End If
            Dim colBound As New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "CMSUSER"
            colBound.HeaderText = m_refMsg.GetMessage("generic Username")
            colBound.HeaderStyle.Width = Unit.Percentage(40)
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "ADUSER"
            colBound.HeaderText = m_refMsg.GetMessage("generic User Group Name")
            colBound.HeaderStyle.Width = Unit.Percentage(50)
            colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "DELETE"
            colBound.HeaderText = m_refMsg.GetMessage("generic Delete title")
            colBound.HeaderStyle.Width = Unit.Percentage(10)
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            AdReportsGrid.Columns.Add(colBound)

            Dim dt As New DataTable
            Dim dr As DataRow = Nothing
            dt.Columns.Add(New DataColumn("CMSUSER", GetType(String)))
            dt.Columns.Add(New DataColumn("ADUSER", GetType(String)))
            dt.Columns.Add(New DataColumn("DELETE", GetType(String)))
            Dim e1count As Integer = 2
            Dim e2count As Integer = 3

            Dim i As Integer = 0
            Dim arrayCount As Integer = 0
            Dim currentUserID As Long = m_refUserApi.UserId
            If (user_group_data.Length = m_intMax) Then
                TD_count.InnerHtml = m_intMax & " " & m_refMsg.GetMessage("ad relationships displayed") & " <a href=""adreports.aspx?action=GetRelationshipsForSynch&max=0"">" & m_refMsg.GetMessage("generic Show All") & "</a><br><br>"
            End If
            If (settings_data.ADIntegration) Then
                For i = 0 To user_group_data.Length - 1
                    dr = dt.NewRow
                    dr(0) += "<input type=""hidden"" name=""userid" & i + 1 & """ value=""" & user_group_data(i).UserId & """>"
                    dr(0) += "<input type=""hidden"" name=""groupid" & i + 1 & """ value=""" & user_group_data(i).GroupId & """>"
                    dr(0) += user_group_data(i).UserName
                    dr(1) = user_group_data(i).GroupName
                    dr(2) = "<input type=""checkbox"" name=""delete" & i + 1 & """ value=""delete"">"
                    dt.Rows.Add(dr)
                Next

                AdReportsGrid.ShowFooter = True

                e1count = e1count + 5
                e2count = e2count + 5

            End If
            usercount.Value = i + 1
            Dim dv As New DataView(dt)
            AdReportsGrid.DataSource = dv
            AdReportsGrid.DataBind()
            GetRelationshipsForSyncToolBar()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub
    Private Sub GetRelationshipsForSyncToolBar()
        Dim result As New System.Text.StringBuilder
        Try
            result.Append("<table><tr>")
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("match r to ad title"))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (relationships)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('synchrelationships', '');"""))
            ' result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/refresh.png", "adreports.aspx?action=GetRelationshipsForSynch&max=" & m_intMax, m_refMsg.GetMessage("generic Refresh"), m_refMsg.GetMessage("btn refresh"), ""))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            result.Append("<td>" & m_refStyle.GetHelpButton(m_strPageAction) & "</td>")
            result.Append("</tr></table>")
            divToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub GetGroupsForSync()
        Try
            TR_count.Visible = True
            Dim group_data As GroupData()
            If (Not (Request.QueryString("max"))) Then
                If (Request.QueryString("max") <> "") Then
                    m_intMax = Request.QueryString("max")
                End If
            End If
            m_refSiteApi = New SiteAPI
            m_refUserApi = New UserAPI
            settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
            group_data = m_refUserApi.GetCMSGroupsToSync(m_intMax)
            If (group_data Is Nothing) Then
                Response.Redirect("adreports.aspx?action=ViewAllReportTypes", False)
                Exit Sub
            End If
            Dim colBound As New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "CMSUSER"
            colBound.HeaderText = m_refMsg.GetMessage("cms group name")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.HeaderStyle.Width = Unit.Percentage(20)
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "ADUSER"
            colBound.HeaderText = m_refMsg.GetMessage("ad group name")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.HeaderStyle.Width = Unit.Percentage(20)
            colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "AT"
            colBound.HeaderText = "@"
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.HeaderStyle.Width = Unit.Percentage(2)
            colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "DOMAIN"
            colBound.HeaderText = m_refMsg.GetMessage("ad domain")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.HeaderStyle.Width = Unit.Percentage(25)
            colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "SEARCH"
            colBound.HeaderText = m_refMsg.GetMessage("generic Search")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "DELETE"
            colBound.HeaderText = m_refMsg.GetMessage("generic Delete title")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            AdReportsGrid.Columns.Add(colBound)

            Dim dt As New DataTable
            Dim dr As DataRow
            dt.Columns.Add(New DataColumn("CMSUSER", GetType(String)))
            dt.Columns.Add(New DataColumn("ADUSER", GetType(String)))
            dt.Columns.Add(New DataColumn("AT", GetType(String)))
            dt.Columns.Add(New DataColumn("DOMAIN", GetType(String)))
            dt.Columns.Add(New DataColumn("SEARCH", GetType(String)))
            dt.Columns.Add(New DataColumn("DELETE", GetType(String)))
            Dim e1count As Integer = 3
            Dim e2count As Integer = 4
            Dim domainArray As Object
            Dim i As Integer = 0
            Dim arrayCount As Integer = 0
            Dim currentUserID As Long = m_refUserApi.UserId
            If (group_data.Length = m_intMax) Then
                TD_count.InnerHtml = m_intMax & " " & m_refMsg.GetMessage("ad groups displayed") & " <a href=""adreports.aspx?action=GetGroupsForSynch&max=0"">" & m_refMsg.GetMessage("generic Show All") & "</a><br><br>"
            End If
            If (settings_data.ADIntegration) Then
                For i = 0 To group_data.Length - 1
                    dr = dt.NewRow
                    domainArray = Split(group_data(i).AdGroupDomain, ",")
                    If group_data(i).IsMemberShipGroup Then
                        dr(0) = MemberShipGroupIcon & group_data(i).GroupName
                    Else
                        dr(0) = CmsGroupIcon & group_data(i).GroupName
                    End If
                    dr(0) += "<input type=""hidden"" name=""userid" & i + 1 & """ value=""" & group_data(i).GroupId & """>"
                    dr(1) = "<input type=""text"" class=""" & INPUTCLASS & """ name=""username" & i + 1 & """ id=""username" & i + 1 & """ value=""" & group_data(i).AdGroupName & """ maxlength=""255""></td>"
                    dr(2) = "@"
                    dr(3) += ""
                    If (UBound(domainArray) > 0) Then
                        dr(3) += "<select name=""sel_domain" & i + 1 & """ onchange=""javascript:document.forms[0].domain" & i + 1 & ".value = document.forms.synchusers.sel_domain" & i + 1 & ".options[document.forms[0].sel_domain" & i + 1 & ".selectedIndex].value;"">"
                        dr(3) += "<option value="""" selected>" & m_refMsg.GetMessage("multiples found")
                        For arrayCount = LBound(domainArray) To UBound(domainArray)
                            dr(3) += "<option value=""" & domainArray(arrayCount) & """>" & domainArray(arrayCount)
                        Next
                        dr(3) += "</select><br>"
                        dr(3) += "<input type=""text"" class=""" & INPUTCLASS & """ name=""domain" & i + 1 & """ id=""domain" & i + 1 & """ maxlength=""255"">"
                    Else
                        'dr(3)+="<input type=""hidden"">")
                        dr(3) += "<input type=""text"" class=""" & INPUTCLASS & """ name=""domain" & i + 1 & """ id=""domain" & i + 1 & """ value=""" & group_data(i).AdGroupDomain & """ maxlength=""255"">"
                    End If
                    dr(4) = "<a href=""#"" OnClick=""javascript:PopUpWindow('users.aspx?action=MapCMSUserGroupToAD&id=" & group_data(i).GroupId & "&f=0&e1=" & "username" & (i + 1) & "&e2=" & "domain" & (i + 1) & "&rp=3','Summary',690,380,1,1);"">" & m_refMsg.GetMessage("generic Search") & "</a>"
                    If (group_data(i).GroupId = CLng(currentUserID)) Then
                        dr(5) = "<input type=""checkbox"" name=""delete" & i + 1 & """ value=""delete"" disabled onClick=""return false;"">"
                    Else
                        dr(5) = "<input type=""checkbox"" name=""delete" & i + 1 & """ value=""delete"">"
                    End If
                    dt.Rows.Add(dr)
                    e1count = e1count + 4
                    e2count = e2count + 4
                Next
                AdReportsGrid.ShowFooter = True
            End If
            usercount.Value = i + 1
            Dim dv As New DataView(dt)
            AdReportsGrid.DataSource = dv
            AdReportsGrid.DataBind()
            GetGroupsForSyncToolBar()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub
    Private Sub GetGroupsForSyncToolBar()
        Dim result As New System.Text.StringBuilder
        Try
            result.Append("<table><tr>")
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("match g to ad title"))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (groups)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('synchusers', '');"""))
            ' result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/refresh.png", "adreports.aspx?action=GetGroupsForSynch&max=" & m_intMax, m_refMsg.GetMessage("generic Refresh"), m_refMsg.GetMessage("btn refresh"), ""))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            result.Append("<td>" & m_refStyle.GetHelpButton(m_strPageAction) & "</td>")
            result.Append("</tr></table>")
            divToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub GetUsersForSync()
        Try
            TR_count.Visible = True
            Dim user_data As UserData()
            If (Not (Request.QueryString("max"))) Then
                If (Request.QueryString("max") <> "") Then
                    m_intMax = Request.QueryString("max")
                End If
            End If
            m_refSiteApi = New SiteAPI
            m_refUserApi = New UserAPI
            settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
            user_data = m_refUserApi.GetCMSUsersToSync(m_intMax)
            If (IsNothing(user_data)) Then
                Response.Redirect("adreports.aspx?action=ViewAllReportTypes", False)
            End If
            Dim colBound As New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "CMSUSER"
            colBound.HeaderText = m_refMsg.GetMessage("cms username")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.HeaderStyle.Width = Unit.Percentage(22)
            colBound.ItemStyle.Width = Unit.Percentage(22)
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "ADUSER"
            colBound.HeaderText = m_refMsg.GetMessage("ad username")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.HeaderStyle.Width = Unit.Percentage(23)
            colBound.ItemStyle.Width = Unit.Percentage(23)
            colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "AT"
            colBound.HeaderText = "@"
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.HeaderStyle.Width = Unit.Percentage(2)
            colBound.ItemStyle.Width = Unit.Percentage(2)
            colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            colBound.ItemStyle.Wrap = False
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "DOMAIN"
            colBound.HeaderText = m_refMsg.GetMessage("ad domain")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.HeaderStyle.Width = Unit.Percentage(25)
            colBound.ItemStyle.Width = Unit.Percentage(25)
            colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            colBound.ItemStyle.Wrap = False
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "SEARCH"
            colBound.HeaderText = m_refMsg.GetMessage("generic Search")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            colBound.HeaderStyle.Width = Unit.Percentage(13)
            colBound.ItemStyle.Width = Unit.Percentage(13)
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Center
            AdReportsGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "DELETE"
            colBound.HeaderText = m_refMsg.GetMessage("generic Delete title")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.HeaderStyle.Width = Unit.Percentage(15)
            colBound.ItemStyle.Width = Unit.Percentage(15)
            colBound.ItemStyle.Wrap = False
            AdReportsGrid.Columns.Add(colBound)

            Dim dt As New DataTable
            Dim dr As DataRow
            dt.Columns.Add(New DataColumn("CMSUSER", GetType(String)))
            dt.Columns.Add(New DataColumn("ADUSER", GetType(String)))
            dt.Columns.Add(New DataColumn("AT", GetType(String)))
            dt.Columns.Add(New DataColumn("DOMAIN", GetType(String)))
            dt.Columns.Add(New DataColumn("SEARCH", GetType(String)))
            dt.Columns.Add(New DataColumn("DELETE", GetType(String)))
            Dim e1count As Integer = 3
            Dim e2count As Integer = 4
            Dim domainArray As Object
            Dim i As Integer = 0
            Dim arrayCount As Integer = 0
            Dim currentUserID As Long = m_refUserApi.UserId
            If ((Not (IsNothing(user_data))) AndAlso (user_data.Length = m_intMax)) Then
                TD_count.InnerHtml = m_intMax & " " & m_refMsg.GetMessage("ad users displayed") & " <a href=""adreports.aspx?action=GetUsersForSynch&max=0"">" & m_refMsg.GetMessage("generic Show All") & "</a><br><br>"
            End If
            If (settings_data.ADAuthentication = 1) Then
                If (Not (IsNothing(user_data))) Then
                    For i = 0 To user_data.Length - 1
                        dr = dt.NewRow
                        domainArray = Split(user_data(i).Domain, ",")
                        If user_data(i).IsMemberShip Then
                            dr(0) = MemberShipUserIcon & user_data(i).Username
                        Else
                            dr(0) = CmsUserIcon & user_data(i).Username
                        End If
                        dr(0) += "<input type=""hidden"" name=""userid" & i + 1 & """ value=""" & user_data(i).Id & """>"
                        dr(1) = "<input type=""text"" class=""" & INPUTCLASS & """ name=""username" & i + 1 & """ id=""username" & i + 1 & """ value=""" & user_data(i).AdUserName & """ maxlength=""255""></td>"
                        dr(2) = "@"
                        dr(3) = ""
                        If (UBound(domainArray) > 0) Then
                            dr(3) += "<select name=""sel_domain" & i + 1 & """ onchange=""javascript:document.forms[0].domain" & i + 1 & ".value = document.forms.synchusers.sel_domain" & i + 1 & ".options[document.forms[0].sel_domain" & i + 1 & ".selectedIndex].value;"">"
                            dr(3) += "<option value="""" selected>" & m_refMsg.GetMessage("multiples found")
                            For arrayCount = LBound(domainArray) To UBound(domainArray)
                                dr(3) += "<option value=""" & domainArray(arrayCount) & """>" & domainArray(arrayCount)
                            Next
                            dr(3) += "</select><br>"
                            dr(3) += "<input type=""text"" class=""" & INPUTCLASS & """ name=""domain" & i + 1 & """ id=""domain" & i + 1 & """ maxlength=""255"">"
                        Else
                            'dr(3)+="<input type=""hidden"">")
                            dr(3) += "<input type=""text"" class=""" & INPUTCLASS & """ name=""domain" & i + 1 & """ id=""domain" & i + 1 & """ value=""" & user_data(i).Domain & """ maxlength=""255"">"
                        End If
                        dr(4) = "<a href=""#"" OnClick=""javascript:PopUpWindow('users.aspx?action=MapCMSUserToAD&id=" & user_data(i).Id & "&f=0&e1=" & "username" & (i + 1) & "&e2=" & "domain" & (i + 1) & "&rp=3','Summary',690,380,1,1);"">" & m_refMsg.GetMessage("generic Search") & "</a>"
                        If (user_data(i).Id = CLng(currentUserID)) Then
                            dr(5) = "<input type=""checkbox"" name=""delete" & i + 1 & """ value=""delete"" disabled onClick=""return false;"">"
                        Else
                            dr(5) = "<input type=""checkbox"" name=""delete" & i + 1 & """ value=""delete"">"
                        End If
                        dt.Rows.Add(dr)
                        e1count = e1count + 4
                        e2count = e2count + 4
                    Next

                    AdReportsGrid.ShowFooter = True

                End If
            End If
            usercount.Value = i + 1
            Dim dv As New DataView(dt)
            AdReportsGrid.DataSource = dv
            AdReportsGrid.DataBind()
            GetUsersForSyncToolBar()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub
    Private Sub GetUsersForSyncToolBar()
        Dim result As New System.Text.StringBuilder
        Try
            result.Append("<table><tr>")
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("match u to ad title"))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (users)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('synchusers', '');"""))
            ' result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/refresh.png", "adreports.aspx?action=GetUsersForSynch&max=" & m_intMax, m_refMsg.GetMessage("generic Refresh"), m_refMsg.GetMessage("btn refresh"), ""))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            result.Append("<td>" & m_refStyle.GetHelpButton(m_strPageAction) & "</td>")
            result.Append("</tr></table>")
            divToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub ViewAllReportTypes()
        AdReportsGrid.ShowHeader = False

        m_refSiteApi = New SiteAPI
        m_refUserApi = New UserAPI
        settings_data = m_refSiteApi.GetSiteVariables(m_refSiteApi.UserId)
        sync_data = m_refUserApi.GetADStatus()

        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = ""
        colBound.HeaderStyle.Height = Unit.Empty
        If (settings_data.ADAuthentication = 1) Or (settings_data.AdValid = True) Then
            colBound.ItemStyle.Wrap = False
        Else
            colBound.ItemStyle.Wrap = True
        End If
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        AdReportsGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        If ((sync_data.SyncUsers) Or (sync_data.SyncGroups) Or (sync_data.SyncRelationships) Or (sync_data.DeSyncGroups) Or (sync_data.DeSyncUsers)) Then

            If (settings_data.ADAuthentication = 1) Then
                ltr_status.Text &= m_refMsg.GetMessage("ad enabled not configured")
            Else
                ltr_status.Text &= m_refMsg.GetMessage("ad disabled not configured")
            End If
            status.Visible = True

            dr = dt.NewRow
            If (sync_data.SyncUsers) Then
                dr = dt.NewRow
                dr(0) = "<a class=""itemstatus"" href=""adreports.aspx?action=GetUsersForSynch"" title=""" & m_refMsg.GetMessage("match cms u to ad") & """>" & m_refMsg.GetMessage("match cms u to ad") & "</a>"
                dt.Rows.Add(dr)
            End If
            If (sync_data.SyncGroups) Then
                dr = dt.NewRow
                dr(0) = "<a class=""itemstatus"" href=""adreports.aspx?action=GetGroupsForSynch"" title=""" & m_refMsg.GetMessage("match cms g to ad") & """>" & m_refMsg.GetMessage("match cms g to ad") & "</a>"
                dt.Rows.Add(dr)
            End If
            If (sync_data.SyncRelationships) Then
                dr = dt.NewRow
                dr(0) = "<a class=""itemstatus"" href=""adreports.aspx?action=GetRelationshipsForSynch"" title=""" & m_refMsg.GetMessage("match cms r to ad") & """>" & m_refMsg.GetMessage("match cms r to ad") & "</a>"
                dt.Rows.Add(dr)
            End If
            If (sync_data.DeSyncUsers) Then
                dr = dt.NewRow
                dr(0) = "<a class=""itemstatus"" href=""adreports.aspx?action=DeSynchUsers"" title=""" & m_refMsg.GetMessage("make u unique") & """>" & m_refMsg.GetMessage("make u unique") & "</a>"
                dt.Rows.Add(dr)
            End If
            If (sync_data.DeSyncGroups) Then
                dr = dt.NewRow
                dr(0) = "<a class=""itemstatus"" href=""adreports.aspx?action=DeSynchGroups"" title=""" & m_refMsg.GetMessage("make g unique") & """>" & m_refMsg.GetMessage("make g unique") & "</a>"
                dt.Rows.Add(dr)
            End If
        Else

            If (settings_data.ADAuthentication = 1) Then
                ltr_status.Text = m_refMsg.GetMessage("ad enabled and configured")
            ElseIf (settings_data.AdValid = True) Then
                ltr_status.Text = m_refMsg.GetMessage("alt Active Directory is not enabled and configured.")
            Else '
                ltr_status.Text = "<span class=""important"">" & m_refMsg.GetMessage("entrprise license with AD required msg") & "</span>"
            End If
            status.Visible = True

        End If
        Dim dv As New DataView(dt)
        AdReportsGrid.DataSource = dv
        AdReportsGrid.DataBind()

        ViewAllReportTypesToolBar()
    End Sub
    Private Sub ViewAllReportTypesToolBar()
        Dim result As New System.Text.StringBuilder
        Try
            result.Append("<table><tr>")
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("ad status"))
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:history.back(1);", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
            result.Append("<td>" & m_refStyle.GetHelpButton(m_strPageAction) & "</td>")
            result.Append("</tr></table>")
            divToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub Process_DeSynchUsers()
        Dim result As AdDeSyncUserData()
        m_refUserApi = New UserAPI
        Try
            result = m_refUserApi.DeSynchUsers(True)
            'Response.Redirect("adreports.aspx?action=ViewAllReportTypes", False)
            Response.Redirect("users.aspx?backaction=viewallusers&action=viewallusers&grouptype=0&groupid=2&id=2&FromUsers=1", False)
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Private Sub Process_SynchCMSUsersToAD()
        Dim cUserIDs As New Collection
        Dim cADUsernames As New Collection
        Dim cADDomains As New Collection
        Dim cActions As New Collection
        Dim count As Integer
        Dim userid As Long
        Dim adusername As String
        Dim addomain As String
        Dim addelete As String
        For count = 1 To CLng(Request.Form("usercount"))
            userid = CLng(Request.Form("userid" & CStr(count)))
            adusername = CStr(Request.Form("username" & CStr(count)))
            addomain = CStr(Request.Form("domain" & CStr(count)))
            addelete = CStr(Request.Form("delete" & CStr(count)))
            If (addelete <> "") Then
                cUserIDs.Add(userid, count)
                cADUsernames.Add("", count)
                cADDomains.Add("", count)
                cActions.Add("delete", count)
            ElseIf ((adusername <> "") And (addomain <> "")) Then
                cUserIDs.Add(userid, count)
                cADUsernames.Add(adusername, count)
                cADDomains.Add(addomain, count)
                cActions.Add("map", count)
            End If
        Next
        m_refUserApi = New UserAPI

        m_refUserApi.SynchCMSUsersToAD(cUserIDs, cADUsernames, cADDomains, cActions, 0)
        Response.Redirect("adreports.aspx?action=GetUsersForSynch", False)
    End Sub
    Private Sub Process_SynchCMSGroupsToAD()
        Dim cUserIDs As New Collection
        Dim cADUsernames As New Collection
        Dim cADDomains As New Collection
        Dim cActions As New Collection
        Dim count As Integer
        Dim userid As Long
        Dim adusername As String
        Dim addomain As String
        Dim addelete As String
        For count = 1 To CLng(Request.Form("usercount"))
            If Not (Request.Form("userid" & CStr(count)) Is Nothing) Then
                userid = CLng(Request.Form("userid" & CStr(count)))
                adusername = CStr(Request.Form("username" & CStr(count)))
                addomain = CStr(Request.Form("domain" & CStr(count)))
                addelete = CStr(Request.Form("delete" & CStr(count)))
                If (addelete <> "") Then
                    cUserIDs.Add(userid, count)
                    cADUsernames.Add("", count)
                    cADDomains.Add("", count)
                    cActions.Add("delete", count)
                ElseIf ((adusername <> "") And (addomain <> "")) Then
                    cUserIDs.Add(userid, count)
                    cADUsernames.Add(adusername, count)
                    cADDomains.Add(addomain, count)
                    cActions.Add("map", count)
                End If
            End If
        Next

        m_refUserApi = New UserAPI
        m_refUserApi.SynchCMSGroupsToAD(cUserIDs, cADUsernames, cADDomains, cActions)

        Response.Redirect("adreports.aspx?action=GetGroupsForSynch", False)

    End Sub
    Private Sub Process_SynchCMSRelationShipsToAD()
        Dim cUserIDs As New Collection
        Dim cGroupIDs As New Collection
        Dim cActions As New Collection
        Dim count As Integer
        Dim userid As Long
        Dim groupid As Integer
        Dim addelete As String
        For count = 1 To CLng(Request.Form("usercount"))
            userid = CLng(Request.Form("userid" & CStr(count)))
            groupid = CStr(Request.Form("groupid" & CStr(count)))
            addelete = CStr(Request.Form("delete" & CStr(count)))
            If (addelete <> "") Then
                cUserIDs.Add(userid, count)
                cGroupIDs.Add(groupid, count)
                cActions.Add("delete", count)
            End If
        Next

        m_refUserApi = New UserAPI
        m_refUserApi.SynchCMSRelationshipsToAD(cUserIDs, cGroupIDs, cActions)

        Response.Redirect("adreports.aspx?action=GetRelationshipsForSynch", False)

    End Sub
    Private Sub RegisterResources()

        Ektron.Cms.API.JS.RegisterJS(Me, AppPath & "java/empjsfunc.js", "EktronEmpJSFuncJS")
        Ektron.Cms.API.JS.RegisterJS(Me, AppPath & "java/toolbar_roll.js", "EktronToolbarRollJS")
        Ektron.Cms.API.JS.RegisterJS(Me, AppPath & "java/workareahelper.js", "EktronWorkareaHelperJS")

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

    End Sub
End Class
