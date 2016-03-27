Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.Content
Imports System.Data

Partial Class controls_Community_PersonalTags_EditTag
	Inherits System.Web.UI.UserControl

    Protected m_containerPage As Community_PersonalTags
    Private m_tagApi As Community.TagsAPI

	Public Sub New()
        m_tagApi = New Community.TagsAPI()
    End Sub

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Dim refCommonAPI As New CommonApi()
        Dim successFlag As Boolean = False

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
				successFlag = SaveData()
			End If
			'Response.ClearContent()
			Response.Redirect("PersonalTags.aspx?action=viewall", False)
		Else
			LoadToolBar()
			DisplayInfo()
		End If

	End Sub

	Protected Sub LoadToolBar()
		Dim result As New System.Text.StringBuilder

		Try
			txtTitleBar.InnerHtml = m_containerPage.RefStyle.GetTitleBar(IIf(m_containerPage.TagId > 0, m_containerPage.RefMsg.GetMessage("edit personal tag page title"), m_containerPage.RefMsg.GetMessage("add personal tag page title")))
            result.Append("<table><tr>")
			result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(m_containerPage.AppImgPath & "../UI/Icons/save.png", "Javascript:doSubmit('" + tagValid.UniqueID + "');", m_containerPage.RefMsg.GetMessage("alt save btn text (personal tag)"), m_containerPage.RefMsg.GetMessage("btn save personal tag"), ""))
            result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(m_containerPage.AppImgPath & "../UI/Icons/back.png", "personaltags.aspx?action=" + IIf(m_containerPage.TagId > 0, "viewtag&id=" + m_containerPage.TagId.ToString, "viewall"), m_containerPage.RefMsg.GetMessage("alt back button"), m_containerPage.RefMsg.GetMessage("alt back button"), ""))
			'If (CStr(m_containerPage.RefCommonAPI.EnableMultilingual = "1")) Then
			'	result.Append("<td>&#160;|&#160;View:</td>")
			'	result.Append(m_containerPage.RefStyle.GetShowAllActiveLanguage(True, "", "javascript:SelLanguage(this.value);", Convert.ToString(m_containerPage.ContentLanguage)))
			'End If

			result.Append("<td>")
			result.Append(m_containerPage.RefStyle.GetHelpButton("EditTag_ascx"))
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
        Dim language As Integer

        Try 

            tagLangLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic language")
            tagNameLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic name")

            ' If editing an existing tag, show current values:
            If (0 < m_containerPage.TagId) Then
                tagIdLabelLit.Text = m_containerPage.RefMsg.GetMessage("generic id") + ":&#160;"
                td = m_tagApi.GetTagByID(m_containerPage.TagId)
                If (Not IsNothing(td)) Then
                    tagIdLit.Text = "<b>" + td.Id.ToString + "</b><br />"
                    tagNameTxt.Text = td.Text
                    tagDescTxt.Text = ""
                    tagLangIdHdn.Value = td.LanguageId.ToString

                    language = td.LanguageId
                End If
            Else
                language = m_containerPage.ContentLanguage

              
            End If

            If (language = -1) Then
                tagLangLit.Text = m_containerPage.RefMsg.GetMessage("generic all")
            Else
                langData = siteApi.GetLanguageById(language)
                tagLangLit.Text = langData.Name
            End If

        Catch ex As Exception
        Finally
            td = Nothing
        End Try

	End Sub

	Public Function SaveData() As Boolean
		Dim result As Boolean = False
		Dim td As TagData

		Try
			If (0 < m_containerPage.TagId) Then
				' Editing existing tag:
                td = m_tagApi.GetTagByID(m_containerPage.TagId)
				If (Not IsNothing(td)) Then
					td.Text = Request.Form.Item(Me.tagNameTxt.UniqueID)
					'td.Description = Request.Form.Item(Me.tagDescTxt.UniqueID)
                    td = m_tagApi.EditTag(td)
					result = ((Not IsNothing(td)) AndAlso (td.Id = m_containerPage.TagId))
				End If
			Else
				' Adding new tag:
				td = New TagData
                td.LanguageId = m_containerPage.RefUserApi.ContentLanguage
                td.Type = TagTypes.All
				td.Text = Request.Form.Item(Me.tagNameTxt.UniqueID)
				'td.Description = Request.Form.Item(Me.tagDescTxt.UniqueID)
                result = m_tagApi.AddTag(td)
			End If

		Catch ex As Exception
		Finally
			SaveData = result
			td = Nothing
		End Try
	End Function
End Class
