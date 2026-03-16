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
    public class RotateTransformTest
    {
        [TestMethod]
        public void Inverse()
        {
            var transform = new RotateTransform { Angle = 30 };
            var invertedTransform = transform.Inverse as MatrixTransform;

            Assert.IsNotNull(invertedTransform);

            var m = invertedTransform.Matrix;

            Assert.IsTrue(m.M11 >= 0.866025 && m.M11 <= 0.866026);
            Assert.IsTrue(m.M12 >= -0.5 && m.M12 <= - 0.499999);
            Assert.IsTrue(m.M21 >= 0.499999 && m.M21 <= 0.5);
            Assert.IsTrue(m.M22 >= 0.866025 && m.M22 <= 0.866026);
            Assert.IsTrue(m.OffsetX >= -0.000001 && m.OffsetX <= 0.000001);
            Assert.IsTrue(m.OffsetY >= -0.000001 && m.OffsetY <= 0.000001);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(-5, -5, 25, 10);
            var transform = new RotateTransform { Angle = 45 };
            var result = transform.TransformBounds(rect);

            Assert.IsTrue(result.X >= -7.071068 && result.X <= - 7.071067);
            Assert.IsTrue(result.Y >= -7.071068 && result.Y <= - 7.071067);
            Assert.IsTrue(result.Height >= 24.748737 && result.Height <= 24.748738);
            Assert.IsTrue(result.Width >= 24.748737 && result.Width <= 24.748738);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(10, 10);
            var transform = new RotateTransform { Angle = 90 };
            var result = transform.TryTransform(point, out var outPoint);

            Assert.IsTrue(result);
            Assert.AreEqual(-10, outPoint.X);
            Assert.AreEqual(10, outPoint.Y);
        }
    }
}
