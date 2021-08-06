using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Media.Tests
#else
namespace Windows.UI.Xaml.Media.Tests
#endif
{
    [TestClass]
    public class FontFamilyConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var fontFamilyConverter = new FontFamilyConverter();
            var test = fontFamilyConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var fontFamilyConverter = new FontFamilyConverter();
            var test = fontFamilyConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var fontFamilyConverter = new FontFamilyConverter();
            var test = fontFamilyConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var fontFamilyConverter = new FontFamilyConverter();
            var test = fontFamilyConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnFontFamily()
        {
            var fontFamilyConverter = new FontFamilyConverter();
            var test = fontFamilyConverter.ConvertFrom("Verdana");
            test.Should().BeOfType<FontFamily>();
            ((FontFamily)test).Source.Should().Be("Verdana");
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var fontFamilyConverter = new FontFamilyConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontFamilyConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var fontFamilyConverter = new FontFamilyConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontFamilyConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var fontFamilyConverter = new FontFamilyConverter();
            var test = fontFamilyConverter.ConvertTo(new FontFamily("Verdana"), typeof(string));
            test.Should().Be("Verdana");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var fontFamilyConverter = new FontFamilyConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontFamilyConverter.ConvertTo(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => fontFamilyConverter.ConvertTo(new FontFamily("Verdana"), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentException()
        {
            var fontFamilyConverter = new FontFamilyConverter();
            Assert.ThrowsException<ArgumentException>(() => fontFamilyConverter.ConvertTo(true, typeof(string)));
        }
    }
}
