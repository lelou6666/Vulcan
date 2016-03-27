Imports Ektron.Cms
Imports Ektron.Cms.UI.CommonUI

Partial Class editarea
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    'Protected WithEvents workareatop As System.Web.UI.HtmlControls.HtmlGenericControl
    'Protected WithEvents main As System.Web.UI.HtmlControls.HtmlGenericControl

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub
    Protected m_refContentApi As New ContentAPI
    Protected m_refMsg As Common.EkMessageHelper
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'register page components
        Me.RegisterJS()
        Me.RegisterCSS()

        'set javascript strings
        Me.SetJavascriptStrings()

        'Put user code to initialize the page here
        Dim strTitle As String = ""
        Dim AddToCollectionType As String = ""
        Dim FromEE As String = ""
        Dim ContentLanguage As Integer = -1
        Dim mycollection As String = ""
        Dim contentType As String = ""
        Dim bShowLogin As Boolean = False
        Dim sXid As String = ""
        Dim updateFieldId As String = ""
        Dim taxonomyId As String = ""
        Dim seltaxonomyId As String = ""

        If (System.Web.HttpContext.Current.Request.QueryString("TaxonomyId") IsNot Nothing AndAlso System.Web.HttpContext.Current.Request.QueryString("TaxonomyId") <> "") Then
            taxonomyId = "&TaxonomyId=" & System.Web.HttpContext.Current.Request.QueryString("TaxonomyId").ToString()
        End If
        If (System.Web.HttpContext.Current.Request.QueryString("SelTaxonomyId") IsNot Nothing AndAlso System.Web.HttpContext.Current.Request.QueryString("SelTaxonomyId") <> "") Then
            seltaxonomyId = "&SelTaxonomyId=" & System.Web.HttpContext.Current.Request.QueryString("SelTaxonomyId").ToString()
        End If
        m_refMsg = m_refContentApi.EkMsgRef
        If (Request.QueryString("LangType") <> "") Then
            ContentLanguage = Request.QueryString("LangType")
            m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
        Else
            If CStr(m_refContentApi.GetCookieValue("LastValidLanguageID")) <> "" Then
                ContentLanguage = m_refContentApi.GetCookieValue("LastValidLanguageID")
            End If
        End If
        If (Request.QueryString("ShowLogin") = "true") Then
            bShowLogin = True
        End If
        If (Not bShowLogin) Then
            'Make sure the user is logged in. If not forward user to login page.
            If ((m_refContentApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn") = False) Then
                bShowLogin = True
            End If
        End If
        If (Request.QueryString("mycollection") <> "") Then
            mycollection = "&mycollection=" & Request.QueryString("mycollection")
        Else
            If (Request.Form("mycollection") <> "") Then
                mycollection = "&mycollection=" & Request.Form("mycollection")
            End If
        End If
        If (Request.QueryString("ContType") <> "" AndAlso IsNumeric(Request.QueryString("ContType"))) Then
            Dim iContentType As Integer = Common.EkFunctions.ReadDbWholeNumber(Request.QueryString("ContType"))
            contentType = String.Format("&ContType={0}", iContentType)
        End If
        If (Request.QueryString("addto") <> "") Then
            AddToCollectionType = "&addto=" & Request.QueryString("addto")
        Else
            If (Request.Form("addto") <> "") Then
                AddToCollectionType = "&addto=" & Request.Form("addto")
            End If
        End If
        If (Request.QueryString("FromEE") <> "") Then
            FromEE = "&FromEE=" & Request.QueryString("FromEE")
        Else
            FromEE = ""
        End If
        If (Request.QueryString("ctlupdateid") <> "") Then
            updateFieldId = "&ctlupdateid=" & Request.QueryString("ctlupdateid") & "&ctlmarkup=" & Request.QueryString("ctlmarkup") & "&cltid=" & Request.QueryString("cltid") & "&ctltype=" & Request.QueryString("ctltype")
        End If
        If (Request.QueryString("cacheidentifier") IsNot Nothing AndAlso Request.QueryString("cacheidentifier") <> "") Then
            updateFieldId = updateFieldId & "&cacheidentifier=" & Request.QueryString("cacheidentifier")
        End If
        If (Request.QueryString("type") = "add") Then
            Dim id As String
            Dim cFolder As FolderData
            id = Request.QueryString("id")
            If Request.QueryString("xid") <> "" Then
                sXid = "&xid=" & Request.QueryString("xid")
            End If
            cFolder = m_refContentApi.GetFolderById(id)
            strTitle = m_refMsg.GetMessage("add content page title")
            strTitle = strTitle & " """ & cFolder.Name & """"
        Else
            strTitle = m_refMsg.GetMessage("edit content page title")
        End If
        Page.Title = m_refContentApi.AppName & " " & strTitle
        If (Request.QueryString("type") <> "add") Then
            workareatop.Attributes("src") = "workareatop.aspx?title=workarea_edit_top.gif"
        End If
        If bShowLogin Then
            Session("RedirectLnk") = "edit.aspx?content_id=" & Request.QueryString("content_id") & "&LangType=" & Request.QueryString("LangType") & sXid & "&id=" & Request.QueryString("id") & "&type=" & Request.QueryString("type") & "&enableFrmbar=" & Request.QueryString("enableFrmbar") & "&pullapproval=" & Request.QueryString("pullapproval") & "&dontcreatetask=" & Request.QueryString("dontcreatetask") & mycollection & contentType & AddToCollectionType & FromEE & updateFieldId & taxonomyId & seltaxonomyId
            ek_main.Attributes("src") = "login.aspx?fromLnkPg=1"
        Else
            ek_main.Attributes("src") = "edit.aspx?content_id=" & Request.QueryString("content_id") & "&LangType=" & Request.QueryString("LangType") & sXid & "&id=" & Request.QueryString("id") & "&type=" & Request.QueryString("type") & "&control=" & Request.QueryString("control") & "&buttonid=" & Request.QueryString("buttonid") & "&enableFrmbar=" & Request.QueryString("enableFrmbar") & "&pullapproval=" & Request.QueryString("pullapproval") & "&dontcreatetask=" & Request.QueryString("dontcreatetask") & mycollection & contentType & AddToCollectionType & FromEE & updateFieldId & taxonomyId & seltaxonomyId
        End If

    End Sub

    Private Sub SetJavascriptStrings()
        Dim AppUI As New ApplicationAPI
        Dim objResult, SiteObj As Object

        SiteObj = AppUI.EkSiteRef
        objResult = SiteObj.GetPermissions(0, 0, "folder")

        litPerReadOnlyLib.Text = LCase(objResult("ReadOnlyLib"))
        litLanguageId1.Text = AppUI.ContentLanguage
        litLanguageId2.Text = AppUI.ContentLanguage
    End Sub

    Private Sub RegisterJS()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronThickBoxJS)
    End Sub

    Private Sub RegisterCSS()
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronThickBoxCss)
    End Sub

End Class
