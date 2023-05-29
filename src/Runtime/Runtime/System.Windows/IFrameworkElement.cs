using OpenSilver.Internal;
using System.Collections;

#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

public interface IFrameworkElement : IUIElement
{
    event RoutedEventHandler Loaded;

    bool IsLoaded { get; }

    DependencyObject Parent { get; }

    ResourceDictionary Resources { get; set; }

    object FindName(string name);
}

internal interface IInternalFrameworkElement : IFrameworkElement, IInternalUIElement
{
    event InheritedPropertyChangedEventHandler InheritedPropertyChanged;

    bool HasLogicalChildren { get; set; }

    bool IsLogicalChildrenIterationInProgress { get; set; }

    bool HasResources { get; }

    bool ShouldLookupImplicitStyles { set; }

    bool IsLoadedInResourceDictionary { get; set; }

    IEnumerator LogicalChildren { get; }

    RoutedEvent LoadedEvent { get; }

    DependencyObject TemplatedParent { get; set; }

    IInternalFrameworkElement TemplateChild { set; }

    DependencyProperty DataContextProperty { get; }

    DependencyProperty ContentPresenterContentProperty { get; }

    void OnInheritedPropertyChanged(InheritablePropertyChangeInfo info);

    void AddLogicalChild(object child);

    void RemoveLogicalChild(object child);

    void ChangeLogicalParent(DependencyObject newParent);

    void SubscribeToSizeChanged();

    void LoadResources();

    void UnloadResources();

    void RaiseLoadedEvent();

    void RaiseUnloadedEvent();

    DependencyObject AsDependencyObject();
}
