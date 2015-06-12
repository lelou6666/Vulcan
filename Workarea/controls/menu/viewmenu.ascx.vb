Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports System.DateTime
Imports System.Collections.Generic
Imports System.IO

Partial Class viewmenu
    Inherits System.Web.UI.UserControl

    Protected m_refCommon As New CommonApi
    Protected m_refstyle As New StyleHelper
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected m_refMsg As EkMessageHelper
    Protected m_strPageAction As String = ""
    Protected m_refContent As Content.EkContent
    Protected m_refContentApi As ContentAPI
    Protected MenuId As Long = 0
    Protected MenuLanguage As Integer = -1
    Protected language_data As LanguageData
    Protected menu_item_data As List(Of AxMenuItemData)
    Protected ParentId As Long = 0
    Protected m_strViewItem As String = "item"
    Protected AddDeleteIcon As Boolean = False
    Protected m_strMenuName As String = ""
    Protected m_intCurrentPage As Integer = 1
    Protected m_intTotalPages As Integer = 1
    Protected m_strDelConfirm As String = ""
    Protected m_strDelItemsConfirm As String = ""
    Protected m_strSelDelWarning As String = ""
    Protected objLocalizationApi As New LocalizationAPI()
    Protected m_strBackPage As String = ""      ' URL to use to return to the current menu page

    Protected m_strTitle As String = ""
    Protected m_strImage As String = ""
    Protected m_strLink As String = ""
    Protected m_strTemplate As String = ""
    Protected m_strDescription As String = ""
    Protected m_strFolderAssociations As String = ""
    Protected m_strTemplateAssociations As String = ""
    Protected sitePath As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = m_refCommon.EkMsgRef
        AppImgPath = m_refCommon.AppImgPath
        AppPath = m_refCommon.AppPath
        m_strPageAction = Request.QueryString("action")
        Utilities.SetLanguage(m_refCommon)
        MenuLanguage = m_refCommon.ContentLanguage
        MenuId = Convert.ToInt64(Request.QueryString("menuid"))
        If (Request.QueryString("view") IsNot Nothing) Then
            m_strViewItem = Request.QueryString("view")
        End If
        m_refContent = m_refCommon.EkContentRef
        m_refContentApi = New ContentAPI()
        Utilities.SetLanguage(m_refContentApi)
        sitePath = m_refCommon.SitePath

        m_strBackPage = Request.QueryString.ToString()
        ' strip off refresh indicator
        If (m_strBackPage.EndsWith("&rf=1")) Then
            ' refresh is needed after we edit a submenu, but we don't want to keep refreshing if we use the same URL
            m_strBackPage = m_strBackPage.Substring(0, m_strBackPage.Length - 5)
        End If

        DisplayPage()
    End Sub

    Private Sub DisplayPage()
        Dim menu As AxMenuData
        menu = m_refContentApi.EkContentRef.GetMenuDataByID(MenuId)

        If (menu IsNot Nothing) Then
            m_strMenuName = menu.Title
            m_strTitle = menu.Title
            m_strImage = menu.Image
            If (menu.Image = "") Then
                chkOverrideImage.Visible = False
            Else
                chkOverrideImage.Text = m_refMsg.GetMessage("alt Use image instead of a title")
                If (menu.ImageOverride) Then
                    chkOverrideImage.Checked = True
                End If
            End If
            If (menu.Link <> "") Then
                m_strLink = sitePath & menu.Link
            End If
            If (menu.Template <> "") Then
                m_strTemplate = sitePath & menu.Template
            End If
            m_strDescription = menu.Description

            Dim folderid As String
            If (menu.AssociatedFolderIdList IsNot Nothing) Then
                For Each folderid In menu.AssociatedFolderIdList.Split(";")
                    If (folderid <> "") Then
                        Dim folderinfo As FolderData = m_refContentApi.GetFolderById(folderid)
                        If (folderinfo IsNot Nothing) Then
                            If (m_strFolderAssociations <> "") Then
                                m_strFolderAssociations = m_strFolderAssociations & "<BR>"
                            End If
                            m_strFolderAssociations = m_strFolderAssociations & folderinfo.NameWithPath & " (ID:" & folderid & ")"
                        End If
                    End If
                Next
            End If

            Dim template As String
            If (menu.AssociatedTemplates IsNot Nothing) Then
                For Each template In menu.AssociatedTemplates.Split(";")
                    If (template <> "") Then
                        If (m_strTemplateAssociations <> "") Then
                            m_strTemplateAssociations = m_strTemplateAssociations & "<BR>"
                        End If
                        m_strTemplateAssociations = m_strTemplateAssociations & template
                    End If
                Next
            End If
        End If

        If (m_strPageAction <> "viewcontent") Then
            chkOverrideImage.Enabled = False
        End If

        MenuToolBar(menu)
    End Sub

    Private Sub MenuToolBar(ByVal menu As AxMenuData)
        Dim strDeleteMsg As String = ""

        strDeleteMsg = m_refMsg.GetMessage("alt delete button text (menu)")
        m_strDelConfirm = m_refMsg.GetMessage("delete menu confirm")
        m_strDelItemsConfirm = m_refMsg.GetMessage("delete menu items confirm")
        m_strSelDelWarning = m_refMsg.GetMessage("select menu item missing warning")

        divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("view menu title") & " """ & m_strMenuName & """" & "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" & objLocalizationApi.GetFlagUrlByLanguageID(MenuLanguage) & "' />")
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        Dim backPage As String = Server.UrlEncode(Request.Url.ToString())

        result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/contentEdit.png", "collections.aspx?action=EditMenu&nid=" & MenuId & "&folderid=" & menu.FolderID & "&back=" & backPage _
                                                         , m_refMsg.GetMessage("edit menu title"), m_refMsg.GetMessage("edit menu title"), ""))

        If (m_strPageAction <> "viewcontent") Then
            result.Append(m_refstyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "menu.aspx?action=viewcontent&view=item&menuid=" & MenuId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If

        Dim ParentMenuId As Long = menu.ParentID
        Dim AncestorMenuId As Long = menu.AncestorID
        Dim FolderID As Long = menu.FolderID

        result.Append("<td>" & m_refstyle.GetHelpButton("ViewMenu") & "</td>")
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub

End Class
