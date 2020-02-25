
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
    /// Html5 canvas circle.
    /// Full documentation is available at:
    /// http://cshtml5.com/links/how-to-use-the-html5-canvas.aspx
    /// </summary>
    /// <example>
    /// You can add a circle to the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <native:HtmlCanvas Width="1000" Height"500" xmlns:native="using:CSHTML5.Native.Html.Controls">
    ///     <native:CircleElement X="200" Y="42" Diameter="100" FillColor="Blue"/>
    /// </native:HtmlCanvas>
    /// </code>
    /// Or in C#:
    /// <code lang="C#">
    /// HtmlCanvas myCanvas = new HtmlCanvas() { Width = 1000, Height = 500 };
    /// CircleElement myCircle = new CircleElement()
    /// {
    ///     X = 200,
    ///     Y = 42,
    ///     Diameter = 100,
    ///     FillColor = Colors.Blue
    /// };
    /// myCanvas.Children.Add(myCircle);
    /// </code>
    /// </example>
    public class CircleElement : HtmlCanvasElement
    {
        /// <summary>
        /// Diameter of the circle == Width == Height
        /// </summary>
        public double Diameter;

        /// <summary>
        /// Radius of the circle
        /// </summary>
        public double Radius {  get { return Diameter / 2; } set { Diameter = 2.0 * value; } }

        /// <summary>
        /// Default circle constructor
        /// </summary>
        public CircleElement()
            : base()
        {
            this.Diameter = 0;
        }

        /// <summary>
        /// Constructor with specified position and size
        /// </summary>
        /// <param name="X">X position of the centre</param>
        /// <param name="Y">Y position of the centre</param>
        /// <param name="Radius">Radius of the circle</param>
        public CircleElement(double X, double Y, double Radius)
            : base(X, Y)
        {
            this.Radius = Radius;
        }

        /// <summary>
        /// Draws the circle
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
                Interop.ExecuteJavaScriptAsync("$0.beginPath()", jsContext2d);
                Interop.ExecuteJavaScriptAsync("$0.arc($1, $2, $3, $4, $5, $6)", jsContext2d, this.X + xParent, this.Y + yParent, this.Radius, 0.0, Math.PI * 2.0, 0);
                Interop.ExecuteJavaScriptAsync("$0.fill()", jsContext2d);
            }

            return currentDrawingStyle;
        }

        public override bool IsPointed(double x, double y)
        {
            return (x - this.X) * (x - this.X) + (y - this.Y) * (y - this.Y) <= Radius * Radius;
        }
    }
}
