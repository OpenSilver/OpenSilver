using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Media.Tests
#else
namespace Windows.UI.Xaml.Media.Tests
#endif
{
    [TestClass]
    public class TransformConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var transformConverter = new TransformConverter();
            var test = transformConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var transformConverter = new TransformConverter();
            var test = transformConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var transformConverter = new TransformConverter();
            var test = transformConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var transformConverter = new TransformConverter();
            var test = transformConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnTransform()
        {
            var transformConverter = new TransformConverter();
            var test = transformConverter.ConvertFrom("1,2,3,4,5,6");
            test.Should().BeOfType(typeof(MatrixTransform));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var transformConverter = new TransformConverter();
            Assert.ThrowsException<NotSupportedException>(() => transformConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var transformConverter = new TransformConverter();
            Assert.ThrowsException<NotSupportedException>(() => transformConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var transformConverter = new TransformConverter();
            var test = transformConverter.ConvertTo(new MatrixTransform(), typeof(string));
            test.Should().Be("100");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var cacheModeConverter = new CacheModeConverter();
            Assert.ThrowsException<ArgumentNullException>(() => cacheModeConverter.ConvertTo(new MatrixTransform(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var cacheModeConverter = new CacheModeConverter();
            Assert.ThrowsException<NotSupportedException>(() => cacheModeConverter.ConvertTo(true, typeof(bool)));
            Assert.ThrowsException<NotSupportedException>(() => cacheModeConverter.ConvertTo(new MatrixTransform(), typeof(bool)));
        }
    }
}
