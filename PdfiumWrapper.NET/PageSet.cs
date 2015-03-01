using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PdfiumWrapper.NET
{
    public class PageSet : IEnumerable, IEnumerable<int>
    {
        protected LinkedList<PageInterval> intervals = new LinkedList<PageInterval>();
        protected int version = 0;

        #region IEnumerable implementation
        public IEnumerator<int> GetEnumerator()
        {
            return new Enumerator(this);
        }
        #endregion

        #region IEnumerable<int> implementaion
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
        #endregion

        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            foreach(var interval in intervals)
            {
                if (interval.FirstIndex == interval.LastIndex)
                    strBuilder.Append(interval.FirstIndex + ", ");
                else
                    strBuilder.Append(interval.FirstIndex + "-" + interval.LastIndex + ", ");
            }

            if (strBuilder.Length > 0)
                strBuilder.Remove(strBuilder.Length - 2, 2);

            return strBuilder.ToString();
        }

        public void Add(params int[] indexes)
        {
            foreach (int index in indexes)
                Add(index);
        }

        public void Add(int index)
        {
            if(index < 0)
                throw new ArgumentOutOfRangeException("index");

            Add(new PageInterval(index, index));
        }

        public void Add(params PageInterval[] intervals)
        {
            foreach (var interval in intervals)
                Add(interval);
        }

        public void Add(PageInterval interval)
        {
            var newNode = new LinkedListNode<PageInterval>(interval);
            LinkedListNode<PageInterval> currNode = intervals.Last;
            ++version;

            while (currNode != null)
            {
                PageInterval currInterval = currNode.Value;
                if (currInterval.FirstIndex > interval.FirstIndex)
                {
                    currNode = currNode.Previous;
                    continue;
                }

                if (currInterval.Includes(interval.FirstIndex))
                {
                    if (!currInterval.Includes(interval.LastIndex))
                        MergeFollowing(currNode, interval.LastIndex);
                    // Else new interval belongs to already defined in current set (no action required)
                    return;
                }

                intervals.AddAfter(currNode, newNode);
                MergeFollowing(newNode, interval.LastIndex);
                return;
            }

            intervals.AddFirst(newNode);
            MergeFollowing(newNode, interval.LastIndex);
        }

        protected void MergeFollowing(LinkedListNode<PageInterval> origin, int lastIndex)
        {
            LinkedListNode<PageInterval> currNode = origin.Next;

            while(currNode != null)
            {
                if (currNode.Value.LastIndex < lastIndex)
                {
                    var temp = currNode.Next;
                    origin.List.Remove(currNode);
                    currNode = temp;
                }
                else
                {
                    if (currNode.Value.FirstIndex <= lastIndex)
                    {
                        origin.Value = new PageInterval(origin.Value.FirstIndex, currNode.Value.LastIndex);
                        origin.List.Remove(currNode);
                        return;
                    }
                    else
                        break;
                }
            }

            origin.Value = new PageInterval(origin.Value.FirstIndex, lastIndex);
        }


        public struct Enumerator : IEnumerator, IEnumerator<int>
        {
            private PageSet owner;
            private readonly int version;
            private int current;
            private LinkedListNode<PageInterval> nextNode;
            int intervalEnd;

            internal Enumerator(PageSet owner)
            {
                this.owner = owner;
                this.version = owner.version;
                this.current = -1;
                this.nextNode = owner.intervals.First;
                this.intervalEnd = -1;
            }

            public int Current
            {
                get { return current; }
            }

            object IEnumerator.Current
            {
                get { return (object)current; }
            }

            public bool MoveNext()
            {
                if (version != owner.version)
                    throw new InvalidOperationException("Enumerator is invalid because set was modified");

                if (current < intervalEnd)
                    ++current;
                else
                {
                    if (nextNode == null)
                        return false;

                    current = nextNode.Value.FirstIndex;
                    intervalEnd = nextNode.Value.LastIndex;
                    nextNode = nextNode.Next;
                }

                return true;
            }

            public void Reset()
            {
                current = -1;
                nextNode = owner.intervals.First;
                intervalEnd = -1;
            }

            public void Dispose() { }
        }
    }
}
