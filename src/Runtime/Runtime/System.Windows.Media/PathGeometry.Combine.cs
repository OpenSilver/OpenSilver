
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

using OpenSilver.Internal.Media;
using OpenSilver.Internal.Media.Geometry.Core;

namespace System.Windows.Media;

public sealed partial class PathGeometry
{
    /// <summary>
    /// Returns the result of a Boolean combination of two Geometry objects.
    /// </summary>
    /// <param name="geometry1">The first Geometry object</param>
    /// <param name="geometry2">The second Geometry object</param>
    /// <param name="mode">The mode in which the objects will be combined</param>
    /// <param name="transform">A transformation to apply to the result, or null</param>
    /// <param name="tolerance">The computational error tolerance</param>
    /// <param name="type">The way the error tolerance will be interpreted - relative or absolute</param>
    internal static PathGeometry InternalCombine(
        Geometry geometry1,
        Geometry geometry2,
        GeometryCombineMode mode,
        Transform transform,
        double tolerance,
        ToleranceType type)
    {
        var ctx = new PathStreamGeometryContext(FillRule.Nonzero, null);

        try
        {
            InternalCombine(
                geometry1,
                geometry2,
                mode,
                Transform.ToMatrix(transform),
                tolerance,
                type,
                ctx);
        }
        catch
        {
            return new PathGeometry();
        }

        return ctx.GetPathGeometry();
    }

    /// <summary>
    /// Performs a Boolean combination and writes the result to the given StreamGeometryContext.
    /// Used by CombinedGeometry.SerializeData to avoid creating an intermediate PathGeometry.
    /// </summary>
    internal static void InternalCombine(
        Geometry geometry1,
        Geometry geometry2,
        GeometryCombineMode mode,
        Matrix outputTransform,
        double tolerance,
        ToleranceType type,
        CapacityStreamGeometryContext context)
    {
        ArgumentNullException.ThrowIfNull(geometry1);
        ArgumentNullException.ThrowIfNull(geometry2);
        ArgumentNullException.ThrowIfNull(context);

        if (tolerance < 0.000001)
        {
            tolerance = 0.000001;
        }

        PathGeometryData data1 = geometry1.GetPathGeometryData();
        PathGeometryData data2 = geometry2.GetPathGeometryData();

        var shape1 = new PathGeometryWrapper(data1.SerializedData, data1.FillRule, data1.Matrix);
        var shape2 = new PathGeometryWrapper(data2.SerializedData, data2.FillRule, data2.Matrix);

        CShapeBase.Combine(
            shape1,
            shape2,
            mode,
            true,
            context,
            outputTransform,
            outputTransform,
            tolerance,
            type == ToleranceType.Relative);
    }
}
