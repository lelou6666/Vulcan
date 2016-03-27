Imports System.Collections.Specialized
Imports System.Text
Imports System.Web.UI
Imports System.Xml
Imports Ektron.Cms.Common
Imports Ektron.Cms.Common.EkEnumeration
Imports Ektron.Cms.Controls
Imports Ektron.Cms.ToolBar
Imports Microsoft.VisualBasic
Imports System


Namespace CMSUtils
	Namespace CMSMenuUtils

		Public Class NStateMenu
			Inherits Control

			Public IsInitialized As Boolean = False
			Private UninitializedMessage As String = "NStateMenu not initialized.  Set values for menu_id, targettype, and targetid and call the Fill() method.  (Altering any of those values after .Fill() will return NStateMenu to an uninitialized state.)"
			Private ExceptionSource As String = Me.GetType.ToString()
            Const CMS_CONTENTBLOCKID_VARNAME As String = "id"

			Private menu_id_val As CMSIDTypes.menu_id
			Public Property menu_id() As CMSIDTypes.menu_id
				Get
					Return menu_id_val
				End Get
				Set(ByVal Value As CMSIDTypes.menu_id)
					menu_id_val = Value
					IsInitialized = False
				End Set
			End Property

			Private targettype_val As CMSMenuItemType
			Public Property targettype() As CMSMenuItemType
				Get
					Return targettype_val
				End Get
				Set(ByVal Value As CMSMenuItemType)
					targettype_val = Value
					IsInitialized = False
				End Set
			End Property

			Private targetid_val As CMSIDTypes.menuitem_id
			Public Property targetid() As CMSIDTypes.menuitem_id
				Get
					Return targetid_val
				End Get
				Set(ByVal Value As CMSIDTypes.menuitem_id)
					targetid_val = Value
					IsInitialized = False
				End Set
			End Property

			Private MenuObj_val As Ektron.Cms.Controls.Menu
			Public ReadOnly Property MenuObj() As Ektron.Cms.Controls.Menu
				Get
					If IsInitialized = True Then
						Return MenuObj_val
					Else
						Dim ex As New UnintializedException(UninitializedMessage)
						ex.Source = ExceptionSource
						Throw ex
					End If
				End Get
			End Property
			Private TargetCrumb_val As XmlNode
			Public ReadOnly Property TargetCrumb() As XmlNode
				Get
					If IsInitialized = True Then
						Return TargetCrumb_val
					Else
						Dim ex As New UnintializedException(UninitializedMessage)
						ex.Source = ExceptionSource
						Throw ex
					End If
				End Get
			End Property
			Friend TierCrumb_val As XmlNodeList
			Private TierCrumb_idx As XMLNodeListIndexer
			Public ReadOnly Property TierCrumb() As XMLNodeListIndexer
				Get
					If IsInitialized = True Then
						Return TierCrumb_idx
					Else
						Dim ex As New UnintializedException(UninitializedMessage)
						ex.Source = ExceptionSource
						Throw ex
					End If
				End Get
			End Property
			Friend TierList_val() As XmlNodeList

			Public ReadOnly TierList As New TierListIndexer(Me)

			Public ReadOnly Property menuIDVarName() As String
				Get
					Return ID & "_mid"
				End Get
			End Property
			Public ReadOnly Property targetIDVarName() As String
				Get
					Return ID & "_mtid"
				End Get
			End Property
			Public ReadOnly Property targetTypeVarName() As String
				Get
					Return ID & "_mtt"
				End Get
			End Property

			Sub New(ByRef menu_id As CMSIDTypes.menu_id, ByRef targettype As CMSMenuItemType, ByRef targetid As CMSIDTypes.menuitem_id)
				Dim motemp As Ektron.Cms.Controls.Menu = New Ektron.Cms.Controls.Menu
				motemp.CacheInterval = 60
				motemp.DefaultMenuID = menu_id.val
				motemp.Fill()
				CommonConstructor(motemp, targettype, targetid)
			End Sub

			Sub New(ByRef menuObject As Ektron.Cms.Controls.Menu, ByRef targettype As CMSMenuItemType, ByRef targetid As CMSIDTypes.menuitem_id)
				CommonConstructor(menuObject, targettype, targetid)
			End Sub

			Sub New(ByRef menu_id As CMSIDTypes.menu_id) 'If no parameters are passed, try to get them from URL vars

				menu_id_val = menu_id
				Me.AcquireVars()

				If Not (IsNothing(Me.menu_id_val) Or IsNothing(Me.targetid_val) Or IsNothing(Me.targettype_val)) Then
					Try
						Fill()
					Catch ex As NStateMenu.TargetNotInMenuException
						'Eat the exception and remain uninitialized if
						' target doesn't exist.
					End Try
				End If

			End Sub

			Function AcquireVars() As Boolean
				If Not (IsNothing(Me.Page)) Then
					If (Me.Page.Request.QueryString(Me.menuIDVarName) <> "") Then
						menu_id_val = New CMSIDTypes.menu_id(Integer.Parse(Me.Page.Request.QueryString(Me.menuIDVarName)))
					End If

					If (Me.Page.Request.QueryString(Me.targetIDVarName) <> "") Then
						targetid_val = New CMSIDTypes.menuitem_id(Integer.Parse(Me.Page.Request.QueryString(Me.targetIDVarName)))
					ElseIf (Me.Page.Request.QueryString(CMS_CONTENTBLOCKID_VARNAME) <> "") Then
						'If no NStateMenu target ID, try to use CMS content block ID
						' This could throw an exception, if the ID isn't in the menu
						targetid_val = New CMSIDTypes.menuitem_id(Integer.Parse(Me.Page.Request.QueryString(CMS_CONTENTBLOCKID_VARNAME)))
					End If

					If (Me.Page.Request.QueryString(Me.targetTypeVarName) <> "") Then
						targettype_val = [Enum].Parse(GetType(CMSMenuItemType), Me.Page.Request.QueryString(Me.targetTypeVarName))
					Else
						'Default to "content" itemtype
						targettype_val = CMSMenuItemType.content
					End If

				End If

				If IsNothing(Me.menu_id_val) Or IsNothing(Me.targetid_val) Or IsNothing(Me.targettype_val) Then
					Return False
				Else
					Return True
				End If

			End Function

			Sub Fill()
				If IsNothing(Me.menu_id_val) Or IsNothing(Me.targetid_val) Or IsNothing(Me.targettype_val) Then
					Dim ex As New Exception(Me.GetType.ToString() & ": Menu_id or targettype or targetid is null.")
					ex.Source = ExceptionSource
					Throw ex
				Else
					Dim motemp As Ektron.Cms.Controls.Menu = New Ektron.Cms.Controls.Menu
					motemp.CacheInterval = 60
					motemp.DefaultMenuID = Me.menu_id_val.val
					motemp.Fill()
					Me.CommonConstructor(motemp, Me.targettype_val, Me.targetid_val)
				End If
			End Sub

			Private Sub CommonConstructor(ByRef menuObject As Ektron.Cms.Controls.Menu, ByRef targettype As CMSMenuItemType, ByRef targetid As CMSIDTypes.menuitem_id)

				Me.menu_id_val = New CMSIDTypes.menu_id(menuObject.DefaultMenuID)
				Me.targettype_val = targettype
				Me.targetid_val = targetid
				MenuObj_val = menuObject

				' Test for standard menu XML xpaths

				Dim xnl As XmlNodeList
                Dim strValidationPath As String
				Dim strcolValidationPaths As New StringCollection
				Dim strarrValidationPaths() As [String] = {"/MenuDataResult", "/MenuDataResult/Item", "/MenuDataResult/Item/Item"}
				strcolValidationPaths.AddRange(strarrValidationPaths)

				For Each strValidationPath In strcolValidationPaths
					xnl = MenuObj_val.XmlDoc.SelectNodes(strValidationPath)
					If (xnl.Count <= 0) Then
						Dim ex As New TargetNotInMenuException(Me.GetType.ToString() & ": CMS returns invalid menu data.  XPath """ & strValidationPath & """ is missing from the XML returned for menu_id " & MenuObj_val.DefaultMenuID.ToString() & ".  This may indicate that no menu with ID " & MenuObj_val.DefaultMenuID.ToString() & " exists.")
						ex.Source = ExceptionSource
						Throw ex
					End If
				Next

				Dim menuTargetXPath As String = "/descendant::Item[child::ItemID='" & targetid.val.ToString() & "' and ItemType='" & targettype.ToString() & "']"
				xnl = MenuObj_val.XmlDoc.SelectNodes(menuTargetXPath)
				If (xnl.Count > 0) Then	 ' Test to ensure that the target item exists in the menu
					TargetCrumb_val = xnl(0)
				Else
					Dim ex As New TargetNotInMenuException(Me.GetType.ToString() & ": Item ID " & targetid.val.ToString() & ", Type """ & targettype.ToString & """ does not exist in menu_id " & MenuObj_val.DefaultMenuID.ToString())
					ex.Source = ExceptionSource
					Throw ex
				End If

				Dim menuAncestorsXPath As String = "(" & menuTargetXPath & "/ancestor::Item[ItemType='" & CMSMenuItemType.Submenu.ToString() & "'])|(" & menuTargetXPath & ")"
				TierCrumb_val = MenuObj_val.XmlDoc.SelectNodes(menuAncestorsXPath)
				TierCrumb_idx = New XMLNodeListIndexer(TierCrumb_val)

				Dim xn As XmlNode
				Dim xn_ItemType As CMSMenuItemType
				Dim xn_ItemID As Integer

				Dim tierXPath As String
				Dim TierListLength As Integer

				If (targettype = CMSMenuItemType.Submenu) Then
					'If target is a submenu we'll grab the children
					TierListLength = TierCrumb_val.Count
				Else
					TierListLength = TierCrumb_val.Count - 1
				End If

				TierList_val = New XmlNodeList(TierListLength) {}

				For I As Integer = 0 To (TierListLength)

					Select Case I
						Case 0 ' Root menu has no parent, requiring a different XPath
							xn = TierCrumb_val(0)
							xn_ItemType = CType([Enum].Parse(GetType(CMSMenuItemType), xn.SelectSingleNode("ItemType").InnerText), CMSMenuItemType)
							xn_ItemID = Integer.Parse(xn.SelectSingleNode("ItemID").InnerText)
							tierXPath = "/descendant::Item[child::ItemID='" & xn_ItemID.ToString() & "' and ItemType='" & xn_ItemType.ToString() & "']"
						Case TierCrumb_val.Count
							xn = TierCrumb_val(I - 1)
							xn_ItemType = CType([Enum].Parse(GetType(CMSMenuItemType), xn.SelectSingleNode("ItemType").InnerText), CMSMenuItemType)
							xn_ItemID = Integer.Parse(xn.SelectSingleNode("ItemID").InnerText)
							tierXPath = "/descendant::Item[child::ItemID='" & xn_ItemID.ToString() & "' and ItemType='" & xn_ItemType.ToString() & "']/child::Menu/child::Item"
						Case Else
							xn = TierCrumb_val(I)
							xn_ItemType = CType([Enum].Parse(GetType(CMSMenuItemType), xn.SelectSingleNode("ItemType").InnerText), CMSMenuItemType)
							xn_ItemID = Integer.Parse(xn.SelectSingleNode("ItemID").InnerText)
							tierXPath = "/descendant::Item[child::ItemID='" & xn_ItemID.ToString() & "' and ItemType='" & xn_ItemType.ToString() & "']/parent::Menu/child::Item"
					End Select
					xnl = MenuObj_val.XmlDoc.SelectNodes(tierXPath)
					TierList_val.SetValue(xnl, I)
				Next I

				IsInitialized = True

			End Sub

			Public Function GetCMSMenuItemLink(ByRef navItem As XmlNode) As String
				Dim ItemLinkStr As String
				Dim startsWithJSURI As Boolean = False
				Dim ItemType As CMSMenuItemType

				ItemType = UtilObj.GetCMSMenuItemType(navItem)

				Select Case ItemType
					'BE CAREFUL ALTERING THIS
					'This case statement uses GoTo statements - variables may have values you don't expect.
					Case CMSMenuItemType.Submenu
						ItemLinkStr = navItem.SelectSingleNode("Menu/Link").InnerText()
						startsWithJSURI = ItemLinkStr.StartsWith("javascript")
						If startsWithJSURI Then
							GoTo JSCase
						End If
						ItemLinkStr = AppendVars(ItemLinkStr, navItem)
					Case CMSMenuItemType.JavaScript
						ItemLinkStr = navItem.SelectSingleNode("ItemLink").InnerText()
JSCase:
						ItemLinkStr = IIf(startsWithJSURI, "", "javascript: void ") & ItemLinkStr.Replace("""", "'")
					Case Else
						ItemLinkStr = navItem.SelectSingleNode("ItemLink").InnerText()
						startsWithJSURI = ItemLinkStr.StartsWith("javascript")
						If startsWithJSURI Then
							GoTo JSCase
						End If
						ItemLinkStr = AppendVars(ItemLinkStr, navItem)
				End Select

				Return (ItemLinkStr)
			End Function

			Public Function GetCMSMenuItemLinkOnClick(ByRef navItem As XmlNode) As String

				Dim ItemLinkStr As String
				Dim ItemType As CMSMenuItemType
				ItemType = UtilObj.GetCMSMenuItemType(navItem)
				Try
					If ItemType = CMSMenuItemType.JavaScript Then
						ItemLinkStr = navItem.SelectSingleNode("ItemLink").InnerText().Replace("""", "'")
					Else
						If ItemType = CMSMenuItemType.Submenu Then
							ItemLinkStr = navItem.SelectSingleNode("Menu/Link").InnerText()
						Else
							ItemLinkStr = navItem.SelectSingleNode("ItemLink").InnerText()
						End If
						ItemLinkStr = "window.location='" & AppendVars(ItemLinkStr, navItem) & "';"
					End If
				Catch ex As Exception
					ItemLinkStr = ""
				End Try
				Return (ItemLinkStr)

			End Function

			Private Function AppendVars(ByVal ItemLinkStr As String, ByRef navItem As XmlNode) As String
				Dim sb As New StringBuilder(ItemLinkStr)
				If ItemLinkStr <> String.Empty Then
					If ItemLinkStr.IndexOf("?"c) > 0 Then
						sb.Append("&"c)
					Else
						sb.Append("?"c)
					End If
					sb.Append(targetIDVarName & "=" & UtilObj.GetCMSMenuItemID(navItem) & "&" & targetTypeVarName & "=" & UtilObj.GetCMSMenuItemType(navItem) & "&" & menuIDVarName & "=" & MenuObj.DefaultMenuID.ToString())
				End If
				Return (sb.ToString())
			End Function

			Public Class UnintializedException
				Inherits System.Exception
				Public Sub New()
				End Sub
				Public Sub New(ByVal Message As String)
					MyBase.New(Message)
				End Sub
			End Class

			Public Class TargetNotInMenuException
				Inherits System.Exception
				Public Sub New()
				End Sub
				Public Sub New(ByVal Message As String)
					MyBase.New(Message)
				End Sub
			End Class

			Public Class TierListIndexer

				Dim parentTS As NStateMenu

				Public ReadOnly Property Length() As Integer
					Get
						If parentTS.IsInitialized = True Then
							Return parentTS.TierList_val.Length
						Else
							Throw New UnintializedException(parentTS.UninitializedMessage)
						End If
					End Get
				End Property

				Default Public ReadOnly Property TierListIndexer(ByVal I As Integer) As XmlNodeList

					Get

						If parentTS.IsInitialized = True Then
							If (I >= parentTS.TierList_val.Length) Or (I < 0) Then
								Throw New System.IndexOutOfRangeException("TierList: " & I & " not a valid tier number.")
							Else
								Return parentTS.TierList_val(I)
							End If
						Else
							Throw New UnintializedException(parentTS.UninitializedMessage)
						End If

					End Get

				End Property

				Sub New(ByRef parentTS As NStateMenu)
					Me.parentTS = parentTS
				End Sub

			End Class

			Public Class XMLNodeListIndexer
				'The only reason that I created this class is because
				' System.Xml.XmlNodeList throws an extremely unhelpful
				' NullReferenceException if you try to access a member
				' that isn't there.  This class throws a more helpful
				' IndexOutOfRangeException.

				Dim innerXNL As XmlNodeList

				Public ReadOnly Property Count() As Integer
					Get
						Return innerXNL.Count
					End Get
				End Property

				Default Public ReadOnly Property XMLNodeListIndexer(ByVal I As Integer) As XmlNode

					Get
						If (I >= innerXNL.Count) Or (I < 0) Then
							Throw New System.IndexOutOfRangeException("XMLNodeListIndexer: " & I & " is out of range." & IIf(I < 0, "(Cannot be less than zero.)", "(Count = " & innerXNL.Count & ")"))
						Else
							Return innerXNL.Item(I)
						End If

					End Get

				End Property

				Sub New(ByRef innerXNL As XmlNodeList)
					Me.innerXNL = innerXNL
				End Sub

			End Class

		End Class


		Public Class UtilObj

			Shared Function GetCMSSubMenuChildNodes(ByRef navItem As XmlNode) As XmlNodeList

				Dim ItemType As CMSMenuItemType = GetCMSMenuItemType(navItem)

				If ItemType = CMSMenuItemType.Submenu Then
					Return (navItem.SelectNodes("Menu/Item"))
				Else
					Return (navItem.SelectNodes("nonexistent_path_to_create_an_empty_XmlNodeList"))
				End If

			End Function

			Shared Function GetCMSMenuItemTitle(ByRef navItem As XmlNode) As String

				Dim ItemTitleStr As String = navItem.SelectSingleNode("ItemTitle").InnerText()
				Return (ItemTitleStr)

			End Function

			Shared Function GetCMSMenuItemType(ByRef navItem As XmlNode) As CMSMenuItemType

				Dim ItemTypeStr As String = navItem.SelectSingleNode("ItemType").InnerText()
				ItemTypeStr = ItemTypeStr.Replace("Javascript", "JavaScript")
				Return ([Enum].Parse(GetType(CMSMenuItemType), ItemTypeStr))

			End Function

			Shared Function GetCMSMenuItemLink(ByRef navItem As XmlNode) As String

				Dim ItemLinkStr As String
				Dim ItemType As CMSMenuItemType
				ItemType = GetCMSMenuItemType(navItem)
				Select Case ItemType
					Case CMSMenuItemType.Submenu
						ItemLinkStr = navItem.SelectSingleNode("Menu/Link").InnerText()
					Case Else
						ItemLinkStr = navItem.SelectSingleNode("ItemLink").InnerText()
				End Select
				Return (ItemLinkStr)

			End Function

			Shared Function GetCMSMenuItemImage(ByRef navItem As XmlNode) As String

				Dim ItemImageStr As String
				Dim ItemType As CMSMenuItemType
				ItemType = GetCMSMenuItemType(navItem)
				Select Case ItemType
					Case CMSMenuItemType.Submenu
						ItemImageStr = navItem.SelectSingleNode("Menu/Image").InnerText()
					Case Else
						ItemImageStr = navItem.SelectSingleNode("ItemImage").InnerText()
				End Select
				Return (ItemImageStr)

			End Function

			Shared Function GetCMSMenuItemImageOverride(ByRef navItem As XmlNode) As Boolean

				Dim ItemImageOverrideBool As String
				Dim ItemType As CMSMenuItemType
				ItemType = GetCMSMenuItemType(navItem)
				Select Case ItemType
					Case CMSMenuItemType.Submenu
						ItemImageOverrideBool = Boolean.Parse(navItem.SelectSingleNode("Menu/ImageOverride").InnerText())
					Case Else
						ItemImageOverrideBool = Boolean.Parse(navItem.SelectSingleNode("ItemImageOverride").InnerText())
				End Select
				Return (ItemImageOverrideBool)

			End Function

			Shared Function GetCMSMenuItemTarget(ByRef navItem As XmlNode) As CMSMenuItemTarget

				Dim ItemTargetStr As String
				Dim ItemTarget As CMSMenuItemTarget
				Dim ItemType As CMSMenuItemType
				ItemType = GetCMSMenuItemType(navItem)
				Select Case ItemType
					Case CMSMenuItemType.Submenu ' As of this writing submenu nodes don't have targets
						ItemTargetStr = CMSMenuItemTarget.Self.ToString()
					Case CMSMenuItemType.JavaScript
						ItemTargetStr = CMSMenuItemTarget.Self.ToString()
					Case Else
						ItemTargetStr = navItem.SelectSingleNode("ItemTarget").InnerText()
				End Select
				ItemTarget = [Enum].Parse(GetType(CMSMenuItemTarget), ItemTargetStr)
				Return (ItemTarget)

			End Function

			Shared Function CMSMenuItemTargetToString(ByVal ItemTarget As CMSMenuItemTarget) As String

				Dim ItemTargetStr As String = "_self"
				Select Case ItemTarget
					Case CMSMenuItemTarget.Self
						ItemTargetStr = "_self"
					Case CMSMenuItemTarget.Popup
						ItemTargetStr = "_blank"
					Case CMSMenuItemTarget.Top
						ItemTargetStr = "_top"
					Case CMSMenuItemTarget.Parent
						ItemTargetStr = "_parent"
				End Select
				Return (ItemTargetStr)

			End Function

			Shared Function GetCMSMenuItemLinkOnClick(ByRef navItem As XmlNode) As String

				Dim ItemLinkStr As String
				Dim ItemType As CMSMenuItemType
				ItemType = GetCMSMenuItemType(navItem)
				Try
					Select Case ItemType
						Case CMSMenuItemType.JavaScript
							ItemLinkStr = navItem.SelectSingleNode("ItemLink").InnerText().Replace("""", "'")
						Case CMSMenuItemType.Submenu
							ItemLinkStr = "window.location='" & navItem.SelectSingleNode("Menu/Link").InnerText() & "';"
						Case Else
							ItemLinkStr = "window.location='" & navItem.SelectSingleNode("ItemLink").InnerText() & "';"
					End Select
				Catch ex As Exception
					ItemLinkStr = ""
				End Try
				Return (ItemLinkStr)

			End Function

			Shared Function GetCMSMenuItemID(ByRef navItem As XmlNode) As Integer

				Dim ItemIDInt As Integer = Integer.Parse(navItem.SelectSingleNode("ItemID").InnerText())
				Return (ItemIDInt)

			End Function

		End Class
	End Namespace

	Namespace CMSIDTypes
		Public MustInherit Class CMS_ID
			Private _val As Integer

			Property [val]() As Integer
				Get
					Return _val
				End Get
				Set(ByVal Value As Integer)
					_val = Value
				End Set
			End Property

			Sub New(ByVal valinit As Integer)
				Me.val = valinit
			End Sub
		End Class
		Public Class menu_id
			Inherits CMS_ID
			Public Shared Null As menu_id = New menu_id(0)
			Sub New(ByVal valinit As Integer)
				MyBase.New(valinit)
			End Sub
		End Class
		Public Class menuitem_id
			Inherits CMS_ID
			Public Shared Null As menuitem_id = New menuitem_id(0)
			Sub New(ByVal valinit As Integer)
				MyBase.New(valinit)
			End Sub
		End Class
	End Namespace

End Namespace
