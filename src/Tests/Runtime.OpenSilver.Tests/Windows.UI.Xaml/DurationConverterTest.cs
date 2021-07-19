using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class DurationConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var durationConverter = new DurationConverter();
            var test = durationConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var durationConverter = new DurationConverter();
            var test = durationConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var durationConverter = new DurationConverter();
            var test = durationConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var durationConverter = new DurationConverter();
            var test = durationConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_String_ShouldReturnDuration()
        {
            var durationConverter = new DurationConverter();
            var test = durationConverter.ConvertFrom("1");
            test.Should().Be(new Duration(TimeSpan.FromDays(1)));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_ArgumentNullException()
        {
            var durationConverter = new DurationConverter();
            Assert.ThrowsException<ArgumentNullException>(() => durationConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var durationConverter = new DurationConverter();
            Assert.ThrowsException<NotSupportedException>(() => durationConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String()
        {
            var durationConverter = new DurationConverter();
            var test = durationConverter.ConvertTo(new Duration(TimeSpan.FromDays(3)), typeof(string));
            test.Should().Be("3");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var durationConverter = new DurationConverter();
            Assert.ThrowsException<ArgumentNullException>(() => durationConverter.ConvertTo(null, typeof(string)));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var durationConverter = new DurationConverter();
            Assert.ThrowsException<NotSupportedException>(() => durationConverter.ConvertTo(true, typeof(string)));
            Assert.ThrowsException<NotSupportedException>(() => durationConverter.ConvertTo(new Duration(TimeSpan.FromSeconds(30)), typeof(bool)));
        }
    }
}
