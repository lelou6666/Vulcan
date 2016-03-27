Imports Ektron.Cms
Imports Ektron.Cms.Common
Partial Class ContentDesigner_configurations_InterfaceBlog
    Inherits ContentDesignerConfigurationBase

    Protected strButtons As String = ""
    Protected options As Hashtable = New Hashtable()
    Protected IsForum As Boolean = False

    Overloads Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load
        Dim strValue As String
        strValue = Request.QueryString("mode")
        If (strValue = "forum") Then
            strButtons = Request.QueryString("toolButtons")
            IsForum = True
            Dim arTools() As String = strButtons.ToLower().Split(",")
            For Each item As String In arTools
                If ((options.ContainsKey(item)) = False) Then
                    options.Add(item, item)
                End If
            Next
        End If
        MyBase.Page_Load(sender, e)
    End Sub
End Class
