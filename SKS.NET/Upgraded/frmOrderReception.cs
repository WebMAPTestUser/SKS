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
	internal partial class frmOrderReception
		: System.Windows.Forms.Form
	{

		private string currentProviderName = String.Empty;
		private int currentIdProvider = 0;
		private string currentContactName = String.Empty;
		private bool editingData = false;

		private double currentSubTotal = 0;
		private double currentTotal = 0;
		private double currentTax = 0;
		private double currentFreightCharge = 0;
		private double currentTotalTax = 0;

		private void cmdAddProducts_Click(Object eventSender, EventArgs eventArgs)
		{
			frmAddProductTo.DefInstance.Id = currentIdProvider;
			frmAddProductTo.DefInstance.ObjectReferred = "Provider " + txtProviderCompany.Text + "|" + txtProviderContact.Text;
			frmAddProductTo.DefInstance.Table = "ProductsByProvider";
			frmAddProductTo.DefInstance.ColumnName = "ProviderId";
			frmAddProductTo.DefInstance.ShowDialog();
			if (frmAddProductTo.DefInstance.SavedChanges)
			{
				LoadProductsById();
			}
		}

		//UPGRADE_NOTE: (7001) The following declaration (txtName_Change) seems to be dead code More Information: http://www.vbtonet.com/ewis/ewi7001.aspx
		//private void txtName_Change()
		//{
				//DoSearchProvider();
		//}

		private void DoSearchProvider(int Id = 0)
		{
			string filter = "";
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!Id.Equals(0))
			{
				filter = "ProviderID = " + Id.ToString();
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(txtProviderName.Text))
			{
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!String.IsNullOrEmpty(filter))
				{
					filter = filter + " AND ";
				}
				filter = "ProviderName LIKE '%" + txtProviderName.Text + "%'";
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(txtContactName.Text))
			{
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!String.IsNullOrEmpty(filter))
				{
					filter = filter + " AND ";
				}
				filter = filter + "ContactFirstName LIKE '%" + txtContactName.Text + "%'";
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(txtContactLastName.Text))
			{
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!String.IsNullOrEmpty(filter))
				{
					filter = filter + " AND ";
				}
				filter = filter + "ContactLastName LIKE '%" + txtContactLastName.Text + "%'";
			}

			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(filter))
			{
				filter = "Where " + filter;
			}
			modConnection.ExecuteSql("Select ProviderID, ProviderName, ContactFirstName, ContactLastName, City, StateOrProvince, 'Country/Region' From Providers " + filter);
			lvProviders.Items.Clear();
			ListViewItem x = null;
			if (modConnection.rs.RecordCount == 0)
			{
				modMain.LogStatus("There are no records with the selected criteria", this);
			}
			else
			{
				foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
				{
					x = (ListViewItem) lvProviders.Items.Add(Convert.ToString(iteration_row[0]));
					for (modMain.i = 1; modMain.i <= (modConnection.rs.FieldsMetadata.Count - 1); modMain.i++)
					{
						//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
						if (modConnection.rs.GetField(modMain.i) != null)
						{
							ListViewHelper.GetListViewSubItem(x, modMain.i).Text = Convert.ToString(iteration_row[modMain.i]);
						}
					}
				}
				if (lvProviders.Items.Count == 1)
				{
					lvProviders.Items[0].Selected = true;
				}
			}
		}

		private void cmdClose_Click(Object eventSender, EventArgs eventArgs)
		{
			this.Close();
		}

		private void cmdProviders_Click(Object eventSender, EventArgs eventArgs)
		{
			frmProviders.DefInstance.ShowDialog();
			txtProviderName.Text = "";
			txtContactLastName.Text = "";
			txtContactName.Text = "";
			DoSearchProvider(frmProviders.DefInstance.CurrentProviderID);
		}

		private void cmdSave_Click(Object eventSender, EventArgs eventArgs)
		{
			int newOrderId = 0;

			try
			{
				modConnection.ExecuteSql("Select * from OrderReceptions");
				modConnection.rs.AddNew();
				modConnection.rs["ProviderId"] = currentIdProvider;
				modConnection.rs["ReceivedBy"] = modMain.UserId;
				modConnection.rs["OrderDate"] = DateTime.Today;
				modConnection.rs["Notes"] = txtNotes.Text;
				modConnection.rs["FreightCharge"] = currentFreightCharge;
				modConnection.rs["SalesTaxRate"] = currentTax * 0.01d;
				modConnection.rs["Status"] = "RECEIVED";
				modConnection.rs.Update();
				newOrderId = Convert.ToInt32(modConnection.rs["OrderID"]);


				for (modMain.i = 1; modMain.i <= fgProducts.RowsCount - 1; modMain.i++)
				{
					if (Convert.ToString(fgProducts[modMain.i, 0].Value) != "0")
					{
						modConnection.ExecuteSql("Insert into OrderReceptionDetails (OrderID, ProductID, DateSold, Quantity, UnitPrice, SalePrice, SalesTax, LineTotal) Values (" + newOrderId.ToString() + ", '" + Convert.ToString(fgProducts[modMain.i, 1].Value) + "', '" + DateTime.Today.ToString("dd/MM/yyyy") + "'," + Convert.ToString(fgProducts[modMain.i, 0].Value) + "," + Convert.ToString(fgProducts[modMain.i, 3].Value) + "," + Convert.ToString(fgProducts[modMain.i, 4].Value) + "," + (currentTax * 0.01d).ToString() + "," + Convert.ToString(fgProducts[modMain.i, 4].Value) + ")");

						//UnitsInTransit
						//ExecuteSql "Update Products Set UnitsOnOrder = UnitsOnOrder + " & fgProducts.TextMatrix(i, 0) & _
						//'" Where ProductId = '" & fgProducts.TextMatrix(i, 1) & "'"

					}
				}



				editingData = false;
				if (MessageBox.Show("Order reception added successfully" + Environment.NewLine + "Would you like to add a new order reception?", "New data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
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

		private void MakeTextBoxVisible(TextBox txtBox, UpgradeHelpers.Windows.Forms.DataGridViewFlex grid)
		{
			txtBox.Text = Convert.ToString(grid[grid.CurrentRowIndex, grid.CurrentColumnIndex].Value);
			txtBox.SetBounds(Convert.ToInt32((float) (grid.CellLeft / 15 + grid.Left)), Convert.ToInt32((float) (grid.CellTop / 15 + grid.Top)), Convert.ToInt32((float) (grid.CellWidth / 15)), Convert.ToInt32((float) (grid.CellHeight / 15)));
			txtBox.Visible = true;
			Application.DoEvents();
			txtBox.Focus();
			modFunctions.SelectAll(txtBox);
		}

		private void fgProducts_Click(Object eventSender, EventArgs eventArgs)
		{
			if (fgProducts.CurrentColumnIndex != 0)
			{
				return;
			}
			MakeTextBoxVisible(txtEntry, fgProducts);
		}

		//UPGRADE_WARNING: (2050) MSFlexGridLib.MSFlexGrid Event fgProducts.EnterCell was not upgraded. More Information: http://www.vbtonet.com/ewis/ewi2050.aspx
		private void fgProducts_EnterCell()
		{
			SaveEdits();
		}

		private void fgProducts_KeyPress(Object eventSender, KeyPressEventArgs eventArgs)
		{
			int KeyAscii = Strings.Asc(eventArgs.KeyChar);
			if (fgProducts.CurrentColumnIndex != 0)
			{
				if (KeyAscii == 0)
				{
					eventArgs.Handled = true;
				}
				return;
			}
			if (KeyAscii == 46 || KeyAscii >= 48 && KeyAscii <= 57)
			{
				//Case 45, 46, 47, 48 To 59, 65 To 90, 97 To 122
				MakeTextBoxVisible(txtEntry, fgProducts);
				txtEntry.Text = Strings.Chr(KeyAscii).ToString();
				txtEntry.SelectionStart = 1;
			}
			else
			{
			}
			if (KeyAscii == 0)
			{
				eventArgs.Handled = true;
			}
			eventArgs.KeyChar = Convert.ToChar(KeyAscii);
		}

		private void txtEntry_KeyDown(Object eventSender, KeyEventArgs eventArgs)
		{
			int KeyCode = (int) eventArgs.KeyCode;
			int Shift = (eventArgs.Shift) ? 1 : 0;
			EditKeyCode(fgProducts, ref txtEntry, KeyCode, Shift);
		}

		private void EditKeyCode(UpgradeHelpers.Windows.Forms.DataGridViewFlex grid, ref TextBox txtBox, int KeyCode, int Shift)
		{
			switch(KeyCode)
			{
				case 27 :  //ESC 
					txtBox.Text = ""; 
					txtBox.Visible = false; 
					grid.Focus(); 
					break;
				case 13 :  //Return 
					grid.Focus(); 
					break;
				case 37 :  //Left Arrow 
					grid.Focus(); 
					Application.DoEvents(); 
					if (grid.CurrentColumnIndex > grid.FixedColumns)
					{
						grid.CurrentColumnIndex--;
					} 
					break;
				case 38 :  //Up Arrow 
					grid.Focus(); 
					Application.DoEvents(); 
					if (grid.CurrentRowIndex > grid.FixedRows)
					{
						grid.CurrentRowIndex--;
					} 
					break;
				case 39 :  //Right Arrow 
					grid.Focus(); 
					Application.DoEvents(); 
					if (grid.CurrentColumnIndex < grid.ColumnsCount - 1)
					{
						grid.CurrentColumnIndex++;
					} 
					break;
				case 40 :  //Down Arrow 
					grid.Focus(); 
					Application.DoEvents(); 
					if (grid.CurrentRowIndex < grid.RowsCount - 1)
					{
						grid.CurrentRowIndex++;
					} 
					break;
			}
		}

		private void txtEntry_Leave(Object eventSender, EventArgs eventArgs)
		{
			SaveEdits();
		}


		private void fgProducts_CellLeave(Object eventSender, EventArgs eventArgs)
		{
			SaveEdits();
		}

		private void txtEntry_KeyPress(Object eventSender, KeyPressEventArgs eventArgs)
		{
			int KeyAscii = Strings.Asc(eventArgs.KeyChar);
			if (KeyAscii == 46 || KeyAscii >= 48 && KeyAscii <= 57)
			{
				//Alphanumeric
				//Case 45, 46, 47, 48 To 59, 65 To 90, 97 To 122
			}
			else
			{
				KeyAscii = 0;
			}
			if (KeyAscii == 0)
			{
				eventArgs.Handled = true;
			}
			eventArgs.KeyChar = Convert.ToChar(KeyAscii);
		}

		private void SaveEdits()
		{
			if (!txtEntry.Visible)
			{
				return;
			}
			double previousLinePrice = modFunctions.DoubleValue(Convert.ToString(fgProducts[fgProducts.CurrentRowIndex, 4].Value));
			fgProducts[fgProducts.CurrentRowIndex, fgProducts.CurrentColumnIndex].Value = txtEntry.Text;
			double lineQuantity = modFunctions.DoubleValue(txtEntry.Text);
			double lineUnitPrice = modFunctions.DoubleValue(Convert.ToString(fgProducts[fgProducts.CurrentRowIndex, 3].Value));
			previousLinePrice = modFunctions.DoubleValue(Convert.ToString(fgProducts[fgProducts.CurrentRowIndex, 4].Value));
			double linePrice = lineQuantity * lineUnitPrice;
			fgProducts[fgProducts.CurrentRowIndex, 4].Value = linePrice.ToString();
			ReCalculateTotals(previousLinePrice, linePrice);
			txtEntry.Visible = false;
			editingData = true;
		}

		private void ReCalculateTotals(double previous, double current)
		{
			currentSubTotal = currentSubTotal - previous + current;
			currentTotalTax = currentSubTotal * currentTax * 0.01d;
			currentTotal = currentFreightCharge + currentSubTotal + currentTotalTax;
			txtSubTotal.Text = StringsHelper.Format(currentSubTotal, "#,##0.00");
			txtTotalTax.Text = StringsHelper.Format(currentTotalTax, "#,##0.00");
			txtTotal.Text = StringsHelper.Format(currentTotal, "#,##0.00");
		}

		private void frmOrderReception_FormClosing(Object eventSender, FormClosingEventArgs eventArgs)
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

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load event and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
		private void frmOrderReception_Load(Object eventSender, EventArgs eventArgs)
		{
			editingData = false;
			ClearFields();
		}

		private void lvProviders_ItemClick(ListViewItem Item)
		{
			RetrieveDataProvider();
		}

		private void RetrieveDataProvider()
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
			if (lvProviders.FocusedItem != null)
			{
				withVar = lvProviders.FocusedItem;
				currentIdProvider = Convert.ToInt32(Double.Parse(lvProviders.FocusedItem.Text));
				currentProviderName = ListViewHelper.GetListViewSubItem(withVar, 1).Text;
				currentContactName = ListViewHelper.GetListViewSubItem(withVar, 2).Text + " " + ListViewHelper.GetListViewSubItem(withVar, 3).Text;
				txtProviderCompany.Text = currentProviderName;
				txtProviderContact.Text = currentContactName;
				editingData = false;
			}
			LoadProductsById();
			cmdSave.Enabled = true;
			cmdAddProducts.Enabled = true;

		}

		private void LoadProductsById()
		{
			string Table = "ProductsByProvider";
			string ColumnName = "ProviderId";
			int Id = currentIdProvider;

			modConnection.ExecuteSql("Select p.ProductID, p.ProductName, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.QuantityPerUnit, p.Unit from Products as p, " + Table + " as pb Where pb." + ColumnName + " = " + Id.ToString() + " And pb.ProductId = p.ProductId");

			//lvProducts.ListItems.Clear
			//If rs.RecordCount > 0 Then
			//    With rs
			//        While Not .EOF
			//            Set x = lvProducts.ListItems.Add(, , 0)
			//            For i = 1 To 5
			//                If Not IsEmpty(.Fields(i - 1)) Then
			//                    x.SubItems(i) = .Fields(i - 1)
			//                End If
			//            Next i
			//            x.SubItems(6) = .Fields(5) & .Fields(6)
			//            .MoveNext
			//        Wend
			//    End With
			//End If

			int lng = 0;
			int intLoopCount = 0;
			const int SCROOL_WIDTH = 320;
			int i = 0;
			fgProducts.ColumnsCount = 8;
			fgProducts.FixedColumns = 0;
			fgProducts.RowsCount = 0;
			fgProducts.AddItem("Quantity" + "\t" + "Code" + "\t" + "Product" + "\t" + "UnitPrice" + "\t" + "Price" + "\t" + "Existence" + "\t" + "Ordered" + "\t" + "Quantity per unit");
			fgProducts.RowsCount = Convert.ToInt32(modConnection.rs.RecordCount + 1);
			fgProducts.FixedRows = (fgProducts.RowsCount == 1) ? 0 : 1;
			i = 1;
			foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
			{
				fgProducts[i, 0].Value = "0";
				for (int j = 1; j <= 6; j++)
				{
					if (j == 4)
					{
						fgProducts[i, j].Value = "0";
					}
					else if (j < 4)
					{ 
						fgProducts[i, j].Value = Convert.ToString(iteration_row[j - 1]);
					}
					else
					{
						fgProducts[i, j].Value = Convert.ToString(iteration_row[j - 2]);
					}
				}
				fgProducts[i, 7].Value = Convert.ToString(iteration_row[5]) + Convert.ToString(iteration_row[6]);
				i++;
			}

		}


		//UPGRADE_NOTE: (7001) The following declaration (lvProducts_ItemCheck) seems to be dead code More Information: http://www.vbtonet.com/ewis/ewi7001.aspx
		//private void lvProducts_ItemCheck(ListViewItem Item)
		//{
				//if (Item.Checked)
				//{
					//Item.Text = "1";
				//}
				//else
				//{
					//Item.Text = "0";
				//}
		//}


		private void txtProviderName_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			DoSearchProvider();
		}

		private void txtNotes_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			editingData = true;
		}

		private void txtContactName_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			DoSearchProvider();
		}

		private void ClearFields()
		{

			fgProducts.RowsCount = 0;
			fgProducts.ColumnsCount = 0;

			currentSubTotal = 0;
			currentTotal = 0;
			currentTax = 0;
			currentTotalTax = 0;
			currentFreightCharge = 0;

			txtSubTotal.Text = "";
			txtTotal.Text = "";
			txtTotalTax.Text = "";
			txtSalesTax.Text = "";
			txtFreightCharge.Text = "";

			txtProviderName.Text = "";
			txtContactLastName.Text = "";
			txtContactName.Text = "";
			txtProviderContact.Text = "";
			txtProviderCompany.Text = "";
			cmdSave.Enabled = false;
			cmdAddProducts.Enabled = false;
			txtNotes.Text = "";
			//txtProviderName.SetFocus
			ReCalculateTotals(0, 0);
			editingData = false;
		}

		private void txtFreightCharge_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			currentFreightCharge = modFunctions.DoubleValue(txtFreightCharge.Text);
			ReCalculateTotals(0, 0);
			editingData = true;
		}

		private void txtFreightCharge_KeyPress(Object eventSender, KeyPressEventArgs eventArgs)
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

		private void txtContactLastName_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			editingData = true;
		}


		private void txtSalesTax_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			currentTax = modFunctions.DoubleValue(txtSalesTax.Text);
			ReCalculateTotals(0, 0);
			editingData = true;
		}

		private void txtSalesTax_KeyPress(Object eventSender, KeyPressEventArgs eventArgs)
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
		private void frmOrderReception_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}