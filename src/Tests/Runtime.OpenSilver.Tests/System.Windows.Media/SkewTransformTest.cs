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
    public class SkewTransformTest
    {
        [TestMethod]
        public void Inverse_When_Not_Invertible()
        {
            // Angle of 45° so that we end up with the a singular matrix
            // ( because tan(45°) = tan(Math.PI / 4) = 1 )
            var transform = new SkewTransform { AngleX = 45, AngleY = 45 };
            var invertedTransform = transform.Inverse;

            Assert.IsNull(invertedTransform);
        }

        [TestMethod]
        public void Inverse_When_Invertible()
        {
            var transform = new SkewTransform { AngleX = 160, AngleY = 20 };
            var invertedTransform = transform.Inverse as MatrixTransform;

            Assert.IsNotNull(invertedTransform);

            var m = invertedTransform.Matrix;

            Assert.IsTrue(m.M11 >= 0.883022 && m.M11 <= 0.883023);
            Assert.IsTrue(m.M12 >= -0.321394 && m.M12 <= - 0.321393);
            Assert.IsTrue(m.M21 >= 0.321393 && m.M21 <= 0.321394);
            Assert.IsTrue(m.M22 >= 0.883022 && m.M22 <= 0.883023);
            Assert.AreEqual(0, m.OffsetX);
            Assert.AreEqual(0, m.OffsetY);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(-5, 5, 100, 100);
            var transform = new SkewTransform { AngleX = 400, AngleY = 20 };
            var result = transform.TransformBounds(rect);

            Assert.IsTrue(result.X >= -0.804502 && result.X <= - 0.804501);
            Assert.IsTrue(result.Y >= 3.180148 && result.Y <= 3.180149);
            Assert.IsTrue(result.Width >= 183.909963 && result.Width <= 183.909964);
            Assert.IsTrue(result.Height >= 136.397023 && result.Height <= 136.397024);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(2, 3);
            var transform = new SkewTransform { AngleX = 20, AngleY = -20 };
            var result = transform.TryTransform(point, out var outPoint);

            Assert.IsTrue(result);
            Assert.IsTrue(outPoint.X >= 3.091910 && outPoint.X <= 3.091911);
            Assert.IsTrue(outPoint.Y >= 2.272059 && outPoint.Y <= 2.272060);
        }
    }
}
