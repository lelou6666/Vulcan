Imports Ektron.Cms.Common
Imports System.Collections.Generic

Partial Class ektransform
    Inherits Ektron.Cms.Workarea.Framework.WorkAreaBasePage
    Dim refContentApi As New Ektron.Cms.ContentAPI()
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AssertInternalReferrer()
        Dim output As String = "<root></root>"

        Try
            Dim sXml As String
            Dim sXslt As String
            sXml = Request("xml")
            sXslt = Request("xslt")
            Dim xmlConfigId As Long = 0
            Dim isPackageXsl As Boolean = False

            If Not String.IsNullOrEmpty(Request("xid")) Then
                xmlConfigId = EkFunctions.ReadDbLong(Request("xid"))
            End If

            Dim args As Ektron.Cms.Xslt.ArgumentList = Nothing
            Dim iArg As Integer
            Dim sArg As String
            Dim pEqu As Integer
            Dim sName As String
            Dim sValue As String
            iArg = 0
            sArg = Request("arg0")  ' format: "arg0=" + escape(name + "=" + value)
            While (Not IsNothing(sArg) AndAlso sArg.Length > 0)
                pEqu = sArg.IndexOf("=") ' separator b/n name and value
                If (pEqu >= 1) Then
                    sName = sArg.Substring(0, pEqu)
                    sValue = sArg.Substring(pEqu + 1)
                    If (IsNothing(args)) Then
                        args = New Ektron.Cms.Xslt.ArgumentList()
                    End If
                    args.AddParam(sName, "", sValue)
                End If
                iArg += 1
                sArg = Request("arg" & iArg)
            End While

            If (Not String.IsNullOrEmpty(sXml) AndAlso Ektron.Cms.Common.EkFunctions.IsURL(sXml)) Then
                Try
                    sXml = New Uri(Request.Url, sXml).AbsoluteUri
                Catch ex As Exception
                    ' Ignore
                End Try
            End If

            ' Can't MapPath b/c the transform will lose its context and relative paths will fail.
            If Not String.IsNullOrEmpty(sXslt) AndAlso Not sXslt.StartsWith("<") Then
                Dim whitelist As Boolean = Ektron.Cms.EkXml.IsWhiteListFile(sXslt, refContentApi.ApplicationPath, System.Web.HttpContext.Current.Request.PhysicalApplicationPath, System.Web.HttpContext.Current.Request.ServerVariables("HTTP_HOST"))
                ' either the file will be whitelisted, or the user is logged in and a member of the smart form admins group.
                If whitelist OrElse Not Ektron.Cms.UserContext.GetCurrentUser().IsMemberShip Then
                    Try
                        If System.Web.HttpContext.Current.Request.IsSecureConnection.Equals(True) Then
                            sXslt = System.Web.HttpContext.Current.Server.MapPath(sXslt)
                        Else
                            sXslt = New Uri(Request.Url, sXslt).AbsoluteUri
                        End If

                    Catch generatedExceptionName As Exception
                    End Try
                Else
                    sXslt = String.Empty
                    Return
                End If
            ElseIf xmlConfigId > 0 Then
                ' smart form editor
                If Not Ektron.Cms.UserContext.GetCurrentUser().IsMemberShip Then
                    isPackageXsl = True
                Else
                    sXslt = ""
                End If
            End If

           
            'Dim sApi As String
            'sApi = Request("api")
            'If (Not IsNothing(sApi) AndAlso "cms" = sApi.ToLower()) Then
            '	' Use the API's transform so the built-in arguments are given to the XSLT.
            '	Dim api As New Ektron.Cms.CommonApi
            '	output = api.XSLTransform(sXml, sXslt, XsltArgs:=args, ReturnExceptionMessage:=False)
            '	api = Nothing
            'Else
            
            output = refContentApi.XSLTransform(sXml, sXslt, False, False, args, True, isPackageXsl)

            'End If
        Catch ex As Exception
            output = String.Format("<html><head><title>ekAjaxTransform Error</title></head><body class=""ekAjaxTransformError"">{0}</body></html>", Server.HtmlEncode(ex.Message))
        End Try

        Response.ContentType = "application/xml"
        Response.ContentEncoding = System.Text.Encoding.UTF8 ' Safari does not encode properly even though this is set
        litContent.Text = output
    End Sub

    ''' <SUMMARY>Computes the Levenshtein Edit Distance between two enumerables.</SUMMARY>
    ''' <TYPEPARAM name="T">The type of the items in the enumerables.</TYPEPARAM>
    ''' <PARAM name="x">The first enumerable.</PARAM>
    ''' <PARAM name="y">The second enumerable.</PARAM>
    ''' <RETURNS>The edit distance.</RETURNS>
    Public Function EditDistance(Of T As IEquatable(Of T))(ByVal x As IEnumerable(Of T), ByVal y As IEnumerable(Of T)) As Integer
        ' Validate parameters
        If x Is Nothing Then
            Throw New ArgumentNullException("x")
        End If
        If y Is Nothing Then
            Throw New ArgumentNullException("y")
        End If

        ' Convert the parameters into IList instances
        ' in order to obtain indexing capabilities
        Dim first As IList(Of T) = If(TryCast(x, IList(Of T)), New List(Of T)(x))
        Dim second As IList(Of T) = If(TryCast(y, IList(Of T)), New List(Of T)(y))

        ' Get the length of both.  If either is 0, return
        ' the length of the other, since that number of insertions
        ' would be required.
        Dim n As Integer = first.Count, m As Integer = second.Count
        If n = 0 Then
            Return m
        End If
        If m = 0 Then
            Return n
        End If

        ' Rather than maintain an entire matrix (which would require O(n*m) space),
        ' just store the current row and the next row, each of which has a length m+1,
        ' so just O(m) space. Initialize the current row.
        Dim curRow As Integer = 0, nextRow As Integer = 1
        Dim rows As Integer()() = New Integer()() {New Integer(m) {}, New Integer(m) {}}
        For j As Integer = 0 To m
            rows(curRow)(j) = j
        Next

        ' For each virtual row (since we only have physical storage for two)
        For i As Integer = 1 To n
            ' Fill in the values in the row
            rows(nextRow)(0) = i
            For j As Integer = 1 To m
                Dim dist1 As Integer = rows(curRow)(j) + 1
                Dim dist2 As Integer = rows(nextRow)(j - 1) + 1
                Dim dist3 As Integer = rows(curRow)(j - 1) + (If(first(i - 1).Equals(second(j - 1)), 0, 1))

                rows(nextRow)(j) = Math.Min(dist1, Math.Min(dist2, dist3))
            Next

            ' Swap the current and next rows
            If curRow = 0 Then
                curRow = 1
                nextRow = 0
            Else
                curRow = 0
                nextRow = 1
            End If
        Next

        ' Return the computed edit distance
        Return rows(curRow)(m)
    End Function
    Private Function IsUserAuthenticated() As Boolean
        Dim isAuthenticated As Boolean = False
        If refContentApi.UserId > 0 AndAlso refContentApi.UniqueId > 0 Then
            Dim user As Ektron.Cms.UserData = Ektron.Cms.ObjectFactory.GetUser(refContentApi.RequestInformationRef).GetItem(refContentApi.UserId)
            If user IsNot Nothing Then
                isAuthenticated = (user.LoginIdentification = refContentApi.UniqueId.ToString())
            End If
        End If
        Return isAuthenticated
    End Function
End Class
