

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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.ComponentModel;
using System.Globalization;
#if MIGRATION
using System.Windows.Controls;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Animates the value of an Object property along a set of KeyFrames over a
    /// specified Duration.
    /// </summary>
    [ContentProperty("KeyFrames")]
    public sealed partial class ObjectAnimationUsingKeyFrames : AnimationTimeline
    {
        private ObjectKeyFrameCollection _keyFrames;

        private int _appliedKeyFramesCount;

        private INTERNAL_ResolvedKeyFramesEntries<ObjectKeyFrame> _resolvedKeyFrames;

        private Dictionary<ObjectKeyFrame, NullableTimer> _keyFramesToObjectTimers;

        /// The collection of ObjectKeyFrame objects that define the animation. The default
        /// is an empty collection.
        /// <summary>
        /// Gets the collection of ObjectKeyFrame objects that define the animation.
        /// </summary>
        public ObjectKeyFrameCollection KeyFrames
            => _keyFrames ?? (_keyFrames = new ObjectKeyFrameCollection(this));

        /// <summary>
        /// Returns the largest time span specified key time from all of the key frames.
        /// If there are not time span key times a time span of one second is returned
        /// to match the default natural duration of the From/To/By animations.
        /// </summary>
        private TimeSpan LargestTimeSpanKeyTime
        {
            get
            {
                if (_keyFrames == null || _keyFrames.Count == 0)
                {
                    return TimeSpan.FromTicks(0);
                }
                if (_resolvedKeyFrames != null)
                {
                    return _keyFrames[_resolvedKeyFrames.GetNextKeyFrameIndex(_keyFrames.Count - 1)].KeyTime.TimeSpan;
                }
                else
                {
                    throw new Exception("DoubleAnimationUsingKeyFrames has not been setup yet.");
                }
            }
        }

        private void InitializeKeyFramesSet()
        {
            _resolvedKeyFrames = new INTERNAL_ResolvedKeyFramesEntries<ObjectKeyFrame>(_keyFrames);
            _keyFramesToObjectTimers = new Dictionary<ObjectKeyFrame, NullableTimer>();
            for (int i = 0; i < KeyFrames.Count; i++)
            {
                int keyFrameIndex = _resolvedKeyFrames.GetNextKeyFrameIndex(i);
                ObjectKeyFrame keyFrame = KeyFrames[keyFrameIndex];
                NullableTimer timer = new NullableTimer(keyFrame.KeyTime.TimeSpan - (i > 0 ? KeyFrames[_resolvedKeyFrames.GetNextKeyFrameIndex(i - 1)].KeyTime.TimeSpan : TimeSpan.Zero));
                timer.Completed += ApplyNextKeyFrame;
                _keyFramesToObjectTimers.Add(keyFrame, timer);
            }
            _appliedKeyFramesCount = 0;
        }

        private void ApplyNextKeyFrame(object sender, EventArgs e)
        {
            // Apply the current key frame
            ApplyKeyFrame(GetNextKeyFrame());

            _appliedKeyFramesCount++;
            // Check timeline and go to next key frame if any.
            if (!CheckTimeLineEndAndRaiseCompletedEvent(_parameters))
            {
                StartKeyFrame(GetNextKeyFrame());
            }
        }

        internal override void GetTargetInformation(IterationParameters parameters)
        {
            _parameters = parameters;
            GetTargetElementAndPropertyInfo(parameters, out DependencyObject target, out PropertyPath propertyPath);
            _propertyContainer = target;
            _targetProperty = propertyPath;
            _propDp = GetProperty(_propertyContainer, _targetProperty);
        }

        internal override void Apply(IterationParameters parameters, bool isLastLoop)
        {
            StartKeyFrame(GetNextKeyFrame());
        }

        private void StartKeyFrame(ObjectKeyFrame keyFrame)
        {
            if (keyFrame != null)
            {
                _keyFramesToObjectTimers[keyFrame].Start();
            }
        }

        private void ApplyKeyFrame(ObjectKeyFrame keyFrame)
        {
            object value = keyFrame.Value;

            if (value != null && !_propDp.PropertyType.IsInstanceOfType(value))
            {
                if (value is Color color && _propDp.PropertyType == typeof(Brush))
                {
                    value = new SolidColorBrush(color);
                }
                else
                {
                    TypeConverter converter = TypeConverterHelper.GetConverter(_propDp.PropertyType);
                    if (converter != null && converter.CanConvertFrom(value.GetType()))
                    {
                        value = converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
                    }
                }
            }

            AnimationHelpers.ApplyValue(_propertyContainer, _targetProperty, value);
        }

        private ObjectKeyFrame GetNextKeyFrame()
        {
            int nextKeyFrameIndex = _resolvedKeyFrames.GetNextKeyFrameIndex(_appliedKeyFramesCount);
            if (nextKeyFrameIndex == -1)
            {
                return null;
            }
            else
            {
                return _keyFrames[nextKeyFrameIndex];
            }
        }

        private void ApplyLastKeyFrame(object sender, EventArgs e)
        {
            if (!_cancelledAnimation)
            {
                ObjectKeyFrame lastKeyFrame = _keyFrames[_resolvedKeyFrames.GetNextKeyFrameIndex(_keyFrames.Count - 1)];
                ApplyKeyFrame(lastKeyFrame);
            }
        }

        internal override void StopAnimation()
        {
            if (_isInitialized)
            {
                StopAllTimers();
            }
        }

        private void StopAllTimers()
        {
            if (_keyFramesToObjectTimers != null)
            {
                if (_keyFramesToObjectTimers.Values != null)
                {
                    foreach (var frameTimers in _keyFramesToObjectTimers.Values)
                    {
                        frameTimers.Stop();
                    }
                }
            }
        }


        object thisLock = new object();
        internal bool CheckTimeLineEndAndRaiseCompletedEvent(IterationParameters parameters)
        {
            bool raiseEvent = false;
            lock (thisLock)
            {
                if (_appliedKeyFramesCount >= KeyFrames.Count)
                {
                    raiseEvent = true;
                }
            }
            if (raiseEvent || _cancelledAnimation)
            {
                OnIterationCompleted(parameters);
            }
            return raiseEvent || _cancelledAnimation;
        }

        internal override void InitializeCore()
        {
            this.Completed -= ApplyLastKeyFrame;
            this.Completed += ApplyLastKeyFrame;
            InitializeKeyFramesSet();
        }

        internal override void RestoreDefaultCore()
        {
            ResetAllTimers();
            _appliedKeyFramesCount = 0;
        }

        private void ResetAllTimers()
        {
            if (_keyFramesToObjectTimers != null)
            {
                if (_keyFramesToObjectTimers.Values != null)
                {
                    foreach (var frameTimers in _keyFramesToObjectTimers.Values)
                    {
                        frameTimers.Reset();
                    }
                }
            }
        }

        protected override Duration GetNaturalDurationCore()
        {
            return new Duration(LargestTimeSpanKeyTime);
        }

        #region Provide a Timer that can be null
        private partial class NullableTimer
        {
            private bool HasTimer
            {
                get
                {
                    return _timer != null;
                }
            }

            private DispatcherTimer _timer;

            internal event EventHandler Completed;

            internal NullableTimer(TimeSpan interval)
            {
                if (interval > TimeSpan.Zero)
                {
                    _timer = NewTimer(interval);
                }
                else
                {
                    _timer = null;
                }
            }

            private DispatcherTimer NewTimer(TimeSpan interval)
            {
                DispatcherTimer timer = new DispatcherTimer()
                {
                    Interval = interval,
                };
                timer.Tick += Timer_Tick;
                return timer;
            }

#if MIGRATION
            private void Timer_Tick(object sender, EventArgs e)
#else
            private void Timer_Tick(object sender, object e)
#endif
            {
                INTERNAL_RaiseCompletedEvent();
                Stop();
            }

            private void INTERNAL_RaiseCompletedEvent()
            {
                if (Completed != null)
                    Completed(this, new EventArgs());
            }

            internal void Start()
            {
                if (HasTimer)
                {
                    _timer.Start();
                }
                else
                {
                    INTERNAL_RaiseCompletedEvent();
                }
            }

            internal void Stop()
            {
                if (HasTimer)
                {
                    _timer.Stop();
                }
            }

            internal void Reset()
            {
                if (HasTimer)
                {
                    _timer.Stop();
                    _timer = NewTimer(_timer.Interval);
                }
            }
        }
        #endregion
    }


}