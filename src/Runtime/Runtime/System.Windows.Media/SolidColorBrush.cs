
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
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Markup;
using OpenSilver.Internal;

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
    public sealed class SolidColorBrush : Brush
    {
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

        internal string INTERNAL_ToHtmlString()
        {
            return this.Color.INTERNAL_ToHtmlString(this.Opacity); //todo-perfs: every time, accessing the "Opacity" property may be slow.
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ConvertToCSSValue()
        {
            return this.Color.INTERNAL_ToHtmlString(this.Opacity); //todo-perfs: every time, accessing the "Opacity" property may be slow.
        }

        public object Clone() => new SolidColorBrush(Color);

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsAlreadyAClone() => false;

        public override string ToString()
        {
            return this.Color.ToString();
        }

        internal override Task<string> GetDataStringAsync(UIElement parent)
            => Task.FromResult(INTERNAL_ToHtmlString());
    }
}
