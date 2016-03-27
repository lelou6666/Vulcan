Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.Common.EkEnumeration
Partial Class CommunityDragDrop
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

#End Region

    Protected m_refContentApi As New ContentAPI
    Protected m_refContent As Ektron.Cms.Content.EkContent
    Protected m_refMsg As Common.EkMessageHelper

    Protected asset_data As AssetInfoData()
    Protected m_bIsMac As Boolean

    Protected mode_set As Boolean = False
    Protected mode_id As Integer = 0 'mode=0->mode_id=folder_id, mode=1->mode_id=content_id
    Protected mode As Integer = 0 ' 0=add, 1=update
    Protected isimage As Integer = 0
    Protected overrideextension As String = ""

    Protected content_data As ContentData
    Protected folder_data As FolderData

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load
        'Put user code to initialize the page here
        'm_refMsg = m_refContentApi.EkMsgRef

        'If (Request.Browser.Platform.IndexOf("Win") = -1) Then
        'm_bIsMac = True
        'Else
        'm_bIsMac = False
        'End If

        'm_refContent = m_refContentApi.EkContentRef()
        'Display_DropUploader()
        If Request.QueryString("mode") = "0" Or IsNothing(Request.QueryString("mode")) Then
            If Not IsNothing(Request.QueryString("folderid")) AndAlso Request.QueryString("folderid") <> "" Then
                dropuploader.FolderID = Request.QueryString("folderid")
            Else
                dropuploader.FolderID = Request.QueryString("mode_id")
            End If
        Else
            If Not IsNothing(Request.QueryString("id")) AndAlso Request.QueryString("id") <> "" Then
                dropuploader.AssetID = Request.QueryString("id")
            Else
                dropuploader.AssetID = Request.QueryString("mode_id")
            End If
            dropuploader.FolderID = Request.QueryString("folder_id")
        End If
        If Not IsNothing(Request.QueryString("lang_id")) AndAlso Request.QueryString("lang_id") <> "" Then
            dropuploader.ContentLanguage = Request.QueryString("lang_id")
        End If
        If Not IsNothing(Request.QueryString("TaxonomyId")) AndAlso Request.QueryString("TaxonomyId") <> "" Then
            dropuploader.TaxonomyId = Request.QueryString("TaxonomyId")
        End If

        If Not IsNothing(Request.QueryString("isimage")) AndAlso Request.QueryString("isimage") <> "" Then
            dropuploader.IsImage = Request.QueryString("isimage")
        End If

        If Not IsNothing(Request.QueryString("overrideextension")) AndAlso Request.QueryString("overrideextension") <> "" Then
            dropuploader.OverrideExtension = Request.QueryString("overrideextension")
        End If

    End Sub

End Class
