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
	internal partial class frmAddStockManual
		: System.Windows.Forms.Form
	{


		private bool editingData = false;
		private string currentIdProduct = String.Empty;
		private string currentQuantityPerUnit = String.Empty;
		private string currentUnit = String.Empty;
		private string currentProductName = String.Empty;
		private double currentPriceReference = 0;
		private bool codeGeneratedChange = false;
		private double quantity = 0;
		private double stockPrice = 0, unitPrice = 0;

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

		private void cmdSave_Click(Object eventSender, EventArgs eventArgs)
		{
			int newStockId = 0;
			int newManualLogId = 0;
			int newStockLogId = 0;
			editingData = false;
			try
			{
				modConnection.ExecuteSql("Select * from Stocks");
				modConnection.rs.AddNew();
				modConnection.rs["ProductID"] = currentIdProduct;
				modConnection.rs["Stock"] = txtValues[0].Text;
				modConnection.rs["InitialStock"] = txtValues[0].Text;
				modConnection.rs["DateStarted"] = DateTime.Today;
				modConnection.rs["DateModified"] = DateTime.Today;
				modConnection.rs["User"] = modMain.UserId;
				newStockId = Convert.ToInt32(modConnection.rs["StockID"]);
				modConnection.rs["UnitPrice"] = txtValues[2].Text;
				modConnection.rs["StockPrice"] = txtValues[1].Text;
				modConnection.rs.Update();
				newStockId = Convert.ToInt32(modConnection.rs["StockID"]);

				modConnection.ExecuteSql("Select * from ManualStocks");
				modConnection.rs.AddNew();
				modConnection.rs["StockID"] = newStockId;
				modConnection.rs["Quantity"] = txtValues[0].Text;
				modConnection.rs["Price"] = txtValues[1].Text;
				modConnection.rs["User"] = modMain.UserId;
				modConnection.rs["Date"] = DateTime.Today;
				modConnection.rs["Action"] = "ADD";
				modConnection.rs.Update();
				newManualLogId = Convert.ToInt32(modConnection.rs["ManualID"]);

				modConnection.ExecuteSql("Select * from StockLog");
				modConnection.rs.AddNew();
				modConnection.rs["User"] = modMain.UserId;
				modConnection.rs["Date"] = DateTime.Today;
				modConnection.rs["Quantity"] = txtValues[0].Text;
				modConnection.rs["StockPrice"] = txtValues[1].Text;
				modConnection.rs["ProductID"] = currentIdProduct;
				modConnection.rs["StockID"] = newStockId;
				modConnection.rs["DocType"] = "MANUAL";
				modConnection.rs["DocID"] = newManualLogId;
				modConnection.rs.Update();
				newStockLogId = Convert.ToInt32(modConnection.rs["ID"]);

				modConnection.ExecuteSql("Update Products Set UnitsInStock = UnitsInStock + " + txtValues[0].Text + 
				                         " Where ProductId = '& currentIdProduct &'");

				if (MessageBox.Show("Data added successfully" + Environment.NewLine + "Would you like to add a new stock manually?", "New data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
				{
					ClearFields();
				}
				else
				{
					this.Close();
				}
			}
			catch (System.Exception excep)
			{
				//UPGRADE_WARNING: (2081) Err.Number has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2081.aspx
				MessageBox.Show("An error has occurred adding the data. Error: (" + Information.Err().Number.ToString() + ") " + excep.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load event and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
		private void frmAddStockManual_Load(Object eventSender, EventArgs eventArgs)
		{
			editingData = false;
			codeGeneratedChange = false;
		}

		private void frmAddStockManual_FormClosing(Object eventSender, FormClosingEventArgs eventArgs)
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

		private void lvProducts_Click(Object eventSender, EventArgs eventArgs)
		{
			RetrieveDataProduct();
		}

		private void lvProducts_ItemClick(ListViewItem Item)
		{
			RetrieveDataProduct();
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
				RetrieveDataProduct();
			}
		}

		private void txtName_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			DoSearchProduct();
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
			ListViewItem x = null;
			if (modConnection.rs.RecordCount == 0)
			{
				modMain.LogStatus("There are no records with the selected criteria", this);
			}
			else
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
					//RetrieveDataProduct
				}
			}
		}

		private void RetrieveDataProduct()
		{
			if (editingData)
			{
				if (MessageBox.Show("Do you want to cancel previous edited data?", "Data edition", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
				{
					return;
				}
			}

			ListViewItem withVar = null;
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (lvProducts.FocusedItem != null)
			{
				withVar = lvProducts.FocusedItem;
				currentIdProduct = lvProducts.FocusedItem.Text;
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!String.IsNullOrEmpty(ListViewHelper.GetListViewSubItem(withVar, 5).Text))
				{
					currentQuantityPerUnit = ListViewHelper.GetListViewSubItem(withVar, 5).Text;
				}
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!String.IsNullOrEmpty(ListViewHelper.GetListViewSubItem(withVar, 6).Text))
				{
					currentUnit = ListViewHelper.GetListViewSubItem(withVar, 6).Text;
				}
				currentProductName = ListViewHelper.GetListViewSubItem(withVar, 1).Text;
				currentPriceReference = Double.Parse(ListViewHelper.GetListViewSubItem(withVar, 2).Text);
				txtProductName.Text = currentProductName;
				txtQuantityPerUnit.Text = currentQuantityPerUnit;
				txtUnit.Text = currentUnit;
				txtValues[0].Text = "1";
				txtValues[1].Text = currentPriceReference.ToString();
				txtValues[2].Text = currentPriceReference.ToString();
				txtValues[0].Focus();
				modFunctions.SelectAll(txtValues[0]);
				editingData = false;
			}
		}


		private void txtName_Leave(Object eventSender, EventArgs eventArgs)
		{
			if (lvProducts.Items.Count == 1)
			{
				RetrieveDataProduct();
			}
		}

		private void txtValues_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			int Index = Array.IndexOf(txtValues, eventSender);
			if (!codeGeneratedChange)
			{
				editingData = true;
				codeGeneratedChange = true;
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!String.IsNullOrEmpty(txtValues[0].Text))
				{
					quantity = Double.Parse(txtValues[0].Text);
				}
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!String.IsNullOrEmpty(txtValues[1].Text))
				{
					stockPrice = Double.Parse(txtValues[1].Text);
				}
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!String.IsNullOrEmpty(txtValues[2].Text))
				{
					unitPrice = Double.Parse(txtValues[2].Text);
				}
				switch(Index)
				{
					case 0 : case 2 : 
						txtValues[1].Text = (unitPrice * quantity).ToString(); 
						break;
					case 1 : 
						txtValues[2].Text = (stockPrice / quantity).ToString(); 
						break;
				}
				lblNewQuantity.Text = StringsHelper.Format(quantity * Double.Parse(currentQuantityPerUnit), "##,###.00") + currentUnit;
				codeGeneratedChange = false;
			}
		}

		private void txtValues_Enter(Object eventSender, EventArgs eventArgs)
		{
			int Index = Array.IndexOf(txtValues, eventSender);
			modFunctions.SelectAll(txtValues[Index]);
		}

		private void txtValues_KeyPress(Object eventSender, KeyPressEventArgs eventArgs)
		{
			int KeyAscii = Strings.Asc(eventArgs.KeyChar);
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
			if (KeyAscii == 0)
			{
				eventArgs.Handled = true;
			}
			eventArgs.KeyChar = Convert.ToChar(KeyAscii);
		}

		private void ClearFields()
		{
			codeGeneratedChange = true;
			txtValues[0].Text = "";
			txtValues[1].Text = "";
			txtValues[2].Text = "";
			txtCode.Text = "";
			txtName.Text = "";
			txtUnit.Text = "";
			txtProductName.Text = "";
			txtQuantityPerUnit.Text = "";
			lvProducts.Items.Clear();
			txtCode.Focus();
			editingData = false;
			codeGeneratedChange = false;
			lblNewQuantity.Text = "";
			modMain.ClearLogStatus(this);
		}
		private void frmAddStockManual_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}