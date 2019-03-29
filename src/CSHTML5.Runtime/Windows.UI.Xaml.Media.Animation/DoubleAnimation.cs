
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


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
    public class DoubleAnimation : Timeline
    {
        ///// <summary>
        ///// Initializes a new instance of the DoubleAnimation class.
        ///// </summary>
        //public DoubleAnimation();

        //// Returns:
        ////     The total amount by which the animation changes its starting value. The default
        ////     is null. If you are programming using C# or Visual Basic, the type of this
        ////     property is projected as double? (a nullable double).
        ///// <summary>
        ///// Gets or sets the total amount by which the animation changes its starting
        ///// value.
        ///// </summary>
        //public double? By { get; set; }
        ///// <summary>
        ///// Identifies the By dependency property.
        ///// </summary>
        //public static DependencyProperty ByProperty { get; }

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
            DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(DoubleAnimation), new PropertyMetadata(null));




        //// Returns:
        ////     True if the animation can be used for a dependent animation case. False if
        ////     the animation cannot be used for a dependent animation case.
        ///// <summary>
        ///// Gets or sets a value that declares whether animated properties that are considered
        ///// dependent animations should be permitted to use this animation declaration.
        ///// </summary>
        //public bool EnableDependentAnimation { get; set; }
        ///// <summary>
        ///// Identifies the EnableDependentAnimation dependency property.
        ///// </summary>
        //public static DependencyProperty EnableDependentAnimationProperty { get; }

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
            DependencyProperty.Register("From", typeof(double?), typeof(DoubleAnimation), new PropertyMetadata(null));



        // Returns:
        //     The ending value of the animation. The default is null. If you are programming
        //     using C# or Visual Basic, the type of this property is projected as double?
        //     (a nullable double).
        /// <summary>
        /// Gets or sets the animation's ending value.
        /// </summary>
        public double To //todo: this is supposed to be double? (nullable double)
        {
            get { return (double)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }
        /// <summary>
        /// Identifies the To dependency property.
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(double), typeof(DoubleAnimation), new PropertyMetadata(null));

        internal override void IterateOnce(IterationParameters parameters, bool isLastLoop)
        {
            base.IterateOnce(parameters, isLastLoop);
            Apply(parameters, isLastLoop);
        }

        internal override void Apply(IterationParameters parameters, bool isLastLoop)
        {
            DependencyObject target;
            PropertyPath propertyPath;
            if (parameters.Target != null)
            {
                GetTargetElementAndPropertyInfo(parameters.Target, out target, out propertyPath, parameters.IsTargetParentTheTarget);
                DependencyProperty dp = GetProperty(target, propertyPath);

                //todo: find out why we put the test on target and put it back? (I removed it because id kept ScaleTransform from working properly)
                if (To != null)// && target is FrameworkElement)
                {
                    // - Get the propertyMetadata from the property
                    PropertyMetadata propertyMetadata = dp.GetTypeMetaData(target.GetType());

                    //we make a specific name for this animation:
                    string specificGroupName = parameters.VisualStateGroupName + animationInstanceSpecificName.ToString();

                    bool cssEquivalentExists = false;
                    if (propertyMetadata.GetCSSEquivalent != null)
                    {
                        CSSEquivalent cssEquivalent = propertyMetadata.GetCSSEquivalent(target);
                        if (cssEquivalent != null)
                        {
                            cssEquivalentExists = true;
                            StartAnimation(target, cssEquivalent, From, To, Duration, EasingFunction, specificGroupName, () =>
                            {
                                if (isLastLoop)
                                {
                                    if (parameters.IsVisualStateChange) //if we change the visual state, we set the VisualStateValue in the storage
                                    {
                                        propertyPath.INTERNAL_PropertySetVisualState(target, To);
                                    }
                                    else //otherwise (if we used Storyboard.Begin()), we set the Local value in the storage
                                    {
                                        propertyPath.INTERNAL_PropertySetLocalValue(target, To);
                                    }
                                }
                                OnIterationCompleted(parameters);
                                //INTERNAL_RaiseCompletedEvent(applyCallGuid);
                            });
                        }
                    }
                    //todo: use GetCSSEquivalent instead (?)
                    if (propertyMetadata.GetCSSEquivalents != null)
                    {
                        List<CSSEquivalent> cssEquivalents = propertyMetadata.GetCSSEquivalents(target);
                        foreach (CSSEquivalent equivalent in cssEquivalents)
                        {
                            cssEquivalentExists = true;
                            StartAnimation(target, equivalent, From, To, Duration, EasingFunction, specificGroupName, () =>
                            {
                                if (isLastLoop)
                                {
                                    if (parameters.IsVisualStateChange) //if we change the visual state, we set the VisualStateValue in the storage
                                    {
                                        propertyPath.INTERNAL_PropertySetVisualState(target, To);
                                    }
                                    else //otherwise (if we used Storyboard.Begin()), we set the Local value in the storage
                                    {
                                        propertyPath.INTERNAL_PropertySetLocalValue(target, To);
                                    }
                                }
                                OnIterationCompleted(parameters);
                                //INTERNAL_RaiseCompletedEvent(applyCallGuid);
                            });
                        }
                    }

                    if (!cssEquivalentExists)
                    {
                        if (isLastLoop)
                        {
                            if (parameters.IsVisualStateChange)
                            {
                                propertyPath.INTERNAL_PropertySetVisualState(target, To);
                            }
                            else
                            {
                                propertyPath.INTERNAL_PropertySetLocalValue(target, To);
                            }
                        }
                        OnIterationCompleted(parameters);
                        //INTERNAL_RaiseCompletedEvent(applyCallGuid);
                    }
                }
                else
                {
                    if (isLastLoop)
                    {
                        if (parameters.IsVisualStateChange)
                        {
                            propertyPath.INTERNAL_PropertySetVisualState(target, To);
                        }
                        else
                        {
                            propertyPath.INTERNAL_PropertySetLocalValue(target, To);
                        }
                    }
                    OnIterationCompleted(parameters);
                    //INTERNAL_RaiseCompletedEvent(applyCallGuid);
                }
            }
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

                        if (from == null)
                        {
                            foreach (string csspropertyName in cssEquivalent.Name)
                            {
                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0[$1] = $2;", newObj, csspropertyName, cssValue);
                            }
                        }
                        else
                        {
                            foreach (string csspropertyName in cssEquivalent.Name)
                            {
                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0[$1] = [$2, $3];", newObj, csspropertyName, cssValue, from);
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



        internal override void Stop(FrameworkElement frameworkElement, string groupName, bool revertToFormerValue = false)
        {
            base.Stop(frameworkElement, groupName, revertToFormerValue);


            DependencyObject target;
            PropertyPath propertyPath;
            GetTargetElementAndPropertyInfo(frameworkElement, out target, out propertyPath);
            Type lastElementType = target.GetType();
            PropertyInfo propertyInfo = lastElementType.GetProperty(propertyPath.INTERNAL_DependencyPropertyName);


            //todo: find out why we put the test on target and put it back? (I removed it because id kept ScaleTransform from working properly)
            if (To != null)// && target is FrameworkElement) //todo: "To" can never be "null", fix this.
            {
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
                            CSHTML5.Interop.ExecuteJavaScriptAsync(@"
Velocity($0, ""stop"", $1);", cssEquivalent.DomElement, specificGroupName);
                        }
                    }
                }
                if (propertyMetadata.GetCSSEquivalents != null)
                {
                    List<CSSEquivalent> cssEquivalents = propertyMetadata.GetCSSEquivalents(target);
                    foreach (CSSEquivalent equivalent in cssEquivalents)
                    {
                        if (equivalent.DomElement != null)

                            CSHTML5.Interop.ExecuteJavaScriptAsync(@"
Velocity($0, ""stop"", $1);", equivalent.DomElement, specificGroupName);
                    }
                }
            }
            else
            {
                propertyPath.INTERNAL_PropertySetVisualState(target, To); //To = null here --> Is it really what we want to do?
            }

            if (revertToFormerValue) //todo: check if this is sufficient or if we need to put stuff into the GetCSSEquivalents thing like for ColorAnimation:
            {
                object formerValue = propertyInfo.GetValue(target);
                propertyInfo.SetValue(target, formerValue);
            }
        }
    }
}