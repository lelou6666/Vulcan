Imports System.Data
Imports System.DateTime
Imports System.Collections.Generic

Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Core.CustomProperty
Imports Ektron.Cms.Framework.Core.CustomProperty

Partial Class addtaxonomy
    Inherits System.Web.UI.UserControl

    Protected m_refApi As New ContentAPI
    Protected m_refstyle As New StyleHelper
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected m_refMsg As EkMessageHelper
    Protected m_strPageAction As String = ""
    Protected m_refContent As Content.EkContent
    Protected TaxonomyLanguage As Integer = -1
    Protected TaxonomyId As Long = 0
    Protected TaxonomyParentId As Long = 0
    Protected language_data As LanguageData
    Protected TitleLabel As String = "taxonomytitle"
    Protected DescriptionLabel As String = "taxonomydescription"
    Protected m_strCurrentBreadcrumb As String = ""
    Protected m_bSynchronized As Boolean = True
    Protected _customPropertyDataList As List(Of CustomPropertyData)
    Protected _customProperty As New CustomProperty
    Protected _customPropertyData As New CustomPropertyData
    Protected _customPropertyObjectData As New CustomPropertyObjectData
    Protected _coreCustomProperty As New CustomPropertyObjectBL

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            m_refMsg = m_refApi.EkMsgRef
            AppImgPath = m_refApi.AppImgPath
            AppPath = m_refApi.AppPath
            m_strPageAction = Request.QueryString("action")
            Utilities.SetLanguage(m_refApi)
            TaxonomyLanguage = m_refApi.ContentLanguage
            If (TaxonomyLanguage = -1) Then
                TaxonomyLanguage = m_refApi.DefaultContentLanguage
            End If
            If (Request.QueryString("taxonomyid") IsNot Nothing) Then
                TaxonomyId = Convert.ToInt64(Request.QueryString("taxonomyid"))
            End If
            If (Request.QueryString("parentid") IsNot Nothing) Then
                TaxonomyParentId = Convert.ToInt64(Request.QueryString("parentid"))
                If (TaxonomyParentId > 0) Then
                    TitleLabel = "categorytitle"
                    DescriptionLabel = "categorydescription"
                End If
            End If
           

            If (Page.IsPostBack) Then
                Dim taxonomy_data As New TaxonomyData
                taxonomy_data.TaxonomyDescription = Request.Form(taxonomydescription.UniqueID)
                taxonomy_data.TaxonomyName = Request.Form(taxonomytitle.UniqueID)
                taxonomy_data.TaxonomyLanguage = TaxonomyLanguage
                taxonomy_data.TaxonomyParentId = TaxonomyParentId
                taxonomy_data.TaxonomyImage = Request.Form(taxonomy_image.UniqueID)
                taxonomy_data.CategoryUrl = Request.Form(categoryLink.UniqueID)
                If tr_enableDisable.Visible = True Then
                    If Not String.IsNullOrEmpty(Request.Form(chkEnableDisable.UniqueID)) Then
                        taxonomy_data.Visible = True
                    Else
                        taxonomy_data.Visible = False
                    End If
                Else
                    taxonomy_data.Visible = True
                End If
               
                If (Request.Form(inherittemplate.UniqueID) IsNot Nothing) Then
                    taxonomy_data.TemplateInherited = True
                End If
                If (Request.Form(taxonomytemplate.UniqueID) IsNot Nothing) Then
                    taxonomy_data.TemplateId = Convert.ToInt64(Request.Form(taxonomytemplate.UniqueID))
                Else
                    taxonomy_data.TemplateId = 0
                End If

                'If (TaxonomyId <> 0) Then
                '  taxonomy_data.TaxonomyId = TaxonomyId
                'End If
                m_refContent = m_refApi.EkContentRef
                TaxonomyId = m_refContent.CreateTaxonomy(taxonomy_data)
                If Request.Form(alllanguages.UniqueID) = "false" Then
                    m_refContent.UpdateTaxonomyVisible(TaxonomyId, -1, False)
                End If
                If (TaxonomyParentId = 0) Then
                    Dim strConfig As String = String.Empty
                    If (Not String.IsNullOrEmpty(Request.Form(chkConfigContent.UniqueID))) Then
                        strConfig = "0"
                    End If
                    If (Not String.IsNullOrEmpty(Request.Form(chkConfigUser.UniqueID))) Then
                        If (String.IsNullOrEmpty(strConfig)) Then
                            strConfig = "1"
                        Else
                            strConfig = strConfig & ",1"
                        End If
                    End If
                    If (Not String.IsNullOrEmpty(Request.Form(chkConfigGroup.UniqueID))) Then
                        If (String.IsNullOrEmpty(strConfig)) Then
                            strConfig = "2"
                        Else
                            strConfig = strConfig & ",2"
                        End If
                    End If
                    If (Not (String.IsNullOrEmpty(strConfig))) Then
                        m_refContent.UpdateTaxonomyConfig(TaxonomyId, strConfig)
                    End If
                End If
                '++++++++++++++++++++++++++++++++++++++++++++++++
                '+++++++++ Adding MetaData Information '+++++++++
                '++++++++++++++++++++++++++++++++++++++++++++++++
                AddCustomProperties()

                If (Request.QueryString("iframe") = "true") Then
                    Response.Write("<script type=""text/javascript"">parent.CloseChildPage();</script>")
                Else
                    'this should jump back to taxonomy that was added
                    'Response.Redirect("taxonomy.aspx?rf=1", True)
                    Response.Redirect("taxonomy.aspx?action=view&view=item&taxonomyid=" & TaxonomyId & "&rf=1", True)
                End If
            Else
                m_refContent = m_refApi.EkContentRef
                Dim req As New TaxonomyRequest
                req.TaxonomyId = TaxonomyParentId
                req.TaxonomyLanguage = TaxonomyLanguage

                If TaxonomyParentId > 0 Then
                    m_bSynchronized = m_refContent.IsSynchronizedTaxonomy(TaxonomyParentId, TaxonomyLanguage)
                ElseIf TaxonomyId > 0 Then
                    m_bSynchronized = m_refContent.IsSynchronizedTaxonomy(TaxonomyId, TaxonomyLanguage)
                End If
                If Not m_bSynchronized Then
                    tr_enableDisable.Visible = False
                End If
                Dim data As TaxonomyBaseData = m_refContent.ReadTaxonomy(req)
                If (data Is Nothing) Then
                    EkException.ThrowException(New Exception("Invalid taxonomy ID: " & TaxonomyId & " parent: " & TaxonomyParentId))
                End If
                language_data = (New SiteAPI).GetLanguageById(TaxonomyLanguage)
                If ((language_data IsNot Nothing) AndAlso (m_refApi.EnableMultilingual = 1)) Then
                    lblLanguage.Text = "[" & language_data.Name & "]"
                End If
                taxonomy_image_thumb.ImageUrl = m_refApi.AppImgPath & "spacer.gif"
                m_strCurrentBreadcrumb = data.TaxonomyPath.Remove(0, 1).Replace("\", " > ")
                If (m_strCurrentBreadcrumb = "") Then
                    m_strCurrentBreadcrumb = "Root"
                End If
                If (TaxonomyParentId = 0) Then
                    inherittemplate.Visible = False
                    lblInherited.Text = "No"
                Else
                    inherittemplate.Checked = True
                    taxonomytemplate.Enabled = False
                    inherittemplate.Visible = True
                    lblInherited.Text = ""
                End If
                Dim templates As TemplateData() = Nothing
                templates = m_refApi.GetAllTemplates("TemplateFileName")
                taxonomytemplate.Items.Add(New System.Web.UI.WebControls.ListItem("-select template-", 0))
                If (templates IsNot Nothing AndAlso templates.Length > 0) Then
                    For i As Integer = 0 To templates.Length - 1
                        taxonomytemplate.Items.Add(New System.Web.UI.WebControls.ListItem(templates(i).FileName, templates(i).Id))
                    Next
                End If

                inherittemplate.Attributes.Add("onclick", "OnInheritTemplateClicked(this)")
                inherittemplate.Attributes.Add("onclick", "OnInheritTemplateClicked(this)")
                If (TaxonomyParentId = 0) Then
                    tr_config.Visible = True
                Else
                    tr_config.Visible = False
                End If
                chkConfigContent.Checked = True
                LoadCustomPropertyList()
                TaxonomyToolBar()
            End If
        Catch ex As System.Threading.ThreadAbortException
            'Do nothing
        Catch ex As Exception
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(ex.Message & ".") & "&LangType=" & TaxonomyLanguage, False)
        End Try
    End Sub

    Private Sub TaxonomyToolBar()
        If (TaxonomyParentId = 0) Then
            txtTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("add taxonomy page title"))
        Else
            txtTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("add category page title"))
        End If

        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        If (TaxonomyParentId = 0) Then
            result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt save button text (taxonomy)"), m_refMsg.GetMessage("btn Save"), "onclick=""javascript:if(SetPropertyIds()){Validate();}"""))
        Else
            result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt save button text (category)"), m_refMsg.GetMessage("btn Save"), "onclick=""javascript:if(SetPropertyIds()){Validate();}"""))
        End If

        If (Request.QueryString("iframe") = "true") Then
            result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("generic Cancel"), "onClick=""javascript:parent.CancelIframe();"""))
        Else
            result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "taxonomy.aspx?action=view&taxonomyid=" & TaxonomyParentId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If

        result.Append("<td>" & m_refstyle.GetHelpButton("AddTaxonomyOrCategory") & "</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Private Sub LoadCustomPropertyList()

        Dim pageInfo As New PagingInfo
        pageInfo.CurrentPage = 1
        pageInfo.RecordsPerPage = 99999

        Dim i As Integer = 0
        Dim j As Integer = 1

        _customPropertyDataList = _customProperty.GetList(EkEnumeration.CustomPropertyObjectType.TaxonomyNode, m_refApi.ContentLanguage, pageInfo)
        For i = 0 To _customPropertyDataList.Count - 1
            If _customPropertyDataList(i).IsEnabled Then
                availableCustomProp.Items.Insert(j, _customPropertyDataList(i).PropertyName)
                availableCustomProp.Items.Item(j).Value = _customPropertyDataList(i).PropertyId
                j = j + 1
            End If
        Next

        availableCustomProp.DataBind()

    End Sub
    
    Private Sub AddCustomProperties()
        Dim propertyCount As Integer = 0
        Dim itemsCount As Integer = 0
        Dim valueCount As Integer = 0
        Dim i As Integer = 0
        Dim cp As New CustomProperty
        Dim cpo As New CustomPropertyObject
        Dim selectedIds As String() = Nothing
        Dim selectedValues As String() = Nothing

        If Request.Form(hdnSelectedIDS.UniqueID) <> "" Then
            selectedIds = Request.Form(hdnSelectedIDS.UniqueID).Remove(Request.Form(hdnSelectedIDS.UniqueID).Length - 1, 1).Split(";")
        End If
        If Request.Form(hdnSelectValue.UniqueID) <> "" Then
            selectedValues = Request.Form(hdnSelectValue.UniqueID).Remove(Request.Form(hdnSelectValue.UniqueID).Length - 1, 1).Split(";")
        End If

        If selectedIds IsNot Nothing And selectedValues IsNot Nothing Then
            If selectedIds.Length = selectedValues.Length Then
                For i = 0 To selectedIds.Length - 1
                    Dim customPropertyData = cp.GetItem(selectedIds(i), m_refApi.ContentLanguage)
                    Dim data As CustomPropertyObjectData = New CustomPropertyObjectData(TaxonomyId, m_refApi.ContentLanguage, selectedIds(i), EkEnumeration.CustomPropertyObjectType.TaxonomyNode)

                    If ((Not IsNothing(customPropertyData)) AndAlso (Not IsNothing(data))) Then

                        Dim inputValue As String = HttpUtility.UrlDecode(selectedValues(i).ToString())

                        Select Case customPropertyData.PropertyDataType
                            Case EkEnumeration.CustomPropertyItemDataType.Boolean
                                Dim booleanValue As Boolean
                                If (Boolean.TryParse(inputValue, booleanValue)) Then
                                    data.AddItem(booleanValue)
                                End If

                            Case EkEnumeration.CustomPropertyItemDataType.DateTime
                                Dim dateTimeValue As DateTime
                                If (DateTime.TryParse(inputValue, dateTimeValue)) Then
                                    data.AddItem(dateTimeValue)
                                End If
                            Case Else
                                data.AddItem(inputValue)
                        End Select
                        cpo.Add(data)
                    End If
                Next
            End If
        End If
    End Sub

End Class
