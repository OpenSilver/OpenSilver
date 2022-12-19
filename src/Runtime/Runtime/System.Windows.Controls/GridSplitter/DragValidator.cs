// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Class to encapsulate drag behavior for a UIElement.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    internal class DragValidator
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private UIElement _targetElement;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private Point _start;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _draggingActive;

        /// <summary>
        /// Occurs when a drag operation has started.
        /// </summary>
        public event EventHandler<DragStartedEventArgs> DragStartedEvent;
        
        /// <summary>
        /// Occurs when a drag operation has completed.
        /// </summary>
        public event EventHandler<DragCompletedEventArgs> DragCompletedEvent;
        
        /// <summary>
        /// Occurs when a drag operation has progressed.
        /// </summary>
        public event EventHandler<DragDeltaEventArgs> DragDeltaEvent;

        /// <summary>
        /// Create an instance of the DragValidator class.
        /// </summary>
        /// <param name="targetElement">
        /// UIElement that represents the source of the drag operation.
        /// </param>
        public DragValidator(UIElement targetElement)
        {
            Debug.Assert(targetElement != null, "targetElement should not be null!");

            _targetElement = targetElement;

#if MIGRATION
            _targetElement.MouseLeftButtonDown += new MouseButtonEventHandler(TargetElement_MouseLeftButtonDown);
            _targetElement.MouseLeftButtonUp += new MouseButtonEventHandler(TargetElement_MouseLeftButtonUp);
            _targetElement.MouseMove += new MouseEventHandler(TargetElement_MouseMove);
#else
            _targetElement.PointerPressed += new PointerEventHandler(TargetElement_MouseLeftButtonDown);
            _targetElement.PointerReleased += new PointerEventHandler(TargetElement_MouseLeftButtonUp);
            _targetElement.PointerMoved += new PointerEventHandler(TargetElement_MouseMove);
#endif
        }

        /// <summary>
        /// Handle the MouseMove event for the UIElement to update the drag
        /// operation.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        private void TargetElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggingActive)
            {
                OnDragDelta(e);
            }
        }

        /// <summary>
        /// Handle the MouseLeftButtonUp event for the UIElement to complete the
        /// drag operation.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        private void TargetElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
#if MIGRATION
            _targetElement.ReleaseMouseCapture();
#else
            _targetElement.ReleasePointerCapture();
#endif
            _draggingActive = false;
            OnDragCompleted(e, false);
        }

        /// <summary>
        /// Handle the MouseLeftButtonDown event for the UIElement to start a
        /// drag operation.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        private void TargetElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#if MIGRATION
            _start = e.GetPosition(_targetElement);
#else
            _start = e.GetCurrentPoint(_targetElement).Position;
#endif
            if (_targetElement.RenderTransform != null)
            {
                _start = _targetElement.RenderTransform.Transform(_start);
            }

#if MIGRATION
            _draggingActive = _targetElement.CaptureMouse();
#else
            _draggingActive = _targetElement.CapturePointer();
#endif

            // Only start the drag operation if CaptureMouse succeeded
            if (_draggingActive)
            {
                OnDragStarted();
            }
        }

        /// <summary>
        /// Invoke the DragStartedEvent handlers.
        /// </summary>
        private void OnDragStarted()
        {
            EventHandler<DragStartedEventArgs> handler = this.DragStartedEvent;
            if (handler != null)
            {
                handler(_targetElement, new DragStartedEventArgs(0.0, 0.0));
            }
        }

        /// <summary>
        /// Invoke the DragDeltaEvent handlers.
        /// </summary>
        /// <param name="e">Inherited code: Requires comment.</param>
        private void OnDragDelta(MouseEventArgs e)
        {
            EventHandler<DragDeltaEventArgs> handler = this.DragDeltaEvent;
            if (handler != null)
            {
#if MIGRATION
                Point p = e.GetPosition(_targetElement);
#else
                Point p = e.GetCurrentPoint(_targetElement).Position;
#endif
                if (_targetElement.RenderTransform != null)
                {
                    p = _targetElement.RenderTransform.Transform(p);
                }
                handler(_targetElement, new DragDeltaEventArgs(p.X - _start.X, p.Y - _start.Y));
            }
        }
        
        /// <summary>
        /// Invoke the DragCompletedEvent handlers.
        /// </summary>
        /// <param name="e">Inherited code: Requires comment.</param>
        /// <param name="canceled">Inherited code: Requires comment 1.</param>
        private void OnDragCompleted(MouseEventArgs e, bool canceled)
        {
            EventHandler<DragCompletedEventArgs> handler = this.DragCompletedEvent;
            if (handler != null)
            {
                Point endPoint;
                if (canceled)
                {
                    endPoint = new Point();
                }
                else
                {
#if MIGRATION
                    Point p = e.GetPosition(_targetElement);
#else
                    Point p = e.GetCurrentPoint(_targetElement).Position;
#endif
                    if (_targetElement.RenderTransform != null)
                    {
                        p = _targetElement.RenderTransform.Transform(p);
                    }
                    endPoint = new Point(p.X - _start.X, p.Y - _start.Y);
                }
                handler(_targetElement, new DragCompletedEventArgs(endPoint.X, endPoint.Y, canceled));
            }
        }
    }
}