<%@ Page Language="vb" aspcompat="true" AutoEventWireup="false" validateRequest="false" %>
<%@ Import Namespace="Ektron.Cms.UI.CommonUI" %>
<%@ Import Namespace="Ektron.Cms.Content" %>
<%@ Import Namespace="Ektron.Cms.Site" %>
<%@ Import Namespace="Ektron.Cms" %>
<%@ Import Namespace="Ektron.Cms.Common" %>

<script language="vb" runat="server">
Dim AppUI as New ApplicationAPI
    Const ALL_CONTENT_LANGUAGES As Integer = -1
    Const CONTENT_LANGUAGES_UNDEFINED As Integer = 0
    Dim AppPath As String = ""
Dim AppImgPath As String=""
Dim ContentLanguage As Integer
dim MsgHelper as EkMessageHelper
Dim m_refLib As Ektron.Cms.Library.EkLibrary
dim m_refContent as Ektron.Cms.Content.EkContent
dim m_refModule as Ektron.Cms.Modules.EkModule
    Dim CurrentUserId, sitePath, AppName, AppeWebPath, EnableMultilingual As Object
    Dim username, foldertype, password, action, cUser, strOnload, ErrString, xmlstring, catid, uid, siteid, InfoMessage, ContentID, mycmshtmstring As Object
    Dim loopcount, h, domain, protocol, ret, ErrorString, cDbObj, myerrormsg, cFolders, cFolder, userObj, objSite As Object
    Dim gtObj, gtNav, gtNavs, cCont, cConts, ekContObj As Object
dim loopnumber as integer = 0
    Dim cid, myFolders, UserRights As Object
    Dim NavArray() As Object
dim RI As Common.EkRequestInformation


    Function addLBpaths(ByRef cFolder As Object, ByVal currentUserID As Object, ByVal Site As Object) As Object
        Dim lbICount, lbFCount, locLibObj, lb, cLbs As Object
        lbICount = 0
        lbFCount = 0
        m_refLib = AppUI.EkLibraryRef
        cLbs = m_refLib.GetAllLBPaths("images")
        If cLbs.Count Then
            For Each lb In cLbs
                lbICount = lbICount + 1
                cFolder.Add("LoadBalanceImagePath_" & lbICount, getPhysicalPath(lb("LoadBalancePath")))
            Next
        End If
        cFolder.Add("LoadBalanceImageCount", lbICount)
        cLbs = Nothing
        lb = Nothing
        cLbs = m_refLib.GetAllLBPaths("files")
        If cLbs.Count Then
            For Each lb In cLbs
                lbFCount = lbFCount + 1
                cFolder.Add("LoadBalanceFilePath_" & lbFCount, getPhysicalPath(lb("LoadBalancePath")))
            Next
        End If
        cFolder.Add("LoadBalanceFileCount", lbFCount)
        cLbs = Nothing
        locLibObj = Nothing
        addLBpaths = False
    End Function

    Function getPhysicalPath(ByVal path As Object) As Object
        On Error Resume Next
        getPhysicalPath = Server.MapPath(path)
        On Error GoTo 0
    End Function

    Function OutputContentFolders(ByVal IncomingString As Object, ByVal Parent As Object, ByRef TestArray As Object) As Object
        Dim cContent As Collection
        Dim contObj As Object
        contObj = m_refContent
        cContent = new Collection()
        cContent.Add(Parent, "ParentID")
        cContent.Add("name", "OrderBy")
        cFolders = contObj.GetAllViewableChildFoldersv2_0(cContent)

        For Each cFolder In cFolders
            ReDim Preserve TestArray(loopnumber + 1)
            ' myFolders = cDbObj.GetFolderInfov2_0( uid, cFolder("ID"), siteid, ErrString)
            ' FolderType

            If (foldertype = "blog" And cFolder("FolderType") = 1) Or (foldertype = "forum" And cFolder("FolderType") = 3) Or foldertype = "all" Then
                TestArray(loopnumber) = "<catid>" & cFolder("ID") & "</catid>" _
                      & "<catname>" & IncomingString & "\" & cFolder("name") & "</catname>" ' <catxml>" &  myFolders("XmlConfiguration").Count  & "</catxml>"
                ' myFolders = nothing
                loopnumber = loopnumber + 1
            End If
            OutputContentFolders(IncomingString & "\" & cFolder("name"), cFolder("ID"), TestArray)

        Next
        cContent = Nothing
        Return Nothing
    End Function
    Function Login(ByVal user As Object, ByVal pass As Object, ByVal domain As Object) As Object
        xmlstring = ""
        protocol = AppUI.AuthProtocol
        userObj = AppUI.EkUserRef()
        domain = Common.EkConstants.CreateADsPathFromDomain(domain)
        cUser = userObj.logInUser(username, password, Request.ServerVariables("SERVER_NAME"), domain, protocol)
        userObj = Nothing
        If ((cUser.count) And (ErrString = "")) Then
            xmlstring = "<count>1</count>" & "<userid>" & cUser("UserID") & "</userid><siteid>" & sitePath & "," & cUser("LoginNumber") & "</siteid><sitepath>" & sitePath & "</sitepath>"
            Response.Write(xmlstring)
        Else
            xmlstring = "<count>0</count><sitepath>" & sitePath & "</sitepath>"
            Response.Write(xmlstring)
        End If

        cUser = Nothing
        Return Nothing
    End Function

	private Function GetMenuList(byval FolderID as string, byval OrderBy as string) as string
        Dim RefString As String = ""
		dim result as object
        Dim results As Object = AppUI.EkContentRef.GetMenuReport(RefString)

        Dim xml As Object = New System.Text.StringBuilder()

		xml.Append("<count>" & results.count & "</count>")
		for each result in results
			xml.Append("<id>" & result("MenuID").ToString() & "</id>")
			xml.Append("<title>" & result("MenuTitle").ToString() & "</title>")
		next

		return xml.ToString()
	End Function
	Private Function GetLanguages() as string
		Dim results as object
		Dim result as object
		Dim key as string
		results = AppUI.EkSiteRef.GetAllActiveLanguages()
        Dim xml As Object = New System.Text.StringBuilder()
		xml.Append("<count>" & results.count & "</count>")
			for each key in results.Keys
				result = cType(results(key), System.Collections.Hashtable)
				xml.Append("<id>" & result("ID").ToString()  & "</id>")
				xml.Append("<title>" & result("Name").ToString() & "</title>")
			next
		return xml.ToString()
	End Function
	Private Function GetXmlConfigurationList(Byval OrderBy as string) as string
		Dim result as object
        Dim results As Object = AppUI.EkContentRef.GetAllXmlConfigurations(OrderBy)
        Dim xml As Object

		xml = "<count>" & results.count & "</count>"

		for each  result in results
			xml &= "<id>" & result("CollectionID").ToString() & "</id>"
			Xml &= "<title>" & result("CollectionTitle").ToString() + "</title>"
		next

		return xml
	End Function
</script>
<%
MsgHelper=AppUI.EkMsgRef
m_refLib = AppUI.EkLibraryRef
CurrentUserId=AppUI.UserId
AppImgPath=AppUI.AppImgPath
sitePath=AppUI.SitePath
AppName=AppUI.AppName
AppeWebPath=AppUI.AppeWebPath
AppPath=AppUI.AppPath
EnableMultilingual=AppUI.EnableMultilingual

if (AppUI.ContentLanguage = -1) then
	AppUI.ContentLanguage = AppUI.GetCookieValue("DefaultLanguage")
end if

If (Request.QueryString("LangType")<>"") Then
		ContentLanguage=Request.QueryString("LangType")
		AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage)
	else
		if CStr(AppUI.GetCookieValue("LastValidLanguageID")) <> ""  then
			ContentLanguage = AppUI.GetCookieValue("LastValidLanguageID")
		end if
End If
AppUI.ContentLanguage=ContentLanguage

    RI = AppUI.RequestInformationRef
    RI.CallerId = Request.QueryString("uid")
    RI.CookieSite = Request.QueryString("siteid")
    RI.UserId = Request.QueryString("uid")
m_refContent = new EkContent(RI)
m_refLib = new Ektron.Cms.Library.EkLibrary(RI)
m_refModule = new Ektron.Cms.Modules.EkModule(RI)


action=trim(Request.QueryString ("action"))

if action = "logintest" then
	username=request.Querystring("u")
	password=request.Querystring("p")
	domain = request.Querystring("d")
	Login(username, password, domain)
elseif action = "categories" then
        Dim TestArray() As Object
	redim TestArray(0)
        Dim loopnumber, g As Object
        loopnumber = 0
        foldertype = Request.QueryString("foldertype")
	cDbObj = m_refContent
	cFolders = cDbObj.GetFolderInfov2_0(0)
	OutputContentFolders("", 0, TestArray)
	if ErrString <> "" then
		response.write("<count>" & UBOUND(TestArray) - LBOUND(TestArray) & "</count>")
	else

            If foldertype = "all" Then
				Response.Write("<count>" & UBound(TestArray) - LBound(TestArray) + 1  & "</count>")
                Response.Write("<catid>0</catid><catname>\</catname>")

            Else
                Response.Write("<count>" & UBound(TestArray) - LBound(TestArray) - 1 & "</count>")
            End If

            End If
            For g = LBound(TestArray) To UBound(TestArray) - 1
                Response.Write(TestArray(g))
            Next

elseif action = "content" then
        Dim cTmp, ContObj As Object
        Dim ContentArray() As Object
            ReDim ContentArray(0)
            loopcount = 0
            catid = Request.QueryString("catid")
            ContObj = m_refContent
            cTmp = New Collection
            cTmp.Add(catid, "FolderID")
            cTmp.Add("Title", "OrderBy")
            cConts = ContObj.GetAllViewableChildContentInfoV4_2(cTmp)
            For Each cCont In cConts
                ReDim Preserve ContentArray(loopcount + 1)
                ContentArray(loopcount) = "<id>" & cCont("ID") & "</id>" _
                       & "<title>" & cCont("Title") & "</title>"
                loopcount = loopcount + 1
            Next
            Response.Write("<count>" & UBound(ContentArray) - LBound(ContentArray) & "</count>")
            For h = LBound(ContentArray) To UBound(ContentArray) - 1
                Response.Write(ContentArray(h))
            Next
            ContObj = Nothing

elseif action="collections" then
        Dim orderby As Object
            ReDim NavArray(0)
            loopcount = 0
            catid = Request.QueryString("catid")
            orderby = "title"
            gtObj = m_refContent
            gtNavs = gtObj.GetAllCollectionsInfo(catid, orderby)
            For Each gtNav In gtNavs
                ReDim Preserve NavArray(loopcount + 1)
                NavArray(loopcount) = "<id>" & gtNav("CollectionID") & "</id>" _
                       & "<title>" & gtNav("CollectionTitle") & "</title>"
                loopcount = loopcount + 1
            Next
            Response.Write("<count>" & UBound(NavArray) - LBound(NavArray) & "</count>")
            For h = LBound(NavArray) To UBound(NavArray) - 1
                Response.Write(NavArray(h))
            Next
            gtNavs = Nothing
            gtObj = Nothing

elseif action="collections_all" then
            ReDim NavArray(0)
            loopcount = 0
            catid = Request.QueryString("catid")
            uid = Request.QueryString("uid")
            siteid = Request.QueryString("siteid")
            gtObj = m_refContent
            gtNavs = gtObj.GetCollectionsReport(ErrorString)
            For Each gtNav In gtNavs
                ReDim Preserve NavArray(loopcount + 1)
                NavArray(loopcount) = "<id>" & gtNav("CollectionID") & "</id>" _
                       & "<title>id=" & gtNav("CollectionID") & ", " & gtNav("CollectionTitle") & "</title>"
                loopcount = loopcount + 1
            Next
            Response.Write("<count>" & UBound(NavArray) - LBound(NavArray) & "</count>")
            For h = LBound(NavArray) To UBound(NavArray) - 1
                Response.Write(NavArray(h))
            Next
            gtNavs = Nothing
            gtObj = Nothing


elseif action = "calendar" then
            ReDim NavArray(0)
            loopcount = 0
            gtObj = m_refModule
            gtNavs = gtObj.GetAllCalendarInfo()
            For Each gtNav In gtNavs
                ReDim Preserve NavArray(loopcount + 1)
                NavArray(loopcount) = "<id>" & gtNav("CalendarID") & "</id>" _
                       & "<title>id=" & gtNav("CalendarID") & ", " & gtNav("CalendarTitle") & "</title>"
                loopcount = loopcount + 1
            Next
            Response.Write("<count>" & UBound(NavArray) - LBound(NavArray) & "</count>")
            For h = LBound(NavArray) To UBound(NavArray) - 1
                Response.Write(NavArray(h))
            Next
            gtNavs = Nothing
            gtObj = Nothing


elseif action = "ecmforms" then
        Dim gtForms, gtForm As Object
            ReDim NavArray(0)
            loopcount = 0
            gtObj = m_refModule
            gtForms = gtObj.GetAllFormInfo()

            For Each gtForm In gtForms
                ReDim Preserve NavArray(loopcount + 1)
                NavArray(loopcount) = "<id>" & gtForm("FormID") & "</id>" _
                       & "<title>id=" & gtForm("FormID") & ", " & gtForm("FormTitle") & "</title>"
                loopcount = loopcount + 1
            Next
            Response.Write("<count>" & UBound(NavArray) - LBound(NavArray) & "</count>")
            For h = LBound(NavArray) To UBound(NavArray) - 1
                Response.Write(NavArray(h))
            Next
            gtNavs = Nothing
            gtObj = Nothing

elseif action = "editcontent" then
        Dim cEContent As Object
        Dim myxmlstring As Object
        Dim PageInfo As Object
        Dim myhtmlcontent As Object
        Dim stylesheetpath As Object = Nothing
        Dim mywhynoteditstring As Object
        Dim canI As Object

            PageInfo = "src=""http://" & Request.ServerVariables("Server_name") & "/"
			cid = Request.QueryString("cid")
            ekContObj = m_refContent
        canI = ekContObj.CanIv2_0(cid, "content")
            If (canI("CanIEdit")) Then
            cEContent = ekContObj.GetContentForEditingV2_0(cid)
                myhtmlcontent = cEContent("ContentHtml")
                If Trim(cEContent("StyleSheet")) <> "" Then
                    stylesheetpath = "http://" & Request.ServerVariables("Server_name") & sitePath & cEContent("StyleSheet")
                End If
                myxmlstring = "<ektron_head_html><!--   Do not remove these xml tags <ektron_head><ektron_edit>yes</ektron_edit><ektron_content_id>" & cid & "</ektron_content_id> <ektron_title>" & cEContent("ContentTitle") & "</ektron_title> <ektron_content_comment>" & cEContent("Comment") & "</ektron_content_comment> <ektron_content_stylesheet>" & stylesheetpath & "</ektron_content_stylesheet> <ektron_folder_id>" & cEContent("FolderID") & "</ektron_folder_id> <ektron_content_language>" & cEContent("ContentLanguage") & "</ektron_content_language> <ektron_go_live>" & cEContent("GoLive") & "</ektron_go_live> <ektron_end_date>" & cEContent("EndDate") & "</ektron_end_date> <ektron_MetadataNumber>" & cEContent("ContentMetadata").Count & "</ektron_MetadataNumber> <ektron_PreviousState>" & cEContent("CurrentContentStatus") & "</ektron_PreviousState> <ektron_iMaxContLength>" & cEContent("MaxContentSize") & "</ektron_iMaxContLength> <ektron_content_Path>" & cEContent("Path") & "</ektron_content_Path></ektron_head> --> </ektron_head_html> "
                myxmlstring = myxmlstring & "<ektron_body_html>" & myhtmlcontent & "</ektron_body_html>"
            Else
                If (canI("ContentState") = "CheckedOut") Then
                    mywhynoteditstring = "You can not edit this cotent, content is checked out to: " & canI("UserName")
                Else
                    mywhynoteditstring = "You can not edit this this content.  The content state is:  " & canI("ContentState") & "; The user is " & canI("UserName")
                End If
                myxmlstring = "<ektron_head_html><!--   <ektron_edit>" & mywhynoteditstring & "</ektron_edit> --></ektron_head_html>"
            End If
            Response.Write(myxmlstring)
            canI = Nothing

elseif action = "publish_update_content" then
        ErrorString = ""
        myerrormsg = ""
        cid = Request.QueryString("cid")
        ekContObj = m_refContent
        Dim cCont As Collection = ekContObj.GetContentByIDv2_0(cid)

        cCont.Remove("ContentHtml")
        cCont.Add(Replace(Request.Form("ContentHTML"), "<myektronand/>", "&"), "ContentHtml")
        cCont.Remove("ContentTitle")
        cCont.Add(Request.Form("content_title"), "ContentTitle")
        cCont.Remove("Comment")
        cCont.Add(Request.Form("content_comment"), "Comment")
        cCont.Remove("ContentID")
        cCont.Add(Request.Form("ektron_content_id"), "ContentID")
        cCont.Remove("FolderID")
        cCont.Add(Request.Form("folder"), "FolderID")
        cCont.Remove("ContentLanguage")
        cCont.Add(Request.Form("ektron_content_language"), "ContentLanguage")
        'cCont.Remove("SearchText")
        cCont.Add(Replace(Request.Form("searchhtml"), "<myektronand/>", "&"), "SearchText")
        cCont.Remove("GoLive")
        cCont.Add(Request.Form("go_live"), "GoLive")
        cCont.Remove("EndDate")
        cCont.Add(Request.Form("end_date"), "EndDate")
        cCont.Remove("ContentType")
        cCont.Add(1, "ContentType")
        cCont.Remove("LockedContentLink")
        cCont.Add(True, "LockedContentLink")
        ekContObj = m_refContent

        Try

            ret = ekContObj.SaveContentv2_0(cCont)
            If (ErrorString <> "") Then
                myerrormsg = ErrorString
            Else
                ContentID = Request.Form("ektron_content_id")
                ret = ekContObj.CheckIn(ContentID)
                If (ErrorString <> "") Then
                    myerrormsg = ErrorString
                Else
                    If (ret = False) Then
                        ret = ekContObj.SubmitForPublicationv2_0(ContentID, Request.Form("ektron_folder_id"))
                    End If
                End If
            End If
        Catch ex As Exception
            myerrormsg = ex.Message.ToString()
        End Try

        If (myerrormsg <> "") Then
            Response.Write(myerrormsg)
        Else
            Response.Write("Content has been saved")
        End If

    ElseIf action = "add_new_content" Then
        cCont = New Collection()
        Try
            cCont.Add(Replace(Request.Form("ContentHTML"), "<myektronand/>", "&"), "ContentHtml")
            cCont.Add(Request.Form("content_title"), "ContentTitle")
            cCont.Add(Request.Form("content_comment"), "Comment")
            cCont.Add(CLng(Request.Form("folder")), "FolderID")
            cCont.Add(Request.Form("ektron_content_language"), "ContentLanguage")
            cCont.Add(Replace(Request.Form("searchhtml"), "<myektronand/>", "&"), "SearchText")
            cCont.Add(Request.Form("go_live"), "GoLive")
            cCont.Add(Request.Form("end_date"), "EndDate")
            cCont.Add(True, "AddToQlink")
            cCont.add(1, "ContentType")
            ekContObj = m_refContent
            ErrorString = ""
            ContentID = ekContObj.AddNewContentv2_0(cCont)
        Catch ex As Exception
            ErrorString = ex.Message.ToString()
        End Try
        If (ErrorString <> "") Then
            myerrormsg = ErrorString
        Else
            ret = ekContObj.CheckIn(ContentID)
            If (ErrorString <> "") Then
                myerrormsg = " Error CheckIn= " & ErrorString
            Else
                If (ret = False) Then
                    ret = ekContObj.SubmitForPublicationv2_0(ContentID, Request.Form("ektron_folder_id"))
                    If (ErrorString <> "") Then
                        myerrormsg = "Error SubmitForPublicationv2_0= " & ErrorString
                    End If
                End If
            End If
        End If
        If (myerrormsg <> "") Then
            Response.Write(myerrormsg)
        Else
            Response.Write("Content has been saved")
        End If

    ElseIf action = "foldertypexml" Then
        Dim folderid As Object
        folderid = Request.Form("folderid")
        cDbObj = m_refContent
        myFolders = cDbObj.GetFolderInfov2_0(folderid)
        If (myFolders("XmlConfiguration").Count = 0) Then
            Response.Write("<XmlConfiguration>no</XmlConfiguration>")
        Else
            Response.Write("<XmlConfiguration>yes</XmlConfiguration>")
        End If
        myFolders = Nothing
    ElseIf action = "canIsave" Then
            cid = Request.Form("cid")
            ekContObj = m_refContent
            cConts = ekContObj.GetContentByIDv2_0(cid)
            UserRights = ekContObj.CanIv2_0(cid, "content")
            If (cConts("ContentStatus") = "O" And UserRights("CanIEdit")) Then
                Response.Write("<canISave>yes</canISave>")
            Else
                If cConts("ContentStatus") = "O" Then
                    Response.Write("<canISave>Did not save!!!!,  Check out by " & UserRights("UserName") & "</canISave>")
                Else
                    Response.Write("<canISave>Did not save!!!  Content is not checked out to you, Its status is Status=" & cConts("ContentStatus") & " The user associated with this is " & UserRights("UserName") & "</canISave>")
                End If
            End If

    ElseIf (action = "AddFolder") Then
        Dim brObj, libObj, bRet1, tmpPath, libSettings As Object
        brObj = m_refContent
        cFolder = New Collection()
            cFolder.Add(Request.Form("foldername"), "FolderName")
            cFolder.Add(Request.Form("folderdescription"), "FolderDescription")
            cFolder.Add(Request.Form("ParentID"), "ParentID")
            cFolder.Add(Request.Form("templatefilename"), "TemplateFileName")
            cFolder.Add(Request.Form("stylesheet"), "StyleSheet")
            libObj = m_refLib
            libSettings = libObj.GetLibrarySettingsv2_0()

            tmpPath = libSettings("ImageDirectory")
            cFolder.Add(getPhysicalPath(tmpPath), "AbsImageDirectory")
            tmpPath = libSettings("FileDirectory")
            cFolder.Add(getPhysicalPath(tmpPath), "AbsFileDirectory")
            libSettings = Nothing
            libObj = Nothing
            bRet1 = addLBpaths(cFolder, uid, siteid)
            cFolder.Add(True, "XmlInherited")
        cFolder.Add(0, "XmlConfiguration")
        cFolder.Add(1, "InheritMetadata") 'break inherit button is check.
        cFolder.Add(0, "InheritMetadataFrom")
            ret = brObj.AddContentFolderv2_0(cFolder)
            Response.Write("Folder added")

    ElseIf action = "metadata" Then
        Dim loopcounter, MetaObj, cMetadataTypes, cMetadataType, j As Object
        Dim MetaDataArray() As Object
            ReDim MetaDataArray(0)
            loopcounter = 0
            MetaObj = m_refContent
            cMetadataTypes = MetaObj.GetMetadataTypes("Name")
            For Each cMetadataType In cMetadataTypes
                ReDim Preserve MetaDataArray(loopcounter + 1)
                MetaDataArray(loopcounter) = "<meta>" & (cMetadataType("MetaTypeName")) & "</meta>"
                loopcounter = loopcounter + 1
            Next
            Response.Write("<count>" & UBound(MetaDataArray) - LBound(MetaDataArray) & "</count>")
            For j = LBound(MetaDataArray) To UBound(MetaDataArray) - 1
                Response.Write(MetaDataArray(j))
            Next

    ElseIf action = "ecmmenus" Then
        Dim FolderID As Object
            FolderID = 0
            Response.Write(GetMenuList(FolderID, "Title"))
    ElseIf action = "ecmxmconfig" Then
            Response.Write(GetXmlConfigurationList(""))
    ElseIf action = "ecmlanguages" Then
        Response.Write(GetLanguages())
    Else
        Response.Write("DreamWeaver - No Action parameter")
    End If

%>