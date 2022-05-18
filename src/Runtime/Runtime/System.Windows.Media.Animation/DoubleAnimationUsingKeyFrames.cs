

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
            DependencyObject target;
            PropertyPath propertyPath;
            DependencyObject targetBeforePath;
            GetPropertyPathAndTargetBeforePath(parameters, out targetBeforePath, out propertyPath);
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

            GetTargetElementAndPropertyInfo(parameters, out target, out propertyPath);

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
            _animationID = Guid.NewGuid();
            ApplyKeyFrame(GetNextKeyFrame(), isLastLoop);
        }

        private void ApplyKeyFrame(DoubleKeyFrame keyFrame, bool isLastLoop)
        {
            if (keyFrame != null)
            {
                //we make a specific name for this animation:
                string specificGroupName = animationInstanceSpecificName.ToString();

                bool cssEquivalentExists = false;
                if (_propertyMetadata.GetCSSEquivalent != null)
                {
                    CSSEquivalent cssEquivalent = _propertyMetadata.GetCSSEquivalent(_propertyContainer);
                    if (cssEquivalent != null)
                    {
                        cssEquivalentExists = true;
                        StartAnimation(_propertyContainer, cssEquivalent, null, keyFrame.Value, GetKeyFrameDuration(keyFrame), keyFrame.INTERNAL_GetEasingFunction(), specificGroupName, _propDp,
                        OnKeyFrameCompleted(_parameters, isLastLoop, keyFrame.Value, _propertyContainer, _targetProperty, _animationID));
                    }
                }
                //todo: use GetCSSEquivalent instead (?)
                if (_propertyMetadata.GetCSSEquivalents != null)
                {
                    List<CSSEquivalent> cssEquivalents = _propertyMetadata.GetCSSEquivalents(_propertyContainer);
                    foreach (CSSEquivalent equivalent in cssEquivalents)
                    {
                        cssEquivalentExists = true;
                        StartAnimation(_propertyContainer, equivalent, null, keyFrame.Value, Duration, keyFrame.INTERNAL_GetEasingFunction(), specificGroupName, _propDp,
                        OnKeyFrameCompleted(_parameters, isLastLoop, keyFrame.Value, _propertyContainer, _targetProperty, _animationID));
                    }
                }

                if (!cssEquivalentExists)
                {
                    OnKeyFrameCompleted(_parameters, isLastLoop, keyFrame.Value, _propertyContainer, _targetProperty, _animationID)();
                }
            }
        }

        private Duration GetKeyFrameDuration(DoubleKeyFrame keyFrame)
        {
            return _keyFrameToDurationMap[keyFrame];
        }

        private Action OnKeyFrameCompleted(IterationParameters parameters, bool isLastLoop, object value, DependencyObject target, PropertyPath propertyPath, Guid callBackGuid)
        {
            return () =>
            {
                if (!this._isUnapplied)
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
            };
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
                AnimationHelpers.ApplyValue(_propertyContainer, _targetProperty, lastKeyFrame.Value);
            }
        }

        internal override void StopAnimation()
        {
            if (_isInitialized)
            {
                string specificGroupName = animationInstanceSpecificName.ToString();

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
                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"Velocity($0, ""stop"", $1);", cssEquivalent.DomElement, specificGroupName);
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
                            CSHTML5.Interop.ExecuteJavaScriptAsync(@"Velocity($0, ""stop"", $1);", equivalent.DomElement, specificGroupName);
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
            this.Completed -= ApplyLastKeyFrame;
            this.Completed += ApplyLastKeyFrame;
            InitializeKeyFramesSet();
            _propertyMetadata = _propDp.GetTypeMetaData(_propertyContainer.GetType());
        }

        internal override void RestoreDefaultCore()
        {
            _appliedKeyFramesCount = 0;
        }

        protected override Duration GetNaturalDurationCore()
        {
            return new Duration(LargestTimeSpanKeyTime);
        }


        static void StartAnimation(DependencyObject target, CSSEquivalent cssEquivalent, double? from, object to, Duration Duration, EasingFunctionBase easingFunction, string visualStateGroupName, DependencyProperty dependencyProperty, Action callbackForWhenfinished = null)
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
                        object cssValue = cssEquivalent.Value(target, to);

                        object newObj = CSHTML5.Interop.ExecuteJavaScriptAsync(@"new Object()");

                        if (AnimationHelpers.IsValueNull(from)) //todo: when using Bridge, I guess we would want to directly use "from == null" since it worked in the first place (I think).
                        {
                            foreach (string csspropertyName in cssEquivalent.Name)
                            {
                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0[$1] = $2;", newObj, csspropertyName, cssValue);
                            }
                        }
                        else
                        {
                            foreach (string csspropertyName in cssEquivalent.Name)
                            {
                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0[$1] = [$2, $3];", newObj, csspropertyName, cssValue, from);
                            }
                        }
                        AnimationHelpers.CallVelocity(cssEquivalent.DomElement, Duration, easingFunction, visualStateGroupName, callbackForWhenfinished, newObj);
                        target.DirtyVisualValue(dependencyProperty);
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
