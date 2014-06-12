Attribute VB_Name = "modFunctions"
Option Explicit

Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As Any, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nsize As Long, ByVal lpFileName As String) As Long
Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As Any, ByVal lpString As Any, ByVal lpFileName As String) As Long
Public Sub AppendAND(ByRef filter As String)
    If filter <> Empty Then
        filter = filter & " AND "
    End If
End Sub

Public Function AddToCollection(col As Collection, Item)
AddToCollection = False
If Not Exists(col, Item) Then
    col.Add Item, Item
    AddToCollection = True
End If
End Function

Public Function Exists(col As Collection, Index) As Boolean
On Error GoTo ExistsTryNonObject
Dim o As Object
Set o = col(Index)
Exists = Not (o Is Nothing)
Exit Function
ExistsTryNonObject:
Exists = ExistsNonObject(col, Index)
End Function

Private Function ExistsNonObject(col As Collection, Index) As Boolean
On Error GoTo ExistsNonObjectErrorHandler
Dim v As Variant
v = col(Index)
ExistsNonObject = Not (v Is Nothing)
Exit Function
ExistsNonObjectErrorHandler:
ExistsNonObject = False
End Function

Public Function DoubleValue(strValue As String)
If Len(strValue) <> 0 Then
    DoubleValue = CDbl(strValue)
Else
    DoubleValue = 0
End If
End Function

Public Sub SelectAll(ByRef txtBox As textbox)
txtBox.SelStart = 0
txtBox.SelLength = Len(txtBox)
End Sub

Public Function UpCase(ByRef KeyAscii As Integer)
UpCase = Asc(UCase(Chr(KeyAscii)))
End Function


''''''''''''''''''''''''''''''''''
''' Combobox related functions '''
''''''''''''''''''''''''''''''''''

Public Sub LoadCombo(Table As String, combo As ComboBox, _
                    field As String, Optional valueField As String)
ExecuteSql "Select * From " & Table
combo.Clear
If (valueField <> Empty) Then
    While Not rs.EOF
        combo.AddItem (rs.Fields(field))
        combo.ItemData(combo.NewIndex) = rs.Fields(valueField)
        rs.MoveNext
    Wend
Else
    While Not rs.EOF
        combo.AddItem (rs.Fields(field))
        rs.MoveNext
    Wend
End If
'If strDefault <> Empty Then
   ' combo = strDefault
'End If
End Sub


Public Function ComboEmpty(ByRef combo As ComboBox, _
                Optional strip As Variant, _
                Optional Index As Integer) _
                As Boolean
If combo.ListIndex = -1 Then
    ComboEmpty = True
    MsgBox "Please select an option from the list", vbExclamation
    If Index <> Empty Then
        'strip.SelectedItem = strip.Tabs(Index)
    End If
    combo.SetFocus
Else
    ComboEmpty = False
End If
End Function

Public Function NoRecords(lstView As ListView, Optional Prompt As String) As Boolean
If lstView.ListItems.Count = 0 Then
    If Prompt <> Empty Then
        MsgBox Prompt, vbExclamation
    End If
    NoRecords = True
Else
    NoRecords = False
End If
End Function

Public Function RcrdId(Table As String, Optional Identifier As String, Optional FldNo As String) As String
Dim RcrdNo As Integer
ExecuteSql "Select * from " & Table & " order by " & FldNo & " ASC"
If rs.EOF = False Then
    rs.MoveLast
    RcrdNo = rs.Fields(FldNo) + 1
Else
    RcrdNo = 1
End If
If Identifier <> Empty Then
    RcrdId = Identifier & RcrdNo & Format(Date, "mm")
Else
    RcrdId = RcrdNo
End If
End Function



'''''''''''''''''''''''''''''''''''''''''
Public Sub SearchShow(Table As String, fieldToSearch As String, itemToSearch As String)
With frmSearch
    .Search Table, fieldToSearch, itemToSearch
    .Show vbModal
End With
End Sub

Public Function ValBox(Prompt As String, Icon As Image, Optional Title As String, _
                        Optional Default As Double, _
                        Optional Header As String = "Value Box") As Double
'With frmValue
'    If Title <> Empty Then
 '       .Caption = Title
'    Else
'        .Caption = App.Title
'    End If
'    .lblHeader.Caption = StrConv(Header, vbUpperCase)
'    .imgIcon.Picture = Icon.Picture
'    .lblPrompt.Caption = Prompt
'    .Default Val(Default)
'    .Show vbModal
'    ValBox = Val(.txtValue.Text)
'    Unload frmValue
'End With
End Function


Public Function TextBoxEmpty(ByRef stext As textbox, Optional TabObject As Variant, Optional TabIndex As Integer) As Boolean
If Trim(stext) = Empty Or stext.Text = "  /  /    " Then
    TextBoxEmpty = True
    MsgBox "You need to fill in all required fields", vbExclamation
    If TabIndex <> Empty Then
        'TabObject.SelectedItem = TabObject.Tabs(TabIndex)
    End If
    stext.SetFocus
Else
    TextBoxEmpty = False
End If
End Function

Public Function TextBoxNumberEmpty(ByRef textbox As textbox) As Boolean
'if the input is not a numeric then true
If IsNumeric(textbox.Text) = False Then
    TextBoxNumberEmpty = True
    MsgBox "The field requires a numeric value.", vbExclamation
    textbox.SetFocus
    SelectAll textbox
Else
    TextBoxNumberEmpty = False
End If
End Function



Public Function Warnings(Optional dType As Integer) As Integer
DetectionType = dType
ExecuteSql "Delete * from tblDetections"

'-------expiration
ExecuteSql "SELECT * From tblStockList"
While Not rs.EOF
    If rs.Fields!expiry_date <> "  /  /    " Then
        d = Format(DateValue(rs.Fields!expiry_date), "mm/dd/yyyy")
        If Format(d, "mm") >= Format(Date, "mm") And _
                Format(d, "mm") <= (Val(Format(Date, "mm")) + 2) And _
                Format(d, "yyyy") = Format(Date, "yyyy") Then
            ExecuteSql2 "Select * from tblInventory where ProductId = '" & rs.Fields!productId & "'"
            If rs2.EOF = False Then
                n = rs2.Fields!quantity
            End If
            If Format(rs.Fields!expiry_date, "mm") <= Format(Date, "mm") And _
                    Format(rs.Fields!expiry_date, "dd") <= Format(Date, "dd") And _
                    Format(rs.Fields!expiry_date, "yyyy") <= Format(Date, "yyyy") Then
                s = "This item is already expired. Please unregister this from Inventory and add new stocks." & _
                vbNewLine & vbNewLine & "Item Description: " & rs.Fields!Description & vbNewLine & vbNewLine & _
                "Expiry Date: " & Format(rs.Fields!expiry_date, "mmm. dd, yyyy") & vbNewLine & _
                "Quantity on Inventory: " & n
                SaveDetection rs.Fields!productId, "Expired", s, "tblDetections"
            Else
                s = Format(rs.Fields!expiry_date, "MM") - Format(Date, "MM") _
                    & " Month(s) before Expiry. Please replace it with new stocks and delete your old stocks. " & _
                    vbNewLine & vbNewLine & "Item Description: " & rs.Fields!Description & vbNewLine & vbNewLine & _
                    "Expiry date: " & Format(rs.Fields!expiry_date, "mmm. dd, yyyy") & vbNewLine & _
                    "quantity on Inventory: " & n
                SaveDetection rs.Fields!productId, "Expiration", s, "tblDetections"
            End If
        End If
    End If
    rs.MoveNext
Wend

'-------out of stock
ExecuteSql "SELECT * From tblInventory WHERE quantity < 10"
While Not rs.EOF
    s = "This item do not have enough quantity on your inventory. Please add stock for this item." & vbNewLine & vbNewLine & _
        "Item Description: " & rs.Fields!Description & vbNewLine & vbNewLine & _
        "Currently on Inventory: " & rs.Fields!quantity
    SaveDetection rs.Fields!productId, "Low Stock", s, "tblDetections"
    rs.MoveNext
Wend

'-------low inventory
ExecuteSql "Select * from tblInventory"
If rs.RecordCount = 0 Or rs.RecordCount <= 10 Then
    s = "You don`t have enough items on your inventory." & _
        "Please add items or register items from database to your inventory list." & vbNewLine & vbNewLine & _
        "Items on Inventory: " & rs.RecordCount
    SaveDetection "Inventory", "Low Inventory", s, "tblDetections"
End If

'-------no sales for the month
ExecuteSql "Select * from tblInventory"
While Not rs.EOF
    If Format(Date, "mm") <> 1 Then
        n = Format(Date, "mm") - 1
        ExecuteSql2 "Select * from tblSales where ProductId = '" & rs.Fields!productId & "' and format([date_sold],'mm') = " & n & _
            "and format([date_sold],'yyyy') = " & Format(Date, "yyyy")
        If rs2.EOF = False Then
            If rs2.Fields!quantity < 30 Then
                i = 0
                While Not rs2.EOF
                    i = i + rs2.Fields!quantity
                    rs2.MoveNext
                Wend
                s = "Sales of this item is less for this month." & vbNewLine & vbNewLine & _
                        "Last Month total sales: " & i
                SaveDetection rs.Fields!productId, "Less Sales", s, "tblDetections"
            End If
        End If
    End If
    rs.MoveNext
Wend

'-----No supplier
ExecuteSql "Select * from tblSuppliers"
If rs.RecordCount = 0 Then
    s = "No supplier saved on database. Please add a supplier for item delivery."
    SaveDetection "Suppliers", "No Supplier", s, "tblDetections"
End If

'-----Items no registered
ExecuteSql "Select * from tblItems where on_inventory = 0"
n = 0
While Not rs.EOF
    ExecuteSql2 "SELECT * From tblStockList WHERE ProductId = '" & rs.Fields!productId & "' and Format$([expiry_date],'mm') Between " _
    & Val(Format(Date, "MM")) & " And " & Val(Format(Date, "MM") + 2) _
    & " and format$(expiry_date, 'yyyy') = " _
    & Format(Date, "yyyy")
    If rs2.EOF Then
        n = n + 1
    End If
    rs.MoveNext
Wend
If n > 0 Then
    s = "Some items on your database are not registered on your inventory list. If you don`t register this items, " & _
        " they will not be included on your sales." & vbNewLine & vbNewLine & _
        "Unregistered Items: " & n
    SaveDetection "Register", "Non-Registered", s, "tblDetections"
End If

'-----Delivery Schedule exceeded
ExecuteSql "Select Sup.Company as Company, Sup.last_delivery as LastDelivery, sched.gap as Gap, sched.gap_value as GapVal from tblSuppliers as Sup " & _
        "INNER JOIN tblDeliverySched as Sched ON Sup.sched_type = Sched.description"
While Not rs.EOF
    d = Scheduler(Format(rs.Fields!lastdelivery, "mm"), _
        Format(rs.Fields!lastdelivery, "dd"), _
        Format(rs.Fields!lastdelivery, "yyyy"), _
        rs.Fields!GapVal, _
        rs.Fields!Gap)
    If Format(d, "mm") <= Format(Date, "mm") And Format(d, "dd") < Format(Date, "dd") And rs.Fields!Gap <> "(none)" Then
        s = "Delivery schedule of supplier, " & rs.Fields!company & ", is not updated. " & _
            "Please record all delivery transactions of your suppliers to update it's delivery schedule." & vbNewLine & vbNewLine & _
            "Last Delivery: " & Format(rs.Fields!lastdelivery, "mmm. dd, yyyy") & vbNewLine & _
            "Expected Date: " & Format(d, "mmm. dd, yyyy")
        SaveDetection rs.Fields!company, "Delivery Sched", s, "tblDetections"
    End If
    rs.MoveNext
Wend
ExecuteSql "Select * from tblDetections"
Warnings = rs.RecordCount

End Function

Private Sub SaveDetection(Reference As String, Title As String, Description As String, Table As String)
ExecuteSql2 "Select * from " & Table
rs2.AddNew
rs2.Fields!record_no = Val(RcrdId(Table, , "record_no"))
rs2.Fields!Reference = Reference
rs2.Fields!war_type = Title
rs2.Fields!Description = Description
rs2.Update
End Sub

Public Function ReadINI(strFile As String, strKey As String, strName As String) As String
Dim intLen As Integer
Dim strText As String
strText = "                                                                                                    "
intLen = GetPrivateProfileString(strKey, strName, "", strText, Len(strText), strFile)
If intLen > -1 Then
    strText = Left(strText, intLen)
Else
    MsgBox "Error on reading configuration", vbCritical
    End
End If
ReadINI = strText
End Function

Public Sub WriteINI(strFile As String, strKey As String, strName As String, strText As String)
Dim intLen As Integer
intLen = WritePrivateProfileString(strKey, strName, strText, strFile)
End Sub


Public Function Scheduler(IntM As Integer, _
                        IntD As Integer, _
                        IntY As Integer, _
                        GapVal As Integer, _
                        Optional Gap As String = "Week") As Date
                        
Dim Max As Long, LastVal As Integer
Select Case Gap
    Case "Day"
        Max = Val(ReadINI(App.Path & "\Settings.ini", "Month Max", MonthName(IntM, True)))
        IntD = IntD + GapVal
        For i = 1 To IntD
            If i = Max Then
                LastVal = Max
                IntM = IntM + 1
                If IntM > 12 Then IntM = 1: IntY = IntY + 1
                Max = Max + Val(ReadINI(App.Path & "\Settings.ini", "Month Max", MonthName(IntM, True)))
            End If
        Next i
        IntD = IntD - LastVal
    Case "Week"
        Dim MaxDays As Integer
        MaxDays = 7 * GapVal
        Max = Val(ReadINI(App.Path & "\Settings.ini", "Month Max", MonthName(IntM, True)))
        IntD = IntD + MaxDays
        For i = 1 To IntD
            If i = Max Then
                LastVal = Max
                IntM = IntM + 1
                If IntM > 12 Then IntM = 1: IntY = IntY + 1
                Max = Max + Val(ReadINI(App.Path & "\Settings.ini", "Month Max", MonthName(IntM, True)))
            End If
        Next i
        IntD = IntD - LastVal
    Case "Month"
        IntM = IntM + GapVal
        If IntM > 12 Then IntM = IntM - 12: IntY = IntY + 1
    Case "Year"
        IntY = IntY + GapVal
End Select
Scheduler = DateSerial(IntY, IntM, IntD)
End Function


Public Function ExecErr(Prompt As String, _
                        Optional PromptFld As String, _
                        Optional Table As String, _
                        Optional RcrdFld As String, _
                        Optional RcrdStr As String) As String
Dim Rcrds As String
If Table <> Empty Then
    ExecuteSql "Select * from " & Table & " where " & RcrdFld & " = '" & RcrdStr & "'"
    While Not rs.EOF
        Rcrds = Rcrds & rs.Fields(PromptFld) & "; "
        rs.MoveNext
    Wend
    ExecErr = "Error: " & Prompt & vbNewLine & vbNewLine & _
            "Related Records: " & Rcrds
Else
    ExecErr = Prompt
End If
End Function

