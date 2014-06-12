using UpgradeHelpers.VB6.DB.ADO;
using UpgradeHelpers.VB6.Gui;
using UpgradeHelpers.VB6.Utils;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmSales
		: System.Windows.Forms.Form
	{


		private void cboCashier_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
		{
			ViewSales("ProductId", "%");
		}

		private void cboMonth_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
		{
			ViewSales("ProductId", "%");
		}

		private void cboYear_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
		{
			ViewSales("ProductId", "%");
		}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load event and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
		private void frmSales_Load(Object eventSender, EventArgs eventArgs)
		{
			for (modMain.i = Convert.ToInt32(Double.Parse(DateTime.Today.ToString("yyyy"))); modMain.i >= 2000; modMain.i--)
			{
				cboYear.AddItem(modMain.i.ToString());
			}
			cboYear.Text = Conversion.Val(DateTime.Today.ToString("yyyy")).ToString();
			cboMonth.SelectedIndex = Convert.ToInt32(Conversion.Val(DateTime.Today.ToString("MM")));
			//SetListView lvwSales, True, True
			string tempRefParam = "id";
			modFunctions.LoadCombo("Users", cboCashier, ref tempRefParam);
			//frmMain.tbrMenu.Buttons(4).Value = tbrPressed
		}

		private bool isInitializingComponent;
		private void frmSales_Resize(Object eventSender, EventArgs eventArgs)
		{
			if (isInitializingComponent)
			{
				return;
			}
			lvwSales.Width = (int) (ClientRectangle.Width - (lvwSales.Left + 7));
			lvwSales.Height = (int) (ClientRectangle.Height - (lvwSales.Top + 7));
			ctrLine.Width = (int) (ClientRectangle.Width - ctrLine.Left);
		}

		private void frmSales_Closed(Object eventSender, EventArgs eventArgs)
		{
			//frmMain.tbrMenu.Buttons(4).Value = tbrUnpressed
		}

		public void ViewSales(string RcrdFld, string RcrdStr)
		{
			if (cboCashier.SelectedIndex == 0)
			{
				modConnection.ExecuteSql("Select record_no, ProductId, description, gross_amount, net_amount, vat, quantity from tblSales where " + RcrdFld + " LIKE '" + RcrdStr + "%' and format(date_sold, 'm') = " + 
				                         cboMonth.SelectedIndex.ToString() + " and format(date_sold,'yyyy') = " + cboYear.Text + " Order by record_no ASC");
			}
			else
			{
				modConnection.ExecuteSql("Select record_no, ProductId, description, gross_amount, net_amount, vat, quantity from tblSales where " + RcrdFld + " LIKE '" + RcrdStr + "%' and format(date_sold, 'm') = " + 
				                         cboMonth.SelectedIndex.ToString() + " and format(date_sold,'yyyy') = " + cboYear.Text + " and cashier_id = '" + cboCashier.Text + "' Order by record_no ASC");
			}
			lvwSales.Items.Clear();
			ListViewItem x = null;
			foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
			{
				x = (ListViewItem) lvwSales.Items.Add(Convert.ToString(iteration_row[0]));
				for (modMain.i = 1; modMain.i <= (modConnection.rs.FieldsMetadata.Count - 1); modMain.i++)
				{
					ListViewHelper.GetListViewSubItem(x, modMain.i).Text = Convert.ToString(iteration_row[modMain.i]);
				}
			}
			modConnection.ExecuteSql("Select format(sum(net_amount),'#0.00') from tblSales where format(date_sold, 'm') = " + cboMonth.SelectedIndex.ToString() + " and format(date_sold,'yyyy') = " + cboYear.Text);
			lblTotalSales.Text = "P " + Conversion.Val(StringsHelper.Format(modConnection.rs[0], "#,##0.00")).ToString();

			modConnection.ExecuteSql("Select ProductId from tblSales where quantity = (Select max(quantity) from tblSales) and format(date_sold, 'm') = " + cboMonth.SelectedIndex.ToString() + " and format(date_sold,'yyyy') = " + cboYear.Text);
			lblSellable.Text = Convert.ToString(modConnection.rs[0]);
		}
	}
}