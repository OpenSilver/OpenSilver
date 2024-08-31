
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Displays content on top of existing content, within the bounds of the application window.
    /// </summary>
    [ContentProperty(nameof(Child))]
    public class Popup : FrameworkElement
    {
        private static readonly List<Popup> _monitoredPopups = new();
        private static readonly EventHandler _onLayoutUpdated = new(OnLayoutUpdated);
        private static LayoutEventList.ListItem _item;

        private PopupRoot _popupRoot;
        private bool _isMonitoringPosition;

        public Popup()
        {
            PopupService.SetRootVisual();
        }

        internal Popup ParentPopup { get; private set; }

        internal PopupRoot PopupRoot => _popupRoot;

        /// <summary>
        /// Occurs when the <see cref="IsOpen"/> property changes to true.
        /// </summary>
        public event EventHandler Opened;

        private void OnOpened() => Opened?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Occurs when the <see cref="IsOpen"/> property changes to false.
        /// </summary>
        public event EventHandler Closed;

        private void OnClosed() => Closed?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Gets or Sets the UIElement that the Popup will stick to. A null value will make the Popup stay at its originally defined position.
        /// </summary>
        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValueInternal(PlacementTargetProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the <see cref="PlacementTarget"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register(
                nameof(PlacementTarget), 
                typeof(UIElement), 
                typeof(Popup), 
                new PropertyMetadata(null, OnPlacementTargetChanged));

        private static void OnPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            popup.UpdatePositionTracker();
            popup.Reposition();
        }

        /// <summary>
        /// Gets or sets the position of the Popup relative to the UIElement it is attached to.
        /// </summary>
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValueInternal(PlacementProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the <see cref="Placement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register(
                nameof(Placement), 
                typeof(PlacementMode), 
                typeof(Popup),
                new PropertyMetadata(PlacementMode.Right, OnPlacementChanged),
                IsValidPlacementMode);

        private static void OnPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            popup.UpdatePositionTracker();
            popup.Reposition();
        }

        private static bool IsValidPlacementMode(object o)
        {
            PlacementMode value = (PlacementMode)o;
            return value == PlacementMode.Bottom
                || value == PlacementMode.Mouse
                || value == PlacementMode.Right
                || value == PlacementMode.Left
                || value == PlacementMode.Top;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new PopupRootAutomationPeer(this);

        /// <summary>
        /// Gets an enumerator that you can use to access the logical child elements of the
        /// <see cref="Popup"/> control.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> that you can use to access the logical child elements 
        /// of a <see cref="Popup"/> control. The default is null.
        /// </returns>
        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                if (Child is not UIElement content)
                {
                    return EmptyEnumerator.Instance;
                }

                return new PopupModelTreeEnumerator(this, content);
            }
        }

        private sealed class PopupModelTreeEnumerator : ModelTreeEnumerator
        {
            internal PopupModelTreeEnumerator(Popup popup, object child)
                : base(child)
            {
                Debug.Assert(popup != null, "popup should be non-null.");
                Debug.Assert(child != null, "child should be non-null.");

                _popup = popup;
            }

            protected override bool IsUnchanged => ReferenceEquals(Content, _popup.Child);

            private readonly Popup _popup;
        }

        #region Dependency Properties

        //-----------------------
        // CHILD
        //-----------------------

        /// <summary>
        ///  Gets or sets the content to be hosted in the popup.
        /// </summary>
        public UIElement Child
        {
            get { return (UIElement)GetValue(ChildProperty); }
            set { SetValueInternal(ChildProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the Child dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register(
                nameof(Child), 
                typeof(UIElement), 
                typeof(Popup), 
                new PropertyMetadata(null, OnChildChanged));

        private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            var newContent = (UIElement)e.NewValue;
            var oldContent = (UIElement)e.OldValue;
            if (oldContent != null)
            {
                popup.RemoveLogicalChild(oldContent);
            }
            if (newContent != null)
            {
                popup.AddLogicalChild(newContent);
            }

            if (popup._popupRoot != null)
            {
                popup._popupRoot.Child = newContent;
            }

            popup.Reposition();
        }

        //-----------------------
        // ISOPEN
        //-----------------------

        /// <summary>
        /// Gets or sets whether the popup is currently displayed on the screen.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValueInternal(IsOpenProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the IsOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                nameof(IsOpen), 
                typeof(bool), 
                typeof(Popup), 
                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsOpenChanged));

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            bool isOpen = (bool)e.NewValue;

            popup.UpdatePositionTracker();

            if (isOpen)
            {
                popup.ShowPopupRootIfNotAlreadyVisible();
                popup.OnOpened();

                // The popup can be closed during during the Opened event
                if (popup.IsOpen)
                {
                    popup.Unloaded += new RoutedEventHandler(CloseOnUnloaded);
                    popup.IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
                }
            }
            else
            {
                popup.OnClosed();
                popup.HidePopupRootIfVisible();
                popup.Unloaded -= new RoutedEventHandler(CloseOnUnloaded);
                popup.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
            }
        }

        private static void CloseOnUnloaded(object sender, RoutedEventArgs e)
            => ((Popup)sender).SetCurrentValue(IsOpenProperty, BooleanBoxes.FalseBox);

        private static void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Popup popup = (Popup)sender;
            popup.UpdatePositionTracker();

            if (popup._popupRoot is PopupRoot popupRoot)
            {
                bool isVisible = (bool)e.NewValue;
                if (isVisible)
                {
                    popupRoot.Visibility = Visibility.Visible;
                    popup.Reposition();
                }
                else
                {
                    popupRoot.Visibility = Visibility.Collapsed;
                }
            }
        }

        //-----------------------
        // HORIZONTALOFFSET
        //-----------------------

        /// <summary>
        /// Gets or sets the distance between the left side of the application window and the left side of the popup.
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValueInternal(HorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the HorizontalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                nameof(HorizontalOffset), 
                typeof(double), 
                typeof(Popup),
                new PropertyMetadata(0d, OnOffsetChanged));

        //-----------------------
        // VERTICALOFFSET
        //-----------------------

        /// <summary>
        /// Gets or sets the distance between the top of the application window and the top of the popup.
        /// </summary>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValueInternal(VerticalOffsetProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the VerticalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(
                nameof(VerticalOffset), 
                typeof(double), 
                typeof(Popup),
                new PropertyMetadata(0d, OnOffsetChanged));

        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Popup)d).Reposition();
        }

        //-----------------------
        // HORIZONTALCONTENTALIGNMENT (This is specific to CSHTML5 and is very useful for having full-screen popups such as ChildWindows)
        //-----------------------

        /// <summary>
        /// Gets or sets the horizontal alignment of the control's content.
        /// </summary>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValueInternal(HorizontalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HorizontalContentAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(HorizontalContentAlignment), 
                typeof(HorizontalAlignment), 
                typeof(Popup), 
                new PropertyMetadata(HorizontalAlignment.Left));

        //-----------------------
        // VERTICALCONTENTALIGNMENT (This is specific to CSHTML5 and is very useful for having full-screen popups such as ChildWindows)
        //-----------------------

        /// <summary>
        /// Gets or sets the vertical alignment of the control's content.
        /// </summary>
        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValueInternal(VerticalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="VerticalContentAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(VerticalContentAlignment), 
                typeof(VerticalAlignment), 
                typeof(Popup),
                new PropertyMetadata(VerticalAlignment.Top));

        /// <summary>
        /// Get or sets a boolean stating whether the popup should stay within the screen boundaries or not.
        /// </summary>
        public bool StaysWithinScreenBounds
        {
            get { return (bool)GetValue(StaysWithinScreenBoundsProperty); }
            set { SetValueInternal(StaysWithinScreenBoundsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="StaysWithinScreenBounds"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StaysWithinScreenBoundsProperty =
            DependencyProperty.Register(
                nameof(StaysWithinScreenBounds), 
                typeof(bool), 
                typeof(Popup), 
                new PropertyMetadata(BooleanBoxes.FalseBox));

        #endregion

        /// <summary>
        /// Reposition the Popup
        /// </summary>
        internal void Reposition()
        {
            if (IsOpen)
            {
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            if (_popupRoot is null || !_popupRoot.IsOpen)
                return;

            if (PlacementTarget is FrameworkElement target && INTERNAL_VisualTreeManager.IsElementInVisualTree(target))
            {
                Rect targetBounds = new Rect(0, 0, 0, 0);
                if (Placement != PlacementMode.Mouse)
                {
                    try
                    {
                        targetBounds = target
                            .TransformToVisual(Window.GetWindow(target))
                            .TransformBounds(new Rect(target.RenderSize));
                    }
                    catch { }
                }

                PerformPlacement(targetBounds);
            }
            else
            {
                Point point;
                if (Placement == PlacementMode.Mouse)
                {
                    point = PopupService.MousePosition;
                }
                else if (VisualTreeHelper.GetParent(this) != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    point = TransformToVisual(null).Transform(new Point(0, 0));
                }
                else
                {
                    point = new Point(0, 0);
                }

                _popupRoot.SetPosition(point.X + HorizontalOffset, point.Y + VerticalOffset);
            }
        }

        private void PerformPlacement(Rect targetBounds)
        {
            if (_popupRoot is null || !_popupRoot.IsOpen)
                return;

            var mode = Placement;
            var root = Application.Current.Host.Content;
            if (root == null)
                return;
            if (Child is not UIElement child)
                return;

            var bounds = new Point(root.ActualWidth, root.ActualHeight);
            var childSize = child.RenderSize;

            Point point = mode == PlacementMode.Mouse ?
                PopupService.MousePosition :
                new Point(targetBounds.Left, targetBounds.Top);

            switch (mode)
            {
                case PlacementMode.Top:
                    point.Y = targetBounds.Top - childSize.Height;
                    break;
                case PlacementMode.Bottom:
                    point.Y = targetBounds.Bottom;
                    break;
                case PlacementMode.Left:
                    point.X = targetBounds.Left - childSize.Width;
                    break;
                case PlacementMode.Right:
                    point.X = targetBounds.Right;
                    break;
                case PlacementMode.Mouse:
                    point.Y += 11.0;
                    break;
                default:
                    throw new NotSupportedException($"PlacementMode '{mode}' is not supported");
            }

            if ((point.Y + childSize.Height) > bounds.Y)
            {
                if (mode == PlacementMode.Bottom)
                    point.Y = targetBounds.Top - childSize.Height;
                else
                    point.Y = bounds.Y - childSize.Height;
            }
            else if (point.Y < 0)
            {
                if (mode == PlacementMode.Top)
                    point.Y = targetBounds.Bottom;
                else
                    point.Y = 0;
            }

            if ((point.X + childSize.Width) > bounds.X)
            {
                if (mode == PlacementMode.Right)
                    point.X = targetBounds.Left - childSize.Width;
                else
                    point.X = bounds.X - childSize.Width;
            }
            else if (point.X < 0)
            {
                if (mode == PlacementMode.Left)
                    point.X = targetBounds.Right;
                else
                    point.X = 0;
            }

            if (StaysWithinScreenBounds)
            {
                if ((point.Y + childSize.Height) > bounds.Y)
                {
                    point.Y = bounds.Y - childSize.Height;
                }
                else if (point.Y < 0)
                {
                    point.Y = 0;
                }

                if ((point.X + childSize.Width) > bounds.X)
                {
                    point.X = bounds.X - childSize.Width;
                }
                else if (point.X < 0)
                {
                    point.X = 0;
                }
            }

            _popupRoot.SetPosition(point.X + HorizontalOffset, point.Y + VerticalOffset);
        }

        private void ShowPopupRootIfNotAlreadyVisible()
        {
            _popupRoot ??= new PopupRoot(this);

            if (_popupRoot.IsOpen) return;

            UpdatePopupParent();

            _popupRoot.Child = Child;
            _popupRoot.Show();

            UpdatePosition();

            // Force layout update to prevent the popup content from briefly appearing in
            // the top left corner of the screen.
            UpdateLayout();
            OpenSilver.Interop.JavaScriptRuntime.Flush();
        }

        private void HidePopupRootIfVisible() => _popupRoot?.Close();

        public event EventHandler ClosedDueToOutsideClick;

        internal void CloseFromAnOutsideClick()
        {
            ClosedDueToOutsideClick?.Invoke(this, EventArgs.Empty);

            if (IsOpen)
            {
                SetCurrentValue(IsOpenProperty, BooleanBoxes.FalseBox);
            }
        }

        internal event EventHandler<CancelEventArgs> OutsideClick;

        internal void OnOutsideClick(CancelEventArgs args) => OutsideClick?.Invoke(this, args);

        public bool StayOpen { get; set; } = true;

        private void UpdatePopupParent()
        {
            UIElement element = PlacementTarget ?? VisualTreeHelper.GetParent(this) as UIElement;

            if (element == null)
            {
                ParentPopup = null;
                return;
            }

            while (true)
            {
                if (VisualTreeHelper.GetParent(element) is not UIElement parent)
                {
                    break;
                }

                element = parent;
            }

            if (element is FrameworkElement fe && fe.Parent is Popup popup)
            {
                ParentPopup = popup;
            }
        }

        internal void PutPopupInFront()
        {
            if (_popupRoot is PopupRoot popupRoot && popupRoot.IsOpen)
            {
                popupRoot.PutPopupInFront();
            }
        }
        
        [OpenSilver.NotImplemented]
        public void SetWindow(Window associatedWindow)
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void EnsurePopupStaysWithinScreenBounds(double forcedWidth = double.NaN, double forcedHeight = double.NaN)
        {
            StaysWithinScreenBounds = true;
            Reposition();
        }

        private static void StartMonitoringPosition(Popup popup)
        {
            if (!popup._isMonitoringPosition)
            {
                popup._isMonitoringPosition = true;
                _monitoredPopups.Add(popup);

                _item ??= LayoutManager.Current.LayoutEvents.Add(_onLayoutUpdated);
            }
        }

        private static void StopMonitoringPosition(Popup popup)
        {
            if (popup._isMonitoringPosition)
            {
                popup._isMonitoringPosition = false;
                _monitoredPopups.Remove(popup);

                if (_monitoredPopups.Count == 0 && _item != null)
                {
                    LayoutManager.Current.LayoutEvents.Remove(_item);
                    _item = null;
                }
            }
        }

        private static void OnLayoutUpdated(object sender, EventArgs e)
        {
            foreach (Popup popup in _monitoredPopups.ToArray())
            {
                popup.RepositionOnLayoutUpdated();
            }
        }

        private void UpdatePositionTracker()
        {
            if (!IsOpen ||
                Placement == PlacementMode.Mouse ||
                PlacementTarget is not UIElement placementTarget ||
                !INTERNAL_VisualTreeManager.IsElementInVisualTree(placementTarget))
            {
                StopMonitoringPosition(this);
            }
            else
            {
                StartMonitoringPosition(this);
            }
        }

        private void RepositionOnLayoutUpdated()
        {
            UIElement target = PlacementTarget;

            if (target is null || !INTERNAL_VisualTreeManager.IsElementInVisualTree(target))
            {
                SetCurrentValue(IsOpenProperty, BooleanBoxes.FalseBox);
                return;
            }

            Point position = target.TransformToVisual(Window.GetWindow(target)).Transform(new Point(0, 0));
            Size size = target.GetBoundingClientSize();
            Rect bounds = new(position, size);

            PerformPlacement(bounds);
        }
    }
}
