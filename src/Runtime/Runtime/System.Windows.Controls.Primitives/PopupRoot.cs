
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
using OpenSilver.Internal;

namespace System.Windows.Controls.Primitives;

internal sealed class PopupRoot : FrameworkElement
{
    private static readonly HashSet<PopupRoot> _popupRoots = new();

    private readonly Popup _popup;
    private readonly TransformLayer _transformLayer;

    static PopupRoot()
    {
        KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(PopupRoot), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
    }

    internal PopupRoot(Popup popup)
    {
        BypassLayoutPolicies = true;

        ParentWindow = GetParentWindowOfPopup(popup);
        _popup = popup;

        _transformLayer = new TransformLayer();

        SetLayoutBindings();
    }

    internal static IEnumerable<PopupRoot> GetActivePopupRoots() => _popupRoots;

    internal UIElement Child
    {
        get => _transformLayer.Child;
        set => _transformLayer.Child = value;
    }

    internal bool IsOpen { get; private set; }

    internal Popup Popup => _popup;

    internal FrameworkElement HiddenVisualParent => _transformLayer;

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

        PropagateResumeLayout(this, _transformLayer);
        INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_transformLayer, this);
        _transformLayer.UpdateIsVisible();

        SetLayoutSize();
    }

    internal void Close()
    {
        if (!_popupRoots.Remove(this))
        {
            return;
        }

        IsOpen = false;

        PropagateSuspendLayout(_transformLayer);
        INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_transformLayer, this);
        _transformLayer.UpdateIsVisible();

        INTERNAL_HtmlDomManager.RemoveNodeNative(OuterDiv);
        OuterDiv = null;
        IsConnectedToLiveTree = false;
    }

    internal void SetPosition(double x, double y) => _transformLayer.SetPosition(x, y);

    internal Matrix Transform
    {
        get => _transformLayer.Transform;
        set => _transformLayer.Transform = value;
    }

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

        return _transformLayer;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        // Note: If a popup has StayOpen=True, the value of "StayOpen" of its parents is ignored.
        // In other words, the parents of a popup that has StayOpen=True will always stay open
        // regardless of the value of their "StayOpen" property.

        var listOfPopupThatMustBeClosed = new HashSet<Popup>();
        var popupRootList = new List<PopupRoot>();

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
        _transformLayer.SetBinding(WidthProperty,
            new Binding { Path = new PropertyPath(WidthProperty), Source = _popup });
        _transformLayer.SetBinding(HeightProperty,
            new Binding { Path = new PropertyPath(HeightProperty), Source = _popup });
        _transformLayer.SetBinding(MaxHeightProperty,
            new Binding { Path = new PropertyPath(MaxHeightProperty), Source = _popup });
        _transformLayer.SetBinding(HorizontalAlignmentProperty,
            new Binding { Path = new PropertyPath(Popup.HorizontalContentAlignmentProperty), Source = _popup });
        _transformLayer.SetBinding(VerticalAlignmentProperty,
            new Binding { Path = new PropertyPath(Popup.VerticalContentAlignmentProperty), Source = _popup });
    }

    private void SetLayoutSize()
    {
        _transformLayer.InvalidateMeasure();
        _transformLayer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        _transformLayer.Arrange(new Rect(new Point(), _transformLayer.DesiredSize));
        _transformLayer.UpdateLayout();
    }

    // If the popup has a placement target, and the latter is in the visual tree,
    // we get the window from there. Otherwise, if the popup itself is inthe visual
    // tree, "Popup.ParentWindow" should be populated. Otherwise, we use the default
    // window (MainWindow) to display the popup.
    private static Window GetParentWindowOfPopup(Popup popup)
        => popup.PlacementTarget?.ParentWindow ?? popup.ParentWindow ?? Application.Current.MainWindow;

    internal sealed class TransformLayer : FrameworkElement
    {
        static TransformLayer()
        {
            RenderTransformProperty.OverrideMetadata(
                typeof(TransformLayer),
                new PropertyMetadata(null, null, CoerceRenderTransform));

            RenderTransformOriginProperty.OverrideMetadata(
                typeof(TransformLayer),
                new PropertyMetadata(new Point(0, 0), null, CoerceRenderTransformOrigin));
        }

        private readonly TransformGroup _renderTransform;
        private readonly MatrixTransform _translateTransform;
        private readonly MatrixTransform _transform;
        private UIElement _child;

        public TransformLayer()
        {
            _renderTransform = new TransformGroup();
            _renderTransform.CanBeInheritanceContext = false;
            _renderTransform.Children.CanBeInheritanceContext = false;

            _translateTransform = new MatrixTransform();
            _transform = new MatrixTransform();

            _renderTransform.Children.Add(_transform);
            _renderTransform.Children.Add(_translateTransform);

            CoerceValue(RenderTransformProperty);
        }

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
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_child, this);
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

        internal Matrix Transform
        {
            get => _transform.Matrix;
            set => _transform.Matrix = value;
        }

        internal void SetPosition(double x, double y) => _translateTransform.Matrix = Matrix.CreateTranslation(x, y);

        private new void AddVisualChild(UIElement child)
        {
            if (child is null) return;

            if (child.InternalVisualParent is not null)
            {
                throw new ArgumentException(Strings.UIElement_HasParent);
            }

            HasVisualChildren = true;

            PropagateResumeLayout(this, child);
            SynchronizeForceInheritProperties(child, this);
        }

        private new void RemoveVisualChild(UIElement child)
        {
            if (child is null) return;

            HasVisualChildren = false;

            PropagateSuspendLayout(child);
            SynchronizeForceInheritProperties(child, this);
        }

        private static object CoerceRenderTransform(DependencyObject d, object value) => ((TransformLayer)d)._renderTransform;

        private static object CoerceRenderTransformOrigin(DependencyObject d, object value) => new Point(0, 0);
    }
}