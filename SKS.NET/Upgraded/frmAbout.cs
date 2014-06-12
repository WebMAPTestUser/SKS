using UpgradeHelpers.VB6.Utils;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	internal partial class frmAbout
		: System.Windows.Forms.Form
	{


		// Reg Key Security Options...
		const int READ_CONTROL = 0x20000;
		const int KEY_QUERY_VALUE = 0x1;
		const int KEY_SET_VALUE = 0x2;
		const int KEY_CREATE_SUB_KEY = 0x4;
		const int KEY_ENUMERATE_SUB_KEYS = 0x8;
		const int KEY_NOTIFY = 0x10;
		const int KEY_CREATE_LINK = 0x20;
		static readonly int KEY_ALL_ACCESS = KEY_QUERY_VALUE + KEY_SET_VALUE + KEY_CREATE_SUB_KEY + KEY_ENUMERATE_SUB_KEYS + KEY_NOTIFY + KEY_CREATE_LINK + READ_CONTROL;

		// Reg Key ROOT Types...
		static readonly int HKEY_LOCAL_MACHINE = unchecked((int) 0x80000002);
		const int ERROR_SUCCESS = 0;
		const int REG_SZ = 1; // Unicode nul terminated string
		const int REG_DWORD = 4; // 32-bit number

		const string gREGKEYSYSINFOLOC = "SOFTWARE\\Microsoft\\Shared Tools Location";
		const string gREGVALSYSINFOLOC = "MSINFO";
		const string gREGKEYSYSINFO = "SOFTWARE\\Microsoft\\Shared Tools\\MSINFO";
		const string gREGVALSYSINFO = "PATH";

		//UPGRADE_NOTE: (2041) The following line was commented. More Information: http://www.vbtonet.com/ewis/ewi2041.aspx
		//[DllImport("advapi32.dll", EntryPoint = "RegOpenKeyExA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int RegOpenKeyEx(int hKey, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpSubKey, int ulOptions, int samDesired, ref int phkResult);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: http://www.vbtonet.com/ewis/ewi2041.aspx
		//[DllImport("advapi32.dll", EntryPoint = "RegQueryValueExA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int RegQueryValueEx(int hKey, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpValueName, int lpReserved, ref int lpType, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpData, ref int lpcbData);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: http://www.vbtonet.com/ewis/ewi2041.aspx
		//[DllImport("advapi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int RegCloseKey(int hKey);


		private void cmdSysInfo_Click(Object eventSender, EventArgs eventArgs)
		{
			StartSysInfo();
		}

		private void cmdOK_Click(Object eventSender, EventArgs eventArgs)
		{
			this.Close();
		}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load event and has a new behavior. More Information: http://www.vbtonet.com/ewis/ewi2080.aspx
		private void frmAbout_Load(Object eventSender, EventArgs eventArgs)
		{
			this.Text = "About " + AssemblyHelper.GetTitle(System.Reflection.Assembly.GetExecutingAssembly());
			lblVersion.Text = "Version " + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileMajorPart.ToString() + "." + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileMinorPart.ToString() + "." + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FilePrivatePart.ToString();
			lblTitle.Text = AssemblyHelper.GetTitle(System.Reflection.Assembly.GetExecutingAssembly());
		}

		public void StartSysInfo()
		{
			try
			{

				int rc = 0;
				string SysInfoPath = String.Empty;

				// Try To Get System Info Program Path\Name From Registry...
				string tempRefParam2 = gREGVALSYSINFOLOC;
				string tempRefParam = gREGVALSYSINFO;
				if (GetKeyValue(HKEY_LOCAL_MACHINE, gREGKEYSYSINFO, ref tempRefParam, ref SysInfoPath))
				{
					// Try To Get System Info Program Path Only From Registry...
				}
				else if (GetKeyValue(HKEY_LOCAL_MACHINE, gREGKEYSYSINFOLOC, ref tempRefParam2, ref SysInfoPath))
				{ 
					// Validate Existance Of Known 32 Bit File Version
					if (FileSystem.Dir(SysInfoPath + "\\MSINFO32.EXE", FileAttribute.Normal) != "")
					{
						SysInfoPath = SysInfoPath + "\\MSINFO32.EXE";

						// Error - File Can Not Be Found...
					}
					else
					{
						throw new Exception();
					}
					// Error - Registry Entry Can Not Be Found...
				}
				else
				{
					throw new Exception();
				}

				//UPGRADE_TODO: (7005) parameters (if any) must be set using the Arguments property of ProcessStartInfo More Information: http://www.vbtonet.com/ewis/ewi7005.aspx
				ProcessStartInfo startInfo = new ProcessStartInfo(SysInfoPath);
				startInfo.WindowStyle = ProcessWindowStyle.Normal;
				Process.Start(startInfo);
			}
			catch
			{
				MessageBox.Show("System Information Is Unavailable At This Time", Application.ProductName, MessageBoxButtons.OK);
			}

		}

		public bool GetKeyValue(int KeyRoot, string KeyName, ref string SubKeyRef, ref string KeyVal)
		{
			bool result = false;
			// Loop Counter
			int hKey = 0; // Handle To An Open Registry Key
			int hDepth = 0; //
			int KeyValType = 0; // Data Type Of A Registry Key
			string tmpVal = String.Empty; // Tempory Storage For A Registry Key Value
			int KeyValSize = 0; // Size Of Registry Key Variable
			//------------------------------------------------------------
			// Open RegKey Under KeyRoot {HKEY_LOCAL_MACHINE...}
			//------------------------------------------------------------
			int rc = SKSPhas2Support.PInvoke.SafeNative.advapi32.RegOpenKeyEx(KeyRoot, ref KeyName, 0, KEY_ALL_ACCESS, ref hKey); // Return Code // Open Registry Key

			if (!(rc != ERROR_SUCCESS))
			{ // Handle Error...

				tmpVal = new string((char) 0, 1024); // Allocate Variable Space
				KeyValSize = 1024; // Mark Variable Size

				//------------------------------------------------------------
				// Retrieve Registry Key Value...
				//------------------------------------------------------------
				rc = SKSPhas2Support.PInvoke.SafeNative.advapi32.RegQueryValueEx(hKey, ref SubKeyRef, 0, ref KeyValType, ref tmpVal, ref KeyValSize); // Get/Create Key Value

				if (!(rc != ERROR_SUCCESS))
				{ // Handle Errors

					if (Strings.Asc(tmpVal.Substring(KeyValSize - 1, Math.Min(1, tmpVal.Length - (KeyValSize - 1)))[0]) == 0)
					{ // Win95 Adds Null Terminated String...
						tmpVal = tmpVal.Substring(0, Math.Min(KeyValSize - 1, tmpVal.Length)); // Null Found, Extract From String
					}
					else
					{
						// WinNT Does NOT Null Terminate String...
						tmpVal = tmpVal.Substring(0, Math.Min(KeyValSize, tmpVal.Length)); // Null Not Found, Extract String Only
					}
					//------------------------------------------------------------
					// Determine Key Value Type For Conversion...
					//------------------------------------------------------------
					switch(KeyValType)
					{ // Search Data Types...
						case REG_SZ :  // String Registry Key Data Type 
							KeyVal = tmpVal;  // Copy String Value 
							break;
						case REG_DWORD :  // Double Word Registry Key Data Type 
							for (int i = tmpVal.Length; i >= 1; i--)
							{ // Convert Each Bit
								KeyVal = KeyVal + Strings.Asc(tmpVal.Substring(i - 1, Math.Min(1, tmpVal.Length - (i - 1)))[0]).ToString("X"); // Build Value Char. By Char.
							} 
							KeyVal = ("&h" + KeyVal).ToString();  // Convert Double Word To String 
							break;
					}

					result = true; // Return Success
					rc = SKSPhas2Support.PInvoke.SafeNative.advapi32.RegCloseKey(hKey); // Close Registry Key
					return result; // Exit

				}
			}
			// Cleanup After An Error Has Occured...
			KeyVal = ""; // Set Return Val To Empty String // Return Failure
			rc = SKSPhas2Support.PInvoke.SafeNative.advapi32.RegCloseKey(hKey); // Close Registry Key
			return result;
		}
		private void frmAbout_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}