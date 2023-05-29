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

    object FindName(string name);

    public ResourceDictionary Resources { get; set; }
}

internal interface IInternalFrameworkElement : IFrameworkElement, IInternalUIElement, ITemplatableElement
{
    event InheritedPropertyChangedEventHandler InheritedPropertyChanged;

    bool HasLogicalChildren { get; set; }

    bool IsLogicalChildrenIterationInProgress { get; set; }

    IEnumerator LogicalChildren { get; }

    RoutedEvent LoadedEvent { get; }

    void OnInheritedPropertyChanged(InheritablePropertyChangeInfo info);

    void AddLogicalChild(object child);

    void RemoveLogicalChild(object child);

    void ChangeLogicalParent(DependencyObject newParent);

    void SubscribeToSizeChanged();

    DependencyObject TemplatedParent { get; set; }

    DependencyProperty DataContextProperty { get; }

    DependencyProperty ContentPresenterContentProperty { get; }

    IInternalFrameworkElement TemplateChild { set; }

    DependencyObject AsDependencyObject();

    bool HasResources { get; }

    bool ShouldLookupImplicitStyles { set; }

    bool IsLoadedInResourceDictionary { get; set; }

    void LoadResources();

    void UnloadResources();

    void RaiseLoadedEvent();

    void RaiseUnloadedEvent();
}
