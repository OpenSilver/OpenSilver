
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

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenSilver.Internal.Controls;

/// <summary>
/// Sizes of the items of a virtualizing panel along its stacking direction. Sizes of the items
/// that have never been measured are estimated with the average size of the measured ones.
/// </summary>
/// <remarks>
/// Positions are computed in O(log n) with Fenwick trees of the known sizes and of their count.
/// </remarks>
internal sealed class ItemSizeCache
{
    internal const double DefaultEstimate = 16.0;

    private readonly List<double> _sizes = new(); // NaN when unknown
    private double[] _sumTree;
    private int[] _countTree;
    private bool _isTreeValid;
    private double _knownSum;
    private int _knownCount;
    private double _fallbackEstimate = DefaultEstimate;

    public int Count => _sizes.Count;

    public double Estimate => _knownCount > 0 && _knownSum > 0.0 ? _knownSum / _knownCount : _fallbackEstimate;

    public bool IsKnown(int index) => !double.IsNaN(_sizes[index]);

    public double GetSize(int index)
    {
        double size = _sizes[index];
        return double.IsNaN(size) ? Estimate : size;
    }

    public void SetSize(int index, double size)
    {
        Debug.Assert(size >= 0.0 && !double.IsInfinity(size));

        double oldSize = _sizes[index];
        if (oldSize == size)
        {
            return;
        }

        bool wasKnown = !double.IsNaN(oldSize);
        _sizes[index] = size;
        _knownSum += wasKnown ? size - oldSize : size;
        if (!wasKnown)
        {
            _knownCount++;
        }

        if (_isTreeValid)
        {
            double sizeDelta = wasKnown ? size - oldSize : size;
            int countDelta = wasKnown ? 0 : 1;
            for (int i = index + 1; i <= _sizes.Count; i += i & -i)
            {
                _sumTree[i] += sizeDelta;
                _countTree[i] += countDelta;
            }
        }
    }

    public void SetCount(int count)
    {
        if (count < _sizes.Count)
        {
            Remove(count, _sizes.Count - count);
        }
        else if (count > _sizes.Count)
        {
            Insert(_sizes.Count, count - _sizes.Count);
        }
    }

    public void Insert(int index, int count)
    {
        Debug.Assert(index >= 0 && index <= _sizes.Count && count >= 0);

        if (count == 0) return;

        if (index == _sizes.Count)
        {
            for (int i = 0; i < count; i++)
            {
                _sizes.Add(double.NaN);
            }
        }
        else
        {
            var unknown = new double[count];
            for (int i = 0; i < count; i++)
            {
                unknown[i] = double.NaN;
            }
            _sizes.InsertRange(index, unknown);
        }

        _isTreeValid = false;
    }

    public void Remove(int index, int count)
    {
        Debug.Assert(index >= 0 && count >= 0 && index + count <= _sizes.Count);

        if (count == 0) return;

        for (int i = index; i < index + count; i++)
        {
            double size = _sizes[i];
            if (!double.IsNaN(size))
            {
                _knownSum -= size;
                _knownCount--;
            }
        }

        _sizes.RemoveRange(index, count);
        _isTreeValid = false;
    }

    public void Move(int oldIndex, int newIndex)
    {
        double size = _sizes[oldIndex];
        _sizes.RemoveAt(oldIndex);
        _sizes.Insert(newIndex, size);
        _isTreeValid = false;
    }

    /// <summary>
    /// Forgets the measured sizes. The current estimate is kept for the items to come.
    /// </summary>
    public void Reset(int count)
    {
        _fallbackEstimate = Estimate;
        _sizes.Clear();
        _knownSum = 0.0;
        _knownCount = 0;
        _isTreeValid = false;
        Insert(0, count);
    }

    /// <summary>
    /// Returns the position of the item at the specified index, that is to say the sum of the sizes
    /// of the items before it. <paramref name="index"/> can be <see cref="Count"/>, in which case
    /// the total size is returned.
    /// </summary>
    public double GetPosition(int index)
    {
        Debug.Assert(index >= 0 && index <= _sizes.Count);

        EnsureTrees();

        double sum = 0.0;
        int count = 0;
        for (int i = index; i > 0; i -= i & -i)
        {
            sum += _sumTree[i];
            count += _countTree[i];
        }

        return sum + (index - count) * Estimate;
    }

    public double TotalSize => GetPosition(_sizes.Count);

    /// <summary>
    /// Returns the index of the item at the specified position, clamped to [0, Count - 1].
    /// Returns -1 if there are no items.
    /// </summary>
    public int IndexAt(double position)
    {
        int n = _sizes.Count;
        if (n == 0)
        {
            return -1;
        }

        if (!(position > 0.0))
        {
            return 0;
        }

        EnsureTrees();

        double estimate = Estimate;
        int index = 0;
        double end = 0.0;
        for (int step = HighestPowerOfTwo(n); step > 0; step >>= 1)
        {
            int next = index + step;
            if (next <= n)
            {
                double size = _sumTree[next] + (step - _countTree[next]) * estimate;
                if (end + size <= position)
                {
                    index = next;
                    end += size;
                }
            }
        }

        // 'index' items end before (or at) the position.
        return Math.Min(index, n - 1);
    }

    private void EnsureTrees()
    {
        if (_isTreeValid)
        {
            return;
        }

        int n = _sizes.Count;
        if (_sumTree is null || _sumTree.Length < n + 1)
        {
            int capacity = Math.Max(n + 1, (_sumTree?.Length ?? 0) * 2);
            _sumTree = new double[capacity];
            _countTree = new int[capacity];
        }
        else
        {
            Array.Clear(_sumTree, 0, n + 1);
            Array.Clear(_countTree, 0, n + 1);
        }

        for (int i = 1; i <= n; i++)
        {
            double size = _sizes[i - 1];
            if (!double.IsNaN(size))
            {
                _sumTree[i] += size;
                _countTree[i] += 1;
            }

            int parent = i + (i & -i);
            if (parent <= n)
            {
                _sumTree[parent] += _sumTree[i];
                _countTree[parent] += _countTree[i];
            }
        }

        _isTreeValid = true;
    }

    private static int HighestPowerOfTwo(int n)
    {
        int p = 1;
        while (p <= n >> 1)
        {
            p <<= 1;
        }
        return p;
    }
}
