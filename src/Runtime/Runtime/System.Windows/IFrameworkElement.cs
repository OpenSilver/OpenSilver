
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.Collections;
using System.ComponentModel;
using OpenSilver.Internal;

#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IFrameworkElement : IUIElement
{
}

internal interface IInternalFrameworkElement : IFrameworkElement, IInternalUIElement
{
    event InheritedPropertyChangedEventHandler InheritedPropertyChanged;

    bool HasLogicalChildren { get; set; }

    bool IsLogicalChildrenIterationInProgress { get; set; }

    bool HasResources { get; }

    bool ShouldLookupImplicitStyles { get; set; }

    bool IsLoadedInResourceDictionary { get; set; }

    IEnumerator LogicalChildren { get; }

    RoutedEvent LoadedEvent { get; }

    DependencyObject TemplatedParent { get; set; }

    IInternalFrameworkElement TemplateChild { get; set; }

    DependencyObject Parent { get; }

    ResourceDictionary Resources { get; set; }

    event RoutedEventHandler Loaded;

    object FindName(string name);

    void OnInheritedPropertyChanged(InheritablePropertyChangeInfo info);

    void AddLogicalChild(object child);

    void RemoveLogicalChild(object child);

    void ChangeLogicalParent(DependencyObject newParent);

    void LoadResources();

    void UnloadResources();

    void RaiseLoadedEvent();

    void RaiseUnloadedEvent();

    DependencyObject AsDependencyObject();
}
