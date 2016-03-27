Imports Ektron.Cms
Imports Ektron.Cms.Common
Imports Ektron.Cms.API

Partial Class controls_Reorder_Reorder
    Inherits System.Web.UI.UserControl


#Region "Private Members"


    Private _messageHelper As EkMessageHelper = Nothing
    Private _appPath As String = ""
    Private _appImgPath As String = ""
    Private _itemCollection As New ListItemCollection()

#End Region


#Region "Page Functions"


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        RegisterJS()

    End Sub


#End Region


#Region "Properties"


    Public Property ItemList() As ListItemCollection
        Get
            Return _itemCollection
        End Get
        Set(ByVal value As ListItemCollection)
            _itemCollection = value
        End Set
    End Property

    Public Property MessageHelper() As EkMessageHelper
        Get
            Return _messageHelper
        End Get
        Set(ByVal value As EkMessageHelper)
            _messageHelper = value
        End Set
    End Property

    Public Property AppPath() As String
        Get
            Return _appPath
        End Get
        Set(ByVal value As String)
            _appPath = value
        End Set
    End Property

    Public Property AppImgPath() As String
        Get
            Return _appImgPath
        End Get
        Set(ByVal value As String)
            _appImgPath = value
        End Set
    End Property

    Public ReadOnly Property ReOrderList() As String
        Get
            Return ConvertToString()
        End Get
    End Property


#End Region


#Region "Public Functions"


    Public Function GetMessage(ByVal resourceString As String) As String

        Return MessageHelper.GetMessage(resourceString)

    End Function

    Public Sub AddItem(ByVal title As String, ByVal id As Long, ByVal language As Long)

        _itemCollection.Add(New ListItem(title, id.ToString() & "|" & language.ToString()))

    End Sub

    Public Sub Initialize(ByVal requestInformation As EkRequestInformation)

        _messageHelper = New EkMessageHelper(requestInformation)
        _appPath = requestInformation.ApplicationPath
        _appImgPath = requestInformation.AppImgPath

    End Sub


#End Region


#Region "Private Functions"


    Private Function ConvertToString() As String

        Dim commaSeperatedList As String = ""

        For i As Integer = 0 To (_itemCollection.Count - 1)

            If commaSeperatedList <> "" Then
                commaSeperatedList &= "," & _itemCollection(i).Value
            Else
                commaSeperatedList = _itemCollection(i).Value
            End If

        Next

        Return commaSeperatedList

    End Function

    Private Sub RegisterJS()

        Dim _ContentAPI As Ektron.Cms.ContentAPI
        _ContentAPI = New ContentAPI()
        Ektron.Cms.API.JS.RegisterJS(Me, _ContentAPI.AppPath & "controls/Reorder/js/Reorder.js", "EktronControlsReorderJs")

    End Sub

#End Region


End Class
