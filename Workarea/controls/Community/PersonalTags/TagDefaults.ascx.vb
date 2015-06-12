Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.Content
Imports System.Data
Partial Class controls_Community_PersonalTags_TagDefaults
    Inherits System.Web.UI.UserControl
    Dim chkTag As String
    Dim isAdmin As Boolean
    Dim permissionData As PermissionData
    Protected m_containerPage As Community_PersonalTags
    Protected imagePath As String = ""
    Private m_tagApi As Community.TagsAPI
    Dim _languageDataArray() As LanguageData
    Dim defaultTagObjectType As EkEnumeration.CMSObjectTypes


    ''' <summary>
    ''' Returns true if there are more than one languages enabled for the site.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsSiteMultilingual() As Boolean
        Get
            If (m_containerPage.RefCommonAPI.EnableMultilingual = 0) Then
                Return False
            End If
            Dim languageEnabledCount As Integer = 0
            For Each lang As LanguageData In LanguageDataArray
                If lang.SiteEnabled Then languageEnabledCount += 1
                If languageEnabledCount > 1 Then Exit For
            Next

            Return languageEnabledCount > 1
        End Get

    End Property
    Private ReadOnly Property LanguageDataArray() As LanguageData()
        Get
            If (_languageDataArray Is Nothing) Then
                Dim siteApi As New SiteAPI
                _languageDataArray = siteApi.GetAllActiveLanguages()
            End If

            Return _languageDataArray
        End Get
    End Property


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        m_containerPage = CType(Page, Community_PersonalTags)
        permissionData = m_containerPage.RefContentApi.LoadPermissions(0, "content")
        m_tagApi = New Community.TagsAPI()
        isAdmin = permissionData.IsAdmin
        imagePath = m_tagApi.AppPath & "images/ui/icons/"

        ' FireFox literally relies on the url object for the query string parse.
        If (Request.QueryString("objectType") IsNot Nothing) Then
            defaultTagObjectType = Integer.Parse(Request.QueryString("objectType"))
        Else
            defaultTagObjectType = Integer.Parse(Request.QueryString("amp;objectType"))
        End If

        Dim isLanguageSiteEnabled As Boolean = False
        For i As Integer = 0 To LanguageDataArray.Length - 1
            If (LanguageDataArray(i).Id = m_containerPage.ContentLanguage AndAlso LanguageDataArray(i).SiteEnabled) Then
                isLanguageSiteEnabled = True
                Exit For
            End If
        Next

        'the default tags page does not support all language - set to default.
        If (m_containerPage.ContentLanguage = ALL_CONTENT_LANGUAGES Or isLanguageSiteEnabled = False) Then
            m_containerPage.ContentLanguage = m_containerPage.RefCommonAPI.DefaultContentLanguage
            m_containerPage.RefCommonAPI.SetCookieValue("LastValidLanguageID", m_containerPage.RefCommonAPI.DefaultContentLanguage)
        End If

        If (IsPostBack()) Then
            SaveDefaults()
            Response.ClearContent()
            Response.Redirect("PersonalTags.aspx?action=viewall", False)
        Else
            LoadToolBar()
            RenderTags()
        End If
    End Sub

    Protected Sub LoadToolBar()
        Dim result As New System.Text.StringBuilder
        Dim title As String = ""
        Dim helpBtnText As String = ""
        Try

            Select Case defaultTagObjectType
                Case EkEnumeration.CMSObjectTypes.User
                    title = m_containerPage.RefMsg.GetMessage("lbl default user tags")
                    helpBtnText = "default_user_tags"
                Case EkEnumeration.CMSObjectTypes.CommunityGroup
                    title = m_containerPage.RefMsg.GetMessage("lbl default group tags")
                    helpBtnText = "default_group_tags"
                Case EkEnumeration.CMSObjectTypes.Content
                    title = m_containerPage.RefMsg.GetMessage("lbl default content tags")
                    helpBtnText = "default_content_tags"
                Case EkEnumeration.CMSObjectTypes.Library
                    title = m_containerPage.RefMsg.GetMessage("lbl default library tags")
                    helpBtnText = "default_library_tags"
                Case Else
                    title = m_containerPage.RefMsg.GetMessage("Default Tags")
                    helpBtnText = "default_tags"
            End Select

            divTitleBar.InnerHtml = m_containerPage.RefStyle.GetTitleBar(IIf(m_containerPage.TagId > 0, title, title))
            result.Append("<table><tr>")
            result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(imagePath & "save.png", "#", m_containerPage.RefMsg.GetMessage("alt save btn text (personal tag)"), m_containerPage.RefMsg.GetMessage("btn save personal tag"), "onClick=""javascript:doSubmit();return false;"""))
            result.Append(m_containerPage.RefStyle.GetButtonEventsWCaption(imagePath & "back.png", "personaltags.aspx?action=" + IIf(m_containerPage.TagId > 0, "viewtag&id=" + m_containerPage.TagId.ToString, "viewall"), m_containerPage.RefMsg.GetMessage("alt back button"), m_containerPage.RefMsg.GetMessage("alt back button"), ""))

            If (1 = m_containerPage.RefCommonAPI.EnableMultilingual) Then
                result.Append("<td class=""label"">&#160;" + m_containerPage.RefMsg.GetMessage("generic view") + ":</td>")
                result.Append(m_containerPage.RefStyle.GetShowAllActiveLanguage(False, "", "javascript:SelLanguage(this.value);", Convert.ToString(m_containerPage.RefCommonAPI.ContentLanguage), True))
            End If

            result.Append("<td>")
            result.Append(m_containerPage.RefStyle.GetHelpButton(helpBtnText))
            result.Append("</td>")
            result.Append("</tr></table>")

            divToolBar.InnerHtml = result.ToString
            result = Nothing
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

    Private Sub RenderTags()

        Dim defaultTags() As TagData = Nothing
        Dim tagData As TagData = Nothing
        Dim languageIndex As Integer = 2
        Dim htLanguages As New Hashtable()
        Dim tagHtmlBuilder As New StringBuilder()
        Dim originalTagListBuilder As New StringBuilder()

        error_InvalidChars.Text = m_containerPage.RefMsg.GetMessage("msg error Tag invalid chars")

        'Need a hashtable to track the index of each language in the language dropdown
        'this is used by the addFlagInit javascript function so that tha language dropdown value can be reset 
        'when the tags are redrawn via JS.
        Dim langCount As Integer = 0
        For i As Integer = 0 To Me.LanguageDataArray.Length - 1
            If (LanguageDataArray(i).SiteEnabled) Then
                If (Not htLanguages.Contains(LanguageDataArray(i).Id)) Then
                    htLanguages.Add(LanguageDataArray(i).Id, langCount)
                    langCount += 1
                End If
            End If
        Next

        tagHtmlBuilder.Append("<input type=""hidden"" name=""defaultLangIndex"" id=""defaultLangIndex"" value=""" & htLanguages(m_containerPage.RefCommonAPI.DefaultContentLanguage) & """ /> ")
        tagHtmlBuilder.Append("<input type=""hidden"" name=""defaultLang"" id=""defaultLangIndex"" value=""" & m_containerPage.RefCommonAPI.DefaultContentLanguage & """ /> ")

        defaultTags = m_tagApi.GetDefaultTags(defaultTagObjectType, m_containerPage.ContentLanguage)

        tagHtmlBuilder.Append("<p id=""parah""></p>")
        tagHtmlBuilder.Append("<input type=""hidden"" id=""Flaglength"" name=""Flaglength"" value=""" & defaultTags.Length.ToString() & """ /><div id=""pFlag"" name=""pFlag"">" & Environment.NewLine)
        Dim sIndent As String = "&nbsp;"
        If isAdmin Then
            If (defaultTags.Length > 0) Then
                For i As Integer = 0 To (defaultTags.Length - 1)

                    'keep list of original tags so we can track which ones were removed on Save
                    'Format <TagText>~<LanguageID>;<TagText>~<LanguageID>
                    If (originalTagListBuilder.Length > 0) Then originalTagListBuilder.Append(";")
                    originalTagListBuilder.Append(defaultTags(i).Text)

                    tagHtmlBuilder.Append("<script type=""text/javascript"">addFlagInit(" & defaultTags(i).Id.ToString() & ",'" & defaultTags(i).Text & "'," & defaultTags(i).LanguageId & "," & htLanguages(defaultTags(i).LanguageId) & " );</script>")
                    tagHtmlBuilder.Append("<input type=""hidden"" name=""flag_iden" & i.ToString() & """ id=""flag_iden" & i.ToString() & """ value=""" & defaultTags(i).Id.ToString() & """ /> ")
                    tagHtmlBuilder.Append("<input type=""text"" id=""flagdefopt" & i.ToString() & """ name=""flagdefopt" & i.ToString() & """ value=""" & Server.HtmlEncode(defaultTags(i).Text) & """ maxlength=""50"" onChange=""javascript:saveFlag(" & i.ToString() & ",this.value,'tagText');"">")


                    tagHtmlBuilder.Append("&#160;")

                    tagHtmlBuilder.Append("<a href=""#"" onclick=""javascript:removeFlag('" + i.ToString() + "'); return false;""><img src=""" & imagePath & "remove.png"" border=""0""/></a>")

                    tagHtmlBuilder.Append("<div class=""ektronTopSpaceSmall""></div>")
                Next
            Else
                tagHtmlBuilder.Append("<script type=""text/javascript"">addFlagInit(0, '', " & m_containerPage.RefCommonAPI.DefaultContentLanguage & "," & htLanguages(m_containerPage.RefCommonAPI.DefaultContentLanguage) & " );</script>")
                tagHtmlBuilder.Append("<input type=""hidden"" name=""flag_iden0"" id=""flag_iden0"" value=""0"" /> ")
                tagHtmlBuilder.Append("<input type=""text"" id=""flagdefopt0"" name=""flagdefopt0"" value="""" maxlength=""50"" onChange=""javascript:saveFlag(0,this.value,'tagText');"">")

                tagHtmlBuilder.Append("&#160;")
            End If
            tagHtmlBuilder.Append("</div>")

            tagHtmlBuilder.Append(Environment.NewLine)
            tagHtmlBuilder.Append("<script type=""text/javascript"">" & Environment.NewLine)
            tagHtmlBuilder.Append(GetJs() & Environment.NewLine)
            tagHtmlBuilder.Append("</script>" & Environment.NewLine)
            literalTags.Text = tagHtmlBuilder.ToString()
            literalAddTag.Text = "<a href=""#"" title=""" & m_containerPage.RefMsg.GetMessage("btn add personal tag") & """ onclick=""javascript:addFlag(); return false;""><img src=""" & imagePath & "add.png"" border=""0""/></a><br/>"

            'orginalTaglist - for access on postback
            literalAddTag.Text &= "<input type=""hidden"" id=""originalTags"" name=""originalTags"" value=""" & originalTagListBuilder.ToString() & """/>"

        End If
    End Sub


    Private Sub SaveDefaults()

        Dim newTags As String = ""
        If (Not IsNothing(Request.Form.Item("newTags"))) Then
            newTags = Request.Form.Item("newTags").Trim
        End If

        Dim newTagsArray() As String = newTags.Split(";")
        Dim newTagsList As New System.Collections.Generic.List(Of String)(newTagsArray)

        Dim originalTagHT As New Hashtable
        Dim orginalTagList As String
        orginalTagList = Request.Form.Item("originalTags").Trim

        'loop through existing default tags and make sure they havent been removed - if so delete them
        'also - build up a hashtable of original default tags so we can reference them later
        'both tag lists are stored in following format:  <TagText>~<LanguageID>;<TagText>~<LanguageID>;
        Dim origTagsArray() As String = orginalTagList.Split(";")
        For Each tag As String In origTagsArray
            chkTag = tag
            originalTagHT.Add(tag, "")
            Dim tagMatch As String = newTagsList.Find(AddressOf FindTagByName)

            If (String.IsNullOrEmpty(tagMatch)) Then
                m_tagApi.RemoveTagAsDefault(tag, m_containerPage.ContentLanguage, defaultTagObjectType)
            End If
        Next

        ' loop throug tagdata stored in newtags field
        For Each tag As String In newTagsList
            m_tagApi.SaveTagAsDefault(tag, m_containerPage.ContentLanguage, defaultTagObjectType)
        Next

    End Sub
    Private Function FindTagByName(ByVal str As String) As Boolean
        If str.ToLower = chkTag.ToLower Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Function GetJs() As String
        Dim sbJs As New StringBuilder()

        sbJs.Append("function createFlag(fid,id,fname, iloc, itot) {").Append(Environment.NewLine)
        sbJs.Append("       var sRet = """";").Append(Environment.NewLine)
        sbJs.Append("       sRet += ""&nbsp;&nbsp;<input type=\""text\"" id=\""flagdefopt"" + id + ""\"" name=\""flagdefopt"" + id + ""\"" value=\"""" + fname + ""\"" maxlength=\""50\"" onChange=\""javascript:saveFlag("" + id + "",this.value,'tagText')\"" />"";").Append(Environment.NewLine)
        sbJs.Append("       sRet += ""<input type=\""hidden\"" name=\""flag_iden"" + id + ""\"" id=\""flag_iden"" + id + ""\"" value=\"""" + fid + ""\"" />"";").Append(Environment.NewLine)
        sbJs.Append("       sRet += ""&#160;""").Append(Environment.NewLine)

        sbJs.Append("       sRet += ""&nbsp;&nbsp;<a href=\""#\"" onclick=\""javascript:removeFlag("" + id + ""); return false;\""><img src=\""" & Me.imagePath & "remove.png\"" border=\""0\""/></a>"";").Append(Environment.NewLine)
        sbJs.Append("       sRet += ""<br/>"";").Append(Environment.NewLine)
        sbJs.Append("       return sRet; ").Append(Environment.NewLine)
        sbJs.Append("}").Append(Environment.NewLine)

        Return sbJs.ToString()
    End Function

    Private Function GetLanguageDropDownOptionMarkup(ByVal selectedlanguageId As Integer) As String

        Dim i As Integer
        Dim markup As New StringBuilder

        If (IsSiteMultilingual) Then
            If (Not (IsNothing(LanguageDataArray))) Then
                For i = 0 To LanguageDataArray.Length - 1
                    If (LanguageDataArray(i).SiteEnabled) Then
                        markup.Append("<option ")
                        If LanguageDataArray(i).Id = selectedlanguageId Then
                            markup.Append(" selected")
                        End If
                        markup.Append(" value=" & LanguageDataArray(i).Id & ">" & LanguageDataArray(i).LocalName)
                    End If
                Next
            End If
        Else
            'hardcode to default site language
            markup.Append(" <option selected value=" & m_containerPage.RefCommonAPI.DefaultContentLanguage & ">")
        End If

        Return markup.ToString()
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Sub New()

    End Sub
End Class
