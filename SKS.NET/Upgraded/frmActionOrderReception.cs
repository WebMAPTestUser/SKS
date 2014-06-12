using UpgradeHelpers.VB6.DB.ADO;
using UpgradeHelpers.VB6.Utils;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmActionOrderReception
		: System.Windows.Forms.Form
	{


		private double currentSubTotal = 0;
		private double currentTotal = 0;
		private double currentTax = 0;
		private double currentFreightCharge = 0;
		private double currentTotalTax = 0;

		public int Action = 0;

		public int OrderId = 0;

		private void cmdApprove_Click(Object eventSender, EventArgs eventArgs)
		{
			try
			{
				if (txtStatus.Text.ToUpper() == "APPROVED")
				{
					modMain.LogStatus("Order is already approved, not need to be approved again", this);
					return;
				}

				if (txtStatus.Text.ToUpper() == "CANCELLED")
				{
					modMain.LogStatus("Order was already approved by " + txtChangedBy.Text + " on " + txtChanged.Text + ", it cannot be approved", this);
					return;
				}


				// UPDATE
				modConnection.ExecuteSql("Update OrderReceptions Set Status = 'APPROVED', ChangedBy = '" + modMain.UserId + "', ChangedDate = #" + DateTimeHelper.ToString(DateTime.Today) + "#" + 
				                         " Where OrderId = " + OrderId.ToString());

				modConnection.ExecuteSql("Select ProductId, Quantity, UnitPrice, LineTotal " + 
				                         "From OrderReceptionDetails Where OrderID = " + OrderId.ToString());


				foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
				{

					modConnection.ExecuteSql2("Insert Into Stocks " + 
					                          "(ProductID, Stock, InitialStock, DateStarted, DateModified, User, UnitPrice, StockPrice) Values " + 
					                          "('" + Convert.ToString(iteration_row["ProductId"]) + "'," + Convert.ToString(iteration_row["Quantity"]) + "," + Convert.ToString(iteration_row["Quantity"]) + ", #" + DateTimeHelper.ToString(DateTime.Today) + "#, #" + DateTimeHelper.ToString(DateTime.Today) + "#, '" + modMain.UserId + "', " + Convert.ToString(iteration_row["UnitPrice"]) + "," + Convert.ToString(iteration_row["LineTotal"]) + ")");

					modConnection.ExecuteSql2("Select Max(StockID) as NewId From Stocks");
					int newId = 0;
					newId = Convert.ToInt32(modConnection.rs2["NewId"]);

					modConnection.ExecuteSql2("Insert Into StockLogs " + 
					                          "(DocID, DocType, StockID, ProductId, Quantity, StockPrice, Date, User) Values " + 
					                          "(" + Convert.ToString(iteration_row["ProductId"]) + "," + Convert.ToString(iteration_row["ProductId"]) + "," + "," + Convert.ToString(iteration_row["ProductId"]) + "," + "," + Convert.ToString(iteration_row["ProductId"]) + "," + "," + Convert.ToString(iteration_row["ProductId"]) + "," + "," + Convert.ToString(iteration_row["ProductId"]) + ",#" + DateTimeHelper.ToString(DateTime.Today) + "#, '" + modMain.UserId + "')");

				}


				modConnection.ExecuteSql("Insert Into Stocks " + 
				                         "(ProductID, Stock, InitialStock, DateStarted, DateModified, User, UnitPrice, StockPrice) " + 
				                         "Select ProductId, Quantity, Quantity, #" + DateTimeHelper.ToString(DateTime.Today) + "#, #" + DateTimeHelper.ToString(DateTime.Today) + "#, '" + modMain.UserId + "', UnitPrice, LineTotal " + 
				                         "From OrderReceptionDetails " + 
				                         "Where OrderID = " + OrderId.ToString());

				modConnection.ExecuteSql("Update Products as p Set UnitsInStock = UnitsInStock + " + 
				                         " ( Select Sum(Quantity) From OrderReceptionDetails Where OrderId = " + OrderId.ToString() + " and ProductId = p.ProductId) " + 
				                         " Where ProductId in Select ProductId From OrderReceptionDetails Where OrderId = " + OrderId.ToString());
			}
			catch (System.Exception excep)
			{
				//UPGRADE_WARNING: (2081) Err.Number has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2081.aspx
				MessageBox.Show("An error has occurred adding the data. Error: (" + Information.Err().Number.ToString() + ") " + excep.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

		private void cmdCancel_Click(Object eventSender, EventArgs eventArgs)
		{
			try
			{
				if (txtStatus.Text.ToUpper() == "CANCELLED")
				{
					modMain.LogStatus("Order was already cancelled, not need to be cancelled again", this);
					return;
				}
				if (txtStatus.Text.ToUpper() == "APPROVED")
				{
					modMain.LogStatus("Order was already cancelled by " + txtChangedBy.Text + " on " + txtChanged.Text + ", it cannot be canceled", this);
					return;
				}


				if (MessageBox.Show("Do you want to cancel the order reception?", "Confirm cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
				{
					return;
				}

				// UPDATE
				modConnection.ExecuteSql("Update OrderReceptions Set Status = 'CANCELLED', ChangedBy = '" + modMain.UserId + "', ChangedDate = #" + DateTimeHelper.ToString(DateTime.Today) + "#" + 
				                         " Where OrderId = " + OrderId.ToString());

				LoadData();
				MessageBox.Show("The order was successfully cancelled", Application.ProductName);
				this.Close();
			}
			catch (System.Exception excep)
			{
				//UPGRADE_WARNING: (2081) Err.Number has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2081.aspx
				MessageBox.Show("An error has occurred adding the data. Error: (" + Information.Err().Number.ToString() + ") " + excep.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load event and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
		private void frmActionOrderReception_Load(Object eventSender, EventArgs eventArgs)
		{
			LoadData();
			if (Action != 0)
			{

				switch((Action))
				{
					case 1 : 
						cmdApprove_Click(cmdApprove, new EventArgs()); 
						break;
					case 2 : 
						cmdCancel_Click(cmdCancel, new EventArgs()); 
						break;
				}
			}
		}

		private void LoadData()
		{
			currentSubTotal = 0;
			currentTotalTax = 0;
			modConnection.ExecuteSql("Select o.OrderDate, u.Fullname, o.Status, p.ProviderName, p.ContactFirstName + ' ' + p.ContactLastName as Contact, o.ChangedDate, o.ChangedBy, o.FreightCharge, o.SalesTaxRate, o.Notes " + 
			                         "From OrderReceptions as o, Users as u, Providers as p " + 
			                         "Where o.OrderID = " + OrderId.ToString() + " And u.Username = o.ReceivedBy And p.ProviderId = o.ProviderId");
			if (modConnection.rs.EOF)
			{
				modMain.LogStatus("The order with the ID '" + OrderId.ToString() + "' does not exist", this);
				return;
			}
			txtOrderID.Text = OrderId.ToString();
			txtReceived.Text = Convert.ToString(modConnection.rs["OrderDate"]);
			txtReceivedBy.Text = Convert.ToString(modConnection.rs["Fullname"]);
			//UPGRADE_WARNING: (6021) Casting 'Variant' to Enum may cause different behaviour. More Information: http://www.vbtonet.com/ewis/ewi6021.aspx
			if (((VariantType) Convert.ToInt32(modConnection.rs["Notes"])) != VariantType.Null)
			{
				txtNotes.Text = Convert.ToString(modConnection.rs["Notes"]);
			}
			txtFreightCharge.Text = Convert.ToString(modConnection.rs["FreightCharge"]);
			currentFreightCharge = Convert.ToDouble(modConnection.rs["FreightCharge"]);
			txtSalesTax.Text = Convert.ToString(modConnection.rs["SalesTaxRate"]);
			currentTax = Convert.ToDouble(modConnection.rs["SalesTaxRate"]);
			txtProviderCompany.Text = Convert.ToString(modConnection.rs["ProviderName"]);
			txtProviderContact.Text = Convert.ToString(modConnection.rs["Contact"]);
			txtStatus.Text = Convert.ToString(modConnection.rs["Status"]);
			//UPGRADE_WARNING: (6021) Casting 'Variant' to Enum may cause different behaviour. More Information: http://www.vbtonet.com/ewis/ewi6021.aspx
			if (((VariantType) Convert.ToInt32(modConnection.rs["ChangedDate"])) != VariantType.Null)
			{
				txtChanged.Text = Convert.ToString(modConnection.rs["ChangedDate"]);
			}
			//UPGRADE_WARNING: (6021) Casting 'Variant' to Enum may cause different behaviour. More Information: http://www.vbtonet.com/ewis/ewi6021.aspx
			if (((VariantType) Convert.ToInt32(modConnection.rs["ChangedBy"])) != VariantType.Null)
			{
				txtChangedBy.Text = Convert.ToString(modConnection.rs["ChangedBy"]);
			}

			bool isReceived = txtStatus.Text == "RECEIVED";
			lblChanged.Visible = !isReceived;
			lblChangedBy.Visible = !isReceived;
			txtChanged.Visible = !isReceived;
			txtChangedBy.Visible = !isReceived;
			cmdApprove.Enabled = true; // Received
			cmdCancel.Enabled = true; // Received

			if (txtStatus.Text == "APPROVED")
			{
				lblChanged.Text = "Approved Date:";
				lblChangedBy.Text = "Approved By:";
			}
			else
			{
				lblChanged.Text = "Cancelled Date:";
				lblChangedBy.Text = "Cancelled By:";
			}
			LoadDetails();
			DisplayTotals();
		}

		private void DisplayTotals()
		{
			currentTotal = currentFreightCharge + currentSubTotal + currentTotalTax;
			txtSubTotal.Text = StringsHelper.Format(currentSubTotal, "#,##0.00");
			txtTotalTax.Text = StringsHelper.Format(currentTotalTax, "#,##0.00");
			txtTotal.Text = StringsHelper.Format(currentTotal, "#,##0.00");
		}


		private void AddToTotals(double current)
		{
			currentSubTotal += current;
			currentTotalTax = currentSubTotal * currentTax;
			currentTotal = currentFreightCharge + currentSubTotal + currentTotalTax;
			txtSubTotal.Text = StringsHelper.Format(currentSubTotal, "#,##0.00");
			txtTotalTax.Text = StringsHelper.Format(currentTotalTax, "#,##0.00");
			txtTotal.Text = StringsHelper.Format(currentTotal, "#,##0.00");
		}


		private void cmdClose_Click(Object eventSender, EventArgs eventArgs)
		{
			this.Close();
		}

		private void LoadDetails()
		{

			modConnection.ExecuteSql("Select d.Quantity, p.ProductID, p.ProductName, d.UnitPrice, d.SalePrice, p.UnitsInStock, p.UnitsOnOrder, Str(p.QuantityPerUnit) + p.Unit, d.LineTotal From Products as p, OrderReceptionDetails as d " + 
			                         "Where d.OrderID = " + OrderId.ToString() + " And d.ProductId = p.ProductId");

			int lng = 0;
			int intLoopCount = 0;
			int i = 0;
			fgDetails.RowsCount = 0;
			fgDetails.ColumnsCount = 9;
			fgDetails.FixedColumns = 0;
			fgDetails.AddItem("Quantity" + "\t" + "Code" + "\t" + "Product" + "\t" + "UnitPrice" + "\t" + "Price" + "\t" + "Existence" + "\t" + "Ordered" + "\t" + "Quantity per unit" + "\t" + "Line Total");
			fgDetails.RowsCount = Convert.ToInt32(modConnection.rs.RecordCount + 1);
			fgDetails.FixedRows = (fgDetails.RowsCount == 1) ? 0 : 1;
			i = 1;
			foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
			{
				int tempForVar = modConnection.rs.FieldsMetadata.Count;
				for (int j = 1; j <= tempForVar; j++)
				{
					//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
					if (modConnection.rs.GetField(i) != null)
					{
						fgDetails[i, j - 1].Value = Convert.ToString(iteration_row[j - 1]);
					}
				}
				AddToTotals(Convert.ToDouble(iteration_row["LineTotal"]));
				i++;
			}

		}
		private void frmActionOrderReception_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}