using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            // TODO: Pass valid geometry
            var test = geometryConverter.ConvertFrom("");
            throw new NotImplementedException();
            test.Should().BeOfType(typeof(Geometry));
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
