using OpenSilver.Internal;
using System.Collections;

#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

internal interface IFrameworkElement : IUIElement
{
    event RoutedEventHandler Loaded;

    event InheritedPropertyChangedEventHandler Internal_InheritedPropertyChanged;

    bool HasLogicalChildren { get; set; }

    bool IsLogicalChildrenIterationInProgress { get; set; }

    DependencyObject Parent { get; }

    IEnumerator GetLogicalChildren();

    bool IsLoadedEvent(RoutedEvent routedEvent);

    void OnInheritedPropertyChanged(InheritablePropertyChangeInfo info);

    void Internal_AddLogicalChild(object child);

    void Internal_RemoveLogicalChild(object child);

    void ChangeLogicalParent(DependencyObject newParent);

    void SubscribeToSizeChanged();

    DependencyObject GetTemplatedParent();

    DependencyProperty GetDataContextProperty();

    DependencyProperty GetContentPresenterContentProperty();

    object FindName(string name);
}
