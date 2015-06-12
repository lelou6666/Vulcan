<%@ Page Language="vb" AutoEventWireup="false" Inherits="calendaraction" validateRequest="False" CodeFile="calendaraction.aspx.vb" %>
<%@ Import Namespace="Ektron.Cms" %>
<%@ Import Namespace="Ektron.Cms.UI.CommonUI" %>

<% 
    
    ' Site, UserCol, uID
    ' linkObj1, linkObj2, contId, orderList, idArray, folderArray
    Dim action, brObj, Ret, actErrString, saveObj, currentUserID As Object
    Dim folderId, lLoop, urlParam As Object
    Dim evTypeSplit, evTypeTemp, EvTypeIDUpdates, y, calendar_id, EvTypeIDNewUpdates As Object
    EvTypeIDNewUpdates = Nothing
    
    Dim returnID, splitEvTypeIDs As Object
    returnID = Nothing

    Dim ridLoop, ridArr As Object

Dim AppUI As New ApplicationAPI
Dim siteRef as new siteAPI
Dim ContentLanguage As Integer

Try

If (Request.QueryString("LangType")<>"") Then
		ContentLanguage=Request.QueryString("LangType")
		AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage)
	else
		if CStr(AppUI.GetCookieValue("LastValidLanguageID")) <> ""  then
			ContentLanguage = AppUI.GetCookieValue("LastValidLanguageID")
		end if
End If
siteRef.ContentLanguage=ContentLanguage

action = Request.QueryString("action")
currentUserID = AppUI.UserId
actErrString = ""

if (action = "doAddEvent") then
	saveObj = new Collection
	saveObj.Add (Request.Form("frm_calendar_id"), "CalendarID")
	saveObj.Add (Request.Form("frm_event_title"), "EventTitle")
	saveObj.Add (Request.Form("frm_event_location"), "EventLocation")
	saveObj.Add (Request.Form("frm_year"), "Year")
	saveObj.Add (Request.Form("frm_month"), "Month")
	saveObj.Add (Request.Form("frm_day"), "Day")
	saveObj.Add (Request.Form("frm_event_start"), "StartTime")
	saveObj.Add (Request.Form("frm_event_end"), "EndTime")
	saveObj.Add (Request.Form("frm_content_id"), "ContentID")
	saveObj.Add (ContentLanguage,"ContentLanguage")
	saveObj.Add (Request.Form("frm_qlink"), "QLink")
	saveObj.add (Request.Form("frm_eventLongDesc"), "LongDescription")
	' Response.write("LONG = " & Request.Form("frm_eventLongDesc"))
	
	' -1 = New Recurring Event, 0 = New Single Event
	If (Request.Form("frm_recursive") = "1") Then
		saveObj.Add (Request.Form("RecurRadio"), "RecursionType")
		saveobj.add (-1, "RecursionID")
	Else
		saveObj.add ("daily", "RecursionType")
		saveobj.add (0, "RecursionID")
	End If

	if (Request.Form("frm_use_qlink") <> "") then
		saveObj.Add (1, "UseQLink")
	else
		saveObj.Add (0, "UseQLink")
	end if
	if (Request.Form("frm_launch_new_browser") <> "") then
		saveObj.Add (1, "LaunchNewBrowser")
	else
		saveObj.Add (0, "LaunchNewBrowser")
	end if
	if (Request.Form("frm_show_start_time") <> "") then
		saveObj.Add (1, "ShowStartTime")
		saveObj.Add (1, "ShowEndTime")
	else
		saveObj.Add (0, "ShowStartTime")
		saveObj.Add (0, "ShowEndTime")
	end if
	'if (Request.Form("frm_show_end_time") <> "") then
	'	saveObj.Add (1, "ShowEndTime")
	'else
		'saveObj.Add (0, "ShowEndTime")
	'end if
	
	brObj = siteRef.EkModuleRef
	
	ret = brObj.AddCalendarEvent(saveObj, returnID)

	ridArr = Split(returnID,",")
	If (Request.Form("selEvTypes") <> "") and (Len(actErrString) = 0) Then
		splitEvTypeIDs = Split(Request.Form("selEvTypes"), ",")
		For ridLoop = 0 to UBound(ridArr)
			If (ridArr(ridLoop) <> "") Then
				For y = 0 to UBound(splitEvTypeIDs)
                            brObj.AddEvTypeIDToEvent(CLng(splitEvTypeIDs(y)), CLng(ridArr(ridLoop)))
				Next
			End If
		Next
		
		if (len(actErrString) > 0) Then ret = True
	End If

	brObj = nothing
	saveObj = nothing
	
		if (len(Request.Form("frm_callback")) = 0) then
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
			Response.WRite ("</scri" & "pt>")
		else
			Dim newDt as Date
			newDt = Convert.ToDateTime(Request.Form("frm_event_start"))
			newDt = DateSerial(newDt.Year, newDt.Month, 1)
			
			urlParam = "cmscalendar.aspx?action=ShowCalendar"
			urlParam =  urlParam & "&calendar_id=" & Request.Form("frm_calendar_id")
			urlParam =  urlParam & "&sdate=" & newDt.ToString("d")
			Response.Redirect(urlparam, false)
		end if


elseif (action = "doEditEvent") then
            Dim tempCol As Object ', tempObj
	
	saveObj = new Collection
	tempCol = new Collection

	If (Request.Form("frm_recursive") = "1") Then
		saveObj.Add (Request.Form("currentSel"), "RecursionType")
		saveObj.Add (Request.Form("frm_recurs_id"), "RecursionID")
	Else
		saveObj.Add (0, "RecursionType")
		saveObj.Add (0, "RecursionID")
	End If

	saveObj.Add (Request.Form("frm_calendar_id"), "CalendarID")
	saveObj.Add (Request.Form("frm_event_id"), "EventID")
	saveObj.Add (Request.Form("frm_event_title"), "EventTitle")
	saveObj.Add (Request.Form("frm_event_location"), "EventLocation")
	saveObj.Add (Request.Form("frm_year"), "Year")
	saveObj.Add (Request.Form("frm_month"), "Month")
	saveObj.Add (Request.Form("frm_day"), "Day")
	saveObj.Add (Request.Form("frm_event_start"), "StartTime")
	saveObj.Add (Request.Form("frm_event_end"), "EndTime")
	saveObj.Add (Request.Form("frm_content_id"), "ContentID")
	saveObj.Add (ContentLanguage,"ContentLanguage")
	saveObj.Add (Request.Form("frm_qlink"), "QLink")
	saveObj.add (Request.Form("frm_eventLongDesc"), "LongDescription")

	if (Request.Form("frm_use_qlink") <> "") then
		saveObj.Add (1, "UseQLink")
	else
		saveObj.Add (0, "UseQLink")
	end if
	if (Request.Form("frm_launch_new_browser") <> "") then
		saveObj.Add (1, "LaunchNewBrowser")
	else
		saveObj.Add (0, "LaunchNewBrowser")
	end if
	if (Request.Form("frm_show_start_time") <> "") then
		saveObj.Add (1, "ShowStartTime")
		saveObj.Add (1, "ShowEndTime")
	else
		saveObj.Add (0, "ShowStartTime")
		saveObj.Add (0, "ShowEndTime")
	end if
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

	ridArr = Split(EvTypeIDUpdates,",")

	If (CBool(Request.Form("frm_ev_type_avail")) and (not ret)) Then 
		For ridLoop = 0 to UBound(ridArr)
			If (ridArr(ridLoop) <> "") Then
				brobj.ClearEvTypesForEvent(ridArr(ridLoop))
				if (len(actErrString) > 0) Then ret = True
			End If
		Next
	End If
	
	If Not (Request.Form("UpdateAll") <> "1") Then
		ridArr = Split(EvTypeIDNewUpdates,",")
	End If

	If ((Request.Form("selEvTypes") <> "") and (Len(actErrString) = 0) and (not ret)) Then
		splitEvTypeIDs = Split(Request.Form("selEvTypes"), ",")
		For ridLoop = 0 to UBound(ridArr)
			If ridArr(ridLoop) <> "" Then
				For y = 0 to UBound(splitEvTypeIDs)
                            brObj.AddEvTypeIDToEvent(CLng(splitEvTypeIDs(y)), CLng(ridArr(ridLoop)))
				Next
			End If
		Next
	End If


		if len(Request.Form("frm_callback")) then
			urlParam = Request.Form("frm_callback")
			Response.Redirect(urlParam, false)
		else
			urlParam = "cmscalendar.aspx?Action=ShowCalendar&calendar_id=" & Request.Form("frm_calendar_id")
			urlParam =  urlParam & "&sdate=" & DateSerial(Request.Form("frm_year"), Request.Form("frm_month"), Request.Form("frm_day")).ToString("d")
			' deprecated:
			' urlParam =  urlParam & "&date=" & Request.Form("frm_month") & "/" & Request.Form("frm_day") & "/" & Request.Form("frm_year")
			Response.Redirect(urlParam, false)
		end if

elseif (action = "doEventDelete") then

	if Request.QueryString("deleteAssoc") = "true" then

		if Request.QueryString("event_id") <> "" then
			brObj = siteRef.EkModuleRef
			ret = brObj.DeleteRecurEventsByEventID(Request.QueryString("event_id"))
		else
			ret = true
			actErrString = "Wrong parameters passed"
		end if
		
	else
		if Request.QueryString("event_id") <> "" then
			brObj = siteRef.EkModuleRef
			ret = brObj.DeleteEventByID(Request.QueryString("event_id"))
		else
			ret = true
			actErrString = "Wrong parameters passed"
		end if
	End If


		urlParam = "cmscalendar.aspx?Action=ViewEvents&calendar_id=" & Request.QueryString("calendar_id") 
		urlParam =  urlParam & "&iMonth=" & Request.QueryString("iMonth") & "&iDay=" & Request.QueryString("iDay") & "&iYear=" & Request.QueryString("iYear")
		Response.Redirect(urlParam, false)

elseif (action = "doAdd") then 
	folderId = Request.Form("frm_folder_id")
            Dim calendarid As Object = Request.Form("frm_calendar_id")

	saveObj = new Collection
	
	If(calendarid <> "") Then
                saveObj.Add(CLng(calendarid), "CalendarID")
	Else
		saveObj.Add(0,"CalendarID")
	End If

	saveObj.Add (Request.Form("frm_calendar_title"), "CalendarTitle")
	saveObj.Add (Request.Form("frm_calendar_description"), "CalendarDescription")
	saveObj.Add (folderId,"RootFolderID")
	saveObj.Add (Request.Form("frm_table_height"), "TableHeight")
	saveObj.Add (Request.Form("frm_table_width"), "TableWidth")
	saveObj.Add (Request.Form("frm_location_label"), "LocationLabel")
	saveObj.Add (Request.Form("frm_start_label"), "StartLabel")
	saveObj.Add (Request.Form("frm_end_label"), "EndLabel")
	saveObj.Add (Request.Form("frm_evtype_label"), "EventTypeLabel")
	saveObj.Add (Request.Form("frm_longDescAvail"), "LongDescriptionAvail")
	saveObj.Add (Request.Form("frm_evtype_instruct"), "EvTypeInstruct")
	saveObj.Add (Request.Form("frm_evtype_showall"), "EvTypeShowall")

	if (Request.Form("frm_forward_only") = "1") then
		saveObj.Add (1, "ForwardOnly") 
	else
		saveObj.Add (0, "ForwardOnly")
	end if
	
	if (Request.Form("frm_show_weekend") <> "") then
		saveObj.Add (1, "ShowWeekEnd")
	else
		saveObj.Add (0, "ShowWeekEnd")
	end if

	if (Request.Form("frm_ev_type_avail") <> "") then
		saveObj.Add (1, "AvailEvType")
	else
		saveObj.Add (0, "AvailEvType")
	end if

	if (Request.Form("frm_ev_type_req") <> "") then
		saveObj.Add (1, "ReqEvType")
	else
		saveObj.Add (0, "ReqEvType")
	end if

	brObj = siteRef.EkModuleRef
	ret = brObj.AddNewCalendar(saveObj)
	
	saveObj=Nothing
	brObj = Nothing	

	Response.Redirect("cmscalendar.aspx?Action=ViewAllCalendars", false)

elseif(action = "doDelete") then
	if Request.QueryString("calendar_id") <> "" then
		brObj = siteRef.EkModuleRef
		ret = brObj.DeleteCalendarByID(Request.QueryString("calendar_id"))
		Response.Redirect("cmscalendar.aspx?Action=ViewAllCalendars", false)
	else
		Throw New Exception("There was no calendar ID passed in to this action page")
	end if
elseif (action = "doUpdate") then  
            Dim edObj As Object
	calendar_id = Request.Form("frm_calendar_id")
	folderId = Request.Form("frm_folder_id")
	edObj = new Collection
	edObj.Add (Request.Form("frm_calendar_title"), "CalendarTitle")
	edObj.Add (Request.Form("frm_calendar_description"), "CalendarDescription")
	edObj.Add (calendar_id, "CalendarID")
	edObj.Add (folderid, "FolderID")
	if (Request.Form("frm_show_weekend") <> "") then
		edObj.Add (1, "ShowWeekEnd")
	else
		edObj.Add (0, "ShowWeekEnd")
	end if
	edObj.Add (Request.Form("frm_table_height"), "TableHeight")
	edObj.Add (Request.Form("frm_table_width"), "TableWidth")
	edObj.Add (Request.Form("frm_location_label"), "LocationLabel")
	edObj.Add (Request.Form("frm_start_label"), "StartLabel")
	edObj.Add (Request.Form("frm_end_label"), "EndLabel")
	edObj.Add (Request.Form("frm_evtype_label"), "EventTypeLabel")
	edObj.Add (Request.Form("frm_longDescAvail"), "LongDescriptionAvail")
	edObj.Add (Request.Form("frm_evtype_instruct"), "EvTypeInstruct")
	edObj.Add (Request.Form("frm_evtype_showall"), "EvTypeShowall")

	if (Request.Form("frm_forward_only") <> "") then
		edObj.Add (1, "ForwardOnly")
	else
		edObj.Add (0, "ForwardOnly")
	end if

	if (Request.Form("frm_ev_type_avail") <> "") then
		edObj.Add (1, "AvailEvType")
	else
		edObj.Add (0, "AvailEvType")
	end if

	if (Request.Form("frm_ev_type_req") <> "") then
		edObj.Add (1, "ReqEvType")
	else
		edObj.Add (0, "ReqEvType")
	end if
	
	brObj = siteRef.EkModuleRef
  	ret = brObj.UpdateCalendarInfo(edObj)
  brObj = Nothing

	Response.Redirect("cmscalendar.aspx?Action=ViewCalendar&calendar_id=" & calendar_id & "&folderid=" & folderid, false)

elseif (action = "doDelete") then	 	 
  brObj = siteRef.EkModuleRef
  ret = brObj.DeleteCollectionItem(Request.QueryString("nId"))
  brObj = Nothing

	Response.Redirect("collections.aspx?folderid=" & Request.QueryString("folderid"), false)


elseif (action="doDeleteEvTypes") Then
	
	brObj = siteRef.EkModuleRef
	
	evTypeSplit = Split(Request.Form("selEventType"),",")
	
	calendar_id = Request.Form("frm_calendar_id")
	folderId = Request.Form("frm_folder_id")
	
	For lLoop = 0 to UBound(evTypeSplit)
		evTypeTemp = new Collection
		
		evTypeTemp.Add (evTypeSplit(lLoop), "EventTypeID")
		evTypeTemp.Add (calendar_id, "CalendarID")

		Ret = brObj.DeleteCalendarEventType(evTypeTemp)
		
		evTypeTemp = Nothing

	Next
	Response.Redirect("cmsCalendar.aspx?action=ViewEvTypes&calendar_id=" & calendar_id & "&folderid=" & Request.QueryString("folderid"), false)

elseif (action="doAddEvTypes") Then
	
	brObj = siteRef.EkModuleRef
	evTypeTemp = new Collection

	calendar_id = Request.Form("frm_calendar_id")
	folderId = Request.Form("frm_folder_id")
		
	evTypeTemp.Add (Request.Form("frm_evTypeName"), "EventTypeName")
	evTypeTemp.Add (calendar_id, "CalendarID")
		
	Ret = brObj.AddCalendarEventType(evTypeTemp)

	evTypeTemp = Nothing

		Response.Redirect("cmsCalendar.aspx?action=ViewEvTypes&calendar_id=" & calendar_id & "&folderid=" & Request.QueryString("folderid"), false)

elseif (action="doUpdateEvTypes") Then
	brObj = siteRef.EkModuleRef
	evTypeTemp = new Collection

	calendar_id = Request.Form("frm_calendar_id")
	folderId = Request.Form("frm_folder_id")

	evTypeTemp.Add (Request.Form("frm_ev_type_id"), "EventTypeID")
	evTypeTemp.Add (Request.Form("frm_evTypeName"), "EventTypeName")
	evTypeTemp.Add (calendar_id, "CalendarID")
		
	Ret = brObj.UpdateCalendarEventType(evTypeTemp)

	evTypeTemp = Nothing
	
	Response.Redirect("cmsCalendar.aspx?action=ViewEvTypes&calendar_id=" & calendar_id & "&folderid=" & Request.QueryString("folderid"), false)


end if

Catch ex as Exception
	Response.Redirect("reterror.aspx?info=" & Server.URLEncode(ex.Message), false)
	' Response.Write("ERROR: " & ex.Message & "<br/>")
End Try
			
%>