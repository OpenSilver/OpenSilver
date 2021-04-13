using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var invertedTransform = transform.Inverse as TranslateTransform;

            invertedTransform.Should().NotBeNull();

            invertedTransform.X.Should().Be(-123);
            invertedTransform.Y.Should().Be(-321);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(0, 0, 100, 100);

            var transform = new TranslateTransform { X = 100, Y = 200 };

            var result = transform.TransformBounds(rect);

            result.X.Should().Be(100);
            result.Y.Should().Be(200);
            result.Height.Should().Be(100);
            result.Width.Should().Be(100);
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
