Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants

Partial Class reportstoolbar
    Inherits System.Web.UI.UserControl

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub
    Protected m_refContentApi As New ContentAPI
    Protected g_ContentTypeSelected As String = Ektron.Cms.Common.EkConstants.CMSContentType_AllTypes
    Public Const CMSContentType_Content As Integer = 1
    Public Const CMSContentType_Forms As Integer = 2
    Public Const CMSContentType_Library As Integer = 7
    Public Const CMSContentType_NonImageLibrary As Integer = 9
    Public Const CMSContentType_PDF As Integer = 10
    Private m_refMsg As Common.EkMessageHelper
    Private m_refStyle As New StyleHelper
    Private m_strPageAction As String = ""
    Private m_strTitleBarMsg As String = ""
    Private m_strAppImgPath As String = ""
    Private m_strFilterType As String = ""
    Private m_data As Object = Nothing
    Protected EnableMultilingual As Integer = 0
    Protected ContentLanguage As Integer = -1
    Protected lContentType As Integer = 0
    Protected asset_data As AssetInfoData()
    Private m_EnableEmail As Boolean = True
    Private m_EnableFolders As Boolean = True
    Private m_EnableContentTypes As Boolean = True
    Private m_EnableDefaultTitlePrefix As Boolean = True
	Private m_strTitlePrefix As String = ""
	Private m_HasData As Boolean = True

    Public Property PageAction() As String
        Get
            Return (m_strPageAction)
        End Get
        Set(ByVal Value As String)
            m_strPageAction = Value
        End Set
	End Property
	Public Property HasData() As Boolean
		Get
			Return (m_HasData)
		End Get
		Set(ByVal value As Boolean)
			m_HasData = value
		End Set
	End Property
    Public Property TitleBarMsg() As String
        Get
            Return (m_strTitleBarMsg)
        End Get
        Set(ByVal Value As String)
            m_strTitleBarMsg = Value
        End Set
    End Property
    Public Property AppImgPath() As String
        Get
            Return (m_strAppImgPath)
        End Get
        Set(ByVal Value As String)
            m_strAppImgPath = Value
        End Set
    End Property
    Public Property FilterType() As String
        Get
            Return (m_strFilterType)
        End Get
        Set(ByVal Value As String)
            m_strFilterType = Value
        End Set
    End Property
    Public Property Data() As Object
        Get
            Return (m_data)
        End Get
        Set(ByVal Value As Object)
            m_data = Value
        End Set
    End Property
    Public Property MultilingualEnabled() As Integer
        Get
            Return EnableMultilingual
        End Get
        Set(ByVal Value As Integer)
            EnableMultilingual = Value
        End Set
    End Property
    Public Property ContentLang() As Integer
        Get
            Return ContentLanguage
        End Get
        Set(ByVal Value As Integer)
            ContentLanguage = Value
        End Set
    End Property
    Public Property EnableEmail() As Boolean
        Get
            If (String.IsNullOrEmpty(m_refContentApi.RequestInformationRef.SystemEmail)) Then
                m_EnableEmail = False
            End If
            Return (m_EnableEmail)
        End Get
        Set(ByVal Value As Boolean)
            m_EnableEmail = Value
        End Set
    End Property
    Public Property EnableFolders() As Boolean
        Get
            Return (m_EnableFolders)
        End Get
        Set(ByVal Value As Boolean)
            m_EnableFolders = Value
        End Set
    End Property
    Public Property EnableContentTypes() As Boolean
        Get
            Return (m_EnableContentTypes)
        End Get
        Set(ByVal Value As Boolean)
            m_EnableContentTypes = Value
        End Set
    End Property
    Public Property EnableDefaultTitlePrefix() As Boolean
        Get
            Return (m_EnableDefaultTitlePrefix)
        End Get
        Set(ByVal Value As Boolean)
            m_EnableDefaultTitlePrefix = Value
        End Set
    End Property
    Public Property TitlePrefix() As String
        Get
            Return (m_strTitlePrefix)
        End Get
        Set(ByVal Value As String)
            m_strTitlePrefix = Value
        End Set
    End Property

#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim count As Integer = 0
        Dim lAddMultiType As Integer = 0
        Dim bSelectedFound As Boolean = False

        m_refMsg = (New CommonApi).EkMsgRef
        Dim result As New System.Text.StringBuilder
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        AppImgPath = m_refContentApi.AppImgPath
        result.Append("<table><tr>" & vbCrLf)

        If (EnableDefaultTitlePrefix) Then
            m_strTitlePrefix = m_refMsg.GetMessage("content reports title bar msg")
        End If

		If (Not (IsNothing(Request.QueryString("action")))) Then
			PageAction = Request.QueryString("action").ToLower
		End If

		If (PageAction = "viewallreporttypes") Then
			txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_strTitlePrefix)
            result.Append(m_refStyle.GetButtonEventsWCaption(Me.m_refContentApi.AppPath & "images/UI/Icons/back.png", "history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else

            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_strTitlePrefix & " " & TitleBarMsg)
            If (PageAction = "viewcheckedout") Then
                If (Not (IsNothing(Data))) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(Me.m_refContentApi.AppPath & "images/UI/Icons/checkIn.png", "#", m_refMsg.GetMessage("alt:checkin all selected icon text"), m_refMsg.GetMessage("btn checkin"), "onclick=""return GetIDs();"""))
                End If
            End If
            If (PageAction = "contentreviews") Then
                If (Not (IsNothing(Data))) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(Me.m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt:save all sel rev icon text"), m_refMsg.GetMessage("btn save"), "onclick=""CheckApproveSelect(); return false;"""))
                End If
            End If
            If (PageAction = "contentflags") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(Me.m_refContentApi.AppPath & "images/UI/Icons/chartBar.png", "#", "Click here to view report", m_refMsg.GetMessage("btn report"), "onclick=""return ReportContentFlags();"""))
            End If
            If (PageAction = "viewcheckedin") Then
                If (Not (IsNothing(Data))) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(Me.m_refContentApi.AppPath & "images/UI/Icons/approvalSubmitFor.png", "#", m_refMsg.GetMessage("alt:submit all selected icon text"), m_refMsg.GetMessage("btn submit"), "onclick=""return GetIDs();"""))
                End If
            End If
            If (PageAction = "viewtoexpire") Then
                result.Append(m_refStyle.GetButtonEventsWCaption(Me.m_refContentApi.AppPath & "images/UI/Icons/chartBar.png", "#", "Click here to view report", m_refMsg.GetMessage("btn report"), "onclick=""return ReportContentToExpire();"""))
            End If

            result.Append("<td>")
            If (True = HasData) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(Me.m_refContentApi.AppPath & "images/UI/Icons/print.png", "#", m_refMsg.GetMessage("Print Report button text"), m_refMsg.GetMessage("btn print"), "onclick=""PrintReport();"""))
                If (EnableEmail()) Then
                    result.Append(m_refStyle.GetButtonEventsWCaption(Me.m_refContentApi.AppPath & "images/UI/Icons/email.png", "#", m_refMsg.GetMessage("Email Report button text"), m_refMsg.GetMessage("btn email"), "onclick=""LoadUserListChildPage('" & PageAction & "');"""))
                End If
            End If

            If (PageAction.ToLower() <> "siteupdateactivity") And (PageAction.ToLower() <> "viewasynchlogfile") And (PageAction.ToLower() <> "viewpreapproval") And EnableFolders() Then
                result.Append("<td>")
                result.Append(m_refStyle.GetButtonEventsWCaption(Me.m_refContentApi.AppPath & "images/UI/Icons/folder.png", "#", m_refMsg.GetMessage("filter report folder"), m_refMsg.GetMessage("filter report folder"), "onclick=""LoadFolderChildPage('" & PageAction & "','" & ContentLanguage & "');"""))
            End If
            result.Append("<td>")
            If (Not Utilities.IsMac() And "siteupdateactivity" = PageAction.ToLower() And True = HasData) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(Me.m_refContentApi.AppPath & "images/UI/Icons/tableExport.png", "#", m_refMsg.GetMessage("btn export"), m_refMsg.GetMessage("btn export"), "onclick=""export_result();"""))
            End If
        End If

        If EnableMultilingual = 1 And (PageAction.ToLower() <> "viewasynchlogfile") And (PageAction.ToLower() <> "viewpreapproval") Then
            Dim m_refsite As New SiteAPI
            Dim language_data() As LanguageData = m_refsite.GetAllActiveLanguages
            count = 0
            result.Append("<td class=""label"">")
            result.Append(m_refMsg.GetMessage("lbl View") & ":")
            result.Append("</td>")
            result.Append("<td>")
            result.Append("<select id=selLang name=selLang OnChange=""LoadLanguage('selections');"">")
            If ContentLanguage = -1 Then
                result.Append("<option value=" & ALL_CONTENT_LANGUAGES & " selected>All</option>")
            Else
                result.Append("<option value=" & ALL_CONTENT_LANGUAGES & ">All</option>")
            End If
            For count = 0 To language_data.Length - 1
                If Convert.ToString(ContentLanguage) = Convert.ToString(language_data(count).Id) Then
                    result.Append("<option value=" & language_data(count).Id & " selected>" & language_data(count).Name & "</option>")
                Else
                    result.Append("<option value=" & language_data(count).Id & ">" & language_data(count).Name & "</option>")
                End If
            Next
            result.Append("</select>")
            result.Append("</td>")
        End If
			If (PageAction.ToLower() <> "viewasynchlogfile") And (PageAction.ToLower() <> "viewpreapproval") And EnableContentTypes() Then
				GetAddMultiType()
				' If there is no content type from querystring check for the cookie and restore it to that value else all types

				If Request.QueryString(ContentTypeUrlParam) <> "" Then
					If IsNumeric(Request.QueryString(ContentTypeUrlParam)) Then
						lContentType = Request.QueryString(ContentTypeUrlParam)
						m_refContentApi.SetCookieValue(ContentTypeUrlParam, lContentType)
					End If
            ElseIf Ektron.Cms.CommonApi.GetEcmCookie()(ContentTypeUrlParam) <> "" Then
                If IsNumeric(Ektron.Cms.CommonApi.GetEcmCookie()(ContentTypeUrlParam)) Then
                    lContentType = CLng(Ektron.Cms.CommonApi.GetEcmCookie()(ContentTypeUrlParam))
                End If
				End If
        End If
		result.Append("<td>")
			result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
		result.Append("</tr></table>")
			htmToolBar.InnerHtml = result.ToString
	End Sub
    Public Function GetAddMultiType() As Long
        ' gets ID for "add multiple" asset type
        GetAddMultiType = 0
        Dim count As Integer
        asset_data = m_refContentApi.GetAssetSupertypes()
        If (Not asset_data Is Nothing) Then

            For count = 0 To asset_data.Length - 1
                If (ManagedAsset_Min <= asset_data(count).TypeId And asset_data(count).TypeId <= ManagedAsset_Max) Then
                    If "*" = asset_data(count).PluginType Then
                        GetAddMultiType = asset_data(count).TypeId
                    End If
                End If
            Next
        End If
    End Function
End Class
