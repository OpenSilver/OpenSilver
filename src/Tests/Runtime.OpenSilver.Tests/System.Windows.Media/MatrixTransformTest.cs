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
    public class MatrixTransformTest
    {
        [TestMethod]
        public void Inverse_When_Not_Invertible()
        {
            var transform = new MatrixTransform(MatrixTest.GetSingularMatrix(2, 4));
            var invertedTransform = transform.Inverse;
            invertedTransform.Should().BeNull();
        }

        [TestMethod]
        public void Inverse_When_Invertible()
        {
            var transform = new MatrixTransform(MatrixTest.GetIncrementalMatrix(0, 1));
            var invertedTransform = transform.Inverse as MatrixTransform;
            invertedTransform.Should().NotBeNull();
            var m = invertedTransform.Matrix;
            m.M11.Should().Be(-1.5);
            m.M12.Should().Be(0.5);
            m.M21.Should().Be(1);
            m.M22.Should().Be(0);
            m.OffsetX.Should().Be(1);
            m.OffsetY.Should().Be(-2);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(-1, 1, 5, 2);
            var transform = new MatrixTransform(MatrixTest.GetIncrementalMatrix(10, 3));
            var result = transform.TransformBounds(rect);
            result.X.Should().Be(28);
            result.Y.Should().Be(31);
            result.Width.Should().Be(82);
            result.Height.Should().Be(103);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(1, 2);
            var transform = new MatrixTransform(MatrixTest.GetIncrementalMatrix(-5, 2));
            var result = transform.TryTransform(point, out var outPoint);
            result.Should().BeTrue();
            outPoint.X.Should().Be(-4);
            outPoint.Y.Should().Be(4);
        }
    }
}
