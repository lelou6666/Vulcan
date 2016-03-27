<%@ Page Language="vb" AutoEventWireup="false" validateRequest="false" Inherits="cmscalendar" CodeFile="cmscalendar.aspx.vb" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="controls/Editor/ContentDesignerWithValidator.ascx" %>
<%@ Import Namespace="Ektron.Cms" %>
<%@ Import Namespace="Ektron.Cms.UI.CommonUI" %>
<script language="vb" runat="server">
Dim CollectionID, msgs, currentUserID, ErrorString, PageTitle, cTmp, ShowStime, ekContObj as Object
Dim astrMonthNames, AppName, l, evtRadFirstPass, evTypeLabel, IsMac, platform as Object
dim action, folderId,ParentFolderID, nID, cFolders, folder,calendar_id as Object
dim title1, descrp,ShowWnds,location_label,start_label,end_label, contObj as Object
dim table_width , table_height, cell_border,forward_only,fldObj,gtnStart ,gtnEnd ,evTypNameVal as Object
Dim AppPath  as Object
Dim AppImgPath as Object
Dim imgIconPath as Object
Dim var1, var2, AppeWebPath as Object
Dim ImTheAdmin As Boolean
dim IsBrowserIE As Boolean = False
Dim gtNav, CollectionTitle, lLoop, siteObj, cPerms, cLinkArray, fLinkArray, FolderName, VerfiyFalse, VerfiyTrue as Object

Dim AppUI as New CommonAPI
Dim siteRef as New SiteAPI
Dim AppWebUI As New ApplicationAPI
Dim contObjForPerms as New ContentAPI
Dim mMsgs as Ektron.Cms.Common.EkMessageHelper
dim subfolderid,backId as Object
Dim ContentLanguage as Object
Dim EnableMultilingual as Object = siteRef.EnableMultilingual
Dim dSo as Object

dim iMonth,iDay, iYear,cCalendar,cInfo,cConts,fPath,eventDate as Object
dim sTime,eTime,EvtTitle,UseQlink,Qlink,LaunchBrowser, ShowETime,eLocation,CanCreateContent as Object

Const ALL_CONTENT_LANGUAGES as Integer =-1
Const CONTENT_LANGUAGES_UNDEFINED as Integer =0
Function IsUserAdmin(ByVal userId as Object , ByVal cSite as Object) as Object
	SiteObj = siteRef.EkSiteRef
	' cPerms = SiteObj.GetPermissionsv2_0(0, "folder")
	cPerms = contObjForPerms.LoadPermissions(0,"folder")

	IsUserAdmin = cPerms.isAdmin
	SiteObj = nothing
	cPerms = nothing
end function

function ampmDD(ByVal inTime as Object, ByVal sFrm as Object, ByVal inFrmName as Object, ByVal eFrm as Object, ByVal eFrmAM as Object) as Object
	dim checkedAM, checkedPM, onBlurText as Object
	checkedAM = Nothing
	checkedPM = Nothing
	If LCase(right(inTime,(Len(inTime)-Instr(inTime," ")))) = "am" Then checkedAM = " selected"
	If LCase(right(inTime,(Len(inTime)-Instr(inTime," ")))) = "pm" Then checkedPM = " selected"

	If sFrm <> "" Then
		onBlurText = " onBlur=""javascript: EndTimeAddHour('" & sFrm & "', '" & inFrmName & "', '" & eFrm & "', '" & eFrmAM & "');"" "
	Else
		onBlurText = ""
	End If
	response.write("<select name=""" & inFrmName & """ " & onBlurText & "><option value=""AM"" " & checkedAM & ">AM</option><option value=""PM""" & checkedPM & ">PM</option></select>")
    Return Nothing
end function

function Radio(ByVal inVal as Object, ByVal inDef as Object, ByVal inIsEdit as Object) as Object
	dim SELD as Object = Nothing

	if (inVal=inDef) Then
		SELD = " checked"
	end if

	if (inIsEdit) Then
		response.write("<INPUT TYPE=""RADIO"" NAME=""RecurRadio"" VALUE=""" & inVal & """ onclick=""flipRecursBG(this.value);"" " & SELD & ">")
		' response.write("<INPUT TYPE=""RADIO"" NAME=""RecurRadio"" VALUE=""" & inVal & """ onclick=""flipRecursBG(this.value); countDays();"" " & SELD & ">")
	Else
		response.write("<INPUT TYPE=""hidden"" NAME=""RecurRadio"" VALUE=""" & inVal & """>")
	End If
	return Nothing
end function

function AvailableEventTypes(ByVal inCalID as Object, ByVal inErrString as Object, ByVal defSelected as Object) as Object
	Dim modObj, calObj, evTypeCol, thing, defThing, bInDefault as Object

	'Set modObj = Server.CreateObject(MODULE_OBJ)
	modObj = siteRef.EkModuleRef
	calObj = modObj.GetCalendarById(inCalID)

	evTypeCol = calobj("EventTypes")

	' If There are no Event Types for the calendar, display "No Event Types" And
	' If The Event Types (for Calendar) Count is the same as the Default Selected, then all default selected
	' will be places in SelectedEventTypes(), so show "No Event Types" to add.

	If ((not (defSelected is nothing)) And (not (evTypeCol is nothing))) Then
		If ((evTypeCol.Count <> 0) And (evTypeCol.count <> defSelected.count)) Then
			For each thing in evTypeCol
				bInDefault = False
				For each defThing in defSelected
					If defThing("EventTypeID") = thing("EventTypeID") Then bInDefault = True
				Next
				If (not bInDefault) Then
					Response.Write ("<option value=""" & thing("EventTypeID") & """>" & thing("EventTypeName") & "</option>")
				End If
			Next
		Else
			Response.Write ("<option value=""-1"" >No Event Types</option>")
		End if
	Else
		If (not (evTypeCol is nothing)) Then
			If (evTypeCol.Count <> 0) Then
	 			For each thing in evTypeCol
					Response.Write ("<option value=""" & thing("EventTypeID") & """>" & thing("EventTypeName") & "</option>")
				Next
			Else
				Response.Write ("<option value=""-1"" >No Event Types</option>")
			End If
		Else
			Response.Write ("<option value=""-1"" >No Event Types</option>")
		End If
	End If

	modObj = Nothing
	calObj = Nothing
	evTypeCol = Nothing
	return Nothing
end function
Function FormatNumber(ByVal Value As String) AS String
	Dim result as string=Value
	If(Value.Trim.Length=1) Then
		result="0" & result
	End If
	Return(result)
End Function
function SelectedEventTypes(ByVal defSelected as Object) as Object
	Dim thing as Object

	if (not(defSelected is nothing)) then
		if (defSelected.count <> 0) then
			For each thing in defSelected
				Response.Write ("<option value=""" & thing("EventTypeID") & """>" & thing("EventTypeName") & "</option>")
			Next
		else
			Response.Write ("<option value=""-1"" >No Event Types</option>")
		end if
	else
		Response.Write ("<option value=""-1"" >No Event Types</option>")
	end if
	return Nothing
end function

function DisplayRecursionTabs(ByVal sMonth as Object, ByVal sDay as Object, ByVal sYear as Object, ByVal sTime as Object, ByVal eMonth as Object, ByVal eDay as Object, ByVal eYear as Object, ByVal eTime as Object, ByVal recurType as Object, ByVal recursID as Object, ByVal isEdit as Object, ByVal ActualDate as Object, ByVal StartLabel as Object, ByVal EndLabel as Object) as Object
	dim IntlStartDate, IntlEndDate, MonthArray, IntlActual, mMsgSelect as Object
	IntlActual = Nothing
	IntlStartDate = Nothing
	IntlEndDate = Nothing

	' Set up Messages Object
	mMsgSelect = "lbl evt will occur, lbl every day, " & _
		"lbl evt times, lbl every weekday, lbl every selected date of every Month, lbl every first selected weekday of every Month" & _
		", lbl yearly every selected date of Month, lbl yearly every first selected weekday of Month, lbl display times, generic date label" & _
		", lbl one time, lbl recurring"

	if (ErrorString <> "") then
		Response.Write(ErrorString)
	end if

	' Date Selector Object, set up the dSo
	dSo.formName = "calendar"

	MonthArray = split(siteRef.getEnglishMonthsAbbrev, ",")

	If (isDate(ActualDate)) Then
		IntlActual = DatePart("d",ActualDate) & "-" & MonthArray(DatePart("m",ActualDate)) & "-" & DatePart("yyyy",ActualDate)
	End If

	If ((cstr(sMonth) <> "") And (cstr(sDay) <> "") And (cstr(sYear) <> "")) Then
		If (isNumeric(sMonth)) Then
			IntlStartDate = sDay & "-" & MonthArray(sMonth) & "-" & sYear
		Else
			IntlStartDate = sDay & "-" & sMonth & "-" & sYear
		End If
	End If

	If (sTime="") Then
		sTime = "12:00 AM"
	end if

	If (cstr(eMonth) <> "") And (cstr(eDay) <> "") And (cstr(eYear) <> "") Then
		If (isNumeric(eMonth)) Then
			IntlEndDate = eDay & "-" & MonthArray(eMonth) & "-" & eYear
		Else
			IntlEndDate = eDay & "-" & eMonth & "-" & eYear
		End If
	End If

	If (cstr(eTime) = "") Then
		eTime = "12:00 AM"
	end if

	If (cstr(recurType) = "") Then
		recurType="daily"
	end if

	response.write("<input type=""hidden"" name=""frm_editdate_only"" value=""" & IntlActual & """ />" & VbCrLf)
	response.write("<input type=""hidden"" name=""frm_recursive"" id=""frm_recursive"" value=""0"" />" &VbCrLf)
	response.write("<input type=""hidden"" name=""frm_recurs_id"" value=""" & recursID & """ />" & VbCrLf)
	response.write("<table width=""100%"">" & VbCrLf)
	response.write("<tr>")
	response.write("<td width=""25%"" class=""selectedTab"" ")
	If (Not (isEdit)) or action="AddEvent"  Then
			response.write("onclick=""flipTab('1');"" ")
	End If
	response.write("id=""tabOne"" style=""cursor:pointer"">&nbsp;&nbsp;" & mMsgs.GetMessage("lbl one time") & "&nbsp;</td>" & VbCrLf)
	response.write("<td width=""25%"" class=""nonSelectedTab"" ")
	If (Not (isEdit)) or action="AddEvent"  Then
			response.write("onclick=""flipTab('2');"" ")
	End If
	response.write("id=""tabTwo"" style=""cursor:pointer"">&nbsp;&nbsp;" & mMsgs.GetMessage("lbl recurring") & "&nbsp;</td>" & VbCrLf)
	response.write("<td width=""75%"" class=""nonTabbed"">&nbsp;</td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("<tr>" & VbCrLf)
	response.write("<td colspan=3 class=""TabContent""><div class=""DisplayTabContent"" id=""OnceTab"">" & VbCrLf)
	response.write("<table class=""ektronGrid"">" & VbCrLf)
	response.write("<tr>" & VbCrLf)
	response.write("<td class=""label"">" & mMsgs.GetMessage("generic date label") & "</td>" & VbCrLf)
	response.write("<td class=""readOnlyValue"">" & VbCrLf)
		dSo.formElement = "frm_event_start_single"
		dSo.spanId = "frm_event_start_single_span"
		dSo.targetDate = ActualDate
	response.write(dSo.displayCultureDate(true))
	' response.write("<input type=""text"" name=""frm_event_start_single"" value=""" & IntlStartDate & """ size=""15"">" & VbCrLf)
	' response.write("&nbsp;<a href=""#"" onClick=""javascript:document.calendar.frm_event_start_single.value = Trim(document.calendar.frm_event_start_single.value);CallCalendar(document.calendar.frm_event_start_single.value, 'calendardateonly.aspx', 'frm_event_start_single', 'calendar', '" & siteRef.ContentLanguage & "');return false;""><img src=""" & siteRef.AppImgPath & "btn_calendar-nm.gif"" alt=""" & gtMess("alt calendar button text") & """ title=""" & gtMess("alt calendar button text") & """ border=""0""></a>" & VbCrLf)
	response.write("</td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("<tr>" & VbCrLf)
	response.write("<td class=""label"">" & StartLabel & "</td>" & VbCrLf)
		dSo.formElement = "frm_start_time_single"
		dSo.spanId = "frm_start_time_single_span"
		if sTime <> "12:00 AM" then
			dSo.targetDate = CDate(ActualDate)
		else
			dSo.targetDate = CDate(sTime)
		end if
		if action<>"AddEvent" then
			response.write("<td class=""readOnlyValue"">" & dSo.displayCultureTime(True) & "</td>")
		else
			response.write("<td class=""readOnlyValue"">" & dSo.displayCultureTime(True,"add_event") & "</td>")
		end if
	' response.write("<input type=""text"" name=""frm_start_time_single"" value=""" & Left(sTime,InStr(1,sTime," ")) & """ size=""8"" onBlur=""javascript: EndTimeAddHour('frm_start_time_single', 'frm_start_date_time_single_ampm', 'frm_end_time_single', 'frm_end_date_time_single_ampm');"">&nbsp; ")
	' ampmDD(sTime, "frm_start_time_single", "frm_start_date_time_single_ampm", "frm_end_time_single", "frm_end_date_time_single_ampm")
	response.write("</td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("<tr>" & VbCrLf)
	response.write("<td class=""label"">" & EndLabel & "</td>" & VbCrLf)
		dSo.formElement = "frm_end_time_single"
		dSo.spanId = "frm_end_time_single_span"
		dSo.targetDate = CDate(eTime)
		if action<>"AddEvent" then
			response.write("<td class=""readOnlyValue"">" & dSo.displayCultureTime(True) & "</td>")
		else
			response.write("<td class=""readOnlyValue"">" & dSo.displayCultureTime(True,"add_event") & "</td>")
		end if
	'response.write("<td><input type=""text"" name=""frm_end_time_single"" value=""" & Left(eTime, InStr(1,eTime," ")) & """ size=""8"">&nbsp;" & VbCrLf)
	'ampmDD (eTime, "", "frm_end_date_time_single_ampm", "", "")
	response.write("</tr>" & VbCrLf)
	response.write("</table>" & VbCrLf)
	response.write("</div>" & VbCrLf)
	response.write("<div class=""NoDisplayTabContent"" id=""RecurTab"">" & VbCrLf)
	response.write("<table class=""ektronGrid"">" & VbCrLf)
	response.write("<tr>" & VbCrLf)
	If (isEdit) Then
		If (recursID <> 0) Then
			response.write("<input type=""checkbox"" name=""updateAll"" value=""1"" checked>&nbsp;Update all recurring events associated with this event.<br />" & VbCrLf)
		End If

			dSo.formElement = "frm_start_date_only"
			dSo.spanId = "frm_start_date_only_span"
			dSo.targetDate = CDate(IntlStartDate)
		response.write("<td valign=""bottom"" nowrap=""true"" class='label' style='text-align: left !important'>" & StartLabel & " " & dSo.displayCultureDate(isEdit) & "</td>")
			dSo.formElement = "frm_end_date_only"
			dSo.spanId = "frm_end_date_only_span"
			dSo.targetDate = CDate(IntlEndDate)
		response.write("<td valign=""bottom"" nowrap=""true"" class='label' style='text-align: left !important'>" & EndLabel & " " & dSo.displayCultureDate(isEdit) & "</td>")

		' response.write("<td valign=""bottom"">" & StartLabel & "&nbsp;<input type=""hidden"" name=""frm_start_date_only"" value=""" & IntlStartDate & """>" & IntlStartDate & "</td>" & VbCrLf)
		' response.write("<td valign=""bottom"">" & EndLabel & "&nbsp;<input type=""hidden"" name=""frm_end_date_only"" value=""" & IntlEndDate & """>" & IntlEndDate & "</td>" & VbCrLf)
	Else

		dSo.formElement = "frm_start_date_only"
		dSo.spanId = "frm_start_date_only_span"
		dSo.targetDate = CDate(IntlStartDate)
	response.write("<td valign=""bottom"">" & StartLabel & "<br/>" & dSo.displayCultureDate(isEdit) & "</td>")
		dSo.formElement = "frm_end_date_only"
		dSo.spanId = "frm_end_date_only_span"
		If (IntlEndDate="") Then
			IntlEndDate = IntlStartDate
		End If
		dSo.targetDate = CDate(IntlEndDate)
	response.write("<td valign=""bottom"">" & EndLabel & "<br/>" & dSo.displayCultureDate(isEdit) & "</td>")

		'response.write("<td>" & StartLabel & "&nbsp;<input type=""text"" name=""frm_start_date_only"" size=""15"" onBlur=""createradioondate(this.value);countDays();"" value=""" & IntlStartDate & """>" & VbCrLf)
		'response.write("&nbsp;<a href=""#"" onClick=""javascript:document.calendar.frm_start_date_only.value = Trim(document.calendar.frm_start_date_only.value);CallCalendar(document.calendar.frm_start_date_only.value, 'calendardateonly.aspx', 'frm_start_date_only', 'calendar', '" & siteRef.ContentLanguage & "');return false;""><img src=""" & AppImgPath & "btn_calendar-nm.gif"" alt=""" & gtMess("alt calendar button text") & """ title=""" & gtMess("alt calendar button text")& """ border=""0""></a></td>" & VbCrLf)
		'response.write("<td>" & EndLabel & "&nbsp;<input type=""text"" name=""frm_end_date_only"" size=""15"" onBlur=""countDays();"" value=""" & IntlEndDate & """>" & VbCrLf)
		'response.write("&nbsp;<a href=""#"" onClick=""javascript:document.calendar.frm_end_date_only.value = Trim(document.calendar.frm_end_date_only.value);CallCalendar(document.calendar.frm_start_date_only.value, 'calendardateonly.aspx', 'frm_end_date_only', 'calendar', '" & siteRef.ContentLanguage & "');return false;""><img src=""" & AppImgPath & "btn_calendar-nm.gif"" alt=""" & gtMess("alt calendar button text") & """ title=""" & gtMess("alt calendar button text") & """ border=""0""></a></td>" & VbCrLf)
	End If
	response.write("</tr>" & VbCrLf)
	'response.write("<tr>" & VbCrLf)
	'response.write("<td colspan=""2"">" & VbCrLf)
	' TODO: Add Num of Events back in...
	'Response.Write("&nbsp;<br/>")
	' response.write("&nbsp;Number of Events")
	' If (Not isEdit) Then
	'	response.write(" To Be Created")
	' End If
	' response.write(": <span id=""time_between"" name=""time_between"">N/A</span>&nbsp;<br />" & VbCrLf)
	'response.write("</td>" & VbCrLf)
	'response.write("</tr>" & VbCrLf)
	'response.write("<tr>" & VbCrLf)
	'response.write("<td colspan=""2""><hr /></td>" & VbCrLf)
	'response.write("</tr>" & VbCrLf)
	response.write("<tr>" & VbCrLf)
	response.write("<td width=""50%""><span class=""ektronHeader"">" & mMsgs.GetMessage("lbl evt will occur") & "</span></td>" & VbCrLf)
	response.write("<td width=""50%""><span class=""ektronHeader"">" & mMsgs.GetMessage("lbl evt times") & "</span></td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("<td>" & VbCrLf)
	response.write("<table>" & VbCrLf)
	response.write("<tr class=""yesBG"" id=""tr_daily"">" & VbCrLf)
	response.write("<td>" & VbCrLf)
	response.write(Radio("daily", recurType, isEdit) & "&nbsp;" & mMsgs.GetMessage("lbl every day") & "</td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("<tr class=""noBG"" id=""tr_weekly"">" & VbCrLf)
	response.write("<td>" & VbCrLf)
	' response.write(Radio("weekly", recurType, isEdit) & "&nbsp;Every <span id=""pref_weekday1"" name=""pref_weekday1"">weekday</span></td>" & VbCrLf)
	response.write(Radio("weekly", recurType, isEdit) & "&nbsp;" & mMsgs.GetMessage("lbl every weekday") & "</td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("<tr class=""noBG"" id=""tr_monthlybyday"">" & VbCrLf)
	response.write("<td>" & VbCrLf)
	' response.write(Radio("monthlybyday", recurType, isEdit) & "&nbsp;Every <span id=""pref_day1"" name=""pref_day1"">#</span> of every Month</td>" & VbCrLf)
	response.write(Radio("monthlybyday", recurType, isEdit) & "&nbsp;" & mMsgs.GetMessage("lbl every selected date of every month") & "</td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("<tr class=""noBG"" id=""tr_monthlybyweekday"">" & VbCrLf)
	response.write("<td>" & VbCrLf)
	' response.write(Radio("monthlybyweekday", recurType, isEdit) & "&nbsp;Every <span id=""pref_weekcount1"" name=""pref_weekcount1"">first</span> <span id=""pref_weekday2"" name=""pref_weekday2"">weekday</span> of every Month</td>" & VbCrLf)
	response.write(Radio("monthlybyweekday", recurType, isEdit) & "&nbsp;" & mMsgs.GetMessage("lbl every first selected weekday of every month") & "</td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("<tr class=""noBG"" id=""tr_yearlybymonthbyday"">" & VbCrLf)
	response.write("<td>" & VbCrLf)
	' response.write(Radio("yearlybymonthbyday", recurType, isEdit) & "&nbsp;Yearly, every <span id=""pref_day2"" name=""pref_day2"">#</span> of <span id=""pref_month1"" name=""pref_month1"">Month</span></td>" & VbCrLf)
	response.write(Radio("yearlybymonthbyday", recurType, isEdit) & "&nbsp;" & mMsgs.GetMessage("lbl yearly every selected date of month") & "</td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("<tr class=""noBG"" id=""tr_yearlybymonthbyweekday"">" & VbCrLf)
	response.write("<td>" & VbCrLf)
	' response.write(Radio("yearlybymonthbyweekday", recurType, isEdit) & "&nbsp;Yearly, every <span id=""pref_weekcount2"" name=""pref_weekcount2"">first</span> <span id=""pref_weekday3"" name=""pref_weekday3"">weekday</span> of <span id=""pref_month3"" name=""pref_month3"">Month</span></td>" & VbCrLf)
	response.write(Radio("yearlybymonthbyweekday", recurType, isEdit) & "&nbsp;" & mMsgs.GetMessage("lbl yearly every first selected weekday of month") & "</td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("</table>")
	response.write("<input type=""hidden"" name=""currentSel"" id=""currentSel"" value=""daily"" />" & VbCrLf)
	response.write("</td>" & VbCrLf)
	response.write("<td valign=""top"" align=""center"">" & VbCrLf)
	response.write("<table width=""100%"">" & VbCrLf)
	response.write("<tr><td colspan=""2"">&nbsp;<br /></td></tr>" & VbCrLf)
	response.write("<tr>" & VbCrLf)
	response.write("<td align=""right"" valign=""bottom"">" & StartLabel & "&nbsp;</td>")
		dSo.formElement = "frm_start_time"
		dSo.spanId = "frm_start_time_span"
		' dSo.targetDate = CDate(Left(sTime,InStr(1,sTime," ")))
		dSo.targetDate = CDate(sTime)
		if action<>"AddEvent" then
			response.write("<td>" & dSo.displayCultureTime(isEdit) & "</td>")
		else
			response.write("<td>" & dSo.displayCultureTime(isEdit,"add_event") & "</td>")
		end if
	' response.write("<td><input type=""text"" name=""frm_start_time"" value=""" & Left(sTime,InStr(1,sTime," ")) & """ size=""8"" onBlur=""javascript: EndTimeAddHour('frm_start_time', 'frm_start_time_ampm', 'frm_end_time', 'frm_end_time_ampm');"">&nbsp;" & VbCrLf)
	' ampmDD (sTime, "frm_start_time", "frm_start_time_ampm", "frm_end_time", "frm_end_time_ampm")
	' response.write("<br /></td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("<tr>" & VbCrLf)
	response.write("<td align=""right"" valign=""bottom"">" & EndLabel & "&nbsp;</td>" & VbCrLf)
		dSo.formElement = "frm_end_time"
		dSo.spanId = "frm_end_time_span"
		' dSo.targetDate = CDate(Left(eTime,InStr(1,eTime," ")))
		dSo.targetDate = CDate(eTime)
		' response.write("<scr" & "ipt>alert('" & eTime & "');</scr" & "ipt>")
		if action<>"AddEvent" then
			response.write("<td>" & dSo.displayCultureTime(isEdit) & "</td>")
		else
			response.write("<td>" & dSo.displayCultureTime(isEdit,"add_event") & "</td>")
		end if
	'response.write("<td><input type=""text"" name=""frm_end_time"" value=""" & Left(eTime,InStr(1,eTime," ")) & """ size=""8"">&nbsp;" & VbCrLf)
	'ampmDD (eTime, "", "frm_end_time_ampm", "", "")
	'response.write("<br /></td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("<tr><td colspan=""2"">&nbsp;<br /></td></tr>" & VbCrLf)
	response.write("</table>" & VbCrLf)
	response.write("</td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("</table>" & VbCrLf)
	response.write("<!------------------------------------------------------------------->" & VbCrLf)
	response.write("<!------------------------------------------------------------------->" & VbCrLf)
	response.write("</div>" & VbCrLf)
	response.write("<div align=""left""><input type=""checkbox"" name=""frm_show_start_time"" ")
	if (ShowStime) then
		response.write("checked=""true"" ")
	end if
	response.write(">" & mMsgs.GetMessage("lbl display times") & "&nbsp;&nbsp;</div>" & VbCrLf)
	response.write("</td>" & VbCrLf)
	response.write("</tr>" & VbCrLf)
	response.write("</table>" & VbCrLf)
	response.write("<script type=""text/javascript"">" & VbCrLf)
	response.write("if (document.getElementById(""frm_start_date_only"").value!='') {" & VbCrLf)
	response.write("createradioondate();" & VbCrLf)
	response.write("// Set the highlight to the proper table row" & VbCrLf)
	response.write("flipRecursBG('" & recurType & "');" & VbCrLf)
	If (isEdit) Then
		response.write("// select recursion tab" & VbCrLf)
		' response.write("countDays();" & VbCrLf)
		If (recursID > 0) Then
			response.write("flipTab('2');" & VbCrLf)
		End If
	End If
	response.write("}" & VbCrLf)
	response.write("</scr")
	response.write("ipt>")
	return Nothing
End Function

Function LongDescriptionDisplay(ByVal thisName as Object,ByVal selectedVal as Object) as Object
	Dim s as String = ""
	Dim sl as string
	If (thisName <> "") Then
		s = "<select name=""" & thisName & """>"
		sl = ""
		If (selectedVal = 0) Then
			sl = " selected=""true"""
		End If
		s &= "<option value=""0""" & sl & ">None</option>"
		sl = ""
		If (selectedVal = 1) Then
			sl = " selected=""true"""
		End If
		s &= "<option value=""1""" & sl & ">Text Only</option>"
		sl = ""
		If (selectedVal = 2) Then
			sl = " selected=""true"""
		End If
		s &= "<option value=""2""" & sl & ">Rich Text (XHTML)</option>"
		s &= "</select>"
	Else
		Select Case selectedVal
			Case 0 : s = "None"
			Case 1 : s = "Text Only"
			Case 2 : s = "Rich Text (XHTML)"
		End Select
	End If

	Return (s)

End Function
</script>

<%
'       '
' SETUP '
'       '

currentUserID=siteRef.UserId
AppImgPath = AppWebUI.AppImgPath
imgIconPath = AppWebUI.AppPath & "images/ui/icons/"
AppName = AppWebUI.AppName
AppPath = AppWebUI.AppPath
AppeWebPath = AppWebUI.AppPath & AppWebUI.AppeWebPath
mMsgs = AppWebUI.EkMsgRef

If (Request.QueryString("LangType")<>"") Then
		ContentLanguage=Request.QueryString("LangType")
		siteRef.SetCookieValue("LastValidLanguageID", ContentLanguage)
	else
		if CStr(siteRef.GetCookieValue("LastValidLanguageID")) <> ""  then
			ContentLanguage = siteRef.GetCookieValue("LastValidLanguageID")
		end if
End If

AppUI.ContentLanguage=ContentLanguage
siteRef.ContentLanguage=ContentLanguage

title1 = request.QueryString("title")
descrp = request.QueryString("description")
'msgs = "BrowserCode,generic Title, js:no items selected, alt back button text, generic page error message, " _
'& "click to sort msg, generic View, generic title label, description label, id label, " _
'& "content LUE label, content DC label, content LED label, generic ID, generic URL Link, " _
'& "generic select all msg, generic clear all msg, foldername label, generic Date Modified, " _
'& "js: confirm collection deletion msg, js: collection title required msg, alt: save collection text, " _
'& "generic template label, generic include subfolders msg, alt: update collection text, " _
'& "view collection title, alt: add collection items text, alt: remove collection items text, " _
'& "alt: reorder collection text, alt: edit collection text, alt: delete collection text, " _
'& "alt: update collection order text, move selection up msg, move selection down msg, " _
'& "delete collection items title, alt: delete collection items text, add collection items title, " _
'& "view collections title, alt: generic previous dir text, alt: add new collection text, " _
'& "alt: generic view folder content text, alt: add selected collection items text, add collection title, " _
'& "reorder collection title, generic first msg, edit collection title,alt add content button text, " _
'& "collections: leave template empty, collections report title bar, generic Description, generic Path," _
'& "js: invalid date format error msg,js: invalid year error msg,js: invalid month error msg,js: invalid day error msg," _
'& "js: invalid time error msg,com: user does not have permission, " _
'& "alt calendar button text, btn add calendar, btn back, btn delete, btn event types, btn edit, btn show calendar, " _
'& "btn save, btn add, btn minus, btn add event, btn add library, btn delete content, del cal evt, del cal recur evt, btn delete recur"


' gtMsgObj = siteRef.EkMessageRef
' gtMess = gtMsgObj.GetMsgsByTitleTwo( msgs, currentUserID)
if (ErrorString <> "") then
	Response.Write(ErrorString)
end if

dSo = siteRef.EkDTSelectorRef
dSo.extendedMeta = True

platform = Request.ServerVariables("HTTP_USER_AGENT")
If (inStr(platform,"Windows")>0) then
	IsMac = 0
Else
	IsMac = 1
End If
If (Request.Browser.Type.IndexOf("IE") <> -1) Then
    IsBrowserIE = True
End If
'Debug
'IsMac = 1
'IsBrowserIE = false
'/Debug

VerfiyTrue = "<img src=""" & imgIconPath & "check.png"" border=""0"" alt=""Enabled"" title=""Enabled"">"
VerfiyFalse = "<img src=""" & AppImgPath & "icon_redx.gif"" border=""0"" alt=""Disabled"" title=""Disabled"">"

action = Request.QueryString("action")
folderId = Request.QueryString("folderid")
ParentFolderID = Request.QueryString("parentfolderid")
if folderId = "" then
	folderId = 0
	parentfolderid = 0
end if
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
<title></title>
    <asp:Panel runat="server" id="pnl_Page">
	<%
		Response.Expires=-1
		Response.AddHeader("Pragma","no-cache")
		Response.AddHeader("cache-control", "no-store")

	ekContObj = siteRef.EkContentRef
	var2 = ekContObj.GetEditorVariablev2_0(0, "eventedit")

	%>
	<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
	<meta http-equiv="pragma" content="no-cache"/>

	<title><%=(AppName & " " & "Collections")%></title>
	<script type="text/javascript"> var AutoNav ; </script>
	<style type="text/css">
	    .minWidthDate { width: 100px !important; }
	    .leftLabel{ color:#1d5987; width:10%; white-space:nowrap; font-weight:bold;}
	</style>
	<script type="text/javascript">
	<!--//--><![CDATA[//><!--
	Ektron.ready(function(){
	    $ektron("div.noDateClass span").removeClass("dateClass");
	    $ektron("table#minWidthDate span").addClass("minWidthDate");
	});
	var invalidFormatMsg = "<%=(mMsgs.GetMessage("js: invalid date format error msg"))%>";
	var invalidYearMsg = "<%=(mMsgs.GetMessage("js: invalid year error msg"))%>";
	var invalidMonthMsg = "<%=(mMsgs.GetMessage("js: invalid month error msg"))%>";
	var invalidDayMsg = "<%=(mMsgs.GetMessage("js: invalid day error msg"))%>";
	var invalidTimeMsg = "<%=(mMsgs.GetMessage("js: invalid time error msg"))%>";
	var ecmMonths = "empty,<%= siteRef.GetClientMonthNames() %>" ;
	var ecmDays = "<%= siteRef.GetClientDayNames() %>" ;
	var elx1;
	var jsDefaultContentLanguage = '<asp:Literal runat="server" id="jsDefaultContLang" />';
	var jsId = '<asp:Literal runat="server" id="jsId" />'

	function ConfirmCalendarDelete() {
		return confirm('<%=mMsgs.GetMessage("alt Are you sure you wish to delete this calendar item Continue?")%>');
	}

	function ConfirmCalendarDeleteRecur() {
		return confirm('<%=mMsgs.GetMessage("alt Are you sure you wish to delete this calendar item and all other recurring calendar items Continue?")%>');
	}

	function ConfirmEvTypesDelete() {
		var flipper = false;
		// Iterate through the list of Checkboxes and make sure that at least one is checked.
		for(var x=0; x < document.forms[0].elements.length; x++) {
			var e = document.forms[0].elements[x];
			if(e.name=='selEventType'){
				if (e.checked) {
					flipper = true ;
					break;
				}
			}
		}

		if(flipper) {
			return confirm('<%=mMsgs.GetMessage("alt Deleting the selected Event Type(s) will remove any associations to Events using them. Continue?")%>') ;
		}
		else
		{
			alert('<%=mMsgs.GetMessage("alt Please select at least one Event Type to remove.")%>') ;
			return false;
		}
	}

	function validateEvTypeAdd(formName) {

		if(document.forms[0].frm_evTypeName.value.length==0) {
			alert('<%=mMsgs.GetMessage("alt You may not enter a blank Event Type name")%>');
			return false ;
		} else {
			return true ;
		}

	}

	function checkSelectEdit() {

		if(document.forms[0].selEventType.value == '') {
			alert('<%=mMsgs.GetMessage("alt You must select an Event Type to continue.")%>') ;
			return false ;
		} else {
			return true ;
		}

	}

	function deleteOption(object,index) {
		object.options[index] = null;

		if (object.length == 0) {
			var optionName = new Option('No Event Types', '-1', false, false)
			object.options[0] = optionName;
		}

	}

	function addOption(object,text,value) {
		var defaultSelected = true;
		var selected = true;
		var optionName = new Option(text, value, defaultSelected, selected) ;

		for (var y=0, t=object.length; y < t; y++) {
			if(object.item(y).value == '-1') {
				object.options[y] = null ;
			}
		}

		object.options[object.length] = optionName;
	}

	function copySelected(fromObject,toObject) {
		for (var i=0, l=fromObject.options.length; i<l; i++) {
			if (fromObject.options[i].selected&&fromObject.options[i].value!='-1')
				addOption(toObject,fromObject.options[i].text,fromObject.options[i].value);
		}
		for (var i=fromObject.options.length-1;i>-1;i--) {
			if (fromObject.options[i].selected)
				deleteOption(fromObject,i);
		}
	}
	//--><!]]>
	</script>
	<script type="text/javascript">
	<!--//--><![CDATA[//><!--
		function selectAllDelEvTypes(){
		for(var x=0; x < document.forms.deleteform.length; x++) {
			if (document.forms.deleteform[x].type == "checkbox")
				document.forms.deleteform[x].checked = true;
		}
		}

		function unSelectAllDelEvTypes() {
			for(var x=0; x < document.forms.deleteform.length; x++) {
				if (document.forms.deleteform[x].type == "checkbox")
					document.forms.deleteform[x].checked = false;
			}
		}

		function NavigateFolder(actionItem,FID){
		var tmpHref =  'cmscalendar.aspx?action=' + actionItem;
		//alert('The action Item is ->' + actionItem);
		tmpHref = tmpHref + '&folderid=' + FID;
		tmpHref = tmpHref + '&calendar_id=' + document.forms.calendar.frm_calendar_id.value;
		tmpHref = tmpHref + '&iYear=' + document.forms.calendar.frm_year.value;
		tmpHref = tmpHref + '&iMonth=' + document.forms.calendar.frm_month.value;
		tmpHref = tmpHref + '&iDay=' + document.forms.calendar.frm_day.value;
		tmpHref = tmpHref + '&start_time=' + escape(document.forms.calendar.frm_event_start.value);
		if (document.forms.calendar.frm_event_location.value.length){
			tmpHref = tmpHref + '&event_location=' + escape(document.forms.calendar.frm_event_location.value);
		}
		if (document.forms.calendar.frm_event_end.value.length ){
			tmpHref = tmpHref + '&end_time=' + escape(document.forms.calendar.frm_event_end.value);
		}
		if(document.forms.calendar.frm_show_start_time.checked == true){
			tmpHref = tmpHref + '&show_start=true';
		}
		//if(document.forms.calendar.frm_show_end_time.checked == true) {
		//	tmpHref = tmpHref + '&show_end=true';
	   	//}
		//alert(tmpHref);
		self.location.href = tmpHref;
		return false;
	}
	function NavForCalendar(actionItem,FID){
		var tmpHref =  'cmscalendar.aspx?action=' + actionItem;
		//alert('The action Item is ->' + actionItem);
		tmpHref = tmpHref + '&folderid=' + FID;
		tmpHref = tmpHref + '&calendar_id=' + document.forms[0].frm_calendar_id.value;
		tmpHref = tmpHref + '&table_height=' + document.forms[0].frm_table_height.value;
		tmpHref = tmpHref + '&table_width=' + document.forms[0].frm_table_width.value;
		tmpHref = tmpHref + '&description=' + escape(document.forms[0].frm_calendar_description.value);
		tmpHref = tmpHref + '&title=' + escape(document.forms[0].frm_calendar_title.value);
		tmpHref = tmpHref + '&location_label=' + escape(document.forms[0].frm_location_label.value);
		tmpHref = tmpHref + '&start_label=' + escape(document.forms[0].frm_start_label.value);
		tmpHref = tmpHref + '&end_label=' + escape(document.forms[0].frm_end_label.value);

		tmpHref = tmpHref + '&evtype_label=' + escape(document.forms[0].frm_evtype_label.value);
		if(document.forms[0].frm_ev_type_avail.checked==true) {
			tmpHref = tmpHref + '&ev_type_avail=on' ;
		} else {
			tmpHref = tmpHref + '&ev_type_avail=off' ;
		}
		if(document.forms[0].frm_ev_type_req.checked==true) {
			tmpHref = tmpHref + '&ev_type_req=on' ;
		} else {
			tmpHref = tmpHref + '&ev_type_req=off' ;
		}

		if(document.forms[0].frm_show_weekend.checked == true){
			tmpHref = tmpHref + '&show_weekend=true';
		}

		//alert(tmpHref);
		self.location.href = tmpHref;
		return false;
	}

		function CallCalendar2(){
			//alert('Date-> ' + endDefualt);
			if (document.calendar.frm_event_end.value.length == 0 ){
				CallCalendar(document.calendar.frm_event_start.value, 'calendar.aspx', 'frm_event_end', 'calendar', '<%= siteRef.ContentLanguage %>');
			}
			else{
				CallCalendar(document.forms[0].frm_event_end.value, 'calendar.aspx', 'frm_event_end', 'calendar', '<%= siteRef.ContentLanguage %>');
			}
			return false;
		}

		function SetContentChoice(cTitle,cId,Qlink){
			document.forms[0].frm_event_title.value = cTitle;
			document.forms[0].frm_content_id.value = cId;
			document.forms[0].frm_qlink.value = Qlink;
			//document.forms.calendar.frm_use_qlink.checked = true;
			SetBrowserState();
			return false;
		}

		function SubmitForm(FormName, Validate) {
			if (Validate.length > 0) {
				if (eval(Validate)) {
					document.forms[0].submit();
					return false;
				}
				else {
					return false;
				}
			}
			else {
				document.forms[0].submit();
				return false;
			}
		}
		function FinalFormSubmit()
		{
			document.forms[0].submit();
		}
		function CopyDescFromEphox(src)
		{
			document.forms[0].frm_eventLongDesc.value = src;
			FinalFormSubmit();
		}
		function SubmitFormWeWEP(FormName, Validate)
		{
			if (Validate.length > 0)
			{
				if (eval(Validate))
				{
					if ('VerifyAddEventForm()' == Validate)
					{
						var editor = Ektron.ContentDesigner.instances["LongDescription"];
						if (editor)
						{
				            document.getElementById("frm_eventLongDesc").value = editor.getContent();
				        }
						FinalFormSubmit();
					}
					return false;
				}
				else
				{
					return false;
				}
			}
			else
			{

					FinalFormSubmit();

				return false;
			}
		}



		function Trim (string) {
			if (string.length > 0) {
				string = RemoveLeadingSpaces (string);
			}
			if (string.length > 0) {
				string = RemoveTrailingSpaces(string);
			}
			return string;
		}

		function RemoveLeadingSpaces(string) {
			while(string.substring(0, 1) == " ") {
				string = string.substring(1, string.length);
			}
			return string;
		}

		function RemoveTrailingSpaces(string) {
			while(string.substring((string.length - 1), string.length) == " ") {
				string = string.substring(0, (string.length - 1));
			}
			return string;
		}

		function CheckKeyValue(item, keys) {
			var keyArray = keys.split(",");
			for (var i = 0; i < keyArray.length; i++) {
				if ((document.layers) || ((!document.all) && (document.getElementById))) {
					if (item.which == keyArray[i]) {
						return false;
					}
				}
				else {
					if (event.keyCode == keyArray[i]) {
						return false;
					}
				}
			}
		}
/* REMOVE BELOW TO COMMENT
		function UpDateFrmDates(){
			var dateStr;
			dateStr = document.calendar.frm_event_start.value;
			var date_times = dateStr.split(" ");
			var myDate = date_times[0].split("-");
			document.calendar.frm_day.value = myDate[0];
			//alert('Setting new day ->'+ myDate[0])
			document.calendar.frm_month.value = MonthValue(myDate[1]);
			document.calendar.frm_year.value = myDate[2];
			return true;
			}
*/
		function MonthValue(m){
			var iLoop;
			var list = '<%=siteRef.getenglishMonthsAbbrev%>'
			var mArray = list.split(",")
			for(iLoop = 0; iLoop < mArray.length; iLoop++)
			{
				if (m.toLowerCase() == mArray[iLoop].toLowerCase())
				{
					break;
				}
			}
			return iLoop;
		}

	function VerifyAddEventForm()
	{
		if((document.forms[0].updateAll)&&(document.forms[0].frm_editdate_only))
		{
			if (!document.forms[0].updateAll.checked) {
				document.forms[0].frm_start_date_only.value =
					document.forms[0].frm_editdate_only.value ;

				document.forms[0].frm_end_date_only.value =
					document.forms[0].frm_editdate_only.value ;
			}
		}

		if(!verifyFormDates()) { return (false) ; }

		if (document.forms[0].frm_event_title.value.length == 0)
		{
			alert('Error: Missing event title');
			return false;
		}

		if(document.forms[0].frm_event_end.value.length != 0 )
		{
		/*
			TODO: Find new method to compare date and time.
			if (!compareDateAndTime(document.forms.calendar.frm_event_start.value, document.forms.calendar.frm_event_end.value))
			{
				alert('Your End Date and Time must be\nlater than your Start Date and Time.');
				return false;
			}
		*/
		}

		// Only proceed if Event Types are available
		if (document.forms[0].frm_ev_type_avail.value == 'True')
		{
			if ((document.forms[0].selEvTypes.options.length==1)&&(document.forms[0].frm_ev_type_req.value == 'True'))
			{
				// if there is only one option in selEvTypes then it may be the default "No Event Types", carry on
				// if the event type is required there is no need to check this.
				if(document.forms[0].selEvTypes.options[0].value == '-1')
				{
					alert ('At least one Event Type is required for this Event.') ;
					return false;
				}
			}
			// alert(document.forms.calendar.selEvTypes.options.length) ;
			for (var d=0; d < document.forms[0].selEvTypes.options.length; d++)
			{
				if(document.forms[0].selEvTypes.options[d].value!='-1')
				{
					document.forms[0].selEvTypes.options[d].selected = true ;
				}
				else
				{
					document.forms[0].selEvTypes.options[d].selected = false ;
				}
			}
		}

		// return UpDateFrmDates();
		return(true) ;
	}

	function verifyFormDates()
	{

		//--------------------------------------------------
		// Added tests performed on the server-side here, to
		// allow user to correct without losing their work:
		//
		//  1.) If recurring, Fail if start date not before end date:
		//  2.) If not recurring, fail if start time not before end time
		//--------------------------------------------------

		if (document.getElementById("frm_recursive").value=='1')
		{
			// Need a start date and an end date to have a recurring event
			//
			if ("" == document.getElementById("frm_start_date_only").value || "" == document.getElementById("frm_end_date_only").value)
			{
				alert('For recurring events you must have\nboth a start and an end date.') ;
				return false ;
			}
			// And those dates need to be different.
			if(document.getElementById("updateAll"))
			{
			    // If update All is not checked the start date and end date will be the same for recurring events.
				if((document.getElementById("frm_start_date_only").value == document.getElementById("frm_end_date_only").value) &&
						(document.getElementById("updateAll").checked))
				{
					alert('For recurring events, Start Date and\nEnd Date must not be the same.');
					return false ;
				}
			}
			else
			{
				if(document.getElementById("frm_start_date_only").value == document.getElementById("frm_end_date_only").value)
				{
					alert('For recurring events, Start Date and\nEnd Date must not be the same.');
					return false ;
				}
			}
			var startYear = getFieldIntValue("frm_start_date_only_yrnum");
			var startMonth = getFieldIntValue("frm_start_date_only_monum");
			var startDay = getFieldIntValue("frm_start_date_only_dom");
			var startHour = getFieldIntValue("frm_start_time_hr");
			var startMin = getFieldIntValue("frm_start_time_mi");
			var endYear = getFieldIntValue("frm_end_date_only_yrnum");
			var endMonth = getFieldIntValue("frm_end_date_only_monum");
			var endDay = getFieldIntValue("frm_end_date_only_dom");
			var endHour = getFieldIntValue("frm_end_time_hr");
			var endMin = getFieldIntValue("frm_end_time_mi");
			if ((startYear < endYear)
				|| ((startYear == endYear)
					&& ((startMonth < endMonth)
						|| ((startMonth == endMonth)
							&& ((startDay < endDay)
								|| ((startDay == endDay)
									&& ((startHour < endHour)
										|| ((startHour == endHour)
											&& (startMin < endMin) ))))))))
			{
				// Okay: start before end.

				// Update: Start time must be before end time
				// - even for recurring events (we don't handle
				// wrapping events overnight properly...):

				if ((startHour > endHour) || ((startHour == endHour) && (startMin > endMin) ))
				{
					alert('Start Time must be before End Time.');
					return false;
				}
			}
			else
			{
				// Error: start not before end.
				alert('For recurring events, \nStart Date must be before End Date.');
				return false;
			}

			// Put together Date & Time and place them in hidden forms for post-processing.
			document.forms[0].frm_event_start.value = document.forms[0].frm_start_date_only.value + ' ' + document.forms[0].frm_start_time.value ;
			document.forms[0].frm_event_end.value = document.forms[0].frm_end_date_only.value + ' ' +  document.forms[0].frm_end_time.value ;

		}
		else
		{
			// Must have an event date
			if ("" == document.getElementById("frm_event_start_single").value)
			{
				alert('You must have an event date.') ;
				return false ;
			}
			var startHour = getFieldIntValue("frm_start_time_single_hr");
			var startMin = getFieldIntValue("frm_start_time_single_mi");
			var endHour = getFieldIntValue("frm_end_time_single_hr");
			var endMin = getFieldIntValue("frm_end_time_single_mi");

			if(isNaN(startHour)&& isNaN(startMin)&& isNaN(endHour)&& isNaN(endMin))
			{
//              Code that sets the calender event start and end timings as one hour from now
//			     var now = new Date();
//			     startMin = now.getMinutes();
//			     if((startMin % 5) != 0)
//			       startMin = startMin +  (5 - (startMin % 5));
//			     endMin = startMin;
//			     startHour = now.getHours();
//			     endHour = startHour  + 1;
//
//			     if(startHour>0 && startHour<10)
//			     startHour = "0" + startHour;
//			     if(endHour>0 && endHour<10)
//			     endHour = "0" + endHour;

			     document.forms[0].frm_event_start.value = document.forms[0].frm_event_start_single.value +' '+ "00:00";
			     document.forms[0].frm_event_end.value = document.forms[0].frm_event_start_single.value + ' ' + "23:55";

			}
			else{
				    if ((startHour < endHour) || ((startHour == endHour) && (startMin < endMin) ))
			           {
				            // Okay: start before end.
			           }
			        else
			           {
				            // Error: start not before end.
				            alert('Start Time must be before End Time.');
				            return false;
			           }
			    // Put together Date & Time and place them in hidden forms for post-processing.
			    document.forms[0].frm_event_start.value = document.forms[0].frm_event_start_single.value + ' ' + document.forms[0].frm_start_time_single.value ;
			    document.forms[0].frm_event_end.value = document.forms[0].frm_event_start_single.value + ' ' + document.forms[0].frm_end_time_single.value ;
			}
		}


		return (true)
	}

	function getFieldIntValue(fieldName)
	{
		var objVal = 0;
		var fldObj = document.getElementById(fieldName);
		if (fldObj)
		{
			if ((undefined != fldObj.value) && (null != fldObj.value))
			{
				if (! isNaN(fldObj.value))
				{
					objVal = parseInt(fldObj.value, 10);
				}
			}
		}
		return (objVal);
	}

		// determines if time is in 24 hour format (positive logic):
		/*function Detect24HrFormat(Date_Time) {
			var idx, timePat, matchArray;

			if ((typeof(Date_Time) != "string")
				|| (Date_Time.length == 0)) {
				return false;
			}
			idx = Date_Time.indexOf(" ");
			if ((idx <= 0) || (idx >= Date_Time.length)) {
				return false;
			}
			time_value = Date_Time.substring(idx+1, Date_Time.length);
			if (time_value.length == 0) {
				return false;
			}

			timePat = /^(\d{1,2}):(\d{2})(:(\d{2}))?(\s?(AM|am|PM|pm))?$/;
			matchArray = time_value.match(timePat);
			if (matchArray == null) {
				return false;
			}
			hour = matchArray[1];
			if ((typeof(hour) == "string") && (hour.length > 0)) {
				hour = parseInt(hour);
			}
			if (hour > 12) {
				return true;
			}
			return false;
		}*/

		function SetBrowserState()
		{
			/*
			if (document.forms.calendar.frm_use_qlink.checked == true){
				document.forms.calendar.frm_launch_new_browser.disabled = false;
			}
			else{
				document.forms.calendar.frm_launch_new_browser.checked = false;
				document.forms.calendar.frm_launch_new_browser.disabled = true;
			}
			*/
			return false;
		}

		function VerifyAddForm ()
		{

			regexp1 = /"/gi;
			document.getElementById("frm_calendar_title").value = Trim(document.getElementById("frm_calendar_title").value);
			document.getElementById("frm_calendar_title").value = document.getElementById("frm_calendar_title").value.replace(regexp1, "'");

			document.getElementById("frm_calendar_description").value = Trim(document.getElementById("frm_calendar_description").value);
			document.getElementById("frm_calendar_description").value = document.getElementById("frm_calendar_description").value.replace(regexp1, "'");
			if (document.getElementById("frm_calendar_title").value == "")
			{
				alert ("<%=("calendar title required ")%>");
				document.getElementById("frm_calendar_title").focus();
				return false;
			}
			if ( !document.getElementById("frm_table_height").value.length ||  !document.getElementById("frm_table_width").value.length ){
				alert('Missing table cell values');
				return false;
			}
/*
			if (!emp_checkinteger(document.forms.calendar.frm_table_height.value)){
				alert('Table cell height value is not numeric');
				return false;
			}
*/
			if (document.getElementById("frm_table_height").value == 0){
				alert('Error: Table height value can not be zero.');
				return false;
			}
			if (document.getElementById("frm_table_width").value == 0){
				alert('Error: Table width value can not be zero.');
				return false;
			}
/*
			if (!emp_checkinteger(document.forms.calendar.frm_table_width.value)){
				alert('Table cell width value is not numeric');
				return false;
			}
*/
			if ((!document.forms[0].frm_ev_type_avail.checked)&&(document.forms[0].frm_ev_type_req.checked)) {
				alert('Event Types must be selected as available\nto make them required.') ;
				return false;
			}
			return true;
		}

	function EndTimeAddHour(frmStartT, frmStartAM, frmEndT, frmEndAM)
	{
	    var endH = '12', endM = '00', endS = '00', endMer = 'AM' ;

	    if(document.forms.calendar[frmStartT].value=='') { return } ;

	    /* if( (RemoveTrailingSpaces(document.forms.calendar[frmEndT].value)!='12:00') &&
			    (document.forms.calendar[frmEndAM].value!='AM') )
	    { return } ; */

	    var tStr = document.getElementById("frmStartT").value ;
	    var sSpl = tStr.split(':') ;

	    if((sSpl.length==3)||(sSpl.length==2))
	    {
		    var tNum = parseInt(sSpl[0],10) ;
		    if (!isNaN(tNum))
		    {
			    // If the user enters a time in the 12th hour, the 12 hour clock is assumed.
			    //
			    if (tNum==12)
			    {
				    tNum = 1 ;
			    }
			    else
			    {
				    tNum += 1 ;
			    }

			    endM = sSpl[1] ;
			    endMer = document.forms.calendar[frmStartAM].value ;
			    if(sSpl.length==3)
			    {
				    endS = sSpl[2] ;
			    }

			    if(tNum>24)
			    {
				    return ;
			    }
			    else if(tNum==12)
			    {
				    if(endMer=='PM')
				    {
					    endH = '11' ; endM = '59' ; endS = '59' ; endMer = 'PM' ;
				    }
				    else
				    {
					    endH = '12' ; endMer = 'PM' ;
				    }
			    }
			    else if(tNum==24)
			    {
				    endH = '23' ; endM = '59' ; endS = '59' ;
			    }
			    else
			    {
				    endH = tNum ;
			    }

			    document.forms.calendar[frmEndAM].value = endMer ;
			    document.forms.calendar[frmEndT].value = endH + ':' + endM ;
			    if(sSpl.length==3)
			    {
				    document.forms.calendar[frmEndT].value += ':' + endS ;
			    }
		    }
	    }
	    return ;
    }


	function flipEvTypeReq()
	{
		if (!document.forms[0].frm_ev_type_avail.checked)
		{
			document.forms[0].frm_ev_type_req.disabled = true ;
		}
		else
		{
			document.forms[0].frm_ev_type_req.disabled = false ;
		}
	}

	function makeArray()
	{
		for (var i = 0; i<makeArray.arguments.length; i++)
			this[i + 1] = makeArray.arguments[i];
	}

	function addDayPost(inpNumber)
	{
		var inArr = new makeArray('1st','2nd','3rd','4th','5th','6th','7th','8th','9th','10th','11th','12th','13th','14th','15th','16th','17th','18th','19th','20th','21st','22nd','23rd','24th','25th','26th','27th','28th','29th','30th','31st') ;
		return inArr[inpNumber] ;
	}

	function stringWCountPost(inWC)
	{
		var inArr = new makeArray('first','second','third','fourth','last') ;
		return inArr[inWC] ;
	}

	function makeHighlight()
	{
		document.getElementById('pref_weekday1').className = 'HLight' ;
		document.getElementById('pref_weekday2').className = 'HLight' ;
		document.getElementById('pref_weekday3').className = 'HLight' ;
		document.getElementById('pref_day1').className = 'HLight' ;
		document.getElementById('pref_day2').className = 'HLight' ;
		document.getElementById('pref_month1').className = 'HLight' ;
		// document.getElementById('pref_month2').className = 'HLight' ;
		document.getElementById('pref_month3').className = 'HLight' ;
	}

	function removeHighlight()
	{
		document.getElementById('pref_weekday1').className = 'noHLight' ;
		document.getElementById('pref_weekday2').className = 'noHLight' ;
		document.getElementById('pref_weekday3').className = 'noHLight' ;
		document.getElementById('pref_day1').className = 'noHLight' ;
		document.getElementById('pref_day2').className = 'noHLight' ;
		document.getElementById('pref_month1').className = 'noHLight' ;
		//document.getElementById('pref_month2').className = 'noHLight' ;
		document.getElementById('pref_month3').className = 'noHLight' ;
	}

	<% if ((action = "EditEvent") Or (action="AddEvent")) Then %>
	function dateUpdatedEvent(dateElem)
	{
		// alert('dateUpdatedEvent') ;
		createradioondate() ;
	}
	<% End If %>

	function createradioondate()
	{

	    // if (!CheckDateOnly(inpDate)) { return }

	    // if(inpDate=='') { return }

	    // var tStr = inpDate ;
	    // var sSpl = tStr.split('-') ;
	    //var fDt = new Date(sSpl[2],convertMonthAbbrevToNumber(sSpl[1])-1,sSpl[0]) ;

	    var months = ecmMonths.split(',') ;
	    var weekdays = ecmDays.split(',') ;

	    //var weekday  = fDt.getDay();
	    //var day  = fDt.getDate();
	    //var month = fDt.getMonth();
	    //var yy = fDt.getYear();

	    var weekday = eval('document.getElementById("frm_start_date_only_dow").value') ;
	    var day = eval('document.getElementById("frm_start_date_only_dom").value') ;
	    var month = eval('document.getElementById("frm_start_date_only_monum").value') ;
	    var yy = eval('document.getElementById("frm_start_date_only_yrnum").value') ;

	    year = (yy < 1000) ? yy + 1900 : yy;

	    var weekCount = Math.ceil(day / 7) ;

	    document.getElementById('pref_weekday1').innerHTML = weekdays[weekday] ;
	    document.getElementById('pref_weekday2').innerHTML = weekdays[weekday] ;
	    document.getElementById('pref_weekday3').innerHTML = weekdays[weekday] ;
	    document.getElementById('pref_day1').innerHTML = addDayPost(day) ;
	    document.getElementById('pref_day2').innerHTML = addDayPost(day) ;
	    document.getElementById('pref_month1').innerHTML = months[month] ;
	    document.getElementById('pref_month3').innerHTML = months[month] ;
	    document.getElementById('pref_weekcount1').innerHTML = stringWCountPost(weekCount) ;
	    document.getElementById('pref_weekcount2').innerHTML = stringWCountPost(weekCount) ;

	    makeHighlight() ;

	    setInterval('removeHighlight()',1000) ;
    }

function flipTab(inpFlag,isEdit)
{
	var iefour = ((document.all) && (!document.getElementById)) ;
	var nnfour = ((document.layers) && (!document.getElementById)) ;

	if(iefour||nnfour)
	{
	    document.getElementById = new Function('var expr = /^\\w[\\w\\d]*$/, elname=arguments[0]; if(!expr.test(elname)) { return null; } else if(eval("document.all."+elname)) { return eval("document.all."+elname); } else return null;')
	}

	// flip the hidden field recursion flag on or off, so the submittal page
	// knows which set of data to use.
	// This may want to be relocated to form submittal time.
	if (inpFlag=='1')
	{
	    document.getElementById("frm_recursive").value = "0";
	}
	else
	{
	    document.getElementById("frm_recursive").value = "1";
	}

	if (inpFlag=='1')
	{
	    document.getElementById('tabOne').className = 'selectedTab' ;
	    document.getElementById('tabTwo').className = 'nonSelectedTab' ;
	    document.getElementById('OnceTab').className = 'DisplayTabContent' ;
	    document.getElementById('RecurTab').className = 'NoDisplayTabContent' ;
	}
	else
	{
	    if(!isEdit)
	    {
		    document.getElementById('tabOne').className = 'nonSelectedTab' ;
	    }
	    else
	    {
		    document.getElementById('tabOne').className = 'nonSelectedTabNoLink' ;
	    }
	    document.getElementById('tabTwo').className = 'selectedTab' ;
	    document.getElementById('OnceTab').className = 'NoDisplayTabContent' ;
	    document.getElementById('RecurTab').className = 'DisplayTabContent' ;
	}

	return true ;
}

function flipRecursBG(inID)
{
	var trName = "tr_" + inID ;
	var trOld = "tr_" + document.getElementById("currentSel").value ;
	var j ;
	var i ;

	var iefour = ((document.all) && (!document.getElementById)) ;
	var nnfour = ((document.layers) && (!document.getElementById)) ;

	if(iefour||nnfour)
	{
	    document.getElementById = new Function('var expr = /^\\w[\\w\\d]*$/, elname=arguments[0]; if(!expr.test(elname)) { return null; } else if(eval("document.all."+elname)) { return eval("document.all."+elname); } else return null;')
	}

	if(trName==trOld)
	{
	    return true ;
	}

	document.getElementById(trName).className = 'yesBG' ;
	document.getElementById(trOld).className = 'noBG' ;
	document.getElementById("currentSel").value = inID ;
	j = document.getElementById("RecurRadio").length;
	for (i=0; i<j; i++)
	{
		if((document.forms[0].RecurRadio[i].value == inID))
		{
			document.forms[0].RecurRadio[i].click() ;
		}
	}
}

function countDays()
{
	// Check that each form has a value entered
	if(("" == document.getElementById("frm_start_date_only").value)||("" == document.getElementById("frm_end_date_only").value)) { return } ;
	// Check that the date is an international date value
	// if (!CheckDateOnly(document.forms.calendar.frm_end_date_only.value)) { return }
	var tStr = document.forms.calendar.frm_start_date_only.value ;
	var sSpl = tStr.split('-') ;
	var sdate = new Date(sSpl[2],convertMonthAbbrevToNumber(sSpl[1]),sSpl[0]) ;
	tStr = document.forms.calendar.frm_end_date_only.value ;
	sSpl = tStr.split('-') ;
	var edate = new Date(sSpl[2],convertMonthAbbrevToNumber(sSpl[1]),sSpl[0]) ;

	var timeBtwn = 0 ;
	var selectedType ;

	// If the dates are the same, or end date is less than start date, exit
	if((sdate.toString()==edate.toString())||(edate<sdate)) {
		// document.getElementById('time_between').innerHTML = 1 ;
		return ;
	}

	var diff = sdate - edate ;

	// Always add one for the first event
	var modifier = 1

	// Always add another for the second event on the daily.
	// Add another to the weekly if the ending weekday is the same as the start weekday.

	if (document.forms.calendar.RecurRadio.length > 0) {
		for (var i = 0; i < document.forms.calendar.RecurRadio.length; i++) {
			if (document.forms.calendar.RecurRadio[i].checked) {
				selectedType = document.forms.calendar.RecurRadio[i].value ;
			}
		}
	}
	else
	{
		selectedType = document.forms.calendar.RecurRadio.value ;
	}

	if(selectedType == 'daily') {
		// Daily
		timeBtwn = Math.abs(Math.ceil(diff/(1000*60*60*24))) ;
		timeBtwn += modifier ;
	}
	if(selectedType == 'weekly') {
		// Weekly
		//
		// if (sdate.getDay() == edate.getDay()) { modifier++ } ;
		timeBtwn = Math.abs(Math.ceil(diff/(1000*60*60*24*7))) ;
		timeBtwn += modifier ;
	}
	if(selectedType == 'monthlybyday') {
		// Every # of every Month
		//
		// Modify for year.
		modifier += (edate.getFullYear() - sdate.getFullYear()) * 12 ;
		// Modify for dates less then start.
		if(edate.getDate() < sdate.getDate()) { modifier-- } ;

		timeBtwn = edate.getMonth() - sdate.getMonth() ;
		timeBtwn += modifier ;
	}
	if(selectedType == 'monthlybyweekday') {
	// Every 1st/2nd/3rd/4th/last weekday of the Month
		// Modify for year.
		modifier += (edate.getFullYear() - sdate.getFullYear()) * 12 ;
		// Modify for dates less then start.
		if(edate.getDate() < sdate.getDate()) { modifier-- } ;

		timeBtwn = edate.getMonth() - sdate.getMonth() ;
		timeBtwn += modifier ;

	}
	if(selectedType == 'yearlybymonthbyday') {
	// Yearly, every # of the month
		if ((edate.getFullYear() - sdate.getFullYear()) > 0) {
			modifier += edate.getFullYear() - sdate.getFullYear() ;
			if(edate.getMonth()==sdate.getMonth()) { if(edate.getDate() < sdate.getDate()) { modifier-- } }
			else { if (edate.getMonth()<sdate.getMonth()) { modifier-- } }
		}
		timeBtwn += modifier ;
	}
	if(selectedType == 'yearlybymonthbyweekday') {
	// Yearly, every first weekday of the month
		if ((edate.getFullYear() - sdate.getFullYear()) > 0) {
			modifier += edate.getFullYear() - sdate.getFullYear() ;
			if(edate.getMonth()==sdate.getMonth()) { if(edate.getDate() < sdate.getDate()) { modifier-- } }
			else { if (edate.getMonth()<sdate.getMonth()) { modifier-- } }
		}
		timeBtwn += modifier ;
	}
	//document.getElementById('time_between').innerHTML = timeBtwn ;
}
		//--><!]]>
	</script>
	<link rel="stylesheet" href="csslib/calendarStyles.css" />
		<!--#include file="common/stylesheetsetup.inc" -->
</asp:Panel>
		
</head>

<body>
<%

dim gtNavs ,gtObj as Object
gtObj = Nothing
cInfo = Nothing
EvtTitle = Nothing
gtNavs = Nothing
eLocation = Nothing
Qlink = Nothing
UseQlink = Nothing
LaunchBrowser = Nothing
fPath = Nothing

AppUI.ContentLanguage=ContentLanguage
siteRef.RequestInformationRef.PreviewMode=False
if ((lcase(action) = "showcalendar") or ((request.QueryString("ekM") <> "") And (request.QueryString("ekY") <> ""))) then
	'Dim newQString As String
	'Dim passOne As Boolean
	'Dim QStringName
	dim CalView as Object
	' Dim CalendarReqData as Ektron.CMS.BE.Modules.CalendarRequest
	Dim sdate, edate as Object

	'CalendarReqData = new Ektron.CMS.BE.Modules.CalendarRequest
	'if ((request.QueryString("ekM") <> "") And (request.QueryString("ekY") <> "")) then
    '    CalendarReqData.CalendarMonth = Request.QueryString("ekM")
    '    CalendarReqData.CalendarYear = Request.QueryString("ekY")
    'Else
    '    CalendarReqData.CalendarMonth = DatePart(DateInterval.Month, Now)
    '   CalendarReqData.CalendarYear = DatePart(DateInterval.Year, Now)
    'End If
	'CalendarReqData.TemplateName = 1'TODO:Verify UDAI/11/22/04
    'CalendarReqData.ImagesPath = AppImgPath
    'CalendarReqData.AbbrevDayNames = True
    'CalendarReqData.CalendarID = Request.QueryString("calendar_id")
    'CalendarReqData.IsWorkspace = True
    'CalendarReqData.QueryString = Request.ServerVariables("query_string")
    'CalendarReqData.PreviewMode = False
	'CalendarReqData.UrlFile = Request.ServerVariables("URL")
	'CalendarReqData.ShowEventType = True
	'If (Request.Form("frm_EventTypeID") <> "") Then
	'	CalendarReqData.EventTypeID = Request.Form("frm_EventTypeID")
	'End If
	' CalView = cCalendar.ecmCalendar(CalendarReqData)

	cCalendar = siteRef.EkModuleRef

	If (request.querystring("sdate") <> "") Then
		sdate = cdate(Request.QueryString("sdate"))
		If( lcase(Request.Querystring("display")) = "month" ) Then
			sdate = DateSerial(DatePart("yyyy",sdate), DatePart("m",sdate),1)
		End If
	Else
		sdate = DateSerial(DatePart("yyyy",Now()), DatePart("m",Now()), 1)
	End If

	If(Request.QueryString("edate") <> "") Then
		edate = cdate(Request.Querystring("edate"))
	Else
		edate = DateAdd("m", 1, sdate)
	End If

	Dim evH as Object
	If (Request.QueryString("evHighlight") <> "") Then
		evH = CLng(Request.QueryString("evHighlight"))
	Else
		evH = 0
	End If

	Dim strDisplay As String
	strDisplay = Request.Querystring("display")
	If (strDisplay <> "") Then
		If (strDisplay = "xml") Then
			CalView = Replace(Server.HTMLEncode(cCalendar.outputRawEventCalendarXML(Request.QueryString("calendar_id"), sdate, edate, evH, 0)),vbcrlf,"<br/>")
		Else
			Dim strDisplayMod As String
			strDisplayMod = ""
			If "month" = strDisplay Then
				strDisplayMod = Request.Querystring("displaymod")
			End If
			CalView = cCalendar.OutputRenderedCalendarHTML(Clng(Request.QueryString("calendar_id")), strDisplay & strDisplayMod, sdate, edate, evH, 0)
		End If
	Else
		CalView = cCalendar.OutputRenderedCalendarHTML(clng(Request.QueryString("calendar_id")), "montheditworkarea", sdate, edate, evH, 0)
	End If

	gtObj = siteRef.EkModuleRef
	gtNavs = gtObj.GetCalendarById(CLng(Request.QueryString("calendar_id")))

	Dim tQString, thing as Object
	tQString = Nothing
	' Rebuild the query string without the langtype because we are adding langtype below.
	For each thing in Request.QueryString
		If(lcase(thing)<>"langtype") Then
			tQString = tQString & "&" & thing & "=" & Request.QueryString(thing)
		End If
	Next
	%>
	<div class="ektronPageHeader">
	    <div class="ektronTitlebar">
	        <%= (GetTitleBar(mMsgs.GetMessage("show calendar") & " """ & gtNavs("CalendarTitle") & """"))%>
	    </div>
	    <div class="ektronToolbar">
		    <table>
			    <tr>
				     <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmscalendar.aspx?action=ViewCalendar&calendar_id=" & Request.QueryString("calendar_id") & "&folderid=" & Request.QueryString("folderid") , mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
				     <td><% If (siteRef.EnableMultilingual) Then %>&nbsp;<%=(mMsgs.GetMessage("view in label"))%>: <%= ShowActiveLangsInList(False, "FFFFFF", "document.location.href='cmscalendar.aspx?langtype=' + this.value + '" & tQString & "' ;", siteRef.ContentLanguage, gtNavs("AvailLangs")) %><% End If %></td>
				    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
			    </tr>
		    </table>
	    </div>
	</div>
	<div class="ektronPageContainer">
	    <table width="100%">
		    <tr>
			    <td><%=(CalView)%></td>
		    </tr>
	    </table>
	</div>
	<%
	cinfo = nothing
	cCalendar = nothing

elseif (action = "EditEvent" andalso not Page.IsPostBack()) then

	cCalendar = siteRef.EkModuleRef
	gtNav = cCalendar.GetEventByID(Request.QueryString("event_id"))
	if (len(ErrorString) = 0) then
		if (request.QueryString("folderid") = "") then
			folderid = gtNav("FolderID")
		end if
		gtObj = siteRef.EkContentRef
		cTmp = New Collection
		cTmp.Add (folderid, "FolderID")
		cTmp.Add (folderid, "ParentID")
		cTmp.Add ("", "OrderBy")
		cFolders = gtObj.GetAllViewableChildFoldersv2_0(cTmp)
	end if
	if (len(ErrorString) = 0) then
		gtNavs = gtObj.GetFolderInfoWithPath(folderid)
		FolderName = gtNavs("FolderName")
		ParentFolderId = gtNavs("ParentID")
		fPath = gtNavs("Path")
		gtNavs = nothing
		cConts = gtObj.GetAllViewableChildContentInfov2_0(cTmp)
	end if
	if (len(ErrorString) = 0) then
		EvtTitle = gtNav("EventTitle")
		eLocation = gtNav("EventLocation")
		calendar_id = gtNav("CalendarID")
		Qlink = gtNav("QLink")
		UseQlink = gtNav("UseQLink")
		LaunchBrowser = gtNav("LaunchNewBrowser")
		ShowStime = gtNav("ShowStartTime")
		ShowEtime = gtNav("ShowEndTime")
		iMonth = request.QueryString("iMonth")
		iYear = request.QueryString("iYear")
		iDay = request.QueryString("iDay")
		gtnStart = gtNav("StartTime")
		gtnEnd = gtNav("EndTime")

		cInfo = cCalendar.GetCalendarById((gtNav("CalendarID")))
	end if
	if (ErrorString <> "") then
	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%= (GetTitleBar(mMsgs.GetMessage("edit cal event"))) %>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<%
	else
	%>
	    <form id="calendar" runat="server">
	        <input type="hidden" name="frm_calendar_id" value="<%=gtNav("CalendarID")%>"/>
	        <input type="hidden" name="frm_event_id" value="<%=gtNav("EventID")%>"/>
	        <input type="hidden" name="frm_year" value="<%=gtNav("Year")%>"/>
	        <input type="hidden" name="frm_month" value="<%=gtNav("Month")%>"/>
	        <input type="hidden" name="frm_day" value="<%=gtNav("Day")%>"/>
	        <input type="hidden" name="frm_content_id" id="frm_content_id" value="<%=gtNav("ContentID")%>"/>
	        <input type="hidden" name="frm_content_langid" id="frm_content_langid" value="<%=gtNav("ContentLanguage")%>"/>
	        <input type="hidden" name="frm_callback" value="cmscalendar.aspx?action=ViewEvents&calendar_id=<%=gtNav("CalendarID")%>&iYear=<%=gtNav("Year")%>&iMonth=<%=gtNav("Month")%>&iDay=<%=gtNav("Day")%>"/>
	        <input type="hidden" name="frm_ev_type_avail" value="<%=CBool(gtNav("AvailEvType"))%>"/>
	        <input type="hidden" name="frm_ev_type_req" value="<%=CBool(gtNav("ReqEvType"))%>"/>
	        <input type="hidden" name="frm_event_start"/>
	        <input type="hidden" name="frm_event_end"/>
	        <input type="hidden" name="frm_associated_recursIDs" value="<%= gtNav("AssociatedRecursIDs") %>"/>
	        <script type="text/javascript"> var AutoNav = '<%= Foldername %>' ; </script>

	        <div class="ektronPageHeader">
        	    <div class="ektronTitlebar">
	                <%= (GetTitleBar(mMsgs.GetMessage("edit cal event"))) %>
	            </div>
	            <div class="ektronToolbar">
		            <table>
			            <tr>
				            <%
				            If 2 = cInfo("LongDescriptionAvail") Then
					            Response.Write(GetButtonEventsWCaption(imgIconPath & "save.png", "#", mMsgs.GetMessage("alt save button text (event)"), mMsgs.GetMessage("btn save"), "Onclick=""javascript:return SubmitFormWeWEP('calendar', 'VerifyAddEventForm()');"""))
				            Else
					            Response.write(GetButtonEventsWCaption(imgIconPath & "save.png", "#", mMsgs.GetMessage("alt save button text (event)"), mMsgs.GetMessage("btn save"), "Onclick=""javascript:return SubmitForm('calendar', 'VerifyAddEventForm()');"""))
			                End If %>
				            <%=(GetButtonEventsWCaption(imgIconPath & "delete.png", "calendaraction.aspx?action=doEventDelete&event_id=" & gtNav("EventID") & "&deleteAssoc=false&calendar_id=" &  gtNav("CalendarID") & "&iMonth=" & gtNav("Month") & "&iDay=" & gtNav("Day") & "&iYear=" & gtNav("Year") , mMsgs.GetMessage("del cal evt"), mMsgs.GetMessage("btn delete"), "OnClick=""javascript: return ConfirmCalendarDelete();"""))%>
				            <% If gtNav("RecursionID") > 0 Then %>
				            <%=(GetButtonEventsWCaption(AppImgPath & "btn_deletedatetime-nm.gif", "calendaraction.aspx?action=doEventDelete&event_id=" & gtNav("EventID") & "&deleteAssoc=true&calendar_id=" &  gtNav("CalendarID") & "&iMonth=" & gtNav("Month") & "&iDay=" & gtNav("Day") & "&iYear=" & gtNav("Year") , mMsgs.GetMessage("del cal recur evt"), mMsgs.GetMessage("btn delete recur"), "OnClick=""javascript: return ConfirmCalendarDeleteRecur();"""))%>
				            <% End If %>
				            <%=(GetButtonEventsWCaption(imgIconPath & "bookAdd.png", "#", mMsgs.GetMessage("alt select qlink button text"), mMsgs.GetMessage("btn add library"), "Onclick=""javascript: QuickLinkSelectv48(" & parentfolderid & ",'calendar','frm_event_title',1,1,0,1) ;return false;"""))%>
				            <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmscalendar.aspx?action=ViewEvents&calendar_id=" & gtNav("CalendarID") & "&iYear=" & gtNav("Year") & "&iMonth=" & gtNav("Month") & "&iDay=" & gtNav("Day"), mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
				            <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
			            </tr>
		            </table>
	            </div>
	        </div>
            <div class="ektronPageContainer ektronPageInfo">
                <table class="ektronGrid" width="100%">
                    <tr>
                        <td class="label" style="text-align: left !important">
                            <%=mMsgs.GetMessage("event title")%>
                            :
                            <input type="Text" name="frm_event_title" id="frm_event_title" size="60" maxlength="255"
                                value="<%=EvtTitle %>" /></td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left !important">
                            <%= mMsgs.GetMessage("event location") %>
                            :
                            <input type="Text" name="frm_event_location" size="60" maxlength="255" value="<%=eLocation %>" /></td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <%
					    Dim d_sMonth, d_sDay, d_sYear, d_sTime as object
					    Dim d_eMonth, d_eDay, d_eYear, d_eTime as object

					    d_sMonth = DatePart("m",gtnStart)
					    d_eMonth = DatePart("m",gtnEnd)

					    d_sDay = DatePart("d",gtnStart)
					    d_eDay = DatePart("d",gtnEnd)

					    d_sYear = DatePart("yyyy",gtnStart)
					    d_eYear = DatePart("yyyy",gtnEnd)

					    d_sTime = FormatDateTime(CDate(gtnStart),3)
					    d_eTime = FormatDateTime(CDate(gtnEnd),3)

					    'Response.Write("d_sMonth = " & d_sMonth & "<br/>")
					    'Response.Write("d_eMonth = " & d_eMonth & "<br/>")
					    'Response.Write("d_sDay = " & d_sDay  & "<br/>")
					    'Response.Write("d_eDay = " & d_eDay  & "<br/>")
					    'Response.Write("d_sYear = " & d_sYear  & "<br/>")
					    'Response.Write("d_eYear = " & d_eYear  & "<br/>")
					    'Response.Write("d_sTime = " & d_sTime  & "<br/>")
					    'Response.Write("d_eTime = " & d_eTime & "<br/>")
					    'Response.Write("ShowStime = " & ShowStime & "<br/>")
					    'Response.Write("ShowEtime = " & ShowEtime & "<br/>")
					    'Response.Write("gtnStart = " & gtnStart & "<br/>")
					    'Response.Write("gtnEnd = " & gtnEnd & "<br/>")

					    'The following is for only foreign culture....we need to find better implementation
					    'this is required because our backend functions are not strength
					    Dim objDT as DateTime=Convert.ToDateTime(gtnStart)

					    If(convert.toint32(objDT.Hour)<12 )Then
						    If(NOT((Instr(d_sTime,"AM")>0)OR(Instr(d_sTime,"PM")>0))) Then
							    d_sTime=d_sTime & " AM"
						    End If
					    Else
						    If(NOT((Instr(d_sTime,"AM")>0)OR(Instr(d_sTime,"PM")>0))) Then
							    If(objDT.Hour-12=0) Then
								    d_sTime=objDT.Hour & ":" & FormatNumber(objDT.Minute()) & ":" & FormatNumber(objDT.Second)  & " PM"
							    Else
								    d_sTime=objDT.Hour-12 & ":" & FormatNumber(objDT.Minute()) & ":" & FormatNumber(objDT.Second)  & " PM"
							    End If
						    End If
					    End If
					    objDT=Convert.ToDateTime(gtnEnd)
					    If(convert.toint32(objDT.Hour)<12 )Then
						    If(NOT((Instr(d_eTime,"AM")>0)OR(Instr(d_eTime,"PM")>0))) Then
							    d_eTime=d_eTime & " AM"
						    End If
					    Else
						    If(NOT((Instr(d_eTime,"AM")>0)OR(Instr(d_eTime,"PM")>0))) Then
							    'd_eTime=d_eTime & " PM"
							    If(objDT.Hour-12=0) Then
								    d_eTime=objDT.Hour & ":" & FormatNumber(objDT.Minute()) & ":" & FormatNumber(objDT.Second)  & " PM"
							    Else
								    d_eTime=objDT.Hour-12 & ":" & FormatNumber(objDT.Minute()) & ":" & FormatNumber(objDT.Second)  & " PM"
							    End If
						    End If
					    End If
					    DisplayRecursionTabs (d_sMonth, d_sDay, d_sYear, d_sTime, _
						    d_eMonth, d_eDay, d_eYear, d_eTime, gtNav("RecursionType"), _
						    gtNav("RecursionID"), True, gtNav("ActualTime"), cInfo("StartLabel"), cInfo("EndLabel"))

                            %>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left !important">
                            <%= mMsgs.GetMessage("generic hyperlink label") %>
                            :
                            <input type="Text" name="frm_qlink" id="frm_qlink" value="<%=Qlink %>" size="60"
                                maxlength="255" />
                        </td>
                    </tr>
                    <!-- <tr>
				    <td class="label" colspan="2">
					    &nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" name="frm_use_qlink"
											    <% if (UseQlink) then
												    response.write("checked=""true""")
											    end if%>  OnClick="javascript:SetBrowserState();"><%= mMsgs.GetMessage("make hyperlink active") %>
				    </td>
			    </tr> -->
                    <tr>
                        <td class="label" style="text-align: left !important">
                            <input type="checkbox" name="frm_launch_new_browser" <% if (LaunchBrowser) then
																		    response.write("checked=""true""")
																	    end if%> />
                            <%= mMsgs.GetMessage("launch in new browser") %>
                        </td>
                    </tr>
                    <!--<tr>
				    <td class="label"><%'=("Root Folder")%></td>
				    <td><%'=FolderName%></td>
			    </tr>-->
                    <% If (gtNav("AvailEvType")) Then %>
                    <tr>
                        <td class="label" colspan="2">
                            <%= cInfo("EventTypeLabel") %>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" colspan="2">
                            <table border="0">
                                <tr>
                                    <td rowspan="2">
                                        <%= mMsgs.GetMessage("generic available") %>
                                        :<br />
                                        <select multiple name="availEvTypes" size="5">
                                            <%=(AvailableEventTypes(calendar_id, ErrorString, gtNav("EventTypes")))%>
                                        </select>
                                    </td>
                                    <td valign="top">
                                        &nbsp;<br />
                                        &nbsp;<input type="button" name="n1" value=">" onclick="copySelected(this.form.availEvTypes,this.form.selEvTypes)" />&nbsp;</td>
                                    <td rowspan="2">
                                        <%= mMsgs.GetMessage("generic selected") %>
                                        :<br />
                                        <select multiple name="selEvTypes" size="5">
                                            <%=(SelectedEventTypes(gtNav("EventTypes")))%>
                                        </select>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="bottom">
                                        &nbsp;<input type="button" name="n2" value="<" onclick="copySelected(this.form.selEvTypes, this.form.availEvTypes) " />&nbsp;</td>
                                </tr>
                            </table>
                        </td>
                    </tr>                    
                            <% End If %>
                    <tr>
                        <td class="leftLabel" colspan="2">
                            <%If (cInfo("LongDescriptionAvail") = 1) Then %>
                            <%= mMsgs.GetMessage("generic long description") %>
                            :<br />
                            <textarea cols="60" rows="10" name="frm_eventLongDesc" id="frm_eventLongDesc"><%= gtNav("LongDescription") %></textarea>
                            <% ElseIf (cInfo("LongDescriptionAvail") = 2) Then %>
                            <%= mMsgs.GetMessage("generic long description") %>
                            :<br />
                            <input type="hidden" name="frm_eventLongDesc" id="frm_eventLongDesc" value="" />
                            <asp:PlaceHolder ID="EditEventEditorHolder" runat="server" />
                            <!-- eWebEditProEditor("LongDescription", "100%", 300, gtNav("LongDescription"), AppeWebPath & "cms_config.aspx?InterfaceName=calendar") -->
                            <% End If %>
                        </td>
                    </tr>
                </table>
            </div>
	    </form>
	<%end if %>

<%elseif (action = "AddEvent") then
	calendar_id = request.QueryString("calendar_id")
	iMonth = request.QueryString("iMonth")
	iYear = request.QueryString("iYear")
	iDay = request.QueryString("iDay")
	sTime = request.QueryString("start_time")
	eTime = request.QueryString("end_time")
	ShowStime = request.QueryString("show_start")
	ShowEtime = request.QueryString("show_end")
	eLocation = request.QueryString("event_location")
	astrMonthNames = split(siteRef.getenglishMonthsAbbrev, ",")
	if (len(sTime)> 0) then
		eventDate = sTime
	else
		eventDate = iDay & "-" & astrMonthNames(iMonth) & "-" & iYear
		eTime = eventDate & " 11:59:00 pm"
	end if
	if (request.QueryString("show_start") <> "") then
		ShowStime = request.QueryString("show_start")
	else
		ShowStime = true
	end if
	if (request.QueryString("show_end") <> "") then
		ShowEtime = true
	else
		ShowEtime = false
	end if
	if (len(eTime) = 0) then
		eTime = eventDate
	end if
	if (calendar_id = 0) then
	    Response.redirect("cmscalendar.aspx?action=AddCalendar", false)
	else
	    cCalendar = siteRef.EkModuleRef
	    cInfo = cCalendar.GetCalendarById(calendar_id)
	    if (len(ErrorString) = 0) then
		    gtObj = siteRef.EkContentRef
		    cTmp = New Collection
		    cTmp.Add ("name","OrderBy")
		    if Request.QueryString("folderid") <> "" then
			    cTmp.Add (Request.QueryString("folderid"),"FolderID")
			    cTmp.Add (Request.QueryString("folderid"),"ParentID")
			    folderId = Request.QueryString("folderid")
		    else
			    cTmp.Add (cInfo("FolderID"),"FolderID")
			    cTmp.Add (cInfo("FolderID"),"ParentID")
			    folderId = cInfo("FolderID")
		    end if

		    cFolders = gtObj.GetAllViewableChildFoldersv2_0(  cTmp)
	    end if
	    if (len(ErrorString) = 0) then
		    gtNavs = gtObj.GetFolderInfoWithPath( folderid)
		    FolderName = gtNavs("FolderName")
		    ParentFolderId = gtNavs("ParentID")
		    fPath = gtNavs("Path")
		    gtNavs = nothing
		    SiteObj = siteRef.EkSiteRef
		    ' cPerms = SiteObj.GetPermissionsv2_0(folderId, "folder")
		    cPerms = contObjForPerms.LoadPermissions(0,"folder")
	    end if

	    if (len(ErrorString) = 0) then
		    if( len(ErrorString)) then
			    CanCreateContent = false
		    else
			    CanCreateContent = cPerms.CanAdd
		    end if
		    SiteObj = nothing
		    ContObj = siteRef.EkContentRef
		    cConts = ContObj.GetAllViewableChildContentInfov2_0(cTmp)
	    end if
	    if (ErrorString <> "") then
	    %>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(mMsgs.GetMessage("add cal event")))%>
		    </div>
			<div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	    <%
	    else
	    %>
		    <form id="addeventcalendar" runat="server">
		        <input type="hidden" name="frm_calendar_id" value="<%=(calendar_id)%>"/>
		        <input type="hidden" name="frm_year" value="<%=iYear%>"/>
		        <input type="hidden" name="frm_month" value="<%=iMonth%>"/>
		        <input type="hidden" name="frm_day" value="<%=iDay%>"/>
		        <input type="hidden" name="frm_content_id" value="0"/>
		        <input type="hidden" name="frm_content_langid" value="<%= siteRef.ContentLanguage %>"/>
		        <input type="hidden" name="frm_ev_type_avail" value="<%=CBool(cInfo("AvailEvType"))%>"/>
		        <input type="hidden" name="frm_ev_type_req" value="<%=CBool(cInfo("ReqEvType"))%>"/>
		        <input type="hidden" name="frm_event_start"/>
		        <input type="hidden" name="frm_event_end"/>
		        <script type="text/javascript"> var AutoNav = '<%= replace(fPath,"\","\\") %>' ; </script>
		        <% if Request.QueryString("callback") = "ViewEvents" then %>
			        <input type="hidden" name="frm_callback" value="cmscalendar.aspx?action=ViewEvents&calendar_id=<%=calendar_id%>&iYear=<%=iYear%>&iMonth=<%=iMonth%>&iDay=<%=iDay%>"/>
		        <% end if %>

	            <div class="ektronPageHeader">
    		        <div class="ektronTitlebar">
		                <%= (GetTitleBar(mMsgs.GetMessage("add cal event"))) %>
		            </div>
			        <div class="ektronToolbar">
			            <table>
				            <tr>
					            <% If (cInfo("LongDescriptionAvail") = 2) Then
						            Response.Write(GetButtonEventsWCaption(imgIconPath & "save.png", "#", mMsgs.GetMessage("alt save button text (event)") , mMsgs.GetMessage("btn save"), "Onclick=""javascript:return SubmitFormWeWEP('calendar', 'VerifyAddEventForm()');"""))
					            Else
						            Response.write(GetButtonEventsWCaption(imgIconPath & "save.png", "#", mMsgs.GetMessage("alt save button text (event)") , mMsgs.GetMessage("btn save"), "Onclick=""javascript:return SubmitForm('calendar', 'VerifyAddEventForm()');"""))
					            End If %>
					            <%=(GetButtonEventsWCaption(imgIconPath & "bookAdd.png", "#",  mMsgs.GetMessage("alt select qlink button text"), mMsgs.GetMessage("btn add library"), "Onclick=""javascript: QuickLinkSelectv48(" & parentfolderid & ",'calendar','frm_event_title',1,1,0,1) ;return false;"""))%>
					            <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmscalendar.aspx?action=ShowCalendar&calendar_id=" & calendar_id, mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					            <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				            </tr>
			            </table>
		            </div>
	            </div>

                <div class="ektronPageContainer ektronPageInfo">
			        <table class="ektronGrid" width="100%">
				        <tr>
					        <td class="label ektronLeft"><%= mMsgs.GetMessage("event title") %>: 
					            <input type="Text" name="frm_event_title" size="60" maxlength="255" value="" /></td>
				        </tr>
				        <tr>
					        <td class="label ektronLeft"><%= mMsgs.GetMessage("event location") %>:
					            <input type="Text" name="frm_event_location" size="60" maxlength="255" value="<%=eLocation%>" /></td>
				        </tr>
				        <tr>
					        <td colspan="3">
					        <%
					        Dim actDate as Object = DateSerial(iYear, iMonth, iDay)

					         DisplayRecursionTabs (iMonth, iDay, iYear, "12:00 AM", _
										        "", "", "", "12:00 AM", "daily", 0, True, actDate, _
										        cInfo("StartLabel"), cInfo("EndLabel")) %>
					        </td>
				        </tr>
				        <tr>
					        <td class="label ektronLeft"><%= mMsgs.GetMessage("generic hyperlink label") %>: 
					            <input type="Text" name="frm_qlink" value="" size="60" maxlength="255" />
					        </td>
				        </tr>
				        <!-- <tr>
					        <td class="label" colspan="2">
					        <input type="checkbox" name="frm_use_qlink" OnClick="javascript:SetBrowserState();"> Make the Hyperlink active
					        </td>
				        </tr> -->
				        <tr>
					        <td class="label ektronLeft" style="text-align:left !important" colspan="2">
					        <input type="checkbox" name="frm_launch_new_browser"/> <%= mMsgs.GetMessage("launch in new browser") %>
					        </td>
				        </tr>
				        <% If (cInfo("AvailEvType")) Then %>
				        <tr>
					        <td class="label" colspan="2">
						        <%= cInfo("EventTypeLabel") %><br />
					        </td>
				        </tr>
				        <tr>
					        <td class="label" colspan="2">
						        <table border="0">
							        <tr>
							        <td rowspan="2"><%= mMsgs.GetMessage("generic available") %>:<br/>
								        <select multiple name="availEvTypes" size="5">
								        <%=(AvailableEventTypes(calendar_id, ErrorString, Nothing))%>
								        </select>
							        </td>
							        <td valign="top">&nbsp;<br/>&nbsp;<input type="button" name="n1" value=">" onclick="copySelected(this.form.availEvTypes,this.form.selEvTypes)"/>&nbsp;</td>
							        <td rowspan="2"><%= mMsgs.GetMessage("generic selected") %>:<br/>
								        <select multiple name="selEvTypes" size="5">
								        <%=(SelectedEventTypes(Nothing) )%>
								        </select>
							        </td>
							        </tr>
							        <tr>
								        <td valign="bottom">&nbsp;<input type="button" name="n2" value="<" onclick="copySelected(this.form.selEvTypes, this.form.availEvTypes) "/>&nbsp;</td>
							        </tr>
						        </table>
					        </td>
				        </tr>
				        <% End If

				        If (cInfo("LongDescriptionAvail") = 1) Then %>
				        <tr>
					        <td class="leftLabel" colspan="2"><%= mMsgs.GetMessage("generic long description") %>:<br/>
						        <textarea cols="60" rows="10" name="frm_eventLongDesc" id="frm_eventLongDesc"></textarea>
					        </td>
				        </tr>
				        <% ElseIf (cInfo("LongDescriptionAvail") = 2) Then %>
				        <tr>
					        <td class="leftLabel" colspan="2"><%= mMsgs.GetMessage("generic long description") %>:<br/>
					        <input type="hidden" name="frm_eventLongDesc" id="frm_eventLongDesc" value=""/>
					        <asp:PlaceHolder ID="AddEventEditorHolder" runat="server" />
					        <!-- eWebEditProEditor("LongDescription", "100%", 300, "", AppeWebPath & "cms_config.aspx?InterfaceName=calendar") -->
					        </td>
				        </tr>
				        <% End If %>
			        </table>
			    </div>
		    </form>
		    <%
	    end if
	    gtObj = nothing
	    cTmp = nothing
	    cCalendar = nothing
    end if
elseif (action = "ViewEvents") then
	cTmp = New Collection
	'gtObj = siteRef.EkContentRef
	cTmp.Add (Request.QueryString("calendar_id"),"CalendarID")
	cTmp.Add (Request.QueryString("iMonth"),"Month")
	cTmp.Add (Request.QueryString("iYear"),"Year")
	cTmp.Add (Request.QueryString("iDay"),"Day")
	'eventDate = iMonth & "/" & iDay & "/" & iYear
	cCalendar = siteRef.EkModuleRef
	cInfo = cCalendar.GetAllEventsForTheDay(cTmp)

	Dim tDt as Date
	tDt = DateSerial(Cint(Request.QueryString("iYear")), Cint(Request.QueryString("iMonth")), Cint(Request.QueryString("iday")) )
	dso.extendedmeta = false
	dso.targetdate = tDt

	if (ErrorString <> "") then	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%= GetTitleBar(mMsgs.GetMessage("view events for") & " for " & dso.GetDisplayDate(False) )  %>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<%
	else
	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar noDateClass">
		        <%= GetTitleBar(mMsgs.GetMessage("view events for") & dso.displayCultureDate(False) )  %>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(AppImgPath & "btn_addevent-nm.gif", "cmscalendar.aspx?action=AddEvent&calendar_id=" & Request.QueryString("calendar_id") & "&iYear="& Request.QueryString("iYear") & "&iMonth=" & Request.QueryString("iMonth")& "&iDay=" & Request.QueryString("iDay") & "&callback=ViewEvents" ,  mMsgs.GetMessage("alt add button text (event)") , mMsgs.GetMessage("btn add event"), ""))%>
					    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmscalendar.aspx?action=ShowCalendar&calendar_id=" & Request.QueryString("calendar_id"), mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>
        <div class="ektronPageContainer">
		    <table width="100%" class="ektronGrid" id="minWidthDate">
			    <tr class="title-header">
				    <td width="30%" colspan="2"><%=(mMsgs.GetMessage("generic Title"))%></td>
				    <td><%= mMsgs.GetMessage("generic author") %></td>
				    <td><%= mMsgs.GetMessage("generic start time") %></td>
				    <td><%= mMsgs.GetMessage("generic end time") %></td>
			    </tr>
			    <%
			    if (cInfo.Count) then
				    For Each gtNav IN cInfo %>
				    <tr>
					    <td valign="middle"><a href="cmscalendar.aspx?action=EditEvent&event_id=<%=(gtNav("EventID"))%>&event_title=" alt='<%=(mMsgs.GetMessage("generic View") & " """ & replace(gtNav("EventTitle"), "'", "`") & """")%>' title='<%=(mMsgs.GetMessage("generic View") & " """ & replace(gtNav("EventTitle"), "'", "`") & """")%>'><%=(gtNav("EventTitle"))%></a></td>
					    <td align="right" valign="middle">&nbsp;
						    <% If (gtNav("RecursionID")>0) Then %>
						    <img src="<%= siteRef.AppImgPath %>btn_RecurEvent.gif" width="22" height="22" alt="Recurring Event"/>
						    <br clear="all"/>
						    <% End If %>
					    </td>
					    <td valign="middle"><%=(gtNav("AuthorsFirstName") & " " & gtNav("AuthorsLastName"))%></td>
					    <%
						    dSo.spanId = "DispStartTime"
						    dSo.targetDate = CDate(gtNav("StartTime"))
					    %>
					    <td valign="middle"><%= dSo.displayCultureTime(false) %></td>
					    <%
						    dSo.spanId = "DispStartTime"
						    dSo.targetDate = CDate(gtNav("EndTime"))
					    %>
					    <td valign="middle"><%= dSo.displayCultureTime(false) %></td>
				    </tr>
				    <%
				    Next
			    end if
				    %>
		    </table>
		</div>
	<%end if%>
<%elseif (action = "AddCalendar") OR (action = "EditCalendar") then
	Dim evTypeAvail, evTypeReq, longDescA, EvTypeInstruct, EvTypeShowall as Object
	EvTypeInstruct = Nothing
    EvTypeShowall = Nothing
    longDescA = Nothing
	title1 = request.QueryString("title")
	descrp = request.QueryString("description")
	calendar_id = request.QueryString("calendar_id")
	ShowWnds = request.QueryString("show_weekend")
	evTypeAvail = Request.QueryString("ev_type_avail")
	evTypeReq = Request.QueryString("ev_type_req")
	Utilities.ValidateUserLogin()
	if (request.QueryString("table_height") <> "") then
		table_height = request.QueryString("table_height")
		table_width = request.QueryString("table_width")
		location_label = request.QueryString("location_label")
		start_label = request.QueryString("start_label")
		end_label = request.QueryString("end_label")
		forward_only = request.QueryString("forward_only")
		evTypeLabel = Request.QueryString("evtype_label")
		If(Request.QueryString("longDesc") <> "") Then
			longDescA = Request.QueryString("longDesc")
		Else
			longDescA = "0"
		End If
		EvTypeInstruct = Request.QueryString("evtinst")
		EvTypeShowall = Request.QueryString("evtshowall")

		' *** For Folder Navigation. Fix this.
		' evTypeLabel = "Event Type: "
		If lcase(Request.QueryString("ev_type_avail")) = "1" Then
			evTypeAvail = True
		else
			evTypeAvail = False
		End If

		If lcase(Request.QueryString("ev_type_req")) = "1" Then
			evTypeReq = True
		else
			evTypeReq = False
		End If
	else
		table_height = 90
		table_width = 150
		location_label = mMsgs.GetMessage("generic location")
		start_label = mMsgs.GetMessage("lbl start")
		end_label = mMsgs.GetMessage("lbl end")
		evTypeLabel = mMsgs.GetMessage("ev types")
		forward_only = 0
	end if

	gtObj = siteRef.EkContentRef
	cTmp = New Collection
	cTmp.Add (Clng(folderid),"ParentID")

	cTmp.Add ("name","OrderBy")
	cFolders = gtObj.GetAllViewableChildFoldersv2_0(  cTmp)
	if (len(ErrorString) = 0) then
		gtNavs = gtObj.GetFolderInfoWithPath(  folderid)
		FolderName = gtNavs("FolderName")
		ParentFolderId = gtNavs("ParentID")
	end if
	if (ErrorString = "") then
		if (Not(IsUserAdmin(currentUserID,Site) OrElse AppUI.IsARoleMember(Common.EkEnumeration.CmsRoleIds.AdminCalendar))) then
			ErrorString = mMsgs.GetMessage("com: user does not have permission")
		end if
	end if
	if (ErrorString <> "") then
	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%= (GetTitleBar(mMsgs.GetMessage("addedit calendar"))) %>
		    </div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<%
	else
	%>
		<%if (action = "EditCalendar") then %>
		    <form action="calendaraction.aspx?Action=doUpdate" method="Post" id="calendar">
		<%else%>
		    <form action="calendaraction.aspx?Action=doAdd" method="Post" id="calendar">
		<%End if%>
		<script type="text/javascript">
	       <!--//--><![CDATA[//><!--
	            var deffolderid;
	            var languageID = <%=ContentLanguage %>;

		        function IsBrowserIE_Email() {
		        // document.all is an IE only property
		        return (document.all ? true : false);
	            }

		        function LoadChildPage() {
			        //debugger
			        deffolderid = document.getElementById("frm_folder_id").value;
			        if (IsBrowserIE_Email())
			        {
				        //debugger;
				        var frameObj = document.getElementById("ChildPage");
				        frameObj.src = "blankredirect.aspx?SelectCreateContent.aspx?FolderID=" + deffolderid + "&LangType=" + languageID + "&browser=0&overrideType=calfolder";

				        var pageObj = document.getElementById("FrameContainer");
				        pageObj.style.display = "";
				        pageObj.style.width = "80%";
				        pageObj.style.height = "80%";
			        }
			        else
			        {
				        // Using Netscape; cant use transparencies & eWebEditPro preperly
				        // - so launch in a seperate pop-up window:
				        PopUpWindow("SelectCreateContent.aspx?FolderID=" + deffolderid + "&LangType=" + languageID +"&browser=1&overrideType=calfolder","SelectFolder", 490,500,1,1);
			        }

		        }

		        function ReturnChildValue(contentid,contenttitle,qlink,folderid,contentlanguage) {
			        // take value, store it, write to display
			        //debugger;
			        CloseChildPage();
			        document.getElementById("contentidspan").innerHTML = "<div id=\"div3\" style=\"display: none;position: block;\"></div><div id=\"contentidspan\" style=\"display: block;position: block;\">(" + contentid + ")&nbsp;" + contenttitle + "&nbsp;&nbsp;</div>";
			        document.getElementById("a_change").style.visibility ="visible";
			        document.getElementById("frm_folder_id").value = contentid;
		        }

		        function UnSelectContent()
		        {
			        document.getElementById("contentidspan").innerHTML = "<div id=\"div3\" style=\"display: none;position: block;\"></div><div id=\"contentidspan\" style=\"display: block;position: block;\">" + "<a href=\"#\" onclick=\"javascript:LoadChildPage();return true;\">" & msghelper.getmessage("generic select") & "</a></div>";
			        document.getElementById("a_change").style.visibility ="hidden";
			        document.getElementById("frm_folder_id").value = 0;
		        }

		        function CloseChildPage()
		        {
			        if (IsBrowserIE_Email())
			        {
				        //debugger
				        var pageObj = document.getElementById("FrameContainer");
				        pageObj.style.display = "none";
				        pageObj.style.width = "1px";
				        pageObj.style.height = "1px";
			        }

		        }

		        function IsChildWaiting() {
			        var pageObj = document.getElementById("FrameContainer");
			        if (pageObj == null) {
				        return (false);
			        }
			        if (pageObj.style.display == "") {
				        return (true);
			        }
			        else {
				        return (false);
			        }
		        }
	        //--><!]]>
        </script>
		<input type="hidden" name="frm_folder_id" id="frm_folder_id" value="<%=(folderid)%>"/>
		<input type="hidden" name="frm_calendar_id" value="<%=(calendar_id)%>"/>
		<div id="FrameContainer" style="POSITION: absolute; TOP: 10px; LEFT: 20px; WIDTH: 1px; HEIGHT: 1px; DISPLAY: none;">
			<iframe id="ChildPage" frameborder="yes" marginheight="2" marginwidth="2" width="100%" height="100%" scrolling="auto">
			</iframe>
		</div>

	    <div class="ektronPageHeader">
		    <div class="ektronTitlebar">
		        <%= (GetTitleBar(mMsgs.GetMessage("addedit calendar"))) %>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					     <%=(GetButtonEventsWCaption(imgIconPath & "save.png", "#", mMsgs.GetMessage("alt Click here to save this calendar"), mMsgs.GetMessage("btn save"), "Onclick=""javascript:return SubmitForm('calendar', 'VerifyAddForm()');"""))%>
					     <%if(action = "EditCalendar") then %>
					 	    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmscalendar.aspx?action=ViewCalendar&calendar_id=" & calendar_id & "&folderid=" & folderid, mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					     <%else %>
					 	    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmscalendar.aspx?action=ViewAllCalendars", mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <%end if %>
				        <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>

		<div class="ektronPageContainer ektronPageInfo">
		    <table class="ektronGrid" width="100%">
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic title label"))%></td>
				    <td><input type="Text" name="frm_calendar_title" id="frm_calendar_title" size="60" maxlength="75" value="<%=title1%>" onkeypress="javascript:return CheckKeyValue(event,'34');"/></td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic description"))%>:</td>
				    <td><input type="Text" name="frm_calendar_description" id="frm_calendar_description" value="<%=descrp%>" size="60" maxlength="255" value=""/></td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic location"))%>:</td>
				    <td><input type="Text" name="frm_location_label" value="<%=location_label%>" size="25" maxlength="25" /></td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic start time"))%>:</td>
				    <td><input type="Text" name="frm_start_label" value="<%=start_label%>" size="25" maxlength="25" /></td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic end time"))%>:</td>
				    <td><input type="Text" name="frm_end_label" value="<%=end_label%>" size="25" maxlength="25" /></td>
			    </tr>
			</table>
        
            <div class="ektronTopSpace"></div>
            <fieldset>
                <legend><%=(mMsgs.GetMessage("lbl table properties"))%></legend>
			    <table class="ektronGrid" width="100%">
			        <tr>
			            <td class="label"><%= mMsgs.GetMessage("display weekends") %>:</td>
			            <td class="value"><input type="checkbox" name="frm_show_weekend" <%if (ShowWnds) then
						        response.write("checked=""true""")
						        end if%>/>
			            </td>
			        </tr>
			        <!--
				       Table Height and Width are obsolete, default to 90x150. Comment hidden inputs below and uncomment below to resurrect.
			        <tr>
				        <td colspan="2">
				            <table>
					            <tr>
						            <td class="label">&nbsp;&nbsp;<%=("Cell Height:")%></td><td><input type="Text" name="frm_table_height" value="<%=table_height%>" size="2" maxlength="10" ></td><td class="label">pixels</td>
					            </tr>
					            <tr>
						            <td class="label">&nbsp;&nbsp;<%=("Cell Width:")%></td><td><input type="Text" name="frm_table_width" value="<%=table_width%>" size="2" maxlength="10"></td><td class="label">pixels</td>
					            </tr>
				            </table>
				        </td>
			        </tr>
			         -->
			        <tr>
				        <td class="label"><%= mMsgs.GetMessage("forward only") %>:</td>
			        </tr>
			        <tr>
				        <td class="label"><%= mMsgs.GetMessage("forward only instruct") %>:</td>
				        <td class="value">				            
				            <input type="checkbox" value="1" name="frm_forward_only" <%if (forward_only) then
						        response.write("checked=""true""")
						        end if%>/>
				        </td>
			        </tr>
			    </table>
			</fieldset>

            <div class="ektronTopSpace"></div>
            <fieldset>
                <legend><%= mMsgs.GetMessage("ev types") %></legend>
			    <table class="ektronGrid" width="100%">
			        <tr>
				        <td class="label"><%= mMsgs.GetMessage("ev types label") %>:</td>
				        <td><input type="text" name="frm_evtype_label" value="<%= evTypeLabel %>"/></td>
			        </tr>
			        <tr>
				        <td class="label"><%= mMsgs.GetMessage("ev types available") %>:</td>
				        <td class="value">
					        <input type="checkbox" name="frm_ev_type_avail" <%if (evTypeAvail) then
																        response.write("checked=""true""")
																        end if%> onclick="flipEvTypeReq();"/>
						</td>
					</tr>
					<tr>
					    <td class="label"><%= mMsgs.GetMessage("ev types required") %>:</td>
					    <td class="value">
					        <input type="checkbox" name="frm_ev_type_req" <%if (evTypeReq) then
													                    response.write("checked=""true""")
																		end if%>/>
				        </td>
			        </tr>
			        <script type="text/javascript">flipEvTypeReq();</script>
			        <tr>
				        <td class="label"><%= mMsgs.GetMessage("generic instructions") %>:</td>
				        <td><input type="text" name="frm_evtype_instruct" value="<%= EvTypeInstruct %>" /></td>
			        </tr>
			        <tr>
				        <td class="label"><%= mMsgs.GetMessage("show all label") %>:</td>
				        <td><input type="text" name="frm_evtype_showall" value="<%= EvTypeShowall %>"/><div id="EvTypeLabelDiv"></div></td>
			        </tr>
			    </table>
            </fieldset>			    

            <div class="ektronTopSpace"></div>
			<table class="ektronGrid" width="100%">
			    <tr>
				    <td class="label ektronLeft"><%= mMsgs.GetMessage("generic long description") %>:</td>
				    <td><%= LongDescriptionDisplay("frm_longDescAvail",longDescA) %></td>
			    </tr>
			    <!-- <tr>
				    <td class="label"><%=("Root Folder")%></td>
				    <td><%=gtNavs%></td>
			    </tr>
			    <tr>
				    <td class="label"><%= mMsgs.GetMessage("generic folder") %>:</td>
				    <td><span id="span_rootfolder_text"><%=gtNavs("Path")%></span></td>
			    </tr> -->
			    <tr>
				    <td class="label" valign="top"><%= mMsgs.GetMessage("select folder") %>:</td>
				    <td>
				        <span id="Div3"></span>
				        <span id="contentidspan">(<%=folderid %>) <%=FolderName %></span>
				        &nbsp;
				        <a href="#" id="a_change" name="a_change" onclick="javascript:LoadChildPage();return true;">Change</a>
				    </td>
			    </tr>
		    </table>
		</div>

        <input type="hidden" name="frm_table_height" id="frm_table_height" value="90" maxlength="10" />
        <input type="hidden" name="frm_table_width" id="frm_table_width" value="150" maxlength="10" />
		</form>
	<%
	end if
	%>
<% elseif (action="ViewEvTypes") Then
	calendar_id = Request.QueryString("calendar_id")

	gtNavs = siteRef.EkModuleRef.GetCalendarByID(calendar_id)

	if ((gtNavs.Count) And (len(ErrorString) = 0)) then
		fldObj = siteRef.EkContentRef.GetFolderInfoWithPath(gtNavs("FolderID"))
	end if
	if ((ErrorString <> "") OR (gtNavs.Count = 0)) then	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%= (GetTitleBar(mMsgs.GetMessage("view ev types"))) %>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmsCalendar.aspx?action=ViewCalendar&calendar_id=" & calendar_id & "&folderid=" & folderid , mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		    <div class="ektronPageContainer">
		        <div class="info"><%=(mMsgs.GetMessage("generic page error message"))%></div>
		        <div class="titlebar-error"><%=(ErrorString)%></div>
		    </div>
		</div>
	<% else %>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(mMsgs.GetMessage("view ev types for") & " """ & Server.HTMLEncode(gtNavs("CalendarTitle")) & """"))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(imgIconPath & "add.png", "cmsCalendar.aspx?action=AddEditEvType&calendar_id=" & calendar_id & "&folderid=" & folderid, mMsgs.GetMessage("alt Add a New Event Type"), mMsgs.GetMessage("btn add"), ""))%>
					    <% If (Not (gtnavs("EventTypes") is nothing)) Then %>
					    <%=(GetButtonEventsWCaption(imgIconPath & "remove.png", "cmsCalendar.aspx?action=RemoveEvTypes&calendar_id=" & calendar_id & "&folderid=" & folderid, mMsgs.GetMessage("alt Remove Event Type(s)"), mMsgs.GetMessage("btn minus"), ""))%>
					    <%=(GetButtonEventsWCaption(imgIconPath & "contentEdit.png", "cmsCalendar.aspx?action=EditEvTypesList&calendar_id=" & calendar_id & "&folderid=" & folderid, mMsgs.GetMessage("alt Modify an Event Type"), mMsgs.GetMessage("btn edit"), ""))%>
					    <% End If %>
					    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmsCalendar.aspx?action=ViewCalendar&calendar_id=" & calendar_id & "&folderid=" & folderid , mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>
		<div class="ektronPageContainer">
		    <table class="ektronGrid" width="100%">
		        <tr>
		            <td class="ektronHeader">
		                Event Types
		            </td>
		        </tr>
		        <% If gtnavs("EventTypes") is nothing Then
				        Response.Write ("No Event Types<br />")
			        Else
				        For each gtNav in gtNavs("EventTypes")
				            response.write("<tr><td>")
					        Response.Write (gtNav("EventTypeName") & "")
				            response.write("</td></tr>")					        
				        Next
		        End If %>
		    </table>
		</div>
	<% end If %>

<% elseif (action="EditEvTypesList") Then
	calendar_id = Request.QueryString("calendar_id")

	'Set contObj = Server.CreateObject(CONTENT_OBJ)
	'Set gtObj = Server.CreateObject(MODULE_OBJ)

	gtNavs = siteRef.EkModuleRef.GetCalendarByID(calendar_id)

	if ((gtNavs.Count) And (len(ErrorString) = 0)) then
		fldObj = siteRef.EkContentRef.GetFolderInfoWithPath(gtNavs("FolderID"))
	end if
	if ((ErrorString <> "") OR (gtNavs.Count = 0)) then	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%= (GetTitleBar(mMsgs.GetMessage("remove ev types"))) %>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmsCalendar.aspx?action=ViewEvTypes&calendar_id=" & calendar_id & "&folderid=" & folderid , mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		    <div class="info"><%=(mMsgs.GetMessage("generic page error message"))%></div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<% else %>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(mMsgs.GetMessage("edit ev type for") & " """ & Server.HTMLEncode(gtNavs("CalendarTitle")) & """"))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(imgIconPath & "contentEdit.png", "#", mMsgs.GetMessage("alt Click here to edit the selected Event Type"), mMsgs.GetMessage("btn edit"), "onClick=""JavaScript: return SubmitForm('editListForm','checkSelectEdit()') ;""")) %>
					    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmsCalendar.aspx?action=ViewEvTypes&calendar_id=" & calendar_id & "&folderid=" & folderid , mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>
		<div class="ektronPageContainer">
		    <table class="ektronGrid">
                <tr>
                    <td class="ektronHeader">
                        Event Types
                    </td>
                </tr>
		        <form id="editListForm" name="editListForm" action="<%= ("cmscalendar.aspx?action=AddEditEvType&calendar_id=" & calendar_id & "&folderid=" & folderid) %>" method="post">		        
			        <%
				        evtRadFirstPass = " CHECKED"
				        If gtnavs("EventTypes") is nothing Then
					        Response.Write ("No Event Types<br />")
				        Else
					        For each gtNav in gtNavs("EventTypes")
						        Response.Write ("<tr><td><input type=""radio"" name=""selEventType"" value=""" & gtNav("EventTypeID") & """ " & evtRadFirstPass & ">" & gtNav("EventTypeName") & "</td></tr>")
						        evtRadFirstPass = ""
					        Next
			        End If %>
		        </form>
            </table>	    
		    
		</div>
	<% end If %>
<% elseif (action="RemoveEvTypes") Then
	calendar_id = Request.QueryString("calendar_id")

	'Set contObj = Server.CreateObject(CONTENT_OBJ)
	'Set gtObj = Server.CreateObject(MODULE_OBJ)

	gtNavs = siteRef.EkModuleRef.GetCalendarByID(calendar_id)

	if ((gtNavs.Count) And (len(ErrorString) = 0)) then
		fldObj = siteRef.EkContentRef.GetFolderInfoWithPath(gtNavs("FolderID"))
	end if
	if ((ErrorString <> "") OR (gtNavs.Count = 0)) then	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%= (GetTitleBar(mMsgs.GetMessage("remove ev types"))) %>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmsCalendar.aspx?action=ViewEvTypes&calendar_id=" & calendar_id & "&folderid=" & folderid , mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		    <div class="info"><%=(mMsgs.GetMessage("generic page error message"))%></div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<% else %>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(mMsgs.GetMessage("remove ev types for") & " """ & Server.HTMLEncode(gtNavs("CalendarTitle")) & """"))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(imgIconPath & "delete.png", "#", mMsgs.GetMessage("alt Click here to delete the selected Event Types"), mMsgs.GetMessage("btn delete"), "onClick="" JavaScript: return SubmitForm('deleteform','ConfirmEvTypesDelete()') ;""")) %>
					    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmsCalendar.aspx?action=ViewEvTypes&calendar_id=" & calendar_id & "&folderid=" & folderid , mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>
		<div class="ektronPageContainer">
		    <div>
		        <table class="ektronForm" width="100%">
		            <tr>
		                <td class="ektronHeader">Event Types</td>
		            </tr>		        
		            <form name="deleteform" id="deleteform" action="<%= ("calendaraction.aspx?action=doDeleteEvTypes&calendar_id=" & calendar_id & "&folderid=" & folderid) %>" method="post">
		                <input type="hidden" name="frm_calendar_id" value="<%= calendar_id %>"/>
		                <% If (gtnavs("EventTypes") is nothing) Then
				                Response.Write ("No Event Types<br />")
			                Else
				                For each gtNav in gtNavs("EventTypes")
					                Response.Write ("<tr><td><input type=""checkbox"" name=""selEventType"" value=""" & gtNav("EventTypeID") & """>" & gtNav("EventTypeName") & "</td></tr>")
				                Next
		                End If %>
		            </form>
		        </table>
            </div>            
            <div class="clearfix ektronTopSpace" style="margin-left: .25em;">
                <a class="button greenHover buttonLeft buttonCheckAll" href="#" onclick="selectAllDelEvTypes();">
                    <%= mMsgs.GetMessage("generic select all msg") %>
                </a><a class="button redHover buttonLeft buttonClear" href="#" onclick="JavaScript: unSelectAllDelEvTypes();">
                    <%= mMsgs.GetMessage("generic clear all msg") %>
                </a>
            </div>
		</div>
	<% end If %>
<% elseif (action="AddEditEvType") Then
	calendar_id = Request.QueryString("calendar_id")
	Dim evTypeNameVal, evTypeID, evType, sEvTypeAction as Object
	evTypeID = Nothing
	gtNavs = siteRef.EkModuleRef.GetCalendarByID(calendar_id)

	If (Request.Form("selEventType") <> "") Then
		evType = siteRef.EkModuleRef.GetEventTypeByID(Request.Form("selEventType"))
		If (not (evType is nothing)) Then
			evTypeID = CLng(evType("EventTypeID"))
			evTypeNameVal = evType("EventTypeName")
		Else
			evTypeNameVal = ""
			evTypeID = 0
		End If
	Else
		evTypeNameVal = ""
	End If

	if ((gtNavs.Count) And (len(ErrorString) = 0)) then
		fldObj = siteRef.EkContentRef.GetFolderInfoWithPath(folderid)
	end if

	if ((ErrorString <> "") OR (gtNavs.Count = 0)) then	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%= (GetTitleBar(mMsgs.GetMessage("addedit ev types"))) %>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWcaption(imgIconPath & "back.png", "cmsCalendar.aspx?action=ViewEvTypes&calendar_id=" & calendar_id & "&folderid=" & folderid , mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		    <div class="info"><%=(mMsgs.GetMessage("generic page error message"))%></div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<% else %>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(mMsgs.GetMessage("addedit ev type for") & " """ & Server.HTMLEncode(gtNavs("CalendarTitle")) & """"))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%= (GetButtonEventsWCaption(imgIconPath & "save.png", "#", mMsgs.GetMessage("alt Click here to save the new Event Type"), mMsgs.GetMessage("btn save"), "onClick="" JavaScript: return SubmitForm('addForm','validateEvTypeAdd()') ;""")) %>
					    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmsCalendar.aspx?action=ViewEvTypes&calendar_id=" & calendar_id & "&folderid=" & folderid , mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>
		<div class="ektronPageContainer">
		    <table class="ektronGrid">
		        <tr>
		            <td class="label" colspan="2" style="text-align: left !important">
		                Add Event Type:
		            </td>
		            <td>
		                <%
		                If (evTypeID <> 0) Then
			                sEvTypeAction = "doUpdateEvTypes"
		                Else
			                sEvTypeAction = "doAddEvTypes"
		                End If%>
		                <form id="addForm" name="addForm" action="<%= ("calendaraction.aspx?action=" & sEvTypeAction & "&calendar_id=" & calendar_id & "&folderid=" & folderid) %>" method="post">
		                    <input type="hidden" name="frm_calendar_id" value="<%=(calendar_id)%>"/>
		                    <input type="hidden" name="frm_folder_id" value="<%=(folderid)%>"/>
		                    <input type="hidden" name="frm_ev_type_id" value="<%=(evTypeID)%>"/>
		                    <input type="text" name="frm_evTypeName" value="<%=(evTypeNameVal)%>" size="30" />
		                </form>
		            </td>
		        </tr>
		    </table>
		</div>
	<% end If %>
<%elseif(action="ViewCalendar") then
	calendar_id = request.QueryString("calendar_id")
	Dim calLang as Object = request.QueryString("LangType")

	contObj = siteRef.EkContentRef
	gtObj = siteRef.EkModuleRef
	gtNavs = gtObj.GetCalendarById(calendar_id)
	if ((gtNavs.Count) And (len(ErrorString) = 0)) then
		fldObj = contObj.GetFolderInfoWithPath( gtNavs("FolderID"))
	end if
	if ((ErrorString <> "") OR (gtNavs.Count = 0)) then	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%= (GetTitleBar(mMsgs.GetMessage("view calendar"))) %>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "collections.aspx", mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		    <div class="info"><%=(mMsgs.GetMessage("generic page error message"))%></div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<%
	else
	%>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(mMsgs.GetMessage("view calendar") & " """ & gtNavs("CalendarTitle") & """"))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(imgIconPath & "calendarView.png", "cmscalendar.aspx?action=ShowCalendar&calendar_id=" & gtNavs("CalendarID") & "&folderid=" & gtNavs("RootFolderID"), mMsgs.GetMessage("generic View") & " """ & Server.HTMLEncode(replace(gtNavs("CalendarTitle"), "'", "\'")) & """", mMsgs.GetMessage("btn show calendar"), ""))%>
					    <% if (IsUserAdmin(currentUserID,Site) OrElse AppUI.IsARoleMember(Common.EkEnumeration.CmsRoleIds.AdminCalendar)) Then
							    ImTheAdmin = true  %>
						    <%=(GetButtonEventsWCaption(imgIconPath & "contentEdit.png", "cmscalendar.aspx?action=EditCalendar&calendar_id=" & calendar_id &"&folderid="& folderid & "&evtype_label=" & Server.URLEncode(gtNavs("EventTypeLabel")) & "&title=" & Server.URLencode(gtNavs("CalendarTitle")) & "&description=" &  Server.URLencode(gtNavs("CalendarDescription")) & "&show_weekend=" & gtNavs("ShowWeekEnd") & "&table_height=" & gtNavs("TableHeight") & "&table_width=" & gtNavs("TableWidth") &  "&location_label=" & Server.URLencode(gtNavs("LocationLabel")) & "&start_label=" & Server.URLencode(gtNavs("StartLabel"))& "&end_label=" & Server.URLencode(gtNavs("EndLabel")) & "&forward_only=" & gtNavs("ForwardOnly") & "&ev_type_avail=" & gtNavs("AvailEvType") & "&ev_type_req=" & gtNavs("ReqEvType") & "&longDesc=" & gtNavs("LongDescriptionAvail") & "&evtinst=" & Server.UrlEncode(gtNavs("EvTypeInstruct")) & "&evtshowall=" & Server.UrlEncode(gtnavs("EvTypeShowAll")) , mMsgs.GetMessage("alt Click here to edit the properties of this calendar"), mMsgs.GetMessage("btn edit"), ""))%>
						    <% if gtNavs("AvailEvType") Then %>
							    <%=(GetButtonEventsWCaption(AppImgPath & "btn_eventtypes-nm.gif", "cmscalendar.aspx?action=ViewEvTypes&calendar_id=" & calendar_id & "&folder_id=" & folderid, mMsgs.GetMessage("alt Click here to edit the Event Types for this calendar"), mMsgs.GetMessage("btn event types"), ""))%>
						    <% end If %>
						    <%=(GetButtonEventsWCaption(imgIconPath & "delete.png", "calendaraction.aspx?action=doDelete&calendar_id=" & calendar_id & "&calLang=" & calLang, mMsgs.GetMessage("alt Click here to delete this calendar"), mMsgs.GetMessage("btn delete"), "OnClick=""javascript: return ConfirmCalendarDelete();"""))%>

					    <%	else
							    ImTheAdmin = false
						    end if%>
					    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "cmscalendar.aspx?action=ViewAllCalendars", mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td nowrap="nowrap"><% If (siteRef.EnableMultilingual) Then %>&nbsp;<%=(mMsgs.GetMessage("view in label"))%>: <%= ShowActiveLangsInList(False, "FFFFFF", "document.location.href='cmscalendar.aspx?langtype=' + this.value + '&action=ViewCalendar&calendar_id=" & gtNavs("CalendarID") & "&folderid=" & gtNavs("RootFolderID") & "' ;", siteRef.ContentLanguage, gtNavs("AvailLangs")) %>&nbsp;
					    <% if ImTheAdmin Then %>
						    <%=(mMsgs.GetMessage("add in label"))%>: <%= ShowActiveLangsNotInList(False, "FFFFFF", "document.location.href='cmscalendar.aspx?langtype=' + this.value + '&action=AddCalendar&calendar_id=" & gtNavs("CalendarID") & "&folderid=" & gtNavs("RootFolderID") & "' ;", siteRef.ContentLanguage, gtNavs("AvailLangs")) %><% End If %></td>
					    <% end If %>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>
	    <div class="ektronPageContainer ektronPageInfo">
		    <table class="ektronGrid" width="100%">
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic title label"))%></td>
				    <td><%=gtNavs("CalendarTitle")%></td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic id"))%>:</td>
				    <td><%=gtNavs("CalendarID")%></td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic description"))%>:</td>
				    <td><%=gtNavs("CalendarDescription")%></td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic location"))%>:</td>
				    <td><%=gtNavs("LocationLabel")%></td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic start time"))%>:</td>
				    <td><%=gtNavs("StartLabel")%></td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic end time"))%>:</td>
				    <td><%=gtNavs("EndLabel")%></td>
			    </tr>
			    <!--- <tr><td class="label" colspan="2">
						    <%if gtNavs("ForwardOnly") then
							    response.write("&nbsp;&nbsp;" & VerfiyTrue)
						    else
							    response.write("&nbsp;&nbsp;" & VerfiyFalse)
						    end if %>
						    Display Forward only events
						    </td></tr>  --->			    
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("lbl table properties") & ":")%></td>				    
			    </tr>
			    <tr>
				    <td colspan="2">
					    <!--
					    <tr>
						    <td class="label">&nbsp;&nbsp;<%=("Cell Height:")%></td><td><%=gtNavs("TableHeight")%>&nbsp;pixels</td>
					    </tr>
					    <tr>
						    <td class="label">&nbsp;&nbsp;<%=("Cell Width:")%></td><td><%=gtNavs("TableWidth")%>&nbsp;pixels</td>
					    </tr>
					    -->
					
					    <%if gtNavs("ShowWeekEnd") then
						    response.write("&nbsp;&nbsp;" & VerfiyTrue)
					    else
						    response.write("&nbsp;&nbsp;" & VerfiyFalse)
					    end if %>&nbsp;<%= mMsgs.GetMessage("display weekends") %>						    
				    </td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("forward only"))%>:</td>
			    </tr>
			    <tr>
				    <td colspan="2">
					    <%if gtNavs("ForwardOnly") then
						    response.write("&nbsp;&nbsp;" & VerfiyTrue)
					    else
						    response.write("&nbsp;&nbsp;" & VerfiyFalse)
					    end if %>&nbsp;<%= mMsgs.GetMessage("forward only instruct") %>
				    </td>
			    </tr>
			    <tr>
				    <td class="label ektronLeft"><%=(mMsgs.GetMessage("ev types"))%>:<br/></td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("ev types label"))%>:&nbsp;</td>
				    <td><%= gtNavs("EventTypeLabel") %><br/></td>
			    </tr>
			    <tr>
				    <td class="label" colspan="2">
                        <span class="ektronLeft">				    
					    <%if gtNavs("AvailEvType") then
						    response.write("&nbsp;&nbsp;" & VerfiyTrue)
					    else
						    response.write("&nbsp;&nbsp;" & VerfiyFalse)
					    end if %>&nbsp;<%=(mMsgs.GetMessage("ev types available"))%></span>
				    </td>
			    </tr>
			    <tr>
                    <td class="label" colspan="2">
                        <span class="ektronLeft">
                            <%if gtNavs("ReqEvType") then
						    response.write("&nbsp;&nbsp;" & VerfiyTrue)
					    else
						    response.write("&nbsp;&nbsp;" & VerfiyFalse)
					    end if %>
					    <%=(mMsgs.GetMessage("ev types required"))%></span>
                    </td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic instructions"))%>:</td>
				    <td><%=gtNavs("EvTypeInstruct")%></td>
			    </tr>
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("show all label"))%>:</td>
				    <td><%=gtNavs("EvTypeShowAll")%></td>
			    </tr>			    
			    <tr>
				    <td class="label ektronLeft"><%=(mMsgs.GetMessage("generic long description"))%>:</td>
				    <td><%= LongDescriptionDisplay("",gtNavs("LongDescriptionAvail")) %></td>
			    </tr>			    
			    <!--<tr>
				    <td class="label"><%=("Folder Name")%></td>
				    <td><%=fldObj("FolderName")%></td>
			    </tr> -->
			    <tr>
				    <td class="label"><%=(mMsgs.GetMessage("generic folder"))%>:</td>
				    <td><%=fldObj("Path")%></td>
			    </tr>
		    </table>
		</div>
	<%
	gtobj = nothing
	gtNavs = nothing
	end if %>
<%elseif(action="ViewAllCalendars") then
	gtObj = siteRef.EkModuleRef
	gtNavs = gtObj.GetAllCalendarInfo()
	FolderName = mMsgs.GetMessage("collections report title bar")
	if (ErrorString <> "") then
	%>
		<div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%= (GetTitleBar(mMsgs.GetMessage("view all calendars"))) %>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <%=(GetButtonEventsWCaption(imgIconPath & "back.png", "collections.aspx", mMsgs.GetMessage("alt back button text"), mMsgs.GetMessage("btn back"), ""))%>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		    <div class="info"><%=(mMsgs.GetMessage("generic page error message"))%></div>
		    <div class="titlebar-error"><%=(ErrorString)%></div>
		</div>
	<%
	else
	%>
		<div class="ektronPageHeader">
	        <div class="ektronTitlebar">
		        <%=(GetTitleBar(mMsgs.GetMessage("calendar modules")))%>
		    </div>
		    <div class="ektronToolbar">
			    <table>
				    <tr>
					    <% if ((IsUserAdmin(currentUserID,Site) OrElse AppUI.IsARoleMember(Common.EkEnumeration.CmsRoleIds.AdminCalendar)) And (siteRef.ContentLanguage <> ALL_CONTENT_LANGUAGES)) then %>
						    <%=(GetButtonEventsWCaption(imgIconPath & "add.png", "cmscalendar.aspx?action=AddCalendar&folderid=0", mMsgs.GetMessage("alt Click here to add another calendar"), mMsgs.GetMessage("btn add calendar"), ""))%>
					    <% else %>
						    <td>&nbsp;</td>
					    <%end if%>
					    <td>
						    <% If (siteRef.EnableMultilingual) Then %>
							    &nbsp;<%=(mMsgs.GetMessage("lbl view"))%>: <%= ShowAllActiveLanguage(True, "FFFFFF", "document.location.href = 'CmsCalendar.aspx?action=ViewAllCalendars&langtype=' + document.getElementById('frm_langID').value ;", siteRef.ContentLanguage) %>
						    <% End If %>
					    </td>
					    <td><%=objStyle.GetHelpButton("calendarmodule" & action)%></td>
				    </tr>
			    </table>
		    </div>
		</div>
		<div class="ektronPageContainer ektronPageGrid">
		    <table class="ektronGrid" width="100%">
			    <tr class="title-header">
				    <td width="25%"><%=(mMsgs.GetMessage("generic title"))%></td>
				    <td width="5%"><%= (mMsgs.GetMessage("generic id")) %></td>
				    <td width="5%"><%= (mMsgs.GetMessage("generic Language")) %></td>
				    <td><%= (mMsgs.GetMessage("generic description")) %></td>
				    <td><%= (mMsgs.GetMessage("generic path")) %></td>
			    </tr>
		    <%
		    if (gtNavs.Count) then
		        dim i as integer = 0
			    For i = 1 to gtNavs.count %>
			        <tr>
				        <td><a href="cmscalendar.aspx?action=ViewCalendar&calendar_id=<%=(gtNavs(i)("CalendarID"))%>&folderid=<%=gtNavs(i)("FolderID")%>&LangType=<%= gtNavs(i)("ContentLanguage") %>" alt='<%=(mMsgs.GetMessage("generic View") & " """ & replace(gtNavs(i)("CalendarTitle"), "'", "`") & """")%>' title='<%=(mMsgs.GetMessage("generic View") & " """ & replace(gtNavs(i)("CalendarTitle"), "'", "`") & """")%>'><%=(gtNavs(i)("CalendarTitle"))%></a></td>
				        <td><%=(gtNavs(i)("CalendarID"))%></td>
				        <td><%=(gtNavs(i)("ContentLanguage"))%></td>
				        <td><%=(gtNavs(i)("CalendarDescription"))%></td>
				        <td><%=(gtNavs(i)("FolderPath"))%></td>
			        </tr>			        
			    <%
			    Next
			    %>
		    </table>
		</div>
		<%
		end if
	gtObj = Nothing
	gtNavs = Nothing
	end if
' gtMsgObj = Nothing
' mMsgs.GetMessage = Nothing
End if %>
</body>
</html>
