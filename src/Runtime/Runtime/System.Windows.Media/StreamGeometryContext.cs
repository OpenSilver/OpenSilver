
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

using System.Collections.Generic;
using System.Windows.Threading;

namespace System.Windows.Media;

/// <summary>
/// Describes a geometry using drawing commands. This class is used with the <see cref="StreamGeometry"/>
/// class to create a lightweight geometry that does not support data binding, animation, or modification.
/// </summary>
public abstract class StreamGeometryContext : DispatcherObject, IDisposable
{
    /// <summary>
    /// Specifies the starting point for a new figure.
    /// </summary>
    /// <param name="startPoint">
    /// The <see cref="Point"/> where the figure begins.
    /// </param>
    /// <param name="isFilled">
    /// true to use the area contained by this figure for hit-testing, rendering, and clipping; otherwise, 
    /// false.
    /// </param>
    /// <param name="isClosed">
    /// true to close the figure; otherwise, false. For example, if two connecting lines are drawn, and 
    /// isClosed is set to false, the drawing will just be of two lines but if isClosed is set to true, 
    /// the two lines will be closed to create a triangle.
    /// </param>
    public abstract void BeginFigure(Point startPoint, bool isFilled, bool isClosed);

    /// <summary>
    /// Draws an arc to the specified point.
    /// </summary>
    /// <param name="point">
    /// The destination point for the end of the arc.
    /// </param>
    /// <param name="size">
    /// The radii (half the width and half the height) of an oval whose perimeter is used to draw the angle.
    /// If the oval is very rounded in all directions, the arc will be rounded, if it is nearly flat, so 
    /// will the arc. For example, a very large width and height would represent a very large oval, which 
    /// would give a slight curvature for the angle.
    /// </param>
    /// <param name="rotationAngle">
    /// The rotation angle of the oval that specifies the curve. The curvature of the arc can be rotated with 
    /// this parameter.
    /// </param>
    /// <param name="isLargeArc">
    /// true to draw the arc greater than 180 degrees; otherwise, false.
    /// </param>
    /// <param name="sweepDirection">
    /// A value that indicates whether the arc is drawn in the <see cref="SweepDirection.Clockwise"/> or 
    /// <see cref="SweepDirection.Counterclockwise"/> direction.
    /// </param>
    /// <param name="isStroked">
    /// true to make the segment stroked when a <see cref="Pen"/> is used to render the segment; otherwise, 
    /// false.
    /// </param>
    /// <param name="isSmoothJoin">
    /// true to treat the join between this segment and the previous segment as a corner when stroked with a 
    /// <see cref="Pen"/>; otherwise, false.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// An attempt was made to add a segment without starting a figure by calling the <see cref="BeginFigure(Point, bool, bool)"/> 
    /// method.
    /// </exception>
    public abstract void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked, bool isSmoothJoin);

    /// <summary>
    /// Draws a Bezier curve to the specified point.
    /// </summary>
    /// <param name="point1">
    /// The first control point used to specify the shape of the curve.
    /// </param>
    /// <param name="point2">
    /// The second control point used to specify the shape of the curve.
    /// </param>
    /// <param name="point3">
    /// The destination point for the end of the curve.
    /// </param>
    /// <param name="isStroked">
    /// true to make the segment stroked when a <see cref="Pen"/> is used to render the segment; otherwise, 
    /// false.
    /// </param>
    /// <param name="isSmoothJoin">
    /// true to treat the join between this segment and the previous segment as a corner when stroked with a 
    /// <see cref="Pen"/>; otherwise, false.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// An attempt was made to add a segment without starting a figure by calling the <see cref="BeginFigure(Point, bool, bool)"/> 
    /// method.
    /// </exception>
    public abstract void BezierTo(Point point1, Point point2, Point point3, bool isStroked, bool isSmoothJoin);

    /// <summary>
    /// Draws a straight line to the specified <see cref="Point"/>.
    /// </summary>
    /// <param name="point">
    /// The destination point for the end of the line.
    /// </param>
    /// <param name="isStroked">
    /// true to make the segment stroked when a <see cref="Pen"/> is used to render the segment; otherwise, false.
    /// </param>
    /// <param name="isSmoothJoin">
    /// true to treat the join between this segment and the previous segment as a corner when stroked with a 
    /// <see cref="Pen"/>; otherwise, false.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// An attempt was made to add a segment without starting a figure by calling the <see cref="BeginFigure(Point, bool, bool)"/>
    /// method.
    /// </exception>
    public abstract void LineTo(Point point, bool isStroked, bool isSmoothJoin);

    /// <summary>
    /// Draws one or more connected Bezier curves.
    /// </summary>
    /// <param name="points">
    /// The list of points that specify control points and destination points for one or more Bezier curves.
    /// The number of points in this list should be a multiple of three.
    /// </param>
    /// <param name="isStroked">
    /// true to make the segment stroked when a <see cref="Pen"/> is used to render the segment; otherwise, false.
    /// </param>
    /// <param name="isSmoothJoin">
    /// true to treat the join between this segment and the previous segment as a corner when stroked with a 
    /// <see cref="Pen"/>; otherwise, false.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// An attempt was made to add a segment without starting a figure by calling the <see cref="BeginFigure(Point, bool, bool)"/>
    /// method.
    /// </exception>
    public abstract void PolyBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin);

    /// <summary>
    /// Draws one or more connected straight lines.
    /// </summary>
    /// <param name="points">
    /// The collection of points that specify destination points for one or more connected straight lines.
    /// </param>
    /// <param name="isStroked">
    /// true to make the segment stroked when a <see cref="Pen"/> is used to render the segment; otherwise, false.
    /// </param>
    /// <param name="isSmoothJoin">
    /// true to treat the join between this segment and the previous segment as a corner when stroked with a 
    /// <see cref="Pen"/>; otherwise, false.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// An attempt was made to add a segment without starting a figure by calling the <see cref="BeginFigure(Point, bool, bool)"/>
    /// method.
    /// </exception>
    public abstract void PolyLineTo(IList<Point> points, bool isStroked, bool isSmoothJoin);

    /// <summary>
    /// Draws one or more connected quadratic Bezier curves.
    /// </summary>
    /// <param name="points">
    /// The collection of points that specify control points and destination points for one or more quadratic 
    /// Bezier curves. The first point in the list specifies the curve's control point, the next point specifies 
    /// the destination point, the next point specifies the control point of the next curve, and so on. The list 
    /// must contain an even number of points.
    /// </param>
    /// <param name="isStroked">
    /// true to make the segment stroked when a <see cref="Pen"/> is used to render the segment; otherwise, false.
    /// </param>
    /// <param name="isSmoothJoin">
    /// true to treat the join between this segment and the previous segment as a corner when stroked with a 
    /// <see cref="Pen"/>; otherwise, false.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// An attempt was made to add a segment without starting a figure by calling the <see cref="BeginFigure(Point, bool, bool)"/>
    /// method.
    /// </exception>
    public abstract void PolyQuadraticBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin);

    /// <summary>
    /// Draws a quadratic Bezier curve.
    /// </summary>
    /// <param name="point1">
    /// The control point used to specify the shape of the curve.
    /// </param>
    /// <param name="point2">
    /// The destination point for the end of the curve.
    /// </param>
    /// <param name="isStroked">
    /// true to make the segment stroked when a <see cref="Pen"/> is used to render the segment; otherwise, false.
    /// </param>
    /// <param name="isSmoothJoin">
    /// true to treat the join between this segment and the previous segment as a corner when stroked with a 
    /// <see cref="Pen"/>; otherwise, false.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// An attempt was made to add a segment without starting a figure by calling the <see cref="BeginFigure(Point, bool, bool)"/>
    /// method.
    /// </exception>
    public abstract void QuadraticBezierTo(Point point1, Point point2, bool isStroked, bool isSmoothJoin);

    /// <summary>
    /// Closes this context and flushes its content so that it can be rendered.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// This context has already been closed or disposed.
    /// </exception>
    public virtual void Close() => DisposeCore();

    void IDisposable.Dispose()
    {
        VerifyAccess();
        DisposeCore();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// This is the same as the Close call:
    /// Closes the Context and flushes the content.
    /// Afterwards the Context can not be used anymore.
    /// This call does not require all Push calls to have been Popped.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// This call is illegal if this object has already been closed or disposed.
    /// </exception>
    internal virtual void DisposeCore() { }

    /// <summary>
    /// SetClosedState - Sets the current closed state of the figure. 
    /// </summary>
    internal abstract void SetClosedState(bool closed);
}