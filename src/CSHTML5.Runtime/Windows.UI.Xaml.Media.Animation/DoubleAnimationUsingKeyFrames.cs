
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
    public sealed class DoubleAnimationUsingKeyFrames : Timeline
    {
        string _targetName;
        PropertyPath _targetProperty;
        DependencyObject _target;

        private IterationParameters _parameters;
        private bool _isLastLoop;

        private DoubleKeyFrameCollection _keyFrames;

        private int _appliedKeyFramesCount;
        private TimeSpan _ellapsedTime;

        private ResolvedKeyFramesEntries _resolvedKeyFrames;

        private DoubleKeyFrame _currentKeyFrame;
        private DoubleAnimation _currentAnimation;
        //     The collection of DoubleKeyFrame objects that define the animation. The default
        //     is an empty collection.
        /// <summary>
        /// Gets the collection of DoubleKeyFrame objects that define the animation.
        /// </summary>
        public DoubleKeyFrameCollection KeyFrames
        {
            get
            {
                if(_keyFrames == null)
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

        private void InitializeKeyFramesSet()
        {
            _resolvedKeyFrames = new ResolvedKeyFramesEntries(_keyFrames);
            _appliedKeyFramesCount = 0;
            _ellapsedTime = new TimeSpan();
        }

        internal override void Apply(IterationParameters parameters, bool isLastLoop)
        {
            StopCurrentAnimation(parameters.Target, revertToFormerValue: false);

            _parameters = parameters;
            _isLastLoop = isLastLoop;
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
            //DependencyObject lastElementBeforeProperty = propertyPath.INTERNAL_AccessPropertyContainer(target);
            DependencyProperty dp = GetProperty(target, propertyPath);

            _currentKeyFrame = GetNextKeyFrame();

            _target = parameters.Target;
            _targetProperty = propertyPath;
            _targetName = Storyboard.GetTargetName(this);

            ApplyKeyFrame(_currentKeyFrame);
        }

        private void ApplyKeyFrame(DoubleKeyFrame keyFrame)
        {
            if (keyFrame != null)
            {
                _currentAnimation = new DoubleAnimation()
                {
                    To = keyFrame.Value,
                    Duration = keyFrame.KeyTime.TimeSpan - _ellapsedTime,
                    BeginTime = TimeSpan.FromTicks(0),
                    EasingFunction = keyFrame.INTERNAL_GetEasingFunction(),
                };
                _currentAnimation.Completed += ApplyNextKeyFrame;
                Storyboard.SetTargetName(_currentAnimation, _targetName);
                Storyboard.SetTargetProperty(_currentAnimation, _targetProperty);
                Storyboard.SetTarget(_currentAnimation, _target);
                _currentAnimation.InitializeIteration();
                _currentAnimation.StartFirstIteration(_parameters, _isLastLoop, new TimeSpan());
                CheckTimeLineEndAndRaiseCompletedEvent(_parameters);
            }
        }

        private void ApplyNextKeyFrame(object sender, EventArgs e)
        {
            _appliedKeyFramesCount++;
            _ellapsedTime = _currentKeyFrame.KeyTime.TimeSpan;
            _currentKeyFrame = GetNextKeyFrame();
            ApplyKeyFrame(_currentKeyFrame);
        }


        private DoubleKeyFrame GetNextKeyFrame()
        {
            int nextKeyFrameIndex = _resolvedKeyFrames.GetNextKeyFrameIndex(_appliedKeyFramesCount);
            if(nextKeyFrameIndex == -1)
            {
                return null;
            }
            else
            {
                return _keyFrames[nextKeyFrameIndex];
            }
        }


        internal override void Stop(FrameworkElement frameworkElement, string groupName, bool revertToFormerValue = false) //frameworkElement is for the animations requiring the use of GetCssEquivalent
        {
            base.Stop(frameworkElement, groupName, revertToFormerValue);

            StopCurrentAnimation(frameworkElement, groupName, revertToFormerValue);
        }

        private void StopCurrentAnimation(FrameworkElement frameworkElement, string groupName = "visualStateGroupName", bool revertToFormerValue = false)
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.Stop(frameworkElement, groupName, revertToFormerValue);
                _currentAnimation = null;
            }
        }

        object thisLock = new object();
        internal void CheckTimeLineEndAndRaiseCompletedEvent(IterationParameters parameters)
        {
            bool raiseEvent = false;
            lock (thisLock)
            {
                if (_appliedKeyFramesCount >= _keyFrames.Count)
                {
                    raiseEvent = true;
                }
            }
            if (raiseEvent)
            {
                OnIterationCompleted(parameters);
            }
        }

        internal override void IterateOnce(IterationParameters parameters, bool isLastLoop)
        {
            InitializeKeyFramesSet();
            base.IterateOnce(parameters, isLastLoop);
            Apply(parameters, isLastLoop);
        }
    }
}
