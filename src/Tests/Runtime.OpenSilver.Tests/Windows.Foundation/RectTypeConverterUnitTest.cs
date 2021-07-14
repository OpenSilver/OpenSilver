using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.Foundation.Tests
#endif
{
    [TestClass]
    public class RectTypeConverterUnitTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var rectConverter = new RectTypeConverter();
            var test = rectConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var rectConverter = new RectTypeConverter();
            var test = rectConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var rectConverter = new RectTypeConverter();
            var test = rectConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var rectConverter = new RectTypeConverter();
            var test = rectConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnRect()
        {
            var rectConverter = new RectTypeConverter();
            var test = rectConverter.ConvertFrom("0,0,100,100");
            test.Should().Be(new Rect(0, 0, 100, 100));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var rectConverter = new RectTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => rectConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var rectConverter = new RectTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => rectConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var rectConverter = new RectTypeConverter();
            var test = rectConverter.ConvertTo(new Rect(0, 0, 100, 100), typeof(string));
            test.Should().Be("0, 0, 100, 100");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var rectConverter = new RectTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => rectConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var rectConverter = new RectTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => rectConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => rectConverter.ConvertTo(new Rect(0, 0, 100, 100), typeof(bool)));
        }
    }
}
