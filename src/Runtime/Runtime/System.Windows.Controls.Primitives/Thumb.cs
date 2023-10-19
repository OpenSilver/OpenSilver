// (c) Copyright Microsoft Corporation. 
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved. 

using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Controls.Primitives
{
    /// <summary> 
    /// Represents a control that can be dragged by the user.
    /// </summary>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    public sealed class Thumb : Control
    {
        /// <summary> 
        /// Initializes a new instance of the <see cref="Thumb"/> class.
        /// </summary> 
        public Thumb()
        {
            DefaultStyleKey = typeof(Thumb);
            IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
        }

        /// <summary>
        /// Occurs when a <see cref="Thumb"/> control receives logical focus and 
        /// mouse capture.
        /// </summary>
        public event DragStartedEventHandler DragStarted;

        /// <summary>
        /// Occurs one or more times as the mouse pointer is moved when a <see cref="Thumb"/>
        /// control has logical focus and mouse capture.
        /// </summary> 
        public event DragDeltaEventHandler DragDelta;

        /// <summary> 
        /// Occurs when the <see cref="Thumb"/> control loses mouse capture.
        /// </summary>
        public event DragCompletedEventHandler DragCompleted;

        private static readonly DependencyPropertyKey IsDraggingPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsDragging),
                typeof(bool),
                typeof(Thumb),
                new PropertyMetadata(OnIsDraggingPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="IsDragging"/> dependency property.
        /// </summary> 
        public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;

        /// <summary> 
        /// Gets whether the <see cref="Thumb"/> control has focus and
        /// mouse capture.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Thumb"/> control has focus and mouse
        /// capture; otherwise false. The default is false.
        /// </returns>
        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            private set { SetValue(IsDraggingPropertyKey, value); }
        }

        private static void OnIsDraggingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Thumb)d).OnDraggingChanged();
        }

        private void OnDraggingChanged()
        {
            UpdateVisualState();
        }

        private static readonly DependencyPropertyKey IsFocusedPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsFocused),
                typeof(bool),
                typeof(Thumb),
                null);

        /// <summary>
        /// Gets the identifier for the <see cref="IsFocused"/> dependency property. 
        /// </summary> 
        public static readonly DependencyProperty IsFocusedProperty = IsFocusedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether the thumb has focus.
        /// </summary> 
        /// <remarks>
        /// true to indicate the thumb has focus; otherwise false. The default is false.
        /// </remarks> 
        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            private set { SetValue(IsFocusedPropertyKey, value); }
        }

        /// <summary> 
        /// Builds the visual tree for the <see cref="Thumb"/> control when 
        /// a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState(false);
        }

        /// <summary>
        /// Cancels a drag operation for the <see cref="Thumb"/>.
        /// </summary> 
        public void CancelDrag()
        {
            if (IsDragging)
            {
                IsDragging = false;
                RaiseDragCompleted(true);
            }
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
            {
                return;
            }

            if (!IsDragging && IsEnabled)
            {
                e.Handled = true;

                CaptureMouse();
                IsDragging = true;

                Debug.Assert(Parent is UIElement);

                _origin = _previousPosition = e.GetPosition((UIElement)Parent);

                // Raise the DragStarted event 
                bool success = false;
                try
                {
                    DragStarted?.Invoke(this, new DragStartedEventArgs(_origin.X, _origin.Y));
                    success = true;
                }
                finally
                {
                    // Cancel the drag if the DragStarted handler failed
                    if (!success)
                    {
                        CancelDrag();
                    }
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (Pointer.INTERNAL_captured == this)
            {
                ReleaseMouseCapture();
            }
        }

        /// <inheritdoc />
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);

            RaiseDragCompleted(false);
            IsDragging = false;
        }

        /// <inheritdoc />
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (IsEnabled)
            {
                _isMouseOver = true;
                UpdateVisualState();
            }
        }

        /// <inheritdoc />
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (IsEnabled)
            {
                _isMouseOver = false;
                UpdateVisualState();
            }
        }

        /// <inheritdoc />
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsDragging)
            {
                Debug.Assert(Parent is UIElement);

                Point position = e.GetPosition((UIElement)Parent);

                if (position != _previousPosition)
                {
                    // Raise the DragDelta event 
                    DragDelta?.Invoke(this, new DragDeltaEventArgs(position.X - _previousPosition.X, position.Y - _previousPosition.Y));

                    _previousPosition = position;
                }
            }
        }

        /// <inheritdoc />
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            FocusChanged(HasFocus());
        }

        /// <inheritdoc />
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            FocusChanged(HasFocus());
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ThumbAutomationPeer(this);
        }

        private void FocusChanged(bool hasFocus)
        {
            IsFocused = hasFocus;
            UpdateVisualState();
        }

        /// <summary> 
        /// Change to the correct visual state for the thumb.
        /// </summary>
        internal void UpdateVisualState()
        {
            UpdateVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the thumb. 
        /// </summary>
        /// <param name="useTransitions">
        /// true to use transitions when updating the visual state, false to 
        /// snap directly to the new visual state.
        /// </param>
        internal void UpdateVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                GoToState(useTransitions, VisualStates.StateDisabled);
            }
            else if (IsDragging)
            {
                GoToState(useTransitions, VisualStates.StatePressed);
            }
            else if (_isMouseOver)
            {
                GoToState(useTransitions, VisualStates.StateMouseOver);
            }
            else
            {
                GoToState(useTransitions, VisualStates.StateNormal);
            }

            if (IsFocused && IsEnabled)
            {
                GoToState(useTransitions, VisualStates.StateFocused);
            }
            else
            {
                GoToState(useTransitions, VisualStates.StateUnfocused);
            }
        }

        internal bool GoToState(bool useTransitions, string stateName)
        {
            Debug.Assert(stateName != null);
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        private bool HasFocus()
        {
            for (DependencyObject doh = FocusManager.GetFocusedElement() as DependencyObject;
                doh != null;
                doh = VisualTreeHelper.GetParent(doh))
            {
                if (ReferenceEquals(doh, this))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Raise the DragCompleted event.
        /// </summary> 
        /// <param name="canceled"> 
        /// A Boolean value that indicates whether the drag operation was
        /// canceled by a call to the CancelDrag method. 
        /// </param>
        private void RaiseDragCompleted(bool canceled)
        {
            DragCompleted?.Invoke(this, new DragCompletedEventArgs(
                _previousPosition.X - _origin.X,
                _previousPosition.Y - _origin.Y,
                canceled));
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled)
            {
                _isMouseOver = false;
            }
            UpdateVisualState();
        }

        /// <summary>
        /// Whether the mouse is currently over the control 
        /// </summary>
        private bool _isMouseOver;

        /// <summary> 
        /// Origin of the thumb's drag operation.
        /// </summary>
        private Point _origin;

        /// <summary> 
        /// Last position of the thumb while during a drag operation.
        /// </summary> 
        private Point _previousPosition;
    }
}
