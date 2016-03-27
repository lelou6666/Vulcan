Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Widget

Partial Class Workarea_controls_widgetSettings_WidgetSync
    Inherits System.Web.UI.UserControl

    Protected m_refContentApi As New Ektron.Cms.ContentAPI()
    Protected m_refStyle As New StyleHelper()
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_strPageAction As String = "syncwidgets"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = m_refContentApi.EkMsgRef
        RegisterResources()
        Toolbar()
        grdWidgets.Columns(0).HeaderText = m_refMsg.GetMessage("lbl widgets")

        lblNoWidgets.Visible = False
        If (Page.IsPostBack) Then
            If (System.IO.Directory.Exists(Server.MapPath(m_refContentApi.RequestInformationRef.WidgetsPath))) Then
                WidgetTypeController.SyncWidgetsDirectory()
                WidgetTypeController.SyncWidgetsDirectory(m_refContentApi.RequestInformationRef.WidgetsPath)
            Else
                lblNoWidgets.Text = m_refMsg.GetMessage("com: folder does not exist") & " " & m_refContentApi.RequestInformationRef.WidgetsPath
                lblNoWidgets.Visible = True
                grdWidgets.Visible = False
            End If
        End If
        Dim widgetTypes() As WidgetTypeData = WidgetTypeFactory.GetModel().FindAll()
        Array.Sort(widgetTypes, New SortWidgetComparer())

        If (widgetTypes.Length = 0) Then
            lblNoWidgets.Text = m_refMsg.GetMessage("lbl no widgets in cms")
            lblNoWidgets.Visible = True
        Else
            grdWidgets.DataSource = widgetTypes
            grdWidgets.DataBind()
        End If
    End Sub

    Private Sub Toolbar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(String.Format(m_refMsg.GetMessage("lbl sync widgets"), m_refContentApi.RequestInformationRef.SitePath & "widgets/"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppImgPath & "../UI/Icons/refresh.png", "#", String.Format(m_refMsg.GetMessage("lbl sync widgets"), m_refContentApi.RequestInformationRef.SitePath & "widgets/"), String.Format(m_refMsg.GetMessage("lbl sync widgets"), m_refContentApi.RequestInformationRef.SitePath & "widgets/"), "onclick=""return SyncWidgets();"""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(m_strPageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronModalJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)

        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronModalCss)
    End Sub

End Class

Public Class SortWidgetComparer
    Implements System.Collections.Generic.IComparer(Of WidgetTypeData)

    Public Function Compare(ByVal x As WidgetTypeData, ByVal y As WidgetTypeData) As Integer Implements System.Collections.Generic.IComparer(Of WidgetTypeData).Compare
        Return x.Title.CompareTo(y.Title)
    End Function
End Class

