
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

using System.Windows.Threading;
using OpenSilver;

namespace System.Windows.Media;

/// <summary>
/// Describes visual content using draw, push, and pop commands.
/// </summary>
[NotImplemented]
public abstract class DrawingContext : DispatcherObject, IDisposable
{
    internal DrawingContext() { }

    /// <summary>
    /// Draw Text at the location specified.
    /// </summary>
    /// <param name="formattedText"> The FormattedText to draw. </param>
    /// <param name="origin"> The location at which to draw the text. </param>
    /// <exception cref="ObjectDisposedException">
    /// This call is illegal if this object has already been closed or disposed.
    /// </exception>
    [NotImplemented]
    public void DrawText(FormattedText formattedText, Point origin)
    {
    }

    /// <summary>
    /// Draws a rounded rectangle.
    /// </summary>
    /// <param name="brush">The brush used to fill the rectangle.</param>
    /// <param name="pen">The pen used to stroke the rectangle.</param>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <param name="radiusX">The horizontal corner radius.</param>
    /// <param name="radiusY">The vertical corner radius.</param>
    [NotImplemented]
    public void DrawRoundedRectangle(Brush brush, object pen, Rect rectangle, double radiusX, double radiusY)
    {
    }

    /// <summary>
    /// Closes the DrawingContext and flushes the content.
    /// Afterwards the DrawingContext can not be used anymore.
    /// This call does not require all Push calls to have been Popped.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// This call is illegal if this object has already been closed or disposed.
    /// </exception>
    public abstract void Close();

    /// <summary>
    /// This is the same as the Close call:
    /// Closes the DrawingContext and flushes the content.
    /// Afterwards the DrawingContext can not be used anymore.
    /// This call does not require all Push calls to have been Popped.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// This call is illegal if this object has already been closed or disposed.
    /// </exception>
    void IDisposable.Dispose()
    {
        VerifyAccess();

        DisposeCore();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose functionality implemented by subclasses
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// This call is illegal if this object has already been closed or disposed.
    /// </exception>
    protected abstract void DisposeCore();

    /// <summary>
    /// This verifies that the API can be called for read only access.
    /// </summary>
    protected virtual void VerifyApiNonstructuralChange() => VerifyAccess();
}