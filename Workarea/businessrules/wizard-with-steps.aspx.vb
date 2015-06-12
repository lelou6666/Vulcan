Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports Ektron.Cms.RulesEngine
Imports System.Data
Imports System.Xml

Partial Class wizard_with_steps
    Inherits System.Web.UI.Page

    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected m_rulesUI As RulesEngine.UI
    Protected m_rulesengineRule As RulesEngine.Rule
    Protected m_acontem As RulesEngine.Template()
    Protected m_aacttem As RulesEngine.Template()
    Protected m_aruleparam As RulesEngine.TemplateParam() = Nothing
    Protected m_tpacondition As RulesEngine.TemplateParam()
    Protected m_tpaaction As RulesEngine.TemplateParam()
    Protected m_refContentApi As New ContentAPI
    Protected m_strStyleSheetJS As String = ""
    Protected m_sPageAction As String = ""
    Protected m_iID As Long = 0
    Protected m_iRuleID As Long = 0

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = m_refContentApi.EkMsgRef
        AppImgPath = m_refContentApi.AppImgPath
        ltr_Title.Text = m_refMsg.GetMessage("lbl cms business rulesets")

        RegisterResources()
        Try
            If m_refContentApi.RequestInformationRef.IsMembershipUser Or m_refContentApi.RequestInformationRef.UserId = 0 Then
                Response.Redirect("../reterror.aspx?info=" & Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), False)
                Exit Sub
            End If
            If Request.QueryString("action") <> "" Then
                m_sPageAction = Request.QueryString("action").ToLower()
            Else
                m_sPageAction = ""
            End If
            If Request.QueryString("id") <> "" Then
                m_iRuleID = Convert.ToInt64(Request.QueryString("id"))
            End If
            If Request.QueryString("rulesetid") <> "" Then
                m_iID = Request.QueryString("rulesetid")
            End If

            m_rulesUI = New RulesEngine.UI(m_refContentApi.RequestInformationRef)
            m_acontem = m_rulesUI.GetAllConditionTemplates()
            m_aacttem = m_rulesUI.GetAllActionTemplates()
            m_tpacondition = m_rulesUI.GetAllConditionTemplateParams()
            m_tpaaction = m_rulesUI.GetAllActionTemplateParams()

            OutputJS()
            ShowHidden()

            If m_sPageAction = "edit" Then
                m_aruleparam = m_rulesUI.GetRuleParams(m_iRuleID)
                m_rulesengineRule = New RulesEngine.Rule(m_refContentApi.RequestInformationRef)
                m_rulesengineRule.load(m_iRuleID)
                ruleNameText.Value = m_rulesengineRule.RuleName
                Dim ucWizard As cmswizard
                ucWizard = CType(LoadControl("../controls/wizard/wizard.ascx"), cmswizard)
                ucWizard.AllowSelect = True
                ucWizard.ID = "ProgressSteps"
                pnlwizard.Controls.Add(ucWizard)
                BuildTemplateJS("edit")
                ShowRuleToolBar()
            ElseIf m_sPageAction = "view" Then
                m_aruleparam = m_rulesUI.GetRuleParams(m_iRuleID)
                m_rulesengineRule = New RulesEngine.Rule(m_refContentApi.RequestInformationRef)
                m_rulesengineRule.load(m_iRuleID)
                ruleNameText.Value = m_rulesengineRule.RuleName
                BuildTemplateJS("view")
                ShowRuleToolBar()
            ElseIf m_sPageAction = "add" Then
                BuildTemplateJS("add")
                Dim ucWizard As cmswizard
                ucWizard = CType(LoadControl("../controls/wizard/wizard.ascx"), cmswizard)
                ucWizard.AllowSelect = False ' do not allow skip for add
                ucWizard.ID = "ProgressSteps"
                pnlwizard.Controls.Add(ucWizard)
                ShowRuleToolBar()
            ElseIf m_sPageAction = "process" Then
                BuildTemplateJS("add")
                ProcessHandler()
            End If
        Catch ex As Exception
            'Response.Write(ex.Message & ex.StackTrace)
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

    Private Sub ProcessHandler()
        Dim i As Integer = 0
        Dim sRulename As String = "New Rule"
        Dim sLogicalOperator As String = "and"
        Dim aParam As Array
        Dim sTemplateType As String = ""
        Dim iTemplateID As Long = 0
        Dim iOrder As Integer = 0
        Dim sParam As String = ""
        Dim sValue As String = ""
        Dim sXMLdoc As XmlDocument
        Dim xmlelem As XmlElement
        Dim xmlatt As XmlAttribute
        Dim stmpText As String = ""
        Dim bWroteCondition As Boolean = False
        Dim bWroteActionTrue As Boolean = False
        Dim bWroteActionFalse As Boolean = False
        Dim bNeedWriteActionTrue As Boolean = False
        Dim bNeedWriteActionFalse As Boolean = False
        Dim icurTemplateID As Long = -1
        Dim tpTemParam As RulesEngine.TemplateParam

        sRulename = Request.QueryString("rule_name").ToString
        If sRulename.ToString.Length = 0 Then
            Throw New Exception("You need a rule name.")
        End If

        If m_iRuleID > 0 Then 'edit
            m_rulesUI.ClearRuleParams(m_iRuleID) 'clear any params for the rule
            m_rulesengineRule = New RulesEngine.Rule(m_refContentApi.RequestInformationRef)
            m_rulesengineRule.load(m_iRuleID)
            m_rulesengineRule.SetName(sRulename)
        Else 'add
            m_rulesengineRule = New RulesEngine.Rule(sRulename, "", True)
            m_rulesengineRule.Initialize(m_refContentApi.RequestInformationRef)
            m_rulesengineRule.AddAndAssociate(m_iID)
            m_iRuleID = m_rulesengineRule.RuleID
        End If

        sXMLdoc = New XmlDocument()
        For i = 0 To (Request.QueryString.Count - 2)
            If Not (Request.QueryString(i) Is Nothing) Then
                Select Case (Request.QueryString.Keys(i).ToLower().Trim())
                    Case "rule_name"
                        xmlelem = sXMLdoc.CreateElement("rule")
                        xmlatt = sXMLdoc.CreateAttribute("name")
                        xmlatt.Value = sRulename
                        xmlelem.Attributes.Append(xmlatt)
                        sXMLdoc.AppendChild(xmlelem)
                    Case "logical_operator"
                        sLogicalOperator = Request.QueryString(i)
                        xmlelem = sXMLdoc.CreateElement("condition")
                        sXMLdoc.ChildNodes.Item(0).AppendChild(xmlelem)
                        xmlelem = sXMLdoc.CreateElement("predicate")
                        xmlatt = sXMLdoc.CreateAttribute("type")
                        xmlatt.Value = sLogicalOperator.ToLower()
                        xmlelem.Attributes.Append(xmlatt)
                        sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(0).AppendChild(xmlelem)

                        xmlelem = sXMLdoc.CreateElement("actions")
                        xmlatt = sXMLdoc.CreateAttribute("case")
                        xmlatt.Value = "true"
                        xmlelem.Attributes.Append(xmlatt)
                        sXMLdoc.ChildNodes.Item(0).AppendChild(xmlelem)

                        xmlelem = sXMLdoc.CreateElement("actions")
                        xmlatt = sXMLdoc.CreateAttribute("case")
                        xmlatt.Value = "false"
                        xmlelem.Attributes.Append(xmlatt)
                        sXMLdoc.ChildNodes.Item(0).AppendChild(xmlelem)
                    Case Else
                        aParam = Split(Request.QueryString.Keys(i), "_")
                        If aParam.Length = 4 Then
                            sTemplateType = aParam(0)
                            iTemplateID = Convert.ToInt64(aParam(1))
                            iOrder = Convert.ToInt32(aParam(2))
                            sParam = aParam(3)
                            sValue = Replace(Request.QueryString(i), "&", "&amp;")
                            If ValidateValue(sTemplateType, iTemplateID, sParam, sValue) Then
                                Select Case sTemplateType.ToLower()
                                    Case "condition"
                                        tpTemParam = New RulesEngine.TemplateParam(iTemplateID, sParam, sValue, Ektron.Cms.Common.EkEnumeration.CustomAttributeValueTypes.String, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.Condition)
                                        m_rulesUI.AddRuleParam(m_iRuleID, tpTemParam)
                                        If icurTemplateID = -1 Or (icurTemplateID = iTemplateID) Then
                                            If icurTemplateID = -1 Then
                                                FindTemplateIndex(m_acontem, iTemplateID)
                                                stmpText = Replace(m_acontem(FindTemplateIndex(m_acontem, iTemplateID)).Predicate, "<predicate type=", "<predicate template=""" & iTemplateID.ToString() & """ type=")
                                            Else
                                                stmpText = Replace(stmpText, "<predicate type=", "<predicate template=""" & iTemplateID.ToString() & """ type=")
                                            End If
                                            stmpText = Replace(stmpText, "[" & sParam.ToLower() & "]", sValue)
                                        ElseIf Not (icurTemplateID = iTemplateID) Then
                                            'write out
                                            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).InnerXml += stmpText
                                            stmpText = "" 'reset
                                            'get values
                                            stmpText = Replace(m_acontem(FindTemplateIndex(m_acontem, iTemplateID)).Predicate, "<predicate type=", "<predicate template=""" & iTemplateID.ToString() & """ type=")
                                            stmpText = Replace(stmpText, "[" & sParam.ToLower() & "]", sValue)
                                        End If
                                        icurTemplateID = iTemplateID
                                    Case "actiontrue"
                                        tpTemParam = New RulesEngine.TemplateParam(iTemplateID, sParam, sValue, Ektron.Cms.Common.EkEnumeration.CustomAttributeValueTypes.String, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionTrue)
                                        m_rulesUI.AddRuleParam(m_iRuleID, tpTemParam)
                                        bNeedWriteActionTrue = True
                                        If Not (bWroteCondition) Then 'this is the first time we come to the actions
                                            'Write out the conditions
                                            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).InnerXml += stmpText
                                            stmpText = "" 'reset
                                            icurTemplateID = -1
                                            bWroteCondition = True
                                        End If
                                        If icurTemplateID = -1 Or (icurTemplateID = iTemplateID) Then
                                            If icurTemplateID = -1 Then
                                                stmpText = Replace(m_aacttem(FindTemplateIndex(m_aacttem, iTemplateID)).Predicate, "<action type=", "<action template=""" & iTemplateID.ToString() & """ type=")
                                            Else
                                                stmpText = Replace(stmpText, "<action type=", "<action template=""" & iTemplateID.ToString() & """ type=")
                                            End If
                                            stmpText = Replace(stmpText, "[" & sParam.ToLower() & "]", sValue)
                                        ElseIf Not (icurTemplateID = iTemplateID) Then
                                            'write out
                                            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(1).InnerXml += stmpText
                                            'sXMLdoc.ChildNodes.Item(1).InnerXml += stmpText
                                            stmpText = "" 'reset
                                            'get values
                                            stmpText = Replace(m_aacttem(FindTemplateIndex(m_aacttem, iTemplateID)).Predicate, "<action type=", "<action template=""" & iTemplateID.ToString() & """ type=")
                                            stmpText = Replace(stmpText, "[" & sParam.ToLower() & "]", sValue)
                                        End If
                                        icurTemplateID = iTemplateID
                                    Case "actionfalse"
                                        tpTemParam = New RulesEngine.TemplateParam(iTemplateID, sParam, sValue, Ektron.Cms.Common.EkEnumeration.CustomAttributeValueTypes.String, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionFalse)
                                        m_rulesUI.AddRuleParam(m_iRuleID, tpTemParam)
                                        bNeedWriteActionFalse = True
                                        If Not (bWroteCondition) Then
                                            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).InnerXml += stmpText
                                            stmpText = "" 'reset
                                            icurTemplateID = -1
                                            bWroteCondition = True
                                        End If
                                        If (Not (bWroteActionTrue)) And bNeedWriteActionTrue Then
                                            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(1).InnerXml += stmpText
                                            stmpText = "" 'reset
                                            icurTemplateID = -1
                                            bWroteActionTrue = True
                                        End If
                                        If icurTemplateID = -1 Or (icurTemplateID = iTemplateID) Then
                                            If icurTemplateID = -1 Then
                                                stmpText = Replace(m_aacttem(FindTemplateIndex(m_aacttem, iTemplateID)).Predicate, "<action type=", "<action template=""" & iTemplateID.ToString() & """ type=")
                                            Else
                                                stmpText = Replace(stmpText, "<action type=", "<action template=""" & iTemplateID.ToString() & """ type=")
                                            End If
                                            stmpText = Replace(stmpText, "[" & sParam.ToLower() & "]", sValue)
                                        ElseIf Not (icurTemplateID = iTemplateID) Then
                                            'write out
                                            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(2).InnerXml += stmpText
                                            'sXMLdoc.ChildNodes.Item(1).InnerXml += stmpText
                                            stmpText = "" 'reset
                                            'get values
                                            stmpText = Replace(m_aacttem(FindTemplateIndex(m_aacttem, iTemplateID)).Predicate, "<action type=", "<action template=""" & iTemplateID.ToString() & """ type=")
                                            stmpText = Replace(stmpText, "[" & sParam.ToLower() & "]", sValue)
                                        End If
                                        icurTemplateID = iTemplateID
                                End Select
                            End If
                        End If
                End Select
            End If
        Next
        If Not (bWroteCondition) Then
            'Write out the conditions
            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).InnerXml += stmpText
        End If
        If (Not (bWroteActionTrue)) And bNeedWriteActionTrue Then
            'Write out the actiontrue
            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(1).InnerXml += stmpText
        End If
        If bNeedWriteActionFalse Then
            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(2).InnerXml += stmpText
        End If

        m_rulesengineRule.UpdateXML(sXMLdoc.InnerXml.ToString())

        Response.Redirect("ruleset.aspx?action=View&id=" & m_iID.ToString(), False)
    End Sub
    Private Function ValidateValue(ByVal TemplateType As String, ByVal templateID As Long, ByVal ParamName As String, ByVal ParamValue As String) As Boolean
        Dim bret As Boolean = False
        Select Case (TemplateType.ToLower())
            Case "condition"
                For i As Integer = 0 To (m_tpacondition.Length - 1)
                    If (templateID = m_tpacondition(i).TemplateID) And (m_tpacondition(i).ParamName.ToLower() = ParamName.ToLower()) Then
                        bret = m_tpacondition(i).Validate(ParamValue)
                        Exit For
                    End If
                Next
            Case "actiontrue"
                For i As Integer = 0 To (m_tpaaction.Length - 1)
                    If (templateID = m_tpaaction(i).TemplateID) And (m_tpaaction(i).ParamName.ToLower() = ParamName.ToLower()) Then
                        bret = m_tpaaction(i).Validate(ParamValue)
                        Exit For
                    End If
                Next
            Case "actionfalse"
                For i As Integer = 0 To (m_tpaaction.Length - 1)
                    If (templateID = m_tpaaction(i).TemplateID) And (m_tpaaction(i).ParamName.ToLower() = ParamName.ToLower()) Then
                        bret = m_tpaaction(i).Validate(ParamValue)
                        Exit For
                    End If
                Next
        End Select
        Return bret
    End Function
    Private Sub ShowHidden()
        ltrhidden.Text = "<input type=""hidden"" id=""rulesetid"" name=""rulesetid"" value=""" & m_iID.ToString() & """/>" & Environment.NewLine
        ltrhidden.Text += "<input type=""hidden"" id=""action"" name=""action"" value=""" & m_sPageAction.ToLower() & """/>" & Environment.NewLine
        ltrhidden.Text += "<input type=""hidden"" id=""ruleid"" name=""ruleid"" value=""" & m_iRuleID.ToString() & """/>" & Environment.NewLine
    End Sub
    Private Sub ShowRuleToolBar()
        Dim result As New System.Text.StringBuilder
        If Me.m_sPageAction = "view" Then
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl view rule"))
        Else
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar("Add/Edit Rules")
        End If
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "#", m_refMsg.GetMessage("alt Click here to go to the previous step"), m_refMsg.GetMessage("alt Go to the Previous Step"), "onclick=""WizardUtil.showPreviousStep()"" "))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/forward.png", "#", m_refMsg.GetMessage("alt Click here to go to the next step"), m_refMsg.GetMessage("alt Go to the Next Step"), "onclick=""WizardUtil.showNextStep()"" "))
        If Not (Me.m_sPageAction = "view") Then
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", "ruleset.aspx?action=View&id=" & m_iID.ToString(), "Click here to go back", "Go Back", ""))
            result.Append("<td>")
            result.Append(m_refStyle.GetHelpButton("Ruleset"))
            result.Append("</td>")
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("alt Click to close this window"), m_refMsg.GetMessage("alt Close this window"), " onclick=""self.close();"" "))
            result.Append("<td>")
            result.Append(m_refStyle.GetHelpButton("Ruleset_view_wizard"))
            result.Append("</td>")
        End If
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub OutputJS()
        Dim sbJS As New StringBuilder()

        Page.ClientScript.RegisterClientScriptInclude("statement", "includes/com.ektron.ui.statement.js")
        Page.ClientScript.RegisterClientScriptInclude("manager", "includes/com.ektron.sc.manager.js")
        Page.ClientScript.RegisterClientScriptInclude("rules", "includes/com.ektron.sc.rules.js")
        Page.ClientScript.RegisterClientScriptInclude("wizard", "includes/com.ektron.utils.wizard.js")

        sbJS.Append("<link rel='stylesheet' type='text/css' href='css/com.ektron.utils.wizard.css' />" & Environment.NewLine)
        sbJS.Append("<link rel='stylesheet' type='text/css' href='css/com.ektron.rules.wizard.css' />" & Environment.NewLine)
        sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""../tree/js/com.ektron.net.http.js""></script>" & Environment.NewLine)
        sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""../tree/js/com.ektron.utils.cookie.js""></script>" & Environment.NewLine)
        sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""../tree/js/com.ektron.utils.debug.js""></script>" & Environment.NewLine)
        sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""../tree/js/com.ektron.utils.dom.js""></script>" & Environment.NewLine)
        sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""../tree/js/com.ektron.utils.form.js""></script>" & Environment.NewLine)
        sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""../tree/js/com.ektron.utils.log.js""></script>" & Environment.NewLine)
        sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""../tree/js/com.ektron.utils.querystring.js""></script>" & Environment.NewLine)
        sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""../tree/js/com.ektron.utils.string.js""></script>" & Environment.NewLine)
        sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""../tree/js/com.ektron.utils.xml.js""></script>" & Environment.NewLine)
        ' sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""includes/com.ektron.ui.statement.js""></script>" & Environment.NewLine)
        'sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""includes/com.ektron.sc.manager.js""></script>" & Environment.NewLine)
        'sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""includes/com.ektron.sc.rules.js""></script>" & Environment.NewLine)
        'sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""includes/com.ektron.utils.wizard.js""></script>" & Environment.NewLine)

        ltrjs.Text = sbJS.ToString()
        sbJS = Nothing
    End Sub
    Private Sub BuildTemplateJS(ByVal action As String)
        Dim sbTemplateJS As New StringBuilder()
        sbTemplateJS.Append("<script type=""text/javascript"" language=""javascript"">" & Environment.NewLine)
        If action = "view" Then
            sbTemplateJS.Append(" s_action = 'view'; " & Environment.NewLine)
            sbTemplateJS.Append(" document.getElementById('ruleNameText').disabled = true; " & Environment.NewLine)
            sbTemplateJS.Append(" document.getElementById('logicalOperator').disabled = true; " & Environment.NewLine)
            step2.Text = "<span class=""stepLabel"">2. Actions to take when conditions are TRUE.</span>"
            step3.Text = "<span class=""stepLabel"">3. Actions to take when conditions are FALSE.</span>"
            step4.Text = "<span class=""stepLabel"">4. Rule Name.</span>"
        Else
            sbTemplateJS.Append(" s_action = ''; " & Environment.NewLine)
        End If
        GenerateWizardJS(action)
        sbTemplateJS.Append(BuildConditionJS())
        sbTemplateJS.Append(BuildActiontrueJS())
        sbTemplateJS.Append(BuildActionfalseJS())
        sbTemplateJS.Append(BuildConditionParamJS(action))
        sbTemplateJS.Append(BuildActionParamtrueJS(action))
        sbTemplateJS.Append(BuildActionParamfalseJS(action))
        sbTemplateJS.Append("function CheckRuleNameForillegalChar() {" & Environment.NewLine)
        sbTemplateJS.Append("   var val = document.getElementById('ruleNameText').value;" & Environment.NewLine)
        sbTemplateJS.Append("   if ((val.indexOf(""\\"") >= 0) || (val.indexOf(""/"") >= 0) || (val.indexOf("":"") >= 0)||(val.indexOf(""*"") >= 0) || (val.indexOf(""?"") >= 0)|| (val.indexOf(""\"""") >= 0) || (val.indexOf(""<"") >= 0)|| (val.indexOf("">"") >= 0) || (val.indexOf(""|"") >= 0) || (val.indexOf(""&"") >= 0) || (val.indexOf(""\'"") >= 0))" & Environment.NewLine)
        sbTemplateJS.Append("   {" & Environment.NewLine)
        sbTemplateJS.Append("       alert(""" & m_refmsg.GetMessage("msg rule name cannot") & " " & "('\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')."");" & Environment.NewLine)
        sbTemplateJS.Append("       return false;" & Environment.NewLine)
        sbTemplateJS.Append("   }" & Environment.NewLine)
        sbTemplateJS.Append("   return true;" & Environment.NewLine)
        sbTemplateJS.Append("}" & Environment.NewLine)

        sbTemplateJS.Append("</script>" & Environment.NewLine)
        templateJS.Text = sbTemplateJS.ToString()
        sbTemplateJS = Nothing
    End Sub

    Private Function BuildConditionJS() As String
        Dim sbConditionJS As New StringBuilder()
        Dim sTemplate As String = ""
        Dim sPredicate As String = ""
        Dim count As Integer = 0
        sbConditionJS.Append("            var condition_templates =" & Environment.NewLine)
        sbConditionJS.Append("            [" & Environment.NewLine)
        If m_acontem.Length > 0 Then
            For i As Integer = 0 To (m_acontem.Length - 1)
                sbConditionJS.Append("	            {" & Environment.NewLine)
                sbConditionJS.Append("		            id: " & count.ToString() & "," & Environment.NewLine)
                sbConditionJS.Append("		            templateid: " & m_acontem(i).ID.ToString() & "," & Environment.NewLine)
                sbConditionJS.Append("		            name: """ & m_acontem(i).Name.Replace("""", "\""") & """," & Environment.NewLine)
                sTemplate = m_acontem(i).TemplateString.Replace("""", "\""")
                sTemplate = sTemplate.Replace("rulewizardmanager", "RuleWizardManager")
                sTemplate = sTemplate.Replace("showinputform", "showInputForm")
                sTemplate = sTemplate.Replace("{:_:}icount{:_:}", "{:_:}" & count.ToString() & "{:_:}")
                sTemplate = sTemplate.Replace("datainputtext", "dataInputText")
                sbConditionJS.Append("		            template: """ & sTemplate & """," & Environment.NewLine)
                sPredicate = m_acontem(i).Predicate.Replace("""", "\""")
                sPredicate = sPredicate.Replace("{:_:}icount{:_:}", "{:_:}" & count.ToString() & "{:_:}")
                sbConditionJS.Append("		            predicate: """ & sPredicate & """," & Environment.NewLine)
                If TemplateExists(m_acontem(i).ID, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.Condition) Then
                    sbConditionJS.Append("		            active: true" & Environment.NewLine)
                Else
                    sbConditionJS.Append("		            active: false" & Environment.NewLine)
                End If
                sbConditionJS.Append("	            }")
                If i = (m_acontem.Length - 1) Then
                    sbConditionJS.Append(Environment.NewLine)
                Else
                    sbConditionJS.Append("," & Environment.NewLine)
                End If
                count = count + 1
            Next
        End If
        sbConditionJS.Append("            ]" & Environment.NewLine)
        Return sbConditionJS.ToString()
    End Function
    Private Function BuildActiontrueJS() As String
        Dim sTemplate As String = ""
        Dim count As Integer = 0
        Dim sbActionJS As New StringBuilder()
        sbActionJS.Append("            var actiontrue_templates = " & Environment.NewLine)
        sbActionJS.Append("            [" & Environment.NewLine)
        If m_aacttem.Length > 0 Then
            For i As Integer = 0 To (m_aacttem.Length - 1)
                sbActionJS.Append("	            {" & Environment.NewLine)
                sbActionJS.Append("		            id: " & count.ToString() & "," & Environment.NewLine)
                sbActionJS.Append("		            templateid: " & m_aacttem(i).ID.ToString() & "," & Environment.NewLine)
                sbActionJS.Append("		            name: """ & m_aacttem(i).Name.Replace("""", "\""") & """," & Environment.NewLine)
                sTemplate = m_aacttem(i).TemplateString.Replace("""", "\""")
                sTemplate = sTemplate.Replace("action{:_:}", "actiontrue{:_:}")
                sTemplate = sTemplate.Replace("{:_:}icount{:_:}", "{:_:}" & count.ToString() & "{:_:}")
                sbActionJS.Append("		            template: """ & sTemplate & """," & Environment.NewLine)
                If TemplateExists(m_aacttem(i).ID, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionTrue) Then
                    sbActionJS.Append("		            active: true" & Environment.NewLine)
                Else
                    sbActionJS.Append("		            active: false" & Environment.NewLine)
                End If
                sbActionJS.Append("	            }")
                If i = (m_aacttem.Length - 1) Then
                    sbActionJS.Append(Environment.NewLine)
                Else
                    sbActionJS.Append("," & Environment.NewLine)
                End If
                count = count + 1
            Next
        End If
        sbActionJS.Append("            ]" & Environment.NewLine)
        Return sbActionJS.ToString()
    End Function

    Private Function BuildActionfalseJS() As String
        Dim sTemplate As String = ""
        Dim count As Integer = 0
        Dim sbActionJS As New StringBuilder()
        sbActionJS.Append("            var actionfalse_templates = " & Environment.NewLine)
        sbActionJS.Append("            [" & Environment.NewLine)
        If m_aacttem.Length > 0 Then
            For i As Integer = 0 To (m_aacttem.Length - 1)
                sbActionJS.Append("	            {" & Environment.NewLine)
                sbActionJS.Append("		            id: " & count.ToString() & "," & Environment.NewLine)
                sbActionJS.Append("		            templateid: " & m_aacttem(i).ID.ToString() & "," & Environment.NewLine)
                sbActionJS.Append("		            name: """ & m_aacttem(i).Name.Replace("""", "\""") & """," & Environment.NewLine)
                sTemplate = m_aacttem(i).TemplateString.Replace("""", "\""")
                sTemplate = sTemplate.Replace("action{:_:}", "actionfalse{:_:}")
                sTemplate = sTemplate.Replace("{:_:}icount{:_:}", "{:_:}" & count.ToString() & "{:_:}")
                sbActionJS.Append("		            template: """ & sTemplate & """," & Environment.NewLine)
                If TemplateExists(m_aacttem(i).ID, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionFalse) Then
                    sbActionJS.Append("		            active: true" & Environment.NewLine)
                Else
                    sbActionJS.Append("		            active: false" & Environment.NewLine)
                End If
                sbActionJS.Append("	            }")
                If i = (m_aacttem.Length - 1) Then
                    sbActionJS.Append(Environment.NewLine)
                Else
                    sbActionJS.Append("," & Environment.NewLine)
                End If
                count = count + 1
            Next
        End If
        sbActionJS.Append("            ]" & Environment.NewLine)
        Return sbActionJS.ToString()
    End Function

    Public Function BuildConditionParamJS(ByVal type As String) As String
        Dim sbConditionParamJS As New StringBuilder()
        Dim icount As Integer = 0
        Dim sTmpValue As String = ""

        sbConditionParamJS.Append("	       var condition_template_params =" & Environment.NewLine)
        sbConditionParamJS.Append("            [" & Environment.NewLine)
        If m_acontem.Length > 0 And m_tpacondition.Length > 0 Then
            For i As Integer = 0 To (m_acontem.Length - 1)
                sbConditionParamJS.Append("                {" & Environment.NewLine)
                sbConditionParamJS.Append("                    id : " & icount.ToString() & "," & Environment.NewLine)
                sbConditionParamJS.Append("                    params:" & Environment.NewLine)
                sbConditionParamJS.Append("                        [" & Environment.NewLine)
                For j As Integer = 0 To (m_tpacondition.Length - 1)
                    If m_tpacondition(j).TemplateID = m_acontem(i).ID Then
                        sTmpValue = Replace(ObtainValue(m_acontem(i).ID, m_tpacondition(j).ParamName, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.Condition), "&amp;", "&")
                        If sTmpValue <> "" Then
                            sbConditionParamJS.Append("                            { """ & m_tpacondition(j).ParamName & """: """ & sTmpValue & """ }," & Environment.NewLine)
                        Else
                            sbConditionParamJS.Append("                            { """ & m_tpacondition(j).ParamName & """: """ & m_tpacondition(j).DefaultValue & """ }," & Environment.NewLine)
                        End If
                    End If
                Next
                sbConditionParamJS.Append("                        ]" & Environment.NewLine)
                sbConditionParamJS.Append("                }")
                If i = (m_acontem.Length - 1) Then
                    sbConditionParamJS.Append(Environment.NewLine)
                Else
                    sbConditionParamJS.Append("," & Environment.NewLine)
                End If
                icount = icount + 1
            Next
        End If
        sbConditionParamJS.Append("            ]" & Environment.NewLine)
        Return sbConditionParamJS.ToString()
    End Function

    Public Function BuildActionParamtrueJS(ByVal type As String) As String
        Dim sbConditionParamJS As New StringBuilder()
        Dim icount As Integer = 0
        Dim sTmpValue As String = ""

        sbConditionParamJS.Append("	       var actiontrue_template_params =" & Environment.NewLine)
        sbConditionParamJS.Append("            [" & Environment.NewLine)
        If m_acontem.Length > 0 And m_tpaaction.Length > 0 Then
            For i As Integer = 0 To (m_aacttem.Length - 1)
                sbConditionParamJS.Append("                {" & Environment.NewLine)
                sbConditionParamJS.Append("                    id : " & icount.ToString() & "," & Environment.NewLine)
                sbConditionParamJS.Append("                    params:" & Environment.NewLine)
                sbConditionParamJS.Append("                        [" & Environment.NewLine)
                For j As Integer = 0 To (m_tpaaction.Length - 1)
                    If m_tpaaction(j).TemplateID = m_aacttem(i).ID Then
                        sTmpValue = Replace(ObtainValue(m_aacttem(i).ID, m_tpaaction(j).ParamName, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionTrue), "&amp;", "&")
                        If sTmpValue <> "" Then
                            sbConditionParamJS.Append("                            { """ & m_tpaaction(j).ParamName & """: """ & sTmpValue & """ }," & Environment.NewLine)
                        Else
                            sbConditionParamJS.Append("                            { """ & m_tpaaction(j).ParamName & """: """ & m_tpaaction(j).DefaultValue & """ }," & Environment.NewLine)
                        End If
                    End If
                Next
                sbConditionParamJS.Append("                        ]" & Environment.NewLine)
                sbConditionParamJS.Append("                }")
                If i = (m_aacttem.Length - 1) Then
                    sbConditionParamJS.Append(Environment.NewLine)
                Else
                    sbConditionParamJS.Append("," & Environment.NewLine)
                End If
                icount = icount + 1
            Next
        End If
        sbConditionParamJS.Append("            ]" & Environment.NewLine)
        Return sbConditionParamJS.ToString()
    End Function

    Public Function BuildActionParamfalseJS(ByVal type As String) As String
        Dim sbConditionParamJS As New StringBuilder()
        Dim icount As Integer = 0
        Dim sTmpValue As String = ""

        sbConditionParamJS.Append("	       var actionfalse_template_params =" & Environment.NewLine)
        sbConditionParamJS.Append("            [" & Environment.NewLine)
        If m_acontem.Length > 0 And m_tpaaction.Length > 0 Then
            For i As Integer = 0 To (m_aacttem.Length - 1)
                sbConditionParamJS.Append("                {" & Environment.NewLine)
                sbConditionParamJS.Append("                    id : " & icount.ToString() & "," & Environment.NewLine)
                sbConditionParamJS.Append("                    params:" & Environment.NewLine)
                sbConditionParamJS.Append("                        [" & Environment.NewLine)
                For j As Integer = 0 To (m_tpaaction.Length - 1)
                    If m_tpaaction(j).TemplateID = m_aacttem(i).ID Then
                        sTmpValue = Replace(ObtainValue(m_aacttem(i).ID, m_tpaaction(j).ParamName, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionFalse), "&amp;", "&")
                        If sTmpValue <> "" Then
                            sbConditionParamJS.Append("                            { """ & m_tpaaction(j).ParamName & """: """ & sTmpValue & """ }," & Environment.NewLine)
                        Else
                            sbConditionParamJS.Append("                            { """ & m_tpaaction(j).ParamName & """: """ & m_tpaaction(j).DefaultValue & """ }," & Environment.NewLine)
                        End If
                    End If
                Next
                sbConditionParamJS.Append("                        ]" & Environment.NewLine)
                sbConditionParamJS.Append("                }")
                If i = (m_aacttem.Length - 1) Then
                    sbConditionParamJS.Append(Environment.NewLine)
                Else
                    sbConditionParamJS.Append("," & Environment.NewLine)
                End If
                icount = icount + 1
            Next
        End If
        sbConditionParamJS.Append("            ]" & Environment.NewLine)
        Return sbConditionParamJS.ToString()
    End Function

    Private Function FindTemplateIndex(ByVal TemplateArray As Template(), ByVal iTemplateID As Long) As Integer
        Dim iret As Integer = 0
        For i As Integer = 0 To (TemplateArray.Length - 1)
            If iTemplateID = TemplateArray(i).ID Then
                iret = i
                Exit For
            End If
        Next
        Return iret
    End Function
    Private Function TemplateExists(ByVal templateid As Long, ByVal type As Ektron.Cms.Common.EkEnumeration.RuleTemplateType) As Boolean
        Dim bRet As Boolean = False
        If m_aruleparam Is Nothing Then
            'return false
        Else
            For i As Integer = 0 To (m_aruleparam.Length - 1)
                If ((m_aruleparam(i).TemplateID = templateid) And (m_aruleparam(i).TemplateType = type)) Then
                    bRet = True
                End If
            Next
        End If
        Return bRet
    End Function
    Private Function ObtainValue(ByVal TemplateID As Long, ByVal ParamName As String, ByVal type As Ektron.Cms.Common.EkEnumeration.RuleTemplateType) As String
        Dim sRet As String = ""
        ParamName = ParamName.ToLower().Trim()
        If m_aruleparam Is Nothing Then
            'return empty
        Else
            For i As Integer = 0 To (m_aruleparam.Length - 1)
                If ((m_aruleparam(i).TemplateID = TemplateID) And (m_aruleparam(i).TemplateType = type) And (m_aruleparam(i).ParamName.ToLower().Trim() = ParamName)) Then
                    sRet = m_aruleparam(i).DefaultValue
                End If
            Next
        End If
        Return sRet
    End Function

    Private Sub GenerateWizardJS(ByVal type As String)
        Dim sbWizard As New StringBuilder()
        sbWizard.Append("       <script type=""text/javascript"" language=""javascript"">").Append(Environment.NewLine)
        sbWizard.Append("").Append(Environment.NewLine)
        sbWizard.Append("            StatementWizard.displayScreen();   ").Append(Environment.NewLine)
        sbWizard.Append("").Append(Environment.NewLine)
        If Not (type = "view") Then
            sbWizard.Append("var oProgressSteps = new ProgressSteps;").Append(Environment.NewLine)
            sbWizard.Append("").Append(Environment.NewLine)
            sbWizard.Append("oProgressSteps.maxSteps = 4;").Append(Environment.NewLine)
            sbWizard.Append("").Append(Environment.NewLine)
            sbWizard.Append("oProgressSteps.define([").Append(Environment.NewLine)
            sbWizard.Append("      { id:""condition"",	    title:""" & m_refMsg.GetMessage("lbl set conditions") & """,			    description:""" & m_refMsg.GetMessage("msg conditions match") & """ }").Append(Environment.NewLine)
            sbWizard.Append("    , { id:""actiontrue"",	title:""" & m_refMsg.GetMessage("lbl set actions for true") & """,		description:""<font color=\""green\"">" & m_refMsg.GetMessage("msg action true") & "</font>"" }").Append(Environment.NewLine)
            sbWizard.Append("    , { id:""actionfalse"",	title:""" & m_refMsg.GetMessage("lbl set actions for false") & """,		description:""<font color=\""red\"">" & m_refMsg.GetMessage("msg action false") & "</font>"" }").Append(Environment.NewLine)
            sbWizard.Append("    , { id:""rulename"",              title:""" & m_refMsg.GetMessage("lbl assign name") & """,		        description:""" & m_refMsg.GetMessage("msg name meaningful") & """ }").Append(Environment.NewLine)
            sbWizard.Append("    ]);").Append(Environment.NewLine)
            sbWizard.Append("").Append(Environment.NewLine)
            sbWizard.Append("if (""object"" == typeof oProgressSteps && oProgressSteps != null)").Append(Environment.NewLine)
            sbWizard.Append("{").Append(Environment.NewLine)
            sbWizard.Append("	oProgressSteps.onselect = function(stepNumber)").Append(Environment.NewLine)
            sbWizard.Append("	{").Append(Environment.NewLine)
            sbWizard.Append("		switch (this.getStep(stepNumber).id)").Append(Environment.NewLine)
            sbWizard.Append("		{").Append(Environment.NewLine)
            sbWizard.Append("		case ""condition"":").Append(Environment.NewLine)
            sbWizard.Append("		    WizardUtil.setStep(1);").Append(Environment.NewLine)
            sbWizard.Append("            WizardUtil.showStep(1);").Append(Environment.NewLine)
            sbWizard.Append("			break;").Append(Environment.NewLine)
            sbWizard.Append("		case ""actiontrue"":").Append(Environment.NewLine)
            sbWizard.Append("			WizardUtil.setStep(2);").Append(Environment.NewLine)
            sbWizard.Append("            WizardUtil.showStep(2);").Append(Environment.NewLine)
            sbWizard.Append("			break;").Append(Environment.NewLine)
            sbWizard.Append("		case ""actionfalse"":").Append(Environment.NewLine)
            sbWizard.Append("		    WizardUtil.setStep(3);").Append(Environment.NewLine)
            sbWizard.Append("            WizardUtil.showStep(3);").Append(Environment.NewLine)
            sbWizard.Append("			break;").Append(Environment.NewLine)
            sbWizard.Append("		case ""rulename"":").Append(Environment.NewLine)
            sbWizard.Append("		    WizardUtil.setStep(4);").Append(Environment.NewLine)
            sbWizard.Append("            WizardUtil.showStep(4);").Append(Environment.NewLine)
            sbWizard.Append("			break;").Append(Environment.NewLine)
            sbWizard.Append("		default:").Append(Environment.NewLine)
            sbWizard.Append("			break;").Append(Environment.NewLine)
            sbWizard.Append("		}").Append(Environment.NewLine)
            sbWizard.Append("	}").Append(Environment.NewLine)
            sbWizard.Append("	oProgressSteps.oncancel = function()").Append(Environment.NewLine)
            sbWizard.Append("	{").Append(Environment.NewLine)
            sbWizard.Append("		var objAElem = document.getElementById(""image_link_101"");").Append(Environment.NewLine)
            sbWizard.Append("		if (objAElem)").Append(Environment.NewLine)
            sbWizard.Append("		{").Append(Environment.NewLine)
            sbWizard.Append("			location.href = objAElem.href;").Append(Environment.NewLine)
            sbWizard.Append("		}").Append(Environment.NewLine)
            sbWizard.Append("	}").Append(Environment.NewLine)
            sbWizard.Append("}").Append(Environment.NewLine)
        End If
        sbWizard.Append("</script>").Append(Environment.NewLine)
        ltr_wizardjs.Text = sbWizard.ToString()
    End Sub
    Private Sub RegisterResources()
        ' register JS
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronJFunctJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronStyleHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)

        ' register CSS
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.AllIE)
    End Sub
End Class
