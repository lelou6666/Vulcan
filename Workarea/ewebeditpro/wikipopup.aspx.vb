Imports Microsoft.VisualBasic
Imports ektron.Cms
Public Class wikipopup
    Inherits System.Web.UI.Page
    Protected m_refStyle As New StyleHelper
    Protected m_commonApi As CommonApi
    Protected m_refMsg As Common.EkMessageHelper
    Protected m_defaultFolderPath As String = ""
    Protected IsMac As Boolean = False
    Protected IsBrowserIE As Boolean = False
    Protected SelectedEditControl As String = ""

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim folderID As Long = -1
        Dim gtNavs As Collection = Nothing
        Dim fPath As String = ""
        Dim strJS As String = ""
        Dim wikiContTitle As String = ""
        Dim wikiContTarget As String = ""

        SelectedEditControl = Utilities.GetEditorPreference(Request)

        If "ContentDesigner" = SelectedEditControl Then
            ClientScript.RegisterClientScriptInclude("RadWindow", "../ContentDesigner/RadWindow.js")
            ClientScript.RegisterClientScriptBlock(Me.GetType(), "InitializeRadWindow", "InitializeRadWindow();", True)
        End If

        If (Request.Browser.Platform.IndexOf("Win") = -1) Then
            IsMac = True
        End If
        If (Request.Browser.Type.IndexOf("IE") <> -1) Then
            IsBrowserIE = True
        End If

        m_commonApi = New CommonApi()
        StyleSheetJS.Text = m_refStyle.GetClientScript()
        m_refMsg = m_commonApi.EkMsgRef
        ltContentTitle.Text = m_refMsg.GetMessage("generic article title label")
        divNewContentText.Text = m_refMsg.GetMessage("lbl new content")
        divdvRelatedContentText.Text = m_refMsg.GetMessage("lbl related content")
        searchButton.Text = m_refMsg.GetMessage("lbl go")

        If (Request.QueryString("wikititle") IsNot Nothing AndAlso Request.QueryString("wikititle") <> "") Then
            wikiContTitle = "wiki_cont_title = '" & Request.QueryString("wikititle").Replace("'", "\'") & "';" & vbCrLf
        End If

        If (Request.QueryString("target") IsNot Nothing AndAlso Request.QueryString("target") <> "") Then
            wikiContTarget = "wiki_link_target = '" & Request.QueryString("target").Replace("'", "\'") & "';" & vbCrLf
        End If
        Me.jsSearchRelatedContent.Text = "<script type=""text/javascript"">loadselectedtext();getContentByID();" & wikiContTitle & vbCrLf & wikiContTarget & "</script>"

        ToolBar()
        If (Request.QueryString("FolderID") IsNot Nothing AndAlso Request.QueryString("FolderID") <> "") Then
            folderID = Request.QueryString("FolderID")
        End If
        If (folderID > -1) Then
            gtNavs = m_commonApi.EkContentRef.GetFolderInfoWithPath(folderID)
            fPath = gtNavs("Path")
        End If
        If (fPath <> "") Then
            fPath = fPath.Replace("\", "\\")
            strJS = strJS & "ReturnChildValue(" & folderID & ",'" & fPath & "','');" & vbCrLf
        End If
        If (Request.QueryString("wikititle") IsNot Nothing AndAlso Request.QueryString("wikititle") <> "") Then
            strJS = strJS & "wiki_cont_title = '" & Request.QueryString("wikititle").Replace("'", "\'") & "';" & vbCrLf
        End If
        If (m_commonApi.RequestInformationRef.IsMembershipUser = 1) Then
            strJS = strJS & "document.getElementById(""a_change"").style.visibility =""hidden"";" & vbCrLf
            strJS = strJS & "document.getElementById(""a_none"").style.visibility =""hidden"";" & vbCrLf
        End If
        If (strJS <> "") Then
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "setdefaultfolderid", strJS, True)
        End If

        RegisterResources()

    End Sub

    Private Sub ToolBar()
        Dim result As New System.Text.StringBuilder
        Dim backup As String = ""
        Dim close As String
        close = Request.QueryString("close")
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("title add edit Wiki Link"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_commonApi.AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("add title"), m_refMsg.GetMessage("add title"), "onclick=""return inserthyperlink();"""))
        If (close <> "true") Then
            If "ContentDesigner" = SelectedEditControl Then
                result.Append(m_refStyle.GetButtonEventsWCaption(m_commonApi.AppImgPath & "../UI/Icons/cancel.png", "#", m_refMsg.GetMessage("close title"), m_refMsg.GetMessage("close title"), "onclick=""CloseDlg();"""))
            Else
                result.Append(m_refStyle.GetButtonEventsWCaption(m_commonApi.AppImgPath & "../UI/Icons/cancel.png", "#", m_refMsg.GetMessage("close title"), m_refMsg.GetMessage("close title"), "onclick=""self.close();"""))
            End If
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("addwikilink"))
        result.Append("</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub

    Private Sub RegisterResources()
        ' Register JS
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUICoreJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronUITabsJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
        API.JS.RegisterJS(Me, m_commonApi.AppPath & "java/eweputil.js", "EktronEWeputilJS")
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)

        ' Register CSS
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
    End Sub
End Class
