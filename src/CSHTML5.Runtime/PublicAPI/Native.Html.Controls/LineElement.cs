
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
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Native.Html.Controls
{
    /// <summary>
    /// Html5 canvas line.
    /// Full documentation is available at:
    /// http://cshtml5.com/links/how-to-use-the-html5-canvas.aspx
    /// </summary>
    /// <example>
    /// You can add a line to the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <native:HtmlCanvas Width="1000" Height="500" xmlns:native="using:CSHTML5.Native.Html.Controls">
    ///     <native:LineElement X1="200" Y1="42" X2="100" Y2="50" StrokeColor="Red" LineWidth="2"/>
    /// </native:HtmlCanvas>
    /// </code>
    /// Or in C#:
    /// <code lang="C#">
    /// HtmlCanvas myCanvas = new HtmlCanvas() { Width = 1000, Height = 500 };
    /// LineElement myLine = new LineElement()
    /// {
    ///     X1 = 200,
    ///     Y1 = 42,
    ///     X2 = 100,
    ///     Y2 = 50,
    ///     StrokeColor = Colors.Red,
    ///     LineWidth = 2
    /// };
    /// myCanvas.Children.Add(myLine);
    /// </code>
    /// </example>
    public class LineElement : HtmlCanvasElement
    {
        /// <summary>
        /// X coordinate of the start point
        /// </summary>
        public double X1;

        /// <summary>
        /// Y coordinate of the start point
        /// </summary>
        public double Y1;

        /// <summary>
        /// X coordinate of the end point
        /// </summary>
        public double X2;

        /// <summary>
        /// Y coordinate of the end point
        /// </summary>
        public double Y2;

        /// <summary>
        /// Default line constructor
        /// </summary>
        public LineElement()
            : base()
        {
            
        }

        /// <summary>
        /// Create line with specified start and end points
        /// </summary>
        /// <param name="X1">X coordinate of the start point</param>
        /// <param name="Y1">Y coordinate of the start point</param>
        /// <param name="X2">X coordinate of the end point</param>
        /// <param name="Y2">Y coordinate of the end point</param>
        public LineElement(double X1, double Y1, double X2, double Y2)
        {
            this.X1 = X1;
            this.Y1 = Y1;
            this.X2 = X2;
            this.Y2 = Y2; 
        }

        /// <summary>
        /// Draws the line
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
                Interop.ExecuteJavaScriptAsync("$0.moveTo($1, $2)", jsContext2d, this.X1 + xParent, this.Y1 + yParent);
                Interop.ExecuteJavaScriptAsync("$0.lineTo($1, $2)", jsContext2d, this.X2 + xParent, this.Y2 + yParent);
                Interop.ExecuteJavaScriptAsync("$0.stroke()", jsContext2d);
            }

            return currentDrawingStyle;
        }

        public override bool IsPointed(double x, double y)
        {
            // TODO: implement the method
            return false;
        }
    }
}
