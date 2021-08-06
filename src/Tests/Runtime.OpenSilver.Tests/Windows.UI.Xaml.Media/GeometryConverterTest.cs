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
            var test = geometryConverter.ConvertFrom("M 10,10 20,0 20,10 L 50,30 50,40 20,40");

            test.Should().BeOfType<PathGeometry>();
            
            var pg = (PathGeometry)test;
            pg.Figures.Count.Should().Be(1);
            pg.Figures[0].Segments.Count.Should().Be(1);
            pg.Figures[0].Segments[0].Should().BeOfType<PolyLineSegment>();

            var segments = (PolyLineSegment)pg.Figures[0].Segments[0];
            segments.Points.Count.Should().Be(5);
            segments.Points[0].Should().Be(new Point(20, 0));
            segments.Points[1].Should().Be(new Point(20, 10));
            segments.Points[2].Should().Be(new Point(50, 30));
            segments.Points[3].Should().Be(new Point(50, 40));
            segments.Points[4].Should().Be(new Point(20, 40));
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
            test.Should().Be(new PathGeometry().ToString());
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
