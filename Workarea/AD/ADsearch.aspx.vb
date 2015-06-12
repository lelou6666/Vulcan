Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea
Imports System.Collections.Generic
Imports Ektron.Newtonsoft
Imports Ektron.Newtonsoft.Json

Namespace Ektron.Workarea.ActiveDirectory

    Public Class SelectedUser

        Private _Username As String
        Private _Domain As String

        Public Property Username() As String
            Get
                Return _Username
            End Get
            Set(ByVal value As String)
                _Username = value
            End Set
        End Property

        Public Property Domain() As String
            Get
                Return _Domain
            End Get
            Set(ByVal value As String)
                _Domain = value
            End Set
        End Property

    End Class

    Partial Class AddUsers
        Inherits workareabase

#Region "Members"

        Protected _CommonApi As New CommonApi
        Protected _SiteApi As New SiteAPI
        Protected _UserApi As New UserAPI
        Protected _GroupType As Integer = 0
        Protected _GroupId As Long = 2
        Protected _SettingsData As SettingsData
        Protected _UserData As UserData() = Array.CreateInstance(GetType(Ektron.Cms.UserData), 0)
        Protected m_strUserName As String = ""
        Protected m_strFirstName As String = ""
        Protected m_strLastName As String = ""
        Protected _Domain As String = ""
        Protected _IsSaveAdded As Boolean = False
        Protected _DomainData As DomainData()
        Protected _MessageHelper As Ektron.Cms.Common.EkMessageHelper

#End Region

#Region "Events"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Response.CacheControl = "no-cache"
            Response.AddHeader("Pragma", "no-cache")
            Response.Expires = -1

            'register js/css
            RegisterResources()

            _GroupType = IIf(String.IsNullOrEmpty(Request.QueryString("grouptype")), 0, Convert.ToInt32(Request.QueryString("grouptype")))
            _GroupId = IIf(String.IsNullOrEmpty(Request.QueryString("groupid")), 2, Convert.ToInt64(Request.QueryString("groupid")))
            _SettingsData = _SiteApi.GetSiteVariables(_SiteApi.UserId)
            _MessageHelper = _CommonApi.EkMsgRef

            Me.litSuccess.Text = _MessageHelper.GetMessage("aduser add success")

        End Sub
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If (Not Page.IsPostBack) And ddlDomainName.Items.Count = 0 Then
                    _DomainData = _UserApi.GetDomains(0, 0)
                    If (Not (IsNothing(_DomainData))) AndAlso _CommonApi.RequestInformationRef.ADAdvancedConfig = False Then
                        ddlDomainName.Items.Add(New ListItem(MyBase.GetMessage("all domain select caption"), ""))
                    End If
                    Dim i As Integer
                    For i = 0 To _DomainData.Length - 1
                        ddlDomainName.Items.Add(New ListItem(_DomainData(i).Name, _DomainData(i).Name))
                    Next
                Else
                    If Not (Page.Session("user_list") Is Nothing) Then
                        _UserData = Page.Session("user_list")
                    End If
                End If

                
            Catch ex As Exception
                Dim sErr As String = ""
                sErr = ex.Message
                If sErr.IndexOf("[") > -1 Then
                    sErr = sErr.Substring(0, sErr.IndexOf("["))
                End If
                Utilities.ShowError(sErr)
            End Try
        End Sub
        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            If _UserData Is Nothing Then
                SetLabels("")
            Else
                If _UserData.Length > 0 Then
                    SetLabels("results")
                    BindDataGrid()
                    Me.dgAddADUser.Visible = True
                Else
                    SetLabels("")
                    Me.dgAddADUser.Visible = False
                    Me.uxPaging.Visible = False
                End If
            End If
        End Sub
        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
            Dim Sort As String = ""
            Dim sdAttributes As New System.Collections.Specialized.NameValueCollection 'New Collection
            Dim sdFilter As New System.Collections.Specialized.NameValueCollection  'New Collection

            Try
                sdAttributes.Add("UserName", "UserName")
                sdAttributes.Add("FirstName", "FirstName")
                sdAttributes.Add("LastName", "LastName")
                sdAttributes.Add("Domain", "Domain")

                m_strUserName = Trim(username.Value)
                m_strFirstName = Trim(firstname.Value)
                m_strLastName = Trim(lastname.Value)
                _Domain = Trim(ddlDomainName.SelectedValue)
                Sort = "UserName"

                If ((m_strUserName = "") And (m_strFirstName = "") And (m_strLastName = "")) Then
                    sdFilter.Add("UserName", "UserName")
                    sdFilter.Add("UserNameValue", "*")
                Else
                    If (m_strUserName <> "") Then
                        sdFilter.Add("UserName", "UserName")
                        sdFilter.Add("UserNameValue", m_strUserName) 'sdFilter.add (UserName,"UserNameValue")
                    End If
                    If (m_strFirstName <> "") Then
                        sdFilter.Add("FirstName", "FirstName")
                        sdFilter.Add("FirstNameValue", m_strFirstName)
                    End If
                    If (m_strLastName <> "") Then
                        sdFilter.Add("LastName", "LastName")
                        sdFilter.Add("LastNameValue", m_strLastName)
                    End If
                End If

                _UserData = _UserApi.GetAvailableADUsers(sdAttributes, sdFilter, Sort, _Domain)

                If Not (Page.Session("user_list") Is Nothing) Then
                    Page.Session.Remove("user_list")
                End If
                Page.Session.Add("user_list", _UserData)

                Me.dgAddADUser.Visible = True
                divAddUser.Visible = True
                divUsersAdded.Visible = False
            Catch ex As Exception
                Utilities.ShowError(ex.Message)
            End Try
        End Sub
        Protected Sub lbSave_Click(ByVal sender As System.Object, ByVal e As EventArgs)

            Try

                Dim selectedUsers As List(Of SelectedUser)
                selectedUsers = JsonConvert.DeserializeObject(Me.hdnSelectedItems.Value, GetType(List(Of SelectedUser)))

                Dim userNames As New Collection
                Dim domains As New Collection
                For Each user As SelectedUser In selectedUsers
                    If ((user.Username <> "") And (user.Domain <> "")) Then
                        userNames.Add(user.Username, userNames.Count)
                        domains.Add(user.Domain, userNames.Count)
                    End If
                Next

                If (_GroupType = 0) Then
                    _UserApi.AddADUsersToCMSByUsername(userNames, domains)
                Else
                    Dim ekUser As User.EkUser
                    ekUser = _UserApi.EkUserRef
                    ekUser.AddADmemberUsersToCmsByUsername(userNames, domains)
                End If

                Page.Session.Remove("user_list")
                _UserData = Nothing
                Me.hdnSelectedItems.Value = String.Empty

                divAddUser.Visible = False
                divUsersAdded.Visible = True

            Catch ex As Exception

                Utilities.ShowError(ex.Message)

            End Try

        End Sub
        Public Sub dgAddADUser_ItemDataBound(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)

            Select Case e.Item.ItemType
                Case ListItemType.Header
                    'cell 0 - checkbox 
                    'cell 1 - username
                    'cell 2 - firstname
                    'cell 3 - lastname
                    'cell 4 - domain
                    CType(e.Item.Cells(1).FindControl("litUsernameHeader"), Literal).Text = "Username"
                    CType(e.Item.Cells(2).FindControl("litLastNameHeader"), Literal).Text = "Last Name"
                    CType(e.Item.Cells(3).FindControl("litFirstNameHeader"), Literal).Text = "First Name"
                    CType(e.Item.Cells(4).FindControl("litDomainHeader"), Literal).Text = "Domain"
                Case ListItemType.Item, ListItemType.AlternatingItem
                    CType(e.Item.Cells(0).FindControl("hdnUsername"), HtmlInputHidden).Value = CType(e.Item.DataItem, UserData).Username
                    CType(e.Item.Cells(0).FindControl("hdnDomain"), HtmlInputHidden).Value = CType(e.Item.DataItem, UserData).Domain
                    CType(e.Item.Cells(1).FindControl("litUsername"), Literal).Text = CType(e.Item.DataItem, UserData).Username
                    CType(e.Item.Cells(1).FindControl("litLastName"), Literal).Text = CType(e.Item.DataItem, UserData).LastName
                    CType(e.Item.Cells(1).FindControl("litFirstName"), Literal).Text = CType(e.Item.DataItem, UserData).FirstName
                    CType(e.Item.Cells(2).FindControl("litDomain"), Literal).Text = CType(e.Item.DataItem, UserData).Domain
            End Select
        End Sub

#End Region

#Region "Helpers"

        Private Sub BindDataGrid()

            Me.dgAddADUser.PageSize = _CommonApi.RequestInformationRef.PagingSize
            Me.dgAddADUser.CurrentPageIndex = Me.uxPaging.SelectedPage
            Me.dgAddADUser.DataSource = _UserData
            Me.dgAddADUser.DataBind()
            If Me.dgAddADUser.PageCount > 1 Then
                Me.uxPaging.TotalPages = Me.dgAddADUser.PageCount
                Me.uxPaging.CurrentPageIndex = Me.dgAddADUser.CurrentPageIndex
                Me.uxPaging.Visible = True
            Else
                Me.uxPaging.Visible = False
            End If

        End Sub
        Private Sub SetLabels(ByVal type As String)
            MyBase.Title = MyBase.GetMessage("view users in active directory msg")
            MyBase.SetTitleBarToMessage("view users in active directory msg")

            If type = "results" Then
                MyBase.AddButtonwithMessages(m_refContentApi.AppImgPath & "../UI/Icons/save.png", "#", "alt add button text (users)", "btn save", " onclick=""Ektron.Workarea.ActiveDirectory.AddUser.submit();return false;"" ")
                _IsSaveAdded = True
            End If

            MyBase.AddBackButton("../users.aspx?backaction=viewallusers&action=viewallusers&grouptype=" & _GroupType.ToString() & "&groupid=2&id=2&FromUsers=1")
            MyBase.AddHelpButton("editusers_ascx")
        End Sub
        Private Function LDAPMembers() As Boolean
            If (_GroupType = 1) Then 'member
                Return (_UserApi.RequestInformationRef.LDAPMembershipUser)
            ElseIf (_GroupType = 0) Then 'CMS user
                Return True
            End If
        End Function
#End Region

#Region "JS/CSS"

        Private Sub RegisterResources()
            Dim sbJS As New System.Text.StringBuilder()

            sbJS.Append("<script language=""JavaScript"">").Append(Environment.NewLine)
            sbJS.Append("	function toggleVisibility(me){").Append(Environment.NewLine)
            sbJS.Append("		if (me.style.visibility==""hidden""){").Append(Environment.NewLine)
            sbJS.Append("			me.style.visibility=""visible"";").Append(Environment.NewLine)
            sbJS.Append("			}").Append(Environment.NewLine)
            sbJS.Append("		else {").Append(Environment.NewLine)
            sbJS.Append("			me.style.visibility=""hidden"";").Append(Environment.NewLine)
            sbJS.Append("	    }").Append(Environment.NewLine)
            sbJS.Append("		(document.getElementById('rp')).value = 'submit';").Append(Environment.NewLine)
            sbJS.Append("		document.forms[0].submit();").Append(Environment.NewLine)
            sbJS.Append("		return false;").Append(Environment.NewLine)
            sbJS.Append("	}").Append(Environment.NewLine)

            sbJS.Append("		function CheckKeyValue(item, keys) {").Append(Environment.NewLine)
            sbJS.Append("			var keyArray = keys.split("","");").Append(Environment.NewLine)
            sbJS.Append("			for (var i = 0; i < keyArray.length; i++) {").Append(Environment.NewLine)
            sbJS.Append("				if ((document.layers) || ((!document.all) && (document.getElementById))) {").Append(Environment.NewLine)
            sbJS.Append("					if (item.which == keyArray[i]) {").Append(Environment.NewLine)
            sbJS.Append("						return false;").Append(Environment.NewLine)
            sbJS.Append("					}").Append(Environment.NewLine)
            sbJS.Append("				}").Append(Environment.NewLine)
            sbJS.Append("				else {").Append(Environment.NewLine)
            sbJS.Append("					if (event.keyCode == keyArray[i]) {").Append(Environment.NewLine)
            sbJS.Append("						return false;").Append(Environment.NewLine)
            sbJS.Append("					}").Append(Environment.NewLine)
            sbJS.Append("				}").Append(Environment.NewLine)
            sbJS.Append("			}").Append(Environment.NewLine)
            sbJS.Append("		}").Append(Environment.NewLine)

            sbJS.Append("</script>").Append(Environment.NewLine)
            sbJS.Append("").Append(Environment.NewLine)

            ltr_js.Text = sbJS.ToString()

            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
            Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJsonJS)
            Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
            Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
            Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        End Sub

#End Region

    End Class
End Namespace