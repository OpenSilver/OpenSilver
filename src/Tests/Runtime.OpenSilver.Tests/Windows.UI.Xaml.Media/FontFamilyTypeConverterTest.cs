using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Media.Tests
#else
namespace Windows.UI.Xaml.Media.Tests
#endif
{
    [TestClass]
    public class FontFamilyTypeConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var fontFamilyConverter = new FontFamilyTypeConverter();
            var test = fontFamilyConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var fontFamilyConverter = new FontFamilyTypeConverter();
            var test = fontFamilyConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var fontFamilyConverter = new FontFamilyTypeConverter();
            var test = fontFamilyConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var fontFamilyConverter = new FontFamilyTypeConverter();
            var test = fontFamilyConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnFontFamily()
        {
            var fontFamilyConverter = new FontFamilyTypeConverter();
            var test = fontFamilyConverter.ConvertFrom("Verdana");
            test.Should().Be(new FontFamily("Verdana"));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var fontFamilyConverter = new FontFamilyTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontFamilyConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var fontFamilyConverter = new FontFamilyTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontFamilyConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var fontFamilyConverter = new FontFamilyTypeConverter();
            var test = fontFamilyConverter.ConvertTo(new FontFamily("Verdana"), typeof(string));
            test.Should().Be("Verdana");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var fontFamilyConverter = new FontFamilyTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontFamilyConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var fontFamilyConverter = new FontFamilyTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontFamilyConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => fontFamilyConverter.ConvertTo(new FontFamily("Verdana"), typeof(bool)));
        }
    }
}
