using System;

namespace PdfiumWrapper.NET
{
    public struct PageInterval
    {
        public int FirstIndex
        {
            get { return firstIndex; }
        }
        public int LastIndex
        {
            get { return lastIndex; }
        }
        private int firstIndex;
        private int lastIndex;

        public PageInterval(int firstIndex, int lastIndex)
        {
            if (firstIndex < 0)
                throw new ArgumentOutOfRangeException("firstIndex");
            if (lastIndex < 0)
                throw new ArgumentOutOfRangeException("lastIndex");
            if (firstIndex > lastIndex)
                throw new ArgumentOutOfRangeException("firstIndex cannot be bigger than lastIndex");

            this.firstIndex = firstIndex;
            this.lastIndex = lastIndex;
        }

        public bool Includes(int index)
        {
            return firstIndex <= index && index <= lastIndex;
        }
    }
}
