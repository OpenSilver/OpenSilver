#if MIGRATION
namespace System.Windows;
#else
using System;

namespace Windows.UI.Xaml;
#endif

public interface IDependencyObject
{
    object GetValue(DependencyProperty dependencyProperty);

    void SetValue(DependencyProperty dp, object value);
}

public interface IUIElement : IDependencyObject
{
    event RoutedEventHandler LostFocus;

    void AddHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo);

    void RemoveHandler(RoutedEvent routedEvent, Delegate handler);
}

internal interface IInternalUIElement : IUIElement
{
    bool _isLoaded { get; }

    bool IsConnectedToLiveTree { get; }

    bool IsVisualChildrenIterationInProgress { get; set; }

    bool HasVisualChildren { get; }

    int VisualChildrenCount { get; }

    DependencyObject GetVisualChild(int index);

    DependencyObject INTERNAL_VisualParent { get; set; }

    void OnVisualParentChanged(DependencyObject oldParent);
}
