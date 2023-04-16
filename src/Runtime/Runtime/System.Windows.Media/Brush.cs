
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
using System.Threading.Tasks;
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

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
    public partial class Brush : DependencyObject,
        IHasAccessToPropertiesWhereItIsUsed2,
#pragma warning disable CS0618 // Type or member is obsolete
        IHasAccessToPropertiesWhereItIsUsed
#pragma warning restore CS0618 // Type or member is obsolete
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

        private HashSet<KeyValuePair<DependencyObject, DependencyProperty>> _propertiesWhereUsedObsolete;

        [Obsolete(Helper.ObsoleteMemberMessage)]
        public HashSet<KeyValuePair<DependencyObject, DependencyProperty>> PropertiesWhereUsed
                => _propertiesWhereUsedObsolete ??= new();

        private Dictionary<WeakDependencyObjectWrapper, HashSet<DependencyProperty>> _propertiesWhereUsed;

        Dictionary<WeakDependencyObjectWrapper, HashSet<DependencyProperty>> IHasAccessToPropertiesWhereItIsUsed2.PropertiesWhereUsed
            => _propertiesWhereUsed ??= new();

        internal static List<CSSEquivalent> MergeCSSEquivalentsOfTheParentsProperties(
            IHasAccessToPropertiesWhereItIsUsed2 brush,
            Func<CSSEquivalent, ValueToHtmlConverter> parentPropertyToValueToHtmlConverter) // note: "CSSEquivalent" here stands for the CSSEquicalent of the parent property.
        {
            var result = new List<CSSEquivalent>();
            foreach (var item in brush.PropertiesWhereUsed.ToArray())
            {
                if (!item.Key.TryGetDependencyObject(out DependencyObject dependencyObject))
                {
                    brush.PropertiesWhereUsed.Remove(item.Key);
                    continue;
                }

                if (dependencyObject is not UIElement uiElement)
                {
                    continue;
                }

                if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
                {
                    brush.PropertiesWhereUsed.Remove(item.Key);
                    continue;
                }

                foreach (var dependencyProperty in item.Value)
                {
                    if (dependencyProperty == Border.BorderBrushProperty)
                    {
                        if (brush is LinearGradientBrush)
                        {
                            result.Add(ProcessCSSEquivalent(
                                new CSSEquivalent { Name = new List<string>(1) { "border-image-source" } },
                                uiElement,
                                parentPropertyToValueToHtmlConverter));

                            result.Add(ProcessCSSEquivalent(
                                new CSSEquivalent { Name = new List<string>(1) { "border-image-slice" } },
                                uiElement,
                                parentPropertyToValueToHtmlConverter));
                        }
                        else
                        {
                            result.Add(ProcessCSSEquivalent(
                                new CSSEquivalent { Name = new List<string>(1) { "borderColor" } },
                                uiElement,
                                parentPropertyToValueToHtmlConverter));
                        }
                    }
                    else if (dependencyProperty == Border.BackgroundProperty ||
                        dependencyProperty == Panel.BackgroundProperty ||
                        dependencyProperty == Control.BackgroundProperty)
                    {
                        result.Add(ProcessCSSEquivalent(
                            new CSSEquivalent { Name = new List<string>(3) { "background", "backgroundColor", "backgroundColorAlpha" } },
                            uiElement,
                            parentPropertyToValueToHtmlConverter));
                    }
                    else if (dependencyProperty == Control.ForegroundProperty)
                    {
                        result.Add(ProcessCSSEquivalent(
                            new CSSEquivalent
                            {
                                Name = new List<string>(2) { "color", "colorAlpha" },
                                ApplyAlsoWhenThereIsAControlTemplate = true,
                            },
                            uiElement,
                            parentPropertyToValueToHtmlConverter));
                    }
                    else
                    {
                        result.Add(ProcessCSSEquivalent(
                            new CSSEquivalent
                            {
                                CallbackMethod = dependencyProperty.GetMetadata(uiElement.GetType()).PropertyChangedCallback,
                                DependencyProperty = dependencyProperty
                            },
                            uiElement,
                            parentPropertyToValueToHtmlConverter));
                    }
                }
            }
            return result;

            static CSSEquivalent ProcessCSSEquivalent(CSSEquivalent cssEquivalent,
                UIElement uie,
                Func<CSSEquivalent, ValueToHtmlConverter> valueConverter)
            {
                cssEquivalent.Value = valueConverter(cssEquivalent);
                cssEquivalent.DomElement ??= uie.INTERNAL_OuterDomElement;
                cssEquivalent.UIElement = uie;
                return cssEquivalent;
            }
        }

        internal virtual Task<string> GetDataStringAsync(UIElement parent)
            => Task.FromResult(string.Empty);

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
