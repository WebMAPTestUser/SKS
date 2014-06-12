using System.Runtime.InteropServices;
using System;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKSPhas2Support.PInvoke.UnsafeNative
{
	[System.Security.SuppressUnmanagedCodeSecurity]
	public static class user32
	{

		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int SetParent(int hWndChild, int hWndNewParent);
	}
}