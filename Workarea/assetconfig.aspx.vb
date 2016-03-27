Imports System.Data
Imports Ektron.Cms
Partial Class AssetManagementConfig
    Inherits System.Web.UI.Page
    Protected m_refStyle As New StyleHelper
    Protected m_refContentApi As ContentAPI
    Protected AppName As String = ""
    Private AppPath As String = ""
    Protected AppImgPath As String = ""
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Private m_dataView As DataView
    Private m_dataTable As DataTable
    Private asset_config As AssetConfigInfo()

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
        AddHandler AMSGrid.EditCommand, AddressOf AMSGrid_Edit
        AddHandler AMSGrid.CancelCommand, AddressOf AMSGrid_Cancel
        AddHandler AMSGrid.UpdateCommand, AddressOf AMSGrid_Update
    End Sub


#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        m_refContentApi = New ContentAPI
        m_refMsg = m_refContentApi.EkMsgRef
        If m_refContentApi.RequestInformationRef.IsMembershipUser OrElse m_refContentApi.RequestInformationRef.UserId = 0 Then
            Response.Redirect("reterror.aspx?info=" & Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), False)
            Exit Sub
        End If
        AppName = m_refContentApi.AppName
        AppImgPath = m_refContentApi.AppImgPath
        AppPath = m_refContentApi.AppPath
        StyleSheetJS.Text = m_refStyle.GetClientScript
        AMSToolBar()

        If (Not (Page.IsPostBack)) Then
            asset_config = m_refContentApi.GetAssetMgtConfigInfo()
            PopulateGridData(asset_config)
            Session("AssetManagementConfigData") = asset_config
        Else
            If (Not IsNothing(Session("AssetManagementConfigTable"))) Then
                m_dataTable = Session("AssetManagementConfigTable")
                asset_config = Session("AssetManagementConfigData")
                m_dataView = New DataView(m_dataTable)
                m_dataView.Sort = "TAG"
            End If
        End If
        RegisterResources()
    End Sub

    Private Sub PopulateGridData(ByVal assetConfig As AssetConfigInfo())

        AMSGrid.BorderColor = Drawing.Color.White

        m_dataTable = New DataTable
        Dim dr As DataRow

        'If m_strPageAction = "edit" Then
        m_dataTable.Columns.Add(New DataColumn("EDIT", GetType(String)))
        'End If
        m_dataTable.Columns.Add(New DataColumn("TAG", GetType(String)))
        m_dataTable.Columns.Add(New DataColumn("VALUE", GetType(String)))
        m_dataTable.Columns.Add(New DataColumn("DESC", GetType(String)))
        Dim i As Integer = 0
        If (Not (IsNothing(assetConfig))) Then
            For i = 0 To assetConfig.Length - 1
                ' Temporary Fix untill chandra removes AsetConfigType.CatalogLocation/CatalogName from DMS code
                If (assetConfig(i) IsNot Nothing) AndAlso assetConfig(i).Tag.ToString() <> "UserName" AndAlso assetConfig(i).Tag.ToString() <> "Password" AndAlso assetConfig(i).Tag.ToString() <> "CatalogLocation" AndAlso assetConfig(i).Tag.ToString() <> "CatalogName" AndAlso assetConfig(i).Tag.ToString() <> "pdfGenerator" Then
                    dr = m_dataTable.NewRow
                    dr(1) = assetConfig(i).Tag.ToString()
                    dr(2) = assetConfig(i).Value
                    dr(3) = assetConfig(i).Description
                    m_dataTable.Rows.Add(dr)
                End If
            Next
        End If

        m_dataView = New DataView(m_dataTable)
        m_dataView.Sort = "TAG"
        Session("AssetManagementConfigTable") = m_dataTable

        BindData()
    End Sub
    Protected Sub BindData()
        AMSGrid.DataSource = m_dataView
        AMSGrid.DataBind()
    End Sub
    Public Sub AMSGrid_Cancel(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles AMSGrid.CancelCommand
        AMSGrid.EditItemIndex = -1
        BindData()
    End Sub
    Public Sub AMSGrid_Edit(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles AMSGrid.EditCommand
        AMSGrid.EditItemIndex = e.Item.ItemIndex
        BindData()
    End Sub

    Public Sub AMSGrid_Update(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles AMSGrid.UpdateCommand
        Dim dataText As TextBox = e.Item.Cells(2).Controls(1)
        Dim data As String = dataText.Text
        Dim tag As String = e.Item.Cells(1).Text
        Dim desc As String = e.Item.Cells(3).Text

        Dim dr As DataRow

        'DataViews filter not getting cleared

        m_dataView.RowFilter = "TAG='" & tag & "'"

        If (m_dataView.Count > 0) Then
            m_dataView.Delete(0)
        End If

        m_dataView.RowFilter = ""
        
        dr = m_dataTable.NewRow()
        m_dataTable.Rows.Add(dr)
        dr(1) = tag
        dr(2) = data
        dr(3) = desc

        Select Case tag
            Case AsetConfigType.CatalogLocation.ToString()
                asset_config(AsetConfigType.CatalogLocation).Value = data
            Case AsetConfigType.CatalogName.ToString()
                asset_config(AsetConfigType.CatalogName).Value = data
            Case AsetConfigType.DomainName.ToString()
                asset_config(AsetConfigType.DomainName).Value = data
             Case AsetConfigType.FileTypes.ToString()
                asset_config(AsetConfigType.FileTypes).Value = data
            Case AsetConfigType.LoadBalanced.ToString()
                asset_config(AsetConfigType.LoadBalanced).Value = data
           Case AsetConfigType.Password.ToString()
               asset_config(AsetConfigType.Password).Value = data
            Case AsetConfigType.ServerName.ToString()
                asset_config(AsetConfigType.ServerName).Value = data
            Case AsetConfigType.StorageLocation.ToString()
                asset_config(AsetConfigType.StorageLocation).Value = data
            Case AsetConfigType.UserName.ToString()
                asset_config(AsetConfigType.UserName).Value = data
            'Case AsetConfigType.VersionInstalled.ToString()
             '   asset_config(AsetConfigType.VersionInstalled).Value = data
            Case AsetConfigType.WebShareDir.ToString()
                asset_config(AsetConfigType.WebShareDir).Value = data
          End Select

        Session("AssetManagementConfigTable") = m_dataTable

        Session("DDSnip") = Nothing

        m_refContentApi.SetAssetMgtConfigInfo(asset_config)

        RefreshDropUploader()

        AMSGrid.EditItemIndex = -1

        BindData()
    End Sub

    Private Sub AMSToolBar()
        Dim result As New System.Text.StringBuilder
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl Asset Management Configuration"))
        result.Append("<table><tr>")
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("assetserverconfig"))
        result.Append("</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub

    Private Sub RefreshDropUploader()
        Dim sJS As New System.Text.StringBuilder
        sJS.Append("<script type=""text/Javascript"">" & vbCrLf)

        sJS.Append("var dragDropFrame = top.GetEkDragDropObject();" & vbCrLf)
        sJS.Append("      if (dragDropFrame != null) {" & vbCrLf)
        sJS.Append("            dragDropFrame.location.reload();" & vbCrLf)
        sJS.Append("      }" & vbCrLf)
        sJS.Append("</script>" & vbCrLf)
        Page.ClientScript.RegisterClientScriptBlock(GetType(Page), "DragRefreshJS", sJS.ToString())
    End Sub
    Private Sub RegisterResources()
        API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        API.JS.RegisterJS(Me, AppPath & "java/toolbar_roll.js", "EktronToolbarRollJS")
        API.JS.RegisterJS(Me, AppPath & "java/workareahelper.js", "EktronWorkareaHelperJS")
    End Sub
End Class
