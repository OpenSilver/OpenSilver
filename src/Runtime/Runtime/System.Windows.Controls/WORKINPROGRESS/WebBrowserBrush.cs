
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

#if MIGRATION
using System.Windows.Media;
namespace System.Windows.Controls
#else
using Windows.UI.Xaml.Media;
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides a brush that renders the currently hosted HTML.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class WebBrowserBrush : TileBrush
    {
        /// <summary>
        /// Initializes a new instance of the WebBrowserBrush class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public WebBrowserBrush() { }

        /// <summary>
        /// Gets the name of the source <see cref="WebBrowser"/> control that provides the HTML content.
        /// </summary>
        [OpenSilver.NotImplemented]
        public string SourceName { get; set; }

        /// <summary>
        /// Forces the brush to asynchronously redraw itself.
        /// </summary>
        [OpenSilver.NotImplemented]
        public void Redraw() { }

        /// <summary>
        /// Sets the source of the content for the <see cref="WebBrowserBrush"/>.
        /// </summary>
        /// <param name="source">
        /// The <see cref="WebBrowser"/> hosting the HTML content that is the source for the brush.
        /// </param>
        [OpenSilver.NotImplemented]
        public void SetSource(WebBrowser source) { }

        /// <summary>
        /// Identifies the <see cref="SourceName"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SourceNameProperty;
    }
}
