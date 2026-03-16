
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
public sealed class PolylineTest
{
    [TestMethod]
    public void Polyline_Points_Should_Use_Factory()
    {
        var polyline = new Polyline();
        var points = polyline.Points;

        Assert.IsNotNull(points);
        Assert.AreEqual(0, points.Count);
        Assert.AreEqual(BaseValueSource.Default, DependencyPropertyHelper.GetValueSource(polyline, Polyline.PointsProperty).BaseValueSource);
    }

    [TestMethod]
    public void Polyline_Points_Should_Be_Promoted_To_Local_Value()
    {
        var polyline = new Polyline();
        polyline.Points.Add(new Point());

        Assert.AreEqual(BaseValueSource.Local, DependencyPropertyHelper.GetValueSource(polyline, Polyline.PointsProperty).BaseValueSource);
    }

    [TestMethod]
    public void Polyline_Points_Should_Not_Reuse_Default_Values_After_Promotion()
    {
        var polyline = new Polyline();

        var points1 = polyline.Points;
        points1.Add(new Point());
        polyline.ClearValue(Polyline.PointsProperty);

        var points2 = polyline.Points;

        Assert.AreEqual(BaseValueSource.Default, DependencyPropertyHelper.GetValueSource(polyline, Polyline.PointsProperty).BaseValueSource);
        Assert.AreNotSame(points1, points2);
    }

    [TestMethod]
    public void Polyline_Points_Should_Reuse_Default_Values_If_Unpromoted()
    {
        var polyline = new Polyline();

        var points1 = polyline.Points;
        polyline.Points = new PointCollection();
        polyline.ClearValue(Polyline.PointsProperty);

        var points2 = polyline.Points;

        Assert.AreEqual(BaseValueSource.Default, DependencyPropertyHelper.GetValueSource(polyline, Polyline.PointsProperty).BaseValueSource);
        Assert.AreSame(points1, points2);
    }
}
