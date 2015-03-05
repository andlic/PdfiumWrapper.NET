using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PdfiumWrapper.NET.TestSamples
{
    static class Program
    {
        static void Main(string[] args)
        {
            double scale = 1.5;
            int width;
            int height;

            using (var pdfDoc = new PdfDocument(@"test.pdf"))
            {
                int pageCount = pdfDoc.PageCount;
                BitmapSource pageImage;
                for (int i = 0; i < pageCount; ++i)
                {
                    width = (int)(pdfDoc.PageSizes[i].Width * scale);
                    height = (int)(pdfDoc.PageSizes[i].Height * scale);
                    pageImage = pdfDoc.GetPagePixels(i, width, height, 96, 96, false)
                            .ToBitmapSource(width, height, 96, 96);
                    SaveBitmapSource(pageImage, @"\images\page " + i.ToString() + ".png");
                }

                Bitmap pageBitmap;
                for (int i = 0; i < pageCount; ++i)
                {
                    width = (int)(pdfDoc.PageSizes[i].Width * scale);
                    height = (int)(pdfDoc.PageSizes[i].Height * scale);
                    pageBitmap = pdfDoc.GetPageImage(i, width, height, 96, 96, false);
                    pageBitmap.Save(@"\bitmaps\page " + i.ToString() + ".png");
                }
            }

            PdfiumDLL.Destroy();
        }


        public static BitmapSource ToBitmapSource(this byte[] pixelsData, int width, int height, double dpiX, double dpiY)
        {
            BitmapSource result = BitmapSource.Create(
                    width,
                    height,
                    dpiX,
                    dpiY,
                    PixelFormats.Bgra32,
                    null /* palette */,
                    pixelsData,
                    width * 4 /* stride */);

            result.Freeze();

            return result;
        }

        public static void SaveBitmapSource(BitmapSource source, string path)
        {
            BitmapFrame frame = BitmapFrame.Create(source);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(frame);

            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                encoder.Save(fs);
            }
        }
    }
}
