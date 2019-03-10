
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using CSHTML5;
#if MIGRATION
using System.Windows.Input;
#else
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
    public sealed class Thumb : Control
    {
        //todo: Perfs: Each time we use GetPosition, only call it once.
        bool _isPointerCaptured;
        Pointer _capturedPointer;
        double _pointerX;
        double _pointerY;

        /// <summary>
        /// Initializes a new instance of the Thumb class.
        /// </summary>
        public Thumb()
        {
            // Set default style:
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultThumbStyle.GetDefaultStyle());

            // Register the pointer events of the current control:
#if MIGRATION
            this.MouseLeftButtonDown += Thumb_MouseLeftButtonDown;
            this.MouseMove += Thumb_MouseMove;
            this.MouseLeftButtonUp += Thumb_MouseLeftButtonUp;
#else
            this.PointerPressed += Thumb_PointerPressed;
            this.PointerMoved += Thumb_PointerMoved;
            this.PointerReleased += Thumb_PointerReleased;
#endif
        }

#if MIGRATION
        void Thumb_MouseLeftButtonDown(object sender, Input.MouseButtonEventArgs e)
#else
        void Thumb_PointerPressed(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            // Remember the current pointer position:

#if MIGRATION
            _pointerX = e.GetPosition(null).X;
            _pointerY = e.GetPosition(null).Y;
#else
            _pointerX = e.GetCurrentPoint(null).Position.X;
            _pointerY = e.GetCurrentPoint(null).Position.Y;
#endif

            // Capture the pointer so that when dragged outside the thumb, we can still get its position:
#if MIGRATION
            this.CaptureMouse();
#else
            this.CapturePointer(e.Pointer);
#endif

            // Remember that the pointer is currently captured:
            _isPointerCaptured = true;
            _capturedPointer = e.Pointer;

            // Set the "IsDragging" dependency property:
            this.IsDragging = true;

            // Raise the "DragStarted" event:
            if (DragStarted != null)
                DragStarted(this, new DragStartedEventArgs(_pointerX, _pointerY));
        }

#if MIGRATION
        void Thumb_MouseLeftButtonUp(object sender, Input.MouseButtonEventArgs e)
#else
        void Thumb_PointerReleased(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            if (_isPointerCaptured && e.Pointer == _capturedPointer)
            {
                CancelDrag();
            }
        }

#if MIGRATION
        void Thumb_MouseMove(object sender, Input.MouseEventArgs e)
#else
        void Thumb_PointerMoved(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            if (_isPointerCaptured)
            {
                // Calculate the delta of the movement:

#if MIGRATION
                double horizontalChange = e.GetPosition(null).X - _pointerX;
                double verticalChange = e.GetPosition(null).Y - _pointerY;
#else
                double horizontalChange = e.GetCurrentPoint(null).Position.X - _pointerX;
                double verticalChange = e.GetCurrentPoint(null).Position.Y - _pointerY;
#endif
                // Raise the "DragDelta" event:
                if (DragDelta != null)
                    DragDelta(this, new DragDeltaEventArgs(horizontalChange, verticalChange));

                // Remember the new pointer position:
#if MIGRATION
                _pointerX = e.GetPosition(null).X;
                _pointerY = e.GetPosition(null).Y;
#else
                _pointerX = e.GetCurrentPoint(null).Position.X;
                _pointerY = e.GetCurrentPoint(null).Position.Y;
#endif
            }
        }

        /// <summary>
        /// Cancels a drag operation for the Thumb.
        /// </summary>
        public void CancelDrag()
        {
            if (_isPointerCaptured)
            {
                // Stop capturing the pointer:
                _isPointerCaptured = false;
#if MIGRATION
                this.ReleaseMouseCapture();
#else
                this.ReleasePointerCapture(_capturedPointer);
#endif

                // Set the "IsDragging" dependency property:
                this.IsDragging = false;

                // Raise the "DragCompleted" event:
                if (DragCompleted != null)
                    DragCompleted(this, new DragCompletedEventArgs(0d, 0d, false));
            }
        }

        /// <summary>
        /// Gets whether the Thumb control has focus and mouse capture. The default is false.
        /// </summary>
        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        /// <summary>
        /// Identifies the IsDragging dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register("IsDragging", typeof(bool), typeof(Thumb), new PropertyMetadata(false));


        /// <summary>
        /// Fires when the Thumb control loses mouse capture.
        /// </summary>
        public event DragCompletedEventHandler DragCompleted;

        /// <summary>
        /// Fires one or more times as the mouse pointer is moved when a Thumb control has logical focus and mouse capture.
        /// </summary>
        public event DragDeltaEventHandler DragDelta;

        /// <summary>
        /// Fires when a Thumb control receives logical focus and mouse capture.
        /// </summary>
        public event DragStartedEventHandler DragStarted;

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();
            if (!Interop.IsRunningInTheSimulator)
            {
                // Prevent the selection of text while dragging from the thumb
                Interop.ExecuteJavaScriptAsync("$0.onselectstart = function() { return false; }", this.INTERNAL_OuterDomElement);
            }
        }
    }
}
