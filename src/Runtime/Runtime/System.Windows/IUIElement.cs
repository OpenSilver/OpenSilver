#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

internal interface IDependencyObject
{
    object GetValue(DependencyProperty dependencyProperty);

    void SetValue(DependencyProperty dp, object value);

    void AddHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo);

    void RemoveHandler(RoutedEvent routedEvent, Delegate handler);
}

internal interface IUIElement : IDependencyObject
{
    bool Internal_IsLoaded { get; }

    bool IsVisualChildrenIterationInProgress { get; set; }

    bool GetHasVisualChildren();

    int GetVisualChildrenCount();

    DependencyObject Internal_GetVisualChild(int index);

    DependencyObject GetINTERNAL_VisualParent();

    void SetVisualParent(DependencyObject visualParent);

    void Internal_OnVisualParentChanged(DependencyObject oldParent);
}
