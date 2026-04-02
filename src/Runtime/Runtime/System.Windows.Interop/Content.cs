
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

namespace System.Windows.Interop;

public class Content
{
    internal Content() { }

    /// <summary>
    /// Gets the browser-determined height of the content area.
    /// </summary>
    public double ActualHeight { get; private set; }

    /// <summary>
    /// Gets the browser-determined width of the Silverlight content area.
    /// </summary>
    public double ActualWidth { get; private set; }

    /// <summary>
    /// Gets or sets a value that indicates whether the page is displaying in full-screen mode.
    /// </summary>
    public bool IsFullScreen
    {
        get => OpenSilver.Interop.ExecuteJavaScriptBoolean("osjs.host.isFullscreen");
        set => OpenSilver.Interop.ExecuteJavaScriptVoid($"osjs.host.isFullscreen = {(value ? "true" : "false")}");
    }

    /// <summary>
    /// Gets the factor by which the current browser window resizes its contents.
    /// </summary>
    /// <returns> 
    /// The zoom setting for the current browser window.
    /// </returns>
    public double ZoomFactor => OpenSilver.Interop.ExecuteJavaScriptDouble("osjs.host.zoomFactor", false);

    /// <summary>
    /// Occurs when the browser enters or exits full-screen mode.
    /// </summary>
    public event EventHandler FullScreenChanged;

    /// <summary>
    /// Occurs when the <see cref="Window"/> gets resized.
    /// </summary>
    public event EventHandler Resized;

    internal void FireFullScreenChanged() => FullScreenChanged?.Invoke(Application.Current?.RootVisual, EventArgs.Empty);

    internal void FireResized() => Resized?.Invoke(Application.Current?.RootVisual, EventArgs.Empty);

    internal void SetSize(Size size)
    {
        ActualWidth = size.Width;
        ActualHeight = size.Height;
    }

    /// <summary>
    /// Gets or sets a value that indicates the behavior of full-screen mode.
    /// </summary>
    [OpenSilver.NotImplemented]
    public FullScreenOptions FullScreenOptions { get; set; }

    /// <summary>
    /// Occurs when the zoom setting in the host browser window changes or is initialized.
    /// </summary>
    [OpenSilver.NotImplemented]
    public event EventHandler Zoomed;
}
