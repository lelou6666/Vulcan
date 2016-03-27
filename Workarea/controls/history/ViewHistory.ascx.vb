Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Commerce
Imports System.Collections.Generic
Imports Ektron.Cms.common
Imports Ektron.Cms.Workarea

Partial Class ViewHistory
    Inherits System.Web.UI.UserControl

#Region "Private members"

    Protected AppName As String = ""
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_refStyle As New StyleHelper
    Protected content_data As ContentData
    Protected ContentId As Long = -1
    Protected HistoryId As Long = -1
    Protected AppImgPath As String = ""
    Protected m_refContentApi As ContentAPI
    Protected security_data As PermissionData
    Protected ContentLanguage As Integer = -1
    Protected bXmlContent As Boolean = False
    Protected bApplyXslt As Boolean = False
    Protected hist_content_data As ContentData
    Protected bIsBlog As Boolean = False
    Private blog_post_data As Ektron.Cms.BlogPostData
    Private arrBlogPostCategories As String()
    Private i As Integer = 0

    'Commerce declarations begins
    Protected m_contentType As Long = EkConstants.CMSContentType_Content
    Protected m_refCatalog As CatalogEntry = Nothing
    Protected entry_data As EntryData = Nothing
    Protected entry_version_data As EntryData = Nothing
    Protected m_intId As Integer = 0
    Protected attrib_data As New List(Of EntryAttributeData)
    Protected m_refSite As Ektron.Cms.Site.EkSite = Nothing
    Protected m_iFolder As Integer = 0
    Protected m_sEditAction As String = ""
    Protected catalog_data As New FolderData
    Protected lValidCounter As Integer = 0
    Protected xid As Integer = 0
    Protected imagePath As String = ""
    'Commerce declaration ends

#End Region

#Region "Events"

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        m_refContentApi = New ContentAPI

        'register page components
        Me.RegisterJS()
        Me.RegisterCSS()

    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If (Not (Request.QueryString("LangType") Is Nothing)) Then
                If (Request.QueryString("LangType") <> "") Then
                    ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                    m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
                Else

                    If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))

                End If
            Else

                If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))

            End If

            If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
                m_refContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
            Else
                m_refContentApi.ContentLanguage = ContentLanguage
            End If

            m_refMsg = m_refContentApi.EkMsgRef

            If (Request.QueryString("id") <> "") Then ContentId = Request.QueryString("id")

            If (Request.QueryString("hist_id") <> "") Then HistoryId = Request.QueryString("hist_id")

            If (Not (Page.IsPostBack)) Then

                AppImgPath = m_refContentApi.AppImgPath
                AppName = m_refContentApi.AppName
                ContentLanguage = m_refContentApi.ContentLanguage
                imagePath = m_refContentApi.AppPath & "images/ui/icons/"

                content_data = New ContentData

                If (ContentId > 0 And HistoryId > 0) Then

                    If (Request.QueryString("xslt") = "remove") Then
                        bApplyXslt = False
                    Else
                        bApplyXslt = True
                    End If

                    security_data = m_refContentApi.LoadPermissions(ContentId, "content", ContentAPI.PermissionResultType.Content)

                    m_contentType = m_refContentApi.EkContentRef.GetContentType(ContentId)

                    Select Case m_contentType

                        Case EkConstants.CMSContentType_CatalogEntry

                            Dim m_refCatalogAPI As New Commerce.CatalogEntryApi()

                            entry_data = m_refCatalogAPI.GetItem(ContentId)
                            entry_version_data = m_refCatalogAPI.GetItemVersion(ContentId, m_refContentApi.ContentLanguage, HistoryId)
                            PopulateCatalogPageData(entry_version_data, entry_data)
                            Display_EntryHistoryToolBar(entry_data)

                        Case Else

                            content_data = m_refContentApi.GetContentById(ContentId)
                            hist_content_data = m_refContentApi.GetContentByHistoryId(HistoryId)
                            Dim folder_data As FolderData
                            blog_post_data = New BlogPostData
                            blog_post_data.Categories = Array.CreateInstance(GetType(String), 0)
                            If (Not (IsNothing(content_data))) Then
                                bIsBlog = m_refContentApi.EkContentRef.GetFolderType(content_data.FolderId) = FolderType.Blog
                                If bIsBlog Then
                                    folder_data = m_refContentApi.GetFolderById(content_data.FolderId)
                                    If Not hist_content_data.MetaData Is Nothing Then
                                        For i As Integer = 0 To (hist_content_data.MetaData.Length - 1)
                                            If hist_content_data.MetaData(i).TypeId = BlogPostDataType.Categories Or hist_content_data.MetaData(i).TypeName.ToLower().IndexOf("blog categories") > -1 Then
                                                hist_content_data.MetaData(i).Text = hist_content_data.MetaData(i).Text.Replace("&#39;", "'")
                                                hist_content_data.MetaData(i).Text = hist_content_data.MetaData(i).Text.Replace("&quot", """")
                                                hist_content_data.MetaData(i).Text = hist_content_data.MetaData(i).Text.Replace("&gt;", ">")
                                                hist_content_data.MetaData(i).Text = hist_content_data.MetaData(i).Text.Replace("&lt;", "<")
                                                blog_post_data.Categories = Split(hist_content_data.MetaData(i).Text, ";")
                                            ElseIf hist_content_data.MetaData(i).TypeId = BlogPostDataType.Ping Or hist_content_data.MetaData(i).TypeName.ToLower().IndexOf("blog pingback") > -1 Then
                                                blog_post_data.Pingback = Ektron.Cms.Common.EkFunctions.GetBoolFromYesNo(hist_content_data.MetaData(i).Text)
                                            ElseIf hist_content_data.MetaData(i).TypeId = BlogPostDataType.Tags Or hist_content_data.MetaData(i).TypeName.ToLower().IndexOf("blog tags") > -1 Then
                                                blog_post_data.Tags = hist_content_data.MetaData(i).Text
                                            ElseIf hist_content_data.MetaData(i).TypeId = BlogPostDataType.Trackback Or hist_content_data.MetaData(i).TypeName.ToLower().IndexOf("blog trackback") > -1 Then
                                                blog_post_data.TrackBackURL = hist_content_data.MetaData(i).Text
                                            End If
                                        Next
                                    End If
                                    If (Not (IsNothing(folder_data.XmlConfiguration))) Then
                                        bXmlContent = True
                                    End If
                                End If
                                hist_content_data.Type = content_data.Type
                                PopulatePageData(hist_content_data, content_data)
                            End If
                            Display_ContentHistoryToolBar()

                    End Select

                ElseIf (ContentId > 0) Then

                    m_contentType = m_refContentApi.EkContentRef.GetContentType(ContentId)

                    Select Case m_contentType

                        Case EkConstants.CMSContentType_CatalogEntry

                            Dim m_refCatalogAPI As New Commerce.CatalogEntryApi()

                            entry_data = m_refCatalogAPI.GetItem(ContentId)
                            entry_version_data = m_refCatalogAPI.GetItemVersion(ContentId, m_refContentApi.ContentLanguage, HistoryId)
                            PopulateCatalogPageData(entry_version_data, entry_data)
                            Display_EntryHistoryToolBar(entry_data)

                        Case Else

                            content_data = m_refContentApi.GetContentById(ContentId)
                            PopulatePageData(hist_content_data, content_data)
                            Display_ContentHistoryToolBar()

                    End Select

                End If

            Else

                m_contentType = m_refContentApi.EkContentRef.GetContentType(ContentId)
                content_data = m_refContentApi.GetContentById(ContentId)

                Select Case m_contentType

                    Case EkConstants.CMSContentType_CatalogEntry

                        Dim m_refCatalogAPI As New Commerce.CatalogEntryApi()
                        HistoryId = Request.QueryString("hist_id")
                        m_refCatalogAPI.Restore(ContentId, HistoryId)

                    Case Else

                        HistoryId = Request.QueryString("hist_id")
                        m_refContentApi.RestoreHistoryContent(HistoryId)

                End Select

                CloseOnRestore.Text = "<script type=""text/javascript"">try { location.href = 'content.aspx?LangType=" & ContentLanguage & "&action=ViewStaged&id=" & ContentId & "&fldid=" & content_data.FolderId & "'; } catch(e) {}</script>"

            End If
        Catch ex As Exception

            ShowError(ex.Message)

        End Try
    End Sub

#End Region


#Region "Catalog entry"

    Private Sub Display_EntryHistoryToolBar(ByVal entry_data As EntryData)
        Dim result As New System.Text.StringBuilder
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view catalog entry history title") & " """ & entry_data.Title & """")

        If HistoryId <> -1 Then
            result.Append("<table><tr>")
            If (security_data.CanRestore) Then
                restore_id.Value = HistoryId
                If (entry_data.Status.ToLower() <> "o") And (entry_data.Status.ToLower() <> "s") And (entry_data.Status.ToLower() <> "p") Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(imagePath & "restore.png", "#", m_refMsg.GetMessage("alt restore button text"), m_refMsg.GetMessage("btn restore"), "  onclick=""javascript:document.forms[0].submit();return false;"" target=""history_frame"""))
                End If
                result.Append(m_refStyle.GetButtonEventsWCaption(imagePath & "contentViewDifferences.png", "#", "View Content Difference", m_refMsg.GetMessage("btn view diff"), "onclick=""javascript:PopUpWindow('compare.aspx?LangType=" & ContentLanguage & "&id=" & ContentId & "&hist_id=" & HistoryId & "', 'Compare', 800, 530, 0, 0);return false;"""))
            End If

            result.Append(m_refStyle.GetButtonEventsWCaption(imagePath & "history.png", "history.aspx?action=report&LangType=" & ContentLanguage & "&id=" & Request.QueryString("id"), m_refMsg.GetMessage("view history report"), m_refMsg.GetMessage("view history report"), ""))
            result.Append("<td>" & m_refStyle.GetButtonEventsWCaption(imagePath & "back.png", "javascript:history.go(-1);", m_refMsg.GetMessage("btn back"), m_refMsg.GetMessage("btn back"), "") & "</td>")
            result.Append("<td>" & m_refStyle.GetHelpButton("ViewContentHistory") & "</td>")
            result.Append("</tr></table>")

            divToolBar.InnerHtml = result.ToString
        Else
            divToolBar.Style.Add("height", "0")
        End If

        result = Nothing
    End Sub

    Private Sub PopulateCatalogPageData(ByVal entry_data_version As EntryData, ByVal entry_data As EntryData)
        Dim bPackageDisplayXSLT As Boolean = False
        Dim CurrentXslt As String = ""

        Display_PropertiesTab(entry_data_version)
        Display_PricingTab(entry_data_version)
        Display_MetadataTab(entry_data_version)

        phItemsTab.Visible = False
        phItems.Visible = False
        'sp_item.Visible = False

        If (Not (IsNothing(entry_data_version.ProductType))) And (bApplyXslt) Then
            If Len(entry_data_version.ProductType.PackageDisplayXslt) Then
                bPackageDisplayXSLT = True
            Else
                If (entry_data_version.ProductType.DefaultXslt.Length > 0) Then
                    bPackageDisplayXSLT = False
                    If (Len(entry_data_version.ProductType.PhysPathComplete("Xslt" & entry_data_version.ProductType.DefaultXslt))) Then
                        CurrentXslt = entry_data_version.ProductType.PhysPathComplete("Xslt" & entry_data_version.ProductType.DefaultXslt)
                    Else
                        CurrentXslt = entry_data_version.ProductType.LogicalPathComplete("Xslt" & entry_data_version.ProductType.DefaultXslt)
                    End If
                Else
                    bPackageDisplayXSLT = True
                End If
            End If

            If (bPackageDisplayXSLT) Then
                divContentHtml.InnerHtml = m_refContentApi.TransformXsltPackage(entry_data_version.Html, entry_data_version.ProductType.PackageDisplayXslt, False)
            Else
                divContentHtml.InnerHtml = m_refContentApi.TransformXSLT(entry_data_version.Html, CurrentXslt)
            End If
        Else
            divContentHtml.InnerHtml = entry_data_version.Html
        End If

        tdsummaryhead.InnerHtml = m_refMsg.GetMessage("content summary label")

        tdsummarytext.InnerHtml &= entry_data_version.Summary

        tdcommenthead.InnerHtml = m_refMsg.GetMessage("content HC label")
        tdcommenttext.InnerHtml = entry_data_version.Comment
    End Sub

    Private Sub Display_PropertiesTab(ByVal entry_data_version As EntryData)

        Dim colBound As New System.Web.UI.WebControls.BoundColumn

        colBound.DataField = "NAME"
        colBound.HeaderText = ""
        colBound.ItemStyle.CssClass = "label"
        PropertiesGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = ""
        PropertiesGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("NAME", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))

        Dim i As Integer = 0

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content title label")
        dr(1) = entry_data_version.Title
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content id label")
        dr(1) = entry_data_version.Id
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content language label")
        dr(1) = entry_data_version.LanguageId
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("lbl calatog entry sku") & "&nbsp;#:"
        dr(1) = entry_data.Sku
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content status label")
        Select Case entry_data_version.Status.ToLower
            Case "a"
                dr(1) = m_refMsg.GetMessage("status:Approved (Published)")
            Case "o"
                dr(1) = m_refMsg.GetMessage("status:Checked Out")
            Case "i"
                dr(1) = m_refMsg.GetMessage("status:Checked In")
            Case "p"
                dr(1) = m_refMsg.GetMessage("status:Approved (PGLD)")
            Case "m"
                dr(1) = "<font color=""Red"">" & m_refMsg.GetMessage("status:Submitted for Deletion") & "</font>"
            Case "s"
                dr(1) = "<font color=""Red"">" & m_refMsg.GetMessage("status:Submitted for Approval") & "</font>"
            Case "t"
                dr(1) = m_refMsg.GetMessage("status:Waiting Approval")
            Case "d"
                dr(1) = "Deleted (Pending Start Date)"
        End Select
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content LUE label")
        dr(1) = entry_data_version.LastEditorFirstName & " " & entry_data_version.LastEditorLastName 'DisplayUserName
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content LED label")
        dr(1) = entry_data_version.DateModified
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic start date label")
        If (entry_data_version.GoLive = DateTime.MinValue OrElse entry_data_version.GoLive = DateTime.MaxValue) Then
            dr(1) = m_refMsg.GetMessage("none specified msg")
        Else
            dr(1) = entry_data_version.GoLive
        End If
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic end date label")
        If (entry_data_version.EndDate = DateTime.MinValue OrElse entry_data_version.EndDate = DateTime.MaxValue) Then
            dr(1) = m_refMsg.GetMessage("none specified msg")
        Else
            dr(1) = entry_data_version.EndDate
        End If
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("End Date Action Title")

        If (entry_data_version.EndDate = DateTime.MinValue OrElse entry_data_version.EndDate = DateTime.MaxValue) Then
            If (entry_data_version.EndDateAction = EndDateActionType_archive_display) Then
                dr(1) = m_refMsg.GetMessage("Archive display descrp")
            ElseIf (entry_data_version.EndDateAction = EndDateActionType_refresh) Then
                dr(1) = m_refMsg.GetMessage("Refresh descrp")
            Else
                dr(1) = m_refMsg.GetMessage("Archive expire descrp")
            End If
        Else
            dr(1) = m_refMsg.GetMessage("none specified msg")
        End If

        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content DC label")
        dr(1) = entry_data_version.DateModified  'DisplayDateCreated
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content approvals label")
        Dim approvallist As New System.Text.StringBuilder
        Dim approvaldata() As ApprovalData
        approvaldata = m_refContentApi.GetCurrentApprovalInfoByID(ContentId)
        approvallist.Append(m_refMsg.GetMessage("none specified msg"))
        If (Not (IsNothing(approvaldata))) Then
            If (approvaldata.Length > 0) Then
                approvallist.Length = 0
                For i = 0 To approvaldata.Length - 1
                    If (approvaldata(i).Type = "user") Then
                        approvallist.Append("<img src=""" & imagePath & "user.png"" align=""absbottom"" alt=""" & m_refMsg.GetMessage("approver is user") & """ title=""" & m_refMsg.GetMessage("approver is user") & """>")
                    Else
                        approvallist.Append("<img src=""" & imagePath & "user.png"" align=""absbottom"" alt=""" & m_refMsg.GetMessage("approver is user group") & """ title=""" & m_refMsg.GetMessage("approver is user group") & """>")
                    End If
                    If (approvaldata(i).IsCurrentApprover) Then
                        approvallist.Append("<span class=""important"">")
                    Else
                        approvallist.Append("<span>")
                    End If
                    approvallist.Append(approvaldata(i).DisplayUserName & "</span>")
                Next
            End If
        End If
        dr(1) = approvallist.ToString
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("xml configuration label")
        dr(1) = "&nbsp;" & entry_data_version.ProductType.Title
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic template label")
        dr(1) = ""
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic Path")
        dr(1) = m_refContentApi.EkContentRef.GetFolderPath(entry_data_version.FolderId)

        Dim dv As New DataView(dt)
        PropertiesGrid.DataSource = dv
        PropertiesGrid.DataBind()
    End Sub

    Private Sub Display_MetadataTab(ByVal versionData As EntryData)
        Dim sbAttrib As New StringBuilder
        Dim sbResult As New StringBuilder
        Dim strResult As String
        Dim strAttrResult As String
        Dim strImage As String = ""
        Dim prod_type_API As New ProductTypeApi()
        Dim prod_type_data As ProductTypeData = Nothing

        EnhancedMetadataScript.Text = Replace(CustomFields.GetEnhancedMetadataScript(), "src=""java/", "src=""../java/")
        EnhancedMetadataArea.Text = CustomFields.GetEnhancedMetadataArea()

        If prod_type_data Is Nothing Then prod_type_data = prod_type_API.GetItem(versionData.ProductType.Id)

        If versionData.Metadata.Count > 0 OrElse prod_type_data.Attributes.Count > 0 Then

            m_refSite = New Site.EkSite(Me.m_refContentApi.RequestInformationRef)
            Dim hPerm As Hashtable = m_refSite.GetPermissions(m_iFolder, 0, "folder")
            sbResult.Append(Ektron.Cms.CustomFields.WriteFilteredMetadataForView(versionData.Metadata.ToArray(), versionData.FolderId, False).Trim)
            If prod_type_data IsNot Nothing Then sbAttrib.Append(CustomFields.WriteFilteredAttributesForView(versionData.Attributes, prod_type_data.Id, False))

        End If

        If (m_sEditAction = "update") Then
            strImage = versionData.Image
            Dim strThumbnailPath As String = versionData.ImageThumbnail
            If (versionData.ImageThumbnail = "") Then
                strThumbnailPath = m_refContentApi.AppImgPath & "spacer.gif"
            ElseIf (catalog_data.IsDomainFolder.ToString = True) Then
                strThumbnailPath = strThumbnailPath
            Else
                strThumbnailPath = m_refContentApi.SitePath & strThumbnailPath
            End If
            sbResult.Append("<fieldset><legend>Image Data:</legend><table><tr><td class=""label"" align=""left"">Image:</td><td><span id=""sitepath""" & Me.m_refContentApi.SitePath & "</span><input type=""textbox"" size=""30"" readonly=""true"" id=""content_image"" name=""content_image"" value=""" & strImage & """ /></td></tr><tr><td colomnspan=""2""><img id=""content_image_thumb"" src=""" & strThumbnailPath & """ /></td></tr></table></fieldset>")
        Else
            sbResult.Append("<fieldset><legend>Image Data:</legend><table><tr><td class=""label"" align=""left"">Image:</td><td><span id=""sitepath""" & Me.m_refContentApi.SitePath & "</span><input type=""textbox"" size=""30"" readonly=""true"" id=""content_image"" name=""content_image"" value=""" & strImage & """ /></td></tr><tr><td colomnspan=""2""><img id=""content_image_thumb"" src=""" & m_refContentApi.AppImgPath & "spacer.gif"" /></td></tr></table></fieldset>")
        End If
        strAttrResult = sbAttrib.ToString().Trim()
        strResult = sbResult.ToString.Trim()
        strResult = Util_FixPath(strResult)

        MetaDataValue.Text = strResult
        ltr_attrib.Text = strAttrResult
    End Sub

    Private Function Util_FixPath(ByVal MetaScript As String) As String
        Dim iTmp As Integer = -1
        iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", 0)
        While iTmp > -1
            iTmp = MetaScript.IndexOf(");return (false);", iTmp)
            MetaScript = MetaScript.Insert(iTmp, ", '" & Me.m_refContentApi.ApplicationPath & "'")
            iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", iTmp + 1)
        End While
        Return MetaScript
    End Function

    Private Sub Display_PricingTab(ByVal versionData As EntryData)

        Dim m_refCurrency As New Currency(m_refContentApi.RequestInformationRef)
        Dim workarearef As New Ektron.Cms.Workarea.workareabase()
        Dim activeCurrencyList As List(Of CurrencyData) = m_refCurrency.GetActiveCurrencyList()
        Dim exchangeRateList As New List(Of ExchangeRateData)
        If activeCurrencyList.Count > 1 Then
            Dim exchangeRateApi As New ExchangeRateApi()
            Dim exchangeRateCriteria As New Criteria(Of ExchangeRateProperty)
            Dim currencyIDList As New List(Of Long)
            For i As Integer = 0 To (activeCurrencyList.Count - 1)
                currencyIDList.Add(activeCurrencyList(i).Id)
            Next
            exchangeRateCriteria.AddFilter(ExchangeRateProperty.BaseCurrencyId, CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId)
            exchangeRateCriteria.AddFilter(ExchangeRateProperty.ExchangeCurrencyId, CriteriaFilterOperator.In, currencyIDList.ToArray())
            exchangeRateList = exchangeRateApi.GetCurrentList(exchangeRateCriteria)
        End If
        ltr_pricing.Text = workarearef.CommerceLibrary.GetPricingMarkup(versionData.Pricing, activeCurrencyList, exchangeRateList, entry_data.EntryType, False, workareaCommerce.ModeType.View)

    End Sub

#End Region


#Region "Other"

    Private Sub Display_ContentHistoryToolBar()
        Dim result As New System.Text.StringBuilder
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view content history title") & " """ & content_data.Title & """")

        If HistoryId <> -1 Then
            result.Append("<table><tr>")
            If (security_data.CanRestore) Then
                restore_id.Value = HistoryId
                If (content_data.Status.ToLower() <> "s") And (content_data.Status.ToLower() <> "p") Then
                    If (content_data.SubType = CMSContentSubtype.WebEvent) Then
                        Dim action As String = "if(confirm('This will only restore description changes. Are you sure?')){javascript:document.forms[0].submit();}return false;"
                        result.Append(m_refStyle.GetButtonEventsWCaption(imagePath & "restore.png", "#", m_refMsg.GetMessage("alt restore button text"), m_refMsg.GetMessage("btn restore"), " onclick=""" & action & """ target=""history_frame"""))
                    Else
                        result.Append(m_refStyle.GetButtonEventsWCaption(imagePath & "restore.png", "#", m_refMsg.GetMessage("alt restore button text"), m_refMsg.GetMessage("btn restore"), " onclick=""javascript:document.forms[0].submit();return false;"" target=""history_frame"""))
                    End If
                End If
            End If

            If (content_data.SubType <> CMSContentSubtype.PageBuilderData AndAlso content_data.SubType <> CMSContentSubtype.PageBuilderMasterData) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(imagePath & "contentViewDifferences.png", "#", "View Content Difference", m_refMsg.GetMessage("btn view diff"), "onclick=""javascript:PopUpWindow('compare.aspx?LangType=" & ContentLanguage & "&id=" & ContentId & "&hist_id=" & HistoryId & "', 'Compare', 800, 530, scrollbar='yes', 0, 0);return false;"""))
            End If

            If (bXmlContent And content_data.Type = 1 And (Not IsNothing(content_data.XmlConfiguration))) Then
                If (bApplyXslt) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_noviewxslt-nm.gif", "history.aspx?LangType=" & ContentLanguage & "&xslt=remove&Id=" & ContentId & "&hist_id=" & HistoryId & "", "Remove applied XSLT", m_refMsg.GetMessage("btn view no xslt"), "target=""history_frame"""))
                Else
                    result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_viewxslt-nm.gif", "history.aspx?LangType=" & ContentLanguage & "&xslt=apply&Id=" & ContentId & "&hist_id=" & HistoryId & "", "Apply default XSLT", m_refMsg.GetMessage("btn view xslt"), "target=""history_frame"""))
                End If
            End If

            result.Append("<td>" & m_refStyle.GetButtonEventsWCaption(imagePath & "back.png", "javascript:history.go(-1);", m_refMsg.GetMessage("btn back"), m_refMsg.GetMessage("btn back"), "") & "</td>")
            result.Append("<td>" & m_refStyle.GetHelpButton("ViewContentHistory") & "</td>")
            result.Append("</tr></table>")
            divToolBar.InnerHtml = result.ToString
        Else
            divToolBar.Style.Add("height", "0")
        End If

        result = Nothing
    End Sub

    ''' <summary>
    ''' Fills the history tabs with content body, summary, meta, and properties
    ''' </summary>
    ''' <param name="content_data_history"></param>
    ''' <param name="content_data">This parameter is needed to get the XML XSLT INFO </param>
    ''' <remarks></remarks>
    Private Sub PopulatePageData(ByVal content_data_history As ContentData, ByVal content_data As ContentData)
        Dim bPackageDisplayXSLT As Boolean = False
        Dim CurrentXslt As String = ""

        phPricingTab.Visible = False
        phPricing.Visible = False
        phAttributesTab.Visible = False
        phAttributes.Visible = False
        phItemsTab.Visible = False
        phItems.Visible = False

        ViewContentProperties(content_data_history)
        ViewMetaData(content_data_history)
        If ((Not (IsNothing(content_data.XmlConfiguration))) And (content_data.Type = 1 Or content_data.Type = 2)) Then
            If (Not (IsNothing(content_data.XmlConfiguration))) And (bApplyXslt) Then
                If Len(content_data.XmlConfiguration.PackageDisplayXslt) Then
                    bPackageDisplayXSLT = True
                Else
                    If (content_data.XmlConfiguration.DefaultXslt.Length > 0) Then
                        bPackageDisplayXSLT = False
                        If (Len(content_data.XmlConfiguration.PhysPathComplete("Xslt" & content_data.XmlConfiguration.DefaultXslt))) Then
                            CurrentXslt = content_data.XmlConfiguration.PhysPathComplete("Xslt" & content_data.XmlConfiguration.DefaultXslt)
                        Else
                            CurrentXslt = content_data.XmlConfiguration.LogicalPathComplete("Xslt" & content_data.XmlConfiguration.DefaultXslt)
                        End If
                    Else
                        bPackageDisplayXSLT = True
                    End If
                End If

                If (bPackageDisplayXSLT) Then
                    divContentHtml.InnerHtml = m_refContentApi.TransformXsltPackage(content_data_history.Html, content_data.XmlConfiguration.PackageDisplayXslt, False)
                Else
                    divContentHtml.InnerHtml = m_refContentApi.TransformXSLT(content_data_history.Html, CurrentXslt)
                End If
            Else
                divContentHtml.InnerHtml = content_data_history.Html
            End If
        Else
            If content_data_history.Type = 104 Then
                divContentHtml.InnerHtml = FixContentHistory(content_data_history, content_data_history.Html)
            Else
                If (content_data_history.SubType = CMSContentSubtype.PageBuilderData Or content_data_history.SubType = CMSContentSubtype.PageBuilderMasterData) Then
                    divContentHtml.InnerHtml = Ektron.Cms.PageBuilder.PageData.RendertoString(content_data_history.Html)
                Else
                    divContentHtml.InnerHtml = content_data_history.Html
                End If
            End If
        End If

        tdsummaryhead.InnerHtml = m_refMsg.GetMessage("content summary label")
        If CMSContentType.Forms = content_data_history.Type Or CMSContentType.Archive_Forms = content_data_history.Type Then
            If Not IsNothing(content_data_history.Teaser) Then
                If content_data_history.Teaser.IndexOf("<ektdesignpackage_design") > -1 Then
                    Dim strDesign As String
                    strDesign = m_refContentApi.XSLTransform(content_data_history.Teaser, _
                      Server.MapPath(m_refContentApi.AppeWebPath() & "unpackageDesign.xslt"), XsltAsFile:=True, _
                      ReturnExceptionMessage:=True)
                    tdsummarytext.InnerHtml = strDesign
                Else
                    tdsummarytext.InnerHtml &= content_data_history.Teaser
                End If
            Else
                tdsummarytext.InnerHtml = ""
            End If
        Else
            If bIsBlog Then
                tdsummarytext.InnerHtml &= ("<table border=""0"" cellpadding=""4"" width=""550"">")
                tdsummarytext.InnerHtml &= ("	<tr>")
                tdsummarytext.InnerHtml &= ("		<td width=""20"">&nbsp;</td>")
                tdsummarytext.InnerHtml &= ("		<td valign=""top"" width=""80%"">")
                tdsummarytext.InnerHtml &= ("			<b>Description</b>")
                tdsummarytext.InnerHtml &= ("		</td>")
                tdsummarytext.InnerHtml &= ("		<td width=""20"">&nbsp;</td>")
                tdsummarytext.InnerHtml &= ("		<td valign=""top"" width=""20%"">")
                tdsummarytext.InnerHtml &= ("			<b>Categories</b>")
                tdsummarytext.InnerHtml &= ("		</td>")
                tdsummarytext.InnerHtml &= ("	</tr>")
                tdsummarytext.InnerHtml &= ("	<tr>")
                tdsummarytext.InnerHtml &= ("		<td width=""20"">&nbsp;</td>")
                tdsummarytext.InnerHtml &= ("		<td valign=""top"">")
            End If
            tdsummarytext.InnerHtml &= content_data_history.Teaser
            If bIsBlog Then
                tdsummarytext.InnerHtml &= ("			<br/><br/>")
                tdsummarytext.InnerHtml &= ("			<b>Tags</b>")
                tdsummarytext.InnerHtml &= ("			<br/>")
                If Not (blog_post_data Is Nothing) Then
                    tdsummarytext.InnerHtml &= (blog_post_data.Tags)
                End If
                tdsummarytext.InnerHtml &= ("		</td>")
                tdsummarytext.InnerHtml &= ("		<td width=""20"">&nbsp;</td>")
                tdsummarytext.InnerHtml &= ("		<td valign=""top"" style=""border: 1px solid #fffff; ""  width=""20%"">")
                tdsummarytext.InnerHtml &= ("	<p>")

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
                            tdsummarytext.InnerHtml &= ("				<input type=""checkbox"" name=""blogcategories" & i.ToString() & """ value=""" & blog_post_data.Categories(i).ToString() & """ checked=""true"" disabled>&nbsp;" & Replace(blog_post_data.Categories(i).ToString(), "~@~@~", ";") & "<br>")
                        End If
                    Next
                Else
                    tdsummarytext.InnerHtml &= ("No categories defined.")
                End If
                tdsummarytext.InnerHtml &= ("				<br/>")
                tdsummarytext.InnerHtml &= ("			</p>")
                tdsummarytext.InnerHtml &= ("		</td>")
                tdsummarytext.InnerHtml &= ("	</tr>")
                tdsummarytext.InnerHtml &= ("	<tr>")
                tdsummarytext.InnerHtml &= ("		<td width=""20"">&nbsp;</td>")
                tdsummarytext.InnerHtml &= ("	    <td colspan=""3"">")
                If Not (blog_post_data Is Nothing) Then
                    tdsummarytext.InnerHtml &= "<br/><input type=""hidden"" name=""blogposttrackbackid"" id=""blogposttrackbackid"" value=""" & blog_post_data.TrackBackURLID.ToString() & """ /><input type=""hidden"" id=""isblogpost"" name=""isblogpost"" value=""true""/><br/><b>TrackBack URL</b><br/>"
                    tdsummarytext.InnerHtml &= "<input type=""text"" size=""75"" id=""trackback"" name=""trackback"" value=""" & Server.HtmlEncode(blog_post_data.TrackBackURL) & """ disabled/>"
                    tdsummarytext.InnerHtml &= "<br/><br/>"
                    If blog_post_data.Pingback = True Then
                        tdsummarytext.InnerHtml &= "<input type=""checkbox"" name=""pingback"" id=""pingback"" checked disabled/>&nbsp;PingBack URLs in this post"
                    Else
                        tdsummarytext.InnerHtml &= "<input type=""checkbox"" name=""pingback"" id=""pingback"" disabled/>&nbsp;PingBack URLs in this post"
                    End If
                Else
                    tdsummarytext.InnerHtml &= "<br/><input type=""hidden"" name=""blogposttrackbackid"" id=""blogposttrackbackid"" value="""" /><input type=""hidden"" id=""isblogpost"" name=""isblogpost"" value=""true""/><br/><b>TrackBack URL</b><br/>"
                    tdsummarytext.InnerHtml &= "<input type=""text"" size=""75"" id=""trackback"" name=""trackback"" value="""" disabled/>"
                    tdsummarytext.InnerHtml &= "<br/><br/>"
                    tdsummarytext.InnerHtml &= "<input type=""checkbox"" name=""pingback"" id=""pingback"" disabled/>&nbsp;PingBack URLs in this post"
                End If
                tdsummarytext.InnerHtml &= ("		</td>")
                tdsummarytext.InnerHtml &= ("	</tr>")
                tdsummarytext.InnerHtml &= ("</table>")
            End If
        End If

        tdcommenthead.InnerHtml = m_refMsg.GetMessage("content HC label")
        tdcommenttext.InnerHtml = content_data_history.Comment
    End Sub

    Private Sub ViewMetaData(ByVal data As ContentData)
        ' Note: History for metadata-to-folder-assignment is not stored, and the
        ' metadata array supplied to this function does not include anything useful
        ' except for the TypeName property (string). We must compare this name to
        ' all the names of the metadata currently assigned to the content-items
        ' folder, showing only the matches and filtering out all of the rest:
        Dim result As New System.Text.StringBuilder
        Dim idx As Integer
        Dim htValidMetadata As Hashtable
        Dim cCustFieldCol As Collection
        Dim cCustFieldItem As Collection
        Dim custFlds As CustomFieldsApi
        Dim sName As String

        ' folder
        Try
            ' build a table of valid names:
            custFlds = New CustomFieldsApi
            cCustFieldCol = custFlds.GetFieldsByFolder(data.FolderId(), data.LanguageId())
            htValidMetadata = New Hashtable
            For Each cCustFieldItem In cCustFieldCol
                htValidMetadata.Add(cCustFieldItem("CustomFieldName"), True)
            Next

            ' now display only those items that belong to the containing folder:
            result.Append("<table class=""ektronForm""><tbody>")

            If Not (data.MetaData Is Nothing) Then
                For idx = 0 To data.MetaData.Length - 1
                    sName = data.MetaData(idx).TypeName
                    If htValidMetadata.ContainsKey(sName) Then
                        result.Append("<tr><td class=""label"">" & sName & ":</td><td>&nbsp;&nbsp;")
                        result.Append(data.MetaData(idx).Text & "</td></tr>")
                    End If
                Next
            Else
                result.Append("<tr><td>There is no metadata defined.</td><tr>")
            End If
            result.Append("</tbody></table>")
            MetaDataValue.Text = result.ToString()

        Catch ex As Exception
            MetaDataValue.Text = ex.Message

        Finally
            result = Nothing
            custFlds = Nothing
            htValidMetadata = Nothing
            cCustFieldCol = Nothing
            cCustFieldItem = Nothing
        End Try
    End Sub

    Private Sub ViewContentProperties(ByVal data As ContentData)
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "NAME"
        colBound.ItemStyle.CssClass = "label"
        PropertiesGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        PropertiesGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("NAME", GetType(String)))
        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))

        Dim i As Integer = 0

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content title label")
        dr(1) = data.Title
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content id label")
        dr(1) = data.Id
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content language label")
        dr(1) = data.LanguageId
        dt.Rows.Add(dr)

        If content_data.Type = 3333 Then
            dr = dt.NewRow()
            dr(0) = m_refMsg.GetMessage("lbl calatog entry sku") & "&nbsp;#:"
            dr(1) = entry_data.Sku
            dt.Rows.Add(dr)
        End If

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content status label")
        Select Case data.Status.ToLower
            Case "a"
                dr(1) = m_refMsg.GetMessage("status:Approved (Published)")
            Case "o"
                dr(1) = m_refMsg.GetMessage("status:Checked Out")
            Case "i"
                dr(1) = m_refMsg.GetMessage("status:Checked In")
            Case "p"
                dr(1) = m_refMsg.GetMessage("status:Approved (PGLD)")
            Case "m"
                dr(1) = "<font color=""Red"">" & m_refMsg.GetMessage("status:Submitted for Deletion") & "</font>"
            Case "s"
                dr(1) = "<font color=""Red"">" & m_refMsg.GetMessage("status:Submitted for Approval") & "</font>"
            Case "t"
                dr(1) = m_refMsg.GetMessage("status:Waiting Approval")
            Case "d"
                dr(1) = "Deleted (Pending Start Date)"
        End Select
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content LUE label")
        dr(1) = data.EditorFirstName & " " & data.EditorLastName  'DisplayUserName
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content LED label")
        dr(1) = data.DisplayLastEditDate
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic start date label")
        If (data.DisplayGoLive.Length = 0) Then
            dr(1) = m_refMsg.GetMessage("none specified msg")
        Else
            dr(1) = data.DisplayGoLive
        End If
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic end date label")
        If (data.DisplayEndDate = "") Then
            dr(1) = m_refMsg.GetMessage("none specified msg")
        Else
            dr(1) = data.DisplayEndDate
        End If
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("End Date Action Title")
        If (data.DisplayEndDate.Length > 0) Then
            If (data.EndDateAction = EndDateActionType_archive_display) Then
                dr(1) = m_refMsg.GetMessage("Archive display descrp")
            ElseIf (data.EndDateAction = EndDateActionType_refresh) Then
                dr(1) = m_refMsg.GetMessage("Refresh descrp")
            Else
                dr(1) = m_refMsg.GetMessage("Archive expire descrp")
            End If
        Else
            dr(1) = m_refMsg.GetMessage("none specified msg")
        End If
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content DC label")
        dr(1) = data.DateCreated 'DisplayDateCreated
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = Me.m_refMsg.GetMessage("lbl approval method")
        If (data.ApprovalMethod = 1) Then
            dr(1) = m_refMsg.GetMessage("display for force all approvers")
        Else
            dr(1) = m_refMsg.GetMessage("display for do not force all approvers")
        End If
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("content approvals label")
        Dim approvallist As New System.Text.StringBuilder
        Dim approvaldata() As ApprovalData
        approvaldata = m_refContentApi.GetCurrentApprovalInfoByID(ContentId)
        approvallist.Append(m_refMsg.GetMessage("none specified msg"))
        If (Not (IsNothing(approvaldata))) Then
            If (approvaldata.Length > 0) Then
                approvallist.Length = 0
                For i = 0 To approvaldata.Length - 1
                    If (approvaldata(i).Type = "user") Then
                        approvallist.Append("<img src=""" & imagePath & "user.png"" align=""absbottom"" alt=""" & m_refMsg.GetMessage("approver is user") & """ title=""" & m_refMsg.GetMessage("approver is user") & """>")
                    Else
                        approvallist.Append("<img src=""" & imagePath & "user.png"" align=""absbottom"" alt=""" & m_refMsg.GetMessage("approver is user group") & """ title=""" & m_refMsg.GetMessage("approver is user group") & """>")
                    End If
                    If (approvaldata(i).IsCurrentApprover) Then
                        approvallist.Append("<span class=""important"">")
                    Else
                        approvallist.Append("<span>")
                    End If
                    approvallist.Append(approvaldata(i).DisplayUserName & "</span>")
                Next
            End If
        End If
        dr(1) = approvallist.ToString
        dt.Rows.Add(dr)
        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("xml configuration label")
        If (data.IsXmlInherited <> False) Then
            If (Not (IsNothing(data.XmlConfiguration))) Then
                dr(1) = "&nbsp;" & data.XmlConfiguration.Title
            Else
                dr(1) = m_refMsg.GetMessage("none specified msg") & " " & m_refMsg.GetMessage("html content assumed")
            End If
        End If
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic template label")
        dr(1) = ""
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = m_refMsg.GetMessage("generic Path")
        dr(1) = m_refContentApi.EkContentRef.GetFolderPath(data.FolderId)
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr(0) = "Content Searchable:"
        dr(1) = data.IsSearchable

        Dim dv As New DataView(dt)
        PropertiesGrid.ShowHeader = False
        PropertiesGrid.DataSource = dv
        PropertiesGrid.DataBind()
    End Sub

#End Region

#Region "Helpers"

    Protected Sub PropertiesGrid_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        Select Case e.Item.ItemType
            Case ListItemType.AlternatingItem, ListItemType.Item
                If (e.Item.Cells(0).Text.Equals(m_refMsg.GetMessage("properties text")) And e.Item.Cells(1).Text.Equals("REMOVEDITEM")) Then
                    e.Item.Cells(0).Attributes.Add("align", "Left")
                    e.Item.Cells(0).ColumnSpan = 2
                    e.Item.Cells(0).CssClass = "title-header"
                    e.Item.Cells.RemoveAt(1)
                End If
        End Select
    End Sub
    Private Sub ShowError(ByVal ex As String)

        Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex), False)

    End Sub

    Public Function FixContentHistory(ByVal myContentData As ContentData, ByVal curSnippet As String) As String
        Dim regExp As Regex
        regExp = New Regex(" ")
        For Each strLineVal As String In regExp.Split(curSnippet)
            regExp = New Regex("=")
            Dim strKeyValues As String()
            strKeyValues = regExp.Split(strLineVal)
            Select Case strKeyValues(0).Trim()
                Case "src"
                    Dim curStringVal As String = strKeyValues(1)
                    If curStringVal.ToLower().IndexOf("/assets") >= 0 Then
                        curStringVal = curStringVal.ToLower().Replace("/assets/" & myContentData.AssetData.Id.ToLower() & myContentData.AssetData.Version.Substring(myContentData.AssetData.Version.IndexOf(".")).ToLower(), "/assetmanagement/DownloadAsset.aspx?history=true&ID=" & myContentData.AssetData.Id & "&version=" & myContentData.AssetData.Version)
                        curSnippet = Regex.Replace(curSnippet, strKeyValues(1), curStringVal)
                    End If
            End Select
        Next
        Return curSnippet
    End Function

#End Region

#Region "CSS, JS"

    Private Sub RegisterJS()

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "java/searchfuncsupport.js", "EktronSearchFuncSupportJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "java/dhtml/tableutil.js", "EktronTableUtilJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "java/metadata_selectlist.js", "EktronMetadataSelectListJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "java/commerce/com.ektron.commerce.mediatab.js", "EktronCommerceMediaTabJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "java/commerce/com.Ektron.Commerce.Pricing.js", "EktronCommercePricingJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronDnRJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS)

    End Sub

    Private Sub RegisterCSS()

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "csslib/tables/tableutil.css", "EktronTableUtilCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "csslib/commerce/MediaTab.css", "EktronMediaTabCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath & "csslib/commerce/Ektron.Commerce.Pricing.css", "EktronCommercePricingCSS")


    End Sub

#End Region

End Class
