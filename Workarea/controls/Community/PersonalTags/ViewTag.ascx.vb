Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.Content
Imports System.Data

Partial Class controls_Community_PersonalTags_ViewTag
	Inherits System.Web.UI.UserControl
    Private m_tagApi As Community.TagsAPI
    Private m_userApi As UserAPI
	Protected m_containerPage As Community_PersonalTags

	Public Sub New()
        m_tagApi = New Community.TagsAPI
	End Sub

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Dim refCommonAPI As New CommonApi()
        Dim successFlag As Boolean = False
        If (m_tagApi.RequestInformationRef.IsMembershipUser = 1) Then
            Response.Redirect(m_tagApi.ApplicationPath & "reterror.aspx?info=Please login as cms user", True)
            Exit Sub
        End If
        m_containerPage = CType(Page, Community_PersonalTags)

		'If (("del" = m_containerPage.Mode) AndAlso (Not IsNothing(Request.Form("PTagsSelCBHdn"))) AndAlso (Request.Form("PTagsSelCBHdn").Trim.Length > 0)) Then
		'Dim sDelList() As String = (Request.Form("PTagsSelCBHdn").Trim.Split(","))
		'Dim idx As Integer
		'Dim delList() As Integer = Array.CreateInstance(GetType(Integer), sDelList.Length)
		'For idx = 0 To sDelList.Length - 1
		'	If (IsNumeric(sDelList(idx))) Then
		'		delList.SetValue(CType(sDelList(idx), Integer), idx)
		'	End If
		'Next

		If (IsPostBack()) Then
			If (Not IsNothing(Request.Form(tagValid.UniqueID)) AndAlso ("1" = Request.Form(tagValid.UniqueID))) Then
				' TODO: If error, display failure message:
				successFlag = DeleteTag()
			End If

			Response.ClearContent()
			Response.Redirect("PersonalTags.aspx?action=viewall", False)
		Else
			LoadToolBar()
			DisplayInfo()
		End If

	End Sub

	Protected Sub LoadToolBar()
		Dim result As New System.Text.StringBuilder

		Try
            m_userApi = New UserAPI()
            txtTitleBar.InnerHtml = m_containerPage.RefStyle.GetTitleBar(m_containerPage.RefMsg.GetMessage("view tag page title"))
            result.Append("<table><tr>")
            If (m_userApi.IsAdmin()) Then
                result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(m_containerPage.AppImgPath & "../UI/Icons/delete.png", "Javascript:doDeleteSubmit('" + tagValid.ClientID + "', '" + m_containerPage.RefMsg.GetMessage("js: confirm delete") + "');", m_containerPage.RefMsg.GetMessage("alt delete button text"), m_containerPage.RefMsg.GetMessage("alt delete button text"), ""))
            End If
            result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(m_containerPage.AppImgPath & "../UI/Icons/back.png", "personaltags.aspx?action=viewall", m_containerPage.RefMsg.GetMessage("alt back button"), m_containerPage.RefMsg.GetMessage("alt back button"), ""))
            result.Append("<td>")
            result.Append(m_containerPage.RefStyle.GetHelpButton("view_tag"))
            result.Append("</td>")
            result.Append("</tr></table>")
            htmToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
	End Sub

	Public Sub DisplayInfo()
        Dim td As TagData
        Dim siteApi As New SiteAPI()
        Dim langData As LanguageData

		Try
			tagIdLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic id")
			tagLangLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic language")
            tagNameLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic name")
            tagStatisticsLabel.Text = m_containerPage.RefMsg.GetMessage("lbl tag statistics")

            td = m_tagApi.GetTagByID(m_containerPage.TagId)
			If (Not IsNothing(td)) Then
				tagNameLit.Text = td.Text
                tagIdLit.Text = td.Id.ToString

                If (td.LanguageId = -1) Then
                    tagLangLit.Text = m_containerPage.RefMsg.GetMessage("generic all")
                Else
                    langData = siteApi.GetLanguageById(td.LanguageId)
                    tagLangLit.Text = langData.Name
                End If

				tagIdHdn.Value = td.Id.ToString
                tagLangIdHdn.Value = td.LanguageId.ToString

                SetupStatisticsGrid()
                tagStatsGrid.DataSource = GetStatisticsTable()
                tagStatsGrid.DataBind()

			End If

		Catch ex As Exception
		Finally
			td = Nothing
		End Try

    End Sub

    Public Sub SetupStatisticsGrid()

        Dim column As New System.Web.UI.WebControls.BoundColumn
        column.DataField = "ObjectType"
        column.HeaderText = m_containerPage.RefMsg.GetMessage("lbl tagged type") '"Type"
        column.HeaderStyle.CssClass = "ptagsHeaderType"
        column.Initialize()
        column.ItemStyle.Wrap = False
        column.ItemStyle.VerticalAlign = VerticalAlign.Middle
        column.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        tagStatsGrid.Columns.Add(column)

        column = New System.Web.UI.WebControls.BoundColumn
        column.DataField = "TaggedCount"
        column.HeaderText = m_containerPage.RefMsg.GetMessage("lbl times used") ' "Times Used"
        column.HeaderStyle.CssClass = "ptagsHeaderTagCount"
        column.Initialize()
        column.ItemStyle.Wrap = False
        column.ItemStyle.VerticalAlign = VerticalAlign.Middle
        column.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        tagStatsGrid.Columns.Add(column)

    End Sub

    Public Function GetStatisticsTable() As DataTable
        Dim table As New DataTable
        Dim row As DataRow
        Dim statistics As Generic.Dictionary(Of EkEnumeration.CMSObjectTypes, Long)
        Dim stat As Generic.KeyValuePair(Of EkEnumeration.CMSObjectTypes, Long)
        Dim typeLbl As String = ""

        table.Columns.Add(New DataColumn("ObjectType", GetType(String)))
        table.Columns.Add(New DataColumn("TaggedCount", GetType(String)))

        statistics = m_tagApi.GetTagStatistics(m_containerPage.TagId)
        For Each stat In statistics

            Select Case stat.Key
                Case EkEnumeration.CMSObjectTypes.Content
                    typeLbl = m_containerPage.RefMsg.GetMessage("content text")
                Case EkEnumeration.CMSObjectTypes.CommunityGroup
                    typeLbl = m_containerPage.RefMsg.GetMessage("lbl community groups")
                Case EkEnumeration.CMSObjectTypes.User
                    typeLbl = m_containerPage.RefMsg.GetMessage("generic users")
                Case EkEnumeration.CMSObjectTypes.Library
                    typeLbl = m_containerPage.RefMsg.GetMessage("generic library title")
                Case Else
                    typeLbl = stat.Key.ToString()
            End Select

            row = table.NewRow()
            row(0) = typeLbl
            row(1) = stat.Value.ToString()
            table.Rows.Add(row)
        Next

        Return table
    End Function



    Public Function DeleteTag() As Boolean
        Dim result As Boolean = False
        Dim td As TagData
        Dim tagId As Long = 0

        Try
            If (Not IsNothing(Request.Form(Me.tagIdHdn.UniqueID)) AndAlso IsNumeric(Request.Form(Me.tagIdHdn.UniqueID))) Then
                tagId = CType(Request.Form(Me.tagIdHdn.UniqueID), Long)
                If (tagId > 0) Then
                    result = m_tagApi.DeleteTagById(tagId)
                End If
            End If

        Catch ex As Exception
        Finally
            DeleteTag = result
            td = Nothing
        End Try
    End Function
End Class
