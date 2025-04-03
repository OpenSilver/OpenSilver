
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

using System.Windows.Media;
using OpenSilver.Internal;

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
                fillColorStr = value.ToHtmlString(1);
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
                strokeColorStr = value.ToHtmlString(1);
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
                shadowColorStr = value.ToHtmlString(1);
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
            string context2d = OpenSilver.Interop.GetVariableStringForJS(jsContext2d);

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"""
                {context2d}.fillStyle = '{fillColorStr}';
                {context2d}.strokeStyle = '{strokeColorStr}';
                {context2d}.shadowColor = '{shadowColorStr}';
                {context2d}.shadowBlur = {ShadowBlur.ToInvariantString()};
                {context2d}.shadowOffsetX = {ShadowOffsetX.ToInvariantString()};
                {context2d}.shadowOffsetY = {ShadowOffsetY.ToInvariantString()};
                {context2d}.lineCap = '{LineCapToHtmlString(LineCap)}';
                {context2d}.lineJoin = '{LineJoinToHtmlString(LineJoin)}';
                {context2d}.lineWidth = {LineWidth.ToInvariantString()};
                {context2d}.miterLimit = {MiterLimit.ToInvariantString()};
                """);
        }

        internal static string LineJoinToHtmlString(LineJoin lineJoin)
        {
            return lineJoin switch
            {
                LineJoin.Bevel => "bevel",
                LineJoin.Round => "round",
                _ => "miter",
            };
        }

        internal static string LineCapToHtmlString(LineCap lineCap)
        {
            return lineCap switch
            {
                LineCap.Butt => "butt",
                LineCap.Round => "round",
                _ => "square",
            };
        }
    }
}
