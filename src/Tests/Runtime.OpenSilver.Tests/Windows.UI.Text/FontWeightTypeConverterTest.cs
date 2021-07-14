using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Text.Tests
#endif
{
    [TestClass]
    public class FontWeightTypeConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var fontWeightConverter = new FontWeightTypeConverter();
            var test = fontWeightConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var fontWeightConverter = new FontWeightTypeConverter();
            var test = fontWeightConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var fontWeightConverter = new FontWeightTypeConverter();
            var test = fontWeightConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var fontWeightConverter = new FontWeightTypeConverter();
            var test = fontWeightConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnFontWeight()
        {
            var fontWeightConverter = new FontWeightTypeConverter();
            var test = fontWeightConverter.ConvertFrom("100");
            test.Should().Be(new FontWeight { Weight = 100 });
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var fontWeightConverter = new FontWeightTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontWeightConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var fontWeightConverter = new FontWeightTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontWeightConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var fontWeightConverter = new FontWeightTypeConverter();
            var fontWeight = new FontWeight { Weight = 100 };
            var test = fontWeightConverter.ConvertTo(fontWeight, typeof(string));
            test.Should().Be("100");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var fontWeightConverter = new FontWeightTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontWeightConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var fontWeightConverter = new FontWeightTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontWeightConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => fontWeightConverter.ConvertTo(new FontWeight(), typeof(bool)));
        }
    }
}
