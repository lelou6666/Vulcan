Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports System.Web
Imports System.Web.UI.WebControls
Imports Microsoft.VisualBasic

Namespace Ektron.Cms.Workarea
    Public Class workareabase
        Inherits System.Web.UI.Page

        Protected m_refContentApi As New ContentAPI
        Protected m_refCommunityGroupApi As New Community.CommunityGroupAPI
        Protected m_refTagsApi As New Community.TagsAPI
        Protected m_refFriendsApi As New Community.FriendsAPI
        Protected m_refFavoritesApi As New Community.FavoritesAPI
        Protected m_refStyle As New StyleHelper
        Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper = m_refContentApi.EkMsgRef
        Private TitleBar As String = String.Empty
        Private Toolbar As String = String.Empty
        Protected ContentLanguage As Integer = -1
        Private m_wamTabs As New workareatabs(m_refMsg, AppImgPath)
        Private m_wajs As New workareajavascript()
        Private m_waCommerce As New workareaCommerce(m_refContentApi.RequestInformationRef.CommerceSettings, Me)
        Private m_waaAjax As New workareaajax(m_refContentApi.AppPath)
        Private m_wawWizard As New workareawizard
        Protected m_sPageAction As String = ""
        Protected m_iID As Long = 0
        Protected m_bAjaxTree As Boolean = True
        Protected m_bIsWindows As Boolean = True
        Private aButtons As New ArrayList
        Private noticeContainer As String = ""
        Protected m_cTR As TableRow = Nothing
        Protected m_checkVariable As String = ""

        Private _Version8TabsImplemented As Boolean = False
        Public Property Version8TabsImplemented() As Boolean
            Get
                Return _Version8TabsImplemented
            End Get
            Set(ByVal Value As Boolean)
                _Version8TabsImplemented = Value
            End Set
        End Property

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (m_refContentApi.RequestInformationRef.UserId = 0) Then
                Response.Redirect(m_refContentApi.ApplicationPath & "blank.htm")
            End If
            If Request.ServerVariables("HTTP_USER_AGENT").ToLower().IndexOf("windows") = -1 Then
                m_bIsWindows = False
            End If
            If (m_refContentApi.TreeModel = 1) Then
                m_bAjaxTree = True
            End If
            If (Not (Request.QueryString("LangType") Is Nothing)) Then
                If (Request.QueryString("LangType") <> "") Then
                    ContentLanguage = GetQueryLanguage()
                    m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
                Else
                    If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                        ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
                    End If
                End If
            Else
                If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If

            If ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
                m_refContentApi.ContentLanguage = ALL_CONTENT_LANGUAGES
            Else
                m_refContentApi.ContentLanguage = ContentLanguage
            End If
            If Request.QueryString("action") <> "" Then
                Me.m_sPageAction = Request.QueryString("action").ToLower()
            Else
                Me.m_sPageAction = ""
            End If
            If Request.QueryString("id") <> "" Then
                Long.TryParse(Request.QueryString("id"), m_iID)
            End If
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            'cache control
            Response.CacheControl = "no-cache"
            Response.AddHeader("Pragma", "no-cache")
            Response.Expires = -1

            'register js and css
            Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS)
            Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS)
            Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "wamenu/includes/com.ektron.ui.menu.js", "EktronComUIMenuJS")
            Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS)
            Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/workareahelper.js", "EktronWorkareaHelperJS")

            Me.ClientScript.RegisterClientScriptBlock(Me.GetType(), "base js", (New StyleHelper).GetClientScript)

            Ektron.Cms.API.Css.RegisterCss(Me, m_refContentApi.AppPath & "wamenu/css/com.ektron.ui.menu.css", "EktronComUIMenuCSS")
        End Sub

        Public Function GetMessage(ByVal MessageTitle As String) As String
            Return m_refMsg.GetMessage(MessageTitle)
        End Function

        Public ReadOnly Property RequestInformationRef() As Common.EkRequestInformation
            Get
                Return m_refContentApi.RequestInformationRef
            End Get
        End Property

        Public ReadOnly Property ContentAPIRef() As ContentAPI
            Get
                Return m_refContentApi
            End Get
        End Property

        Public ReadOnly Property AppImgPath() As String
            Get
                Return m_refContentApi.RequestInformationRef.AppImgPath
            End Get
        End Property

        Public Property MenuCheckVariable() As String
            Get
                Return m_checkVariable
            End Get
            Set(ByVal value As String)
                m_checkVariable = value
            End Set
        End Property

        Public Sub SetTitleBarToMessage(ByVal MessageTitle As String)
            TitleBar = m_refStyle.GetTitleBar(m_refMsg.GetMessage(MessageTitle))
        End Sub

        Public Sub SetTitleBarToString(ByVal MessageTitle As String)
            TitleBar = m_refStyle.GetTitleBar(MessageTitle)
        End Sub

        Public Function FormatCurrency(ByVal Price As Decimal, Optional ByVal CurrencyCharacter As String = "") As String
            If CurrencyCharacter = "" Then
                Return FormatNumber(Price, 2)
            Else
                Return Common.EkFunctions.FormatCurrency(Price, Me.m_refContentApi.RequestInformationRef.CommerceSettings.CurrencyCultureCode)
            End If
        End Function

        Public Sub AddButton(ByVal ImageFile As String, ByVal hrefPath As String, ByVal altText As String, ByVal HeaderText As String, ByVal SpecialEvents As String)
            aButtons.Add(m_refStyle.GetButtonEventsWCaption(ImageFile, hrefPath, altText, HeaderText, SpecialEvents))
        End Sub
        Public Sub AddButtonwithMessages(ByVal ImageFile As String, ByVal hrefPath As String, ByVal altTextMessage As String, ByVal HeaderTextMessage As String, ByVal SpecialEvents As String)
            aButtons.Add(m_refStyle.GetButtonEventsWCaption(ImageFile, hrefPath, GetMessage(altTextMessage), GetMessage(HeaderTextMessage), SpecialEvents))
        End Sub
        Public Sub AddButtonwithMessages(ByVal ImageFile As String, ByVal hrefPath As String, ByVal altTextMessage As String, ByVal HeaderTextMessage As String, ByVal SpecialEvents As String, ByVal InsertAt As Integer)
            aButtons.Insert(InsertAt, m_refStyle.GetButtonEventsWCaption(ImageFile, hrefPath, GetMessage(altTextMessage), GetMessage(HeaderTextMessage), SpecialEvents))
        End Sub
        Public Sub AddBreak()
            aButtons.Add("<td>&nbsp;|&nbsp;</td>")
        End Sub
        Public Sub AddMenuButtonLink(ByVal ImageFile As String, ByVal TextMessage As String, ByVal hrefPath As String)
            AddMenuButton(ImageFile, TextMessage, "window.location.href='" & hrefPath & "'")
        End Sub
        Public Sub AddMenuButton(ByVal ImageFile As String, ByVal TextMessage As String, ByVal SpecialEvents As String)
            aButtons.Add("<td class=""menuRootItem"" onclick=""" & SpecialEvents & """ onmouseover=""this.className='menuRootItemSelected';"" onmouseout=""this.className='menuRootItem';""><span style=""background-image: url('" & ImageFile & "')"">" & TextMessage & "</span></td>")
        End Sub

        Public Sub AddHelpButton(ByVal MessageTitle As String)
            aButtons.Add("<td>" & m_refStyle.GetHelpButton(MessageTitle) & "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>")
        End Sub
        Public Sub AddSearchBox(ByVal SearchText As String, ByVal DropDownItems As ListItemCollection, ByVal SearchJSFunction As String)
            Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
            Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS)
            Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareaSearchBoxInputLabelInitJS")
            Dim sbSearch As New StringBuilder()

            'sbSearch.Append("<td class=""searchSeparator""></td>")
            sbSearch.Append("<td class=""label"">&nbsp;|&nbsp;")
            sbSearch.Append("<label for=""txtSearch"">" & GetMessage("generic search") & "</label>")
            sbSearch.Append("</td>")
            sbSearch.Append("<td>")
            sbSearch.Append("<input type=""text"" size=""25"" class=""ektronTextMedium"" id=""txtSearch"" name=""txtSearch"" value=""" & Server.HtmlEncode(SearchText) & """ onkeydown=""CheckForReturn(event)"" autocomplete=""off"">")
            If DropDownItems.Count > 0 Then
                sbSearch.Append("<select id=""searchlist"" name=""searchlist"">")
                For i As Integer = 0 To (DropDownItems.Count - 1)
                    sbSearch.Append("<option value=""").Append(Server.HtmlEncode(DropDownItems(i).Value)).Append("""").Append(IIf(DropDownItems(i).Selected, " selected ", "")).Append(">").Append(Server.HtmlEncode(DropDownItems(i).Text)).Append("</option>")
                Next
                sbSearch.Append("</select>")
            End If
            sbSearch.Append("</td>")
            sbSearch.Append("<td>")
            sbSearch.Append("<input type=button value=""" & GetMessage("generic search") & """ title=""" & GetMessage("generic search") & """ id=""btnSearch"" name=""btnSearch"" class=""ektronWorkareaSearch"" onclick=""").Append(SearchJSFunction).Append("();"" />")

            sbSearch.Append("    <script type=""text/javascript"">").Append(Environment.NewLine)
            sbSearch.Append(" function CheckForReturn(e) ").Append(Environment.NewLine)
            sbSearch.Append(" {  ").Append(Environment.NewLine)
            sbSearch.Append("   var keynum; ").Append(Environment.NewLine)
            sbSearch.Append("   var keychar; ").Append(Environment.NewLine)
            sbSearch.Append("   if(window.event) // IE ").Append(Environment.NewLine)
            sbSearch.Append("   { ").Append(Environment.NewLine)
            sbSearch.Append("       keynum = e.keyCode ").Append(Environment.NewLine)
            sbSearch.Append("   } ").Append(Environment.NewLine)
            sbSearch.Append("   else if(e.which) // Netscape/Firefox/Opera ").Append(Environment.NewLine)
            sbSearch.Append("   { ").Append(Environment.NewLine)
            sbSearch.Append("       keynum = e.which ").Append(Environment.NewLine)
            sbSearch.Append("   } ").Append(Environment.NewLine)
            sbSearch.Append("   if( keynum == 13 ) { ").Append(Environment.NewLine)
            sbSearch.Append("       document.getElementById('btnSearch').focus(); ").Append(Environment.NewLine)
            sbSearch.Append("   } ").Append(Environment.NewLine)
            sbSearch.Append(" } ").Append(Environment.NewLine)
            sbSearch.Append("</script>")
            sbSearch.Append("</td>")

            aButtons.Add(sbSearch.ToString())
        End Sub

        Public Function eWebWPEditor(ByVal FieldName As String, ByVal Width As String, ByVal Height As String, ByVal ContentHtml As String, ByVal onsubmit As String) As String
            Dim sbWP As New StringBuilder()
            sbWP.Append("<script type=""text/javascript"">" & Environment.NewLine)
            sbWP.Append("var eWebWPPath = """ & m_refContentApi.AppPath & "ewebwp/"";" & Environment.NewLine)
            sbWP.Append("</script>" & Environment.NewLine)
            sbWP.Append("<script type=""text/javascript"" src=""" & m_refContentApi.AppPath & "ewebwp/ewebwp.js""></script>" & Environment.NewLine)
            sbWP.Append("<input type=""hidden"" name=""" & FieldName & """ value=""" & Server.HtmlEncode(ContentHtml) & """>")
            sbWP.Append("<script language=""javaScript1.2"">" & Environment.NewLine)
            sbWP.Append("<!--" & Environment.NewLine)
            If TypeName(Width) = "String" Then
                Width = """" & Width & """"
            End If
            If TypeName(Height) = "String" Then
                Height = """" & Height & """"
            End If
            If Trim(onsubmit).Length > 0 Then
                sbWP.Append("function " & FieldName & "_onsubmit() { " & Environment.NewLine)
                sbWP.Append(onsubmit).Append(Environment.NewLine)
                sbWP.Append("}" & Environment.NewLine)
            Else
                sbWP.Append("function " & FieldName & "_onsubmit(){ return true; }" & Environment.NewLine)
            End If
            sbWP.Append("eWebWP.parameters.removeFont = true;" & Environment.NewLine)
            sbWP.Append("eWebWP.create(""" & FieldName & """, " & Width & ", " & Height & ");" & Environment.NewLine)
            sbWP.Append("//-->" & Environment.NewLine)
            sbWP.Append("</script>")
            Return sbWP.ToString()
        End Function

        Public Sub AddBackButton(ByVal ReturnPath As String)
            aButtons.Add(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", ReturnPath, m_refMsg.GetMessage("alt back button"), m_refMsg.GetMessage("btn back"), ""))
        End Sub

        Public Sub AddButtonText(ByVal ButtonText As String)
            aButtons.Add(ButtonText)
        End Sub

        Public Sub AddMenu(ByVal waMenu As workareamenu)
            aButtons.Add(waMenu.Render(Me.m_checkVariable))
        End Sub

        Public Sub AddNotice(ByVal noticeText As String)

            noticeContainer = noticeText

        End Sub


        Public Property Tabs() As workareatabs
            Get
                Return Me.m_wamTabs
            End Get
            Set(ByVal value As workareatabs)
                Me.m_wamTabs = value
            End Set
        End Property

        Public Property JSLibrary() As workareajavascript
            Get
                Return Me.m_wajs
            End Get
            Set(ByVal value As workareajavascript)
                Me.m_wajs = value
            End Set
        End Property

        Public Property CommerceLibrary() As workareaCommerce
            Get
                Return Me.m_waCommerce
            End Get
            Set(ByVal value As workareaCommerce)
                Me.m_waCommerce = value
            End Set
        End Property

        Public Property AJAX() As workareaajax
            Get
                Return Me.m_waaAjax
            End Get
            Set(ByVal value As workareaajax)
                Me.m_waaAjax = value
            End Set
        End Property

        Public Property Wizard() As workareawizard
            Get
                Return Me.m_wawWizard
            End Get
            Set(ByVal value As workareawizard)
                Me.m_wawWizard = value
            End Set
        End Property

        Public Sub AddLanguageDropdown(Optional ByVal ShowAll As Boolean = True)
            Dim result As New System.Text.StringBuilder
            If m_refContentApi.EnableMultilingual = 1 Then
                Dim m_refsite As New SiteAPI
                Dim language_data() As LanguageData = m_refsite.GetAllActiveLanguages

                result.Append("<td>&nbsp;|&nbsp;</td>")
                result.Append("<td class=""label"">View:</td>")
                result.Append("<td>")
                result.Append("<select id=selLang name=selLang OnChange=""javascript:LoadLanguage('frmContent');"">")
                If ShowAll Then
                    result.Append("<option value=""" & ALL_CONTENT_LANGUAGES & """")
                    If (ContentLanguage = ALL_CONTENT_LANGUAGES) Then
                        result.Append(" selected=""selected""")
                    End If
                    result.Append(">")
                    result.Append("All")
                    result.Append("</option>")
                End If
                For count As Integer = 0 To language_data.Length - 1
                    With language_data(count)
                        result.Append("<option value=""" & .Id & """")
                        If (.Id = ContentLanguage) Then
                            result.Append(" selected=""selected""")
                        Else
                        End If
                        result.Append(">")
                        result.Append(.LocalName)
                        result.Append("</option>")
                    End With
                Next
                result.Append("</select></td>")
            End If
            result.Append("<td>")
            aButtons.Add(result.ToString())
        End Sub
        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            Dim tHeadertable As New Table
            Dim tHeaderrow As New TableRow
            Dim tHeadercell As New TableCell
            Dim tOutertable As New Table
            Dim tOuterrow As New TableRow
            Dim tOutercell As New TableCell
            '    'Get a reference to the page
            Dim form As System.Web.UI.HtmlControls.HtmlForm = Page.Form
            '    'inner
            tHeadertable.CellPadding = 0
            tHeadertable.CellSpacing = 0
            tHeadertable.Width = Unit.Percentage(100)

            tHeadercell.CssClass = "ektronTitlebar"
            tHeadercell.ID = "txtTitleBar"
            tHeadercell.Text = TitleBar
            tHeaderrow.Controls.Add(tHeadercell)
            tHeadertable.Controls.Add(tHeaderrow)

            tHeaderrow = New TableRow
            tHeadercell = New TableCell

            tHeadercell = New TableCell
            tHeadercell.CssClass = "ektronToolbar"
            tHeadercell.ID = "htmToolBar"
            tHeadercell.Text = GetToolBar()

            tHeaderrow.Controls.Add(tHeadercell)
            tHeadertable.Controls.Add(tHeaderrow)

            'outer
            tOutertable.CellPadding = 0
            tOutertable.CellSpacing = 0
            tOutertable.Width = Unit.Percentage(100)

            tOutercell.Controls.Add(tHeadertable)
            tOuterrow.Controls.Add(tOutercell)
            tOutertable.Controls.Add(tOuterrow)
            tOutertable.CssClass = "baseClassToolbar"

            If m_cTR IsNot Nothing AndAlso m_cTR.GetType() Is GetType(TableRow) Then
                tOutertable.Controls.Add(m_cTR)
            End If

            If Me.m_wamTabs.TabsOn And Me.Version8TabsImplemented = False Then
                tOutercell = New TableCell
                tOuterrow = New TableRow
                tOutercell.Text = Me.m_wamTabs.RenderTabs
                tOuterrow.Controls.Add(tOutercell)
                tOutertable.Controls.Add(tOuterrow)
            End If

            Dim divWrapper As New Panel
            divWrapper.CssClass = "ektronPageHeader"
            divWrapper.Controls.Add(tOutertable)
            form.Controls.AddAt(0, divWrapper)

            '    'Render as usual
            MyBase.Render(writer)
        End Sub

        Public Sub AddTableRow(ByVal tr As TableRow)
            m_cTR = tr
        End Sub

        Private Function GetToolBar() As String
            Dim sbButtons As New System.Text.StringBuilder
            sbButtons.Append("<table cellspacing=""0""><tr>")
            For i As Integer = 0 To (aButtons.Count - 1)
                sbButtons.Append(aButtons(i))
            Next
            sbButtons.Append("</tr></table>")
            If noticeContainer <> "" Then
                sbButtons.Append("<div class=""noticeContainer"">")
                sbButtons.Append(noticeContainer)
                sbButtons.Append("</div>")
            End If
            Return sbButtons.ToString()
        End Function

        Private Function RenderTreeJS() As String
            Dim sbTreeJS As New StringBuilder()
            sbTreeJS.Append("    <script language=""javascript"" type=""text/javascript"">").Append(Environment.NewLine)
            sbTreeJS.Append(" // begin tree JS ").Append(Environment.NewLine)
            If Me.m_bAjaxTree = True Then
                sbTreeJS.Append("			if(typeof(top[""ek_nav_bottom""])!= 'undefined'){").Append(Environment.NewLine)
                sbTreeJS.Append("				if(typeof(top[""ek_nav_bottom""][""NavIframeContainer""])!= 'undefined'){").Append(Environment.NewLine)
                sbTreeJS.Append("					if(typeof(top[""ek_nav_bottom""][""NavIframeContainer""][""nav_folder_area""])!= 'undefined'){").Append(Environment.NewLine)
                sbTreeJS.Append("						if(typeof(top[""ek_nav_bottom""][""NavIframeContainer""][""nav_folder_area""][""ContentTree""])!= 'undefined'){").Append(Environment.NewLine)
                sbTreeJS.Append("							var treeobj=top[""ek_nav_bottom""][""NavIframeContainer""][""nav_folder_area""][""ContentTree""];").Append(Environment.NewLine)
                sbTreeJS.Append("							if(treeobj.document.getElementById(""selected_folder_id"")!=null){").Append(Environment.NewLine)
                sbTreeJS.Append("								var SelectedTreeId=treeobj.document.getElementById(""selected_folder_id"").value;").Append(Environment.NewLine)
                sbTreeJS.Append("								var CurrentFolderId=""" & Request.QueryString("id") & """;").Append(Environment.NewLine)
                sbTreeJS.Append("								if(CurrentFolderId==0 && SelectedTreeId!=0) {").Append(Environment.NewLine)
                sbTreeJS.Append("									var stylenode = treeobj.document.getElementById( SelectedTreeId );").Append(Environment.NewLine)
                sbTreeJS.Append("									if(stylenode!=null){").Append(Environment.NewLine)
                sbTreeJS.Append("										stylenode.style[""background""] = ""#ffffff"";").Append(Environment.NewLine)
                sbTreeJS.Append("										stylenode.style[""color""] = ""#000000"";").Append(Environment.NewLine)
                sbTreeJS.Append("										var stylenode = treeobj.document.getElementById( 0/*CurrentFolderId*/ );").Append(Environment.NewLine)
                sbTreeJS.Append("										stylenode.style[""background""] = ""#3366CC"";").Append(Environment.NewLine)
                sbTreeJS.Append("										stylenode.style[""color""] = ""#ffffff"";").Append(Environment.NewLine)
                sbTreeJS.Append("									}").Append(Environment.NewLine)
                sbTreeJS.Append("								}").Append(Environment.NewLine)
                sbTreeJS.Append("							}").Append(Environment.NewLine)
                sbTreeJS.Append("						}").Append(Environment.NewLine)
                sbTreeJS.Append("					}").Append(Environment.NewLine)
                sbTreeJS.Append("				}").Append(Environment.NewLine)
                sbTreeJS.Append("			}").Append(Environment.NewLine)
                sbTreeJS.Append("			function reloadFolder(pid){").Append(Environment.NewLine)
                sbTreeJS.Append("				alert('reloadtree');").Append(Environment.NewLine)
                sbTreeJS.Append("				reloadTreeByName(pid,""ContentTree"");").Append(Environment.NewLine)
                sbTreeJS.Append("				reloadTreeByName(pid,""FormsTree"");").Append(Environment.NewLine)
                sbTreeJS.Append("				reloadTreeByName(pid,""LibraryTree"");").Append(Environment.NewLine)
                sbTreeJS.Append("			}").Append(Environment.NewLine)
                sbTreeJS.Append("			function reloadTreeByName(pid,TreeName){").Append(Environment.NewLine)
                sbTreeJS.Append("				alert('reload ' + pid + ' ' + TreeName);").Append(Environment.NewLine)
                sbTreeJS.Append("				var obj=top[""ek_nav_bottom""][""NavIframeContainer""][""nav_folder_area""][TreeName];").Append(Environment.NewLine)
                sbTreeJS.Append("				if(obj!=null){").Append(Environment.NewLine)
                sbTreeJS.Append("				    var node = obj.document.getElementById( ""T"" + pid );").Append(Environment.NewLine)
                sbTreeJS.Append("				    if(node!=null){").Append(Environment.NewLine)
                sbTreeJS.Append("					    for (var i=0;i<node.childNodes.length;i++){").Append(Environment.NewLine)
                sbTreeJS.Append("						    if(node.childNodes(i).nodeName=='LI' || node.childNodes(i).nodeName=='UL'){").Append(Environment.NewLine)
                sbTreeJS.Append("				alert('1');").Append(Environment.NewLine)
                sbTreeJS.Append("							    var parent = node.childNodes(i).parentElement;").Append(Environment.NewLine)
                sbTreeJS.Append("							    parent.removeChild( node.childNodes(i));").Append(Environment.NewLine)
                sbTreeJS.Append("						    }").Append(Environment.NewLine)
                sbTreeJS.Append("					    }").Append(Environment.NewLine)
                sbTreeJS.Append("					    obj.TREES[""T"" + pid].children = [];").Append(Environment.NewLine)
                sbTreeJS.Append("					    obj.onToggleClick(pid,obj.callback_function,pid);").Append(Environment.NewLine)
                sbTreeJS.Append("				    }").Append(Environment.NewLine)
                sbTreeJS.Append("				} ").Append(Environment.NewLine)
                sbTreeJS.Append("			}").Append(Environment.NewLine)
            End If
            sbTreeJS.Append(" // end tree JS ").Append(Environment.NewLine)
            sbTreeJS.Append("    </script>").Append(Environment.NewLine)
            Return sbTreeJS.ToString()
        End Function

        Private Function RenderCSS() As String
            Dim sbCSS As New System.Text.StringBuilder()
            sbCSS.Append("<script type=""text/javascript"" src=""" & m_refContentApi.AppPath & "java/jfunct.js""></script>" & Environment.NewLine)
            sbCSS.Append("<script type=""text/javascript"" src=""" & m_refContentApi.AppPath & "java/toolbar_roll.js""></script>" & Environment.NewLine)
            sbCSS.Append("<link rel='stylesheet' type='text/css' href='" & m_refContentApi.AppPath & "explorer/css/com.ektron.ui.menu.css' />" & Environment.NewLine)
            sbCSS.Append("<script language=""javascript"" src='" & m_refContentApi.AppPath & "explorer/includes/com.ektron.ui.menu.js' type=""text/javascript""></script>" & Environment.NewLine)
            sbCSS.Append(Environment.NewLine)
            Return sbCSS.ToString()
        End Function

        Public Function ReloadClientScript() As String
            Dim result As New System.Text.StringBuilder
            Dim pid As Long = 0
            Try
                If (Not (Request.QueryString("id") Is Nothing)) Then
                    pid = Request.QueryString("id")
                ElseIf (Not (Request.QueryString("id") Is Nothing)) Then
                    pid = Request.QueryString("folder_id")
                End If
                result.Append("<script language=""javascript"">" & vbCrLf)
                If (m_refContentApi.TreeModel = 1 And pid <> 0) Then
                    If ((Not (Request.QueryString("TreeUpdated") Is Nothing)) AndAlso (Request.QueryString("TreeUpdated") = 1)) Then
                        pid = m_refContentApi.GetParentIdByFolderId(pid)
                        If (pid = -1) Then
                            result.Length = 0
                            Exit Try
                        End If
                    End If

                    result.Append("reloadFolder(" + Convert.ToString(pid) + ");" & vbCrLf)
                Else
                    result.Append("<!--" & vbCrLf)
                    result.Append("	// If reloadtrees paramter exists, reload selected navigation trees:" & vbCrLf)
                    result.Append("	var m_reloadTrees = """ & Request.QueryString("reloadtrees") & """;" & vbCrLf)
                    result.Append("	top.ReloadTrees(m_reloadTrees);" & vbCrLf)
                    result.Append("	self.location.href=""" & Request.ServerVariables("path_info") & "?" & Replace(Request.ServerVariables("query_string"), "&reloadtrees=" & Request.QueryString("reloadtrees"), "") & """;" & vbCrLf)
                    result.Append("	// If TreeNav parameters exist, ensure the desired folders are opened:" & vbCrLf)
                    result.Append("	var strTreeNav = """ & Request.QueryString("TreeNav") & """;" & vbCrLf)
                    result.Append("	if (strTreeNav != null) {" & vbCrLf)
                    result.Append("		strTreeNav = strTreeNav.replace(/\\\\/g,""\\"");" & vbCrLf)
                    result.Append("		top.TreeNavigation(""ContentTree"", strTreeNav);" & vbCrLf)
                    result.Append("	}" & vbCrLf)
                    result.Append("//-->" & vbCrLf)
                End If
                result.Append("</script>" & vbCrLf)
            Catch ex As Exception
            End Try
            Return (result.ToString)
        End Function

        Protected Function GetQueryLanguage() As Integer
            Dim result As Integer = ContentLanguage
            Dim tempVal As Integer = 0
            Try
                If (Not IsNothing(Request.QueryString("LangType")) AndAlso (Request.QueryString("LangType").Length > 0)) Then
                    If (Integer.TryParse(Request.QueryString("LangType").Replace("#", ""), tempVal)) Then
                        result = tempVal
                    End If
                End If
            Catch ex As Exception
            End Try
            Return result
        End Function

        Public ReadOnly Property IsLoggedIn() As Boolean
            Get
                Return m_refContentApi.IsLoggedIn
            End Get
        End Property

        Public ReadOnly Property IsAdmin() As Boolean
            Get
                Return IsLoggedIn AndAlso m_refContentApi.IsAdmin()
            End Get
        End Property

        Public ReadOnly Property IsCommerceAdmin() As Boolean
            Get
                Return IsLoggedIn AndAlso (IsAdmin OrElse m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))
            End Get
        End Property

    End Class

    Public Class workareamenu

        Protected m_sName As String = ""
        Protected m_sLabel As String = ""
        Protected m_sImagePath As String = ""
        Protected m_lItems As System.Collections.Generic.List(Of String) = New System.Collections.Generic.List(Of String)

        Public Sub New(ByVal MenuName As String, ByVal Label As String, ByVal ImagePath As String)
            Me.m_sName = MenuName
            Me.m_sLabel = Label
            Me.m_sImagePath = ImagePath
        End Sub
        Public Sub AddItem(ByVal ImagePath As String, ByVal Label As String, ByVal JScode As String)
            m_lItems.Add(m_sName & "menu.addItem(""&nbsp;<img valign='center' src='" & ImagePath & "' />&nbsp;&nbsp;" & Label & """, function() { " & JScode & " } ); ")
        End Sub
        Public Sub AddLinkItem(ByVal ImagePath As String, ByVal Label As String, ByVal URL As String)
            Me.AddItem(ImagePath, Label, "window.location.href = '" & URL & "';")
        End Sub
        Public Sub AddHREFLinkItem(ByVal ImagePath As String, ByVal Label As String, ByVal URL As String)
            m_lItems.Add(m_sName & "menu.addItem(""&nbsp;<img valign='center' src='" & ImagePath & "' />&nbsp;&nbsp;<a href='" & URL.Replace("""", "\""") & "'>" & Label & "</a>"", function() { return false; } ); ")
        End Sub
        Public Sub AddBreak()
            m_lItems.Add(m_sName & "menu.addBreak();")
        End Sub
        Public Function Render() As String
            Return Render("")
        End Function
        Public Function Render(ByVal checkvariable As String) As String

            Dim result As New StringBuilder()

            If m_lItems.Count > 0 Then

                result.Append("<script language=""javascript""> ")
                result.Append("var " & m_sName & "menu = new Menu( """ & m_sName & """ ); ")

                For i As Integer = 0 To (m_lItems.Count - 1)

                    result.Append(m_lItems.Item(i).ToString())

                Next

                result.Append("MenuUtil.add( " & m_sName & "menu ); ")
                result.Append("</script>" & Environment.NewLine)

                If checkvariable <> "" Then

                    result.Append("<td class=""menuRootItem"" onclick=""if (" & checkvariable & ") { MenuUtil.use(event, '" & m_sName & "'); } else { return false; }"" onmouseover=""if (" & checkvariable & ") { this.className='menuRootItemSelected';MenuUtil.use(event, '" & m_sName & "'); } else { return false; }"" onmouseout=""if (" & checkvariable & ") { this.className='menuRootItem';} else { return false; }""><span style=""background-image: url('" & Me.m_sImagePath & "')"">" & m_sLabel & "</span></td>")

                Else

                    result.Append("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, '" & m_sName & "');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, '" & m_sName & "');"" onmouseout=""this.className='menuRootItem'""><span style=""background-image: url('" & Me.m_sImagePath & "')"">" & m_sLabel & "</span></td>")

                End If

            End If

            Return result.ToString()

        End Function

    End Class

    Public Class workareaCommerce

        Protected m_CommerceSettings As Common.CommerceSettings = Nothing
        Protected m_WorkAreaBase As Workarea.workareabase = Nothing

        Public Sub New(ByVal eCommerceSettings As Common.CommerceSettings, ByVal workareaRef As Workarea.workareabase)

            Me.m_CommerceSettings = eCommerceSettings
            m_WorkAreaBase = workareaRef

        End Sub

        Public Enum ModeType
            Add
            Edit
            View
        End Enum

        Public Sub CheckCommerceAdminAccess()

            If Not m_WorkAreaBase.ContentAPIRef.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin) Then
                Throw New Exception(m_WorkAreaBase.GetMessage("err not role commerce-admin"))
            End If

        End Sub

        Public Function GetProductImage(ByVal entryType As Common.EkEnumeration.CatalogEntryType) As String

            Return Utilities.GetProductImage(entryType, m_WorkAreaBase.AppImgPath)

        End Function

        Public Function GetPricingMarkup(ByVal pricing As Ektron.Cms.Commerce.PricingData, ByVal currencyList As List(Of Ektron.Cms.Commerce.CurrencyData), ByVal exchangeRateList As List(Of Ektron.Cms.Commerce.ExchangeRateData), ByVal entryType As Ektron.Cms.Common.EkEnumeration.CatalogEntryType, ByVal showPricingTier As Boolean, Optional ByVal Mode As ModeType = ModeType.Edit) As String
            Dim sbPricing As New StringBuilder()
            Dim showRemoveForDefault As Boolean = False
            Dim defaultCurrencyName As String = ""
            Dim defaultCurrencyId As Integer = 0
            Dim defaultCurrency As Ektron.Cms.Commerce.CurrencyData = Nothing

            For i As Integer = 0 To (currencyList.Count - 1)
                If currencyList(i).Id = m_CommerceSettings.DefaultCurrencyId Then
                    defaultCurrencyName = currencyList(i).Name
                    defaultCurrencyId = currencyList(i).Id
                    defaultCurrency = currencyList(i)
                    Exit For
                End If
            Next

            sbPricing.Append("             <table width=""100%"" border=""1"" bordercolor=""#d8e6ff""> ").Append(Environment.NewLine)
            sbPricing.Append("             <tr> ").Append(Environment.NewLine)
            sbPricing.Append("                 <td width=""100%""> ").Append(Environment.NewLine)
            sbPricing.Append(" 						    <div class=""ektron ektron_PricingWrapper""> ").Append(Environment.NewLine)
            sbPricing.Append("                             <h3> ").Append(Environment.NewLine)
            sbPricing.Append(" 	                            <span class=""currencyLabel"">").Append(m_CommerceSettings.ISOCurrencySymbol).Append(m_CommerceSettings.CurrencySymbol).Append(" ").Append(defaultCurrencyName).Append("</span> ").Append(Environment.NewLine)
            sbPricing.Append(" 	                            <select onchange=""Ektron.Commerce.Pricing.selectCurrency(this.options[this.selectedIndex].value, " & defaultCurrencyId & ");return false;""> ").Append(Environment.NewLine)
            For i As Integer = 0 To (currencyList.Count - 1)
                sbPricing.Append(" 		                            <option value=""id:ektron_Pricing_").Append(currencyList(i).Id).Append(";label:").Append(currencyList(i).Name).Append(";symbol:").Append(currencyList(i).ISOCurrencySymbol).Append(currencyList(i).CurrencySymbol).Append(""" " & IIf(currencyList(i).Id = m_CommerceSettings.DefaultCurrencyId, "selected=""selected""", "") & ">").Append(currencyList(i).AlphaIsoCode).Append("</option> ").Append(Environment.NewLine)
            Next
            sbPricing.Append(" 	                            </select> ").Append(Environment.NewLine)
            sbPricing.Append("                             </h3> ").Append(Environment.NewLine)
            sbPricing.Append("                             <div class=""ektron_Pricing_InnerWrapper""> ").Append(Environment.NewLine)
            For i As Integer = 0 To (currencyList.Count - 1)
                Dim IsDefaultCurrency As Boolean = (m_CommerceSettings.DefaultCurrencyId = currencyList(i).Id)
                Dim actualCost As Decimal = 0.0
                Dim listPrice As Decimal = 0.0
                Dim currentPrice As Decimal = 0.0
                Dim currencyPricing As Ektron.Cms.Commerce.CurrencyPricingData = pricing.GetCurrencyById(currencyList(i).Id)
                Dim tierPrices As New List(Of Ektron.Cms.Commerce.TierPriceData)
                Dim tierCount As Integer = 0
                Dim tierId As Long = 0
                Dim IsFloated As Boolean = False
                Dim exchangeRate As Decimal = 1
                If currencyPricing IsNot Nothing Then
                    'actualCost = currencyPricing.ActualCost
                    listPrice = currencyPricing.ListPrice
                    currentPrice = currencyPricing.GetSalePrice(1)
                    tierPrices = currencyPricing.TierPrices
                    tierCount = tierPrices.Count
                    IsFloated = (currencyPricing.PricingType = Common.EkEnumeration.PricingType.Floating)
                    If Mode = ModeType.Add AndAlso Not IsDefaultCurrency Then IsFloated = True
                    If tierPrices.Count > 0 Then
                        tierId = tierPrices(0).Id
                    End If
                    If (tierPrices.Count = 0 Or (tierPrices.Count = 1 AndAlso tierPrices(0).Quantity = 1)) Then tierCount = 1
                    If (currencyPricing.CurrencyId = m_CommerceSettings.DefaultCurrencyId AndAlso tierPrices.Count > 0) Then showRemoveForDefault = True
                Else
                    IsFloated = True
                    tierCount = 1
                End If
                For k As Integer = 0 To (exchangeRateList.Count - 1)
                    If exchangeRateList(k).ExchangeCurrencyId = currencyList(i).Id Then
                        exchangeRate = exchangeRateList(k).Rate
                        Exit For
                    End If
                Next
                sbPricing.Append(" 	                            <div id=""ektron_Pricing_").Append(currencyList(i).Id).Append(""" class=""ektron_Pricing_CurrencyWrapper ektron_Pricing_").Append(currencyList(i).AlphaIsoCode).Append("" & IIf(currencyList(i).Id = m_CommerceSettings.DefaultCurrencyId, " ektron_Pricing_CurrencyWrapper_Active", "") & """> ").Append(Environment.NewLine)
                sbPricing.Append(" 		                            <table class=""ektron_UnitPricing_Table"" summary=""").Append(m_WorkAreaBase.GetMessage("lbl unit pricing data")).Append("""> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            <colgroup> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <col class=""narrowCol""/> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <col class=""wideCol"" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            </colgroup> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            <thead> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <th colspan=""2"" class=""alignLeft noBorderRight""> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.GetMessage("lbl unit pricing")).Append(" ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            </thead> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            <tbody> ").Append(Environment.NewLine)

                If Not currencyList(i).Id = m_CommerceSettings.DefaultCurrencyId Then
                    sbPricing.Append(" 				                            <tr> ").Append(Environment.NewLine)
                    sbPricing.Append(" 					                            <th class=""noBorderRight""> ").Append(Environment.NewLine)
                    sbPricing.Append(" 						                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl price float exchange")).Append(""" title=""").Append(m_WorkAreaBase.GetMessage("lbl price float exchange")).Append(""" class=""moreInfo"" /> ").Append(Environment.NewLine)
                    sbPricing.Append(" 						                            <label for=""ektron_UnitPricing_Float_").Append(currencyList(i).Id).Append(""">").Append(m_WorkAreaBase.GetMessage("lbl price float")).Append(":</label> ").Append(Environment.NewLine)
                    sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine)
                    sbPricing.Append(" 					                            <td class=""noBorderLeft""> ").Append(Environment.NewLine)
                    sbPricing.Append(" 						                            <span class=""currencySymbol"">").Append(Environment.NewLine)
                    sbPricing.Append("<input onclick=""Ektron.Commerce.Pricing.floatToggle(this);"" id=""ektron_UnitPricing_Float_").Append(currencyList(i).Id).Append(""" name=""ektron_UnitPricing_Float_").Append(currencyList(i).Id).Append(""" class=""actualPrice"" type=""checkbox"" ").Append(IIf(IsFloated, "checked=""checked"" ", "")).Append(" ").Append(IIf(Not (Mode = ModeType.View), "", "disabled=""disabled"" ")).Append("/> ").Append(Environment.NewLine)
                    sbPricing.Append("</span> ").Append(Environment.NewLine)
                    sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.GetMessage("lbl price current rate")).Append(": ").Append(defaultCurrency.ISOCurrencySymbol).Append(defaultCurrency.CurrencySymbol).Append("1 = ").Append(currencyList(i).ISOCurrencySymbol).Append(currencyList(i).CurrencySymbol).Append(m_WorkAreaBase.FormatCurrency(exchangeRate, "")).Append(Environment.NewLine)

                    sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine)
                    sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine)
                End If

                'sbPricing.Append(" 				                            <tr> ").Append(Environment.NewLine)
                'sbPricing.Append(" 					                            <th class=""noBorderRight""> ").Append(Environment.NewLine)
                'sbPricing.Append(" 						                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl our purchase cost")).Append(""" title=""").Append(m_WorkAreaBase.GetMessage("lbl our purchase cost desc")).Append(""" class=""moreInfo"" /> ").Append(Environment.NewLine)
                'sbPricing.Append(" 						                            <label for=""ektron_UnitPricing_ActualPrice_").Append(currencyList(i).Id).Append(""">").Append(m_WorkAreaBase.GetMessage("lbl our purchase cost")).Append(":</label> ").Append(Environment.NewLine)
                'sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine)
                'sbPricing.Append(" 					                            <td class=""noBorderLeft""> ").Append(Environment.NewLine)
                'sbPricing.Append(" 						                            <span class=""currencySymbol"">").Append(currencyList(i).ISOCurrencySymbol).Append(currencyList(i).CurrencySymbol).Append("</span> ").Append(Environment.NewLine)
                If Not (Mode = ModeType.View) Then
                    'sbPricing.Append(" 						                            <input maxlength=""8"" id=""ektron_UnitPricing_ActualPrice_").Append(currencyList(i).Id).Append(""" onchange=""UpdateActualPrice(this);"" name=""ektron_UnitPricing_ActualPrice_").Append(currencyList(i).Id).Append(""" class=""actualPrice"" type=""text"" value=""" & m_WorkAreaBase.FormatCurrency(actualCost, "") & """ " + IIf(IsFloated, "disabled=""disabled"" ", "") + " /> ").Append(Environment.NewLine)
                Else
                    'sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.FormatCurrency(actualCost, "")).Append(Environment.NewLine)
                End If
                'sbPricing.Append(" 						                            &#160;").Append(m_WorkAreaBase.GetMessage("lbl per unit")).Append(" ").Append(Environment.NewLine)
                'sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine)
                'sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <tr class=""stripe""> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <th class=""noBorderRight""> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl list price")).Append(""" title=""").Append(m_WorkAreaBase.GetMessage("lbl list price desc")).Append(""" class=""moreInfo"" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <label for=""ektron_UnitPricing_ListPrice_").Append(currencyList(i).Id).Append(""" class=""listPrice"">").Append(m_WorkAreaBase.GetMessage("lbl list price")).Append(":</label> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <td class=""noBorderLeft""> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <span class=""currencySymbol"">").Append(currencyList(i).ISOCurrencySymbol).Append(currencyList(i).CurrencySymbol).Append("</span> ").Append(Environment.NewLine)
                If Not (Mode = ModeType.View) Then
                    sbPricing.Append(" 						                            <input maxlength=""8"" id=""ektron_UnitPricing_ListPrice_").Append(currencyList(i).Id).Append(""" onchange=""UpdateListPrice(this);"" name=""ektron_UnitPricing_ListPrice_").Append(currencyList(i).Id).Append(""" type=""text"" value=""" & m_WorkAreaBase.FormatCurrency(listPrice, "") & """ " + IIf(IsFloated, "disabled=""disabled"" ", "") + " /> ").Append(Environment.NewLine)
                Else
                    sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.FormatCurrency(listPrice, "")).Append(Environment.NewLine)
                End If
                sbPricing.Append(" 						                            &#160;").Append(m_WorkAreaBase.GetMessage("lbl per unit")).Append(" ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <th class=""noBorderRight""> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl our sales price")).Append(""" title=""").Append(m_WorkAreaBase.GetMessage("lbl our sales price desc")).Append(""" class=""moreInfo"" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <label for=""ektron_UnitPricing_SalesPrice_").Append(currencyList(i).Id).Append(""">").Append(m_WorkAreaBase.GetMessage("lbl our sales price")).Append(":</label> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <td class=""noBorderLeft""> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <span class=""currencySymbol"">").Append(currencyList(i).ISOCurrencySymbol).Append(currencyList(i).CurrencySymbol).Append("</span> ").Append(Environment.NewLine)
                If Not (Mode = ModeType.View) Then
                    sbPricing.Append(" 						                            <input maxlength=""8"" onchange=""UpdateSalesPrice(this);"" id=""ektron_UnitPricing_SalesPrice_").Append(currencyList(i).Id).Append(""" name=""ektron_UnitPricing_SalesPrice_").Append(currencyList(i).Id).Append(""" type=""text"" value=""" & m_WorkAreaBase.FormatCurrency(currentPrice, "") & """ " + IIf(IsFloated, "disabled=""disabled"" ", "") + " /> ").Append(Environment.NewLine)
                Else
                    sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.FormatCurrency(currentPrice, "")).Append(Environment.NewLine)
                End If
                sbPricing.Append(" 							                            <input id=""hdn_ektron_UnitPricing_DefaultTier_").Append(currencyList(i).Id).Append(""" name=""hdn_ektron_UnitPricing_DefaultTier_").Append(currencyList(i).Id).Append(""" class=""noFloat"" type=""hidden"" ")
                sbPricing.Append("value=""").Append(tierId).Append("""")
                sbPricing.Append("/> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            &#160;").Append(m_WorkAreaBase.GetMessage("lbl per unit")).Append(" ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            </tbody> ").Append(Environment.NewLine)
                sbPricing.Append(" 		                            </table> ").Append(Environment.NewLine)

                sbPricing.Append(" 		                            <div class=""ektron_TierPricing_Wrapper"" ").Append(IIf(tierCount > 1, "style=""display:block;""", "")).Append("> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            <table class=""ektron_TierPricing_Table"" summary=""").Append(m_WorkAreaBase.GetMessage("lbl tier pricing data")).Append("""> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <colgroup> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <col class=""ektron_TierPricing_TierRemove"" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <col class=""ektron_TierPricing_TierQuantity"" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <col class=""ektron_TierPricing_TierPrice"" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            </colgroup> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <thead> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <th colspan=""3"" class=""alignLeft""> ").Append(Environment.NewLine)
                sbPricing.Append(" 							                            ").Append(m_WorkAreaBase.GetMessage("lbl tier pricing")).Append(" ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            </th> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <tr class=""ektron_TierPricing_HeaderLabels""> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <th><img class=""ektron_TierPricing_HeaderRemoveImage"" src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/delete.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl remove pricing tier")).Append(""" title=""").Append(m_WorkAreaBase.GetMessage("lbl remove pricing tier")).Append(""" /></th> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <th>").Append(m_WorkAreaBase.GetMessage("lbl if num units greater")).Append("</th> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <th>").Append(m_WorkAreaBase.GetMessage("lbl then tier price is")).Append("</th> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            </thead> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <tbody> ").Append(Environment.NewLine)
                Dim jModifier As Integer = 0
                For j As Integer = 0 To (tierCount - 1)
                    Dim tierQuantity As Integer = 0
                    Dim tierQId As Long = 0
                    Dim tierSalePrice As Decimal = 0.0

                    Dim bShow As Boolean = True

                    If (j = 0 And tierPrices.Count = 0) Then ' old way
                        ' do nothing
                    ElseIf (j = 0 And tierPrices.Count = 1 And tierPrices(0).Quantity = 1) Then 'no tier pricing
                        'do nothing
                    ElseIf (j = 0 And tierPrices.Count > 0 AndAlso tierPrices(0).Quantity = 1) Then ' first is quantity 1 so skip
                        jModifier = -1
                        If tierPrices.Count > 1 Then bShow = False
                    Else
                        tierQuantity = tierPrices(j).Quantity
                        tierSalePrice = tierPrices(j).SalePrice
                        tierQId = tierPrices(j).Id
                    End If
                    If bShow Then
                        sbPricing.Append(" 					                            <tr class=""tier stripe"" id=""tier_").Append(j + jModifier).Append("""> ").Append(Environment.NewLine)
                        sbPricing.Append(" 						                            <td class=""tierRemove""> ").Append(Environment.NewLine)
                        If Not (Mode = ModeType.View) Then
                            sbPricing.Append(" 							                            <input type=""checkbox"" title=""").Append(m_WorkAreaBase.GetMessage("lbl remove tier")).Append(""" class=""ektron_RemoveTier_Checkbox"" onclick=""Ektron.Commerce.Pricing.Tier.toggleRemove();""/> ").Append(Environment.NewLine)
                        End If
                        sbPricing.Append(" 						                            </td> ").Append(Environment.NewLine)
                        sbPricing.Append(" 						                            <td class=""tierQuantity""> ").Append(Environment.NewLine)
                        If Not (Mode = ModeType.View) = True AndAlso Not IsFloated Then
                            sbPricing.Append(" 							                            <input maxlength=""9"" id=""ektron_TierPricing_TierQuantity_").Append(currencyList(i).Id).Append("_").Append(j + jModifier).Append(""" name=""ektron_TierPricing_TierQuantity_").Append(currencyList(i).Id).Append("_").Append(j + jModifier).Append(""" type=""text"" ")
                            sbPricing.Append("value=""" & tierQuantity & """")
                            sbPricing.Append("/> ").Append(Environment.NewLine)
                        Else
                            sbPricing.Append(" 							                            <input maxlength=""9"" id=""ektron_TierPricing_TierQuantity_").Append(currencyList(i).Id).Append("_").Append(j + jModifier).Append(""" name=""ektron_TierPricing_TierQuantity_").Append(currencyList(i).Id).Append("_").Append(j + jModifier).Append(""" type=""text"" disabled=""disabled"" ")
                            sbPricing.Append("value=""" & tierQuantity & """")
                            sbPricing.Append("/> ").Append(Environment.NewLine)
                        End If
                        sbPricing.Append(" 							                            <input id=""hdn_ektron_TierPricing_TierId_").Append(currencyList(i).Id).Append("_").Append(j + jModifier).Append(""" name=""hdn_ektron_TierPricing_TierId_").Append(currencyList(i).Id).Append("_").Append(j + jModifier).Append(""" type=""hidden"" ")
                        sbPricing.Append("value=""" & tierQId & """")
                        sbPricing.Append("/> ").Append(Environment.NewLine)
                        sbPricing.Append(" 						                            </td> ").Append(Environment.NewLine)
                        sbPricing.Append(" 						                            <td class=""tierPrice""> ").Append(Environment.NewLine)
                        sbPricing.Append(" 							                            <span class=""currencySymbol noFloat"">").Append(currencyList(i).ISOCurrencySymbol).Append(currencyList(i).CurrencySymbol).Append("</span> ").Append(Environment.NewLine)
                        If Not (Mode = ModeType.View) = True AndAlso Not IsFloated Then
                            sbPricing.Append(" 							                            <input maxlength=""12"" id=""ektron_TierPricing_TierPrice_").Append(currencyList(i).Id).Append("_").Append(j + jModifier).Append(""" name=""ektron_TierPricing_TierPrice_").Append(currencyList(i).Id).Append("_").Append(j + jModifier).Append(""" class=""noFloat"" type=""text"" ")
                            sbPricing.Append("value=""" & m_WorkAreaBase.FormatCurrency(tierSalePrice, "") & """")
                            sbPricing.Append("/> ").Append(Environment.NewLine)
                        Else
                            sbPricing.Append(" 							                            <input maxlength=""12"" id=""ektron_TierPricing_TierPrice_").Append(currencyList(i).Id).Append("_").Append(j + jModifier).Append(""" name=""ektron_TierPricing_TierPrice_").Append(currencyList(i).Id).Append("_").Append(j + jModifier).Append(""" class=""noFloat"" type=""text"" disabled=""disabled"" ")
                            sbPricing.Append("value=""" & m_WorkAreaBase.FormatCurrency(tierSalePrice, "") & """")
                            sbPricing.Append("/> ").Append(Environment.NewLine)
                        End If
                        sbPricing.Append(" 						                            </td> ").Append(Environment.NewLine)
                        sbPricing.Append(" 					                            </tr> ").Append(Environment.NewLine)
                    End If
                Next
                sbPricing.Append(" 				                            </tbody> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            </table> ").Append(Environment.NewLine)
                sbPricing.Append(" 		                            </div> ").Append(Environment.NewLine)
                sbPricing.Append(" 	                            </div> ").Append(Environment.NewLine)
            Next
            If (Mode <> ModeType.View And showPricingTier = True) Then
                sbPricing.Append(" 	                            <p class=""ektron_TierPricing_Commands clearfix""> ").Append(Environment.NewLine)
                sbPricing.Append(" 		                            <a href=""#AddPricingTier"" class=""button buttonRight greenHover marginLeft"" title=""").Append(m_WorkAreaBase.GetMessage("lbl add pricing tier")).Append(""" onclick=""Ektron.Commerce.Pricing.Tier.addTier(this);return false;""> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/coins_add.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl add pricing tier")).Append(""" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            ").Append(m_WorkAreaBase.GetMessage("lbl add pricing tier")).Append(" ").Append(Environment.NewLine)
                sbPricing.Append(" 		                            </a> ").Append(Environment.NewLine)
                sbPricing.Append(" 		                            <a href=""#RemovePricingTier"" class=""button buttonRight ektron_RemovePricingTier_Button disabled"" title=""").Append(m_WorkAreaBase.GetMessage("lbl remove pricing tier")).Append(""" onclick=""Ektron.Commerce.Pricing.Tier.removeTier();return false;"" ").Append(IIf(showRemoveForDefault, "style=""display:block;""", "")).Append("> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/coins_delete.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl remove pricing tier")).Append(""" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            ").Append(m_WorkAreaBase.GetMessage("lbl remove pricing tier")).Append(" ").Append(Environment.NewLine)
                sbPricing.Append(" 		                            </a> ").Append(Environment.NewLine)
                sbPricing.Append(" 	                            </p> ").Append(Environment.NewLine)
            End If
            sbPricing.Append("                             </div> ").Append(Environment.NewLine)
            sbPricing.Append("                             <div id=""ektron_Pricing_Modal"" class=""ektronWindow""> ").Append(Environment.NewLine)
            sbPricing.Append(" 	                            <h4 id=""ektron_Pricing_Modal_Header""> ").Append(Environment.NewLine)
            sbPricing.Append(" 		                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/closeButton.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl cancel and close window")).Append(""" class=""ektronModalClose"" />	 ").Append(Environment.NewLine)
            sbPricing.Append(" 	                            </h4> ").Append(Environment.NewLine)
            sbPricing.Append(" 	                            <div class=""ektron_Pricing_Modal_InnerWrapper""> ").Append(Environment.NewLine)
            sbPricing.Append(" 		                            <p>").Append(m_WorkAreaBase.GetMessage("js confirm remove selected pricing tiers")).Append("</p> ").Append(Environment.NewLine)
            sbPricing.Append(" 		                            <p class=""buttons clearfix""> ").Append(Environment.NewLine)
            sbPricing.Append(" 			                            <a href=""#Ok"" class=""button buttonRight greenHover marginLeft ektronModalClose"" title=""").Append(m_WorkAreaBase.GetMessage("lbl ok")).Append(""" onclick=""Ektron.Commerce.Pricing.Tier.removeTier();return false;""> ").Append(Environment.NewLine)
            sbPricing.Append(" 				                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/accept.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl ok")).Append(""" /> ").Append(Environment.NewLine)
            sbPricing.Append(" 				                            ").Append(m_WorkAreaBase.GetMessage("lbl ok")).Append(" ").Append(Environment.NewLine)
            sbPricing.Append(" 			                            </a> ").Append(Environment.NewLine)
            sbPricing.Append(" 			                            <a href=""#Cancel"" class=""button buttonRight redHover ektronModalClose"" title=""").Append(m_WorkAreaBase.GetMessage("generic cancel")).Append(""" onclick=""return false;""> ").Append(Environment.NewLine)
            sbPricing.Append(" 				                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/cancel.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("generic cancel")).Append(""" /> ").Append(Environment.NewLine)
            sbPricing.Append(" 				                            ").Append(m_WorkAreaBase.GetMessage("generic cancel")).Append(" ").Append(Environment.NewLine)
            sbPricing.Append(" 			                            </a> ").Append(Environment.NewLine)
            sbPricing.Append(" 		                            </p> ").Append(Environment.NewLine)
            sbPricing.Append(" 	                            </div> ").Append(Environment.NewLine)
            sbPricing.Append("                             </div> ").Append(Environment.NewLine)
            sbPricing.Append("                         </div> ").Append(Environment.NewLine)
            sbPricing.Append("                          ").Append(Environment.NewLine)
            sbPricing.Append("                 </td> ").Append(Environment.NewLine)
            sbPricing.Append("             </tr> ").Append(Environment.NewLine)
            sbPricing.Append("             </table> ").Append(Environment.NewLine)

            If entryType = Common.EkEnumeration.CatalogEntryType.SubscriptionProduct Then

                Dim recurrenceType As Ektron.Cms.Common.RecurrenceType = Ektron.Cms.Common.RecurrenceType.MonthlyByDay
                Dim recurrenceInterval As Integer = 1

                If pricing.IsRecurringPrice Then

                    recurrenceType = pricing.Recurrence.RecurrenceType
                    recurrenceInterval = pricing.Recurrence.Intervals

                End If


                sbPricing.Append(" 		                            <input class=""EktronRecurringPricingEditStatus"" type=""hidden"" value=""").Append(IIf(pricing.IsRecurringPrice, "true", "false")).Append(""" />").Append(Environment.NewLine)
                sbPricing.Append(" 		                            <input class=""EktronRecurringPricingMode"" type=""hidden"" value=""").Append(Mode.ToString()).Append(""" />").Append(Environment.NewLine)

                sbPricing.Append(" 		                            <table class=""ektron_RecurringPricing_Table"" summary=""").Append(m_WorkAreaBase.GetMessage("lbl recurring billing data")).Append("""> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            <colgroup> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <col class=""narrowCol""/> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <col class=""wideCol"" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            </colgroup> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            <thead> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <th colspan=""2"" class=""alignLeft noBorderRight""> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.GetMessage("lbl recurring billing")).Append(" ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            </thead> ").Append(Environment.NewLine)
                sbPricing.Append(" 			                            <tbody> ").Append(Environment.NewLine)

                '''''''''''''''''''''''''''''''''''''''''''
                'Row: Use Recurrent Billing
                sbPricing.Append(" 				                            <tr>").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <th> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl use recurrent billing")).Append(""" title=""").Append(m_WorkAreaBase.GetMessage("lbl use recurrent billing")).Append(""" class=""moreInfo"" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <label for=""PricingTabRecurringBillingUseRecurrentBilling").Append(""">").Append(m_WorkAreaBase.GetMessage("lbl use recurrent billing")).Append(":</label> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <td> ").Append(Environment.NewLine)
                If Mode = ModeType.View Then
                    sbPricing.Append(" 						                            <span id=""PricingTabRecurringBillingUseRecurrentBilling"">").Append(Environment.NewLine)
                    sbPricing.Append(IIf(pricing.IsRecurringPrice, "Yes", "No"))
                    sbPricing.Append("                                                  </span> ").Append(Environment.NewLine)
                Else
                    sbPricing.Append(" 						                            <select class=""recurringBilling"" onchange=""Ektron.Commerce.Pricing.floatRecurring(this);"" name=""PricingTabRecurringBillingUseRecurrentBilling"" id=""PricingTabRecurringBillingUseRecurrentBilling"">").Append(Environment.NewLine)
                    sbPricing.Append(" 						                                <option value=""true""").Append(IIf(pricing.IsRecurringPrice, "selected=""selected""", "")).Append(">Yes</option>").Append(Environment.NewLine)
                    sbPricing.Append(" 						                                <option value=""false""").Append(IIf(pricing.IsRecurringPrice, "", "selected=""selected""")).Append(">No</option>").Append(Environment.NewLine)
                    sbPricing.Append("                                                  </span> ").Append(Environment.NewLine)
                End If
                sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine)

                '''''''''''''''''''''''''''''''''''''''''''
                'Row: Billing Cycle
                sbPricing.Append(" 				                            <tr class=""billingCycle stripe"">").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <th>").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl billing cycle")).Append(""" title=""").Append(m_WorkAreaBase.GetMessage("lbl billing cycle desc")).Append(""" class=""moreInfo"" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <label for=""PricingTabRecurringBillingBillingCycle").Append(""" class=""billingCycle"">").Append(m_WorkAreaBase.GetMessage("lbl billing cycle")).Append(":</label> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </th>").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <td>").Append(Environment.NewLine)
                If Mode = ModeType.View Then
                    sbPricing.Append(" 						                            <span id=""PricingTabRecurringBillingBillingCycle"">").Append(Environment.NewLine)
                    sbPricing.Append(IIf(recurrenceType = Ektron.Cms.Common.RecurrenceType.MonthlyByDay, "Monthly", ""))
                    sbPricing.Append(IIf(recurrenceType = Ektron.Cms.Common.RecurrenceType.Yearly, "Yearly", ""))
                Else
                    sbPricing.Append(" 						                            <select id=""PricingTabRecurringBillingBillingCycle"" name=""PricingTabRecurringBillingBillingCycle"" ").Append(Me.GetEnabled(Mode, pricing)).Append(" /> ").Append(Environment.NewLine)
                    sbPricing.Append("                                                      <option value=""month""").Append(IIf(recurrenceType = Ektron.Cms.Common.RecurrenceType.MonthlyByDay, " SELECTED ", "")).Append(">Monthly</option>")
                    sbPricing.Append("                                                      <option value=""year""").Append(IIf(recurrenceType = Ektron.Cms.Common.RecurrenceType.Yearly, " SELECTED ", "")).Append(">Yearly</option>")
                    sbPricing.Append("                                                  </select>").Append(Environment.NewLine)
                End If
                sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            </tr>").Append(Environment.NewLine)

                '''''''''''''''''''''''''''''''''''''''''''
                'Row: Interval
                sbPricing.Append(" 				                            <tr class=""interval"">").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <th>").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <img src=""").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif"" alt=""").Append(m_WorkAreaBase.GetMessage("lbl billing intervals")).Append(""" title=""").Append(m_WorkAreaBase.GetMessage("lbl billing intervals desc")).Append(""" class=""moreInfo"" /> ").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <label for=""PricingTabRecurringBillingInterval").Append(""" class=""StartDate"">").Append(m_WorkAreaBase.GetMessage("lbl billing intervals")).Append(":</label> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </th>").Append(Environment.NewLine)
                sbPricing.Append(" 					                            <td>").Append(Environment.NewLine)
                sbPricing.Append(" 						                            <input maxlength=""8"" type=""text"" class=""interval"" ").Append(Me.GetEnabled(Mode, pricing)).Append(" name=""PricingTabRecurringBillingInterval"" title=""Select Interval"" id=""PricingTabRecurringBillingInterval"" value=""" & recurrenceInterval & """ />").Append(Environment.NewLine)
                sbPricing.Append(" 					                                <span class=""intervalRequired"">* must be numeric</span> ").Append(Environment.NewLine)
                sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine)

                sbPricing.Append(" 			                            </tbody> ").Append(Environment.NewLine)
                sbPricing.Append(" 		                            </table> ").Append(Environment.NewLine)

                If Not (Mode = ModeType.View) Then

                    sbPricing.Append("                                  <div class=""finish""> ").Append(Environment.NewLine)
                    sbPricing.Append(" 		                            <h3>").Append(m_WorkAreaBase.GetMessage("lbl important")).Append("</h3> ").Append(Environment.NewLine)
                    sbPricing.Append(" 		                            <div class=""innerWrapper""> ").Append(Environment.NewLine)
                    sbPricing.Append(" 		                            <p><span>").Append(m_WorkAreaBase.GetMessage("lbl recurring billing test")).Append("</span></p> ").Append(Environment.NewLine)
                    sbPricing.Append(" 		                            </div> ").Append(Environment.NewLine)
                    sbPricing.Append(" 		                            </div> ").Append(Environment.NewLine)

                End If

            End If

            Return sbPricing.ToString()
        End Function

        Private Function GetEnabled(ByVal Mode As Ektron.Cms.Workarea.workareaCommerce.ModeType, ByVal pricing As Ektron.Cms.Commerce.PricingData) As String
            GetEnabled = String.Empty
            If Mode = ModeType.View Or (Mode <> ModeType.View And pricing.IsRecurringPrice = False) Then
                GetEnabled = "disabled=""disabled"""
            End If
            Return GetEnabled
        End Function

        Private Function GetDate(ByVal pricing As Ektron.Cms.Commerce.PricingData, ByVal StartOrEnd As String) As String
            GetDate = Date.Today().ToString("MM/dd/yyyy")

            If pricing IsNot Nothing And pricing.Recurrence IsNot Nothing Then
                Dim parsedDate As Date
                Dim isDate As Boolean
                If StartOrEnd = "start" Then
                    GetDate = Date.TryParse(pricing.Recurrence.StartDateUtc.ToString(), parsedDate)
                Else
                    GetDate = Date.TryParse(pricing.Recurrence.EndDateUtc.ToString(), parsedDate)
                End If
                If isDate = True Then
                    GetDate = parsedDate
                End If
            End If

            Return GetDate
        End Function


    End Class


    Public Class workareajavascript
        Public Enum ErrorType
            Alert
            ErrorCollection
        End Enum
        Protected m_sPrefix As String = ""
        Public Property FunctionPrefix() As String
            Get
                Return m_sPrefix
            End Get
            Set(ByVal value As String)
                m_sPrefix = value
            End Set
        End Property
        Public ReadOnly Property RemoveHTMLFunctionName() As String
            Get
                Return m_sPrefix & "RemoveHTML"
            End Get
        End Property
        Public Function RemoveHTML() As String
            Dim sbJS As New StringBuilder()
            sbJS.Append(" function ").Append(m_sPrefix).Append("RemoveHTML(strText) ").Append(Environment.NewLine)
            sbJS.Append(" { ").Append(Environment.NewLine)
            sbJS.Append(" 	return strText.replace(/<[^>]*>/g, """"); ").Append(Environment.NewLine)
            sbJS.Append(" } ").Append(Environment.NewLine)
            Return sbJS.ToString()
        End Function
        Public ReadOnly Property ToggleDivFunctionName() As String
            Get
                Return m_sPrefix & "ToggleDiv"
            End Get
        End Property
        Public Function ToggleDiv() As String
            Dim sbJS As New StringBuilder()
            sbJS.Append("function ").Append(m_sPrefix).Append("ToggleDiv(sDiv, overrd) {" & Environment.NewLine)
            sbJS.Append("   var objcustom = document.getElementById(sDiv); " & Environment.NewLine)
            sbJS.Append("   var bOverRide = (overrd != null); " & Environment.NewLine)
            sbJS.Append("   if ((bOverRide && overrd) || (!bOverRide && objcustom.style.visibility == 'hidden')) { " & Environment.NewLine)
            sbJS.Append("       objcustom.style.position = ''; " & Environment.NewLine)
            sbJS.Append("       objcustom.style.visibility = 'visible';" & Environment.NewLine)
            sbJS.Append("   } else { " & Environment.NewLine)
            sbJS.Append("       objcustom.style.position = 'absolute'; " & Environment.NewLine)
            sbJS.Append("       objcustom.style.visibility = 'hidden';" & Environment.NewLine)
            sbJS.Append("   } " & Environment.NewLine)
            sbJS.Append("}" & Environment.NewLine)
            Return sbJS.ToString()
        End Function

        Public ReadOnly Property URLEncodeFunctionName() As String
            Get
                Return m_sPrefix & "JSURLEncode"
            End Get
        End Property
        Public Function URLEncode() As String

            Dim sbJS As New StringBuilder()

            sbJS.Append(" function ").Append(m_sPrefix).Append("JSURLEncode (clearString) { ").Append(Environment.NewLine)
            sbJS.Append("   var output = ''; ").Append(Environment.NewLine)
            sbJS.Append("   var x = 0; ").Append(Environment.NewLine)
            sbJS.Append("   clearString = clearString.toString(); ").Append(Environment.NewLine)
            sbJS.Append("   var regex = /(^[a-zA-Z0-9_.]*)/; ").Append(Environment.NewLine)
            sbJS.Append("   while (x < clearString.length) { ").Append(Environment.NewLine)
            sbJS.Append("     var match = regex.exec(clearString.substr(x)); ").Append(Environment.NewLine)
            sbJS.Append("     if (match != null && match.length > 1 && match[1] != '') { ").Append(Environment.NewLine)
            sbJS.Append("     	output += match[1]; ").Append(Environment.NewLine)
            sbJS.Append("       x += match[1].length; ").Append(Environment.NewLine)
            sbJS.Append("     } else { ").Append(Environment.NewLine)
            sbJS.Append("       if (clearString[x] == ' ') ").Append(Environment.NewLine)
            sbJS.Append("         output += '+'; ").Append(Environment.NewLine)
            sbJS.Append("       else { ").Append(Environment.NewLine)
            sbJS.Append("         var charCode = clearString.charCodeAt(x); ").Append(Environment.NewLine)
            sbJS.Append("         var hexVal = charCode.toString(16); ").Append(Environment.NewLine)
            sbJS.Append("         output += '%' + ( hexVal.length < 2 ? '0' : '' ) + hexVal.toUpperCase(); ").Append(Environment.NewLine)
            sbJS.Append("       } ").Append(Environment.NewLine)
            sbJS.Append("       x++; ").Append(Environment.NewLine)
            sbJS.Append("     } ").Append(Environment.NewLine)
            sbJS.Append("   } ").Append(Environment.NewLine)
            sbJS.Append("   return output; ").Append(Environment.NewLine)
            sbJS.Append(" } ").Append(Environment.NewLine)

            Return sbJS.ToString()

        End Function
        Public Function HasIllegalCharacters(ByVal eErrorType As ErrorType) As String
            Dim sbJS As New StringBuilder()
            sbJS.Append("function ").Append(m_sPrefix).Append("HasIllegalChar(sElement, sErr) {" & Environment.NewLine)
            sbJS.Append("   var val = document.getElementById(sElement).value;" & Environment.NewLine)
            sbJS.Append("   if ((val.indexOf("";"") > -1) || (val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
            sbJS.Append("   { " & Environment.NewLine)
            sbJS.Append("       sErr = sErr + ""(';', '\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')""; " & Environment.NewLine)
            If eErrorType = ErrorType.Alert Then
                sbJS.Append("       alert(sErr);" & Environment.NewLine)
            ElseIf eErrorType = ErrorType.ErrorCollection Then
                sbJS.Append("       ").Append(m_sPrefix).Append("AddError(sErr);" & Environment.NewLine)
            End If
            sbJS.Append("       return true;" & Environment.NewLine)
            sbJS.Append("   }" & Environment.NewLine)
            sbJS.Append("   return false;" & Environment.NewLine)
            sbJS.Append("}" & Environment.NewLine)
            Return sbJS.ToString()
        End Function
#Region "CheckKeyValue"
        Public ReadOnly Property CheckKeyValueName() As String
            Get
                Return m_sPrefix & "CheckKeyValue"
            End Get
        End Property
        Public Function CheckKeyValue() As String
            Dim sbJS As New StringBuilder()
            sbJS.Append("function ").Append(m_sPrefix).Append("CheckKeyValue(item, keys) {").Append(Environment.NewLine)
            sbJS.Append("  var keyArray = keys.split(',');").Append(Environment.NewLine)
            sbJS.Append("  for (var i = 0; i < keyArray.length; i++) {").Append(Environment.NewLine)
            sbJS.Append("    if ((document.layers) || ((!document.all) && (document.getElementById))) {").Append(Environment.NewLine)
            sbJS.Append("      if (item.which == keyArray[i]) { return false; }").Append(Environment.NewLine)
            sbJS.Append("    } else {").Append(Environment.NewLine)
            sbJS.Append("      if (event.keyCode == keyArray[i]) { return false; }").Append(Environment.NewLine)
            sbJS.Append("    }").Append(Environment.NewLine)
            sbJS.Append("  }").Append(Environment.NewLine)
            sbJS.Append("}").Append(Environment.NewLine)
            Return sbJS.ToString()
        End Function
#End Region
        Public ReadOnly Property AddErrorFunctionName() As String
            Get
                Return m_sPrefix & "AddError"
            End Get
        End Property
        Public Function AddError(ByVal ErrorCollectionName As String) As String
            Dim sbJS As New StringBuilder()
            sbJS.Append(" function ").Append(m_sPrefix).Append("AddError(sErrText) { ").Append(Environment.NewLine)
            sbJS.Append("   var iNew = ").Append(ErrorCollectionName).Append(".length; ").Append(Environment.NewLine)
            sbJS.Append("   ").Append(ErrorCollectionName).Append("[iNew] = sErrText; ").Append(Environment.NewLine)
            sbJS.Append(" } ").Append(Environment.NewLine)
            Return sbJS.ToString()
        End Function
        Public ReadOnly Property ShowErrorFunctionName() As String
            Get
                Return m_sPrefix & "ShowError"
            End Get
        End Property
        Public Function ShowError(ByVal ErrorCollectionName As String) As String
            Dim sbJS As New StringBuilder()
            sbJS.Append(" function ").Append(m_sPrefix).Append("ShowError(sValid, sNotValidBefore, sNotValidAfter) { ").Append(Environment.NewLine)
            sbJS.Append("   if (").Append(ErrorCollectionName).Append(".length > 0) { ").Append(Environment.NewLine)
            sbJS.Append("       if (sNotValidBefore != null && sNotValidBefore != '') { eval(sNotValidBefore); } ").Append(Environment.NewLine)
            sbJS.Append("       alert(").Append(ErrorCollectionName).Append(".join('\n')); ").Append(Environment.NewLine)
            sbJS.Append("       if (sNotValidAfter != null && sNotValidAfter != '') { eval(sNotValidAfter); } ").Append(Environment.NewLine)
            sbJS.Append("       ").Append(ResetErrorFunctionName).Append("();").Append(Environment.NewLine)
            sbJS.Append("   } else { ").Append(Environment.NewLine)
            sbJS.Append("       if (sValid != '') { eval(sValid); } ").Append(Environment.NewLine)
            sbJS.Append("   } ").Append(Environment.NewLine)
            sbJS.Append(" } ").Append(Environment.NewLine)
            Return sbJS.ToString()
        End Function
        Public ReadOnly Property ResetErrorFunctionName() As String
            Get
                Return m_sPrefix & "ResetError"
            End Get
        End Property
        Public Function ResetError(ByVal ErrorCollectionName As String) As String
            Dim sbJS As New StringBuilder()
            sbJS.Append(" function ").Append(m_sPrefix).Append("ResetError() { ").Append(Environment.NewLine)
            sbJS.Append("   ").Append(ErrorCollectionName).Append(" = new Array(); ").Append(Environment.NewLine)
            sbJS.Append(" } ").Append(Environment.NewLine)
            Return sbJS.ToString()
        End Function
        Public ReadOnly Property ResizeFrameFunctionName() As String
            Get
                Return m_sPrefix & "ResizeFrame"
            End Get
        End Property
        Public Function ResizeFrame() As String
            Dim sbJS As New StringBuilder()
            sbJS.Append(" function ").Append(m_sPrefix).Append("ResizeFrame(val) { ").Append(Environment.NewLine)
            sbJS.Append("   if ((typeof(top.ResizeFrame) == ""function"") && top != self) { ").Append(Environment.NewLine)
            sbJS.Append("       top.ResizeFrame(val); ").Append(Environment.NewLine)
            sbJS.Append("   } ").Append(Environment.NewLine)
            sbJS.Append(" } ").Append(Environment.NewLine)
            Return sbJS.ToString()
        End Function
    End Class

    Public Class workareatabs

        Private m_aTabs As New ArrayList
        Private m_aTabCode As New ArrayList
        Private m_bUseTabs As Boolean = False
        Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
        Protected m_sPreface As String = ""
        Protected m_bViewAsWizard As Boolean = False
        Protected m_sImgPath As String = ""
        Protected m_sCSSActive As String = "tab_actived"
        Protected m_sCSSDisabled As String = "tab_disabled"

        Public Sub New(ByVal Messages As Ektron.Cms.Common.EkMessageHelper, ByVal ImagePath As String)
            m_refMsg = Messages
            m_sImgPath = ImagePath
        End Sub
        Public Property DivPreface() As String
            Get
                Return Me.m_sPreface
            End Get
            Set(ByVal value As String)
                Me.m_sPreface = value
            End Set
        End Property
        Public ReadOnly Property TabsOn() As Boolean
            Get
                Return Me.m_bUseTabs
            End Get
        End Property

        Public Sub [On]()
            Me.m_bUseTabs = True
        End Sub

        Public Sub Off()
            Me.m_bUseTabs = False
        End Sub

        Public Sub ViewAsWizard()
            Me.m_bViewAsWizard = Not m_bViewAsWizard
            m_sCSSActive = "wizardstep_actived"
            m_sCSSDisabled = "wizardstep_disabled"
        End Sub

        Public Sub RemoveAt(ByVal index As Integer)
            If index > -1 And index < Me.m_aTabs.Count Then
                m_aTabCode.RemoveAt(index)
                m_aTabs.RemoveAt(index)
            End If
        End Sub

        Public Sub AddTabByString(ByVal TabName As String, ByVal TabIdentifier As String)
            Dim sbTab As New StringBuilder
            Dim sClass As String = m_sCSSDisabled ' "tab_disabled"
            If Me.m_aTabs.Count = 0 Then
                sClass = m_sCSSActive ' "tab_actived"
            End If
            sbTab.Append("          <td class=""").Append(sClass & """ id=""").Append(TabIdentifier).Append("""" & IIf(m_bViewAsWizard, "", " width=""1%""") & " nowrap onClick=""ShowPane('").Append(TabIdentifier).Append("');return false;"">").Append(Environment.NewLine)
            sbTab.Append("              <b>&nbsp;").Append(TabName).Append("&nbsp;</b>").Append(Environment.NewLine)
            sbTab.Append("          </td>").Append(Environment.NewLine)
            m_aTabCode.Add(sbTab.ToString())
            m_aTabs.Add(TabIdentifier)
        End Sub

        Public Sub SetActiveTab(ByVal TabIndex As Integer)
            If m_aTabCode.Count >= (TabIndex + 1) Then
                For i As Integer = 0 To (m_aTabCode.Count - 1)
                    If i = TabIndex Then
                        m_aTabCode(i) = Replace(m_aTabCode(i), "class=""" & m_sCSSDisabled & "", "class=""" & m_sCSSActive & "")
                    Else
                        m_aTabCode(i) = Replace(m_aTabCode(i), "class=""" & m_sCSSActive & "", "class=""" & m_sCSSDisabled & "")
                    End If
                Next
            End If
        End Sub

        Public Sub AddTabByString(ByVal TabName As String, ByVal TabIdentifier As String, ByVal urlpath As String)
            Dim sbTab As New StringBuilder
            Dim sClass As String = m_sCSSDisabled ' "tab_disabled"
            If Me.m_aTabs.Count = 0 Then
                sClass = m_sCSSActive ' "tab_actived"
            End If
            sbTab.Append("          <td class=""").Append(sClass & """ id=""").Append(TabIdentifier).Append(""" width=""1%"" nowrap onClick=""window.location.href='" & Replace(urlpath, "'", "\'") & "';"">").Append(Environment.NewLine)
            sbTab.Append("              <b>&nbsp;").Append(TabName).Append("&nbsp;</b>").Append(Environment.NewLine)
            sbTab.Append("          </td>").Append(Environment.NewLine)
            m_aTabCode.Add(sbTab.ToString())
            m_aTabs.Add(TabIdentifier)
        End Sub

        Public Sub AddTabByMessage(ByVal TabMessage As String, ByVal TabIdentifier As String)
            AddTabByString(m_refMsg.GetMessage(TabMessage), TabIdentifier)
        End Sub

        Public Function RenderTabs() As String
            If Me.m_bUseTabs Then
                Dim sbJS As New StringBuilder()
                Dim sbTabs As New StringBuilder()

                sbJS.Append("<script type=""text/javascript"">" & Environment.NewLine)
                sbJS.Append("		function IsBrowserIE() " & Environment.NewLine)
                sbJS.Append("		{" & Environment.NewLine)
                sbJS.Append("		    // document.all is an IE only property" & Environment.NewLine)
                sbJS.Append("		    return (document.all ? true : false);" & Environment.NewLine)
                sbJS.Append("		}" & Environment.NewLine)
                'sbJS.Append("		function ShowPane(tabID) " & Environment.NewLine)
                'sbJS.Append("		{" & Environment.NewLine)
                'sbJS.Append("			var arTab = new Array(")
                sbJS.Append("       bEnableTabs = true;").Append(Environment.NewLine)
                sbJS.Append("           function ShowPane(tabID, paneshift)  ").Append(Environment.NewLine)
                sbJS.Append("			{").Append(Environment.NewLine)
                sbJS.Append("			    if (typeof inPublishProcess == 'boolean' && inPublishProcess == true){").Append(Environment.NewLine)
                sbJS.Append("                   return false;").Append(Environment.NewLine)
                sbJS.Append("               }").Append(Environment.NewLine)
                sbJS.Append("				if (false == bEnableTabs){").Append(Environment.NewLine)
                sbJS.Append("					return false;").Append(Environment.NewLine)
                sbJS.Append("				}").Append(Environment.NewLine)
                sbJS.Append("				").Append(Environment.NewLine)
                sbJS.Append("				// For Netscape/FireFox: Objects appear to get destroyed when ""display"" is set to ""none"" and re-created ").Append(Environment.NewLine)
                sbJS.Append("				// when ""display"" is set to ""block."" Instead will use the appropriate style-sheet ").Append(Environment.NewLine)
                sbJS.Append("				// class to move the unselected items to a position where they are not visible.").Append(Environment.NewLine)
                sbJS.Append("				// For IE: If the ActiveX control is display=""none"" programmatically rather than by user click,").Append(Environment.NewLine)
                sbJS.Append("				// the ActiveX control seems to uninitialize, for example, the DHTML Edit Control (DEC) is gone.").Append(Environment.NewLine)
                sbJS.Append("				var CurrentPaneIndex = -1; ").Append(Environment.NewLine)
                sbJS.Append("				var aryTabs = [")

                sbTabs.Append("<table height=""20"" width=""100%"" " & IIf(m_bViewAsWizard, "class=""workareatabbar"" ", "") & ">").Append(Environment.NewLine).Append("     <tr>").Append(Environment.NewLine)
                If m_bViewAsWizard Then
                    sbTabs.Append(" <td valign=""center"" nowrap=""nowrap"" width=""5%"">&nbsp;Step <span id=""currentStep"">1</span> of <span id=""totalSteps"">").Append(m_aTabCode.Count.ToString()).Append("</span>&nbsp;&nbsp;&nbsp;&nbsp;</td> ").Append(Environment.NewLine)
                    sbTabs.Append(" <td nowrap=""nowrap""><table id=""stepsTable"" cellspacing=""0"" cellpadding=""0"" border=""1""> ").Append(Environment.NewLine)
                    sbTabs.Append("     <tbody> ").Append(Environment.NewLine)
                    sbTabs.Append("       <tr> ").Append(Environment.NewLine)
                    For i As Integer = 0 To (m_aTabCode.Count - 1)
                        sbJS.Append("""" & Me.m_aTabs(i) & """")
                        sbTabs.Append(m_aTabCode(i))
                        If Not (i = (m_aTabCode.Count - 1)) Then
                            sbJS.Append(",")
                        End If
                    Next
                    'sbJS.Append("         <td id=""step1"">1</td> ").Append(Environment.NewLine)
                    sbTabs.Append("       </tr> ").Append(Environment.NewLine)
                    sbTabs.Append("     </tbody> ").Append(Environment.NewLine)
                    sbTabs.Append(" </table></td> ").Append(Environment.NewLine)
                    sbTabs.Append(" <td valign=""center"" nowrap=""nowrap"" width=""10%"">&nbsp;&nbsp;&nbsp;&nbsp;<a id=""btnBackStep"" title=""Back"" onClick=""ShowPane(null, -1); return false;"" href=""#""><img height=""16"" alt=""Back"" src=""").Append(m_sImgPath).Append("btn_left_blue.gif"" width=""16"" align=""middle"" border=""0"" />&nbsp;<span id=""spn_back"">Begin</span></a>&nbsp;&nbsp;<a id=""btnNextStep"" title=""Next"" onClick=""ShowPane(null, 1); return false;"" href=""#""><img height=""16"" alt=""Next"" src=""").Append(m_sImgPath).Append("btn_right_blue.gif"" width=""16"" align=""middle"" border=""0"" />&nbsp;<span id=""spn_next"">Next</span></a>")
                    ' sbTabs.Append("<a id=""btnDoneSteps"" title=""Done"" onClick=""javascript:oProgressSteps.done(); return false;"" href=""#""><img height=""16"" alt=""Done"" src=""").Append(m_sImgPath).Append("btn_square_blue.gif"" width=""16"" align=""middle"" border=""0"" />&nbsp;Done</a>&nbsp;&nbsp;<a id=""btnCancelSteps"" title=""Cancel"" onClick=""javascript:oProgressSteps.cancel(); return false;"" href=""#""><img height=""16"" alt=""Cancel"" src=""").Append(m_sImgPath).Append("btn_x_blue.gif"" width=""16"" align=""middle"" border=""0"" />&nbsp;Cancel</a> ")
                    sbTabs.Append("</td> ").Append(Environment.NewLine)
                    ' sbTabs.Append(" <td valign=""center""><span id=""helpBtn5"">&nbsp;<a href=""#""><img id=""DeskTopHelp"" title=""Click here to get help "" onClick=""javascript:PopUpWindow('/WEBSRC/workarea/help/index.html?alias=NoCollectiontopiclist.xml', 'SitePreview', 600, 500, 1, 1);return false;"" src=""").Append(m_sImgPath).Append("menu/help.gif"" border=""0"" /></a>&nbsp; </span></td> ").Append(Environment.NewLine)
                    sbTabs.Append(" <td width=""80%"">&nbsp;&nbsp;</td> ").Append(Environment.NewLine)
                Else
                    For i As Integer = 0 To (m_aTabCode.Count - 1)
                        sbJS.Append("""" & Me.m_aTabs(i) & """")
                        sbTabs.Append(m_aTabCode(i))
                        If i = (m_aTabCode.Count - 1) Then
                            sbTabs.Append("          <td class=""tab_last"" width=""91%"" nowrap>&nbsp;</td>").Append(Environment.NewLine)
                        Else
                            sbJS.Append(",")
                            sbTabs.Append("          <td class=""tab_spacer"" width=""1%"" nowrap>&nbsp;</td>").Append(Environment.NewLine)
                        End If
                    Next
                End If

                sbJS.Append("];").Append(Environment.NewLine)
                sbJS.Append(" 				if ( paneshift != null && (paneshift == 1 || paneshift == -1) ) { ").Append(Environment.NewLine)
                sbJs.Append(" 					 ").Append(Environment.Newline)
                sbJs.Append(" 					for (var i = 0; i < aryTabs.length; i++)  ").Append(Environment.Newline)
                sbJs.Append(" 					{ ").Append(Environment.Newline)
                sbJs.Append(" 						objElem = document.getElementById(aryTabs[i]); ").Append(Environment.Newline)
                sbJs.Append(" 						if ( objElem != null && objElem.className == ""wizardstep_actived"" ) { CurrentPaneIndex = i; tabID = aryTabs[i]; break; } ").Append(Environment.Newline)
                sbJs.Append(" 					} ").Append(Environment.Newline)
                sbJs.Append(" 					 ").Append(Environment.Newline)
                sbJs.Append(" 					if ( aryTabs[CurrentPaneIndex + paneshift] != null ) { ").Append(Environment.Newline)
                sbJS.Append(" 						tabID = aryTabs[CurrentPaneIndex + paneshift]; ").Append(Environment.NewLine)
                sbJS.Append(" 						if ( (CurrentPaneIndex + paneshift) == 0 ) { document.getElementById('spn_back').innerHTML = 'Begin'; } else { document.getElementById('spn_back').innerHTML = 'Back'; } ").Append(Environment.NewLine)
                sbJS.Append(" 						if ( (CurrentPaneIndex + paneshift) == (aryTabs.length - 1) ) { document.getElementById('spn_next').innerHTML = 'Done'; } ").Append(Environment.NewLine)
                sbJs.Append(" 					} ").Append(Environment.Newline)
                sbJs.Append(" 					 ").Append(Environment.Newline)
                sbJs.Append(" 				} ").Append(Environment.Newline)
                sbJS.Append("				for (var i = 0; i < aryTabs.length; i++) ").Append(Environment.NewLine)
                sbJS.Append("				{").Append(Environment.NewLine)
                sbJS.Append("					SetPaneVisible(aryTabs[i], false);").Append(Environment.NewLine)
                sbJS.Append("					SetPaneVisible(aryTabs[i], (tabID == aryTabs[i]));").Append(Environment.NewLine)
                sbJS.Append("				    if ((tabID == aryTabs[i]) && document.getElementById('currentStep') != null) { document.getElementById('currentStep').innerHTML = (i + 1); }").Append(Environment.NewLine)
                sbJS.Append("				}").Append(Environment.NewLine)
                sbJS.Append("			}").Append(Environment.NewLine)
                sbJS.Append("			").Append(Environment.NewLine)
                sbJS.Append("			function SetPaneVisible(tabID, bVisible)").Append(Environment.NewLine)
                sbJS.Append("			{").Append(Environment.NewLine)
                sbJS.Append("				var objElem = null;").Append(Environment.NewLine)
                sbJS.Append("				objElem = document.getElementById(tabID);").Append(Environment.NewLine)
                sbJS.Append("				if (objElem != null) ").Append(Environment.NewLine)
                sbJS.Append("				{").Append(Environment.NewLine)
                sbJS.Append("					objElem.className = (bVisible ? """).Append(m_sCSSActive).Append(""" : """).Append(m_sCSSDisabled).Append(""");").Append(Environment.NewLine)
                sbJS.Append("				}").Append(Environment.NewLine)
                sbJS.Append("				objElem = document.getElementById(""" & m_sPreface & "_"" + tabID);").Append(Environment.NewLine)
                sbJS.Append("				if (objElem != null) ").Append(Environment.NewLine)
                sbJS.Append("				{").Append(Environment.NewLine)
                'sbJS.Append("					// For Safari on the Mac (to fix Ephox Editor issues), ").Append(Environment.NewLine)
                'sbJS.Append("					// the actual class names are overridden in the code behind").Append(Environment.NewLine)
                'sbJS.Append("					// (uses special classes when Safari on the Mac is detected):").Append(Environment.NewLine)
                sbJS.Append("					objElem.className = (bVisible ? ""selected_editor"" : ""unselected_editor"");").Append(Environment.NewLine)
                sbJS.Append("				}").Append(Environment.NewLine)
                sbJS.Append("			}").Append(Environment.NewLine)
                sbJS.Append("</script>" & Environment.NewLine)

                sbTabs.Append("     </tr>").Append(Environment.NewLine & "</table>").Append(Environment.NewLine)
                Return sbJS.ToString() & Environment.NewLine & sbTabs.ToString()
            Else
                Return ""
            End If
        End Function
    End Class

    Public Class workareawizard

        Private m_bUseWizard As Boolean = False

        Public ReadOnly Property WizardOn() As Boolean
            Get
                Return Me.m_bUseWizard
            End Get
        End Property

        Public Sub [On]()
            Me.m_bUseWizard = True
        End Sub

        Public Sub Off()
            Me.m_bUseWizard = False
        End Sub

    End Class

    Public Class workareautil

        Public Sub New()

        End Sub

    End Class

    Public Class workareaajax

        Private m_sAppPath As String = ""

        Public Sub New(ByVal Apppath As String)
            m_sAppPath = Apppath
        End Sub

        Public URLQuery As String = ""
        Public ResponseJS As String = ""
        Public FunctionName As String = ""

        Public Function Render() As String
            Dim sbAEJS As New System.Text.StringBuilder
            sbAEJS.Append("var req;").Append(Environment.NewLine)
            sbAEJS.Append("var bexists;").Append(Environment.NewLine)
            sbAEJS.Append("function loadXMLDoc(url) ").Append(Environment.NewLine)
            sbAEJS.Append("{").Append(Environment.NewLine)
            sbAEJS.Append("    // branch for native XMLHttpRequest object").Append(Environment.NewLine)
            sbAEJS.Append("    if (window.XMLHttpRequest) {").Append(Environment.NewLine)
            sbAEJS.Append("        req = new XMLHttpRequest();").Append(Environment.NewLine)
            sbAEJS.Append("        req.onreadystatechange = processReqChange;").Append(Environment.NewLine)
            sbAEJS.Append("        req.open(""GET"", url, true);").Append(Environment.NewLine)
            sbAEJS.Append("        req.send(null);").Append(Environment.NewLine)
            sbAEJS.Append("    // branch for IE/Windows ActiveX version").Append(Environment.NewLine)
            sbAEJS.Append("    } else if (window.ActiveXObject) {").Append(Environment.NewLine)
            sbAEJS.Append("        req = new ActiveXObject(""Microsoft.XMLHTTP"");").Append(Environment.NewLine)
            sbAEJS.Append("        if (req) {").Append(Environment.NewLine)
            sbAEJS.Append("            req.onreadystatechange = processReqChange;").Append(Environment.NewLine)
            sbAEJS.Append("            req.open(""GET"", url, true);").Append(Environment.NewLine)
            sbAEJS.Append("            req.send();").Append(Environment.NewLine)
            sbAEJS.Append("        }").Append(Environment.NewLine)
            sbAEJS.Append("    }").Append(Environment.NewLine)
            sbAEJS.Append("}").Append(Environment.NewLine)
            sbAEJS.Append("function processReqChange() ").Append(Environment.NewLine)
            sbAEJS.Append("{").Append(Environment.NewLine)
            sbAEJS.Append("    // only if req shows ""complete""").Append(Environment.NewLine)
            sbAEJS.Append("    if (req.readyState == 4) {").Append(Environment.NewLine)
            sbAEJS.Append("        // only if ""OK""").Append(Environment.NewLine)
            sbAEJS.Append("        if (req.status == 200) {").Append(Environment.NewLine)
            sbAEJS.Append("            // ...processing statements go here...").Append(Environment.NewLine)
            sbAEJS.Append("      response  = req.responseXML.documentElement;").Append(Environment.NewLine)
            sbAEJS.Append("").Append(Environment.NewLine)
            sbAEJS.Append("      method    = response.getElementsByTagName('method')[0].firstChild.data;").Append(Environment.NewLine)
            sbAEJS.Append("").Append(Environment.NewLine)
            sbAEJS.Append("      result    = response.getElementsByTagName('result')[0].firstChild.data;").Append(Environment.NewLine)
            sbAEJS.Append("").Append(Environment.NewLine)
            sbAEJS.Append("      eval(method + '(\'\', result);');").Append(Environment.NewLine)
            sbAEJS.Append("        } else {").Append(Environment.NewLine)
            sbAEJS.Append("            alert(""There was a problem retrieving the XML data:\n"" + req.statusText);").Append(Environment.NewLine)
            sbAEJS.Append("        }").Append(Environment.NewLine)
            sbAEJS.Append("    }").Append(Environment.NewLine)
            sbAEJS.Append("}").Append(Environment.NewLine)
            sbAEJS.Append("").Append(Environment.NewLine)
            sbAEJS.Append("function ").Append(FunctionName).Append("(input, response)").Append(Environment.NewLine)
            sbAEJS.Append("{").Append(Environment.NewLine)
            sbAEJS.Append("  if (response != ''){ ").Append(Environment.NewLine)
            sbAEJS.Append("    // Response mode").Append(Environment.NewLine)
            sbAEJS.Append(ResponseJS).Append(Environment.NewLine)
            sbAEJS.Append("  }else{").Append(Environment.NewLine)
            sbAEJS.Append("    // Input mode").Append(Environment.NewLine)
            sbAEJS.Append("    url = '").Append(m_sAppPath).Append("AJAXbase.aspx?").Append(URLQuery).Append("';").Append(Environment.NewLine)
            sbAEJS.Append("    loadXMLDoc(url);").Append(Environment.NewLine)
            sbAEJS.Append("  }").Append(Environment.NewLine)
            sbAEJS.Append("").Append(Environment.NewLine)
            sbAEJS.Append("}").Append(Environment.NewLine)
            Return sbAEJS.ToString()
        End Function
    End Class
End Namespace