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

            Assert.IsNull(invertedTransform);
        }

        [TestMethod]
        public void Inverse_When_Invertible()
        {
            var transform = new MatrixTransform(MatrixTest.GetIncrementalMatrix(0, 1));
            var invertedTransform = transform.Inverse as MatrixTransform;

            Assert.IsNotNull(invertedTransform);

            var m = invertedTransform.Matrix;

            Assert.AreEqual(-1.5, m.M11);
            Assert.AreEqual(0.5, m.M12);
            Assert.AreEqual(1, m.M21);
            Assert.AreEqual(0, m.M22);
            Assert.AreEqual(1, m.OffsetX);
            Assert.AreEqual(-2, m.OffsetY);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(-1, 1, 5, 2);
            var transform = new MatrixTransform(MatrixTest.GetIncrementalMatrix(10, 3));
            var result = transform.TransformBounds(rect);
            Assert.AreEqual(28, result.X);
            Assert.AreEqual(31, result.Y);
            Assert.AreEqual(82, result.Width);
            Assert.AreEqual(103, result.Height);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(1, 2);
            var transform = new MatrixTransform(MatrixTest.GetIncrementalMatrix(-5, 2));
            var result = transform.TryTransform(point, out var outPoint);
            Assert.IsTrue(result);
            Assert.AreEqual(-4, outPoint.X);
            Assert.AreEqual(4, outPoint.Y);
        }
    }
}
