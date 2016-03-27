Imports System
Imports Ektron.Cms
Imports System.Web
Imports System.Xml
Imports System.IO

Partial Class Blogs_xmlrpc
    Inherits System.Web.UI.Page

    Dim m_xdRequest As New XmlDocument
    Dim m_xdResponse As New XmlDocument
    Dim m_xnMethodCall As XmlNode
    Dim m_refContAPI As New ContentAPI
    Dim m_refBlog As Content.Blog

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Response.Clear()
            Response.ContentType = "text/xml"

            'get the XML from the request.
            m_xdRequest.Load(Request.InputStream)

            m_refBlog = New Content.Blog(m_refContAPI.RequestInformationRef)
            If m_xdRequest.InnerXml.ToLower().IndexOf("<methodname>metaweblog.newmediaobject</methodname>") > -1 Then
                Dim lib_setting_data As New Collection
                Dim libdata As New Collection
                Dim m_refLibrary As Ektron.Cms.Library.EkLibrary
                Dim iblogid As Long = 0
                Dim xnlParams As System.Xml.XmlNodeList
                Dim imgpath As String = ""
                Dim filepath As String = ""
                Dim xeElem As XmlElement

                m_refLibrary = m_refContAPI.EkLibraryRef
                xnlParams = m_xdRequest.SelectNodes("methodCall/params/param")
                If xnlParams(0).FirstChild.HasChildNodes = True Then
                    iblogid = Convert.ToInt64(xnlParams(0).FirstChild.FirstChild.InnerText)
                Else
                    iblogid = Convert.ToInt64(xnlParams(0).FirstChild.InnerText)
                End If
                libdata = New Collection
                libdata.Add(iblogid, "FolderID")
                libdata.add(True, "Override")
                lib_setting_data = m_refLibrary.GetLibrarySettingsv2_2(libdata)
                imgpath = lib_setting_data("ImageDirectory")
                filepath = lib_setting_data("FileDirectory")
                Try
                    imgpath = Server.MapPath(imgpath)
                    filepath = Server.MapPath(filepath)
                Catch ex As Exception
                    ' eat this exception
                End Try
                ' now attach to the xml.
                xeElem = m_xdRequest.CreateElement("member")
                xeElem.AppendChild(m_xdRequest.CreateElement("name"))
                xeElem.ChildNodes(0).InnerText = "imgpath"
                xeElem.AppendChild(m_xdRequest.CreateElement("value"))
                xeElem.ChildNodes(1).AppendChild(m_xdRequest.CreateElement("string"))
                xeElem.ChildNodes(1).ChildNodes(0).InnerText = imgpath
                m_xdRequest.ChildNodes(1).ChildNodes(1).ChildNodes(3).ChildNodes(0).ChildNodes(0).AppendChild(xeElem)

                xeElem = m_xdRequest.CreateElement("member")
                xeElem.AppendChild(m_xdRequest.CreateElement("name"))
                xeElem.ChildNodes(0).InnerText = "filepath"
                xeElem.AppendChild(m_xdRequest.CreateElement("value"))
                xeElem.ChildNodes(1).AppendChild(m_xdRequest.CreateElement("string"))
                xeElem.ChildNodes(1).ChildNodes(0).InnerText = filepath
                m_xdRequest.ChildNodes(1).ChildNodes(1).ChildNodes(3).ChildNodes(0).ChildNodes(0).AppendChild(xeElem)
            End If
            m_xdResponse = m_refContAPI.MetaWeblogAPI(m_xdRequest)
            ShowResponse(m_xdResponse)
        Catch ex As Exception
            ErrorResponse(1, ex.Message)
        End Try
    End Sub

    Private Sub ErrorResponse(ByVal errNum As Integer, ByVal errText As String)
        Try
            Dim d As New XmlDocument
            Dim root As XmlElement = d.CreateElement("response")
            d.AppendChild(root)
            Dim er As XmlElement = d.CreateElement("error")
            root.AppendChild(er)
            er.AppendChild(d.CreateTextNode(errNum.ToString))
            If errText <> "" Then
                Dim msg As XmlElement = d.CreateElement("message")
                root.AppendChild(msg)
                msg.AppendChild(d.CreateTextNode(errText))
            End If
            d.Save(Response.Output)
            Response.End()
        Catch ex As Exception
            'handle the error.
        End Try
    End Sub

    Private Sub ShowResponse(ByVal ResponseXML As XmlDocument)
        Try
            ResponseXML.Save(Response.Output)
            ' Response.End() ' this throws an error.
        Catch ex As Exception
            ErrorResponse(1, ex.Message)
        End Try
    End Sub
End Class