Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkFunctions
Imports Ektron.Cms.Workarea

Partial Class ContentFlagging_flagsets
    Inherits workareabase

#Region "Page Variables"

    Protected security_data As PermissionData
    Protected aFlagSets() As FlagDefData = Array.CreateInstance(GetType(Ektron.Cms.FlagDefData), 0)
    Protected fdFlagDef As New FlagDefData
    Protected bFlagEditor As Boolean = False
    Protected bAdmin As Boolean = False
    Private objLocalizationApi As LocalizationAPI = Nothing
    Private AddLink As Boolean = True
    Dim communityflagaction As String = ""
#End Region

#Region "Page Functions"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Utilities.SetLanguage(m_refContentApi)
        Me.security_data = Me.m_refContentApi.LoadPermissions(0, "content")
        bAdmin = Me.security_data.IsAdmin
        bFlagEditor = Me.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminRuleEditor, m_refContentApi.UserId, False)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        RegisterResources()

        Try
            If (Request.QueryString("communityonly") IsNot Nothing AndAlso Request.QueryString("communityonly") = "true") Then
                communityflagaction = "communityonly=true&"
            End If
            If Me.security_data.IsAdmin() = True Then
                If Page.IsPostBack Then
                    Select Case Me.m_sPageAction
                        Case "addedit"
                            Me.Process_AddEdit()
                    End Select
                Else
                    Select Case Me.m_sPageAction
                        Case "addedit"
                            Me.AddEdit()
                        Case "remove"
                            Me.Process_Remove()
                        Case Else 'view
                            Me.ViewAll()
                    End Select
                End If
                Me.SetLabels()
                Me.SetJS()
            Else
                Throw New Exception(Me.GetMessage("err flagset no access"))
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub

#End Region

#Region "Display"

    Public Sub AddEdit()

        If Me.m_iID > 0 Then
            fdFlagDef = Me.m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(Me.m_iID, True)
        End If

        ltr_options.Text = "<input type=""hidden"" id=""Flaglength"" name=""Flaglength"" value=""" & fdFlagDef.Items.Length.ToString() & """ /><div id=""pFlag"" name=""pFlag"">" & Environment.NewLine
        Dim sIndent As String = "&nbsp;"
        If Me.bAdmin Or Me.bFlagEditor Then
            For i As Integer = 0 To (fdFlagDef.Items.Length - 1)
                ltr_options.Text &= "<script type=""text/javascript"">addFlagInit(" & fdFlagDef.Items(i).ID.ToString() & ",'" & fdFlagDef.Items(i).Name & "');</script>"
                ltr_options.Text &= "<input type=""hidden"" name=""flag_iden" & i.ToString() & """ id=""flag_iden" & i.ToString() & """ value=""" & fdFlagDef.Items(i).ID.ToString() & """ /> "
                ltr_options.Text &= sIndent & sIndent & "<input type=""text"" id=""flagdefopt" & i.ToString() & """ name=""flagdefopt" & i.ToString() & """ value=""" & (fdFlagDef.Items(i).Name) & """ maxlength=""50"" size=""35"" onChange=""javascript:saveFlag(" & i.ToString() & ",this.value,'fname');"">"
                If i = (fdFlagDef.Items.Length - 1) Then
                    ltr_options.Text &= sIndent & sIndent & "<img src=""" & Me.AppImgPath & "movedown_disabled.gif""/>"
                Else
                    ltr_options.Text &= sIndent & sIndent & "<a href=""#"" onclick=""javascript:moveFlag(" & i.ToString() & ",'down'); return false;""><img src=""" & Me.AppImgPath & "movedown.gif""/></a>"
                End If
                If i = 0 Then
                    ltr_options.Text &= sIndent & sIndent & "<img src=""" & Me.AppImgPath & "moveup_disabled.gif""/>"
                Else
                    ltr_options.Text &= sIndent & sIndent & "<a href=""#"" onclick=""javascript:moveFlag(" & i.ToString() & ",'up'); return false;""><img src=""" & Me.AppImgPath & "moveup.gif""/></a>"
                End If
                ltr_options.Text &= sIndent & sIndent & "<a href=""#"" onclick=""javascript:removeFlag('" + i.ToString() + "'); return false;""><img src=""" & m_refContentApi.RequestInformationRef.ApplicationPath & "images/UI/Icons/delete.png""/></a>"
                ltr_options.Text &= "<br/>"
            Next
            ltr_options.Text &= sIndent & sIndent & "<a href=""#"" onclick=""javascript:addFlag(); return false;""><img src=""" & m_refContentApi.RequestInformationRef.ApplicationPath & "images/UI/Icons/add.png""/></a><br/>"
        Else
            For i As Integer = 0 To (fdFlagDef.Items.Length - 1)
                ltr_options.Text &= sIndent & sIndent & "<input type=""text"" id=""flagdefopt" & i.ToString() & """ name=""flagdefopt" & i.ToString() & """ value=""" & (fdFlagDef.Items(i).Name) & """ disabled ><br/>"
            Next
        End If
        ltr_options.Text &= "</div>"
    End Sub

    Public Sub ViewAll()

        Me.tbledit.Visible = False
        Dim sbContent As New StringBuilder()
        sbContent.Append("<div class=""ektronPageContainer""><table class=""ektronGrid"" width=""100%"">" & Environment.NewLine)

        sbContent.Append("<tr class=""title-header"">" & Environment.NewLine)
        sbContent.Append("<th align=""center"">" & Environment.NewLine)
        sbContent.Append(GetMessage("generic id"))
        sbContent.Append("</th>" & Environment.NewLine)
        sbContent.Append("<th>" & Environment.NewLine)
        sbContent.Append(GetMessage("generic name"))
        sbContent.Append("</th>" & Environment.NewLine)
        sbContent.Append("<th>" & Environment.NewLine)
        sbContent.Append(GetMessage("generic description"))
        sbContent.Append("</th>" & Environment.NewLine)
        sbContent.Append("<th align=""center"">" & Environment.NewLine)
        sbContent.Append(GetMessage("lbl language"))
        sbContent.Append("</th>" & Environment.NewLine)
        sbContent.Append("<th align=""center"">" & Environment.NewLine)
        sbContent.Append(GetMessage("generic items"))
        sbContent.Append("</th>" & Environment.NewLine)
        sbContent.Append("</tr>" & Environment.NewLine)
        If (Not String.IsNullOrEmpty(communityflagaction)) Then
            Dim flagdata As FlagDefData = Me.m_refContentApi.EkContentRef.GetCommunityFlaggingDefinition(False)
            If (flagdata IsNot Nothing) Then
                AddLink = False
                sbContent.Append(ReadFlagSet(flagdata, 0))
            End If
        Else
            aFlagSets = Me.m_refContentApi.EkContentRef.GetAllFlaggingDefinitions(False)
            For i As Integer = 0 To (aFlagSets.Length - 1)
                sbContent.Append(ReadFlagSet(aFlagSets(i), i))
            Next
        End If

        sbContent.Append("</table>" & Environment.NewLine)
        Me.ltr_view.Text = sbContent.ToString()
    End Sub
    Public Function ReadFlagSet(ByVal flag As FlagDefData, ByVal i As Integer) As String
        If (objLocalizationApi Is Nothing) Then
            objLocalizationApi = New LocalizationAPI()
        End If
        Dim sb As New StringBuilder()
        sb.Append("<tr>" & Environment.NewLine)
        sb.Append("<td align=""center"">" & Environment.NewLine)
        sb.Append("<a href=""flagsets.aspx?" & communityflagaction & "action=addedit&id=" & flag.ID.ToString() & "&LangType=" & flag.Language & """>" & flag.ID.ToString() & "</a>")
        sb.Append("</td>" & Environment.NewLine)
        sb.Append("<td>" & Environment.NewLine)
        sb.Append("<a href=""flagsets.aspx?" & communityflagaction & "action=addedit&id=" & flag.ID.ToString() & "&LangType=" & flag.Language & """ class=""flagEdit"">" & flag.Name & "</a>")
        sb.Append("</td>" & Environment.NewLine)
        sb.Append("<td>" & Environment.NewLine)
        sb.Append(flag.Description)
        sb.Append("</td>" & Environment.NewLine)
        sb.Append("<td align=""center"">" & Environment.NewLine)
        sb.Append("<img src='" & objLocalizationApi.GetFlagUrlByLanguageID(flag.Language) & "' />")
        sb.Append("</td>" & Environment.NewLine)
        sb.Append("<td align=""center"">" & Environment.NewLine)
        sb.Append(flag.Items.Length)
        sb.Append("</td>" & Environment.NewLine)
        sb.Append("</tr>" & Environment.NewLine)
        Return sb.ToString
    End Function
#End Region

#Region "Process"

    Public Sub Process_AddEdit()
        If Me.bAdmin Or Me.bFlagEditor Then
            If Me.m_iID > 0 Then
                fdFlagDef = Me.m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(Me.m_iID, True)
            Else
                fdFlagDef.ID = 0 ' signal backend that this is a new item.
            End If

            fdFlagDef.Name = Me.txt_fd_name.Text
            fdFlagDef.Description = Me.txt_fd_desc.Text
            fdFlagDef.Language = IIf(Me.ContentLanguage > 0, Me.ContentLanguage, Me.m_refContentApi.RequestInformationRef.DefaultContentLanguage)

            Dim iLength As Integer = 0
            Dim alFlagList As New ArrayList()
            Dim aRet As FlagItemData() = Array.CreateInstance(GetType(FlagItemData), 0)
            iLength = CInt(Request.Form("Flaglength"))
            If iLength > 0 Then
                For i As Integer = 0 To (iLength - 1)
                    Dim fiTMP As New FlagItemData()
                    fiTMP.ID = CLng(Request.Form("flag_iden" & i.ToString()))
                    fiTMP.Name = Request.Form("flagdefopt" & i.ToString())
                    fiTMP.SortOrder = (i + 1)
                    fiTMP.FlagDefinitionID = Me.m_iID
                    fiTMP.FlagDefinitionLanguage = IIf(Me.ContentLanguage > 0, Me.ContentLanguage, Me.m_refContentApi.RequestInformationRef.DefaultContentLanguage)
                    alFlagList.Add(fiTMP)
                Next
                aRet = alFlagList.ToArray(GetType(FlagItemData))
            End If

            fdFlagDef.Items = aRet

            If Me.m_iID > 0 Then
                fdFlagDef = Me.m_refContentApi.EkContentRef.UpdateFlaggingDefinition(fdFlagDef)
            Else
                If (Not (String.IsNullOrEmpty(communityflagaction))) Then
                    fdFlagDef.Hidden = True
                End If
                fdFlagDef = Me.m_refContentApi.EkContentRef.AddFlaggingDefinition(fdFlagDef)
            End If
            Response.Redirect("flagsets.aspx?" & communityflagaction & "action=viewall", False) ' &id=" & fdFlagDef.ID.ToString(), False)
        Else
            Throw New Exception(Me.GetMessage("err flagset no access"))
        End If
    End Sub

    Public Sub Process_Remove()
        If Me.bAdmin Or Me.bFlagEditor Then
            If Me.m_iID > 0 Then
                fdFlagDef = Me.m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(Me.m_iID, True)
                If fdFlagDef.ID > 0 Then
                    Me.m_refContentApi.EkContentRef.DeleteFlaggingDefinition(fdFlagDef.ID)
                End If
            End If
            Response.Redirect("flagsets.aspx?action=viewall", False)
        Else
            Throw New Exception(Me.GetMessage("err flagset no access"))
        End If
    End Sub

#End Region

#Region "Private Helpers"

    Private Sub SetLabels()
        Select Case Me.m_sPageAction
            Case "addedit"
                If Me.m_iID > 0 Then
                    Me.SetTitleBarToMessage("lbl flagset edit")
                Else
                    Me.SetTitleBarToMessage("lbl flagset add")
                End If

                Me.ltr_name.Text = Me.GetMessage("generic name") & ":"
                Me.ltr_desc.Text = Me.GetMessage("generic description") & ":"

                Me.txt_fd_name.Text = Server.HtmlDecode(fdFlagDef.Name)
                Me.hdn_fd_name.Value = Server.HtmlDecode(fdFlagDef.Name)
                Me.txt_fd_desc.Text = Server.HtmlDecode(fdFlagDef.Description)
                If Me.bAdmin Or Me.bFlagEditor Then
                    MyBase.AddButtonwithMessages(m_refContentApi.RequestInformationRef.ApplicationPath & "images/UI/Icons/save.png", "#", "lbl alt save flagset", "btn save", " onclick=""javascript:SubmitForm();"" ")
                    If (String.IsNullOrEmpty(communityflagaction)) Then
                        If Me.m_iID > 0 Then
                            MyBase.AddButton(m_refContentApi.RequestInformationRef.ApplicationPath & "images/UI/Icons/delete.png", "flagsets.aspx?action=Remove&id=" & m_iID.ToString(), GetMessage("lbl delete flag"), GetMessage("lbl delete flag def"), "onclick=""javascript:return VerifyDelete()"" ")
                        End If
                    End If
                Else
                    Me.txt_fd_name.Enabled = False
                    Me.txt_fd_desc.Enabled = False
                End If
                Me.AddBackButton("flagsets.aspx?action=viewall" + IIf((String.IsNullOrEmpty(communityflagaction)), "", "&communityonly=true"))
                If Me.m_iID > 0 Then
                    MyBase.AddHelpButton("edit_flagdef")
                Else
                    MyBase.AddHelpButton("add_flagdef")
                End If
            Case Else
                If (String.IsNullOrEmpty(communityflagaction)) Then
                    Me.SetTitleBarToMessage("wa tree flag def")
                Else
                    Me.SetTitleBarToMessage("wa tree community flag def")
                End If

                If (AddLink) Then
                    If ((bFlagEditor Or bAdmin) And Me.m_refContentApi.ContentLanguage > 0) Then
                        MyBase.AddButton(m_refContentApi.RequestInformationRef.ApplicationPath & "images/UI/Icons/add.png", "flagsets.aspx?" & communityflagaction & "action=addedit", GetMessage("lbl add flag"), GetMessage("lbl add flag def"), "")
                    End If
                End If
                Me.AddLanguageDropdown(String.IsNullOrEmpty(communityflagaction))
                MyBase.AddHelpButton("view_flagdef")
        End Select
    End Sub

    Private Sub SetJS()
        Dim sbJS As New StringBuilder()
        sbJS.Append("  		<script type=""text/javascript""> ").Append(Environment.NewLine)

        sbJS.Append("  			function LoadLanguage(FormName){ ").Append(Environment.NewLine)
        sbJS.Append("  				var num=document.forms[FormName].selLang.selectedIndex; ").Append(Environment.NewLine)
        sbJS.Append("  				window.location.href=""flagsets.aspx?" & communityflagaction & "action=viewall""+""&LangType=""+document.forms[FormName].selLang.options[num].value; ").Append(Environment.NewLine)
        sbJS.Append("  				//document.forms[FormName].submit(); ").Append(Environment.NewLine)
        sbJS.Append("  				return false; ").Append(Environment.NewLine)
        sbJS.Append("  			} ").Append(Environment.NewLine)

        sbJS.Append("           function CheckFlagDefParam() {" & Environment.NewLine)
        sbJS.Append("	            document.forms.frmContent.submit();" & Environment.NewLine)
        sbJS.Append("  			    return false; ").Append(Environment.NewLine)
        sbJS.Append("  			} ").Append(Environment.NewLine)

        sbJS.Append("           function VerifyDelete()" & Environment.NewLine())
        sbJS.Append("           {" & Environment.NewLine())
        sbJS.Append("               var agree=confirm('" & GetMessage("js cnfrm del flag def") & "');" & Environment.NewLine())
        sbJS.Append("               if (agree) {" & Environment.NewLine())
        sbJS.Append("	                return true;" & Environment.NewLine())
        sbJS.Append("               } else {" & Environment.NewLine())
        sbJS.Append("	                return false;" & Environment.NewLine())
        sbJS.Append("               }" & Environment.NewLine())
        sbJS.Append("           }" & Environment.NewLine)

        sbJS.Append("/// Flag Items").Append(Environment.NewLine)
        sbJS.Append("        ").Append(Environment.NewLine)
        sbJS.Append("        var arrFlagID = new Array(0);").Append(Environment.NewLine)
        sbJS.Append("        var arrFlag = new Array(0);").Append(Environment.NewLine)
        sbJS.Append("        var arrFlagName = new Array(0);").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function addFlag() {").Append(Environment.NewLine)
        sbJS.Append("          arrFlagID.push(""0"");").Append(Environment.NewLine)
        sbJS.Append("          arrFlag.push(arrFlag.length);").Append(Environment.NewLine)
        sbJS.Append("          arrFlagName.push("""");").Append(Environment.NewLine)
        sbJS.Append("          displayFlag();").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("          function addFlagInit(fid,fname) {").Append(Environment.NewLine)
        sbJS.Append("          arrFlagID.push(fid);").Append(Environment.NewLine)
        sbJS.Append("          arrFlag.push(arrFlag.length);").Append(Environment.NewLine)
        sbJS.Append("          arrFlagName.push(fname);").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function displayFlag() {").Append(Environment.NewLine)
        sbJS.Append("          var sItem = '';").Append(Environment.NewLine)
        sbJS.Append("          var sList = '';").Append(Environment.NewLine)
        sbJS.Append("          document.getElementById('pFlag').innerHTML='';").Append(Environment.NewLine)
        sbJS.Append("          for (intI = 0; intI < arrFlag.length; intI++) {").Append(Environment.NewLine)
        sbJS.Append("            sItem = createFlag(arrFlagID[intI], arrFlag[intI], arrFlagName[intI], intI, arrFlag.length);").Append(Environment.NewLine)
        sbJS.Append("            sList += sItem;").Append(Environment.NewLine)
        sbJS.Append("          }").Append(Environment.NewLine)
        sbJS.Append("            sList += ""&nbsp;&nbsp;<a href=\""#\"" onclick=\""javascript:addFlag(); return false;\""><img src=\""" & m_refContentApi.RequestInformationRef.ApplicationPath & "images/UI/Icons/add.png\""/></a><br/>"";").Append(Environment.NewLine)
        sbJS.Append("          document.getElementById('pFlag').innerHTML = sList;").Append(Environment.NewLine)
        sbJS.Append("          document.getElementById('Flaglength').value = arrFlag.length;").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function saveFlag(intId,strValue,type) {").Append(Environment.NewLine)
        ' sbJS.Append("            alert(strValue + '-' + type); ").Append(Environment.NewLine)
        sbJS.Append("            if (type == ""fname"") {").Append(Environment.NewLine)
        sbJS.Append("                arrFlagName[intId]=strValue;").Append(Environment.NewLine)
        sbJS.Append("            }").Append(Environment.NewLine)
        sbJS.Append("        }  ").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function createFlag(fid,id,fname, iloc, itot) {").Append(Environment.NewLine)
        sbJS.Append("          var sRet = """";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""&nbsp;&nbsp;<input type=\""text\"" id=\""flagdefopt"" + id + ""\"" name=\""flagdefopt"" + id + ""\"" value=\"""" + fname + ""\"" maxlength=\""50\"" size=\""35\"" onChange=\""javascript:saveFlag("" + id + "",this.value,'fname')\"">"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""<input type=\""hidden\"" name=\""flag_iden"" + id + ""\"" id=\""flag_iden"" + id + ""\"" value=\"""" + fid + ""\"" />"";").Append(Environment.NewLine)
        sbJS.Append("          if (iloc == (itot - 1)) {").Append(Environment.NewLine)
        sbJS.Append("               sRet = sRet + ""&nbsp;&nbsp;<img src=\""" & Me.AppImgPath & "movedown_disabled.gif\""/>"";").Append(Environment.NewLine)
        sbJS.Append("          } else {").Append(Environment.NewLine)
        sbJS.Append("               sRet = sRet + ""&nbsp;&nbsp;<a href=\""#\"" onclick=\""javascript:moveFlag("" + id + "",'down');return false;\""><img src=\""" & Me.AppImgPath & "movedown.gif\""/></a>"";").Append(Environment.NewLine)
        sbJS.Append("          }").Append(Environment.NewLine)
        sbJS.Append("          if (iloc == 0) {").Append(Environment.NewLine)
        sbJS.Append("               sRet = sRet + ""&nbsp;&nbsp;<img src=\""" & Me.AppImgPath & "moveup_disabled.gif\""/>"";").Append(Environment.NewLine)
        sbJS.Append("          } else {").Append(Environment.NewLine)
        sbJS.Append("               sRet = sRet + ""&nbsp;&nbsp;<a href=\""#\"" onclick=\""javascript:moveFlag("" + id + "",'up');return false;\""><img src=\""" & Me.AppImgPath & "moveup.gif\""/></a>"";").Append(Environment.NewLine)
        sbJS.Append("          }").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""&nbsp;&nbsp;<a href=\""#\"" onclick=\""javascript:removeFlag("" + id + ""); return false;\""><img src=\""" & m_refContentApi.RequestInformationRef.ApplicationPath & "images/UI/Icons/delete.png\""/></a>"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + ""<br/>"";").Append(Environment.NewLine)
        sbJS.Append("          return sRet; ").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function deleteFlag() {").Append(Environment.NewLine)
        sbJS.Append("            //remove last").Append(Environment.NewLine)
        sbJS.Append("            var cnfm = confirm(""").Append(GetMessage("js confirm remove last flag item")).Append(""");").Append(Environment.NewLine)
        sbJS.Append("            if (cnfm == true)").Append(Environment.NewLine)
        sbJS.Append("            {").Append(Environment.NewLine)
        sbJS.Append("              if (arrFlag.length > 0) { ").Append(Environment.NewLine)
        sbJS.Append("                 arrFlagID.pop(); ").Append(Environment.NewLine)
        sbJS.Append("                 arrFlag.pop(); ").Append(Environment.NewLine)
        sbJS.Append("                 arrFlagName.pop();").Append(Environment.NewLine)
        sbJS.Append("              }").Append(Environment.NewLine)
        sbJS.Append("              displayFlag();").Append(Environment.NewLine)
        sbJS.Append("            }").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function removeFlag(id) {").Append(Environment.NewLine)
        sbJS.Append("            //remove last").Append(Environment.NewLine)
        sbJS.Append("            var cnfm = confirm(""").Append(GetMessage("js confirm remove flag item")).Append(""");").Append(Environment.NewLine)
        sbJS.Append("            if (cnfm == true)").Append(Environment.NewLine)
        sbJS.Append("            {").Append(Environment.NewLine)
        sbJS.Append("              if (arrFlag.length > 0) { ").Append(Environment.NewLine)
        sbJS.Append("                 arrFlagID.splice(id,1); ").Append(Environment.NewLine)
        sbJS.Append("                 arrFlag.pop(); ").Append(Environment.NewLine)
        sbJS.Append("                 arrFlagName.splice(id,1);").Append(Environment.NewLine)
        sbJS.Append("              }").Append(Environment.NewLine)
        sbJS.Append("              displayFlag(); ").Append(Environment.NewLine)
        sbJS.Append("            }").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function moveFlag(id, direc) {").Append(Environment.NewLine)
        sbJS.Append("            if (direc == 'up')").Append(Environment.NewLine)
        sbJS.Append("            {").Append(Environment.NewLine)
        sbJS.Append("                var strID = arrFlagID[id];").Append(Environment.NewLine)
        sbJS.Append("                var strName = arrFlagName[id];").Append(Environment.NewLine)
        sbJS.Append("                arrFlagID[id]=arrFlagID[(id - 1)];").Append(Environment.NewLine)
        sbJS.Append("                arrFlagName[id]=arrFlagName[(id - 1)];").Append(Environment.NewLine)
        sbJS.Append("                arrFlagID[(id - 1)]=strID;").Append(Environment.NewLine)
        sbJS.Append("                arrFlagName[(id - 1)]=strName;").Append(Environment.NewLine)
        sbJS.Append("            } else if (direc == 'down') {").Append(Environment.NewLine)
        sbJS.Append("                var strID = arrFlagID[id];").Append(Environment.NewLine)
        sbJS.Append("                var strName = arrFlagName[id];").Append(Environment.NewLine)
        sbJS.Append("                arrFlagID[id]=arrFlagID[(id + 1)];").Append(Environment.NewLine)
        sbJS.Append("                arrFlagName[id]=arrFlagName[(id + 1)];").Append(Environment.NewLine)
        sbJS.Append("                arrFlagID[(id + 1)]=strID;").Append(Environment.NewLine)
        sbJS.Append("                arrFlagName[(id + 1)]=strName;").Append(Environment.NewLine)
        sbJS.Append("            } ").Append(Environment.NewLine)
        sbJS.Append("            displayFlag(); ").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function SubmitForm() {" & Environment.NewLine)
        sbJS.Append("           if (!CheckForillegalChar()) { " & Environment.NewLine)
        sbJS.Append("               return false; " & Environment.NewLine)
        sbJS.Append("           } else { " & Environment.NewLine)
        sbJS.Append("               var strRet = CheckFlag();" & Environment.NewLine)
        sbJS.Append("               if (strRet.length > 0)" & Environment.NewLine)
        sbJS.Append("               {" & Environment.NewLine)
        sbJS.Append("                   alert(strRet);" & Environment.NewLine)
        sbJS.Append("               } else { " & Environment.NewLine)
        sbJS.Append("                   document.forms[0].submit();" & Environment.NewLine)
        sbJS.Append("               }" & Environment.NewLine)
        sbJS.Append("           }" & Environment.NewLine)
        sbJS.Append("        }" & Environment.NewLine)

        sbJS.Append("       function CheckForillegalChar() {" & Environment.NewLine)
        sbJS.Append("           var val = document.forms[0]." & Replace(Me.txt_fd_name.UniqueID, "$", "_") & ".value;" & Environment.NewLine)
        sbJS.Append("           if (Trim(val) == '')" & Environment.NewLine)
        sbJS.Append("           {" & Environment.NewLine)
        sbJS.Append("               alert('" & GetMessage("lbl please enter flag def name") & "'); " & Environment.NewLine)
        sbJS.Append("               return false; " & Environment.NewLine)
        sbJS.Append("           } else { " & Environment.NewLine)
        sbJS.Append("               if ((val.indexOf("";"") > -1) || (val.indexOf(""\\"") > -1) || (val.indexOf(""/"") > -1) || (val.indexOf("":"") > -1)||(val.indexOf(""*"") > -1) || (val.indexOf(""?"") > -1)|| (val.indexOf(""\"""") > -1) || (val.indexOf(""<"") > -1)|| (val.indexOf("">"") > -1) || (val.indexOf(""|"") > -1) || (val.indexOf(""&"") > -1) || (val.indexOf(""\'"") > -1))" & Environment.NewLine)
        sbJS.Append("               {" & Environment.NewLine)
        sbJS.Append("                   alert(""" & String.Format(GetMessage("lbl flag def name disallowed chars"), "(';', '\\', '/', ':', '*', '?', ' \"" ', '<', '>', '|', '&', '\'')") & """);" & Environment.NewLine)
        sbJS.Append("                   return false;" & Environment.NewLine)
        sbJS.Append("               }" & Environment.NewLine)
        sbJS.Append("           }" & Environment.NewLine)
        sbJS.Append("           return true;" & Environment.NewLine)
        sbJS.Append("       }" & Environment.NewLine)

        sbJS.Append("").Append(Environment.NewLine)

        sbJS.Append("        function CheckFlag() {" & Environment.NewLine)
        sbJS.Append("           var sErr = """"; " & Environment.NewLine)
        sbJS.Append("           var bName = false; " & Environment.NewLine)
        sbJS.Append("           var bDupe = false; " & Environment.NewLine)
        sbJS.Append("           if (arrFlag.length == 0) { " & Environment.NewLine)
        sbJS.Append("               sErr = """ & GetMessage("lbl flag def at least one flag item") & """;" & Environment.NewLine)
        sbJS.Append("           } else { " & Environment.NewLine)
        sbJS.Append("               for (var j = 0; j < arrFlag.length; j++)" & Environment.NewLine)
        sbJS.Append("               {" & Environment.NewLine)

        sbJS.Append("                   if ((Trim(arrFlagName[j]).length < 1) && (bName == false)) {" & Environment.NewLine)
        sbJS.Append("                       if (sErr.length > 0) {" & Environment.NewLine)
        sbJS.Append("                           sErr = sErr + ""\n"";" & Environment.NewLine)
        sbJS.Append("                       }" & Environment.NewLine)
        sbJS.Append("                       sErr = sErr + """ & GetMessage("lbl flag items no name") & """;" & Environment.NewLine)
        sbJS.Append("                       bName = true; " & Environment.NewLine)
        sbJS.Append("                   } " & Environment.NewLine)
        sbJS.Append("                   for (var k = 0; k < arrFlag.length; k++) { " & Environment.NewLine)
        sbJS.Append("                       if ((bDupe == false) && (j != k) && (Trim(arrFlagName[j]) == Trim(arrFlagName[k]))) {" & Environment.NewLine)
        sbJS.Append("                           if (sErr.length > 0) {" & Environment.NewLine)
        sbJS.Append("                               sErr = sErr + ""\n"";" & Environment.NewLine)
        sbJS.Append("                           }" & Environment.NewLine)
        sbJS.Append("                           sErr = sErr + """ & GetMessage("lbl flag items unique") & """;" & Environment.NewLine)
        sbJS.Append("                           bDupe = true; " & Environment.NewLine)
        sbJS.Append("                       } " & Environment.NewLine)
        sbJS.Append("                   } " & Environment.NewLine)

        sbJS.Append("               }" & Environment.NewLine)
        sbJS.Append("           }" & Environment.NewLine)
        sbJS.Append("           return sErr;" & Environment.NewLine)
        sbJS.Append("        }" & Environment.NewLine)

        sbJS.Append("  		</script> ").Append(Environment.NewLine)
        ltr_js.Text = sbJS.ToString()
    End Sub
    Protected Sub RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss)

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS)
    End Sub
#End Region

End Class
