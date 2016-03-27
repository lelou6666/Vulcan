Imports System
Imports System.Text
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkConstants
Imports System.Web.HttpContext
Imports Microsoft.VisualBasic
Imports System.Web

Public Class StyleHelper
    Private myTemp As System.String = ""
    Private MyButtonName As System.Int32 = 100
    Private DisplayTransText As System.Boolean = False
    Private m_refMsg As Ektron.Cms.Common.EkMessageHelper = Nothing
    Private m_refAPI As CommonApi = Nothing
    Private ContentLanguage As Integer = -1

    Public Sub New()
        m_refAPI = New CommonApi
        Dim strLangID As String
        strLangID = Current.Request.QueryString("LangType")
        If (Not IsNothing(strLangID) AndAlso IsNumeric(strLangID)) Then
            Try
                ContentLanguage = Convert.ToInt32(strLangID)
            Catch ex As Exception
                EkException.LogException(ex, Diagnostics.EventLogEntryType.Warning)
                EkException.WriteToEventLog("Language string was: " + strLangID, Diagnostics.EventLogEntryType.Warning)
            End Try
            m_refAPI.SetCookieValue("LastValidLanguageID", ContentLanguage)
        Else
            strLangID = m_refAPI.GetCookieValue("LastValidLanguageID")
            If (Not IsNothing(strLangID) AndAlso IsNumeric(strLangID)) Then
                ContentLanguage = Convert.ToInt32(strLangID)
            End If
        End If
        If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            m_refAPI.ContentLanguage = ALL_CONTENT_LANGUAGES
        Else
            m_refAPI.ContentLanguage = ContentLanguage
        End If
        DisplayTransText = m_refAPI.DisplayTransText
        m_refMsg = m_refAPI.EkMsgRef
    End Sub
    Public Function CreateBoundField(ByVal DataField As String, ByVal HeaderText As String, ByVal CssClass As String, ByVal HeaderHorizontalAlign As System.Web.UI.WebControls.HorizontalAlign, ByVal ItemHorizontalAlign As System.Web.UI.WebControls.HorizontalAlign, ByVal HeaderWidth As System.Web.UI.WebControls.Unit, ByVal ItemWidth As System.Web.UI.WebControls.Unit, ByVal HtmlEncode As Boolean, ByVal ItemWrap As Boolean) As System.Web.UI.WebControls.BoundField
        Dim colBound As New System.Web.UI.WebControls.BoundField
        colBound.DataField = DataField
        colBound.HeaderText = HeaderText
        colBound.HeaderStyle.CssClass = CssClass
        colBound.HeaderStyle.Width = HeaderWidth
        colBound.ItemStyle.Width = ItemWidth
        colBound.ItemStyle.HorizontalAlign = ItemHorizontalAlign
        colBound.HeaderStyle.HorizontalAlign = HeaderHorizontalAlign
        colBound.HtmlEncode = HtmlEncode
        colBound.ItemStyle.Wrap = ItemWrap
        Return colBound
    End Function
    Public Function HyperlinkWCaption(ByVal HrefPath As System.String, ByVal DisplayText As System.String, ByVal HeaderText As System.String, ByVal specialEvents As System.String) As System.String
        Dim result As StringBuilder = Nothing
        Try
            result = New StringBuilder
            result.Append("<a href=""" & HrefPath & """ onMouseOver=""")

            If (DisplayTransText) Then
                If (HeaderText <> "") Then
                    result.Append("ShowTransString('" & Replace(HeaderText, "'", "\'") & "');")
                End If
            End If
            result.Append(""" onMouseOut=""")
            If (DisplayTransText) Then
                If (HeaderText <> "") Then
                    result.Append("HideTransString();")
                End If
            End If
            result.Append(""">" & DisplayText & "</a>")

            MyButtonName = MyButtonName + 1
        Catch ex As Exception
            result.Length = 0
        Finally
        End Try
        Return (result.ToString)
    End Function
    Public Function GetButtonEvents(ByVal ImageFile As System.String, ByVal hrefPath As System.String, ByVal altText As System.String, ByVal specialEvents As System.String) As System.String
        Dim result As StringBuilder = Nothing
        Try
            result = New StringBuilder
            result.Append("<td ")
            result.Append("id=""image_cell_" & MyButtonName & """ ")
            result.Append("class=""button"" title=""" & altText & """>")
            result.Append("<a id=""image_link_" & MyButtonName & """ href=""" & hrefPath & """ " & specialEvents & " ")
            result.Append("onMouseOver=""RollOver(this);"" onMouseOut=""RollOut(this);"" style=""cursor: default;"">")
            result.Append("<img onClick=""" & "SelectButton(this);" & """ src=""" & ImageFile & """ id=""image_" & MyButtonName & """ class=""button"">")
            result.Append("</a></td>")
            MyButtonName = MyButtonName + 1
        Catch ex As Exception
            result.Length = 0
        End Try
        Return (result.ToString)
    End Function
    Public Function GetButtonEventsWCaption(ByVal imageFile As System.String, ByVal hrefPath As System.String, ByVal altText As System.String, ByVal HeaderText As System.String, ByVal specialEvents As System.String) As System.String
        Dim result As StringBuilder = Nothing
        Try
            result = New StringBuilder
            result.Append("<td ")
            result.Append("id=""image_cell_" & MyButtonName & """ ")
            result.Append("class=""button"" title=""" & altText & """>")
            result.Append("<a id=""image_link_" & MyButtonName & """ href=""" & hrefPath & """ " & specialEvents & " ")
            result.Append("onMouseOver=""")
            If (DisplayTransText) Then
                If (HeaderText <> "") Then
                    result.Append("ShowTransString('" & Replace(HeaderText, "'", "\'") & "');")
                End If
            End If
            result.Append("RollOver(this);"" onMouseOut=""")
            If (DisplayTransText) Then
                If (HeaderText <> "") Then
                    result.Append("HideTransString();")
                End If
            End If
            result.Append("RollOut(this);""")
            result.Append(" style=""cursor: default;"">")
            result.Append("<img onClick=""" & "SelectButton(this);" & """ src=""" & imageFile & """ id=""image_" & MyButtonName & """ class=""button"">")
            result.Append("</a></td>")

            MyButtonName = MyButtonName + 1
        Catch ex As Exception
            result.Length = 0
        Finally
        End Try
        Return (result.ToString)
    End Function

    ''' <summary>
    ''' Converts an ASP.Net image button so it works in the CMS400 toolbar
    ''' All your code is handled in the codebehind for the button click
    ''' </summary>
    ''' <param name="btn">button to update</param>
    ''' <param name="altText">alt text for button for flyover help</param>
    ''' <param name="HeaderText">text to put in toolbar header on mouseover of button</param>
    ''' <remarks></remarks>
    Public Sub MakeToolbarButton(ByVal btn As ImageButton, _
            ByVal altText As String, _
            ByVal HeaderText As String)
        btn.AlternateText = altText
        btn.Attributes.Add("onMouseOver", "ShowTransString('" + HeaderText + "'); RollOver(this);")
        btn.Attributes.Add("onMouseOut", "HideTransString(); RollOut(this);")
        btn.Attributes.Add("style", "cursor:default;")
        btn.Attributes.Add("style", "border-width:1px")
    End Sub

    Public Function GetTitleBar(ByVal Title As System.String) As System.String
        Dim result As New StringBuilder
        Try
            result.Append("<span id=""WorkareaTitlebar"">" & Title & "</span>")
            result.Append("<span style=""display:none"" id=""_WorkareaTitlebar""></span>")
        Catch ex As Exception
            result.Length = 0
        End Try
        Return (result.ToString)
    End Function
    Public Function GetShowAllActiveLanguage(ByVal showAllOpt As System.Boolean, ByVal bgColor As System.String, ByVal OnChangeEvt As System.String, ByVal SelLang As System.String) As System.String
        Return GetShowAllActiveLanguage(showAllOpt, bgColor, OnChangeEvt, SelLang, False)
    End Function
    Public Function GetShowAllActiveLanguage(ByVal showAllOpt As System.Boolean, ByVal bgColor As System.String, ByVal OnChangeEvt As System.String, ByVal SelLang As System.String, ByVal showOnlySiteEnabled As Boolean) As System.String
        Return ("<td>" & ShowAllActiveLanguage(showAllOpt, bgColor, OnChangeEvt, SelLang, showOnlySiteEnabled) & "</td>")
    End Function
    Public Function ShowAllActiveLanguage(ByVal showAllOpt As System.Boolean, ByVal bgColor As System.String, ByVal OnChangeEvt As System.String, ByVal SelLang As System.String) As System.String
        Return ShowAllActiveLanguage(showAllOpt, bgColor, OnChangeEvt, SelLang, False)
    End Function
    Public Function ShowAllActiveLanguage(ByVal showAllOpt As System.Boolean, ByVal bgColor As System.String, ByVal OnChangeEvt As System.String, ByVal SelLang As System.String, ByVal showOnlySiteEnabled As Boolean) As System.String
        Dim result As New StringBuilder()
        Dim language_data As LanguageData()
        Dim m_refSiteApi As New SiteAPI
        Dim LanguageId As Integer = m_refSiteApi.ContentLanguage
        Try
            If (OnChangeEvt = "") Then
                OnChangeEvt = "SelLanguage(this.value)"
            End If
            If SelLang.Trim <> "" Then
                LanguageId = Convert.ToInt32(SelLang)
            End If
            language_data = m_refSiteApi.GetAllActiveLanguages()
            result = New StringBuilder
            If (m_refAPI.EnableMultilingual = 1) Then
                result.Append("<select id=""frm_langID"" name=""frm_langID"" OnChange=""" & OnChangeEvt & """>" & vbCrLf)
                If CBool(showAllOpt) Then
                    result.Append("<option value=""" & ALL_CONTENT_LANGUAGES & """")
                    If (LanguageId = ALL_CONTENT_LANGUAGES) Then
                        result.Append(" selected=""selected""")
                    End If
                    result.Append(">")
                    result.Append("All")
                    result.Append("</option>")
                End If
                If (Not (IsNothing(language_data))) Then
                    For iLang As Integer = 0 To language_data.Length - 1
                        With language_data(iLang)
                            If (.Id <> 0 AndAlso (.SiteEnabled OrElse showOnlySiteEnabled = False)) Then
                                result.Append("<option value=""" & .Id & """")
                                If (LanguageId = .Id) Then
                                    result.Append(" selected=""selected""")
                                Else
                                End If
                                result.Append(">")
                                result.Append(.LocalName)
                                result.Append("</option>")
                            End If
                        End With
                    Next
                End If
                result.Append("</select>")
            End If
        Catch ex As Exception
            result.Length = 0
        End Try
        Return (result.ToString)
    End Function
    'Public Shared Function GetClientScript() As String
    Public Function GetClientScript() As String
        Dim result As StringBuilder
        Try
            Dim m_refAPI As New CommonApi
            If ((Not (Current.Request.QueryString("LangType") Is Nothing)) AndAlso (Current.Request.QueryString("LangType") <> "")) Then
                m_refAPI.ContentLanguage = Current.Request.QueryString("LangType")
            End If

            Dim page As Page = HttpContext.Current.Handler
            Ektron.Cms.API.Css.RegisterCss(page, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
            Ektron.Cms.API.Css.RegisterCss(page, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
            Ektron.Cms.API.JS.RegisterJS(page, Ektron.Cms.API.JS.ManagedScript.EktronJS)
            Ektron.Cms.API.JS.RegisterJS(page, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
            Ektron.Cms.API.JS.RegisterJS(page, m_refAPI.AppPath & "java/stylehelper.js", "EktronStyleHelperJS")

            result = New StringBuilder
            result.Append("<script type=""text/javascript"">" & vbCrLf)
            result.Append("<!--//--><![CDATA[//><!--" & vbCrLf)
            result.Append(" " & vbCrLf)
            result.Append("var g_relativeClassPath = '" & m_refAPI.AppPath & "csslib/';" & vbCrLf)
            result.Append("g_relativeClassPath = g_relativeClassPath.toLowerCase();" & vbCrLf)
            result.Append("UpdateWorkareaTitleToolbars();" & vbCrLf)
            result.Append(" " & vbCrLf)
            result.Append("function GetRelativeClassPath() {" & vbCrLf)
            result.Append("    return(g_relativeClassPath);" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append(" " & vbCrLf)
            result.Append("function UpdateWorkareaTitleToolbars() {" & vbCrLf)
            result.Append("    if (document.styleSheets.length > 0) {" & vbCrLf)
            result.Append("        MakeClassPathRelative('*', 'button', 'backgroundImage', '" & m_refAPI.AppImgPath & "', g_relativeClassPath)" & vbCrLf)
            result.Append("        MakeClassPathRelative('*', 'button-over', 'backgroundImage', '" & m_refAPI.AppImgPath & "', g_relativeClassPath)" & vbCrLf)
            result.Append("        MakeClassPathRelative('*', 'button-selected', 'backgroundImage', '" & m_refAPI.AppImgPath & "', g_relativeClassPath)" & vbCrLf)
            result.Append("        MakeClassPathRelative('*', 'button-selectedOver', 'backgroundImage', '" & m_refAPI.AppImgPath & "', g_relativeClassPath)" & vbCrLf)
            result.Append("        MakeClassPathRelative('*', 'ektronToolbar', 'backgroundImage', '" & m_refAPI.AppImgPath & "', g_relativeClassPath)" & vbCrLf)
            result.Append("        MakeClassPathRelative('*', 'ektronTitlebar', 'backgroundImage', '" & m_refAPI.AppImgPath & "', g_relativeClassPath)" & vbCrLf)
            result.Append("    } else {" & vbCrLf)
            result.Append("        setTimeout('UpdateWorkareaTitleToolbars()', 500);" & vbCrLf)
            result.Append("    }" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("function ShowTransString(Text) {" & vbCrLf)
            result.Append("var ObjId = ""WorkareaTitlebar"";" & vbCrLf)
            result.Append("var ObjShow = document.getElementById('_' + ObjId);" & vbCrLf)
            result.Append("var ObjHide = document.getElementById(ObjId);" & vbCrLf)
            result.Append("if ((typeof ObjShow != ""undefined"") && (ObjShow != null)) {" & vbCrLf)
            result.Append("ObjShow.innerHTML = Text;" & vbCrLf)
            result.Append("ObjShow.style.display = ""inline"";" & vbCrLf)
            result.Append("if ((typeof ObjHide != ""undefined"") && (ObjHide != null)) {" & vbCrLf)
            result.Append("ObjHide.style.display = ""none"";" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("}")
            result.Append("function HideTransString() {" & vbCrLf)
            result.Append("var ObjId = ""WorkareaTitlebar"";" & vbCrLf)
            result.Append("var ObjShow = document.getElementById(ObjId);" & vbCrLf)
            result.Append("var ObjHide = document.getElementById('_' + ObjId);" & vbCrLf)

            result.Append("if ((typeof ObjShow != ""undefined"") && (ObjShow != null)) {" & vbCrLf)
            result.Append("ObjShow.style.display = ""inline"";")
            result.Append("if ((typeof ObjHide != ""undefined"") && (ObjHide != null)) {" & vbCrLf)
            result.Append("ObjHide.style.display = ""none"";" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("function GetCellObject(MyObj) {" & vbCrLf)
            result.Append("var tmpName = """";" & vbCrLf)

            result.Append("tmpName = MyObj.id;" & vbCrLf)
            result.Append("if (tmpName.indexOf(""link_"") >= 0) {" & vbCrLf)
            result.Append("tmpName = tmpName.replace(""link_"", ""cell_"");" & vbCrLf)
            result.Append("}")
            result.Append("else if (tmpName.indexOf(""cell_"") >= 0) {" & vbCrLf)
            result.Append("tmpName = tmpName;" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("else {" & vbCrLf)
            result.Append("tmpName = tmpName.replace(""image_"", ""image_cell_"");" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("MyObj = document.getElementById(tmpName);" & vbCrLf)
            result.Append("return (MyObj);" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("var g_OldBtnObject = null;" & vbCrLf)

            result.Append("function ClearPrevBtn() {" & vbCrLf)
            result.Append("if (g_OldBtnObject){" & vbCrLf)
            result.Append("  RollOut(g_OldBtnObject);" & vbCrLf)
            result.Append("  g_OldBtnObject = null;" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("function RollOver(MyObj) {" & vbCrLf)
            result.Append("ClearPrevBtn();" & vbCrLf)
            result.Append("g_OldBtnObject = MyObj;" & vbCrLf)
            result.Append("MyObj = GetCellObject(MyObj);")
            result.Append("MyObj.className = ""button-over"";" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("function RollOut(MyObj) {" & vbCrLf)
            result.Append("if (g_OldBtnObject == MyObj){" & vbCrLf)
            result.Append("g_OldBtnObject = null;" & vbCrLf)
            result.Append("}" & vbCrLf)
            result.Append("MyObj = GetCellObject(MyObj);" & vbCrLf)
            result.Append("MyObj.className = ""button"";" & vbCrLf)
            result.Append("}" & vbCrLf)

            result.Append("function SelectButton(MyObj) {" & vbCrLf)

            result.Append("}" & vbCrLf)

            result.Append("function UnSelectButtons() { " & vbCrLf)
            result.Append("var iLoop = 100; " & vbCrLf)

            result.Append("while (document.getElementById(""image_cell_"" + iLoop.toString()) != null) { " & vbCrLf)
            result.Append("document.getElementById(""image_cell_"" + iLoop.toString()).className = ""button""; " & vbCrLf)
            result.Append("iLoop++; " & vbCrLf)
            result.Append("} " & vbCrLf)
            result.Append("} " & vbCrLf)

            result.Append("function Trim (string) { " & vbCrLf)
            result.Append("if (string.length > 0) { " & vbCrLf)
            result.Append("string = RemoveLeadingSpaces (string); " & vbCrLf)
            result.Append("} " & vbCrLf)
            result.Append("if (string.length > 0) { " & vbCrLf)
            result.Append("string = RemoveTrailingSpaces(string); " & vbCrLf)
            result.Append("} " & vbCrLf)
            result.Append("return string; " & vbCrLf)
            result.Append("} " & vbCrLf)

            result.Append("function RemoveLeadingSpaces(string) {")
            result.Append("while(string.substring(0, 1) == "" "") { " & vbCrLf)
            result.Append("string = string.substring(1, string.length); " & vbCrLf)
            result.Append("} " & vbCrLf)
            result.Append("return string; " & vbCrLf)
            result.Append("} " & vbCrLf)

            result.Append("function RemoveTrailingSpaces(string) { " & vbCrLf)
            result.Append("while(string.substring((string.length - 1), string.length) == "" "") { " & vbCrLf)
            result.Append("string = string.substring(0, (string.length - 1)); " & vbCrLf)
            result.Append("} " & vbCrLf)
            result.Append("return string; " & vbCrLf)
            result.Append("} " & vbCrLf)

			result.Append("function SelLanguage(inVal) { " & vbCrLf)
			Dim myTemp As System.String = Current.Request.ServerVariables("PATH_INFO").Substring(Current.Request.ServerVariables("PATH_INFO").LastIndexOf("/") + 1)
            myTemp = myTemp & "?" & Ektron.Cms.API.JS.Escape(Replace(Current.Request.ServerVariables("QUERY_STRING"), "LangType", ""))
            myTemp = Replace(myTemp, "'", """")
			myTemp = Replace(myTemp, "\x", "\\x")
			myTemp = Replace(myTemp, "\\\x", "\\x")
			myTemp = Replace(myTemp, "\u", "\\u")
			myTemp = Replace(myTemp, "\\\u", "\\u")
            myTemp = Replace(myTemp, "SelectAll=1&", "")

            result.Append("top.notifyLanguageSwitch(inVal, -1)" & vbCrLf)

            result.Append("document.location = '" & myTemp & "&LangType=' + inVal ; " & vbCrLf)
            result.Append("} " & vbCrLf)
            result.Append("//--><!]]>" & vbCrLf)
            result.Append("</script> " & vbCrLf)
        Catch ex As Exception
            result = New StringBuilder
        Finally
        End Try
        Return (result.ToString)
    End Function
    Public Function GetAddAnchor(ByVal Id As Integer) As String
        Dim sResult As String = ""
        Dim sFormQuery As String = ""
        Try
            If (((Not IsNothing(Current.Request.QueryString("ContType"))) AndAlso (Current.Request.QueryString("ContType").ToString = "2")) Or ((Not IsNothing(Ektron.Cms.CommonApi.GetEcmCookie()("ContType"))) AndAlso (Ektron.Cms.CommonApi.GetEcmCookie()("ContType").ToString = "2"))) Then
                sFormQuery = "&folder_id=" & Id '& "&callbackpage=content.aspx&parm1=action&value1=viewcontentbycategory&parm2=folder_id&value2=" & Id
            End If
            sResult = (GetButtonEventsWCaption(m_refAPI.AppPath & _
             "images/UI/Icons/add.png", "#", _
             m_refMsg.GetMessage("alt add content button text"), _
             m_refMsg.GetMessage("btn add content"), _
             "onclick=""AddNewContent('LangType=" & ContentLanguage & _
              "&type=add&createtask=1&id=" & Id & sFormQuery & "&" & _
              GetBackParams() & "');return false;"" "))
        Catch ex As Exception
            sResult = ""
        End Try
        Return sResult
    End Function

    Public Function GetAddAnchorByContentType(ByVal Id As Long, ByVal contType As Integer, Optional ByVal AllowNonFormattedHTML As Boolean = False) As String
        Dim sResult As String = ""
        Dim sFormQuery As String = ""
        Try
            If (contType = 2) Then
                sFormQuery = "&folder_id=" & Id '& "&callbackpage=content.aspx&parm1=action&value1=viewcontentbycategory&parm2=folder_id&value2=" & Id
            End If
            sResult = "AddNewContent('LangType=" & ContentLanguage & _
              "&type=add&createtask=1&id=" & Id & "&folderid=" & Id & sFormQuery & "&" & _
              GetBackParams()
            If (AllowNonFormattedHTML) Then
                sResult &= "&AllowHTML=1"
            End If
            sResult &= "', " & contType & ");"
        Catch ex As Exception
            sResult = ""
        End Try
        Return sResult
    End Function

    Public Function GetTypeOverrideAddAnchor(ByVal Id As Long, ByVal xml_id As Long, ByVal contType As Integer) As String
        Dim sResult As String = "AddNewContent('LangType=" & ContentLanguage & _
                "&type=add&createtask=1&id=" & Id & "&xid=" & xml_id & "&" & _
                 GetBackParams() & "'," & contType & "); "
        Return sResult
    End Function
    Function GetAddOtherAnchor(ByVal id As Object) As String
        Dim sResult As String = GetButtonEventsWCaption(m_refAPI.AppPath _
            & "images/UI/Icons/add.png", "#", _
            m_refMsg.GetMessage("alt add content button text"), _
            m_refMsg.GetMessage("btn add content"), _
            "OnClick=""showMultiMenu(event)"" ")
        Return sResult
    End Function
    Public Function GetAddOtherMenuStyle() As String
        Dim html As String = "" & vbCrLf
        html += "<STYLE>" & vbCrLf
        html += "	#xmladdMenu { " & vbCrLf
        html += "	position: absolute; " & vbCrLf
        html += "	visibility: hidden; " & vbCrLf
        html += "	width: 120px; " & vbCrLf
        html += "	background-color: lightgrey; " & vbCrLf
        html += "	layer-background-color: lightgrey; " & vbCrLf
        html += "	border: 2px outset white; " & vbCrLf
        html += "	} " & vbCrLf
        html += "	.A:Menu { " & vbCrLf
        html += "	color: black; " & vbCrLf
        html += "	text-decoration: none; " & vbCrLf
        html += "	cursor: default; " & vbCrLf
        html += "	width: 100% " & vbCrLf
        html += "	} " & vbCrLf
        html += "	.A:MenuOn { " & vbCrLf
        html += "	color: white; " & vbCrLf
        html += "	text-decoration: none; " & vbCrLf
        html += "	background-color: darkblue; " & vbCrLf
        html += "	cursor: default; " & vbCrLf
        html += "	width: 100% " & vbCrLf
        html += "	} " & vbCrLf
        html += "</STYLE> " & vbCrLf
        GetAddOtherMenuStyle = html
    End Function
    Public Function GetAddBlogPostAnchor(ByVal Id As Long) As String
        Dim sResult As String = ""
        Try
            sResult = (GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/add.png", "#", m_refMsg.GetMessage("alt add blogpost button text"), m_refMsg.GetMessage("btn add blogpost"), "OnClick=""javascript:AddNewContent('LangType=" & ContentLanguage & "&type=add&createtask=1&id=" & Id & "&" & GetBackParams() & "');return false;"" "))
        Catch ex As Exception
            sResult = ""
        End Try
        Return sResult
    End Function
    Public Function GetAddForumPostAnchor(ByVal Id As Long) As String
        Dim sResult As String = ""
        Try
            sResult = (GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/add.png", "#", m_refMsg.GetMessage("alt add forumpost button text"), m_refMsg.GetMessage("btn add forumpost"), "OnClick=""javascript:AddNewContent('LangType=" & ContentLanguage & "&type=add&createtask=1&id=" & Id & "&" & GetBackParams() & "');return false;"" "))
        Catch ex As Exception
            sResult = ""
        End Try
        Return sResult
    End Function
    Function GetAddMultiAnchor(ByVal id As Long, ByVal ContType As Integer) As String
        Return (GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/add.png", "#", "Add Several Files", m_refMsg.GetMessage("btn add content"), "OnClick=""javascript:AddNewContent('LangType=" & ContentLanguage & "&type=add&multi=" & ContType & "&createtask=1&id=" & id & "&" & GetBackParams() & "',  " & CMSContentType_AllTypes & ");return false;"" "))
    End Function
    Private Function GetBackParams() As String
        Dim backURL As StringBuilder
        Dim Value As Object
        backURL = New StringBuilder
        backURL.Append("back_file=content.aspx")
        Value = Current.Request.QueryString("action")
        If (Len(Value) > 0) Then
            backURL.Append("&back_action=" & Value)
        End If
        Value = Current.Request.QueryString("folder_id")
        If (Len(Value) > 0) Then
            backURL.Append("&back_folder_id=" & Value)
        End If
        Value = Current.Request.QueryString("id")
        If (Len(Value) > 0) Then
            backURL.Append("&back_id=" & Value)
        End If
        Value = ContentLanguage
        If (Len(Value) > 0) Then
            backURL.Append("&back_LangType=" & ContentLanguage)
        End If
        Return (backURL.ToString)
    End Function
    Function StatusWithToolTip(ByVal Status As String) As String
        Dim ToolTip As String = ""
        Dim result As StringBuilder = Nothing
        Try
            result = New StringBuilder

            Select Case UCase(Status)
                Case "A"
                    ToolTip = "The content has been through the workflow and published on the Web site."
                Case "O"
                    ToolTip = "The content is currently being edited, and has not been checked in for publishing."
                Case "I"
                    ToolTip = "The content has been checked in for other users to edit."
                Case "S"
                    ToolTip = "The content block has been saved and submitted into the approval chain."
                Case "M"
                    ToolTip = "This content has been requested to be deleted from Ektron CMS400."
                Case "P"
                    ToolTip = "This content has been approved,but the Go Live date hasn’t occurred yet."
                Case "T"
                    ToolTip = "This content has been submitted, but waiting for completion of associated tasks."
                Case "D"
                    ToolTip = "This content has been mark for delete on the Go Live date."
            End Select
            result.Append("<a href=""#Status"" onmouseover=""ddrivetip('" & ToolTip & "','ADC5EF', 300);"" onmouseout=""hideddrivetip();"" onclick=""return false;"">" & Status & "</a>")
        Catch ex As Exception
        Finally
        End Try
        Return (result.ToString)
    End Function
    Public Function GetPermLayerTop() As String
        Dim result As String = ""
        Dim bNS6 As Boolean = False
        If (InStr(UCase(Current.Request.ServerVariables("http_user_agent")), "GECKO")) Then
            bNS6 = True
        Else
            bNS6 = False
        End If
        If (Not (bNS6)) Then
            result = result & "<ILAYER name=""permLayer""><LAYER name=""standard"" visibility=""show""><NOLAYER>"
        End If
        result = result & "<div id=""standard"" style=""display: block;"">"
        If (Not (bNS6)) Then
            result = result & "</NOLAYER>"
        End If
        Return (result)
    End Function
    Public Function GetPermLayerMid() As String
        Dim result As String = ""
        Dim bNS6 As Boolean = False
        If (InStr(UCase(Current.Request.ServerVariables("http_user_agent")), "GECKO")) Then
            bNS6 = True
        Else
            bNS6 = False
        End If
        If (Not (bNS6)) Then
            result = result & "</LAYER><LAYER name=""advanced"" visibility=""hidden""><NOLAYER>"
        End If
        result = result & "</div><div id=""advanced"" style=""display: none;"">"
        If (Not (bNS6)) Then
            result = result & "</NOLAYER>"
        End If
        Return (result)
    End Function
    Public Function GetPermLayerBottom() As String
        Dim bNS6 As Boolean = False
        Dim result As String = ""
        If (InStr(UCase(Current.Request.ServerVariables("http_user_agent")), "GECKO")) Then
            bNS6 = True
        Else
            bNS6 = False
        End If
        If (Not (bNS6)) Then
            result = result & "</LAYER></ILAYER><NOLAYER>"
        End If
        result = result & "</div>"
        If (Not (bNS6)) Then
            result = result & "</NOLAYER>"
        End If
        Return (result)
    End Function
    Public Function PermissionFlag(ByVal PermFlag As Boolean) As Integer
        Dim result As Integer = 0
        If (PermFlag) Then
            result = 1
        End If
        Return (result)
    End Function
    Public Function GetEnableAllPrompt() As String

        Dim enableAll As String
        enableAll = "<div id=""enablealldiv"" class=""clearfix"">"
        enableAll += "<a href=""#EnableAll"" class=""button greenHover buttonLeft buttonCheckAll"" onclick=""return SelectAllPerms();"" title=""" & m_refMsg.GetMessage("enable all permissions") & """>" & m_refMsg.GetMessage("generic Enable All") & "</a>"
        enableAll += "<a href=""#DisableAll"" class=""button redHover buttonLeft buttonClear"" onclick=""return UnselectAllPerms();"" title=""" & m_refMsg.GetMessage("generic Disable All") & """>" & m_refMsg.GetMessage("generic Disable All") & "</a>"
        enableAll += "</div>"

        Return enableAll
    End Function
    Public Function GetSwapNavPrompt() As String
        Dim bNS6 As Boolean = False
        Dim result As String = ""
        If (InStr(UCase(Current.Request.ServerVariables("http_user_agent")), "GECKO")) Then
            bNS6 = True
        Else
            bNS6 = False
        End If
        If (Not (bNS6)) Then
            result = result & "<ILAYER name=""messLayer""><LAYER name=""advancedMess"" visibility=""show""><NOLAYER>"
        End If
        result = result & "<div id=""advancedMess"" style=""display: block;"">"
        If (Not (bNS6)) Then
            result = result & "</NOLAYER>"
        End If
        result = result & "<a href=""#"" onClick=""SwapPermDisplay();return false;"" title=""" & m_refMsg.GetMessage("alt display adv perms text") & """ alt=""" & m_refMsg.GetMessage("alt display adv perms text") & """>" & m_refMsg.GetMessage("display advanced permissions msg") & "</a>"
        If (Not (bNS6)) Then
            result = result & "<NOLAYER>"
        End If
        result = result & "</div>"
        If (Not (bNS6)) Then
            result = result & "</NOLAYER></LAYER><LAYER name=""standardMess"" visibility=""hidden""><NOLAYER>"
        End If
        result = result & "<div id=""standardMess"" style=""display: none;"">"
        If (Not (bNS6)) Then
            result = result & "</NOLAYER>"
        End If
        result = result & "<a href=""#"" onClick=""SwapPermDisplay();return false;"" title=""" & m_refMsg.GetMessage("alt display std perms text") & """ alt=""" & m_refMsg.GetMessage("alt display std perms text") & """>" & m_refMsg.GetMessage("display standard permissions msg") & "</a>"
        If (Not (bNS6)) Then
            result = result & "<NOLAYER>"
        End If
        result = result & "</div>"
        If (Not (bNS6)) Then
            result = result & "</NOLAYER></LAYER></ILAYER>"
        End If
        Return (result)
    End Function
    Public Function GetMemberShipNavSwap(ByVal action As String, ByVal strType As String, ByVal id As Object, ByVal membership As Object) As String
        Dim result As String = ""
        If (membership = "true") Then
            result = "<a href=""content.aspx?LangType=" & ContentLanguage & "&action=" & action & "&type=" & strType & "&id=" & CStr(id) & "&membership=false"">" & m_refMsg.GetMessage("lbl view cms users") & "</a>"
        Else
            result = "<a href=""content.aspx?LangType=" & ContentLanguage & "&action=" & action & "&type=" & strType & "&id=" & id & "&membership=true"">" & m_refMsg.GetMessage("lbl view memberShip users") & "</a>"
        End If
        Return (result)
    End Function
    Public Function GetCatalogEditAnchor(ByVal Id As Long, Optional ByVal Type As Integer = 3333, Optional ByVal bFromApproval As Boolean = False) As String
        Dim result As String = ""
        Dim SRC As String = ""
        Dim str, backStr As String
        If (Type = 1) Then
            If bFromApproval Then
                backStr = "back_file=approval.aspx"
            Else
                backStr = "back_file=content.aspx"
            End If
        Else
            backStr = "back_file=cmsform.aspx"
        End If
        str = Current.Request.QueryString("action")
        If (Len(str) > 0) Then
            backStr = backStr & "&back_action=" & str
        End If

        If bFromApproval Then
            str = Current.Request.QueryString("page")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_page=" & str
            End If
        End If

        If Not bFromApproval Then
            str = Current.Request.QueryString("folder_id")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_folder_id=" & str
            End If
        End If

        If (Type = 1) Then
            str = Current.Request.QueryString("id")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_id=" & str
            End If
        Else
            str = Current.Request.QueryString("form_id")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_form_id=" & str
            End If
        End If
        If (Not IsNothing(Current.Request.QueryString("callerpage"))) Then
            str = Current.Request.QueryString("callerpage")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_callerpage=" & str
            End If
        End If
        If (Not IsNothing(Current.Request.QueryString("origurl"))) Then
            str = Current.Request.QueryString("origurl")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_origurl=" & HttpUtility.UrlEncode(str)
            End If
        End If
        str = ContentLanguage
        If (Len(str) > 0) Then
            backStr = backStr & "&back_LangType=" & str & "&rnd=" & CInt(Int((10 * Rnd()) + 1))
        End If

        SRC = "commerce/catalogentry.aspx?close=false&LangType=" & ContentLanguage & "&id=" & Id & "&type=update&" & backStr
        If (bFromApproval) Then
            SRC &= "&pullapproval=true"
        End If
        result = GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/contentEdit.png", "#", m_refMsg.GetMessage("alt edit button text"), m_refMsg.GetMessage("btn edit"), "OnClick=""javascript:top.document.getElementById('ek_main').src='" & SRC & "';return false;""" & ",'EDIT',790,580,1,1);return false;""")
        Return (result)
    End Function
    Public Function GetPageBuilderEditAnchor(ByVal Id As Long, ByVal languageId As Integer, ByVal quickLink As String) As String
        'make popup window with link to this pageids wireframe, and pass in the id and an edit flag
        Dim capi As New ContentAPI()
        Dim fd As FolderData = capi.GetFolderById(capi.GetFolderIdForContentId(Id))

        Dim URL As String = ""
        Dim u As New Ektron.Cms.API.UrlAliasing.UrlAliasCommon
        URL = u.GetAliasForContent(Id)

        If (URL = String.Empty Or fd.IsDomainFolder) Then
            URL = quickLink
        End If

        If (URL.Contains("?")) Then
            URL = URL + "&ektronPageBuilderEdit=true"
        Else
            URL = URL + "?ektronPageBuilderEdit=true"
        End If
        If (URL.IndexOf("LangType=") = -1) Then
            URL = URL + "&LangType=" + LanguageId.ToString()
        End If
        URL = Me.m_refAPI.SitePath + URL
        URL = GetButtonEventsWCaption(m_refAPI.AppImgPath & "layout_edit.gif", _
                                         "#", _
                                         m_refMsg.GetMessage("generic edit page layout"), _
                                         m_refMsg.GetMessage("generic edit page layout"), _
                                         "OnClick=""window.open('" & URL & "', 'CMS400EditPage');return false;""")
        Return URL
    End Function
    Public Function GetPageBuilderEditAnchor(ByVal Id As Long, ByRef contentdata As ContentData) As String
        'make popup window with link to this pageids wireframe, and pass in the id and an edit flag

        Dim URL As String = ""
        If (contentdata.SubType = EkEnumeration.CMSContentSubtype.PageBuilderData OrElse contentdata.SubType = EkEnumeration.CMSContentSubtype.PageBuilderMasterData) Then
            If (contentdata.ContType = EkEnumeration.CMSContentType.Content OrElse (contentdata.ContType = EkEnumeration.CMSContentType.Archive_Content And contentdata.EndDateAction <> 1)) Then
                URL = GetPageBuilderEditAnchor(Id, contentdata.LanguageId, contentdata.Quicklink)
            End If
        End If
        Return URL
    End Function
    Public Function GetEditAnchor(ByVal Id As Long, Optional ByVal Type As Integer = 1, Optional ByVal bFromApproval As Boolean = False, optional byval subType as ekenumeration.CMSContentSubtype = EkEnumeration.CMSContentSubtype.Content) As String
        Dim result As String = ""
        Dim SRC As String = ""
        Dim str, backStr As String
        If Type = 3333 Then
            Return GetCatalogEditAnchor(Id, Type, bFromApproval)
        ElseIf (Type = 1) Then
            If bFromApproval Then
                backStr = "back_file=approval.aspx"
            Else
                backStr = "back_file=content.aspx"
            End If
        Else
            backStr = "back_file=cmsform.aspx"
        End If
        str = Current.Request.QueryString("action")
        If (Len(str) > 0) Then
            backStr = backStr & "&back_action=" & str
        End If

        If bFromApproval Then
            str = Current.Request.QueryString("page")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_page=" & str
            End If
        End If

        If Not bFromApproval Then
            str = Current.Request.QueryString("folder_id")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_folder_id=" & str & "&folderid=" & str
            End If
        End If

        If (Type = 1) Then
            str = Current.Request.QueryString("id")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_id=" & str
            End If
        Else
            str = Current.Request.QueryString("form_id")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_form_id=" & str
            End If
        End If
        If (Not IsNothing(Current.Request.QueryString("callerpage"))) Then
            str = Current.Request.QueryString("callerpage")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_callerpage=" & str
            End If
        End If
        If (Not IsNothing(Current.Request.QueryString("origurl"))) Then
            str = Current.Request.QueryString("origurl")
            If (Len(str) > 0) Then
                backStr = backStr & "&back_origurl=" & HttpUtility.UrlEncode(str)
            End If
        End If
        str = ContentLanguage
        If (Len(str) > 0) Then
            backStr = backStr & "&back_LangType=" & str & "&rnd=" & CInt(Int((10 * Rnd()) + 1))
        End If

        SRC = "edit.aspx?close=false&LangType=" & ContentLanguage & "&id=" & Id & "&type=update&" & backStr
        If (bFromApproval) Then
            SRC &= "&pullapproval=true"
        End If
        If (subType = EkEnumeration.CMSContentSubtype.PageBuilderData) Then
            SRC &= "&menuItemType=editproperties"
        End If
        ' Fixed #15583 - using FF, new window opens up when editing the form
        result = GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/contentEdit.png", "#", m_refMsg.GetMessage("alt edit button text"), m_refMsg.GetMessage("btn edit"), "OnClick=""javascript:if(undefined != top.document.getElementById('ek_main')){top.document.getElementById('ek_main').src='" & SRC & "';return false;} else {location.href ='" & SRC & "';return false;}""" & ",'EDIT',790,580,1,1);return false;""")
        Return (result)
    End Function
    Public Function getCallBackupPage(ByVal defualt As String) As String
        Dim tmpStr As String = ""
        If (Current.Request.QueryString("callbackpage") <> "") Then
            tmpStr = Current.Request.QueryString("callbackpage")
            If (tmpStr = "cmsform.aspx") Then
                If (Current.Request.QueryString("fldid") <> "") Then
                    tmpStr = tmpStr & "?folder_id=" & Current.Request.QueryString("fldid") & "&"
                ElseIf (Current.Request.QueryString("folder_id") <> "") _
                  AndAlso Current.Request.QueryString("parm1") <> "folder_id" _
                  AndAlso Current.Request.QueryString("parm2") <> "folder_id" _
                  AndAlso Current.Request.QueryString("parm3") <> "folder_id" _
                  AndAlso Current.Request.QueryString("parm4") <> "folder_id" Then
                    tmpStr = tmpStr & "?folder_id=" & Current.Request.QueryString("folder_id") & "&"
                Else
                    tmpStr = tmpStr & "?"
                End If
            Else
                tmpStr = tmpStr & "?"
            End If
            tmpStr = tmpStr & Current.Request.QueryString("parm1") & "=" & Current.Request.QueryString("value1")

            If (Current.Request.QueryString("parm2") <> "") Then
                tmpStr = tmpStr & "&" & Current.Request.QueryString("parm2") & "=" & Current.Request.QueryString("value2")
                If (Current.Request.QueryString("parm3") <> "") Then
                    tmpStr = tmpStr & "&" & Current.Request.QueryString("parm3") & "=" & Current.Request.QueryString("value3")
                    If (Current.Request.QueryString("parm4") <> "") Then
                        tmpStr = tmpStr & "&" & Current.Request.QueryString("parm4") & "=" & Current.Request.QueryString("value4")
                    End If
                End If
            End If
            getCallBackupPage = tmpStr
        Else
            getCallBackupPage = defualt
        End If
    End Function
    'This function will pass pack the url paremeter so that they can be passed along
    Public Function BuildCallBackParms(ByVal leadingChar As String) As String
        Dim strTmp2 As String = ""
        If (Current.Request.QueryString("callbackpage") <> "") Then
            strTmp2 = "callbackpage=" & Current.Request.QueryString("callbackpage") & "&parm1=" & Current.Request.QueryString("parm1") & "&value1=" & Current.Request.QueryString("value1")
            If (Current.Request.QueryString("parm2") <> "") Then
                strTmp2 = strTmp2 & "&parm2=" & Current.Request.QueryString("parm2") & "&value2=" & Current.Request.QueryString("value2")
                If (Current.Request.QueryString("parm3") <> "") Then
                    strTmp2 = strTmp2 & "&parm3=" & Current.Request.QueryString("parm3") & "&value3=" & Current.Request.QueryString("value3")
                    If (Current.Request.QueryString("parm4") <> "") Then
                        strTmp2 = strTmp2 & "&parm4=" & Current.Request.QueryString("parm4") & "&value4=" & Current.Request.QueryString("value4")
                    End If
                End If
            End If
            strTmp2 = leadingChar & strTmp2
            BuildCallBackParms = strTmp2
        Else
            BuildCallBackParms = ""
        End If
    End Function

    Public Function getCallingpage(ByVal defualt As String) As String
        Dim tmp2 As String = ""
        If (Current.Request.QueryString("callbackpage") <> "") Then
            tmp2 = Current.Request.QueryString("callbackpage")
            getCallingpage = tmp2
        Else
            getCallingpage = defualt
        End If
    End Function

    Public Function GetHelpButton(ByVal myAlias As String, Optional ByVal alignString As String = "") As String
        Dim ret As New StringBuilder
        Dim linkString As String = ""
        Dim debugString As String = ""

        ret.Append("&nbsp;<a href=""#""><img ")
        ret.Append(" id=""DeskTopHelp"" title= """ & m_refMsg.GetMessage("alt help button text") & " """)
        If (alignString.Length) Then
            ret.Append(" align=""" & alignString & """ ")
        End If

        ret.Append(" src=""" & m_refAPI.AppPath & "images/UI/Icons/help.png"" ")
        ret.Append(" onclick=""PopUpWindow('")
        Try
            linkString = m_refAPI.fetchhelpLink(myAlias)
            ret.Append(linkString)
        Catch ex As Exception

        End Try
        ret.Append("', 'SitePreview', 600, 500, 1, 1);")

        If (m_refAPI.Debug_ShowHelpAlias()) Then
            debugString = "if (DebugMsg) DebugMsg('Alias = " & myAlias & ", Link = " & linkString & "');"
            debugString += " else alert('Alias = " & myAlias & ", Link = " & linkString & "');"
        End If
        ret.Append(debugString & "return false;"">")
        ret.Append("</a>&nbsp;")
        Return ret.ToString()
    End Function

    Public Function GetHelpAliasPrefix(ByVal folder_data As FolderData) As String
        Dim result As String = ""
        If (Not IsNothing(folder_data)) Then
            Select Case folder_data.FolderType
                Case Common.EkEnumeration.FolderType.Blog
                    result = "blog_"
                Case Common.EkEnumeration.FolderType.Content
                    result = ""
                Case Common.EkEnumeration.FolderType.DiscussionBoard
                    result = "discussionboard_"
                Case Common.EkEnumeration.FolderType.DiscussionForum
                    result = "discussionforum_"
                Case Common.EkEnumeration.FolderType.Domain
                    result = ""
                Case Common.EkEnumeration.FolderType.Root
                    result = ""
                Case Else
                    result = ""
            End Select
        End If
        GetHelpAliasPrefix = result
    End Function

    Public Function GetExportTranslationButton(ByVal hrefPath As String, Optional ByVal altText As String = "Export for translation", Optional ByVal HeaderText As String = "Export for translation") As String
        Dim result As New StringBuilder
        Try
            If (m_refAPI.IsARoleMember(EkEnumeration.CmsRoleIds.AdminXliff) AndAlso Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refAPI.RequestInformationRef, DataIO.LicenseManager.Feature.Xliff, False)) Then
                result.Append(GetButtonEventsWCaption(m_refAPI.AppPath & "images/UI/Icons/translation.png", _
                 hrefPath, altText, HeaderText, ""))
            End If
        Catch ex As Exception
            ' ignore
        End Try
        Return result.ToString
    End Function

    Public Function IsExportTranslationSupportedForContentType(ByVal ContentType As EkEnumeration.CMSContentType) As Boolean
        Return (ContentType = EkEnumeration.CMSContentType.Content Or ContentType = EkEnumeration.CMSContentType.Forms)
    End Function

#Region "Calendar Enhancement Functions added April, 2005 by Doug Glennon"
    Public Function ShowActiveLangsInList(ByVal showAllOpt As Boolean, ByVal bgColor As String, ByVal OnChangeEvt As String, ByVal SelLang As Integer, ByVal includeList As String) As String
        ' This function works like "ShowAllActiveLanguage" except it accepts a comma-seperated
        ' string value of Language IDs. Use this for showing a drop-down of languages that
        ' a piece of content, or other item, has been translated to.
        '
        ' For example, if your active languages are French, German and English and you have a
        ' piece of content that is translated in English and French already but not German, you would
        ' call ShowActiveLangsInList(showAllOpt, bgColor, OnChangeEvt, SelLang, "1033,1036"). This function
        ' will go to the DB, determine what 1033 and 1036 is and display them in a drop down, but would
        ' not show German.
        '
        Return ShowFilteredLangs(showAllOpt, bgColor, OnChangeEvt, SelLang, includeList, ExcludeLangsInList:=False)
    End Function
    Public Function ShowActiveLangsNotInList(ByVal showAllOpt As Boolean, ByVal bgColor As String, ByVal OnChangeEvt As String, ByVal SelLang As Integer, ByVal excludelist As String) As String
        ' This function works like ShowActiveLangsInList, however, it does the opposite. It
        ' shows all languages in the drop-down that are not in the excludelist. This way you
        ' can show a drop-down of all the languages for which a piece of content, or other item,
        ' has not yet been translated.
        '
        ' For example, if your active languages are French, German and English and you have a
        ' piece of content that is translated in English and French already but not German, you would
        ' call ShowActiveLangsNotInList(showAllOpt, bgColor, OnChangeEvt, SelLang, "1033,1036"). This function
        ' will go to the DB, get all the active languages and build a drop-down without 1033 and 1036 in it, thereby
        ' leaving only German (and "All" if available).
        '
        Return ShowFilteredLangs(showAllOpt, bgColor, OnChangeEvt, SelLang, excludelist, ExcludeLangsInList:=True)
    End Function

    Private Function ShowFilteredLangs(ByVal showAllOpt As Boolean, ByVal bgColor As String, ByVal OnChangeEvt As String, _
      ByVal SelLang As Integer, ByVal csvLangList As String, ByVal ExcludeLangsInList As Boolean) As String
        ' See "ShowActiveLangsInList" and "ShowActiveLangsNotInList"
        Dim sbResult As New StringBuilder
        Dim aryLangFilterList() As Integer
        Dim refSiteApi As New SiteAPI
        Dim nContentLanguage As Integer

        If (m_refAPI.EnableMultilingual = "1") Then
            nContentLanguage = SelLang
            If (CONTENT_LANGUAGES_UNDEFINED = nContentLanguage) Then
                nContentLanguage = refSiteApi.ContentLanguage
            End If

            If (OnChangeEvt = "") Then
                OnChangeEvt = "SelLanguage(this.value)"
            End If

            Dim aryLangList() As String
            aryLangList = csvLangList.Split(",")
            If aryLangList.Length > 0 Then
                ReDim aryLangFilterList(aryLangList.Length - 1)
                For iLang As Integer = 0 To aryLangList.Length - 1
                    Dim strLang As String = aryLangList(iLang).Trim
                    If (IsNumeric(strLang)) Then
                        aryLangFilterList(iLang) = Convert.ToInt32(strLang)
                    Else
                        aryLangFilterList(iLang) = CONTENT_LANGUAGES_UNDEFINED
                    End If
                Next
            Else
                If (ExcludeLangsInList) Then
                    Erase aryLangFilterList
                Else
                    ReDim aryLangFilterList(0)
                    aryLangFilterList(0) = nContentLanguage
                End If
            End If

            sbResult.Append("<select id=""frm_langID"" name=""frm_langID"" OnChange=""" & OnChangeEvt & """>" & vbCrLf)

            If (showAllOpt) Then
                If (ALL_CONTENT_LANGUAGES = nContentLanguage) Then
                    sbResult.Append("<option value=""" & ALL_CONTENT_LANGUAGES & """ selected=""selected"">All</option>")
                Else
                    sbResult.Append("<option value=""" & ALL_CONTENT_LANGUAGES & """>All</option>")
                End If
            End If

            If (ExcludeLangsInList) Then
                sbResult.Append("<option value=""0"">Select a Language</option>")
            End If

            Dim aryLangData As LanguageData()
            aryLangData = refSiteApi.GetAllActiveLanguages()

            For iLang As Integer = 0 To aryLangData.Length - 1
                With aryLangData(iLang)
                    If (.Id <> CONTENT_LANGUAGES_UNDEFINED) Then
                        If (IsInList(.Id, aryLangFilterList) Xor ExcludeLangsInList) Then
                            sbResult.Append("<option value=""" & .Id & """")
                            If (.Id = nContentLanguage) Then
                                sbResult.Append(" selected=""selected""")
                            Else
                            End If
                            sbResult.Append(">")
                            sbResult.Append(.LocalName)
                            sbResult.Append("</option>")
                        End If
                    End If
                End With
            Next
            sbResult.Append("</select>")
        End If

        Return sbResult.ToString
    End Function

    Private Function IsInList(ByVal Value As Integer, ByVal ValueList() As Integer) As Boolean
        If (IsNothing(ValueList)) Then Return False
        For iItem As Integer = 0 To ValueList.Length - 1
            If (Value = ValueList(iItem)) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function enabledIcon(ByVal inEnabled As Boolean) As String
        Dim strRet As String

        If (inEnabled) Then
            strRet = "<img src=""" & m_refAPI.AppPath & "images/UI/Icons/check.png"" alt=""Enabled"" title=""Enabled"">"
        Else
            strRet = "<img src=""" & m_refAPI.AppImgPath & "icon_redx.gif"" alt=""Disabled"" title=""Disabled"">"
        End If

        Return (strRet)
    End Function
    Public Function inputTag(ByVal tagName As String, ByVal tagValue As String, Optional ByVal tagSize As Integer = 25) As String
        Return ("<input type=""text"" name=""" & tagName & """ value=""" & tagValue & """ size=""" & tagSize & """/>")
    End Function
    Public Function inputTagHidden(ByVal tagName As String, ByVal tagValue As String) As String
        Return ("<input type=""hidden"" name=""" & tagName & """ value=""" & tagValue & """/>")
    End Function
    Public Function inputTagCheckbox(ByVal tagName As String, ByVal isChecked As Boolean, ByVal checkedValue As String) As String
        If (isChecked) Then
            Return ("<input type=""checkbox"" name=""" & tagName & """ value=""" & checkedValue & """ checked=""true""/>")
        Else
            Return ("<input type=""checkbox"" name=""" & tagName & """ value=""" & checkedValue & """/>")
        End If
    End Function

#End Region

#Region "Catalog"
    Public Function GetCatalogAddAnchor(ByVal Id As Long) As String
        Dim sResult As String = ""
        Try
            sResult = "AddNewContent('LangType=" & ContentLanguage & _
              "&type=add&createtask=1&id=" & Id & "&" & _
              GetBackParams()
            sResult &= "&AllowHTML=1"
            sResult &= "', 3333);"
        Catch ex As Exception
            sResult = ""
        End Try
        Return sResult
    End Function

    Public Function GetCatalogAddAnchorType(ByVal Id As Long, ByVal xml_id As Long) As String
        Dim sResult As String = "AddNewContent('LangType=" & ContentLanguage & _
                "&type=add&createtask=1&id=" & Id & "&xid=" & xml_id & "&" & _
                 GetBackParams() & "',3333); "
        Return sResult
    End Function
#End Region

End Class
