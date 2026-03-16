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
    public class ScaleTransformTest
    {
        [TestMethod]
        public void Inverse()
        {
            var transform = new ScaleTransform { ScaleX = 10, ScaleY = -2 };
            var invertedTransform = transform.Inverse as MatrixTransform;

            Assert.IsNotNull(invertedTransform);

            var m = invertedTransform.Matrix;

            Assert.AreEqual(0.1, m.M11);
            Assert.AreEqual(0, m.M12);
            Assert.AreEqual(0, m.M21);
            Assert.AreEqual(-0.5, m.M22);
            Assert.AreEqual(0, m.OffsetX);
            Assert.AreEqual(0, m.OffsetY);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(1, 1, 100, 110);
            var transform = new ScaleTransform { ScaleX = -1.5, ScaleY = -2 };
            var result = transform.TransformBounds(rect);
            Assert.AreEqual(-151.5, result.X);
            Assert.AreEqual(-222, result.Y);
            Assert.AreEqual(150, result.Width);
            Assert.AreEqual(220, result.Height);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(-10, 1.5);
            var transform = new ScaleTransform { ScaleX = 100, ScaleY = 2 };
            var result = transform.TryTransform(point, out var outPoint);
            Assert.IsTrue(result);
            Assert.AreEqual(-1000, outPoint.X);
            Assert.AreEqual(3, outPoint.Y);
        }
    }
}
