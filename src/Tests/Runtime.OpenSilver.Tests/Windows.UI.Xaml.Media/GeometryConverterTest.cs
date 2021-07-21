using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Media.Tests
#else
namespace Windows.UI.Xaml.Media.Tests
#endif
{
    [TestClass]
    public class GeometryConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var geometryConverter = new GeometryConverter();
            var test = geometryConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var geometryConverter = new GeometryConverter();
            var test = geometryConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var geometryConverter = new GeometryConverter();
            var test = geometryConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var geometryConverter = new GeometryConverter();
            var test = geometryConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnGeometry()
        {
            var geometryConverter = new GeometryConverter();
            var points = new List<Point> { new Point(20, 0), new Point(20, 10), new Point(50, 30), new Point(50, 40), new Point(20, 40), };
            var lineSegment = new PolyLineSegment { Points = new PointCollection(points) };
            var segments = new PathSegmentCollection { lineSegment };

            var expected = new PathGeometry(new List<PathFigure> { new PathFigure { Segments = segments, StartPoint = new Point(10, 10), } });
            var test = geometryConverter.ConvertFrom("M 10,10 20,0 20,10 L 50,30 50,40 20,40");
            test.Should().Be(expected);
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var geometryConverter = new GeometryConverter();
            Assert.ThrowsException<NotSupportedException>(() => geometryConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var geometryConverter = new GeometryConverter();
            Assert.ThrowsException<NotSupportedException>(() => geometryConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var geometryConverter = new GeometryConverter();
            var test = geometryConverter.ConvertTo(new PathGeometry(), typeof(string));
            test.Should().Be("");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var geometryConverter = new GeometryConverter();
            Assert.ThrowsException<ArgumentNullException>(() => geometryConverter.ConvertTo(new PathGeometry(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var geometryConverter = new GeometryConverter();
            Assert.ThrowsException<NotSupportedException>(() => geometryConverter.ConvertTo(true, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => geometryConverter.ConvertTo(new PathGeometry(), typeof(bool)));
        }
    }
}
