using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal static class modMain
	{


		public static bool CurrentUserAdmin = false;
		public static string UserFullname = String.Empty;
		public static string UserLevel = String.Empty;
		public static string UserId = String.Empty;

		public static string DatabasePath = String.Empty;
		public static string ConnectionString = String.Empty;

		public static int DetectionType = 0;
		public static double n = 0;
		public static int i = 0;
		public static string s = String.Empty;
		public static System.DateTime d = DateTime.FromOADate(0);
		public static string msg = String.Empty;
		public static string ImgName = String.Empty, ImgSrc = String.Empty;

		//UPGRADE_NOTE: (2041) The following line was commented. More Information: http://www.vbtonet.com/ewis/ewi2041.aspx
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int SetParent(int hWndChild, int hWndNewParent);

		internal static void SetParentChild(Form Parent, frmMain Child)
		{
            SKSPhas2Support.PInvoke.SafeNative.user32.SetParent(Parent.Handle.ToInt32(), Child.Handle.ToInt32());
		}

		internal static void SetNoParentChild(Form Parent)
		{
			SKSPhas2Support.PInvoke.SafeNative.user32.SetParent(Parent.Handle.ToInt32(), 0);
		}


		//UPGRADE_WARNING: (1047) Application will terminate when Sub Main() finishes. More Information: http://www.vbtonet.com/ewis/ewi1047.aspx
		[STAThread]
		public static void Main()
		{
			DatabasePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Database\\Orders.mdb";
			ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabasePath;
			modConnection.OpenConnection();
			CurrentUserAdmin = true;
			UserFullname = "Allan Cantillo";
			UserLevel = "Administrator";
			UserId = "acantillo";
			//frmLogin.Show vbModal
			//If (frmLogin.LoginSucceeded) Then
			Application.Run(frmMain.DefInstance);
			//End If
		}

		internal static void LogStatus(string message, Form frm = null)
		{
			StatusStrip sb = null;
			sb = null;
			frmMain.DefInstance.sbStatusBar.Items[0].Text = message;
			if (frm != null)
			{
				if (frm == frmAdjustStockManual.DefInstance)
				{
					sb = frmAdjustStockManual.DefInstance.sbStatusBar;
				}
				else if (frm == frmActionOrderReception.DefInstance)
				{ 
					sb = frmActionOrderReception.DefInstance.sbStatusBar;
				}
				else if (frm == frmActionOrderRequest.DefInstance)
				{ 
					sb = frmActionOrderRequest.DefInstance.sbStatusBar;
				}
				else if (frm == frmAddStockManual.DefInstance)
				{ 
					sb = frmAddStockManual.DefInstance.sbStatusBar;
				}
				else if (frm == frmReceptionApproval.DefInstance)
				{ 
					sb = frmReceptionApproval.DefInstance.sbStatusBar;
				}
				else if (frm == frmOrderReception.DefInstance)
				{ 
					sb = frmOrderReception.DefInstance.sbStatusBar;
				}
				else if (frm == frmOrderRequest.DefInstance)
				{ 
					sb = frmOrderRequest.DefInstance.sbStatusBar;
				}
				else if (frm == frmRequestApproval.DefInstance)
				{ 
					sb = frmRequestApproval.DefInstance.sbStatusBar;
				}
				if (sb != null)
				{
					if (sb.Items[0] != null)
					{
						sb.Items[0].Text = message;
					}
				}
			}
		}

		internal static void ClearLogStatus(Form frm = null)
		{
			LogStatus("", frm);
		}
	}
}