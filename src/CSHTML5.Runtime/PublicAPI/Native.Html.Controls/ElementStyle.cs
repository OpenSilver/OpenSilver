
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
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml.Media;
#endif

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
            Interop.ExecuteJavaScriptAsync(@"
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
            Interop.ExecuteJavaScriptAsync(@"
$0.miterLimit = $1", jsContext2d,
                      this.MiterLimit);
        }

        static private string ConvertColorToHtml(Color color)
        {
            return "rgba(" + color.R + ", " + color.G + ", " + color.B + ", " + ((double)color.A / 255).ToString().Replace(',', '.') + ")";
        }
    }
}
