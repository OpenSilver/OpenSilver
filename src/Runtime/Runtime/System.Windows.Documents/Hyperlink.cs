
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
using System.Windows.Browser;
using System.Windows.Input;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Provides an inline-level content element that provides facilities for hosting hyperlinks.
    /// </summary>
    public sealed class Hyperlink : Span
    {
        Uri _uri;

        /// <summary>
        /// Initializes a new instance of the Hyperlink class.
        /// </summary>
        public Hyperlink()
        {
            this.Foreground = new SolidColorBrush(Color.FromArgb(245, 26, 13, 171)); //todo: move this to a default style.
            this.Cursor = Cursors.Hand; //todo: move this to a default style.

#if MIGRATION
            this.MouseLeftButtonDown += Hyperlink_MouseLeftButtonDown;
            this.TextDecorations = System.Windows.TextDecorations.Underline;
#else
            this.PointerPressed += Hyperlink_PointerPressed;
            this.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
#endif
        }

        internal override string TagName => "a";

        /// <summary>
        /// Occurs when the left mouse button is clicked on a Hyperlink.
        /// </summary>
        public event RoutedEventHandler Click;

        /// <summary>
        /// Gets or sets a URI to navigate to when the Hyperlink
        /// is activated. The default is null.
        /// </summary>
        public Uri NavigateUri
        {
            get { return _uri; }
            set { _uri = value; }
        }

#if MIGRATION
        void Hyperlink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#else
        void Hyperlink_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            if (Click != null)
            {
                Click(this, new RoutedEventArgs
                {
                    OriginalSource = this
                });
            }

            if (_uri != null)
                HtmlPage.Window.Navigate(_uri, "_blank");
        }

        [OpenSilver.NotImplemented]
        public string TargetName { get; set; }
    }
}
