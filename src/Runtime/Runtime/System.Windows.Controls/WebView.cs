

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


using CSHTML5;
using CSHTML5.Internal;
using System;
#if MIGRATION
using System.Windows.Navigation;
#else
using Windows.UI.Xaml.Navigation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides a control that hosts an HTML page in an app using an iframe.
    /// Note: some websites explicitly forbid being embedded in an iframe.
    /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
    /// </summary>
#if MIGRATION
    public partial class WebBrowser : FrameworkElement
#else
    public partial class WebView : FrameworkElement
#endif
    {
        private object _iFrame;
        private string _htmlString;
        private JavascriptCallback _jsCallbackOnIframeLoaded;

#if MIGRATION
        public WebBrowser()
#else
        public WebView()
#endif
        {
            Unloaded += (o, e) => DisposeJsCallbacks();
        }

        internal override bool EnablePointerEventsCore
        {
            get { return true; }
        }        

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            object outerDiv;
            var outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out outerDiv);
            outerDivStyle.width = "100%";
            outerDivStyle.height = "100%";

            var iFrameStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("iframe", outerDiv, this, out _iFrame);
            iFrameStyle.width = "100%";
            iFrameStyle.height = "100%";
            iFrameStyle.border = "none";

            DisposeJsCallbacks();
            _jsCallbackOnIframeLoaded = JavascriptCallback.Create((Action)OnIframeLoad);
            OpenSilver.Interop.ExecuteJavaScriptAsync("$0.onload = $1", _iFrame, _jsCallbackOnIframeLoaded);

#if MIGRATION
            var source = this.SourceUri;
#else
            var source = this.Source;
#endif
            if (source != null && !string.IsNullOrEmpty(source.OriginalString))
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync("$0.src = $1", _iFrame, source.OriginalString);
            }
            else if (_htmlString != null)
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync("srcDoc.set($0, $1)", _iFrame, _htmlString);
            }
            else
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync("$0.src = 'about:blank'", _iFrame);
            }

            domElementWhereToPlaceChildren = _iFrame;
            return outerDiv;
        }

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) source of the HTML content to display in the control inside an iframe.
        /// Note: some websites explicitly forbid being embedded in an iframe.
        /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
        /// </summary>
#if MIGRATION
        public Uri SourceUri
        {
            get { return (Uri)GetValue(SourceUriProperty); }
            set { SetValue(SourceUriProperty, value); }
        }
#else
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
#endif
        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) source of the HTML content to display in the control inside an iframe.
        /// Note: some websites explicitly forbid being embedded in an iframe.
        /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
        /// </summary>
#if MIGRATION
        public static readonly DependencyProperty SourceUriProperty =
            DependencyProperty.Register("SourceUri", typeof(Uri), typeof(WebBrowser), new PropertyMetadata(null, Source_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
#else
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(WebView), new PropertyMetadata(null, Source_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
#endif

        private static void Source_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if MIGRATION
            WebBrowser webView = (WebBrowser)d;
#else
            WebView webView = (WebView)d;
#endif

            if (webView._isLoaded) // Note: if not loaded, we will set the source later when adding the control to the visual tree.
            {
#if MIGRATION
                var source = webView.SourceUri;
#else
                var source = webView.Source;
#endif
                if (source != null && !string.IsNullOrEmpty(source.OriginalString))
                {
                    string uri = INTERNAL_UriHelper.ConvertToHtml5Path(source.OriginalString, null);
                    CSHTML5.Interop.ExecuteJavaScriptAsync("$0.src = $1", webView._iFrame, uri);
                }
                else
                {
                    CSHTML5.Interop.ExecuteJavaScriptAsync("$0.src = 'about:blank'", webView._iFrame);
                }
            }
        }

        /// <summary>
        /// Loads the HTML content at the specified Uniform Resource Identifier (URI) inside an iframe.
        /// Note: some websites explicitly forbid being embedded in an iframe.
        /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
        /// </summary>
        /// <param name="source">The Uniform Resource Identifier (URI) to load.</param>
        public void Navigate(Uri source)
        {
#if MIGRATION
            this.SourceUri = source;
#else
            this.Source = source;
#endif
        }

        /// <summary>
        /// Displays the specified HTML content inside an iframe.
        /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
        /// </summary>
        /// <param name="text">The HTML content to display in the control.</param>
        public void NavigateToString(string text)
        {
            _htmlString = text;

            if (this._isLoaded) // Note: if not loaded, we will set the HTML later when adding the control to the visual tree.
            {
                if (_htmlString != null)
                {
                    CSHTML5.Interop.ExecuteJavaScriptAsync("srcDoc.set($0, $1)", _iFrame, _htmlString);
                }
                else
                {
                    CSHTML5.Interop.ExecuteJavaScriptAsync("$0.src = 'about:blank'", _iFrame);
                }
            }
        }

        /// <summary>
        /// Occurs when top-level navigation completes and the content loads into the WebBrowser control or when an error occurs during loading.
        /// </summary>
        public event LoadCompletedEventHandler LoadCompleted;

        /// <summary>
        /// Called from JavaScript when the iframe loads
        /// </summary>
        private void OnIframeLoad()
        {
            if (null != LoadCompleted && this._isLoaded)
            {
                Uri source;
#if MIGRATION
                source = this.SourceUri;
#else
                source = this.Source;
#endif
                LoadCompleted(this, new NavigationEventArgs(null, source));
            }
        }

        private void DisposeJsCallbacks()
        {
            _jsCallbackOnIframeLoaded?.Dispose();
            _jsCallbackOnIframeLoaded = null;
        }
    }
}
