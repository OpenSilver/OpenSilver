
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

using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="Popup"/> class.
        /// </summary>
        public Popup() { }

        internal Popup ParentPopup { get; private set; }

        internal PopupRoot PopupRoot => _popupRoot;

        /// <summary>
        /// Occurs when the <see cref="IsOpen"/> property changes to true.
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// Responds to the condition in which the value of the <see cref="IsOpen"/> property changes from false to true.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnOpened(EventArgs e) => Opened?.Invoke(this, e);

        /// <summary>
        /// Occurs when the <see cref="IsOpen"/> property changes to false.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Responds when the value of the <see cref="IsOpen"/> property changes from to true to false.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
        protected virtual void OnClosed(EventArgs e) => Closed?.Invoke(this, e);

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
            return value == PlacementMode.Absolute
                || value == PlacementMode.Relative
                || value == PlacementMode.Bottom
                || value == PlacementMode.Center
                || value == PlacementMode.Right
                || value == PlacementMode.Mouse
                || value == PlacementMode.MousePoint
                || value == PlacementMode.Left
                || value == PlacementMode.Top;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether a <see cref="Popup"/> control can contain 
        /// transparent content.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Popup"/> control can contain transparent content; otherwise, false.
        /// The default is false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool AllowsTransparency
        {
            get => (bool)GetValue(AllowsTransparencyProperty);
            set => SetValueInternal(AllowsTransparencyProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AllowsTransparency"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty AllowsTransparencyProperty =
            Window.AllowsTransparencyProperty.AddOwner(
                typeof(Popup),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, OnAllowsTransparencyChanged));

        private static void OnAllowsTransparencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(PopupAnimationProperty);
        }

        /// <summary>
        /// Gets or sets an animation for the opening and closing of a <see cref="Popup"/> control.
        /// </summary>
        /// <returns>
        /// The <see cref="Primitives.PopupAnimation"/> enumeration value that defines an animation to open 
        /// and close a <see cref="Popup"/> control. The default is <see cref="PopupAnimation.None"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public PopupAnimation PopupAnimation
        {
            get => (PopupAnimation)GetValue(PopupAnimationProperty);
            set => SetValueInternal(PopupAnimationProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PopupAnimation"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PopupAnimationProperty =
            DependencyProperty.Register(
                nameof(PopupAnimation),
                typeof(PopupAnimation),
                typeof(Popup),
                new FrameworkPropertyMetadata(PopupAnimation.None, null, CoercePopupAnimation),
                IsValidPopupAnimation);

        private static object CoercePopupAnimation(DependencyObject o, object value)
        {
            return ((Popup)o).AllowsTransparency ? value : PopupAnimation.None;
        }

        private static bool IsValidPopupAnimation(object o)
        {
            PopupAnimation value = (PopupAnimation)o;
            return value == PopupAnimation.None ||
                   value == PopupAnimation.Fade ||
                   value == PopupAnimation.Slide ||
                   value == PopupAnimation.Scroll;
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

            if (e.OldValue is UIElement oldChild)
            {
                // Disconnect the old child from the visual tree immediately, because it would be
                // unsafe to maintain it connected after the IsVisualTreeRoot flag has been cleared.
                if (popup._popupRoot is not null)
                {
                    popup._popupRoot.Child = null;
                }

                oldChild.IsVisualTreeRoot = false;

                popup.RemoveLogicalChild(oldChild);
            }

            if (e.NewValue is UIElement newChild)
            {
                popup.AddLogicalChild(newChild);

                newChild.IsVisualTreeRoot = true;

                if (popup._popupRoot is not null)
                {
                    popup._popupRoot.Child = newChild;
                }
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
                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsOpenChanged, CoerceIsOpen));

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            bool isOpen = (bool)e.NewValue;

            popup.UpdatePositionTracker();

            if (isOpen)
            {
                popup.ShowPopupRootIfNotAlreadyVisible();
                popup.OnOpened(EventArgs.Empty);

                // The popup can be closed during during the Opened event
                if (popup.IsOpen)
                {
                    popup.Unloaded += new RoutedEventHandler(CloseOnUnloaded);
                    popup.IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
                }
            }
            else
            {
                popup.OnClosed(EventArgs.Empty);
                popup.HidePopupRootIfVisible();
                popup.Unloaded -= new RoutedEventHandler(CloseOnUnloaded);
                popup.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
            }
        }

        private static object CoerceIsOpen(DependencyObject d, object value)
        {
            if ((bool)value && Application.Current.MainWindow is null)
            {
                var popup = (Popup)d;
                popup.DeferOpenUntilMainWindowReady();
                return BooleanBoxes.FalseBox;
            }

            return value;
        }

        private void DeferOpenUntilMainWindowReady() => Application.Current.MainWindowReady += OpenOnMainWindowReady;

        private void OpenOnMainWindowReady(object sender, EventArgs e)
        {
            ((Application)sender).MainWindowReady -= OpenOnMainWindowReady;

            CoerceValue(IsOpenProperty);
        }

        private static void CloseOnUnloaded(object sender, RoutedEventArgs e)
            => ((Popup)sender).SetCurrentValueInternal(IsOpenProperty, BooleanBoxes.FalseBox);

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

        private Matrix3D GetPopupTransform()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                return InternalTransformToAncestor(null);
            }

            return GetLocalTransform();
        }

        private Matrix3D GetLocalTransform()
        {
            Matrix3D m = Matrix3D.Identity;

            if (Projection is Projection projection)
            {
                m *= Matrix3DUtil.Flatten(projection.GetProjectionMatrix(new Size(0, 0)));
            }

            if (LayoutTransform is Transform layoutTransform)
            {
                m *= layoutTransform.Matrix3D;
            }

            if (RenderTransform is Transform renderTransform)
            {
                m *= renderTransform.Matrix3D;
            }

            return m;
        }

        private void UpdatePosition()
        {
            if (_popupRoot is null || !_popupRoot.IsOpen) return;

            Matrix3D transform = GetPopupTransform();
            _popupRoot.Transform = transform;

            var placement = new PlacementContext(this, transform);

            Point position;
            PlacementMode placementMode = Placement;

            if (!placement.IsValid)
            {
                // The popup is not visible (seen edge-on, for instance).
                position = new Point(HorizontalOffset, VerticalOffset);
            }
            else if (IsAbsolutePlacementMode(placementMode))
            {
                position = PerformAbsolutePlacement(placementMode, placement);
            }
            else if (PlacementTarget is FrameworkElement target && INTERNAL_VisualTreeManager.IsElementInVisualTree(target))
            {
                position = PerformRelativePlacement(target, placementMode, placement);
            }
            else if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                // In Silverlight, when a popup is in the visual tree, it always position the Child relative
                // to the popup's top left corner.
                position = new Point(HorizontalOffset, VerticalOffset);
            }
            else
            {
                // Nothing worked, use absolute placement
                position = PerformAbsolutePlacement(PlacementMode.Absolute, placement);
            }

            _popupRoot.SetPosition(position.X, position.Y);
        }

        private static bool IsAbsolutePlacementMode(PlacementMode placement)
        {
            return placement == PlacementMode.Absolute ||
                   placement == PlacementMode.Mouse ||
                   placement == PlacementMode.MousePoint;
        }

        private static bool IsRelativePlacementMode(PlacementMode placement)
        {
            return placement == PlacementMode.Relative ||
                   placement == PlacementMode.Bottom ||
                   placement == PlacementMode.Center ||
                   placement == PlacementMode.Right ||
                   placement == PlacementMode.Left ||
                   placement == PlacementMode.Top;
        }

        private Point PerformAbsolutePlacement(PlacementMode placement, in PlacementContext context)
        {
            Debug.Assert(IsAbsolutePlacementMode(placement));

            if (Child is not UIElement child)
            {
                return new Point(HorizontalOffset, VerticalOffset);
            }

            Point anchor;

            switch (placement)
            {
                case PlacementMode.Absolute:
                    anchor = new Point(0, 0);
                    break;
                case PlacementMode.Mouse:
                    anchor = Mouse.GetPosition(null);
                    anchor.Y += _cursorOffsetY;
                    break;
                case PlacementMode.MousePoint:
                    anchor = Mouse.GetPosition(null);
                    break;
                default:
                    Debug.Assert(false, $"Unexpected absolute placement mode: '{placement}'.");
                    anchor = new Point(0, 0);
                    break;
            }

            anchor.Offset(HorizontalOffset, VerticalOffset);

            if (!context.TryAnchorToLocal(anchor, out Point position))
            {
                return new Point(HorizontalOffset, VerticalOffset);
            }

            if (StaysWithinScreenBounds)
            {
                position = PutInScreenBounds(position, child.RenderSize, GetWindowSize(), context);
            }

            return position;
        }

        private Point PerformRelativePlacement(UIElement placementTarget, PlacementMode placement, in PlacementContext context)
        {
            Debug.Assert(IsRelativePlacementMode(placement));

            var offsets = new Point(HorizontalOffset, VerticalOffset);

            if (Child is not UIElement child || !context.TryGetTargetInterestPoints(placementTarget, offsets, out InterestPoints target))
            {
                return offsets;
            }

            Size windowSize = GetWindowSize();
            Size childSize = child.RenderSize;

            Point position = GetCandidatePosition(placement, target, childSize);
            Rect candidateBounds = context.GetBounds(position, childSize);

            if (candidateBounds.Bottom > windowSize.Height)
            {
                if (placement == PlacementMode.Bottom)
                {
                    position = GetCandidatePosition(PlacementMode.Top, target, childSize);
                }
            }
            else if (candidateBounds.Top < 0)
            {
                if (placement == PlacementMode.Top)
                {
                    position = GetCandidatePosition(PlacementMode.Bottom, target, childSize);
                }
            }

            candidateBounds = context.GetBounds(position, childSize);

            if (candidateBounds.Right > windowSize.Width)
            {
                if (placement == PlacementMode.Right)
                {
                    position = GetCandidatePosition(PlacementMode.Left, target, childSize);
                }
            }
            else if (candidateBounds.Left < 0)
            {
                if (placement == PlacementMode.Left)
                {
                    position = GetCandidatePosition(PlacementMode.Right, target, childSize);
                }
            }

            if (StaysWithinScreenBounds)
            {
                position = PutInScreenBounds(position, childSize, windowSize, context);
            }

            return position;

            static Point GetCandidatePosition(PlacementMode placement, InterestPoints target, Size childSize)
            {
                switch (placement)
                {
                    case PlacementMode.Relative:
                        return target.TopLeft;
                    case PlacementMode.Top:
                        return new Point(target.TopLeft.X, target.TopLeft.Y - childSize.Height);
                    case PlacementMode.Bottom:
                        return target.BottomLeft;
                    case PlacementMode.Left:
                        return new Point(target.TopLeft.X - childSize.Width, target.TopLeft.Y);
                    case PlacementMode.Right:
                        return target.TopRight;
                    case PlacementMode.Center:
                        return new Point(target.Center.X - childSize.Width / 2, target.Center.Y - childSize.Height / 2);
                    default:
                        Debug.Assert(false, $"Unexpected relative placement mode: '{placement}'.");
                        return new Point(0, 0);
                }
            }
        }

        // With a projection, the bounds of the child are not a translation of the bounds at another
        // position, so the position is corrected iteratively: the corner of the child that overflows
        // is moved onto the edge of the window. In 2D, the correction of each axis is exact.
        private static Point PutInScreenBounds(Point position, Size childSize, Size windowSize, in PlacementContext context)
        {
            for (int i = 0; i < 6; i++)
            {
                Rect childBounds = context.GetBounds(position, childSize);
                if (childBounds.IsEmpty)
                {
                    break;
                }

                double dx = 0, dy = 0;

                if (childBounds.Bottom > windowSize.Height)
                {
                    dy = windowSize.Height - childBounds.Bottom;
                }
                else if (childBounds.Top < 0)
                {
                    dy = -childBounds.Top;
                }

                if (childBounds.Right > windowSize.Width)
                {
                    dx = windowSize.Width - childBounds.Right;
                }
                else if (childBounds.Left < 0)
                {
                    dx = -childBounds.Left;
                }

                if (dx == 0 && dy == 0)
                {
                    break;
                }

                // One axis at a time, since the overflowing corners may differ.
                if (Math.Abs(dx) >= Math.Abs(dy))
                {
                    dy = 0;
                }
                else
                {
                    dx = 0;
                }

                Vector corner = GetOverflowingCorner(position, childSize, dx, dy, context);
                if (!context.TryLocalToRoot(position + corner, out Point point) ||
                    !context.TryRootToLocal(new Point(point.X + dx, point.Y + dy), out point))
                {
                    break;
                }

                position = point - corner;
            }

            return position;

            static Vector GetOverflowingCorner(Point position, Size childSize, double dx, double dy, in PlacementContext context)
            {
                Vector best = default;
                double bestOverflow = double.NegativeInfinity;

                for (int i = 0; i < 4; i++)
                {
                    var corner = new Vector((i & 1) * childSize.Width, (i >> 1) * childSize.Height);
                    if (!context.TryLocalToRoot(position + corner, out Point point))
                    {
                        // The bounds are clipped by the camera plane.
                        return default;
                    }

                    double overflow = dx != 0 ? -Math.Sign(dx) * point.X : -Math.Sign(dy) * point.Y;
                    if (overflow > bestOverflow)
                    {
                        best = corner;
                        bestOverflow = overflow;
                    }
                }

                return best;
            }
        }

        private static Size GetWindowSize()
        {
            var root = Application.Current.Host.Content;
            return new Size(root.ActualWidth, root.ActualHeight);
        }

        private struct InterestPoints
        {
            public Point TopLeft;
            public Point TopRight;
            public Point BottomLeft;
            public Point BottomRight;
            public Point Center;
        }

        private readonly struct PlacementContext
        {
            private readonly Matrix3D _transform;
            private readonly Matrix3D _inverse;
            private readonly Vector _anchorOffset;

            public PlacementContext(Popup popup, Matrix3D transform)
            {
                _transform = transform;
                IsValid = Matrix3DUtil.TryInvert(transform, out _inverse);

                // When the popup is not in the visual tree, the image of its origin by its transform
                // is added to the placement anchors.
                if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(popup) &&
                    Matrix3DUtil.TryTransform(transform, new Point(), out Point origin))
                {
                    _anchorOffset = (Vector)origin;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the coordinate space of the popup can be computed
            /// from the root.
            /// </summary>
            public bool IsValid { get; }

            /// <summary>
            /// Converts a placement anchor from the root to the coordinate space of the popup.
            /// </summary>
            public bool TryAnchorToLocal(Point anchor, out Point result)
                => TryRootToLocal(anchor + _anchorOffset, out result);

            public bool TryRootToLocal(Point point, out Point result)
                => Matrix3DUtil.TryTransform(_inverse, point, out result);

            public bool TryLocalToRoot(Point point, out Point result)
                => Matrix3DUtil.TryTransform(_transform, point, out result);

            /// <summary>
            /// Gets the visible bounds, in the root, of the child at the specified position.
            /// </summary>
            public Rect GetBounds(Point position, Size childSize)
                => Matrix3DUtil.TransformBounds(_transform, new Rect(position, childSize));

            public bool TryGetTargetInterestPoints(UIElement target, Point offsets, out InterestPoints interestPoints)
            {
                interestPoints = new();

                Matrix3D transform = target.InternalTransformToAncestor(null);
                var rect = new Rect(offsets, target.RenderSize);

                return TryToLocal(transform, rect.TopLeft, ref interestPoints.TopLeft) &&
                       TryToLocal(transform, rect.TopRight, ref interestPoints.TopRight) &&
                       TryToLocal(transform, rect.BottomLeft, ref interestPoints.BottomLeft) &&
                       TryToLocal(transform, rect.BottomRight, ref interestPoints.BottomRight) &&
                       TryToLocal(transform, new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2), ref interestPoints.Center);
            }

            private bool TryToLocal(Matrix3D transform, Point point, ref Point result)
                => Matrix3DUtil.TryTransform(transform, point, out Point anchor) && TryAnchorToLocal(anchor, out result);
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

            // Force layout update to prevent the popup content from briefly appearing in
            // the top left corner of the screen.
            UpdateLayout();
            UpdatePosition();

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

            if (RegisteredPopupsField.GetValue(placementTarget) is not List<Popup> registeredPopups)
            {
                registeredPopups = new(1);
                RegisteredPopupsField.SetValue(placementTarget, registeredPopups);
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

            if (RegisteredPopupsField.GetValue(placementTarget) is List<Popup> registeredPopups)
            {
                registeredPopups.Remove(popup);

                // If after removing this popup from the placement targets popup registration list, no more
                // popups are left, we can also get rid of the array list.
                if (registeredPopups.Count == 0)
                {
                    RegisteredPopupsField.ClearValue(placementTarget);
                }
            }
        }

        internal static readonly UncommonField<List<Popup>> RegisteredPopupsField = new();

        public event EventHandler ClosedDueToOutsideClick;

        internal event EventHandler<CancelEventArgs> OutsideClick;

        internal void OnOutsideClick(bool isClosing)
        {
            bool cancelled = false;

            if (OutsideClick is EventHandler<CancelEventArgs> handler)
            {
                var args = new CancelEventArgs();
                handler(this, args);
                cancelled = args.Cancel;
            }

            if (!cancelled && isClosing)
            {
                CloseFromAnOutsideClick();
            }
        }

        private void CloseFromAnOutsideClick()
        {
            ClosedDueToOutsideClick?.Invoke(this, EventArgs.Empty);

            if (IsOpen)
            {
                SetCurrentValueInternal(IsOpenProperty, BooleanBoxes.FalseBox);
            }
        }

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
                IsAbsolutePlacementMode(Placement) ||
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
                SetCurrentValueInternal(IsOpenProperty, BooleanBoxes.FalseBox);
                return;
            }

            UpdatePosition();
        }
    }
}
