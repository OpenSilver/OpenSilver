
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
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace OpenSilver.Internal.Media.Animation;

internal interface IKeyFrameAnimation<T>
{
    IKeyFrameCollection<T> KeyFrames { get; }
}

internal interface IKeyFrameCollection<T>
{
    IKeyFrame<T> this[int index] { get; }
    int Count { get; }
}

internal interface IKeyFrame<T> : IKeyFrame
{
    /// <summary>
    /// The value associated with the key frame.
    /// </summary>
    new T Value { get; }

    T InterpolateValue(T baseValue, double keyFrameProgress);
}

internal static class KeyFrameAnimationHelpers
{
    /// <summary>
    /// Returns the largest time span specified key time from all of the key frames.
    /// If there are not time span key times a time span of one second is returned
    /// to match the default natural duration of the From/To/By animations.
    /// </summary>
    public static TimeSpan GetLargestTimeSpanKeyTime<T>(IKeyFrameAnimation<T> animation)
    {
        Debug.Assert(animation is not null);

        IKeyFrameCollection<T> frames = animation.KeyFrames;

        bool hasTimeSpanKeyTime = false;
        TimeSpan largestTimeSpanKeyTime = TimeSpan.Zero;

        if (frames != null)
        {
            int keyFrameCount = frames.Count;

            for (int i = 0; i < keyFrameCount; i++)
            {
                KeyTime keyTime = frames[i].KeyTime;

                if (keyTime.Type == KeyTimeType.TimeSpan)
                {
                    hasTimeSpanKeyTime = true;

                    if (keyTime.TimeSpan > largestTimeSpanKeyTime)
                    {
                        largestTimeSpanKeyTime = keyTime.TimeSpan;
                    }
                }
            }
        }

        if (hasTimeSpanKeyTime)
        {
            return largestTimeSpanKeyTime;
        }
        else
        {
            return AnimationTimeline.DefaultDuration;
        }
    }
}
