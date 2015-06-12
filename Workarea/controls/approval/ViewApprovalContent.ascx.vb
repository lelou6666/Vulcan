Imports Ektron.Cms
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration

Partial Class ViewApprovalContent
    Inherits System.Web.UI.UserControl
    Private m_refAPI As New CommonApi
    Private m_refContent As EkContent
    Private m_refContentApi As New ContentAPI
    Private m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_refStyle As New StyleHelper
    Private m_cCont As Collection
    Private m_meObj As Collection
    Private cApprovals As Collection
    Private m_TaskExists As Boolean
    Private m_sPage As String
    Private toggle As String
    Protected ContentLanguage As Integer = -1
    Private CurrentUserId As Long = 0
    Private ekrw As ektUrlRewrite
    Protected SitePath As String = ""
    ' blog - SK
    Private m_bIsBlog As Boolean = False
    Private blog_post_data As Ektron.Cms.BlogPostData
    Private arrBlogPostCategories As String()
    Dim i As Integer = 0
    Private aprId As Long = 0
    'END: blog - SK

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim fldid As String
        m_refMsg = m_refAPI.EkMsgRef
        m_sPage = Request.QueryString("page")
        aprId = CLng(Request.QueryString("id"))
        CurrentUserId = m_refAPI.UserId
        SitePath = m_refAPI.SitePath
        RegisterResources()

        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            If (Request.QueryString("LangType") <> "") Then
                ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                m_refAPI.SetCookieValue("LastValidLanguageID", ContentLanguage)
            Else
                If m_refAPI.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(m_refAPI.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If m_refAPI.GetCookieValue("LastValidLanguageID") <> "" Then
                ContentLanguage = Convert.ToInt32(m_refAPI.GetCookieValue("LastValidLanguageID"))
            End If
        End If
        m_refAPI.ContentLanguage = ContentLanguage
        m_refContent = m_refAPI.EkContentRef

        If (Request.QueryString("content") = "published") Then
            m_cCont = m_refContent.GetContentByIDv2_0(aprId)
            toggle = "staged"
        Else
            m_cCont = m_refContent.GetStagedContByIDv2_0(aprId)
            toggle = "published"
            If (m_cCont.Count = 0) Then
                m_cCont = Nothing
                m_cCont = m_refContent.GetContentByIDv2_0(aprId)
                toggle = "staged"
                If (m_sPage = "workarea") Then
                    Response.Redirect("approval.aspx?action=viewApprovalList&id=" & Request.QueryString("fldid"), False)
                Else
                    Response.Write("<script language=""Javascript"">" & _
                                    "top.opener.location.reload(true);" & _
                                    "top.close();" & _
                                    "</script>")
                End If
            End If
        End If
        m_meObj = m_refContent.CanIv2_0(aprId, "content")
        fldid = m_cCont("FolderID")

        cApprovals = m_refContent.GetCurrentApprovalInfoByIDv1_1(aprId)

        m_TaskExists = m_refContent.DoesTaskExistForContent(m_cCont("ContentID"))
        ViewContent()
    End Sub

    Private Sub ViewContent()
        Dim bCanAlias As Boolean = False

        ekrw = m_refAPI.EkUrlRewriteRef()
        'ekrw.Load()
        'If ((m_refAPI.RedirectorOn = True) _
        '   And (ekrw.aliasOn = True) _
        '   And m_refAPI.IsARoleMember(Common.EkEnumeration.CmsRoleIds.EditAlias)) Then
        '    bCanAlias = True
        'End If

        ViewToolBar()
        Dim result As New System.Text.StringBuilder
        Dim cApproval As Collection

        blog_post_data = New BlogPostData
        blog_post_data.Categories = Array.CreateInstance(GetType(String), 0)
        For Each cApproval In m_cCont("ContentMetadata")
            If (CInt(cApproval("ObjectType")) > 0) Then
                Select Case CInt(cApproval("ObjectType"))
                    Case BlogPostDataType.Categories
                        Dim sTmp As String = cApproval("MetaText")
                        sTmp = sTmp.Replace("&#39;", "'")
                        sTmp = sTmp.Replace("&quot", """")
                        sTmp = sTmp.Replace("&gt;", ">")
                        sTmp = sTmp.Replace("&lt;", "<")
                        blog_post_data.Categories = Split(sTmp, ";")
                    Case BlogPostDataType.Ping
                        If Not (cApproval("MetaText").ToString().Trim().ToLower() = "no") Then
                            m_bIsBlog = True
                        End If
                        blog_post_data.Pingback = Ektron.Cms.Common.EkFunctions.GetBoolFromYesNo(cApproval("MetaText"))
                    Case BlogPostDataType.Tags
                        blog_post_data.Tags = cApproval("MetaText")
                    Case BlogPostDataType.Trackback
                        blog_post_data.TrackBackURL = cApproval("MetaText")
                End Select
            End If
        Next

        result.Append("<div class=""ektronPageContainer ektronPageTabbed"">")
        result.Append("<div class=""tabContainerWrapper"">")
        result.Append("<div class=""tabContainer""><ul>")
        result.Append("<li><a href=""#dvContent"">" & (m_refMsg.GetMessage("content text")) & "</a></li>")
        result.Append("<li><a href=""#dvSummary"">" & (m_refMsg.GetMessage("Summary text")) & "</a></li>")

        If True = bCanAlias Then
            result.Append("<li><a href=""#dvAlias"">" & (m_refMsg.GetMessage("lbl alias")) & "</a></li>")
        End If

        result.Append("<li><a href=""#dvMetadata"">" & (m_refMsg.GetMessage("metadata text")) & "</a></li>")
        result.Append("<li><a href=""#dvProperties"">" & (m_refMsg.GetMessage("properties text")) & "</a></li>")
        result.Append("<li><a href=""#dvComment"">" & (m_refMsg.GetMessage("comment text")) & "</a></li>")

        'Taxonomy
        result.Append("<li><a href=""#dvTaxonomy"">" & (m_refMsg.GetMessage("viewtaxonomytabtitle")) & "</a></li>")
        result.Append("</ul>")

        result.Append("<div id=""dvProperties"">")
        result.Append("<table class=""ektronGrid"">")
        result.Append("<tr>")
        result.Append("<td class=""label"">" & (m_refMsg.GetMessage("content title label")) & "</td>")
        result.Append("<td>" & m_cCont("ContentTitle") & "</td>")
        result.Append("</tr>")
        result.Append("<tr>")
        result.Append("<td class=""label"">" & (m_refMsg.GetMessage("content id label")) & "</td>")
        result.Append("<td>" & (m_cCont("ContentID")) & "</td>")
        result.Append("</tr>")
        result.Append("<tr>")
        result.Append("<td class=""label"">" & (m_refMsg.GetMessage("content status label")) & "</td>")
        result.Append("<td>")
        If (CStr(m_cCont("ContentStatus")) = CStr("S")) Then
            result.Append(m_refMsg.GetMessage("status:Submitted for Approval"))
        Else
            result.Append(m_refMsg.GetMessage("status:Submitted for Deletion"))
        End If
        result.Append("</td>")
        result.Append("</tr>")
        result.Append("<tr>")
        result.Append("<td class=""label"">" & m_refMsg.GetMessage("submitted by label") & "</td>")
        result.Append("<td>")
        result.Append(m_cCont("EditorLName") & ", " & m_cCont("EditorFName"))
        result.Append("</td>")
        result.Append("</tr>")
        result.Append("<tr>")
        result.Append("<td class=""label"">" & m_refMsg.GetMessage("content LED label") & "</td>")
        result.Append("<td>" & m_cCont("DisplayLastEditDate") & "</td>")
        result.Append("</tr>")
        result.Append("<tr>")
        result.Append("<td class=""label"">" & m_refMsg.GetMessage("generic start date label") & "</td>")
        result.Append("<td>")
        If (m_cCont("DisplayGoLive") <> "") Then
            result.Append(m_cCont("DisplayGoLive"))
        Else
            result.Append(m_refMsg.GetMessage("none specified msg"))
        End If
        result.Append("</td>")
        result.Append("</tr>")
        result.Append("<tr>")
        result.Append("<td class=""label"">" & m_refMsg.GetMessage("generic end date label") & "</td>")
        result.Append("<td>")
        If (m_cCont("DisplayEndDate") <> "") Then
            result.Append(m_cCont("DisplayEndDate"))
        Else
            result.Append(m_refMsg.GetMessage("none specified msg"))
        End If
        result.Append("</td>")
        result.Append("</tr>")
        result.Append("<tr>")
        result.Append("<td class=""label"">" & (m_refMsg.GetMessage("content DC label")) & "</td>")
        result.Append("<td>" & (m_cCont("DisplayDateCreated")) & "</td>")
        result.Append("</tr>")
        result.Append("<tr>")
        result.Append("<td class=""label"">" & (m_refMsg.GetMessage("content approvals label")) & "</td>")
        result.Append("<td>")
        If (cApprovals.Count > 0) Then
            For Each cApproval In cApprovals
                If (LCase(cApproval("ApproverType")) = "user") Then
                    result.Append("<img class=""imgUsers"" src=""" & m_refAPI.AppPath & "images/UI/Icons/user.png"" align=""absbottom"" alt=""" & m_refMsg.GetMessage("approver is user") & """ title=""" & m_refMsg.GetMessage("approver is user") & """/>")
                Else
                    result.Append("<img class=""imgUsers"" src=""" & m_refAPI.AppPath & "images/UI/Icons/users.png"" align=""absbottom"" alt=""" & m_refMsg.GetMessage("approver is user group") & """ title=""" & m_refMsg.GetMessage("approver is user group") & """/>")
                End If
                If (cApproval("CurrentApprover")) Then
                    result.Append("<font color=""""red"""">")
                Else
                    result.Append(" <font> ")
                End If
                result.Append(cApproval("Name"))
            Next
        Else
            result.Append(m_refMsg.GetMessage("none specified msg"))
        End If
        result.Append("</td>")
        result.Append("</tr>")
        result.Append("</table>")
        result.Append("</div>")

        result.Append("<div id=""dvMetadata"">")
        result.Append("<table class=""ektronGrid"">")
        For Each cApproval In m_cCont("ContentMetadata")
            If Not (CInt(cApproval("ObjectType")) > 0) Then
                result.Append("<tr>")
                result.Append("<td class=""label"">" & (cApproval("MetaTypeName")) & ":</td>")
                result.Append("<td>" & (cApproval("MetaText")) & "</td>")
                result.Append("</tr>")
            End If
        Next
        result.Append("</table>")
        result.Append("</div>")

        If True = bCanAlias Then
            Dim m_strAliasPageName As String = String.Empty

            If Request.QueryString("content") = "published" Then
                'm_strAliasPageName = ekrw.GetPrimaryAliasedPageNameByCID(m_cCont("ContentID"))
            Else
                m_strAliasPageName = m_cCont("ManualAlias")
            End If

            If (m_strAliasPageName <> "") Then
                m_strAliasPageName = SitePath & m_strAliasPageName
            Else
                m_strAliasPageName = " [Not Defined]"
            End If

            result.Append("<DIV id=""dvAlias""")
            result.Append("	<TABLE class=""ektronGrid"">")
            result.Append("<TR>")
            result.Append("<TD class=""label"">" & m_refMsg.GetMessage("lbl aliased page") & ":""</TD>")
            result.Append("<TD>" & m_strAliasPageName & "</TD>")
            result.Append("</TR>")
            result.Append("</TABLE>")
            result.Append("</DIV>")

        End If

        result.Append("<div id=""dvSummary"">")
        result.Append("<table class=""ektronGrid"">")
        result.Append("<tr>")

        Dim strTeaser As String
        Dim nContentType As Integer
        strTeaser = m_cCont("ContentTeaser")
        If IsNumeric(m_cCont("ContentType")) Then
            nContentType = CInt(m_cCont("ContentType"))
        Else
            nContentType = CMSContentType.Content ' default
        End If
        If CMSContentType.Forms = nContentType Or CMSContentType.Archive_Forms = nContentType Then
            If Not IsNothing(strTeaser) Then
                If strTeaser.IndexOf("<ektdesignpackage_design") > -1 Then
                    Dim strDesign As String
                    strDesign = m_refAPI.XSLTransform(strTeaser, _
                      Server.MapPath(m_refAPI.AppeWebPath() & "unpackageDesign.xslt"), XsltAsFile:=True, _
                      ReturnExceptionMessage:=True)
                    strTeaser = strDesign
                Else
                    strTeaser = strTeaser
                End If
            Else
                strTeaser = ""
            End If
        End If

        result.Append("<td class=""label"">")
        result.Append(m_refMsg.GetMessage("lbl teaser"))
        result.Append(":</td><td>")
        result.Append(strTeaser)
        result.Append("</td></tr>")

        If m_bIsBlog Then
            result.Append("<tr><td class=""label"">")
            result.Append(m_refMsg.GetMessage("lbl tags"))
            result.Append(":</td><td>")
            If Not (blog_post_data Is Nothing) Then
                result.AppendLine(blog_post_data.Tags)
            End If
            result.AppendLine("</td></tr>")

            result.Append("<tr><td class=""label"">")
            result.Append(m_refMsg.GetMessage("categories text"))
            result.Append(":</td><td>")
            If Not (blog_post_data.Categories Is Nothing) Then
                arrBlogPostCategories = blog_post_data.Categories
                If arrBlogPostCategories.Length > 0 Then
                    Array.Sort(arrBlogPostCategories)
                End If
            Else
                arrBlogPostCategories = Nothing
            End If
            If blog_post_data.Categories.Length > 0 Then
                For i = 0 To (blog_post_data.Categories.Length - 1)
                    If blog_post_data.Categories(i).ToString() <> "" Then
                        result.AppendLine("				<input type=""checkbox"" name=""blogcategories" & i.ToString() & """ value=""" & blog_post_data.Categories(i).ToString() & """ checked=""true"" disabled>&nbsp;" & Replace(blog_post_data.Categories(i).ToString(), "~@~@~", ";") & "<br>")
                    End If
                Next
            Else
                result.AppendLine("No categories defined.")
            End If
            result.Append("</td></tr>")

            result.Append("<tr><td class=""label"">")
            result.Append(m_refMsg.GetMessage("lbl trackback url"))
            result.Append(":</td><td>")
            result.AppendLine("<input type=""hidden"" name=""blogposttrackbackid"" id=""blogposttrackbackid"" value=""")
            If Not (blog_post_data Is Nothing) Then
                result.Append(blog_post_data.TrackBackURLID.ToString())
            End If
            result.Append(""" /><input type=""hidden"" id=""isblogpost"" name=""isblogpost"" value=""true""/>")
            If Not (blog_post_data Is Nothing) Then
                result.AppendLine("<input type=""text"" size=""75"" id=""trackback"" name=""trackback"" value=""" & Server.HtmlEncode(blog_post_data.TrackBackURL) & """ disabled/>")
            End If

            result.Append("<tr><td class=""label"">")
            result.Append(m_refMsg.GetMessage("lbl blog ae ping"))
            result.Append(":</td><td>")
            result.Append("<input type=""checkbox"" name=""pingback"" id=""pingback"" ")
            If Not (blog_post_data Is Nothing) Then
                If blog_post_data.Pingback = True Then
                    result.Append("checked ")
                End If
            End If
            result.Append(" disabled/>")

            result.AppendLine("</td>")
            result.AppendLine("</tr>")
            result.AppendLine("</table>")
        End If
        result.Append(" </td>")
        result.Append("</tr>")
        result.Append("</table>")
        result.Append("</div>")

        result.Append("<div id=""dvComment"">")
        result.Append("<table class=""ektronGrid"">")
        result.Append("<tr>")
        result.Append("<td class=""label"">" & (m_refMsg.GetMessage("content HC label")) & "</td>")
        result.Append("<td>" & (m_cCont("Comment")) & "</td>")
        result.Append("</tr>")
        result.Append("</table>")
        result.Append("</div>")

        'Taxonomy
        result.Append("<div id=""dvTaxonomy"">")
        result.Append("<table class=""ektronGrid"">")
        result.Append("<tr><td class=""label"">Assigned Taxonomy/Category:</td><td><table>")
        Dim taxonomy_cat_arr As TaxonomyBaseData() = Nothing
        taxonomy_cat_arr = m_refContent.ReadAllAssignedCategory(aprId)
        If (taxonomy_cat_arr IsNot Nothing AndAlso taxonomy_cat_arr.Length > 0) Then
            For Each taxonomy_cat As TaxonomyBaseData In taxonomy_cat_arr
                result.Append("<tr>")
                result.Append("<td><li>" & taxonomy_cat.TaxonomyPath.Remove(0, 1).Replace("\", " > ") & "</li></td>")
                result.Append("</tr>")
            Next
        Else
            result.Append("<tr><td>&nbsp;</td><td>No categories selected.</td></tr>")
        End If
        result.Append("</table></td></tr></table>")
        result.Append("</div>")


        result.Append("<div id=""dvContent"">")

        Dim bPackageDisplayXSLT As Boolean
        Dim CurrentXslt As String
        bPackageDisplayXSLT = False
        CurrentXslt = ""
        If (m_cCont("XmlConfiguration").Count) Then
            'check to see if there is alread a defualt display XSLT
            If Len(m_cCont("XmlConfiguration")("PackageDisplayXslt")) Then
                bPackageDisplayXSLT = True
            Else
                If (m_cCont("XmlConfiguration")("DefaultXslt") > 0) Then
                    bPackageDisplayXSLT = False
                    If (Len(m_cCont("XmlConfiguration")("PhysPathComplete")("Xslt" & m_cCont("XmlConfiguration")("DefaultXslt")))) Then
                        CurrentXslt = m_cCont("XmlConfiguration")("PhysPathComplete")("Xslt" & m_cCont("XmlConfiguration")("DefaultXslt"))
                    Else
                        CurrentXslt = m_cCont("XmlConfiguration")("LogicalPathComplete")("Xslt" & m_cCont("XmlConfiguration")("DefaultXslt"))
                    End If
                Else
                    bPackageDisplayXSLT = True
                End If
            End If

            If (bPackageDisplayXSLT) Then
                result.Append(m_refAPI.TransformXsltPackage(m_cCont("ContentHtml"), m_cCont("XmlConfiguration")("PackageDisplayXslt"), False))
            Else
                result.Append(m_refAPI.TransformXSLT(m_cCont("ContentHTML"), CurrentXslt))
            End If
        Else
            '----- Defect #28122 - Content tab is blank when viewing dms asset from View Approval Report screen.
            '----- Only contentHtml was being added to the content tab div.  Int he case of an asset, it must be
            '----- downloaded.
            If (Ektron.Cms.Common.EkConstants.IsAssetContentType(m_cCont("ContentType"))) Then
                If (m_cCont("ContentStatus") <> "A" AndAlso Convert.ToString(Request.QueryString("action")).ToLower.Trim = "view") Then
                    result.Append("<iframe width=""100%"" height=""100%"" src=""" & m_refContentApi.GetViewUrl(m_cCont("AssetID"), m_cCont("ContentType")) & """></iframe>")
                Else
                    Dim ver As String = ""
                    'If (m_strPageAction = "viewstaged" Or m_strPageAction = "view") Then
                    ver = "&version=" & m_cCont("AssetVersion")
                    'End If
                    result.Append("<iframe width=""100%"" height=""100%"" src=""" & m_refContentApi.SitePath & "assetmanagement/DownloadAsset.aspx?ID=" & m_cCont("AssetID") & ver & """></iframe>")
                End If
            Else
                If (Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData = m_cCont("ContentSubType")) Then
                    result.Append(Ektron.Cms.PageBuilder.PageData.RendertoString(m_cCont("ContentHTML")))
                Else
                    result.Append(m_cCont("ContentHTML"))
                End If
            End If
        End If
        result.Append("</div>")

        result.Append("</div>")      'tabContainer
        result.Append("</div>")      'tabContainerWrapper
        result.Append("</div>")   'ektronPageContainer ektronPageTabbed
        litViewContent.Text = result.ToString()

    End Sub

    Private Sub ViewToolBar()
        Dim AltPublishMsg, AltApproveMsg, AltDeclineMsg, PublishIcon, CaptionKey As String
        Dim result As New System.Text.StringBuilder
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("generic approve title") & " '" & m_cCont("ContentTitle") & "'?")
        result.Append("<table><tr>")
        If (CStr(m_cCont("ContentStatus")) = CStr("S") Or CStr(m_cCont("ContentStatus")) = CStr("M")) Then
            If (CStr(m_cCont("ContentStatus")) = CStr("S")) Then
                AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (change)")
                AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (change)")
                AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (change)")
                PublishIcon = "../UI/Icons/contentPublish.png"
                CaptionKey = "btn publish"
            Else
                AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (delete)")
                AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (delete)")
                AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (delete)")
                PublishIcon = "../UI/Icons/delete.png"
                CaptionKey = "btn delete"
            End If
            If (m_meObj("CanIPublish")) Then
                If m_TaskExists = True Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppImgPath & PublishIcon, "#", AltPublishMsg, m_refMsg.GetMessage(CaptionKey), "Onclick=""javascript:return LoadChildPage('action=approveContentAction&id=" & m_cCont("ContentID") & "&fldid=" & m_cCont("FolderID") & "&page=" & m_sPage & "&LangType=" & m_refAPI.ContentLanguage & "');"""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppImgPath & PublishIcon, "approval.aspx?action=approveContentAction&id=" & m_cCont("ContentID") & "&fldid=" & m_cCont("FolderID") & "&page=" & m_sPage & "&LangType=" & m_refAPI.ContentLanguage & "", AltPublishMsg, m_refMsg.GetMessage(CaptionKey), ""))
                End If
            ElseIf m_meObj("CanIApprove") Then
                If m_TaskExists = True Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/approvalApproveItem.png", "#", AltApproveMsg, m_refMsg.GetMessage("btn approve"), "Onclick=""javascript:return LoadChildPage('action=approveContentAction&id=" & m_cCont("ContentID") & "&fldid=" & m_cCont("FolderID") & "&page=" & m_sPage & "&LangType=" & m_refAPI.ContentLanguage & "');"""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/approvalApproveItem.png", "approval.aspx?action=approveContentAction&id=" & m_cCont("ContentID") & "&fldid=" & m_cCont("FolderID") & "&page=" & m_sPage & "&LangType=" & m_refAPI.ContentLanguage & "", AltApproveMsg, m_refMsg.GetMessage("btn approve"), ""))
                End If
            End If
            If (m_meObj("CanIPublish") Or m_meObj("CanIApprove")) Then
                If m_TaskExists = True Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/approvalDenyItem.png", "#", AltDeclineMsg, m_refMsg.GetMessage("btn decline"), "Onclick=""javascript:return LoadChildPage('action=declineContentAction&id=" & m_cCont("ContentID") & "&fldid=" & m_cCont("FolderID") & "&page=" & m_sPage & "&LangType=" & m_refAPI.ContentLanguage & "');"""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/approvalDenyItem.png", "javascript:DeclineContent('" & m_cCont("ContentID") & "', '" & m_cCont("FolderID") & "', '" & m_sPage & "', '" & m_refAPI.ContentLanguage & "')", AltDeclineMsg, m_refMsg.GetMessage("btn decline"), ""))
                End If
            End If
            If (m_meObj("CanIEditSubmitted")) Then
                If m_cCont("ContentType") <> 3333 AndAlso Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent <> m_cCont("ContentSubType") Then
                    result.Append(Me.m_refStyle.GetEditAnchor(m_cCont("ContentID"), , True, m_cCont("ContentSubType")))
                End If
                If Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData = m_cCont("ContentSubType") Then
                    result.Append(m_refStyle.GetPageBuilderEditAnchor(m_cCont("ContentID"), m_cCont("ContentLanguage"), m_cCont("Quicklink")))
                End If
                'result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/contentEdit.png", "#", m_refMsg.GetMessage("alt edit button text"), m_refMsg.GetMessage("btn edit"), "OnClick=""javascript:PopEditWindow('editarea.aspx?id=" & ID & "&LangType=" & m_refAPI.ContentLanguage & "&type=update&pullapproval=true'" & ",'EDIT',790,580,1,1);return false;"""))
            End If
            If ((toggle = "published") And (CStr(m_cCont("ContentStatus")) = CStr("S"))) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/contentViewPublished.png", "approval.aspx?LangType=" & m_refAPI.ContentLanguage & "&action=viewContent&id=" & m_cCont("ContentID") & "&content=" & toggle & "&fldid=" & m_cCont("FolderID") & "&page=" & m_sPage & "", m_refMsg.GetMessage("alt view published button text (approvals)"), m_refMsg.GetMessage("btn view publish"), ""))
                If Not (IsAssetContentType(m_cCont("ContentType")) OrElse Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData = m_cCont("ContentSubType") OrElse Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent = m_cCont("ContentSubType")) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/contentViewDifferences.png", "#", "View Difference", m_refMsg.GetMessage("btn view diff"), "onclick=""PopEditWindow('compare.aspx?id=" & m_cCont("ContentID") & "&LangType=" & m_refAPI.ContentLanguage & "', 'Compare', 785, 650, 1, 1);"""))
                End If
            ElseIf (CStr(m_cCont("ContentStatus")) = CStr("S")) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/preview.png", "approval.aspx?LangType=" & m_refAPI.ContentLanguage & "&action=viewContent&id=" & m_cCont("ContentID") & "&content=" & toggle & "&fldid=" & m_cCont("FolderID") & "&page=" & m_sPage & "", m_refMsg.GetMessage("alt view staged button text (approvals)"), m_refMsg.GetMessage("btn view staged"), ""))
            End If
            If m_TaskExists = True Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppImgPath & "btn_viewtask-nm.gif", "tasks.aspx?action=viewcontenttask&ty=both&cid=" & m_cCont("ContentID") & "&callbackpage=content.aspx&parm1=action&value1=" & Request.QueryString("action") & "&parm2=id&value2=" & m_cCont("ContentID") & "&parm3=LangType&value3=" & m_refAPI.ContentLanguage & "&LangType=" & m_refAPI.ContentLanguage, "View Task", m_refMsg.GetMessage("btn view task"), ""))
                'result.Append(GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/commentAdd.png", "javascript:openComment(" & cCont("ContentID") & ");" , "Add Comment", m_refMsg.GetMessage("btn add comment"), ""))
            End If
        End If
        If (Request.QueryString("page") = "workarea") Then
            ' redirect to workarea when user clicks back button if we're in workarea
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(Request.QueryString("action")))
        result.Append("</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)

        ' Language Translation String
        ltr_decline.Text = m_refMsg.GetMessage("reason to decline")
    End Sub
End Class
