
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


using CSHTML5;
using CSHTML5.Internal;
using System;

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
    public class WebBrowser : FrameworkElement
#else
    public class WebView : FrameworkElement
#endif
    {
        object _iFrame;
        string _htmlString;

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

            var source = this.Source;
            if (source != null && !string.IsNullOrEmpty(source.OriginalString))
            {
                Interop.ExecuteJavaScriptAsync("$0.src = $1", _iFrame, source.OriginalString);
            }
            else if (_htmlString != null)
            {
                Interop.ExecuteJavaScriptAsync("srcDoc.set($0, $1)", _iFrame, _htmlString);
            }
            else
            {
                Interop.ExecuteJavaScriptAsync("$0.src = 'about:blank'", _iFrame);
            }

            domElementWhereToPlaceChildren = _iFrame;
            return outerDiv;
        }

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) source of the HTML content to display in the control inside an iframe.
        /// Note: some websites explicitly forbid being embedded in an iframe.
        /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
        /// </summary>
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) source of the HTML content to display in the control inside an iframe.
        /// Note: some websites explicitly forbid being embedded in an iframe.
        /// Note: to embed a piece of HTML code without using an iframe, use the HtmlPresenter control instead.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
#if MIGRATION
            DependencyProperty.Register("Source", typeof(Uri), typeof(WebBrowser), new PropertyMetadata(null, Source_Changed));
#else
            DependencyProperty.Register("Source", typeof(Uri), typeof(WebView), new PropertyMetadata(null, Source_Changed));
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
                var source = webView.Source;
                if (source != null && !string.IsNullOrEmpty(source.OriginalString))
                {
                    string uri = INTERNAL_UriHelper.ConvertToHtml5Path(source.OriginalString, null);
                    Interop.ExecuteJavaScriptAsync("$0.src = $1", webView._iFrame, uri);
                }
                else
                {
                    Interop.ExecuteJavaScriptAsync("$0.src = 'about:blank'", webView._iFrame);
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
            this.Source = source;
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
                    Interop.ExecuteJavaScriptAsync("srcDoc.set($0, $1)", _iFrame, _htmlString);
                }
                else
                {
                    Interop.ExecuteJavaScriptAsync("$0.src = 'about:blank'", _iFrame);
                }
            }
        }
    }
}
