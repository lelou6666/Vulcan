Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.Community
Imports Ektron.Cms.Content
Imports System.Data

Partial Class controls_Community_PersonalTags_ViewAllTags
	Inherits System.Web.UI.UserControl

    Private m_tagApi As Community.TagsAPI
    Protected m_containerPage As Community_PersonalTags
    Private m_intTotalPages As Integer
    Private m_intCurrentPage As Integer = 1
    Private m_sortOrderBy As TagOrderBy = TagOrderBy.TaggedCount
    Private m_sortOrder As String = "desc"


	Public Sub New()
        m_tagApi = New Community.TagsAPI()
    End Sub

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_containerPage = CType(Page, Community_PersonalTags)
        If (m_tagApi.RequestInformationRef.IsMembershipUser = 1) Then
            Response.Redirect(m_tagApi.ApplicationPath & "reterror.aspx?info=Please login as cms user", True)
            Exit Sub
        End If
		If (("del" = m_containerPage.Mode) AndAlso (Not IsNothing(Request.Form("PTagsSelCBHdn"))) AndAlso (Request.Form("PTagsSelCBHdn").Trim.Length > 0)) Then

			Response.ClearContent()
			Response.Redirect("PersonalTags.aspx?action=viewall", False)
        Else

            ltlIsPostDataId.Text = tags_isPostData.UniqueID
            LoadToolBar()

            'only loadgrid if this is the first load - otherwise let paging navigation handle it.
            If (Page.IsPostBack And Request.Form(tags_isPostData.UniqueID) <> "") OrElse IsPostBack = False Then
                LoadGrid()
            End If

            tags_isPostData.Value = "true"
        End If

    End Sub

	Protected Sub LoadToolBar()
		Dim result As New System.Text.StringBuilder

		Try
            txtTitleBar.InnerHtml = m_containerPage.RefStyle.GetTitleBar(m_containerPage.RefMsg.GetMessage("personal tags page title"))

            result.Append("<table><tr>")
            'result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(m_containerPage.AppImgPath & "../UI/Icons/add.png", "personaltags.aspx?action=addtag", m_containerPage.RefMsg.GetMessage("alt add btn text (personal tag)"), m_containerPage.RefMsg.GetMessage("btn add personal tag"), ""))            

            If (1 = m_containerPage.RefCommonAPI.EnableMultilingual) Then
                result.Append("<td class=""label"">&#160;" + m_containerPage.RefMsg.GetMessage("generic view") + ":</td>")
                result.Append(m_containerPage.RefStyle.GetShowAllActiveLanguage(True, "", "javascript:SelLanguage(this.value);", Convert.ToString(m_containerPage.RefCommonAPI.ContentLanguage), True))
            End If

            result.Append("<td>")
            result.Append(m_containerPage.RefStyle.GetHelpButton("ViewAllTags_ascx"))
            result.Append("</td>")
            result.Append("</tr></table>")

            htmToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
	End Sub

    Protected Sub LoadGrid()
        If (Not String.IsNullOrEmpty(Request.QueryString("orderBy"))) Then
            m_sortOrderBy = Request.QueryString("orderBy")
        End If

        If (Not String.IsNullOrEmpty(Request.QueryString("order"))) Then
            m_sortOrder = Request.QueryString("order")
        End If

        Dim cb As New System.Web.UI.WebControls.BoundColumn
        cb.DataField = "fId"
        cb.HeaderText = m_containerPage.RefMsg.GetMessage("generic id") '"ID"        
        cb.Initialize()
        _dg.Columns.Add(cb)

        cb = New System.Web.UI.WebControls.BoundColumn
        cb.DataField = "fName"
        'cb.HeaderText = m_containerPage.RefMsg.GetMessage("generic name") ' "Name"
        cb.HeaderText = "<a href=""personaltags.aspx?orderBy=" & TagOrderBy.Text & "&order=" & IIf(m_sortOrderBy = TagOrderBy.Text AndAlso m_sortOrder = "asc", "desc", "asc") & """>" & m_containerPage.RefMsg.GetMessage("generic name") & "</a>"
        cb.Initialize()
        _dg.Columns.Add(cb)


        cb = New System.Web.UI.WebControls.BoundColumn
        cb.DataField = "fTotal"
        'cb.HeaderText = m_containerPage.RefMsg.GetMessage("lbl times used") ' "Times Used"
        cb.HeaderText = "<a href=""personaltags.aspx?orderBy=" & TagOrderBy.TaggedCount & "&order=" & IIf(m_sortOrderBy = TagOrderBy.TaggedCount AndAlso m_sortOrder = "asc", "desc", "asc") & """>" & m_containerPage.RefMsg.GetMessage("lbl times used") & "</a>"        
        cb.Initialize()
        _dg.Columns.Add(cb)


        cb = New System.Web.UI.WebControls.BoundColumn
        cb.DataField = "fLanguage"
        cb.HeaderText = m_containerPage.RefMsg.GetMessage("generic language") ' "Language"        
        cb.Initialize()
        _dg.Columns.Add(cb)

        _dg.DataSource = CreateMsgData()
        _dg.DataBind()

        LoadPageSettings()
    End Sub

	Protected Function CreateMsgData() As ICollection
		Dim dt As New DataTable
		Dim dr As DataRow
        Dim totalTags As Integer = 0
        Dim tags() As TagData
        Dim localizationApi As New LocalizationAPI()

		Try
			' header:
			dt.Columns.Add(New DataColumn("fId", GetType(String))) ' 0
			dt.Columns.Add(New DataColumn("fName", GetType(String))) ' 1
			dt.Columns.Add(New DataColumn("fTotal", GetType(String))) ' 3
			dt.Columns.Add(New DataColumn("fLanguage", GetType(String))) ' 4

            ' data:
            Dim request As New TagRequestData()
            request.PageSize = m_containerPage.RefCommonAPI.RequestInformationRef.PagingSize
            request.PageIndex = m_intCurrentPage
            request.LanguageId = m_containerPage.ContentLanguage
            request.OrderByDirection = IIf(m_sortOrder = "asc", EkEnumeration.OrderByDirection.Ascending, EkEnumeration.OrderByDirection.Descending)
            request.OrderBy = m_sortOrderBy
            tags = m_tagApi.GetAllTags(request, totalTags)

            'get totalpages
            m_intTotalPages = totalTags / m_containerPage.RefCommonAPI.RequestInformationRef.PagingSize
            If (m_intTotalPages * m_containerPage.RefCommonAPI.RequestInformationRef.PagingSize < totalTags) Then m_intTotalPages += 1

            For Each tag As TagData In tags
                dr = dt.NewRow()
                dr(0) = tag.Id.ToString
                dr(1) = "<a href=""?action=viewtag&id=" + tag.Id.ToString + """ title=""" + m_containerPage.RefMsg.GetMessage("btn click to view tag") + """ target=""_self"" >" + tag.Text + "</a>"
                dr(2) = tag.TotalUsedCount.ToString()
                dr(3) = "<img src='" & localizationApi.GetFlagUrlByLanguageID(tag.LanguageId) & "' border=""0"" />"
                dt.Rows.Add(dr)
            Next

        Catch ex As Exception
        Finally
            CreateMsgData = New DataView(dt)
        End Try
	End Function

    Private Sub LoadPageSettings()
        If (m_intTotalPages <= 1) Then
            SetPageControlsVisible(False)
        Else
            SetPageControlsVisible(True)
            TotalPages.Text = (System.Math.Ceiling(m_intTotalPages)).ToString()
            CurrentPage.Text = m_intCurrentPage.ToString()
            PreviousPage.Enabled = True
            FirstPage.Enabled = True
            NextPage.Enabled = True
            LastPage.Enabled = True
            If m_intCurrentPage = 1 Then
                PreviousPage.Enabled = False
                FirstPage.Enabled = False
            ElseIf m_intCurrentPage = m_intTotalPages Then
                NextPage.Enabled = False
                LastPage.Enabled = False
            End If
        End If
    End Sub
    Private Sub SetPageControlsVisible(ByVal areVisible As Boolean)
        TotalPages.Visible = areVisible
        CurrentPage.Visible = areVisible
        PreviousPage.Visible = areVisible
        NextPage.Visible = areVisible
        LastPage.Visible = areVisible
        FirstPage.Visible = areVisible
        PageLabel.Visible = areVisible
        OfLabel.Visible = areVisible
    End Sub

    Sub NavigationLink_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "First"
                m_intCurrentPage = 1
            Case "Last"
                m_intCurrentPage = Int32.Parse(TotalPages.Text)
            Case "Next"
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) + 1
            Case "Prev"
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) - 1
        End Select
        LoadGrid()
    End Sub
End Class

