using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class CornerRadiusTypeConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var cornerRadiusConverter = new CornerRadiusTypeConverter();
            var test = cornerRadiusConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var cornerRadiusConverter = new CornerRadiusTypeConverter();
            var test = cornerRadiusConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var cornerRadiusConverter = new CornerRadiusTypeConverter();
            var test = cornerRadiusConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var cornerRadiusConverter = new CornerRadiusTypeConverter();
            var test = cornerRadiusConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnCornerRadius()
        {
            var cornerRadiusConverter = new CornerRadiusTypeConverter();
            var test = cornerRadiusConverter.ConvertFrom("1,1,1,1");
            test.Should().Be(new CornerRadius(1));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var cornerRadiusConverter = new CornerRadiusTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => cornerRadiusConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var cornerRadiusConverter = new CornerRadiusTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => cornerRadiusConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var cornerRadiusConverter = new CornerRadiusTypeConverter();
            var test = cornerRadiusConverter.ConvertTo(new CornerRadius(1), typeof(string));
            test.Should().Be("1, 1, 1, 1");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var cornerRadiusConverter = new CornerRadiusTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => cornerRadiusConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var cornerRadiusConverter = new CornerRadiusTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => cornerRadiusConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => cornerRadiusConverter.ConvertTo(new CornerRadius(1), typeof(bool)));
        }
    }
}
