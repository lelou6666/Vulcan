Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Workarea
Imports Ektron.Newtonsoft.Json
Imports Ektron.Cms.Commerce.Workarea
Imports Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs
Imports Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData

Namespace Ektron.Cms.Commerce.Workarea.CatalogEntry
    Partial Class CatalogEntry
        Inherits workareabase

#Region "Variables"

        Private _ApplicationPath As String
        Private _SitePath As String

        Protected _ContentEditorId As String = ""
		Protected cdEditor As Ektron.ContentDesignerWithValidator
        Protected m_sEditAction As String = ""
        Protected editorPackage As String = ""
        Protected m_refProductType As ProductType = Nothing
        Protected prod_type_data As ProductTypeData = Nothing
        Protected xid As Long = 0
        Protected bSuppressTemplate As Boolean = False
        Protected catalog_data As New FolderData
        Protected lValidCounter As Integer = 0
        Protected meta_data As New List(Of ContentMetaData)
        Protected entry_edit_data As EntryData = Nothing
        Protected m_refSite As Ektron.Cms.Site.EkSite = Nothing
        Protected m_iFolder As Long = 0
        Protected m_mMeasures As MeasurementData = Nothing
        Protected m_refCatalog As Ektron.Cms.Commerce.CatalogEntry = Nothing
        Protected m_refTaxClass As TaxClass = Nothing
        Protected m_refCurrency As Currency = Nothing

        Protected TaxonomyTreeIdList As String = ""
        Protected TaxonomyTreeParentIdList As String = ""
        Protected TaxonomyRoleExists As Boolean = False
        Protected m_intTaxFolderId As Long = 0
        Protected TaxonomyOverrideId As Long = 0
        Protected TaxonomySelectId As Long = 0
        Private m_SelectedEditControl As String
        Private AppLocaleString As String = ""

        'Varibles used for Aliasing
        Private _urlAliasSettingApi As New UrlAliasing.UrlAliasSettingsApi
        Private m_strManualAlias As String = String.Empty
        Private m_manualAliasId As Long = 0
        Private m_strManualAliasExt As String = String.Empty
        Private m_prevManualAliasName As String = String.Empty
        Private m_currManualAliasName As String = String.Empty
        Private m_prevManualAliasExt As String = String.Empty
        Private m_currManualAliasExt As String = String.Empty
        Private m_cPerms As PermissionData = Nothing
        Private UserRights As PermissionData
        Private ShowTaxonomyTab As Boolean = True
        Private m_refContent As Ektron.Cms.Content.EkContent

        'js member vars
        'js: page function vars
        Private _JsPageFunctions_ContentEditorId As String = "default"
        'js: taxonomy function vars
        Private _JSTaxonomyFunctions_FolderId As String = "default"
        Private _JSTaxonomyFunctions_TaxonomyOverrideId As String = "default"
        Private _JSTaxonomyFunctions_TaxonomyTreeIdList As String = "default"
        Private _JSTaxonomyFunctions_TaxonomyTreeParentIdList As String = "default"
        Private _JSTaxonomyFunctions_ShowTaxonomy As String = "default"
        Private _JSTaxonomyFunctions_TaxonomyFolderId As String = "default"
        Private _inContextEditing As Boolean = False
        Private _stylesheet As String = ""
        Private _stylesheetPath As String = ""
        Private backLangType As Long = 1033
        Private otherLangId As Long = 0


#End Region

#Region "Properties"

        Private Property ApplicationPath() As String
            Get
                Return _ApplicationPath
            End Get
            Set(ByVal value As String)
                _ApplicationPath = value
            End Set
        End Property

        Private Property SitePath() As String
            Get
                Return _SitePath
            End Get
            Set(ByVal value As String)
                _SitePath = value
            End Set
        End Property

#End Region

#Region "Page Functions"

        Protected Sub New()

            Dim slash() As Char = {"/"}
            Me.SitePath = m_refContentApi.SitePath.TrimEnd(slash)
            Me.ApplicationPath = m_refContentApi.ApplicationPath.TrimEnd(slash)

        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
			With contentEditor
				If Request.Browser.Type = "IE6" Then
					.Width = New Unit(1200, UnitType.Pixel)
					.Height = New Unit(400, UnitType.Pixel)
				ElseIf Request.Browser.Type.IndexOf("Firefox") <> -1 Then
					.Width = New Unit(100, UnitType.Percentage)
					.Height = New Unit(800, UnitType.Pixel)
				Else
					.Width = New Unit(100, UnitType.Percentage)
					.Height = New Unit(500, UnitType.Pixel)
				End If
			End With

			With summaryEditor
				If Request.Browser.Type = "IE6" Then
					.Width = New Unit(1200, UnitType.Pixel)
					.Height = New Unit(400, UnitType.Pixel)
				ElseIf Request.Browser.Type.IndexOf("Firefox") <> -1 Then
					.Width = New Unit(100, UnitType.Percentage)
					.Height = New Unit(800, UnitType.Pixel)
				Else
					.Width = New Unit(100, UnitType.Percentage)
					.Height = New Unit(500, UnitType.Pixel)
				End If
			End With
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Try
                If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
                    Throw New Exception(GetMessage("feature locked error"))
                End If
                Util_ObtainValues()
                Util_CheckAccess()
                m_refCatalog = New Ektron.Cms.Commerce.CatalogEntry(m_refContentApi.RequestInformationRef)
                m_refCurrency = New Currency(m_refContentApi.RequestInformationRef)
                m_refContent = m_refContentApi.EkContentRef
                hdn_defaultCurrency.Value = m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId

                Select Case Me.m_sEditAction
                    Case "add", "addlang"
                        m_iFolder = Me.m_iID
                        If Not Page.IsPostBack Then
                            UserRights = m_refContentApi.LoadPermissions(m_iFolder, "folder", ContentAPI.PermissionResultType.Folder)
                            Dim defaultMeta() As ContentMetaData
                            Util_CheckFolderType()
                            If m_sEditAction = "addlang" Then
                                entry_edit_data = m_refCatalog.GetItem(otherLangId, backLangType)
                            End If
                            If entry_edit_data Is Nothing Then Util_GetEntryType()
                            defaultMeta = m_refContentApi.GetMetaDataTypes("id")
                            If defaultMeta IsNot Nothing AndAlso defaultMeta.Length > 0 Then meta_data.AddRange(defaultMeta)
                            Display_ContentTab()
                            Display_SummaryTab()
                            Display_EntryTab()
                            Display_PricingTab()
                            Display_MediaTab()
                            Display_ItemTab()
                            Display_MetadataTab()
                            Display_ScheduleTab()
                            Display_TaxonomyTab()
                            Display_CommentTab()
                            Display_TemplateTab()
                            If ((_urlAliasSettingApi.IsManualAliasEnabled Or _urlAliasSettingApi.IsAutoAliasEnabled) _
                            And m_refContentApi.IsARoleMember(Common.EkEnumeration.CmsRoleIds.EditAlias)) Then
                                Display_AliasTab()
                            End If
                            Util_SetLabels()
                        Else
                            Process_Add()
                        End If
                    Case "update"
                        If Not Page.IsPostBack Then
                            UserRights = m_refContentApi.LoadPermissions(m_iID, "content", ContentAPI.PermissionResultType.Content)
                            entry_edit_data = m_refCatalog.GetItemEdit(m_iID, m_refContentApi.RequestInformationRef.ContentLanguage, True)
                            If entry_edit_data.ProductType.Id > 0 Then
                                m_refProductType = New ProductType(m_refContentApi.RequestInformationRef)
                                prod_type_data = m_refProductType.GetItem(entry_edit_data.ProductType.Id, True)
                                editorPackage = prod_type_data.PackageXslt
                                xid = prod_type_data.Id
                                hdn_xmlid.Value = xid
                                hdn_entrytype.Value = entry_edit_data.EntryType
                            End If
                            meta_data = entry_edit_data.Metadata
                            m_iFolder = entry_edit_data.FolderId
                            Util_CheckFolderType()
                            Display_ContentTab()
                            Display_SummaryTab()
                            Display_EntryTab()
                            Display_PricingTab()
                            Display_MediaTab()
                            Display_ItemTab()
                            Display_MetadataTab()
                            Display_ScheduleTab()
                            Display_TaxonomyTab()
                            Display_CommentTab()
                            Display_TemplateTab()
                            If ((_urlAliasSettingApi.IsManualAliasEnabled Or _urlAliasSettingApi.IsAutoAliasEnabled) _
                           And m_refContentApi.IsARoleMember(Common.EkEnumeration.CmsRoleIds.EditAlias)) Then
                                Display_AliasTab()
                            End If
                            Util_SetLabels()
                        Else
                            Process_Edit()
                        End If
                End Select
                Util_SetJS()

                If prod_type_data IsNot Nothing Then hdn_productType.Value = prod_type_data.EntryClass

                Me.RegisterJs()
                Me.RegisterCss()
            Catch ex As Exception

                Utilities.ShowError(ex.Message)

            End Try

        End Sub

#End Region

#Region "Process"


#Region "Add"

        Public Sub Process_Add()
            If Request.Form("hdn_xmlid") <> "" AndAlso Request.Form("hdn_xmlid") > 0 Then
                hdn_xmlid.Value = Request.Form("hdn_xmlid")
                m_refProductType = New ProductType(m_refContentApi.RequestInformationRef)
                prod_type_data = m_refProductType.GetItem(hdn_xmlid.Value, True)
                xid = prod_type_data.Id
            End If
            Select Case Request.Form("hdn_entrytype")
                Case CatalogEntryType.SubscriptionProduct
                    Process_AddSubscription()
                Case CatalogEntryType.Bundle
                    Process_AddBundle()
                Case CatalogEntryType.Kit
                    Process_AddKit()
                Case CatalogEntryType.Product, CatalogEntryType.ComplexProduct
                    Process_AddProduct()
            End Select
        End Sub

        Public Sub Process_AddSubscription()

            Dim entry As New SubscriptionProductData()
            Dim urlAliasInfo As New UrlAliasInfo()

            entry = Process_GetEntryAddValues(entry)

            entry.SubscriptionInfo = Process_GetSubscriptionInfo(entry)

            urlAliasInfo = Process_Alias()

            Try
                Select Case hdn_publishaction.Value
                    Case EkEnumeration.AssetActionType.Save
                        m_refCatalog.AddAndSave(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderId)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" & entry.LanguageId & "&id=" & entry.Id & "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" & entry.FolderId & "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "&back_LangType=" & entry.LanguageId & "&rnd=6") ' goes to edit screen.
                    Case EkEnumeration.AssetActionType.Checkin
                        m_refCatalog.AddAndCheckIn(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderId)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                    Case EkEnumeration.AssetActionType.Submit
                        m_refCatalog.AddAndPublish(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderId)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=ViewContentByCategory&id=" & Me.m_iFolder.ToString()) ' goes to folder
                End Select
            Catch ex As Exception
                Util_ResponseHandler("../reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & entry.LanguageId)
            End Try
        End Sub

        Public Sub Process_AddBundle()
            Dim entry As New BundleData()
            Dim urlAliasInfo As New UrlAliasInfo()

            entry = Process_GetEntryAddValues(entry)

            entry.BundledItems = Process_GetBundledItems()

            urlAliasInfo = Process_Alias()

            Try
                Select Case hdn_publishaction.Value
                    Case EkEnumeration.AssetActionType.Save
                        m_refCatalog.AddAndSave(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderId)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" & entry.LanguageId & "&id=" & entry.Id & "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" & entry.FolderId & "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "&back_LangType=" & entry.LanguageId & "&rnd=6") ' goes to edit screen.
                    Case EkEnumeration.AssetActionType.Checkin
                        m_refCatalog.AddAndCheckIn(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderId)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                    Case EkEnumeration.AssetActionType.Submit
                        m_refCatalog.AddAndPublish(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderId)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=ViewContentByCategory&id=" & Me.m_iFolder.ToString()) ' goes to folder
                End Select
            Catch ex As Exception
                Util_ResponseHandler("../reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & entry.LanguageId)
            End Try
        End Sub

        Public Sub Process_AddKit()
            Dim entry As New KitData()
            Dim urlAliasInfo As New UrlAliasInfo()

            entry = Process_GetEntryAddValues(entry)

            entry.OptionGroups = Process_GetKitGroups()

            urlAliasInfo = Process_Alias()

            Try
                Select Case hdn_publishaction.Value
                    Case EkEnumeration.AssetActionType.Save
                        m_refCatalog.AddAndSave(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderId)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" & entry.LanguageId & "&id=" & entry.Id & "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" & entry.FolderId & "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "&back_LangType=" & entry.LanguageId & "&rnd=6") ' goes to edit screen.
                    Case EkEnumeration.AssetActionType.Checkin
                        m_refCatalog.AddAndCheckIn(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderId)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                    Case EkEnumeration.AssetActionType.Submit
                        m_refCatalog.AddAndPublish(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderId)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=ViewContentByCategory&id=" & Me.m_iFolder.ToString()) ' goes to folder
                End Select
            Catch ex As Exception
                Util_ResponseHandler("../reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & entry.LanguageId)
            End Try
        End Sub

        Public Sub Process_AddProduct()

            Dim entry As New ProductData()
            Dim urlAliasInfo As New UrlAliasInfo()

            entry = Process_GetEntryAddValues(entry)

            Me.ucItem.EntryEditData = entry
            Me.ucItem.ItemsFolderId = m_iFolder
            entry.Variants = Process_GetVariants()

            If entry.Variants.Count > 0 Then entry.EntryType = CatalogEntryType.ComplexProduct

            urlAliasInfo = Process_Alias()

            Try
                Select Case hdn_publishaction.Value
                    Case EkEnumeration.AssetActionType.Save
                        m_refCatalog.AddAndSave(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderID)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" & entry.LanguageId & "&id=" & entry.Id & "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" & entry.FolderId & "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "&back_LangType=" & entry.LanguageId & "&rnd=6") ' goes to edit screen.
                    Case EkEnumeration.AssetActionType.Checkin
                        m_refCatalog.AddAndCheckIn(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderID)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                    Case EkEnumeration.AssetActionType.Submit
                        m_refCatalog.AddAndPublish(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id, entry.FolderID)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=ViewContentByCategory&id=" & Me.m_iFolder.ToString()) ' goes to folder
                End Select
            Catch ex As Exception
                Util_ResponseHandler("../reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & entry.LanguageId)
            End Try
        End Sub

        Public Function Process_GetEntryAddValues(ByVal entry As EntryData) As EntryData

            If Request.QueryString("content_id") <> "" Then entry.Id = Request.QueryString("content_id")

            entry.Title = Request.Form("content_title")
            entry.LanguageId = ContentLanguage
			entry.Html = contentEditor.Content
			entry.Summary = summaryEditor.Content 
            entry.Image = Process_GetDefaultImage() ' Request.Form("entry_image")
            entry.Comment = txt_comment.Text
            entry.FolderId = m_iFolder
            entry.Comment = txt_comment.Text
            entry.ProductType.Id = hdn_xmlid.Value
            entry.TemplateId = drp_tempsel.SelectedValue

            entry.Sku = txt_sku.Text
            entry.QuantityMultiple = EkFunctions.ReadIntegerValue(txt_quantity.Text, 1)
            entry.EntryType = Request.Form("hdn_entrytype")
            entry.TaxClassId = drp_taxclass.SelectedValue
            entry.IsArchived = chk_avail.Checked
            ' entry.IsMarkedForDeletion = chk_markdel.Checked
            entry.IsBuyable = chk_buyable.Checked

            If Not chk_tangible.Checked Then
                entry.Dimensions.Height = 0
                entry.Dimensions.Length = 0
                entry.Dimensions.Width = 0
                entry.Weight.Amount = 0
            Else
                If m_refContentApi.RequestInformationRef.MeasurementSystem = Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English Then
                    entry.Dimensions.Units = LinearUnit.Inches
                Else
                    entry.Dimensions.Units = LinearUnit.Centimeters
                End If
                entry.Dimensions.Height = EkFunctions.ReadDecimalValue(txt_height.Text)
                entry.Dimensions.Length = EkFunctions.ReadDecimalValue(txt_length.Text)
                entry.Dimensions.Width = EkFunctions.ReadDecimalValue(txt_width.Text)
                If m_refContentApi.RequestInformationRef.MeasurementSystem = Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English Then
                    entry.Weight.Units = WeightUnit.Pounds
                Else
                    entry.Weight.Units = WeightUnit.Kilograms
                End If
                entry.Weight.Amount = EkFunctions.ReadDecimalValue(txt_weight.Text)
            End If

            entry.Pricing = Process_GetPricing()

            entry.Media = Process_GetMedia()

            entry.Metadata = Process_GetMetaData()

            entry.Attributes = Process_GetAttributes()

            If (Request.Form("end_date") <> "") Then
                entry.EndDate = CDate(Trim(Request.Form("end_date")))
                entry.EndDateAction = rblaction.SelectedValue
            Else
                entry.EndDate = Date.MinValue
                entry.EndDateAction = 1
            End If

            If (Request.Form("go_live") <> "") Then
                entry.GoLive = Request.Form("go_live")
            Else
                entry.GoLive = Date.MinValue
            End If

            entry.DisableInventoryManagement = chk_disableInv.Checked

            Return entry

        End Function

#End Region


#Region "Edit"

        Public Sub Process_Edit()
            If Request.Form("hdn_xmlid") <> "" AndAlso Request.Form("hdn_xmlid") > 0 Then
                hdn_xmlid.Value = Request.Form("hdn_xmlid")
                m_refProductType = New ProductType(m_refContentApi.RequestInformationRef)
                prod_type_data = m_refProductType.GetItem(hdn_xmlid.Value, True)
                xid = prod_type_data.Id
            End If
            Select Case Request.Form("hdn_entrytype")
                Case CatalogEntryType.SubscriptionProduct
                    Process_EditSubscription()
                Case CatalogEntryType.Bundle
                    Process_EditBundle()
                Case CatalogEntryType.Kit
                    Process_EditKit()
                Case CatalogEntryType.Product, CatalogEntryType.ComplexProduct
                    Process_EditProduct()
            End Select
        End Sub

        Public Function Process_GetEntryValues(ByVal entry As EntryData) As EntryData

            entry.Title = Request.Form("content_title")
			entry.Html = contentEditor.Content
			entry.Summary = summaryEditor.Content
			entry.Image = Process_GetDefaultImage()	' Request.Form("entry_image")
            entry.Comment = txt_comment.Text
            'entry.FolderId = m_iFolder
            entry.Comment = txt_comment.Text
            entry.ProductType.Id = hdn_xmlid.Value
            entry.TemplateId = drp_tempsel.SelectedValue

            entry.Sku = txt_sku.Text
            entry.QuantityMultiple = EkFunctions.ReadIntegerValue(txt_quantity.Text, 1)
            ' entry.EntryType = Request.Form("hdn_entrytype")
            entry.TaxClassId = drp_taxclass.SelectedValue
            entry.IsArchived = chk_avail.Checked
            ' entry.IsMarkedForDeletion = chk_markdel.Checked
            entry.IsBuyable = chk_buyable.Checked

            If Not chk_tangible.Checked Then
                entry.Dimensions.Height = 0
                entry.Dimensions.Length = 0
                entry.Dimensions.Width = 0
                entry.Weight.Amount = 0
            Else
                If m_refContentApi.RequestInformationRef.MeasurementSystem = Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English Then
                    entry.Dimensions.Units = LinearUnit.Inches
                Else
                    entry.Dimensions.Units = LinearUnit.Centimeters
                End If
                entry.Dimensions.Height = EkFunctions.ReadDecimalValue(txt_height.Text)
                entry.Dimensions.Length = EkFunctions.ReadDecimalValue(txt_length.Text)
                entry.Dimensions.Width = EkFunctions.ReadDecimalValue(txt_width.Text)
                If m_refContentApi.RequestInformationRef.MeasurementSystem = Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English Then
                    entry.Weight.Units = WeightUnit.Pounds
                Else
                    entry.Weight.Units = WeightUnit.Kilograms
                End If
                entry.Weight.Amount = EkFunctions.ReadDecimalValue(txt_weight.Text)
            End If

            entry.Pricing = Process_GetPricing(entry.Pricing)

            entry.Media = Process_GetMedia()

            entry.Metadata = Process_GetMetaData()

            entry.Attributes = Process_GetAttributes()

            If (Request.Form("end_date") <> "") Then
                entry.EndDate = CDate(Trim(Request.Form("end_date")))
                entry.EndDateAction = rblaction.SelectedValue
            Else
                entry.EndDate = Date.MinValue
                entry.EndDateAction = 0
            End If

            If (Request.Form("go_live") <> "") Then
                entry.GoLive = Request.Form("go_live")
            Else
                entry.GoLive = Date.MinValue
            End If

            entry.DisableInventoryManagement = chk_disableInv.Checked

            Return entry

        End Function

        Public Sub Process_EditSubscription()
            Dim entry As SubscriptionProductData = Nothing
            Dim urlAliasInfo As New UrlAliasInfo()
            entry = m_refCatalog.GetItemEdit(m_iID, m_refContentApi.RequestInformationRef.ContentLanguage, False)
            If hdn_publishaction.Value <> EkEnumeration.AssetActionType.UndoCheckout Then

                entry = Process_GetEntryValues(entry)

                entry.SubscriptionInfo = Process_GetSubscriptionInfo(entry)

                urlAliasInfo = Process_Alias()

            End If
            Try


                Select Case hdn_publishaction.Value
                    Case EkEnumeration.AssetActionType.Save
                        m_refCatalog.Save(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" & entry.LanguageId & "&id=" & entry.Id & "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" & entry.FolderId & "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "&back_LangType=" & entry.LanguageId & "&rnd=6") ' goes to edit screen.
                    Case EkEnumeration.AssetActionType.Checkin
                        m_refCatalog.SaveAndCheckIn(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                    Case EkEnumeration.AssetActionType.Submit
                        m_refCatalog.SaveAndPublish(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                    Case EkEnumeration.AssetActionType.UndoCheckout
                        m_refCatalog.UndoCheckOut(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & ContentLanguage & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                End Select
            Catch ex As Exception
                Util_ResponseHandler("../reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & entry.LanguageId)
            End Try
        End Sub

        Public Sub Process_EditBundle()
            Dim entry As BundleData = Nothing
            Dim urlAliasInfo As New UrlAliasInfo()
            entry = m_refCatalog.GetItemEdit(m_iID, m_refContentApi.RequestInformationRef.ContentLanguage, False)
            If hdn_publishaction.Value <> EkEnumeration.AssetActionType.UndoCheckout Then

                entry = Process_GetEntryValues(entry)

                entry.BundledItems = Process_GetBundledItems()

                urlAliasInfo = Process_Alias()

            End If
            Try


                Select Case hdn_publishaction.Value
                    Case EkEnumeration.AssetActionType.Save
                        m_refCatalog.Save(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" & entry.LanguageId & "&id=" & entry.Id & "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" & entry.FolderId & "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "&back_LangType=" & entry.LanguageId & "&rnd=6") ' goes to edit screen.
                    Case EkEnumeration.AssetActionType.Checkin
                        m_refCatalog.SaveAndCheckIn(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                    Case EkEnumeration.AssetActionType.Submit
                        m_refCatalog.SaveAndPublish(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                    Case EkEnumeration.AssetActionType.UndoCheckout
                        m_refCatalog.UndoCheckOut(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & ContentLanguage & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                End Select
            Catch ex As Exception
                Util_ResponseHandler("../reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & entry.LanguageId)
            End Try
        End Sub

        Public Sub Process_EditKit()
            Dim entry As KitData = Nothing
            Dim urlAliasInfo As New UrlAliasInfo()
            entry = m_refCatalog.GetItemEdit(m_iID, m_refContentApi.RequestInformationRef.ContentLanguage, False)
            If hdn_publishaction.Value <> EkEnumeration.AssetActionType.UndoCheckout Then

                entry = Process_GetEntryValues(entry)

                entry.OptionGroups = Process_GetKitGroups()

                urlAliasInfo = Process_Alias()

            End If
            Try


                Select Case hdn_publishaction.Value
                    Case EkEnumeration.AssetActionType.Save
                        m_refCatalog.Save(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" & entry.LanguageId & "&id=" & entry.Id & "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" & entry.FolderId & "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "&back_LangType=" & entry.LanguageId & "&rnd=6") ' goes to edit screen.
                    Case EkEnumeration.AssetActionType.Checkin
                        m_refCatalog.SaveAndCheckIn(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                    Case EkEnumeration.AssetActionType.Submit
                        m_refCatalog.SaveAndPublish(entry, urlAliasInfo)
                        Process_Taxonomy(entry.Id)
                        Process_Inventory(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                    Case EkEnumeration.AssetActionType.UndoCheckout
                        m_refCatalog.UndoCheckOut(entry.Id)
                        Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & ContentLanguage & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                End Select
            Catch ex As Exception
                Util_ResponseHandler("../reterror.aspx?info=" & Server.UrlEncode(ex.Message) & "&LangType=" & entry.LanguageId)
            End Try
        End Sub

        Public Sub Process_EditProduct()
            Dim entry As ProductData = Nothing
            Dim urlAliasInfo As New UrlAliasInfo()
            entry = m_refCatalog.GetItemEdit(m_iID, m_refContentApi.RequestInformationRef.ContentLanguage, False)
            If hdn_publishaction.Value <> EkEnumeration.AssetActionType.UndoCheckout Then

                entry = Process_GetEntryValues(entry)

                Me.ucItem.EntryEditData = entry
                Me.ucItem.ItemsFolderId = m_iFolder
                entry.Variants = Process_GetVariants()

                urlAliasInfo = Process_Alias()

                If entry.Variants.Count > 0 Then entry.EntryType = CatalogEntryType.ComplexProduct
                If entry.Variants.Count = 0 Then entry.EntryType = CatalogEntryType.Product

            End If
            Select Case hdn_publishaction.Value
                Case EkEnumeration.AssetActionType.Save
                    m_refCatalog.Save(entry, urlAliasInfo)
                    Process_Taxonomy(entry.Id, entry.FolderId)
                    Process_Inventory(entry.Id)
                    Util_ResponseHandler("catalogentry.aspx?close=false&LangType=" & entry.LanguageId & "&id=" & entry.Id & "&type=update&back_file=cmsform.aspx&back_action=ViewStaged&back_folder_id=" & entry.FolderId & "&back_callerpage=content.aspx&back_origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "&back_LangType=" & entry.LanguageId & "&rnd=6") ' goes to edit screen.
                Case EkEnumeration.AssetActionType.Checkin
                    m_refCatalog.SaveAndCheckIn(entry, urlAliasInfo)
                    Process_Taxonomy(entry.Id, entry.FolderId)
                    Process_Inventory(entry.Id)
                    Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                Case EkEnumeration.AssetActionType.Submit
                    m_refCatalog.SaveAndPublish(entry, urlAliasInfo)
                    Process_Taxonomy(entry.Id, entry.FolderId)
                    Process_Inventory(entry.Id)
                    Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & entry.LanguageId & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
                Case EkEnumeration.AssetActionType.UndoCheckout
                    m_refCatalog.UndoCheckOut(entry.Id)
                    Util_ResponseHandler("../content.aspx?action=View&folder_id=" & entry.FolderId & "&id=" & entry.Id & "&LangType=" & ContentLanguage & "&callerpage=content.aspx&origurl=action%3dViewContentByCategory%26id%3d" & entry.FolderId & "%26contentid%3d0%26form_id%3d0%26LangType%3d" & entry.LanguageId) ' goes to content view screen
            End Select
        End Sub

#End Region


#Region "Other"

        Public Function Process_GetAttributes() As List(Of EntryAttributeData)
            Dim attributeList As New List(Of EntryAttributeData)()

            Dim iValidCounter As Integer = 101
            While Request.Form("frm_meta_type_id_" & iValidCounter) <> ""
                Dim attributeIndex As Integer = -1
                Dim attribute As New EntryAttributeData()
                Dim MetaSelect As String
                Dim separater As String = ""
                attribute.AttributeTypeId = Request.Form("frm_meta_type_id_" & iValidCounter)
                If prod_type_data IsNot Nothing Then
                    For j As Integer = 0 To (prod_type_data.Attributes.Count - 1)
                        If prod_type_data.Attributes(j).Id = attribute.AttributeTypeId Then
                            attribute.ActiveStatus = prod_type_data.Attributes(j).ActiveStatus
                            attribute.DataType = prod_type_data.Attributes(j).DataType
                            attribute.DefaultValue = prod_type_data.Attributes(j).DefaultValue
                            attribute.DisplayOrder = prod_type_data.Attributes(j).DisplayOrder
                            attribute.Name = prod_type_data.Attributes(j).Name
                            attributeIndex = j

                            separater = Request.Form("MetaSeparator_" & iValidCounter)
                            MetaSelect = Request.Form("MetaSelect_" & iValidCounter)
                            If (MetaSelect <> "") Then
                                If attribute.DataType = ProductTypeAttributeDataType.Boolean Then
                                    If Replace(Request.Form("frm_text_" & iValidCounter), ", ", separater) <> "" Then
                                        Select Case Request.Form("frm_text_" & iValidCounter)
                                            Case "Yes"
                                                attribute.CurrentValue = True
                                            Case "No"
                                                attribute.CurrentValue = False
                                        End Select
                                    Else
                                        attribute.CurrentValue = False
                                    End If
                                Else
                                    If Request.Form("frm_text_" & iValidCounter) IsNot Nothing Then
                                        attribute.CurrentValue = Request.Form("frm_text_" & iValidCounter)
                                    Else
                                        attribute.CurrentValue = attribute.DefaultValue
                                    End If
                                End If
                            Else
                                Dim myMeta As String = ""
                                myMeta = Request.Form("frm_text_" & iValidCounter)
                                myMeta = Server.HtmlDecode(myMeta)
                                attribute.CurrentValue = Replace(myMeta, ";", separater)
                            End If
                            attributeList.Add(attribute)

                            Exit For
                        End If
                    Next
                End If
                iValidCounter = iValidCounter + 1
            End While

            Return attributeList
        End Function

        Public Sub Process_Taxonomy(ByVal entryId As Long)
            Process_Taxonomy(entryId, -1)
        End Sub
        Public Sub Process_Taxonomy(ByVal entryId As Long, ByVal folderID As Long)
            If (TaxonomyOverrideId > 0) Then
                TaxonomyTreeIdList = TaxonomyOverrideId
            End If
            If (Request.Form(taxonomyselectedtree.UniqueID) IsNot Nothing AndAlso Request.Form(taxonomyselectedtree.UniqueID) <> "") Then
                TaxonomyTreeIdList = Request.Form(taxonomyselectedtree.UniqueID)
                If (TaxonomyTreeIdList.Trim.EndsWith(",")) Then
                    TaxonomyTreeIdList = TaxonomyTreeIdList.Substring(0, TaxonomyTreeIdList.Length - 1)
                End If
            End If
            Dim entry_request As New TaxonomyContentRequest
            If folderID <> -1 Then
                entry_request.FolderID = folderID
            End If
            entry_request.ContentId = entryId
            entry_request.TaxonomyList = TaxonomyTreeIdList
            m_refContentApi.AddTaxonomyItem(entry_request)
        End Sub

        Public Sub Process_Inventory(ByVal entryId As Long)

            If Not chk_disableInv.Checked Then

                Dim inventoryApi As New InventoryApi()
                Dim inventoryData As New InventoryData()

                inventoryData.EntryId = entryId
                inventoryData.UnitsInStock = EkFunctions.ReadIntegerValue(txt_instock.Text)
                inventoryData.UnitsOnOrder = EkFunctions.ReadIntegerValue(txt_onorder.Text)
                inventoryData.ReorderLevel = EkFunctions.ReadIntegerValue(txt_reorder.Text)

                inventoryApi.SaveInventory(inventoryData)

            End If

        End Sub

        Public Function Process_GetMetaData() As List(Of ContentMetaData)
            Dim lMeta As New System.Collections.Generic.List(Of ContentMetaData)()
            Dim iValidCounter As Integer = 0

            If (Request.Form("frm_validcounter") <> "") Then
                iValidCounter = CInt(Request.Form("frm_validcounter"))
            Else
                iValidCounter = 0
            End If
            For i As Integer = 1 To iValidCounter
                Dim eMeta As New ContentMetaData()
                Dim MetaSelect As String
                eMeta.TypeId = Request.Form("frm_meta_type_id_" & i)
                eMeta.Separater = Request.Form("MetaSeparator_" & i)
                MetaSelect = Request.Form("MetaSelect_" & i)
                If (MetaSelect <> "") Then
                    eMeta.Text = Replace(Request.Form("frm_text_" & i), ", ", eMeta.Separater)
                    If (Left(eMeta.Text, 1) = eMeta.Separater) Then
                        eMeta.Text = Right(eMeta.Text, (Len(eMeta.Text) - 1))
                    End If
                Else
                    Dim myMeta As String = ""
                    myMeta = Request.Form("frm_text_" & i)
                    myMeta = Server.HtmlDecode(myMeta)
                    eMeta.Text = Replace(myMeta, ";", eMeta.Separater)
                End If
                lMeta.Add(eMeta)
            Next
            Return lMeta
        End Function

        Public Function Process_GetBundledItems() As List(Of EntryData)

            Dim aProducts As New List(Of EntryData)

            For i As Integer = 0 To (Me.ucItem.ItemData.Count - 1)
                If Me.ucItem.ItemData(i) IsNot Nothing And Me.ucItem.ItemData(i).MarkedForDelete = False Then
                    Dim BundleProduct As New ProductData()
                    BundleProduct.Id = Me.ucItem.ItemData(i).Id
                    BundleProduct.LanguageId = Me.ContentLanguage
                    aProducts.Add(BundleProduct)
                End If
            Next

            Return aProducts

        End Function

        Public Function Process_GetVariants() As List(Of ProductVariantData)

            Dim aVariants As New List(Of ProductVariantData)

            If Me.ucItem.ItemData IsNot Nothing Then
                For i As Integer = 0 To (Me.ucItem.ItemData.Count - 1)
                    If Me.ucItem.ItemData(i) IsNot Nothing And Me.ucItem.ItemData(i).MarkedForDelete = False Then
                        Dim ProductVariant As New ProductVariantData()
                        ProductVariant.Id = Me.ucItem.ItemData(i).Id
                        ProductVariant.LanguageId = Me.ContentLanguage
                        aVariants.Add(ProductVariant)
                    End If
                Next
            End If

            Return aVariants

        End Function

        Public Function Process_GetKitGroups() As List(Of OptionGroupData)
            Dim aGroups As New List(Of OptionGroupData)

            If Me.ucItem.ItemData IsNot Nothing Then

                For i As Integer = 0 To (Me.ucItem.ItemData.Count - 1)

                    If Me.ucItem.ItemData(i) IsNot Nothing And Me.ucItem.ItemData(i).MarkedForDelete = False Then

                        Dim OptionGroup As New OptionGroupData()
                        Dim kitGroup As Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.ClientData.Kitdata = Me.ucItem.ItemData(i)

                        OptionGroup.Id = kitGroup.Id
                        OptionGroup.Name = kitGroup.Title
                        OptionGroup.Image = ""
                        OptionGroup.Description = kitGroup.Description
                        OptionGroup.DisplayOrder = kitGroup.Order

                        Dim aOptions As New OptionGroupItemCollection()

                        For j As Integer = 0 To (kitGroup.Items.Count - 1)

                            If Not kitGroup.Items(j).MarkedForDelete Then

                                Dim OptionItem As New OptionGroupItemData()

                                OptionItem.DisplayOrder = kitGroup.Items(j).Order
                                OptionItem.Name = kitGroup.Items(j).Title
                                OptionItem.GroupId = OptionGroup.Id
                                OptionItem.Id = kitGroup.Items(j).Id
                                OptionItem.ExtraText = kitGroup.Items(j).ExtraText
                                OptionItem.PriceModification = Convert.ToDecimal(kitGroup.Items(j).PriceModifierDollars & "." & kitGroup.Items(j).PriceModifierCents)

                                If kitGroup.Items(j).PriceModifierPlusMinus = "-" Then OptionItem.PriceModification = OptionItem.PriceModification * -1

                                aOptions.Add(OptionItem)

                            End If

                        Next

                        OptionGroup.Options = aOptions
                        aGroups.Add(OptionGroup)

                    End If

                Next

            End If

            Return aGroups

        End Function

        Private Function Process_Alias() As UrlAliasInfo

            'Aliasing logic for 7.6 starts here
            Dim _manualAliasInfo As New UrlAliasInfo
            m_strManualAlias = Trim(CStr(Request.Form("frm_manalias")))
            m_strManualAliasExt = CStr(Request.Form("frm_manaliasExt"))
            m_prevManualAliasName = CStr(Request.Form("prev_frm_manalias_name"))
            m_prevManualAliasExt = CStr(Request.Form("prev_frm_manalias_ext"))
            m_currManualAliasName = m_strManualAlias
            m_currManualAliasExt = m_strManualAliasExt

            If (Request.Form("frm_manalias_id") <> "") Then
                m_manualAliasId = CLng(Request.Form("frm_manalias_id"))
            End If
            _manualAliasInfo.AliasId = m_manualAliasId
            _manualAliasInfo.CurrentAliasName = m_currManualAliasName
            _manualAliasInfo.CurrentAliasExtension = m_currManualAliasExt
            _manualAliasInfo.PreviousAliasName = m_prevManualAliasName
            _manualAliasInfo.PreviousAliasExtension = m_prevManualAliasExt
            Return _manualAliasInfo
        End Function

        Protected Function Process_GetSubscriptionInfo(ByVal entry As EntryData) As Object

            Dim subscriptionInfo As New Ektron.Cms.Commerce.subscriptions.MembershipSubscriptionInfo()
            Dim authorGroupId As Long = 0
            Dim memberGroupId As Long = 0

            Try

                authorGroupId = IIf(Request.Form("EktronSusbscriptionCmsGroupMarkedForDelete") = "false", Request.Form("EktronSusbscriptionCmsGroupId"), 0)
                memberGroupId = Request.Form("EktronSusbscriptionMembershipGroupId")

                subscriptionInfo.EntryId = entry.Id
                subscriptionInfo.AuthorGroupId = authorGroupId
                subscriptionInfo.MemberGroupId = memberGroupId

            Catch ex As Exception

            End Try


            Return subscriptionInfo

        End Function

        Protected Function Process_GetDefaultImage() As String

            Dim defaultImage As String = ""

            If Me.m_sEditAction = "addlang" Then

                defaultImage = Request.Form("entry_image")

            Else

                If ucMedia.ImageData IsNot Nothing Then

                    For i As Integer = 0 To (ucMedia.ImageData.Count - 1)

                        If ucMedia.ImageData(i).MarkedForDelete = False AndAlso ucMedia.ImageData(i).Default Then defaultImage = ucMedia.ImageData(i).Path

                    Next

                End If

            End If

            Return defaultImage

        End Function

        Protected Function Process_GetMedia(Optional ByVal media As MediaGalleryData = Nothing) As MediaGalleryData

            Dim ImageList As New List(Of ImageMediaData)
            If media Is Nothing Then media = New MediaGalleryData()

            If ucMedia.ImageData IsNot Nothing Then

                For i As Integer = 0 To (ucMedia.ImageData.Count - 1)

                    Dim image As New ImageMediaData

                    If ucMedia.ImageData(i).MarkedForDelete = False AndAlso ucMedia.ImageData(i).Id > 0 Then

                        image.Id = ucMedia.ImageData(i).Id
                        image.FileName = ucMedia.ImageData(i).Title
                        image.FilePath = ucMedia.ImageData(i).Path
                        image.Alt = ucMedia.ImageData(i).AltText
                        image.IncludedInGallery = ucMedia.ImageData(i).Gallery
                        If prod_type_data.DefaultThumbnails.Count > 0 Then

                            If ucMedia.ImageData(i).Thumbnails IsNot Nothing Then

                                For j As Integer = 0 To ucMedia.ImageData(i).Thumbnails.Count - 1

                                    image.Thumbnails.Add(New ThumbnailData())

                                    If ucMedia.ImageData(i).Thumbnails.Item(j).Path.IndexOf(m_refContentApi.SitePath) > -1 And m_refContentApi.SitePath <> "/" Then
                                        image.Thumbnails.Item(j).FilePath = ucMedia.ImageData(i).Thumbnails.Item(j).Path.Substring(ucMedia.ImageData(i).Thumbnails.Item(j).Path.IndexOf(m_refContentApi.SitePath) + m_refContentApi.SitePath.Length)
                                    ElseIf m_refContentApi.SitePath = "/" AndAlso ucMedia.ImageData(i).Thumbnails.Item(j).Path.StartsWith("/") Then
                                        image.Thumbnails.Item(j).FilePath = Replace(ucMedia.ImageData(i).Thumbnails.Item(j).Path, "/", "", 1, 1)
                                    Else
                                        image.Thumbnails.Item(j).FilePath = ucMedia.ImageData(i).Thumbnails.Item(j).Path
                                    End If

                                    image.Thumbnails(j).FilePath = image.Thumbnails(j).FilePath.Replace("\", "/") & "/" & ucMedia.ImageData(i).Thumbnails.Item(j).ImageName

                                    If ucMedia.ImageData(i).Thumbnails.Item(j).Path.LastIndexOf("/") > -1 Then
                                        image.Thumbnails.Item(j).FileName = ucMedia.ImageData(i).Thumbnails.Item(j).ImageName.Substring(ucMedia.ImageData(i).Thumbnails.Item(j).ImageName.LastIndexOf("/") + 1)
                                    Else
                                        image.Thumbnails.Item(j).FileName = ucMedia.ImageData(i).Thumbnails.Item(j).ImageName
                                    End If

                                Next

                            End If

                        End If

                        ImageList.Add(image)

                    End If

                Next

            End If

            media.Images = ImageList

            Return media

        End Function

        Protected Function Process_GetPricing(Optional ByVal currentPricing As PricingData = Nothing) As PricingData

            Dim updatedPricing As PricingData = New PricingData()
            ' If currentPricing Is Nothing Then currentPricing = New PricingData()

            Dim currencyList As List(Of CurrencyData)
            Dim currencyPriceList As New List(Of CurrencyPricingData)

            currencyList = m_refCurrency.GetActiveCurrencyList()

            For i As Integer = 0 To (currencyList.Count - 1)

                If Not Request.Form("ektron_UnitPricing_Float_" & currencyList(i).Id.ToString()) <> "" Then

                    Dim currencyPrice As New CurrencyPricingData()
                    Dim tierPriceList As New List(Of TierPriceData)
                    Dim tierIndex As Integer = 0
                    Dim defaultTierPrice As New TierPriceData()

                    'currencyPrice.ActualCost = EkFunctions.ReadDecimalValue(Request.Form("ektron_UnitPricing_ActualPrice_" & currencyList(i).Id.ToString()))
                    currencyPrice.AlphaIsoCode = currencyList(i).AlphaIsoCode
                    currencyPrice.CurrencyId = currencyList(i).Id
                    currencyPrice.ListPrice = EkFunctions.ReadDecimalValue(Request.Form("ektron_UnitPricing_ListPrice_" & currencyList(i).Id.ToString()))
                    currencyPrice.PricingType = PricingType.Fixed

                    defaultTierPrice.Quantity = 1
                    defaultTierPrice.Id = EkFunctions.ReadDbLong(Request.Form("hdn_ektron_UnitPricing_DefaultTier_" & currencyList(i).Id.ToString()))
                    defaultTierPrice.SalePrice = EkFunctions.ReadDecimalValue(Request.Form("ektron_UnitPricing_SalesPrice_" & currencyList(i).Id.ToString()))
                    currencyPrice.TierPrices.Add(defaultTierPrice)

                    While Request.Form("ektron_TierPricing_TierPrice_" & currencyList(i).Id.ToString() & "_" & tierIndex.ToString()) IsNot Nothing AndAlso IsNumeric(Request.Form("ektron_TierPricing_TierPrice_" & currencyList(i).Id.ToString() & "_" & tierIndex.ToString()))

                        Dim tierPrice As New TierPriceData()

                        If ((Request.Form("ektron_TierPricing_TierQuantity_" & currencyList(i).Id.ToString() & "_" & tierIndex.ToString()) = 0 Or Request.Form("ektron_TierPricing_TierQuantity_" & currencyList(i).Id.ToString() & "_" & tierIndex.ToString()) = 1) And (tierPrice.Id = 0 Or tierPrice.Id = defaultTierPrice.Id)) Then

                            Exit While

                        End If

                        If Request.Form("ektron_TierPricing_TierQuantity_" & currencyList(i).Id.ToString() & "_" & tierIndex.ToString()) > 1 And tierPrice.Id = 0 Then

                            tierPrice.Id = EkFunctions.ReadDbLong(Request.Form("hdn_ektron_TierPricing_TierId_" & currencyList(i).Id.ToString() & "_" & tierIndex.ToString()))
                            tierPrice.Quantity = EkFunctions.ReadDecimalValue(Request.Form("ektron_TierPricing_TierQuantity_" & currencyList(i).Id.ToString() & "_" & tierIndex.ToString()))
                            tierPrice.SalePrice = EkFunctions.ReadDecimalValue(Request.Form("ektron_TierPricing_TierPrice_" & currencyList(i).Id.ToString() & "_" & tierIndex.ToString()))

                            currencyPrice.TierPrices.Add(tierPrice)

                        End If

                        tierIndex = tierIndex + 1

                    End While

                    currencyPriceList.Add(currencyPrice)

                End If

            Next

            updatedPricing.CurrencyPricelist = currencyPriceList

            If Request.Form("PricingTabRecurringBillingUseRecurrentBilling") = "true" Then

                Dim pricingRecurrance As Ektron.Cms.Common.recurrenceData

                If Request.Form("PricingTabRecurringBillingBillingCycle") = "month" Then

                    pricingRecurrance = Ektron.Cms.Common.recurrenceData.CreateMonthlyByDayRecurrence(1, Ektron.Cms.Common.RecurrenceDayOfMonth.First, Ektron.Cms.Common.RecurrenceDaysOfWeek.Tuesday)

                Else

                    pricingRecurrance = Ektron.Cms.Common.recurrenceData.CreateYearlyRecurrence(1, 4, 15)

                End If

                pricingRecurrance.StartDateUtc = DateTime.Now
                pricingRecurrance.EndDateUtc = DateTime.Now
                pricingRecurrance.Intervals = Request.Form("PricingTabRecurringBillingInterval")

                updatedPricing.recurrence = pricingRecurrance

            End If

            Return updatedPricing

        End Function

#End Region


#End Region

#Region "Display - Tabs"

        Private Sub Display_ContentTab()
			content_title.Value = entry_edit_data.Title
			contentEditor.SetPermissions(m_cPerms)
			contentEditor.AllowFonts = True
			If _stylesheet <> "" Then
				contentEditor.Stylesheet = _stylesheetPath
			End If

			contentEditor.LoadPackage(m_refContentApi, editorPackage)
			Dim strXml As String = ""
			If entry_edit_data IsNot Nothing Then strXml = entry_edit_data.Html
			If (Trim(Len(strXml)) = 0) Then
				If (Trim(Len(editorPackage)) > 0) Then
					strXml = m_refContentApi.TransformXsltPackage(editorPackage, Server.MapPath(contentEditor.ScriptLocation & "unpackageDocument.xslt"), True)
				End If
			End If
			contentEditor.DataDocumentXml = strXml


            'set CatalogEntry_PageFunctions_Js vars - see RegisterJS() and CatalogEntry.PageFunctions.aspx under CatalogEntry/js
			_JsPageFunctions_ContentEditorId = "contentEditor"
        End Sub

        Private Sub Display_SummaryTab()

			contentEditor.SetPermissions(m_cPerms)
			contentEditor.AllowFonts = True
			If _stylesheet <> "" Then
				summaryEditor.Stylesheet = _stylesheetPath
			End If
			If entry_edit_data IsNot Nothing Then
				summaryEditor.Content = entry_edit_data.Summary
			End If
        End Sub

        Private Sub Display_EntryTab()

            Dim TaxClassList As New System.Collections.Generic.List(Of TaxClassData)()
            Dim criteria As New Ektron.Cms.Common.Criteria(Of TaxClassProperty)(TaxClassProperty.Id, Common.EkEnumeration.OrderByDirection.Ascending)

            m_refTaxClass = New TaxClass(Me.m_refContentApi.RequestInformationRef)
            TaxClassList = m_refTaxClass.GetList(criteria)

            drp_taxclass.DataTextField = "name"
            drp_taxclass.DataValueField = "id"
            drp_taxclass.DataSource = TaxClassList
            drp_taxclass.DataBind()

            If m_refContentApi.RequestInformationRef.MeasurementSystem = Ektron.Cms.Common.EkEnumeration.MeasurementSystem.English Then

                ltr_heightmeasure.Text = GetMessage("lbl inches")
                ltr_lengthmeasure.Text = GetMessage("lbl inches")
                ltr_widthmeasure.Text = GetMessage("lbl inches")
                ltr_weightmeasure.Text = GetMessage("lbl pounds")

            Else

                ltr_heightmeasure.Text = GetMessage("lbl centimeters")
                ltr_lengthmeasure.Text = GetMessage("lbl centimeters")
                ltr_widthmeasure.Text = GetMessage("lbl centimeters")
                ltr_weightmeasure.Text = GetMessage("lbl kilograms")

            End If

            Util_BindFieldList()

            If entry_edit_data IsNot Nothing Then
                txt_sku.Text = entry_edit_data.Sku
                txt_quantity.Text = entry_edit_data.QuantityMultiple
                drp_taxclass.SelectedValue = entry_edit_data.TaxClassId
                chk_avail.Checked = entry_edit_data.IsArchived
                ' chk_markdel.Checked = entry_edit_data.IsMarkedForDeletion
                If entry_edit_data.Id = 0 Then chk_buyable.Checked = True
                If entry_edit_data.IsArchived Then Page.ClientScript.RegisterStartupScript(Page.GetType(), "chk_buyable", "document.getElementById('chk_buyable').disabled = true;", True)
                If Not entry_edit_data.IsArchived Then chk_buyable.Checked = entry_edit_data.IsBuyable Else chk_buyable.Checked = False

                chk_tangible.Checked = entry_edit_data.IsTangible
                chk_disableInv.Enabled = Util_IsEditable()
                If Util_IsEditable() = False OrElse Not entry_edit_data.IsTangible Then
                    txt_height.Enabled = False
                    txt_length.Enabled = False
                    txt_width.Enabled = False
                    txt_weight.Enabled = False
                End If

                txt_height.Text = entry_edit_data.Dimensions.Height
                txt_length.Text = entry_edit_data.Dimensions.Length
                txt_width.Text = entry_edit_data.Dimensions.Width
                txt_weight.Text = entry_edit_data.Weight.Amount

                Dim inventoryApi As New InventoryApi()
                Dim inventoryData As InventoryData = inventoryApi.GetInventory(entry_edit_data.Id)

                chk_disableInv.Checked = entry_edit_data.DisableInventoryManagement
                chk_disableInv.Enabled = Util_IsEditable()
                If Util_IsEditable() = False OrElse entry_edit_data.DisableInventoryManagement Then
                    txt_instock.Enabled = False
                    txt_onorder.Enabled = False
                    txt_reorder.Enabled = False
                End If

                txt_instock.Text = inventoryData.UnitsInStock
                txt_onorder.Text = inventoryData.UnitsOnOrder
                txt_reorder.Text = inventoryData.ReorderLevel
            Else

                txt_height.Enabled = False
                txt_length.Enabled = False
                txt_width.Enabled = False
                txt_weight.Enabled = False

                txt_instock.Enabled = False
                txt_onorder.Enabled = False
                txt_reorder.Enabled = False

            End If

            Util_ToggleProperties(Util_IsEditable())

        End Sub

        Private Sub Display_PricingTab()

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

            Dim showPricingTier As Boolean = Me.ShowPricingTier()
            ltr_pricing.Text = Me.CommerceLibrary.GetPricingMarkup(entry_edit_data.Pricing, activeCurrencyList, exchangeRateList, entry_edit_data.EntryType, showPricingTier, Util_GetMode())

        End Sub

        Private Function ShowPricingTier() As Boolean
            Select Case entry_edit_data.EntryType
                Case CatalogEntryType.Bundle
                    Dim bundleData As BundleData = DirectCast(entry_edit_data, BundleData)
                    ShowPricingTier = IIf(bundleData.BundledItems.Count > 0, False, True)
                Case CatalogEntryType.ComplexProduct
                    Dim complexProductData As ProductData = DirectCast(entry_edit_data, ProductData)
                    ShowPricingTier = IIf(complexProductData.Variants.Count > 0, False, True)
                Case CatalogEntryType.Kit
                    Dim kitData As KitData = DirectCast(entry_edit_data, KitData)
                    ShowPricingTier = IIf(kitData.OptionGroups.Count > 0, False, True)
                Case CatalogEntryType.Product
                    Dim productData As ProductData = DirectCast(entry_edit_data, ProductData)
                    ShowPricingTier = IIf(productData.Variants.Count > 0, False, True)
                Case CatalogEntryType.SubscriptionProduct
                    ShowPricingTier = False
            End Select

            Return ShowPricingTier
        End Function

        Private Sub Display_ItemTab()

            If entry_edit_data IsNot Nothing Then
                Me.ucItem.EntryEditData = entry_edit_data
                Me.ucItem.ItemsFolderId = m_iFolder
                Me.ucItem.SubscriptionControlPath = Me.ApplicationPath + "/Commerce/CatalogEntry/Items/Subscriptions/Membership/Membership.ascx"
                If Util_IsEditable() = True Then
                    Me.ucItem.DisplayMode = Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Item.DisplayModeValue.Edit
                Else
                    Me.ucItem.DisplayMode = Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Item.DisplayModeValue.View
                End If
            End If
        End Sub

        Private Sub Display_MediaTab()

            ucMedia.EntryEditData = entry_edit_data
            ucMedia.ProductId = xid
            ucMedia.FolderId = m_iFolder
            If Util_IsEditable() = True Then
                ucMedia.DisplayMode = Workarea.CatalogEntry.Tabs.Medias.Media.DisplayModeValue.Edit
            Else
                ucMedia.DisplayMode = Workarea.CatalogEntry.Tabs.Medias.Media.DisplayModeValue.View
            End If
        End Sub

        Private Sub Display_MetadataTab()

            Dim sbAttrib As New StringBuilder
            Dim sbResult As New StringBuilder
            Dim strResult As String
            Dim strAttrResult As String
            Dim strImage As String = ""

            'Dim enhancedMetadataScript As New Literal
            'enhancedMetadataScript.Text = Replace(CustomFields.GetEnhancedMetadataScript(), "src=""java/", "src=""../java/")
            'Me.Page.Header.Controls.Add(enhancedMetadataScript)
            EnhancedMetadataArea.Text = CustomFields.GetEnhancedMetadataArea()
            If (Not meta_data Is Nothing) OrElse prod_type_data IsNot Nothing Then
                m_refSite = New Site.EkSite(Me.m_refContentApi.RequestInformationRef)
                Dim hPerm As Hashtable = m_refSite.GetPermissions(m_iFolder, 0, "folder")
                If meta_data IsNot Nothing Then sbResult = CustomFields.WriteFilteredMetadataForEdit(meta_data.ToArray(), False, m_sEditAction, m_iFolder, lValidCounter, hPerm)
                If prod_type_data IsNot Nothing Then
                    If Util_IsEditable() Then
                        sbAttrib = CustomFields.WriteFilteredAttributesForEdit(entry_edit_data.Attributes, m_sEditAction, xid, lValidCounter, hPerm)
                    Else
                        sbAttrib.Append(CustomFields.WriteFilteredAttributesForView(entry_edit_data.Attributes, xid, False))
                    End If
                End If
            End If
            If (m_sEditAction = "update") Then
                strImage = entry_edit_data.Image
                Dim strThumbnailPath As String = entry_edit_data.ImageThumbnail
                If (entry_edit_data.ImageThumbnail = "") Then
                    strThumbnailPath = m_refContentApi.AppImgPath & "spacer.gif"
                ElseIf (catalog_data.IsDomainFolder.ToString = True) Then
                    strThumbnailPath = strThumbnailPath
                Else
                    strThumbnailPath = m_refContentApi.SitePath & strThumbnailPath
                End If
                If (System.IO.Path.GetExtension(strThumbnailPath).ToLower().IndexOf(".gif") <> -1 AndAlso strThumbnailPath.ToLower().IndexOf("spacer.gif") = -1) Then
                    strThumbnailPath = strThumbnailPath.Replace(".gif", ".png")
                End If
                ' sbResult.Append("<fieldset><legend>Image Data:</legend><table><tr><td class=""info"" align=""left"">Image:</td><td><span id=""sitepath""" & Me.m_refContentApi.SitePath & "</span><input type=""textbox"" size=""30"" readonly=""true"" id=""entry_image"" name=""entry_image"" value=""" & strImage & """ /> <a href=""#"" onclick=""PopUpWindow('../mediamanager.aspx?scope=images&upload=true&retfield=entry_image&showthumb=false&autonav=" & catalog_data.Id & "', 'Meadiamanager', 790, 580, 1,1);return false;"">Change</a>&nbsp;<a href=""#"" onclick=""RemoveEntryImage('" & m_refContentApi.AppImgPath & "spacer.gif');return false"">Remove</a></td></tr><tr><td colomnspan=""2""><img id=""entry_image_thumb"" src=""" & strThumbnailPath & """ /></td></tr></table></fieldset>")
            Else
                ' sbResult.Append("<fieldset><legend>Image Data:</legend><table><tr><td class=""info"" align=""left"">Image:</td><td><span id=""sitepath""" & Me.m_refContentApi.SitePath & "</span><input type=""textbox"" size=""30"" readonly=""true"" id=""entry_image"" name=""entry_image"" value=""" & strImage & """ /> <a href=""#"" onclick=""PopUpWindow('../mediamanager.aspx?scope=images&upload=true&retfield=entry_image&showthumb=false&autonav=" & catalog_data.Id & "', 'Meadiamanager', 790, 580, 1,1);return false;"">Change</a>&nbsp;<a href=""#"" onclick=""RemoveEntryImage('" & m_refContentApi.AppImgPath & "spacer.gif');return false"">Remove</a></td></tr><tr><td colomnspan=""2""><img id=""entry_image_thumb"" src=""" & m_refContentApi.AppImgPath & "spacer.gif"" /></td></tr></table></fieldset>")
            End If

            If Me.m_sEditAction = "addlang" Then sbResult.Append("<input type=""hidden"" id=""entry_image"" name=""entry_image"" value=""" & entry_edit_data.Image & """ />")

            strAttrResult = sbAttrib.ToString().Trim()
            strAttrResult = Replace(strAttrResult, "src=""java/", "src=""../java/")
            strAttrResult = Replace(strAttrResult, "src=""images/", "src=""../images/")

            strResult = sbResult.ToString.Trim()
            strResult = Util_FixPath(strResult)
            strResult = Replace(strResult, "src=""java/", "src=""../java/")
            strResult = Replace(strResult, "src=""images/", "src=""../images/")

            ltr_meta.Text = strResult
            ltr_attrib.Text = strAttrResult
        End Sub

        Private Sub Display_ScheduleTab()

            Dim dateSchedule As EkDTSelector
            Dim end_date_action As Integer = 1
            Dim go_live As String = ""
            Dim end_date As String = ""

            If entry_edit_data IsNot Nothing Then
                go_live = entry_edit_data.GoLive
                If Not (entry_edit_data.EndDate = DateTime.MinValue Or entry_edit_data.EndDate = DateTime.MaxValue) Then
                    end_date = entry_edit_data.EndDate
                End If
                end_date_action = entry_edit_data.EndDateAction
            End If

            dateSchedule = Me.m_refContentApi.EkDTSelectorRef
            dateSchedule.formName = "frmMain"
            dateSchedule.extendedMeta = True
            ' start
            dateSchedule.formElement = "go_live"
            dateSchedule.spanId = "go_live_span"
            If (go_live <> "") Then
                dateSchedule.targetDate = CDate(go_live)
            End If
            ltr_startdatesel.Text = (dateSchedule.displayCultureDateTime(True))
            dateSchedule.formElement = "end_date"
            dateSchedule.spanId = "end_date_span"
            If (end_date <> "") Then
                dateSchedule.targetDate = CDate(end_date)
            Else
                dateSchedule.targetDate = Nothing
            End If
            ltr_enddatesel.Text = (dateSchedule.displayCultureDateTime(True))
            ' end
            ' action
            rblaction.Items.Add(New ListItem("Archive and remove from site (expire)", "1"))
            ' rblaction.Items.Add(New ListItem("Archive and remain on site", "2"))
            rblaction.Items.Add(New ListItem("Add to the CMS Refresh Report", "3"))
            ' action
            If Me.m_sEditAction = "add" Then
                rblaction.SelectedIndex = 0
            Else
                Select Case end_date_action
                    Case 3
                        rblaction.SelectedIndex = 1
                    Case Else
                        rblaction.SelectedIndex = 0
                End Select
            End If

        End Sub

        Private Sub Display_TaxonomyTab()

            If m_cPerms.CanEdit OrElse m_cPerms.CanAdd OrElse (m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator, m_refContentApi.RequestInformationRef.UserId)) Then
                TaxonomyRoleExists = True
            End If
            EditTaxonomyHtml.Text = "<p class=""info"">" & Me.m_refMsg.GetMessage("lbl select categories entry") & "</p><div id=""TreeOutput""></div>"
            lit_add_string.Text = m_refMsg.GetMessage("generic add title")

            Dim taxonomy_cat_arr As TaxonomyBaseData() = Nothing
            m_refContentApi.RequestInformationRef.ContentLanguage = ContentLanguage
            m_refContentApi.ContentLanguage = ContentLanguage

            Dim taxonomy_request As New TaxonomyRequest
            Dim taxonomy_data_arr As TaxonomyBaseData() = Nothing
            If m_sEditAction = "add" Then
                If (Request.QueryString("SelTaxonomyId") IsNot Nothing AndAlso Request.QueryString("SelTaxonomyId") <> "") Then
                    TaxonomySelectId = Convert.ToInt64(Request.QueryString("SelTaxonomyId"))
                End If
                If (TaxonomySelectId > 0) Then
                    taxonomyselectedtree.Value = TaxonomySelectId
                    TaxonomyTreeIdList = taxonomyselectedtree.Value
                    taxonomy_cat_arr = m_refContentApi.EkContentRef.GetTaxonomyRecursiveToParent(TaxonomySelectId, m_refContentApi.ContentLanguage, 0)
                    If (taxonomy_cat_arr IsNot Nothing AndAlso taxonomy_cat_arr.Length > 0) Then
                        For Each taxonomy_cat As TaxonomyBaseData In taxonomy_cat_arr
                            If (TaxonomyTreeParentIdList = "") Then
                                TaxonomyTreeParentIdList = Convert.ToString(taxonomy_cat.TaxonomyId)
                            Else
                                TaxonomyTreeParentIdList = TaxonomyTreeParentIdList & "," & Convert.ToString(taxonomy_cat.TaxonomyId)
                            End If
                        Next
                    End If
                End If
            Else
                taxonomy_cat_arr = m_refContentApi.EkContentRef.ReadAllAssignedCategory(m_iID)
                If (taxonomy_cat_arr IsNot Nothing AndAlso taxonomy_cat_arr.Length > 0) Then
                    For Each taxonomy_cat As TaxonomyBaseData In taxonomy_cat_arr
                        If (taxonomyselectedtree.Value = "") Then
                            taxonomyselectedtree.Value = Convert.ToString(taxonomy_cat.TaxonomyId)
                        Else
                            taxonomyselectedtree.Value = taxonomyselectedtree.Value & "," & Convert.ToString(taxonomy_cat.TaxonomyId)
                        End If
                    Next
                End If
                TaxonomyTreeIdList = taxonomyselectedtree.Value
                If (TaxonomyTreeIdList.Trim.Length > 0) Then
                    TaxonomyTreeParentIdList = m_refContentApi.EkContentRef.ReadDisableNodeList(m_iID)
                End If
            End If

            taxonomy_request.TaxonomyId = m_iFolder
            taxonomy_request.TaxonomyLanguage = m_refContentApi.ContentLanguage
            taxonomy_data_arr = m_refContentApi.EkContentRef.GetAllFolderTaxonomy(m_iFolder)

            If ((taxonomy_data_arr Is Nothing OrElse taxonomy_data_arr.Length = 0) AndAlso (TaxonomyOverrideId = 0)) Then
                ShowTaxonomyTab = False
            End If

            m_intTaxFolderId = m_iFolder
            'If (Request.QueryString("TaxonomyId") IsNot Nothing AndAlso Request.QueryString("TaxonomyId") <> "") Then
            '    TaxonomyOverrideId = Convert.ToInt32(Request.QueryString("TaxonomyId"))
            'End If

            'set CatalogEntry_Taxonomy_A_Js vars - see RegisterJS() and CatalogEntry.Taxonomy.A.aspx under CatalogEntry/js
            Me._JSTaxonomyFunctions_TaxonomyTreeIdList = Server.UrlEncode(TaxonomyTreeIdList)
            Me._JSTaxonomyFunctions_TaxonomyTreeParentIdList = Server.UrlEncode(TaxonomyTreeParentIdList)
            Me._JSTaxonomyFunctions_TaxonomyOverrideId = TaxonomyOverrideId
            Me._JSTaxonomyFunctions_TaxonomyFolderId = m_iFolder

        End Sub

        Private Sub Display_CommentTab()

            If entry_edit_data IsNot Nothing Then txt_comment.Text = entry_edit_data.Comment

        End Sub

        Private Sub Display_TemplateTab()



            Dim active_template_list As TemplateData() = m_refContentApi.GetEnabledTemplatesByFolder(catalog_data.Id)
            Dim default_template As Long = 0

            If (active_template_list.Length < 1) Then bSuppressTemplate = True
            If Me.m_sEditAction = "update" Then default_template = entry_edit_data.TemplateId
            If (default_template = 0) Then default_template = catalog_data.TemplateId

            drp_tempsel.DataValueField = "Id"
            drp_tempsel.DataTextField = "FileName"
            drp_tempsel.DataSource = active_template_list
            drp_tempsel.DataBind()

            For i As Integer = 0 To (active_template_list.Length - 1)

                If (active_template_list(i).Id = default_template) Then
                    drp_tempsel.SelectedIndex = i
                    Exit For
                End If

            Next

        End Sub

        Private Sub Display_AliasTab()

            Dim sbHtml As New StringBuilder
            Dim _manualAliasApi As New UrlAliasing.UrlAliasManualApi
            Dim _autoaliasApi As New UrlAliasing.UrlAliasAutoApi
            Dim aliasExtensions As System.Collections.Generic.List(Of String)
            Dim ext As String = ""
            Dim i As Integer

            Dim defaultManualAlias As New Ektron.Cms.Common.UrlAliasManualData(0, 0, String.Empty, String.Empty)
            Dim autoAliasList As New System.Collections.Generic.List(Of UrlAliasAutoData)

            aliasExtensions = _manualAliasApi.GetFileExtensions()
            If (Not IsNothing(entry_edit_data)) Then
                defaultManualAlias = _manualAliasApi.GetDefaultAlias(entry_edit_data.Id)
            End If
            sbHtml.Append("<div>")
            If (_urlAliasSettingApi.IsManualAliasEnabled) Then
                If m_refContentApi.IsARoleMember(Common.EkEnumeration.CmsRoleIds.EditAlias) Then
                    sbHtml.Append("<fieldset><legend><strong>" & m_refMsg.GetMessage("lbl tree url manual aliasing") & "</strong></legend>")
                    sbHtml.Append("<table width=""100%"" border=""0"" cellpadding=""2"" cellspacing=""2"">")
                    sbHtml.Append("<tr><td colspan=4>&nbsp;<br></td></tr>")
                    sbHtml.Append("<tr><td>&nbsp;</td><td class=""info"" nowrap=""true"">" & m_refMsg.GetMessage("lbl primary") & " " & m_refMsg.GetMessage("lbl alias name") & ":&nbsp;")
                    sbHtml.Append("<td>&nbsp;<input type=""hidden"" name=""frm_manalias_id"" value=""" & defaultManualAlias.AliasId & """></td>")
                    sbHtml.Append("<td>&nbsp;<input type=""hidden"" name=""prev_frm_manalias_name"" value=""" & defaultManualAlias.AliasName & """></td>")
                    sbHtml.Append("<td>&nbsp;<input type=""hidden"" name=""prev_frm_manalias_ext"" value=""" & defaultManualAlias.FileExtension & """></td>")
                    If (catalog_data.IsDomainFolder) Then
                        sbHtml.Append("<td width=""95%"">http://" & catalog_data.DomainProduction & "/<input type=""text""  size=""35"" name=""frm_manalias"" value=""" & defaultManualAlias.AliasName & """>")
                    Else
                        sbHtml.Append("<td width=""95%"">" & m_refContentApi.SitePath & "<input type=""text""  size=""35"" name=""frm_manalias"" value=""" & defaultManualAlias.AliasName & """>")
                    End If

                    For i = 0 To aliasExtensions.Count - 1
                        If (ext <> "") Then
                            ext = ext & ","
                        End If
                        ext = ext & aliasExtensions(i)
                    Next
                    sbHtml.Append(m_refContentApi.RenderHTML_RedirExtensionDD("frm_ManAliasExt", defaultManualAlias.FileExtension, ext))
                    sbHtml.Append("<br/></td>")
                    sbHtml.Append("</tr></table></fieldset>")
                    sbHtml.Append("<br/><br/><br/>")
                Else
                    sbHtml.Append("<input type=""hidden"" name=""frm_manalias_id"" value=""" & defaultManualAlias.AliasId & """>")
                    sbHtml.Append("<input type=""hidden"" name=""frm_manalias"" value=""" & defaultManualAlias.AliasName & defaultManualAlias.FileExtension & """>")
                End If

            End If
            If (_urlAliasSettingApi.IsAutoAliasEnabled) Then
                If (Not IsNothing(entry_edit_data)) Then
                    autoAliasList = _autoaliasApi.GetListForContent(entry_edit_data.Id)
                End If
                sbHtml.Append("<div class=""autoAlias"" style=""width: auto; height: auto; overflow: auto;"" id=""autoAliasList"">")
                sbHtml.Append("<fieldset><legend><strong>" & m_refMsg.GetMessage("lbl automatic") & "</strong></legend><br/>")
                sbHtml.Append("<table width=""100%"" border=""0"" cellpadding=""2"" cellspacing=""2"">")
                sbHtml.Append("<tr><td><u><strong>" & m_refMsg.GetMessage("generic type") & "</strong></u></td>")
                sbHtml.Append("<td><u><strong>" & m_refMsg.GetMessage("lbl alias name") & "</strong></u></td></tr>")
                For i = 0 To autoAliasList.Count() - 1
                    sbHtml.Append("<tr><td>" & autoAliasList(i).AutoAliasType.ToString() & "</td>")
                    sbHtml.Append("<td>" & autoAliasList(i).AliasName & "</td></tr>")
                Next
                sbHtml.Append("</table></fieldset></div>")
            End If
            sbHtml.Append("</div>")
            ltrEditAlias.Text = sbHtml.ToString
        End Sub

#End Region

#Region "Util"

        Private Sub Util_SetLabels()

            MyBase.Version8TabsImplemented = True
            'session expiration
            lbl_SessionExpiringLabel.Text = GetMessage("editor session expiring 10")
            lbl_ContinueEditingLabel.Text = GetMessage("continue editing")

            'set title
            lbl_GenericTitleLabel.Text = m_refMsg.GetMessage("generic title label")

            'content tab
            liContent.Visible = True
            divContent.Visible = True
            litTabContentLabel.Text = MyBase.m_refMsg.GetMessage("content text")

            'summary tab
            liSummary.Visible = True
            divSummary.Visible = True
            litTabSummaryLabel.Text = MyBase.m_refMsg.GetMessage("summary text")

            'properties tab
            liProperties.Visible = True
            divProperties.Visible = True
            litTabPropertiesLabel.Text = MyBase.m_refMsg.GetMessage("properties text")

            'comment tab - not implemented
            'liComment.Visible = True
            'divComment.Visible = True
            'litTabCommentLabel.Text = MyBase.m_refMsg.GetMessage("comment text")

            'pricing tab
            liPricing.Visible = True
            divPricing.Visible = True
            litTabPricingLabel.Text = MyBase.m_refMsg.GetMessage("lbl pricing")

            'attributes tab
            If prod_type_data.Attributes.Count > 0 Then
                liAttributes.Visible = True
                divAttributes.Visible = True
                litTabAttributesLabel.Text = MyBase.m_refMsg.GetMessage("lbl entry attrib tab")
            End If

            'items tab
            liItems.Visible = True
            divItems.Visible = True
            litTabItemsLabel.Text = MyBase.m_refMsg.GetMessage("lbl variants")

            'media tab
            liMedia.Visible = True
            divMedia.Visible = True
            litTabMediaLabel.Text = MyBase.m_refMsg.GetMessage("lbl media")

            'metadata tab
            liMetadata.Visible = True
            divMetadata.Visible = True
            litTabMetadataLabel.Text = MyBase.m_refMsg.GetMessage("metadata text")

            'schedule tab
            liSchedule.Visible = True
            divSchedule.Visible = True
            litTabScheduleLabel.Text = MyBase.m_refMsg.GetMessage("schedule text")

            'category tab
            If ShowTaxonomyTab Then
                liCategory.Visible = True
                divCategories.Visible = True
                litTabCateogoryLabel.Text = MyBase.m_refMsg.GetMessage("lbl category")
            End If

            'alias tab
            If ((_urlAliasSettingApi.IsManualAliasEnabled Or _urlAliasSettingApi.IsAutoAliasEnabled) _
                        And m_refContentApi.IsARoleMember(Common.EkEnumeration.CmsRoleIds.EditAlias)) Then
                liAlias.Visible = True
                divAlias.Visible = True
                litTabAliasLabel.Text = MyBase.m_refMsg.GetMessage("lbl alias")
            End If

            'templates tab - not implemented
            'If Not bSuppressTemplate Then
            'liTemplates.Visible = True
            'divTemplates.Visible = True
            'litTabTemplatesLabel.Text = MyBase.m_refMsg.GetMessage("template label")
            'End If

            Me.MenuCheckVariable = "checkVariable"
            Select Case Me.m_sEditAction
                Case "update"
                    Dim actionMenu As New workareamenu("action", Me.GetMessage("lbl action"), m_refContentApi.AppPath & "images/UI/Icons/check.png") ' check2.gif
                    actionMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/save.png", Me.GetMessage("btn save"), " SubmitForm(" & EkEnumeration.AssetActionType.Save & "); ")

                    Dim aURL As String = entry_edit_data.Quicklink & "&cmsMode=Preview"
                    actionMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/preview.png", Me.GetMessage("btn preview"), " PreviewContent('" & aURL & "', " & EkEnumeration.AssetActionType.Save & ", '" & entry_edit_data.Title & "'); return false; ")

                    actionMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/checkIn.png", Me.GetMessage("btn checkin"), " SubmitForm(" & EkEnumeration.AssetActionType.Checkin & "); ")
                    If UserRights.CanPublish Then
                        actionMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/contentPublish.png", Me.GetMessage("generic publish"), " SubmitForm(" & EkEnumeration.AssetActionType.Submit & "); ")
                    Else
                        actionMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/contentPublish.png", Me.GetMessage("btn submit"), " SubmitForm(" & EkEnumeration.AssetActionType.Submit & "); ")
                    End If
                    actionMenu.AddBreak()
                    actionMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/cancel.png", Me.GetMessage("generic undocheckout"), "SubmitForm(" & EkEnumeration.AssetActionType.UndoCheckout & "); ")
                    Me.AddMenu(actionMenu)

                    Dim miscMenu As New workareamenu("misc", Me.GetMessage("btn change"), Me.AppImgPath & "menu/product.gif") ' check2.gif
                    miscMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/comment.png", Me.GetMessage("comment text"), "ektb_show('" & GetMessage("comment text") & "','#EkTB_inline?height=200&width=500&inlineId=" + divComment.ClientID + "&modal=true', null, '');return false;")
                    If Not bSuppressTemplate Then miscMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/contentTemplate.png", Me.GetMessage("template label"), "ektb_show('" & Me.GetMessage("lbl template selection") & "','#EkTB_inline?height=200&width=500&inlineId=" + divTemplates.ClientID + "&modal=true', null, '');return false;")
                    Me.AddMenu(miscMenu)

                    Me.SetTitleBarToMessage("lbl edit catalog entry")
                    Me.AddHelpButton("editcatentry")
                Case "add", "addlang"
                    Dim actionMenu As New workareamenu("action", Me.GetMessage("lbl action"), m_refContentApi.AppPath & "images/UI/Icons/check.png") ' check2.gif
                    actionMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/checkIn.png", Me.GetMessage("btn checkin"), " SubmitForm(" & EkEnumeration.AssetActionType.Checkin & "); ")
                    If UserRights.CanPublish Then
                        actionMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/contentPublish.png", Me.GetMessage("generic publish"), " SubmitForm(" & EkEnumeration.AssetActionType.Submit & "); ")
                    Else
                        actionMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/contentPublish.png", Me.GetMessage("btn submit"), " SubmitForm(" & EkEnumeration.AssetActionType.Submit & "); ")
                    End If
                    actionMenu.AddBreak()
                    actionMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/cancel.png", Me.GetMessage("generic cancel"), "SubmitForm(" & EkEnumeration.AssetActionType.Cancel & "); ")
                    Me.AddMenu(actionMenu)

                    Dim miscMenu As New workareamenu("misc", Me.GetMessage("btn change"), m_refContentApi.AppImgPath & "menu/product.gif") ' check2.gif
                    miscMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/comment.png", Me.GetMessage("comment text"), "ektb_show('" & GetMessage("comment text") & "','#EkTB_inline?height=200&width=500&inlineId=" + divComment.ClientID + "&modal=true', null, '');return false;")
                    If Not bSuppressTemplate Then miscMenu.AddItem(m_refContentApi.AppPath & "Images/ui/icons/contentTemplate.png", Me.GetMessage("template label"), "ektb_show('" & Me.GetMessage("lbl template selection") & "','#EkTB_inline?height=200&width=500&inlineId=" + divTemplates.ClientID + "&modal=true', null, '');return false;")
                    Me.AddMenu(miscMenu)

                    Me.SetTitleBarToMessage("lbl add catalog entry")
                    Me.AddHelpButton("addcatentry")
            End Select

            'Dim tc As New TableCell()
            'Dim tr As New TableRow()
            'tc.Controls.Add(New LiteralControl(Util_GetTitleField()))
            'tr.Controls.Add(tc)
            'Me.AddTableRow(tr)

            ' labels
            ltr_sku.Text = GetMessage("lbl calatog entry sku")
            ltr_quantity.Text = GetMessage("lbl number of units")
            ltr_avail.Text = GetMessage("lbl archived")
            chk_avail.Attributes.Add("onclick", "ToggleAvail(this);")
            ' ltr_markdel.Text = GetMessage("lbl deleted")
            ltr_buyable.Text = GetMessage("lbl buyable")
            ltr_taxclass.Text = GetMessage("lbl tax class")
            ltr_tangible.Text = GetMessage("lbl tangible")
            chk_tangible.Attributes.Add("onclick", "ToggleTangible(this);")
            ltr_height.Text = GetMessage("lbl height")
            ltr_width.Text = GetMessage("lbl width")
            ltr_length.Text = GetMessage("lbl length")
            ' ltr_weightmeasure.Text = GetMessage("lbl weight measure")
            ltr_weight.Text = GetMessage("lbl weight")
            ltr_disableInv.Text = GetMessage("lbl disable inventory")
            ltr_instock.Text = GetMessage("lbl in stock")
            ltr_onorder.Text = GetMessage("lbl on order")
            ltr_reorder.Text = GetMessage("lbl reorder")
            ' ltr_currency.Text = GetMessage("lbl currency")
            ltr_comment.Text = GetMessage("comment text")
            'ltr_tempsel.Text = GetMessage("lbl template selection")
            ltr_actionend.Text = GetMessage("end date action title")
            ltr_startdate.Text = GetMessage("generic go live")
            ltr_enddate.Text = GetMessage("generic end date")
            ltr_ship.Text = GetMessage("lbl dimensions")
            ltr_inv.Text = GetMessage("lbl inventory")
            chk_disableInv.Attributes.Add("onclick", "ToggleInventory(this);")

            cmdCommentOk.Text = " " & GetMessage("lbl ok") & " "
            cmdCommentOk.Attributes.Add("onclick", "CloseEntryModal(); return false;")

            ltr_holdmsg.Text = m_refMsg.GetMessage("one moment msg")
        End Sub

        Protected Function Util_GetTitleField() As String

            Dim sbTitle As New StringBuilder()
            Dim objLocalizationApi As New LocalizationAPI()
            Dim language_data As LanguageData = m_refContentApi.EkSiteRef.GetLanguageDataById(ContentLanguage)

            sbTitle.Append(" <table border=""0"" cellpadding=""2"" cellspacing=""2""> ").Append(Environment.NewLine)
            sbTitle.Append("     <tr> ").Append(Environment.NewLine)
            sbTitle.Append("         <td>").Append(GetMessage("generic title")).Append(":</td> ").Append(Environment.NewLine)
            sbTitle.Append("         <td nowrap=""nowrap"" align=""left""> ").Append(Environment.NewLine)
            sbTitle.Append("             <input name=""content_title"" type=""text"" id=""content_title"" size=""50"" maxlength=""200"" onkeypress=""return CheckKeyValue(event, '34,13');"" value=""")
            If entry_edit_data IsNot Nothing Then sbTitle.Append(Server.HtmlEncode(entry_edit_data.Title))
            sbTitle.Append(""" /> [").Append(language_data.Name).Append("] ").Append(Environment.NewLine)
            sbTitle.Append("<img src='" & objLocalizationApi.GetFlagUrlByLanguageID(ContentLanguage) & "' border=""0"" />")
            sbTitle.Append("         </td> ").Append(Environment.NewLine)
            sbTitle.Append("         <td>&nbsp;</td> ").Append(Environment.NewLine)
            sbTitle.Append("     </tr> ").Append(Environment.NewLine)
            sbTitle.Append(" </table> ").Append(Environment.NewLine)

            Return sbTitle.ToString()

        End Function

        Protected Sub Util_CheckFolderType()
            catalog_data = m_refContentApi.GetFolderById(m_iFolder)
            If catalog_data.FolderType <> FolderType.Catalog Then Throw New Exception("Not a catalog")
            _stylesheet = m_refContentApi.GetStyleSheetByFolderID(catalog_data.Id)
            _stylesheetPath = Util_GetServerPath() & m_refContentApi.SitePath & _stylesheet
        End Sub

        Protected Sub Util_CheckAccess()

        End Sub

        Protected Sub Util_ObtainValues()
            If Request.QueryString("back_LangType") <> "" Then backLangType = Request.QueryString("back_LangType")
            If Request.QueryString("content_id") <> "" Then otherlangid = Request.QueryString("content_id")
            If Request.QueryString("type") <> "" Then
                m_sEditAction = Request.QueryString("type")
            End If
            If (Not (Request.QueryString("folder_id") Is Nothing)) Then ' add
                m_iFolder = Convert.ToInt64(Request.QueryString("folder_id"))
            End If
            If (Not (Request.QueryString("back_folder_id") Is Nothing)) Then ' edit
                m_iFolder = Convert.ToInt64(Request.QueryString("back_folder_id"))
            End If
            If Request.QueryString("xid") <> "" Then
                xid = Request.QueryString("xid")
                If xid > 0 Then
                    m_refProductType = New ProductType(m_refContentApi.RequestInformationRef)
                    prod_type_data = m_refProductType.GetItem(xid, True)
                    Me.editorPackage = prod_type_data.PackageXslt
                    hdn_entrytype.Value = prod_type_data.EntryClass
                End If
            End If
            hdn_xmlid.Value = xid

            m_cPerms = m_refContentApi.LoadPermissions(m_iFolder, "folder")

            If Request.QueryString("incontext") <> "" Then _inContextEditing = Convert.ToBoolean(Request.QueryString("incontext"))

            'm_mMeasures = New Measurements(m_refContentApi.RequestInformationRef).GetMeasurements()
        End Sub

        Private Sub Util_SetJS()
            Dim id As String = Me.m_iID
            Dim taxonomyRequired As Boolean = False
            If catalog_data.CategoryRequired = True AndAlso m_refContent.GetAllFolderTaxonomy(catalog_data.Id).Length > 0 Then
                taxonomyRequired = True
            End If

            'set CatalogEntry_Taxonomy_B_Js vars - see RegisterJS() and CatalogEntry.Taxonomy.B.aspx under CatalogEntry/js
            Me._JSTaxonomyFunctions_ShowTaxonomy = TaxonomyRoleExists.ToString()
            Me._JSTaxonomyFunctions_FolderId = m_intTaxFolderId.ToString()
        End Sub

        Private Function Util_FixPath(ByVal MetaScript As String) As String
            Dim iTmp As Integer = -1
            iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", 0)
            While iTmp > -1
                iTmp = MetaScript.IndexOf(");return false;", iTmp)
                MetaScript = MetaScript.Insert(iTmp, ", '" & Me.m_refContentApi.ApplicationPath & "'")
                iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", iTmp + 1)
            End While
            Return MetaScript
        End Function

        Public Sub Util_BindFieldList()
            If prod_type_data IsNot Nothing Then
                Dim xDoc As New System.Xml.XmlDocument()
                Dim xList As System.Xml.XmlNodeList
                If prod_type_data.FieldList <> "" Then
                    xDoc.LoadXml(prod_type_data.FieldList)
                    xList = xDoc.SelectNodes("/fieldlist/field/@xpath")
                    For i As Integer = 0 To (xList.Count - 1)
                        drp_field.Items.Add(xList(i).Value)
                        drp_field2.Items.Add(xList(i).Value)
                    Next
                End If
            End If
            chk_field.Visible = False
            drp_field.Visible = False
            chk_field2.Visible = False
            drp_field2.Visible = False
        End Sub

        Public Sub Util_GetEntryType()
            Select Case prod_type_data.EntryClass

                Case CatalogEntryType.SubscriptionProduct

                    entry_edit_data = New SubscriptionProductData
                    entry_edit_data.EntryType = CatalogEntryType.SubscriptionProduct

                Case CatalogEntryType.Bundle
                    entry_edit_data = New BundleData
                    entry_edit_data.EntryType = CatalogEntryType.Bundle
                Case CatalogEntryType.Kit
                    entry_edit_data = New KitData
                    entry_edit_data.EntryType = CatalogEntryType.Kit
                Case Else
                    entry_edit_data = New ProductData()
                    entry_edit_data.EntryType = CatalogEntryType.Product
            End Select
        End Sub

        Private Function GetLocaleFileString(ByVal localeFileNumber As String) As String
            Dim LocaleFileString As String
            If (CStr(localeFileNumber) = "" Or CInt(localeFileNumber) = 1) Then
                LocaleFileString = "0000"
            Else
                LocaleFileString = New String("0", 4 - Len(Hex(localeFileNumber)))
                LocaleFileString = LocaleFileString & Hex(localeFileNumber)
                If Not System.IO.File.Exists(Server.MapPath(m_refContentApi.AppeWebPath & "locale" & LocaleFileString & "b.xml")) Then
                    LocaleFileString = "0000"
                End If
            End If
            Return LocaleFileString.ToString
        End Function

        Private Function Util_GetMode() As workareaCommerce.ModeType

            Dim mode As workareaCommerce.ModeType = workareaCommerce.ModeType.Edit

            If Not Util_IsEditable() Then

                mode = workareaCommerce.ModeType.View

            ElseIf m_sEditAction = "add" Or m_sEditAction = "addlang" Then

                mode = workareaCommerce.ModeType.Add

            End If

            Return mode

        End Function

        Private Function Util_IsEditable() As Boolean

            Dim editable As Boolean = True

            If m_sEditAction = "addlang" Then

                editable = False

            ElseIf entry_edit_data IsNot Nothing Then

                editable = (entry_edit_data.StatusLanguage = 0 OrElse (entry_edit_data.StatusLanguage = ContentLanguage))

            End If

            Return editable

        End Function

        Private Sub Util_ToggleProperties(ByVal editable As Boolean)

            txt_sku.Enabled = editable
            txt_quantity.Enabled = editable

            drp_taxclass.Enabled = editable

            chk_avail.Enabled = editable
            ' chk_markdel.Enabled = editable
            chk_buyable.Enabled = editable

            'txt_height.Enabled = editable
            'txt_width.Enabled = editable
            'txt_length.Enabled = editable
            'txt_weight.Enabled = editable

            'txt_instock.Enabled = editable
            'txt_onorder.Enabled = editable
            'txt_reorder.Enabled = editable
            chk_tangible.Enabled = editable

        End Sub

        Private Sub Util_ResponseHandler(ByVal redirectUrl As String)

            If _inContextEditing Then

                Page.ClientScript.RegisterStartupScript(GetType(Page), "ReloadAndClose", "opener.location.href = opener.location; self.close();", True)

            Else

                Response.Redirect(redirectUrl, False)

            End If

        End Sub

        Private Function Util_GetServerPath() As String

            Dim strPath As String

            If Request.ServerVariables("SERVER_PORT_SECURE") = "1" Then
                strPath = "https://" & Request.ServerVariables("SERVER_NAME")
                If Request.ServerVariables("SERVER_PORT") <> "443" Then
                    strPath = strPath & ":" & Request.ServerVariables("SERVER_PORT")
                End If
            Else
                strPath = "http://" & Request.ServerVariables("SERVER_NAME")
                If Request.ServerVariables("SERVER_PORT") <> "80" Then
                    strPath = strPath & ":" & Request.ServerVariables("SERVER_PORT")
                End If
            End If

            Return strPath

        End Function

#End Region

#Region "Css, Js"

        Private Sub RegisterCss()
            Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
            Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
            Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronModalCss)
            Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
            Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/csslib/box.css", "EktronBoxCss")
            Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/csslib/tables/tableutil.css", "EktronTableUtilCss")
            Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/csslib/commerce/Ektron.Commerce.Pricing.css", "EktronCommercePricingCss")
            Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/csslib/commerce/Ektron.Commerce.Pricing.ie6.css", "EktronCommercePricingCss", API.Css.BrowserTarget.IE6)
            Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/Commerce/CatalogEntry/css/CatalogEntry.css", "EktronCommerceCatalogEntryCss")
            Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/Tree/css/com.ektron.ui.tree.css", "EktronTreeCss")

            Ektron.Cms.API.Css.RegisterCss(Me, Me.ApplicationPath & "/csslib/commerce/Ektron.Commerce.Session.css", "EktronCommerceSessionCss")

        End Sub

        Private Sub RegisterJs()
            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronThickBoxJS)
            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJsonJS)
            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronModalJS)
            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronDnRJS)
            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/internCalendarDisplayFuncs.js", "EktronInternalCalendarDisplayJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/searchfuncsupport.js", "EktronSearchFunctionSupportJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/dhtml/tableutil.js", "EktronTableUtilitiesJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/metadata_selectlist.js", "EktronMetadataSelectListJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/toolbar_roll.js", "EktronToolbarRollJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/commerce/com.Ektron.Commerce.Pricing.js", "EktronPricingJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/ContentDesigner/EkRadEditor.js", "EktronContentDesignerJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/java/metadata_associations.js", "Ektron_Metadata_Association.js")

            If Request.IsSecureConnection AndAlso (Session("ecmComplianceRequired") IsNot Nothing AndAlso Session("ecmComplianceRequired") = True) Then
                Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath & "java/commerce/com.ektron.commerce.session.js", "EktronSessionJs")
                Page.ClientScript.RegisterStartupScript(Me.GetType(), "Session", "timeoutWarning=setTimeout(showWarning, timeoutPeriod * 60000);", True)
            End If

            'Tree Js
            If entry_edit_data IsNot Nothing Then
                Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Commerce/CatalogEntry/js/CatalogEntry.PageFunctions.aspx?id=" & _JsPageFunctions_ContentEditorId & "&entrytype=" & entry_edit_data.EntryType & "&folder_id=" & Me.m_iFolder & "&taxonomyRequired=" & catalog_data.CategoryRequired, "Ektron_CatalogEntry_PageFunctions_Js")
            Else
                Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Commerce/CatalogEntry/js/CatalogEntry.PageFunctions.aspx?id=" & _JsPageFunctions_ContentEditorId & "&entrytype=" & Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product & "&folder_id=" & Me.m_iFolder, "Ektron_CatalogEntry_PageFunctions_Js")
            End If
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.A.aspx?folderId=" & _JSTaxonomyFunctions_FolderId & "&taxonomyOverrideId=" & _JSTaxonomyFunctions_TaxonomyOverrideId & "&taxonomyTreeIdList=" & _JSTaxonomyFunctions_TaxonomyTreeIdList & "&taxonomyTreeParentIdList=" & _JSTaxonomyFunctions_TaxonomyTreeParentIdList, "Ektron_CatalogEntry_Taxonomy_A_Js")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.B.aspx?showTaxonomy=" & _JSTaxonomyFunctions_ShowTaxonomy & "&taxonomyFolderId=" & _JSTaxonomyFunctions_TaxonomyFolderId, "Ektron_CatalogEntry_Taxonomy_B_Js")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.utils.url.js", "EktronTreeUtilsUrlJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.explorer.init.js", "EktronTreeExplorerInitJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.explorer.js", "EktronTreeExplorerJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.explorer.config.js", "EktronTreeExplorerConfigJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.explorer.windows.js", "EktronTreeExplorerWindowsJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.cms.types.js", "EktronTreeCmsTypesJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.cms.parser.js", "EktronTreeCmsParserJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.cms.toolkit.js", "EktronTreeCmsToolkitJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.cms.api.js", "EktronTreeCmsApiJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.ui.contextmenu.js", "EktronTreeUiContextMenuJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.ui.iconlist.js", "EktronTreeUiIconListJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.ui.tabs.js", "EktronTreeUiTabsJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.ui.explore.js", "EktronTreeUiExploreJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.ui.taxonomytree.js", "EktronTreeUiTaxonomyTreeJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.net.http.js", "EktronTreeNetHttpJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.lang.exception.js", "EktronTreeLanguageExceptionJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.utils.form.js", "EktronTreeUtilsFormJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.utils.log.js", "EktronTreeUtilsLogJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.utils.dom.js", "EktronTreeUtilsDomJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.utils.debug.js", "EktronTreeUtilsDebugJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.utils.string.js", "EktronTreeUtilsStringJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.utils.cookie.js", "EktronTreeUtilsCookieJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Tree/js/com.ektron.utils.querystring.js", "EktronTreeUtilsQuerystringJs")
            Ektron.Cms.API.JS.RegisterJS(Me, Me.ApplicationPath & "/Commerce/CatalogEntry/js/CatalogEntry.Taxonomy.C.js", "EktronCatalogEntryTaxonomyCJs")
        End Sub

#End Region

    End Class

End Namespace