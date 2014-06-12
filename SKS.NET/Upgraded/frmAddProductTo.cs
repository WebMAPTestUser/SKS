using UpgradeHelpers.VB6.DB.ADO;
using UpgradeHelpers.VB6.Gui;
using Microsoft.VisualBasic;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmAddProductTo
		: System.Windows.Forms.Form
	{


		public int Id = 0;
		public string ObjectReferred = String.Empty;
		public string Table = String.Empty;
		public string ColumnName = String.Empty;

		public bool SavedChanges = false;
		private OrderedDictionary productsStored = null;
		private OrderedDictionary productsToDelete = null;
		private OrderedDictionary productsToAdd = null;
		private bool editingData = false;
		private string currentIdProduct = String.Empty;

		private bool codeGeneratedChange = false;

		private void chkAll_CheckStateChanged(Object eventSender, EventArgs eventArgs)
		{
			bool check = chkAll.CheckState == CheckState.Checked;
			int tempForVar = lvProductsBy.Items.Count;
			for (modMain.i = 1; modMain.i <= tempForVar; modMain.i++)
			{
				lvProductsBy.Items[modMain.i - 1].Checked = check;
			}
		}

		private void cmdClose_Click(Object eventSender, EventArgs eventArgs)
		{
			this.Close();
		}

		private void cmdProducts_Click(Object eventSender, EventArgs eventArgs)
		{
			frmProducts.DefInstance.ShowDialog();
			txtCode.Text = frmProducts.DefInstance.CurrentProductID;
			txtName.Text = "";
			DoSearchProduct();
		}

		private void cmdRemove_Click(Object eventSender, EventArgs eventArgs)
		{
			string productIdToDelete = String.Empty;
			for (modMain.i = lvProductsBy.Items.Count; modMain.i >= 1; modMain.i--)
			{
				if (lvProductsBy.Items[modMain.i - 1].Checked)
				{
					productIdToDelete = lvProductsBy.Items[modMain.i - 1].Text;

					if (modFunctions.Exists(productsStored, productIdToDelete))
					{
						if (modFunctions.Exists(productsToAdd, productIdToDelete))
						{
							productsToDelete.Remove(productIdToDelete);
						}
						else
						{
							//UPGRADE_WARNING: (1068) tempRefParam of type Variant is being forced to string. More Information: http://www.vbtonet.com/ewis/ewi1068.aspx
							object tempRefParam = productIdToDelete;
							modFunctions.AddToCollection(productsToDelete, ref tempRefParam);
							productIdToDelete = Convert.ToString(tempRefParam);
						}
					}
					else
					{
						if (modFunctions.Exists(productsToAdd, currentIdProduct))
						{
							productsToAdd.Remove(currentIdProduct);
						}
					}

					lvProductsBy.Items.RemoveAt(modMain.i - 1);
					editingData = true;
				}
			}
		}

		private void cmdSave_Click(Object eventSender, EventArgs eventArgs)
		{

			if (productsToAdd.Count == 0 && productsToDelete.Count == 0)
			{
				editingData = true;
				MessageBox.Show("No data to be saved", "No data modified", MessageBoxButtons.OK, MessageBoxIcon.Information);
				this.Close();
				return;
			}
			SavedChanges = true;
			foreach (string productCode in productsToAdd.Values)
			{
				modConnection.ExecuteSql("Insert into " + Table + "(" + ColumnName + ", ProductID) Values (" + Id.ToString() + ", '" + productCode + "')");
			}
			foreach (string productCode in productsToDelete.Values)
			{
				modConnection.ExecuteSql("Delete from " + Table + " Where " + ColumnName + " = " + Id.ToString() + " And ProductID = '" + productCode + "'");
			}

			editingData = false;
			MessageBox.Show("Data was succesfully saved", "New data", MessageBoxButtons.OK, MessageBoxIcon.Information);
			this.Close();
			return;

			//UPGRADE_WARNING: (2081) Err.Number has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2081.aspx
			MessageBox.Show("An error has occurred adding the data. Error: (" + Information.Err().Number.ToString() + ") " + Information.Err().Description, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load event and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
		private void frmAddProductTo_Load(Object eventSender, EventArgs eventArgs)
		{
			editingData = false;
			editingData = false;
			codeGeneratedChange = false;
			this.Text = "Add product(s) to " + ObjectReferred;
			lblProductsRelated.Text = "Products related to " + ObjectReferred;
			productsStored = new OrderedDictionary(System.StringComparer.OrdinalIgnoreCase);
			productsToDelete = new OrderedDictionary(System.StringComparer.OrdinalIgnoreCase);
			productsToAdd = new OrderedDictionary(System.StringComparer.OrdinalIgnoreCase);
			LoadProductsById();
		}

		private void frmAddProductTo_FormClosing(Object eventSender, FormClosingEventArgs eventArgs)
		{
			int Cancel = (eventArgs.Cancel) ? 1 : 0;
			int UnloadMode = (int) eventArgs.CloseReason;
			DialogResult res = (DialogResult) 0;
			if (editingData)
			{
				res = MessageBox.Show("Do you want to save the edited data?", "Save data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (res == System.Windows.Forms.DialogResult.Yes)
				{
					cmdSave_Click(cmdSave, new EventArgs());
				}
				else if (res != System.Windows.Forms.DialogResult.No)
				{ 
					Cancel = -1;
				}
			}
			eventArgs.Cancel = Cancel != 0;
		}

		private void lvProducts_ItemClick(ListViewItem Item)
		{
			AddProductToSet();
		}

		private void txtCode_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			DoSearchProduct();
		}

		private void txtCode_KeyPress(Object eventSender, KeyPressEventArgs eventArgs)
		{
			int KeyAscii = Strings.Asc(eventArgs.KeyChar);
			//UPGRADE_ISSUE: (1058) Assignment not supported: KeyAscii to a non-positive constant More Information: http://www.vbtonet.com/ewis/ewi1058.aspx
			KeyAscii = modFunctions.UpCase(KeyAscii);
			if (KeyAscii == 0)
			{
				eventArgs.Handled = true;
			}
			eventArgs.KeyChar = Convert.ToChar(KeyAscii);
		}

		private void txtCode_Leave(Object eventSender, EventArgs eventArgs)
		{
			if (lvProducts.Items.Count == 1)
			{
				AddProductToSet();
			}
		}

		private void txtName_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			DoSearchProduct();
		}

		private void LoadProductsById()
		{
			string productCode = String.Empty;
			modConnection.ExecuteSql("Select p.ProductID, p.ProductName, p.UnitPrice, p.QuantityPerUnit, p.Unit from Products as p, " + Table + " as pb Where pb." + ColumnName + " = " + Id.ToString() + " And pb.ProductId = p.ProductId");

			modMain.LogStatus("There are " + modConnection.rs.RecordCount.ToString() + " records with the selected criteria", this);
			ListViewItem x = null;
			if (modConnection.rs.RecordCount > 0)
			{
				foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
				{
					productCode = Convert.ToString(iteration_row[0]);
					//UPGRADE_WARNING: (1068) tempRefParam of type Variant is being forced to string. More Information: http://www.vbtonet.com/ewis/ewi1068.aspx
					object tempRefParam = productCode;
					modFunctions.AddToCollection(productsStored, ref tempRefParam);
					productCode = Convert.ToString(tempRefParam);
					x = (ListViewItem) lvProductsBy.Items.Add(productCode);
					for (modMain.i = 1; modMain.i <= 2; modMain.i++)
					{
						//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
						if (modConnection.rs.GetField(modMain.i) != null)
						{
							ListViewHelper.GetListViewSubItem(x, modMain.i).Text = Convert.ToString(iteration_row[modMain.i]);
						}
					}
					ListViewHelper.GetListViewSubItem(x, 3).Text = Convert.ToString(iteration_row[3]) + Convert.ToString(iteration_row[4]);
				}
			}
		}

		private void DoSearchProduct()
		{
			string filter = "";
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(txtCode.Text))
			{
				filter = "ProductId LIKE '%" + txtCode.Text + "%'";
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(txtName.Text))
			{
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!String.IsNullOrEmpty(filter))
				{
					filter = filter + " AND ";
				}
				filter = filter + "ProductName LIKE '%" + txtName.Text + "%'";
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(filter))
			{
				filter = "Where " + filter;
			}
			modConnection.ExecuteSql("Select ProductID, ProductName, UnitPrice, UnitsInStock, UnitsOnOrder, QuantityPerUnit, Unit from Products " + filter);
			lvProducts.Items.Clear();
			modMain.LogStatus("There are " + modConnection.rs.RecordCount.ToString() + " records with the selected criteria", this);
			ListViewItem x = null;
			if (modConnection.rs.RecordCount > 0)
			{
				foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
				{
					x = (ListViewItem) lvProducts.Items.Add(Convert.ToString(iteration_row[0]));
					for (modMain.i = 1; modMain.i <= (modConnection.rs.FieldsMetadata.Count - 1); modMain.i++)
					{
						//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
						if (modConnection.rs.GetField(modMain.i) != null)
						{
							ListViewHelper.GetListViewSubItem(x, modMain.i).Text = Convert.ToString(iteration_row[modMain.i]);
						}
					}
				}
				if (lvProducts.Items.Count == 1)
				{
					lvProducts.Items[0].Selected = true;
				}
			}
		}

		private void AddProductToSet()
		{

			ListViewItem y = null;
			int i = 0;
			bool found = false;
			ListViewItem x = null;
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (lvProducts.FocusedItem != null)
			{
				y = (ListViewItem) lvProducts.FocusedItem;
				currentIdProduct = lvProducts.FocusedItem.Text;
				found = false;
				int tempForVar = lvProductsBy.Items.Count;
				for (i = 1; i <= tempForVar; i++)
				{
					if (lvProductsBy.Items[i - 1].Text == currentIdProduct)
					{
						lvProductsBy.Items[i - 1].Selected = true;
						found = true;
						break;
					}
					else if (String.CompareOrdinal(lvProductsBy.Items[i - 1].Text, currentIdProduct) > 0)
					{ 
						break;
					}
				}
				if (!found)
				{
					editingData = true;
					if (!modFunctions.Exists(productsStored, currentIdProduct))
					{
						if (modFunctions.Exists(productsToDelete, currentIdProduct))
						{
							productsToDelete.Remove(currentIdProduct);
						}
						else
						{
							//UPGRADE_WARNING: (1068) tempRefParam of type Variant is being forced to string. More Information: http://www.vbtonet.com/ewis/ewi1068.aspx
							object tempRefParam = currentIdProduct;
							modFunctions.AddToCollection(productsToAdd, ref tempRefParam);
							currentIdProduct = Convert.ToString(tempRefParam);
						}
					}
					else
					{
						if (modFunctions.Exists(productsToDelete, currentIdProduct))
						{
							productsToDelete.Remove(currentIdProduct);
						}
					}
					x = (ListViewItem) lvProductsBy.Items.Insert(i - 1, currentIdProduct);
					ListViewHelper.GetListViewSubItem(x, 1).Text = ListViewHelper.GetListViewSubItem(y, 1).Text;
					ListViewHelper.GetListViewSubItem(x, 2).Text = ListViewHelper.GetListViewSubItem(y, 2).Text;
					ListViewHelper.GetListViewSubItem(x, 3).Text = ListViewHelper.GetListViewSubItem(y, 5).Text + ListViewHelper.GetListViewSubItem(y, 6).Text;
				}
			}
		}

		//UPGRADE_NOTE: (7001) The following declaration (ClearFields) seems to be dead code More Information: http://www.vbtonet.com/ewis/ewi7001.aspx
		//private void ClearFields()
		//{
				//codeGeneratedChange = true;
				//txtCode.Text = "";
				//txtName.Text = "";
				//lvProducts.Items.Clear();
				//lvProductsBy.Items.Clear();
				//txtCode.Focus();
				//editingData = false;
				//codeGeneratedChange = false;
		//}
		private void frmAddProductTo_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}