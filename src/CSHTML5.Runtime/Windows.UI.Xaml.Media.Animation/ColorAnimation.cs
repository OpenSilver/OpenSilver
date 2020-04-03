

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
using CSHTML5;
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
    /// Animates the value of a Color property between two target values using linear
    /// interpolation over a specified Duration.
    /// </summary>
    public sealed partial class ColorAnimation : AnimationTimeline
    {
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
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(ColorAnimation), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Gets or sets the animation's starting value.
        /// </summary>
        public Color? From
        {
            get { return (Color?)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }
        /// <summary>
        /// Identifies the From dependency property.
        /// </summary>
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(Color?), typeof(ColorAnimation), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Gets or sets the animation's ending value.
        /// </summary>
        public Color? To
        {
            get { return (Color?)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }
        /// <summary>
        /// Identifies the To dependency property.
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(Color?), typeof(ColorAnimation), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

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
                var castedValue = DynamicCast(To, _propDp.PropertyType); //Note: we put this line here because the Xaml could use a Color gotten from a StaticResource (which was therefore not converted to a SolidColorbrush by the compiler in the .g.cs file) and led to a wrong type set in a property (Color value in a property of type Brush).

                // - Get the propertyMetadata from the property
                PropertyMetadata propertyMetadata = _propDp.GetTypeMetaData(_propertyContainer.GetType());
                // - Get the cssPropertyName from the PropertyMetadata

                //we make a specific name for this animation:
                string specificGroupName = parameters.VisualStateGroupName + animationInstanceSpecificName.ToString();

                bool cssEquivalentExists = false;
                if (propertyMetadata.GetCSSEquivalent != null)
                {
                    CSSEquivalent cssEquivalent = propertyMetadata.GetCSSEquivalent(_propertyContainer);
                    if (cssEquivalent != null)
                    {
                        cssEquivalentExists = true;
                        TryStartAnimation(_propertyContainer, cssEquivalent, From, To, Duration, EasingFunction, specificGroupName,
                                          OnAnimationCompleted(parameters, isLastLoop, castedValue, _propertyContainer, _targetProperty, _animationID));
                    }
                }
                if (propertyMetadata.GetCSSEquivalents != null)
                {
                    List<CSSEquivalent> cssEquivalents = propertyMetadata.GetCSSEquivalents(_propertyContainer);
                    bool isFirst = true;
                    foreach (CSSEquivalent equivalent in cssEquivalents)
                    {
                        cssEquivalentExists = true;
                        if (equivalent.CallbackMethod == null)
                        {
                            if (isFirst)
                            {
                                bool updateIsFirst = TryStartAnimation(_propertyContainer, equivalent, From, To, Duration, EasingFunction, specificGroupName,
                                                                       OnAnimationCompleted(parameters, isLastLoop, castedValue, _propertyContainer, _targetProperty, _animationID));
                                if (updateIsFirst)
                                {
                                    isFirst = false;
                                }
                            }
                            else
                            {
                                TryStartAnimation(_propertyContainer, equivalent, From, To, Duration, EasingFunction, specificGroupName,
                                                  OnAnimationCompleted(parameters, isLastLoop, castedValue, _propertyContainer, _targetProperty, _animationID));
                            }
                        }
                        else
                        {
                            OnAnimationCompleted(parameters, isLastLoop, castedValue, _propertyContainer, _targetProperty, _animationID)();
                        }
                    }
                }
                if (!cssEquivalentExists)
                {
                    OnAnimationCompleted(parameters, isLastLoop, castedValue, _propertyContainer, _targetProperty, _animationID)();
                }
            }
            else
            {
                OnAnimationCompleted(parameters, isLastLoop, To, _propertyContainer, _targetProperty, _animationID)();
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

        static bool TryStartAnimation(DependencyObject target, CSSEquivalent cssEquivalent, Color? from, object to, Duration Duration, EasingFunctionBase easingFunction, string visualStateGroupName, Action callbackForWhenfinished = null)
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
                            cssEquivalent.Value = (finalInstance, value) =>
                                {
                                    if (value == null)
                                    {
                                        return "";
                                    }
                                    else
                                    {
                                        Dictionary<string, object> valuesDict = new Dictionary<string, object>();
                                        foreach (string name in cssEquivalent.Name)
                                        {
                                            if (!name.EndsWith("Alpha"))
                                            {
                                                valuesDict.Add(name, ((Color)value).INTERNAL_ToHtmlStringForVelocity());
                                            }
                                            else
                                            {
                                                valuesDict.Add(name, ((double)((Color)value).A) / 255);
                                            }
                                        }
                                        return valuesDict;
                                    }
                                    //return value ?? "";
                                }; // Default value
                        }
                        object cssValue = cssEquivalent.Value(target, to);

                        //INTERNAL_HtmlDomManager.SetDomElementStyleProperty(cssEquivalent.DomElement, cssEquivalent.Name, cssValue);


                        object newObj = CSHTML5.Interop.ExecuteJavaScriptAsync(@"new Object()");

                        if (AnimationHelpers.IsValueNull(from)) //todo: when using Bridge, I guess we would want to directly use "from == null" since it worked in the first place (I think).
                        {
                            if (!(cssValue is Dictionary<string, object>))
                            {
                                foreach (string csspropertyName in cssEquivalent.Name)
                                {
                                    //todo: check the note below once the clone will work properly (a value set through velocity is not set in c#, which makes the clone take the former value).
                                    //Note: the test below is to avoid setting Background because Velocity cannot handle it,
                                    //      which makes the element go transparent (no color) before then changing color with backgroundColor.
                                    //      Therefore, we no longer go in the animation from the previous color to the new one but from no color to the new one
                                    if (csspropertyName != "background") //todo: when we will be able to use velocity for linearGradientBrush, we will need another solution here.
                                    {
                                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0[$1] = $2;", newObj, csspropertyName, cssValue);
                                    }
                                }
                            }
                            else
                            {
                                Dictionary<string, object> cssValueAsDictionary = (Dictionary<string, object>)cssValue;
                                foreach (string csspropertyName in cssEquivalent.Name)
                                {
                                    //todo: check the note below once the clone will work properly (a value set through velocity is not set in c#, which makes the clone take the former value).
                                    //Note: the test below is to avoid setting Background because Velocity cannot handle it,
                                    //      which makes the element go transparent (no color) before then changing color with backgroundColor.
                                    //      Therefore, we no longer go in the animation from the previous color to the new one but from no color to the new one
                                    if (csspropertyName != "background") //todo: when we will be able to use velocity for linearGradientBrush, we will need another solution here.
                                    {
                                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0[$1] = $2;", newObj, csspropertyName, cssValueAsDictionary[csspropertyName]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            object fromCssValue = cssEquivalent.Value(target, from);
                            foreach (string csspropertyName in cssEquivalent.Name)
                            {
                                //todo: check the note below once the clone will work properly (a value set through velocity is not set in c#, which makes the clone take the former value).
                                //Note: the test below is to avoid setting Background because Velocity cannot handle it,
                                //      which makes the element go transparent (no color) before then changing color with backgroundColor.
                                //      Therefore, we no longer go in the animation from the previous color to the new one but from no color to the new one
                                if (csspropertyName != "background") //todo: when we will be able to use velocity for linearGradientBrush, we will need another solution here.
                                {
                                    object currentCssValue = cssValue;
                                    if (cssValue is Dictionary<string, object>)
                                    {
                                        currentCssValue = ((Dictionary<string, object>)cssValue)[csspropertyName];
                                    }
                                    object currentFromCssValue = fromCssValue;
                                    if (fromCssValue is Dictionary<string, object>)
                                    {
                                        currentFromCssValue = ((Dictionary<string, object>)fromCssValue)[csspropertyName];
                                    }
                                    CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0[$1] = [$2, $3];", newObj, csspropertyName, currentCssValue, currentFromCssValue);
                                }
                            }
                        }

                        AnimationHelpers.CallVelocity(cssEquivalent.DomElement, Duration, easingFunction, visualStateGroupName, callbackForWhenfinished, newObj);

                        return true;
                    }
                }
                return false;
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
                Type lastElementType = _propertyContainer.GetType();
                PropertyInfo propertyInfo = lastElementType.GetProperty(_targetProperty.INTERNAL_DependencyPropertyName);

                //todo: find out why we put the test on target and put it back? (I removed it because it kept ScaleTransform from working properly)
                if (To != null)// && target is FrameworkElement)
                {
                    Type dependencyPropertyContainerType = propertyInfo.DeclaringType;
                    FieldInfo dependencyPropertyField = dependencyPropertyContainerType.GetField(_targetProperty.INTERNAL_DependencyPropertyName + "Property");
                    // - Get the DependencyProperty
#if MIGRATION
                    DependencyProperty dp = (global::System.Windows.DependencyProperty)dependencyPropertyField.GetValue(null);
#else
                DependencyProperty dp = (global::Windows.UI.Xaml.DependencyProperty)dependencyPropertyField.GetValue(null);
#endif
                    // - Get the propertyMetadata from the property
                    PropertyMetadata propertyMetadata = dp.GetTypeMetaData(_propertyContainer.GetType());
                    // - Get the cssPropertyName from the PropertyMetadata

                    //we make a specific name for this animation:
                    string specificGroupName = groupName + animationInstanceSpecificName.ToString();

                    bool cssEquivalentExists = false;
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
                                cssEquivalentExists = true;
                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"Velocity($0, ""stop"", $1);", cssEquivalent.DomElement, specificGroupName);
                            }
                        }
                    }
                    //todo: use GetCSSEquivalent instead (?)
                    if (propertyMetadata.GetCSSEquivalents != null)
                    {
                        List<CSSEquivalent> cssEquivalents = propertyMetadata.GetCSSEquivalents(_propertyContainer);
                        foreach (CSSEquivalent equivalent in cssEquivalents)
                        {
                            if (equivalent.DomElement != null && equivalent.CallbackMethod == null)
                            {
                                cssEquivalentExists = true;
                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"Velocity($0, ""stop"", $1);", equivalent.DomElement, specificGroupName);
                            }
                        }
                    }
                }
            }
        }

#if WORKINPROGRESS
        public static readonly DependencyProperty ByProperty = DependencyProperty.Register("By", typeof(Color?), typeof(ColorAnimation), null);
        public Color? By
        {
            get { return (Color?)this.GetValue(ByProperty); }
            set { this.SetValue(ByProperty, value); }
        }
#endif
    }
}
