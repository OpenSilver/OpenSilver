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
    public class TranslateTransformTest
    {
        [TestMethod]
        public void Inverse()
        {
            var transform = new TranslateTransform { X = 123, Y = 321 };
            var invertedTransform = transform.Inverse as MatrixTransform;

            Assert.IsNotNull(invertedTransform);

            var m = invertedTransform.Matrix;

            Assert.AreEqual(1, m.M11);
            Assert.AreEqual(0, m.M12);
            Assert.AreEqual(0, m.M21);
            Assert.AreEqual(1, m.M22);
            Assert.AreEqual(-123, m.OffsetX);
            Assert.AreEqual(-321, m.OffsetY);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(0, 0, 100, 100);
            var transform = new TranslateTransform { X = 100, Y = 200 };
            var result = transform.TransformBounds(rect);

            Assert.AreEqual(100, result.X);
            Assert.AreEqual(200, result.Y);
            Assert.AreEqual(100, result.Width);
            Assert.AreEqual(100, result.Height);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(0, 0);
            var transform = new TranslateTransform { X = 100, Y = 200 };
            var result = transform.TryTransform(point, out var outPoint);

            Assert.IsTrue(result);
            Assert.AreEqual(100, outPoint.X);
            Assert.AreEqual(200, outPoint.Y);
        }
    }
}
