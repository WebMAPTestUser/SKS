Attribute VB_Name = "modMain"
Option Explicit

Public CurrentUserAdmin As Boolean
Public UserFullname As String
Public UserLevel As String
Public UserId As String

Public DatabasePath As String
Public ConnectionString As String

Public DetectionType As Integer
Global n As Double, i As Long, s As String, d As Date
Public msg As String
Public ImgName As String, ImgSrc As String

Private Declare Function SetParent Lib "user32" (ByVal hWndChild As Long, ByVal hWndNewParent As Long) As Long

Sub SetParentChild(Parent As Form, Child As Form)
SetParent Parent.hWnd, Child.hWnd
End Sub

Sub SetNoParentChild(Parent As Form)
SetParent Parent.hWnd, 0&
End Sub


Public Sub Main()
    DatabasePath = App.Path & "\Database\Orders.mdb"
    ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & DatabasePath
    OpenConnection
    CurrentUserAdmin = True
    UserFullname = "Allan Cantillo"
    UserLevel = "Administrator"
    UserId = "acantillo"
    'frmLogin.Show vbModal
    'If (frmLogin.LoginSucceeded) Then
        frmMain.Show
    'End If
End Sub

Public Sub LogStatus(message As String, Optional frm As Form)
    Dim sb As StatusBar
    Set sb = Nothing
    frmMain.sbStatusBar.Panels(1).Text = message
    If Not frm Is Nothing Then
        If frm Is frmAdjustStockManual Then
            Set sb = frmAdjustStockManual.sbStatusBar
        ElseIf frm Is frmActionOrderReception Then
            Set sb = frmActionOrderReception.sbStatusBar
        ElseIf frm Is frmActionOrderRequest Then
            Set sb = frmActionOrderRequest.sbStatusBar
        ElseIf frm Is frmAddStockManual Then
            Set sb = frmAddStockManual.sbStatusBar
        ElseIf frm Is frmReceptionApproval Then
            Set sb = frmReceptionApproval.sbStatusBar
        ElseIf frm Is frmOrderReception Then
            Set sb = frmOrderReception.sbStatusBar
        ElseIf frm Is frmOrderRequest Then
            Set sb = frmOrderRequest.sbStatusBar
        ElseIf frm Is frmRequestApproval Then
            Set sb = frmRequestApproval.sbStatusBar
        End If
        If Not sb Is Nothing Then
            If Not sb.Panels(1) Is Nothing Then
                sb.Panels(1).Text = message
            End If
        End If
    End If
End Sub

Public Sub ClearLogStatus(Optional frm As Form)
    LogStatus "", frm
End Sub



