

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
            DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(ColorAnimation), new PropertyMetadata(null));

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
            DependencyProperty.Register("From", typeof(Color?), typeof(ColorAnimation), new PropertyMetadata(null));

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
            DependencyProperty.Register("To", typeof(Color?), typeof(ColorAnimation), new PropertyMetadata(null));

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
                string specificGroupName = animationInstanceSpecificName.ToString();

                bool cssEquivalentExists = false;
                if (propertyMetadata.GetCSSEquivalent != null)
                {
                    CSSEquivalent cssEquivalent = propertyMetadata.GetCSSEquivalent(_propertyContainer);
                    if (cssEquivalent != null)
                    {
                        cssEquivalentExists = true;
                        TryStartAnimation(_propertyContainer, cssEquivalent, From, To, Duration, EasingFunction, specificGroupName, _propDp,
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
                                bool updateIsFirst = TryStartAnimation(_propertyContainer, equivalent, From, To, Duration, EasingFunction, specificGroupName, _propDp,
                                                                       OnAnimationCompleted(parameters, isLastLoop, castedValue, _propertyContainer, _targetProperty, _animationID));
                                if (updateIsFirst)
                                {
                                    isFirst = false;
                                }
                            }
                            else
                            {
                                TryStartAnimation(_propertyContainer, equivalent, From, To, Duration, EasingFunction, specificGroupName, _propDp,
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
                if (!this._isUnapplied)
                {
                    if (isLastLoop && _animationID == callBackGuid)
                    {
                        AnimationHelpers.ApplyValue(target, propertyPath, value);
                    }
                    OnIterationCompleted(parameters);
                }
            };
        }

        static bool TryStartAnimation(DependencyObject target, CSSEquivalent cssEquivalent, Color? from, object to, Duration Duration, EasingFunctionBase easingFunction, string visualStateGroupName, DependencyProperty dependencyProperty, Action callbackForWhenfinished = null)
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
                        string fromToValues;
                        if (!from.HasValue)
                        {
                            if (cssValue is Dictionary<string, object> cssValueAsDictionary)
                            {
                                fromToValues = "{" +
                                    string.Join(",", cssEquivalent.Name
                                        .Where(name => name != "background")
                                        .Select(name => $"\"{name}\":{INTERNAL_InteropImplementation.GetVariableStringForJS(cssValueAsDictionary[name])}")) + "}";
                            }
                            else
                            {
                                string sCssValue = INTERNAL_InteropImplementation.GetVariableStringForJS(cssValue);
                                fromToValues = "{" +
                                    string.Join(",", cssEquivalent.Name
                                        .Where(name => name != "background")
                                        .Select(name => $"\"{name}\":{sCssValue}")) + "}";
                            }
                        }
                        else
                        {
                            object fromCssValue = cssEquivalent.Value(target, from);
                            fromToValues = "{" +
                                string.Join(",", cssEquivalent.Name
                                    .Where(name => name != "background")
                                    .Select(name =>
                                    {
                                        object currentCssValue = cssValue is Dictionary<string, object> d1 ? d1[name] : cssValue;
                                        object currentFromCssValue = fromCssValue is Dictionary<string, object> d2 ? d2[name] : fromCssValue;
                                        string sCurrentCssValue = INTERNAL_InteropImplementation.GetVariableStringForJS(currentCssValue);
                                        string sCurrentFromCssValue = INTERNAL_InteropImplementation.GetVariableStringForJS(currentFromCssValue);
                                        return $"\"{name}\":[{sCurrentCssValue}, {sCurrentFromCssValue}]";
                                    })) + "}";
                        }

                        AnimationHelpers.CallVelocity(
                            cssEquivalent.DomElement,
                            Duration,
                            easingFunction,
                            visualStateGroupName,
                            callbackForWhenfinished,
                            fromToValues);

                        target.DirtyVisualValue(dependencyProperty);
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

        internal override void StopAnimation()
        {
            if (_isInitialized)
            {
                //todo: find out why we put the test on target and put it back? (I removed it because it kept ScaleTransform from working properly)
                if (To != null)// && target is FrameworkElement)
                {
                    DependencyProperty dp = CSHTML5.Internal.INTERNAL_TypeToStringsToDependencyProperties
                        .GetPropertyInTypeOrItsBaseTypes(_propertyContainer.GetType(), _targetProperty.SVI[_targetProperty.SVI.Length - 1].propertyName);

                    // - Get the propertyMetadata from the property
                    PropertyMetadata propertyMetadata = dp.GetTypeMetaData(_propertyContainer.GetType());
                    // - Get the cssPropertyName from the PropertyMetadata

                    //we make a specific name for this animation:
                    string specificGroupName = animationInstanceSpecificName.ToString();

                    bool cssEquivalentExists = false;
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
                                    cssEquivalentExists = true;
                                    string sDomElement = INTERNAL_InteropImplementation.GetVariableStringForJS(cssEquivalent.DomElement);
                                    CSHTML5.Interop.ExecuteJavaScriptFastAsync($@"Velocity({sDomElement}, ""stop"", ""{specificGroupName}"");");
                                }
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
                                string sDomElement = INTERNAL_InteropImplementation.GetVariableStringForJS(equivalent.DomElement);
                                CSHTML5.Interop.ExecuteJavaScriptFastAsync($@"Velocity({sDomElement}, ""stop"", ""{specificGroupName}"");");
                            }
                        }
                    }
                }
            }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty ByProperty = DependencyProperty.Register("By", typeof(Color?), typeof(ColorAnimation), null);
        [OpenSilver.NotImplemented]
        public Color? By
        {
            get { return (Color?)this.GetValue(ByProperty); }
            set { this.SetValue(ByProperty, value); }
        }
    }
}
