
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
using OpenSilver.Internal;

namespace System.Windows;

public partial class FrameworkElement : IInternalFrameworkElement
{
    bool IInternalFrameworkElement.HasLogicalChildren
    {
        get => HasLogicalChildren;
        set => HasLogicalChildren = value;
    }

    bool IInternalFrameworkElement.IsLogicalChildrenIterationInProgress
    {
        get => IsLogicalChildrenIterationInProgress;
        set => IsLogicalChildrenIterationInProgress = value;
    }

    bool IInternalFrameworkElement.HasResources => HasResources;

    bool IInternalFrameworkElement.ShouldLookupImplicitStyles
    {
        get => ShouldLookupImplicitStyles;
        set => ShouldLookupImplicitStyles = value;
    }

    IEnumerator IInternalFrameworkElement.LogicalChildren => LogicalChildren;

    RoutedEvent IInternalFrameworkElement.LoadedEvent => LoadedEvent;

    DependencyObject IInternalFrameworkElement.TemplatedParent
    {
        get => TemplatedParent;
        set => TemplatedParent = value;
    }

    IFrameworkElement IInternalFrameworkElement.TemplateChild
    {
        get => TemplateChild;
        set => TemplateChild = (FrameworkElement)value;
    }

    event InheritedPropertyChangedEventHandler IInternalFrameworkElement.InheritedPropertyChanged
    {
        add => InheritedPropertyChanged += value;
        remove => InheritedPropertyChanged -= value;
    }

    void IInternalFrameworkElement.AddLogicalChild(object child) => AddLogicalChild(child);

    void IInternalFrameworkElement.ChangeLogicalParent(DependencyObject newParent) => ChangeLogicalParent(newParent);

    void IInternalFrameworkElement.RemoveLogicalChild(object child) => RemoveLogicalChild(child);
}
