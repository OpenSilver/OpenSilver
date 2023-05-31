﻿

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
using System.ComponentModel;
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
    public sealed partial class DoubleAnimationUsingKeyFrames : AnimationTimeline
    {
        private DoubleKeyFrameCollection _keyFrames;

        private int _appliedKeyFramesCount = -1;

        private INTERNAL_ResolvedKeyFramesEntries<DoubleKeyFrame> _resolvedKeyFrames;

        private Dictionary<DoubleKeyFrame, Duration> _keyFrameToDurationMap;

        //     The collection of DoubleKeyFrame objects that define the animation. The default
        //     is an empty collection.
        /// <summary>
        /// Gets the collection of DoubleKeyFrame objects that define the animation.
        /// </summary>
        public DoubleKeyFrameCollection KeyFrames
        {
            get => _keyFrames ?? (_keyFrames = new DoubleKeyFrameCollection(this));
            [EditorBrowsable(EditorBrowsableState.Never)]
            set => _keyFrames = value;
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
            _keyFrameToDurationMap = new Dictionary<DoubleKeyFrame, Duration>();
            DoubleKeyFrame keyFrame;
            for (int i = 0; i < KeyFrames.Count; i++)
            {
                keyFrame = KeyFrames[_resolvedKeyFrames.GetNextKeyFrameIndex(i)];
                _keyFrameToDurationMap.Add(keyFrame, keyFrame.KeyTime.TimeSpan - (i > 0 ? KeyFrames[_resolvedKeyFrames.GetNextKeyFrameIndex(i - 1)].KeyTime.TimeSpan : TimeSpan.Zero));
            }
            _appliedKeyFramesCount = 0;
        }

        internal override void GetTargetInformation(IterationParameters parameters)
        {
            _parameters = parameters;
            GetTargetElementAndPropertyInfo(parameters, out DependencyObject target, out PropertyPath propertyPath);
            _propertyContainer = target;
            _targetProperty = propertyPath;
            _propDp = GetProperty(_propertyContainer, _targetProperty);
            _target = Storyboard.GetTarget(this);
            _targetName = Storyboard.GetTargetName(this);
        }

        private PropertyMetadata _propertyMetadata;

        // This guid is used to specifically target a particular call to the animation. It prevents the callback which should be called when velocity's animation end 
        // to be called when the callback is called from a previous call to the animation. This could happen when the animation was started quickly multiples times in a row. 
        private Guid _animationID;

        internal override void Apply(IterationParameters parameters, bool isLastLoop)
        {
            Duration duration = ResolveDuration();
            if (IsZeroDuration(duration))
            {
                SetFinalValue();
                OnIterationCompleted(parameters);
                return;
            }

            _animationID = Guid.NewGuid();
            ApplyKeyFrame(GetNextKeyFrame(), isLastLoop);
        }

        private void ApplyKeyFrame(DoubleKeyFrame keyFrame, bool isLastLoop)
        {
            if (keyFrame != null)
            {
                //we make a specific name for this animation:
                string specificGroupName = animationInstanceSpecificName;

                bool cssEquivalentExists = false;
                if (_propertyMetadata.GetCSSEquivalent != null)
                {
                    CSSEquivalent cssEquivalent = _propertyMetadata.GetCSSEquivalent(_propertyContainer);
                    if (cssEquivalent != null)
                    {
                        cssEquivalentExists = true;
                        StartAnimation(_propertyContainer,
                            cssEquivalent,
                            null,
                            keyFrame.Value,
                            GetKeyFrameDuration(keyFrame),
                            keyFrame.INTERNAL_GetEasingFunction(),
                            specificGroupName,
                            _propDp,
                            GetKeyFrameCompletedCallback(_parameters, isLastLoop, keyFrame.Value, _propertyContainer, _targetProperty, _animationID));
                    }
                }
                //todo: use GetCSSEquivalent instead (?)
                if (_propertyMetadata.GetCSSEquivalents != null)
                {
                    List<CSSEquivalent> cssEquivalents = _propertyMetadata.GetCSSEquivalents(_propertyContainer);
                    foreach (CSSEquivalent equivalent in cssEquivalents)
                    {
                        cssEquivalentExists = true;
                        StartAnimation(_propertyContainer,
                            equivalent,
                            null,
                            keyFrame.Value,
                            GetKeyFrameDuration(keyFrame),
                            keyFrame.INTERNAL_GetEasingFunction(),
                            specificGroupName,
                            _propDp,
                            GetKeyFrameCompletedCallback(_parameters, isLastLoop, keyFrame.Value, _propertyContainer, _targetProperty, _animationID));
                    }
                }

                if (!cssEquivalentExists)
                {
                    OnKeyFrameCompleted(_parameters, isLastLoop, keyFrame.Value, _propertyContainer, _targetProperty, _animationID);
                }
            }
        }

        private Duration GetKeyFrameDuration(DoubleKeyFrame keyFrame)
        {
            return _keyFrameToDurationMap[keyFrame];
        }

        private void OnKeyFrameCompleted(IterationParameters parameters,
            bool isLastLoop,
            object value, 
            DependencyObject target,
            PropertyPath propertyPath,
            Guid callBackGuid)
        {
            if (!_isUnapplied)
            {
                if (_animationID == callBackGuid)
                {
                    AnimationHelpers.ApplyValue(target, propertyPath, value);
                    _appliedKeyFramesCount++;
                    if (!CheckTimeLineEndAndRaiseCompletedEvent(_parameters))
                    {
                        ApplyKeyFrame(GetNextKeyFrame(), isLastLoop);
                    }
                }
            }
        }

        private Action GetKeyFrameCompletedCallback(IterationParameters parameters,
            bool isLastLoop,
            object value,
            DependencyObject target,
            PropertyPath propertyPath,
            Guid callBackGuid)
        {
            return () => OnKeyFrameCompleted(parameters, isLastLoop, value, target, propertyPath, callBackGuid);
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
                SetFinalValue();
            }
        }

        private void SetFinalValue()
            => AnimationHelpers.ApplyValue(
                _propertyContainer,
                _targetProperty,
                _keyFrames[_resolvedKeyFrames.GetNextKeyFrameIndex(_keyFrames.Count - 1)].Value);

        internal override void StopAnimation()
        {
            if (_isInitialized)
            {
                string specificGroupName = animationInstanceSpecificName;

                if (_propertyMetadata.GetCSSEquivalent != null)
                {
                    CSSEquivalent cssEquivalent = _propertyMetadata.GetCSSEquivalent(_propertyContainer);
                    if (cssEquivalent != null)
                    {
                        UIElement uiElement = cssEquivalent.UIElement ?? (_propertyContainer as UIElement); // If no UIElement is specified, we assume that the property is intended to be applied to the instance on which the PropertyChanged has occurred.

                        bool hasTemplate = (uiElement is Control) && ((Control)uiElement).HasTemplate;

                        if (!hasTemplate || cssEquivalent.ApplyAlsoWhenThereIsAControlTemplate)
                        {
                            if (cssEquivalent.DomElement == null && uiElement != null)
                            {
                                cssEquivalent.DomElement = uiElement.INTERNAL_OuterDomElement; // Default value
                            }
                            if (cssEquivalent.DomElement != null)
                            {
                                string sDomElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(cssEquivalent.DomElement);
                                AnimationHelpers.StopVelocity(sDomElement, specificGroupName);
                            }
                        }
                    }
                }
                if (_propertyMetadata.GetCSSEquivalents != null)
                {
                    List<CSSEquivalent> cssEquivalents = _propertyMetadata.GetCSSEquivalents(_propertyContainer);
                    foreach (CSSEquivalent equivalent in cssEquivalents)
                    {
                        if (equivalent.DomElement != null)
                        {
                            string sDomElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(equivalent.DomElement);
                            AnimationHelpers.StopVelocity(sDomElement, specificGroupName);
                        }
                    }
                }
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
            Completed -= ApplyLastKeyFrame;            
            InitializeKeyFramesSet();
            _propertyMetadata = _propDp.GetTypeMetaData(_propertyContainer.GetType());

            if (!IsZeroDuration(ResolveDuration()))
            {
                Completed += ApplyLastKeyFrame;
            }
        }

        protected override Duration GetNaturalDurationCore()
        {
            return new Duration(LargestTimeSpanKeyTime);
        }

        private void StartAnimation(DependencyObject target, CSSEquivalent cssEquivalent, double? from, object to, Duration Duration, EasingFunctionBase easingFunction, string visualStateGroupName, DependencyProperty dependencyProperty, Action callbackForWhenfinished = null)
        {
            if (cssEquivalent.Name != null && cssEquivalent.Name.Count != 0)
            {
                UIElement uiElement = cssEquivalent.UIElement ?? (target as UIElement); // If no UIElement is specified, we assume that the property is intended to be applied to the instance on which the PropertyChanged has occurred.

                bool hasTemplate = (uiElement is Control) && ((Control)uiElement).HasTemplate;

                if (!hasTemplate || cssEquivalent.ApplyAlsoWhenThereIsAControlTemplate)
                {
                    if (cssEquivalent.DomElement == null && uiElement != null)
                    {
                        cssEquivalent.DomElement = uiElement.INTERNAL_OuterDomElement; // Default value
                    }
                    if (cssEquivalent.DomElement != null)
                    {
                        if (cssEquivalent.Value == null)
                        {
                            cssEquivalent.Value = (finalInstance, value) => { return value ?? ""; }; // Default value
                        }
                        string sCssValue = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(cssEquivalent.Value(target, to));
                        string fromToValues;
                        if (!from.HasValue)
                        {
                            fromToValues = "{" + string.Join(",", cssEquivalent.Name.Select(name => $"\"{name}\":{sCssValue}")) + "}";
                        }
                        else
                        {
                            string sFrom = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(from);
                            fromToValues = "{" + string.Join(",", cssEquivalent.Name.Select(name => $"\"{name}\":[{sCssValue},{sFrom}]")) + "}";
                        }

                        AnimationHelpers.CallVelocity(
                            this,
                            cssEquivalent.DomElement,
                            Duration,
                            easingFunction,
                            visualStateGroupName,
                            callbackForWhenfinished,
                            fromToValues);

                        target.DirtyVisualValue(dependencyProperty);
                    }
                    else
                    {
                        callbackForWhenfinished?.Invoke();
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Please set the Name property of the CSSEquivalent class.");
            }
        }
    }
}
