

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
    public sealed partial class Run : Inline
    {
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
            DependencyProperty.Register(
                nameof(Text), 
                typeof(string), 
                typeof(Run),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextPropertyChanged));

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Run run = (Run)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(run))
            {
                INTERNAL_HtmlDomManager.SetContentString(run, (string)e.NewValue);
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var span = INTERNAL_HtmlDomManager.CreateRunDomElementAndAppendIt(parentRef, this);
            var style = CSHTML5.Internal.INTERNAL_HtmlDomManager.GetDomElementStyleForModification(span);
            // Set overflow for scrolling
            style.wordBreak = "inherit";            
            domElementWhereToPlaceChildren = span;
            return span;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            INTERNAL_HtmlDomManager.SetContentString(this, this.Text);
        }
    }
}
