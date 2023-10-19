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
    public class TransformGroupTest
    {
        [TestMethod]
        public void Inverse_When_Not_Invertible()
        {
            var transform = new TransformGroup();
            transform.Children.Add(new MatrixTransform(MatrixTest.GetSingularMatrix(1, 2)));
            transform.Children.Add(new TranslateTransform() { X = 1, Y = 9 });
            transform.Children.Add(new SkewTransform() { AngleX = 0.5, AngleY = 0.9 });
            var invertedTransform = transform.Inverse;
            invertedTransform.Should().BeNull();
        }

        [TestMethod]
        public void Inverse_When_Invertible()
        {
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform() { X = 1, Y = 9 });
            transform.Children.Add(new ScaleTransform() { ScaleX = -2, ScaleY = 6 });
            var invertedTransform = transform.Inverse as MatrixTransform;
            invertedTransform.Should().NotBeNull();
            var m = invertedTransform.Matrix;
            m.M11.Should().Be(-0.5);
            m.M12.Should().Be(0);
            m.M21.Should().Be(0);
            m.M22.Should().Be(1.0 / 6.0);
            m.OffsetX.Should().Be(-1);
            m.OffsetY.Should().Be(-9);
        }

        [TestMethod]
        public void Inverse_When_Invertible_Nested()
        {
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform() { X = 1, Y = 9 });
            transform.Children.Add(new ScaleTransform() { ScaleX = -2, ScaleY = 6 });
            var nestedTransform = new TransformGroup();
            nestedTransform.Children.Add(new TranslateTransform() { X = -1, Y = -9 });
            nestedTransform.Children.Add(new ScaleTransform() { ScaleX = -0.5, ScaleY = 1.0 / 6.0 });
            transform.Children.Add(nestedTransform);
            var invertedTransform = transform.Inverse as MatrixTransform;
            invertedTransform.Should().NotBeNull();
            var m = invertedTransform.Matrix;
            m.M11.Should().Be(1);
            m.M12.Should().Be(0);
            m.M21.Should().Be(0);
            m.M22.Should().Be(1);
            m.OffsetX.Should().Be(-1.5);
            m.OffsetY.Should().Be(-7.5);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(0, 0, 100, 100);
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform() { X = 1, Y = 9 });
            transform.Children.Add(new MatrixTransform(MatrixTest.GetIncrementalMatrix(3, 1)));
            var result = transform.TransformBounds(rect);
            result.X.Should().Be(55);
            result.Y.Should().Be(66);
            result.Width.Should().Be(800);
            result.Height.Should().Be(1000);
        }

        [TestMethod]
        public void TransformBounds_When_Nested()
        {
            var rect = new Rect(1, 2, 20, 10);
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform() { X = 1, Y = 9 });
            transform.Children.Add(new MatrixTransform(MatrixTest.GetIncrementalMatrix(3, 1)));
            var nestedTransform = new TransformGroup();
            nestedTransform.Children.Add(new TranslateTransform() { X = -1, Y = -9 });
            nestedTransform.Children.Add(new ScaleTransform() { ScaleX = -0.5, ScaleY = 1.0 / 6.0 });
            transform.Children.Add(nestedTransform);
            var result = transform.TransformBounds(rect);
            result.X.Should().Be(-88.5);
            result.Y.Should().BeInRange(12.166666, 12.166667);
            result.Width.Should().Be(55);
            result.Height.Should().BeInRange(23.333333, 23.333334);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(1, 2);
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform() { X = 1, Y = 9 });
            transform.Children.Add(new MatrixTransform(MatrixTest.GetIncrementalMatrix(3, 4)));
            var result = transform.TryTransform(point, out var outPoint);
            result.Should().BeTrue();
            outPoint.X.Should().Be(146);
            outPoint.Y.Should().Be(202);
        }

        [TestMethod]
        public void TryTransform_When_Nested()
        {
            var point = new Point(-1, 3);
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform() { X = 1, Y = 9 });
            transform.Children.Add(new MatrixTransform(MatrixTest.GetIncrementalMatrix(3, 1)));
            var nestedTransform = new TransformGroup();
            nestedTransform.Children.Add(new TranslateTransform() { X = -1, Y = -9 });
            nestedTransform.Children.Add(new RotateTransform() { Angle = 25 });
            transform.Children.Add(nestedTransform);
            var result = transform.TryTransform(point, out var outPoint);
            result.Should().BeTrue();
            outPoint.X.Should().BeInRange(29.810417, 29.810418);
            outPoint.Y.Should().BeInRange(92.240658, 92.240659);
        }
    }
}
