
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

namespace System.Windows.Controls;

/// <summary>
/// Represents the method that will handle the <see cref="ScrollViewer.ScrollChanged"/> routed event.
/// </summary>
/// <param name="sender">
/// The object where the event handler is attached.
/// </param>
/// <param name="e">
/// The event data.
/// </param>
public delegate void ScrollChangedEventHandler(object sender, ScrollChangedEventArgs e);

/// <summary>
/// Describes a change in the scrolling state and contains the required arguments for a <see cref="ScrollViewer.ScrollChanged"/> event.
/// </summary>
public class ScrollChangedEventArgs : RoutedEventArgs
{
    private readonly Vector _offset;
    private readonly Vector _offsetChange;
    private readonly Size _extent;
    private readonly Vector _extentChange;
    private readonly Size _viewport;
    private readonly Vector _viewportChange;

    internal ScrollChangedEventArgs(Vector offset, Vector offsetChange, Size extent, Vector extentChange, Size viewport, Vector viewportChange)
    {
        _offset = offset;
        _offsetChange = offsetChange;
        _extent = extent;
        _extentChange = extentChange;
        _viewport = viewport;
        _viewportChange = viewportChange;
    }

    /// <summary>
    /// Gets the updated horizontal offset value for a <see cref="ScrollViewer"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the updated value of the horizontal offset for a <see cref="ScrollViewer"/>.
    /// </returns>
    public double HorizontalOffset => _offset.X;

    /// <summary>
    /// Gets the updated value of the vertical offset for a <see cref="ScrollViewer"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the updated vertical offset of a <see cref="ScrollViewer"/>.
    /// </returns>
    public double VerticalOffset => _offset.Y;

    /// <summary>
    /// Gets a value that indicates the change in horizontal offset for a <see cref="ScrollViewer"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the change in horizontal offset for a <see cref="ScrollViewer"/>.
    /// </returns>
    public double HorizontalChange => _offsetChange.X;

    /// <summary>
    /// Gets a value that indicates the change in vertical offset of a <see cref="ScrollViewer"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the change in vertical offset of a <see cref="ScrollViewer"/>.
    /// </returns>
    public double VerticalChange => _offsetChange.Y;

    /// <summary>
    /// Gets the updated value of the viewport width for a <see cref="ScrollViewer"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the updated value of the viewport width for a <see cref="ScrollViewer"/>.
    /// </returns>
    public double ViewportWidth => _viewport.Width;

    /// <summary>
    /// Gets the updated value of the viewport height for a <see cref="ScrollViewer"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the updated viewport height of a <see cref="ScrollViewer"/>.
    /// </returns>
    public double ViewportHeight => _viewport.Height;

    /// <summary>
    /// Gets a value that indicates the change in viewport width of a <see cref="ScrollViewer"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the change in viewport width for a <see cref="ScrollViewer"/>.
    /// </returns>
    public double ViewportWidthChange => _viewportChange.X;

    /// <summary>
    /// Gets a value that indicates the change in value of the viewport height for a <see cref="ScrollViewer"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the change in vertical viewport height for a <see cref="ScrollViewer"/>.
    /// </returns>
    public double ViewportHeightChange => _viewportChange.Y;

    /// <summary>
    /// Gets the updated width of the <see cref="ScrollViewer"/> extent.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the updated extent width.
    /// </returns>
    public double ExtentWidth => _extent.Width;

    /// <summary>
    /// Gets the updated height of the <see cref="ScrollViewer"/> extent.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the updated extent height.
    /// </returns>
    public double ExtentHeight => _extent.Height;

    /// <summary>
    /// Gets a value that indicates the change in width of the <see cref="ScrollViewer"/> extent.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the change in extent width.
    /// </returns>
    public double ExtentWidthChange => _extentChange.X;

    /// <summary>
    /// Gets a value that indicates the change in height of the <see cref="ScrollViewer"/> extent.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> that represents the change in extent height.
    /// </returns>
    public double ExtentHeightChange => _extentChange.Y;

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((ScrollChangedEventHandler)genericHandler)(genericTarget, this);
}
