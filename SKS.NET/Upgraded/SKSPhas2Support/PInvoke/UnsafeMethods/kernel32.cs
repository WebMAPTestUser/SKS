using System.Runtime.InteropServices;
using System;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKSPhas2Support.PInvoke.UnsafeNative
{
	[System.Security.SuppressUnmanagedCodeSecurity]
	public static class kernel32
	{

		[DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int GetPrivateProfileString([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpApplicationName, System.IntPtr lpKeyName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpDefault, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpReturnedString, int nsize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);
		[DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int WritePrivateProfileString([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpApplicationName, System.IntPtr lpKeyName, System.IntPtr lpString, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);
	}
}