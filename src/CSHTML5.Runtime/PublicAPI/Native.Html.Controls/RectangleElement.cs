
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
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Native.Html.Controls
{
    /// <summary>
    /// Html5 canvas rectangle.
    /// Full documentation is available at:
    /// http://cshtml5.com/links/how-to-use-the-html5-canvas.aspx
    /// </summary>
    /// <example>
    /// You can add a rectangle to the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <native:HtmlCanvas Width="1000" Height"500" xmlns:native="using:CSHTML5.Native.Html.Controls">
    ///     <native:RectangleElement X="200" Y="42" Width="100" Height="50" FillColor="Blue"/>
    /// </native:HtmlCanvas>
    /// </code>
    /// Or in C#:
    /// <code lang="C#">
    /// HtmlCanvas myCanvas = new HtmlCanvas() { Width = 1000, Height = 500 };
    /// RectangleElement myRectangle = new RectangleElement()
    /// {
    ///     X = 200,
    ///     Y = 42,
    ///     Width = 100,
    ///     Height = 50,
    ///     FillColor = Colors.Blue
    /// };
    /// myCanvas.Children.Add(myRectangle);
    /// </code>
    /// </example>
    public class RectangleElement : HtmlCanvasElement
    {
        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public double Width;

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public double Height;

        /// <summary>
        /// Default rectangle constructor
        /// </summary>
        public RectangleElement()
            : base()
        {
            this.Width = 0;
            this.Height = 0;
        }

        /// <summary>
        /// Constructor with specified position and size
        /// </summary>
        /// <param name="X">X position</param>
        /// <param name="Y">Y position</param>
        /// <param name="Width">Width of the rectangle</param>
        /// <param name="Height">Height of the rectangle</param>
        public RectangleElement(double X, double Y, double Width, double Height)
            : base(X, Y)
        {
            this.Width = Width;
            this.Height = Height;
        }

        /// <summary>
        /// Draws the rectangle
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
                //Interop.ExecuteJavaScript("$0.fillStyle = $1", jsContext2d, this.fillStyle);
                Interop.ExecuteJavaScriptAsync("$0.fillRect($1, $2, $3, $4)", jsContext2d, this.X + xParent, this.Y + yParent, this.Width, this.Height);
                Interop.ExecuteJavaScriptAsync("$0.strokeRect($1, $2, $3, $4)", jsContext2d, this.X + xParent, this.Y + yParent, this.Width, this.Height);
            }

            return currentDrawingStyle;
        }

        public override bool IsPointed(double x, double y)
        {
            return x >= this.X && x < this.X + this.Width && y >= this.Y && y < this.Y + this.Height;
        }
    }
}
