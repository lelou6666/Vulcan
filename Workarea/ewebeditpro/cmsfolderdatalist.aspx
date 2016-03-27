<%@ Page ContentType="text/xml" Language="vb" AutoEventWireup="false" %><?xml version="1.0" encoding="utf-8"?>
<select>
<%
	Dim strParam As String
    Dim nFolderID As Long
	Dim bRecursive As Boolean
	Dim nLangType As Integer
	nFolderID = 0
	bRecursive = False
	nLangType = -1
	strParam = Request.QueryString("id")
	If Not IsNothing(strParam) AndAlso IsNumeric(strParam) Then
        nFolderID = Convert.ToInt64(strParam)
		If nFolderID < 0 Then nFolderID = 0
	End If
	strParam = Request.QueryString("recursive")
	If Not IsNothing(strParam) AndAlso ("1" = strParam OrElse "true" = strParam.ToLower) Then
		bRecursive = True
	End If
	strParam = Request.QueryString("LangType")
	If Not IsNothing(strParam) AndAlso IsNumeric(strParam) Then
		nLangType = Convert.ToInt32(strParam)
		If nFolderID < -1 Then nFolderID = -1
	End If
	
	Dim objListSummary As New Ektron.Cms.Controls.ListSummary
	objListSummary.Page = Me
	objListSummary.FolderID = nFolderID
	objListSummary.Recursive = bRecursive
	objListSummary.LanguageID = nLangType
	' objListSummary.MaxResults = some number

	Dim iItem As Integer
	Dim strName As String
	Dim strValue As String
	For iItem = 0 To objListSummary.EkItems.Length - 1
		strName = objListSummary.EkItems(iItem).Title
		strName = strName.Replace("&amp;#39;", "'")
		strValue = Convert.ToString(objListSummary.EkItems(iItem).Id)
		' strValue = objListSummary.EkItems(iItem).QuickLink
		Response.Write("<option value=""" & strValue & """>" & strName & "</option>" & vbCrLf)
	Next
%>
</select>
