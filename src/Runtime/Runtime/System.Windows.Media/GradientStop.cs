
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
using System.Windows.Markup;
using OpenSilver.Internal;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Describes the location and color of a transition point in a gradient.
    /// </summary>
    [ContentProperty("Color")]
    public sealed class GradientStop : DependencyObject
    {
        internal Brush INTERNAL_ParentBrush;

        // Returns:
        //     The color of the gradient stop. The default is Transparent.
        /// <summary>
        /// Gets or sets the color of the gradient stop.
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
            DependencyProperty.Register("Color", typeof(Color), typeof(GradientStop), new PropertyMetadata(Color.FromArgb(0, 0, 0, 0))
            {
                GetCSSEquivalents = static (instance) =>
                {
                    GradientStop gradientStop = (GradientStop)instance;
                    Brush brush = gradientStop.INTERNAL_ParentBrush;
                    if (brush != null)
                    {
                        ValueToHtmlConverter ConvertValueToHtml(CSSEquivalent cssEquivalent) => (inst, value) => brush;

                        return Brush.MergeCSSEquivalentsOfTheParentsProperties(brush, ConvertValueToHtml);
                    }
                    else
                    {
                        // this may happen if the color property is set before the GradientStop has been added to the GradientStopCollection/GradientBrush. 
                        return new List<CSSEquivalent>();
                    }
                }
            });

        /// <summary>
        /// Gets the location of the gradient stop within the gradient vector.
        /// </summary>
        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }
        /// <summary>
        /// Identifies the Offset dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(GradientStop), new PropertyMetadata(0d));

        public object Clone() => new GradientStop { Color = Color, Offset = Offset };

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsAlreadyAClone() => false;
    }
}
