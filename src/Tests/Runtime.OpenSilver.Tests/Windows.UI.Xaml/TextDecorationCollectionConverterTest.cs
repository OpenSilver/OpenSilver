using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design.Serialization;
using System.Linq;

#if MIGRATION
namespace System.Windows.Tests
{
    [TestClass]
    public class TextDecorationCollectionConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionConverter();
            var test = textDecorationCollectionConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionConverter();
            var test = textDecorationCollectionConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionConverter();
            var test = textDecorationCollectionConverter.CanConvertTo(typeof(InstanceDescriptor));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionConverter();
            var test = textDecorationCollectionConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnTextDecorationCollection()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionConverter();
            var expected = new TextDecorationCollection { Decoration = new TextDecoration(TextDecorationLocation.Underline) };
            var test = textDecorationCollectionConverter.ConvertFrom("Underline");
            test.Should().Be(expected);
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionConverter();
            Assert.ThrowsException<NotSupportedException>(() => textDecorationCollectionConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionConverter();
            Assert.ThrowsException<NotSupportedException>(() => textDecorationCollectionConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_InvalidOperationException()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionConverter();
            var invalidTextDecoration = "invalid text decoration";
            Assert.ThrowsException<InvalidOperationException>(() => textDecorationCollectionConverter.ConvertFrom(invalidTextDecoration));
        }

        [TestMethod]
        public void ConvertTo_InstanceDescriptor()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionConverter();
            var test = textDecorationCollectionConverter.ConvertTo(Enumerable.Empty<TextDecoration>(), typeof(InstanceDescriptor));
            test.Should().BeOfType(typeof(InstanceDescriptor));
        }

        [TestMethod]
        public void ConvertTo_ShouldThrow_NotSupportedException()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionConverter();
            Assert.ThrowsException<NotSupportedException>(() => textDecorationCollectionConverter.ConvertTo(Enumerable.Empty<TextDecoration>(), typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => textDecorationCollectionConverter.ConvertTo(new TextDecorationCollectionConverter(), typeof(InstanceDescriptor)));
        }
    }
}
#endif