﻿
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

#if !MIGRATION
using Windows.UI.Text;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Provides a base class for inline text elements, such as Span and Run.
    /// </summary>
    public abstract class Inline : TextElement
    {
        internal override bool EnablePointerEventsCore
        {
            get
            {
                return true;
            }
        }

        // Defining an implicit conversion from string to Inline allows to
        // support the following usage: TextBlock1.Inlines.Add("test");
        public static implicit operator Inline(string s)
        {
            return new Run() { Text = s };
        }

#if MIGRATION
        /// <summary>
        /// Gets or sets the text decorations (underline, strikethrough...).
        /// </summary>
        public new TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }

        /// <summary>
        /// Identifies the TextDecorations dependency property.
        /// </summary>
        public new static readonly DependencyProperty TextDecorationsProperty = 
            DependencyProperty.Register(
                nameof(TextDecorations), 
                typeof(TextDecorationCollection), 
                typeof(Inline), 
                new PropertyMetadata((object)null) 
                {
                    MethodToUpdateDom = static (d, newValue) =>
                    {
                        var domStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(((Inline)d).INTERNAL_OuterDomElement);
                        domStyle.textDecoration = ((TextDecorationCollection)newValue)?.ToHtmlString() ?? string.Empty;
                    },
                });
#else
                /// <summary>
        /// Gets or sets the text decorations (underline, strikethrough...).
        /// </summary>
        public new TextDecorations? TextDecorations
        {
            get { return (TextDecorations?)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }
        /// <summary>
        /// Identifies the TextDecorations dependency property.
        /// </summary>
        public new static readonly DependencyProperty TextDecorationsProperty =
            DependencyProperty.Register(
                nameof(TextDecorations),
                typeof(TextDecorations?),
                typeof(Inline),
                new PropertyMetadata(null)
                {
                    MethodToUpdateDom = static (d, newValue) =>
                    {
                        var domStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(((Inline)d).INTERNAL_OuterDomElement);
                        var newTextDecoration = (TextDecorations?)newValue;
                        if (newTextDecoration.HasValue)
                        {
                            domStyle.textDecoration = newTextDecoration switch
                            {
                                Text.TextDecorations.OverLine => "overline",
                                Text.TextDecorations.Strikethrough => "line-through",
                                Text.TextDecorations.Underline => "underline",
                                _ => "",
                            };
                        }
                        else
                        {
                            domStyle.textDecoration = "";
                        }
                    },
                });
#endif
        
        protected override void OnAfterApplyHorizontalAlignmentAndWidth()
        {
            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(this.INTERNAL_OuterDomElement);
            style.display = "inline";
        }

        protected override void OnAfterApplyVerticalAlignmentAndWidth()
        {
            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(this.INTERNAL_OuterDomElement);
            style.display = "inline";
        }
    }
}
