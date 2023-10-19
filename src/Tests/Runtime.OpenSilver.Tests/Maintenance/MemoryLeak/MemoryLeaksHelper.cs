
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
using System.Windows;

namespace OpenSilver.MemoryLeak;

public static class MemoryLeaksHelper
{
    public static void Collect()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    private static readonly DependencyProperty TrackerHolderProperty =
        DependencyProperty.RegisterAttached(
            "TrackerHolder",
            typeof(TrackerHolder),
            typeof(MemoryLeaksHelper),
            null);

    public static GCTracker GetTracker(DependencyObject d)
    {
        if (d is null)
        {
            throw new ArgumentNullException(nameof(d));
        }

        var trackerHolder = (TrackerHolder)d.GetValue(TrackerHolderProperty);
        if (trackerHolder != null)
        {
            return trackerHolder.Tracker;
        }

        return null;
    }

    public static void SetTracker(DependencyObject d, GCTracker tracker)
    {
        if (d is null)
        {
            throw new ArgumentNullException(nameof(d));
        }

        if (tracker is null)
        {
            throw new ArgumentNullException(nameof(tracker));
        }

        d.SetValue(TrackerHolderProperty, new TrackerHolder(tracker));
    }

    private sealed class TrackerHolder
    {
        public TrackerHolder(GCTracker tracker)
        {
            Tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
        }

        ~TrackerHolder() => Tracker.MarkAsCollected();

        public GCTracker Tracker { get; }
    }
}