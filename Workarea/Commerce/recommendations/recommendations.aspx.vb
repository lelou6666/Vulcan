Imports System.Collections.Generic
Imports Ektron.Cms
Imports Ektron.Cms.Commerce
Imports Ektron.Cms.Common
Imports Ektron.Cms.Workarea
Imports Microsoft.Security.Application

Partial Class Commerce_recommendations
    Inherits workareabase

    Protected m_iFolderId As Long = 0
    Protected RecommendationList As List(Of Ektron.Cms.Commerce.RecommendationItemData)
    Protected recommendationManager As RecommendationApi = Nothing
    Protected RecType As EkEnumeration.RecommendationType = EkEnumeration.RecommendationType.CrossSell
    Protected security_data As PermissionData = Nothing

#Region "Page Functions"
    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce)) Then
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"))
        End If
        Util_RegisterResources()
        Try
            If Request.QueryString("folder") <> "" Then m_iFolderId = Request.QueryString("folder")
            Util_CheckAccess()
            Select Case m_sPageAction
                Case "crosssell"
                    RecType = EkEnumeration.RecommendationType.CrossSell
                    If Page.IsPostBack Then Process_CrossSell() Else Display_CrossSell()
                Case "upsell"
                    RecType = EkEnumeration.RecommendationType.Upsell
                    If Page.IsPostBack Then Process_UpSell() Else Display_UpSell()
            End Select
            Util_SetLabels()
            Util_SetJS()
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region
#Region "Display"
    Protected Sub Display_CrossSell()
        recommendationManager = New Ektron.Cms.Commerce.RecommendationApi()
        RecommendationList = recommendationManager.GetList(m_iID, ContentLanguage, RecType)
        Util_BuildRecommendations()
    End Sub
    Protected Sub Display_UpSell()
        recommendationManager = New Ektron.Cms.Commerce.RecommendationApi()
        RecommendationList = recommendationManager.GetList(m_iID, ContentLanguage, RecType)
        Util_BuildRecommendations()
    End Sub
#End Region
#Region "Process"
    Protected Sub Process_CrossSell()
        recommendationManager = New Ektron.Cms.Commerce.RecommendationApi()
        RecommendationList = Process_GetRecommendations()
        recommendationManager.UpdateCrossSell(m_iID, RecommendationList)

        Response.Redirect("../../content.aspx?action=View&folder_id=" & m_iFolderId & "&id=" & m_iID & "&LangType=" & ContentLanguage & "&callerpage=content.aspx&origurl=action=%3dViewContentByCategory%26id%3d" & m_iFolderId, False)
    End Sub
    Protected Sub Process_UpSell()
        recommendationManager = New Ektron.Cms.Commerce.RecommendationApi()
        RecommendationList = Process_GetRecommendations()
        recommendationManager.UpdateUpSell(m_iID, RecommendationList)

        Response.Redirect("../../content.aspx?action=View&folder_id=" & m_iFolderId & "&id=" & m_iID & "&LangType=" & ContentLanguage & "&callerpage=content.aspx&origurl=action=%3dViewContentByCategory%26id%3d" & m_iFolderId, False)
    End Sub
    Public Function Process_GetRecommendations() As List(Of RecommendationItemData)
        Dim aRecommendations As New System.Collections.Generic.List(Of RecommendationItemData)
        Dim i As Integer = 1
        Do While Request.Form("rec_" & i.ToString() & "_posidx") IsNot Nothing
            Dim Recommendation As New RecommendationItemData()
            Dim idx As Integer = Request.Form("rec_" & i.ToString() & "_posidx")
            Recommendation.DisplayOrder = i + 1
            Recommendation.Id = Request.Form("rec_" & idx.ToString() & "_id")
            Recommendation.EntryId = Request.Form("rec_" & idx.ToString() & "_entryid")
            Recommendation.EntryLanguage = Me.ContentLanguage
            aRecommendations.Add(Recommendation)
            i = i + 1
        Loop
        Return aRecommendations
    End Function
#End Region
#Region "Util"
    Protected Sub Util_SetLabels()
        Select Case m_sPageAction
            Case "crosssell"
                SetTitleBarToMessage("lbl cross sell rec")
            Case "upsell"
                SetTitleBarToMessage("lbl up sell rec")
        End Select

        Dim actionMenu As New workareamenu("action", Me.GetMessage("lbl action"), m_refContentApi.AppPath & "images/ui/icons/brick.png") ' check2.gif
        If security_data.CanEdit Then
            actionMenu.AddItem(m_refContentApi.AppPath & "images/ui/icons/save.png", Me.GetMessage("btn save"), "document.forms[0].submit();")
            actionMenu.AddBreak()
        End If
        actionMenu.AddLinkItem(m_refContentApi.AppPath & "images/ui/icons/cancel.png", Me.GetMessage("generic cancel"), "../../content.aspx?action=View&folder_id=" & m_iFolderId & "&id=" & m_iID & "&LangType=" & ContentLanguage & "&callerpage=content.aspx&origurl=action=%3dViewContentByCategory%26id%3d" & m_iFolderId)
        Me.AddMenu(actionMenu)
        AddBackButton("../../content.aspx?action=View&folder_id=" & m_iFolderId & "&id=" & m_iID & "&LangType=" & ContentLanguage & "&callerpage=content.aspx&origurl=action=%3dViewContentByCategory%26id%3d" & m_iFolderId)
        AddHelpButton(AntiXss.HtmlEncode(m_sPageAction))
    End Sub
    Protected Sub Util_BuildRecommendations()
        Dim sbItems As New StringBuilder()
        sbItems = New StringBuilder()
        sbItems.Append("<table width=""100%"" id=""Table1"" runat=""server"">		").Append(Environment.NewLine)
        sbItems.Append("<tr>").Append(Environment.NewLine)
        sbItems.Append("<td>").Append(Environment.NewLine)
        sbItems.Append("    <table width=""100%"">").Append(Environment.NewLine)
        sbItems.Append("    <tr>").Append(Environment.NewLine)
        sbItems.Append("    <td>").Append(Environment.NewLine)
        sbItems.Append("        <table width=""100%"">").Append(Environment.NewLine)
        sbItems.Append("        <tr>").Append(Environment.NewLine)
        sbItems.Append("        <td>").Append(Environment.NewLine)
        sbItems.Append("            <table align=""left"" width=""100%"">").Append(Environment.NewLine)
        sbItems.Append("            <tr>").Append(Environment.NewLine)
        sbItems.Append("            <td>").Append(Environment.NewLine)
        sbItems.Append("                <table align=""center"" width=""100%"">").Append(Environment.NewLine)
        sbItems.Append("                <tr>").Append(Environment.NewLine)
        sbItems.Append("                <td width=""50%"">").Append(Environment.NewLine)
        sbItems.Append("                        <table id=""tblRecommendations"" class=""ektableutil ektronGrid"">").Append(Environment.NewLine)
        sbItems.Append("                        <thead>").Append(Environment.NewLine)
        sbItems.Append("                        <tr class=""title-header"">").Append(Environment.NewLine)
        sbItems.Append("                            <th></th><th>").Append(GetMessage("generic id")).Append("</th><th>").Append(GetMessage("generic title")).Append("</th><th>&#160;</th><th>&#160;</th>").Append(Environment.NewLine)
        sbItems.Append("                        </tr>").Append(Environment.NewLine)
        sbItems.Append("                        </thead>").Append(Environment.NewLine)
        sbItems.Append("                        <tbody>").Append(Environment.NewLine)
        For i As Integer = 0 To (RecommendationList.Count - 1)
            sbItems.Append("<tr")
            If i Mod 2 > 0 Then sbItems.Append(" class=""itemrow0""")
            sbItems.Append(">").Append(Environment.NewLine)
            sbItems.Append("<td>").Append((i + 1)).Append("</td>").Append(Environment.NewLine)
            sbItems.Append("<td>").Append(RecommendationList(i).EntryId).Append("</td>").Append(Environment.NewLine)
            sbItems.Append("<td>").Append(RecommendationList(i).Title).Append("</td>").Append(Environment.NewLine)
            sbItems.Append("<td>")
            sbItems.Append("<input type=""hidden"" id=""rec_").Append((i + 1)).Append("_id"" name=""rec_").Append((i + 1)).Append("_id"" value=""").Append(RecommendationList(i).Id).Append(""" />")
            sbItems.Append("<input type=""hidden"" id=""rec_").Append((i + 1)).Append("_entryid"" name=""rec_").Append((i + 1)).Append("_entryid"" value=""").Append(RecommendationList(i).EntryId).Append(""" />")
            sbItems.Append("<input type=""hidden"" id=""rec_").Append((i + 1)).Append("_posidx"" name=""rec_").Append((i + 1)).Append("_posidx"" value=""").Append((i + 1)).Append(""" />")
            sbItems.Append("</td>").Append(Environment.NewLine)
            sbItems.Append("<td><input type=""radio"" value=""").Append((i + 1)).Append(""" name=""radInput"" /></td>").Append(Environment.NewLine)
            sbItems.Append("</tr>").Append(Environment.NewLine)
        Next
        sbItems.Append("                        </tbody>").Append(Environment.NewLine)
        sbItems.Append("                        </table>").Append(Environment.NewLine)
        sbItems.Append("<p>")
        If security_data.CanEdit Then
            sbItems.Append("<div class=""ektronTopSpace"">")
            sbItems.Append("    <a class=""button buttonInline greenHover buttonAdd"" style=""cursor: pointer; margin-right: .25em;"" title="" ").Append(GetMessage("generic add title")).Append(" "" onclick=""AddRecommendation();"">").Append(GetMessage("generic add title")).Append("</a>")
            sbItems.Append("    <a class=""button buttonInline redHover buttonClear"" style=""cursor: pointer;"" title=""").Append(GetMessage("generic remove")).Append(""" onclick=""DeleteRecommendation();"" />").Append(GetMessage("generic remove")).Append("</a>")
            sbItems.Append("</div>")
        End If
        sbItems.Append("</p> ").Append(Environment.NewLine)
        sbItems.Append("		</td>").Append(Environment.NewLine)
        sbItems.Append("		</tr>").Append(Environment.NewLine)
        sbItems.Append("                </table>").Append(Environment.NewLine)
        sbItems.Append("            </td>").Append(Environment.NewLine)
        sbItems.Append("            </tr>").Append(Environment.NewLine)
        sbItems.Append("            </table>").Append(Environment.NewLine)
        sbItems.Append("        </td>").Append(Environment.NewLine)
        sbItems.Append("        </tr>").Append(Environment.NewLine)
        sbItems.Append("        </table>").Append(Environment.NewLine)
        sbItems.Append("    </td>").Append(Environment.NewLine)
        sbItems.Append("    </tr>").Append(Environment.NewLine)
        sbItems.Append("    </table>").Append(Environment.NewLine)
        sbItems.Append("</td>").Append(Environment.NewLine)
        sbItems.Append("</tr>").Append(Environment.NewLine)
        sbItems.Append("</table>").Append(Environment.NewLine)
        ltr_recommendations.Text = sbItems.ToString()
    End Sub
    Protected Sub Util_CheckAccess()
        Utilities.ValidateUserLogin()
        If m_refContentApi.RequestInformationRef.IsMembershipUser Then
            Response.Redirect(m_refContentApi.ApplicationPath & "reterror.aspx?info=" & m_refMsg.GetMessage("msg login cms user"), False)
        End If
        security_data = m_refContentApi.LoadPermissions(m_iID, "content")
    End Sub
    Protected Sub Util_SetJS()
        Dim sbJS As New StringBuilder()
        sbJS.Append("<script  type=""text/javascript"">").Append(Environment.NewLine)
        sbJS.Append("   function AddRecommendation() { ").Append(Environment.NewLine)
        sbJS.Append("       ektb_show('<span style=""font-size:1.3em; line-height:20px; color:#000080;"">" & GetMessage("lbl select item to add") & "</span>','../itemselection.aspx?action=").Append(m_sPageAction).Append("&id=").Append(m_iFolderId.ToString()).Append("&EkTB_iframe=true&height=300&width=500&modal=true', null); ").Append(Environment.NewLine)
        sbJS.Append("   } ").Append(Environment.NewLine)
        sbJS.Append("   function DeleteRecommendation() {").Append(Environment.NewLine)
        sbJS.Append("       var iAttr = getCheckedInt(false);").Append(Environment.NewLine)
        sbJS.Append("        if (iAttr == -1) {").Append(Environment.NewLine)
        sbJS.Append("            alert('").Append(GetMessage("js please sel rec")).Append("');").Append(Environment.NewLine)
        sbJS.Append("        } else {").Append(Environment.NewLine)
        sbJS.Append("            deleteChecked();").Append(Environment.NewLine)
        sbJS.Append("        }").Append(Environment.NewLine)
        sbJS.Append("   }").Append(Environment.NewLine)
        sbJS.Append("</script>").Append(Environment.NewLine)
        ltr_js.Text = sbJS.ToString()
    End Sub
    Protected Sub Util_RegisterResources()
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE)
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.AppPath & "csslib/tables/tableutil.css", "EktronTableUtilCSS")
        Ektron.Cms.API.Css.RegisterCss(Me, Me.m_refContentApi.AppPath & "csslib/box.css", "EktronBoxCSS")

        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, Me.m_refContentApi.AppPath & "java/dhtml/rectableutil.js", "EktronRectableUtilJS")
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS)
    End Sub
#End Region

End Class
