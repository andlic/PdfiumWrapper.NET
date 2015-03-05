using System;
using System.Runtime.InteropServices;

namespace PdfiumWrapper.NET
{
    public static class PdfiumDLL
    {
        public static bool IsInitialized { get; private set; }

        public static void EnsureIsInitialized()
        {
            if (!IsInitialized)
            {
                FPDF_InitLibrary();
                IsInitialized = true;
            }
        }

        public static void Destroy()
        {
            FPDF_DestroyLibrary();
            IsInitialized = false;
        }

        [DllImport("pdfium.dll")]
        internal static extern void FPDF_InitLibrary();

        [DllImport("pdfium.dll")]
        internal static extern void FPDF_DestroyLibrary();

        [DllImport("pdfium.dll")]
        internal static extern IntPtr FPDF_LoadDocument(string filePath, string password);

        [DllImport("pdfium.dll")]
        internal static extern void FPDF_CloseDocument(IntPtr documentHandle);

        [DllImport("pdfium.dll")]
        internal static extern int FPDF_GetPageCount(IntPtr documentHandle);

        [DllImport("pdfium.dll")]
        internal static extern IntPtr FPDF_LoadPage(IntPtr documentHandle, int pageIndex);

        [DllImport("pdfium.dll")]
        internal static extern void FPDF_ClosePage(IntPtr pageHandle);

        [DllImport("pdfium.dll")]
        internal static extern int FPDF_GetPageSizeByIndex(IntPtr documentHandle, int pageIndex, out double width, out double height);

        [DllImport("pdfium.dll")]
        internal static extern IntPtr FPDFBitmap_CreateEx(int width, int height, int format, byte[] firstScan, int stride);

        [DllImport("pdfium.dll")]
        internal static extern IntPtr FPDFBitmap_CreateEx(int width, int height, int format, IntPtr firstScan, int stride);

        [DllImport("pdfium.dll")]
        internal static extern IntPtr FPDFBitmap_Destroy(IntPtr bitmapHandle);

        [DllImport("pdfium.dll")]
        internal static extern void FPDFBitmap_FillRect(IntPtr bitmapHandle, int left, int top, int width, int height, FPDFColor color);

        [DllImport("pdfium.dll")]
        internal static extern void FPDF_RenderPageBitmap(IntPtr bitmapHandle, IntPtr page, int leftX, int topY, int width, int height, int rotate, int flags);     // TODO: custom type for flags

        [DllImport("pdfium.dll")]
        internal static extern IntPtr FPDFBitmap_GetBuffer(IntPtr bitmapHandle);
    }
}
