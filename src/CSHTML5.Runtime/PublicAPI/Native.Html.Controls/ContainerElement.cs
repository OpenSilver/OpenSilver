
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
using System.Windows.Markup;
using System.Diagnostics;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Native.Html.Controls
{
    /// <summary>
    /// The container can contain child elements (HtmlCanvasElements). Their position
    /// is relative to the container position.
    /// Full documentation is available at:
    /// http://cshtml5.com/links/how-to-use-the-html5-canvas.aspx
    /// </summary>
    /// <example>
    /// You can add a Container to the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <native:Canvas Width="1000" Height"500" xmlns:native="using:CSHTML5.Native.Html.Controls">
    ///     <native:Container X="200" Y="42" Width="100" Height="50" FillColor="Blue">
    ///             <!-- Place children here -->
    ///     </native:Container>
    /// </native:Canvas>
    /// </code>
    /// Or in C#:
    /// <code lang="C#">
    /// HtmlCanvas myCanvas = new HtmlCanvas() { Width = 1000, Height = 500 };
    /// ContainerElement myContainer = new ContainerElement()
    /// {
    ///     X = 200,
    ///     Y = 42,
    ///     Width = 100,
    ///     Height = 50,
    ///     FillColor = Colors.Blue
    /// };
    /// myCanvas.Children.Add(myContainer);
    /// </code>
    /// </example>
    [ContentProperty("Children")]
    public class ContainerElement : HtmlCanvasElement
    {
        /// <summary>
        /// Gets the collection of child elements of the container.
        /// </summary>
        public List<HtmlCanvasElement> Children;

        /// <summary>
        /// Width of the container
        /// </summary>
        public double Width;
        /// <summary>
        /// Height of the container
        /// </summary>
        public double Height;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContainerElement() : base()
        {
            this.Children = new List<HtmlCanvasElement>();
        }

        /// <summary>
        /// Draws the container and its children only if the container is visible.
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
                Interop.ExecuteJavaScriptAsync("$0.fillRect($1, $2, $3, $4)", jsContext2d, this.X + xParent, this.Y + yParent, this.Width, this.Height);
                Interop.ExecuteJavaScriptAsync("$0.strokeRect($1, $2, $3, $4)", jsContext2d, this.X + xParent, this.Y + yParent, this.Width, this.Height);

                foreach (HtmlCanvasElement elem in this.Children)
                {
                    currentDrawingStyle = elem.Draw(currentDrawingStyle, jsContext2d, this.X + xParent, this.Y + yParent);
                }
            }
            return currentDrawingStyle;
        }

        public override bool IsPointed(double x, double y)
        {
            return x >= this.X && x < this.X + this.Width && y >= this.Y && y < this.Y + this.Height;
        }
    }
}
