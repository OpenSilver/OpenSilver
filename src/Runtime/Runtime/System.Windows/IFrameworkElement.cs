using OpenSilver.Internal;
using System.Collections;

#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

internal interface IFrameworkElement : IUIElement
{
    bool HasLogicalChildren { get; set; }

    bool IsLogicalChildrenIterationInProgress { get; set; }

    DependencyObject GetParent();

    IEnumerator GetLogicalChildren();

    bool ShouldRaisePropertyChanged(DependencyProperty dp);

    void OnInheritedPropertyChanged(InheritablePropertyChangeInfo info);
}
