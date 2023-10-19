
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
using System.ComponentModel;

namespace System.Windows;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IUIElement : IDependencyObject
{
}

internal interface IInternalUIElement : IUIElement, IInternalDependencyObject
{
    bool IsLoaded { get; }

    bool IsConnectedToLiveTree { get; }

    bool IsVisualChildrenIterationInProgress { get; set; }

    bool HasVisualChildren { get; }

    int VisualChildrenCount { get; }

    DependencyObject VisualParent { get; set; }

    void OnVisualParentChanged(DependencyObject oldParent);

    DependencyObject GetVisualChild(int index);

    event RoutedEventHandler LostFocus;

    void AddHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo);

    void RemoveHandler(RoutedEvent routedEvent, Delegate handler);
}
