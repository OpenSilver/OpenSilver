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
            invertedTransform.Should().BeNull();
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
            invertedTransform.Should().NotBeNull();
            var m = invertedTransform.Matrix;
            (Matrix.IsZero(m.M11)).Should().BeTrue();
            (Matrix.IsZero(m.M12 - 1)).Should().BeTrue();
            (Matrix.IsZero(m.M21 - 0.5)).Should().BeTrue();
            (Matrix.IsZero(m.M22)).Should().BeTrue();
            (Matrix.IsZero(m.OffsetX - -2.5)).Should().BeTrue();
            (Matrix.IsZero(m.OffsetY - -10)).Should().BeTrue();
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
            result.X.Should().Be(-40);
            result.Y.Should().Be(10);
            result.Width.Should().Be(40);
            result.Height.Should().Be(10);
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
            result.Should().BeTrue();
            outPoint.X.Should().BeInRange(-3.783502, -3.783501);
            outPoint.Y.Should().BeInRange(102.649236, 102.649237);
        }
    }
}
