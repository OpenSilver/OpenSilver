using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class FontStretchTypeConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var fontStretchConverter = new FontStretchTypeConverter();
            var test = fontStretchConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var fontStretchConverter = new FontStretchTypeConverter();
            var test = fontStretchConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var fontStretchConverter = new FontStretchTypeConverter();
            var test = fontStretchConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var fontStretchConverter = new FontStretchTypeConverter();
            var test = fontStretchConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnFontStretch()
        {
            var fontStretchConverter = new FontStretchTypeConverter();
            throw new NotImplementedException();
            var test = fontStretchConverter.ConvertFrom("");
            test.Should().Be(new FontStretch());
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var fontStretchConverter = new FontStretchTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontStretchConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var fontStretchConverter = new FontStretchTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontStretchConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var fontStretchConverter = new FontStretchTypeConverter();
            var test = fontStretchConverter.ConvertTo(new FontStretch(), typeof(string));
            throw new NotImplementedException();
            test.Should().Be("");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var fontStretchConverter = new FontStretchTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontStretchConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var fontStretchConverter = new FontStretchTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontStretchConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => fontStretchConverter.ConvertTo(new FontStretch(), typeof(bool)));
        }
    }
}
