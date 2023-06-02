
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

using System;

#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

public partial class UIElement : IInternalUIElement
{
    bool IInternalUIElement.IsLoaded => _isLoaded;

    bool IInternalUIElement.IsConnectedToLiveTree => IsConnectedToLiveTree;

    bool IInternalUIElement.IsVisualChildrenIterationInProgress
    {
        get => IsVisualChildrenIterationInProgress;
        set => IsVisualChildrenIterationInProgress = value;
    }

    bool IInternalUIElement.HasVisualChildren => HasVisualChildren;

    int IInternalUIElement.VisualChildrenCount => VisualChildrenCount;

    DependencyObject IInternalUIElement.VisualParent
    {
        get => INTERNAL_VisualParent;
        set => INTERNAL_VisualParent = value;
    }

    event RoutedEventHandler IInternalUIElement.LostFocus
    {
        add => LostFocus += value;
        remove => LostFocus -= value;
    }

    void IInternalUIElement.AddHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        => AddHandler(routedEvent, handler, handledEventsToo);

    void IInternalUIElement.RemoveHandler(RoutedEvent routedEvent, Delegate handler)
        => RemoveHandler(routedEvent, handler);

    DependencyObject IInternalUIElement.GetVisualChild(int index) => GetVisualChild(index);

    void IInternalUIElement.OnVisualParentChanged(DependencyObject oldParent) => OnVisualParentChanged(oldParent);
}
