#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

internal interface IUIElement
{
    bool Internal_IsLoaded { get; }

    bool IsVisualChildrenIterationInProgress { get; set; }

    bool GetHasVisualChildren();

    int GetVisualChildrenCount();

    DependencyObject Internal_GetVisualChild(int index);

    DependencyObject GetINTERNAL_VisualParent();
}
