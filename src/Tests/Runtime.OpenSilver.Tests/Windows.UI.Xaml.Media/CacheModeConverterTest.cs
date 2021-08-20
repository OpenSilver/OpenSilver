using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Media.Tests
#else
namespace Windows.UI.Xaml.Media.Tests
#endif
{
#if WORKINPROGRESS
    [TestClass]
    public class CacheModeConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var cacheModeConverter = new CacheModeConverter();
            var test = cacheModeConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var cacheModeConverter = new CacheModeConverter();
            var test = cacheModeConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var cacheModeConverter = new CacheModeConverter();
            var test = cacheModeConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var cacheModeConverter = new CacheModeConverter();
            var test = cacheModeConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnBitmapCache()
        {
            var cacheModeConverter = new CacheModeConverter();
            var test = cacheModeConverter.ConvertFrom("bitmapcache");
            test.Should().BeOfType(typeof(BitmapCache));
        }

        [TestMethod]
        public void ConvertFrom_InvalidCacheMode_ShouldThrow_Exception()
        {
            var cacheModeConverter = new CacheModeConverter();
            var invalidcacheMode = "invalid cache mode";
            Assert.ThrowsException<Exception>(() => cacheModeConverter.ConvertFrom(invalidcacheMode));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var cacheModeConverter = new CacheModeConverter();
            Assert.ThrowsException<NotSupportedException>(() => cacheModeConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var cacheModeConverter = new CacheModeConverter();
            Assert.ThrowsException<NotSupportedException>(() => cacheModeConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var cacheModeConverter = new CacheModeConverter();
            var test = cacheModeConverter.ConvertTo(new BitmapCache(), typeof(string));
            test.Should().Be("");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var cacheModeConverter = new CacheModeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => cacheModeConverter.ConvertTo(new BitmapCache(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var cacheModeConverter = new CacheModeConverter();
            Assert.ThrowsException<NotSupportedException>(() => cacheModeConverter.ConvertTo(true, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => cacheModeConverter.ConvertTo(new BitmapCache(), typeof(bool)));
        }
    }
#endif
}
