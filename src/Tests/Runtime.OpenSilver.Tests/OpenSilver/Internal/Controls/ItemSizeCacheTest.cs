
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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenSilver.Internal.Controls.Tests;

[TestClass]
public class ItemSizeCacheTest
{
    [TestMethod]
    public void Empty_Cache()
    {
        var cache = new ItemSizeCache();

        Assert.AreEqual(0, cache.Count);
        Assert.AreEqual(0, cache.TotalSize);
        Assert.AreEqual(-1, cache.IndexAt(10));
        Assert.AreEqual(ItemSizeCache.DefaultEstimate, cache.Estimate);
    }

    [TestMethod]
    public void Unknown_Sizes_Should_Use_The_Average_Of_Known_Sizes()
    {
        var cache = new ItemSizeCache();
        cache.SetCount(10);

        Assert.AreEqual(10 * ItemSizeCache.DefaultEstimate, cache.TotalSize);

        cache.SetSize(0, 10);
        cache.SetSize(1, 30);

        Assert.AreEqual(20, cache.Estimate);
        Assert.IsTrue(cache.IsKnown(1));
        Assert.IsFalse(cache.IsKnown(2));
        Assert.AreEqual(20, cache.GetSize(5));
        Assert.AreEqual(40, cache.GetPosition(2));
        Assert.AreEqual(60, cache.GetPosition(3));
        Assert.AreEqual(200, cache.TotalSize);
    }

    [TestMethod]
    public void IndexAt_Boundaries()
    {
        var cache = new ItemSizeCache();
        cache.SetCount(3);
        cache.SetSize(0, 10);
        cache.SetSize(1, 20);
        cache.SetSize(2, 30);

        Assert.AreEqual(0, cache.IndexAt(-5));
        Assert.AreEqual(0, cache.IndexAt(0));
        Assert.AreEqual(0, cache.IndexAt(9.9));
        Assert.AreEqual(1, cache.IndexAt(10));
        Assert.AreEqual(1, cache.IndexAt(29.9));
        Assert.AreEqual(2, cache.IndexAt(30));
        Assert.AreEqual(2, cache.IndexAt(60));
        Assert.AreEqual(2, cache.IndexAt(1000));
        Assert.AreEqual(2, cache.IndexAt(double.PositiveInfinity));
        Assert.AreEqual(0, cache.IndexAt(double.NaN));
    }

    [TestMethod]
    public void Insert_Remove_And_Move_Should_Keep_Known_Sizes()
    {
        var cache = new ItemSizeCache();
        cache.SetCount(4);
        for (int i = 0; i < 4; i++)
        {
            cache.SetSize(i, 10 * (i + 1));
        }

        cache.Insert(1, 2);
        Assert.AreEqual(6, cache.Count);
        Assert.IsFalse(cache.IsKnown(1));
        Assert.IsFalse(cache.IsKnown(2));
        Assert.AreEqual(20, cache.GetSize(3));
        Assert.AreEqual(25, cache.GetSize(1)); // average of 10, 20, 30 and 40

        cache.Remove(0, 2);
        Assert.AreEqual(4, cache.Count);
        Assert.AreEqual(30, cache.Estimate); // average of 20, 30 and 40
        Assert.AreEqual(30 + 20, cache.GetPosition(2));

        cache.Move(3, 0);
        Assert.AreEqual(40, cache.GetSize(0));
        Assert.AreEqual(40 + 30 + 20, cache.GetPosition(3));
    }

    [TestMethod]
    public void Reset_Should_Keep_The_Estimate()
    {
        var cache = new ItemSizeCache();
        cache.SetCount(2);
        cache.SetSize(0, 40);
        cache.SetSize(1, 60);

        cache.Reset(5);

        Assert.AreEqual(5, cache.Count);
        Assert.IsFalse(cache.IsKnown(0));
        Assert.AreEqual(50, cache.Estimate);
        Assert.AreEqual(250, cache.TotalSize);

        cache.SetSize(0, 10);
        Assert.AreEqual(10, cache.Estimate);
    }

    [TestMethod]
    public void Random_Operations_Should_Match_Reference_Implementation()
    {
        var random = new Random(42);
        var cache = new ItemSizeCache();
        var reference = new List<double>();

        for (int step = 0; step < 2000; step++)
        {
            switch (random.Next(6))
            {
                case 0:
                    {
                        int index = random.Next(reference.Count + 1);
                        int count = random.Next(5);
                        cache.Insert(index, count);
                        reference.InsertRange(index, Enumerable.Repeat(double.NaN, count));
                        break;
                    }

                case 1 when reference.Count > 0:
                    {
                        int index = random.Next(reference.Count);
                        int count = random.Next(Math.Min(5, reference.Count - index) + 1);
                        cache.Remove(index, count);
                        reference.RemoveRange(index, count);
                        break;
                    }

                case 2 when reference.Count > 0:
                    {
                        int oldIndex = random.Next(reference.Count);
                        int newIndex = random.Next(reference.Count);
                        cache.Move(oldIndex, newIndex);
                        double size = reference[oldIndex];
                        reference.RemoveAt(oldIndex);
                        reference.Insert(newIndex, size);
                        break;
                    }

                default:
                    if (reference.Count > 0)
                    {
                        int index = random.Next(reference.Count);
                        double size = random.Next(4) == 0 ? 0 : random.Next(1, 100);
                        cache.SetSize(index, size);
                        reference[index] = size;
                    }
                    break;
            }

            Verify(cache, reference, random);
        }
    }

    private static void Verify(ItemSizeCache cache, List<double> reference, Random random)
    {
        Assert.AreEqual(reference.Count, cache.Count);

        double[] known = reference.Where(s => !double.IsNaN(s)).ToArray();
        double sum = known.Sum();
        if (known.Length > 0 && sum > 0)
        {
            Assert.AreEqual(sum / known.Length, cache.Estimate, 1e-9);
        }
        double estimate = cache.Estimate;

        double position = 0;
        var positions = new double[reference.Count + 1];
        for (int i = 0; i < reference.Count; i++)
        {
            positions[i] = position;
            position += double.IsNaN(reference[i]) ? estimate : reference[i];
        }
        positions[reference.Count] = position;

        for (int k = 0; k < 5 && reference.Count > 0; k++)
        {
            int index = random.Next(reference.Count + 1);
            Assert.AreEqual(positions[index], cache.GetPosition(index), 1e-6);

            double p = random.NextDouble() * (position + 10);
            int expected = 0;
            while (expected < reference.Count - 1 && positions[expected + 1] <= p)
            {
                expected++;
            }
            Assert.AreEqual(expected, cache.IndexAt(p), $"IndexAt({p})");
        }
    }
}
