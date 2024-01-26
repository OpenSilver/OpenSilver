
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

namespace System.Windows;

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
        get => VisualParent;
        set => VisualParent = value;
    }

    DependencyObject IInternalUIElement.GetVisualChild(int index) => GetVisualChild(index);

    void IInternalUIElement.OnVisualParentChanged(DependencyObject oldParent) => OnVisualParentChanged(oldParent);

    DependencyObject IInternalUIElement.AsDependencyObject() => this;
}
