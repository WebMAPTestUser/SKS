using UpgradeHelpers.VB6.DB.ADO;
using UpgradeHelpers.VB6.Gui;
using UpgradeHelpers.VB6.Utils;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Media;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmProducts
		: System.Windows.Forms.Form
	{

		private bool NewMode = false;
		private bool EditMode = false;
		private bool CancellingMode = false;
		public string CurrentProductID = String.Empty;

		private void cmdCategories_Click(Object eventSender, EventArgs eventArgs)
		{
			//frmCategories.Show vbModal
			//txtCategory = frmCategories.CurrentCategoryId
		}

		//Private Sub dcProducts_MoveComplete(ByVal adReason As ADODB.EventReasonEnum, ByVal pError As ADODB.Error, adStatus As ADODB.EventStatusEnum, ByVal pRecordset As ADODB.Recordset)
		//NewMode = False
		//EditMode = False
		//CancellingMode = False
		//HandleCommands
		//End Sub

		//Private Sub dcProducts_WillMove(ByVal adReason As ADODB.EventReasonEnum, adStatus As ADODB.EventStatusEnum, ByVal pRecordset As ADODB.Recordset)
		//CancellingMode = True
		//HandleCommands
		//End Sub

		private void cmbCategory_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
		{
			if (cmbCategory.Items.Count == 0 || cmbCategory.SelectedIndex == -1)
			{
				return;
			}
			txtCategory.Text = cmbCategory.GetItemData(cmbCategory.SelectedIndex).ToString();
		}

		private void frmProducts_Closed(Object eventSender, EventArgs eventArgs)
		{
			CurrentProductID = Convert.ToString(dcProducts.Recordset["ProductId"]);
		}

		private void txtCategory_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			if (cmbCategory.Items.Count == 0)
			{
				string tempRefParam = "CategoryName";
				string tempRefParam2 = "CategoryID";
				modFunctions.LoadCombo("Categories", cmbCategory, ref tempRefParam, ref tempRefParam2);
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (String.IsNullOrEmpty(txtCategory.Text))
			{
				cmbCategory.SelectedIndex = -1;
				return;
			}
			int Index = -1;
			int tempForVar = cmbCategory.Items.Count;
			for (modMain.i = 0; modMain.i <= tempForVar; modMain.i++)
			{
				if (cmbCategory.GetItemData(modMain.i) == StringsHelper.ToDoubleSafe(txtCategory.Text))
				{
					Index = modMain.i;
					break;
				}
			}
			cmbCategory.SelectedIndex = modMain.i;
		}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load event and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
		private void frmProducts_Load(Object eventSender, EventArgs eventArgs)
		{
			dcProducts.ConnectionString = modMain.ConnectionString;
			NewMode = false;
			EditMode = false;
			CancellingMode = false;
			HandleCommands();
			if (cmbCategory.Items.Count == 0)
			{
				string tempRefParam = "CategoryName";
				string tempRefParam2 = "CategoryID";
				modFunctions.LoadCombo("Categories", cmbCategory, ref tempRefParam, ref tempRefParam2);
			}
		}

		private void HandleCommands()
		{
			if (EditMode || NewMode)
			{
				Toolbar1.Items[2].Enabled = true;
				Toolbar1.Items[5].Enabled = true;
			}
			else
			{
				Toolbar1.Items[2].Enabled = false;
				Toolbar1.Items[5].Enabled = false;
			}
			Toolbar1.Items[0].Enabled = !NewMode;
			Toolbar1.Items[1].Enabled = !EditMode;
		}

		private void Toolbar1_ButtonClick(Object eventSender, EventArgs eventArgs)
		{
			ToolStripItem Button = (ToolStripItem) eventSender;
			object x = null;
			switch(Button.Text)
			{
				case "Add" : 
					//Add new record 
					NewMode = true; 
					dcProducts.Recordset.AddNew(); 
					dcProducts.Recordset["UnitsInStock"] = 0; 
					dcProducts.Recordset["UnitsOnOrder"] = 0; 
					break;
				case "Edit" : 
					//Edit mode 
					EditMode = true; 
					//dcProducts.Recordset.EditMode = 
					break;
				case "Save" : 
					//Save data 
					dcProducts.Recordset.Update(); 
					EditMode = false; 
					NewMode = false; 
					break;
				case "Delete" : 
					//Delete record 
					if (MessageBox.Show("Are you sure you want to delete this record?", "Delete record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						dcProducts.Recordset.Delete();
						dcProducts.Recordset.Requery();
					} 
					break;
				case "Search" : 
					//Search for records 
					modFunctions.SearchShow("Products", "ProductName", "product"); 
					break;
				case "Cancel" : 
					CancellingMode = true; 
					//Cancel edited changes 
					EditMode = false; 
					NewMode = false; 
					dcProducts.Recordset.CancelUpdate(); 
					dcProducts.Recordset.Requery(); 
					CancellingMode = false; 
					break;
			}
			HandleCommands();
		}

		private void txtField_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			if (!CancellingMode)
			{
				EditMode = true;
				HandleCommands();
			}
		}

		//Used in Search Form
		//Public Sub SearchCriteria(field As String, value As String)
		//ExecuteSql "Select * from Products where " & field & " LIKE '" & value & "%'"
		//If rs.RecordCount = 0 Then
		//    MsgBox "There are no records with the selected criteria", vbInformation, "Search"
		//Else
		//    Set dcProducts.Recordset = rs
		//End If
		//End Sub

		private void txtField_KeyPress(Object eventSender, KeyPressEventArgs eventArgs)
		{
			int KeyAscii = Strings.Asc(eventArgs.KeyChar);
			int Index = Array.IndexOf(txtField, eventSender);
			if (Index == 0)
			{
				//UPGRADE_ISSUE: (1058) Assignment not supported: KeyAscii to a non-positive constant More Information: http://www.vbtonet.com/ewis/ewi1058.aspx
				KeyAscii = Strings.Asc(Strings.Chr(KeyAscii).ToString().ToUpper()[0]);
			}
			else if (Index == 4 || Index == 5)
			{ 
				if (KeyAscii >= ((int) Keys.D0) && KeyAscii <= ((int) Keys.D9))
				{
				}
				else if (KeyAscii == ((int) Keys.Back) || KeyAscii == ((int) Keys.Clear) || KeyAscii == ((int) Keys.Delete))
				{ 
				}
				else if (KeyAscii == ((int) Keys.Left) || KeyAscii == ((int) Keys.Right) || KeyAscii == ((int) Keys.Up) || KeyAscii == ((int) Keys.Down) || KeyAscii == ((int) Keys.Tab))
				{ 
				}
				else
				{
					KeyAscii = 0;
					SystemSounds.Beep.Play();
				}
			}
			if (KeyAscii == 0)
			{
				eventArgs.Handled = true;
			}
			eventArgs.KeyChar = Convert.ToChar(KeyAscii);
		}
	}
}