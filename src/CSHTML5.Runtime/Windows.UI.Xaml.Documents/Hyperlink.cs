
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


using CSHTML5.Internal;
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
        void Hyperlink_MouseLeftButtonDown(object sender, Input.MouseButtonEventArgs e)
#else
        void Hyperlink_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            if (Click != null)
                Click(this, new RoutedEventArgs());

            if (_uri != null)
                HtmlPage.Window.Navigate(_uri, "_blank");
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            dynamic a = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("a", parentRef, this);
            domElementWhereToPlaceChildren = a;
            return a;
        }
    }
}
