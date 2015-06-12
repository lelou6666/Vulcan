Imports Microsoft.VisualBasic

Public Class FormActionRewriterControlAdapter
    Inherits System.Web.UI.Adapters.ControlAdapter
    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        MyBase.Render(New RewriteFormActionHtmlTextWriter(writer))
    End Sub
End Class

Public Class RewriteFormActionHtmlTextWriter
    Inherits HtmlTextWriter
    Sub New(ByVal writer As HtmlTextWriter)
        MyBase.New(writer)
        Me.InnerWriter = writer.InnerWriter
    End Sub
    Sub New(ByVal writer As System.IO.TextWriter)
        MyBase.New(writer)
        MyBase.InnerWriter = writer
    End Sub
    Public Overrides Sub WriteAttribute(ByVal name As String, ByVal value As String, ByVal fEncode As Boolean)
        If (name = "action") Then
            Dim Context As HttpContext
            Context = HttpContext.Current
            If Context.Items("ActionAlreadyWritten") Is Nothing Then
                value = Context.Request.RawUrl
                Context.Items("ActionAlreadyWritten") = True
            End If
        End If
        MyBase.WriteAttribute(name, value, fEncode)
    End Sub
End Class