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
    public class SkewTransformTest
    {
        [TestMethod]
        public void Inverse_When_Not_Invertible()
        {
            // Angle of 45° so that we end up with the a singular matrix
            // ( because tan(45°) = tan(Math.PI / 4) = 1 )
            var transform = new SkewTransform { AngleX = 45, AngleY = 45 };
            var invertedTransform = transform.Inverse;
            invertedTransform.Should().BeNull();
        }

        [TestMethod]
        public void Inverse_When_Invertible()
        {
            var transform = new SkewTransform { AngleX = 160, AngleY = 20 };
            var invertedTransform = transform.Inverse as MatrixTransform;
            invertedTransform.Should().NotBeNull();
            var m = invertedTransform.Matrix;
            m.M11.Should().BeInRange(0.883022, 0.883023);
            m.M12.Should().BeInRange(-0.321394, -0.321393);
            m.M21.Should().BeInRange(0.321393, 0.321394);
            m.M22.Should().BeInRange(0.883022, 0.883023);
            m.OffsetX.Should().Be(0);
            m.OffsetY.Should().Be(0);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(-5, 5, 100, 100);
            var transform = new SkewTransform { AngleX = 400, AngleY = 20 };
            var result = transform.TransformBounds(rect);
            result.X.Should().BeInRange(-0.804502, -0.804501);
            result.Y.Should().BeInRange(3.180148, 3.180149);
            result.Width.Should().BeInRange(183.909963, 183.909964);
            result.Height.Should().BeInRange(136.397023, 136.397024);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(2, 3);
            var transform = new SkewTransform { AngleX = 20, AngleY = -20 };
            var result = transform.TryTransform(point, out var outPoint);
            result.Should().BeTrue();
            outPoint.X.Should().BeInRange(3.091910, 3.091911);
            outPoint.Y.Should().BeInRange(2.272059, 2.272060);
        }
    }
}
