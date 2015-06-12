
Imports Ektron.Cms
Imports Ektron.Cms.API
Imports System.Collections.Generic
Imports Ektron.ASM.AssetConfig
Imports System.IO
Imports Ektron.Cms.Controls

Partial Class Workarea_Community_ManageCommunityDocuments
    Inherits System.Web.UI.Page
    Public ImagegalleryImageWidthLbl As String = ""
    Public ImagegalleryTitleLbl As String = ""
    Public ImagegalleryAddressLbl As String = ""
    Public ImagegalleryDescriptionLbl As String = ""
    Public HeaderLbl As String = ""
    Private _jsetoolbar As String = "ParagraphMenu,FontFacesMenu,FontSizesMenu,FontForeColorsMenu|Bold,Italic,Underline,Strikethrough;Superscript,Subscript,RemoveFormat|JustifyLeft,JustifyCenter,JustifyRight,JustifyFull;BulletedList,NumberedList,Indent,Outdent;CreateLink,Unlink,InsertImage{0},InsertRule{1}|Cut,Copy,Paste;Undo,Redo,Print|InsertTable,EditTable;InsertTableRowAfter,InsertTableRowBefore,DeleteTableRow;InsertTableColumnAfter,InsertTableColumnBefore,DeleteTableColumn"
    Protected ctlEditor As Ektron.ContentDesignerWithValidator
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_refContApi As New ContentAPI
    Protected _contentID As Long
    Protected _folderID As Long = -1
    Protected _contentLanguage As Integer = -1
    Protected _fileExtension As String = ""
    Protected _taxonomyID As Long = -1
    Protected isMetadataOrTaxonomyRequired As Boolean = False

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        ctlEditor = LoadControl("../controls/Editor/ContentDesignerWithValidator.ascx")
        With ctlEditor
            .ID = "ekImagegalleryDescription"
            .AllowScripts = False
            .Height = New Unit(250, UnitType.Pixel)
            .Width = New Unit(100, UnitType.Percentage)
            .Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Minimal
            .ShowHtmlMode = False
        End With
        ekImagegalleryDescriptionEditorHolder.Controls.Add(ctlEditor)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = m_refContApi.EkMsgRef
        RegisterResources()
        If (Request.QueryString("folderiD") IsNot Nothing) Then
            _folderID = Convert.ToInt64(Request.QueryString("folderID"))
            jsFolderID.Text = _folderID.ToString()
        End If

        If (Request.QueryString("TaxonomyId") IsNot Nothing) Then
            _taxonomyID = Convert.ToInt64(Request.QueryString("TaxonomyId"))
            jsTaxonomyId.Text = _taxonomyID
            jsTaxonomyIdReloadFrame.Text = _taxonomyID
        End If

        If (Request.QueryString("LangType") IsNot Nothing) Then
            _contentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
            jsLanguageID.Text = _contentLanguage.ToString()
        End If

		If m_refContApi.EkContentRef.DoesFolderRequireMetadataOrTaxonomy(_folderID, _contentLanguage) Then
            isMetadataOrTaxonomyRequired = True
        End If
		
        Dim dragDrop As New ExplorerDragDrop

        Dim _useSSL As Boolean = Ektron.Cms.Common.EkFunctions.GetConfigBoolean("ek_UseOffloadingSSL", False)
        Dim _scheme As String = String.Empty
        If (_useSSL) Then
            _scheme = "https"
        Else
            _scheme = Page.Request.Url.Scheme
        End If

        destination.Value = Convert.ToString(_scheme & Uri.SchemeDelimiter & Me.Page.Request.Url.Authority & m_refContApi.ApplicationPath) & "processMultiupload.aspx?close=true"
        PostURL.Value = Convert.ToString(_scheme & Uri.SchemeDelimiter & Me.Page.Request.Url.Authority & m_refContApi.ApplicationPath) & "processMultiupload.aspx?close=true"
        NextUsing.Value = Convert.ToString(_scheme & Uri.SchemeDelimiter & Page.Request.Url.Authority & m_refContApi.ApplicationPath & "content.aspx")
        content_id.Value = "-1"
        content_folder.Value = _folderID.ToString()
        content_language.Value = _contentLanguage.ToString()
        requireMetaTaxonomy.Value = isMetadataOrTaxonomyRequired.ToString()
        taxonomyselectedtree.Value = _taxonomyID.ToString()
        content_teaser.Value = ""
        Dim linebreak As New HtmlGenericControl("div")
        linebreak.InnerHtml += "<div id='divFileTypes' style='display:none;float:left;'> " & m_refMsg.GetMessage("lbl valid file types") & "<p class='dmsSupportedFileTypes' style='font-size:11px;'>" & DocumentManagerData.Instance.FileTypes & "</p></div>"
        linebreak.InnerHtml += "<br />"
        linebreak.InnerHtml += "<div id=idMultipleView style='display:inline'>"
        linebreak.InnerHtml += "<script type=""text/javascript"">" + Environment.NewLine
        linebreak.InnerHtml += " AC_AX_RunContent('id','idUploadCtl','name','idUploadCtl','classid','CLSID:07B06095-5687-4d13-9E32-12B4259C9813','width','100%','height','350px');" + Environment.NewLine
        linebreak.InnerHtml += Environment.NewLine + " </script> </div> " + Environment.NewLine
        linebreak.InnerHtml += "<br /><br />"
        tabMultipleDMS.Controls.Add(linebreak)

        If (Not Page.IsPostBack) Then

            dragDrop.FolderID = _folderID
            dragDrop.TaxonomyId = _taxonomyID

            If (_contentLanguage <> -1) Then
                dragDrop.ContentLanguage = _contentLanguage
            End If

            HelpMessage.Text = "Fill out the description and then click next to upload image(s)." 'm_refMsg.GetMessage("lbl fill out the description and then click next to drag and drop image(s)")
            btnNext.Text = m_refMsg.GetMessage("next") & ">>"
            ImagegalleryTitleLbl = m_refMsg.GetMessage("generic title label")
            ImagegalleryImageWidthLbl = m_refMsg.GetMessage("lbl maximum width")
            ImagegalleryAddressLbl = m_refMsg.GetMessage("lbl image mapaddress")
            ImagegalleryDescriptionLbl = m_refMsg.GetMessage("description label")
            HeaderLabel.Text = m_refMsg.GetMessage("lbl photo data")
            btnNext.Attributes.Add("onclick", "ResizeContainer();")

            If (Request.QueryString("prop") IsNot Nothing AndAlso Request.QueryString("prop") <> "" AndAlso (Request.QueryString("type") = "update") OrElse (Request.QueryString("prop") = "image")) Then
                ' Content Designer
                With ctlEditor
                    .FolderId = _folderID
                    .AllowFonts = True
                End With

                If Request.QueryString("prop") = "image" Then
                    dragDrop.IsImage = 1
                End If

                Me.panelImageProperties.Visible = True
                Me.panelDragDrop.Visible = False
                If (Request.QueryString("type") IsNot Nothing AndAlso Request.QueryString("type") = "update") Then
                    HelpMessage.Text = m_refMsg.GetMessage("lbl fill out the description and then click save")
                    btnNext.Attributes.Add("onclick", "return HideContainer(this);")
                    Dim id As Long = 0
                    If (Request.QueryString("id") IsNot Nothing) Then
                        id = Convert.ToInt64(Request.QueryString("id"))
                    End If
                    ekImagegalleryImageWidthLbl.Visible = False
                    ekImagegalleryImageWidth.Visible = False
                    If (id > 0) Then
                        Dim data As Ektron.Cms.ContentData
                        Dim api As New Ektron.Cms.ContentAPI()
                        data = api.GetContentById(id)
                        Me.ekImagegalleryTitle.Value = Server.HtmlDecode(data.Title)
                        ctlEditor.Content = data.Teaser
                        If (Request.QueryString("prop") = "image") Then
                            Me.HeaderLabel.Text = "Image Properties"
                            For Each item As Ektron.Cms.ContentMetaData In data.MetaData
                                If (item.TypeName.ToLower() = "mapaddress") Then
                                    Me.ekImagegalleryAddress.Value = item.Text
                                    Exit For
                                End If
                            Next
                        Else
                            Me.HeaderLabel.Text = "Document Properties"
                            Me.ekImagegalleryAddress.Visible = False
                            Me.ekImagegalleryAddressLbl.Visible = False
                        End If
                        btnNext.Text = "Save"
                    End If
                Else
                    ekImagegalleryTitleLbl.Visible = False
                    ekImagegalleryTitle.Visible = False
                End If
            Else
                Me.panelImageProperties.Visible = False
                Me.panelDragDrop.Visible = True
            End If
        Else
            Dim api As New Ektron.Cms.CommonApi()
            Dim desc As String = ""
            Dim width As Integer = 800
            Dim address As String = ""
            desc = ctlEditor.Content
            If (Request.Form("ekImagegalleryImageWidth") IsNot Nothing) Then
                width = Request.Form("ekImagegalleryImageWidth").ToString()
            End If
            If (Request.Form("ekImagegalleryAddress") IsNot Nothing) Then
                address = Request.Form("ekImagegalleryAddress").ToString()
            End If

            Dim imageProp(3) As String
            imageProp(0) = width.ToString() 'width
            imageProp(1) = "-1"             'height
            imageProp(2) = address          'mapaddress
            imageProp(3) = desc             'Descriptions

            If (Request.QueryString("type") <> "update") Then
                Ektron.ASM.EkDavProtocol.Constants.GetCustomCacheManger().Remove(api.UserId.ToString() & "_" & api.UniqueId.ToString() & "_MapMeta")
                Ektron.ASM.EkDavProtocol.Constants.GetCustomCacheManger().Add(api.UserId.ToString() & "_" & api.UniqueId.ToString() & "_MapMeta", imageProp)
            End If
        End If

        If Request.Browser.Type.IndexOf("Firefox") <> -1 Then
            liDragDrop.Visible = True
            tabDragDrop.Visible = True
            tabDragDrop.Controls.Add(dragDrop)
        End If

        literal_wait.Text = m_refMsg.GetMessage("one moment msg")
    End Sub

    Protected Sub btnNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNext.Click
        Dim api As New Ektron.Cms.CommonApi()
        Dim desc As String = ""
        Dim width As Integer = 800
        Dim address As String = ""
        Dim isimage As Integer = 0
        Me.panelImageProperties.Visible = True
        Me.panelDragDrop.Visible = False
        If (Request.QueryString("prop") = "image") Then
            isimage = 1
        End If
        If (Request.QueryString("type") IsNot Nothing AndAlso Request.QueryString("type") = "update") Then
            Dim id As Long = 0
            If (Request.QueryString("id") IsNot Nothing) Then
                id = Convert.ToInt64(Request.QueryString("id"))

                Dim data As Ektron.Cms.ContentEditData
                Dim apiContent As New Ektron.Cms.ContentAPI()
                Dim metaId As Integer = 0
                data = apiContent.GetContentForEditing(id)
                If (Me.ekImagegalleryTitle.Value.IndexOf("<") <> -1 Or Me.ekImagegalleryTitle.Value.IndexOf(">") <> -1 Or Me.ekImagegalleryTitle.Value.IndexOf("'") <> -1 Or Me.ekImagegalleryTitle.Value.IndexOf("""") <> -1) Then
                    Me.ekImagegalleryTitle.Value = Me.ekImagegalleryTitle.Value.Replace("<", "").Replace(">", "").Replace("'", "").Replace("""", "")
                End If
                data.Title = Me.ekImagegalleryTitle.Value
                data.Teaser = ctlEditor.Content
                content_teaser.Value = ctlEditor.Content
                data.FileChanged = False
                apiContent.SaveContent(data)
                If (Request.QueryString("prop") = "image") Then
                    isimage = 1
                    For Each item As Ektron.Cms.ContentMetaData In data.MetaData
                        If (item.TypeName.ToLower() = "mapaddress") Then
                            metaId = item.TypeId
                            Exit For
                        End If
                    Next
                    If (metaId > 0) Then
                        apiContent.UpdateContentMetaData(data.Id, metaId, Me.ekImagegalleryAddress.Value)
                    End If
                End If
                apiContent.PublishContentById(id, data.FolderId, data.LanguageId, "", api.UserId, "")
                Me.ctlEditor.Visible = False 'so that the content designer will not be initialized again.
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "close_box", "Close();", True)
            End If
        Else
            desc = ctlEditor.Content
            If (Request.Form("ekImagegalleryImageWidth") IsNot Nothing) Then
                width = Request.Form("ekImagegalleryImageWidth").ToString()
            End If
            If (Request.Form("ekImagegalleryAddress") IsNot Nothing) Then
                address = Request.Form("ekImagegalleryAddress").ToString()
            End If

            Dim imageProp(3) As String
            imageProp(0) = width.ToString() 'width
            imageProp(1) = "-1"             'height
            imageProp(2) = address          'mapaddress
            imageProp(3) = desc             'Descriptions

            Ektron.ASM.EkDavProtocol.Constants.GetCustomCacheManger().Remove(api.UserId.ToString() & "_" & api.UniqueId.ToString() & "_MapMeta")
            Ektron.ASM.EkDavProtocol.Constants.GetCustomCacheManger().Add(api.UserId.ToString() & "_" & api.UniqueId.ToString() & "_MapMeta", imageProp)

            Me.ctlEditor.Visible = False 'so that the content designer will not be initialized again.
            Dim _doHide As String = String.Empty
            Dim _galleryString As String = String.Empty
            If Not Request.QueryString("hidecancel") Is Nothing AndAlso Request.QueryString("hidecancel") = "true" Then
                _doHide = "&hidecancel=true"
            End If
            If Not Request.QueryString("isimagegallery") Is Nothing AndAlso Request.QueryString("isimagegallery") = "true" Then
                _galleryString = "&isimagegallery=true"
            End If

            Response.Redirect(m_refContApi.AppPath & "DragDropCtl.aspx?mode=0&folder_id=" & Request.QueryString("folderiD") & "&lang_id=" & m_refContApi.ContentLanguage & "&TaxonomyId=" & Request.QueryString("TaxonomyId") & "&isimage=" & isimage & _doHide & _galleryString)
        End If
    End Sub

    Private Sub RegisterResources()
        ' Css
        Css.RegisterCss(Me, Css.ManagedStyleSheet.EktronWorkareaCss)
        Css.RegisterCss(Me, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7)
        ' JS
        JS.RegisterJS(Me, JS.ManagedScript.EktronJS)
        JS.RegisterJS(Me, JS.ManagedScript.EktronXmlJS)
        JS.RegisterJS(Me, JS.ManagedScript.EktronUICoreJS)
        JS.RegisterJS(Me, JS.ManagedScript.EktronUITabsJS)
        JS.RegisterJS(Me, m_refContApi.ApplicationPath + "java/ActiveXActivate.js", "EktronActiveXActivateJs")
        JS.RegisterJS(Me, m_refContApi.ApplicationPath + "java/RunActiveContent.js", "EktronRunActiveContentJs")
        JS.RegisterJS(Me, m_refContApi.ApplicationPath + "java/determineoffice.js", "EktronDetermineOfficeJs")

        ltrPleaseWait.Text = m_refMsg.GetMessage("msg pls wait file uploads")
    End Sub

    Protected Sub uploadFile_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim fileName As String = String.Empty
        Dim fileUpld As HttpPostedFile = ekFileUpload.PostedFile
        Dim hasValidExtension As String = ""
        Dim AllowedFileTypes As New List(Of String)()

        AllowedFileTypes.AddRange(DocumentManagerData.Instance.FileTypes.ToString().Split(","c))
        If fileUpld.ContentLength > 0 Then
            Dim uAPI As Ektron.Cms.UserAPI = New UserAPI()

            _fileExtension = Path.GetExtension(fileUpld.FileName)
            hasValidExtension = AllowedFileTypes.Find(New Predicate(Of String)(AddressOf CheckExtension))

            If hasValidExtension IsNot Nothing AndAlso hasValidExtension <> "" Then

                'If Image Gallery, Should check if the file type is an image file type
                If Request.QueryString("prop") IsNot Nothing AndAlso Request.QueryString("prop").ToLower = "image" Then
                    If Not Ektron.Cms.Common.EkFunctions.IsImage(_fileExtension) Then
                        ltrStatus.Text = m_refMsg.GetMessage("msg invalid file upload images only")
                        setInvalid()
                        Exit Sub
                    End If
                End If

                fileName = Path.GetFileName(fileUpld.FileName)
                Dim fileLength As Integer = fileUpld.ContentLength
                Dim fileData As Byte() = New Byte(fileLength - 1) {}
                Dim file As String = Convert.ToString(fileUpld.InputStream.Read(fileData, 0, fileLength))

                If fileData.Length > 0 Then
                    Dim stream As New System.IO.MemoryStream(fileData)
                    m_refContApi.RequestInformationRef.UserId = uAPI.UserId
                    m_refContApi.ContentLanguage = _contentLanguage

                    Dim asstData As New Ektron.ASM.AssetConfig.AssetData()
                    Dim cContent As New Ektron.Cms.API.Content.Content()
                    asstData = m_refContApi.EkContentRef.GetAssetDataBasedOnFileName(fileName, _folderID, -1)
                    If asstData IsNot Nothing AndAlso asstData.ID <> "" AndAlso asstData.Name <> "" Then
                        Dim astData As Ektron.Cms.AssetUpdateData = New AssetUpdateData()
                        Dim taxonomyCatArray As TaxonomyBaseData() = Nothing
                        _contentID = Convert.ToInt64(asstData.ID)
                        Dim cData As ContentData = cContent.GetContent(_contentID, Ektron.Cms.ContentAPI.ContentResultType.Published)

                        astData.FileName = fileName
                        astData.FolderId = _folderID
                        astData.ContentId = cData.Id
                        astData.Teaser = cData.Teaser
                        astData.Comment = cData.Comment
                        astData.Title = cData.Title
                        astData.GoLive = cData.GoLive
                        astData.TaxonomyTreeIds = Me._taxonomyID

                        'Assigning the categories
                        taxonomyCatArray = m_refContApi.ReadAllAssignedCategory(_contentID)
                        If taxonomyCatArray IsNot Nothing AndAlso taxonomyCatArray.Length > 0 Then
                            For Each tBaseData As TaxonomyBaseData In taxonomyCatArray
                                If astData.TaxonomyTreeIds = "" Then
                                    astData.TaxonomyTreeIds = tBaseData.TaxonomyId.ToString()
                                Else
                                    astData.TaxonomyTreeIds += "," & tBaseData.TaxonomyId.ToString()
                                End If
                            Next
                        End If

                        'Assigning the metadatas
                        If cData.MetaData IsNot Nothing AndAlso cData.MetaData.Length > 0 Then
                            ReDim astData.MetaData(cData.MetaData.Length - 1)
                            For i As Integer = 0 To cData.MetaData.Length - 1
                                astData.MetaData(i) = New AssetUpdateMetaData()
                                astData.MetaData(i).TypeId = cData.MetaData(i).TypeId
                                astData.MetaData(i).ContentId = cData.Id
                                astData.MetaData(i).Text = cData.MetaData(i).Text
                            Next
                        End If
                        astData.EndDate = cData.EndDate
                        astData.EndDateAction = CType([Enum].Parse(GetType(Ektron.Cms.Common.EkEnumeration.CMSEndDateAction), cData.EndDateAction, True), Ektron.Cms.Common.EkEnumeration.CMSEndDateAction)

                        'Updating the Content
                        Dim isUpdated As Boolean = m_refContApi.EditAsset(stream, astData)
                    Else
                        Dim astData As Ektron.Cms.AssetUpdateData = New AssetUpdateData()
                        astData.FileName = fileName
                        astData.FolderId = _folderID
                        astData.Title = Path.GetFileNameWithoutExtension(fileName)
                        astData.LanguageId = _contentLanguage
                        astData.TaxonomyTreeIds = Me._taxonomyID
                        _contentID = m_refContApi.AddAsset(stream, astData)
                    End If
                    jsMetaUrl.Text = ""
                     If m_refContApi.EkContentRef.DoesFolderRequireMetadataOrTaxonomy(_folderID, _contentLanguage) Then
                        Dim _taxString = String.Empty
                        If Me._taxonomyID <> -1 Then
                            _taxString = "&taxonomyId=" & Me._taxonomyID
                        End If
                        jsMetaUrl.Text = m_refContApi.AppPath & "DMSMetadata.aspx?contentId=" & _contentID & "&idString=" & _contentID & "&folderId=" & _folderID & _taxString & "&close=true&EkTB_iframe=true&height=550&width=650&modal=true&refreshCaller=true"
                    End If

                    isFileUploadComplete.Value = "true"
                    ClientScript.RegisterStartupScript(Me.GetType(), "closeThickBox", "uploadClick();", True)
                End If
            Else
                ltrStatus.Text = m_refMsg.GetMessage("msg invalid file upload")
                setInvalid()
            End If
        Else
            ltrStatus.Text = m_refMsg.GetMessage("lbl upload file")
            setInvalid()
        End If
    End Sub

    Private Sub setInvalid()
        DragDropUI.Style.Add("position", "relative")
        DragDropUI.Style.Add("left", "0px")
        ek_DmsFileUploadWait.Style.Add("position", "relative")
        ek_DmsFileUploadWait.Style.Add("left", "-10000px")
        isFileUploadComplete.Value = "invalid"
    End Sub
    Private Function CheckExtension(ByVal item As String) As Boolean
        Return item.ToLower.Replace(" ", "") = "*" & Me._fileExtension.ToLower
    End Function

    <System.Web.Services.WebMethod()> _
<System.Web.Script.Services.ScriptMethod()> _
Public Shared Function CheckFileExists(ByVal FileName As String, ByVal FolderID As String, ByVal ContLanguage As String) As [Boolean]
        Dim cApi As New ContentAPI()
        cApi.ContentLanguage = Convert.ToInt32(ContLanguage)
        Dim assetDat As New Ektron.ASM.AssetConfig.AssetData()
        assetDat = cApi.EkContentRef.GetAssetDataBasedOnFileName(Path.GetFileName(FileName), Convert.ToInt64(FolderID), -1)
        If assetDat IsNot Nothing AndAlso assetDat.ID <> "" AndAlso assetDat.Name <> "" Then
            Return True
        Else
            Return False
        End If
    End Function


End Class
