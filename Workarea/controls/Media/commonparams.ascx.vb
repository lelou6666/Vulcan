
Partial Class Multimedia_commonparams
    Inherits System.Web.UI.UserControl

    Private m_refContentApi As New Ektron.Cms.CommonApi
    Private m_contentHtml As String = String.Empty
    Private m_mimeType As String = String.Empty
    Private m_assetID As String = String.Empty
    Private m_assetFileName As String = String.Empty
    Private m_assetVersion As String = String.Empty
    Private objectTagStr As String = String.Empty

    Public WriteOnly Property ContentHtml() As String
        Set(ByVal value As String)
            value = value.Substring(value.IndexOf("<root>"), value.IndexOf("</root>") + 7)
            m_contentHtml = value
        End Set
    End Property

    Public WriteOnly Property MimeType() As String
        Set(ByVal value As String)
            m_mimeType = value
        End Set
    End Property

    Public WriteOnly Property AssetID() As String
        Set(ByVal value As String)
            m_assetID = value
        End Set
    End Property

    Public WriteOnly Property AssetVersion() As String
        Set(ByVal value As String)
            m_AssetVersion = value
        End Set
    End Property

    Public WriteOnly Property AssetFileName() As String
        Set(ByVal value As String)
            m_assetFileName = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim sInclude As String = String.Empty
        sInclude = "<script type=""text/javascript"" language=""JavaScript"" src=""" & m_refContentApi.AppPath & "java/colorpicker.js""></script>" & vbCrLf
        sInclude &= "<script type=""text/javascript"" src=""" & m_refContentApi.AppPath & "java/com.ektron.utils.tabs.js"" language=""javascript""></script> " & vbCrLf
        sInclude &= "<link href=""" & m_refContentApi.AppPath & "csslib/EktTabs.css"" rel=""stylesheet"" type=""text/css"" /> " & vbCrLf
        sInclude &= "<input type=""hidden"" id=""media_title"" name=""media_title"" value=""" & m_assetID & """/>"
        sInclude &= "<input type=""hidden"" id=""media_fileName"" name=""media_fileName"" value=""" & m_assetFileName & """ />"

        ''''''''''''
        '' MAC: I think we should disable preview in mac.
        '' Psudo plan:
        '' Read supported players
        '' Create check boxes of all supoorted players and make default player checkbox read only
        '' Now loop through all object from MediaXML (content_html)
        '' mark checkbox if it found and enable ui.
        ''
        '''''''''''''
        Dim htSupportedPlayer As Hashtable = m_refContentApi.EkContentRef.GetSupportedPlayer(m_mimeType)
        Dim sHtml As New StringBuilder
        Dim strCont As String = ""
        Dim strMediaPlayer As String = String.Empty
        Dim strCheckboxes As String = String.Empty
        Dim strDefaultScript As String = String.Empty
        Dim varIDs As String = String.Empty
        Dim fillObjects As String = String.Empty
        Dim bDefault As Boolean = False
        Dim strDisplayName As String
        Dim xDoc As New System.Xml.XmlDocument
        Dim root As System.Xml.XmlNode
        Dim nodelist As System.Xml.XmlNodeList
        Dim mediaPlayerNode As System.Xml.XmlNode

        'BugFix for 30930
        m_contentHtml = m_contentHtml.Replace("&", "&amp;")

        xDoc.LoadXml(m_contentHtml)
        root = xDoc.DocumentElement

        lblMultimedia.Text = m_refContentApi.EkMsgRef.GetMessage("lbl multimedia properties")
        lblwidth.Text = m_refContentApi.EkMsgRef.GetMessage("lbl width")
        lblHeight.Text = m_refContentApi.EkMsgRef.GetMessage("lbl height")
        lblAuto.Text = m_refContentApi.EkMsgRef.GetMessage("lbl autostart")
        lblLoop.Text = m_refContentApi.EkMsgRef.GetMessage("lbl loop")

        For Each strMediaPlayer In htSupportedPlayer.Keys
            bDefault = False
            strCont = ""
            strDisplayName = ""
            strDisplayName = strMediaPlayer
            strMediaPlayer = strMediaPlayer.ToLower()

            If (strMediaPlayer <> "default") Then
                strCheckboxes &= "<input type=""checkbox"" onclick=""enableMediaTab(this, '" & strMediaPlayer & "')"" id=""" & strMediaPlayer & "-chk"" "
                If (strMediaPlayer.ToLower() = htSupportedPlayer("default").ToString().ToLower()) Then
                    bDefault = True
                    strCheckboxes &= " checked=""checked"" disabled=""disabled"" "
                    strDefaultScript &= "myTitle = '" & m_assetID & "';" & vbCrLf
                    'strDefaultScript &= "alert(document.getElementById( '" & strMediaPlayer & "-chk' ));" & vbCrLf
                    strDefaultScript &= "enableMediaTab(document.getElementById( '" & strMediaPlayer & "-chk' ), '" & strMediaPlayer & "');" & vbCrLf
                End If
                strCheckboxes &= " />" & strDisplayName & "&nbsp;"

                If (varIDs <> "") Then
                    varIDs &= ",'" & strMediaPlayer & "'"
                Else
                    varIDs = "'" & strMediaPlayer & "'"
                End If
                If (bDefault) Then
                    fillObjects &= "js" & strMediaPlayer & ".defaultPlayer=true;" & vbCrLf
                End If
                fillObjects &= "js" & strMediaPlayer & ".CLSID = '" & m_refContentApi.EkContentRef.ReadMediaSettings(m_refContentApi.RequestInformationRef.MultimediaSettings, strMediaPlayer & "-CLSID", "") & "';" & vbCrLf

                'Do not fix the path if player is real player since real play needs the physical file.  Real player will not play 
                'the dynamically streamed content.
                If (strMediaPlayer.ToLower() <> "realplayer") Then
                    fillObjects &= "js" & strMediaPlayer & ".URL = '" & m_refContentApi.SitePath & "assetmanagement/DownloadAsset.aspx?history=true&ID=" & m_assetID & "&version=" & m_assetVersion & "';"
                Else
                    fillObjects &= "js" & strMediaPlayer & ".URL = '" & m_assetFileName & "';"
                End If

                fillObjects &= "js" & strMediaPlayer & ".Codebase = '" & m_refContentApi.EkContentRef.ReadMediaSettings(m_refContentApi.RequestInformationRef.MultimediaSettings, strMediaPlayer & "-Codebase", "") & "';" & vbCrLf
                fillObjects &= "js" & strMediaPlayer & ".fill(""" & m_assetID & "_" & strMediaPlayer & """);" & vbCrLf

                'generate preview span tag mediaPlayerNode
                nodelist = root.SelectNodes("MediaPlayer[@player='" & strMediaPlayer & "']")
                sHtml.Append("<span ")
                sHtml.Append("id=""" & strMediaPlayer & "-preview"" >")
                If (nodelist.Count > 0) Then
                    If (Not bDefault) Then
                        fillObjects &= " document.getElementById( '" & strMediaPlayer & "-chk' ).checked = true;" & vbCrLf
                        fillObjects &= " enableMediaTab(document.getElementById( '" & strMediaPlayer & "-chk' ), '" & strMediaPlayer & "');" & vbCrLf
                    End If
                    mediaPlayerNode = nodelist(0)
                    strCont = Server.HtmlDecode(mediaPlayerNode.InnerXml.ToString())
                    strCont = FixURL(strCont)
                    If (strMediaPlayer = "quicktime") Then
                        objectTagStr = strCont
                        strCont = ""
                    End If
                End If

                'Do not fix the path if player is real player since real play needs the physical file.  Real player will not play 
                'the dynamically streamed content.
                If (strMediaPlayer.ToLower() <> "realplayer") Then
                    strCont = FixPath(strCont)
                End If

                sHtml.Append(strCont)
                sHtml.Append("</span>")

                'Render html but ui still not visiable
                If (strMediaPlayer.ToLower() = "windowsmedia") Then
                    Me.WindowsMediaPanel.Visible = True
                ElseIf (strMediaPlayer.ToLower() = "quicktime") Then
                    Me.QuicktimePanel.Visible = True
                ElseIf (strMediaPlayer.ToLower() = "realplayer") Then
                    Me.RealPlayerPanel.Visible = True
                ElseIf (strMediaPlayer.ToLower() = "flash") Then
                    Me.FlashPanel.Visible = True
                    Me.PlaceHolder1.Visible = False
                End If


            End If
        Next

        ltInclude.Text = sInclude
        ltCheckboxes.Text = strCheckboxes
        'Line below work only in IE
        'Page.ClientScript.RegisterStartupScript(Me.GetType(), "array_of_ids", "arrID = [" & varIDs & "];", True)
        'Page.ClientScript.RegisterStartupScript(Me.GetType(), "enabledefaulttab", fillObjects & strDefaultScript, True)
        ltResults.Text = (sHtml.ToString())
        jsLiteral.Text = "<script language=""Javascript"">"

        If (strMediaPlayer = "quicktime") Then
            Dim js As New StringBuilder()
            js.Append("function InsertIntoObjMovieTag(objStr)")
            js.Append("{")
            js.Append(" var target = document.getElementById('" & strMediaPlayer & "-preview');")
            js.Append(" target.innerHTML = objStr;")
            js.Append(" }")
            jsLiteral.Text &= js.ToString()
            jsLiteral.Text &= "InsertIntoObjMovieTag('" & objectTagStr & "');"
        End If
        Dim ext As String = ""
        ext = System.IO.Path.GetExtension(m_assetFileName)

        jsLiteral.Text &= "SetDMSExt(""" & ext & """,""" & Me.ID.Replace("""", "\""") & """);" & vbCrLf & "arrID = [" & varIDs & "];" & vbCrLf & fillObjects & vbCrLf & strDefaultScript & "</script>"

    End Sub
    Public Function FixPath(ByVal html As String) As String
        'this is used when the content is first checked out
        Return html.Replace(m_assetFileName, m_refContentApi.SitePath & "assetmanagement/DownloadAsset.aspx?history=true&ID=" & m_assetID & "&version=" & m_assetVersion)
    End Function
    Private Function FixURL(ByVal strCont As String) As String
        'This is used when the content is drag and drop in the edit window
        Dim iPos1 As Integer = -1
        Dim iPos2 As Integer = -1
        Dim oldURL As String = ""
        iPos1 = strCont.IndexOf(m_refContentApi.SitePath & "assetmanagement/DownloadAsset.aspx?history=true&ID=" & m_assetID & "&version=")
        If (iPos1 > -1) Then
            iPos2 = strCont.IndexOf("""", iPos1)
            If (iPos2 > -1) Then
                oldURL = strCont.Substring(iPos1, iPos2 - iPos1)
                strCont = strCont.Replace(oldURL, m_refContentApi.SitePath & "assetmanagement/DownloadAsset.aspx?history=true&ID=" & m_assetID & "&version=" & m_assetVersion)
            End If
        End If
        Return strCont
    End Function

End Class
