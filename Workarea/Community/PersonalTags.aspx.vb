Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.Content
Imports System.Data
Imports Ektron.Cms.Workarea

Partial Class Community_PersonalTags
    Inherits workareabase

	Protected m_action As String = ""
	Protected m_mode As String
	Protected m_appImgPath As String
	Protected m_ContentLanguage As Integer
    Protected m_userId As Long
    Protected m_id As Long
	Protected m_refUserApi As UserAPI
	Protected m_refCommonAPI As CommonApi

	Public Sub New()
		m_refStyle = New StyleHelper
		m_mode = ""
		m_userId = 0
		m_id = 0
		m_refUserApi = New UserAPI
		m_refCommonAPI = New CommonApi()
		m_refContentApi = New ContentAPI
        m_refMsg = RefCommonAPI.EkMsgRef

        Utilities.SetLanguage(m_refCommonAPI)
	End Sub

	Public Property RefStyle() As StyleHelper
		Get
			Return (m_refStyle)
		End Get
		Set(ByVal value As StyleHelper)
			m_refStyle = value
		End Set
	End Property
	Public Property RefMsg() As Ektron.Cms.Common.EkMessageHelper
		Get
			Return (m_refMsg)
		End Get
		Set(ByVal value As Ektron.Cms.Common.EkMessageHelper)
			m_refMsg = value
		End Set
	End Property
	Public Property RefUserApi() As UserAPI
		Get
			Return (m_refUserApi)
		End Get
		Set(ByVal value As UserAPI)
			m_refUserApi = value
		End Set
	End Property
	Public Property RefCommonAPI() As CommonApi
		Get
			Return (m_refCommonAPI)
		End Get
		Set(ByVal value As CommonApi)
			m_refCommonAPI = value
		End Set
	End Property
	Public Property RefContentApi() As ContentAPI
		Get
			Return (m_refContentApi)
		End Get
		Set(ByVal value As ContentAPI)
			m_refContentApi = value
		End Set
	End Property

	Public Property Action() As String
		Get
			Return (m_action)
		End Get
		Set(ByVal value As String)
			m_action = value
		End Set
	End Property

	Public Property Mode() As String
		Get
			Return (m_mode)
		End Get
		Set(ByVal value As String)
			m_mode = value
		End Set
	End Property

    Public Shadows Property ContentLanguage() As Integer
        Get
            Return (m_refCommonAPI.ContentLanguage)
        End Get
        Set(ByVal value As Integer)
            m_refCommonAPI.ContentLanguage = value
        End Set
    End Property

    Public Property UserId() As Long
        Get
            Return (m_userId)
        End Get
        Set(ByVal value As Long)
            m_userId = value
        End Set
    End Property

    Public Property TagId() As Long
        Get
            Return (m_id)
        End Get
        Set(ByVal value As Long)
            m_id = value
        End Set
    End Property

    Public Shadows Property AppImgPath() As String
        Get
            Return (m_appImgPath)
        End Get
        Set(ByVal value As String)
            m_appImgPath = value
        End Set
    End Property

    Protected Function CheckAccess() As Boolean
        If Me.m_refContentApi.IsLoggedIn() Then
            If Me.m_iID > 0 And Me.m_sPageAction = "delete" Then
                Dim mMemberStatus As Ektron.Cms.Common.EkEnumeration.GroupMemberStatus
                mMemberStatus = Me.m_refCommunityGroupApi.GetGroupMemberStatus(Me.m_iID, Me.m_refContentApi.UserId())
                Return (Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) OrElse mMemberStatus = Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.Leader)
            Else ' if logged in, can see this
                Return True
            End If
        End If

        Return False
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ctl As System.Web.UI.UserControl = Nothing


        Try
            Utilities.ValidateUserLogin()
            If Not CheckAccess() Then
                Throw New Exception(Me.GetMessage("err communityaddedit no access"))
            End If

            RegisterResources()
            ptagsJSContainerLit.Text = RefStyle.GetClientScript
            UserId = RefCommonAPI.RequestInformationRef().UserId
            AppImgPath = RefUserApi.AppImgPath
            ContentLanguage = RefCommonAPI.RequestInformationRef.ContentLanguage
            If (Not IsNothing(Request.QueryString("action"))) Then
                Action = Request.QueryString("action").ToLower()
            End If
            If (Not IsNothing(Request.QueryString("mode"))) Then
                Mode = Request.QueryString("mode").ToLower()
            End If
            If (Not IsNothing(Request.QueryString("id"))) Then
                TagId = Request.QueryString("id").ToLower()
            End If

            Select Case m_action
                Case "addtag"
                    ctl = LoadControl("../Controls/Community/PersonalTags/EditTag.ascx")
                Case "edittag"
                    ctl = LoadControl("../Controls/Community/PersonalTags/EditTag.ascx")
                    'CType(ctl, controls_Community_PersonalTags_EditTag).TagId = 
                Case "viewall"
                    ctl = LoadControl("../Controls/Community/PersonalTags/ViewAllTags.ascx")
                Case "viewtag"
                    ctl = LoadControl("../Controls/Community/PersonalTags/ViewTag.ascx")
                Case "viewdefaulttags"
                    ctl = LoadControl("../Controls/Community/PersonalTags/TagDefaults.ascx")
                Case Else
                    ctl = LoadControl("../Controls/Community/PersonalTags/ViewAllTags.ascx")
            End Select
            If (Not IsNothing(ctl)) Then
                FindControl("PTagsCtlHolder").Controls.Add(ctl)
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message & ex.StackTrace)
        End Try
    End Sub
    Protected Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronCommunityCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)

    End Sub
End Class
