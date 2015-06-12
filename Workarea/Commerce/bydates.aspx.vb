Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Workarea
Imports System.DateTime
Imports System.Collections.Generic
Partial Class Commerce_bydates
    Inherits workareabase
    Dim api As New Ektron.Cms.CommonApi
    Protected sTarget As String = "ekavatarpath"

#Region "Variables"
	Protected cdEditor As Ektron.ContentDesignerWithValidator
    Protected m_sEditAction As String = ""
    Protected editorPackage As String = ""
    Protected m_refProductType As ProductType = Nothing
    Protected prod_type_data As ProductTypeData = Nothing
    Protected xid As Integer = 0
    Protected bSuppressTemplate As Boolean = False
    Protected catalog_data As New FolderData
    Protected lValidCounter As Integer = 0
    Protected meta_data() As ContentMetaData = Nothing
    Protected entry_edit_data As EntryData = Nothing
    Protected m_refSite As Ektron.Cms.Site.EkSite = Nothing
    Protected m_iFolder As Integer = 0
    Protected m_mMeasures As MeasurementData = Nothing
#End Region
    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If

        Utils_RegisterResources()

        Dim dateSchedule As EkDTSelector
        Dim end_date_action As Integer = 1
        Dim go_live As String = ""
        Dim end_date As String = ""

        ltr_errStartEndDate.Text = m_refContentApi.EkMsgRef.GetMessage("js err start end date")
        ltr_startdate.Text = GetMessage("generic go live")
        ltr_enddate.Text = GetMessage("generic end date")
        If entry_edit_data IsNot Nothing Then
            go_live = entry_edit_data.GoLive
            If Not (entry_edit_data.EndDate = DateTime.MinValue Or entry_edit_data.EndDate = DateTime.MaxValue) Then
                end_date = entry_edit_data.EndDate
            End If
            end_date_action = entry_edit_data.EndDateAction
        End If

        dateSchedule = Me.m_refContentApi.EkDTSelectorRef
        dateSchedule.formName = "frmMain"
        dateSchedule.extendedMeta = True
        ' start
        dateSchedule.formElement = "go_live"
        dateSchedule.spanId = "go_live_span"
        If (go_live <> "") Then
            dateSchedule.targetDate = CDate(go_live)
        End If
        ltr_startdatesel.Text = (dateSchedule.displayCultureDateTime(True))
        dateSchedule.formElement = "end_date"
        dateSchedule.spanId = "end_date_span"
        If (end_date <> "") Then
            dateSchedule.targetDate = CDate(end_date)
        Else
            dateSchedule.targetDate = Nothing
        End If
        ltr_enddatesel.Text = (dateSchedule.displayCultureDateTime(True))
        ' end

    End Sub
    Protected Sub Utils_RegisterResources()

        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, m_refContentApi.AppPath & "csslib/box.css", "EktronBoxCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, m_refContentApi.AppPath & "csslib/pop_style.css", "EktronPopStyleCSS")

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS)
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncsJS")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "wamenu/includes/com.ektron.ui.menu.js", "EktronUIMenuJS")

    End Sub
End Class
