using System.Runtime.InteropServices;
using System;
using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKSPhas2Support.PInvoke.SafeNative
{
	public static class kernel32
	{

		public static int GetPrivateProfileString(ref string lpApplicationName, string lpKeyName, ref string lpDefault, ref string lpReturnedString, int nsize, ref string lpFileName)
		{
			int result = 0;
			IntPtr tmpPtr = Marshal.StringToHGlobalAnsi(lpKeyName);
			try
			{
				result = SKSPhas2Support.PInvoke.UnsafeNative.kernel32.GetPrivateProfileString(ref lpApplicationName, tmpPtr, ref lpDefault, ref lpReturnedString, nsize, ref lpFileName);
				lpKeyName = Marshal.PtrToStringAnsi(tmpPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(tmpPtr);
			}
			return result;
		}
		public static int WritePrivateProfileString(ref string lpApplicationName, string lpKeyName, string lpString, ref string lpFileName)
		{
			int result = 0;
			IntPtr tmpPtr = Marshal.StringToHGlobalAnsi(lpKeyName);
			IntPtr tmpPtr2 = Marshal.StringToHGlobalAnsi(lpString);
			try
			{
				result = SKSPhas2Support.PInvoke.UnsafeNative.kernel32.WritePrivateProfileString(ref lpApplicationName, tmpPtr, tmpPtr2, ref lpFileName);
				lpString = Marshal.PtrToStringAnsi(tmpPtr2);
				lpKeyName = Marshal.PtrToStringAnsi(tmpPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(tmpPtr);
				Marshal.FreeHGlobal(tmpPtr2);
			}
			return result;
		}
	}
}