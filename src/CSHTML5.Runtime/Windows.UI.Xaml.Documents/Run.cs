
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Represents a discrete section of formatted or unformatted text.
    /// </summary>
    [ContentProperty("Text")]
    public sealed class Run : Inline
    {
        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            dynamic span = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("span", parentRef, this);
            domElementWhereToPlaceChildren = span;
            return span;
        }

        /// <summary>
        /// Get or Set the Text property
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Run), new PropertyMetadata(string.Empty) { MethodToUpdateDom = Text_MethodToUpdateDom });

        static void Text_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var run = (Run)d;
            
            string newText = newValue as string;
            if (newText == null || newText == string.Empty)
                newText = "\u00A0"; // We put a non-breaking space here because otherwise with string.Empty the Height of the TextBlock in HTML becomes 0.
            newText = newText + "\u00A0"; // We add a space at the end of the text so that two <Run> tags do not appear to touch each other (eg. <Run>This is a</Run><Run>test.</Run>).
            INTERNAL_HtmlDomManager.SetContentString(run, newText);
        }
    }
}
