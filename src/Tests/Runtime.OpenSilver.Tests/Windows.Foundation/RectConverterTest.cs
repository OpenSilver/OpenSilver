using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.Foundation.Tests
#endif
{
    [TestClass]
    public class RectConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var rectConverter = new RectConverter();
            var test = rectConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var rectConverter = new RectConverter();
            var test = rectConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var rectConverter = new RectConverter();
            var test = rectConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var rectConverter = new RectConverter();
            var test = rectConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnRect()
        {
            var rectConverter = new RectConverter();
            var test = rectConverter.ConvertFrom("0,0,100,100");
            test.Should().Be(new Rect(0, 0, 100, 100));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var rectConverter = new RectConverter();
            Assert.ThrowsException<NotSupportedException>(() => rectConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var rectConverter = new RectConverter();
            Assert.ThrowsException<NotSupportedException>(() => rectConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertFrom_InvalidRect_ShouldThrow_FormatException()
        {
            var rectConverter = new RectConverter();
            var invalidRect = "1,1,1";
            Assert.ThrowsException<FormatException>(() => rectConverter.ConvertFrom(invalidRect));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var rectConverter = new RectConverter();
            var test = rectConverter.ConvertTo(new Rect(0, 0, 100, 100), typeof(string));
            test.Should().Be("0, 0, 100, 100");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var rectConverter = new RectConverter();
            Assert.ThrowsException<ArgumentNullException>(() => rectConverter.ConvertTo(new Rect(0, 0, 100, 100), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var rectConverter = new RectConverter();
            var notSupportedType = typeof(bool);
            Assert.ThrowsException<NotSupportedException>(() => rectConverter.ConvertTo(new Rect(0, 0, 100, 100), notSupportedType));
        }

        [TestMethod]
        public void ConvertTo_String_FromBool()
        {
            var cursorConverter = new RectConverter();
            var test = cursorConverter.ConvertTo(true, typeof(string));
            test.Should().Be("True");
        }
    }
}
