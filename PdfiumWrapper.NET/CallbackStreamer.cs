using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PdfiumWrapper.NET
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate bool Callback(ref CallbackStreamer _this, IntPtr data, uint size);


    [StructLayout(LayoutKind.Sequential)]
    public struct CallbackStreamer
    {
        public readonly int Version;
        private IntPtr CallbackPtr;
        public Stream ReceivingStream;

        public CallbackStreamer(Stream receivingStream)
        {
            this.Version = 1;       // Comment from pdfium: "Version number of the interface. Currently must be 1"
            this.ReceivingStream = receivingStream;
            this.CallbackPtr = Marshal.GetFunctionPointerForDelegate(CallbackDelegate);
        }

        static bool Callback(ref CallbackStreamer _this, IntPtr data, uint size)
        {
            byte[] buffer = new byte[size];
            Marshal.Copy(data, buffer, 0, (int)size);
            _this.ReceivingStream.Write(buffer, 0, (int)size);
            return true;        // Succeeded
        }

        static Callback CallbackDelegate = Callback;
    }
}
