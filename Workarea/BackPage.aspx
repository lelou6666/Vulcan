<script language="vb" runat="server">
dim callBackPage As String
    Function getCallBackupPage(ByVal defval As String) As String
        Dim tmpStr As String
        If (Request.QueryString("callbackpage") <> "") Then
            tmpStr = Request.QueryString("callbackpage")
            If ((tmpStr = "cmsform.aspx") And (Request.QueryString("fldid") <> "")) Then
                tmpStr = tmpStr & "?folder_id=" & Request.QueryString("fldid") & "&" & Request.QueryString("parm1") & "=" & Request.QueryString("value1")
            Else
                tmpStr = tmpStr & "?" & Request.QueryString("parm1") & "=" & Request.QueryString("value1")
            End If
 		
            If (Request.QueryString("parm2") <> "") Then
                tmpStr = tmpStr & "&" & Request.QueryString("parm2") & "=" & Request.QueryString("value2")
                If (Request.QueryString("parm3") <> "") Then
                    tmpStr = tmpStr & "&" & Request.QueryString("parm3") & "=" & Request.QueryString("value3")
                    If (Request.QueryString("parm4") <> "") Then
                        tmpStr = tmpStr & "&" & Request.QueryString("parm4") & "=" & Request.QueryString("value4")
                    End If
                End If
            End If
            getCallBackupPage = tmpStr
        Else
            getCallBackupPage = defval
        End If
    End Function
'This function will pass pack the url paremeter so that they can be passed along
    Function BuildCallBackParms(ByVal leadingChar As String) As String
        Dim strTmp2 As String
        If (Request.QueryString("callbackpage") <> "") Then
            strTmp2 = "callbackpage=" & Request.QueryString("callbackpage") & "&parm1=" & Request.QueryString("parm1") & "&value1=" & Request.QueryString("value1")
            If (Request.QueryString("parm2") <> "") Then
                strTmp2 = strTmp2 & "&parm2=" & Request.QueryString("parm2") & "&value2=" & Request.QueryString("value2")
                If (Request.QueryString("parm3") <> "") Then
                    strTmp2 = strTmp2 & "&parm3=" & Request.QueryString("parm3") & "&value3=" & Request.QueryString("value3")
                    If (Request.QueryString("parm4") <> "") Then
                        strTmp2 = strTmp2 & "&parm4=" & Request.QueryString("parm4") & "&value4=" & Request.QueryString("value4")
                    End If
                End If
            End If
            strTmp2 = leadingChar & strTmp2
            BuildCallBackParms = strTmp2
        Else
            BuildCallBackParms = ""
        End If
    End Function

    Function getCallingpage(ByVal defVal As String) As String
        Dim tmp2 As String
        If (Request.QueryString("callbackpage") <> "") Then
            tmp2 = Request.QueryString("callbackpage")
            getCallingpage = tmp2
        Else
            getCallingpage = defVal
        End If
    End Function
</script>

