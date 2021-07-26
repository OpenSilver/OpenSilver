using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
    public class PriorityQueue<TValue>
    {
        private static readonly TValue DefaultValue = default(TValue);

        public int Count { get { return count; } }

        private Queue<TValue>[] queues;
        private int count;
        private int topPriority;

        public PriorityQueue(int maxPriotiry)
        {
            queues = new Queue<TValue>[maxPriotiry];
        }

        public void Enqueue(int priotiry, TValue value)
        {
            Queue<TValue> queue = queues[priotiry];
            if (queue == null)
            {
                queue = new Queue<TValue>();
                queues[priotiry] = queue;
            }

            queue.Enqueue(value);
            count++;

            if (topPriority < priotiry)
            {
                topPriority = priotiry;
            }
        }

        public TValue Dequeue()
        {
            TValue value;
            if (TryDequeue(out value))
            {
                return value;
            }

            throw new InvalidOperationException("Queue is empty");
        }

        public bool TryDequeue(out TValue value)
        {
            if (count == 0)
            {
                value = DefaultValue;
                return false;
            }

            while (queues[topPriority] == null || queues[topPriority].Count == 0)
            {
                topPriority--;
            }

            value = queues[topPriority].Dequeue();
            count--;

            return true;
        }

        public TValue Peek()
        {
            TValue value;
            if (TryPeek(out value))
            {
                return value;
            }

            throw new InvalidOperationException("Queue is empty");
        }

        public bool TryPeek(out TValue value)
        {
            if (count == 0)
            {
                value = DefaultValue;
                return false;
            }

            while (queues[topPriority] == null || queues[topPriority].Count == 0)
            {
                topPriority--;
            }

            value = queues[topPriority].Peek();

            if (value == null)
            {
                // process when unexpected null entry exists
                value = Dequeue();
                return TryPeek(out value);
            }

            return true;
        }
    }
}