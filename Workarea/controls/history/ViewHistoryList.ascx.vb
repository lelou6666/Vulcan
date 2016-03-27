Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports System.Collections.Generic
Imports Ektron.Cms.Analytics

Partial Class ViewHistoryList
    Inherits System.Web.UI.UserControl

#Region "Private members"

    Protected m_refContentApi As ContentAPI
    Protected AppName As String = ""
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_refStyle As New StyleHelper
    Protected ContentLanguage As Integer = 0
    Protected m_contentType As Long = CMSContentType_Content
    Protected ContentId As Long = 0
    Protected _analyticsEnabled As Boolean = False
    Protected ShowBackButton As Boolean = True
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            m_refContentApi = New ContentAPI
            _analyticsEnabled = AnalyticsSecurity.Enabled(m_refContentApi.RequestInformationRef)
            RegisterResources()
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

            m_refMsg = m_refContentApi.EkMsgRef

            If (Not (IsNothing(Request.QueryString("id")))) Then
                If (Request.QueryString("id") <> "") Then
                    ContentId = Request.QueryString("id")
                End If
            End If
            If (Not (IsNothing(Request.QueryString("showbackbutton")))) Then
                If (Request.QueryString("showbackbutton") <> "") Then
                    ShowBackButton = Request.QueryString("showbackbutton")
                End If
            End If


            If ContentId > 0 Then m_contentType = m_refContentApi.EkContentRef.GetContentType(ContentId)

            Select Case m_contentType

                Case CMSContentType_CatalogEntry

                    Populate_EntryHistoryListGrid(ContentId)
                    DisplayEntryHistoryToolBar()
                    Util_SetResources()

                Case Else

                    Populate_HistoryListGrid(ContentId)
                    DisplayHistoryToolBar()
                    Util_SetResources()

            End Select

        Catch ex As Exception
            Response.Redirect(m_refContentApi.ApplicationPath & "reterror.aspx?info=" & ex.Message, False)
        End Try

    End Sub


#Region "Catalog Entry"

    Private Sub DisplayEntryHistoryToolBar()

        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view catalog entry history title"))
        htmToolBar.InnerHtml = m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/ui/icons/contentCompareAnalytics.png", "javascript:CompareAnalytics(" & ContentId.ToString() & ", " & ContentLanguage.ToString() & ");", m_refMsg.GetMessage("lbl compare analytics"), m_refMsg.GetMessage("lbl compare analytics"), "")
        htmToolBar.InnerHtml += m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/ui/icons/back.png", "javascript:history.go(-1);", m_refMsg.GetMessage("btn back"), m_refMsg.GetMessage("btn back"), "")

    End Sub

    Private Sub Populate_EntryHistoryListGrid(ByVal Id As Long)

        Dim m_refCatalogAPI As New Commerce.CatalogEntryApi()
        Dim dt As New DataTable
        Dim strLink As String = ""
        Dim entry_version_list As New List(Of Commerce.EntryData)
        Dim offset As Integer = 0
        Dim majorVersions As Integer = 0

        If (Id <> -1) Then

            entry_version_list = m_refCatalogAPI.GetVersionList(Id, ContentLanguage)

        End If

        If (Not (IsNothing(entry_version_list))) Then

            If (_analyticsEnabled) Then
                offset = 1
                HistoryListGrid.Columns.Add(GetColumn("EkCompare", m_refMsg.GetMessage("lbl compare"), False, "title-header bottom"))
            End If
            HistoryListGrid.Columns.Add(GetColumn("EkVersion", m_refMsg.GetMessage("version"), False, "title-header bottom"))
            HistoryListGrid.Columns.Add(GetColumn("PublishDate", m_refMsg.GetMessage("hist list title") & "<div class=""caption ektronCaption"">(<span style=""background-image: url('" & m_refContentApi.AppPath & "images/ui/icons/forward.png'); display: inline-block; width: 16px; height: 16px; background-position: center center; background-repeat: no-repeat;text-indent: -10000px"">&nbsp;</span> = " & m_refMsg.GetMessage("lbl content pd label") & ")</div>", False, "title-header bottom"))
            HistoryListGrid.Columns.Add(GetColumn("TITLE", m_refMsg.GetMessage("generic title"), False, "title-header bottom"))
            HistoryListGrid.Columns.Add(GetColumn("Editor", m_refMsg.GetMessage("content LUE label"), False, "title-header bottom"))
            HistoryListGrid.Columns.Add(GetColumn("Comments", m_refMsg.GetMessage("comment text"), False, "title-header bottom contentHistoryComment"))

            HistoryListGrid.BorderColor = Drawing.Color.White

            Dim dr As DataRow
            If (_analyticsEnabled) Then
                dt.Columns.Add(New DataColumn("EkCompare", GetType(String)))
            End If
            dt.Columns.Add(New DataColumn("EkVersion", GetType(String)))
            dt.Columns.Add(New DataColumn("PublishDate", GetType(String)))
            dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
            dt.Columns.Add(New DataColumn("Editor", GetType(String)))
            dt.Columns.Add(New DataColumn("Comments", GetType(String)))
            Dim majorRev, minorRev, numMajors, pntr As Integer
            Dim firstRadio As Boolean = True
            majorRev = 0
            minorRev = 0
            numMajors = 0
            pntr = 0

            Dim minorarray(entry_version_list.Count) As Integer
            For i As Integer = 0 To (entry_version_list.Count - 1)
                If entry_version_list(i).Status = "A" Then
                    minorarray(numMajors) = minorRev
                    numMajors += 1
                    minorRev = 0
                Else
                    minorRev += 1
                End If
            Next
            minorarray(numMajors) = minorRev  ' This is really fist 1
            minorRev = minorarray(pntr)
            For i As Integer = 0 To (entry_version_list.Count - 1)
                dr = dt.NewRow()
                'class=""history-list""
                strLink = "<a href=""history.aspx?LangType=" & entry_version_list(i).LanguageId & "&hist_id=" & entry_version_list(i).VersionId & "&Id=" & Id & """ target=""history_frame"" title=""" & m_refMsg.GetMessage("view this version msg") & """>"

                dr(1 + offset) = strLink

                If entry_version_list(i).Status = "A" Then

                    minorRev = 0
                    dr(offset) = numMajors & ".0"
                    pntr += 1
                    minorRev = minorarray(pntr)
                    numMajors -= 1

                    Dim radiochecked As String = ""
                    If firstRadio Then
                        firstRadio = False
                        radiochecked = "checked "
                    End If

                    dr(1 + offset) += "<img src=""" & m_refContentApi.AppPath & "Images/ui/icons/forward.png"" align=""bottom"" alt=""Published"" title=""Published"" />"
                    dr(1 + offset) += entry_version_list(i).DateModified.ToShortDateString() & " " & entry_version_list(i).DateModified.ToShortTimeString()
                    If (_analyticsEnabled) Then
                        dr(0) = "<input class=""compare"" value=""" & entry_version_list(i).Id & """ id=""oldid"" name=""oldid"" type=""radio"" " & radiochecked & "/>&nbsp;"
                        dr(0) += "<input class=""compare"" value=""" & entry_version_list(i).Id & """ id=""diff"" name=""diff"" type=""radio"" " & radiochecked & "/>"
                    End If

                    majorVersions = majorVersions + 1

                Else
                    dr(offset) = numMajors & "." & minorRev
                    minorRev -= 1
                    dr(1 + offset) += "<div style='margin-left:15px;'>" & entry_version_list(i).DateModified.ToShortDateString() & " " & entry_version_list(i).DateModified.ToShortTimeString() & "</div>"

                End If
                dr(1 + offset) += "</a>"
                dr(2 + offset) = strLink & entry_version_list(i).Title.ToString() & "</a>"
                dr(3 + offset) = "<a href=""#"" onclick=""EmailUser(" & entry_version_list(i).LastEditorId.ToString() & ", '" & m_refMsg.GetMessage("btn email") & "'); return false;"">"
                dr(3 + offset) += "<img alt=""" & m_refMsg.GetMessage("btn email") & """ title=""" & m_refMsg.GetMessage("btn email") & """ src=""" & m_refContentApi.AppPath & "Images/ui/icons/email.png"" />&#160;"
                dr(3 + offset) += entry_version_list(i).LastEditorFirstName & " " & entry_version_list(i).LastEditorLastName & "</a>"
                dr(4 + offset) = entry_version_list(i).Comment

                dt.Rows.Add(dr)
            Next

            _analyticsEnabled = (majorVersions > 1)

        Else
            Dim colBound As New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "EkVersion"
            colBound.HeaderText = m_refMsg.GetMessage("lbl history status")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.Initialize()
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
            colBound.ItemStyle.CssClass = "history-list"
            colBound.HeaderStyle.Height = Unit.Empty
            HistoryListGrid.Columns.Add(colBound)

            dt.Columns.Add(New DataColumn("EkVersion", GetType(String)))

            Dim dr As DataRow
            dr = dt.NewRow()
            dr(0) = m_refMsg.GetMessage("msg current history")
            dt.Rows.Add(dr)

            _analyticsEnabled = False

        End If

        Dim dv As New DataView(dt)

        HistoryListGrid.DataSource = dv
        HistoryListGrid.DataBind()

    End Sub

#End Region


#Region "Other"

    Private Sub DisplayHistoryToolBar()

        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view content history title"))
        If (_analyticsEnabled) Then
            htmToolBar.InnerHtml = m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/ui/icons/contentCompareAnalytics.png", "javascript:CompareAnalytics(" & ContentId.ToString() & ", " & ContentLanguage.ToString() & ");", m_refMsg.GetMessage("lbl compare analytics"), m_refMsg.GetMessage("lbl compare analytics"), "")
        End If
        If ShowBackButton Then
            htmToolBar.InnerHtml += m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/ui/icons/back.png", "javascript:history.go(-1);", m_refMsg.GetMessage("btn back"), m_refMsg.GetMessage("btn back"), "")
        End If
        htmToolBar.InnerHtml += m_refStyle.GetHelpButton("ViewContentHistoryList")

    End Sub

    Private Sub Populate_HistoryListGrid(ByVal Id As Long)

        Dim dt As DataTable = GetHistoryList(Id)
        Dim dv As New DataView(dt)
        HistoryListGrid.DataSource = dv
        HistoryListGrid.DataBind()

    End Sub

    Private Function GetHistoryList(ByVal id As Long) As DataTable
        Dim i As Integer = 0
        Dim dt As New DataTable
        Dim strLink As String = ""
        Dim content_history_list() As ContentHistoryData = Nothing
        Dim offset As Integer = 0
        Dim majorVersions As Integer = 0

        If (id <> -1) Then
            content_history_list = m_refContentApi.GetHistoryList(id)
        End If
        If (Not (IsNothing(content_history_list))) Then

            If (_analyticsEnabled) Then
                offset = 1
                HistoryListGrid.Columns.Add(GetColumn("EkCompare", m_refMsg.GetMessage("lbl compare"), False, "title-header bottom"))
            End If
            HistoryListGrid.Columns.Add(GetColumn("EkVersion", m_refMsg.GetMessage("lbl version"), False, "title-header bottom"))
            HistoryListGrid.Columns.Add(GetColumn("PublishDate", m_refMsg.GetMessage("hist list title") & "<div class=""caption ektronCaption"">(<span style=""background-image: url('" & m_refContentApi.AppPath & "images/ui/icons/forward.png'); display: inline-block; width: 16px; height: 16px; background-position: center center; background-repeat: no-repeat;text-indent: -10000px"">&nbsp;</span> = " & m_refMsg.GetMessage("lbl content pd label") & ")</div>", False, "title-header bottom"))
            HistoryListGrid.Columns.Add(GetColumn("TITLE", m_refMsg.GetMessage("generic title"), False, "title-header bottom contentHistoryTitle"))
            HistoryListGrid.Columns.Add(GetColumn("Editor", m_refMsg.GetMessage("lbl content lue label"), False, "title-header bottom"))
            HistoryListGrid.Columns.Add(GetColumn("Comments", m_refMsg.GetMessage("comment text"), False, "title-header bottom contentHistoryComment"))

            'HistoryListGrid.BorderColor = Drawing.Color.White

            Dim dr As DataRow
            If (_analyticsEnabled) Then
                dt.Columns.Add(New DataColumn("EkCompare", GetType(String)))
            End If
            dt.Columns.Add(New DataColumn("EkVersion", GetType(String)))
            dt.Columns.Add(New DataColumn("PublishDate", GetType(String)))
            dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
            dt.Columns.Add(New DataColumn("Editor", GetType(String)))
            dt.Columns.Add(New DataColumn("Comments", GetType(String)))
            Dim majorRev, minorRev, numMajors, pntr As Integer
            Dim firstRadio As Boolean = True
            Dim nextRadio As Boolean = True
            majorRev = 0
            minorRev = 0
            numMajors = 0
            pntr = 0

            Dim minorarray(content_history_list.Length) As Integer
            For i = 0 To content_history_list.Length - 1
                If content_history_list(i).Status = "A" Then
                    minorarray(numMajors) = minorRev
                    numMajors += 1
                    minorRev = 0
                Else
                    minorRev += 1
                End If
            Next
            minorarray(numMajors) = minorRev  ' This is really fist 1
            minorRev = minorarray(pntr)
            For i = 0 To content_history_list.Length - 1
                dr = dt.NewRow()
                'class=""history-list""
                strLink = "<a href=""history.aspx?LangType=" & content_history_list(i).LanguageId & "&hist_id=" & content_history_list(i).Id & "&Id=" & id & """ target=""history_frame"" title=""" & m_refMsg.GetMessage("view this version msg") & """>"
                dr(1 + offset) = strLink
                If content_history_list(i).Status = "A" Then

                    minorRev = 0
                    dr(offset) = numMajors & ".0"
                    pntr += 1
                    minorRev = minorarray(pntr)
                    numMajors -= 1

                    Dim radiochecked As String = ""
                    Dim nextradiochecked As String = ""
                    If firstRadio Then
                        firstRadio = False
                        radiochecked = "checked "
                    ElseIf nextRadio Then
                        nextRadio = False
                        nextradiochecked = "checked "
                    End If

                    dr(1 + offset) += "<img src=""" & m_refContentApi.AppPath & "Images/ui/icons/forward.png"" align=""bottom"" alt=""Published"" title=""Published"" />"
                    dr(1 + offset) += content_history_list(i).DateInserted.ToShortDateString() & " " & content_history_list(i).DateInserted.ToShortTimeString()
                    If (_analyticsEnabled) Then
                        dr(0) = "<span class=""compare_option_primary""><input class=""compare"" value=""" & content_history_list(i).Id & """ name=""oldid"" type=""radio"" " & radiochecked & "/></span>"
                        dr(0) += "<span class=""compare_option_secondary""><input class=""compare"" value=""" & content_history_list(i).Id & """ name=""diff"" type=""radio"" " & nextradiochecked & "/></span>"
                    End If

                    majorVersions = majorVersions + 1

                Else

                    dr(offset) = numMajors & "." & minorRev
                    minorRev -= 1
                    dr(1 + offset) += "<div style='margin-left:15px;'>" & content_history_list(i).DateInserted.ToShortDateString() & " " & content_history_list(i).DateInserted.ToShortTimeString() & "</div>"

                End If
                dr(1 + offset) += "</a>"
                dr(2 + offset) = strLink & content_history_list(i).Title.ToString() & "</a>"
                If (content_history_list(i).DisplayName <> "") Then
                    dr(3 + offset) = content_history_list(i).DisplayName
                Else
                    dr(3 + offset) = content_history_list(i).UserName
                End If
                dr(4 + offset) = content_history_list(i).Comment

                dt.Rows.Add(dr)
            Next

            _analyticsEnabled = (majorVersions > 1)

        Else
            Dim colBound As New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "EkVersion"
            colBound.HeaderText = m_refMsg.GetMessage("lbl history status")
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.Initialize()
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
            colBound.ItemStyle.CssClass = "history-list"
            colBound.HeaderStyle.Height = Unit.Empty
            HistoryListGrid.Columns.Add(colBound)

            dt.Columns.Add(New DataColumn("EkVersion", GetType(String)))

            Dim dr As DataRow
            dr = dt.NewRow()
            dr(0) = m_refMsg.GetMessage("msg current history")
            dt.Rows.Add(dr)

            _analyticsEnabled = False

        End If

        Return dt
    End Function

#End Region


#Region "Private Helpers"


    Private Sub Util_SetResources()

        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "csslib/history/historyList.css", "HistoryList")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.ApplicationPath & "java/history/historylist.js", "HistoryListJs", True)

    End Sub
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
    End Sub

    Private Function GetColumn(ByVal dataField As String, ByVal headerText As String, Optional ByVal wrap As Boolean = True, Optional ByVal cssClass As String = "title-header") As WebControls.BoundColumn

        Dim colBound As New System.Web.UI.WebControls.BoundColumn

        colBound.DataField = dataField
        colBound.HeaderText = headerText
        colBound.Initialize()
        colBound.HeaderStyle.CssClass = cssClass

        If dataField.ToLower = "comments" Then
            colBound.ItemStyle.Width = "450"
        End If
        colBound.ItemStyle.Wrap = wrap
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        colBound.ItemStyle.CssClass = "history-list"
        colBound.HeaderStyle.Height = Unit.Empty
        Return colBound

    End Function


#End Region


End Class
