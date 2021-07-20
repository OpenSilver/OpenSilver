using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media.Imaging;

#if MIGRATION
namespace System.Windows.Media.Tests
#else
namespace Windows.UI.Xaml.Media.Tests
#endif
{
    [TestClass]
    public class ImageSourceConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var imageSourceConverter = new ImageSourceConverter();
            var test = imageSourceConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var imageSourceConverter = new ImageSourceConverter();
            var test = imageSourceConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var imageSourceConverter = new ImageSourceConverter();
            var test = imageSourceConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var imageSourceConverter = new ImageSourceConverter();
            var test = imageSourceConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnBitmapImage()
        {
            var imageSourceConverter = new ImageSourceConverter();
            // TODO: Pass valid BitmapImage
            var test = imageSourceConverter.ConvertFrom("");
            throw new NotImplementedException();
            test.Should().BeOfType(typeof(BitmapImage));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var imageSourceConverter = new ImageSourceConverter();
            Assert.ThrowsException<NotSupportedException>(() => imageSourceConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var imageSourceConverter = new ImageSourceConverter();
            Assert.ThrowsException<NotSupportedException>(() => imageSourceConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var imageSourceConverter = new ImageSourceConverter();
            var test = imageSourceConverter.ConvertTo(new BitmapImage(), typeof(string));
            test.Should().Be("");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var imageSourceConverter = new ImageSourceConverter();
            Assert.ThrowsException<ArgumentNullException>(() => imageSourceConverter.ConvertTo(new BitmapCache(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var imageSourceConverter = new ImageSourceConverter();
            Assert.ThrowsException<NotSupportedException>(() => imageSourceConverter.ConvertTo(true, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => imageSourceConverter.ConvertTo(new BitmapCache(), typeof(bool)));
        }
    }
}
