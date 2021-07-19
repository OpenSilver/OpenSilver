using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.Foundation.Tests
#endif
{
    [TestClass]
    public class SizeConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var sizeConverter = new SizeConverter();
            var test = sizeConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var sizeConverter = new SizeConverter();
            var test = sizeConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var sizeConverter = new SizeConverter();
            var test = sizeConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var sizeConverter = new SizeConverter();
            var test = sizeConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnSize()
        {
            var sizeConverter = new SizeConverter();
            var test = sizeConverter.ConvertFrom("100,100");
            test.Should().Be(new Size(100, 100));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var sizeConverter = new SizeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => sizeConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var sizeConverter = new SizeConverter();
            Assert.ThrowsException<NotSupportedException>(() => sizeConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var sizeConverter = new SizeConverter();
            var test = sizeConverter.ConvertTo(new Size(100, 100), typeof(string));
            test.Should().Be("100, 100");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var sizeConverter = new SizeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => sizeConverter.ConvertTo(new Size(1, 1), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var sizeConverter = new SizeConverter();
            var notSupportedType = typeof(bool);
            Assert.ThrowsException<NotSupportedException>(() => sizeConverter.ConvertTo(new Size(1, 1), notSupportedType));
        }

        [TestMethod]
        public void ConvertTo_String_FromBool()
        {
            var cursorConverter = new SizeConverter();
            var test = cursorConverter.ConvertTo(true, typeof(string));
            test.Should().Be("True");
        }
    }
}
