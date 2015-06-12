Imports System.Data
Imports System
Imports System.Web.UI
Imports System.Web
Imports System.Web.UI.WebControls

Public Class CustomItemTemplate
    Implements ITemplate
    Private columnName As String
    Private WithEvents editControl As TextBox
    Public Sub New(ByVal colName As String)
        columnName = colName
    End Sub
    Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        editControl = New TextBox
        AddHandler container.DataBinding, AddressOf BindDataCtrl
        container.Controls.Add(editControl)
    End Sub

    Private Sub BindDataCtrl(ByVal sender As Object, ByVal e As EventArgs) Handles editControl.DataBinding
        Dim container As DataGridItem = CType(editControl.NamingContainer, DataGridItem)
        Dim str As String = (CType(container.DataItem, DataRowView))(columnName).ToString()
        editControl.Text = str
    End Sub
End Class
