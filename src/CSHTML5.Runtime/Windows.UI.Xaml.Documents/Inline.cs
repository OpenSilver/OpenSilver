

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


#if MIGRATION
using System.Windows.Controls;
using CSHTML5.Internal;
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
    public abstract partial class Inline : TextElement
    {
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
        public new static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register("TextDecorations", 
                                                                                                            typeof(TextDecorationCollection), 
                                                                                                            typeof(Inline), 
                                                                                                            new PropertyMetadata(System.Windows.TextDecorations.None) 
                                                                                                            {
                                                                                                                GetCSSEquivalent = Control.INTERNAL_GetCSSEquivalentForTextDecorations,
                                                                                                                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
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
            DependencyProperty.Register("TextDecorations", typeof(TextDecorations?), typeof(Inline), new PropertyMetadata(null)
            {
                GetCSSEquivalent = Control.INTERNAL_GetCSSEquivalentForTextDecorations,
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            }
            );
#endif
        
        protected override void OnAfterApplyHorizontalAlignmentAndWidth()
        {
            dynamic style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(this.INTERNAL_OuterDomElement);
            style.display = "inline";
        }

        protected override void OnAfterApplyVerticalAlignmentAndWidth()
        {
            dynamic style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(this.INTERNAL_OuterDomElement);
            style.display = "inline";
        }
    }
}
