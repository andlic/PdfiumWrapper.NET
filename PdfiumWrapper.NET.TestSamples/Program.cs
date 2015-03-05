using System.IO;
using System.Windows.Media.Imaging;

namespace PdfiumWrapper.NET.TestSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            double scale = 1.5;
            BitmapSource pageImage;
            int width;
            int height;

            using (var pdfDoc = new PdfDocument(@"test.pdf"))
            {
                int pageCount = pdfDoc.PageCount;
                for (int i = 0; i < pageCount; ++i)
                {
                    width = (int)(pdfDoc.PageSizes[i].Width * scale);
                    height = (int)(pdfDoc.PageSizes[i].Height * scale);
                    pageImage = pdfDoc.RenderPage(i, width, height, 96, 96, false);
                    SaveBitmapSource(pageImage, @"\images\page " + i.ToString() + ".png");
                }
            }

            PdfiumDLL.Destroy();
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
