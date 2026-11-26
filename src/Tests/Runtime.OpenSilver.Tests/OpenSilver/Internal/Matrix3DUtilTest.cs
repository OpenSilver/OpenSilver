
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
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenSilver.Internal.Tests;

[TestClass]
public class Matrix3DUtilTest
{
    // Reference values were measured in Silverlight with single precision and are
    // rounded to 4 decimals.
    private const double Tolerance = 2e-3;

    private static readonly Matrix3D Camera = new(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, -0.001,
        0, 0, 0, 0.999);

    [TestMethod]
    public void FromMatrix_Then_TryGetAffineMatrix_Should_Round_Trip()
    {
        var matrix = new Matrix(1, 2, 3, 4, 5, 6);

        Matrix3D m = Matrix3DUtil.FromMatrix(matrix);

        Assert.IsTrue(m.IsAffine);
        Assert.IsTrue(Matrix3DUtil.TryGetAffineMatrix(m, out Matrix result));
        Assert.AreEqual(matrix, result);
    }

    [TestMethod]
    public void Flatten_Should_Keep_2D_Results()
    {
        var m = new Matrix3D(
            1, 2, 3, 0.001,
            4, 5, 6, 0.002,
            7, 8, 9, 0.003,
            10, 11, 12, 1);

        Matrix3D flat = Matrix3DUtil.Flatten(m);

        AssertMatrix(new Matrix3D(
            1, 2, 0, 0.001,
            4, 5, 0, 0.002,
            0, 0, 1, 0,
            10, 11, 0, 1), flat);
        AssertPoint(Transform(m, new Point(20, 30)), Transform(flat, new Point(20, 30)));
    }

    [TestMethod]
    public void Flatten_Should_Drop_Depth_Between_Levels()
    {
        // A child rotated by -60 in a parent rotated by 60 does not cancel out once
        // each level is flattened.
        Matrix3D parent = Matrix3DUtil.CreateRotationY(Deg(60));
        Matrix3D child = Matrix3DUtil.CreateRotationY(Deg(-60));

        Matrix3D flat = Matrix3DUtil.Flatten(child) * Matrix3DUtil.Flatten(parent);

        AssertMatrix(Matrix3D.Identity, child * parent);
        Assert.AreEqual(25, Transform(flat, new Point(100, 0)).X, 1e-9);
    }

    [TestMethod]
    public void CreateTranslation()
    {
        Point p = Transform(Matrix3DUtil.CreateTranslation(10, 20, 30), new Point(1, 2));

        AssertPoint(new Point(11, 22), p);
    }

    [TestMethod]
    public void CreateScale()
    {
        Point p = Transform(Matrix3DUtil.CreateScale(2, 3, 4), new Point(1, 2));

        AssertPoint(new Point(2, 6), p);
    }

    [TestMethod]
    public void CreateRotationX_Should_Match_Silverlight()
    {
        AssertMatrix(new Matrix3D(
            1, 0, 0, 0,
            0, 0.866, -0.5, 0,
            0, 0.5, 0.866, 0,
            0, 0, 0, 1), Matrix3DUtil.CreateRotationX(Deg(30)));
    }

    [TestMethod]
    public void CreateRotationY_Should_Match_Silverlight()
    {
        AssertMatrix(new Matrix3D(
            0.866, 0, 0.5, 0,
            0, 1, 0, 0,
            -0.5, 0, 0.866, 0,
            0, 0, 0, 1), Matrix3DUtil.CreateRotationY(Deg(30)));
    }

    [TestMethod]
    public void CreateRotationZ_Should_Match_Silverlight()
    {
        AssertMatrix(new Matrix3D(
            0.866, -0.5, 0, 0,
            0.5, 0.866, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1), Matrix3DUtil.CreateRotationZ(Deg(30)));
    }

    [TestMethod]
    public void Rotations_Should_Be_Applied_In_X_Y_Z_Order()
    {
        Matrix3D m = Matrix3DUtil.CreateRotationX(Deg(30))
            * Matrix3DUtil.CreateRotationY(Deg(40))
            * Matrix3DUtil.CreateRotationZ(Deg(50));

        AssertMatrix(new Matrix3D(
            0.4924, -0.5868, 0.6428, 0,
            0.87, 0.3105, -0.383, 0,
            0.0252, 0.7478, 0.6634, 0,
            0, 0, 0, 1), m);
    }

    [TestMethod]
    public void Center_Of_Rotation_Should_Match_Silverlight()
    {
        // RotationY=40, CenterOfRotationX=0 on a 200x100 element: c = (-100, 0, 0).
        Matrix3D m = Matrix3DUtil.CreateTranslation(100, 0, 0)
            * Matrix3DUtil.CreateRotationY(Deg(40))
            * Matrix3DUtil.CreateTranslation(-100, 0, 0);

        Assert.AreEqual(-23.3956, m.OffsetX, Tolerance);
        Assert.AreEqual(0, m.OffsetY, Tolerance);
        Assert.AreEqual(64.2788, m.OffsetZ, Tolerance);
    }

    [TestMethod]
    public void TryGetAffineMatrix_Should_Divide_By_M44()
    {
        var m = new Matrix3D(
            2, 0, 0, 0,
            0, 4, 0, 0,
            0, 0, 1, 0,
            6, 8, 0, 2);

        Assert.IsTrue(Matrix3DUtil.TryGetAffineMatrix(m, out Matrix matrix));
        Assert.AreEqual(new Matrix(1, 0, 0, 2, 3, 4), matrix);
    }

    [TestMethod]
    public void TryGetAffineMatrix_Should_Fail_With_Perspective()
    {
        Assert.IsFalse(Matrix3DUtil.TryGetAffineMatrix(WithPerspective(0.002, 0), out _));
        Assert.IsFalse(Matrix3DUtil.TryGetAffineMatrix(WithPerspective(0, 0.002), out _));
    }

    [TestMethod]
    public void TryGetAffineMatrix_Should_Fail_When_M44_Is_Zero()
    {
        var m = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);

        Assert.IsFalse(Matrix3DUtil.TryGetAffineMatrix(m, out _));
    }

    [TestMethod]
    public void TryGetAffineMatrix_Should_Fail_When_M44_Is_Negative()
    {
        var m = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -1);

        Assert.IsFalse(Matrix3DUtil.TryGetAffineMatrix(m, out _));
    }

    [TestMethod]
    public void TryGetAffineMatrix_Should_Ignore_Z()
    {
        Matrix3D m = Matrix3DUtil.CreateRotationY(Deg(90)) * Matrix3DUtil.CreateTranslation(0, 0, 500);

        Assert.IsTrue(Matrix3DUtil.TryGetAffineMatrix(m, out Matrix matrix));
        Assert.AreEqual(0, matrix.M11, 1e-12);
        Assert.AreEqual(1, matrix.M22, 1e-12);
    }

    [TestMethod]
    public void GetAffineApproximation_Should_Ignore_Perspective()
    {
        Matrix matrix = Matrix3DUtil.GetAffineApproximation(WithPerspective(0.002, 0.004));

        Assert.AreEqual(Matrix.Identity, matrix);
    }

    [TestMethod]
    public void GetAffineApproximation_Should_Divide_By_Negative_M44()
    {
        var m = new Matrix3D(
            2, 0, 0, 0.002,
            0, 4, 0, 0,
            0, 0, 1, 0,
            6, 8, 0, -2);

        Matrix matrix = Matrix3DUtil.GetAffineApproximation(m);

        Assert.AreEqual(new Matrix(-1, 0, 0, -2, -3, -4), matrix);
    }

    [TestMethod]
    public void GetAffineApproximation_Should_Not_Divide_When_M44_Is_Zero()
    {
        var m = new Matrix3D(
            2, 0, 0, 0.002,
            0, 4, 0, 0,
            0, 0, 1, 0,
            6, 8, 0, 0);

        Assert.AreEqual(new Matrix(2, 0, 0, 4, 6, 8), Matrix3DUtil.GetAffineApproximation(m));
    }

    [TestMethod]
    public void Transform_PlaneProjection_RotationX_30()
    {
        AssertCorners(PlaneToCanvas(Matrix3DUtil.CreateRotationX(Deg(30))),
            new Point(147.3306, 155.5428),
            new Point(352.6694, 155.5428),
            new Point(347.6563, 242.2864),
            new Point(152.3438, 242.2864));
    }

    [TestMethod]
    public void Transform_PlaneProjection_RotationY_30()
    {
        AssertCorners(PlaneToCanvas(Matrix3DUtil.CreateRotationY(Deg(30))),
            new Point(167.4428, 152.3356),
            new Point(341.2566, 147.313),
            new Point(341.2566, 252.687),
            new Point(167.4428, 247.6644));
    }

    [TestMethod]
    public void Transform_PlaneProjection_RotationY_60()
    {
        AssertCorners(PlaneToCanvas(Matrix3DUtil.CreateRotationY(Deg(60))),
            new Point(203.9426, 153.9426),
            new Point(304.8007, 145.1993),
            new Point(304.8007, 254.8007),
            new Point(203.9426, 246.0574));
    }

    [TestMethod]
    public void Transform_PlaneProjection_RotationY_180()
    {
        AssertCorners(PlaneToCanvas(Matrix3DUtil.CreateRotationY(Deg(180))),
            new Point(350.1001, 149.95),
            new Point(149.8999, 149.95),
            new Point(149.8999, 250.05),
            new Point(350.1001, 250.05));
    }

    [TestMethod]
    public void Transform_Matrix3DProjection_Perspective()
    {
        AssertPoint(new Point(233.3333, 191.6667), Transform(ToCanvas(WithPerspective(0.002, 0)), new Point(100, 50)));
        AssertPoint(new Point(233.3333, 191.6667), Transform(ToCanvas(WithPerspective(0, 0.004)), new Point(100, 50)));
    }

    [TestMethod]
    public void TryTransform_Should_Succeed_In_Front_Of_Camera()
    {
        Matrix3D m = PlaneToCanvas(Matrix3DUtil.CreateRotationY(Deg(30)));

        Assert.IsTrue(Matrix3DUtil.TryTransform(m, new Point(0, 0), out Point p));
        AssertPoint(new Point(167.4428, 152.3356), p);
    }

    [TestMethod]
    public void TryTransform_Should_Fail_Behind_Camera()
    {
        Matrix3D m = PlaneToCanvas(Matrix3DUtil.CreateTranslation(0, 0, 1500));

        Assert.IsFalse(Matrix3DUtil.TryTransform(m, new Point(0, 0), out _));
    }

    [TestMethod]
    public void TryTransform_Should_Fail_When_M44_Is_Negative()
    {
        Matrix3D m = ToCanvas(new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -1));

        Assert.IsFalse(Matrix3DUtil.TryTransform(m, new Point(200, 0), out _));
    }

    [TestMethod]
    public void TryTransform_Should_Fail_On_Camera_Plane()
    {
        var m = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);

        Assert.IsFalse(Matrix3DUtil.TryTransform(m, new Point(10, 10), out _));
    }

    [TestMethod]
    public void TryTransform_Should_Fail_With_NaN()
    {
        Assert.IsFalse(Matrix3DUtil.TryTransform(Matrix3D.Identity, new Point(double.NaN, 0), out _));
    }

    [TestMethod]
    public void TryTransform_Should_Handle_Partially_Visible_Element()
    {
        // M14=-0.01: w <= 0 for x >= 100.
        Matrix3D m = ToCanvas(WithPerspective(-0.01, 0));

        Assert.IsTrue(Matrix3DUtil.TryTransform(m, new Point(50, 50), out _));
        Assert.IsFalse(Matrix3DUtil.TryTransform(m, new Point(150, 50), out _));
    }

    [TestMethod]
    public void TransformBounds_Affine()
    {
        Matrix3D scale = ToCanvas(new Matrix3D(2, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));
        Matrix3D w2 = ToCanvas(new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 2));

        AssertRect(new Rect(150, 150, 400, 50), Matrix3DUtil.TransformBounds(scale, Element));
        AssertRect(new Rect(150, 150, 100, 50), Matrix3DUtil.TransformBounds(w2, Element));
    }

    [TestMethod]
    public void TransformBounds_Perspective()
    {
        AssertRect(new Rect(150, 150, 142.8571, 100), Matrix3DUtil.TransformBounds(ToCanvas(WithPerspective(0.002, 0)), Element));
        AssertRect(new Rect(150, 150, 200, 71.4286), Matrix3DUtil.TransformBounds(ToCanvas(WithPerspective(0, 0.004)), Element));
    }

    [TestMethod]
    public void TransformBounds_PlaneProjection()
    {
        Matrix3D rz = PlaneToCanvas(Matrix3DUtil.CreateRotationZ(Deg(30)));
        Matrix3D rxyz = PlaneToCanvas(Matrix3DUtil.CreateRotationX(Deg(30))
            * Matrix3DUtil.CreateRotationY(Deg(40))
            * Matrix3DUtil.CreateRotationZ(Deg(50)));

        AssertRect(new Rect(138.2858, 106.6053, 223.4285, 186.7893), Matrix3DUtil.TransformBounds(rz, Element));
        AssertRect(new Rect(161.179, 118.9512, 186.0463, 149.6036), Matrix3DUtil.TransformBounds(rxyz, Element));
    }

    [TestMethod]
    public void TransformBounds_Edge_On()
    {
        Matrix3D m = PlaneToCanvas(Matrix3DUtil.CreateRotationY(Deg(90)));

        AssertRect(new Rect(250, 144.3826, 0, 111.2347), Matrix3DUtil.TransformBounds(m, Element));
    }

    [TestMethod]
    public void TransformBounds_Should_Be_Empty_Behind_Camera()
    {
        Matrix3D behind = PlaneToCanvas(Matrix3DUtil.CreateTranslation(0, 0, 1500));
        Matrix3D negative = ToCanvas(new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -1));

        Assert.IsTrue(Matrix3DUtil.TransformBounds(behind, Element).IsEmpty);
        Assert.IsTrue(Matrix3DUtil.TransformBounds(negative, Element).IsEmpty);
    }

    [TestMethod]
    public void TransformBounds_Should_Clip_At_Near_Plane()
    {
        // M14=-0.01: w goes from 1 at x = 0 to -1 at x = 200. The clipping plane is at
        // w = 0.001, i.e. x = 99.9, where the element is magnified 1000 times.
        Matrix3D m = ToCanvas(WithPerspective(-0.01, 0));

        Rect r = Matrix3DUtil.TransformBounds(m, Element);

        Assert.AreEqual(150, r.X, 1e-6);
        Assert.AreEqual(150, r.Y, 1e-6);
        Assert.AreEqual(99900, r.Width, 1e-6);
        Assert.AreEqual(100000, r.Height, 1e-6);
    }

    [TestMethod]
    public void TransformBounds_Should_Not_Depend_On_Homogeneous_Scale()
    {
        Matrix3D m = ToCanvas(WithPerspective(-0.01, 0));
        Matrix3D scaled = m * new Matrix3D(5, 0, 0, 0, 0, 5, 0, 0, 0, 0, 5, 0, 0, 0, 0, 5);

        AssertRect(Matrix3DUtil.TransformBounds(m, Element), Matrix3DUtil.TransformBounds(scaled, Element));
    }

    [TestMethod]
    public void TransformBounds_Empty()
    {
        Assert.IsTrue(Matrix3DUtil.TransformBounds(WithPerspective(0.002, 0), Rect.Empty).IsEmpty);
    }

    [TestMethod]
    public void TryInvert_Should_Round_Trip()
    {
        Matrix3D m = Matrix3DUtil.Flatten(PlaneToCanvas(Matrix3DUtil.CreateRotationX(Deg(30))
            * Matrix3DUtil.CreateRotationY(Deg(40))
            * Matrix3DUtil.CreateRotationZ(Deg(50))));

        Assert.IsTrue(Matrix3DUtil.TryInvert(m, out Matrix3D inverse));

        foreach (Point p in Corners(Element))
        {
            AssertPoint(p, Transform(inverse, Transform(m, p)), 1e-9);
        }
    }

    [TestMethod]
    public void TryInvert_Should_Fail_When_Singular()
    {
        var m11 = new Matrix3D(0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        Matrix3D edgeOn = Matrix3DUtil.Flatten(PlaneToCanvas(Matrix3DUtil.CreateRotationY(Deg(90))));

        Assert.IsFalse(Matrix3DUtil.TryInvert(m11, out _));
        Assert.IsFalse(Matrix3DUtil.TryInvert(edgeOn, out _));
    }

    [TestMethod]
    public void TryInvert_2D_Should_Match_Matrix()
    {
        var matrix = new Matrix(0.3, 1.7, -2.1, 0.9, 13.7, -4.2);
        Matrix expected = matrix;
        expected.Invert();

        Assert.IsTrue(Matrix3DUtil.TryInvert(Matrix3DUtil.FromMatrix(matrix), out Matrix3D inverse));
        Assert.IsTrue(Matrix3DUtil.TryGetAffineMatrix(inverse, out Matrix actual));
        Assert.AreEqual(expected, actual);
    }

    private static readonly Rect Element = new(0, 0, 200, 100);

    private static double Deg(double degrees) => degrees * Math.PI / 180;

    private static Matrix3D WithPerspective(double m14, double m24) =>
        new(1, 0, 0, m14,
            0, 1, 0, m24,
            0, 0, 1, 0,
            0, 0, 0, 1);

    // Full PlaneProjection matrix of a 200x100 element (see PROJECTION.md, 1.3.3),
    // expressed in the coordinates of a Canvas where the element is at (150, 150).
    private static Matrix3D PlaneToCanvas(Matrix3D mpp) =>
        ToCanvas(Matrix3DUtil.CreateTranslation(-100, -50, 0) * mpp * Camera * Matrix3DUtil.CreateTranslation(100, 50, 0));

    private static Matrix3D ToCanvas(Matrix3D m) => m * Matrix3DUtil.CreateTranslation(150, 150, 0);

    private static Point[] Corners(Rect r) =>
    [
        new Point(r.Left, r.Top),
        new Point(r.Right, r.Top),
        new Point(r.Right, r.Bottom),
        new Point(r.Left, r.Bottom),
    ];

    private static void AssertCorners(Matrix3D m, Point topLeft, Point topRight, Point bottomRight, Point bottomLeft)
    {
        Point[] corners = Corners(Element);
        AssertPoint(topLeft, Transform(m, corners[0]));
        AssertPoint(topRight, Transform(m, corners[1]));
        AssertPoint(bottomRight, Transform(m, corners[2]));
        AssertPoint(bottomLeft, Transform(m, corners[3]));
        AssertPoint(new Point(250, 200), Transform(m, new Point(100, 50)));
    }

    private static void AssertPoint(Point expected, Point actual, double tolerance = Tolerance)
    {
        Assert.AreEqual(expected.X, actual.X, tolerance, $"X (expected {expected}, actual {actual})");
        Assert.AreEqual(expected.Y, actual.Y, tolerance, $"Y (expected {expected}, actual {actual})");
    }

    private static void AssertRect(Rect expected, Rect actual)
    {
        Assert.AreEqual(expected.X, actual.X, Tolerance, $"X (expected {expected}, actual {actual})");
        Assert.AreEqual(expected.Y, actual.Y, Tolerance, $"Y (expected {expected}, actual {actual})");
        Assert.AreEqual(expected.Width, actual.Width, Tolerance, $"Width (expected {expected}, actual {actual})");
        Assert.AreEqual(expected.Height, actual.Height, Tolerance, $"Height (expected {expected}, actual {actual})");
    }

    private static void AssertMatrix(Matrix3D expected, Matrix3D actual)
    {
        double[] e = ToArray(expected);
        double[] a = ToArray(actual);
        for (int i = 0; i < 16; i++)
        {
            Assert.AreEqual(e[i], a[i], Tolerance, $"Element {i / 4 + 1}{i % 4 + 1} (expected {expected}, actual {actual})");
        }
    }

    private static double[] ToArray(Matrix3D m) =>
    [
        m.M11, m.M12, m.M13, m.M14,
        m.M21, m.M22, m.M23, m.M24,
        m.M31, m.M32, m.M33, m.M34,
        m.OffsetX, m.OffsetY, m.OffsetZ, m.M44,
    ];

    private static Point Transform(Matrix3D m, Point point)
    {
        Assert.IsTrue(Matrix3DUtil.TryTransform(m, point, out Point result), $"{point} is not transformed in front of the camera");
        return result;
    }
}
