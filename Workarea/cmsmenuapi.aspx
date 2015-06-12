<script language="vb" runat="server">
'This is internal file make sure all required included are included before including this file
' Globals required by nearly everyone, including the menus

    Function p_MenuXML(ByVal inCol As Object, ByVal BaseID As Object, ByVal MenuXML As Object) As String
        Dim SingleItem As Object
        If (inCol("Items").Count) Then
            For Each SingleItem In inCol("Items")
                Select Case CInt(SingleItem("ItemType"))
                    Case 4 'g_CMSMenuItemType("Submenu")
                        MenuXML = MenuXML & p_SubMenu(SingleItem("Menu"))
                    Case 1 'g_CMSMenuItemType("Content")
                        MenuXML = MenuXML & "<item type=""content"" id=""" & SingleItem("ItemID") & """ itemid=""" & SingleItem("ID") & """ title=""" & SingleItem("ItemTitle") & """>"
                        MenuXML = MenuXML & "</item>	"
                    Case 5 'g_CMSMenuItemType("ExternalLink")
                        MenuXML = MenuXML & "<item type=""link"" id=""" & SingleItem("ItemID") & """ itemid=""" & SingleItem("ID") & """ title=""" & SingleItem("ItemTitle") & """>"
                        MenuXML = MenuXML & "</item>"
                    Case 2 'g_CMSMenuItemType("Library")
                        MenuXML = MenuXML & "<item type=""library"" id=""" & SingleItem("ItemID") & """ itemid=""" & SingleItem("ID") & """ title=""" & SingleItem("ItemTitle") & """>"
                        MenuXML = MenuXML & "</item>"
                End Select
            Next
        End If
        p_MenuXML = MenuXML
    End Function

    Function p_SubMenu(ByVal SubMenu As Object) As String
        Dim Items As Object
        Dim SingleItem As Object
        Dim MenuXML As String
        MenuXML = ""
        Items = SubMenu("Items")
        If (Items.count > 0) Then
            For Each SingleItem In Items
                Select Case CInt(SingleItem("ItemType"))
                    Case 4 'g_CMSMenuItemType("Submenu")
                        MenuXML = MenuXML & p_SubMenu(SingleItem("Menu"))
                    Case 1 'g_CMSMenuItemType("Content")
                        If (SingleItem("ContentType") <> 2) Then
                            MenuXML = MenuXML & "<item type=""content"" id=""" & SingleItem("ItemID") & """ itemid=""" & SingleItem("ID") & """ title=""" & SingleItem("ItemTitle") & """>"
                        Else
                            MenuXML = MenuXML & "<item type=""form"" id=""" & SingleItem("ItemID") & """ itemid=""" & SingleItem("ID") & """ title=""" & SingleItem("ItemTitle") & """>"
                        End If
                        MenuXML = MenuXML & "</item>"
                    Case 5 'g_CMSMenuItemType("ExternalLink")
                        MenuXML = MenuXML & "<item type=""link"" id=""" & SingleItem("ItemID") & """ itemid=""" & SingleItem("ID") & """ title=""" & SingleItem("ItemTitle") & """>"
                        MenuXML = MenuXML & "</item>"
                    Case 2 'g_CMSMenuItemType("Library")
                        MenuXML = MenuXML & "<item type=""library"" id=""" & SingleItem("ItemID") & """ itemid=""" & SingleItem("ID") & """ title=""" & SingleItem("ItemTitle") & """>"
                        MenuXML = MenuXML & "</item>"
                End Select
            Next
        End If
        MenuXML = "<menu folder=""" & SubMenu("FolderID") & """ id=""" & SubMenu("MenuID") & """ title=""" & SubMenu("MenuTitle") & """ template=""" & SubMenu("MenuTemplate") & """>" & MenuXML
        MenuXML = MenuXML & "</menu>"
        p_SubMenu = MenuXML
    End Function

    Function p_ItemTarget(ByVal ItemTarget As Object) As String
        Select Case ItemTarget
            Case 1
                ' PopUp
                p_ItemTarget = "_blank"
            Case 2
                ' Self
                p_ItemTarget = "_self"
            Case 3
                ' Parent
                p_ItemTarget = "_parent"
            Case 4
                ' Top
                p_ItemTarget = "_top"
            Case Else
                p_ItemTarget = ""
        End Select
    End Function

    Function p_DoesKeyExist(ByRef collectionObject As Object, ByRef keyName As Object) As String
        Dim dummy As Object
        On Error Resume Next    ' Used to determine condition, only affects this procedure 
        ' (reverts back to previous method when out of scope).
        Err.Clear()
        dummy = collectionObject.Item(keyName)
        If (Err.Number = 0) Then
            p_DoesKeyExist = True
        Else
            p_DoesKeyExist = False
        End If
    End Function

</script>
