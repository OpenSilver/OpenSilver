
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



#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
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
        // Defining an implicit conversion from string to Inline allows to
        // support the following usage: TextBlock1.Inlines.Add("test");
        public static implicit operator Inline(string s)
        {
            return new Run() { Text = s };
        }

        /// <summary>
        /// Gets or sets the text decorations (underline, strikethrough...).
        /// </summary>
        public TextDecorations? TextDecorations
        {
            get { return (TextDecorations?)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }
        /// <summary>
        /// Identifies the TextDecorations dependency property.
        /// </summary>
        public static readonly DependencyProperty TextDecorationsProperty =
            DependencyProperty.Register("TextDecorations", typeof(TextDecorations?), typeof(Inline), new PropertyMetadata(null)
            {
                GetCSSEquivalent = TextBlock.INTERNAL_GetCSSEquivalentForTextDecorations
            }
            );
    }
}
