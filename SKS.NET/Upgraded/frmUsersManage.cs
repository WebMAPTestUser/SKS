using UpgradeHelpers.VB6.DB.ADO;
using UpgradeHelpers.VB6.Gui;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmUsersManage
		: System.Windows.Forms.Form
	{

		string CurrentEditedUser = String.Empty;

		private void cmdClear_Click(Object eventSender, EventArgs eventArgs)
		{
			txtUsername.Text = String.Empty;
			txtUsername.Focus();
			ClearFields();
		}

		private void cmdDelete_Click(Object eventSender, EventArgs eventArgs)
		{
			if (modFunctions.NoRecords(lstAccounts, "Please add a user"))
			{
				return;
			}
			if (MessageBox.Show("Are you sure you want to delete the user '" + lstAccounts.FocusedItem.Text + "'?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
			{
				modConnection.ExecuteSql("Select * from Users");
				if (modConnection.rs.RecordCount == 1)
				{
					//UPGRADE_WARNING: (6021) Casting 'string' to Enum may cause different behaviour. More Information: http://www.vbtonet.com/ewis/ewi6021.aspx
					//UPGRADE_ISSUE: (1046) MsgBox Parameter 'context' is not supported, and was removed. More Information: http://www.vbtonet.com/ewis/ewi1046.aspx
					//UPGRADE_ISSUE: (1046) MsgBox Parameter 'helpfile' is not supported, and was removed. More Information: http://www.vbtonet.com/ewis/ewi1046.aspx
					Interaction.MsgBox("You cannot delete the last user", (MsgBoxStyle) Convert.ToInt32(Double.Parse("Delete error")), ((int) MsgBoxStyle.Critical).ToString());
					return;
				}
				modConnection.ExecuteSql("delete * from Users where Username = '" + lstAccounts.FocusedItem.Text + "'");
				LoadUsers();
			}
		}

		private void cmdEdit_Click(Object eventSender, EventArgs eventArgs)
		{
			if (modFunctions.NoRecords(lstAccounts, "No user found on the list. Please add a user account"))
			{
				return;
			}
			modConnection.ExecuteSql("Select * from Users where Username = '" + lstAccounts.FocusedItem.Text + "'");
			txtUsername.Text = Convert.ToString(modConnection.rs["UserName"]);
			if (modConnection.rs.EOF)
			{
				MessageBox.Show("This user does not exist", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				txtUsername.Focus();
			}
			else
			{
				txtUsername.Text = Convert.ToString(modConnection.rs["UserName"]);
				CurrentEditedUser = txtUsername.Text;
				txtPassword.Text = Convert.ToString(modConnection.rs["Password"]);
				txtFullname.Text = Convert.ToString(modConnection.rs["Fullname"]);
				cboLevel.Text = Convert.ToString(modConnection.rs["Level"]);
				cmdSave.Text = "&Update";
			}
		}

		private void cmdSave_Click(Object eventSender, EventArgs eventArgs)
		{
			string SecId = String.Empty;
			if (modFunctions.TextBoxEmpty(txtUsername))
			{
				return;
			}
			if (modFunctions.TextBoxEmpty(txtPassword))
			{
				return;
			}
			if (modFunctions.TextBoxEmpty(txtFullname))
			{
				return;
			}
			if (modFunctions.ComboEmpty(cboLevel))
			{
				return;
			}

			modConnection.ExecuteSql("Select * from Users where Username = '" + txtUsername.Text + "'");
			if (cmdSave.Text != "&Update")
			{
				if (cboLevel.Text != "Administrator")
				{
					modConnection.ExecuteSql2("Select * from Users where level = 'Administrator'");
					if (modConnection.rs2.EOF)
					{
						MessageBox.Show("Update failed: No any Administrator found on accounts.  You are not allowed to change the level of this account", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
				if (!modMain.CurrentUserAdmin && cboLevel.Text == "Administrator")
				{
					MessageBox.Show("You cannot add another level without being 'Administrator'", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					cboLevel.Focus();
					return;
				}
				modConnection.rs.AddNew();
				modMain.msg = "Added new user " + txtUsername.Text;
			}
			else if (CurrentEditedUser != txtUsername.Text)
			{ 
				modConnection.ExecuteSql2("Select * from Users where username = '" + txtUsername.Text + "'");
				if (!modConnection.rs2.EOF)
				{
					MessageBox.Show("Username '" + txtUsername.Text + "' already exists.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					txtUsername.Focus();
					modFunctions.SelectAll(txtUsername);
					return;
				}
				modMain.msg = "Record for the user " + txtUsername.Text + " has been successfully updated";
			}
			else
			{
				modMain.msg = "Record for the user " + txtUsername.Text + " has been successfully updated";
			}
			modConnection.rs["UserName"] = txtUsername.Text;
			modConnection.rs["Password"] = txtPassword.Text;
			modConnection.rs["Level"] = cboLevel.Text;
			modConnection.rs["Fullname"] = txtFullname.Text;
			modConnection.rs.Update();
			modMain.LogStatus(modMain.msg);
			ClearFields();
			LoadUsers();

			if (modMain.CurrentUserAdmin)
			{
				this.Close();
			}
		}

		public void LoadUsers()
		{
			modConnection.ExecuteSql("Select * from Users");
			lstAccounts.Items.Clear();
			ListViewItem x = null;
			foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
			{
				x = (ListViewItem) lstAccounts.Items.Add(Convert.ToString(iteration_row["UserName"]));
				ListViewHelper.GetListViewSubItem(x, 1).Text = Convert.ToString(iteration_row["Fullname"]);
				ListViewHelper.GetListViewSubItem(x, 2).Text = Convert.ToString(iteration_row["Level"]);
			}
		}

		public void LoadUsersAvoidingWith()
		{
			modConnection.ExecuteSql("Select * from Users");
			lstAccounts.Items.Clear();
			ListViewItem x = null;
			foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
			{
				x = (ListViewItem) lstAccounts.Items.Add(Convert.ToString(iteration_row["UserName"]));
				ListViewHelper.GetListViewSubItem(x, 1).Text = Convert.ToString(iteration_row["Fullname"]);
				ListViewHelper.GetListViewSubItem(x, 2).Text = Convert.ToString(iteration_row["Level"]);
			}
		}


		public void ClearFields()
		{
			txtUsername.Text = String.Empty;
			txtPassword.Text = String.Empty;
			txtFullname.Text = String.Empty;
			cboLevel.SelectedIndex = -1;
			cmdSave.Text = "&Save";
		}

		private void cmdClose_Click(Object eventSender, EventArgs eventArgs)
		{
			this.Close();
		}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load event and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
		private void frmUsersManage_Load(Object eventSender, EventArgs eventArgs)
		{
			modConnection.ExecuteSql("Select * from Levels");
			foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
			{
				cboLevel.AddItem(Convert.ToString(iteration_row["Level"]));
			}
			if (modMain.CurrentUserAdmin)
			{
				cboLevel.Text = "Administrator";
			}
			else
			{
				cboLevel.SelectedIndex = -1;
			}
			LoadUsers();
		}

		private void frmUsersManage_Closed(Object eventSender, EventArgs eventArgs)
		{
			if (modMain.CurrentUserAdmin)
			{
				modConnection.ExecuteSql("Select * from Users");
				if (modConnection.rs.EOF)
				{
					MessageBox.Show("System has failed to initialized. Please contact your administrator" + Environment.NewLine + Environment.NewLine + "Status: analysing accounts configuration" + Environment.NewLine + 
					                "Error: No users found", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					Environment.Exit(0);
				}
				//frmxSplash.tmrLoad.Enabled = True
			}
			modMain.LogStatus("");
		}

		private void lstAccounts_DoubleClick(Object eventSender, EventArgs eventArgs)
		{
			cmdEdit_Click(cmdEdit, new EventArgs());
		}

		private void txtFullname_Enter(Object eventSender, EventArgs eventArgs)
		{
			modFunctions.SelectAll(txtFullname);
		}

		private void txtPassword_Enter(Object eventSender, EventArgs eventArgs)
		{
			modFunctions.SelectAll(txtPassword);
		}

		private void txtUsername_Enter(Object eventSender, EventArgs eventArgs)
		{
			modFunctions.SelectAll(txtUsername);
		}
	}
}