
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace OpenSilver.Internal;

internal class TouchScrollHelper
{
    private readonly ScrollViewer _scrollViewer;
    private readonly MouseButtonEventHandler _mouseLeftButtonDownHandler;
    private readonly MouseButtonEventHandler _mouseLeftButtonUpHandler;
    private readonly MouseEventHandler _mouseMoveHandler;

    private Point _pointerPosition;
    private double _horizontalOffset;
    private double _verticalOffset;
    private double _velocityX;
    private double _velocityY;
    private DispatcherTimer _inertiaTimer;

    private bool IsHorizontalScrollBarVisible => _scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible;
    private bool IsVerticalScrollBarVisible => _scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible;

    public TouchScrollHelper(ScrollViewer scrollViewer)
    {
        _scrollViewer = scrollViewer ?? throw new ArgumentNullException(nameof(scrollViewer));
        _mouseLeftButtonDownHandler = new MouseButtonEventHandler(OnMouseLeftButtonDown);
        _mouseLeftButtonUpHandler = new MouseButtonEventHandler(OnMouseLeftButtonUp);
        _mouseMoveHandler = new MouseEventHandler(OnMouseMove);
    }

    public void SubscribeToEvents()
    {
        _scrollViewer.AddHandler(UIElement.MouseLeftButtonDownEvent, _mouseLeftButtonDownHandler, handledEventsToo: true);
        _scrollViewer.AddHandler(UIElement.MouseLeftButtonUpEvent, _mouseLeftButtonUpHandler, handledEventsToo: true);
        _scrollViewer.AddHandler(UIElement.MouseMoveEvent, _mouseMoveHandler, handledEventsToo: true);
    }

    public void UnsubscribeFromEvents()
    {
        _scrollViewer.RemoveHandler(UIElement.MouseLeftButtonDownEvent, _mouseLeftButtonDownHandler);
        _scrollViewer.RemoveHandler(UIElement.MouseLeftButtonUpEvent, _mouseLeftButtonUpHandler);
        _scrollViewer.RemoveHandler(UIElement.MouseMoveEvent, _mouseMoveHandler);
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!e.Handled && _scrollViewer.Focus())
        {
            e.Handled = true;
        }

        if (!e.IsTouchEvent ||
            Pointer.Captured != null ||
            !IsHorizontalScrollBarVisible && !IsVerticalScrollBarVisible)
        {
            return;
        }

        if (_inertiaTimer != null)
        {
            _inertiaTimer.Stop();
            _inertiaTimer = null;
        }

        _scrollViewer.CaptureMouse();

        _pointerPosition = e.GetPosition(null);
        _horizontalOffset = _scrollViewer.ScrollInfo.HorizontalOffset;
        _verticalOffset = _scrollViewer.ScrollInfo.VerticalOffset;
        _velocityX = 0;
        _velocityY = 0;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!e.IsTouchEvent || Pointer.Captured != _scrollViewer)
        {
            return;
        }

        _scrollViewer.ReleaseMouseCapture();

        if (IsVerticalScrollBarVisible || IsHorizontalScrollBarVisible)
        {
            StartScrollingInertia();
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!e.IsTouchEvent || Pointer.Captured != _scrollViewer)
        {
            return;
        }

        var position = e.GetPosition(null);

        if (IsHorizontalScrollBarVisible)
        {
            double deltaX = _pointerPosition.X - position.X;
            _velocityX = deltaX;
            _horizontalOffset += deltaX;
            _scrollViewer.ScrollToHorizontalOffset(_horizontalOffset);
        }

        if (IsVerticalScrollBarVisible)
        {
            double deltaY = _pointerPosition.Y - position.Y;
            _verticalOffset += deltaY;
            _velocityY = deltaY;
            _scrollViewer.ScrollToVerticalOffset(_verticalOffset);
        }

        _pointerPosition = position;
    }

    private void StartScrollingInertia()
    {
        const double Deceleration = 0.97;
        const double Threshold = 0.1;

        _inertiaTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) }; // Approximately 60 FPS

        _inertiaTimer.Tick += (_, _) =>
        {
            var scrolledHorizontally = Math.Abs(_velocityX) < Threshold || !IsHorizontalScrollBarVisible;
            var scrolledVertically = Math.Abs(_velocityY) < Threshold || !IsVerticalScrollBarVisible;

            if (scrolledHorizontally && scrolledVertically)
            {
                _inertiaTimer.Stop();
                return;
            }

            if (IsHorizontalScrollBarVisible)
            {
                _horizontalOffset += _velocityX;
                _scrollViewer.ScrollToHorizontalOffset(_horizontalOffset);
                _velocityX *= Deceleration;
            }

            if (IsVerticalScrollBarVisible)
            {
                _verticalOffset += _velocityY;
                _scrollViewer.ScrollToVerticalOffset(_verticalOffset);
                _velocityY *= Deceleration;
            }
        };

        _inertiaTimer.Start();
    }
}
