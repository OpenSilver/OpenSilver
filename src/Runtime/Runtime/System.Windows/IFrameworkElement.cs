
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

namespace System.Windows;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IFrameworkElement : IUIElement
{
    event RoutedEventHandler Loaded;

    bool IsLoaded { get; }

    DependencyObject Parent { get; }

    ResourceDictionary Resources { get; set; }

    string Name { get; set; }

    object FindName(string name);
}

internal interface IInternalFrameworkElement : IFrameworkElement, IInternalUIElement
{
    event InheritedPropertyChangedEventHandler InheritedPropertyChanged;

    bool HasLogicalChildren { get; set; }

    bool IsLogicalChildrenIterationInProgress { get; set; }

    bool HasResources { get; }

    bool ShouldLookupImplicitStyles { get; set; }

    IEnumerator LogicalChildren { get; }

    RoutedEvent LoadedEvent { get; }

    DependencyObject TemplatedParent { get; set; }

    IFrameworkElement TemplateChild { get; set; }

    void AddLogicalChild(object child);

    void RemoveLogicalChild(object child);

    void ChangeLogicalParent(DependencyObject newParent);
}
