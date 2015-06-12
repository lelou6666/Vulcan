Imports System
Imports Microsoft.VisualBasic

Public MustInherit Class ContentDesignerConfigurationBase
    Inherits System.Web.UI.Page

    Protected settings_data As Ektron.Cms.SettingsData
    Protected bCanModifyImg As Boolean = False

    Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load
        Try
            Dim refUserAPI As New Ektron.Cms.UserAPI
            Dim refSiteApi As New Ektron.Cms.SiteAPI
            Dim user_info As New Ektron.Cms.UserData
            Dim RequestInfo As Ektron.Cms.Common.EkRequestInformation
            settings_data = refSiteApi.GetSiteVariables()
            RequestInfo = refUserAPI.RequestInformationRef
            If IsNothing(RequestInfo) AndAlso 0 = RequestInfo.IsMembershipUser Then
                bCanModifyImg = True
            End If
            If (Not (IsNothing(Request.QueryString("CanModifyImg"))) AndAlso True = Request.QueryString("CanModifyImg")) Then
                bCanModifyImg = Request.QueryString("CanModifyImg")
            End If
        Catch ex As Exception
            ' Mostly likely not logged and file is being browsed directly
            settings_data = Nothing
        End Try
        Response.ContentType = "text/xml"
    End Sub
End Class
