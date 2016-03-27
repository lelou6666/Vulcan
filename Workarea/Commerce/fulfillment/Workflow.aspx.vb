Imports Ektron
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea
Imports Ektron.Cms.Commerce
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpRequest
Imports System.Web.UI.page

Partial Class FulfillmentWorkflow
    Inherits workareabase

    Protected curTypeName As String = String.Empty
    Protected m_strWfImgPath As String = String.Empty
    Protected AppPath As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        Dim api As New CommonApi()
        Utilities.ValidateUserLogin()
        Util_CheckAccess()
        AppPath = api.AppPath

        m_strWfImgPath = AppPath & "workflowimage.aspx?type=preview&id="
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        ' Register necessary JS files
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronModalJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronCookieJS)
        Ektron.Cms.API.JS.RegisterJS(Me, api.AppPath & "java/jfunct.js", "EktronJFunctJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronStringJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronScrollToJS)
        Ektron.Cms.API.JS.RegisterJS(Me, api.AppPath & "java/toolbar_roll.js", "EktronToolbarRollJS")

        lnkWorkflow.Text = "<img style=""cursor:pointer""  alt='click here to preview the selected workflow' src='" & api.AppPath & "Images/ui/icons/preview.png' onclick=""setImageUrl('" & m_strWfImgPath & "');$ektron('#wfImgModal').modalShow(); return false;""/>"
        workflowTitle.Text = GetMessage("lbl view workflow")

        If (Not Page.IsPostBack) Then

            Util_BindData()
            Util_SetLabels()
        Else
            Process_Save()
            Util_SetLabels()
        End If
    End Sub

#Region "Process"

    Protected Sub Process_Save()

        Try

            m_refContentApi.EkSiteRef.UpdateWorkflowType(ddWf.Text, Ektron.Cms.Common.EkEnumeration.WorkflowType.Order)

            ltr_workflow.Text = GetMessage("lbl workflow saved")

            Util_BindData()

        Catch ex As Exception

            ltr_workflow.Text = ex.Message

        End Try

    End Sub

#End Region

#Region "Util"

    Protected Sub Util_BindData()

        curTypeName = m_refContentApi.EkSiteRef.GetWorkflowType(Ektron.Cms.Common.EkEnumeration.WorkflowType.Order)

        ddWf.DataSource = Ektron.Workflow.Runtime.WorkflowHandler.GetOrderingWorkflows()
        ddWf.DataBind()

        If (Not String.IsNullOrEmpty(curTypeName)) Then
            If ddWf.Items.FindByValue(curTypeName) IsNot Nothing Then ddWf.Items.FindByValue(curTypeName).Selected = True
        End If

    End Sub

    Protected Sub Util_SetLabels()

        If Not Page.IsPostBack Then ltr_workflow.Text = GetMessage("lbl avail workflows")

        SetTitleBarToMessage("lbl order workflow")

        Dim actionMenu As New workareamenu("action", Me.GetMessage("lbl action"), AppPath & "images/UI/Icons/check.png")
        actionMenu.AddItem(AppPath & "images/ui/icons/save.png", Me.GetMessage("btn save"), " document.forms[0].submit(); ")
        AddMenu(actionMenu)

        AddHelpButton("orderworkflow")

    End Sub

    Protected Sub Util_CheckAccess()
        Try
            If Not m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin) Then
                Throw New Exception("error not role commerce-admin")
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try

    End Sub
#End Region

End Class
