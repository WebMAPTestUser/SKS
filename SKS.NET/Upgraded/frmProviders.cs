using UpgradeHelpers.VB6.DB.ADO;
using System;
using System.Data;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmProviders
		: System.Windows.Forms.Form
	{

		private bool NewMode = false;
		private bool EditMode = false;
		private bool CancellingMode = false;
		public int CurrentProviderID = 0;

		//Private Sub adcProviders_MoveComplete(ByVal adReason As ADODB.EventReasonEnum, ByVal pError As ADODB.Error, adStatus As ADODB.EventStatusEnum, ByVal pRecordset As ADODB.Recordset)
		//NewMode = False
		//EditMode = False
		//CancellingMode = False
		//End Sub

		//Private Sub dcProviders_WillMove(ByVal adReason As ADODB.EventReasonEnum, adStatus As ADODB.EventStatusEnum, ByVal pRecordset As ADODB.Recordset)
		//CancellingMode = True
		//End Sub

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load event and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
		private void frmProviders_Load(Object eventSender, EventArgs eventArgs)
		{
			dcProviders.ConnectionString = modMain.ConnectionString;
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

		private void frmProviders_Closed(Object eventSender, EventArgs eventArgs)
		{
			CurrentProviderID = Convert.ToInt32(dcProviders.Recordset["ProviderId"]);
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
					dcProviders.Recordset.AddNew(); 
					break;
				case "Edit" : 
					//Edit mode 
					EditMode = true; 
					//dcProviders.Recordset.EditMode = 
					break;
				case "Save" : 
					//Save data 
					dcProviders.Recordset.Update(); 
					EditMode = false; 
					NewMode = false; 
					break;
				case "Delete" : 
					//Delete record 
					if (MessageBox.Show("Are you sure you want to delete this record?", "Delete record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						dcProviders.Recordset.Delete();
						dcProviders.Recordset.Requery();
					} 
					break;
				case "Search" : 
					//Search for records 
					modFunctions.SearchShow("Providers", "ProviderName", "Provider"); 
					break;
				case "Cancel" : 
					CancellingMode = true; 
					//Cancel edited changes 
					EditMode = false; 
					NewMode = false; 
					dcProviders.Recordset.CancelUpdate(); 
					dcProviders.Recordset.Requery(); 
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

		//Used in search form
		//Public Sub SearchCriteria(field As String, value As String)
		//ExecuteSql "Select * from Providers where " & field & " LIKE '" & value & "%'"
		//If rs.RecordCount = 0 Then
		//    MsgBox "There are no records with the selected criteria", vbInformation, "Search"
		//Else
		//    LogStatus "There are " & rs.RecordCount & " that meet with the selected criteria"
		//    Set dcProviders.Recordset = rs
		//End If
		//End Sub
	}
}