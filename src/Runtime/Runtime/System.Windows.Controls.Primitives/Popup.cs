
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
        private const double _cursorOffsetY = 18.0;

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

            if (popup.IsOpen)
            {
                popup.UpdatePlacementTargetRegistration((UIElement)e.OldValue, (UIElement)e.NewValue);
            }
            else if (e.OldValue is not null)
            {
                UnregisterPopupFromPlacementTarget(popup, (UIElement)e.OldValue);
            }

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
                new PropertyMetadata(PlacementMode.Bottom, OnPlacementChanged),
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
                || value == PlacementMode.Right
                || value == PlacementMode.Mouse
                || value == PlacementMode.MousePoint
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

        private void UpdateTransform()
        {
            Matrix transform;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                transform = GetRelativeTransform(Window.GetWindow(this));
                transform.OffsetX = transform.OffsetY = 0;
            }
            else if (GetValue(RenderTransformProperty) is Transform popupTransform)
            {
                transform = popupTransform.Matrix;
            }
            else
            {
                transform = Matrix.Identity;
            }

            _popupRoot.SetTransform(transform);
        }

        private void UpdatePosition()
        {
            if (_popupRoot is null || !_popupRoot.IsOpen) return;

            UpdateTransform();

            Point offset;

            if (PlacementTarget is FrameworkElement target && INTERNAL_VisualTreeManager.IsElementInVisualTree(target))
            {
                offset = PerformPlacement(target);
            }
            else
            {
                PlacementMode placement = Placement;

                if (placement == PlacementMode.Mouse)
                {
                    offset = PopupService.MousePosition;
                    offset.Y += _cursorOffsetY;
                }
                else if (placement == PlacementMode.MousePoint)
                {
                    offset = PopupService.MousePosition;
                }
                else if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    offset = GetRelativeTransform(null).Transform(new Point(0, 0));
                }
                else
                {
                    offset = new Point(0, 0);
                }
            }

            _popupRoot.SetPosition(offset.X + HorizontalOffset, offset.Y + VerticalOffset);
        }

        private Point PerformPlacement(UIElement placementTarget)
        {
            var point = new Point();

            if (Child is UIElement child)
            {
                PlacementMode placement = Placement;
                var root = Application.Current.Host.Content;
                var windowBounds = new Size(root.ActualWidth, root.ActualHeight);
                InterestPoints targetInterestPoints = GetInterestPoints(placementTarget, Window.GetWindow(placementTarget));
                InterestPoints childInterestPoints = GetInterestPoints(child, _popupRoot.InternalGetVisualChild(0));

                switch (placement)
                {
                    case PlacementMode.Mouse:
                        point.X = PopupService.MousePosition.X;
                        point.Y = PopupService.MousePosition.Y + _cursorOffsetY;
                        break;
                    case PlacementMode.MousePoint:
                        point = PopupService.MousePosition;
                        break;
                    case PlacementMode.Top:
                        point.X = targetInterestPoints.TopLeft.X + childInterestPoints.TopLeft.X - childInterestPoints.BottomLeft.X;
                        point.Y = targetInterestPoints.TopLeft.Y + childInterestPoints.TopLeft.Y - childInterestPoints.BottomLeft.Y;
                        break;
                    case PlacementMode.Bottom:
                        point.X = targetInterestPoints.BottomLeft.X;
                        point.Y = targetInterestPoints.BottomLeft.Y;
                        break;
                    case PlacementMode.Left:
                        point.X = targetInterestPoints.TopLeft.X + childInterestPoints.TopLeft.X - childInterestPoints.TopRight.X;
                        point.Y = targetInterestPoints.TopLeft.Y + childInterestPoints.TopLeft.Y - childInterestPoints.TopRight.Y;
                        break;
                    case PlacementMode.Right:
                        point.X = targetInterestPoints.TopRight.X;
                        point.Y = targetInterestPoints.TopRight.Y;
                        break;
                    default:
                        throw new NotSupportedException($"PlacementMode '{placement}' is not supported");
                }

                Rect childBounds = GetBounds(childInterestPoints);
                childBounds.X += childInterestPoints.TopLeft.X + point.X;
                childBounds.Y += childInterestPoints.TopLeft.Y + point.Y;

                if (childBounds.Y + childBounds.Height > windowBounds.Height)
                {
                    if (placement == PlacementMode.Bottom)
                    {
                        point.X = targetInterestPoints.TopLeft.X + childInterestPoints.TopLeft.X - childInterestPoints.BottomLeft.X;
                        point.Y = targetInterestPoints.TopLeft.Y + childInterestPoints.TopLeft.Y - childInterestPoints.BottomLeft.Y;
                    }
                    else
                    {
                        point.Y -= childBounds.Y + childBounds.Height - windowBounds.Height;
                    }
                }
                else if (childBounds.Y < 0)
                {
                    if (placement == PlacementMode.Top)
                    {
                        point.X = targetInterestPoints.BottomLeft.X;
                        point.Y = targetInterestPoints.BottomLeft.Y;
                    }
                    else
                    {
                        point.Y -= childBounds.Y;
                    }
                }

                childBounds = GetBounds(childInterestPoints);
                childBounds.X += childInterestPoints.TopLeft.X + point.X;
                childBounds.Y += childInterestPoints.TopLeft.Y + point.Y;

                if (childBounds.X + childBounds.Width > windowBounds.Width)
                {
                    if (placement == PlacementMode.Right)
                    {
                        point.X = targetInterestPoints.TopLeft.X + childInterestPoints.TopLeft.X - childInterestPoints.TopRight.X;
                        point.Y = targetInterestPoints.TopLeft.Y + childInterestPoints.TopLeft.Y - childInterestPoints.TopRight.Y;
                    }
                    else
                    {
                        point.X -= childBounds.X + childBounds.Width - windowBounds.Width;
                    }
                }
                else if (childBounds.X < 0)
                {
                    if (placement == PlacementMode.Left)
                    {
                        point.X = targetInterestPoints.TopRight.X;
                        point.Y = targetInterestPoints.TopRight.Y;
                    }
                    else
                    {
                        point.X -= childBounds.X;
                    }
                }

                childBounds = GetBounds(childInterestPoints);
                childBounds.X += childInterestPoints.TopLeft.X + point.X;
                childBounds.Y += childInterestPoints.TopLeft.Y + point.Y;

                if (StaysWithinScreenBounds)
                {
                    if (childBounds.Y + childBounds.Height > windowBounds.Height)
                    {
                        point.Y -= childBounds.Y + childBounds.Height - windowBounds.Height;
                    }
                    else if (childBounds.Y < 0)
                    {
                        point.Y -= childBounds.Y;
                    }

                    if (childBounds.X + childBounds.Width > windowBounds.Width)
                    {
                        point.X -= childBounds.X + childBounds.Width - windowBounds.Width;
                    }
                    else if (childBounds.X < 0)
                    {
                        point.X -= childBounds.X;
                    }
                }
            }

            return point;
        }

        private struct InterestPoints
        {
            public Point TopLeft;
            public Point TopRight;
            public Point BottomLeft;
            public Point BottomRight;
        }

        private static InterestPoints GetInterestPoints(UIElement element, UIElement relativeTo)
        {
            var transform = element.TransformToVisual(relativeTo);

            return new InterestPoints
            {
                TopLeft = transform.Transform(new Point(0, 0)),
                TopRight = transform.Transform(new Point(element.RenderSize.Width, 0)),
                BottomLeft = transform.Transform(new Point(0, element.RenderSize.Height)),
                BottomRight = transform.Transform(new Point(element.RenderSize.Width, element.RenderSize.Height)),
            };
        }

        // Gets the smallest rectangle that contains all points in the list
        private static Rect GetBounds(InterestPoints interestPoints)
        {
            Point topLeft = interestPoints.TopLeft;
            Point topRight = interestPoints.TopRight;
            Point bottomLeft = interestPoints.BottomLeft;
            Point bottomRight = interestPoints.BottomRight;

            double left = Math.Min(Math.Min(topLeft.X, topRight.X), Math.Min(bottomLeft.X, bottomRight.X));
            double right = Math.Max(Math.Max(topLeft.X, topRight.X), Math.Max(bottomLeft.X, bottomRight.X));
            double top = Math.Min(Math.Min(topLeft.Y, topRight.Y), Math.Min(bottomLeft.Y, bottomRight.Y));
            double bottom = Math.Max(Math.Max(topLeft.Y, topRight.Y), Math.Max(bottomLeft.Y, bottomRight.Y));

            return new Rect(left, top, right - left, bottom - top);
        }

        private void ShowPopupRootIfNotAlreadyVisible()
        {
            _popupRoot ??= new PopupRoot(this);

            if (_popupRoot.IsOpen) return;

            UpdatePopupParent();

            _popupRoot.Child = Child;

            // When opening, set the placement target registration
            UpdatePlacementTargetRegistration(null, PlacementTarget);

            _popupRoot.Show();

            UpdatePosition();

            // Force layout update to prevent the popup content from briefly appearing in
            // the top left corner of the screen.
            UpdateLayout();
            OpenSilver.Interop.JavaScriptRuntime.Flush();
        }

        private void HidePopupRootIfVisible()
        {
            _popupRoot?.Close();

            // When closing, clear the placement target registration
            UpdatePlacementTargetRegistration(PlacementTarget, null);
        }

        /// <summary>
        /// Updates the popup's placement target registration.
        /// This method is only called when IsOpen changes or when PlacementTarget changes,
        /// When IsOpen changes, your before/after is either PlacementTarget or null. When 
        /// PlacementTarget changes, the before/after are stored in the event args.
        /// </summary>
        private void UpdatePlacementTargetRegistration(UIElement oldValue, UIElement newValue)
        {
            // A popup will be registered with its placement target to enable the descendent walker
            // to traverse into the popup. This is required for style sheet invalidations, etc.
            //
            // To avoid life-time issues, the popup will only be registered with the placement target
            // if the popup is in the Open state. Otherwise the strong-ref from the placement target
            // back to the popup could potentially keep the popup alive even though it has long
            // been closed.

            if (oldValue is not null)
            {
                UnregisterPopupFromPlacementTarget(this, oldValue);

                if (newValue is null && VisualTreeHelper.GetParent(this) is null)
                {
                    TreeWalkHelper.InvalidateOnTreeChange(this, oldValue, false);
                }
            }

            if (newValue is not null)
            {
                // Only register with PlacementTarget if we aren't in a tree
                if (VisualTreeHelper.GetParent(this) is null)
                {
                    RegisterPopupWithPlacementTarget(this, newValue);

                    // Invalidate relevant properties for this subtree
                    TreeWalkHelper.InvalidateOnTreeChange(this, newValue, true);
                }
            }
        }

        /// <summary>
        /// Registers this popup with the specified placement target. The descendant walker requires this so that
        /// it can traverse into the popup's element tree.
        /// </summary>
        private static void RegisterPopupWithPlacementTarget(Popup popup, UIElement placementTarget)
        {
            Debug.Assert(popup is not null, "Popup must be non-null");
            Debug.Assert(placementTarget is not null, "Placement target must be non-null.");

            //
            // The registered popups are stored in an array list on the specified element (which is
            // typically the placement target).
            // The array list for storing the registered popups on the placement target is lazily created.
            //

            if (placementTarget.GetValue(RegisteredPopupsField) is not List<Popup> registeredPopups)
            {
                registeredPopups = new(1);
                placementTarget.SetValue(RegisteredPopupsField, registeredPopups);
            }

            if (!registeredPopups.Contains(popup))
            {
                registeredPopups.Add(popup);
            }
        }

        /// <summary>
        /// Unregisters the popup from the spefied placement target. For more details see comments on
        /// RegisterPopupWithPlacementTarget.
        /// </summary>
        private static void UnregisterPopupFromPlacementTarget(Popup popup, UIElement placementTarget)
        {
            Debug.Assert(popup is not null, "Popup must be non-null");
            Debug.Assert(placementTarget is not null, "Placement target must be non-null.");

            if (placementTarget.GetValue(RegisteredPopupsField) is List<Popup> registeredPopups)
            {
                registeredPopups.Remove(popup);

                // If after removing this popup from the placement targets popup registration list, no more
                // popups are left, we can also get rid of the array list.
                if (registeredPopups.Count == 0)
                {
                    placementTarget.ClearValue(RegisteredPopupsField);
                }
            }
        }

        internal static readonly DependencyProperty RegisteredPopupsField =
            DependencyProperty.RegisterAttached(
                "_RegisteredPopupsField",
                typeof(List<Popup>),
                typeof(Popup),
                null);

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
        public void SetWindow(Window associatedWindow) { }

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

            UpdatePosition();
        }
    }
}
