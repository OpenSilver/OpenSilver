
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
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Media;
#else
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
        private JavaScriptCallback _clickCallback;

        /// <summary>
        /// Initializes a new instance of the Hyperlink class.
        /// </summary>
        public Hyperlink()
        {
            Foreground = new SolidColorBrush(Color.FromArgb(245, 26, 13, 171)); //todo: move this to a default style.
            Cursor = Cursors.Hand; //todo: move this to a default style.

#if MIGRATION
            TextDecorations = Windows.TextDecorations.Underline;
#else
            TextDecorations = Windows.UI.Text.TextDecorations.Underline;
#endif
        }

        internal override string TagName => "a";

        /// <summary>
        /// Occurs when the left mouse button is clicked on a Hyperlink.
        /// </summary>
        public event RoutedEventHandler Click;

        /// <summary>
        /// Identifies the <see cref="NavigateUri"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigateUriProperty =
            DependencyProperty.Register(
                nameof(NavigateUri),
                typeof(Uri),
                typeof(Hyperlink),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets a URI to navigate to when the Hyperlink
        /// is activated. The default is null.
        /// </summary>
        public Uri NavigateUri
        {
            get => (Uri)GetValue(NavigateUriProperty);
            set => SetValue(NavigateUriProperty, value);
        }

        public override void INTERNAL_AttachToDomEvents()
        {
            base.INTERNAL_AttachToDomEvents();

            _clickCallback = JavaScriptCallback.Create(OnClickNative, true);

            string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
            string sClickCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_clickCallback);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                $"{sDiv}.addEventListener('click', function (e) {{ {sClickCallback}(); }});");
        }

        public override void INTERNAL_DetachFromDomEvents()
        {
            base.INTERNAL_DetachFromDomEvents();

            _clickCallback?.Dispose();
            _clickCallback = null;
        }

        private void OnClickNative() => OnClick();

        internal void OnClick()
        {
            Click?.Invoke(this, new RoutedEventArgs { OriginalSource = this });

            if (NavigateUri is Uri uri)
            {
                HtmlPage.Window.Navigate(uri, "_blank");
            }
        }

        [OpenSilver.NotImplemented]
        public string TargetName { get; set; }
    }
}
