using UpgradeHelpers.VB6.DB.ADO;
using UpgradeHelpers.VB6.Gui;
using System;
using System.Data;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmSearch
		: System.Windows.Forms.Form
	{

		string SearchTable = String.Empty;
		private void cboSrchBy_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
		{
			lblSrchBy.Text = cboSrchBy.Text;
		}

		private void cmdClose_Click(Object eventSender, EventArgs eventArgs)
		{
			this.Close();
		}


		public void Search(string Table, string fieldToSearch, string itemToSearch)
		{
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(itemToSearch))
			{
				Label20.Text = "Search for a " + itemToSearch;
			}
			SearchTable = Table;
			modConnection.ExecuteSql("Select Top 1 * from " + Table);
			for (modMain.i = 0; modMain.i <= (modConnection.rs.FieldsMetadata.Count - 1); modMain.i++)
			{
				cboSrchBy.AddItem(modConnection.rs.FieldsMetadata[modMain.i].ColumnName);
			}
			cboSrchBy.Text = fieldToSearch;
		}

		private void cmdSearch_Click(Object eventSender, EventArgs eventArgs)
		{
			if (txtSrchStr.Text.Substring(Math.Max(txtSrchStr.Text.Length - 1, 0)) == "'")
			{
				txtSrchStr.Text = String.Empty;
			}
			string txtToSearch = String.Empty;

			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(txtSrchStr.Text.Trim()))
			{
				txtToSearch = txtSrchStr.Text;
			}
			else
			{
				txtToSearch = "%";
			}
			if (SearchTable == "Customers")
			{
				SearchCriteriaCustomers(lblSrchBy.Text, txtToSearch);
			}
			else if (SearchTable == "Products")
			{ 
				SearchCriteriaProducts(lblSrchBy.Text, txtToSearch);
			}
			else if (SearchTable == "Providers")
			{ 
				SearchCriteriaProviders(lblSrchBy.Text, txtToSearch);
			}
		}

		//''
		public void SearchCriteriaCustomers(string field, string value)
		{
			modConnection.ExecuteSql("Select * from Customers where " + field + " LIKE '" + value + "%'");
			if (modConnection.rs.RecordCount == 0)
			{
				MessageBox.Show("There are no records with the selected criteria", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				modMain.LogStatus("There are " + modConnection.rs.RecordCount.ToString() + " that meet with the selected criteria");
				frmCustomers.DefInstance.dcCustomers.Recordset = modConnection.rs;
			}
		}

		public void SearchCriteriaProducts(string field, string value)
		{
			modConnection.ExecuteSql("Select * from Products where " + field + " LIKE '" + value + "%'");
			if (modConnection.rs.RecordCount == 0)
			{
				MessageBox.Show("There are no records with the selected criteria", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				frmProducts.DefInstance.dcProducts.Recordset = modConnection.rs;
			}
		}

		public void SearchCriteriaProviders(string field, string value)
		{
			modConnection.ExecuteSql("Select * from Providers where " + field + " LIKE '" + value + "%'");
			if (modConnection.rs.RecordCount == 0)
			{
				MessageBox.Show("There are no records with the selected criteria", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				modMain.LogStatus("There are " + modConnection.rs.RecordCount.ToString() + " that meet with the selected criteria");
				frmProviders.DefInstance.dcProviders.Recordset = modConnection.rs;
			}
		}
		private void frmSearch_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}