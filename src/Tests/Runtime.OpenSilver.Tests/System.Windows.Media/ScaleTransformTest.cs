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
    public class ScaleTransformTest
    {
        [TestMethod]
        public void Inverse()
        {
            var transform = new ScaleTransform { ScaleX = 10, ScaleY = -2 };
            var invertedTransform = transform.Inverse as MatrixTransform;
            invertedTransform.Should().NotBeNull();
            var m = invertedTransform.Matrix;
            m.M11.Should().Be(0.1);
            m.M12.Should().Be(0);
            m.M21.Should().Be(0);
            m.M22.Should().Be(-0.5);
            m.OffsetX.Should().Be(0);
            m.OffsetY.Should().Be(0);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(1, 1, 100, 110);
            var transform = new ScaleTransform { ScaleX = -1.5, ScaleY = -2 };
            var result = transform.TransformBounds(rect);
            result.X.Should().Be(-151.5);
            result.Y.Should().Be(-222);
            result.Width.Should().Be(150);
            result.Height.Should().Be(220);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(-10, 1.5);
            var transform = new ScaleTransform { ScaleX = 100, ScaleY = 2 };
            var result = transform.TryTransform(point, out var outPoint);
            result.Should().BeTrue();
            outPoint.X.Should().Be(-1000);
            outPoint.Y.Should().Be(3);
        }
    }
}
