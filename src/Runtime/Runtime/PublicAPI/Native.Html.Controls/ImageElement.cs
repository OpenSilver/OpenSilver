
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

using System.Windows;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace CSHTML5.Native.Html.Controls
{
    /// <summary>
    /// Html5 canvas image.
    /// Full documentation is available at:
    /// http://cshtml5.com/links/how-to-use-the-html5-canvas.aspx
    /// </summary>
    /// <example>
    /// You can add an image to the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <native:HtmlCanvas Width="1000" Height="500" xmlns:native="using:CSHTML5.Native.Html.Controls">
    ///     <native:ImageElement Source="ms-appx:///MyAssembly/MyFolder/Image.png" X="200" Y="42" Width="100" Height="50"/>
    /// </native:HtmlCanvas>
    /// </code>
    /// Or in C#:
    /// <code lang="C#">
    /// HtmlCanvas myCanvas = new HtmlCanvas() { Width = 1000, Height = 500 };
    /// ImageElement myImage = new ImageElement()
    /// {
    ///     Source = "ms-appx:///MyAssembly/MyFolder/Image.png",
    ///     X = 200,
    ///     Y = 42,
    ///     Width = 100,
    ///     Height = 50,
    /// };
    /// myCanvas.Children.Add(myImage);
    /// </code>
    /// </example>
    public class ImageElement : HtmlCanvasElement
    {
        private object _jsImage;
        private string _sourceCache;
        private string _source;
        private string _lastDrawnSource;
        private double _lastDrawnWidth;
        private double _lastDrawnHeight;

        /// <summary>
        /// Width of the container
        /// </summary>
        public double Width;
        /// <summary>
        /// Height of the container
        /// </summary>
        public double Height;

        /// <summary>
        /// Gets or sets the source for the image. It is possible to use either the
        /// syntax "ms-appx:///AssemblyName/FolderName/FileName.png" or the syntax
        /// "/AssemblyName;component/FolderName/FileName.png".
        /// </summary>
        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
                _sourceCache = INTERNAL_UriHelper.ConvertToHtml5Path(value, null);
            }
        }

        /// <summary>
        /// Default image constructor
        /// </summary>
        public ImageElement() : base()
        {
            _jsImage = OpenSilver.Interop.ExecuteJavaScript("document.createElement('img')");
        }

        /// <summary>
        /// Draws the image
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

                string context2d = OpenSilver.Interop.GetVariableStringForJS(jsContext2d);
                string image = OpenSilver.Interop.GetVariableStringForJS(_jsImage);

                if (this._lastDrawnSource != this._sourceCache
                    || this._lastDrawnWidth != this.Width
                    || this._lastDrawnHeight != this.Height)
                {
                    OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                        $"""
                        {image}.width = {Width.ToInvariantString()};
                        {image}.height = {Height.ToInvariantString()};
                        {image}.src = {OpenSilver.Interop.GetVariableStringForJS(_sourceCache)};
                        """);

                    this._lastDrawnSource = this._sourceCache;
                    this._lastDrawnWidth = this.Width;
                    this._lastDrawnHeight = this.Height;
                }

                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"{context2d}.drawImage({image}, {(X + xParent).ToInvariantString()}, {(Y + yParent).ToInvariantString()})");
            }

            return currentDrawingStyle;
        }

        public override bool IsPointed(double x, double y)
        {
            return x >= this.X && x < this.X + this.Width && y >= this.Y && y < this.Y + this.Height;
        }
    }
}
