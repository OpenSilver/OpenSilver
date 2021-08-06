using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design.Serialization;

#if MIGRATION
namespace System.Windows.Media.Animation.Tests
#else
namespace Windows.UI.Xaml.Media.Animation.Tests
#endif
{
    [TestClass]
    public class KeyTimeConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var keyTimeConverter = new KeyTimeConverter();
            var test = keyTimeConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var keyTimeConverter = new KeyTimeConverter();
            var test = keyTimeConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var keyTimeConverter = new KeyTimeConverter();
            var test = keyTimeConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var keyTimeConverter = new KeyTimeConverter();
            var test = keyTimeConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnKeyTime()
        {
            var keyTimeConverter = new KeyTimeConverter();
            var test = keyTimeConverter.ConvertFrom("3");
            test.Should().Be(new KeyTime(TimeSpan.FromDays(3)));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var keyTimeConverter = new KeyTimeConverter();
            Assert.ThrowsException<NotSupportedException>(() => keyTimeConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var keyTimeConverter = new KeyTimeConverter();
            Assert.ThrowsException<NotSupportedException>(() => keyTimeConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertFrom_Uniform_ShouldThrow_Exception()
        {
            var keyTimeConverter = new KeyTimeConverter();
            Assert.ThrowsException<Exception>(() => keyTimeConverter.ConvertFrom("Uniform"));
        }

        [TestMethod]
        public void ConvertFrom_Paced_ShouldThrow_Exception()
        {
            var keyTimeConverter = new KeyTimeConverter();
            Assert.ThrowsException<Exception>(() => keyTimeConverter.ConvertFrom("Paced"));
        }

        [TestMethod]
        public void ConvertFrom_Percent_ShouldThrow_Exception()
        {
            var keyTimeConverter = new KeyTimeConverter();
            Assert.ThrowsException<Exception>(() => keyTimeConverter.ConvertFrom("%"));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var keyTimeConverter = new KeyTimeConverter();
            var test = keyTimeConverter.ConvertTo(new KeyTime(TimeSpan.FromDays(3)), typeof(string));
            test.Should().Be("3.00:00:00");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var keyTimeConverter = new KeyTimeConverter();
            Assert.ThrowsException<NotSupportedException>(() => keyTimeConverter.ConvertTo(null, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => keyTimeConverter.ConvertTo(true, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => keyTimeConverter.ConvertTo(new KeyTime(), typeof(bool)));
        }
    }
}
