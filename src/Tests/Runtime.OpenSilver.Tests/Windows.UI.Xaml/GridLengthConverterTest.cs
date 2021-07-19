using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class GridLengthConverterTest
    {

        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var gridLengthConverter = new GridLengthConverter();
            var test = gridLengthConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var gridLengthConverter = new GridLengthConverter();
            var test = gridLengthConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var gridLengthConverter = new GridLengthConverter();
            var test = gridLengthConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var gridLengthConverter = new GridLengthConverter();
            var test = gridLengthConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnGridLength()
        {
            var gridLengthConverter = new GridLengthConverter();
            var test = gridLengthConverter.ConvertFrom("100");
            test.Should().Be(new GridLength(100));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var gridLengthConverter = new GridLengthConverter();
            Assert.ThrowsException<ArgumentNullException>(() => gridLengthConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var gridLengthConverter = new GridLengthConverter();
            Assert.ThrowsException<NotSupportedException>(() => gridLengthConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var gridLengthConverter = new GridLengthConverter();
            var test = gridLengthConverter.ConvertTo(new GridLength(100), typeof(string));
            test.Should().Be("100");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var gridLengthConverter = new GridLengthConverter();
            Assert.ThrowsException<ArgumentNullException>(() => gridLengthConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var gridLengthConverter = new GridLengthConverter();
            Assert.ThrowsException<NotSupportedException>(() => gridLengthConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => gridLengthConverter.ConvertTo(new GridLength(100), typeof(bool)));
        }
    }
}
