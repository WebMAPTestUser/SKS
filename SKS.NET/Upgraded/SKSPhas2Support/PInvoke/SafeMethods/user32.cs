using System.Runtime.InteropServices;
using System;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKSPhas2Support.PInvoke.SafeNative
{
	public static class user32
	{

		public static int SetParent(int hWndChild, int hWndNewParent)
		{
			return SKSPhas2Support.PInvoke.UnsafeNative.user32.SetParent(hWndChild, hWndNewParent);
		}
	}
}