
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
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
    public sealed class ObjectAnimationUsingKeyFrames : AnimationTimeline
    {
        private DependencyProperty _dp;

        private ObjectKeyFrameCollection _keyFrames;

        private int _appliedKeyFramesCount;

        private INTERNAL_ResolvedKeyFramesEntries<ObjectKeyFrame> _resolvedKeyFrames;

        private Dictionary<ObjectKeyFrame, NullableTimer> _keyFramesToObjectTimers;

        private ObjectKeyFrame _currentKeyFrame;

        /// The collection of ObjectKeyFrame objects that define the animation. The default
        /// is an empty collection.
        /// <summary>
        /// Gets the collection of ObjectKeyFrame objects that define the animation.
        /// </summary>
        public ObjectKeyFrameCollection KeyFrames
        {
            get
            {
                if (_keyFrames == null)
                {
                    _keyFrames = new ObjectKeyFrameCollection();
                }
                return _keyFrames;
            }
        }

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
            _currentKeyFrame = null;
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
            DependencyObject target;
            PropertyPath propertyPath;
            DependencyObject targetBeforePath;
            GetPropertyPathAndTargetBeforePath(parameters.Target, out targetBeforePath, out propertyPath);
            DependencyObject parentElement = targetBeforePath; //this will be the parent of the clonable element (if any).
            foreach (Tuple<DependencyObject, DependencyProperty, int?> element in GoThroughElementsToAccessProperty(propertyPath, targetBeforePath))
            {
                DependencyObject depObject = element.Item1;
                DependencyProperty depProp = element.Item2;
                int? index = element.Item3;
                if (depObject is ICloneOnAnimation)
                {
                    if (!((ICloneOnAnimation)depObject).IsAlreadyAClone())
                    {
                        object clone = ((ICloneOnAnimation)depObject).Clone();
                        if (index != null)
                        {
#if BRIDGE
                            parentElement.GetType().GetProperty("Item").SetValue(parentElement, clone, new object[] { index });
#else
                            //JSIL does not support SetValue(object, object, object[])
#endif
                        }
                        else
                        {
                            parentElement.SetValue(depProp, clone);
                        }
                    }
                    break;
                }
                else
                {
                    parentElement = depObject;
                }
            }

            GetTargetElementAndPropertyInfo(parameters.Target, out target, out propertyPath);

            _dp = GetProperty(target, propertyPath);
            _propertyContainer = target;
            _targetProperty = propertyPath;
        }

        internal override void Apply(IterationParameters parameters, bool isLastLoop)
        {
            _currentKeyFrame = GetNextKeyFrame();
            StartKeyFrame(_currentKeyFrame);
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
            if (value is string && _dp.PropertyType != typeof(string))
            {
                if (_dp.PropertyType.IsEnum)
                {
                    value = Enum.Parse(_dp.PropertyType, (string)value);
                }
                else
                {
                    value = DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(_dp.PropertyType, (string)value);
                }
            }

            object castedValue = DynamicCast(value, _dp.PropertyType);

            AnimationHelpers.ApplyValue(_propertyContainer, _targetProperty, castedValue, _parameters.IsVisualStateChange);
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

        internal override void StopAnimation(string groupName)
        {
            if (_isInitialized)
            {
                if(_currentKeyFrame != null)
                {
                    StopAllTimers();
                }
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
                if (_appliedKeyFramesCount >= _keyFrames.Count)
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
            _currentKeyFrame = null;
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
        private class NullableTimer
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