
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Native.Html.Controls
{
    /// <summary>
    /// Html5 canvas text.
    /// Full documentation is available at:
    /// http://cshtml5.com/links/how-to-use-the-html5-canvas.aspx
    /// </summary>
    /// <example>
    /// You can add text to the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <native:HtmlCanvas Width="1000" Height"500" xmlns:native="using:CSHTML5.Native.Html.Controls">
    ///     <native:TextElement X="200" Y="42" Text="I am the text" Font="20px Arial"/>
    /// </native:HtmlCanvas>
    /// </code>
    /// Or in C#:
    /// <code lang="C#">
    /// HtmlCanvas myCanvas = new HtmlCanvas() { Width = 1000, Height = 500 };
    /// TextElement myText = new TextElement()
    /// {
    ///     X = 200,
    ///     Y = 42,
    ///     Text = "I am the text",
    ///     Font = "20px Arial"
    /// };
    /// myCanvas.Children.Add(myText);
    /// </code>
    /// </example>
    public class TextElement : HtmlCanvasElement
    {
        static private object _jsCanvasForMeasuringTextWidth;

        private const double DEFAULT_FONT_HEIGHT = 12;

        /// <summary>
        /// Text value
        /// </summary>
        public string Text = "";

        /// <summary>
        /// Font of the text, in javascript format (eg. "20px Arial")
        /// </summary>
        public string Font;

        public double FontHeight;

        public FontWeight FontWeight;

        public virtual double ActualWidth
        {
            get
            {
                if (_jsCanvasForMeasuringTextWidth == null)
                    _jsCanvasForMeasuringTextWidth = Interop.ExecuteJavaScriptAsync("document.createElement('canvas').getContext('2d')");
                return Convert.ToDouble(Interop.ExecuteJavaScript("$0.measureText($1).width", _jsCanvasForMeasuringTextWidth, this.Text)) * FontHeight / DEFAULT_FONT_HEIGHT;
            }
        }

        /// <summary>
        /// Default text constructor (Black, 12px, Georgia)
        /// </summary>
        public TextElement() : base()
        {
            this.FillColor = Color.FromArgb(255, 0, 0, 0);
            this.FontWeight = FontWeights.Normal;
            this.FontHeight = DEFAULT_FONT_HEIGHT;
            this.Font = "Georgia";
        }

        /// <summary>
        /// Draws the text
        /// </summary>
        /// <param name="currentDrawingStyle">Draw style used for last element (can allow optimizations, null if unknown)</param>
        /// <param name="jsContext2d">Canvas 2d javascript context</param>
        /// <param name="xParent">X position of the parent element</param>
        /// <param name="yParent">Y position of the parent element</param>
        /// <returns>Draw style used for this element (can be null)</returns>
        public override ElementStyle Draw(ElementStyle currentDrawingStyle, object jsContext2d, double xParent = 0, double yParent = 0)
        {
            if (this.Visibility == Visibility.Visible)
            {
                currentDrawingStyle = this.ApplyStyle(currentDrawingStyle, jsContext2d);

                Interop.ExecuteJavaScriptAsync("$0.font = $1", jsContext2d, this.FontWeight.ToString() + " " + this.FontHeight.ToString() + "px " + this.Font); //todo: use "InvariantCulture" in the ToString() when supported.
                Interop.ExecuteJavaScriptAsync("$0.fillText($1, $2, $3)", jsContext2d, this.Text, this.X + xParent, this.Y + yParent + this.FontHeight);
            }

            return currentDrawingStyle;
        }

        public override bool IsPointed(double x, double y)
        {
            return x >= this.X && x < this.X + this.ActualWidth && y >= this.Y && y < this.Y + this.ActualHeight;
        }
    }
}
