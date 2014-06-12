using Microsoft.VisualBasic.Compatibility.VB6;
using System;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmMain
		: System.Windows.Forms.Form
	{


		public void mnuAbout_Click(Object eventSender, EventArgs eventArgs)
		{
			frmAbout.DefInstance.ShowDialog(this);
		}

		public void mnuAddStockManually_Click(Object eventSender, EventArgs eventArgs)
		{
			frmAddStockManual.DefInstance.ShowDialog();
		}

		public void mnuAdjustStockManually_Click(Object eventSender, EventArgs eventArgs)
		{
			frmAdjustStockManual.DefInstance.ShowDialog();
		}

		public void mnuCategories_Click(Object eventSender, EventArgs eventArgs)
		{
			//frmCategories.Show vbModal
		}

		public void mnuCreateOrderReception_Click(Object eventSender, EventArgs eventArgs)
		{
			frmOrderReception.DefInstance.ShowDialog();
		}

		public void mnuCreateOrderRequest_Click(Object eventSender, EventArgs eventArgs)
		{
			frmOrderRequest.DefInstance.ShowDialog();
		}

		public void mnuCustomer_Click(Object eventSender, EventArgs eventArgs)
		{
			modMain.SetParentChild(frmCustomers.DefInstance, this);
			frmCustomers.DefInstance.Show();
		}

		public void mnuExit_Click(Object eventSender, EventArgs eventArgs)
		{
			this.Close();
		}

		public void mnuOrderReceptionsApproval_Click(Object eventSender, EventArgs eventArgs)
		{
			frmReceptionApproval.DefInstance.ShowDialog();
		}

		public void mnuOrderRequestsApproval_Click(Object eventSender, EventArgs eventArgs)
		{
			frmRequestApproval.DefInstance.ShowDialog();
		}

		public void mnuProducts_Click(Object eventSender, EventArgs eventArgs)
		{
			modMain.SetParentChild(frmProducts.DefInstance, this);
			frmProducts.DefInstance.Show();
		}

		public void mnuProviders_Click(Object eventSender, EventArgs eventArgs)
		{
			modMain.SetParentChild(frmProviders.DefInstance, this);
			frmProviders.DefInstance.Show();
		}

		public void mnuSecurity_Click(Object eventSender, EventArgs eventArgs)
		{
			frmUsersManage.DefInstance.ShowDialog();
		}

		//UPGRADE_NOTE: (7001) The following declaration (mnuUsers_Click) seems to be dead code More Information: http://www.vbtonet.com/ewis/ewi7001.aspx
		//private void mnuUsers_Click()
		//{
				//
		//}
		//UPGRADE_NOTE: (7001) The following declaration (Form_Unload) seems to be dead code More Information: http://www.vbtonet.com/ewis/ewi7001.aspx
		//private void Form_Unload(int Cancel)
		//{
		//}
	}
}