using System;
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

            using (var pdfDoc = new PdfDocument(@"d:\2.pdf"))
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

            var set = new PageSet();
            set.Add(42);
            set.Add(23);
            set.Add(new PageInterval(16, 20));
            set.Add(new PageInterval(50, 70));

            foreach (var index in set)
                Console.Write(index.ToString() + "  ");

            set.Add(new PageInterval(52, 73));
            set.Add(new PageInterval(40, 50));
            set.Add(new PageInterval(10, 80));
            set.Add(new PageInterval(11, 70));
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
