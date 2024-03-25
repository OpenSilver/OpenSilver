
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

namespace System.Collections.Generic;

public class PriorityQueue<TValue>
{
    private readonly Queue<TValue>[] _queues;
    private int _count;
    private int _topPriority;

    public PriorityQueue(int maxPriority)
    {
        _queues = new Queue<TValue>[maxPriority];
    }

    public int Count => _count;

    internal int MaxPriority => _topPriority;

    public void Enqueue(int priority, TValue value)
    {
        if (priority < 0 || priority >= _queues.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(priority));
        }

        Queue<TValue> queue = _queues[priority];
        if (queue == null)
        {
            queue = new Queue<TValue>();
            _queues[priority] = queue;
        }

        queue.Enqueue(value);
        _count++;

        if (_topPriority < priority)
        {
            _topPriority = priority;
        }
    }

    public TValue Dequeue()
    {
        if (TryDequeue(out TValue value))
        {
            return value;
        }

        throw new InvalidOperationException("Queue is empty");
    }

    public bool TryDequeue(out TValue value)
    {
        if (_count == 0)
        {
            value = default;
            return false;
        }

        value = _queues[_topPriority].Dequeue();
        _count--;

        while (_topPriority > 0)
        {
            if (_queues[_topPriority] != null && _queues[_topPriority].Count > 0)
            {
                break;
            }
            
            _topPriority--;
        }

        return true;
    }

    public TValue Peek()
    {
        if (TryPeek(out TValue value))
        {
            return value;
        }

        throw new InvalidOperationException("Queue is empty");
    }

    public bool TryPeek(out TValue value)
    {
        if (_count == 0)
        {
            value = default;
            return false;
        }

        value = _queues[_topPriority].Peek();

        return true;
    }
}