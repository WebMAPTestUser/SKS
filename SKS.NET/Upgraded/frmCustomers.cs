using UpgradeHelpers.VB6.DB.ADO;
using System;
using System.Data;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmCustomers
		: System.Windows.Forms.Form
	{


		private bool NewMode = false;
		private bool EditMode = false;
		private bool CancellingMode = false;
		public string CurrentCustomerID = String.Empty;

		private void frmCustomers_Closed(Object eventSender, EventArgs eventArgs)
		{
			CurrentCustomerID = Convert.ToString(dcCustomers.Recordset["CustomerId"]);
		}


		//Private Sub dcCustomers_MoveComplete(ByVal adReason As ADODB.EventReasonEnum, ByVal pError As ADODB.Error, adStatus As ADODB.EventStatusEnum, ByVal pRecordset As ADODB.Recordset)
		//NewMode = False
		//EditMode = False
		//CancellingMode = False
		//End Sub

		//Private Sub dcCustomers_WillMove(ByVal adReason As ADODB.EventReasonEnum, adStatus As ADODB.EventStatusEnum, ByVal pRecordset As ADODB.Recordset)
		//CancellingMode = True
		//End Sub

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load event and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
		private void frmCustomers_Load(Object eventSender, EventArgs eventArgs)
		{
			dcCustomers.ConnectionString = modMain.ConnectionString;
			NewMode = false;
			EditMode = false;
			CancellingMode = false;
			HandleCommands();
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
					dcCustomers.Recordset.AddNew(); 
					break;
				case "Edit" : 
					//Edit mode 
					EditMode = true; 
					//dcCustomers.Recordset.EditMode = 
					break;
				case "Save" : 
					//Save data 
					dcCustomers.Recordset.Update(); 
					EditMode = false; 
					NewMode = false; 
					break;
				case "Delete" : 
					//Delete record 
					if (MessageBox.Show("Are you sure you want to delete this record?", "Delete record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						dcCustomers.Recordset.Delete();
						dcCustomers.Recordset.Requery();
					} 
					break;
				case "Search" : 
					//Search for records 
					modFunctions.SearchShow("Customers", "CompanyName", "customer"); 
					break;
				case "Cancel" : 
					CancellingMode = true; 
					//Cancel edited changes 
					EditMode = false; 
					NewMode = false; 
					dcCustomers.Recordset.CancelUpdate(); 
					dcCustomers.Recordset.Requery(); 
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

		//Used already in frmSearch
		public void SearchCriteriaProducts(string field, string value)
		{
			modConnection.ExecuteSql("Select * from Customers where " + field + " LIKE '" + value + "%'");
			if (modConnection.rs.RecordCount == 0)
			{
				MessageBox.Show("There are no records with the selected criteria", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				modMain.LogStatus("There are " + modConnection.rs.RecordCount.ToString() + " that meet with the selected criteria");
				dcCustomers.Recordset = modConnection.rs;
			}
		}
	}
}