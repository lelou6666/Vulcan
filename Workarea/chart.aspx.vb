Imports Ektron.Cms
Imports System.Drawing
Imports System.Drawing.Imaging

Imports System.Collections.Generic
Partial Class chart
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
	Protected objMod As Ektron.Cms.Modules.EkModule
	Protected m_refContentApi As New ContentAPI
#End Region

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		'Put user code to initialize the page here
		Dim i As Integer

		Dim strStale As String = ""

		Dim lMapWidth As Integer = 600
		Dim lMapHeight As Integer = 500
		Dim strRptDisplay As String = "0"
		Dim drawFont As New System.Drawing.Font("Arial", 10, FontStyle.Bold)
		Dim objGraphics As Graphics
		Dim xPos As Integer = 200
		Dim yPos As Integer = 300
		Dim xInterval As Integer = 20
		Dim dbPercent As Decimal = 0
		Dim lCnt As Integer = 0
		Dim SF As New StringFormat
		SF.FormatFlags = StringFormatFlags.DirectionVertical
		Dim arrValues() As String = Nothing
		Dim arrFieldValues() As String = Nothing
		Dim aTotalCount() As String = Nothing
		Dim strFolderNames() As String
		Dim arrValueNames(1) As String
		Dim strFromPage As String = ""
		Dim strFieldOptionNames() As String = Nothing
		Dim strFieldNames() As String = Nothing
		Dim arrValuesLength As Integer = 0
		Dim bShowPercent As Boolean = False
		Dim symbolLeg As PointF
		Dim descLeg As PointF
		Dim xDimension As Integer = 200
		Dim yDimension As Integer = 200
		Dim strNames As String = ""
        Dim FormId As Long
		Dim arrItem As ArrayList
		Dim cForm As Collection
		Dim arrResult As New ArrayList
		Dim hshQuestions As New Hashtable
		Dim llResponses As Integer = 1
		Dim objReportType As New Ektron.Cms.Common.EkEnumeration.CMSFormReportType


		If (Request.QueryString("showLabels") Is Nothing) Then

			If Not (Request.QueryString("grpdisplay") Is Nothing) Then
				strRptDisplay = Request.QueryString("grpdisplay")
				If (strRptDisplay = Common.EkEnumeration.CMSFormReportType.Combined) Then
					bShowPercent = True
				End If
			End If

			If Not (Request.QueryString("form_page") Is Nothing) Then
				strFromPage = Request.QueryString("form_page")
                FormId = CLng(Request.QueryString("FormId"))
				llResponses = CLng(Request.QueryString("responses"))
				If strRptDisplay = Common.EkEnumeration.CMSFormReportType.Pie Then
					'the total will be re-calculated for the pie chart below
					llResponses = 0
				ElseIf llResponses < 1 Then	'it causes mathematical error if llResponses is <= 0
					llResponses = 1
				End If
				objMod = m_refContentApi.EkModuleRef

				'originally from the formresponse.aspx.vb
				cForm = objMod.GetFormById(FormId) 'collection
				arrResult = m_refContentApi.GetFormDataHistogramById(FormId)	'array
				hshQuestions = m_refContentApi.GetFormFieldQuestionsById(FormId) 'hashtable

				'chart.Visible = True

				' Now we have the data get the values
				'For Each item In FormStats
				'strNames = "18-21,22-25,26-30,31-40,41-50,51-60,61-over:10k-20k,21k-30k,31k-40k:High School,Some College,Degree(Associates),Master,Doctoral,Professional"
				'strStale = "10,30,25,10,5,5,15:10,50,40:10,10,10,10,10,10"
				'strFieldNames = "Age range:Annual Income:Education level"

				'EktComma is used to retain the commas in the fields and field option names
				Dim idx, j As Integer
				Dim iOptionHit As Integer = 0

				'questions array
				If hshQuestions.Count > 0 And arrResult.Count > 0 Then
					ReDim strFieldNames(arrResult.Count - 1)
					For idx = 0 To arrResult.Count - 1
						arrItem = arrResult.Item(idx)
						If arrItem.Count > 0 Then
							'sFieldNames = sFieldNames & ":" & hshQuestions(cItem.Item(0).ToString().Replace(",", "EktComma"))
							strFieldNames(idx) = Common.EkFunctions.HtmlDecode(hshQuestions(arrItem.Item(0).ToString()))
						End If
					Next
				End If

				Dim sSubmit As String
				Dim iSubmit As Integer
				Dim iMaxSubmit As Integer = 0
				arrItem = Nothing
				'For Each cItem In cResult
				If arrResult.Count > 0 Then
					ReDim strFieldOptionNames(arrResult.Count - 1)
					ReDim arrFieldValues(arrResult.Count - 1)
					ReDim aTotalCount(arrResult.Count - 1)
					For idx = 0 To arrResult.Count - 1
						arrItem = arrResult.Item(idx)
						If arrItem.Count > 1 Then
							For j = 1 To arrItem.Count - 1
								'option text list
								strFieldOptionNames(idx) = strFieldOptionNames(idx) & arrItem.Item(j).ToString().Substring(0, arrItem.Item(j).ToString().LastIndexOf(",") - 5) & "{sep}" 'Count = 5 chars
								sSubmit = arrItem.Item(j).ToString().Substring(arrItem.Item(j).ToString().LastIndexOf(",") + 1)
								iSubmit = CInt(sSubmit.ToString.Substring(sSubmit.ToString.IndexOf("/") + 1))
								iOptionHit = CInt(sSubmit.ToString.Substring(0, sSubmit.ToString.IndexOf("/")))
								If iSubmit > iMaxSubmit Then
									iMaxSubmit = iSubmit
								End If
								'iOptionHit = arrItem.Item(j).ToString().Substring(arrItem.Item(j).ToString().LastIndexOf(",") + 1)
								If strRptDisplay = Common.EkEnumeration.CMSFormReportType.Pie Then
									'option count list
									aTotalCount(idx) = aTotalCount(idx) + iOptionHit
									arrFieldValues(idx) = arrFieldValues(idx) & iOptionHit & ","
								Else
									'option count (in percent) list
									arrFieldValues(idx) = arrFieldValues(idx) & CInt((iOptionHit * 100) / llResponses) & ","
								End If
								iOptionHit = 0
							Next
							'option text list
							strFieldOptionNames(idx) = strFieldOptionNames(idx).Substring(0, strFieldOptionNames(idx).Length - 5) ' {sep} = 5 chars
							'option count (in percent) list
							arrFieldValues(idx) = arrFieldValues(idx).Substring(0, arrFieldValues(idx).Length - 1)
						End If
					Next
					'sFieldOptionNames = Server.UrlEncode(sFieldOptionNames.Substring(0, sFieldOptionNames.Length - 1))
					'sFieldOptionValues = sFieldOptionValues.Substring(0, sFieldOptionValues.Length - 1)
					arrValuesLength = arrItem.Count + arrResult.Count
				End If
			End If

			If (strFromPage = "") Then
				arrValueNames(0) = "Updated Content"
				arrValueNames(1) = "Stale Content"
				arrValues = Request.QueryString("stale").Split(",")
				arrValuesLength = arrValues.Length
			End If

			If (strRptDisplay = Common.EkEnumeration.CMSFormReportType.DataTable) Then
				strFolderNames = Request.QueryString("names").Split(",")
			ElseIf (strFromPage = "form_page") Then
				'strFieldOptionNames = Server.UrlDecode(Request.QueryString("fieldOptionNames")).Split(":")
				'strFieldNames = Server.UrlDecode(Request.QueryString("fieldNames")).Split(":")
				'arrFieldValues = Request.QueryString("FormValues").Split(":") 'Values for multiple fields separated by :
				'arrValuesLength = Request.QueryString("FormValues").Split(",").Length + strFieldNames.Length
			End If

			If (strRptDisplay = Common.EkEnumeration.CMSFormReportType.DataTable) Then
				lMapWidth = 220 + (arrValuesLength * 40) + (arrValuesLength - 1) * 20
				yPos = 400
				lMapHeight = yPos + 20
			End If

			If (strRptDisplay = "0") Or (strRptDisplay = Common.EkEnumeration.CMSFormReportType.Combined) Then
				lMapHeight = 220 + (arrValuesLength * 40) + (arrValuesLength - 1) * 20
			End If

			Dim NumOfLegends As Integer = 0

			If (strRptDisplay = Common.EkEnumeration.CMSFormReportType.Pie) And (strFromPage = "form_page") Then
				lMapWidth = 750 + (40 * arrValuesLength) + 700
				'lMapHeight = (225 * strFieldNames.Length)  'give 500 as padding
				For i = 0 To arrFieldValues.GetUpperBound(0)
					NumOfLegends = NumOfLegends + arrFieldValues(i).Split(",").GetLength(0)
					If ((20 * NumOfLegends) > 400) Then
						lMapHeight = lMapHeight + ((20 * NumOfLegends) + 30)
					Else
						lMapHeight = lMapHeight + 450
					End If
				Next
				'If (300 * strFieldNames.Length) > ((20 * NumOfLegends) + (40 * strFieldNames.Length)) Then
				'    lMapHeight = 300 * strFieldNames.Length
				'Else
				'    lMapHeight = (20 * NumOfLegends) + (40 * strFieldNames.Length)
				'End If
				yPos = lMapHeight
			End If
            Dim objBitMap As New System.Drawing.Bitmap(lMapWidth, lMapHeight)

			objGraphics = Graphics.FromImage(objBitMap)

			objGraphics.Clear(Color.White)

			If (strFromPage = "") Then
				objGraphics.DrawString("Stale Content Report", drawFont, Brushes.Black, New PointF(5, 5))

				symbolLeg = New PointF(lMapWidth - 190, 20)

				descLeg = New PointF(lMapWidth - 165, 16)

				For i = 0 To arrValueNames.Length - 1

					objGraphics.FillRectangle(New SolidBrush(GetColor(i Mod 2)), symbolLeg.X, symbolLeg.Y, 10, 10)
					objGraphics.DrawRectangle(Pens.Black, symbolLeg.X, symbolLeg.Y, 10, 10)

					objGraphics.DrawString(arrValueNames(i).ToString, drawFont, Brushes.Black, descLeg)

					symbolLeg.Y += 15

					descLeg.Y += 15

				Next i
			End If
			'Loop through the values to create the Bar Chart.

			If (strRptDisplay = Common.EkEnumeration.CMSFormReportType.DataTable) Then
				Dim j As Integer
				xPos = 50
				For i = 0 To arrValuesLength - 1
					' Vertical display
					objGraphics.DrawLine(Pens.Black, xPos, 50, xPos, yPos) ' Vertical axis
					objGraphics.DrawString("Percent Stale Content ->", drawFont, Brushes.Black, New PointF(xPos, 20), SF)
					objGraphics.DrawLine(Pens.Black, xPos, yPos, xPos + lMapWidth, yPos) ' Horizontal axis

					objGraphics.DrawString("Content Folders ->", drawFont, Brushes.Black, New PointF(xPos + lMapWidth - 180, yPos))

					For j = 0 To 10
						objGraphics.DrawLine(Pens.Black, xPos - 2, yPos - (30 * j), xPos + 2, yPos - (30 * j))
						objGraphics.DrawString(CStr(10 * j), drawFont, Brushes.Black, New PointF(xPos - 25, yPos - (30 * j) - 5))
						'objGraphics.DrawString("Test", drawFont, Brushes.Black, New PointF(xPos - 50, yPos - (30 * j) - 5))
					Next

					'objGraphics.DrawString("yPos = " & yPos & " x =" & xPos + xInterval, drawFont, Brushes.Black, New PointF(xPos + xInterval, yPos - dbPercent))

					If (CDbl(arrValues(i).Substring(arrValues(i).IndexOf(":") + 1)) > 0) Then
						dbPercent = System.Math.Round(((CDbl(arrValues(i).Substring(0, arrValues(i).IndexOf(":")))) / (CDbl(arrValues(i).Substring(arrValues(i).IndexOf(":") + 1)))) * 100, 3)
					Else
						dbPercent = 0
					End If

					objGraphics.FillRectangle(New SolidBrush(GetColor(0)), xPos + xInterval, yPos - (dbPercent * 3), 20, (dbPercent * 3))
					objGraphics.DrawRectangle(Pens.Black, xPos + xInterval, yPos - (dbPercent * 3), 20, (dbPercent * 3))

					If (CDbl(arrValues(i).Substring(arrValues(i).IndexOf(":") + 1)) > 0) Then
						dbPercent = System.Math.Round(((CDbl(arrValues(i).Substring(arrValues(i).IndexOf(":") + 1)) - CDbl(arrValues(i).Substring(0, arrValues(i).IndexOf(":")))) / (CDbl(arrValues(i).Substring(arrValues(i).IndexOf(":") + 1)))) * 100, 3)
					End If

					xInterval = xInterval + 20
					objGraphics.FillRectangle(New SolidBrush(GetColor(1)), xPos + xInterval, yPos - (dbPercent * 3), 20, (dbPercent * 3))
					objGraphics.DrawRectangle(Pens.Black, xPos + xInterval, yPos - (dbPercent * 3), 20, (dbPercent * 3))
					'objGraphics.DrawString(strFolderNames(i).Substring(strFolderNames(i).LastIndexOf("\\") + 2), drawFont, Brushes.Black, New PointF(xPos + xInterval, yPos), SF)
					xInterval = xInterval + 40
				Next
			ElseIf (strRptDisplay = "0") Or ((strRptDisplay = Common.EkEnumeration.CMSFormReportType.Combined) And (strFromPage = "form_page")) Then
				' Horizontal display
				If (strFromPage = "form_page") Then
					Dim j As Integer
					Dim bShowAxis As Boolean
					'Dim dScale As Double


					If Not (Request.QueryString("showAxis") Is Nothing) Then
						bShowAxis = CBool(Request.QueryString("showAxis"))
					End If

					'If Not (Request.QueryString("scale") Is Nothing) Then
					'    dScale = CDbl(Request.QueryString("scale"))
					'End If

					lMapHeight = yPos + (arrValuesLength * 40) + (arrValuesLength * 20)
					'yPos = lMapHeight - 300
					yPos = 20
					xPos = 0
					'objGraphics.DrawString("Responses - " & Request.QueryString("responses"), drawFont, Brushes.Black, New PointF(xPos + 100, yPos - 20))

					For j = 0 To strFieldOptionNames.Length - 1
						strFolderNames = System.Text.RegularExpressions.Regex.Split(strFieldOptionNames(j).ToString(), "{sep}")
						arrValues = arrFieldValues(j).Split(",")

						'objGraphics.DrawString(strFieldNames(j).Replace("EktComma", ","), drawFont, Brushes.Black, New PointF(xPos + 50, yPos + 10)) ' The Bar title below the x axis
						objGraphics.DrawString(strFieldNames(j), drawFont, Brushes.Black, New PointF(xPos + 50, yPos + 10))	' The Bar title below the x axis
						yPos = yPos + 15
						If (bShowAxis) Then
							objGraphics.DrawLine(Pens.Black, xPos, yPos + (arrValues.Length) * 40, xPos, yPos) ' Vertical axis
							objGraphics.DrawLine(Pens.Black, xPos, yPos + (arrValues.Length) * 40, xPos + lMapWidth, yPos + (arrValues.Length) * 40) ' Horizontal axis

							For i = 0 To 10
								objGraphics.DrawLine(Pens.Black, xPos + (30 * i), (yPos + (arrValues.Length) * 40) - 2, xPos + (30 * i), (yPos + (arrValues.Length) * 40) + 2)
								objGraphics.DrawString(CStr(10 * i), drawFont, Brushes.Black, New PointF(xPos + (30 * i), yPos + (arrValues.Length) * 40))
							Next
						End If
						yPos = yPos + xInterval
						'xInterval = xInterval + (30 * arrValues.Length)
						For i = 0 To arrValues.Length - 1
							'strFolderNames(i) = strFolderNames(i).ToString().Replace("EktComma", ",")
							strFolderNames(i) = strFolderNames(i).ToString()
							dbPercent = System.Math.Round(CDbl(arrValues(i)), 3) '* dScale (?)
							objGraphics.FillRectangle(New SolidBrush(GetColor(i Mod 24)), xPos, yPos, dbPercent, 10)
							objGraphics.DrawRectangle(Pens.Black, xPos, yPos, dbPercent, 10)
							If (objGraphics.MeasureString(strFolderNames(i), drawFont).Width > xDimension) Then
								xDimension = objGraphics.MeasureString(strFolderNames(i), drawFont).Width
							End If
							If (bShowPercent) Then
								objGraphics.DrawString(Common.EkFunctions.HtmlDecode(strFolderNames(i)) & " (" & dbPercent.ToString() & "%)", drawFont, Brushes.Black, New PointF(xPos, (yPos + 10)))
							Else
								objGraphics.DrawString(Common.EkFunctions.HtmlDecode(strFolderNames(i)), drawFont, Brushes.Black, New PointF(xPos, yPos + 10))
							End If
							'xInterval = xInterval - 30
							yPos = yPos + 30
						Next
						'yPos = yPos + xInterval + 30
						yPos = yPos + 10
						'xInterval = 20
					Next
					yDimension = yPos
				Else
					lMapHeight = yPos + (arrValuesLength * 40) + (arrValuesLength * 20)
					yPos = lMapHeight - 200

					objGraphics.DrawLine(Pens.Black, xPos, 50, xPos, yPos) ' Vertical axis
					objGraphics.DrawLine(Pens.Black, xPos, yPos, xPos + lMapWidth, yPos) ' Horizontal axis
					objGraphics.DrawString("Content Folders ->", drawFont, Brushes.Black, New PointF(xPos - 20, 10), SF)
					objGraphics.DrawString("Percent Stale Content ->", drawFont, Brushes.Black, New PointF(lMapWidth - 180, yPos + 15))

					For i = 0 To 10
						objGraphics.DrawLine(Pens.Black, xPos + (30 * i), yPos - 2, xPos + (30 * i), yPos + 2)
						objGraphics.DrawString(CStr(10 * i), drawFont, Brushes.Black, New PointF(xPos + (30 * i), yPos))
					Next

					For i = 0 To arrValuesLength - 1
						If (CDbl(arrValues(i).Substring(arrValues(i).IndexOf(":") + 1)) > 0) Then
							dbPercent = System.Math.Round(((CDbl(arrValues(i).Substring(0, arrValues(i).IndexOf(":")))) / (CDbl(arrValues(i).Substring(arrValues(i).IndexOf(":") + 1)))) * 100, 3)
						Else
							dbPercent = 0
						End If

						objGraphics.FillRectangle(New SolidBrush(GetColor(0)), xPos, yPos - xInterval, dbPercent * 3, 20)
						objGraphics.DrawRectangle(Pens.Black, xPos, yPos - xInterval, dbPercent * 3, 20)

						If (CDbl(arrValues(i).Substring(arrValues(i).IndexOf(":") + 1)) > 0) Then
							dbPercent = System.Math.Round(((CDbl(arrValues(i).Substring(arrValues(i).IndexOf(":") + 1)) - CDbl(arrValues(i).Substring(0, arrValues(i).IndexOf(":")))) / (CDbl(arrValues(i).Substring(arrValues(i).IndexOf(":") + 1)))) * 100, 3)
						End If

						xInterval = xInterval + 20
						objGraphics.FillRectangle(New SolidBrush(GetColor(1)), xPos, yPos - xInterval, dbPercent * 3, 20)
						objGraphics.DrawRectangle(Pens.Black, xPos, yPos - xInterval, dbPercent * 3, 20)

						'objGraphics.DrawString(strFolderNames(i), drawFont, Brushes.Black, New PointF(xPos - 100, yPos - xInterval))
						xInterval = xInterval + 40
					Next
				End If


			ElseIf (strRptDisplay = Common.EkEnumeration.CMSFormReportType.Pie) Then
				Dim sglCurrentAngle As Single = 0
				Dim sglTotalAngle As Single = 0
				Dim llTotal As Integer = 0
				Dim j, idx As Integer
				Dim xMax As Integer = 0
				Dim yMax As Integer = 0
				Dim dPercent As Decimal = 0.0
                Dim dTotalCheck As Decimal = 0.0
                Dim QuestionBoxHeight As Integer = 15
				yPos = 0
				xPos = 0
				'objGraphics.DrawString("Responses - " & Request.QueryString("responses"), drawFont, Brushes.Black, New PointF(xPos + 100, yPos + 5))
				If strFieldOptionNames.Length > 0 Then
					ReDim strFolderNames(strFieldOptionNames.Length - 1)
					For j = 0 To strFieldOptionNames.Length - 1
						strFolderNames = System.Text.RegularExpressions.Regex.Split(strFieldOptionNames(j).ToString(), "{sep}")
						arrValues = arrFieldValues(j).Split(",")
						llTotal = 0
						For i = 0 To arrValues.Length - 1
							llTotal = llTotal + arrValues(i)
						Next
                        If aTotalCount(j) > 0 Then
                            QuestionBoxHeight = (CInt(strFieldNames(j).Length / 50) + 1) * 20
                            objGraphics.DrawString(strFieldNames(j).Replace("EktComma", ","), drawFont, Brushes.Black, New RectangleF(xPos, yPos, 320, QuestionBoxHeight))
                        End If
						'Dim test As Integer = yPos
						For i = 0 To arrValues.Length - 1

							'Current Value / (sum of all the Values) * 360 degree angle
							If aTotalCount(j) < 1 Then
								Exit For
							End If
							sglCurrentAngle = (arrValues(i) / llTotal) * 360
							'yPos = yPos + (i * 200)
							'objGraphics.FillPie(New SolidBrush(GetColor(i Mod 16)), xPos, yPos - 230, 200, 200, sglTotalAngle, sglCurrentAngle)
							'objGraphics.DrawString(strFieldNames(j), drawFont, Brushes.Black, New PointF(xPos + 75, yPos - 275))
							'objGraphics.DrawPie(Pens.Black, xPos, yPos - 230, 200, 200, sglTotalAngle, sglCurrentAngle)
                            objGraphics.FillPie(New SolidBrush(GetColor(i Mod 24)), xPos, yPos + QuestionBoxHeight, 200, 200, sglTotalAngle, sglCurrentAngle)
							'objGraphics.DrawString(strFieldNames(j).Replace("EktComma", ","), drawFont, Brushes.Black, New PointF(xPos, yPos))
                            objGraphics.DrawPie(Pens.Black, xPos, yPos + QuestionBoxHeight, 200, 200, sglTotalAngle, sglCurrentAngle)
                            yMax = yPos + QuestionBoxHeight + 200
							'objGraphics.DrawLine(Pens.Blue, xPos + 100, yPos + 175, xPos + 200, yPos + 200)
							'objGraphics.DrawString("center = ", drawFont, Brushes.Black, New PointF(xPos + 100, yPos + 175))
							'objGraphics.DrawString("angle = " & sglCurrentAngle.ToString(), drawFont, Brushes.Black, New PointF(xPos, test))

							'test = test + 20
							sglTotalAngle += sglCurrentAngle
						Next
                        yPos = yPos + QuestionBoxHeight
						symbolLeg = New PointF(xPos + 210, yPos)
						descLeg = New PointF(xPos + 220, yPos)
						arrValueNames = System.Text.RegularExpressions.Regex.Split(strFieldOptionNames(j), "{sep}")
						'reset check total for the next set of results
						dTotalCheck = 0.0
						For idx = 0 To arrValueNames.Length - 1
							If aTotalCount(j) < 1 Then
								'to avoid division overflow
								Exit For
							End If
							objGraphics.FillRectangle(New SolidBrush(GetColor(idx Mod 24)), symbolLeg.X, symbolLeg.Y, 10, 10)
							objGraphics.DrawRectangle(Pens.Black, symbolLeg.X, symbolLeg.Y, 10, 10)
							If (xMax < objGraphics.MeasureString(arrValueNames(idx).ToString().Replace("EktComma", ",") & " (" & arrValues(idx) & " %)", drawFont).Width) Then
								xMax = objGraphics.MeasureString(arrValueNames(idx).ToString().Replace("EktComma", ",") & " (" & arrValues(idx) & " %)", drawFont).Width + 10
							End If
							dPercent = Decimal.Round(Convert.ToDecimal(arrValues(idx) / aTotalCount(j) * 100), 2)
							'correct the percentage so if the total is over 100 
							dTotalCheck = dTotalCheck + dPercent
							If dTotalCheck > 100.0 And dPercent > 0 Then
								dPercent = dPercent - (dTotalCheck - 100.0)
								'reset check total for the current set of results
								dTotalCheck = 100.0
							End If
							objGraphics.DrawString(Common.EkFunctions.HtmlDecode(arrValueNames(idx).ToString().Replace("EktComma", ",")) & " (" & dPercent & " %)", drawFont, Brushes.Black, descLeg)
							symbolLeg.Y += 20
							descLeg.Y += 20
						Next
						yPos = descLeg.Y + 10
						If (yPos < yMax) Then
							yPos = yMax
						End If
					Next
					If (yPos > yMax) Then
						' more legends than the circle
						yMax = yPos
					End If
					yDimension = yMax '- 30 'lMapHeight - 30
					xDimension = xMax + 200
				End If
			End If
			'Loop through the values to create the Pie Chart.
			If (strFromPage = "form_page") Then
				'Calculate exact dimensions
				objGraphics.Dispose()
                Dim bmp As New System.Drawing.Bitmap(xDimension + 25, yDimension + 30)
				objGraphics = Graphics.FromImage(bmp)
				objGraphics.DrawImage(objBitMap, 0, 0, lMapWidth, lMapHeight)
				bmp.Save(Response.OutputStream, ImageFormat.Gif)
			Else
				objBitMap.Save(Response.OutputStream, ImageFormat.Gif)
			End If
		Else
			Dim strFolder As String = ""
			strFolder = Request.QueryString("showLabels").ToString()

            Dim objBitMap As System.Drawing.Bitmap = New System.Drawing.Bitmap(50, 200)

			objGraphics = Graphics.FromImage(objBitMap)
			objGraphics.Clear(Color.White)
			If (strFolder.Length > 20) Then
				strFolder = strFolder.Substring(0, 20) & "..."
			End If

			objGraphics.DrawString(strFolder, drawFont, Brushes.Black, New PointF(0, 15), SF)
			objBitMap.Save(Response.OutputStream, ImageFormat.Gif)
		End If

	End Sub

	Private Function GetColor(ByVal itemIndex As Integer) As Color
		Dim ColorO As Integer = &H99CCFF 'light blue
		' These color is selected in the order of opposite location in the Web Designer's Color Card 
		' by Visibone.  It hopes to accommodate every espects including B&W printer and color-blindness.
		Select Case itemIndex
			Case 0
				ColorO = &H6666FF 'blue
			Case 1
				ColorO = &HFFFF66 'yellow
			Case 2
				ColorO = &H669999  'grey
			Case 3
				ColorO = &H996666 'red
			Case 4
				ColorO = &HCC99FF 'light purple
			Case 5
				ColorO = &HCCFF99 'light grreen
			Case 6
				ColorO = &HFF99CC 'light pink
			Case 7
				ColorO = &H99FFCC 'light green
			Case 8
				ColorO = &HCC66FF 'purple
			Case 9
				ColorO = &H99FF66 'green
			Case 10
				ColorO = &HFF66CC 'purple
			Case 11
				ColorO = &H66FF99 'green
			Case 12
				ColorO = &H666699 'purple
			Case 13
				ColorO = &H999966 'green
			Case 14
				ColorO = &H66FFFF 'blue
			Case 15
				ColorO = &HFF6666 'pink
			Case 16
				ColorO = &H9999FF 'Blue
			Case 17
				ColorO = &H99CCCC 'gray
			Case 18
				ColorO = &HFFFF99 'yellow
			Case 19
				ColorO = &HCC9999 'pink
			Case 20
				ColorO = &H9933FF 'Lavender
			Case 21
				ColorO = &H99FF33 'green
			Case 22
				ColorO = &H33FF99 'green
			Case 23
				ColorO = &HFF3399 'dark pink
			Case Else
				ColorO = &H99CCFF 'light blue

		End Select

		Return ColorTranslator.FromOle(ColorO)

	End Function


End Class
