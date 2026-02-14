
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

using System.Windows.Navigation;
using CSHTML5;
using CSHTML5.Internal;
using OpenSilver;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides a control that hosts an HTML page in an app using an iframe.
    /// Note: some websites explicitly forbid being embedded in an iframe.
    /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
    /// </summary>
    public partial class WebBrowser : FrameworkElement
    {
        private HtmlElementReference _iFrame;
        private string _htmlString;
        private JavaScriptCallback _jsCallbackOnIframeLoaded;

        public WebBrowser()
        {
            Unloaded += (o, e) => DisposeJsCallbacks();
        }

        internal sealed override bool EnablePointerEventsCore => true;

        /// <inheritdoc />
        protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent)
        {
            var outerDiv = INTERNAL_HtmlDomManager.CreateDomLayoutElementAndAppendIt("div", parent, this, false);

            _iFrame = INTERNAL_HtmlDomManager.AppendDomElement("iframe", outerDiv, this);
            _iFrame.SetCssStyleProperty(CssPropertyNames.Width, "100%");
            _iFrame.SetCssStyleProperty(CssPropertyNames.Height, "100%");
            _iFrame.SetCssStyleProperty(CssPropertyNames.Border, "none");

            DisposeJsCallbacks();
            _jsCallbackOnIframeLoaded = JavaScriptCallback.Create(OnIframeLoad);
            var sLoadedCallback = OpenSilver.Interop.GetVariableStringForJS(_jsCallbackOnIframeLoaded);

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.addListener('{_iFrame.Uid}', 'load', function (e) {{ {sLoadedCallback}(); }}))");

            var source = this.SourceUri;
            if (source != null && !string.IsNullOrEmpty(source.OriginalString))
            {
                _iFrame.SetProperty("src", INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(source.OriginalString));
            }
            else if (_htmlString != null)
            {
                _iFrame.SetProperty("srcdoc", INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(_htmlString));
            }
            else
            {
                _iFrame.SetProperty("src", "about:blank");
            }

            return outerDiv;
        }

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) source of the HTML content to display in the control inside an iframe.
        /// Note: some websites explicitly forbid being embedded in an iframe.
        /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
        /// </summary>
        public Uri SourceUri
        {
            get { return (Uri)GetValue(SourceUriProperty); }
            set { SetValueInternal(SourceUriProperty, value); }
        }
        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) source of the HTML content to display in the control inside an iframe.
        /// Note: some websites explicitly forbid being embedded in an iframe.
        /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
        /// </summary>
        public static readonly DependencyProperty SourceUriProperty =
            DependencyProperty.Register(
                nameof(SourceUri),
                typeof(Uri),
                typeof(WebBrowser),
                new PropertyMetadata((object)null)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var webView = (WebBrowser)d;
                        var source = (Uri)newValue;
                        if (source != null && !string.IsNullOrEmpty(source.OriginalString))
                        {
                            string uri = INTERNAL_UriHelper.ConvertToHtml5Path(source.OriginalString, null);
                            webView._iFrame.SetAttribute("src", INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(uri));
                        }
                        else
                        {
                            webView._iFrame.SetAttribute("src", "about:blank");
                        }
                    },
                });

        /// <summary>
        /// Loads the HTML content at the specified Uniform Resource Identifier (URI) inside an iframe.
        /// Note: some websites explicitly forbid being embedded in an iframe.
        /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
        /// </summary>
        /// <param name="source">The Uniform Resource Identifier (URI) to load.</param>
        public void Navigate(Uri source)
        {
            this.SourceUri = source;
        }

        /// <summary>
        /// Displays the specified HTML content inside an iframe.
        /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
        /// </summary>
        /// <param name="text">The HTML content to display in the control.</param>
        public void NavigateToString(string text)
        {
            _htmlString = text;

            if (IsLoaded) // Note: if not loaded, we will set the HTML later when adding the control to the visual tree.
            {
                if (_htmlString != null)
                {
                    _iFrame.SetProperty("srcdoc", INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(_htmlString));
                }
                else
                {
                    _iFrame.SetProperty("src", "about:blank");
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
            if (null != LoadCompleted && IsLoaded)
            {
                Uri source;
                source = this.SourceUri;
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
