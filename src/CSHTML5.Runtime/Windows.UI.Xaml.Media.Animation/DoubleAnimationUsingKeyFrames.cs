
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
using CSHTML5.Internal;
#if MIGRATION
using System.Windows.Controls;
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
    /// Animates the value of a Double property along a set of key frames.
    /// </summary>
    [ContentProperty("KeyFrames")]
    public sealed class DoubleAnimationUsingKeyFrames : AnimationTimeline
    {
        private DoubleKeyFrameCollection _keyFrames;

        private int _appliedKeyFramesCount = -1;

        private INTERNAL_ResolvedKeyFramesEntries<DoubleKeyFrame> _resolvedKeyFrames;

        private Dictionary<DoubleKeyFrame, DoubleAnimation> _keyFrameToDoubleAnimationMap;

        private DoubleKeyFrame _currentKeyFrame;

        //     The collection of DoubleKeyFrame objects that define the animation. The default
        //     is an empty collection.
        /// <summary>
        /// Gets the collection of DoubleKeyFrame objects that define the animation.
        /// </summary>
        public DoubleKeyFrameCollection KeyFrames
        {
            get
            {
                if (_keyFrames == null)
                {
                    _keyFrames = new DoubleKeyFrameCollection();
                }
                return _keyFrames;
            }
            set
            {
                _keyFrames = value;
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
            _resolvedKeyFrames = new INTERNAL_ResolvedKeyFramesEntries<DoubleKeyFrame>(_keyFrames);
            _keyFrameToDoubleAnimationMap = new Dictionary<DoubleKeyFrame, DoubleAnimation>();
            double? fromIfAny = null;
            DoubleKeyFrame keyFrame;
            for (int i = 0; i < KeyFrames.Count; i++)
            {
                keyFrame = KeyFrames[_resolvedKeyFrames.GetNextKeyFrameIndex(i)];
                DoubleAnimation db = InstantiateAnimationFromResolvedKeyFrameIndex(i, fromIfAny);
                _keyFrameToDoubleAnimationMap.Add(keyFrame, db);
                fromIfAny = keyFrame.Value;
            }
            _appliedKeyFramesCount = 0;
            _currentKeyFrame = null;
            _isInitialized = true;
        }

        internal override void GetTargetInformation(IterationParameters parameters)
        {
            _parameters = parameters;
            DependencyObject target;
            PropertyPath propertyPath;
            DependencyObject targetBeforePath;
            GetPropertyPathAndTargetBeforePath(parameters.Target, out targetBeforePath, out propertyPath, parameters.IsTargetParentTheTarget);
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

            GetTargetElementAndPropertyInfo(parameters.Target, out target, out propertyPath, parameters.IsTargetParentTheTarget);

            _propertyContainer = target;
            _targetProperty = propertyPath;
            _target = Storyboard.GetTarget(this);
            _targetName = Storyboard.GetTargetName(this);
        }

        internal override void Apply(IterationParameters parameters, bool isLastLoop)
        {
            _currentKeyFrame = GetNextKeyFrame();
            ApplyKeyFrame(_currentKeyFrame);
        }

        private void ApplyKeyFrame(DoubleKeyFrame keyFrame)
        {
            if (keyFrame != null)
            {
                _keyFrameToDoubleAnimationMap[keyFrame].StartFirstIteration(_parameters, true, null);
            }
        }

        private DoubleAnimation InstantiateAnimationFromResolvedKeyFrameIndex(int index, double? fromIfAny)
        {
            DoubleKeyFrame keyFrame = KeyFrames[_resolvedKeyFrames.GetNextKeyFrameIndex(index)];
            DoubleAnimation db = new DoubleAnimation()
            {
                BeginTime = TimeSpan.Zero,
                From = fromIfAny,
                To = keyFrame.Value,
                Duration = keyFrame.KeyTime.TimeSpan - (index > 0 ? KeyFrames[_resolvedKeyFrames.GetNextKeyFrameIndex(index - 1)].KeyTime.TimeSpan : TimeSpan.Zero),
                EasingFunction = keyFrame.INTERNAL_GetEasingFunction(),
                RectifyWhenAnimationEnds = false,
            };
            Storyboard.SetTargetName(db, _targetName);
            Storyboard.SetTargetProperty(db, _targetProperty);
            Storyboard.SetTarget(db, _target);
            db.InitializeIteration();
            db.Completed -= ApplyNextKeyFrame;
            db.Completed += ApplyNextKeyFrame;
            return db;
        }

        private void ApplyNextKeyFrame(object sender, EventArgs e)
        {
            _appliedKeyFramesCount++;
            if (!CheckTimeLineEndAndRaiseCompletedEvent(_parameters))
            {
                ApplyKeyFrame(GetNextKeyFrame());
            }
        }

        private DoubleKeyFrame GetNextKeyFrame()
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
                DoubleKeyFrame lastKeyFrame = _keyFrames[_resolvedKeyFrames.GetNextKeyFrameIndex(_keyFrames.Count - 1)];
                AnimationHelpers.ApplyValue(_propertyContainer, _targetProperty, lastKeyFrame.Value, _parameters.IsVisualStateChange);
            }
        }

        internal override void StopAnimation(string groupName) 
        {
            if (_isInitialized)
            {
                if(_currentKeyFrame != null)
                {
                    StopAllAnimations(groupName);
                }
            }
        }

        private void StopAllAnimations(string groupName)
        {
            foreach(var animation in _keyFrameToDoubleAnimationMap.Values)
            {
                animation.StopAnimation(groupName);
            }
        }

        object thisLock = new object();
        private bool CheckTimeLineEndAndRaiseCompletedEvent(IterationParameters parameters)
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
            StopAnimation(_parameters.VisualStateGroupName);
            _currentKeyFrame = null;
            _appliedKeyFramesCount = 0;
        }

        protected override Duration GetNaturalDurationCore()
        {
            return new Duration(LargestTimeSpanKeyTime);
        }
    }
}
