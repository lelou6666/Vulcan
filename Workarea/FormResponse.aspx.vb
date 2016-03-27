Imports Ektron.Cms
Imports System.Data

Partial Class formresponse
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub
    Protected FormId As Long
    Protected CurrentUserId As Long
    Protected StartDate As String
    Protected EndDate As String
    Protected gtForm As Collection
    Protected gtForms As Collection
    Protected m_refStyle As New StyleHelper
    Protected m_refContentApi As New ContentAPI
    Protected DefaultFormTitle As String = ""
    Protected DisplayType As String = ""
    Protected sFormDataIds As String = ""
    Protected DataType As String = ""
    Protected AppImgPath As String = ""
    Protected ContentLanguage As Integer = -1
    Protected Flag As String = "false"
    Protected Security_info As PermissionData
    Protected Action As String = ""
    Protected ResultType As String = ""
    Protected EnableMultilingual As Integer = 0
    Protected objForm As Ektron.Cms.Modules.EkModule
    Protected strFolderID As String
#End Region

#Const SaveXmlAsFile = False

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Dim FormFieldStats As Collection
        Dim FormStats As New Collection
        'Dim item As Collection
        Dim Target As String

        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1

        StyleSheetJS.Text = (New StyleHelper).GetClientScript
        'Put user code to initialize the page here
        ContentLanguage = m_refContentApi.ContentLanguage
        AppImgPath = m_refContentApi.AppImgPath
        FormId = Request.QueryString("id")
        DisplayType = Request.QueryString("display_type")
        Target = Request.QueryString("display_target")

        If (Request.QueryString("LangType") <> "") Then
            ContentLanguage = Request.QueryString("LangType")
            m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
        Else
            If CStr(m_refContentApi.GetCookieValue("LastValidLanguageID")) <> "" Then
                ContentLanguage = m_refContentApi.GetCookieValue("LastValidLanguageID")
            End If
        End If

        EnableMultilingual = m_refContentApi.EnableMultilingual
        m_refContentApi.ContentLanguage = ContentLanguage

        Security_info = m_refContentApi.LoadPermissions(FormId, "content")
        objForm = m_refContentApi.EkModuleRef

        If ((Convert.ToString(FormId) <> "") AndAlso (FormId > 0)) Then
            DefaultFormTitle = objForm.GetFormTitleById(FormId)
        Else
            DefaultFormTitle = Request.QueryString("FormTitle")
        End If

        Dim i As Integer
        Dim strNames As String = ""
        'Dim strNamesArray As Array
        Dim strStale As String = ""
        'Dim count As Integer
        Dim strFieldNames As String = ""
        Dim strFieldOptionNames As String = ""
        Dim strFieldOptionValues As String = ""
        Dim cResult As New ArrayList
        Dim cItem As ArrayList
        Dim hshQuestions As Hashtable
        Dim llResponses As Integer
        cResult = m_refContentApi.GetFormDataHistogramById(CLng(FormId))
        'llResponses = CInt(cResult.Item(0)(0).ToString.Substring(cResult.Item(0)(0).ToString.IndexOf("responses") + 9))
        llResponses = m_refContentApi.EkModuleRef.GetFormSubmissionsByFormId(CLng(FormId))
        hshQuestions = m_refContentApi.GetFormFieldQuestionsById(CLng(FormId))

        If (DisplayType = "1") Then
            FormReportGrid.Visible = True
            Dim colBound As BoundColumn
            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "Names"
            colBound.HeaderText = ""
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.ItemStyle.Wrap = False
            FormReportGrid.Columns.Add(colBound)

            colBound = New System.Web.UI.WebControls.BoundColumn
            colBound.DataField = "Percent"
            colBound.HeaderText = ""
            colBound.HeaderStyle.CssClass = "title-header"
            colBound.ItemStyle.Wrap = False
            colBound.ItemStyle.VerticalAlign = VerticalAlign.Top
            colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left
            FormReportGrid.Columns.Add(colBound)

            FormReportGrid.BorderColor = Drawing.Color.White

            Dim dt As New DataTable
            'Dim dr As DataRow
            dt.Columns.Add(New DataColumn("Names", GetType(String)))
            dt.Columns.Add(New DataColumn("Percent", GetType(String)))
            Dim j As Integer

            
            ' loop through the names and value pairs to populate the dr and add the dr to dt
            lblTbl.Visible = True
            lblTbl.Text = ""
            lblTbl.Text = lblTbl.Text & "<Table>"
            lblTbl.Text = lblTbl.Text & "<tr><td align=center><B><U>Responses - " & llResponses.ToString() & "</U></B></td></tr>"
            lblTbl.Text = lblTbl.Text & "</table>"

            For i = 0 To cResult.Count - 1
                cItem = cResult.Item(i)
                lblTbl.Text = lblTbl.Text & "<Table>"
                lblTbl.Text = lblTbl.Text & "<tr><td align=center><B><U>" & hshQuestions(cItem.Item(0)) & "</U></B></td></tr>"
                lblTbl.Text = lblTbl.Text & "</table>"
                lblTbl.Text = lblTbl.Text & "<table>"
                For j = 1 To cItem.Count - 1
                    lblTbl.Text = lblTbl.Text & "<tr><td>"
                    lblTbl.Text = lblTbl.Text & "<b>" & CInt(((cItem.Item(j).ToString().Substring(cItem.Item(j).ToString().LastIndexOf(",") + 1)) * 100) / llResponses) & "%" & "</b>&nbsp;&nbsp;&nbsp;" & cItem.Item(j).ToString().Substring(0, cItem.Item(j).ToString().LastIndexOf(",") - 5)
                    lblTbl.Text = lblTbl.Text & "</td></tr>"
                Next
                lblTbl.Text = lblTbl.Text & "</Table>"
            Next

            'dt.Rows.Add(dr)
            'Dim dv As New DataView(dt)
            'FormReportGrid.DataSource = dv
            'FormReportGrid.DataBind()
        Else
            chart.Visible = True

            ' Now we have the data get the values
            'For Each item In FormStats
            'strNames = "18-21,22-25,26-30,31-40,41-50,51-60,61-over:10k-20k,21k-30k,31k-40k:High School,Some College,Degree(Associates),Master,Doctoral,Professional"
            ''strNames = "18-21,22-25,26-30,31-40,41-50,51-60,61-over:10k-20k,21k-30k,31k-40k"
            'strStale = "10,30,25,10,5,5,15:10,50,40:10,10,10,10,10,10"
            'strFieldNames = "Age range:Annual Income:Education level"
            strFieldNames = ""
            strFieldOptionNames = ""
            strFieldOptionValues = ""
            'EktComma is used to retain the commas in the fields and field option names
            Dim idx, j As Integer
            'Dim scaleFactor As Double

            For idx = 0 To cResult.Count - 1
                cItem = cResult.Item(idx)
                If cItem.Count > 0 Then
                    strFieldNames = strFieldNames & ":" & hshQuestions(cItem.Item(0).ToString().Replace(",", "EktComma"))
                End If
            Next

            strFieldNames = Server.UrlEncode(strFieldNames.Substring(1, strFieldNames.Length - 1))

            cItem = Nothing
            'For Each cItem In cResult

            For idx = 0 To cResult.Count - 1
                cItem = cResult.Item(idx)
                For j = 1 To cItem.Count - 1
                    strFieldOptionNames = strFieldOptionNames & cItem.Item(j).ToString().Substring(0, cItem.Item(j).ToString().LastIndexOf(",") - 5).Replace(",", "EktComma") & ","
                    strFieldOptionValues = strFieldOptionValues & CInt((cItem.Item(j).ToString().Substring(cItem.Item(j).ToString().LastIndexOf(",") + 1) * 100) / llResponses) & ","
                Next

                strFieldOptionNames = strFieldOptionNames.Substring(0, strFieldOptionNames.Length - 1) & ":"
                strFieldOptionValues = strFieldOptionValues.Substring(0, strFieldOptionValues.Length - 1) & ":"
            Next

            strFieldOptionNames = Server.UrlEncode(strFieldOptionNames.Substring(0, strFieldOptionNames.Length - 1))
            strFieldOptionValues = strFieldOptionValues.Substring(0, strFieldOptionValues.Length - 1)

            If (DisplayType = "2") Then
                DisplayType = "0" 'Horizontal bar chart in chart.aspx
            End If

            Dim showAxis As Boolean = False
            chart.ImageUrl = "chart.aspx?fieldOptionNames=" & strFieldOptionNames & "&FieldNames=" & strFieldNames & "&FormValues=" & strFieldOptionValues & "&form_page=form_page&grpdisplay=" & DisplayType & "&responses=" & llResponses & "&showAxis=" & showAxis '& Request.QueryString("report_display")
            'Next
        End If
        



    End Sub

End Class
