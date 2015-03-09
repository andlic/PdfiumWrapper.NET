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

                for (int i = 0; i < pageCount; ++i)
                {
                    width = (int)(pdfDoc.PageSizes[i].Width * scale);
                    height = (int)(pdfDoc.PageSizes[i].Height * scale);
                    pdfDoc.GetPagePixels(i, width, height, 96, 96, false)
                            .ToBitmapSource(width, height, 96, 96)
                            .Save(@"\images\page " + i.ToString() + ".png");
                }

                for (int i = 0; i < pageCount; ++i)
                {
                    width = (int)(pdfDoc.PageSizes[i].Width * scale);
                    height = (int)(pdfDoc.PageSizes[i].Height * scale);
                    var bitmap = pdfDoc.GetPageImage(i, width, height, 96, 96, false);
                    bitmap.Save(@"\bitmaps\page " + i.ToString() + ".png");
                    bitmap.Dispose();
                }

                for (int i = 0; i < pdfDoc.PageCount; ++i)
                {
                    var cropBox = pdfDoc.GetPageCropBox(i);

                    float cropDX = (cropBox.Right - cropBox.Left) / 10f;
                    float cropDY = (cropBox.Bottom - cropBox.Top) / 10f;

                    cropBox.Left += cropDX;
                    cropBox.Right -= cropDX;
                    cropBox.Top += cropDY;
                    cropBox.Bottom -= cropDY;

                    pdfDoc.SetPageCropBox(i, cropBox.Left, cropBox.Right, cropBox.Top, cropBox.Bottom);
                }

                pdfDoc.SaveCopy("test_cropped.pdf");
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

        public static void Save(this BitmapSource source, string path)
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
