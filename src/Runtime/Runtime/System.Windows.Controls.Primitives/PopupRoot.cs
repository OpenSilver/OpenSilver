
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using CSHTML5.Internal;

namespace System.Windows.Controls.Primitives;

internal sealed class PopupRoot : FrameworkElement
{
    private static readonly HashSet<PopupRoot> _popupRoots = new();

    private readonly Popup _popup;
    private readonly PopupDecorator _decorator;

    internal PopupRoot(Popup popup)
    {
        BypassLayoutPolicies = true;

        ParentWindow = GetParentWindowOfPopup(popup);
        _popup = popup;

        _decorator = new PopupDecorator();

        SetLayoutBindings();
    }

    internal static IEnumerable<PopupRoot> GetActivePopupRoots() => _popupRoots;

    internal UIElement Child
    {
        get => _decorator.Child;
        set => _decorator.Child = value;
    }

    internal bool IsOpen { get; private set; }

    internal Popup Popup => _popup;

    internal PopupDecorator HiddenVisualParent => _decorator;

    internal void Show()
    {
        if (!_popupRoots.Add(this))
        {
            return;
        }

        IsOpen = true;

        OuterDiv = INTERNAL_HtmlDomManager.CreatePopupRootDomElementAndAppendIt(this);
        IsConnectedToLiveTree = true;
        UpdateIsVisible();

        PropagateResumeLayout(this, _decorator);
        INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_decorator, this);
        _decorator.UpdateIsVisible();

        SetLayoutSize();
    }

    internal void Close()
    {
        if (!_popupRoots.Remove(this))
        {
            return;
        }

        IsOpen = false;

        PropagateSuspendLayout(_decorator);
        INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_decorator, this);
        _decorator.UpdateIsVisible();

        INTERNAL_HtmlDomManager.RemoveNodeNative(OuterDiv);
        OuterDiv = null;
        IsConnectedToLiveTree = false;
    }

    internal void SetPosition(double x, double y) => _decorator.Margin = new Thickness(x, y, 0, 0);

    internal void PutPopupInFront()
    {
        if (OuterDiv is null) return;

        string parentDiv = OpenSilver.Interop.GetVariableStringForJS(ParentWindow.RootDomElement);
        string popupDiv = OpenSilver.Interop.GetVariableStringForJS(OuterDiv);
        OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{parentDiv}.appendChild({popupDiv})");
    }

    protected override int VisualChildrenCount => 1;

    protected override UIElement GetVisualChild(int index)
    {
        if (index != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return _decorator;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        // Note: If a popup has StayOpen=True, the value of "StayOpen" of its parents is ignored.
        // In other words, the parents of a popup that has StayOpen=True will always stay open
        // regardless of the value of their "StayOpen" property.

        HashSet<Popup> listOfPopupThatMustBeClosed = new HashSet<Popup>();
        List<PopupRoot> popupRootList = new List<PopupRoot>();

        foreach (PopupRoot root in GetActivePopupRoots())
        {
            popupRootList.Add(root);

            if (root._popup != null)
            {
                listOfPopupThatMustBeClosed.Add(root._popup);
            }
        }

        // We determine which popup needs to stay open after this click
        foreach (PopupRoot popupRoot in popupRootList)
        {
            if (popupRoot._popup != null)
            {
                // We must prevent all the parents of a popup to be closed when:
                // - this popup is set to StayOpen
                // - or the click happend in this popup

                Popup popup = popupRoot._popup;

                if (popup.StayOpen)
                {
                    do
                    {
                        if (!listOfPopupThatMustBeClosed.Contains(popup))
                            break;

                        listOfPopupThatMustBeClosed.Remove(popup);

                        popup = popup.ParentPopup;

                    } while (popup != null);
                }
            }
        }

        foreach (Popup popup in listOfPopupThatMustBeClosed)
        {
            var args = new CancelEventArgs();
            popup.OnOutsideClick(args);
            if (!args.Cancel)
            {
                popup.CloseFromAnOutsideClick();
            }
        }
    }

    private void SetLayoutBindings()
    {
        _decorator.SetBinding(WidthProperty,
            new Binding { Path = new PropertyPath(WidthProperty), Source = _popup });
        _decorator.SetBinding(HeightProperty,
            new Binding { Path = new PropertyPath(HeightProperty), Source = _popup });
        _decorator.SetBinding(MaxHeightProperty,
            new Binding { Path = new PropertyPath(MaxHeightProperty), Source = _popup });
        _decorator.SetBinding(HorizontalAlignmentProperty,
            new Binding { Path = new PropertyPath(Popup.HorizontalContentAlignmentProperty), Source = _popup });
        _decorator.SetBinding(VerticalAlignmentProperty,
            new Binding { Path = new PropertyPath(Popup.VerticalContentAlignmentProperty), Source = _popup });
    }

    private void SetLayoutSize()
    {
        _decorator.InvalidateMeasure();
        _decorator.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        _decorator.Arrange(new Rect(new Point(), _decorator.DesiredSize));
        _decorator.UpdateLayout();
    }

    // If the popup has a placement target, and the latter is in the visual tree,
    // we get the window from there. Otherwise, if the popup itself is inthe visual
    // tree, "Popup.ParentWindow" should be populated. Otherwise, we use the default
    // window (MainWindow) to display the popup.
    private static Window GetParentWindowOfPopup(Popup popup)
        => popup.PlacementTarget?.ParentWindow ?? popup.ParentWindow ?? Application.Current.MainWindow;
}

internal sealed class PopupDecorator : FrameworkElement
{
    private UIElement _child;

    public UIElement Child
    {
        get => _child;
        set
        {
            if (_child == value) return;

            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_child, this);
            RemoveVisualChild(_child);

            _child = value;

            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_child, this, 0);
            AddVisualChild(_child);

            InvalidateMeasure();
        }
    }

    protected override int VisualChildrenCount => _child is null ? 0 : 1;

    protected override UIElement GetVisualChild(int index)
    {
        if (_child is not UIElement child || index != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return child;
    }

    protected internal override void INTERNAL_OnAttachedToVisualTree()
    {
        base.INTERNAL_OnAttachedToVisualTree();
        INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(Child, this);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_child is UIElement child)
        {
            child.Measure(availableSize);
            return child.DesiredSize;
        }
        return new Size();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _child?.Arrange(new Rect(finalSize));
        return finalSize;
    }

    private new void AddVisualChild(UIElement child)
    {
        if (child == null)
        {
            return;
        }

        if (VisualTreeHelper.GetParent(child) != null)
        {
            throw new ArgumentException("Must disconnect specified child from current parent UIElement before attaching to new parent UIElement.");
        }

        HasVisualChildren = true;

        PropagateResumeLayout(this, child);

        SynchronizeForceInheritProperties(child, this);
    }

    private new void RemoveVisualChild(UIElement child)
    {
        if (child == null || VisualTreeHelper.GetParent(child) == null)
        {
            return;
        }

        if (VisualChildrenCount == 0)
        {
            HasVisualChildren = false;
        }

        PropagateSuspendLayout(child);

        SynchronizeForceInheritProperties(child, this);
    }
}
