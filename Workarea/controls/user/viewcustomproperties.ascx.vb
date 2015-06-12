Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.Common.EkConstants
Imports System.Collections.Generic
Imports Ektron.Cms.Core.CustomProperty
imports Ektron.Cms.Framework.Core
Imports Ektron.Cms.Framework.Core.CustomProperty
Imports Ektron.Cms.common
Partial Class viewcustomproperties
    Inherits System.Web.UI.UserControl

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Protected m_refComAPI As New CommonApi
    Private m_UCPData As UserCustomPropertyData()
    Private PageAction As String = "ViewCustomProp"
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppImgPath As String = String.Empty
    Protected ContentLanguage As Integer = -1
    Protected EnableMultiLanguage As Integer = -1
    Protected _coreCustomProperty As New CustomPropertyBL


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim EkUserObj As User.EkUser = m_refComAPI.EkUserRef
        Dim strOrder As String
        Dim bRet As Boolean = False
        'Test Read ALL
        'Dim AllProperty As UserCustomPropertyData()
        RegisterResources()
        m_refMsg = m_refComAPI.EkMsgRef
        AppImgPath = m_refComAPI.AppImgPath
        EnableMultiLanguage = m_refComAPI.EnableMultilingual
        If (Request.QueryString("LangType") <> "") Then
            ContentLanguage = Request.QueryString("LangType")
            m_refComAPI.SetCookieValue("LastValidLanguageID", ContentLanguage)
        Else
            If CStr(m_refComAPI.GetCookieValue("LastValidLanguageID")) <> "" Then
                ContentLanguage = m_refComAPI.GetCookieValue("LastValidLanguageID")
            End If
        End If

        If ContentLanguage = ALL_CONTENT_LANGUAGES Or ContentLanguage = CONTENT_LANGUAGES_UNDEFINED Then
            ContentLanguage = m_refComAPI.DefaultContentLanguage
            m_refComAPI.SetCookieValue("LastValidLanguageID", ContentLanguage)
        End If
        m_refComAPI.ContentLanguage = ContentLanguage

        If (Not Request.QueryString("action") Is Nothing) Then
            If (Request.QueryString("action") <> "") Then
                PageAction = Request.QueryString("action")
            End If
        End If

        m_UCPData = EkUserObj.GetAllCustomProperty("") 'make sure to do a get before toolbar.
        If (PageAction.ToString().ToLower() = "reorderproperties") Then
            If (IsPostBack) Then
                strOrder = Request.Form("LinkOrder")
                bRet = EkUserObj.UpdateCustomPropertiesItemOrder(strOrder)
                If (bRet) Then
                    Response.Redirect("users.aspx?action=ViewCustomProp", False)
                End If
            Else
                Populate_ReOrder()
                ReOrderProperties_Toolbar()
            End If
        Else
            ViewCustomProperties_Toolbar()
            Populate_ViewGrid()
        End If
    End Sub
    Private Sub Populate_ViewGrid()
        Dim i As Integer
        Dim colBound As New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "TITLE"
        colBound.HeaderText = m_refMsg.GetMessage("generic Title")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(70)
        colBound.ItemStyle.Width = Unit.Percentage(70)
        ViewAllGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "ID"
        colBound.HeaderText = m_refMsg.GetMessage("generic Id")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(5)
        colBound.ItemStyle.Width = Unit.Percentage(5)
        ViewAllGrid.Columns.Add(colBound)

        colBound = New System.Web.UI.WebControls.BoundColumn
        colBound.DataField = "VALUE"
        colBound.HeaderText = m_refMsg.GetMessage("generic type")
        colBound.ItemStyle.Wrap = False
        colBound.HeaderStyle.Width = Unit.Percentage(20)
        colBound.ItemStyle.Width = Unit.Percentage(20)
        ViewAllGrid.Columns.Add(colBound)

        'colBound = New System.Web.UI.WebControls.BoundColumn
        'colBound.DataField = "REQ"
        'colBound.HeaderText = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"    
        'ViewAllGrid.Columns.Add(colBound)

        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("TITLE", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("VALUE", GetType(String)))
        dt.Columns.Add(New DataColumn("REQ", GetType(String)))

        If (Not (IsNothing(m_UCPData))) Then
            For i = 0 To m_UCPData.Length - 1
                dr = dt.NewRow
                dr(0) = "<a href=""users.aspx?action=editcustomprop&id=" & m_UCPData(i).ID & """>" & m_UCPData(i).Name & "</a>"
                dr(1) = m_UCPData(i).ID.ToString()
                dr(2) = "<a href=""users.aspx?action=editcustomprop&id=" & m_UCPData(i).ID & """>" & m_UCPData(i).PropertyValueType.ToString() & "</a>"
                'If (m_UCPData(i).Required) Then
                '    dr(3) = "<input type=""checkbox"" disabled checked=true />"
                'Else
                '    dr(3) = "<input type=""checkbox"" disabled />"
                'End If
                dt.Rows.Add(dr)
            Next
        End If
        Dim dv As New DataView(dt)
        ViewAllGrid.DataSource = dv
        ViewAllGrid.DataBind()

    End Sub
    Private Sub ViewCustomProperties_Toolbar()
        Dim result As New System.Text.StringBuilder

        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("user custom props view"))
        result.Append("<table><tr>")
        If (Me.m_refComAPI.ContentLanguage = Me.m_refComAPI.DefaultContentLanguage) Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/add.png", "users.aspx?action=addcustomprop", m_refMsg.GetMessage("alt add button text (user property)"), m_refMsg.GetMessage("btn add"), ""))
            If ((Not m_UCPData Is Nothing) AndAlso (m_UCPData.Length > 1)) Then
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/arrowUpDown.png", "users.aspx?action=reorderproperties", m_refMsg.GetMessage("alt reorder items"), m_refMsg.GetMessage("btn reorder"), ""))
            End If
        End If
        If (EnableMultiLanguage = 1) Then
            result.Append("<td align=""right"">" & m_refStyle.ShowAllActiveLanguage(False, "", "javascript:SelLanguage(this.value);", ContentLanguage) & "</td>")
        Else
            result.Append("<td>&nbsp;</td>")
        End If
        result.Append("<td>&nbsp;&nbsp;|&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl object type") + ":&nbsp;" + GetObjectTypeDropDown("SetObjectType(this); return false;"))

        'result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "users.aspx?action=ViewUsersByGroup&LangType=" & ContentLanguage & "&FromUsers=" & Request.QueryString("FromUsers") & "&id=2&OrderBy=" & Request.QueryString("OrderBy"), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("ViewCustomProperties"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
    Private Sub Populate_ReOrder()
        litReOrder.Text = ""
        Dim sBuild As New System.Text.StringBuilder
        Dim reOrderList As String = String.Empty
        Dim data As UserCustomPropertyData

        sBuild.Append("")
        sBuild.Append("<div class=""ektronPageInfo"">")
        sBuild.Append("<table>")
        sBuild.Append("	<tr>")
        sBuild.Append("     <td>")
        sBuild.Append("         <select name=""OrderList"" size=""" & IIf(m_UCPData.Length < 20, m_UCPData.Length.ToString(), "20") & """>")
        reOrderList = ""
        For Each data In m_UCPData
            If (Len(reOrderList)) Then
                reOrderList = reOrderList & "," & data.ID
            Else
                reOrderList = data.ID
            End If
            sBuild.Append("         <option value=""" & data.ID & """>" & data.Name)
        Next
        sBuild.Append("         </select>")
        sBuild.Append("     </td>")
        sBuild.Append("     <td>&nbsp;&nbsp;</td>")
        sBuild.Append("		<td>")
        sBuild.Append("         <a href=""javascript:Move('up', document.forms[0].OrderList, document.forms[0].LinkOrder)""><img src=""" & Me.m_refComAPI.AppImgPath & "../UI/Icons/arrowHeadUp.png"" alt=""" & m_refMsg.GetMessage("move selection up msg") & """ title=""" & m_refMsg.GetMessage("move selection up msg") & """></a>")
        sBuild.Append("         <br />")
        sBuild.Append("			<a href=""javascript:Move('dn', document.forms[0].OrderList, document.forms[0].LinkOrder)""><img src=""" & Me.m_refComAPI.AppImgPath & "../UI/Icons/arrowHeadDown.png"" alt=""" & m_refMsg.GetMessage("move selection down msg") & """ title=""" & m_refMsg.GetMessage("move selection down msg") & """></a>")
        sBuild.Append("		</td>")
        sBuild.Append("	</tr>")
        sBuild.Append("</table>")
        sBuild.Append("</div>")

        If (reOrderList.Length > 0) Then
            sBuild.Append("<script language=""javascript"">")
            sBuild.Append("document.forms[0].OrderList[0].selected = true;")
            sBuild.Append("</script>")
        End If
        sBuild.Append("		<input type=""hidden"" name=""LinkOrder"" value=""" & reOrderList & """>")
        litReOrder.Text = sBuild.ToString()
    End Sub
    Private Sub ReOrderProperties_Toolbar()
        Dim result As New System.Text.StringBuilder
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("user custom props reorder"))
        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('userinfo', 'true');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "users.aspx?action=viewcustomprop", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("ReorderCustomProperties"))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
    End Sub
#Region "GridItemBound"
    Protected Sub Grid_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        If (PageAction.ToString().ToLower() = "viewcustomprop") Then
            Select Case e.Item.ItemType
                Case ListItemType.AlternatingItem, ListItemType.Item
                    If (e.Item.Cells(1).Text.Equals("REMOVE-ITEM") Or e.Item.Cells(1).Text.Equals("important") Or e.Item.Cells(1).Text.Equals("input-box-text")) Then
                        e.Item.Cells(0).Attributes.Add("align", "Left")
                        e.Item.Cells(0).ColumnSpan = 2
                        If (e.Item.Cells(0).Text.Equals("REMOVE-ITEM")) Then
                            'e.Item.Cells(0).CssClass = ""
                        Else
                            e.Item.Cells(0).CssClass = e.Item.Cells(1).Text
                        End If
                        e.Item.Cells.RemoveAt(1)
                    End If
            End Select
        End If
    End Sub
#End Region
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaJS)
    End Sub
    Public Function GetObjectTypeDropDown(ByVal OnChangeEvt As String) As [String]
        Dim result As New StringBuilder()
        Dim _comObjectType As List(Of Ektron.Cms.Common.EkEnumeration.CustomPropertyObjectType) = _coreCustomProperty.GetObjectTypeList()

        Try

            Dim _objectType As List(Of Ektron.Cms.Common.EkEnumeration.CustomPropertyObjectType) = _coreCustomProperty.GetObjectTypeList()

            result.Append("<select name='objectType' id='objectType' onchange=""" & OnChangeEvt & """>")
            result.Append("<option value='0' selected='selected'>")
            result.Append("User")
            result.Append("</option>")

            ' Right now there is only one CustomPropertyObjectType that is TaxonomyNode,
            'Need to implement objectType in the future as new ones are added.

            result.Append(" <option value='1' >")
            result.Append(GetObjectTypeString(Ektron.Cms.Common.EkEnumeration.CustomPropertyObjectType.TaxonomyNode))
            result.Append("</option>")

            'If _comObjectType IsNot Nothing Then
            '    Dim objectTypeValue As Ektron.Cms.Common.EkEnumeration.CustomPropertyObjectType() = _objectType.ToArray()
            '    For iObj As Integer = 0 To _objectType.Count - 1
            '        result.Append("<option value=""" & iObj + 1 & """")
            '        result.Append(">")
            '        result.Append(_objectType.Item(iObj))
            '        result.Append("</option>")
            '    Next
            'End If
            result.Append("</select>")
        Catch ex As Exception
            EkException.WriteToEventLog(("CMS400: " & ex.Message & ":") + ex.StackTrace, System.Diagnostics.EventLogEntryType.[Error])
            result.Length = 0
        End Try

        Return (result.ToString())
    End Function
    Private Function GetObjectTypeString(ByVal objectType As EkEnumeration.CustomPropertyObjectType) As String
        Return m_refMsg.GetMessage("CmsObjectType" + objectType.ToString())
    End Function

End Class