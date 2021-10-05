using System.Windows;
using System;
using System.Windows.Controls;
using System.Collections.Generic;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    public sealed partial class PointAnimation : AnimationTimeline
    {
        public IEasingFunction EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        /// <summary>
        /// Identifies the EasingFunction dependency property.
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(PointAnimation), new PropertyMetadata(null));
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(nameof(From), typeof(Nullable<Point>), typeof(PointAnimation), new PropertyMetadata());
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(nameof(To), typeof(Nullable<Point>), typeof(PointAnimation), new PropertyMetadata());
        public static readonly DependencyProperty ByProperty = DependencyProperty.Register(nameof(By), typeof(Nullable<Point>), typeof(PointAnimation), new PropertyMetadata());
        public Nullable<Point> From
        {
            get
            {
                return (Nullable<Point>)this.GetValue(PointAnimation.FromProperty);
            }

            set
            {
                this.SetValue(PointAnimation.FromProperty, value);
            }
        }

        public Nullable<Point> To
        {
            get
            {
                return (Nullable<Point>)this.GetValue(PointAnimation.ToProperty);
            }

            set
            {
                this.SetValue(PointAnimation.ToProperty, value);
            }
        }

        public Nullable<Point> By
        {
            get
            {
                return (Nullable<Point>)this.GetValue(PointAnimation.ByProperty);
            }

            set
            {
                this.SetValue(PointAnimation.ByProperty, value);
            }
        }

        public PointAnimation()
        {
        }


        internal override void GetTargetInformation(IterationParameters parameters)
        {
            _parameters = parameters;
            DependencyObject target;
            PropertyPath propertyPath;

            GetTargetElementAndPropertyInfo(parameters.Target, out target, out propertyPath, parameters.IsTargetParentTheTarget);

            _propertyContainer = target;
            _targetProperty = propertyPath;
            _propDp = GetProperty(_propertyContainer, _targetProperty);
            _target = Storyboard.GetTarget(this);
            _targetName = Storyboard.GetTargetName(this);
        }

        // This guid is used to specifically target a particular call to the animation. It prevents the callback which should be called when velocity's animation end 
        // to be called when the callback is called from a previous call to the animation. This could happen when the animation was started quickly multiples times in a row. 
        private Guid _animationID;

        internal override void Apply(IterationParameters parameters, bool isLastLoop)
        {
            if (To != null)
            {
                // - Get the propertyMetadata from the property
                PropertyMetadata propertyMetadata = _propDp.GetTypeMetaData(_propertyContainer.GetType());

                //we make a specific name for this animation:
                string specificGroupName = parameters.VisualStateGroupName + animationInstanceSpecificName.ToString();

                _animationID = Guid.NewGuid();

                bool cssEquivalentExists = false;
                if (propertyMetadata.GetCSSEquivalent != null)
                {
                    CSSEquivalent cssEquivalent = propertyMetadata.GetCSSEquivalent(_propertyContainer);
                    if (cssEquivalent != null)
                    {
                        cssEquivalentExists = true;
                        StartAnimation(_propertyContainer, cssEquivalent, From, To, Duration, (EasingFunctionBase)EasingFunction, specificGroupName, _propDp,
                        OnAnimationCompleted(parameters, isLastLoop, To.Value, _propertyContainer, _targetProperty, _animationID));

                    }
                }

                if (propertyMetadata.GetCSSEquivalents != null)
                {
                    List<CSSEquivalent> cssEquivalents = propertyMetadata.GetCSSEquivalents(_propertyContainer);
                    foreach (CSSEquivalent equivalent in cssEquivalents)
                    {
                        cssEquivalentExists = true;
                        StartAnimation(_propertyContainer, equivalent, From, To, Duration, (EasingFunctionBase)EasingFunction, specificGroupName, _propDp,
                        OnAnimationCompleted(parameters, isLastLoop, To.Value, _propertyContainer, _targetProperty, _animationID));
                    }
                }

                if (!cssEquivalentExists)
                {
                    OnAnimationCompleted(parameters, isLastLoop, To.Value, _propertyContainer, _targetProperty, _animationID)();
                }
            }
        }

        private Action OnAnimationCompleted(IterationParameters parameters, bool isLastLoop, object value, DependencyObject target, PropertyPath propertyPath, Guid callBackGuid)
        {
            return () =>
            {
                if (!this._isUnapplied)
                {
                    if (isLastLoop && _animationID == callBackGuid)
                    {
                        AnimationHelpers.ApplyValue(target, propertyPath, value, parameters.IsVisualStateChange);
                    }
                    OnIterationCompleted(parameters);
                }
            };
        }

        static void StartAnimation(DependencyObject target, CSSEquivalent cssEquivalent, Point? from, object to, Duration Duration, EasingFunctionBase easingFunction, string visualStateGroupName, DependencyProperty dependencyProperty, Action callbackForWhenfinished = null)
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
 
        internal override void StopAnimation(string groupName)
        {
            if (_isInitialized)
            {
                //todo: find out why we put the test on target and put it back? (I removed it because id kept ScaleTransform from working properly)
                if (To != null)// && target is FrameworkElement) //todo: "To" can never be "null", fix this.
                {
                    // - Get the propertyMetadata from the property
                    PropertyMetadata propertyMetadata = _propDp.GetTypeMetaData(_propertyContainer.GetType());

                    //we make a specific name for this animation:
                    string specificGroupName = groupName + animationInstanceSpecificName.ToString();

                    // - Get the cssPropertyName from the PropertyMetadata
                    if (propertyMetadata.GetCSSEquivalent != null)
                    {
                        CSSEquivalent cssEquivalent = propertyMetadata.GetCSSEquivalent(_propertyContainer);
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
                    if (propertyMetadata.GetCSSEquivalents != null)
                    {
                        List<CSSEquivalent> cssEquivalents = propertyMetadata.GetCSSEquivalents(_propertyContainer);
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
        }

    }
}
