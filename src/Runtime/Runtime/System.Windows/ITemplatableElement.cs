#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

public interface ITemplatableElement : IUIElement
{
    void SetTemplatedParent(ITemplatableElement templatedParent);
}
