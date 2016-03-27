
Partial Class Multimedia_windowsmediaparams
    Inherits System.Web.UI.UserControl
    Private m_MediaText As String
    Private m_refContentApi As New Ektron.Cms.CommonApi
    Public WriteOnly Property MediaText() As String
        Set(ByVal value As String)
            m_MediaText = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Me.ltWMPreview.Text = "<span id=""Results_WindowsMedia"" name=""Results_WindowsMedia"">" & m_MediaText & "</span>"
        lblContextMenu.Text = m_refContentApi.EkMsgRef.GetMessage("lbl context menu")
        lblPlayCount.Text = m_refContentApi.EkMsgRef.GetMessage("lbl playcount")
        lblEnabled.Text = m_refContentApi.EkMsgRef.GetMessage("enabled")
        lblMode.Text = m_refContentApi.EkMsgRef.GetMessage("lbl uimode")
        lblWindowless.Text = m_refContentApi.EkMsgRef.GetMessage("lbl windowless")

    End Sub
End Class
