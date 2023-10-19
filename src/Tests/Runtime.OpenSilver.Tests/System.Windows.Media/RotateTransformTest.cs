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
    public class RotateTransformTest
    {
        [TestMethod]
        public void Inverse()
        {
            var transform = new RotateTransform { Angle = 30 };
            var invertedTransform = transform.Inverse as MatrixTransform;
            invertedTransform.Should().NotBeNull();
            var m = invertedTransform.Matrix;

            m.M11.Should().BeInRange(0.866025, 0.866026);
            m.M12.Should().BeInRange(-0.5, -0.499999);
            m.M21.Should().BeInRange(0.499999, 0.5);
            m.M22.Should().BeInRange(0.866025, 0.866026);
            m.OffsetX.Should().BeInRange(-0.000001, 0.000001);
            m.OffsetY.Should().BeInRange(-0.000001, 0.000001);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(-5, -5, 25, 10);
            var transform = new RotateTransform { Angle = 45 };
            var result = transform.TransformBounds(rect);
            result.X.Should().BeInRange(-7.071068, -7.071067);
            result.Y.Should().BeInRange(-7.071068, -7.071067);
            result.Height.Should().BeInRange(24.748737, 24.748738);
            result.Width.Should().BeInRange(24.748737, 24.748738);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(10, 10);
            var transform = new RotateTransform { Angle = 90 };
            var result = transform.TryTransform(point, out var outPoint);
            result.Should().BeTrue();
            outPoint.X.Should().Be(-10);
            outPoint.Y.Should().Be(10);
        }
    }
}
