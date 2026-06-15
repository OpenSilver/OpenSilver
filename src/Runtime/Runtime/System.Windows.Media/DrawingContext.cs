
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

namespace System.Windows.Media;

/// <summary>
/// Describes visual content using draw, push, and pop commands.
/// </summary>
[OpenSilver.NotImplemented]
public abstract class DrawingContext : DispatcherObject, IDisposable
{
    internal DrawingContext() { }

    /// <summary>
    /// Closes the <see cref="DrawingContext"/> and flushes the content. Afterward, the <see cref="DrawingContext"/> 
    /// cannot be modified.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// This object has already been closed or disposed.
    /// </exception>
    public abstract void Close();

    /// <summary>
    /// Draws the specified <see cref="Drawing"/> object.
    /// </summary>
    /// <param name="drawing">
    /// The drawing to append.
    /// </param>
    public abstract void DrawDrawing(Drawing drawing);

    /// <summary>
    /// Draws an ellipse with the specified <see cref="Brush"/> and <see cref="Pen"/>.
    /// </summary>
    /// <param name="brush">
    /// The brush with which to fill the ellipse. This is optional, and can be null. If the brush is null, no fill 
    /// is drawn.
    /// </param>
    /// <param name="pen">
    /// The pen with which to stroke the ellipse. This is optional, and can be null. If the pen is null, no stroke 
    /// is drawn.
    /// </param>
    /// <param name="center">
    /// The location of the center of the ellipse.
    /// </param>
    /// <param name="radiusX">
    /// The horizontal radius of the ellipse.
    /// </param>
    /// <param name="radiusY">
    /// The vertical radius of the ellipse.
    /// </param>
    public abstract void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY);

    /// <summary>
    /// Draws the specified <see cref="Geometry"/> using the specified <see cref="Brush"/> and <see cref="Pen"/>.
    /// </summary>
    /// <param name="brush">
    /// The <see cref="Brush"/> with which to fill the <see cref="Geometry"/>. This is optional, and can be null. 
    /// If the brush is null, no fill is drawn.
    /// </param>
    /// <param name="pen">
    /// The <see cref="Pen"/> with which to stroke the <see cref="Geometry"/>. This is optional, and can be null. 
    /// If the pen is null, no stroke is drawn.
    /// </param>
    /// <param name="geometry">
    /// The <see cref="Geometry"/> to draw.
    /// </param>
    public abstract void DrawGeometry(Brush brush, Pen pen, Geometry geometry);

    /// <summary>
    /// Draws an image into the region defined by the specified <see cref="Rect"/>.
    /// </summary>
    /// <param name="imageSource">
    /// The image to draw.
    /// </param>
    /// <param name="rectangle">
    /// The region in which to draw bitmapSource.
    /// </param>
    public abstract void DrawImage(ImageSource imageSource, Rect rectangle);

    /// <summary>
    /// Draws a line between the specified points using the specified <see cref="Pen"/>.
    /// </summary>
    /// <param name="pen">
    /// The pen with which to stroke the line.
    /// </param>
    /// <param name="point0">
    /// The start point of the line.
    /// </param>
    /// <param name="point1">
    /// The end point of the line.
    /// </param>
    public abstract void DrawLine(Pen pen, Point point0, Point point1);

    /// <summary>
    /// Draws a rectangle with the specified <see cref="Brush"/> and <see cref="Pen"/>. The pen and the brush can
    /// be null. 
    /// </summary>
    /// <param name="brush">
    /// The brush with which to fill the rectangle. This is optional, and can be null. If the brush is null, no 
    /// fill is drawn.
    /// </param>
    /// <param name="pen">
    /// The pen with which to stroke the rectangle. This is optional, and can be null. If the pen is null, no stroke 
    /// is drawn.
    /// </param>
    /// <param name="rectangle">
    /// The rectangle to draw.
    /// </param>
    public abstract void DrawRectangle(Brush brush, Pen pen, Rect rectangle);

    /// <summary>
    /// Draws a rounded rectangle with the specified <see cref="Brush"/> and <see cref="Pen"/>.
    /// </summary>
    /// <param name="brush">
    /// The brush used to fill the rectangle.
    /// </param>
    /// <param name="pen">
    /// The pen used to stroke the rectangle.
    /// </param>
    /// <param name="rectangle">
    /// The rectangle to draw.
    /// </param>
    /// <param name="radiusX">
    /// The radius in the X dimension of the rounded corners. This value will be clamped to the range of 0 to 
    /// <see cref="Rect.Width"/> / 2.
    /// </param>
    /// <param name="radiusY">
    /// The radius in the Y dimension of the rounded corners. This value will be clamped to a value between 0 to 
    /// <see cref="Rect.Height"/> / 2.
    /// </param>
    public abstract void DrawRoundedRectangle(Brush brush, Pen pen, Rect rectangle, double radiusX, double radiusY);

    /// <summary>
    /// Draws formatted text at the specified location.
    /// </summary>
    /// <param name="formattedText">
    /// The formatted text to be drawn.
    /// </param>
    /// <param name="origin">
    /// The location where the text is to be drawn.
    /// </param>
    /// <exception cref="ObjectDisposedException">
    /// The object has already been closed or disposed.
    /// </exception>
    public void DrawText(FormattedText formattedText, Point origin) { }

    /// <summary>
    /// Pops the last opacity mask, opacity, clip, effect, or transform operation that was pushed onto the drawing 
    /// context.
    /// </summary>
    public abstract void Pop();

    /// <summary>
    /// Pushes the specified clip region onto the drawing context.
    /// </summary>
    /// <param name="clipGeometry">
    /// The clip region to apply to subsequent drawing commands.
    /// </param>
    public abstract void PushClip(Geometry clipGeometry);

    /// <summary>
    /// Pushes the specified <see cref="GuidelineSet"/> onto the drawing context.
    /// </summary>
    /// <param name="guidelines">
    /// The guideline set to apply to subsequent drawing commands.
    /// </param>
    public abstract void PushGuidelineSet(GuidelineSet guidelines);

    /// <summary>
    /// Pushes the specified opacity setting onto the drawing context.
    /// </summary>
    /// <param name="opacity">
    /// The opacity factor to apply to subsequent drawing commands. This factor is cumulative with previous 
    /// <see cref="PushOpacity(double)"/> operations.
    /// </param>
    public abstract void PushOpacity(double opacity);

    /// <summary>
    /// Pushes the specified opacity mask onto the drawing context.
    /// </summary>
    /// <param name="opacityMask">
    /// The opacity mask to apply to subsequent drawings. The alpha values of this brush determine the opacity 
    /// of the drawing to which it is applied.
    /// </param>
    public abstract void PushOpacityMask(Brush opacityMask);

    /// <summary>
    /// Pushes the specified <see cref="Transform"/> onto the drawing context.
    /// </summary>
    /// <param name="transform">
    /// The transform to apply to subsequent drawing commands.
    /// </param>
    public abstract void PushTransform(Transform transform);

    /// <summary>
    /// Releases all resources used by the <see cref="DrawingContext"/>.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// The object has already been closed or disposed.
    /// </exception>
    protected abstract void DisposeCore();

    /// <summary>
    /// This verifies that the API can be called for read only access.
    /// </summary>
    protected virtual void VerifyApiNonstructuralChange() => VerifyAccess();

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
}