VERSION 5.00
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "MSCOMCTL.OCX"
Begin VB.MDIForm frmMain 
   BackColor       =   &H8000000C&
   Caption         =   "Sales Agent"
   ClientHeight    =   7365
   ClientLeft      =   225
   ClientTop       =   870
   ClientWidth     =   11475
   LinkTopic       =   "MDIForm1"
   StartUpPosition =   3  'Windows Default
   Begin MSComctlLib.StatusBar sbStatusBar 
      Align           =   2  'Align Bottom
      Height          =   375
      Left            =   0
      TabIndex        =   0
      Top             =   6990
      Width           =   11475
      _ExtentX        =   20241
      _ExtentY        =   661
      _Version        =   393216
      BeginProperty Panels {8E3867A5-8586-11D1-B16A-00C0F0283628} 
         NumPanels       =   3
         BeginProperty Panel1 {8E3867AB-8586-11D1-B16A-00C0F0283628} 
            AutoSize        =   1
            Object.Width           =   14570
            MinWidth        =   1764
         EndProperty
         BeginProperty Panel2 {8E3867AB-8586-11D1-B16A-00C0F0283628} 
            Style           =   5
            TextSave        =   "4:33 PM"
         EndProperty
         BeginProperty Panel3 {8E3867AB-8586-11D1-B16A-00C0F0283628} 
            Style           =   6
            TextSave        =   "5/9/2014"
         EndProperty
      EndProperty
   End
   Begin VB.Menu mnuFile 
      Caption         =   "&File"
      Begin VB.Menu mnuCustomer 
         Caption         =   "&Manage Customers"
      End
      Begin VB.Menu mnuProviders 
         Caption         =   "Manage Su&ppliers "
      End
      Begin VB.Menu mnuSales 
         Caption         =   "&Sales"
      End
      Begin VB.Menu mnuDatabase 
         Caption         =   "&Database"
         Visible         =   0   'False
         Begin VB.Menu mnuDataAction 
            Caption         =   "&Back up"
            Index           =   0
         End
         Begin VB.Menu mnuDataAction 
            Caption         =   "&Restore"
            Index           =   1
         End
         Begin VB.Menu mnuDataAction 
            Caption         =   "-"
            Index           =   2
         End
         Begin VB.Menu mnuDataAction 
            Caption         =   "C&lear records"
            Index           =   3
            Begin VB.Menu mnuClear 
               Caption         =   "&Inventory Items"
               Index           =   0
            End
            Begin VB.Menu mnuClear 
               Caption         =   "&Transaction Items"
               Index           =   1
            End
            Begin VB.Menu mnuClear 
               Caption         =   "-"
               Index           =   2
            End
            Begin VB.Menu mnuClear 
               Caption         =   "&All items"
               Index           =   3
            End
         End
         Begin VB.Menu mnuDataAction 
            Caption         =   "&Compact"
            Index           =   4
         End
      End
      Begin VB.Menu mnuReports 
         Caption         =   "&Reports"
         Visible         =   0   'False
         Begin VB.Menu mnuReport 
            Caption         =   "&Sales Report"
         End
      End
      Begin VB.Menu lExit 
         Caption         =   "-"
         Index           =   1
      End
      Begin VB.Menu mnuExit 
         Caption         =   "&Exit"
      End
   End
   Begin VB.Menu mnuOrders 
      Caption         =   "&Orders"
      Begin VB.Menu mnuCreateOrderRequest 
         Caption         =   "Create Order"
      End
      Begin VB.Menu mnuOrderRequestsApproval 
         Caption         =   "Create Invoice"
      End
      Begin VB.Menu lExit2 
         Caption         =   "-"
      End
      Begin VB.Menu mnuCreateOrderReception 
         Caption         =   "Add Stock Order"
      End
      Begin VB.Menu mnuOrderReceptionsApproval 
         Caption         =   "Add Stock to Inventory"
      End
   End
   Begin VB.Menu mnuMainInventory 
      Caption         =   "&Inventory"
      Begin VB.Menu mnuAddStockManually 
         Caption         =   "Inventory Update"
      End
      Begin VB.Menu mnuAdjustStockManually 
         Caption         =   "Inventory Adjust"
      End
   End
   Begin VB.Menu mnuAccounts 
      Caption         =   "&Maintenance"
      Begin VB.Menu mnuProducts 
         Caption         =   "Manage Products"
      End
      Begin VB.Menu mnuCategories 
         Caption         =   "Manage Product Categories"
      End
      Begin VB.Menu mnuSecurity 
         Caption         =   "Manage Users"
      End
   End
   Begin VB.Menu mnuHelp 
      Caption         =   "&Help"
      Begin VB.Menu mnuViewHelp 
         Caption         =   "&View Help"
      End
      Begin VB.Menu mnuAbout 
         Caption         =   "&About"
      End
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private Sub mnuAbout_Click()
frmAbout.Show vbModal, Me
End Sub

Private Sub mnuAddStockManually_Click()
frmAddStockManual.Show vbModal
End Sub

Private Sub mnuAdjustStockManually_Click()
frmAdjustStockManual.Show vbModal
End Sub

Private Sub mnuCategories_Click()
'frmCategories.Show vbModal
End Sub

Private Sub mnuCreateOrderReception_Click()
frmOrderReception.Show vbModal
End Sub

Private Sub mnuCreateOrderRequest_Click()
frmOrderRequest.Show vbModal
End Sub

Private Sub mnuCustomer_Click()
SetParentChild frmCustomers, Me
frmCustomers.Show
frmCustomers.InitForm
End Sub

Private Sub mnuExit_Click()
Unload Me
End Sub

Private Sub mnuOrderReceptionsApproval_Click()
frmReceptionApproval.Show vbModal
End Sub

Private Sub mnuOrderRequestsApproval_Click()
frmRequestApproval.Show vbModal
End Sub

Private Sub mnuProducts_Click()
SetParentChild frmProducts, Me
frmProducts.Show
End Sub

Private Sub mnuProviders_Click()
SetParentChild frmProviders, Me
frmProviders.Show
End Sub

Private Sub mnuSecurity_Click()
frmUsersManage.Show vbModal
End Sub

Private Sub mnuUsers_Click()

End Sub
