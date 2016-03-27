Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Microsoft.Security.Application

Partial Class editxml_config
    Inherits System.Web.UI.UserControl

    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Common.EkMessageHelper
    Protected PageAction As String = ""
    Protected AppImgPath As String = ""
    Protected AppPath As String = ""
    Protected EnableMultilingual As Integer = 0
    Protected ContentLanguage As Integer = 0
    Protected _ContentApi As New ContentAPI
    Protected XMLList As System.Web.UI.WebControls.DataGrid
    Protected m_strOrderBy As String = "title"
    Protected ConfigId As Long = 0
    Protected cXmlCollection As XmlConfigData
    Protected m_intId As Long = 0
    Protected pkDisplay As String
    Protected bDefaultXsltExists As Boolean = False
    Protected XmlPath As String = ""
    Protected Xslt1Checked As String = ""
    Protected m_strTitle As String

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here
        RegisterResources()
        m_refMsg = _ContentApi.EkMsgRef
        If (Not (IsNothing(Request.QueryString("action")))) Then
            If (Request.QueryString("action") <> "") Then
                PageAction = Request.QueryString("action").ToLower
            End If
        End If
        AppImgPath = _ContentApi.AppImgPath
        AppPath = _ContentApi.AppPath
        XmlPath = _ContentApi.XmlPath
    End Sub

#Region "XmlConfigs"
    Public Function EditXmlConfig() As Boolean
        If (Not (IsNothing(Request.QueryString("id")))) Then
            If (Request.QueryString("id") <> "") Then
                m_intId = Request.QueryString("id")
            End If
        End If
        If (Not (Page.IsPostBack)) Then
            If (PageAction.ToString().ToLower() = "newinheritconfiguration") Then
                Display_NewInheritConfiguration()
            Else
                Display_EditXmlConfig()
            End If
            Return (False)
        Else
            cXmlCollection = New XmlConfigData
            If (PageAction = "newinheritconfiguration") Then
                Process_ReplicateXmlConfig()
                Return (True)
            Else
                Return (Process_UpdateXmlConfig())
            End If
        End If
    End Function
    Public Function AddXmlConfig() As Long
        If (Not (Page.IsPostBack)) Then
            Display_AddXmlConfig()
        Else
            Return (Process_AddXmlConfig())
        End If
    End Function
    Private Sub Display_AddXmlConfig()
        AddXmlConfigToolBar()
    End Sub
    Private Sub Display_EditXmlConfig()
        cXmlCollection = _ContentApi.GetXmlConfiguration(m_intId)
        If (cXmlCollection Is Nothing) Then
            cXmlCollection = New XmlConfigData
        End If
        EditXmlConfigToolBar()
        m_strTitle = cXmlCollection.Title
        pkDisplay = cXmlCollection.PackageDisplayXslt
    End Sub
    Private Sub Display_NewInheritConfiguration()
        InheritXmlConfigToolBar()
    End Sub
    Private Function Process_AddXmlConfig() As Long
        Dim cXml As New Collection
        Dim retVal As Long = 0
        Try
            cXml.Add(EkEnumeration.XmlConfigType.Content, "Type")
            cXml.Add(Request.Form("frm_xmltitle"), "CollectionTitle")
            cXml.Add(AntiXss.HtmlEncode(Request.Form("frm_xmldescription")), "CollectionDescription")
            cXml.Add(Request.Form("frm_editxslt"), "EditXslt")
            cXml.Add(Request.Form("frm_savexslt"), "SaveXslt")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt1")), "Xslt1")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt2")), "Xslt2")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt3")), "Xslt3")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt4")), "Xslt4")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt5")), "Xslt5")
            cXml.Add(Request.Form("frm_xmlschema"), "XmlSchema")
            cXml.Add(Request.Form("frm_xmlnamespace"), "XmlNameSpace")
            cXml.Add(Request.Form("frm_xmladvconfig"), "XmlAdvConfig")
            cXml.Add(Request.Form("frm_xsltdefault"), "DefaultXslt")
            cXml.Add(Server.MapPath(XmlPath), "PhysicalPath")
            retVal = _ContentApi.AddXmlConfiguration(cXml)
            If (retVal = -1) Then
                Display_AddXmlConfig()
                lbl_addXmlError.Visible = True
                lbl_addXmlError.Text = "<tr><td colspan=""2"">" & _ContentApi.EkMsgRef.GetMessage("lbl smart form unique title required") & "</td></tr>"
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message.ToString())
        End Try
        Return (retVal)
    End Function
    Private Sub Process_ReplicateXmlConfig()
        Dim newTitle As String
        Dim iRet As Integer
        Try
            newTitle = Request.Form("frm_xmltitle")
            iRet = _ContentApi.ReplicateXmlConfiguration(m_intId, newTitle)
            If (iRet > 0) Then
                Response.Redirect("xml_config.aspx?action=ViewXmlConfiguration&id=" & iRet, False)
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message.ToString())
        End Try
    End Sub
    Private Function Process_UpdateXmlConfig() As Boolean
        Dim cXml As New Collection
        Try
            cXml.Add(Request.Form("frm_collectionid"), "CollectionID")
            cXml.Add(Request.Form("frm_xmltitle"), "CollectionTitle")
            cXml.Add(AntiXss.HtmlEncode(Request.Form("frm_xmldescription")), "CollectionDescription")
            cXml.Add(Request.Form("frm_editxslt"), "EditXslt")
            cXml.Add(Request.Form("frm_savexslt"), "SaveXslt")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt1")), "Xslt1")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt2")), "Xslt2")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt3")), "Xslt3")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt4")), "Xslt4")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt5")), "Xslt5")
            cXml.Add(Request.Form("frm_xmlschema"), "XmlSchema")
            cXml.Add(Request.Form("frm_xmlnamespace"), "XmlNameSpace")
            cXml.Add(Request.Form("frm_xmladvconfig"), "XmlAdvConfig")
            cXml.Add(Request.Form("frm_xsltdefault"), "DefaultXslt")
            cXml.Add(Server.MapPath(XmlPath), "PhysicalPath")
            _ContentApi.UpdateXmlConfiguration(cXml)
        Catch ex As Exception
            Utilities.ShowError(ex.Message.ToString())
        End Try
        Return (True)
    End Function
    Private Sub AddXmlConfigToolBar()
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>")
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add xml config msg"))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("lbl Select to continue"), m_refMsg.GetMessage("btn save"), "onclick=""return SubmitForm('xmlconfiguration', 'VerifyXmlForm()');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "xml_config.aspx?page=xml_config.aspx&action=ViewAllXmlConfigurations", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Private Function ValidateXSLT(ByVal GivenXslt As Object) As String
        Dim strReturn As String = ""
        Try
            strReturn = Convert.ToString(GivenXslt)
            '
            '#21753 - custom xsl getting errors when used with a smart form.
            'The error appears to be overriding the path to use the XSL with a ..\\file.xsl instead of ..\file.xsl.
            '
            'If (strReturn.Trim <> "" AndAlso strReturn.ToLower.IndexOf("http") = -1) Then
            '    If ((Left(strReturn, 1) <> "/") And (Left(strReturn, 1) <> "\")) Then
            '        strReturn = "/" & strReturn
            '    End If
            'End If
        Catch Ex As Exception
            strReturn = ""
        End Try
        Return (strReturn)
    End Function
    Private Sub EditXmlConfigToolBar()
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>")
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("edit xml config msg") & " """ & cXmlCollection.Title & """")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (xml config)"), m_refMsg.GetMessage("btn update"), "onclick=""return SubmitForm('xmlconfiguration', 'VerifyXmlForm()');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "xml_config.aspx?action=ViewXmlConfiguration&id=" & m_intId & "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
    Private Sub InheritXmlConfigToolBar()
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>")
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add xml config msg"))
        'result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (xml config)"), m_refMsg.GetMessage("btn update"), "Onclick=""javascript:return SubmitForm('xmlconfiguration', 'VerifyXmlForm()');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("lbl Select to continue"), m_refMsg.GetMessage("btn add xml"), "onclick=""return SubmitForm('xmlconfiguration', 'VerifyXmlForm()');"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppPath & "images/UI/Icons/back.png", "xml_config.aspx?action=ViewXmlConfiguration&id=" & m_intId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton(PageAction))
        result.Append("</td>")
        result.Append("</tr></table>")
        htmToolBar.InnerHtml = result.ToString
        result = Nothing
    End Sub
#End Region

#Region "Product Types"
    Dim m_sProductTypePage As String = "producttypes.aspx"

    Public Function EditProductType() As Boolean
        If (Not (IsNothing(Request.QueryString("id")))) Then
            If (Request.QueryString("id") <> "") Then
                m_intId = Request.QueryString("id")
            End If
        End If
        If (Not (Page.IsPostBack)) Then
            If (PageAction.ToString().ToLower() = "newinheritconfiguration") Then
                ' Display_NewInheritProductType()
            Else
                Display_EditProductType()
            End If
            Return (False)
        Else
            cXmlCollection = New XmlConfigData
            If (PageAction = "newinheritconfiguration") Then
                ' Process_ReplicateXmlConfig()
                Return (True)
            Else
                Return (Process_UpdateProductType())
            End If
        End If
    End Function
    Private Sub Display_EditProductType()
        cXmlCollection = _ContentApi.GetXmlConfiguration(m_intId)
        If cXmlCollection Is Nothing Then cXmlCollection = New XmlConfigData()
        lbl_desc.Text = m_refMsg.GetMessage("generic description")
        ' m_strTitle = pProductType.Title
        ' pkDisplay = pProductType.PackageDisplayXslt
    End Sub
    Private Sub Display_NewInheritProductType()
        InheritProductTypeToolBar()
    End Sub
    Private Sub Process_ReplicateProductType()
        Dim newTitle As String
        Dim iRet As Integer
        Try
            newTitle = Request.Form("frm_xmltitle")
            iRet = _ContentApi.ReplicateXmlConfiguration(m_intId, newTitle)
            If (iRet > 0) Then
                Response.Redirect(m_sProductTypePage & "?action=ViewXmlConfiguration&id=" & iRet, False)
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message.ToString())
        End Try
    End Sub
    Private Function Process_UpdateProductType() As Boolean
        Dim cXml As New Collection
        Try
            cXml.Add(m_intId, "CollectionID")
            cXml.Add(EkEnumeration.XmlConfigType.Product, "Type")
            ' cXml.Add(Request.Form(drp_edittype.UniqueID), "SecondType")
            cXml.Add(Request.Form("frm_xmltitle"), "CollectionTitle")
            cXml.Add(AntiXss.HtmlEncode(Request.Form("frm_xmldescription")), "CollectionDescription")
            cXml.Add(Request.Form("frm_editxslt"), "EditXslt")
            cXml.Add(Request.Form("frm_savexslt"), "SaveXslt")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt1")), "Xslt1")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt2")), "Xslt2")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt3")), "Xslt3")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt4")), "Xslt4")
            cXml.Add(ValidateXSLT(Request.Form("frm_Xslt5")), "Xslt5")
            cXml.Add(Request.Form("frm_xmlschema"), "XmlSchema")
            cXml.Add(Request.Form("frm_xmlnamespace"), "XmlNameSpace")
            cXml.Add(Request.Form("frm_xmladvconfig"), "XmlAdvConfig")
            cXml.Add(Request.Form("frm_xsltdefault"), "DefaultXslt")
            cXml.Add(Server.MapPath(XmlPath), "PhysicalPath")
            _ContentApi.UpdateXmlConfiguration(cXml)
        Catch ex As Exception
            Utilities.ShowError(ex.Message.ToString())
        End Try
        Return (True)
    End Function
    Private Sub InheritProductTypeToolBar()
        'Dim result As New System.Text.StringBuilder
        'result.Append("<table><tr>")
        'txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add xml config msg"))
        'result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_addxml-nm.gif", "#", m_refMsg.GetMessage("lbl Select to continue"), m_refMsg.GetMessage("btn add xml"), "Onclick=""javascript:return SubmitForm('xmlconfiguration', 'VerifyXmlForm()');"""))
        'result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", m_sProductTypePage & "?action=viewproducttype&id=" & m_intId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        'result.Append("<td>")
        'result.Append(m_refStyle.GetHelpButton(PageAction))
        'result.Append("</td>")
        'result.Append("</tr></table>")
        'htmToolBar.InnerHtml = result.ToString
        'result = Nothing
    End Sub
#End Region
    Private Sub RegisterResources()
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronWorkareaHelperJS)
        Ektron.Cms.API.JS.RegisterJS(Me, API.JS.ManagedScript.EktronToolBarRollJS)
    End Sub
End Class
