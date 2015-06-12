Imports Ektron.Cms
Imports Ektron.Cms.UI.CommonUI
Partial Class cmscalendar
    Inherits System.Web.UI.Page

    Protected m_contentApi As New ContentAPI


#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		ctlEditor = LoadControl("controls/Editor/ContentDesignerWithValidator.ascx")
        With ctlEditor
            .ID = "LongDescription"
            .AllowScripts = True
            .Height = New Unit(200, UnitType.Pixel)
            .Width = New Unit(600, UnitType.Pixel)
			.ToolsFile = m_contentApi.ApplicationPath & "ContentDesigner/configurations/InterfaceCalendar.xml"
			.ShowHtmlMode = False
			.Stylesheet = m_contentApi.ApplicationPath + "csslib/ektron.workarea.css"
        End With
        Action = Request.QueryString("action")
        If "AddEvent" = Action Then
            AddEventEditorHolder.Controls.Add(ctlEditor)
        ElseIf "EditEvent" = Action Then
            EditEventEditorHolder.Controls.Add(ctlEditor)
        End If
    End Sub


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region
    Public objStyle As StyleHelper
	Protected ctlEditor As Ektron.ContentDesignerWithValidator
    Private Action As String = ""

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		'Put user code to initialize the page here
        objStyle = New StyleHelper

        Dim AppUI As New ApplicationAPI
        Dim siteRef As New SiteAPI
        Dim saveObj, brObj, ret, returnID, ridArr, splitEvTypeIDs, urlParam, ridLoop, y As Object
        Dim actErrString As String

        Dim EnableClassicCalendar As Boolean = False
        Boolean.TryParse(ConfigurationManager.AppSettings.Item("ek_enableClassicCalendar"), EnableClassicCalendar)
        If (Not EnableClassicCalendar) Then
            Utilities.ShowError(AppUI.EkMsgRef.GetMessage("v7 calendar disabled"))
            Return
        End If

        RegisterResources()
        returnID = Nothing
        actErrString = ""
        Dim ContentLanguage As Integer
        If (Request.QueryString("LangType") <> "") Then
            ContentLanguage = Request.QueryString("LangType")
            AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage)
        Else
            If CStr(AppUI.GetCookieValue("LastValidLanguageID")) <> "" Then
                ContentLanguage = AppUI.GetCookieValue("LastValidLanguageID")
            End If
        End If
        siteRef.ContentLanguage = ContentLanguage
        jsDefaultContLang.Text = ContentLanguage
        jsId.Text = Request.Form("frm_calendar_id")

        Dim cCalendar As New Object
        Dim cInfo As New Object
        cCalendar = siteRef.EkModuleRef
        If "AddEvent" = Action Or "EditEvent" = Action Then
			With ctlEditor
				.AllowFonts = True
			End With
            If (IsPostBack()) Then
                cInfo = cCalendar.GetCalendarById(Request.Form("frm_calendar_id"))
                ' from calendaraction.aspx
                If "AddEvent" = Action Then
                    saveObj = New Collection
                    saveObj.Add(Request.Form("frm_calendar_id"), "CalendarID")
                    saveObj.Add(Request.Form("frm_event_title"), "EventTitle")
                    saveObj.Add(Request.Form("frm_event_location"), "EventLocation")
                    saveObj.Add(Request.Form("frm_year"), "Year")
                    saveObj.Add(Request.Form("frm_month"), "Month")
                    saveObj.Add(Request.Form("frm_day"), "Day")
                    saveObj.Add(Request.Form("frm_event_start"), "StartTime")
                    saveObj.Add(Request.Form("frm_event_end"), "EndTime")
                    saveObj.Add(Request.Form("frm_content_id"), "ContentID")
                    saveObj.Add(ContentLanguage, "ContentLanguage")
                    saveObj.Add(Request.Form("frm_qlink"), "QLink")
                    If 2 = cInfo("LongDescriptionAvail") Then
						saveObj.Add(ctlEditor.Content, "LongDescription")
                    ElseIf 1 = cInfo("LongDescriptionAvail") Then
                        saveObj.Add(Request.Form("frm_eventLongDesc"), "LongDescription")
                    End If
                    ' Response.write("LONG = " & Request.Form("frm_eventLongDesc"))

                    ' -1 = New Recurring Event, 0 = New Single Event
                    If (Request.Form("frm_recursive") = "1") Then
                        saveObj.Add(Request.Form("RecurRadio"), "RecursionType")
                        saveObj.add(-1, "RecursionID")
                    Else
                        saveObj.add("daily", "RecursionType")
                        saveObj.add(0, "RecursionID")
                    End If

                    If (Request.Form("frm_use_qlink") <> "") Then
                        saveObj.Add(1, "UseQLink")
                    Else
                        saveObj.Add(0, "UseQLink")
                    End If
                    If (Request.Form("frm_launch_new_browser") <> "") Then
                        saveObj.Add(1, "LaunchNewBrowser")
                    Else
                        saveObj.Add(0, "LaunchNewBrowser")
                    End If
                    If (Request.Form("frm_show_start_time") <> "") Then
                        saveObj.Add(1, "ShowStartTime")
                        saveObj.Add(1, "ShowEndTime")
                    Else
                        saveObj.Add(0, "ShowStartTime")
                        saveObj.Add(0, "ShowEndTime")
                    End If
                    'if (Request.Form("frm_show_end_time") <> "") then
                    '	saveObj.Add (1, "ShowEndTime")
                    'else
                    'saveObj.Add (0, "ShowEndTime")
                    'end if

                    brObj = siteRef.EkModuleRef

                    ret = brObj.AddCalendarEvent(saveObj, returnID)

                    ridArr = Split(returnID, ",")
                    If (Request.Form("selEvTypes") <> "") And (Len(actErrString) = 0) Then
                        splitEvTypeIDs = Split(Request.Form("selEvTypes"), ",")
                        For ridLoop = 0 To UBound(ridArr)
                            If (ridArr(ridLoop) <> "") Then
                                For y = 0 To UBound(splitEvTypeIDs)
                                    brObj.AddEvTypeIDToEvent(CLng(splitEvTypeIDs(y)), CLng(ridArr(ridLoop)))
                                Next
                            End If
                        Next

                        If (Len(actErrString) > 0) Then ret = True
                    End If

                    If (Len(Request.Form("frm_callback")) = 0) Then
                        ' Sometimes there is an apostrophe on the href, so we have to clean it first otherwise
                        ' the base site won't refresh.
                        Response.Write("<scr" & "ipt>")
                        Response.Write("var tRef = parent.opener.location.href ;")
                        Response.Write("if(tRef.substr(tRef.length-1,tRef.length)=='#') {")
                        Response.Write("parent.opener.location.href=tRef.substr(0,tRef.length-1) ;")
                        Response.Write("} else {")
                        Response.Write("parent.opener.location.href=parent.opener.location.href;")
                        Response.Write("}")
                        Response.Write("parent.window.close();")
                        Response.Write("</scri" & "pt>")
                    Else
                        Dim newDt As Date
                        newDt = Convert.ToDateTime(Request.Form("frm_event_start"))
                        newDt = DateSerial(newDt.Year, newDt.Month, 1)

                        urlParam = "cmscalendar.aspx?action=ShowCalendar"
                        urlParam = urlParam & "&calendar_id=" & Request.Form("frm_calendar_id")
                        urlParam = urlParam & "&sdate=" & newDt.ToString("d")
                        Response.Redirect(urlParam, False)
                    End If
                ElseIf "EditEvent" = Action Then
                    Dim tempCol, EvTypeIDUpdates, EvTypeIDNewUpdates As Object ', tempObj
                    EvTypeIDNewUpdates = Nothing

                    saveObj = New Collection
                    tempCol = New Collection

                    If (Request.Form("frm_recursive") = "1") Then
                        saveObj.Add(Request.Form("currentSel"), "RecursionType")
                        saveObj.Add(Request.Form("frm_recurs_id"), "RecursionID")
                    Else
                        saveObj.Add(0, "RecursionType")
                        saveObj.Add(0, "RecursionID")
                    End If

                    saveObj.Add(Request.Form("frm_calendar_id"), "CalendarID")
                    saveObj.Add(Request.Form("frm_event_id"), "EventID")
                    saveObj.Add(Request.Form("frm_event_title"), "EventTitle")
                    saveObj.Add(Request.Form("frm_event_location"), "EventLocation")
                    saveObj.Add(Request.Form("frm_year"), "Year")
                    saveObj.Add(Request.Form("frm_month"), "Month")
                    saveObj.Add(Request.Form("frm_day"), "Day")
                    saveObj.Add(Request.Form("frm_event_start"), "StartTime")
                    saveObj.Add(Request.Form("frm_event_end"), "EndTime")
                    saveObj.Add(Request.Form("frm_content_id"), "ContentID")
                    saveObj.Add(ContentLanguage, "ContentLanguage")
                    saveObj.Add(Request.Form("frm_qlink"), "QLink")
                    If 2 = cInfo("LongDescriptionAvail") Then
						saveObj.add(ctlEditor.Content, "LongDescription")
                    ElseIf 1 = cInfo("LongDescriptionAvail") Then
                        saveObj.Add(Request.Form("frm_eventLongDesc"), "LongDescription")
                    End If

                    If (Request.Form("frm_use_qlink") <> "") Then
                        saveObj.Add(1, "UseQLink")
                    Else
                        saveObj.Add(0, "UseQLink")
                    End If
                    If (Request.Form("frm_launch_new_browser") <> "") Then
                        saveObj.Add(1, "LaunchNewBrowser")
                    Else
                        saveObj.Add(0, "LaunchNewBrowser")
                    End If
                    If (Request.Form("frm_show_start_time") <> "") Then
                        saveObj.Add(1, "ShowStartTime")
                        saveObj.Add(1, "ShowEndTime")
                    Else
                        saveObj.Add(0, "ShowStartTime")
                        saveObj.Add(0, "ShowEndTime")
                    End If
                    'if (Request.Form("frm_show_end_time") <> "") then
                    '	saveObj.Add (1, "ShowEndTime")
                    'else
                    '	saveObj.Add (0, "ShowEndTime")
                    'end if

                    brObj = siteRef.EkModuleRef

                    If (Request.Form("UpdateAll") <> "1") Then
                        ret = brObj.UpdateCalendarEvent(saveObj)
                        EvTypeIDUpdates = Request.Form("frm_event_id")
                    Else
                        EvTypeIDNewUpdates = Request.Form("frm_associated_recursIDs")
                        ret = brObj.UpdateRecurringCalendarEvent(saveObj, EvTypeIDNewUpdates) 'Request.Form("frm_associated_recursIDs"))
                        EvTypeIDUpdates = Request.Form("frm_associated_recursIDs")
                    End If

                    ridArr = Split(EvTypeIDUpdates, ",")

                    If (CBool(Request.Form("frm_ev_type_avail")) And (Not ret)) Then
                        For ridLoop = 0 To UBound(ridArr)
                            If (ridArr(ridLoop) <> "") Then
                                brObj.ClearEvTypesForEvent(ridArr(ridLoop))
                                If (Len(actErrString) > 0) Then ret = True
                            End If
                        Next
                    End If

                    If Not (Request.Form("UpdateAll") <> "1") Then
                        ridArr = Split(EvTypeIDNewUpdates, ",")
                    End If

                    If ((Request.Form("selEvTypes") <> "") And (Len(actErrString) = 0) And (Not ret)) Then
                        splitEvTypeIDs = Split(Request.Form("selEvTypes"), ",")
                        For ridLoop = 0 To UBound(ridArr)
                            If ridArr(ridLoop) <> "" Then
                                For y = 0 To UBound(splitEvTypeIDs)
                                    brObj.AddEvTypeIDToEvent(CLng(splitEvTypeIDs(y)), CLng(ridArr(ridLoop)))
                                Next
                            End If
                        Next
                    End If


                    If Len(Request.Form("frm_callback")) Then
                        urlParam = Request.Form("frm_callback")
                        Response.Redirect(urlParam, False)
                    Else
                        urlParam = "cmscalendar.aspx?Action=ShowCalendar&calendar_id=" & Request.Form("frm_calendar_id")
                        urlParam = urlParam & "&sdate=" & DateSerial(Request.Form("frm_year"), Request.Form("frm_month"), Request.Form("frm_day")).ToString("d")
                        ' deprecated:
                        ' urlParam =  urlParam & "&date=" & Request.Form("frm_month") & "/" & Request.Form("frm_day") & "/" & Request.Form("frm_year")
                        Response.Redirect(urlParam, False)
                    End If
                    saveObj = Nothing
                    brObj = Nothing
                End If
            Else
                If "EditEvent" = Action Then
                    Dim gtNav As New Object
                    gtNav = cCalendar.GetEventByID(CLng(Request.QueryString("event_id")))
					ctlEditor.Content = gtNav("LongDescription")
                End If
            End If
        End If
    End Sub
    Protected Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss)

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS)
        Ektron.Cms.API.JS.RegisterJS(Me, m_contentApi.AppPath & "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncsJS")
    End Sub
End Class
