using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Tests
{
    [TestClass]
    public class FontStyleConverterTest
    {

        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var fontStyleConverter = new FontStyleConverter();
            var test = fontStyleConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var fontStyleConverter = new FontStyleConverter();
            var test = fontStyleConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var fontStyleConverter = new FontStyleConverter();
            var test = fontStyleConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var fontStyleConverter = new FontStyleConverter();
            var test = fontStyleConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnFontStyle()
        {
            var fontStyleConverter = new FontStyleConverter();
            var test = fontStyleConverter.ConvertFrom("Normal");
            test.Should().Be(new FontStyle(0));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var fontStyleConverter = new FontStyleConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontStyleConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var fontStyleConverter = new FontStyleConverter();
            Assert.ThrowsException<NotSupportedException>(() => fontStyleConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_Exception()
        {
            var fontStyleConverter = new FontStyleConverter();
            var invalidFontStyle = "invalid font style";
            Assert.ThrowsException<Exception>(() => fontStyleConverter.ConvertFrom(invalidFontStyle));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var fontStyleConverter = new FontStyleConverter();
            var test = fontStyleConverter.ConvertTo(new FontStyle(0), typeof(string));
            test.Should().Be("Normal");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var fontStyleConverter = new FontStyleConverter();
            Assert.ThrowsException<ArgumentNullException>(() => fontStyleConverter.ConvertTo(new FontStyle(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var fontStyleConverter = new FontStyleConverter();
            var notSupportedType = typeof(bool);
            Assert.ThrowsException<NotSupportedException>(() => fontStyleConverter.ConvertTo(new FontStyle(), notSupportedType));
        }

        [TestMethod]
        public void ConvertTo_String_FromBool()
        {
            var fontStyleConverter = new FontStyleConverter();
            var test = fontStyleConverter.ConvertTo(true, typeof(string));
            test.Should().Be("True");
        }

        [TestMethod]
        public void ConvertTo_InstanceDescriptor()
        {
            var fontStyleConverter = new FontStyleConverter();
            var fontStyle = new FontStyle();
            var test = fontStyleConverter.ConvertTo(fontStyle, typeof(InstanceDescriptor));
            test.Should().BeOfType(typeof(InstanceDescriptor));
        }
    }
}
