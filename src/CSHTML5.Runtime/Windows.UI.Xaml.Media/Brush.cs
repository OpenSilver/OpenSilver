
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


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Defines objects used to paint graphical objects. Classes that derive from
    /// Brush describe how the area is painted.
    /// </summary>
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(BrushConverter))]
#endif
    public class Brush : DependencyObject, IHasAccessToPropertiesWhereItIsUsed
    {
        static Brush()
        {
            TypeFromStringConverters.RegisterConverter(typeof(Brush), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string colorcode)
        {
            return new SolidColorBrush((Color)Color.INTERNAL_ConvertFromString(colorcode));
        }

        /// <summary>
        /// Gets or sets the degree of opacity of a Brush.
        /// The value of the Opacity property is expressed as a value between 0 and 1.0.
        /// The default value is 1.0, which is full opacity. 0 is transparent opacity.
        /// </summary>
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        /// <summary>
        /// Identifies the Opacity dependency property.
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(Brush), new PropertyMetadata(1d));

        private HashSet2<KeyValuePair<DependencyObject, DependencyProperty>> _propertiesWhereUsed;
        public HashSet2<KeyValuePair<DependencyObject, DependencyProperty>> PropertiesWhereUsed
        {
            get
            {
                if(_propertiesWhereUsed == null)
                {
                    _propertiesWhereUsed = new HashSet2<KeyValuePair<DependencyObject, DependencyProperty>>();
                }
                return _propertiesWhereUsed;
            }
        }

        internal static List<CSSEquivalent> MergeCSSEquivalentsOfTheParentsProperties(Brush brush, Func<CSSEquivalent, ValueToHtmlConverter> parentPropertyToValueToHtmlConverter) // note: "CSSEquivalent" here stands for the CSSEquicalent of the parent property.
        {
            List<CSSEquivalent> result = new List<CSSEquivalent>();
            foreach (KeyValuePair<DependencyObject, DependencyProperty> tuple in brush.PropertiesWhereUsed)
            {
                UIElement uiElement = tuple.Key as UIElement;
                DependencyProperty dependencyProperty = tuple.Value;
                if (uiElement != null)
                {
                    if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
                    {
                        brush.PropertiesWhereUsed.Remove(tuple);
                    }
                    else
                    {
                        PropertyMetadata propertyMetadata = dependencyProperty.GetTypeMetaData(uiElement.GetType());
                        if (propertyMetadata.GetCSSEquivalent != null) // If the parent has a CSSEquivalent, we use it, otherwise we use the parent PropertyChanged method.
                        {
                            var parentPropertyCSSEquivalent = propertyMetadata.GetCSSEquivalent(uiElement);
                            CSSEquivalent newCSSEquivalent = new CSSEquivalent()
                            {
                                Name = parentPropertyCSSEquivalent.Name,
                                ApplyAlsoWhenThereIsAControlTemplate = parentPropertyCSSEquivalent.ApplyAlsoWhenThereIsAControlTemplate,

                                Value = parentPropertyToValueToHtmlConverter(parentPropertyCSSEquivalent),
                                DomElement = parentPropertyCSSEquivalent.DomElement,
                                UIElement = uiElement
                            };
                            if (newCSSEquivalent.DomElement == null)
                            {
                                newCSSEquivalent.DomElement = uiElement.INTERNAL_OuterDomElement;
                            }
                            result.Add(newCSSEquivalent);
                        }
                        else
                        {
                            //we want to create a CSSEquivalent that will just make the UIElement call the property callback if any:
                            if (propertyMetadata.PropertyChangedCallback != null)
                            {
                                result.Add(new CSSEquivalent()
                                {
                                    UIElement = uiElement,
                                    CallbackMethod = propertyMetadata.PropertyChangedCallback,
                                    DependencyProperty = dependencyProperty
                                });
                            }
                        }
                    }
                }
                else
                {
                    //Commented because it could be a Setter in a Style
                    //throw new NotSupportedException("A solidColorBrush cannot currently be set inside a class that desn't inherit from UIElement.");
                }
            }
            return result;
        }

#if WORKINPROGRESS
        #region Transform, RelativeTransform (Not supported yet)
        /// <summary>Identifies the <see cref="P:System.Windows.Media.Brush.RelativeTransform" /> dependency property. </summary>
        /// <returns>The <see cref="P:System.Windows.Media.Brush.RelativeTransform" /> dependency property identifier.</returns>
        public static readonly DependencyProperty RelativeTransformProperty = DependencyProperty.Register("RelativeTransform", typeof(Transform), typeof(Brush), null);

        /// <summary>Gets or sets the transformation that is applied to the brush using relative coordinates. </summary>
        /// <returns>The transformation that is applied to the brush using relative coordinates. The default value is null.</returns>
        public Transform RelativeTransform
        {
            get { return (Transform)this.GetValue(Brush.RelativeTransformProperty); }
            set { this.SetValue(Brush.RelativeTransformProperty, (DependencyObject)value); }
        }

        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", typeof(Transform), typeof(Brush), null);

        /// <summary>
        ///     Transform - Transform.  Default value is Transform.Identity.
        /// </summary>
        public Transform Transform
        {
            get { return (Transform)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }
        #endregion
#endif
    }
}
