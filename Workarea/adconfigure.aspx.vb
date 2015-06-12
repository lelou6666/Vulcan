Imports Ektron.Cms
Partial Class adconfigure
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub
    Private pagedata As System.Collections.Specialized.NameValueCollection
    Protected m_strPageAction As String = ""
    Protected m_editadconfigure As editadconfigure
    Protected m_viewadconfigure As viewadconfigure
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1
        StyleSheetJs.Text = (New StyleHelper).GetClientScript
    End Sub
    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
        Dim bCompleted As Boolean
        RegisterResources()
        Try
            If (Not (IsNothing(Request.QueryString("action")))) Then
                If (Request.QueryString("action") <> "") Then
                    m_strPageAction = Request.QueryString("action").ToLower
                End If
            End If
            Select Case m_strPageAction
                Case "edit"
                    m_editadconfigure = CType(LoadControl("controls/configuration/editadconfigure.ascx"), editadconfigure)
                    DataHolder.Controls.Add(m_editadconfigure)
                    bCompleted = m_editadconfigure.EditAdConfiguration
                    If (bCompleted = True) Then
                        Response.Redirect("adconfigure.aspx", False)
                    End If
                Case Else
                    m_viewadconfigure = CType(LoadControl("controls/configuration/viewadconfigure.ascx"), viewadconfigure)
                    DataHolder.Controls.Add(m_viewadconfigure)
                    bCompleted = m_viewadconfigure.Display_ViewConfiguration
                    If (bCompleted) Then
                        Response.Redirect("adconfigure.aspx", False)
                    End If
            End Select
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
    Protected Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub
End Class
