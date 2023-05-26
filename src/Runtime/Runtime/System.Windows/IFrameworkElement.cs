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

    DependencyObject Parent { get; }

    object FindName(string name);

    public ResourceDictionary Resources { get; set; }
}

internal interface IInternalFrameworkElement : IFrameworkElement, IInternalUIElement, ITemplatableElement
{
    event InheritedPropertyChangedEventHandler Internal_InheritedPropertyChanged;

    bool HasLogicalChildren { get; set; }

    bool IsLogicalChildrenIterationInProgress { get; set; }

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

    void SetTemplateChild(IInternalFrameworkElement templateChild);

    DependencyObject AsDependencyObject();

    bool Internal_HasResources { get; }
}
