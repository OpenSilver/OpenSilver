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

namespace System.Windows.Media;

public delegate HitTestFilterBehavior HitTestFilterCallback(DependencyObject potentialHitTestTarget);

public delegate HitTestResultBehavior HitTestResultCallback(HitTestResult result);

public enum HitTestFilterBehavior
{
    Continue,
    ContinueSkipChildren,
    ContinueSkipSelf,
    ContinueSkipSelfAndChildren,
    Stop,
}

public enum HitTestResultBehavior
{
    Continue,
    Stop,
}

public enum IntersectionDetail
{
    Empty,
    FullyInside,
    FullyContains,
    Intersects,
    NotCalculated,
}

public abstract class HitTestParameters
{
    internal HitTestParameters() { }
}

public class PointHitTestParameters : HitTestParameters
{
    public PointHitTestParameters(Point point)
    {
        HitPoint = point;
    }

    public Point HitPoint { get; }
}

public class GeometryHitTestParameters : HitTestParameters
{
    public GeometryHitTestParameters(Geometry geometry)
    {
        ArgumentNullException.ThrowIfNull(geometry);
        HitGeometry = geometry;
    }

    public Geometry HitGeometry { get; }
}

public abstract class HitTestResult
{
    internal HitTestResult(DependencyObject visualHit)
    {
        VisualHit = visualHit;
    }

    public DependencyObject VisualHit { get; }
}

public class PointHitTestResult : HitTestResult
{
    public PointHitTestResult(Visual visualHit, Point pointHit)
        : base(visualHit)
    {
        PointHit = pointHit;
    }

    public Point PointHit { get; }

    public new Visual VisualHit => (Visual)base.VisualHit;
}

public class GeometryHitTestResult : HitTestResult
{
    public GeometryHitTestResult(Visual visualHit, IntersectionDetail intersectionDetail)
        : base(visualHit)
    {
        IntersectionDetail = intersectionDetail;
    }

    public IntersectionDetail IntersectionDetail { get; }

    public new Visual VisualHit => (Visual)base.VisualHit;
}
