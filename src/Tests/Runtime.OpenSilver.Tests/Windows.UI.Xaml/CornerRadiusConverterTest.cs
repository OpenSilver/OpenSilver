using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design.Serialization;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class CornerRadiusConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var cornerRadiusConverter = new CornerRadiusConverter();
            var test = cornerRadiusConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var cornerRadiusConverter = new CornerRadiusConverter();
            var test = cornerRadiusConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var cornerRadiusConverter = new CornerRadiusConverter();
            var test = cornerRadiusConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var cornerRadiusConverter = new CornerRadiusConverter();
            var test = cornerRadiusConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnCornerRadius()
        {
            var cornerRadiusConverter = new CornerRadiusConverter();
            var test = cornerRadiusConverter.ConvertFrom("1,1,1,1");
            test.Should().Be(new CornerRadius(1));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var cornerRadiusConverter = new CornerRadiusConverter();
            Assert.ThrowsException<ArgumentNullException>(() => cornerRadiusConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var cornerRadiusConverter = new CornerRadiusConverter();
            Assert.ThrowsException<NotSupportedException>(() => cornerRadiusConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var cornerRadiusConverter = new CornerRadiusConverter();
            var test = cornerRadiusConverter.ConvertTo(new CornerRadius(1), typeof(string));
            test.Should().Be("1, 1, 1, 1");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var cornerRadiusConverter = new CornerRadiusConverter();
            Assert.ThrowsException<ArgumentNullException>(() => cornerRadiusConverter.ConvertTo(null, typeof(string)));
            Assert.ThrowsException<ArgumentNullException>(() => cornerRadiusConverter.ConvertTo(true, null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var cornerRadiusConverter = new CornerRadiusConverter();
            Assert.ThrowsException<NotSupportedException>(() => cornerRadiusConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => cornerRadiusConverter.ConvertTo(new CornerRadius(1), typeof(bool)));
        }

        [TestMethod]
        public void ConvertTo_InstanceDescriptor()
        {
            var cursorConverter = new CornerRadiusConverter();
            var cornerRadius = new CornerRadius();
            var test = cursorConverter.ConvertTo(cornerRadius, typeof(InstanceDescriptor));
            test.Should().BeOfType(typeof(InstanceDescriptor));
        }
    }
}
