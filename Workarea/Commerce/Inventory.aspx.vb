Imports Ektron.Cms.Workarea

Partial Class Commerce_Inventory
    Inherits workareabase

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        SetLabels()
    End Sub

    Protected Sub SetLabels()
        Me.SetTitleBarToMessage("lbl inventory")
        Me.AddHelpButton("inventory")
    End Sub
End Class
