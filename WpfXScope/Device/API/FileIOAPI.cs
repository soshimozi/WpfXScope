using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace WpfXScope.Device.API
{
    ///  <summary>
    ///  API declarations relating to file I/O (and used by WinUsb).
    ///  </summary>
    sealed internal class FileIO
    {
        internal const Int32 FileAttributeNormal = 0X80;
        internal const Int32 FileFlagOverlapped = 0X40000000;
        internal const Int32 FileShareRead = 1;
        internal const Int32 FileShareWrite = 2;
        internal const UInt32 GenericRead = 0X80000000;
        internal const UInt32 GenericWrite = 0X40000000;
        internal const Int32 InvalidHandleValue = -1;
        internal const Int32 OpenExisting = 3;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern SafeFileHandle CreateFile(String lpFileName, UInt32 dwDesiredAccess, Int32 dwShareMode, IntPtr lpSecurityAttributes, Int32 dwCreationDisposition, Int32 dwFlagsAndAttributes, Int32 hTemplateFile);
    }
}
