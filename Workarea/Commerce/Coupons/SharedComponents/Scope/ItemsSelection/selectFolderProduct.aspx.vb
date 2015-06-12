Imports System
Imports System.Data
Imports System.Configuration
Imports System.Collections
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Collections.Specialized
Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Workarea
Imports Ektron.Cms.Commerce.Workarea
Imports Ektron.Cms.API
Imports Ektron.Cms.Commerce.Workarea.Coupons
Imports Ektron.Cms.Common.EkEnumeration

Namespace Ektron.Cms.Commerce.Workarea.Coupons
    Partial Class SelectFolderProduct
        Inherits workareabase

#Region "Enumerations"

        Private Enum Mode
            Folder
            Product
        End Enum

#End Region

#Region "Variables"

        Private _FolderApi As Ektron.Cms.API.Folder
        Private _IsProductSelected As Boolean
        Private _IsFolderSelected As Boolean
        Private _Mode As Mode
        Private _FolderId As Long
        Private _SelectedFolderIds() As String
        Private _SelectedFolderList As New List(Of Long)
        Private _SelectedProductIds() As String
        Private _SelectedProductList As New List(Of Long)

#End Region

#Region "Page Functions"

        Public Sub New()
            _FolderApi = New Ektron.Cms.API.Folder()
            _FolderId = 0
            _IsProductSelected = False
            _IsFolderSelected = False
            _Mode = Mode.Folder
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Try

                'ensure commerce permissions
                CheckAccess()
                CommerceLibrary.CheckCommerceAdminAccess()

                'set page not to cache
                System.Web.HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache")
                System.Web.HttpContext.Current.Response.Expires = 0
                System.Web.HttpContext.Current.Response.Cache.SetNoStore()
                System.Web.HttpContext.Current.Response.AddHeader("Pragma", "no-cache")

                Dim defaultId() As String = {""}
                If (Me.IsPostBack) Then
                    ' recover previously checked items as we traverse tree
                    _SelectedFolderIds = IIf(Request.Form("hdnFolderList") = Nothing, defaultId, Split(Request.Form("hdnFolderList"), ","))
                    _SelectedProductIds = IIf(Request.Form("hdnProductList") = Nothing, defaultId, Split(Request.Form("hdnProductList"), ","))
                Else
                    ' first time load - initialize
                    If (Request.QueryString("mode") = "catalog") Then
                        InitDataForFolders()
                    Else
                        InitDataForProducts()
                    End If
                End If

                If Request.Form("hdnFolderId") = Nothing Then
                    _FolderId = 0
                Else
                    _FolderId = Long.Parse(Request.Form("hdnFolderId"))
                End If

                _Mode = IIf(Request.QueryString("mode") = "catalog", Mode.Folder, Mode.Product)

                'register page components
                Me.RegisterJs()
                Me.RegisterCss()

            Catch ex As Exception

                Utilities.ShowError(ex.Message)

            End Try

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try

                'get and display folders
                GetFolders()
                DisplayFolders()

                'get and display products if in product mode
                If _Mode = Mode.Product Then
                    GetProducts()
                    DisplayProducts()
                End If
            Catch ex As Exception
                Utilities.ShowError(ex.Message)
            End Try
        End Sub

#End Region

#Region "Folder"

        Protected Sub GetFolders()
            Dim folderIdLong As Long
            Dim convertsToLong As Boolean = False

            If (Not IsNothing(Me._SelectedFolderIds)) Then
                For Each folderIdString As String In Me._SelectedFolderIds
                    convertsToLong = Long.TryParse(folderIdString, folderIdLong)
                    If convertsToLong Then
                        Me._SelectedFolderList.Add(folderIdLong)
                    End If
                Next
            End If
        End Sub

        Private Sub DisplayFolders()
            Me.uxPaging.Visible = False

            'get folderdata for selected folder
            Dim curentFolderData As FolderData = m_refContentApi.GetFolderById(_FolderId)

            'prepare databind data
            '(1) populate subfolder folderdata array with select folders's child folder's folderdata
            Dim subFolders() As FolderData = m_refContentApi.GetChildFolders(_FolderId, False, FolderOrderBy.Name)

            '(2) if currenct folder is not zero (root), update databind data
            'add "go back" node as topmost/index-zero/first node in array
            If curentFolderData.Id > 0 Then
                Dim goBack As New FolderData()
                Dim folderList As New List(Of FolderData)()

                goBack.Id = curentFolderData.ParentId
                goBack.Name = "..."

                If subFolders IsNot Nothing Then folderList.AddRange(subFolders)
                folderList.Insert(0, goBack)

                subFolders = folderList.ToArray()
            End If

            '(3) databind to subfolders folderdata array
            gvFolders.DataSource = subFolders
            gvFolders.DataBind()

            'set label with current folder's name
            litCurrentFolderName.Text = "Current Folder: " & curentFolderData.Name

        End Sub

        Protected Sub gvFolders_OnRowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvFolders.RowDataBound

            Dim folderData As Ektron.Cms.FolderData
            Dim path As String
            Dim idControl As HtmlInputControl
            Dim nameControl As HtmlInputControl
            Dim pathControl As HtmlInputControl
            Dim checkboxControl As HtmlInputCheckBox
            Dim imageControl As HtmlImage
            Dim linkControl As HtmlAnchor

            folderData = TryCast(e.Row.DataItem, Ektron.Cms.FolderData)

            If folderData IsNot Nothing Then
                'set "go up one level" tooltip text
                Dim title As String = IIf(folderData.Name = "...", Me.GetGoUpLevelMessage(), folderData.Name)

                idControl = TryCast(e.Row.FindControl("hdnFolderId"), HtmlInputControl)
                If idControl IsNot Nothing Then
                    idControl.Value = folderData.Id
                End If

                nameControl = TryCast(e.Row.FindControl("hdnFolderName"), HtmlInputControl)
                If nameControl IsNot Nothing Then
                    nameControl.Value = folderData.Name
                End If

                path = _FolderApi.GetPath(folderData.Id)
                pathControl = TryCast(e.Row.FindControl("hdnFolderPath"), HtmlInputControl)
                If pathControl IsNot Nothing Then
                    pathControl.Value = path
                End If

                imageControl = TryCast(e.Row.FindControl("imgFolderIcon"), HtmlImage)
                If imageControl IsNot Nothing Then
                    imageControl.Src = GetFolderIcon(folderData.Id, folderData.FolderType, folderData.ParentId)
                    imageControl.Alt = folderData.Name
                    imageControl.Attributes.Add("title", Title)
                End If

                checkboxControl = TryCast(e.Row.FindControl("cbxFolder"), HtmlInputCheckBox)
                If checkboxControl IsNot Nothing Then
                    checkboxControl.Disabled = IIf(IsFolderDisabled(folderData.FolderType) = True, True, False)
                    If (IsFolderDisabled(folderData.FolderType)) Then
                        checkboxControl.Style("display") = "none"
                    End If
                    checkboxControl.Checked = IIf(IsFolderSelected(folderData.Id) = True, True, False)
                End If

                linkControl = TryCast(e.Row.FindControl("aFolder"), HtmlAnchor)
                If linkControl IsNot Nothing Then
                    linkControl.Attributes.Add("onclick", GetFolderOnclick(folderData.Id))
                    linkControl.Attributes.Add("class", GetFolderCssClass(folderData.FolderType))
                    linkControl.Title = title
                    linkControl.InnerText = folderData.Name
                End If
            End If
        End Sub

        Protected Function GetFolderOnclick(ByVal folderId As Long) As String
            If Me._IsProductSelected Then
                Return "Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Actions.Click.item(this, " & folderId.ToString() & ", 0);return false;"
            Else
                Return "Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Actions.Click.item(this, " & folderId.ToString() & ", 1);return false;"
            End If
        End Function

        Protected Function GetFolderCssClass(ByVal folderType As EkEnumeration.FolderType) As String
            Dim CssClass As String = String.Empty
            If folderType = EkEnumeration.FolderType.Catalog Then
                CssClass = "catalogFolder"
            End If
            Return CssClass
        End Function

        Protected Function IsFolderDisabled(ByVal folderType As EkEnumeration.FolderType) As String
            Dim disabled As Boolean = True
            If folderType = EkEnumeration.FolderType.Catalog And _Mode = Mode.Folder Then
                disabled = False
            End If
            Return disabled
        End Function

        Protected Function IsFolderSelected(ByVal folderId As Long) As Boolean
            Return _SelectedFolderList.Contains(folderId)
        End Function

        Protected Function GetFolderIcon(ByVal folderId As Long, ByVal folderType As EkEnumeration.FolderType, ByVal parentId As Long) As String
            If folderId = parentId Then
                Return GetAppImgPath() & "folderbackup_1.gif"
            Else
                Select Case folderType
                    Case EkEnumeration.FolderType.DiscussionBoard
                        Return GetAppImgPath() & "menu/users2.gif"
                    Case EkEnumeration.FolderType.Blog
                        Return GetAppImgPath() & "menu/pen_blue.gif"
                    Case EkEnumeration.FolderType.Community
                        Return GetAppImgPath() & "menu/house2.gif"
                    Case EkEnumeration.FolderType.Catalog
                        Return GetAppImgPath() & "commerce/catalogclosed_1.gif"
                    Case Else
                        Return GetAppImgPath() & "folderclosed_1.gif"
                End Select
            End If
        End Function

#End Region

#Region "Products"

        Protected Sub GetProducts()
            Dim productIdLong As Long
            Dim convertsToLong As Boolean = False

            If (Not IsNothing(Me._SelectedProductIds)) Then
                For Each productIdString As String In Me._SelectedProductIds
                    convertsToLong = Long.TryParse(productIdString, productIdLong)
                    If convertsToLong Then
                        Me._SelectedProductList.Add(productIdLong)
                    End If
                Next
            End If
        End Sub

        Private Sub DisplayProducts()

            Dim CatalogManager As New CatalogEntryApi()
            Dim entryList As New System.Collections.Generic.List(Of EntryData)()
            Dim entryCriteria As New Ektron.Cms.Common.Criteria(Of EntryProperty)

            entryCriteria.PagingInfo.RecordsPerPage = Me._FolderApi.EkContentRef.RequestInformation.PagingSize
            entryCriteria.PagingInfo.CurrentPage = Me.uxPaging.SelectedPage + 1

            entryCriteria.AddFilter(EntryProperty.CatalogId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, Me._FolderId)
            entryCriteria.AddFilter(EntryProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, Long.Parse(Request.QueryString("languageId")))
            entryList = CatalogManager.GetList(entryCriteria)

            If entryCriteria.PagingInfo.TotalPages > 0 Then
                Me.uxPaging.Visible = True
                Me.uxPaging.TotalPages = entryCriteria.PagingInfo.TotalPages
                Me.uxPaging.CurrentPageIndex = Me.uxPaging.SelectedPage
            Else
                Me.uxPaging.Visible = False
            End If

            gvProducts.DataSource = entryList
            gvProducts.DataBind()

        End Sub

        Protected Sub gvProducts_OnRowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvProducts.RowDataBound

            Dim entryData As Ektron.Cms.Commerce.EntryData
            Dim path As String
            Dim idControl As HtmlInputControl
            Dim nameControl As HtmlInputControl
            Dim pathControl As HtmlInputControl
            Dim subTypeControl As HtmlInputControl
            Dim checkboxControl As HtmlInputCheckBox
            Dim imageControl As HtmlImage
            Dim titleControl As HtmlGenericControl

            entryData = TryCast(e.Row.DataItem, Ektron.Cms.Commerce.EntryData)
            If entryData IsNot Nothing Then
                idControl = TryCast(e.Row.FindControl("hdnProductId"), HtmlInputControl)
                If idControl IsNot Nothing Then
                    idControl.Value = entryData.Id
                End If

                nameControl = TryCast(e.Row.FindControl("hdnProductName"), HtmlInputControl)
                If nameControl IsNot Nothing Then
                    nameControl.Value = entryData.Title
                End If

                path = _FolderApi.GetPath(entryData.FolderId)
                pathControl = TryCast(e.Row.FindControl("hdnProductPath"), HtmlInputControl)
                If pathControl IsNot Nothing Then
                    pathControl.Value = path
                End If

                subTypeControl = TryCast(e.Row.FindControl("hdnProductSubType"), HtmlInputControl)
                If subTypeControl IsNot Nothing Then
                    subTypeControl.Value = Me.GetProductTypeName(entryData.EntryType)
                End If

                checkboxControl = TryCast(e.Row.FindControl("cbxProduct"), HtmlInputCheckBox)
                If checkboxControl IsNot Nothing Then
                    checkboxControl.Checked = IIf(IsProductSelected(entryData.Id) = True, True, False)
                End If

                imageControl = TryCast(e.Row.FindControl("imgProduct"), HtmlImage)
                If imageControl IsNot Nothing Then
                    imageControl.Src = GetProductIcon(entryData.EntryType)
                    imageControl.Alt = entryData.Title
                    imageControl.Attributes.Add("title", entryData.Title)
                End If

                titleControl = TryCast(e.Row.FindControl("spanProduct"), HtmlGenericControl)
                If titleControl IsNot Nothing Then
                    titleControl.InnerText = entryData.Title
                End If
            End If
        End Sub

        Protected Function IsProductSelected(ByVal entryId As Long) As Boolean
            Return _SelectedProductList.Contains(entryId)
        End Function

        Protected Function GetProductIcon(ByVal entryType As EkEnumeration.CatalogEntryType) As String
            Dim productImage As String
            Select Case entryType
                Case Common.EkEnumeration.CatalogEntryType.Bundle
                    productImage = m_refContentApi.ApplicationPath & "images/ui/icons/package.png"
                Case Common.EkEnumeration.CatalogEntryType.ComplexProduct
                    productImage = m_refContentApi.ApplicationPath & "images/ui/icons/bricks.png"
                Case Common.EkEnumeration.CatalogEntryType.Kit
                    productImage = m_refContentApi.ApplicationPath & "images/ui/icons/box.png"
                Case Common.EkEnumeration.CatalogEntryType.SubscriptionProduct
                    productImage = m_refContentApi.ApplicationPath & "images/ui/icons/bookGreen.png"
                Case Else
                    productImage = m_refContentApi.ApplicationPath & "images/ui/icons/brick.png"
            End Select

            Return productImage
        End Function

#End Region

#Region "Helpers"

        Protected Sub CheckAccess()
            If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
                Throw New Exception(GetMessage("feature locked error"))
            End If

        End Sub

        Protected Function GetAppImgPath() As String
            Return m_refContentApi.AppImgPath
        End Function


        Protected Sub InitDataForFolders()
            hdnFolderList.Value = Request.QueryString("idlist")
            _SelectedFolderIds = Request.QueryString("idlist").split(",")

            Dim json As String = ""
            Dim folderIdLong As Long
            Dim fd As FolderData

            If (Not IsNothing(Me._SelectedFolderIds)) Then
                For Each folderIdString As String In Me._SelectedFolderIds
                    If Long.TryParse(folderIdString, folderIdLong) Then
                        fd = m_refContentApi.GetFolderById(folderIdLong)
                        json += IIf(json.Length > 0, ",", "[")
                        json += "{""Id"":""" + fd.Id.ToString() _
                        + """,""Name"":""" + fd.Name _
                        + """,""Path"":""" + fd.NameWithPath.TrimEnd("/").Replace("/", "\\") _
                        + """,""Type"":""catalog"",""SubType"":""catalog"",""TypeCode"":""1"",""MarkedForDelete"":""false"",""NewlyAdded"":""false""}"
                    End If
                Next
            End If

            json += IIf(json.Length > 0, "]", "")
            hdnData.Value = json
        End Sub

        Protected Sub InitDataForProducts()
            hdnProductList.Value = Request.QueryString("idlist")
            _SelectedProductIds = Request.QueryString("idlist").split(",")

            Dim json As String = ""
            Dim contentIdLong As Long
            Dim cd As ContentData
            Dim entryData As EntryData
            Dim catalogEntryApi As New Commerce.CatalogEntryApi()
            Dim origLang As Integer = m_refContentApi.ContentLanguage

            Try
            If (Not IsNothing(Me._SelectedProductIds)) Then
                    m_refContentApi.ContentLanguage = Utilities.GetLanguageId(m_refContentApi)
                For Each contentIdString As String In Me._SelectedProductIds
                    If Long.TryParse(contentIdString, contentIdLong) Then
                        cd = m_refContentApi.GetContentById(contentIdLong, ContentAPI.ContentResultType.Published)
                        entryData = catalogEntryApi.GetItem(contentIdLong)
                        json += IIf(json.Length > 0, ",", "[")
                        json += "{""Id"":""" + cd.Id.ToString() _
                        + """,""Name"":""" + cd.Title _
                        + """,""Path"":""" + cd.Path.TrimEnd("/").Replace("\", "\\").Replace("/", "\\") _
                        + """,""Type"":""product"",""SubType"":""" + GetProductTypeName(entryData.EntryType) + """,""TypeCode"":""0"",""MarkedForDelete"":""false"",""NewlyAdded"":""false""}"
                    End If
                Next
            End If

            Finally
                m_refContentApi.ContentLanguage = origLang
            End Try

            json += IIf(json.Length > 0, "]", "")
            hdnData.Value = json
        End Sub

#End Region

#Region "Localized Strings"

        Public Function GetLocalizedJavascriptStrings() As String
            Dim selectedFolderMessage As String
            Dim selectedProductMessage As String

            selectedFolderMessage = "You have selected this folder and all its decendants.  To select among this folder&#39;s descendants, you must first unselect this folder."
            selectedProductMessage = "This item has no children."

            Return "{""selectedFolderClickMessage"": """ & selectedFolderMessage & """, ""selectedProductClickMessage"": """ & selectedProductMessage & """}"
        End Function

        Public Function GetGoUpLevelMessage() As String
            Return "Go up one level"
        End Function

        Public Function GetProductTypeName(ByVal subType As EkEnumeration.CatalogEntryType) As String
            Dim localizedType As String = ""
            Select Case subType
                Case CatalogEntryType.Bundle
                    localizedType = "Bundle"
                Case CatalogEntryType.ComplexProduct
                    localizedType = "Complex Product"
                Case CatalogEntryType.Kit
                    localizedType = "Kit"
                Case CatalogEntryType.Product
                    localizedType = "Product"
                Case CatalogEntryType.SubscriptionProduct
                    localizedType = "Subscription"
            End Select
            Return localizedType
        End Function
#End Region

#Region "Css, Js"

        Private Sub RegisterCss()
            Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.ApplicationPath.TrimEnd("/") & "/Commerce/Coupons/SharedComponents/Scope/ItemsSelection/css/selectFolderProduct.css", "EktronCommerceCouponsScopeItemsSelectFolderProductCss")
        End Sub

        Private Sub RegisterJs()
            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJsonJS)
            Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.ApplicationPath.TrimEnd("/") & "/Commerce/Coupons/SharedComponents/Scope/ItemsSelection/js/selectFolderProduct.js", "EktronCommerceCouponsScopeItemsSelectFolderProductJs")
        End Sub

#End Region

    End Class

End Namespace