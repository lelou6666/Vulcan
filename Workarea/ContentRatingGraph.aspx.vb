Imports System
Imports System.Data
Imports System.Configuration
Imports System.Collections
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Data.SqlClient
Imports Ektron.Cms
Imports System.IO

Partial Class ContentRatingGraph
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim graph As WorkareaGraphBase

        Dim tmp As String = Request.QueryString("type")
        If (tmp = "pie") Then
            graph = New CircleGraph()
        ElseIf (tmp = "time") Then
            graph = New TimeGraph()
        Else
            graph = New BarGraph()
        End If

        graph.Init(Me)
    End Sub

End Class

Friend Class TimeGraph
    Inherits WorkareaGraphBase

    Private _height As Integer = 200
    Private _width As Integer = 600
    Private _bottomArea As Integer = 15
    Private _percentageSpace As Single = 0.3F
    Private _numBars As Integer = 24
    Private _fontSize As Integer = 8
    Private _clusterSize As Integer = 2

    Private _barBrush As New SolidBrush(Color.LightBlue)
    Private _barColor As Color = Color.LightBlue

    Private _bgBrush As New SolidBrush(Color.White)
    Private _bgColor As Color = Color.White

    Private _fontBrush As New SolidBrush(Color.Black)
    Private _fontColor As Color = Color.Black

    Private weights() As Integer
    Private heights() As Single

    Private CurrentView As String

    Private Divisions As Integer = 8

    Private Analytics As New Ektron.Cms.AnalyticsAPI()

#Region "Properties"
    Private ReadOnly Property FontSize() As Integer
        Get
            Return _fontSize
        End Get
    End Property

    Private Property Height() As Integer
        Get
            Return _height
        End Get
        Set(ByVal Value As Integer)
            If _height > 0 Then
                _height = Value
            End If
        End Set
    End Property

    Private Property Width() As Integer
        Get
            Return _width
        End Get
        Set(ByVal Value As Integer)
            If _width > 0 Then
                _width = Value
            End If
        End Set
    End Property

    Private Property PercentageSpace() As Single
        Get
            Return _percentageSpace
        End Get
        Set(ByVal Value As Single)
            If Value >= 0 Or Value < 1 Then
                _percentageSpace = Value
            End If
        End Set
    End Property

    Private Property NumBars() As Integer
        Get
            Return _numBars
        End Get
        Set(ByVal Value As Integer)
            If Value > 0 Then
                _numBars = Value
            End If
        End Set
    End Property

    Private Property ClusterSize() As Integer
        Get
            Return _clusterSize
        End Get
        Set(ByVal value As Integer)
            If value > 0 Then
                _clusterSize = value
            End If
        End Set
    End Property

    Private ReadOnly Property BarWidth() As Single
        Get
            Return Width * (1 - PercentageSpace) / (NumBars * ClusterSize)
        End Get
    End Property

    Private ReadOnly Property SpaceWidth() As Single
        Get
            Return Width * PercentageSpace / (NumBars * ClusterSize)
        End Get
    End Property

    Private Property TextHeight() As Integer
        Get
            Return _bottomArea
        End Get
        Set(ByVal Value As Integer)
            _bottomArea = Value
        End Set
    End Property

    Private Property BarColor() As Color
        Get
            Return _barColor
        End Get
        Set(ByVal Value As Color)
            _barBrush = New SolidBrush(Value)
        End Set
    End Property

    Private Property BGColor() As Color
        Get
            Return _bgColor
        End Get
        Set(ByVal Value As Color)
            _bgBrush = New SolidBrush(Value)
        End Set
    End Property

    Private Property FontColor() As Color
        Get
            Return _fontColor
        End Get
        Set(ByVal Value As Color)
            _fontBrush = New SolidBrush(Value)
        End Set
    End Property
#End Region

    Protected Function CheckAccess() As Boolean
        Dim contentApi As New ContentAPI

        If contentApi.IsLoggedIn() Then
            If ((Not Page.Request.QueryString("res_type") Is Nothing) _
                AndAlso Page.Request.QueryString("res_type").ToLower() = "content") Then

                Dim contentId As Long = CLng(Page.Request.QueryString("res"))
                Dim permissions As ContentAPI.userPermissions = contentApi.GetUserPermissionsForContent(contentId)

                If (Ektron.Cms.Common.EkFunctions.GetBit(contentApi.userPermissions.View, permissions)) Then
                    Return True
                End If
                If (Ektron.Cms.Common.EkFunctions.GetBit(contentApi.userPermissions.Edit, permissions)) Then
                    Return True
                End If
            Else
                Return True 'this isn't content - return true.
            End If
        End If

        Return False
    End Function

    Public Overrides Sub Initialize()
        Try
            CurrentView = Page.Request.QueryString("view")
        Catch ex As Exception
            CurrentView = "day"
        End Try
        Height = 200
        Width = 525
        PercentageSpace = 0.2F
        NumBars = 24
        ClusterSize = 2
        weights = New Integer(NumBars * ClusterSize) {}
        heights = New Single(NumBars * ClusterSize) {}
        Dim max As Integer = 1
        BarColor = Color.Blue

        Select Case (CurrentView)
            Case "day"
                NumBars = 24
                Divisions = 8
            Case "week"
                NumBars = 7
                Divisions = 7
            Case "month"
                NumBars = 30
                Divisions = 6
            Case "year"
                NumBars = 12
                Divisions = 12
        End Select

        If Not Page.Request.QueryString("barColor") Is Nothing Then
            Try
                BarColor = Color.FromName(Page.Request.QueryString("barColor"))
            Catch

            End Try
        End If

        If Not Page.Request.QueryString("fontColor") Is Nothing Then
            Try
                FontColor = Color.FromName(Page.Request.QueryString("fontColor"))
            Catch

            End Try
        End If
    End Sub

    Private Function AddRestriction(ByVal sqlStr As String) As String
        Dim res_type As String = ""
        Dim res As String = ""

        Try
            res_type = Page.Request.QueryString("res_type")
        Catch ex As Exception
            res_type = ""
        End Try

        Try
            res = Ektron.Cms.Common.EkFunctions.GetDbString(Page.Request.QueryString("res"))
        Catch ex As Exception
            res = ""
        End Try

        If (res_type = "content") Then
            sqlStr &= " AND content_id = " & CLng(res)
        ElseIf (res_type = "page") Then
            sqlStr &= " AND url = '" & res & "'"
        ElseIf (res_type = "referring") Then
            sqlStr &= " AND referring_url = '" & res & "'"
        End If

        Return sqlStr
    End Function

    Public Overrides Sub DrawGraphic()
        Dim tmpooo As Integer = 1000
        Dim side As Integer = 10 * Convert.ToInt32(tmpooo.ToString().Length - 1) + 5
        Dim bmp As System.Drawing.Bitmap = New System.Drawing.Bitmap(Width + side, Height + TextHeight)
        Dim ms As MemoryStream = New MemoryStream
        Dim i As Integer
        Dim g As Graphics = Graphics.FromImage(bmp)
        Dim myfont As New System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, Convert.ToSingle(12), FontStyle.Regular, GraphicsUnit.Pixel)
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High

        g.FillRectangle(Brushes.White, 0, 0, Width + side, Height + TextHeight)


        If Not CheckAccess() Then
            Dim contentApi As New ContentAPI
            Dim msgApi As Ektron.Cms.Common.EkMessageHelper = contentApi.EkMsgRef
            Throw New Exception(msgApi.GetMessage("com: user does not have permission"))
        End If

        Dim StartDate As DateTime
        Dim EndDate As DateTime
        Try
            EndDate = DateTime.Parse(Page.Request.QueryString("EndDate"))
        Catch ex As Exception
            EndDate = DateTime.Today
        End Try

        Try
            StartDate = DateTime.Parse(Page.Request.QueryString("StartDate"))
        Catch ex As Exception
            StartDate = DateTime.MinValue
        End Try

        Dim sqlCommands(NumBars) As String
        Dim dbData As DataSet

        Dim data(NumBars * ClusterSize) As Integer

        If (StartDate = DateTime.MinValue) Then

            If (CurrentView = "day") Then
                For i = 0 To NumBars - 1
                    If (i = NumBars - 1) Then
                        sqlCommands(i) = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " & _
                        "hit_date >= " & AnalyticsAPI.FormatDate(EndDate.AddHours(i), AnalyticsAPI.ProviderInvariantName) & " AND hit_date <= " & AnalyticsAPI.FormatDate(EndDate.AddHours(i + 1), AnalyticsAPI.ProviderInvariantName) & "")
                    Else
                        sqlCommands(i) = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " & _
                        "hit_date >= " & AnalyticsAPI.FormatDate(EndDate.AddHours(i), AnalyticsAPI.ProviderInvariantName) & " AND hit_date < " & AnalyticsAPI.FormatDate(EndDate.AddHours(i + 1), AnalyticsAPI.ProviderInvariantName) & "")
                    End If

                Next
                dbData = Analytics.QueryAnalytics(sqlCommands)
                i = 0
                For i = 0 To dbData.Tables.Count - 1
                    data(i * 2) = dbData.Tables(i).Rows(0)(0)
                    data(i * 2 + 1) = dbData.Tables(i).Rows(0)(1)
                Next
            ElseIf (CurrentView = "week") Then
                For i = 0 To NumBars - 1
                    If (i = NumBars - 1) Then
                        sqlCommands(i) = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " & _
                        "hit_date <= " & AnalyticsAPI.FormatDate(EndDate.AddDays(-1 * (NumBars - i - 2)), AnalyticsAPI.ProviderInvariantName) & " AND hit_date >= " & AnalyticsAPI.FormatDate(EndDate.AddDays(-1 * (NumBars - i - 1)), AnalyticsAPI.ProviderInvariantName) & "")
                    Else
                        sqlCommands(i) = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " & _
                        "hit_date < " & AnalyticsAPI.FormatDate(EndDate.AddDays(-1 * (NumBars - i - 2)), AnalyticsAPI.ProviderInvariantName) & " AND hit_date >= " & AnalyticsAPI.FormatDate(EndDate.AddDays(-1 * (NumBars - i - 1)), AnalyticsAPI.ProviderInvariantName) & "")
                    End If

                Next
                dbData = Analytics.QueryAnalytics(sqlCommands)
                i = 0
                For i = 0 To dbData.Tables.Count - 1
                    data(i * 2) = dbData.Tables(i).Rows(0)(0)
                    data(i * 2 + 1) = dbData.Tables(i).Rows(0)(1)
                Next
            ElseIf (CurrentView = "month") Then
                For i = 0 To NumBars - 1
                    If (i = NumBars - 1) Then
                        sqlCommands(i) = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " & _
                        "hit_date <= " & AnalyticsAPI.FormatDate(EndDate.AddDays(-1 * (NumBars - i - 2)), AnalyticsAPI.ProviderInvariantName) & " AND hit_date >= " & AnalyticsAPI.FormatDate(EndDate.AddDays(-1 * (NumBars - i - 1)), AnalyticsAPI.ProviderInvariantName) & "")
                    Else
                        sqlCommands(i) = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " & _
                        "hit_date < " & AnalyticsAPI.FormatDate(EndDate.AddDays(-1 * (NumBars - i - 2)), AnalyticsAPI.ProviderInvariantName) & " AND hit_date >= " & AnalyticsAPI.FormatDate(EndDate.AddDays(-1 * (NumBars - i - 1)), AnalyticsAPI.ProviderInvariantName) & "")
                    End If

                Next
                dbData = Analytics.QueryAnalytics(sqlCommands)
                i = 0
                For i = 0 To dbData.Tables.Count - 1
                    data(i * 2) = dbData.Tables(i).Rows(0)(0)
                    data(i * 2 + 1) = dbData.Tables(i).Rows(0)(1)
                Next
            ElseIf (CurrentView = "year") Then
                For i = 0 To NumBars - 1
                    If (i = NumBars - 1) Then
                        ' MM-01-yyyy
                        sqlCommands(i) = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " & _
                        "hit_date <= " & AnalyticsAPI.FormatDate(EndDate.AddMonths(-1 * (NumBars - i - 2)), AnalyticsAPI.ProviderInvariantName) & " AND hit_date >= " & AnalyticsAPI.FormatDate(EndDate.AddMonths(-1 * (NumBars - i - 1)), AnalyticsAPI.ProviderInvariantName) & "")
                    Else
                        sqlCommands(i) = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " & _
                        "hit_date < " & AnalyticsAPI.FormatDate(EndDate.AddMonths(-1 * (NumBars - i - 2)), AnalyticsAPI.ProviderInvariantName) & " AND hit_date >= " & AnalyticsAPI.FormatDate(EndDate.AddMonths(-1 * (NumBars - i - 1)), AnalyticsAPI.ProviderInvariantName) & "")
                    End If

                Next
                dbData = Analytics.QueryAnalytics(sqlCommands)
                i = 0
                For i = 0 To dbData.Tables.Count - 1
                    data(i * 2) = dbData.Tables(i).Rows(0)(0)
                    data(i * 2 + 1) = dbData.Tables(i).Rows(0)(1)
                Next
            End If
        Else

        End If

        Dim tmpS As Single
        If (CurrentView = "day") Then
            For i = 0 To 7
                tmpS = i * (Width / Divisions) + side
                g.DrawString(EndDate.AddHours(3 * i).ToString("hh:mm tt"), myfont, Brushes.Black, New System.Drawing.Point(tmpS, Height))
            Next
        ElseIf (CurrentView = "week") Then
            For i = 0 To 6
                tmpS = (Divisions - i - 1) * (Width / Divisions) + side
                g.DrawString(EndDate.AddDays(-1 * i).ToString("ddd MM-dd"), myfont, Brushes.Black, New System.Drawing.Point(tmpS, Height))
            Next
        ElseIf (CurrentView = "month") Then
            For i = 0 To 5
                tmpS = (Divisions - i - 1) * (Width / Divisions) + side
                g.DrawString(EndDate.AddDays(-5 * i).ToString("MM-dd-yyyy"), myfont, Brushes.Black, New System.Drawing.Point(tmpS, Height))
            Next
        ElseIf (CurrentView = "year") Then
            For i = 0 To 11
                tmpS = (Divisions - i - 1) * (Width / Divisions) + side
                g.DrawString(EndDate.AddMonths(-1 * i).ToString("MMM-yy"), myfont, Brushes.Black, New System.Drawing.Point(tmpS, Height))
            Next
        End If


        For i = 0 To Divisions - 1
            Dim start As Integer = i * (Width / Divisions) + side
            g.DrawLine(Pens.Black, start, Height, start, 0)
        Next


        Dim rand As New Random()

        Dim max As Integer = 1
        For i = 0 To NumBars * ClusterSize - 1
            If (data(i) > max) Then
                max = data(i)
            End If
        Next

        Dim oom As Integer = Me.GetOrderOfMagnitude(max)
        'g.DrawString(max, font, Brushes.Black, 0, 12)
        Dim trying As Integer = 0
        trying = (oom / max) * (Height)


        Dim j As Integer = 1
        While ((j * oom) < max)
            Dim tmp1 As Single = Height - trying * j
            g.DrawLine(Pens.Black, side, tmp1, Width + side, tmp1)
            g.DrawString((oom * j).ToString(), myfont, Brushes.Black, 0, tmp1 - 7)
            j = j + 1
        End While

        g.DrawLine(Pens.Black, side, Height, Width + side, Height)

        For i = 0 To NumBars * ClusterSize - 1
            data(i) = data(i) * Height / max
            If (i Mod 2 = 0) Then
                g.FillRectangle(Brushes.Red, (BarWidth + SpaceWidth) * i + side, Height - data(i), BarWidth, data(i))
            Else
                g.FillRectangle(Brushes.Blue, (BarWidth + SpaceWidth) * i + side, Height - data(i), BarWidth, data(i))
            End If
        Next

        bmp.Save(ms, ImageFormat.Png)
        ms.WriteTo(Page.Response.OutputStream)

        bmp.Dispose()
        ms.Dispose()
    End Sub

    Private Function GetOrderOfMagnitude(ByVal val As Integer) As Integer
        Dim i As Integer = 1

        While (True)
            If (Math.Floor(val / (i * 10)) = 0) Then
                Return i
            Else
                i = i * 10
            End If
        End While
    End Function

End Class

Friend Class CircleGraph
    Inherits WorkareaGraphBase

    Private _height As Integer = 100
    Private _width As Integer = 100

    Protected Property Height() As Integer
        Get
            Return _height
        End Get
        Set(ByVal value As Integer)
            _height = value
        End Set
    End Property

    Protected Property Width() As Integer
        Get
            Return _width
        End Get
        Set(ByVal value As Integer)
            _width = value
        End Set
    End Property


    Public Overrides Sub Initialize()
        Try
            Dim tmp As Integer = Convert.ToInt32(Page.Request.QueryString("size"))
            Height = tmp
            Width = tmp
        Catch ex As Exception
            Height = 100
            Width = 100
        End Try

    End Sub

    Public Overrides Sub DrawGraphic()
        Dim bmp As New System.Drawing.Bitmap(Width, Height)
        Dim ms As MemoryStream = New MemoryStream
        Dim objGraphics As Graphics = Graphics.FromImage(bmp)

        objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
        objGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
        objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High

        Dim whiteBrush As New SolidBrush(Color.White)
        Dim blackBrush As New SolidBrush(Color.Black)

        Dim r1 As Integer = Convert.ToInt32(Page.Request.QueryString("r1"))
        Dim r2 As Integer = Convert.ToInt32(Page.Request.QueryString("r2"))

        Dim total As Integer = r1 + r2

        Dim tHeight As Integer = Math.Floor(Height * 0.9)
        Dim tWidth As Integer = Math.Floor(Width * 0.9)
        Dim delta As Integer = Math.Floor(Width * 0.05)

        Dim d1 As Integer
        Try
            d1 = r1 * 360 / total
        Catch ex As Exception
            objGraphics.FillRectangle(whiteBrush, 0, 0, Width, Height)
            objGraphics.FillPie(Brushes.Black, delta, delta, tWidth, tHeight, 0, 360)
            bmp.Save(ms, ImageFormat.Png)
            ms.WriteTo(Page.Response.OutputStream)
            Return
        End Try

        Dim d2 As Integer = 360 - d1



        objGraphics.FillRectangle(whiteBrush, 0, 0, Width, Height)
        objGraphics.FillPie(Brushes.Red, delta, delta, tWidth, tHeight, 0, d1)
        objGraphics.FillPie(Brushes.Blue, delta, delta, tWidth, tHeight, d1, d2)

        bmp.Save(ms, ImageFormat.Png)
        ms.WriteTo(Page.Response.OutputStream)

        bmp.Dispose()
        ms.Dispose()
    End Sub
End Class

Friend Class BarGraph
    Inherits WorkareaGraphBase

    Private _height As Integer = 100
    Private _width As Integer = 100
    Private _bottomArea As Integer = 15
    Private _percentageSpace As Single = 0.3F
    Private _numBars As Integer = 9
    Private _fontSize As Integer = 8

    Private _barBrush As New SolidBrush(Color.LightBlue)
    Private _barColor As Color = Color.LightBlue

    Private _bgBrush As New SolidBrush(Color.White)
    Private _bgColor As Color = Color.White

    Private _fontBrush As New SolidBrush(Color.Black)
    Private _fontColor As Color = Color.Black


    Private weights() As Integer
    Private heights() As Single

    Private m_b0 As Boolean = True
    Private m_b10 As Boolean = True

    Private m_bStars As Boolean = False

    Public Overrides Sub Initialize()
        Height = 125
        Width = 150
        PercentageSpace = 0.3F
        NumBars = 11
        weights = New Integer(NumBars) {}
        heights = New Single(NumBars) {}
        Dim max As Integer = 1
        BarColor = Color.LightBlue

        Dim i As Integer
        For i = 0 To 10
            Try
                Dim val As String = Page.Request.QueryString.Get("R" & (i))
                If i = 0 And val Is Nothing Then
                    m_b0 = False
                ElseIf i = 10 And val Is Nothing Then
                    m_b10 = False
                Else
                    weights(i) = Convert.ToInt32(val)
                End If
            Catch
                weights(i) = 0
            End Try
            If weights(i) > max Then
                max = weights(i)
            End If
        Next

        For i = 0 To 10
            heights(i) = Height * CType(weights(i) / max, Single)
        Next

        GetColor("fontColor", Me.FontColor)
        GetColor("barColor", Me.BarColor)
        GetColor("bgColor", Me.BGColor)
        If (Page.Request.QueryString("fontColor") = "0") Then
            FontColor = Color.Black
        End If
        If (Page.Request.QueryString("bgColor") = "0") Then
            BGColor = Color.White
        End If
        If (Page.Request.QueryString("stars") <> "") Then
            m_bStars = True
            Width = 250
            Height = 150
            _bottomArea = 90
        End If
    End Sub

    Private Sub GetColor(ByVal target As String, ByRef col As Color)
        If Not Page.Request.QueryString(target) Is Nothing Then
            Try
                col = Color.FromArgb(Convert.ToInt64(Page.Request.QueryString(target)))
            Catch

            End Try
        End If
    End Sub


    Private ReadOnly Property FontSize() As Integer
        Get
            Return _fontSize
        End Get
    End Property

    Private Property Height() As Integer
        Get
            Return _height
        End Get
        Set(ByVal Value As Integer)
            If _height > 0 Then
                _height = Value
            End If
        End Set
    End Property

    Private Property Width() As Integer
        Get
            Return _width
        End Get
        Set(ByVal Value As Integer)
            If _width > 0 Then
                _width = Value
            End If
        End Set
    End Property

    Private Property PercentageSpace() As Single
        Get
            Return _percentageSpace
        End Get
        Set(ByVal Value As Single)
            If Value >= 0 Or Value < 1 Then
                _percentageSpace = Value
            End If
        End Set
    End Property

    Private Property NumBars() As Integer
        Get
            Return _numBars
        End Get
        Set(ByVal Value As Integer)
            If Value > 0 Then
                _numBars = Value
            End If
        End Set
    End Property

    Private ReadOnly Property BarWidth() As Single
        Get
            Return Width * (1 - PercentageSpace) / NumBars
        End Get
    End Property

    Private ReadOnly Property SpaceWidth() As Single
        Get
            Return Width * PercentageSpace / NumBars
        End Get
    End Property

    Private Property TextHeight() As Integer
        Get
            Return _bottomArea
        End Get
        Set(ByVal Value As Integer)
            _bottomArea = Value
        End Set
    End Property

    Private Property BarColor() As Color
        Get
            Return _barColor
        End Get
        Set(ByVal Value As Color)
            _barBrush = New SolidBrush(Value)
        End Set
    End Property

    Private Property BGColor() As Color
        Get
            Return _bgColor
        End Get
        Set(ByVal Value As Color)
            _bgBrush = New SolidBrush(Value)
        End Set
    End Property

    Private Property FontColor() As Color
        Get
            Return _fontColor
        End Get
        Set(ByVal Value As Color)
            _fontBrush = New SolidBrush(Value)
        End Set
    End Property

    Public Overrides Sub DrawGraphic()
        Dim bmp As System.Drawing.Bitmap = New System.Drawing.Bitmap(Width, Height + TextHeight)
        Dim cAPI As ContentAPI = Nothing
        Dim iStar As System.Drawing.Image = Nothing
        Dim iStarH As System.Drawing.Image = Nothing
        Dim iStop As System.Drawing.Image = Nothing
        Dim ms As MemoryStream = New MemoryStream()

        If m_bStars = True Then
            cAPI = New ContentAPI
            iStar = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(cAPI.AppPath & "images/UI/icons/star.png"))
            iStarH = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(cAPI.AppPath & "images/UI/icons/starHalf.png"))
            iStop = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(cAPI.AppPath & "images/UI/icons/stop.png"))
        End If

        Dim objGraphics As Graphics = Graphics.FromImage(bmp)
        objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
        objGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
        objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High

        Dim whiteBrush As Brush = _bgBrush
        Dim blackBrush As Brush = _fontBrush

        objGraphics.FillRectangle(_bgBrush, 0, 0, Width, Height + TextHeight)
        Dim i As Integer
        If m_b0 = False And m_b0 = False Then
            NumBars = 9
            For i = 0 To NumBars - 1
                objGraphics.FillRectangle(_barBrush, (BarWidth + SpaceWidth) * i, Height - heights(i + 1), BarWidth, heights(i + 1))
                objGraphics.DrawString((i + 1).ToString(), New System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, Convert.ToSingle(8)), blackBrush, (BarWidth + SpaceWidth) * (i), Height)
            Next
        Else
            For i = 0 To NumBars - 1
                objGraphics.FillRectangle(_barBrush, (BarWidth + SpaceWidth) * i, Height - heights(i), BarWidth, heights(i))
                If m_bStars = True Then
                    If i Mod 2 = 0 And i > 0 Then
                        For j As Integer = 2 To i Step 2
                            objGraphics.DrawImage(iStar, (BarWidth + SpaceWidth) * (i), Height + ((j - 2) * 9))
                        Next
                    ElseIf i > 1 Then
                        Dim dHeight As Decimal
                        For j As Integer = 3 To i Step 2
                            dHeight = (Height + ((j - 2) * 9) - 9)
                            objGraphics.DrawImage(iStar, (BarWidth + SpaceWidth) * (i), dHeight)
                        Next
                        If i = 1 Then
                            dHeight = Height - heights(i) - 18
                        End If
                        objGraphics.DrawImage(iStarH, (BarWidth + SpaceWidth) * (i), dHeight + 18)
                    ElseIf i = 1 Then
                        objGraphics.DrawImage(iStarH, (BarWidth + SpaceWidth) * (i), (Height + (-1 * 9) + 9))
                    ElseIf i = 0 Then
                        objGraphics.DrawImage(iStop, (BarWidth + SpaceWidth) * (i), (Height + (-1 * 9) + 9))
                    End If
                Else
                    objGraphics.DrawString((i).ToString(), New System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, Convert.ToSingle(8)), blackBrush, (BarWidth + SpaceWidth) * i, Height)
                End If
            Next
        End If

        bmp.Save(ms, ImageFormat.Png)
        ms.WriteTo(Page.Response.OutputStream)

        bmp.Dispose()
        ms.Dispose()
    End Sub
End Class

Friend MustInherit Class WorkareaGraphBase
    Protected Page As System.Web.UI.Page

    Public Sub Init(ByVal page As System.Web.UI.Page)
        Me.Page = page
        Me.Page.Response.ContentType = "image/png"
        Initialize()
        Drawgraphic()
    End Sub

    MustOverride Sub Initialize()

    MustOverride Sub Drawgraphic()
End Class