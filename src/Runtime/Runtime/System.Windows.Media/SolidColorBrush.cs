

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
using System.Windows.Markup;
#if !MIGRATION
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Paints an area with a solid color.
    /// </summary>
    [ContentProperty("Color")]
    public sealed partial class SolidColorBrush : Brush, ICanConvertToCSSValue, ICloneOnAnimation
    {
        bool _isAlreadyAClone = false;

        /// <summary>
        /// Initializes a new instance of the SolidColorBrush class with no color.
        /// </summary>
        public SolidColorBrush()
        {
        }
        /// <summary>
        /// Initializes a new instance of the SolidColorBrush class with the specified
        /// Color.
        /// </summary>
        /// <param name="color">The color to apply to the brush.</param>
        public SolidColorBrush(Color color)
        {
            this.Color = color;
        }

        /// <summary>
        /// Gets or sets the color of this SolidColorBrush.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        /// <summary>
        /// Identifies the Color dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(SolidColorBrush), new PropertyMetadata(Color.FromArgb(0, 0, 0, 0))
            {
                GetCSSEquivalents = (instance) =>
                    {
                        Brush brush = instance as Brush;
                        if (brush != null)
                        {
                            Func<CSSEquivalent, ValueToHtmlConverter> parentPropertyToValueToHtmlConverter =
                                (parentPropertyCSSEquivalent) =>
                                    ((inst, value) =>
                                    {
                                        Dictionary<string, object> valuesDict = new Dictionary<string, object>();
                                        foreach (string name in parentPropertyCSSEquivalent.Name)
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
                                    });

                            return MergeCSSEquivalentsOfTheParentsProperties(brush, parentPropertyToValueToHtmlConverter);
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                    }
            }
            );


        //todo: put back the Color_Changed method and use PropertiesWhereUsed to do something (only when there is no CSSEquivalent?) OR do something in the GetCSSEquivalents above.



        //private static void Color_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    SolidColorBrush brush = (SolidColorBrush)d;
        //    PropertyMetadata metaData = ColorProperty.GetTypeMetaData(typeof(SolidColorBrush));
        //    List<CSSEquivalent> cssEquivalents = metaData.GetCSSEquivalents(brush);
        //    Color newValue = (Color)e.NewValue;
        //    foreach (CSSEquivalent cssEquivalent in cssEquivalents)
        //    {
        //        if (cssEquivalent.ApplyWhenControlHasTemplate) //Note: this is to handle the case of a Control with a ControlTemplate (some properties must not be applied on the control itself)
        //        {
        //            INTERNAL_HtmlDomManager.SetDomElementStyleProperty(cssEquivalent.DomElement, cssEquivalent.Name, newValue.INTERNAL_ToHtmlString(brush.Opacity));
        //        }
        //    }
        //}


        internal string INTERNAL_ToHtmlString()
        {
            return this.Color.INTERNAL_ToHtmlString(this.Opacity); //todo-perfs: every time, accessing the "Opacity" property may be slow.
        }

        ///// <summary>
        ///// Intended to be called by objects that use the brush in order to apply the brush.
        ///// </summary>
        //public void Render(DependencyObject parentObjectWhereTheBrushIsUsed, DependencyProperty parentPropertyWhereTheBrushIsUsed)
        //{
        //    PropertyMetadata metaData = parentPropertyWhereTheBrushIsUsed.GetTypeMetaData(parentObjectWhereTheBrushIsUsed.GetType());
        //    CSSEquivalent parentPropertyCSSEquivalent = metaData.GetCSSEquivalent(parentObjectWhereTheBrushIsUsed);
        //    INTERNAL_HtmlDomManager.SetDomElementStyleProperty(parentPropertyCSSEquivalent.DomElement, parentPropertyCSSEquivalent.Name, this.INTERNAL_ToHtmlString());
        //}

        public object ConvertToCSSValue()
        {
            return this.Color.INTERNAL_ToHtmlString(this.Opacity); //todo-perfs: every time, accessing the "Opacity" property may be slow.
        }

        public object Clone()
        {
            return new SolidColorBrush(this.Color) { _isAlreadyAClone = true };
        }

        public bool IsAlreadyAClone()
        {
            return _isAlreadyAClone;
        }


        public override string ToString()
        {
            return this.Color.ToString();
        }
    }
}
