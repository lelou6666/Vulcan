Imports System.Collections.Generic
Imports Ektron.Cms.Widget
Imports System.Reflection

Public Class PropertyData
    Public FullTypeName As String
    Public TypeName As String
    Public Name As String
    <System.Xml.Serialization.XmlIgnore()> Public Property Type() As Type
        Get
            Dim tmp As Type = System.Type.GetType(TypeName)
            If tmp Is Nothing Then
                tmp = System.Type.GetType(FullTypeName)
            End If
            Return tmp
        End Get
        Set(ByVal value As Type)
            TypeName = value.Name
            FullTypeName = value.AssemblyQualifiedName
        End Set
    End Property
    Public Value As Object
End Class

Partial Class Workarea_controls_widgetSettings_WidgetEdit
    Inherits System.Web.UI.UserControl

    Protected m_refContentApi As New Ektron.Cms.ContentAPI()
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected m_refWidgetModel As New WidgetTypeModel()
    Protected m_refStyle As New StyleHelper()

    Protected info As WidgetTypeData = Nothing
    Protected m_widgetID As Long
    Protected propertydic As New Dictionary(Of PropertyInfo, GlobalWidgetData)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronJS)
        Ektron.Cms.API.JS.RegisterJS(Me, m_refContentApi.AppPath & "java/dateParser.js", "DateJS")

        m_refMsg = m_refContentApi.EkMsgRef

        ToolBar()

        If (Not String.IsNullOrEmpty(Request.QueryString("widgetid"))) Then
            If (Not Long.TryParse(Request.QueryString("widgetid"), m_widgetID)) Then
                ShowError("Could not find that widget")
                Return
            End If
        End If

        If (m_widgetID > -1) Then
            If (Not m_refWidgetModel.FindByID(m_widgetID, info)) Then
                ShowError("Could not find that widget")
                Return
            End If
        End If

        If (info IsNot Nothing) Then
            lblID.Text = info.ID.ToString()
            lblFilename.Text = info.ControlURL
            txtTitle.Text = info.Title
            txtLabel.Text = info.ButtonText


            'get properties marked as global settings and fill in table
            LoadWidgetGlobalProps()
            SetupPropertyEditor()

            viewset.SetActiveView(viewSettings)

            If (Page.IsPostBack) Then
                SaveProperties(Nothing, Nothing)
            End If
        Else
            ShowError("Unknown error")
            Return
        End If
    End Sub

    Private Sub ToolBar()
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("generic edit title") & " " & m_refMsg.GetMessage("generic widget"))
        Dim result As New System.Text.StringBuilder
        result.Append("<table><tr>" & vbCrLf)
        'result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/save.png", _
        '                                                 "#", m_refMsg.GetMessage("alt update button text"), _
        '                                                 m_refMsg.GetMessage("btn update"), _
        '                                                 "onclick=""document.forms[0].submit();"""))
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/back.png", _
            "widgetsettings.aspx?action=widgetsync", _
            m_refMsg.GetMessage("alt back button text"), _
            m_refMsg.GetMessage("btn back"), "onclick=""return closeParentWin();"""))
        result.Append("</tr></table>")
        divToolBar.InnerHtml = result.ToString()
        result = Nothing
    End Sub

    Protected Sub ShowError(ByVal message As String)
        lblError.Text = message
        viewset.SetActiveView(viewError)
    End Sub

    Protected Sub SaveProperties(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim info As WidgetTypeData = Nothing
        Dim str As New StringBuilder

        If (m_widgetID < 0) Then
            ShowError("Could not find that widget")
            Return
        End If

        Dim collection As New List(Of GlobalWidgetPropertySettings)
        If (propertydic.Count > 0) Then
            For Each item As KeyValuePair(Of PropertyInfo, GlobalWidgetData) In propertydic
                Dim conv As New GlobalWidgetPropertySettings
                conv.PropertyName = item.Key.Name
                conv.Type = item.Key.PropertyType
                conv.value = item.Value.getDefault
                collection.Add(conv)
            Next

            For Each propertyitem As RepeaterItem In globalProps.Items
                Dim lblpropname As Label = CType(propertyitem.FindControl("lblPropertyName"), Label)
                For Each listitem As GlobalWidgetPropertySettings In collection
                    If (listitem.PropertyName = lblpropname.Text) Then
                        Dim ph As PlaceHolder = CType(propertyitem.FindControl("phValue"), PlaceHolder)
                        Dim cntpropval As Control = ph.FindControl("value")
                        decodeValue(listitem.Type, cntpropval, listitem.value)
                    End If
                Next
            Next
        End If

        Dim serializer As New System.Xml.Serialization.XmlSerializer(GetType(List(Of GlobalWidgetPropertySettings)))
        Dim s As New IO.StringWriter()
        serializer.Serialize(s, collection)

        'now serialize collection into settings
        Dim title As String = Request.Form(txtTitle.UniqueID)   ' for some reason, the submit doesn't automatically update txtTitle.Text and txtLabel.Text
        Dim label As String = Request.Form(txtLabel.UniqueID)
        If (Not m_refWidgetModel.Update(m_widgetID, title, label, lblFilename.Text, s.GetStringBuilder().ToString(), True)) Then
            ShowError("Could not update widget")
        End If

        successmessage.Text = "Properties Successfully saved"
        str.AppendLine("    window.parent.$ektron('#editWidget').modalHide();")
        viewset.SetActiveView(viewSuccess)

        Ektron.Cms.API.JS.RegisterJSBlock(Me, Str.ToString(), "HideEditModal")
    End Sub

    Protected Sub LoadWidgetGlobalProps()
        If (info IsNot Nothing) Then
            Dim uc As UserControl
            uc = CType(LoadControl(Request.ApplicationPath & "/Widgets/" & info.ControlURL), UserControl)
            If (uc IsNot Nothing) Then
                'collect properties and types
                Dim properties As PropertyInfo() = uc.GetType().GetProperties()
                For Each pi As PropertyInfo In properties
                    Dim propertyattributes As GlobalWidgetData()
                    propertyattributes = CType(pi.GetCustomAttributes(GetType(GlobalWidgetData), True), GlobalWidgetData())
                    If (propertyattributes IsNot Nothing AndAlso propertyattributes.Length > 0) Then
                        propertydic.Add(pi, propertyattributes(0))
                    End If
                Next
            End If
        End If
    End Sub

    Protected Sub SetupPropertyEditor()
        Dim collection As New List(Of GlobalWidgetPropertySettings)

        If (propertydic.Count > 0) Then
            GlobalSettingsRow.Visible = True
            For Each item As KeyValuePair(Of PropertyInfo, GlobalWidgetData) In propertydic
                Dim conv As New GlobalWidgetPropertySettings
                conv.PropertyName = item.Key.Name
                conv.Type = item.Key.PropertyType
                conv.value = item.Value.getDefault
                conv.Settings = item.Value.PropertySettings
                collection.Add(conv)
            Next
            LoadSavedProps(collection)
            globalProps.DataSource = collection
            globalProps.DataBind()
        Else
            GlobalSettingsRow.Visible = False
        End If
    End Sub

    Protected Sub LoadSavedProps(ByRef autogenprops As List(Of GlobalWidgetPropertySettings))
        If (info.Settings <> String.Empty) Then
            Dim savedprops As List(Of GlobalWidgetPropertySettings)
            Dim serializer As New System.Xml.Serialization.XmlSerializer(GetType(List(Of GlobalWidgetPropertySettings)))
            Dim s As New IO.StringReader(info.Settings)
            Try
                savedprops = serializer.Deserialize(s)
            Catch
                savedprops = New List(Of GlobalWidgetPropertySettings)
            End Try
            For Each toupdate As GlobalWidgetPropertySettings In autogenprops
                For Each iter As GlobalWidgetPropertySettings In savedprops
                    If toupdate.PropertyName = iter.PropertyName AndAlso toupdate.FullTypeName = iter.FullTypeName AndAlso iter.value IsNot Nothing Then
                        toupdate.value = iter.value
                        Exit For
                    End If
                Next
            Next
        End If
    End Sub

    Protected Sub Properties_OnItemDataBound(ByVal Sender As Object, ByVal e As RepeaterItemEventArgs)
        Dim data As GlobalWidgetPropertySettings = e.Item.DataItem
        Dim lblPropertyName As Label = CType(e.Item.FindControl("lblPropertyName"), Label)
        Dim lblType As Label = CType(e.Item.FindControl("lblType"), Label)
        Dim phValue As PlaceHolder = CType(e.Item.FindControl("phValue"), PlaceHolder)
        lblPropertyName.Text = data.PropertyName
        lblType.Text = data.Type.Name
        phValue.Controls.Clear()
        GenerateInputField(phValue, data)
    End Sub

    Protected Sub GenerateInputField(ByRef owner As Control, ByRef data As GlobalWidgetPropertySettings)
        Dim type As Type = data.Type
        Dim value As Object = data.value
        'DateTime, int, long, double, bool, string, Enumeration EkEnumeration.OrderByDirection()
        If (Not type.IsEnum) Then
            Select Case type.Name
                Case GetType(String).Name
                    Dim text As New TextBox()
                    text.ID = "value"
                    text.Text = CType(value, String)
                    text.CssClass = "freetext"
                    If ((data.Settings And GlobalPropertyAttributes.PasswordField) = GlobalPropertyAttributes.PasswordField) Then
                        text.TextMode = TextBoxMode.Password
                        text.Attributes.Add("value", CType(value, String))
                    End If
                    owner.Controls.Add(text)
                Case GetType(DateTime).Name
                    Dim hdnvalue As New HiddenField()
                    hdnvalue.ID = "value"
                    hdnvalue.Value = CType(value, DateTime).ToString()
                    owner.Controls.Add(hdnvalue)
                    Dim text As New TextBox()
                    text.ID = "textinput"
                    text.Text = CType(value, DateTime).ToString()
                    text.CssClass = "datetime"
                    text.Attributes.Add("hiddenfield", hdnvalue.ClientID)
                    owner.Controls.Add(text)
                    Dim span As New System.Web.UI.HtmlControls.HtmlGenericControl
                    span.TagName = "span"
                    span.Attributes.Add("class", "displayParsedDate")
                    owner.Parent.FindControl("phExtraOutput").Controls.Add(span)
                Case GetType(Integer).Name
                    Dim text As New TextBox()
                    text.ID = "value"
                    text.Text = CType(value, Integer).ToString()
                    text.CssClass = "integer"
                    owner.Controls.Add(text)
                Case GetType(Long).Name
                    Dim text As New TextBox()
                    text.ID = "value"
                    text.Text = CType(value, Long).ToString()
                    text.CssClass = "integer"
                    owner.Controls.Add(text)
                Case GetType(Double).Name
                    Dim text As New TextBox()
                    text.ID = "value"
                    text.Text = CType(value, Double).ToString()
                    text.CssClass = "double"
                    owner.Controls.Add(text)
                Case GetType(Boolean).Name
                    Dim list As New DropDownList()
                    list.ID = "value"
                    list.Items.Add(New ListItem("True", "true"))
                    list.Items.Add(New ListItem("False", "false"))
                    If (CType(value, Boolean) = True) Then
                        list.SelectedIndex = 0
                    Else
                        list.SelectedIndex = 1
                    End If
                    owner.Controls.Add(list)
                Case Else
                    Dim ex As String = "Unsupported global property specified in " & info.ControlURL & vbCrLf
                    ex &= "The type " & type.Name & " is not supported by this version of the Portal Framework. Only the following types are supported: " & vbCrLf
                    ex &= "DateTime, int, long, double, bool, string, Enumeration"
                    Throw New Exception(ex)
            End Select
        Else 'is an enum
            Dim list As New DropDownList()
            list.ID = "value"
            Dim items As String() = System.Enum.GetNames(type)
            For Each item As String In items
                Dim li As New ListItem()
                li.Text = item
                li.Value = item
                If (value IsNot Nothing AndAlso item = System.Enum.GetName(type, value)) Then
                    li.Selected = True
                End If
                list.Items.Add(li)
            Next
            owner.Controls.Add(list)
        End If
    End Sub

    Protected Sub decodeValue(ByVal type As Type, ByRef inputitem As Control, ByRef value As Object)
        If (Not type.IsEnum) Then
            Select Case type.Name
                Case GetType(String).Name
                    Dim text As TextBox = CType(inputitem, TextBox)
                    value = CType(text.Text, String)
                Case GetType(DateTime).Name
                    Dim text As HiddenField = CType(inputitem, HiddenField)
                    value = DateTime.Parse(text.Value)
                Case GetType(Integer).Name
                    Dim text As TextBox = CType(inputitem, TextBox)
                    value = Integer.Parse(text.Text)
                Case GetType(Long).Name
                    Dim text As TextBox = CType(inputitem, TextBox)
                    Long.TryParse(text.Text, value)
                    'value = Long.Parse(text.Text)
                Case GetType(Double).Name
                    Dim text As TextBox = CType(inputitem, TextBox)
                    value = Double.Parse(text.Text)
                Case GetType(Boolean).Name
                    Dim list As DropDownList = CType(inputitem, DropDownList)
                    value = Boolean.Parse(list.SelectedValue)
                Case Else
                    Dim ex As String = "Unsupported global property specified in " & info.ControlURL & vbCrLf
                    ex &= "The type " & type.Name & " is not supported by this version of the Portal Framework. Only the following types are supported: " & vbCrLf
                    ex &= "DateTime, int, long, double, bool, string, Enumeration"
                    Throw New Exception(ex)
            End Select
        Else 'is an enum
            Dim list As DropDownList = CType(inputitem, DropDownList)
            value = System.Enum.Parse(type, list.SelectedValue)
        End If
    End Sub
End Class
