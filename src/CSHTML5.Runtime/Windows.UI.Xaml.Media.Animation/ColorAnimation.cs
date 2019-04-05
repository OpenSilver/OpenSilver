
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
    public sealed class ColorAnimation : Timeline
    {
        ///// <summary>
        ///// Initializes a new instance of the ColorAnimation class.
        ///// </summary>
        //public ColorAnimation();

        ///// <summary>
        ///// Gets or sets the total amount by which the animation changes its starting
        ///// value.
        ///// </summary>
        //public Color? By
        //{
        //    get { return (Color?)GetValue(ByProperty); }
        //    set { SetValue(ByProperty, value); }
        //}
        ///// <summary>
        ///// Identifies the By dependency property.
        ///// </summary>
        //public static readonly DependencyProperty ByProperty =
        //    DependencyProperty.Register("By", typeof(Color?), typeof(ColorAnimation), new PropertyMetadata(null));

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

        ///// <summary>
        ///// Gets or sets a value that declares whether animated properties that are considered
        ///// dependent animations should be permitted to use this animation declaration.
        ///// </summary>
        //public bool EnableDependentAnimation
        //{
        //    get { return (bool)GetValue(EnableDependentAnimationProperty); }
        //    set { SetValue(EnableDependentAnimationProperty, value); }
        //}
        ///// <summary>
        ///// Identifies the EnableDependentAnimation dependency property.
        ///// </summary>
        //public static readonly DependencyProperty EnableDependentAnimationProperty =
        //    DependencyProperty.Register("EnableDependentAnimation", typeof(bool), typeof(ColorAnimation), new PropertyMetadata(false)); // todo: check if the default value should be true

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

        internal override void IterateOnce(IterationParameters parameters, bool isLastLoop)
        {
            base.IterateOnce(parameters, isLastLoop);
            Apply(parameters, isLastLoop);
        }

        internal override void Apply(IterationParameters parameters, bool isLastLoop)
        {
            DependencyObject target;
            PropertyPath propertyPath;
            DependencyObject targetBeforePath;
            GetPropertyPathAndTargetBeforePath(parameters.Target, out targetBeforePath, out propertyPath, parameters.IsTargetParentTheTarget);
            DependencyObject parentElement = targetBeforePath; //this will be the parent of the clonable element (if any).
            //we clone what needs to be cloned (ICloneOnAnimation: contient Clone() ) --> 
            // foreach sur le résultat de la méthode autogénérée avec les yield return et on clone le 1er ICloneOnAnimation
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
                        if (index.HasValue)
                        {
                            if (Interop.IsRunningInTheSimulator)
                            {
                                parentElement.GetType().GetProperty("Item").SetValue(parentElement, clone, new object[] { index.Value });
                            }
                            else
                            {
#if BRIDGE
                                parentElement.GetType().GetProperty("Item").SetValue(parentElement, clone, new object[]{ index.Value });
#else
                                //JSIL does not support SetValue(object, object, object[])
#endif
                            }
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

            //we do the following normally

            Type lastElementType = target.GetType();
            PropertyInfo propertyInfo = lastElementType.GetProperty(propertyPath.INTERNAL_DependencyPropertyName);


            //todo: find out why we put the test on target and put it back? (I removed it because it kept ScaleTransform from working properly)
            if (To != null)// && target is FrameworkElement)
            {
                Type propertyType = propertyInfo.PropertyType;
                var castedValue = DynamicCast(To, propertyType); //Note: we put this line here because the Xaml could use a Color gotten from a StaticResource (which was therefore not converted to a SolidColorbrush by the compiler in the .g.cs file) and led to a wrong type set in a property (Color value in a property of type Brush).


                Type dependencyPropertyContainerType = propertyInfo.DeclaringType;
                FieldInfo dependencyPropertyField = dependencyPropertyContainerType.GetField(propertyPath.INTERNAL_DependencyPropertyName + "Property");
                // - Get the DependencyProperty
#if MIGRATION
                DependencyProperty dp = (global::System.Windows.DependencyProperty)dependencyPropertyField.GetValue(null);
#else
                DependencyProperty dp = (global::Windows.UI.Xaml.DependencyProperty)dependencyPropertyField.GetValue(null);
#endif
                // - Get the propertyMetadata from the property
                PropertyMetadata propertyMetadata = dp.GetTypeMetaData(target.GetType());
                // - Get the cssPropertyName from the PropertyMetadata


                //we make a specific name for this animation:
                string specificGroupName = parameters.VisualStateGroupName + animationInstanceSpecificName.ToString();


                bool cssEquivalentExists = false;
                if (propertyMetadata.GetCSSEquivalent != null)
                {
                    CSSEquivalent cssEquivalent = propertyMetadata.GetCSSEquivalent(target);
                    if (cssEquivalent != null)
                    {
                        cssEquivalentExists = true;
                        TryStartAnimation(target, cssEquivalent, From, To, Duration, EasingFunction, specificGroupName, () =>
                            {
                                if (isLastLoop)
                                {
                                    if (parameters.IsVisualStateChange) //if we change the visual state, we set the VisualStateValue in the storage
                                    {
                                        propertyPath.INTERNAL_PropertySetVisualState(target, castedValue);
                                    }
                                    else //otherwise (if we used Storyboard.Begin()), we set the Local value in the storage
                                    {
                                        propertyPath.INTERNAL_PropertySetLocalValue(target, castedValue);
                                    }
                                }
                                OnIterationCompleted(parameters);
                            });
                    }
                }
                if (propertyMetadata.GetCSSEquivalents != null)
                {
                    List<CSSEquivalent> cssEquivalents = propertyMetadata.GetCSSEquivalents(target);
                    bool isFirst = true;
                    foreach (CSSEquivalent equivalent in cssEquivalents)
                    {
                        cssEquivalentExists = true;
                        if (equivalent.CallbackMethod == null)
                        {
                            if (isFirst)
                            {
                                bool updateIsFirst = TryStartAnimation(target, equivalent, From, To, Duration, EasingFunction, specificGroupName, () =>
                                    {
                                        if (isLastLoop)
                                        {
                                            if (parameters.IsVisualStateChange) //if we change the visual state, we set the VisualStateValue in the storage
                                            {
                                                propertyPath.INTERNAL_PropertySetVisualState(target, castedValue);
                                            }
                                            else //otherwise (if we used Storyboard.Begin()), we set the Local value in the storage
                                            {
                                                propertyPath.INTERNAL_PropertySetLocalValue(target, castedValue);
                                            }
                                        }
                                        OnIterationCompleted(parameters);
                                    });
                                if (updateIsFirst)
                                {
                                    isFirst = false;
                                }
                            }
                            else
                            {
                                TryStartAnimation(target, equivalent, From, To, Duration, EasingFunction, specificGroupName, () =>
                                {
                                    if (isLastLoop)
                                    {
                                        if (parameters.IsVisualStateChange) //if we change the visual state, we set the VisualStateValue in the storage
                                        {
                                            propertyPath.INTERNAL_PropertySetVisualState(target, castedValue);
                                        }
                                        else //otherwise (if we used Storyboard.Begin()), we set the Local value in the storage
                                        {
                                            propertyPath.INTERNAL_PropertySetLocalValue(target, castedValue);
                                        }
                                    }
                                });
                            }
                        }
                        else
                        {
                            if (isLastLoop)
                            {
                                if (parameters.IsVisualStateChange)
                                {
                                    propertyPath.INTERNAL_PropertySetVisualState(target, castedValue);
                                }
                                else
                                {
                                    propertyPath.INTERNAL_PropertySetLocalValue(target, castedValue);
                                }
                            }
                            OnIterationCompleted(parameters);
                        }
                    }
                }
                if (!cssEquivalentExists)
                {
                    if (parameters.IsVisualStateChange)
                    {
                        propertyPath.INTERNAL_PropertySetVisualState(target, castedValue);
                    }
                    else
                    {
                        propertyPath.INTERNAL_PropertySetLocalValue(target, castedValue);
                    }
                    OnIterationCompleted(parameters);
                }
            }
            else
            {
                if (parameters.IsVisualStateChange)
                {
                    propertyPath.INTERNAL_PropertySetVisualState(target, To);
                }
                else
                {
                    propertyPath.INTERNAL_PropertySetLocalValue(target, To);
                }
                OnIterationCompleted(parameters);
            }
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
                                        //return ((Color)value).INTERNAL_ToHtmlStringForVelocity();
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

                        if (from == null)
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
                                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0[$1] = $2;", newObj, csspropertyName, cssValue);
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
                                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0[$1] = $2;", newObj, csspropertyName, cssValueAsDictionary[csspropertyName]);
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
                                    CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0[$1] = [$2, $3];", newObj, csspropertyName, currentCssValue, currentFromCssValue);
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

        internal override void Stop(FrameworkElement frameworkElement, string groupName, bool revertToFormerValue = false)
        {
            base.Stop(frameworkElement, groupName, revertToFormerValue);

            DependencyObject target;
            PropertyPath propertyPath;
            GetTargetElementAndPropertyInfo(frameworkElement, out target, out propertyPath);
            //DependencyObject lastElementBeforeProperty = propertyPath.INTERNAL_AccessPropertyContainer(target);
            Type lastElementType = target.GetType();
            PropertyInfo propertyInfo = lastElementType.GetProperty(propertyPath.INTERNAL_DependencyPropertyName);

            Color? valuetoSet = To;
            if (revertToFormerValue)
            {
                valuetoSet = (Color?)propertyInfo.GetValue(target);
                //propertyInfo.SetValue(target, formerValue);
            }

            //todo: find out why we put the test on target and put it back? (I removed it because it kept ScaleTransform from working properly)
            if (valuetoSet != null)// && target is FrameworkElement)
            {
                Type propertyType = propertyInfo.PropertyType;
                var castedValue = DynamicCast(valuetoSet, propertyType); //Note: we put this line here because the Xaml could use a Color gotten from a StaticResource (which was therefore not converted to a SolidColorbrush by the compiler in the .g.cs file) and led to a wrong type set in a property (Color value in a property of type Brush).

                Type dependencyPropertyContainerType = propertyInfo.DeclaringType;
                FieldInfo dependencyPropertyField = dependencyPropertyContainerType.GetField(propertyPath.INTERNAL_DependencyPropertyName + "Property");
                // - Get the DependencyProperty
#if MIGRATION
                DependencyProperty dp = (global::System.Windows.DependencyProperty)dependencyPropertyField.GetValue(null);
#else
                DependencyProperty dp = (global::Windows.UI.Xaml.DependencyProperty)dependencyPropertyField.GetValue(null);
#endif
                // - Get the propertyMetadata from the property
                PropertyMetadata propertyMetadata = dp.GetTypeMetaData(target.GetType());
                // - Get the cssPropertyName from the PropertyMetadata

                //we make a specific name for this animation:
                string specificGroupName = groupName + animationInstanceSpecificName.ToString();

                bool cssEquivalentExists = false;
                if (propertyMetadata.GetCSSEquivalent != null)
                {
                    CSSEquivalent cssEquivalent = propertyMetadata.GetCSSEquivalent(target);
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
                            cssEquivalentExists = true;
                            CSHTML5.Interop.ExecuteJavaScriptAsync(@"
        Velocity($0, ""stop"", $1);", cssEquivalent.DomElement, specificGroupName);
                        }
                    }
                }
                //todo: use GetCSSEquivalent instead (?)
                if (propertyMetadata.GetCSSEquivalents != null)
                {
                    List<CSSEquivalent> cssEquivalents = propertyMetadata.GetCSSEquivalents(target);
                    foreach (CSSEquivalent equivalent in cssEquivalents)
                    {
                        if (equivalent.DomElement != null && equivalent.CallbackMethod == null)
                        {
                            cssEquivalentExists = true;
                            CSHTML5.Interop.ExecuteJavaScriptAsync(@"
Velocity($0, ""stop"", $1);", equivalent.DomElement, specificGroupName);
                            if (revertToFormerValue)
                            {
                                propertyPath.INTERNAL_PropertySetVisualState(target, castedValue);
                            }
                        }
                    }
                }
                if (!cssEquivalentExists)
                {
                    propertyPath.INTERNAL_PropertySetVisualState(target, castedValue);
                }
            }
            else
            {
                propertyPath.INTERNAL_PropertySetVisualState(target, valuetoSet);
            }
        }
    }
}
