

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


using CSHTML5;
using System;
#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Input;
#else
using Windows.UI.Input;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a control that can be dragged by the user.
    /// </summary>
    public sealed partial class Thumb : Control
    {
        #region IsDragging
        /// <summary>
        /// Gets whether the Thumb control has logical focus and mouse capture 
        /// and the left mouse button is pressed. 
        /// </summary>
        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            internal set { SetValue(IsDraggingProperty, value); }
        }

        /// <summary> 
        /// Identifies the IsDragging dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register("IsDragging", typeof(bool), typeof(Thumb), new PropertyMetadata(OnIsDraggingPropertyChanged));

        /// <summary> 
        /// IsDraggingProperty property changed handler.
        /// </summary> 
        /// <param name="d">Thumb that changed IsDragging.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsDraggingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Thumb thumb = d as Thumb;
            thumb.OnDraggingChanged();
        }

        /// <summary>
        /// This method is invoked when the IsDragging property changes. 
        /// </summary>
        private void OnDraggingChanged()
        {
            UpdateVisualState(true);
        }

        #endregion IsDragging

        #region IsFocused 
        // copy-pasted from Slider.cs

        /// <summary>
        /// Gets a value that determines whether this element has logical focus.
        /// </summary> 
        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            internal set { SetValue(IsFocusedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsFocused dependency property.
        /// </summary> 
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.Register("IsFocused", typeof(bool), typeof(Thumb), new PropertyMetadata(OnIsFocusedPropertyChanged));

        /// <summary>
        /// IsFocusedProperty property changed handler. 
        /// </summary> 
        /// <param name="d">Thumb that changed IsFocused.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Thumb t = d as Thumb;
            if (t.ElementRoot != null)
            {
                t.UpdateVisualState(true);
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowGotFocus(e))
            {
                Interaction.OnGotFocusBase();
            }
            // we're not calling base on purpose
            // SL2 does not fire the GotFocus event on Thumbs
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowLostFocus(e))
            {
                Interaction.OnLostFocusBase();
            }
            // we're not calling base on purpose
            // SL2 does not fire the GotFocus event on Thumbs
        }

        #endregion IsFocused 

        #region Events
        /// <summary> 
        /// Identifies the DragStarted routed event. 
        /// </summary>
        public event DragStartedEventHandler DragStarted;

        /// <summary>
        /// Identifies the DragDelta routed event. 
        /// </summary>
        public event DragDeltaEventHandler DragDelta;

        /// <summary> 
        /// Occurs when the Thumb control loses mouse capture.
        /// </summary> 
        public event DragCompletedEventHandler DragCompleted;
        #endregion Events

        #region Visual state management
        /// <summary>
        /// Gets or sets the helper that provides all of the standard
        /// interaction functionality.
        /// </summary>
        private InteractionHelper Interaction { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the Thumb class. 
        /// </summary> 
        public Thumb()
        {
            this.DefaultStyleKey = typeof(Thumb);
            Interaction = new InteractionHelper(this);
        }

        /// <summary> 
        /// Apply a template to the thumb.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            // Get the parts 
            ElementRoot = GetTemplateChild(ElementRootName) as FrameworkElement;

            // Get the states
            Interaction.OnApplyTemplateBase();
        }
        #endregion Constructor

        #region Mouse Handlers 
        /// <summary>
        /// Handle the MouseLeftButtonDown event.
        /// </summary> 
        /// <param name="e">MouseButtonEventArgs.</param>         
#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
#endif
        {
            if (!IsDragging && Interaction.AllowMouseLeftButtonDown(e))
            {
                e.Handled = true;

#if MIGRATION
                CaptureMouse();
#else
                CapturePointer();
#endif
                IsDragging = true;

#if MIGRATION
                _origin = _previousPosition = e.GetPosition((UIElement)this.Parent);
#else
                _origin = _previousPosition = e.GetCurrentPoint((UIElement)this.Parent);
#endif
                // Raise the DragStarted event
                bool success = false;
                try
                {
                    DragStartedEventHandler handler = DragStarted;
                    if (handler != null)
                    {
#if MIGRATION
                        handler(this, new DragStartedEventArgs(_origin.X, _origin.Y));
#else
                        handler(this, new DragStartedEventArgs(_origin.Position.X, _origin.Position.Y));
#endif
                    }
                    success = true;
                }
                finally
                {
                    // Cancel the drag if the DragStarted handler failed
                    if (!success)
                    {
                        CancelDrag();
                    }
                    Interaction.OnMouseLeftButtonDownBase();
                }
            }
        }

        /// <summary>
        /// Handle the MouseLeftButtonUp event. 
        /// </summary>
        /// <param name="e">MouseButtonEventArgs.</param>
#if MIGRATION
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
#endif
        {
            if (IsDragging && Interaction.AllowMouseLeftButtonUp(e))
            {
                e.Handled = true;
                IsDragging = false;
#if MIGRATION
                ReleaseMouseCapture();
#else
                ReleasePointerCapture();
#endif
                RaiseDragCompleted(false);

                Interaction.OnMouseLeftButtonUpBase();
            }
        }

        /// <summary> 
        /// Handle the MouseEnter event. 
        /// </summary>
        /// <param name="e">MouseEventArgs.</param> 
#if MIGRATION
        protected override void OnMouseEnter(MouseEventArgs e)
#else
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
#endif

        {
            if (Interaction.AllowMouseEnter(e))
                Interaction.OnMouseEnterBase();
        }

        /// <summary> 
        /// Handle the MouseLeave event.
        /// </summary>
        /// <param name="e">MouseEventArgs.</param> 
#if MIGRATION
        protected override void OnMouseLeave(MouseEventArgs e)
#else
        protected override void OnPointerExited(PointerRoutedEventArgs e)
#endif
        {
            if (Interaction.AllowMouseLeave(e))
                Interaction.OnMouseLeaveBase();
        }

        /// <summary> 
        /// Handle the MouseMove event.
        /// </summary>
        /// <param name="e">MouseEventArgs.</param> 
#if MIGRATION
        protected override void OnMouseMove(MouseEventArgs e)
#else
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
#endif
        {
            if (IsDragging)
            {
#if MIGRATION
                Point position = e.GetPosition((UIElement)this.Parent);
#else
                PointerPoint position = e.GetCurrentPoint((UIElement)this.Parent);
#endif
                if (position != _previousPosition)
                {
                    //                    e.Handled = true; 

                    // Raise the DragDelta event
                    DragDeltaEventHandler handler = DragDelta;
                    if (handler != null)
                    {
#if MIGRATION
                        handler(this, new DragDeltaEventArgs(position.X - _previousPosition.X, position.Y - _previousPosition.Y));
#else
                        handler(this, new DragDeltaEventArgs(position.Position.X - _previousPosition.Position.X, position.Position.Y - _previousPosition.Position.Y));
#endif
                    }

                    _previousPosition = position;
                }
            }
        }
        #endregion Mouse Handlers

        #region Change State 

        /// <summary>
        /// Change to the correct visual state for the thumb. 
        /// </summary> 
        internal void UpdateVisualState(bool useTransitions)
        {
            // all states are managed by the default InteractionHelper
            Interaction.UpdateVisualStateBase(useTransitions);
        }
        #endregion Change State

        #region Drag Cancel/Complete 
        /// <summary> 
        /// Cancel a drag operation if it is currently in progress.
        /// </summary> 
        public void CancelDrag()
        {
            if (IsDragging)
            {
                IsDragging = false;
                RaiseDragCompleted(true);
            }
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
            DragCompletedEventHandler handler = DragCompleted;
            if (handler != null)
            {
#if MIGRATION
                DragCompletedEventArgs args = new DragCompletedEventArgs(
                    _previousPosition.X - _origin.X,
                    _previousPosition.Y - _origin.Y,
                    canceled);
#else
                DragCompletedEventArgs args = new DragCompletedEventArgs(
                    _previousPosition.Position.X - _origin.Position.X,
                    _previousPosition.Position.Y - _origin.Position.Y,
                    canceled);
#endif
                handler(this, args);
            }
        }
        #endregion Drag Cancel/Complete

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            throw new NotImplementedException();
        }

        #region Template Parts
        /// <summary>
        /// Root of the thumb template. 
        /// </summary> 
        internal FrameworkElement ElementRoot { get; set; }
        internal const string ElementRootName = "RootElement";
        #endregion Template Parts 

        #region Member Variables 
        /// <summary>
        /// Whether the mouse is currently over the control
        /// </summary> 
        internal bool IsMouseOver
        {
            get { return Interaction.IsMouseOver; }
        }

#if MIGRATION
        /// <summary>
        /// Origin of the thumb's drag operation. 
        /// </summary> 
        internal Point _origin;
        /// <summary> 
        /// Last position of the thumb while during a drag operation.
        /// </summary>
        internal Point _previousPosition;
#else
        /// <summary>
        /// Origin of the thumb's drag operation. 
        /// </summary> 
        internal PointerPoint _origin;
        /// <summary> 
        /// Last position of the thumb while during a drag operation.
        /// </summary>
        internal PointerPoint _previousPosition;
#endif
        #endregion Member Variables
    }
}
