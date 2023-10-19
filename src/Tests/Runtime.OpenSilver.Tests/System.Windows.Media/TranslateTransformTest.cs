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
    public class TranslateTransformTest
    {
        [TestMethod]
        public void Inverse()
        {
            var transform = new TranslateTransform { X = 123, Y = 321 };
            var invertedTransform = transform.Inverse as MatrixTransform;
            invertedTransform.Should().NotBeNull();
            var m = invertedTransform.Matrix;
            m.M11.Should().Be(1);
            m.M12.Should().Be(0);
            m.M21.Should().Be(0);
            m.M22.Should().Be(1);
            m.OffsetX.Should().Be(-123);
            m.OffsetY.Should().Be(-321);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(0, 0, 100, 100);
            var transform = new TranslateTransform { X = 100, Y = 200 };
            var result = transform.TransformBounds(rect);
            result.X.Should().Be(100);
            result.Y.Should().Be(200);
            result.Width.Should().Be(100);
            result.Height.Should().Be(100);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(0, 0);
            var transform = new TranslateTransform { X = 100, Y = 200 };
            var result = transform.TryTransform(point, out var outPoint);
            result.Should().BeTrue();
            outPoint.X.Should().Be(100);
            outPoint.Y.Should().Be(200);
        }
    }
}
