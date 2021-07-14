using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Media.Tests
#else
namespace Windows.UI.Xaml.Media.Tests
#endif
{
    [TestClass]
    public class CacheModeTypeConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var cacheModeConverter = new CacheModeTypeConverter();
            var test = cacheModeConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var cacheModeConverter = new CacheModeTypeConverter();
            var test = cacheModeConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var cacheModeConverter = new CacheModeTypeConverter();
            var test = cacheModeConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var cacheModeConverter = new CacheModeTypeConverter();
            var test = cacheModeConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnCacheMode()
        {
            var cacheModeConverter = new CacheModeTypeConverter();
            throw new NotImplementedException();
            var test = cacheModeConverter.ConvertFrom("");
            test.Should().Be(new BitmapCache());
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var cacheModeConverter = new CacheModeTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => cacheModeConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var cacheModeConverter = new CacheModeTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => cacheModeConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var cacheModeConverter = new CacheModeTypeConverter();
            var test = cacheModeConverter.ConvertTo(new BitmapCache(), typeof(string));
            throw new NotImplementedException();
            test.Should().Be("");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var cacheModeConverter = new CacheModeTypeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => cacheModeConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var cacheModeConverter = new CacheModeTypeConverter();
            Assert.ThrowsException<NotSupportedException>(() => cacheModeConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => cacheModeConverter.ConvertTo(new BitmapCache(), typeof(bool)));
        }
    }
}
