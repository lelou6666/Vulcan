Imports Ektron.Cms
Imports Ektron.Cms.Workarea

Partial Class AD_ADDomains
    Inherits workareabase

    Private DefinedDomains As ADDomain()
    Private eUser As User.EkUser
    Protected setting_data As SettingsData = Nothing
    Protected m_refSiteApi As New SiteAPI
    Protected m_refUserApi As New UserAPI
    Protected AdValid As Boolean = False

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        RegisterCss()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1
      
        eUser = m_refContentApi.EkUserRef

        CheckAccess()
        ADCheck()

        If AdValid = True Then
            SetLabels(Me.m_sPageAction)
            Select Case (Me.m_sPageAction)
                Case "edit"
                    If Page.IsPostBack Then
                        ProcessDomains()
                    Else
                        'm_refSiteApi = New SiteAPI
                        'setting_data = m_refSiteApi.GetSiteVariables()
                        DefinedDomains = eUser.GetDomainsAdvanced()
                        EditDomains()
                    End If
                Case Else
                    DefinedDomains = eUser.GetDomainsAdvanced()
                    ShowDomains()
            End Select
        Else
            SetLabels("error")
        End If
    End Sub

    Private Sub CheckAccess()
        Utilities.ValidateUserLogin()
        If Not m_refContentApi.IsAdmin() AndAlso m_refContentApi.RequestInformationRef.UserId <> Ektron.Cms.Common.EkConstants.BuiltIn Then
            HttpContext.Current.Response.Redirect(m_refContentApi.AppPath & "reterror.aspx?info=" & m_refContentApi.EkMsgRef.GetMessage("msg login cms administrator"), False)
            Exit Sub
        End If
    End Sub

    Private Sub ADCheck()
        setting_data = m_refSiteApi.GetSiteVariables(m_refUserApi.UserId)
        AdValid = setting_data.AdValid 'CBool(siteVars("AdValid"))
        If (Not (AdValid)) Then
            lbl_msg.Visible = True
            lbl_msg.Text = MyBase.GetMessage("entrprise license with AD required msg")
        End If
    End Sub

    Private Sub SetLabels(ByVal type As String)
        If type.ToLower() = "error" Then
            MyBase.Title = MyBase.GetMessage("adconfig page title")
            MyBase.SetTitleBarToMessage("adconfig page title")
        ElseIf type.ToLower() = "edit" Then
            MyBase.Title = MyBase.GetMessage("generic edit ad domains")
            MyBase.SetTitleBarToMessage("generic edit ad domains")

            MyBase.AddButtonwithMessages(m_refContentApi.AppImgPath & "../UI/Icons/save.png", "#", "lbl alt save ad domains", "btn save", " onclick=""javascript:return SubmitForm();"" ")
            MyBase.AddBackButton("ADDomains.aspx")
        Else
            MyBase.Title = MyBase.GetMessage("generic ad domains")
            MyBase.SetTitleBarToMessage("generic ad domains")
            MyBase.AddButtonwithMessages(m_refContentApi.AppImgPath & "../UI/Icons/contentEdit.png", "ADDomains.aspx?action=edit", "lbl alt edit ad domains", "btn edit", "")
        End If
        If Not (type.ToLower() = "error") Then
            MyBase.AddHelpButton("ADDomains")
        End If
    End Sub

    Private Sub ProcessDomains()
        Dim iLength As Integer = 0
        Dim adTMP As ADDomain = Nothing
        Dim alDomainList As New ArrayList()
        Dim aRet As ADDomain() = Array.CreateInstance(GetType(ADDomain), 0)
        iLength = CInt(Request.Form("Domainlength"))
        If iLength > 0 Then
            For i As Integer = 0 To (iLength - 1)
                Dim aDName As String()
                adTMP = New ADDomain()
                adTMP.ID = CLng(Request.Form("domain_iden" & i.ToString()))
                'adTMP.DomainShortName = Request.Form("domain_name" & i.ToString())
                adTMP.DomainDNS = Request.Form("domain_dns" & i.ToString())
                aDName = Split(adTMP.DomainDNS, ".")
                If aDName.Length > 0 Then
                    ' use first
                    adTMP.DomainShortName = aDName(0)
                Else
                    adTMP.DomainShortName = adTMP.DomainDNS
                End If
                adTMP.DomainPath = Ektron.Cms.Common.EkConstants.CreateADsPathFromDomain(adTMP.DomainDNS)
                If Request.Form("use_name" & i.ToString()) <> "" Then
                    adTMP.NetBIOS = adTMP.DomainShortName
                Else
                    If Request.Form("netbios" & i.ToString()) <> "" Then
                        adTMP.NetBIOS = Request.Form("netbios" & i.ToString())
                    Else
                        adTMP.NetBIOS = adTMP.DomainShortName
                    End If
                End If
                adTMP.Username = Request.Form("user_name" & i.ToString())
                adTMP.Password = Request.Form("password" & i.ToString()).Trim()
                adTMP.ServerIP = Request.Form("server_ip" & i.ToString())
                alDomainList.Add(adTMP)
            Next
            aRet = alDomainList.ToArray(GetType(ADDomain))
        End If

        eUser.UpdateDomainsAdvanced(aRet)

        Response.Redirect("ADDomains.aspx", False)
    End Sub

    Private Sub EditDomains()
        RenderJS()
        Dim sbContent As New StringBuilder()
        sbContent.Append("<a href=""javascript:addDomain()"">" & m_refMsg.GetMessage("lbl Add New Domain") & "</a>&nbsp;|&nbsp;<a href=""javascript:deleteDomain()"">" & m_refMsg.GetMessage("lbl Remove Last Domain") & "</a><p id=""parah""></p>")
        sbContent.Append("<input type=""hidden"" id=""Domainlength"" name=""Domainlength"" value=""" & DefinedDomains.Length.ToString() & """ />")
        sbContent.Append("<div id=""pDomain"" name=""pDomain"">" & Environment.NewLine)
        sbContent.Append("<table class=""ektronForm"">" & Environment.NewLine)
        For i As Integer = 0 To (DefinedDomains.Length - 1)

            'remove link
            sbContent.Append("<tr>" & Environment.NewLine)
            sbContent.Append("<td colspan=""2"">")
            sbContent.Append("<a href=""#"" onclick=""javascript:removeDomain('" + i.ToString() + "');"">" & m_refMsg.GetMessage("lbl Remove Domain") & "</a>")
            sbContent.Append(Environment.NewLine)
            sbContent.Append("<script language=""javascript"" type=""text/javascript"">addDomainInit(" & DefinedDomains(i).ID.ToString() & ",'" & DefinedDomains(i).DomainDNS & "','" & DefinedDomains(i).NetBIOS & "','" & DefinedDomains(i).Username & "','          ','" & DefinedDomains(i).ServerIP & "');</script>")
            sbContent.Append(Environment.NewLine)
            sbContent.Append("<input type=""hidden"" name=""domain_iden" + i.ToString() + """ id=""domain_iden" + i.ToString() + """ value=""" & DefinedDomains(i).ID.ToString() & """ /> ")

            'Link Name
            'sbContent.Append("<tr>" & Environment.NewLine)
            'sbContent.Append("<td class=""label"" width=""160"">" & Environment.NewLine)
            'sbContent.Append(GetMessage("lbl ad domain name") & ":")
            'sbContent.Append("</td><td>" & Environment.NewLine)
            'sbContent.Append("<input name=""domain_name" + i.ToString() + """ type=""text"" value=""" + Server.HtmlEncode(DefinedDomains(i).DomainShortName) + """ id=""domain_name" + i.ToString() + """ onChange=""javascript:saveDomain(" + i.ToString() + ",this.value,'dname')"" />")
            'sbContent.Append("</td></tr>" & Environment.NewLine)

            'Short Description
            sbContent.Append("<tr>" & Environment.NewLine)
            sbContent.Append("<td class=""label"">" & Environment.NewLine)
            sbContent.Append(GetMessage("lbl ad domain dns") & ":")
            sbContent.Append("</td>")
            sbContent.Append("<td class=""value"">" & Environment.NewLine)
            sbContent.Append("<input class=""ektronTextMedium"" name=""domain_dns" + i.ToString() + """ type=""text"" value=""" + Server.HtmlEncode(DefinedDomains(i).DomainDNS) + """ id=""domain_dns" + i.ToString() + """ onChange=""javascript:saveDomain(" + i.ToString() + ",this.value,'dns')"" />")
            sbContent.Append("</td>")
            sbContent.Append("</tr>" & Environment.NewLine)

            'Relationship
            sbContent.Append("<tr>" & Environment.NewLine)
            sbContent.Append("<td class=""label"">")
            sbContent.Append(GetMessage("lbl ad netbios") & ":")
            sbContent.Append("</td>")
            sbContent.Append("<td class=""value"">")
            If DefinedDomains(i).NetBIOS.Trim().ToLower() = DefinedDomains(i).DomainShortName.Trim().ToLower() Then
                sbContent.Append("<input class=""ektronTextMedium"" disabled name=""netbios" + i.ToString() + """ type=""text"" value=""" + Server.HtmlEncode(DefinedDomains(i).NetBIOS) + """ id=""netbios" + i.ToString() + """ onChange=""javascript:saveDomain(" + i.ToString() + ",this.value,'netbios')"" />")
                sbContent.Append(Environment.NewLine)
                sbContent.Append("<div class=""ektronCaption"">")
                sbContent.Append("<input type=""checkbox"" name=""use_name" & ID.ToString() & """ id=""use_name" & ID.ToString() & """ checked onclick=""(document.getElementById('netbios" + i.ToString() + "').disabled)=(!(document.getElementById('netbios" + i.ToString() + "').disabled));"" />")
            Else
                sbContent.Append("<input class=""ektronTextMedium"" name=""netbios" + i.ToString() + """ type=""text"" value=""" + Server.HtmlEncode(DefinedDomains(i).NetBIOS) + """ id=""netbios" + i.ToString() + """ onChange=""javascript:saveDomain(" + i.ToString() + ",this.value,'netbios')"" />")
                sbContent.Append(Environment.NewLine)
                sbContent.Append("<div class=""ektronCaption"">")
                sbContent.Append("<input type=""checkbox"" name=""use_name" & ID.ToString() & """ id=""use_name" & ID.ToString() & """ onclick=""(document.getElementById('netbios" + i.ToString() + "').disabled)=(!(document.getElementById('netbios" + i.ToString() + "').disabled));"" />")
            End If
            sbContent.Append(GetMessage("lbl ad use domainname"))
            sbContent.Append("</div>")
            sbContent.Append("</td>")
            sbContent.Append("</tr>" & Environment.NewLine)
            '
            sbContent.Append("<tr>" & Environment.NewLine)
            sbContent.Append("<td class=""label"">")
            sbContent.Append(GetMessage("generic username") & ":")
            sbContent.Append("</td>")
            sbContent.Append("<td class=""value"">")
            sbContent.Append("<input class=""ektronTextMedium"" name=""user_name" + i.ToString() + """ type=""text"" value='" + Server.HtmlEncode(DefinedDomains(i).Username) + "' id=""user_name" + i.ToString() + """ onChange=""javascript:saveDomain(" + i.ToString() + ",this.value,'uname')"" /> <span class=""ektronCaption"">ex: name@domain.com</span>")
            sbContent.Append("</td>")
            sbContent.Append("</tr>" & Environment.NewLine)
            '
            sbContent.Append("<tr>" & Environment.NewLine)
            sbContent.Append("<td class=""label"">")
            sbContent.Append(GetMessage("password label") & "</b>")
            sbContent.Append("</td>")
            sbContent.Append("<td class=""value"">")
            sbContent.Append("<input class=""ektronTextMedium"" name=""password" + i.ToString() + """ type=""password"" value=""          "" id=""password" + i.ToString() + """ onChange=""javascript:saveDomain(" + i.ToString() + ",this.value,'pwd')"" />")
            sbContent.Append("</td>")
            sbContent.Append("</tr>" & Environment.NewLine)
            '
            sbContent.Append("<tr>" & Environment.NewLine)
            sbContent.Append("<td class=""label"">")
            sbContent.Append(GetMessage("lbl ad serverip") & ":")
            sbContent.Append("</td>")
            sbContent.Append("<td class=""value"">")
            sbContent.Append("<input class=""ektronTextXXSmall"" name=""server_ip" + i.ToString() + """ type=""text"" value=""" + Server.HtmlEncode(DefinedDomains(i).ServerIP) + """ id=""server_ip" + i.ToString() + """ onChange=""javascript:saveDomain(" + i.ToString() + ",this.value,'sip')"" />")
            sbContent.Append("</td>")
            sbContent.Append("</tr>" & Environment.NewLine)

            ' horizontal rule
            sbContent.Append("<tr>" & Environment.NewLine)
            sbContent.Append("<td colspan=""2"">")
            sbContent.Append("<hr/>")
            sbContent.Append("</td>")
            sbContent.Append("</tr>" & Environment.NewLine)
        Next
        sbContent.Append("</table>" & Environment.NewLine)
        sbContent.Append("</div>" & Environment.NewLine)
        lbl_add_domain.Text = sbContent.ToString()
    End Sub

    Private Sub ShowDomains()
        If DefinedDomains.Length > 0 Then
            Dim tRoll As New Table
            tRoll.CssClass = "ektronForm"
            Dim tRollRow As New TableRow
            Dim tRollCell As New TableCell

            tRollCell = New TableCell
            For i As Integer = 0 To DefinedDomains.Length - 1
                'Link Name
                'tRollCell = New TableCell
                'tRollRow = New TableRow
                'tRollCell.HorizontalAlign = HorizontalAlign.Right
                'tRollCell.Text = GetMessage("lbl ad domain name") & ":"
                'tRollCell.Width = Unit.Pixel(160)
                'tRollRow.Controls.Add(tRollCell)
                'tRollCell = New TableCell
                'tRollCell.Text = DefinedDomains(i).DomainShortName
                'tRollRow.Controls.Add(tRollCell)
                'tRoll.Controls.Add(tRollRow)
                'Short Description
                tRollCell = New TableCell
                tRollRow = New TableRow
                tRollCell.CssClass = "label"
                tRollCell.Text = GetMessage("lbl ad domain dns") & ":"
                tRollRow.Controls.Add(tRollCell)
                tRollCell = New TableCell
                tRollCell.Text = DefinedDomains(i).DomainDNS
                tRollRow.Controls.Add(tRollCell)
                tRoll.Controls.Add(tRollRow)
                'Relationship
                tRollCell = New TableCell
                tRollRow = New TableRow
                tRollCell.CssClass = "label"
                tRollCell.VerticalAlign = VerticalAlign.Top
                tRollCell.Text = GetMessage("lbl ad netbios") & ":"
                tRollRow.Controls.Add(tRollCell)
                tRollCell = New TableCell
                tRollCell.Text = DefinedDomains(i).NetBIOS
                tRollRow.Controls.Add(tRollCell)
                tRoll.Controls.Add(tRollRow)
                '
                tRollCell = New TableCell
                tRollRow = New TableRow
                tRollCell.CssClass = "label"
                tRollCell.Text = GetMessage("generic username") & ":"
                tRollRow.Controls.Add(tRollCell)
                tRollCell = New TableCell
                tRollCell.Text = DefinedDomains(i).Username
                tRollRow.Controls.Add(tRollCell)
                tRoll.Controls.Add(tRollRow)
                '
                tRollCell = New TableCell
                tRollRow = New TableRow
                tRollCell.CssClass = "label"
                tRollCell.Text = GetMessage("lbl ad serverip") & ":"
                tRollRow.Controls.Add(tRollCell)
                tRollCell = New TableCell
                tRollCell.Text = DefinedDomains(i).ServerIP
                tRollRow.Controls.Add(tRollCell)
                tRoll.Controls.Add(tRollRow)

                ' horizontal rule
                tRollCell = New TableCell
                tRollRow = New TableRow
                tRollCell.Text = "<hr/>"
                tRollCell.ColumnSpan = 2
                tRollRow.Controls.Add(tRollCell)
                tRoll.Controls.Add(tRollRow)
            Next
            lbl_add_domain.Controls.Add(tRoll)
        End If
    End Sub

    Private Sub RenderJS()
        Dim sbJS As New System.Text.StringBuilder()

        sbJS.Append("/// AD Domains").Append(Environment.NewLine)
        sbJS.Append("        ").Append(Environment.NewLine)
        sbJS.Append("        var arrDomainID = new Array(0);").Append(Environment.NewLine)
        sbJS.Append("        var arrDomain = new Array(0);").Append(Environment.NewLine)
        sbJS.Append("        // var arrDomainValue = new Array(0);").Append(Environment.NewLine)
        sbJS.Append("        var arrDomainDNS = new Array(0);").Append(Environment.NewLine)
        sbJS.Append("        var arrDomainNetBIOS = new Array(0);").Append(Environment.NewLine)
        sbJS.Append("        var arrDomainUsername = new Array(0);").Append(Environment.NewLine)
        sbJS.Append("        var arrDomainPassword = new Array(0);").Append(Environment.NewLine)
        sbJS.Append("        var arrDomainServerIP = new Array(0);").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function addDomain() {").Append(Environment.NewLine)
        sbJS.Append("          arrDomainID.push(""0"");").Append(Environment.NewLine)
        sbJS.Append("          arrDomain.push(arrDomain.length);").Append(Environment.NewLine)
        sbJS.Append("          // arrDomainValue.push("""");").Append(Environment.NewLine)
        sbJS.Append("          arrDomainDNS.push("""");").Append(Environment.NewLine)
        sbJS.Append("          arrDomainNetBIOS.push("""");").Append(Environment.NewLine)
        sbJS.Append("          arrDomainUsername.push("""");").Append(Environment.NewLine)
        sbJS.Append("          arrDomainPassword.push("""");").Append(Environment.NewLine)
        sbJS.Append("          arrDomainServerIP.push("""");").Append(Environment.NewLine)
        sbJS.Append("          displayDomain();").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("          function addDomainInit(did,dns,netbios,uname,pwd,sip) {").Append(Environment.NewLine)
        sbJS.Append("          arrDomainID.push(did);").Append(Environment.NewLine)
        sbJS.Append("          arrDomain.push(arrDomain.length);").Append(Environment.NewLine)
        sbJS.Append("          // arrDomainValue.push(dname);").Append(Environment.NewLine)
        sbJS.Append("          arrDomainDNS.push(dns);").Append(Environment.NewLine)
        sbJS.Append("          arrDomainNetBIOS.push(netbios);").Append(Environment.NewLine)
        sbJS.Append("          arrDomainUsername.push(uname);").Append(Environment.NewLine)
        sbJS.Append("          arrDomainPassword.push(pwd);").Append(Environment.NewLine)
        sbJS.Append("          arrDomainServerIP.push(sip);").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function displayDomain() {").Append(Environment.NewLine)
        sbJS.Append("          var sItem = '';").Append(Environment.NewLine)
        sbJS.Append("          var sList = '';").Append(Environment.NewLine)
        sbJS.Append("          document.getElementById('pDomain').innerHTML='';").Append(Environment.NewLine)
        sbJS.Append("          for (intI = 0; intI < arrDomain.length; intI++) {").Append(Environment.NewLine)
        sbJS.Append("            sItem = createDomain(arrDomainID[intI], arrDomain[intI], arrDomainDNS[intI], arrDomainNetBIOS[intI], arrDomainUsername[intI], arrDomainPassword[intI], arrDomainServerIP[intI]);").Append(Environment.NewLine)
        sbJS.Append("            sList += sItem;").Append(Environment.NewLine)
        sbJS.Append("          }").Append(Environment.NewLine)
        sbJS.Append("          document.getElementById('pDomain').innerHTML = sList;").Append(Environment.NewLine)
        sbJS.Append("          document.getElementById('Domainlength').value = arrDomain.length;").Append(Environment.NewLine)
        sbJS.Append("          FixPass();").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function saveDomain(intId,strValue,type) {").Append(Environment.NewLine)
        ' sbJS.Append("            alert(strValue + '-' + type); ").Append(Environment.NewLine)
        sbJS.Append("            if (type == ""dname"") {").Append(Environment.NewLine)
        sbJS.Append("                // arrDomainValue[intId]=strValue;").Append(Environment.NewLine)
        sbJS.Append("            }else if (type == ""dns"") {").Append(Environment.NewLine)
        sbJS.Append("                arrDomainDNS[intId]=strValue;").Append(Environment.NewLine)
        sbJS.Append("            }else if (type == ""netbios"") {").Append(Environment.NewLine)
        sbJS.Append("                arrDomainNetBIOS[intId]=strValue;").Append(Environment.NewLine)
        sbJS.Append("            }else if (type == ""uname"") {").Append(Environment.NewLine)
        sbJS.Append("                arrDomainUsername[intId]=strValue;").Append(Environment.NewLine)
        sbJS.Append("            }else if (type == ""pwd"") {").Append(Environment.NewLine)
        sbJS.Append("                arrDomainPassword[intId]=strValue;").Append(Environment.NewLine)
        sbJS.Append("            }else if (type == ""sip"") {").Append(Environment.NewLine)
        sbJS.Append("                arrDomainServerIP[intId]=strValue;").Append(Environment.NewLine)
        sbJS.Append("            }").Append(Environment.NewLine)
        sbJS.Append("        }  ").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function FixPass() {").Append(Environment.NewLine)
        sbJS.Append("          for (intI=0;intI<arrDomain.length;intI++) {").Append(Environment.NewLine)
        sbJS.Append("            document.getElementById('password' + intI).value = arrDomainPassword[intI];").Append(Environment.NewLine)
        'sbJS.Append("            alert(document.getElementById('password' + intI).value);").Append(Environment.NewLine)
        sbJS.Append("          }").Append(Environment.NewLine)
        sbJS.Append("        }  ").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function createDomain(did,id,dns,netbios,uname,pwd,sip) {").Append(Environment.NewLine)
        sbJS.Append("          var sRet = """";").Append(Environment.NewLine)
        sbJS.Append("          var dname = dns.split(""."");").Append(Environment.NewLine)
        sbJS.Append("          var dname = dname[0].toLowerCase();").Append(Environment.NewLine)
        sbJS.Append("          sRet = ""<table class=\""ektronForm\"" ><tr>"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""<td colspan=\""2\""><a href=\""#\"" onClick=\""javascript:removeDomain("" + id + "")\"">" & m_refMsg.GetMessage("lbl Remove Domain") & "</a></td></tr>"";").Append(Environment.NewLine)
        'sbJS.Append("          sRet = sRet + ""<tr><td class=\""label\"" style=\""width:160px;\""><b>" & MyBase.GetMessage("lbl ad domain name") & ":</b></td><td><input name=\""domain_name"" + id + ""\"" type=\""text\"" value=\"""" + dname + ""\"" size=\""55\"" id=\""domain_name"" + id + ""\"" onChange=\""javascript:saveDomain("" + id + "",this.value,'dname')\"" /></td>"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""<input type=\""hidden\"" name=\""domain_iden"" + id + ""\"" id=\""domain_iden"" + id + ""\"" value=\"""" + did + ""\"" />"";").Append(Environment.NewLine)
        'sbJS.Append("          sRet = sRet + ""</tr>"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""<tr class=\""stripe\""><td class=\""label\""><b>" & MyBase.GetMessage("lbl ad domain dns") & ":</b></td><td class=\""value\""><input name=\""domain_dns"" + id + ""\"" class=\""ektronTextMedium\"" type=\""text\"" value=\"""" + dns + ""\"" size=\""55\"" id=\""domain_dns"" + id + ""\"" onChange=\""javascript:saveDomain("" + id + "",this.value,'dns')\"" /></td>"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""</tr><tr>"";").Append(Environment.NewLine)
        sbJS.Append("          if (dname == netbios.toLowerCase()) { ").Append(Environment.NewLine)
        sbJS.Append("               sRet = sRet + ""<td class=\""label\"" valign=\""top\"" ><b>" & MyBase.GetMessage("lbl ad netbios") & ":</b></td><td class=\""value\""><input name=\""netbios"" + id + ""\"" class=\""ektronTextMedium\"" type=\""text\"" value=\"""" + netbios + ""\"" size=\""55\"" id=\""netbios"" + id + ""\"" disabled onChange=\""javascript:saveDomain("" + id + "",this.value,'netbios')\"" />"";").Append(Environment.NewLine)
        sbJS.Append("               sRet = sRet + ""<div class=\""ektronCaption\""><input type=\""checkbox\"" name=\""use_name"" + id + ""\"" id=\""use_name"" + id + ""\"" onclick=\""(document.getElementById('netbios"" + id + ""').disabled)=(!(document.getElementById('netbios"" + id + ""').disabled));\"" checked />" & MyBase.GetMessage("lbl ad use domainname") & "</div></td>"";").Append(Environment.NewLine)
        sbJS.Append("          } else { ").Append(Environment.NewLine)
        sbJS.Append("               sRet = sRet + ""<td class=\""label\"" valign=\""top\"" ><b>" & MyBase.GetMessage("lbl ad netbios") & ":</b></td><td class=\""value\""><input name=\""netbios"" + id + ""\"" class=\""ektronTextMedium\"" type=\""text\"" value=\"""" + netbios + ""\"" size=\""55\"" id=\""netbios"" + id + ""\"" onChange=\""javascript:saveDomain("" + id + "",this.value,'netbios')\"" />"";").Append(Environment.NewLine)
        sbJS.Append("               sRet = sRet + ""<div class=\""ektronCaption\""><input type=\""checkbox\"" name=\""use_name"" + id + ""\"" id=\""use_name"" + id + ""\"" onclick=\""(document.getElementById('netbios"" + id + ""').disabled)=(!(document.getElementById('netbios"" + id + ""').disabled));\"" />" & MyBase.GetMessage("lbl ad use domainname") & "</div></td>"";").Append(Environment.NewLine)
        sbJS.Append("          } ").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""</tr><tr class=\""stripe\"">"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""<td class=\""label\""><b>" & m_refMsg.GetMessage("username label") & "</b></td><td class=\""value\""><input class=\""ektronTextMedium\"" name=\""user_name"" + id + ""\"" type=\""text\"" value=\"""" + uname + ""\"" size=\""55\"" id=\""user_name"" + id + ""\"" onChange=\""javascript:saveDomain("" + id + "",this.value,'uname')\"" /> <span class=\""ektronCaption\"">ex: name@domain.com</span></td>"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""</tr><tr>"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""<td class=\""label\""><b>" & m_refMsg.GetMessage("password label") & "</b></td><td class=\""value\""><input class=\""ektronTextMedium\"" name=\""password"" + id + ""\"" type=\""password\"" size=\""55\"" id=\""password"" + id + ""\"" value=\"""" + pwd + ""\"" onChange=\""javascript:saveDomain("" + id + "",this.value,'pwd')\"" /></td>"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""</tr><tr class=\""stripe\"">"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""<td class=\""label\""><b>" & MyBase.GetMessage("lbl ad serverip") & ":</b></td><td class=\""value\""><input class=\""ektronTextMedium\"" name=\""server_ip"" + id + ""\"" type=\""text\"" value=\"""" + sip + ""\"" size=\""55\"" id=\""server_ip"" + id + ""\"" onChange=\""javascript:saveDomain("" + id + "",this.value,'sip')\"" /></td>"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""</tr><tr><td colspan=\""2\""><hr/></td></tr></table>"";").Append(Environment.NewLine)
        sbJS.Append("          return sRet; ").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function deleteDomain() {").Append(Environment.NewLine)
        sbJS.Append("            //remove last").Append(Environment.NewLine)
        sbJS.Append("            var cnfm = confirm(""" & MyBase.GetMessage("delete domain msg") & """);").Append(Environment.NewLine)
        sbJS.Append("            if (cnfm == true)").Append(Environment.NewLine)
        sbJS.Append("            {").Append(Environment.NewLine)
        sbJS.Append("              if (arrDomain.length > 0) { ").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainID.pop(); ").Append(Environment.NewLine)
        sbJS.Append("                 arrDomain.pop(); ").Append(Environment.NewLine)
        sbJS.Append("                 // arrDomainValue.pop();").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainDNS.pop();").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainNetBIOS.pop();").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainUsername.pop();").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainPassword.pop();").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainServerIP.pop();").Append(Environment.NewLine)
        sbJS.Append("              }").Append(Environment.NewLine)
        sbJS.Append("              displayDomain();").Append(Environment.NewLine)
        sbJS.Append("            }").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function removeDomain(id) {").Append(Environment.NewLine)
        sbJS.Append("            //remove last").Append(Environment.NewLine)
        sbJS.Append("            var cnfm = confirm(""" & MyBase.GetMessage("alt are you sure you want to remove this domain?") & """);").Append(Environment.NewLine)
        sbJS.Append("            if (cnfm == true)").Append(Environment.NewLine)
        sbJS.Append("            {").Append(Environment.NewLine)
        sbJS.Append("              if (arrDomain.length > 0) { ").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainID.splice(id,1); ").Append(Environment.NewLine)
        sbJS.Append("                 arrDomain.pop(); ").Append(Environment.NewLine)
        sbJS.Append("                 // arrDomainValue.splice(id,1);").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainDNS.splice(id,1);").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainNetBIOS.splice(id,1);").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainUsername.splice(id,1);").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainPassword.splice(id,1);").Append(Environment.NewLine)
        sbJS.Append("                 arrDomainServerIP.splice(id,1);").Append(Environment.NewLine)
        sbJS.Append("              }").Append(Environment.NewLine)
        sbJS.Append("              displayDomain(); ").Append(Environment.NewLine)
        sbJS.Append("            }").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function SubmitForm() {" & Environment.NewLine)
        sbJS.Append("           var strRet = CheckDomain();" & Environment.NewLine)
        sbJS.Append("           if (strRet.length > 0)" & Environment.NewLine)
        sbJS.Append("           {" & Environment.NewLine)
        sbJS.Append("               alert(strRet);" & Environment.NewLine)
        sbJS.Append("           } else { " & Environment.NewLine)
        sbJS.Append("               document.forms[0].submit();" & Environment.NewLine)
        sbJS.Append("           }" & Environment.NewLine)
        sbJS.Append("        }" & Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function CheckDomain() {" & Environment.NewLine)
        sbJS.Append("           var sErr = """"; " & Environment.NewLine)
        sbJS.Append("           // var bName = false; " & Environment.NewLine)
        sbJS.Append("           var bDNS = false; " & Environment.NewLine)
        sbJS.Append("           var bUser = false; " & Environment.NewLine)
        sbJS.Append("           var bPass = false; " & Environment.NewLine)
        sbJS.Append("           var bSIP = false; " & Environment.NewLine)

        If setting_data.ADAuthentication = 1 And setting_data.ADIntegration = True Then
            sbJS.Append("           if (arrDomain.length < 1)" & Environment.NewLine)
            sbJS.Append("           {" & Environment.NewLine)
            sbJS.Append("               sErr = """ & MyBase.GetMessage("js err need domain") & """;" & Environment.NewLine)
            sbJS.Append("           }" & Environment.NewLine)
        End If

        sbJS.Append("           for (var j = 0; j < arrDomain.length; j++)" & Environment.NewLine)
        sbJS.Append("           {" & Environment.NewLine)
        'sbJS.Append("                if ((Trim(arrDomainValue[j]).length < 1) && (bName == false)) {" & Environment.NewLine)
        'sbJS.Append("                    if (sErr.length > 0) {" & Environment.NewLine)
        'sbJS.Append("                       sErr = sErr + ""\n"";" & Environment.NewLine)
        'sbJS.Append("                    }" & Environment.NewLine)
        'sbJS.Append("                    sErr = sErr + ""All domains must have a name."";" & Environment.NewLine)
        'sbJS.Append("                    bName = true; " & Environment.NewLine)
        'sbJS.Append("                }" & Environment.NewLine)

        sbJS.Append("                if ((Trim(arrDomainDNS[j]).length < 1) && (bDNS == false)) {" & Environment.NewLine)
        sbJS.Append("                    if (sErr.length > 0) {" & Environment.NewLine)
        sbJS.Append("                       sErr = sErr + ""\n"";" & Environment.NewLine)
        sbJS.Append("                    }" & Environment.NewLine)
        sbJS.Append("                    sErr = sErr + """ & MyBase.GetMessage("js err domains dns") & """;" & Environment.NewLine)
        sbJS.Append("                    bDNS = true; " & Environment.NewLine)
        sbJS.Append("                }" & Environment.NewLine)

        sbJS.Append("                if ((Trim(arrDomainUsername[j]).length < 1) && (bUser == false)) {" & Environment.NewLine)
        sbJS.Append("                    if (sErr.length > 0) {" & Environment.NewLine)
        sbJS.Append("                       sErr = sErr + ""\n"";" & Environment.NewLine)
        sbJS.Append("                    }" & Environment.NewLine)
        sbJS.Append("                    sErr = sErr + """ & MyBase.GetMessage("js err domains uname") & """;" & Environment.NewLine)
        sbJS.Append("                    bUser = true; " & Environment.NewLine)
        sbJS.Append("                }" & Environment.NewLine)

        sbJS.Append("                if (( (arrDomainPassword[j] != ""          "") && (Trim(arrDomainPassword[j]).length < 1) ) && (bPass == false)) {" & Environment.NewLine)
        sbJS.Append("                    if (sErr.length > 0) {" & Environment.NewLine)
        sbJS.Append("                       sErr = sErr + ""\n"";" & Environment.NewLine)
        sbJS.Append("                    }" & Environment.NewLine)
        sbJS.Append("                    sErr = sErr + """ & MyBase.GetMessage("js err domains pwd") & """;" & Environment.NewLine)
        sbJS.Append("                    bPass = true; " & Environment.NewLine)
        sbJS.Append("                }" & Environment.NewLine)

        sbJS.Append("                if ((Trim(arrDomainServerIP[j]).length < 1) && (bSIP == false)) {" & Environment.NewLine)
        sbJS.Append("                    if (sErr.length > 0) {" & Environment.NewLine)
        sbJS.Append("                       sErr = sErr + ""\n"";" & Environment.NewLine)
        sbJS.Append("                    }" & Environment.NewLine)
        sbJS.Append("                    sErr = sErr + """ & MyBase.GetMessage("js err domains dc") & """;" & Environment.NewLine)
        sbJS.Append("                    bSIP = true; " & Environment.NewLine)
        sbJS.Append("                }" & Environment.NewLine)

        sbJS.Append("           }" & Environment.NewLine)
        sbJS.Append("           return sErr;" & Environment.NewLine)
        sbJS.Append("        }" & Environment.NewLine)

        Me.ltr_add_domain_js.Text = sbJS.ToString()
    End Sub

    Private Sub RegisterCss()

        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronWorkareaIeCss, API.Css.BrowserTarget.LessThanEqualToIE7)
        Ektron.Cms.API.Css.RegisterCss(Me, API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

    End Sub

End Class
