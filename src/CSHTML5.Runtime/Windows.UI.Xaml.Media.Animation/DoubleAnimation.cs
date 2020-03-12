

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
    /// Animates the value of a Double property between two target values using linear
    /// interpolation over a specified Duration.
    /// </summary>
    public partial class DoubleAnimation : AnimationTimeline
    {
#if WORKINPROGRESS
        public IEasingFunction EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        /// <summary>
        /// Identifies the EasingFunction dependency property.
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(DoubleAnimation), new PropertyMetadata(null)
        { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
#else
        /// <summary>
        /// Gets or sets the easing function applied to this animation.
        /// </summary>
        public EasingFunctionBase EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        /// <summary>
        /// Identifies the EasingFunction dependency property.
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(DoubleAnimation), new PropertyMetadata(null)
        { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

#endif

        /// <summary>
        /// Gets or sets the animation's starting value.
        /// </summary>
        public double? From
        {
            get { return (double?)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }
        /// <summary>
        /// Identifies the From dependency property.
        /// </summary>
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(double?), typeof(DoubleAnimation), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });



        // Returns:
        //     The ending value of the animation. The default is null. If you are programming
        //     using C# or Visual Basic, the type of this property is projected as double?
        //     (a nullable double).
        /// <summary>
        /// Gets or sets the animation's ending value.
        /// </summary>
        public double? To
        {
            get { return (double?)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }
        /// <summary>
        /// Identifies the To dependency property.
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(double?), typeof(DoubleAnimation), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

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
                        StartAnimation(_propertyContainer, cssEquivalent, From, To, Duration, (EasingFunctionBase)EasingFunction, specificGroupName,
                        OnAnimationCompleted(parameters, isLastLoop, To.Value, _propertyContainer, _targetProperty, _animationID));

                    }
                }
                //todo: use GetCSSEquivalent instead (?)
                if (propertyMetadata.GetCSSEquivalents != null)
                {
                    List<CSSEquivalent> cssEquivalents = propertyMetadata.GetCSSEquivalents(_propertyContainer);
                    foreach (CSSEquivalent equivalent in cssEquivalents)
                    {
                        cssEquivalentExists = true;
                        StartAnimation(_propertyContainer, equivalent, From, To, Duration, (EasingFunctionBase)EasingFunction, specificGroupName,
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
                if (isLastLoop && _animationID == callBackGuid)
                {
                    AnimationHelpers.ApplyValue(target, propertyPath, value, parameters.IsVisualStateChange);
                }
                OnIterationCompleted(parameters);
            };
        }

        static void StartAnimation(DependencyObject target, CSSEquivalent cssEquivalent, double? from, object to, Duration Duration, EasingFunctionBase easingFunction, string visualStateGroupName, Action callbackForWhenfinished = null)
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

#if WORKINPROGRESS
        public static readonly DependencyProperty ByProperty = DependencyProperty.Register("By", typeof(double?), typeof(DoubleAnimation), null);
        public double? By
        {
            get { return (double?)this.GetValue(ByProperty); }
            set { this.SetValue(ByProperty, value); }
        }
#endif
    }
}