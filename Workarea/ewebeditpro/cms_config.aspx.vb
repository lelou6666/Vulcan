
Partial Class cms_config
	Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

	'This call is required by the Web Form Designer.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

	End Sub


	Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
		'CODEGEN: This method call is required by the Web Form Designer
		'Do not modify it using the code editor.
		InitializeComponent()
	End Sub
	Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
	Protected InterfaceName As String = "standard"
	Protected MinimalFeatureSet As Boolean = False
	Protected DefaultGetContentType As String = "htmlbody"
	Protected NoSrcView As String = ""
	Protected FormToolbarVisible As String = ""
	Protected FormToolbarEnabled As String = ""
	Protected EKSLK As String = ""
	Protected AppeWebPath As String = ""
	Protected extensions As String = ""
	Protected mode As String = ""
	Protected ExtUI As String = ""
	Protected PresWrdStyl As String = ""
	Protected PresWrdCls As String = ""
	Protected FontList As String = ""
	Protected sAccess As String = "none"
	Protected bAccessEval As String = "false"
	Protected settings_data As Ektron.Cms.SettingsData
    Protected ContentLanguage As Integer
    Protected IsForum As Boolean = False
    Protected options As Hashtable = New Hashtable()
    Protected strButtons As String = ""
    Protected bEnableFontButtons As Boolean = False
    Protected bWikiButton As Boolean = True
#End Region


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Dim refContApi As New Ektron.Cms.ContentAPI
		Dim refSiteApi As New Ektron.Cms.SiteAPI
        Dim refSite As New Ektron.Cms.Site.EkSite(refContApi.RequestInformationRef)
        Dim folder As Long = 0
        internal.Text = refSite.GetEditorInternal()
        If (LCaseQueryString("mode") = "forum") Then
            strButtons = Request.QueryString("toolButtons")
            Dim arTools() As String = strButtons.ToLower().Split(",")
            For Each Item As String In arTools
                If (Not (options.ContainsKey(Item))) Then
                    options.Add(Item, Item)
                End If
            Next
        End If
        Response.ContentType = "text/xml"
        m_refMsg = refSiteApi.EkMsgRef
        settings_data = refSiteApi.GetSiteVariables()
        refSite = refContApi.EkSiteRef
        FontList = refContApi.GetFontConfigList
        EKSLK = refSite.GetInternal()
        AppeWebPath = refContApi.AppPath & refContApi.AppeWebPath

        If (LCaseQueryString("FormToolbarVisible") = "true") Then
            FormToolbarVisible = "true"
            FormToolbarEnabled = "true"
        Else
            FormToolbarVisible = "false"
            FormToolbarEnabled = "true"
        End If
        If "0" = LCaseQueryString("wiki") Then
            bWikiButton = "false"
        Else
            bWikiButton = "true"
        End If
        'Set the state for word styles
        If (settings_data.PreserveWordStyles = True) Then
            PresWrdStyl = "true"
        Else
            PresWrdStyl = "false"
        End If
        If (settings_data.PreserveWordClasses = True) Then
            PresWrdCls = "true"
        Else
            PresWrdCls = "false"
        End If
        Select Case settings_data.Accessibility
            Case 1
                sAccess = "loose"
                bAccessEval = "true"
            Case 2
                sAccess = "strict"
                bAccessEval = "true"
            Case Else
                sAccess = "none"
                bAccessEval = "false"
        End Select
        NoSrcView = LCaseQueryString("nosrc")
        mode = LCaseQueryString("mode")
        If (mode = "") Then
            mode = "wysiwyg"
        End If

        If (mode = "forum") Then
            mode = "wysiwyg"
            IsForum = True
        End If

        If "datadesign" = mode Or "dataentry" = mode Then
            DefaultGetContentType = "datadesignpackage"
        ElseIf "formdesign" = mode Then
            DefaultGetContentType = "designpage"
        End If

        ExtUI = LCaseQueryString("extui")

        InterfaceName = LCaseQueryString("InterfaceName")
        If "" = InterfaceName Then
            If "datadesign" = mode Or "formdesign" = mode Or "dataentry" = mode Then
                InterfaceName = mode
                FormToolbarEnabled = "false"
            Else
                InterfaceName = "standard"
            End If
        ElseIf "none" = InterfaceName Then
            MinimalFeatureSet = False
            FormToolbarEnabled = "false"
        ElseIf "minimal" = InterfaceName Or "calendar" = InterfaceName Or "task" = InterfaceName Then
            If Len(strButtons) > 0 AndAlso True = options.ContainsKey("wmv") Then
                MinimalFeatureSet = False
            Else
                MinimalFeatureSet = True
            End If
            FormToolbarEnabled = "false"
        End If

        If settings_data.EnableFontButtons Or True = options.ContainsKey("fontmenu") Then
            bEnableFontButtons = True
        End If

        Dim strFolder As String
        strFolder = Request.QueryString("folder")
        If (Not IsNothing(strFolder)) Then
            If IsNumeric(strFolder) Then
                folder = Convert.ToInt64(strFolder)
            End If
        End If
		Dim lib_settings As Ektron.Cms.LibraryConfigData
        extensions = ""
        Try
            ' Only make this call if we are logged in
			Dim refContentApi As New Ektron.Cms.ContentAPI
			Dim refContent As Ektron.Cms.Content.EkContent
            refContent = refContentApi.EkContentRef

            If (refContent.IsAllowed(0, 0, "users", "IsLoggedIn")) Then
                ' An exception will be thrown if the user is not authenticated.
                lib_settings = refContApi.GetLibrarySettings(folder)
                If (Not IsNothing(lib_settings)) Then
                    extensions = lib_settings.ImageExtensions & "," & lib_settings.FileExtensions
                End If
            End If
        Catch ex As Exception
            ' ignore error
        End Try

        ContentLanguage = refContApi.RequestInformationRef.ContentLanguage

        PopulateDataLists(refContApi)
    End Sub

	Private Sub PopulateDataLists(ByVal refContApi As Ektron.Cms.ContentAPI)
		Try
			If "datadesign" = mode Or "formdesign" = mode Or "xsltdesign" = mode Or "dataentry" = mode Then
				Dim objCmsCollection As Ektron.Cms.CollectionListData
				Dim colCollection As Collection
				Dim colItems As Collection
				Dim colItem As Collection
				Dim strErrorMessage As String
				Dim nLangType As Integer
				Dim nItemType As Integer
				Dim strItemContent As String
				Dim nListID As Long
				Dim strListName As String
				Dim strCollectionName As String
				Dim bForChoice As Boolean
				Dim bForList As Boolean
				Dim strListChoice As String
				Dim strDataList As String
				Dim sbChoiceFld As New StringBuilder
				Dim sbListFld As New StringBuilder
				Dim sbDataLists As New StringBuilder

				'refContApi.GetAllXmlConfigurations("")
				'refContApi.GetXmlConfiguration(0)
				' GetAllContentByXmlConfigID doesn't do what is says
				'refContApi.EkContentRef.GetAllContentByXmlConfigID(0)

				' Find collection named "CMS Data Lists"
				objCmsCollection = GetCollectionByTitle(refContApi, "CMS Data Lists")
				If IsNothing(objCmsCollection) Then Exit Sub
				strErrorMessage = ""
				colCollection = refContApi.EkContentRef.GetEcmCollectionByID(objCmsCollection.Id, GetHTML:=True, PreviewMode:=False, ErrString:=strErrorMessage)
				If Len(strErrorMessage) > 0 Then Exit Sub

				nLangType = refContApi.RequestInformationRef.ContentLanguage
				colItems = colCollection("Contents")
				For Each colItem In colItems
					nItemType = colItem("ContentType")
					If 1 = nItemType Then ' 1 = content
						strItemContent = colItem("ContentHtml")
						If strItemContent.Contains("<CmsDataList") AndAlso strItemContent.Contains("<ol") Then
							nListID = colItem("ContentID")
							strCollectionName = colItem("ContentTitle")
							strListName = String.Format("cms{0:d}", nListID)
							Select Case mode
								Case "datadesign"
									bForChoice = strItemContent.Contains("DataDesignChoiceFld")
									bForList = strItemContent.Contains("DataDesignListFld")
								Case "formdesign"
									bForChoice = strItemContent.Contains("FormDesignChoiceFld")
									bForList = strItemContent.Contains("FormDesignListFld")
								Case Else
									bForChoice = False
									bForList = False
							End Select
							If bForChoice Or bForList Then
								' <listchoice data="{list-name}">{display-text}</listchoice>
								strListChoice = String.Format("<listchoice data=""{0}"">{1}</listchoice>", strListName, strCollectionName)
								If bForChoice Then
									sbChoiceFld.AppendLine(strListChoice)
								End If
								If bForList Then
									sbListFld.AppendLine(strListChoice)
								End If
							End If
							' <datalist name="{list-name}" src="{AppeWebPath}cmsdatalist.aspx?id={id}" cache="false" select="/CmsDataList/ol/li" captionxpath="." valuexpath="@title" />
							strDataList = String.Format("<datalist name=""{0}"" src=""{1}cmsdatalist.aspx?id={2:d}&amp;LangType={3:d}"" cache=""false"" select=""/CmsDataList/ol/li"" captionxpath=""."" valuexpath=""@title"" />", strListName, AppeWebPath, nListID, nLangType)
							sbDataLists.AppendLine(strDataList)
						End If
					End If
				Next

				Select Case mode
					Case "datadesign"
						DataDesignChoiceFld.Text = sbChoiceFld.ToString
						DataDesignListFld.Text = sbListFld.ToString
					Case "formdesign"
						FormDesignChoiceFld.Text = sbChoiceFld.ToString
						FormDesignListFld.Text = sbListFld.ToString
				End Select
				DataLists.Text = sbDataLists.ToString
			End If
		Catch ex As Exception
			' ignore
		End Try
	End Sub

	Private Function GetCollectionByTitle(ByVal refContApi As Ektron.Cms.ContentAPI, ByVal Title As String) As Ektron.Cms.CollectionListData
		Try
			Dim objCollectionList As Ektron.Cms.CollectionListData()
			Dim iCol As Integer
			objCollectionList = refContApi.EkContentRef.GetCollectionList()
			If IsNothing(objCollectionList) Then Return Nothing
			For iCol = LBound(objCollectionList) To UBound(objCollectionList)
				If Title = objCollectionList(iCol).Title Then
					Return objCollectionList(iCol)
				End If
			Next
		Catch ex As Exception
			' ignore
		End Try
		Return Nothing
	End Function

	Private Function LCaseQueryString(ByVal Name As String) As String
		Dim strValue As String
		strValue = Request.QueryString(Name)
		If IsNothing(strValue) Then
			strValue = ""
		End If
		Return strValue.ToLower
	End Function

End Class
