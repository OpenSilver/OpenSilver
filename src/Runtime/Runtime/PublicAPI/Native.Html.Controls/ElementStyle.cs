

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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CSHTML5.Native.Html.Controls
{
    /// <summary>
    /// Provides a drawing style to an element
    /// </summary>
    public class ElementStyle
    {
        /// <summary>
        /// Visibility of the element
        /// </summary>
        public bool IsVisible;

        // Color cached as strings for speed improvement
        internal string fillColorStr;
        internal string strokeColorStr;
        internal string shadowColorStr;

        // Color properties
        private Color _fillColor;
        private Color _strokeColor;
        private Color _shadowColor;

        /// <summary>
        /// Fill color
        /// </summary>
        public Color FillColor {
            get { return _fillColor; }
            set
            {
                fillColorStr = ConvertColorToHtml(value);
                _fillColor = value;
            }
        }

        /// <summary>
        /// Stroke color
        /// </summary>
        public Color StrokeColor
        {
            get { return _strokeColor; }
            set
            {
                strokeColorStr = ConvertColorToHtml(value);
                _strokeColor = value;
            }
        }

        /// <summary>
        /// Shadow color
        /// </summary>
        public Color ShadowColor
        {
            get { return _shadowColor; }
            set
            {
                shadowColorStr = ConvertColorToHtml(value);
                _shadowColor = value;
            }
        }

        /// <summary>
        /// Shadow blur
        /// </summary>
        public double ShadowBlur { get; set; }

        /// <summary>
        /// Shadow horizontal offset
        /// </summary>
        public double ShadowOffsetX { get; set; }

        /// <summary>
        /// Shadow vertical offset
        /// </summary>
        public double ShadowOffsetY { get; set; }

        /// <summary>
        /// Style of the end caps for a line
        /// </summary>
        public LineCap LineCap { get; set; }

        /// <summary>
        /// Type of corner created, when two lines meet
        /// </summary>
        public LineJoin LineJoin { get; set; }

        /// <summary>
        /// Line Width
        /// </summary>
        public double LineWidth { get; set; }

        /// <summary>
        /// Maximum miter length
        /// </summary>
        public double MiterLimit { get; set; }

        private static ElementStyle _Default { get; set; }
        /// <summary>
        /// Default style (can be modified)
        /// </summary>
        public static ElementStyle Default
        {
            get
            {
                if (_Default == null)
                    _Default = new ElementStyle();
                return _Default;
            }
        }

        /// <summary>
        /// Empty style constructor
        /// </summary>
        public ElementStyle()
        {
            // Default js style values
            //this.IsVisible = true;
            this.FillColor = Color.FromArgb(0, 0, 0, 0);
            this.StrokeColor = Color.FromArgb(0, 0, 0, 0);
            this.ShadowColor = Color.FromArgb(0, 0, 0, 0);
            this.LineWidth = 1;
            this.MiterLimit = 10;
        }

        /// <summary>
        /// Apply this style as the current drawing style
        /// </summary>
        /// <param name="jsContext2d">Canvas 2d javascript context</param>
        internal void Apply(object jsContext2d)
        {
            OpenSilver.Interop.ExecuteJavaScriptAsync(@"
$0.fillStyle = $1;
$0.strokeStyle = $2;
$0.shadowColor = $3;
$0.shadowBlur = $4;
$0.shadowOffsetX = $5;
$0.shadowOffsetY = $6;
$0.lineCap = $7;
$0.lineJoin = $8;
$0.lineWidth = $9", jsContext2d,
                      this.fillColorStr,
                      this.strokeColorStr,
                      this.shadowColorStr,
                      this.ShadowBlur,
                      this.ShadowOffsetX,
                      this.ShadowOffsetY,
                      this.LineCap.ToString().ToLower(),
                      this.LineJoin.ToString().ToLower(),
                      this.LineWidth);

            // Note: the following is done on a separate line because of a limitation of JSIL where $10 is understood as $1 followed by a 0.
            OpenSilver.Interop.ExecuteJavaScriptAsync(@"
$0.miterLimit = $1", jsContext2d,
                      this.MiterLimit);
        }

        static private string ConvertColorToHtml(Color color)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "rgba({0}, {1}, {2}, {3})",
                color.R, color.G, color.B, color.A / 255d);
        }
    }
}
