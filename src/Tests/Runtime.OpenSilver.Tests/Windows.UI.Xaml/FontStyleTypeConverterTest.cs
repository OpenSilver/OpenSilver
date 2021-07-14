using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Tests
{
    [TestClass]
    public class FontStyleTypeConverterTest
    {

        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var fontStyleConverter = new FontStyleTypeConverter();
            var test = fontStyleConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var fontStyleConverter = new FontStyleTypeConverter();
            var test = fontStyleConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var fontStyleConverter = new FontStyleTypeConverter();
            var test = fontStyleConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var fontStyleConverter = new FontStyleTypeConverter();
            var test = fontStyleConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnFontStyle()
        {
            var fontStyleConverter = new FontStyleTypeConverter();
            var test = fontStyleConverter.ConvertFrom("Normal");
            test.Should().Be(new FontStyle(0));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var fontStyleConverter = new FontStyleTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontStyleConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var fontStyleConverter = new FontStyleTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontStyleConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var fontStyleConverter = new FontStyleTypeConverter();
            var test = fontStyleConverter.ConvertTo(new FontStyle(0), typeof(string));
            test.Should().Be("Normal");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var fontStyleConverter = new FontStyleTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontStyleConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var fontStyleConverter = new FontStyleTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontStyleConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => fontStyleConverter.ConvertTo(new FontStyle(0), typeof(bool)));
        }
    }
}
