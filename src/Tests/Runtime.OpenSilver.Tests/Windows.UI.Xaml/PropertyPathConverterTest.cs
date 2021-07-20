using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class PropertyPathConverterTest
    {

        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var propertyPathConverter = new PropertyPathConverter();
            var test = propertyPathConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var propertyPathConverter = new PropertyPathConverter();
            var test = propertyPathConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var propertyPathConverter = new PropertyPathConverter();
            var test = propertyPathConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var propertyPathConverter = new PropertyPathConverter();
            var test = propertyPathConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnPropertyPath()
        {
            var propertyPathConverter = new PropertyPathConverter();
            var test = propertyPathConverter.ConvertFrom("testpath");
            test.Should().Be(new PropertyPath("testpath"));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var propertyPathConverter = new PropertyPathConverter();
            Assert.ThrowsException<NotSupportedException>(() => propertyPathConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var propertyPathConverter = new PropertyPathConverter();
            Assert.ThrowsException<NotSupportedException>(() => propertyPathConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var propertyPathConverter = new PropertyPathConverter();
            var test = propertyPathConverter.ConvertTo(new PropertyPath("testpath"), typeof(string));
            test.Should().Be("0, 0, 100, 100");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var propertyPathConverter = new PropertyPathConverter();
            Assert.ThrowsException<ArgumentNullException>(() => propertyPathConverter.ConvertTo(null, typeof(string)));
            Assert.ThrowsException<ArgumentNullException>(() => propertyPathConverter.ConvertTo(new PropertyPath("testpath"), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentException()
        {
            var propertyPathConverter = new PropertyPathConverter();
            Assert.ThrowsException<ArgumentException>(() => propertyPathConverter.ConvertTo(true, typeof(string)));
        }
    }
}