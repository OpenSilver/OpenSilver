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

            Assert.IsNull(invertedTransform);
        }

        [TestMethod]
        public void Inverse_When_Invertible()
        {
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform() { X = 1, Y = 9 });
            transform.Children.Add(new ScaleTransform() { ScaleX = -2, ScaleY = 6 });
            var invertedTransform = transform.Inverse as MatrixTransform;

            Assert.IsNotNull(invertedTransform);

            var m = invertedTransform.Matrix;

            Assert.AreEqual(-0.5, m.M11);
            Assert.AreEqual(0, m.M12);
            Assert.AreEqual(0, m.M21);
            Assert.AreEqual(1.0 / 6.0, m.M22);
            Assert.AreEqual(-1, m.OffsetX);
            Assert.AreEqual(-9, m.OffsetY);
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

            Assert.IsNotNull(invertedTransform);

            var m = invertedTransform.Matrix;

            Assert.AreEqual(1, m.M11);
            Assert.AreEqual(0, m.M12);
            Assert.AreEqual(0, m.M21);
            Assert.AreEqual(1, m.M22);
            Assert.AreEqual(-1.5, m.OffsetX);
            Assert.AreEqual(-7.5, m.OffsetY);
        }

        [TestMethod]
        public void TransformBounds()
        {
            var rect = new Rect(0, 0, 100, 100);
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform() { X = 1, Y = 9 });
            transform.Children.Add(new MatrixTransform(MatrixTest.GetIncrementalMatrix(3, 1)));
            var result = transform.TransformBounds(rect);

            Assert.AreEqual(55, result.X);
            Assert.AreEqual(66, result.Y);
            Assert.AreEqual(800, result.Width);
            Assert.AreEqual(1000, result.Height);
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

            Assert.AreEqual(-88.5, result.X);
            Assert.IsTrue(result.Y >= 12.166666 && result.Y <= 12.166667);
            Assert.AreEqual(55, result.Width);
            Assert.IsTrue(result.Height >= 23.333333 && result.Height <= 23.333334);
        }

        [TestMethod]
        public void TryTransform()
        {
            var point = new Point(1, 2);
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform() { X = 1, Y = 9 });
            transform.Children.Add(new MatrixTransform(MatrixTest.GetIncrementalMatrix(3, 4)));
            var result = transform.TryTransform(point, out var outPoint);

            Assert.IsTrue(result);
            Assert.AreEqual(146, outPoint.X);
            Assert.AreEqual(202, outPoint.Y);
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

            Assert.IsTrue(result);
            Assert.IsTrue(outPoint.X >= 29.810417 && outPoint.X <= 29.810418);
            Assert.IsTrue(outPoint.Y >= 92.240658 && outPoint.Y <= 92.240659);
        }
    }
}
