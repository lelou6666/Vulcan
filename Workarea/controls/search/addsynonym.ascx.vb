Imports System.Data
Imports Ektron.Cms
Imports System.Collections.Generic

Partial Class addsynonym
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
    Protected api As New CommonApi()
    Protected PageMode As String = ""
    Protected PageAction As String = ""
    Protected inputSynonymID As Guid = Nothing

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

#Region "FUNCTION: CheckTermNameExists(name1, name2) - Used in other functions to see if a provided name exists already"
    Function CheckTermNameExists(ByRef one As Object, ByRef two As Object) As Boolean
        If LCase(one.Name) = LCase(two.Name) Then
            Return True
        End If
        Return False
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

#Region "FUNCTION: CheckTermExists(termsData, termsDataObject) - checks to see if a provided TERM already exists in the Terms dB table"
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
                Throw New Exception("<strong>" & m_refMsg.GetMessage("msg error improper type specification") & "</strong>:" & m_refMsg.GetMessage("msg error term validation rule"))
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
                If isUpdate Then
                    If term.TermID <> termsData.TermID Then
                        If CheckTermNameExists(compareTermsData, term) Then
                            Return term
                        End If
                    End If
                Else
                    If CheckTermNameExists(compareTermsData, term) Then
                        Return term
                    End If
                End If
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
    Public Function addNewTerm(ByRef termsData As Ektron.Cms.Common.TermsData, ByRef termsObject As Ektron.Cms.Content.Terms) As Guid
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
                Throw New Exception(Server.UrlEncode(String.Format(m_refMsg.GetMessage("msg error improper type specification"), "<em>" & termsData.Terms & "</em>", "<em>" & existingTermID.ToString & "</em>")))
            End If

            ' if we made it this far, add the new term
            Dim result As Guid = termsObject.AddTerms(termsData)
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

#Region "SUB: ProcessAddSynonymSubmit"
    Protected Sub ProcessAddSynonymSubmit()
        ' clear the Show Duplicates text
        showDuplicates.Text = ""
        ' Create new TermsData instance
        Dim synonymSetTermData As New Ektron.Cms.Common.TermsData
        Dim synonymSetData As New Ektron.Cms.SynonymData
        Dim synonymSetTermsObject As New Ektron.Cms.Content.Terms(api.RequestInformationRef)
        Dim synonymSetObject As New Ektron.Cms.Content.Synonyms(api.RequestInformationRef)
        Dim addSynonymSetSuccess As Guid = Nothing

        Try
            ' assign values to the required properties
            synonymSetTermData.Name = Trim(Request.Form("addsynonym$synonymName"))
            synonymSetTermData.Terms = Trim(Request.Form("addsynonym$synonymTerms")).Trim(";")
            synonymSetTermData.DateModified = Date.Now()
            synonymSetTermData.Type = 1
            synonymSetTermData.TermLanguage = ContentLanguage
            CheckName(synonymSetTermData.Name)
            CheckTerms(synonymSetTermData.Terms, synonymSetTermData.Type)

            ' first check to see if this term already exists in the system
            Dim termID As Guid = Nothing
            If Not IsNothing(CheckTermExists(synonymSetTermData, synonymSetTermsObject, 0)) Then
                termID = CheckTermExists(synonymSetTermData, synonymSetTermsObject, 0).TermID
            End If

            If Not IsNothing(termID) AndAlso Not (termID.ToString().Trim() = Guid.Empty.ToString()) Then
                ' use the existing term's ID value
                synonymSetTermData.TermID = termID
            Else
                ' the term doesn't exist already, so let's add it to terms dB table
                termID = addNewTerm(synonymSetTermData, synonymSetTermsObject)
                synonymSetTermData.TermID = termID
            End If

            ' assign the values to the synonymSetData object
            synonymSetData.Name = synonymSetTermData.Name.ToString
            synonymSetData.TermName = synonymSetTermData.Name.ToString
            synonymSetData.TermID = synonymSetTermData.TermID
            synonymSetData.DateModified = Date.Now()
            synonymSetData.LanguageID = ContentLanguage

            ' check to make sure that the Synonym Set doesn't already exist
            Dim synonymSetExists As New List(Of Ektron.Cms.SynonymData)
            If Not IsNothing(synonymSetObject.GetSynonymForTerm(synonymSetData.TermID)) Then
                synonymSetExists = synonymSetObject.GetSynonymForTerm(synonymSetData.TermID)
            End If
            If Integer.Parse(synonymSetExists.Count) > 0 Then
                Throw New Exception(Server.UrlEncode(String.Format(m_refMsg.GetMessage("msg error duplicate synonym set") & "<br />", "<em>" & synonymSetExists(0).TermsKeywords & "</em>", "<em><a href=""search/synonyms.aspx?action=ViewSynonym&LangType=" & synonymSetExists(0).LanguageID & "&id=" & synonymSetExists(0).ID.ToString & """>" & synonymSetExists(0).Name & "</a></em>")))
            End If

            addSynonymSetSuccess = synonymSetObject.AddSynonyms(synonymSetData)
            If IsNothing(addSynonymSetSuccess) Then
                Throw New Exception(m_refMsg.GetMessage("msg error add synonym set unsuccessful"))
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
        If Not IsNothing(addSynonymSetSuccess) Then Response.Redirect("synonyms.aspx?action=ViewSynonyms&LangType=" & ContentLanguage)
    End Sub

#End Region

#Region "SUB: ProcessEditSynonymSet"
    Protected Sub ProcessEditSynonymSet(ByRef synonymSetData As Ektron.Cms.SynonymData)
        ' clear the Show Duplicates text
        showDuplicates.Text = ""
        ' Create new TermsData instance
        Dim synonymSetTermData As New Ektron.Cms.Common.TermsData
        Dim synonymSetTermsObject As New Ektron.Cms.Content.Terms(api.RequestInformationRef)
        Dim synonymSetObject As New Ektron.Cms.Content.Synonyms(api.RequestInformationRef)
        Dim editSynonymSetSuccess As Guid = Nothing

        Try
            ' get the existing data for the Synonym Set
            If IsNothing(synonymSetData) Then
                Throw New Exception(m_refMsg.GetMessage("msg error synonym set not found"))
            End If

            ' set up the TermData object with the correct information
            synonymSetTermData.Type = 1
            If Trim(Request.Form("addsynonym$synonymName")).Length > 0 Then
                synonymSetTermData.Name = Trim(Request.Form("addsynonym$synonymName"))
                synonymSetData.Name = Trim(Request.Form("addsynonym$synonymName"))
            Else
                synonymSetTermData.Name = synonymSetData.Name.ToString()
            End If

            If Not IsNothing(Request.Form(termID.UniqueID.ToString)) And Request.Form(termID.UniqueID.ToString) <> "" Then
                synonymSetTermData.TermID = New Guid(Request.Form(termID.UniqueID.ToString))
            End If

            CheckName(synonymSetTermData.Name)
            If Trim(Request.Form("addsynonym$synonymTerms")).Length > 0 Then
                synonymSetTermData.Terms = Trim(Request.Form("addsynonym$synonymTerms")).Trim(";")
                CheckTerms(synonymSetTermData.Terms, synonymSetTermData.Type)

                ' check to see if this term already exists in the system
                Dim term As Ektron.Cms.Common.TermsData = CheckTermExists(synonymSetTermData, synonymSetTermsObject, 1)

                If Not IsNothing(term) Then
                    ' use the existing term's ID value
                    synonymSetTermData = term
                Else
                    synonymSetTermData.TermID = synonymSetData.TermID
                    ' the term doesn't already exist already, so let's update it
                    synonymSetTermData.TermID = synonymSetTermsObject.UpdateTerms(synonymSetTermData)
                End If
            Else
                synonymSetTermData.Terms = synonymSetData.TermsKeywords.ToString()
            End If

            ' assign the values to the synonymSetData object
            synonymSetData.TermName = synonymSetTermData.Name.ToString
            synonymSetData.TermID = synonymSetTermData.TermID
            synonymSetData.TermsKeywords = Trim(Request.Form("addsynonym$synonymTerms")).Trim(";")
            synonymSetData.DateModified = Date.Now()

            editSynonymSetSuccess = synonymSetObject.UpdateSynonyms(synonymSetData)
            If IsNothing(editSynonymSetSuccess) Then
                Throw New Exception(String.Format(m_refMsg.GetMessage("msg error synonym set update failed"), "<em>" & synonymSetData.Name.ToString & "</em>"))
            End If
        Catch ex As Exception
            Utilities.ShowError(ex.Message)
        End Try
        If Not IsNothing(editSynonymSetSuccess) Then Response.Redirect("synonyms.aspx?action=ViewSynonym&LangType=" & synonymSetData.LanguageID & "&id=" & synonymSetData.ID.ToString)
    End Sub
#End Region

#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        m_refMsg = (New CommonApi).EkMsgRef
        m_refMsg = m_refSiteApi.EkMsgRef
        AppImgPath = m_refSiteApi.AppImgPath
        ContentLanguage = m_refSiteApi.ContentLanguage
        PageAction = Request.QueryString("action").ToLower
        RegisterResources()

        Dim synonymSetData As New Ektron.Cms.SynonymData
        Dim synonymSetObject As New Ektron.Cms.Content.Synonyms(api.RequestInformationRef)
        If Trim(Request.QueryString("id")).Length > 0 Then
            If Not Trim(Request.QueryString("id").ToString()) = Guid.Empty.ToString() Then
                inputSynonymID = New Guid(Trim(Request.QueryString("id")))
                ' intantiate the data class for this Synonym Set
                synonymSetData = synonymSetObject.GetSynonymForId(inputSynonymID, ContentLanguage)
                ViewToolBar(synonymSetData.Name)
                synonymName.Text = synonymSetData.Name
                synonymTerms.Text = synonymSetData.TermsKeywords
                termID.Value = synonymSetData.TermID.ToString()
            End If
        Else
            ViewToolBar()
        End If

        checkDuplicates.Text = m_refMsg.GetMessage("btn check for duplicates")
        checkDuplicates.ToolTip = m_refMsg.GetMessage("btn title check for duplicates")
        PageMode = Request.Form("addsynonym$submitMode")

        If (Page.IsPostBack) And PageMode = "0" And PageAction = "addsynonym" Then
            ProcessAddSynonymSubmit()
        ElseIf (Page.IsPostBack) And PageMode = "0" And PageAction = "editsynonym" Then
            ProcessEditSynonymSet(synonymSetData)
        ElseIf (Page.IsPostBack) Then
            showDuplicates.Text = CheckForDuplicates()
        End If
    End Sub
#End Region

#Region "SUB: Toolbar"
    Protected Sub ViewToolBar(Optional ByRef synonymName As String = "")
        Dim result As New System.Text.StringBuilder
        ' check the Page Action and set the Title Bar Text appropriately
        If PageAction = "addsynonym" Then
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("msg add synonym set"))
        Else
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(String.Format(m_refMsg.GetMessage("msg edit synonym set"), synonymName))
        End If

        result.Append("<table><tr>")
        result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/save.png", "javascript:document.Form1.addsynonym_submitMode.value=0;checkAddSynonyms(document.Form1);", m_refMsg.GetMessage("alt save synonym set"), m_refMsg.GetMessage("btn save synonym"), ""))
        If PageAction = "addsynonym" Then
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "synonyms.aspx?action=ViewSynonyms&LangType=" & ContentLanguage, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        Else
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "synonyms.aspx?action=ViewSynonym&LangType=" & ContentLanguage & "&id=" & inputSynonymID.ToString & "&#38;bck=vs", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
        End If
        result.Append("<td>")
        result.Append(m_refStyle.GetHelpButton("addsynonymset"))
        result.Append("</td>")
        result.Append("</tr></table>")

        ' output the result string to the htmToolBar
        htmToolBar.InnerHtml = result.ToString
    End Sub
#End Region

#Region "FUNCTION: Check Duplicates"
    Protected Function CheckForDuplicates() As String
        ' Create new TermsData instance
        Dim synonymSetData As List(Of Ektron.Cms.SynonymData)
        Dim synonymSetObject As New Ektron.Cms.Content.Synonyms(api.RequestInformationRef)
        Dim currentTerms As String = Trim(Request.Form("addsynonym$synonymTerms")).Trim(";")
        Dim arrTerms As Array = Split(currentTerms.ToString, ";")

        Try
            synonymSetData = synonymSetObject.GetSynonyms(ContentLanguage, Ektron.Cms.Common.EkEnumeration.SynonymOrderBy.name)
            If IsNothing(synonymSetData) Then
                Throw New Exception(m_refMsg.GetMessage("msg error synonym set equals nothing"))
            End If
            Dim termOutput As String = ""
            Dim tempTermOutput As String = ""
            Dim arrExistingSynonymSetTerms As Array
            For Each term As String In arrTerms
                ' check to see if any other Synonym Set contains the term
                For Each synonymSet As Ektron.Cms.SynonymData In synonymSetData
                    ' make sure that this isn't the synonym set we're currently working with
                    If inputSynonymID <> synonymSet.ID Then
                        ' each Synonym Set needs it's terms split into an array so we can check every term
                        arrExistingSynonymSetTerms = Split(synonymSet.TermsKeywords, ";")
                        For Each existingTerm As String In arrExistingSynonymSetTerms
                            ' if the terms match, add it to the list of matches for this term
                            If LCase(term.ToString.Trim) = LCase(existingTerm.Trim) Then
                                If tempTermOutput.Length = 0 Then
                                    ' first match found for this term
                                    tempTermOutput = "<li>" & String.Format(m_refMsg.GetMessage("msg synonym set is also contained in"), term.ToString.Trim, "<a href=""synonyms.aspx?action=ViewSynonym&LangType=" & synonymSet.LanguageID & "&id=" & synonymSet.ID.ToString & """ onclick=""return VerifyFollowDupe();"">" & synonymSet.Name & "</a>")
                                Else
                                    tempTermOutput += ", <a href=""synonyms.aspx?action=ViewSynonym&LangType=" & synonymSet.LanguageID & "&id=" & synonymSet.ID.ToString & """  onclick=""return VerifyFollowDupe();"">" & synonymSet.Name & "</a>"
                                End If
                            End If
                        Next
                    End If
                Next
                ' if we've added any matches to the string, close the paragraph tag
                If tempTermOutput.Length > 0 Then tempTermOutput += "</li>"
                ' add the string to the final output string
                termOutput += tempTermOutput.Trim
                ' clear the temporary string for the next loop
                tempTermOutput = ""
            Next
            ' set up a string builder
            Dim result As New System.Text.StringBuilder
            ' build the final output indicating if any duplicates were found or not
            result.Append("<ul class=""duplicates"">")
            If termOutput.Trim.Length > 0 Then
                result.Append(termOutput)
            Else
                result.Append("<li>" & m_refMsg.GetMessage("msg no duplicates found") & "</li>")
            End If
            result.Append("</ul>")
            Return result.ToString

        Catch ex As Exception
            Utilities.ShowError(ex.Message)
            Return Nothing
        End Try

    End Function
#End Region

    Protected Sub RegisterResources()
        ' register JS
        Ektron.Cms.API.JS.RegisterJS(Me, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS)
    End Sub
End Class