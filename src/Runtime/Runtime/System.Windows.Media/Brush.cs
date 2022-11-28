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
using System.Linq;
using System.Windows.Controls;
using CSHTML5.Internal;

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
    public partial class Brush : DependencyObject, IHasAccessToPropertiesWhereItIsUsed2
    {
        internal static Brush Parse(string source)
        {
            return new SolidColorBrush(Color.INTERNAL_ConvertFromString(source));
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

        private Dictionary<WrapperOfWeakReferenceOfDependencyObject, HashSet<DependencyProperty>> _propertiesWhereUsed;
        public Dictionary<WrapperOfWeakReferenceOfDependencyObject, HashSet<DependencyProperty>> PropertiesWhereUsed
        {
            get
            {
                if (_propertiesWhereUsed == null)
                {
                    _propertiesWhereUsed = new Dictionary<WrapperOfWeakReferenceOfDependencyObject, HashSet<DependencyProperty>>();
                }
                return _propertiesWhereUsed;
            }
        }

        internal static List<CSSEquivalent> MergeCSSEquivalentsOfTheParentsProperties(Brush brush, Func<CSSEquivalent, ValueToHtmlConverter> parentPropertyToValueToHtmlConverter) // note: "CSSEquivalent" here stands for the CSSEquicalent of the parent property.
        {
            List<CSSEquivalent> result = new List<CSSEquivalent>();
            foreach (var item in brush.PropertiesWhereUsed.ToList())
            {
                bool hasKey = item.Key.DependencyObject.TryGetTarget(out var dependencyObject);
                if (!hasKey)
                {
                    brush.PropertiesWhereUsed.Remove(item.Key);
                    continue;
                }

                UIElement uiElement = dependencyObject as UIElement;
                if (uiElement != null)
                {
                    if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
                    {
                        brush.PropertiesWhereUsed.Remove(item.Key);
                    }
                    else
                    {
                        foreach (var dependencyProperty in item.Value)
                        {
                            PropertyMetadata propertyMetadata = dependencyProperty.GetTypeMetaData(uiElement.GetType());
                            if (propertyMetadata.GetCSSEquivalent != null) // If the parent has a CSSEquivalent, we use it, otherwise we use the parent PropertyChanged method.
                            {
                                var parentPropertyCSSEquivalent = propertyMetadata.GetCSSEquivalent(uiElement);
                                if (parentPropertyCSSEquivalent != null)
                                {
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
                            }
                            else if (propertyMetadata.GetCSSEquivalents != null)
                            {
                                var parentPropertyCSSEquivalents = propertyMetadata.GetCSSEquivalents(uiElement);
                                foreach (var parentPropertyCSSEquivalent in parentPropertyCSSEquivalents)
                                {
                                    if (parentPropertyCSSEquivalent != null)
                                    {
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
                                }
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
                }
                else
                {
                    //Commented because it could be a Setter in a Style
                    //throw new NotSupportedException("A solidColorBrush cannot currently be set inside a class that desn't inherit from UIElement.");
                }
            }
            return result;
        }

        #region Transform, RelativeTransform (Not supported yet)
        /// <summary>Identifies the <see cref="P:System.Windows.Media.Brush.RelativeTransform" /> dependency property. </summary>
        /// <returns>The <see cref="P:System.Windows.Media.Brush.RelativeTransform" /> dependency property identifier.</returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RelativeTransformProperty = DependencyProperty.Register("RelativeTransform", typeof(Transform), typeof(Brush), null);

        /// <summary>Gets or sets the transformation that is applied to the brush using relative coordinates. </summary>
        /// <returns>The transformation that is applied to the brush using relative coordinates. The default value is null.</returns>
        [OpenSilver.NotImplemented]
        public Transform RelativeTransform
        {
            get { return (Transform)this.GetValue(Brush.RelativeTransformProperty); }
            set { this.SetValue(Brush.RelativeTransformProperty, (DependencyObject)value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", typeof(Transform), typeof(Brush), null);

        /// <summary>
        ///     Transform - Transform.  Default value is Transform.Identity.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Transform Transform
        {
            get { return (Transform)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }
        #endregion
    }
}
