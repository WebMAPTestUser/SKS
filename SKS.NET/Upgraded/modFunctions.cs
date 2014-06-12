using UpgradeHelpers.VB6.DB.ADO;
using UpgradeHelpers.VB6.Gui;
using UpgradeHelpers.VB6.Utils;
using Microsoft.VisualBasic;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal static class modFunctions
	{


		//UPGRADE_NOTE: (2041) The following line was commented. More Information: http://www.vbtonet.com/ewis/ewi2041.aspx
		//[DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetPrivateProfileString([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpApplicationName, System.IntPtr lpKeyName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpDefault, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpReturnedString, int nsize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: http://www.vbtonet.com/ewis/ewi2041.aspx
		//[DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int WritePrivateProfileString([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpApplicationName, System.IntPtr lpKeyName, System.IntPtr lpString, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);
		internal static void AppendAND(ref string filter)
		{
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(filter))
			{
				filter = filter + " AND ";
			}
		}

		internal static bool AddToCollection(OrderedDictionary col, ref object Item)
		{
			bool result = false;
			if (!Exists(col, Item))
			{
				//UPGRADE_WARNING: (1068) Item of type Variant is being forced to string. More Information: http://www.vbtonet.com/ewis/ewi1068.aspx
				col.Add(Convert.ToString(Item), Item);
				result = true;
			}
			return result;
		}

		internal static bool Exists(OrderedDictionary col, object Index)
		{
			try
			{
				object o = null;
				o = col[Index];
				return o != null;
			}
			catch
			{
				return ExistsNonObject(col, Index);
			}
		}

		private static bool ExistsNonObject(OrderedDictionary col, object Index)
		{
			try
			{
				object v = null;
				//UPGRADE_WARNING: (1068) col() of type Variant is being forced to Scalar. More Information: http://www.vbtonet.com/ewis/ewi1068.aspx
				//UPGRADE_WARNING: (1037) Couldn't resolve default property of object v. More Information: http://www.vbtonet.com/ewis/ewi1037.aspx
				v = col[Index];
				return v != null;
			}
			catch
			{
				return false;
			}
		}

		internal static double DoubleValue(string strValue)
		{
			if (strValue.Length != 0)
			{
				return Double.Parse(strValue);
			}
			else
			{
				return 0;
			}
		}

		internal static void SelectAll(TextBox txtBox)
		{
			txtBox.SelectionStart = 0;
			txtBox.SelectionLength = Strings.Len(txtBox.Text);
		}

		internal static int UpCase(int KeyAscii)
		{
			return Strings.Asc(Strings.Chr(KeyAscii).ToString().ToUpper()[0]);
		}


		//'''''''''''''''''''''''''''''''''
		//'' Combobox related functions '''
		//'''''''''''''''''''''''''''''''''

		internal static void LoadCombo(string Table, ComboBox combo, ref string field, ref string valueField)
		{
			modConnection.ExecuteSql("Select * From " + Table);
			combo.Items.Clear();
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(valueField))
			{
				foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
				{
					combo.AddItem(Convert.ToString(iteration_row[field]));
					combo.SetItemData(combo.GetNewIndex(), Convert.ToInt32(iteration_row[valueField]));
				}
			}
			else
			{
				foreach (DataRow iteration_row_2 in modConnection.rs.Tables[0].Rows)
				{
					combo.AddItem(Convert.ToString(iteration_row_2[field]));
				}
			}
			//If strDefault <> Empty Then
			// combo = strDefault
			//End If
		}

		internal static void LoadCombo(string Table, ComboBox combo, ref string field)
		{
			string tempRefParam = String.Empty;
			LoadCombo(Table, combo, ref field, ref tempRefParam);
		}


		internal static bool ComboEmpty(ComboBox combo, object strip = null, int Index = 0)
		{
			bool result = false;
			if (combo.SelectedIndex == -1)
			{
				result = true;
				MessageBox.Show("Please select an option from the list", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!Index.Equals(0))
				{
					//strip.SelectedItem = strip.Tabs(Index)
				}
				combo.Focus();
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal static bool NoRecords(ListView lstView, string Prompt = "")
		{
			if (lstView.Items.Count == 0)
			{
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!String.IsNullOrEmpty(Prompt))
				{
					MessageBox.Show(Prompt, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		internal static string RcrdId(string Table, string Identifier, ref string FldNo)
		{
			int RcrdNo = 0;
			modConnection.ExecuteSql("Select * from " + Table + " order by " + FldNo + " ASC");
			if (!modConnection.rs.EOF)
			{
				modConnection.rs.MoveLast();
				RcrdNo = Convert.ToInt32(Convert.ToDouble(modConnection.rs[FldNo]) + 1);
			}
			else
			{
				RcrdNo = 1;
			}
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(Identifier))
			{
				return Identifier + RcrdNo.ToString() + DateTime.Today.ToString("MM");
			}
			else
			{
				return RcrdNo.ToString();
			}
		}

		internal static string RcrdId(string Table, string Identifier)
		{
			string tempRefParam2 = String.Empty;
			return RcrdId(Table, Identifier, ref tempRefParam2);
		}

		internal static string RcrdId(string Table)
		{
			string tempRefParam3 = String.Empty;
			return RcrdId(Table, String.Empty, ref tempRefParam3);
		}



		//''''''''''''''''''''''''''''''''''''''''
		internal static void SearchShow(string Table, string fieldToSearch, string itemToSearch)
		{
			frmSearch.DefInstance.Search(Table, fieldToSearch, itemToSearch);
			frmSearch.DefInstance.ShowDialog();
		}

		internal static double ValBox(string Prompt, PictureBox Icon, string Title = "", double Default = 0, string Header = "Value Box")
		{
			//With frmValue
			//    If Title <> Empty Then
			//       .Caption = Title
			//    Else
			//        .Caption = App.Title
			//    End If
			//    .lblHeader.Caption = StrConv(Header, vbUpperCase)
			//    .imgIcon.Picture = Icon.Picture
			//    .lblPrompt.Caption = Prompt
			//    .Default Val(Default)
			//    .Show vbModal
			//    ValBox = Val(.txtValue.Text)
			//    Unload frmValue
			//End With
			return 0;
		}


		internal static bool TextBoxEmpty(TextBox stext, object TabObject = null, int TabIndex = 0)
		{
			//UPGRADE_TODO: (1067) Member Text is not defined in type Variant. More Information: http://www.vbtonet.com/ewis/ewi1067.aspx
			//UPGRADE_WARNING: (1068) stext of type Variant is being forced to string. More Information: http://www.vbtonet.com/ewis/ewi1068.aspx
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			bool result = false;
			if (String.IsNullOrEmpty(Convert.ToString(stext).Trim()) || Convert.ToString(stext.Text) == "  /  /    ")
			{
				result = true;
				MessageBox.Show("You need to fill in all required fields", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
				if (!TabIndex.Equals(0))
				{
					//TabObject.SelectedItem = TabObject.Tabs(TabIndex)
				}
				//UPGRADE_TODO: (1067) Member SetFocus is not defined in type Variant. More Information: http://www.vbtonet.com/ewis/ewi1067.aspx
				stext.Focus();
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal static bool TextBoxNumberEmpty(TextBox textbox)
		{
			//if the input is not a numeric then true
			bool result = false;
			double dbNumericTemp = 0;
			if (!Double.TryParse(textbox.Text, NumberStyles.Number, CultureInfo.CurrentCulture.NumberFormat, out dbNumericTemp))
			{
				result = true;
				MessageBox.Show("The field requires a numeric value.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				textbox.Focus();
				SelectAll(textbox);
			}
			else
			{
				result = false;
			}
			return result;
		}



		internal static int Warnings(int dType = 0)
		{
			modMain.DetectionType = dType;
			modConnection.ExecuteSql("Delete * from tblDetections");

			//-------expiration
			modConnection.ExecuteSql("SELECT * From tblStockList");
			foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
			{
				if (Convert.ToString(iteration_row["expiry_date"]) != "  /  /    ")
				{
					modMain.d = DateTime.Parse(DateTime.Parse(Convert.ToString(iteration_row["expiry_date"])).ToString("MM/dd/yyyy"));
					if (String.CompareOrdinal(modMain.d.ToString("MM"), DateTime.Today.ToString("MM")) >= 0 && StringsHelper.ToDoubleSafe(modMain.d.ToString("MM")) <= (Conversion.Val(DateTime.Today.ToString("MM")) + 2) && modMain.d.ToString("yyyy") == DateTime.Today.ToString("yyyy"))
					{
						modConnection.ExecuteSql2("Select * from tblInventory where ProductId = '" + Convert.ToString(iteration_row["productId"]) + "'");
						if (!modConnection.rs2.EOF)
						{
							modMain.n = Convert.ToDouble(modConnection.rs2["quantity"]);
						}
						if (String.CompareOrdinal(Convert.ToDateTime(iteration_row["expiry_date"]).ToString("MM"), DateTime.Today.ToString("MM")) <= 0 && String.CompareOrdinal(Convert.ToDateTime(iteration_row["expiry_date"]).ToString("dd"), DateTime.Today.ToString("dd")) <= 0 && String.CompareOrdinal(Convert.ToDateTime(iteration_row["expiry_date"]).ToString("yyyy"), DateTime.Today.ToString("yyyy")) <= 0)
						{
							modMain.s = "This item is already expired. Please unregister this from Inventory and add new stocks." + 
							            Environment.NewLine + Environment.NewLine + "Item Description: " + Convert.ToString(iteration_row["Description"]) + Environment.NewLine + Environment.NewLine + 
							            "Expiry Date: " + Convert.ToDateTime(iteration_row["expiry_date"]).ToString("MMM. dd, yyyy") + Environment.NewLine + 
							            "Quantity on Inventory: " + modMain.n.ToString();
							SaveDetection(Convert.ToString(iteration_row["productId"]), "Expired", modMain.s, "tblDetections");
						}
						else
						{
							modMain.s = (Double.Parse(Convert.ToDateTime(iteration_row["expiry_date"]).ToString("MM")) - Double.Parse(DateTime.Today.ToString("MM"))).ToString() + " Month(s) before Expiry. Please replace it with new stocks and delete your old stocks. " + 
							            Environment.NewLine + Environment.NewLine + "Item Description: " + Convert.ToString(iteration_row["Description"]) + Environment.NewLine + Environment.NewLine + 
							            "Expiry date: " + Convert.ToDateTime(iteration_row["expiry_date"]).ToString("MMM. dd, yyyy") + Environment.NewLine + 
							            "quantity on Inventory: " + modMain.n.ToString();
							SaveDetection(Convert.ToString(iteration_row["productId"]), "Expiration", modMain.s, "tblDetections");
						}
					}
				}
			}

			//-------out of stock
			modConnection.ExecuteSql("SELECT * From tblInventory WHERE quantity < 10");
			foreach (DataRow iteration_row_2 in modConnection.rs.Tables[0].Rows)
			{
				modMain.s = "This item do not have enough quantity on your inventory. Please add stock for this item." + Environment.NewLine + Environment.NewLine + 
				            "Item Description: " + Convert.ToString(iteration_row_2["Description"]) + Environment.NewLine + Environment.NewLine + 
				            "Currently on Inventory: " + Convert.ToString(iteration_row_2["quantity"]);
				SaveDetection(Convert.ToString(iteration_row_2["productId"]), "Low Stock", modMain.s, "tblDetections");
			}

			//-------low inventory
			modConnection.ExecuteSql("Select * from tblInventory");
			if (modConnection.rs.RecordCount == 0 || modConnection.rs.RecordCount <= 10)
			{
				modMain.s = "You don`t have enough items on your inventory." + 
				            "Please add items or register items from database to your inventory list." + Environment.NewLine + Environment.NewLine + 
				            "Items on Inventory: " + modConnection.rs.RecordCount.ToString();
				SaveDetection("Inventory", "Low Inventory", modMain.s, "tblDetections");
			}

			//-------no sales for the month
			modConnection.ExecuteSql("Select * from tblInventory");
			foreach (DataRow iteration_row_3 in modConnection.rs.Tables[0].Rows)
			{
				if (StringsHelper.ToDoubleSafe(DateTime.Today.ToString("MM")) != 1)
				{
					modMain.n = Double.Parse(DateTime.Today.ToString("MM")) - 1;
					modConnection.ExecuteSql2("Select * from tblSales where ProductId = '" + Convert.ToString(iteration_row_3["productId"]) + "' and format([date_sold],'mm') = " + modMain.n.ToString() + 
					                          "and format([date_sold],'yyyy') = " + DateTime.Today.ToString("yyyy"));
					if (!modConnection.rs2.EOF)
					{
						if (Convert.ToDouble(modConnection.rs2["quantity"]) < 30)
						{
							modMain.i = 0;
							foreach (DataRow iteration_row_4 in modConnection.rs2.Tables[0].Rows)
							{
								modMain.i = Convert.ToInt32(modMain.i + Convert.ToDouble(iteration_row_4["quantity"]));
							}
							modMain.s = "Sales of this item is less for this month." + Environment.NewLine + Environment.NewLine + 
							            "Last Month total sales: " + modMain.i.ToString();
							SaveDetection(Convert.ToString(iteration_row_3["productId"]), "Less Sales", modMain.s, "tblDetections");
						}
					}
				}
			}

			//-----No supplier
			modConnection.ExecuteSql("Select * from tblSuppliers");
			if (modConnection.rs.RecordCount == 0)
			{
				modMain.s = "No supplier saved on database. Please add a supplier for item delivery.";
				SaveDetection("Suppliers", "No Supplier", modMain.s, "tblDetections");
			}

			//-----Items no registered
			modConnection.ExecuteSql("Select * from tblItems where on_inventory = 0");
			modMain.n = 0;
			foreach (DataRow iteration_row_5 in modConnection.rs.Tables[0].Rows)
			{
				modConnection.ExecuteSql2("SELECT * From tblStockList WHERE ProductId = '" + Convert.ToString(iteration_row_5["productId"]) + "' and Format$([expiry_date],'mm') Between " + Conversion.Val(DateTime.Today.ToString("MM")).ToString() + " And " + Conversion.Val((Double.Parse(DateTime.Today.ToString("MM")) + 2).ToString()).ToString() + " and format$(expiry_date, 'yyyy') = " + DateTime.Today.ToString("yyyy"));
				if (modConnection.rs2.EOF)
				{
					modMain.n++;
				}
			}
			if (modMain.n > 0)
			{
				modMain.s = "Some items on your database are not registered on your inventory list. If you don`t register this items, " + 
				            " they will not be included on your sales." + Environment.NewLine + Environment.NewLine + 
				            "Unregistered Items: " + modMain.n.ToString();
				SaveDetection("Register", "Non-Registered", modMain.s, "tblDetections");
			}

			//-----Delivery Schedule exceeded
			modConnection.ExecuteSql("Select Sup.Company as Company, Sup.last_delivery as LastDelivery, sched.gap as Gap, sched.gap_value as GapVal from tblSuppliers as Sup " + 
			                         "INNER JOIN tblDeliverySched as Sched ON Sup.sched_type = Sched.description");
			foreach (DataRow iteration_row_6 in modConnection.rs.Tables[0].Rows)
			{
				int tempRefParam = Convert.ToInt32(Double.Parse(Convert.ToDateTime(iteration_row_6["lastdelivery"]).ToString("MM")));
				int tempRefParam2 = Convert.ToInt32(Double.Parse(Convert.ToDateTime(iteration_row_6["lastdelivery"]).ToString("dd")));
				int tempRefParam3 = Convert.ToInt32(Double.Parse(Convert.ToDateTime(iteration_row_6["lastdelivery"]).ToString("yyyy")));
				modMain.d = Scheduler(ref tempRefParam, ref tempRefParam2, ref tempRefParam3, Convert.ToInt32(iteration_row_6["GapVal"]), Convert.ToString(iteration_row_6["Gap"]));
				if (String.CompareOrdinal(modMain.d.ToString("MM"), DateTime.Today.ToString("MM")) <= 0 && String.CompareOrdinal(modMain.d.ToString("dd"), DateTime.Today.ToString("dd")) < 0 && Convert.ToString(iteration_row_6["Gap"]) != "(none)")
				{
					modMain.s = "Delivery schedule of supplier, " + Convert.ToString(iteration_row_6["company"]) + ", is not updated. " + 
					            "Please record all delivery transactions of your suppliers to update it's delivery schedule." + Environment.NewLine + Environment.NewLine + 
					            "Last Delivery: " + Convert.ToDateTime(iteration_row_6["lastdelivery"]).ToString("MMM. dd, yyyy") + Environment.NewLine + 
					            "Expected Date: " + modMain.d.ToString("MMM. dd, yyyy");
					SaveDetection(Convert.ToString(iteration_row_6["company"]), "Delivery Sched", modMain.s, "tblDetections");
				}
			}
			modConnection.ExecuteSql("Select * from tblDetections");
			return modConnection.rs.RecordCount;

		}

		private static void SaveDetection(string Reference, string Title, string Description, string Table)
		{
			modConnection.ExecuteSql2("Select * from " + Table);
			modConnection.rs2.AddNew();
			string tempRefParam = "record_no";
			modConnection.rs2["record_no"] = Conversion.Val(RcrdId(Table, String.Empty, ref tempRefParam));
			modConnection.rs2["Reference"] = Reference;
			modConnection.rs2["war_type"] = Title;
			modConnection.rs2["Description"] = Description;
			modConnection.rs2.Update();
		}

		internal static string ReadINI(string strFile, string strKey, string strName)
		{
			string strText = "                                                                                                    ";
			string tempRefParam = "";
			int intLen = SKSPhas2Support.PInvoke.SafeNative.kernel32.GetPrivateProfileString(ref strKey, strName, ref tempRefParam, ref strText, strText.Length, ref strFile);
			if (intLen > -1)
			{
				strText = strText.Substring(0, Math.Min(intLen, strText.Length));
			}
			else
			{
				MessageBox.Show("Error on reading configuration", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				Environment.Exit(0);
			}
			return strText;
		}

		internal static void WriteINI(string strFile, string strKey, string strName, string strText)
		{
			int intLen = SKSPhas2Support.PInvoke.SafeNative.kernel32.WritePrivateProfileString(ref strKey, strName, strText, ref strFile);
		}


		internal static System.DateTime Scheduler(ref int IntM, ref int IntD, ref int IntY, int GapVal, string Gap = "Week")
		{

			int Max = 0;
			int LastVal = 0;
			int MaxDays = 0;
			switch(Gap)
			{
				case "Day" : 
					Max = Convert.ToInt32(Conversion.Val(ReadINI(Path.GetDirectoryName(Application.ExecutablePath) + "\\Settings.ini", "Month Max", DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(IntM)))); 
					IntD += GapVal; 
					for (modMain.i = 1; modMain.i <= IntD; modMain.i++)
					{
						if (modMain.i == Max)
						{
							LastVal = Max;
							IntM++;
							if (IntM > 12)
							{
								IntM = 1;
								IntY++;
							}
							Max = Convert.ToInt32(Max + Conversion.Val(ReadINI(Path.GetDirectoryName(Application.ExecutablePath) + "\\Settings.ini", "Month Max", DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(IntM))));
						}
					} 
					IntD -= LastVal; 
					break;
				case "Week" : 
					MaxDays = 7 * GapVal; 
					Max = Convert.ToInt32(Conversion.Val(ReadINI(Path.GetDirectoryName(Application.ExecutablePath) + "\\Settings.ini", "Month Max", DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(IntM)))); 
					IntD += MaxDays; 
					for (modMain.i = 1; modMain.i <= IntD; modMain.i++)
					{
						if (modMain.i == Max)
						{
							LastVal = Max;
							IntM++;
							if (IntM > 12)
							{
								IntM = 1;
								IntY++;
							}
							Max = Convert.ToInt32(Max + Conversion.Val(ReadINI(Path.GetDirectoryName(Application.ExecutablePath) + "\\Settings.ini", "Month Max", DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(IntM))));
						}
					} 
					IntD -= LastVal; 
					break;
				case "Month" : 
					IntM += GapVal; 
					if (IntM > 12)
					{
						IntM -= 12;
						IntY++;
					} 
					break;
				case "Year" : 
					IntY += GapVal; 
					break;
			}
			return DateAndTime.DateSerial(IntY, IntM, IntD);
		}


		internal static string ExecErr(string Prompt, ref string PromptFld, string Table, string RcrdFld, string RcrdStr)
		{
			StringBuilder Rcrds = new StringBuilder();
			//UPGRADE_WARNING: (2080) IsEmpty was upgraded to a comparison and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
			if (!String.IsNullOrEmpty(Table))
			{
				modConnection.ExecuteSql("Select * from " + Table + " where " + RcrdFld + " = '" + RcrdStr + "'");
				foreach (DataRow iteration_row in modConnection.rs.Tables[0].Rows)
				{
					Rcrds.Append(Convert.ToString(iteration_row[PromptFld]) + "; ");
				}
				return "Error: " + Prompt + Environment.NewLine + Environment.NewLine + 
				"Related Records: " + Rcrds.ToString();
			}
			else
			{
				return Prompt;
			}
		}

		internal static string ExecErr(string Prompt, ref string PromptFld, string Table, string RcrdFld)
		{
			return ExecErr(Prompt, ref PromptFld, Table, RcrdFld, String.Empty);
		}

		internal static string ExecErr(string Prompt, ref string PromptFld, string Table)
		{
			return ExecErr(Prompt, ref PromptFld, Table, String.Empty, String.Empty);
		}

		internal static string ExecErr(string Prompt, ref string PromptFld)
		{
			return ExecErr(Prompt, ref PromptFld, String.Empty, String.Empty, String.Empty);
		}

		internal static string ExecErr(string Prompt)
		{
			string tempRefParam4 = String.Empty;
			return ExecErr(Prompt, ref tempRefParam4, String.Empty, String.Empty, String.Empty);
		}
	}
}