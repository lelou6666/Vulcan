Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.RulesEngine
Imports System.Data
Imports Ektron.Cms.Workarea

Partial Class businessrules_ruleset
    Inherits workareabase

    Protected m_rulesUI As RulesEngine.UI
    Protected m_aRuleset As RuleSet()
    Protected m_refContent As Content.EkContent
    Protected m_strStyleSheetJS As String = ""
    Protected m_refCommon As New Ektron.Cms.CommonApi
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If m_refCommon.RequestInformationRef.IsMembershipUser Or m_refCommon.RequestInformationRef.UserId = 0 Then
                Response.Redirect("../login.aspx?fromLnkPg=1", False)
                Exit Sub
            Else
                Page.Title = "CMS Business Rulesets"
                noRulesInSet.Text = m_refMsg.GetMessage("lbl no rules in bussiness ruleset")
                ShowHidden()

                m_rulesUI = New RulesEngine.UI(m_refContentApi.RequestInformationRef)
                m_aRuleset = m_rulesUI.GetAllRulesets

                If m_sPageAction = "edit" Then
                    If Not (Page.IsPostBack) Then
                        traddruleset.Visible = True
                        trgrid.Visible = False
                        GoGet()
                        AddEditRulesetToolBar()
                        RulesJS()
                        AddEditJS("edit")
                    Else
                        RulesetSaveHandler()
                        Response.Redirect("ruleset.aspx?action=View&id=" & m_iID.ToString(), False)
                    End If
                    traddedit.Visible = True
                    trgrid.Visible = False
                ElseIf m_sPageAction = "view" Then
                    If Not (Page.IsPostBack) Then
                        GoGet()
                        ViewRulesetToolBar()
                        RulesJS()
                        AddEditJS("add")
                    End If
                    traddedit.Visible = True
                    trgrid.Visible = False
                ElseIf m_sPageAction = "add" Then
                    SetAction("")
                    If Not (Page.IsPostBack) Then
                        AddRulesetToolBar()
                        traddruleset.Visible = True
                        trgrid.Visible = False
                        AddJS()
                        If (Request.QueryString("identifier") <> "") Then
                            txtIdentifier.Value = Server.HtmlEncode(Request.QueryString("identifier"))
                        Else
                            tridentifier.Visible = False
                        End If
                    Else
                        m_iID = RulesetAddHandler()
                        Response.Redirect("ruleset.aspx?action=View&id=" & m_iID.ToString(), False)
                    End If
                ElseIf m_sPageAction = "select" Then
                    SetAction("")
                    If Not (Page.IsPostBack) Then
                        traddedit.Visible = True
                        SelectRulesToolBar()
                        GetSelectableRules()
                        RulesJS()
                        AddEditJS("select")
                    Else
                        RulesetSelectHandler()
                        Response.Redirect("ruleset.aspx?action=View&id=" & m_iID.ToString(), False)
                    End If
                ElseIf m_sPageAction = "remove" Then
                    RulesetDeleteHandler()
                    Response.Redirect("ruleset.aspx", False)
                Else
                    traddedit.Visible = False
                    ShowRulesetToolBar()
                    PopulateViewRuleSetGrid(m_aRuleset)
                End If
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try

        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)
    End Sub
    Private Sub RulesetSelectHandler()
        Dim aruleids As String()
        Dim rsNew As RuleSet = New RuleSet(m_refContentApi.RequestInformationRef, m_iID)
        If Request.Form("txtactiverules") <> "" Then
            aruleids = Split(Request.Form("txtactiverules"), ",")
        Else
            aruleids = Array.CreateInstance(GetType(String), 0)
        End If
        rsNew.AddAssociatedRules(aruleids)
    End Sub
    Private Function RulesetAddHandler() As Long
        Dim sName As String = ""
        Dim rsNew As RuleSet = New RuleSet(m_refContentApi.RequestInformationRef)

        sName = Request.Form("txtRulesetName")
        rsNew.SetName(sName)
        If Request.Form("txtIdentifier") <> "" Then
            rsNew.SetIdentifier(Request.Form("txtIdentifier"))
        End If
        rsNew.Save()
        Return rsNew.ID
    End Function

    Private Sub RulesetSaveHandler()
        Dim aruleids As String()
        Dim aenabledruleids As String()
        Dim rsNew As RuleSet = New RuleSet(m_refContentApi.RequestInformationRef, m_iID)
        If Request.Form("txtactiverules") <> "" Then
            aruleids = Split(Request.Form("txtactiverules"), ",")
        Else
            aruleids = Array.CreateInstance(GetType(String), 0)
        End If
        If Request.Form("txtenabledrules") <> "" Then
            aenabledruleids = Split(Request.Form("txtenabledrules"), ",")
        Else
            aenabledruleids = Array.CreateInstance(GetType(String), 0)
        End If
        rsNew.SetName(Request.Form(txtRulesetName.UniqueID))
        rsNew.Save()
        rsNew.UpdateRules(aruleids, aenabledruleids)
    End Sub

    Private Sub RulesetDeleteHandler()
        Dim rsNew As RuleSet = New RuleSet(m_refContentApi.RequestInformationRef, m_iID)
        rsNew.Delete()
    End Sub

    Private Sub ShowRulesetToolBar()
        Dim bAdmin As Boolean = False
        Dim bRuleEditor As Boolean = False
        MyBase.SetTitleBarToMessage("lbl ruleset")
        If m_refContent Is Nothing Then
            m_refContent = m_refContentApi.EkContentRef
        End If
        bAdmin = m_refContent.IsAllowed(0, 0, "users", "IsAdmin")
        bRuleEditor = m_refContent.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminRuleEditor, m_refContentApi.UserId, False)
        If (bRuleEditor Or bAdmin) Then
            MyBase.AddButton(m_refContentApi.AppPath & "images/UI/Icons/add.png", "ruleset.aspx?action=Add", m_refMsg.GetMessage("alt addruleset"), m_refMsg.GetMessage("btn addruleset"), "")
        End If
        MyBase.AddHelpButton("view_rulesets")
    End Sub

    Private Sub AddEditRulesetToolBar()
        Dim bAdmin As Boolean = False
        Dim bRuleEditor As Boolean = False
        SetAction("edit")
        MyBase.SetTitleBarToMessage("Business Rules")
        If m_refContent Is Nothing Then
            m_refContent = m_refContentApi.EkContentRef
        End If
        bAdmin = m_refContent.IsAllowed(0, 0, "users", "IsAdmin")
        bRuleEditor = m_refContent.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminRuleEditor, m_refContentApi.UserId, False)
        If m_sPageAction = "edit" Then
            MyBase.SetTitleBarToString(m_refMsg.GetMessage("lbl edit ruleset") & " " & m_aRuleset(0).Name & "")
        Else
            MyBase.SetTitleBarToString(m_refMsg.GetMessage("alt View Ruleset") & " " & m_aRuleset(0).Name & "")
        End If
        txtRulesetName.Text = m_aRuleset(0).Name
        If (bRuleEditor Or bAdmin) Then
            MyBase.AddButton(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt Click here to save this ruleset"), m_refMsg.GetMessage("lbl save ruleset"), "onclick=""SaveRule();"" ")
            MyBase.AddButton(m_refContentApi.AppPath & "images/UI/Icons/remove.png", "#", m_refMsg.GetMessage("alt Click here to remove rule"), m_refMsg.GetMessage("lbl Remove Rule"), "onclick=""RuleWizard.removeRuleItem(RuleWizard.getSelectedRule())"" ")
            If m_aRuleset(0).Rules.Length > 1 Then
                MyBase.AddButton(m_refContentApi.AppPath & "images/UI/Icons/arrowUp.png", "#", m_refMsg.GetMessage("alt Click to move up"), m_refMsg.GetMessage("lbl move up"), "onclick=""RuleWizard.moveRuleItem('up', RuleWizard.getSelectedRule())""  ")
                MyBase.AddButton(m_refContentApi.AppPath & "images/UI/Icons/arrowDown.png", "#", m_refMsg.GetMessage("alt Click to move down"), m_refMsg.GetMessage("lbl Move Down"), "onclick=""RuleWizard.moveRuleItem('down', RuleWizard.getSelectedRule())"" ")
            End If
        End If
        MyBase.AddBackButton("ruleset.aspx?action=View&id=" & m_iID.ToString() & "&LangType=" & m_refContentApi.ContentLanguage.ToString())
        MyBase.AddHelpButton("edit_ruleset")
    End Sub

    Private Sub ViewRulesetToolBar()
        Dim bAdmin As Boolean = False
        Dim bRuleEditor As Boolean = False
        SetAction("view")
        MyBase.SetTitleBarToMessage("lbl ruleset")
        If m_refContent Is Nothing Then
            m_refContent = m_refContentApi.EkContentRef
        End If
        bAdmin = m_refContent.IsAllowed(0, 0, "users", "IsAdmin")
        bRuleEditor = m_refContent.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminRuleEditor, m_refContentApi.UserId, False)

        MyBase.SetTitleBarToString(m_refMsg.GetMessage("alt View Ruleset") & " " & m_aRuleset(0).Name & "")
        If (bRuleEditor Or bAdmin) Then
            MyBase.AddButtonText("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'file');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'file');"" onmouseout=""this.className='menuRootItem'""><span class=""new"">" & m_refMsg.GetMessage("lbl New") & "</span></td>")
            MyBase.AddButtonText("<td class=""menuRootItem"" onclick=""MenuUtil.use(event, 'action');"" onmouseover=""this.className='menuRootItemSelected';MenuUtil.use(event, 'action');"" onmouseout=""this.className='menuRootItem'""><span class=""action"">" & m_refMsg.GetMessage("lbl Action") & "</span></td>")

            Dim result As New StringBuilder()
            result.Append("<script type=""text/javascript"">" & Environment.NewLine)
            result.Append("    var actmenu = new Menu( ""action"" );" & Environment.NewLine)
            result.Append("    actmenu.addItem(""&nbsp;<img src='" & m_refContentApi.AppPath & "images/ui/icons/contentEdit.png' />&nbsp;&nbsp;" & MyBase.GetMessage("lbl edit ruleset") & """, function() { window.location.href = 'ruleset.aspx?action=Edit&id=" & m_iID.ToString() & "' } );" & Environment.NewLine)
            result.Append("    actmenu.addItem(""&nbsp;<img src='" & m_refContentApi.AppPath & "images/ui/icons/cogEdit.png' />&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl Edit Rule") & """, function() { RuleWizard.showEditRuleWizard(RuleWizard.getSelectedRule()) } );" & Environment.NewLine)
            result.Append("    actmenu.addItem(""&nbsp;<img src='" & m_refContentApi.AppPath & "images/ui/icons/delete.png' />&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl Delete Ruleset") & """, function() { var agree = VerifyDelete(); if(agree === true) {window.location.href = 'ruleset.aspx?action=Remove&id=" & m_iID.ToString() & "';}} );" & Environment.NewLine)
            result.Append("    actmenu.addItem(""&nbsp;<img src='" & m_refContentApi.AppPath & "images/ui/icons/back.png' />&nbsp;&nbsp;" & m_refMsg.GetMessage("lbl Back") & """, function() { window.location.href = 'ruleset.aspx'} );" & Environment.NewLine)

            result.Append("    MenuUtil.add( actmenu );" & Environment.NewLine)
            'end
            result.Append("    </script>" & Environment.NewLine)
            ltrrulejs.Text = result.ToString()
        End If
        MyBase.AddHelpButton("view_ruleset")
    End Sub

    Private Sub SelectRulesToolBar()
        MyBase.SetTitleBarToString(m_refMsg.GetMessage("lbl Add Existing Ruleset"))
        MyBase.AddButton(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt Click here to add these rules"), m_refMsg.GetMessage("lbl save ruleset"), "onclick=""SaveRule();"" ")
        MyBase.AddBackButton("ruleset.aspx?action=View&id=" & m_iID.ToString())
        MyBase.AddHelpButton("SelectExistingRule")
    End Sub

    Private Sub AddRulesetToolBar()
        MyBase.SetTitleBarToString(m_refMsg.GetMessage("btn addruleset"))
        MyBase.AddButton(m_refContentApi.AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt Click here to save this ruleset"), m_refMsg.GetMessage("lbl save ruleset"), "onclick=""return VerifyForm()"" ")
        MyBase.AddBackButton("ruleset.aspx")
        MyBase.AddHelpButton("AddRuleset")
    End Sub

    Private Sub PopulateViewRuleSetGrid(ByVal aruleset As RuleSet())
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        Dim iCount As Integer

        ViewRuleSetGrid.ShowHeader = True

        colBound.DataField = "ID"
        colBound.HeaderText = m_refMsg.GetMessage("rulesetheader id")
        colBound.HeaderStyle.CssClass = "center widthNarrow"
        colBound.ItemStyle.CssClass = "center"
        ViewRuleSetGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "NAME"
        colBound.HeaderText = m_refMsg.GetMessage("rulesetheader name")
        colBound.ItemStyle.Wrap = False
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        ViewRuleSetGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "IDENTIFIER"
        colBound.HeaderText = m_refMsg.GetMessage("rulesetheader identifier")
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        ' ViewRuleSetGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("NAME", GetType(String)))
        dt.Columns.Add(New DataColumn("IDENTIFIER", GetType(String)))

        Dim i As Integer = 0
        iCount = (aruleset.Length - 1)
        For i = 0 To iCount
            If Not (aruleset(i) Is Nothing) Then
                dr = dt.NewRow()
                dr(0) = "<a href=""ruleset.aspx?action=View&id=" & aruleset(i).ID.ToString() & """>" & aruleset(i).ID.ToString() & "</a>"
                dr(1) = "<a href=""ruleset.aspx?action=View&id=" & aruleset(i).ID.ToString() & """>" & aruleset(i).Name & "</a>"
                dr(2) = Server.HtmlEncode(aruleset(i).Identifier)
                dt.Rows.Add(dr)
            End If
        Next i
        Dim dv As New DataView(dt)
        ViewRuleSetGrid.DataSource = dv
        ViewRuleSetGrid.DataBind()
    End Sub

    Private Sub AddEditJS(ByVal actiontype As String)
        Dim sbAEJS As New StringBuilder()

        ' register JS files
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "businessrules/includes/com.ektron.ui.rules.js", "EktronRulesJS")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "tree/js/com.ektron.net.http.js", "EktronTreeNetHttpJS")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "tree/js/com.ektron.utils.cookie.js", "EktronTreeUtilsCookieJS")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "tree/js/com.ektron.utils.debug.js", "EktronTreeUtilsDebugJS")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "tree/js/com.ektron.utils.dom.js", "EktronTreeUtilsDomJS")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "tree/js/com.ektron.utils.form.js", "EktronTreeUtilsFormJS")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "tree/js/com.ektron.utils.log.js", "EktronTreeUtilsLogJS")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "tree/js/com.ektron.utils.querystring.js", "EktronTreeUtilsQueryString")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "tree/js/com.ektron.utils.string.js", "EktronTreeUtilsString")
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "tree/js/com.ektron.utils.xml.js", "EktronTreeUtilsXmlJS")

        ' register CSS files
        Ektron.Cms.API.Css.RegisterCss(Me, m_refContentApi.AppPath & "businessrules/css/com.ektron.rules.wizard.css", "EktronBusinessRulesWizardCss")

        ' build JS
        sbAEJS.Append("<script type=""text/javascript"">" & Environment.NewLine)
        If actiontype <> "select" Then
            sbAEJS.Append(AJAXcheck(GetResponseString("VerifyRule"), "action=existingruleset&rid=" & m_iID.ToString() & "&rname=' + input + '")).Append(Environment.NewLine)
        End If
        sbAEJS.Append("function VerifyDelete()" & Environment.NewLine())
        sbAEJS.Append("{" & Environment.NewLine())
        sbAEJS.Append("    var agree=confirm('" & m_refMsg.GetMessage("alt delete this ruleset?") & "');" & Environment.NewLine())
        sbAEJS.Append("    if (agree) {" & Environment.NewLine())
        sbAEJS.Append("	     return true;" & Environment.NewLine())
        sbAEJS.Append("    } else {" & Environment.NewLine())
        sbAEJS.Append("	     return false;" & Environment.NewLine())
        sbAEJS.Append("    }" & Environment.NewLine())
        sbAEJS.Append("}" & Environment.NewLine)
        sbAEJS.Append("function SaveRule()" & Environment.NewLine)
        sbAEJS.Append("{" & Environment.NewLine)
        If actiontype <> "select" Then
            sbAEJS.Append("    var stext = Trim(document.getElementById('txtRulesetName').value);" & Environment.NewLine())
            sbAEJS.Append("    checkRuleset(stext,''); " & Environment.NewLine())
            sbAEJS.Append("    return bexists; " & Environment.NewLine())
            sbAEJS.Append("}" & Environment.NewLine)
            sbAEJS.Append("function VerifyRule()" & Environment.NewLine)
            sbAEJS.Append("{" & Environment.NewLine)
            sbAEJS.Append("    var stext = Trim(document.getElementById('txtRulesetName').value);" & Environment.NewLine())
            sbAEJS.Append("    if (stext.length > 0) {" & Environment.NewLine())
            sbAEJS.Append("    } else {" & Environment.NewLine())
            sbAEJS.Append("    alert('" & m_refMsg.GetMessage("alt a ruleset name is required!") & "');" & Environment.NewLine())
            sbAEJS.Append("    return false;    " & Environment.NewLine())
            sbAEJS.Append("    }" & Environment.NewLine())
            sbAEJS.Append("    if (!CheckRuleSetNameForillegalChar()) {" & Environment.NewLine())
            sbAEJS.Append("         return false;    " & Environment.NewLine())
            sbAEJS.Append("    }" & Environment.NewLine())
            sbAEJS.Append("function CheckRuleSetNameForillegalChar() {" & Environment.NewLine)
            sbAEJS.Append("   var val = document.getElementById('txtRulesetName').value;" & Environment.NewLine)
            sbAEJS.Append("   if ((val.indexOf(""\\"") > 0) || (val.indexOf(""/"") > 0) || (val.indexOf("":"") > 0)||(val.indexOf(""*"") > 0) || (val.indexOf(""?"") > 0)|| (val.indexOf(""\"""") > 0) || (val.indexOf(""<"") > 0)|| (val.indexOf("">"") > 0) || (val.indexOf(""|"") > 0) || (val.indexOf(""&"") > 0) || (val.indexOf(""\'"") > 0))" & Environment.NewLine)
            sbAEJS.Append("   {" & Environment.NewLine)
            sbAEJS.Append("       alert(""" & m_refMsg.GetMessage("alert msg ruleset name cant") & " " & "('\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')."");" & Environment.NewLine)
            sbAEJS.Append("       return false;" & Environment.NewLine)
            sbAEJS.Append("   }" & Environment.NewLine)
            sbAEJS.Append("   return true;" & Environment.NewLine)
            sbAEJS.Append("}" & Environment.NewLine)
        End If
        sbAEJS.Append("    var sactivetext = """";" & Environment.NewLine)
        sbAEJS.Append("    var senabledtext = """";" & Environment.NewLine)
        sbAEJS.Append("    for (i = 0; i < ruleset.length; i++) {" & Environment.NewLine)
        sbAEJS.Append("        if(ruleset[i].active == true) {" & Environment.NewLine)
        sbAEJS.Append("            sactivetext += ruleset[i].id + "","";" & Environment.NewLine)
        sbAEJS.Append("        }else {" & Environment.NewLine)
        sbAEJS.Append("            senabledtext += ruleset[i].id + "","";" & Environment.NewLine)
        sbAEJS.Append("        }" & Environment.NewLine)
        sbAEJS.Append("    }" & Environment.NewLine)
        sbAEJS.Append("    if (sactivetext.substr((sactivetext.length-1),1)) {" & Environment.NewLine)
        sbAEJS.Append("        sactivetext = sactivetext.substr(0,(sactivetext.length-1));" & Environment.NewLine)
        sbAEJS.Append("    }" & Environment.NewLine)
        sbAEJS.Append("    if (senabledtext.substr((senabledtext.length-1),1)) {" & Environment.NewLine)
        sbAEJS.Append("        senabledtext = senabledtext.substr(0,(senabledtext.length-1));" & Environment.NewLine)
        sbAEJS.Append("    }" & Environment.NewLine)
        sbAEJS.Append("    document.getElementById('txtactiverules').value = sactivetext;" & Environment.NewLine())
        sbAEJS.Append("    document.getElementById('txtenabledrules').value = senabledtext;" & Environment.NewLine())
        sbAEJS.Append("    document.forms[0].submit();" & Environment.NewLine)
        sbAEJS.Append("}" & Environment.NewLine)
        sbAEJS.Append("function noenter(e) {" & Environment.NewLine())
        sbAEJS.Append("    if (e && e.keyCode == 13) {" & Environment.NewLine())
        sbAEJS.Append("        return SaveRule(); " & Environment.NewLine())
        sbAEJS.Append("    }" & Environment.NewLine())
        sbAEJS.Append("}" & Environment.NewLine())
        sbAEJS.Append("</script>" & Environment.NewLine)
        txtRulesetName.Attributes.Add("onkeypress", "javascript:return noenter(event);")
        ltrjs.Text = sbAEJS.ToString()
        sbAEJS = Nothing
    End Sub

    Private Sub AddJS()
        Dim sbaddJS As New StringBuilder()
        sbaddJS.Append("<script type=""text/javascript"">" & Environment.NewLine)

        sbaddJS.Append(AJAXcheck(GetResponseString("VerifyAdd"), "action=existingruleset&rid=" & m_iID.ToString() & "&rname=' + input + '")).Append(Environment.NewLine)
        sbaddJS.Append("function VerifyForm()" & Environment.NewLine)
        sbaddJS.Append("{" & Environment.NewLine)
        sbaddJS.Append("    var stext = Trim(document.getElementById('txtRulesetName').value);" & Environment.NewLine())
        sbaddJS.Append("    checkRuleset(stext,''); " & Environment.NewLine())
        sbaddJS.Append("    return bexists; " & Environment.NewLine())
        sbaddJS.Append("}" & Environment.NewLine)

        sbaddJS.Append("function VerifyAdd()" & Environment.NewLine())
        sbaddJS.Append("{" & Environment.NewLine())
        sbaddJS.Append("    var stext = Trim(document.getElementById('txtRulesetName').value);" & Environment.NewLine())
        sbaddJS.Append("    if (stext.length > 0) {" & Environment.NewLine())
        sbaddJS.Append("        if (!CheckRuleSetNameForillegalChar()) {" & Environment.NewLine())
        sbaddJS.Append("           return false;    " & Environment.NewLine())
        sbaddJS.Append("        } else { " & Environment.NewLine())
        sbaddJS.Append("            document.forms[0].submit();" & Environment.NewLine())
        sbaddJS.Append("        } " & Environment.NewLine())
        sbaddJS.Append("    } else {" & Environment.NewLine())
        sbaddJS.Append("    alert('" & m_refMsg.GetMessage("alt a ruleset name is required!") & "');" & Environment.NewLine())
        sbaddJS.Append("    return false;    " & Environment.NewLine())
        sbaddJS.Append("    }" & Environment.NewLine())
        sbaddJS.Append("}" & Environment.NewLine())
        sbaddJS.Append("function CheckRuleSetNameForillegalChar() {" & Environment.NewLine)
        sbaddJS.Append("   var val = document.getElementById('txtRulesetName').value;" & Environment.NewLine)
        sbaddJS.Append("   if ((val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbaddJS.Append("   {" & Environment.NewLine)
        sbaddJS.Append("       alert(""" & m_refMsg.GetMessage("alert msg ruleset name cant") & " " & "('\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')."");" & Environment.NewLine)
        sbaddJS.Append("       return false;" & Environment.NewLine)
        sbaddJS.Append("   }" & Environment.NewLine)
        sbaddJS.Append("   return true;" & Environment.NewLine)
        sbaddJS.Append("}" & Environment.NewLine)
        sbaddJS.Append("function noenter(e) {" & Environment.NewLine())
        sbaddJS.Append("    var iKey = e.keyCode; " & Environment.NewLine())
        sbaddJS.Append("    iKey = iKey + 1; " & Environment.NewLine())
        sbaddJS.Append("    iKey = iKey - 1; " & Environment.NewLine())
        sbaddJS.Append("    if (e && (iKey == 13)) {" & Environment.NewLine())
        sbaddJS.Append("        VerifyForm(); " & Environment.NewLine())
        sbaddJS.Append("        return false; " & Environment.NewLine())
        sbaddJS.Append("    }" & Environment.NewLine())
        sbaddJS.Append("}" & Environment.NewLine())
        sbaddJS.Append("</script>" & Environment.NewLine)
        txtRulesetName.Attributes.Add("onkeypress", "javascript:return noenter(event);")
        ltrjs.Text = sbaddJS.ToString()
        sbaddJS = Nothing
    End Sub

    Private Sub RulesJS()
        Dim sbruleJS As New StringBuilder()
        sbruleJS.Append("            <script type=""text/javascript"">" & Environment.NewLine)
        sbruleJS.Append("            var ruleset = " & Environment.NewLine)
        sbruleJS.Append("            [" & Environment.NewLine)
        If m_aRuleset(0).Rules.Length > 0 Then
            For i As Integer = 0 To (m_aRuleset(0).Rules.Length - 1)
                sbruleJS.Append("	            {" & Environment.NewLine)
                sbruleJS.Append("		            name: """ & m_aRuleset(0).Rules(i).RuleName.Replace("""", "\""") & """," & Environment.NewLine)
                sbruleJS.Append("		            order: " & i.ToString() & "," & Environment.NewLine)
                sbruleJS.Append("		            active: " & m_aRuleset(0).Rules(i).Active.ToString().ToLower() & "," & Environment.NewLine)
                sbruleJS.Append("		            id: " & m_aRuleset(0).Rules(i).RuleID.ToString() & "" & Environment.NewLine)
                If i = (m_aRuleset(0).Rules.Length - 1) Then
                    sbruleJS.Append("	            }" & Environment.NewLine)
                Else
                    sbruleJS.Append("	            }," & Environment.NewLine)
                End If
            Next
        End If
        sbruleJS.Append("            ]" & Environment.NewLine)
        sbruleJS.Append("" & Environment.NewLine)
        sbruleJS.Append("            RuleWizard.displayScreen(""ruleset"", ruleset);" & Environment.NewLine)
        sbruleJS.Append("            </script>" & Environment.NewLine)
        sbruleJS.Append("<script type=""text/javascript"">" & Environment.NewLine)
        sbruleJS.Append("    var filemenu = new Menu( ""file"" );" & Environment.NewLine)
        sbruleJS.Append("    filemenu.addItem(""&nbsp;<img valign='center' src='" & m_refContentApi.AppPath & "images/ui/icons/cogAdd.png" & "' />&nbsp;&nbsp;" & MyBase.GetMessage("btn addnewrule") & """, function() { window.location.href = 'wizard-with-steps.aspx?action=add&rulesetid=" & m_iID.ToString() & "' } );" & Environment.NewLine)
        'sbruleJS.Append("    filemenu.addBreak();" & Environment.NewLine)
        sbruleJS.Append("    filemenu.addItem(""&nbsp;<img valign='center' src='" & m_refContentApi.AppPath & "images/ui/icons/cog.png" & "' />&nbsp;&nbsp;" & MyBase.GetMessage("btn addexistrule") & """, function() { window.location.href = 'ruleset.aspx?action=select&id=" & m_iID.ToString() & "' } );" & Environment.NewLine)
        sbruleJS.Append("" & Environment.NewLine)
        sbruleJS.Append("    MenuUtil.add( filemenu );" & Environment.NewLine)
        sbruleJS.Append("    </script>" & Environment.NewLine)
        sbruleJS.Append("" & Environment.NewLine)

        ltrrulejs.Text &= sbruleJS.ToString()
        sbruleJS = Nothing
    End Sub

    Private Sub GoGet()
        Dim rsNew As RuleSet

        rsNew = New RuleSet(m_refContentApi.RequestInformationRef, m_iID)
        ReDim m_aRuleset(1)
        m_aRuleset(0) = rsNew
    End Sub

    Private Sub GetSelectableRules()
        Dim rsNew As RuleSet

        rsNew = New RuleSet(m_refContentApi.RequestInformationRef)
        ReDim m_aRuleset(1)
        m_aRuleset(0) = rsNew

        m_aRuleset(0).Rules = m_rulesUI.GetSelectableRules(m_iID)
    End Sub

    Private Sub ShowHidden()
        ltrrulesetid.Text = "<input type=""hidden"" id=""rulesetid"" name=""rulesetid"" value=""" & m_iID.ToString() & """ />"
    End Sub

    Private Sub SetAction(ByVal stype As String)
        ltr_action_js.Text = Environment.NewLine & "<script type=""text/javascript"">var s_action = """ & stype.ToLower() & """;</script>"
    End Sub

    Private Function AJAXcheck(ByVal sResponse As String, ByVal sURLQuery As String) As String
        MyBase.AJAX.ResponseJS = sResponse
        MyBase.AJAX.URLQuery = sURLQuery
        MyBase.AJAX.FunctionName = "checkRuleset"
        Return MyBase.AJAX.Render
    End Function

    Private Function GetResponseString(ByVal nextfunction As String) As String
        Dim sbAEJS As New System.Text.StringBuilder
        sbAEJS.Append("    if (response == '1'){").Append(Environment.NewLine)
        sbAEJS.Append("	        alert('This ruleset already exists.');").Append(Environment.NewLine)
        sbAEJS.Append("	        bexists = false;").Append(Environment.NewLine)
        sbAEJS.Append("    }else{").Append(Environment.NewLine)
        sbAEJS.Append("	        bexists = ").Append(nextfunction).Append("();").Append(Environment.NewLine)
        sbAEJS.Append("    } ").Append(Environment.NewLine)
        Return sbAEJS.ToString()
    End Function
End Class
