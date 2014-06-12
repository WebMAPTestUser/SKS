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
	internal partial class frmAdjustStockManual
		: System.Windows.Forms.Form
	{


		private bool editingData = false;
		private string currentIdProduct = String.Empty;
		private int currentIdStock = 0;
		private string currentQuantityPerUnit = String.Empty;
		private string currentUnit = String.Empty;
		private string currentProductName = String.Empty;
		private double currentStockPrice = 0;
		private double currentStock = 0;
		private double changedStockPrice = 0;
		private double changedStock = 0;
		private bool codeGeneratedChange = false;
		private double quantity = 0;
		private double stockPrice = 0, unitPrice = 0;

		private void cmdClose_Click(Object eventSender, EventArgs eventArgs)
		{
			this.Close();
		}

		private void cmdSave_Click(Object eventSender, EventArgs eventArgs)
		{
			int newStockId = 0;
			int newManualLogId = 0;
			int newStockLogId = 0;
			editingData = false;
			try
			{

				double deltaStockPrice = 0;
				double deltaStock = 0;
				changedStockPrice = Double.Parse(txtValues[0].Text);
				changedStock = Double.Parse(txtValues[1].Text);

				deltaStockPrice = changedStockPrice - currentStockPrice;
				deltaStock = changedStock - currentStock;

				if (deltaStockPrice == 0 && deltaStock == 0)
				{
					modMain.LogStatus("There is no modification of the Stock, the data won't be saved", this);
					return;
				}
				// UPDATE
				modConnection.ExecuteSql("Update Stocks Set StockPrice = " + changedStockPrice.ToString() + 
				                         ", Stock = " + changedStock.ToString() + " Where StockId = " + currentIdStock.ToString());

				// NEW
				modConnection.ExecuteSql("Select * from ManualStocks");
				modConnection.rs.AddNew();
				modConnection.rs["StockID"] = currentIdStock;
				modConnection.rs["Quantity"] = deltaStock;
				modConnection.rs["Price"] = deltaStockPrice;
				modConnection.rs["User"] = modMain.UserId;
				modConnection.rs["Date"] = DateTime.Today;
				modConnection.rs["Action"] = "MOD";
				modConnection.rs.Update();
				newManualLogId = Convert.ToInt32(modConnection.rs["ManualID"]);

				//NEW
				modConnection.ExecuteSql("Select * from StockLog");
				modConnection.rs.AddNew();
				modConnection.rs["User"] = modMain.UserId;
				modConnection.rs["Date"] = DateTime.Today;
				modConnection.rs["Quantity"] = deltaStock;
				modConnection.rs["StockPrice"] = deltaStockPrice;
				modConnection.rs["ProductID"] = currentIdProduct;
				modConnection.rs["StockID"] = currentIdStock;
				modConnection.rs["DocType"] = "MANUAL";
				modConnection.rs["DocID"] = newManualLogId;
				modConnection.rs.Update();
				newStockLogId = Convert.ToInt32(modConnection.rs["ID"]);

				modConnection.ExecuteSql("Update Products Set UnitsInStock = UnitsInStock + " + deltaStock.ToString() + 
				                         " Where ProductId = '& currentIdProduct &'");

				if (MessageBox.Show("Data modified successfully" + Environment.NewLine + "Would you like to modify another stock manually?", "Modify data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
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

		private void frmAdjustStockManual_FormClosing(Object eventSender, FormClosingEventArgs eventArgs)
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

		private void cmdProducts_Click(Object eventSender, EventArgs eventArgs)
		{
			frmProducts.DefInstance.ShowDialog();
			txtCode.Text = frmProducts.DefInstance.CurrentProductID;
			txtName.Text = "";
			DoSearchProduct();
		}

		private void lvProducts_ItemClick(ListViewItem Item)
		{
			DoSearchStocks();
		}

		private void lvStocks_ItemClick(ListViewItem Item)
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
				modMain.LogStatus("There are no records with the selected criteria");
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
				}
			}
		}


		private void DoSearchStocks()
		{
			if (lvProducts.FocusedItem == null)
			{
				return;
			}
			if (editingData)
			{
				if (MessageBox.Show("Do you want to cancel previous edited data?", "Data edition", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
				{
					return;
				}
			}
			string productId = lvProducts.FocusedItem.Text;
			string productName = ListViewHelper.GetListViewSubItem(lvProducts.FocusedItem, 1).Text;
			currentIdProduct = lvProducts.FocusedItem.Text;
			ListViewItem withVar = null;
			withVar = lvProducts.FocusedItem;
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
			txtProductName.Text = productName;
			txtUnit.Text = currentUnit;
			txtQuantityPerUnit.Text = currentQuantityPerUnit;

			modConnection.ExecuteSql("Select StockID, Stock, InitialStock, UnitPrice, " + 
			                         "StockPrice, DateStarted, DateModified, User From Stocks " + 
			                         " Where ProductId = '" + productId + "'");
			lvStocks.Items.Clear();
			ListViewItem x = null;
			if (modConnection.rs.RecordCount == 0)
			{
				modMain.LogStatus("There are no stock records of the product " + productName);
				RetrieveDataProduct();
			}
			else
			{
				foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
				{
					x = (ListViewItem) lvStocks.Items.Add(Convert.ToString(iteration_row[0]));
					for (modMain.i = 1; modMain.i <= (modConnection.rs.FieldsMetadata.Count - 1); modMain.i++)
					{
						ListViewHelper.GetListViewSubItem(x, modMain.i).Text = Convert.ToString(iteration_row[modMain.i]);
					}
				}
				if (lvStocks.Items.Count == 1)
				{
					lvStocks.Items[0].Selected = true;
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

			bool setEmpty = true;
			ListViewItem withVar = null;
			if (lvStocks.FocusedItem != null)
			{
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (lvStocks.FocusedItem != null)
				{
					currentIdStock = Convert.ToInt32(Double.Parse(lvStocks.FocusedItem.Text));
					withVar = lvStocks.FocusedItem;
					currentStock = Double.Parse(ListViewHelper.GetListViewSubItem(withVar, 1).Text);
					currentStockPrice = Double.Parse(ListViewHelper.GetListViewSubItem(withVar, 4).Text);
					codeGeneratedChange = true;
					txtOriginalQuantity.Text = currentStock.ToString();
					txtOriginalPrice.Text = currentStockPrice.ToString();
					txtStockID.Text = currentIdStock.ToString();
					txtValues[0].Text = currentStockPrice.ToString();
					txtValues[1].Text = currentStock.ToString();
					lblNewQuantity.Text = StringsHelper.Format(currentStock * Double.Parse(currentQuantityPerUnit), "##,###.00") + currentUnit;
					lblCurrentQuantity.Text = StringsHelper.Format(currentStock * Double.Parse(currentQuantityPerUnit), "##,###.00") + currentUnit;
					codeGeneratedChange = false;
					setEmpty = false;
					txtValues[0].Focus();
				}
			}
			if (setEmpty)
			{
				codeGeneratedChange = true;
				txtOriginalQuantity.Text = "";
				txtOriginalPrice.Text = "";
				txtStockID.Text = "";
				txtValues[0].Text = "";
				txtValues[1].Text = "";
				lblNewQuantity.Text = "";
				lblCurrentQuantity.Text = "";
				codeGeneratedChange = false;
			}
			editingData = false;

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
					changedStockPrice = Double.Parse(txtValues[0].Text);
				}
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!String.IsNullOrEmpty(txtValues[1].Text))
				{
					changedStock = Double.Parse(txtValues[1].Text);
				}
				switch(Index)
				{
					case 1 : 
						if (changedStock > currentStock)
						{
							changedStock = currentStock;
							modMain.LogStatus("Cannot pass the original stock, to add more, add a new stock manually", this);
							txtValues[1].Text = changedStock.ToString();
						} 
						break;
				}
				lblNewQuantity.Text = StringsHelper.Format(changedStock * Double.Parse(currentQuantityPerUnit), "##,###.00") + currentUnit;
				lblCurrentQuantity.Text = StringsHelper.Format(currentStock * Double.Parse(currentQuantityPerUnit), "##,###.00") + currentUnit;
				codeGeneratedChange = false;
			}
		}

		private void ClearFields()
		{
			codeGeneratedChange = true;
			txtValues[0].Text = "";
			txtValues[1].Text = "";
			txtCode.Text = "";
			txtName.Text = "";
			txtUnit.Text = "";
			txtStockID.Text = "";
			txtOriginalPrice.Text = "";
			txtOriginalQuantity.Text = "";
			txtProductName.Text = "";
			txtQuantityPerUnit.Text = "";
			lvProducts.Items.Clear();
			lvStocks.Items.Clear();
			lblCurrentQuantity.Text = "";
			lblNewQuantity.Text = "";
			txtCode.Focus();
			editingData = false;
			codeGeneratedChange = false;
			modMain.ClearLogStatus(this);
		}
		private void frmAdjustStockManual_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}