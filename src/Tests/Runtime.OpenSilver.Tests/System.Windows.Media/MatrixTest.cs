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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Media.Tests
{
    [TestClass]
    public class MatrixTest
    {
        #region Identity

        [TestMethod]
        public void Identity()
        {
            Matrix.Identity.IsIdentity.Should().BeTrue();

            var m = new Matrix();

            // ctor returns Matrix.Identity
            m.IsIdentity.Should().BeTrue();
            m.Should().Be(Matrix.Identity);

            // Invert of Identity is Identity
            m.Invert();
            m.Should().Be(Matrix.Identity);

            m.M11.Should().Be(1);
            m.M12.Should().Be(0);
            m.M21.Should().Be(0);
            m.M22.Should().Be(1);
            m.OffsetX.Should().Be(0);
            m.OffsetY.Should().Be(0);
        }

        #endregion Identity

        #region Multiplication

        [TestMethod]
        public void Multiply_When_Any_Translation()
        {
            var left = GetIncrementalMatrix(0, 1);
            var right = GetTranslateMatrix(10, 12);
            var m = Matrix.Multiply(left, right);
            m.Should().Be(new Matrix(0, 1, 
                                     2, 3, 
                                     14, 17));
        }

        [TestMethod]
        public void Multiply_When_Translation_Any()
        {
            var left = GetTranslateMatrix(10, 12);
            var right = GetIncrementalMatrix(0, 1);
            var m = Matrix.Multiply(left, right);
            m.Should().Be(new Matrix(0, 1, 
                                     2, 3, 
                                     28, 51));
        }

        [TestMethod]
        public void Multiply_When_Scaling_Scaling()
        {
            var left = GetScaleMatrix(1.2, 2);
            var right = GetScaleMatrix(-10, 10);
            var m = Matrix.Multiply(left, right);
            m.Should().Be(new Matrix(-12, 0, 
                                     0, 20, 
                                     0, 0));
        }

        [TestMethod]
        public void Multiply_When_Scaling_ScalingTranslation()
        {
            var left = GetScaleMatrix(1.2, 2);
            var right = GetScaleTranslateMatrix(2, 3, 4, 5);
            var m = Matrix.Multiply(left, right);
            m.Should().Be(new Matrix(2.4, 0, 
                                     0, 6, 
                                     4, 5));
        }

        [TestMethod]
        public void Multiply_When_ScalingTranslation_Scaling()
        {
            var left = GetScaleTranslateMatrix(2, 3, 4, 5);
            var right = GetScaleMatrix(1.2, 2);
            var m = Matrix.Multiply(left, right);
            m.Should().Be(new Matrix(2.4, 0, 
                                     0, 6, 
                                     4.8, 10));
        }

        [TestMethod]
        public void Multiply_When_ScalingTranslation_ScalingTranslation()
        {
            var left = GetScaleTranslateMatrix(2, 3, 4, 5);
            var right = GetScaleTranslateMatrix(-1, 10, -2, -3);
            var m = Matrix.Multiply(left, right);
            m.Should().Be(new Matrix(-2, 0, 
                                     0, 30, 
                                     -6, 47));
        }

        [TestMethod]
        public void Multiply_Default()
        {
            /* Possible configurations
             * 
             *   Scaling * Unknown
             *   Scaling | Translation * Unknown
             *   Unknown * Scaling
             *   Unknown * Scaling | Translation
             *   Unknown * Unknown
             */

            var left = GetIncrementalMatrix(0, 1);
            var right = GetIncrementalMatrix(1, 2);
            var m = Matrix.Multiply(left, right);
            m.Should().Be(new Matrix(5, 7,
                                     17, 27,
                                     38, 58));
        }

        #endregion Multiplication

        #region Invert

        [TestMethod]
        public void Invert_When_Invertible()
        {
            var m = GetIncrementalMatrix(0, 1);
            m.HasInverse.Should().BeTrue();
            m.Invert();
            m.Should().Be(new Matrix(-1.5, 0.5, 1, 0, 1, -2));
            
            // make sure the operation is reversible
            m.HasInverse.Should().BeTrue();
            m.Invert();
            m.Should().Be(GetIncrementalMatrix(0, 1));
        }

        [TestMethod]
        public void Invert_When_Not_Invertible()
        {
            var m = GetSingularMatrix(2, 6);
            m.HasInverse.Should().BeFalse();

            Assert.ThrowsException<InvalidOperationException>(() => m.Invert());
            
            m.Should().Be(GetSingularMatrix(2, 6));
        }

        #endregion Invert

        #region MultiplyPoint

        [TestMethod]
        public void MultiplyPoint_When_Identity()
        {
            Point p = new Point(10, 25);
            Matrix m = Matrix.Identity;
            m.MultiplyPoint(ref p._x, ref p._y);
            p.Should().Be(new Point(10, 25));
        }

        [TestMethod]
        public void MultiplyPoint_When_Translation()
        {
            Point p = new Point(10, 25);
            Matrix m = GetTranslateMatrix(-10, 5);
            m.MultiplyPoint(ref p._x, ref p._y);
            p.Should().Be(new Point(0, 30));
        }

        [TestMethod]
        public void MultiplyPoint_When_Scaling()
        {
            Point p = new Point(10, 25);
            Matrix m = GetScaleMatrix(0.8, -1.5);
            m.MultiplyPoint(ref p._x, ref p._y);
            p.Should().Be(new Point(8, -37.5));
        }

        [TestMethod]
        public void MultiplyPoint_When_Translation_And_Scaling()
        {
            Point p = new Point(10, 25);
            Matrix m = GetScaleTranslateMatrix(2, 3, 1, -3);
            m.MultiplyPoint(ref p._x, ref p._y);
            p.Should().Be(new Point(21, 72));
        }

        [TestMethod]
        public void MultiplyPoint_When_Unknown()
        {
            Point p = new Point(10, 25);
            Matrix m = GetIncrementalMatrix(2, 0.5);
            m.MultiplyPoint(ref p._x, ref p._y);
            p.Should().Be(new Point(99, 117));
        }

        #endregion MultiplyPoint

        #region Equal Operator

        [TestMethod]
        public void Equal_Operator_With_DistinguishedIdentity()
        {
            Matrix m = new Matrix();
            m._type.Should().Be(MatrixTypes.TRANSFORM_IS_IDENTITY);
            (m == Matrix.Identity).Should().BeTrue();
            (GetIncrementalMatrix(0, 1) == m).Should().BeFalse();
        }

        [TestMethod]
        public void Equal_Operator_With_Identity()
        {
            Matrix m = new Matrix(1, 0, 1, 1, 0, 0);
            m.M21 = 0;
            m._type.Should().Be(MatrixTypes.TRANSFORM_IS_UNKNOWN);
            m.IsIdentity.Should().BeTrue();
            (Matrix.Identity == m).Should().BeTrue();
            (m == GetIncrementalMatrix(0, 1)).Should().BeFalse();
        }

        [TestMethod]
        public void Equal_Operator()
        {
            (GetIncrementalMatrix(0, 1) == GetIncrementalMatrix(0, 2)).Should().BeFalse();
            (GetIncrementalMatrix(0, 2) == GetIncrementalMatrix(0, 1)).Should().BeFalse();
            (GetIncrementalMatrix(0, 1) == GetIncrementalMatrix(0, 1)).Should().BeTrue();
        }

        #endregion Equal Operator

        #region MatrixUtil.TransformRect

        [TestMethod]
        public void MatrixUtil_TransformRect_With_Rect_Empty()
        {
            var r = Rect.Empty;
            var m = GetIncrementalMatrix(1, 2);
            MatrixUtil.TransformRect(ref r, ref m);
            r.Should().Be(Rect.Empty);
        }

        [TestMethod]
        public void MatrixUtil_TransformRect_With_Matrix_Identity()
        {
            var rInit = new Rect(10, 12, 30, 3);

            var r = rInit;
            var m = Matrix.Identity;
            MatrixUtil.TransformRect(ref r, ref m);
            r.Should().Be(rInit);
        }

        [TestMethod]
        public void MatrixUtil_TransformRect_With_Matrix_Scaling()
        {
            var r = new Rect(10, 12, 30, 3);
            var m = GetScaleMatrix(2, -5);
            MatrixUtil.TransformRect(ref r, ref m);
            r.Should().Be(new Rect(20, -75, 60, 15));
        }

        [TestMethod]
        public void MatrixUtil_TransformRect_With_Matrix_Translation()
        {
            var r = new Rect(10, 12, 30, 3);
            var m = GetTranslateMatrix(-12, -5);
            MatrixUtil.TransformRect(ref r, ref m);
            r.Should().Be(new Rect(-2, 7, 30, 3));
        }

        [TestMethod]
        public void MatrixUtil_TransformRect_With_Matrix_Unknown()
        {
            var r = new Rect(10, 12, 30, 3);
            var m = GetIncrementalMatrix(1, 2);
            MatrixUtil.TransformRect(ref r, ref m);
            r.Should().Be(new Rect(79, 125, 45, 111));
        }

        #endregion MatrixUtil.TransformRect

        #region Helpers

        internal static Matrix GetIncrementalMatrix(double value, double increment)
        {
            return new Matrix(value, value + increment,
                value + 2 * increment, value + 3 * increment,
                value + 4 * increment, value + 5 * increment);
        }

        internal static Matrix GetScaleMatrix(double scaleX, double scaleY)
        {
            return new Matrix(scaleX, 0, 0, scaleY, 0, 0);
        }

        internal static Matrix GetTranslateMatrix(double translateX, double translateY)
        {
            return new Matrix(1, 0, 0, 1, translateX, translateY);
        }

        internal static Matrix GetScaleTranslateMatrix(double scaleX, double scaleY, double translateX, double translateY)
        {
            return new Matrix(scaleX, 0, 0, scaleY, translateX, translateY);
        }

        internal static Matrix GetSingularMatrix(double v1, double v2)
        {
            return new Matrix(v1, v2, v1, v2, 0, 0);
        }

        #endregion Helpers
    }
}
