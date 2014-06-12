using UpgradeHelpers.VB6.DB.ADO;
using System;
using System.Data;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmReceptionApproval
		: System.Windows.Forms.Form
	{

		private string Id = String.Empty;


		private void cmbStatus_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
		{
			DoSearchReception();
		}

		private void cmdApprove_Click(Object eventSender, EventArgs eventArgs)
		{
			LoadActionOrderReception(1);
		}

		private void cmdCancel_Click(Object eventSender, EventArgs eventArgs)
		{
			LoadActionOrderReception(2);
		}

		private void cmdInfo_Click(Object eventSender, EventArgs eventArgs)
		{
			LoadActionOrderReception();
		}

		private void LoadActionOrderReception(int Action = 0)
		{
			int OrderId = 0;
			if (fgOrders.CurrentRowIndex > 0)
			{
				OrderId = Convert.ToInt32(Double.Parse(Convert.ToString(fgOrders[fgOrders.CurrentRowIndex, 1].Value)));
				frmActionOrderReception.DefInstance.OrderId = OrderId;
				frmActionOrderReception.DefInstance.Action = Action;
				frmActionOrderReception.DefInstance.ShowDialog();
			}
		}

		private void dtFrom_ValueChanged(Object eventSender, EventArgs eventArgs)
		{
			chkFrom.CheckState = CheckState.Checked;
			DoSearchReception();
		}

		private void dtTo_ValueChanged(Object eventSender, EventArgs eventArgs)
		{
			chkTo.CheckState = CheckState.Checked;
			DoSearchReception();
		}


		private void fgOrders_DoubleClick(Object eventSender, EventArgs eventArgs)
		{
			cmdInfo_Click(cmdInfo, new EventArgs());
		}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load event and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
		private void frmReceptionApproval_Load(Object eventSender, EventArgs eventArgs)
		{
			InitGrid();
		}

		private void txtOrderID_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			DoSearchReception();
		}

		private void txtProductID_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			DoSearchReception();
		}

		private void txtProviderName_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			DoSearchReception();
		}

		private void txtContactLastName_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			DoSearchReception();
		}

		private void txtContactName_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			DoSearchReception();
		}

		//UPGRADE_NOTE: (7001) The following declaration (txtName_Change) seems to be dead code More Information: http://www.vbtonet.com/ewis/ewi7001.aspx
		//private void txtName_Change()
		//{
				//DoSearchReception();
		//}

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
			DoSearchReception(frmProviders.DefInstance.CurrentProviderID);
		}

		private void DoSearchReception(int Id = -1)
		{
			string filter = "";
			if (Id != -1)
			{
				filter = "o.ProviderID = " + Id.ToString();
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(txtProviderName.Text))
			{
				modFunctions.AppendAND(ref filter);
				filter = "p.ProviderName LIKE '%" + txtProviderName.Text + "%'";
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(txtContactName.Text))
			{
				modFunctions.AppendAND(ref filter);
				filter = filter + "p.ContactFirstName LIKE '%" + txtContactName.Text + "%'";
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(txtContactLastName.Text))
			{
				modFunctions.AppendAND(ref filter);
				filter = filter + "p.ContactLastName LIKE '%" + txtContactLastName.Text + "%'";
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(txtOrderID.Text))
			{
				modFunctions.AppendAND(ref filter);
				filter = filter + "o.OrderID = " + txtOrderID.Text;
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(txtProductID.Text))
			{
				modFunctions.AppendAND(ref filter);
				filter = filter + "d.ProductID LIKE '%" + txtProductID.Text + "%'";
			}
			if (chkFrom.CheckState == CheckState.Checked)
			{
				modFunctions.AppendAND(ref filter);
				filter = filter + "o.OrderDate >= #" + Convert.ToDateTime(dtFrom.Value).ToString("MM/dd/yyyy") + "#";
			}
			if (chkTo.CheckState == CheckState.Checked)
			{
				modFunctions.AppendAND(ref filter);
				filter = filter + "o.OrderDate <= #" + Convert.ToDateTime(dtTo.Value).ToString("MM/dd/yyyy") + "#";
			}
			if (cmbStatus.SelectedIndex != -1 && cmbStatus.Text != "All")
			{
				modFunctions.AppendAND(ref filter);
				filter = filter + "o.Status = '" + cmbStatus.Text + "'";
			}

			string where = " Where o.OrderID = d.OrderID And p.ProviderID = o.ProviderID And u.Username = o.ReceivedBy ";
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(filter))
			{
				filter = where + " AND " + filter;
			}
			else
			{
				filter = where;
			}


			string sql = "Select o.OrderDate, o.OrderID, p.ProviderName, p.ContactFirstName + ' ' + p.ContactLastName as ContactName, u.Fullname as [Received by], Sum(d.LineTotal) as Price, o.Status " + 
			             "From OrderReceptions as o, OrderReceptionDetails as d, Providers as p, Users as u " + 
			             filter + " Group by o.orderDate, o.OrderID, p.ProviderName, p.ContactFirstName + ' ' + p.ContactLastName, u.Fullname, o.Status ";
			modConnection.ExecuteSql(sql);
			modMain.LogStatus("There are " + modConnection.rs.RecordCount.ToString() + " records with the selected criteria", this);
			int i = 0;
			fgOrders.RowsCount = Convert.ToInt32(modConnection.rs.RecordCount + 1);
			fgOrders.FixedRows = (fgOrders.RowsCount == 1) ? 0 : 1;
			i = 1;
			foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
			{
				for (int j = 0; j <= modConnection.rs.FieldsMetadata.Count - 1; j++)
				{
					if (modConnection.rs.GetField(j) != null)
					{
						fgOrders[i, j].Value = Convert.ToString(iteration_row[j]);
					}
				}
				i++;
			}
		}

		private void InitGrid()
		{
			fgOrders.RowsCount = 0;
			fgOrders.ColumnsCount = 7;
			fgOrders.FixedColumns = 0;
			fgOrders.AddItem("Date" + "\t" + "Order" + "\t" + "Supplier" + "\t" + "Contact" + "\t" + "Received by" + "\t" + "Price" + "\t" + "Status");
			fgOrders.RowsCount = 1;
			fgOrders.FixedRows = 0;
		}



		//UPGRADE_NOTE: (7001) The following declaration (MakeTextBoxVisible) seems to be dead code More Information: http://www.vbtonet.com/ewis/ewi7001.aspx
		//private void MakeTextBoxVisible(TextBox txtBox, UpgradeHelpers.Windows.Forms.DataGridViewFlex grid)
		//{
				//txtBox.Text = Convert.ToString(grid[grid.CurrentRowIndex, grid.CurrentColumnIndex].Value);
				//txtBox.SetBounds(Convert.ToInt32((float) (grid.CellLeft / 15 + grid.Left)), Convert.ToInt32((float) (grid.CellTop / 15 + grid.Top)), Convert.ToInt32((float) (grid.CellWidth / 15)), Convert.ToInt32((float) (grid.CellHeight / 15)));
				//txtBox.Visible = true;
				//Application.DoEvents();
				//txtBox.Focus();
		//}
		private void frmReceptionApproval_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}