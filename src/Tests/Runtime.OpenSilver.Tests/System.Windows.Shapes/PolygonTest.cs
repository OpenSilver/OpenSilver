
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;

namespace System.Windows.Shapes.Tests;

[TestClass]
public sealed class PolygonTest
{
    [TestMethod]
    public void Polygon_Points_Should_Use_Factory()
    {
        var polygon = new Polygon();
        var points = polygon.Points;

        Assert.IsNotNull(points);
        Assert.AreEqual(0, points.Count);
        Assert.AreEqual(BaseValueSource.Default, DependencyPropertyHelper.GetValueSource(polygon, Polygon.PointsProperty).BaseValueSource);
    }

    [TestMethod]
    public void Polygon_Points_Should_Be_Promoted_To_Local_Value()
    {
        var polygon = new Polygon();
        polygon.Points.Add(new Point());

        Assert.AreEqual(BaseValueSource.Local, DependencyPropertyHelper.GetValueSource(polygon, Polygon.PointsProperty).BaseValueSource);
    }

    [TestMethod]
    public void Polygon_Points_Should_Not_Reuse_Default_Values_After_Promotion()
    {
        var polygon = new Polygon();

        var points1 = polygon.Points;
        points1.Add(new Point());
        polygon.ClearValue(Polygon.PointsProperty);

        var points2 = polygon.Points;

        Assert.AreEqual(BaseValueSource.Default, DependencyPropertyHelper.GetValueSource(polygon, Polygon.PointsProperty).BaseValueSource);
        Assert.AreNotSame(points1, points2);
    }

    [TestMethod]
    public void Polygon_Points_Should_Reuse_Default_Values_If_Unpromoted()
    {
        var polygon = new Polygon();

        var points1 = polygon.Points;
        polygon.Points = new PointCollection();
        polygon.ClearValue(Polygon.PointsProperty);

        var points2 = polygon.Points;

        Assert.AreEqual(BaseValueSource.Default, DependencyPropertyHelper.GetValueSource(polygon, Polygon.PointsProperty).BaseValueSource);
        Assert.AreSame(points1, points2);
    }
}
