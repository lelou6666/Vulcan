Imports System.Data
Imports System.DateTime
Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Content
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.CustomFieldsApi
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration

Partial Class controls_Community_Components_UserSelectControl
    Inherits System.Web.UI.UserControl

    Protected m_userId As Long = 0
    Protected m_refUserApi As New UserAPI
    Protected m_callbackresult As String = ""
    Protected m_recipientsPage As Integer = 1
    Protected m_friendsOnly As Boolean = False
    Protected m_singleSelection As Boolean = False
    Protected m_refContentAPI As New ContentAPI
    Protected m_user_list As UserData() = Array.CreateInstance(GetType(Ektron.Cms.UserData), 0)
    Protected m_strSearchText As String = ""
    Protected m_strKeyWords As String = ""
    Protected m_intTotalPages As Integer = 0
    Protected m_searchMode As String = "display_name"
    'Protected m_enableButtons As Boolean = False

    ''' <summary>
    ''' When true, narrows user listing to just friends of the current user.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FriendsOnly() As Boolean
        Get
            Return (m_friendsOnly)
        End Get
        Set(ByVal value As Boolean)
            m_friendsOnly = value
        End Set
    End Property

    ''' <summary>
    ''' If true, only allows a single user to be selected, otherwise no limit (default).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SingleSelection() As Boolean
        Get
            Return (m_singleSelection)
        End Get
        Set(ByVal value As Boolean)
            m_singleSelection = value
        End Set
    End Property

    'Public Property EnableButtons() As Boolean
    '    Get
    '        Return (m_enableButtons)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        m_enableButtons = value
    '    End Set
    'End Property

    ''' <summary>
    ''' Returns the user search control id, to be used when calling 
    ''' client script methods UserSelectCtl_GetSelectUsers() or 
    ''' UserSelectCtl_GetUserName().
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ControlId() As String
        Get
            Return (usersel_comsearch.ClientID)
        End Get
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_userId = m_refContentAPI.RequestInformationRef.UserId

        usersel_comsearch_jsinit.Text = "<script type=""text/javascript"">var usersel_comsearch_ClientID = '" + usersel_comsearch.ClientID + "'; var usersel_comsearch_SingleSelection = " + SingleSelection.ToString.ToLower() + ";</script>"

        usersel_comsearch.TemplateGroupParamName = "id"
        usersel_comsearch.TemplateUserParamName = "id"
        usersel_comsearch.TemplateTarget = Ektron.Cms.Controls.EkWebControl.ItemLinkTargets._blank
        usersel_comsearch.MembersOnly = "false"
        usersel_comsearch.MaxTagCount = "100"
        usersel_comsearch.EnableMap = "false"
        usersel_comsearch.UserTaxonomyID = "440"
        usersel_comsearch.PageSize = 4
        usersel_comsearch.TemplateUserProfile = m_refContentAPI.AppPath + "UserProfile.aspx"
        usersel_comsearch.DisplayXslt = m_refContentAPI.AppPath + "/xslt/ekUserSelect.xsl"
        usersel_comsearch.FriendsOnly = FriendsOnly

        If (Not Page.IsCallback) Then
            EmitJavascript()
        End If
    End Sub

    Protected Function GetMessage(ByVal resource As String) As String
        Return m_refContentAPI.EkMsgRef.GetMessage(resource)
    End Function

    Protected Sub EmitJavascript()
        If ((Not IsNothing(Page)) AndAlso Not Page.ClientScript.IsClientScriptBlockRegistered("UserSelectCtl_AjaxJavascript")) Then
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "UserSelectCtl_AjaxJavascript", GetInitializationJavascript())
        End If
    End Sub

    Protected Function GetInitializationJavascript() As String
        Dim result As String = ""
        Dim sb As New System.Text.StringBuilder()

        Try
            If (Not (Page.IsCallback)) Then

                sb.Append("<script type=""text/javascript"">" + Environment.NewLine)
                sb.Append("<!-- \n " + Environment.NewLine)
                'sb.Append("" + Environment.NewLine)
                sb.Append("//-->" + Environment.NewLine)
                sb.Append("</script>" + Environment.NewLine)
            End If

        Catch ex As Exception

        Finally
            result = sb.ToString()
            sb = Nothing
        End Try

        Return (result)
    End Function

End Class
