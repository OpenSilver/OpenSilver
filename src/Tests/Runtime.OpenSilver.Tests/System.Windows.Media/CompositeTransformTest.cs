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
using OpenSilver.Internal;

namespace System.Windows.Media.Tests
{
    [TestClass]
    public class CompositeTransformTest
    {
        [TestMethod]
        public void Inverse_When_Not_Invertible()
        {
            var transform = new CompositeTransform()
            {
                SkewX = 45,
                SkewY = 45
            };
            var invertedTransform = transform.Inverse;

            Assert.IsNull(invertedTransform);
        }

        [TestMethod]
        public void Inverse_When_Invertible()
        {
            var transform = new CompositeTransform()
            {
                TranslateX = 10,
                TranslateY = 5,
                ScaleX = 2,
                ScaleY = -1,
                SkewX = 180,
                SkewY = 180,
                Rotation = 90,
            };
            var invertedTransform = transform.Inverse as MatrixTransform;

            Assert.IsNotNull(invertedTransform);

            var m = invertedTransform.Matrix;

            Assert.IsTrue(DoubleUtil.IsZero(m.M11));
            Assert.IsTrue(DoubleUtil.IsZero(m.M12 - 1));
            Assert.IsTrue(DoubleUtil.IsZero(m.M21 - 0.5));
            Assert.IsTrue(DoubleUtil.IsZero(m.M22));
            Assert.IsTrue(DoubleUtil.IsZero(m.OffsetX - -2.5));
            Assert.IsTrue(DoubleUtil.IsZero(m.OffsetY - -10));
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(0, 0, 10, 10);
            var transform = new CompositeTransform()
            {
                TranslateX = -10,
                TranslateY = 10,
                ScaleX = 3,
                ScaleY = -1,
                SkewX = 45,
                SkewY = 0,
                Rotation = 180,
            };
            var result = transform.TransformBounds(rect);

            Assert.AreEqual(-40, result.X);
            Assert.AreEqual(10, result.Y);
            Assert.AreEqual(40, result.Width);
            Assert.AreEqual(10, result.Height);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(1, 2);
            var transform = new CompositeTransform()
            {
                TranslateX = 0,
                TranslateY = 100,
                ScaleX = 0,
                ScaleY = 2,
                SkewX = 150,
                SkewY = 30,
                Rotation = 25,
            };
            var result = transform.TryTransform(point, out var outPoint);

            Assert.IsTrue(result);
            Assert.IsTrue(outPoint.X >= -3.783502 && outPoint.X <= -3.783501);
            Assert.IsTrue(outPoint.Y >= 102.649236 && outPoint.Y <= 102.649237);
        }
    }
}
