
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Shapes;
using OpenSilver.Internal;
using OpenSilver.Internal.Media.Animation;

namespace System.Windows.Media
{
    /// <summary>
    /// Paints an area with a solid color.
    /// </summary>
    [ContentProperty(nameof(Color))]
    public sealed class SolidColorBrush : Brush, ICloneOnAnimation<SolidColorBrush>
    {
        private readonly bool _isClone;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidColorBrush"/> class 
        /// with no color.
        /// </summary>
        public SolidColorBrush() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidColorBrush"/> class
        /// with the specified <see cref="Color"/>.
        /// </summary>
        /// <param name="color">
        /// The color to apply to the brush.
        /// </param>
        public SolidColorBrush(Color color)
        {
            Color = color;
        }

        private SolidColorBrush(SolidColorBrush original)
            : base(original)
        {
            _isClone = true;

            Color = original.Color;
        }

        /// <summary>
        /// Gets or sets the color of this <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <returns>
        /// The brush's color. The default value is <see cref="Colors.Transparent"/>.
        /// </returns>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(Color),
                typeof(SolidColorBrush),
                new PropertyMetadata(Colors.Transparent)
                {
                    GetCSSEquivalents = static (instance) =>
                    {
                        static ValueToHtmlConverter ConvertValueToHtml(CSSEquivalent cssEquivalent)
                        {
                            return (inst, value) =>
                            {
                                var valuesDict = new Dictionary<string, object>();
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
                            };
                        }

                        return MergeCSSEquivalentsOfTheParentsProperties((Brush)instance, ConvertValueToHtml);
                    }
                });

        internal string INTERNAL_ToHtmlString() => Color.INTERNAL_ToHtmlString(Opacity);

        SolidColorBrush ICloneOnAnimation<SolidColorBrush>.Clone() => new SolidColorBrush(this);

        bool ICloneOnAnimation<SolidColorBrush>.IsClone => _isClone;

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ConvertToCSSValue() => Color.INTERNAL_ToHtmlString(Opacity);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Clone() => new SolidColorBrush(this);

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsAlreadyAClone() => false;

        public override string ToString() => Color.ToString();

        internal override Task<string> GetDataStringAsync(UIElement parent)
            => Task.FromResult(INTERNAL_ToHtmlString());

        internal override ISvgBrush GetSvgElement() => new SvgSolidColorBrush(this);

        private sealed class SvgSolidColorBrush : ISvgBrush
        {
            private readonly SolidColorBrush _brush;

            public SvgSolidColorBrush(SolidColorBrush scb)
            {
                _brush = scb ?? throw new ArgumentNullException(nameof(scb));
            }

            public string GetBrush(Shape shape) => _brush.INTERNAL_ToHtmlString();

            public void DestroyBrush(Shape shape) { }
        }
    }
}
