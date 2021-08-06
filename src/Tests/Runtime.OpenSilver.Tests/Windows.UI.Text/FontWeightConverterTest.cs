using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design.Serialization;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Text.Tests
#endif
{
    [TestClass]
    public class FontWeightConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var fontWeightConverter = new FontWeightConverter();
            var test = fontWeightConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var fontWeightConverter = new FontWeightConverter();
            var test = fontWeightConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var fontWeightConverter = new FontWeightConverter();
            var test = fontWeightConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var fontWeightConverter = new FontWeightConverter();
            var test = fontWeightConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnFontWeight()
        {
            var fontWeightConverter = new FontWeightConverter();
            var test = fontWeightConverter.ConvertFrom("100");
            test.Should().Be(new FontWeight { Weight = 100 });
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var fontWeightConverter = new FontWeightConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontWeightConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var fontWeightConverter = new FontWeightConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontWeightConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertFrom_InvalidFontWeight_ShouldThrow_Exception()
        {
            var fontWeightConverter = new FontWeightConverter();
            var invalidFontWeight = "-5";
            Assert.ThrowsException<Exception>(() => fontWeightConverter.ConvertFrom(invalidFontWeight));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var fontWeightConverter = new FontWeightConverter();
            var fontWeight = new FontWeight { Weight = 100 };
            var test = fontWeightConverter.ConvertTo(fontWeight, typeof(string));
            test.Should().Be("100");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var fontWeightConverter = new FontWeightConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontWeightConverter.ConvertTo(new FontWeight(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var fontWeightConverter = new FontWeightConverter();
            var notSupportedType = typeof(bool);
            Assert.ThrowsException<NotSupportedException>(() => fontWeightConverter.ConvertTo(new FontWeight(), notSupportedType));
        }

        [TestMethod]
        public void ConvertTo_String_FromBool()
        {
            var cursorConverter = new FontWeightConverter();
            var test = cursorConverter.ConvertTo(true, typeof(string));
            test.Should().Be("True");
        }
    }
}
