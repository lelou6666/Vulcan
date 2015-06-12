Imports System.Threading
Imports System.IO
Imports System.Xml
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Content
Imports Ektron.Cms.UI.CommonUI

Partial Class BadLinkCheck
    Inherits System.Web.UI.Page
    Protected m_RequestInfo As EkRequestInformation = Nothing
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As EkMessageHelper

    Protected Sub btnCheck_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnCheck.Click
        If (txtURL.Text <> "") Then
            If (txtURL.Text.IndexOf("://") < 0) Then
                txtURL.Text = "http://" + txtURL.Text
            End If
            EkThreads.URLCheckerClass.CheckURL = txtURL.Text
            EkThreads.URLCheckerClass.Debug = True
        End If
        Dim m_AppRef As ApplicationAPI = New ApplicationAPI
        EkThreads.URLCheckerClass.getInstance().Start(m_AppRef.RequestInformationRef, Request.Url.Authority)
        Dim i As Integer
        For i = 1 To 20
            Thread.Sleep(100)  ' give thread some time to spin up
            If (EkThreads.URLCheckerClass.ThreadRunning) Then
                Exit For
            End If
        Next
        Response.Redirect("BadLinkCheck.aspx", False)
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnCancel.Click
        EkThreads.URLCheckerClass.Cancel = True
        ' give thread some time to stop
        Dim i As Integer
        For i = 1 To 20
            Thread.Sleep(100)  ' give thread some time to shut down
            If (Not EkThreads.URLCheckerClass.ThreadRunning) Then
                Exit For
            End If
        Next
        Response.Redirect("BadLinkCheck.aspx", False)
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Dim IsRunning As Boolean = EkThreads.URLCheckerClass.ThreadRunning
        StyleSheetJS.Text = m_refStyle.GetClientScript
        Dim m_refSiteApi As New SiteAPI
        m_refMsg = m_refSiteApi.EkMsgRef
        If (m_RequestInfo Is Nothing) Then
            Dim refContentApi As New ContentAPI
            m_RequestInfo = refContentApi.RequestInformationRef
        End If
        RegisterResources()

        If m_RequestInfo.IsMembershipUser Or m_RequestInfo.UserId = 0 Then
            Response.Redirect("blank.htm", False)
            Exit Sub
        End If

        If (IsRunning) Then
            Response.AppendHeader("Refresh", "5")
        End If

        ' set initial values of fields on page
        If (Page.IsPostBack = False) Then
            If (IsRunning) Then
                checkWrapper.Visible = False
                btnCheck.Visible = False
                printWrapper.Visible = False
                btnPrint.Visible = False
                cancelWrapper.Visible = True
                btnCancel.Visible = True
                lnkTabTestURL.Visible = False
                lnkTabStatus.Enabled = False
                ' purge page state
                HttpContext.Current.Session("_PAGE_STATE_" + Request.Url.AbsolutePath) = Nothing
            Else
                btnCancel.Visible = False
                cancelWrapper.Visible = False
            End If

            If (ConfigurationManager.AppSettings("ek_DebugLinkCheck") <> "True") Then
                lnkTabTestURL.Visible = False
            End If

            If (Not IsRunning) Then
                txtStatus.Text = m_refMsg.GetMessage("txt linkcheck idle")
            Else
                txtStatus.Text = EkThreads.URLCheckerClass.ThreadStatus
            End If

            If (EkThreads.URLCheckerClass.ThreadLog.Length = 0) Then
                Dim m_AppRef As ApplicationAPI = New ApplicationAPI
                Dim reportfile As String = m_AppRef.RequestInformationRef.PhysicalAppPath + "ekbadlinkrpt.html"
                If (File.Exists(reportfile)) Then
                    Dim oRead As System.IO.StreamReader
                    Try
                        oRead = System.IO.File.OpenText(reportfile)
                        EkThreads.URLCheckerClass.ThreadLog = New StringBuilder(oRead.ReadToEnd())
                    Catch
                        ' ignore errors reading from report file
                    End Try
                Else
                    txtStatus.Text = ""     ' don't display status if it was never run
                End If
            End If

            If (EkThreads.URLCheckerClass.ThreadLog.Length > 0) Then
                txtReport.Text = txtReport.Text
                If (IsRunning) Then
                    txtReport.Text = txtReport.Text + "<ul style='margin: .5em 2em'>" + _
                        "<li>Objects Checked: " + CStr(EkThreads.URLCheckerClass.CountObjsChecked)
                    txtReport.Text = txtReport.Text + "</ li>" + _
                        "<li>Links Checked: " + CStr(EkThreads.URLCheckerClass.CountLinksChecked)
                    txtReport.Text = txtReport.Text + "</li>" + _
                        "<li>Bad Links: " + CStr(EkThreads.URLCheckerClass.CountBadLinks) + "</li></ul>"

                    If (EkThreads.URLCheckerClass.CountBadLinks > 500) Then
                        txtReport.Text = txtReport.Text + "Too many errors.  Please wait for report to be completed."
                    Else
                        txtReport.Text = txtReport.Text + EkThreads.URLCheckerClass.ThreadLog.ToString()
                    End If
                Else
                    txtReport.Text = txtReport.Text + "<span id=""ReportDataGrid"">"
                    txtReport.Text = txtReport.Text + "<span id=""viewApprovalList_ViewGrid"">"
                    txtReport.Text = txtReport.Text + "<table><tr><td>"
                    txtReport.Text = txtReport.Text + EkThreads.URLCheckerClass.ThreadLog.ToString()
                    txtReport.Text = txtReport.Text + "</td></tr></table>"
                    txtReport.Text = txtReport.Text + "</span>"
                    txtReport.Text = txtReport.Text + "</span>"
                End If
            End If

            ' handle localization text
            Dim m_refStyle As New StyleHelper
            If (Not IsRunning) Then
                m_refStyle.MakeToolbarButton(btnCheck, _
                    m_refMsg.GetMessage("alt linkcheck button text"), _
                    m_refMsg.GetMessage("alt linkcheck button text"))
                m_refStyle.MakeToolbarButton(btnPrint, _
                    m_refMsg.GetMessage("btn print"), _
                    m_refMsg.GetMessage("btn print"))
            Else
                m_refStyle.MakeToolbarButton(btnCancel, _
                    m_refMsg.GetMessage("generic cancel"), _
                    m_refMsg.GetMessage("generic cancel"))
            End If
            lnkTabStatus.Text = m_refMsg.GetMessage("tab linkcheck status")
            lnkTabTestURL.Text = m_refMsg.GetMessage("tab linkcheck testurl")
            lblStatus.Text = m_refMsg.GetMessage("lbl linkcheck status")
            lblURL.Text = m_refMsg.GetMessage("lbl linkcheck testurl")
            btnHelp.Text = m_refStyle.GetHelpButton("badlinkcheck")
        End If
    End Sub


    Protected Sub lnkTabStatus_Click(ByVal sender As Object, ByVal e As EventArgs)
        If (EkThreads.URLCheckerClass.ThreadRunning) Then
            Response.Redirect("BadLinkCheck.aspx", False)
        End If
        MultiView1.SetActiveView(Status)
        lnkTabStatus.BackColor = System.Drawing.Color.White
        lnkTabTestURL.BackColor = System.Drawing.Color.FromArgb(&HAD, &HC5, &HEF)
    End Sub

    Protected Sub lnkTabTestURL_Click(ByVal sender As Object, ByVal e As EventArgs)
        If (EkThreads.URLCheckerClass.ThreadRunning) Then
            Response.Redirect("BadLinkCheck.aspx", False)
        End If
        MultiView1.SetActiveView(TestURL)
        lnkTabStatus.BackColor = System.Drawing.Color.FromArgb(&HAD, &HC5, &HEF)
        lnkTabTestURL.BackColor = System.Drawing.Color.White
    End Sub
    Private Sub RegisterResources()
        'Register CSS
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        API.Css.RegisterCss(Me, m_RequestInfo.ApplicationPath & "csslib/tabui.css", "EktronTabUICSS")
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        'Register JS
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
    End Sub
End Class