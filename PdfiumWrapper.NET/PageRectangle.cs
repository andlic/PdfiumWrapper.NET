
namespace PdfiumWrapper.NET
{
    public class PageRectangle
    {
        public float Left 
        {
            get { return left; }
            set { left = value; }
        }
        public float Right
        {
            get { return right; }
            set { right = value; }
        }
        public float Top 
        {
            get { return top; }
            set { top = value; }
        }
        public float Bottom 
        {
            get { return bottom; }
            set { bottom = value; }
        }
        internal float left;
        internal float right;
        internal float top;
        internal float bottom;
    }
}
