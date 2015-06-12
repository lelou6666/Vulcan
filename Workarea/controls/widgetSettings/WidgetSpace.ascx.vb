Imports Ektron.Cms
Imports Ektron.Cms.API
Imports Ektron.Cms.Common.EkConstants
Imports System.Data
Imports Ektron.Cms.Workarea
Imports Ektron.Cms.Personalization
Imports System.Collections.Generic
Imports System.IO
Imports Ektron.Cms.Widget
Imports Microsoft.Security.Application

Partial Class Workarea_controls_widgetSettings_WidgetSpace
    Inherits System.Web.UI.UserControl
    Implements System.Web.UI.ICallbackEventHandler
    Protected m_refContentApi As New Ektron.Cms.ContentAPI()
    Protected m_refStyle As New StyleHelper()
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_strPageAction As String = "widgetsspace"
    Protected m_mode As String = ""
    Protected m_id As Long = 0
    Protected m_siteApi As New SiteAPI()


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        m_refMsg = m_refContentApi.EkMsgRef
        RegisterResources()

        'Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().
        If (Not String.IsNullOrEmpty(Request.QueryString("mode"))) Then
            m_mode = Request.QueryString("mode")
        End If

        If (Not String.IsNullOrEmpty(Request.QueryString("id"))) Then
            m_id = Convert.ToInt64(Request.QueryString("id"))
        End If

        If (Not Page.IsPostBack) Then
            Select Case m_mode.ToLower()
                Case "add"
                    AddWidgetsSpace()
                Case "edit"
                    AddWidgetsSpace()
                Case "remove"
                    RemoveWidgetsSpace()
                Case Else
                    ViewAllWidgetSpaces()
            End Select
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Using PageBuilder common text values
        lblSelectWidgets.Text = m_refMsg.GetMessage("lbl pagebuilder select widgets")
        widgetTitle.Text = m_refMsg.GetMessage("lbl pagebuilder widgets title")
        btnSelectNone.Text = m_refMsg.GetMessage("lbl pagebuilder select none")
        btnSelectNone.ToolTip = "#" & (btnSelectNone.Text).Replace(" ", "")
        btnSelectAll.Text = m_refMsg.GetMessage("lbl pagebuilder select all")
        btnSelectAll.ToolTip = "#" & (btnSelectAll.Text).Replace(" ", "")

        Css.RegisterCss(Me, "csslib/ektron.widgets.selector.css", "EktronWidgetsSelectorCss")

        'Gets all Widgets in Add mode
        If (Request.QueryString("mode") <> "" AndAlso Request.QueryString("mode") = "add") Then
            Ektron.Cms.Widget.WidgetTypeController.SyncWidgetsDirectory(m_refContentApi.RequestInformationRef.WidgetsPath)
            Dim model As New Ektron.Cms.Widget.WidgetTypeModel()
            Dim widgetTypes As Ektron.Cms.Widget.WidgetTypeData() = model.FindAll()
            repWidgetTypes.DataSource = AppendWidgetPath(widgetTypes)
            repWidgetTypes.DataBind()
        End If
       
        If (Page.IsPostBack()) Then
            Select Case m_mode.ToLower()
                Case "add"
                    doAddWidgetSpace()
                Case "edit"
                    doUpdateWidgetSpace()
                Case "remove"
                    doRemoveWidgetSpace()
            End Select
        End If
    End Sub

    Private Sub ViewAllWidgetSpaces()
        ViewAllToolbar()
        ViewSet.SetActiveView(ViewAll)
        Dim spaceData() As Ektron.Cms.Personalization.WidgetSpaceData
        spaceData = Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().FindAll()
        If (spaceData.Length = 0) Then
            lblNoWidgetSpaces.Text = m_refMsg.GetMessage("lbl no widget space setup")
            lblNoWidgetSpaces.Visible = True
        Else
            lblNoWidgetSpaces.Visible = False
        End If
        Me.ViewAllRepeater.DataSource = spaceData
        Me.ViewAllRepeater.DataBind()

    End Sub

    Private Sub AddWidgetsSpace()
        ViewSet.SetActiveView(ViewAdd)
        ViewAddEditToolbar()
        Page.SetFocus(txtTitle)
        DisplayAddWidgetSpace()
    End Sub

    Private Sub DisplayAddWidgetSpace()
        lblWidgetsSpaceTitle.Text = m_refMsg.GetMessage("generic title label")
        ltrGroupSpace.Text = m_refMsg.GetMessage("group space label")
    End Sub

    Private Sub RemoveWidgetsSpace()
        ViewSet.SetActiveView(ViewRemove)
        ViewRemoveToolbar()
        Dim spaceData() As Ektron.Cms.Personalization.WidgetSpaceData
        spaceData = Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().FindAll()
        If (spaceData.Length = 0) Then
            lblNoWidgetSpaces.Text = m_refMsg.GetMessage("lbl no widget space setup")
            lblNoWidgetSpaces.Visible = True
        Else
            lblNoWidgetSpaces.Visible = False
        End If
        Me.viewAllForRemove.DataSource = spaceData
        Me.viewAllForRemove.DataBind()
    End Sub

#Region "Actions"
    Private Sub doAddWidgetSpace()
        Dim widgetSpace As WidgetSpaceData = Nothing
        Dim title As String = ""
        Dim scope As Ektron.Cms.Personalization.WidgetSpaceScope = WidgetSpaceScope.User
        Dim model As New Ektron.Cms.Personalization.WidgetSpaceModel()
        Dim widgetSpaceCreated As Boolean = False
        If (chkGroupSpace.checked) Then
            scope = Ektron.Cms.Personalization.WidgetSpaceScope.CommunityGroup
        End If
        title = AntiXss.HtmlEncode(Request.Form(txtTitle.UniqueID))
        widgetSpaceCreated = Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().Create(title, scope, widgetSpace)
        If (widgetSpaceCreated) Then
            'Adding widgetSpace_To_widgets association
            For Each Key As String In Request.Form.AllKeys
                If Key.StartsWith("widget") Then
                    Try
                        model.AddWidgetSpaceAssociation(widgetSpace.ID, Long.Parse(Key.Substring(6)))
                    Catch ex As Exception
                        EkException.ThrowException(ex)
                    End Try
                End If
            Next
        End If
        Response.Redirect("widgetsettings.aspx?action=widgetspace", False)
        'ViewAllWidgetSpaces()
    End Sub

    Private Sub doUpdateWidgetSpace()
        Dim widgetSpace As WidgetSpaceData = Nothing
        Dim title As String = ""
        Dim scope As Ektron.Cms.Personalization.WidgetSpaceScope = WidgetSpaceScope.User
        Dim model As New Ektron.Cms.Personalization.WidgetSpaceModel()
        If (chkGroupSpace.Checked) Then
            scope = Ektron.Cms.Personalization.WidgetSpaceScope.CommunityGroup
        Else
            Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().FindByID(m_id, widgetSpace)
            scope = widgetSpace.Scope
        End If
        title = AntiXss.HtmlEncode(Request.Form(txtTitle.UniqueID))

        Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().Update(m_id, scope, title)
        model.RemoveAllWidgetSpaceAssociations(m_id)
        For Each Key As String In Request.Form.AllKeys
            If Key.StartsWith("widget") Then
                Try
                    model.AddWidgetSpaceAssociation(m_id, Long.Parse(Key.Substring(6)))
                Catch ex As Exception
                    EkException.ThrowException(ex)
                End Try
            End If
        Next

        Response.Redirect("widgetsettings.aspx?action=widgetspace", False)
        'ViewAllWidgetSpaces()
    End Sub

    Private Sub doRemoveWidgetSpace()
        Dim widgetSpaces() As Ektron.Cms.Personalization.WidgetSpaceData = Nothing
        Dim data As Ektron.Cms.Personalization.WidgetSpaceData = Nothing
        widgetSpaces = Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().FindAll()
        For Each data In widgetSpaces
            Dim check As String = Request.Form("chkSpace" & data.ID)
            If (check IsNot Nothing AndAlso check = "on") Then
                Ektron.Cms.Personalization.WidgetSpaceFactory.GetModel().Remove(data.ID)
            End If
        Next
        Response.Redirect("widgetsettings.aspx?action=widgetspace", False)
        'ViewAllWidgetSpaces()
    End Sub
#End Region

#Region "Toolbars"
    Private Sub ViewAllToolbar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl widgets space"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath & "../UI/Icons/add.png", "Widgetsettings.aspx?action=widgetsspace&mode=add", m_refMsg.GetMessage("lbl add widgets space title"), m_refMsg.GetMessage("lbl add widgets space alt"), ""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath & "../UI/Icons/remove.png", "Widgetsettings.aspx?action=widgetsspace&mode=remove", m_refMsg.GetMessage("lbl remove widgets space title"), m_refMsg.GetMessage("lbl remove widgets space alt"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub ViewAddEditToolbar()
        Dim result As New System.Text.StringBuilder
        Dim toolTip1 As String = m_refMsg.GetMessage("lbl add widgets space title")
        Dim toolTip2 As String = m_refMsg.GetMessage("lbl add widgets space alt")
        If (m_mode.ToLower() <> "add") Then
            toolTip1 = m_refMsg.GetMessage("lbl save new widgets space title")
            toolTip2 = m_refMsg.GetMessage("lbl save new widgets space alt")
        End If
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl widgets space"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath & "../UI/Icons/save.png", "#", toolTip1, toolTip2, "onclick=""return VerifyWidgetsSpace('" & m_mode & "', " & m_id & ");"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath & "../UI/Icons/back.png", "Widgetsettings.aspx?action=widgetsspace", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("alt back button text"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub ViewRemoveToolbar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl widgets space"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath & "../UI/Icons/delete.png", "#", m_refMsg.GetMessage("lbl delete widgets space title"), m_refMsg.GetMessage("lbl delete widgets space alt"), "onclick=""return VerifyWidgetsSpace('remove', 0);"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath & "../UI/Icons/back.png", "Widgetsettings.aspx?action=widgetsspace", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("alt back button text"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
#End Region

    Public Function GetCallbackResult() As String Implements System.Web.UI.ICallbackEventHandler.GetCallbackResult
        Return ""
    End Function

    Public Sub RaiseCallbackEvent(ByVal eventArgument As String) Implements System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent

    End Sub
   
    Protected Sub editButton_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        m_mode = "edit"
        m_id = Convert.ToInt64(CType(sender, ImageButton).CommandArgument)
        AddWidgetsSpace()
        Dim spaceData As WidgetSpaceData = Nothing
        Dim model As New Ektron.Cms.Personalization.WidgetSpaceModel()
        Dim widgetTypes As Ektron.Cms.Widget.WidgetTypeData()
        WidgetSpaceFactory.GetModel().FindByID(m_id, spaceData)
        'Get widgets based on scope in Edit Mode
        If spaceData.Scope = Ektron.Cms.Personalization.WidgetSpaceScope.WorkareaDashboard Then
            tr_groupSpace.Visible = False
            widgetTypes = WidgetTypeFactory.GetModel().FindAll(WidgetSpaceScope.WorkareaDashboard)
        Else
            Ektron.Cms.Widget.WidgetTypeController.SyncWidgetsDirectory(m_refContentApi.RequestInformationRef.WidgetsPath)
            widgetTypes = WidgetTypeFactory.GetModel().FindAll()
        End If
        repWidgetTypes.DataSource = AppendWidgetPath(widgetTypes)
        repWidgetTypes.DataBind()
        If (spaceData IsNot Nothing) Then
            txtTitle.Text = Server.HtmlDecode(spaceData.Title)
            chkGroupSpace.Checked = IIf(spaceData.Scope = Ektron.Cms.Personalization.WidgetSpaceScope.CommunityGroup, True, False)
            ViewSet.SetActiveView(Me.ViewAdd)
        End If

        'Sync earlier selected widgets during edit mode
        Dim selectedWidgets As Ektron.Cms.Widget.WidgetTypeData() = model.GetAssociatedWidgetTypesByWidgetSpaceID(m_id)
        Dim widgetIds As New List(Of String)
        For Each widget As Ektron.Cms.Widget.WidgetTypeData In selectedWidgets
            If spaceData.Scope = Ektron.Cms.Personalization.WidgetSpaceScope.WorkareaDashboard AndAlso widget.Scope = Ektron.Cms.Personalization.WidgetSpaceScope.WorkareaDashboard Then
                widgetIds.Add(widget.ID.ToString())
            ElseIf spaceData.Scope <> Ektron.Cms.Personalization.WidgetSpaceScope.WorkareaDashboard Then
                widgetIds.Add(widget.ID.ToString())
            End If
        Next
		System.Web.UI.ScriptManager.RegisterClientScriptBlock(Me.Page, Me.GetType(), "widgetSpaceSelectedIds", "Ektron.ready(function(){SelectWidgets([" + String.Join(", ", widgetIds.ToArray()) + "]);});", True)
    End Sub
    Public Function AppendWidgetPath(ByVal widgetTypes As Ektron.Cms.Widget.WidgetTypeData()) As WidgetTypeData()
        Dim widgetTypeList As New List(Of WidgetTypeData)()
        For Each widget As Ektron.Cms.Widget.WidgetTypeData In widgetTypes
            If widget.Scope = WidgetSpaceScope.WorkareaDashboard Then
                widget.ControlURL = m_refContentApi.RequestInformationRef.ApplicationPath + "widgets/" + widget.ControlURL.Replace("\", "/")
            Else
                widget.ControlURL = m_refContentApi.RequestInformationRef.WidgetsPath + widget.ControlURL
            End If
            Dim widgettypeData As New Ektron.Cms.Widget.WidgetTypeData
            Dim chkIsIWidget As Ektron.Cms.Widget.IWidget
            chkIsIWidget = TryCast(Page.LoadControl(widget.ControlURL), Ektron.Cms.Widget.IWidget)
            If chkIsIWidget IsNot Nothing Then
                widgettypeData.Active = widget.Active
                widgettypeData.ButtonText = widget.ButtonText
                widgettypeData.ControlURL = widget.ControlURL
                widgettypeData.ID = widget.ID
                widgettypeData.Scope = widget.Scope
                widgettypeData.Settings = widget.Settings
                widgettypeData.Title = widget.Title
                widgetTypeList.Add(widgettypeData)
            End If
        Next
        Return widgetTypeList.ToArray()
    End Function

    Private Sub RegisterResources()
        JS.RegisterJS(Me, JS.ManagedScript.EktronJS)
        JS.RegisterJS(Me, JS.ManagedScript.EktronWorkareaJS)
        JS.RegisterJS(Me, "controls/widgetsettings/ektron.widgetSpace.js", "EktronWidgetSpaceJS")
    End Sub
End Class
