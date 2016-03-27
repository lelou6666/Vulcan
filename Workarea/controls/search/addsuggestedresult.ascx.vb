Imports System.Data
Imports Ektron.Cms
Imports Ektron.Cms.SuggestedResultData
Imports System.Collections.Generic

Partial Class Workarea_controls_search_addsuggestedresult
    Inherits System.Web.UI.UserControl

    Protected language_data As LanguageData()
    Protected user_data As UserData
    Protected security_data As PermissionData
    Protected m_refSiteApi As New SiteAPI
    Protected m_refUserApi As New UserAPI
    Protected m_refContentApi As New ContentAPI
    Protected m_refStyle As New StyleHelper
    Protected m_refMsg As Ektron.Cms.Common.EkMessageHelper
    Protected AppImgPath As String = ""
    Protected ContentLanguage As Integer = -1
    Protected thisContentLanguage As String = ""
    Protected api As New CommonApi()
    Protected PageMode As String = ""
    Protected PageAction As String = ""
    Protected inputTermID As Guid = Nothing
    Protected SuggestedResultRecommendedMaxSize As Integer = 10
#Region "FUNCTION: AreArraysEqual(array1, array2) - Checks if 2 ARRAYS are identical to one another"
    ''' <summary>
    ''' A simple function that will verify if two different arrays are in fact equal to one another.
    ''' </summary>
    ''' <param name="one">The first array for comparison.</param>
    ''' <param name="two">The second array for comparison.</param>
    ''' <returns>True if the arrays are inf act equal.</returns>
    ''' <remarks></remarks>
    Public Function AreArraysEqual(ByRef one As Object(), ByRef two As Object()) As Boolean
        ' check lengths are the same
        If one.Length <> two.Length Then
            Return False
        End If
        ' check each element
        For index As Integer = 0 To one.Length - 1
            If (one.GetValue(index) <> two.GetValue(index)) Then
                Return False
            End If
        Next
        Return True
    End Function
#End Region

#Region "FUNCTION: CheckName(name) - Used to validate NAME inputs"
    ''' <summary>
    ''' A function for validating the name property for new Terms.
    ''' </summary>
    ''' <param name="name">The name we're going to validate.</param>
    ''' <returns>True if the name is valid.</returns>
    ''' <remarks></remarks>
    Public Function CheckName(ByRef name As String) As Boolean
        If name.Length = 0 Then
            Throw New Exception(m_refMsg.GetMessage("msg error term name unspecified"))
            Return False
        End If
        If Regex.IsMatch(name, ",") Then
            Throw New Exception(m_refMsg.GetMessage("msg error term name violation") & "<br />")
            Return False
        End If
        Return True
    End Function
#End Region

#Region "FUNCTION: CheckTerms(terms, type) - Used to validate TERMS inputs"
    ''' <summary>
    ''' A function for validating the Terms property of TermsData class objects.
    ''' </summary>
    ''' <param name="terms">The termss string we wish to validate.</param>
    ''' <param name="type">The TermsData type of the TermsData class associated with these terms.</param>
    ''' <returns>True if the terms are valid.</returns>
    ''' <remarks></remarks>
    Public Function CheckTerms(ByRef terms As String, ByRef type As Integer) As Boolean
        If Not terms.Length > 0 Then
            Throw New Exception(m_refMsg.GetMessage("msg error term-termset unspecified"))
            Return False
        End If
        If type = 0 And Regex.IsMatch(terms, ";") Then
            Throw New Exception(m_refMsg.GetMessage("msg error term type single violation"))
            Return False
        End If
        If Regex.IsMatch(terms, ",") Then
            Throw New Exception(m_refMsg.GetMessage("msg error term name violation"))
            Return False
        End If
        Return True
    End Function
#End Region

#Region "FUNCTION: CheckTermExists(termsData, termsDataObject, isUpdate) - checks to see if a provided TERM already exists in the Terms dB table"
    ''' <summary>
    ''' A simple function to determine if a given term already exists in the CMS system.
    ''' </summary>
    ''' <param name="termsData">A TermsData class object that we're checking to see if it exists.</param>
    ''' <param name="termsObject">A Terms object so we can access the necessary access the Terms API methods.</param>
    ''' <param name="isUpdate">A binary indicating whether or not we are updating an existing term or trying to add a whole new term.  This makes a difference for when we check to see if the term exists or not.  If it's an update, we're really just checking for duplicates, as obviously the term already exists.  We just want to ensure the new properties for the TermsData won't result in duplication in the system.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckTermExists(ByRef termsData As Ektron.Cms.Common.TermsData, ByRef termsObject As Ektron.Cms.Content.Terms, ByRef isUpdate As Boolean) As Ektron.Cms.Common.TermsData
        ' get a collection of the existing terms
        Dim termsDataList As List(Of Ektron.Cms.Common.TermsData) = termsObject.GetTermsByType(termsData.Type, ContentLanguage)

        ' place the terms in an array for later comparison, and create a string to use for later comparison
        Dim TermsToAdd As Array = Split(termsData.Terms, ";")

        Try
            ' check to make sure we have more than one Term if the Type is TermSet
            If termsData.Type = 1 And TermsToAdd.Length = 1 Then
                Throw New Exception(Server.UrlEncode("<strong>" & m_refMsg.GetMessage("msg error improper type specification") & "</strong>:" & m_refMsg.GetMessage("msg error term validation rule")))
            End If
            Dim TermsToAddTrimmedString As String = ""
            ' trim the individual values
            Dim TermsToAddIndex As Integer
            For TermsToAddIndex = 0 To TermsToAdd.Length - 1
                If TermsToAdd(TermsToAddIndex) <> "" Then
                    TermsToAdd(TermsToAddIndex) = Trim(TermsToAdd(TermsToAddIndex))
                    If TermsToAddTrimmedString = "" Then
                        TermsToAddTrimmedString = Trim(TermsToAdd(TermsToAddIndex))
                    Else
                        TermsToAddTrimmedString += "; " & Trim(TermsToAdd(TermsToAddIndex))
                    End If
                End If
            Next

            Array.Sort(TermsToAdd)
            Dim compareTerms As String = ""
            For Each compareTerm As String In TermsToAdd
                compareTerms += ";" & compareTerm
            Next
            ' rebuild the terms to add string with the trimmed values
            termsData.Terms = TermsToAddTrimmedString

            ' create a new array that will store any existing terms, and a string to use for comparison
            Dim ExistingTerms As Array
            Dim compareTermsData As New Ektron.Cms.Common.TermsData
            compareTermsData.Name = termsData.Name
            compareTermsData.TermID = termsData.TermID
            compareTermsData.Type = termsData.Type
            compareTermsData.DateModified = termsData.DateModified
            compareTermsData.Terms = LCase(compareTerms.Trim(";"))
            Dim compareTermsArray As Array = Split(compareTermsData.Terms, ";")
            ' loop through the existing terms on file and check for collisions and errors
            For Each term As Ektron.Cms.Common.TermsData In termsDataList
                ExistingTerms = Split(LCase(term.Terms), ";")
                For existingTermIndex As Integer = 0 To ExistingTerms.Length - 1
                    ExistingTerms(existingTermIndex) = Trim(ExistingTerms(existingTermIndex))
                Next
                Array.Sort(ExistingTerms)

                ' check to see if the arrays are equal
                If isUpdate Then
                    If term.TermID <> termsData.TermID Then
                        If AreArraysEqual(ExistingTerms, compareTermsArray) Then
                            ' Term Set already exists, return it's ID
                            Return term
                        End If
                    End If
                Else
                    If AreArraysEqual(ExistingTerms, compareTermsArray) Then
                        ' Term Set already exists, return it's ID
                        Return term
                    End If
                End If

                Dim index As Integer
                Dim length As Integer
                Array.Clear(ExistingTerms, index, length)
            Next
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
            Return Nothing
        End Try
        Return Nothing
    End Function
#End Region

#Region "FUNCTION: addNewTerm(termsData, termsObject)"
    ''' <summary>
    ''' This function will add a new term into the database for use by either Synonym Sets, Suggested Results or other future Search related features.
    ''' </summary>
    ''' <param name="termsData">A TermsData class object.  The TermsData class object will need to have valid name and terms property values in order fot he term to be added correctly. </param>
    ''' <param name="termsObject">A Terms class object.  This object's AddTerms() method is what allows us to add the term.</param>
    ''' <returns>The ID of the sucessully added term.</returns>
    ''' <remarks></remarks>
    Public Function addNewTerm(ByRef termsData As Ektron.Cms.Common.TermsData, ByRef termsObject As Ektron.Cms.Content.Terms) As System.Guid
        ' Use the AddTerms method to add the new entry to the dB
        Try
            ' check if name exists
            CheckName(termsData.Name)
            CheckTerms(termsData.Terms, termsData.Type)
            Dim existingTermID As Guid = Nothing
            If Not IsNothing(CheckTermExists(termsData, termsObject, 0)) Then
                existingTermID = CheckTermExists(termsData, termsObject, 0).TermID
            End If

            If Not existingTermID.Equals(Guid.Empty) Then
                Throw New Exception(String.Format(m_refMsg.GetMessage("msg error improper type specification"), termsData.Terms, existingTermID.ToString()))
            End If

            ' if we made it this far, add the new term
            termsData.TermLanguage = ContentLanguage
            Dim result As System.Guid = termsObject.AddTerms(termsData)
            If Not IsNothing(result) Then
                Return result
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
            Return Nothing
        End Try
        Return Nothing
    End Function
#End Region

#Region "SUB ProcessAddSuggestedResult"
    Private Sub ProcessAddSuggestedResult()
        ' instantiate the required variables we'll use
        Dim theTerm As New Ektron.Cms.Common.TermsData
        Dim suggestedResultsTermsObject As New Ektron.Cms.Content.Terms(api.RequestInformationRef)
        Dim numberSuggestedResults As Integer = Integer.Parse(Request.Form("numSuggestedResults"))
        Dim addSuggestedResultData As New Ektron.Cms.SuggestedResultData
        Dim suggestedResultsToAddObject As New Ektron.Cms.Content.EkSuggestedResults(api.RequestInformationRef)

        ' get the term information
        theTerm.Type = Integer.Parse(Request.Form("termType"))
        If (theTerm.Type = 1) Then
            ' the term is a Synonym Set, get it's ID
            theTerm.TermID = New Guid(Request.Form("synonymSetTerm"))
        Else
            ' the term is a singular term, and we're going to have to check if it already exists
            Dim termID As Guid = Nothing

            theTerm.Name = Request.Form("addsuggestedresult$term")
            theTerm.Terms = Request.Form("addsuggestedresult$term")
            Try
                CheckName(theTerm.Name)
                CheckTerms(theTerm.Terms, theTerm.Type)
                If Not IsNothing(CheckTermExists(theTerm, suggestedResultsTermsObject, 0)) Then
                    termID = CheckTermExists(theTerm, suggestedResultsTermsObject, 0).TermID
                End If

                If Not IsNothing(termID) AndAlso Not (termID.ToString().Trim() = Guid.Empty.ToString()) Then
                    ' use the existing term's ID value
                    theTerm.TermID = termID
                Else
                    ' the term doesn't exist already, so let's add it to terms dB table
                    termID = addNewTerm(theTerm, suggestedResultsTermsObject)
                    theTerm.TermID = termID
                End If
            Catch ex As Exception
                Utilities.ShowError(ex.Message)
            End Try
        End If

        Dim binAddSuccessful As Boolean = False
        Try
            theTerm = suggestedResultsTermsObject.GetTermById(theTerm.TermID, ContentLanguage)
            ' first check to see if this term already has some suggested results
            Dim suggestedResultList As New List(Of SuggestedResultData)
            ' get the results for this term
            suggestedResultList = suggestedResultsToAddObject.GetSuggestedResultSetForTerms(theTerm.TermID, ContentLanguage)
            If suggestedResultList.Count > 0 Then
                Throw New Exception(Server.UrlEncode(String.Format(m_refMsg.GetMessage("msg error edit suggested results instead"), theTerm.Name.ToString, theTerm.TermID.ToString, ContentLanguage).ToString))
            End If


            ' if we've made it this far, let's add some Suggested Results
            Dim toAddCount As String = ""
            For suggestedResultToAdd As Integer = 0 To numberSuggestedResults - 1
                ' let's make a shortened version of the count
                toAddCount = suggestedResultToAdd.ToString
                ' assign the suggested results the correct data points
                addSuggestedResultData.KeywordID = theTerm.TermID
                addSuggestedResultData.TermName = theTerm.Name
                addSuggestedResultData.Type = theTerm.Type
                addSuggestedResultData.Summary = Request.Form("suggestedResult_Summary_" & toAddCount)
                addSuggestedResultData.Title = Request.Form("suggestedResult_Title_" & toAddCount)
                addSuggestedResultData.QuickLink = Request.Form("suggestedResult_Link_" & toAddCount)
                Dim ContentIDValue As String = Request.Form("suggestedResult_ContentID_" & toAddCount)
                If ContentIDValue IsNot Nothing And ContentIDValue < "" Then
                    addSuggestedResultData.ContentID = Long.Parse(ContentIDValue)
                Else
                    addSuggestedResultData.ContentID = -1
                End If

                addSuggestedResultData.ContentLanguage = ContentLanguage

                addSuggestedResultData.SearchOrder() = Integer.Parse(Request.Form("suggestedResult_SearchOrder_" & toAddCount))

                ' let's add it
                Try
                    Dim addSuccessful As Guid = suggestedResultsToAddObject.AddSugestedResult(addSuggestedResultData)
                    If IsNothing(addSuccessful) Then
                        Throw New Exception(m_refMsg.GetMessage("msg error unable to add suggested result"))
                    End If
                Catch ex As Exception
                    Utilities.ShowError(ex.Message)
                End Try
            Next
            ' made it this far, we're successful!
            binAddSuccessful = True
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try

        If binAddSuccessful Then
            ' take the user back to the list of all suggested results
            Response.Redirect("suggestedresults.aspx?action=ViewSuggestedResults&LangType=" & ContentLanguage)
        End If

    End Sub
#End Region

#Region "Sub: DeleteSuggestedResultsSets(termID)"
    ''' <summary>
    ''' This subroutine accepts an integer indicating the ID for the term to be deleted as specified by the user, and then outputs the current Synonym Sets stored in the CMS to the workarea.  The Keywords associated with each Synonym Set will be truncated in the output so that only one line of terms will be displayed.  A "title" attribute provides the entire list of terms on mouseover.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteSuggestedResultSets(ByVal termID As Guid)
        Dim api As New Ektron.Cms.CommonApi()
        Dim suggestedResultData As New List(Of SuggestedResultData)
        Dim idList As New List(Of Guid)
        Dim deleteSuccessful As Boolean = False

        Try
            Dim suggetsedResultObject As New Ektron.Cms.Content.EkSuggestedResults(api.RequestInformationRef)
            ' get the suggested results to delete
            suggestedResultData = suggetsedResultObject.GetSuggestedResultSetForTerms(termID, ContentLanguage)
            For Each SR As SuggestedResultData In suggestedResultData
                idList.Add(SR.ID)
            Next
            deleteSuccessful = suggetsedResultObject.DeleteSugestedResultSet(idList)

            If Not deleteSuccessful Then
                Throw New Exception("Delete unsuccessful.")
            End If

        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
    End Sub
#End Region

#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' initialize necessary variables
        m_refMsg = (New CommonApi).EkMsgRef
        m_refMsg = m_refSiteApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        ContentLanguage = m_refSiteApi.ContentLanguage
        PageAction = Request.QueryString("action").ToLower

        ' register CSS
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss)
        Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE)

        ' populate any text variables as needed
        lblCMSContent.Text = m_refMsg.GetMessage("generic cms Content")

        ' get the term info if needed
        Dim termData As New Ektron.Cms.Common.TermsData
        Dim termDataObject As New Ektron.Cms.Content.Terms(api.RequestInformationRef)
        If Trim(Request.QueryString("termID")).Length > 0 Then
            If Not (Trim(Request.QueryString("termID").ToString()) = Guid.Empty.ToString()) Then
                inputTermID = New Guid(Trim(Request.QueryString("termID")))
                ' instantiate the data class for this Synonym Set
                termData = termDataObject.GetTermById(inputTermID, ContentLanguage)
                ViewToolBar(termData.Name)
            End If
        Else
            ViewToolBar()
        End If
        If (Not (Request.QueryString("LangType") Is Nothing)) Then
            If (Request.QueryString("LangType") <> "") Then
                ContentLanguage = Convert.ToInt32(Request.QueryString("LangType"))
                m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage)
            Else
                If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                    ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
                End If
            End If
        Else
            If m_refContentApi.GetCookieValue("LastValidLanguageID") <> "" Then
                ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"))
            End If
        End If
        thisContentLanguage = ContentLanguage.ToString()
        add_editViewToolBar()
        ' get list of Synonym Sets
        Dim synonymSetData As New List(Of SynonymsNotUsed)
        Dim suggestedResultDataObj As New Content.EkSuggestedResults(api.RequestInformationRef)
        synonymSetData = suggestedResultDataObj.GetSynonymsNotUsed(ContentLanguage)
        ' if we're in edit mode, be sure to list the current Synonym Set being editted as an option
        If (Not Page.IsPostBack) And PageAction = "editsuggestedresult" And termData.Type = Ektron.Cms.Common.TermType.SetTerm Then
            Dim thisSynonymSet As New SynonymsNotUsed
            thisSynonymSet.ID = termData.TermID
            thisSynonymSet.Name = termData.Name
            synonymSetData.Insert(0, thisSynonymSet)
        End If
        Dim synonymComparer As New SynonymsNotUsedComparer
        synonymSetData.Sort(synonymComparer)

        Dim selectSynonyms As New System.Text.StringBuilder
        If synonymSetData.Count = 0 Then
            ' there are no Synonym Sets for this ContentLanguage
            selectSynonyms.Append("<option value=""-1"">" & m_refMsg.GetMessage("lbl no synonym sets available") & "</option>" & vbCrLf)
        Else
            selectSynonyms.Append("<option value=""-1"">" & m_refMsg.GetMessage("generic please select") & "</option>" & vbCrLf)
            For Each synonymSet As Ektron.Cms.SynonymsNotUsed In synonymSetData
                selectSynonyms.Append("<option value=""" & synonymSet.ID.ToString & """>" & Server.HtmlEncode(synonymSet.Name) & "</option>" & vbCrLf)
            Next
        End If
        selectSynonymSet.Text = selectSynonyms.ToString

        ' figure out what mode we're in
        PageMode = Request.Form("addsuggestedresult$submitMode")
        If (Page.IsPostBack) And PageMode = "0" And PageAction = "addsuggestedresult" Then
            ' if we've made it this far, add the NEW suggested results
            ProcessAddSuggestedResult()

        ElseIf (Page.IsPostBack) And PageMode = "0" And PageAction = "editsuggestedresult" Then
            ' if we've made it this far, clear the previous suggested results for this term
            DeleteSuggestedResultSets(termData.TermID)
            ' now add the Suggested Results for the term
            ProcessAddSuggestedResult()
        ElseIf (Not Page.IsPostBack) And PageAction = "editsuggestedresult" Then
            ' if we're editing an existing set of Suggested Results,
            ' let's get them and output them to the array
            Dim suggestedResultList As New List(Of SuggestedResultData)
            Dim jsBuilder As New System.Text.StringBuilder
            ' get the results for this term
            suggestedResultList = suggestedResultDataObj.GetSuggestedResultSetForTerms(termData.TermID, ContentLanguage)

            For Each SR As SuggestedResultData In suggestedResultList
                jsBuilder.Append("var existingSuggestedResultObject = new suggestedResultObject(")
                jsBuilder.Append("'" & SR.ID.ToString.Replace("'", "\'") & "',")
                jsBuilder.Append("'" & SR.Title.ToString.Replace("'", "\'") & "',")
                jsBuilder.Append("'" & SR.QuickLink.ToString.Replace("'", "\'") & "',")
                jsBuilder.Append("'" & SR.Summary.ToString.Replace("'", "\'").Replace(Chr(13), " ").Replace(Chr(10), " ") & "',")
                jsBuilder.Append(SR.ContentID.ToString & ",")
                jsBuilder.Append(SR.SearchOrder.ToString)
                jsBuilder.Append(");" & vbCrLf)
                jsBuilder.Append("arrSuggestedResults.push(existingSuggestedResultObject);" & vbCrLf & vbCrLf)
            Next
            ' we need some JS statements to populate the drop downs and text boxes properly
            If termData.Type = Ektron.Cms.Common.TermType.SingleTerm Then
                jsBuilder.Append("document.getElementById('addsuggestedresult_term').value = '")
                jsBuilder.Append(termData.Name.ToString.Replace("'", "\'"))
                jsBuilder.Append("';" & vbCrLf)

                jsBuilder.Append("document.getElementById('termType').selectedIndex=0;'")
                jsBuilder.Append(termData.Name.ToString.Replace("'", "\'"))
                jsBuilder.Append("';" & vbCrLf)
            Else
                jsBuilder.Append("document.getElementById('termType').selectedIndex=1;'")
                jsBuilder.Append(termData.Name.ToString.Replace("'", "\'"))
                jsBuilder.Append("';" & vbCrLf)

                jsBuilder.Append("setSynonymDropDown('" & termData.TermID.ToString & "');" & vbCrLf)
            End If
            javaScriptSRObjects.Text = jsBuilder.ToString
        End If

		With HtmlEditor1
			.AllowFonts = True
		End With

    End Sub
#End Region

#Region "SUB: Toolbar"
    Protected Sub ViewToolBar(Optional ByRef termName As String = "")
        Dim result As New System.Text.StringBuilder
        ' check the Page Action and set the Title Bar Text appropriately
        If PageAction = "addsuggestedresult" Then
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("msg add suggested results"))
        Else
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("msg edit suggested results") & " """ & termName & """")
        End If

        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "javascript:checkAddSuggestedResults(document.Form1);", m_refMsg.GetMessage("alt save button text suggested result"), m_refMsg.GetMessage("btn save suggested result"), ""))
        If PageAction = "addsuggestedresult" Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "suggestedresults.aspx?action=ViewSuggestedResults&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "suggestedresults.aspx?action=ViewSuggestedResult&LangType=" & ContentLanguage & "&termID=" & inputTermID.ToString, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        If PageAction = "addsuggestedresult" Then
            result.Append(m_refStyle.GetHelpButton("addsuggestedresults"))
        Else
            result.Append(m_refStyle.GetHelpButton("editsuggestedresults"))
        End If
        result.Append("</td>")
        result.Append("</tr></table>")

        ' output the result string to the htmToolBar
        htmToolBar.InnerHtml = result.ToString
    End Sub
#End Region

#Region "SUB: Add/Edit Toolbar"
    Protected Sub add_editViewToolBar()
        Dim result As New System.Text.StringBuilder
        ' check the Page Action and set the Title Bar Text appropriately
        If PageAction = "addsuggestedresult" Then
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("msg add new suggested result"))
        Else
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("msg edit suggested result"))
        End If

        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "javascript:transferAddEdit();", m_refMsg.GetMessage("alt update button text single suggested result"), m_refMsg.GetMessage("btn add suggested result"), ""))
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "javascript:cancelAddEdit();", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))

        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("addeditsuggestedresult"))
        result.Append("</td>")
        result.Append("</tr></table>")

        ' output the result string to the htmToolBar
        divToolBar.InnerHtml = result.ToString
    End Sub
#End Region


End Class
