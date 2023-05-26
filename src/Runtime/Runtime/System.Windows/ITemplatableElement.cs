#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

public interface ITemplatableElement : IFrameworkElement
{
    void SetTemplatedParent(ITemplatableElement templatedParent);
}
