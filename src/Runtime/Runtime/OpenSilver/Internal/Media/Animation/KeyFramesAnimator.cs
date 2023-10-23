
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
using System.Windows;
using System.Windows.Media.Animation;

namespace OpenSilver.Internal.Media.Animation;

internal sealed class KeyFramesAnimator<T> : IValueAnimator<T>
{
    private readonly IKeyFrameAnimation<T> _animation;
    private ResolvedKeyFrameEntry[] _sortedResolvedKeyFrames;
    private bool _areKeyTimesValid;

    public KeyFramesAnimator(IKeyFrameAnimation<T> animation)
    {
        Debug.Assert(animation is not null);
        _animation = animation;
    }

    public T GetCurrentValue(T initialValue, DependencyProperty dp, TimelineClock clock)
    {
        if (_animation.KeyFrames is null)
        {
            return initialValue;
        }

        if (!_areKeyTimesValid)
        {
            ResolveKeyTimes();
        }

        if (_sortedResolvedKeyFrames is null)
        {
            return initialValue;
        }

        int keyFrameCount = _sortedResolvedKeyFrames.Length;
        int maxKeyFrameIndex = keyFrameCount - 1;

        T currentIterationValue;

        Debug.Assert(maxKeyFrameIndex >= 0, "maxKeyFrameIndex is less than zero which means we don't actually have any key frames.");

        int currentResolvedKeyFrameIndex = 0;

        // Skip all the key frames with key times lower than the current time.
        // currentResolvedKeyFrameIndex will be greater than maxKeyFrameIndex 
        // if we are past the last key frame.
        while (currentResolvedKeyFrameIndex < keyFrameCount
               && clock.CurrentTime > _sortedResolvedKeyFrames[currentResolvedKeyFrameIndex]._resolvedKeyTime)
        {
            currentResolvedKeyFrameIndex++;
        }

        // If there are multiple key frames at the same key time, be sure to go to the last one.
        while (currentResolvedKeyFrameIndex < maxKeyFrameIndex
               && clock.CurrentTime == _sortedResolvedKeyFrames[currentResolvedKeyFrameIndex + 1]._resolvedKeyTime)
        {
            currentResolvedKeyFrameIndex++;
        }

        if (currentResolvedKeyFrameIndex == keyFrameCount)
        {
            // Past the last key frame.
            currentIterationValue = GetResolvedKeyFrameValue(maxKeyFrameIndex);
        }
        else if (clock.CurrentTime == _sortedResolvedKeyFrames[currentResolvedKeyFrameIndex]._resolvedKeyTime)
        {
            // Exactly on a key frame.
            currentIterationValue = GetResolvedKeyFrameValue(currentResolvedKeyFrameIndex);
        }
        else
        {
            // Between two key frames.
            double currentSegmentProgress;
            T fromValue;

            if (currentResolvedKeyFrameIndex == 0)
            {
                // The current key frame is the first key frame so we have
                // some special rules for determining the fromValue and an
                // optimized method of calculating the currentSegmentProgress.

                fromValue = initialValue;

                // Current segment time divided by the segment duration.
                // Note: the reason this works is that we know that we're in
                // the first segment, so we can assume:
                //
                // currentTime.TotalMilliseconds                                  = current segment time
                // _sortedResolvedKeyFrames[0]._resolvedKeyTime.TotalMilliseconds = current segment duration

                currentSegmentProgress = clock.CurrentTime.TotalMilliseconds
                                       / _sortedResolvedKeyFrames[0]._resolvedKeyTime.TotalMilliseconds;
            }
            else
            {
                int previousResolvedKeyFrameIndex = currentResolvedKeyFrameIndex - 1;
                TimeSpan previousResolvedKeyTime = _sortedResolvedKeyFrames[previousResolvedKeyFrameIndex]._resolvedKeyTime;

                fromValue = GetResolvedKeyFrameValue(previousResolvedKeyFrameIndex);

                TimeSpan segmentCurrentTime = clock.CurrentTime - previousResolvedKeyTime;
                TimeSpan segmentDuration = _sortedResolvedKeyFrames[currentResolvedKeyFrameIndex]._resolvedKeyTime - previousResolvedKeyTime;

                currentSegmentProgress = segmentCurrentTime.TotalMilliseconds
                                       / segmentDuration.TotalMilliseconds;
            }

            currentIterationValue = GetResolvedKeyFrame(currentResolvedKeyFrameIndex).InterpolateValue(fromValue, currentSegmentProgress);
        }

        return currentIterationValue;
    }

    private void ResolveKeyTimes()
    {
        Debug.Assert(!_areKeyTimesValid, $"KeyFramesAnimator<T>.ResolveKeyTimes() shouldn't be called if the key times are already valid.");

        int keyFrameCount = 0;

        if (_animation.KeyFrames != null)
        {
            keyFrameCount = _animation.KeyFrames.Count;
        }

        if (keyFrameCount == 0)
        {
            _sortedResolvedKeyFrames = null;
            _areKeyTimesValid = true;
            return;
        }

        _sortedResolvedKeyFrames = new ResolvedKeyFrameEntry[keyFrameCount];

        // Initialize the _originalKeyFrameIndex.
        for (int i = 0; i < keyFrameCount; i++)
        {
            _sortedResolvedKeyFrames[i]._originalKeyFrameIndex = i;
        }

        for (int i = 0; i < keyFrameCount; i++)
        {
            KeyTime keyTime = _animation.KeyFrames[i].KeyTime;

            _sortedResolvedKeyFrames[i]._resolvedKeyTime = keyTime.Type switch
            {
                KeyTimeType.TimeSpan => keyTime.TimeSpan,
                _ => throw new InvalidOperationException("KeyTime property on KeyFrame object must be set to a non-negative TimeSpan value."),
            };
        }

        //
        // Sort resolved key frame entries.
        //

        Array.Sort(_sortedResolvedKeyFrames);

        _areKeyTimesValid = true;
    }

    private T GetResolvedKeyFrameValue(int resolvedKeyFrameIndex)
    {
        Debug.Assert(_areKeyTimesValid, "The key frames must be resolved and sorted before calling GetResolvedKeyFrameValue");

        return GetResolvedKeyFrame(resolvedKeyFrameIndex).Value;
    }

    private IKeyFrame<T> GetResolvedKeyFrame(int resolvedKeyFrameIndex)
    {
        Debug.Assert(_areKeyTimesValid, "The key frames must be resolved and sorted before calling GetResolvedKeyFrame");

        return _animation.KeyFrames[_sortedResolvedKeyFrames[resolvedKeyFrameIndex]._originalKeyFrameIndex];
    }
}
