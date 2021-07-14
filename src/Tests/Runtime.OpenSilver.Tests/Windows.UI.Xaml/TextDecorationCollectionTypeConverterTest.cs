using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Tests
{
    [TestClass]
    public class TextDecorationCollectionTypeConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionTypeConverter();
            var test = textDecorationCollectionConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionTypeConverter();
            var test = textDecorationCollectionConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionTypeConverter();
            var test = textDecorationCollectionConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionTypeConverter();
            var test = textDecorationCollectionConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnTextDecorationCollection()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionTypeConverter();
            var expected = new TextDecorationCollection { Decoration = new TextDecoration(TextDecorationLocation.Underline) };
            var test = textDecorationCollectionConverter.ConvertFrom("Underline");
            test.Should().Be(expected);
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => textDecorationCollectionConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => textDecorationCollectionConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionTypeConverter();
            var textDecoration = new TextDecorationCollection { Decoration = new TextDecoration(TextDecorationLocation.Underline) };
            var test = textDecorationCollectionConverter.ConvertTo(textDecoration, typeof(string));
            test.Should().Be("Underline");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => textDecorationCollectionConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var textDecorationCollectionConverter = new TextDecorationCollectionTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => textDecorationCollectionConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => textDecorationCollectionConverter.ConvertTo(new TextDecorationCollection(), typeof(bool)));
        }
    }
}
