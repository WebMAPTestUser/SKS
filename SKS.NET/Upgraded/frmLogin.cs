using UpgradeHelpers.VB6.DB.ADO;
using UpgradeHelpers.VB6.Utils;
using System;
using System.Data;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmLogin
		: System.Windows.Forms.Form
	{


		public bool LoginSucceeded = false;

		private void cmdCancel_Click(Object eventSender, EventArgs eventArgs)
		{
			LoginSucceeded = false;
			this.Close();
		}

		private void cmdOK_Click(Object eventSender, EventArgs eventArgs)
		{
			modConnection.ExecuteSql("SELECT * FROM Users WHERE username = '" + txtUserName.Text + "' and password = '" + txtPassword.Text + "'");
			if (modConnection.rs.EOF)
			{
				MessageBox.Show("Invalid 'Username' or 'Password', please try again!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtUserName.Focus();
				modFunctions.SelectAll(txtUserName);
				return;
			}
			modMain.UserFullname = Convert.ToString(modConnection.rs["Fullname"]);
			modMain.UserLevel = Convert.ToString(modConnection.rs["Level"]);
			modMain.CurrentUserAdmin = (modMain.UserLevel == "Administrator");
			this.Cursor = Cursors.Default;
			LoginSucceeded = true;
			modMain.LogStatus("User : " + modMain.UserFullname + " logged at " + DateTimeHelper.ToString(DateTime.Parse(DateTimeHelper.ToString(DateTime.Now))) + "," + DateTimeHelper.ToString(DateTime.Now));
			this.Close();
		}
		private void frmLogin_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}