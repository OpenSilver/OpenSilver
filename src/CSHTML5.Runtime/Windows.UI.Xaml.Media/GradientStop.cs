

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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

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
    public sealed partial class GradientStop : DependencyObject, ICloneOnAnimation
    {
        internal Brush INTERNAL_ParentBrush;

        private bool _isAlreadyAClone = false;

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
                GetCSSEquivalents = (instance) =>
                {
                    GradientStop gradientStop = instance as GradientStop;
                    if (gradientStop != null)
                    {
                        Brush brush = gradientStop.INTERNAL_ParentBrush;
                        if(brush != null)
                        {
                            Func<CSSEquivalent, ValueToHtmlConverter> parentPropertyToValueToHtmlConverter =
                            (parentPropertyCSSEquivalent) =>
                                ((inst, value) =>
                                {
                                    return brush;

                                    //todo: support "Velocity" animations by using a similar approach as the one of the SolidColorBrush?
                                });

                            return Brush.MergeCSSEquivalentsOfTheParentsProperties(brush, parentPropertyToValueToHtmlConverter);
                        }
                        else
                        {
                            // this may happen if the color property is set before the GradientStop has been added to the GradientStopCollection/GradientBrush. 
                            return new List<CSSEquivalent>();
                        }
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            }
            );

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
            DependencyProperty.Register("Offset", typeof(double), typeof(GradientStop), new PropertyMetadata(0d)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        public object Clone()
        {
            return new GradientStop
            {
                Color = this.Color,
                Offset = this.Offset,
                _isAlreadyAClone = true,
            };
        }

        public bool IsAlreadyAClone()
        {
            return _isAlreadyAClone;
        }

    }
}
