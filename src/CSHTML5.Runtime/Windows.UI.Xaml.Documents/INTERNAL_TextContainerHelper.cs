#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    internal class INTERNAL_TextContainerHelper
    {
        public static INTERNAL_ITextContainer FromOwner(DependencyObject parent)
        {
            INTERNAL_TextContainer container = null;
            if (parent is TextBlock)
            {
                container = new INTERNAL_TextContainerTextBlock((TextBlock)parent);
            }
            else if (parent is Span)
            {
                container = new INTERNAL_TextContainerSpan((Span)parent);
            }
            return container;
        }
    }
}