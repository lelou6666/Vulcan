Imports Ektron.Cms
Partial Class editdesign
    Inherits System.Web.UI.Page

#Region "Member Variables"

    Protected _IsBrowserIE As Boolean = False
    Protected _AppImgPath As String = ""
    Protected _AppPath As String = ""
    Protected _AppeWebPath As String = ""
    Protected _AppName As String = ""
    Protected _CurrentUserId As Long = 0
    Protected _ContentLanguage As Integer = 0
    Protected _StyleHelper As New StyleHelper
    Protected _MessageHelper As Common.EkMessageHelper
    Protected _ContentApi As New ContentAPI
    Protected _LocaleFileString As String = ""
    Protected _PageAction As String = ""
    Protected _Id As Long = 0
    Protected _LocaleFileNumber As String = "1033"
    Protected _AppLocaleString As String = ""
    Protected _EditAction As String
    Protected _LanguageData As LanguageData
    Protected _UnPackDisplayXslt As String = ""
    Protected _SitePath As String = ""
    Protected _UploadPrivs As Boolean
    Protected _Var1 As String
    Protected _Var2 As String
    Protected _XmlConfigData As XmlConfigData
    Protected _EkContent As Ektron.Cms.Content.EkContent
    Protected _UserRights, _MetadataNumber, _PreviousState, _MetaComplete, _MaxContLength, _Path, cLibPerms, _XmlId As Object
    Protected _StyleSheets, _StyleCount, _ii, _KeyName, _StyleSheet As Object
    Protected _st() As Object
    Protected _Segment As Integer = 0
    Protected _ContLoop As Object
    Protected _MyStart, _MyEnd, _MyLength, _XmlStartTag, _XmlEndTag, _IndexCms As Object
    Protected _IsProduct As Boolean = False
    Protected _XmlPage As String = "xml_config.aspx"
    Protected _ProductPage As String = "commerce/producttypes.aspx"

    Private _SelectedEditControl As String
	Private _ContentDesigner As Ektron.ContentDesignerWithValidator

#End Region

#Region "Events"

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
		Dim i As Integer = 0
		_ContentDesigner = LoadControl("controls/Editor/ContentDesignerWithValidator.ascx")
		phEditContent.Controls.Add(_ContentDesigner)
        With _ContentDesigner
            .Visible = False
            .ID = "content_html"
			.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.DataDesigner
			.Height = New Unit(635, UnitType.Pixel)
            .AllowFonts = True
            .ShowPreviewMode = True
		End With
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim security_lib_data As PermissionData
        Dim strBrowserCode As String = ""
        If (Request.Browser.Type.IndexOf("IE") <> -1) Then
            _IsBrowserIE = True
        End If
        If Request.QueryString("type") <> "" Then
            _IsProduct = (Request.QueryString("type").ToLower() = "product")
        End If
        _MessageHelper = _ContentApi.EkMsgRef
        _EkContent = _ContentApi.EkContentRef
        'Make sure the user is logged in.
        If (_EkContent.IsAllowed(0, 0, "users", "IsLoggedIn") = False OrElse _EkContent.IsAllowed(0, 0, "users", "IsAdmin") = False) Then
            If (_IsProduct AndAlso Not _EkContent.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin, _EkContent.RequestInformation.UserId)) _
            OrElse (Not _IsProduct AndAlso Not _EkContent.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminXmlConfig, _EkContent.RequestInformation.UserId)) Then
                Utilities.ShowError(_MessageHelper.GetMessage("com: user does not have permission"))
            End If
        End If

        StyleSheetJS.Text = _StyleHelper.GetClientScript

        RegisterResources()

        _CurrentUserId = _ContentApi.UserId
        _ContentLanguage = _ContentApi.ContentLanguage
        _AppImgPath = _ContentApi.AppImgPath
        _AppImgPath = _ContentApi.AppPath
        _SitePath = _ContentApi.SitePath
        _AppeWebPath = _ContentApi.ApplicationPath & _ContentApi.AppeWebPath
        _AppName = _ContentApi.AppName
        _Id = Request.QueryString("id")
        _PageAction = Request.QueryString("action")
        _Var1 = Request.ServerVariables("SERVER_NAME")
        _LanguageData = (New SiteAPI).GetLanguageById(_ContentLanguage)
        _Var2 = _EkContent.GetEditorVariablev2_0(_Id, _PageAction)
        If Not _LanguageData Is Nothing Then
            _LocaleFileNumber = Convert.ToString(_LanguageData.Id)
            strBrowserCode = _LanguageData.BrowserCode
        End If

        Page.Title = _AppName & " " & _MessageHelper.GetMessage("edit content page title") & " """ & Ektron.Cms.CommonApi.GetEcmCookie()("username") & """"
        _LocaleFileString = GetLocaleFileString(_LocaleFileNumber)

        Dim refSiteApi As New SiteAPI
        Dim settings_data As SettingsData
        settings_data = refSiteApi.GetSiteVariables(refSiteApi.UserId)
        _AppLocaleString = GetLocaleFileString(settings_data.Language)

		_SelectedEditControl = "eWebEditPro"
		Dim bIsMac As Boolean = False
		Try
			If (Request.Browser.Platform.IndexOf("Win") = -1) Then
				bIsMac = True
			End If
			If bIsMac Then
				_SelectedEditControl = "ContentDesigner"
			Else
				If (ConfigurationManager.AppSettings("ek_DataDesignControl") IsNot Nothing) Then
					_SelectedEditControl = ConfigurationManager.AppSettings("ek_DataDesignControl").ToString()
				End If
			End If
		Catch ex As Exception
			_SelectedEditControl = "ContentDesigner"
		End Try

		If (_SelectedEditControl = "eWebEditPro") Then
			EditorJS.Text = Utilities.EditorScripts(_Var2, _AppeWebPath, strBrowserCode)
		Else
			EditorJS.Text = ""
		End If

		security_lib_data = _ContentApi.LoadPermissions(_Id, "folder")
		_UploadPrivs = security_lib_data.CanAddToFileLib Or security_lib_data.CanAddToImageLib

		_EditAction = Request.QueryString("editaction")	'Request.Form("editaction")
		If (Not (Page.IsPostBack)) Then
			If (_EditAction = "cancel") Then
				'do nothing
			Else
				DisplayControl()
			End If

		Else
			UpdateEditDesign()
		End If
		jsAppeWebPath.Text = _AppeWebPath
		jsAppLocaleString.Text = _AppLocaleString
		jsSitePath.Text = _SitePath
		jsPath.Text = Replace(_Path, "\", "\\")
		jsTitle.Text = _MessageHelper.GetMessage("generic title label")
		jsXmlStyle.Text = _MessageHelper.GetMessage("lbl select style for xml design")
		jsInputTmpStyleSheet.Text = "<input type=""text"" maxlength=""255"" size=""45"" value=""" & _StyleSheet & """ name=""stylesheet"" id=""stylesheet"" onchange=""SetStyleSheetFromInput();"" />"
		jsTmpStyleSheet.Text = _StyleSheet
		jshdnContentLanguage.Text = "<input type=""hidden"" name=""content_language"" value=""" & _ContentLanguage & """/>"
		jshdnMaxContLength.Text = "<input type=""hidden"" name=""maxcontentsize"" value=""" & _MaxContLength & """/>"
		jshdnXml_id.Text = "<input type=""hidden"" name=""xml_collection_id"" value=""" & _XmlId & """/>"
		jshdnIndex_cms.Text = "<input type=""hidden"" name=""index_cms"" value=""" & _IndexCms & """/>"
		jshdniSegment.Text = "<input type=""hidden"" name=""numberoffields"" value=""" & _Segment & """/>"
		jsMaxContLength.Text = _MaxContLength
		jsEditorMsg.Text = _MessageHelper.GetMessage("lbl wait editor not loaded")
		sContentInvalid.Text = _MessageHelper.GetMessage("msg content invalid")
		sDesignIncompatible.Text = _MessageHelper.GetMessage("msg design incompatible")
		sContinue.Text = _MessageHelper.GetMessage("msg wish to continue")
	End Sub

#End Region

#Region "Helpers"

    Private Function GetLocaleFileString(ByVal localeFileNumber As String) As String
        Dim LocaleFileString As String
        If (CStr(localeFileNumber) = "" Or CInt(localeFileNumber) = 1) Then
            LocaleFileString = "0000"
        Else
            LocaleFileString = New String("0", 4 - Len(Hex(localeFileNumber)))
            LocaleFileString = LocaleFileString & Hex(localeFileNumber)
            If Not System.IO.File.Exists(Server.MapPath(_AppeWebPath & "locale" & LocaleFileString & "b.xml")) Then
                LocaleFileString = "0000"
            End If
        End If
        Return LocaleFileString.ToString
    End Function

    Private Sub DisplayControl()
        If _IsProduct Then
            EditProductDesignToolBar()
        Else
            EditDesignToolBar()
        End If


        _XmlId = _Id

        'stop
        _StyleSheets = _EkContent.GetAllStyleSheets()  ' get a list of registered style sheets
        _StyleCount = _StyleSheets("NumberOfStyleSheets")
        ReDim _st(_StyleCount)
        For _ii = 1 To _StyleCount
            _KeyName = "StyleSheet_" & _ii
            _st(_ii) = _StyleSheets(_KeyName)
        Next

        _XmlConfigData = _ContentApi.GetXmlConfiguration(_Id)


        Dim cPer As PermissionData
        Dim content_html, content_title, content_stylesheet As Object
        cPer = _ContentApi.LoadPermissions(_Id, "content")

        content_html = _XmlConfigData.PackageXslt '("PackageXslt")
        _StyleSheet = _XmlConfigData.DesignStyleSheet '("DesignStyleSheet")
        content_title = "Design Mode"
        content_stylesheet = ""

        TD_ColTitle.InnerHtml = _XmlConfigData.Title '("CollectionTitle")

        If _StyleCount <> 0 Then
            stylesheetoptions.Items.Add(New ListItem("--" & "-- Select a style sheet --" & "--", "ignore"))

            For _ii = 1 To _StyleCount
                stylesheetoptions.Items.Add(New ListItem(_st(_ii), _st(_ii)))
            Next
            If _StyleSheet <> "" Then
                stylesheetoptions.SelectedValue = _StyleSheet
            End If

            stylesheetoptions.Attributes.Add("onchange", "SetStyleSheet();")
        End If
        Dim SiteVars As SettingsData

        SiteVars = (New SiteAPI).GetSiteVariables(_CurrentUserId)
        _MaxContLength = SiteVars.MaxContentSize
        'strStyleInfo = ""

        _XmlStartTag = "<index_cms>"
        _XmlEndTag = "</index_cms>"
        _MyStart = InStr(1, content_html, _XmlStartTag)
        _IndexCms = ""
        If _MyStart > 0 Then
            _MyEnd = InStr(1, content_html, _XmlEndTag)
            If _MyEnd > 0 Then
                _MyStart = _MyStart + Len(_XmlStartTag)
                _MyLength = _MyEnd - _MyStart
                _IndexCms = Mid(content_html, _MyStart, _MyLength)
                _IndexCms = Server.HtmlEncode(_IndexCms)
            End If
        End If

        _ContLoop = 1
        If (Len(content_html) > _MaxContLength) Then
            _Var1 = Len(content_html)
        Else
            _Var1 = _MaxContLength
        End If
        Do While _ContLoop <= _Var1
            hiddenfields.Text += "<input type=""hidden"" name=""hiddencontent" & _Segment + 1 & """ value="""">"
            _ContLoop = (_ContLoop + 65000)
            _Segment = (_Segment + 1)
        Loop
        loadSegmentsFn.Text = GetLoadSegmentsScript()

        If (_UploadPrivs = False) Then
            DisabledUpload.Text = "DisableUpload('content_html');"
        End If
        If "ContentDesigner" = _SelectedEditControl Then
            With _ContentDesigner
                .Visible = True
                .Width = New Unit(100, UnitType.Percentage)
				.Height = New Unit(635, UnitType.Pixel)
                Dim smartFormDesign As String = _ContentApi.TransformXsltPackage(content_html, Server.MapPath(.ScriptLocation & "unpackageDesign.xslt"), True)
                .Content = smartFormDesign
            End With
            ' Selecting a CSS stylesheet is only for eWebEditPro
            SelectStyleCaption.Visible = False
            SelectStyleControl.Visible = False
        Else
            Dim ctlEditor As New Ektron.Cms.Controls.HtmlEditor
            phEditContent.Controls.Remove(_ContentDesigner)
            With ctlEditor
                .WorkareaMode(2)  ' We are designing in the workarea
                .ID = "content_html"
                .Width = New Unit(100, UnitType.Percentage)
                .Height = New Unit(90, UnitType.Percentage)
                .Path = _AppeWebPath
                .MaxContentSize = _MaxContLength
                .Text = content_html
            End With
            phEditContent.Controls.Add(ctlEditor)
        End If
    End Sub

    Private Function GetLoadSegmentsScript() As String
        Dim result As New System.Text.StringBuilder
        result.Append("function loadSegments() {" & vbCrLf)
        result.Append("}")
        Return (result.ToString)
    End Function

    Private Sub UpdateEditDesign()
        'Dim xml, xsl
        Dim displayXslt As String
        Dim cCont As Collection
        Dim strcontent, icontloop As Object
        Dim index_cms, ContentId As Object
        Dim configType As Common.EkEnumeration.XmlConfigType = Common.EkEnumeration.XmlConfigType.Content

        configType = _EkContent.GetXMLConfigType(_Id)

        UnPackDisplay()
        cCont = New Collection
        strcontent = ""
        If "ContentDesigner" = _SelectedEditControl Then
            Dim strDesign As String = _ContentDesigner.Content
            Dim strSchema As String = _ContentApi.TransformXsltPackage(strDesign, Server.MapPath(_ContentDesigner.ScriptLocation & "DesignToSchema.xslt"), True)
            Dim strFieldList As String = _ContentApi.TransformXsltPackage(strDesign, Server.MapPath(_ContentDesigner.ScriptLocation & "DesignToFieldList.xslt"), True)
            Dim strViewEntryXslt As String = _ContentApi.TransformXsltPackage(strDesign, Server.MapPath(_ContentDesigner.ScriptLocation & "DesignToEntryXSLT.xslt"), True)

			Dim objXsltArgs As New System.Xml.Xsl.XsltArgumentList()
			objXsltArgs.AddParam("srcPath", "", _ContentDesigner.ScriptLocation)

			Dim strViewXslt As String = _ContentApi.XSLTransform("<root>" & strDesign & "<ektdesignpackage_list>" & strFieldList & "</ektdesignpackage_list></root>", _
			 Server.MapPath(_ContentDesigner.ScriptLocation & "DesignToViewXSLT.xslt"), True, False, objXsltArgs)
            Dim strInitDoc As String = _ContentApi.TransformXsltPackage(strDesign, Server.MapPath(_ContentDesigner.ScriptLocation & "PresentationToData.xslt"), True)
            Dim sbPackage As New StringBuilder
            With sbPackage
                .Append("<ektdesignpackage_forms><ektdesignpackage_form><ektdesignpackage_designs><ektdesignpackage_design>")
                .Append(strDesign)
                .Append("</ektdesignpackage_design></ektdesignpackage_designs><ektdesignpackage_schemas><ektdesignpackage_schema>")
                .Append(strSchema)
                .Append("</ektdesignpackage_schema></ektdesignpackage_schemas><ektdesignpackage_lists><ektdesignpackage_list>")
                .Append(strFieldList)
                .Append("</ektdesignpackage_list></ektdesignpackage_lists><ektdesignpackage_views><ektdesignpackage_view>")
                .Append(strViewEntryXslt)
                .Append("</ektdesignpackage_view><ektdesignpackage_view>")
                .Append(strViewXslt)
                .Append("</ektdesignpackage_view></ektdesignpackage_views><ektdesignpackage_initialDocuments><ektdesignpackage_initialDocument>")
                .Append(strInitDoc)
                .Append("</ektdesignpackage_initialDocument></ektdesignpackage_initialDocuments></ektdesignpackage_form></ektdesignpackage_forms>")
            End With
            strcontent = sbPackage.ToString()
        Else
            icontloop = 1
            Do While (Len(Request.Form("hiddencontent" & icontloop)) > 0)
                strcontent = strcontent & Request.Form("hiddencontent" & icontloop)
                icontloop = icontloop + 1
            Loop
        End If
        icontloop = 1
        cCont.Add(strcontent, "Package")
        If Request.Form("stylesheet") <> "" Then
            cCont.Add(Request.Form("stylesheet"), "DesignStylesheet")
        Else
            cCont.Add("", "DesignStylesheet")
        End If

        cCont.Add(Request.Form("xml_collection_id"), "XmlCollectionID")

        displayXslt = _ContentApi.TransformXsltPackage(strcontent, _UnPackDisplayXslt, False)
        cCont.Add(displayXslt, "displayXslt")


        _EkContent.UpdatexmlCollectionPackage(cCont, _CurrentUserId)
        ContentId = Request.Form("xml_collection_id")

        index_cms = Request.Form("index_cms")
        'now that index_cms is server side .net can play with it
        Server.Transfer("xml_index_select.aspx?action=servertransfer" & IIf(configType = Common.EkEnumeration.XmlConfigType.Product, "&type=product", ""), True)
        'Response.Redirect("xml_config.aspx?LangType=" & ContentLanguage & "&page=&id=" & Request.Form("xml_collection_id") & "&action=ViewXmlConfiguration", False)
    End Sub

    Private Sub EditProductDesignToolBar()
        Dim result As New System.Text.StringBuilder
        divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl product type xml config"))
        result.Append("<table><tr>")
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/save.png", "editdesign.aspx", _MessageHelper.GetMessage("alt Save XML Configuration"), _MessageHelper.GetMessage("btn update"), "OnClick=""javascript:return SetAction('update');"""))
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", _ProductPage & "?LangType=" & _ContentLanguage & "&page=" & _ProductPage & "&id=" & Request.QueryString("id") & "&action=viewproducttype", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "OnClick=""javascript:return SetAction('cancel');"""))
        result.Append("<td>" & _StyleHelper.GetHelpButton("EditProductTypeXML") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub

    Private Sub EditDesignToolBar()
        Dim result As New System.Text.StringBuilder
        divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl XML Configuration"))
        result.Append("<table><tr>")
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/save.png", "editdesign.aspx", _MessageHelper.GetMessage("alt Save XML Configuration"), _MessageHelper.GetMessage("btn update"), "OnClick=""javascript:return SetAction('update');"""))
        result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath & "images/UI/Icons/back.png", _XmlPage & "?LangType=" & _ContentLanguage & "&page=" & _XmlPage & "&id=" & Request.QueryString("id") & "&action=ViewXmlConfiguration", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "OnClick=""javascript:return SetAction('cancel');"""))
        result.Append("<td>" & _StyleHelper.GetHelpButton("EditPackage") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub

    Private Sub UnPackDisplay()
        _UnPackDisplayXslt = "<?xml version=""1.0"" encoding=""UTF-8""?>" _
            & "<xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">" _
            & "<xsl:output method=""xml"" version=""1.0"" encoding=""UTF-8"" indent=""yes""/>" _
            & "<xsl:template match=""/"">" _
            & "<xsl:choose>" _
            & "<xsl:when test=""ektdesignpackage_forms/ektdesignpackage_form[1]/ektdesignpackage_views/ektdesignpackage_view[2]"">" _
            & "<xsl:copy-of select=""ektdesignpackage_forms/ektdesignpackage_form		[1]/ektdesignpackage_views/ektdesignpackage_view[2]/node()""/>" _
            & "</xsl:when>" _
            & "	<xsl:otherwise>" _
            & "<xsl:copy-of select=""ektdesignpackage_forms/ektdesignpackage_form		[1]/ektdesignpackage_views/ektdesignpackage_view[1]/node()""/>" _
            & "</xsl:otherwise>" _
            & "</xsl:choose>" _
            & "</xsl:template>" _
            & "</xsl:stylesheet>"
    End Sub

#End Region

#Region "CSS, JS"

    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, _EkContent.RequestInformation.ApplicationPath & "java/empjsfunc.js", "EktronEmpJSFuncJS")
        Ektron.Cms.API.JS.RegisterJS(Me, _EkContent.RequestInformation.ApplicationPath & "java/datejsfunc.js", "EktronDateJSFuncJS")
        Ektron.Cms.API.JS.RegisterJS(Me, _EkContent.RequestInformation.ApplicationPath & "java/toolbar_roll.js", "EktronToolbarRollJS")

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub

#End Region

End Class
