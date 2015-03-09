using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


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

        public byte[] GetPagePixels(int pageIndex, int width, int height, float dpiX, float dpiY, bool forPrinting)
        {
            var buffer = new byte[width * height * 4];
            return GetPagePixelsInternal(buffer, pageIndex, width, height, dpiX, dpiY, forPrinting);
        }

        public byte[] GetPagePixels(byte[] buffer, int pageIndex, int width, int height, float dpiX, float dpiY, bool forPrinting)
        {
            if (buffer.Length < width * height * 4)
                throw new ArgumentException("Buffer capacity is not enough", "buffer");

            return GetPagePixelsInternal(buffer, pageIndex, width, height, dpiX, dpiY, forPrinting);
        }

        public Bitmap GetPageImage(int pageIndex, int width, int height, float dpiX, float dpiY, bool forPrinting)
        {
            var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            BitmapData bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr bitmapHandle = PdfiumDLL.FPDFBitmap_CreateEx(width, height, 4, bitmapData.Scan0, width * 4);
            PdfiumDLL.FPDFBitmap_FillRect(bitmapHandle, 0, 0, width, height, new FPDFColor(0xFFFFFFFF));

            IntPtr pageHandle = PdfiumDLL.FPDF_LoadPage(documentHandle, pageIndex);
            PdfiumDLL.FPDF_RenderPageBitmap(bitmapHandle, pageHandle, 0, 0, width, height, 0, 0);
            PdfiumDLL.FPDF_ClosePage(pageHandle);

            bitmap.UnlockBits(bitmapData);
            PdfiumDLL.FPDFBitmap_Destroy(bitmapHandle);

            return bitmap;
        }

        public PageRectangle GetPageCropBox(int index)
        {
            var rect = new PageRectangle();
            IntPtr pageHandle = PdfiumDLL.FPDF_LoadPage(documentHandle, index);
            PdfiumDLL.FPDFPage_GetCropBox(pageHandle, out rect.left, out rect.bottom, out rect.right, out rect.top);
            PdfiumDLL.FPDF_ClosePage(pageHandle);

            return rect;
        }

        public void SetPageCropBox(int index, float left, float right, float top, float bottom)
        {
            IntPtr pageHandle = PdfiumDLL.FPDF_LoadPage(documentHandle, index);
            PdfiumDLL.FPDFPage_SetCropBox(pageHandle, left, bottom, right, top);
            PdfiumDLL.FPDF_ClosePage(pageHandle);
        }

        public void SaveCopy(string path)       // TODO: must not match path of open document
        {
            using(var stream = new FileStream(path, FileMode.Create))
            {
                var callbackStreamer = new CallbackStreamer(stream);
                PdfiumDLL.FPDF_SaveAsCopy(documentHandle, ref callbackStreamer, 0);
            }
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

        protected byte[] GetPagePixelsInternal(byte[] buffer, int pageIndex, int width, int height, float dpiX, float dpiY, bool forPrinting)
        {
            IntPtr bitmapHandle;
            bitmapHandle = PdfiumDLL.FPDFBitmap_CreateEx(width, height, 4, buffer, width * 4);
            PdfiumDLL.FPDFBitmap_FillRect(bitmapHandle, 0, 0, width, height, new FPDFColor(0xFFFFFFFF));

            IntPtr pageHandle = PdfiumDLL.FPDF_LoadPage(documentHandle, pageIndex);
            PdfiumDLL.FPDF_RenderPageBitmap(bitmapHandle, pageHandle, 0, 0, width, height, 0, 0);
            PdfiumDLL.FPDF_ClosePage(pageHandle);

            PdfiumDLL.FPDFBitmap_Destroy(bitmapHandle);

            return buffer;
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
