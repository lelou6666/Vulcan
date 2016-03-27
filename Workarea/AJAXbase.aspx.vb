Imports System.Text
Imports Ektron.Cms
Imports Ektron.Cms.Content
Imports Microsoft.Security.Application

Partial Class AJAXbase
    Inherits System.Web.UI.Page

    Private m_sAction As String = ""
    Private m_sbResponse As New StringBuilder

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If Request.QueryString("action") <> "" Then
                m_sAction = AntiXss.HtmlEncode(Request.QueryString("action").ToLower())
            End If

            If (m_sAction.ToLower() = "getcontenttemplates") Then
                'we return pre formtted html for get content template
            Else
                Response.ContentType = "text/xml"
                Response.CacheControl = "no-cache"
                Response.AddHeader("Pragma", "no-cache")
                Response.Expires = -1

                m_sbResponse.Append("<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>").Append(Environment.NewLine)
                m_sbResponse.Append("<response>").Append(Environment.NewLine)
            End If

            Select Case Me.m_sAction
                Case "existinguser"
                    m_sbResponse.Append(Handler_ExistingUser)
                Case "existingfolder"
                    m_sbResponse.Append(Handler_ExistingFolder)
                Case "existingrule"
                    m_sbResponse.Append(Handler_ExistingRule)
                Case "existingruleset"
                    m_sbResponse.Append(Handler_ExistingRuleset)
                Case "existinguserrank"
                    m_sbResponse.Append(Handler_ExistingUserRank)
                Case "addeditcontentrating"
                    m_sbResponse.Append(Handler_AddEditContentRating)
                Case "addeditcontentflag"
                    m_sbResponse.Append(Handler_AddEditContentFlag)
                Case "addpersonalfolder"
                    m_sbResponse.Append(Handler_AddPersonalFolder)
                Case "removeitems"
                    m_sbResponse.Append(Handler_RemoveItems)
                Case "addfavorite"
                    m_sbResponse.Append(Handler_AddFavorite)
                Case "addfriend"
                    m_sbResponse.Append(Handler_AddFriend)
                Case "addto"
                    m_sbResponse.Append(Handler_AddTo)
                Case "shareuserblog"
                    m_sbResponse.Append(Hander_ShareUserBlog)
                Case "getcontenttemplates"
                    m_sbResponse.Append(Handler_GetContentTemplates)
            End Select


            If (m_sAction.ToLower() <> "getcontenttemplates") Then
                m_sbResponse.Append("</response>").Append(Environment.NewLine)
            End If

            Response.Write(m_sbResponse.ToString())
        Catch ex As Exception

        End Try
    End Sub

#Region "Handlers"
    Public Function Handler_GetContentTemplates() As String
        Dim id As Long = 0
        Dim sbRet As StringBuilder = New StringBuilder()
        Dim i As Integer = 0
        Dim m_aliasAPI As New UrlAliasing.UrlAliasManualApi
        Dim alias_templates As System.Collections.Generic.List(Of LibraryData)
        Dim apiContent As New ContentAPI
        Dim workareaDir As String = String.Empty
        Long.TryParse(Request.QueryString("id"), id)
        alias_templates = m_aliasAPI.GetTemplateList(id)
        workareaDir = apiContent.RequestInformationRef.WorkAreaDir

        For i = 0 To alias_templates.Count - 1
            If (alias_templates(i).FileName.ToLower().IndexOf("downloadasset.aspx?") <> -1) Then
                sbRet.Append("<option value=""").Append(alias_templates(i).Id).Append(""">").Append(workareaDir & alias_templates(i).FileName).Append("</option>")
            Else
                sbRet.Append("<option value=""").Append(alias_templates(i).Id).Append(""">").Append(alias_templates(i).FileName).Append("</option>")
            End If
        Next
        Return sbRet.ToString()
    End Function
    Public Function Hander_ShareUserBlog() As String
        Dim resp As StringBuilder = New StringBuilder("<method>AJAX_ShareUserBlog</method>")
        Dim apiContent As New ContentAPI
        Dim BlogId As Long
        Dim shareOption As Integer
        Try
            If (Request.QueryString("BlogId") <> "") Then
                BlogId = Convert.ToInt64(Request.QueryString("BlogId"))
                shareOption = Convert.ToInt32(Request.QueryString("shareoption"))
                Dim list As System.Collections.Generic.List(Of String) = New System.Collections.Generic.List(Of String)
                list.Add(BlogId.ToString() & "," & shareOption.ToString())
                apiContent.SetWorkSpaceShare(apiContent.RequestInformationRef.UserId, Ektron.Cms.Common.EkEnumeration.WorkSpace.User, list)
            End If
        Catch ex As Exception
            resp.Append("<result>-1</result>")
            resp.Append("<returnmsg>" & ex.Message.ToString() & "</returnmsg>")
        End Try
        Return resp.ToString()
    End Function

    Public Function Handler_AddTo() As String
        Dim sbRet As New StringBuilder
        Dim apiContent As New ContentAPI
        Dim apiCommunityGroup As New Community.CommunityGroupAPI
        Dim apiFavorites As New Community.FavoritesAPI
        Dim apiFriends As New Community.FriendsAPI
        Dim iObjID As Long = 0
        Dim sbObjType As Common.EkEnumeration.CMSSocialBarTypes = Common.EkEnumeration.CMSSocialBarTypes.Content
        Dim sMode As String = ""
        Dim sMsg As String = ""
        Dim sKey As String = ""
        Dim sResult As String = ""
        Dim iIdx As Integer = 0
        Dim bIsMine As Boolean = False
        Dim bIsMyLinkID As Long = 0
        Dim sRetMsg As String = ""
        Dim bAuth As Boolean = False
        Dim iLang As Integer = 0
        Dim title As String = ""
        Dim link As String = ""
        Try
            iObjID = Convert.ToInt64(Request.QueryString("oid"))
            sbObjType = AntiXss.HtmlEncode(Request.QueryString("otype"))
            sMode = AntiXss.HtmlEncode(Request.QueryString("mode"))
            sKey = AntiXss.HtmlEncode(Request.QueryString("key"))
            iIdx = Convert.ToInt32(Request.QueryString("idx"))
            title = AntiXss.HtmlEncode(Request.QueryString("title"))
            link = AntiXss.HtmlEncode(Request.QueryString("link"))

            If iObjID > 0 Then
                sbRet.Append("  <method>AJAX_AddTo</method>").Append(Environment.NewLine)
            ElseIf iObjID = 0 Then
                sbRet.Append("  <method>AJAX_AddLinkTo</method>").Append(Environment.NewLine)
            End If

            bAuth = (apiContent.LoadPermissions(0, "users")).IsLoggedIn
            If bAuth Then
                Select Case sbObjType
                    Case Common.EkEnumeration.CMSSocialBarTypes.CommunityGroup
                        Dim mMemberStatus As Common.EkEnumeration.GroupMemberStatus = Common.EkEnumeration.GroupMemberStatus.NotInGroup
                        mMemberStatus = apiCommunityGroup.GetGroupMemberStatus(iObjID, apiContent.UserId)
                        Select Case mMemberStatus
                            Case Common.EkEnumeration.GroupMemberStatus.Approved
                                If sMode = "remove" Then
                                    apiCommunityGroup.RemoveUserFromCommunityGroup(iObjID, apiContent.UserId)
                                    Ektron.Cms.Common.Cache.ApplicationCache.Invalidate("GroupAccess_" & iObjID.ToString() & "_" & apiContent.UserId.ToString())
                                    sMsg = apiContent.EkMsgRef.GetMessage("lbl left group")
                                    sRetMsg = apiContent.EkMsgRef.GetMessage("lbl join group")
                                    sResult = "1"
                                ElseIf sMode = "add" Then
                                    sMsg = apiContent.EkMsgRef.GetMessage("lbl already in group")
                                    sRetMsg = apiContent.EkMsgRef.GetMessage("lbl leave group")
                                    sResult = "-1"
                                End If
                            Case Common.EkEnumeration.GroupMemberStatus.Leader
                                sMsg = apiContent.EkMsgRef.GetMessage("lbl leader of group")
                                sRetMsg = apiContent.EkMsgRef.GetMessage("lbl leader of group")
                                sResult = "-1"
                            Case Common.EkEnumeration.GroupMemberStatus.NotInGroup
                                If sMode = "remove" Then
                                    sMsg = apiContent.EkMsgRef.GetMessage("lbl not in group")
                                    sRetMsg = apiContent.EkMsgRef.GetMessage("lbl join group")
                                    sResult = "-1"
                                ElseIf sMode = "add" Then
                                    apiCommunityGroup.AddUserToCommunityGroup(iObjID, apiContent.UserId)
                                    Ektron.Cms.Common.Cache.ApplicationCache.Invalidate("GroupAccess_" & iObjID.ToString() & "_" & apiContent.UserId.ToString())
                                    mMemberStatus = apiCommunityGroup.GetGroupMemberStatus(iObjID, apiContent.UserId)
                                    If mMemberStatus = Common.EkEnumeration.GroupMemberStatus.Pending Then
                                        sRetMsg = apiContent.EkMsgRef.GetMessage("lbl cgroup cancel join req")
                                        sMsg = apiContent.EkMsgRef.GetMessage("lbl leave group")
                                        sResult = "0"
                                    Else
                                        sRetMsg = apiContent.EkMsgRef.GetMessage("lbl leave group")
                                        sMsg = apiContent.EkMsgRef.GetMessage("lbl joined group")
                                        sResult = "0"
                                    End If
                                End If
                            Case Common.EkEnumeration.GroupMemberStatus.Pending
                                If sMode = "remove" Then
                                    apiCommunityGroup.CancelJoinRequestForCommunityGroup(iObjID, apiContent.UserId)
                                    Ektron.Cms.Common.Cache.ApplicationCache.Invalidate("GroupAccess_" & iObjID.ToString() & "_" & apiContent.UserId.ToString())
                                    sMsg = apiContent.EkMsgRef.GetMessage("lbl cancel group join")
                                    sRetMsg = apiContent.EkMsgRef.GetMessage("lbl join group")
                                    sResult = "1"
                                ElseIf sMode = "add" Then
                                    sMsg = apiContent.EkMsgRef.GetMessage("lbl requested join group")
                                    sRetMsg = apiContent.EkMsgRef.GetMessage("lbl cgroup cancel join req")
                                    sResult = "-1"
                                End If
                            Case Common.EkEnumeration.GroupMemberStatus.InvitedMember
                                apiCommunityGroup.AddUserToCommunityGroup(iObjID, apiContent.UserId)
                                Ektron.Cms.Common.Cache.ApplicationCache.Invalidate("GroupAccess_" & iObjID.ToString() & "_" & apiContent.UserId.ToString())
                                mMemberStatus = apiCommunityGroup.GetGroupMemberStatus(iObjID, apiContent.UserId)
                                If mMemberStatus = Common.EkEnumeration.GroupMemberStatus.Pending Then
                                    sRetMsg = apiContent.EkMsgRef.GetMessage("lbl cgroup cancel join req")
                                    sMsg = apiContent.EkMsgRef.GetMessage("lbl leave group")
                                    sResult = "0"
                                Else
                                    sRetMsg = apiContent.EkMsgRef.GetMessage("lbl leave group")
                                    sMsg = apiContent.EkMsgRef.GetMessage("lbl cgrp accept inv")
                                    sResult = "0"
                                End If
                        End Select
                    Case Common.EkEnumeration.CMSSocialBarTypes.Content
                        If Request.QueryString("lang") <> "" AndAlso IsNumeric(Request.QueryString("lang")) Then
                            iLang = Request.QueryString("lang")
                        End If
                        If iLang > 0 Then
                            apiFavorites.ContentLanguage = iLang
                        End If
                        If iObjID > 0 Then
                            bIsMine = apiFavorites.IsMyContentFavorite(iObjID)
                        Else
                            bIsMyLinkID = apiFavorites.GetFavoriteId(title, link, apiContent.UserId)
                        End If


                        If sMode = "remove" And bIsMine Then
                            apiFavorites.DeleteMyContentFavorite(iObjID)
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl no longer fav")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add fav")
                            sResult = "1"
                        ElseIf sMode = "remove" And Not bIsMine Then
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl not fav")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add fav")
                            sResult = "-1"
                        ElseIf sMode = "add" And bIsMine Then
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl already fav")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove fav")
                            sResult = "-1"
                        ElseIf sMode = "add" And Not bIsMine Then
                            apiFavorites.AddContentFavorite(iObjID)
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl now fav")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove fav")
                            sResult = "0"
                        ElseIf sMode = "addlink" And bIsMyLinkID = 0 Then
                            If (title <> "") Then
                                apiFavorites.AddFavoriteLink(apiContent.UserId, 0, iLang, title, link, "")
                            End If
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl now fav")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove fav")
                            sResult = "0"
                        ElseIf sMode = "addlink" And bIsMyLinkID > 0 Then
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl already fav")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove fav")
                            sResult = "-1"
                        ElseIf sMode = "removelink" And bIsMyLinkID > 0 Then
                            apiFavorites.DeleteFavoriteLink(bIsMyLinkID)
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl no longer fav")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add fav")
                            sResult = "1"
                        ElseIf sMode = "removelink" And bIsMyLinkID = 0 Then
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl not fav")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add fav")
                            sResult = "-1"
                        End If
                    Case Common.EkEnumeration.CMSSocialBarTypes.User
                        Dim fFriendStatus As Common.EkEnumeration.FriendStatus = Common.EkEnumeration.FriendStatus.NotFriend
                        ' bIsMine = apiContent.IsMyFriend(iObjID)
                        fFriendStatus = apiFriends.GetFriendStatus(iObjID, apiContent.UserId)
                        If sMode = "remove" And fFriendStatus = Common.EkEnumeration.FriendStatus.Approved Then
                            apiFriends.DeleteMyFriend(iObjID)
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl no longer friend")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add friend")
                            sResult = "1"
                        ElseIf sMode = "remove" And fFriendStatus = Common.EkEnumeration.FriendStatus.NotFriend Then
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl not friend")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add friend")
                            sResult = "-1"
                        ElseIf sMode = "add" And fFriendStatus = Common.EkEnumeration.FriendStatus.Approved Then
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl already friend")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove friend")
                            sResult = "-1"
                        ElseIf sMode = "add" And fFriendStatus = Common.EkEnumeration.FriendStatus.NotFriend Then
                            apiFriends.AddPendingFriend(iObjID)
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl now friend req")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove friend req")
                            sResult = "0"
                        ElseIf sMode = "remove" And fFriendStatus = Common.EkEnumeration.FriendStatus.Pending Then
                            apiFriends.DeletePendingFriendRequest(iObjID)
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl no longer pending friend")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add friend")
                            sResult = "1"
                        ElseIf sMode = "add" And fFriendStatus = Common.EkEnumeration.FriendStatus.Pending Then
                            apiFriends.AcceptPendingFriend(iObjID)
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl already request friend")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove friend")
                            sResult = "0"
                        ElseIf sMode = "remove" And fFriendStatus = Common.EkEnumeration.FriendStatus.Invited Then
                            apiFriends.DeleteSentFriendRequest(iObjID)
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl no longer pending friend")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl add friend")
                            sResult = "1"
                        ElseIf sMode = "add" And fFriendStatus = Common.EkEnumeration.FriendStatus.Invited Then
                            sMsg = apiContent.EkMsgRef.GetMessage("lbl already request friend")
                            sRetMsg = apiContent.EkMsgRef.GetMessage("lbl remove friend")
                            sResult = "0"
                        End If
                End Select
            Else
                sMsg = apiContent.EkMsgRef.GetMessage("lbl not logged in")
                sRetMsg = apiContent.EkMsgRef.GetMessage("lbl not logged in")
                sResult = "-1"
            End If
            sbRet.Append("  <result>").Append(sResult).Append("</result>").Append(Environment.NewLine)
            sbRet.Append("  <returnmsg>").Append(sMsg).Append("</returnmsg>").Append(Environment.NewLine)
            sbRet.Append("  <oid>").Append(iObjID).Append("</oid>").Append(Environment.NewLine)
            sbRet.Append("  <otype>").Append(sbObjType).Append("</otype>").Append(Environment.NewLine)
            sbRet.Append("  <ilang>").Append(iLang.ToString()).Append("</ilang>").Append(Environment.NewLine)
            sbRet.Append("  <idx>").Append(iIdx).Append("</idx>").Append(Environment.NewLine)
            sbRet.Append("  <retmsg>").Append(sRetMsg).Append("</retmsg>").Append(Environment.NewLine)
            sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine)
            If (iObjID = 0) Then
                sbRet.Append("  <title>").Append(title.ToString()).Append("</title>").Append(Environment.NewLine)
                sbRet.Append("  <link>").Append(link).Append("</link>").Append(Environment.NewLine)
            End If

        Catch ex As Exception
            sbRet.Append("  <method>AJAX_AddTo</method>").Append(Environment.NewLine)
            sbRet.Append("  <result>error</result>").Append(Environment.NewLine)
            sbRet.Append("  <returnmsg>").Append(ex.Message).Append("</returnmsg>").Append(Environment.NewLine)
            sbRet.Append("  <oid>").Append(iObjID).Append("</oid>").Append(Environment.NewLine)
            sbRet.Append("  <otype>").Append(sbObjType).Append("</otype>").Append(Environment.NewLine)
            sbRet.Append("  <ilang>").Append(iLang.ToString()).Append("</ilang>").Append(Environment.NewLine)
            sbRet.Append("  <idx>").Append(iIdx).Append("</idx>").Append(Environment.NewLine)
            sbRet.Append("  <retmsg>").Append(sRetMsg).Append("</retmsg>").Append(Environment.NewLine)
            sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine)
        End Try
        Return sbRet.ToString()
    End Function
    Public Function Handler_AddFavorite() As String
        Dim sbRet As New StringBuilder
        Dim apiContent As New ContentAPI
        Dim apiFavorites As New Community.FavoritesAPI
        Dim iID As Long = 0
        Dim sMode As String = ""
        Dim sKey As String = ""
        Dim sResult As String = ""
        Try
            iID = Convert.ToInt64(Request.QueryString("node"))
            sMode = AntiXss.HtmlEncode(Request.QueryString("mode"))
            sKey = AntiXss.HtmlEncode(Request.QueryString("key"))

            sbRet.Append("  <method>AddtoFavorite</method>").Append(Environment.NewLine)
            If sMode = "check" Then
                If apiFavorites.IsMyContentFavorite(iID) Then
                    sResult = Server.HtmlEncode("Favorite has been added")
                Else
                    sResult = Server.HtmlEncode("<a href=""javascript:pdhdlr('addfav', '" & sKey & "');"">Add to Favorites</a>")
                End If
            ElseIf apiFavorites.IsMyContentFavorite(iID) Then
                sResult = "Already a Favorite"
            Else
                apiFavorites.AddContentFavorite(iID)
                sResult = "Favorite has been added"
            End If
            sbRet.Append("  <result>").Append(sResult).Append("</result>").Append(Environment.NewLine)
            sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine)
        Catch ex As Exception
            sbRet.Append("  <method>AddtoFavorite</method>").Append(Environment.NewLine)
            sbRet.Append("  <result>-1</result>").Append(Environment.NewLine)
            sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine)
        End Try
        Return sbRet.ToString()
    End Function
    Public Function Handler_AddFriend() As String
        Dim sbRet As New StringBuilder
        Dim apiContent As New ContentAPI
        Dim apiFavorites As New Community.FavoritesAPI
        Dim iID As Long = 0
        Dim sMode As String = ""
        Dim sKey As String = ""
        Dim sResult As String = ""
        Try
            iID = Convert.ToInt64(Request.QueryString("node"))
            sMode = AntiXss.HtmlEncode(Request.QueryString("mode"))
            sKey = AntiXss.HtmlEncode(Request.QueryString("key"))

            sbRet.Append("  <method>AddtoFriend</method>").Append(Environment.NewLine)
            If sMode = "check" Then
                If apiFavorites.IsMyContentFavorite(iID) Then
                    sResult = Server.HtmlEncode("Friend has been added")
                Else
                    sResult = Server.HtmlEncode("<a href=""javascript:pdhdlr('addfr', '" & sKey & "');"">Add to Friends</a>")
                End If
            ElseIf apiFavorites.IsMyContentFavorite(iID) Then
                sResult = "Already a Friend"
            Else
                apiFavorites.AddContentFavorite(iID)
                sResult = "Friend has been added"
            End If
            sbRet.Append("  <result>").Append(sResult).Append("</result>").Append(Environment.NewLine)
            sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine)
        Catch ex As Exception
            sbRet.Append("  <method>AddtoFriend</method>").Append(Environment.NewLine)
            sbRet.Append("  <result>-1</result>").Append(Environment.NewLine)
            sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine)
        End Try
        Return sbRet.ToString()
    End Function
    Public Function Handler_RemoveItems() As String
        Dim sbRet As New StringBuilder
        Dim apiContent As New ContentAPI
        Dim sItemList As String = ""
        Dim iRet As Integer = 0
        Dim sKey As String = ""
        Dim iNode As Long = 0
        Try
            sItemList = Request.QueryString("itemlist")
            sKey = AntiXss.HtmlEncode(Request.QueryString("key"))

            Try
                Dim aValues() As String
                aValues = Split(sItemList, ",")
                If aValues IsNot Nothing AndAlso aValues.Length > 0 Then
                    For i As Integer = 0 To (aValues.Length - 1)
                        If aValues(i).ToLower().IndexOf(sKey & "_i") = 0 Then
                            Dim tReq As New TaxonomyRequest
                            Dim aVal() As String = Split(aValues(i), "_")
                            Dim iType As Integer = 0

                            iNode = Request.QueryString("node")
                            tReq.TaxonomyIdList = Mid(aVal(1), 2)
                            iType = aVal(2)
                            Select Case iType
                                Case 7

                                Case Else ' 1 - content
                                    tReq.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content
                            End Select
                            tReq.TaxonomyId = iNode
                            tReq.TaxonomyLanguage = apiContent.ContentLanguage
                            apiContent.RemoveTaxonomyItem(tReq)
                        ElseIf aValues(i).ToLower().IndexOf(sKey & "_f") = 0 Then
                            Dim tReq As New TaxonomyRequest
                            iNode = CLng(Mid(aValues(i), ((sKey & "_f").Length) + 1))
                            tReq.TaxonomyId = iNode
                            tReq.TaxonomyLanguage = apiContent.ContentLanguage
                            If iNode > 0 Then
                                apiContent.DeleteTaxonomy(tReq)
                            End If
                        End If
                    Next
                End If
                iRet = 0
            Catch ex As Exception
                EkException.ThrowException(ex)
            End Try
            sbRet.Append("  <method>RemoveItems</method>").Append(Environment.NewLine)
            sbRet.Append("  <result>").Append(iRet).Append("</result>").Append(Environment.NewLine)
            sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine)
            Return sbRet.ToString()
        Catch ex As Exception

        End Try
        Return sbRet.ToString()
    End Function
    Public Function Handler_AddPersonalFolder() As String
        Dim sbRet As New StringBuilder
        Dim apiContent As New ContentAPI
        Dim iNode As Long = 0
        Dim iFolder As Integer = 0
        Dim sName As String = ""
        Dim sDesc As String = ""
        Dim sKey As String = ""
        Try
            iNode = Convert.ToInt32(Request.QueryString("node"))
            If Request.QueryString("name") <> "" Then
                sName = Request.QueryString("name")
            End If
            sKey = AntiXss.HtmlEncode(Request.QueryString("key"))
            If Request.QueryString("desc") <> "" Then
                sDesc = Request.QueryString("desc")
            End If

            Try
                iFolder = apiContent.AddPersonalDirectoryFolder(iNode, sName, sDesc)
            Catch ex As Exception
                EkException.ThrowException(ex)
            End Try
            sbRet.Append("  <method>AddPersonalFolder</method>").Append(Environment.NewLine)
            sbRet.Append("  <result>").Append(iFolder).Append("</result>").Append(Environment.NewLine)
            sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine)
            Return sbRet.ToString()
        Catch ex As Exception

        End Try
        Return sbRet.ToString()
    End Function
    Public Function Handler_AddEditContentFlag() As String
        Dim sbRet As New StringBuilder
        Dim apiContent As New ContentAPI
        Dim apiFlagging As New Community.FlaggingAPI
        Dim iFlag As Long = 0
        Dim iContent As Long = 0
        Dim iLang As Integer = 0
        Dim iret As Long = 0
        Dim iDef As Integer = 0
        Dim sComment As String = ""
        Dim sKey As String = ""
        Dim cfFlag As New ContentFlagData
        Try
            Dim visitorid As String

            ' Check and see if the cookie has been set by the user
            If (Page.Request.Cookies("ekContentRatingID") Is Nothing And Not Page.IsPostBack) Then
                visitorid = System.Guid.NewGuid().ToString().Replace("-", "")
                Page.Response.Cookies.Add(New System.Web.HttpCookie("ekContentRatingID", visitorid))
                Page.Response.Cookies("ekContentRatingID").Expires = Date.MaxValue
            Else
                Try
                    visitorid = Page.Request.Cookies("ekContentRatingID").Value
                Catch ex As Exception
                    visitorid = String.Empty
                End Try
            End If
            If Request.QueryString("comment") <> "" Then
                sComment = Server.HtmlEncode(Request.QueryString("comment"))
            End If
            If Request.QueryString("lang") <> "" Then
                iLang = Request.QueryString("lang")
            End If
            iFlag = Convert.ToInt64(Request.QueryString("flag"))
            ' iDef = Convert.ToInt32(Request.QueryString("def"))
            iContent = Convert.ToInt64(Request.QueryString("contentid"))
            sKey = AntiXss.HtmlEncode(Request.QueryString("key"))
            If apiFlagging.ContentLanguage < 1 And iLang > 0 Then
                apiFlagging.ContentLanguage = iLang
            End If
            Dim objectFlag As New ObjectFlagData()
            Dim fdRet As ContentFlagData = Nothing
            Try
                fdRet = apiFlagging.GetContentFlagData(iContent, apiContent.UserId, visitorid)

                If fdRet.EntryId = 0 Then
                    objectFlag.FlagComment = sComment
                    objectFlag.FlagItemId = iFlag
                    objectFlag.VisitorID = visitorid
                    objectFlag.UserId = apiContent.UserId
                    objectFlag.ObjectId = iContent
                    objectFlag.ObjectType = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content
                    objectFlag.ObjectLanguageId = iLang
                    iret = apiFlagging.AddFlagObject(objectFlag)
                Else
                    objectFlag.FlagComment = sComment
                    objectFlag.FlagItemId = iFlag
                    objectFlag.FlagId = fdRet.FlagDefinition.ID
                    objectFlag.VisitorID = fdRet.VisitorID
                    objectFlag.UserId = fdRet.FlaggedUser.Id
                    objectFlag.ObjectId = fdRet.Id
                    objectFlag.ObjectType = Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Content
                    objectFlag.ObjectLanguageId = fdRet.LanguageId
                    objectFlag.FlagEntryId = fdRet.EntryId
                    apiFlagging.UpdateFlagObject(objectFlag)
                    iret = fdRet.EntryId
                End If
            Catch ex As Exception
                EkException.ThrowException(ex)
            End Try
            'cfFlag = apiContent.AddContentFlag(iContent, visitorid, iFlag, sComment)

            sbRet.Append("  <method>AddEditFlag</method>").Append(Environment.NewLine)
            sbRet.Append("  <result>").Append(iFlag).Append("</result>").Append(Environment.NewLine)
            sbRet.Append("  <key>").Append(sKey).Append("</key>").Append(Environment.NewLine)
            Return sbRet.ToString()
        Catch ex As Exception

        End Try

        Return sbRet.ToString()
    End Function

    Public Function Handler_AddEditContentRating() As String
        Dim sbRet As New StringBuilder
        Dim apiContent As New ContentAPI
        Dim iRating As Integer = 0
        Dim iContent As Long = 0
        Dim iret As Integer = 0
        Dim bapproved As Boolean = True
        Dim sReview As String = ""
        Dim skey As String = ""
        Try
            Dim visitorid As String
            ' Check and see if the cookie has been set by the user
            If (Page.Request.Cookies("ekContentRatingID") Is Nothing And Not Page.IsPostBack) Then
                visitorid = System.Guid.NewGuid().ToString().Replace("-", "")
                Page.Response.Cookies.Add(New System.Web.HttpCookie("ekContentRatingID", visitorid))
                Page.Response.Cookies("ekContentRatingID").Expires = Date.MaxValue
            Else
                Try
                    visitorid = Page.Request.Cookies("ekContentRatingID").Value
                Catch ex As Exception
                    visitorid = String.Empty
                End Try
            End If
            If Request.QueryString("review") <> "" Then
                sReview = AntiXss.HtmlEncode(Request.QueryString("review"))
            End If
            If Request.QueryString("key") <> "" Then
                skey = AntiXss.HtmlEncode(Request.QueryString("key"))
            End If
            iRating = Convert.ToInt32(Request.QueryString("rating"))
            iContent = Convert.ToInt64(Request.QueryString("contentid"))

            If Request.QueryString("LangType") IsNot Nothing AndAlso Request.QueryString("LangType") <> "" Then
                apiContent.ContentLanguage = Convert.ToInt32(Page.Request.QueryString("LangType"))
            Else
                Dim ecmCookie As System.Web.HttpCookie = Ektron.Cms.CommonApi.GetEcmCookie()
                apiContent.ContentLanguage = ecmCookie("SiteLanguage")
            End If

            bapproved = Convert.ToBoolean(Request.QueryString("approved"))
            If bapproved = False And sReview = "" Then
                bapproved = True 'if there is no review text, it goes live auto.
            End If
            iret = apiContent.AddEditContentRating(iContent, visitorid, iRating, bapproved, sReview)
            sReview = HttpUtility.UrlDecode(sReview)

            sbRet.Append("  <method>AddEditRating</method>").Append(Environment.NewLine)
            sbRet.Append("  <result>").Append(iRating).Append("</result>").Append(Environment.NewLine)
            sbRet.Append("  <key>").Append(skey).Append("</key>").Append(Environment.NewLine)
            Return sbRet.ToString()
        Catch ex As Exception

        End Try

        Return sbRet.ToString()
    End Function

    Public Function Handler_ExistingUserRank() As String
        Dim sbRet As New StringBuilder
        Dim apiContent As New ContentAPI
        Dim brContent As New Ektron.Cms.Content.EkContent(apiContent.RequestInformationRef)
        Dim iRet As Integer = 0
        Dim iUserRank As Long = 0
        Dim iPosts As Integer = 0
        Dim sUserRank As String = ""
        Dim iboard As Long = 0
        Dim bisstart As Boolean = False
        Dim aUserRank As UserRank() = Array.CreateInstance(GetType(Ektron.Cms.UserRank), 0)

        Try
            iUserRank = Convert.ToInt64(Request.QueryString("urid"))
            sUserRank = Request.QueryString("urname")
            iboard = Convert.ToInt64(Request.QueryString("boardid"))
            If (Request.QueryString("posts") <> "") Then
                iPosts = Convert.ToInt32(Request.QueryString("posts"))
            End If
            If (Request.QueryString("isstart") <> "") Then
                bisstart = Convert.ToBoolean(Request.QueryString("isstart"))
            End If

            aUserRank = brContent.SelectUserRankByBoard(iboard)

            If aUserRank.Length > 0 Then
                For i As Integer = 0 To (aUserRank.Length - 1)
                    ' check for existing name
                    If aUserRank(i).Name = sUserRank And Not (iUserRank = aUserRank(i).ID) Then
                        iRet = 2 ' name conflict
                        Exit For
                    End If
                    ' check for number of posts
                    If iPosts > 0 Then ' if its not a ladder rank, we don't do this check
                        If aUserRank(i).Posts > 0 And aUserRank(i).Posts = iPosts And Not (iUserRank = aUserRank(i).ID) Then
                            iRet = 1 ' post conflict
                            Exit For
                        End If
                    End If
                    ' check for starting rank
                    If bisstart = True And aUserRank(i).StartGroup = True And Not (iUserRank = aUserRank(i).ID) Then
                        iRet = 3 ' start conflict
                        Exit For
                    End If
                Next
            End If

            sbRet.Append("  <method>checkUserRank</method>").Append(Environment.NewLine)
            sbRet.Append("  <result>").Append(iRet.ToString()).Append("</result>").Append(Environment.NewLine)

            Return sbRet.ToString()
        Catch ex As Exception

        End Try

        Return sbRet.ToString()
    End Function

    Public Function Handler_ExistingRule() As String
        Dim sbRet As New StringBuilder
        Dim apiContent As New ContentAPI
        Dim rwModule As Ektron.Cms.DataIO.EkModuleRW
        Dim bRet As Boolean = False
        Dim iRule As Long = 0
        Dim sRule As String = ""

        Try
            iRule = Convert.ToInt64(Request.QueryString("rid"))
            sRule = AntiXss.HtmlEncode(Request.QueryString("rname"))

            rwModule = New Ektron.Cms.DataIO.EkModuleRW(apiContent.RequestInformationRef)

            bRet = rwModule.GetRuleByName(sRule, iRule)

            sbRet.Append("  <method>checkRule</method>").Append(Environment.NewLine)
            sbRet.Append("  <result>").Append(Convert.ToInt32(bRet)).Append("</result>").Append(Environment.NewLine)

            Return sbRet.ToString()
        Catch ex As Exception

        End Try

        Return sbRet.ToString()
    End Function

    Public Function Handler_ExistingRuleset() As String
        Dim sbRet As New StringBuilder
        Dim apiContent As New ContentAPI
        Dim rwModule As Ektron.Cms.DataIO.EkModuleRW
        Dim bRet As Boolean = False
        Dim iRuleset As Long = 0
        Dim sRuleset As String = ""

        Try
            iRuleset = Convert.ToInt64(Request.QueryString("rid"))
            sRuleset = AntiXss.HtmlEncode(Request.QueryString("rname"))

            rwModule = New Ektron.Cms.DataIO.EkModuleRW(apiContent.RequestInformationRef)

            bRet = rwModule.GetRuleSetByName(sRuleset, iRuleset)

            sbRet.Append("  <method>checkRuleset</method>").Append(Environment.NewLine)
            sbRet.Append("  <result>").Append(Convert.ToInt32(bRet)).Append("</result>").Append(Environment.NewLine)

            Return sbRet.ToString()
        Catch ex As Exception

        End Try

        Return sbRet.ToString()
    End Function

    Public Function Handler_ExistingFolder() As String
        Dim sbRet As New StringBuilder
        Dim apiContent As New ContentAPI
        Dim cContent As EkContent
        Dim iParent As Long = 0
        Dim iFolder As Long = 0
        Dim sFolder As String = ""
        Dim bExists As Boolean = False

        Try
            iParent = Convert.ToInt64(Request.QueryString("pid"))
            sFolder = AntiXss.HtmlEncode(Request.QueryString("fname"))

            cContent = apiContent.EkContentRef
            bExists = cContent.DoesFolderExistsWithName(sFolder, iParent, iFolder)

            sbRet.Append("  <method>checkName</method>").Append(Environment.NewLine)
            sbRet.Append("  <result>").Append(iFolder.ToString()).Append("</result>").Append(Environment.NewLine)
        Catch ex As Exception

        End Try
        Return sbRet.ToString()
    End Function

    Public Function Handler_ExistingUser() As String

        Return ""
    End Function

#End Region

End Class
