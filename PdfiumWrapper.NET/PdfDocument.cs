using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace PdfiumWrapper.NET
{
    public class PdfDocument : IDisposable
    {
        public IList<Size> PageSizes { get; private set; }
        public int PageCount
        {
            get { return pageSizes.Count; }
        }
        protected IntPtr documentHandle;
        protected List<Size> pageSizes;
        private bool isDisposed;

        public PdfDocument(string filePath)
        {
            PdfiumDLL.EnsureIsInitialized();
            documentHandle = PdfiumDLL.FPDF_LoadDocument(filePath, null);
            LoadDocumentInfo();
        }

        ~PdfDocument()
        {
            Dispose(false);
        }

        public BitmapSource RenderPage(int pageIndex, int width, int height, float dpiX, float dpiY, bool forPrinting)
        {
            IntPtr bitmapHandle = PdfiumDLL.FPDFBitmap_Create(width, height, 1);
            PdfiumDLL.FPDFBitmap_FillRect(bitmapHandle, 0, 0, width, height, new FPDFColor(0xFFFFFFFF));

            IntPtr pageHandle = PdfiumDLL.FPDF_LoadPage(documentHandle, pageIndex);
            PdfiumDLL.FPDF_RenderPageBitmap(bitmapHandle, pageHandle, 0, 0, width, height, 0, 0);
            PdfiumDLL.FPDF_ClosePage(pageHandle);

            IntPtr pixelsData = PdfiumDLL.FPDFBitmap_GetBuffer(bitmapHandle);
            byte[] buffer = new byte[width * height * 4];
            Marshal.Copy(pixelsData, buffer, 0, buffer.Length);
            PdfiumDLL.FPDFBitmap_Destroy(bitmapHandle);

            BitmapSource result = BitmapSource.Create(
                    width,
                    height,
                    dpiX,
                    dpiY,
                    PixelFormats.Bgra32,
                    null /* palette */,
                    buffer,
                    width * 4 /* stride */);

            result.Freeze();

            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                PdfiumDLL.FPDF_CloseDocument(documentHandle);
            }
            isDisposed = true;
        }

        protected void LoadDocumentInfo()
        {
            int pageCount = PdfiumDLL.FPDF_GetPageCount(documentHandle);
            pageSizes = new List<Size>(pageCount);
            double width;
            double height;

            for (int i = 0; i < pageCount; ++i)
            {
                PdfiumDLL.FPDF_GetPageSizeByIndex(documentHandle, i, out width, out height);
                pageSizes.Add(new Size() { Width = width, Height = height });
                PageSizes = pageSizes.AsReadOnly() as IList<Size>;
            }
        }
    }
}
