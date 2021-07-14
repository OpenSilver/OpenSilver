using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.Foundation.Tests
#endif
{
    [TestClass]
    public class PointTypeConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var pointConverter = new PointTypeConverter();
            var test = pointConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var pointConverter = new PointTypeConverter();
            var test = pointConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var pointConverter = new PointTypeConverter();
            var test = pointConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var pointConverter = new PointTypeConverter();
            var test = pointConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnPoint()
        {
            var pointConverter = new PointTypeConverter();
            var test = pointConverter.ConvertFrom("1,1");
            test.Should().Be(new Point(1, 1));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var pointConverter = new PointTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => pointConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var pointConverter = new PointTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => pointConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var pointConverter = new PointTypeConverter();
            var test = pointConverter.ConvertTo(new Point(1, 1), typeof(string));
            test.Should().Be("1, 1");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var pointConverter = new PointTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => pointConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var pointConverter = new PointTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => pointConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => pointConverter.ConvertTo(new Point(1, 1), typeof(bool)));
        }
    }
}
