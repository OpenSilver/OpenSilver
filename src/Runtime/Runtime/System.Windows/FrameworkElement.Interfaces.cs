
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
using System.Collections;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

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

    bool IInternalFrameworkElement.IsLoadedInResourceDictionary
    {
        get => IsLoadedInResourceDictionary;
        set => IsLoadedInResourceDictionary = value;
    }

    IEnumerator IInternalFrameworkElement.LogicalChildren => LogicalChildren;

    RoutedEvent IInternalFrameworkElement.LoadedEvent => LoadedEvent;

    DependencyObject IInternalFrameworkElement.TemplatedParent
    {
        get => TemplatedParent;
        set => TemplatedParent = value;
    }

    IInternalFrameworkElement IInternalFrameworkElement.TemplateChild
    {
        get => TemplateChild;
        set => TemplateChild = (FrameworkElement)value;
    }

    DependencyProperty IInternalFrameworkElement.DataContextProperty => DataContextProperty;

    DependencyProperty IInternalFrameworkElement.ContentPresenterContentProperty => ContentPresenter.ContentProperty;

    DependencyObject IInternalFrameworkElement.Parent => Parent;

    ResourceDictionary IInternalFrameworkElement.Resources
    {
        get => Resources;
        set => Resources = value;
    }

    event InheritedPropertyChangedEventHandler IInternalFrameworkElement.InheritedPropertyChanged
    {
        add => InheritedPropertyChanged += value;
        remove => InheritedPropertyChanged -= value;
    }

    event RoutedEventHandler IInternalFrameworkElement.Loaded
    {
        add => Loaded += value;
        remove => Loaded -= value;
    }

    void IInternalFrameworkElement.AddLogicalChild(object child) => AddLogicalChild(child);

    DependencyObject IInternalFrameworkElement.AsDependencyObject() => this;

    void IInternalFrameworkElement.ChangeLogicalParent(DependencyObject newParent) => ChangeLogicalParent(newParent);

    object IInternalFrameworkElement.FindName(string name) => FindName(name);

    void IInternalFrameworkElement.LoadResources() => LoadResources();

    void IInternalFrameworkElement.OnInheritedPropertyChanged(InheritablePropertyChangeInfo info) => OnInheritedPropertyChanged(this, info);

    void IInternalFrameworkElement.RaiseLoadedEvent() => RaiseLoadedEvent();

    void IInternalFrameworkElement.RaiseUnloadedEvent() => RaiseUnloadedEvent();

    void IInternalFrameworkElement.RemoveLogicalChild(object child) => RemoveLogicalChild(child);

    void IInternalFrameworkElement.SubscribeToSizeChanged() => SubscribeToSizeChanged();

    void IInternalFrameworkElement.UnloadResources() => UnloadResources();
}
