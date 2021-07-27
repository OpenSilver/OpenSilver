#if WORKINPROGRESS
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Markup.Tests
{
    [TestClass]
    public class XmlLanguageConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            var test = xmlLanguageConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            var test = xmlLanguageConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            var test = xmlLanguageConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_InstanceDescriptor_ShouldReturnTrue()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            var test = xmlLanguageConverter.CanConvertTo(typeof(InstanceDescriptor));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            var test = xmlLanguageConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnXmlLanguage()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            var test = xmlLanguageConverter.ConvertFrom("en-GB");
            test.Should().Be("");
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            Assert.ThrowsException<NotSupportedException>(() => xmlLanguageConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            Assert.ThrowsException<NotSupportedException>(() => xmlLanguageConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            var test = xmlLanguageConverter.ConvertTo(new XmlLanguage(), typeof(string));
            test.Should().Be("");
        }

        [TestMethod]
        public void ConvertTo_InstanceDescriptor()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            var test = xmlLanguageConverter.ConvertTo(new XmlLanguage(), typeof(InstanceDescriptor));
            test.Should().BeOfType(typeof(InstanceDescriptor));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            Assert.ThrowsException<ArgumentNullException>(() => xmlLanguageConverter.ConvertTo(new XmlLanguage(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var xmlLanguageConverter = new XmlLanguageConverter();
            Assert.ThrowsException<NotSupportedException>(() => xmlLanguageConverter.ConvertTo(true, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => xmlLanguageConverter.ConvertTo(new XmlLanguage(), typeof(bool)));
        }
    }
}
#endif